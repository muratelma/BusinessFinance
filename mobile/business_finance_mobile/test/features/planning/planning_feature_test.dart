import 'dart:convert';

import 'package:flutter/material.dart';
import '../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_list_row.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:business_finance_mobile/core/widgets/app_row_action.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'package:business_finance_mobile/features/planning/data/planning_repository.dart';
import 'package:business_finance_mobile/features/planning/presentation/planning_controller.dart';
import 'package:business_finance_mobile/features/planning/presentation/planning_page.dart';

void main() {
  testWidgets('planlama ekranı erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(repository: _FakePlanningRepository()),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  test('repository preserves money strings and sends report filters', () async {
    final requestedPaths = <String>[];
    final repository = PlanningRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          requestedPaths.add(request.url.toString());
          return http.Response(
            jsonEncode(_responseFor(request.url.path)),
            200,
            headers: {'content-type': 'application/json'},
          );
        }),
      ),
    );

    final snapshot = await repository.load(
      year: 2026,
      month: 8,
      asOfDate: '2026-08-11',
      daysAhead: 30,
    );

    expect(snapshot.recurringTransactions, hasLength(2));

    // A plan written before credit-card sources existed carries no sourceType
    // and is account funded by definition.
    final legacy = snapshot.recurringTransactions.first;
    expect(legacy.amount, '125.5000');
    expect(legacy.sourceType, 'account');
    expect(legacy.accountId, 'account');
    expect(legacy.creditCardId, isNull);

    // A card-sourced plan sends a null accountId. Reading it as a required
    // string broke the whole planning and reports screen, because both are fed
    // by this one load.
    final cardPlan = snapshot.recurringTransactions.last;
    expect(cardPlan.sourceType, 'credit-card');
    expect(cardPlan.accountId, isNull);
    expect(cardPlan.creditCardId, 'card');

    // The occurrence carries its category, so every screen can name the item the
    // same way instead of one saying "Düzenli gider" and another "Fatura".
    expect(snapshot.occurrences.single.categoryId, 'category');
    expect(snapshot.occurrences.single.canRealize, isTrue);

    expect(snapshot.report.netWorth, '4800.0000');
    // Only active cards are offered as a recurring source: an inactive one
    // cannot take a charge, so it would only fail at realization time.
    expect(snapshot.creditCards, hasLength(1));
    expect(snapshot.creditCards.single.name, 'Test Kart');
    expect(
      requestedPaths,
      contains(
        'https://api.test/api/v1/reports/advanced?year=2026&month=8&asOfDate=2026-08-11&trendMonths=6&daysAhead=30',
      ),
    );
  });

  test(
    'recurring writes preserve money and use PATCH for active state',
    () async {
      late http.Request captured;
      final repository = PlanningRepository(
        ApiClient(
          config: ApiConfig.fromEnvironment(value: 'https://api.test'),
          httpClient: MockClient((request) async {
            captured = request;
            return http.Response('{}', 201);
          }),
        ),
      );

      await repository.createRecurring({
        'accountId': 'account',
        'categoryId': 'category',
        'amount': '125.5000',
        'currency': 'TRY',
      });

      final body = jsonDecode(captured.body) as Map<String, dynamic>;
      expect(captured.url.path, '/api/v1/recurring-transactions');
      expect(body['amount'], '125.5000');
      expect(body['amount'], isA<String>());

      await repository.setRecurringActive('recurring-id', false);
      expect(captured.method, 'PATCH');
      expect(
        captured.url.path,
        '/api/v1/recurring-transactions/recurring-id/active',
      );
      expect(jsonDecode(captured.body), {'isActive': false});
    },
  );

  test('controller distinguishes unauthorized and stale cached data', () async {
    final unauthorized = PlanningController(
      _FakePlanningRepository(
        error: const ApiException(
          statusCode: 401,
          code: 'authentication.required',
          message: 'Oturum gerekli.',
        ),
      ),
      now: () => DateTime(2026, 8, 11),
    );
    await unauthorized.load();
    expect(unauthorized.unauthorized, isTrue);

    final repository = _FakePlanningRepository();
    final cached = PlanningController(
      repository,
      now: () => DateTime(2026, 8, 11),
    );
    await cached.load();
    repository.error = const ApiException(
      statusCode: 503,
      code: 'network.unavailable',
      message: 'Bağlantı kurulamadı.',
    );
    await cached.load();
    expect(cached.snapshot, isNotNull);
    expect(cached.isStale, isTrue);
  });

  test(
    'realizing an occurrence invalidates transactions, dashboard and budgets',
    () async {
      final changes = FinancialDataChanges();
      final controller = PlanningController(
        _FakePlanningRepository(),
        financialDataChanges: changes,
        now: () => DateTime(2026, 8, 13),
      );

      final realized = await controller.realizeOccurrence('occurrence');

      expect(realized, isTrue);
      expect(changes.dashboardRevision, 1);
      expect(changes.budgetsRevision, 1);
    },
  );

  /// A plan that never produced money can be removed, so the list does not turn
  /// into a growing pile of disabled rows the user cannot clear.
  testWidgets('a plan can be deleted after confirming', (tester) async {
    final repository = _FakePlanningRepository(
      plans: [_plan(id: 'p1', description: 'Netflix')],
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();

    // Silme satırda değil: nadir ve geri alınamaz bir iş, her satırda
    // bağırmamalı. Satıra dokununca açılan panelde.
    expect(find.text('Sil'), findsNothing);
    await tester.tap(find.text('Netflix'));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Sil'));
    await tester.pumpAndSettle();
    expect(find.text('Plan silinsin mi?'), findsOneWidget);

    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();
    expect(repository.deletedPlans, isEmpty);

    await tester.tap(find.text('Netflix'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Sil'));
    await tester.pumpAndSettle();
    await tester.tap(find.widgetWithText(FilledButton, 'Sil'));
    await tester.pumpAndSettle();

    expect(repository.deletedPlans, ['p1']);
  });

  /// Onay bekleyen kayıt iki yerde değil, bir yerde.
  ///
  /// Yaklaşanlar listesi bu kayıtları **zaten** taşıyordu (`sourceType`
  /// `recurring-occurrence`); Tekrarlayanlar sekmesinin altındaki ayrı
  /// "Planlanan kayıtlar" bölümü aynı satırları ikinci kez gösteriyordu.
  /// Eksik olan tek şey onay eylemiydi ve o da artık satırın üzerinde.
  testWidgets('yaklaşan kayıt kendi satırından onaylanıyor', (tester) async {
    final repository = _FakePlanningRepository(
      occurrences: const [
        RecurringOccurrenceItem(
          id: 'occ-1',
          recurringTransactionId: 'plan-1',
          categoryId: 'category',
          amount: '125.5000',
          currency: 'TRY',
          kind: 'bill-payment',
          scheduledDate: '2026-08-15',
          description: 'İnternet',
          status: 'planned',
        ),
      ],
      upcomingPayments: const [
        UpcomingPaymentItem(
          sourceId: 'occ-1',
          sourceType: 'recurring-occurrence',
          title: 'İnternet',
          amount: '125.5000',
          currency: 'TRY',
          dueDate: '2026-08-15',
          timing: 'today',
          description: null,
        ),
        // Henüz occurrence üretilmemiş satır: kimlik planın kimliği.
        UpcomingPaymentItem(
          sourceId: 'plan-9',
          sourceType: 'recurring-occurrence',
          title: 'Kira',
          amount: '5000.0000',
          currency: 'TRY',
          dueDate: '2026-08-14',
          timing: 'overdue',
          description: null,
        ),
        // Vakti gelmemiş: onay çıkmaz.
        UpcomingPaymentItem(
          sourceId: 'plan-8',
          sourceType: 'recurring-occurrence',
          title: 'Spor salonu',
          amount: '400.0000',
          currency: 'TRY',
          dueDate: '2026-09-15',
          timing: 'upcoming',
          description: null,
        ),
        UpcomingPaymentItem(
          sourceId: 'stmt-1',
          sourceType: 'credit-card-statement',
          title: 'Kart ekstresi',
          amount: '350.0000',
          currency: 'TRY',
          dueDate: '2026-08-20',
          timing: 'today',
          description: null,
        ),
      ],
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();

    // Tekrarlayanlar sekmesinde artık onay listesi yok.
    expect(find.text('Planlanan kayıtlar'), findsNothing);

    await tester.tap(find.text('Yaklaşanlar'));
    await tester.pumpAndSettle();

    // Üretme düğmesi hiçbir yerde yok: gerçekleştirmenin kendisi eksik kaydı
    // sunucuda üretiyor ve geriye kullanıcının vermek zorunda olmadığı bir
    // karar kalıyordu.
    expect(find.text('Bugüne kadar üret'), findsNothing);
    // İki satırda onay var, üçünde yok: kart ekstresi onaylanacak bir şey
    // değil (vadesi gelen bir yükümlülüktür) ve vakti gelmemiş satır henüz bir
    // tahmindir.
    expect(find.widgetWithText(AppRowAction, 'Gerçekleştir'), findsNWidgets(2));

    // Üretilmiş kayıt kendi kimliğiyle gerçekleşiyor.
    await _tapActionOf(tester, 'İnternet');
    expect(repository.realizedOccurrences, ['occ-1']);
    expect(repository.realizedDue, isEmpty);
  });

  /// Henüz üretilmemiş kayıt planı ve tarihiyle gerçekleşiyor.
  ///
  /// Yaklaşanlar listesi kimlik olarak, occurrence varsa onun kimliğini, yoksa
  /// **planın** kimliğini taşıyor. Kullanıcı için ikisi de tek bir "Onayla";
  /// üretmek sistemin defter işi.
  testWidgets('üretilmemiş kayıt planı ve tarihiyle onaylanıyor', (
    tester,
  ) async {
    final repository = _FakePlanningRepository(
      upcomingPayments: const [
        UpcomingPaymentItem(
          sourceId: 'plan-9',
          sourceType: 'recurring-occurrence',
          title: 'Kira',
          amount: '5000.0000',
          currency: 'TRY',
          dueDate: '2026-08-14',
          timing: 'overdue',
          description: null,
        ),
      ],
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Yaklaşanlar'));
    await tester.pumpAndSettle();

    await _tapActionOf(tester, 'Kira');

    expect(repository.realizedDue, ['plan-9@2026-08-14']);
    expect(repository.realizedOccurrences, isEmpty);
  });

  /// Plan satırında hiç denetim yok.
  ///
  /// İki deneme de cihazda yanlış çıktı: önce üç satırlık bir `SwitchListTile`
  /// ve altında tam genişlikte bir `Sil` şeridi, sonra rozet alanına taşınan
  /// bir silme butonu. Duraklatma ve silme artık satıra dokununca açılan
  /// panelde; liste tek işi yapıyor ve tutar sağ bloğuna döndü.
  testWidgets('plan satırı denetim taşımıyor, panel açıyor', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(
          repository: _FakePlanningRepository(
            plans: [_plan(id: 'p1', description: 'Netflix')],
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.byType(SwitchListTile), findsNothing);
    expect(find.byType(Switch), findsNothing);
    expect(find.text('Sil'), findsNothing);
    expect(find.text('Duraklat'), findsNothing);
    // Tutar satırın sağ bloğunda.
    expect(find.byType(AppMoneyText), findsWidgets);

    await tester.tap(find.text('Netflix'));
    await tester.pumpAndSettle();

    expect(find.text('Duraklat'), findsOneWidget);
    expect(find.text('Sil'), findsOneWidget);
  });

  /// Paused plans stay available but folded away, so a dozen of them cannot bury
  /// the plans that still run.
  testWidgets('paused plans are collapsed behind a counted section', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(
          repository: _FakePlanningRepository(
            plans: [
              _plan(id: 'p1', description: 'Netflix'),
              _plan(id: 'p2', description: 'Eski spor salonu', isActive: false),
              _plan(id: 'p3', description: 'Eski dergi', isActive: false),
            ],
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Netflix'), findsOneWidget);
    expect(find.text('Duraklatılmış planlar (2)'), findsOneWidget);
    // Folded away until asked for.
    expect(find.text('Eski spor salonu'), findsNothing);

    await tester.tap(find.text('Duraklatılmış planlar (2)'));
    await tester.pumpAndSettle();

    expect(find.text('Eski spor salonu'), findsOneWidget);
    expect(find.text('Eski dergi'), findsOneWidget);
  });

  testWidgets('reports expose chart semantics and a visible text alternative', (
    tester,
  ) async {
    final semantics = tester.ensureSemantics();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(repository: _FakePlanningRepository()),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Tekrarlayanlar'), findsOneWidget);
    expect(find.text('Henüz tekrarlayan plan yok'), findsOneWidget);
    await tester.tap(find.text('Raporlar'));
    await tester.pumpAndSettle();

    expect(find.byKey(const Key('cash-flow-chart')), findsOneWidget);
    expect(
      tester.getSemantics(find.byKey(const Key('cash-flow-chart'))).label,
      contains('Nakit akışı grafiği. 7.2026 net ₺1.400,00'),
    );
    expect(find.byKey(const Key('cash-flow-chart-labels')), findsOneWidget);
    expect(
      find.descendant(
        of: find.byKey(const Key('cash-flow-chart-labels')),
        matching: find.text('07.2026'),
      ),
      findsOneWidget,
    );
    await tester.drag(find.byType(ListView).last, const Offset(0, -400));
    await tester.pumpAndSettle();
    expect(find.byKey(const Key('cash-flow-text-alternative')), findsOneWidget);
    expect(find.text('Gelir ₺2.000,00 • gider ₺600,00'), findsOneWidget);
    semantics.dispose();
  });

  /// Hesap bakiyesi ile kart borcu aynı tonda yazılıyordu.
  ///
  /// İkisi zıt şeyler: biri elinizdeki para, diğeri yükümlülük. Aynı renkte
  /// alt alta duran iki liste, "5.150 var" ile "350 borçluyum" arasındaki farkı
  /// tek bakışta göstermiyordu.
  testWidgets('kart borçları gider tonunda, hesap bakiyeleri nötr', (
    tester,
  ) async {
    // Dağılım bölümleri raporlar sekmesinin en altında; ekran büyütülüyor ki
    // testin ölçtüğü şey kaydırma değil ton olsun.
    tester.view.physicalSize = const Size(1200, 3200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: PlanningPage(repository: _FakePlanningRepository()),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Raporlar'));
    await tester.pumpAndSettle();

    final sections = tester
        .widgetList<AppMoneyText>(find.byType(AppMoneyText))
        .where(
          (money) => money.amount == '350.0000' || money.amount == '5150.0000',
        )
        .toList(growable: false);

    final debt = sections.firstWhere((money) => money.amount == '350.0000');
    final balance = sections.firstWhere((money) => money.amount == '5150.0000');
    expect(debt.effect, AppMoneyEffect.expense);
    // Bakiye rol taşımıyor: `neutral` tonu maviyi getiriyor (ADR 0008) ve
    // bakiye listesi renkli bir role sahipmiş gibi okunuyordu.
    expect(balance.effect, isNull);
  });
}

Object _responseFor(String path) => switch (path) {
  '/api/v1/recurring-transactions' => {
    'items': [
      {
        'id': 'recurring',
        'accountId': 'account',
        'categoryId': 'category',
        'amount': '125.5000',
        'currency': 'TRY',
        'kind': 'bill-payment',
        'frequency': 'monthly',
        'startDate': '2026-08-15',
        'endDate': null,
        'nextOccurrenceDate': '2026-08-15',
        'monthEndBehavior': 'clamp-to-last-day',
        'description': 'İnternet',
        'isActive': true,
      },
      // Card sourced: accountId is null and the card carries the source. Written
      // by the server since credit-card recurring plans landed.
      {
        'id': 'recurring-card',
        'sourceType': 'credit-card',
        'accountId': null,
        'creditCardId': 'card',
        'categoryId': 'category',
        'amount': '149.9000',
        'currency': 'TRY',
        'kind': 'bill-payment',
        'frequency': 'monthly',
        'startDate': '2026-08-15',
        'endDate': null,
        'nextOccurrenceDate': '2026-09-15',
        'monthEndBehavior': 'clamp-to-last-day',
        'description': 'Streaming',
        'isActive': true,
      },
    ],
  },
  '/api/v1/recurring-transactions/occurrences' => {
    'items': [
      {
        'id': 'occurrence',
        'recurringTransactionId': 'recurring',
        'occurrenceKey': 'k',
        'sourceType': 'account',
        'accountId': 'account',
        'creditCardId': null,
        'categoryId': 'category',
        'amount': '125.5000',
        'currency': 'TRY',
        'kind': 'bill-payment',
        'scheduledDate': '2026-08-15',
        'description': null,
        'status': 'planned',
        'budgetTransactionId': null,
        'creditCardChargeId': null,
        'realizedAtUtc': null,
      },
    ],
  },
  '/api/v1/upcoming-payments' => {'items': <Object>[]},
  '/api/v1/reports/advanced' => _reportJson,
  '/api/v1/accounts' => {
    'items': [
      {'id': 'account', 'name': 'Ana hesap'},
    ],
  },
  '/api/v1/categories' => {
    'items': [
      {'id': 'category', 'name': 'Fatura', 'type': 'expense'},
    ],
  },
  '/api/v1/credit-cards' => {
    'items': [
      {'id': 'card', 'name': 'Test Kart', 'isActive': true},
      // Inactive cards are dropped: they cannot take a charge.
      {'id': 'closed-card', 'name': 'Kapalı Kart', 'isActive': false},
    ],
  },
  _ => throw StateError('Unexpected path: $path'),
};

const _reportJson = {
  'asOfDate': '2026-08-11',
  'currency': 'TRY',
  'netWorth': {
    'liquidAssets': '5150.0000',
    'creditCardDebt': '350.0000',
    'receivableDebt': '0.0000',
    'payableDebt': '0.0000',
    'netWorth': '4800.0000',
  },
  'periodComparison': {
    'current': {
      'year': 2026,
      'month': 8,
      'income': '3000.0000',
      'expense': '1100.0000',
      'net': '1900.0000',
    },
    'previous': {
      'year': 2026,
      'month': 7,
      'income': '2000.0000',
      'expense': '600.0000',
      'net': '1400.0000',
    },
    'incomeChange': '1000.0000',
    'expenseChange': '500.0000',
    'netChange': '500.0000',
  },
  'cashFlowTrend': [
    {
      'year': 2026,
      'month': 7,
      'income': '2000.0000',
      'expense': '600.0000',
      'net': '1400.0000',
    },
    {
      'year': 2026,
      'month': 8,
      'income': '3000.0000',
      'expense': '1100.0000',
      'net': '1900.0000',
    },
  ],
  'budgetVariances': <Object>[],
  'futureLoad': {
    'fromDate': '2026-08-11',
    'throughDate': '2026-09-10',
    'recurringAmount': '125.0000',
    'creditCardStatementAmount': '350.0000',
    'installmentAmount': '100.0000',
    'totalAmount': '575.0000',
  },
  'accountDistribution': [
    {
      'accountId': 'account',
      'accountName': 'Ana hesap',
      'balance': '5150.0000',
    },
  ],
  'cardDistribution': [
    {
      'creditCardId': 'card',
      'creditCardName': 'Ana kart',
      'debt': '350.0000',
      'availableLimit': '1650.0000',
    },
  ],
};

/// Adı geçen satırın kendi eylemine dokunur ve açılan onayı kabul eder.
///
/// Sıraya güvenmiyor: liste zamanlamaya göre gruplandığı ve başarı bildirimi
/// satırları aşağı ittiği için `first`/`last` başka bir satıra denk gelebiliyor.
Future<void> _tapActionOf(
  WidgetTester tester,
  String title, {
  bool confirm = true,
}) async {
  final row = find.ancestor(
    of: find.text(title),
    matching: find.byType(AppListRow),
  );
  final action = find.descendant(of: row, matching: find.text('Gerçekleştir'));
  await tester.ensureVisible(action);
  await tester.pumpAndSettle();
  await tester.tap(action);
  await tester.pumpAndSettle();

  // Para hareket etmeden önce açık onay: tek dokunuşla bakiye değişmez.
  expect(find.text('Gerçekleştirilsin mi?'), findsOneWidget);
  // Onay butonu diyaloğun içinde aranıyor: satırın kendi eylemi de artık aynı
  // metni taşıyan dolgulu bir buton.
  await tester.tap(
    find.descendant(
      of: find.byType(Dialog),
      matching: find.widgetWithText(
        confirm ? FilledButton : TextButton,
        confirm ? 'Gerçekleştir' : 'Vazgeç',
      ),
    ),
  );
  await tester.pumpAndSettle();
}

RecurringTransactionItem _plan({
  required String id,
  required String description,
  bool isActive = true,
}) => RecurringTransactionItem(
  id: id,
  sourceType: 'account',
  accountId: 'account',
  creditCardId: null,
  categoryId: 'category',
  amount: '125.5000',
  currency: 'TRY',
  kind: 'bill-payment',
  frequency: 'monthly',
  startDate: '2026-08-15',
  endDate: null,
  nextOccurrenceDate: '2026-09-15',
  monthEndBehavior: 'clamp-to-last-day',
  description: description,
  isActive: isActive,
);

class _FakePlanningRepository implements PlanningRepositoryContract {
  _FakePlanningRepository({
    this.error,
    this.plans = const [],
    this.occurrences = const [],
    this.upcomingPayments = const [],
  });

  ApiException? error;
  final List<RecurringTransactionItem> plans;
  final List<RecurringOccurrenceItem> occurrences;
  final List<UpcomingPaymentItem> upcomingPayments;

  @override
  Future<PlanningSnapshot> load({
    required int year,
    required int month,
    required String asOfDate,
    required int daysAhead,
  }) async {
    if (error case final value?) throw value;
    return PlanningSnapshot(
      recurringTransactions: plans,
      occurrences: occurrences,
      upcomingPayments: upcomingPayments,
      report: AdvancedReport.fromJson(_reportJson),
      accounts: const [],
      categories: const [],
      creditCards: const [],
    );
  }

  @override
  Future<void> createRecurring(Map<String, Object?> input) async {}

  final List<String> realizedOccurrences = [];
  final List<String> realizedDue = [];

  @override
  Future<void> realizeOccurrence(String id) async {
    realizedOccurrences.add(id);
  }

  @override
  Future<void> realizeDue(String planId, String scheduledDate) async {
    realizedDue.add('$planId@$scheduledDate');
  }

  final List<String> deletedPlans = [];

  @override
  Future<void> deleteRecurring(String id) async {
    deletedPlans.add(id);
  }

  @override
  Future<void> setRecurringActive(String id, bool isActive) async {}
}
