import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/day_close_repository.dart';

/// Gün sonu panelinin ve gün sonu ayrıntısının durumu (ADR 0019 T1–T2).
///
/// "+" menüsünden de Kasa'dan da açılır; bu yüzden hiçbir listeye bağlı
/// değildir. Yazma ve geri alma sonucunu [FinancialDataChanges] ile duyurur,
/// ekranlar kendini oradan yeniler.
class DayCloseController extends ChangeNotifier {
  DayCloseController(this._repository, {this.changes})
    : _seenCashRevision = changes?.cashRevision ?? 0 {
    changes?.addListener(_handleFinancialDataChanged);
  }

  final DayCloseRepositoryContract _repository;
  final FinancialDataChanges? changes;

  /// Kasa'nın en son gördüğü değişiklik; başka bir ekrandan gelen değişiklik
  /// (İşlemler'den geri alma) "bugün" bilgisini yeniden okutur.
  int _seenCashRevision;

  /// Bugünün bütünü (gün sonları ve kayıtları); henüz okunmadıysa `null`.
  DayCloseDay? today;
  String? _todayDate;

  /// Kasa'daki "bugün" kartı için günü okur. Hata sessizdir: kart son bilinen
  /// hâliyle kalır, Kasa'nın geri kalanı çalışır.
  Future<void> loadDay(String date) async {
    _todayDate = date;
    try {
      today = await _repository.day(date: date);
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = unauthorized || error.isUnauthorized;
    } on FormatException {
      // Kart eski hâliyle kalır.
    }
    notifyListeners();
  }

  /// Gün ayrıntısı için bir günü okur; okunamazsa `null` döner ve
  /// [errorMessage] dolar.
  Future<DayCloseDay?> readDay(String date) async {
    try {
      final day = await _repository.day(date: date);
      errorMessage = null;
      errorCode = null;
      return day;
    } on ApiException catch (error) {
      unauthorized = unauthorized || error.isUnauthorized;
      errorMessage = error.message;
      return null;
    } on FormatException {
      errorMessage = _unexpected;
      return null;
    }
  }

  void _handleFinancialDataChanged() {
    final revision = changes?.cashRevision ?? 0;
    if (revision == _seenCashRevision) return;
    _seenCashRevision = revision;
    final date = _todayDate;
    if (date != null) loadDay(date);
  }

  @override
  void dispose() {
    changes?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }

  bool isSubmitting = false;
  bool unauthorized = false;

  /// Kayıt ya da geri alma reddedildiğinde sunucunun söylediği.
  String? errorMessage;

  /// Reddedilen kaydın hata kodu; panel onu ilgili alanın yanında söyler.
  String? errorCode;

  /// Önizleme alınamadıysa nedeni.
  String? previewError;

  static const _unexpected = 'Sunucudan beklenmeyen bir yanıt alındı.';

  Future<DayCloseOptions> loadOptions() => _repository.loadOptions();

  /// Önizlemeyi okur; alınamazsa `null` döner ve [previewError] dolar.
  Future<DayClosePreview?> preview(DayCloseInput input) async {
    try {
      final preview = await _repository.preview(input);
      previewError = null;
      return preview;
    } on ApiException catch (error) {
      unauthorized = unauthorized || error.isUnauthorized;
      previewError = error.message;
      return null;
    } on FormatException {
      previewError = _unexpected;
      return null;
    }
  }

  /// Gün sonunu yazar. Hata [errorMessage] ve [errorCode] alanlarına yazılır
  /// ve panel açık kalır.
  Future<DayClose?> create({
    required String clientRequestId,
    required DayCloseInput input,
  }) => _write(
    () => _repository.create(clientRequestId: clientRequestId, input: input),
  );

  Future<DayClose?> get(String dayCloseId) async {
    try {
      return await _repository.get(dayCloseId: dayCloseId);
    } on ApiException catch (error) {
      unauthorized = unauthorized || error.isUnauthorized;
      errorMessage = error.message;
      return null;
    } on FormatException {
      errorMessage = _unexpected;
      return null;
    }
  }

  /// Gün sonunu bir bütün olarak geri alır: ürettiği kayıtlar iptal olur ve
  /// gün yeniden açılır.
  Future<DayClose?> revert(String dayCloseId) =>
      _write(() => _repository.revert(dayCloseId: dayCloseId));

  Future<DayClose?> _write(Future<DayClose> Function() action) async {
    if (isSubmitting) return null;
    isSubmitting = true;
    errorMessage = null;
    errorCode = null;
    notifyListeners();
    try {
      final dayClose = await action();
      changes?.dayCloseChanged();
      return dayClose;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      errorCode = error.code;
      return null;
    } on FormatException {
      errorMessage = _unexpected;
      return null;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }
}
