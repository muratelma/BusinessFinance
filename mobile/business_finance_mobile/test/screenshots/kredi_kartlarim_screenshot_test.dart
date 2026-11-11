import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/cards/presentation/finance_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Kredi kartlarım: gerçek sayfa, örnek veriyle. Üstte yan yana kayan kartlar,
/// altında seçili kartın işlemleri, limiti, ekstresi, taksitleri ve
/// hareketleri.
void main() {
  testWidgets('kredi kartlarım · üst', (tester) async {
    await captureScreen(
      tester,
      'kredi-kartlarim-ust',
      FinancePage(repository: _Cards()),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);

  Future<void> shot(
    WidgetTester tester,
    String name,
    Future<void> Function(WidgetTester tester) before,
  ) => captureScreen(
    tester,
    name,
    FinancePage(repository: _Cards()),
    withNavBar: false,
    pushed: true,
    before: before,
  );

  testWidgets('kredi kartlarım · alt', (tester) async {
    await shot(tester, 'kredi-kartlarim-alt', (tester) async {
      await tester.drag(find.byType(ListView).first, const Offset(0, -520));
    });
  }, skip: !screenshotsEnabled);

  testWidgets('kredi kartlarım · taksit paneli', (tester) async {
    await shot(tester, 'kredi-kartlarim-taksit', (tester) async {
      await tester.drag(find.byType(ListView).first, const Offset(0, -300));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Buzdolabı'));
      await tester.pumpAndSettle();
    });
  }, skip: !screenshotsEnabled);

  testWidgets('kredi kartlarım · ekstre paneli', (tester) async {
    await shot(tester, 'kredi-kartlarim-ekstre', (tester) async {
      await tester.tap(find.text('Ekstre'));
      await tester.pumpAndSettle();
    });
  }, skip: !screenshotsEnabled);

  testWidgets('kredi kartlarım · hareketler sayfası', (tester) async {
    await shot(tester, 'kredi-kartlarim-hareketler', (tester) async {
      await tester.drag(find.byType(ListView).first, const Offset(0, -520));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Tümü'));
      await tester.pumpAndSettle();
    });
  }, skip: !screenshotsEnabled);

  testWidgets('kredi kartlarım · filtre paneli', (tester) async {
    await shot(tester, 'kredi-kartlarim-filtre', (tester) async {
      await tester.drag(find.byType(ListView).first, const Offset(0, -520));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Tümü'));
      await tester.pumpAndSettle();
      await tester.tap(find.byTooltip('Filtrele'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Ödemeler'));
      await tester.pumpAndSettle();
    });
  }, skip: !screenshotsEnabled);

  testWidgets('kredi kartlarım · boş', (tester) async {
    await captureScreen(
      tester,
      'kredi-kartlarim-bos',
      FinancePage(repository: _Cards(empty: true)),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);
}

class _Cards extends Fake implements FinanceRepositoryContract {
  _Cards({this.empty = false});

  final bool empty;

  static const _bonus = CreditCardItem(
    id: 'bonus',
    name: 'Bonus Kart',
    limit: '50000.0000',
    currentDebt: '12450.0000',
    availableLimit: '37550.0000',
    currency: 'TRY',
    statementClosingDay: 15,
    paymentDueDay: 25,
    minimumPaymentRate: '20.0000',
    isActive: true,
  );

  static const _personal = CreditCardItem(
    id: 'sahsi',
    name: 'Şahsi kart',
    limit: '30000.0000',
    currentDebt: '4200.0000',
    availableLimit: '25800.0000',
    currency: 'TRY',
    statementClosingDay: 3,
    paymentDueDay: 13,
    minimumPaymentRate: '20.0000',
    isActive: true,
  );

  @override
  Future<FinanceSnapshot> load({
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async => FinanceSnapshot(
    transfers: const [],
    cards: empty ? const [] : const [_bonus, _personal],
    plans: empty
        ? const []
        : const [
            InstallmentPlanModel(
              id: 'p1',
              creditCardId: 'bonus',
              totalAmount: '18000.0000',
              currency: 'TRY',
              description: 'Buzdolabı',
              items: [
                InstallmentItemModel(
                  sequence: 1,
                  amount: '3000.0000',
                  currency: 'TRY',
                  scheduledDate: '2026-09-15',
                  isRealized: true,
                ),
                InstallmentItemModel(
                  sequence: 2,
                  amount: '3000.0000',
                  currency: 'TRY',
                  scheduledDate: '2026-10-15',
                  isRealized: true,
                ),
                InstallmentItemModel(
                  sequence: 3,
                  amount: '3000.0000',
                  currency: 'TRY',
                  scheduledDate: '2026-11-15',
                  isRealized: false,
                ),
                InstallmentItemModel(
                  sequence: 4,
                  amount: '3000.0000',
                  currency: 'TRY',
                  scheduledDate: '2026-12-15',
                  isRealized: false,
                ),
                InstallmentItemModel(
                  sequence: 5,
                  amount: '3000.0000',
                  currency: 'TRY',
                  scheduledDate: '2027-01-15',
                  isRealized: false,
                ),
                InstallmentItemModel(
                  sequence: 6,
                  amount: '3000.0000',
                  currency: 'TRY',
                  scheduledDate: '2027-02-15',
                  isRealized: false,
                ),
              ],
            ),
          ],
    accounts: const [FinanceChoice(id: 'a1', name: 'Ziraat Vadesiz')],
    expenseCategories: const [
      FinanceChoice(id: 'c1', name: 'Ticari mal alımı'),
    ],
  );

  @override
  Future<CardActivity> loadActivity(
    String cardId, {
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async => const CardActivity(
    charges: [
      CardChargeItem(
        id: 'c1',
        amount: '2340.0000',
        currency: 'TRY',
        date: '2026-11-06',
        description: 'Market alışverişi',
        isCancelled: false,
      ),
      CardChargeItem(
        id: 'c2',
        amount: '3000.0000',
        currency: 'TRY',
        date: '2026-10-15',
        description: 'Buzdolabı · 2. taksit',
        isCancelled: false,
      ),
      CardChargeItem(
        id: 'c3',
        amount: '860.0000',
        currency: 'TRY',
        date: '2026-10-09',
        description: null,
        isCancelled: false,
      ),
    ],
    payments: [
      CardPaymentItem(
        id: 'p1',
        amount: '5000.0000',
        currency: 'TRY',
        date: '2026-10-24',
        description: null,
        isCancelled: false,
      ),
    ],
  );

  @override
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) async => (await loadCurrentStatement(cardId))!;

  @override
  Future<CardStatement?> loadCurrentStatement(String cardId) async =>
      const CardStatement(
        periodStart: '2026-09-16',
        closingDate: '2026-10-15',
        dueDate: '2026-10-25',
        year: 2026,
        month: 10,
        previousBalance: '0.0000',
        periodCharges: '11200.0000',
        paymentsThroughClosing: '0.0000',
        statementBalance: '11200.0000',
        paymentsAfterClosing: '5000.0000',
        remainingBalance: '6200.0000',
        minimumPayment: '2240.0000',
        remainingMinimumPayment: '2240.0000',
        minimumPaymentRate: '20.0000',
        currency: 'TRY',
        paymentStatus: 'open',
      );
}
