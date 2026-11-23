import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_date_leaf.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_section_empty.dart';
import '../../../core/widgets/app_section_header.dart';
import '../data/debt_book_models.dart';
import 'debt_book_parts.dart';

/// Taksitli ya da kişisiz kaydın sayfası (`05-EKRAN-TASARIMI.md` §3.5).
/// Veriyi okumaz; borcun cevabını ([record]) çizer.
class DebtRecordView extends StatelessWidget {
  const DebtRecordView({
    required this.record,
    super.key,
    this.scopeVisible = false,
    this.onBack,
    this.onMore,
    this.onPay,
    this.onOpenPerson,
    this.onShowSchedule,
    this.onOpenClosing,
  });

  /// Takvimde görünen açık taksit sayısı (K18).
  static const int scheduleLimit = 3;

  final DebtRecordData record;
  final bool scopeVisible;
  final VoidCallback? onBack;
  final VoidCallback? onMore;
  final VoidCallback? onPay;
  final VoidCallback? onOpenPerson;
  final VoidCallback? onShowSchedule;
  final ValueChanged<DebtRecordClosing>? onOpenClosing;

  String get _actionLabel => record.isInterestBearing
      ? 'Taksit öde'
      : record.side == DebtSide.receivable
      ? 'Tahsilat kaydet'
      : 'Ödeme kaydet';

  @override
  Widget build(BuildContext context) {
    final open = record.openInstallments;
    final closings = [
      for (final closing in record.closings)
        if (!closing.isCancelled) closing,
    ];
    final closed = !isPositiveAmount(record.remaining);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppPageHeader(
          title: record.title,
          onBack: onBack,
          actions: [
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
              _RecordSummary(
                record: record,
                scopeVisible: scopeVisible,
                onOpenPerson: onOpenPerson,
              ),
              if (!closed) ...[
                const SizedBox(height: AppGaps.block),
                FilledButton(onPressed: onPay, child: Text(_actionLabel)),
              ],
              if (record.isInstallment && open.isNotEmpty) ...[
                const SizedBox(height: AppGaps.section),
                const AppSectionHeader(title: 'Takvim'),
                DebtSectionCard(
                  indent: AppIconCapsule.rowInset,
                  children: [
                    for (final item in open.take(scheduleLimit))
                      _InstallmentRow(item: item),
                    if (record.installments.length > scheduleLimit)
                      _ShowAllRow(
                        count: record.installments.length,
                        onTap: onShowSchedule,
                      ),
                  ],
                ),
              ],
              const SizedBox(height: AppGaps.section),
              AppSectionHeader(
                title: record.side == DebtSide.receivable
                    ? 'Tahsilatlar'
                    : 'Ödemeler',
              ),
              if (closings.isEmpty)
                AppSectionEmpty(
                  record.side == DebtSide.receivable
                      ? 'Henüz tahsilat yok.'
                      : 'Henüz ödeme yok.',
                )
              else
                DebtSectionCard(
                  indent: AppIconCapsule.rowInset,
                  children: [
                    for (final closing in closings.reversed)
                      _ClosingRow(
                        closing: closing,
                        paymentLabel: record.side == DebtSide.receivable
                            ? 'Tahsilat'
                            : 'Ödeme',
                        onTap: () => onOpenClosing?.call(closing),
                      ),
                  ],
                ),
            ],
          ),
        ),
      ],
    );
  }
}

/// Koyu özet kartı: kişi ve kapsam, kalan, taksit ilerlemesi, sıradaki ya da
/// geciken taksit; faizlide kalan anapara.
class _RecordSummary extends StatelessWidget {
  const _RecordSummary({
    required this.record,
    required this.scopeVisible,
    required this.onOpenPerson,
  });

  final DebtRecordData record;
  final bool scopeVisible;
  final VoidCallback? onOpenPerson;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final onInk = DebtInkCard.onInk(context);
    final muted = DebtInkCard.onInkMuted(context);
    final alert = DebtInkCard.alertOnInk(context);
    final small = theme.textTheme.bodySmall?.copyWith(color: muted);
    final open = record.openInstallments;
    final next = open.firstOrNull;
    final total = record.installments.length;
    final paid = record.paidCount;
    final scope = scopeVisible ? record.scope?.label : null;
    final person = record.counterpartyName;
    final closed = !isPositiveAmount(record.remaining);

