import 'dart:math' as math;

import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_date_leaf.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_donut_chart.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_share_bar.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_unknown_amount.dart';
import '../../activities/data/planned_activity_models.dart';
import '../../planning/data/planning_models.dart';
import '../data/dashboard_models.dart';

/// Özet'in bir bölümü: başlık, isteğe bağlı sayaç ve not, altında kart.
///
/// Kendi ekranı olan bölümde ([onOpen]) başlığın tamamı dokunulabilir ve
/// sağında ok durur.
class DashboardBlock extends StatelessWidget {
  const DashboardBlock({
    required this.title,
    required this.child,
    super.key,
    this.count,
    this.note,
    this.onOpen,
  });

  final String title;
  final String? count;
  final String? note;
  final VoidCallback? onOpen;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final countLabel = count == null
        ? null
        : Text(
            count!,
            style: theme.textTheme.bodyMedium?.copyWith(
              fontWeight: FontWeight.w600,
              height: 1.3,
              letterSpacing: 0.3,
              color: surfaces.inkFaint,
            ),
          );
    Widget header = AppSectionHeader(
      title: title,
      trailing: onOpen == null
          ? countLabel
          : Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                ?countLabel,
                const SizedBox(width: AppSpacing.xSmall),
                Icon(Icons.chevron_right, size: 22, color: surfaces.inkMuted),
              ],
            ),
    );
    if (onOpen != null) {
      header = Semantics(
        button: true,
        label: '$title ekranını aç',
        child: InkWell(
          onTap: onOpen,
          borderRadius: BorderRadius.circular(AppRadius.field),
          child: ExcludeSemantics(child: header),
        ),
      );
    }
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.large),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          header,
          if (note != null)
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.small),
              child: Text(
                note!,
                style: theme.textTheme.bodySmall?.copyWith(letterSpacing: 0.2),
              ),
            ),
          child,
        ],
      ),
    );
  }
}

/// Kategori giderleri: 104'lük halka ve yanında ayın toplamı ile en büyük
/// pay; altında her dilim için pay çubuklu satır.
///
/// Gruplamayı ve "Diğer" toplamını **sunucu** yapar. Yüzde ve çubuk boyu
/// yalnız çizim oranıdır; yeni bir tutar üretilmez.
class DashboardCategoryCard extends StatelessWidget {
  const DashboardCategoryCard({required this.report, super.key});

  final DashboardReport report;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final slices = report.categoryExpenseSlices;
    if (slices.isEmpty) {
      return AppCard(
        child: Text(
          'Bu ay kategori gideri yok.',
          style: theme.textTheme.bodySmall,
        ),
      );
    }
    final values = [for (final slice in slices) _value(slice.amount)];
    final total = values.fold<double>(0, (sum, value) => sum + value);
    final topIndex = slices.indexWhere((slice) => !slice.isOther);
    final top = topIndex < 0 ? null : slices[topIndex];

    final summary = Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          'Bu ay toplam',
          style: theme.textTheme.bodyMedium?.copyWith(
            fontWeight: FontWeight.w500,
            height: 1.3,
            color: surfaces.inkMuted,
          ),
        ),
        const SizedBox(height: AppSpacing.xxSmall),
        AppMoneyText(
          amount: report.totalExpense,
          currency: report.currency,
          effect: AppMoneyEffect.expense,
          size: AppMoneySize.metric,
        ),
        if (top != null) ...[
          const SizedBox(height: AppSpacing.xSmall),
          Divider(height: 1, thickness: 1, color: surfaces.border),
          const SizedBox(height: AppSpacing.small),
          Text(
            'En büyük pay',
            style: theme.textTheme.labelMedium?.copyWith(
              fontWeight: FontWeight.w400,
              letterSpacing: 0,
              color: surfaces.inkMuted,
            ),
          ),
          const SizedBox(height: AppSpacing.xxSmall),
          Text(
            '${top.categoryName} · %${_percent(values[topIndex], total)}',
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: theme.textTheme.bodyMedium?.copyWith(
              fontWeight: FontWeight.w600,
              height: 1.3,
            ),
          ),
        ],
      ],
    );

    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              AppDonutChart(
                size: 104,
                thickness: 16,
                showLegend: false,
                slices: [
                  for (var i = 0; i < slices.length; i++)
                    AppDonutSlice(
                      label: slices[i].categoryName,
                      value: values[i],
                      color: _sliceColor(context, slices, i),
                      formattedValue: MoneyText.format(
                        slices[i].amount,
                        report.currency,
                      ),
                    ),
                ],
              ),
              const SizedBox(width: AppSpacing.large - AppSpacing.xSmall),
              Expanded(child: summary),
            ],
          ),
          const SizedBox(height: AppSpacing.large),
          for (var i = 0; i < slices.length; i++) ...[
            if (i > 0)
              const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
            _CategoryLine(
              slice: slices[i],
              currency: report.currency,
              color: _sliceColor(context, slices, i),
              ratio: total <= 0 ? 0 : values[i] / total,
              percent: _percent(values[i], total),
            ),
          ],
        ],
      ),
    );
  }

  static double _value(String amount) => double.tryParse(amount) ?? 0;

  static int _percent(double value, double total) =>
      total <= 0 ? 0 : (value / total * 100).round();

  /// "Diğer" paletin dışında kendi grisini alır: bir kategori değil, kalanın
  /// toplamıdır.
  static Color _sliceColor(
    BuildContext context,
    List<CategoryExpenseSlice> slices,
    int index,
  ) {
    final colors = AppFinanceColors.of(context);
    return slices[index].isOther
        ? colors.categoryOtherSlice
        : colors.categorySlice(index);
  }
}

