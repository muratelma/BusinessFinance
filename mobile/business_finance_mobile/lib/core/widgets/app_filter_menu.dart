import 'package:flutter/material.dart';
import '../theme/app_breakpoints.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'app_animated_menu.dart';
import 'app_dashed_border.dart';
import 'app_money_text.dart';
import '../theme/app_typography.dart';
import '../theme/app_radius.dart';

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
  static const double _edge = AppSpacing.step14;

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
          right: isMenu ? AppSpacing.step6 : _edge,
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
                    BorderRadius.circular(AppRadius.chip),
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
    this.maxHeight = maxMenuHeight,
    this.alignToPageStart = false,
    this.muted = false,
    this.onUnavailable,
    super.key,
  });

  /// Filtre menüsünün en çok boyu, bütün sayfalarda (kullanıcı kararı,
  /// 15 Kasım 2026): hesap seçme menüsüyle aynı sınır. Satırlar burada 48 dp
  /// olduğu için başlık, `Hepsi` ve yaklaşık yedi satır görünür; fazlası
  /// menünün içinde kayar. Eskiden seçenek sayısına göre 520–690 dp'ye kadar
  /// uzuyor, sayfanın sonuna iniyordu.
  static const double maxMenuHeight = 442;

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
  @override
  Widget build(BuildContext context) {
    final compact = context.windowSize == AppWindowSize.compact;
    // Menü diğer seçim menüleriyle aynı bileşendir ([AppAnimatedMenu]):
    // aynı açılış ve kapanış, dışarı dokununca da toplanarak kapanır, uzun
    // liste içinde kayar ve kaydırma çubuğu görünür.
    return AppAnimatedMenu(
      maxMenuHeight: context.usesLargeText ? 640 : widget.maxHeight,
      expectedHeight: (widget.options.length * 48.0 + 64).clamp(
        96.0,
        widget.maxHeight,
      ),
      menuWidth: widget.width,
      // Telefonda menü çipe değil sayfanın kenarına hizalanır.
      menuScreenLeft: !compact
          ? null
          : widget.alignToPageStart
          ? AppSpacing.medium
          : MediaQuery.sizeOf(context).width - widget.width - AppSpacing.medium,
      menuBuilder: (context, close) => Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: appFilterMenuItems(
          context,
          options: widget.options,
          selected: widget.selected,
          width: null,
          caption: widget.caption,
          closeOnActivate: false,
          onSelected: (key) {
            close();
            widget.onSelected(key);
          },
        ),
      ),
      builder: (context, open, toggle) => _FilterPill(
        label: widget.label,
        selected: widget.selected != null,
        menuOpen: open,
        muted: widget.muted,
        onTap: widget.muted ? widget.onUnavailable : toggle,
      ),
    );
  }
}

/// Süzgeç menüsünün satırları: isteğe bağlı başlık, gruplar, seçili satırda
/// onay işareti ve gri zemin, sağda tutar. Çipin menüsü ([AppFilterMenu]) ve
/// alttan panelin içindeki seçim alanı aynı satırları çizer. [width] boşsa
/// satırlar bulundukları kutuya yayılır (akış içindeki menü).
List<Widget> appFilterMenuItems(
  BuildContext context, {
  required List<AppFilterOption> options,
  required String? selected,
  required double? width,
  required ValueChanged<String?> onSelected,
  String? caption,
  bool closeOnActivate = true,
}) {
  final surfaces = AppSurfaces.of(context);
  final theme = Theme.of(context);
  final children = <Widget>[];
  if (caption != null) {
    children.add(
      Padding(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.compact,
          AppSpacing.medium,
          AppSpacing.small,
        ),
        child: Text(
          caption,
          // Grup başlıklarıyla aynı yazı: menüde iki ayrı başlık biçimi
          // olmasın (kullanıcı, 15 Kasım 2026).
          style: theme.textTheme.labelMedium?.copyWith(
            fontSize: AppTextSizes.micro,
            height: 1,
            letterSpacing: 0.48,
          ),
        ),
      ),
    );
    children.add(const Divider(height: 1));
  }
  children.add(const SizedBox(height: AppSpacing.xSmall));
  String? group;
  for (final option in options) {
    if (option.group != null && group != option.group) {
      group = option.group;
      children.add(const SizedBox(height: AppSpacing.xSmall));
      children.add(const Divider(height: 1));
      children.add(
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.compact,
            AppSpacing.medium,
            AppSpacing.xSmall,
          ),
          child: Text(
            group!,
            style: theme.textTheme.labelMedium?.copyWith(
              fontSize: AppTextSizes.micro,
              height: 1,
              letterSpacing: 0.48,
            ),
          ),
        ),
      );
    }
    final checked = selected == option.key;
    children.add(
      Semantics(
        selected: checked,
        child: MenuItemButton(
          closeOnActivate: closeOnActivate,
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
          onPressed: () => onSelected(option.key),
          overflowAxis: width == null ? Axis.vertical : Axis.horizontal,
          child: SizedBox(
            width: width == null ? null : width - AppSpacing.medium * 2,
            child: Row(
              children: [
                Icon(
                  checked ? Icons.check : option.icon,
                  size: 20,
                  color: checked ? surfaces.ink : surfaces.inkMuted,
                ),
                const SizedBox(width: AppSpacing.compact),
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
                          fontSize: AppTextSizes.compact,
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
                            fontSize: AppTextSizes.caption,
                            height: 1.35,
                          ),
                        ),
                      if (context.usesLargeText && option.amount != null)
                        _filterMenuAmount(option, checked),
                    ],
                  ),
                ),
                if (!context.usesLargeText && option.amount != null) ...[
                  const SizedBox(width: AppSpacing.compact),
                  _filterMenuAmount(option, checked),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
  children.add(const SizedBox(height: AppSpacing.xSmall));
  return children;
}

Widget _filterMenuAmount(AppFilterOption option, bool checked) => AppMoneyText(
  amount: option.amount!,
  currency: 'TRY',
  size: AppMoneySize.body,
  effect: option.effect,
  signedByAmount: option.signedByAmount,
  style: TextStyle(
    fontSize: AppTextSizes.compact,
    height: 1.25,
    fontWeight: checked ? FontWeight.w700 : FontWeight.w600,
  ),
);
