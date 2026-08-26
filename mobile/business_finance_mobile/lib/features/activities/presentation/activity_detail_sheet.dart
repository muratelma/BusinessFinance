import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_submit_button.dart';
import '../data/activity_models.dart';
import 'activity_tile.dart' show moneyEffectOf;

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
    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              activity.title.isEmpty ? activity.kind.label : activity.title,
              style: theme.textTheme.titleLarge,
            ),
            const SizedBox(height: AppSpacing.xSmall),
            Text(activity.kind.label, style: theme.textTheme.bodyMedium),
            const SizedBox(height: AppSpacing.medium),
            _AmountLine(activity: activity),
            const Divider(height: AppSpacing.large),
            ..._rows(context),
            const SizedBox(height: AppSpacing.medium),
            _Actions(
              activity: activity,
              onCancel: onCancel,
              isCancelling: isCancelling,
            ),
          ],
        ),
      ),
    );
  }

  List<Widget> _rows(BuildContext context) {
    final rows = <_DetailRow>[
      _DetailRow(
        'Durum',
        activity.isCancelled ? 'İptal edildi' : 'Gerçekleşti',
      ),
      _DetailRow('İş tarihi', activity.activityDate),
      if (activity.categoryName != null)
        _DetailRow('Kategori', activity.categoryName!),
      // Source and destination are labelled per kind: "kaynak → hedef" means
      // something different for a transfer than for a card payment.
      ...switch (activity.kind) {
        ActivityKind.transfer => [
          if (activity.sourceName != null)
            _DetailRow('Gönderen hesap', activity.sourceName!),
          if (activity.destinationName != null)
            _DetailRow('Alan hesap', activity.destinationName!),
        ],
        ActivityKind.cardPayment => [
          if (activity.sourceName != null)
            _DetailRow('Ödeme hesabı', activity.sourceName!),
          if (activity.destinationName != null)
            _DetailRow('Ödenen kart', activity.destinationName!),
        ],
        ActivityKind.cardCharge => [
          if (activity.sourceName != null)
            _DetailRow('Kredi kartı', activity.sourceName!),
        ],
        // Taksit tek para hareketidir; anapara/faiz onun bölünmesidir.
        // Kırılım burada duruyor çünkü "2.600'ün nesi gider" sorusunun cevabı
        // bu: yalnız faiz gider, anapara alınan paranın geri ödemesi.
        ActivityKind.debtPayment || ActivityKind.debtCollection => [
          if (activity.sourceName != null)
            _DetailRow('Hesap', activity.sourceName!),
          ...?_splitRows(),
        ],
        // Nakit kaynaklı açılışta paranın girdiği/çıktığı hesap var; gider
        // kaynaklı açılışta hesap yoktur, kategori satırı yukarıda zaten
        // yazılıyor.
        ActivityKind.debtOpening => [
          if (activity.sourceName != null)
            _DetailRow('Hesap', activity.sourceName!),
        ],
        ActivityKind.accountTransaction => [
          if (activity.sourceName != null)
            _DetailRow('Hesap', activity.sourceName!),
        ],
        // Borçlandırmanın hesabı yoktur: para el değiştirmedi, yalnız
        // kimin kime borçlandığı yazıldı.
        ActivityKind.counterpartyCharge => [
          if (activity.sourceName != null)
            _DetailRow('Karşı taraf', activity.sourceName!),
        ],
        ActivityKind.counterpartySettlement => [
          if (activity.sourceName != null)
            _DetailRow('Hesap', activity.sourceName!),
          if (activity.destinationName != null)
            _DetailRow('Karşı taraf', activity.destinationName!),
        ],
        ActivityKind.obligation => [
          if (activity.sourceName != null)
            _DetailRow('Karşı taraf', activity.sourceName!),
        ],
        ActivityKind.obligationSettlement => [
          if (activity.sourceName != null)
            _DetailRow('Hesap', activity.sourceName!),
          if (activity.destinationName != null)
            _DetailRow('Karşı taraf', activity.destinationName!),
        ],
        // Satış ve komisyon anında paranın çıktığı bir yer yok: hesap, paranın
        // birkaç gün sonra **geçeceği** yerdir ve satır bunu böyle yazar.
        ActivityKind.posSale || ActivityKind.posCommission => [
          if (activity.destinationName != null)
            _DetailRow('Paranın geçeceği hesap', activity.destinationName!),
        ],
        ActivityKind.posTransfer => [
          if (activity.destinationName != null)
            _DetailRow('Paranın geçtiği hesap', activity.destinationName!),
        ],
      },
      _DetailRow('Köken', activity.origin.label),
      // The title already carries the description when the user wrote one, so
      // repeating it here would be the same sentence twice.
      if (activity.description != null &&
          activity.description!.isNotEmpty &&
          activity.description != activity.title)
        _DetailRow('Açıklama', activity.description!),
      if (activity.cancelledAtUtc != null)
        _DetailRow('İptal zamanı', activity.cancelledAtUtc!),
    ];
    return [for (final row in rows) row];
  }

  /// Borç taksidinin anapara/faiz kırılımı.
  ///
  /// Faizi olmayan taksitte hiç çizilmez: `anapara 2.600 · faiz 0` satırı
  /// bilgi taşımaz, yalnız paneli uzatır.
  List<_DetailRow>? _splitRows() {
    final interest = activity.interestPortion;
    final principal = activity.principalPortion;
    if (interest == null || principal == null || interest == '0.0000') {
      return null;
    }
    final isPayable = activity.kind == ActivityKind.debtPayment;
    return [
      _DetailRow('Anapara', MoneyText.format(principal, activity.currency)),
      _DetailRow(
        isPayable ? 'Faiz (gider)' : 'Faiz (gelir)',
        MoneyText.format(interest, activity.currency),
      ),
    ];
  }
}

