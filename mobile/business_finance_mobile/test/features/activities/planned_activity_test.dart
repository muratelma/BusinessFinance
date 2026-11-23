import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_error_messages.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/planned_activity_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/planned_activity_page.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';

/// Örnek gün: 15 Ağustos 2026. Özet girişinde dönem 15–21 Ağustos'tur.
final _today = DateTime(2026, 8, 15);

void main() {
  for (final openOverdue in [true, false]) {
    testWidgets(
      'Özet girişi aynı haftayı okur, gecikenlerin açık durumu $openOverdue',
      (tester) async {
        final repository = _FakeRepository(
          page: _page(
            [
              _activity(dueDate: '2026-07-25', title: 'Temmuz ödemesi'),
              _activity(dueDate: '2026-08-20', title: 'Ağustos ödemesi'),
            ],
            period: _period(
              paymentCount: 1,
              paymentTotal: '149.9000',
              earlierCount: 1,
              earlierTotal: '149.9000',
            ),
          ),
        );
        await _pumpPageWith(tester, repository, openOverdue: openOverdue);

        expect(repository.ranges, [
          (DateTime(2026, 8, 15), DateTime(2026, 8, 21)),
        ]);
        expect(find.text('15–21 Ağustos'), findsOneWidget);
        expect(find.text('Ağustos ödemesi'), findsOneWidget);
        expect(find.text('1 geciken kayıt'), findsOneWidget);
        expect(find.text('15 Ağustos öncesi'), findsOneWidget);
        expect(
          find.text('Temmuz ödemesi'),
          openOverdue ? findsOneWidget : findsNothing,
        );
      },
    );
  }

  group('parsing', () {
    test('reads a planned item including readiness and projection', () {
      final item = PlannedActivity.fromJson(_json());

      expect(item.plannedKind, PlannedKind.recurringOccurrence);
      expect(item.timing, PlannedTiming.overdue);
      expect(item.readiness, PlannedReadiness.needsAttention);
      expect(item.attentionCode, PlannedAttention.cardLimitInsufficient);
      expect(item.actionKind, PlannedAction.realize);
      expect(item.amount, '149.9000');
      expect(item.isProjected, isFalse);
    });

    test('tutarı belli olmayan vergi kalemi boş tutarla okunur', () {
      final item = PlannedActivity.fromJson({
        ..._json(attention: null),
        'timing': 'upcoming',
        'amount': null,
        'recurringTransactionId': 'plan-1',
        'taxKind': 'vat-return',
      });

      expect(item.amount, isNull);
      expect(item.isTax, isTrue);
      expect(item.recurringTransactionId, 'plan-1');
      expect(item.isDirectlyRealizable, isFalse);
    });

    test('reads both one-time obligation directions and actions', () {
      final payable = PlannedActivity.fromJson(
        _json(
          kind: 'payable-obligation',
          action: 'pay-obligation',
          attention: null,
        ),
      );
      final receivable = PlannedActivity.fromJson(
        _json(
          kind: 'receivable-obligation',
          action: 'collect-obligation',
          attention: null,
        ),
      );

      expect(payable.plannedKind, PlannedKind.payableObligation);
      expect(payable.actionKind, PlannedAction.payObligation);
      expect(receivable.plannedKind, PlannedKind.receivableObligation);
      expect(receivable.actionKind, PlannedAction.collectObligation);
    });

    test('a single item rejects an unknown code', () {
      expect(
        () => PlannedActivity.fromJson(_json(attention: 'meteor-strike')),
        throwsFormatException,
      );
    });

    // Yeni sunucu, eski uygulama: tanınmayan tür bütün listeyi bozmaz; atlanır
    // ve sayılır, ekran bunu söyler.
    test('the page skips an unknown kind and counts it', () {
      final page = PlannedActivityPage.fromJson({
        'asOfDate': '2026-08-15',
        'daysAhead': 7,
        'totalCount': 2,
        'items': [_json(), _json(kind: 'mortgage')],
        'period': _periodJson(),
      });

      expect(page.items, hasLength(1));
      expect(page.skippedCount, 1);
      expect(page.period!.paymentTotal, '300.0000');
    });
  });

  group('controller', () {
    test('asks the server for 30 days from today by default', () async {
      final repository = _FakeRepository(page: _page([]));
      final controller = PlannedActivityController(repository);
      await controller.load();

      final today = DateTime.now();
      final start = DateTime(today.year, today.month, today.day);
      expect(repository.ranges.single.$1, start);
      expect(
        repository.ranges.single.$2,
        DateTime(start.year, start.month, start.day + 29),
      );
    });

    test('sends the type to the server so the list and totals agree', () async {
      final repository = _FakeRepository(page: _page([]));
      final controller = PlannedActivityController(repository, asOf: _today);
      await controller.load();

      await controller.apply(
        period: controller.period,
        type: PlannedTypeFilter.tax,
      );

      expect(repository.types, [PlannedTypeFilter.all, PlannedTypeFilter.tax]);
      await controller.clearType();
      expect(repository.types.last, PlannedTypeFilter.all);
    });

    test('a custom range reaches the server as chosen', () async {
      final repository = _FakeRepository(page: _page([]));
      final controller = PlannedActivityController(repository, asOf: _today);
      await controller.load();

      await controller.apply(
        period: PlannedPeriodChoice.range(
          DateTime(2026, 9, 1),
          DateTime(2026, 9, 30),
        ),
        type: PlannedTypeFilter.all,
      );

      expect(repository.ranges.last, (
        DateTime(2026, 9, 1),
        DateTime(2026, 9, 30),
      ));
      expect(controller.periodLabel, '1–30 Eylül');
    });

    test(
      'a failed refresh of the same selection keeps the last list',
      () async {
        final repository = _FakeRepository(page: _page([_activity()]));
        final controller = PlannedActivityController(repository, asOf: _today);
        await controller.load();

        repository.error = const ApiException(
          code: 'network',
          message: 'Bağlantı yok',
          statusCode: 0,
        );
        await controller.load();

        expect(controller.page!.items, hasLength(1));
        expect(controller.refreshFailed, isTrue);
        expect(controller.errorMessage, isNull);
      },
    );

    test(
      'an expired session is reported rather than shown as nothing planned',
      () async {
        final repository = _FakeRepository(
          error: const ApiException(
            code: 'authentication.unauthorized',
            message: 'Oturum sona erdi',
            statusCode: 401,
          ),
        );
        final controller = PlannedActivityController(repository);

        await controller.load();

        expect(controller.unauthorized, isTrue);
        expect(controller.isEmpty, isFalse);
      },
    );
  });

  group('list', () {
    testWidgets('dönem kartı yalnız kaydı olan toplamları yazar', (
      tester,
    ) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(
          page: _page(
            [_activity()],
            period: _period(
              paymentCount: 2,
              paymentTotal: '300.0000',
              unknown: 1,
            ),
          ),
        ),
      );

      expect(find.text('Ödenecek'), findsOneWidget);
      expect(find.text('Tutarı eksik: 1 ödeme'), findsOneWidget);
      expect(find.text('Beklenen giriş'), findsNothing);
      expect(find.text('Karta yansıyacak'), findsNothing);
    });

    testWidgets('satır türü, kaynağı ve kısa eylemi yazar', (tester) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(page: _page([_activity(actionTargetId: 'plan-1')])),
      );

      expect(find.text('Tekrarlayan ödeme'), findsOneWidget);
      expect(find.text('Test Kart · Hesaptan ödenecek'), findsOneWidget);
      expect(find.text('Ödeme kaydet ›'), findsOneWidget);
    });

    testWidgets('explains an attention state in words', (tester) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(
          page: _page([
            _activity(attention: PlannedAttention.cardLimitInsufficient),
          ]),
        ),
      );

      expect(
        find.textContaining('Kullanılabilir kart limiti yetersiz'),
        findsOneWidget,
      );
    });

    testWidgets('bugünün kaydı gün başlığında Bugün yazar', (tester) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(page: _page([_activity(dueDate: '2026-08-15')])),
      );

      expect(find.textContaining('Bugün', findRichText: true), findsOneWidget);
    });

    testWidgets('tanınmayan kayıt varsa ekran bunu söyler', (tester) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(
          page: PlannedActivityPage(
            asOfDate: '2026-08-15',
            daysAhead: 7,
            totalCount: 2,
            items: [_activity()],
            skippedCount: 1,
          ),
        ),
      );

      expect(
        find.textContaining('1 kayıt bu sürümde gösterilemiyor'),
        findsOneWidget,
      );
    });

    testWidgets('dönem boş, önceki gecikme varsa ikisi ayrı görünür', (
      tester,
    ) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(page: _page([_activity(dueDate: '2026-07-25')])),
      );

      expect(find.text('1 geciken kayıt'), findsOneWidget);
      expect(find.text('Bu dönemde bekleyen kayıt yok.'), findsOneWidget);
    });

    testWidgets('günü gelmemiş kart taksidi planı gösterir, yansıtmaz', (
      tester,
    ) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(
          page: _page([
            _activity(
              kind: PlannedKind.cardInstallment,
              actionTargetId: 'plan-1',
            ),
          ]),
        ),
      );

      expect(find.text('Planı görüntüle ›'), findsOneWidget);
      expect(find.text('Karta yansıt ›'), findsNothing);
    });

    // Kişinin vadesi kişinin sayfasında ödenir (K6); satır oraya götürür.
    // Adresi olmayan satır bilgi satırıdır.
    testWidgets('kişinin vadesi kişi sayfasına götürür', (tester) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(
          page: _page([
            _activity(
              kind: PlannedKind.counterpartyPayable,
              action: PlannedAction.payCounterparty,
              actionTargetId: 'person-1',
            ),
          ]),
        ),
      );

      expect(find.text('Öde ›'), findsOneWidget);
    });

    testWidgets('adresi olmayan kişi vadesi bilgi satırıdır', (tester) async {
      await _pumpPageWith(
        tester,
        _FakeRepository(
          page: _page([
            _activity(
              kind: PlannedKind.counterpartyPayable,
              action: PlannedAction.payCounterparty,
            ),
          ]),
        ),
      );

      expect(find.text('Öde ›'), findsNothing);
    });

    testWidgets('describes a row in one sentence for a screen reader', (
      tester,
    ) async {
      final handle = tester.ensureSemantics();
      await _pumpPageWith(
        tester,
        _FakeRepository(page: _page([_activity(actionTargetId: 'plan-1')])),
      );

      expect(
        find.bySemanticsLabel(
          RegExp('Streaming.*Tekrarlayan ödeme.*Ödeme kaydet'),
        ),
        findsOneWidget,
      );
      handle.dispose();
    });
  });

  group('planlanan satırın eylemi', () {
    test('engelli satır ve günü gelmemiş taksit gerçekleşmez', () {
      final blocked = PlannedActivity.fromJson({
        ..._json(kind: 'card-installment'),
        'actionTargetId': 'plan-1',
        'actionSequence': 1,
      });
      final future = PlannedActivity.fromJson({
        ..._json(attention: null),
        'timing': 'upcoming',
        'actionTargetId': 'plan-1',
      });
      final futureInstallment = PlannedActivity.fromJson({
        ..._json(kind: 'card-installment', attention: null),
        'timing': 'upcoming',
        'actionTargetId': 'plan-1',
        'actionSequence': 2,
      });

      expect(blocked.isDirectlyRealizable, isFalse);
      expect(future.isDirectlyRealizable, isTrue);
      expect(futureInstallment.isDirectlyRealizable, isFalse);
    });

    testWidgets('vadesi gelmiş taksit onay sorar, onayla karta yansır', (
      tester,
    ) async {
      final repository = _FakeRepository(
        page: _page([
          _activity(
            kind: PlannedKind.cardInstallment,
            actionTargetId: 'plan-1',
            timing: PlannedTiming.today,
            dueDate: '2026-08-15',
          ),
        ]),
      );
      await _pumpPageWith(tester, repository);

      await tester.tap(find.text('Karta yansıt ›'));
      await tester.pumpAndSettle();
      expect(find.text('Karta yansıtılsın mı?'), findsOneWidget);
      expect(repository.realizedPlanned, isEmpty);

      await tester.tap(find.widgetWithText(FilledButton, 'Karta yansıt'));
      await tester.pumpAndSettle();
      expect(repository.realizedPlanned.single.actionTargetId, 'plan-1');
    });

    testWidgets('satırın tamamı aynı paneli açar; gün sorulur', (tester) async {
      final repository = _FakeRepository(
        page: _page([_activity(actionTargetId: 'plan-1')]),
      );
      await _pumpPageWith(tester, repository);

      await tester.tap(find.text('Streaming'));
      await tester.pumpAndSettle();

      final sheet = find.byType(AppFormSheet<String>);
      expect(sheet, findsOneWidget);
      expect(repository.realizedPlanned, isEmpty);

      await tester.tap(
        find.descendant(of: sheet, matching: find.text('Gerçekleştir')),
      );
      await tester.pumpAndSettle();

      expect(repository.realizedPlanned, hasLength(1));
      expect(repository.paidOn, [AppDateField.format(DateTime.now())]);
    });

    testWidgets('önceki kalem bekliyorsa onay sorar, onayla yineler', (
      tester,
    ) async {
      final repository = _FakeRepository(
        page: _page([_activity(actionTargetId: 'plan-1')]),
      )..earlierItemPending = true;
      await _pumpPageWith(tester, repository);

      Future<void> openAndSubmit() async {
        await tester.tap(find.text('Ödeme kaydet ›'));
        await tester.pumpAndSettle();
        await tester.tap(
          find.descendant(
            of: find.byType(AppFormSheet<String>),
            matching: find.text('Gerçekleştir'),
          ),
        );
        await tester.pumpAndSettle();
      }

      await openAndSubmit();
      expect(find.text('Önceki kalem bekliyor'), findsOneWidget);
      await tester.tap(find.text('Yine de gerçekleştir'));
      await tester.pumpAndSettle();

      expect(repository.realizedPlanned, hasLength(1));
      expect(repository.allowEarlierPending, [false, true]);
    });
  });
}

