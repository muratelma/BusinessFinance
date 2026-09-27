import 'package:flutter/services.dart';

/// Dört ondalıklı para dizeleri üzerinde **tam** aritmetik.
///
/// Uygulama finansal toplamı istemcide hesaplamaz; bu yardımcı yalnız
/// kullanıcının **girdisini** kurmak ve kaydetmeden önce göstermek için
/// vardır: banknot adetlerinden sayılan tutarı toplamak ve sayım panelinde
/// canlı farkı önizlemek. Kaydedilen fark yine sunucudan gelir. Hesap `double`
/// ile yapılmaz; tutar 10⁴ ile ölçeklenmiş tam sayıdır, sunucunun
/// `decimal(19,4)` sonucuyla birebir aynıdır.
abstract final class MoneyMath {
  static final BigInt _scale = BigInt.from(10000);

  /// `1234.5600` → 12345600. Biçim tutmazsa `null`.
  static BigInt? parse(String wire) {
    final match = RegExp(
      r'^(-?)(\d+)(?:\.(\d{1,4}))?$',
    ).firstMatch(wire.trim());
    if (match == null) return null;
    final fraction = (match.group(3) ?? '').padRight(4, '0');
    final value = BigInt.parse('${match.group(2)}$fraction');
    return match.group(1) == '-' ? -value : value;
  }

  /// 12345600 → `1234.5600`.
  static String wire(BigInt scaled) {
    final negative = scaled.isNegative;
    final digits = scaled.abs().toString().padLeft(5, '0');
    final integer = digits.substring(0, digits.length - 4);
    final fraction = digits.substring(digits.length - 4);
    return '${negative ? '-' : ''}$integer.$fraction';
  }

  /// Tam lira tutarı (banknot değeri × adet).
  static BigInt lira(int value) => BigInt.from(value) * _scale;

  /// Kullanıcının yazdığı Türkçe tutar (`23.100,50`, `23100`, `23100.5`)
  /// → dört ondalıklı dize. Tutar okunamazsa `null`.
  static String? fromInput(String input) {
    var text = input.trim().replaceAll(' ', '');
    if (text.isEmpty) return null;
    if (text.contains(',')) {
      // Virgül ondalık ayıracıdır; noktalar binlik ayracı.
      text = text.replaceAll('.', '').replaceAll(',', '.');
    } else if (RegExp(r'^\d{1,3}(\.\d{3})+$').hasMatch(text)) {
      // `23.100` binlik gruplu tam sayıdır, 23,1 değil.
      text = text.replaceAll('.', '');
    }
    final value = parse(text);
    return value == null || value.isNegative ? null : wire(value);
  }
}

/// Tutar alanında Türkçe binlik ayırıcı: yazarken `23100` → `23.100`,
/// ondalık virgülle (`23.100,5`). En fazla dört ondalık.
class TurkishAmountInputFormatter extends TextInputFormatter {
  const TurkishAmountInputFormatter();

  @override
  TextEditingValue formatEditUpdate(
    TextEditingValue oldValue,
    TextEditingValue newValue,
  ) {
    final raw = newValue.text.replaceAll('.', '');
    if (raw.isEmpty) return newValue.copyWith(text: '');
    if (!RegExp(r'^\d*(,\d{0,4})?$').hasMatch(raw)) return oldValue;
    final parts = raw.split(',');
    final integer = parts.first.replaceFirst(RegExp(r'^0+(?=\d)'), '');
    final grouped = StringBuffer();
    for (var i = 0; i < integer.length; i++) {
      if (i > 0 && (integer.length - i) % 3 == 0) grouped.write('.');
      grouped.write(integer[i]);
    }
    final text = parts.length > 1 ? '$grouped,${parts[1]}' : grouped.toString();
    return TextEditingValue(
      text: text,
      selection: TextSelection.collapsed(offset: text.length),
    );
  }
}
