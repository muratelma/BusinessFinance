import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_text_action.dart';
import '../data/finance_models.dart';

/// Planın gösterilen adı: açıklama yoksa `Taksitli harcama`.
String installmentPlanTitle(InstallmentPlanModel plan) =>
    plan.description ?? 'Taksitli harcama';

int _realized(InstallmentPlanModel plan) =>
    plan.items.where((item) => item.isRealized).length;

InstallmentItemModel? _next(InstallmentPlanModel plan) =>
    plan.items.where((item) => !item.isRealized).firstOrNull;

String _left(InstallmentPlanModel plan) {
  final left = plan.items.length - _realized(plan);
  return left == 0
      ? 'Tamamlandı'
      : left == 1
      ? 'Son taksit'
      : '$left taksit kaldı';
}

/// Kart sayfasındaki Taksitler bölümü (teslim §3, `T3Sec`): kısa yatay
/// şerit. Yüksekliği plan sayısından bağımsızdır; ikinci kart sağda görünür
/// ve kaydırıldığı anlaşılır. Tek planda şerit yoktur, kart tam genişliktir
/// ve `Tümü` çizilmez. Karta dokunmak Taksitler sayfasını açar.
class CardInstallmentStrip extends StatelessWidget {
  const CardInstallmentStrip({
    required this.plans,
    required this.onOpen,
    super.key,
  });

  final List<InstallmentPlanModel> plans;
  final VoidCallback onOpen;

  /// Şeritteki kartın genişliği.
  static const double cardWidth = 248;

  @override
  Widget build(BuildContext context) {
    final single = plans.length == 1;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
          child: AppSectionHeader(
            title: 'Taksitler',
            padding: single ? null : EdgeInsets.zero,
            trailing: single
                ? null
                : AppTextAction(
                    label: 'Tümü',
                    trailingIcon: Icons.chevron_right,
                    onPressed: onOpen,
                  ),
          ),
        ),
        if (single)
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
            child: InstallmentPlanCard(plan: plans.single, onTap: onOpen),
          )
        else
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
            child: IntrinsicHeight(
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  for (final (index, plan) in plans.indexed) ...[
                    if (index > 0) const SizedBox(width: AppGaps.card),
                    SizedBox(
                      width:
                          cardWidth *
                          MediaQuery.textScalerOf(
                            context,
                          ).scale(1).clamp(1.0, 1.4),
                      child: InstallmentPlanCard(
                        plan: plan,
                        compact: true,
                        onTap: onOpen,
                      ),
                    ),
                  ],
                ],
              ),
            ),
          ),
      ],
    );
  }
}

/// Bir taksitli harcamanın kartı: ad ve toplam, mavi `Sıradaki` kutusu,
/// yansıyan / kalan sayısı ve taksit çubuğu. [compact] şeritteki dar
/// hâldir: sıradaki tutar kutuda alta iner, yazılar bir tık küçüktür.
class InstallmentPlanCard extends StatelessWidget {
  const InstallmentPlanCard({
    required this.plan,
    required this.onTap,
    super.key,
    this.compact = false,
  });

  final InstallmentPlanModel plan;
  final VoidCallback onTap;
  final bool compact;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final done = _realized(plan);
    final total = plan.items.length;
    final small = theme.textTheme.bodySmall?.copyWith(
      fontSize: compact ? AppTextSizes.micro : AppTextSizes.caption,
      height: 1.2,
      color: surfaces.inkMuted,
    );
    return AppCard(
      onTap: onTap,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            crossAxisAlignment: compact
                ? CrossAxisAlignment.baseline
                : CrossAxisAlignment.center,
            textBaseline: TextBaseline.alphabetic,
            children: [
              Expanded(
                child: Text(
                  installmentPlanTitle(plan),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontSize: compact ? AppTextSizes.compact : AppTextSizes.row,
                    height: 1.3,
                  ),
                ),
              ),
              const SizedBox(width: AppSpacing.small),
              AppMoneyText(
                amount: plan.totalAmount,
                currency: plan.currency,
                size: AppMoneySize.row,
                style: TextStyle(
                  fontSize: compact ? AppTextSizes.caption : AppTextSizes.body,
                  fontWeight: compact ? FontWeight.w500 : FontWeight.w600,
                  color: compact ? surfaces.inkMuted : null,
                  height: 1.2,
                ),
              ),
              if (!compact) ...[
                const SizedBox(width: AppSpacing.small),
                Icon(Icons.chevron_right, size: 20, color: surfaces.inkMuted),
              ],
            ],
          ),
          const SizedBox(height: AppSpacing.compact),
          _NextBox(plan: plan, stacked: compact),
          const SizedBox(height: AppSpacing.compact),
          Row(
            children: [
              Expanded(
                child: Text(
                  compact
                      ? '$done / $total yansıdı'
                      : '$done / $total taksit yansıdı',
                  style: small,
                ),
              ),
              Text(_left(plan), style: small),
            ],
          ),
          const SizedBox(height: AppSpacing.step6),
          InstallmentSegments(total: total, realized: done),
        ],
      ),
    );
  }
}

