import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Listelerin ortak satırı: ikon kapsülü, başlık, alt satır ve sağ blok.
///
/// `ListTile`'ın yerine geçmez, onun üstüne uygulamanın ritmini koyar: ikon
/// her zaman aynı boyda bir kapsülün içindedir, böylece farklı uzunluktaki
/// başlıklar arasında sol kenar hizası bozulmaz.
class AppListRow extends StatelessWidget {
  const AppListRow({
    required this.title,
    super.key,
    this.subtitle,
    this.icon,
    this.iconColor,
    this.iconBackground,
    this.trailing,
    this.badge,
    this.onTap,
    this.dimmed = false,
    this.titleMaxLines = 2,
  });

  final String title;

  /// Başlık kaç satıra kadar uzayabilir.
  ///
  /// Kullanıcının serbest metin yazdığı listelerde `1` verilir: kayıt adı
  /// bir cümle olabilir ve iki satıra yayılınca satırın asıl bilgisini
  /// (tutar, kategori, tarih) aşağı iter. Tam metin ayrıntı panelindedir.
  final int titleMaxLines;
  final String? subtitle;
  final IconData? icon;
  final Color? iconColor;
  final Color? iconBackground;

  /// Sağ blok: genelde tutar.
  final Widget? trailing;

  /// Başlığın altındaki durum rozeti.
  final Widget? badge;

  final VoidCallback? onTap;

  /// İptal edilmiş gibi artık etkin olmayan kayıtlar soluklaşır. Solukluk tek
  /// başına anlam taşımaz; her zaman bir rozetle birlikte kullanılır.
  final bool dimmed;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final radius = BorderRadius.circular(AppRadius.field);

    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: radius,
        child: Opacity(
          opacity: dimmed ? 0.55 : 1,
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppSpacing.medium,
              vertical: AppSpacing.small + AppSpacing.xSmall,
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                if (icon != null) ...[
                  Container(
                    width: 40,
                    height: 40,
                    decoration: BoxDecoration(
                      color: iconBackground ?? surfaces.cardMuted,
                      borderRadius: BorderRadius.circular(AppRadius.field),
                    ),
                    child: Icon(
                      icon,
                      size: 20,
                      color: iconColor ?? theme.colorScheme.onSurfaceVariant,
                    ),
                  ),
                  const SizedBox(width: AppSpacing.medium),
                ],
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        title,
                        style: theme.textTheme.titleSmall,
                        maxLines: titleMaxLines,
                        overflow: TextOverflow.ellipsis,
                      ),
                      if (subtitle != null)
                        Padding(
                          padding: const EdgeInsets.only(
                            top: AppSpacing.xSmall,
                          ),
                          child: Text(
                            subtitle!,
                            style: theme.textTheme.bodySmall,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                      if (badge != null)
                        Padding(
                          padding: const EdgeInsets.only(
                            top: AppSpacing.xSmall,
                          ),
                          child: badge,
                        ),
                    ],
                  ),
                ),
                if (trailing != null) ...[
                  const SizedBox(width: AppSpacing.small),
                  trailing!,
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}
