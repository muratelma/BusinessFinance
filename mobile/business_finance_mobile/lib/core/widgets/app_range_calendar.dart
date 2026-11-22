import 'package:flutter/material.dart';

import '../formatters/date_text.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';

/// Tarih aralığı takvimi: haftalar, seçim şeridi ve uçlarda dolu daire.
///
/// Kart Hareketleri'nin ekstre seçicisinden (13 Kasım 2026) ortak bileşene
/// alındı; Planlananlar aynı ızgarayı ay sayfalarıyla kullanır.
class AppRangeCalendarGrid extends StatelessWidget {
  const AppRangeCalendarGrid({
    super.key,
    required this.days,
    required this.first,
    required this.last,
    required this.start,
    required this.end,
    required this.dimOutside,
    required this.onTap,
  });
  final List<DateTime> days;
  final DateTime first;
  final DateTime last;
  final DateTime? start;
  final DateTime? end;

  /// Gösterilen dönemin ([first]–[last]) dışındaki günler soluk yazılır.
  final bool dimOutside;
  final ValueChanged<DateTime> onTap;

  static const _weekdays = ['Pt', 'Sa', 'Ça', 'Pe', 'Cu', 'Ct', 'Pz'];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final brand = theme.colorScheme.primary;
    final band = theme.colorScheme.primaryContainer;
    final rows = <Widget>[
      Row(
        children: [
          for (final name in _weekdays)
            Expanded(
              child: SizedBox(
                height: 28,
                child: Center(
                  child: Text(
                    name,
                    style: theme.textTheme.bodySmall?.copyWith(
                      fontSize: AppTextSizes.caption,
                      color: surfaces.inkMuted,
                    ),
                  ),
                ),
              ),
            ),
        ],
      ),
    ];
    for (var w = 0; w < days.length; w += 7) {
      final week = days.sublist(w, w + 7);
      // Seçim şeridi haftada tek parça çizilir; gün gün çizilince günlerin
      // arasında ince dikişler görünüyordu.
      final banded = [for (final day in week) _banded(day)];
      final from = banded.indexOf(true);
      final to = banded.lastIndexOf(true);
      rows.add(
        Padding(
          padding: const EdgeInsets.only(top: AppSpacing.xSmall),
          child: LayoutBuilder(
            builder: (context, constraints) {
              final cell = constraints.maxWidth / 7;
              final roundLeft =
                  from == 0 ||
                  (from >= 0 && DateUtils.isSameDay(week[from], start));
              final roundRight =
                  to == 6 || (to >= 0 && DateUtils.isSameDay(week[to], end));
              return Stack(
                children: [
                  if (from >= 0)
                    Positioned(
                      left: cell * from,
                      width: cell * (to - from + 1),
                      top: 0,
                      bottom: 0,
                      child: DecoratedBox(
                        decoration: BoxDecoration(
                          color: band,
                          borderRadius: BorderRadius.horizontal(
                            left: Radius.circular(
                              roundLeft ? AppRadius.chip : 0,
                            ),
                            right: Radius.circular(
                              roundRight ? AppRadius.chip : 0,
                            ),
                          ),
                        ),
                      ),
                    ),
                  Row(
                    children: [
                      for (final day in week)
                        Expanded(child: _day(context, day, brand, surfaces)),
                    ],
                  ),
                ],
              );
            },
          ),
        ),
      );
    }
    return Column(mainAxisSize: MainAxisSize.min, children: rows);
  }

  /// Gün seçim şeridinin içinde mi (uçlar dahil, tek günlük seçim hariç).
  bool _banded(DateTime day) {
    if (start == null || end == null || DateUtils.isSameDay(start, end)) {
      return false;
    }
    return !day.isBefore(start!) && !day.isAfter(end!);
  }

  Widget _day(
    BuildContext context,
    DateTime day,
    Color brand,
    AppSurfaces surfaces,
  ) {
    final theme = Theme.of(context);
    final isStart = start != null && DateUtils.isSameDay(day, start);
    final isEnd =
        (end != null && DateUtils.isSameDay(day, end)) ||
        (end == null && isStart);
    final inside =
        start != null &&
        end != null &&
        day.isAfter(start!) &&
        day.isBefore(end!);
    final outside = dimOutside && (day.isBefore(first) || day.isAfter(last));
    final edge = isStart || isEnd;
    final showMonth = day == days.first || day.day == 1;
    return Semantics(
      button: true,
      selected: edge || inside,
      label: DateText.dayMonthYear(
        '${day.year}-${day.month.toString().padLeft(2, '0')}-${day.day.toString().padLeft(2, '0')}',
      ),
      excludeSemantics: true,
      child: GestureDetector(
        behavior: HitTestBehavior.opaque,
        onTap: () => onTap(day),
        child: Container(
          height: 46,
          alignment: Alignment.center,
          child: Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: edge ? brand : null,
            ),
            alignment: Alignment.center,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (showMonth)
                  Text(
                    DateText.shortMonths[day.month - 1],
                    style: theme.textTheme.labelSmall?.copyWith(
                      fontSize: AppTextSizes.leafMonth,
                      height: 1.1,
                      fontWeight: FontWeight.w600,
                      color: edge
                          ? theme.colorScheme.onPrimary
                          : surfaces.inkMuted,
                    ),
                  ),
                Text(
                  '${day.day}',
                  style: theme.textTheme.bodyMedium?.copyWith(
                    height: 1.1,
                    fontWeight: edge ? FontWeight.w700 : FontWeight.w400,
                    color: edge
                        ? theme.colorScheme.onPrimary
                        : outside
                        ? surfaces.inkFaint
                        : surfaces.ink,
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

/// Ay sayfalı tarih aralığı seçici (Planlananlar, 22 Kasım 2026).
///
/// Kart Hareketleri'nin seçicisiyle aynı dil: ortalı başlıkta seçim, ay
/// okları, ızgara ve altta `Vazgeç` / `Seç`. İlk dokunuş başlangıç, ikincisi
/// bitiştir; tek gün de seçilebilir. Sonuç `(başlangıç, bitiş)` olarak döner.
class AppDateRangeDialog extends StatefulWidget {
  const AppDateRangeDialog({
    required this.initialStart,
    required this.initialEnd,
    super.key,
    this.maxDays,
  });

  final DateTime initialStart;
  final DateTime initialEnd;

  /// Seçilebilecek en uzun aralık (gün); aşılırsa `Seç` kapanır ve neden yazılır.
  final int? maxDays;

  static Future<(DateTime, DateTime)?> show(
    BuildContext context, {
    required DateTime start,
    required DateTime end,
    int? maxDays,
  }) => showDialog<(DateTime, DateTime)>(
    context: context,
    builder: (_) => AppDateRangeDialog(
      initialStart: start,
      initialEnd: end,
      maxDays: maxDays,
    ),
  );

  @override
  State<AppDateRangeDialog> createState() => _AppDateRangeDialogState();
}

class _AppDateRangeDialogState extends State<AppDateRangeDialog> {
  late DateTime _month = DateTime(
    widget.initialStart.year,
    widget.initialStart.month,
  );
  late DateTime? _start = DateUtils.dateOnly(widget.initialStart);
  late DateTime? _end = DateUtils.dateOnly(widget.initialEnd);

  void _shift(int by) =>
      setState(() => _month = DateTime(_month.year, _month.month + by));

  void _tap(DateTime day) => setState(() {
    if (_start == null || _end != null) {
      _start = day;
      _end = null;
    } else if (day.isBefore(_start!)) {
      _start = day;
    } else {
      _end = day;
    }
  });

  static String _iso(DateTime d) =>
      '${d.year.toString().padLeft(4, '0')}-'
      '${d.month.toString().padLeft(2, '0')}-'
      '${d.day.toString().padLeft(2, '0')}';

  bool get _tooLong {
    final max = widget.maxDays;
    if (max == null || _start == null) return false;
    final end = _end ?? _start!;
    return end.difference(_start!).inDays + 1 > max;
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final first = _month;
    final last = DateTime(_month.year, _month.month + 1, 0);
    final gridStart = first.subtract(Duration(days: first.weekday - 1));
    final gridEnd = last.add(Duration(days: 7 - last.weekday));
    final days = [
      for (
        var d = gridStart;
        !d.isAfter(gridEnd);
        d = DateTime(d.year, d.month, d.day + 1)
      )
        d,
    ];
    final title = _start == null
        ? 'Tarih aralığı'
        : _end == null
        ? '${DateText.dayShortMonth(_iso(_start!))} – …'
        : DateText.dayRangeShort(_iso(_start!), _iso(_end!));
    final monthName = '${DateText.months[_month.month - 1]} ${_month.year}';
    return Dialog(
      insetPadding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.large,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppRadius.sheet),
      ),
      clipBehavior: Clip.antiAlias,
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 380),
        child: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.small,
                  AppSpacing.medium,
                  AppSpacing.small,
                  AppSpacing.compact,
                ),
                child: Row(
                  children: [
                    IconButton(
                      tooltip: 'Önceki ay',
                      onPressed: () => _shift(-1),
                      icon: const Icon(Icons.chevron_left),
                    ),
                    Expanded(
                      child: Semantics(
                        liveRegion: true,
                        child: Column(
                          children: [
                            Text(
                              title,
                              textAlign: TextAlign.center,
                              style: theme.textTheme.titleMedium?.copyWith(
                                fontWeight: FontWeight.w700,
                              ),
                            ),
                            const SizedBox(height: AppSpacing.xxSmall),
                            Text(
                              monthName,
                              textAlign: TextAlign.center,
                              style: theme.textTheme.bodySmall?.copyWith(
                                fontSize: AppTextSizes.caption,
                                color: surfaces.inkMuted,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    IconButton(
                      tooltip: 'Sonraki ay',
                      onPressed: () => _shift(1),
                      icon: const Icon(Icons.chevron_right),
                    ),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.compact,
                  0,
                  AppSpacing.compact,
                  AppSpacing.compact,
                ),
                child: AppRangeCalendarGrid(
                  days: days,
                  first: first,
                  last: last,
                  start: _start,
                  end: _end,
                  dimOutside: true,
                  onTap: _tap,
                ),
              ),
              if (_tooLong)
                Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppSpacing.large,
                    0,
                    AppSpacing.large,
                    AppSpacing.compact,
                  ),
                  child: Text(
                    'En çok ${widget.maxDays} gün seçilebilir.',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                ),
              Divider(height: 1, color: surfaces.border),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.large,
                  AppSpacing.compact,
                  AppSpacing.compact,
                  AppSpacing.compact,
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    TextButton(
                      onPressed: () => Navigator.of(context).pop(),
                      child: const Text('Vazgeç'),
                    ),
                    const SizedBox(width: AppSpacing.xSmall),
                    FilledButton(
                      onPressed: _start == null || _tooLong
                          ? null
                          : () => Navigator.of(
                              context,
                            ).pop((_start!, _end ?? _start!)),
                      style: FilledButton.styleFrom(
                        shape: const StadiumBorder(),
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppSpacing.large,
                        ),
                      ),
                      child: const Text('Seç'),
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
}
