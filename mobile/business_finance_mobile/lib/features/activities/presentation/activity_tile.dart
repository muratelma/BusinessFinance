import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/activity_models.dart';

/// Feed'in etki boyutunu tasarım sisteminin para/renk sözleşmesine çevirir.
AppMoneyEffect moneyEffectOf(ActivityEffect effect) => switch (effect) {
  ActivityEffect.income => AppMoneyEffect.income,
  ActivityEffect.expense => AppMoneyEffect.expense,
  ActivityEffect.neutral => AppMoneyEffect.neutral,
};

/// One row of the unified history. Deliberately plain: name, source, date and
/// amount. Description, origin, destination and anything technical belong to the
/// detail sheet, not to a list the user scans.
class ActivityTile extends StatelessWidget {
  const ActivityTile({required this.activity, super.key, this.onTap});

  final FinancialActivity activity;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final cancelled = activity.isCancelled;
    final capsule = _capsule(context);

    return Semantics(
      button: onTap != null,
      label: _semanticsLabel(),
      excludeSemantics: true,
      child: AppListRow(
        onTap: onTap,
        dimmed: cancelled,
        icon: _icon,
        iconColor: capsule.foreground,
        // İkon kapsülü hareketin etkisini taşır; renk tek başına anlam
        // vermez, ikon da türü gösterir.
        //
        // Kapsül zemini metin renginin alfa'lı bir kopyası değil, kendi
        // `*Container` token'ıdır. Alfa yolu her rol için üçüncü bir ton
        // üretiyordu ve zeminden zemine kayıyordu; container/onContainer
        // çifti ise kontrast kapısında ölçülen tek bir çifttir.
        iconBackground: capsule.background,
        title: activity.title.isEmpty ? activity.kind.label : activity.title,
        // Kayıt adı kullanıcının yazdığı serbest metindir ve bir cümle
        // olabilir. Tek satıra sığdırılır; tamamı ayrıntı panelindedir.
        titleMaxLines: 1,
        subtitle: _subtitle,
        badge: cancelled
            ? const AppStatusChip(
                label: 'İptal edildi',
                icon: Icons.block,
                tone: AppStatusTone.cancelled,
              )
            : null,
        trailing: AppMoneyText(
          amount: activity.amount,
          currency: activity.currency,
          effect: moneyEffectOf(activity.effect),
          isCancelled: cancelled,
          signed: true,
          textAlign: TextAlign.end,
          style: theme.textTheme.titleSmall,
        ),
      ),
    );
  }

  /// Yeşil yalnız gelirdir. Nötr hareket kendi tonunu alır, böylece bir
  /// transfer para girişiyle karıştırılmaz.
  ({Color background, Color foreground}) _capsule(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    return switch (activity.effect) {
      ActivityEffect.income => (
        background: colors.incomeContainer,
        foreground: colors.onIncomeContainer,
      ),
      ActivityEffect.expense => (
        background: colors.expenseContainer,
        foreground: colors.onExpenseContainer,
      ),
      ActivityEffect.neutral => (
        background: colors.neutralContainer,
        foreground: colors.onNeutralContainer,
      ),
    };
  }

  IconData get _icon => switch (activity.kind) {
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
      _readableDate,
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
