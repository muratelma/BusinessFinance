import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_month_picker.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/pos_repository.dart';
import 'pos_controller.dart';
import 'pos_deposit_sheets.dart';
import 'pos_settlements_view.dart';

/// `Tüm tahsilatlar` sayfasının çizilen üç varyantı.
///
/// [a]: iki süzgeç satırı (durum; tür ve POS), hesaba geçenler yatışa göre
/// gruplu. [b]: İşlemler gibi tek hızlı süzgeç satırı, liste satış gününe
/// göre. [c]: üstte `Yolda · Hesaba geçti` rayı, altında tür ve POS.
enum PosPageLayout { a, b, c }

/// B varyantının tek satırlık hızlı süzgeci.
enum _Quick {
  all('Hepsi'),
  inTransit('Yolda'),
  deposited('Hesaba geçti'),
  sale('Satış'),
  collection('Tahsilat');

  const _Quick(this.label);

  final String label;
}

/// Tahsilatın durumuna göre süzgeç.
enum PosStatusFilter {
  all('Hepsi'),
  inTransit('Yolda'),
  deposited('Hesaba geçti');

  const PosStatusFilter(this.label);

  final String label;
}

/// Tahsilatın türüne göre süzgeç: satış ya da daha önce tanınmış bir alacağın
/// kartla tahsili (cari, tek seferlik alacak).
enum PosKindFilter {
  all('Tüm türler'),
  sale('Satış'),
  collection('Tahsilat');

  const PosKindFilter(this.label);

  final String label;
}

/// `Tüm tahsilatlar`: bir ayın POS tahsilatları, süzgeçlerle (Aşama 06.3 K11).
///
/// Kasa yalnız yoldakileri gösterir; geçmiş burada aranır. Yoldakiler
/// beklenen güne göre, hesaba geçenler **yatışa göre gruplu** durur: kullanıcının
/// yaptığı iş yatıştır. Süzmek para hesabı değildir; istemci hiçbir tutarı
/// toplamaz.
class PosAllSettlementsPage extends StatefulWidget {
  const PosAllSettlementsPage({
    required this.controller,
    super.key,
    this.now,
    this.layout = PosPageLayout.a,
  });

  /// Tasarım varyantı (K11); karar verilince tek hâl kalır.
  final PosPageLayout layout;

  final PosController controller;

  /// Açılışta gösterilen ay; testlerde sabitlenir.
  final DateTime Function()? now;

  @override
  State<PosAllSettlementsPage> createState() => _PosAllSettlementsPageState();
}

class _PosAllSettlementsPageState extends State<PosAllSettlementsPage> {
  late DateTime month = () {
    final today = (widget.now ?? DateTime.now)();
    return DateTime(today.year, today.month);
  }();
  PosStatusFilter status = PosStatusFilter.all;
  PosKindFilter kind = PosKindFilter.all;
  String? posName;

