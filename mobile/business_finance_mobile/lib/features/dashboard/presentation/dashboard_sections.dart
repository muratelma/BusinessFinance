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
import '../../../core/widgets/app_donut_chart.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_share_bar.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_unknown_amount.dart';
import '../../activities/data/planned_activity_models.dart';
import '../../planning/data/planning_models.dart';
import 'package:provider/provider.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/widgets/app_account_field.dart';
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
  const DashboardCategoryCard({required this.report, this.onOpen, super.key});

  final DashboardReport report;
  final VoidCallback? onOpen;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final slices = report.categoryExpenseSlices;
    if (slices.isEmpty) {
      return AppCard(
        onTap: onOpen,
        child: Text(
          'Bu ay kategori gideri yok.',
          style: theme.textTheme.bodySmall,
        ),
      );
    }
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
        // Toplamın altı: önceki aya göre fark. Karşılaştırma yoksa (önceki
        // ayda gider yok, eski sunucu) en büyük pay yazılır. İkisi aynı
        // yeri kaplar; kart uzamaz.
        if (report.expenseComparison case final c?) ...[
          const SizedBox(height: AppSpacing.xSmall),
          Divider(height: 1, thickness: 1, color: surfaces.border),
          const SizedBox(height: AppSpacing.small),
          Text(
            c.throughToday ? 'Geçen ayın aynı gününe göre' : 'Önceki aya göre',
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: theme.textTheme.labelMedium?.copyWith(
              fontWeight: FontWeight.w400,
              letterSpacing: 0,
              color: surfaces.inkMuted,
            ),
          ),
          const SizedBox(height: AppSpacing.xxSmall),
          _ChangeText(change: c.change, currency: report.currency),
        ] else if (top != null) ...[
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
            '${top.categoryName} · %${MoneyText.percent(top.share)}',
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
      onTap: onOpen,
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
                      value: slices[i].barRatio,
                      color: _sliceColor(context, slices, i),
                      formattedValue: MoneyText.format(
                        slices[i].amount,
                        report.currency,
                      ),
                    ),
                ],
              ),
              const SizedBox(width: AppSpacing.step20),
              Expanded(child: summary),
            ],
          ),
          const SizedBox(height: AppSpacing.large),
          for (var i = 0; i < slices.length; i++) ...[
            if (i > 0) const SizedBox(height: AppSpacing.compact),
            _CategoryLine(
              slice: slices[i],
              currency: report.currency,
              color: _sliceColor(context, slices, i),
              ratio: slices[i].barRatio,
              percent: MoneyText.percent(slices[i].share),
            ),
          ],
        ],
      ),
    );
  }

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
  final String percent;

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
            const SizedBox(width: AppSpacing.compact),
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
                  const SizedBox(height: AppSpacing.step6),
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

/// Önceki aya göre fark: ok, tutar ve `fazla` / `az` kelimesi.
/// Gider artışı gider renginde, azalışı gelir renginde; yön renkle
/// birlikte okla ve kelimeyle de söylenir.
class _ChangeText extends StatelessWidget {
  const _ChangeText({required this.change, required this.currency});

