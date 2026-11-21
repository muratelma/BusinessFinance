import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../data/activity_repository.dart';
import '../data/planned_activity_models.dart';

class PlannedActivityController extends ChangeNotifier {
  PlannedActivityController(
    this._repository, {
    this.financialDataChanges,
    this.scopeController,
    this.asOf,
  }) : horizon = asOf == null ? PlannedHorizon.month : PlannedHorizon.week,
       _seenRevision = financialDataChanges?.planningRevision ?? 0,
       _seenScope = scopeController?.scope {
    financialDataChanges?.addListener(_handleFinancialDataChanged);
    scopeController?.addListener(_handleScopeChanged);
  }

  final ActivityRepositoryContract _repository;
  final DateTime? asOf;
  final FinancialDataChanges? financialDataChanges;
  final ScopeController? scopeController;
  int _seenRevision;
  TransactionScope? _seenScope;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  PlannedActivityPage? page;
  PlannedHorizon horizon;
  PlannedTypeFilter typeFilter = PlannedTypeFilter.all;

  /// Filtered in memory: the horizon already bounds the result, and the server
  /// contract has no type parameter, so asking again would only add a round trip.
  List<PlannedActivity> get visibleItems => [
    ...?page?.items.where(typeFilter.matches),
  ];

  bool get isEmpty =>
      !isLoading && errorMessage == null && visibleItems.isEmpty;

  /// Görünümün o an okuduğu kapsam; başlıkta da bu yazılı durur.
  ///
  /// Kapsam doluyken kart ekstresi gibi kapsamsız satırlar listeden düşer:
  /// ikisini birden iki tarafta göstermek aynı ödemeyi iki kez saydırırdı.
  TransactionScope? get scope => scopeController?.scope;

  bool get isScopeVisible => scopeController?.isVisible ?? false;

  Future<void> load() async {
    final request = ++_loadGeneration;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      final loaded = await _repository.listPlanned(
        horizon: horizon,
        scope: scope,
        today: asOf,
      );
      if (request != _loadGeneration) return;
      page = loaded;
      unauthorized = false;
    } on ApiException catch (error) {
      if (request != _loadGeneration) return;
      page = null;
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      if (request != _loadGeneration) return;
      page = null;
      errorMessage = 'Sunucudan beklenmeyen bir planlama yanıtı alındı.';
    } finally {
      if (request == _loadGeneration) {
        isLoading = false;
        notifyListeners();
      }
    }
  }

  /// Planlanan satırı gerçek harekete çevirir.
  ///
  /// Başarıda `true` döner. Liste yeniden yüklenmez: gerçekleşen kayıt artık
  /// planlanan değildir ve [FinancialDataChanges] üzerinden gelen sinyal
  /// zaten yeniden yüklemeyi tetikler — burada da yüklemek aynı sayfayı iki
  /// kez çekerdi.
  /// Son gerçekleştirme isteği, planın daha önceki bir kalemi beklediği için
  /// onay istedi. Hata değildir: ekran sorar ve onayla aynı isteği yineler.
  bool earlierItemPending = false;

  /// Bekleyen en eski kalemin vadesi ve bekleyen kalem sayısı; sunucu
  /// göndermediyse boştur.
  String? earlierPendingDate;
  int? earlierPendingCount;

  Future<bool> realize(
    PlannedActivity activity, {
    String? paidOn,
    bool allowEarlierPending = false,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    earlierItemPending = false;
    notifyListeners();
    try {
      await _repository.realizePlanned(
        activity,
        paidOn: paidOn,
        allowEarlierPending: allowEarlierPending,
      );
      switch (activity.plannedKind) {
        case PlannedKind.cardInstallment:
          financialDataChanges?.installmentRealized();
        case PlannedKind.recurringOccurrence:
          financialDataChanges?.recurringRealized();
        case PlannedKind.cardStatement:
        case PlannedKind.debtInstallment:
        case PlannedKind.receivableInstallment:
        case PlannedKind.payableObligation:
        case PlannedKind.receivableObligation:
        case PlannedKind.counterpartyPayable:
        case PlannedKind.counterpartyReceivable:
          break;
      }
      // Sinyal dinlenmiyorsa (test ya da bağlanmamış kabuk) liste elle
      // tazelenir; iki yol da tek bir yeniden yükleme yapar.
      if (financialDataChanges == null) await load();
      return true;
    } on ApiException catch (error) {
      if (error.isEarlierItemPending) {
        earlierItemPending = true;
        earlierPendingDate = error.pendingScheduledDate;
        earlierPendingCount = error.pendingCount;
        return false;
      }
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

  void _handleScopeChanged() {
    final current = scopeController?.scope;
    if (current == _seenScope) return;
    _seenScope = current;
    page = null;
    load();
  }

  void _handleFinancialDataChanged() {
    final changes = financialDataChanges;
    if (changes == null) return;
    final revision = changes.planningRevision;
    if (revision == _seenRevision) return;
    _seenRevision = revision;
    // Realizing a plan turns it into a movement, so the planned list must drop
    // it rather than keep offering the action.
    load();
  }

  @override
  void dispose() {
    _loadGeneration++;
    financialDataChanges?.removeListener(_handleFinancialDataChanged);
    scopeController?.removeListener(_handleScopeChanged);
    super.dispose();
  }

  int _loadGeneration = 0;
}