  List<PosSettlementItem>? items;
  bool isLoading = false;
  bool unauthorized = false;
  String? errorMessage;

  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_reloadAfterChange);
    _load();
  }

  @override
  void dispose() {
    widget.controller.removeListener(_reloadAfterChange);
    super.dispose();
  }

  /// Bu sayfadan açılan yatış ya da iptal Kasa'nın denetleyicisini yeniler;
  /// o yenilendiğinde bu sayfa da yeniden okur.
  void _reloadAfterChange() {
    if (!widget.controller.isLoading && !isLoading) _load();
  }

  Future<void> _load() async {
    if (isLoading) return;
    setState(() {
      isLoading = true;
      errorMessage = null;
    });
    try {
      final last = DateTime(month.year, month.month + 1, 0);
      final loaded = await widget.controller.repository.list(
        inTransitOnly: false,
        from: _iso(month),
        to: _iso(last),
      );
      if (!mounted) return;
      items = loaded.items;
      unauthorized = false;
      // Seçili POS bu ayda yoksa süzgeç boş liste göstermesin.
      if (!_posNames.contains(posName)) posName = null;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
    } finally {
      if (mounted) setState(() => isLoading = false);
    }
  }

  List<String> get _posNames => {
    for (final item in items ?? const <PosSettlementItem>[])
      ?item.posDefinitionName,
  }.toList()..sort();

  Iterable<PosSettlementItem> get _filtered =>
      (items ?? const <PosSettlementItem>[]).where(
        (item) =>
            (status == PosStatusFilter.all ||
                item.isInTransit == (status == PosStatusFilter.inTransit)) &&
            (kind == PosKindFilter.all ||
                item.isCollection == (kind == PosKindFilter.collection)) &&
            (posName == null || item.posDefinitionName == posName),
      );

  Future<void> _pickMonth() async {
    final picked = await AppMonthPicker.show(
      context: context,
      initialMonth: month,
    );
    if (picked == null || !mounted) return;
    month = DateTime(picked.year, picked.month);
    await _load();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Tüm tahsilatlar',
              onBack: () => Navigator.of(context).maybePop(),
              actions: [
                TextButton.icon(
                  onPressed: _pickMonth,
                  icon: const Icon(Icons.expand_more),
                  iconAlignment: IconAlignment.end,
                  label: Text(
                    '${DateText.months[month.month - 1]} ${month.year}',
                  ),
                ),
              ],
            ),
            ..._filters(context),
            Expanded(child: _body(context)),
          ],
        ),
      ),
    );
  }

  _Quick quick = _Quick.all;

  List<Widget> _filters(BuildContext context) {
    final kindChips = [
      for (final value in PosKindFilter.values)
        ChoiceChip(
          label: Text(value.label),
          selected: value == kind,
          onSelected: (_) => setState(() => kind = value),
        ),
      if (_posNames.length > 1) _posChip(),
    ];
    switch (widget.layout) {
      case PosPageLayout.a:
        return [
          _FilterRow(
            label: 'Durum',
            children: [
              for (final value in PosStatusFilter.values)
                ChoiceChip(
                  label: Text(value.label),
                  selected: value == status,
                  onSelected: (_) => setState(() => status = value),
                ),
            ],
          ),
          _FilterRow(label: 'Tür ve POS', children: kindChips),
        ];
      case PosPageLayout.b:
        return [
          _FilterRow(
            label: 'Süzgeç',
            children: [
              for (final value in _Quick.values)
                ChoiceChip(
                  label: Text(value.label),
                  selected: value == quick,
                  onSelected: (_) => setState(() {
                    quick = value;
                    status = switch (value) {
                      _Quick.inTransit => PosStatusFilter.inTransit,
                      _Quick.deposited => PosStatusFilter.deposited,
                      _ => PosStatusFilter.all,
                    };
                    kind = switch (value) {
                      _Quick.sale => PosKindFilter.sale,
                      _Quick.collection => PosKindFilter.collection,
                      _ => PosKindFilter.all,
                    };
                  }),
                ),
              if (_posNames.length > 1) _posChip(),
            ],
          ),
        ];
      case PosPageLayout.c:
        final surfaces = AppSurfaces.of(context);
        final shown = status == PosStatusFilter.deposited
            ? PosStatusFilter.deposited
            : PosStatusFilter.inTransit;
        if (status == PosStatusFilter.all) status = shown;
        return [
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.xSmall,
              AppSpacing.medium,
              AppSpacing.small,
            ),
            child: AppSegmentRail<PosStatusFilter>(
              values: const [
                PosStatusFilter.inTransit,
                PosStatusFilter.deposited,
              ],
              selected: shown,
              semanticLabel: 'Durum',
              segmentLabel: (value) => value.label,
              onChanged: (value) => setState(() => status = value),
              segmentBuilder: (context, value, isSelected) => Text(
                value.label,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                  color: isSelected ? surfaces.ink : surfaces.inkMuted,
                ),
              ),
            ),
          ),
          _FilterRow(label: 'Tür ve POS', children: kindChips),
        ];
    }
  }

  /// POS süzgeci: seçiliyse POS'un adını yazar; dokununca liste açılır.
  Widget _posChip() => PopupMenuButton<String?>(
    tooltip: 'POS seç',
    onSelected: (value) => setState(() => posName = value),
    itemBuilder: (_) => [
      const PopupMenuItem<String?>(child: Text("Tüm POS'lar")),
      for (final name in _posNames)
        PopupMenuItem<String?>(value: name, child: Text(name)),
    ],
    child: Chip(
      avatar: const Icon(Icons.expand_more, size: 18),
      label: Text(posName ?? "Tüm POS'lar"),
    ),
  );

  Widget _body(BuildContext context) {
    if (items == null) {
      if (isLoading) {
        return const AppLoadingView(message: 'Tahsilatlar yükleniyor');
      }
      if (unauthorized) return const AppUnauthorizedView();
      return AppErrorView(
        message: errorMessage ?? 'Tahsilatlar okunamadı.',
        onRetry: _load,
      );
    }
    final filtered = _filtered.toList();
    final inTransit = sortedInTransit(filtered);
    final deposits = _byDeposit(filtered);
    return RefreshIndicator(
      onRefresh: _load,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.small,
          AppSpacing.medium,
          AppSpacing.xLarge,
        ),
        children: [
          if (filtered.isEmpty)
            const AppCard(
              child: AppEmptyView(
                title: 'Tahsilat yok.',
                message: 'Bu ayda bu süzgece uyan POS tahsilatı yok.',
                icon: Icons.credit_score_outlined,
              ),
            ),
          if (widget.layout == PosPageLayout.b)
            ..._byDay(context, filtered)
          else ...[
            if (inTransit.isNotEmpty)
              _Group(
                title: 'Yolda',
                rows: [for (final item in inTransit) _row(context, item)],
              ),
            for (final deposit in deposits)
              _Group(
                title: 'Yatış · ${DateText.dayMonth(deposit.date)}',
                onOpen: () => showPosDepositDetail(
                  context,
                  repository: widget.controller.repository,
                  depositId: deposit.id,
                  changes: widget.controller.changes,
                ),
                rows: [for (final item in deposit.items) _row(context, item)],
              ),
          ],
        ],
      ),
    );
  }

  Widget _row(BuildContext context, PosSettlementItem item) => PosSettlementRow(
    item: item,
    onTap: () => openPosSettlementDetail(context, widget.controller, item),
  );

  /// B varyantı: İşlemler gibi satış gününe göre, en yeni gün üstte.
  List<Widget> _byDay(BuildContext context, List<PosSettlementItem> filtered) {
    final days = <String, List<PosSettlementItem>>{};
    for (final item in filtered) {
      days.putIfAbsent(item.settlementDate, () => []).add(item);
    }
    final ordered = days.keys.toList()..sort((a, b) => b.compareTo(a));
    return [
      for (final day in ordered)
        _Group(
          title: DateText.dayMonth(day),
          rows: [for (final item in days[day]!) _row(context, item)],
        ),
    ];
  }

  /// Hesaba geçenleri yatışına göre gruplar; en yeni yatış üstte.
  List<_Deposit> _byDeposit(List<PosSettlementItem> filtered) {
    final groups = <String, _Deposit>{};
    for (final item in filtered) {
      final id = item.posDepositId;
      if (item.isInTransit || id == null) continue;
      groups
          .putIfAbsent(id, () => _Deposit(id, item.transferredOn ?? ''))
          .items
          .add(item);
    }
    return groups.values.toList()..sort((a, b) => b.date.compareTo(a.date));
  }

  static String _iso(DateTime value) =>
      '${value.year.toString().padLeft(4, '0')}-'
      '${value.month.toString().padLeft(2, '0')}-'
      '${value.day.toString().padLeft(2, '0')}';
}

