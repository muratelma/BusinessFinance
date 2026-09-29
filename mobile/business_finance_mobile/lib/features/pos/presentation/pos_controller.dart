import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/pos_repository.dart';

class PosController extends ChangeNotifier {
  PosController(this._repository, {this.changes})
    : _seenCashRevision = changes?.cashRevision ?? 0 {
    changes?.addListener(_handleFinancialDataChanged);
  }

  final PosRepositoryContract _repository;
  final FinancialDataChanges? changes;

  /// Kasa ekranındaki POS listesi de `cash` hedefini izler; kendi yazdığı
  /// değişiklikte kendini ikinci kez yüklemez.
  int _seenCashRevision;

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
      _announce((c) => c.posSettlementRecognized());
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
      _announce((c) => c.posSettlementTransferred());
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

  /// Yanlışlıkla "hesaba geçti" denmiş tahsilatı yeniden yola döndürür.
  Future<bool> revertTransfer(PosSettlementItem item) => _mutate(
    () => _repository.revertTransfer(settlementId: item.id),
    (c) => c.posSettlementTransferReverted(),
  );

  /// Silme yerine iptal.
  Future<bool> cancel(PosSettlementItem item) => _mutate(
    () => _repository.cancel(settlementId: item.id),
    (c) => c.posSettlementCancelled(),
  );

  Future<bool> _mutate(
    Future<void> Function() request,
    void Function(FinancialDataChanges changes) raise,
  ) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await request();
      _announce(raise);
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

  void _announce(void Function(FinancialDataChanges changes) raise) {
    final current = changes;
    if (current == null) return;
    _seenCashRevision = current.cashRevision + 1;
    raise(current);
  }

  void _handleFinancialDataChanged() {
    final revision = changes?.cashRevision ?? 0;
    if (revision == _seenCashRevision) return;
    _seenCashRevision = revision;
    load();
  }

  @override
  void dispose() {
    changes?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }
}
