import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../data/auth_models.dart';
import '../data/auth_repository.dart';

enum AuthStatus { restoring, authenticated, unauthenticated }

class AuthController extends ChangeNotifier {
  AuthController(this._repository);

  final AuthSessionRepository _repository;

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

  Future<void> login(String email, String password) async {
    if (_isSubmitting) {
      return;
    }
    _setSubmitting(true);
    try {
      await _repository.login(email, password);
      _errorMessage = null;
      _startAuthenticatedSession();
    } on ApiException catch (error) {
      _errorMessage = error.message;
      rethrow;
    } finally {
      _setSubmitting(false);
    }
  }

  Future<void> register(String email, String password) async {
    if (_isSubmitting) {
      return;
    }
    _setSubmitting(true);
    try {
      await _repository.register(email, password);
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
