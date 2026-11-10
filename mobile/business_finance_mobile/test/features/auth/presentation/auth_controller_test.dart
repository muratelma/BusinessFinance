import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_controller.dart';

import '../../../helpers/fake_auth.dart';

void main() {
  test('initialize exposes restoring then unauthenticated state', () async {
    final repository = FakeAuthSessionRepository();
    final controller = AuthController(repository);

    expect(controller.status, AuthStatus.restoring);
    await controller.initialize();

    expect(controller.status, AuthStatus.unauthenticated);
    expect(controller.sessionGeneration, 0);
  });

  test('login changes state only after repository success', () async {
    final repository = FakeAuthSessionRepository();
    final controller = AuthController(repository);
    await controller.initialize();

    await controller.login('user@example.test', 'password');

    expect(controller.status, AuthStatus.authenticated);
    expect(controller.session?.email, 'user@example.test');
    expect(controller.isSubmitting, isFalse);
    expect(controller.sessionGeneration, 1);
  });

  test('Özet geçişten önce, sunucu kabul ettikten sonra hazırlanır', () async {
    final repository = FakeAuthSessionRepository();
    final controller = AuthController(repository);
    await controller.initialize();
    final order = <String>[];
    controller.prepareHome = () async {
      // Oturum anahtarı kayıtlı ama ekranlar henüz açılmadı.
      expect(repository.currentSession, isNotNull);
      expect(controller.status, AuthStatus.unauthenticated);
      order.add('prepare');
    };

    await controller.login(
      'user@example.test',
      'password',
      onVerified: () async => order.add('transition'),
    );

    expect(order, ['prepare', 'transition']);
    expect(controller.status, AuthStatus.authenticated);
  });

  test('Özet hazırlanamazsa giriş yine açılır', () async {
    final repository = FakeAuthSessionRepository();
    final controller = AuthController(repository)
      ..prepareHome = () async => throw ApiException.network();
    await controller.initialize();

    await controller.login('user@example.test', 'password');

    expect(controller.status, AuthStatus.authenticated);
  });

  test(
    'failed login preserves unauthenticated state and safe message',
    () async {
      final repository = FakeAuthSessionRepository()
        ..loginError = const ApiException(
          statusCode: 401,
          code: 'authentication.invalid_credentials',
          message: 'E-posta veya parola geçersiz.',
        );
      final controller = AuthController(repository);
      await controller.initialize();

      await expectLater(
        controller.login('user@example.test', 'wrong'),
        throwsA(isA<ApiException>()),
      );

      expect(controller.status, AuthStatus.unauthenticated);
      expect(controller.errorMessage, 'E-posta veya parola geçersiz.');
    },
  );

  test('logout always publishes unauthenticated state', () async {
    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    expect(controller.sessionGeneration, 1);
    repository.logoutError = ApiException.network();

    await controller.logout();

    expect(controller.status, AuthStatus.unauthenticated);
    expect(controller.sessionGeneration, 2);
    expect(controller.errorMessage, contains('Sunucuya ulaşılamadı'));
  });
}
