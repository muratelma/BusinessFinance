import 'package:flutter/material.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_first_use_view.dart';
import '../../../core/widgets/app_no_results_view.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_section_empty.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../data/debt_book_models.dart';
import 'debt_book_parts.dart';

/// `Borç ve alacak` listesinin sekmesi (K11).
enum DebtBookTab {
  receivables('Alacaklar'),
  payables('Borçlar'),
  people('Kişiler');

  const DebtBookTab(this.label);
  final String label;

  DebtSide? get side => switch (this) {
    receivables => DebtSide.receivable,
    payables => DebtSide.payable,
    people => null,
  };
}

/// `Borç ve alacak` listesi (`05-EKRAN-TASARIMI.md` §3.1). Veriyi okumaz;
/// sunucunun cevabını ([book]) çizer. Sekme, `Yalnız gecikenler` ve arama bu
/// ekranın kendi durumudur.
class DebtBookView extends StatefulWidget {
  const DebtBookView({
    required this.book,
    super.key,
    this.scope,
    this.scopeVisible = false,
    this.onScope,
    this.initialTab = DebtBookTab.receivables,
    this.initialQuery,
    this.initialOverdueOnly = false,
    this.onBack,
    this.onAdd,
    this.onOpenItem,
    this.onOpenPerson,
    this.onOpenClosed,
    this.notice,
    this.onRefresh,
  });

  final DebtBook book;
  final TransactionScope? scope;
  final bool scopeVisible;
  final ValueChanged<TransactionScope?>? onScope;
  final DebtBookTab initialTab;

  /// Dolu verilirse ekran arama hâlinde açılır.
  final String? initialQuery;
  final bool initialOverdueOnly;
  final VoidCallback? onBack;
  final VoidCallback? onAdd;
  final ValueChanged<DebtBookItem>? onOpenItem;
  final ValueChanged<DebtBookPerson>? onOpenPerson;
  final ValueChanged<DebtSide>? onOpenClosed;

  /// Listenin üstündeki not (`Liste güncellenemedi.`).
  final Widget? notice;

  /// Aşağı çekerek yenileme.
  final Future<void> Function()? onRefresh;

  @override
  State<DebtBookView> createState() => _DebtBookViewState();
}

class _DebtBookViewState extends State<DebtBookView> {
  late DebtBookTab _tab = widget.initialTab;
  late bool _overdueOnly = widget.initialOverdueOnly;
  late bool _searching = widget.initialQuery != null;
  late final TextEditingController _query = TextEditingController(
    text: widget.initialQuery,
  );
  final ScrollController _scroll = ScrollController();

  @override
  void dispose() {
    _query.dispose();
    _scroll.dispose();
    super.dispose();
  }

  void _selectTab(DebtBookTab tab) {
    if (tab == _tab) return;
    setState(() {
      _tab = tab;
      _overdueOnly = false;
    });
    if (_scroll.hasClients) _scroll.jumpTo(0);
  }

  void _closeSearch() => setState(() {
    _searching = false;
    _query.clear();
  });

