import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/loadable_view_model.dart';
import '../../activities/data/activity_models.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'debt_settle_sheet.dart';

/// Kişi sayfasının durumu (Faz 6): özet, sayfalı hareketler ve süzgeç;
/// tahsilat ve ödeme yazma. Özet süzgeçten etkilenmez (K15).
class CounterpartyPageController extends LoadableViewModel {
  CounterpartyPageController(
    this._repository, {
    required this.counterpartyId,
    FinancialDataChanges? changes,
    DateTime Function()? now,
  }) : _changes = changes,
       _now = now ?? DateTime.now {
    _seenCounterparties = changes?.counterpartiesRevision ?? 0;
    _seenFeed = changes?.activityFeedRevision ?? 0;
    _changes?.addListener(_handleChanges);
  }

  static const int pageSize = 20;

  final DebtBookRepositoryContract _repository;
  final String counterpartyId;
  final FinancialDataChanges? _changes;
  final DateTime Function() _now;
  int _seenCounterparties = 0;
  int _seenFeed = 0;
  int _generation = 0;

  CounterpartyPageData? page;
  List<FinancialActivity> movements = const [];
  int _loadedPages = 0;
  int _totalCount = 0;
  bool isLoadingMore = false;
  PersonMovementFilter filter = const PersonMovementFilter();
  DebtSettleSources? sources;

  bool get unauthorized => error?.isUnauthorized ?? false;
  String? get errorMessage => error?.message;
  bool get isStale => page != null && error != null && !unauthorized;
  bool get hasMore => movements.length < _totalCount;
  DateTime get today => _now();

  String get todayIso {
    final day = _now();
    return '${day.year.toString().padLeft(4, '0')}-'
        '${day.month.toString().padLeft(2, '0')}-'
        '${day.day.toString().padLeft(2, '0')}';
  }

  Future<void> load() {
    final generation = ++_generation;
    return loadSafely(() async {
      final results = await Future.wait([
        _repository.loadPerson(counterpartyId, asOfDate: todayIso),
        _repository.loadMovements(
          counterpartyId,
          filter: filter,
          today: _now(),
          pageSize: pageSize,
        ),
      ]);
      if (generation != _generation) return;
      page = results[0] as CounterpartyPageData;
      final first = results[1] as ActivityPage;
      movements = first.items;
      _totalCount = first.pagination.totalCount;
      _loadedPages = 1;
    }, isCurrent: () => generation == _generation);
  }

  /// Süzgeç yalnız hareketleri değiştirir; özet güncel kalır.
  Future<void> applyFilter(PersonMovementFilter value) {
    filter = value;
    return load();
  }

  Future<void> loadMore() async {
    if (isLoadingMore || !hasMore) return;
    isLoadingMore = true;
    notifyListeners();
    try {
      final next = await _repository.loadMovements(
        counterpartyId,
        filter: filter,
        today: _now(),
        pageNumber: _loadedPages + 1,
        pageSize: pageSize,
      );
      movements = [...movements, ...next.items];
      _totalCount = next.pagination.totalCount;
      _loadedPages++;
    } on ApiException {
      // Liste yerinde kalır; kullanıcı yeniden dener.
    } finally {
      isLoadingMore = false;
      notifyListeners();
    }
  }

  Future<DebtSettleSources> loadSources() async =>
      sources ??= await _repository.loadSettleSources();

  /// Tahsilat ya da ödemeyi yazar. Yazıldıysa `null`, reddedildiyse panelin
  /// içinde gösterilecek cümle.
  Future<String?> settle(DebtSettlement settlement) async {
    final source = settlement.sourceId;
    final colon = source.indexOf(':');
    final kind = source.substring(0, colon);
    final id = source.substring(colon + 1);
    final body = <String, Object?>{
      'direction': settlement.side.apiValue,
      'amount': settlement.amount,
      'currency': 'TRY',
      'paymentDate': settlement.date,
      'description': settlement.description,
      'clientRequestId': settlement.clientRequestId,
      if (kind == 'account') 'accountId': id,
      if (kind == 'card') 'creditCardId': id,
      if (kind == 'pos') 'card': {'posDefinitionId': id},
      if (settlement.closeRemainderAsDiscount) 'closeRemainderAsDiscount': true,
    };
    try {
      await _repository.settle(counterpartyId, body);
    } on ApiException catch (error) {
      return error.message;
    }
    // Tahsilat kasayı (POS'ta yoldaki parayı), kartla ödeme kart borcunu
    // değiştirir; bütçe değişmez (gelir/gider satış günü yazıldı). Kalanı
    // indirim saymak gider yazar: o zaman bütçe de değişir.
    if (kind == 'card') {
      _changes?.cardPaymentChanged();
    }
    _changes?.counterpartyLedgerChanged();
    await load();
    return null;
  }

  /// Satış ya da alıma indirim. Yazıldıysa `null`, reddedildiyse cümle.
  /// Gelir ya da gider değişir: bütçe ve rapor da yenilenir.
  Future<String?> discount(
    FinancialActivity charge, {
    required String amount,
    required String date,
    required String clientRequestId,
  }) async {
    try {
      await _repository.discountCharge(charge.activityId, {
        'amount': amount,
        'discountDate': date,
        'clientRequestId': clientRequestId,
      });
    } on ApiException catch (error) {
      return error.message;
    }
    _changes?.counterpartyLedgerChanged();
    await load();
    return null;
  }

  /// Kişinin adı, notu ve türü. Tür boşsa değişmez.
  Future<String?> edit({
    required String name,
    String? note,
    TransactionScope? scope,
  }) => _writePerson(
    () => _repository.updateCounterparty(
      counterpartyId,
      name: name,
      isActive: page?.isActive ?? true,
      note: note,
      scope: scope,
    ),
  );

  /// Pasif kişiye yeni satış, alım ve borç para yazılmaz; tahsilat ve ödeme
  /// yazılır.
  Future<String?> setActive(bool isActive) {
    final current = page;
    if (current == null) return Future.value();
    return _writePerson(
      () => _repository.updateCounterparty(
        counterpartyId,
        name: current.name,
        isActive: isActive,
        note: current.note,
      ),
    );
  }

  /// Uygulamadan önceki bakiye; ikisi de sıfır ya da artı, sıfır devri
  /// kaldırır.
  Future<String?> setOpening({
    required String receivable,
    required String payable,
  }) => _writePerson(
    () => _repository.setOpeningBalance(
      counterpartyId,
      receivable: receivable,
      payable: payable,
    ),
  );

  /// Silindiyse `null`, reddedildiyse sunucunun cümlesi.
  Future<String?> delete() async {
    try {
      await _repository.deleteCounterparty(counterpartyId);
    } on ApiException catch (error) {
      return error.message;
    }
    _changes?.counterpartyLedgerChanged();
    return null;
  }

  Future<String?> _writePerson(Future<void> Function() write) async {
    try {
      await write();
    } on ApiException catch (error) {
      return error.message;
    }
    _changes?.counterpartyLedgerChanged();
    await load();
    return null;
  }

  void _handleChanges() {
    final changes = _changes;
    if (changes == null) return;
    final counterparties = changes.counterpartiesRevision;
    final feed = changes.activityFeedRevision;
    if (counterparties == _seenCounterparties && feed == _seenFeed) return;
    _seenCounterparties = counterparties;
    _seenFeed = feed;
    load();
  }

  @override
  void dispose() {
    _changes?.removeListener(_handleChanges);
    super.dispose();
  }
}
