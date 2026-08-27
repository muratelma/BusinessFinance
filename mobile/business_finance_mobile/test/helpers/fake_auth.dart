import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/auth/data/auth_models.dart';
import 'package:business_finance_mobile/features/auth/data/auth_repository.dart';

AuthSession testSession({
  String userId = '11111111-1111-1111-1111-111111111111',
  String email = 'user@example.test',
  String? sessionId = '33333333-3333-3333-3333-333333333333',
}) => AuthSession(
  userId: userId,
  email: email,
  sessionId: sessionId,
  accessToken: 'access-token',
  accessTokenExpiresAtUtc: DateTime.utc(2026, 8, 9, 13),
  refreshToken: 'refresh-token',
  refreshTokenExpiresAtUtc: DateTime.utc(2026, 9, 9, 12),
);

class FakeAuthSessionRepository implements AuthSessionRepository {
  AuthSession? session;
  AuthSession loginSession = testSession();
  ApiException? loginError;
  ApiException? logoutError;

  @override
  AuthSession? get currentSession => session;

  @override
  Future<String?> getValidAccessToken() async => session?.accessToken;

  @override
  Future<AuthSession> login(String email, String password) async {
    if (loginError case final error?) {
      throw error;
    }
    return session = loginSession;
  }

  @override
  Future<void> logout() async {
    session = null;
    if (logoutError case final error?) {
      throw error;
    }
  }

  @override
  Future<AuthSession?> refreshSession() async => session;

  @override
  Future<RegisterResult> register(
    String email,
    String password, {
    required bool hasBusiness,
  }) async => RegisterResult(userId: testSession().userId, email: email);

  @override
  Future<AuthSession?> restoreSession() async => session;

  @override
  Future<AuthSession?> adoptRotatedTokens({
    String? sessionId,
    required String accessToken,
    required DateTime accessTokenExpiresAtUtc,
    required String refreshToken,
    required DateTime refreshTokenExpiresAtUtc,
  }) async {
    final current = session;
    if (current == null) {
      return null;
    }
    return session = current.rotate(
      newSessionId: sessionId,
      newAccessToken: accessToken,
      newAccessTokenExpiresAtUtc: accessTokenExpiresAtUtc,
      newRefreshToken: refreshToken,
      newRefreshTokenExpiresAtUtc: refreshTokenExpiresAtUtc,
    );
  }
}