class _AmountLine extends StatelessWidget {
  const _AmountLine({required this.activity});

  final FinancialActivity activity;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final (label, color) = switch (activity.effect) {
      ActivityEffect.income => ('Gelir', colors.income),
      ActivityEffect.expense => ('Gider', colors.expense),
      // Spelled out, because a neutral movement moves money without changing
      // what was earned or spent and users read that as a missing number.
      ActivityEffect.neutral => (
        'Gelir/gider raporunu etkilemez',
        colors.neutral,
      ),
    };
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        AppMoneyText(
          amount: activity.amount,
          currency: activity.currency,
          effect: moneyEffectOf(activity.effect),
          isCancelled: activity.isCancelled,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.w700,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Text(label, style: theme.textTheme.bodyMedium?.copyWith(color: color)),
      ],
    );
  }
}

class _DetailRow extends StatelessWidget {
  const _DetailRow(this.label, this.value);

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 140,
            child: Text(
              label,
              style: theme.textTheme.bodySmall?.copyWith(
                color: theme.colorScheme.outline,
              ),
            ),
          ),
          // Long names and descriptions wrap instead of overflowing: this sheet
          // is where the full text is supposed to be readable.
          Expanded(child: Text(value, style: theme.textTheme.bodyMedium)),
        ],
      ),
    );
  }
}

class _Actions extends StatelessWidget {
  const _Actions({
    required this.activity,
    required this.onCancel,
    required this.isCancelling,
  });

  final FinancialActivity activity;
  final Future<void> Function()? onCancel;
  final bool isCancelling;

  @override
  Widget build(BuildContext context) {
    final children = <Widget>[];

    if (activity.supportsAttachments) {
      children.add(
        OutlinedButton.icon(
          // Attachments exist only for budget transactions, and managing them
          // still lives in Veri Araçları.
          onPressed: null,
          icon: const Icon(Icons.attach_file),
          label: const Text('Belgeler Veri Araçları ekranında'),
        ),
      );
    }

    if (onCancel != null) {
      children.add(
        AppSubmitButton(
          label: 'Hareketi iptal et',
          icon: Icons.block,
          isBusy: isCancelling,
          style: AppSubmitButtonStyle.tonal,
          onSubmit: () => _confirm(context),
        ),
      );
    } else if (!activity.isCancelled) {
      // Saying why beats an invisible or dead button.
      children.add(
        Text(switch (activity.origin) {
          ActivityOrigin.recurring =>
            'Tekrarlayan plandan üretilen hareket iptal edilemez.',
          ActivityOrigin.installment =>
            'Taksit planından üretilen hareket iptal edilemez.',
          _ => 'Bu hareket türü iptal edilemez.',
        }, style: Theme.of(context).textTheme.bodySmall),
      );
    }

    if (children.isEmpty) return const SizedBox.shrink();
    return Wrap(
      spacing: AppSpacing.small,
      runSpacing: AppSpacing.small,
      children: children,
    );
  }

  Future<void> _confirm(BuildContext context) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.block,
      destructive: true,
      title: 'Hareket iptal edilsin mi?',
      message:
          'Hareket geçmişte kalır ve silinmez, ancak bakiyeye ve raporlara '
          'katılmaz.',
      confirmLabel: 'İptal et',
    );
    if (!confirmed) return;
    await onCancel!();
  }
}