  final String change;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final less = MoneyText.isNegative(change);
    final color = less ? colors.income : colors.expense;
    final amount = MoneyText.format(MoneyText.unsigned(change), currency);
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(
          less ? Icons.arrow_downward : Icons.arrow_upward,
          size: AppTextSizes.caption,
          color: color,
        ),
        const SizedBox(width: AppSpacing.xxSmall),
        Flexible(
          child: Text(
            '$amount ${less ? 'az' : 'fazla'}',
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: theme.textTheme.labelMedium?.copyWith(
              fontWeight: FontWeight.w600,
              letterSpacing: 0,
              fontFeatures: AppTypography.tabularFigures,
              color: color,
            ),
          ),
        ),
      ],
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
    final columns = context.usesLargeText ? 1 : math.min(visible.length, 2);
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
        padding: const EdgeInsets.all(AppSpacing.small),
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
                          single: visible.length == 1,
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
  const _BudgetRing({
    required this.item,
    required this.currency,
    required this.single,
  });

  final BudgetVarianceItem item;
  final String currency;
  final bool single;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final over = item.isExceeded;
    final ink = over ? colors.onExpenseContainer : surfaces.ink;
    final style = theme.textTheme.labelLarge?.copyWith(
      color: ink,
      fontWeight: FontWeight.w600,
    );
    final pct = '%${(item.spentRatio * 100).round()}';
    final title = Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(
          AppFinanceIcons.forCategory(
            item.canonicalName,
            displayName: item.categoryName,
          ),
          color: ink,
          size: AppTextSizes.section,
        ),
        const SizedBox(width: AppSpacing.xSmall),
        Expanded(
          child: Text(
            item.categoryName,
            style: style?.copyWith(fontSize: AppTextSizes.compact),
          ),
        ),
      ],
    );
    // Halka 60 dp, çizgi 6 dp: yüzde içeride nefes alır, halka kutunun
    // ağırlığını taşır. Yüzde tutardan küçüktür; kutunun asıl sayısı tutardır.
    final ring = SizedBox.square(
      dimension: AppSizes.ring,
      child: Stack(
        alignment: Alignment.center,
        children: [
          SizedBox.expand(
            child: CircularProgressIndicator(
              value: item.spentRatio.clamp(0.0, 1.0),
              strokeWidth: AppSizes.ringStroke,
              strokeCap: StrokeCap.round,
              backgroundColor: surfaces.card,
              color: over ? colors.expenseFill : colors.neutralFill,
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(AppSpacing.small),
            child: FittedBox(
              fit: BoxFit.scaleDown,
              child: Text(
                pct,
                style: TextStyle(
                  fontSize: AppTextSizes.compact,
                  fontWeight: FontWeight.w700,
                  fontFeatures: AppTypography.tabularFigures,
                  color: ink,
                ),
              ),
            ),
          ),
        ],
      ),
    );
    final label = Text(
      over ? 'Aşım' : 'Kalan',
      style: theme.textTheme.bodySmall?.copyWith(
        fontSize: AppTextSizes.body,
        color: over ? ink : surfaces.inkMuted,
      ),
    );
    final money = Text(
      MoneyText.format(MoneyText.unsigned(item.remaining), currency),
      maxLines: 1,
      style: TextStyle(
        fontSize: AppTextSizes.row,
        fontWeight: FontWeight.w700,
        fontFeatures: AppTypography.tabularFigures,
        color: ink,
      ),
    );
    final value = single
        ? Wrap(
            spacing: AppSpacing.small,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [label, money],
          )
        : IntrinsicWidth(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                label,
                const SizedBox(height: AppSpacing.xxSmall),
                Divider(
                  color: (over ? ink : surfaces.inkMuted).withValues(
                    alpha: 0.4,
                  ),
                  thickness: 1,
                  height: 1,
                ),
                const SizedBox(height: AppSpacing.xSmall),
                FittedBox(
                  fit: BoxFit.scaleDown,
                  alignment: Alignment.centerLeft,
                  child: money,
                ),
              ],
            ),
          );
    final amountRow = Row(
      children: [
        ring,
        const SizedBox(width: AppSpacing.compact),
        Flexible(child: value),
      ],
    );
    return Container(
      padding: const EdgeInsets.all(AppSpacing.compact),
      decoration: BoxDecoration(
        color: over ? colors.expenseContainer : surfaces.cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
      child: single
          ? Row(
              children: [
                ring,
                const SizedBox(width: AppSpacing.medium),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      title,
                      const SizedBox(height: AppSpacing.xSmall),
                      value,
                    ],
                  ),
                ),
              ],
            )
          : Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                title,
                const Spacer(),
                const SizedBox(height: AppSpacing.compact),
                amountRow,
              ],
            ),
    );
  }
}

/// Haftanın ilk dört vadesi; altta haftanın bütün ödemelerinin toplamı.
/// Ödeme adedi yalnız listede görünmeyen ödeme varsa yazılır.
///
/// İçerik kartın sol kenarına yakındır: tarih ve zaman çizgisi dar tutulur,
/// başlık ve tutara yer kalır. Alt satırda gün yalnız `Bugün` ve `Yarın`
/// için yazılır; diğer günlerin tarihi solda zaten yazılıdır. Dolu nokta
/// bugünün kaydıdır. Tahsilat ve karta yansıyan kalem kendi renginden
/// anlaşılır; altta toplanmaz (kullanıcı kararı, 22 Kasım 2026).
class DashboardUpcomingCard extends StatelessWidget {
  const DashboardUpcomingCard({
    required this.items,
    required this.total,
    required this.currency,
    required this.today,
    required this.onOpen,
    super.key,
    this.paymentCount,
    this.unknownCount,
  });

