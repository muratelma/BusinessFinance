import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/reminders/data/notification_scheduler.dart';
import 'package:business_finance_mobile/features/reminders/data/reminder_models.dart';
import 'package:business_finance_mobile/features/reminders/data/reminder_preferences.dart';
import 'package:business_finance_mobile/features/reminders/presentation/reminder_controller.dart';
import 'package:business_finance_mobile/features/reminders/presentation/reminder_settings_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  /// Ekran uzun bir listedir; kısa bir pencerede alt bölümler hiç kurulmaz ve
  /// arama boş döner. Testler bu yüzden yüksek bir pencere kullanıyor.
  void tallWindow(WidgetTester tester) {
    tester.view.physicalSize = const Size(400, 2000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
  }

  testWidgets('kapalıyken yalnız ana anahtar görünür', (tester) async {
    await tester.pumpWidget(_app(_controller()));
    await tester.pumpAndSettle();

    expect(find.text('Hatırlatmaları aç'), findsOneWidget);
    expect(find.text('Neler hatırlatılsın'), findsNothing);
    expect(find.text('Hatırlatma saati'), findsNothing);
  });

  testWidgets('açıldığında türler ve saat çıkar', (tester) async {
    tallWindow(tester);
    final controller = _controller(
      items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
    );
    await tester.pumpWidget(_app(controller));
    await tester.pumpAndSettle();

    await tester.tap(find.byType(Switch).first);
    await tester.pumpAndSettle();

    expect(find.text('Neler hatırlatılsın'), findsOneWidget);
    for (final kind in ReminderKind.values) {
      expect(find.text(kind.label), findsOneWidget);
    }
    expect(find.text('09:00'), findsOneWidget);
    expect(
      find.text('Önümüzdeki 30 gün için 1 güne hatırlatma kuruldu.'),
      findsOneWidget,
    );
  });

  testWidgets('izin reddedilince ekran bunu söyler ve anahtar kapalı kalır', (
    tester,
  ) async {
    final controller = _controller(grantPermission: false);
    await tester.pumpWidget(_app(controller));
    await tester.pumpAndSettle();

    await tester.tap(find.byType(Switch).first);
    await tester.pumpAndSettle();

    expect(find.textContaining('Bildirim izni verilmedi'), findsOneWidget);
    expect(find.text('Neler hatırlatılsın'), findsNothing);
    expect(controller.settings.isEnabled, isFalse);
  });

  testWidgets('liste okunamazsa kurulu hatırlatmaların durduğu yazılır', (
    tester,
  ) async {
    final repository = _FakeActivityRepository()..fails = true;
    final controller = _controller(repository: repository);
    await tester.pumpWidget(_app(controller));
    await tester.pumpAndSettle();

    await tester.tap(find.byType(Switch).first);
    await tester.pumpAndSettle();

    expect(find.textContaining('okunamadı'), findsOneWidget);
    expect(find.text('Tekrar dene'), findsOneWidget);
  });

  testWidgets(
    'bütün türler kapatılınca hiçbir hatırlatma kurulmadığı yazılır',
    (tester) async {
      tallWindow(tester);
      final controller = _controller(
        items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
      );
      await tester.pumpWidget(_app(controller));
      await tester.pumpAndSettle();
      await tester.tap(find.byType(Switch).first);
      await tester.pumpAndSettle();

      for (final kind in ReminderKind.values) {
        await tester.tap(find.widgetWithText(SwitchListTile, kind.label));
        await tester.pumpAndSettle();
      }

      expect(
        find.text('Hiçbir tür seçili değil; hatırlatma kurulmadı.'),
        findsOneWidget,
      );
    },
  );

  testWidgets('gizlilik notu her hâlde durur', (tester) async {
    await tester.pumpWidget(_app(_controller()));
    await tester.pumpAndSettle();

    expect(
      find.textContaining('Bildirimde tutar ve kişi adı yazmaz'),
      findsOneWidget,
    );
  });
}

Widget _app(ReminderController controller) => MaterialApp(
  theme: AppTheme.light(),
  home: ReminderSettingsPage(controller: controller),
);

ReminderController _controller({
  List<PlannedActivity> items = const [],
  bool grantPermission = true,
  _FakeActivityRepository? repository,
}) => ReminderController(
  _FakePreferences(),
  _FakeScheduler(grantPermission: grantPermission),
  repository ?? _FakeActivityRepository(items: items),
  now: () => DateTime(2026, 8, 27, 12),
);

class _FakeScheduler implements NotificationSchedulerContract {
  _FakeScheduler({this.grantPermission = true});

  final bool grantPermission;
  final List<ScheduledReminder> scheduled = [];

  @override
  Future<bool> requestPermission() async => grantPermission;

  @override
  Future<void> cancelAll() async => scheduled.clear();

  @override
  Future<void> schedule(ScheduledReminder reminder) async =>
      scheduled.add(reminder);
}

class _FakePreferences implements ReminderPreferencesContract {
  ReminderSettings stored = ReminderSettings.initial;

  @override
  Future<ReminderSettings> read() async => stored;

  @override
  Future<void> write(ReminderSettings settings) async => stored = settings;

  @override
  Future<void> clear() async => stored = ReminderSettings.initial;
}

class _FakeActivityRepository implements ActivityRepositoryContract {
  _FakeActivityRepository({this.items = const []});

  List<PlannedActivity> items;
  bool fails = false;

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async {
    if (fails) throw Exception('planlanan liste okunamadı');
    return PlannedActivityPage(
      asOfDate: '2026-08-27',
      daysAhead: horizon.days,
      totalCount: items.length,
      items: items,
    );
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
  Future<void> realizePlanned(PlannedActivity activity) =>
      throw UnimplementedError();
}

PlannedActivity _item({required String kind, required String due}) =>
    PlannedActivity.fromJson({
      'plannedActivityId': '$kind-$due',
      'plannedKind': kind,
      'effect': 'expense',
      'timing': 'upcoming',
      'readiness': 'ready',
      'actionKind': 'realize',
      'dueDate': due,
      'amount': '100.0000',
      'currency': 'TRY',
      'title': 'Planlanan kayıt',
      'isProjected': false,
      'isPaymentObligation': true,
    });
