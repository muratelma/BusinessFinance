import 'package:business_finance_mobile/core/formatters/date_text.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/theme/app_surfaces.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_divided_column.dart';
import 'package:business_finance_mobile/core/widgets/app_icon_capsule.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:business_finance_mobile/core/widgets/app_segment_rail.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_tile.dart';
import 'package:flutter/material.dart';

/// Liste sistemi karar çizimlerinin (Aşama 06.3 Grup 8) ortak parçaları.
/// Yalnız çizim içindir; uygulama bittiğinde çizim dosyalarıyla birlikte
/// kalkar.

Widget designRail(BuildContext context, List<String> values, String selected) {
  final theme = Theme.of(context);
  final surfaces = AppSurfaces.of(context);
  return AppSegmentRail<String>(
    values: values,
    selected: selected,
    semanticLabel: 'Seçim',
    segmentLabel: (value) => value,
    onChanged: (_) {},
    segmentBuilder: (context, value, isSelected) => Text(
      value,
      style: theme.textTheme.bodyMedium?.copyWith(
        fontWeight: FontWeight.w600,
        color: isSelected ? surfaces.ink : surfaces.inkMuted,
      ),
    ),
  );
}

/// Sağ üstteki ay: dokununca ay seçici açılır.
Widget designMonthButton() => TextButton.icon(
  onPressed: () {},
  icon: const Icon(Icons.expand_more),
  iconAlignment: IconAlignment.end,
  label: const Text('Ekim 2026'),
);

Widget designChipRow(List<Widget> children) => SingleChildScrollView(
  scrollDirection: Axis.horizontal,
  padding: const EdgeInsets.fromLTRB(
    AppSpacing.medium,
    AppSpacing.xSmall,
    AppSpacing.medium,
    AppSpacing.small,
  ),
  child: Row(
    children: [
      for (final child in children)
        Padding(
          padding: const EdgeInsets.only(right: AppSpacing.small),
          child: child,
        ),
    ],
  ),
);

Widget designChoice(String label, {bool selected = false}) =>
    ChoiceChip(label: Text(label), selected: selected, onSelected: (_) {});

/// Açılır süzgeç çipi. [value] doluysa seçim yapılmıştır: çip seçilenin adını
/// yazar ve dolu görünür; dokununca diğer seçenekler açılır.
Widget designDropChip(String label, {String? value}) => Chip(
  avatar: const Icon(Icons.expand_more, size: 18),
  label: Text(value ?? label),
  backgroundColor: value == null ? null : const Color(0xFFE4E6EA),
);

/// Çipin hemen altında açılan menü: ad ve tutar; seçili olanın başında tik.
class DesignMenu extends StatelessWidget {
  const DesignMenu({required this.items, super.key, this.width = 300});

  /// (ad, tutar, seçili mi). Tutar boşsa yazılmaz.
  final List<(String, String, bool)> items;
  final double width;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Container(
      width: width,
      decoration: BoxDecoration(
        color: surfaces.card,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: surfaces.border),
        boxShadow: const [
          BoxShadow(
            color: Color(0x29000000),
            blurRadius: 18,
            offset: Offset(0, 6),
          ),
        ],
      ),
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          for (final (name, amount, selected) in items)
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.small + AppSpacing.xSmall,
              ),
              child: Row(
                children: [
                  SizedBox(
                    width: 28,
                    child: selected
                        ? Icon(Icons.check, size: 20, color: surfaces.ink)
                        : null,
                  ),
                  Expanded(
                    child: Text(
                      name,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: theme.textTheme.bodyLarge,
                    ),
                  ),
                  if (amount.isNotEmpty)
                    AppMoneyText(
                      amount: amount,
                      currency: 'TRY',
                      size: AppMoneySize.row,
                      style: TextStyle(color: surfaces.inkMuted),
                    ),
                ],
              ),
            ),
        ],
      ),
    );
  }
}

