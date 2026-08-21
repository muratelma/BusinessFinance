import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/debt_models.dart';
import '../data/debt_repository.dart';

class DebtsController extends ChangeNotifier {
  DebtsController(
    this._repository, {
    DateTime Function()? now,
    this.financialDataChanges,
  }) : _now = now ?? DateTime.now;

  final DebtRepositoryContract _repository;

  /// Borç açılışı para hareket ettirir ya da gider/gelir yazar; taksit ödemesi
  /// bakiyeyi değiştirir. Her ikisi de arkadaki ekranları bayatlatır.
  final FinancialDataChanges? financialDataChanges;
  final DateTime Function() _now;

  DebtsSnapshot? snapshot;
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;

  String get today => _date(_now());

  /// Sunucu erişilemezken elde kalan son okuma.
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
      errorMessage = 'Sunucudan beklenmeyen bir borç yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> create(Map<String, Object?> input) => _submit(
    () => _repository.create({...input, 'asOfDate': today}),
    'Borç planı oluşturuldu.',
  );

  Future<bool> recordOpening(String debtId, Map<String, Object?> input) =>
      _submit(
        () => _repository.recordOpening(debtId, {...input, 'asOfDate': today}),
        'Borcun açılışı kaydedildi.',
      );

  /// Taksit ödemesi ya da alacak tahsilatı.
  ///
  /// Mesaj yöne göre değişir: alacak tahsil edildiğinde "Taksit ödendi"
  /// demek, kullanıcıya para verdiğini söylemektir — oysa para almıştır.
  Future<bool> pay(
    String debtId,
    int sequence,
    String accountId, {
    required bool isReceivable,
  }) => _submit(
    () => _repository.pay(debtId, sequence, {
      'accountId': accountId,
      'paymentDate': today,
      'asOfDate': today,
    }),
    isReceivable ? 'Taksit tahsil edildi.' : 'Taksit ödendi.',
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
      financialDataChanges?.debtChanged();
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
}
