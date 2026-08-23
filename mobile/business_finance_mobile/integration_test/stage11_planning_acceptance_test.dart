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
import 'package:business_finance_mobile/features/categories/data/category_repository.dart';
import 'package:business_finance_mobile/features/planning/data/planning_repository.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets(
    'validates recurring idempotency upcoming feed and advanced report with real SQL',
    (tester) async {
      final suffix = DateTime.now().microsecondsSinceEpoch;
      final email = 'stage11.planning.$suffix@example.com';
      const password = 'Stage11-Planning-2026';
      final today = DateTime.now();
      final date = _dateText(today);
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

      await auth.register(email, password, hasBusiness: false);
      await auth.login(email, password);
      final client = ApiClient(
        config: ApiConfig.fromEnvironment(),
        httpClient: httpClient,
        accessTokenProvider: auth.getValidAccessToken,
        onUnauthorized: () async => (await auth.refreshSession())?.accessToken,
      );
      final accounts = ApiAccountRepository(client);
      final categories = ApiCategoryRepository(client);
      final planning = PlanningRepository(client);
      final account = await accounts.create(
        name: 'Stage 11 account $suffix',
        type: 'bank',
        openingBalance: '1000',
      );
      final expenseCategory = (await categories.list(
        type: 'expense',
        isActive: true,
      )).first;

      await planning.createRecurring({
        'accountId': account.id,
        'categoryId': expenseCategory.id,
        'amount': '125.0000',
        'currency': 'TRY',
        'kind': 'bill-payment',
        'frequency': 'monthly',
        'startDate': date,
        'endDate': null,
        'monthEndBehavior': 'clamp-to-last-day',
        'description': 'Stage 11 internet',
      });
      var snapshot = await planning.load(
        year: today.year,
        month: today.month,
        asOfDate: date,
        daysAhead: 30,
      );
      expect(snapshot.recurringTransactions, hasLength(1));
      // Hiç kimse bir şey üretmedi: kayıt henüz yok, ama yükümlülük planlanan
      // görünümde zaten duruyor.
      expect(snapshot.occurrences, isEmpty);
      expect(
        snapshot.upcomingPayments.where(
          (item) => item.sourceType == 'recurring-occurrence',
        ),
        hasLength(1),
      );

      // Gerçekleştirme eksik kaydı kendi üretiyor ve iki çağrı tek hareket
      // yazıyor; ayrı bir "üret" adımı yok.
      final planId = snapshot.recurringTransactions.single.id;
      await planning.realizeDue(planId, date);
      await planning.realizeDue(planId, date);
      snapshot = await planning.load(
        year: today.year,
        month: today.month,
        asOfDate: date,
        daysAhead: 30,
      );
      expect(snapshot.occurrences, hasLength(1));
      expect(snapshot.occurrences.single.status, 'realized');
      expect(snapshot.report.currentPeriod.expense, '125.0000');
      expect(snapshot.report.netWorth, '875.0000');
      expect(
        snapshot.upcomingPayments.where(
          (item) => item.sourceType == 'recurring-occurrence',
        ),
        isEmpty,
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
