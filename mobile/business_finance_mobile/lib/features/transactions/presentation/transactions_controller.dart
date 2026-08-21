import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/transaction_models.dart';
import '../data/transaction_repository.dart';

class TransactionsController extends ChangeNotifier {
  TransactionsController(this._repository, {this.financialDataChanges})
    : _seenTransactionsRevision =
          financialDataChanges?.transactionsRevision ?? 0 {
    financialDataChanges?.addListener(_handleFinancialDataChanged);
  }
  final TransactionRepositoryContract _repository;
  final FinancialDataChanges? financialDataChanges;
  int _seenTransactionsRevision;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;
  TransactionPage? page;
  TransactionFilter filter = const TransactionFilter();

  Future<void> load({int pageNumber = 1}) async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      page = await _repository.list(pageNumber: pageNumber, filter: filter);
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir işlem yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> applyFilter(TransactionFilter value) async {
    filter = value;
    await load();
  }

  Future<bool> create(CreateTransactionInput input) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await _repository.create(input);
      _announceTransactionsChanged();
      successMessage = 'İşlem kaydedildi.';
      await load();
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir işlem yanıtı alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<bool> cancel(String id) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      final cancelled = await _repository.cancel(id);
      _announceTransactionsChanged();
      final current = page;
      if (current != null) {
        page = TransactionPage(
          items: current.items
              .map((item) => item.id == id ? cancelled : item)
              .toList(growable: false),
          pageNumber: current.pageNumber,
          pageSize: current.pageSize,
          totalCount: current.totalCount,
          totalPages: current.totalPages,
          hasPreviousPage: current.hasPreviousPage,
          hasNextPage: current.hasNextPage,
        );
      }
      successMessage = 'İşlem iptal edildi; geçmiş kaydı korunuyor.';
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

  Future<List<TransactionChoice>> loadAccounts() => _repository.listAccounts();
  Future<List<TransactionChoice>> loadCategories(TransactionKind kind) =>
      _repository.listCategories(kind);

  Future<List<TransactionChoice>> loadFilterCategories(
    TransactionKind? kind,
  ) async {
    if (kind != null) return loadCategories(kind);
    final choices = await Future.wait([
      loadCategories(TransactionKind.income),
      loadCategories(TransactionKind.expense),
    ]);
    return [...choices[0], ...choices[1]];
  }

  void clearMessage() {
    successMessage = null;
    errorMessage = null;
  }

  void _announceTransactionsChanged() {
    final changes = financialDataChanges;
    if (changes == null) return;
    _seenTransactionsRevision = changes.transactionsRevision + 1;
    changes.transactionsChanged();
  }

  void _handleFinancialDataChanged() {
    final revision = financialDataChanges?.transactionsRevision ?? 0;
    if (revision == _seenTransactionsRevision) return;
    _seenTransactionsRevision = revision;
    load();
  }

  @override
  void dispose() {
    financialDataChanges?.removeListener(_handleFinancialDataChanged);
    super.dispose();
  }
}
