import 'package:flutter/material.dart';

/// Ortak kesikli kenarlık: kart, soluk filtre ve kapalı ay hücresi.
class AppDashedBorder extends CustomPainter {
  AppDashedBorder(this.color, this.radius);
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
  bool shouldRepaint(AppDashedBorder old) =>
      color != old.color || radius != old.radius;
}
