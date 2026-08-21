import 'dart:math' as math;
import 'dart:ui';

/// WCAG 2.1 bağıl parlaklığı (relative luminance).
double relativeLuminance(Color color) {
  double linearize(double channel) {
    return channel <= 0.03928
        ? channel / 12.92
        : math.pow((channel + 0.055) / 1.055, 2.4).toDouble();
  }

  return 0.2126 * linearize(color.r) +
      0.7152 * linearize(color.g) +
      0.0722 * linearize(color.b);
}

/// İki opak renk arasındaki WCAG kontrast oranı (1.0 – 21.0).
double contrastRatio(Color foreground, Color background) {
  final a = relativeLuminance(foreground);
  final b = relativeLuminance(background);
  final lighter = math.max(a, b);
  final darker = math.min(a, b);
  return (lighter + 0.05) / (darker + 0.05);
}

/// Normal boyutlu metin için WCAG AA eşiği.
const double wcagAaNormalText = 4.5;

/// Büyük metin, ikon ve metin dışı öğeler için WCAG AA eşiği.
const double wcagAaLargeText = 3.0;
