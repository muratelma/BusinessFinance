import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Kartın başlık şeridi: solda bağlam (tarih, bölüm), sağda durum.
///
/// Yükseklik 52, yatay boşluk 16, altında 1 dp kenar çizgisi. Başlık 15/600;
/// isteğe bağlı [meta] başlığın yanında gri 14 ile yazılır.
class AppCardHead extends StatelessWidget {
  const AppCardHead({required this.title, super.key, this.meta, this.status});

  final String title;
  final String? meta;
  final Widget? status;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Container(
      constraints: const BoxConstraints(minHeight: 52),
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
      decoration: BoxDecoration(
        border: Border(bottom: BorderSide(color: surfaces.border)),
      ),
      child: Row(
        children: [
          Expanded(
            child: Text.rich(
              TextSpan(
                children: [
                  TextSpan(
                    text: title,
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontSize: 15,
                      height: 1.3,
                    ),
                  ),
                  if (meta != null)
                    TextSpan(
                      text: '  $meta',
                      style: theme.textTheme.bodyMedium?.copyWith(
                        height: 1.3,
                        color: surfaces.inkMuted,
                      ),
                    ),
                ],
              ),
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          ),
          if (status != null) ...[
            const SizedBox(width: AppSpacing.small),
            status!,
          ],
        ],
      ),
    );
  }
}
