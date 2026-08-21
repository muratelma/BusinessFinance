/// Ödenmemiş bir faturanın plan formunu önerilerle açması için gereken her şey.
///
/// Fiş sözleşmesinden bilerek bağımsız: planlama ekranı önerilerin bir
/// fotoğraftan mı başka bir yerden mi geldiğini bilmek zorunda değil. Çeviri
/// tek yönlü ve tek yerde (`receipt_prefill.dart`) — `TransferPrefill` ve
/// `LendingPrefill` ile aynı desen.
class BillPrefill {
  const BillPrefill({
    this.amount,
    this.dueDate,
    this.description,
    this.categoryId,
  });

  /// Fatura tutarı; dört ondalıklı string.
  final String? amount;

  /// `yyyy-MM-dd` — planın başlayacağı **ve** biteceği gün. Faturanın kendi
  /// düzenlenme tarihi değil: ikisi karıştırılırsa henüz ödenmemiş bir fatura
  /// düzenlendiği gün harcanmış görünür.
  final String? dueDate;

  final String? description;

  /// Kullanıcının kendi kategorisine çözülmüş kimlik; çözülemediyse `null` ve
  /// alan boş kalır.
  final String? categoryId;

  /// **Ödeme kaynağı taşınmıyor**: fatura hangi hesaptan ödeneceğini söylemez.
  bool get isEmpty => amount == null && dueDate == null;
}
