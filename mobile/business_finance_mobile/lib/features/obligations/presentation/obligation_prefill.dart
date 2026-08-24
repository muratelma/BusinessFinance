import '../../activities/presentation/quick_add_models.dart';

/// Tek seferlik yükümlülük formuna gelen öneriler.
///
/// Düzenleme tarihi ile vade ayrı alanlardır. Ödeme kaynağı yoktur; para henüz
/// çıkmadığı için hesap seçmek bu aşamada yanlış bir finansal olay üretirdi.
class ObligationPrefill {
  const ObligationPrefill({
    this.amount,
    this.issueDate,
    this.dueDate,
    this.description,
    this.categoryId,
    this.counterpartyId,
    this.warnings = const [],
  });

  final QuickAddSuggestion? amount;
  final QuickAddSuggestion? issueDate;
  final QuickAddSuggestion? dueDate;
  final QuickAddSuggestion? description;
  final QuickAddSuggestion? categoryId;
  final String? counterpartyId;
  final List<String> warnings;

  bool get isEmpty =>
      amount == null &&
      issueDate == null &&
      dueDate == null &&
      description == null &&
      categoryId == null &&
      counterpartyId == null;
}
