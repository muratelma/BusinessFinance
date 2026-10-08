import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_scope_selector.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_form_page.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';

void main() {
  testWidgets('işletmesi olmayan kullanıcıda kapsam alanı hiç çizilmez', (
    tester,
  ) async {
    await _pumpExpenseForm(tester, hasBusiness: false);

    expect(find.byType(AppScopeField), findsNothing);
    expect(find.text('Kapsam'), findsNothing);
  });

  testWidgets('kaynağın etiketi çipi doldurur ve nereden geldiğini yazar', (
    tester,
  ) async {
    await _pumpExpenseForm(tester, hasBusiness: true);

    await _selectSource(tester, 'Dükkân kasası');
    // Kategori seçilmeden taraf bölümü çizilmez: sorulacak bir şey yoktur.
    expect(find.text('Kapsam'), findsNothing);

    await _selectCategory(tester, 'Kırtasiye');

    expect(_selectedScope(tester), TransactionScope.business);
    expect(
      find.text('Dükkân kasası etiketinden geldi — değiştirebilirsiniz.'),
      findsOneWidget,
    );
  });

  // Kasadan market: kasa işletme etiketli, kategori şahsi. Kayıt şahsidir;
  // hesabın etiketi yalnız iki tarafa açık kategoride ön değerdir (ADR 0020).
  testWidgets('tek taraflı kategori tarafı söyler, kaynağın etiketi ezemez', (
    tester,
  ) async {
    await _pumpExpenseForm(tester, hasBusiness: true);

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Market');

    final row = tester.widget<AppScopeInfoRow>(find.byType(AppScopeInfoRow));
    expect(row.scope, TransactionScope.personal);
    expect(row.reason, 'kategoriden');
  });

  // Kategori tarafı söylüyorsa sorulacak bir şey yoktur: çip çizilmez ve
  // kayıt kategorinin tarafına yazılır.
  testWidgets('tek taraflı kategoride çip çizilmez, kayıt o tarafa yazılır', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: true,
      transactions: transactions,
    );

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Market');
    expect(find.byType(AppScopeField), findsNothing);
    expect(find.byType(AppScopeInfoRow), findsOneWidget);

    await tester.enterText(find.byType(TextFormField).first, '125,50');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(transactions.created.single.scope, TransactionScope.personal);
  });

  testWidgets('kullanıcının seçimi kaynağın etiketini yener', (tester) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: true,
      transactions: transactions,
    );

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Kırtasiye');
    await tester.tap(find.text('Şahsi'));
    await tester.pumpAndSettle();

    expect(_selectedScope(tester), TransactionScope.personal);
    expect(find.text('Bu kayıt için siz seçtiniz.'), findsOneWidget);
  });

  testWidgets('yazılan kapsam ekranda görünenle aynıdır', (tester) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: true,
      transactions: transactions,
    );

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Kırtasiye');
    // İki tarafa açık kategoride hesabın etiketi ön değerdir.
    expect(_selectedScope(tester), TransactionScope.business);

    await tester.enterText(find.byType(TextFormField).first, '125,50');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(transactions.created.single.scope, TransactionScope.business);
  });

  testWidgets('zincir çözülemezse kayıt yazılmaz ve alanın yanında söylenir', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpExpenseForm(
      tester,
      hasBusiness: true,
      transactions: transactions,
    );

    await _selectSource(tester, 'Ortak hesap');
    await _selectCategory(tester, 'Kırtasiye');
    await tester.enterText(find.byType(TextFormField).first, '125,50');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(transactions.created, isEmpty);
    expect(find.text('Bu kayıt için kapsam seçin.'), findsOneWidget);
  });

  testWidgets('kart harcaması da kapsamı taşır', (tester) async {
    final finance = _FakeFinance();
    await _pumpExpenseForm(tester, hasBusiness: true, finance: finance);

    await _selectSource(tester, 'İşletme kartı');
    await _selectCategory(tester, 'Market');
    await tester.enterText(find.byType(TextFormField).first, '40');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    // Kartın etiketi işletme, kategori şahsi: harcama şahsidir.
    expect(finance.charges.single.$2['scope'], 'personal');
  });

  testWidgets('gelir formunda kapsam hesabın etiketinden gelir', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpForm(
      tester,
      isExpense: false,
      hasBusiness: true,
      transactions: transactions,
    );

    await tester.tap(find.byType(DropdownButtonFormField<String>).first);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Dükkân kasası').last);
    await tester.pumpAndSettle();
    await _selectCategory(tester, 'Diğer gelir');

    expect(_selectedScope(tester), TransactionScope.business);
  });
}

TransactionScope? _selectedScope(WidgetTester tester) =>
    tester.widget<AppScopeField>(find.byType(AppScopeField)).value;

Future<void> _selectSource(WidgetTester tester, String name) async {
  await tester.tap(find.byType(DropdownButtonFormField<PaymentSource>));
  await tester.pumpAndSettle();
  // Kart satırı adının yanında kullanılabilir limiti de yazıyor; eşleşme
  // bu yüzden tam metin değil.
  await tester.tap(find.textContaining(name).last);
  await tester.pumpAndSettle();
}

Future<void> _selectCategory(WidgetTester tester, String name) async {
  await tester.tap(find.byType(DropdownButtonFormField<String>).last);
  await tester.pumpAndSettle();
  await tester.tap(find.text(name).last);
  await tester.pumpAndSettle();
}

Future<void> _pumpExpenseForm(
  WidgetTester tester, {
  required bool hasBusiness,
  _FakeTransactions? transactions,
  _FakeFinance? finance,
}) => _pumpForm(
  tester,
  isExpense: true,
  hasBusiness: hasBusiness,
  transactions: transactions,
  finance: finance,
);

Future<void> _pumpForm(
  WidgetTester tester, {
  required bool isExpense,
  required bool hasBusiness,
  _FakeTransactions? transactions,
  _FakeFinance? finance,
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
