import 'package:flutter/material.dart';
import '../theme/app_breakpoints.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'app_dashed_border.dart';
import 'app_money_text.dart';

class AppFilterOption {
  const AppFilterOption({
    required this.key,
    required this.label,
    this.amount,
    this.group,
    this.icon = Icons.account_balance_outlined,
    this.effect,
    this.subtitle,
    this.signedByAmount = false,
  });
  final String? key;
  final String label;
  final String? amount;
  final String? group;
  final IconData icon;
  final AppMoneyEffect? effect;
  final String? subtitle;
  final bool signedByAmount;
}

/// Kapsam ve yön seçiminde aynı ölçü ve görünür seçim işareti.
class AppFilterChip extends StatelessWidget {
  const AppFilterChip({
    required this.label,
    required this.selected,
    required this.onSelected,
    super.key,
  });
  final String label;
  final bool selected;
  final VoidCallback onSelected;
  @override
  Widget build(BuildContext context) => _FilterPill(
    label: label,
    selected: selected,
    checked: selected,
    onTap: onSelected,
  );
}

/// Filtre satırının tek hapı: seçim çipi ve menü düğmesi aynı ölçüyü taşır.
///
/// Teslimde hap 36 dp, satırı 8 dp üst ve 4 dp alt boşlukla 48 dp'dir. Dokunma
/// alanı o 48 dp'nin tamamıdır; hap görünür ölçüsünü korur.
class _FilterPill extends StatelessWidget {
  const _FilterPill({
    required this.label,
    required this.onTap,
    this.selected = false,
    this.checked = false,
    this.menuOpen,
    this.muted = false,
  });

  final String label;
  final VoidCallback? onTap;
  final bool selected;
  final bool checked;

  /// Boş değilse hap bir menü açar; değer menünün açık olup olmadığıdır.
  final bool? menuOpen;
  final bool muted;

  /// Yazının hap kenarına uzaklığı (teslimde 14 dp).
  static const double _edge =
      AppSpacing.small + AppSpacing.xSmall + AppSpacing.xxSmall;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final theme = Theme.of(context);
    final isMenu = menuOpen != null;
    final open = menuOpen ?? false;
    final filled = !muted && (selected || open);
    final ink = muted ? surfaces.inkFaint : surfaces.ink;
    final pill = ConstrainedBox(
      constraints: const BoxConstraints(minHeight: 36, maxWidth: 200),
      child: Padding(
        padding: EdgeInsets.only(
          left: isMenu || !checked ? _edge : AppSpacing.small,
          right: isMenu ? AppSpacing.xSmall + AppSpacing.xxSmall : _edge,
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            if (checked && !isMenu) ...[
              Icon(Icons.check, size: 18, color: ink),
              const SizedBox(width: AppSpacing.xSmall),
            ],
            Flexible(
              child: Text(
                label,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.bodyMedium?.copyWith(
                  fontWeight: muted ? FontWeight.w500 : FontWeight.w600,
                  height: 1,
                  letterSpacing: 0,
                  color: ink,
                ),
              ),
            ),
            if (isMenu) ...[
              const SizedBox(width: AppSpacing.xSmall),
              Icon(
                open ? Icons.arrow_drop_up : Icons.arrow_drop_down,
                size: 20,
                color: ink,
              ),
            ],
          ],
        ),
      ),
    );
    return Semantics(
      button: true,
      selected: isMenu ? null : selected,
      expanded: isMenu ? open : null,
      child: GestureDetector(
        behavior: HitTestBehavior.opaque,
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.only(
            top: AppSpacing.small,
            bottom: AppSpacing.xSmall,
          ),
          child: CustomPaint(
            foregroundPainter: muted
                ? AppDashedBorder(
                    surfaces.borderStrong,
                    BorderRadius.circular(999),
                  )
                : null,
            child: Material(
              color: muted
                  ? Colors.transparent
                  : filled
                  ? theme.colorScheme.primaryContainer
                  : surfaces.card,
              shape: StadiumBorder(
                side: muted
                    ? BorderSide.none
                    : BorderSide(
                        color: !filled
                            ? surfaces.borderStrong
                            : open
                            ? surfaces.ink
                            : Colors.transparent,
                      ),
              ),
              clipBehavior: Clip.antiAlias,
              child: InkWell(onTap: onTap, child: pill),
            ),
          ),
        ),
      ),
    );
  }
}

/// Çipin altında açılan menü. Başlıklar seçim değildir; tutarlar sunucudan gelir.
class AppFilterMenu extends StatefulWidget {
  const AppFilterMenu({
    required this.label,
    required this.options,
    required this.selected,
    required this.onSelected,
    this.caption,
    this.width = 320,
    this.maxHeight = 520,
    this.alignToPageStart = false,
    this.muted = false,
    this.onUnavailable,
    super.key,
  });
  final String label;
  final List<AppFilterOption> options;
  final String? selected;
  final String? caption;
  final double width;
  final double maxHeight;
  final bool alignToPageStart;
  final ValueChanged<String?> onSelected;
  final bool muted;
  final VoidCallback? onUnavailable;
  @override
  State<AppFilterMenu> createState() => _AppFilterMenuState();
}

