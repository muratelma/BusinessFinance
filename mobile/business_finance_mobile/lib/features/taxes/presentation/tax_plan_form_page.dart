import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_month_chips.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../../../core/widgets/app_submit_button.dart';
import '../data/tax_models.dart';
import 'tax_controller.dart';
import 'tax_parts.dart';
import 'tax_schedule.dart';
import 'tax_sheets.dart';

/// Vergi tanımı formu (V7): oluşturma ve düzenleme.
///
/// Sıra: kapsam rayı (en üstte, başlıksız; yalnız kapsamı gören kullanıcıda)
/// → Ad → Ritim → aylar → Gün → Tutar (isteğe bağlı) → Nereden ödenir
/// (isteğe bağlı) → Başlangıç. Başlangıç seçenekleri ritimden türetilir; sunucu
/// başlangıcın ritme uymasını ister.
///
/// Kapsamın varsayılanı profilin tarafıdır ve hesap/kart seçimi onu
/// değiştirmez (ADR 0018 İ9, 30 Eylül 2026): işletme vergisi şahsi kartla da
/// ödenebilir.
class TaxPlanFormPage extends StatefulWidget {
  const TaxPlanFormPage({
    required this.controller,
    super.key,
    this.plan,
    this.suggestion,
    this.lastSettledDate,
  });

  final TaxController controller;

  /// Düzenlenen tanım; boşsa yeni tanım.
  final TaxPlan? plan;

  /// Hazır türden açıldıysa önerisi; ikisi de boşsa "Kendi türüm".
  final TaxSuggestion? suggestion;

  /// Düzenlemede son ödenen ya da kapatılan kalemin günü. Ritim değişirse yeni
  /// başlangıç bundan sonra olmalı.
  final String? lastSettledDate;

  @override
  State<TaxPlanFormPage> createState() => _TaxPlanFormPageState();
}

class _TaxPlanFormPageState extends State<TaxPlanFormPage> {
  TaxController get controller => widget.controller;
  TaxPlan? get plan => widget.plan;
  bool get editing => plan != null;

  late final TaxKind kind =
      plan?.taxKind ?? widget.suggestion?.taxKind ?? TaxKind.custom;
  late final name = TextEditingController(
    text: plan?.name ?? (kind == TaxKind.custom ? '' : kind.label),
  );
  late final amount = TextEditingController(
    text: plan?.amount == null ? '' : taxEditable(plan!.amount!),
  );
  late TaxRhythm rhythm =
      plan?.rhythm ?? widget.suggestion?.rhythm ?? TaxRhythm.selectedMonths;
  late Set<int> months = _initialMonths();
  late int day =
      plan?.effectiveDay ??
      widget.suggestion?.dayOfMonth ??
      TaxSchedule.monthEnd;
  late String? sourceId = plan?.sourceId;
  late String? categoryId =
      plan?.categoryId ?? controller.defaultTaxCategory?.id;
  late TransactionScope scope = plan?.scope ?? TransactionScope.business;
  late String startDate = plan?.startDate ?? _options.first;
  bool showErrors = false;

  Set<int> _initialMonths() {
    final current = plan;
    if (current != null) {
      return current.rhythm == TaxRhythm.yearly
          ? {DateTime.parse(current.startDate).month}
          : current.months.toSet();
    }
    final suggested = widget.suggestion?.months ?? const <int>[];
    return suggested.isEmpty ? {controller.today.month} : suggested.toSet();
  }

  @override
  void dispose() {
    name.dispose();
    amount.dispose();
    super.dispose();
  }

  /// Ritmin alanlarından biri kayıtlı hâlinden farklı mı. Düzenlemede ritim
  /// değişirse bekleyen kalemler yeniden kurulur.
  bool get rhythmChanged =>
      editing && (!_savedRhythm || plan!.startDate != startDate);

  /// Ritim, gün ve aylar kayıtlı hâliyle aynı mı (başlangıç hariç).
  bool get _savedRhythm {
    final current = plan;
    if (current == null) return false;
    final savedMonths = current.rhythm == TaxRhythm.yearly
        ? {DateTime.parse(current.startDate).month}
        : current.months.toSet();
    return current.rhythm == rhythm &&
        current.effectiveDay == day &&
        (!_usesMonths || _sameSet(savedMonths, months));
  }

  bool get _usesMonths =>
      rhythm == TaxRhythm.selectedMonths || rhythm == TaxRhythm.yearly;

  static bool _sameSet(Set<int> a, Set<int> b) =>
      a.length == b.length && a.containsAll(b);

  /// Başlangıcın en erken günü: yeni tanımda bugün, düzenlemede son ödenen
  /// kalemin ertesi günü.
  DateTime get _earliest {
    final settled = widget.lastSettledDate;
    if (settled == null) return controller.today;
    final next = DateTime.parse(settled).add(const Duration(days: 1));
    return editing ? next : controller.today;
  }

