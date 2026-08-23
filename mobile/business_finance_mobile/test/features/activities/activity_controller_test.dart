import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_controller.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';

void main() {
  test('load fills the list and clears the stale marker', () async {
    final repository = _FakeRepository(
      pages: [
        _page(['a', 'b']),
      ],
    );
    final controller = ActivityController(repository);

    await controller.load();

    expect(controller.items, hasLength(2));
    expect(controller.isLoading, isFalse);
    expect(controller.isStale, isFalse);
    expect(controller.errorMessage, isNull);
  });

  test(
    'an unauthorized response is surfaced rather than shown as empty',
    () async {
      final repository = _FakeRepository(
        error: const ApiException(
          code: 'authentication.unauthorized',
          message: 'Oturum sona erdi',
          statusCode: 401,
        ),
      );
      final controller = ActivityController(repository);

      await controller.load();

      expect(controller.unauthorized, isTrue);
      expect(controller.errorMessage, 'Oturum sona erdi');
      expect(controller.items, isEmpty);
    },
  );

  test(
    'a malformed payload becomes a visible error, not a silent empty list',
    () async {
      final repository = _FakeRepository(error: const FormatException('bozuk'));
      final controller = ActivityController(repository);

      await controller.load();

      expect(controller.errorMessage, isNotNull);
      expect(controller.unauthorized, isFalse);
    },
  );

  test(
    'empty is only reported once loading finished without an error',
    () async {
      final repository = _FakeRepository(pages: [_page([])]);
      final controller = ActivityController(repository);
      expect(controller.isEmpty, isTrue);

      await controller.load();

      expect(controller.isEmpty, isTrue);
      expect(controller.items, isEmpty);
    },
  );

  test('loadMore appends the next page and asks for it once', () async {
    final repository = _FakeRepository(
      pages: [
        _page(['a', 'b'], pageNumber: 1, totalCount: 4, hasNext: true),
        _page(['c', 'd'], pageNumber: 2, totalCount: 4),
      ],
    );
    final controller = ActivityController(repository);

    await controller.load();
    await controller.loadMore();

    expect(controller.items.map((item) => item.activityId), [
      'a',
      'b',
      'c',
      'd',
    ]);
    expect(controller.hasMore, isFalse);
    expect(repository.requestedPages, [1, 2]);
  });

  /// A movement created while the user reads shifts every later page down, so the
  /// server can hand back a row that is already on screen. Without the guard it
  /// would be drawn twice and its duplicate key would break the list.
  test('loadMore drops rows already on screen after the pages shift', () async {
    final repository = _FakeRepository(
      pages: [
        _page(['a', 'b'], pageNumber: 1, totalCount: 4, hasNext: true),
        _page(['b', 'c'], pageNumber: 2, totalCount: 4),
      ],
    );
    final controller = ActivityController(repository);

    await controller.load();
    await controller.loadMore();

    expect(controller.items.map((item) => item.activityId), ['a', 'b', 'c']);
    expect(
      controller.items.map((item) => item.listKey).toSet(),
      hasLength(controller.items.length),
    );
  });

  test(
    'loadMore does nothing when the server says there is no next page',
    () async {
      final repository = _FakeRepository(
        pages: [
          _page(['a']),
        ],
      );
      final controller = ActivityController(repository);

      await controller.load();
      await controller.loadMore();

      expect(repository.requestedPages, [1]);
    },
  );

  test('changing a quick filter reloads from the first page', () async {
    final repository = _FakeRepository(
      pages: [
        _page(['a'], hasNext: true),
        _page(['b']),
      ],
    );
    final controller = ActivityController(repository);

    await controller.load();
    await controller.selectQuickFilter(ActivityQuickFilter.creditCards);

    expect(controller.filter.quickFilter, ActivityQuickFilter.creditCards);
    expect(repository.requestedPages, [1, 1]);
    expect(
      repository.requestedFilters.last.quickFilter,
      ActivityQuickFilter.creditCards,
    );
  });

  test('clearing advanced filters keeps the chosen quick filter', () async {
    final repository = _FakeRepository(pages: [_page([]), _page([])]);
    final controller = ActivityController(repository);

    await controller.applyFilter(
      const ActivityFilter(
        quickFilter: ActivityQuickFilter.accounts,
        dateRange: ActivityDateRange.thisMonth,
        includeCancelled: false,
      ),
    );
    await controller.clearAdvancedFilters();

    expect(controller.filter.quickFilter, ActivityQuickFilter.accounts);
    expect(controller.filter.hasAdvancedFilters, isFalse);
  });

  test('a financial change elsewhere reloads the feed on its own', () async {
    final changes = FinancialDataChanges();
    final repository = _FakeRepository(
      pages: [
        _page(['a']),
        _page(['a', 'b']),
      ],
    );
    final controller = ActivityController(
      repository,
      financialDataChanges: changes,
    );
    await controller.load();

    changes.transactionsChanged();
    await Future<void>.delayed(Duration.zero);

    expect(repository.requestedPages, [1, 1]);
    expect(controller.items, hasLength(2));
  });

  test(
    'cancel refreshes the feed and tells the other screens to reread',
    () async {
      final changes = FinancialDataChanges();
      final repository = _FakeRepository(
        pages: [
          _page(['a']),
          _page([]),
        ],
      );
      final controller = ActivityController(
        repository,
        financialDataChanges: changes,
      );
      await controller.load();
      final before = changes.dashboardRevision;

      final ok = await controller.cancel(controller.items.first);

      expect(ok, isTrue);
      expect(repository.cancelled, hasLength(1));
      // Reloaded after cancelling, so the row does not linger as if it survived.
      expect(repository.requestedPages, [1, 1]);
      expect(controller.successMessage, isNotNull);
      // Balances and budgets change too, so the shared signal has to move.
      expect(changes.dashboardRevision, greaterThan(before));
    },
  );

  test('cancel is refused for a movement the server locked', () async {
    final repository = _FakeRepository(
      pages: [
        _page(['a'], canCancel: false),
      ],
    );
    final controller = ActivityController(repository);
    await controller.load();

    final ok = await controller.cancel(controller.items.first);

    expect(ok, isFalse);
    expect(repository.cancelled, isEmpty);
  });

  test(
    'a failed cancel surfaces the reason and leaves the row in place',
    () async {
      final repository =
          _FakeRepository(
              pages: [
                _page(['a']),
              ],
            )
            ..cancelError = const ApiException(
              code: 'transactions.cancel_origin_locked',
              message: 'Tekrarlayan plandan üretilmiş hareket iptal edilemez.',
              statusCode: 409,
            );
      final controller = ActivityController(repository);
      await controller.load();

      final ok = await controller.cancel(controller.items.first);

      expect(ok, isFalse);
      expect(
        controller.errorMessage,
        'Tekrarlayan plandan üretilmiş hareket iptal edilemez.',
      );
      expect(controller.items, hasLength(1));
      expect(controller.isCancelling, isFalse);
    },
  );
}

