import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/theme/app_surfaces.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_divided_column.dart';
import 'package:business_finance_mobile/core/widgets/app_icon_capsule.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:business_finance_mobile/core/widgets/app_page_header.dart';
import 'package:business_finance_mobile/core/widgets/app_row.dart';
import 'package:business_finance_mobile/core/widgets/app_share_bar.dart';
import 'package:business_finance_mobile/core/widgets/app_status_chip.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';
import 'tasarim_ortak.dart';

/// `Kategori özeti` ve kategorinin sayfası: **kararlaştırılmış** hâl (Aşama
/// 06.3 Grup 8, kullanıcı kararı 5 Ekim 2026). Geliştirmenin görsel
/// kaynağıdır; uygulama bittiğinde kalkar.
void main() {
  Future<void> shoot(WidgetTester tester, String name, Widget page) =>
      captureScreen(tester, name, page, withNavBar: false);

  testWidgets('01 diğer menüsü', (tester) async {
    await shoot(tester, '01-diger-menusu', const _MoreMenu());
  }, skip: !screenshotsEnabled);

  testWidgets('02 kategori özeti', (tester) async {
    await shoot(tester, '02-kategori-ozeti', const _Summary());
  }, skip: !screenshotsEnabled);

  testWidgets('03 ay seçici', (tester) async {
    await shoot(
      tester,
      '03-ay-secici',
      const DesignDialogOver(page: _Summary(), child: DesignMonthDialog()),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('04 özet · hesap menüsü', (tester) async {
    await shoot(tester, '04-ozet-hesap-menusu', const _Summary(menu: true));
  }, skip: !screenshotsEnabled);

  testWidgets('05 kategori sayfası', (tester) async {
    await shoot(tester, '05-kategori-sayfasi', const _Category());
  }, skip: !screenshotsEnabled);

  testWidgets('06 hesap menüsü', (tester) async {
    await shoot(
      tester,
      '06-kategori-hesap-menusu',
      const _Category(menu: _Menu.source),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('07 hesap seçilmiş', (tester) async {
    await shoot(
      tester,
      '07-kategori-hesap-secilmis',
      const _Category(source: 'Dükkan Kasası'),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('08 seçiliyken menü', (tester) async {
    await shoot(
      tester,
      '08-kategori-seciliyken-menu',
      const _Category(source: 'Dükkan Kasası', menu: _Menu.source),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('09 kişi menüsü', (tester) async {
    await shoot(
      tester,
      '09-kategori-kisi-menusu',
      const _Category(menu: _Menu.person),
    );
  }, skip: !screenshotsEnabled);
}

enum _Menu { source, person }

// ------------------------------------------------------------ Diğer

/// Menünün **sırası** gösterilir: `Kategoriler`, `Para ve hesaplar`ın altında
/// ve `Planlama`nın üstündedir. Satırların görünümü uygulamadaki menüyle
/// aynı kalır.
class _MoreMenu extends StatelessWidget {
  const _MoreMenu();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    Widget group(String title, List<(IconData, String)> items) => Padding(
      padding: const EdgeInsets.only(bottom: AppSpacing.medium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(bottom: AppSpacing.xSmall),
            child: Text(
              title,
              style: theme.textTheme.titleSmall?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
          ),
          AppCard(
            padding: EdgeInsets.zero,
            child: AppDividedColumn(
              inset: AppIconCapsule.rowInset,
              children: [
                for (final (icon, label) in items)
                  AppRow(
                    onTap: () {},
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.medium,
                      vertical: AppSpacing.xSmall,
                    ),
                    leading: AppIconCapsule(
                      icon: icon,
                      tone: AppStatusTone.neutral,
                    ),
                    title: label,
                    trailing: Icon(
                      Icons.chevron_right,
                      color: surfaces.inkMuted,
                    ),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            const AppPageHeader(title: 'Diğer'),
            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                ),
                children: [
                  group('Para ve hesaplar', const [
                    (
                      Icons.account_balance_wallet_outlined,
                      'Hesaplar ve transferler',
                    ),
                    (Icons.credit_card_outlined, 'Kredi kartlarım'),
                    (Icons.handshake_outlined, 'Borç ve alacaklar'),
                    (Icons.people_outline, 'Cari hesap'),
                  ]),
                  group('Kategoriler', const [
                    (Icons.pie_chart_outline, 'Kategori özeti'),
                    (Icons.category_outlined, 'Kategori yönetimi'),
                  ]),
                  group('Planlama', const [
                    (Icons.donut_small_outlined, 'Bütçeler'),
                    (Icons.event_note_outlined, 'Yükümlülükler'),
                    (Icons.savings_outlined, 'Tasarruf hedefleri'),
                    (Icons.event_repeat, 'Planlama ve raporlar'),
                  ]),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ----------------------------------------------------- Kategori özeti

const _spent = [
  ('Ticari mal alımı', Icons.inventory_2_outlined, '12480.0000', 0.46),
  ('İşyeri kirası', Icons.storefront_outlined, '6500.0000', 0.24),
  ('Personel ücreti', Icons.badge_outlined, '4200.0000', 0.16),
  ('Elektrik, su, doğalgaz', Icons.bolt_outlined, '1860.0000', 0.07),
  ('Banka ve POS komisyonu', Icons.percent, '1140.0000', 0.04),
  ('Kasa farkı', Icons.difference_outlined, '150.0000', 0.01),
];

class _Summary extends StatelessWidget {
  const _Summary({this.menu = false});

  final bool menu;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    return Scaffold(
      body: SafeArea(
        child: Stack(
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                AppPageHeader(
                  title: 'Kategori özeti',
                  onBack: () {},
                  actions: [designMonthButton()],
                ),
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                  ),
                  child: designRail(context, const ['Gider', 'Gelir'], 'Gider'),
                ),
                const SizedBox(height: AppSpacing.small),
                designChipRow([
                  designChoice('Hepsi'),
                  designChoice('İşletme', selected: true),
                  designChoice('Şahsi'),
                  designDropChip('Hesap ya da kart'),
                ]),
                Expanded(
                  child: ListView(
                    padding: const EdgeInsets.fromLTRB(
                      AppSpacing.medium,
                      0,
                      AppSpacing.medium,
                      AppSpacing.large,
                    ),
                    children: [
                      AppCard(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'Ekim gideri · İşletme',
                              style: theme.textTheme.bodySmall,
                            ),
                            const SizedBox(height: AppSpacing.xxSmall),
                            const AppMoneyText(
                              amount: '26330.0000',
                              currency: 'TRY',
                              size: AppMoneySize.metric,
                            ),
                            const SizedBox(height: AppSpacing.xSmall),
                            Text(
                              '6 kategori · geçen ay ₺24.180,00',
                              style: theme.textTheme.bodySmall,
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: AppSpacing.medium),
                      AppCard(
                        padding: EdgeInsets.zero,
                        child: AppDividedColumn(
                          inset: AppIconCapsule.rowInset,
                          children: [
                            for (final item in _spent)
                              AppRow(
                                onTap: () {},
                                leading: AppIconCapsule(
                                  icon: item.$2,
                                  tone: AppStatusTone.expense,
                                ),
                                title: item.$1,
                                subtitle: 'Pay %${(item.$4 * 100).round()}',
                                trailing: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Column(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.end,
                                      mainAxisSize: MainAxisSize.min,
                                      children: [
                                        AppMoneyText(
                                          amount: item.$3,
                                          currency: 'TRY',
                                          size: AppMoneySize.row,
                                        ),
                                        const SizedBox(
                                          height: AppSpacing.xSmall,
                                        ),
                                        SizedBox(
                                          width: 72,
                                          child: AppShareBar(
                                            ratio: item.$4 * 2,
                                            color: colors.expenseFill,
                                          ),
                                        ),
                                      ],
                                    ),
                                    Icon(
                                      Icons.chevron_right,
                                      color: surfaces.inkMuted,
                                    ),
                                  ],
                                ),
                              ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            if (menu)
              const Positioned(
                right: AppSpacing.medium,
                top: 190,
                child: DesignMenu(
                  items: [
                    ('Hepsi', '', true),
                    ('Dükkan Kasası', '9840.0000', false),
                    ('Ziraat Vadesiz', '12560.0000', false),
                    ('Bonus Kart', '3930.0000', false),
                  ],
                ),
              ),
          ],
        ),
      ),
    );
  }
}

// ---------------------------------------------- Kategorinin sayfası

/// Kategorinin sayfası. Süzgeç çipine dokununca seçenekler çipin altında
/// açılır; seçim yapılınca **çip seçilenin adını yazar** ve ona dokununca
/// diğer seçenekler çıkar.
class _Category extends StatelessWidget {
  const _Category({this.source, this.menu});

  final String? source;
  final _Menu? menu;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final filtered = source != null;
    return Scaffold(
      body: SafeArea(
        child: Stack(
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                AppPageHeader(
                  title: 'Ticari mal alımı',
                  onBack: () {},
                  actions: [designMonthButton()],
                ),
                Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppSpacing.medium,
                    0,
                    AppSpacing.medium,
                    AppSpacing.small,
                  ),
                  child: AppCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          filtered ? 'Ekim gideri · $source' : 'Ekim gideri',
                          style: theme.textTheme.bodySmall,
                        ),
                        const SizedBox(height: AppSpacing.xxSmall),
                        AppMoneyText(
                          amount: filtered ? '5100.0000' : '12480.0000',
                          currency: 'TRY',
                          size: AppMoneySize.metric,
                          effect: AppMoneyEffect.expense,
                        ),
                        const SizedBox(height: AppSpacing.xSmall),
                        Text(
                          filtered
                              ? 'Kategorinin tamamı ₺12.480,00'
                              : 'Geçen ay ₺10.940,00',
                          style: theme.textTheme.bodySmall,
                        ),
                      ],
                    ),
                  ),
                ),
                designChipRow([
                  designDropChip('Hesap ya da kart', value: source),
                  designDropChip('Kişi'),
                ]),
                Expanded(
                  child: SingleChildScrollView(
                    child: DesignDays(
                      items: filtered
                          ? _items
                                .where((item) => item.sourceName == source)
                                .toList()
                          : _items,
                    ),
                  ),
                ),
              ],
            ),
            if (menu == _Menu.source)
              Positioned(
                left: AppSpacing.medium,
                top: 238,
                child: DesignMenu(
                  items: [
                    ('Hepsi', '12480.0000', !filtered),
                    ('Dükkan Kasası', '5100.0000', filtered),
                    const ('Ziraat Vadesiz', '4200.0000', false),
                    const ('Bonus Kart', '3180.0000', false),
                  ],
                ),
              ),
            if (menu == _Menu.person)
              const Positioned(
                right: AppSpacing.medium,
                top: 238,
                child: DesignMenu(
                  items: [
                    ('Hepsi', '', true),
                    ('Örnek Toptan Kağıt', '3180.0000', false),
                    ('Test Toptancı', '2600.0000', false),
                  ],
                ),
              ),
          ],
        ),
      ),
    );
  }
}

final _items = [
  designActivity(
    'Toptancı ödemesi',
    '2026-10-05',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '4200.0000',
    category: 'Ticari mal alımı',
    source: 'Ziraat Vadesiz',
  ),
  designActivity(
    'Kırtasiye toptan',
    '2026-10-04',
    ActivityKind.cardCharge,
    ActivityEffect.expense,
    '3180.0000',
    category: 'Ticari mal alımı',
    source: 'Bonus Kart',
    group: ActivitySourceGroup.creditCard,
  ),
  designActivity(
    'Hal alışverişi',
    '2026-10-02',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '2600.0000',
    category: 'Ticari mal alımı',
    source: 'Dükkan Kasası',
  ),
  designActivity(
    'Ambalaj',
    '2026-10-01',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '2500.0000',
    category: 'Ticari mal alımı',
    source: 'Dükkan Kasası',
  ),
];
