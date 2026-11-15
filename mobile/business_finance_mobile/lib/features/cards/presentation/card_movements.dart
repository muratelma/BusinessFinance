import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_day_heading.dart';
import '../../../core/widgets/app_filter_menu.dart';
import '../../../core/widgets/app_filter_sheet.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_no_results_view.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_section_empty.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../../../core/widgets/app_text_action.dart';
import '../../activities/data/activity_models.dart';
import '../../activities/presentation/activity_tile.dart';
import '../../finance_pages/data/finance_page_models.dart';
import '../data/card_page_models.dart';
import '../data/finance_models.dart';
import '../data/finance_repository.dart';

/// Bir kart hareketine dokununca ortak işlem ayrıntısı açılır; ayrıntıda
/// kayıt iptal edildiyse `true` döner ve liste yalnız o zaman tazelenir.
typedef OpenCardMovement = Future<bool> Function(FinancePageActivity row);

/// Kart hareketinin satırı: İşlemler'deki ortak satır ([ActivityTile]).
///
/// Harcama kategori simgesi ve gider renginde, eksi işaretli; kart ödemesi
/// mavi ve işaretsizdir. Alt yazı kategoriyi (taksitte `Taksit 1/9`) ve
/// ödemede hesabı söyler; [withDate] kart sayfasındaki bölümde günü öne
/// ekler (`12 Kasım · Taksit 1/9`). Hareketler sayfası günleri başlıkla
/// ayırdığı için gün yazmaz.
Widget cardMovementTile(
  FinancePageActivity row, {
  required bool withDate,
  VoidCallback? onTap,
}) {
  final a = row.activity;
  final payment = a.kind == ActivityKind.cardPayment;
  final title = a.kind == ActivityKind.cardCharge
      ? (a.description ?? a.categoryName ?? a.kindLabel)
      : payment
      ? a.kindLabel
      : (a.title.isEmpty ? a.kindLabel : a.title);
  final detail = <String>[
    if (row.installmentSequence != null && row.installmentCount != null)
      'Taksit ${row.installmentSequence}/${row.installmentCount}',
    if (payment && a.sourceName != null) a.sourceName!,
    if (a.kind == ActivityKind.debtPayment) a.kindLabel,
    if (a.kind == ActivityKind.cardCharge &&
        a.categoryName != null &&
        a.categoryName != title &&
        !(withDate && row.installmentSequence != null))
      a.categoryName!,
  ];
  return ActivityTile(
    activity: a,
    showDate: false,
    onTap: onTap,
    icon: payment
        ? Icons.credit_score_outlined
        : a.categoryName != null
        ? AppFinanceIcons.forCategory(a.categoryName!)
        : null,
    title: title,
    subtitle: [
      if (withDate) DateText.dayMonth(a.activityDate),
      ...detail,
    ].join(' · '),
    amount: row.amount,
    effect: payment ? ActivityEffect.neutral : ActivityEffect.expense,
    signedByAmount: !payment,
    hideFee: true,
  );
}

/// Kartın sayfasındaki Hareketler bölümü (teslim §1, R2): İşlemler gibi tam
/// genişlik beyaz blok, en çok 6 kayıt; başlıktaki `Tümü` Hareketler
/// sayfasını açar. Sayfanın son bölümüdür.
class CardMovementsSection extends StatefulWidget {
  const CardMovementsSection({
    required this.repository,
    required this.cardId,
    required this.revision,
    required this.onOpenAll,
    super.key,
    this.onOpenRow,
  });

  final FinanceRepositoryContract repository;
  final String cardId;

  /// Kartın verisi değiştiğinde artar; bölüm yeniden okunur.
  final int revision;
  final VoidCallback onOpenAll;
  final OpenCardMovement? onOpenRow;

  /// Bölümde gösterilen en çok satır.
  static const int previewCount = 6;

  @override
  State<CardMovementsSection> createState() => _CardMovementsSectionState();
}

class _CardMovementsSectionState extends State<CardMovementsSection> {
  late Future<CardPageData> _page = _load();

  Future<CardPageData> _load() => widget.repository.loadCardPage(
    widget.cardId,
    const CardMovementQuery(period: CardPeriodChoice.everything()),
    limit: CardMovementsSection.previewCount,
  );

  @override
  void didUpdateWidget(CardMovementsSection oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.revision != widget.revision ||
        oldWidget.cardId != widget.cardId) {
      _page = _load();
    }
  }

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      Padding(
        padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
        child: AppSectionHeader(
          title: 'Hareketler',
          padding: EdgeInsets.zero,
          trailing: AppTextAction(
            label: 'Tümü',
            trailingIcon: Icons.chevron_right,
            onPressed: widget.onOpenAll,
          ),
        ),
      ),
      FutureBuilder<CardPageData>(
        future: _page,
        builder: (context, snapshot) {
          // Tazelenirken eski satırlar yerinde kalır (FutureBuilder yeni
          // gelecekte eski veriyi tutar); yükleme görünümü sayfa boyunu
          // çökertip kaydırmayı yukarı atıyordu.
          if (snapshot.connectionState != ConnectionState.done &&
              !snapshot.hasData) {
            return const AppLoadingView(message: 'Kart hareketleri yükleniyor');
          }
          if (snapshot.hasError) {
            return Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppPageInsets.edge,
              ),
              child: AppErrorView(
                message: 'Kart hareketleri yüklenemedi.',
                onRetry: () => setState(() => _page = _load()),
              ),
            );
          }
          final rows = snapshot.data!.items;
          if (rows.isEmpty) {
            return const Padding(
              padding: EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
              child: AppSectionEmpty('Bu kartta henüz hareket yok.'),
            );
          }
          return AppCard(
            flat: true,
            padding: EdgeInsets.zero,
            child: Column(
              children: [
                for (final (index, row) in rows.indexed) ...[
                  if (index > 0) const Divider(height: 1),
                  cardMovementTile(
                    row,
                    withDate: true,
                    onTap: widget.onOpenRow == null
                        ? null
                        : () => widget.onOpenRow!(row),
                  ),
                ],
              ],
            ),
          );
        },
      ),
    ],
  );
}

