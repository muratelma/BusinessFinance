import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../network/api_client.dart';
import '../network/api_exception.dart';
import '../models/transaction_scope.dart';
import '../presentation/financial_data_changes.dart';
import '../presentation/scope_controller.dart';
import '../../features/accounts/data/account_repository.dart';
import '../../features/accounts/presentation/account_directory.dart';
import '../../features/accounts/presentation/account_form_page.dart';
import '../../features/accounts/presentation/account_settings_page.dart';
import '../../features/finance_pages/data/finance_page_repository.dart';
import '../../features/finance_pages/presentation/finance_page_controller.dart';
import '../../features/finance_pages/presentation/finance_report_page.dart';
import '../../features/finance_pages/presentation/accounts_overview_page.dart';
import '../../features/finance_pages/presentation/transfer_page.dart';
import '../../features/accounts/presentation/accounts_view_model.dart';
import '../../features/account/data/account_repository.dart' as user_account;
import '../../features/account/presentation/account_page.dart';
import '../../features/auth/data/auth_repository.dart';
import '../../features/auth/presentation/auth_controller.dart';
import '../../features/auth/presentation/auth_flow.dart';
import '../../features/auth/presentation/auth_restoring_view.dart';
import '../../features/budgets/data/budget_repository.dart';
import '../../features/budgets/presentation/budgets_page.dart';
import '../../features/categories/data/category_repository.dart';
import '../../features/categories/presentation/categories_page.dart';
import '../../features/categories/presentation/category_create_page.dart';
import '../../features/categories/presentation/categories_view_model.dart';
import '../../features/dashboard/presentation/dashboard_page.dart';
import '../../features/dashboard/presentation/dashboard_view_model.dart';
import '../../features/more/presentation/more_page.dart';
import '../../features/shell/presentation/main_shell.dart';
import '../../features/activities/data/activity_repository.dart';
import '../../features/activities/presentation/activity_feed_page.dart';
import '../../features/activities/presentation/planned_activity_page.dart';
import '../../features/activities/data/receipt_fee_writer.dart';
import '../../features/activities/presentation/quick_add_controller.dart';
import '../../features/activities/presentation/quick_add_form_page.dart';
import '../../features/activities/presentation/quick_add_models.dart';
import '../../features/activities/presentation/quick_add_navigation.dart';
import '../../features/transactions/data/transaction_repository.dart';
import '../../features/transactions/presentation/transactions_page.dart';
import '../../features/cards/data/finance_repository.dart';
import '../../features/cards/presentation/finance_page.dart';
import '../../features/planning/data/planning_repository.dart';
import '../../features/planning/presentation/planning_page.dart';
import '../../features/data_tools/data/data_tools_repository.dart';
import '../../features/data_tools/presentation/data_tools_page.dart';
import '../../features/counterparties/data/counterparty_repository.dart';
import '../../features/counterparties/presentation/counterparties_page.dart';
import '../../features/debts/data/debt_repository.dart';
import '../../features/debts/presentation/debts_page.dart';
import '../../features/debts/presentation/lending_prefill.dart';
import '../../features/goals/data/goal_repository.dart';
import '../../features/goals/presentation/goals_page.dart';
import '../../features/cash/data/cash_repository.dart';
import '../../features/cash/presentation/cash_controller.dart';
import '../../features/cash/presentation/cash_page.dart';
import '../../features/taxes/data/tax_repository.dart';
import '../../features/taxes/presentation/tax_controller.dart';
import '../../features/taxes/presentation/tax_tracking_page.dart';
import '../../features/day_close/data/day_close_repository.dart';
import '../../features/day_close/presentation/day_close_controller.dart';
import '../../features/pos/data/pos_repository.dart';
import '../../features/pos/presentation/pos_controller.dart';
import '../../features/obligations/data/obligation_repository.dart';
import '../../features/obligations/presentation/obligation_controller.dart';
import '../../features/obligations/presentation/obligation_form_page.dart';
import '../../features/obligations/presentation/obligation_prefill.dart';
import '../../features/obligations/presentation/obligations_page.dart';
import '../../features/receipts/data/receipt_image_source.dart';
import '../../features/cards/presentation/transfer_prefill.dart';
import '../../features/receipts/data/receipt_models.dart';
import '../../features/receipts/presentation/bank_document_decision_page.dart';
import '../../features/receipts/presentation/receipt_prefill.dart';
import '../../features/receipts/data/receipt_photo.dart';
import '../../features/receipts/data/receipt_preferences.dart';
import '../../features/receipts/data/receipt_repository.dart';
import '../../features/receipts/presentation/receipt_scan_controller.dart';
import '../../features/receipts/presentation/receipt_scan_page.dart';
import '../../features/receipts/presentation/invoice_decision_page.dart';
import '../../features/receipts/presentation/refund_decision_page.dart';
import '../../features/reminders/presentation/reminder_controller.dart';
import '../../features/reminders/presentation/reminder_settings_page.dart';
import '../widgets/app_state_views.dart';
import 'app_locations.dart';

// Yol sabitleri kendi dosyasında; buradan yeniden dışa veriliyor ki bu dosyayı
// import eden çağrılar değişmesin.
export 'app_locations.dart';

