import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../auth/data/auth_repository.dart';
import '../data/account_models.dart';
import '../data/account_repository.dart';

/// `Hesabım` sayfasının durumu.
///
/// Hesap bilgisi ve açık oturumlar tek yükleme turunda okunur; ikisi de aynı
/// sorunun iki yüzü ("bu hesap kimin ve nereden açık?") ve ayrı ayrı yüklemek
/// ekranda iki bağımsız iskelet gösterirdi.
class AccountController extends ChangeNotifier {
  AccountController(this._repository, this._authRepository);

  final AccountRepositoryContract _repository;
  final AuthSessionRepository _authRepository;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  UserAccount? account;
  List<UserSessionSummary> sessions = const [];

  /// Bu cihazın oturum kimliği. Eski bir kurulumdan gelen oturumda
  /// bulunmayabilir; o hâlde hiçbir satır "bu cihaz" diye işaretlenmez —
  /// yanlış satırı işaretlemektense hiç işaretlememek doğrudur.
  String? get currentSessionId => _authRepository.currentSession?.sessionId;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      final loaded = await _repository.read();
      final openSessions = await _repository.listSessions();
      account = loaded;
      sessions = openSessions;
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> revokeSession(String sessionId) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.revokeSession(sessionId);
      sessions = sessions
          .where((session) => session.sessionId != sessionId)
          .toList(growable: false);
      final current = account;
      if (current != null) {
        account = UserAccount(
          userId: current.userId,
          email: current.email,
          emailConfirmed: current.emailConfirmed,
          createdAtUtc: current.createdAtUtc,
          activeSessionCount: sessions.length,
        );
      }
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  /// Parolayı değiştirir ve dönen taze token çiftini **hemen** benimser:
  /// sunucu bütün oturumları kapattığı için elimizdeki refresh token artık
  /// ölüdür ve benimsenmezse kullanıcı bir sonraki istekte düşerdi.
  Future<bool> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      final rotated = await _repository.changePassword(
        currentPassword: currentPassword,
        newPassword: newPassword,
      );
      await _authRepository.adoptRotatedTokens(
        sessionId: rotated.sessionId,
        accessToken: rotated.accessToken,
        accessTokenExpiresAtUtc: rotated.accessTokenExpiresAtUtc,
        refreshToken: rotated.refreshToken,
        refreshTokenExpiresAtUtc: rotated.refreshTokenExpiresAtUtc,
      );
      return true;
    } on ApiException catch (error) {
      // Yanlış parola oturumu düşürmez: bu 401 kimlik doğrulamanın değil,
      // yazılan parolanın cevabıdır.
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<bool> deleteAccount({required String password}) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.deleteAccount(password: password);
      return true;
    } on ApiException catch (error) {
      errorMessage = error.message;
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }
}
