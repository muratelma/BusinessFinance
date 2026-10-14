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
    this.titleMaxLines = 1,
    this.minHeight = 56,
    this.compact = false,
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
  final int titleMaxLines;
  final double minHeight;
  final bool compact;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    // Büyük yazıda sondaki öğe metnin altına iner; yan yana sığmıyordu.
    final stacked = trailing != null && context.usesLargeText;
    final content = ConstrainedBox(
      constraints: BoxConstraints(minHeight: minHeight),
      child: Padding(
        padding: padding,
        child: Row(
          children: [
            if (leading != null) ...[
              leading!,
              SizedBox(width: compact ? 14 : AppSpacing.medium),
            ],
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    title,
                    maxLines: stacked ? 2 : titleMaxLines,
                    overflow: TextOverflow.ellipsis,
                    style:
                        titleStyle ??
                        (compact
                            ? theme.textTheme.titleSmall?.copyWith(height: 1.3)
                            : theme.textTheme.titleSmall),
                  ),
                  if (subtitle != null)
                    Padding(
                      padding: EdgeInsets.only(
                        top: compact ? 0 : AppSpacing.xxSmall,
                      ),
                      child: Text(
                        subtitle!,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: compact
                            ? theme.textTheme.bodySmall?.copyWith(height: 1.35)
                            : theme.textTheme.bodySmall,
                      ),
                    ),
                  if (stacked) ...[
                    const SizedBox(height: AppSpacing.xSmall),
                    Align(
                      alignment: Alignment.centerRight,
                      child: FittedBox(fit: BoxFit.scaleDown, child: trailing),
                    ),
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
