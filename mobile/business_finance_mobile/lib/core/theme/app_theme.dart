import 'package:flutter/material.dart';

import 'app_finance_colors.dart';
import 'app_radius.dart';
import 'app_spacing.dart';
import 'app_surfaces.dart';
import 'app_typography.dart';

abstract final class AppTheme {
  /// Marka rengi **akromatiktir**: neredeyse siyah bir antrasit.
  ///
  /// Bu, iki adımda varılan bir karar. Önce seed yeşildi ve `primary` ile
  /// gelir yeşili aynı aileden görünüyordu; lacivert yaparak yeşili gelire
  /// iade ettim. Ama lacivert de üçüncü bir doygun renk ailesi ekledi:
  /// ekranda lacivert buton, yeşil gelir ve kırmızı gider aynı anda yarışıyor,
  /// hiçbiri diğerine göre öne çıkmıyordu.
  ///
  /// Akromatik marka rengiyle ekranda **anlamlı renk yalnız ikidir**: yeşil
  /// gelir, kırmızı gider. Renk artık dekorasyon değil, yalnız bilgi taşır.
  static const Color _brand = Color(0xFF16191D);
  static const Color _brandDark = Color(0xFFEDEFF2);

  static ThemeData light() => _build(Brightness.light);

  static ThemeData dark() => _build(Brightness.dark);

