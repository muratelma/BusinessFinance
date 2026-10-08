import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_controller.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_definitions_page.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_deposit_sheets.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_settlements_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// POS tanımı ekranları (Aşama 06.3 Grup 4): POS'larım, tanım formu ve
/// tanımdan dolan tahsilat formu; yatış panelleri (Grup 5): "Hesaba geçenleri
/// işaretle" ve yatış ayrıntısı. Tasarım teslimi yok; mevcut dille kuruldu.
void main() {
  PosController controller() => PosController(_DesignPos());

  testWidgets("POS'larım", (tester) async {
    await captureScreen(
      tester,
      'pos-01-poslarim',
      PosDefinitionsPage(controller: controller()),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('POS tanımı formu', (tester) async {
    await captureScreen(
      tester,
      'pos-02-tanim-formu',
      PosDefinitionFormPage(controller: controller()),
      withNavBar: false,
      pushed: true,
      before: (tester) async {
        await tester.enterText(find.byType(TextFormField).at(0), 'Ziraat POS');
        await tester.enterText(find.byType(TextFormField).at(1), '1,79');
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('tanımdan dolan tahsilat formu', (tester) async {
    final pos = controller();
    await captureScreen(
      tester,
      'pos-03-tahsilat-formu',
      Scaffold(
        appBar: AppBar(title: const Text('Kasa')),
        body: const SizedBox.expand(),
      ),
      withNavBar: false,
      before: (tester) async {
        showPosSettlementForm(tester.element(find.text('Kasa')), pos);
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
        await tester.enterText(find.byType(TextFormField).first, '22650');
        await tester.pump(const Duration(milliseconds: 400));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('hesaba geçenleri işaretle: eksik yatan tutar', (tester) async {
    final pos = controller();
    await pos.load();
    await captureScreen(
      tester,
      'pos-05-yatis-formu',
      Scaffold(
        appBar: AppBar(title: const Text('Kasa')),
        body: const SizedBox.expand(),
      ),
      withNavBar: false,
      before: (tester) async {
        showPosDepositForm(tester.element(find.text('Kasa')), pos);
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
        await tester.enterText(
          find.widgetWithText(TextFormField, 'Yatan tutar'),
          '40600',
        );
        await tester.pump(const Duration(milliseconds: 400));
        await tester.pump(const Duration(seconds: 1));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yatış ayrıntısı', (tester) async {
    await captureScreen(
      tester,
      'pos-06-yatis-ayrintisi',
      Scaffold(
        appBar: AppBar(title: const Text('Kasa')),
        body: const SizedBox.expand(),
      ),
      withNavBar: false,
      before: (tester) async {
        showPosDepositDetail(
          tester.element(find.text('Kasa')),
          repository: _DesignPos(),
          depositId: 'yatis',
        );
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('POS seçimi açık', (tester) async {
    final pos = controller();
    await captureScreen(
      tester,
      'pos-04-pos-secimi',
      Scaffold(
        appBar: AppBar(title: const Text('Kasa')),
        body: const SizedBox.expand(),
      ),
      withNavBar: false,
      before: (tester) async {
        showPosSettlementForm(tester.element(find.text('Kasa')), pos);
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
        await tester.tap(find.text('Ziraat POS'));
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
      },
    );
  }, skip: !screenshotsEnabled);
}

class _DesignPos implements PosRepositoryContract {
  static const _definitions = [
    PosDefinitionItem(
      id: 'ziraat',
      name: 'Ziraat POS',
      accountId: 'ziraat-hesap',
      accountName: 'Ziraat işletme',
      salesCategoryId: 'satis',
      salesCategoryName: 'Satış geliri',
      commissionCategoryId: 'komisyon',
      commissionCategoryName: 'Banka ve POS komisyonu',
      commissionRate: '0.0175',
      transferDays: 1,
      businessDaysOnly: true,
      isActive: true,
    ),
    PosDefinitionItem(
      id: 'yemek',
      name: 'Yemek kartı',
      accountId: 'ziraat-hesap',
      accountName: 'Ziraat işletme',
      salesCategoryId: 'satis',
      salesCategoryName: 'Satış geliri',
      commissionCategoryId: 'komisyon',
      commissionCategoryName: 'Banka ve POS komisyonu',
      commissionRate: '0.0650',
      transferDays: 20,
      businessDaysOnly: false,
      isActive: true,
    ),
    PosDefinitionItem(
      id: 'eski',
      name: 'Eski banka POS',
      accountId: 'ziraat-hesap',
      accountName: 'Ziraat işletme',
      salesCategoryId: 'satis',
      salesCategoryName: 'Satış geliri',
      commissionRate: '0.0000',
      transferDays: 0,
      businessDaysOnly: true,
      isActive: false,
    ),
  ];

  @override
  Future<List<PosDefinitionItem>> listDefinitions() async => _definitions;

  @override
  Future<PosOptions> loadOptions() async => const PosOptions(
    accounts: [
      DataChoice(
        'ziraat-hesap',
        'Ziraat işletme',
        defaultScope: TransactionScope.business,
      ),
    ],
    incomeCategories: [DataChoice('satis', 'Satış geliri')],
    expenseCategories: [
      DataChoice('kira', 'Kira'),
      DataChoice('komisyon', 'Banka ve POS komisyonu'),
    ],
  );

  @override
  Future<PosPreview> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  }) async => const PosPreview(
    commissionAmount: '396.3800',
    netAmount: '22253.6200',
    currency: 'TRY',
    expectedTransferDate: '2026-09-28',
  );

  static const _inTransit = [
    PosSettlementItem(
      id: 'carsamba',
      accountId: 'ziraat-hesap',
      accountName: 'Ziraat işletme',
      categoryName: 'Satış geliri',
      grossAmount: '22650.0000',
      commissionAmount: '396.3800',
      netAmount: '22253.6200',
      currency: 'TRY',
      settlementDate: '2026-09-25',
      expectedTransferDate: '2026-09-28',
      isInTransit: true,
      isLate: true,
      posDefinitionName: 'Ziraat POS',
    ),
    PosSettlementItem(
      id: 'persembe',
      accountId: 'ziraat-hesap',
      accountName: 'Ziraat işletme',
      categoryName: 'Satış geliri',
      grossAmount: '18738.0000',
      commissionAmount: '328.0000',
      netAmount: '18410.0000',
      currency: 'TRY',
      settlementDate: '2026-09-26',
      expectedTransferDate: '2026-09-29',
      isInTransit: true,
      isLate: false,
      posDefinitionName: 'Ziraat POS',
    ),
  ];

  @override
  Future<PosSettlementList> list({
    required bool inTransitOnly,
    String? from,
    String? to,
  }) async => const PosSettlementList(
    items: _inTransit,
    moneyInTransit: '40663.6200',
    inTransitCount: 2,
  );

  @override
  Future<void> create(Map<String, Object?> input) async {}

  @override
  Future<PosDepositPreview> previewDeposit({
    required List<String> settlementIds,
    String? depositedAmount,
  }) async => PosDepositPreview(
    accountName: 'Ziraat işletme',
    settlementCount: 2,
    expectedAmount: '40663.6200',
    depositedAmount: depositedAmount ?? '40663.6200',
    deductionAmount: depositedAmount == null ? '0.0000' : '63.6200',
    exceedsExpected: false,
    deductionCategoryId: 'komisyon',
    deductionCategoryName: 'Banka ve POS komisyonu',
    currency: 'TRY',
    earliestDepositDate: '2026-09-26',
  );

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
      const PosDeposit(
        id: 'yatis',
        accountName: 'Ziraat işletme',
        depositDate: '2026-09-29',
        expectedAmount: '40663.6200',
        depositedAmount: '40600.0000',
        deductionAmount: '63.6200',
        deductionCategoryName: 'Banka ve POS komisyonu',
        currency: 'TRY',
        isCancelled: false,
        settlements: _inTransit,
      );

  @override
  Future<PosDeposit> revertDeposit({required String depositId}) async =>
      throw UnimplementedError();

  @override
  Future<void> cancel({required String settlementId}) async {}

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
}
