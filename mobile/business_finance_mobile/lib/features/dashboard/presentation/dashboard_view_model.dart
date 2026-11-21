import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/loadable_view_model.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../activities/data/activity_repository.dart';
import '../../activities/data/planned_activity_models.dart';
import '../../planning/data/planning_models.dart';
import '../data/dashboard_models.dart';
import '../data/dashboard_repository.dart';

class DashboardViewModel extends LoadableViewModel {
  DashboardViewModel(
    this._repository, {
    this._activityRepository,
    FinancialDataChanges? changes,
    ScopeController? scopeController,
    DateTime Function()? now,
  }) : _changes = changes,
       _scopeController = scopeController,
       _seenDashboardRevision = changes?.dashboardRevision ?? 0,
       _seenScope = scopeController?.scope,
       _now = now ?? DateTime.now {
    final current = _now();
    year = current.year;
    month = current.month;
    _changes?.addListener(_handleFinancialDataChanged);
    _scopeController?.addListener(_handleScopeChanged);
  }

  final DashboardDataSource _repository;

  /// Gecikmiş yükümlülükler aynı planlanan projeksiyondan okunuyor; özet
  /// ekranı için ikinci bir sorgu tutmuyoruz.
  final ActivityRepositoryContract? _activityRepository;
  final FinancialDataChanges? _changes;

  /// Uygulama genelindeki kapsam anahtarı. Gelir/gider tarafını böler;
  /// net varlık ve hesap bakiyeleri ondan etkilenmez (ADR 0013).
  final ScopeController? _scopeController;
  final DateTime Function() _now;
  int _seenDashboardRevision;
  TransactionScope? _seenScope;

  late int year;
  late int month;
  DashboardReport? report;

  /// İkincil okuma. Null olması bir hata durumu değildir: ay özeti tek başına
  /// geçerli bir ekrandır ve gelişmiş rapor düşerse yalnız o bölümler gizlenir.
  AdvancedReport? advanced;

  /// Vadesi geçmiş, hâlâ ödenmemiş yükümlülükler.
  ///
  /// Gerçekleşmemiş bir kart taksidi hiçbir yerde borç ya da gider üretmiyor —
  /// bu doğru, onaysız para hareketi olmuyor. Ama kullanıcı `Gerçekleştir`'e
  /// basmayı unutursa gerçekten yapılmış bir harcama sessizce kayıt dışı
  /// kalıyordu. Özet ekranı bu sessizliği bozan yer.
  List<PlannedActivity> overdue = const [];

  /// Vadesi henüz gelmemiş yükümlülükler — bugün dâhil, yakın uçtan uzağa.
  ///
  /// Aynı istekten geliyor: sorgu zaten yapılıyordu, gecikmişler süzülüp
  /// gerisi atılıyordu. Ekranın "önümde ne var" sorusunu yanıtlayan yarısı bu
  /// listeydi ve indirilip çöpe atılıyordu.
  List<PlannedActivity> upcoming = const [];

  /// Yaklaşanların hepsi gösterilmiyor; kaçının ekranda olmadığı bu sayıda.
  int upcomingHiddenCount = 0;

  /// Pencere içinde vadesi gelmemiş ödemelerin sunucudaki toplamı
  /// ("7 günde çıkacak"); okunamadıysa `null`.
  String? upcomingOutgoingTotal;
  DateTime? plannedAsOf;
  int? upcomingPaymentCount;
  int? unknownAmountCount;
  bool plannedReadFailed = false;

  /// Gecikmiş ödemelerin tutarı belli olanlarının sunucudaki toplamı;
  /// okunamadıysa `null`. Yaklaşanlar kartının gecikenler satırı bunu yazar.
  String? overdueOutgoingTotal;

  /// Yaklaşan listesinin penceresi. Ekranda yazılı olmak zorunda: aynı
  /// ekranda 30 günlük başka bir toplam da bulunabiliyor ve iki pencere
  /// etiketsiz yan yana durursa kullanıcı ikisini karşılaştırıp tutturamaz.
  static const upcomingHorizon = PlannedHorizon.week;

  /// Ekranda gösterilen en fazla satır sayısı.
  static const upcomingVisibleCount = 4;

  /// Ekranın o an okuduğu kapsam; başlıkta da bu yazılı durur.
  TransactionScope? get scope => _scopeController?.scope;

  /// Göreli vade etiketleri (`Yarın`, `3 gün sonra`) bu güne göre yazılır.
  DateTime get today => _now();

  /// Kapsam boyutu bu kullanıcıda görünür mü.
  bool get isScopeVisible => _scopeController?.isVisible ?? false;

