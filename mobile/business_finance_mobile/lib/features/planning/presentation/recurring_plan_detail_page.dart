import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_section_empty.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_text_action.dart';
import '../data/planning_models.dart';
import '../data/planning_repository.dart';
import 'planning_controller.dart';
import 'realize_day_sheet.dart';

/// Planın tam sayfa ayrıntısı (Codex 07 ve 17. panolar, 19–20 Kasım 2026).
///
/// Üstte planın özeti (ritim, tutar, kaynak); `Açık vadeler` (gecikenler ve
/// sıradaki vade, her birinde o dönemin ödeme eylemi); `Geçmiş ödemeler`
/// (son ikisi, fazlası `Tümü ›`); altta planı duraklatma/sürdürme ve silme.
/// Bir dönemin ödemesi yalnız o dönemi yazar; planın kuralı değişmez.
/// Bölüm başlıkları kartlarla aynı kenarda ve üstteki karttan ayrık.
const _sectionPadding = EdgeInsets.fromLTRB(
  AppPageInsets.edge,
  AppSpacing.large,
  AppPageInsets.edge,
  AppSpacing.small,
);

class RecurringPlanDetailPage extends StatelessWidget {
  const RecurringPlanDetailPage({
    required this.planId,
    required this.controller,
    super.key,
  });

  final String planId;
  final PlanningController controller;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: AnimatedBuilder(
        animation: controller,
        builder: (context, _) {
          final snapshot = controller.snapshot;
          final plan = snapshot?.recurringTransactions
              .where((item) => item.id == planId)
              .firstOrNull;
          final view = snapshot == null || plan == null
              ? null
              : RecurringPlanView.of(plan, snapshot, controller.asOfDate);
          return Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AppPageHeader(
                title: view?.title ?? 'Plan',
                onBack: () => Navigator.maybePop(context),
              ),
              if (controller.isLoading && snapshot != null)
                const LinearProgressIndicator(),
              Expanded(child: _body(context, snapshot, view)),
            ],
          );
        },
      ),
    ),
  );

  Widget _body(
    BuildContext context,
    PlanningSnapshot? snapshot,
    RecurringPlanView? view,
  ) {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (snapshot == null) {
      return controller.errorMessage != null
          ? AppErrorView(
              message: controller.errorMessage!,
              onRetry: controller.load,
            )
          : const AppLoadingView(message: 'Plan yükleniyor');
    }
    if (view == null) {
      return const AppErrorView(message: 'Plan artık bulunamıyor.');
    }
    final history = view.history;
    return RefreshIndicator(
      onRefresh: controller.load,
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
              if (controller.errorMessage != null)
                Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppPageInsets.edge,
                    0,
                    AppPageInsets.edge,
                    AppSpacing.small,
                  ),
                  child: Text(
                    controller.errorMessage!,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Theme.of(context).colorScheme.error,
                    ),
                  ),
                ),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppPageInsets.edge,
                  AppSpacing.small,
                  AppPageInsets.edge,
                  0,
                ),
                child: _SummaryCard(view: view),
              ),
              const AppSectionHeader(
                title: 'Açık vadeler',
                padding: _sectionPadding,
              ),
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppPageInsets.edge,
                ),
                child: view.open.isEmpty
                    ? AppSectionEmpty(
                        view.plan.isPaused
                            ? 'Plan duraklatıldı; yeni vade oluşmuyor.'
                            : 'Bekleyen vade yok.',
                      )
                    : Column(
                        children: [
                          for (final (index, due) in view.open.indexed) ...[
                            if (index > 0) const SizedBox(height: AppGaps.card),
                            _OpenDueCard(
                              view: view,
                              due: due,
                              enabled: !controller.isSubmitting,
                              onPay: () => _pay(context, view, due),
                            ),
                          ],
                        ],
                      ),
              ),
              AppSectionHeader(
                title: 'Geçmiş ödemeler',
                padding: _sectionPadding,
                trailing: history.length > 2
                    ? AppTextAction(
                        label: 'Tümü',
                        trailingIcon: Icons.chevron_right,
                        onPressed: () => Navigator.of(context).push(
                          MaterialPageRoute<void>(
                            builder: (_) => _HistoryPage(
                              title: view.title,
                              history: history,
                            ),
                          ),
                        ),
                      )
                    : null,
              ),
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppPageInsets.edge,
                ),
                child: history.isEmpty
                    ? const AppSectionEmpty('Henüz ödeme yok.')
                    : _HistoryCard(items: history.take(2).toList()),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppPageInsets.edge,
                  AppSpacing.large,
                  AppPageInsets.edge,
                  0,
                ),
                child: _ManageActions(
                  view: view,
                  enabled: !controller.isSubmitting,
                  onToggle: () => controller.setRecurringActive(
                    view.plan.id,
                    !view.plan.isActive,
                  ),
                  onDelete: () => _delete(context, view),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  /// Tek dönemin ödemesi: gün sorulur (bugünle açılır), önceki dönem
  /// bekliyorsa sıra atlama onayı istenir.
  Future<void> _pay(
    BuildContext context,
    RecurringPlanView view,
    RecurringOpenDue due,
  ) async {
    final paidOn = await showRealizeDaySheet(
      context,
      title: view.title,
      amount: due.amount,
      currency: view.plan.currency,
      dueDate: due.date,
      sourceName: view.sourceName,
      isIncome: view.isIncome,
    );
    if (paidOn == null || !context.mounted) return;
    Future<bool> run({bool allow = false}) => due.occurrenceId == null
        ? controller.realizeDue(
            view.plan.id,
            due.date,
            paidOn: paidOn,
            allowEarlierPending: allow,
          )
        : controller.realizeOccurrence(
            due.occurrenceId!,
            paidOn: paidOn,
            allowEarlierPending: allow,
          );
    var done = await run();
    if (!done && controller.earlierItemPending && context.mounted) {
      final confirmed = await confirmSkippingEarlierItem(
        context,
        title: view.title,
        dueDate: due.date,
        paidOn: paidOn,
        pendingDate: controller.earlierPendingDate,
        pendingCount: controller.earlierPendingCount,
      );
      if (!confirmed || !context.mounted) return;
      done = await run(allow: true);
    }
    if (done && context.mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            '${DateText.dayMonth(due.date)} ${view.isIncome ? 'geliri' : 'ödemesi'} kaydedildi.',
          ),
        ),
      );
    }
  }

  Future<void> _delete(BuildContext context, RecurringPlanView view) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.delete_outline,
      destructive: true,
      highlight: view.title,
      title: 'Plan silinsin mi?',
      message: 'Plan ve bekleyen vadeleri kaldırılır.',
      confirmLabel: 'Sil',
    );
    if (!confirmed || !context.mounted) return;
    final deleted = await controller.deleteRecurring(view.plan.id);
    if (deleted && context.mounted) Navigator.of(context).maybePop();
  }
}