PlannedPeriod _period({
  int paymentCount = 0,
  String paymentTotal = '0.0000',
  int unknown = 0,
  int earlierCount = 0,
  String earlierTotal = '0.0000',
}) => PlannedPeriod(
  from: '2026-08-15',
  to: '2026-08-21',
  paymentTotal: paymentTotal,
  paymentCount: paymentCount,
  unknownAmountCount: unknown,
  incomingTotal: '0.0000',
  incomingCount: 0,
  cardPostingTotal: '0.0000',
  cardPostingCount: 0,
  earlierOverdueCount: earlierCount,
  earlierOverduePaymentTotal: earlierTotal,
  earlierOverdueUnknownAmountCount: 0,
);

Map<String, dynamic> _periodJson() => {
  'from': '2026-08-15',
  'to': '2026-08-21',
  'paymentTotal': '300.0000',
  'paymentCount': 2,
  'unknownAmountCount': 0,
  'incomingTotal': '0.0000',
  'incomingCount': 0,
  'cardPostingTotal': '0.0000',
  'cardPostingCount': 0,
  'earlierOverdueCount': 0,
  'earlierOverduePaymentTotal': '0.0000',
  'earlierOverdueUnknownAmountCount': 0,
};

Future<void> _pumpPageWith(
  WidgetTester tester,
  _FakeRepository repository, {
  bool openOverdue = false,
}) async {
  tester.view.physicalSize = const Size(500, 1600);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: PlannedActivityPageView(
        repository: repository,
        asOf: _today,
        openOverdue: openOverdue,
      ),
    ),
  );
  await tester.pumpAndSettle();
}

