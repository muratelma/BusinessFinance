import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../data/category_models.dart';
import '../data/category_repository.dart';

enum CategoriesViewStatus {
  initial,
  loading,
  ready,
  empty,
  error,
  unauthorized,
}

class CategoriesViewModel extends ChangeNotifier {
  CategoriesViewModel(this._repository);

  final CategoryRepository _repository;
  CategoriesViewStatus _status = CategoriesViewStatus.initial;
  List<BudgetCategory> _categories = const [];
  String? _message;
  bool _isSubmitting = false;

  CategoriesViewStatus get status => _status;
  List<BudgetCategory> get categories => _categories;
  String? get message => _message;
  bool get isSubmitting => _isSubmitting;

  Future<void> load() async {
    _status = CategoriesViewStatus.loading;
    _message = null;
    notifyListeners();
    try {
      _categories = await _repository.list();
      _status = _categories.isEmpty
          ? CategoriesViewStatus.empty
          : CategoriesViewStatus.ready;
    } on ApiException catch (error) {
      _setApiError(error);
    } on FormatException {
      _message = 'Kategori verisi beklenen biçimde alınamadı.';
      _status = CategoriesViewStatus.error;
    }
    notifyListeners();
  }

  Future<bool> save({
    BudgetCategory? category,
    required String name,
    required String type,
    required bool isActive,
    TransactionScope? defaultScope,
    bool isTax = false,
  }) async {
    if (_isSubmitting) return false;
    _isSubmitting = true;
    _message = null;
    notifyListeners();
    try {
      final String successMessage;
      if (category == null) {
        await _repository.create(
          name: name,
          type: type,
          defaultScope: defaultScope,
          isTax: isTax,
        );
        successMessage = 'Kategori oluşturuldu.';
      } else {
        await _repository.update(
          id: category.id,
          name: name,
          isActive: isActive,
          defaultScope: defaultScope,
          isTax: isTax,
        );
        successMessage = 'Kategori güncellendi.';
      }
      await load();
      _message = successMessage;
      return true;
    } on ApiException catch (error) {
      // Reddedilen kayıt listeyi bozmaz: yüklenmiş kategoriler yerinde kalır,
      // cümleyi form gösterir. Yalnız oturumun düşmesi ekranı değiştirir.
      _message = error.message;
      if (error.isUnauthorized) _status = CategoriesViewStatus.unauthorized;
      return false;
    } on FormatException {
      _message = 'Kategori verisi beklenen biçimde alınamadı.';
      _status = CategoriesViewStatus.error;
      return false;
    } finally {
      _isSubmitting = false;
      notifyListeners();
    }
  }

  void _setApiError(ApiException error) {
    _message = error.message;
    _status = error.isUnauthorized
        ? CategoriesViewStatus.unauthorized
        : CategoriesViewStatus.error;
  }
}
