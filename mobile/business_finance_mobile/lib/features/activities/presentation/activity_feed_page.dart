import 'dart:async';

import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/activity_models.dart';
import '../data/activity_repository.dart';
import '../data/planned_activity_models.dart';
import 'activity_controller.dart';
import 'activity_detail_sheet.dart';
import 'activity_filter_sheet.dart';
import 'activity_tile.dart';

/// The unified history: every realized movement in one chronological list,
/// whichever write model produced it.
///
/// Tasarım teslimi (27 Eylül 2026): arama alanı, tür çipleri, planlananlar
/// şeridi ve güne göre gruplu akış. Gün başlığı kaydırırken üstte durur; her
/// günün kayıtları tam genişlik beyaz bir blokta, ayırıcı yazının
/// başladığı yerden.
class ActivityFeedPage extends StatefulWidget {
  const ActivityFeedPage({
    required this.repository,
    super.key,
    this.changes,
    this.onCreateTransaction,
    this.onShowPlanned,
    this.scopeController,
    this.now,
  });

  final ActivityRepositoryContract repository;
  final FinancialDataChanges? changes;

  /// Uygulama genelindeki kapsam anahtarı. Bu ekran anahtarı **değiştirmez**,
  /// yalnız uygular ve başlığında yazar; tek anahtar Özet ekranındadır.
  final ScopeController? scopeController;
  final VoidCallback? onCreateTransaction;
  final VoidCallback? onShowPlanned;

  /// Gün başlıklarındaki `Bugün` / `Dün` bu güne göre yazılır.
  final DateTime Function()? now;

  @override
  State<ActivityFeedPage> createState() => _ActivityFeedPageState();
}

class _ActivityFeedPageState extends State<ActivityFeedPage> {
  late final ActivityController _controller;
  final ScrollController _scrollController = ScrollController();
  final TextEditingController _searchController = TextEditingController();
  Timer? _searchDebounce;

  @override
  void initState() {
    super.initState();
    _controller = ActivityController(
      widget.repository,
      financialDataChanges: widget.changes,
      scopeController: widget.scopeController,
    );
    _scrollController.addListener(_handleScroll);
    _controller.load();
    _loadPlannedSummary();
  }

  PlannedActivityPage? _planned;

  /// The summary is a separate, small read. It must not block or fail the
  /// history: a planned outage should not empty the list of what already
  /// happened.
  Future<void> _loadPlannedSummary() async {
    try {
      final page = await widget.repository.listPlanned(
        horizon: PlannedHorizon.month,
        scope: widget.scopeController?.scope,
      );
      if (mounted) setState(() => _planned = page);
    } on Exception {
      if (mounted) setState(() => _planned = null);
    }
  }

  @override
  void dispose() {
    _searchDebounce?.cancel();
    _searchController.dispose();
    _scrollController
      ..removeListener(_handleScroll)
      ..dispose();
    _controller.dispose();
    super.dispose();
  }

  void _handleScroll() {
    if (!_scrollController.hasClients) return;
    final position = _scrollController.position;
    if (position.pixels >= position.maxScrollExtent - 200) {
      _controller.loadMore();
    }
  }

  /// Arama yazarken her tuşta istek gitmez; kısa bir duraklamadan sonra
  /// sunucuda aranır.
  void _onSearchChanged(String text) {
    _searchDebounce?.cancel();
    _searchDebounce = Timer(
      const Duration(milliseconds: 350),
      () => _controller.search(text),
    );
    setState(() {});
  }

