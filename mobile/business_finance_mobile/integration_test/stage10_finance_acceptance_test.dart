import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:integration_test/integration_test.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/storage/secure_session_store.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_models.dart';
import 'package:business_finance_mobile/features/auth/data/auth_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_service.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/categories/data/category_repository.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets(
    'validates transfer card statement and installment flow with real SQL',
    (tester) async {
      final suffix = DateTime.now().microsecondsSinceEpoch;
      final email = 'stage10.finance.$suffix@example.com';
      const password = 'Stage10-Finance-2026';
      final now = DateTime.now();
      final date = _dateText(now);
      final requestId = _requestId(suffix);
      final httpClient = http.Client();
      final sessionStore = _MemorySessionStore();
      final publicClient = ApiClient(
        config: ApiConfig.fromEnvironment(),
        httpClient: httpClient,
      );
      final auth = AuthRepository(
        remoteService: ApiAuthService(publicClient),
        sessionStore: sessionStore,
      );
      addTearDown(httpClient.close);

      await auth.register(email, password);
      await auth.login(email, password);
      final client = ApiClient(
        config: ApiConfig.fromEnvironment(),
        httpClient: httpClient,
        accessTokenProvider: auth.getValidAccessToken,
        onUnauthorized: () async => (await auth.refreshSession())?.accessToken,
      );
      final accounts = ApiAccountRepository(client);
      final categories = ApiCategoryRepository(client);
      final finance = FinanceRepository(client);
      final dashboard = DashboardRepository(client);
      final source = await accounts.create(
        name: 'Stage 10 source $suffix',
        type: 'bank',
        openingBalance: '1000',
      );
      final destination = await accounts.create(
        name: 'Stage 10 destination $suffix',
        type: 'bank',
        openingBalance: '100',
      );
      final expenseCategory = (await categories.list(
        type: 'expense',
        isActive: true,
      )).first;

      await finance.createTransfer({
        'sourceAccountId': source.id,
        'destinationAccountId': destination.id,
        'amount': '250.0000',
        'currency': 'TRY',
        'transferDate': date,
        'description': 'Stage 10 transfer',
      });
      await finance.createCard({
        'name': 'Stage 10 card $suffix',
        'limit': '1000.0000',
        'currency': 'TRY',
        'statementClosingDay': now.day.clamp(1, 28),
        'paymentDueDay': now.day < 28 ? now.day + 1 : 1,
      });
      var snapshot = await finance.load();
      final card = snapshot.cards.single;
      await finance.createCharge(card.id, {
        'categoryId': expenseCategory.id,
        'amount': '300.0000',
        'currency': 'TRY',
        'chargeDate': date,
        'description': 'Stage 10 purchase',
      });
      await finance.createPayment(card.id, {
        'accountId': source.id,
        'amount': '100.0000',
        'currency': 'TRY',
        'paymentDate': date,
        'description': 'Stage 10 card payment',
      });
      final planInput = {
        'creditCardId': card.id,
        'categoryId': expenseCategory.id,
        'clientRequestId': requestId,
        'totalAmount': '100.0000',
        'currency': 'TRY',
        'installmentCount': 3,
        'firstInstallmentDate': date,
        'description': 'Stage 10 installment',
      };
      await finance.createPlan(planInput);
      await finance.createPlan(planInput);
      snapshot = await finance.load();
      final plan = snapshot.plans.single;
      await finance.realizeInstallment(plan.id, 1);
      await finance.realizeInstallment(plan.id, 1);

      snapshot = await finance.load();
      final activity = await finance.loadActivity(card.id);
      final statement = await finance.loadStatement(
        card.id,
        now.year,
        now.month,
        date,
      );
      final accountPage = await accounts.list();
      final report = await dashboard.getMonthly(now.year, now.month);
      final updatedCard = snapshot.cards.single;

      expect(snapshot.transfers, hasLength(1));
      expect(snapshot.plans, hasLength(1));
      expect(activity.charges, hasLength(2));
      expect(activity.payments, hasLength(1));
      expect(updatedCard.currentDebt, '233.3333');
      expect(report.totalExpense, '333.3333');
      expect(statement.statementBalance, '233.3333');
      expect(
        accountPage.items.singleWhere((item) => item.id == source.id).balance,
        '650.0000',
      );
      expect(
        accountPage.items
            .singleWhere((item) => item.id == destination.id)
            .balance,
        '350.0000',
      );

      await auth.logout();
      expect(await sessionStore.read(), isNull);
    },
  );
}

class _MemorySessionStore implements SessionStore {
  AuthSession? session;
  @override
  Future<void> clear() async => session = null;
  @override
  Future<AuthSession?> read() async => session;
  @override
  Future<void> write(AuthSession session) async => this.session = session;
}

String _dateText(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';

String _requestId(int suffix) {
  final tail = suffix.toRadixString(16).padLeft(12, '0');
  return '00000000-0000-4000-8000-${tail.substring(tail.length - 12)}';
}
