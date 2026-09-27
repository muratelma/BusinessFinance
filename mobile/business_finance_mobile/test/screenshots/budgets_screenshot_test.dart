import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_models.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_repository.dart';
import 'package:business_finance_mobile/features/budgets/presentation/budgets_page.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

void main() {
  testWidgets('05 bütçeler', (tester) async {
    final scope = ScopeController(readHasBusiness: () async => true);
    await tester.runAsync(scope.ensureLoaded);
    await captureScreen(
      tester,
      '05-butceler',
      BudgetsPage(repository: _DesignBudgets(), scopeController: scope),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);
}

class _DesignBudgets extends Fake implements BudgetRepositoryContract {
  @override
  Future<List<BudgetItem>> list(int year, int month) async => [
    _item(
      'Ticari mal alımı',
      '26507.2000',
      '25000.0000',
      '0.0000',
      '1507.2000',
    ),
    _item('Araç ve yakıt', '3260.0000', '3000.0000', '0.0000', '260.0000'),
    _item(
      'Elektrik, su, doğalgaz',
      '3950.8800',
      '4500.0000',
      '549.1200',
      '0.0000',
    ),
    _item('Kırtasiye', '640.0000', '1000.0000', '360.0000', '0.0000'),
  ];

  BudgetItem _item(
    String name,
    String spent,
    String limit,
    String remaining,
    String exceeded,
  ) => BudgetItem(
    id: name,
    categoryId: name,
    categoryName: name,
    limit: limit,
    spent: spent,
    remaining: remaining,
    exceeded: exceeded,
    currency: 'TRY',
    scope: TransactionScope.business,
    year: 2026,
    month: 9,
  );
}
