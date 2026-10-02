import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_feed_page.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_test/flutter_test.dart';

import 'design_sample_data.dart';
import 'screenshot_harness.dart';

void main() {
  Widget page() => ActivityFeedPage(
    repository: _DesignFeed(),
    onShowPlanned: () {},
    now: () => designToday,
  );

  testWidgets('06 işlemler', (tester) async {
    await captureScreen(tester, '06-islemler', page(), selectedTab: 1);
  }, skip: !screenshotsEnabled);

  Future<void> detail(WidgetTester tester, String name, String title) =>
      captureScreen(
        tester,
        name,
        page(),
        selectedTab: 1,
        before: (tester) async {
          await tester.scrollUntilVisible(
            find.text(title),
            200,
            scrollable: find.byType(Scrollable).last,
          );
          await tester.tap(find.text(title).first);
        },
      );

  testWidgets('07 işlem detayı · gider', (tester) async {
    await detail(tester, '07-islem-detay-gider', 'Banka ve POS komisyonu');
  }, skip: !screenshotsEnabled);

  testWidgets('08 işlem detayı · transfer', (tester) async {
    await detail(
      tester,
      '08-islem-detay-transfer',
      'Kart ekstresi kismi odeme',
    );
  }, skip: !screenshotsEnabled);

  testWidgets('09 işlem detayı · iptal', (tester) async {
    await detail(tester, '09-islem-detay-iptal', 'Toner ve zimba');
  }, skip: !screenshotsEnabled);
}

class _DesignFeed extends Fake implements ActivityRepositoryContract {
  @override
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async => const [
    ActivityBalance(
      holder: ActivityBalanceHolder.account,
      name: 'Ziraat Vadesiz',
      balance: '48250.0000',
      currency: 'TRY',
      change: ActivityBalanceChange.decreased,
    ),
  ];

  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) async => ActivityPage(
    items: _items,
    pagination: ActivityPagination(
      pageNumber: 1,
      pageSize: 20,
      totalCount: _items.length,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false,
    ),
  );

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async => const PlannedActivityPage(
    asOfDate: '2026-09-25',
    daysAhead: 30,
    totalCount: 21,
    items: [],
  );
}

FinancialActivity _a(
  String title,
  String date,
  ActivityKind kind,
  ActivityEffect effect,
  String amount, {
  String? category,
  String? source,
  String? destination,
  ActivitySourceGroup group = ActivitySourceGroup.account,
  bool cancelled = false,
  bool canCancel = true,
  TransactionScope? scope = TransactionScope.business,
}) => FinancialActivity(
  activityId: title,
  kind: kind,
  effect: effect,
  sourceGroup: group,
  origin: ActivityOrigin.manual,
  status: cancelled ? ActivityStatus.cancelled : ActivityStatus.realized,
  activityDate: date,
  amount: amount,
  currency: 'TRY',
  title: title,
  categoryName: category,
  sourceName: source,
  destinationName: destination,
  canCancel: canCancel && !cancelled,
  supportsAttachments: false,
  scope: scope,
);

final _items = [
  _a(
    'Gun sonu sayimi',
    '2026-09-24',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '85.0000',
    category: 'Diğer işletme gideri',
    source: 'Dukkan Kasasi',
  ),
  _a(
    'Kart ekstresi kismi odeme',
    '2026-09-23',
    ActivityKind.cardPayment,
    ActivityEffect.neutral,
    '5000.0000',
    source: 'Ziraat Vadesiz',
    destination: 'Ticari Kart',
    group: ActivitySourceGroup.creditCard,
    scope: null,
  ),
  _a(
    'Isyeri elektrik faturasi',
    '2026-09-23',
    ActivityKind.obligationSettlement,
    ActivityEffect.expense,
    '3950.8800',
    category: 'Elektrik, su, doğalgaz',
  ),
  _a(
    'Gun sonu POS - yolda',
    '2026-09-23',
    ActivityKind.posSale,
    ActivityEffect.income,
    '22650.0000',
    category: 'Satış geliri',
    destination: 'Ziraat Vadesiz',
    canCancel: false,
  ),
  _a(
    'Banka ve POS komisyonu',
    '2026-09-23',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '396.3800',
    category: 'Banka ücreti',
    source: 'Ziraat Vadesiz',
  ),
  _a(
    'Dis hekimi',
    '2026-09-22',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '1250.0000',
    category: 'Sağlık',
    source: 'Sahsi Cuzdan',
    scope: TransactionScope.personal,
  ),
  _a(
    'Toner ve zimba',
    '2026-09-21',
    ActivityKind.cardCharge,
    ActivityEffect.expense,
    '640.0000',
    category: 'Kırtasiye',
    source: 'Ticari Kart',
    group: ActivitySourceGroup.creditCard,
    cancelled: true,
  ),
];
