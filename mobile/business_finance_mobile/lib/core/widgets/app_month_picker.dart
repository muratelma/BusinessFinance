import 'package:flutter/material.dart';

import '../formatters/date_text.dart';
import '../models/month_amounts.dart';
import '../network/api_exception.dart';
import '../theme/app_finance_colors.dart';
import '../theme/app_surfaces.dart';
import 'app_money_text.dart';
import 'app_row.dart';
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
  static Future<DateTime?> showWithAmounts({
    required BuildContext context,
    required DateTime initialMonth,
    required Future<FinanceYearMonths> Function(int) load,
    required String caption,
    AppMoneyEffect? effect,
  }) => showDialog<DateTime>(
    context: context,
    builder: (_) => _AmountMonthDialog(
      initialMonth: initialMonth,
      load: load,
      caption: caption,
      effect: effect,
    ),
  );

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

class _AmountMonthDialog extends StatefulWidget {
  const _AmountMonthDialog({
    required this.initialMonth,
    required this.load,
    required this.caption,
    this.effect,
  });
  final DateTime initialMonth;
  final Future<FinanceYearMonths> Function(int) load;
  final String caption;
  final AppMoneyEffect? effect;
  @override
  State<_AmountMonthDialog> createState() => _AmountMonthDialogState();
}

class _AmountMonthDialogState extends State<_AmountMonthDialog> {
  late int year = widget.initialMonth.year;
  late DateTime selected = widget.initialMonth;
  FinanceYearMonths? data;
  String? error;
  int generation = 0;
  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final current = ++generation;
    setState(() {
      data = null;
      error = null;
    });
    try {
      final result = await widget.load(year);
      if (mounted && generation == current) setState(() => data = result);
    } on ApiException catch (e) {
      if (mounted && generation == current) {
        setState(
          () => error = e.isUnauthorized ? 'Oturumunuz sona erdi.' : e.message,
        );
      }
    } on FormatException {
      if (mounted && generation == current) {
        setState(() => error = 'Aylar alınamadı.');
      }
    }
  }

  void _year(int delta) {
    setState(() => year += delta);
    _load();
  }

  @override
  Widget build(BuildContext context) => Dialog(
    child: ConstrainedBox(
      constraints: const BoxConstraints(maxWidth: 348),
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Align(
              alignment: Alignment.centerLeft,
              child: Text(
                widget.caption,
                style: Theme.of(context).textTheme.bodySmall,
              ),
            ),
            const SizedBox(height: AppSpacing.xSmall),
            AppRow(
              padding: EdgeInsets.zero,
              minHeight: 36,
              title: DateText.monthYear(selected.year, selected.month),
              titleStyle: Theme.of(context).textTheme.titleLarge,
              trailing: data?.year == selected.year
                  ? AppMoneyText(
                      amount:
                          data!.months
                              .where((item) => item.month == selected.month)
                              .firstOrNull
                              ?.amount ??
                          '0.0000',
                      currency: 'TRY',
                      size: AppMoneySize.row,
                    )
                  : null,
            ),
            const Divider(),
            Row(
              children: [
                IconButton(
                  tooltip: 'Önceki yıl',
                  onPressed: () => _year(-1),
                  icon: const Icon(Icons.chevron_left),
                ),
                Expanded(
                  child: Text(
                    '$year',
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                ),
                IconButton(
                  tooltip: 'Sonraki yıl',
                  onPressed: () => _year(1),
                  icon: const Icon(Icons.chevron_right),
                ),
              ],
            ),
            if (error != null) ...[
              Text(error!),
              TextButton(onPressed: _load, child: const Text('Yeniden dene')),
            ] else if (data == null)
              const Padding(
                padding: EdgeInsets.all(AppSpacing.xLarge),
                child: CircularProgressIndicator(),
              )
            else
              LayoutBuilder(
                builder: (context, constraints) => Wrap(
                  spacing: 4,
                  runSpacing: 8,
                  children: [
                    for (final item in data!.months)
                      SizedBox(
                        width: (constraints.maxWidth - 12) / 4,
                        child: Semantics(
                          selected:
                              selected.year == year &&
                              selected.month == item.month,
                          child: OutlinedButton(
                            onPressed: item.enabled
                                ? () => setState(
                                    () => selected = DateTime(year, item.month),
                                  )
                                : null,
                            style: OutlinedButton.styleFrom(
                              padding: const EdgeInsets.symmetric(
                                horizontal: AppSpacing.xxSmall,
                                vertical: AppSpacing.small + AppSpacing.xxSmall,
                              ),
                              foregroundColor:
                                  selected.year == year &&
                                      selected.month == item.month
                                  ? Theme.of(context).colorScheme.onPrimary
                                  : null,
                              backgroundColor:
                                  selected.year == year &&
                                      selected.month == item.month
                                  ? Theme.of(context).colorScheme.primary
                                  : AppSurfaces.of(context).cardMuted,
                            ),
                            child: Column(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Text(
                                  DateText.months[item.month - 1].substring(
                                    0,
                                    3,
                                  ),
                                  semanticsLabel:
                                      DateText.months[item.month - 1],
                                ),
                                const SizedBox(
                                  height:
                                      AppSpacing.xSmall + AppSpacing.xxSmall,
                                ),
                                // Oran sunucunun alanıdır; boyama için kullanılır.
                                if (item.barRatio > 0)
                                  LinearProgressIndicator(
                                    value: item.barRatio.clamp(0, 1),
                                    color: switch (widget.effect) {
                                      AppMoneyEffect.income =>
                                        AppFinanceColors.of(context).income,
                                      AppMoneyEffect.expense =>
                                        AppFinanceColors.of(context).expense,
                                      _ => AppSurfaces.of(context).inkMuted,
                                    },
                                  ),
                              ],
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
              ),
            const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
            Wrap(
              alignment: WrapAlignment.end,
              spacing: 4,
              children: [
                TextButton(
                  onPressed: () {
                    final now = DateTime.now();
                    setState(() {
                      year = now.year;
                      selected = DateTime(now.year, now.month);
                    });
                    _load();
                  },
                  child: const Text('Bu ay'),
                ),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text('Vazgeç'),
                ),
                FilledButton(
                  onPressed:
                      data != null &&
                          data!.year == selected.year &&
                          data!.months.any(
                            (m) => m.month == selected.month && m.enabled,
                          )
                      ? () => Navigator.pop(context, selected)
                      : null,
                  child: const Text('Göster'),
                ),
              ],
            ),
          ],
        ),
      ),
    ),
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
