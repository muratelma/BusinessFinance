import 'package:flutter/foundation.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../data/activity_repository.dart';
import '../data/planned_activity_models.dart';

/// Planlananlar'ın dönemi: bugünden başlayan hazır bir gün sayısı ya da
/// takvimden seçilen aralık. İki uç da dahildir.
@immutable
class PlannedPeriodChoice {
  const PlannedPeriodChoice.days(int this.days) : from = null, to = null;
  const PlannedPeriodChoice.range(DateTime this.from, DateTime this.to)
    : days = null;

  /// Hazır dönemler: `Önümüzdeki 7 / 30 / 90 gün`.
  static const presets = [7, 30, 90];

  final int? days;
  final DateTime? from;
  final DateTime? to;

  bool get isRange => days == null;

  /// Bugün dahil N gün: 8 Aralık'ta 7 gün 8–14 Aralık'tır.
  DateTime start(DateTime today) => from ?? today;
  DateTime end(DateTime today) =>
      to ?? DateTime(today.year, today.month, today.day + days! - 1);

  @override
  bool operator ==(Object other) =>
      other is PlannedPeriodChoice &&
      other.days == days &&
      other.from == from &&
      other.to == to;

  @override
  int get hashCode => Object.hash(days, from, to);
}

class PlannedActivityController extends ChangeNotifier {
  PlannedActivityController(
    this._repository, {
    this.financialDataChanges,
    this.scopeController,
    this.asOf,
  }) : period = asOf == null
           ? const PlannedPeriodChoice.days(30)
           : const PlannedPeriodChoice.days(7),
       _seenRevision = financialDataChanges?.planningRevision ?? 0,
       _seenScope = scopeController?.scope {
    financialDataChanges?.addListener(_handleFinancialDataChanged);
    scopeController?.addListener(_handleScopeChanged);
  }

  final ActivityRepositoryContract _repository;

  /// Özet'ten gelindiyse onun okuduğu gün; yoksa bugün.
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
  PlannedPeriodChoice period;
  PlannedTypeFilter typeFilter = PlannedTypeFilter.all;

  /// Aynı seçimin yenilemesi başarısız oldu; son okunan liste ekranda kalır
  /// ve `Liste güncellenemedi.` yazılır (18-01).
  bool refreshFailed = false;

  /// Son başarılı okumanın seçimi; farklı bir seçimin eski listesi yeni
  /// başlıkla gösterilmez.
  Object? _pageSelection;

  DateTime get today {
    final day = asOf ?? DateTime.now();
    return DateTime(day.year, day.month, day.day);
  }

  DateTime get from => period.start(today);
  DateTime get to => period.end(today);

  /// Başlıktaki dönem: aynı ayda `8–21 Aralık`, değilse `28 Kasım – 4 Aralık`.
  String get periodLabel {
    final a = from;
    final b = to;
    final year = a.year != b.year || a.year != today.year;
    String day(DateTime d) =>
        '${d.day} ${DateText.months[d.month - 1]}${year ? ' ${d.year}' : ''}';
    if (a == b) return day(a);
    if (a.year == b.year && a.month == b.month) {
      return '${a.day}–${day(b)}';
    }
    return '${day(a)} – ${day(b)}';
  }

  /// Önceki gecikenlerin satırındaki tarih: `8 Aralık öncesi`.
  String get earlierLabel =>
      '${from.day} ${DateText.months[from.month - 1]} öncesi';

  /// Seçimden önce gecikenler (vadesi hem başlangıçtan hem bugünden önce).
  List<PlannedActivity> get earlierItems => [
    ...?page?.items.where((item) => _isBeforeStart(item.dueDate)),
  ];

  /// Seçilen aralıktaki kayıtlar, vade sırasıyla.
  List<PlannedActivity> get periodItems => [
    ...?page?.items.where((item) => !_isBeforeStart(item.dueDate)),
  ];

  bool _isBeforeStart(String iso) {
    final due = DateTime.tryParse(iso);
    return due != null && due.isBefore(from);
  }

  bool get isEmpty =>
      !isLoading && errorMessage == null && page != null && page!.items.isEmpty;

  /// Görünümün o an okuduğu kapsam.
  TransactionScope? get scope => scopeController?.scope;

  bool get isScopeVisible => scopeController?.isVisible ?? false;

  Object get _selection => (period, typeFilter, scope);

  Future<void> load() async {
    final request = ++_loadGeneration;
    final selection = _selection;
    if (_pageSelection != selection) page = null;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      final loaded = await _repository.listPlannedRange(
        from: from,
        to: to,
        today: today,
        scope: scope,
        type: typeFilter,
      );
      if (request != _loadGeneration) return;
      page = loaded;
      _pageSelection = selection;
      refreshFailed = false;
      unauthorized = false;
    } on ApiException catch (error) {
      if (request != _loadGeneration) return;
      unauthorized = error.isUnauthorized;
      _failed(error.message);
    } on FormatException {
      if (request != _loadGeneration) return;
      _failed('Sunucudan beklenmeyen bir planlama yanıtı alındı.');
    } finally {
      if (request == _loadGeneration) {
        isLoading = false;
        notifyListeners();
      }
    }
  }

  /// Aynı seçimin son listesi varsa yerinde kalır, yalnız uyarı çıkar; yoksa
  /// ekran hata görünümüne geçer.
  void _failed(String message) {
    if (page != null && _pageSelection == _selection) {
      refreshFailed = true;
      return;
    }
    page = null;
    errorMessage = message;
  }

  /// Son gerçekleştirme isteği, planın daha önceki bir kalemi beklediği için
  /// onay istedi. Hata değildir: ekran sorar ve onayla aynı isteği yineler.
  bool earlierItemPending = false;

  /// Bekleyen en eski kalemin vadesi ve bekleyen kalem sayısı; sunucu
  /// göndermediyse boştur.
  String? earlierPendingDate;
  int? earlierPendingCount;

  /// Planlanan satırı gerçek harekete çevirir; başarıda `true`.
  ///
  /// Liste burada yeniden yüklenmez: [FinancialDataChanges] sinyali zaten
  /// yeniden yüklemeyi tetikler.
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

  /// Filtre panelinin `Sonuçları göster`i: dönem ve tür birlikte uygulanır.
  Future<void> apply({
    required PlannedPeriodChoice period,
    required PlannedTypeFilter type,
  }) async {
    if (this.period == period && typeFilter == type) return;
    this.period = period;
    typeFilter = type;
    await load();
  }

  /// `Filtreyi temizle` ve etiketin ✕'i: türü kaldırır, dönemi korur.
  Future<void> clearType() =>
      apply(period: period, type: PlannedTypeFilter.all);

  Future<void> selectScope(TransactionScope? value) async {
    await scopeController?.select(value);
  }

  void _handleScopeChanged() {
    final current = scopeController?.scope;
    if (current == _seenScope) return;
    _seenScope = current;
    load();
  }

  void _handleFinancialDataChanged() {
    final changes = financialDataChanges;
    if (changes == null) return;
    final revision = changes.planningRevision;
    if (revision == _seenRevision) return;
    _seenRevision = revision;
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
