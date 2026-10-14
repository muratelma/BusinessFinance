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

  static const shortMonths = [
    'Oca',
    'Şub',
    'Mar',
    'Nis',
    'May',
    'Haz',
    'Tem',
    'Ağu',
    'Eyl',
    'Eki',
    'Kas',
    'Ara',
  ];

  static const _monthLocatives = [
    "Ocak'ta",
    "Şubat'ta",
    "Mart'ta",
    "Nisan'da",
    "Mayıs'ta",
    "Haziran'da",
    "Temmuz'da",
    "Ağustos'ta",
    "Eylül'de",
    "Ekim'de",
    "Kasım'da",
    "Aralık'ta",
  ];

  /// `10` → `Ekim'de`. Ek ay adına göre değişir; cümle içinde kurulmaz.
  static String monthLocative(int month) =>
      month < 1 || month > 12 ? '$month. ayda' : _monthLocatives[month - 1];

  /// `2026-10-20` → `20 Eki`. Tek satıra sığması gereken durum yazısı için.
  static String dayShortMonth(String isoDate) {
    final parsed = _parse(isoDate);
    if (parsed == null) return isoDate;
    return '${parsed.$3} ${shortMonths[parsed.$2 - 1]}';
  }

  /// İki günün kısa aralığı: aynı ayda `15–17 Eyl`, ay değişiyorsa
  /// `28 Eyl–2 Eki`, aynı günse `15 Eyl`.
  static String dayRangeShort(String firstIso, String lastIso) {
    final first = _parse(firstIso);
    final last = _parse(lastIso);
    if (first == null || last == null) return '$firstIso–$lastIso';
    if (first == last) return dayShortMonth(firstIso);
    if (first.$1 == last.$1 && first.$2 == last.$2) {
      return '${first.$3}–${last.$3} ${shortMonths[last.$2 - 1]}';
    }
    return '${dayShortMonth(firstIso)}–${dayShortMonth(lastIso)}';
  }

  /// `2026-08-10` → `10 Ağustos`. Çözümlenemeyen değer olduğu gibi döner:
  /// bozuk bir tarihi gizlemek, yanlış bir tarihi göstermekten kötüdür.
  static String dayMonth(String isoDate) {
    final parsed = _parse(isoDate);
    if (parsed == null) return isoDate;
    return '${parsed.$3} ${months[parsed.$2 - 1]}';
  }

  static const weekdays = [
    'Pazartesi',
    'Salı',
    'Çarşamba',
    'Perşembe',
    'Cuma',
    'Cumartesi',
    'Pazar',
  ];

  /// `2026-09-25` → `25 Eylül Perşembe`.
  static String dayMonthWeekday(String isoDate) {
    final parsed = _parse(isoDate);
    if (parsed == null) return isoDate;
    final weekday = DateTime(parsed.$1, parsed.$2, parsed.$3).weekday;
    return '${parsed.$3} ${months[parsed.$2 - 1]} ${weekdays[weekday - 1]}';
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
