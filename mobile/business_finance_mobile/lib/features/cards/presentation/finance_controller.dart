import 'dart:async';

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

  /// Kartların kesilmiş son ekstresi; kartın yüzündeki limit çubuğu dönem
  /// borcunu bundan çizer. Kart listesinden sonra, beklemeden okunur.
  final Map<String, CardStatement?> statements = {};
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;

  /// Listelerin dönemi. Varsayılan son üç ay: kart hareketleri ve transferler
  /// her gün büyüyor, tümünü çekmek ilk aylarda görünmez sonra yavaşlar.
  HistoryPeriod period = HistoryPeriod.threeMonths;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      snapshot = await _repository.load(period: period);
      unauthorized = false;
      unawaited(_loadStatements(snapshot!.cards));
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

  Future<void> _loadStatements(List<CreditCardItem> cards) async {
    final values = await Future.wait([
      for (final card in cards)
        Future.sync(
          () => _repository.loadCurrentStatement(card.id),
        ).then<CardStatement?>((value) => value).catchError((Object _) => null),
    ]);
    if (_disposed) return;
    for (var i = 0; i < cards.length; i++) {
      statements[cards[i].id] = values[i];
    }
    notifyListeners();
  }

  bool _disposed = false;

  @override
  void dispose() {
    _disposed = true;
    super.dispose();
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

  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) => _repository.loadStatement(cardId, year, month, asOf);
  Future<CardStatement?> loadCurrentStatement(String cardId) =>
      _repository.loadCurrentStatement(cardId);
  FinanceRepositoryContract get repository => _repository;

  /// Başarı cümlesini bir kez verir ve siler. Sayfa onu kendiliğinden kapanan
  /// bir bildirimle söyler; hata cümlesi ise şeritte kapatılana kadar durur.
  String? takeSuccessMessage() {
    final message = successMessage;
    successMessage = null;
    return message;
  }

  void clearMessage() {
    errorMessage = null;
    successMessage = null;
    notifyListeners();
  }

  /// Formun içinde gösterilecek ret cümlesi; kayıt yazıldıysa `null`.
  ///
  /// Cümleyi form söylediği için sayfanın üstündeki şeride bırakılmaz: form
  /// kapandıktan sonra orada durursa eski bir reddi yeniymiş gibi gösterir.
  Future<String?> refusalOf(Future<bool> write) async {
    if (await write) return null;
    final message = errorMessage ?? 'Kaydedilemedi. Tekrar deneyin.';
    errorMessage = null;
    notifyListeners();
    return message;
  }
}
