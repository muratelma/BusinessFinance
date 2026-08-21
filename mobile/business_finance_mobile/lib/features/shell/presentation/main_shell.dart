import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_content_width.dart';
import '../../activities/presentation/quick_add_navigation.dart';

class MainShell extends StatelessWidget {
  const MainShell({required this.navigationShell, super.key});

  static const _destinations = [
    _ShellDestination(
      label: 'Özet',
      icon: Icons.space_dashboard_outlined,
      selectedIcon: Icons.space_dashboard,
    ),
    _ShellDestination(
      label: 'İşlemler',
      icon: Icons.receipt_long_outlined,
      selectedIcon: Icons.receipt_long,
    ),
    _ShellDestination(
      label: 'Bütçeler',
      icon: Icons.donut_small_outlined,
      selectedIcon: Icons.donut_small,
    ),
    _ShellDestination(
      label: 'Diğer',
      icon: Icons.more_horiz,
      selectedIcon: Icons.more,
    ),
  ];

  final StatefulNavigationShell navigationShell;

  void _selectDestination(int index) {
    navigationShell.goBranch(
      index,
      initialLocation: index == navigationShell.currentIndex,
    );
  }

  @override
  Widget build(BuildContext context) {
    // Üç kademe: telefon dikeyde alt gezinme çubuğu, telefon yatay/küçük
    // tablette ikon rayı, geniş tablette etiketleri açık genişletilmiş ray.
    // Genişletilmiş ray dar ekranda içeriğe ayrılan yeri yiyeceği için yalnız
    // `expanded` sınıfında açılır.
    return switch (context.windowSize) {
      AppWindowSize.compact => _buildCompact(context),
      AppWindowSize.medium => _buildWithRail(context, extended: false),
      AppWindowSize.expanded => _buildWithRail(context, extended: true),
    };
  }

  Widget _buildWithRail(BuildContext context, {required bool extended}) {
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
                for (final destination in _destinations)
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

  Widget _buildCompact(BuildContext context) {
    // Başlık sayfanın kendisinden gelir: shell de bir AppBar çizseydi
    // "İşlemler" gibi başlıklar ekranda iki kez görünürdü.
    return Scaffold(
      body: navigationShell,
      // Birincil eylem çubuğun ortasına oturur ve çubuk ona bir çentik açar:
      // işlem eklemek uygulamanın en sık yapılan işidir, başparmağın doğal
      // durduğu yerde olmalı.
      floatingActionButtonLocation: FloatingActionButtonLocation.centerDocked,
      floatingActionButton: FloatingActionButton(
        heroTag: 'main-shell-new-transaction',
        tooltip: 'İşlem ekle',
        onPressed: () => openQuickAdd(context),
        child: const Icon(Icons.add),
      ),
      bottomNavigationBar: _NotchedNavigationBar(
        destinations: _destinations,
        currentIndex: navigationShell.currentIndex,
        onSelected: _selectDestination,
      ),
    );
  }
}

/// Ortasında kayan eylem butonu için çentik bulunan alt gezinme çubuğu.
///
/// `NavigationBar` çentiği desteklemediği için çubuk elle çiziliyor. Bunun
/// bedeli, `NavigationBar`'ın hazır verdiği seçili göstergesini ve
/// erişilebilirlik anlamlarını burada açıkça vermek zorunda olmak.
class _NotchedNavigationBar extends StatelessWidget {
  const _NotchedNavigationBar({
    required this.destinations,
    required this.currentIndex,
    required this.onSelected,
  });

  final List<_ShellDestination> destinations;
  final int currentIndex;
  final ValueChanged<int> onSelected;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    // Dört hedef ikiye ikiye ayrılır; çentik tam ortada kalır.
    final half = destinations.length ~/ 2;

    return BottomAppBar(
      color: surfaces.card,
      elevation: 0,
      shape: const CircularNotchedRectangle(),
      // Butonla çubuk arasındaki nefes payı: çentik butona yapışırsa buton
      // çubuğun bir parçası gibi görünür ve yüzdüğü okunmaz.
      notchMargin: AppSpacing.small,
      padding: EdgeInsets.zero,
      height: 76,
      child: Row(
        children: [
          for (var index = 0; index < destinations.length; index++) ...[
            if (index == half) const SizedBox(width: AppSpacing.navNotch),
            Expanded(
              child: _NavItem(
                destination: destinations[index],
                selected: index == currentIndex,
                onTap: () => onSelected(index),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class _NavItem extends StatelessWidget {
  const _NavItem({
    required this.destination,
    required this.selected,
    required this.onTap,
  });

  final _ShellDestination destination;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final color = selected
        ? theme.colorScheme.onSurface
        : theme.colorScheme.onSurfaceVariant;

    return Semantics(
      button: true,
      selected: selected,
      label: destination.label,
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppRadius.field),
        child: SizedBox(
          // 48 dp dokunma hedefi çubuğun kendi yüksekliğinden gelir.
          height: 76,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(
                selected ? destination.selectedIcon : destination.icon,
                size: 24,
                color: color,
              ),
              const SizedBox(height: AppSpacing.xSmall),
              Text(
                destination.label,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.labelSmall?.copyWith(
                  color: color,
                  fontWeight: selected ? FontWeight.w700 : FontWeight.w600,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _ShellDestination {
  const _ShellDestination({
    required this.label,
    required this.icon,
    required this.selectedIcon,
  });

  final String label;
  final IconData icon;
  final IconData selectedIcon;
}
