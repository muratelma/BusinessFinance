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
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_first_use_view.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_no_results_view.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/planning_models.dart';
import '../data/planning_repository.dart';
import 'planning_controller.dart';
import 'planning_page.dart';

/// Tekrarlayanlar (Codex 09, 10, 11. panolar; 19–20 Kasım 2026 kararları).
///
/// Planların kendi sayfası: arama, plan başına tek satır (ad ve tutar;
/// altında yön ve sıklık; sağda en eski gecikme ya da sıradaki vade) ve en
/// altta duraklatılan ve biten planlara giriş. Plan tanımı para üretmez;
/// satır yalnız planı anlatır.
class RecurringPlansPage extends StatefulWidget {
  const RecurringPlansPage({
    required this.repository,
    this.financialDataChanges,
    super.key,
  });

  final PlanningRepositoryContract repository;
  final FinancialDataChanges? financialDataChanges;

  @override
  State<RecurringPlansPage> createState() => _RecurringPlansPageState();
}

class _RecurringPlansPageState extends State<RecurringPlansPage> {
  late final PlanningController _controller = PlanningController(
    widget.repository,
    financialDataChanges: widget.financialDataChanges,
  );

  @override
  void initState() {
    super.initState();
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
        builder: (context, _) {
          final snapshot = _controller.snapshot;
          return Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AppPageHeader(
                title: 'Tekrarlayanlar',
                onBack: () => Navigator.maybePop(context),
                actions: [
                  if (snapshot != null &&
                      snapshot.recurringTransactions.isNotEmpty)
                    IconButton(
                      tooltip: 'Tekrarlayan plan ekle',
                      onPressed: _controller.isSubmitting
                          ? null
                          : () => showRecurringPlanForm(
                              context,
                              snapshot: snapshot,
                              controller: _controller,
                            ),
                      icon: const Icon(Icons.add),
                    ),
                ],
              ),
              if (_controller.isLoading && snapshot != null)
                const LinearProgressIndicator(),
              Expanded(child: _body(snapshot)),
            ],
          );
        },
      ),
    ),
  );

  Widget _body(PlanningSnapshot? snapshot) {
    if (_controller.unauthorized) return const AppUnauthorizedView();
    if (snapshot == null) {
      return _controller.errorMessage != null
          ? AppErrorView(
              message: _controller.errorMessage!,
              onRetry: _controller.load,
            )
          : const AppLoadingView(message: 'Planlar yükleniyor');
    }
    return RefreshIndicator(
      onRefresh: _controller.load,
      child: Align(
        alignment: Alignment.topCenter,
        child: ConstrainedBox(
          constraints: const BoxConstraints(
            maxWidth: AppBreakpoints.contentMaxWidth,
          ),
          child: RecurringPlanList(
            snapshot: snapshot,
            controller: _controller,
            showAddButton: false,
          ),
        ),
      ),
    );
  }
}

/// Planların listesi; Tekrarlayanlar sayfası ve Planlama'nın sekmesi ortak.
class RecurringPlanList extends StatefulWidget {
  const RecurringPlanList({
    required this.snapshot,
    required this.controller,
    super.key,
    this.showAddButton = true,
  });

  final PlanningSnapshot snapshot;
  final PlanningController controller;

  /// Kendi sayfasında ekleme başlıktaki `+`; sekmede listenin üstünde düğme.
  final bool showAddButton;

  @override
  State<RecurringPlanList> createState() => _RecurringPlanListState();
}

class _RecurringPlanListState extends State<RecurringPlanList> {
  final _search = TextEditingController();
  String _query = '';

  @override
  void dispose() {
    _search.dispose();
    super.dispose();
  }

  PlanningSnapshot get _snapshot => widget.snapshot;
  DateTime get _today => widget.controller.asOfDate;

