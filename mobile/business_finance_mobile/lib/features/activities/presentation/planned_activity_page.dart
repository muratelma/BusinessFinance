import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_filter_menu.dart';
import '../../../core/widgets/app_filter_sheet.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_no_results_view.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_range_calendar.dart';
import '../../../core/widgets/app_section_empty.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_text_action.dart';
import '../../../core/widgets/app_unknown_amount.dart';
import '../../planning/presentation/realize_day_sheet.dart';
import '../data/activity_models.dart';
import '../data/activity_repository.dart';
import '../data/planned_activity_models.dart';
import 'planned_activity_controller.dart';

/// Planlananlar (Codex 04, 06 ve 18. panolar; 19–20 Kasım 2026 kararları).
///
/// Yukarıdan aşağı: dönem kartı (tarih, kapsam ve ödenecek / beklenen giriş /
/// karta yansıyacak toplamları), seçimden önce gecikenlerin açılır satırı ve
/// gün gün kartlar. Toplamlar sunucudandır ve birbirinden düşülmez. Hiçbir
/// satır bakiyeye girmemiştir.
class PlannedActivityPageView extends StatefulWidget {
  const PlannedActivityPageView({
    required this.repository,
    super.key,
    this.changes,
    this.scopeController,
    this.asOf,
    this.openOverdue = false,
  });

  final ActivityRepositoryContract repository;

  /// Özet'ten gelindiyse onun günü: dönem bu günden başlayan 7 gündür.
  final DateTime? asOf;

  /// Özet'in gecikme şeridinden gelindiyse gecikenler açık gelir.
  final bool openOverdue;
  final FinancialDataChanges? changes;
  final ScopeController? scopeController;

  @override
  State<PlannedActivityPageView> createState() =>
      _PlannedActivityPageViewState();
}

class _PlannedActivityPageViewState extends State<PlannedActivityPageView> {
  late final PlannedActivityController _controller;
  late bool _earlierOpen = widget.openOverdue;