/// Planlananlar'ın `Planı görüntüle`si gibi dışarıdan açılış: sayfa kendi
/// verisini okur.
class RecurringPlanDetailHost extends StatefulWidget {
  const RecurringPlanDetailHost({
    required this.planId,
    required this.repository,
    this.financialDataChanges,
    super.key,
  });

  final String planId;
  final PlanningRepositoryContract repository;
  final FinancialDataChanges? financialDataChanges;

  @override
  State<RecurringPlanDetailHost> createState() =>
      _RecurringPlanDetailHostState();
}

class _RecurringPlanDetailHostState extends State<RecurringPlanDetailHost> {
  late final PlanningController _controller = PlanningController(
    widget.repository,
    financialDataChanges: widget.financialDataChanges,
  )..load();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) =>
      RecurringPlanDetailPage(planId: widget.planId, controller: _controller);
}

/// Açık bir vade: üretilmiş bekleyen kalem ya da henüz üretilmemiş sıradaki
/// tarih (o zaman [occurrenceId] boştur ve plan + vade ile gerçekleşir).
class RecurringOpenDue {
  const RecurringOpenDue({
    required this.date,
    required this.amount,
    this.occurrenceId,
  });

  final String date;
  final String amount;
  final String? occurrenceId;
}

/// Ayrıntının okuması: plan, kaynağı, açık vadeleri ve geçmişi.
class RecurringPlanView {
  const RecurringPlanView({
    required this.plan,
    required this.title,
    required this.categoryName,
    required this.sourceName,
    required this.open,
    required this.history,
    required this.today,
  });