/// Bir kartın bütün hareketleri (teslim §2): başlıkta ay düğmesi yoktur,
/// sağda filtre düğmesi ve uygulanan filtre sayısı durur. Özet kartı seçili
/// dönemi ve toplamları söyler; uygulanan filtreler özetin altında
/// kaldırılabilir çip olur. Varsayılan dönem (bu ekstre) çip üretmez.
class CardMovementsPage extends StatefulWidget {
  const CardMovementsPage({
    required this.repository,
    required this.card,
    super.key,
    this.onOpenRow,
    this.showScope = false,
  });

  final FinanceRepositoryContract repository;
  final CreditCardItem card;
  final OpenCardMovement? onOpenRow;

  /// Kapsam boyutu görünüyorsa filtrede `Kapsam` bölümü çizilir.
  final bool showScope;

  @override
  State<CardMovementsPage> createState() => _CardMovementsPageState();
}

class _CardMovementsPageState extends State<CardMovementsPage> {
  final _scroll = ScrollController();
  CardMovementQuery _query = const CardMovementQuery();
  CardPageData? _data;
  List<FinancePageActivity> _items = const [];
  bool _loading = true;
  bool _loadingMore = false;
  String? _error;
  int _generation = 0;

  @override
  void initState() {
    super.initState();
    _scroll.addListener(() {
      if (_scroll.hasClients && _scroll.position.extentAfter < 250) {
        _loadMore();
      }
    });
    _load();
  }

