import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/reminders/data/reminder_models.dart';
import 'package:business_finance_mobile/features/reminders/data/reminder_planner.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  const planner = ReminderPlanner();
  final now = DateTime(2026, 8, 27, 12);

  group('ne planlanır', () {
    test('aynı güne düşen kalemler tek bildirimde toplanır', () {
      final reminders = planner.plan(
        items: [
          _item(kind: 'payable-obligation', due: '2026-08-30'),
          _item(kind: 'payable-obligation', due: '2026-08-30'),
          _item(kind: 'card-statement', due: '2026-08-30'),
        ],
        settings: ReminderSettings.initial.copyWith(isEnabled: true),
        now: now,
      );

      expect(reminders, hasLength(1));
      expect(reminders.single.at, DateTime(2026, 8, 30, 9));
      expect(reminders.single.id, 20260830);
      expect(reminders.single.body, '2 ödenecek yükümlülük, 1 kart ekstresi');
    });

    test('gövde tutar ve karşı taraf adı taşımaz', () {
      final reminders = planner.plan(
        items: [
          _item(
            kind: 'payable-obligation',
            due: '2026-08-30',
            title: 'Ahmet Yılmaz elektrik faturası',
            amount: '1234.5600',
          ),
        ],
        settings: ReminderSettings.initial.copyWith(isEnabled: true),
        now: now,
      );

      final reminder = reminders.single;
      expect(reminder.title, isNot(contains('Ahmet')));
      expect(reminder.body, isNot(contains('Ahmet')));
      expect(reminder.body, isNot(contains('1234')));
      expect(reminder.body, '1 ödenecek yükümlülük');
    });

    test('taksitin üç türü tek kovada toplanır', () {
      final reminders = planner.plan(
        items: [
          _item(kind: 'card-installment', due: '2026-09-01'),
          _item(kind: 'debt-installment', due: '2026-09-01'),
          _item(kind: 'receivable-installment', due: '2026-09-01'),
        ],
        settings: ReminderSettings.initial.copyWith(isEnabled: true),
        now: now,
      );

      expect(reminders.single.body, '3 taksit');
    });

    test('seçilmemiş tür hiç planlanmaz', () {
      final reminders = planner.plan(
        items: [
          _item(kind: 'card-statement', due: '2026-08-30'),
          _item(kind: 'payable-obligation', due: '2026-08-31'),
        ],
        settings: ReminderSettings.initial.copyWith(
          isEnabled: true,
          kinds: {ReminderKind.obligation},
        ),
        now: now,
      );

      expect(reminders, hasLength(1));
      expect(reminders.single.at, DateTime(2026, 8, 31, 9));
    });

    test('günler tarih sırasına dizilir', () {
      final reminders = planner.plan(
        items: [
          _item(kind: 'payable-obligation', due: '2026-09-05'),
          _item(kind: 'payable-obligation', due: '2026-08-29'),
        ],
        settings: ReminderSettings.initial.copyWith(isEnabled: true),
        now: now,
      );

      expect(reminders.map((reminder) => reminder.at), [
        DateTime(2026, 8, 29, 9),
        DateTime(2026, 9, 5, 9),
      ]);
    });
  });

  group('ne planlanmaz', () {
    test('kapalıyken hiçbir şey', () {
      final reminders = planner.plan(
        items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
        settings: ReminderSettings.initial,
        now: now,
      );

      expect(reminders, isEmpty);
    });

    test('açık ama hiçbir tür seçili değilken hiçbir şey', () {
      final reminders = planner.plan(
        items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
        settings: ReminderSettings.initial.copyWith(
          isEnabled: true,
          kinds: <ReminderKind>{},
        ),
        now: now,
      );

      expect(reminders, isEmpty);
    });

    test('geçmişe kalan gün ve bugünün geçmiş saati atlanır', () {
      final reminders = planner.plan(
        items: [
          _item(kind: 'payable-obligation', due: '2026-08-20'),
          // Bugün, ama saat 09:00 çoktan geçti (şu an 12:00).
          _item(kind: 'payable-obligation', due: '2026-08-27'),
        ],
        settings: ReminderSettings.initial.copyWith(isEnabled: true),
        now: now,
      );

      expect(reminders, isEmpty);
    });

    test('bozuk tarih bütün listeyi düşürmez', () {
      final reminders = planner.plan(
        items: [
          _item(kind: 'payable-obligation', due: 'yarın'),
          _item(kind: 'payable-obligation', due: '2026-08-30'),
        ],
        settings: ReminderSettings.initial.copyWith(isEnabled: true),
        now: now,
      );

      expect(reminders, hasLength(1));
    });
  });

  test('seçilen saat bildirim saatidir', () {
    final reminders = planner.plan(
      items: [_item(kind: 'payable-obligation', due: '2026-08-30')],
      settings: ReminderSettings.initial.copyWith(
        isEnabled: true,
        hour: 18,
        minute: 30,
      ),
      now: now,
    );

    expect(reminders.single.at, DateTime(2026, 8, 30, 18, 30));
  });
}

PlannedActivity _item({
  required String kind,
  required String due,
  String title = 'Planlanan kayıt',
  String amount = '100.0000',
}) => PlannedActivity.fromJson({
  'plannedActivityId': '$kind-$due-$title',
  'plannedKind': kind,
  'effect': 'expense',
  'timing': 'upcoming',
  'readiness': 'ready',
  'actionKind': 'realize',
  'dueDate': due,
  'amount': amount,
  'currency': 'TRY',
  'title': title,
  'isProjected': false,
  'isPaymentObligation': true,
});
