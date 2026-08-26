import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_form_page.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';

/// Aşama 05 Grup 7: formdaki vergi bölümü. KDV isteğe bağlı ve katlanmış,
/// indirilebilirlik yalnız işletme kapsamlı giderde görünür (ADR 0016).
void main() {
  testWidgets('işletmesi olmayan kullanıcıda vergi bölümü hiç çizilmez', (
    tester,
  ) async {
    await _pumpForm(tester, hasBusiness: false);

    expect(find.text('Vergi bilgisi (isteğe bağlı)'), findsNothing);
  });

  testWidgets('bölüm kapalı açılır ve boşken tek satırdır', (tester) async {
    await _pumpForm(tester, hasBusiness: true);

    expect(find.text('Vergi bilgisi (isteğe bağlı)'), findsOneWidget);
    expect(find.text('KDV girilmedi'), findsOneWidget);
    // Kapalıyken alanlar çizilmiyor: form sadeliği bozulmuyor.
    expect(find.text('KDV oranı'), findsNothing);
    expect(find.text('KDV tutarı'), findsNothing);
  });

  testWidgets('KDV boş bırakıldığında istekte hiç gitmez', (tester) async {
    final transactions = _FakeTransactions();
    await _pumpForm(tester, hasBusiness: true, transactions: transactions);

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Market');
    await tester.enterText(find.byType(TextFormField).first, '120');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    final input = transactions.created.single;
    expect(input.vat, isNull);
    expect(input.toJson()['vatRate'], isNull);
    expect(input.toJson()['vatAmount'], isNull);
  });

  testWidgets('yalnız oran girilirse tutar boş gider — istemci de bölmez', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpForm(tester, hasBusiness: true, transactions: transactions);

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Market');
    await tester.enterText(find.byType(TextFormField).first, '120');
    await _openTaxSection(tester);
    await tester.enterText(
      find.widgetWithText(TextFormField, 'KDV oranı'),
      '20',
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    final input = transactions.created.single;
    expect(input.vat!.rate, '0.2000');
    expect(input.vat!.amount, isNull);
  });

  testWidgets('kaydın tutarını aşan KDV alanın yanında reddedilir', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpForm(tester, hasBusiness: true, transactions: transactions);

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Market');
    await tester.enterText(find.byType(TextFormField).first, '120');
    await _openTaxSection(tester);
    await tester.enterText(
      find.widgetWithText(TextFormField, 'KDV tutarı'),
      '200',
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(transactions.created, isEmpty);
    expect(find.text('KDV, kaydın tutarından büyük olamaz.'), findsOneWidget);
  });

  testWidgets('indirilebilirlik yalnız işletme kapsamlı giderde görünür', (
    tester,
  ) async {
    await _pumpForm(tester, hasBusiness: true);

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Market');
    await _openTaxSection(tester);
    expect(find.text('Vergiden düşülebilir'), findsOneWidget);

    // Aynı kayıt şahsi işaretlenince soru kayboluyor.
    await tester.tap(find.text('Şahsi'));
    await tester.pumpAndSettle();
    expect(find.text('Vergiden düşülebilir'), findsNothing);
  });

  testWidgets('gelir formunda indirilebilirlik sorulmaz', (tester) async {
    await _pumpForm(tester, hasBusiness: true, isExpense: false);

    await tester.tap(find.byType(DropdownButtonFormField<String>).first);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Dükkân kasası').last);
    await tester.pumpAndSettle();
    await _openTaxSection(tester);

    expect(find.text('KDV oranı'), findsOneWidget);
    expect(find.text('Vergiden düşülebilir'), findsNothing);
  });

  testWidgets('şahsi kayıtta indirilebilirlik istekte hiç gitmez', (
    tester,
  ) async {
    final transactions = _FakeTransactions();
    await _pumpForm(tester, hasBusiness: true, transactions: transactions);

    await _selectSource(tester, 'Dükkân kasası');
    await _selectCategory(tester, 'Market');
    await _openTaxSection(tester);
    await tester.tap(find.text('Vergiden düşülebilir'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Şahsi'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).first, '120');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(transactions.created.single.isTaxDeductible, isNull);
  });

  testWidgets('kart harcaması da KDV taşır', (tester) async {
    final finance = _FakeFinance();
    await _pumpForm(tester, hasBusiness: true, finance: finance);

    await _selectSource(tester, 'İşletme kartı');
    await _selectCategory(tester, 'Market');
    await tester.enterText(find.byType(TextFormField).first, '120');
    await _openTaxSection(tester);
    await tester.enterText(
      find.widgetWithText(TextFormField, 'KDV tutarı'),
      '20',
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(finance.charges.single.$2['vatAmount'], '20.0000');
  });
}

Future<void> _openTaxSection(WidgetTester tester) async {
  await tester.tap(find.text('Vergi bilgisi (isteğe bağlı)'));
  await tester.pumpAndSettle();
}

Future<void> _selectSource(WidgetTester tester, String name) async {
  await tester.tap(find.byType(DropdownButtonFormField<PaymentSource>));
  await tester.pumpAndSettle();
  await tester.tap(find.textContaining(name).last);
  await tester.pumpAndSettle();
}

Future<void> _selectCategory(WidgetTester tester, String name) async {
  await tester.tap(find.byType(DropdownButtonFormField<String>).last);
  await tester.pumpAndSettle();
  await tester.tap(find.text(name).last);
  await tester.pumpAndSettle();
}

Future<void> _pumpForm(
  WidgetTester tester, {
  required bool hasBusiness,
  bool isExpense = true,
  _FakeTransactions? transactions,
  _FakeFinance? finance,
}) async {
  final scopeController = ScopeController(
    store: _MemoryStore(hasBusiness: hasBusiness),
  );
  await scopeController.ensureLoaded();

  tester.view.physicalSize = const Size(500, 1800);
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
    ],
    expenseCategories: [
      FinanceChoice(
        id: 'cat-1',
        name: 'Market',
        defaultScope: TransactionScope.business,
        defaultIsTaxDeductible: true,
      ),
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
