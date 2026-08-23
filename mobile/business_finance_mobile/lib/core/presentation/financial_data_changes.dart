import 'package:flutter/foundation.dart';

/// Tells screens that the data behind them moved, so they reread instead of
/// showing yesterday's numbers.
///
/// This is a refresh signal only. Balances, budgets and reports are always the
/// server's answer; nothing here recomputes them. Each mutation raises exactly
/// the targets it can affect, so a transfer does not make the budget screen
/// refetch work that cannot have changed.
///
/// **Kapsam anahtarının burada bir hedefi yok** ve olmamalı: anahtar veriyi
/// değiştirmez, aynı veriye başka bir soru sorar. Ekranlar onu kendi
/// kanalından (`ScopeController`) dinler. Buraya bağlansaydı her kapsam
/// dokunuşu, kapsamdan etkilenmeyen ekranları (hesaplar, kartlar) da boşuna
/// yeniden yükletirdi.
class FinancialDataChanges extends ChangeNotifier {
  int _activityFeedRevision = 0;
  int _dashboardRevision = 0;
  int _budgetsRevision = 0;
  int _accountsRevision = 0;
  int _cardsRevision = 0;
  int _planningRevision = 0;

  int get activityFeedRevision => _activityFeedRevision;
  int get dashboardRevision => _dashboardRevision;
  int get budgetsRevision => _budgetsRevision;
  int get accountsRevision => _accountsRevision;
  int get cardsRevision => _cardsRevision;
  int get planningRevision => _planningRevision;

  /// Kept for the transaction list, which predates the unified feed and follows
  /// the same signal.
  int get transactionsRevision => _activityFeedRevision;

  /// Opening, renaming or closing an account. No money moved, so only the
  /// balances that list accounts need to reread.
  void accountsChanged() => _raise(accounts: true, dashboard: true);

  /// Income or expense created or cancelled: it moves a balance and counts
  /// towards a category budget.
  void transactionsChanged() =>
      _raise(feed: true, dashboard: true, budgets: true, accounts: true);

  /// Money moved between two of the user's own accounts. Budgets are untouched,
  /// because a transfer is neither income nor expense.
  void transferChanged() => _raise(feed: true, dashboard: true, accounts: true);

  /// A card purchase is an expense and raises the card debt, but leaves account
  /// balances alone.
  void cardSpendingChanged() =>
      _raise(feed: true, dashboard: true, budgets: true, cards: true);

  /// Paying the card settles debt already counted as expense when the charges
  /// happened, so it is not a second expense and budgets stay put.
  void cardPaymentChanged() =>
      _raise(feed: true, dashboard: true, accounts: true, cards: true);

  /// Realizing an installment turns a plan into a real card charge.
  void installmentRealized() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    cards: true,
    planning: true,
  );

  /// A recurring occurrence can realize into either an account transaction or a
  /// card charge, so both sides are refreshed.
  void recurringRealized() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    cards: true,
    planning: true,
  );

  /// Confirming imported rows creates real transactions.
  void importConfirmed() =>
      _raise(feed: true, dashboard: true, budgets: true, accounts: true);

  /// Paying a debt instalment or collecting a receivable moves an account
  /// balance and clears an obligation, without touching income or expense.
  /// Borç açılışı ya da taksit ödemesi.
  ///
  /// `budgets` de yükselir: gider kaynaklı bir borcun açılışı kategorili bir
  /// giderdir ve bütçeyi tüketir. Kredi kartı ödemesinin bütçeyi
  /// yükseltmemesiyle karışmasın — orada gider harcama anında yazılır,
  /// burada borcun doğduğu anda.
  void debtChanged() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    planning: true,
  );

  /// A restore replaces everything the user has, so every screen is stale.
  void restoreCompleted() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    cards: true,
    planning: true,
  );

  void _raise({
    bool feed = false,
    bool dashboard = false,
    bool budgets = false,
    bool accounts = false,
    bool cards = false,
    bool planning = false,
  }) {
    if (feed) _activityFeedRevision++;
    if (dashboard) _dashboardRevision++;
    if (budgets) _budgetsRevision++;
    if (accounts) _accountsRevision++;
    if (cards) _cardsRevision++;
    if (planning) _planningRevision++;
    notifyListeners();
  }
}