GoRouter createAppRouter({
  String initialLocation = '/dashboard',
  AuthController? authController,
  AuthSessionRepository? authRepository,
  user_account.AccountRepositoryContract? userAccountRepository,
  DashboardViewModel Function()? dashboardViewModelFactory,
  AccountRepository? accountRepository,
  BudgetRepositoryContract? budgetRepository,
  CategoryRepository? categoryRepository,
  TransactionRepositoryContract? transactionRepository,
  ActivityRepositoryContract? activityRepository,
  FinancePageRepositoryContract? financePageRepository,
  FinancialDataChanges? financialDataChanges,
  ScopeController? scopeController,
  FinanceRepositoryContract? financeRepository,
  PlanningRepositoryContract? planningRepository,
  DataToolsRepositoryContract? dataToolsRepository,
  DebtRepositoryContract? debtRepository,
  CounterpartyRepositoryContract? counterpartyRepository,
  GoalRepositoryContract? goalRepository,
  ObligationRepositoryContract? obligationRepository,
  CashRepositoryContract? cashRepository,
  PosRepositoryContract? posRepository,
  DayCloseRepositoryContract? dayCloseRepository,
  TaxRepositoryContract? taxRepository,
  ReceiptRepositoryContract? receiptRepository,
  ReceiptImageSourceContract? receiptImageSource,
  ReceiptImageNormalizerContract? receiptImageNormalizer,
  ReceiptPreferencesContract? receiptPreferences,
  ReminderController? reminderController,
}) {
  return GoRouter(
    initialLocation: initialLocation,
    refreshListenable: authController,
    redirect: authController == null
        ? null
        : (context, state) => _authRedirect(authController, state),
    routes: [
      GoRoute(
        path: '/restoring',
        pageBuilder: _pageBuilder(const AuthRestoringView()),
      ),
      // Üç kimlik yolu tek ekrana açılır. Sayfa anahtarı ortaktır: yol değişse
      // de ekran (ve açılış animasyonu) yeniden kurulmaz, paneller kendi
      // içinde kayar.
      GoRoute(
        path: '/login',
        pageBuilder: (context, state) =>
            _authPage(state, AuthView.login, authController, authRepository),
      ),
      GoRoute(
        path: '/password-reset',
        pageBuilder: (context, state) =>
            _authPage(state, AuthView.reset, authController, authRepository),
      ),
      GoRoute(
        path: '/register',
        pageBuilder: (context, state) =>
            _authPage(state, AuthView.register, authController, authRepository),
      ),
      StatefulShellRoute.indexedStack(
        builder: (context, state, navigationShell) => MainShell(
          navigationShell: navigationShell,
          scopeController: scopeController,
        ),
        branches: [
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/dashboard',
                pageBuilder: (context, state) {
                  final child = dashboardViewModelFactory == null
                      ? const DashboardPage()
                      : ChangeNotifierProvider(
                          create: (_) {
                            final viewModel = dashboardViewModelFactory();
                            if (authController?.status ==
                                AuthStatus.authenticated) {
                              viewModel.load();
                            }
                            return viewModel;
                          },
                          child: const DashboardPage(),
                        );
                  return _sessionPage(state, child, authController);
                },
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/transactions',
                // The list now reads the unified feed. Creating a movement still
                // goes through the existing form until the shared launcher lands.
                pageBuilder: _sessionPageBuilder(
                  activityRepository == null
                      ? TransactionsPage(
                          repository: transactionRepository,
                          changes: financialDataChanges,
                        )
                      : Builder(
                          builder: (context) => ActivityFeedPage(
                            repository: activityRepository,
                            pageRepository: financePageRepository,
                            changes: financialDataChanges,
                            scopeController: scopeController,
                            posRepository: posRepository,
                            dayCloseRepository: dayCloseRepository,
                            onCreateTransaction: () => openQuickAdd(context),
                            onShowPlanned: () =>
                                context.push('/transactions/planned'),
                          ),
                        ),
                  authController,
                ),
                routes: [
                  GoRoute(
                    path: 'new',
                    pageBuilder: _sessionPageBuilder(
                      TransactionsPage(
                        repository: transactionRepository,
                        changes: financialDataChanges,
                        showCreateOnOpen: true,
                      ),
                      authController,
                    ),
                    routes: [
                      GoRoute(
                        path: 'expense',
                        pageBuilder: _quickAddPageBuilder(
                          isExpense: true,
                          transactionRepository: transactionRepository,
                          financeRepository: financeRepository,
                          financialDataChanges: financialDataChanges,
                          dataToolsRepository: dataToolsRepository,
                          authController: authController,
                          scopeController: scopeController,
                        ),
                      ),
                      // Karar sayfası fiş ekranıyla **kardeş**: dekont
                      // okunduktan sonra ana tutarın ne olduğu burada sorulur.
                      // Üst seviyeye konduğunda buradan gider formuna geçmek
                      // shell zincirini yeniden kuruyor ve Navigator aynı sayfa
                      // anahtarını iki kez kaydedip patlıyordu.
                      GoRoute(
                        // Yol `bankDocumentDecisionLocation` ile aynı olmalı.
                        path: 'bank-document',
                        pageBuilder: (context, state) => _sessionPage(
                          state,
                          state.extra is! ReceiptDraft
                              ? const Scaffold(
                                  body: AppErrorView(
                                    message: 'Okunmuş bir belge bulunamadı.',
                                  ),
                                )
                              : BankDocumentDecisionPage(
                                  draft: state.extra! as ReceiptDraft,
                                  onDecided: (decision, recordFee) =>
                                      _recordBankDocument(
                                        context,
                                        state.extra! as ReceiptDraft,
                                        decision,
                                        recordFee,
                                      ),
                                ),
                          authController,
                        ),
                      ),
                      // Fatura sayfası da kardeş: ödendiyse buradan gider
                      // formuna geçiliyor.
                      GoRoute(
                        // Yol `invoiceDecisionLocation` ile aynı olmalı.
                        path: 'invoice',
                        pageBuilder: (context, state) => _sessionPage(
                          state,
                          state.extra is! ReceiptDraft
                              ? const Scaffold(
                                  body: AppErrorView(
                                    message: 'Okunmuş bir belge bulunamadı.',
                                  ),
                                )
                              : InvoiceDecisionPage(
                                  draft: state.extra! as ReceiptDraft,
                                  onDecided: (isPaid) => _recordInvoice(
                                    context,
                                    state.extra! as ReceiptDraft,
                                    isPaid,
                                  ),
                                ),
                          authController,
                        ),
                      ),
                      GoRoute(
                        // Yol `obligationCreateLocation` ile aynı olmalı.
                        path: 'obligation',
                        pageBuilder: _obligationFormPageBuilder(
                          obligationRepository: obligationRepository,
                          financialDataChanges: financialDataChanges,
                          scopeController: scopeController,
                          authController: authController,
                        ),
                      ),
                      // İade sayfası da fiş ekranıyla kardeş, aynı
                      // Navigator gerekçesiyle.
                      GoRoute(
                        // Yol `refundDecisionLocation` ile aynı olmalı.
                        path: 'refund',
                        pageBuilder: (context, state) => _sessionPage(
                          state,
                          state.extra is! ReceiptDraft
                              ? const Scaffold(
                                  body: AppErrorView(
                                    message: 'Okunmuş bir belge bulunamadı.',
                                  ),
                                )
                              : RefundDecisionPage(
                                  draft: state.extra! as ReceiptDraft,
                                  onCancelExpense: (match) => _recordRefund(
                                    context,
                                    match,
                                    transactionRepository,
                                    financialDataChanges,
                                  ),
                                ),
                          authController,
                        ),
                      ),
                      GoRoute(
                        path: 'receipt',
                        pageBuilder: _receiptScanPageBuilder(
                          receiptRepository: receiptRepository,
                          receiptImageSource: receiptImageSource,
                          receiptImageNormalizer: receiptImageNormalizer,
                          receiptPreferences: receiptPreferences,
                          authController: authController,
                        ),
                      ),
                      // Dekontun kendi yolu: aynı okuma ekranı, ama yön
                      // sormadan ve belge sınıfı baştan bildirilmiş olarak.
                      GoRoute(
                        path: 'bank-slip',
                        pageBuilder: _receiptScanPageBuilder(
                          variant: ReceiptScanVariant.bankSlip,
                          receiptRepository: receiptRepository,
                          receiptImageSource: receiptImageSource,
                          receiptImageNormalizer: receiptImageNormalizer,
                          receiptPreferences: receiptPreferences,
                          authController: authController,
                        ),
                      ),
                      GoRoute(
                        path: 'income',
                        pageBuilder: _quickAddPageBuilder(
                          isExpense: false,
                          transactionRepository: transactionRepository,
                          financeRepository: financeRepository,
                          financialDataChanges: financialDataChanges,
                          dataToolsRepository: dataToolsRepository,
                          authController: authController,
                          scopeController: scopeController,
                        ),
                      ),
                    ],
                  ),
                  GoRoute(
                    path: 'planned',
                    pageBuilder: _sessionPageBuilder(
                      activityRepository == null
                          ? const SizedBox.shrink()
                          : PlannedActivityPageView(
                              repository: activityRepository,

                              changes: financialDataChanges,
                              scopeController: scopeController,
                            ),
                      authController,
                    ),
                  ),
                ],
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/budgets',
                pageBuilder: (context, state) => _sessionPage(
                  state,
                  _AdaptiveThirdDestination(
                    scopeController: scopeController,
                    budgetRepository: budgetRepository,
                    cashRepository: cashRepository,
                    posRepository: posRepository,
                    dayCloseRepository: dayCloseRepository,
                    changes: financialDataChanges,
                  ),
                  authController,
                ),
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/more',
                pageBuilder: _sessionPageBuilder(
                  const MorePage(),
                  authController,
                ),
              ),
            ],
          ),
        ],
      ),
      GoRoute(
        path: accountsLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          financePageRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Hesap servisi yapılandırılmadı.',
                  ),
                )
              : Builder(
                  builder: (pageContext) => AccountsOverviewPage(
                    repository: financePageRepository,
                    changes: financialDataChanges,
                    onCreate: accountRepository == null
                        ? null
                        : () => _openPageAccountForm(
                            pageContext,
                            financePageRepository,
                            accountRepository,
                            financialDataChanges,
                          ),
                  ),
                ),
          authController,
        ),
      ),
      // `:accountId`'den önce: `new` bir hesap kimliği değildir.
      GoRoute(
        path: accountCreateLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          accountRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Hesap servisi yapılandırılmadı.',
                  ),
                )
              : AccountCreatePage(
                  repository: accountRepository,
                  changes: financialDataChanges,
                  initialType: state.uri.queryParameters['type'],
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '$accountsLocation/:accountId',
        pageBuilder: (context, state) => _sessionPage(
          state,
          financePageRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Hesap servisi yapılandırılmadı.',
                  ),
                )
              : Builder(
                  builder: (pageContext) => FinanceReportPage(
                    repository: financePageRepository,
                    kind: FinancePageKind.account,
                    id: state.pathParameters['accountId'],
                    changes: financialDataChanges,
                    activityRepository: activityRepository,
                    posRepository: posRepository,
                    dayCloseRepository: dayCloseRepository,
                    onAccountSettings: accountRepository == null
                        ? null
                        : () => _openPageAccountForm(
                            pageContext,
                            financePageRepository,
                            accountRepository,
                            financialDataChanges,
                            id: state.pathParameters['accountId'],
                          ),
                  ),
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '$accountsLocation/:accountId/in-transit',
        pageBuilder: (context, state) => _sessionPage(
          state,
          financePageRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Hesap servisi yapılandırılmadı.',
                  ),
                )
              : FinanceReportPage(
                  repository: financePageRepository,
                  kind: FinancePageKind.transit,
                  id: state.pathParameters['accountId'],
                  changes: financialDataChanges,
                  activityRepository: activityRepository,
                  posRepository: posRepository,
                  dayCloseRepository: dayCloseRepository,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: transferLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          financePageRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Transfer servisi yapılandırılmadı.',
                  ),
                )
              : TransferPage(
                  repository: financePageRepository,
                  changes: financialDataChanges,
                  prefill: state.extra is TransferPrefill
                      ? state.extra! as TransferPrefill
                      : null,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: categorySummaryLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          financePageRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Kategori servisi yapılandırılmadı.',
                  ),
                )
              : FinanceReportPage(
                  repository: financePageRepository,
                  kind: FinancePageKind.summary,
                  month: _pageMonth(state),
                  income: state.uri.queryParameters['direction'] == 'income',
                  scopeController: scopeController,
                  source: state.uri.queryParameters['source'],
                  changes: financialDataChanges,
                  activityRepository: activityRepository,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '$categorySummaryLocation/:categoryId',
        pageBuilder: (context, state) => _sessionPage(
          state,
          financePageRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Kategori servisi yapılandırılmadı.',
                  ),
                )
              : FinanceReportPage(
                  repository: financePageRepository,
                  kind: FinancePageKind.category,
                  id: state.pathParameters['categoryId'] == 'uncategorized'
                      ? null
                      : state.pathParameters['categoryId'],
                  month: _pageMonth(state),
                  income: state.uri.queryParameters['direction'] == 'income',
                  scope: state.uri.queryParameters['scope'] == null
                      ? null
                      : TransactionScope.fromApi(
                          state.uri.queryParameters['scope']!,
                        ),
                  source: state.uri.queryParameters['source'],
                  changes: financialDataChanges,
                  activityRepository: activityRepository,
                  posRepository: posRepository,
                  dayCloseRepository: dayCloseRepository,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: categoryCreateLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          categoryRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Kategori servisi yapılandırılmadı.',
                  ),
                )
              : CategoryCreatePage(
                  repository: categoryRepository,
                  initialName: state.uri.queryParameters['name'],
                  initialType: state.uri.queryParameters['type'],
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/categories',
        pageBuilder: (context, state) => _sessionPage(
          state,
          categoryRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Kategori servisi yapılandırılmadı.',
                  ),
                )
              : CategoriesPage(
                  viewModel: CategoriesViewModel(categoryRepository),
                  ownsViewModel: true,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/cards',
        pageBuilder: (context, state) => _sessionPage(
          state,
          financeRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Kart ve transfer servisi yapılandırılmadı.',
                  ),
                )
              : FinancePage(
                  repository: financeRepository,
                  financialDataChanges: financialDataChanges,
                  cardPaymentPrefill: state.extra is CardPaymentPrefill
                      ? state.extra! as CardPaymentPrefill
                      : null,
                  installmentPrefill: state.extra is InstallmentPrefill
                      ? state.extra! as InstallmentPrefill
                      : null,
                  recordFee: _feeRecorder(
                    transactionRepository,
                    financeRepository,
                    financialDataChanges,
                  ),
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/counterparties',
        pageBuilder: (context, state) => _sessionPage(
          state,
          counterpartyRepository == null
              ? const Scaffold(
                  body: AppErrorView(message: 'Cari servisi yapılandırılmadı.'),
                )
              : CounterpartiesPage(
                  repository: counterpartyRepository,
                  changes: financialDataChanges,
                  posRepository: posRepository,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/debts',
        pageBuilder: (context, state) => _sessionPage(
          state,
          debtRepository == null
              ? const Scaffold(
                  body: AppErrorView(message: 'Borç servisi yapılandırılmadı.'),
                )
              : DebtsPage(
                  repository: debtRepository,
                  changes: financialDataChanges,
                  lendingPrefill: state.extra is LendingPrefill
                      ? state.extra! as LendingPrefill
                      : null,
                  recordFee: _feeRecorder(
                    transactionRepository,
                    financeRepository,
                    financialDataChanges,
                  ),
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/obligations',
        pageBuilder: (context, state) => _sessionPage(
          state,
          obligationRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Yükümlülük servisi yapılandırılmadı.',
                  ),
                )
              : ObligationsPage(
                  controller: ObligationListController(
                    obligationRepository,
                    changes: financialDataChanges,
                    posRepository: posRepository,
                  ),
                ),
          authController,
        ),
        routes: [
          // Liste shell'in dışında; formu `obligationCreateLocation`'a açmak
          // shell zincirini ikinci kez kurup Navigator'ı aynı sayfa
          // anahtarıyla patlatıyordu. Liste kendi altındaki rotaya açar.
          GoRoute(
            // Yol `obligationListCreateLocation` ile aynı olmalı.
            path: 'new',
            pageBuilder: _obligationFormPageBuilder(
              obligationRepository: obligationRepository,
              financialDataChanges: financialDataChanges,
              scopeController: scopeController,
              authController: authController,
            ),
          ),
        ],
      ),
      GoRoute(
        path: '/more/budgets',
        pageBuilder: _sessionPageBuilder(
          BudgetsPage(
            repository: budgetRepository,
            changes: financialDataChanges,
            scopeController: scopeController,
          ),
          authController,
        ),
      ),
      GoRoute(
        // `Kasa` ekranı: gün sonu sayımı ve POS tahsilatları. Aşama 04 Grup
        // 5'te işletme profilinde ana sekmeye çıkacak; kişisel profilde
        // `Diğer` altında kalacak. İki yerleşimde de aynı rota okunur.
        path: cashLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          cashRepository == null || posRepository == null
              ? const Scaffold(
                  body: AppErrorView(message: 'Kasa servisi yapılandırılmadı.'),
                )
              : _CashPageHost(
                  cashRepository: cashRepository,
                  posRepository: posRepository,
                  dayCloseRepository: dayCloseRepository,
                  changes: financialDataChanges,
                  scopeController: scopeController,
                  // `İşlem ekle > Gün sonu` ve `POS tahsilatı` doğrudan kendi
                  // paneline açılıyor; menüden gelen kullanıcıyı Kasa'ya
                  // bırakıp düğmeyi elle buldurmak yolun yarısında bırakmaktı.
                  initialTab: switch (state.uri.queryParameters['tab']) {
                    'pos' => 1,
                    'day-close' => 2,
                    'transit' => 3,
                    _ => 0,
                  },
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/goals',
        pageBuilder: (context, state) => _sessionPage(
          state,
          goalRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Tasarruf hedefi servisi yapılandırılmadı.',
                  ),
                )
              : GoalsPage(repository: goalRepository),
          authController,
        ),
      ),
      GoRoute(
        path: taxesLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          taxRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Vergi servisi yapılandırılmadı.',
                  ),
                )
              : _TaxesHost(
                  repository: taxRepository,
                  changes: financialDataChanges,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/planning',
        pageBuilder: (context, state) => _sessionPage(
          state,
          planningRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Planlama ve rapor servisi yapılandırılmadı.',
                  ),
                )
              : PlanningPage(
                  repository: planningRepository,
                  financialDataChanges: financialDataChanges,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: accountLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          userAccountRepository == null || authRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Hesap servisi yapılandırılmadı.',
                  ),
                )
              : AccountPage(
                  repository: userAccountRepository,
                  authRepository: authRepository,
                ),
          authController,
        ),
      ),
      GoRoute(
        path: remindersLocation,
        pageBuilder: (context, state) => _sessionPage(
          state,
          reminderController == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Hatırlatma servisi yapılandırılmadı.',
                  ),
                )
              : ReminderSettingsPage(controller: reminderController),
          authController,
        ),
      ),
      GoRoute(
        path: '/more/data-tools',
        pageBuilder: (context, state) => _sessionPage(
          state,
          dataToolsRepository == null
              ? const Scaffold(
                  body: AppErrorView(
                    message: 'Veri araçları servisi yapılandırılmadı.',
                  ),
                )
              : DataToolsPage(
                  repository: dataToolsRepository,
                  changes: financialDataChanges,
                ),
          authController,
        ),
      ),
    ],
  );
}

