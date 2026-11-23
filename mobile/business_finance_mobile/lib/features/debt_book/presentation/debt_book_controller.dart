import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/loadable_view_model.dart';
import '../../../core/presentation/scope_controller.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';

/// `Borç ve alacak` listesinin durumu. Kapsam uygulamanın tek kapsam
/// anahtarından okunur ([ScopeController]); anahtar değişince liste o kapsamla
/// yeniden okunur. Borç, cari ya da akış değişince de yenilenir.
class DebtBookController extends LoadableViewModel {
  DebtBookController(
    this._repository, {
    this.scopeController,
    FinancialDataChanges? changes,
    DateTime Function()? now,
  }) : _changes = changes,
       _now = now ?? DateTime.now {
    _seenScope = scopeController?.scope;
    _seenCounterparties = changes?.counterpartiesRevision ?? 0;
    _seenFeed = changes?.activityFeedRevision ?? 0;
    scopeController?.addListener(_handleScope);
    _changes?.addListener(_handleChanges);
  }

  final DebtBookRepositoryContract _repository;
  final ScopeController? scopeController;
  final FinancialDataChanges? _changes;
  final DateTime Function() _now;
  TransactionScope? _seenScope;
  int _seenCounterparties = 0;
  int _seenFeed = 0;
  int _generation = 0;

  DebtBook? book;

  bool get unauthorized => error?.isUnauthorized ?? false;
  String? get errorMessage => error?.message;

  /// Okuma başarısız ama elde önceki liste var: liste yerinde kalır, üstünde
  /// uyarı çizilir.
  bool get isStale => book != null && error != null && !unauthorized;

  TransactionScope? get scope => scopeController?.scope;
  bool get scopeVisible => scopeController?.isVisible ?? false;

  String get todayIso {
    final today = _now();
    return '${today.year.toString().padLeft(4, '0')}-'
        '${today.month.toString().padLeft(2, '0')}-'
        '${today.day.toString().padLeft(2, '0')}';
  }

  Future<void> load() {
    final generation = ++_generation;
    final scope = this.scope;
    return loadSafely(() async {
      final result = await _repository.load(asOfDate: todayIso, scope: scope);
      if (generation == _generation) book = result;
    }, isCurrent: () => generation == _generation);
  }

  /// Sayfanın kapsam menüsü uygulamanın genel kapsamını değiştirir.
  Future<void> selectScope(TransactionScope? value) async =>
      scopeController?.select(value);

  void _handleScope() {
    final current = scopeController?.scope;
    if (current == _seenScope) return;
    _seenScope = current;
    load();
  }

  void _handleChanges() {
    final changes = _changes;
    if (changes == null) return;
    final counterparties = changes.counterpartiesRevision;
    final feed = changes.activityFeedRevision;
    if (counterparties == _seenCounterparties && feed == _seenFeed) return;
    _seenCounterparties = counterparties;
    _seenFeed = feed;
    load();
  }

  @override
  void dispose() {
    scopeController?.removeListener(_handleScope);
    _changes?.removeListener(_handleChanges);
    super.dispose();
  }
}
