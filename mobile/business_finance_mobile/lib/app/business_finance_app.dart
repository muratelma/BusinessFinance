import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../core/localization/app_locale.dart';
import '../core/presentation/financial_data_changes.dart';
import '../core/presentation/scope_controller.dart';
import '../core/routing/app_router.dart';
import '../core/theme/app_theme.dart';
import '../features/account/presentation/account_status_controller.dart';
import '../features/auth/presentation/auth_controller.dart';
import '../features/dashboard/presentation/dashboard_view_model.dart';
import '../features/profile/data/profile_repository.dart';
import '../features/reminders/presentation/reminder_controller.dart';
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
                     scopeController: dependencies.scopeController,
                   ),
             authRepository: dependencies?.authRepository,
             userAccountRepository: dependencies?.userAccountRepository,
             accountRepository: dependencies?.accountRepository,
             budgetRepository: dependencies?.budgetRepository,
             categoryRepository: dependencies?.categoryRepository,
             transactionRepository: dependencies?.transactionRepository,
             activityRepository: dependencies?.activityRepository,
             financialDataChanges: dependencies?.financialDataChanges,
             scopeController: dependencies?.scopeController,
             financeRepository: dependencies?.financeRepository,
             planningRepository: dependencies?.planningRepository,
             dataToolsRepository: dependencies?.dataToolsRepository,
             debtRepository: dependencies?.debtRepository,
             counterpartyRepository: dependencies?.counterpartyRepository,
             goalRepository: dependencies?.goalRepository,
             obligationRepository: dependencies?.obligationRepository,
             cashRepository: dependencies?.cashRepository,
             posRepository: dependencies?.posRepository,
             dayCloseRepository: dependencies?.dayCloseRepository,
             taxRepository: dependencies?.taxRepository,
             receiptRepository: dependencies?.receiptRepository,
             receiptImageSource: dependencies?.receiptImageSource,
             receiptImageNormalizer: dependencies?.receiptImageNormalizer,
             receiptPreferences: dependencies?.receiptPreferences,
             reminderController: dependencies?.reminderController,
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

    final dependencies = _dependencies;
    return MultiProvider(
      providers: [
        ChangeNotifierProvider.value(value: authController),
        if (dependencies != null) ...[
          Provider.value(value: dependencies.apiClient),
          ChangeNotifierProvider.value(value: dependencies.scopeController),
          ChangeNotifierProvider.value(
            value: dependencies.accountStatusController,
          ),
          Provider<ProfileRepositoryContract>.value(
            value: dependencies.profileRepository,
          ),
        ],
      ],
      child: dependencies == null
          ? app
          : _ScopeSessionBinder(
              authController: authController,
              scopeController: dependencies.scopeController,
              accountStatusController: dependencies.accountStatusController,
              reminderController: dependencies.reminderController,
              financialDataChanges: dependencies.financialDataChanges,
              child: app,
            ),
    );
  }
}

/// Kapsam anahtarını oturuma bağlar.
///
/// Kompozisyon kökündedir çünkü hem kimlik hem kapsam denetimini tanıması
/// gereken tek yer burasıdır; `ScopeController` auth özelliğini, `AuthController`
/// da kapsamı tanımaz.
///
/// Oturum kapanınca seçim **unutulur**: aynı cihazdan giren ikinci kullanıcı
/// birincisinin anahtar konumunu ve işletme cevabını devralmamalı.
class _ScopeSessionBinder extends StatefulWidget {
  const _ScopeSessionBinder({
    required this.authController,
    required this.scopeController,
    required this.accountStatusController,
    required this.reminderController,
    required this.financialDataChanges,
    required this.child,
  });

  final AuthController authController;
  final ScopeController scopeController;
  final AccountStatusController accountStatusController;

  /// Hatırlatma listesi burada tazelenir: ayar ekranı kapalıyken de ödenmiş
  /// bir kalemin bildirimi düşmeli.
  final ReminderController reminderController;
  final FinancialDataChanges financialDataChanges;
  final Widget child;

  @override
  State<_ScopeSessionBinder> createState() => _ScopeSessionBinderState();
}

class _ScopeSessionBinderState extends State<_ScopeSessionBinder> {
  int _plannedRevision = 0;

  @override
  void initState() {
    super.initState();
    _plannedRevision = widget.financialDataChanges.planningRevision;
    widget.authController.addListener(_handleAuthChanged);
    widget.financialDataChanges.addListener(_handleDataChanged);
    _handleAuthChanged();
  }

  void _handleAuthChanged() {
    switch (widget.authController.status) {
      case AuthStatus.authenticated:
        widget.scopeController.ensureLoaded();
        widget.accountStatusController.ensureLoaded();
        widget.reminderController.ensureLoaded();
      case AuthStatus.unauthenticated:
        widget.scopeController.forget();
        widget.accountStatusController.forget();
        widget.reminderController.forget();
      case AuthStatus.restoring:
        break;
    }
  }

  /// Yalnız **planlanan** görünüm değiştiğinde yeniden kurulur.
  ///
  /// Her mutation'a bağlansaydı, hatırlatmayı hiç ilgilendirmeyen bir gün sonu
  /// sayımı bile telefonun bütün bildirimlerini silip yeniden yazdırırdı.
  void _handleDataChanged() {
    final revision = widget.financialDataChanges.planningRevision;
    if (revision == _plannedRevision) return;
    _plannedRevision = revision;
    widget.reminderController.sync();
  }

  @override
  void dispose() {
    widget.authController.removeListener(_handleAuthChanged);
    widget.financialDataChanges.removeListener(_handleDataChanged);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => widget.child;
}