  @override
  void dispose() {
    _scroll.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    final generation = ++_generation;
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final data = await widget.repository.loadCardPage(widget.card.id, _query);
      if (!mounted || generation != _generation) return;
      setState(() {
        _data = data;
        _items = data.items;
        _loading = false;
      });
    } on ApiException catch (error) {
      if (!mounted || generation != _generation) return;
      setState(() {
        _error = error.message;
        _loading = false;
      });
    } on FormatException {
      if (!mounted || generation != _generation) return;
      setState(() {
        _error = 'Sunucudan beklenmeyen bir yanıt alındı.';
        _loading = false;
      });
    }
  }

  Future<void> _loadMore() async {
    final data = _data;
    if (data == null || !data.hasMore || _loadingMore || _loading) return;
    setState(() => _loadingMore = true);
    final generation = _generation;
    try {
      final next = await widget.repository.loadCardPage(
        widget.card.id,
        _query,
        offset: _items.length,
      );
      if (!mounted || generation != _generation) return;
      setState(() {
        _data = next;
        _items = [..._items, ...next.items];
      });
    } on Exception {
      // Bir sonraki kaydırmada yeniden denenir.
    } finally {
      if (mounted) setState(() => _loadingMore = false);
    }
  }

  /// Ayrıntıda bir kayıt değişince listeyi **yerinde** tazeler: o ana kadar
  /// yüklenmiş satır sayısı kadar yeniden okunur, eski satırlar okuma
  /// bitene kadar ekranda kalır. Yalnız ilk sayfayı okumak liste boyunu
  /// kısaltıp kullanıcıyı başa atıyordu.
  Future<void> _refreshInPlace() async {
    final generation = ++_generation;
    final wanted = _items.length;
    setState(() => _loading = true);
    try {
      var data = await widget.repository.loadCardPage(widget.card.id, _query);
      var items = data.items;
      while (data.hasMore && items.length < wanted) {
        data = await widget.repository.loadCardPage(
          widget.card.id,
          _query,
          offset: items.length,
        );
        items = [...items, ...data.items];
      }
      if (!mounted || generation != _generation) return;
      setState(() {
        _data = data;
        _items = items;
        _error = null;
        _loading = false;
      });
    } on ApiException catch (error) {
      if (!mounted || generation != _generation) return;
      setState(() {
        _error = error.message;
        _loading = false;
      });
    } on FormatException {
      if (!mounted || generation != _generation) return;
      setState(() {
        _error = 'Sunucudan beklenmeyen bir yanıt alındı.';
        _loading = false;
      });
    }
  }

  void _apply(CardMovementQuery query) {
    if (query == _query) return;
    _query = query.normalized();
    _load();
  }

  Future<void> _openFilter() async {
    final data = _data;
    final result = await AppAdaptiveSheet.show<CardMovementQuery>(
      context: context,
      builder: (_) => _CardFilterSheet(
        initial: _query,
        showScope: widget.showScope,
        categories: data?.categories ?? const [],
        statements: data?.statements ?? const [],
        currentRange: _currentRange(data),
        loadCategories: (draft) async => (await widget.repository.loadCardPage(
          widget.card.id,
          draft,
          limit: 1,
        )).categories,
      ),
    );
    if (result != null) _apply(result);
  }

  /// Açık ekstrenin dönemi; filtrede `Bu ekstre`nin yanında yazar.
  CardStatementRange? _currentRange(CardPageData? data) =>
      data?.statements.lastOrNull;

  int get _filterCount => [
    _query.kind != CardMovementKind.all,
    _query.scope != null,
    _query.categoryId != null,
    !_query.period.isCurrent,
  ].where((on) => on).length;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Hareketler',
            onBack: () => Navigator.maybePop(context),
            actions: [
              IconButton(
                tooltip: _filterCount == 0
                    ? 'Filtrele'
                    : 'Filtre, $_filterCount seçili',
                onPressed: _openFilter,
                icon: Badge(
                  isLabelVisible: _filterCount > 0,
                  label: Text('$_filterCount'),
                  child: const Icon(Icons.tune, size: 22),
                ),
              ),
            ],
          ),
          // İlerleme çubuğu listenin üstüne biner; listeyi aşağı itmez.
          Expanded(
            child: Stack(
              children: [
                Positioned.fill(child: _body()),
                if (_loading && _data != null)
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
  );

  Widget _body() {
    final data = _data;
    if (data == null) {
      return _error != null
          ? AppErrorView(message: _error!, onRetry: _load)
          : const AppLoadingView(message: 'Kart hareketleri yükleniyor');
    }
    final chips = _chips(data);
    return RefreshIndicator(
      onRefresh: _load,
      child: Align(
        alignment: Alignment.topCenter,
        child: ConstrainedBox(
          constraints: const BoxConstraints(
            maxWidth: AppBreakpoints.contentMaxWidth,
          ),
          child: ListView(
            controller: _scroll,
            physics: const AlwaysScrollableScrollPhysics(),
            padding: AppPageInsets.fullBleedList,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppPageInsets.edge,
                  AppSpacing.small,
                  AppPageInsets.edge,
                  0,
                ),
                child: _CardPeriodSummary(data: data),
              ),
              if (chips.isNotEmpty)
                SingleChildScrollView(
                  scrollDirection: Axis.horizontal,
                  padding: const EdgeInsets.fromLTRB(
                    AppPageInsets.edge,
                    AppSpacing.compact,
                    AppPageInsets.edge,
                    0,
                  ),
                  child: Row(
                    children: [
                      for (final (index, chip) in chips.indexed) ...[
                        if (index > 0) const SizedBox(width: AppGaps.chip),
                        chip,
                      ],
                    ],
                  ),
                ),
              if (_error != null)
                Padding(
                  padding: const EdgeInsets.all(AppPageInsets.edge),
                  child: Text(_error!),
                ),
              if (_items.isEmpty) _empty() else ..._days(),
              if (_loadingMore)
                const Padding(
                  padding: EdgeInsets.all(AppSpacing.medium),
                  child: Center(child: CircularProgressIndicator()),
                ),
            ],
          ),
        ),
      ),
    );
  }

  List<Widget> _chips(CardPageData data) {
    final category = data.categories
        .where((o) => o.id != null && o.id == _query.categoryId)
        .firstOrNull;
    return [
      if (_query.kind != CardMovementKind.all)
        _ActiveChip(
          label: _query.kind.label,
          onRemove: () => _apply(_query.copyWith(kind: CardMovementKind.all)),
        ),
      if (_query.scope != null)
        _ActiveChip(
          label: _query.scope!.label,
          onRemove: () => _apply(_query.copyWith(scope: () => null)),
        ),
      if (_query.categoryId != null)
        _ActiveChip(
          label: category?.name ?? 'Kategori',
          onRemove: () => _apply(_query.copyWith(categoryId: () => null)),
        ),
      if (!_query.period.isCurrent)
        _ActiveChip(
          label: _periodChip(_query.period, data),
          onRemove: () =>
              _apply(_query.copyWith(period: const CardPeriodChoice.current())),
        ),
    ];
  }

  static String _periodChip(CardPeriodChoice period, CardPageData data) {
    if (period.all) return 'Tüm zamanlar';
    if (period.isRange) {
      return DateText.dayRangeShort(period.from!, period.to!);
    }
    return '${DateText.months[period.month! - 1]} ekstresi';
  }

  Widget _empty() {
    final filtered =
        _query.kind != CardMovementKind.all ||
        _query.scope != null ||
        _query.categoryId != null;
    if (filtered) {
      return AppNoResultsView(
        reason: AppNoResultsReason.filter,
        title: 'Bu filtreye uyan hareket yok',
        message: AppNoResultsView.filterSentence([
          if (_query.kind != CardMovementKind.all) _query.kind.label,
          if (_query.scope != null) _query.scope!.label,
          if (_query.categoryId != null)
            _data?.categories
                    .where((o) => o.id == _query.categoryId)
                    .firstOrNull
                    ?.name ??
                'Kategori',
        ]),
        actionLabel: 'Filtreyi temizle',
        onAction: () => _apply(CardMovementQuery(period: _query.period)),
      );
    }
    if (_query.period.all) {
      return const AppNoResultsView(
        reason: AppNoResultsReason.period,
        title: 'Bu kartta hareket yok',
      );
    }
    return AppNoResultsView(
      reason: AppNoResultsReason.period,
      title: _query.period.isRange
          ? 'Bu aralıkta hareket yok'
          : 'Bu ekstrede hareket yok',
      actionLabel: 'Tüm zamanlara bak',
      onAction: () =>
          _apply(_query.copyWith(period: const CardPeriodChoice.everything())),
    );
  }

  List<Widget> _days() {
    final groups = <String, List<FinancePageActivity>>{};
    for (final row in _items) {
      groups.putIfAbsent(row.activity.activityDate, () => []).add(row);
    }
    return [
      for (final entry in groups.entries) ...[
        AppDayHeading(entry.key),
        AppCard(
          flat: true,
          padding: EdgeInsets.zero,
          child: Column(
            children: [
              for (final (index, row) in entry.value.indexed) ...[
                if (index > 0) const Divider(height: 1, indent: 72),
                cardMovementTile(
                  row,
                  withDate: false,
                  onTap: widget.onOpenRow == null
                      ? null
                      : () async {
                          if (await widget.onOpenRow!(row) && mounted) {
                            await _refreshInPlace();
                          }
                        },
                ),
              ],
            ],
          ),
        ),
      ],
    ];
  }
}

