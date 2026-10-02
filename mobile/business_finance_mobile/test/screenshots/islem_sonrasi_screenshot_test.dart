import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_feed_page.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// İşlem ayrıntısındaki "işlem sonrası" satırları: tahsilatta kişinin açık
/// carisi, borç taksidinde kalan borç, kart harcamasında kalan limit.
void main() {
  Widget page() => ActivityFeedPage(
    repository: _Feed(),
    onShowPlanned: () {},
    now: () => DateTime(2026, 10, 2),
  );

  Future<void> detail(WidgetTester tester, String name, String title) =>
      captureScreen(
        tester,
        name,
        page(),
        selectedTab: 1,
        before: (tester) async => tester.tap(find.text(title).first),
      );

  testWidgets('cari tahsilat ayrıntısı: kişinin kalan alacağı', (tester) async {
    await detail(tester, 'islem-sonrasi-01-cari-tahsilat', 'Ahmet Bakkal');
  }, skip: !screenshotsEnabled);

  testWidgets('borç taksidi ayrıntısı: kalan borç', (tester) async {
    await detail(tester, 'islem-sonrasi-02-borc-taksidi', 'Dükkan kredisi');
  }, skip: !screenshotsEnabled);

  testWidgets('kart harcaması ayrıntısı: kalan limit', (tester) async {
    await detail(tester, 'islem-sonrasi-03-kart-harcamasi', 'Toptancı alımı');
  }, skip: !screenshotsEnabled);
}

FinancialActivity _row(
  String id,
  ActivityKind kind,
  ActivityEffect effect,
  ActivitySourceGroup group,
  String title,
  String amount, {
  String? source,
  String? destination,
  String? category,
  TransactionScope? scope,
}) => FinancialActivity(
  activityId: id,
  kind: kind,
  effect: effect,
  sourceGroup: group,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: '2026-10-02',
  amount: amount,
  currency: 'TRY',
  title: title,
  sourceName: source,
  destinationName: destination,
  categoryName: category,
  canCancel: true,
  supportsAttachments: false,
  scope: scope,
);

final _items = [
  _row(
    'tahsilat',
    ActivityKind.counterpartySettlement,
    ActivityEffect.neutral,
    ActivitySourceGroup.account,
    'Ahmet Bakkal',
    '200.0000',
    source: 'Kasa',
    destination: 'Ahmet Bakkal',
  ),
  _row(
    'taksit',
    ActivityKind.debtPayment,
    ActivityEffect.neutral,
    ActivitySourceGroup.account,
    'Dükkan kredisi',
    '600.0000',
    source: 'Ziraat Vadesiz',
  ),
  _row(
    'kart',
    ActivityKind.cardCharge,
    ActivityEffect.expense,
    ActivitySourceGroup.creditCard,
    'Toptancı alımı',
    '300.0000',
    source: 'Bonus',
    category: 'Mal alımı',
    scope: TransactionScope.business,
  ),
];

class _Feed extends Fake implements ActivityRepositoryContract {
  @override
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async => switch (activity.kind) {
    ActivityKind.counterpartySettlement => const [
      ActivityBalance(
        holder: ActivityBalanceHolder.account,
        name: 'Kasa',
        balance: '1200.0000',
        currency: 'TRY',
        change: ActivityBalanceChange.increased,
      ),
      ActivityBalance(
        holder: ActivityBalanceHolder.counterparty,
        name: 'Ahmet Bakkal',
        balance: '500.0000',
        currency: 'TRY',
        change: ActivityBalanceChange.decreased,
        side: ActivityBalanceSide.receivable,
      ),
    ],
    ActivityKind.debtPayment => const [
      ActivityBalance(
        holder: ActivityBalanceHolder.account,
        name: 'Ziraat Vadesiz',
        balance: '18400.0000',
        currency: 'TRY',
        change: ActivityBalanceChange.decreased,
      ),
      ActivityBalance(
        holder: ActivityBalanceHolder.debt,
        name: 'Dükkan kredisi',
        balance: '6600.0000',
        currency: 'TRY',
        change: ActivityBalanceChange.decreased,
        side: ActivityBalanceSide.payable,
      ),
    ],
    _ => const [
      ActivityBalance(
        holder: ActivityBalanceHolder.card,
        name: 'Bonus',
        balance: '4300.0000',
        currency: 'TRY',
        change: ActivityBalanceChange.increased,
        availableLimit: '15700.0000',
      ),
    ],
  };

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
    asOfDate: '2026-10-02',
    daysAhead: 30,
    totalCount: 0,
    items: [],
  );
}
