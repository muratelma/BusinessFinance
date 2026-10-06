import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/theme/app_surfaces.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_divided_column.dart';
import 'package:business_finance_mobile/core/widgets/app_icon_capsule.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:business_finance_mobile/core/widgets/app_page_header.dart';
import 'package:business_finance_mobile/core/widgets/app_row.dart';
import 'package:business_finance_mobile/core/widgets/app_status_chip.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';
import 'tasarim_ortak.dart';

/// Hesabın kendi sayfası: karar çizimleri (Aşama 06.3 Grup 8). Kategori
/// sayfasında kararlaştırılan kalıpla kuruldu: ay sağ üstte, süzgeç çipin
/// altında açılan menüyle, seçim çipin adında.
void main() {
  Future<void> shoot(WidgetTester tester, String name, Widget page) =>
      captureScreen(tester, name, page, withNavBar: false);

  testWidgets('01 hesaplar listesi', (tester) async {
    await shoot(tester, '01-hesaplar-listesi', const _Accounts());
  }, skip: !screenshotsEnabled);

  testWidgets('02 hesap sayfası', (tester) async {
    await shoot(tester, '02-hesap-sayfasi', const _Account());
  }, skip: !screenshotsEnabled);

  testWidgets('03 kategori menüsü', (tester) async {
    await shoot(tester, '03-hesap-kategori-menusu', const _Account(menu: true));
  }, skip: !screenshotsEnabled);

  testWidgets('04 çıkan ve kategori seçilmiş', (tester) async {
    await shoot(
      tester,
      '04-hesap-cikan-kategori-secilmis',
      const _Account(direction: 'Çıkan', category: 'İşyeri kirası'),
    );
  }, skip: !screenshotsEnabled);
}

/// `Hesaplar`: satıra dokununca hesabın hareketleri açılır; düzenleme
/// satırın sonundaki kalemdedir.
class _Accounts extends StatelessWidget {
  const _Accounts();

  static const _rows = [
    (
      'Ziraat Vadesiz',
      'Banka · İşletme',
      '13916.8650',
      Icons.account_balance_outlined,
    ),
    (
      'Dükkan Kasası',
      'Nakit · İşletme',
      '35900.0000',
      Icons.storefront_outlined,
    ),
    (
      'Birikim Hesabı',
      'Banka · Şahsi',
      '66176.0000',
      Icons.account_balance_outlined,
    ),
    ('Şahsi Cüzdan', 'Nakit · Şahsi', '1630.0000', Icons.wallet),
  ];

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Hesaplar',
              onBack: () {},
              actions: [
                IconButton(onPressed: () {}, icon: const Icon(Icons.add)),
              ],
            ),
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
              ),
              child: designRail(context, const [
                'Hesaplar',
                'Transferler',
              ], 'Hesaplar'),
            ),
            const SizedBox(height: AppSpacing.medium),
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
              ),
              child: AppCard(
                padding: EdgeInsets.zero,
                child: AppDividedColumn(
                  inset: AppIconCapsule.rowInset,
                  children: [
                    for (final (name, kind, balance, icon) in _rows)
                      AppRow(
                        onTap: () {},
                        leading: AppIconCapsule(
                          icon: icon,
                          tone: AppStatusTone.neutral,
                        ),
                        title: name,
                        subtitle: kind,
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            AppMoneyText(
                              amount: balance,
                              currency: 'TRY',
                              size: AppMoneySize.row,
                            ),
                            IconButton(
                              tooltip: 'Hesabı düzenle',
                              visualDensity: VisualDensity.compact,
                              onPressed: () {},
                              icon: Icon(
                                Icons.edit_outlined,
                                size: 20,
                                color: surfaces.inkMuted,
                              ),
                            ),
                          ],
                        ),
                      ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _Account extends StatelessWidget {
  const _Account({this.menu = false, this.direction = 'Tümü', this.category});

  final bool menu;
  final String direction;
  final String? category;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final filtered = category != null;
    Widget cell(String label, String amount, AppMoneyEffect? effect) =>
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(label, style: theme.textTheme.bodySmall),
              const SizedBox(height: AppSpacing.xxSmall),
              AppMoneyText(
                amount: amount,
                currency: 'TRY',
                size: AppMoneySize.row,
                effect: effect,
              ),
            ],
          ),
        );
    return Scaffold(
      body: SafeArea(
        child: Stack(
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                AppPageHeader(
                  title: 'Ziraat Vadesiz',
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
                          'Bakiye · bugün',
                          style: theme.textTheme.bodySmall,
                        ),
                        const SizedBox(height: AppSpacing.xxSmall),
                        const AppMoneyText(
                          amount: '13916.8650',
                          currency: 'TRY',
                          size: AppMoneySize.metric,
                        ),
                        const SizedBox(height: AppSpacing.small),
                        Divider(height: 1, color: surfaces.border),
                        const SizedBox(height: AppSpacing.small),
                        Row(
                          children: filtered
                              ? [
                                  cell(
                                    'Ekim · $category · çıkan',
                                    '6500.0000',
                                    AppMoneyEffect.expense,
                                  ),
                                ]
                              : [
                                  cell(
                                    'Ekim giren',
                                    '18400.0000',
                                    AppMoneyEffect.income,
                                  ),
                                  cell(
                                    'Ekim çıkan',
                                    '9250.0000',
                                    AppMoneyEffect.expense,
                                  ),
                                ],
                        ),
                      ],
                    ),
                  ),
                ),
                designChipRow([
                  for (final value in const ['Tümü', 'Giren', 'Çıkan'])
                    designChoice(value, selected: value == direction),
                  designDropChip('Kategori', value: category),
                ]),
                Expanded(
                  child: SingleChildScrollView(
                    child: DesignDays(
                      items: filtered
                          ? _items
                                .where((item) => item.categoryName == category)
                                .toList()
                          : _items,
                    ),
                  ),
                ),
              ],
            ),
            if (menu)
              const Positioned(
                right: AppSpacing.medium,
                top: 296,
                child: DesignMenu(
                  items: [
                    ('Hepsi', '', true),
                    ('İşyeri kirası', '6500.0000', false),
                    ('Ticari mal alımı', '4200.0000', false),
                    ('Elektrik, su, doğalgaz', '1860.0000', false),
                    ('Satış geliri', '10490.0000', false),
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
    '',
    '2026-10-05',
    ActivityKind.posDeposit,
    ActivityEffect.neutral,
    '2910.0000',
    destination: 'Ziraat Vadesiz',
  ),
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
    'Havale ile satış',
    '2026-10-03',
    ActivityKind.accountTransaction,
    ActivityEffect.income,
    '10490.0000',
    category: 'Satış geliri',
    source: 'Ziraat Vadesiz',
  ),
  designActivity(
    'Ekim kirası',
    '2026-10-01',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '6500.0000',
    category: 'İşyeri kirası',
    source: 'Ziraat Vadesiz',
  ),
  designActivity(
    'Elektrik faturası',
    '2026-10-01',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '1860.0000',
    category: 'Elektrik, su, doğalgaz',
    source: 'Ziraat Vadesiz',
  ),
];