  /// Anahtarın konumunu değiştirir. Yeniden okuma kendi dinleyicisinden
  /// gelir; burada ikinci bir `load()` çağrısı iki isteğe dönüşürdü.
  Future<void> selectScope(TransactionScope? value) async =>
      _scopeController?.select(value);

  int _loadGeneration = 0;
  TransactionScope? _loadedScope;
  Future<void> load() {
    final request = ++_loadGeneration;
    final selectedScope = scope;
    final selectedYear = year;
    final selectedMonth = month;
    bool current() => request == _loadGeneration;
    if (_loadedScope != selectedScope ||
        report?.year != year ||
        report?.month != month) {
      report = null;
    }
    return loadSafely(() async {
      final monthly = await _repository.getMonthly(
        selectedYear,
        selectedMonth,
        scope: selectedScope,
      );
      if (!current()) return;
      AdvancedReport? details;
      try {
        details = await _repository.getAdvanced(
          selectedYear,
          selectedMonth,
          scope: selectedScope,
        );
      } on Exception {
        details = null;
      }
      if (!current()) return;
      await _loadOverdue(current, selectedScope);
      if (!current()) return;
      report = monthly;
      advanced = details;
      _loadedScope = selectedScope;
    }, isCurrent: current);
  }

  /// Ay özetini düşürmemek için ayrı ve bağışlayıcı: buradaki bir hata
  /// kullanıcıyı bütün ekrandan etmemeli.
  Future<void> _loadOverdue(
    bool Function() current,
    TransactionScope? selectedScope,
  ) async {
    plannedAsOf = _now();
    upcomingPaymentCount = null;
    unknownAmountCount = null;
    plannedReadFailed = true;
    if (_activityRepository == null) {
      overdue = const [];
      upcoming = const [];
      upcomingHiddenCount = 0;
      upcomingOutgoingTotal = null;
      overdueOutgoingTotal = null;
      return;
    }
    try {
      // Gecikmişlerin ufukla ilgisi yok: sunucu alt sınır uygulamıyor, vadesi
      // geçmiş her kayıt hangi ufuk istenirse istensin dönüyor. En kısasını
      // istemek sorguyu gereksiz büyütmemek için.
      final page = await _activityRepository.listPlanned(
        horizon: upcomingHorizon,
        today: plannedAsOf,
        scope: selectedScope,
      );
      if (!current()) return;
      final obligations = page.items
          .where((item) => item.isPaymentObligation)
          .toList();
      overdue = obligations
          .where((item) => item.timing == PlannedTiming.overdue)
          .toList(growable: false);
      // Bugün vadesi gelenler yaklaşanlarla birlikte gösteriliyor: henüz
      // gecikmemişler, ama uyarı bandına da girmiyorlar. Aralarında kalıp
      // hiçbir yerde görünmemeleri, bandın kapatmaya çalıştığı sessizliğin
      // aynısı olurdu.
      final ahead =
          page.items
              .where((item) => item.timing != PlannedTiming.overdue)
              .toList()
            ..sort((a, b) => a.dueDate.compareTo(b.dueDate));
      upcoming = ahead.take(upcomingVisibleCount).toList(growable: false);
      upcomingOutgoingTotal = page.upcomingOutgoingTotal;
      upcomingPaymentCount = page.upcomingPaymentCount;
      unknownAmountCount = page.unknownAmountCount;
      plannedReadFailed = false;
      overdueOutgoingTotal = page.overdueOutgoingTotal;
      upcomingHiddenCount = ahead.length - upcoming.length;
    } on Exception {
      if (!current()) return;
      overdue = const [];
      upcoming = const [];
      upcomingHiddenCount = 0;
      upcomingOutgoingTotal = null;
      overdueOutgoingTotal = null;
    }
  }

  Future<void> previousMonth() async {
    final previous = DateTime(year, month - 1);
    year = previous.year;
    month = previous.month;
    await load();
  }

  Future<void> nextMonth() async {
    final next = DateTime(year, month + 1);
    year = next.year;
    month = next.month;
    await load();
  }

  /// Anahtar konum değiştirdiğinde ekran yeniden okunur. Aynı konuma
  /// dokunmak (ya da yalnız profilin yüklenmesi) yeniden okuma başlatmaz.
  void _handleScopeChanged() {
    final current = _scopeController?.scope;
    if (current == _seenScope) return;
    _seenScope = current;
    load();
  }

  void _handleFinancialDataChanged() {
    final revision = _changes?.dashboardRevision ?? 0;
    if (revision == _seenDashboardRevision) return;
    _seenDashboardRevision = revision;
    load();
  }

  @override
  void dispose() {
    _loadGeneration++;
    _changes?.removeListener(_handleFinancialDataChanged);
    _scopeController?.removeListener(_handleScopeChanged);
    super.dispose();
  }
}