class _CategoryLine extends StatelessWidget {
  const _CategoryLine({
    required this.slice,
    required this.currency,
    required this.color,
    required this.ratio,
    required this.percent,
  });

  final CategoryExpenseSlice slice;
  final String currency;
  final Color color;
  final double ratio;
  final int percent;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      container: true,
      label:
          '${slice.categoryName}: ${MoneyText.format(slice.amount, currency)}, '
          'yüzde $percent',
      child: ExcludeSemantics(
        child: Row(
          children: [
            AppIconCapsule(
              icon: slice.isOther
                  ? Icons.more_horiz
                  : AppFinanceIcons.forCategory(
                      slice.canonicalName,
                      displayName: slice.categoryName,
                    ),
              size: 32,
              iconColor: color,
            ),
            const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  if (context.usesLargeText) ...[
                    Text(
                      slice.categoryName,
                      style: theme.textTheme.bodyMedium?.copyWith(height: 1.3),
                    ),
                    Wrap(
                      alignment: WrapAlignment.end,
                      crossAxisAlignment: WrapCrossAlignment.center,
                      spacing: AppSpacing.small,
                      children: [
                        Text(
                          '%$percent',
                          style: theme.textTheme.labelMedium?.copyWith(
                            fontWeight: FontWeight.w400,
                            letterSpacing: 0,
                            color: surfaces.inkMuted,
                          ),
                        ),
                        AppMoneyText(
                          amount: slice.amount,
                          currency: currency,
                          size: AppMoneySize.body,
                          style: const TextStyle(fontWeight: FontWeight.w600),
                        ),
                      ],
                    ),
                  ] else
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.baseline,
                      textBaseline: TextBaseline.alphabetic,
                      children: [
                        Expanded(
                          child: Text(
                            slice.categoryName,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: theme.textTheme.bodyMedium?.copyWith(
                              height: 1.3,
                            ),
                          ),
                        ),
                        const SizedBox(width: AppSpacing.small),
                        Text(
                          '%$percent',
                          style: theme.textTheme.labelMedium?.copyWith(
                            fontWeight: FontWeight.w400,
                            letterSpacing: 0,
                            fontFeatures: AppTypography.tabularFigures,
                            color: surfaces.inkMuted,
                          ),
                        ),
                        const SizedBox(width: AppSpacing.small),
                        ConstrainedBox(
                          constraints: const BoxConstraints(minWidth: 88),
                          child: AppMoneyText(
                            amount: slice.amount,
                            currency: currency,
                            size: AppMoneySize.body,
                            textAlign: TextAlign.right,
                            style: const TextStyle(fontWeight: FontWeight.w600),
                          ),
                        ),
                      ],
                    ),
                  const SizedBox(
                    height: AppSpacing.xSmall + AppSpacing.xxSmall,
                  ),
                  AppShareBar(ratio: ratio, color: color, height: 4),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Bütçeler: en dolu bütçelerin halkaları. Dört ve fazlası 2×2, iki–üç
/// ikili, bir tane tek.
///
/// Kalan ve aşım tutarı sunucunun `remaining`'idir (aşımda negatif);
/// halkanın dolu kısmı ve yüzde yalnız çizim oranıdır.
class DashboardBudgetRings extends StatelessWidget {
  const DashboardBudgetRings({
    required this.items,
    required this.currency,
    required this.onOpen,
    super.key,
  });

  final List<BudgetVarianceItem> items;
  final String currency;
  final VoidCallback onOpen;

  /// Kaç halka gösterildiği: 4+ → 4, 2–3 → 2, 1 → 1.
  static int shownCount(int total) => total >= 4
      ? 4
      : total >= 2
      ? 2
      : total;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    if (items.isEmpty) {
      return AppCard(
        onTap: onOpen,
        child: Text(
          'Bu ay için bütçe yok. Kategori başına limit koyarak harcamayı '
          'izleyebilirsiniz.',
          style: theme.textTheme.bodySmall,
        ),
      );
    }
    final shown = [...items]
      ..sort((a, b) => b.spentRatio.compareTo(a.spentRatio));
    final visible = shown.take(shownCount(items.length)).toList();
    final columns = math.min(visible.length, 2);
    final rows = <List<BudgetVarianceItem>>[
      for (var i = 0; i < visible.length; i += columns)
        visible.sublist(i, math.min(i + columns, visible.length)),
    ];

    // Kartın tamamı Bütçeler ekranına gider: ekran okuyucu için tek durak,
    // halkaların cümleleri etiketin içinde.
    return _TappableSummary(
      onTap: onOpen,
      label: [
        for (final item in visible) _ringSentence(item, currency),
        'Tüm bütçeleri açar',
      ].join('. '),
      child: AppCard(
        onTap: onOpen,
        child: Column(
          children: [
            for (var r = 0; r < rows.length; r++) ...[
              if (r > 0) const SizedBox(height: AppSpacing.small),
              IntrinsicHeight(
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    for (var c = 0; c < rows[r].length; c++) ...[
                      if (c > 0) const SizedBox(width: AppSpacing.small),
                      Expanded(
                        child: _BudgetRing(
                          item: rows[r][c],
                          currency: currency,
                        ),
                      ),
                    ],
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _BudgetRing extends StatelessWidget {
  const _BudgetRing({required this.item, required this.currency});

  final BudgetVarianceItem item;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final over = item.isExceeded;
    final ratio = item.spentRatio;
    final percent = (ratio * 100).round();
    final strong = over ? colors.onExpenseContainer : surfaces.ink;
    final soft = over ? colors.onExpenseContainer : surfaces.inkMuted;
    final difference = MoneyText.format(
      MoneyText.unsigned(item.remaining),
      currency,
    );
    final differenceLabel = '$difference ${over ? 'fazla' : 'kaldı'}';

    return Semantics(
      container: true,
      label: _ringSentence(item, currency),
      child: ExcludeSemantics(
        child: Container(
          padding: const EdgeInsets.symmetric(
            vertical: AppSpacing.medium,
            horizontal: AppSpacing.small,
          ),
          decoration: BoxDecoration(
            color: over ? colors.expenseContainer : surfaces.cardMuted,
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
          child: Column(
            children: [
              Stack(
                alignment: Alignment.center,
                children: [
                  AppDonutChart(
                    size: 96,
                    thickness: 10,
                    showLegend: false,
                    slices: [
                      AppDonutSlice(
                        label: 'Harcanan',
                        value: math.min(1, ratio),
                        color: over ? colors.expenseFill : colors.neutralFill,
                        formattedValue: '%$percent',
                      ),
                      AppDonutSlice(
                        label: 'Kalan',
                        value: math.max(0, 1 - ratio),
                        color: surfaces.card,
                        formattedValue: '',
                      ),
                    ],
                  ),
                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(
                        AppFinanceIcons.forCategory(
                          item.canonicalName,
                          displayName: item.categoryName,
                        ),
                        size: 16,
                        color: soft,
                      ),
                      const SizedBox(width: AppSpacing.xSmall - 1),
                      Text(
                        '%$percent',
                        textScaler: TextScaler.noScaling,
                        style: TextStyle(
                          fontSize: 16,
                          height: 1,
                          fontWeight: FontWeight.w700,
                          fontFeatures: AppTypography.tabularFigures,
                          color: strong,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
              const SizedBox(height: AppSpacing.small),
              Text(
                item.categoryName,
                textAlign: TextAlign.center,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                  height: 1.3,
                  color: strong,
                ),
              ),
              const SizedBox(height: AppSpacing.small),
              Text(
                differenceLabel,
                textAlign: TextAlign.center,
                style: theme.textTheme.labelMedium?.copyWith(
                  fontWeight: FontWeight.w400,
                  letterSpacing: 0,
                  fontFeatures: AppTypography.tabularFigures,
                  color: soft,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Yaklaşanlar: önümüzdeki 7 günün ödemeleri zaman çizelgesinde; altta
/// sunucunun "7 günde çıkacak" toplamı.
///
/// **Boş durumda gizlenmiyor**: "ödeme yok" kendi başına iyi haberdir ve
/// bölüm her hafta yerinde durmalı.
///
/// **Gecikenler listeye girmez**, kartın başında tek bir satırda durur: sayı,
/// en eski vade ve sunucunun gecikmiş toplamı. Yedi gecikmiş ödeme listeye
/// girseydi haftanın ödemelerini kartın dışına iterdi. Geciken toplamı
/// "7 günde çıkacak"a eklenmez.
class DashboardUpcomingCard extends StatelessWidget {
  const DashboardUpcomingCard({
    required this.items,
    required this.total,
    required this.currency,
    required this.today,
    required this.onOpen,
    super.key,
    this.overdue = const [],
    this.overdueTotal,
  });

  final List<PlannedActivity> items;

  /// Gecikmiş ödeme yükümlülükleri; kartın başındaki tek satır.
  final List<PlannedActivity> overdue;

  /// Gecikmişlerin tutarı belli olanlarının sunucudaki toplamı.
  final String? overdueTotal;
  final String? total;
  final String currency;
  final DateTime today;
  final VoidCallback onOpen;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    if (items.isEmpty && overdue.isEmpty) {
      return AppCard(
        onTap: onOpen,
        child: Text('Bu hafta ödeme yok.', style: theme.textTheme.bodyMedium),
      );
    }
    final oldest = overdue.isEmpty
        ? null
        : overdue
              .map((item) => item.dueDate)
              .reduce((a, b) => a.compareTo(b) <= 0 ? a : b);
    final shownOverdueTotal =
        overdueTotal != null && MoneyMath.parse(overdueTotal!) != BigInt.zero
        ? overdueTotal
        : null;
    return _TappableSummary(
      onTap: onOpen,
      label: [
        if (oldest != null)
          _overdueSentence(overdue.length, oldest, shownOverdueTotal, currency),
        if (items.isEmpty) 'Bu hafta ödeme yok',
        for (final item in items)
          _upcomingSentence(item, _relative(item.dueDate)),
        if (total != null)
          '7 günde çıkacak: ${MoneyText.format(total!, currency)}',
        'Planlananları açar',
      ].join('. '),
      child: AppCard(
        padding: EdgeInsets.zero,
        onTap: onOpen,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (oldest != null)
              _OverdueRow(
                count: overdue.length,
                oldest: oldest,
                total: shownOverdueTotal,
                currency: currency,
              ),
            if (items.isEmpty)
              Padding(
                padding: const EdgeInsets.all(AppSpacing.medium),
                child: Text(
                  'Bu hafta ödeme yok.',
                  style: theme.textTheme.bodyMedium,
                ),
              )
            else
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                  vertical: AppSpacing.small,
                ),
                child: Column(
                  children: [
                    for (var i = 0; i < items.length; i++)
                      _TimelineRow(
                        item: items[i],
                        first: i == 0,
                        last: i == items.length - 1,
                        relative: _relative(items[i].dueDate),
                      ),
                  ],
                ),
              ),
            if (total != null)
              MergeSemantics(
                child: Container(
                  constraints: const BoxConstraints(minHeight: 48),
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                  ),
                  decoration: BoxDecoration(
                    color: surfaces.cardMuted,
                    border: Border(top: BorderSide(color: surfaces.border)),
                  ),
                  child: Row(
                    children: [
                      Expanded(
                        child: Text(
                          '7 günde çıkacak',
                          style: theme.textTheme.bodyMedium?.copyWith(
                            fontWeight: FontWeight.w600,
                            height: 1.3,
                          ),
                        ),
                      ),
                      AppMoneyText(
                        amount: total!,
                        currency: currency,
                        effect: AppMoneyEffect.expense,
                        size: AppMoneySize.row,
                      ),
                    ],
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }

  String _relative(String isoDate) {
    final due = DateTime.tryParse(isoDate);
    if (due == null) return DateText.dayMonth(isoDate);
    final start = DateTime(today.year, today.month, today.day);
    final days = DateTime(
      due.year,
      due.month,
      due.day,
    ).difference(start).inDays;
    return switch (days) {
      <= 0 => 'Bugün',
      1 => 'Yarın',
      _ => '$days gün sonra',
    };
  }
}

class _TimelineRow extends StatelessWidget {
  const _TimelineRow({
    required this.item,
    required this.first,
    required this.last,
    required this.relative,
  });

  final PlannedActivity item;
  final bool first;
  final bool last;
  final String relative;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final due = DateTime.tryParse(item.dueDate);
    final leaf = due == null ? null : AppDateLeaf.fromDate(due);
    final large = context.usesLargeText;
    final amount = item.amount == null
        ? const AppUnknownAmount()
        : AppMoneyText(
            amount: item.amount!,
            currency: item.currency,
            effect: AppMoneyEffect.expense,
            size: AppMoneySize.row,
          );

    return Semantics(
      container: true,
      label: _upcomingSentence(item, relative),
      child: ExcludeSemantics(
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 60),
          child: IntrinsicHeight(
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                SizedBox(
                  width: 40,
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        due == null ? '' : '${due.day}',
                        style: TextStyle(
                          fontSize: 18,
                          height: 1.1,
                          fontWeight: FontWeight.w700,
                          fontFeatures: AppTypography.tabularFigures,
                          color: surfaces.ink,
                        ),
                      ),
                      Text(
                        leaf?.month ?? '',
                        style: TextStyle(
                          fontSize: 12,
                          height: 1.2,
                          fontWeight: FontWeight.w600,
                          color: surfaces.inkMuted,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: AppSpacing.small),
                SizedBox(
                  width: 16,
                  child: CustomPaint(
                    painter: _TimelinePainter(
                      first: first,
                      last: last,
                      line: surfaces.border,
                      ink: surfaces.ink,
                      fill: surfaces.card,
                    ),
                  ),
                ),
                const SizedBox(width: AppSpacing.small),
                Expanded(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        item.title,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.titleSmall,
                      ),
                      const SizedBox(height: AppSpacing.xxSmall),
                      Text(
                        '$relative · ${item.plannedKind.label}',
                        maxLines: large ? 2 : 1,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.bodySmall,
                      ),
                      if (large)
                        Align(alignment: Alignment.centerRight, child: amount),
                    ],
                  ),
                ),
                if (!large) ...[
                  const SizedBox(width: AppSpacing.small),
                  Center(child: amount),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Zaman çizelgesinin dikey çizgisi ve noktası; ilk nokta dolu.
class _TimelinePainter extends CustomPainter {
  const _TimelinePainter({
    required this.first,
    required this.last,
    required this.line,
    required this.ink,
    required this.fill,
  });

  final bool first;
  final bool last;
  final Color line;
  final Color ink;
  final Color fill;

  @override
  void paint(Canvas canvas, Size size) {
    final centerX = size.width / 2;
    final centerY = size.height / 2;
    final linePaint = Paint()
      ..color = line
      ..strokeWidth = 2;
    canvas.drawLine(
      Offset(centerX, first ? centerY : 0),
      Offset(centerX, last ? centerY : size.height),
      linePaint,
    );
    canvas.drawCircle(Offset(centerX, centerY), 5, Paint()..color = ink);
    if (!first) {
      canvas.drawCircle(Offset(centerX, centerY), 3, Paint()..color = fill);
    }
  }

  @override
  bool shouldRepaint(covariant _TimelinePainter oldDelegate) =>
      oldDelegate.first != first ||
      oldDelegate.last != last ||
      oldDelegate.line != line ||
      oldDelegate.ink != ink ||
      oldDelegate.fill != fill;
}

/// Varlık durumu: net varlık, varlık/borç oranı çubuğu ve iki taraf.
///
/// Kalemler sunucudan gelir; iki tarafın toplamı da (`totalAssets`,
/// `totalLiabilities`). Sıfır olan kalem çizilmez, likit varlık hep durur.
/// Renk yönü söyler: varlık tarafı nötr mavi, borç tarafı gider kırmızısı.
class DashboardNetWorthCard extends StatelessWidget {
  const DashboardNetWorthCard({
    required this.report,
    super.key,
    this.today,
    this.onOpenTransit,
  });

  final AdvancedReport report;

  /// Yoldaki paranın beklenen günü bununla karşılaştırılır; verilmezse gün
  /// geçmiş sayılmaz.
  final DateTime? today;

  /// `Yolda` satırına dokununca; verilirse satır ok taşır.
  final VoidCallback? onOpenTransit;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final currency = report.currency;
    final cardCredit = MoneyText.isNegative(report.creditCardDebt);
    final assets = report.totalAssets;
    final liabilities = report.totalLiabilities;
    final transitOverdue = _isBefore(report.nextTransitDate, today);

    final assetLines = <_NetLine>[
      _NetLine(
        icon: Icons.savings_outlined,
        label: 'Likit varlık',
        subtitle: 'Kasa ve banka',
        amount: report.liquidAssets,
      ),
      if (_hasAmount(report.moneyInTransit))
        _NetLine(
          icon: Icons.schedule_outlined,
          label: 'Yolda',
          subtitle: 'Kartla gelecek',
          amount: report.moneyInTransit,
          // Gün her zaman yazılır; geçmişse Kasa'daki gecikme işaretiyle
          // (ikon, söz ve gider tonu) gösterilir.
          when: report.nextTransitDate == null
              ? null
              : transitOverdue
              ? '${DateText.dayMonth(report.nextTransitDate!)} · Gecikti'
              : DateText.dayMonth(report.nextTransitDate!),
          whenOverdue: transitOverdue,
          onTap: onOpenTransit,
        ),
      if (_hasAmount(report.receivableDebt))
        _NetLine(
          icon: Icons.handshake_outlined,
          label: 'Alacak',
          subtitle: 'faiz hariç',
          amount: report.receivableDebt,
        ),
      // Fazla ödenmiş kartta duran para kullanıcınındır ve varlıktır.
      if (cardCredit)
        _NetLine(
          icon: Icons.credit_card,
          label: 'Kart alacağı',
          subtitle: 'Fazla ödeme',
          amount: MoneyText.unsigned(report.creditCardDebt),
        ),
    ];
    final liabilityLines = <_NetLine>[
      if (!cardCredit && _hasAmount(report.creditCardDebt))
        _NetLine(
          icon: Icons.credit_card,
          label: 'Kart borcu',
          subtitle: 'Dönem ekstresi dahil',
          amount: report.creditCardDebt,
        ),
      if (_hasAmount(report.payableDebt))
        _NetLine(
          icon: Icons.account_balance_outlined,
          label: 'Borç',
          subtitle: 'faiz hariç',
          amount: report.payableDebt,
        ),
    ];

    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          MergeSemantics(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Net varlık',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w500,
                    height: 1.3,
                    color: surfaces.inkMuted,
                  ),
                ),
                const SizedBox(height: AppSpacing.xSmall),
                AppMoneyText(
                  amount: report.netWorth,
                  currency: currency,
                  size: AppMoneySize.metric,
                ),
              ],
            ),
          ),
          if (assets != null && liabilities != null) ...[
            const SizedBox(height: AppSpacing.medium),
            _BalanceBar(
              assets: _value(assets),
              liabilities: _value(liabilities),
              assetColor: colors.neutralFill,
              liabilityColor: colors.expenseFill,
            ),
          ],
          const SizedBox(height: AppSpacing.medium),
          _NetSide(
            title: 'Varlıklar',
            total: assets,
            currency: currency,
            tone: AppStatusTone.neutral,
            lines: assetLines,
          ),
          if (liabilityLines.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.small),
            Divider(height: 1, thickness: 1, color: surfaces.border),
            const SizedBox(height: AppSpacing.medium),
            _NetSide(
              title: 'Borçlar',
              total: liabilities,
              currency: currency,
              tone: AppStatusTone.expense,
              lines: liabilityLines,
            ),
          ],
        ],
      ),
    );
  }

  /// Tutarın gösterilecek bir değeri var mı; para aritmetiği değil, varlık
  /// kontrolü.
  static bool _hasAmount(String amount) => double.tryParse(amount) != 0;

  static double _value(String amount) => (double.tryParse(amount) ?? 0).abs();

  /// `yyyy-MM-dd` günü [today]'den önce mi; saat dikkate alınmaz.
  static bool _isBefore(String? date, DateTime? today) {
    final parsed = date == null ? null : DateTime.tryParse(date);
    if (parsed == null || today == null) return false;
    return parsed.isBefore(DateTime(today.year, today.month, today.day));
  }
}

/// Varlık ve borç tarafının oranı; yalnız çizim, sayı taşımaz.
class _BalanceBar extends StatelessWidget {
  const _BalanceBar({
    required this.assets,
    required this.liabilities,
    required this.assetColor,
    required this.liabilityColor,
  });

  final double assets;
  final double liabilities;
  final Color assetColor;
  final Color liabilityColor;

  @override
  Widget build(BuildContext context) {
    final total = assets + liabilities;
    if (total <= 0) return const SizedBox.shrink();
    final assetFlex = math.max(1, (assets / total * 1000).round());
    final liabilityFlex = (liabilities / total * 1000).round();
    return ExcludeSemantics(
      child: ClipRRect(
        borderRadius: BorderRadius.circular(AppRadius.chip),
        child: SizedBox(
          height: 10,
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                flex: assetFlex,
                child: ColoredBox(color: assetColor),
              ),
              if (liabilityFlex > 0) ...[
                const SizedBox(width: AppSpacing.xxSmall),
                Expanded(
                  flex: liabilityFlex,
                  child: ColoredBox(color: liabilityColor),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

class _NetLine {
  const _NetLine({
    required this.icon,
    required this.label,
    required this.subtitle,
    required this.amount,
    this.when,
    this.whenOverdue = false,
    this.onTap,
  });

  final IconData icon;
  final String label;
  final String subtitle;
  final String amount;
  final String? when;
  final bool whenOverdue;
  final VoidCallback? onTap;
}

class _NetSide extends StatelessWidget {
  const _NetSide({
    required this.title,
    required this.total,
    required this.currency,
    required this.tone,
    required this.lines,
  });

  final String title;
  final String? total;
  final String currency;
  final AppStatusTone tone;
  final List<_NetLine> lines;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final expense = tone == AppStatusTone.expense;
    final effect = expense ? AppMoneyEffect.expense : AppMoneyEffect.neutral;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        MergeSemantics(
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
            child: Row(
              children: [
                Container(
                  width: 10,
                  height: 10,
                  decoration: BoxDecoration(
                    color: expense ? colors.expenseFill : colors.neutralFill,
                    shape: BoxShape.circle,
                  ),
                ),
                const SizedBox(width: AppSpacing.small),
                Expanded(child: Text(title, style: theme.textTheme.titleSmall)),
                if (total != null)
                  AppMoneyText(
                    amount: total!,
                    currency: currency,
                    size: AppMoneySize.row,
                  ),
              ],
            ),
          ),
        ),
        for (final line in lines)
          AppRow(
            padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
            leading: AppIconCapsule(icon: line.icon, tone: tone),
            title: line.label,
            subtitle: line.subtitle,
            onTap: line.onTap,
            // Renk bir konuşma kanalı değildir: satırın net varlıktaki rolü
            // ekran okuyucuya cümleyle söylenir.
            semanticLabel:
                '${line.label}: ${MoneyText.format(line.amount, currency)}, '
                '${expense ? 'net varlığı düşürür' : 'net varlığa eklenir'}, '
                '${line.subtitle}'
                '${line.when == null ? '' : ', hesaba geçecek ${line.when}'}',
            // Sağ taraf satırın yarısından fazlasını alamaz; alırsa başlık
            // sıkışır. Sığmayan gün yazısı kısalır.
            trailing: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 200),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  // Esnek: büyük yazıda tutar satıra sığmazsa daralabilmeli.
                  Flexible(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        AppMoneyText(
                          amount: line.amount,
                          currency: currency,
                          effect: effect,
                          size: AppMoneySize.row,
                        ),
                        if (line.when != null)
                          Padding(
                            padding: const EdgeInsets.only(
                              top: AppSpacing.xxSmall,
                            ),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Icon(
                                  line.whenOverdue
                                      ? Icons.warning_amber_rounded
                                      : Icons.event_outlined,
                                  size: 14,
                                  color: line.whenOverdue
                                      ? colors.expense
                                      : surfaces.inkMuted,
                                ),
                                const SizedBox(width: AppSpacing.xSmall),
                                Flexible(
                                  child: Text(
                                    line.when!,
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                    style: theme.textTheme.labelSmall?.copyWith(
                                      fontSize: 12,
                                      fontWeight: line.whenOverdue
                                          ? FontWeight.w500
                                          : FontWeight.w400,
                                      letterSpacing: 0,
                                      color: line.whenOverdue
                                          ? colors.expense
                                          : surfaces.inkMuted,
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ),
                      ],
                    ),
                  ),
                  if (line.onTap != null) ...[
                    const SizedBox(width: AppSpacing.xSmall),
                    Icon(Icons.chevron_right, color: surfaces.inkMuted),
                  ],
                ],
              ),
            ),
          ),
      ],
    );
  }
}

/// Hesap bakiyeleri: kapsüllü satırlar, ayırıcı yazının başladığı yerden.
class DashboardAccountBalances extends StatelessWidget {
  const DashboardAccountBalances({
    required this.report,
    required this.onOpen,
    super.key,
  });

  final DashboardReport report;
  final VoidCallback onOpen;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final items = report.accountBalances;
    if (items.isEmpty) {
      return AppCard(
        onTap: onOpen,
        child: Text('Henüz hesap yok.', style: theme.textTheme.bodySmall),
      );
    }
    return AppCard(
      padding: EdgeInsets.zero,
      child: AppDividedColumn(
        inset: AppIconCapsule.rowInset,
        children: [
          for (final item in items)
            AppRow(
              onTap: onOpen,
              // Nakit ile banka hesabı ayrı ikon taşır; tür sunucudan gelir.
              leading: AppIconCapsule(
                icon: AppFinanceIcons.forAccountType(item.type),
              ),
              title: item.accountName,
              subtitle: _typeLabel(item.type),
              trailing: AppMoneyText(
                amount: item.balance,
                currency: report.currency,
                size: AppMoneySize.row,
              ),
            ),
        ],
      ),
    );
  }

  static String? _typeLabel(String? type) => switch (type) {
    'cash' => 'Nakit',
    'bank' => 'Banka hesabı',
    _ => null,
  };
}

/// Halkanın ekran okuyucu cümlesi.
String _ringSentence(BudgetVarianceItem item, String currency) {
  final difference = MoneyText.format(
    MoneyText.unsigned(item.remaining),
    currency,
  );
  return '${item.categoryName}: yüzde ${(item.spentRatio * 100).round()}, '
      '$difference ${item.isExceeded ? 'fazla, limit aşıldı' : 'kaldı'}';
}

/// Yaklaşan satırın ekran okuyucu cümlesi.
String _upcomingSentence(PlannedActivity item, String relative) =>
    '${item.title}: ${_amountSentence(item)}, '
    'vadesi ${DateText.dayMonth(item.dueDate)}, $relative, '
    '${item.plannedKind.label}';

/// Dokununca başka ekrana giden özet kartı: ekran okuyucuya tek durak,
/// içeriğin cümleleri etikette, eylem aynı düğümde.
class _TappableSummary extends StatelessWidget {
  const _TappableSummary({
    required this.label,
    required this.onTap,
    required this.child,
  });

  final String label;
  final VoidCallback onTap;
  final Widget child;

  @override
  Widget build(BuildContext context) => Semantics(
    container: true,
    button: true,
    label: label,
    onTap: onTap,
    child: ExcludeSemantics(child: child),
  );
}

String _amountSentence(PlannedActivity item) => item.amount == null
    ? AppUnknownAmount.text
    : MoneyText.format(item.amount!, item.currency);

String _overdueSentence(
  int count,
  String oldest,
  String? total,
  String currency,
) {
  final amount = total == null ? '' : ', ${MoneyText.format(total, currency)}';
  return '$count gecikmiş ödeme, en eskisi ${DateText.dayMonth(oldest)}$amount';
}

/// Yaklaşanlar kartının başındaki gecikenler satırı: gider zemini, sayı, en
/// eski vade ve toplam. Ok yok: kartın tamamı zaten Planlananlar'ı açıyor.
class _OverdueRow extends StatelessWidget {
  const _OverdueRow({
    required this.count,
    required this.oldest,
    required this.currency,
    this.total,
  });

  final int count;
  final String oldest;
  final String currency;
  final String? total;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final foreground = colors.onExpenseContainer;
    return Container(
      constraints: const BoxConstraints(minHeight: 64),
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.small + AppSpacing.xSmall,
      ),
      decoration: BoxDecoration(
        color: colors.expenseContainer,
        border: Border(bottom: BorderSide(color: surfaces.border)),
      ),
      child: Row(
        children: [
          Icon(Icons.error_outline, size: 22, color: foreground),
          const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  '$count gecikmiş ödeme',
                  style: theme.textTheme.titleSmall?.copyWith(
                    color: foreground,
                  ),
                ),
                const SizedBox(height: AppSpacing.xxSmall),
                Text(
                  'En eskisi ${DateText.dayMonth(oldest)}',
                  style: theme.textTheme.bodySmall?.copyWith(color: foreground),
                ),
              ],
            ),
          ),
          if (total != null) ...[
            const SizedBox(width: AppSpacing.small),
            AppMoneyText(
              amount: total!,
              currency: currency,
              effect: AppMoneyEffect.expense,
              onContainer: true,
              size: AppMoneySize.row,
            ),
          ],
        ],
      ),
    );
  }
}
