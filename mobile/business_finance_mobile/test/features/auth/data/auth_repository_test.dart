import 'dart:async';

import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/storage/secure_session_store.dart';
import 'package:business_finance_mobile/features/auth/data/auth_models.dart';
import 'package:business_finance_mobile/features/auth/data/auth_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_service.dart';

void main() {
  final now = DateTime.utc(2026, 8, 9, 12);

  test('login stores the complete session without a password field', () async {
    final service = FakeAuthService(session: _session(now));
    final store = FakeSessionStore();
    final repository = AuthRepository(
      remoteService: service,
      sessionStore: store,
      utcNow: () => now,
    );

    final session = await repository.login(
      ' user@example.test ',
      'private-password',
    );

    expect(service.loginEmail, 'user@example.test');
    expect(store.session, same(session));
    expect(session.toJson().containsKey('password'), isFalse);
  });

  test(
    'restore refreshes a nearly expired access token and rotates both tokens',
    () async {
      final stored = _session(
        now,
        accessToken: 'old-access',
        refreshToken: 'old-refresh',
        accessExpiresAt: now.add(const Duration(seconds: 30)),
      );
      final rotated = _session(
        now,
        accessToken: 'new-access',
        refreshToken: 'new-refresh',
      );
      final service = FakeAuthService(session: rotated);
      final store = FakeSessionStore()..session = stored;
      final repository = AuthRepository(
        remoteService: service,
        sessionStore: store,
        utcNow: () => now,
      );

      final restored = await repository.restoreSession();

      expect(restored?.accessToken, 'new-access');
      expect(restored?.refreshToken, 'new-refresh');
      expect(store.session?.refreshToken, 'new-refresh');
      expect(service.refreshCount, 1);
    },
  );

  test('concurrent refresh calls share one rotation request', () async {
    final service = FakeAuthService(session: _session(now));
    final store = FakeSessionStore();
    final repository = AuthRepository(
      remoteService: service,
      sessionStore: store,
      utcNow: () => now,
    );
    await repository.login('user@example.test', 'password');
    final refreshCompleter = Completer<AuthSession>();
    service.refreshCompleter = refreshCompleter;

    final first = repository.refreshSession();
    final second = repository.refreshSession();
    await Future<void>.delayed(Duration.zero);

    expect(service.refreshCount, 1);
    refreshCompleter.complete(
      _session(now, accessToken: 'rotated', refreshToken: 'rotated-refresh'),
    );
    expect((await first)?.accessToken, 'rotated');
    expect((await second)?.accessToken, 'rotated');
  });

  test('unauthorized refresh clears the local session', () async {
    final service = FakeAuthService(session: _session(now));
    final store = FakeSessionStore();
    final repository = AuthRepository(
      remoteService: service,
      sessionStore: store,
      utcNow: () => now,
    );
    await repository.login('user@example.test', 'password');
    service.refreshError = const ApiException(
      statusCode: 401,
      code: 'authentication.invalid_refresh_token',
      message: 'Session expired.',
    );

    await expectLater(
      repository.refreshSession(),
      throwsA(isA<ApiException>()),
    );

    expect(repository.currentSession, isNull);
    expect(store.clearCount, 1);
  });

  test('logout clears local session even when server logout fails', () async {
    final service = FakeAuthService(session: _session(now))
      ..logoutError = ApiException.network();
    final store = FakeSessionStore();
    final repository = AuthRepository(
      remoteService: service,
      sessionStore: store,
      utcNow: () => now,
    );
    await repository.login('user@example.test', 'password');

    await expectLater(repository.logout(), throwsA(isA<ApiException>()));

    expect(repository.currentSession, isNull);
    expect(store.clearCount, 1);
  });

  test('corrupt stored JSON state is cleared during restore', () async {
    final store = ThrowingSessionStore();
    final repository = AuthRepository(
      remoteService: FakeAuthService(session: _session(now)),
      sessionStore: store,
      utcNow: () => now,
    );

    expect(await repository.restoreSession(), isNull);
    expect(store.clearCount, 1);
  });
}

AuthSession _session(
  DateTime now, {
  String accessToken = 'access-token',
  String refreshToken = 'refresh-token',
  DateTime? accessExpiresAt,
}) => AuthSession(
  userId: '11111111-1111-1111-1111-111111111111',
  email: 'user@example.test',
  accessToken: accessToken,
  accessTokenExpiresAtUtc:
      accessExpiresAt ?? now.add(const Duration(minutes: 15)),
  refreshToken: refreshToken,
  refreshTokenExpiresAtUtc: now.add(const Duration(days: 30)),
);

class FakeSessionStore implements SessionStore {
  AuthSession? session;
  int clearCount = 0;

  @override
  Future<AuthSession?> read() async => session;

  @override
  Future<void> write(AuthSession session) async => this.session = session;

  @override
  Future<void> clear() async {
    clearCount++;
    session = null;
  }
}

class ThrowingSessionStore implements SessionStore {
  int clearCount = 0;

  @override
  Future<AuthSession?> read() => throw const FormatException('corrupt');

  @override
  Future<void> write(AuthSession session) async {}

  @override
  Future<void> clear() async => clearCount++;
}

class FakeAuthService implements AuthRemoteService {
  FakeAuthService({required this.session});

  AuthSession session;
  String? loginEmail;
  int refreshCount = 0;
  Completer<AuthSession>? refreshCompleter;
  ApiException? refreshError;
  ApiException? logoutError;

  @override
  Future<AuthSession> login(String email, String password) async {
    loginEmail = email;
    return session;
  }

  @override
  Future<void> logout(String refreshToken) async {
    if (logoutError case final error?) {
      throw error;
    }
  }

  @override
  Future<AuthSession> refresh(AuthSession currentSession) async {
    refreshCount++;
    if (refreshError case final error?) {
      throw error;
    }
    return refreshCompleter?.future ?? session;
  }

  @override
  Future<RegisterResult> register(String email, String password) async =>
      RegisterResult(userId: session.userId, email: email);
}
