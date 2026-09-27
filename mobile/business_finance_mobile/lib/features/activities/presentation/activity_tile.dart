import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/activity_models.dart';

/// Feed'in etki boyutunu tasarım sisteminin para/renk sözleşmesine çevirir.
AppMoneyEffect moneyEffectOf(ActivityEffect effect) => switch (effect) {
  ActivityEffect.income => AppMoneyEffect.income,
  ActivityEffect.expense => AppMoneyEffect.expense,
  ActivityEffect.neutral => AppMoneyEffect.neutral,
};

/// One row of the unified history. Deliberately plain: name, source and
/// amount. Description, origin, destination and anything technical belong to the
/// detail sheet, not to a list the user scans.
///
/// Tasarım teslimi (27 Eylül 2026): yuvarlak rol kapsülü 40, başlık 16/600,
/// alt satır kategori ve hesap, sağda işaretli tutar. İptal edilmiş kayıt
/// soluk (`0.55`), tutarı üstü çizili ve altında `İptal edildi` rozeti.
class ActivityTile extends StatelessWidget {
  const ActivityTile({
    required this.activity,
    super.key,
    this.onTap,
    this.showDate = true,
  });

  final FinancialActivity activity;
  final VoidCallback? onTap;

  /// Gün başlığıyla gruplanmış listede tarih satırda tekrar yazılmaz.
  final bool showDate;

