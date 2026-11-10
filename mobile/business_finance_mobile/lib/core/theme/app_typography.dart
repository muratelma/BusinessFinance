import 'package:flutter/material.dart';

/// Yazı boyları. Hiçbir ekran `fontSize` için sayı yazmaz; boyun adını seçer
/// ve bir boyu değiştirmek bütün uygulamayı değiştirir. Çoğu metin boyunu
/// zaten tema kademesinden alır ([AppTypography.textTheme]); bu adlar kademenin
/// ağırlığını ya da rengini değil yalnız boyunu isteyen yerler içindir.
///
/// Adlar boyun en sık kullanıldığı yeri söyler, tek kullanım yerini değil.
/// Giriş akışı (`features/auth`) kendi oranlı ölçeğini taşır (`NomivaScale`).
abstract final class AppTextSizes {
  /// Tarih yaprağının ayı. Yaprak 44×48 sabit bir kutudur; gün ve ay ona
  /// göre ölçülüdür ve komşu kademeye çekilince büyük yazıda taşar.
  static const double leafMonth = 11;

  /// Tarih yaprağının günü.
  static const double leafDay = 17;

  /// Grup etiketi ve rozet.
  static const double micro = 12;

  /// Alt satır, yardımcı not, bölüm üstü etiket.
  static const double caption = 13;

  /// Gövde ve yardımcı metin.
  static const double body = 14;

  /// Seçici, menü ve sık satırların yazısı.
  static const double compact = 15;

  /// Liste satırı başlığı, düğme, geniş gövde.
  static const double row = 16;

  /// Bölüm başlığı.
  static const double section = 18;

  /// Kart içi tutar, onay başlığı, içine girilen sayfanın başlığı.
  static const double metric = 22;

  /// Tema ekran başlığı (`headlineSmall`).
  static const double screenTitle = 24;

  /// Ana sekme başlığı.
  static const double pageTitle = 26;

  /// Panelin ve özet kartının büyük tutarı.
  static const double amount = 28;

  /// Sayfanın ana sayısı (Özet'in neti, Kasa bakiyesi, toplam bakiye).
  /// 10 Kasım 2026'da 36'dan 32'ye indi ve Hesaplar'ın 32'siyle birleşti.
  static const double hero = 32;
}

/// Tipografi ölçeği ve para stilleri.
///
/// Yeni font paketi eklenmez: `google_fonts` ağdan indirir, bu uygulama ise
/// yerel ve çevrimdışı çalışacak biçimde kuruldu. Hiyerarşi boyut, ağırlık ve
/// harf aralığıyla kurulur.
abstract final class AppTypography {
  static const List<FontFeature> tabularFigures = [
    FontFeature.tabularFigures(),
  ];

  /// Verilen taban stili para gösterimi için uyarlar: sabit genişlikli
  /// rakam, harf aralıksız. Orantılı rakamlarda tutar sütunu kayar ve
  /// `1.000,00` ile `9.999,99` göz kaydırmadan karşılaştırılamaz.
  static TextStyle money(TextStyle base) =>
      base.copyWith(fontFeatures: tabularFigures, letterSpacing: 0);

  /// Ekranın tepesindeki tek büyük sayı (net durum). Ölçeğin en üstü burada
  /// kullanılır; başka hiçbir yerde bu boyut yoktur.
  static TextStyle heroMoney(TextStyle base) =>
      money(base).copyWith(fontWeight: FontWeight.w700, letterSpacing: -0.8);

  /// Gövde, başlık ve düğme kademeleri harf aralıksızdır (10 Kasım 2026):
  /// boş bırakılan kademe Material'ın aralığını (gövdede 0,25–0,5) devralır,
  /// bu yüzden sıfır açıkça yazılır. Aralık taşıyan yalnız büyük sayı ve
  /// ekran başlığı (sıkı) ile küçük bölüm etiketleridir (açık).
  ///
  /// [faint] verilmezse etiketler yardımcı metinle aynı kademede kalır.
  static TextTheme textTheme(ColorScheme colors, {Color? faint}) {
    final onSurface = colors.onSurface;
    final onSurfaceVariant = colors.onSurfaceVariant;
    // Bölüm etiketi ve sayaç en soluk kademededir: ekranda var olmaları
    // gerekir ama okunmaları zorunlu değildir. Yine de AA eşiğini geçerler.
    final label = faint ?? onSurfaceVariant;

    return TextTheme(
      // Hero metrik.
      displaySmall: TextStyle(
        fontSize: AppTextSizes.hero,
        height: 1.15,
        fontWeight: FontWeight.w700,
        letterSpacing: -0.8,
        color: onSurface,
      ),
      // Ekran başlığı (AppBar).
      headlineSmall: TextStyle(
        fontSize: AppTextSizes.screenTitle,
        height: 1.25,
        fontWeight: FontWeight.w700,
        letterSpacing: -0.4,
        color: onSurface,
      ),
      // Kart içi metrik.
      titleLarge: TextStyle(
        fontSize: AppTextSizes.metric,
        height: 1.3,
        fontWeight: FontWeight.w700,
        letterSpacing: 0,
        color: onSurface,
      ),
      // Bölüm başlığı.
      titleMedium: TextStyle(
        fontSize: AppTextSizes.section,
        height: 1.4,
        fontWeight: FontWeight.w600,
        letterSpacing: 0,
        color: onSurface,
      ),
      // Liste satırı başlığı.
      titleSmall: TextStyle(
        fontSize: AppTextSizes.row,
        height: 1.4,
        fontWeight: FontWeight.w600,
        letterSpacing: 0,
        color: onSurface,
      ),
      bodyLarge: TextStyle(
        fontSize: AppTextSizes.row,
        height: 1.5,
        letterSpacing: 0,
        color: onSurface,
      ),
      bodyMedium: TextStyle(
        fontSize: AppTextSizes.body,
        height: 1.5,
        letterSpacing: 0,
        color: onSurface,
      ),
      // Yardımcı metin: satır altı, tarih, kaynak.
      bodySmall: TextStyle(
        fontSize: AppTextSizes.body,
        height: 1.45,
        letterSpacing: 0,
        color: onSurfaceVariant,
      ),
      labelLarge: const TextStyle(
        fontSize: AppTextSizes.row,
        height: 1.2,
        fontWeight: FontWeight.w600,
        letterSpacing: 0,
      ),
      // Bölüm üstü küçük etiket: büyük harf aralığıyla başlıktan ayrılır.
      labelMedium: TextStyle(
        fontSize: AppTextSizes.caption,
        height: 1.3,
        fontWeight: FontWeight.w600,
        letterSpacing: 0.4,
        color: label,
      ),
      labelSmall: TextStyle(
        fontSize: AppTextSizes.caption,
        height: 1.3,
        fontWeight: FontWeight.w600,
        letterSpacing: 0.3,
        color: label,
      ),
    );
  }
}
