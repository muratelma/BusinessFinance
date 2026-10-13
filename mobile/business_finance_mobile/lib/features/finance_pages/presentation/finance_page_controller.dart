import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../data/finance_page_models.dart';
import '../data/finance_page_repository.dart';

enum FinancePageKind { summary, category, account, accounts, transit }

/// Sayfa okumaları ve filtreler. Para ve oran burada hesaplanmaz.
class FinancePageController extends ChangeNotifier {
  FinancePageController(
    this.repository, {
    required this.kind,
    this.id,
    DateTime? month,
    this.income = false,
    this.scope,
    this.source,
    this.person,
    this.changes,
    this.scopeController,
  }) : month = DateTime(
         (month ?? DateTime.now()).year,
         (month ?? DateTime.now()).month,
       ) {
    changes?.addListener(_changed);
    _revisions = _revision();
    if (kind == FinancePageKind.summary) {
      scope = scopeController?.scope ?? scope;
      scopeController?.addListener(_scopeChanged);
    }
  }
  final FinancePageRepositoryContract repository;
  final FinancePageKind kind;
  final String? id;
  final FinancialDataChanges? changes;
  final ScopeController? scopeController;
  DateTime month;
  bool income;
  TransactionScope? scope;
  String? source;
  String? person;
  String? direction;
  String? selection;
  CategorySummaryData? summary;
  CategoryPageData? category;
  AccountDetailData? account;
  AccountsOverviewData? accounts;
  TransitPageData? transit;
  bool isLoading = false;
  bool isLoadingMore = false;
  bool unauthorized = false;
  bool isStale = false;
  String? errorMessage;
  int _generation = 0;
  bool _disposed = false;
  late List<int> _revisions;

  CategorySummaryData? get categorySummary => summary ?? category?.summary;
  bool get hasData => switch (kind) {
    FinancePageKind.summary => summary != null,
    FinancePageKind.category => category != null,
    FinancePageKind.account => account != null,
    FinancePageKind.accounts => accounts != null,
    FinancePageKind.transit => transit != null,
  };
  List<FinancePageActivity> get items =>
      category?.items ?? account?.items ?? transit?.items ?? const [];
  bool get hasMore =>
      category?.hasMore ?? account?.hasMore ?? transit?.hasMore ?? false;
  int get totalCount =>
      category?.totalCount ?? account?.totalCount ?? transit?.totalCount ?? 0;

  List<int> _revision() => switch (kind) {
    FinancePageKind.accounts => [changes?.accountsRevision ?? 0],
    FinancePageKind.account => [
      changes?.accountsRevision ?? 0,
      changes?.cashRevision ?? 0,
    ],
    FinancePageKind.transit => [
      changes?.cashRevision ?? 0,
      changes?.accountsRevision ?? 0,
    ],
    _ => [changes?.dashboardRevision ?? 0, changes?.budgetsRevision ?? 0],
  };
  void _changed() {
    final next = _revision();
    if (listEquals(next, _revisions)) return;
    _revisions = next;
    load();
  }

  void _scopeChanged() {
    if (scope == scopeController?.scope) {
      notifyListeners();
      return;
    }
    scope = scopeController?.scope;
    load(clear: true);
  }

  void _clear() {
    isStale = false;
    summary = null;
    category = null;
    account = null;
    accounts = null;
    transit = null;
  }

