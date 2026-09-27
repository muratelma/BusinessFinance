import 'package:flutter/material.dart';

import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_row.dart';
import 'quick_add_models.dart';

/// The single entry point for recording a movement. It only routes: no financial
/// rule lives here, and there is no one dynamic form trying to be all of them.
///
/// Tasarım teslimi (27 Eylül 2026): en sık üç işlem büyük renkli kutucukta
/// (Gelir · Gider · Transfer), belge okuma ikinci sırada iki gri kutucukta,
/// seyrek işlemler `Diğer` altında tek satırlık kısa listede. Panel kök
/// Navigator'da açılır ve alt çubuğun üstüne biner.
class QuickAddLauncher extends StatelessWidget {
  const QuickAddLauncher({super.key});

  static Future<QuickAddOption?> show(BuildContext context) =>
      AppAdaptiveSheet.show<QuickAddOption>(
        context: context,
        builder: (context) => const QuickAddLauncher(),
      );

  static const _primary = [
    QuickAddOption.income,
    QuickAddOption.expense,
    QuickAddOption.transfer,
  ];
  static const _documents = [QuickAddOption.receipt, QuickAddOption.bankSlip];
  static const _more = [
    QuickAddOption.posCollection,
    QuickAddOption.obligation,
    QuickAddOption.cardPayment,
    QuickAddOption.recurringPlan,
  ];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final label = theme.textTheme.labelMedium;
    void pick(QuickAddOption option) => Navigator.of(context).pop(option);

    return SafeArea(
      // Büyük yazı ölçeğinde panel taşabilir; kaydırma meşrudur, kırpılma
      // değildir.
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          0,
          AppSpacing.medium,
          AppSpacing.large,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Semantics(
              header: true,
              child: Text('İşlem ekle', style: theme.textTheme.headlineSmall),
            ),
            const SizedBox(height: AppSpacing.large - AppSpacing.xSmall),
            IntrinsicHeight(
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  for (var i = 0; i < _primary.length; i++) ...[
                    if (i > 0) const SizedBox(width: AppSpacing.small),
                    Expanded(
                      child: _PrimaryTile(
                        option: _primary[i],
                        onTap: () => pick(_primary[i]),
                      ),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(height: AppSpacing.large),
            Text('Belgeden oku', style: label),
            const SizedBox(height: AppSpacing.small),
            IntrinsicHeight(
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  for (var i = 0; i < _documents.length; i++) ...[
                    if (i > 0) const SizedBox(width: AppSpacing.small),
                    Expanded(
                      child: _DocumentTile(
                        option: _documents[i],
                        onTap: () => pick(_documents[i]),
                      ),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(height: AppSpacing.large),
            Text('Diğer', style: label),
            const SizedBox(height: AppSpacing.xSmall),
            AppDividedColumn(
              inset: AppIconCapsule.rowInset - AppSpacing.medium,
              children: [
                for (final option in _more)
                  AppRow(
                    padding: const EdgeInsets.symmetric(
                      vertical: AppSpacing.small + AppSpacing.xxSmall,
                    ),
                    leading: AppIconCapsule(icon: quickAddIcon(option)),
                    title: option.label,
                    subtitle: option.description,
                    semanticLabel: option.spokenLabel,
                    trailing: Icon(
                      Icons.chevron_right,
                      size: 22,
                      color: surfaces.inkMuted,
                    ),
                    onTap: () => pick(option),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

/// Menüdeki seçeneğin ikonu.
IconData quickAddIcon(QuickAddOption option) => switch (option) {
  QuickAddOption.expense => Icons.north_east,
  QuickAddOption.receipt => Icons.receipt_long_outlined,
  QuickAddOption.obligation => Icons.schedule_outlined,
  QuickAddOption.income => Icons.south_west,
  QuickAddOption.posCollection => Icons.point_of_sale_outlined,
  QuickAddOption.bankSlip => Icons.account_balance_outlined,
  QuickAddOption.transfer => Icons.swap_horiz,
  QuickAddOption.cardPayment => Icons.credit_card,
  QuickAddOption.recurringPlan => Icons.event_repeat,
};

/// Rol renginde büyük kutucuk: 96 dp, kart yarıçapı, ortada ikon 24 ve etiket.
/// Transfer nötr mavidir: gelir de gider de değildir (ADR 0014).
class _PrimaryTile extends StatelessWidget {
  const _PrimaryTile({required this.option, required this.onTap});

  final QuickAddOption option;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final (background, foreground) = switch (option) {
      QuickAddOption.income => (
        colors.incomeContainer,
        colors.onIncomeContainer,
      ),
      QuickAddOption.expense => (
        colors.expenseContainer,
        colors.onExpenseContainer,
      ),
      _ => (colors.neutralContainer, colors.onNeutralContainer),
    };
    final radius = BorderRadius.circular(AppRadius.card);
    return Semantics(
      button: true,
      label: option.spokenLabel,
      onTap: onTap,
      excludeSemantics: true,
      child: Material(
        color: background,
        borderRadius: radius,
        child: InkWell(
          borderRadius: radius,
          onTap: onTap,
          child: ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 96),
            child: Padding(
              padding: const EdgeInsets.all(AppSpacing.small),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(quickAddIcon(option), size: 24, color: foreground),
                  const SizedBox(height: AppSpacing.small),
                  Text(
                    option.label,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                      height: 1.3,
                      color: foreground,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Belge okuma kutucuğu: gri zemin, 64 dp, solda ikon.
class _DocumentTile extends StatelessWidget {
  const _DocumentTile({required this.option, required this.onTap});

  final QuickAddOption option;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final radius = BorderRadius.circular(AppRadius.card);
    return Semantics(
      button: true,
      label: option.spokenLabel,
      onTap: onTap,
      excludeSemantics: true,
      child: Material(
        color: surfaces.cardMuted,
        borderRadius: radius,
        child: InkWell(
          borderRadius: radius,
          onTap: onTap,
          child: ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 64),
            child: Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium - AppSpacing.xxSmall,
                vertical: AppSpacing.small,
              ),
              child: Row(
                children: [
                  Icon(quickAddIcon(option), size: 22, color: surfaces.ink),
                  const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                  Expanded(
                    child: Text(
                      option.label,
                      style: Theme.of(context).textTheme.titleSmall?.copyWith(
                        fontSize: 15,
                        height: 1.3,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