class _Deposit {
  _Deposit(this.id, this.date);

  final String id;
  final String date;
  final items = <PosSettlementItem>[];
}

/// Yatay kayan süzgeç satırı; İşlemler'deki hızlı süzgeçle aynı kalıp.
class _FilterRow extends StatelessWidget {
  const _FilterRow({required this.label, required this.children});

  final String label;
  final List<Widget> children;

  @override
  Widget build(BuildContext context) => Semantics(
    container: true,
    label: label,
    child: SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.xSmall,
        AppSpacing.medium,
        AppSpacing.xSmall,
      ),
      child: Row(
        children: [
          for (final child in children)
            Padding(
              padding: const EdgeInsets.only(right: AppSpacing.small),
              child: child,
            ),
        ],
      ),
    ),
  );
}

/// Bir grup tahsilat: `Yolda` ya da bir yatış. Yatış grubunun başlığı
/// yatışın ayrıntısını açar.
class _Group extends StatelessWidget {
  const _Group({required this.title, required this.rows, this.onOpen});

  final String title;
  final List<Widget> rows;
  final VoidCallback? onOpen;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.only(bottom: AppSpacing.medium),
      child: AppCard(
        padding: EdgeInsets.zero,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppRow(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.xSmall,
              ),
              title: title,
              titleStyle: theme.textTheme.titleSmall,
              trailing: onOpen == null
                  ? null
                  : Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Text(
                          'Yatışı gör',
                          style: theme.textTheme.labelLarge?.copyWith(
                            color: surfaces.inkMuted,
                          ),
                        ),
                        Icon(Icons.chevron_right, color: surfaces.inkMuted),
                      ],
                    ),
              onTap: onOpen,
            ),
            Divider(height: 1, thickness: 1, color: surfaces.border),
            AppDividedColumn(inset: AppIconCapsule.rowInset, children: rows),
          ],
        ),
      ),
    );
  }
}
