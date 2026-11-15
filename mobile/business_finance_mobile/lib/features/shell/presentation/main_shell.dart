import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_content_width.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../activities/presentation/quick_add_navigation.dart';

class MainShell extends StatelessWidget {
  const MainShell({
    required this.navigationShell,
    super.key,
    this.scopeController,
    this.hideBar = false,
  });

  /// Telefonda alt çubuk çizilmez: kayıt formu tam ekran açılır.
  final bool hideBar;

  static const _leadingDestinations = [
    ShellDestination(
      label: 'Özet',
      icon: Icons.home_outlined,
      selectedIcon: Icons.home,
    ),
    ShellDestination(
      label: 'İşlemler',
      icon: Icons.swap_vert,
      selectedIcon: Icons.swap_vert,
    ),
  ];

  static const _budgetsDestination = ShellDestination(
    label: 'Bütçeler',
    icon: Icons.donut_small_outlined,
    selectedIcon: Icons.donut_small,
  );

  static const _cashDestination = ShellDestination(
    label: 'Kasa',
    icon: Icons.storefront_outlined,
    selectedIcon: Icons.storefront,
  );

  static const _moreDestination = ShellDestination(
    label: 'Diğer',
    icon: Icons.grid_view_outlined,
    selectedIcon: Icons.grid_view,
  );

  final StatefulNavigationShell navigationShell;
  final ScopeController? scopeController;

  /// Ana sekmeler; üçüncüsü onboarding ön ayarına göre değişir (ADR 0015).
  static List<ShellDestination> destinationsFor(bool hasBusiness) => [
    ..._leadingDestinations,
    hasBusiness ? _cashDestination : _budgetsDestination,
    _moreDestination,
  ];

  void _selectDestination(int index) {
    navigationShell.goBranch(
      index,
      initialLocation: index == navigationShell.currentIndex,
    );
  }

  @override
  Widget build(BuildContext context) {
    final controller = scopeController;
    if (controller == null) return _buildForProfile(context, false);
    return AnimatedBuilder(
      animation: controller,
      builder: (context, _) => _buildForProfile(context, controller.isVisible),
    );
  }

  Widget _buildForProfile(BuildContext context, bool hasBusiness) {
    final destinations = destinationsFor(hasBusiness);
    // Üç kademe: telefon dikeyde alt gezinme çubuğu, telefon yatay/küçük
    // tablette ikon rayı, geniş tablette etiketleri açık genişletilmiş ray.
    // Genişletilmiş ray dar ekranda içeriğe ayrılan yeri yiyeceği için yalnız
    // `expanded` sınıfında açılır.
    return switch (context.windowSize) {
      AppWindowSize.compact => _buildCompact(context, destinations),
      AppWindowSize.medium => _buildWithRail(
        context,
        destinations,
        extended: false,
      ),
      AppWindowSize.expanded => _buildWithRail(
        context,
        destinations,
        extended: true,
      ),
    };
  }

  Widget _buildWithRail(
    BuildContext context,
    List<ShellDestination> destinations, {
    required bool extended,
  }) {
    return Scaffold(
      body: SafeArea(
        child: Row(
          children: [
            NavigationRail(
              selectedIndex: navigationShell.currentIndex,
              onDestinationSelected: _selectDestination,
              extended: extended,
              labelType: extended ? null : NavigationRailLabelType.all,
              // Rayda çentik yoktur; birincil eylem rayın kendi başlığında
              // durur, böylece ekranın sağ altı içeriğe kalır.
              leading: Padding(
                padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
                child: extended
                    ? FloatingActionButton.extended(
                        heroTag: 'main-shell-new-transaction',
                        onPressed: () => openQuickAdd(context),
                        icon: const Icon(Icons.add),
                        label: const Text('İşlem ekle'),
                        shape: const StadiumBorder(),
                      )
                    : FloatingActionButton(
                        heroTag: 'main-shell-new-transaction',
                        tooltip: 'İşlem ekle',
                        onPressed: () => openQuickAdd(context),
                        child: const Icon(Icons.add),
                      ),
              ),
              destinations: [
                for (final destination in destinations)
                  NavigationRailDestination(
                    icon: Icon(destination.icon),
                    selectedIcon: Icon(destination.selectedIcon),
                    label: Text(destination.label),
                  ),
              ],
            ),
            const VerticalDivider(width: 1),
            Expanded(child: AppContentWidth(child: navigationShell)),
          ],
        ),
      ),
    );
  }

