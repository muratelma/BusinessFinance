import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_filter_sheet.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../accounts/presentation/account_directory.dart';
import '../../activities/data/activity_models.dart';
import '../../activities/data/activity_repository.dart';
import '../../activities/presentation/activity_controller.dart';
import '../../activities/presentation/show_activity_detail.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'counterparty_page_controller.dart';
import 'counterparty_page_view.dart';
import 'debt_discount_sheet.dart';
import 'debt_entry_sheet.dart';
import 'debt_settle_sheet.dart';

/// Kişi sayfası (Faz 6): özet, `Tahsilat` / `Ödeme`, taksitli kayıtlar ve
/// hareketler. Yeni kayıt formları Faz 7'de bağlanır; o zamana kadar `+`
/// seçimi [entryPage]'i kişi dolu açar, `⋮` eski kişi sayfasını
/// (düzenle, açılış bakiyesi, sil) açar.
class CounterpartyPage extends StatefulWidget {
  const CounterpartyPage({
    required this.repository,
    required this.counterpartyId,
    required this.name,
    super.key,
    this.activityRepository,
    this.changes,
    this.entryPage,
    this.managePage,
    this.recordPage,
  });

  final DebtBookRepositoryContract repository;
  final String counterpartyId;

  /// Sayfa okunana kadar başlıkta yazan ad.
  final String name;
  final ActivityRepositoryContract? activityRepository;
  final FinancialDataChanges? changes;

  /// Yeni kayıt formu; kişi dolu ve kilitli gelir.
  final Widget Function(DebtEntryKind kind, DebtPersonChoice? person)?
  entryPage;

  /// Eski kişi sayfası (düzenle, açılış bakiyesi, sil); `⋮` açar ve dönüşte
  /// sayfa yenilenir. Faz 7–8'de yeni menüyle değişir.
  final WidgetBuilder? managePage;

  /// Taksitli kaydın sayfası (kimlik, başlık); dönüşte sayfa yenilenir.
  final Widget Function(String debtId, String title)? recordPage;

  @override
  State<CounterpartyPage> createState() => _CounterpartyPageState();
}

class _CounterpartyPageState extends State<CounterpartyPage> {
  late final CounterpartyPageController _controller =
      CounterpartyPageController(
        widget.repository,
        counterpartyId: widget.counterpartyId,
        changes: widget.changes,
      );

  /// Ayrıntı paneli ve iptal için; akışı kendisi izlemez.
  late final ActivityController? _activities = widget.activityRepository == null
      ? null
      : ActivityController(
          widget.activityRepository!,
          financialDataChanges: widget.changes,
          observeChanges: false,
        );

  @override
  void initState() {
    super.initState();
    _controller.load();
  }

  @override
  void dispose() {
    _controller.dispose();
    _activities?.dispose();
    super.dispose();
  }

  bool get _scopeVisible {
    try {
      return context.read<ScopeController?>()?.isVisible ?? false;
    } on ProviderNotFoundException {
      return false;
    }
  }

  Future<void> _add(CounterpartyPageData page) async {
    final kind = await AppAdaptiveSheet.show<DebtEntryKind>(
      context: context,
      builder: (_) =>
          DebtEntrySheet(personName: page.name, personActive: page.isActive),
    );
    final builder = widget.entryPage;
    if (kind == null || builder == null || !mounted) return;
    final person = DebtPersonChoice(
      id: page.id,
      name: page.name,
      scope: page.scope,
    );
    await Navigator.of(
      context,
    ).push(MaterialPageRoute<bool>(builder: (_) => builder(kind, person)));
    if (mounted) await _controller.load();
  }

