import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';
import 'app_card.dart';
import '../formatters/money_text.dart';
import 'app_money_text.dart';

/// Bir sayıyı ve ne olduğunu gösteren kutu: gelir, gider, net, bakiye.
///
/// İki boyu vardır çünkü hiyerarşi boyutla anlatılır: ekranın söylemek
/// istediği tek şey `hero`, onu destekleyen sayılar `standard`.
enum AppMetricSize { standard, hero }

class AppMetricTile extends StatelessWidget {
  const AppMetricTile({
    required this.label,
    required this.amount,
    required this.currency,
    super.key,
    this.icon,
    this.effect,
    this.caption,
    this.size = AppMetricSize.standard,
    this.onTap,
    this.tinted = false,
  });

  final String label;
  final String amount;
  final String currency;
  final IconData? icon;
  final AppMoneyEffect? effect;

  /// Sayının altındaki bağlam satırı (ör. `Önceki döneme göre`).
  final String? caption;

  final AppMetricSize size;
  final VoidCallback? onTap;

  /// Kutunun zeminini kendi finansal rolünün tonuna boyar.
  ///
  /// Gelir ve gider kutusu yan yana durduğunda hangisinin hangisi olduğu tek
  /// bakışta okunur; tutarı okumaya gerek kalmaz. Bütün kutulara verilmez —
  /// her kart renkliyse renk yine hiçbir şey söylemez.
  final bool tinted;

  bool get _isHero => size == AppMetricSize.hero;

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    final amountStyle = _isHero
        ? AppTypography.heroMoney(textTheme.displaySmall!)
        : AppTypography.money(textTheme.titleLarge!);

    // Ekran okuyucu kutuyu tek cümlede duyar: etiket, tutar ve varsa bağlam.
    // Parçalar ayrı ayrı okunsaydı hangi sayının neye ait olduğu kaybolurdu.
    return Semantics(
      container: true,
      label: [
        '$label: ${MoneyText.format(amount, currency)}',
        if (caption != null) caption,
      ].join('. '),
      child: ExcludeSemantics(child: _card(context, textTheme, amountStyle)),
    );
  }

  Widget _card(
    BuildContext context,
    TextTheme textTheme,
    TextStyle amountStyle,
  ) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    // İkon kapsülü ve (istenirse) zemin, rolün kontrast kapısında ölçülmüş
    // `*Container` / `on*Container` çiftinden gelir. Metin renginin alfa'lı
    // bir kopyası kullanılsaydı her rol için üçüncü bir ton doğardı.
    final (capsule, onCapsule) = switch (effect) {
      AppMoneyEffect.income => (
        colors.incomeContainer,
        colors.onIncomeContainer,
      ),
      AppMoneyEffect.expense => (
        colors.expenseContainer,
        colors.onExpenseContainer,
      ),
      AppMoneyEffect.neutral => (
        colors.neutralContainer,
        colors.onNeutralContainer,
      ),
      null => (surfaces.cardMuted, theme.colorScheme.onSurfaceVariant),
    };
    return AppCard(
      onTap: onTap,
      background: tinted ? capsule : null,
      padding: EdgeInsets.all(_isHero ? AppSpacing.large : AppSpacing.medium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            children: [
              if (icon != null) ...[
                Container(
                  padding: const EdgeInsets.all(AppSpacing.xSmall),
                  decoration: BoxDecoration(
                    color: tinted ? surfaces.card : capsule,
                    borderRadius: BorderRadius.circular(AppRadius.field),
                  ),
                  child: Icon(icon, size: 16, color: onCapsule),
                ),
                const SizedBox(width: AppSpacing.small),
              ],
              Flexible(
                child: Text(
                  label,
                  // Zemin renklenince etiket de kendi çiftine döner; nötr
                  // mürekkep merdiveni renkli zemine göre ölçülmemiştir.
                  style: tinted
                      ? textTheme.labelMedium?.copyWith(color: onCapsule)
                      : textTheme.labelMedium,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.small),
          AppMoneyText(
            amount: amount,
            currency: currency,
            effect: effect,
            style: amountStyle,
            onContainer: tinted,
          ),
          if (caption != null) ...[
            const SizedBox(height: AppSpacing.xSmall),
            Text(
              caption!,
              style: tinted
                  ? textTheme.bodySmall?.copyWith(color: onCapsule)
                  : textTheme.bodySmall,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ],
      ),
    );
  }
}
