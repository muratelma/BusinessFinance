import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';
import 'app_animated_menu.dart';
import 'app_filter_menu.dart';
import 'app_icon_capsule.dart';
import 'app_segment_rail.dart';
import 'app_text_action.dart';

/// Alttan açılan filtre paneli (Claude Design, kart hareketleri teslimi §3):
/// solda ✕, başlık, sağda `Sıfırla`; bölümler; altta tam genişlik
/// `Sonuçları göster`. Seçim taslaktır, düğmeyle geçerli olur.
///
/// Borç-alacak ve İşlemler aynı düzeni kullanacak; değişen yalnız
/// bölümlerdir.
///
/// `Sıfırla` her zaman görünür, altı çizilidir ve yazısı bölümlerin
/// iç hizasında biter (kullanıcı, 12–13 Kasım 2026: kenara fazla yakın
/// duruyordu).
class AppFilterSheet extends StatelessWidget {
  const AppFilterSheet({
    required this.title,
    required this.sections,
    required this.onSubmit,
    super.key,
    this.onReset,
    this.submitLabel = 'Sonuçları göster',
  });

  final String title;
  final List<Widget> sections;
  final VoidCallback onSubmit;

  /// Boşsa `Sıfırla` çizilmez (seçim zaten varsayılan).
  final VoidCallback? onReset;
  final String submitLabel;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return SafeArea(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.only(
              left: AppSpacing.xSmall,
              // `Sıfırla`nın yazısı bölümlerin iç hizasında (alanların 12 dp
              // iç boşluğu: `Bu ekstre`nin tarihi, satırların oku) biter;
              // düğmenin yuvarlak ucuyla aynı çizgide dururken göze kenara
              // yapışık görünüyordu (kullanıcı, 13 Kasım 2026). Kendi 4 dp
              // iç boşluğu düşülür.
              right:
                  AppSheetInsets.edge + AppSpacing.compact - AppSpacing.xSmall,
            ),
            child: Row(
              children: [
                IconButton(
                  tooltip: 'Kapat',
                  onPressed: () => Navigator.of(context).pop(),
                  icon: const Icon(Icons.close),
                ),
                const SizedBox(width: AppSpacing.xSmall),
                Expanded(
                  child: Semantics(
                    header: true,
                    child: Text(title, style: theme.textTheme.titleLarge),
                  ),
                ),
                // Her zaman yerinde durur (teslim); sıfırlanacak bir şey
                // yokken soluktur. Seçim değişince belirip başlığı
                // oynatmaz. Altı yazıdan 2 dp ayrık çizilir.
                Opacity(
                  opacity: onReset == null ? 0.38 : 1,
                  child: AppTextAction(
                    label: 'Sıfırla',
                    underlined: true,
                    onPressed: onReset,
                  ),
                ),
              ],
            ),
          ),
          Flexible(
            child: SingleChildScrollView(
              padding: const EdgeInsets.fromLTRB(
                AppSheetInsets.edge,
                AppSpacing.compact,
                AppSheetInsets.edge,
                0,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                mainAxisSize: MainAxisSize.min,
                children: [
                  for (final (index, section) in sections.indexed) ...[
                    if (index > 0) const SizedBox(height: AppSpacing.step20),
                    section,
                  ],
                ],
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(
              AppSheetInsets.edge,
            ).copyWith(top: AppSpacing.large),
            child: FilledButton(
              onPressed: onSubmit,
              style: FilledButton.styleFrom(
                minimumSize: const Size.fromHeight(52),
              ),
              child: Text(submitLabel),
            ),
          ),
        ],
      ),
    );
  }
}

/// Filtre panelinin bir bölümü: 15/600 başlık ve altında 10 dp sonra içerik.
class AppFilterSection extends StatelessWidget {
  const AppFilterSection({required this.title, required this.child, super.key});
  final String title;
  final Widget child;

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    mainAxisSize: MainAxisSize.min,
    children: [
      Semantics(
        header: true,
        child: Text(
          title,
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
            fontSize: AppTextSizes.compact,
            height: 1.3,
          ),
        ),
      ),
      const SizedBox(height: AppSpacing.step10),
      child,
    ],
  );
}

/// Filtre panelindeki bölmeli seçim: gri ray, seçili dilim beyaz ve kenarlı.
class AppFilterSegments<T> extends StatelessWidget {
  const AppFilterSegments({
    required this.values,
    required this.selected,
    required this.label,
    required this.onChanged,
    required this.semanticLabel,
    super.key,
  });
  final List<T> values;
  final T selected;
  final String Function(T value) label;
  final ValueChanged<T> onChanged;
  final String semanticLabel;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return AppSegmentRail<T>(
      values: values,
      selected: selected,
      onChanged: onChanged,
      semanticLabel: semanticLabel,
      segmentLabel: label,
      minSegmentHeight: 44,
      radius: AppRadius.field,
      segmentBuilder: (context, value, on) => Padding(
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.xSmall),
        child: FittedBox(
          fit: BoxFit.scaleDown,
          child: Text(
            label(value),
            maxLines: 1,
            style: theme.textTheme.bodyMedium?.copyWith(
              fontWeight: on ? FontWeight.w600 : FontWeight.w500,
              height: 1.2,
              color: on ? surfaces.ink : surfaces.inkMuted,
            ),
          ),
        ),
      ),
    );
  }
}