ActivityPage _page(
  List<String> ids, {
  int pageNumber = 1,
  int? totalCount,
  bool hasNext = false,
  bool canCancel = true,
}) => ActivityPage(
  items: [for (final id in ids) _activity(id, canCancel: canCancel)],
  pagination: ActivityPagination(
    pageNumber: pageNumber,
    pageSize: 2,
    totalCount: totalCount ?? ids.length,
    totalPages: hasNext ? pageNumber + 1 : pageNumber,
    hasPreviousPage: pageNumber > 1,
    hasNextPage: hasNext,
  ),
);

FinancialActivity _activity(String id, {bool canCancel = true}) =>
    FinancialActivity(
      activityId: id,
      kind: ActivityKind.accountTransaction,
      effect: ActivityEffect.expense,
      sourceGroup: ActivitySourceGroup.account,
      origin: ActivityOrigin.manual,
      status: ActivityStatus.realized,
      activityDate: '2026-08-14',
      amount: '10.0000',
      currency: 'TRY',
      title: 'Market',
      canCancel: canCancel,
      supportsAttachments: true,
    );

class _FakeRepository implements ActivityRepositoryContract {
  _FakeRepository({this.pages = const [], this.error});

  final List<ActivityPage> pages;
  final Object? error;
  final List<int> requestedPages = [];
  final List<ActivityFilter> requestedFilters = [];
  int _call = 0;

  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) async {
    requestedPages.add(pageNumber);
    requestedFilters.add(filter);
    if (error != null) throw error!;
    final page = pages[_call.clamp(0, pages.length - 1)];
    _call++;
    return page;
  }

  final List<FinancialActivity> cancelled = [];
  Object? cancelError;

  @override
  Future<void> cancel(FinancialActivity activity) async {
    cancelled.add(activity);
    if (cancelError != null) throw cancelError!;
  }

  PlannedActivityPage? plannedPage;

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async =>
      plannedPage ??
      const PlannedActivityPage(
        asOfDate: '2026-08-14',
        daysAhead: 30,
        totalCount: 0,
        items: [],
      );

  @override
  Future<void> realizePlanned(PlannedActivity activity) =>
      throw UnimplementedError();
}
