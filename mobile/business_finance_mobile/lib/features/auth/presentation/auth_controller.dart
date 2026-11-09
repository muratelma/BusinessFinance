import 'dart:async';

import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../data/auth_models.dart';
import '../data/auth_repository.dart';
import '../data/google_id_token_source.dart';

enum AuthStatus { restoring, authenticated, unauthenticated }

class AuthController extends ChangeNotifier {
  /// [_google] yoksa (testler) Google düğmesi "yapılamadı" der.
  AuthController(this._repository, [this._google]);

  final AuthSessionRepository _repository;
  final GoogleIdTokenSource? _google;

  AuthStatus _status = AuthStatus.restoring;
  bool _isSubmitting = false;
  String? _errorMessage;
  int _sessionGeneration = 0;

  AuthStatus get status => _status;
  bool get isSubmitting => _isSubmitting;
  String? get errorMessage => _errorMessage;
  AuthSession? get session => _repository.currentSession;
  int get sessionGeneration => _sessionGeneration;

  Future<void> initialize() async {
    try {
      final restored = await _repository.restoreSession();
      if (restored == null) {
        _status = AuthStatus.unauthenticated;
      } else {
        _startAuthenticatedSession();
      }
    } on ApiException catch (error) {
      _errorMessage = error.message;
      _status = _repository.currentSession == null
          ? AuthStatus.unauthenticated
          : AuthStatus.authenticated;
    }
    notifyListeners();
  }

  /// [onVerified] sunucu girişi kabul ettikten **sonra**, oturum ekranlara
  /// açılmadan önce beklenir: giriş ekranı Özet'e geçiş animasyonunu bu arada
  /// oynatır. Oturum açılınca yönlendirici giriş ekranını hemen kaldırır;
  /// animasyon ancak bu sırayla görünür.
  Future<void> login(
    String email,
    String password, {
    Future<void> Function()? onVerified,
  }) async {
    if (_isSubmitting) {
      return;
    }
    _setSubmitting(true);
    try {
      await _repository.login(email, password);
      _errorMessage = null;
      if (onVerified != null) {
        try {
          await onVerified();
        } catch (_) {
          // Animasyonun yarıda kalması girişi geri almaz.
        }
      }
      _startAuthenticatedSession();
    } on ApiException catch (error) {
      _errorMessage = error.message;
      rethrow;
    } finally {
      _setSubmitting(false);
    }
  }

  /// Google penceresini açar. Kişi kapatırsa null döner. Google yoksa
  /// (testler, yapılandırılmamış kurulum) [GoogleSignInUnavailable] fırlatır.
  Future<String?> obtainGoogleIdToken() {
    final google = _google;
    if (google == null) throw const GoogleSignInUnavailable();
    return google.obtainIdToken();
  }

  /// Belirteci sunucuya götürür. Hesap varsa (ya da bu çağrıda açıldıysa)
  /// oturum [onVerified]'den sonra açılır ve null döner; hesap yoksa
  /// "Bir adım kaldı"nın sorusu döner, hiçbir şey yazılmaz.
  Future<GoogleSignupRequired?> signInWithGoogle(
    String idToken, {
    String? username,
    bool hasBusiness = false,
    Future<void> Function()? onVerified,
  }) async {
    if (_isSubmitting) return null;
    _setSubmitting(true);
    try {
      final outcome = await _repository.googleSignIn(
        idToken,
        username: username,
        hasBusiness: hasBusiness,
      );
      _errorMessage = null;
      switch (outcome) {
        case GoogleSignupRequired():
          return outcome;
        case GoogleSignedIn():
          if (onVerified != null) {
            try {
              await onVerified();
            } catch (_) {
              // Animasyonun yarıda kalması girişi geri almaz.
            }
          }
          _startAuthenticatedSession();
          return null;
      }
    } on ApiException catch (error) {
      _errorMessage = error.message;
      rethrow;
    } finally {
      _setSubmitting(false);
    }
  }

  Future<void> register(
    String email,
    String password, {
    required bool hasBusiness,
    String? username,
  }) async {
    if (_isSubmitting) {
      return;
    }
    _setSubmitting(true);
    try {
      await _repository.register(
        email,
        password,
        hasBusiness: hasBusiness,
        username: username,
      );
      _errorMessage = null;
    } on ApiException catch (error) {
      _errorMessage = error.message;
      rethrow;
    } finally {
      _setSubmitting(false);
    }
  }

  Future<String?> refreshAfterUnauthorized() async {
    try {
      final refreshed = await _repository.refreshSession();
      if (refreshed == null) {
        _endAuthenticatedSession();
      }
      notifyListeners();
      return refreshed?.accessToken;
    } on ApiException {
      if (_repository.currentSession == null) {
        _endAuthenticatedSession();
        notifyListeners();
      }
      rethrow;
    }
  }

  Future<void> logout() async {
    _endAuthenticatedSession();
    notifyListeners();
    // Google oturumu da kapanır: aynı telefonda başka bir Google hesabıyla
    // girilebilsin. Hata çıkışı engellemez.
    unawaited(_google?.signOut());
    try {
      await _repository.logout();
    } on ApiException catch (error) {
      _errorMessage = error.message;
      notifyListeners();
    }
  }

  void _startAuthenticatedSession() {
    _status = AuthStatus.authenticated;
    _sessionGeneration++;
  }

  void _endAuthenticatedSession() {
    if (_status == AuthStatus.authenticated) {
      _sessionGeneration++;
    }
    _status = AuthStatus.unauthenticated;
  }

  void _setSubmitting(bool value) {
    _isSubmitting = value;
    notifyListeners();
  }
}