  Widget _buildCompact(
    BuildContext context,
    List<ShellDestination> destinations,
  ) {
    // Başlık sayfanın kendisinden gelir: shell de bir AppBar çizseydi
    // "İşlemler" gibi başlıklar ekranda iki kez görünürdü.
    //
    // Çubuk içeriğin üstünde yüzer ve içerik altından akar (`extendBody`);
    // sayfalar son satırı `AppPageInsets.aboveBar` ile çubuğun üstüne çıkarır.
    // Klavye açıkken çubuk gizlenir: alanın ve gönder düğmesinin üstünde
    // durmasın.
    final keyboardOpen = MediaQuery.viewInsetsOf(context).bottom > 0;
    return Scaffold(
      extendBody: true,
      body: navigationShell,
      bottomNavigationBar: keyboardOpen || hideBar
          ? null
          : ShellNavigationBar(
              destinations: destinations,
              currentIndex: navigationShell.currentIndex,
              onSelected: _selectDestination,
              onAdd: () => openQuickAdd(context),
            ),
    );
  }
}

/// Yüzen kapsül alt gezinme çubuğu: dört sekme ve ortada `+`.
///
/// Tasarım teslimi `design/design_handoff_altcubuk` (N7, 10 Kasım 2026).
/// Kapsülün boyu sabittir, yalnız genişliği ekrana uyar; konumu ekranın
/// altından verilir ([bottomOffset]). Arkasında sayfa renginde bir perde
/// vardır: içerik çubuğa yaklaşırken silinir ve çubuğun altından görünmez. İşlem eklemek
/// uygulamanın en sık yapılan işidir ve başparmağın durduğu yerde, çubuğun
/// ortasındadır. Seçili sekme dolgulu ikonla değil hap, renk ve kalın yazıyla
/// anlatılır.
class ShellNavigationBar extends StatelessWidget {
  const ShellNavigationBar({
    required this.destinations,
    required this.currentIndex,
    required this.onSelected,
    required this.onAdd,
    super.key,
  });

  final List<ShellDestination> destinations;
  final int currentIndex;
  final ValueChanged<int> onSelected;
  final VoidCallback onAdd;

  /// Kapsülün yüksekliği.
  static const double height = 64;

  /// Perdenin kapsülün üstünde kalan, içeriğin silindiği kısmı.
  static const double fade = AppSpacing.large;

  /// Kapsülün ekranın altından uzaklığı.
  ///
  /// Hareket çizgisi olan telefonda (iPhone 34, Android 16–24 dp'lik sistem
  /// boşluğu) kapsül çizginin hemen üstünde, sabit bir yerde durur: sistem
  /// boşluğunun tamamı eklenseydi iPhone'da çizgiyle arası 30 dp'ye çıkıyordu
  /// (10 Kasım 2026; büyük uygulamalarda bu ara 8 dp kadardır). Üç düğmeli
  /// gezinmede düğmelerin üstünde kalır.
  static double bottomOffset(BuildContext context) {
    final system = MediaQuery.viewPaddingOf(context).bottom;
    if (system == 0) return AppSpacing.compact;
    if (system < 40) return 22;
    return system + AppSpacing.small;
  }

  /// Ortadaki `+` yuvasının genişliği.
  static const double addSlot = 64;

  /// Seçili sekmenin hapı.
  static const double pillHeight = 56;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final colors = Theme.of(context).colorScheme;
    // Dört hedef ikiye ikiye ayrılır; `+` tam ortada kalır.
    final half = destinations.length ~/ 2;