  factory RecurringPlanView.of(
    RecurringTransactionItem plan,
    PlanningSnapshot snapshot,
    DateTime today,
  ) {
    final mine = snapshot.occurrences
        .where((item) => item.recurringTransactionId == plan.id)
        .toList();
    final pending = mine.where((item) => item.canRealize).toList()
      ..sort((a, b) => a.scheduledDate.compareTo(b.scheduledDate));
    final open = <RecurringOpenDue>[
      for (final item in pending)
        RecurringOpenDue(
          date: item.scheduledDate,
          amount: item.amount,
          occurrenceId: item.id,
        ),
    ];
    // Henüz üretilmemiş sıradaki vade: yalnız çalışan planda ve bekleyen
    // kalemlerle aynı gün değilse. Duraklatılan plan vade üretmez.
    final next = plan.isActive ? plan.nextOccurrenceDate : null;
    if (next != null && pending.every((item) => item.scheduledDate != next)) {
      open.add(RecurringOpenDue(date: next, amount: plan.amount));
    }
    // Gecikenlerin hepsi ve gelecekteki yalnız ilk vade (07. pano).
    final todayIso = _iso(today);
    final firstFuture = open.indexWhere(
      (due) => due.date.compareTo(todayIso) >= 0,
    );
    final shown = firstFuture < 0 ? open : open.sublist(0, firstFuture + 1);
    final history = mine.where((item) => item.status == 'realized').toList()
      ..sort((a, b) => b.scheduledDate.compareTo(a.scheduledDate));
    final category = snapshot.categories
        .where((item) => item.id == plan.categoryId)
        .firstOrNull
        ?.name;
    final source = [...snapshot.accounts, ...snapshot.creditCards]
        .where((item) => item.id == (plan.accountId ?? plan.creditCardId))
        .firstOrNull;
    return RecurringPlanView(
      plan: plan,
      title:
          plan.description ??
          category ??
          (plan.kind == 'income' ? 'Düzenli gelir' : 'Düzenli gider'),
      categoryName: category,
      sourceName: source?.name,
      open: shown,
      history: history,
      today: today,
    );
  }

  final RecurringTransactionItem plan;
  final String title;
  final String? categoryName;
  final String? sourceName;
  final List<RecurringOpenDue> open;
  final List<RecurringOccurrenceItem> history;
  final DateTime today;

  bool get isIncome => plan.kind == 'income';
  bool get isCard => plan.creditCardId != null;

  /// Ritim cümlesi: `Her ayın 5'i`, `Her hafta Pazartesi`, `Her yıl 5 Mart`.
  String get rhythm {
    final start = DateTime.tryParse(plan.startDate);
    if (start == null) return plan.frequency;
    final day = start.day;
    return switch (plan.frequency) {
      'daily' => 'Her gün',
      'weekly' => 'Her hafta ${DateText.weekdays[start.weekday - 1]}',
      'monthly' => 'Her ayın $day${_suffix(day)}',
      'quarterly' => 'Üç ayda bir, ayın $day${_suffix(day)}',
      'yearly' => 'Her yıl ${DateText.dayMonth(plan.startDate)}',
      'selected-months' => 'Seçilen aylarda, ayın $day${_suffix(day)}',
      _ => plan.frequency,
    };
  }

  /// Ek, sayının okunuşuna göre: 1'i, 3'ü, 5'i, 6'sı, 9'u, 10'u, 30'u.
  static String _suffix(int day) {
    const byLast = {
      0: "'u",
      1: "'i",
      2: "'si",
      3: "'ü",
      4: "'ü",
      5: "'i",
      6: "'sı",
      7: "'si",
      8: "'i",
      9: "'u",
    };
    if (day == 10 || day == 30) return "'u";
    if (day == 20) return "'si";
    return byLast[day % 10]!;
  }

  static String _iso(DateTime d) =>
      '${d.year}-${d.month.toString().padLeft(2, '0')}-'
      '${d.day.toString().padLeft(2, '0')}';
}

