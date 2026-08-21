class ApiException implements Exception {
  const ApiException({
    required this.code,
    required this.message,
    this.statusCode,
    this.traceId,
  });

  final String code;
  final String message;
  final int? statusCode;
  final String? traceId;

  bool get isUnauthorized => statusCode == 401;

  factory ApiException.fromProblemDetails(
    int statusCode,
    Map<String, dynamic> json,
  ) {
    final code = json['code'] as String? ?? 'server.request_failed';
    final serverMessage =
        json['detail'] as String? ??
        json['title'] as String? ??
        'İstek tamamlanamadı.';
    return ApiException(
      statusCode: statusCode,
      code: code,
      message: _localizedMessage(code, serverMessage),
      traceId: json['traceId'] as String?,
    );
  }

  static String _localizedMessage(String code, String fallback) =>
      switch (code) {
        'authentication.invalid_credentials' => 'E-posta veya parola geçersiz.',
        'authentication.required' ||
        'auth.authentication_required' ||
        'imports.authentication_required' =>
          'Oturumunuz sona erdi. Lütfen yeniden giriş yapın.',
        'authentication.invalid_refresh_token' =>
          'Oturum yenilenemedi. Lütfen yeniden giriş yapın.',
        'authentication.registration_conflict' =>
          'Bu e-posta adresiyle daha önce kayıt oluşturulmuş.',
        'authentication.password_policy' =>
          'Parola güvenlik kurallarını karşılamıyor.',
        'authentication.invalid_registration' =>
          'Kayıt bilgilerini kontrol edip tekrar deneyin.',
        'recurring.has_realized_history' =>
          'Bu plan daha önce gerçekleşmiş hareket ürettiği için silinemez. '
              'Durdurmak için planı duraklatabilirsiniz.',
        'authorization.forbidden' ||
        'accounts.forbidden' => 'Bu işlem için yetkiniz bulunmuyor.',
        'request.invalid_format' => 'Gönderilen bilgilerin biçimi geçersiz.',
        'server.unexpected_error' =>
          'Beklenmeyen bir sunucu hatası oluştu. Lütfen tekrar deneyin.',
        _ => fallback,
      };

  factory ApiException.network() => const ApiException(
    code: 'network.unavailable',
    message: 'Sunucuya ulaşılamadı. Bağlantınızı kontrol edip tekrar deneyin.',
  );

  factory ApiException.timeout() => const ApiException(
    code: 'network.timeout',
    message: 'İstek zaman aşımına uğradı. Lütfen tekrar deneyin.',
  );

  @override
  String toString() => 'ApiException($code, statusCode: $statusCode)';
}
