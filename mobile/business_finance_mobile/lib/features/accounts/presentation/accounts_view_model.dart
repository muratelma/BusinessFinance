import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/account_models.dart';
import '../data/account_repository.dart';

enum AccountsViewStatus { initial, loading, ready, empty, error, unauthorized }

class AccountsViewModel extends ChangeNotifier {
  AccountsViewModel(this._repository, {this.financialDataChanges});

  final AccountRepository _repository;
  final FinancialDataChanges? financialDataChanges;
  AccountsViewStatus _status = AccountsViewStatus.initial;
  List<Account> _accounts = const [];
  String? _message;
  bool _isSubmitting = false;

  AccountsViewStatus get status => _status;
  List<Account> get accounts => _accounts;
  String? get message => _message;
  bool get isSubmitting => _isSubmitting;

  Future<void> load() async {
    _status = AccountsViewStatus.loading;
    _message = null;
    notifyListeners();
    try {
      final page = await _repository.list();
      _accounts = page.items;
      _status = _accounts.isEmpty
          ? AccountsViewStatus.empty
          : AccountsViewStatus.ready;
    } on ApiException catch (error) {
      _setApiError(error);
    } on FormatException {
      _message = 'Hesap verisi beklenen biçimde alınamadı.';
      _status = AccountsViewStatus.error;
    }
    notifyListeners();
  }

  Future<bool> save({
    Account? account,
    required String name,
    required String type,
    required String openingBalance,
    required bool isActive,
    TransactionScope? defaultScope,
  }) async {
    if (_isSubmitting) return false;
    _isSubmitting = true;
    _message = null;
    notifyListeners();
    try {
      final String successMessage;
      if (account == null) {
        await _repository.create(
          name: name,
          type: type,
          openingBalance: openingBalance,
          defaultScope: defaultScope,
        );
        successMessage = 'Hesap oluşturuldu.';
      } else {
        await _repository.update(
          id: account.id,
          name: name,
          isActive: isActive,
          defaultScope: defaultScope,
        );
        successMessage = 'Hesap güncellendi.';
      }
      financialDataChanges?.accountsChanged();
      await load();
      _message = successMessage;
      return true;
    } on ApiException catch (error) {
      _setApiError(error);
      return false;
    } on FormatException {
      _message = 'Hesap verisi beklenen biçimde alınamadı.';
      _status = AccountsViewStatus.error;
      return false;
    } finally {
      _isSubmitting = false;
      notifyListeners();
    }
  }

  Future<String?> delete(Account account) async {
    if (_isSubmitting) return 'Başka bir hesap işlemi devam ediyor.';
    _isSubmitting = true;
    _message = null;
    notifyListeners();
    try {
      await _repository.delete(account.id);
      financialDataChanges?.accountsChanged();
      await load();
      _message = 'Hesap silindi.';
      return null;
    } on ApiException catch (error) {
      _setApiError(error);
      return error.message;
    } on FormatException {
      const message = 'Hesap verisi beklenen biçimde alınamadı.';
      _message = message;
      _status = AccountsViewStatus.error;
      return message;
    } finally {
      _isSubmitting = false;
      notifyListeners();
    }
  }

  void _setApiError(ApiException error) {
    _message = error.message;
    _status = error.isUnauthorized
        ? AccountsViewStatus.unauthorized
        : AccountsViewStatus.error;
  }
}
