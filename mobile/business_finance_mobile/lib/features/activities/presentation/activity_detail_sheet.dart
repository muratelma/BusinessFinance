import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_detail_block.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/activity_models.dart';
import 'activity_tile.dart' show activityIcon, moneyEffectOf;

/// The detail behind a feed row: a shared header plus the fields that only make
/// sense for this kind. Actions are offered only where they can actually work,
/// so the sheet never shows a button that the server would refuse.
class ActivityDetailSheet extends StatelessWidget {
  const ActivityDetailSheet({
    required this.activity,
    super.key,
    this.onCancel,
    this.isCancelling = false,
  });

  final FinancialActivity activity;

  /// Null when this kind cannot be cancelled, which hides the action entirely.
  final Future<void> Function()? onCancel;
  final bool isCancelling;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final cancelled = activity.isCancelled;
    final effectLabel = switch (activity.effect) {
      ActivityEffect.income => 'Gelir',
      ActivityEffect.expense => 'Gider',
      // Nötr hareketin adı türüdür: transfer, kart ödemesi, borç ödemesi.
      ActivityEffect.neutral => activity.kind.label,
    };
    final subtitle = [
      effectLabel,
      if (activity.scope != null) activity.scope!.label,
    ].join(' · ');

    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          0,
          AppSpacing.medium,
          AppSpacing.medium,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          mainAxisSize: MainAxisSize.min,
          children: [
            Row(
              children: [
                AppIconCapsule(icon: activityIcon(activity), tone: _tone),
                const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Semantics(
                        header: true,
                        child: Text(
                          activity.title.isEmpty
                              ? activity.kind.label
                              : activity.title,
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                          style: theme.textTheme.titleSmall?.copyWith(
                            height: 1.3,
                          ),
                        ),
                      ),
                      Text(
                        subtitle,
                        style: theme.textTheme.labelMedium?.copyWith(
                          fontWeight: FontWeight.w400,
                          letterSpacing: 0,
                          color: surfaces.inkMuted,
                        ),
                      ),
                    ],
                  ),
                ),
                IconButton(
                  tooltip: 'Kapat',
                  onPressed: () => Navigator.of(context).maybePop(),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.medium),
            Wrap(
              alignment: WrapAlignment.spaceBetween,
              crossAxisAlignment: WrapCrossAlignment.center,
              spacing: AppSpacing.small,
              runSpacing: AppSpacing.small,
              children: [
                AppMoneyText(
                  amount: activity.amount,
                  currency: activity.currency,
                  effect: moneyEffectOf(activity.effect),
                  isCancelled: cancelled,
                  signed: true,
                  size: AppMoneySize.metric,
                  style: const TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                // Sayfadaki tek önemli durum: dolgulu kapsül.
                cancelled
                    ? const AppStatusChip(
                        label: 'İptal edildi',
                        icon: Icons.block,
                        tone: AppStatusTone.cancelled,
                      )
                    : const AppStatusChip(
                        label: 'Gerçekleşti',
                        icon: Icons.check_circle_outline,
                        tone: AppStatusTone.planned,
                      ),
              ],
            ),
            const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
            AppDetailBlock(rows: _rows(context)),
            const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
            _Actions(
              activity: activity,
              effectLabel: effectLabel,
              onCancel: onCancel,
              isCancelling: isCancelling,
            ),
          ],
        ),
      ),
    );
  }

  AppStatusTone get _tone => activity.isCancelled
      ? AppStatusTone.cancelled
      : switch (activity.effect) {
          ActivityEffect.income => AppStatusTone.income,
          ActivityEffect.expense => AppStatusTone.expense,
          ActivityEffect.neutral => AppStatusTone.neutral,
        };

  List<AppDetailRow> _rows(BuildContext context) {
    AppDetailRow row(IconData icon, String label, String value) =>
        AppDetailRow(icon: icon, label: label, value: value);
    final source = activity.sourceName;
    final destination = activity.destinationName;
    final route = (source != null && destination != null)
        ? '$source → $destination'
        : null;
    return [
      row(
        Icons.event_outlined,
        'Tarih',
        DateText.dayMonthYear(activity.activityDate),
      ),
      if (activity.categoryName != null)
        row(Icons.local_offer_outlined, 'Kategori', activity.categoryName!),
      // Kaynak ve hedef türe göre adlandırılır: "kaynak → hedef" transferde
      // ve kart ödemesinde farklı bir şey söyler.
      ...switch (activity.kind) {
        ActivityKind.transfer || ActivityKind.cardPayment => [
          if (route != null)
            row(Icons.swap_horiz, 'Hesaplar', route)
          else if (source != null)
            row(Icons.account_balance_outlined, 'Hesap', source),
        ],
        ActivityKind.cardCharge => [
          if (source != null) row(Icons.credit_card, 'Kredi kartı', source),
        ],
        // Taksit tek para hareketidir; anapara/faiz onun bölünmesidir. Kırılım
        // burada çünkü "2.600'ün nesi gider" sorusunun cevabı bu.
        ActivityKind.debtPayment || ActivityKind.debtCollection => [
          if (source != null)
            row(Icons.account_balance_outlined, 'Hesap', source),
          ...?_splitRows(),
        ],
        ActivityKind.debtOpening || ActivityKind.accountTransaction => [
          if (source != null) row(_accountIcon(source), 'Hesap', source),
        ],
        // Borçlandırmanın hesabı yoktur: para el değiştirmedi.
        ActivityKind.counterpartyCharge || ActivityKind.obligation => [
          if (source != null) row(Icons.group_outlined, 'Karşı taraf', source),
        ],
        ActivityKind.counterpartySettlement ||
        ActivityKind.obligationSettlement => [
          if (source != null)
            row(Icons.account_balance_outlined, 'Hesap', source),
          if (destination != null)
            row(Icons.group_outlined, 'Karşı taraf', destination),
        ],
        // Satış ve komisyon anında paranın çıktığı bir yer yok: hesap, paranın
        // birkaç gün sonra **geçeceği** yerdir.
        ActivityKind.posSale || ActivityKind.posCommission => [
          if (destination != null)
            row(
              Icons.account_balance_outlined,
              'Paranın geçeceği hesap',
              destination,
            ),
        ],
        ActivityKind.posTransfer => [
          if (destination != null)
            row(
              Icons.account_balance_outlined,
              'Paranın geçtiği hesap',
              destination,
            ),
        ],
      },
      // Başlık zaten açıklamayı taşıyorsa aynı cümle iki kez yazılmaz.
      if (activity.description != null &&
          activity.description!.isNotEmpty &&
          activity.description != activity.title)
        row(Icons.notes, 'Açıklama', activity.description!),
      row(Icons.edit_note, 'Köken', activity.origin.label),
      // Belgeler yalnız işlem kaydında olur ve yönetimi Veri araçlarındadır.
      if (activity.supportsAttachments)
        row(Icons.attach_file, 'Belge', 'Veri araçlarında'),
    ];
  }

  static IconData _accountIcon(String name) {
    final lower = name.toLowerCase();
    if (lower.contains('kart')) return Icons.credit_card;
    if (lower.contains('kasa') || lower.contains('cüzdan')) {
      return Icons.payments_outlined;
    }
    return Icons.account_balance_outlined;
  }

  /// Borç taksidinin anapara/faiz kırılımı.
  ///
  /// Faizi olmayan taksitte hiç çizilmez: `anapara 2.600 · faiz 0` satırı
  /// bilgi taşımaz, yalnız paneli uzatır.
  List<AppDetailRow>? _splitRows() {
    final interest = activity.interestPortion;
    final principal = activity.principalPortion;
    if (interest == null || principal == null || interest == '0.0000') {
      return null;
    }
    final isPayable = activity.kind == ActivityKind.debtPayment;
    return [
      AppDetailRow(
        icon: Icons.account_balance_wallet_outlined,
        label: 'Anapara',
        value: MoneyText.format(principal, activity.currency),
      ),
      AppDetailRow(
        icon: Icons.percent,
        label: isPayable ? 'Faiz (gider)' : 'Faiz (gelir)',
        value: MoneyText.format(interest, activity.currency),
      ),
    ];
  }
}

