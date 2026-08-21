import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../auth/presentation/auth_controller.dart';

class MorePage extends StatelessWidget {
  const MorePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Diğer')),
      body: _body(context),
    );
  }

  Widget _body(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.all(AppSpacing.medium),
      children: [
        // Sıra frekansa değil, ne yaptığınıza göre. İlk ikisi yalnız bakmak
        // için açtığınız yerler; sonraki üçü kurduğunuz şeyler; sonuncular
        // bakış ve dosya işleri.
        //
        // Önceki hâlde tek bir satır "İçe aktarma, borç, hedef ve yedek"
        // diyordu: dört alakasız şeyi sayan bir başlık, gruplamanın yanlış
        // olduğunun kendi itirafıydı.
        //
        // Yedisi **tek** kartın içinde. Menü satırı bir liste kaydı değil, bir
        // kapı: hepsi aynı yere ait ve aralarına boşluk koymak yedi ayrı kutu
        // izlenimi veriyordu. Ayrım için çerçeve değil ince ayırıcı yeter —
        // ayarlar listeleri her yerde böyle kurulu.
        _MenuGroup(
          items: [
            _MenuItem(
              icon: Icons.account_balance_wallet_outlined,
              title: 'Hesaplar ve transferler',
              onTap: () => context.push('/more/accounts'),
            ),
            _MenuItem(
              icon: Icons.credit_card_outlined,
              title: 'Kredi kartları',
              onTap: () => context.push('/more/cards'),
            ),
            _MenuItem(
              icon: Icons.category_outlined,
              title: 'Kategoriler',
              onTap: () => context.push('/more/categories'),
            ),
            _MenuItem(
              icon: Icons.handshake_outlined,
              title: 'Borç ve alacaklar',
              onTap: () => context.push('/more/debts'),
            ),
            _MenuItem(
              icon: Icons.savings_outlined,
              title: 'Tasarruf hedefleri',
              onTap: () => context.push('/more/goals'),
            ),
            _MenuItem(
              icon: Icons.event_repeat,
              title: 'Planlama ve raporlar',
              onTap: () => context.push('/more/planning'),
            ),
            _MenuItem(
              icon: Icons.folder_outlined,
              title: 'Veri ve yedek',
              onTap: () => context.push('/more/data-tools'),
            ),
          ],
        ),
        const Divider(height: AppSpacing.xLarge),
        AppCard(
          padding: EdgeInsets.zero,
          child: AppListRow(
            icon: Icons.logout,
            title: 'Çıkış yap',
            subtitle: 'Bu cihazdaki güvenli oturum kapatılır.',
            onTap: () => _confirmLogout(context),
          ),
        ),
      ],
    );
  }

  Future<void> _confirmLogout(BuildContext context) async {
    final shouldLogout = await AppConfirmDialog.show(
      context: context,
      icon: Icons.logout,
      title: 'Çıkış yapılsın mı?',
      message: 'Bu cihazdaki oturum bilgileri güvenli biçimde silinecek.',
      confirmLabel: 'Çıkış yap',
    );
    if (shouldLogout && context.mounted) {
      await context.read<AuthController>().logout();
    }
  }
}

class _MenuItem {
  const _MenuItem({
    required this.icon,
    required this.title,
    required this.onTap,
  });

  final IconData icon;
  final String title;
  final VoidCallback onTap;
}

/// Tek çerçeve, içinde satırlar.
///
/// Kartın kendisi bir kez çiziliyor; satırlar arasındaki sınırı ikon
/// hizasından başlayan ince bir ayırıcı söylüyor. Satır başına ayrı kart
/// (ister yapışık ister boşluklu) iki yönden de yanlıştı: yapışık hâlde yan
/// yana iki kenarlık kalın bir çizgi gibi görünüyor, boşluklu hâlde ise
/// birbiriyle ilgili yedi kapı yedi ayrı kutu gibi okunuyordu.
class _MenuGroup extends StatelessWidget {
  const _MenuGroup({required this.items});

  final List<_MenuItem> items;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);

    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        children: [
          for (final (index, item) in items.indexed) ...[
            if (index > 0)
              Divider(
                height: 1,
                thickness: 1,
                // Ayırıcı ikonun altından değil başlığın hizasından başlar;
                // ikon sütunu kesintisiz kalır.
                indent: AppSpacing.medium + 40 + AppSpacing.medium,
                color: surfaces.border,
              ),
            AppListRow(
              icon: item.icon,
              title: item.title,
              trailing: const Icon(Icons.chevron_right),
              onTap: item.onTap,
            ),
          ],
        ],
      ),
    );
  }
}
