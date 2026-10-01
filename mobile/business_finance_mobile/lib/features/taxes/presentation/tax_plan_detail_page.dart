import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_leaf.dart';
import '../../../core/widgets/app_detail_block.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../activities/data/planned_activity_models.dart';
import '../data/tax_models.dart';
import 'tax_controller.dart';
import 'tax_parts.dart';
import 'tax_plan_form_page.dart';
import 'tax_schedule.dart';
import 'tax_sheets.dart';

/// Vergi tanımının ayrıntısı (V10): ritim, tutar, kaynak, kapsam; sıradaki
/// kalemler; geçmiş ödemeler; Duraklat/Sürdür ve Sil.
///
/// Ödenmiş ya da kapatılmış kalemi olan vergi silinmez: geçmiş kalemler
/// ödemelerine bağlıdır. Duraklatma yeni kalem üretmeyi durdurur.
class TaxPlanDetailPage extends StatefulWidget {
  const TaxPlanDetailPage({
    required this.controller,
    required this.planId,
    super.key,
  });

  final TaxController controller;
  final String planId;

  @override
  State<TaxPlanDetailPage> createState() => _TaxPlanDetailPageState();
}

class _TaxPlanDetailPageState extends State<TaxPlanDetailPage> {
  TaxController get controller => widget.controller;

  TaxPlanDetail? detail;
  ApiException? error;
  bool loading = false;
  int _seenFeed = 0;
  int _seenPlanning = 0;

  @override
  void initState() {
    super.initState();
    _seenFeed = controller.changes?.activityFeedRevision ?? 0;
    _seenPlanning = controller.changes?.planningRevision ?? 0;
    controller.changes?.addListener(_handleChanges);
    _load();
  }

  @override
  void dispose() {
    controller.changes?.removeListener(_handleChanges);
    super.dispose();
  }

  void _handleChanges() {
    final changes = controller.changes!;
    if (changes.activityFeedRevision == _seenFeed &&
        changes.planningRevision == _seenPlanning) {
      return;
    }
    _seenFeed = changes.activityFeedRevision;
    _seenPlanning = changes.planningRevision;
    _load();
  }

  Future<void> _load() async {
    setState(() {
      loading = true;
      error = null;
    });
    try {
      final loaded = await controller.repository.loadPlanDetail(
        widget.planId,
        asOfDate: controller.todayIso,
      );
      if (mounted) setState(() => detail = loaded);
    } on ApiException catch (exception) {
      if (mounted) setState(() => error = exception);
    } on FormatException {
      if (mounted) {
        setState(() => error = ApiException.local('response.invalid_format'));
      }
    } finally {
      if (mounted) setState(() => loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final plan = detail?.plan;
    return TaxPageScaffold(
      title: plan?.name ?? 'Vergi',
      actions: [
        if (plan != null)
          IconButton(
            tooltip: 'Düzenle',
            onPressed: () => _edit(plan),
            icon: const Icon(Icons.edit_outlined),
          ),
      ],
      body: _body(context),
    );
  }

  Widget _body(BuildContext context) {
    if (error?.isUnauthorized ?? false) return const AppUnauthorizedView();
    final loaded = detail;
    if (loaded == null) {
      if (error != null) {
        return AppErrorView(message: error!.message, onRetry: _load);
      }
      return const AppLoadingView(message: 'Vergi yükleniyor');
    }
    final plan = loaded.plan;
    final failure = error;
    final surfaces = AppSurfaces.of(context);
    final scopeVisible = context.watch<ScopeController?>()?.isVisible ?? false;
    final sourceName = controller.options?.nameOf(plan.sourceId);
    return TaxPageBody(
      onRefresh: _load,
      children: [
        if (failure != null)
          Padding(
            padding: const EdgeInsets.only(bottom: AppSpacing.medium),
            child: Text(
              'Güncellenemedi: ${failure.message}',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Theme.of(context).colorScheme.error,
              ),
            ),
          ),
        // Ayrıntı bloğu burada kart yüzeyinde: sayfanın zemini gri.
        DecoratedBox(
          decoration: BoxDecoration(
            color: surfaces.card,
            border: Border.all(color: surfaces.border),
            borderRadius: BorderRadius.circular(AppRadius.card),
          ),
          child: AppDetailBlock(
            background: surfaces.card,
            rows: [
              AppDetailRow(label: 'Ritim', value: TaxSchedule.planRhythm(plan)),
              if (plan.amount == null)
                const AppDetailRow(label: 'Tutar', value: 'Belli değil')
              else
                AppDetailRow(
                  label: 'Tutar',
                  trailing: AppMoneyText(
                    amount: plan.amount!,
                    currency: plan.currency,
                    size: AppMoneySize.body,
                    style: const TextStyle(fontWeight: FontWeight.w500),
                  ),
                ),
              AppDetailRow(
                label: 'Nereden ödenir',
                value: sourceName ?? 'Seçilmedi',
              ),
              if (scopeVisible)
                AppDetailRow(label: 'Kapsam', value: plan.scope.label),
            ],
          ),
        ),
        TaxSection(
          title: 'Sıradaki',
          child: loaded.upcoming.isEmpty
              ? AppCard(
                  child: Text(
                    plan.isActive
                        ? 'Sıradaki kalem yok.'
                        : 'Duraklatıldı; yeni kalem üretmiyor.',
                    style: Theme.of(
                      context,
                    ).textTheme.bodyMedium?.copyWith(color: surfaces.inkMuted),
                  ),
                )
              : AppCard(
                  padding: EdgeInsets.zero,
                  child: AppDividedColumn(
                    inset: 76,
                    children: [
                      for (final item in loaded.upcoming)
                        _UpcomingRow(
                          item: item,
                          today: controller.today,
                          onTap: () =>
                              showTaxPendingSheet(context, controller, item),
                        ),
                    ],
                  ),
                ),
        ),
        if (loaded.history.isNotEmpty)
          TaxSection(
            title: 'Geçmiş ödemeler',
            child: AppCard(
              padding: EdgeInsets.zero,
              child: AppDividedColumn(
                inset: 76,
                children: [
                  for (final item in loaded.history)
                    _HistoryRow(
                      item: item,
                      onTap: () => item.isClosed
                          ? showTaxClosedItemSheet(
                              context,
                              controller,
                              plan: plan,
                              item: item,
                            )
                          : showTaxPaidSheet(context, controller, item.payment),
                    ),
                ],
              ),
            ),
          ),
        TaxActionCard(
          top: AppSpacing.large,
          children: [
            if (plan.isActive)
              TaxActionRow(
                icon: Icons.pause_circle_outline,
                title: 'Duraklat',
                subtitle: 'Yeni kalem üretmez; istediğinizde sürdürürsünüz.',
                onTap: () => _setActive(false),
              )
            else
              TaxActionRow(
                icon: Icons.play_circle_outline,
                title: 'Sürdür',
                subtitle: 'Sıradaki kalemler yeniden görünür.',
                onTap: () => _setActive(true),
              ),
            TaxActionRow(
              icon: Icons.delete_outline,
              title: 'Sil',
              danger: true,
              disabled: loaded.history.isNotEmpty,
              subtitle: loaded.history.isEmpty
                  ? 'Tanım ve bekleyen kalemleri silinir.'
                  : 'Ödenmiş kalemleri olduğu için silinemez.',
              onTap: () => _delete(plan),
            ),
          ],
        ),
      ],
    );
  }

  Future<void> _edit(TaxPlan plan) async {
    await Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => TaxPlanFormPage(
          controller: controller,
          plan: plan,
          lastSettledDate: detail?.history.isEmpty ?? true
              ? null
              : detail!.history
                    .map((item) => item.scheduledDate)
                    .reduce((a, b) => a.compareTo(b) >= 0 ? a : b),
        ),
      ),
    );
    if (controller.changes == null) await _load();
  }

  Future<void> _setActive(bool isActive) async {
    final saved = await controller.setPlanActive(widget.planId, isActive);
    if (!mounted) return;
    if (!saved) {
      _snack(controller.writeError ?? 'Kaydedilemedi.');
    } else if (controller.changes == null) {
      await _load();
    }
  }

  Future<void> _delete(TaxPlan plan) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      destructive: true,
      subject: AppConfirmSubject(
        title: plan.name,
        detail: TaxSchedule.planRhythm(plan),
        amount: plan.amount == null
            ? null
            : MoneyText.format(plan.amount!, plan.currency),
      ),
      message: 'Tanım ve bekleyen kalemleri silinir; ödemeler etkilenmez.',
      confirmLabel: 'Sil',
    );
    if (!confirmed || !mounted) return;
    final deleted = await controller.deletePlan(plan.id);
    if (!mounted) return;
    if (deleted) {
      Navigator.of(context).pop();
    } else {
      _snack(controller.writeError ?? 'Silinemedi.');
    }
  }

  void _snack(String message) => ScaffoldMessenger.of(
    context,
  ).showSnackBar(SnackBar(content: Text(message)));
}

