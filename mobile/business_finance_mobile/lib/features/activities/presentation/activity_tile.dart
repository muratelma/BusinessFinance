import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_finance_icons.dart';
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
    this.title,
    this.subtitle,
    this.subtitleIcon,
    this.stateLabel,
    this.stateTone,
    this.amount,
    this.effect,
    this.signedByAmount = false,
    this.amountNote,
    this.hideFee = false,
    this.compact = false,
    this.alignTop = false,
    this.icon,
    this.mutedIcon = false,
  });

  final FinancialActivity activity;
  final VoidCallback? onTap;

  /// Gün başlığıyla gruplanmış listede tarih satırda tekrar yazılmaz.
  final bool showDate;
  final String? title;
  final String? subtitle;
  final IconData? subtitleIcon;
  final String? stateLabel;
  final AppStatusTone? stateTone;
  final String? amount;
  final ActivityEffect? effect;
  final bool signedByAmount;
  final String? amountNote;
  final bool hideFee;
  final bool compact;
  final bool alignTop;
  final IconData? icon;
  final bool mutedIcon;

  /// Üste hizalı satırda (kategori sayfası) kapsül ile yazının arası.
  static const double _topGap =
      AppSpacing.small + AppSpacing.xSmall + AppSpacing.xxSmall;

  /// Üste hizalı satırda yazı kapsülün 1 dp altından başlar.
  static const double _nudge = AppSpacing.xxSmall / 2;

  @override
  Widget build(BuildContext context) {
    final cancelled = activity.isCancelled;
    final dim = cancelled ? 0.55 : 1.0;
    final nudge = alignTop ? _nudge : 0.0;

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
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              mainAxisSize: MainAxisSize.min,
              children: [
                Row(
                  crossAxisAlignment: !alignTop && stateLabel == null
                      ? CrossAxisAlignment.center
                      : CrossAxisAlignment.start,
                  children: [
                    Opacity(
                      opacity: dim,
                      child: AppIconCapsule(
                        icon: icon ?? _icon,
                        tone: mutedIcon
                            ? null
                            : cancelled
                            ? AppStatusTone.cancelled
                            : switch (effect ?? activity.effect) {
                                ActivityEffect.income => AppStatusTone.income,
                                ActivityEffect.expense => AppStatusTone.expense,
                                ActivityEffect.neutral => AppStatusTone.neutral,
                              },
                      ),
                    ),
                    SizedBox(width: alignTop ? _topGap : AppSpacing.medium),
                    Expanded(
                      child: Padding(
                        // Üste hizalı satırda yazı kapsülün 1 dp altından
                        // başlar (teslim).
                        padding: EdgeInsets.only(top: nudge),
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
                                  _titleText(context),
                                  if ((subtitle ?? _subtitle).isNotEmpty) ...[
                                    SizedBox(
                                      height: compact ? 0 : AppSpacing.xxSmall,
                                    ),
                                    _subtitleText(context),
                                  ],
                                ],
                              ),
                            ),
                            if (stateLabel != null && !alignTop)
                              Padding(
                                padding: const EdgeInsets.only(
                                  top: AppSpacing.xSmall,
                                ),
                                child: Text(
                                  stateLabel!,
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                  style: Theme.of(context).textTheme.bodySmall
                                      ?.copyWith(
                                        fontWeight: FontWeight.w600,
                                        color: stateTone == AppStatusTone.income
                                            ? AppFinanceColors.of(
                                                context,
                                              ).income
                                            : stateTone == AppStatusTone.neutral
                                            ? AppFinanceColors.of(
                                                context,
                                              ).neutral
                                            : null,
                                      ),
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
                    ),
                    if (!context.usesLargeText) ...[
                      SizedBox(
                        width: !compact
                            ? AppSpacing.small + AppSpacing.xSmall
                            : alignTop
                            ? _topGap
                            : AppSpacing.medium,
                      ),
                      Padding(
                        padding: EdgeInsets.only(top: nudge),
                        child: Opacity(opacity: dim, child: _amount),
                      ),
                    ],
                  ],
                ),
                if (stateLabel != null && alignTop)
                  Padding(
                    padding: const EdgeInsets.only(
                      left:
                          AppIconCapsule.rowInset -
                          AppSpacing.medium -
                          AppSpacing.xxSmall,
                    ),
                    child: Text(
                      stateLabel!,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        fontSize: 13,
                        height: 1.35,
                        fontWeight: FontWeight.w600,
                        color: stateTone == AppStatusTone.income
                            ? AppFinanceColors.of(context).income
                            : stateTone == AppStatusTone.neutral
                            ? AppFinanceColors.of(context).neutral
                            : null,
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

  /// Tutar ve, varsa, kaydın parçası olan gider (komisyon / kesinti).
  ///
  /// Parça ayrı bir satır değildir: beş satışın beş komisyonu alt alta beş
  /// kırmızı satır olunca hangisinin hangi satışa ait olduğu okunmuyordu.
  /// Tutarın altında durur ki sol alt satır kısa kalsın ve taşmasın.
  Widget get _amount => Builder(
    builder: (context) {
      final amount = AppMoneyText(
        amount: signedByAmount
            ? (this.amount ?? activity.amount)
            : (this.amount?.replaceFirst('-', '') ?? activity.amount),
        currency: activity.currency,
        effect: moneyEffectOf(effect ?? activity.effect),
        isCancelled: activity.isCancelled,
        signed: true,
        signedByAmount: signedByAmount,
        textAlign: TextAlign.end,
        size: AppMoneySize.row,
        style: compact ? const TextStyle(height: 1.25) : null,
      );
      final fee = amountNote ?? (hideFee ? null : _feeLabel);
      if (fee == null) return amount;
      return Column(
        crossAxisAlignment: CrossAxisAlignment.end,
        mainAxisSize: MainAxisSize.min,
        children: [
          amount,
          const SizedBox(height: AppSpacing.xxSmall),
          Text(
            fee,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
              fontSize: compact ? 13 : null,
              height: compact ? 1.35 : null,
              decoration: activity.isCancelled
                  ? TextDecoration.lineThrough
                  : null,
            ),
          ),
        ],
      );
    },
  );

  /// `komisyon ₺97,50` (POS satışı) · `kesinti ₺7,50` (yatış).
  String? get _feeLabel {
    if (activity.interestPortion != null &&
        activity.interestPortion != '0.0000') {
      return 'faizi ${MoneyText.format(activity.interestPortion!, activity.currency)}';
    }
    if (!activity.hasFee) return null;
    final name = activity.kind == ActivityKind.posDeposit
        ? 'kesinti'
        : activity.kind == ActivityKind.transfer
        ? 'masraf'
        : 'komisyon';
    return '$name ${MoneyText.format(activity.feeAmount!, activity.currency)}';
  }

  IconData get _icon => activityIcon(activity);

  Widget _titleText(BuildContext context) {
    final text =
        title ?? (activity.title.isEmpty ? activity.kindLabel : activity.title);
    final style = compact
        ? Theme.of(context).textTheme.titleSmall?.copyWith(height: 1.3)
        : Theme.of(context).textTheme.titleSmall;
    if (compact && text.contains(' → ')) {
      final parts = text.split(' → ');
      return Text.rich(
        TextSpan(
          children: [
            TextSpan(text: '${parts.first} '),
            WidgetSpan(
              alignment: PlaceholderAlignment.middle,
              child: Icon(Icons.arrow_forward, size: 16, color: style?.color),
            ),
            TextSpan(text: ' ${parts.skip(1).join(' → ')}'),
          ],
        ),
        maxLines: 2,
        overflow: TextOverflow.ellipsis,
        style: style,
      );
    }
    return Text(
      text,
      maxLines: 2,
      overflow: TextOverflow.ellipsis,
      style: style,
    );
  }

  Widget _subtitleText(BuildContext context) {
    final style = compact
        ? Theme.of(context).textTheme.bodySmall?.copyWith(height: 1.35)
        : Theme.of(context).textTheme.bodySmall;
    final text = Text(
      subtitle ?? _subtitle,
      maxLines: 2,
      overflow: TextOverflow.ellipsis,
      style: style,
    );
    if (subtitleIcon == null) return text;
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(subtitleIcon, size: 16, color: style?.color),
        const SizedBox(width: AppSpacing.xSmall),
        Expanded(child: text),
      ],
    );
  }

  String get _subtitle {
    // Source and destination read as one route, so they are joined by the arrow
    // rather than separated like unrelated facts.
    final source = activity.sourceName;
    final destination = activity.destinationName;
    final route = switch ((source, destination)) {
      // Kartla tahsilde para POS'tan geçer: yer POS'tur, hesap ayrıntıdadır.
      (final String account, final String person)
          when activity.isCardCollection =>
        '$person → ${activity.channelName ?? '$account (kartla)'}',
      // Tahsilatta para karşı taraftan hesaba gelir: ok ters yazılır.
      (final String account, final String person) when activity.isCollection =>
        '$person → $account',
      (final String from, final String to) => '$from → $to',
      (final String from, null) => from,
      (null, final String to) => to,
      _ => null,
    };
    // POS satışında satırı tanıtan şey POS'tur; paranın geçeceği hesap
    // ayrıntıdadır. POS seçilmeden girilen satışta hesap yazılır.
    final place = activity.kind == ActivityKind.posSale
        ? (activity.channelName ?? route)
        : route;
    return [
      ?_categoryLabel,
      // Kaydın bir gün sonuna ait olduğu satırda okunur.
      if (activity.isWrittenByDayClose) 'Gün sonu',
      // Tek POS'un tahsilatlarını kapatan yatışta POS'un adı.
      if (activity.kind == ActivityKind.posDeposit) ?activity.channelName,
      // Başlık zaten bu adsa tekrar yazılmaz.
      if (place != null && place != activity.title) place,
      ?_settlementCountLabel,
      if (showDate) _readableDate,
    ].join(' • ');
  }

  /// Birden çok tahsilatı kapatan yatışta kaç tahsilat olduğu.
  String? get _settlementCountLabel {
    final count = activity.settlementCount;
    return count == null || count < 2 ? null : '$count tahsilat';
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
      ..write(activity.kindLabel)
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
    if (title != null) buffer.write('. $title');
    if (subtitle != null) buffer.write('. $subtitle');
    if (stateLabel != null) buffer.write('. $stateLabel');
    if (amount != null) {
      buffer.write(
        '. Satır tutarı ${MoneyText.format(amount!, activity.currency)}',
      );
    }
    if (amountNote != null) buffer.write('. $amountNote');
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
    activity.categoryName != null
        ? AppFinanceIcons.forCategory(activity.categoryName!)
        : activity.effect == ActivityEffect.income
        ? Icons.south_west
        : Icons.north_east,
  ActivityKind.transfer => Icons.swap_horiz,
  ActivityKind.cardCharge => Icons.credit_card,
  ActivityKind.cardPayment => Icons.credit_card,
  ActivityKind.debtPayment => Icons.request_quote_outlined,
  ActivityKind.debtCollection => Icons.request_quote_outlined,
  ActivityKind.debtOpening => Icons.handshake_outlined,
  ActivityKind.counterpartyCharge =>
    activity.effect == ActivityEffect.income
        ? Icons.south_west
        : Icons.north_east,
  ActivityKind.counterpartySettlement => Icons.groups_outlined,
  ActivityKind.obligation => Icons.event_note_outlined,
  ActivityKind.obligationSettlement => Icons.receipt_long_outlined,
  ActivityKind.posSale => Icons.point_of_sale_outlined,
  // Para yolda değil artık: hesaba indi.
  ActivityKind.posDeposit => Icons.move_to_inbox_outlined,
};
