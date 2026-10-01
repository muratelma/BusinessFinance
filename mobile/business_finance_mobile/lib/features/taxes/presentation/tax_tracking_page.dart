import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/money_math.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_card_head.dart';
import '../../../core/widgets/app_date_leaf.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../../../core/widgets/app_text_action.dart';
import '../../activities/data/planned_activity_models.dart';
import '../data/tax_models.dart';
import 'tax_controller.dart';
import 'tax_parts.dart';
import 'tax_payments_page.dart';
import 'tax_plan_detail_page.dart';
import 'tax_schedule.dart';
import 'tax_sheets.dart';
import 'tax_type_page.dart';

/// Vergi takibi (ADR 0018): bekleyenler, tanımlı vergiler, ödenenler.
///
/// **Ana eylem her hâlde altta sabittir**: "Vergi ödemesi ekle" hiçbir tanım
/// istemez ve kaydırmayla kaybolmaz. Hiç vergi tanımlamamış kullanıcıda
/// "Vergilerimi tanımla" ikincil kalır ve ana eylemi gölgelemez (İ4).
class TaxTrackingPage extends StatefulWidget {
  const TaxTrackingPage({
    required this.controller,
    super.key,
    this.ownsController = true,
  });

  final TaxController controller;
  final bool ownsController;

  @override
  State<TaxTrackingPage> createState() => _TaxTrackingPageState();
}

class _TaxTrackingPageState extends State<TaxTrackingPage> {
  TaxController get controller => widget.controller;

  @override
  void initState() {
    super.initState();
    controller.addListener(_changed);
    if (!controller.hasLoaded && !controller.isLoading) controller.load();
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    if (widget.ownsController) controller.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final ready = controller.overview != null && controller.options != null;
    return TaxPageScaffold(
      title: 'Vergi takibi',
      footer: ready
          // Gönderim değil, panel açan düğme: `AppSubmitButton` panel açık
          // kaldıkça yükleniyor simgesi gösterirdi.
          ? FilledButton.icon(
              style: FilledButton.styleFrom(
                minimumSize: const Size.fromHeight(48),
              ),
              onPressed: () async {
                final saved = await showTaxPaymentSheet(context, controller);
                if (saved == true && mounted) _saved('Ödeme kaydedildi.');
              },
              icon: const Icon(Icons.add),
              label: const Text('Vergi ödemesi ekle'),
            )
          : null,
      body: _body(context),
    );
  }

  Widget _body(BuildContext context) {
    if (controller.unauthorized) return const AppUnauthorizedView();
    final overview = controller.overview;
    if (overview == null || controller.options == null) {
      if (controller.errorMessage != null) {
        return AppErrorView(
          message: controller.errorMessage!,
          onRetry: controller.load,
        );
      }
      return const AppLoadingView(message: 'Vergiler yükleniyor');
    }
    final stale = controller.errorMessage;
    return TaxPageBody(
      onRefresh: controller.load,
      children: [
        // Eski veriyle açık kalan ekran bunu söyler; sayılar dünün olabilir.
        if (stale != null)
          Padding(
            padding: const EdgeInsets.only(bottom: AppSpacing.medium),
            child: Semantics(
              liveRegion: true,
              child: Text(
                'Güncellenemedi: $stale',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Theme.of(context).colorScheme.error,
                ),
              ),
            ),
          ),
        if (controller.hasNoPlans)
          ..._firstUse(context, overview)
        else ...[
          _pendingSection(context, overview),
          _plansSection(context, overview),
          if (overview.recentPayments.isNotEmpty)
            _paidSection(context, overview)
          else
            TaxSection(
              title: 'Ödenenler',
              child: AppCard(
                child: Text(
                  'Henüz ödenen vergi yok.',
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
              ),
            ),
        ],
      ],
    );
  }

  // V1 · İlk kullanım -------------------------------------------------------

