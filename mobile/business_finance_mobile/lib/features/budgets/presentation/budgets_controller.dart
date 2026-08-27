import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../data/budget_models.dart';
import '../data/budget_repository.dart';

class BudgetsController extends ChangeNotifier {
  BudgetsController(
    this._repository, {
    FinancialDataChanges? changes,
    ScopeController? scopeController,
    DateTime? initialMonth,
    // Adı `_` ile başlayan bir named parametre Dart'ta yasak; bu iki alan
    // initializing formal olamaz.
    // ignore: prefer_initializing_formals
  }) : _scopeController = scopeController,
       _changes = changes,
       _seenBudgetsRevision = changes?.budgetsRevision ?? 0,
       selectedMonth = DateTime(
         (initialMonth ?? DateTime.now()).year,
         (initialMonth ?? DateTime.now()).month,
       ) {
    _changes?.addListener(_handleFinancialDataChanged);
  }

  final BudgetRepositoryContract _repository;
  final FinancialDataChanges? _changes;

  /// Yalnız **görünürlük** için: bütçe listesi kapsam anahtarıyla
  /// filtrelenmez. Bütçe kendi kapsamını taşır ve her iki tarafın sınırı da
  /// aynı listede durur; anahtar burada bir soru sormaz, yalnız kapsam
  /// boyutunun arayüzde olup olmadığını söyler.
  final ScopeController? _scopeController;
  int _seenBudgetsRevision;
  DateTime selectedMonth;
  List<BudgetItem>? items;
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      items = await _repository.list(selectedMonth.year, selectedMonth.month);
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir bütçe yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> changeMonth(int offset) async {
    selectedMonth = DateTime(selectedMonth.year, selectedMonth.month + offset);
    items = null;
    await load();
  }

  /// Uzağa gitmek için: oklar komşu ay içindir.
  Future<void> selectMonth(DateTime month) async {
    final normalized = DateTime(month.year, month.month);
    if (normalized == selectedMonth) return;
    selectedMonth = normalized;
    items = null;
    await load();
  }

  bool get isScopeVisible => _scopeController?.isVisible ?? false;

  /// O ay zaten bütçesi olan kategoriler.
  ///
  /// Tekil indeks (kullanıcı + kategori + yıl + ay) ikincisini reddediyor;
  /// seçilebilir bırakmak, kullanıcıyı sunucunun İngilizce çakışma cevabına
  /// götürürdü.
  Set<String> get budgetedCategoryIds => {
    for (final item in items ?? const <BudgetItem>[]) item.categoryId,
  };

  Future<bool> create(CreateBudgetInput input) async =>
      _submit(() => _repository.create(input), 'Aylık bütçe oluşturuldu.');

  Future<bool> update(String id, String limit) async =>
      _submit(() => _repository.update(id, limit), 'Bütçe limiti güncellendi.');

  Future<bool> _submit(
    Future<BudgetItem> Function() action,
    String message,
  ) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      final saved = await action();
      final current = items ?? const <BudgetItem>[];
      final index = current.indexWhere((item) => item.id == saved.id);
      items = index < 0
          ? [...current, saved]
          : [
              for (final item in current)
                if (item.id == saved.id) saved else item,
            ];
      successMessage = message;
      unauthorized = false;
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir bütçe yanıtı alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<bool> delete(String id) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await _repository.delete(id);
      items = [
        for (final item in items ?? const <BudgetItem>[])
          if (item.id != id) item,
      ];
      successMessage = 'Bütçe silindi.';
      unauthorized = false;
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

  /// Seçili ayın bir önceki ayındaki bütçeleri bu aya kurar.
  ///
  /// Bütçe aya özeldir ve öyle kalır — kopyalanan yalnız kullanıcının zaten
  /// verdiği karardır (limit ve kapsam). Ayın ilkinde altı kategoriyi altı
  /// forma yeniden yazmak, sınırın kendisinden daha çok emek istiyordu.
  ///
  /// Bu ayda karşılığı olan kategori **atlanır**: tekil indeks onu zaten
  /// reddeder ve kullanıcının bu ay elle değiştirdiği limit ezilmez.
  Future<void> copyFromPreviousMonth() async {
    if (isSubmitting) return;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      final previous = DateTime(selectedMonth.year, selectedMonth.month - 1);
      final source = await _repository.list(previous.year, previous.month);
      final existing = budgetedCategoryIds;
      final copied = <BudgetItem>[];
      for (final item in source) {
        if (existing.contains(item.categoryId)) continue;
        copied.add(
          await _repository.create(
            CreateBudgetInput(
              categoryId: item.categoryId,
              limit: item.limit,
              year: selectedMonth.year,
              month: selectedMonth.month,
              // Kapsamı görmeyen kullanıcıda hiçbir istekte `scope` gitmez;
              // zincir kategorinin varsayılanından çözer ve kopya, kaynağın
              // kapsamını yine aynen alır.
              scope: isScopeVisible ? item.scope : null,
            ),
          ),
        );
      }
      items = [...items ?? const <BudgetItem>[], ...copied];
      unauthorized = false;
      successMessage = copied.isEmpty
          ? 'Geçen aydan kopyalanacak bütçe bulunamadı.'
          : '${copied.length} bütçe geçen aydan kopyalandı.';
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir bütçe yanıtı alındı.';
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<List<BudgetCategory>> loadCategories() =>
      _repository.listExpenseCategories();

  Future<List<BudgetSpendingLine>> loadSpending(BudgetItem budget) =>
      _repository.listSpending(budget);

  void clearMessage() {
    errorMessage = null;
    successMessage = null;
    notifyListeners();
  }

  void _handleFinancialDataChanged() {
    final revision = _changes?.budgetsRevision ?? 0;
    if (revision == _seenBudgetsRevision) return;
    _seenBudgetsRevision = revision;
    load();
  }

  @override
  void dispose() {
    _changes?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }
}