  @override
  Widget build(BuildContext context) {
    final plans = _snapshot.recurringTransactions;
    if (plans.isEmpty) {
      // İlk kullanım: boş arama ve ikinci bir ekleme düğmesi çizilmez (10).
      return ListView(
        padding: AppPageInsets.underTabs,
        children: [
          AppFirstUseView(
            title: 'Henüz tekrarlayan plan yok',
            message: 'Kira, maaş ve aboneliklerini ekle.',
            icon: Icons.event_repeat,
            actionLabel: 'Tekrarlayan plan ekle',
            onAction: widget.controller.isSubmitting ? null : () => _add(),
          ),
        ],
      );
    }
    final rows = [
      for (final plan in plans)
        RecurringPlanRowData.of(plan, _snapshot, _today),
    ];
    final running = rows.where((row) => row.isRunning).toList()..sort(_order);
    final stopped = rows.where((row) => !row.isRunning).toList()
      ..sort((a, b) => _key(a.title).compareTo(_key(b.title)));
    final query = _key(_query.trim());
    final matches =
        query.isEmpty
              ? running
              : rows.where((row) => _key(row.title).contains(query)).toList()
          ..sort(_order);

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: AppPageInsets.aboveBar(context, AppPageInsets.fullBleedList),
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppPageInsets.edge,
            AppSpacing.small,
            AppPageInsets.edge,
            AppSpacing.compact,
          ),
          child: _SearchField(
            controller: _search,
            onChanged: (value) => setState(() => _query = value),
            onClear: () => setState(() {
              _search.clear();
              _query = '';
            }),
          ),
        ),
        if (widget.showAddButton && query.isEmpty)
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppPageInsets.edge,
              0,
              AppPageInsets.edge,
              AppSpacing.compact,
            ),
            child: FilledButton.icon(
              onPressed: widget.controller.isSubmitting ? null : () => _add(),
              icon: const Icon(Icons.add),
              label: const Text('Tekrarlayan plan ekle'),
            ),
          ),
        if (matches.isEmpty && query.isNotEmpty)
          // Sonuçsuz arama: yazılan adla plan formu açılabilir; yalnız ad
          // taşınır ve düzenlenebilir (10. pano).
          AppNoResultsView(
            reason: AppNoResultsReason.filter,
            title: '“${_query.trim()}” adında plan yok',
            actionLabel: '“${_query.trim()}” adıyla plan ekle',
            onAction: () => _add(name: _query.trim()),
          )
        else if (matches.isNotEmpty)
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
            child: AppCard(
              padding: EdgeInsets.zero,
              child: Column(
                children: [
                  for (final (index, row) in matches.indexed) ...[
                    if (index > 0)
                      const Divider(
                        height: 1,
                        indent: AppIconCapsule.rowInset - AppSpacing.small,
                        endIndent: AppSpacing.medium,
                      ),
                    RecurringPlanRow(row: row, onTap: () => _open(row)),
                  ],
                ],
              ),
            ),
          ),
        if (query.isEmpty && stopped.isNotEmpty)
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppPageInsets.edge,
              AppSpacing.compact,
              AppPageInsets.edge,
              0,
            ),
            child: _StoppedLink(
              count: stopped.length,
              onTap: () => Navigator.of(context).push(
                MaterialPageRoute<void>(
                  builder: (_) => _StoppedPlansPage(
                    controller: widget.controller,
                    onOpen: _open,
                  ),
                ),
              ),
            ),
          ),
      ],
    );
  }

  Future<void> _add({String? name}) => showRecurringPlanForm(
    context,
    snapshot: _snapshot,
    controller: widget.controller,
    initialName: name,
  );

  Future<void> _open(RecurringPlanRowData row) => showRecurringPlanActions(
    context,
    item: row.plan,
    title: row.title,
    controller: widget.controller,
  );

  /// Önce gecikenler (en eskisi önce), sonra sıradaki vade; eşitlikte ad.
  static int _order(RecurringPlanRowData a, RecurringPlanRowData b) {
    final byDate = (a.date ?? '9999').compareTo(b.date ?? '9999');
    return byDate != 0 ? byDate : _key(a.title).compareTo(_key(b.title));
  }

  /// Aramada harf büyüklüğü ve Türkçe i/ı farkı ad farkı değildir.
  static String _key(String value) =>
      value.replaceAll('İ', 'i').replaceAll('I', 'ı').toLowerCase();
}

/// Satırın verisi: planın adı, gösterilecek vade ve tutarı.
class RecurringPlanRowData {
  const RecurringPlanRowData({
    required this.plan,
    required this.title,
    required this.categoryName,
    required this.date,
    required this.amount,
    required this.isOverdue,
    required this.isToday,
    required this.isRunning,
  });