  final List<PlannedActivity> items;

  final int? paymentCount;
  final int? unknownCount;
  final String? total;
  final String currency;
  final DateTime today;
  final VoidCallback onOpen;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    if (items.isEmpty) {
      return AppCard(
        onTap: onOpen,
        child: Text('Bu hafta ödeme yok.', style: theme.textTheme.bodyMedium),
      );
    }
    // Ödeme adedi yalnız satırlar haftanın bütün ödemelerini göstermiyorsa
    // yazılır: dört satırda görünmeyen ödeme varsa. Sayı satır saymaktır,
    // tutar sunucunundur.
    final visiblePayments = items
        .where((item) => item.isPaymentObligation)
        .length;
    final footer = paymentCount == null || paymentCount! <= visiblePayments
        ? '7 günde ödenecek'
        : '7 günde ödenecek · $paymentCount ödeme';
    final unknown = (unknownCount ?? 0) > 0
        ? '$unknownCount ödemenin tutarı belli değil'
        : null;
    final hasKnownAmount = unknown == null || paymentCount != unknownCount;
    final showPayments = total != null && paymentCount != 0;
    final footerStyle = theme.textTheme.bodyMedium?.copyWith(
      fontWeight: FontWeight.w600,
      height: 1.3,
    );
    return _TappableSummary(
      onTap: onOpen,
      label: [
        for (final item in items)
          _upcomingSentence(item, _relative(item.dueDate)),
        if (showPayments) footer,
        if (showPayments && hasKnownAmount)
          'Bilinen tutar ${MoneyText.format(total!, currency)}',
        if (showPayments) ?unknown,
        'Planlananları açar',
      ].join('. '),
      child: AppCard(
        padding: EdgeInsets.zero,
        onTap: onOpen,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.compact,
                AppSpacing.small,
                AppSpacing.medium,
                AppSpacing.small,
              ),
              child: Column(
                children: [
                  for (var i = 0; i < items.length; i++)
                    _TimelineRow(
                      item: items[i],
                      first: i == 0,
                      last: i == items.length - 1,
                      isToday: _days(items[i].dueDate) <= 0,
                      near: _near(items[i].dueDate),
                      relative: _relative(items[i].dueDate),
                    ),
                ],
              ),
            ),
            if (showPayments)
              MergeSemantics(
                child: Container(
                  constraints: const BoxConstraints(minHeight: 48),
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                    vertical: AppSpacing.small,
                  ),
                  decoration: BoxDecoration(
                    color: surfaces.cardMuted,
                    border: Border(top: BorderSide(color: surfaces.border)),
                  ),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Row(
                        children: [
                          Expanded(
                            child: Text(
                              unknown == null ? footer : '$footer\n$unknown',
                              style: footerStyle,
                            ),
                          ),
                          if (hasKnownAmount)
                            Column(
                              crossAxisAlignment: CrossAxisAlignment.end,
                              children: [
                                if (unknown != null)
                                  Text(
                                    'Bilinen tutar',
                                    style: theme.textTheme.bodySmall,
                                  ),
                                AppMoneyText(
                                  amount: total!,
                                  currency: currency,
                                  size: AppMoneySize.row,
                                ),
                              ],
                            ),
                        ],
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

  int _days(String isoDate) {
    final due = DateTime.tryParse(isoDate);
    if (due == null) return 1;
    final start = DateTime(today.year, today.month, today.day);
    return DateTime(due.year, due.month, due.day).difference(start).inDays;
  }

  /// Alt satıra yazılan gün: yalnız bugün ve yarın.
  String? _near(String isoDate) => switch (_days(isoDate)) {
    <= 0 => 'Bugün',
    1 => 'Yarın',
    _ => null,
  };

  /// Ekran okuyucunun cümlesi her günü söyler.
  String _relative(String isoDate) {
    final due = DateTime.tryParse(isoDate);
    if (due == null) return DateText.dayMonth(isoDate);
    final days = _days(isoDate);
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
    required this.isToday,
    required this.near,
    required this.relative,
  });

  final PlannedActivity item;
  final bool first;
  final bool last;
  final bool isToday;
  final String? near;
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
            effect: item.isPaymentObligation
                ? AppMoneyEffect.expense
                : item.postsToCard ||
                      item.plannedKind == PlannedKind.cardInstallment
                ? AppMoneyEffect.neutral
                : AppMoneyEffect.income,
            size: AppMoneySize.row,
          );
    final subtitle = near == null
        ? item.paymentLabel
        : '$near · ${item.paymentLabel}';

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
                  width: 30,
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        due == null ? '' : '${due.day}',
                        style: TextStyle(
                          fontSize: AppTextSizes.section,
                          height: 1.1,
                          fontWeight: FontWeight.w700,
                          fontFeatures: AppTypography.tabularFigures,
                          color: surfaces.ink,
                        ),
                      ),
                      Text(
                        leaf?.month ?? '',
                        style: TextStyle(
                          fontSize: AppTextSizes.micro,
                          height: 1.2,
                          fontWeight: FontWeight.w600,
                          color: surfaces.inkMuted,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: AppSpacing.xSmall),
                SizedBox(
                  width: 16,
                  child: CustomPaint(
                    painter: _TimelinePainter(
                      first: first,
                      last: last,
                      filled: isToday,
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
                        item.displayTitle,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.titleSmall,
                      ),
                      const SizedBox(height: AppGaps.rowText),
                      Text(
                        subtitle,
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

/// Zaman çizelgesinin dikey çizgisi ve noktası; bugünün kaydı dolu nokta.
class _TimelinePainter extends CustomPainter {
  const _TimelinePainter({
    required this.first,
    required this.last,
    required this.filled,
    required this.line,
    required this.ink,
    required this.fill,
  });

  final bool first;
  final bool last;
  final bool filled;
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
    if (!filled) {
      canvas.drawCircle(Offset(centerX, centerY), 3, Paint()..color = fill);
    }
  }

  @override
  bool shouldRepaint(covariant _TimelinePainter oldDelegate) =>
      oldDelegate.first != first ||
      oldDelegate.last != last ||
      oldDelegate.filled != filled ||
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
    this.showScopeNote = false,
  });

  final AdvancedReport report;

  /// Yoldaki paranın beklenen günü bununla karşılaştırılır; verilmezse gün
  /// geçmiş sayılmaz.
  final DateTime? today;

  /// `Yolda` satırına dokununca; verilirse satır ok taşır.
  final VoidCallback? onOpenTransit;
  final bool showScopeNote;

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
        label: 'Kasa ve banka',
        subtitle: '',
        amount: report.liquidAssets,
      ),
      if (_hasAmount(report.moneyInTransit))
        _NetLine(
          icon: Icons.schedule_outlined,
          label: 'POS yatışları',
          subtitle: transitOverdue ? 'Geciken yatış var' : '',
          amount: report.moneyInTransit,
          // Yoldaki para henüz hesapta değildir; uygulamanın her yerinde mavi.
          effect: AppMoneyEffect.neutral,
          whenOverdue: transitOverdue,
          onTap: onOpenTransit,
        ),
      if (_hasAmount(report.receivableDebt))
        _NetLine(
          icon: Icons.handshake_outlined,
          label: 'Alacaklar',
          subtitle: 'Faiz hariç',
          amount: report.receivableDebt,
        ),
      // Fazla ödenmiş kartta duran para kullanıcınındır ve varlıktır.
      if (cardCredit)
        _NetLine(
          icon: Icons.credit_card,
          label: 'Kartta fazla ödeme',
          subtitle: '',
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
          subtitle: 'Faiz hariç',
          amount: report.payableDebt,
        ),
    ];

    Widget summaryTotal(
      String label,
      String amount,
      Color color,
      CrossAxisAlignment align,
    ) => Column(
      crossAxisAlignment: align,
      children: [
        Wrap(
          crossAxisAlignment: WrapCrossAlignment.center,
          spacing: AppSpacing.xSmall,
          children: [
            DecoratedBox(
              decoration: BoxDecoration(color: color, shape: BoxShape.circle),
              child: const SizedBox.square(dimension: AppSpacing.small),
            ),
            Text(label, style: theme.textTheme.bodySmall),
          ],
        ),
        AppMoneyText(
          amount: amount,
          currency: currency,
          size: AppMoneySize.row,
        ),
      ],
    );
    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          ColoredBox(
            color: surfaces.cardMuted,
            child: Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.compact,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Net varlık', style: theme.textTheme.titleSmall),
                  AppMoneyText(
                    amount: report.netWorth,
                    currency: currency,
                    size: AppMoneySize.metric,
                  ),
                  if (showScopeNote)
                    Text(
                      'İşletme ve şahsi toplamı',
                      style: theme.textTheme.bodySmall,
                    ),
                  if (assets != null && liabilities != null) ...[
                    const SizedBox(height: AppSpacing.small),
                    _BalanceBar(
                      assets: _value(assets),
                      liabilities: _value(liabilities),
                      assetColor: colors.neutralFill,
                      liabilityColor: colors.expenseFill,
                    ),
                    const SizedBox(height: AppSpacing.small),
                    Row(
                      children: [
                        Expanded(
                          child: summaryTotal(
                            'Varlıklar',
                            assets,
                            colors.neutral,
                            CrossAxisAlignment.start,
                          ),
                        ),
                        Expanded(
                          child: summaryTotal(
                            'Borçlar',
                            liabilities,
                            colors.expense,
                            CrossAxisAlignment.end,
                          ),
                        ),
                      ],
                    ),
                  ],
                ],
              ),
            ),
          ),
          for (final (index, line) in [
            ...assetLines,
            ...liabilityLines,
          ].indexed) ...[
            Semantics(
              container: true,
              label:
                  '${line.label}: ${MoneyText.format(line.amount, currency)}, ${liabilityLines.contains(line) ? 'net varlığı düşürür' : 'net varlığa eklenir'}. ${line.subtitle}',
              button: line.onTap != null,
              onTap: line.onTap,
              child: ExcludeSemantics(
                child: InkWell(
                  onTap: line.onTap,
                  child: Container(
                    constraints: const BoxConstraints(
                      minHeight: AppSizes.button,
                    ),
                    padding:
                        const EdgeInsets.symmetric(
                          horizontal: AppSpacing.medium,
                          vertical: AppSpacing.step6,
                        ).copyWith(
                          // İlk satır üst yüzeyden, son satır kartın altından
                          // biraz daha uzak durur.
                          top: index == 0 ? AppSpacing.step10 : null,
                          bottom:
                              index ==
                                  assetLines.length + liabilityLines.length - 1
                              ? AppSpacing.step10
                              : null,
                        ),
                    // Aynı grubun satırları arasında simgeden sonra başlayan
                    // çizgi; varlıklarla borçlar arasında aynı renkte, boydan
                    // boya çizgi (ayrımı uzunluk söyler, koyuluk değil).
                    foregroundDecoration: _RowDivider(
                      color: surfaces.inkMuted.withValues(alpha: 0.22),
                      inset: index == 0 || index == assetLines.length
                          ? 0
                          : AppSizes.menuButton + AppSpacing.compact,
                    ),
                    child: Row(
                      children: [
                        AppIconCapsule(
                          icon: line.icon,
                          size: AppSizes.menuButton,
                          tone: liabilityLines.contains(line)
                              ? AppStatusTone.expense
                              : AppStatusTone.neutral,
                        ),
                        const SizedBox(width: AppSpacing.compact),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  Flexible(
                                    child: Text(
                                      line.label,
                                      style: theme.textTheme.bodyMedium
                                          ?.copyWith(
                                            fontSize: AppTextSizes.compact,
                                          ),
                                    ),
                                  ),
                                  if (line.onTap != null)
                                    Icon(
                                      Icons.chevron_right,
                                      size: AppTextSizes.row,
                                      color: surfaces.inkMuted,
                                    ),
                                ],
                              ),
                              if (line.subtitle.isNotEmpty)
                                Text(
                                  line.subtitle,
                                  style: theme.textTheme.bodySmall?.copyWith(
                                    color: line.whenOverdue
                                        ? colors.expense
                                        : surfaces.inkMuted,
                                    fontSize: AppTextSizes.body,
                                  ),
                                ),
                            ],
                          ),
                        ),
                        const SizedBox(width: AppSpacing.small),
                        Flexible(
                          fit: FlexFit.loose,
                          child: Align(
                            alignment: Alignment.centerRight,
                            child: AppMoneyText(
                              amount: line.amount,
                              currency: currency,
                              effect: line.effect,
                              size: AppMoneySize.row,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
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
          height: AppSpacing.xSmall,
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

/// Satırın üst çizgisi; [inset] soldan, kartın iç kenarından sonra başlar.
class _RowDivider extends Decoration {
  const _RowDivider({required this.color, required this.inset});

  final Color color;
  final double inset;

  @override
  BoxPainter createBoxPainter([VoidCallback? onChanged]) =>
      _RowDividerPainter(this);
}

class _RowDividerPainter extends BoxPainter {
  _RowDividerPainter(this.decoration);

  final _RowDivider decoration;

  @override
  void paint(Canvas canvas, Offset offset, ImageConfiguration configuration) {
    final size = configuration.size;
    if (size == null) return;
    final start = decoration.inset == 0
        ? 0.0
        : AppSpacing.medium + decoration.inset;
    final end = decoration.inset == 0
        ? size.width
        : size.width - AppSpacing.medium;
    // Çizgi ekranın piksel ızgarasına oturtulur ve tam piksel kalınlığında
    // çizilir: 2,625 gibi bir piksel oranında 1 dp'lik çizgi satırın yerine
    // göre kimi yerde keskin, kimi yerde bulanık ve soluk görünüyordu.
    final ratio = configuration.devicePixelRatio ?? 1;
    final thickness = math.max(1, ratio.round()) / ratio;
    final top = (offset.dy * ratio).round() / ratio;
    canvas.drawRect(
      Rect.fromLTWH(offset.dx + start, top, end - start, thickness),
      Paint()..color = decoration.color,
    );
  }
}

class _NetLine {
  const _NetLine({
    required this.icon,
    required this.label,
    required this.subtitle,
    required this.amount,
    this.whenOverdue = false,
    this.effect,
    this.onTap,
  });

  final IconData icon;
  final String label;
  final String subtitle;
  final String amount;
  final bool whenOverdue;
  final AppMoneyEffect? effect;
  final VoidCallback? onTap;
}

/// Hesap bakiyeleri (Claude Design V1): tek kart, içinde `Nakit` ve `Banka`
/// grupları. Grup satırı renkli nokta ve ad taşır, toplam taşımaz; gruplar
/// arasında çizgi, satırlar arasında çizgi yoktur. Satırda türün renginde
/// simge, ad, kapsam ve bakiye. Grup içi sıra bakiyeye göre büyükten
/// küçüğedir; pasif hesap çizilmez.
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
    final items = report.accountBalances.where((item) => item.isActive);
    if (items.isEmpty) {
      return AppCard(
        onTap: onOpen,
        child: Text('Henüz hesap yok.', style: theme.textTheme.bodySmall),
      );
    }
    final showScope = _scopeVisible(context);
    // Özet en çok [visibleCount] hesap gösterir: tutarı (eksi ya da artı)
    // en büyük olanlar; tamamı başlıktaki sayıdan Hesaplar'da açılır.
    final shown =
        (items.toList()..sort(
              (a, b) => (MoneyMath.parse(b.balance) ?? BigInt.zero)
                  .abs()
                  .compareTo((MoneyMath.parse(a.balance) ?? BigInt.zero).abs()),
            ))
            .take(visibleCount)
            .toList();
    final groups = [
      for (final kind in [AppAccountKind.cash, AppAccountKind.bank])
        (
          kind: kind,
          items: shown.where((item) => _kindOf(item) == kind).toList()
            // Yalnız sıralama; tutar sunucunundur.
            ..sort(
              (a, b) => (MoneyMath.parse(b.balance) ?? BigInt.zero).compareTo(
                MoneyMath.parse(a.balance) ?? BigInt.zero,
              ),
            ),
        ),
    ].where((group) => group.items.isNotEmpty).toList();
    // Teslimle kurulan bölüm: metin harf aralıksız.
    return AppCard(
      padding: const EdgeInsets.only(bottom: AppSpacing.step6),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          for (var g = 0; g < groups.length; g++) ...[
            if (g > 0) ...[
              const SizedBox(height: AppSpacing.step6),
              const Divider(height: 1),
            ],
            _GroupRow(kind: groups[g].kind),
            for (final item in groups[g].items)
              _AccountRow(
                item: item,
                kind: groups[g].kind,
                currency: report.currency,
                showScope: showScope,
                onTap: onOpen,
              ),
          ],
        ],
      ),
    );
  }

  /// Özet'te gösterilen en fazla hesap sayısı.
  static const visibleCount = 5;

  /// Başlıktaki sayı: pasif hesap sayılmaz.
  static int activeCount(DashboardReport report) =>
      report.accountBalances.where((item) => item.isActive).length;

  static AppAccountKind _kindOf(AccountBalance item) =>
      item.type == 'cash' ? AppAccountKind.cash : AppAccountKind.bank;

  static bool _scopeVisible(BuildContext context) {
    try {
      return context.watch<ScopeController?>()?.isVisible ?? false;
    } on ProviderNotFoundException {
      return false;
    }
  }
}

/// Grup satırı: 8 dp nokta (simgenin rengi) ve 15/600 ad.
class _GroupRow extends StatelessWidget {
  const _GroupRow({required this.kind});
  final AppAccountKind kind;
  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.fromLTRB(
      AppSpacing.medium,
      AppSpacing.step14,
      AppSpacing.medium,
      AppSpacing.step6,
    ),
    child: Row(
      children: [
        Container(
          width: 8,
          height: 8,
          decoration: BoxDecoration(
            color: accountKindColor(context, kind),
            shape: BoxShape.circle,
          ),
        ),
        const SizedBox(width: AppSpacing.step10),
        Text(
          kind == AppAccountKind.cash ? 'Nakit' : 'Banka',
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
            fontSize: AppTextSizes.compact,
            height: 1.3,
          ),
        ),
      ],
    ),
  );
}

