import 'package:flutter/material.dart';

import '../../../core/formatters/money_input.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../activities/presentation/activity_tile.dart';
import '../data/counterparty_models.dart';
import 'counterparties_controller.dart';
import 'counterparty_forms.dart';

/// Bir kişiyle olan her şey: açık cari bakiyesi, hareket geçmişi ve varsa
/// taksitli sözleşmeleri.
///
/// Üç blok tek ekranda ama **hiçbiri diğerinin toplamına karışmaz**: cari
/// bakiye kendi hareketlerinden, sözleşmenin kalanı kendi taksitlerinden
/// hesaplanır. İkisini toplayan bir sayı burada bilerek yok — aynı kişiyle
/// iki ayrı hesabınız var ve onları toplamak ikisini de anlamsız kılardı.
class CounterpartyDetailPage extends StatefulWidget {
  const CounterpartyDetailPage({
    required this.controller,
    required this.counterpartyId,
    super.key,
    this.showScope = false,
  });

  final CounterpartiesController controller;
  final String counterpartyId;
  final bool showScope;

  @override
  State<CounterpartyDetailPage> createState() => _CounterpartyDetailPageState();
}

class _CounterpartyDetailPageState extends State<CounterpartyDetailPage> {
  CounterpartiesController get controller => widget.controller;

  @override
  void initState() {
    super.initState();
    controller.addListener(_changed);
    // Okuma ilk kareden sonra: controller listeyi de besliyor ve initState
    // içinde bildirim yayması, hâlâ kurulmakta olan liste ekranını build
    // sırasında setState'e zorlardı.
    Future.microtask(() => controller.loadDetail(widget.counterpartyId));
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    super.dispose();
  }

  CounterpartyDetail? get _detail =>
      controller.detail?.counterparty.id == widget.counterpartyId
      ? controller.detail
      : null;

  @override
  Widget build(BuildContext context) {
    final detail = _detail;
    return Scaffold(
      appBar: AppBar(
        title: Text(detail?.counterparty.name ?? 'Karşı taraf'),
        actions: [
          if (detail != null)
            IconButton(
              tooltip: 'Düzenle',
              icon: const Icon(Icons.edit_outlined),
              onPressed: controller.isSubmitting
                  ? null
                  : () => _edit(detail.counterparty),
            ),
          if (detail != null)
            IconButton(
              tooltip: 'Sil',
              icon: const Icon(Icons.delete_outline),
              onPressed: controller.isSubmitting ? null : _delete,
            ),
        ],
      ),
      body: _body(detail),
    );
  }

