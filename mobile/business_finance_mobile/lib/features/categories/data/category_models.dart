import '../../../core/localization/default_category_labels.dart';

class BudgetCategory {
  const BudgetCategory({
    required this.id,
    required this.name,
    required this.type,
    required this.isActive,
  });

  final String id;
  final String name;
  final String type;
  final bool isActive;

  factory BudgetCategory.fromJson(Map<String, dynamic> json) => BudgetCategory(
    id: _readString(json, 'id'),
    name: DefaultCategoryLabels.localized(_readString(json, 'name')),
    type: _readString(json, 'type'),
    isActive: _readBool(json, 'isActive'),
  );
}

class CategoryList {
  const CategoryList(this.items);

  final List<BudgetCategory> items;

  factory CategoryList.fromJson(Map<String, dynamic> json) {
    final rawItems = json['items'];
    if (rawItems is! List) {
      throw const FormatException('Category list response is invalid.');
    }
    return CategoryList(
      rawItems
          .map((item) {
            if (item is! Map<String, dynamic>) {
              throw const FormatException('Category item is invalid.');
            }
            return BudgetCategory.fromJson(item);
          })
          .toList(growable: false),
    );
  }
}

String _readString(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value is String) return value;
  throw FormatException('$key must be a string.');
}

bool _readBool(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value is bool) return value;
  throw FormatException('$key must be a boolean.');
}
