import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/budget_models.dart';
import '../data/budget_repository.dart';

class BudgetsController extends ChangeNotifier {
  BudgetsController(
    this._repository, {
    FinancialDataChanges? changes,
    DateTime? initialMonth,
  }) : _changes = changes,
       _seenBudgetsRevision = changes?.budgetsRevision ?? 0,
       selectedMonth = DateTime(
         (initialMonth ?? DateTime.now()).year,
         (initialMonth ?? DateTime.now()).month,
       ) {
    _changes?.addListener(_handleFinancialDataChanged);
  }

  final BudgetRepositoryContract _repository;
  final FinancialDataChanges? _changes;
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

  Future<List<BudgetCategory>> loadCategories() =>
      _repository.listExpenseCategories();

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
