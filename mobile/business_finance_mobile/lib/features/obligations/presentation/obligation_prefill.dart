import '../../../core/models/tax_fields.dart';
import '../../activities/presentation/quick_add_models.dart';
import '../data/obligation_direction.dart';

/// Tek seferlik yükümlülük formuna gelen öneriler.
///
/// Düzenleme tarihi ile vade ayrı alanlardır. Ödeme kaynağı yoktur; para henüz
/// çıkmadığı için hesap seçmek bu aşamada yanlış bir finansal olay üretirdi.
class ObligationPrefill {
  const ObligationPrefill({
    this.direction,
    this.amount,
    this.issueDate,
    this.dueDate,
    this.description,
    this.categoryId,
    this.counterpartyId,
    this.vat,
    this.warnings = const [],
  });

  /// Formu açan yol yönü biliyorsa doludur ve form onu **sormaz**: fişte
  /// "henüz ödemedim" diyen kullanıcı borçlu olduğunu zaten söyledi. Elle
  /// girişte `null` gelir ve yönü kullanıcı seçer.
  final ObligationDirection? direction;

  final QuickAddSuggestion? amount;
  final QuickAddSuggestion? issueDate;
  final QuickAddSuggestion? dueDate;
  final QuickAddSuggestion? description;
  final QuickAddSuggestion? categoryId;
  final String? counterpartyId;

  /// Belgede yazan KDV. Yükümlülük gideri **tanıyan** bir kayıttır ve
  /// ADR 0016'ya göre KDV taşır; okunan bir bilgiyi forma taşımamak onu
  /// kullanıcıya yeniden yazdırmak olurdu.
  final VatFields? vat;

  final List<String> warnings;

  bool get isEmpty =>
      amount == null &&
      issueDate == null &&
      dueDate == null &&
      description == null &&
      categoryId == null &&
      counterpartyId == null;
}
