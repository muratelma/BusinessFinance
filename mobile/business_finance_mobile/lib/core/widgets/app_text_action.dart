import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Bölüm başlığının sağındaki metin eylemi (`+ Ekle`, `Tümü ›`).
///
/// Dolgusuz, 14/600 mürekkep rengi, ikon 18. Dokunma yüksekliği 48 dp;
/// görünen metin başlık satırının yüksekliğini büyütmesin diye dikeyde
/// taşar.
class AppTextAction extends StatelessWidget {
  const AppTextAction({
    required this.label,
    required this.onPressed,
    super.key,
    this.icon,
    this.trailingIcon,
  });

  final String label;
  final IconData? icon;
  final IconData? trailingIcon;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    final ink = AppSurfaces.of(context).ink;
    final style = Theme.of(context).textTheme.bodyMedium?.copyWith(
      fontWeight: FontWeight.w600,
      height: 1,
      color: ink,
    );
    return Semantics(
      button: true,
      child: InkWell(
        onTap: onPressed,
        borderRadius: BorderRadius.circular(AppRadius.field),
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.xSmall),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (icon != null) ...[
                  Icon(icon, size: 18, color: ink),
                  const SizedBox(width: AppSpacing.xSmall),
                ],
                // Büyük yazıda etiket satır kırar; eylem ekran dışına taşmaz.
                Flexible(child: Text(label, style: style)),
                if (trailingIcon != null) ...[
                  const SizedBox(width: AppSpacing.xSmall),
                  Icon(trailingIcon, size: 18, color: ink),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}
