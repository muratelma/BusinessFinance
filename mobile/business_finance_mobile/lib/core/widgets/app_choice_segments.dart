import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Formdaki tek seçimli parçalı düğme (Claude Design, POS tahsilatı
/// `Komisyon`): tam genişlik, eşit parçalar, parçalar arası 1 dp çizgi.
/// 40 dp, köşesi form alanıyla aynı (16 yarıçap; kullanıcı, 28 Ekim 2026:
/// teslimin 48 dp hap biçimi yerine). Seçili parça marka dolgulu ve 600; işaret yoktur, bu
/// yüzden seçim değişince parça genişlemez ve yazı kaymaz.
///
/// Üstündeki küçük başlık ([label], 13 soluk, 8 dp boşluk) grubun adıdır;
/// parçalar kısa kalır.
class AppChoiceSegments<T> extends StatelessWidget {
  const AppChoiceSegments({
    required this.label,
    required this.values,
    required this.selected,
    required this.labelOf,
    required this.onChanged,
    super.key,
  });

  final String label;
  final List<T> values;
  final T selected;
  final String Function(T value) labelOf;
  final ValueChanged<T>? onChanged;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final scheme = theme.colorScheme;
    final side = BorderSide(color: surfaces.borderStrong);
    return Semantics(
      container: true,
      label: label,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          ExcludeSemantics(
            child: Text(
              label,
              style: theme.textTheme.bodySmall?.copyWith(
                fontSize: 13,
                color: surfaces.inkMuted,
              ),
            ),
          ),
          const SizedBox(height: AppSpacing.small),
          ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 40),
            child: DecoratedBox(
              decoration: ShapeDecoration(shape: _shape.copyWith(side: side)),
              child: ClipPath(
                clipper: const ShapeBorderClipper(shape: _shape),
                child: IntrinsicHeight(
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      for (var i = 0; i < values.length; i++) ...[
                        if (i > 0) VerticalDivider(width: 1, color: side.color),
                        Expanded(
                          child: _Segment(
                            label: labelOf(values[i]),
                            selected: values[i] == selected,
                            fill: scheme.primary,
                            onFill: scheme.onPrimary,
                            onTap: onChanged == null
                                ? null
                                : () => onChanged!(values[i]),
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

const _shape = RoundedRectangleBorder(
  borderRadius: BorderRadius.all(Radius.circular(AppRadius.field)),
);

class _Segment extends StatelessWidget {
  const _Segment({
    required this.label,
    required this.selected,
    required this.fill,
    required this.onFill,
    required this.onTap,
  });
  final String label;
  final bool selected;
  final Color fill;
  final Color onFill;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) => Semantics(
    button: true,
    inMutuallyExclusiveGroup: true,
    selected: selected,
    child: Material(
      color: selected ? fill : Colors.transparent,
      child: InkWell(
        onTap: onTap,
        child: Center(
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppSpacing.small,
              vertical: AppSpacing.small,
            ),
            child: Text(
              label,
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontSize: 14,
                fontWeight: selected ? FontWeight.w600 : FontWeight.w500,
                color: selected ? onFill : AppSurfaces.of(context).ink,
              ),
            ),
          ),
        ),
      ),
    ),
  );
}
