import 'package:flutter/material.dart';

import '../theme/app_breakpoints.dart';

/// Geniş ekranda içeriği okunabilir bir genişliğe sınırlar ve ortalar.
///
/// Sınırsız genişlikte bir liste satırı tablette ekran boyunca uzar; göz satır
/// sonundan satır başına dönerken yerini kaybeder. Telefon genişliğinde
/// (`compact`) hiçbir şey değişmez, sarmalayıcı şeffaftır.
class AppContentWidth extends StatelessWidget {
  const AppContentWidth({
    required this.child,
    this.maxWidth = AppBreakpoints.contentMaxWidth,
    super.key,
  });

  final Widget child;
  final double maxWidth;

  @override
  Widget build(BuildContext context) {
    return Align(
      alignment: Alignment.topCenter,
      child: ConstrainedBox(
        constraints: BoxConstraints(maxWidth: maxWidth),
        child: child,
      ),
    );
  }
}
