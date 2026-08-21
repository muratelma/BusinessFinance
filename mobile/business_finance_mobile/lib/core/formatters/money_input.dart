import 'money_text.dart';

/// Kullanıcının yazdığı tutarı okumak ve sunucuya göndermek.
///
/// [MoneyText] gelen parayı ekrana yazar; bu ise girilen parayı geri okur.
/// İkisi ayrı yönler ve ayrı kurallar: gösterimde virgül ve iki basamak,
/// gönderimde nokta ve dört basamak.
class MoneyInput {
  const MoneyInput._();

  /// Girilen metni API sözleşmesinin dört ondalıklı biçimine çevirir.
  static String wire(String value) =>
      (double.tryParse(value.trim().replaceAll(',', '.')) ?? 0).toStringAsFixed(
        4,
      );

  /// Doğrulama için sayısal karşılık; biçim geçersizse `null`.
  static double? parse(String value) {
    final normalized = MoneyText.normalizeInput(value);
    return normalized == null ? null : double.tryParse(normalized);
  }

  /// Sıfırdan büyük tutar isteyen alanların ortak hata metni.
  static String? positiveError(String? value) {
    final amount = parse(value ?? '');
    return amount == null || amount <= 0
        ? 'Sıfırdan büyük geçerli bir tutar girin.'
        : null;
  }
}
