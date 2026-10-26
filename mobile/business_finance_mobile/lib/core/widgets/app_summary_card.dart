import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'package:flutter/material.dart';
import 'app_card.dart';
import 'app_money_text.dart';

/// Sayfa özetlerinin ortak yüzeyi; yalnız hazır tutarları gösterir.
///
/// Teslimin ölçüsü: 16/20 iç boşluk, etiket ve alt satır 14/1,35 soluk,
/// tutar 28/700 ve 1,25 satır yüksekliği. Alt hücreler ince çizginin altında,
/// çizgiden 12 dp aşağıda durur.
class AppSummaryCard extends StatelessWidget {
  const AppSummaryCard({
    required this.label,
    required this.amount,
    super.key,
    this.effect,
    this.subtitle,
    this.footer,
    this.signedByAmount = false,
    this.subtitleIcon,
    this.trailing,
  });
  final String label;
  final String amount;
  final AppMoneyEffect? effect;
  final String? subtitle;
  final Widget? footer;
  final bool signedByAmount;

  /// Alt satırın önündeki 16 dp ikon (yön oku gibi); renk taşımaz.
  final IconData? subtitleIcon;

  /// Sol bloğun sağında, alt kenarından hizalı küçük görsel (16 dp ara).
  final Widget? trailing;

  /// Kartın etiket, alt satır ve dip notu yazısı.
  static TextStyle? helperStyle(BuildContext context) => Theme.of(context)
      .textTheme
      .bodySmall
      ?.copyWith(height: 1.35, color: AppSurfaces.of(context).inkMuted);

  /// Alt hücrelerin 13 dp etiketi (`Ekim giren`, `Nakit`).
  static TextStyle? smallStyle(BuildContext context) =>
      helperStyle(context)?.copyWith(fontSize: 13);

  @override
  Widget build(BuildContext context) {
    final main = _main(context);
    return AppCard(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium + AppSpacing.xSmall,
        vertical: AppSpacing.medium,
      ),
      child: trailing == null
          ? main
          : Row(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Expanded(child: main),
                const SizedBox(width: AppSpacing.medium),
                trailing!,
              ],
            ),
    );
  }

  Widget _main(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      Text(label, style: helperStyle(context)),
      const SizedBox(height: AppSpacing.xSmall),
      AppMoneyText(
        amount: amount,
        currency: 'TRY',
        effect: effect,
        size: AppMoneySize.metric,
        signedByAmount: signedByAmount,
        style: const TextStyle(
          fontSize: 28,
          height: 1.25,
          fontWeight: FontWeight.w700,
        ),
      ),
      if (subtitle != null) ...[
        const SizedBox(height: AppSpacing.xxSmall),
        if (subtitleIcon == null)
          Text(subtitle!, style: helperStyle(context))
        else
          Row(
            children: [
              Icon(subtitleIcon, size: 16, color: AppSurfaces.of(context).ink),
              const SizedBox(width: AppSpacing.xSmall),
              Expanded(child: Text(subtitle!, style: helperStyle(context))),
            ],
          ),
      ],
      if (footer != null) ...[
        const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
        const Divider(height: 1),
        const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
        footer!,
      ],
    ],
  );
}