  static ThemeData _build(Brightness brightness) {
    final isDark = brightness == Brightness.dark;
    final surfaces = isDark ? AppSurfaces.dark : AppSurfaces.light;
    final financeColors = isDark
        ? AppFinanceColors.dark
        : AppFinanceColors.light;

    final base = ColorScheme.fromSeed(
      seedColor: isDark ? _brandDark : _brand,
      brightness: brightness,
    );
    // Yüzeyleri tonal merdivenden değil kendi paletimizden veriyoruz; M3'ün
    // `surfaceContainer*` tonları birbirine çok yakın ve kart zeminden ayırt
    // edilemiyordu.
    final colorScheme = base.copyWith(
      // Marka rolleri elle veriliyor: tonal üretici akromatik seedden bile
      // hafif renkli tonlar çıkarıyor ve o tonlar gelir/gider renkleriyle
      // yarışıyordu.
      primary: isDark ? _brandDark : _brand,
      onPrimary: isDark ? _brand : const Color(0xFFFFFFFF),
      primaryContainer: isDark
          ? const Color(0xFF2A2E34)
          : const Color(0xFFE4E6E9),
      onPrimaryContainer: isDark ? _brandDark : _brand,
      secondary: isDark ? _brandDark : _brand,
      onSecondary: isDark ? _brand : const Color(0xFFFFFFFF),
      secondaryContainer: isDark
          ? const Color(0xFF2A2E34)
          : const Color(0xFFE4E6E9),
      onSecondaryContainer: isDark ? _brandDark : _brand,
      surface: surfaces.canvas,
      // Mürekkep rolleri de kendi merdivenimizden geliyor. Akromatik seedden
      // türeyen `onSurface`/`onSurfaceVariant` birbirine çok yakın iki koyu
      // ton çıkarıyor ve ekran baştan sona siyah okunuyordu.
      onSurface: surfaces.ink,
      onSurfaceVariant: surfaces.inkMuted,
      surfaceContainerLowest: surfaces.card,
      surfaceContainerLow: surfaces.card,
      surfaceContainer: surfaces.cardMuted,
      surfaceContainerHigh: surfaces.cardMuted,
      surfaceContainerHighest: surfaces.cardMuted,
      outlineVariant: surfaces.border,
    );

    final textTheme = AppTypography.textTheme(
      colorScheme,
      faint: surfaces.inkFaint,
    );

    return ThemeData(
      useMaterial3: true,
      brightness: brightness,
      colorScheme: colorScheme,
      textTheme: textTheme,
      extensions: [financeColors, surfaces],
      scaffoldBackgroundColor: surfaces.canvas,
      dividerTheme: DividerThemeData(
        color: surfaces.border,
        thickness: 1,
        space: 1,
      ),
      appBarTheme: AppBarTheme(
        centerTitle: false,
        backgroundColor: surfaces.canvas,
        foregroundColor: colorScheme.onSurface,
        surfaceTintColor: Colors.transparent,
        scrolledUnderElevation: 0,
        titleTextStyle: textTheme.headlineSmall,
      ),
      cardTheme: CardThemeData(
        elevation: AppElevation.flat,
        color: surfaces.card,
        surfaceTintColor: Colors.transparent,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.card),
          side: surfaces.cardBorder,
        ),
      ),
      listTileTheme: ListTileThemeData(
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.xSmall,
        ),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.field),
        ),
        titleTextStyle: textTheme.titleMedium,
        subtitleTextStyle: textTheme.bodySmall,
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: surfaces.cardMuted,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.medium,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.field),
          borderSide: BorderSide(color: surfaces.border),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.field),
          borderSide: BorderSide(color: surfaces.border),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.field),
          borderSide: BorderSide(color: colorScheme.primary, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.field),
          borderSide: BorderSide(color: colorScheme.error),
        ),
        labelStyle: textTheme.bodyMedium,
        helperStyle: textTheme.bodySmall,
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          // 48 dp: dokunma hedefi eşiği butonun kendi yüksekliğinden gelir,
          // görünmez bir dolgudan değil.
          minimumSize: const Size(0, 48),
          padding: const EdgeInsets.symmetric(horizontal: AppSpacing.large),
          textStyle: textTheme.labelLarge,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          minimumSize: const Size(0, 48),
          padding: const EdgeInsets.symmetric(horizontal: AppSpacing.large),
          textStyle: textTheme.labelLarge,
          side: BorderSide(color: surfaces.borderStrong),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          minimumSize: const Size(0, 48),
          textStyle: textTheme.labelLarge,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
        ),
      ),
      floatingActionButtonTheme: FloatingActionButtonThemeData(
        elevation: AppElevation.raised,
        backgroundColor: colorScheme.primary,
        foregroundColor: colorScheme.onPrimary,
        shape: const CircleBorder(),
        extendedTextStyle: textTheme.labelLarge,
        extendedPadding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.large,
        ),
      ),
      chipTheme: ChipThemeData(
        backgroundColor: surfaces.card,
        // Seçili zemin `primary` olsaydı etiket rengi onunla birlikte
        // değişmediği için okunamazdı (ölçülen 1,45:1). `primaryContainer`
        // hem seçiliyi belli eder hem `onSurface` etiketi okunur bırakır.
        selectedColor: colorScheme.primaryContainer,
        checkmarkColor: colorScheme.onSurface,
        side: BorderSide(color: surfaces.border),
        labelStyle: textTheme.labelMedium?.copyWith(
          color: colorScheme.onSurface,
        ),
        secondaryLabelStyle: textTheme.labelMedium?.copyWith(
          color: colorScheme.onSurface,
        ),
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.small,
          vertical: AppSpacing.small,
        ),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.chip),
        ),
      ),
      bottomSheetTheme: BottomSheetThemeData(
        backgroundColor: surfaces.card,
        surfaceTintColor: Colors.transparent,
        elevation: AppElevation.floating,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(AppRadius.sheet),
          ),
        ),
      ),
      dialogTheme: DialogThemeData(
        backgroundColor: surfaces.card,
        surfaceTintColor: Colors.transparent,
        elevation: AppElevation.floating,
        titleTextStyle: textTheme.titleLarge,
        contentTextStyle: textTheme.bodyMedium,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.sheet),
        ),
      ),
      navigationBarTheme: NavigationBarThemeData(
        backgroundColor: surfaces.card,
        surfaceTintColor: Colors.transparent,
        elevation: AppElevation.flat,
        height: 68,
        labelBehavior: NavigationDestinationLabelBehavior.alwaysShow,
        indicatorColor: colorScheme.primaryContainer,
        labelTextStyle: WidgetStateProperty.resolveWith(
          (states) => states.contains(WidgetState.selected)
              ? textTheme.labelMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                  color: colorScheme.onSurface,
                )
              : textTheme.labelMedium?.copyWith(
                  color: colorScheme.onSurfaceVariant,
                ),
        ),
      ),
      navigationRailTheme: NavigationRailThemeData(
        backgroundColor: surfaces.card,
        indicatorColor: colorScheme.primaryContainer,
        useIndicator: true,
        selectedLabelTextStyle: textTheme.labelMedium?.copyWith(
          fontWeight: FontWeight.w600,
          color: colorScheme.onSurface,
        ),
        unselectedLabelTextStyle: textTheme.labelMedium?.copyWith(
          color: colorScheme.onSurfaceVariant,
        ),
      ),
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.field),
        ),
      ),
      progressIndicatorTheme: ProgressIndicatorThemeData(
        linearTrackColor: surfaces.cardMuted,
        circularTrackColor: surfaces.cardMuted,
      ),
    );
  }
}
