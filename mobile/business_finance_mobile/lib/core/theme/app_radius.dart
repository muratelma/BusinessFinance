/// Köşe yarıçapları. Değerler `AppTheme` içinde gömülü sayılardan çıkarıldı;
/// yeni bir yüzey eklenirken buradaki adlardan biri seçilir, yeni sayı
/// uydurulmaz.
abstract final class AppRadius {
  /// Kart, panel ve liste kutusu.
  static const double card = 20;

  /// Form alanı ve buton yüzeyi.
  static const double field = 16;

  /// Bottom sheet ve dialog üst köşeleri.
  static const double sheet = 28;

  /// Kart içindeki küçük yüzey: menü satırı, seçili hücre.
  static const double inner = 12;

  /// Satır içi hata ve küçük etiket kutusu.
  static const double small = 8;

  /// Chip ve etiket: tam yuvarlak uç.
  static const double chip = 999;
}

/// Dokunulan öğelerin boyları. Düğme yüksekliği temadan gelir; bu adlar
/// temayı ezen ya da satır içinde küçülen düğmeler içindir.
abstract final class AppSizes {
  /// Düğme ve en küçük dokunma alanı.
  static const double button = 48;

  /// Menü ve seçici içindeki düğme (`Yeni ekle`).
  static const double menuButton = 40;

  /// Satırın içindeki küçük düğme (`Ödedim`).
  static const double rowButton = 36;
}