/// Özet kartı (teslim §2, O1): ilk satırda dönemin adı ve sağda durumu,
/// altında tarih aralığı; çizgi; `Harcama` ve `Ödeme` satırları. Tutarlar
/// sunucudandır ve filtreye uyar; seçim ödemeyi dışarıda bırakıyorsa ödeme
/// satırı çizilmez.
///
/// Kullanıcı düzeltmesi (12 Kasım 2026): durum adla aynı satırda; satır
/// simgeleri yazıyla aynı ortada; kartın alt boşluğu üstüyle aynı.
class _CardPeriodSummary extends StatelessWidget {
  const _CardPeriodSummary({required this.data});
  final CardPageData data;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final period = data.period;
    final title = switch (period.kind) {
      'all' => 'Tüm zamanlar',
      'range' => 'Seçilen aralık',
      _ => '${DateText.months[(period.month ?? 1) - 1]} ekstresi',
    };
    final range = period.start != null && period.end != null
        ? '${DateText.dayMonth(period.start!)} – ${DateText.dayMonth(period.end!)}'
        : period.kind == 'all'
        ? 'Kartın bütün hareketleri'
        : period.start != null
        ? '${DateText.dayMonth(period.start!)} sonrası'
        : period.end != null
        ? '${DateText.dayMonth(period.end!)} öncesi'
        : null;
    final status = switch (period.status) {
      'current' => const AppStatusTag(
        label: 'Açık dönem',
        icon: Icons.schedule,
        tone: AppStatusTone.planned,
      ),
      'paid' => const AppStatusTag(
        label: 'Ödendi',
        icon: Icons.check_circle_outline,
        tone: AppStatusTone.income,
      ),
      'overdue' => const AppStatusTag(
        label: 'Gecikti',
        icon: Icons.error_outline,
        tone: AppStatusTone.expense,
      ),
      'open' => const AppStatusTag(
        label: 'Ödeme bekliyor',
        icon: Icons.schedule,
        tone: AppStatusTone.neutral,
      ),
      _ => null,
    };
    return AppCard(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.step20,
        AppSpacing.medium,
        AppSpacing.step20,
        AppSpacing.small,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  title,
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontSize: AppTextSizes.row,
                    height: 1.3,
                  ),
                ),
              ),
              if (status != null) ...[
                const SizedBox(width: AppSpacing.small),
                status,
              ],
            ],
          ),
          if (range != null) ...[
            const SizedBox(height: AppSpacing.xSmall),
            Text(
              range,
              style: theme.textTheme.bodySmall?.copyWith(
                fontSize: AppTextSizes.caption,
                height: 1.35,
                color: surfaces.inkMuted,
              ),
            ),
          ],
          const SizedBox(height: AppSpacing.compact),
          Divider(height: 1, color: surfaces.border),
          const SizedBox(height: AppSpacing.xSmall),
          _SummaryRow(
            icon: Icons.shopping_bag_outlined,
            label: 'Harcama',
            amount: data.spending == '0.0000'
                ? data.spending
                : '-${data.spending}',
            effect: AppMoneyEffect.expense,
            signed: true,
          ),
          if (data.payments != null)
            _SummaryRow(
              icon: Icons.credit_score_outlined,
              label: 'Ödeme',
              amount: data.payments!,
              effect: AppMoneyEffect.neutral,
              signed: false,
            ),
        ],
      ),
    );
  }
}

class _SummaryRow extends StatelessWidget {
  const _SummaryRow({
    required this.icon,
    required this.label,
    required this.amount,
    required this.effect,
    required this.signed,
  });
  final IconData icon;
  final String label;
  final String amount;
  final AppMoneyEffect effect;
  final bool signed;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Icon(icon, size: 20, color: surfaces.inkMuted),
          const SizedBox(width: AppSpacing.compact),
          Expanded(
            child: Text(
              label,
              style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                fontSize: AppTextSizes.compact,
                height: 1.25,
              ),
            ),
          ),
          AppMoneyText(
            amount: amount,
            currency: 'TRY',
            effect: effect,
            signedByAmount: signed,
            size: AppMoneySize.row,
            style: const TextStyle(
              fontSize: AppTextSizes.row,
              fontWeight: FontWeight.w600,
              height: 1.25,
            ),
          ),
        ],
      ),
    );
  }
}

