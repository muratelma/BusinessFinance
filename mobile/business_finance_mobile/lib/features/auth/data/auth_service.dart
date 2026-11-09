import '../../../core/network/api_client.dart';
import 'auth_models.dart';

abstract interface class AuthRemoteService {
  /// [username] isteğe bağlıdır; gönderilmezse hesabın kullanıcı adı
  /// e-postası olur. Kayıt formu her zaman gönderir.
  Future<RegisterResult> register(
    String email,
    String password, {
    required bool hasBusiness,
    String? username,
  });

  /// Kayıt formunun sorusu: bu kullanıcı adı boş mu. Kural dışı ad
  /// `authentication.invalid_username` ile reddedilir.
  Future<bool> isUsernameAvailable(String username);

  Future<AuthSession> login(String email, String password);

  /// Google kimlik belirtecini sunucuya götürür. [username] yalnız "Bir adım
  /// kaldı"daki `Devam et`'te gönderilir; hesap o istekte açılır.
  Future<GoogleSignInOutcome> googleSignIn(
    String idToken, {
    String? username,
    bool hasBusiness = false,
  });

  Future<AuthSession> refresh(AuthSession currentSession);

  Future<void> logout(String refreshToken);

  Future<void> requestPasswordReset(String email);

  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  });
}

class ApiAuthService implements AuthRemoteService {
  ApiAuthService(this._apiClient);

  final ApiClient _apiClient;

  @override
  Future<RegisterResult> register(
    String email,
    String password, {
    required bool hasBusiness,
    String? username,
  }) async {
    final response = await _apiClient.post(
      '/api/v1/auth/register',
      body: {
        'email': email,
        'password': password,
        'username': ?username,
        // Onboarding'in tek sorusu. Sonraya bırakılamaz: varsayılan kategori
        // seti ilk kategori okumasında kuruluyor ve yalnız bir kez kuruluyor.
        'hasBusiness': hasBusiness,
      },
    );
    return RegisterResult.fromJson(response.requireObject());
  }

  @override
  Future<AuthSession> login(String email, String password) async {
    final response = await _apiClient.post(
      '/api/v1/auth/login',
      body: {'email': email, 'password': password},
    );
    return AuthSession.fromJson(response.requireObject());
  }

  @override
  Future<GoogleSignInOutcome> googleSignIn(
    String idToken, {
    String? username,
    bool hasBusiness = false,
  }) async {
    final response = await _apiClient.post(
      '/api/v1/auth/google',
      body: {
        'idToken': idToken,
        'username': ?username,
        'hasBusiness': hasBusiness,
      },
    );
    return GoogleSignInOutcome.fromJson(response.requireObject());
  }

  @override
  Future<AuthSession> refresh(AuthSession currentSession) async {
    final response = await _apiClient.post(
      '/api/v1/auth/refresh',
      body: {'refreshToken': currentSession.refreshToken},
    );
    final json = response.requireObject();
    return currentSession.rotate(
      newSessionId: _optionalString(json, 'sessionId'),
      newAccessToken: _requiredString(json, 'accessToken'),
      newAccessTokenExpiresAtUtc: _requiredUtc(json, 'accessTokenExpiresAtUtc'),
      newRefreshToken: _requiredString(json, 'refreshToken'),
      newRefreshTokenExpiresAtUtc: _requiredUtc(
        json,
        'refreshTokenExpiresAtUtc',
      ),
    );
  }

  @override
  Future<bool> isUsernameAvailable(String username) async {
    final response = await _apiClient.get(
      '/api/v1/auth/username-availability'
      '?username=${Uri.encodeQueryComponent(username)}',
    );
    final available = response.requireObject()['available'];
    if (available is! bool) {
      throw const FormatException('available alanı eksik.');
    }
    return available;
  }

  @override
  Future<void> requestPasswordReset(String email) async {
    await _apiClient.post(
      '/api/v1/auth/password-reset',
      body: {'email': email},
    );
  }

  @override
  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  }) async {
    await _apiClient.post(
      '/api/v1/auth/password-reset/confirm',
      body: {'email': email, 'code': code, 'newPassword': newPassword},
    );
  }

  @override
  Future<void> logout(String refreshToken) async {
    await _apiClient.post(
      '/api/v1/auth/logout',
      body: {'refreshToken': refreshToken},
    );
  }
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

DateTime _requiredUtc(Map<String, dynamic> json, String key) {
  final parsed = DateTime.tryParse(_requiredString(json, key));
  if (parsed == null) {
    throw FormatException('Missing or invalid $key.');
  }
  return parsed.toUtc();
}