/// Planın özeti: koyu yüzeyde ritim ve tutar; altında kaynak, yön ve durum.
class _SummaryCard extends StatelessWidget {
  const _SummaryCard({required this.view});

  final RecurringPlanView view;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final onInk = theme.colorScheme.onInverseSurface;
    final plan = view.plan;
    final limit = plan.occurrenceLimit;
    final details = [
      ?view.sourceName,
      view.isIncome
          ? 'Gelir'
          : view.isCard
          ? 'Karta yansır'
          : 'Gider',
      if (plan.isPaused) 'Duraklatıldı',
      if (plan.isCompleted) 'Tamamlandı',
      if (limit != null) '${plan.generatedOccurrenceCount} / $limit tekrar',
      if (plan.endDate != null) '${DateText.dayMonthYear(plan.endDate!)} bitiş',
    ];
    return Container(
      padding: const EdgeInsets.all(AppSpacing.medium),
      decoration: BoxDecoration(
        color: surfaces.ink,
        borderRadius: BorderRadius.circular(AppRadius.card),
      ),
      child: MergeSemantics(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    view.rhythm,
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontSize: AppTextSizes.compact,
                      color: onInk,
                    ),
                  ),
                ),
                // Koyu zeminde para bileşeni kendi rengini kullanıp
                // kayboluyordu; tutar aynı biçimle düz yazıdır.
                Text(
                  MoneyText.format(plan.amount, plan.currency),
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontSize: AppTextSizes.row,
                    fontWeight: FontWeight.w700,
                    fontFeatures: AppTypography.tabularFigures,
                    color: onInk,
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.small),
            Text(
              details.join(' · '),
              style: theme.textTheme.bodySmall?.copyWith(
                color: onInk.withValues(alpha: 0.8),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Açık vade kartı (07. pano): gün ve tutar; altında tür ve durum; en altta
/// kaynak ve kısa eylem. Kartın tamamı eylemi açar.
class _OpenDueCard extends StatelessWidget {
  const _OpenDueCard({
    required this.view,
    required this.due,
    required this.enabled,
    required this.onPay,
  });

  final RecurringPlanView view;
  final RecurringOpenDue due;
  final bool enabled;
  final VoidCallback onPay;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    final date = DateTime.tryParse(due.date);
    final today = view.today;
    final late = date != null && date.isBefore(today);
    final isToday = date != null && DateUtils.isSameDay(date, today);
    final kind = view.isIncome
        ? 'Tekrarlayan gelir'
        : view.isCard
        ? 'Tekrarlayan harcama'
        : 'Tekrarlayan ödeme';
    final impact = [
      ?view.sourceName,
      view.isIncome
          ? 'Hesaba girecek'
          : view.isCard
          ? 'Karta yansıyacak'
          : 'Hesaptan ödenecek',
    ].join(' · ');
    final action = view.isIncome
        ? 'Geliri kaydet'
        : view.isCard
        ? 'Harcamayı kaydet'
        : 'Ödeme kaydet';
    final small = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    return Semantics(
      button: true,
      label: [
        DateText.dayMonthYear(due.date),
        MoneyText.format(due.amount, view.plan.currency),
        kind,
        if (late) 'Gecikti',
        if (isToday) 'Bugün',
        impact,
        action,
      ].join('. '),
      excludeSemantics: true,
      child: AppCard(
        padding: EdgeInsets.zero,
        child: InkWell(
          onTap: enabled ? onPay : null,
          child: Padding(
            padding: const EdgeInsets.all(AppSpacing.medium),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    AppIconCapsule(
                      icon: view.isIncome
                          ? Icons.south_west
                          : AppFinanceIcons.forCategory(
                              view.categoryName ?? '',
                            ),
                      size: AppSizes.rowButton,
                    ),
                    const SizedBox(width: AppSpacing.compact),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          Row(
                            children: [
                              Expanded(
                                child: Text(
                                  DateText.dayMonth(due.date),
                                  style: theme.textTheme.titleSmall,
                                ),
                              ),
                              AppMoneyText(
                                amount: due.amount,
                                currency: view.plan.currency,
                                size: AppMoneySize.row,
                                style: const TextStyle(
                                  fontWeight: FontWeight.w700,
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: AppGaps.rowText),
                          Row(
                            children: [
                              Expanded(child: Text(kind, style: small)),
                              if (late)
                                Text(
                                  'Gecikti',
                                  style: small?.copyWith(color: colors.expense),
                                )
                              else if (isToday)
                                Text('Bugün', style: small),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: AppSpacing.small),
                Wrap(
                  alignment: WrapAlignment.spaceBetween,
                  crossAxisAlignment: WrapCrossAlignment.center,
                  spacing: AppSpacing.small,
                  runSpacing: AppSpacing.xSmall,
                  children: [
                    Text(impact, style: small),
                    Text(
                      '$action ›',
                      style: theme.textTheme.bodySmall?.copyWith(
                        fontWeight: FontWeight.w600,
                        color: colors.neutral,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _HistoryCard extends StatelessWidget {
  const _HistoryCard({required this.items});

  final List<RecurringOccurrenceItem> items;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        children: [
          for (final (index, item) in items.indexed) ...[
            if (index > 0)
              const Divider(
                height: 1,
                indent: AppSpacing.medium,
                endIndent: AppSpacing.medium,
              ),
            MergeSemantics(
              child: ConstrainedBox(
                constraints: const BoxConstraints(minHeight: AppSizes.button),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                    vertical: AppSpacing.small,
                  ),
                  child: Row(
                    children: [
                      Expanded(
                        child: Text(
                          DateText.dayMonthYear(item.scheduledDate),
                          style: theme.textTheme.bodyMedium,
                        ),
                      ),
                      AppMoneyText(
                        amount: item.amount,
                        currency: item.currency,
                        size: AppMoneySize.row,
                        style: const TextStyle(fontWeight: FontWeight.w600),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class _HistoryPage extends StatelessWidget {
  const _HistoryPage({required this.title, required this.history});

  final String title;
  final List<RecurringOccurrenceItem> history;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Geçmiş ödemeler',
            onBack: () => Navigator.maybePop(context),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppPageInsets.edge,
              0,
              AppPageInsets.edge,
              AppSpacing.small,
            ),
            child: Text(
              title,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: AppSurfaces.of(context).inkMuted,
              ),
            ),
          ),
          Expanded(
            child: ListView(
              padding: AppPageInsets.fullBleedList,
              children: [
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppPageInsets.edge,
                  ),
                  child: _HistoryCard(items: history),
                ),
              ],
            ),
          ),
        ],
      ),
    ),
  );
}

/// Planın yönetimi: duraklat / sürdür ve (geçmişi olmayan planda) sil.
/// Metinli, görünür eylemler (10. pano); çöp simgesiyle gizlenmez.
class _ManageActions extends StatelessWidget {
  const _ManageActions({
    required this.view,
    required this.enabled,
    required this.onToggle,
    required this.onDelete,
  });

  final RecurringPlanView view;
  final bool enabled;
  final VoidCallback onToggle;
  final VoidCallback onDelete;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final plan = view.plan;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (!plan.isCompleted) ...[
          OutlinedButton.icon(
            onPressed: enabled ? onToggle : null,
            icon: Icon(
              plan.isActive
                  ? Icons.pause_circle_outline
                  : Icons.play_circle_outline,
            ),
            label: Text(plan.isActive ? 'Planı duraklat' : 'Planı sürdür'),
          ),
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.xSmall),
            child: Text(
              plan.isActive
                  ? 'Duraklatılan plan yeni vade üretmez; bekleyen vadeleri '
                        'listelerden kalkar. Geçmiş ödemeler kalır.'
                  : 'Plan yeniden vade üretmeye başlar.',
              style: theme.textTheme.bodySmall?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
          ),
        ],
        // Geçmişi olan plan silinemez (sunucu da reddeder); duraklatılır.
        if (view.history.isEmpty) ...[
          const SizedBox(height: AppSpacing.compact),
          TextButton.icon(
            onPressed: enabled ? onDelete : null,
            style: TextButton.styleFrom(
              foregroundColor: theme.colorScheme.error,
            ),
            icon: const Icon(Icons.delete_outline),
            label: const Text('Planı sil'),
          ),
        ],
      ],
    );
  }
}
