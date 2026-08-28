import 'api_error_messages.dart';

class ApiException implements Exception {
  const ApiException({
    required this.code,
    required this.message,
    this.statusCode,
    this.traceId,
  });

  final String code;

  /// Kullanıcıya gösterilebilir Türkçe cümle.
  ///
  /// Her zaman [ApiErrorMessages] tarafından üretilir; sunucunun `detail`
  /// alanı buraya **hiçbir koşulda** girmez. Gerekçe ve katmanlar için
  /// `api_error_messages.dart`.
  final String message;

  final int? statusCode;
  final String? traceId;

  bool get isUnauthorized => statusCode == 401;

  factory ApiException.fromProblemDetails(
    int statusCode,
    Map<String, dynamic> json,
  ) {
    final code = json['code'] as String? ?? 'server.request_failed';
    return ApiException(
      statusCode: statusCode,
      code: code,
      message: ApiErrorMessages.resolve(code),
      traceId: json['traceId'] as String?,
    );
  }

  /// İstemcinin kendi ürettiği hata; cümlesi de koddan gelir.
  factory ApiException.local(String code, {int? statusCode}) => ApiException(
    code: code,
    message: ApiErrorMessages.resolve(code),
    statusCode: statusCode,
  );

  factory ApiException.network() => ApiException.local('network.unavailable');

  factory ApiException.timeout() => ApiException.local('network.timeout');

  @override
  String toString() => 'ApiException($code, statusCode: $statusCode)';
}
