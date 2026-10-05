import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/features/day_close/data/day_close_repository.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_controller.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_detail.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_sheets.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Gün sonu paneli (Aşama 06.3 Grup 5, ADR 0019 T1–T2). Tasarım teslimi yok;
/// mevcut dille kuruldu.
void main() {
  Future<void> open(
    WidgetTester tester,
    String name, {
    required _DesignDayClose repository,
    Future<void> Function(WidgetTester tester)? then,
  }) => captureScreen(
    tester,
    name,
    Scaffold(
      appBar: AppBar(title: const Text('Kasa')),
      body: const SizedBox.expand(),
    ),
    withNavBar: false,
    before: (tester) async {
      showDayCloseForm(
        tester.element(find.text('Kasa')),
        DayCloseController(repository),
        initialDate: '2026-10-03',
      );
      await tester.pump();
      await tester.pump(const Duration(seconds: 1));
      await then?.call(tester);
    },
  );

  Future<void> fill(WidgetTester tester) async {
    await tester.enterText(
      find.byKey(const ValueKey('day-close-cash')),
      '3350',
    );
    await tester.enterText(
      find.byKey(const ValueKey('day-close-pos-ziraat')),
      '2680',
    );
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(seconds: 1));
  }

  testWidgets('boş panel', (tester) async {
    await open(tester, 'gunsonu-01-bos', repository: _DesignDayClose());
  }, skip: !screenshotsEnabled);

  testWidgets('dolu panel: üst', (tester) async {
    await open(
      tester,
      'gunsonu-02-dolu-ust',
      repository: _DesignDayClose(),
      then: fill,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('dolu panel: yazılacaklar', (tester) async {
    await open(
      tester,
      'gunsonu-03-dolu-alt',
      repository: _DesignDayClose(),
      then: (tester) async {
        await fill(tester);
        await tester.drag(
          find.byType(SingleChildScrollView).last,
          const Offset(0, -900),
        );
        await tester.pump(const Duration(seconds: 1));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('toplamdan hesaplanan kart', (tester) async {
    await open(
      tester,
      'gunsonu-04-toplamdan',
      repository: _DesignDayClose(records: false),
      then: (tester) async {
        await tester.enterText(
          find.byKey(const ValueKey('day-close-cash')),
          '3350',
        );
        await tester.enterText(
          find.byKey(const ValueKey('day-close-total')),
          '6030',
        );
        await tester.pump(const Duration(milliseconds: 400));
        await tester.pump(const Duration(seconds: 1));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('gün ayrıntısı: ana ve ek gün sonu', (tester) async {
    await captureScreen(
      tester,
      'gunsonu-08-gun-ana-ve-ek',
      Scaffold(
        appBar: AppBar(title: const Text('Kasa')),
        body: const SizedBox.expand(),
      ),
      withNavBar: false,
      before: (tester) async {
        showDayCloseDay(
          tester.element(find.text('Kasa')),
          controller: DayCloseController(
            _DesignDayClose(closes: [_designClose, _designAdditional]),
          ),
          date: '2026-10-03',
        );
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('gün ayrıntısı', (tester) async {
    await captureScreen(
      tester,
      'gunsonu-06-ayrinti',
      Scaffold(
        appBar: AppBar(title: const Text('Kasa')),
        body: const SizedBox.expand(),
      ),
      withNavBar: false,
      before: (tester) async {
        showDayCloseDay(
          tester.element(find.text('Kasa')),
          controller: DayCloseController(
            _DesignDayClose(closes: [_designClose]),
          ),
          date: '2026-10-03',
        );
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('Kasa kartı: açık ve kapalı gün', (tester) async {
    final open = DayCloseController(_DesignDayClose());
    final closed = DayCloseController(_DesignDayClose(closes: [_designClose]));
    await tester.runAsync(() async {
      await open.loadDay('2026-10-03');
      await closed.loadDay('2026-10-03');
    });
    await captureScreen(
      tester,
      'gunsonu-07-kasa-karti',
      Scaffold(
        appBar: AppBar(title: const Text('Kasa')),
        body: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            DayCloseTodayCard(
              controller: open,
              onEnter: ({required additional}) {},
              onOpen: (_) {},
            ),
            const SizedBox(height: 16),
            DayCloseTodayCard(
              controller: closed,
              onEnter: ({required additional}) {},
              onOpen: (_) {},
            ),
          ],
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('gün zaten kapatılmış', (tester) async {
    await open(
      tester,
      'gunsonu-05-kapali',
      repository: _DesignDayClose(closed: true),
    );
  }, skip: !screenshotsEnabled);
}

class _DesignDayClose implements DayCloseRepositoryContract {
  _DesignDayClose({
    this.records = true,
    this.closed = false,
    this.closes = const [],
  });

  final bool records;
  final bool closed;
  final List<DayClose> closes;

  @override
  Future<DayClosePreview> preview(DayCloseInput input) async {
    final cash = input.cashAmount != null;
    final card = input.posAmounts.containsKey('ziraat');
    final fromTotal = cash && !card && input.totalAmount != null;
    final cardStated = card || fromTotal;
    final deduct = records && !input.isAdditional;
    return DayClosePreview(
      date: input.date,
      currency: 'TRY',
      closedBy: closed
          ? const [
              DayCloseSummary(
                id: 'ilk',
                closedOn: '2026-10-03',
                isAdditional: false,
              ),
            ]
          : const [],
      cash: DayCloseCashLine(
        stated: cash,
        enteredAmount: cash ? '3350.0000' : '0.0000',
        isComputed: false,
        deductedAmount: cash && deduct ? '1250.0000' : '0.0000',
        amountToWrite: !cash
            ? '0.0000'
            : deduct
            ? '2100.0000'
            : '3350.0000',
        accountId: 'kasa',
        accountName: 'Dükkan kasası',
        categoryId: 'satis',
        categoryName: 'Satış geliri',
      ),
      posLines: [
        DayClosePosLine(
          posDefinitionId: 'ziraat',
          name: 'Ziraat POS',
          isDefault: true,
          accountName: 'Ziraat işletme',
          stated: cardStated,
          enteredAmount: cardStated ? '2680.0000' : '0.0000',
          isComputed: fromTotal,
          deductedAmount: cardStated && deduct ? '800.0000' : '0.0000',
          amountToWrite: !cardStated
              ? '0.0000'
              : deduct
              ? '1880.0000'
              : '2680.0000',
          commissionAmount: !cardStated
              ? '0.0000'
              : deduct
              ? '32.9000'
              : '46.9000',
          netAmount: !cardStated
              ? '0.0000'
              : deduct
              ? '1847.1000'
              : '2633.1000',
          expectedTransferDate: '2026-10-05',
        ),
        const DayClosePosLine(
          posDefinitionId: 'yemek',
          name: 'Yemek kartı',
          isDefault: false,
          accountName: 'Ziraat işletme',
          stated: false,
          enteredAmount: '0.0000',
          isComputed: false,
          deductedAmount: '0.0000',
          amountToWrite: '0.0000',
          commissionAmount: '0.0000',
          netAmount: '0.0000',
          expectedTransferDate: '2026-10-23',
        ),
      ],
      totalEntered: input.totalAmount,
      totalComputed: cash ? '6030.0000' : '0.0000',
      existingRecords: records
          ? [
              DayCloseExistingRecord(
                kind: 'income',
                id: 'satis-1',
                isCash: true,
                date: '2026-10-03',
                amount: '1250.0000',
                title: 'Toptan satış',
                accountName: 'Dükkan kasası',
                includedByDefault: !input.isAdditional,
                included: !input.isAdditional,
              ),
              const DayCloseExistingRecord(
                kind: 'counterparty-payment',
                id: 'tahsilat-1',
                isCash: true,
                date: '2026-10-03',
                amount: '300.0000',
                title: 'Ahmet Bakkal',
                accountName: 'Dükkan kasası',
                includedByDefault: false,
                included: false,
              ),
              DayCloseExistingRecord(
                kind: 'pos-settlement',
                id: 'pos-1',
                isCash: false,
                date: '2026-10-03',
                amount: '800.0000',
                title: 'Satış geliri',
                posDefinitionId: 'ziraat',
                accountName: 'Ziraat işletme',
                includedByDefault: !input.isAdditional,
                included: !input.isAdditional,
              ),
            ]
          : const [],
      blockerCode: closed && !input.isAdditional
          ? 'day_closes.already_closed'
          : cash || card
          ? null
          : 'day_closes.amounts_required',
    );
  }

  @override
  Future<DayClose> create({
    required String clientRequestId,
    required DayCloseInput input,
  }) async => throw UnimplementedError();

  @override
  Future<List<DayClose>> list({
    required String from,
    required String to,
  }) async => closes;

  @override
  Future<DayCloseDay> day({required String date}) async => DayCloseDay(
    date: date,
    closes: closes,
    outsideRecords: const [
      DayCloseExistingRecord(
        kind: 'counterparty-payment',
        id: 'tahsilat-1',
        isCash: true,
        date: '2026-10-03',
        amount: '300.0000',
        title: 'Ahmet Bakkal',
        accountName: 'Dükkan kasası',
        includedByDefault: false,
        included: false,
      ),
    ],
    cashTotal: closes.length > 1 ? '3750.0000' : '3350.0000',
    cardTotal: closes.isEmpty ? '0.0000' : '2680.0000',
    currency: 'TRY',
  );

  @override
  Future<DayClose> get({required String dayCloseId}) async =>
      throw UnimplementedError();

  @override
  Future<DayClose> revert({required String dayCloseId}) async =>
      throw UnimplementedError();

  @override
  Future<DayCloseOptions> loadOptions() async => const DayCloseOptions(
    cashAccounts: [DataChoice('kasa', 'Dükkan kasası')],
    categories: [DataChoice('satis', 'Satış geliri')],
  );
}

const _designClose = DayClose(
  id: 'gun-sonu',
  closedOn: '2026-10-03',
  isAdditional: false,
  isCancelled: false,
  incomes: [
    DayCloseIncome(
      transactionId: 'gelir',
      accountName: 'Dükkan kasası',
      categoryName: 'Satış geliri',
      amount: '2100.0000',
      date: '2026-10-03',
      isCancelled: false,
    ),
  ],
  settlements: [
    PosSettlementItem(
      id: 'tahsilat',
      accountName: 'Ziraat işletme',
      categoryName: 'Satış geliri',
      grossAmount: '1880.0000',
      commissionAmount: '32.9000',
      netAmount: '1847.1000',
      currency: 'TRY',
      settlementDate: '2026-10-03',
      expectedTransferDate: '2026-10-05',
      isInTransit: true,
      isLate: false,
      posDefinitionName: 'Ziraat POS',
      dayCloseId: 'gun-sonu',
    ),
  ],
  cashAmount: '2100.0000',
  cardGrossAmount: '1880.0000',
  commissionAmount: '32.9000',
  currency: 'TRY',
  countedRecords: [
    DayCloseExistingRecord(
      kind: 'income',
      id: 'satis-1',
      isCash: true,
      date: '2026-10-03',
      amount: '1250.0000',
      title: 'Toptan satış',
      accountName: 'Dükkan kasası',
      includedByDefault: true,
      included: true,
    ),
    DayCloseExistingRecord(
      kind: 'pos-settlement',
      id: 'pos-1',
      isCash: false,
      date: '2026-10-03',
      amount: '800.0000',
      title: 'Satış geliri',
      posDefinitionId: 'ziraat',
      accountName: 'Ziraat işletme',
      includedByDefault: true,
      included: true,
    ),
  ],
  countedCashAmount: '1250.0000',
  countedCardAmount: '800.0000',
);

const _designAdditional = DayClose(
  id: 'ek',
  closedOn: '2026-10-03',
  isAdditional: true,
  isCancelled: false,
  incomes: [
    DayCloseIncome(
      transactionId: 'ek-gelir',
      accountName: 'Dükkan kasası',
      categoryName: 'Satış geliri',
      amount: '400.0000',
      date: '2026-10-03',
      isCancelled: false,
    ),
  ],
  settlements: [],
  cashAmount: '400.0000',
  cardGrossAmount: '0.0000',
  commissionAmount: '0.0000',
  currency: 'TRY',
);
