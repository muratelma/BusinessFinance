import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_list_row.dart';

/// The one line the realized history shows about what is still coming.
///
/// It reports a count and the nearest date, never a total: adding planned
/// income, expenses, statements and neutral obligations into one number would
/// mislead, which is why the server does not send one either.
class PlannedSummaryCard extends StatelessWidget {
  const PlannedSummaryCard({
    required this.count,
    required this.onTap,
    super.key,
    this.nearestDueDate,
    this.hasAttention = false,
  });

  final int count;
  final String? nearestDueDate;
  final bool hasAttention;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    if (count == 0) return const SizedBox.shrink();
    final theme = Theme.of(context);
    final colors = theme.colorScheme;

    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        0,
        AppSpacing.medium,
        AppSpacing.small,
      ),
      child: AppCard(
        padding: EdgeInsets.zero,
        child: AppListRow(
          onTap: onTap,
          icon: hasAttention ? Icons.warning_amber_outlined : Icons.schedule,
          iconColor: hasAttention ? colors.error : colors.primary,
          title: '$count planlanan işlem',
          // Tarih okunur biçimde: ham `2026-09-30` sunucunun iç gösterimidir,
          // kullanıcı cümlesi değil.
          subtitle: nearestDueDate == null
              ? 'Bakiyeye dahil değil'
              : 'En yakını ${DateText.dayMonth(nearestDueDate!)} • '
                    'Bakiyeye dahil değil',
          trailing: const Icon(Icons.chevron_right),
        ),
      ),
    );
  }
}