  void _clearSearch() {
    _searchDebounce?.cancel();
    _searchController.clear();
    _controller.search('');
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: AnimatedBuilder(
          animation: _controller,
          builder: (context, _) => Column(
            children: [
              // Aktif kapsam başlıkta yazılı durur: bu liste bölünen bir okuma
              // ve filtrenin açık olduğu ekranda görünmezse eksik liste, kayıp
              // kayıt gibi okunur.
              AppPageHeader(
                title: _controller.scope == null
                    ? 'İşlemler'
                    : 'İşlemler · ${_controller.scope!.label}',
                actions: [
                  IconButton(
                    onPressed: _openFilters,
                    tooltip: 'Gelişmiş filtre',
                    icon: Icon(
                      _controller.filter.hasAdvancedFilters
                          ? Icons.filter_alt
                          : Icons.tune,
                    ),
                  ),
                ],
              ),
              if (_controller.unauthorized)
                const Expanded(child: AppUnauthorizedView())
              else ...[
                Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppSpacing.medium,
                    AppSpacing.xSmall,
                    AppSpacing.medium,
                    0,
                  ),
                  child: _SearchField(
                    controller: _searchController,
                    onChanged: _onSearchChanged,
                    onClear: _clearSearch,
                  ),
                ),
                _QuickFilterBar(
                  selected: _controller.filter.quickFilter,
                  onSelected: _controller.selectQuickFilter,
                ),
                if (_controller.isStale) const _StaleBanner(),
                Expanded(child: _buildList(context)),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _plannedStrip() => Padding(
    padding: const EdgeInsets.fromLTRB(
      AppSpacing.medium,
      AppSpacing.xSmall,
      AppSpacing.medium,
      0,
    ),
    child: _PlannedStrip(
      count: _planned!.totalCount,
      hasAttention: _planned!.items.any((item) => item.needsAttention),
      onTap: widget.onShowPlanned!,
    ),
  );

  Widget _buildList(BuildContext context) {
    final showPlanned = _planned != null && widget.onShowPlanned != null;
    Widget withPlanned(Widget child) => showPlanned
        ? Column(
            children: [
              _plannedStrip(),
              Expanded(child: child),
            ],
          )
        : child;

    if (_controller.isLoading && _controller.items.isEmpty) {
      return withPlanned(const AppLoadingView());
    }
    if (_controller.errorMessage != null && _controller.items.isEmpty) {
      return withPlanned(
        AppErrorView(
          message: _controller.errorMessage!,
          onRetry: _controller.load,
        ),
      );
    }
    if (_controller.isEmpty) {
      final searching = _controller.filter.search != null;
      return withPlanned(
        AppEmptyView(
          title: searching ? 'Eşleşen hareket yok' : 'Henüz hareket yok',
          message: searching
              ? '"${_controller.filter.search}" için bir kayıt bulunamadı.'
              : _controller.filter.hasAdvancedFilters
              ? 'Seçtiğiniz filtrelere uyan hareket bulunamadı.'
              : 'Gelir, gider, transfer ve kart hareketleriniz burada listelenir.',
          icon: searching ? Icons.search_off : Icons.receipt_long_outlined,
        ),
      );
    }

    final groups = _groupByDay(_controller.items);
    return RefreshIndicator(
      onRefresh: _controller.load,
      child: CustomScrollView(
        controller: _scrollController,
        slivers: [
          if (showPlanned) SliverToBoxAdapter(child: _plannedStrip()),
          for (final group in groups)
            SliverMainAxisGroup(
              slivers: [
                PinnedHeaderSliver(
                  child: _DayHeader(date: group.date, today: _today),
                ),
                SliverToBoxAdapter(
                  child: _DayBlock(
                    activities: group.items,
                    onOpen: _openDetail,
                  ),
                ),
              ],
            ),
          SliverToBoxAdapter(child: _buildFooter(context)),
          const SliverToBoxAdapter(
            child: SizedBox(height: AppSpacing.fabClearance),
          ),
        ],
      ),
    );
  }

  DateTime get _today => (widget.now ?? DateTime.now)();

  static List<({String date, List<FinancialActivity> items})> _groupByDay(
    List<FinancialActivity> items,
  ) {
    final groups = <({String date, List<FinancialActivity> items})>[];
    for (final item in items) {
      if (groups.isNotEmpty && groups.last.date == item.activityDate) {
        groups.last.items.add(item);
      } else {
        groups.add((date: item.activityDate, items: [item]));
      }
    }
    return groups;
  }

  Widget _buildFooter(BuildContext context) {
    if (_controller.isLoadingMore) {
      return const Padding(
        padding: EdgeInsets.all(AppSpacing.medium),
        child: Center(child: CircularProgressIndicator()),
      );
    }
    if (_controller.hasMore) {
      return Padding(
        padding: const EdgeInsets.all(AppSpacing.medium),
        child: Center(
          child: TextButton(
            onPressed: _controller.loadMore,
            child: const Text('Daha fazla göster'),
          ),
        ),
      );
    }
    final total = _controller.pagination?.totalCount ?? 0;
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.large - AppSpacing.xSmall,
        AppSpacing.medium,
        0,
      ),
      child: Center(
        child: Text(
          'Toplam $total hareket',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ),
    );
  }

  Future<void> _openDetail(FinancialActivity activity) async {
    await AppAdaptiveSheet.show<void>(
      context: context,
      builder: (sheetContext) => AnimatedBuilder(
        animation: _controller,
        builder: (context, _) => ActivityDetailSheet(
          activity: activity,
          isCancelling: _controller.isCancelling,
          onCancel: activity.canCancel
              ? () async {
                  final cancelled = await _controller.cancel(activity);
                  if (cancelled && sheetContext.mounted) {
                    Navigator.of(sheetContext).pop();
                  }
                }
              : null,
        ),
      ),
    );
    if (!mounted) return;
    _showPendingMessage();
  }

  void _showPendingMessage() {
    final message = _controller.successMessage ?? _controller.errorMessage;
    if (message == null) return;
    ScaffoldMessenger.of(context)
      ..hideCurrentSnackBar()
      ..showSnackBar(SnackBar(content: Text(message)));
    _controller.clearMessages();
  }

  Future<void> _openFilters() async {
    final result = await AppAdaptiveSheet.show<ActivityFilter>(
      context: context,
      builder: (context) => ActivityFilterSheet(filter: _controller.filter),
    );
    if (result != null) await _controller.applyFilter(result);
  }
}

/// Arama alanı: gri zemin, 48 dp, solda büyüteç. Arama sunucuda yapılır.
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
        hintText: 'İşlem, kategori veya hesap ara',
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
          vertical: AppSpacing.small + AppSpacing.xSmall,
        ),
        border: border,
        enabledBorder: border,
        focusedBorder: border,
      ),
    );
  }
}

