import 'package:business_finance_mobile/core/formatters/date_text.dart';
import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/theme/app_surfaces.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_divided_column.dart';
import 'package:business_finance_mobile/core/widgets/app_icon_capsule.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:business_finance_mobile/core/widgets/app_page_header.dart';
import 'package:business_finance_mobile/core/widgets/app_row.dart';
import 'package:business_finance_mobile/core/widgets/app_segment_rail.dart';
import 'package:business_finance_mobile/core/widgets/app_share_bar.dart';
import 'package:business_finance_mobile/core/widgets/app_status_chip.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_tile.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Liste sisteminin (Aşama 06.3 Grup 8) karşılaştırma çizimleri. Gerçek
/// bileşenlerle kurulmuş **durağan** ekranlardır; uygulama koduna bağlı
/// değildir. Karar verilince bu dosya kalkar.
void main() {
  Future<void> shoot(WidgetTester tester, String name, Widget page) =>
      captureScreen(tester, name, page, withNavBar: false);

  testWidgets('1a hesap · kapı', (tester) async {
    await shoot(
      tester,
      '1a-hesap-kapi-islemler',
      _FilteredFeed(
        chip: 'Ziraat Vadesiz',
        income: '18400.0000',
        expense: '9250.0000',
        net: '9150.0000',
        items: _accountItems,
      ),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('1b hesap · kendi sayfası', (tester) async {
    await shoot(tester, '1b-hesap-kendi-sayfasi', const _AccountPage());
  }, skip: !screenshotsEnabled);

  testWidgets('2 kategori giderleri', (tester) async {
    await shoot(tester, '2-kategori-giderleri', const _CategoriesPage());
  }, skip: !screenshotsEnabled);

  testWidgets('3a kategori · kapı', (tester) async {
    await shoot(
      tester,
      '3a-kategori-kapi-islemler',
      _FilteredFeed(
        chip: 'Ticari mal alımı',
        expense: '12480.0000',
        items: _categoryItems,
      ),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('3b kategori · kendi sayfası', (tester) async {
    await shoot(tester, '3b-kategori-kendi-sayfasi', const _CategoryPage());
  }, skip: !screenshotsEnabled);

  testWidgets('4 süzgeç paneli', (tester) async {
    await shoot(tester, '4-suzgec-paneli', const _FilterPanel());
  }, skip: !screenshotsEnabled);
}

// ---------------------------------------------------------------- ortak

Widget _chips(List<Widget> children) => SingleChildScrollView(
  scrollDirection: Axis.horizontal,
  padding: const EdgeInsets.fromLTRB(
    AppSpacing.medium,
    AppSpacing.xSmall,
    AppSpacing.medium,
    AppSpacing.small,
  ),
  child: Row(
    children: [
      for (final child in children)
        Padding(
          padding: const EdgeInsets.only(right: AppSpacing.small),
          child: child,
        ),
    ],
  ),
);

Widget _menuChip(String label) =>
    Chip(avatar: const Icon(Icons.expand_more, size: 18), label: Text(label));

/// Süzülmüş listenin toplamı; tutarlar sunucudan gelir.
class _Totals extends StatelessWidget {
  const _Totals({this.income, this.expense, this.net});

  final String? income;
  final String? expense;
  final String? net;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
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
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        0,
        AppSpacing.medium,
        AppSpacing.small,
      ),
      child: AppCard(
        child: Row(
          children: [
            if (income != null) cell('Giren', income!, AppMoneyEffect.income),
            if (expense != null)
              cell('Çıkan', expense!, AppMoneyEffect.expense),
            if (net != null) cell('Net', net!, null),
          ],
        ),
      ),
    );
  }
}

/// Akışın gün gün dizilmiş satırları: İşlemler'deki satırın kendisi.
class _Days extends StatelessWidget {
  const _Days({required this.items});

  final List<FinancialActivity> items;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final days = <String, List<FinancialActivity>>{};
    for (final item in items) {
      days.putIfAbsent(item.activityDate, () => []).add(item);
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        for (final day in days.keys) ...[
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.small,
            ),
            child: Text(
              DateText.dayMonth(day),
              style: theme.textTheme.titleSmall?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: AppCard(
              padding: EdgeInsets.zero,
              child: AppDividedColumn(
                inset: AppIconCapsule.rowInset,
                children: [
                  for (final item in days[day]!)
                    ActivityTile(activity: item, showDate: false),
                ],
              ),
            ),
          ),
        ],
      ],
    );
  }
}

