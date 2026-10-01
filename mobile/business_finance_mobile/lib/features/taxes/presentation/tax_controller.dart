import 'dart:math';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/loadable_view_model.dart';
import '../../activities/data/planned_activity_models.dart';
import '../data/tax_models.dart';
import '../data/tax_repository.dart';
import 'tax_schedule.dart';

/// Vergi takibi ekranının durumu ve eylemleri (ADR 0018).
///
/// Ekran üç şeyi okur — tanımlı vergiler, bekleyenler, son ödenenler — ve
/// hepsi tek uçtan gelir. Toplamlar sunucudandır; istemci bekleyenleri
/// toplamaz. Her yazma işi `FinancialDataChanges`'e yalnız etkileyebileceği
/// hedefleri bildirir; ekran da gider ve plan değişikliklerini dinler, çünkü
/// gider formundan vergi kategorisiyle girilen ödeme de burada görünür.
class TaxController extends LoadableViewModel {
  TaxController(
    this._repository, {
    FinancialDataChanges? changes,
    DateTime Function()? now,
  }) : _changes = changes,
       _now = now ?? DateTime.now {
    _seenFeed = changes?.activityFeedRevision ?? 0;
    _seenPlanning = changes?.planningRevision ?? 0;
    _changes?.addListener(_handleChanges);
  }

  final TaxRepositoryContract _repository;
  final FinancialDataChanges? _changes;
  final DateTime Function() _now;
  int _seenFeed = 0;
  int _seenPlanning = 0;

  TaxOverview? overview;
  TaxOptions? options;
  List<TaxSuggestion>? suggestions;

  bool _isSubmitting = false;
  bool get isSubmitting => _isSubmitting;

  /// Son yazmanın hatası. Okuma hatasından ayrıdır: panelin içinde, formun
  /// yanında gösterilir; sayfayı hata ekranına çevirmez.
  String? writeError;

  TaxRepositoryContract get repository => _repository;
  FinancialDataChanges? get changes => _changes;

  DateTime get today => _now();
  String get todayIso => TaxSchedule.iso(_now());

  bool get unauthorized => error?.isUnauthorized ?? false;
  String? get errorMessage => error?.message;

  /// Hiç vergi tanımlanmamış: ekranın ana eylemi tek tutarla ödemedir.
  bool get hasNoPlans => overview?.plans.isEmpty ?? true;

  int get overdueCount =>
      overview?.pending
          .where((item) => item.timing == PlannedTiming.overdue)
          .length ??
      0;

  /// Tek vergi kategorisi varsa o; birden çoksa kullanıcı seçer, hiç yoksa
  /// vergi yazılamaz (sunucu da reddeder).
  TaxChoice? get defaultTaxCategory {
    final categories = options?.taxCategories ?? const [];
    return categories.isEmpty ? null : categories.first;
  }

  Future<void> load() => loadSafely(() async {
    final results = await Future.wait([
      _repository.loadOverview(asOfDate: todayIso),
      _repository.loadOptions(),
    ]);
    overview = results[0] as TaxOverview;
    options = results[1] as TaxOptions;
  });

  /// Hazır türler yalnız tür seçimi açılınca okunur.
  Future<List<TaxSuggestion>> loadSuggestions() async =>
      suggestions ??= await _repository.loadSuggestions();

  Future<bool> pay({
    required PlannedActivity item,
    required String amount,
    required String paidOn,
    required String sourceId,
  }) {
    final card = options?.isCard(sourceId) ?? false;
    return _write(
      () => _repository.pay(
        planId: item.recurringTransactionId!,
        scheduledDate: item.dueDate,
        amount: amount,
        paidOn: paidOn,
        accountId: card ? null : sourceId,
        creditCardId: card ? sourceId : null,
      ),
      () => _changes?.taxPaymentChanged(card: card),
    );
  }

