import '../data/debt_book_models.dart';
import 'debt_entry_sheet.dart';

/// Yeni kayıt formuna başka bir yoldan gelen öneriler (fiş "henüz ödemedim",
/// dekont "borç verdim"). Hepsi değiştirilebilir; kişi önerisi kişi
/// sayfasından gelen kişi gibi kilitlenmez.
///
/// Belge sözleşmesinden bağımsızdır: çeviri tek yönlü ve tek yerde
/// (`receipt_prefill.dart`); borç ekranı önerinin nereden geldiğini bilmez.
class DebtEntryPrefill {
  const DebtEntryPrefill({
    this.person,
    this.amount,
    this.date,
    this.dueDate,
    this.description,
    this.categoryId,
    this.notices = const [],
    this.feeAmount,
    this.feeDescription,
  });

  /// Belgedeki ad kullanıcının kişilerinden biriyle eşleştiyse o kişi.
  /// Eşleşme yoksa boştur; ad [description]a gider ve kayıt kişisiz
  /// yazılabilir (K10).
  final DebtPersonChoice? person;
  final String? amount;
  final String? date;
  final String? dueDate;
  final String? description;
  final String? categoryId;

  /// Belge okumasının uyarıları; formun başında yazılır.
  final List<String> notices;

  /// Dekonttaki işlem ücreti; kayıt yazıldıktan sonra paranın çıktığı
  /// hesaptan gider olarak yazılır. Borca eklenmez: ücreti karşı taraf değil
  /// banka aldı.
  final String? feeAmount;
  final String? feeDescription;
}

/// Yeni kayıt rotasının (`debtEntryLocation`) isteği: hangi kayıt ve hangi
/// önerilerle.
class DebtEntryRequest {
  const DebtEntryRequest(this.kind, {this.prefill});

  final DebtEntryKind kind;
  final DebtEntryPrefill? prefill;
}
