/// Bir transfer formunun önerilerle açılması için gereken her şey.
///
/// Fiş sözleşmesinden bilerek bağımsız: transfer ekranı önerilerin bir
/// fotoğraftan mı başka bir yerden mi geldiğini bilmek zorunda değil. Aradaki
/// çeviri tek yönlü ve tek yerde (`receipt_prefill.dart`), böylece `cards`
/// özelliği `receipts`e bağlanmıyor.
class TransferPrefill {
  const TransferPrefill({
    this.amount,
    this.date,
    this.description,
    this.feeAmount,
    this.feeDescription,
  });

  /// Taşınan tutar; dört ondalıklı string.
  final String? amount;

  /// `yyyy-MM-dd`.
  final String? date;

  /// Dekontta yazan karşı taraf.
  final String? description;

  /// İşlem ücreti — taşınan tutara **eklenmez**.
  ///
  /// Transfer parayı taşır, harcamaz; bu belgede gerçekten harcanan tek şey
  /// budur ve kendi gider kaydını hak eder. Bu yüzden ayrı taşınıyor: transfer
  /// kaydedildikten sonra transferin **kaynak hesabından** yazılır.
  final String? feeAmount;

  /// Ücret kaydının adı.
  final String? feeDescription;

  /// Kaynak ve hedef **bilerek yok.** Dekont hangi hesabın kullanıldığını
  /// söylemez; iki ucu da kullanıcı seçer.
  bool get isEmpty => amount == null && date == null && description == null;
}

/// Bir kart ödemesi formunun önerilerle açılması için gereken her şey.
///
/// **Kart taşınmıyor.** Dekont hangi kartın borcunun kapatıldığını söylemez ve
/// yanlış kart, başka bir kartın borcunu azaltır; kartı kullanıcı seçer.
///
/// Kart ödemesi gider **üretmez**: harcamalar kart harcaması olarak zaten
/// sayıldı, ödeme onların ikinci kez sayılması olurdu.
class CardPaymentPrefill {
  const CardPaymentPrefill({
    this.amount,
    this.date,
    this.description,
    this.feeAmount,
    this.feeDescription,
  });

  /// Ödenen tutar; dört ondalıklı string.
  final String? amount;

  /// `yyyy-MM-dd`.
  final String? date;

  /// Kaydın adı: dekontta okunan karşı taraf.
  ///
  /// Taşınmasının bir sebebi daha var: aynı dekontun ikinci kez okutulduğunu
  /// söyleyen uyarı tarih, tutar **ve** adın üçüne birden bakıyor. Ad boş
  /// kalırsa kart ödemesi yolunda o uyarı hiç çalışmaz.
  final String? description;

  /// Dekonttaki işlem ücreti; ödeme yazıldıktan sonra **ödeme hesabından**
  /// gider olarak yazılır. Kart borcuna eklenmez: bankanın aldığı parayı
  /// karta borç göstermek olurdu.
  final String? feeAmount;

  /// Ücret kaydının adı.
  final String? feeDescription;

  bool get isEmpty => amount == null && date == null;
}

/// Taksitli bir fişten gelen plan önerileri.
///
/// **Kart taşınmıyor**: fiş hangi karttan ödendiğini söylemez — üstünde
/// "KREDİ KARTI" yazsa bile hangi kart olduğunu yazmaz — ve yanlış kart başka
/// bir kartın limitini ve borcunu bozar. Kartı kullanıcı seçer.
///
/// Plan **yalnız niyettir**: yalnız gerçekleşen taksit kart harcaması ve gider
/// üretir. Taksitli fişi tek seferlik tam tutar gideri yazmak, o ayın
/// bütçesini gerçekte çıkmayan tutar kadar şişirirdi.
class InstallmentPrefill {
  const InstallmentPrefill({
    this.totalAmount,
    this.installmentCount,
    this.firstInstallmentDate,
    this.description,
    this.categoryId,
  });

  /// Fişin tamamı; dört ondalıklı string. Taksit tutarı **hesaplanmıyor** —
  /// bölmeyi sunucu yapar.
  final String? totalAmount;

  final int? installmentCount;

  /// `yyyy-MM-dd`.
  final String? firstInstallmentDate;
  final String? description;
  final String? categoryId;

  bool get isEmpty => totalAmount == null || installmentCount == null;
}
