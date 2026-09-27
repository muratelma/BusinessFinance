import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../data/activity_models.dart';

/// Advanced filters. The date range lives here rather than being applied
/// silently, so narrowing the history is always something the user chose.
class ActivityFilterSheet extends StatefulWidget {
  const ActivityFilterSheet({required this.filter, super.key});

  final ActivityFilter filter;

  @override
  State<ActivityFilterSheet> createState() => _ActivityFilterSheetState();
}

class _ActivityFilterSheetState extends State<ActivityFilterSheet> {
  late ActivityFilter _draft = widget.filter;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Gelişmiş filtre', style: theme.textTheme.titleLarge),
            const SizedBox(height: AppSpacing.medium),
            Text('Tarih aralığı', style: theme.textTheme.titleSmall),
            const SizedBox(height: AppSpacing.small),
            Wrap(
              spacing: AppSpacing.small,
              children: [
                for (final range in ActivityDateRange.values)
                  ChoiceChip(
                    label: Text(range.label),
                    selected: _draft.dateRange == range,
                    onSelected: (_) => setState(
                      () => _draft = _draft.copyWith(dateRange: range),
                    ),
                  ),
              ],
            ),
            const SizedBox(height: AppSpacing.medium),
            Text('Etki', style: theme.textTheme.titleSmall),
            const SizedBox(height: AppSpacing.small),
            Wrap(
              spacing: AppSpacing.small,
              children: [
                ChoiceChip(
                  label: const Text('Tümü'),
                  selected: _draft.effect == null,
                  onSelected: (_) => setState(
                    () => _draft = _draft.copyWith(clearEffect: true),
                  ),
                ),
                ChoiceChip(
                  label: const Text('Gelir'),
                  selected: _draft.effect == ActivityEffect.income,
                  onSelected: (_) => setState(
                    () =>
                        _draft = _draft.copyWith(effect: ActivityEffect.income),
                  ),
                ),
                ChoiceChip(
                  label: const Text('Gider'),
                  selected: _draft.effect == ActivityEffect.expense,
                  onSelected: (_) => setState(
                    () => _draft = _draft.copyWith(
                      effect: ActivityEffect.expense,
                    ),
                  ),
                ),
                ChoiceChip(
                  label: const Text('Nötr'),
                  selected: _draft.effect == ActivityEffect.neutral,
                  onSelected: (_) => setState(
                    () => _draft = _draft.copyWith(
                      effect: ActivityEffect.neutral,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.small),
            SwitchListTile(
              contentPadding: EdgeInsets.zero,
              value: _draft.includeCancelled,
              onChanged: (value) => setState(
                () => _draft = _draft.copyWith(includeCancelled: value),
              ),
              title: const Text('İptal edilenleri göster'),
              subtitle: const Text(
                'İptal edilen hareketler geçmişte kalır, bakiyeye katılmaz.',
              ),
            ),
            const SizedBox(height: AppSpacing.medium),
            Row(
              children: [
                TextButton(
                  onPressed: () => setState(
                    () => _draft = ActivityFilter(
                      quickFilter: _draft.quickFilter,
                      // Arama ayrı bir denetimdir; filtre temizliği onu silmez.
                      search: _draft.search,
                    ),
                  ),
                  child: const Text('Temizle'),
                ),
                const Spacer(),
                TextButton(
                  onPressed: () => Navigator.of(context).pop(),
                  child: const Text('Vazgeç'),
                ),
                const SizedBox(width: AppSpacing.small),
                FilledButton(
                  onPressed: () => Navigator.of(context).pop(_draft),
                  child: const Text('Uygula'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
