import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';

DashboardViewModel testDashboardViewModel() {
  final viewModel = DashboardViewModel(
    _FakeDashboardDataSource(),
    now: () => DateTime(2026, 8, 9),
  );
  viewModel.load();
  return viewModel;
}

class _FakeDashboardDataSource implements DashboardDataSource {
  @override
  Future<DashboardReport> getMonthly(int year, int month) async =>
      DashboardReport(
        year: year,
        month: month,
        totalIncome: '0.0000',
        totalExpense: '0.0000',
        net: '0.0000',
        currency: 'TRY',
        categoryExpenses: const [],
        categoryExpenseSlices: const [],
        accountBalances: const [],
      );
  @override
  Future<AdvancedReport> getAdvanced(int year, int month) =>
      Future.error(Exception('gelişmiş rapor bu testte yapılandırılmadı'));
}
