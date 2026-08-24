import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/obligation_repository.dart';

class ObligationController extends ChangeNotifier {
  ObligationController(this._repository, {this.changes});

  final ObligationRepositoryContract _repository;
  final FinancialDataChanges? changes;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  ObligationOptions? options;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      options = await _repository.loadPayableOptions();
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

  Future<bool> create({
    required String amount,
    required String categoryId,
    required String issueDate,
    required String dueDate,
    required TransactionScope? scope,
    String? counterpartyId,
    String? description,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.create({
        'direction': 'payable',
        'amount': amount,
        'currency': 'TRY',
        'categoryId': categoryId,
        'issueDate': issueDate,
        'dueDate': dueDate,
        'scope': scope?.apiValue,
        'counterpartyId': counterpartyId,
        'description': description,
      });
      changes?.obligationRecognized();
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
