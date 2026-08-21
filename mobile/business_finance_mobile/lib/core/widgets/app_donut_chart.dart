import 'dart:math' as math;

import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Halka grafiğin bir dilimi.
@immutable
class AppDonutSlice {
  const AppDonutSlice({
    required this.label,
    required this.value,
    required this.color,
    required this.formattedValue,
  });

  final String label;
  final double value;
  final Color color;

  /// Kayıpsız, biçimlenmiş tutar. Efsanede ve ekran okuyucuda bu gösterilir.
  final String formattedValue;
}

/// Bir bütünün paylarını gösteren halka.
///
/// Sütun grafiği bir **zaman serisi** için doğrudur; tek bir dönemin iki
/// parçaya bölünmesi için değil. Altı ayın beşinde veri yokken beş boş sütun
/// çizmek bilgi vermiyordu. Halka, "bu ayın parası nasıl bölündü" sorusunu tek
/// bakışta yanıtlar.
///
/// Renk tek başına anlam taşımaz: her dilim efsanede adı, tutarı ve yüzdesiyle
/// birlikte yazılır.
class AppDonutChart extends StatelessWidget {
  const AppDonutChart({
    required this.slices,
    super.key,
    this.centerLabel,
    this.centerValue,
    this.size = 148,
    this.thickness = 26,
    this.showLegend = true,
  });

  final List<AppDonutSlice> slices;

  /// Halkanın ortasındaki özet (ör. `Net`).
  final String? centerLabel;
  final String? centerValue;

  final double size;
  final double thickness;

  /// Halkanın kendi efsanesini çizip çizmeyeceği.
  ///
  /// Kategori dağılımı efsaneyi kendisi çiziyor: satırlarda renkli nokta
  /// yerine kategori ikonu var ve tutar biçimi çağıranın elinde. İki efsane
  /// birden çizilmesin diye kapatılabiliyor.
  final bool showLegend;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final total = slices.fold<double>(0, (sum, slice) => sum + slice.value);

    final ring = SizedBox.square(
      dimension: size,
      child: CustomPaint(
        painter: _DonutPainter(
          slices: slices,
          total: total,
          thickness: thickness,
          emptyColor: surfaces.cardMuted,
        ),
        child: Center(
          child: Padding(
            padding: EdgeInsets.all(thickness),
            child: FittedBox(
              fit: BoxFit.scaleDown,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  if (centerLabel != null)
                    Text(centerLabel!, style: theme.textTheme.labelSmall),
                  if (centerValue != null)
                    Text(
                      centerValue!,
                      textAlign: TextAlign.center,
                      style: theme.textTheme.titleSmall,
                    ),
                ],
              ),
            ),
          ),
        ),
      ),
    );

    if (!showLegend) return ring;

    return Row(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        ring,
        const SizedBox(width: AppSpacing.medium),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              for (final slice in slices) ...[
                _LegendRow(slice: slice, total: total),
                if (slice != slices.last)
                  const SizedBox(height: AppSpacing.small),
              ],
            ],
          ),
        ),
      ],
    );
  }
}

class _LegendRow extends StatelessWidget {
  const _LegendRow({required this.slice, required this.total});

  final AppDonutSlice slice;
  final double total;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final percent = total <= 0 ? 0 : (slice.value / total * 100).round();

    return Semantics(
      container: true,
      label:
          '${slice.label}: ${slice.formattedValue}, '
          'yüzde $percent',
      child: ExcludeSemantics(
        child: Row(
          children: [
            Container(
              width: 10,
              height: 10,
              decoration: BoxDecoration(
                color: slice.color,
                shape: BoxShape.circle,
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    '${slice.label}  %$percent',
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.labelMedium,
                  ),
                  Text(
                    slice.formattedValue,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.titleSmall,
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _DonutPainter extends CustomPainter {
  const _DonutPainter({
    required this.slices,
    required this.total,
    required this.thickness,
    required this.emptyColor,
  });

  final List<AppDonutSlice> slices;
  final double total;
  final double thickness;
  final Color emptyColor;

  @override
  void paint(Canvas canvas, Size size) {
    final rect = Rect.fromLTWH(
      thickness / 2,
      thickness / 2,
      size.width - thickness,
      size.height - thickness,
    );
    final base = Paint()
      ..style = PaintingStyle.stroke
      ..strokeWidth = thickness
      ..color = emptyColor;

    // Veri yokken bile halka çizilir: boş bir çerçeve, kaybolmuş bir grafikten
    // daha az kafa karıştırıcıdır.
    canvas.drawArc(rect, 0, math.pi * 2, false, base);
    if (total <= 0) return;

    var start = -math.pi / 2;
    for (final slice in slices) {
      if (slice.value <= 0) continue;
      final sweep = (slice.value / total) * math.pi * 2;
      final paint = Paint()
        ..style = PaintingStyle.stroke
        ..strokeWidth = thickness
        ..strokeCap = StrokeCap.butt
        ..color = slice.color;
      canvas.drawArc(rect, start, sweep, false, paint);
      start += sweep;
    }
  }

  @override
  bool shouldRepaint(covariant _DonutPainter oldDelegate) =>
      oldDelegate.total != total ||
      oldDelegate.thickness != thickness ||
      oldDelegate.emptyColor != emptyColor ||
      oldDelegate.slices != slices;
}