    final String? dueLine;
    if (closed) {
      dueLine = null;
    } else if (next?.dueDate == null) {
      dueLine = record.isInstallment ? null : 'Vadesiz';
    } else if (record.isInstallment) {
      dueLine = next!.isOverdue
          ? '${next.sequence}. taksit gecikti · '
                '${DateText.dayMonth(next.dueDate!)}'
          : 'Sıradaki ${DateText.dayMonth(next.dueDate!)} · '
                '${MoneyText.format(next.openAmount, 'TRY')}';
    } else {
      dueLine = next!.isOverdue
          ? 'Vadesi geçti · ${DateText.dayMonth(next.dueDate!)}'
          : 'Vade ${DateText.dayMonth(next.dueDate!)}';
    }
    final overdueLine = next?.isOverdue ?? false;

    return Semantics(
      container: true,
      label: [
        record.title,
        ?person,
        ?scope,
        if (closed)
          'Kapandı'
        else
          'Kalan ${MoneyText.format(record.remaining, 'TRY')}',
        if (record.isInstallment) '$total taksitin $paid tanesi ödendi',
        ?dueLine,
      ].join('. '),
      child: ExcludeSemantics(
        child: DebtInkCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (person != null || scope != null) ...[
                Row(
                  children: [
                    if (person != null)
                      Expanded(
                        child: Align(
                          alignment: Alignment.centerLeft,
                          child: InkWell(
                            onTap: onOpenPerson,
                            borderRadius: BorderRadius.circular(
                              AppRadius.small,
                            ),
                            child: Padding(
                              padding: const EdgeInsets.symmetric(
                                vertical: AppSpacing.xSmall,
                              ),
                              child: Text(
                                '$person ›',
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: small?.copyWith(
                                  color: onInk,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                            ),
                          ),
                        ),
                      )
                    else if (scope != null)
                      // Kişisiz kayıtta kapsam satırın başındadır (kişi
                      // sayfasındaki `İşletme · cari hesap` satırı gibi).
                      Expanded(child: Text(scope, style: small)),
                    if (person != null && scope != null)
                      Text(scope, style: small),
                  ],
                ),
                const SizedBox(height: AppSpacing.small),
              ],
              Text(
                closed ? 'Kapandı' : 'Kalan',
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: AppTextSizes.compact,
                  color: onInk,
                ),
              ),
              const SizedBox(height: AppSpacing.xSmall),
              FittedBox(
                fit: BoxFit.scaleDown,
                alignment: Alignment.centerLeft,
                child: Text(
                  MoneyText.format(record.remaining, 'TRY'),
                  maxLines: 1,
                  style: theme.textTheme.headlineSmall?.copyWith(
                    fontSize: AppTextSizes.amount,
                    fontWeight: FontWeight.w700,
                    fontFeatures: AppTypography.tabularFigures,
                    letterSpacing: 0,
                    color: onInk,
                  ),
                ),
              ),
              if (record.isInterestBearing &&
                  record.remainingPrincipal != null) ...[
                const SizedBox(height: AppSpacing.xSmall),
                Text(
                  'Kalan anapara '
                  '${MoneyText.format(record.remainingPrincipal!, 'TRY')}',
                  style: small,
                ),
              ],
              if (record.isInstallment && total > 0) ...[
                const SizedBox(height: AppSpacing.compact),
                _InkProgress(value: paid / total),
                const SizedBox(height: AppSpacing.small),
                Text('$paid/$total taksit ödendi', style: small),
              ],
              if (dueLine != null) ...[
                const SizedBox(height: AppSpacing.xSmall),
                Text(
                  dueLine,
                  style: overdueLine
                      ? small?.copyWith(
                          color: alert,
                          fontWeight: FontWeight.w600,
                        )
                      : small,
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

/// Koyu karttaki 6 dp ilerleme çizgisi. Oran yalnız çizimdir; sayı olarak
/// altında `3/12 taksit ödendi` yazar.
class _InkProgress extends StatelessWidget {
  const _InkProgress({required this.value});

  final double value;

  @override
  Widget build(BuildContext context) {
    final onInk = DebtInkCard.onInk(context);
    return ClipRRect(
      borderRadius: BorderRadius.circular(AppRadius.chip),
      child: SizedBox(
        height: AppSpacing.step6,
        child: Stack(
          fit: StackFit.expand,
          children: [
            ColoredBox(color: onInk.withValues(alpha: 0.2)),
            FractionallySizedBox(
              alignment: Alignment.centerLeft,
              widthFactor: value.clamp(0, 1),
              heightFactor: 1,
              child: ColoredBox(color: onInk),
            ),
          ],
        ),
      ),
    );
  }
}

/// Takvim yaprağının genişliği (`AppDateLeaf`, 44×48); vadesiz satır aynı
/// hizada başlar.
const double _leafWidth = 44;

/// Takvimin satırı: gün ve taksit sırası, tutar; gecikmede gider rengi,

/// kısmen ödenmişte ödenen kısım.
class _InstallmentRow extends StatelessWidget {
  const _InstallmentRow({required this.item});

  final DebtRecordInstallment item;

  @override
  Widget build(BuildContext context) {
    final due = item.dueDate;
    final date = due == null ? 'Vadesiz' : DateText.dayMonth(due);
    final paidPart = item.isPartlyPaid
        ? '${MoneyText.format(item.paidAmount, 'TRY')} ödendi'
        : null;
    return DebtRowFrame(
      leading: due == null
          ? const SizedBox(width: _leafWidth)
          : AppDateLeaf.fromDate(DateTime.parse(due), urgent: item.isOverdue),
      title: '${item.sequence}. taksit',
      trailing: DebtRowAmount(item.openAmount),
      subtitle: item.isOverdue || paidPart != null
          ? DebtRowSubtitle(
              alert: item.isOverdue ? 'Gecikti' : null,
              parts: [?paidPart],
            )
          : null,
      semanticLabel: [
        date,
        '${item.sequence}. taksit',
        MoneyText.format(item.openAmount, 'TRY'),
        if (item.isOverdue) 'Gecikti',
        ?paidPart,
      ].join('. '),
    );
  }
}

class _ShowAllRow extends StatelessWidget {
  const _ShowAllRow({required this.count, required this.onTap});

  final int count;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      label: 'Bütün takvimi göster, $count taksit',
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: AppSizes.button),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: Row(
              children: [
                Expanded(
                  child: Text(
                    'Tümünü göster',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
                Text(
                  '$count taksit',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: surfaces.inkMuted,
                  ),
                ),
                const SizedBox(width: AppSpacing.xSmall),
                Icon(Icons.chevron_right, color: surfaces.inkMuted),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Ödeme ya da indirim satırı; dokununca ayrıntı ve geri alma.
class _ClosingRow extends StatelessWidget {
  const _ClosingRow({
    required this.closing,
    required this.paymentLabel,
    required this.onTap,
  });

  final DebtRecordClosing closing;

  /// `Tahsilat` ya da `Ödeme`.
  final String paymentLabel;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final source = closing.isDiscount ? null : closing.sourceName;
    final title = closing.isDiscount ? 'İndirim' : paymentLabel;
    return DebtRowFrame(
      leading: AppDateLeaf.fromDate(DateTime.parse(closing.date)),
      title: title,
      trailing: closing.isDiscount
          ? Text(
              MoneyText.format(closing.amount, 'TRY'),
              style: TextStyle(
                color: colors.neutral,
                fontWeight: FontWeight.w700,
                fontFeatures: AppTypography.tabularFigures,
              ),
            )
          : DebtRowAmount(closing.amount),
      subtitle: source == null ? null : DebtRowSubtitle(parts: [source]),
      onTap: onTap,
      semanticLabel: [
        DateText.dayMonthYear(closing.date),
        title,
        ?source,
        MoneyText.format(closing.amount, 'TRY'),
      ].join('. '),
    );
  }
}
