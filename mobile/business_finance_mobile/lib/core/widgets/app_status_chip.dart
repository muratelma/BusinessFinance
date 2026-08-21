import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';

enum AppStatusTone { income, expense, neutral, planned, cancelled }

/// Durum rozeti: her zaman ikon **ve** metin taşır.
///
/// Rozet yalnız renkten ibaret olsaydı (soluk bir satır, kırmızı bir nokta)
/// renk körü kullanıcı ile ekran okuyucu kullanıcısı bilgiyi hiç almazdı. Bu
/// yüzden ikon ve etiket zorunlu, renk yalnız destekleyicidir.
class AppStatusChip extends StatelessWidget {
  const AppStatusChip({
    required this.label,
    required this.icon,
    required this.tone,
    super.key,
  });

  final String label;
  final IconData icon;
  final AppStatusTone tone;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
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
    };

    return Container(
      // Rozet cihazda olduğundan küçük duruyordu: yanındaki eylem butonuyla
      // aynı aileden görünmesi için yükseklik ve iç boşluk bir kademe arttı.
      constraints: const BoxConstraints(minHeight: 32),
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.small + AppSpacing.xSmall,
        vertical: AppSpacing.xSmall + 2,
      ),
      decoration: BoxDecoration(
        color: background,
        borderRadius: BorderRadius.circular(AppRadius.chip),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 16, color: foreground),
          const SizedBox(width: AppSpacing.xSmall),
          // Etiket metin ölçeğiyle büyür; sabit yükseklik verilmediği için
          // 2.0x ölçekte kırpılmaz.
          Flexible(
            child: Text(
              label,
              style: Theme.of(
                context,
              ).textTheme.labelMedium?.copyWith(color: foreground),
            ),
          ),
        ],
      ),
    );
  }
}