class _AppFilterMenuState extends State<AppFilterMenu> {
  bool open = false;
  final _menu = MenuController();
  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final theme = Theme.of(context);
    final children = <Widget>[];
    if (widget.caption != null) {
      children.add(
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.small + AppSpacing.xSmall,
            AppSpacing.medium,
            AppSpacing.small,
          ),
          child: Text(
            widget.caption!,
            style: theme.textTheme.bodySmall?.copyWith(
              fontSize: 13,
              height: 1.35,
            ),
          ),
        ),
      );
      children.add(const Divider(height: 1));
    }
    children.add(const SizedBox(height: AppSpacing.xSmall));
    String? group;
    for (final option in widget.options) {
      if (option.group != null && group != option.group) {
        group = option.group;
        children.add(const SizedBox(height: AppSpacing.xSmall));
        children.add(const Divider(height: 1));
        children.add(
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.small + AppSpacing.xSmall,
              AppSpacing.medium,
              AppSpacing.xSmall,
            ),
            child: Text(
              group!,
              style: theme.textTheme.labelMedium?.copyWith(
                fontSize: 12,
                height: 1,
                letterSpacing: 0.48,
              ),
            ),
          ),
        );
      }
      final checked = widget.selected == option.key;
      children.add(
        Semantics(
          selected: checked,
          child: MenuItemButton(
            style: ButtonStyle(
              backgroundColor: WidgetStatePropertyAll(
                checked ? surfaces.cardMuted : null,
              ),
              minimumSize: WidgetStatePropertyAll(
                Size.fromHeight(option.subtitle == null ? 48 : 56),
              ),
              padding: const WidgetStatePropertyAll(
                EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                  vertical: AppSpacing.xSmall,
                ),
              ),
            ),
            onPressed: () => widget.onSelected(option.key),
            child: SizedBox(
              width: widget.width - AppSpacing.medium * 2,
              child: Row(
                children: [
                  Icon(
                    checked ? Icons.check : option.icon,
                    size: 20,
                    color: checked ? surfaces.ink : surfaces.inkMuted,
                  ),
                  const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Text(
                          option.label,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: theme.textTheme.bodyLarge?.copyWith(
                            fontSize: 15,
                            height: 1.3,
                            fontWeight: checked
                                ? FontWeight.w600
                                : FontWeight.w400,
                          ),
                        ),
                        if (option.subtitle != null)
                          Text(
                            option.subtitle!,
                            style: theme.textTheme.bodySmall?.copyWith(
                              fontSize: 13,
                              height: 1.35,
                            ),
                          ),
                        if (context.usesLargeText && option.amount != null)
                          _amount(option, checked),
                      ],
                    ),
                  ),
                  if (!context.usesLargeText && option.amount != null) ...[
                    const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                    _amount(option, checked),
                  ],
                ],
              ),
            ),
          ),
        ),
      );
    }
    children.add(const SizedBox(height: AppSpacing.xSmall));
    // Menü açıkken geri tuşu sayfayı değil menüyü kapatır.
    return PopScope(
      canPop: !open,
      onPopInvokedWithResult: (didPop, _) {
        if (!didPop) _menu.close();
      },
      child: _anchor(context, surfaces, children),
    );
  }

  Widget _anchor(
    BuildContext context,
    AppSurfaces surfaces,
    List<Widget> children,
  ) {
    return MenuAnchor(
      controller: _menu,
      reservedPadding: const EdgeInsets.all(AppSpacing.medium),
      onOpen: () => setState(() => open = true),
      onClose: () => setState(() => open = false),
      alignmentOffset: const Offset(0, 4),
      style: MenuStyle(
        padding: const WidgetStatePropertyAll(EdgeInsets.zero),
        backgroundColor: WidgetStatePropertyAll(surfaces.card),
        side: WidgetStatePropertyAll(BorderSide(color: surfaces.border)),
        alignment: AlignmentDirectional.bottomStart,
        maximumSize: WidgetStatePropertyAll(
          Size(widget.width, context.usesLargeText ? 640 : widget.maxHeight),
        ),
        shape: WidgetStatePropertyAll(
          RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        ),
      ),
      menuChildren: children,
      builder: (context, controller, _) => _FilterPill(
        label: widget.label,
        selected: widget.selected != null,
        menuOpen: open,
        muted: widget.muted,
        onTap: widget.muted
            ? widget.onUnavailable
            : () {
                if (controller.isOpen) {
                  controller.close();
                  return;
                }
                final anchor = context.findRenderObject();
                if (context.windowSize == AppWindowSize.compact &&
                    anchor is RenderBox) {
                  final left = widget.alignToPageStart
                      ? AppSpacing.medium
                      : MediaQuery.sizeOf(context).width -
                            widget.width -
                            AppSpacing.medium;
                  controller.open(
                    position: Offset(
                      left - anchor.localToGlobal(Offset.zero).dx,
                      anchor.size.height + AppSpacing.xSmall,
                    ),
                  );
                } else {
                  controller.open();
                }
              },
      ),
    );
  }

  Widget _amount(AppFilterOption option, bool checked) => AppMoneyText(
    amount: option.amount!,
    currency: 'TRY',
    size: AppMoneySize.body,
    effect: option.effect,
    signedByAmount: option.signedByAmount,
    style: TextStyle(
      fontSize: 15,
      height: 1.25,
      fontWeight: checked ? FontWeight.w700 : FontWeight.w600,
    ),
  );
}
