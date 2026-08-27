import '../../../core/models/json_readers.dart';

/// Kullanıcının kendi hesabı. Sunucu yalnız kullanıcının kendisine ait
/// bilgiyi döner; parola, token veya token hash'i hiçbir alanda yoktur.
class UserAccount {
  const UserAccount({
    required this.userId,
    required this.email,
    required this.emailConfirmed,
    required this.createdAtUtc,
    required this.activeSessionCount,
  });

  final String userId;
  final String email;
  final bool emailConfirmed;
  final DateTime createdAtUtc;
  final int activeSessionCount;

  factory UserAccount.fromJson(Map<String, dynamic> json) => UserAccount(
    userId: JsonReaders.string(json, 'userId'),
    email: JsonReaders.string(json, 'email'),
    emailConfirmed: JsonReaders.boolean(json, 'emailConfirmed'),
    createdAtUtc: _utc(json, 'createdAtUtc'),
    activeSessionCount: JsonReaders.integer(json, 'activeSessionCount'),
  );
}

/// Açık bir oturum satırı.
///
/// Ne olduğunu değil ne zaman açıldığını söyler: cihaz adı, IP ve konum
/// taşınmıyor çünkü sunucu bunları hiç saklamıyor. Uydurulmuş bir "Windows,
/// İstanbul" satırı, doğruluğu denetlenemeyen bir güven vaadi olurdu.
class UserSessionSummary {
  const UserSessionSummary({
    required this.sessionId,
    required this.createdAtUtc,
    required this.expiresAtUtc,
  });

  final String sessionId;
  final DateTime createdAtUtc;
  final DateTime expiresAtUtc;

  factory UserSessionSummary.fromJson(Map<String, dynamic> json) =>
      UserSessionSummary(
        sessionId: JsonReaders.string(json, 'sessionId'),
        createdAtUtc: _utc(json, 'createdAtUtc'),
        expiresAtUtc: _utc(json, 'expiresAtUtc'),
      );
}

/// Kod gönderme isteğinin sonucu.
///
/// `codeSent` yanlışsa istek başarılıdır ama posta gitmemiştir — sunucuda
/// gönderici yapılandırılmamış olabilir. Kullanıcıya "kod yolda" demek, hiç
/// gelmeyecek bir postayı beklemesine sebep olurdu.
class VerificationSendResult {
  const VerificationSendResult({
    required this.alreadyConfirmed,
    required this.codeSent,
  });

  final bool alreadyConfirmed;
  final bool codeSent;

  factory VerificationSendResult.fromJson(Map<String, dynamic> json) =>
      VerificationSendResult(
        alreadyConfirmed: JsonReaders.boolean(json, 'alreadyConfirmed'),
        codeSent: JsonReaders.boolean(json, 'codeSent'),
      );
}

/// Parola değişiminden sonra sunucunun verdiği taze token çifti.
class RotatedTokens {
  const RotatedTokens({
    required this.sessionId,
    required this.accessToken,
    required this.accessTokenExpiresAtUtc,
    required this.refreshToken,
    required this.refreshTokenExpiresAtUtc,
  });

  final String sessionId;
  final String accessToken;
  final DateTime accessTokenExpiresAtUtc;
  final String refreshToken;
  final DateTime refreshTokenExpiresAtUtc;

  factory RotatedTokens.fromJson(Map<String, dynamic> json) => RotatedTokens(
    sessionId: JsonReaders.string(json, 'sessionId'),
    accessToken: JsonReaders.string(json, 'accessToken'),
    accessTokenExpiresAtUtc: _utc(json, 'accessTokenExpiresAtUtc'),
    refreshToken: JsonReaders.string(json, 'refreshToken'),
    refreshTokenExpiresAtUtc: _utc(json, 'refreshTokenExpiresAtUtc'),
  );
}

/// Sunucu zaman damgalarını UTC gönderir; istemci de UTC tutar ve yalnız
/// gösterirken yerel saate çevirir.
DateTime _utc(Map<String, dynamic> json, String key) {
  final parsed = DateTime.tryParse(JsonReaders.string(json, key));
  if (parsed == null) {
    throw FormatException('Invalid $key.');
  }
  return parsed.toUtc();
}
