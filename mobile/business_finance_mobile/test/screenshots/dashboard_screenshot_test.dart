import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/account/presentation/account_status_controller.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_page.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'design_sample_data.dart';
import 'screenshot_harness.dart';

void main() {
  Future<Widget> page(TransactionScope? scope) async {
    final scopeController = ScopeController(readHasBusiness: () async => true);
    await scopeController.ensureLoaded();
    await scopeController.select(scope);
    final viewModel = DashboardViewModel(
      DesignDashboardSource(),
      activityRepository: DesignActivities(),
      scopeController: scopeController,
      now: () => designToday,
    );
    await viewModel.load();
    final status = AccountStatusController(DesignAccount());
    await status.refresh();
    return MultiProvider(
      providers: [
        ChangeNotifierProvider.value(value: viewModel),
        ChangeNotifierProvider<AccountStatusController?>.value(value: status),
      ],
      child: const DashboardPage(),
    );
  }

  testWidgets('01 özet · hepsi', (tester) async {
    final widget = await tester.runAsync(() => page(null));
    await captureScreen(tester, '01-ozet-hepsi', widget!);
  }, skip: !screenshotsEnabled);

  testWidgets('02 özet · işletme', (tester) async {
    final widget = await tester.runAsync(() => page(TransactionScope.business));
    await captureScreen(tester, '02-ozet-isletme', widget!);
  }, skip: !screenshotsEnabled);

  testWidgets('03 özet · şahsi', (tester) async {
    final widget = await tester.runAsync(() => page(TransactionScope.personal));
    await captureScreen(tester, '03-ozet-sahsi', widget!);
  }, skip: !screenshotsEnabled);

  testWidgets('04 özet · alt', (tester) async {
    final widget = await tester.runAsync(() => page(null));
    await captureScreen(
      tester,
      '04-ozet-alt',
      widget!,
      before: (tester) async {
        await tester.drag(find.byType(ListView), const Offset(0, -1000));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('04b özet · en alt', (tester) async {
    final widget = await tester.runAsync(() => page(null));
    await captureScreen(
      tester,
      '04b-ozet-en-alt',
      widget!,
      before: (tester) async {
        await tester.drag(find.byType(ListView), const Offset(0, -2400));
      },
    );
  }, skip: !screenshotsEnabled);
}