  @override
  void initState() {
    super.initState();
    _controller = PlannedActivityController(
      widget.repository,
      asOf: widget.asOf,
      financialDataChanges: widget.changes,
      scopeController: widget.scopeController,
    );
    _controller.load();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: AnimatedBuilder(
        animation: _controller,
        builder: (context, _) => Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Planlananlar',
              onBack: () => Navigator.maybePop(context),
              actions: [
                Padding(
                  padding: const EdgeInsets.only(right: AppSpacing.small),
                  child: AppTextAction(
                    label: 'Filtre',
                    underlined: true,
                    onPressed: _openFilter,
                  ),
                ),
              ],
            ),
            Expanded(
              child: Stack(
                children: [
                  Positioned.fill(child: _body(context)),
                  if (_controller.isLoading && _controller.page != null)
                    const Positioned(
                      top: 0,
                      left: 0,
                      right: 0,
                      child: LinearProgressIndicator(),
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    ),
  );

  Future<void> _openFilter() async {
    final result =
        await AppAdaptiveSheet.show<(PlannedPeriodChoice, PlannedTypeFilter)>(
          context: context,
          builder: (_) => _PlannedFilterSheet(
            period: _controller.period,
            type: _controller.typeFilter,
            today: _controller.today,
          ),
        );
    if (result == null) return;
    await _controller.apply(period: result.$1, type: result.$2);
  }

  Widget _body(BuildContext context) {
    if (_controller.unauthorized) return const AppUnauthorizedView();
    final page = _controller.page;
    if (page == null) {
      if (_controller.errorMessage != null) {
        return AppErrorView(
          message: _controller.errorMessage!,
          onRetry: _controller.load,
        );
      }
      return const AppLoadingView(message: 'Planlananlar yükleniyor');
    }
    final earlier = _controller.earlierItems;
    final items = _controller.periodItems;
    return RefreshIndicator(
      onRefresh: _controller.load,
      child: Align(
        alignment: Alignment.topCenter,
        child: ConstrainedBox(
          constraints: const BoxConstraints(
            maxWidth: AppBreakpoints.contentMaxWidth,
          ),
          child: ListView(
            physics: const AlwaysScrollableScrollPhysics(),
            padding: AppPageInsets.aboveBar(
              context,
              AppPageInsets.fullBleedList,
            ),
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppPageInsets.edge,
                  AppSpacing.small,
                  AppPageInsets.edge,
                  0,
                ),
                child: _PeriodCard(
                  label: _controller.periodLabel,
                  period: page.period,
                  scopeVisible: _controller.isScopeVisible,
                  scope: _controller.scope,
                  onScope: _controller.selectScope,
                ),
              ),
              if (_controller.typeFilter != PlannedTypeFilter.all)
                Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppPageInsets.edge,
                    AppSpacing.compact,
                    AppPageInsets.edge,
                    0,
                  ),
                  child: Align(
                    alignment: Alignment.centerLeft,
                    child: _ActiveTypeChip(
                      label: _controller.typeFilter.label,
                      onRemove: _controller.clearType,
                    ),
                  ),
                ),
              if (_controller.refreshFailed)
                AppInlineNotice(
                  message: 'Liste güncellenemedi.',
                  actionLabel: 'Tekrar dene',
                  onAction: _controller.load,
                ),
              if (page.skippedCount > 0)
                AppInlineNotice(
                  message:
                      '${page.skippedCount} kayıt bu sürümde gösterilemiyor. '
                      'Uygulamayı güncelleyince görünür.',
                ),
              if (earlier.isNotEmpty)
                Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppPageInsets.edge,
                    AppSpacing.compact,
                    AppPageInsets.edge,
                    0,
                  ),
                  child: _EarlierCard(
                    label: _controller.earlierLabel,
                    period: page.period,
                    items: earlier,
                    open: _earlierOpen,
                    onToggle: () =>
                        setState(() => _earlierOpen = !_earlierOpen),
                    showScope: _controller.scope == null,
                    onAct: _act,
                  ),
                ),
              if (items.isEmpty)
                Padding(
                  padding: const EdgeInsets.only(top: AppSpacing.compact),
                  child: _empty(earlier.isNotEmpty),
                )
              else
                ..._days(items),
            ],
          ),
        ),
      ),
    );
  }

  /// Boş durumlar (06. pano): tür seçiliyse sebep filtredir; değilse dönem.
  /// Önceki gecikenler varken dönem boşsa kartın altında kısa bilgi satırı.
  Widget _empty(bool hasEarlier) {
    if (_controller.typeFilter != PlannedTypeFilter.all && !hasEarlier) {
      return AppNoResultsView(
        reason: AppNoResultsReason.filter,
        title: 'Bu filtreye uyan planlanan işlem yok',
        message: AppNoResultsView.filterSentence([
          _controller.typeFilter.label,
        ]),
        actionLabel: 'Filtreyi temizle',
        onAction: _controller.clearType,
      );
    }
    if (hasEarlier) {
      return const Padding(
        padding: EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
        child: AppSectionEmpty('Bu dönemde bekleyen kayıt yok.'),
      );
    }
    return AppNoResultsView(
      reason: AppNoResultsReason.period,
      title: 'Bu dönemde bekleyen kayıt yok',
      actionLabel: 'Dönem seç',
      onAction: _openFilter,
    );
  }

  List<Widget> _days(List<PlannedActivity> items) {
    final groups = <String, List<PlannedActivity>>{};
    for (final item in items) {
      groups.putIfAbsent(item.dueDate, () => []).add(item);
    }
    final today = _controller.today;
    return [
      for (final entry in groups.entries) ...[
        _DayHeading(date: entry.key, today: today),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
          child: AppCard(
            padding: EdgeInsets.zero,
            child: Column(
              children: [
                for (final (index, item) in entry.value.indexed) ...[
                  if (index > 0)
                    const Divider(
                      height: 1,
                      indent: AppSpacing.medium,
                      endIndent: AppSpacing.medium,
                    ),
                  _PlannedRow(
                    key: ValueKey(item.listKey),
                    activity: item,
                    showScope: _controller.scope == null,
                    onAct: _act,
                  ),
                ],
              ],
            ),
          ),
        ),
      ],
    ];
  }

  /// Satırın eylemini yürütür. Satırın tamamı ve kısa eylem aynı hedefi
  /// açar; tek dokunuş mali kaydı onaylamaz.
  Future<void> _act(PlannedActivity activity) async {
    // Vergi kalemi tutar, gün ve hesap/kart ister; onları vergi ekranının
    // "Ödedim" paneli sorar (ADR 0018 T4).
    if (activity.isTax) {
      await context.push(taxesLocation);
      return;
    }
    // Kişinin vadeleri ve eski yükümlülükler borç-alacak ekranlarıyla
    // bağlanacak; şimdilik bilgi satırıdır.
    if (!_PlannedRow.hasAction(activity)) return;
    if (!activity.isDirectlyRealizable) {
      final destination = switch (activity.plannedKind) {
        PlannedKind.cardStatement ||
        PlannedKind.cardInstallment => '/more/cards',
        PlannedKind.debtInstallment ||
        PlannedKind.receivableInstallment => '/more/debts',
        PlannedKind.recurringOccurrence => '/more/planning',
        PlannedKind.payableObligation ||
        PlannedKind.counterpartyPayable ||
        PlannedKind.counterpartyReceivable ||
        PlannedKind.receivableObligation => null,
      };
      if (destination != null) await context.push(destination);
      return;
    }

    final source = activity.sourceName;
    if (activity.plannedKind == PlannedKind.recurringOccurrence) {
      final paidOn = await showRealizeDaySheet(
        context,
        title: activity.displayTitle,
        amount: activity.amount!,
        currency: activity.currency,
        dueDate: activity.dueDate,
        sourceName: source,
        isIncome: activity.effect == ActivityEffect.income,
      );
      if (paidOn == null || !mounted) return;
      await _realize(activity, paidOn: paidOn);
      return;
    }

    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.playlist_add_check,
      title: 'Karta yansıtılsın mı?',
      highlight:
          '${activity.displayTitle}\n'
          '${MoneyText.format(activity.amount!, activity.currency)}',
      message:
          '${source == null ? 'Bu taksit' : '$source kartındaki bu taksit'} '
          'kart harcaması olarak yazılacak.',
      confirmLabel: 'Karta yansıt',
    );
    if (!confirmed || !mounted) return;
    await _realize(activity);
  }

  Future<void> _realize(PlannedActivity activity, {String? paidOn}) async {
    var success = await _controller.realize(activity, paidOn: paidOn);
    if (!mounted) return;
    if (!success && _controller.earlierItemPending && paidOn != null) {
      final confirmed = await confirmSkippingEarlierItem(
        context,
        title: activity.displayTitle,
        dueDate: activity.dueDate,
        paidOn: paidOn,
        pendingDate: _controller.earlierPendingDate,
        pendingCount: _controller.earlierPendingCount,
      );
      if (!confirmed || !mounted) return;
      success = await _controller.realize(
        activity,
        paidOn: paidOn,
        allowEarlierPending: true,
      );
      if (!mounted) return;
    }
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success
              ? 'Kayıt gerçekleşti ve bakiyeye girdi.'
              : _controller.errorMessage ?? 'Gerçekleştirilemedi.',
        ),
      ),
    );
  }
}

