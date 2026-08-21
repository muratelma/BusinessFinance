import 'dart:async';

import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';
import 'package:business_finance_mobile/features/transactions/presentation/transactions_controller.dart';

void main() {
  test('loads empty state and exposes unauthorized error', () async {
    final repository = _FakeTransactionRepository();
    final controller = TransactionsController(repository);

    await controller.load();
    expect(controller.page?.items, isEmpty);

    repository.listError = const ApiException(
      code: 'authentication.required',
      message: 'Giriş gerekli.',
      statusCode: 401,
    );
    await controller.load();
    expect(controller.unauthorized, isTrue);
    expect(controller.errorMessage, 'Giriş gerekli.');
  });

  test('prevents double create and announces success', () async {
    final repository = _FakeTransactionRepository();
    final changes = FinancialDataChanges();
    final controller = TransactionsController(
      repository,
      financialDataChanges: changes,
    );
    final input = CreateTransactionInput(
      accountId: 'account',
      categoryId: 'category',
      amount: '12.3400',
      kind: TransactionKind.expense,
      transactionDate: '2026-08-09',
    );

    final first = controller.create(input);
    final second = await controller.create(input);
    expect(second, isFalse);
    expect(repository.createCalls, 1);
    repository.createCompleter.complete(_transaction(amount: '12.3400'));
    expect(await first, isTrue);
    expect(controller.successMessage, 'İşlem kaydedildi.');
    expect(changes.dashboardRevision, 1);
    expect(changes.budgetsRevision, 1);
  });

  test('cancelling a transaction invalidates dashboard and budgets', () async {
    final changes = FinancialDataChanges();
    final controller = TransactionsController(
      _FakeTransactionRepository(),
      financialDataChanges: changes,
    );

    expect(await controller.cancel('transaction'), isTrue);

    expect(changes.dashboardRevision, 1);
    expect(changes.budgetsRevision, 1);
  });

  test('external transaction changes reload the transaction list', () async {
    final changes = FinancialDataChanges();
    final repository = _FakeTransactionRepository();
    final controller = TransactionsController(
      repository,
      financialDataChanges: changes,
    );
    await controller.load();

    changes.transactionsChanged();
    await Future<void>.delayed(Duration.zero);

    expect(repository.listCalls, 2);
    controller.dispose();
  });
}

class _FakeTransactionRepository implements TransactionRepositoryContract {
  ApiException? listError;
  int createCalls = 0;
  int listCalls = 0;
  final createCompleter = Completer<TransactionItem>();

  @override
  Future<TransactionPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    TransactionFilter filter = const TransactionFilter(),
  }) async {
    listCalls++;
    if (listError case final error?) throw error;
    return const TransactionPage(
      items: [],
      pageNumber: 1,
      pageSize: 20,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: false,
      hasNextPage: false,
    );
  }

  @override
  Future<TransactionItem> create(CreateTransactionInput input) {
    createCalls++;
    return createCompleter.future;
  }

  @override
  Future<TransactionItem> cancel(String id) async =>
      _transaction(cancelled: true);

  @override
  Future<List<TransactionChoice>> listAccounts() async => const [];

  @override
  Future<List<TransactionChoice>> listCategories(TransactionKind kind) async =>
      const [];
}

TransactionItem _transaction({
  String amount = '10.0000',
  bool cancelled = false,
}) => TransactionItem(
  id: 'transaction',
  accountId: 'account',
  categoryId: 'category',
  amount: amount,
  currency: 'TRY',
  kind: TransactionKind.expense,
  transactionDate: '2026-08-09',
  isCancelled: cancelled,
);
