import 'package:business_finance_mobile/core/widgets/app_status_chip.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/planned_activity_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/planned_activity_page.dart';
import 'package:business_finance_mobile/features/activities/presentation/planned_summary_card.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';

void main() {
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

    test('rejects an unknown code instead of showing a blank warning', () {
      expect(
        () => PlannedActivity.fromJson(_json(attention: 'meteor-strike')),
        throwsFormatException,
      );
      expect(
        () => PlannedActivity.fromJson(_json(kind: 'mortgage')),
        throwsFormatException,
      );
    });

    test('page carries a count and nearest date but never one total', () {
      final page = PlannedActivityPage.fromJson({
        'asOfDate': '2026-08-15',
        'daysAhead': 30,
        'totalCount': 2,
        'nearestDueDate': '2026-08-20',
        'items': [_json(), _json(kind: 'card-statement', action: 'pay-card')],
      });

      expect(page.totalCount, 2);
      expect(page.nearestDueDate, '2026-08-20');
      expect(page.items, hasLength(2));
    });
  });

  group('controller', () {
    test('filters by type without asking the server again', () async {
      final repository = _FakeRepository(
        page: _page([
          _activity(kind: PlannedKind.recurringOccurrence),
          _activity(kind: PlannedKind.cardStatement),
          _activity(kind: PlannedKind.debtInstallment),
        ]),
      );
      final controller = PlannedActivityController(repository);
      await controller.load();

      controller.selectTypeFilter(PlannedTypeFilter.statement);

      expect(controller.visibleItems, hasLength(1));
      expect(
        controller.visibleItems.single.plannedKind,
        PlannedKind.cardStatement,
      );
      // The horizon already bounds the result, so narrowing by type is local.
      expect(repository.requestedHorizons, [PlannedHorizon.month]);
    });

    test('debt filter covers both directions', () async {
      final repository = _FakeRepository(
        page: _page([
          _activity(kind: PlannedKind.debtInstallment),
          _activity(kind: PlannedKind.receivableInstallment),
        ]),
      );
      final controller = PlannedActivityController(repository);
      await controller.load();

      controller.selectTypeFilter(PlannedTypeFilter.debt);

      expect(controller.visibleItems, hasLength(2));
    });

    test('changing the horizon does reach the server', () async {
      final repository = _FakeRepository(page: _page([]));
      final controller = PlannedActivityController(repository);
      await controller.load();

      await controller.selectHorizon(PlannedHorizon.quarter);

      expect(repository.requestedHorizons, [
        PlannedHorizon.month,
        PlannedHorizon.quarter,
      ]);
    });

    test('reselecting the same horizon does not refetch', () async {
      final repository = _FakeRepository(page: _page([]));
      final controller = PlannedActivityController(repository);
      await controller.load();

      await controller.selectHorizon(PlannedHorizon.month);

      expect(repository.requestedHorizons, hasLength(1));
    });

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
    /// Cümle ekranda bir kez duruyor, her satırda değil.
    ///
    /// Satır başına tekrarlanan "Bakiyeye dahil değil" etiketi, ekranın
    /// tepesindeki cümlenin kopyasıydı ve on satırlık bir listede on kez
    /// yazılıyordu. Ekran okuyucu için satırda kalmaya devam ediyor.
    testWidgets('says a planned row is not in the balance', (tester) async {
      await _pumpPage(tester, _page([_activity()]));

      expect(
        find.textContaining('gerçekleşene kadar bakiyeye'),
        findsOneWidget,
      );
      expect(find.text('Bakiyeye dahil değil'), findsNothing);
      expect(
        tester.getSemantics(find.byType(AppCard)).label,
        contains('Bakiyeye dahil değil'),
      );
    });

    testWidgets('explains an attention state in words', (tester) async {
      await _pumpPage(
        tester,
        _page([_activity(attention: PlannedAttention.cardLimitInsufficient)]),
      );

      expect(
        find.textContaining('Kullanılabilir kart limiti yetersiz'),
        findsOneWidget,
      );
    });

    // Gecikme kartın kenarlığıyla anlatılıyordu: yerel bir `shape` token'ı ezip
    // bu kartı diğer bütün kartlardan farklı bir yarıçapla çiziyor, gecikmeyi de
    // ekranın en görünmez yerine koyuyordu.
    testWidgets('gecikmeyi çerçeveyle değil rozetle söyler', (tester) async {
      await _pumpPage(
        tester,
        _page([_activity(timing: PlannedTiming.overdue)]),
      );

      final chip = tester.widget<AppStatusChip>(find.byType(AppStatusChip));
      expect(chip.label, 'Gecikmiş');
      expect(chip.tone, AppStatusTone.expense);
      expect(find.byType(Card), findsNothing);
    });

    testWidgets('vakti gelmemiş satır plan tonunda kalır', (tester) async {
      await _pumpPage(tester, _page([_activity()]));

      final chip = tester.widget<AppStatusChip>(find.byType(AppStatusChip));
      expect(chip.label, 'Yaklaşan');
      expect(chip.tone, AppStatusTone.planned);
    });

    /// Occurrence'ın üretilip üretilmediği kullanıcının bilmesi gereken bir şey
    /// değil.
    ///
    /// "Henüz oluşturulmadı; onaylandığında oluşturulur" cümlesi hem yanlış bilgi
    /// veriyordu — onaylamak tam da yapılamayan şeydi, çünkü `actionTargetId`
    /// `null` gelip butonu kalıcı olarak kapatıyordu — hem de üretmek bir
    /// kullanıcı kararı değil, sistemin defter işi.
    testWidgets('üretilmemiş satır iç durumunu ekrana yazmıyor', (
      tester,
    ) async {
      await _pumpPage(
        tester,
        _page([
          _activity(
            isProjected: true,
            timing: PlannedTiming.today,
            actionTargetId: 'plan-1',
          ),
        ]),
      );

      expect(find.textContaining('Henüz oluşturulmadı'), findsNothing);
      // Ve eylem çalışıyor: sunucu eksik kaydı kendi üretiyor.
      final button = tester.widget<FilledButton>(
        find.widgetWithText(FilledButton, 'Gerçekleştir'),
      );
      expect(button.onPressed, isNotNull);
    });

    /// Vakti gelmemiş satırda eylem hiç çıkmaz, gri de çıkmaz.
    ///
    /// Kapalı bir buton "bir şey eksik" der ve kullanıcıyı eksiği aramaya
    /// gönderir; oysa eksik bir şey yok, yalnız gün gelmemiş.
    testWidgets('vakti gelmemiş satırda eylem hiç yok', (tester) async {
      await _pumpPage(tester, _page([_activity(actionTargetId: 'plan-1')]));

      expect(find.text('Gerçekleştir'), findsNothing);
    });

    testWidgets('offers the three horizons the server accepts', (tester) async {
      await _pumpPage(tester, _page([]));

      for (final horizon in PlannedHorizon.values) {
        expect(find.text(horizon.label), findsOneWidget);
      }
    });

    testWidgets('describes a row in one sentence for a screen reader', (
      tester,
    ) async {
      final handle = tester.ensureSemantics();
      await _pumpPage(
        tester,
        _page([_activity(attention: PlannedAttention.cardInactive)]),
      );

      expect(
        find.bySemanticsLabel(RegExp('Bakiyeye dahil değil')),
        findsOneWidget,
      );
      handle.dispose();
    });
  });

  group('summary card', () {
    testWidgets('reports a count and the nearest date, not a total', (
      tester,
    ) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: PlannedSummaryCard(
              count: 3,
              nearestDueDate: '2026-08-20',
              onTap: () {},
            ),
          ),
        ),
      );

      expect(find.text('3 planlanan işlem'), findsOneWidget);
      // Ham `2026-08-20` sunucunun iç gösterimi; ekranda okunur tarih durur.
      expect(find.textContaining('En yakını 20 Ağustos'), findsOneWidget);
      expect(find.textContaining('Bakiyeye dahil değil'), findsOneWidget);
    });

    testWidgets('stays out of the way when nothing is planned', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(body: PlannedSummaryCard(count: 0, onTap: () {})),
        ),
      );

      expect(find.textContaining('planlanan işlem'), findsNothing);
    });
  });

  group('planlanan satırın eylemi', () {
    test('sözleşme eylemin hangi kaydı adresleyeceğini taşır', () {
      // `actionKind` tek başına yetmiyordu: hangi kayıt üzerinde çağrılacağı
      // yoktu, o yüzden ekran eylemi hiç sunamıyordu.
      final item = PlannedActivity.fromJson({
        ..._json(kind: 'card-installment', attention: null),
        'actionTargetId': 'plan-1',
        'actionSequence': 4,
      });

      expect(item.actionTargetId, 'plan-1');
      expect(item.actionSequence, 4);
      expect(item.isDirectlyRealizable, isTrue);
    });

    test('engelli ya da vakti gelmemiş satır tek dokunuşla gerçekleşmez', () {
      // Limiti dolu kart: yapılacak iş var ama şu an yapılamıyor.
      final blocked = PlannedActivity.fromJson({
        ..._json(kind: 'card-installment'),
        'actionTargetId': 'plan-1',
        'actionSequence': 1,
      });
      // Vakti gelmemiş: plan tarihi gelene kadar bir tahmindir ve gelecek ayın
      // kirasını bugün yazmak parayı çıkmadığı bir aya koyar.
      final future = PlannedActivity.fromJson({
        ..._json(attention: null),
        'timing': 'upcoming',
        'actionTargetId': 'plan-1',
      });
      // Üretilmemiş ama vakti gelmiş satır **gerçekleştirilebilir**: sunucu eksik
      // kaydı kendi üretiyor ve satır planı adresliyor.
      final unborn = PlannedActivity.fromJson({
        ..._json(attention: null),
        'timing': 'today',
        'isProjected': true,
        'actionTargetId': 'plan-1',
      });

      expect(blocked.isDirectlyRealizable, isFalse);
      expect(future.isDirectlyRealizable, isFalse);
      expect(unborn.isDirectlyRealizable, isTrue);
    });

    testWidgets('gerçekleştirilebilir satırda buton çıkar ve onay sorar', (
      tester,
    ) async {
      final repository = _FakeRepository(
        page: _page([
          _activity(
            kind: PlannedKind.cardInstallment,
            actionTargetId: 'plan-1',
            timing: PlannedTiming.today,
          ),
        ]),
      );
      await _pumpPageWith(tester, repository);

      await tester.tap(find.widgetWithText(FilledButton, 'Gerçekleştir'));
      await tester.pumpAndSettle();

      // Onaysız para hareketi olmaz.
      expect(find.text('Gerçekleştirilsin mi?'), findsOneWidget);
      expect(repository.realizedPlanned, isEmpty);

      await tester.tap(find.widgetWithText(FilledButton, 'Gerçekleştir').last);
      await tester.pumpAndSettle();

      expect(repository.realizedPlanned, hasLength(1));
      expect(repository.realizedPlanned.single.actionTargetId, 'plan-1');
    });

    testWidgets('engelli satırın butonu kapalı durur', (tester) async {
      // Nedeni zaten satırda yazılı; buton sessizce çalışmıyor görünmemeli.
      final repository = _FakeRepository(
        page: _page([
          _activity(
            kind: PlannedKind.cardInstallment,
            attention: PlannedAttention.cardLimitInsufficient,
            actionTargetId: 'plan-1',
            timing: PlannedTiming.today,
          ),
        ]),
      );
      await _pumpPageWith(tester, repository);

      final button = tester.widget<FilledButton>(
        find.widgetWithText(FilledButton, 'Gerçekleştir'),
      );
      expect(button.onPressed, isNull);
    });

    testWidgets('ödeme isteyen satır kendi ekranına yollar', (tester) async {
      // Ödeme hangi hesaptan yapılacağını sorar; formu bu listeye kopyalamak
      // yerine kullanıcı o kaydın ekranına gider.
      final repository = _FakeRepository(
        page: _page([
          _activity(
            kind: PlannedKind.cardStatement,
            action: PlannedAction.payCard,
            actionTargetId: 'card-1',
          ),
        ]),
      );
      await _pumpPageWith(tester, repository);

      expect(
        find.widgetWithText(FilledButton, 'Kart ödemesi yap →'),
        findsOneWidget,
      );
      // Doğrudan gerçekleştirme yolu bu tür için kapalı.
      expect(repository.realizedPlanned, isEmpty);
    });
  });
}