class _Actions extends StatelessWidget {
  const _Actions({
    required this.activity,
    required this.effectLabel,
    required this.onCancel,
    required this.isCancelling,
  });

  final FinancialActivity activity;
  final String effectLabel;
  final Future<void> Function()? onCancel;
  final bool isCancelling;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final note = theme.textTheme.labelMedium?.copyWith(
      fontWeight: FontWeight.w400,
      letterSpacing: 0,
      height: 1.4,
      color: surfaces.inkMuted,
    );

    if (activity.isCancelled) {
      return Text('Kayıt duruyor; toplamları artık etkilemez.', style: note);
    }
    // Nötr hareket para taşır ama kazanılan ya da harcanan tutarı değiştirmez;
    // yazılmazsa eksik bir sayı gibi okunur.
    final neutralNote = activity.effect == ActivityEffect.neutral
        ? Padding(
            padding: const EdgeInsets.only(bottom: AppSpacing.small),
            child: Text('Gelir/gider raporunu etkilemez', style: note),
          )
        : null;
    if (onCancel == null) {
      // Neden iptal edilemediğini söylemek görünmez ya da ölü bir düğmeden
      // iyidir.
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          ?neutralNote,
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(Icons.lock_outline, size: 16, color: surfaces.inkMuted),
              const SizedBox(width: AppSpacing.small),
              Expanded(
                child: Text(switch ((activity.origin, activity.kind)) {
                  (ActivityOrigin.recurring, _) =>
                    'Tekrarlayan plandan üretilen hareket iptal edilemez.',
                  (ActivityOrigin.installment, _) =>
                    'Taksit planından üretilen hareket iptal edilemez.',
                  (
                    _,
                    ActivityKind.posSale ||
                        ActivityKind.posCommission ||
                        ActivityKind.posTransfer,
                  ) =>
                    'Bu hareket türü iptal edilemez: POS tahsilatıyla birlikte '
                        'oluşur.',
                  _ => 'Bu hareket türü iptal edilemez.',
                }, style: note),
              ),
            ],
          ),
        ],
      );
    }
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.xSmall),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          ?neutralNote,
          OutlinedButton.icon(
            onPressed: isCancelling ? null : () => _confirm(context),
            style: OutlinedButton.styleFrom(
              minimumSize: const Size.fromHeight(48),
            ),
            icon: isCancelling
                ? const SizedBox.square(
                    dimension: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.block),
            label: const Text('Hareketi iptal et'),
          ),
        ],
      ),
    );
  }

  Future<void> _confirm(BuildContext context) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.block,
      destructive: true,
      title: 'Hareket iptal edilsin mi?',
      highlight:
          '$effectLabel · ${MoneyText.format(activity.amount, activity.currency)}',
      message:
          'Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları '
          'artık etkilemez.',
      confirmLabel: 'Hareketi iptal et',
    );
    if (!confirmed) return;
    await onCancel!();
  }
}
