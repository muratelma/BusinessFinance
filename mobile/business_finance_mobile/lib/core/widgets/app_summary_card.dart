import '../theme/app_spacing.dart';
import 'package:flutter/material.dart';
import 'app_card.dart';
import 'app_money_text.dart';

/// Sayfa özetlerinin ortak yüzeyi; yalnız hazır tutarları gösterir.
class AppSummaryCard extends StatelessWidget {
  const AppSummaryCard({
    required this.label,
    required this.amount,
    super.key,
    this.effect,
    this.subtitle,
    this.footer,
    this.signedByAmount = false,
  });
  final String label;
  final String amount;
  final AppMoneyEffect? effect;
  final String? subtitle;
  final Widget? footer;
  final bool signedByAmount;
  @override
  Widget build(BuildContext context) => AppCard(
    padding: const EdgeInsets.symmetric(
      horizontal: AppSpacing.medium + AppSpacing.xSmall,
      vertical: AppSpacing.medium,
    ),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(label, style: Theme.of(context).textTheme.bodySmall),
        const SizedBox(height: AppSpacing.xSmall),
        AppMoneyText(
          amount: amount,
          currency: 'TRY',
          effect: effect,
          size: AppMoneySize.metric,
          signedByAmount: signedByAmount,
          style: const TextStyle(fontSize: 28, fontWeight: FontWeight.w700),
        ),
        if (subtitle != null) ...[
          const SizedBox(height: AppSpacing.xxSmall),
          Text(subtitle!, style: Theme.of(context).textTheme.bodySmall),
        ],
        if (footer != null) ...[
          const SizedBox(height: AppSpacing.medium),
          footer!,
        ],
      ],
    ),
  );
}