/// Üçüncü ana hedefin ön ayarı profil cevabından gelir; özellik kapatılmaz.
/// Yerinden inen ekran `Diğer` altında kendi doğrudan rotasını korur.
class _AdaptiveThirdDestination extends StatelessWidget {
  const _AdaptiveThirdDestination({
    required this.scopeController,
    required this.budgetRepository,
    required this.cashRepository,
    required this.posRepository,
    required this.changes,
    this.dayCloseRepository,
  });

  final ScopeController? scopeController;
  final BudgetRepositoryContract? budgetRepository;
  final CashRepositoryContract? cashRepository;
  final PosRepositoryContract? posRepository;
  final DayCloseRepositoryContract? dayCloseRepository;
  final FinancialDataChanges? changes;

  @override
  Widget build(BuildContext context) {
    final controller = scopeController;
    if (controller == null) return _budgets();
    return AnimatedBuilder(
      animation: controller,
      builder: (context, _) => controller.isVisible ? _cash() : _budgets(),
    );
  }

  Widget _budgets() => BudgetsPage(
    key: const ValueKey('primary-budgets'),
    repository: budgetRepository,
    changes: changes,
    scopeController: scopeController,
  );

  Widget _cash() {
    final cashRepository = this.cashRepository;
    final posRepository = this.posRepository;
    if (cashRepository == null || posRepository == null) {
      return const Scaffold(
        key: ValueKey('primary-cash-unavailable'),
        body: AppErrorView(message: 'Kasa servisi yapılandırılmadı.'),
      );
    }
    return _CashPageHost(
      key: const ValueKey('primary-cash'),
      cashRepository: cashRepository,
      posRepository: posRepository,
      dayCloseRepository: dayCloseRepository,
      changes: changes,
      scopeController: scopeController,
    );
  }
}

