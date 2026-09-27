import 'package:flutter/material.dart';

import '../theme/app_breakpoints.dart';
import '../theme/app_spacing.dart';

/// Kart içi satır: başta bir öğe (ikon kapsülü, tarih yaprağı), başlık
/// 16/600 ve isteğe bağlı iki satırlık alt metin, sonda tutar veya ok.
///
/// [AppListRow]'dan farkı başın serbest olmasıdır: tasarımdaki satırlar
/// yuvarlak kapsül, tarih yaprağı veya durum kapsülüyle başlar. Ayırıcı
/// kapsüllü listelerde yazının başladığı yerden çizilir
/// (`AppIconCapsule.rowInset`).
class AppRow extends StatelessWidget {
  const AppRow({
    required this.title,
    super.key,
    this.leading,
    this.subtitle,
    this.trailing,
    this.onTap,
    this.padding = const EdgeInsets.symmetric(
      horizontal: AppSpacing.medium,
      vertical: AppSpacing.small + AppSpacing.xxSmall + AppSpacing.xSmall,
    ),
    this.titleStyle,
    this.semanticLabel,
  });

  final Widget? leading;
  final String title;
  final String? subtitle;
  final Widget? trailing;
  final VoidCallback? onTap;
  final EdgeInsetsGeometry padding;
  final TextStyle? titleStyle;

  /// Satır tek cümleyle okunacaksa; verilmezse parçalar birleştirilir.
  final String? semanticLabel;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    // Büyük yazıda sondaki öğe metnin altına iner; yan yana sığmıyordu.
    final stacked = trailing != null && context.usesLargeText;
    final content = ConstrainedBox(
      constraints: const BoxConstraints(minHeight: 56),
      child: Padding(
        padding: padding,
        child: Row(
          children: [
            if (leading != null) ...[
              leading!,
              const SizedBox(width: AppSpacing.medium),
            ],
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    title,
                    maxLines: stacked ? 2 : 1,
                    overflow: TextOverflow.ellipsis,
                    style: titleStyle ?? theme.textTheme.titleSmall,
                  ),
                  if (subtitle != null)
                    Padding(
                      padding: const EdgeInsets.only(top: AppSpacing.xxSmall),
                      child: Text(
                        subtitle!,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.bodySmall,
                      ),
                    ),
                  if (stacked) ...[
                    const SizedBox(height: AppSpacing.xSmall),
                    Align(alignment: Alignment.centerRight, child: trailing),
                  ],
                ],
              ),
            ),
            if (trailing != null && !stacked) ...[
              const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
              trailing!,
            ],
          ],
        ),
      ),
    );
    // Dokunma alanı satırın metniyle aynı düğümde olmalı: ayrı kalırsa
    // ekran okuyucu adsız bir "düğme" okur.
    final tappable = onTap == null
        ? content
        : InkWell(onTap: onTap, child: content);
    if (semanticLabel == null) {
      return MergeSemantics(
        child: Semantics(button: onTap != null, child: tappable),
      );
    }
    return Semantics(
      container: true,
      button: onTap != null,
      label: semanticLabel,
      onTap: onTap,
      child: ExcludeSemantics(child: tappable),
    );
  }
}
