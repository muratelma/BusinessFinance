import 'package:flutter/material.dart';

import '../formatters/date_text.dart';
import '../models/month_amounts.dart';
import '../network/api_exception.dart';
import '../theme/app_finance_colors.dart';
import '../theme/app_surfaces.dart';
import 'app_money_text.dart';
import 'app_dashed_border.dart';
import 'app_row.dart';
import 'app_untracked_text.dart';
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
    builder: (_) => AppUntrackedText(
      child: _AmountMonthDialog(
        initialMonth: initialMonth,
        load: load,
        caption: caption,
        effect: effect,
      ),
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
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final now = DateTime.now();
    final isNow = selected.year == now.year && selected.month == now.month;
    return Dialog(
      insetPadding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.large,
        vertical: AppSpacing.large,
      ),
      child: SizedBox(
        width: 348,
        child: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.large,
                  AppSpacing.medium + AppSpacing.xSmall,
                  AppSpacing.large,
                  AppSpacing.medium,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      widget.caption,
                      style: theme.textTheme.bodySmall?.copyWith(
                        fontSize: 13,
                        height: 1.35,
                      ),
                    ),
                    const SizedBox(height: AppSpacing.xSmall),
                    AppRow(
                      padding: EdgeInsets.zero,
                      minHeight: 28,
                      title: DateText.monthYear(selected.year, selected.month),
                      titleStyle: theme.textTheme.titleLarge,
                      trailing: data?.year == selected.year
                          ? AppMoneyText(
                              amount:
                                  data!.months
                                      .where(
                                        (item) => item.month == selected.month,
                                      )
                                      .firstOrNull
                                      ?.amount ??
                                  '0.0000',
                              currency: 'TRY',
                              size: AppMoneySize.row,
                            )
                          : null,
                    ),
                  ],
                ),
              ),
              const Divider(height: 1),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.small + AppSpacing.xSmall,
                  AppSpacing.small,
                  AppSpacing.small + AppSpacing.xSmall,
                  AppSpacing.xSmall,
                ),
                child: Row(
                  children: [
                    IconButton(
                      tooltip: 'Önceki yıl',
                      onPressed: year > 1 ? () => _year(-1) : null,
                      icon: const Icon(Icons.chevron_left),
                    ),
                    Expanded(
                      child: Text(
                        '$year',
                        textAlign: TextAlign.center,
                        style: theme.textTheme.titleMedium,
                      ),
                    ),
                    IconButton(
                      tooltip: 'Sonraki yıl',
                      onPressed: year < now.year ? () => _year(1) : null,
                      icon: const Icon(Icons.chevron_right),
                    ),
                  ],
                ),
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
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                  ),
                  child: LayoutBuilder(
                    builder: (context, constraints) => Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: [
                        for (final item in data!.months)
                          SizedBox(
                            width: (constraints.maxWidth - 24) / 4,
                            child: _monthCell(item),
                          ),
                      ],
                    ),
                  ),
                ),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.small + AppSpacing.xSmall,
                  AppSpacing.small + AppSpacing.xSmall,
                  AppSpacing.small + AppSpacing.xSmall,
                  AppSpacing.medium,
                ),
                // `Bu ay` solda durur; sağda yalnız vazgeçme ve onay kalır.
                child: Row(
                  children: [
                    if (!isNow)
                      TextButton(
                        onPressed: () {
                          setState(() {
                            year = now.year;
                            selected = DateTime(now.year, now.month);
                          });
                          _load();
                        },
                        style: TextButton.styleFrom(
                          textStyle: theme.textTheme.bodyMedium?.copyWith(
                            fontSize: 15,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        child: const Text('Bu ay'),
                      ),
                    Expanded(
                      child: OverflowBar(
                        alignment: MainAxisAlignment.end,
                        overflowAlignment: OverflowBarAlignment.end,
                        spacing: 4,
                        children: [
                          TextButton(
                            onPressed: () => Navigator.pop(context),
                            style: TextButton.styleFrom(
                              textStyle: theme.textTheme.bodyMedium?.copyWith(
                                fontSize: 15,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                            child: const Text('Vazgeç'),
                          ),
                          FilledButton(
                            style: FilledButton.styleFrom(
                              shape: const StadiumBorder(),
                              textStyle: theme.textTheme.bodyMedium?.copyWith(
                                fontSize: 15,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                            onPressed:
                                data != null &&
                                    data!.year == selected.year &&
                                    data!.months.any(
                                      (m) =>
                                          m.month == selected.month &&
                                          m.enabled,
                                    )
                                ? () => Navigator.pop(context, selected)
                                : null,
                            child: const Text('Göster'),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _monthCell(FinanceMonthValue item) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final on = selected.year == year && selected.month == item.month;
    final now = DateTime.now();
    final current = year == now.year && item.month == now.month;
    final radius = BorderRadius.circular(16);
    final hasBar = item.enabled && item.barRatio > 0;
    final scale = MediaQuery.textScalerOf(context).scale(15) / 15;
    return Semantics(
      selected: on,
      child: CustomPaint(
        foregroundPainter: !item.enabled
            ? AppDashedBorder(surfaces.borderStrong, radius)
            : null,
        child: OutlinedButton(
          onPressed: item.enabled
              ? () => setState(() => selected = DateTime(year, item.month))
              : null,
          style: OutlinedButton.styleFrom(
            padding: EdgeInsets.zero,
            minimumSize: Size.fromHeight(60 * scale.clamp(1, 2)),
            tapTargetSize: MaterialTapTargetSize.shrinkWrap,
            shape: RoundedRectangleBorder(borderRadius: radius),
            side: BorderSide(
              color: !item.enabled
                  ? Colors.transparent
                  : on || current
                  ? theme.colorScheme.primary
                  : surfaces.border,
              width: on || current ? 2 : 1,
            ),
            foregroundColor: on ? theme.colorScheme.onPrimary : surfaces.ink,
            disabledForegroundColor: surfaces.inkFaint,
            backgroundColor: on
                ? theme.colorScheme.primary
                : item.enabled
                ? surfaces.cardMuted
                : Colors.transparent,
            textStyle: theme.textTheme.bodyMedium?.copyWith(
              fontSize: 15,
              height: 1,
              fontWeight: on || current ? FontWeight.w700 : FontWeight.w500,
            ),
          ),
          child: SizedBox(
            height: 60 * scale.clamp(1, 2),
            width: double.infinity,
            child: Stack(
              alignment: Alignment.center,
              children: [
                Padding(
                  padding: hasBar
                      ? const EdgeInsets.only(bottom: AppSpacing.small)
                      : EdgeInsets.zero,
                  child: Text(
                    DateText.months[item.month - 1].substring(0, 3),
                    semanticsLabel: DateText.months[item.month - 1],
                  ),
                ),
                if (hasBar)
                  Positioned(
                    left: 12,
                    right: 12,
                    bottom: 10,
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(999),
                      child: LinearProgressIndicator(
                        minHeight: 4,
                        value: item.barRatio.clamp(0.1, 1),
                        backgroundColor: on
                            ? surfaces.inkMuted
                            : surfaces.border,
                        color: on
                            ? theme.colorScheme.onPrimary
                            : switch (widget.effect) {
                                AppMoneyEffect.income => AppFinanceColors.of(
                                  context,
                                ).income,
                                AppMoneyEffect.expense => AppFinanceColors.of(
                                  context,
                                ).expense,
                                _ => surfaces.inkMuted,
                              },
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
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