/// Controller'ları sayfanın yeniden çizimlerinden daha uzun yaşatır.
class _CashPageHost extends StatefulWidget {
  const _CashPageHost({
    required this.cashRepository,
    required this.posRepository,
    required this.changes,
    required this.scopeController,
    super.key,
    this.initialTab = 0,
    this.dayCloseRepository,
  });

  final CashRepositoryContract cashRepository;
  final PosRepositoryContract posRepository;
  final DayCloseRepositoryContract? dayCloseRepository;
  final FinancialDataChanges? changes;
  final ScopeController? scopeController;
  final int initialTab;

  @override
  State<_CashPageHost> createState() => _CashPageHostState();
}

class _CashPageHostState extends State<_CashPageHost> {
  late final CashCountController _cashController;
  late final PosController _posController;
  DayCloseController? _dayCloseController;

  @override
  void initState() {
    super.initState();
    final dayCloseRepository = widget.dayCloseRepository;
    if (dayCloseRepository != null) {
      _dayCloseController = DayCloseController(
        dayCloseRepository,
        changes: widget.changes,
      );
    }
    _cashController = CashCountController(
      widget.cashRepository,
      changes: widget.changes,
      // Kapsam boyutunu gören (işletmesi olan) kullanıcıda Kasa işletmenin
      // kasasıdır; şahsi cüzdan Hesaplar'da durur.
      hidesPersonalAccounts: () => widget.scopeController?.isVisible ?? false,
    );
    _posController = PosController(
      widget.posRepository,
      changes: widget.changes,
    );
  }

