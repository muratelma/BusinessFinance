import 'dart:async';
import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/finance_page_models.dart';
import '../data/finance_page_repository.dart';

class TransferController extends ChangeNotifier {
  TransferController(this.repository, {this.changes});
  final FinancePageRepositoryContract repository;
  final FinancialDataChanges? changes;
  AccountsOverviewData? accounts;
  TransferPreviewData? preview;
  TransferInput? input;
  bool isLoading = false;
  bool isPreviewing = false;
  bool isSaving = false;
  bool unauthorized = false;
  String? errorMessage;
  Timer? _timer;
  int _generation = 0;
  bool _disposed = false;

  Future<void> load() async {
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      final next = await repository.accounts();
      if (_disposed) return;
      accounts = next;
      unauthorized = false;
    } on ApiException catch (error) {
      if (_disposed) return;
      errorMessage = error.message;
      unauthorized = error.isUnauthorized;
    } on FormatException {
      if (_disposed) return;
      errorMessage = 'Hesaplar beklenen biçimde alınamadı.';
    } finally {
      if (!_disposed) {
        isLoading = false;
        notifyListeners();
      }
    }
  }

  void setInput(TransferInput? value) {
    if (_disposed || isSaving) return;
    input = value;
    preview = null;
    errorMessage = null;
    isPreviewing = false;
    final generation = ++_generation;
    _timer?.cancel();
    if (value != null) {
      isPreviewing = true;
      _timer = Timer(
        const Duration(milliseconds: 250),
        () => _preview(value, generation),
      );
    }
    notifyListeners();
  }

  Future<void> _preview(TransferInput value, int generation) async {
    try {
      final result = await repository.previewTransfer(value);
      if (_disposed || generation != _generation) return;
      preview = result;
      unauthorized = false;
    } on ApiException catch (error) {
      if (_disposed || generation != _generation) return;
      errorMessage = error.message;
      unauthorized = error.isUnauthorized;
    } on FormatException {
      if (_disposed || generation != _generation) return;
      errorMessage = 'Transfer önizlemesi alınamadı.';
    } finally {
      if (!_disposed && generation == _generation) {
        isPreviewing = false;
        notifyListeners();
      }
    }
  }

  Future<bool> save() async {
    final value = input;
    final approved = preview;
    if (isSaving ||
        value == null ||
        approved == null ||
        approved.needsScope ||
        isPreviewing) {
      return false;
    }
    isSaving = true;
    errorMessage = null;
    notifyListeners();
    try {
      await repository.createTransfer(value);
      if (_disposed) return false;
      changes?.transferChanged(fee: approved.feeAmount != '0.0000');
      return true;
    } on ApiException catch (error) {
      if (_disposed) return false;
      errorMessage = error.message;
      unauthorized = error.isUnauthorized;
      return false;
    } on FormatException {
      if (_disposed) return false;
      errorMessage = 'Transfer kaydedilemedi.';
      return false;
    } finally {
      if (!_disposed) {
        isSaving = false;
        notifyListeners();
      }
    }
  }

  @override
  void dispose() {
    _disposed = true;
    _generation++;
    _timer?.cancel();
    super.dispose();
  }
}
