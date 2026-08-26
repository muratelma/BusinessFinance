import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_input.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_metric_tile.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/pos_repository.dart';
import 'pos_controller.dart';

/// POS tahsilatları sekmesi.
///
/// Bu ekranda `kart` kelimesi tek başına geçmez (ADR 0015): borçlandığın kart
/// başka bir şeydir. Henüz hesaba geçmemiş para **yolda**dır; `bloke`
/// bankacılık jargonudur ve kullanıcının kelimesi değildir.
class PosSettlementsView extends StatefulWidget {
  const PosSettlementsView({
    required this.controller,
    super.key,
    this.scopeController,
  });

  final PosController controller;
  final ScopeController? scopeController;

  @override
  State<PosSettlementsView> createState() => _PosSettlementsViewState();
}

class _PosSettlementsViewState extends State<PosSettlementsView> {
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
    if (controller.isLoading && controller.settlements == null) {
      return const AppLoadingView(message: 'Tahsilatlar yükleniyor');
    }
    if (controller.unauthorized && controller.settlements == null) {
      return const AppUnauthorizedView();
    }
    if (controller.errorMessage != null && controller.settlements == null) {
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }

    return Scaffold(
      body: RefreshIndicator(
        onRefresh: controller.load,
        child: ListView(
          padding: const EdgeInsets.all(AppSpacing.medium),
          children: [
            if (controller.isStale)
              AppInlineNotice(
                message:
                    '${controller.errorMessage} Son bilinen liste gösteriliyor.',
                actionLabel: 'Yenile',
                onAction: controller.load,
              ),
            AppCard(
              child: AppMetricTile(
                label: 'Yolda',
                amount: controller.moneyInTransit,
                currency: 'TRY',
                caption: controller.inTransitCount == 0
                    ? 'Bekleyen tahsilat yok'
                    : '${controller.inTransitCount} tahsilat hesaba geçmedi',
                size: AppMetricSize.hero,
              ),
            ),
            const SizedBox(height: AppSpacing.small),
            Text(
              'Yoldaki para sizindir ve net varlığınıza girer, ama bugün '
              'harcanamaz: banka komisyonu kesip kalanı birkaç gün içinde '
              'hesabınıza geçirir.',
              style: Theme.of(context).textTheme.bodySmall,
            ),
            const SizedBox(height: AppSpacing.medium),
            SegmentedButton<bool>(
              segments: const [
                ButtonSegment(value: false, label: Text('Hepsi')),
                ButtonSegment(value: true, label: Text('Yolda')),
              ],
              selected: {controller.inTransitOnly},
              onSelectionChanged: (value) =>
                  controller.setInTransitOnly(value.first),
            ),
            const SizedBox(height: AppSpacing.medium),
            ..._list(controller),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        heroTag: 'pos-settlement-new',
        onPressed: _openForm,
        icon: const Icon(Icons.add),
        label: const Text('Tahsilat ekle'),
      ),
    );
  }

  List<Widget> _list(PosController controller) {
    if (controller.items.isEmpty) {
      return const [
        AppEmptyView(
          title: 'Tahsilat yok.',
          message:
              'Müşterinizin ödediği tutarı yazdığınızda satış o gün gelir '
              'olarak tanınır; para hesabınıza geçtiğinde işaretlersiniz.',
          icon: Icons.point_of_sale_outlined,
        ),
      ];
    }
    return [
      for (final item in controller.items)
        Padding(
          padding: const EdgeInsets.only(bottom: AppSpacing.small),
          child: AppCard(
            padding: EdgeInsets.zero,
            child: AppListRow(
              icon: Icons.south_west,
              title: item.description ?? item.categoryName,
              subtitle: _subtitle(item),
              badge: AppStatusChip(
                label: item.isInTransit
                    ? (item.isLate ? 'Gecikti' : 'Yolda')
                    : 'Hesaba geçti',
                icon: item.isInTransit
                    ? (item.isLate ? Icons.warning_amber : Icons.schedule)
                    : Icons.check_circle_outline,
                tone: item.isInTransit
                    ? (item.isLate
                          ? AppStatusTone.expense
                          : AppStatusTone.planned)
                    : AppStatusTone.income,
              ),
              trailing: AppMoneyText(
                amount: item.grossAmount,
                currency: item.currency,
              ),
              onTap: item.isInTransit ? () => _confirmTransfer(item) : null,
            ),
          ),
        ),
    ];
  }

  /// Komisyon brütün yanında okunur, ona katılmaz.
  String _subtitle(PosSettlementItem item) {
    final where = item.isInTransit
        ? 'Beklenen ${DateText.dayMonth(item.expectedTransferDate)}'
        : 'Geçti ${DateText.dayMonth(item.transferredOn!)}';
    if (_isZeroMoney(item.commissionAmount)) {
      return '${DateText.dayMonth(item.settlementDate)} · $where';
    }
    return '${DateText.dayMonth(item.settlementDate)} · Komisyon '
        '${MoneyText.format(item.commissionAmount, item.currency)} · Net '
        '${MoneyText.format(item.netAmount, item.currency)} · $where';
  }

  Future<void> _confirmTransfer(PosSettlementItem item) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.account_balance_outlined,
      title: 'Para hesaba geçti mi?',
      message:
          'Hesabınıza net tutar eklenir. Satış tahsil edildiği gün zaten gelir '
          'olarak yazıldı; bu adım gelir veya gider yazmaz.',
      highlight: MoneyText.format(item.netAmount, item.currency),
      confirmLabel: 'Geçti olarak işaretle',
    );
    if (!confirmed || !mounted) return;
    await widget.controller.markTransferred(item, _today());
  }

  Future<void> _openForm() async {
    await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => _SettlementForm(
        controller: widget.controller,
        scopeController: widget.scopeController,
      ),
    );
  }

  static String _today() {
    final value = DateTime.now();
    return '${value.year.toString().padLeft(4, '0')}-'
        '${value.month.toString().padLeft(2, '0')}-'
        '${value.day.toString().padLeft(2, '0')}';
  }
}

