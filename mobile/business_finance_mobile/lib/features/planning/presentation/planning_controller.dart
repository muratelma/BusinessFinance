import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/planning_models.dart';
import '../data/planning_repository.dart';

class PlanningController extends ChangeNotifier {
  PlanningController(
    this._repository, {
    this.financialDataChanges,
    DateTime Function()? now,
  }) : _now = now ?? DateTime.now {
    final today = _dateOnly(_now());
    asOfDate = today;
    reportYear = today.year;
    reportMonth = today.month;
  }

  final PlanningRepositoryContract _repository;
  final FinancialDataChanges? financialDataChanges;
  final DateTime Function() _now;

  PlanningSnapshot? snapshot;
  late DateTime asOfDate;
  late int reportYear;
  late int reportMonth;
  int daysAhead = 30;
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;

  bool get isStale => snapshot != null && errorMessage != null;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      snapshot = await _repository.load(
        year: reportYear,
        month: reportMonth,
        asOfDate: _formatDate(asOfDate),
        daysAhead: daysAhead,
      );
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir planlama yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> changeDaysAhead(int value) async {
    if (daysAhead == value) return;
    daysAhead = value;
    await load();
  }

  Future<void> changeAsOfDate(DateTime value) async {
    asOfDate = _dateOnly(value);
    await load();
  }

  Future<void> moveReportMonth(int offset) async {
    final moved = DateTime(reportYear, reportMonth + offset);
    reportYear = moved.year;
    reportMonth = moved.month;
    await load();
  }

  Future<bool> createRecurring(Map<String, Object?> input) =>
      _submit(() => _repository.createRecurring(input), 'Plan oluşturuldu.');

  Future<bool> setRecurringActive(String id, bool isActive) => _submit(
    () => _repository.setRecurringActive(id, isActive),
    isActive ? 'Plan etkinleştirildi.' : 'Plan duraklatıldı.',
  );

  Future<bool> deleteRecurring(String id) =>
      _submit(() => _repository.deleteRecurring(id), 'Plan silindi.');

  /// Son gerçekleştirme isteği, planın daha önceki bir kalemi beklediği için
  /// onay istedi. Hata değildir ve hata olarak gösterilmez: ekran sorar ve
  /// onayla aynı isteği yineler.
  bool earlierItemPending = false;

  /// Bekleyen en eski kalemin vadesi ve bekleyen kalem sayısı; sunucu
  /// göndermediyse boştur.
  String? earlierPendingDate;
  int? earlierPendingCount;

  Future<bool> realizeOccurrence(
    String id, {
    String? paidOn,
    bool allowEarlierPending = false,
  }) => _realize(
    () => _repository.realizeOccurrence(
      id,
      paidOn: paidOn,
      allowEarlierPending: allowEarlierPending,
    ),
  );

  Future<bool> _realize(Future<void> Function() action) async {
    earlierItemPending = false;
    final realized = await _submit(() async {
      try {
        await action();
      } on ApiException catch (error) {
        earlierItemPending = error.isEarlierItemPending;
        earlierPendingDate = error.pendingScheduledDate;
        earlierPendingCount = error.pendingCount;
        rethrow;
      }
    }, 'Planlanan kayıt gerçekleşen işleme dönüştürüldü.');
    if (earlierItemPending) {
      errorMessage = null;
      notifyListeners();
    }
    if (realized) financialDataChanges?.recurringRealized();
    return realized;
  }

  /// Henüz üretilmemiş bir kaydı planı ve tarihiyle gerçekleştirir.
  ///
  /// Yaklaşanlar listesi kimlik olarak, occurrence varsa onun kimliğini, yoksa
  /// **planın** kimliğini taşıyor. Ekran ikisini ayırt edip doğru uç noktaı
  /// seçiyor; kullanıcı için ikisi de tek bir "Onayla".
  Future<bool> realizeDue(
    String planId,
    String scheduledDate, {
    String? paidOn,
    bool allowEarlierPending = false,
  }) => _realize(
    () => _repository.realizeDue(
      planId,
      scheduledDate,
      paidOn: paidOn,
      allowEarlierPending: allowEarlierPending,
    ),
  );

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
      errorMessage = 'Sunucudan beklenmeyen bir planlama yanıtı alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  void clearMessage() {
    errorMessage = null;
    successMessage = null;
    notifyListeners();
  }

  static DateTime _dateOnly(DateTime value) =>
      DateTime(value.year, value.month, value.day);

  static String _formatDate(DateTime value) =>
      '${value.year.toString().padLeft(4, '0')}-'
      '${value.month.toString().padLeft(2, '0')}-'
      '${value.day.toString().padLeft(2, '0')}';
}
