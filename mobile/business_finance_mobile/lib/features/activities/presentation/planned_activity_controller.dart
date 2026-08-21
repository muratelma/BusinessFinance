import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/activity_repository.dart';
import '../data/planned_activity_models.dart';

class PlannedActivityController extends ChangeNotifier {
  PlannedActivityController(this._repository, {this.financialDataChanges})
    : _seenRevision = financialDataChanges?.planningRevision ?? 0 {
    financialDataChanges?.addListener(_handleFinancialDataChanged);
  }

  final ActivityRepositoryContract _repository;
  final FinancialDataChanges? financialDataChanges;
  int _seenRevision;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  PlannedActivityPage? page;
  PlannedHorizon horizon = PlannedHorizon.month;
  PlannedTypeFilter typeFilter = PlannedTypeFilter.all;

  /// Filtered in memory: the horizon already bounds the result, and the server
  /// contract has no type parameter, so asking again would only add a round trip.
  List<PlannedActivity> get visibleItems => [
    ...?page?.items.where(typeFilter.matches),
  ];

  bool get isEmpty =>
      !isLoading && errorMessage == null && visibleItems.isEmpty;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      page = await _repository.listPlanned(horizon: horizon);
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

  /// Planlanan satırı gerçek harekete çevirir.
  ///
  /// Başarıda `true` döner. Liste yeniden yüklenmez: gerçekleşen kayıt artık
  /// planlanan değildir ve [FinancialDataChanges] üzerinden gelen sinyal
  /// zaten yeniden yüklemeyi tetikler — burada da yüklemek aynı sayfayı iki
  /// kez çekerdi.
  Future<bool> realize(PlannedActivity activity) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.realizePlanned(activity);
      switch (activity.plannedKind) {
        case PlannedKind.cardInstallment:
          financialDataChanges?.installmentRealized();
        case PlannedKind.recurringOccurrence:
          financialDataChanges?.recurringRealized();
        case PlannedKind.cardStatement:
        case PlannedKind.debtInstallment:
        case PlannedKind.receivableInstallment:
          break;
      }
      // Sinyal dinlenmiyorsa (test ya da bağlanmamış kabuk) liste elle
      // tazelenir; iki yol da tek bir yeniden yükleme yapar.
      if (financialDataChanges == null) await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<void> selectHorizon(PlannedHorizon value) async {
    if (horizon == value) return;
    horizon = value;
    await load();
  }

  void selectTypeFilter(PlannedTypeFilter value) {
    typeFilter = value;
    notifyListeners();
  }

  void _handleFinancialDataChanged() {
    final changes = financialDataChanges;
    if (changes == null) return;
    final revision = changes.planningRevision;
    if (revision == _seenRevision) return;
    _seenRevision = revision;
    // Realizing a plan turns it into a movement, so the planned list must drop
    // it rather than keep offering the action.
    if (!isLoading) load();
  }

  @override
  void dispose() {
    financialDataChanges?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }
}