/// Sayfanın üstüne binen katman: arkası kararır, [child] ortada durur.
class DesignDialogOver extends StatelessWidget {
  const DesignDialogOver({required this.page, required this.child, super.key});

  final Widget page;
  final Widget child;

  @override
  Widget build(BuildContext context) => Stack(
    children: [
      page,
      const Positioned.fill(child: ColoredBox(color: Colors.black54)),
      Center(child: child),
    ],
  );
}

/// Ay seçici: ekranın ortasında, her zaman aynı boyda; on iki ay eşit
/// hücrelerde. Sağ üstteki aya dokununca açılır.
class DesignMonthDialog extends StatelessWidget {
  const DesignMonthDialog({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.large),
      child: Material(
        borderRadius: BorderRadius.circular(28),
        clipBehavior: Clip.antiAlias,
        child: Padding(
          padding: const EdgeInsets.all(AppSpacing.large),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                children: [
                  IconButton(
                    onPressed: () {},
                    icon: const Icon(Icons.chevron_left),
                  ),
                  Expanded(
                    child: Text(
                      '2026',
                      textAlign: TextAlign.center,
                      style: theme.textTheme.titleLarge,
                    ),
                  ),
                  IconButton(
                    onPressed: () {},
                    icon: const Icon(Icons.chevron_right),
                  ),
                ],
              ),
              const SizedBox(height: AppSpacing.medium),
              for (var row = 0; row < 4; row++)
                Padding(
                  padding: const EdgeInsets.only(bottom: AppSpacing.small),
                  child: Row(
                    children: [
                      for (var column = 0; column < 3; column++) ...[
                        if (column > 0) const SizedBox(width: AppSpacing.small),
                        Expanded(
                          child: _cell(theme, surfaces, row * 3 + column),
                        ),
                      ],
                    ],
                  ),
                ),
              const SizedBox(height: AppSpacing.small),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  TextButton(onPressed: () {}, child: const Text('Bu ay')),
                  TextButton(onPressed: () {}, child: const Text('Kapat')),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _cell(ThemeData theme, AppSurfaces surfaces, int month) {
    final selected = month == 9;
    final future = month > 9;
    return Container(
      height: 48,
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: selected ? surfaces.ink : null,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: surfaces.border),
      ),
      child: Text(
        DateText.months[month],
        style: theme.textTheme.bodyMedium?.copyWith(
          fontWeight: FontWeight.w600,
          color: selected
              ? surfaces.card
              : future
              ? surfaces.inkMuted
              : surfaces.ink,
        ),
      ),
    );
  }
}

/// Akışın gün gün dizilmiş satırları: İşlemler'deki satırın kendisi.
class DesignDays extends StatelessWidget {
  const DesignDays({required this.items, super.key});

  final List<FinancialActivity> items;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final days = <String, List<FinancialActivity>>{};
    for (final item in items) {
      days.putIfAbsent(item.activityDate, () => []).add(item);
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        for (final day in days.keys) ...[
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.small,
            ),
            child: Text(
              DateText.dayMonth(day),
              style: theme.textTheme.titleSmall?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: AppCard(
              padding: EdgeInsets.zero,
              child: AppDividedColumn(
                inset: AppIconCapsule.rowInset,
                children: [
                  for (final item in days[day]!)
                    ActivityTile(activity: item, showDate: false),
                ],
              ),
            ),
          ),
        ],
      ],
    );
  }
}

FinancialActivity designActivity(
  String title,
  String date,
  ActivityKind kind,
  ActivityEffect effect,
  String amount, {
  String? category,
  String? source,
  String? destination,
  ActivitySourceGroup group = ActivitySourceGroup.account,
}) => FinancialActivity(
  activityId: '$title$date$amount',
  kind: kind,
  effect: effect,
  sourceGroup: group,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: date,
  amount: amount,
  currency: 'TRY',
  title: title,
  categoryName: category,
  sourceName: source,
  destinationName: destination,
  canCancel: true,
  supportsAttachments: false,
);