/// Sıradaki satırı: tarih yaprağı, tam tarih, durum, tutar ya da "Belli
/// değil".
class _UpcomingRow extends StatelessWidget {
  const _UpcomingRow({
    required this.item,
    required this.today,
    required this.onTap,
  });

  final PlannedActivity item;
  final DateTime today;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final overdue = item.timing == PlannedTiming.overdue;
    final days = TaxSchedule.daysUntil(item.dueDate, today);
    return AppRow(
      leading: AppDateLeaf.fromDate(
        DateTime.parse(item.dueDate),
        urgent: overdue,
      ),
      title: DateText.dayMonth(item.dueDate),
      subtitle: switch (item.timing) {
        PlannedTiming.overdue => '${-days} gün gecikti',
        PlannedTiming.today => 'Bugün',
        PlannedTiming.upcoming => days == 1 ? 'Yarın' : '$days gün sonra',
      },
      trailing: item.amount == null
          ? Text(
              'Belli değil',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: AppSurfaces.of(context).inkMuted,
              ),
            )
          : AppMoneyText(
              amount: item.amount!,
              currency: item.currency,
              size: AppMoneySize.body,
            ),
      onTap: onTap,
    );
  }
}

/// Geçmiş satırı: kapatılmış kalem nötr işaretle, ödenmiş kalem eksi tutarla.
class _HistoryRow extends StatelessWidget {
  const _HistoryRow({required this.item, required this.onTap});

  final TaxHistoryItem item;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final payment = item.payment;
    return AppRow(
      leading: AppDateLeaf.fromDate(DateTime.parse(item.scheduledDate)),
      title: DateText.dayMonth(item.scheduledDate),
      subtitle: item.isClosed
          ? 'Kapatıldı · ${TaxSchedule.shortDate(payment.paidOn)} toplu '
                'ödemeyle'
          : 'Ödendi ${TaxSchedule.shortDate(payment.paidOn)} · '
                '${payment.sourceName}',
      // Kapatılmışta durum alt satırda yazıyor; sağda yalnız işareti durur,
      // yoksa alt satır iki satıra bölünür.
      trailing: item.isClosed
          ? Icon(
              Icons.task_alt,
              size: 18,
              color: AppFinanceColors.of(context).neutral,
            )
          : AppMoneyText(
              amount: payment.amount,
              currency: payment.currency,
              effect: AppMoneyEffect.expense,
              signed: true,
              size: AppMoneySize.body,
            ),
      onTap: onTap,
    );
  }
}
