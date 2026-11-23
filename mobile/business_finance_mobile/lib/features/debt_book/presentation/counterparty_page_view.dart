import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_section_empty.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_text_action.dart';
import '../../activities/data/activity_models.dart';
import '../../activities/presentation/activity_tile.dart';
import '../data/debt_book_models.dart';
import 'debt_book_parts.dart';

/// Kişi sayfası (`05-EKRAN-TASARIMI.md` §3.2). Veriyi okumaz; kişinin özetini
/// ([page]) ve hareketlerini ([movements], akıştan) çizer.
class CounterpartyPageView extends StatelessWidget {
  const CounterpartyPageView({
    required this.page,
    required this.movements,
    super.key,
    this.scopeVisible = false,
    this.onBack,
    this.onAdd,
    this.onMore,
    this.onCollect,
    this.onPay,
    this.onOpenRecord,
    this.onOpenMovement,
    this.onFilter,
  });

  final CounterpartyPageData page;
  final List<FinancialActivity> movements;
  final bool scopeVisible;
  final VoidCallback? onBack;
  final VoidCallback? onAdd;
  final VoidCallback? onMore;
  final VoidCallback? onCollect;
  final VoidCallback? onPay;
  final ValueChanged<DebtBookItem>? onOpenRecord;
  final ValueChanged<FinancialActivity>? onOpenMovement;
  final VoidCallback? onFilter;

  @override
  Widget build(BuildContext context) {
    final actions = [
      if (page.owesYou) ('Tahsilat', onCollect),
      if (page.youOwe) ('Ödeme', onPay),
    ];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppPageHeader(
          title: page.name,
          onBack: onBack,
          actions: [
            IconButton(
              tooltip: 'Yeni kayıt',
              icon: const Icon(Icons.add),
              onPressed: onAdd,
            ),
            IconButton(
              tooltip: 'Diğer seçenekler',
              icon: const Icon(Icons.more_vert),
              onPressed: onMore,
            ),
          ],
        ),
        Expanded(
          child: ListView(
            padding: AppPageInsets.aboveBar(context, AppPageInsets.list),
            children: [
              _PersonSummary(page: page, scopeVisible: scopeVisible),
              if (!page.isActive) ...[
                const SizedBox(height: AppGaps.notice),
                const AppInlineNotice(
                  margin: EdgeInsets.zero,
                  message:
                      'Pasif kişi. Yeni satış ve alım yazılmaz; tahsilat ve '
                      'ödeme yazılır.',
                ),
              ],
              if (actions.isNotEmpty) ...[
                const SizedBox(height: AppGaps.block),
                Row(
                  children: [
                    for (final (index, (label, onPressed))
                        in actions.indexed) ...[
                      if (index > 0) const SizedBox(width: AppSpacing.small),
                      Expanded(
                        child: FilledButton(
                          onPressed: onPressed,
                          child: Text(label),
                        ),
                      ),
                    ],
                  ],
                ),
              ],
              if (page.records.isNotEmpty) ...[
                const SizedBox(height: AppGaps.section),
                const AppSectionHeader(title: 'Taksitli kayıtlar'),
                DebtSectionCard(
                  children: [
                    for (final record in page.records)
                      DebtBookItemRow(
                        item: record,
                        onTap: () => onOpenRecord?.call(record),
                      ),
                  ],
                ),
              ],
              const SizedBox(height: AppGaps.section),
              AppSectionHeader(
                title: 'Hareketler',
                padding: EdgeInsets.zero,
                trailing: AppTextAction(
                  label: 'Filtre',
                  underlined: true,
                  onPressed: onFilter,
                ),
              ),
              if (movements.isEmpty)
                const AppSectionEmpty('Henüz hareket yok.')
              else
                AppCard(
                  padding: EdgeInsets.zero,
                  child: Column(
                    children: [
                      for (final (index, activity) in movements.indexed) ...[
                        if (index > 0)
                          const Divider(
                            height: 1,
                            indent: AppIconCapsule.rowInset,
                          ),
                        ActivityTile(
                          activity: activity,
                          // Kişinin kendi sayfasında başlık kişinin adını
                          // tekrar etmez; adın yerine kaydın türü yazılır.
                          title: activity.title == page.name
                              ? personMovementTitle(activity)
                              : null,
                          onTap: () => onOpenMovement?.call(activity),
                        ),
                      ],
                    ],
                  ),
                ),
            ],
          ),
        ),
      ],
    );
  }
}

/// Kişi sayfasındaki hareketin kısa adı: kişi zaten başlıkta olduğu için
/// `Cari tahsilat` değil `Tahsilat`.
String personMovementTitle(FinancialActivity activity) {
  final payable = activity.direction == ActivityDirection.payable;
  return switch (activity.kind) {
    ActivityKind.counterpartyCharge =>
      payable ? 'Vadeli alım' : 'Veresiye satış',
    ActivityKind.counterpartySettlement => payable ? 'Ödeme' : 'Tahsilat',
    _ => activity.kindLabel,
  };
}

/// Koyu özet kartı: kişinin türü, açık taraf(lar), gecikmiş ve sıradaki vade.
class _PersonSummary extends StatelessWidget {
  const _PersonSummary({required this.page, required this.scopeVisible});