  /// En eski açık vade (üretilmiş bekleyen kalem ya da henüz üretilmemiş
  /// sıradaki tarih). Gecikme vadeden okunur; oluşturma gününden ya da son
  /// ödemeden tahmin edilmez (19 Kasım kararı).
  factory RecurringPlanRowData.of(
    RecurringTransactionItem plan,
    PlanningSnapshot snapshot,
    DateTime today,
  ) {
    RecurringOccurrenceItem? earliest;
    for (final occurrence in snapshot.occurrences) {
      if (occurrence.recurringTransactionId != plan.id ||
          !occurrence.canRealize) {
        continue;
      }
      if (earliest == null ||
          occurrence.scheduledDate.compareTo(earliest.scheduledDate) < 0) {
        earliest = occurrence;
      }
    }
    final projected = plan.isPaused ? null : plan.nextOccurrenceDate;
    final usesOccurrence =
        earliest != null &&
        (projected == null || earliest.scheduledDate.compareTo(projected) <= 0);
    final date = usesOccurrence ? earliest.scheduledDate : projected;
    final due = date == null ? null : DateTime.tryParse(date);
    final category = snapshot.categories
        .where((item) => item.id == plan.categoryId)
        .firstOrNull
        ?.name;
    final running = plan.isActive || (plan.isCompleted && earliest != null);
    return RecurringPlanRowData(
      plan: plan,
      title: plan.description ?? category ?? _kindTitle(plan.kind),
      categoryName: category,
      date: running ? date : null,
      // Daha eski bekleyen kalemin tutarı plan tarifesinden farklı olabilir;
      // satır gösterdiği vadenin tutarını yazar (19 Kasım kararı).
      amount: usesOccurrence ? earliest.amount : plan.amount,
      isOverdue: running && due != null && due.isBefore(today),
      isToday: running && due != null && DateUtils.isSameDay(due, today),
      isRunning: running,
    );
  }

  final RecurringTransactionItem plan;
  final String title;
  final String? categoryName;
  final String? date;
  final String amount;
  final bool isOverdue;
  final bool isToday;
  final bool isRunning;

  bool get isIncome => plan.kind == 'income';

  /// `Aylık gider`, `Haftalık gelir`, `Üç ayda bir gider`.
  String get rhythm =>
      '${_frequency(plan.frequency)} ${isIncome ? 'gelir' : 'gider'}';

  static String _kindTitle(String kind) =>
      kind == 'income' ? 'Düzenli gelir' : 'Düzenli gider';

  static String _frequency(String frequency) => switch (frequency) {
    'daily' => 'Günlük',
    'weekly' => 'Haftalık',
    'monthly' => 'Aylık',
    'quarterly' => 'Üç ayda bir',
    'yearly' => 'Yıllık',
    'selected-months' => 'Seçilen aylarda',
    _ => frequency,
  };
}

/// Plan başına tek satır (11. pano): sol üst ad, sağ üst tutar; sol alt yön
/// ve sıklık, sağ alt vade (`Vade 13 Şub`, `Bugün`, kırmızı `5 Şub gecikti`).
class RecurringPlanRow extends StatelessWidget {
  const RecurringPlanRow({required this.row, required this.onTap, super.key});

