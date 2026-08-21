import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/features/auth/data/auth_models.dart';
import 'package:business_finance_mobile/features/auth/data/auth_service.dart';

void main() {
  test('login maps the backend token pair contract', () async {
    final service = _service((request) async {
      expect(request.url.path, '/api/v1/auth/login');
      expect(jsonDecode(request.body), {
        'email': 'user@example.test',
        'password': 'Valid-Password-123!',
      });
      return http.Response(jsonEncode(_sessionJson()), 200);
    });

    final session = await service.login(
      'user@example.test',
      'Valid-Password-123!',
    );

    expect(session.accessToken, 'access-token');
    expect(session.refreshToken, 'refresh-token');
    expect(session.accessTokenExpiresAtUtc.isUtc, isTrue);
  });

  test(
    'refresh preserves identity and atomically maps rotated tokens',
    () async {
      final current = AuthSession.fromJson(_sessionJson());
      final service = _service((request) async {
        expect(jsonDecode(request.body), {'refreshToken': 'refresh-token'});
        return http.Response(
          jsonEncode({
            'accessToken': 'new-access',
            'accessTokenExpiresAtUtc': '2026-08-09T13:00:00Z',
            'refreshToken': 'new-refresh',
            'refreshTokenExpiresAtUtc': '2026-09-09T12:00:00Z',
          }),
          200,
        );
      });

      final rotated = await service.refresh(current);

      expect(rotated.userId, current.userId);
      expect(rotated.email, current.email);
      expect(rotated.accessToken, 'new-access');
      expect(rotated.refreshToken, 'new-refresh');
    },
  );

  test('register maps identity only and does not expect tokens', () async {
    final service = _service(
      (_) async => http.Response(
        '{"userId":"11111111-1111-1111-1111-111111111111","email":"new@example.test"}',
        201,
      ),
    );

    final result = await service.register(
      'new@example.test',
      'Valid-Password-123!',
    );

    expect(result.email, 'new@example.test');
  });
}

ApiAuthService _service(Future<http.Response> Function(http.Request) handler) {
  return ApiAuthService(
    ApiClient(
      config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
      httpClient: MockClient(handler),
    ),
  );
}

Map<String, dynamic> _sessionJson() => {
  'userId': '11111111-1111-1111-1111-111111111111',
  'email': 'user@example.test',
  'accessToken': 'access-token',
  'accessTokenExpiresAtUtc': '2026-08-09T12:15:00Z',
  'refreshToken': 'refresh-token',
  'refreshTokenExpiresAtUtc': '2026-09-08T12:00:00Z',
};
