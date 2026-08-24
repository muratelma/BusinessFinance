import 'package:flutter/foundation.dart';

import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/cash_repository.dart';

/// Gün sonu sayım ekranının durumu.
///
/// Fark **hiçbir yerde istemcide hesaplanmaz**: sunucu beklenen bakiyeyi ve
/// farkı birlikte gönderir, ekran onu gösterir. İstemci çıkarma yapsaydı iki
/// gerçek doğar ve yuvarlama farkı kullanıcının kasasında görünürdü.
class CashCountController extends ChangeNotifier {
  CashCountController(this._repository, {this.changes});

  final CashRepositoryContract _repository;
  final FinancialDataChanges? changes;

  List<CashAccount> accounts = const [];
  String? selectedAccountId;
  CashCountToday? today;
  List<CashCountItem> history = const [];

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  bool isStale = false;
  String? errorMessage;

  bool get hasCashAccount => accounts.isNotEmpty;

  /// Bugünün sayımı; henüz sayılmadıysa boş.
  CashCountItem? get todayCount => today?.count;

  /// Kaydedilecek bir fark var mı: sayım yapılmış, tutmamış ve düzeltmesi
  /// henüz yazılmamış.
  bool get hasOpenDifference {
    final count = todayCount;
    if (count == null || count.isAdjusted) return false;
    return _moneySign(count.difference) != 0;
  }

  /// Farkın hangi kategori türünü istediği: fazla nakit gelir, eksik nakit
  /// giderdir. Yönü kırpmak, kasadan eksileni fazla gibi göstermek olurdu.
  String get differenceCategoryType {
    return _moneySign(todayCount?.difference) > 0 ? 'income' : 'expense';
  }

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      if (accounts.isEmpty) {
        accounts = await _repository.loadCashAccounts();
      }
      selectedAccountId ??= accounts.isEmpty ? null : accounts.first.id;
      final accountId = selectedAccountId;
      if (accountId != null) {
        today = await _repository.loadToday(accountId: accountId);
        history = await _repository.list(accountId: accountId);
      }
      unauthorized = false;
      isStale = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      isStale = today != null;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      isStale = today != null;
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> selectAccount(String accountId) async {
    if (selectedAccountId == accountId) return;
    selectedAccountId = accountId;
    today = null;
    history = const [];
    await load();
  }

  Future<bool> recordCount({
    required String countedAmount,
    required String countDate,
    required TransactionScope? scope,
    String? note,
  }) async {
    final accountId = selectedAccountId;
    if (accountId == null || isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.create(
        accountId: accountId,
        countedAmount: countedAmount,
        countDate: countDate,
        scope: scope?.apiValue,
        note: note,
      );
      // Sayım hiçbir bakiyeyi değiştirmez; yalnız bu ekran yenilenir.
      changes?.cashCountRecorded();
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

  Future<List<DataChoice>> loadDifferenceCategories() =>
      _repository.loadCategories(type: differenceCategoryType);

  Future<bool> confirmDifference(String categoryId) async {
    final count = todayCount;
    if (count == null || isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.confirmDifference(
        cashCountId: count.id,
        categoryId: categoryId,
      );
      changes?.cashDifferenceConfirmed();
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

  /// Formların varsayılan günü.
  static String todayDate() {
    final value = DateTime.now();
    return '${value.year.toString().padLeft(4, '0')}-'
        '${value.month.toString().padLeft(2, '0')}-'
        '${value.day.toString().padLeft(2, '0')}';
  }
}

/// Para üzerinde hesap yapmaz; sunucunun gönderdiği kanonik ondalığın yalnız
/// yönünü okur. `double` kullanmak çok büyük/küçük değerlerde hassasiyet ve
/// taşma davranışını istemciye taşırdı.
int _moneySign(String? value) {
  final normalized = value?.trim();
  if (normalized == null || normalized.isEmpty) return 0;
  final digits = normalized.replaceAll(RegExp('[^0-9]'), '');
  if (digits.isEmpty || !digits.contains(RegExp('[1-9]'))) return 0;
  return normalized.startsWith('-') ? -1 : 1;
}