  @override
  void dispose() {
    _cashController.dispose();
    _posController.dispose();
    _dayCloseController?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => CashPage(
    cashController: _cashController,
    posController: _posController,
    dayCloseController: _dayCloseController,
    scopeController: widget.scopeController,
    ownsControllers: false,
    initialTab: widget.initialTab,
    // Şahsi hesabı olmayan kullanıcı onu Hesaplar'da açar.
    onOpenPersonalAccount: () => context.push(accountsLocation),
  );
}

/// Vergi takibi ekranının controller'ını **bir kez** kuran kabuk.
///
/// Rota kurucusu her yeniden çizimde çalışır; controller orada kurulsaydı üste
/// itilen bir sayfadan geri dönüldüğünde ekran yüklenmemiş yeni bir
/// controller'a bağlanır ve boş görünürdü. Kabuk aynı deseni `Kasa`
/// ekranındakiyle paylaşıyor.
class _TaxesHost extends StatefulWidget {
  const _TaxesHost({required this.repository, this.changes});

  final TaxRepositoryContract repository;
  final FinancialDataChanges? changes;

  @override
  State<_TaxesHost> createState() => _TaxesHostState();
}

class _TaxesHostState extends State<_TaxesHost> {
  late final TaxController _controller = TaxController(
    widget.repository,
    changes: widget.changes,
  );

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) =>
      TaxTrackingPage(controller: _controller, ownsController: false);
}

