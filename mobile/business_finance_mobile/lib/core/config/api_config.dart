import 'package:flutter/foundation.dart';

class ApiConfig {
  ApiConfig._(this.baseUrl);

  static const String androidEmulatorBaseUrl = 'http://10.0.2.2:5284';
  static const String webDevelopmentBaseUrl = 'http://localhost:5284';

  final Uri baseUrl;

  factory ApiConfig.fromEnvironment({
    String value = const String.fromEnvironment(
      'API_BASE_URL',
      defaultValue: '',
    ),
    bool isWeb = kIsWeb,
  }) {
    final configuredValue = value.trim();
    final effectiveValue = configuredValue.isEmpty
        ? (isWeb ? webDevelopmentBaseUrl : androidEmulatorBaseUrl)
        : configuredValue;
    final uri = Uri.tryParse(effectiveValue);
    if (uri == null ||
        !uri.isAbsolute ||
        (uri.scheme != 'http' && uri.scheme != 'https') ||
        uri.host.isEmpty ||
        uri.userInfo.isNotEmpty ||
        uri.hasQuery ||
        uri.hasFragment) {
      throw ArgumentError.value(
        effectiveValue,
        'API_BASE_URL',
        'Geçerli bir HTTP(S) adresi olmalıdır.',
      );
    }

    final normalizedPath = uri.path.endsWith('/') ? uri.path : '${uri.path}/';
    return ApiConfig._(uri.replace(path: normalizedPath));
  }
}
