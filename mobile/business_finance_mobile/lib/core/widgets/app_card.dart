import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Uygulamanın tek kart yüzeyi.
///
/// Kart zeminden **kenarlıkla** ayrılır, gölgeyle değil: gölge yalnız gerçekten
/// yüzen katmanlara (FAB, sheet, dialog) ayrılmıştır. Her kart gölge taşısaydı
/// derinlik bilgisi anlamını kaybederdi.
class AppCard extends StatelessWidget {
  const AppCard({
    required this.child,
    super.key,
    this.padding = const EdgeInsets.all(AppSpacing.medium),
    this.onTap,
    this.selected = false,
    this.background,
  });

  final Widget child;
  final EdgeInsetsGeometry padding;
  final VoidCallback? onTap;

  /// Seçili kart kenarlığını vurgular. Seçim yalnız renkle değil, kenarlık
  /// kalınlığıyla da bildirilir.
  final bool selected;

  final Color? background;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final scheme = Theme.of(context).colorScheme;
    final radius = BorderRadius.circular(AppRadius.card);

    final content = Padding(padding: padding, child: child);

    return Material(
      color: background ?? surfaces.card,
      borderRadius: radius,
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        borderRadius: radius,
        // Dokunulabilir kart bastırıldığını belli eder; dokunulamayan kart
        // yanlışlıkla tıklanabilir görünmez.
        child: Container(
          decoration: BoxDecoration(
            borderRadius: radius,
            border: Border.all(
              color: selected ? scheme.primary : surfaces.border,
              width: selected ? 2 : 1,
            ),
          ),
          child: content,
        ),
      ),
    );
  }
}
