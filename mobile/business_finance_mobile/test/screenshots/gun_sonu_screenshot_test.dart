import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';
import 'package:business_finance_mobile/features/day_close/data/day_close_repository.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_controller.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_detail.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_sheets.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../helpers/day_close_handoff.dart';
import 'screenshot_harness.dart';

/// Teslimin sekiz hâli gerçek gün sonu panelinden çizilir.
void main() {
  Future<void> open(
    WidgetTester tester,
    String name, {
    required DayCloseRepositoryContract repository,
    Size frame = designFrame,
    Future<void> Function(WidgetTester tester)? then,
  }) => captureScreen(
    tester,
    name,
    const Scaffold(body: SizedBox.expand()),
    withNavBar: false,
    frame: frame,
    before: (tester) async {
      final controller = DayCloseController(repository);
      addTearDown(controller.dispose);
      showDayCloseForm(
        tester.element(find.byType(Scaffold).first),
        controller,
        initialDate: '2026-10-10',
      );
      await tester.pumpAndSettle();
      await then?.call(tester);
      FocusManager.instance.primaryFocus?.unfocus();
      await tester.pumpAndSettle();
      // Tam panel karesi: gerçek bileşen, 412 dp genişlik, üstte 56 dp perde.
      final height = tester.getSize(find.byType(AppFormSheet<bool>)).height;
      tester.view.physicalSize = Size(412, height + 56 + 48) * 2;
      await tester.pumpAndSettle();
    },
  );

  Future<void> type(WidgetTester tester, String key, String value) async {
    await tester.enterText(find.byKey(ValueKey(key)), value);
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pumpAndSettle();
  }

  Future<void> tap(WidgetTester tester, Finder finder) async {
    await tester.ensureVisible(finder);
    await tester.tap(finder);
    await tester.pumpAndSettle();
  }

  const names = [
    '01-g1-cevap-bekleyen',
    '02-g2-cevaplanmis-ve-hesap',
    '03-g3-bazilari-dahil',
    '04-g4-satis-ve-tahsilat-birlikte',
    '05-g5-toplam-farki',
    '06-g4b-alacak-faturasi-ve-tahsilat',
    '07-g4c-ayni-gun-iki-kisi',
    '08-g6-nakit-bos-kartli-kayit',
  ];
  for (var index = 0; index < names.length; index++) {
    final frameNumber = index + 1;
    testWidgets('teslim ${names[index]}', (tester) async {
      await open(
        tester,
        'gunsonu-${names[index]}',
        repository: HandoffDayCloseRepository(frameNumber),
        frame: Size(
          412,
          frameNumber == 7
              ? 1900
              : frameNumber == 4 || frameNumber == 6
              ? 1600
              : frameNumber == 5 || frameNumber == 8
              ? 1050
              : 1400,
        ),
        then: (tester) async {
          if (frameNumber == 5 || frameNumber == 8) {
            await type(
              tester,
              'day-close-pos-ziraat',
              frameNumber == 5 ? '500' : '1300',
            );
            if (frameNumber == 5) await type(tester, 'day-close-total', '1800');
            return;
          }
          await type(
            tester,
            'day-close-cash',
            frameNumber <= 3
                ? '1670'
                : frameNumber == 7
                ? '2400'
                : '1600',
          );
          if (frameNumber == 1) return;
          await tap(
            tester,
            find.text(frameNumber <= 3 ? 'Hiçbiri' : 'Hepsi içinde'),
          );
          if (frameNumber <= 3) {
            await tap(
              tester,
              find.byKey(
                const ValueKey('day-close-record-counterparty-payment/ahmet'),
              ),
            );
            if (frameNumber == 2) {
              await tap(
                tester,
                find.byKey(
                  const ValueKey(
                    'day-close-record-counterparty-payment/mehmet',
                  ),
                ),
              );
            }
          } else if (frameNumber == 4) {
            await tap(
              tester,
              find.byKey(const ValueKey('day-close-overlap-mehmet-partial')),
            );
            await type(tester, 'day-close-shared-mehmet', '200');
          } else if (frameNumber == 6) {
            await tap(
              tester,
              find.byKey(const ValueKey('day-close-overlap-invoice-inside')),
            );
          } else {
            await tap(
              tester,
              find.byKey(const ValueKey('day-close-overlap-mehmet-separate')),
            );
            await tap(
              tester,
              find.byKey(const ValueKey('day-close-overlap-ahmet-inside')),
            );
          }
        },
      );
    }, skip: !screenshotsEnabled);
  }
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

  // Veresiye satış ve tahsilatı birlikte sayılmış gün: ikisinde de görünen
  // tutar ayrı satırdır; onsuz sayılanlar günün toplamını tutmaz
  // (1.000 yazıldı + 500 + 300 − 200 = 1.600).
  testWidgets(
    'gün ayrıntısı: satışta da tahsilatta da görünen tutar',
    (tester) async {
      await captureScreen(
        tester,
        'gunsonu-09-ayrinti-ikisinde-de',
        Scaffold(
          appBar: AppBar(title: const Text('Kasa')),
          body: const SizedBox.expand(),
        ),
        withNavBar: false,
        before: (tester) async {
          showDayCloseDay(
            tester.element(find.text('Kasa')),
            controller: DayCloseController(
              _DesignDayClose(
                closes: [_designSharedClose],
                cashTotal: '1600.0000',
                cardTotal: '0.0000',
                outside: false,
              ),
            ),
            date: '2026-10-10',
          );
          await tester.pump();
          await tester.pump(const Duration(seconds: 1));
        },
      );
    },
    skip: !screenshotsEnabled,
  );

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
    this.closed = false,
    this.closes = const [],
    this.cashTotal,
    this.cardTotal,
    this.outside = true,
  });

  final bool records = true;
  final bool closed;
  final List<DayClose> closes;

  /// Günün toplamı; verilmezse ana örneğin tutarları.
  final String? cashTotal;
  final String? cardTotal;

  /// Gün sonunun dışında kalan tahsilat gösterilsin mi.
  final bool outside;

  @override
  Future<DayClosePreview> preview(DayCloseInput input) async {
    final cash = input.cashAmount != null;
    final card = input.posAmounts.containsKey('ziraat');
    final cardStated = card;
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
    outsideRecords: [
      if (outside)
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
    ],
    cashTotal: cashTotal ?? (closes.length > 1 ? '3750.0000' : '3350.0000'),
    cardTotal: cardTotal ?? (closes.isEmpty ? '0.0000' : '2680.0000'),
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

/// Brif 4'ün G4 örneği: Mehmet Usta'nın ₺500 veresiye satışı ve ₺300
/// tahsilatı yazılan ₺1.600 nakdin içinde, ₺200'ü ikisinde de var.
const _designSharedClose = DayClose(
  id: 'gun-sonu-ortak',
  closedOn: '2026-10-10',
  isAdditional: false,
  isCancelled: false,
  incomes: [
    DayCloseIncome(
      transactionId: 'gelir-ortak',
      accountName: 'Dükkan kasası',
      categoryName: 'Satış geliri',
      amount: '1000.0000',
      date: '2026-10-10',
      isCancelled: false,
    ),
  ],
  settlements: [],
  cashAmount: '1000.0000',
  cardGrossAmount: '0.0000',
  commissionAmount: '0.0000',
  currency: 'TRY',
  countedRecords: [
    DayCloseExistingRecord(
      kind: 'counterparty-charge',
      id: 'veresiye-1',
      isCash: true,
      date: '2026-10-10',
      amount: '500.0000',
      title: 'Mehmet Usta',
      accountName: '',
      includedByDefault: false,
      included: true,
      requiresAnswer: true,
      groupId: 'mehmet',
      groupName: 'Mehmet Usta',
    ),
    DayCloseExistingRecord(
      kind: 'counterparty-payment',
      id: 'tahsilat-2',
      isCash: true,
      date: '2026-10-10',
      amount: '300.0000',
      title: 'Mehmet Usta',
      accountName: 'Dükkan kasası',
      includedByDefault: false,
      included: true,
      requiresAnswer: true,
      groupId: 'mehmet',
      groupName: 'Mehmet Usta',
    ),
  ],
  countedCashAmount: '600.0000',
  overlaps: [
    DayCloseOverlapGroup(
      groupId: 'mehmet',
      name: 'Mehmet Usta',
      isInvoice: false,
      salesAmount: '500.0000',
      collectionsAmount: '300.0000',
      maximumOverlap: '300.0000',
      separateAmount: '800.0000',
      insideAmount: '500.0000',
      collectionsLarger: false,
      overlapAmount: '200.0000',
      deductedAmount: '600.0000',
    ),
  ],
);

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
