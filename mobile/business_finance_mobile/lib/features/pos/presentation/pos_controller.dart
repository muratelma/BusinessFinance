import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/pos_repository.dart';

class PosController extends ChangeNotifier {
  PosController(this._repository, {this.changes});

  final PosRepositoryContract _repository;
  final FinancialDataChanges? changes;

  PosSettlementList? settlements;
  PosOptions? options;

  /// Yalnız yolda olanları göster.
  bool inTransitOnly = false;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  bool isStale = false;
  String? errorMessage;

  List<PosSettlementItem> get items => settlements?.items ?? const [];

  String get moneyInTransit => settlements?.moneyInTransit ?? '0.0000';

  int get inTransitCount => settlements?.inTransitCount ?? 0;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      settlements = await _repository.list(inTransitOnly: inTransitOnly);
      unauthorized = false;
      isStale = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      isStale = settlements != null;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      isStale = settlements != null;
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> setInTransitOnly(bool value) async {
    if (inTransitOnly == value) return;
    inTransitOnly = value;
    await load();
  }

  Future<PosOptions> loadOptions() async {
    return options ??= await _repository.loadOptions();
  }

  /// Komisyon **ya tutar ya oran** olarak gider; ikisi birden gönderilirse
  /// sunucu isteği reddeder ve haklıdır: iki gerçek arasında seçim yapmak
  /// onun işi değildir.
  Future<bool> create({
    required String accountId,
    required String categoryId,
    required String grossAmount,
    required String settlementDate,
    required String expectedTransferDate,
    required TransactionScope? scope,
    String? commissionAmount,
    String? commissionRate,
    String? commissionCategoryId,
    String? description,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.create({
        'accountId': accountId,
        'categoryId': categoryId,
        'grossAmount': grossAmount,
        'currency': 'TRY',
        'settlementDate': settlementDate,
        'expectedTransferDate': expectedTransferDate,
        'commissionAmount': commissionAmount,
        'commissionRate': commissionRate,
        'commissionCategoryId': commissionCategoryId,
        'scope': scope?.apiValue,
        'description': description,
      });
      changes?.posSettlementRecognized();
      await load();
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

  Future<bool> markTransferred(
    PosSettlementItem item,
    String transferDate,
  ) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.markTransferred(
        settlementId: item.id,
        transferDate: transferDate,
      );
      changes?.posSettlementTransferred();
      await load();
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
