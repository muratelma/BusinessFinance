import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../core/localization/app_locale.dart';
import '../core/routing/app_router.dart';
import '../core/theme/app_theme.dart';
import '../features/auth/presentation/auth_controller.dart';
import '../features/dashboard/presentation/dashboard_view_model.dart';
import 'app_dependencies.dart';

class BusinessFinanceApp extends StatelessWidget {
  BusinessFinanceApp({
    super.key,
    AppDependencies? dependencies,
    GoRouter? router,
    AuthController? authController,
  }) : _dependencies = dependencies,
       _authController = authController ?? dependencies?.authController,
       router =
           router ??
           createAppRouter(
             authController: authController ?? dependencies?.authController,
             dashboardViewModelFactory: dependencies == null
                 ? null
                 : () => DashboardViewModel(
                     dependencies.dashboardRepository,
                     activityRepository: dependencies.activityRepository,
                     changes: dependencies.financialDataChanges,
                   ),
             accountRepository: dependencies?.accountRepository,
             budgetRepository: dependencies?.budgetRepository,
             categoryRepository: dependencies?.categoryRepository,
             transactionRepository: dependencies?.transactionRepository,
             activityRepository: dependencies?.activityRepository,
             financialDataChanges: dependencies?.financialDataChanges,
             financeRepository: dependencies?.financeRepository,
             planningRepository: dependencies?.planningRepository,
             dataToolsRepository: dependencies?.dataToolsRepository,
             debtRepository: dependencies?.debtRepository,
             goalRepository: dependencies?.goalRepository,
             receiptRepository: dependencies?.receiptRepository,
             receiptImageSource: dependencies?.receiptImageSource,
             receiptImageNormalizer: dependencies?.receiptImageNormalizer,
             receiptPreferences: dependencies?.receiptPreferences,
           );

  final GoRouter router;
  final AppDependencies? _dependencies;
  final AuthController? _authController;

  @override
  Widget build(BuildContext context) {
    final app = MaterialApp.router(
      title: 'Kişisel Bütçe',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light(),
      darkTheme: AppTheme.dark(),
      themeMode: ThemeMode.system,
      locale: AppLocale.turkish,
      supportedLocales: AppLocale.supported,
      localizationsDelegates: AppLocale.delegates,
      routerConfig: router,
    );

    final authController = _authController;
    if (authController == null) {
      return app;
    }

    return MultiProvider(
      providers: [
        ChangeNotifierProvider.value(value: authController),
        if (_dependencies case final dependencies?)
          Provider.value(value: dependencies.apiClient),
      ],
      child: app,
    );
  }
}