bool _isZeroMoney(String value) =>
    !value.replaceAll(RegExp('[^0-9]'), '').contains(RegExp('[1-9]'));

enum _CommissionMode { none, amount, rate }

class _SettlementForm extends StatefulWidget {
  const _SettlementForm({required this.controller, this.scopeController});

  final PosController controller;
  final ScopeController? scopeController;

  @override
  State<_SettlementForm> createState() => _SettlementFormState();
}

class _SettlementFormState extends State<_SettlementForm> {
  final formKey = GlobalKey<FormState>();
  final grossController = TextEditingController();
  final commissionController = TextEditingController();
  final descriptionController = TextEditingController();

  late final Future<PosOptions> options;
  String? accountId;
  String? categoryId;
  String? commissionCategoryId;
  _CommissionMode commissionMode = _CommissionMode.none;
  late String settlementDate = _PosSettlementsViewState._today();
  late String expectedTransferDate = _PosSettlementsViewState._today();
  TransactionScope? explicitScope;
  bool scopeMissing = false;
  PosOptions? loaded;

  @override
  void initState() {
    super.initState();
    options = widget.controller.loadOptions();
  }

  @override
  void dispose() {
    grossController.dispose();
    commissionController.dispose();
    descriptionController.dispose();
    super.dispose();
  }

  TransactionScope? get resolvedScope =>
      explicitScope ??
      _choiceScope(accountId, loaded?.accounts) ??
      _choiceScope(categoryId, loaded?.incomeCategories);

  TransactionScope? _choiceScope(String? id, List<DataChoice>? choices) {
    if (id == null || choices == null) return null;
    for (final choice in choices) {
      if (choice.id == id) return choice.defaultScope;
    }
    return null;
  }

