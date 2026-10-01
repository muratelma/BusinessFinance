import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_submit_button.dart';
import '../data/tax_models.dart';
import 'tax_controller.dart';
import 'tax_parts.dart';
import 'tax_plan_form_page.dart';
import 'tax_schedule.dart';

/// Vergi ekle — tür seçimi (V6).
///
/// İlk kurulumda ([multiple]) hazır türler onay kutusuyla seçilir ve önerilen
/// ritimle tek dokunuşta eklenir; sonraki "+ Ekle" aynı listeyi tek seçimle
/// açar ve tanım formuna gider. Hazır tür yalnız ritmi ve günü önerir, tutar
/// önermez (İ1).
class TaxTypePage extends StatefulWidget {
  const TaxTypePage({
    required this.controller,
    required this.multiple,
    super.key,
  });

  final TaxController controller;
  final bool multiple;

  @override
  State<TaxTypePage> createState() => _TaxTypePageState();
}

class _TaxTypePageState extends State<TaxTypePage> {
  TaxController get controller => widget.controller;

  List<TaxSuggestion>? suggestions;
  ApiException? error;
  final selected = <TaxKind>{};

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => error = null);
    try {
      final loaded = await controller.loadSuggestions();
      if (mounted) setState(() => suggestions = loaded);
    } on ApiException catch (exception) {
      if (mounted) setState(() => error = exception);
    } on FormatException {
      if (mounted) {
        setState(() => error = ApiException.local('response.invalid_format'));
      }
    }
  }

  bool get _noCategory => controller.defaultTaxCategory == null;

  @override
  Widget build(BuildContext context) {
    return TaxPageScaffold(
      title: widget.multiple ? 'Vergilerimi tanımla' : 'Vergi ekle',
      footer: widget.multiple && suggestions != null
          ? ListenableBuilder(
              listenable: controller,
              builder: (context, _) => AppSubmitButton(
                label: '${selected.length} vergiyi ekle',
                icon: Icons.check,
                isBusy: controller.isSubmitting,
                onSubmit: selected.isEmpty || _noCategory ? null : _addAll,
              ),
            )
          : null,
      body: _body(context),
    );
  }

  Widget _body(BuildContext context) {
    if (error?.isUnauthorized ?? false) return const AppUnauthorizedView();
    final items = suggestions;
    if (items == null) {
      return error == null
          ? const AppLoadingView(message: 'Türler yükleniyor')
          : AppErrorView(message: error!.message, onRetry: _load);
    }
    final surfaces = AppSurfaces.of(context);
    return TaxPageBody(
      children: [
        Padding(
          padding: const EdgeInsets.only(bottom: AppSpacing.medium),
          child: Text(
            widget.multiple
                ? 'Ödediklerinizi seçin. Hazır tür yalnız ritmi ve günü önerir; '
                      'tutarı ödediğinizde yazarsınız. Emin değilseniz '
                      'muhasebecinize sorun.'
                : 'Hazır tür yalnız ritmi ve günü önerir; tutarı '
                      'ödediğinizde yazarsınız.',
            style: Theme.of(
              context,
            ).textTheme.bodyMedium?.copyWith(color: surfaces.inkMuted),
          ),
        ),
        if (_noCategory)
          AppInlineNotice(
            margin: const EdgeInsets.only(bottom: AppSpacing.medium),
            message: 'Vergi işaretli bir gider kategoriniz yok.',
            actionLabel: 'Kategorilere git',
            onAction: () => GoRouter.of(context).push('/more/categories'),
          ),
        AppCard(
          padding: EdgeInsets.zero,
          child: AppDividedColumn(
            inset: widget.multiple ? 48 : AppIconCapsule.rowInset,
            children: [
              for (final suggestion in items)
                widget.multiple
                    ? TaxCheckRow(
                        selected: selected.contains(suggestion.taxKind),
                        title: suggestion.taxKind.label,
                        subtitle: _subtitle(suggestion),
                        onChanged: (value) => setState(() {
                          if (value) {
                            selected.add(suggestion.taxKind);
                          } else {
                            selected.remove(suggestion.taxKind);
                          }
                        }),
                      )
                    : AppRow(
                        leading: AppIconCapsule(
                          icon: taxKindIcon(suggestion.taxKind),
                        ),
                        title: suggestion.taxKind.label,
                        subtitle: _subtitle(suggestion),
                        trailing: Icon(
                          Icons.chevron_right,
                          size: 20,
                          color: surfaces.inkMuted,
                        ),
                        onTap: () => _openForm(suggestion),
                      ),
            ],
          ),
        ),
        const SizedBox(height: AppSpacing.small),
        AppCard(
          padding: EdgeInsets.zero,
          child: AppRow(
            leading: const AppIconCapsule(icon: Icons.add),
            title: 'Kendi türüm',
            subtitle: 'Adını ve ritmini siz yazın',
            trailing: Icon(
              Icons.chevron_right,
              size: 20,
              color: surfaces.inkMuted,
            ),
            onTap: () => _openForm(null),
          ),
        ),
      ],
    );
  }

  /// `Her ay · ay sonu · Esnafın çoğu öder`.
  String _subtitle(TaxSuggestion suggestion) {
    final rhythm = TaxSchedule.rhythmLabel(
      rhythm: suggestion.rhythm,
      day: suggestion.dayOfMonth,
      months: suggestion.months,
      startDate: TaxSchedule.iso(_firstDate(suggestion)),
    );
    final hint = suggestion.taxKind.hint;
    return hint == null ? rhythm : '$rhythm · $hint';
  }

  DateTime _firstDate(TaxSuggestion suggestion) => TaxSchedule.candidates(
    rhythm: suggestion.rhythm,
    day: suggestion.dayOfMonth,
    months: suggestion.months.toSet(),
    from: controller.today,
    count: 1,
  ).first;

  Future<void> _addAll() async {
    final category = controller.defaultTaxCategory;
    if (category == null) return;
    final chosen = [
      for (final suggestion in suggestions!)
        if (selected.contains(suggestion.taxKind)) suggestion,
    ];
    final saved = await controller.createPlans([
      for (final suggestion in chosen)
        TaxPlanInput(
          name: suggestion.taxKind.label,
          taxKind: suggestion.taxKind,
          rhythm: suggestion.rhythm,
          months: suggestion.months,
          dayOfMonth: suggestion.dayOfMonth,
          startDate: TaxSchedule.iso(_firstDate(suggestion)),
          categoryId: category.id,
        ),
    ]);
    if (!mounted) return;
    if (saved) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('${chosen.length} vergi eklendi.')),
      );
      Navigator.of(context).pop();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(controller.writeError ?? 'Eklenemedi.')),
      );
    }
  }

  Future<void> _openForm(TaxSuggestion? suggestion) async {
    final saved = await Navigator.of(context).push<bool>(
      MaterialPageRoute(
        builder: (_) =>
            TaxPlanFormPage(controller: controller, suggestion: suggestion),
      ),
    );
    if (saved == true && mounted) Navigator.of(context).pop();
  }
}
