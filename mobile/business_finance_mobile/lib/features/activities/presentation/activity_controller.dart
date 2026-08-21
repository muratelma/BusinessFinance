import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/activity_models.dart';
import '../data/activity_repository.dart';

class ActivityController extends ChangeNotifier {
  ActivityController(this._repository, {this.financialDataChanges})
    : _seenRevision = financialDataChanges?.activityFeedRevision ?? 0 {
    financialDataChanges?.addListener(_handleFinancialDataChanged);
  }

  final ActivityRepositoryContract _repository;
  final FinancialDataChanges? financialDataChanges;
  int _seenRevision;

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

  bool get hasMore => pagination?.hasNextPage ?? false;
  bool get isEmpty => !isLoading && errorMessage == null && _items.isEmpty;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      final page = await _repository.list(pageNumber: 1, filter: filter);
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

  Future<void> clearAdvancedFilters() =>
      applyFilter(ActivityFilter(quickFilter: filter.quickFilter));

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
    super.dispose();
  }
}