  bool get showScope =>
      (widget.scopeController?.isVisible ?? false) || resolvedScope == null;

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: 'POS tahsilatı',
      description:
          'Satış bugün gelir olarak tanınır; para hesabınıza geçtiğinde '
          'ayrıca işaretlersiniz.',
      submitLabel: 'Tahsilatı kaydet',
      onSubmit: _submit,
      children: [
        FutureBuilder<PosOptions>(
          future: options,
          builder: (context, snapshot) {
            if (snapshot.hasError) {
              return const Text(
                'Hesap ve kategoriler yüklenemedi. Paneli kapatıp yeniden '
                'deneyin.',
              );
            }
            if (!snapshot.hasData) return const LinearProgressIndicator();
            loaded = snapshot.data;
            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: _fields(snapshot.data!),
            );
          },
        ),
      ],
    ),
  );

  List<Widget> _fields(PosOptions options) {
    return [
      TextFormField(
        controller: grossController,
        keyboardType: const TextInputType.numberWithOptions(decimal: true),
        decoration: const InputDecoration(
          labelText: 'Müşterinin ödediği',
          helperText: 'Brüt tutar: kestiğiniz fatura budur.',
        ),
        validator: MoneyInput.positiveError,
      ),
      const SizedBox(height: AppSpacing.medium),
      DropdownButtonFormField<String>(
        initialValue: accountId,
        isExpanded: true,
        decoration: const InputDecoration(
          labelText: 'Paranın geçeceği hesap',
          helperText: 'POS parası bankaya geçer.',
        ),
        items: [
          for (final account in options.accounts)
            DropdownMenuItem(value: account.id, child: Text(account.name)),
        ],
        onChanged: (value) => setState(() => accountId = value),
        validator: (value) => value == null ? 'Hesap seçin.' : null,
      ),
      const SizedBox(height: AppSpacing.medium),
      DropdownButtonFormField<String>(
        initialValue: categoryId,
        isExpanded: true,
        decoration: const InputDecoration(labelText: 'Satış kategorisi'),
        items: [
          for (final category in options.incomeCategories)
            DropdownMenuItem(value: category.id, child: Text(category.name)),
        ],
        onChanged: (value) => setState(() => categoryId = value),
        validator: (value) => value == null ? 'Kategori seçin.' : null,
      ),
      const SizedBox(height: AppSpacing.medium),
      AppDateField(
        label: 'Tahsilat günü',
        value: settlementDate,
        lastDate: DateTime.now(),
        onChanged: (value) => setState(() => settlementDate = value),
      ),
      const SizedBox(height: AppSpacing.medium),
      AppDateField(
        label: 'Paranın beklendiği gün',
        value: expectedTransferDate,
        firstDate: AppDateField.parse(settlementDate),
        onChanged: (value) => setState(() => expectedTransferDate = value),
      ),
      const SizedBox(height: AppSpacing.medium),
      // Başlık segmentin dışında: üç segment genişliği paylaşınca
      // `Komisyon yok` kelimenin ortasından bölünüyordu. Kapsam seçicisinde
      // olduğu gibi grubun adı üstte durur, segmentler kısa kalır.
      Semantics(
        container: true,
        label: 'Komisyon',
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Komisyon', style: Theme.of(context).textTheme.labelMedium),
            const SizedBox(height: AppSpacing.xSmall),
            SegmentedButton<_CommissionMode>(
              segments: const [
                ButtonSegment(value: _CommissionMode.none, label: Text('Yok')),
                ButtonSegment(
                  value: _CommissionMode.amount,
                  label: Text('Tutar'),
                ),
                ButtonSegment(value: _CommissionMode.rate, label: Text('Oran')),
              ],
              selected: {commissionMode},
              onSelectionChanged: (value) => setState(() {
                commissionMode = value.first;
                commissionController.clear();
              }),
            ),
          ],
        ),
      ),
      if (commissionMode != _CommissionMode.none) ...[
        const SizedBox(height: AppSpacing.medium),
        TextFormField(
          controller: commissionController,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          decoration: InputDecoration(
            labelText: commissionMode == _CommissionMode.amount
                ? 'Kesilen komisyon'
                : 'Komisyon oranı (%)',
            helperText: commissionMode == _CommissionMode.amount
                ? 'Brüt tutardan düşülmez, yanında ayrı gider olarak durur.'
                : 'Örnek: 1,5 yazın. Oran saklanmaz, tutara çevrilir.',
          ),
          validator: MoneyInput.positiveError,
        ),
        const SizedBox(height: AppSpacing.medium),
        DropdownButtonFormField<String>(
          initialValue: commissionCategoryId,
          isExpanded: true,
          decoration: const InputDecoration(
            labelText: 'Komisyon gider kategorisi',
          ),
          items: [
            for (final category in options.expenseCategories)
              DropdownMenuItem(value: category.id, child: Text(category.name)),
          ],
          onChanged: (value) => setState(() => commissionCategoryId = value),
          validator: (value) =>
              value == null ? 'Komisyon için kategori seçin.' : null,
        ),
      ],
      const SizedBox(height: AppSpacing.medium),
      TextFormField(
        controller: descriptionController,
        decoration: const InputDecoration(labelText: 'Açıklama (isteğe bağlı)'),
      ),
      if (showScope) ...[
        const SizedBox(height: AppSpacing.medium),
        AppScopeField(
          value: resolvedScope,
          onChanged: (value) => setState(() {
            explicitScope = value;
            scopeMissing = false;
          }),
          errorText: scopeMissing ? 'Bu tahsilat için kapsam seçin.' : null,
        ),
      ],
    ];
  }

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    if (resolvedScope == null) {
      setState(() => scopeMissing = true);
      return null;
    }

    // Komisyon **ya tutar ya oran** gider; ikisini birden göndermek sunucunun
    // reddettiği bir istektir ve haklıdır.
    String? commissionAmount;
    String? commissionRate;
    if (commissionMode == _CommissionMode.amount) {
      commissionAmount = MoneyInput.wire(commissionController.text);
    } else if (commissionMode == _CommissionMode.rate) {
      final percent = MoneyInput.parse(commissionController.text) ?? 0;
      commissionRate = (percent / 100).toStringAsFixed(4);
    }

    final description = descriptionController.text.trim();
    final saved = await widget.controller.create(
      accountId: accountId!,
      categoryId: categoryId!,
      grossAmount: MoneyInput.wire(grossController.text),
      settlementDate: settlementDate,
      expectedTransferDate: expectedTransferDate,
      scope: resolvedScope,
      commissionAmount: commissionAmount,
      commissionRate: commissionRate,
      commissionCategoryId: commissionMode == _CommissionMode.none
          ? null
          : commissionCategoryId,
      description: description.isEmpty ? null : description,
    );
    return saved ? true : null;
  }
}