    final bar = Padding(
      padding: EdgeInsets.fromLTRB(
        AppPageInsets.edge,
        fade,
        AppPageInsets.edge,
        bottomOffset(context),
      ),
      child: Container(
        height: height,
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.xSmall),
        decoration: BoxDecoration(
          color: surfaces.card,
          border: Border.all(color: surfaces.border),
          borderRadius: BorderRadius.circular(AppRadius.chip),
          boxShadow: [
            BoxShadow(
              color: surfaces.overlay,
              offset: const Offset(0, 2),
              blurRadius: 6,
            ),
            BoxShadow(
              color: surfaces.overlay,
              offset: const Offset(0, 1),
              blurRadius: 2,
            ),
          ],
        ),
        // Sekme yazısı büyük yazıda kapsülü büyütmez (Material'ın gezinme
        // çubuğu da sınırlar).
        child: MediaQuery.withClampedTextScaling(
          maxScaleFactor: 1.3,
          child: Row(
            children: [
              for (var index = 0; index < destinations.length; index++) ...[
                if (index == half)
                  SizedBox(
                    width: addSlot,
                    child: Center(
                      child: Tooltip(
                        message: 'İşlem ekle',
                        child: Semantics(
                          button: true,
                          label: 'İşlem ekle',
                          excludeSemantics: true,
                          child: Material(
                            key: const ValueKey('main-shell-new-transaction'),
                            color: colors.primary,
                            shape: const CircleBorder(),
                            clipBehavior: Clip.antiAlias,
                            child: InkWell(
                              onTap: onAdd,
                              child: SizedBox.square(
                                dimension: AppSizes.button,
                                child: Icon(
                                  Icons.add,
                                  size: 26,
                                  color: colors.onPrimary,
                                ),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                Expanded(
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.xxSmall / 2,
                    ),
                    child: _NavItem(
                      destination: destinations[index],
                      selected: index == currentIndex,
                      onTap: () => onSelected(index),
                    ),
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
    return Stack(
      children: [
        Positioned.fill(
          child: IgnorePointer(
            child: DecoratedBox(
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: [
                    surfaces.canvas.withValues(alpha: 0),
                    surfaces.canvas,
                  ],
                  stops: const [0, 0.5],
                ),
              ),
            ),
          ),
        ),
        bar,
      ],
    );
  }
}

class _NavItem extends StatelessWidget {
  const _NavItem({
    required this.destination,
    required this.selected,
    required this.onTap,
  });

  final ShellDestination destination;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final color = selected ? surfaces.ink : surfaces.inkMuted;
    final shape = BorderRadius.circular(AppRadius.chip);

    return Semantics(
      button: true,
      selected: selected,
      label: destination.label,
      excludeSemantics: true,
      child: Center(
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 200),
          curve: Curves.easeInOut,
          // Hap sekmenin tamamını kaplar, yalnız yazının genişliğini değil.
          width: double.infinity,
          height: ShellNavigationBar.pillHeight,
          decoration: BoxDecoration(
            color: selected
                ? surfaces.cardMuted
                : surfaces.cardMuted.withValues(alpha: 0),
            borderRadius: shape,
          ),
          child: Material(
            type: MaterialType.transparency,
            child: InkWell(
              onTap: onTap,
              borderRadius: shape,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    destination.icon,
                    size: 24,
                    weight: selected ? 600 : 400,
                    color: color,
                  ),
                  const SizedBox(height: AppSpacing.xxSmall),
                  Text(
                    destination.label,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.labelSmall?.copyWith(
                      fontSize: AppTextSizes.micro,
                      height: 16 / 12,
                      letterSpacing: 0,
                      color: color,
                      fontWeight: selected ? FontWeight.w600 : FontWeight.w500,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class ShellDestination {
  const ShellDestination({
    required this.label,
    required this.icon,
    required this.selectedIcon,
  });

  final String label;
  final IconData icon;
  final IconData selectedIcon;
}
