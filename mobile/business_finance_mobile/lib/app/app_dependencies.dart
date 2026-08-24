import 'package:http/http.dart' as http;

import '../core/config/api_config.dart';
import '../core/network/api_client.dart';
import '../core/presentation/financial_data_changes.dart';
import '../core/presentation/scope_controller.dart';
import '../core/storage/secure_session_store.dart';
import '../features/auth/data/auth_repository.dart';
import '../features/auth/data/auth_service.dart';
import '../features/auth/presentation/auth_controller.dart';
import '../features/accounts/data/account_repository.dart';
import '../features/budgets/data/budget_repository.dart';
import '../features/categories/data/category_repository.dart';
import '../features/dashboard/data/dashboard_repository.dart';
import '../features/activities/data/activity_repository.dart';
import '../features/transactions/data/transaction_repository.dart';
import '../features/cards/data/finance_repository.dart';
import '../features/planning/data/planning_repository.dart';
import '../features/pos/data/pos_repository.dart';
import '../features/profile/data/profile_repository.dart';
import '../features/profile/data/scope_preferences.dart';
import '../features/data_tools/data/data_tools_repository.dart';
import '../features/cash/data/cash_repository.dart';
import '../features/counterparties/data/counterparty_repository.dart';
import '../features/debts/data/debt_repository.dart';
import '../features/goals/data/goal_repository.dart';
import '../features/obligations/data/obligation_repository.dart';
import '../features/receipts/data/receipt_image_source.dart';
import '../features/receipts/data/receipt_photo.dart';
import '../features/receipts/data/receipt_preferences.dart';
import '../features/receipts/data/receipt_repository.dart';

class AppDependencies {
  AppDependencies._(
    this.authController,
    this.authRepository,
    this.apiClient,
    this.financialDataChanges,
    this.scopeController,
    this.profileRepository,
    this.accountRepository,
    this.budgetRepository,
    this.categoryRepository,
    this.dashboardRepository,
    this.transactionRepository,
    this.activityRepository,
    this.financeRepository,
    this.planningRepository,
    this.dataToolsRepository,
    this.debtRepository,
    this.counterpartyRepository,
    this.goalRepository,
    this.obligationRepository,
    this.cashRepository,
    this.posRepository,
    this.receiptRepository,
    this.receiptImageSource,
    this.receiptImageNormalizer,
    this.receiptPreferences,
    this._httpClient,
  );

  factory AppDependencies.create() {
    final httpClient = http.Client();
    final config = ApiConfig.fromEnvironment();
    final publicApiClient = ApiClient(config: config, httpClient: httpClient);
    final financialDataChanges = FinancialDataChanges();
    final authRepository = AuthRepository(
      remoteService: ApiAuthService(publicApiClient),
      sessionStore: SecureSessionStore(),
    );
    final authController = AuthController(authRepository);
    final apiClient = ApiClient(
      config: config,
      httpClient: httpClient,
      accessTokenProvider: authRepository.getValidAccessToken,
      onUnauthorized: authController.refreshAfterUnauthorized,
    );
    final accountRepository = ApiAccountRepository(apiClient);
    final budgetRepository = BudgetRepository(apiClient);
    final categoryRepository = ApiCategoryRepository(apiClient);
    final dashboardRepository = DashboardRepository(apiClient);
    final transactionRepository = TransactionRepository(apiClient);
    final activityRepository = ActivityRepository(apiClient);
    final financeRepository = FinanceRepository(apiClient);
    final planningRepository = PlanningRepository(apiClient);
    final dataToolsRepository = DataToolsRepository(apiClient);
    final debtRepository = DebtRepository(apiClient);
    final counterpartyRepository = CounterpartyRepository(apiClient);
    final goalRepository = GoalRepository(apiClient);
    final obligationRepository = ObligationRepository(apiClient);
    final cashRepository = CashRepository(apiClient);
    final posRepository = PosRepository(apiClient);
    final receiptRepository = ReceiptRepository(apiClient);
    final profileRepository = ProfileRepository(apiClient);
    // Kapsam anahtarı cihazdan, onboarding cevabı sunucudan okunur; ikisi de
    // oturum açıldığında yüklenir (`BusinessFinanceApp`).
    final scopeController = ScopeController(
      store: ScopePreferences(),
      readHasBusiness: () async => (await profileRepository.read()).hasBusiness,
    );

    return AppDependencies._(
      authController,
      authRepository,
      apiClient,
      financialDataChanges,
      scopeController,
      profileRepository,
      accountRepository,
      budgetRepository,
      categoryRepository,
      dashboardRepository,
      transactionRepository,
      activityRepository,
      financeRepository,
      planningRepository,
      dataToolsRepository,
      debtRepository,
      counterpartyRepository,
      goalRepository,
      obligationRepository,
      cashRepository,
      posRepository,
      receiptRepository,
      ImagePickerReceiptImageSource(),
      const ReceiptImageNormalizer(),
      ReceiptPreferences(),
      httpClient,
    );
  }

  final AuthController authController;
  final AuthSessionRepository authRepository;
  final ApiClient apiClient;
  final FinancialDataChanges financialDataChanges;

  /// Uygulama genelindeki tek kapsam anahtarı (ADR 0013: sekme başına ayrı
  /// filtre yoktur).
  final ScopeController scopeController;
  final ProfileRepositoryContract profileRepository;
  final AccountRepository accountRepository;
  final BudgetRepositoryContract budgetRepository;
  final CategoryRepository categoryRepository;
  final DashboardRepository dashboardRepository;
  final TransactionRepositoryContract transactionRepository;
  final ActivityRepositoryContract activityRepository;
  final FinanceRepositoryContract financeRepository;
  final PlanningRepositoryContract planningRepository;
  final DataToolsRepositoryContract dataToolsRepository;
  final DebtRepositoryContract debtRepository;
  final CounterpartyRepositoryContract counterpartyRepository;
  final GoalRepositoryContract goalRepository;
  final ObligationRepositoryContract obligationRepository;
  final CashRepositoryContract cashRepository;
  final PosRepositoryContract posRepository;
  final ReceiptRepositoryContract receiptRepository;

  /// Fiş çekme akışının cihaz tarafı. Controller kısa ömürlüdür ve akış
  /// başlarken kurulur; burada yalnız onun bağımlılıkları duruyor.
  final ReceiptImageSourceContract receiptImageSource;
  final ReceiptImageNormalizerContract receiptImageNormalizer;
  final ReceiptPreferencesContract receiptPreferences;
  final http.Client _httpClient;

  void dispose() {
    authController.dispose();
    financialDataChanges.dispose();
    scopeController.dispose();
    _httpClient.close();
  }
}
