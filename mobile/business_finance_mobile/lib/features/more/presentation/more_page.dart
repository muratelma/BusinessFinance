import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/presentation/scope_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_list_row.dart';

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
    final hasBusiness = context.watch<ScopeController?>()?.isVisible ?? false;
    return ListView(
      // Alt boşluk `+` düğmesini aşacak kadar: kabul turunda son satır
      // (`Muhasebeci paketi`) düğmenin altında kalıyordu ve dokunuş menüye
      // değil düğmeye gidiyordu.
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.medium,
        AppSpacing.medium,
        AppSpacing.fabClearance,
      ),
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
              // Borçlandığınız kart. Tahsil ettiğiniz POS ayrı bir şeydir ve
              // `Kasa` altındadır (ADR 0015).
              title: 'Kredi kartlarım',
              onTap: () => context.push('/more/cards'),
            ),
            if (hasBusiness)
              _MenuItem(
                icon: Icons.donut_small_outlined,
                title: 'Bütçeler',
                onTap: () => context.push('/more/budgets'),
              )
            else
              _MenuItem(
                icon: Icons.point_of_sale_outlined,
                title: 'Kasa',
                onTap: () => context.push('/more/cash'),
              ),
            _MenuItem(
              icon: Icons.category_outlined,
              title: 'Kategoriler',
              onTap: () => context.push('/more/categories'),
            ),
            // Cari hesap ile taksitli sözleşme kardeş kapılar: biri
            // yürüyen bir hesap, diğeri vadesi belli bir plan. Aynı kapıya
            // koymak, kullanıcıya iki farklı soruyu tek yerde sordurur.
            _MenuItem(
              icon: Icons.people_outline,
              title: 'Cari hesap',
              onTap: () => context.push('/more/counterparties'),
            ),
            _MenuItem(
              icon: Icons.event_note_outlined,
              title: 'Yükümlülükler',
              onTap: () => context.push('/more/obligations'),
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
            // Vergi tarafı yalnız işletmesi olana açılır: kapsamı arayüzünde
            // hiç görmeyen kullanıcının KDV beyanı ve muhasebecisi yoktur.
            if (hasBusiness) ...[
              _MenuItem(
                icon: Icons.event_available_outlined,
                title: 'Vergi takvimi',
                onTap: () => context.push('/more/tax-calendar'),
              ),
              _MenuItem(
                icon: Icons.description_outlined,
                title: 'Muhasebeci paketi',
                onTap: () => context.push('/more/accountant-package'),
              ),
            ],
            // Hatırlatma bir cihaz ayarıdır, hesap ayarı değil: aynı hesaba
            // başka bir telefondan girildiğinde o telefon kendi kararını
            // taşır. Bu yüzden `Hesabım` içinde değil, kendi kapısında.
            _MenuItem(
              icon: Icons.notifications_active_outlined,
              title: 'Hatırlatmalar',
              onTap: () => context.push(remindersLocation),
            ),
            _MenuItem(
              icon: Icons.folder_outlined,
              title: 'Veri ve yedek',
              onTap: () => context.push('/more/data-tools'),
            ),
            // Hesabın ikinci kapısı. Birincisi Özet'in sağ üstündeki ikon;
            // ikisinin birden durup durmayacağına Aşama 06.2 karar verir.
            _MenuItem(
              icon: Icons.account_circle_outlined,
              title: 'Hesabım',
              onTap: () => context.push(accountLocation),
            ),
          ],
        ),
      ],
    );
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
