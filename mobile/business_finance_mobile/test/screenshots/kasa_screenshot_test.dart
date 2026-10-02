import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/widgets/app_adaptive_sheet.dart';
import 'package:business_finance_mobile/features/cash/data/cash_repository.dart';
import 'package:business_finance_mobile/features/cash/presentation/cash_controller.dart';
import 'package:business_finance_mobile/features/cash/presentation/cash_count_view.dart';
import 'package:business_finance_mobile/features/cash/presentation/cash_page.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_controller.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Tasarımın `KasaV4` verisi: dükkan kasası 23.185 beklenen, dün 21.400
/// sayıldı, bugün 2.450 giriş ve 665 çıkış; 23 Eylül POS'u yolda.
void main() {
  late CashCountController cash;

  Widget page({CashCountItem? todayCount}) {
    cash = CashCountController(
      _DesignCash(todayCount),
      clock: () => DateTime(2026, 9, 25, 18),
    );
    return CashPage(
      cashController: cash,
      posController: PosController(_DesignPos()),
    );
  }

  testWidgets('11 kasa', (tester) async {
    await captureScreen(tester, '11-kasa', page(), selectedTab: 2);
  }, skip: !screenshotsEnabled);

  testWidgets('12 sayım · toplam', (tester) async {
    await captureScreen(
      tester,
      '12-sayim-toplam',
      page(),
      selectedTab: 2,
      before: (tester) async {
        await tester.tap(find.text('Sayımı gir'));
        await tester.pump(const Duration(seconds: 1));
        await tester.enterText(find.byType(TextField), '23100');
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('13 sayım · banknot', (tester) async {
    await captureScreen(
      tester,
      '13-sayim-banknot',
      page(),
      selectedTab: 2,
      before: (tester) async {
        AppAdaptiveSheet.show<bool>(
          context: tester.element(find.text('Kasa').first),
          builder: (_) => CashCountSheet(
            controller: cash,
            initialMode: CashCountMode.notes,
            initialNotes: const [100, 25, 8, 7, 4, 2],
            initialCoins: '10',
          ),
        );
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('14 kasa · sayımdan sonra', (tester) async {
    await captureScreen(
      tester,
      '14-kasa-sayimdan-sonra',
      page(
        todayCount: _count(
          '2026-09-25',
          '23100.0000',
          '23185.0000',
          '-85.0000',
        ),
      ),
      selectedTab: 2,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('15 POS detayı', (tester) async {
    await captureScreen(
      tester,
      '15-pos-detay',
      page(),
      selectedTab: 2,
      before: (tester) async {
        await tester.scrollUntilVisible(
          find.text('23 Eylül gün sonu'),
          200,
          scrollable: find.byType(Scrollable).first,
        );
        await tester.tap(find.text('23 Eylül gün sonu'));
      },
    );
  }, skip: !screenshotsEnabled);
}

class _DesignCash implements CashRepositoryContract {
  _DesignCash(this.todayCount);

  final CashCountItem? todayCount;

  @override
  Future<List<CashAccount>> loadCashAccounts() async => const [
    CashAccount(
      id: 'dukkan',
      name: 'Dükkan kasası',
      defaultScope: TransactionScope.business,
    ),
    CashAccount(
      id: 'cuzdan',
      name: 'Şahsi cüzdan',
      defaultScope: TransactionScope.personal,
    ),
  ];

  @override
  Future<CashCountToday> loadToday({required String accountId}) async =>
      accountId == 'dukkan'
      ? CashCountToday(
          accountId: accountId,
          accountName: 'Dükkan kasası',
          expectedBalance: '23185.0000',
          currency: 'TRY',
          count: todayCount,
          previousCount: _history.first,
          todayInflow: '2450.0000',
          todayOutflow: '665.0000',
        )
      : CashCountToday(
          accountId: accountId,
          accountName: 'Şahsi cüzdan',
          expectedBalance: '1840.0000',
          currency: 'TRY',
          todayInflow: '0.0000',
          todayOutflow: '260.0000',
        );

  @override
  Future<List<CashCountItem>> list({required String accountId}) async => [
    ?todayCount,
    ..._history,
  ];

  @override
  Future<CashCountItem> create({
    required String accountId,
    required String countedAmount,
    required String countDate,
    String? scope,
    String? note,
  }) => throw UnimplementedError();

  @override
  Future<List<DataChoice>> loadCategories({required String type}) async =>
      const [];

  @override
  Future<CashCountItem> confirmDifference({
    required String cashCountId,
    required String categoryId,
  }) => throw UnimplementedError();
}

final _history = [
  _count('2026-09-24', '21400.0000', '21520.0000', '-120.0000', saved: true),
  _count('2026-09-23', '19870.0000', '19870.0000', '0.0000'),
  _count('2026-09-22', '18225.0000', '18225.0000', '0.0000'),
  _count('2026-09-20', '16950.0000', '16910.0000', '40.0000', saved: true),
];

CashCountItem _count(
  String date,
  String counted,
  String expected,
  String difference, {
  bool saved = false,
}) => CashCountItem(
  id: date,
  accountId: 'dukkan',
  accountName: 'Dükkan kasası',
  countDate: date,
  countedAmount: counted,
  currency: 'TRY',
  isCancelled: false,
  expectedBalance: expected,
  difference: difference,
  adjustmentTransactionId: saved ? 'adjustment-$date' : null,
);

class _DesignPos implements PosRepositoryContract {
  @override
  Future<PosSettlementList> list({required bool inTransitOnly}) async =>
      const PosSettlementList(
        items: [
          PosSettlementItem(
            id: 'p1',
            accountName: 'Ziraat işletme',
            categoryName: 'Satış',
            grossAmount: '22650.0000',
            commissionAmount: '396.3800',
            netAmount: '22253.6300',
            currency: 'TRY',
            settlementDate: '2026-09-23',
            expectedTransferDate: '2026-09-27',
            isInTransit: true,
            isLate: false,
            scope: TransactionScope.business,
          ),
          PosSettlementItem(
            id: 'p2',
            accountName: 'Ziraat işletme',
            categoryName: 'Satış',
            grossAmount: '12750.0000',
            commissionAmount: '223.1300',
            netAmount: '12526.8700',
            currency: 'TRY',
            settlementDate: '2026-09-09',
            expectedTransferDate: '2026-09-11',
            transferredOn: '2026-09-11',
            isInTransit: false,
            isLate: false,
            scope: TransactionScope.business,
          ),
        ],
        moneyInTransit: '22253.6300',
        inTransitCount: 1,
      );

  @override
  Future<PosOptions> loadOptions() async => const PosOptions(
    accounts: [],
    incomeCategories: [],
    expenseCategories: [],
  );

  @override
  Future<List<PosDefinitionItem>> listDefinitions() async => const [];

  @override
  Future<void> saveDefinition(
    PosDefinitionInput input, {
    String? definitionId,
  }) async {}

  @override
  Future<void> setDefinitionActive({
    required String definitionId,
    required bool isActive,
  }) async {}

  @override
  Future<void> deleteDefinition({required String definitionId}) async {}

  @override
  Future<void> setDefaultDefinition({required String definitionId}) async {}

  @override
  Future<PosPreview> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  }) async => throw UnimplementedError();

  @override
  Future<void> create(Map<String, Object?> input) async {}

  @override
  Future<PosDepositPreview> previewDeposit({
    required List<String> settlementIds,
    String? depositedAmount,
  }) async => throw UnimplementedError();

  @override
  Future<PosDeposit> createDeposit({
    required String clientRequestId,
    required List<String> settlementIds,
    required String depositedAmount,
    required String depositDate,
    String? deductionCategoryId,
  }) async => throw UnimplementedError();

  @override
  Future<PosDeposit> getDeposit({required String depositId}) async =>
      throw UnimplementedError();

  @override
  Future<PosDeposit> revertDeposit({required String depositId}) async =>
      throw UnimplementedError();

  @override
  Future<void> cancel({required String settlementId}) async {}
}