/// Hesap satırı: en az 56 dp; 40 dp kapsül, ad 16/600, kapsam, bakiye 16/600.
class _AccountRow extends StatelessWidget {
  const _AccountRow({
    required this.item,
    required this.kind,
    required this.currency,
    required this.showScope,
    required this.onTap,
  });
  final AccountBalance item;
  final AppAccountKind kind;
  final String currency;
  final bool showScope;
  final VoidCallback onTap;
  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final scope = showScope ? item.defaultScope?.label : null;
    final amount = AppMoneyText(
      amount: item.balance,
      currency: currency,
      size: AppMoneySize.row,
      style: const TextStyle(fontSize: AppTextSizes.row, height: 1.3),
    );
    return MergeSemantics(
      child: InkWell(
        onTap: onTap,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 56),
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppSpacing.medium,
              vertical: AppSpacing.xSmall,
            ),
            child: Row(
              children: [
                AppIconCapsule(
                  icon: AppFinanceIcons.forAccountType(item.type),
                  iconColor: accountKindColor(context, kind),
                ),
                const SizedBox(width: AppSpacing.step14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        item.accountName,
                        maxLines: context.usesLargeText ? 2 : 1,
                        overflow: TextOverflow.ellipsis,
                        // Ad satırın en kalın yazısı değil; tutarla aynı
                        // boyda, yarı kalın (Varlık durumu satırlarıyla uyumlu).
                        style: theme.textTheme.titleSmall?.copyWith(
                          fontSize: AppTextSizes.compact,
                          fontWeight: FontWeight.w500,
                          height: 1.3,
                        ),
                      ),
                      if (scope != null)
                        Text(
                          scope,
                          style: theme.textTheme.bodySmall?.copyWith(
                            height: 1.35,
                          ),
                        ),
                      if (context.usesLargeText) amount,
                    ],
                  ),
                ),
                if (!context.usesLargeText) ...[
                  const SizedBox(width: AppSpacing.small),
                  amount,
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
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
    '${item.displayTitle}: ${_amountSentence(item)}, '
    'vadesi ${DateText.dayMonth(item.dueDate)}, $relative, '
    '${item.paymentLabel}';

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