  Future<bool> setAmount({
    required PlannedActivity item,
    required String amount,
  }) => _write(
    () => _repository.setAmount(
      planId: item.recurringTransactionId!,
      scheduledDate: item.dueDate,
      amount: amount,
    ),
    () => _changes?.taxPlansChanged(),
  );

  Future<bool> createPayment({
    required String clientRequestId,
    required String amount,
    required String paidOn,
    required String sourceId,
    required String categoryId,
    required List<PlannedActivity> closes,
    String? note,
  }) {
    final card = options?.isCard(sourceId) ?? false;
    return _write(
      () => _repository.createPayment(
        clientRequestId: clientRequestId,
        amount: amount,
        paidOn: paidOn,
        categoryId: categoryId,
        accountId: card ? null : sourceId,
        creditCardId: card ? sourceId : null,
        note: note,
        closes: [
          for (final item in closes)
            (planId: item.recurringTransactionId!, scheduledDate: item.dueDate),
        ],
      ),
      () => _changes?.taxPaymentChanged(card: card),
    );
  }

  Future<bool> undoPayment(TaxPayment payment) => _write(
    () => _repository.undoPayment(payment.paymentId),
    () => _changes?.taxPaymentChanged(card: payment.isCard),
  );

  Future<bool> createPlans(List<TaxPlanInput> inputs) => _write(
    () => _repository.createPlans(inputs),
    () => _changes?.taxPlansChanged(),
  );

  Future<bool> createPlan(TaxPlanInput input) => _write(
    () => _repository.createPlan(input),
    () => _changes?.taxPlansChanged(),
  );

  Future<bool> updatePlan(String planId, TaxPlanInput input) => _write(
    () => _repository.updatePlan(planId, input),
    () => _changes?.taxPlansChanged(),
  );

  Future<bool> setPlanActive(String planId, bool isActive) => _write(
    () => _repository.setPlanActive(planId, isActive),
    () => _changes?.taxPlansChanged(),
  );

  Future<bool> deletePlan(String planId) => _write(
    () => _repository.deletePlan(planId),
    () => _changes?.taxPlansChanged(),
  );

  /// Yazma: hata controller'da kalır ve formun yanında gösterilir; başarıda
  /// değişiklik bildirilir. Bildirim yoksa (test, bağlanmamış kabuk) ekran
  /// kendini yeniler.
  Future<bool> _write(
    Future<void> Function() operation,
    void Function() notify,
  ) async {
    if (_isSubmitting) return false;
    _isSubmitting = true;
    writeError = null;
    notifyListeners();
    try {
      await operation();
      if (_changes == null) {
        await load();
      } else {
        notify();
      }
      return true;
    } on ApiException catch (error) {
      writeError = error.message;
      return false;
    } on FormatException {
      writeError = ApiException.local('response.invalid_format').message;
      return false;
    } finally {
      _isSubmitting = false;
      notifyListeners();
    }
  }

  /// Panel açılırken önceki panelin hatası taşınmasın.
  void clearWriteError() {
    if (writeError == null) return;
    writeError = null;
    notifyListeners();
  }

  void _handleChanges() {
    final changes = _changes;
    if (changes == null) return;
    final feed = changes.activityFeedRevision;
    final planning = changes.planningRevision;
    if (feed == _seenFeed && planning == _seenPlanning) return;
    _seenFeed = feed;
    _seenPlanning = planning;
    load();
  }

  @override
  void dispose() {
    _changes?.removeListener(_handleChanges);
    super.dispose();
  }
}

/// İstek kimliği: toplu vergi ödemesi aynı istek ikinci kez gelirse ikinci
/// gider yazmaz (sunucu kimliği bundan türetir).
String newTaxRequestId() {
  final random = Random.secure();
  String hex(int length) =>
      List.generate(length, (_) => random.nextInt(16).toRadixString(16)).join();
  return '${hex(8)}-${hex(4)}-4${hex(3)}-'
      '${['8', '9', 'a', 'b'][random.nextInt(4)]}${hex(3)}-${hex(12)}';
}
