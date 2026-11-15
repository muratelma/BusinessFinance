import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_breakpoints.dart';

/// İki bilgi grubu dar alanda veya büyük yazıda alt alta geçer.
class CardInfoPair extends StatelessWidget {
  const CardInfoPair({
    required this.leading,
    required this.trailing,
    super.key,
  });
  final Widget leading;
  final Widget trailing;

  @override
  Widget build(BuildContext context) => LayoutBuilder(
    builder: (context, constraints) {
      final stacked =
          constraints.maxWidth / MediaQuery.textScalerOf(context).scale(1) <
          AppBreakpoints.cardInfoPair;
      if (stacked) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            leading,
            const SizedBox(height: AppSpacing.small),
            Align(alignment: Alignment.centerRight, child: trailing),
          ],
        );
      }
      return Row(
        children: [
          Expanded(child: leading),
          const SizedBox(width: AppSpacing.medium),
          Expanded(
            child: Align(alignment: Alignment.centerRight, child: trailing),
          ),
        ],
      );
    },
  );
}

class CardStatusLine extends StatelessWidget {
  const CardStatusLine({
    required this.icon,
    required this.label,
    this.color,
    super.key,
  });
  final IconData icon;
  final String label;
  final Color? color;

  @override
  Widget build(BuildContext context) => Row(
    mainAxisSize: MainAxisSize.min,
    children: [
      Icon(icon, color: color),
      const SizedBox(width: AppSpacing.small),
      Flexible(
        child: Text(label, style: TextStyle(color: color)),
      ),
    ],
  );
}
