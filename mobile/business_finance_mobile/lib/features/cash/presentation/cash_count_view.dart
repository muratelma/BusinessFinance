import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_input.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_metric_tile.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/cash_repository.dart';
import 'cash_controller.dart';

/// Gün sonu sayımı sekmesi.
///
/// Ekran iki sayıyı yan yana koyar: uygulamanın beklediği ve elde sayılan.
/// Fark **sunucudan gelir**; istemci çıkarma yapmaz.
class CashCountView extends StatefulWidget {
  const CashCountView({
    required this.controller,
    super.key,
    this.scopeController,
  });

  final CashCountController controller;
  final ScopeController? scopeController;

  @override
  State<CashCountView> createState() => _CashCountViewState();
}

class _CashCountViewState extends State<CashCountView> {
  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_changed);
    widget.controller.load();
  }

  @override
  void dispose() {
    widget.controller.removeListener(_changed);
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final controller = widget.controller;
    if (controller.isLoading && controller.today == null) {
      return const AppLoadingView(message: 'Kasa yükleniyor');
    }
    if (controller.unauthorized && controller.today == null) {
      return const AppUnauthorizedView();
    }
    if (controller.errorMessage != null && controller.today == null) {
      if (!controller.hasCashAccount && !controller.isLoading) {
        return _noCashAccount();
      }
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }
    if (!controller.hasCashAccount) return _noCashAccount();

    final today = controller.today;
    if (today == null) {
      return AppErrorView(message: 'Kasa okunamadı.', onRetry: controller.load);
    }

    return RefreshIndicator(
      onRefresh: controller.load,
      child: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          if (controller.isStale)
            AppInlineNotice(
              message:
                  '${controller.errorMessage} Son bilinen sayım gösteriliyor.',
              actionLabel: 'Yenile',
              onAction: controller.load,
            ),
          if (controller.accounts.length > 1) ...[
            _accountPicker(controller),
            const SizedBox(height: AppSpacing.medium),
          ],
          _todayCard(controller, today),
          const SizedBox(height: AppSpacing.large),
          const AppSectionHeader(title: 'Geçmiş sayımlar'),
          ..._history(controller),
        ],
      ),
    );
  }

  Widget _noCashAccount() => const AppEmptyView(
    title: 'Sayılacak bir kasa yok.',
    message:
        'Gün sonu sayımı yalnız nakit hesaplar içindir; banka bakiyesi elle '
        'sayılmaz. Önce bir nakit hesap açın.',
    icon: Icons.point_of_sale_outlined,
  );

  Widget _accountPicker(CashCountController controller) {
    return DropdownButtonFormField<String>(
      initialValue: controller.selectedAccountId,
      isExpanded: true,
      decoration: const InputDecoration(labelText: 'Kasa'),
      items: [
        for (final account in controller.accounts)
          DropdownMenuItem(value: account.id, child: Text(account.name)),
      ],
      onChanged: (value) {
        if (value != null) controller.selectAccount(value);
      },
    );
  }

  Widget _todayCard(CashCountController controller, CashCountToday today) {
    final count = controller.todayCount;
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          AppMetricTile(
            label: 'Uygulamaya göre',
            amount: today.expectedBalance,
            currency: today.currency,
            caption:
                'Bugün ${DateText.dayMonth(CashCountController.todayDate())}',
            size: AppMetricSize.hero,
          ),
          const SizedBox(height: AppSpacing.medium),
          if (count == null)
            Text(
              'Bugün henüz sayım yapılmadı. Sayım bir gözlemdir: yazmak hiçbir '
              'bakiyeyi değiştirmez.',
              style: Theme.of(context).textTheme.bodyMedium,
            )
          else ...[
            AppMetricTile(
              label: 'Elde sayılan',
              amount: count.countedAmount,
              currency: count.currency,
            ),
            const SizedBox(height: AppSpacing.small),
            _differenceRow(count),
          ],
          const SizedBox(height: AppSpacing.medium),
          Wrap(
            spacing: AppSpacing.small,
            runSpacing: AppSpacing.small,
            children: [
              FilledButton.tonalIcon(
                onPressed: controller.isSubmitting ? null : _openCountForm,
                icon: const Icon(Icons.calculate_outlined),
                label: Text(count == null ? 'Sayımı gir' : 'Yeniden say'),
              ),
              if (controller.hasOpenDifference)
                FilledButton.icon(
                  onPressed: controller.isSubmitting
                      ? null
                      : _openDifferenceForm,
                  icon: const Icon(Icons.playlist_add_check),
                  label: const Text('Farkı kaydet'),
                ),
            ],
          ),
          if (controller.hasOpenDifference) ...[
            const SizedBox(height: AppSpacing.small),
            Text(
              'Fark kendiliğinden yazılmaz. Kaydederseniz tek bir '
              '${controller.differenceCategoryType == 'income' ? 'gelir' : 'gider'} '
              'kaydı oluşur ve kasa sayılan tutara oturur.',
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ],
        ],
      ),
    );
  }

  Widget _differenceRow(CashCountItem count) {
    final difference = count.difference;
    if (difference == null) return const SizedBox.shrink();
    final sign = _moneySign(difference);
    final balanced = sign == 0;
    return Row(
      children: [
        AppStatusChip(
          label: count.isAdjusted
              ? 'Fark kaydedildi'
              : balanced
              ? 'Sayım tuttu'
              : sign > 0
              ? 'Fazla'
              : 'Eksik',
          icon: balanced ? Icons.check_circle_outline : Icons.difference,
          tone: balanced
              ? AppStatusTone.neutral
              : sign > 0
              ? AppStatusTone.income
              : AppStatusTone.expense,
        ),
        const SizedBox(width: AppSpacing.small),
        AppMoneyText(
          amount: difference,
          currency: count.currency,
          signed: true,
          effect: balanced
              ? AppMoneyEffect.neutral
              : sign > 0
              ? AppMoneyEffect.income
              : AppMoneyEffect.expense,
        ),
      ],
    );
  }

  List<Widget> _history(CashCountController controller) {
    if (controller.history.isEmpty) {
      return const [
        AppEmptyView(
          title: 'Geçmiş sayım yok.',
          message: 'Yaptığınız her sayım tarihiyle burada kalır.',
          icon: Icons.history,
        ),
      ];
    }
    return [
      for (final item in controller.history)
        Padding(
          padding: const EdgeInsets.only(bottom: AppSpacing.small),
          child: AppCard(
            padding: EdgeInsets.zero,
            child: AppListRow(
              icon: Icons.calculate_outlined,
              title: DateText.dayMonth(item.countDate),
              subtitle: item.isCancelled
                  ? 'Yerine yeni sayım yapıldı'
                  : item.isAdjusted
                  ? 'Farkı kaydedildi'
                  : item.note ?? 'Sayım',
              trailing: AppMoneyText(
                amount: item.countedAmount,
                currency: item.currency,
                isCancelled: item.isCancelled,
              ),
              dimmed: item.isCancelled,
            ),
          ),
        ),
    ];
  }

  Future<void> _openCountForm() async {
    await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => _CountForm(
        controller: widget.controller,
        scopeController: widget.scopeController,
      ),
    );
  }

  Future<void> _openDifferenceForm() async {
    await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => _DifferenceForm(controller: widget.controller),
    );
  }
}

