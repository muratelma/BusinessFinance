import 'package:flutter/foundation.dart';

import '../network/api_exception.dart';

abstract class LoadableViewModel extends ChangeNotifier {
  bool _isLoading = false;
  bool _hasLoaded = false;
  ApiException? _error;

  bool get isLoading => _isLoading;
  bool get hasLoaded => _hasLoaded;
  ApiException? get error => _error;

  @protected
  Future<void> loadSafely(
    Future<void> Function() operation, {
    bool Function()? isCurrent,
  }) async {
    _isLoading = true;
    _error = null;
    notifyListeners();
    try {
      await operation();
      if (isCurrent?.call() ?? true) _hasLoaded = true;
    } on ApiException catch (error) {
      if (isCurrent?.call() ?? true) _error = error;
    } on FormatException {
      if (isCurrent?.call() ?? true) {
        _error = ApiException.local('response.invalid_format');
      }
    } finally {
      if (isCurrent?.call() ?? true) {
        _isLoading = false;
        notifyListeners();
      }
    }
  }

  @protected
  Future<T> mutate<T>(Future<T> Function() operation) async {
    _error = null;
    try {
      return await operation();
    } on ApiException catch (error) {
      _error = error;
      notifyListeners();
      rethrow;
    }
  }
}
