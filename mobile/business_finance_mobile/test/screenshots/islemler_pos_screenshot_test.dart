import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_feed_page.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// İşlemler'de POS satışı ve yatışı (2 Ekim 2026 emülatör turu): komisyon ve
/// kesinti ayrı satır değil, bağlı olduğu kaydın parçasıdır. Veri, kullanıcının
/// turda gördüğü 1–2 Ekim kayıtlarıdır.
void main() {
  Widget page() => ActivityFeedPage(
    repository: _Feed(),
    posRepository: _Pos(),
    onShowPlanned: () {},
    now: () => DateTime(2026, 10, 2),
  );

  testWidgets('işlemler: POS satışı ve yatışı', (tester) async {
    await captureScreen(
      tester,
      'islemler-pos-01-liste',
      page(),
      selectedTab: 1,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('işlemler: 1 Ekim satışları', (tester) async {
    await captureScreen(
      tester,
      'islemler-pos-02-liste-1-ekim',
      page(),
      selectedTab: 1,
      before: (tester) async {
        await tester.drag(find.byType(Scrollable).last, const Offset(0, -330));
      },
    );
  }, skip: !screenshotsEnabled);

  Future<void> detail(WidgetTester tester, String name, String title) =>
      captureScreen(
        tester,
        name,
        page(),
        selectedTab: 1,
        before: (tester) async => tester.tap(find.text(title).first),
      );

  testWidgets('POS satışı ayrıntısı: hesaba geçti', (tester) async {
    await detail(
      tester,
      'islemler-pos-03-satis-detay',
      'Uc kisilik aksam yemegi',
    );
  }, skip: !screenshotsEnabled);

  testWidgets('POS satışı ayrıntısı: yolda', (tester) async {
    await detail(
      tester,
      'islemler-pos-04-satis-detay-yolda',
      'Dugun pastasi siparisi',
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yatış ayrıntısı', (tester) async {
    await detail(tester, 'islemler-pos-05-yatis-detay', 'POS yatışı');
  }, skip: !screenshotsEnabled);
}

FinancialActivity _sale(
  String id,
  String title,
  String date,
  String amount, {
  required String pos,
  required String account,
  required String fee,
  required String net,
  String? description,
  String? transferredOn,
  String expected = '2026-10-05',
  bool cancelled = false,
}) => FinancialActivity(
  activityId: id,
  kind: ActivityKind.posSale,
  effect: ActivityEffect.income,
  sourceGroup: ActivitySourceGroup.pos,
  origin: ActivityOrigin.manual,
  status: cancelled ? ActivityStatus.cancelled : ActivityStatus.realized,
  activityDate: date,
  amount: amount,
  currency: 'TRY',
  title: title,
  description: description,
  categoryName: 'Satış geliri',
  destinationName: account,
  canCancel: false,
  supportsAttachments: false,
  scope: TransactionScope.business,
  channelName: pos,
  feeAmount: fee,
  netAmount: net,
  expectedTransferDate: expected,
  transferredOn: transferredOn,
);

FinancialActivity _deposit(
  String id,
  String date,
  String amount, {
  required String account,
  String? pos,
  String? fee,
  int count = 1,
}) => FinancialActivity(
  activityId: id,
  kind: ActivityKind.posDeposit,
  effect: ActivityEffect.neutral,
  sourceGroup: ActivitySourceGroup.pos,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: date,
  amount: amount,
  currency: 'TRY',
  title: 'POS yatışı',
  destinationName: account,
  canCancel: false,
  supportsAttachments: false,
  channelName: pos,
  feeAmount: fee,
  settlementCount: count,
);

/// Giriş sırasına göre, en yeni üstte.
final _items = [
  _deposit(
    'yatis-yemek',
    '2026-10-02',
    '1402.5000',
    account: 'Ziraat Vadesiz',
    pos: 'Yemek kartı',
  ),
  _deposit(
    'yatis-birikim',
    '2026-10-02',
    '1176.0000',
    account: 'Birikim Hesabı',
  ),
  _sale(
    'aksam',
    'Uc kisilik aksam yemegi',
    '2026-10-02',
    '1500.0000',
    description: 'Uc kisilik aksam yemegi',
    pos: 'Yemek kartı',
    account: 'Ziraat Vadesiz',
    fee: '97.5000',
    net: '1402.5000',
    transferredOn: '2026-10-02',
  ),
  _sale(
    'iptal',
    'Satış geliri',
    '2026-10-02',
    '1250.0000',
    pos: 'Ziraat POS',
    account: 'Ziraat Vadesiz',
    fee: '37.5000',
    net: '1212.5000',
    cancelled: true,
  ),
  // Kesinti örneği: banka 2.364 yerine 2.350 yatırdı.
  _deposit(
    'yatis-kesinti',
    '2026-10-01',
    '2350.0000',
    account: 'Ziraat Vadesiz',
    fee: '14.0000',
    count: 2,
  ),
  _sale(
    'pasta',
    'Dugun pastasi siparisi',
    '2026-10-01',
    '5200.0000',
    description: 'Dugun pastasi siparisi',
    pos: 'Ziraat POS',
    account: 'Ziraat Vadesiz',
    fee: '78.0000',
    net: '5122.0000',
  ),
  _sale(
    'iptal-2',
    'Satış geliri',
    '2026-10-01',
    '1000.0000',
    pos: 'Ziraat POS',
    account: 'Ziraat Vadesiz',
    fee: '17.9000',
    net: '982.1000',
    cancelled: true,
  ),
  _sale(
    'gun-sonu',
    'Satış geliri',
    '2026-10-01',
    '2400.0000',
    pos: 'Ziraat POS',
    account: 'Ziraat Vadesiz',
    fee: '36.0000',
    net: '2364.0000',
    transferredOn: '2026-10-01',
  ),
  _sale(
    'birikim',
    'Satış geliri',
    '2026-10-01',
    '1200.0000',
    pos: 'Garanti POS',
    account: 'Birikim Hesabı',
    fee: '24.0000',
    net: '1176.0000',
    transferredOn: '2026-10-02',
  ),
  _sale(
    'iptal-3',
    'Satış geliri',
    '2026-10-01',
    '1000.0000',
    pos: 'Ziraat POS',
    account: 'Ziraat Vadesiz',
    fee: '17.9000',
    net: '982.1000',
    cancelled: true,
  ),
];

class _Feed extends Fake implements ActivityRepositoryContract {
  @override
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async => activity.kind == ActivityKind.posSale
      ? const [
          ActivityBalance(
            isCard: false,
            name: 'Ziraat Vadesiz',
            balance: '18400.0000',
            currency: 'TRY',
            change: ActivityBalanceChange.unchanged,
          ),
        ]
      : const [];

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

class _Pos extends Fake implements PosRepositoryContract {
  @override
  Future<PosDeposit> getDeposit({required String depositId}) async =>
      const PosDeposit(
        id: 'yatis-yemek',
        accountName: 'Ziraat Vadesiz',
        depositDate: '2026-10-02',
        expectedAmount: '1402.5000',
        grossAmount: '1500.0000',
        commissionAmount: '97.5000',
        balanceAfter: '19802.5000',
        depositedAmount: '1402.5000',
        deductionAmount: '0.0000',
        currency: 'TRY',
        isCancelled: false,
        settlements: [
          PosSettlementItem(
            id: 'aksam',
            accountId: 'ziraat',
            accountName: 'Ziraat Vadesiz',
            categoryName: 'Satış geliri',
            grossAmount: '1500.0000',
            commissionAmount: '97.5000',
            netAmount: '1402.5000',
            currency: 'TRY',
            settlementDate: '2026-10-02',
            expectedTransferDate: '2026-10-22',
            isInTransit: false,
            isLate: false,
            description: 'Uc kisilik aksam yemegi',
            posDefinitionName: 'Yemek kartı',
          ),
        ],
      );
}
