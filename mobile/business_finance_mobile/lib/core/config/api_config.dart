class ApiConfig {
  ApiConfig._(this.baseUrl);

  static const String defaultBaseUrl = 'http://10.0.2.2:5284';

  final Uri baseUrl;

  factory ApiConfig.fromEnvironment({
    String value = const String.fromEnvironment(
      'API_BASE_URL',
      defaultValue: defaultBaseUrl,
    ),
  }) {
    final uri = Uri.tryParse(value.trim());
    if (uri == null ||
        !uri.isAbsolute ||
        (uri.scheme != 'http' && uri.scheme != 'https') ||
        uri.host.isEmpty ||
        uri.userInfo.isNotEmpty ||
        uri.hasQuery ||
        uri.hasFragment) {
      throw ArgumentError.value(
        value,
        'API_BASE_URL',
        'Geçerli bir HTTP(S) adresi olmalıdır.',
      );
    }

    final normalizedPath = uri.path.endsWith('/') ? uri.path : '${uri.path}/';
    return ApiConfig._(uri.replace(path: normalizedPath));
  }
}