  Future<void> load({bool clear = false}) async {
    final generation = ++_generation;
    if (clear) _clear();
    isLoading = true;
    isLoadingMore = false;
    errorMessage = null;
    notifyListeners();
    try {
      switch (kind) {
        case FinancePageKind.summary:
          final result = await repository.summary(
            month,
            income: income,
            scope: scope,
            source: source,
          );
          if (!_current(generation)) return;
          summary = result;
        case FinancePageKind.category:
          final result = await repository.category(
            id,
            month,
            income: income,
            scope: scope,
            source: source,
            person: person,
          );
          if (!_current(generation)) return;
          category = result;
        case FinancePageKind.account:
          final result = await repository.account(
            id!,
            month,
            direction: direction,
            selection: selection,
          );
          if (!_current(generation)) return;
          account = result;
        case FinancePageKind.accounts:
          final result = await repository.accounts();
          if (!_current(generation)) return;
          accounts = result;
        case FinancePageKind.transit:
          final result = await repository.transit(id!);
          if (!_current(generation)) return;
          transit = result;
      }
      unauthorized = false;
      isStale = false;
    } on ApiException catch (error) {
      if (!_current(generation)) return;
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      if (unauthorized) _clear();
      isStale = hasData;
    } on FormatException {
      if (!_current(generation)) return;
      errorMessage = 'Bilgiler beklenen biçimde alınamadı.';
      isStale = hasData;
    } finally {
      if (_current(generation)) {
        isLoading = false;
        notifyListeners();
      }
    }
  }

  bool _current(int generation) => !_disposed && generation == _generation;
  Future<void> chooseMonth(DateTime value) async {
    month = DateTime(value.year, value.month);
    await load(clear: true);
  }

  Future<void> chooseIncome(bool value) async {
    income = value;
    await load(clear: true);
  }

  Future<void> chooseSource(String? value) async {
    source = value;
    await load(clear: true);
  }

  Future<void> choosePerson(String? value) async {
    person = value;
    await load(clear: true);
  }

  Future<void> chooseDirection(String? value) async {
    direction = value;
    selection = null;
    await load(clear: true);
  }

  Future<void> chooseSelection(String? value) async {
    selection = value;
    await load(clear: true);
  }

  Future<void> clearFilters() async {
    source = null;
    person = null;
    selection = null;
    await load(clear: true);
  }

  Future<void> loadMore() async {
    if (isLoading || isLoadingMore || !hasMore) return;
    final generation = _generation;
    final offset =
        (category?.offset ?? account?.offset ?? transit?.offset ?? 0) + 50;
    isLoadingMore = true;
    errorMessage = null;
    notifyListeners();
    List<FinancePageActivity> merge(List<FinancePageActivity> next) {
      final seen = items.map((i) => i.listKey).toSet();
      return [...items, ...next.where((i) => seen.add(i.listKey))];
    }

    try {
      switch (kind) {
        case FinancePageKind.category:
          final next = await repository.category(
            id,
            month,
            income: income,
            scope: scope,
            source: source,
            person: person,
            offset: offset,
          );
          if (!_current(generation)) return;
          category = next.withItems(merge(next.items));
        case FinancePageKind.account:
          final next = await repository.account(
            id!,
            month,
            direction: direction,
            selection: selection,
            offset: offset,
          );
          if (!_current(generation)) return;
          account = next.withItems(merge(next.items));
        case FinancePageKind.transit:
          final next = await repository.transit(id!, offset: offset);
          if (!_current(generation)) return;
          transit = next.withItems(merge(next.items));
        case FinancePageKind.summary:
        case FinancePageKind.accounts:
          return;
      }
    } on ApiException catch (error) {
      if (!_current(generation)) return;
      errorMessage = error.message;
      unauthorized = error.isUnauthorized;
      if (unauthorized) {
        _clear();
      } else {
        isStale = true;
      }
    } on FormatException {
      if (!_current(generation)) return;
      errorMessage = 'Bilgiler beklenen biçimde alınamadı.';
    } finally {
      if (_current(generation)) {
        isLoadingMore = false;
        notifyListeners();
      }
    }
  }

  Future<FinanceYearMonths> months(int year) => kind == FinancePageKind.account
      ? repository.accountMonths(id!, year)
      : repository.categoryMonths(
          year,
          id: id,
          summary: kind == FinancePageKind.summary,
          income: income,
          scope: scope,
          source: source,
          person: person,
        );
  @override
  void dispose() {
    _disposed = true;
    _generation++;
    changes?.removeListener(_changed);
    scopeController?.removeListener(_scopeChanged);
    super.dispose();
  }
}
