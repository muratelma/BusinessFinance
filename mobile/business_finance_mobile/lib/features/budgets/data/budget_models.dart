import '../../../core/localization/default_category_labels.dart';

class BudgetItem {
  const BudgetItem({
    required this.id,
    required this.categoryId,
    required this.categoryName,
    required this.limit,
    required this.spent,
    required this.remaining,
    required this.exceeded,
    required this.currency,
    required this.year,
    required this.month,
  });

  factory BudgetItem.fromJson(Map<String, dynamic> json) => BudgetItem(
    id: json['id'] as String,
    categoryId: json['categoryId'] as String,
    categoryName: DefaultCategoryLabels.localized(
      json['categoryName'] as String,
    ),
    limit: json['limit'] as String,
    spent: json['spent'] as String,
    remaining: json['remaining'] as String,
    exceeded: json['exceeded'] as String,
    currency: json['currency'] as String,
    year: json['year'] as int,
    month: json['month'] as int,
  );

  final String id;
  final String categoryId;
  final String categoryName;
  final String limit;
  final String spent;
  final String remaining;
  final String exceeded;
  final String currency;
  final int year;
  final int month;

  bool get isExceeded => !_isZero(exceeded);
  double get progress {
    final limitValue = _scaledAmount(limit);
    final spentValue = _scaledAmount(spent);
    if (limitValue == null || spentValue == null || limitValue <= BigInt.zero) {
      return 0;
    }
    if (spentValue >= limitValue) return 1;
    return spentValue.toDouble() / limitValue.toDouble();
  }

  static bool _isZero(String value) =>
      RegExp(r'^-?0+(\.0+)?$').hasMatch(value.trim());

  static BigInt? _scaledAmount(String value) {
    final match = RegExp(r'^(\d+)\.(\d{4})$').firstMatch(value);
    if (match == null) return null;
    return BigInt.tryParse('${match.group(1)}${match.group(2)}');
  }
}

class CreateBudgetInput {
  const CreateBudgetInput({
    required this.categoryId,
    required this.limit,
    required this.year,
    required this.month,
  });
  final String categoryId;
  final String limit;
  final int year;
  final int month;

  Map<String, Object> toJson() => {
    'categoryId': categoryId,
    'limit': limit,
    'currency': 'TRY',
    'year': year,
    'month': month,
  };
}

class BudgetCategory {
  const BudgetCategory({
    required this.id,
    required this.name,
    required this.isActive,
  });
  final String id;
  final String name;
  final bool isActive;
}
