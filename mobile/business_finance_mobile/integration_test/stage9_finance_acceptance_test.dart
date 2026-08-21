import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:integration_test/integration_test.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/storage/secure_session_store.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_models.dart';
import 'package:business_finance_mobile/features/auth/data/auth_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_service.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_models.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_repository.dart';
import 'package:business_finance_mobile/features/categories/data/category_repository.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets(
    'validates finance math, filters, cancellation and isolation with real SQL',
    (tester) async {
      final suffix = DateTime.now().microsecondsSinceEpoch;
      final firstEmail = 'stage9.finance.a.$suffix@example.com';
      final secondEmail = 'stage9.finance.b.$suffix@example.com';
      final accountName = 'Stage 9 finance account $suffix';
      final incomeDescription = 'Stage 9 salary $suffix';
      final expenseDescription = 'Stage 9 market $suffix';
      const password = 'Stage9-Finance-2026';
      final today = DateTime.now();
      final transactionDate = _dateText(today);
      final sessionStore = _MemorySessionStore();
      final httpClient = http.Client();
      final publicApiClient = ApiClient(
        config: ApiConfig.fromEnvironment(),
        httpClient: httpClient,
      );
      final authRepository = AuthRepository(
        remoteService: ApiAuthService(publicApiClient),
        sessionStore: sessionStore,
      );

      addTearDown(() {
        httpClient.close();
      });

      await authRepository.register(firstEmail, password);
      await authRepository.login(firstEmail, password);
      final firstUser = _FinanceRepositories(authRepository, httpClient);
      final account = await firstUser.accounts.create(
        name: accountName,
        type: 'cash',
        openingBalance: '1000',
      );
      final incomeCategory = (await firstUser.categories.list(
        type: 'income',
        isActive: true,
      )).first;
      final expenseCategory = (await firstUser.categories.list(
        type: 'expense',
        isActive: true,
      )).first;

      await firstUser.budgets.create(
        CreateBudgetInput(
          categoryId: expenseCategory.id,
          limit: '100',
          year: today.year,
          month: today.month,
        ),
      );
      await firstUser.transactions.create(
        CreateTransactionInput(
          accountId: account.id,
          categoryId: incomeCategory.id,
          amount: '250',
          kind: TransactionKind.income,
          transactionDate: transactionDate,
          description: incomeDescription,
        ),
      );
      final expense = await firstUser.transactions.create(
        CreateTransactionInput(
          accountId: account.id,
          categoryId: expenseCategory.id,
          amount: '125.50',
          kind: TransactionKind.expense,
          transactionDate: transactionDate,
          description: expenseDescription,
        ),
      );

      final expenseFilter = await firstUser.transactions.list(
        filter: TransactionFilter(
          dateFrom: transactionDate,
          dateTo: transactionDate,
          accountId: account.id,
          categoryId: expenseCategory.id,
          kind: TransactionKind.expense,
        ),
      );
      final incomeFilter = await firstUser.transactions.list(
        filter: TransactionFilter(
          dateFrom: transactionDate,
          dateTo: transactionDate,
          accountId: account.id,
          categoryId: incomeCategory.id,
          kind: TransactionKind.income,
        ),
      );

      expect(expenseFilter.items.map((item) => item.description), [
        expenseDescription,
      ]);
      expect(incomeFilter.items.map((item) => item.description), [
        incomeDescription,
      ]);

      final accountAfterTransactions = (await firstUser.accounts.list()).items
          .singleWhere((item) => item.id == account.id);
      final budgetAfterExpense = (await firstUser.budgets.list(
        today.year,
        today.month,
      )).single;
      final dashboardAfterTransactions = await firstUser.dashboard.getMonthly(
        today.year,
        today.month,
      );

      expect(accountAfterTransactions.balance, '1124.5000');
      expect(budgetAfterExpense.spent, '125.5000');
      expect(budgetAfterExpense.remaining, '0.0000');
      expect(budgetAfterExpense.exceeded, '25.5000');
      expect(dashboardAfterTransactions.totalIncome, '250.0000');
      expect(dashboardAfterTransactions.totalExpense, '125.5000');
      expect(dashboardAfterTransactions.net, '124.5000');
      expect(
        dashboardAfterTransactions.accountBalances
            .singleWhere((item) => item.accountId == account.id)
            .balance,
        '1124.5000',
      );

      await firstUser.transactions.cancel(expense.id);

      final accountAfterCancellation = (await firstUser.accounts.list()).items
          .singleWhere((item) => item.id == account.id);
      final budgetAfterCancellation = (await firstUser.budgets.list(
        today.year,
        today.month,
      )).single;

      expect(accountAfterCancellation.balance, '1250.0000');
      expect(budgetAfterCancellation.spent, '0.0000');
      expect(budgetAfterCancellation.remaining, '100.0000');
      expect(budgetAfterCancellation.exceeded, '0.0000');

      await firstUser.accounts.update(
        id: account.id,
        name: accountName,
        isActive: false,
      );
      expect(
        (await firstUser.transactions.listAccounts()).map((item) => item.id),
        isNot(contains(account.id)),
      );
      await expectLater(
        firstUser.transactions.create(
          CreateTransactionInput(
            accountId: account.id,
            categoryId: expenseCategory.id,
            amount: '1',
            kind: TransactionKind.expense,
            transactionDate: transactionDate,
          ),
        ),
        throwsA(
          isA<ApiException>().having(
            (error) => error.statusCode,
            'statusCode',
            400,
          ),
        ),
      );

      await authRepository.logout();
      await authRepository.register(secondEmail, password);
      await authRepository.login(secondEmail, password);
      final secondUser = _FinanceRepositories(authRepository, httpClient);

      expect((await secondUser.accounts.list()).items, isEmpty);
      expect((await secondUser.transactions.list()).items, isEmpty);
      expect(await secondUser.budgets.list(today.year, today.month), isEmpty);
      final secondDashboard = await secondUser.dashboard.getMonthly(
        today.year,
        today.month,
      );
      expect(secondDashboard.totalIncome, '0.0000');
      expect(secondDashboard.totalExpense, '0.0000');
      expect(secondDashboard.accountBalances, isEmpty);

      await authRepository.logout();
      expect(await sessionStore.read(), isNull);
    },
  );
}

class _FinanceRepositories {
  _FinanceRepositories(
    AuthSessionRepository authRepository,
    http.Client httpClient,
  ) {
    final client = ApiClient(
      config: ApiConfig.fromEnvironment(),
      httpClient: httpClient,
      accessTokenProvider: authRepository.getValidAccessToken,
      onUnauthorized: () async =>
          (await authRepository.refreshSession())?.accessToken,
    );
    accounts = ApiAccountRepository(client);
    budgets = BudgetRepository(client);
    categories = ApiCategoryRepository(client);
    dashboard = DashboardRepository(client);
    transactions = TransactionRepository(client);
  }

  late final AccountRepository accounts;
  late final BudgetRepositoryContract budgets;
  late final CategoryRepository categories;
  late final DashboardDataSource dashboard;
  late final TransactionRepositoryContract transactions;
}

class _MemorySessionStore implements SessionStore {
  AuthSession? _session;

  @override
  Future<void> clear() async => _session = null;

  @override
  Future<AuthSession?> read() async => _session;

  @override
  Future<void> write(AuthSession session) async => _session = session;
}

String _dateText(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';
