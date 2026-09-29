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
  CashCountController(
    this._repository, {
    this.changes,
    DateTime Function()? clock,
  }) : _clock = clock ?? DateTime.now,
       _seenCashRevision = changes?.cashRevision ?? 0 {
    changes?.addListener(_handleFinancialDataChanged);
  }

  final CashRepositoryContract _repository;
  final FinancialDataChanges? changes;
  final DateTime Function() _clock;

  /// Başka bir ekranın yaptığı değişiklik (nakit gider, transfer, cari
  /// tahsilat…) Kasa'yı eskittiğinde yeniden yüklemek için. Kendi yazdığı
  /// değişiklikte kendini ikinci kez yüklemez.
  int _seenCashRevision;

  /// Dışarıdan gelen bir değişiklikten sonra kasa listesi ve diğer kasaların
  /// bakiyesi de yeniden okunur: yeni açılan ya da kapanan bir kasa, başka bir
  /// kasanın değişen bakiyesi önbellekte kalmasın.
  bool _reloadAccounts = false;

  /// Cihazın bugünü (`yyyy-MM-dd`); sayım bu güne yazılır.
  String get todayIso => _isoDate(_clock());

  List<CashAccount> accounts = const [];
  String? selectedAccountId;
  CashCountToday? today;
  List<CashCountItem> history = const [];

  /// Kasa seçicideki her kasanın uygulamaya göre bakiyesi. Seçili kasanınki
  /// her yüklemede tazelenir; diğerleri ilk yüklemede okunur.
  Map<String, String> expectedByAccount = const {};

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  bool isStale = false;
  String? errorMessage;

  bool get hasCashAccount => accounts.isNotEmpty;

  /// Bugünün sayımı; henüz sayılmadıysa boş.
  CashCountItem? get todayCount => today?.count;

  /// `Son sayımlar`: geçerli sayımlar, **bugünkü dahil**, yeniden eskiye
  /// (kullanıcı, 29 Eylül: bugünün sayımı listede de görünsün). Yerine yenisi
  /// yazılan sayım listede tekrar görünmez.
  List<CashCountItem> get recentCounts => [
    for (final item in history)
      if (!item.isCancelled) item,
  ];

  CashAccount? get selectedAccount {
    for (final account in accounts) {
      if (account.id == selectedAccountId) return account;
    }
    return null;
  }

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
      if (accounts.isEmpty || _reloadAccounts) {
        accounts = await _repository.loadCashAccounts();
        _reloadAccounts = false;
        if (!accounts.any((account) => account.id == selectedAccountId)) {
          selectedAccountId = null;
        }
      }
      selectedAccountId ??= accounts.isEmpty ? null : accounts.first.id;
      final accountId = selectedAccountId;
      if (accountId != null) {
        today = await _repository.loadToday(accountId: accountId);
        history = await _repository.list(accountId: accountId);
        await _loadOtherBalances(accountId);
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

  /// Seçicinin ikinci satırı: diğer kasaların bakiyesi. İkincil bir okumadır;
  /// düşerse seçici yalnız adları gösterir, ekran ayakta kalır.
  Future<void> _loadOtherBalances(String selectedId) async {
    final balances = {...expectedByAccount};
    final current = today;
    if (current != null) balances[selectedId] = current.expectedBalance;
    if (accounts.length > 1) {
      for (final account in accounts) {
        if (account.id == selectedId || balances.containsKey(account.id)) {
          continue;
        }
        try {
          final other = await _repository.loadToday(accountId: account.id);
          balances[account.id] = other.expectedBalance;
        } on ApiException {
          // Sessiz: seçici bakiyesiz kalır.
        } on FormatException {
          // Sessiz: seçici bakiyesiz kalır.
        }
      }
    }
    expectedByAccount = balances;
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
      _announce((c) => c.cashCountRecorded());
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
      _announce((c) => c.cashDifferenceConfirmed());
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

  /// Kendi değişikliğini duyururken dinleyicinin bu ekranı ikinci kez
  /// yüklemesini önler; ekran zaten ardından kendini yüklüyor.
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
    _reloadAccounts = true;
    expectedByAccount = const {};
    load();
  }

  @override
  void dispose() {
    changes?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }
}

String _isoDate(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';

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
