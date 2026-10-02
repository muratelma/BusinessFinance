import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/reminders/data/notification_scheduler.dart';
import 'package:business_finance_mobile/features/reminders/data/reminder_models.dart';
import 'package:business_finance_mobile/features/reminders/data/reminder_preferences.dart';
import 'package:business_finance_mobile/features/reminders/presentation/reminder_controller.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  final now = DateTime(2026, 8, 27, 12);

  ReminderController build({
    _FakeScheduler? scheduler,
    _FakePreferences? preferences,
    _FakeActivityRepository? repository,
  }) => ReminderController(
    preferences ?? _FakePreferences(),
    scheduler ?? _FakeScheduler(),
    repository ?? _FakeActivityRepository(),
    now: () => now,
  );

  test('kapalıyken izin sorulmaz ve hiçbir bildirim kurulmaz', () async {
    final scheduler = _FakeScheduler();
    final repository = _FakeActivityRepository();
    final controller = build(scheduler: scheduler, repository: repository);

    await controller.ensureLoaded();

    expect(controller.settings.isEnabled, isFalse);
    expect(scheduler.permissionRequests, 0);
    expect(scheduler.scheduled, isEmpty);
    expect(repository.plannedReads, 0);
  });

  test('izin reddedilirse hatırlatma açılmaz ama uygulama çalışır', () async {
    final scheduler = _FakeScheduler(grantPermission: false);
    final preferences = _FakePreferences();
    final controller = build(scheduler: scheduler, preferences: preferences);
    await controller.ensureLoaded();

    await controller.setEnabled(true);

    expect(controller.permissionDenied, isTrue);
    expect(controller.settings.isEnabled, isFalse);
    expect(scheduler.scheduled, isEmpty);
    expect(preferences.written, isNull);
  });

  test('açıldığında liste okunur ve bildirimler kurulur', () async {
    final scheduler = _FakeScheduler();
    final preferences = _FakePreferences();
    final repository = _FakeActivityRepository(
      items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
    );
    final controller = build(
      scheduler: scheduler,
      preferences: preferences,
      repository: repository,
    );
    await controller.ensureLoaded();

    await controller.setEnabled(true);

    expect(repository.horizons, [PlannedHorizon.month]);
    expect(scheduler.scheduled, hasLength(1));
    expect(controller.scheduledCount, 1);
    expect(preferences.written?.isEnabled, isTrue);
  });

  test('veri değişince kurulu bildirim düşer', () async {
    final scheduler = _FakeScheduler();
    final repository = _FakeActivityRepository(
      items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
    );
    final controller = build(scheduler: scheduler, repository: repository);
    await controller.ensureLoaded();
    await controller.setEnabled(true);
    expect(scheduler.scheduled, hasLength(1));

    // Yükümlülük ödendi: planlanan listeden düştü.
    repository.items = const [];
    await controller.sync();

    expect(scheduler.scheduled, isEmpty);
    expect(scheduler.cancelAllCalls, greaterThan(1));
    expect(controller.scheduledCount, 0);
  });

  test('tür kapatmak o türün bildirimini düşürür', () async {
    final scheduler = _FakeScheduler();
    final repository = _FakeActivityRepository(
      items: [
        _item(kind: 'payable-obligation', due: '2026-08-30'),
        _item(kind: 'card-statement', due: '2026-09-02'),
      ],
    );
    final controller = build(scheduler: scheduler, repository: repository);
    await controller.ensureLoaded();
    await controller.setEnabled(true);
    expect(scheduler.scheduled, hasLength(2));

    await controller.setKindEnabled(ReminderKind.cardStatement, false);

    expect(scheduler.scheduled, hasLength(1));
    expect(scheduler.scheduled.single.at, DateTime(2026, 8, 30, 9));
  });

  test('liste okunamazsa kurulu bildirimler olduğu gibi kalır', () async {
    final scheduler = _FakeScheduler();
    final repository = _FakeActivityRepository(
      items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
    );
    final controller = build(scheduler: scheduler, repository: repository);
    await controller.ensureLoaded();
    await controller.setEnabled(true);
    final scheduledBefore = List.of(scheduler.scheduled);

    repository.fails = true;
    await controller.sync();

    expect(controller.syncFailed, isTrue);
    expect(scheduler.scheduled, scheduledBefore);
  });

  test('oturum kapanınca ayar ve kurulu bildirimler unutulur', () async {
    final scheduler = _FakeScheduler();
    final preferences = _FakePreferences();
    final repository = _FakeActivityRepository(
      items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
    );
    final controller = build(
      scheduler: scheduler,
      preferences: preferences,
      repository: repository,
    );
    await controller.ensureLoaded();
    await controller.setEnabled(true);

    await controller.forget();

    expect(controller.settings, ReminderSettings.initial);
    expect(scheduler.scheduled, isEmpty);
    expect(preferences.cleared, isTrue);
  });

  test('saat değiştirmek bildirimi yeni saate taşır', () async {
    final scheduler = _FakeScheduler();
    final repository = _FakeActivityRepository(
      items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
    );
    final controller = build(scheduler: scheduler, repository: repository);
    await controller.ensureLoaded();
    await controller.setEnabled(true);

    await controller.setTime(hour: 20, minute: 15);

    expect(scheduler.scheduled.single.at, DateTime(2026, 8, 30, 20, 15));
  });
}

class _FakeScheduler implements NotificationSchedulerContract {
  _FakeScheduler({this.grantPermission = true});

  final bool grantPermission;
  final List<ScheduledReminder> scheduled = [];
  int permissionRequests = 0;
  int cancelAllCalls = 0;

  @override
  Future<bool> requestPermission() async {
    permissionRequests++;
    return grantPermission;
  }

  @override
  Future<void> cancelAll() async {
    cancelAllCalls++;
    scheduled.clear();
  }

  @override
  Future<void> schedule(ScheduledReminder reminder) async =>
      scheduled.add(reminder);
}

class _FakePreferences implements ReminderPreferencesContract {
  ReminderSettings stored = ReminderSettings.initial;
  ReminderSettings? written;
  bool cleared = false;

  @override
  Future<ReminderSettings> read() async => stored;

  @override
  Future<void> write(ReminderSettings settings) async {
    written = settings;
    stored = settings;
  }

  @override
  Future<void> clear() async => cleared = true;
}

class _FakeActivityRepository implements ActivityRepositoryContract {
  @override
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async => balances;

  /// İşlem ayrıntısındaki "kalan bakiye" için dönecek cevap.
  List<ActivityBalance> balances = const [];

  _FakeActivityRepository({this.items = const []});

  List<PlannedActivity> items;
  bool fails = false;
  int plannedReads = 0;
  final List<PlannedHorizon> horizons = [];

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async {
    plannedReads++;
    horizons.add(horizon);
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
