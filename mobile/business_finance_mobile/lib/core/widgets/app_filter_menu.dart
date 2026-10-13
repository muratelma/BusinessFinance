import '../theme/app_spacing.dart';
import 'package:flutter/material.dart';
import '../theme/app_surfaces.dart';
import 'app_menu_group_label.dart';
import 'app_money_text.dart';
import 'app_row.dart';

class AppFilterOption {
  const AppFilterOption({
    required this.key,
    required this.label,
    this.amount,
    this.group,
    this.icon = Icons.account_balance_outlined,
    this.effect,
    this.subtitle,
    this.signedByAmount = false,
  });
  final String? key;
  final String label;
  final String? amount;
  final String? group;
  final IconData icon;
  final AppMoneyEffect? effect;
  final String? subtitle;
  final bool signedByAmount;
}

/// Çipin altında açılan menü. Başlıklar seçim değildir; tutarlar sunucudan gelir.
class AppFilterMenu extends StatelessWidget {
  const AppFilterMenu({
    required this.label,
    required this.options,
    required this.selected,
    required this.onSelected,
    this.muted = false,
    this.onUnavailable,
    super.key,
  });
  final String label;
  final List<AppFilterOption> options;
  final String? selected;
  final ValueChanged<String?> onSelected;
  final bool muted;
  final VoidCallback? onUnavailable;
  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final children = <Widget>[];
    String? group;
    for (final option in options) {
      if (option.group != null && group != option.group) {
        group = option.group;
        children.add(
          Padding(
            padding: const EdgeInsets.only(
              left: AppSpacing.medium,
              top: AppSpacing.small + AppSpacing.xSmall,
              right: AppSpacing.medium,
              bottom: AppSpacing.xSmall,
            ),
            child: AppMenuGroupLabel(group!, compact: true),
          ),
        );
      }
      children.add(
        MenuItemButton(
          style: const ButtonStyle(
            padding: WidgetStatePropertyAll(
              EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            ),
          ),
          onPressed: () => onSelected(option.key),
          child: SizedBox(
            width: 280,
            child: AppRow(
              minHeight: 48,
              title: option.label,
              subtitle: option.subtitle,
              padding: const EdgeInsets.symmetric(
                vertical: AppSpacing.xSmall + AppSpacing.xxSmall,
              ),
              leading: Icon(
                selected == option.key ? Icons.check : option.icon,
                size: 20,
              ),
              trailing: option.amount == null
                  ? null
                  : AppMoneyText(
                      amount: option.amount!,
                      currency: 'TRY',
                      size: AppMoneySize.body,
                      effect: option.effect,
                      signedByAmount: option.signedByAmount,
                    ),
            ),
          ),
        ),
      );
    }
    return MenuAnchor(
      alignmentOffset: const Offset(-16, 4),
      style: MenuStyle(
        backgroundColor: WidgetStatePropertyAll(surfaces.card),
        side: WidgetStatePropertyAll(BorderSide(color: surfaces.border)),
        alignment: AlignmentDirectional.bottomEnd,
        maximumSize: const WidgetStatePropertyAll(Size(360, 560)),
        shape: WidgetStatePropertyAll(
          RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        ),
      ),
      menuChildren: children,
      builder: (context, controller, _) => OutlinedButton(
        onPressed: muted
            ? onUnavailable
            : () => controller.isOpen ? controller.close() : controller.open(),
        style: OutlinedButton.styleFrom(
          minimumSize: const Size(0, 36),
          tapTargetSize: MaterialTapTargetSize.padded,
          shape: const StadiumBorder(),
          foregroundColor: muted ? surfaces.inkMuted : surfaces.ink,
          backgroundColor: selected == null
              ? surfaces.card
              : surfaces.cardMuted,
          padding: const EdgeInsets.symmetric(
            horizontal: AppSpacing.small + AppSpacing.xSmall,
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(label),
            const SizedBox(width: AppSpacing.xSmall),
            Icon(
              controller.isOpen ? Icons.expand_less : Icons.expand_more,
              size: 18,
            ),
          ],
        ),
      ),
    );
  }
}