int _moneySign(String value) {
  final normalized = value.trim();
  final digits = normalized.replaceAll(RegExp('[^0-9]'), '');
  if (digits.isEmpty || !digits.contains(RegExp('[1-9]'))) return 0;
  return normalized.startsWith('-') ? -1 : 1;
}

class _CountForm extends StatefulWidget {
  const _CountForm({required this.controller, this.scopeController});

  final CashCountController controller;
  final ScopeController? scopeController;

  @override
  State<_CountForm> createState() => _CountFormState();
}

class _CountFormState extends State<_CountForm> {
  final formKey = GlobalKey<FormState>();
  final amountController = TextEditingController();
  final noteController = TextEditingController();
  TransactionScope? explicitScope;
  bool scopeMissing = false;

  @override
  void dispose() {
    amountController.dispose();
    noteController.dispose();
    super.dispose();
  }

  /// Zincirin önizlemesi: kullanıcının seçimi → kasanın etiketi. Sayımın
  /// kategorisi yoktur, zincirin üçüncü halkası burada sorulmaz.
  TransactionScope? get resolvedScope => explicitScope ?? accountScope;

  TransactionScope? get accountScope {
    for (final account in widget.controller.accounts) {
      if (account.id == widget.controller.selectedAccountId) {
        return account.defaultScope;
      }
    }
    return null;
  }