/// Uygulanan filtre çipi: koyu kenarsız kapsül ve ✕.
class _ActiveChip extends StatelessWidget {
  const _ActiveChip({required this.label, required this.onRemove});
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

/// Hareketleri filtrele (teslim §3, `KartFiltre5`): Tür · Kapsam · Dönem ·
/// Kategori. Tür `Ödeme` ise Kapsam ve Kategori kalkar (ödemenin ikisi de
/// yoktur). Dönemde `Bu ekstre`, `Tüm zamanlar` ve takvim; `Geçen ekstre`
/// yoktur, takvimin okları o işi yapar.
class _CardFilterSheet extends StatefulWidget {
  const _CardFilterSheet({
    required this.initial,
    required this.showScope,
    required this.categories,
    required this.statements,
    required this.currentRange,
    required this.loadCategories,
  });

  /// Taslağın dönemi, türü ve kapsamına göre kategoriler ve tutarları.
  /// Panel açıkken dönem değişince liste yeniden okunur; yoksa kullanıcı
  /// kategoriyi seçebilmek için önce sonuçları görmek zorunda kalırdı
  /// (kullanıcı, 15 Kasım 2026).
  final Future<List<FinancePageOption>> Function(CardMovementQuery draft)
  loadCategories;

  final CardMovementQuery initial;
  final bool showScope;
  final List<FinancePageOption> categories;
  final List<CardStatementRange> statements;
  final CardStatementRange? currentRange;

  @override
  State<_CardFilterSheet> createState() => _CardFilterSheetState();
}

class _CardFilterSheetState extends State<_CardFilterSheet> {
  late CardMovementQuery _draft = widget.initial;
  late List<FinancePageOption> _categories = widget.categories;
  int _categoryRead = 0;

  bool get _payment => _draft.kind == CardMovementKind.payment;

  /// Taslağı değiştirir; kategorilerin tutarını belirleyen bir seçim
  /// (dönem, tür, kapsam) değiştiyse liste yeni seçime göre okunur. Okuma
  /// sürerken ve okunamazsa eski liste yerinde kalır.
  void _change(CardMovementQuery next) {
    final previous = _draft;
    setState(() => _draft = next);
    if (next.kind == CardMovementKind.payment ||
        (next.period == previous.period &&
            next.kind == previous.kind &&
            next.scope == previous.scope)) {
      return;
    }
    final read = ++_categoryRead;
    widget.loadCategories(next).then((categories) {
      if (mounted && read == _categoryRead) {
        setState(() => _categories = categories);
      }
    }, onError: (Object _) {});
  }

  Future<void> _pickRange() async {
    final result = await showDialog<CardPeriodChoice>(
      context: context,
      builder: (_) => CardStatementRangeDialog(
        statements: widget.statements,
        initial: _draft.period,
      ),
    );
    if (result != null) {
      _change(_draft.copyWith(period: result));
    }
  }

  @override
  Widget build(BuildContext context) {
    final current = widget.currentRange;
    final period = _draft.period;
    final custom = !period.isCurrent && !period.all;
    final options = [
      for (final option in _categories)
        AppFilterOption(
          key: option.key == 'all' ? null : option.id,
          label: option.key == 'all'
              ? 'Bütün kategoriler'
              : option.name ?? 'Kategori',
          amount: option.amount,
          effect: AppMoneyEffect.expense,
          group: option.key == 'all' ? null : 'Gider',
          icon: option.key == 'all'
              ? Icons.layers_outlined
              : AppFinanceIcons.forCategory(option.name ?? ''),
        ),
    ];
    final selectedCategory = options
        .where((o) => o.key != null && o.key == _draft.categoryId)
        .firstOrNull;
    return AppFilterSheet(
      title: 'Hareketleri filtrele',
      onReset: _draft == const CardMovementQuery()
          ? null
          : () => _change(const CardMovementQuery()),
      onSubmit: () => Navigator.of(context).pop(_draft.normalized()),
      sections: [
        AppFilterSection(
          title: 'Tür',
          child: AppFilterSegments<CardMovementKind>(
            values: CardMovementKind.values,
            selected: _draft.kind,
            label: (v) => v.label,
            semanticLabel: 'Hareket türü',
            onChanged: (v) => _change(_draft.copyWith(kind: v)),
          ),
        ),
        if (widget.showScope && !_payment)
          AppFilterSection(
            title: 'Kapsam',
            child: AppFilterSegments<TransactionScope?>(
              values: const [
                null,
                TransactionScope.business,
                TransactionScope.personal,
              ],
              selected: _draft.scope,
              label: (v) => v?.label ?? 'Hepsi',
              semanticLabel: 'Kapsam',
              onChanged: (v) => _change(_draft.copyWith(scope: () => v)),
            ),
          ),
        AppFilterSection(
          title: 'Dönem',
          child: Padding(
            // Satırlar diğer alanların iç boşluğuyla hizalı.
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.compact),
            child: Column(
              children: [
                _RadioRow(
                  label: 'Bu ekstre',
                  trailing: current == null
                      ? null
                      : DateText.dayRangeShort(current.start, current.end),
                  selected: period.isCurrent,
                  onTap: () => _change(
                    _draft.copyWith(period: const CardPeriodChoice.current()),
                  ),
                ),
                _RadioRow(
                  label: 'Tüm zamanlar',
                  selected: period.all,
                  onTap: () => _change(
                    _draft.copyWith(
                      period: const CardPeriodChoice.everything(),
                    ),
                  ),
                ),
                _RangeRow(
                  label: custom
                      ? _choiceLabel(period, widget.statements)
                      : 'Tarih aralığı seç…',
                  selected: custom,
                  onTap: _pickRange,
                ),
              ],
            ),
          ),
        ),
        if (!_payment)
          AppFilterSection(
            title: 'Kategori',
            child: AppFilterSelectField(
              label: selectedCategory?.label ?? 'Bütün kategoriler',
              semanticLabel: 'Kategori',
              icon: Icons.layers_outlined,
              options: options,
              selected: _draft.categoryId,
              onSelected: (key) => setState(
                () => _draft = _draft.copyWith(categoryId: () => key),
              ),
            ),
          ),
      ],
    );
  }