// ------------------------------------------------- A: kapı = İşlemler

/// "Kapı" yaklaşımı: her ekran İşlemler'in kendisini süzgeçli açar. Başlık
/// `İşlemler` kalır; süzgeç kalkabilir bir çiptir.
class _FilteredFeed extends StatelessWidget {
  const _FilteredFeed({
    required this.chip,
    required this.items,
    this.income,
    this.expense,
    this.net,
  });

  final String chip;
  final String? income;
  final String? expense;
  final String? net;
  final List<FinancialActivity> items;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'İşlemler',
            onBack: () {},
            actions: [
              IconButton(onPressed: () {}, icon: const Icon(Icons.search)),
              IconButton(onPressed: () {}, icon: const Icon(Icons.tune)),
            ],
          ),
          _chips([
            InputChip(label: Text(chip), onDeleted: () {}, selected: true),
            _menuChip('Ekim 2026'),
            ActionChip(
              avatar: const Icon(Icons.add, size: 18),
              label: const Text('Süzgeç ekle'),
              onPressed: () {},
            ),
          ]),
          _Totals(income: income, expense: expense, net: net),
          Expanded(
            child: SingleChildScrollView(child: _Days(items: items)),
          ),
        ],
      ),
    ),
  );
}

// ------------------------------------------- B: ekranın kendi sayfası

/// "Kendi sayfası" yaklaşımı: hesap kendi sorusunu (bakiye, bu ay giren ve
/// çıkan) başlıkta cevaplar; altında İşlemler'in **aynı satırları** gömülüdür.
class _AccountPage extends StatelessWidget {
  const _AccountPage();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Ziraat Vadesiz',
              onBack: () {},
              actions: [
                IconButton(
                  onPressed: () {},
                  icon: const Icon(Icons.edit_outlined),
                ),
              ],
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
                      'Bakiye · Banka · İşletme',
                      style: theme.textTheme.bodySmall,
                    ),
                    const SizedBox(height: AppSpacing.xxSmall),
                    const AppMoneyText(
                      amount: '13916.8650',
                      currency: 'TRY',
                      size: AppMoneySize.metric,
                    ),
                  ],
                ),
              ),
            ),
            _chips([
              _menuChip('Ekim 2026'),
              ChoiceChip(
                label: const Text('Tümü'),
                selected: true,
                onSelected: (_) {},
              ),
              ChoiceChip(
                label: const Text('Giren'),
                selected: false,
                onSelected: (_) {},
              ),
              ChoiceChip(
                label: const Text('Çıkan'),
                selected: false,
                onSelected: (_) {},
              ),
            ]),
            const _Totals(
              income: '18400.0000',
              expense: '9250.0000',
              net: '9150.0000',
            ),
            Expanded(
              child: SingleChildScrollView(child: _Days(items: _accountItems)),
            ),
          ],
        ),
      ),
    );
  }
}

/// Kategori giderleri: ayın kategorileri ve payları. Kategoriye özel
/// süzgeçler buradadır (gider/gelir, kapsam); satır kategoriyi açar.
class _CategoriesPage extends StatelessWidget {
  const _CategoriesPage();

