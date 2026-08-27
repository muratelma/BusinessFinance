import 'package:flutter/material.dart';

import '../formatters/date_text.dart';
import '../theme/app_spacing.dart';
import 'app_adaptive_sheet.dart';

/// Ay ve yıl seçimi: iki okun yanındaki üçüncü yol.
///
/// Oklar komşu aya gitmek için doğru araçtır ve öyle kalıyor. Uzağa gitmek
/// için değil: sekiz ay geriye bakmak sekiz dokunuş ve sekiz ağ isteği
/// demekti; aradaki yedi ay kullanıcının sormadığı sorulardı.
///
/// Takvim (`showDatePicker`) kullanılmıyor — gün seçtirir, oysa burada gün
/// diye bir şey yok ve kullanıcı olmayan bir kararı vermek zorunda kalırdı.
abstract final class AppMonthPicker {
  /// Seçilen ayın ilk gününü döner; vazgeçilirse `null`.
  static Future<DateTime?> show({
    required BuildContext context,
    required DateTime initialMonth,
    int yearSpan = 5,
  }) => AppAdaptiveSheet.show<DateTime>(
    context: context,
    builder: (_) =>
        _MonthPickerSheet(initialMonth: initialMonth, yearSpan: yearSpan),
  );
}

class _MonthPickerSheet extends StatefulWidget {
  const _MonthPickerSheet({required this.initialMonth, required this.yearSpan});

  final DateTime initialMonth;
  final int yearSpan;

  @override
  State<_MonthPickerSheet> createState() => _MonthPickerSheetState();
}

class _MonthPickerSheetState extends State<_MonthPickerSheet> {
  late int _year = widget.initialMonth.year;

  int get _minYear => widget.initialMonth.year - widget.yearSpan;
  int get _maxYear => widget.initialMonth.year + widget.yearSpan;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.all(AppSpacing.medium),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Semantics(
            header: true,
            child: Text('Dönem seçin', style: theme.textTheme.titleMedium),
          ),
          const SizedBox(height: AppSpacing.medium),
          Row(
            children: [
              IconButton(
                tooltip: 'Önceki yıl',
                onPressed: _year > _minYear
                    ? () => setState(() => _year--)
                    : null,
                icon: const Icon(Icons.chevron_left),
              ),
              Expanded(
                child: Semantics(
                  liveRegion: true,
                  child: Text(
                    '$_year',
                    textAlign: TextAlign.center,
                    style: theme.textTheme.titleLarge,
                  ),
                ),
              ),
              IconButton(
                tooltip: 'Sonraki yıl',
                onPressed: _year < _maxYear
                    ? () => setState(() => _year++)
                    : null,
                icon: const Icon(Icons.chevron_right),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.small),
          Wrap(
            spacing: AppSpacing.small,
            runSpacing: AppSpacing.small,
            children: [
              for (var month = 1; month <= 12; month++)
                ChoiceChip(
                  label: Text(DateText.months[month - 1]),
                  selected:
                      _year == widget.initialMonth.year &&
                      month == widget.initialMonth.month,
                  onSelected: (_) =>
                      Navigator.of(context).pop(DateTime(_year, month)),
                  materialTapTargetSize: MaterialTapTargetSize.padded,
                ),
            ],
          ),
        ],
      ),
    );
  }
}
