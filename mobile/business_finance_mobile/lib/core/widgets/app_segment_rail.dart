import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Tek parça seçim rayı: gri zemin, 1 dp kenar, 4 dp iç boşluk; seçili dilim
/// beyaz yüzey ve güçlü kenar. Gölge yok.
///
/// Kapsam anahtarı (`Hepsi · İşletme · Şahsi`, çip yarıçapı) ve kasa seçici
/// (ad + bakiye, alan yarıçapı) aynı rayı kullanır. Dilimler genişliği eşit
/// paylaşır; içerik [segmentBuilder]'dan gelir ve büyük yazıda dilim
/// yüksekliği büyür, kırpılmaz.
class AppSegmentRail<T> extends StatelessWidget {
  const AppSegmentRail({
    required this.values,
    required this.selected,
    required this.onChanged,
    required this.segmentBuilder,
    required this.semanticLabel,
    super.key,
    this.segmentLabel,
    this.minSegmentHeight = 40,
    this.radius = AppRadius.chip,
  });

  final List<T> values;
  final T selected;
  final ValueChanged<T> onChanged;

  /// Dilimin içeriği; `selected` seçili dilimin mürekkep rengini seçmek için.
  final Widget Function(BuildContext context, T value, bool selected)
  segmentBuilder;

  /// Ekran okuyucunun dilim için okuyacağı ad; verilmezse içerik okunur.
  final String Function(T value)? segmentLabel;
  final String semanticLabel;
  final double minSegmentHeight;
  final double radius;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final outer = BorderRadius.circular(radius);
    final inner = BorderRadius.circular(
      radius >= AppRadius.chip ? radius : radius - AppSpacing.xSmall,
    );
    return Semantics(
      container: true,
      label: semanticLabel,
      child: Container(
        // Dikey iç boşluk dilimin içindedir: dokunma alanı rayın kenarına
        // kadar uzanır ve 40 dp dilim 48 dp hedef olur.
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.xSmall),
        decoration: BoxDecoration(
          color: surfaces.cardMuted,
          border: Border.all(color: surfaces.border),
          borderRadius: outer,
        ),
        child: Row(
          children: [
            for (var i = 0; i < values.length; i++) ...[
              if (i > 0) const SizedBox(width: AppSpacing.xSmall),
              Expanded(
                child: _RailSegment(
                  selected: values[i] == selected,
                  radius: inner,
                  minHeight: minSegmentHeight,
                  label: segmentLabel?.call(values[i]),
                  onTap: () => onChanged(values[i]),
                  child: segmentBuilder(
                    context,
                    values[i],
                    values[i] == selected,
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _RailSegment extends StatelessWidget {
  const _RailSegment({
    required this.selected,
    required this.radius,
    required this.minHeight,
    required this.onTap,
    required this.child,
    this.label,
  });

  final bool selected;
  final BorderRadius radius;
  final double minHeight;
  final VoidCallback onTap;
  final Widget child;
  final String? label;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      selected: selected,
      label: label,
      onTap: onTap,
      excludeSemantics: label != null,
      child: GestureDetector(
        behavior: HitTestBehavior.opaque,
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
          child: Material(
            color: selected ? surfaces.card : Colors.transparent,
            shape: RoundedRectangleBorder(
              borderRadius: radius,
              side: BorderSide(
                color: selected ? surfaces.borderStrong : Colors.transparent,
              ),
            ),
            clipBehavior: Clip.antiAlias,
            child: InkWell(
              onTap: onTap,
              child: ConstrainedBox(
                constraints: BoxConstraints(minHeight: minHeight),
                child: Center(heightFactor: 1, child: child),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
