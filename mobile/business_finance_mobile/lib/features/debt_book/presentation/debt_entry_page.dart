import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_category_field.dart';
import '../../../core/widgets/app_filter_sheet.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../accounts/presentation/account_directory.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'debt_book_parts.dart';
import 'debt_entry_form_view.dart';
import 'debt_entry_prefill.dart';
import 'debt_entry_sheet.dart';
import 'debt_entry_submitter.dart';

/// Yeni borç ya da alacak: formun seçeneklerini okur, kişi seçer, takvim
/// önizlemesini ve kaydı sunucuya bağlar. Kaydedilince `true` ile kapanır.
/// Fiş ve dekont yolları [prefill] ile açar.
class DebtEntryPage extends StatefulWidget {
  const DebtEntryPage({
    required this.repository,
    required this.kind,
    super.key,
    this.person,
    this.prefill,
    this.onSaved,
    this.changes,
  });

  final DebtBookRepositoryContract repository;
  final DebtEntryKind kind;

  /// Kişi sayfasından açıldıysa kişi; form onu değiştirmez.
  final DebtPersonChoice? person;

  /// Belgeden gelen öneriler; kişi önerisi kilitlenmez.
  final DebtEntryPrefill? prefill;

  /// Kayıt yazıldıktan sonra (ör. dekonttaki ücreti yazmak); kaydın
  /// sonucunu değiştirmez.
  final Future<void> Function(DebtEntryDraft draft)? onSaved;
  final FinancialDataChanges? changes;

  @override
  State<DebtEntryPage> createState() => _DebtEntryPageState();
}

class _DebtEntryPageState extends State<DebtEntryPage> {
  late Future<DebtEntryOptions> _options = widget.repository.loadEntryOptions();
  late final DebtEntrySubmitter _submitter = DebtEntrySubmitter(
    widget.repository,
    changes: widget.changes,
  );
  List<DebtBookPerson>? _people;

  String get _todayIso {
    final day = DateTime.now();
    return '${day.year.toString().padLeft(4, '0')}-'
        '${day.month.toString().padLeft(2, '0')}-'
        '${day.day.toString().padLeft(2, '0')}';
  }

  bool get _scopeVisible {
    try {
      return context.read<ScopeController?>()?.isVisible ?? false;
    } on ProviderNotFoundException {
      return false;
    }
  }

  Future<DebtPersonChoice?> _pickPerson() async {
    try {
      _people ??= (await widget.repository.load(asOfDate: _todayIso)).people;
    } on Exception {
      _people = const [];
    }
    if (!mounted) return null;
    final trade =
        widget.kind == DebtEntryKind.sale ||
        widget.kind == DebtEntryKind.purchase;
    return AppAdaptiveSheet.show<DebtPersonChoice>(
      context: context,
      builder: (_) => DebtPersonPickerSheet(
        people: _people!,
        scopeVisible: _scopeVisible,
        // Yeni kişinin türünün ön değeri: satış ve alımda işletme, borç
        // parada şahsi (`02-TASARIM.md` §6.4).
        defaultScope: trade
            ? TransactionScope.business
            : TransactionScope.personal,
      ),
    );
  }

  Future<String?> _submit(DebtEntryDraft draft) async {
    final error = await _submitter.submit(draft, asOfDate: _todayIso);
    if (error != null) return error;
    await widget.onSaved?.call(draft);
    if (mounted) Navigator.of(context).pop(true);
    return null;
  }

  @override
  Widget build(BuildContext context) {
    final choices = AccountChoices.of(context);
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: FutureBuilder<DebtEntryOptions>(
          future: _options,
          builder: (context, snapshot) {
            void back() => Navigator.maybePop(context);
            final options = snapshot.data;
            if (options == null) {
              return Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  AppPageHeader(title: widget.kind.formTitle, onBack: back),
                  Expanded(
                    child: snapshot.hasError
                        ? AppErrorView(
                            message: 'Form açılamadı.',
                            onRetry: () => setState(() {
                              _options = widget.repository.loadEntryOptions();
                            }),
                          )
                        : const AppLoadingView(message: 'Form hazırlanıyor'),
                  ),
                ],
              );
            }
            final categories = widget.kind == DebtEntryKind.sale
                ? options.incomeCategories
                : options.expenseCategories;
            final accounts = [
              for (final account in options.sources.accounts)
                choices.account(account.id, account.name, type: account.type),
            ];
            final prefill = widget.prefill;
            final categoryKnown =
                prefill?.categoryId != null &&
                categories.any(
                  (category) => category.id == prefill!.categoryId,
                );
            return DebtEntryFormView(
              kind: widget.kind,
              today: DateTime.now(),
              initialPerson: widget.person ?? prefill?.person,
              initialAmount: prefill?.amount,
              initialDate: prefill?.date,
              initialDueDate: prefill?.dueDate,
              initialDescription: prefill?.description,
              initialCategoryId: categoryKnown ? prefill!.categoryId : null,
              notices: prefill?.notices ?? const [],
              personLocked: widget.person != null,
              categoryOptions: [
                for (final category in categories)
                  AppCategoryOption(
                    id: category.id,
                    name: category.name,
                    scope: category.scope,
                  ),
              ],
              accountOptions: accounts,
              initialAccountId: accounts.firstOrNull?.id,
              onBack: back,
              onPickPerson: widget.person == null ? _pickPerson : null,
              onPreview: (query) async {
                try {
                  return await widget.repository.previewSchedule(query);
                } on Exception {
                  return null;
                }
              },
              onSubmit: _submit,
            );
          },
        ),
      ),
    );
  }
}

