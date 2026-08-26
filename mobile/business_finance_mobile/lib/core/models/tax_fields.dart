import '../formatters/money_text.dart';

/// Bir kaydın üstünde taşınan KDV bilgisi.
///
/// ADR 0016: taşınan bir bilgidir. Oran ve tutar **iki bağımsız alandır** ve
/// istemci de sunucu da birini diğerinden türetmez. İkisi de boşsa kayıt KDV
/// taşımaz ve istekte `vat` alanları hiç gitmez.
class VatFields {
  const VatFields({this.rate, this.amount});

  /// Sunucu cevabındaki `vat` nesnesi; KDV yoksa `null` gelir.
  static VatFields? fromJson(Object? value) {
    if (value == null) return null;
    if (value is! Map<String, dynamic>) {
      throw FormatException('Invalid vat payload: $value');
    }
    return VatFields(
      rate: value['rate'] as String?,
      amount: value['amount'] as String?,
    );
  }

  /// Belgedeki oran, dört ondalıklı string (0,20 → `"0.2000"`).
  final String? rate;

  /// Belgedeki KDV tutarı, dört ondalıklı string.
  final String? amount;

  bool get isEmpty => rate == null && amount == null;

  /// Ekranda yüzde olarak okunan oran; oran yoksa boştur.
  String? get ratePercentLabel {
    final value = double.tryParse(rate ?? '');
    if (value == null) return null;
    final percent = value * 100;
    final text = percent == percent.roundToDouble()
        ? percent.round().toString()
        : percent.toStringAsFixed(2);
    return '%$text';
  }

  /// Kullanıcının yazdığı tutarı sözleşmenin biçimine çevirir (200 → 200.0000).
  ///
  /// Sunucu dört ondalıklı string bekliyor ve ekran biçimlendiricisi de aynı
  /// biçimi okuyor; ham girdi gönderilseydi tutar ekranda para gibi değil
  /// `200 TRY` diye görünürdü.
  static String? amountFromInput(String? input) {
    final normalized = MoneyText.normalizeInput(input ?? '');
    if (normalized == null) return null;
    return double.parse(normalized).toStringAsFixed(4);
  }

  /// Kullanıcının yazdığı yüzdeyi sözleşmenin oranına çevirir (20 → 0.2000).
  ///
  /// Bu bir **vergi hesabı değildir**: yazılan sayının birimini değiştirir,
  /// tutardan hiçbir şey türetmez.
  static String? rateFromPercentInput(String? input) {
    final normalized = MoneyText.normalizeInput(input ?? '');
    if (normalized == null) return null;
    final percent = double.tryParse(normalized);
    if (percent == null || percent < 0 || percent >= 100) return null;
    return (percent / 100).toStringAsFixed(4);
  }
}