  static String _choiceLabel(
    CardPeriodChoice period,
    List<CardStatementRange> statements,
  ) {
    if (period.isRange) return DateText.dayRangeShort(period.from!, period.to!);
    final range = statements
        .where((s) => s.year == period.year && s.month == period.month)
        .firstOrNull;
    final name = '${DateText.months[period.month! - 1]} ekstresi';
    return range == null
        ? name
        : '$name · ${DateText.dayRangeShort(range.start, range.end)}';
  }
}

class _RadioRow extends StatelessWidget {
  const _RadioRow({
    required this.label,
    required this.selected,
    required this.onTap,
    this.trailing,
  });
  final String label;
  final bool selected;
  final VoidCallback onTap;
  final String? trailing;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      inMutuallyExclusiveGroup: true,
      checked: selected,
      button: true,
      label: [label, ?trailing].join(', '),
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppRadius.small),
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Row(
            children: [
              Container(
                width: 20,
                height: 20,
                alignment: Alignment.center,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  border: Border.all(
                    width: 2,
                    color: selected
                        ? theme.colorScheme.primary
                        : surfaces.borderStrong,
                  ),
                ),
                child: selected
                    ? Container(
                        width: 10,
                        height: 10,
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          color: theme.colorScheme.primary,
                        ),
                      )
                    : null,
              ),
              const SizedBox(width: AppSpacing.step14),
              Expanded(
                child: Text(
                  label,
                  style: theme.textTheme.bodyLarge?.copyWith(
                    fontSize: AppTextSizes.compact,
                    height: 1.3,
                    fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
                  ),
                ),
              ),
              if (trailing != null)
                Text(
                  trailing!,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontSize: AppTextSizes.caption,
                    color: surfaces.inkMuted,
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

class _RangeRow extends StatelessWidget {
  const _RangeRow({
    required this.label,
    required this.selected,
    required this.onTap,
  });
  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      selected: selected,
      label: selected ? 'Tarih aralığı: $label' : label,
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppRadius.small),
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Row(
            children: [
              Icon(Icons.date_range_outlined, size: 20, color: surfaces.ink),
              const SizedBox(width: AppSpacing.step14),
              Expanded(
                child: Text(
                  label,
                  style: theme.textTheme.bodyLarge?.copyWith(
                    fontSize: AppTextSizes.compact,
                    height: 1.3,
                    fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
                  ),
                ),
              ),
              Icon(Icons.chevron_right, size: 20, color: surfaces.inkMuted),
            ],
          ),
        ),
      ),
    );
  }
}

/// Tarih aralığı · ekstreye göre (teslim §4; kullanıcı, 13 Kasım 2026'da
/// yeniden kuruldu). Yukarıdan aşağı:
///
/// - **Başlık:** ‹ seçim ›. Ekstre modunda ekstrenin adı ve aralığı; özel
///   aralıkta büyük satır seçilen aralık (`10 Eki – 10 Kas`), küçük satır
///   okların gösterdiği ekstre. Oklar ekstreler arasında gezer.
/// - **Mod:** `Ekstre | Özel aralık`. Ekstre modunda seçim, görünen ekstrenin
///   tamamıdır ve oklar seçimi de taşır. Özel aralıkta oklar yalnız sayfayı
///   çevirir, seçim korunur; böylece iki ekstreye yayılan bir aralık (10 Ekim –
///   10 Kasım) seçilebilir. Ekstre modunda bir güne dokunmak özel aralığa
///   geçer ve o günü başlangıç yapar.
/// - **Izgara:** ekstreyi kapsayan haftalar; ilk gün ve ayın 1'i küçük ay adı
///   taşır, ekstre modunda ekstre dışı günler soluktur.
/// - **Alt satır:** sağda `Vazgeç` ve `Seç` (kullanıcı, 13 Kasım 2026:
///   seçim üstte; altta ayrı bir seçim ya da ipucu yazısı yok).
class CardStatementRangeDialog extends StatefulWidget {
  const CardStatementRangeDialog({
    required this.statements,
    required this.initial,
    super.key,
  });

  /// Eskiden yeniye; sonuncusu açık ekstre.
  final List<CardStatementRange> statements;
  final CardPeriodChoice initial;

  @override
  State<CardStatementRangeDialog> createState() =>
      _CardStatementRangeDialogState();
}

class _CardStatementRangeDialogState extends State<CardStatementRangeDialog> {
  late int _index;
  DateTime? _start;
  DateTime? _end;
  bool _custom = false;

  @override
  void initState() {
    super.initState();
    final statements = widget.statements;
    _index = statements.isEmpty ? 0 : statements.length - 1;
    final initial = widget.initial;
    if (initial.isRange) {
      _custom = true;
      _start = DateTime.tryParse(initial.from!);
      _end = DateTime.tryParse(initial.to!);
      final found = statements.lastIndexWhere(
        (s) => _start != null && !DateTime.parse(s.start).isAfter(_start!),
      );
      if (found >= 0) _index = found;
    } else if (initial.year != null) {
      final found = statements.indexWhere(
        (s) => s.year == initial.year && s.month == initial.month,
      );
      if (found >= 0) _index = found;
      _selectStatement();
    } else {
      _selectStatement();
    }
  }