class _QuickFilterBar extends StatelessWidget {
  const _QuickFilterBar({required this.selected, required this.onSelected});

  final ActivityQuickFilter selected;
  final ValueChanged<ActivityQuickFilter> onSelected;

  @override
  Widget build(BuildContext context) {
    return Semantics(
      container: true,
      label: 'İşlem türü',
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.small + AppSpacing.xSmall,
          AppSpacing.medium,
          AppSpacing.small,
        ),
        child: Row(
          children: [
            for (final filter in ActivityQuickFilter.values)
              Padding(
                padding: const EdgeInsets.only(right: AppSpacing.small),
                child: ChoiceChip(
                  label: Text(filter.label),
                  selected: filter == selected,
                  onSelected: (_) => onSelected(filter),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

/// Planlananlar şeridi: sayı ve bakiyeye dahil olmadığı; dokununca
/// Planlananlar açılır.
class _PlannedStrip extends StatelessWidget {
  const _PlannedStrip({
    required this.count,
    required this.hasAttention,
    required this.onTap,
  });

  final int count;
  final bool hasAttention;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final foreground = colors.onPlannedContainer;
    final radius = BorderRadius.circular(AppRadius.field);
    final title = '$count planlanan işlem';
    return Semantics(
      button: true,
      label:
          '$title, bakiyeye dahil değil'
          '${hasAttention ? ', dikkat isteyen kalem var' : ''}. '
          'Planlananları açar.',
      excludeSemantics: true,
      child: Material(
        color: colors.plannedContainer,
        borderRadius: radius,
        child: InkWell(
          borderRadius: radius,
          onTap: onTap,
          child: ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 48),
            child: Padding(
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.medium,
                AppSpacing.small,
                AppSpacing.small,
                AppSpacing.small,
              ),
              child: Row(
                children: [
                  Icon(
                    hasAttention ? Icons.error_outline : Icons.event_repeat,
                    size: 20,
                    color: foreground,
                  ),
                  const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                  Flexible(
                    child: Text(
                      title,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                        height: 1.3,
                        color: foreground,
                      ),
                    ),
                  ),
                  const SizedBox(width: AppSpacing.small),
                  Expanded(
                    child: Text(
                      'Bakiyeye dahil değil',
                      textAlign: TextAlign.right,
                      style: theme.textTheme.labelMedium?.copyWith(
                        fontWeight: FontWeight.w400,
                        letterSpacing: 0,
                        color: foreground,
                      ),
                    ),
                  ),
                  Icon(Icons.chevron_right, size: 22, color: foreground),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Gün başlığı: `24 Eylül` ve yanında `Dün` / `Bugün` / haftanın günü.
/// Kaydırırken üstte durur; zemini sayfa zeminidir.
class _DayHeader extends StatelessWidget {
  const _DayHeader({required this.date, required this.today});

  final String date;
  final DateTime today;

  static const _weekdays = [
    'Pazartesi',
    'Salı',
    'Çarşamba',
    'Perşembe',
    'Cuma',
    'Cumartesi',
    'Pazar',
  ];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final parsed = DateTime.tryParse(date);
    final relative = parsed == null ? null : _relative(parsed);
    return Semantics(
      header: true,
      child: Container(
        width: double.infinity,
        color: surfaces.canvas,
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.large - AppSpacing.xSmall,
          AppSpacing.medium,
          AppSpacing.small,
        ),
        child: Text.rich(
          TextSpan(
            children: [
              TextSpan(
                text: DateText.dayMonth(date),
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: 15,
                  height: 1.3,
                ),
              ),
              if (relative != null)
                TextSpan(text: '  $relative', style: theme.textTheme.bodySmall),
            ],
          ),
        ),
      ),
    );
  }

  String _relative(DateTime day) {
    final start = DateTime(today.year, today.month, today.day);
    final days = start
        .difference(DateTime(day.year, day.month, day.day))
        .inDays;
    return switch (days) {
      0 => 'Bugün',
      1 => 'Dün',
      _ => _weekdays[day.weekday - 1],
    };
  }
}

/// Bir günün kayıtları: tam genişlik beyaz blok, üst ve alt kenar çizgisi,
/// satırlar arası ayırıcı yazının başladığı yerden (72 dp).
class _DayBlock extends StatelessWidget {
  const _DayBlock({required this.activities, required this.onOpen});

  final List<FinancialActivity> activities;
  final ValueChanged<FinancialActivity> onOpen;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return DecoratedBox(
      decoration: BoxDecoration(
        color: surfaces.card,
        border: Border.symmetric(
          horizontal: BorderSide(color: surfaces.border),
        ),
      ),
      child: Material(
        type: MaterialType.transparency,
        child: Column(
          children: [
            for (var i = 0; i < activities.length; i++) ...[
              if (i > 0)
                Padding(
                  padding: const EdgeInsets.only(left: AppIconCapsule.rowInset),
                  child: Divider(
                    height: 1,
                    thickness: 1,
                    color: surfaces.border,
                  ),
                ),
              ActivityTile(
                key: ValueKey(activities[i].listKey),
                activity: activities[i],
                showDate: false,
                onTap: () => onOpen(activities[i]),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _StaleBanner extends StatelessWidget {
  const _StaleBanner();

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    return Semantics(
      liveRegion: true,
      child: Container(
        width: double.infinity,
        color: colors.secondaryContainer,
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.small,
        ),
        child: Text(
          'Bu liste güncel olmayabilir. Aşağı çekerek yenileyin.',
          style: Theme.of(
            context,
          ).textTheme.bodySmall?.copyWith(color: colors.onSecondaryContainer),
        ),
      ),
    );
  }
}