  Future<void> _settle(CounterpartyPageData page, DebtSide side) async {
    final DebtSettleSources sources;
    try {
      sources = await _controller.loadSources();
    } on Exception {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Hesaplar okunamadı. Tekrar deneyin.')),
      );
      return;
    }
    if (!mounted) return;
    // `AccountChoices.of` rehberi izler (`watch`); yalnız build içinde
    // okunabilir, bu yüzden build'de alınıp burada kullanılır.
    final choices = _choices;
    final collect = side == DebtSide.receivable;
    final options = [
      for (final account in sources.accounts)
        choices
            .account(account.id, account.name, type: account.type)
            .withId('account:${account.id}'),
      if (collect)
        for (final pos in sources.posDefinitions)
          choices
              .pos(pos.id, pos.name, accountName: pos.note)
              .withId('pos:${pos.id}')
      else
        for (final card in sources.cards)
          choices.card(card.id, card.name).withId('card:${card.id}'),
    ];
    await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => DebtSettleSheet(
        side: side,
        personName: page.name,
        shortcuts: collect ? page.collect : page.pay,
        options: options,
        initialAccountId: options.firstOrNull?.id,
        today: _controller.today,
        onSubmit: _controller.settle,
      ),
    );
  }

  Future<bool> _discount(FinancialActivity charge) async {
    final done = await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => DebtDiscountSheet(
        title: charge.title,
        amount: charge.amount,
        isSale: charge.direction != ActivityDirection.payable,
        today: _controller.today,
        onSubmit: (amount, date, requestId) => _controller.discount(
          charge,
          amount: amount,
          date: date,
          clientRequestId: requestId,
        ),
      ),
    );
    return done ?? false;
  }

  Future<void> _openRecord(DebtBookItem record) async {
    final builder = widget.recordPage!;
    await Navigator.of(context).push(
      MaterialPageRoute<void>(builder: (_) => builder(record.id, record.title)),
    );
    if (mounted) await _controller.load();
  }

  Future<void> _manage() async {
    await Navigator.of(
      context,
    ).push(MaterialPageRoute<void>(builder: widget.managePage!));
    if (mounted) await _controller.load();
  }

  Future<void> _filter(CounterpartyPageData page) async {
    final twoWay = page.owesYou && page.youOwe;
    final value = await AppAdaptiveSheet.show<PersonMovementFilter>(
      context: context,
      builder: (_) =>
          _MovementFilterSheet(initial: _controller.filter, showSide: twoWay),
    );
    if (value != null) await _controller.applyFilter(value);
  }

  Future<void> _openMovement(FinancialActivity activity) async {
    final activities = _activities;
    if (activities == null) return;
    final discountable =
        activity.kind == ActivityKind.counterpartyCharge &&
        activity.canCancel &&
        !activity.isCancelled;
    final changed = await showActivityDetail(
      context,
      activity: activity,
      controller: activities,
      changes: widget.changes,
      onDiscount: discountable ? _discount : null,
    );
    if (changed && mounted) await _controller.load();
  }

  late AccountChoices _choices;

  @override
  Widget build(BuildContext context) {
    _choices = AccountChoices.of(context);
    return _scaffold(context);
  }

  Widget _scaffold(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: AnimatedBuilder(
        animation: _controller,
        builder: (context, _) {
          final page = _controller.page;
          void back() => Navigator.maybePop(context);
          if (_controller.unauthorized) {
            return _framed(back, const AppUnauthorizedView());
          }
          if (page == null) {
            return _framed(
              back,
              _controller.errorMessage != null
                  ? AppErrorView(
                      message: _controller.errorMessage!,
                      onRetry: _controller.load,
                    )
                  : const AppLoadingView(message: 'Kişi yükleniyor'),
            );
          }
          return CounterpartyPageView(
            page: page,
            movements: _controller.movements,
            scopeVisible: _scopeVisible,
            onBack: back,
            onAdd: () => _add(page),
            onMore: widget.managePage == null ? null : _manage,
            onCollect: () => _settle(page, DebtSide.receivable),
            onPay: () => _settle(page, DebtSide.payable),
            onOpenRecord: widget.recordPage == null ? null : _openRecord,
            onOpenMovement: _openMovement,
            onFilter: () => _filter(page),
            filterLabels: _controller.filter.labels,
            onClearFilter: () =>
                _controller.applyFilter(const PersonMovementFilter()),
            hasMore: _controller.hasMore,
            isLoadingMore: _controller.isLoadingMore,
            onLoadMore: _controller.loadMore,
            onRefresh: _controller.load,
            notice: _controller.isStale
                ? AppInlineNotice(
                    margin: EdgeInsets.zero,
                    message: 'Sayfa güncellenemedi.',
                    actionLabel: 'Tekrar dene',
                    onAction: _controller.load,
                  )
                : null,
          );
        },
      ),
    ),
  );

  Widget _framed(VoidCallback back, Widget body) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      AppPageHeader(title: widget.name, onBack: back),
      Expanded(child: body),
    ],
  );
}

/// Hareket süzgeci: dönem, tür ve (iki yönlü kişide) taraf (K15).
class _MovementFilterSheet extends StatefulWidget {
  const _MovementFilterSheet({required this.initial, required this.showSide});

  final PersonMovementFilter initial;
  final bool showSide;

  @override
  State<_MovementFilterSheet> createState() => _MovementFilterSheetState();
}

class _MovementFilterSheetState extends State<_MovementFilterSheet> {
  late PersonMovementPeriod _period = widget.initial.period;
  late PersonMovementKind _kind = widget.initial.kind;
  late DebtSide? _side = widget.initial.side;

  PersonMovementFilter get _value =>
      PersonMovementFilter(period: _period, kind: _kind, side: _side);

  @override
  Widget build(BuildContext context) => AppFilterSheet(
    title: 'Hareketleri süz',
    onReset: _value.isEmpty
        ? null
        : () => setState(() {
            _period = PersonMovementPeriod.all;
            _kind = PersonMovementKind.all;
            _side = null;
          }),
    onSubmit: () => Navigator.of(context).pop(_value),
    sections: [
      AppFilterSection(
        title: 'Dönem',
        child: AppFilterSegments<PersonMovementPeriod>(
          values: PersonMovementPeriod.values,
          selected: _period,
          label: (value) => value.label,
          onChanged: (value) => setState(() => _period = value),
          semanticLabel: 'Dönem',
        ),
      ),
      AppFilterSection(
        title: 'Tür',
        child: AppFilterSegments<PersonMovementKind>(
          values: PersonMovementKind.values,
          selected: _kind,
          label: (value) => value.label,
          onChanged: (value) => setState(() => _kind = value),
          semanticLabel: 'Tür',
        ),
      ),
      if (widget.showSide)
        AppFilterSection(
          title: 'Taraf',
          child: AppFilterSegments<DebtSide?>(
            values: const [null, DebtSide.receivable, DebtSide.payable],
            selected: _side,
            label: (value) => switch (value) {
              null => 'Hepsi',
              DebtSide.receivable => 'Alacak',
              DebtSide.payable => 'Borç',
            },
            onChanged: (value) => setState(() => _side = value),
            semanticLabel: 'Taraf',
          ),
        ),
    ],
  );
}
