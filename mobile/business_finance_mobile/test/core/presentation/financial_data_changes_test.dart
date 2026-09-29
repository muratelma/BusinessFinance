import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';

/// The mutation-to-refresh matrix. Each case asserts both what must refresh and
/// what must not: over-raising makes screens refetch work that cannot have
/// changed, and under-raising leaves the user looking at numbers that no longer
/// exist.
void main() {
  test('opening or closing an account moves no money', () {
    final changes = FinancialDataChanges();

    changes.accountsChanged();

    expect(changes.accountsRevision, 1);
    expect(changes.dashboardRevision, 1);
    expect(changes.budgetsRevision, 0);
    expect(changes.activityFeedRevision, 0);
    expect(changes.cardsRevision, 0);
  });

  test('an account income or expense counts towards a budget', () {
    final changes = FinancialDataChanges();

    changes.transactionsChanged();

    expect(changes.activityFeedRevision, 1);
    expect(changes.dashboardRevision, 1);
    expect(changes.budgetsRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.cardsRevision, 0);
  });

  test('a transfer moves balances but is neither income nor expense', () {
    final changes = FinancialDataChanges();

    changes.transferChanged();

    expect(changes.activityFeedRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.budgetsRevision, 0);
  });

  /// A card purchase is an expense and raises the card debt, but the money has
  /// not left any account yet.
  test('a card charge hits budgets and cards, not account balances', () {
    final changes = FinancialDataChanges();

    changes.cardSpendingChanged();

    expect(changes.activityFeedRevision, 1);
    expect(changes.budgetsRevision, 1);
    expect(changes.cardsRevision, 1);
    expect(changes.accountsRevision, 0);
  });

  /// Paying the card settles debt already counted as expense when the charges
  /// happened. Counting it again would double the same purchase.
  test('a card payment moves an account and the card, never a budget', () {
    final changes = FinancialDataChanges();

    changes.cardPaymentChanged();

    expect(changes.activityFeedRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.cardsRevision, 1);
    expect(changes.budgetsRevision, 0);
  });

  test('realizing an installment turns a plan into a real card charge', () {
    final changes = FinancialDataChanges();

    changes.installmentRealized();

    expect(changes.activityFeedRevision, 1);
    expect(changes.budgetsRevision, 1);
    expect(changes.cardsRevision, 1);
    expect(changes.planningRevision, 1);
  });

  /// A recurring occurrence can land on either side, so both are refreshed.
  test('realizing a recurring occurrence refreshes accounts and cards', () {
    final changes = FinancialDataChanges();

    changes.recurringRealized();

    expect(changes.activityFeedRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.cardsRevision, 1);
    expect(changes.planningRevision, 1);
    expect(changes.budgetsRevision, 1);
  });

  test('confirming an import creates real transactions', () {
    final changes = FinancialDataChanges();

    changes.importConfirmed();

    expect(changes.activityFeedRevision, 1);
    expect(changes.budgetsRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.cardsRevision, 0);
  });

  test('a debt movement can consume a budget through its opening', () {
    // Bu kapı eskiden bütçenin hiç yükselmemesini şart koşuyordu, çünkü borç
    // hiçbir gider üretmiyordu. Artık gider kaynaklı bir borcun açılışı
    // kategorili bir giderdir ve bütçeyi tüketir. Kredi kartıyla karışmasın:
    // orada gider harcama anında yazılır ve ödeme bütçeye dokunmaz; burada
    // gider borcun doğduğu anda yazılır ve taksit ödemesi bütçeye dokunmaz.
    final changes = FinancialDataChanges();

    changes.debtChanged();

    expect(changes.activityFeedRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.planningRevision, 1);
    expect(changes.budgetsRevision, 1);

    // Kart yine dışarıda: borcun kartla hiçbir ilişkisi yok.
    expect(changes.cardsRevision, 0);
  });

  test('settling an obligation moves cash without consuming budget again', () {
    final changes = FinancialDataChanges();

    changes.obligationSettled();

    expect(changes.activityFeedRevision, 1);
    expect(changes.dashboardRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.planningRevision, 1);
    expect(changes.counterpartiesRevision, 1);
    expect(changes.budgetsRevision, 0);
  });

  /// Kasa nakit hesapların bakiyesini gösterir. Bir hesabın bakiyesini
  /// değiştirebilen her olay Kasa'yı da eskitir; yenilemeyen ekran kullanıcıya
  /// kasada olmayan bir sayı gösterir (28 Eylül denetimi U11).
  test('every event that can move an account balance refreshes Kasa', () {
    final events = <String, void Function(FinancialDataChanges)>{
      'accountsChanged': (c) => c.accountsChanged(),
      'transactionsChanged': (c) => c.transactionsChanged(),
      'transferChanged': (c) => c.transferChanged(),
      'cardPaymentChanged': (c) => c.cardPaymentChanged(),
      'recurringRealized': (c) => c.recurringRealized(),
      'importConfirmed': (c) => c.importConfirmed(),
      'debtChanged': (c) => c.debtChanged(),
      'counterpartyLedgerChanged': (c) => c.counterpartyLedgerChanged(),
      'obligationSettled': (c) => c.obligationSettled(),
      'cashDifferenceConfirmed': (c) => c.cashDifferenceConfirmed(),
      'posSettlementTransferred': (c) => c.posSettlementTransferred(),
      'posSettlementTransferReverted': (c) => c.posSettlementTransferReverted(),
      'posSettlementCancelled': (c) => c.posSettlementCancelled(),
    };

    for (final entry in events.entries) {
      final changes = FinancialDataChanges();
      entry.value(changes);
      expect(changes.accountsRevision, 1, reason: entry.key);
      expect(changes.cashRevision, 1, reason: entry.key);
    }
  });

  /// Kart harcaması hiçbir hesabı kıpırdatmaz; Kasa'yı boşuna yükletmez.
  test('a card charge leaves Kasa alone', () {
    final changes = FinancialDataChanges();

    changes.cardSpendingChanged();

    expect(changes.cashRevision, 0);
  });

  test('a restore replaces everything, so every screen is stale', () {
    final changes = FinancialDataChanges();

    changes.restoreCompleted();

    expect(changes.activityFeedRevision, 1);
    expect(changes.dashboardRevision, 1);
    expect(changes.budgetsRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.cardsRevision, 1);
    expect(changes.planningRevision, 1);
  });

  test('the transaction list follows the same signal as the feed', () {
    final changes = FinancialDataChanges();

    changes.cardSpendingChanged();

    expect(changes.transactionsRevision, changes.activityFeedRevision);
  });

  test('listeners are notified once per mutation', () {
    final changes = FinancialDataChanges();
    var notifications = 0;
    changes.addListener(() => notifications++);

    changes.transactionsChanged();
    changes.transferChanged();

    expect(notifications, 2);
  });
}