PlannedActivityPage _page(
  List<PlannedActivity> items, {
  PlannedPeriod? period,
}) => PlannedActivityPage(
  asOfDate: '2026-08-15',
  daysAhead: 7,
  totalCount: items.length,
  nearestDueDate: items.isEmpty ? null : items.first.dueDate,
  items: items,
  period: period,
);

PlannedActivity _activity({
  PlannedKind kind = PlannedKind.recurringOccurrence,
  PlannedAttention? attention,
  bool isPaymentObligation = true,
  PlannedAction action = PlannedAction.realize,
  String? actionTargetId,
  int? actionSequence = 1,
  PlannedTiming timing = PlannedTiming.upcoming,
  String dueDate = '2026-08-20',
  String title = 'Streaming',
}) => PlannedActivity(
  plannedActivityId: '${kind.apiValue}-$dueDate-$title',
  plannedKind: kind,
  effect: ActivityEffect.expense,
  timing: DateTime.parse(dueDate).isBefore(_today)
      ? PlannedTiming.overdue
      : timing,
  readiness: attention == null
      ? PlannedReadiness.ready
      : PlannedReadiness.needsAttention,
  attentionCode: attention,
  actionKind: action,
  dueDate: dueDate,
  amount: '149.9000',
  currency: 'TRY',
  title: title,
  sourceName: 'Test Kart',
  isProjected: false,
  isPaymentObligation: isPaymentObligation,
  actionTargetId: actionTargetId,
  actionSequence: actionSequence,
);

