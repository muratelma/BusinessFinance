import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/loadable_view_model.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'debt_settle_sheet.dart';

/// Taksitli ya da kişisiz kaydın sayfası (Faz 7): okuma ve yazmalar. Her
/// yazma kaydı yeniden okur; sayıların hepsi sunucudandır.
class DebtRecordController extends LoadableViewModel {
  DebtRecordController(
    this._repository, {
    required this.debtId,
    this.changes,
    DateTime Function()? now,
  }) : _now = now ?? DateTime.now;

  final DebtBookRepositoryContract _repository;
  final String debtId;
  final FinancialDataChanges? changes;
  final DateTime Function() _now;

  DebtRecordData? record;
  DebtSettleSources? sources;

  /// Kayıt iptal edildi; sayfa kapanır.
  bool cancelled = false;

  bool get unauthorized => error?.isUnauthorized ?? false;
  String? get errorMessage => error?.message;
  bool get isStale => record != null && error != null && !unauthorized;
  DateTime get today => _now();

  String get todayIso {
    final day = _now();
    return '${day.year.toString().padLeft(4, '0')}-'
        '${day.month.toString().padLeft(2, '0')}-'
        '${day.day.toString().padLeft(2, '0')}';
  }

  Future<void> load() => loadSafely(() async {
    final results = await Future.wait([
      _repository.loadRecord(debtId, asOfDate: todayIso),
      _loadSources(),
    ]);
    record = _named(results[0] as DebtRecordData);
  });

  Future<DebtSettleSources?> _loadSources() async {
    try {
      return sources ??= await _repository.loadSettleSources();
    } on ApiException {
      // Paranın yerinin adı olmadan da sayfa okunur.
      return null;
    }
  }

  Future<DebtSettleSources> loadSources() async =>
      sources ??= await _repository.loadSettleSources();

  /// Kapanışın yerine adını verir (cevap yalnız kimliği taşır).
  DebtRecordData _named(DebtRecordData data) {
    final known = sources;
    if (known == null) return data;
    String? nameOf(DebtRecordClosing closing) {
      if (closing.isPos) return 'POS';
      for (final account in known.accounts) {
        if (account.id == closing.accountId) return account.name;
      }
      for (final card in known.cards) {
        if (card.id == closing.creditCardId) return card.name;
      }
      return null;
    }

    return DebtRecordData(
      id: data.id,
      title: data.title,
      side: data.side,
      remaining: data.remaining,
      planKind: data.planKind,
      counterpartyId: data.counterpartyId,
      counterpartyName: data.counterpartyName,
      scope: data.scope,
      isInterestBearing: data.isInterestBearing,
      remainingPrincipal: data.remainingPrincipal,
      isCarriedOver: data.isCarriedOver,
      sourceType: data.sourceType,
      installments: data.installments,
      closings: [
        for (final closing in data.closings) closing.named(nameOf(closing)),
      ],
    );
  }

  /// Ödeme ya da tahsilat. Yazıldıysa `null`, reddedildiyse cümle.
  Future<String?> pay(DebtSettlement settlement) {
    final colon = settlement.sourceId.indexOf(':');
    final kind = settlement.sourceId.substring(0, colon);
    final id = settlement.sourceId.substring(colon + 1);
    return _write(
      () => _repository.payRecord(debtId, {
        'amount': settlement.amount,
        'paymentDate': settlement.date,
        'asOfDate': todayIso,
        'clientRequestId': settlement.clientRequestId,
        if (kind == 'account') 'accountId': id,
        if (kind == 'card') 'creditCardId': id,
        if (kind == 'pos') 'card': {'posDefinitionId': id},
      }),
      card: kind == 'card',
    );
  }

  Future<String?> reverse(DebtRecordClosing closing) => _write(
    () => _repository.reverseRecordPayment(
      debtId,
      closing.id,
      asOfDate: todayIso,
    ),
    card: closing.creditCardId != null,
  );

  Future<String?> discount(String amount, String date, String requestId) =>
      _write(
        () => _repository.discountRecord(debtId, {
          'amount': amount,
          'discountDate': date,
          'asOfDate': todayIso,
          'clientRequestId': requestId,
        }),
      );

  Future<String?> changePlan({
    required String planKind,
    required String? firstDueDate,
    required int installmentCount,
  }) => _write(
    () => _repository.changeRecordPlan(debtId, {
      'planKind': planKind,
      'firstDueDate': firstDueDate,
      'installmentCount': installmentCount,
      'asOfDate': todayIso,
    }),
  );

  Future<String?> cancel() async {
    try {
      await _repository.cancelRecord(debtId, asOfDate: todayIso);
    } on ApiException catch (error) {
      return error.message;
    }
    cancelled = true;
    changes?.debtChanged();
    notifyListeners();
    return null;
  }

  Future<String?> _write(
    Future<void> Function() operation, {
    bool card = false,
  }) async {
    try {
      await operation();
    } on ApiException catch (error) {
      return error.message;
    }
    // Borcun her yazması hesabı, raporu ve planlananları değiştirebilir;
    // kartla ödeme kart borcunu da.
    changes?.debtChanged();
    if (card) changes?.cardPaymentChanged();
    await load();
    return null;
  }
}
