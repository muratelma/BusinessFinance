import '../theme/app_spacing.dart';
import 'package:flutter/material.dart';
import '../formatters/date_text.dart';
import '../theme/app_surfaces.dart';

/// Gün başlığı: tarih koyu, gün adı soluk (`25 Ekim Pazar`).
///
/// Teslimin ölçüsü: üstte 20, altta 8 dp; tarih 15/600, gün adı 14/1,35 ve
/// aralarında 8 dp. İki yazı taban çizgisinde hizalanır.
class AppDayHeading extends StatelessWidget {
  const AppDayHeading(this.date, {super.key});
  final String date;
  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.only(
        left: AppSpacing.medium,
        top: AppSpacing.medium + AppSpacing.xSmall,
        right: AppSpacing.medium,
        bottom: AppSpacing.small,
      ),
      child: Semantics(
        header: true,
        child: Text.rich(
          TextSpan(
            children: [
              TextSpan(
                text: DateText.dayMonth(date),
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: 15,
                  height: 1.3,
                ),
              ),
              const WidgetSpan(child: SizedBox(width: AppSpacing.small)),
              TextSpan(
                text: DateText.dayMonthWeekday(date).split(' ').last,
                style: theme.textTheme.bodySmall?.copyWith(
                  height: 1.35,
                  color: AppSurfaces.of(context).inkMuted,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
