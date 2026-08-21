import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';

/// Kullanılabilir genişliğe göre sütun sayısını seçen kart ızgarası.
///
/// Daha önce her ekran kendi eşiğini ve sütun genişliğini elle hesaplıyordu
/// (`constraints.maxWidth >= 700 ? (maxWidth - 16) / 2 : maxWidth`). Sütun
/// sayısı artık kartın okunabilir en küçük genişliğinden türetilir, sabit bir
/// eşikten değil; böylece üç sütun sığan bir ekranda iki sütunda kalınmaz.
class AppResponsiveGrid extends StatelessWidget {
  const AppResponsiveGrid({
    required this.children,
    this.minItemWidth = 320,
    this.spacing = AppSpacing.medium,
    super.key,
  });

  final List<Widget> children;

  /// Bir kartın içeriğinin sıkışmadan sığdığı en küçük genişlik, **varsayılan
  /// metin ölçeğinde**. Gerçek eşik kullanıcının yazı boyutuyla birlikte
  /// büyür: 320 dp'ye sığan bir kart 2.0x ölçekte aynı içeriği sığdıramaz ve
  /// sabit tutulursa tutar satır ortasından bölünür.
  final double minItemWidth;

  final double spacing;

  @visibleForTesting
  static int columnCountFor({
    required double maxWidth,
    required double minItemWidth,
    required double spacing,
  }) {
    final fits = ((maxWidth + spacing) / (minItemWidth + spacing)).floor();
    return fits < 1 ? 1 : fits;
  }

  @override
  Widget build(BuildContext context) {
    if (children.isEmpty) {
      return const SizedBox.shrink();
    }
    final scaledMinItemWidth = MediaQuery.textScalerOf(
      context,
    ).scale(minItemWidth);
    return LayoutBuilder(
      builder: (context, constraints) {
        final columns = columnCountFor(
          maxWidth: constraints.maxWidth,
          minItemWidth: scaledMinItemWidth,
          spacing: spacing,
        );
        final itemWidth =
            (constraints.maxWidth - spacing * (columns - 1)) / columns;
        return Wrap(
          spacing: spacing,
          runSpacing: spacing,
          children: [
            for (final child in children)
              SizedBox(width: itemWidth, child: child),
          ],
        );
      },
    );
  }
}
