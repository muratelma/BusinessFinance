import 'package:flutter/material.dart';

import '../formatters/money_text.dart';
import '../theme/app_finance_colors.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Bir dönemin net sonucu.
@immutable
class AppTrendPoint {
  const AppTrendPoint({
    required this.label,
    required this.net,
    required this.hasData,
  });

  final String label;

  /// Kayıpsız string; ekran okuyucuya ve etiketlere olduğu gibi verilir.
  final String net;

  /// O dönemde hiç hareket olmadıysa çubuk çizilmez, yalnız taban çizgisi
  /// kalır. Sıfır yüksekliğinde bir çubuk "sıfır net" ile "veri yok"u aynı
  /// gösteriyordu.
  final bool hasData;
}

/// Aylık net eğilim.
///
/// Önceki tasarım her ay için gelir ve gider sütunlarını yan yana çiziyordu:
/// altı ayın beşinde veri yokken ekranda on boş sütun kalıyor ve grafik
/// bilgiden çok gürültü üretiyordu. Dönem başına **tek net çubuk** kalınca
/// soru da tek oluyor: o ay artıda mı kapandı, ekside mi.
///
/// Çubuklar sıfır çizgisinin üstünde veya altında durur; yön bilgiyi renkten
/// bağımsız da taşır.
class AppTrendChart extends StatelessWidget {
  const AppTrendChart({
    required this.points,
    required this.currency,
    super.key,
    this.height = 120,
  });

  final List<AppTrendPoint> points;
  final String currency;
  final double height;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);

    final withData = points.where((point) => point.hasData).toList();
    if (withData.isEmpty) {
      return Text(
        'Bu dönemde karşılaştırılacak hareket yok.',
        style: theme.textTheme.bodySmall,
      );
    }

    final values = {
      for (final point in points) point.label: double.tryParse(point.net) ?? 0,
    };
    final peak = values.values
        .map((value) => value.abs())
        .fold<double>(0, (max, value) => value > max ? value : max);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          height: height,
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              for (final point in points)
                Expanded(
                  child: Semantics(
                    label: point.hasData
                        ? '${point.label}. Net '
                              '${MoneyText.format(point.net, currency)}'
                        : '${point.label}. Hareket yok',
                    child: ExcludeSemantics(
                      child: Padding(
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppSpacing.xSmall,
                        ),
                        child: _Column(
                          label: point.label,
                          value: values[point.label] ?? 0,
                          peak: peak,
                          hasData: point.hasData,
                          positive: colors.incomeFill,
                          negative: colors.expenseFill,
                          baseline: surfaces.border,
                          labelStyle: theme.textTheme.labelSmall,
                        ),
                      ),
                    ),
                  ),
                ),
            ],
          ),
        ),
        const SizedBox(height: AppSpacing.small),
        Text(
          'Çubuklar ayın netini gösterir: çizginin üstü artı, altı eksi.',
          style: theme.textTheme.bodySmall,
        ),
      ],
    );
  }
}

class _Column extends StatelessWidget {
  const _Column({
    required this.label,
    required this.value,
    required this.peak,
    required this.hasData,
    required this.positive,
    required this.negative,
    required this.baseline,
    required this.labelStyle,
  });

  final String label;
  final double value;
  final double peak;
  final bool hasData;
  final Color positive;
  final Color negative;
  final Color baseline;
  final TextStyle? labelStyle;

  @override
  Widget build(BuildContext context) {
    final ratio = peak <= 0 ? 0.0 : (value.abs() / peak).clamp(0.0, 1.0);
    final isPositive = value >= 0;
    final bar = hasData && ratio > 0
        ? FractionallySizedBox(
            heightFactor: ratio,
            // Genişlik oranı verilmezse kutu çocuğunun genişliğini alır ve
            // `DecoratedBox` boyutsuz olduğu için çubuk sıfır genişlikte
            // çizilirdi: grafik tamamen boş görünüyordu.
            widthFactor: 1,
            alignment: isPositive
                ? Alignment.bottomCenter
                : Alignment.topCenter,
            child: DecoratedBox(
              decoration: BoxDecoration(
                color: isPositive ? positive : negative,
                borderRadius: BorderRadius.vertical(
                  top: Radius.circular(isPositive ? AppRadius.field : 0),
                  bottom: Radius.circular(isPositive ? 0 : AppRadius.field),
                ),
              ),
            ),
          )
        : const SizedBox.shrink();

    return Column(
      children: [
        // Artı ve eksi için eşit yer ayrılır; sıfır çizgisi hep aynı yükseklikte
        // kalır, böylece aylar birbiriyle karşılaştırılabilir. Çubuk yalnız
        // kendi yarısında çizilir.
        Expanded(
          child: Align(
            alignment: Alignment.bottomCenter,
            child: isPositive ? bar : const SizedBox.shrink(),
          ),
        ),
        Container(height: 1, color: baseline),
        Expanded(
          child: Align(
            alignment: Alignment.topCenter,
            child: isPositive ? const SizedBox.shrink() : bar,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Text(label, maxLines: 1, style: labelStyle),
      ],
    );
  }
}
