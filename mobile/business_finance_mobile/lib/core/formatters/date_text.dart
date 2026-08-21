/// Tarihlerin kullanıcıya gösterilen biçimi.
///
/// API tarihleri `yyyy-MM-dd` gönderir; bu biçim makine için doğru ama ekranda
/// okunmuyor. Ay adları daha önce iki ayrı dosyada kopyalanmıştı; üçüncü kopya
/// yerine ortak yer burası.
///
/// Ay adları burada sabit: uygulama tek dilli ve `intl` yerelleştirmesi henüz
/// kurulu değil. Çok dil geldiğinde değişecek tek yer de burası olur.
class DateText {
  const DateText._();

  static const months = [
    'Ocak',
    'Şubat',
    'Mart',
    'Nisan',
    'Mayıs',
    'Haziran',
    'Temmuz',
    'Ağustos',
    'Eylül',
    'Ekim',
    'Kasım',
    'Aralık',
  ];

  /// `2026-08-10` → `10 Ağustos`. Çözümlenemeyen değer olduğu gibi döner:
  /// bozuk bir tarihi gizlemek, yanlış bir tarihi göstermekten kötüdür.
  static String dayMonth(String isoDate) {
    final parsed = _parse(isoDate);
    if (parsed == null) return isoDate;
    return '${parsed.$3} ${months[parsed.$2 - 1]}';
  }

  /// `2026-08-10` → `10 Ağustos 2026`.
  static String dayMonthYear(String isoDate) {
    final parsed = _parse(isoDate);
    if (parsed == null) return isoDate;
    return '${parsed.$3} ${months[parsed.$2 - 1]} ${parsed.$1}';
  }

  /// `(2026, 8)` → `Ağustos 2026`.
  static String monthYear(int year, int month) =>
      month < 1 || month > 12 ? '$month/$year' : '${months[month - 1]} $year';

  static (int, int, int)? _parse(String isoDate) {
    final parts = isoDate.split('-');
    if (parts.length != 3) return null;
    final year = int.tryParse(parts[0]);
    final month = int.tryParse(parts[1]);
    final day = int.tryParse(parts[2]);
    if (year == null || month == null || day == null) return null;
    if (month < 1 || month > 12) return null;
    return (year, month, day);
  }
}
