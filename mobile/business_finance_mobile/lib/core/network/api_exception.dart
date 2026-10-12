import 'api_error_messages.dart';

class ApiException implements Exception {
  const ApiException({
    required this.code,
    required this.message,
    this.statusCode,
    this.traceId,
    this.pendingScheduledDate,
    this.pendingCount,
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

  /// Sunucu bir hata değil, onay bildiriyor: vadesi gelmemiş tekrarlayan kalem
  /// yazılmak isteniyor ama aynı planın daha önceki bir kalemi bekliyor. Ekran
  /// sorar ve onayla aynı isteği yineler.
  bool get isEarlierItemPending => code == 'recurring.earlier_item_pending';

  /// Onay isteğinin ayrıntısı: bekleyen en eski kalemin vadesi (`yyyy-MM-dd`)
  /// ve kaç kalemin beklediği. Onay kutusu hangi kalemin beklediğini bunlarla
  /// yazar; ayrıntı gelmediyse kalemin gününü vermeden sorar.
  final String? pendingScheduledDate;
  final int? pendingCount;

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
      pendingScheduledDate: json['pendingScheduledDate'] as String?,
      pendingCount: int.tryParse('${json['pendingCount'] ?? ''}'),
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