Map<String, dynamic> _json({
  String kind = 'recurring-occurrence',
  String action = 'realize',
  String? attention = 'card-limit-insufficient',
}) => <String, dynamic>{
  'plannedActivityId': 'p1',
  'plannedKind': kind,
  'effect': 'expense',
  'timing': 'overdue',
  'readiness': attention == null ? 'ready' : 'needs-attention',
  'attentionCode': attention,
  'actionKind': action,
  'dueDate': '2026-08-10',
  'amount': '149.9000',
  'currency': 'TRY',
  'title': 'Streaming',
  'description': null,
  'sourceId': 'card-1',
  'sourceName': 'Test Kart',
  'categoryId': null,
  'categoryName': null,
  'isProjected': false,
  'isPaymentObligation': true,
  'actionTargetId': null,
  'actionSequence': null,
};

class _FakeRepository implements ActivityRepositoryContract {
  @override
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async => const [];

  _FakeRepository({this.page, this.error});

  final PlannedActivityPage? page;
  Object? error;
  final List<(DateTime, DateTime)> ranges = [];
  final List<PlannedTypeFilter> types = [];

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) => throw UnimplementedError();

  @override
  Future<PlannedActivityPage> listPlannedRange({
    required DateTime from,
    required DateTime to,
    DateTime? today,
    TransactionScope? scope,
    PlannedTypeFilter type = PlannedTypeFilter.all,
  }) async {
    ranges.add((from, to));
    types.add(type);
    if (error != null) throw error!;
    return page!;
  }

  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) => throw UnimplementedError();

  @override
  Future<void> cancel(FinancialActivity activity, {bool withPaidNow = false}) =>
      throw UnimplementedError();

  @override
  Future<void> realizePlanned(
    PlannedActivity activity, {
    String? paidOn,
    bool allowEarlierPending = false,
  }) async {
    this.allowEarlierPending.add(allowEarlierPending);
    if (earlierItemPending && !allowEarlierPending) {
      throw ApiException(
        statusCode: 409,
        code: 'recurring.earlier_item_pending',
        message: ApiErrorMessages.resolve('recurring.earlier_item_pending'),
        pendingScheduledDate: '2026-11-20',
        pendingCount: 1,
      );
    }
    realizedPlanned.add(activity);
    this.paidOn.add(paidOn);
  }

  bool earlierItemPending = false;
  final List<PlannedActivity> realizedPlanned = [];
  final List<String?> paidOn = [];
  final List<bool> allowEarlierPending = [];
}
