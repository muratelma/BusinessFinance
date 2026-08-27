import '../../../core/localization/default_category_labels.dart';
import '../../../core/models/budget_threshold.dart';
import '../../../core/models/transaction_scope.dart';

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
    required this.scope,
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
    // Sunucuda zorunlu: bütçe kapsam **taşır** ve hangi tarafı sınırladığı
    // kendi kapsamından okunur. Boş gelmesi sözleşme kaymasıdır, sessizce
    // "kapsamsız" diye okunmaz.
    scope: TransactionScope.fromApi(json['scope'] as String),
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

  /// Bütçenin sınırladığı taraf. Harcama kategoriyle değil, **kategori +
  /// kapsam çiftiyle** toplanır; öteki tarafın harcaması bu bütçeye girmez.
  final TransactionScope scope;
  final int year;
  final int month;

  bool get isExceeded => !_isZero(exceeded);

  /// Aşılmadı ama eşiği geçti.
  bool get isNearLimit => !isExceeded && progress >= budgetWarningThreshold;

  /// Limitin harcanan yüzdesi; ilerleme çubuğunun aksine **kırpılmaz**.
  int? get spentPercent {
    final limitValue = _scaledAmount(limit);
    final spentValue = _scaledAmount(spent);
    if (limitValue == null || spentValue == null || limitValue <= BigInt.zero) {
      return null;
    }
    return (spentValue.toDouble() / limitValue.toDouble() * 100).round();
  }

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
    this.scope,
  });
  final String categoryId;
  final String limit;
  final int year;
  final int month;

  /// Kullanıcının açık seçimi. Boş bırakılırsa sunucu zinciri yürütür
  /// (kategorinin varsayılanı); çözemezse isteği reddeder ve kapsam uydurmaz.
  final TransactionScope? scope;

  Map<String, Object> toJson() => {
    'categoryId': categoryId,
    'limit': limit,
    'currency': 'TRY',
    if (scope != null) 'scope': scope!.apiValue,
    'year': year,
    'month': month,
  };
}

class BudgetCategory {
  const BudgetCategory({
    required this.id,
    required this.name,
    required this.isActive,
    this.defaultScope,
  });
  final String id;
  final String name;
  final bool isActive;

  /// Kapsam zincirinin bütçedeki tek ara halkası: bütçenin hesabı yoktur.
  final TransactionScope? defaultScope;
}

/// Bütçenin `spent` toplamının arkasındaki tek satır.
///
/// Kanonik feed projection'ının **daraltılmış görünümü**: ikinci bir harcama
/// sorgusu kurulmuyor, aynı okuma modeli kategori + kapsam + ay penceresiyle
/// okunuyor. Satırın taşıdığı alanlar da o kadar — bütçe ekranında sorulan
/// soru "bu 1.500'ü ne yaptı", "bu kaydı nasıl iptal ederim" değil.
class BudgetSpendingLine {
  const BudgetSpendingLine({
    required this.date,
    required this.title,
    required this.amount,
    required this.currency,
    this.sourceName,
  });

  factory BudgetSpendingLine.fromJson(Map<String, dynamic> json) =>
      BudgetSpendingLine(
        date: json['activityDate'] as String,
        title: json['title'] as String,
        amount: json['amount'] as String,
        currency: json['currency'] as String,
        sourceName: json['sourceName'] as String?,
      );

  final String date;
  final String title;
  final String amount;
  final String currency;

  /// Hesap veya kart adı; feed satırının kendi alanı.
  final String? sourceName;
}
