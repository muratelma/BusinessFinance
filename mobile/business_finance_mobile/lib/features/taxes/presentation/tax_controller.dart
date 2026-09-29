import 'package:flutter/foundation.dart';

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