  List<String> get _options {
    final dates = TaxSchedule.candidates(
      rhythm: rhythm,
      day: day,
      months: months,
      from: _earliest,
    ).map(TaxSchedule.iso).toList();
    return dates.isEmpty ? [TaxSchedule.iso(controller.today)] : dates;
  }

  /// Ritim alanı değişti: başlangıç yeni ritmin ilk uygun günü olur.
  void _rhythmEdited(VoidCallback change) => setState(() {
    change();
    // Ritim kayıtlı hâline döndüyse kayıtlı başlangıç da geri gelir; aksi
    // hâlde ritimde hiçbir şey değişmediği hâlde plan yeniden kurulurdu.
    if (_savedRhythm) {
      startDate = plan!.startDate;
      return;
    }
    final options = _options;
    if (!options.contains(startDate)) startDate = options.first;
  });

  @override
  Widget build(BuildContext context) {
    final scopeVisible = context.watch<ScopeController?>()?.isVisible ?? false;
    final options = controller.options;
    final categories = options?.taxCategories ?? const <TaxChoice>[];
    final surfaces = AppSurfaces.of(context);
    final startOptions = {
      ..._options,
      // Düzenlemede kayıtlı başlangıç, ritim değişmedikçe seçili kalır.
      if (editing && plan!.startDate == startDate) startDate,
    }.toList()..sort();
    final following = TaxSchedule.following(
      rhythm: rhythm,
      day: day,
      months: months,
      start: DateTime.parse(startDate),
    );
    final settled = widget.lastSettledDate;
    return TaxPageScaffold(
      title: editing ? 'Vergiyi düzenle' : 'Vergi tanımı',
      footer: ListenableBuilder(
        listenable: controller,
        builder: (context, _) => AppSubmitButton(
          label: 'Kaydet',
          icon: Icons.check,
          isBusy: controller.isSubmitting,
          onSubmit: categoryId == null ? null : _submit,
        ),
      ),
      body: TaxPageBody(
        children: [
          if (scopeVisible) ...[
            AppSegmentRail<TransactionScope>(
              values: TransactionScope.values,
              selected: scope,
              semanticLabel: 'Kapsam',
              segmentLabel: (value) => value.label,
              onChanged: (value) => setState(() => scope = value),
              segmentBuilder: (context, value, selected) => Text(
                value.label,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                  color: selected ? surfaces.ink : surfaces.inkMuted,
                ),
              ),
            ),
            const SizedBox(height: AppSpacing.large),
          ],
          TextField(
            controller: name,
            maxLength: 200,
            onChanged: (_) {
              if (showErrors) setState(() {});
            },
            decoration: InputDecoration(
              labelText: 'Ad',
              helperText: kind == TaxKind.custom || editing
                  ? null
                  : 'Türden geldi; değiştirebilirsiniz.',
              errorText: showErrors && name.text.trim().isEmpty
                  ? 'Vergiye bir ad verin.'
                  : null,
            ),
          ),
          const SizedBox(height: AppSpacing.medium),
          DropdownButtonFormField<TaxRhythm>(
            initialValue: rhythm,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Ritim'),
            items: [
              for (final value in TaxRhythm.values)
                DropdownMenuItem(value: value, child: Text(value.label)),
            ],
            onChanged: (value) => _rhythmEdited(() {
              rhythm = value!;
              if (rhythm == TaxRhythm.yearly && months.length != 1) {
                months = {
                  months.isEmpty ? controller.today.month : months.first,
                };
              }
            }),
          ),
          if (_usesMonths) ...[
            const SizedBox(height: AppSpacing.small),
            AppMonthChips(
              selected: months,
              multiple: rhythm == TaxRhythm.selectedMonths,
              onChanged: (value) => _rhythmEdited(() => months = value),
            ),
            if (showErrors && months.isEmpty)
              _error(context, 'En az bir ay seçin.'),
          ],
          const SizedBox(height: AppSpacing.medium),
          DropdownButtonFormField<int>(
            initialValue: day,
            isExpanded: true,
            decoration: InputDecoration(
              labelText: 'Gün',
              helperText: months.isEmpty && _usesMonths
                  ? null
                  : 'Sıradaki: ${DateText.dayMonthYear(TaxSchedule.iso(following[0]))}, '
                        'sonra ${DateText.dayMonthYear(TaxSchedule.iso(following[1]))}',
              helperMaxLines: 2,
            ),
            items: [
              const DropdownMenuItem(
                value: TaxSchedule.monthEnd,
                child: Text('Ay sonu'),
              ),
              for (var value = 1; value < TaxSchedule.monthEnd; value++)
                DropdownMenuItem(
                  value: value,
                  child: Text(TaxSchedule.dayLabel(value)),
                ),
            ],
            onChanged: (value) => _rhythmEdited(() => day = value!),
          ),
          if (rhythmChanged)
            const AppInlineNotice(
              margin: EdgeInsets.only(top: AppSpacing.small),
              message:
                  'Bekleyen kalemler yeni ritme göre yeniden kurulur; '
                  'ödenenler yerinde kalır.',
            ),
          const SizedBox(height: AppSpacing.medium),
          TextField(
            controller: amount,
            onChanged: (_) {
              if (showErrors) setState(() {});
            },
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            inputFormatters: const [TurkishAmountInputFormatter()],
            decoration: InputDecoration(
              labelText: 'Tutar (isteğe bağlı)',
              suffixText: '₺',
              helperText:
                  'Her dönem değişiyorsa boş bırakın; ödediğinizde '
                  'yazarsınız.',
              helperMaxLines: 2,
              errorText: showErrors && _amountInvalid
                  ? 'Sıfırdan büyük bir tutar yazın ya da boş bırakın.'
                  : null,
            ),
          ),
          const SizedBox(height: AppSpacing.medium),
          if (options != null)
            TaxSourceField(
              options: options,
              value: sourceId,
              allowNone: true,
              label: 'Nereden ödenir (isteğe bağlı)',
              helperText: 'Ödediğinizde de seçebilirsiniz.',
              onChanged: (value) => setState(() => sourceId = value),
            ),
          if (categories.length > 1) ...[
            const SizedBox(height: AppSpacing.medium),
            DropdownButtonFormField<String>(
              initialValue: categoryId,
              isExpanded: true,
              decoration: const InputDecoration(labelText: 'Kategori'),
              items: [
                for (final category in categories)
                  DropdownMenuItem(
                    value: category.id,
                    child: Text(category.name),
                  ),
              ],
              onChanged: (value) => setState(() => categoryId = value),
            ),
          ],
          const SizedBox(height: AppSpacing.medium),
          DropdownButtonFormField<String>(
            key: ValueKey('start-$rhythm-$day-${months.join(',')}'),
            initialValue: startDate,
            isExpanded: true,
            decoration: InputDecoration(
              labelText: 'Başlangıç',
              prefixIcon: const Icon(Icons.event_outlined),
              helperText: settled != null && editing
                  ? 'En erken '
                        '${DateText.dayMonth(TaxSchedule.iso(_earliest))} '
                        '(son ödeme ${DateText.dayMonth(settled)})'
                  : 'Varsayılan: sıradaki vade.',
            ),
            items: [
              for (final value in startOptions)
                DropdownMenuItem(
                  value: value,
                  child: Text(DateText.dayMonthYear(value)),
                ),
            ],
            onChanged: (value) => setState(() => startDate = value!),
          ),
          if (categoryId == null)
            AppInlineNotice(
              margin: const EdgeInsets.only(top: AppSpacing.medium),
              message: 'Vergi işaretli bir gider kategoriniz yok.',
              actionLabel: 'Kategorilere git',
              onAction: () => GoRouter.of(context).push('/more/categories'),
            ),
          ListenableBuilder(
            listenable: controller,
            builder: (context, _) => controller.writeError == null
                ? const SizedBox.shrink()
                : _error(context, controller.writeError!),
          ),
        ],
      ),
    );
  }

  bool get _amountInvalid {
    final text = amount.text.trim();
    if (text.isEmpty) return false;
    final wire = MoneyMath.fromInput(text);
    return wire == null || MoneyMath.parse(wire) == BigInt.zero;
  }

  Widget _error(BuildContext context, String message) => Padding(
    padding: const EdgeInsets.only(top: AppSpacing.small),
    child: Semantics(
      liveRegion: true,
      child: Text(
        message,
        style: Theme.of(context).textTheme.bodySmall?.copyWith(
          color: Theme.of(context).colorScheme.error,
        ),
      ),
    ),
  );

  Future<void> _submit() async {
    final category = categoryId;
    if (name.text.trim().isEmpty ||
        (_usesMonths && months.isEmpty) ||
        _amountInvalid ||
        category == null) {
      setState(() => showErrors = true);
      return;
    }
    final card = controller.options?.isCard(sourceId) ?? false;
    final scopeVisible = context.read<ScopeController?>()?.isVisible ?? false;
    final text = amount.text.trim();
    final input = TaxPlanInput(
      name: name.text.trim(),
      taxKind: kind,
      rhythm: rhythm,
      months: rhythm == TaxRhythm.selectedMonths ? months.toList() : const [],
      dayOfMonth: day,
      startDate: startDate,
      categoryId: category,
      amount: text.isEmpty ? null : MoneyMath.fromInput(text),
      accountId: card ? null : sourceId,
      creditCardId: card ? sourceId : null,
      // Kapsamı görmeyen kullanıcıda gönderilmez; sunucu profilin tarafını
      // (şahsi) yazar.
      scope: scopeVisible ? scope : null,
    );
    final saved = editing
        ? await controller.updatePlan(plan!.id, input)
        : await controller.createPlan(input);
    if (saved && mounted) Navigator.of(context).pop(true);
  }
}
