import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_surfaces.dart';
import 'app_status_chip.dart';

/// Satırın başındaki yuvarlak ikon kapsülü.
///
/// Zemin rolün `*Container` tonu, ikon `on*Container` tonudur; rol yoksa gri
/// zemin ve mürekkep rengi. 40 dp kapsülde ikon 20, daha küçüklerde 18 dp.
/// Kart içi satır listelerinde ayırıcı yazının başladığı yerden çizilir:
/// 16 kenar + 40 kapsül + 16 boşluk = [rowInset].
class AppIconCapsule extends StatelessWidget {
  const AppIconCapsule({
    required this.icon,
    super.key,
    this.tone,
    this.size = 40,
    this.iconColor,
  });

  /// Kapsüllü satırda ayırıcının sol boşluğu.
  static const double rowInset = 72;

  final IconData icon;
  final AppStatusTone? tone;
  final double size;

  /// Gri kapsülde ikonu kategori rengiyle boyamak için.
  final Color? iconColor;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final (background, foreground) = switch (tone) {
      AppStatusTone.income => (
        colors.incomeContainer,
        colors.onIncomeContainer,
      ),
      AppStatusTone.expense => (
        colors.expenseContainer,
        colors.onExpenseContainer,
      ),
      AppStatusTone.neutral => (
        colors.neutralContainer,
        colors.onNeutralContainer,
      ),
      AppStatusTone.planned => (
        colors.plannedContainer,
        colors.onPlannedContainer,
      ),
      AppStatusTone.cancelled => (
        colors.cancelledContainer,
        colors.onCancelledContainer,
      ),
      null => (surfaces.cardMuted, surfaces.ink),
    };
    return ExcludeSemantics(
      child: Container(
        width: size,
        height: size,
        decoration: BoxDecoration(color: background, shape: BoxShape.circle),
        alignment: Alignment.center,
        child: Icon(
          icon,
          size: size >= 40 ? 20 : 18,
          color: iconColor ?? foreground,
        ),
      ),
    );
  }
}