  static const _rows = [
    ('Ticari mal alımı', Icons.inventory_2_outlined, '12480.0000', 0.46, null),
    ('İşyeri kirası', Icons.storefront_outlined, '6500.0000', 0.24, null),
    ('Personel ücreti', Icons.badge_outlined, '4200.0000', 0.16, null),
    ('Elektrik, su, doğalgaz', Icons.bolt_outlined, '1860.0000', 0.07, null),
    ('Banka ve POS komisyonu', Icons.percent, '1140.0000', 0.04, null),
    ('Kasa farkı', Icons.difference_outlined, '150.0000', 0.01, null),
  ];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Kategoriler',
              onBack: () {},
              actions: [
                TextButton.icon(
                  onPressed: () {},
                  icon: const Icon(Icons.expand_more),
                  iconAlignment: IconAlignment.end,
                  label: const Text('Ekim 2026'),
                ),
              ],
            ),
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
              ),
              child: AppSegmentRail<String>(
                values: const ['Gider', 'Gelir'],
                selected: 'Gider',
                semanticLabel: 'Tür',
                segmentLabel: (value) => value,
                onChanged: (_) {},
                segmentBuilder: (context, value, isSelected) => Text(
                  value,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w600,
                    color: isSelected ? surfaces.ink : surfaces.inkMuted,
                  ),
                ),
              ),
            ),
            const SizedBox(height: AppSpacing.small),
            _chips([
              ChoiceChip(
                label: const Text('Hepsi'),
                selected: false,
                onSelected: (_) {},
              ),
              ChoiceChip(
                label: const Text('İşletme'),
                selected: true,
                onSelected: (_) {},
              ),
              ChoiceChip(
                label: const Text('Şahsi'),
                selected: false,
                onSelected: (_) {},
              ),
              _menuChip('Tüm hesaplar'),
            ]),
            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                ),
                children: [
                  AppCard(
                    child: Row(
                      children: [
                        Expanded(
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
                            ],
                          ),
                        ),
                        Text('6 kategori', style: theme.textTheme.bodySmall),
                      ],
                    ),
                  ),
                  const SizedBox(height: AppSpacing.medium),
                  AppCard(
                    padding: EdgeInsets.zero,
                    child: AppDividedColumn(
                      inset: AppIconCapsule.rowInset,
                      children: [
                        for (final (name, icon, amount, share, budget) in _rows)
                          AppRow(
                            onTap: () {},
                            leading: AppIconCapsule(
                              icon: icon,
                              tone: AppStatusTone.expense,
                            ),
                            title: name,
                            subtitle: budget == null
                                ? 'Pay %${(share * 100).round()}'
                                : 'Pay %${(share * 100).round()} · '
                                      'bütçe $budget',
                            trailing: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Column(
                                  crossAxisAlignment: CrossAxisAlignment.end,
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    AppMoneyText(
                                      amount: amount,
                                      currency: 'TRY',
                                      size: AppMoneySize.row,
                                    ),
                                    const SizedBox(height: AppSpacing.xSmall),
                                    SizedBox(
                                      width: 72,
                                      child: AppShareBar(
                                        ratio: share * 2,
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
      ),
    );
  }
}

/// Kategorinin kendi sayfası: bu ay, geçen ay, bütçesi; altında aynı satırlar.
class _CategoryPage extends StatelessWidget {
  const _CategoryPage();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Ticari mal alımı',
              onBack: () {},
              actions: [
                TextButton.icon(
                  onPressed: () {},
                  icon: const Icon(Icons.expand_more),
                  iconAlignment: IconAlignment.end,
                  label: const Text('Ekim 2026'),
                ),
              ],
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
                    Text('Ekim gideri', style: theme.textTheme.bodySmall),
                    const SizedBox(height: AppSpacing.xxSmall),
                    const AppMoneyText(
                      amount: '12480.0000',
                      currency: 'TRY',
                      size: AppMoneySize.metric,
                      effect: AppMoneyEffect.expense,
                    ),
                    const SizedBox(height: AppSpacing.xSmall),
                    Text(
                      'Geçen ay ₺10.940,00',
                      style: theme.textTheme.bodySmall,
                    ),
                  ],
                ),
              ),
            ),
            _chips([
              _menuChip('Tüm hesaplar ve kartlar'),
              _menuChip('Tüm kişiler'),
            ]),
            Expanded(
              child: SingleChildScrollView(child: _Days(items: _categoryItems)),
            ),
          ],
        ),
      ),
    );
  }
}