  List<Widget> _firstUse(BuildContext context, TaxOverview overview) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return [
      AppCard(
        padding: const EdgeInsets.all(AppSpacing.large),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const AppIconCapsule(icon: Icons.receipt_long_outlined, size: 56),
            const SizedBox(height: AppSpacing.medium),
            Text(
              'Ödediğiniz vergiyi tek tutarla yazın',
              style: theme.textTheme.titleMedium?.copyWith(
                fontSize: 18,
                height: 1.35,
              ),
            ),
            const SizedBox(height: AppSpacing.xSmall),
            Text(
              'İsterseniz vergilerinizi tanımlayıp ne zaman ödeneceğini '
              'takip edin. Ödenene kadar hiçbir vergi bakiyenizi ya da '
              'bütçenizi etkilemez.',
              style: theme.textTheme.bodyLarge?.copyWith(
                fontSize: 15,
                color: surfaces.inkMuted,
              ),
            ),
            const SizedBox(height: AppSpacing.medium),
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                style: OutlinedButton.styleFrom(
                  minimumSize: const Size.fromHeight(48),
                ),
                onPressed: () => _openTypes(context, multiple: true),
                icon: const Icon(Icons.event_repeat_outlined),
                label: const Text('Vergilerimi tanımla'),
              ),
            ),
          ],
        ),
      ),
      if (overview.recentPayments.isNotEmpty)
        _paidSection(context, overview)
      else
        const TaxRule(
          "Ödenen vergi normal bir giderdir; İşlemler'de de görünür. Hangi "
          'vergileri ödemeniz gerektiğini muhasebeciniz söyler.',
          top: AppSpacing.medium,
        ),
    ];
  }

  // V2 · Bekleyenler ---------------------------------------------------------

  Widget _pendingSection(BuildContext context, TaxOverview overview) {
    final overdue = controller.overdueCount;
    return TaxSection(
      first: true,
      title: 'Bekleyenler',
      child: AppCard(
        padding: EdgeInsets.zero,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppCardHead(
              title: 'Gecikenler ve 30 gün',
              status: overdue == 0
                  ? null
                  : AppStatusTag(
                      label: '$overdue gecikti',
                      icon: Icons.error_outline,
                      tone: AppStatusTone.expense,
                    ),
            ),
            if (overview.pending.isEmpty)
              Padding(
                padding: const EdgeInsets.all(AppSpacing.medium),
                child: Text(
                  '30 gün içinde bekleyen vergi yok.',
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: AppSurfaces.of(context).inkMuted,
                  ),
                ),
              )
            else ...[
              AppDividedColumn(
                inset: 76,
                children: [
                  for (final item in overview.pending)
                    _PendingRow(
                      item: item,
                      today: controller.today,
                      onOpen: () => showTaxPendingSheet(
                        context,
                        controller,
                        item,
                        onOpenPlan: () =>
                            _openPlan(context, item.recurringTransactionId),
                      ),
                      onPay: () async {
                        final saved = await showTaxPaySheet(
                          context,
                          controller,
                          item,
                        );
                        if (saved == true && mounted) {
                          _saved('Ödeme kaydedildi.');
                        }
                      },
                    ),
                ],
              ),
              _TotalsStrip(overview: overview),
            ],
          ],
        ),
      ),
    );
  }

  // V2 · Vergilerim ----------------------------------------------------------

  Widget _plansSection(BuildContext context, TaxOverview overview) {
    final scopeVisible = context.watch<ScopeController?>()?.isVisible ?? false;
    return TaxSection(
      title: 'Vergilerim',
      trailing: AppTextAction(
        label: 'Ekle',
        icon: Icons.add,
        onPressed: () => _openTypes(context, multiple: false),
      ),
      child: AppCard(
        padding: EdgeInsets.zero,
        child: AppDividedColumn(
          inset: AppIconCapsule.rowInset,
          children: [
            for (final plan in overview.plans)
              AppRow(
                leading: AppIconCapsule(
                  icon: taxKindIcon(plan.taxKind),
                  tone: plan.isActive ? null : AppStatusTone.cancelled,
                ),
                title: plan.name,
                subtitle: [
                  TaxSchedule.planRhythm(plan),
                  if (plan.isActive && plan.nextDate != null)
                    'sıradaki ${TaxSchedule.shortDate(plan.nextDate!)}',
                ].join(' · '),
                trailing: _planTrailing(plan, scopeVisible),
                onTap: () => _openPlan(context, plan.id),
              ),
          ],
        ),
      ),
    );
  }

  Widget? _planTrailing(TaxPlan plan, bool scopeVisible) {
    if (!plan.isActive) {
      return const AppStatusTag(
        label: 'Duraklatıldı',
        icon: Icons.pause_circle_outline,
        tone: AppStatusTone.planned,
      );
    }
    if (scopeVisible && plan.scope == TransactionScope.personal) {
      return const AppStatusTag(
        label: 'Şahsi',
        icon: Icons.person_outline,
        tone: AppStatusTone.planned,
      );
    }
    return plan.amount == null
        ? null
        : AppMoneyText(
            amount: plan.amount!,
            currency: plan.currency,
            size: AppMoneySize.body,
          );
  }

  // V2 · Ödenenler -----------------------------------------------------------

  Widget _paidSection(BuildContext context, TaxOverview overview) => TaxSection(
    title: 'Ödenenler',
    trailing: AppTextAction(
      label: 'Tümü',
      trailingIcon: Icons.chevron_right,
      onPressed: () => Navigator.of(context).push(
        MaterialPageRoute<void>(
          builder: (_) => TaxPaymentsPage(controller: controller),
        ),
      ),
    ),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppCard(
          padding: EdgeInsets.zero,
          child: AppDividedColumn(
            inset: 76,
            children: [
              for (final payment in overview.recentPayments)
                TaxPaidRow(
                  payment: payment,
                  onTap: () => showTaxPaidSheet(context, controller, payment),
                ),
            ],
          ),
        ),
        const TaxRule(
          'Vergi işaretli kategorilerdeki giderler; Gider formundan '
          'girilenler de burada görünür.',
        ),
      ],
    ),
  );

  Future<void> _openTypes(BuildContext context, {required bool multiple}) =>
      Navigator.of(context).push(
        MaterialPageRoute<void>(
          builder: (_) =>
              TaxTypePage(controller: controller, multiple: multiple),
        ),
      );

  void _openPlan(BuildContext context, String? planId) {
    if (planId == null) return;
    Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) =>
            TaxPlanDetailPage(controller: controller, planId: planId),
      ),
    );
  }

  void _saved(String message) {
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
  }
}

