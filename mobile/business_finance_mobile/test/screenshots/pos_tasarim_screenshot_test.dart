import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_all_settlements_page.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_controller.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_settlements_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// `POS tahsilatları` bölümünün ve `Tüm tahsilatlar` sayfasının tasarım
/// varyantları (Aşama 06.3 K11). Karar verilince seçilmeyenler kalkar.
void main() {
  const today = '2026-10-05';

  Future<void> section(
    WidgetTester tester,
    String name,
    PosSectionLayout layout,
  ) async {
    final controller = PosController(_Pos());
    await captureScreen(
      tester,
      name,
      Scaffold(
        body: ListView(
          padding: const EdgeInsets.all(AppSpacing.medium),
          children: [
            PosSection(controller: controller, layout: layout, today: today),
          ],
        ),
      ),
      withNavBar: false,
    );
  }

  Future<void> page(
    WidgetTester tester,
    String name,
    PosPageLayout layout,
  ) async {
    await captureScreen(
      tester,
      name,
      PosAllSettlementsPage(
        controller: PosController(_Pos()),
        layout: layout,
        now: () => DateTime(2026, 10, 5),
      ),
      withNavBar: false,
    );
  }

  testWidgets('kasa 1 · düz liste', (tester) async {
    await section(tester, 'kasa-1-duz-liste', PosSectionLayout.list);
  }, skip: !screenshotsEnabled);

  testWidgets('kasa 2 · güne göre', (tester) async {
    await section(tester, 'kasa-2-gune-gore', PosSectionLayout.byDay);
  }, skip: !screenshotsEnabled);

  testWidgets('kasa 3 · yalnız geciken', (tester) async {
    await section(tester, 'kasa-3-yalniz-geciken', PosSectionLayout.minimal);
  }, skip: !screenshotsEnabled);

  testWidgets('sayfa A · iki süzgeç satırı', (tester) async {
    await page(tester, 'sayfa-a-iki-suzgec', PosPageLayout.a);
  }, skip: !screenshotsEnabled);

  testWidgets('sayfa B · işlemler gibi', (tester) async {
    await page(tester, 'sayfa-b-islemler-gibi', PosPageLayout.b);
  }, skip: !screenshotsEnabled);

  testWidgets('sayfa C · ray', (tester) async {
    await page(tester, 'sayfa-c-ray', PosPageLayout.c);
  }, skip: !screenshotsEnabled);
}

PosSettlementItem _item(
  String id,
  String title, {
  required String sold,
  required String expected,
  required String gross,
  required String net,
  required String pos,
  bool late = false,
  String? deposit,
  String? depositedOn,
  String? payer,
}) => PosSettlementItem(
  id: id,
  accountName: 'Ziraat Vadesiz',
  categoryName: payer == null ? 'Satış geliri' : null,
  grossAmount: gross,
  commissionAmount: '0.0000',
  netAmount: net,
  currency: 'TRY',
  settlementDate: sold,
  expectedTransferDate: expected,
  isInTransit: deposit == null,
  isLate: late,
  transferredOn: depositedOn,
  description: title,
  posDefinitionName: pos,
  posDepositId: deposit,
  kind: payer == null ? PosSettlementKind.sale : PosSettlementKind.collection,
  counterpartyName: payer,
);

final _items = [
  _item(
    'a',
    'Gecikmiş satış',
    sold: '2026-10-01',
    expected: '2026-10-02',
    gross: '1000.0000',
    net: '980.0000',
    pos: 'Garanti',
    late: true,
  ),
  _item(
    'b',
    '4 Ekim gün sonu',
    sold: '2026-10-04',
    expected: '2026-10-05',
    gross: '3200.0000',
    net: '3104.0000',
    pos: 'Ziraat POS',
  ),
  _item(
    'c',
    '4 Ekim gün sonu',
    sold: '2026-10-04',
    expected: '2026-10-05',
    gross: '800.0000',
    net: '784.0000',
    pos: 'Garanti',
  ),
  _item(
    'd',
    '5 Ekim gün sonu',
    sold: '2026-10-05',
    expected: '2026-10-06',
    gross: '2000.0000',
    net: '1960.0000',
    pos: 'Garanti',
  ),
  _item(
    'e',
    'Ahmet Bakkal',
    sold: '2026-10-05',
    expected: '2026-10-06',
    gross: '900.0000',
    net: '882.0000',
    pos: 'Garanti',
    payer: 'Ahmet Bakkal',
  ),
  _item(
    'f',
    'Öğle servisi',
    sold: '2026-10-02',
    expected: '2026-10-22',
    gross: '2000.0000',
    net: '1870.0000',
    pos: 'Yemek Kartı',
  ),
  _item(
    'g',
    '1 Ekim gün sonu',
    sold: '2026-10-01',
    expected: '2026-10-21',
    gross: '1200.0000',
    net: '1122.0000',
    pos: 'Yemek Kartı',
  ),
  _item(
    'h',
    'Okul Kooperatifi',
    sold: '2026-10-05',
    expected: '2026-10-06',
    gross: '1500.0000',
    net: '1455.0000',
    pos: 'Ziraat POS',
    deposit: 'y2',
    depositedOn: '2026-10-05',
    payer: 'Okul Kooperatifi',
  ),
  _item(
    'i',
    'Akşam servisi',
    sold: '2026-10-02',
    expected: '2026-10-05',
    gross: '1500.0000',
    net: '1455.0000',
    pos: 'Ziraat POS',
    deposit: 'y2',
    depositedOn: '2026-10-05',
  ),
  _item(
    'j',
    '1 Ekim gün sonu',
    sold: '2026-10-01',
    expected: '2026-10-02',
    gross: '1000.0000',
    net: '970.0000',
    pos: 'Ziraat POS',
    deposit: 'y1',
    depositedOn: '2026-10-02',
  ),
];

class _Pos implements PosRepositoryContract {
  @override
  Future<PosSettlementList> list({
    required bool inTransitOnly,
    String? from,
    String? to,
  }) async => PosSettlementList(
    items: _items,
    moneyInTransit: '10702.0000',
    inTransitCount: 7,
  );

  @override
  Future<PosDeposit> getDeposit({required String depositId}) async =>
      const PosDeposit(
        id: 'y2',
        accountName: 'Ziraat Vadesiz',
        depositDate: '2026-10-05',
        expectedAmount: '2910.0000',
        depositedAmount: '2910.0000',
        deductionAmount: '0.0000',
        currency: 'TRY',
        isCancelled: false,
        settlements: [],
      );

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}