/// Kişi seçici: arama, kişiler ve `"…" kişisini ekle`. Kapsam görünen
/// kullanıcıda yeni kişinin türü aynı panelde sorulur. Pasif kişi
/// seçilemez (yeni kayıt yazılmaz).
class DebtPersonPickerSheet extends StatefulWidget {
  const DebtPersonPickerSheet({
    required this.people,
    required this.scopeVisible,
    required this.defaultScope,
    super.key,
  });

  final List<DebtBookPerson> people;
  final bool scopeVisible;
  final TransactionScope defaultScope;

  @override
  State<DebtPersonPickerSheet> createState() => _DebtPersonPickerSheetState();
}

class _DebtPersonPickerSheetState extends State<DebtPersonPickerSheet> {
  final TextEditingController _query = TextEditingController();
  late TransactionScope _scope = widget.defaultScope;
  bool _adding = false;

  @override
  void dispose() {
    _query.dispose();
    super.dispose();
  }

  void _add() {
    final name = _query.text.trim();
    if (!widget.scopeVisible) {
      Navigator.of(context).pop(DebtPersonChoice(name: name));
    } else {
      setState(() => _adding = true);
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final query = _query.text.trim();
    final lower = query.toLowerCase();
    final matches = [
      for (final person in widget.people)
        if (person.isActive &&
            (lower.isEmpty || person.name.toLowerCase().contains(lower)))
          person,
    ];
    final exact = widget.people.any(
      (person) => person.name.toLowerCase() == lower,
    );
    final border = OutlineInputBorder(
      borderRadius: BorderRadius.circular(AppRadius.field),
      borderSide: BorderSide.none,
    );
    return SafeArea(
      child: Padding(
        padding: AppSheetInsets.detail(
          context,
          keyboard: MediaQuery.viewInsetsOf(context).bottom,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              _adding ? 'Yeni kişi' : 'Kişi',
              style: theme.textTheme.titleLarge,
            ),
            const SizedBox(height: AppSpacing.compact),
            if (_adding) ...[
              Text(query, style: theme.textTheme.titleSmall),
              const SizedBox(height: AppGaps.field),
              AppFilterSegments<TransactionScope>(
                values: TransactionScope.values,
                selected: _scope,
                label: (value) => value.label,
                onChanged: (value) => setState(() => _scope = value),
                semanticLabel: 'Kişinin türü',
              ),
              const SizedBox(height: AppGaps.notice),
              const DebtFieldNote(
                'Tür listeyi böler; tutarları ve raporu değiştirmez.',
              ),
              const SizedBox(height: AppGaps.section),
              FilledButton(
                onPressed: () => Navigator.of(
                  context,
                ).pop(DebtPersonChoice(name: query, scope: _scope)),
                child: const Text('Kişiyi ekle'),
              ),
            ] else ...[
              TextField(
                controller: _query,
                autofocus: true,
                onChanged: (_) => setState(() {}),
                style: theme.textTheme.bodyMedium,
                decoration: InputDecoration(
                  isDense: true,
                  filled: true,
                  fillColor: surfaces.cardMuted,
                  hintText: 'Kişi ara ya da yeni ad yaz',
                  prefixIcon: Icon(
                    Icons.search,
                    size: 20,
                    color: surfaces.inkMuted,
                  ),
                  constraints: const BoxConstraints(minHeight: 48),
                  border: border,
                  enabledBorder: border,
                  focusedBorder: border,
                ),
              ),
              const SizedBox(height: AppGaps.block),
              Flexible(
                child: ListView(
                  shrinkWrap: true,
                  children: [
                    if (query.isNotEmpty && !exact)
                      DebtRowFrame(
                        leading: const Icon(Icons.person_add_alt_1_outlined),
                        title: '“$query” kişisini ekle',
                        onTap: _add,
                        semanticLabel: '$query kişisini ekle',
                      ),
                    for (final person in matches)
                      DebtRowFrame(
                        leading: PersonInitials(name: person.name),
                        title: person.name,
                        subtitle: widget.scopeVisible && person.scope != null
                            ? DebtRowSubtitle(parts: [person.scope!.label])
                            : null,
                        onTap: () => Navigator.of(context).pop(
                          DebtPersonChoice(
                            id: person.counterpartyId,
                            name: person.name,
                            scope: person.scope,
                          ),
                        ),
                        semanticLabel: person.name,
                      ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