String? _authRedirect(AuthController controller, GoRouterState state) {
  final location = state.uri.path;
  // Parola sıfırlama da oturumsuz bir kimlik yoludur: giriş yapamayan
  // kullanıcı buraya gelir ve girişe geri yönlendirilmemelidir.
  final isAuthRoute =
      location == '/login' ||
      location == '/register' ||
      location == '/password-reset';
  final isRestoringRoute = location == '/restoring';

  switch (controller.status) {
    case AuthStatus.restoring:
      return isRestoringRoute ? null : '/restoring';
    case AuthStatus.unauthenticated:
      if (isAuthRoute) {
        return null;
      }
      final from = state.uri.toString();
      return Uri(path: '/login', queryParameters: {'from': from}).toString();
    case AuthStatus.authenticated:
      if (location == '/login') {
        return _safeReturnLocation(state.uri.queryParameters['from']) ??
            '/dashboard';
      }
      return isAuthRoute || isRestoringRoute ? '/dashboard' : null;
  }
}

String? _safeReturnLocation(String? value) {
  if (value == null ||
      !value.startsWith('/') ||
      value.startsWith('//') ||
      value.startsWith('/login') ||
      value.startsWith('/register') ||
      value.startsWith('/restoring')) {
    return null;
  }
  return value;
}

Page<dynamic> _authPage(
  GoRouterState state,
  AuthView view,
  AuthController? authController,
  AuthSessionRepository? authRepository,
) {
  return NoTransitionPage<void>(
    key: const ValueKey('auth-flow'),
    child: AuthFlow(
      initialView: view,
      initialEmail: state.uri.queryParameters['email'],
      onLogin: (email, password, {onVerified}) async =>
          authController?.login(email, password, onVerified: onVerified),
      // Kayıt bitince oturum hemen açılır; kişi e-postasını ve parolasını
      // ikinci kez yazmaz.
      onRegister:
          (
            email,
            password, {
            required hasBusiness,
            username,
            onVerified,
          }) async {
            if (authController == null) return;
            await authController.register(
              email,
              password,
              hasBusiness: hasBusiness,
              username: username,
            );
            await authController.login(email, password, onVerified: onVerified);
          },
      onCheckUsername: authRepository?.isUsernameAvailable,
      onRequestPasswordReset: authRepository == null
          ? (_) async {}
          : authRepository.requestPasswordReset,
      onResetPassword: authRepository == null
          ? ({required email, required code, required newPassword}) async {}
          : authRepository.resetPassword,
    ),
  );
}

Page<dynamic> Function(BuildContext, GoRouterState) _pageBuilder(Widget child) {
  return (context, state) =>
      NoTransitionPage<void>(key: state.pageKey, child: child);
}

/// Fiş tarama adımı. Okuma bitince aynı gider rotasına, önerileri `extra` ile
/// taşıyarak geçer; ayrı bir onay formu yoktur. `pushReplacement` kullanılıyor
/// çünkü geri tuşu kullanıcıyı okunmuş bir fişin tarama ekranına döndürmemeli.
Page<dynamic> Function(BuildContext, GoRouterState) _receiptScanPageBuilder({
  required ReceiptRepositoryContract? receiptRepository,
  required ReceiptImageSourceContract? receiptImageSource,
  required ReceiptImageNormalizerContract? receiptImageNormalizer,
  required ReceiptPreferencesContract? receiptPreferences,
  required AuthController? authController,
  ReceiptScanVariant variant = ReceiptScanVariant.receipt,
}) {
  return (context, state) => _sessionPage(
    state,
    receiptRepository == null ||
            receiptImageSource == null ||
            receiptImageNormalizer == null ||
            receiptPreferences == null
        ? const SizedBox.shrink()
        : ReceiptScanPage(
            variant: variant,
            controller: ReceiptScanController(
              receiptImageSource,
              receiptRepository,
              receiptImageNormalizer,
              receiptPreferences,
              intent: variant.initialIntent,
            ),
            // Yön kullanıcının bildirdiği şeydir; taslak ona göre gider veya
            // gelir formuna gider. Formun hangi yönde olduğunu rota belirler,
            // taslak değil.
            // Dekont doğrudan bir forma gitmiyor: ana tutarın ne olduğu
            // belgeden okunamaz, önce sorulur.
            onBankDocumentReady: (draft) => context.pushReplacement(
              bankDocumentDecisionLocation,
              extra: draft,
            ),
            // İade yeni kayıt üretmez; geri verdiği harcamayı iptal eder.
            onRefundReady: (draft) =>
                context.pushReplacement(refundDecisionLocation, extra: draft),
            // Son ödeme tarihi taşıyan belge ödendiğini söylemez; sorulur.
            onInvoiceReady: (draft) =>
                context.pushReplacement(invoiceDecisionLocation, extra: draft),
            // Taksitli satış tek seferlik tam tutar gideri değildir. Kartı fiş
            // söylemez; kullanıcı listeden seçer.
            onInstallmentReady: (draft) => context.pushReplacement(
              '/more/cards',
              extra: receiptInstallmentPrefillFrom(draft),
            ),
            onDraftReady: (prefill, intent) => context.pushReplacement(
              intent == ReceiptCaptureIntent.income
                  ? '/transactions/new/income'
                  : '/transactions/new/expense',
              extra: prefill,
            ),
          ),
    authController,
  );
}

