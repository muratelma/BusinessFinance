import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../data/category_models.dart';
import '../data/category_repository.dart';
import 'category_form_page.dart';

/// Formdaki `+ "…" ekle`: kategori formu doğrudan açılır (Kategoriler
/// sayfası araya girmez).
const categoryCreateLocation = '/more/categories/new';

/// Kategori seçicideki `+ "…" ekle`. Form adı ve türü dolu açar; kaydedilirse
/// yeni kategori döner ve açıldığı form onu seçer. Vazgeçilirse `null`.
/// [type] `expense` ya da `income`dır.
Future<BudgetCategory?> addCategoryFromForm(
  BuildContext context, {
  required String type,
  String name = '',
}) async {
  final uri = Uri(
    path: categoryCreateLocation,
    queryParameters: {'type': type, if (name.isNotEmpty) 'name': name},
  );
  final result = await context.push<Object?>(uri.toString());
  return result is BudgetCategory ? result : null;
}

/// Kayıttan sonra yeni kategoriyle kapanan ekleme formu.
class CategoryCreatePage extends StatefulWidget {
  const CategoryCreatePage({
    required this.repository,
    super.key,
    this.initialName,
    this.initialType,
  });

  final CategoryRepository repository;
  final String? initialName;
  final String? initialType;

  @override
  State<CategoryCreatePage> createState() => _CategoryCreatePageState();
}

class _CategoryCreatePageState extends State<CategoryCreatePage> {
  BudgetCategory? _created;
  String? _error;

  Future<bool> _save({
    BudgetCategory? category,
    required String name,
    required String type,
    required bool isActive,
    TransactionScope? defaultScope,
    bool isTax = false,
  }) async {
    try {
      _created = await widget.repository.create(
        name: name,
        type: type,
        defaultScope: defaultScope,
        isTax: isTax,
      );
      return true;
    } on FormatException {
      _error = 'Kategori verisi beklenen biçimde alınamadı.';
      return false;
    }
  }

  @override
  Widget build(BuildContext context) => CategoryFormPage(
    onSave: _save,
    readError: () => _error,
    initialName: widget.initialName,
    initialType: widget.initialType,
    savedResult: () => _created,
  );
}

/// Listesi dışarıdan gelen formun eklediği kategori.
extension AddedCategoryChoice on BudgetCategory {
  DataChoice get choice =>
      DataChoice(id, name, type: type, defaultScope: defaultScope);
}