/// Dönem kartı: açık gri başlıkta tarih ve kapsam; beyaz yüzeyde yalnız
/// ilgili kaydı olan toplamlar.
class _PeriodCard extends StatelessWidget {
  const _PeriodCard({
    required this.label,
    required this.period,
    required this.scopeVisible,
    required this.scope,
    required this.onScope,
  });

  final String label;
  final PlannedPeriod? period;
  final bool scopeVisible;
  final TransactionScope? scope;
  final ValueChanged<TransactionScope?> onScope;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final p = period;
    final lines = <Widget>[
      if (p != null && p.paymentCount > 0)
        _TotalLine(
          label: 'Ödenecek',
          amount: p.paymentCount == p.unknownAmountCount
              ? null
              : p.paymentTotal,
          note: p.unknownAmountCount == 0
              ? null
              : 'Tutarı eksik: ${p.unknownAmountCount} ödeme',
        ),
      if (p != null && p.incomingCount > 0)
        _TotalLine(label: 'Beklenen giriş', amount: p.incomingTotal),
      if (p != null && p.cardPostingCount > 0)
        _TotalLine(label: 'Karta yansıyacak', amount: p.cardPostingTotal),
    ];
    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Container(
            constraints: const BoxConstraints(minHeight: 56),
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.small,
              AppSpacing.small,
              AppSpacing.small,
            ),
            color: surfaces.cardMuted,
            child: Row(
              children: [
                Expanded(
                  child: Semantics(
                    header: true,
                    child: Text(
                      label,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontSize: AppTextSizes.section,
                        fontWeight: FontWeight.w700,
                        height: 1.25,
                      ),
                    ),
                  ),
                ),
                // Uygulamanın genel kapsamını değiştirir; diğer ekranlar da
                // aynı seçimle açılır.
                if (scopeVisible)
                  PopupMenuButton<String>(
                    tooltip: 'Kapsam: ${scope?.label ?? 'Hepsi'}',
                    initialValue: scope?.apiValue ?? 'all',
                    onSelected: (key) => onScope(
                      key == 'all'
                          ? null
                          : TransactionScope.values.firstWhere(
                              (value) => value.apiValue == key,
                            ),
                    ),
                    itemBuilder: (_) => [
                      const PopupMenuItem(value: 'all', child: Text('Hepsi')),
                      for (final value in TransactionScope.values)
                        PopupMenuItem(
                          value: value.apiValue,
                          child: Text(value.label),
                        ),
                    ],
                    child: ConstrainedBox(
                      constraints: const BoxConstraints(minHeight: 48),
                      child: Padding(
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppSpacing.small,
                        ),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Text(
                              scope?.label ?? 'Hepsi',
                              style: theme.textTheme.bodyMedium,
                            ),
                            const SizedBox(width: AppSpacing.xSmall),
                            Icon(
                              Icons.expand_more,
                              size: AppTextSizes.section,
                              color: surfaces.inkMuted,
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
              ],
            ),
          ),
          if (lines.isNotEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.small,
              ),
              child: Column(children: lines),
            ),
        ],
      ),
    );
  }
}

