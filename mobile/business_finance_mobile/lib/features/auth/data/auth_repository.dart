import '../../../core/network/api_exception.dart';
import '../../../core/storage/secure_session_store.dart';
import 'auth_models.dart';
import 'auth_service.dart';

abstract interface class AuthSessionRepository {
  AuthSession? get currentSession;

  Future<RegisterResult> register(
    String email,
    String password, {
    required bool hasBusiness,
  });

  Future<AuthSession> login(String email, String password);

  Future<AuthSession?> restoreSession();

  Future<String?> getValidAccessToken();

  Future<AuthSession?> refreshSession();

  /// Parola değiştirme gibi, sunucunun **kendiliğinden** taze bir token çifti
  /// verdiği durumlarda çağrılır: bütün oturumlar kapandığı için elimizdeki
  /// refresh token artık ölüdür ve yenisi hemen benimsenmezse kullanıcı bir
  /// sonraki istekte uygulamadan düşer.
  Future<AuthSession?> adoptRotatedTokens({
    String? sessionId,
    required String accessToken,
    required DateTime accessTokenExpiresAtUtc,
    required String refreshToken,
    required DateTime refreshTokenExpiresAtUtc,
  });

  Future<void> logout();

  /// Parolasını unutan kullanıcı için kod ister. Kayıtlı olmayan adres de
  /// başarıyla döner: sunucu hangi adresin hesabı olduğunu söylemez.
  Future<void> requestPasswordReset(String email);

  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  });
}

class AuthRepository implements AuthSessionRepository {
  factory AuthRepository({
    required AuthRemoteService remoteService,
    required SessionStore sessionStore,
    DateTime Function()? utcNow,
  }) => AuthRepository._(
    remoteService,
    sessionStore,
    utcNow ?? (() => DateTime.now().toUtc()),
  );

  AuthRepository._(this._remoteService, this._sessionStore, this._utcNow);

  static const _refreshMargin = Duration(seconds: 60);

  final AuthRemoteService _remoteService;
  final SessionStore _sessionStore;
  final DateTime Function() _utcNow;

  AuthSession? _session;
  Future<AuthSession?>? _refreshInFlight;

  @override
  AuthSession? get currentSession => _session;

  @override
  Future<RegisterResult> register(
    String email,
    String password, {
    required bool hasBusiness,
  }) =>
      _remoteService.register(email.trim(), password, hasBusiness: hasBusiness);

  @override
  Future<AuthSession> login(String email, String password) async {
    final session = await _remoteService.login(email.trim(), password);
    await _sessionStore.write(session);
    _session = session;
    return session;
  }

  @override
  Future<AuthSession?> restoreSession() async {
    try {
      final stored = await _sessionStore.read();
      if (stored == null) {
        _session = null;
        return null;
      }
      _session = stored;
      if (!stored.refreshTokenExpiresAtUtc.isAfter(_utcNow())) {
        await _clearSession();
        return null;
      }
      if (_needsRefresh(stored)) {
        return refreshSession();
      }
      return stored;
    } on FormatException {
      await _clearSession();
      return null;
    }
  }

  @override
  Future<String?> getValidAccessToken() async {
    final session = _session ?? await restoreSession();
    if (session == null) {
      return null;
    }
    final validSession = _needsRefresh(session)
        ? await refreshSession()
        : session;
    return validSession?.accessToken;
  }

  @override
  Future<AuthSession?> refreshSession() {
    return _refreshInFlight ??= _refreshOnce().whenComplete(
      () => _refreshInFlight = null,
    );
  }

  Future<AuthSession?> _refreshOnce() async {
    final session = _session;
    if (session == null ||
        !session.refreshTokenExpiresAtUtc.isAfter(_utcNow())) {
      await _clearSession();
      return null;
    }

    try {
      final rotated = await _remoteService.refresh(session);
      await _sessionStore.write(rotated);
      _session = rotated;
      return rotated;
    } on ApiException catch (error) {
      if (error.isUnauthorized) {
        await _clearSession();
      }
      rethrow;
    } on FormatException {
      await _clearSession();
      rethrow;
    }
  }

  @override
  Future<AuthSession?> adoptRotatedTokens({
    String? sessionId,
    required String accessToken,
    required DateTime accessTokenExpiresAtUtc,
    required String refreshToken,
    required DateTime refreshTokenExpiresAtUtc,
  }) async {
    final session = _session;
    if (session == null) {
      return null;
    }
    final rotated = session.rotate(
      newSessionId: sessionId,
      newAccessToken: accessToken,
      newAccessTokenExpiresAtUtc: accessTokenExpiresAtUtc,
      newRefreshToken: refreshToken,
      newRefreshTokenExpiresAtUtc: refreshTokenExpiresAtUtc,
    );
    await _sessionStore.write(rotated);
    _session = rotated;
    return rotated;
  }

  @override
  Future<void> requestPasswordReset(String email) =>
      _remoteService.requestPasswordReset(email.trim());

  @override
  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  }) => _remoteService.resetPassword(
    email: email.trim(),
    code: code.trim(),
    newPassword: newPassword,
  );

  @override
  Future<void> logout() async {
    final refreshToken = _session?.refreshToken;
    try {
      if (refreshToken != null) {
        await _remoteService.logout(refreshToken);
      }
    } finally {
      await _clearSession();
    }
  }

  bool _needsRefresh(AuthSession session) =>
      !session.accessTokenExpiresAtUtc.isAfter(_utcNow().add(_refreshMargin));

  Future<void> _clearSession() async {
    _session = null;
    await _sessionStore.clear();
  }
}
