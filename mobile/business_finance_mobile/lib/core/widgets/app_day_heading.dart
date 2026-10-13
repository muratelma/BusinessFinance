import '../theme/app_spacing.dart';
import 'package:flutter/material.dart';
import '../formatters/date_text.dart';
import '../theme/app_surfaces.dart';

class AppDayHeading extends StatelessWidget {
  const AppDayHeading(this.date, {super.key});
  final String date;
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(
        left: AppSpacing.medium,
        top: AppSpacing.medium + AppSpacing.xxSmall,
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
                style: Theme.of(
                  context,
                ).textTheme.titleSmall?.copyWith(fontSize: 15),
              ),
              TextSpan(
                text: '  ${DateText.dayMonthWeekday(date).split(' ').last}',
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
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
