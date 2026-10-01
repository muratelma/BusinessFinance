import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/presentation/scope_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_avatar.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../account/presentation/account_status_controller.dart';

/// `Diğer`: ana sekmelere sığmayan kapılar.
///
/// Tasarım teslimi (27 Eylül 2026, DigerV2): üstte hesap kartı, altında
/// kullanıcının sorusuna göre dört grup — *param nerede* (Para ve hesaplar),
/// *ne zaman ne olacak* (Planlama), *vergim ne zaman* (Vergi, yalnız
/// işletmesi olana) ve *ayarlarım* (Ayarlar). Her grup tek
/// kart; satırlar arasında ikon hizasından başlayan ince ayırıcı var.
class MorePage extends StatelessWidget {
  const MorePage({super.key});

  @override
  Widget build(BuildContext context) {
    final hasBusiness = context.watch<ScopeController?>()?.isVisible ?? false;
    final groups = _groups(context, hasBusiness);
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            const AppPageHeader(title: 'Diğer'),
            Expanded(
              child: ListView(
                // Alt boşluk `+` düğmesini aşacak kadar: son satır düğmenin
                // altında kalıp dokunuşu yutmamalı.
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  AppSpacing.xSmall,
                  AppSpacing.medium,
                  AppSpacing.fabClearance,
                ),
                children: [
                  const _AccountCard(),
                  for (final group in groups) ...[
                    const SizedBox(height: AppSpacing.large),
                    _MenuGroup(group: group),
                  ],
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  List<_Group> _groups(BuildContext context, bool hasBusiness) {
    void go(String location) => context.push(location);
    return [
      _Group('Para ve hesaplar', [
        _MenuItem(
          icon: Icons.account_balance_wallet_outlined,
          title: 'Hesaplar ve transferler',
          onTap: () => go('/more/accounts'),
        ),
        // Borçlandığınız kart. Tahsil ettiğiniz POS ayrı bir şeydir ve
        // `Kasa` altındadır (ADR 0015).
        _MenuItem(
          icon: Icons.credit_card_outlined,
          title: 'Kredi kartlarım',
          onTap: () => go('/more/cards'),
        ),
        // Kişisel profilde üçüncü sekme `Bütçeler`dir; yerinden inen `Kasa`
        // paranın durduğu yerlerin yanına gelir (ADR 0015).
        if (!hasBusiness)
          _MenuItem(
            icon: Icons.point_of_sale_outlined,
            title: 'Kasa',
            onTap: () => go('/more/cash'),
          ),
        _MenuItem(
          icon: Icons.handshake_outlined,
          title: 'Borç ve alacaklar',
          onTap: () => go('/more/debts'),
        ),
        _MenuItem(
          icon: Icons.people_outline,
          title: 'Cari hesap',
          onTap: () => go('/more/counterparties'),
        ),
      ]),
      _Group('Planlama', [
        // İşletme profilinde üçüncü sekme `Kasa`; `Bütçeler` buradadır.
        if (hasBusiness)
          _MenuItem(
            icon: Icons.donut_small_outlined,
            title: 'Bütçeler',
            onTap: () => go('/more/budgets'),
          ),
        _MenuItem(
          icon: Icons.event_note_outlined,
          title: 'Yükümlülükler',
          onTap: () => go('/more/obligations'),
        ),
        _MenuItem(
          icon: Icons.savings_outlined,
          title: 'Tasarruf hedefleri',
          onTap: () => go('/more/goals'),
        ),
        _MenuItem(
          icon: Icons.event_repeat,
          title: 'Planlama ve raporlar',
          onTap: () => go('/more/planning'),
        ),
      ]),
      // Vergi tarafı yalnız işletmesi olana açılır (06.2 Grup 1).
      if (hasBusiness)
        _Group('Vergi', [
          _MenuItem(
            icon: Icons.receipt_long_outlined,
            title: 'Vergi takibi',
            onTap: () => go(taxesLocation),
          ),
        ]),
      _Group('Ayarlar', [
        _MenuItem(
          icon: Icons.category_outlined,
          title: 'Kategoriler',
          onTap: () => go('/more/categories'),
        ),
        // Hatırlatma bir cihaz ayarıdır, hesap ayarı değil: aynı hesaba başka
        // bir telefondan girildiğinde o telefon kendi kararını taşır.
        _MenuItem(
          icon: Icons.notifications_active_outlined,
          title: 'Hatırlatmalar',
          onTap: () => go(remindersLocation),
        ),
        _MenuItem(
          icon: Icons.folder_outlined,
          title: 'Veri ve yedek',
          onTap: () => go('/more/data-tools'),
        ),
      ]),
    ];
  }
}

/// Hesabın ikinci kapısı (birincisi Özet'in sağ üstündeki avatar): baş
/// harfler, e-posta ve doğrulanmamış adresin kalıcı uyarısı.
class _AccountCard extends StatelessWidget {
  const _AccountCard();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final status = context.watch<AccountStatusController?>();
    final email = status?.email;
    final unverified = status?.needsEmailVerification ?? false;
    // Kartın dokunma düğümü ile metni tek düğüm: ekran okuyucu adsız bir
    // düğme okumasın.
    return MergeSemantics(
      child: Semantics(
        button: true,
        child: AppCard(
          padding: EdgeInsets.zero,
          onTap: () => context.push(accountLocation),
          child: Padding(
            padding: const EdgeInsets.all(AppSpacing.medium),
            child: Row(
              children: [
                AppAvatar(initials: status?.initials ?? '', size: 48),
                const SizedBox(width: AppSpacing.medium),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Hesabım', style: theme.textTheme.titleSmall),
                      if (email != null) ...[
                        const SizedBox(height: AppSpacing.xxSmall),
                        Text(
                          email,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: theme.textTheme.bodySmall,
                        ),
                      ],
                      if (unverified) ...[
                        const SizedBox(height: AppSpacing.xSmall),
                        const AppStatusChip(
                          label: 'E-posta doğrulanmadı',
                          icon: Icons.mail_outline,
                          tone: AppStatusTone.expense,
                        ),
                      ],
                    ],
                  ),
                ),
                const SizedBox(width: AppSpacing.small),
                Icon(Icons.chevron_right, size: 22, color: surfaces.inkMuted),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _Group {
  const _Group(this.label, this.items);

  final String label;
  final List<_MenuItem> items;
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

/// Grup etiketi ve tek kart; satırlar ikon kapsülü, başlık ve chevron.
class _MenuGroup extends StatelessWidget {
  const _MenuGroup({required this.group});

  final _Group group;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.xSmall,
            0,
            AppSpacing.xSmall,
            AppSpacing.small,
          ),
          child: Semantics(
            header: true,
            child: Text(
              group.label,
              style: theme.textTheme.labelMedium?.copyWith(
                fontSize: 14,
                letterSpacing: 0.1,
                color: surfaces.inkMuted,
              ),
            ),
          ),
        ),
        AppCard(
          padding: EdgeInsets.zero,
          child: AppDividedColumn(
            inset: AppIconCapsule.rowInset,
            children: [
              for (final item in group.items)
                AppRow(
                  padding: const EdgeInsets.fromLTRB(
                    AppSpacing.medium,
                    AppSpacing.small,
                    AppSpacing.small + AppSpacing.xSmall,
                    AppSpacing.small,
                  ),
                  leading: AppIconCapsule(icon: item.icon),
                  title: item.title,
                  trailing: Icon(
                    Icons.chevron_right,
                    size: 22,
                    color: surfaces.inkMuted,
                  ),
                  onTap: item.onTap,
                ),
            ],
          ),
        ),
      ],
    );
  }
}
