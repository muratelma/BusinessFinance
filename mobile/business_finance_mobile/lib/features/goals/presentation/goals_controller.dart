import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../data/goal_models.dart';
import '../data/goal_repository.dart';

class GoalsController extends ChangeNotifier {
  GoalsController(this._repository, {DateTime Function()? now})
    : _now = now ?? DateTime.now;

  final GoalRepositoryContract _repository;
  final DateTime Function() _now;

  GoalsSnapshot? snapshot;
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;

  String get today => _date(_now());
  bool get isStale => snapshot != null && errorMessage != null;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      snapshot = await _repository.load(today);
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir hedef yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> create(Map<String, Object?> input) => _submit(
    () => _repository.create({...input, 'asOfDate': today}),
    'Tasarruf hedefi oluşturuldu.',
  );

  Future<bool> delete(String goalId) =>
      _submit(() => _repository.delete(goalId), 'Tasarruf hedefi silindi.');

  Future<bool> contribute(String goalId, String amount) => _submit(
    () => _repository.contribute(goalId, {
      'amount': amount,
      'currency': 'TRY',
      'contributionDate': today,
      // Ağ tekrarında aynı katkının iki kez yazılmaması sunucudaki
      // idempotency anahtarına bağlı; anahtarı istemci üretir.
      'clientRequestId': _requestId(),
      'note': null,
      'asOfDate': today,
    }),
    'Katkı hedefe eklendi.',
  );

  void clearMessage() {
    errorMessage = null;
    successMessage = null;
    notifyListeners();
  }

  Future<bool> _submit(Future<void> Function() action, String success) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await action();
      await load();
      successMessage = success;
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
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

  static String _date(DateTime value) =>
      '${value.year.toString().padLeft(4, '0')}-'
      '${value.month.toString().padLeft(2, '0')}-'
      '${value.day.toString().padLeft(2, '0')}';

  String _requestId() {
    final time = _now().microsecondsSinceEpoch
        .toRadixString(16)
        .padLeft(12, '0');
    return '00000000-0000-4000-8000-${time.substring(time.length - 12)}';
  }
}