/// Mavi `Sıradaki · 12 Kasım` kutusu ve sıradaki taksidin tutarı.
class _NextBox extends StatelessWidget {
  const _NextBox({required this.plan, required this.stacked});
  final InstallmentPlanModel plan;
  final bool stacked;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final next = _next(plan);
    final label = Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(
          Icons.calendar_today_outlined,
          size: 16,
          color: colors.onNeutralContainer,
        ),
        const SizedBox(width: AppSpacing.step6),
        Flexible(
          child: Text(
            next == null
                ? 'Bütün taksitler yansıdı'
                : 'Sıradaki · ${DateText.dayMonth(next.scheduledDate)}',
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
              fontSize: AppTextSizes.caption,
              height: 1.2,
              color: colors.onNeutralContainer,
            ),
          ),
        ),
      ],
    );
    final amount = next == null
        ? null
        : AppMoneyText(
            amount: next.amount,
            currency: next.currency,
            effect: AppMoneyEffect.neutral,
            onContainer: true,
            size: AppMoneySize.row,
            style: TextStyle(
              fontSize: stacked ? AppTextSizes.section : AppTextSizes.metric,
              fontWeight: FontWeight.w700,
              height: 1.2,
            ),
          );
    return Container(
      padding: stacked
          ? const EdgeInsets.symmetric(
              horizontal: AppSpacing.compact,
              vertical: AppSpacing.step10,
            )
          : const EdgeInsets.symmetric(
              horizontal: AppSpacing.step14,
              vertical: AppSpacing.compact,
            ),
      decoration: BoxDecoration(
        color: colors.neutralContainer,
        borderRadius: BorderRadius.circular(AppRadius.inner),
      ),
      child: stacked || context.usesLargeText
          ? Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                label,
                if (amount != null) ...[
                  const SizedBox(height: AppSpacing.xSmall),
                  amount,
                ],
              ],
            )
          : Row(
              children: [
                Expanded(child: label),
                const SizedBox(width: AppSpacing.step10),
                ?amount,
              ],
            ),
    );
  }
}

/// Taksit çubuğu: her taksit bir parça; yansıyanlar mavi.
class InstallmentSegments extends StatelessWidget {
  const InstallmentSegments({
    required this.total,
    required this.realized,
    super.key,
    this.background,
  });
  final int total;
  final int realized;

  /// Kalan parçaların rengi; gri kutunun içinde koyu çizgi.
  final Color? background;

