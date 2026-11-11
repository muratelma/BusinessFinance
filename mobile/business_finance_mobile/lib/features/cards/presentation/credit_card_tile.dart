import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_typography.dart';
import '../data/finance_models.dart';

/// Kredi kartlarım sayfasının üstünde yan yana kayan kartın yüzü.
///
/// Üstte kartın adı, ortada güncel borcu, altta kesim ve son ödeme günü.
/// Limit, ekstre, taksitler ve hareketler kartın altındaki bölümlerdedir.
/// Tutar sunucudan gelir. Yüzey uygulamanın birincil düğmesiyle aynı renk
/// çiftini kullanır, bu yüzden iki temada da okunur.
class CreditCardTile extends StatelessWidget {
  const CreditCardTile({required this.card, super.key, this.onTap});

  final CreditCardItem card;
  final VoidCallback? onTap;

  /// Kartın normal yazı boyundaki yüksekliği.
  static const double height = 196;

  /// Kartın bu ekrandaki yüksekliği; kaydırma şeridi bu boyda kurulur.
  /// Büyük yazı ayarında yazılar sığsın diye kart da uzar.
  static double heightOf(BuildContext context) {
    final scale = MediaQuery.textScalerOf(context).scale(1);
    return height + (scale - 1).clamp(0.0, 2.0) * 120;
  }

  /// `Ayın 15'i`: günün iyelik eki son rakama göre değişir.
  static String dayOfMonth(int day) {
    final suffix = switch (day) {
      10 => "'u",
      20 => "'si",
      _ => switch (day % 10) {
        1 || 5 || 8 => "'i",
        2 || 7 => "'si",
        3 || 4 => "'ü",
        6 => "'sı",
        _ => "'u",
      },
    };
    return 'Ayın $day$suffix';
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surface = theme.colorScheme.primary;
    final ink = theme.colorScheme.onPrimary;
    final soft = ink.withValues(alpha: 0.7);
    final inCredit = MoneyText.isNegative(card.currentDebt);
    final debtLabel = inCredit ? 'Kart alacağınız' : 'Güncel borç';
    final debt = MoneyText.format(
      inCredit ? MoneyText.unsigned(card.currentDebt) : card.currentDebt,
      card.currency,
    );
    final closing = dayOfMonth(card.statementClosingDay);
    final due = dayOfMonth(card.paymentDueDay);
    final label = theme.textTheme.bodySmall?.copyWith(
      fontSize: AppTextSizes.micro,
      color: soft,
    );

    Widget fact(String name, String value, {bool end = false}) => Column(
      crossAxisAlignment: end
          ? CrossAxisAlignment.end
          : CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(name, style: label),
        const SizedBox(height: AppSpacing.xxSmall),
        Text(
          value,
          style: theme.textTheme.titleSmall?.copyWith(
            fontSize: AppTextSizes.body,
            color: ink,
          ),
        ),
      ],
    );

    return Semantics(
      button: onTap != null,
      label:
          '${card.name}${card.isActive ? '' : ', pasif'}. $debtLabel $debt. '
          'Hesap kesimi $closing, son ödeme $due.',
      excludeSemantics: true,
      child: Opacity(
        opacity: card.isActive ? 1 : 0.55,
        child: Material(
          color: surface,
          borderRadius: BorderRadius.circular(AppRadius.card),
          clipBehavior: Clip.antiAlias,
          child: InkWell(
            onTap: onTap,
            child: SizedBox(
              height: heightOf(context),
              child: Stack(
                children: [
                  // Köşedeki iki soluk daire: yüzeyi düz bir kutudan ayıran
                  // tek süs.
                  Positioned(
                    right: -56,
                    top: -72,
                    child: _Glow(size: 208, color: ink),
                  ),
                  Positioned(
                    right: 48,
                    bottom: -96,
                    child: _Glow(size: 168, color: ink),
                  ),
                  Padding(
                    padding: const EdgeInsets.all(AppSpacing.large),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        Row(
                          children: [
                            Expanded(
                              child: Text(
                                card.isActive
                                    ? card.name
                                    : '${card.name} · Pasif',
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: theme.textTheme.titleSmall?.copyWith(
                                  fontSize: AppTextSizes.row,
                                  color: ink,
                                ),
                              ),
                            ),
                            const SizedBox(width: AppSpacing.small),
                            Icon(Icons.contactless_outlined, color: soft),
                          ],
                        ),
                        const Spacer(),
                        Text(debtLabel, maxLines: 1, style: label),
                        const SizedBox(height: AppSpacing.xxSmall),
                        FittedBox(
                          fit: BoxFit.scaleDown,
                          alignment: Alignment.centerLeft,
                          child: Text(
                            debt,
                            maxLines: 1,
                            style: theme.textTheme.headlineSmall?.copyWith(
                              fontSize: AppTextSizes.amount,
                              fontWeight: FontWeight.w700,
                              height: 1.2,
                              color: ink,
                            ),
                          ),
                        ),
                        const Spacer(),
                        Row(
                          crossAxisAlignment: CrossAxisAlignment.end,
                          children: [
                            Expanded(
                              child: FittedBox(
                                fit: BoxFit.scaleDown,
                                alignment: Alignment.bottomLeft,
                                child: fact('Hesap kesimi', closing),
                              ),
                            ),
                            const SizedBox(width: AppSpacing.small),
                            Expanded(
                              child: FittedBox(
                                fit: BoxFit.scaleDown,
                                alignment: Alignment.bottomRight,
                                child: fact('Son ödeme', due, end: true),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _Glow extends StatelessWidget {
  const _Glow({required this.size, required this.color});

  final double size;
  final Color color;

  @override
  Widget build(BuildContext context) => Container(
    width: size,
    height: size,
    decoration: BoxDecoration(
      shape: BoxShape.circle,
      color: color.withValues(alpha: 0.06),
    ),
  );
}
