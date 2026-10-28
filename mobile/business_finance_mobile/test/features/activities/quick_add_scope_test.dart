import '../../support/account_field_helpers.dart';
import '../../support/category_field_helpers.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_form_page.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';

void main() {
  testWidgets('işletmesi olmayan kullanıcıda kapsam hiç yazılmaz ve sorulmaz', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: false,
      transactions: transactions,
    );

    await _selectSource(tester, 'Dükkân kasası');
    await tester.tap(categoryField());
    await tester.pumpAndSettle();
    // Düz liste: grup başlığı ve kapsam satırı yok.
    expect(find.text('İşletme veya şahsi'), findsNothing);
    await tester.tap(categoryMenuRow('Kırtasiye'));
    await tester.pumpAndSettle();
    expect(find.text('Şahsi'), findsNothing);
    expect(find.text('İşletme'), findsNothing);

    await _save(tester);
    expect(transactions.created.single.scope, isNull);
  });

  // Kasadan market: kasa işletme etiketli, kategori şahsi. Kayıt şahsidir;
  // hesabın etiketi kategoriyi ezmez (ADR 0020).
  testWidgets('tek kapsamlı kategori kapsamı söyler, kayıt o kapsama yazılır', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: true,
      transactions: transactions,
    );

    await _selectSource(tester, 'Dükkân kasası');
    await chooseCategory(tester, 'Market');

    expect(
      find.descendant(of: categoryField(), matching: find.text('Şahsi')),
      findsOneWidget,
    );
    await _save(tester);
    expect(transactions.created.single.scope, TransactionScope.personal);
  });

  testWidgets('iki kapsama açık kategoride kapsam menüde seçilir', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: true,
      transactions: transactions,
    );

    await _selectSource(tester, 'Dükkân kasası');
    await chooseCategory(tester, 'Kırtasiye', scope: TransactionScope.personal);

    expect(
      find.descendant(of: categoryField(), matching: find.text('Şahsi')),
      findsOneWidget,
    );
    await _save(tester);
    expect(transactions.created.single.scope, TransactionScope.personal);
  });

  // Kullanıcı kararı D5 (28 Ekim 2026): hesabın kapsamı ön değer değildir.
  testWidgets('hesabın kapsamı ön değer olmaz; kapsamsız kayıt yazılmaz', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: true,
      transactions: transactions,
      prefill: const QuickAddPrefill(
        categoryId: QuickAddSuggestion('cat-2', QuickAddSuggestionState.read),
      ),
    );

    await _selectSource(tester, 'Dükkân kasası');
    expect(
      find.descendant(of: categoryField(), matching: find.text('Kapsam seçin')),
      findsOneWidget,
    );
    await _save(tester);

    expect(transactions.created, isEmpty);
    expect(find.text('Kapsam seçin.'), findsOneWidget);
  });

  testWidgets('kart harcaması da kapsamı taşır', (tester) async {
    final finance = _FakeFinance();
    await _pumpExpenseForm(tester, hasBusiness: true, finance: finance);

    await _selectSource(tester, 'İşletme kartı');
    await chooseCategory(tester, 'Market');
    await tester.enterText(find.byType(TextFormField).first, '40');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    // Kartın etiketi işletme, kategori şahsi: harcama şahsidir.
    expect(finance.charges.single.$2['scope'], 'personal');
  });

  testWidgets('gelir formunda da kapsam kategoriyle birlikte seçilir', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpForm(
      tester,
      isExpense: false,
      hasBusiness: true,
      transactions: transactions,
    );

    await chooseAccount(tester, 'Dükkân kasası', label: 'Hesap');
    await chooseCategory(
      tester,
      'Diğer gelir',
      scope: TransactionScope.business,
    );
    await _save(tester);

    expect(transactions.created.single.scope, TransactionScope.business);
  });
}

Future<void> _save(WidgetTester tester) async {
  await tester.enterText(find.byType(TextFormField).first, '125,50');
  await tester.tap(find.text('Kaydet'));
  await tester.pumpAndSettle();
}

Future<void> _selectSource(WidgetTester tester, String name) async {
  await chooseAccount(tester, name, label: 'Ödeme kaynağı');
}

Future<void> _pumpExpenseForm(
  WidgetTester tester, {
  required bool hasBusiness,
  _FakeTransactions? transactions,
  _FakeFinance? finance,
  QuickAddPrefill? prefill,
}) => _pumpForm(
  tester,
  isExpense: true,
  hasBusiness: hasBusiness,
  transactions: transactions,
  finance: finance,
  prefill: prefill,
);

