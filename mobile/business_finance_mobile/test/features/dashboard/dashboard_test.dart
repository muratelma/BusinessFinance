import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_page.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';
import '../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:provider/provider.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';

void main() {
  testWidgets('özet ekranı erişilebilirlik kapısını geçer', (tester) async {
    final viewModel = DashboardViewModel(
      FakeDashboardDataSource(DashboardReport.fromJson(_reportJson())),
      now: () => DateTime(2026, 8, 9),
    );
    await viewModel.load();

    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: ChangeNotifierProvider.value(
          value: viewModel,
          child: const DashboardPage(),
        ),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  test('dashboard model preserves backend string money values', () {
    final report = DashboardReport.fromJson(_reportJson());

    expect(report.totalIncome, '500.0000');
    expect(report.net, '375.0000');
    expect(report.categoryExpenses.single.amount, '125.0000');
  });

  testWidgets('dashboard renders API totals without recalculating them', (
    tester,
  ) async {
    final viewModel = DashboardViewModel(
      FakeDashboardDataSource(DashboardReport.fromJson(_reportJson())),
      now: () => DateTime(2026, 8, 9),
    );
    await viewModel.load();

    // 2.0x ölçekte özet kartları bilinçli olarak tek sütuna düşer, bu yüzden
    // içerik uzar. Test yüzeyi bu yüksekliği karşılayacak kadar uzun tutulur;
    // amaç düzeni değil, toplamların olduğu gibi gösterildiğini doğrulamak.
    tester.view.physicalSize = const Size(400, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: MediaQuery(
          data: const MediaQueryData(textScaler: TextScaler.linear(2)),
          child: ChangeNotifierProvider.value(
            value: viewModel,
            child: const DashboardPage(),
          ),
        ),
      ),
    );

    expect(find.text('₺500,00'), findsOneWidget);
    expect(find.text('₺125,00'), findsWidgets);
    expect(find.text('₺375,00'), findsOneWidget);
    expect(find.text('Market'), findsOneWidget);
    expect(find.text('Nakit'), findsOneWidget);
  });

  testWidgets('dashboard announces loading and summary names with values', (
    tester,
  ) async {
    final semantics = tester.ensureSemantics();
    final source = _PendingDashboardDataSource();
    final viewModel = DashboardViewModel(source);
    final loading = viewModel.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ChangeNotifierProvider.value(
          value: viewModel,
          child: const DashboardPage(),
        ),
      ),
    );

    expect(find.bySemanticsLabel('Finansal özet yükleniyor'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);

    source.complete(DashboardReport.fromJson(_reportJson()));
    await loading;
    await tester.pump();

    expect(find.bySemanticsLabel('Gelir: ₺500,00'), findsOneWidget);
    expect(find.bySemanticsLabel('Gider: ₺125,00'), findsOneWidget);
    // Hero kart bağlamını da duyurur: "Net" tek başına neyin neti olduğunu
    // söylemiyordu.
    expect(
      find.bySemanticsLabel('Bu ayın neti: ₺375,00. Gelir eksi gider'),
      findsOneWidget,
    );
    semantics.dispose();
  });

  test('dashboard reloads when an account or transaction changes', () async {
    final source = FakeDashboardDataSource(
      DashboardReport.fromJson(_reportJson()),
    );
    final changes = FinancialDataChanges();
    final viewModel = DashboardViewModel(source, changes: changes);
    await viewModel.load();
    expect(source.calls, 1);

    changes.accountsChanged();
    await Future<void>.delayed(Duration.zero);
    expect(source.calls, 2);

    changes.transactionsChanged();
    await Future<void>.delayed(Duration.zero);
    expect(source.calls, 3);
  });
}

Map<String, dynamic> _reportJson() => {
  'year': 2026,
  'month': 8,
  'totalIncome': '500.0000',
  'totalExpense': '125.0000',
  'net': '375.0000',
  'currency': 'TRY',
  'categoryExpenseSlices': [
    {
      'categoryId': '11111111-1111-1111-1111-111111111111',
      'categoryName': 'Market',
      'amount': '125.0000',
    },
  ],
  'categoryExpenses': [
    {
      'categoryId': '11111111-1111-1111-1111-111111111111',
      'categoryName': 'Market',
      'amount': '125.0000',
    },
  ],
  'accountBalances': [
    {
      'accountId': '22222222-2222-2222-2222-222222222222',
      'accountName': 'Nakit',
      'balance': '475.0000',
    },
  ],
};

class FakeDashboardDataSource implements DashboardDataSource {
  FakeDashboardDataSource(this.report);

  final DashboardReport report;
  int calls = 0;

  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) async {
    calls++;
    return report;
  }

  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) => Future.error(Exception('gelişmiş rapor bu testte yapılandırılmadı'));
}

class _PendingDashboardDataSource implements DashboardDataSource {
  final _completer = Completer<DashboardReport>();

  void complete(DashboardReport report) => _completer.complete(report);

  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) => _completer.future;
  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) => Future.error(Exception('gelişmiş rapor bu testte yapılandırılmadı'));
}
