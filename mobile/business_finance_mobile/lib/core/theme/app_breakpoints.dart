import 'package:flutter/widgets.dart';

/// Material 3 pencere boyut sınıfı.
///
/// Uygulama daha önce iki ayrı sabit kullanıyordu (shell 720, planlama 700);
/// aynı kavramın iki eşiği ekranların birbirinden farklı davranmasına yol
/// açıyordu. Kırılım noktası artık tek yerde tanımlıdır.
enum AppWindowSize {
  /// Telefon dikey.
  compact,

  /// Telefon yatay, küçük tablet.
  medium,

  /// Tablet ve daha geniş.
  expanded;

  bool get isCompact => this == AppWindowSize.compact;
  bool get isAtLeastMedium => index >= AppWindowSize.medium.index;
  bool get isExpanded => this == AppWindowSize.expanded;
}

abstract final class AppBreakpoints {
  /// `medium` sınıfının alt sınırı.
  static const double medium = 600;

  /// `expanded` sınıfının alt sınırı.
  static const double expanded = 840;

  /// Geniş ekranda okunabilir metin satırı için içerik genişliği sınırı.
  /// Sınırsız genişlikte liste satırı ekran boyunca uzar ve göz satır başını
  /// kaybeder.
  static const double contentMaxWidth = 720;

  /// Halka grafiğin efsanesinin **yanına** sığdığı en dar kap genişliği.
  ///
  /// Pencere sınıfı değil, bileşen eşiği: kart pencereden dar olabilir ve
  /// karar veren şey kabın kendi genişliğidir. Yine de sayı burada duruyor —
  /// bu dosyanın sözü "kırılım noktası tek yerde tanımlıdır".
  ///
  /// Altında halka ve efsane alt alta geçer; yan yana kalsalardı kategori
  /// adları kesilirdi.
  static const double donutWithLegend = 320;

  static AppWindowSize sizeOfWidth(double width) {
    if (width >= expanded) {
      return AppWindowSize.expanded;
    }
    if (width >= medium) {
      return AppWindowSize.medium;
    }
    return AppWindowSize.compact;
  }
}

extension AppWindowSizeContext on BuildContext {
  /// Pencere boyut sınıfını okur. Yalnız genişliğe abone olur; tam
  /// `MediaQuery` bağımlılığı kurulmaz, böylece klavye açılması gibi
  /// değişiklikler gereksiz rebuild üretmez.
  AppWindowSize get windowSize =>
      AppBreakpoints.sizeOfWidth(MediaQuery.sizeOf(this).width);
}
