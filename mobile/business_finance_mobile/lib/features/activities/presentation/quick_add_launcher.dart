import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import 'quick_add_models.dart';

/// The single entry point for recording a movement. It only routes: no financial
/// rule lives here, and there is no one dynamic form trying to be all of them.
class QuickAddLauncher extends StatelessWidget {
  const QuickAddLauncher({super.key});

  static Future<QuickAddOption?> show(BuildContext context) =>
      AppAdaptiveSheet.show<QuickAddOption>(
        context: context,
        builder: (context) => const QuickAddLauncher(),
      );

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return SafeArea(
      // The entries with their descriptions do not fit a short screen, a
      // landscape phone or a large text scale, so the list scrolls instead of
      // overflowing.
      child: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.medium,
                0,
                AppSpacing.medium,
                AppSpacing.small,
              ),
              child: Text('İşlem ekle', style: theme.textTheme.titleLarge),
            ),
            for (final option in QuickAddOption.values)
              ListTile(
                leading: Icon(_icon(option)),
                title: Text(option.label),
                subtitle: Text(option.description),
                onTap: () => Navigator.of(context).pop(option),
              ),
            const SizedBox(height: AppSpacing.small),
          ],
        ),
      ),
    );
  }

  IconData _icon(QuickAddOption option) => switch (option) {
    QuickAddOption.expense => Icons.north_east,
    QuickAddOption.receipt => Icons.receipt_long_outlined,
    QuickAddOption.obligation => Icons.schedule_outlined,
    QuickAddOption.income => Icons.south_west,
    QuickAddOption.bankSlip => Icons.account_balance_outlined,
    QuickAddOption.transfer => Icons.swap_horiz,
    QuickAddOption.cardPayment => Icons.payments_outlined,
    QuickAddOption.recurringPlan => Icons.event_repeat,
  };
}
