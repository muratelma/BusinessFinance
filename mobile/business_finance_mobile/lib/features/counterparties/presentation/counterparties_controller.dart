import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/counterparty_models.dart';
import '../data/counterparty_repository.dart';

/// Cari listesi ve bir karşı tarafın ayrıntısı.
///
/// Tek controller iki ekranı besliyor çünkü yazma yolları ortak: ayrıntıdan
/// alınan bir tahsilat listedeki bakiyeyi de değiştirir ve iki ayrı controller
/// aynı yazımdan sonra birbirini tazelemek zorunda kalırdı.
class CounterpartiesController extends ChangeNotifier {
  CounterpartiesController(
    this._repository, {
    DateTime Function()? now,
    this.financialDataChanges,
  }) : _now = now ?? DateTime.now;

  final CounterpartyRepositoryContract _repository;

  /// Borçlandırma gelir/gider tanır, tahsilat kasayı değiştirir; ikisi de
  /// arkadaki ekranları bayatlatır.
  final FinancialDataChanges? financialDataChanges;
  final DateTime Function() _now;

  CounterpartiesSnapshot? snapshot;
  CounterpartyDetail? detail;
  CounterpartyBalanceFilter filter = CounterpartyBalanceFilter.all;
  bool isLoading = false;
  bool isDetailLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;

  String get today => _date(_now());

  /// Sunucu erişilemezken elde kalan son okuma.
  bool get isStale => snapshot != null && errorMessage != null;

  List<CounterpartySummary> get counterparties =>
      snapshot?.counterparties ?? const [];

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      snapshot = await _repository.load(filter);
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir cari yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> changeFilter(CounterpartyBalanceFilter value) async {
    if (filter == value) return;
    filter = value;
    notifyListeners();
    await load();
  }

  /// Ayrıntı okuması listeyi bozmaz: liste elde kalır ve kullanıcı geri
  /// döndüğünde boş bir ekranla karşılaşmaz.
  Future<void> loadDetail(String counterpartyId) async {
    if (isDetailLoading) return;
    isDetailLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      detail = await _repository.loadDetail(counterpartyId);
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir cari yanıtı alındı.';
    } finally {
      isDetailLoading = false;
      notifyListeners();
    }
  }

  Future<bool> create(String name, String? note) => _submit(
    () => _repository.create(name, note),
    'Karşı taraf eklendi.',
    peopleOnly: true,
  );

  Future<bool> update(
    String counterpartyId, {
    required String name,
    required bool isActive,
    String? note,
  }) => _submit(
    () => _repository.update(
      counterpartyId,
      name: name,
      isActive: isActive,
      note: note,
    ),
    isActive ? 'Karşı taraf güncellendi.' : 'Karşı taraf pasife alındı.',
    peopleOnly: true,
    detailId: counterpartyId,
  );

  Future<bool> delete(String counterpartyId) => _submit(
    () => _repository.delete(counterpartyId),
    'Karşı taraf silindi.',
    peopleOnly: true,
  );

  /// Veresiye satış ya da vadeli alım: gelir/gider bugün yazılır, kasa
  /// kıpırdamaz.
  Future<bool> addCharge(
    String counterpartyId, {
    required bool isReceivable,
    required String amount,
    required String categoryId,
    required String chargeDate,
    String? scope,
    String? description,
  }) => _submit(
    () => _repository.addCharge(counterpartyId, {
      'direction': isReceivable ? 'receivable' : 'payable',
      'amount': amount,
      'currency': 'TRY',
      'categoryId': categoryId,
      'chargeDate': chargeDate,
      'scope': ?scope,
      if (description != null && description.isNotEmpty)
        'description': description,
    }),
    isReceivable ? 'Veresiye satış yazıldı.' : 'Vadeli alım yazıldı.',
    detailId: counterpartyId,
  );

  /// Tahsilat ya da ödeme: kasa değişir, gelir/gider üretilmez.
  ///
  /// Mesaj yöne göre değişir — alacak tahsil edilirken "ödeme yapıldı" demek,
  /// kullanıcıya para verdiğini söylemek olurdu.
  Future<bool> addPayment(
    String counterpartyId, {
    required bool isReceivable,
    required String amount,
    required String accountId,
    required String paymentDate,
    String? description,
  }) => _submit(
    () => _repository.addPayment(counterpartyId, {
      'direction': isReceivable ? 'receivable' : 'payable',
      'amount': amount,
      'currency': 'TRY',
      'accountId': accountId,
      'paymentDate': paymentDate,
      if (description != null && description.isNotEmpty)
        'description': description,
    }),
    isReceivable ? 'Tahsilat kaydedildi.' : 'Ödeme kaydedildi.',
    detailId: counterpartyId,
  );

  void clearMessage() {
    errorMessage = null;
    successMessage = null;
    notifyListeners();
  }

  /// [peopleOnly] yalnız kişi listesini değiştiren yazımlar için: ad değişti,
  /// kayıt pasifleşti. Para hareket etmediği için gider/kasa sinyali
  /// yükseltilmez — yükseltilseydi bütçe ekranı hiç değişmemiş bir sayı için
  /// yeniden yüklenirdi.
  Future<bool> _submit(
    Future<void> Function() action,
    String success, {
    bool peopleOnly = false,
    String? detailId,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await action();
      if (peopleOnly) {
        financialDataChanges?.counterpartiesChanged();
      } else {
        financialDataChanges?.counterpartyLedgerChanged();
      }
      await load();
      if (detailId != null && detail?.counterparty.id == detailId) {
        await loadDetail(detailId);
      }
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