/// Builds the quick-add form with a controller scoped to that page, so leaving
/// the form drops its state instead of carrying a half-filled entry forward.
Page<dynamic> Function(BuildContext, GoRouterState) _quickAddPageBuilder({
  required bool isExpense,
  required TransactionRepositoryContract? transactionRepository,
  required FinanceRepositoryContract? financeRepository,
  required FinancialDataChanges? financialDataChanges,
  required DataToolsRepositoryContract? dataToolsRepository,
  required AuthController? authController,
  ScopeController? scopeController,
}) {
  return (context, state) => _sessionPage(
    state,
    transactionRepository == null || financeRepository == null
        ? const SizedBox.shrink()
        : QuickAddFormPage(
            isExpense: isExpense,
            scopeController: scopeController,
            prefill: state.extra is QuickAddPrefill
                ? state.extra! as QuickAddPrefill
                : null,
            controller: QuickAddController(
              transactionRepository,
              financeRepository,
              financialDataChanges: financialDataChanges,
              // Belge uç noktası zaten veri araçları deposunda; ikinci bir
              // istemci yazmak aynı sözleşmeyi iki yerde tutmak olurdu.
              uploadAttachment: dataToolsRepository == null
                  ? null
                  : (transactionId, attachment) =>
                        dataToolsRepository.uploadAttachment(
                          transactionId,
                          ApiUpload(
                            bytes: attachment.bytes,
                            fileName: attachment.fileName,
                            mediaType: attachment.mediaType,
                          ),
                        ),
            ),
          ),
    authController,
  );
}

/// Yükümlülük formu iki rotadan açılır: fiş ekranının kardeşi olarak ve
/// `Yükümlülükler` listesinin altında.
///
/// Öneri **zorunlu değil**: form fişten de, elle de açılır. Boş `extra`,
/// okunmuş belgesi olmayan kullanıcının yolu; eksik olan bir öneri, eksik olan
/// bir ekran değildir.
Page<dynamic> Function(BuildContext, GoRouterState) _obligationFormPageBuilder({
  required ObligationRepositoryContract? obligationRepository,
  required FinancialDataChanges? financialDataChanges,
  required ScopeController? scopeController,
  required AuthController? authController,
}) {
  return (context, state) => _sessionPage(
    state,
    obligationRepository == null
        ? const Scaffold(
            body: AppErrorView(message: 'Yükümlülük formu açılamadı.'),
          )
        : ObligationFormPage(
            controller: ObligationController(
              obligationRepository,
              changes: financialDataChanges,
            ),
            prefill: state.extra is ObligationPrefill
                ? state.extra! as ObligationPrefill
                : const ObligationPrefill(),
            scopeController: scopeController,
          ),
    authController,
  );
}

Page<dynamic> Function(BuildContext, GoRouterState) _sessionPageBuilder(
  Widget child,
  AuthController? authController,
) {
  return (context, state) => _sessionPage(state, child, authController);
}

/// Alt sekmelerin kökleri. Sekmeler arasında geçiş kayarak olmaz: alt çubuk
/// yerinde durur, yalnız içerik değişir.
const _tabRoots = {'/dashboard', '/transactions', '/budgets', '/more'};

/// Oturum açıkken görünen sayfa.
///
/// Sekme kökleri geçişsizdir. Bir sayfanın üstüne açılan her şey (Hesaplar,
/// Kategori özeti, Vergi, formlar…) platformun kendi geçişiyle açılır:
/// iPhone'da sağdan kayarak gelir ve **soldan kenarından çekince kapanır**,
/// Android'de kendi geçişiyle. Hepsi geçişsizken iPhone'da kenardan çekerek
/// geri dönülemiyordu; bu hareket yalnız kayarak açılan sayfada vardır
/// (kullanıcının iPhone bulgusu, 7 Kasım 2026).
Page<dynamic> _sessionPage(
  GoRouterState state,
  Widget child,
  AuthController? authController,
) {
  final key = ValueKey((state.pageKey, authController?.sessionGeneration ?? 0));
  if (_tabRoots.contains(state.matchedLocation)) {
    return NoTransitionPage<void>(key: key, child: child);
  }
  return MaterialPage<void>(key: key, child: child);
}

/// Faturanın ödendi/ödenmedi cevabını yazma yollarına bağlar.
///
/// Ödendiyse sıradan bir gider: para çıktı, aylık gidere girer. Ödenmediyse
/// belge tarihinde gider tanıyan, fakat hesap bakiyesini değiştirmeyen tek
/// seferlik bir borç doğar. Sonraki ödeme bu borcu kapatacak ayrı nakit olayıdır.
///
/// Form sorunun **üstüne** açılır, yerine değil: formdan geri dönen kullanıcı
/// okunmuş faturasıyla "ödedim mi?" sorusuna döner ve öbür cevabı seçebilir.
/// Yerine açıldığında geri tuşu onu okutmaya başladığı ekrana atıyor, fatura
/// kayboluyordu. Kayıt yazılınca soru sayfası da kapanır.
Future<void> _recordInvoice(
  BuildContext context,
  ReceiptDraft draft,
  bool isPaid,
) async {
  final saved = isPaid
      // Ödeme tarihi faturanın vadesi değil: para bugün çıktı. Taslağın kendi
      // tarihi taşınıyor, `dueDate` taşınmıyor.
      ? await context.push<bool>(
          '/transactions/new/expense',
          extra: receiptPrefillFrom(draft),
        )
      : await context.push<bool>(
          obligationCreateLocation,
          extra: receiptObligationPrefillFrom(draft),
        );
  if ((saved ?? false) && context.mounted) context.pop();
}

