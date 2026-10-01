import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/pos_repository.dart';

class PosController extends ChangeNotifier {
  PosController(this._repository, {this.changes})
    : _seenCashRevision = changes?.cashRevision ?? 0 {
    changes?.addListener(_handleFinancialDataChanged);
  }

  final PosRepositoryContract _repository;
  final FinancialDataChanges? changes;

  /// Kasa ekranındaki POS listesi de `cash` hedefini izler; kendi yazdığı
  /// değişiklikte kendini ikinci kez yüklemez.
  int _seenCashRevision;

  PosSettlementList? settlements;
  PosOptions? options;

  /// POS tanımları (ADR 0019 T4). `null`: henüz okunmadı.
  List<PosDefinitionItem>? definitions;

  /// Tanım yazma ya da okuma hatası; tahsilat listesinin hatasından ayrıdır,
  /// kendi sayfasında gösterilir.
  String? definitionError;

  List<PosDefinitionItem> get activeDefinitions => [
    for (final definition in definitions ?? const <PosDefinitionItem>[])
      if (definition.isActive) definition,
  ];

  /// Tahsilat formunda seçili gelecek POS: ana POS, yoksa listedeki ilk
  /// aktif POS.
  PosDefinitionItem? get preferredDefinition {
    final active = activeDefinitions;
    for (final definition in active) {
      if (definition.isDefault) return definition;
    }
    return active.isEmpty ? null : active.first;
  }

  /// Yalnız yolda olanları göster.
  bool inTransitOnly = false;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  bool isStale = false;
  String? errorMessage;

  List<PosSettlementItem> get items => settlements?.items ?? const [];

  String get moneyInTransit => settlements?.moneyInTransit ?? '0.0000';

  int get inTransitCount => settlements?.inTransitCount ?? 0;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      settlements = await _repository.list(inTransitOnly: inTransitOnly);
      unauthorized = false;
      isStale = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      isStale = settlements != null;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      isStale = settlements != null;
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> setInTransitOnly(bool value) async {
    if (inTransitOnly == value) return;
    inTransitOnly = value;
    await load();
  }

  /// Hesap ve kategori seçenekleri **her açılışta yeniden okunur**. Bir kez
  /// okunup saklandığında sonradan açılan hesap formda görünmüyor, silinen
  /// hesap ise seçenek olarak kalıyordu (1 Ekim 2026 emülatör denemesi).
  Future<PosOptions> loadOptions() async {
    return options = await _repository.loadOptions();
  }

  /// Komisyon **ya tutar ya oran** olarak gider; ikisi birden gönderilirse
  /// sunucu isteği reddeder ve haklıdır: iki gerçek arasında seçim yapmak
  /// onun işi değildir.
  ///
  /// [posDefinitionId] verilirse hesap, kategori, komisyon ve beklenen gün
  /// boş bırakılabilir; sunucu tanımdan doldurur.
  Future<bool> create({
    required String grossAmount,
    required String settlementDate,
    required TransactionScope? scope,
    String? accountId,
    String? categoryId,
    String? expectedTransferDate,
    String? commissionAmount,
    String? commissionRate,
    String? commissionCategoryId,
    String? description,
    String? posDefinitionId,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.create({
        'accountId': accountId,
        'categoryId': categoryId,
        'grossAmount': grossAmount,
        'currency': 'TRY',
        'settlementDate': settlementDate,
        'expectedTransferDate': expectedTransferDate,
        'commissionAmount': commissionAmount,
        'commissionRate': commissionRate,
        'commissionCategoryId': commissionCategoryId,
        'scope': scope?.apiValue,
        'description': description,
        'posDefinitionId': posDefinitionId,
      });
      _announce((c) => c.posSettlementRecognized());
      await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<bool> markTransferred(
    PosSettlementItem item,
    String transferDate,
  ) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _repository.markTransferred(
        settlementId: item.id,
        transferDate: transferDate,
      );
      _announce((c) => c.posSettlementTransferred());
      await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  /// Tanımları okur. Hata tahsilat listesini bozmaz: form tanımsız yoluyla
  /// çalışmaya devam eder.
  Future<void> loadDefinitions() async {
    try {
      definitions = await _repository.listDefinitions();
      definitionError = null;
    } on ApiException catch (error) {
      unauthorized = unauthorized || error.isUnauthorized;
      definitionError = error.message;
    } on FormatException {
      definitionError = 'Sunucudan beklenmeyen bir yanıt alındı.';
    }
    notifyListeners();
  }

  /// Tanım para taşımaz; hiçbir finansal hedefi yükseltmez. Yalnız tanım
  /// listesi (ve tanımın adını taşıyan tahsilat listesi) yeniden okunur.
  Future<bool> saveDefinition(
    PosDefinitionInput input, {
    String? definitionId,
  }) => _mutateDefinition(
    () => _repository.saveDefinition(input, definitionId: definitionId),
  );

  Future<bool> setDefinitionActive(PosDefinitionItem item, bool isActive) =>
      _mutateDefinition(
        () => _repository.setDefinitionActive(
          definitionId: item.id,
          isActive: isActive,
        ),
      );

  Future<bool> setDefaultDefinition(PosDefinitionItem item) =>
      _mutateDefinition(
        () => _repository.setDefaultDefinition(definitionId: item.id),
      );

  Future<bool> deleteDefinition(PosDefinitionItem item) => _mutateDefinition(
    () => _repository.deleteDefinition(definitionId: item.id),
  );

  /// Önizleme hatası formu durdurmaz; `null` döner ve özet çizilmez.
  Future<PosPreview?> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  }) async {
    try {
      return await _repository.preview(
        definitionId: definitionId,
        grossAmount: grossAmount,
        settlementDate: settlementDate,
      );
    } on ApiException {
      return null;
    } on FormatException {
      return null;
    }
  }

  Future<bool> _mutateDefinition(Future<void> Function() request) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    definitionError = null;
    notifyListeners();
    try {
      await request();
      await loadDefinitions();
      if (settlements != null) await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = unauthorized || error.isUnauthorized;
      definitionError = error.message;
      return false;
    } on FormatException {
      definitionError = 'Sunucudan beklenmeyen bir yanıt alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  /// Yanlışlıkla "hesaba geçti" denmiş tahsilatı yeniden yola döndürür.
  Future<bool> revertTransfer(PosSettlementItem item) => _mutate(
    () => _repository.revertTransfer(settlementId: item.id),
    (c) => c.posSettlementTransferReverted(),
  );

  /// Silme yerine iptal.
  Future<bool> cancel(PosSettlementItem item) => _mutate(
    () => _repository.cancel(settlementId: item.id),
    (c) => c.posSettlementCancelled(),
  );

  Future<bool> _mutate(
    Future<void> Function() request,
    void Function(FinancialDataChanges changes) raise,
  ) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    notifyListeners();
    try {
      await request();
      _announce(raise);
      await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  void _announce(void Function(FinancialDataChanges changes) raise) {
    final current = changes;
    if (current == null) return;
    _seenCashRevision = current.cashRevision + 1;
    raise(current);
  }

  void _handleFinancialDataChanged() {
    final revision = changes?.cashRevision ?? 0;
    if (revision == _seenCashRevision) return;
    _seenCashRevision = revision;
    load();
  }

  @override
  void dispose() {
    changes?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }
}
