import '../../../core/network/api_client.dart';
import 'auth_models.dart';

abstract interface class AuthRemoteService {
  Future<RegisterResult> register(
    String email,
    String password, {
    required bool hasBusiness,
  });

  Future<AuthSession> login(String email, String password);

  Future<AuthSession> refresh(AuthSession currentSession);

  Future<void> logout(String refreshToken);
}

class ApiAuthService implements AuthRemoteService {
  ApiAuthService(this._apiClient);

  final ApiClient _apiClient;

  @override
  Future<RegisterResult> register(
    String email,
    String password, {
    required bool hasBusiness,
  }) async {
    final response = await _apiClient.post(
      '/api/v1/auth/register',
      body: {
        'email': email,
        'password': password,
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
  Future<AuthSession> refresh(AuthSession currentSession) async {
    final response = await _apiClient.post(
      '/api/v1/auth/refresh',
      body: {'refreshToken': currentSession.refreshToken},
    );
    final json = response.requireObject();
    return currentSession.rotate(
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
  Future<void> logout(String refreshToken) async {
    await _apiClient.post(
      '/api/v1/auth/logout',
      body: {'refreshToken': refreshToken},
    );
  }
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
