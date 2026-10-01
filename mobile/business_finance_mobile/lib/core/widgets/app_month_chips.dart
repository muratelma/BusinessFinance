import 'package:flutter/material.dart';

import '../theme/app_breakpoints.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// "Seçilen aylarda" ritminin ay seçicisi: 12 çip, 6×2 ızgara; büyük yazıda
/// 4×3.
///
/// Seçili çip marka dolgusu, seçili olmayan beyaz yüzey ve güçlü kenar. Seçim
/// yalnız renkle değil, ekran okuyucuya `seçili` durumuyla da bildirilir. Her
/// çip en az 48 dp yüksekliktedir.
class AppMonthChips extends StatelessWidget {
  const AppMonthChips({
    required this.selected,
    required this.onChanged,
    super.key,
    this.multiple = true,
  });

  static const shortNames = [
    'Oca',
    'Şub',
    'Mar',
    'Nis',
    'May',
    'Haz',
    'Tem',
    'Ağu',
    'Eyl',
    'Eki',
    'Kas',
    'Ara',
  ];

  static const _fullNames = [
    'Ocak',
    'Şubat',
    'Mart',
    'Nisan',
    'Mayıs',
    'Haziran',
    'Temmuz',
    'Ağustos',
    'Eylül',
    'Ekim',
    'Kasım',
    'Aralık',
  ];

  /// Seçili aylar (1–12).
  final Set<int> selected;
  final ValueChanged<Set<int>> onChanged;

  /// `false` ise tek ay seçilir (yılda bir).
  final bool multiple;

  @override
  Widget build(BuildContext context) {
    final columns = context.usesLargeText ? 4 : 6;
    return Semantics(
      container: true,
      label: 'Aylar',
      child: LayoutBuilder(
        builder: (context, constraints) {
          const gap = AppSpacing.xSmall;
          final width = (constraints.maxWidth - gap * (columns - 1)) / columns;
          return Wrap(
            spacing: gap,
            runSpacing: gap,
            children: [
              for (var month = 1; month <= 12; month++)
                SizedBox(
                  width: width,
                  child: _MonthChip(
                    label: shortNames[month - 1],
                    semanticLabel: _fullNames[month - 1],
                    selected: selected.contains(month),
                    onTap: () => onChanged(_toggle(month)),
                  ),
                ),
            ],
          );
        },
      ),
    );
  }

  Set<int> _toggle(int month) {
    if (!multiple) return {month};
    final next = {...selected};
    if (!next.remove(month)) next.add(month);
    return next;
  }
}

class _MonthChip extends StatelessWidget {
  const _MonthChip({
    required this.label,
    required this.semanticLabel,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final String semanticLabel;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final surfaces = AppSurfaces.of(context);
    final shape = RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(AppRadius.chip),
      side: BorderSide(
        color: selected ? scheme.primary : surfaces.borderStrong,
      ),
    );
    return Semantics(
      button: true,
      selected: selected,
      label: semanticLabel,
      excludeSemantics: true,
      child: Material(
        color: selected ? scheme.primary : surfaces.card,
        shape: shape,
        clipBehavior: Clip.antiAlias,
        child: InkWell(
          onTap: onTap,
          child: ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 48),
            child: Center(
              child: Text(
                label,
                maxLines: 1,
                style: Theme.of(context).textTheme.labelLarge?.copyWith(
                  fontWeight: FontWeight.w600,
                  color: selected ? scheme.onPrimary : surfaces.ink,
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
