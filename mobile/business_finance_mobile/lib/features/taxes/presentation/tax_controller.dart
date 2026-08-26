import 'package:flutter/foundation.dart';

import '../../../core/network/api_client.dart';
import '../../../core/network/api_exception.dart';
import '../data/tax_models.dart';
import '../data/tax_repository.dart';

/// Vergi takvimi ekranının durumu.
class TaxCalendarController extends ChangeNotifier {
  TaxCalendarController(this._repository);

  final TaxRepositoryContract _repository;

  List<TaxCalendarSuggestion> suggestions = const [];
  bool isLoading = false;
  bool unauthorized = false;
  String? errorMessage;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      suggestions = await _repository.loadSuggestions();
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
}

/// Ay sonu paketi ekranının durumu.
///
/// Toplamların hepsi sunucudan gelir; bu sınıf hiçbir tutarı hesaplamaz ve
/// hiçbirini toplamaz.
class AccountantPackageController extends ChangeNotifier {
  AccountantPackageController(this._repository, {DateTime? today})
    : _period = _previousMonth(today ?? DateTime.now());

  final TaxRepositoryContract _repository;
  DateTime _period;

  AccountantPackage? package;
  bool isLoading = false;
  bool isDownloading = false;
  bool unauthorized = false;
  String? errorMessage;

  int get year => _period.year;
  int get month => _period.month;

  /// Varsayılan dönem **geçen aydır**: paket ay kapandıktan sonra hazırlanır ve
  /// açılışta yarım bir ayı göstermek, eksik bir paketi tam sanmaya davettir.
  static DateTime _previousMonth(DateTime today) {
    final firstOfThisMonth = DateTime(today.year, today.month);
    return DateTime(firstOfThisMonth.year, firstOfThisMonth.month - 1);
  }

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      package = await _repository.loadPackage(year, month);
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

  Future<void> shiftMonth(int delta) async {
    _period = DateTime(_period.year, _period.month + delta);
    package = null;
    await load();
  }

  /// Paketi indirir; dosya çağırana verilir, paylaşmak onun işidir.
  Future<ApiBinaryResponse?> download() async {
    if (isDownloading) return null;
    isDownloading = true;
    errorMessage = null;
    notifyListeners();
    try {
      return await _repository.downloadPackage(year, month);
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return null;
    } finally {
      isDownloading = false;
      notifyListeners();
    }
  }
}
