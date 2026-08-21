import '../../../core/presentation/loadable_view_model.dart';
import '../../../core/presentation/financial_data_changes.dart';
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
    DateTime Function()? now,
  }) : _changes = changes,
       _seenDashboardRevision = changes?.dashboardRevision ?? 0,
       _now = now ?? DateTime.now {
    final current = _now();
    year = current.year;
    month = current.month;
    _changes?.addListener(_handleFinancialDataChanged);
  }

  final DashboardDataSource _repository;

  /// Gecikmiş yükümlülükler aynı planlanan projeksiyondan okunuyor; özet
  /// ekranı için ikinci bir sorgu tutmuyoruz.
  final ActivityRepositoryContract? _activityRepository;
  final FinancialDataChanges? _changes;
  final DateTime Function() _now;
  int _seenDashboardRevision;

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

  /// Yaklaşan listesinin penceresi. Ekranda yazılı olmak zorunda: aynı
  /// ekranda 30 günlük başka bir toplam da bulunabiliyor ve iki pencere
  /// etiketsiz yan yana durursa kullanıcı ikisini karşılaştırıp tutturamaz.
  static const upcomingHorizon = PlannedHorizon.week;

  /// Ekranda gösterilen en fazla satır sayısı.
  static const upcomingVisibleCount = 3;

  Future<void> load() => loadSafely(() async {
    report = await _repository.getMonthly(year, month);
    await _loadAdvanced();
    await _loadOverdue();
  });

  /// Ay özetini düşürmemek için ayrı ve bağışlayıcı: buradaki bir hata
  /// kullanıcıyı bütün ekrandan etmemeli.
  Future<void> _loadAdvanced() async {
    try {
      advanced = await _repository.getAdvanced(year, month);
    } on Exception {
      advanced = null;
    }
  }

  /// Gelişmiş rapor gibi bağışlayıcı: uyarı bandının düşmesi kullanıcıyı ay
  /// özetinden etmemeli.
  Future<void> _loadOverdue() async {
    if (_activityRepository == null) {
      overdue = const [];
      upcoming = const [];
      upcomingHiddenCount = 0;
      return;
    }
    try {
      // Gecikmişlerin ufukla ilgisi yok: sunucu alt sınır uygulamıyor, vadesi
      // geçmiş her kayıt hangi ufuk istenirse istensin dönüyor. En kısasını
      // istemek sorguyu gereksiz büyütmemek için.
      final page = await _activityRepository.listPlanned(
        horizon: upcomingHorizon,
        today: _now(),
      );
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
          obligations
              .where((item) => item.timing != PlannedTiming.overdue)
              .toList()
            ..sort((a, b) => a.dueDate.compareTo(b.dueDate));
      upcoming = ahead.take(upcomingVisibleCount).toList(growable: false);
      upcomingHiddenCount = ahead.length - upcoming.length;
    } on Exception {
      overdue = const [];
      upcoming = const [];
      upcomingHiddenCount = 0;
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

  void _handleFinancialDataChanged() {
    final revision = _changes?.dashboardRevision ?? 0;
    if (revision == _seenDashboardRevision) return;
    _seenDashboardRevision = revision;
    load();
  }

  @override
  void dispose() {
    _changes?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }
}