/// İadeyi yazar: eski harcamayı **iptal eder**, silmez.
///
/// Kısmi iadede iptal tek başına yetmez — harcamanın bir kısmı gerçekten
/// yapıldı — ve kalanı için gider formu açılır. Kalan tutar **sunucudan**
/// geliyor; istemci finansal toplamı ikinci kez hesaplamaz.
Future<void> _recordRefund(
  BuildContext context,
  ReceiptRefundMatch match,
  TransactionRepositoryContract? transactions,
  FinancialDataChanges? changes,
) async {
  if (transactions == null) return;
  final messenger = ScaffoldMessenger.of(context);
  final remaining = match.remainingAmount;

  // Kısmi iade: düzeltme = eski hareketi iptal et + yeni doğru hareket yaz.
  // İkisi **formun kaydında** birlikte yapılır; burada yalnız form açılır.
  // Önce burada iptal ediliyordu: form bırakılınca harcama tümüyle
  // kayboluyor, kalan hiç yazılmıyordu (backlog 16a). Kalan tutar öneri
  // olarak geliyor; kaynağı ve kategoriyi kullanıcı seçiyor.
  if (remaining != null) {
    context.pushReplacement(
      '/transactions/new/expense',
      extra: QuickAddPrefill(
        replaces: QuickAddReplacement(
          transactionId: match.transactionId,
          amount: match.amount,
          date: match.transactionDate,
        ),
        amount: QuickAddSuggestion(remaining, QuickAddSuggestionState.read),
        date: QuickAddSuggestion(
          match.transactionDate,
          QuickAddSuggestionState.read,
        ),
        description: match.description == null
            ? null
            : QuickAddSuggestion(
                match.description!,
                QuickAddSuggestionState.read,
              ),
      ),
    );
    return;
  }

  // Tam iade tek adımdır: yarım kalacak ikinci bir kayıt yok.
  try {
    // İptal sunucuda idempotent: ikinci kez onaylamak ikinci bir iptal
    // üretmez, aynı UTC damgalı kaydı geri döner.
    await transactions.cancel(match.transactionId);
    changes?.transactionsChanged();
  } on ApiException catch (error) {
    messenger.showSnackBar(
      SnackBar(content: Text('Harcama iptal edilemedi: ${error.message}')),
    );
    return;
  }
  if (!context.mounted) return;
  messenger.showSnackBar(
    const SnackBar(content: Text('Harcama iptal edildi.')),
  );
  context.pop();
}

/// Ücret yazıcısını depolardan kurar.
///
/// Ekranlar depoları tanımıyor; buradan dar bir imza olarak iniyor. Depo yoksa
/// `null` döner ve ücret adımı hiç çalışmaz — ana kayıt bundan etkilenmez.
ReceiptFeeRecorder? _feeRecorder(
  TransactionRepositoryContract? transactions,
  FinanceRepositoryContract? finance,
  FinancialDataChanges? changes,
) {
  if (transactions == null || finance == null) return null;
  return ({
    required String sourceId,
    required String amount,
    required String date,
    required String description,
  }) => recordReceiptFee(
    transactions: transactions,
    finance: finance,
    sourceId: sourceId,
    // Bu iki yolda kaynak daima bir hesaptır: transferin çıkış ucu ve kart
    // ödemesinin ödeme hesabı. Ücreti karta yazmak, bankanın aldığı parayı
    // kart borcu göstermek olurdu.
    sourceIsCard: false,
    amount: amount,
    date: date,
    description: description,
    changes: changes,
  );
}

/// Karar sayfasının seçimini yazma yollarına bağlar.
///
/// Yönlendirme burada, sayfada değil: karar sayfası hangi rotanın neyi yazdığını
/// bilmek zorunda değil. Ücret ana kayıttan **sonra** öneriliyor — ana kayıt
/// yazılmadıysa ortada harcanmış bir ücret de yoktur — ve yalnız kullanıcı
/// sayfada onayladıysa.
Future<void> _recordBankDocument(
  BuildContext context,
  ReceiptDraft draft,
  BankDocumentDecision decision,
  bool recordFee,
) async {
  switch (decision) {
    case BankDocumentDecision.expense:
      // Ücret formla birlikte gidiyor ve ana kayıt yazıldıktan sonra aynı
      // kaynağa yazılıyor; ikinci bir form açılmıyor.
      context.push(
        '/transactions/new/expense',
        extra: receiptPrefillFrom(
          draft,
          autoFee: recordFee ? receiptAutoFeeFrom(draft) : null,
        ),
      );

    case BankDocumentDecision.ownTransfer:
      // Transfer ekranı bir form değil bir sayfa; ücret taslakla birlikte
      // taşınıyor ve transfer yazıldıktan sonra transferin **kaynak
      // hesabından** yazılıyor. Kullanıcı karar sayfasında anahtarı kapattıysa
      // hiç taşınmıyor.
      context.push(
        transferLocation,
        extra: receiptTransferPrefillFrom(draft, withFee: recordFee),
      );

    case BankDocumentDecision.cardPayment:
      // Kartı dekont söylemez; kullanıcı kart listesinden seçer, ödeme formu
      // o kartın sayfasında önerilerle açılır. Kart ödemesi gider üretmez.
      // Ücret, ödemenin çıktığı **hesaptan** yazılıyor: kart borcuna eklemek
      // bankanın aldığı parayı borç gibi gösterirdi.
      context.push(
        '/more/cards',
        extra: receiptCardPaymentPrefillFrom(draft, withFee: recordFee),
      );

    // Kişiye havale bir harcama olmayabilir: para geri beklenen bir alacaktır.
    // Gider yazılsaydı hem gider raporu şişer hem alacak hiç kaydedilmezdi.
    // Hesabı dekont söylemez; kullanıcı seçer.
    case BankDocumentDecision.lending:
      context.push(
        '/more/debts',
        extra: receiptLendingPrefillFrom(draft, withFee: recordFee),
      );
  }
}

DateTime? _pageMonth(GoRouterState state) {
  final year = int.tryParse(state.uri.queryParameters['year'] ?? '');
  final month = int.tryParse(state.uri.queryParameters['month'] ?? '');
  return year == null || month == null || month < 1 || month > 12
      ? null
      : DateTime(year, month);
}

Future<void> _openPageAccountForm(
  BuildContext context,
  FinancePageRepositoryContract pages,
  AccountRepository repository,
  FinancialDataChanges? changes, {
  String? id,
}) async {
  final vm = AccountsViewModel(repository, financialDataChanges: changes);
  try {
    if (!context.mounted) return;
    await Navigator.push<bool>(
      context,
      MaterialPageRoute(
        builder: (_) => id == null
            ? AccountFormPage(onSave: vm.save, readError: () => vm.message)
            : AccountSettingsPage(
                load: () => pages.accountSettings(id),
                onSave: vm.save,
                onDelete: vm.delete,
                readError: () => vm.message,
              ),
      ),
    );
  } on ApiException catch (error) {
    if (context.mounted) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(error.message)));
    }
  } on FormatException {
    if (context.mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Hesap ayarları alınamadı.')),
      );
    }
  } finally {
    vm.dispose();
  }
}
