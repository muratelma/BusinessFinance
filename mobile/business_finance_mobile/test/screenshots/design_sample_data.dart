import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/account/data/account_models.dart';
import 'package:business_finance_mobile/features/account/data/account_repository.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'package:flutter_test/flutter_test.dart';

/// Tasarım teslim paketinin örnek verisi (`screens/Screens.jsx` → `DATA`,
/// `OzetV4.jsx` → `OZ_SCOPE`). Ekran görüntüleri tasarımla aynı sayılarla
/// çizilir ki fark yalnız yerleşimden gelsin.
final designToday = DateTime(2026, 9, 25);

class DesignDashboardSource implements DashboardDataSource {
  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) async {
    final data = _scopeData(scope);
    return DashboardReport(
      year: year,
      month: month,
      totalIncome: data.income,
      totalExpense: data.expense,
      net: data.net,
      currency: 'TRY',
      categoryExpenses: [
        for (var i = 0; i < data.categoryCount; i++)
          CategoryExpense(
            categoryId: 'c$i',
            categoryName: 'Kategori $i',
            canonicalName: 'Kategori $i',
            amount: '1.0000',
          ),
      ],
      categoryExpenseSlices: [
        for (final (name, amount) in data.slices)
          CategoryExpenseSlice(
            categoryId: name == 'Diğer' ? null : name,
            categoryName: name,
            canonicalName: name,
            amount: amount,
          ),
      ],
      accountBalances: const [
        AccountBalance(
          accountId: 'a1',
          accountName: 'Birikim Hesabi',
          balance: '65000.0000',
          type: 'bank',
        ),
        AccountBalance(
          accountId: 'a2',
          accountName: 'Dukkan Kasasi',
          balance: '23185.0000',
          type: 'cash',
        ),
        AccountBalance(
          accountId: 'a3',
          accountName: 'Sahsi Cuzdan',
          balance: '-2290.0000',
          type: 'cash',
        ),
        AccountBalance(
          accountId: 'a4',
          accountName: 'Ziraat Vadesiz',
          balance: '-1853.1300',
          type: 'bank',
        ),
      ],
      scopeBreakdown: scope != null
          ? null
          : const MonthlyScopeBreakdown(
              business: ScopeTotals(
                income: '66900.0000',
                expense: '60747.2100',
                net: '6152.7900',
              ),
              personal: ScopeTotals(
                income: '1650.0000',
                expense: '6769.0000',
                net: '-5119.0000',
              ),
            ),
    );
  }

  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) async => AdvancedReport(
    asOfDate: '2026-09-30',
    currency: 'TRY',
    liquidAssets: '62041.8700',
    creditCardDebt: '16868.0000',
    receivableDebt: '13300.0000',
    payableDebt: '25715.4600',
    netWorth: '55012.0400',
    moneyInTransit: '22253.6300',
    totalAssets: '97595.5000',
    totalLiabilities: '42583.4600',
    nextTransitDate: '2026-09-27',
    netChange: '11176.5400',
    currentPeriod: const PeriodTotals(
      year: 2026,
      month: 9,
      income: '0.0000',
      expense: '0.0000',
      net: '0.0000',
    ),
    previousPeriod: const PeriodTotals(
      year: 2026,
      month: 8,
      income: '0.0000',
      expense: '0.0000',
      net: '0.0000',
    ),
    cashFlowTrend: const [],
    budgetVariances: _scopeData(scope).budgets,
    futureLoad: '0.0000',
    accountDistribution: const [],
    cardDistribution: const [],
  );
}

class DesignActivities extends Fake implements ActivityRepositoryContract {
  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async {
    final data = _scopeData(scope);
    return PlannedActivityPage(
      asOfDate: '2026-09-25',
      daysAhead: 7,
      totalCount: data.upcoming.length + data.overdueCount,
      upcomingOutgoingTotal: data.upcomingTotal,
      items: [
        for (var i = 0; i < data.overdueCount; i++)
          _planned(
            'Gecikmiş $i',
            i == 0 ? data.oldestOverdue : '2026-09-0${i + 1}',
            '100.0000',
            PlannedTiming.overdue,
          ),
        for (final (title, due, amount) in data.upcoming)
          _planned(title, due, amount, PlannedTiming.upcoming),
      ],
    );
  }
}

class DesignAccount extends Fake implements AccountRepositoryContract {
  @override
  Future<UserAccount> read() async => UserAccount(
    userId: 'u1',
    email: 'esnaf@ornek.com',
    emailConfirmed: false,
    createdAtUtc: DateTime.utc(2026, 3, 14),
    activeSessionCount: 3,
  );
}

