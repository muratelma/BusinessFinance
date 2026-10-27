import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_directory.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_form_page.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_form_page.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';

/// Gelir ekle'deki `Hesap ekle`: ekleme formu doğrudan açılır, kaydedince
/// forma dönülür; yazılan tutar kalır ve yeni hesap seçili gelir.
void main() {
  testWidgets('yeni hesap forma döner, seçili gelir ve tutar silinmez', (
    tester,
  ) async {
    final transactions = _Transactions();
    final accounts = _Accounts();
    final router = GoRouter(
      routes: [
        GoRoute(
          path: '/',
          builder: (_, _) => QuickAddFormPage(
            isExpense: false,
            today: DateTime(2026, 10, 28),
            controller: QuickAddController(transactions, _Finance()),
          ),
        ),
        GoRoute(
          path: accountCreateLocation,
          builder: (_, _) => AccountCreatePage(repository: accounts),
        ),
      ],
    );
    addTearDown(router.dispose);
    tester.view.physicalSize = const Size(500, 1200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      MaterialApp.router(theme: AppTheme.light(), routerConfig: router),
    );
    await tester.pumpAndSettle();

    await tester.enterText(find.widgetWithText(TextFormField, 'Tutar'), '250');
    await tester.tap(find.text('Hesap'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Hesap ekle'));
    await tester.pumpAndSettle();
    expect(find.byType(AccountFormPage), findsOneWidget);

    await tester.enterText(find.byType(TextFormField).first, 'Ziraat');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(accounts.created, 'Ziraat');
    expect(find.byType(QuickAddFormPage), findsOneWidget);
    expect(transactions.accountReads, 2);
    expect(
      find.descendant(
        of: find.byType(AppAccountField),
        matching: find.text('Ziraat'),
      ),
      findsOneWidget,
    );
    expect(find.text('250'), findsOneWidget);
    expect(tester.takeException(), isNull);
  });
}

class _Transactions implements TransactionRepositoryContract {
  int accountReads = 0;

  @override
  Future<List<TransactionChoice>> listAccounts() async {
    accountReads++;
    return [
      if (accountReads > 1)
        const TransactionChoice(id: 'new', name: 'Ziraat', isActive: true),
    ];
  }

  @override
  Future<List<TransactionChoice>> listCategories(TransactionKind kind) async =>
      const [TransactionChoice(id: 'cat-1', name: 'Satış', isActive: true)];

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _Finance implements FinanceRepositoryContract {
  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _Accounts implements AccountRepository {
  String? created;

  @override
  Future<Account> create({
    required String name,
    required String type,
    required String openingBalance,
    TransactionScope? defaultScope,
  }) async {
    created = name;
    return Account(
      id: 'new',
      name: name,
      type: type,
      currency: 'TRY',
      isActive: true,
      openingBalance: openingBalance,
      balance: openingBalance,
    );
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}
