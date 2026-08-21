import 'package:flutter/material.dart';

import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/category_models.dart';
import 'categories_view_model.dart';
import 'category_form_page.dart';

class CategoriesPage extends StatefulWidget {
  const CategoriesPage({
    required this.viewModel,
    this.ownsViewModel = false,
    super.key,
  });

  final CategoriesViewModel viewModel;
  final bool ownsViewModel;

  @override
  State<CategoriesPage> createState() => _CategoriesPageState();
}

class _CategoriesPageState extends State<CategoriesPage>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    // Gelir ve gider kategorileri iki ayrı listedir: kullanıcı kategori
    // ararken yalnız birinin içindedir ve karışık liste onu her defasında
    // ayıklamaya zorluyordu.
    _tabController = TabController(length: 2, vsync: this);
    widget.viewModel.addListener(_refresh);
    if (widget.viewModel.status == CategoriesViewStatus.initial) {
      widget.viewModel.load();
    }
  }

  @override
  void didUpdateWidget(covariant CategoriesPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.viewModel != widget.viewModel) {
      oldWidget.viewModel.removeListener(_refresh);
      widget.viewModel.addListener(_refresh);
    }
  }

  @override
  void dispose() {
    _tabController.dispose();
    widget.viewModel.removeListener(_refresh);
    if (widget.ownsViewModel) widget.viewModel.dispose();
    super.dispose();
  }

  void _refresh() {
    if (mounted) setState(() {});
  }

  Future<void> _openForm([BudgetCategory? category]) async {
    final saved = await Navigator.of(context).push<bool>(
      MaterialPageRoute(
        builder: (_) =>
            CategoryFormPage(category: category, onSave: widget.viewModel.save),
      ),
    );
    if (saved == true && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(widget.viewModel.message ?? 'Kategori kaydedildi.'),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Kategoriler'),
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Giderler'),
            Tab(text: 'Gelirler'),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        heroTag: 'categories-add-category',
        tooltip: 'Kategori ekle',
        onPressed: widget.viewModel.isSubmitting ? null : _openForm,
        child: const Icon(Icons.add),
      ),
      body: switch (widget.viewModel.status) {
        CategoriesViewStatus.initial || CategoriesViewStatus.loading =>
          const AppLoadingView(message: 'Kategoriler yükleniyor'),
        CategoriesViewStatus.empty => const AppEmptyView(
          title: 'Henüz kategori yok',
          message: 'İlk gelir veya gider kategorinizi ekleyin.',
          icon: Icons.category_outlined,
        ),
        CategoriesViewStatus.error => AppErrorView(
          message: widget.viewModel.message ?? 'Kategoriler yüklenemedi.',
          onRetry: widget.viewModel.load,
        ),
        CategoriesViewStatus.unauthorized => const AppUnauthorizedView(),
        CategoriesViewStatus.ready => TabBarView(
          controller: _tabController,
          children: [
            _CategoryList(
              categories: widget.viewModel.categories
                  .where((category) => category.type != 'income')
                  .toList(growable: false),
              isIncome: false,
              onRefresh: widget.viewModel.load,
              onEdit: _openForm,
            ),
            _CategoryList(
              categories: widget.viewModel.categories
                  .where((category) => category.type == 'income')
                  .toList(growable: false),
              isIncome: true,
              onRefresh: widget.viewModel.load,
              onEdit: _openForm,
            ),
          ],
        ),
      },
    );
  }
}

/// Tek türün kategorileri. Aktifler önce, pasifler ayrı bir başlık altında:
/// pasif kategori geçmişte kullanılmış olabilir, silinmez ama çalışanları da
/// gömmemelidir.
class _CategoryList extends StatelessWidget {
  const _CategoryList({
    required this.categories,
    required this.isIncome,
    required this.onRefresh,
    required this.onEdit,
  });

  final List<BudgetCategory> categories;
  final bool isIncome;
  final Future<void> Function() onRefresh;
  final void Function(BudgetCategory) onEdit;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final active = categories.where((item) => item.isActive).toList();
    final inactive = categories.where((item) => !item.isActive).toList();

    if (categories.isEmpty) {
      return AppEmptyView(
        icon: isIncome
            ? Icons.savings_outlined
            : Icons.shopping_basket_outlined,
        title: isIncome ? 'Gelir kategorisi yok' : 'Gider kategorisi yok',
        message: 'Sağ alttaki artı ile ekleyebilirsiniz.',
      );
    }

    return RefreshIndicator(
      onRefresh: onRefresh,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.medium,
          AppSpacing.medium,
          AppSpacing.fabClearance,
        ),
        children: [
          if (active.isNotEmpty) ...[
            Text('Kullanımda', style: theme.textTheme.labelMedium),
            const SizedBox(height: AppSpacing.small),
            _Group(items: active, isIncome: isIncome, onEdit: onEdit),
          ],
          if (inactive.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.large),
            Text('Pasif', style: theme.textTheme.labelMedium),
            const SizedBox(height: AppSpacing.small),
            _Group(items: inactive, isIncome: isIncome, onEdit: onEdit),
          ],
        ],
      ),
    );
  }
}

class _Group extends StatelessWidget {
  const _Group({
    required this.items,
    required this.isIncome,
    required this.onEdit,
  });

  final List<BudgetCategory> items;
  final bool isIncome;
  final void Function(BudgetCategory) onEdit;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final accent = isIncome ? colors.incomeFill : colors.expenseFill;

    return AppCard(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          for (var i = 0; i < items.length; i++) ...[
            if (i > 0)
              const Padding(
                padding: EdgeInsets.symmetric(horizontal: AppSpacing.medium),
                child: Divider(height: 1),
              ),
            AppListRow(
              icon: AppFinanceIcons.forCategory(items[i].name),
              iconColor: accent,
              iconBackground: accent.withValues(alpha: 0.12),
              title: items[i].name,
              subtitle: items[i].isActive ? null : 'Yeni kayıtta seçilemez',
              dimmed: !items[i].isActive,
              onTap: () => onEdit(items[i]),
              trailing: Icon(
                Icons.chevron_right,
                color: Theme.of(context).colorScheme.onSurfaceVariant,
              ),
            ),
          ],
        ],
      ),
    );
  }
}
