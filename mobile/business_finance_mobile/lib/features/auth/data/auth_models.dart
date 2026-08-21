class AuthSession {
  const AuthSession({
    required this.userId,
    required this.email,
    required this.accessToken,
    required this.accessTokenExpiresAtUtc,
    required this.refreshToken,
    required this.refreshTokenExpiresAtUtc,
  });

  final String userId;
  final String email;
  final String accessToken;
  final DateTime accessTokenExpiresAtUtc;
  final String refreshToken;
  final DateTime refreshTokenExpiresAtUtc;

  factory AuthSession.fromJson(Map<String, dynamic> json) => AuthSession(
    userId: _requiredString(json, 'userId'),
    email: _requiredString(json, 'email'),
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
    'accessToken': accessToken,
    'accessTokenExpiresAtUtc': accessTokenExpiresAtUtc.toIso8601String(),
    'refreshToken': refreshToken,
    'refreshTokenExpiresAtUtc': refreshTokenExpiresAtUtc.toIso8601String(),
  };

  AuthSession rotate({
    required String newAccessToken,
    required DateTime newAccessTokenExpiresAtUtc,
    required String newRefreshToken,
    required DateTime newRefreshTokenExpiresAtUtc,
  }) => AuthSession(
    userId: userId,
    email: email,
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
