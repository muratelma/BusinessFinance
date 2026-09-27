import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_spacing.dart';
import 'app_status_chip.dart';

/// Dolgusuz durum etiketi: ikon 16 + metin 13/600, rolün metin renginde.
///
/// Liste satırında ve kart köşesinde durum bununla söylenir (Tuttu, Eksik,
/// Yolda, Sayılmadı). Dolgulu kapsül ([AppStatusChip]) yalnız sayfada tek ve
/// önemli bir durum içindir; dar satırlarda kapsül taşıyordu.
class AppStatusTag extends StatelessWidget {
  const AppStatusTag({
    required this.label,
    required this.tone,
    super.key,
    this.icon,
  });

  final String label;
  final IconData? icon;
  final AppStatusTone tone;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final color = switch (tone) {
      AppStatusTone.income => colors.income,
      AppStatusTone.expense => colors.expense,
      AppStatusTone.neutral => colors.neutral,
      AppStatusTone.planned => colors.planned,
      AppStatusTone.cancelled => colors.cancelled,
    };
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (icon != null) ...[
          Icon(icon, size: 16, color: color),
          const SizedBox(width: AppSpacing.xSmall),
        ],
        Flexible(
          child: Text(
            label,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: Theme.of(context).textTheme.labelMedium?.copyWith(
              color: color,
              letterSpacing: 0,
              height: 1.2,
            ),
          ),
        ),
      ],
    );
  }
}