  /// Kapsam çipleri boyut görünürken **ya da** zincir çözülemediğinde çizilir.
  /// İkincisi olmasaydı kasasını etiketlememiş kullanıcı sayımını hiç
  /// kaydedemezdi: sunucu kapsam uydurmaz ve isteği reddederdi.
  bool get showScope =>
      (widget.scopeController?.isVisible ?? false) || resolvedScope == null;

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: 'Gün sonu sayımı',
      description:
          'Sayım bir gözlemdir: yazmak hiçbir bakiyeyi değiştirmez ve hiçbir '
          'rapora girmez.',
      submitLabel: 'Sayımı kaydet',
      onSubmit: _submit,
      children: [
        TextFormField(
          controller: amountController,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          decoration: const InputDecoration(
            labelText: 'Kasada sayılan',
            helperText: 'Kasa boşsa sıfır yazın; bu da bir sayımdır.',
          ),
          autofocus: true,
          validator: (value) {
            final amount = MoneyInput.parse(value ?? '');
            if (amount == null || amount < 0) {
              return 'Geçerli bir tutar girin.';
            }
            return null;
          },
        ),
        const SizedBox(height: AppSpacing.medium),
        TextFormField(
          controller: noteController,
          decoration: const InputDecoration(labelText: 'Not (isteğe bağlı)'),
        ),
        if (showScope) ...[
          const SizedBox(height: AppSpacing.medium),
          AppScopeField(
            value: resolvedScope,
            onChanged: (value) => setState(() {
              explicitScope = value;
              scopeMissing = false;
            }),
            helperText: explicitScope != null
                ? 'Bu sayım için siz seçtiniz.'
                : accountScope != null
                ? 'Kasanın etiketinden geldi — değiştirebilirsiniz.'
                : 'Kasa kapsam taşımıyor; bu sayım için seçin.',
            errorText: scopeMissing ? 'Bu sayım için kapsam seçin.' : null,
          ),
        ],
      ],
    ),
  );

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    // Zincir çözülemediyse istek sunucuya gitmeden burada duruyor: sunucu da
    // reddederdi (`cash_counts.scope_unresolved`) ama kullanıcı hatayı
    // düzeltebileceği yerde görmeli.
    if (resolvedScope == null) {
      setState(() => scopeMissing = true);
      return null;
    }
    final note = noteController.text.trim();
    final saved = await widget.controller.recordCount(
      countedAmount: MoneyInput.wire(amountController.text),
      countDate: CashCountController.todayDate(),
      scope: resolvedScope,
      note: note.isEmpty ? null : note,
    );
    return saved ? true : null;
  }
}

class _DifferenceForm extends StatefulWidget {
  const _DifferenceForm({required this.controller});

  final CashCountController controller;

  @override
  State<_DifferenceForm> createState() => _DifferenceFormState();
}

class _DifferenceFormState extends State<_DifferenceForm> {
  final formKey = GlobalKey<FormState>();
  late final Future<List<DataChoice>> categories;
  String? categoryId;

  @override
  void initState() {
    super.initState();
    categories = widget.controller.loadDifferenceCategories();
  }

  @override
  Widget build(BuildContext context) {
    final isIncome = widget.controller.differenceCategoryType == 'income';
    return Form(
      key: formKey,
      child: AppFormSheet<bool>(
        title: isIncome ? 'Fazlayı kaydet' : 'Eksiği kaydet',
        description: isIncome
            ? 'Kasada beklenenden fazla nakit çıktı; bu tek bir gelir kaydı '
                  'olarak yazılır.'
            : 'Kasada beklenenden az nakit çıktı; bu tek bir gider kaydı '
                  'olarak yazılır.',
        submitLabel: 'Kaydet',
        onSubmit: _submit,
        children: [
          FutureBuilder<List<DataChoice>>(
            future: categories,
            builder: (context, snapshot) {
              if (snapshot.hasError) {
                return const Text(
                  'Kategoriler yüklenemedi. Paneli kapatıp yeniden deneyin.',
                );
              }
              if (!snapshot.hasData) return const LinearProgressIndicator();
              return DropdownButtonFormField<String>(
                initialValue: categoryId,
                isExpanded: true,
                decoration: const InputDecoration(labelText: 'Kategori'),
                items: [
                  for (final category in snapshot.data!)
                    DropdownMenuItem(
                      value: category.id,
                      child: Text(category.name),
                    ),
                ],
                onChanged: (value) => categoryId = value,
                validator: (value) => value == null ? 'Kategori seçin.' : null,
              );
            },
          ),
        ],
      ),
    );
  }

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    final saved = await widget.controller.confirmDifference(categoryId!);
    return saved ? true : null;
  }
}