  @override
  Widget build(BuildContext context) {
    final fill = AppFinanceColors.of(context).neutralFill;
    final rest = background ?? AppSurfaces.of(context).cardMuted;
    final gap = total > 8 ? 3.0 : AppSpacing.xSmall;
    // LayoutBuilder yok: şerit kartları IntrinsicHeight içinde eşit boyda.
    if (total == 0 || total > 24) {
      return ExcludeSemantics(
        child: ClipRRect(
          borderRadius: BorderRadius.circular(AppRadius.chip),
          child: LinearProgressIndicator(
            value: total == 0 ? 0 : realized / total,
            minHeight: 4,
            color: fill,
            backgroundColor: rest,
          ),
        ),
      );
    }
    return ExcludeSemantics(
      child: Row(
        children: [
          for (var i = 0; i < total; i++) ...[
            if (i > 0) SizedBox(width: gap),
            Expanded(
              child: Container(
                height: 4,
                decoration: BoxDecoration(
                  color: i < realized ? fill : rest,
                  borderRadius: BorderRadius.circular(AppRadius.chip),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

/// Taksitler sayfası (teslim §4, TS1): başlığın altında kart adı yazmaz;
/// kullanıcı kartın sayfasından geldi. Liste hemen başlar.
class CardInstallmentsPage extends StatelessWidget {
  const CardInstallmentsPage({
    required this.listenable,
    required this.plans,
    required this.onOpenPlan,
    super.key,
  });

  /// Gerçekleştirmeden sonra liste tazelenir.
  final Listenable listenable;
  final List<InstallmentPlanModel> Function() plans;
  final ValueChanged<InstallmentPlanModel> onOpenPlan;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Taksitler',
            onBack: () => Navigator.maybePop(context),
          ),
          Expanded(
            child: ListenableBuilder(
              listenable: listenable,
              builder: (context, _) => Align(
                alignment: Alignment.topCenter,
                child: ConstrainedBox(
                  constraints: const BoxConstraints(
                    maxWidth: AppBreakpoints.contentMaxWidth,
                  ),
                  child: ListView(
                    padding: AppPageInsets.list,
                    children: [
                      for (final (index, plan) in plans().indexed) ...[
                        if (index > 0) const SizedBox(height: AppGaps.card),
                        InstallmentPlanCard(
                          plan: plan,
                          onTap: () => onOpenPlan(plan),
                        ),
                      ],
                    ],
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    ),
  );
}

/// Taksit ayrıntısı (teslim §5, TD1 + G1): başlık harcamanın adı; özet
/// kutusunda toplam, taksit sayısı ve çubuk; taksit satırları kayar,
/// `N. taksiti gerçekleştir` düğmesi altta sabittir ve tutarı düğmenin
/// sağında yazar. Düğme yalnız günü gelmiş sıradaki taksitte çıkar.
Future<void> showInstallmentPlanDetail(
  BuildContext context, {
  required Listenable listenable,
  required InstallmentPlanModel? Function() plan,
  required bool Function() isSubmitting,
  required Future<void> Function(InstallmentPlanModel plan, int sequence)
  onRealize,
}) => AppAdaptiveSheet.show<void>(
  context: context,
  maxHeightFactor: AppAdaptiveSheet.tallDetailHeight,
  builder: (sheetContext) => ListenableBuilder(
    listenable: listenable,
    builder: (sheetContext, _) {
      final value = plan();
      if (value == null) return const SizedBox.shrink();
      return _PlanDetail(
        plan: value,
        submitting: isSubmitting(),
        onRealize: (sequence) => onRealize(value, sequence),
      );
    },
  ),
);

class _PlanDetail extends StatefulWidget {
  const _PlanDetail({
    required this.plan,
    required this.submitting,
    required this.onRealize,
  });
  final InstallmentPlanModel plan;
  final bool submitting;
  final ValueChanged<int> onRealize;

  @override
  State<_PlanDetail> createState() => _PlanDetailState();
}

class _PlanDetailState extends State<_PlanDetail> {
  final _nextKey = GlobalKey();

  @override
  void initState() {
    super.initState();
    // İlk açılışta sıradaki taksit görünür.
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final target = _nextKey.currentContext;
      if (mounted && target != null) {
        Scrollable.ensureVisible(
          target,
          alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd,
        );
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final plan = widget.plan;
    final done = _realized(plan);
    final next = _next(plan);
    final today = DateUtils.dateOnly(DateTime.now());
    final due =
        next != null &&
        !(DateTime.tryParse(next.scheduledDate)?.isAfter(today) ?? false);
    final muted = theme.textTheme.bodySmall?.copyWith(
      fontSize: AppTextSizes.caption,
      height: 1.2,
      color: surfaces.inkMuted,
    );
    return SafeArea(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Başlık sabittir ve tutamacın altında, uygulamanın bütün
          // panellerindeki boşlukla durur (`AppSheetInsets.header`); liste
          // altında kayar. Önceden gövdeyle birlikte kayıyor ve tutamaçtan
          // 16 dp aşağıda başlıyordu.
          Padding(
            padding: AppSheetInsets.header(context),
            child: Semantics(
              header: true,
              child: Text(
                installmentPlanTitle(plan),
                style: theme.textTheme.titleLarge,
              ),
            ),
          ),
          // Toplam tutar kutusu da sabittir: başlık ve kutu panelin tutma
          // yeridir, buradan aşağı çekince panel kapanır; liste kendi içinde
          // kayar (kullanıcı, 13 Kasım 2026).
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppSheetInsets.edge,
            ),
            child: Container(
              padding: const EdgeInsets.all(AppSpacing.medium),
              decoration: BoxDecoration(
                color: surfaces.cardMuted,
                borderRadius: BorderRadius.circular(AppRadius.field),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Wrap(
                    alignment: WrapAlignment.spaceBetween,
                    runSpacing: AppSpacing.small,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text('Toplam tutar', style: muted),
                          const SizedBox(height: AppSpacing.xSmall),
                          AppMoneyText(
                            amount: plan.totalAmount,
                            currency: plan.currency,
                            size: AppMoneySize.metric,
                          ),
                        ],
                      ),
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.end,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text('Taksit sayısı', style: muted),
                          const SizedBox(height: AppSpacing.xSmall),
                          Text(
                            '${plan.items.length} taksit',
                            style: theme.textTheme.titleLarge?.copyWith(
                              fontSize: AppTextSizes.metric,
                              height: 1.2,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                  const SizedBox(height: AppSpacing.compact),
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          '$done / ${plan.items.length} taksit yansıdı',
                          style: muted,
                        ),
                      ),
                      Text(_left(plan), style: muted),
                    ],
                  ),
                  const SizedBox(height: AppSpacing.step6),
                  InstallmentSegments(
                    total: plan.items.length,
                    realized: done,
                    background: surfaces.borderStrong,
                  ),
                ],
              ),
            ),
          ),
          Flexible(
            child: SingleChildScrollView(
              padding: AppSheetInsets.body,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  for (final item in plan.items)
                    KeyedSubtree(
                      key: item.sequence == next?.sequence ? _nextKey : null,
                      child: _ItemRow(
                        item: item,
                        isNext: item.sequence == next?.sequence,
                      ),
                    ),
                  const SizedBox(height: AppGaps.block),
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Icon(
                        Icons.info_outline,
                        size: 20,
                        color: surfaces.inkMuted,
                      ),
                      const SizedBox(width: AppSpacing.step10),
                      Expanded(
                        child: Text(
                          'Taksitler kart borcuna yansır; ödeme kart '
                          'üzerinden yapılır.',
                          style: theme.textTheme.bodyMedium?.copyWith(
                            height: 1.35,
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
          if (due) ...[
            Divider(height: 1, color: surfaces.border),
            Padding(
              padding: AppSheetInsets.actions,
              child: Semantics(
                button: true,
                label:
                    '${next.sequence}. taksiti gerçekleştir, '
                    '${MoneyText.format(next.amount, next.currency)}',
                excludeSemantics: true,
                child: FilledButton(
                  onPressed: widget.submitting
                      ? null
                      : () => widget.onRealize(next.sequence),
                  style: FilledButton.styleFrom(
                    minimumSize: const Size.fromHeight(52),
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.step20,
                    ),
                  ),
                  child: Row(
                    children: [
                      Expanded(
                        child: Text(
                          '${next.sequence}. taksiti gerçekleştir',
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      const SizedBox(width: AppSpacing.compact),
                      Text(
                        MoneyText.format(next.amount, next.currency),
                        style: const TextStyle(
                          fontWeight: FontWeight.w700,
                          fontFeatures: AppTypography.tabularFigures,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

/// Taksit satırı: yansıyan kırmızı simge ve tutar (`Karta yansıdı`),
/// sıradaki mavi zemin (`Sıradaki`), kalanlar `Planlandı`.
class _ItemRow extends StatelessWidget {
  const _ItemRow({required this.item, required this.isNext});
  final InstallmentItemModel item;
  final bool isNext;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    final ink = isNext ? colors.onNeutralContainer : surfaces.ink;
    final sub = theme.textTheme.bodySmall?.copyWith(
      fontSize: AppTextSizes.caption,
      height: 1.2,
      color: isNext ? colors.onNeutralContainer : surfaces.inkMuted,
    );
    return Container(
      margin: const EdgeInsets.only(bottom: AppSpacing.xxSmall),
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.compact,
        vertical: AppSpacing.step10,
      ),
      decoration: BoxDecoration(
        color: isNext ? colors.neutralContainer : null,
        borderRadius: BorderRadius.circular(AppRadius.inner),
      ),
      child: Row(
        children: [
          Icon(
            isNext
                ? Icons.schedule
                : item.isRealized
                ? Icons.receipt_long_outlined
                : Icons.calendar_today_outlined,
            size: 20,
            color: isNext
                ? colors.onNeutralContainer
                : item.isRealized
                ? colors.expense
                : surfaces.inkMuted,
          ),
          const SizedBox(width: AppSpacing.step14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  '${item.sequence}. taksit',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontSize: AppTextSizes.compact,
                    height: 1.25,
                    color: ink,
                  ),
                ),
                const SizedBox(height: AppSpacing.xxSmall),
                Text(DateText.dayMonthYear(item.scheduledDate), style: sub),
              ],
            ),
          ),
          const SizedBox(width: AppSpacing.small),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              AppMoneyText(
                amount: item.amount,
                currency: item.currency,
                size: AppMoneySize.row,
                effect: item.isRealized
                    ? AppMoneyEffect.expense
                    : isNext
                    ? AppMoneyEffect.neutral
                    : null,
                onContainer: isNext,
                style: const TextStyle(
                  fontSize: AppTextSizes.compact,
                  fontWeight: FontWeight.w600,
                  height: 1.25,
                ),
              ),
              const SizedBox(height: AppSpacing.xxSmall),
              Text(
                item.isRealized
                    ? 'Karta yansıdı'
                    : isNext
                    ? 'Sıradaki'
                    : 'Planlandı',
                style: sub,
              ),
            ],
          ),
        ],
      ),
    );
  }
}