  CardStatementRange? get _statement =>
      widget.statements.isEmpty ? null : widget.statements[_index];

  void _selectStatement() {
    final statement = _statement;
    if (statement == null) return;
    _custom = false;
    _start = DateTime.parse(statement.start);
    _end = DateTime.parse(statement.end);
  }

  void _setMode(bool custom) => setState(() {
    if (custom == _custom) return;
    if (custom) {
      // Özel aralık boş başlar; ilk dokunuş başlangıçtır.
      _custom = true;
      _start = null;
      _end = null;
    } else {
      _selectStatement();
    }
  });

  /// Ekstre modunda ok o ekstreyi seçer; özel aralıkta yalnız sayfa çevrilir
  /// ve seçim korunur (kullanıcı, 13 Kasım 2026).
  void _shift(int by) => setState(() {
    _index = (_index + by).clamp(0, widget.statements.length - 1);
    if (!_custom) _selectStatement();
  });

  void _tap(DateTime day) => setState(() {
    if (!_custom) {
      _custom = true;
      _start = day;
      _end = null;
      return;
    }
    if (_start == null || _end != null) {
      _start = day;
      _end = null;
    } else if (day.isBefore(_start!)) {
      _start = day;
    } else {
      _end = day;
    }
  });

  static String _iso(DateTime d) =>
      '${d.year.toString().padLeft(4, '0')}-'
      '${d.month.toString().padLeft(2, '0')}-'
      '${d.day.toString().padLeft(2, '0')}';

