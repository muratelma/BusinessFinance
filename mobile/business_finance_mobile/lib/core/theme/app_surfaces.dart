import 'package:flutter/material.dart';

/// Yüzey ve kenarlık token'ları.
///
/// Material 3'ün `surfaceContainer*` merdiveni tonal olarak doğru fakat
/// birbirine çok yakın tonlar üretir; kartlar zeminden ancak çok az ayrılır ve
/// ekran "tek düzlem" gibi okunur. Bu uzantı kartı zeminden **kenarlıkla**
/// ayırır: kenarlık tonal farktan daha güvenilirdir, karanlık temada da
/// çalışır ve gölge maliyeti getirmez.
///
/// Gölge yalnız gerçekten yüzen katmanlara (FAB, sheet, dialog) ayrılmıştır;
/// her kartın gölge taşıması derinlik bilgisini anlamsızlaştırır.
@immutable
class AppSurfaces extends ThemeExtension<AppSurfaces> {
  const AppSurfaces({
    required this.canvas,
    required this.card,
    required this.cardMuted,
    required this.border,
    required this.borderStrong,
    required this.overlay,
    required this.ink,
    required this.inkMuted,
    required this.inkFaint,
  });

  /// Aydınlık palet. Yüzeyler **nötr gridir**, mavimsi değil: ekrandaki tek
  /// renkli şeyler para göstergeleri olsun diye.
  static const light = AppSurfaces(
    canvas: Color(0xFFF7F8F9),
    card: Color(0xFFFFFFFF),
    cardMuted: Color(0xFFF0F1F3),
    border: Color(0xFFE5E7EA),
    borderStrong: Color(0xFFCBCFD4),
    overlay: Color(0x14101114),
    ink: Color(0xFF16191D),
    inkMuted: Color(0xFF5C6572),
    inkFaint: Color(0xFF646D7A),
  );

  /// Karanlık palet. Kart zeminden **daha açık**, kenarlık ondan biraz daha
  /// açık: koyu temada derinlik açıklıkla anlatılır, gölgeyle değil.
  static const dark = AppSurfaces(
    canvas: Color(0xFF0D0F12),
    card: Color(0xFF17191D),
    cardMuted: Color(0xFF1F2227),
    border: Color(0xFF282C32),
    borderStrong: Color(0xFF3A3F47),
    overlay: Color(0x40000000),
    ink: Color(0xFFEDEFF2),
    inkMuted: Color(0xFFA5ADB8),
    inkFaint: Color(0xFF8E97A3),
  );

  /// Sayfa zemini.
  final Color canvas;

  /// Kart ve panel zemini.
  final Color card;

  /// Kart içindeki ikincil bölge (ikon kapsülü, ilerleme yolu, girdi alanı).
  final Color cardMuted;

  /// Kartı zeminden ayıran ince kenarlık.
  final Color border;

  /// Vurgulanan kenarlık: seçili kart, odaklı girdi.
  final Color borderStrong;

  /// Yüzen katmanların gölge rengi.
  final Color overlay;

  /// Mürekkep merdiveni: bir ekranda üç okuma kademesi vardır ve hepsi
  /// birden koyu olamaz.
  ///
  /// Önceki palet `ColorScheme.fromSeed`'in ürettiği tonları kullanıyordu;
  /// akromatik seed ile `onSurface` ve `onSurfaceVariant` birbirine çok
  /// yakın çıkıyor (7,05:1 ve 15,6:1 arası bir aralık değil, iki koyu ton) ve
  /// ekran "her yeri siyah" okunuyordu. Üç kademe kasıtlı olarak ayrıldı:
  ///
  /// - [ink] — satır başlığı, tutar, ekran başlığı. Ekranın söylediği şey.
  /// - [inkMuted] — yardımcı satır: tarih, kaynak, açıklama.
  /// - [inkFaint] — bölüm etiketi, yer tutucu, sayaç. Var ama okunması
  ///   zorunlu değil.
  ///
  /// Üçü de AA metin eşiğini geçer; soluk olmak silik olmak değildir.
  final Color ink;
  final Color inkMuted;
  final Color inkFaint;

  static AppSurfaces of(BuildContext context) {
    final surfaces = Theme.of(context).extension<AppSurfaces>();
    if (surfaces == null) {
      throw FlutterError(
        'AppSurfaces tema uzantısı bulunamadı. Tema AppTheme.light() veya '
        'AppTheme.dark() ile kurulmalıdır.',
      );
    }
    return surfaces;
  }

  /// Kartların ortak kenarlığı.
  BorderSide get cardBorder => BorderSide(color: border);

  @override
  AppSurfaces copyWith({
    Color? canvas,
    Color? card,
    Color? cardMuted,
    Color? border,
    Color? borderStrong,
    Color? overlay,
    Color? ink,
    Color? inkMuted,
    Color? inkFaint,
  }) {
    return AppSurfaces(
      canvas: canvas ?? this.canvas,
      card: card ?? this.card,
      cardMuted: cardMuted ?? this.cardMuted,
      border: border ?? this.border,
      borderStrong: borderStrong ?? this.borderStrong,
      overlay: overlay ?? this.overlay,
      ink: ink ?? this.ink,
      inkMuted: inkMuted ?? this.inkMuted,
      inkFaint: inkFaint ?? this.inkFaint,
    );
  }

  @override
  AppSurfaces lerp(covariant AppSurfaces? other, double t) {
    if (other == null) return this;
    return AppSurfaces(
      canvas: Color.lerp(canvas, other.canvas, t)!,
      card: Color.lerp(card, other.card, t)!,
      cardMuted: Color.lerp(cardMuted, other.cardMuted, t)!,
      border: Color.lerp(border, other.border, t)!,
      borderStrong: Color.lerp(borderStrong, other.borderStrong, t)!,
      overlay: Color.lerp(overlay, other.overlay, t)!,
      ink: Color.lerp(ink, other.ink, t)!,
      inkMuted: Color.lerp(inkMuted, other.inkMuted, t)!,
      inkFaint: Color.lerp(inkFaint, other.inkFaint, t)!,
    );
  }
}

/// Yükseklik ölçeği. Değerler ölçek adlarıyla anlatılır ki her ekran kendi
/// gölgesini uydurmasın.
abstract final class AppElevation {
  /// Kart ve panel: gölge yok, ayrım kenarlıkla yapılır.
  static const double flat = 0;

  /// Kayan eylem butonu.
  static const double raised = 3;

  /// Bottom sheet ve dialog.
  static const double floating = 6;
}