  @override
  Widget build(BuildContext context) {
    final cancelled = activity.isCancelled;
    final dim = cancelled ? 0.55 : 1.0;

    return Semantics(
      button: onTap != null,
      label: _semanticsLabel(),
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 64),
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppSpacing.medium,
              vertical: AppSpacing.small + AppSpacing.xSmall,
            ),
            child: Row(
              children: [
                Opacity(
                  opacity: dim,
                  child: AppIconCapsule(
                    icon: _icon,
                    tone: cancelled
                        ? AppStatusTone.cancelled
                        : switch (activity.effect) {
                            ActivityEffect.income => AppStatusTone.income,
                            ActivityEffect.expense => AppStatusTone.expense,
                            ActivityEffect.neutral => AppStatusTone.neutral,
                          },
                  ),
                ),
                const SizedBox(width: AppSpacing.medium),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Opacity(
                        opacity: dim,
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            // Kayıt adı kullanıcının yazdığı serbest metindir
                            // ve bir cümle olabilir; tamamı ayrıntıdadır.
                            Text(
                              activity.title.isEmpty
                                  ? activity.kind.label
                                  : activity.title,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: Theme.of(context).textTheme.titleSmall,
                            ),
                            if (_subtitle.isNotEmpty) ...[
                              const SizedBox(height: AppSpacing.xxSmall),
                              Text(
                                _subtitle,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: Theme.of(context).textTheme.bodySmall,
                              ),
                            ],
                          ],
                        ),
                      ),
                      if (cancelled) ...[
                        const SizedBox(height: AppSpacing.xSmall),
                        const AppStatusChip(
                          label: 'İptal edildi',
                          icon: Icons.block,
                          tone: AppStatusTone.cancelled,
                        ),
                      ],
                      if (context.usesLargeText) ...[
                        const SizedBox(height: AppSpacing.xSmall),
                        Align(
                          alignment: Alignment.centerRight,
                          child: Opacity(opacity: dim, child: _amount),
                        ),
                      ],
                    ],
                  ),
                ),
                if (!context.usesLargeText) ...[
                  const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                  Opacity(opacity: dim, child: _amount),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget get _amount => AppMoneyText(
    amount: activity.amount,
    currency: activity.currency,
    effect: moneyEffectOf(activity.effect),
    isCancelled: activity.isCancelled,
    signed: true,
    textAlign: TextAlign.end,
    size: AppMoneySize.row,
  );

  IconData get _icon => activityIcon(activity);

  String get _subtitle {
    // Source and destination read as one route, so they are joined by the arrow
    // rather than separated like unrelated facts.
    final source = activity.sourceName;
    final destination = activity.destinationName;
    final route = switch ((source, destination)) {
      (final String from, final String to) => '$from → $to',
      (final String from, null) => from,
      (null, final String to) => to,
      _ => null,
    };
    return [
      ?_categoryLabel,
      ?route,
      if (showDate) _readableDate,
      ?_interestLabel,
    ].join(' • ');
  }

  /// Kategori satırın ilk bilgisidir çünkü hareketin **ne olduğunu** o söyler.
  ///
  /// Başlık kullanıcının yazdığı addır ("İstanbulkart'a yükleme yaptım") ve
  /// hangi kovaya düştüğünü söylemez. Kullanıcı bir açıklama yazdığı anda
  /// kategori satırdan tamamen kayboluyordu; oysa listeyi tarayan kişinin
  /// aradığı ilk şey odur.
  ///
  /// Kullanıcı açıklama yazmadıysa sunucu başlığı zaten kategori adından
  /// üretir; o durumda kategori tekrar yazılmaz.
  String? get _categoryLabel {
    final category = activity.categoryName;
    if (category == null || category.isEmpty) return null;
    return category == activity.title ? null : category;
  }

  String get _readableDate => DateText.dayMonth(activity.activityDate);

  /// Borç taksidinin faiz payı.
  ///
  /// Faiz ayrı bir satır değil — ayrı yazılsaydı işlemler toplamı hesaptan
  /// çıkan parayla tutmazdı (2.600 + 100 = 2.700). Muhasebede de tek kaydın
  /// bir ayağıdır. Ama tutarın hangi kısmının gerçekten gider olduğu, ödemenin
  /// göründüğü yerde okunabilmeli; yoksa 2.600'ün nesi gider anlaşılmıyor.
  String? get _interestLabel {
    final interest = activity.interestPortion;
    if (interest == null || interest == '0.0000') return null;
    return 'faizi ${MoneyText.format(interest, activity.currency)}';
  }

  /// One sentence carrying kind, amount, date, source and cancellation, so a
  /// screen reader conveys everything the colour and icon imply visually.
  String _semanticsLabel() {
    final buffer = StringBuffer()
      ..write(activity.kind.label)
      ..write('. ')
      ..write(activity.title.isEmpty ? '' : '${activity.title}. ')
      ..write(switch (activity.effect) {
        ActivityEffect.income => 'Gelir',
        ActivityEffect.expense => 'Gider',
        ActivityEffect.neutral => 'Bakiye etkisi nötr',
      })
      ..write(' ')
      ..write(MoneyText.format(activity.amount, activity.currency))
      ..write('. ')
      ..write(_readableDate);
    if (_interestLabel case final interest?) {
      buffer.write('. Bunun $interest');
    }
    if (_categoryLabel case final category?) {
      buffer.write('. Kategori $category');
    }
    if (activity.sourceName != null) {
      buffer.write('. Kaynak ${activity.sourceName}');
    }
    if (activity.destinationName != null) {
      buffer.write('. Hedef ${activity.destinationName}');
    }
    if (activity.isCancelled) buffer.write('. İptal edildi');
    return buffer.toString();
  }
}

/// Hareketin türünü gösteren ikon; liste satırı ve ayrıntı paneli aynı ikonu
/// kullanır.
IconData activityIcon(FinancialActivity activity) => switch (activity.kind) {
  ActivityKind.accountTransaction =>
    activity.effect == ActivityEffect.income
        ? Icons.south_west
        : Icons.north_east,
  ActivityKind.transfer => Icons.swap_horiz,
  ActivityKind.cardCharge => Icons.credit_card,
  ActivityKind.cardPayment => Icons.payments_outlined,
  ActivityKind.debtPayment => Icons.trending_down,
  ActivityKind.debtCollection => Icons.trending_up,
  ActivityKind.debtOpening => Icons.handshake_outlined,
  ActivityKind.counterpartyCharge =>
    activity.effect == ActivityEffect.income
        ? Icons.south_west
        : Icons.north_east,
  ActivityKind.counterpartySettlement => Icons.price_check,
  ActivityKind.obligation => Icons.event_note_outlined,
  ActivityKind.obligationSettlement => Icons.price_check,
  ActivityKind.posSale => Icons.point_of_sale_outlined,
  ActivityKind.posCommission => Icons.percent,
  // Para yolda değil artık: hesaba indi.
  ActivityKind.posTransfer => Icons.move_to_inbox_outlined,
};
