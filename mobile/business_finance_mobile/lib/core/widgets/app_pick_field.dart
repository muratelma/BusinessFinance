import 'package:flutter/material.dart';

import '../theme/app_breakpoints.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';
import 'app_animated_menu.dart';
import 'app_icon_capsule.dart';

/// [AppPickField]'ın bir seçeneği.
class AppPickOption<T> {
  const AppPickOption({
    required this.value,
    required this.label,
    this.icon,
    this.subtitle,
    this.iconColor,
    this.blank = false,
  });

  final T value;
  final String label;

  /// Kullanıcı seçmeden bu değer geldiyse alan **boş** görünür (yalnız
  /// başlık); menüden seçilince adını yazar (kullanıcı, 13 Kasım 2026: yeni
  /// kartta varsayılan kapsam boş gelir, `Yok` seçilince `Yok` yazar).
  final bool blank;

  /// Boşsa satır yalnız yazıdır (gün, tarih biçimi gibi simgesi anlam
  /// taşımayan seçenekler).
  final IconData? icon;

  /// Seçeneğin kuralını söyleyen kısa alt yazı (`Kategoride yoksa geçerli`).
  final String? subtitle;

  /// Boşsa simge mürekkep rengindedir; hesap türünde türün rengi.
  final Color? iconColor;
}

/// Uygulamanın ortak seçim alanı (Claude Design, kart taksit ve limit teslimi
/// §6): hesap türü, varsayılan kapsam ve benzeri az seçenekli sorular.
///
/// Kapalı alan simge kapsülü, ad ve isteğe bağlı alt yazıyla çizilir; alan
/// boşken de seçiliyken de 56 dp'dir, yalnız büyük yazıda uzar. Menü alanın
/// 8 dp altında, alan genişliğinde, alanın üstünde yüzer; seçili satırda
/// solda 4 dp çubuk ve gri zemin vardır, onay işareti yoktur. Hesap ve
/// kategori seçicisiyle aynı satırı kullanır ([AppPickMenuRow]).
///
/// [showSubtitle] kapalı alanda alt yazıyı gösterir (varsayılan kapsam);
/// basit alanda (hesap türü) alt yazı ne alanda ne menüde çizilir.
class AppPickField<T> extends StatefulWidget {
  const AppPickField({
    required this.label,
    required this.options,
    required this.value,
    required this.onChanged,
    super.key,
    this.showSubtitle = false,
    this.helperText,
  });

  final String label;
  final List<AppPickOption<T>> options;
  final T value;

  /// Boşsa alan kapalıdır (düzenlemede değiştirilemeyen tür).
  final ValueChanged<T>? onChanged;
  final bool showSubtitle;
  final String? helperText;

  @override
  State<AppPickField<T>> createState() => _AppPickFieldState<T>();
}

class _AppPickFieldState<T> extends State<AppPickField<T>> {
  /// Kullanıcı menüden boş görünen seçeneği (`Yok`) seçti mi. Seçtiyse alan
  /// artık boş değildir, seçeneğin adını yazar.
  bool _blankChosen = false;

  AppPickOption<T>? get _selected {
    for (final option in widget.options) {
      if (option.value == widget.value) {
        return option.blank && !_blankChosen ? null : option;
      }
    }
    return null;
  }

  void _choose(AppPickOption<T> option) {
    setState(() => _blankChosen = option.blank);
    widget.onChanged?.call(option.value);
  }