/// Bekleyen satırı (en az 72 dp): tarih yaprağı, ad, durum ve göreli gün,
/// tutar ya da "Tutar ödemede girilecek"; sağda "Ödedim". Satıra dokunmak
/// ayrıntıyı, "Ödedim" doğrudan paneli açar.
class _PendingRow extends StatelessWidget {
  const _PendingRow({
    required this.item,
    required this.today,
    required this.onOpen,
    required this.onPay,
  });

  final PlannedActivity item;
  final DateTime today;
  final VoidCallback onOpen;
  final VoidCallback onPay;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final help = theme.textTheme.bodyMedium?.copyWith(color: surfaces.inkMuted);
    final overdue = item.timing == PlannedTiming.overdue;
    return ConstrainedBox(
      constraints: const BoxConstraints(minHeight: 72),
      child: Padding(
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.small + AppSpacing.xSmall,
        ),
        child: Row(
          children: [
            Expanded(
              child: Semantics(
                button: true,
                child: InkWell(
                  onTap: onOpen,
                  child: Row(
                    children: [
                      AppDateLeaf.fromDate(
                        DateTime.parse(item.dueDate),
                        urgent: overdue,
                      ),
                      const SizedBox(width: AppSpacing.medium),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Text(
                              item.title,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: theme.textTheme.titleSmall,
                            ),
                            const SizedBox(height: AppSpacing.xxSmall),
                            Wrap(
                              spacing: 6,
                              runSpacing: AppSpacing.xxSmall,
                              crossAxisAlignment: WrapCrossAlignment.center,
                              children: [
                                taxTimingTag(item),
                                Text(
                                  TaxSchedule.relative(item.dueDate, today),
                                  softWrap: false,
                                  style: help,
                                ),
                              ],
                            ),
                            const SizedBox(height: AppSpacing.xxSmall),
                            if (item.amount == null)
                              Text(
                                'Tutar ödemede girilecek',
                                softWrap: false,
                                overflow: TextOverflow.fade,
                                style: help,
                              )
                            else
                              AppMoneyText(
                                amount: item.amount!,
                                currency: item.currency,
                                size: AppMoneySize.body,
                                style: const TextStyle(
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
            TaxPayButton(onPressed: onPay),
          ],
        ),
      ),
    );
  }
}

/// Bekleyenler kartının alt şeridi: solda "Ödenecek" (gecikenler dahil
/// toplam), sağda tutarı belli olmayanların sayısı. İkisi de sunucudan.
class _TotalsStrip extends StatelessWidget {
  const _TotalsStrip({required this.overview});

  final TaxOverview overview;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final help = theme.textTheme.bodyMedium?.copyWith(color: surfaces.inkMuted);
    final unknown = overview.pendingUnknownAmountCount;
    final currency = overview.pending.first.currency;
    final hasTotal = MoneyMath.parse(overview.pendingTotal) != BigInt.zero;
    Widget cell(String label, Widget value) => Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.small + AppSpacing.xSmall,
      ),
      child: MergeSemantics(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(label, style: help),
            const SizedBox(height: AppSpacing.xSmall),
            value,
          ],
        ),
      ),
    );
    final total = cell(
      'Ödenecek',
      hasTotal
          ? AppMoneyText(
              amount: overview.pendingTotal,
              currency: currency,
              effect: AppMoneyEffect.expense,
              size: AppMoneySize.row,
            )
          : Text('—', style: theme.textTheme.titleSmall),
    );
    final unknownCell = cell(
      'Tutarı belli olmayan',
      Text('$unknown ödeme', style: theme.textTheme.titleSmall),
    );
    return Container(
      decoration: BoxDecoration(
        color: surfaces.cardMuted,
        border: Border(top: BorderSide(color: surfaces.border)),
      ),
      child: unknown == 0
          ? Align(alignment: Alignment.centerLeft, child: total)
          : IntrinsicHeight(
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Expanded(child: total),
                  Padding(
                    padding: const EdgeInsets.symmetric(
                      vertical: AppSpacing.small + AppSpacing.xSmall,
                    ),
                    child: VerticalDivider(
                      width: 1,
                      thickness: 1,
                      color: surfaces.border,
                    ),
                  ),
                  Expanded(child: unknownCell),
                ],
              ),
            ),
    );
  }
}

/// Ödenenler satırı: tarih yaprağı, ad, kaynak (toplu ödemede kapattıkları),
/// eksi tutar.
class TaxPaidRow extends StatelessWidget {
  const TaxPaidRow({required this.payment, required this.onTap, super.key});

  final TaxPayment payment;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final closed = payment.closedItems.isEmpty
        ? ''
        : ' · Kapattı: ${payment.closedItems.map((item) => '${item.label} ${TaxSchedule.shortDate(item.scheduledDate)}').join(', ')}';
    return AppRow(
      leading: AppDateLeaf.fromDate(DateTime.parse(payment.paidOn)),
      title: payment.title,
      subtitle: '${payment.sourceName}$closed',
      trailing: AppMoneyText(
        amount: payment.amount,
        currency: payment.currency,
        effect: AppMoneyEffect.expense,
        signed: true,
        size: AppMoneySize.row,
      ),
      onTap: onTap,
    );
  }
}
