import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../data/activity_models.dart';
import '../data/activity_repository.dart';

class ActivityController extends ChangeNotifier {
  ActivityController(
    this._repository, {
    this.financialDataChanges,
    this.scopeController,
  }) : _seenRevision = financialDataChanges?.activityFeedRevision ?? 0,
       _seenScope = scopeController?.scope {
    financialDataChanges?.addListener(_handleFinancialDataChanged);
    scopeController?.addListener(_handleScopeChanged);
  }

  final ActivityRepositoryContract _repository;
  final FinancialDataChanges? financialDataChanges;

  /// Uygulama genelindeki kapsam anahtarı; feed'in kendi filtresi değildir.
  final ScopeController? scopeController;
  int _seenRevision;
  TransactionScope? _seenScope;

  bool isLoading = false;
  bool isLoadingMore = false;
  bool unauthorized = false;
  String? errorMessage;

  /// True while the visible list is known to be behind the server, so the screen
  /// can say so instead of quietly showing yesterday's numbers.
  bool isStale = false;

  ActivityFilter filter = const ActivityFilter();
  ActivityPagination? pagination;

  final List<FinancialActivity> _items = [];
  List<FinancialActivity> get items => List.unmodifiable(_items);

  /// Listenin o an okuduğu kapsam; başlıkta da bu yazılı durur.
  TransactionScope? get scope => scopeController?.scope;

  bool get isScopeVisible => scopeController?.isVisible ?? false;

  bool get hasMore => pagination?.hasNextPage ?? false;
  bool get isEmpty => !isLoading && errorMessage == null && _items.isEmpty;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      final page = await _repository.list(
        pageNumber: 1,
        filter: filter,
        scope: scope,
      );
      _items
        ..clear()
        ..addAll(page.items);
      pagination = page.pagination;
      unauthorized = false;
      isStale = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir hareket yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  /// Appends the next page. Ids are checked against what is already shown: a
  /// movement created while the user was reading shifts the pages, and without
  /// this guard the same row would appear twice.
  Future<void> loadMore() async {
    final current = pagination;
    if (isLoading || isLoadingMore || current == null || !current.hasNextPage) {
      return;
    }
    isLoadingMore = true;
    notifyListeners();
    try {
      final page = await _repository.list(
        pageNumber: current.pageNumber + 1,
        filter: filter,
        scope: scope,
      );
      final seen = _items.map((item) => item.listKey).toSet();
      _items.addAll(page.items.where((item) => !seen.contains(item.listKey)));
      pagination = page.pagination;
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir hareket yanıtı alındı.';
    } finally {
      isLoadingMore = false;
      notifyListeners();
    }
  }

  bool isCancelling = false;
  String? successMessage;

  /// Cancels a movement and reloads. The lock is the only defence against a
  /// double tap creating two requests: the server has no idempotency key yet, so
  /// a second call would be a second write.
  Future<bool> cancel(FinancialActivity activity) async {
    if (isCancelling || !activity.canCancel) return false;
    isCancelling = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await _repository.cancel(activity);
      // Only the screens this kind can affect are refreshed: cancelling a
      // transfer cannot change a budget, and cancelling a card payment is not a
      // second expense.
      switch (activity.kind) {
        case ActivityKind.accountTransaction:
          financialDataChanges?.transactionsChanged();
        case ActivityKind.transfer:
          financialDataChanges?.transferChanged();
        case ActivityKind.cardCharge:
          financialDataChanges?.cardSpendingChanged();
        case ActivityKind.cardPayment:
          financialDataChanges?.cardPaymentChanged();
        case ActivityKind.debtPayment:
        case ActivityKind.debtCollection:
        case ActivityKind.debtOpening:
          financialDataChanges?.debtChanged();
        // Borçlandırmanın iptali tanınan gelir/gideri, tahsilatınki
        // kasadaki parayı geri alır; ikisi de cari bakiyeyi değiştirir.
        case ActivityKind.counterpartyCharge:
        case ActivityKind.counterpartySettlement:
          financialDataChanges?.counterpartyLedgerChanged();
        // Bu beş tür feed üzerinden iptal edilemiyor (canCancel:false), yani
        // buraya hiç düşmezler. Yine de sessiz bir dal bırakmak, ileride biri
        // iptal edilebilir olduğunda hangi ekranların yenileneceğini kimseye
        // sormadan geçirirdi.
        case ActivityKind.obligationSettlement:
        case ActivityKind.posSale:
        case ActivityKind.posDeposit:
          financialDataChanges?.transactionsChanged();
        case ActivityKind.obligation:
          financialDataChanges?.obligationRecognized();
      }
      successMessage = 'Hareket iptal edildi.';
      await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir hareket yanıtı alındı.';
      return false;
    } finally {
      isCancelling = false;
      notifyListeners();
    }
  }

  /// İşlem ayrıntısındaki "kalan bakiye". Hata ayrıntıyı bozmaz: satır
  /// gösterilmez, panel geri kalanıyla açılır.
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async {
    try {
      return await _repository.balancesAfter(activity);
    } on ApiException {
      return const [];
    } on FormatException {
      return const [];
    }
  }

  void clearMessages() {
    errorMessage = null;
    successMessage = null;
    notifyListeners();
  }

  Future<void> applyFilter(ActivityFilter value) async {
    filter = value;
    await load();
  }

  Future<void> selectQuickFilter(ActivityQuickFilter value) =>
      applyFilter(filter.copyWith(quickFilter: value));

  /// Aramayı uygular; boş metin aramayı kaldırır. Aynı metin ikinci kez
  /// istek göndermez.
  Future<void> search(String text) {
    final trimmed = text.trim();
    final next = trimmed.isEmpty ? null : trimmed;
    if (next == filter.search) return Future.value();
    return applyFilter(
      next == null
          ? filter.copyWith(clearSearch: true)
          : filter.copyWith(search: next),
    );
  }

  Future<void> clearAdvancedFilters() => applyFilter(
    ActivityFilter(quickFilter: filter.quickFilter, search: filter.search),
  );

  void _handleScopeChanged() {
    final current = scopeController?.scope;
    if (current == _seenScope) return;
    _seenScope = current;
    // Sayfalama baştan kurulur: kapsam değişince satır kümesi değişiyor ve
    // eski sayfa numarası artık başka bir listeye işaret ediyor.
    if (!isLoading) load();
  }

  void _handleFinancialDataChanged() {
    final changes = financialDataChanges;
    if (changes == null) return;
    final revision = changes.activityFeedRevision;
    if (revision == _seenRevision) return;
    _seenRevision = revision;
    if (isLoading) {
      // A reload is already in flight against the previous state, so mark the
      // result as behind rather than racing a second request.
      isStale = true;
      notifyListeners();
      return;
    }
    load();
  }

  @override
  void dispose() {
    financialDataChanges?.removeListener(_handleFinancialDataChanged);
    scopeController?.removeListener(_handleScopeChanged);
    super.dispose();
  }
}