  @override
  Widget build(BuildContext context) {
    final selected = _selected;
    // Menü alanın üstünde yüzer ve hareketle açılır ([AppAnimatedMenu]);
    // boyu ortak kuraldır, uzun liste içinde kayar.
    return AppAnimatedMenu(
      expectedHeight: widget.options.length * 56.0 + AppSpacing.medium,
      anchorHeight: widget.helperText == null || context.usesLargeText
          ? null
          : 56,
      menuBuilder: (context, close) => Padding(
        padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            for (final option in widget.options)
              AppPickMenuRow(
                selected: identical(option, selected),
                semanticsLabel: [
                  option.label,
                  if (widget.showSubtitle) ?option.subtitle,
                ].join(', '),
                onPressed: () {
                  close();
                  _choose(option);
                },
                child: _MenuContent(
                  option: option,
                  selected: identical(option, selected),
                  subtitle: widget.showSubtitle,
                ),
              ),
          ],
        ),
      ),
      builder: (context, open, toggle) =>
          _closed(context, selected, open, toggle),
    );
  }

  Widget _closed(
    BuildContext context,
    AppPickOption<T>? selected,
    bool open,
    VoidCallback toggle,
  ) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      expanded: open,
      label: widget.label,
      value: selected == null
          ? null
          : [
              selected.label,
              if (widget.showSubtitle) ?selected.subtitle,
            ].join(', '),
      child: ExcludeSemantics(
        child: InkWell(
          borderRadius: BorderRadius.circular(AppRadius.field),
          onTap: widget.onChanged == null ? null : toggle,
          child: InputDecorator(
            isFocused: open,
            decoration: InputDecoration(
              // Değer yokken başlık alanın içinde yer tutucu olarak yazar.
              labelText: selected == null ? null : widget.label,
              helperText: widget.helperText,
              helperMaxLines: 3,
              enabled: widget.onChanged != null,
              contentPadding: EdgeInsets.fromLTRB(
                selected?.icon == null ? AppSpacing.medium : AppSpacing.compact,
                AppSpacing.small,
                AppSpacing.step6,
                AppSpacing.small,
              ),
              suffixIcon: AppMenuArrow(open: open),
            ),
            // Alan boşken de seçiliyken de 56 dp: 8 + 40 + 8. Yalnız büyük
            // yazıda uzar (kullanıcı, 13 Kasım 2026: boş alan kısa kalıyordu).
            child: ConstrainedBox(
              constraints: const BoxConstraints(minHeight: 40),
              child: SizedBox(
                height: context.usesLargeText ? null : 40,
                child: Align(
                  alignment: Alignment.centerLeft,
                  child: selected == null
                      ? Text(
                          widget.label,
                          maxLines: context.usesLargeText ? 2 : 1,
                          overflow: TextOverflow.ellipsis,
                          style:
                              theme.inputDecorationTheme.hintStyle ??
                              theme.textTheme.bodyLarge?.copyWith(
                                color: surfaces.inkMuted,
                              ),
                        )
                      : _ClosedValue(
                          option: selected,
                          subtitle: widget.showSubtitle,
                        ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Seçim menülerinin ortak görünüşü: beyaz, 16 köşe, kenarlıksız, yüzen
/// gölge; alan genişliğinde.
MenuStyle appPickMenuStyle(BuildContext context, double width) {
  final surfaces = AppSurfaces.of(context);
  return MenuStyle(
    padding: const WidgetStatePropertyAll(
      EdgeInsets.symmetric(vertical: AppSpacing.small),
    ),
    backgroundColor: WidgetStatePropertyAll(surfaces.card),
    surfaceTintColor: const WidgetStatePropertyAll(Colors.transparent),
    elevation: const WidgetStatePropertyAll(AppElevation.floating),
    side: const WidgetStatePropertyAll(BorderSide.none),
    minimumSize: WidgetStatePropertyAll(Size(width, 0)),
    maximumSize: WidgetStatePropertyAll(Size(width, 600)),
    shape: WidgetStatePropertyAll(
      RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
    ),
  );
}

/// Seçim menüsünün satırı: en az 56 dp; seçili satırda solda 4 dp marka
/// çubuğu ve soluk zemin, işaret yok. Hesap, kategori ve ortak seçim alanı
/// aynı satırı kullanır.
class AppPickMenuRow extends StatelessWidget {
  const AppPickMenuRow({
    required this.selected,
    required this.onPressed,
    required this.child,
    super.key,
    this.width,
    this.semanticsLabel,
    this.minHeight = 56,
  });

  /// Yüzen menüde satırın genişliği (alanın genişliği). Boşsa satır
  /// bulunduğu kutunun genişliğine yayılır.
  final double? width;
  final bool selected;
  final VoidCallback? onPressed;
  final Widget child;
  final String? semanticsLabel;
  final double minHeight;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      selected: selected,
      label: semanticsLabel,
      child: MenuItemButton(
        onPressed: onPressed,
        // Dikey taşmada etiket satırın kalan genişliğini doldurur.
        overflowAxis: width == null ? Axis.vertical : Axis.horizontal,
        style: ButtonStyle(
          backgroundColor: WidgetStatePropertyAll(
            selected ? surfaces.cardMuted : null,
          ),
          minimumSize: WidgetStatePropertyAll(Size.fromHeight(minHeight)),
          padding: const WidgetStatePropertyAll(EdgeInsets.zero),
        ),
        child: SizedBox(
          width: width,
          child: Stack(
            children: [
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                  vertical: AppSpacing.step6,
                ),
                child: semanticsLabel == null
                    ? child
                    : ExcludeSemantics(child: child),
              ),
              if (selected)
                Positioned(
                  left: 0,
                  top: AppSpacing.small,
                  bottom: AppSpacing.small,
                  width: AppSpacing.xSmall,
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      color: Theme.of(context).colorScheme.primary,
                      borderRadius: const BorderRadius.horizontal(
                        right: Radius.circular(4),
                      ),
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

class _ClosedValue<T> extends StatelessWidget {
  const _ClosedValue({required this.option, required this.subtitle});
  final AppPickOption<T> option;
  final bool subtitle;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final large = context.usesLargeText;
    return Row(
      children: [
        if (option.icon != null) ...[
          AppIconCapsule(
            icon: option.icon!,
            size: 32,
            iconColor: option.iconColor ?? surfaces.ink,
          ),
          const SizedBox(width: AppSpacing.compact),
        ],
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              // Ad kısalmaz; büyük yazıda alt satıra iner.
              Text(
                option.label,
                maxLines: large ? 2 : 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: AppTextSizes.compact,
                  height: 20 / 15,
                  fontWeight: FontWeight.w600,
                ),
              ),
              if (subtitle && option.subtitle != null)
                Text(
                  option.subtitle!,
                  maxLines: large ? 2 : 1,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontSize: AppTextSizes.caption,
                    height: 16 / 13,
                    color: surfaces.inkMuted,
                  ),
                ),
            ],
          ),
        ),
      ],
    );
  }
}

class _MenuContent<T> extends StatelessWidget {
  const _MenuContent({
    required this.option,
    required this.selected,
    required this.subtitle,
  });
  final AppPickOption<T> option;
  final bool selected;
  final bool subtitle;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Row(
      children: [
        if (option.icon != null) ...[
          Icon(option.icon, size: 20, color: option.iconColor ?? surfaces.ink),
          const SizedBox(width: AppSpacing.step14),
        ],
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                option.label,
                style: theme.textTheme.bodyLarge?.copyWith(
                  fontSize: AppTextSizes.compact,
                  height: 1.25,
                  fontWeight: selected ? FontWeight.w700 : FontWeight.w400,
                  color: surfaces.ink,
                ),
              ),
              if (subtitle && option.subtitle != null)
                Text(
                  option.subtitle!,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontSize: AppTextSizes.caption,
                    height: 1.25,
                    color: surfaces.inkMuted,
                  ),
                ),
            ],
          ),
        ),
      ],
    );
  }
}