  @override
  Widget build(BuildContext context) {
    final book = widget.book;
    final firstUse = book.isEmpty;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppPageHeader(
          title: 'Borç ve alacak',
          onBack: widget.onBack,
          actions: [
            if (!firstUse && !_searching)
              IconButton(
                tooltip: 'Ara',
                icon: const Icon(Icons.search),
                onPressed: () => setState(() => _searching = true),
              ),
            if (!firstUse)
              IconButton(
                tooltip: 'Borç veya alacak ekle',
                icon: const Icon(Icons.add),
                onPressed: widget.onAdd,
              ),
          ],
        ),
        Expanded(
          child: firstUse
              ? AppFirstUseView(
                  title: 'Henüz borç ya da alacak yok',
                  message: 'Veresiye, borç para ya da krediyi ekleyin.',
                  icon: Icons.swap_horiz,
                  actionLabel: 'Borç veya alacak ekle',
                  onAction: widget.onAdd,
                )
              : _searching
              ? _searchBody(context)
              : _listBody(context),
        ),
      ],
    );
  }

  Widget _listBody(BuildContext context) {
    final book = widget.book;
    final side = _tab.side;
    final showScope = widget.scopeVisible && widget.scope == null;
    final children = <Widget>[
      if (widget.notice != null) ...[
        widget.notice!,
        const SizedBox(height: AppGaps.notice),
      ],
      _SummaryCard(
        book: book,
        scopeVisible: widget.scopeVisible,
        scope: widget.scope,
        onScope: widget.onScope ?? (_) {},
      ),
      const SizedBox(height: AppGaps.block),
      AppSegmentRail<DebtBookTab>(
        values: DebtBookTab.values,
        selected: _tab,
        onChanged: _selectTab,
        semanticLabel: 'Liste',
        segmentBuilder: (context, tab, selected) => Text(
          tab.label,
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
          style: Theme.of(context).textTheme.labelLarge?.copyWith(
            fontWeight: selected ? FontWeight.w700 : FontWeight.w500,
            letterSpacing: 0,
          ),
        ),
      ),
    ];

    if (side == null) {
      final people = [
        for (final person in book.people)
          if (person.isActive) person,
      ];
      children
        ..add(const SizedBox(height: AppGaps.block))
        ..add(
          people.isEmpty
              ? const AppSectionEmpty('Henüz kişi yok.')
              : DebtSectionCard(
                  children: [
                    for (final person in people)
                      DebtPersonRow(
                        person: person,
                        showScope: showScope,
                        onTap: () => widget.onOpenPerson?.call(person),
                      ),
                  ],
                ),
        );
    } else {
      final items = book.itemsOf(side);
      final hasOverdue = isPositiveAmount(book.overdueOf(side));
      if (hasOverdue) {
        // Şerit sayfanın kenar payının içinde: tek çip, kaydırma yok.
        children
          ..add(const SizedBox(height: AppGaps.notice))
          ..add(
            Align(
              alignment: Alignment.centerLeft,
              child: FilterChip(
                label: const Text('Yalnız gecikenler'),
                selected: _overdueOnly,
                onSelected: (value) => setState(() => _overdueOnly = value),
              ),
            ),
          )
          ..add(const SizedBox(height: AppGaps.notice));
      } else {
        children.add(const SizedBox(height: AppGaps.block));
      }
      final sections = [
        (DebtBookSection.overdue, 'Geciken', null),
        if (!_overdueOnly) ...[
          (DebtBookSection.upcoming, 'Yaklaşan', '30 gün içinde'),
          (DebtBookSection.other, 'Diğer', null),
        ],
      ];
      var drawn = 0;
      for (final (section, title, note) in sections) {
        final rows = [
          for (final item in items)
            if (item.section == section) item,
        ];
        if (rows.isEmpty) continue;
        if (drawn > 0) children.add(const SizedBox(height: AppGaps.section));
        drawn++;
        children
          ..add(DebtSectionTitle(title: title, note: note))
          ..add(
            DebtSectionCard(
              children: [
                for (final item in rows)
                  DebtBookItemRow(
                    item: item,
                    showScope: showScope,
                    onTap: () => widget.onOpenItem?.call(item),
                  ),
              ],
            ),
          );
      }
      if (drawn == 0) {
        children.add(
          AppSectionEmpty(
            side == DebtSide.receivable
                ? 'Açık alacağınız yok.'
                : 'Açık borcunuz yok.',
          ),
        );
      }
      final closed = book.closedCountOf(side);
      if (closed > 0 && !_overdueOnly) {
        children
          ..add(const SizedBox(height: AppGaps.section))
          ..add(
            DebtGateRow(
              label: 'Kapananlar',
              count: closed,
              onTap: () => widget.onOpenClosed?.call(side),
            ),
          );
      }
    }

    final list = ListView(
      controller: _scroll,
      physics: const AlwaysScrollableScrollPhysics(),
      padding: AppPageInsets.aboveBar(context, AppPageInsets.list),
      children: children,
    );
    final refresh = widget.onRefresh;
    return refresh == null
        ? list
        : RefreshIndicator(onRefresh: refresh, child: list);
  }

  Widget _searchBody(BuildContext context) {
    final query = _query.text.trim().toLowerCase();
    final showScope = widget.scopeVisible && widget.scope == null;
    final people = [
      for (final person in widget.book.people)
        if (query.isNotEmpty && person.name.toLowerCase().contains(query))
          person,
    ];
    final records = [
      for (final item in widget.book.items)
        if (!item.isLedger &&
            query.isNotEmpty &&
            item.title.toLowerCase().contains(query))
          item,
    ];
    final empty = query.isNotEmpty && people.isEmpty && records.isEmpty;
    return ListView(
      padding: AppPageInsets.aboveBar(context, AppPageInsets.list),
      children: [
        _SearchField(
          controller: _query,
          onChanged: (_) => setState(() {}),
          onClose: _closeSearch,
        ),
        const SizedBox(height: AppGaps.block),
        if (empty)
          AppNoResultsView(
            reason: AppNoResultsReason.search,
            title: '“${_query.text.trim()}” için sonuç yok',
            actionLabel: 'Aramayı temizle',
            onAction: () => setState(_query.clear),
          )
        else if (people.isNotEmpty || records.isNotEmpty)
          DebtSectionCard(
            children: [
              for (final person in people)
                DebtPersonRow(
                  person: person,
                  showScope: showScope,
                  onTap: () => widget.onOpenPerson?.call(person),
                ),
              for (final item in records)
                DebtBookItemRow(
                  item: item,
                  showScope: showScope,
                  onTap: () => widget.onOpenItem?.call(item),
                ),
            ],
          ),
      ],
    );
  }
}