/// Filtre panelindeki seçim alanı: menüsü alanın üstünde yüzer ve hareketle
/// açılıp kapanır ([AppAnimatedMenu]); satırlar kategori çipinin menüsüyle
/// aynıdır ([appFilterMenuItems]). Altında yer varsa aşağı açılır.
class AppFilterSelectField extends StatelessWidget {
  const AppFilterSelectField({
    required this.label,
    required this.icon,
    required this.options,
    required this.selected,
    required this.onSelected,
    super.key,
    this.semanticLabel,
  });

  final String label;
  final IconData icon;
  final List<AppFilterOption> options;
  final String? selected;
  final ValueChanged<String?> onSelected;
  final String? semanticLabel;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return AppAnimatedMenu(
      expectedHeight: options.length * 56.0,
      menuBuilder: (context, close) => Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: appFilterMenuItems(
          context,
          options: options,
          selected: selected,
          width: null,
          onSelected: (key) {
            close();
            onSelected(key);
          },
        ),
      ),
      builder: (context, open, toggle) => Semantics(
        button: true,
        expanded: open,
        label: semanticLabel,
        value: label,
        child: ExcludeSemantics(
          child: InkWell(
            onTap: toggle,
            borderRadius: BorderRadius.circular(AppRadius.field),
            child: InputDecorator(
              isFocused: open,
              decoration: InputDecoration(
                contentPadding: const EdgeInsets.fromLTRB(
                  AppSpacing.compact,
                  AppSpacing.small,
                  AppSpacing.step6,
                  AppSpacing.small,
                ),
                suffixIcon: AnimatedRotation(
                  // Ok menüyle birlikte döner.
                  turns: open ? 0.5 : 0,
                  duration: const Duration(milliseconds: 220),
                  curve: Curves.easeOutCubic,
                  child: const Icon(Icons.arrow_drop_down, size: 24),
                ),
              ),
              child: ConstrainedBox(
                constraints: const BoxConstraints(minHeight: 40),
                child: Row(
                  children: [
                    AppIconCapsule(
                      icon: icon,
                      size: 32,
                      iconColor: surfaces.ink,
                    ),
                    const SizedBox(width: AppSpacing.compact),
                    Expanded(
                      child: Text(
                        label,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: Theme.of(context).textTheme.titleSmall?.copyWith(
                          fontSize: AppTextSizes.compact,
                          height: 1.25,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Filtre panelinde tek seçimli satır: solda daire, ad ve sağda soluk not.
class AppFilterRadioRow extends StatelessWidget {
  const AppFilterRadioRow({
    super.key,
    required this.label,
    required this.selected,
    required this.onTap,
    this.trailing,
  });
  final String label;
  final bool selected;
  final VoidCallback onTap;
  final String? trailing;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      inMutuallyExclusiveGroup: true,
      checked: selected,
      button: true,
      label: [label, ?trailing].join(', '),
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppRadius.small),
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Row(
            children: [
              Container(
                width: 20,
                height: 20,
                alignment: Alignment.center,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  border: Border.all(
                    width: 2,
                    color: selected
                        ? theme.colorScheme.primary
                        : surfaces.borderStrong,
                  ),
                ),
                child: selected
                    ? Container(
                        width: 10,
                        height: 10,
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          color: theme.colorScheme.primary,
                        ),
                      )
                    : null,
              ),
              const SizedBox(width: AppSpacing.step14),
              Expanded(
                child: Text(
                  label,
                  style: theme.textTheme.bodyLarge?.copyWith(
                    fontSize: AppTextSizes.compact,
                    height: 1.3,
                    fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
                  ),
                ),
              ),
              if (trailing != null)
                Text(
                  trailing!,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontSize: AppTextSizes.caption,
                    color: surfaces.inkMuted,
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Filtre panelinde takvimi açan satır (`Tarih aralığı seç…`).
class AppFilterRangeRow extends StatelessWidget {
  const AppFilterRangeRow({
    super.key,
    required this.label,
    required this.selected,
    required this.onTap,
  });
  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      selected: selected,
      label: selected ? 'Tarih aralığı: $label' : label,
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppRadius.small),
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Row(
            children: [
              Icon(Icons.date_range_outlined, size: 20, color: surfaces.ink),
              const SizedBox(width: AppSpacing.step14),
              Expanded(
                child: Text(
                  label,
                  style: theme.textTheme.bodyLarge?.copyWith(
                    fontSize: AppTextSizes.compact,
                    height: 1.3,
                    fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
                  ),
                ),
              ),
              Icon(Icons.chevron_right, size: 20, color: surfaces.inkMuted),
            ],
          ),
        ),
      ),
    );
  }
}