class _TotalLine extends StatelessWidget {
  const _TotalLine({required this.label, required this.amount, this.note});

  final String label;

  /// Hiçbir tutar bilinmiyorsa `null`: sıfır yazılmaz.
  final String? amount;
  final String? note;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.step6),
      child: MergeSemantics(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    label,
                    style: theme.textTheme.bodyLarge?.copyWith(
                      fontSize: AppTextSizes.compact,
                      height: 1.3,
                    ),
                  ),
                ),
                if (amount == null)
                  const AppUnknownAmount()
                else
                  AppMoneyText(
                    amount: amount!,
                    currency: 'TRY',
                    size: AppMoneySize.row,
                    style: const TextStyle(fontWeight: FontWeight.w700),
                  ),
              ],
            ),
            if (note != null)
              Padding(
                padding: const EdgeInsets.only(top: AppSpacing.xxSmall),
                child: Row(
                  children: [
                    Icon(
                      Icons.error_outline,
                      size: AppTextSizes.caption,
                      color: surfaces.inkMuted,
                    ),
                    const SizedBox(width: AppSpacing.xSmall),
                    Flexible(
                      child: Text(
                        note!,
                        style: theme.textTheme.bodySmall?.copyWith(
                          fontSize: AppTextSizes.caption,
                          color: surfaces.inkMuted,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
          ],
        ),
      ),
    );
  }
}