class _SummaryCard extends StatelessWidget {
  const _SummaryCard({
    required this.book,
    required this.scopeVisible,
    required this.scope,
    required this.onScope,
  });

  final DebtBook book;
  final bool scopeVisible;
  final TransactionScope? scope;
  final ValueChanged<TransactionScope?> onScope;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (scopeVisible)
            DebtScopeStrip(label: 'Toplam', scope: scope, onScope: onScope),
          Padding(
            padding: const EdgeInsets.all(AppSpacing.medium),
            child: IntrinsicHeight(
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Expanded(
                    child: DebtTotalColumn(
                      side: DebtSide.receivable,
                      amount: book.receivableTotal,
                      overdue: book.receivableOverdue,
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.compact,
                    ),
                    child: VerticalDivider(
                      width: 1,
                      thickness: 1,
                      color: surfaces.border,
                    ),
                  ),
                  Expanded(
                    child: DebtTotalColumn(
                      side: DebtSide.payable,
                      amount: book.payableTotal,
                      overdue: book.payableOverdue,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Tekrarlayanlar'ın arama alanının dili; sağda aramayı kapatan `✕`.
class _SearchField extends StatelessWidget {
  const _SearchField({
    required this.controller,
    required this.onChanged,
    required this.onClose,
  });

  final TextEditingController controller;
  final ValueChanged<String> onChanged;
  final VoidCallback onClose;

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
      autofocus: controller.text.isEmpty,
      onChanged: onChanged,
      textInputAction: TextInputAction.search,
      style: theme.textTheme.bodyMedium,
      decoration: InputDecoration(
        isDense: true,
        filled: true,
        fillColor: surfaces.cardMuted,
        hintText: 'Kişi ya da kayıt ara',
        hintStyle: theme.textTheme.bodyMedium?.copyWith(
          color: surfaces.inkFaint,
        ),
        prefixIcon: Icon(Icons.search, size: 20, color: surfaces.inkMuted),
        suffixIcon: IconButton(
          tooltip: 'Aramayı kapat',
          onPressed: onClose,
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