Future<void> _pumpForm(
  WidgetTester tester, {
  required bool isExpense,
  required bool hasBusiness,
  _FakeTransactions? transactions,
  _FakeFinance? finance,
  QuickAddPrefill? prefill,
}) async {
  final scopeController = ScopeController(
    store: _MemoryStore(hasBusiness: hasBusiness),
  );
  await scopeController.ensureLoaded();

  tester.view.physicalSize = const Size(500, 1400);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: QuickAddFormPage(
        isExpense: isExpense,
        prefill: prefill,
        scopeController: scopeController,
        controller: QuickAddController(
          transactions ?? _FakeTransactions(),
          finance ?? _FakeFinance(),
        ),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

class _MemoryStore implements ScopeStore {
  _MemoryStore({this.hasBusiness});

  TransactionScope? scope;
  bool? hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => scope;

  @override
  Future<void> writeScope(TransactionScope? value) async => scope = value;

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async => hasBusiness = value;

  @override
  Future<void> clear() async {
    scope = null;
    hasBusiness = null;
  }
}

class _FakeTransactions implements TransactionRepositoryContract {
  final List<CreateTransactionInput> created = [];

  @override
  Future<TransactionItem> create(CreateTransactionInput input) async {
    created.add(input);
    return TransactionItem(
      id: 't1',
      accountId: input.accountId,
      categoryId: input.categoryId,
      amount: input.amount,
      currency: 'TRY',
      kind: input.kind,
      transactionDate: input.transactionDate,
      isCancelled: false,
    );
  }

  @override
  Future<List<TransactionChoice>> listAccounts() async => const [
    TransactionChoice(
      id: 'acc-1',
      name: 'Dükkân kasası',
      isActive: true,
      defaultScope: TransactionScope.business,
    ),
    TransactionChoice(id: 'acc-2', name: 'Ortak hesap', isActive: true),
  ];

  @override
  Future<List<TransactionChoice>> listCategories(TransactionKind kind) async =>
      const [
        TransactionChoice(
          id: 'cat-1',
          name: 'Satış geliri',
          isActive: true,
          defaultScope: TransactionScope.business,
        ),
        // İki tarafa açık gelir kalemi: taraf kayıtta sorulur.
        TransactionChoice(id: 'cat-2', name: 'Diğer gelir', isActive: true),
      ];

  @override
  Future<TransactionItem> cancel(String id) => throw UnimplementedError();

  @override
  Future<TransactionPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    TransactionFilter filter = const TransactionFilter(),
  }) => throw UnimplementedError();
}

class _FakeFinance implements FinanceRepositoryContract {
  final List<(String, Map<String, Object?>)> charges = [];

  @override
  Future<FinanceSnapshot> load({
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async => const FinanceSnapshot(
    transfers: [],
    cards: [
      CreditCardItem(
        id: 'card-1',
        name: 'İşletme kartı',
        limit: '10000.0000',
        currentDebt: '0.0000',
        availableLimit: '10000.0000',
        currency: 'TRY',
        statementClosingDay: 15,
        paymentDueDay: 25,
        minimumPaymentRate: '20.0000',
        isActive: true,
        defaultScope: TransactionScope.business,
      ),
    ],
    plans: [],
    accounts: [
      FinanceChoice(
        id: 'acc-1',
        name: 'Dükkân kasası',
        defaultScope: TransactionScope.business,
      ),
      FinanceChoice(id: 'acc-2', name: 'Ortak hesap'),
    ],
    expenseCategories: [
      FinanceChoice(
        id: 'cat-1',
        name: 'Market',
        defaultScope: TransactionScope.personal,
      ),
      // Kullanıcının kendi açtığı, kapsamsız kategori: zincirin çözülemediği
      // tek meşru durum bu.
      FinanceChoice(id: 'cat-2', name: 'Kırtasiye'),
    ],
  );

  @override
  Future<void> createCharge(String cardId, Map<String, Object?> input) async {
    charges.add((cardId, input));
  }

  @override
  Future<void> cancelTransfer(String transferId) => throw UnimplementedError();
  @override
  Future<void> createCard(Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> updateCard(String cardId, Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> createPayment(String cardId, Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> createPlan(Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> createTransfer(Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<CardActivity> loadActivity(
    String cardId, {
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) => throw UnimplementedError();
  @override
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) => throw UnimplementedError();
  @override
  Future<CardStatement?> loadCurrentStatement(String cardId) =>
      throw UnimplementedError();
  @override
  Future<void> realizeInstallment(String planId, int sequence) =>
      throw UnimplementedError();
}
