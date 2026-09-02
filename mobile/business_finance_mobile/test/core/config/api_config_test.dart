import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';

void main() {
  test('uses the Android emulator host as the secret-free default', () {
    final config = ApiConfig.fromEnvironment(isWeb: false);

    expect(config.baseUrl.toString(), '${ApiConfig.androidEmulatorBaseUrl}/');
    expect(config.baseUrl.userInfo, isEmpty);
  });

  test('uses the local API as the web development default', () {
    final config = ApiConfig.fromEnvironment(isWeb: true);

    expect(config.baseUrl.toString(), '${ApiConfig.webDevelopmentBaseUrl}/');
    expect(config.baseUrl.userInfo, isEmpty);
  });

  test('normalizes a configured base URL', () {
    final config = ApiConfig.fromEnvironment(
      value: 'https://api.example.test/v1',
    );

    expect(config.baseUrl.toString(), 'https://api.example.test/v1/');
  });

  test('rejects credentials, query, fragment and unsupported schemes', () {
    for (final value in [
      'ftp://api.example.test',
      'https://user:secret@api.example.test',
      'https://api.example.test?token=secret',
      'https://api.example.test#fragment',
    ]) {
      expect(
        () => ApiConfig.fromEnvironment(value: value),
        throwsArgumentError,
        reason: value,
      );
    }
  });
}
