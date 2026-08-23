import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_feed_page.dart';
import 'package:business_finance_mobile/features/activities/presentation/planned_activity_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/planned_activity_page.dart';

void main() {
  group('sözleşme', () {
    test('kapsam feed sorgusuna iner', () async {
      final requests = <http.Request>[];
      final repository = _httpRepository(requests);

      await repository.list(scope: TransactionScope.business);

      expect(requests.single.url.queryParameters['scope'], 'business');
    });

    test('kapsam boşsa filtre gönderilmez', () async {
      final requests = <http.Request>[];
      final repository = _httpRepository(requests);

      await repository.list();

      expect(requests.single.url.queryParameters.containsKey('scope'), isFalse);
    });

    test('planlanan görünüm de aynı filtreyi taşır', () async {
      final requests = <http.Request>[];
      final repository = _httpRepository(requests, planned: true);

      await repository.listPlanned(
        horizon: PlannedHorizon.week,
        today: DateTime(2026, 8, 9),
        scope: TransactionScope.personal,
      );

      expect(requests.single.url.queryParameters['scope'], 'personal');
    });
  });

  group('hareket listesi', () {
    test('aktif kapsam her sayfada uygulanır', () async {
      final repository = _FakeRepository(hasNextPage: true);
      final controller = ActivityController(
        repository,
        scopeController: await _scopeController(
          hasBusiness: true,
          selected: TransactionScope.business,
        ),
      );

      await controller.load();
      await controller.loadMore();

      expect(repository.scopes, [
        TransactionScope.business,
        TransactionScope.business,
      ]);
    });

    test('anahtar konum değiştirince liste baştan okunur', () async {
      final repository = _FakeRepository();
      final scope = await _scopeController(hasBusiness: true);
      final controller = ActivityController(repository, scopeController: scope);
      await controller.load();

      await scope.select(TransactionScope.personal);
      await Future<void>.delayed(Duration.zero);

      expect(repository.scopes, [null, TransactionScope.personal]);
      expect(repository.pages, [1, 1]);
    });

    test('işletmesi olmayan kullanıcıda filtre gönderilmez', () async {
      final repository = _FakeRepository();
      final controller = ActivityController(
        repository,
        scopeController: await _scopeController(
          hasBusiness: false,
          selected: TransactionScope.personal,
        ),
      );

      await controller.load();

      expect(repository.scopes, [null]);
      expect(controller.isScopeVisible, isFalse);
    });

    test('yetkisiz cevap kapsamla birlikte de görünür ele alınır', () async {
      final repository = _FakeRepository(
        error: const ApiException(
          code: 'authentication.required',
          message: 'Oturum gerekli.',
          statusCode: 401,
        ),
      );
      final controller = ActivityController(
        repository,
        scopeController: await _scopeController(
          hasBusiness: true,
          selected: TransactionScope.business,
        ),
      );

      await controller.load();

      expect(controller.unauthorized, isTrue);
      expect(controller.errorMessage, 'Oturum gerekli.');
      expect(controller.items, isEmpty);
    });
  });

  group('planlanan görünüm', () {
    test('aktif kapsam sorguya iner ve değişince yeniden okunur', () async {
      final repository = _FakeRepository();
      final scope = await _scopeController(
        hasBusiness: true,
        selected: TransactionScope.business,
      );
      final controller = PlannedActivityController(
        repository,
        scopeController: scope,
      );
      await controller.load();

      await scope.select(null);
      await Future<void>.delayed(Duration.zero);

      expect(repository.plannedScopes, [TransactionScope.business, null]);
    });
  });

  group('başlık', () {
    testWidgets('bölünen liste aktif kapsamı başlığında yazar', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: ActivityFeedPage(
            repository: _FakeRepository(),
            scopeController: await _scopeController(
              hasBusiness: true,
              selected: TransactionScope.business,
            ),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('İşlemler · İşletme'), findsOneWidget);
    });

    testWidgets('filtre yokken başlık sade kalır', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: ActivityFeedPage(
            repository: _FakeRepository(),
            scopeController: await _scopeController(hasBusiness: true),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('İşlemler'), findsOneWidget);
    });

    testWidgets('planlanan görünüm de kapsamı yazar', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: PlannedActivityPageView(
            repository: _FakeRepository(),
            scopeController: await _scopeController(
              hasBusiness: true,
              selected: TransactionScope.personal,
            ),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('Planlananlar · Şahsi'), findsOneWidget);
    });
  });
}

Future<ScopeController> _scopeController({
  required bool hasBusiness,
  TransactionScope? selected,
}) async {
  final controller = ScopeController(
    store: _MemoryStore(scope: selected, hasBusiness: hasBusiness),
  );
  await controller.ensureLoaded();
  return controller;
}

class _MemoryStore implements ScopeStore {
  _MemoryStore({this.scope, this.hasBusiness});

  TransactionScope? scope;
  bool? hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => scope;

  @override
  Future<void> writeScope(TransactionScope? value) async => scope = value;

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async => hasBusiness = value;

  @override
  Future<void> clear() async {
    scope = null;
    hasBusiness = null;
  }
}

ActivityRepository _httpRepository(
  List<http.Request> requests, {
  bool planned = false,
}) {
  final client = MockClient((request) async {
    requests.add(request);
    return http.Response(
      jsonEncode(
        planned
            ? <String, dynamic>{
                'asOfDate': '2026-08-09',
                'daysAhead': 7,
                'totalCount': 0,
                'items': <dynamic>[],
              }
            : <String, dynamic>{
                'items': <dynamic>[],
                'pagination': {
                  'pageNumber': 1,
                  'pageSize': 20,
                  'totalCount': 0,
                  'totalPages': 0,
                  'hasPreviousPage': false,
                  'hasNextPage': false,
                },
              },
      ),
      200,
      headers: {'content-type': 'application/json'},
    );
  });
  return ActivityRepository(
    ApiClient(
      config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
      httpClient: client,
    ),
  );
}

class _FakeRepository implements ActivityRepositoryContract {
  _FakeRepository({this.hasNextPage = false, this.error});

  final bool hasNextPage;
  final ApiException? error;
  final scopes = <TransactionScope?>[];
  final plannedScopes = <TransactionScope?>[];
  final pages = <int>[];

  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) async {
    if (error case final failure?) throw failure;
    scopes.add(scope);
    pages.add(pageNumber);
    return ActivityPage(
      items: const [],
      pagination: ActivityPagination(
        pageNumber: pageNumber,
        pageSize: pageSize,
        totalCount: 0,
        totalPages: hasNextPage ? 2 : 0,
        hasPreviousPage: false,
        hasNextPage: hasNextPage && pageNumber == 1,
      ),
    );
  }

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async {
    plannedScopes.add(scope);
    return PlannedActivityPage(
      asOfDate: '2026-08-09',
      daysAhead: horizon.days,
      totalCount: 0,
      items: const [],
    );
  }

  @override
  Future<void> cancel(FinancialActivity activity) async {}

  @override
  Future<void> realizePlanned(PlannedActivity activity) async {}
}
