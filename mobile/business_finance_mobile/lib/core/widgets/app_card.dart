import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Uygulamanın tek kart yüzeyi.
///
/// Kart zeminden **kenarlıkla** ayrılır, gölgeyle değil: gölge yalnız gerçekten
/// yüzen katmanlara (FAB, sheet, dialog) ayrılmıştır. Her kart gölge taşısaydı
/// derinlik bilgisi anlamını kaybederdi.
class AppCard extends StatelessWidget {
  const AppCard({
    required this.child,
    super.key,
    this.padding = const EdgeInsets.all(AppSpacing.medium),
    this.onTap,
    this.selected = false,
    this.background,
    this.flat = false,
    this.dashed = false,
  });

  final Widget child;
  final EdgeInsetsGeometry padding;
  final VoidCallback? onTap;

  /// Seçili kart kenarlığını vurgular. Seçim yalnız renkle değil, kenarlık
  /// kalınlığıyla da bildirilir.
  final bool selected;

  final Color? background;
  final bool flat;
  final bool dashed;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final scheme = Theme.of(context).colorScheme;
    final radius = flat
        ? BorderRadius.zero
        : BorderRadius.circular(AppRadius.card);

    final content = Padding(padding: padding, child: child);

    return Material(
      color: background ?? surfaces.card,
      borderRadius: radius,
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        borderRadius: radius,
        // Dokunulabilir kart bastırıldığını belli eder; dokunulamayan kart
        // yanlışlıkla tıklanabilir görünmez.
        child: CustomPaint(
          foregroundPainter: dashed
              ? _DashedBorder(surfaces.border, radius)
              : null,
          child: Container(
            decoration: BoxDecoration(
              borderRadius: radius,
              border: flat || dashed
                  ? null
                  : Border.all(
                      color: selected ? scheme.primary : surfaces.border,
                      width: selected ? 2 : 1,
                    ),
            ),
            child: content,
          ),
        ),
      ),
    );
  }
}

class _DashedBorder extends CustomPainter {
  _DashedBorder(this.color, this.radius);
  final Color color;
  final BorderRadius radius;
  @override
  void paint(Canvas canvas, Size size) {
    final path = Path()
      ..addRRect(radius.toRRect(Offset.zero & size).deflate(0.5));
    final paint = Paint()
      ..color = color
      ..style = PaintingStyle.stroke
      ..strokeWidth = 1;
    for (final metric in path.computeMetrics()) {
      for (double start = 0; start < metric.length; start += 9) {
        canvas.drawPath(metric.extractPath(start, start + 5), paint);
      }
    }
  }

  @override
  bool shouldRepaint(_DashedBorder old) =>
      color != old.color || radius != old.radius;
}
