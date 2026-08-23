import 'package:flutter/material.dart';

import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/activity_models.dart';
import '../data/activity_repository.dart';
import 'activity_controller.dart';
import '../data/planned_activity_models.dart';
import 'activity_detail_sheet.dart';
import 'activity_filter_sheet.dart';
import 'activity_tile.dart';
import 'planned_summary_card.dart';

/// The unified history: every realized movement in one chronological list,
/// whichever write model produced it.
class ActivityFeedPage extends StatefulWidget {
  const ActivityFeedPage({
    required this.repository,
    super.key,
    this.changes,
    this.onCreateTransaction,
    this.onShowPlanned,
    this.scopeController,
  });

  final ActivityRepositoryContract repository;
  final FinancialDataChanges? changes;

  /// Uygulama genelindeki kapsam anahtarı. Bu ekran anahtarı **değiştirmez**,
  /// yalnız uygular ve başlığında yazar; tek anahtar Özet ekranındadır.
  final ScopeController? scopeController;
  final VoidCallback? onCreateTransaction;
  final VoidCallback? onShowPlanned;

  @override
  State<ActivityFeedPage> createState() => _ActivityFeedPageState();
}

class _ActivityFeedPageState extends State<ActivityFeedPage> {
  late final ActivityController _controller;
  final ScrollController _scrollController = ScrollController();

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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: AnimatedBuilder(
          animation: _controller,
          // Aktif kapsam başlıkta yazılı durur: bu liste bölünen bir okuma ve
          // filtrenin açık olduğu ekranda görünmezse eksik liste, kayıp kayıt
          // gibi okunur.
          builder: (context, _) => Text(
            _controller.scope == null
                ? 'İşlemler'
                : 'İşlemler · ${_controller.scope!.label}',
          ),
        ),
        actions: [
          AnimatedBuilder(
            animation: _controller,
            builder: (context, _) => IconButton(
              onPressed: _openFilters,
              tooltip: 'Gelişmiş filtre',
              icon: Icon(
                _controller.filter.hasAdvancedFilters
                    ? Icons.filter_alt
                    : Icons.filter_alt_outlined,
              ),
            ),
          ),
        ],
      ),
      body: AnimatedBuilder(
        animation: _controller,
        builder: (context, _) => _buildBody(context),
      ),
    );
  }

  Widget _buildBody(BuildContext context) {
    if (_controller.unauthorized) return const AppUnauthorizedView();

    return Column(
      children: [
        _QuickFilterBar(
          selected: _controller.filter.quickFilter,
          onSelected: _controller.selectQuickFilter,
        ),
        if (_controller.isStale) const _StaleBanner(),
        if (_planned != null && widget.onShowPlanned != null)
          PlannedSummaryCard(
            count: _planned!.totalCount,
            nearestDueDate: _planned!.nearestDueDate,
            hasAttention: _planned!.items.any((item) => item.needsAttention),
            onTap: widget.onShowPlanned!,
          ),
        Expanded(
          child: ColoredBox(
            color: AppSurfaces.of(context).card,
            child: _buildList(context),
          ),
        ),
      ],
    );
  }

  Widget _buildList(BuildContext context) {
    if (_controller.isLoading && _controller.items.isEmpty) {
      return const AppLoadingView();
    }
    if (_controller.errorMessage != null && _controller.items.isEmpty) {
      return AppErrorView(
        message: _controller.errorMessage!,
        onRetry: _controller.load,
      );
    }
    if (_controller.isEmpty) {
      return AppEmptyView(
        title: 'Henüz hareket yok',
        message: _controller.filter.hasAdvancedFilters
            ? 'Seçtiğiniz filtrelere uyan hareket bulunamadı.'
            : 'Gelir, gider, transfer ve kart hareketleriniz burada listelenir.',
        icon: Icons.receipt_long_outlined,
      );
    }

    final items = _controller.items;
    return RefreshIndicator(
      onRefresh: _controller.load,
      child: ListView.separated(
        controller: _scrollController,
        padding: const EdgeInsets.only(bottom: AppSpacing.xLarge),
        itemCount: items.length + 1,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          if (index == items.length) return _buildFooter(context);
          final activity = items[index];
          return ActivityTile(
            key: ValueKey(activity.listKey),
            activity: activity,
            onTap: () => _openDetail(activity),
          );
        },
      ),
    );
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
      padding: const EdgeInsets.all(AppSpacing.medium),
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

class _QuickFilterBar extends StatelessWidget {
  const _QuickFilterBar({required this.selected, required this.onSelected});

  final ActivityQuickFilter selected;
  final ValueChanged<ActivityQuickFilter> onSelected;

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.small,
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