  Widget _body(CounterpartyDetail? detail) {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (detail == null) {
      if (controller.isDetailLoading) {
        return const AppLoadingView(message: 'Cari hesap yükleniyor');
      }
      return AppErrorView(
        message: controller.errorMessage ?? 'Cari hesap alınamadı.',
        onRetry: () => controller.loadDetail(widget.counterpartyId),
      );
    }

    return Column(
      children: [
        if (controller.isDetailLoading || controller.isSubmitting)
          const LinearProgressIndicator(),
        if (controller.errorMessage != null ||
            controller.successMessage != null)
          MaterialBanner(
            content: Text(
              controller.errorMessage ?? controller.successMessage!,
            ),
            leading: Icon(
              controller.errorMessage == null
                  ? Icons.check_circle_outline
                  : Icons.error_outline,
            ),
            actions: [
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: () => controller.loadDetail(widget.counterpartyId),
            child: ListView(
              padding: const EdgeInsets.all(AppSpacing.medium),
              children: [
                _balanceCard(detail.counterparty),
                const SizedBox(height: AppSpacing.medium),
                _actions(detail.counterparty),
                if (detail.agreements.isNotEmpty) ...[
                  const SizedBox(height: AppSpacing.medium),
                  _agreements(detail),
                ],
                const SizedBox(height: AppSpacing.medium),
                _history(detail),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _balanceCard(CounterpartySummary person) {
    final theme = Theme.of(context);
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(person.name, style: theme.textTheme.titleMedium),
              ),
              if (!person.isActive)
                const AppStatusChip(
                  label: 'Pasif',
                  icon: Icons.pause_circle_outline,
                  tone: AppStatusTone.cancelled,
                ),
            ],
          ),
          if (person.note != null && person.note!.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.xSmall),
            Text(person.note!, style: theme.textTheme.bodySmall),
          ],
          const SizedBox(height: AppSpacing.small),
          // İki taraf ayrı ayrı duruyor: aynı kişi hem müşteri hem tedarikçi
          // olabilir ve tek sayıya indirmek hangi tarafın açık olduğunu
          // gizlerdi. Net ikisini tek cümleye indiren üçüncü satırdır.
          _amountRow('Size borcu', person.receivable, AppMoneyEffect.income),
          if (person.hasOverdueReceivable) ...[
            _amountRow(
              'Vadesi geçmiş alacak',
              person.overdueReceivable,
              AppMoneyEffect.income,
            ),
            _amountRow(
              'Vadesi geçmemiş veya vadesiz alacak',
              person.notOverdueReceivable,
              AppMoneyEffect.income,
            ),
          ],
          _amountRow('Sizin borcunuz', person.payable, AppMoneyEffect.expense),
          if (person.hasOverduePayable) ...[
            _amountRow(
              'Vadesi geçmiş borç',
              person.overduePayable,
              AppMoneyEffect.expense,
            ),
            _amountRow(
              'Vadesi geçmemiş veya vadesiz borç',
              person.notOverduePayable,
              AppMoneyEffect.expense,
            ),
          ],
          const Divider(height: AppSpacing.large),
          _amountRow(
            'Net',
            person.net,
            person.isSettled
                ? AppMoneyEffect.neutral
                : person.isReceivableSide
                ? AppMoneyEffect.income
                : AppMoneyEffect.expense,
            emphasise: true,
          ),
          if (person.isSettled)
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.xSmall),
              child: Text('Hesap kapandı.', style: theme.textTheme.bodySmall),
            ),
        ],
      ),
    );
  }

  Widget _amountRow(
    String label,
    String amount,
    AppMoneyEffect effect, {
    bool emphasise = false,
  }) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.xSmall),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Expanded(
            child: Text(
              label,
              style: emphasise
                  ? theme.textTheme.titleSmall
                  : theme.textTheme.bodyMedium,
            ),
          ),
          const SizedBox(width: AppSpacing.small),
          AppMoneyText(
            amount: amount,
            currency: 'TRY',
            effect: effect,
            style: emphasise
                ? theme.textTheme.titleMedium
                : theme.textTheme.bodyMedium,
          ),
        ],
      ),
    );
  }

  /// Dört eylem iki satırda: borçlandırma tanır, tahsilat taşır.
  ///
  /// Pasif karşı tarafta borçlandırma kapalı ama tahsilat açık kalır — kalan
  /// borç kapatılabilmeli, yoksa bakiye sonsuza kadar açık kalırdı.
  Widget _actions(CounterpartySummary person) {
    final blocked = !person.isActive;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (blocked)
          const Padding(
            padding: EdgeInsets.only(bottom: AppSpacing.small),
            child: AppInlineNotice(
              icon: Icons.info_outline,
              message:
                  'Bu karşı taraf pasif: yeni borç yazamazsınız ama kalan '
                  'bakiyeyi tahsil edebilirsiniz.',
            ),
          ),
        Row(
          children: [
            Expanded(
              child: OutlinedButton.icon(
                onPressed: blocked || controller.isSubmitting
                    ? null
                    : () => _charge(person, isReceivable: true),
                icon: const Icon(Icons.south_west),
                label: const Text('Veresiye satış'),
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            Expanded(
              child: OutlinedButton.icon(
                onPressed: blocked || controller.isSubmitting
                    ? null
                    : () => _charge(person, isReceivable: false),
                icon: const Icon(Icons.north_east),
                label: const Text('Vadeli alım'),
              ),
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.small),
        Row(
          children: [
            Expanded(
              child: FilledButton.icon(
                onPressed: controller.isSubmitting
                    ? null
                    : () => _settle(person, isReceivable: true),
                icon: const Icon(Icons.price_check),
                label: const Text('Tahsilat'),
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            Expanded(
              child: FilledButton.tonalIcon(
                onPressed: controller.isSubmitting
                    ? null
                    : () => _settle(person, isReceivable: false),
                icon: const Icon(Icons.payments_outlined),
                label: const Text('Ödeme'),
              ),
            ),
          ],
        ),
      ],
    );
  }

  /// Taksitli sözleşmeler cari hesabın **dışında** durur ve toplamı ayrı
  /// yazılır; ikisini toplamak aynı kişiyle iki farklı hesabı tek sayıya
  /// indirmek olurdu.
  Widget _agreements(CounterpartyDetail detail) {
    final theme = Theme.of(context);
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Taksitli sözleşmeler', style: theme.textTheme.titleSmall),
          const SizedBox(height: AppSpacing.xSmall),
          Text(
            'Cari hesabın dışında; kalan tutarları yukarıdaki bakiyeye '
            'eklenmez.',
            style: theme.textTheme.bodySmall,
          ),
          for (final agreement in detail.agreements)
            AppListRow(
              icon: agreement.isReceivable
                  ? Icons.trending_up
                  : Icons.trending_down,
              title: agreement.isReceivable ? 'Alacak planı' : 'Borç planı',
              subtitle:
                  '${agreement.installmentCount} taksitin '
                  '${agreement.paidCount} tanesi ödendi',
              trailing: AppMoneyText(
                amount: agreement.remaining,
                currency: 'TRY',
                effect: agreement.isReceivable
                    ? AppMoneyEffect.income
                    : AppMoneyEffect.expense,
              ),
            ),
        ],
      ),
    );
  }

  Widget _history(CounterpartyDetail detail) {
    final theme = Theme.of(context);
    if (detail.activities.isEmpty) {
      return const SizedBox(
        height: 180,
        child: AppEmptyView(
          icon: Icons.receipt_long_outlined,
          title: 'Hareket yok',
          message: 'Bu kişiyle henüz bir alışveriş kaydedilmedi.',
        ),
      );
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Hareketler', style: theme.textTheme.titleSmall),
        const SizedBox(height: AppSpacing.xSmall),
        for (final activity in detail.activities)
          ActivityTile(activity: activity),
      ],
    );
  }

  Future<void> _charge(
    CounterpartySummary person, {
    required bool isReceivable,
  }) async {
    final snapshot = controller.snapshot;
    if (snapshot == null) return;
    final payload = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => CounterpartyChargeForm(
        today: controller.today,
        categories: snapshot.categories,
        isReceivable: isReceivable,
        showScope: widget.showScope,
      ),
    );
    if (payload == null) return;
    await controller.addCharge(
      person.id,
      isReceivable: isReceivable,
      amount: payload['amount']! as String,
      categoryId: payload['categoryId']! as String,
      chargeDate: payload['chargeDate']! as String,
      dueDate: payload['dueDate'] as String?,
      scope: payload['scope'] as String?,
      description: payload['description'] as String?,
    );
  }

  Future<void> _settle(
    CounterpartySummary person, {
    required bool isReceivable,
  }) async {
    final snapshot = controller.snapshot;
    if (snapshot == null) return;
    final open = isReceivable ? person.receivable : person.payable;
    final payload = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => CounterpartyPaymentForm(
        today: controller.today,
        accounts: snapshot.accounts,
        isReceivable: isReceivable,
        suggestedAmount: MoneyInput.parse(open) == null || open.startsWith('-')
            ? null
            : open,
      ),
    );
    if (payload == null) return;
    await controller.addPayment(
      person.id,
      isReceivable: isReceivable,
      amount: payload['amount']! as String,
      accountId: payload['accountId']! as String,
      paymentDate: payload['paymentDate']! as String,
      description: payload['description'] as String?,
    );
  }

  Future<void> _edit(CounterpartySummary person) async {
    final payload = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => CounterpartyForm(existing: person),
    );
    if (payload == null) return;
    await controller.update(
      person.id,
      name: payload['name']! as String,
      isActive: payload['isActive']! as bool,
      note: payload['note'] as String?,
    );
  }

  /// Hareketi olan karşı taraf silinmez; sunucu `409` döner ve mesajı
  /// pasifleştirmeye yönlendirir. İstemci burada kendi kuralını yazmıyor —
  /// yazsaydı iki kural birbirinden ayrı düşerdi.
  Future<void> _delete() async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      title: 'Karşı taraf silinsin mi?',
      message:
          'Hareketi olan bir karşı taraf silinemez; onun yerine pasife '
          'alabilirsiniz.',
      confirmLabel: 'Sil',
      icon: Icons.delete_outline,
      destructive: true,
    );
    if (!confirmed) return;
    final removed = await controller.delete(widget.counterpartyId);
    if (removed && mounted) Navigator.of(context).pop();
  }
}
