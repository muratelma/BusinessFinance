import 'dart:async';

import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
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

  test('jumps to a distant month in one read', () async {
    final repository = _FakeBudgetRepository();
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await controller.load();
    await controller.selectMonth(DateTime(2025, 12));
    expect(controller.selectedMonth, DateTime(2025, 12));
    // Sekiz ayın sekiz isteği değil, tek istek.
    expect(repository.listCalls, 2);

    await controller.selectMonth(DateTime(2025, 12));
    expect(repository.listCalls, 2, reason: 'aynı ay yeniden okunmaz');
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

  test('removes the limit and lets the category be budgeted again', () async {
    final repository = _FakeBudgetRepository()..pages[(2026, 8)] = [_budget()];
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await controller.load();
    expect(controller.budgetedCategoryIds, {'category'});

    expect(await controller.delete('budget'), isTrue);
    expect(repository.deletedIds, ['budget']);
    expect(controller.items, isEmpty);
    expect(controller.budgetedCategoryIds, isEmpty);
    expect(controller.successMessage, 'Bütçe silindi.');
  });

  test('reports the server error when deleting fails', () async {
    final repository = _FakeBudgetRepository()
      ..pages[(2026, 8)] = [_budget()]
      ..deleteError = const ApiException(
        code: 'budgets.not_found',
        message: 'Bulunamadı.',
        statusCode: 404,
      );
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await controller.load();
    expect(await controller.delete('budget'), isFalse);
    expect(controller.items, hasLength(1));
    expect(controller.errorMessage, 'Bulunamadı.');
  });

  test('copies the previous month and skips categories already set', () async {
    final repository = _FakeBudgetRepository()
      ..pages[(2026, 7)] = [
        _budget(id: 'july-market', categoryId: 'market', limit: '500.0000'),
        _budget(id: 'july-fuel', categoryId: 'fuel', limit: '800.0000'),
      ]
      ..pages[(2026, 8)] = [
        _budget(id: 'august-fuel', categoryId: 'fuel', limit: '900.0000'),
      ];
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await controller.load();
    await controller.copyFromPreviousMonth();

    expect(repository.createdInputs, hasLength(1));
    final copied = repository.createdInputs.single;
    expect(copied.categoryId, 'market');
    expect(copied.limit, '500.0000');
    expect(copied.year, 2026);
    expect(copied.month, 8);
    expect(controller.successMessage, '1 bütçe geçen aydan kopyalandı.');
    // Bu ayda zaten değiştirilmiş limit ezilmez.
    expect(
      controller.items?.firstWhere((item) => item.categoryId == 'fuel').limit,
      '900.0000',
    );
  });

  test('says so when the previous month has nothing to copy', () async {
    final repository = _FakeBudgetRepository();
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await controller.load();
    await controller.copyFromPreviousMonth();
    expect(repository.createdInputs, isEmpty);
    expect(
      controller.successMessage,
      'Geçen aydan kopyalanacak bütçe bulunamadı.',
    );
  });

  test('never sends scope for a user who cannot see the dimension', () async {
    final repository = _FakeBudgetRepository()
      ..pages[(2026, 7)] = [
        _budget(
          id: 'july',
          categoryId: 'market',
          scope: TransactionScope.business,
        ),
      ];
    final controller = BudgetsController(
      repository,
      scopeController: ScopeController(),
      initialMonth: DateTime(2026, 8),
    );
    await controller.load();
    expect(controller.isScopeVisible, isFalse);
    await controller.copyFromPreviousMonth();
    expect(repository.createdInputs.single.scope, isNull);
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
  ApiException? deleteError;
  final updateCompleter = Completer<BudgetItem>();
  final deletedIds = <String>[];
  final createdInputs = <CreateBudgetInput>[];

  /// `(yıl, ay)` başına dönecek liste; verilmeyen ay boş döner.
  final Map<(int, int), List<BudgetItem>> pages = {};

  @override
  Future<List<BudgetItem>> list(int year, int month) async {
    listCalls++;
    if (listError case final error?) throw error;
    return pages[(year, month)] ?? const [];
  }

  @override
  Future<BudgetItem> update(String id, String limit) {
    updateCalls++;
    return updateCompleter.future;
  }

  @override
  Future<BudgetItem> create(CreateBudgetInput input) async {
    createdInputs.add(input);
    return _budget(
      id: 'created-${input.categoryId}',
      categoryId: input.categoryId,
      limit: input.limit,
      scope: input.scope ?? TransactionScope.personal,
    );
  }

  @override
  Future<void> delete(String id) async {
    if (deleteError case final error?) throw error;
    deletedIds.add(id);
  }

  @override
  Future<List<BudgetSpendingLine>> listSpending(BudgetItem budget) async =>
      const [];

  @override
  Future<List<BudgetCategory>> listExpenseCategories() async => const [];
}

BudgetItem _budget({
  String id = 'budget',
  String categoryId = 'category',
  String limit = '100.0000',
  String spent = '50.0000',
  String remaining = '50.0000',
  String exceeded = '0.0000',
  TransactionScope scope = TransactionScope.personal,
}) => BudgetItem(
  id: id,
  categoryId: categoryId,
  categoryName: 'Market',
  limit: limit,
  spent: spent,
  remaining: remaining,
  exceeded: exceeded,
  currency: 'TRY',
  scope: scope,
  year: 2026,
  month: 8,
);
