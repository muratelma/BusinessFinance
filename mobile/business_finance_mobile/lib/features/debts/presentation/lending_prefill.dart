/// Bir borç verme kaydının önerilerle açılması için gereken her şey.
///
/// Fiş sözleşmesinden bilerek bağımsız: borç ekranı önerilerin bir fotoğraftan
/// mı başka bir yerden mi geldiğini bilmek zorunda değil. Aradaki çeviri tek
/// yönlü ve tek yerde (`receipt_prefill.dart`), böylece `debts` özelliği
/// `receipts`e bağlanmıyor — `TransferPrefill` ile aynı desen.
class LendingPrefill {
  const LendingPrefill({
    this.counterpartyName,
    this.matchedCounterpartyId,
    this.amount,
    this.date,
    this.feeAmount,
    this.feeDescription,
  });

  /// Dekontta yazan karşı taraf — parayı ödünç alan kişi.
  final String? counterpartyName;

  /// Bu ad kullanıcının defterindeki bir karşı tarafla **eşleştiyse** onun
  /// kimliği; eşleşme yoksa `null`.
  ///
  /// Form bu kimliği sunucuya göndermez — sözleşme karşı tarafı addan bulur ya
  /// da kurar. Taşınmasının sebebi ekranda söylenmesi: dolu gelen bir ad,
  /// kullanıcıya kaydın **var olan** bir kişiye, açık bakiyesiyle birlikte
  /// bağlanacağını söylemeden onaylatılırsa yanlış eşleşme sessizce iki
  /// kişinin hesabını birbirine karıştırır. Model önerir, kullanıcı onaylar
  /// (ADR 0011).
  final String? matchedCounterpartyId;

  /// Verilen tutar; dört ondalıklı string. Anapara **ve** toplam geri ödeme
  /// olarak önerilir: kişiler arası borç tipik olarak faizsizdir ve uydurulmuş
  /// bir faiz, kullanıcının hiç konuşmadığı bir maliyeti deftere yazardı.
  final String? amount;

  /// `yyyy-MM-dd` — paranın çıktığı gün.
  final String? date;

  /// Dekonttaki işlem ücreti; alacak açıldıktan sonra **açılış hesabından**
  /// gider olarak yazılır.
  ///
  /// Alacağa eklenmez: karşı taraf o ücreti sana borçlanmadı, bankaya sen
  /// ödedin. Anaparaya eklemek geri beklediğin tutarı şişirirdi.
  final String? feeAmount;

  /// Ücret kaydının adı.
  final String? feeDescription;

  /// **Hesap taşınmıyor.** Dekont paranın hangi hesaptan çıktığını söylemez;
  /// açılış hesabını kullanıcı seçer.
  bool get isEmpty => amount == null && counterpartyName == null;

  /// Ekranda gösterilecek bir eşleşme var mı.
  bool get hasCounterpartyMatch =>
      matchedCounterpartyId != null &&
      (counterpartyName?.trim().isNotEmpty ?? false);
}
