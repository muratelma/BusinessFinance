import 'package:flutter/material.dart';

/// Tipografi ölçeği ve para stilleri.
///
/// Yeni font paketi eklenmez: `google_fonts` ağdan indirir, bu uygulama ise
/// yerel ve çevrimdışı çalışacak biçimde kuruldu. Hiyerarşi boyut, ağırlık ve
/// harf aralığıyla kurulur.
abstract final class AppTypography {
  static const List<FontFeature> tabularFigures = [
    FontFeature.tabularFigures(),
  ];

  /// Verilen taban stili para gösterimi için uyarlar: sabit genişlikli rakam
  /// ve hafif sıkı harf aralığı. Orantılı rakamlarda tutar sütunu kayar ve
  /// `1.000,00` ile `9.999,99` göz kaydırmadan karşılaştırılamaz.
  static TextStyle money(TextStyle base) =>
      base.copyWith(fontFeatures: tabularFigures, letterSpacing: -0.2);

  /// Ekranın tepesindeki tek büyük sayı (net durum). Ölçeğin en üstü burada
  /// kullanılır; başka hiçbir yerde bu boyut yoktur.
  static TextStyle heroMoney(TextStyle base) =>
      money(base).copyWith(fontWeight: FontWeight.w700, letterSpacing: -0.8);

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
        fontSize: 34,
        height: 1.15,
        fontWeight: FontWeight.w700,
        letterSpacing: -0.8,
        color: onSurface,
      ),
      // Ekran başlığı (AppBar).
      headlineSmall: TextStyle(
        fontSize: 24,
        height: 1.25,
        fontWeight: FontWeight.w700,
        letterSpacing: -0.4,
        color: onSurface,
      ),
      // Kart içi metrik.
      titleLarge: TextStyle(
        fontSize: 20,
        height: 1.3,
        fontWeight: FontWeight.w600,
        letterSpacing: -0.2,
        color: onSurface,
      ),
      // Bölüm başlığı.
      titleMedium: TextStyle(
        fontSize: 16,
        height: 1.4,
        fontWeight: FontWeight.w600,
        color: onSurface,
      ),
      // Liste satırı başlığı.
      titleSmall: TextStyle(
        fontSize: 15,
        height: 1.4,
        fontWeight: FontWeight.w600,
        color: onSurface,
      ),
      bodyLarge: TextStyle(fontSize: 16, height: 1.5, color: onSurface),
      bodyMedium: TextStyle(fontSize: 14, height: 1.5, color: onSurface),
      // Yardımcı metin: satır altı, tarih, kaynak.
      bodySmall: TextStyle(fontSize: 13, height: 1.45, color: onSurfaceVariant),
      labelLarge: const TextStyle(
        fontSize: 15,
        height: 1.2,
        fontWeight: FontWeight.w600,
      ),
      // Bölüm üstü küçük etiket: büyük harf aralığıyla başlıktan ayrılır.
      labelMedium: TextStyle(
        fontSize: 12,
        height: 1.3,
        fontWeight: FontWeight.w600,
        letterSpacing: 0.4,
        color: label,
      ),
      labelSmall: TextStyle(
        fontSize: 11,
        height: 1.3,
        fontWeight: FontWeight.w600,
        letterSpacing: 0.3,
        color: label,
      ),
    );
  }
}
