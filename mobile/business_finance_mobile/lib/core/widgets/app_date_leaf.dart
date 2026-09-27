import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';

/// Takvim yaprağı: gün (17/700) ve ay kısaltması (11/600), 44×48.
///
/// Tarihli satırlarda (son sayımlar, yaklaşanlar) ikon kapsülünün yerini
/// alır. [urgent] vadesi geçmiş kaydı gider zemininde gösterir.
class AppDateLeaf extends StatelessWidget {
  const AppDateLeaf({
    required this.day,
    required this.month,
    super.key,
    this.urgent = false,
  });

  final int day;

  /// Kısa ay adı (`Eyl`).
  final String month;
  final bool urgent;

  static const _shortMonths = [
    'Oca',
    'Şub',
    'Mar',
    'Nis',
    'May',
    'Haz',
    'Tem',
    'Ağu',
    'Eyl',
    'Eki',
    'Kas',
    'Ara',
  ];

  /// `yyyy-MM-dd` biçimli tarihten yaprak kurar.
  factory AppDateLeaf.fromDate(
    DateTime date, {
    bool urgent = false,
    Key? key,
  }) => AppDateLeaf(
    key: key,
    day: date.day,
    month: _shortMonths[date.month - 1],
    urgent: urgent,
  );

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    final background = urgent ? colors.expenseContainer : surfaces.card;
    final border = urgent ? colors.expenseContainer : surfaces.border;
    final strong = urgent ? colors.onExpenseContainer : surfaces.ink;
    final soft = urgent ? colors.onExpenseContainer : surfaces.inkMuted;
    return ExcludeSemantics(
      child: Container(
        width: 44,
        height: 48,
        decoration: BoxDecoration(
          color: background,
          border: Border.all(color: border),
          borderRadius: BorderRadius.circular(AppRadius.field),
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              '$day',
              style: TextStyle(
                fontSize: 17,
                height: 1,
                fontWeight: FontWeight.w700,
                fontFeatures: AppTypography.tabularFigures,
                color: strong,
              ),
            ),
            const SizedBox(height: AppSpacing.xxSmall),
            Text(
              month,
              style: TextStyle(
                fontSize: 11,
                height: 1,
                fontWeight: FontWeight.w600,
                letterSpacing: 0.4,
                color: soft,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
