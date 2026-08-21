import 'dart:async';

import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_models.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_repository.dart';
import 'package:business_finance_mobile/features/budgets/presentation/budgets_controller.dart';

void main() {
  test('moves between months and exposes empty state', () async {
    final repository = _FakeBudgetRepository();
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 1),
    );
    await controller.load();
    await controller.changeMonth(1);
    expect(controller.selectedMonth, DateTime(2026, 2));
    expect(controller.items, isEmpty);
    expect(repository.listCalls, 2);
  });

  test('prevents double update and preserves string money', () async {
    final repository = _FakeBudgetRepository();
    final controller = BudgetsController(repository);
    final first = controller.update('budget', '123.4567');
    final second = await controller.update('budget', '123.4567');
    expect(second, isFalse);
    expect(repository.updateCalls, 1);
    repository.updateCompleter.complete(_budget(limit: '123.4567'));
    expect(await first, isTrue);
    expect(controller.items?.single.limit, '123.4567');
  });

  test('marks 401 as unauthorized', () async {
    final repository = _FakeBudgetRepository()
      ..listError = const ApiException(
        code: 'authentication.required',
        message: 'Giriş gerekli.',
        statusCode: 401,
      );
    final controller = BudgetsController(repository);
    await controller.load();
    expect(controller.unauthorized, isTrue);
  });

  test('reloads for transaction changes but ignores account changes', () async {
    final repository = _FakeBudgetRepository();
    final changes = FinancialDataChanges();
    final controller = BudgetsController(repository, changes: changes);
    await controller.load();

    changes.accountsChanged();
    await Future<void>.delayed(Duration.zero);
    expect(repository.listCalls, 1);

    changes.transactionsChanged();
    await Future<void>.delayed(Duration.zero);
    expect(repository.listCalls, 2);
  });
}

class _FakeBudgetRepository implements BudgetRepositoryContract {
  int listCalls = 0;
  int updateCalls = 0;
  ApiException? listError;
  final updateCompleter = Completer<BudgetItem>();

  @override
  Future<List<BudgetItem>> list(int year, int month) async {
    listCalls++;
    if (listError case final error?) throw error;
    return [];
  }

  @override
  Future<BudgetItem> update(String id, String limit) {
    updateCalls++;
    return updateCompleter.future;
  }

  @override
  Future<BudgetItem> create(CreateBudgetInput input) async => _budget();
  @override
  Future<List<BudgetCategory>> listExpenseCategories() async => const [];
}

BudgetItem _budget({String limit = '100.0000'}) => BudgetItem(
  id: 'budget',
  categoryId: 'category',
  categoryName: 'Market',
  limit: limit,
  spent: '50.0000',
  remaining: '50.0000',
  exceeded: '0.0000',
  currency: 'TRY',
  year: 2026,
  month: 8,
);
