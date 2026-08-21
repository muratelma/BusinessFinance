import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/finance_models.dart';
import '../data/finance_repository.dart';

/// Which screens a card or transfer mutation can affect. Naming the mutation
/// rather than a vague target keeps the refresh honest: a transfer cannot change
/// a budget, and paying a card is not a second expense.
enum FinanceMutationImpact {
  none,
  transfer,
  cardCharge,
  cardPayment,
  installmentRealized,
}

class FinanceController extends ChangeNotifier {
  FinanceController(this._repository, {this.financialDataChanges});
  final FinanceRepositoryContract _repository;
  final FinancialDataChanges? financialDataChanges;

  FinanceSnapshot? snapshot;
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;

  /// Listelerin dönemi. Varsayılan son üç ay: kart hareketleri ve transferler
  /// her gün büyüyor, tümünü çekmek ilk aylarda görünmez sonra yavaşlar.
  HistoryPeriod period = HistoryPeriod.threeMonths;

  Future<void> selectPeriod(HistoryPeriod value) async {
    if (period == value) return;
    period = value;
    await load();
  }

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      snapshot = await _repository.load(period: period);
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir finans yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> submit(
    Future<void> Function() action,
    String message, {
    FinanceMutationImpact impact = FinanceMutationImpact.none,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await action();
      switch (impact) {
        case FinanceMutationImpact.none:
          break;
        case FinanceMutationImpact.transfer:
          financialDataChanges?.transferChanged();
          break;
        case FinanceMutationImpact.cardCharge:
          financialDataChanges?.cardSpendingChanged();
          break;
        case FinanceMutationImpact.cardPayment:
          financialDataChanges?.cardPaymentChanged();
          break;
        case FinanceMutationImpact.installmentRealized:
          financialDataChanges?.installmentRealized();
          break;
      }
      await load();
      successMessage = message;
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir finans yanıtı alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<CardActivity> loadActivity(String cardId) =>
      _repository.loadActivity(cardId, period: period);
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) => _repository.loadStatement(cardId, year, month, asOf);
  Future<CardStatement?> loadCurrentStatement(String cardId) =>
      _repository.loadCurrentStatement(cardId);
  FinanceRepositoryContract get repository => _repository;

  void clearMessage() {
    errorMessage = null;
    successMessage = null;
    notifyListeners();
  }
}