PlannedActivity _planned(
  String title,
  String due,
  String amount,
  PlannedTiming timing,
) => PlannedActivity(
  plannedActivityId: title,
  plannedKind: PlannedKind.recurringOccurrence,
  effect: ActivityEffect.expense,
  timing: timing,
  readiness: PlannedReadiness.ready,
  actionKind: PlannedAction.realize,
  dueDate: due,
  amount: amount,
  currency: 'TRY',
  title: title,
  isProjected: false,
  isPaymentObligation: true,
);

BudgetVarianceItem _budget(
  String name,
  String spent,
  String limit,
  String remaining,
) => BudgetVarianceItem(
  categoryName: name,
  canonicalName: name,
  limit: limit,
  spent: spent,
  remaining: remaining,
  isExceeded: remaining.startsWith('-'),
);

final _businessBudgets = [
  _budget('Ticari mal alımı', '26507.2000', '25000.0000', '-1507.2000'),
  _budget('Araç ve yakıt', '3260.0000', '3000.0000', '-260.0000'),
  _budget('Elektrik, su, doğalgaz', '3950.8800', '4500.0000', '549.1200'),
  _budget('Kırtasiye', '640.0000', '1000.0000', '360.0000'),
];

final _personalBudgets = [
  _budget('Ulaşım', '1100.0000', '1000.0000', '-100.0000'),
  _budget('Market ve gıda', '2480.0000', '3000.0000', '520.0000'),
];

typedef _ScopeData = ({
  String income,
  String expense,
  String net,
  int categoryCount,
  List<(String, String)> slices,
  List<BudgetVarianceItem> budgets,
  List<(String, String, String)> upcoming,
  String upcomingTotal,
  int overdueCount,
  String oldestOverdue,
});

_ScopeData _scopeData(TransactionScope? scope) => switch (scope) {
  null => (
    income: '68550.0000',
    expense: '67516.2100',
    net: '1033.7900',
    categoryCount: 14,
    slices: const [
      ('Ticari mal alımı', '26507.2000'),
      ('Personel ücretleri', '22000.0000'),
      ('SGK ve vergi ödemeleri', '4120.0000'),
      ('Elektrik, su, doğalgaz', '3950.8800'),
      ('Diğer', '10938.1300'),
    ],
    budgets: [..._businessBudgets, ..._personalBudgets],
    upcoming: const [
      ('Muhtasar beyanı', '2026-09-26', '2800.0000'),
      ('Telefon faturası', '2026-09-27', '450.0000'),
      ('KDV beyanı', '2026-09-28', '6250.0000'),
      ('Ev kirası', '2026-09-29', '8500.0000'),
      ('SGK / Bağkur primi', '2026-09-30', '5400.0000'),
    ],
    upcomingTotal: '23400.0000',
    overdueCount: 6,
    oldestOverdue: '2026-08-10',
  ),
  TransactionScope.business => (
    income: '66900.0000',
    expense: '60747.2100',
    net: '6152.7900',
    categoryCount: 9,
    slices: const [
      ('Ticari mal alımı', '26507.2000'),
      ('Personel ücretleri', '22000.0000'),
      ('SGK ve vergi ödemeleri', '4120.0000'),
      ('Elektrik, su, doğalgaz', '3950.8800'),
      ('Diğer', '4169.1300'),
    ],
    budgets: _businessBudgets,
    upcoming: const [
      ('Muhtasar beyanı', '2026-09-26', '2800.0000'),
      ('KDV beyanı', '2026-09-28', '6250.0000'),
      ('SGK / Bağkur primi', '2026-09-30', '5400.0000'),
    ],
    upcomingTotal: '14450.0000',
    overdueCount: 4,
    oldestOverdue: '2026-08-10',
  ),
  TransactionScope.personal => (
    income: '1650.0000',
    expense: '6769.0000',
    net: '-5119.0000',
    categoryCount: 5,
    slices: const [
      ('Market ve gıda', '2480.0000'),
      ('Sağlık', '1250.0000'),
      ('Ulaşım', '1100.0000'),
      ('Eğitim', '900.0000'),
      ('Diğer', '1039.0000'),
    ],
    budgets: _personalBudgets,
    upcoming: const [
      ('Telefon faturası', '2026-09-27', '450.0000'),
      ('Ev kirası', '2026-09-29', '8500.0000'),
    ],
    upcomingTotal: '8950.0000',
    overdueCount: 2,
    oldestOverdue: '2026-09-05',
  ),
};
