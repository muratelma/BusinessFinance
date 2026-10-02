import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/pos_repository.dart';

/// Tek bir POS yatışının ayrıntısı ve geri alınması (ADR 0019 T5).
///
/// Kasa'daki tahsilat panelinden de İşlemler'deki yatış satırından da açılır;
/// bu yüzden tahsilat listesinin controller'ına bağlı değildir. Geri alma
/// sonucunu [FinancialDataChanges] ile duyurur, listeler kendini oradan
/// yeniler.
class PosDepositController extends ChangeNotifier {
  PosDepositController(this._repository, this.depositId, {this.changes});

  final PosRepositoryContract _repository;
  final String depositId;
  final FinancialDataChanges? changes;

  PosDeposit? deposit;
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      deposit = await _repository.getDeposit(depositId: depositId);
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

  /// Yatışı geri alır: tahsilatlar yola döner, kesinti gideri iptal olur.
  /// Kayıt silinmez; geri alınmış olarak kalır.
  Future<bool> revert() async {
    final current = deposit;
    if (isSubmitting || current == null || current.isCancelled) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      deposit = await _repository.revertDeposit(depositId: depositId);
      changes?.posDepositChanged(deduction: current.hasDeduction);
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
}