  CardPeriodChoice _result(CardStatementRange statement) => _custom
      ? CardPeriodChoice.range(_iso(_start!), _iso(_end ?? _start!))
      : _index == widget.statements.length - 1
      ? const CardPeriodChoice.current()
      : CardPeriodChoice.statement(statement.year, statement.month);

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final statement = _statement;
    if (statement == null) {
      return const AlertDialog(content: Text('Henüz ekstre dönemi yok.'));
    }
    final first = DateTime.parse(statement.start);
    final last = DateTime.parse(statement.end);
    final gridStart = first.subtract(Duration(days: first.weekday - 1));
    final gridEnd = last.add(Duration(days: 7 - last.weekday));
    final days = [
      for (
        var d = gridStart;
        !d.isAfter(gridEnd);
        d = DateTime(d.year, d.month, d.day + 1)
      )
        d,
    ];
    final canNext = _index < widget.statements.length - 1;
    final pageName = '${DateText.months[statement.month - 1]} ekstresi';
    final pageRange = DateText.dayRangeShort(statement.start, statement.end);
    // Başlık seçimi söyler (kullanıcı, 13 Kasım 2026: seçim altta değil
    // üstte). Ekstre modunda seçim görünen ekstredir; özel aralıkta büyük
    // satır seçilen aralık, küçük satır okların gösterdiği ekstredir.
    final String title;
    final String subtitle;
    if (!_custom) {
      title = pageName;
      subtitle = pageRange;
    } else {
      title = _start == null
          ? 'Özel aralık'
          : _end == null
          ? '${DateText.dayShortMonth(_iso(_start!))} – …'
          : DateText.dayRangeShort(_iso(_start!), _iso(_end!));
      subtitle = pageName;
    }
    return Dialog(
      insetPadding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.large,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppRadius.sheet),
      ),
      clipBehavior: Clip.antiAlias,
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 380),
        child: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.small,
                  AppSpacing.medium,
                  AppSpacing.small,
                  AppSpacing.compact,
                ),
                child: Row(
                  children: [
                    IconButton(
                      tooltip: 'Önceki ekstre',
                      onPressed: _index > 0 ? () => _shift(-1) : null,
                      icon: const Icon(Icons.chevron_left),
                    ),
                    Expanded(
                      child: Semantics(
                        liveRegion: true,
                        child: Column(
                          children: [
                            Text(
                              title,
                              textAlign: TextAlign.center,
                              style: theme.textTheme.titleMedium?.copyWith(
                                fontWeight: FontWeight.w700,
                              ),
                            ),
                            const SizedBox(height: AppSpacing.xxSmall),
                            Text(
                              subtitle,
                              textAlign: TextAlign.center,
                              style: theme.textTheme.bodySmall?.copyWith(
                                fontSize: AppTextSizes.caption,
                                color: surfaces.inkMuted,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    IconButton(
                      tooltip: 'Sonraki ekstre',
                      onPressed: canNext ? () => _shift(1) : null,
                      icon: const Icon(Icons.chevron_right),
                    ),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                ),
                child: AppFilterSegments<bool>(
                  values: const [false, true],
                  selected: _custom,
                  label: (custom) => custom ? 'Özel aralık' : 'Ekstre',
                  semanticLabel: 'Seçim türü',
                  onChanged: _setMode,
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.compact,
                  AppSpacing.small,
                  AppSpacing.compact,
                  AppSpacing.compact,
                ),
                child: _Grid(
                  days: days,
                  first: first,
                  last: last,
                  start: _start,
                  end: _end,
                  custom: _custom,
                  onTap: _tap,
                ),
              ),
              Divider(height: 1, color: surfaces.border),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.large,
                  AppSpacing.compact,
                  AppSpacing.compact,
                  AppSpacing.compact,
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    TextButton(
                      onPressed: () => Navigator.of(context).pop(),
                      child: const Text('Vazgeç'),
                    ),
                    const SizedBox(width: AppSpacing.xSmall),
                    FilledButton(
                      onPressed: _start == null
                          ? null
                          : () => Navigator.of(context).pop(_result(statement)),
                      style: FilledButton.styleFrom(
                        shape: const StadiumBorder(),
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppSpacing.large,
                        ),
                      ),
                      child: const Text('Seç'),
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

class _Grid extends StatelessWidget {
  const _Grid({
    required this.days,
    required this.first,
    required this.last,
    required this.start,
    required this.end,
    required this.custom,
    required this.onTap,
  });
  final List<DateTime> days;
  final DateTime first;
  final DateTime last;
  final DateTime? start;
  final DateTime? end;
  final bool custom;
  final ValueChanged<DateTime> onTap;

  static const _weekdays = ['Pt', 'Sa', 'Ça', 'Pe', 'Cu', 'Ct', 'Pz'];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final brand = theme.colorScheme.primary;
    final band = theme.colorScheme.primaryContainer;
    final rows = <Widget>[
      Row(
        children: [
          for (final name in _weekdays)
            Expanded(
              child: SizedBox(
                height: 28,
                child: Center(
                  child: Text(
                    name,
                    style: theme.textTheme.bodySmall?.copyWith(
                      fontSize: AppTextSizes.caption,
                      color: surfaces.inkMuted,
                    ),
                  ),
                ),
              ),
            ),
        ],
      ),
    ];
    for (var w = 0; w < days.length; w += 7) {
      final week = days.sublist(w, w + 7);
      // Seçim şeridi haftada tek parça çizilir; gün gün çizilince günlerin
      // arasında ince dikişler görünüyordu.
      final banded = [for (final day in week) _banded(day)];
      final from = banded.indexOf(true);
      final to = banded.lastIndexOf(true);
      rows.add(
        Padding(
          padding: const EdgeInsets.only(top: AppSpacing.xSmall),
          child: LayoutBuilder(
            builder: (context, constraints) {
              final cell = constraints.maxWidth / 7;
              final roundLeft =
                  from == 0 ||
                  (from >= 0 && DateUtils.isSameDay(week[from], start));
              final roundRight =
                  to == 6 || (to >= 0 && DateUtils.isSameDay(week[to], end));
              return Stack(
                children: [
                  if (from >= 0)
                    Positioned(
                      left: cell * from,
                      width: cell * (to - from + 1),
                      top: 0,
                      bottom: 0,
                      child: DecoratedBox(
                        decoration: BoxDecoration(
                          color: band,
                          borderRadius: BorderRadius.horizontal(
                            left: Radius.circular(
                              roundLeft ? AppRadius.chip : 0,
                            ),
                            right: Radius.circular(
                              roundRight ? AppRadius.chip : 0,
                            ),
                          ),
                        ),
                      ),
                    ),
                  Row(
                    children: [
                      for (final day in week)
                        Expanded(child: _day(context, day, brand, surfaces)),
                    ],
                  ),
                ],
              );
            },
          ),
        ),
      );
    }
    return Column(mainAxisSize: MainAxisSize.min, children: rows);
  }

  /// Gün seçim şeridinin içinde mi (uçlar dahil, tek günlük seçim hariç).
  bool _banded(DateTime day) {
    if (start == null || end == null || DateUtils.isSameDay(start, end)) {
      return false;
    }
    return !day.isBefore(start!) && !day.isAfter(end!);
  }

  Widget _day(
    BuildContext context,
    DateTime day,
    Color brand,
    AppSurfaces surfaces,
  ) {
    final theme = Theme.of(context);
    final isStart = start != null && DateUtils.isSameDay(day, start);
    final isEnd =
        (end != null && DateUtils.isSameDay(day, end)) ||
        (end == null && isStart);
    final inside =
        start != null &&
        end != null &&
        day.isAfter(start!) &&
        day.isBefore(end!);
    final outside = !custom && (day.isBefore(first) || day.isAfter(last));
    final edge = isStart || isEnd;
    final showMonth = day == days.first || day.day == 1;
    return Semantics(
      button: true,
      selected: edge || inside,
      label: DateText.dayMonthYear(
        '${day.year}-${day.month.toString().padLeft(2, '0')}-${day.day.toString().padLeft(2, '0')}',
      ),
      excludeSemantics: true,
      child: GestureDetector(
        behavior: HitTestBehavior.opaque,
        onTap: () => onTap(day),
        child: Container(
          height: 46,
          alignment: Alignment.center,
          child: Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: edge ? brand : null,
            ),
            alignment: Alignment.center,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (showMonth)
                  Text(
                    DateText.shortMonths[day.month - 1],
                    style: theme.textTheme.labelSmall?.copyWith(
                      fontSize: AppTextSizes.leafMonth,
                      height: 1.1,
                      fontWeight: FontWeight.w600,
                      color: edge
                          ? theme.colorScheme.onPrimary
                          : surfaces.inkMuted,
                    ),
                  ),
                Text(
                  '${day.day}',
                  style: theme.textTheme.bodyMedium?.copyWith(
                    height: 1.1,
                    fontWeight: edge ? FontWeight.w700 : FontWeight.w400,
                    color: edge
                        ? theme.colorScheme.onPrimary
                        : outside
                        ? surfaces.inkFaint
                        : surfaces.ink,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