/// Seçimden önce gecikenler: kapalıyken tek satır (adet, tarih, bilinen
/// ödeme tutarı), açıkken kayıtlar aynı kartta. Dönem toplamına girmez.
class _EarlierCard extends StatelessWidget {
  const _EarlierCard({
    required this.label,
    required this.period,
    required this.items,
    required this.open,
    required this.onToggle,
    required this.showScope,
    required this.onAct,
  });

  final String label;
  final PlannedPeriod? period;
  final List<PlannedActivity> items;
  final bool open;
  final VoidCallback onToggle;
  final bool showScope;
  final ValueChanged<PlannedActivity> onAct;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final total = period?.earlierOverduePaymentTotal;
    final showTotal = total != null && total != '0.0000';
    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Semantics(
            button: true,
            expanded: open,
            child: InkWell(
              onTap: onToggle,
              child: Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  AppSpacing.compact,
                  AppSpacing.compact,
                  AppSpacing.compact,
                ),
                child: Row(
                  children: [
                    const AppIconCapsule(
                      icon: Icons.schedule,
                      size: AppSizes.rowButton,
                      tone: AppStatusTone.expense,
                    ),
                    const SizedBox(width: AppSpacing.compact),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            '${items.length} geciken kayıt',
                            style: theme.textTheme.titleSmall,
                          ),
                          const SizedBox(height: AppGaps.rowText),
                          Text(
                            label,
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: surfaces.inkMuted,
                            ),
                          ),
                        ],
                      ),
                    ),
                    if (showTotal)
                      AppMoneyText(
                        amount: total,
                        currency: 'TRY',
                        size: AppMoneySize.row,
                        style: const TextStyle(fontWeight: FontWeight.w700),
                      ),
                    Icon(
                      open ? Icons.expand_less : Icons.expand_more,
                      color: surfaces.inkMuted,
                    ),
                  ],
                ),
              ),
            ),
          ),
          if (open)
            for (final item in items) ...[
              const Divider(
                height: 1,
                indent: AppSpacing.medium,
                endIndent: AppSpacing.medium,
              ),
              _PlannedRow(
                key: ValueKey('earlier:${item.listKey}'),
                activity: item,
                showScope: showScope,
                withDate: true,
                onAct: onAct,
              ),
            ],
        ],
      ),
    );
  }
}

/// Gün başlığı: tarih ve gerekiyorsa `Gecikmiş` (kırmızı) ya da `Bugün`.
class _DayHeading extends StatelessWidget {
  const _DayHeading({required this.date, required this.today});