/// Tek süzgeç paneli: İşlemler'in bugünkü panelinin genişlemiş hâli.
class _FilterPanel extends StatelessWidget {
  const _FilterPanel();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    Widget section(String title, List<Widget> chips) => Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const SizedBox(height: AppSpacing.medium),
        Text(title, style: theme.textTheme.titleSmall),
        const SizedBox(height: AppSpacing.small),
        Wrap(
          spacing: AppSpacing.small,
          runSpacing: AppSpacing.small,
          children: chips,
        ),
      ],
    );
    Widget chip(String label, {bool selected = false}) =>
        ChoiceChip(label: Text(label), selected: selected, onSelected: (_) {});
    Widget field(String label, String value) => Padding(
      padding: const EdgeInsets.only(top: AppSpacing.medium),
      child: InputDecorator(
        decoration: InputDecoration(
          labelText: label,
          floatingLabelBehavior: FloatingLabelBehavior.always,
          suffixIcon: const Icon(Icons.expand_more),
        ),
        child: Text(value),
      ),
    );
    return Scaffold(
      backgroundColor: Colors.black54,
      body: Align(
        alignment: Alignment.bottomCenter,
        child: Material(
          borderRadius: const BorderRadius.vertical(top: Radius.circular(28)),
          clipBehavior: Clip.antiAlias,
          child: SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(AppSpacing.large),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text('Süzgeç', style: theme.textTheme.titleLarge),
                  section('Dönem', [
                    chip('Ekim 2026', selected: true),
                    chip('Son 3 ay'),
                    chip('Bu yıl'),
                    chip('Tümü'),
                    chip('Özel aralık'),
                  ]),
                  field('Hesap ya da kart', 'Ziraat Vadesiz'),
                  field('Kategori', 'Tüm kategoriler'),
                  field('Kişi', 'Herkes'),
                  section('Tür', [
                    chip('Tümü', selected: true),
                    chip('Gelir'),
                    chip('Gider'),
                    chip('Transfer'),
                    chip('POS'),
                    chip('Borç'),
                  ]),
                  section('Kapsam', [
                    chip('Hepsi', selected: true),
                    chip('İşletme'),
                    chip('Şahsi'),
                  ]),
                  const SizedBox(height: AppSpacing.large),
                  Row(
                    children: [
                      TextButton(
                        onPressed: () {},
                        child: const Text('Temizle'),
                      ),
                      const Spacer(),
                      FilledButton(
                        onPressed: () {},
                        child: const Text('Uygula'),
                      ),
                    ],
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

// ---------------------------------------------------------------- veri

FinancialActivity _a(
  String title,
  String date,
  ActivityKind kind,
  ActivityEffect effect,
  String amount, {
  String? category,
  String? source,
  String? destination,
  ActivitySourceGroup group = ActivitySourceGroup.account,
}) => FinancialActivity(
  activityId: '$title$date',
  kind: kind,
  effect: effect,
  sourceGroup: group,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: date,
  amount: amount,
  currency: 'TRY',
  title: title,
  categoryName: category,
  sourceName: source,
  destinationName: destination,
  canCancel: true,
  supportsAttachments: false,
);

final _accountItems = [
  _a(
    '',
    '2026-10-05',
    ActivityKind.posDeposit,
    ActivityEffect.neutral,
    '2910.0000',
    destination: 'Ziraat Vadesiz',
  ),
  _a(
    'Toptancı ödemesi',
    '2026-10-05',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '4200.0000',
    category: 'Ticari mal alımı',
    source: 'Ziraat Vadesiz',
  ),
  _a(
    '',
    '2026-10-03',
    ActivityKind.transfer,
    ActivityEffect.neutral,
    '5000.0000',
    source: 'Dükkan Kasası',
    destination: 'Ziraat Vadesiz',
    group: ActivitySourceGroup.transfer,
  ),
  _a(
    'Ekim kirası',
    '2026-10-01',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '6500.0000',
    category: 'İşyeri kirası',
    source: 'Ziraat Vadesiz',
  ),
  _a(
    'Elektrik faturası',
    '2026-10-01',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '1860.0000',
    category: 'Elektrik, su, doğalgaz',
    source: 'Ziraat Vadesiz',
  ),
];

final _categoryItems = [
  _a(
    'Toptancı ödemesi',
    '2026-10-05',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '4200.0000',
    category: 'Ticari mal alımı',
    source: 'Ziraat Vadesiz',
  ),
  _a(
    'Kırtasiye toptan',
    '2026-10-04',
    ActivityKind.cardCharge,
    ActivityEffect.expense,
    '3180.0000',
    category: 'Ticari mal alımı',
    source: 'Bonus Kart',
    group: ActivitySourceGroup.creditCard,
  ),
  _a(
    'Hal alışverişi',
    '2026-10-02',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '2600.0000',
    category: 'Ticari mal alımı',
    source: 'Dükkan Kasası',
  ),
  _a(
    'Ambalaj',
    '2026-10-01',
    ActivityKind.accountTransaction,
    ActivityEffect.expense,
    '2500.0000',
    category: 'Ticari mal alımı',
    source: 'Dükkan Kasası',
  ),
];
