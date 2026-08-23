import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../auth/presentation/auth_controller.dart';
import '../../profile/data/profile_repository.dart';

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
            // Cari hesap ile taksitli sözleşme kardeş kapılar: biri
            // yürüyen bir hesap, diğeri vadesi belli bir plan. Aynı kapıya
            // koymak, kullanıcıya iki farklı soruyu tek yerde sordurur.
            _MenuItem(
              icon: Icons.people_outline,
              title: 'Cari hesap',
              onTap: () => context.push('/more/counterparties'),
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
        const _BusinessAnswerCard(),
        const SizedBox(height: AppSpacing.medium),
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

/// Kaydolurken sorulan sorunun sonradan değiştirilebildiği yer.
///
/// Cevap **hiçbir özelliği kapatmaz**: yalnız işletme/şahsi ayrımının
/// arayüzde görünüp görünmeyeceğini belirler. Kategorilere dokunmaz — o
/// noktada liste artık kullanıcınındır ve sildiği bir kategoriyi geri
/// getirmek silme eylemini anlamsız kılardı.
class _BusinessAnswerCard extends StatefulWidget {
  const _BusinessAnswerCard();

  @override
  State<_BusinessAnswerCard> createState() => _BusinessAnswerCardState();
}

class _BusinessAnswerCardState extends State<_BusinessAnswerCard> {
  bool _isSaving = false;

  @override
  Widget build(BuildContext context) {
    final scopeController = context.watch<ScopeController?>();
    final repository = context.read<ProfileRepositoryContract?>();
    // Bağlanmamış kabukta (test ya da bağımlılıksız kurulum) kart hiç
    // çizilmez: değiştirilemeyen bir anahtar göstermek, kullanıcıya
    // çalışmayan bir düğme vermek olurdu.
    if (scopeController == null || repository == null) {
      return const SizedBox.shrink();
    }
    return AppCard(
      child: SwitchListTile(
        value: scopeController.isVisible,
        onChanged: _isSaving
            ? null
            : (value) => _save(repository, scopeController, value),
        contentPadding: EdgeInsets.zero,
        title: const Text('İşletmem var'),
        subtitle: const Text(
          'Açıkken kayıtlarınızı işletme ve şahsi olarak ayrı '
          'okuyabilirsiniz. Kapalıyken bu ayrım hiç görünmez. '
          'Kategorileriniz iki durumda da olduğu gibi kalır.',
        ),
      ),
    );
  }

  Future<void> _save(
    ProfileRepositoryContract repository,
    ScopeController scopeController,
    bool value,
  ) async {
    setState(() => _isSaving = true);
    final messenger = ScaffoldMessenger.of(context);
    try {
      final profile = await repository.update(hasBusiness: value);
      // Sunucunun döndürdüğü hâl uygulanır; istemcinin gönderdiği değil.
      await scopeController.applyHasBusiness(profile.hasBusiness);
    } on ApiException catch (error) {
      messenger.showSnackBar(SnackBar(content: Text(error.message)));
    } on FormatException {
      messenger.showSnackBar(
        const SnackBar(
          content: Text('Sunucudan beklenmeyen bir yanıt alındı.'),
        ),
      );
    } finally {
      if (mounted) setState(() => _isSaving = false);
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