  final String date;
  final DateTime today;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final due = DateTime.tryParse(date);
    final tag = due == null
        ? null
        : due.isBefore(today)
        ? ('Gecikmiş', colors.expense)
        : DateUtils.isSameDay(due, today)
        ? ('Bugün', surfaces.inkMuted)
        : null;
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppPageInsets.edge,
        AppSpacing.step20,
        AppPageInsets.edge,
        AppSpacing.small,
      ),
      child: Semantics(
        header: true,
        child: Text.rich(
          TextSpan(
            children: [
              TextSpan(
                text: DateText.dayMonth(date),
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: AppTextSizes.compact,
                  height: 1.3,
                ),
              ),
              if (tag != null) ...[
                const WidgetSpan(child: SizedBox(width: AppSpacing.small)),
                TextSpan(
                  text: tag.$1,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontWeight: FontWeight.w600,
                    height: 1.35,
                    color: tag.$2,
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

/// Bir vade: simge, ad ve tutar; altında tür; en altta paraya etkisi ve kısa
/// eylem. Satırın tamamı ve eylem aynı hedefi açar.
class _PlannedRow extends StatelessWidget {
  const _PlannedRow({
    required this.activity,
    required this.showScope,
    required this.onAct,
    super.key,
    this.withDate = false,
  });

  final PlannedActivity activity;

  /// `Hepsi` görünümünde karma ekstre gibi kapsam bilgisi yazılır.
  final bool showScope;

  /// Gecikenler kartında satır kendi gününü taşır (gün başlığı yoktur).
  final bool withDate;
  final ValueChanged<PlannedActivity> onAct;

  /// Satırın bir eylemi var mı. Kişinin vadeleri ve eski yükümlülükler
  /// borç-alacak ekranlarıyla bağlanana kadar bilgi satırıdır.
  static bool hasAction(PlannedActivity a) =>
      a.plannedKind != PlannedKind.counterpartyPayable &&
      a.plannedKind != PlannedKind.counterpartyReceivable &&
      a.plannedKind != PlannedKind.payableObligation &&
      a.plannedKind != PlannedKind.receivableObligation;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final a = activity;
    final action = hasAction(a) ? _actionLabel(a) : null;
    final kind = [
      _kindLabel(a),
      if (withDate) DateText.dayMonth(a.dueDate),
    ].join(' · ');
    final impact = _impact(a);
    return Semantics(
      container: true,
      button: action != null,
      label: [
        a.displayTitle,
        a.amount == null
            ? AppUnknownAmount.text
            : MoneyText.format(a.amount!, a.currency),
        kind,
        ?impact,
        ?action,
        if (a.attentionCode != null) a.attentionCode!.message,
      ].join('. '),
      excludeSemantics: true,
      child: InkWell(
        onTap: action == null ? null : () => onAct(a),
        child: Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.compact,
            AppSpacing.medium,
            AppSpacing.compact,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  AppIconCapsule(
                    icon: _icon(a),
                    size: AppSizes.rowButton,
                    tone: _tone(a),
                  ),
                  const SizedBox(width: AppSpacing.compact),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Expanded(
                              child: Text(
                                a.displayTitle,
                                style: theme.textTheme.titleSmall,
                              ),
                            ),
                            const SizedBox(width: AppSpacing.small),
                            if (a.amount == null)
                              const AppUnknownAmount()
                            else
                              AppMoneyText(
                                amount: a.amount!,
                                currency: a.currency,
                                size: AppMoneySize.row,
                                style: const TextStyle(
                                  fontWeight: FontWeight.w700,
                                ),
                              ),
                          ],
                        ),
                        const SizedBox(height: AppGaps.rowText),
                        Text(
                          kind,
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: surfaces.inkMuted,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              if (impact != null || action != null) ...[
                const SizedBox(height: AppSpacing.small),
                Wrap(
                  alignment: WrapAlignment.spaceBetween,
                  crossAxisAlignment: WrapCrossAlignment.center,
                  spacing: AppSpacing.small,
                  runSpacing: AppSpacing.xSmall,
                  children: [
                    if (impact != null)
                      Text(
                        impact,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: surfaces.inkMuted,
                        ),
                      ),
                    if (action != null)
                      Text(
                        '$action ›',
                        style: theme.textTheme.bodySmall?.copyWith(
                          fontWeight: FontWeight.w600,
                          color: theme.colorScheme.primary,
                        ),
                      ),
                  ],
                ),
              ],
              if (a.attentionCode != null) ...[
                const SizedBox(height: AppSpacing.xSmall),
                Text(
                  a.attentionCode!.message,
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: theme.colorScheme.error,
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  static bool _isCardPosting(PlannedActivity a) =>
      a.postsToCard || a.plannedKind == PlannedKind.cardInstallment;

  static String _kindLabel(PlannedActivity a) => switch (a.plannedKind) {
    PlannedKind.recurringOccurrence when a.isTax => 'Vergi',
    PlannedKind.recurringOccurrence when a.effect == ActivityEffect.income =>
      'Tekrarlayan gelir',
    PlannedKind.recurringOccurrence when a.postsToCard => 'Tekrarlayan harcama',
    PlannedKind.recurringOccurrence => 'Tekrarlayan ödeme',
    PlannedKind.cardStatement =>
      a.isMixedStatement ? 'Kart ekstresi · Karma' : 'Kart ekstresi',
    _ => a.plannedKind.label,
  };

  /// Paraya etkisi ve kaynağı. Kaynak adı ek almadan yazılır
  /// (`Ziraat vadesiz · Hesaptan ödenecek`).
  static String? _impact(PlannedActivity a) {
    final source = a.sourceName;
    String withSource(String text) => source == null ? text : '$source · $text';
    if (a.plannedKind == PlannedKind.cardStatement) return 'Kalan ekstre borcu';
    if (_isCardPosting(a)) return withSource('Karta yansıyacak');
    if (a.isTax && source == null) return 'Ödeme kaynağı ödemede seçilecek';
    if (a.plannedKind == PlannedKind.recurringOccurrence) {
      return a.effect == ActivityEffect.income
          ? withSource('Hesaba girecek')
          : withSource('Hesaptan ödenecek');
    }
    return a.paymentLabel;
  }

  static String _actionLabel(PlannedActivity a) {
    if (a.isTax) return 'Ödedim';
    return switch (a.plannedKind) {
      PlannedKind.recurringOccurrence when a.effect == ActivityEffect.income =>
        'Geliri kaydet',
      PlannedKind.recurringOccurrence when a.postsToCard => 'Harcamayı kaydet',
      PlannedKind.recurringOccurrence => 'Ödeme kaydet',
      PlannedKind.cardStatement => 'Ekstreyi öde',
      PlannedKind.cardInstallment =>
        a.isRealizableNow && !a.needsAttention
            ? 'Karta yansıt'
            : 'Planı görüntüle',
      _ => a.actionKind.label,
    };
  }

  static IconData _icon(PlannedActivity a) => switch (a.plannedKind) {
    PlannedKind.recurringOccurrence when a.isTax => Icons.receipt_long_outlined,
    PlannedKind.recurringOccurrence when a.effect == ActivityEffect.income =>
      Icons.south_west,
    PlannedKind.recurringOccurrence => Icons.receipt_outlined,
    PlannedKind.cardInstallment ||
    PlannedKind.cardStatement => Icons.credit_card,
    PlannedKind.debtInstallment ||
    PlannedKind.receivableInstallment => Icons.handshake_outlined,
    PlannedKind.payableObligation ||
    PlannedKind.counterpartyPayable ||
    PlannedKind.counterpartyReceivable ||
    PlannedKind.receivableObligation => Icons.person_outline,
  };

  static AppStatusTone _tone(PlannedActivity a) {
    if (a.effect == ActivityEffect.income) return AppStatusTone.income;
    if (_isCardPosting(a) || a.plannedKind == PlannedKind.cardStatement) {
      return AppStatusTone.neutral;
    }
    if (!a.isPaymentObligation) return AppStatusTone.income;
    return AppStatusTone.expense;
  }
}

/// Uygulanan tür etiketi: kapsül ve ✕; dokununca tür kalkar, dönem kalır.
class _ActiveTypeChip extends StatelessWidget {
  const _ActiveTypeChip({required this.label, required this.onRemove});
  final String label;
  final VoidCallback onRemove;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      label: '$label filtresini kaldır',
      excludeSemantics: true,
      child: Material(
        color: theme.colorScheme.primaryContainer,
        shape: const StadiumBorder(),
        child: InkWell(
          customBorder: const StadiumBorder(),
          onTap: onRemove,
          child: ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 32),
            child: Padding(
              padding: const EdgeInsets.only(
                left: AppSpacing.compact,
                right: AppSpacing.step6,
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    label,
                    style: theme.textTheme.bodySmall?.copyWith(
                      fontSize: AppTextSizes.caption,
                      fontWeight: FontWeight.w600,
                      height: 1,
                      color: surfaces.ink,
                    ),
                  ),
                  const SizedBox(width: AppSpacing.xSmall),
                  Icon(Icons.close, size: 16, color: surfaces.ink),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Filtre paneli (18-02, 18-04, 18-05): tür menüsü ve dönem. Seçimler
/// taslaktır; `Sonuçları göster` uygular, `Sıfırla` 30 gün ve tüm türlere
/// döner, paneli kapatmak değişiklikleri bırakır. Kapsam ana ekrandadır.
class _PlannedFilterSheet extends StatefulWidget {
  const _PlannedFilterSheet({
    required this.period,
    required this.type,
    required this.today,
  });

  final PlannedPeriodChoice period;
  final PlannedTypeFilter type;
  final DateTime today;

  @override
  State<_PlannedFilterSheet> createState() => _PlannedFilterSheetState();
}

class _PlannedFilterSheetState extends State<_PlannedFilterSheet> {
  late PlannedPeriodChoice _period = widget.period;
  late PlannedTypeFilter _type = widget.type;

  static const _default = PlannedPeriodChoice.days(30);

  static const _typeIcons = {
    PlannedTypeFilter.all: Icons.layers_outlined,
    PlannedTypeFilter.recurring: Icons.event_repeat,
    PlannedTypeFilter.statement: Icons.receipt_long_outlined,
    PlannedTypeFilter.installment: Icons.credit_card,
    PlannedTypeFilter.payable: Icons.north_east,
    PlannedTypeFilter.receivable: Icons.south_west,
    PlannedTypeFilter.tax: Icons.account_balance_outlined,
  };

  Future<void> _pickRange() async {
    final result = await AppDateRangeDialog.show(
      context,
      start: _period.start(widget.today),
      end: _period.end(widget.today),
      maxDays: 366,
    );
    if (result == null) return;
    setState(() => _period = PlannedPeriodChoice.range(result.$1, result.$2));
  }

  String _iso(DateTime d) =>
      '${d.year}-${d.month.toString().padLeft(2, '0')}-'
      '${d.day.toString().padLeft(2, '0')}';

  @override
  Widget build(BuildContext context) {
    final options = [
      for (final type in PlannedTypeFilter.values)
        AppFilterOption(
          key: type == PlannedTypeFilter.all ? null : type.name,
          label: type.label,
          icon: _typeIcons[type]!,
        ),
    ];
    return AppFilterSheet(
      title: 'Planlananları filtrele',
      onReset: _period == _default && _type == PlannedTypeFilter.all
          ? null
          : () => setState(() {
              _period = _default;
              _type = PlannedTypeFilter.all;
            }),
      onSubmit: () => Navigator.of(context).pop((_period, _type)),
      sections: [
        AppFilterSection(
          title: 'Tür',
          child: AppFilterSelectField(
            label: _type.label,
            semanticLabel: 'Tür',
            icon: _typeIcons[_type]!,
            options: options,
            selected: _type == PlannedTypeFilter.all ? null : _type.name,
            onSelected: (key) => setState(
              () => _type = key == null
                  ? PlannedTypeFilter.all
                  : PlannedTypeFilter.values.byName(key),
            ),
          ),
        ),
        AppFilterSection(
          title: 'Dönem',
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.compact),
            child: Column(
              children: [
                for (final days in PlannedPeriodChoice.presets)
                  AppFilterRadioRow(
                    label: 'Önümüzdeki $days gün',
                    selected: _period == PlannedPeriodChoice.days(days),
                    onTap: () => setState(
                      () => _period = PlannedPeriodChoice.days(days),
                    ),
                  ),
                AppFilterRangeRow(
                  label: _period.isRange
                      ? DateText.dayRangeShort(
                          _iso(_period.from!),
                          _iso(_period.to!),
                        )
                      : 'Tarih aralığı seç…',
                  selected: _period.isRange,
                  onTap: _pickRange,
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}
