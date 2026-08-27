class AuthSession {
  const AuthSession({
    required this.userId,
    required this.email,
    this.sessionId,
    required this.accessToken,
    required this.accessTokenExpiresAtUtc,
    required this.refreshToken,
    required this.refreshTokenExpiresAtUtc,
  });

  final String userId;
  final String email;

  /// Bu cihazın açık oturumunun kimliği. Sır değildir — token olmadan hiçbir
  /// işe yaramaz — ama `Hesabım` sayfasının oturum listesinde hangi satırın
  /// kendisi olduğunu bilmesini sağlar. Eski bir kurulumda saklanmış oturumda
  /// bulunmayabilir; o hâlde hiçbir satır "bu cihaz" diye işaretlenmez.
  final String? sessionId;
  final String accessToken;
  final DateTime accessTokenExpiresAtUtc;
  final String refreshToken;
  final DateTime refreshTokenExpiresAtUtc;

  factory AuthSession.fromJson(Map<String, dynamic> json) => AuthSession(
    userId: _requiredString(json, 'userId'),
    email: _requiredString(json, 'email'),
    sessionId: _optionalString(json, 'sessionId'),
    accessToken: _requiredString(json, 'accessToken'),
    accessTokenExpiresAtUtc: _requiredUtcDateTime(
      json,
      'accessTokenExpiresAtUtc',
    ),
    refreshToken: _requiredString(json, 'refreshToken'),
    refreshTokenExpiresAtUtc: _requiredUtcDateTime(
      json,
      'refreshTokenExpiresAtUtc',
    ),
  );

  Map<String, dynamic> toJson() => {
    'userId': userId,
    'email': email,
    if (sessionId != null) 'sessionId': sessionId,
    'accessToken': accessToken,
    'accessTokenExpiresAtUtc': accessTokenExpiresAtUtc.toIso8601String(),
    'refreshToken': refreshToken,
    'refreshTokenExpiresAtUtc': refreshTokenExpiresAtUtc.toIso8601String(),
  };

  AuthSession rotate({
    String? newSessionId,
    required String newAccessToken,
    required DateTime newAccessTokenExpiresAtUtc,
    required String newRefreshToken,
    required DateTime newRefreshTokenExpiresAtUtc,
  }) => AuthSession(
    userId: userId,
    email: email,
    sessionId: newSessionId ?? sessionId,
    accessToken: newAccessToken,
    accessTokenExpiresAtUtc: newAccessTokenExpiresAtUtc,
    refreshToken: newRefreshToken,
    refreshTokenExpiresAtUtc: newRefreshTokenExpiresAtUtc,
  );
}

class RegisterResult {
  const RegisterResult({required this.userId, required this.email});

  final String userId;
  final String email;

  factory RegisterResult.fromJson(Map<String, dynamic> json) => RegisterResult(
    userId: _requiredString(json, 'userId'),
    email: _requiredString(json, 'email'),
  );
}

String? _optionalString(Map<String, dynamic> json, String key) {
  final value = json[key];
  return value is String && value.trim().isNotEmpty ? value : null;
}

String _requiredString(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value is! String || value.trim().isEmpty) {
    throw FormatException('Missing or invalid $key.');
  }
  return value;
}

DateTime _requiredUtcDateTime(Map<String, dynamic> json, String key) {
  final value = _requiredString(json, key);
  final parsed = DateTime.tryParse(value);
  if (parsed == null) {
    throw FormatException('Missing or invalid $key.');
  }
  return parsed.toUtc();
}