  final RecurringPlanRowData row;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    final plan = row.plan;
    final status = !row.isRunning
        ? (plan.isPaused ? 'Duraklatıldı' : 'Tamamlandı')
        : row.date == null
        ? 'Tamamlandı'
        : row.isOverdue
        ? '${DateText.dayShortMonth(row.date!)} gecikti'
        : row.isToday
        ? 'Bugün'
        : 'Vade ${DateText.dayShortMonth(row.date!)}';
    final small = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    return Semantics(
      button: true,
      label: [
        row.title,
        MoneyText.format(row.amount, plan.currency),
        row.rhythm,
        status,
      ].join('. '),
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.compact,
            AppSpacing.medium,
            AppSpacing.compact,
          ),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              AppIconCapsule(
                icon: row.isIncome
                    ? Icons.south_west
                    : AppFinanceIcons.forCategory(row.categoryName ?? ''),
                size: AppSizes.rowButton,
                // Gider simgesi gri kalır: gecikmeyi kırmızı tarih söyler;
                // her satır kırmızı olunca hiçbiri öne çıkmıyordu.
                tone: !row.isRunning
                    ? AppStatusTone.cancelled
                    : row.isIncome
                    ? AppStatusTone.income
                    : null,
              ),
              const SizedBox(width: AppSpacing.compact),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Text(
                            row.title,
                            style: theme.textTheme.titleSmall,
                          ),
                        ),
                        const SizedBox(width: AppSpacing.small),
                        AppMoneyText(
                          amount: row.amount,
                          currency: plan.currency,
                          size: AppMoneySize.row,
                          style: const TextStyle(fontWeight: FontWeight.w700),
                        ),
                      ],
                    ),
                    const SizedBox(height: AppGaps.rowText),
                    Row(
                      children: [
                        Expanded(child: Text(row.rhythm, style: small)),
                        const SizedBox(width: AppSpacing.small),
                        Text(
                          status,
                          style: row.isOverdue
                              ? small?.copyWith(color: colors.expense)
                              : small,
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _SearchField extends StatelessWidget {
  const _SearchField({
    required this.controller,
    required this.onChanged,
    required this.onClear,
  });

  final TextEditingController controller;
  final ValueChanged<String> onChanged;
  final VoidCallback onClear;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final theme = Theme.of(context);
    final border = OutlineInputBorder(
      borderRadius: BorderRadius.circular(AppRadius.field),
      borderSide: BorderSide.none,
    );
    return TextField(
      controller: controller,
      onChanged: onChanged,
      textInputAction: TextInputAction.search,
      style: theme.textTheme.bodyMedium,
      decoration: InputDecoration(
        isDense: true,
        filled: true,
        fillColor: surfaces.cardMuted,
        hintText: 'Plan ara',
        hintStyle: theme.textTheme.bodyMedium?.copyWith(
          color: surfaces.inkFaint,
        ),
        prefixIcon: Icon(Icons.search, size: 20, color: surfaces.inkMuted),
        suffixIcon: controller.text.isEmpty
            ? null
            : IconButton(
                tooltip: 'Aramayı temizle',
                onPressed: onClear,
                icon: const Icon(Icons.close, size: 20),
              ),
        constraints: const BoxConstraints(minHeight: 48),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.compact,
        ),
        border: border,
        enabledBorder: border,
        focusedBorder: border,
      ),
    );
  }
}

class _StoppedLink extends StatelessWidget {
  const _StoppedLink({required this.count, required this.onTap});

  final int count;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return AppCard(
      padding: EdgeInsets.zero,
      child: InkWell(
        onTap: onTap,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: AppSizes.button + 8),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: Row(
              children: [
                Expanded(
                  child: Text(
                    'Duraklatılan ve bitenler',
                    style: theme.textTheme.titleSmall,
                  ),
                ),
                Text(
                  '$count',
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: surfaces.inkMuted,
                  ),
                ),
                const SizedBox(width: AppSpacing.xSmall),
                Icon(Icons.chevron_right, color: surfaces.inkMuted),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Duraklatılan ve biten planlar; gelecek vade varmış gibi tarih yazılmaz.
class _StoppedPlansPage extends StatelessWidget {
  const _StoppedPlansPage({required this.controller, required this.onOpen});

  final PlanningController controller;
  final Future<void> Function(RecurringPlanRowData row) onOpen;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: AnimatedBuilder(
        animation: controller,
        builder: (context, _) {
          final snapshot = controller.snapshot;
          final rows = snapshot == null
              ? const <RecurringPlanRowData>[]
              : [
                  for (final plan in snapshot.recurringTransactions)
                    RecurringPlanRowData.of(
                      plan,
                      snapshot,
                      controller.asOfDate,
                    ),
                ].where((row) => !row.isRunning).toList();
          return Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AppPageHeader(
                title: 'Duraklatılan ve bitenler',
                onBack: () => Navigator.maybePop(context),
              ),
              Expanded(
                child: rows.isEmpty
                    ? const AppNoResultsView(
                        reason: AppNoResultsReason.filter,
                        title: 'Duraklatılan ya da biten plan yok',
                      )
                    : ListView(
                        padding: AppPageInsets.fullBleedList,
                        children: [
                          Padding(
                            padding: const EdgeInsets.symmetric(
                              horizontal: AppPageInsets.edge,
                            ),
                            child: AppCard(
                              padding: EdgeInsets.zero,
                              child: Column(
                                children: [
                                  for (final (index, row) in rows.indexed) ...[
                                    if (index > 0)
                                      const Divider(
                                        height: 1,
                                        indent:
                                            AppIconCapsule.rowInset -
                                            AppSpacing.small,
                                        endIndent: AppSpacing.medium,
                                      ),
                                    RecurringPlanRow(
                                      row: row,
                                      onTap: () => onOpen(row),
                                    ),
                                  ],
                                ],
                              ),
                            ),
                          ),
                        ],
                      ),
              ),
            ],
          );
        },
      ),
    ),
  );
}
