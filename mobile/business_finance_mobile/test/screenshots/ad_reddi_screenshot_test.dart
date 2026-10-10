import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_error_messages.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/accounts/presentation/accounts_page.dart';
import 'package:business_finance_mobile/features/accounts/presentation/accounts_view_model.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/cards/presentation/finance_page.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_repository.dart';
import 'package:business_finance_mobile/features/counterparties/presentation/counterparties_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Aynı adlı kayıt reddedilince formun içinde çıkan cümle (10 Ekim 2026):
/// tam sayfa form (hesap) ve iki panel (kart, kişi). Veri sentetiktir.
void main() {
  testWidgets('ad reddi 01 hesap', (tester) async {
    await captureScreen(
      tester,
      'ad-reddi-01-hesap',
      AccountsPage(viewModel: AccountsViewModel(_Accounts())),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.byType(FloatingActionButton));
        await _settle(tester);
        await tester.enterText(
          find.byType(TextFormField).first,
          'ziraatvadesiz',
        );
        await tester.tap(find.text('Kaydet'));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('ad reddi 02 kart', (tester) async {
    await captureScreen(
      tester,
      'ad-reddi-02-kart',
      FinancePage(repository: _Finance()),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Kart ekle'));
        await _settle(tester);
        await tester.enterText(
          find.widgetWithText(TextFormField, 'Kart adı'),
          'garanti-bonus',
        );
        await tester.enterText(
          find.widgetWithText(TextFormField, 'Limit'),
          '50000',
        );
        await tester.tap(find.text('Kaydet'));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('ad reddi 03 kişi', (tester) async {
    await captureScreen(
      tester,
      'ad-reddi-03-kisi',
      CounterpartiesPage(repository: _Counterparties()),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Karşı taraf ekle'));
        await _settle(tester);
        await tester.enterText(find.byType(TextFormField).first, 'ahmetbakkal');
        await tester.tap(find.text('Ekle'));
      },
    );
  }, skip: !screenshotsEnabled);
}

Future<void> _settle(WidgetTester tester) async {
  for (var i = 0; i < 10; i++) {
    await tester.pump(const Duration(milliseconds: 100));
  }
}

ApiException _refusal(String code) => ApiException(
  statusCode: 409,
  code: code,
  message: ApiErrorMessages.resolve(code),
);

class _Accounts implements AccountRepository {
  @override
  Future<AccountPage> list({bool? isActive, String? type}) async => AccountPage(
    items: [
      Account.fromJson(const {
        'id': 'account-1',
        'name': 'Ziraat Vadesiz',
        'type': 'bank',
        'currency': 'TRY',
        'isActive': true,
        'openingBalance': '12500.0000',
        'balance': '18240.5000',
      }),
      Account.fromJson(const {
        'id': 'account-2',
        'name': 'Dükkan kasası',
        'type': 'cash',
        'currency': 'TRY',
        'isActive': true,
        'openingBalance': '1000.0000',
        'balance': '3420.0000',
      }),
    ],
    pagination: const AccountPagination(
      pageNumber: 1,
      pageSize: 100,
      totalCount: 2,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false,
    ),
  );

  @override
  Future<Account> create({
    required String name,
    required String type,
    required String openingBalance,
    TransactionScope? defaultScope,
  }) async => throw _refusal('accounts.duplicate_name');

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _Finance implements FinanceRepositoryContract {
  @override
  Future<FinanceSnapshot> load({
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async => FinanceSnapshot(
    transfers: const [],
    cards: [
      CreditCardItem.fromJson(const {
        'id': 'card-1',
        'name': 'Garanti Bonus',
        'limit': '60000.0000',
        'currentDebt': '8250.5000',
        'availableLimit': '51749.5000',
        'currency': 'TRY',
        'statementClosingDay': 10,
        'paymentDueDay': 20,
        'minimumPaymentRate': '20.0000',
        'isActive': true,
      }),
    ],
    plans: const [],
    accounts: const [],
    expenseCategories: const [],
  );

  @override
  Future<CardStatement?> loadCurrentStatement(String cardId) async => null;

  @override
  Future<void> createCard(Map<String, Object?> input) async =>
      throw _refusal('credit_cards.duplicate_name');

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _Counterparties implements CounterpartyRepositoryContract {
  @override
  Future<CounterpartiesSnapshot> load(
    CounterpartyBalanceFilter filter,
    String asOfDate,
  ) async => const CounterpartiesSnapshot(
    counterparties: [
      CounterpartySummary(
        id: 'cp-1',
        name: 'Ahmet Bakkal',
        isActive: true,
        receivable: '400.0000',
        payable: '0.0000',
        net: '400.0000',
        isSettled: false,
      ),
    ],
    accounts: [DataChoice('account-1', 'Kasa')],
    categories: [],
  );

  @override
  Future<void> create(String name, String? note) async =>
      throw _refusal('counterparties.duplicate_name');

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}
