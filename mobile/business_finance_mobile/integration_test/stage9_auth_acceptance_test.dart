import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:integration_test/integration_test.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/storage/secure_session_store.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_service.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets(
    'restores, rotates and clears a real session while isolating two users',
    (tester) async {
      final suffix = DateTime.now().microsecondsSinceEpoch;
      final firstEmail = 'stage9.auth.a.$suffix@example.com';
      final secondEmail = 'stage9.auth.b.$suffix@example.com';
      final privateAccountName = 'Stage 9 private account $suffix';
      const password = 'Stage9-Auth-2026';
      final sessionStore = SecureSessionStore();
      final httpClient = http.Client();
      final publicApiClient = ApiClient(
        config: ApiConfig.fromEnvironment(),
        httpClient: httpClient,
      );
      final authService = ApiAuthService(publicApiClient);

      addTearDown(() async {
        await sessionStore.clear();
        httpClient.close();
      });
      await sessionStore.clear();

      final firstRepository = _authRepository(authService, sessionStore);
      await firstRepository.register(firstEmail, password, hasBusiness: false);
      final firstLogin = await firstRepository.login(firstEmail, password);
      final firstAccounts = ApiAccountRepository(
        _protectedApiClient(firstRepository, httpClient),
      );
      await firstAccounts.create(
        name: privateAccountName,
        type: 'cash',
        openingBalance: '1000',
      );

      final restoredRepository = _authRepository(authService, sessionStore);
      final restored = await restoredRepository.restoreSession();

      expect(restored, isNotNull);
      expect(restored!.userId, firstLogin.userId);
      expect(restored.email, firstEmail);

      final rotated = await restoredRepository.refreshSession();

      expect(rotated, isNotNull);
      expect(rotated!.accessToken, isNot(firstLogin.accessToken));
      expect(rotated.refreshToken, isNot(firstLogin.refreshToken));

      final restoredAccounts = ApiAccountRepository(
        _protectedApiClient(restoredRepository, httpClient),
      );
      expect(
        (await restoredAccounts.list()).items.map((account) => account.name),
        contains(privateAccountName),
      );

      await restoredRepository.logout();

      expect(await sessionStore.read(), isNull);
      await expectLater(
        authService.refresh(rotated),
        throwsA(
          isA<ApiException>().having(
            (error) => error.statusCode,
            'statusCode',
            401,
          ),
        ),
      );

      final loggedOutRepository = _authRepository(authService, sessionStore);
      expect(await loggedOutRepository.restoreSession(), isNull);

      await loggedOutRepository.register(
        secondEmail,
        password,
        hasBusiness: false,
      );
      await loggedOutRepository.login(secondEmail, password);
      final secondAccounts = ApiAccountRepository(
        _protectedApiClient(loggedOutRepository, httpClient),
      );

      expect((await secondAccounts.list()).items, isEmpty);

      await loggedOutRepository.logout();
      await loggedOutRepository.login(firstEmail, password);
      final firstAccountsAgain = ApiAccountRepository(
        _protectedApiClient(loggedOutRepository, httpClient),
      );

      expect(
        (await firstAccountsAgain.list()).items.map((account) => account.name),
        contains(privateAccountName),
      );
      await loggedOutRepository.logout();
      expect(await sessionStore.read(), isNull);
    },
  );
}

AuthRepository _authRepository(
  AuthRemoteService service,
  SessionStore sessionStore,
) => AuthRepository(remoteService: service, sessionStore: sessionStore);

ApiClient _protectedApiClient(
  AuthSessionRepository repository,
  http.Client httpClient,
) => ApiClient(
  config: ApiConfig.fromEnvironment(),
  httpClient: httpClient,
  accessTokenProvider: repository.getValidAccessToken,
  onUnauthorized: () async => (await repository.refreshSession())?.accessToken,
);