Future<void> _pumpPage(WidgetTester tester, PlannedActivityPage page) async {
  tester.view.physicalSize = const Size(500, 1200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: PlannedActivityPageView(repository: _FakeRepository(page: page)),
    ),
  );
  await tester.pumpAndSettle();
}

Future<void> _pumpPageWith(
  WidgetTester tester,
  _FakeRepository repository,
) async {
  tester.view.physicalSize = const Size(500, 1200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: PlannedActivityPageView(repository: repository),
    ),
  );
  await tester.pumpAndSettle();
}

PlannedActivityPage _page(List<PlannedActivity> items) => PlannedActivityPage(
  asOfDate: '2026-08-15',
  daysAhead: 30,
  totalCount: items.length,
  nearestDueDate: items.isEmpty ? null : items.first.dueDate,
  items: items,
);

PlannedActivity _activity({
  PlannedKind kind = PlannedKind.recurringOccurrence,
  PlannedAttention? attention,
  bool isProjected = false,
  bool isPaymentObligation = true,
  PlannedAction action = PlannedAction.realize,
  String? actionTargetId,
  int? actionSequence = 1,
  PlannedTiming timing = PlannedTiming.upcoming,
}) => PlannedActivity(
  plannedActivityId: '${kind.apiValue}-1',
  plannedKind: kind,
  effect: ActivityEffect.expense,
  timing: timing,
  readiness: attention == null
      ? PlannedReadiness.ready
      : PlannedReadiness.needsAttention,
  attentionCode: attention,
  actionKind: action,
  dueDate: '2026-08-20',
  amount: '149.9000',
  currency: 'TRY',
  title: 'Streaming',
  sourceName: 'Test Kart',
  isProjected: isProjected,
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
  _FakeRepository({this.page, this.error});

  final PlannedActivityPage? page;
  final Object? error;
  final List<PlannedHorizon> requestedHorizons = [];

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async {
    requestedHorizons.add(horizon);
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
  Future<void> cancel(FinancialActivity activity) => throw UnimplementedError();

  @override
  Future<void> realizePlanned(PlannedActivity activity) async {
    realizedPlanned.add(activity);
  }

  final List<PlannedActivity> realizedPlanned = [];
}
