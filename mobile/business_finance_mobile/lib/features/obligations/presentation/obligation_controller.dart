import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/obligation_direction.dart';
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

  /// Kategori listesi yöne bağlıdır: alacak gelir, borç gider kategorisi
  /// ister. Yön değişince liste yeniden okunur; tek bir listeyi iki yöne
  /// vermek, sunucunun reddedeceği bir seçimi kullanıcıya sunmak olurdu.
  Future<void> load({
    ObligationDirection direction = ObligationDirection.payable,
  }) async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      options = await _repository.loadOptions(
        categoryType: direction.categoryType,
      );
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
    required ObligationDirection direction,
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
        'direction': direction.apiValue,
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

class ObligationListController extends ChangeNotifier {
  ObligationListController(this._repository, {this.changes});

  final ObligationRepositoryContract _repository;
  final FinancialDataChanges? changes;

  List<ObligationItem> items = const [];
  bool isLoading = false;
  bool isStale = false;
  bool unauthorized = false;
  String? errorMessage;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      items = await _repository.list(asOfDate: _today());
      unauthorized = false;
      isStale = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      isStale = items.isNotEmpty;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      isStale = items.isNotEmpty;
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<List<ObligationAccount>> loadAccounts() =>
      _repository.loadActiveAccounts();

  Future<bool> settle(ObligationItem item, String accountId) async {
    try {
      await _repository.settle(
        obligationId: item.id,
        accountId: accountId,
        settlementDate: _today(),
      );
      changes?.obligationSettled();
      await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
    }
    notifyListeners();
    return false;
  }

  static String _today() {
    final value = DateTime.now();
    return '${value.year.toString().padLeft(4, '0')}-'
        '${value.month.toString().padLeft(2, '0')}-'
        '${value.day.toString().padLeft(2, '0')}';
  }
}