  final CounterpartyPageData page;
  final bool scopeVisible;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final onInk = DebtInkCard.onInk(context);
    final muted = DebtInkCard.onInkMuted(context);
    final small = theme.textTheme.bodySmall?.copyWith(color: muted);
    final kind = !scopeVisible || page.scope == null
        ? null
        : page.scope == TransactionScope.business
        ? 'İşletme · cari hesap'
        : 'Şahsi';
    final sides = [
      if (page.owesYou)
        _Side(
          label: 'Alacağınız',
          amount: page.owedToYou,
          overdue: page.overdueReceivable,
          nextDate: page.nextReceivableDueDate,
          nextAmount: page.nextReceivableDueAmount,
        ),
      if (page.youOwe)
        _Side(
          label: 'Borcunuz',
          amount: page.owedByYou,
          overdue: page.overduePayable,
          nextDate: page.nextPayableDueDate,
          nextAmount: page.nextPayableDueAmount,
        ),
    ];
    final sentence = [
      page.name,
      ?kind,
      for (final side in sides) side.sentence,
      if (sides.isEmpty) 'Hesap kapalı',
      if (page.records.isNotEmpty) 'Taksitli kayıtlar dahil',
    ].join('. ');
    return Semantics(
      label: sentence,
      excludeSemantics: true,
      child: DebtInkCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (kind != null) ...[
              Text(kind, style: small),
              const SizedBox(height: AppSpacing.small),
            ],
            if (sides.isEmpty) ...[
              Text(
                'Hesap kapalı',
                style: theme.textTheme.titleLarge?.copyWith(
                  fontSize: AppTextSizes.metric,
                  fontWeight: FontWeight.w700,
                  color: onInk,
                ),
              ),
              const SizedBox(height: AppSpacing.xSmall),
              Text('Bakiye yok', style: small),
            ] else if (sides.length == 1)
              _SideBlock(side: sides.single, large: true)
            else
              IntrinsicHeight(
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Expanded(child: _SideBlock(side: sides[0], large: false)),
                    Padding(
                      padding: const EdgeInsets.symmetric(
                        horizontal: AppSpacing.compact,
                      ),
                      child: VerticalDivider(
                        width: 1,
                        thickness: 1,
                        color: onInk.withValues(alpha: 0.2),
                      ),
                    ),
                    Expanded(child: _SideBlock(side: sides[1], large: false)),
                  ],
                ),
              ),
            if (page.records.isNotEmpty) ...[
              const SizedBox(height: AppSpacing.small),
              Text('Taksitli kayıtlar dahil', style: small),
            ],
          ],
        ),
      ),
    );
  }
}

class _Side {
  const _Side({
    required this.label,
    required this.amount,
    required this.overdue,
    required this.nextDate,
    required this.nextAmount,
  });

  final String label;
  final String amount;
  final String overdue;
  final String? nextDate;
  final String nextAmount;

  bool get hasOverdue => isPositiveAmount(overdue);

  String get overdueText => '${MoneyText.format(overdue, 'TRY')} gecikmiş';

  String? get nextText {
    final date = nextDate;
    if (date == null) return null;
    final partial = isPositiveAmount(nextAmount) && nextAmount != amount;
    return partial
        ? 'Sıradaki vade ${DateText.dayMonth(date)} · '
              '${MoneyText.format(nextAmount, 'TRY')}'
        : 'Sıradaki vade ${DateText.dayMonth(date)}';
  }

  String get sentence => [
    '$label ${MoneyText.format(amount, 'TRY')}',
    if (hasOverdue) overdueText,
    ?nextText,
  ].join(', ');
}

class _SideBlock extends StatelessWidget {
  const _SideBlock({required this.side, required this.large});

  final _Side side;

  /// Tek taraf açıksa tutar 28, iki taraf yan yanaysa 22.
  final bool large;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final onInk = DebtInkCard.onInk(context);
    final muted = DebtInkCard.onInkMuted(context);
    final alert = DebtInkCard.alertOnInk(context);
    final small = theme.textTheme.bodySmall?.copyWith(color: muted);
    final next = side.nextText;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          side.label,
          style: theme.textTheme.titleSmall?.copyWith(
            fontSize: large ? AppTextSizes.compact : AppTextSizes.caption,
            fontWeight: large ? FontWeight.w600 : FontWeight.w400,
            color: large ? onInk : muted,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        FittedBox(
          fit: BoxFit.scaleDown,
          alignment: Alignment.centerLeft,
          child: Text(
            MoneyText.format(side.amount, 'TRY'),
            maxLines: 1,
            style: theme.textTheme.headlineSmall?.copyWith(
              fontSize: large ? AppTextSizes.amount : AppTextSizes.metric,
              fontWeight: FontWeight.w700,
              fontFeatures: AppTypography.tabularFigures,
              letterSpacing: 0,
              color: onInk,
            ),
          ),
        ),
        if (side.hasOverdue) ...[
          const SizedBox(height: AppSpacing.small),
          Row(
            children: [
              Icon(Icons.schedule, size: AppTextSizes.body, color: alert),
              const SizedBox(width: AppSpacing.xSmall),
              Flexible(
                child: Text(
                  side.overdueText,
                  style: small?.copyWith(
                    color: alert,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ],
          ),
        ],
        if (next != null) ...[
          SizedBox(
            height: side.hasOverdue ? AppSpacing.xSmall : AppSpacing.small,
          ),
          Text(next, style: small),
        ],
      ],
    );
  }
}
