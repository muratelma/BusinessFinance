import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/theme/app_surfaces.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_divided_column.dart';
import 'package:business_finance_mobile/core/widgets/app_icon_capsule.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:business_finance_mobile/core/widgets/app_month_picker.dart';
import 'package:business_finance_mobile/core/widgets/app_page_header.dart';
import 'package:business_finance_mobile/core/widgets/app_row.dart';
import 'package:business_finance_mobile/core/widgets/app_share_bar.dart';
import 'package:business_finance_mobile/core/widgets/app_status_chip.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';
import 'tasarim_ortak.dart';

/// Kategori ekranlarının **reddedilmiş iki ara turu** (5 Ekim 2026). Kayıt
/// olarak durur: neyin neden beğenilmediği devir notunda yazılıdır. Tur 1
/// alttan açılan panelleri ve birleşik sayfayı, tur 2 çift rayı, ok'lu ay
/// denetimini ve sekmeli (A) ile menülü (B) kategori sayfasını gösterir.
void main() {
  Future<void> shoot(
    WidgetTester tester,
    String name,
    Widget page, {
    Future<void> Function(WidgetTester tester)? before,
  }) => captureScreen(tester, name, page, withNavBar: false, before: before);

  // ------------------------------------------------------------ tur 1
  testWidgets('tur1 k1', (tester) async {
    await shoot(tester, 'tur1-k1-birlesik-kategoriler', const _T1Categories());
  }, skip: !screenshotsEnabled);
  testWidgets('tur1 k2', (tester) async {
    await shoot(
      tester,
      'tur1-k2-iki-sekme-tutarlar',
      const _T1Categories(tab: 0),
    );
  }, skip: !screenshotsEnabled);
  testWidgets('tur1 k3', (tester) async {
    await shoot(
      tester,
      'tur1-k3-iki-sekme-duzenle',
      const _T1Categories(tab: 1),
    );
  }, skip: !screenshotsEnabled);
  testWidgets('tur1 k4', (tester) async {
    await shoot(tester, 'tur1-k4-kategori-sayfasi', const _T1Category());
  }, skip: !screenshotsEnabled);
  testWidgets('tur1 k5', (tester) async {
    await shoot(
      tester,
      'tur1-k5-hesap-ve-kart-secici',
      const _T1Sheet(child: _T1SourcePicker()),
    );
  }, skip: !screenshotsEnabled);
  testWidgets('tur1 k6', (tester) async {
    await shoot(
      tester,
      'tur1-k6-kisi-secici',
      const _T1Sheet(child: _T1PersonPicker()),
    );
  }, skip: !screenshotsEnabled);
  testWidgets('tur1 k7', (tester) async {
    await shoot(
      tester,
      'tur1-k7-kategori-suzulmus',
      const _T1Category(source: 'Dükkan Kasası'),
    );
  }, skip: !screenshotsEnabled);
  testWidgets('tur1 k8', (tester) async {
    await shoot(
      tester,
      'tur1-k8-ay-secici',
      const _T1Category(),
      before: (tester) async => tester.tap(find.text('Ekim 2026')),
    );
  }, skip: !screenshotsEnabled);

  // ------------------------------------------------------------ tur 2
  testWidgets('tur2 01', (tester) async {
    await shoot(tester, 'tur2-01-diger-menusu', const _T2Menu());
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 02', (tester) async {
    await shoot(tester, 'tur2-02-kategori-ozeti', const _T2Summary());
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 03', (tester) async {
    await shoot(
      tester,
      'tur2-03-ay-secici',
      const DesignDialogOver(page: _T2Summary(), child: DesignMonthDialog()),
    );
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 A1', (tester) async {
    await shoot(tester, 'tur2-A1-sekmeli-hareketler', const _T2Tabbed(tab: 0));
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 A2', (tester) async {
    await shoot(tester, 'tur2-A2-sekmeli-hesaplar', const _T2Tabbed(tab: 1));
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 A3', (tester) async {
    await shoot(tester, 'tur2-A3-sekmeli-kisiler', const _T2Tabbed(tab: 2));
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 A4', (tester) async {
    await shoot(
      tester,
      'tur2-A4-sekmeli-hesap-secilmis',
      const _T2Tabbed(tab: 0, source: 'Dükkan Kasası'),
    );
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 B1', (tester) async {
    await shoot(tester, 'tur2-B1-menulu-kapali', const _T2Chipped());
  }, skip: !screenshotsEnabled);
  testWidgets('tur2 B2', (tester) async {
    await shoot(
      tester,
      'tur2-B2-menulu-hesap-menusu',
      const _T2Chipped(menu: true),
    );
  }, skip: !screenshotsEnabled);
}

// ================================================================ ortak

const _spent = [
  ('Ticari mal alımı', Icons.inventory_2_outlined, '12480.0000', 0.46),
  ('İşyeri kirası', Icons.storefront_outlined, '6500.0000', 0.24),
  ('Personel ücreti', Icons.badge_outlined, '4200.0000', 0.16),
  ('Elektrik, su, doğalgaz', Icons.bolt_outlined, '1860.0000', 0.07),
  ('Banka ve POS komisyonu', Icons.percent, '1140.0000', 0.04),
  ('Kasa farkı', Icons.difference_outlined, '150.0000', 0.01),
];

const _idle = [
  ('Nakliye ve kargo', Icons.local_shipping_outlined, false),
  ('Bakım-onarım', Icons.build_outlined, false),
  ('Reklam', Icons.campaign_outlined, false),
  ('Eski tedarik', Icons.archive_outlined, true),
];

Widget _spentRow(
  BuildContext context,
  (String, IconData, String, double) item,
) {
  final surfaces = AppSurfaces.of(context);
  final colors = AppFinanceColors.of(context);
  return AppRow(
    onTap: () {},
    leading: AppIconCapsule(icon: item.$2, tone: AppStatusTone.expense),
    title: item.$1,
    subtitle: 'Pay %${(item.$4 * 100).round()}',
    trailing: Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Column(
          crossAxisAlignment: CrossAxisAlignment.end,
          mainAxisSize: MainAxisSize.min,
          children: [
            AppMoneyText(
              amount: item.$3,
              currency: 'TRY',
              size: AppMoneySize.row,
            ),
            const SizedBox(height: AppSpacing.xSmall),
            SizedBox(
              width: 72,
              child: AppShareBar(ratio: item.$4 * 2, color: colors.expenseFill),
            ),
          ],
        ),
        Icon(Icons.chevron_right, color: surfaces.inkMuted),
      ],
    ),
  );
}

Widget _totalCard(BuildContext context, {String? note}) {
  final theme = Theme.of(context);
  return AppCard(
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Ekim gideri · İşletme', style: theme.textTheme.bodySmall),
        const SizedBox(height: AppSpacing.xxSmall),
        const AppMoneyText(
          amount: '26330.0000',
          currency: 'TRY',
          size: AppMoneySize.metric,
        ),
        if (note != null) ...[
          const SizedBox(height: AppSpacing.xSmall),
          Text(note, style: theme.textTheme.bodySmall),
        ],
      ],
    ),
  );
}

Widget _headCard(BuildContext context, {String? source}) {
  final theme = Theme.of(context);
  final filtered = source != null;
  return Padding(
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
            filtered ? 'Kategorinin tamamı ₺12.480,00' : 'Geçen ay ₺10.940,00',
            style: theme.textTheme.bodySmall,
          ),
        ],
      ),
    ),
  );
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

List<FinancialActivity> _itemsOf(String? source) => source == null
    ? _items
    : _items.where((item) => item.sourceName == source).toList();

// ================================================================ tur 1

/// Tur 1 — `Kategoriler`. [tab] boşsa birleşik sayfa; `0` iki sekmeli
/// seçeneğin `Tutarlar`, `1` `Düzenle` sekmesi.
class _T1Categories extends StatelessWidget {
  const _T1Categories({this.tab});

  final int? tab;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final manage = tab == 1;
    Widget idleRow((String, IconData, bool) item, {bool edit = false}) =>
        AppRow(
          onTap: () {},
          leading: AppIconCapsule(icon: item.$2, tone: AppStatusTone.cancelled),
          title: item.$1,
          subtitle: item.$3 ? 'Pasif' : null,
          trailing: Icon(
            edit ? Icons.edit_outlined : Icons.chevron_right,
            color: surfaces.inkMuted,
          ),
        );
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Kategoriler',
              onBack: () {},
              actions: [
                if (!manage) designMonthButton(),
                if (tab != 0)
                  IconButton(onPressed: () {}, icon: const Icon(Icons.add)),
              ],
            ),
            if (tab != null) ...[
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                ),
                child: designRail(context, const [
                  'Tutarlar',
                  'Düzenle',
                ], manage ? 'Düzenle' : 'Tutarlar'),
              ),
              const SizedBox(height: AppSpacing.small),
            ],
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
              ),
              child: designRail(context, const ['Gider', 'Gelir'], 'Gider'),
            ),
            const SizedBox(height: AppSpacing.small),
            if (!manage)
              designChipRow([
                designChoice('Hepsi'),
                designChoice('İşletme', selected: true),
                designChoice('Şahsi'),
                designDropChip('Tüm hesaplar ve kartlar'),
              ]),
            Expanded(
              child: ListView(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  0,
                  AppSpacing.medium,
                  AppSpacing.large,
                ),
                children: manage
                    ? [
                        AppCard(
                          padding: EdgeInsets.zero,
                          child: AppDividedColumn(
                            inset: AppIconCapsule.rowInset,
                            children: [
                              for (final item in _spent)
                                idleRow((item.$1, item.$2, false), edit: true),
                              for (final item in _idle)
                                idleRow(item, edit: true),
                            ],
                          ),
                        ),
                      ]
                    : [
                        Row(children: [Expanded(child: _totalCard(context))]),
                        const SizedBox(height: AppSpacing.medium),
                        AppCard(
                          padding: EdgeInsets.zero,
                          child: AppDividedColumn(
                            inset: AppIconCapsule.rowInset,
                            children: [
                              for (final item in _spent)
                                _spentRow(context, item),
                            ],
                          ),
                        ),
                        if (tab == null) ...[
                          Padding(
                            padding: const EdgeInsets.fromLTRB(
                              0,
                              AppSpacing.large,
                              0,
                              AppSpacing.small,
                            ),
                            child: Text(
                              'Bu ay hareketi yok',
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
                                for (final item in _idle) idleRow(item),
                              ],
                            ),
                          ),
                        ],
                      ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Tur 1 — kategorinin sayfası: kalem başlıkta, ay çip satırında.
class _T1Category extends StatelessWidget {
  const _T1Category({this.source});

  final String? source;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Ticari mal alımı',
            onBack: () {},
            actions: [
              IconButton(
                onPressed: () {},
                icon: const Icon(Icons.edit_outlined),
              ),
            ],
          ),
          _headCard(context, source: source),
          Builder(
            builder: (context) => designChipRow([
              ActionChip(
                avatar: const Icon(Icons.expand_more, size: 18),
                label: const Text('Ekim 2026'),
                onPressed: () => AppMonthPicker.show(
                  context: context,
                  initialMonth: DateTime(2026, 10),
                ),
              ),
              if (source != null)
                InputChip(
                  label: Text(source!),
                  selected: true,
                  onDeleted: () {},
                )
              else
                designDropChip('Tüm hesaplar ve kartlar'),
              designDropChip('Tüm kişiler'),
            ]),
          ),
          Expanded(
            child: SingleChildScrollView(
              child: DesignDays(items: _itemsOf(source)),
            ),
          ),
        ],
      ),
    ),
  );
}

/// Tur 1 — sayfanın altından açılan seçim paneli.
class _T1Sheet extends StatelessWidget {
  const _T1Sheet({required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) => Stack(
    children: [
      const _T1Category(),
      const Positioned.fill(child: ColoredBox(color: Colors.black54)),
      Align(
        alignment: Alignment.bottomCenter,
        child: Material(
          borderRadius: const BorderRadius.vertical(top: Radius.circular(28)),
          clipBehavior: Clip.antiAlias,
          child: SafeArea(top: false, child: child),
        ),
      ),
    ],
  );
}

Widget _pickRow(
  BuildContext context, {
  required IconData icon,
  required String title,
  String? subtitle,
  String? amount,
  bool selected = false,
  bool muted = false,
}) {
  final surfaces = AppSurfaces.of(context);
  return AppRow(
    onTap: () {},
    leading: AppIconCapsule(
      icon: icon,
      tone: muted ? AppStatusTone.cancelled : AppStatusTone.neutral,
    ),
    title: title,
    subtitle: subtitle,
    trailing: Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (amount != null)
          AppMoneyText(
            amount: amount,
            currency: 'TRY',
            size: AppMoneySize.row,
            style: TextStyle(color: surfaces.inkMuted),
          ),
        if (selected) ...[
          const SizedBox(width: AppSpacing.small),
          Icon(Icons.check, color: surfaces.ink),
        ],
      ],
    ),
  );
}

Widget _sheetBody(
  BuildContext context, {
  required String title,
  required String note,
  required List<Widget> children,
}) {
  final theme = Theme.of(context);
  final surfaces = AppSurfaces.of(context);
  return Padding(
    padding: const EdgeInsets.fromLTRB(
      AppSpacing.large,
      AppSpacing.large,
      AppSpacing.large,
      AppSpacing.medium,
    ),
    child: Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(title, style: theme.textTheme.titleLarge),
        const SizedBox(height: AppSpacing.xxSmall),
        Text(
          note,
          style: theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted),
        ),
        const SizedBox(height: AppSpacing.small),
        ...children,
      ],
    ),
  );
}

Widget _label(BuildContext context, String text) => Padding(
  padding: const EdgeInsets.only(top: AppSpacing.medium, bottom: 2),
  child: Text(
    text,
    style: Theme.of(context).textTheme.labelMedium?.copyWith(
      color: AppSurfaces.of(context).inkMuted,
      fontWeight: FontWeight.w600,
    ),
  ),
);

class _T1SourcePicker extends StatelessWidget {
  const _T1SourcePicker();

  @override
  Widget build(BuildContext context) => _sheetBody(
    context,
    title: 'Hesap ya da kart',
    note: 'Ticari mal alımı · Ekim 2026',
    children: [
      _pickRow(
        context,
        icon: Icons.all_inclusive,
        title: 'Tüm hesaplar ve kartlar',
        amount: '12480.0000',
        selected: true,
      ),
      _label(context, 'Hesaplar'),
      _pickRow(
        context,
        icon: Icons.storefront_outlined,
        title: 'Dükkan Kasası',
        subtitle: 'Nakit · 2 kayıt',
        amount: '5100.0000',
      ),
      _pickRow(
        context,
        icon: Icons.account_balance_outlined,
        title: 'Ziraat Vadesiz',
        subtitle: 'Banka · 1 kayıt',
        amount: '4200.0000',
      ),
      _label(context, 'Kredi kartları'),
      _pickRow(
        context,
        icon: Icons.credit_card,
        title: 'Bonus Kart',
        subtitle: '1 kayıt',
        amount: '3180.0000',
      ),
    ],
  );
}

class _T1PersonPicker extends StatelessWidget {
  const _T1PersonPicker();

  @override
  Widget build(BuildContext context) => _sheetBody(
    context,
    title: 'Kişi',
    note: 'Ticari mal alımı · Ekim 2026 · vadeli alımlar',
    children: [
      _pickRow(
        context,
        icon: Icons.all_inclusive,
        title: 'Tüm kişiler',
        amount: '12480.0000',
        selected: true,
      ),
      _label(context, 'Cariler'),
      _pickRow(
        context,
        icon: Icons.person_outline,
        title: 'Örnek Toptan Kağıt',
        subtitle: '1 kayıt',
        amount: '3180.0000',
      ),
      _pickRow(
        context,
        icon: Icons.person_outline,
        title: 'Test Toptancı',
        subtitle: '1 kayıt',
        amount: '2600.0000',
      ),
      _label(context, 'Diğer'),
      _pickRow(
        context,
        icon: Icons.person_off_outlined,
        title: 'Kişisiz kayıtlar',
        subtitle: 'Cariye bağlı olmayan giderler · 2 kayıt',
        amount: '6700.0000',
        muted: true,
      ),
    ],
  );
}

// ================================================================ tur 2

/// Tur 2 — başlıkta ok'lu ay denetimi.
Widget _arrowMonth(BuildContext context) => Row(
  mainAxisSize: MainAxisSize.min,
  children: [
    IconButton(
      visualDensity: VisualDensity.compact,
      onPressed: () {},
      icon: const Icon(Icons.chevron_left),
    ),
    Text('Ekim 2026', style: Theme.of(context).textTheme.titleSmall),
    IconButton(
      visualDensity: VisualDensity.compact,
      onPressed: () {},
      icon: const Icon(Icons.chevron_right),
    ),
  ],
);

class _T2Menu extends StatelessWidget {
  const _T2Menu();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    Widget group(String title, List<(IconData, String)> items) => Padding(
      padding: const EdgeInsets.only(bottom: AppSpacing.large),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(bottom: AppSpacing.small),
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
                  group('Planlama', const [
                    (Icons.donut_small_outlined, 'Bütçeler'),
                    (Icons.event_note_outlined, 'Yükümlülükler'),
                  ]),
                  group('Kategoriler', const [
                    (Icons.pie_chart_outline, 'Kategori özeti'),
                    (Icons.category_outlined, 'Kategori yönetimi'),
                  ]),
                  group('Ayarlar', const [
                    (Icons.notifications_active_outlined, 'Hatırlatmalar'),
                    (Icons.folder_outlined, 'Veri ve yedek'),
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

/// Tur 2 — `Kategori özeti`: üst üste iki ray, hesap süzgeci yok.
class _T2Summary extends StatelessWidget {
  const _T2Summary();

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Kategori özeti',
            onBack: () {},
            actions: [_arrowMonth(context)],
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: designRail(context, const ['Gider', 'Gelir'], 'Gider'),
          ),
          const SizedBox(height: AppSpacing.small),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: designRail(context, const [
              'Hepsi',
              'İşletme',
              'Şahsi',
            ], 'İşletme'),
          ),
          const SizedBox(height: AppSpacing.medium),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.medium,
                0,
                AppSpacing.medium,
                AppSpacing.large,
              ),
              children: [
                _totalCard(context, note: '6 kategori · geçen ay ₺24.180,00'),
                const SizedBox(height: AppSpacing.medium),
                AppCard(
                  padding: EdgeInsets.zero,
                  child: AppDividedColumn(
                    inset: AppIconCapsule.rowInset,
                    children: [
                      for (final item in _spent) _spentRow(context, item),
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

Widget _breakdownRow(
  BuildContext context, {
  required IconData icon,
  required String title,
  required String count,
  required String amount,
  required double share,
}) {
  final surfaces = AppSurfaces.of(context);
  final colors = AppFinanceColors.of(context);
  return AppRow(
    onTap: () {},
    leading: AppIconCapsule(icon: icon, tone: AppStatusTone.neutral),
    title: title,
    subtitle: count,
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
              child: AppShareBar(ratio: share, color: colors.expenseFill),
            ),
          ],
        ),
        Icon(Icons.chevron_right, color: surfaces.inkMuted),
      ],
    ),
  );
}

/// Tur 2, A — sayfa içi sekmeler: `Hareketler · Hesaplar · Kişiler`.
class _T2Tabbed extends StatelessWidget {
  const _T2Tabbed({required this.tab, this.source});

  final int tab;
  final String? source;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    const tabs = ['Hareketler', 'Hesaplar', 'Kişiler'];
    Widget card(List<Widget> rows) => Padding(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.medium,
        AppSpacing.medium,
        0,
      ),
      child: AppCard(
        padding: EdgeInsets.zero,
        child: AppDividedColumn(inset: AppIconCapsule.rowInset, children: rows),
      ),
    );
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Ticari mal alımı',
              onBack: () {},
              actions: [_arrowMonth(context)],
            ),
            _headCard(context, source: source),
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
              ),
              child: designRail(context, tabs, tabs[tab]),
            ),
            if (source != null)
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  AppSpacing.small,
                  AppSpacing.medium,
                  0,
                ),
                child: Align(
                  alignment: Alignment.centerLeft,
                  child: InputChip(
                    label: Text(source!),
                    selected: true,
                    onDeleted: () {},
                  ),
                ),
              ),
            Expanded(
              child: SingleChildScrollView(
                child: switch (tab) {
                  0 => DesignDays(items: _itemsOf(source)),
                  1 => card([
                    _breakdownRow(
                      context,
                      icon: Icons.storefront_outlined,
                      title: 'Dükkan Kasası',
                      count: '2 kayıt',
                      amount: '5100.0000',
                      share: 0.82,
                    ),
                    _breakdownRow(
                      context,
                      icon: Icons.account_balance_outlined,
                      title: 'Ziraat Vadesiz',
                      count: '1 kayıt',
                      amount: '4200.0000',
                      share: 0.67,
                    ),
                    _breakdownRow(
                      context,
                      icon: Icons.credit_card,
                      title: 'Bonus Kart',
                      count: '1 kayıt',
                      amount: '3180.0000',
                      share: 0.51,
                    ),
                  ]),
                  _ => Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      card([
                        _breakdownRow(
                          context,
                          icon: Icons.person_outline,
                          title: 'Örnek Toptan Kağıt',
                          count: '1 kayıt',
                          amount: '3180.0000',
                          share: 0.51,
                        ),
                        _breakdownRow(
                          context,
                          icon: Icons.person_outline,
                          title: 'Test Toptancı',
                          count: '1 kayıt',
                          amount: '2600.0000',
                          share: 0.42,
                        ),
                      ]),
                      Padding(
                        padding: const EdgeInsets.all(AppSpacing.medium),
                        child: Row(
                          children: [
                            Expanded(
                              child: Text(
                                'Bir kişiye bağlı olmayan giderler',
                                style: theme.textTheme.bodySmall?.copyWith(
                                  color: surfaces.inkMuted,
                                ),
                              ),
                            ),
                            AppMoneyText(
                              amount: '6700.0000',
                              currency: 'TRY',
                              size: AppMoneySize.row,
                              style: TextStyle(color: surfaces.inkMuted),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Tur 2, B — çipin altında açılan menü (ilk hâli; ok'lu ay ile).
class _T2Chipped extends StatelessWidget {
  const _T2Chipped({this.menu = false});

  final bool menu;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      child: Stack(
        children: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AppPageHeader(
                title: 'Ticari mal alımı',
                onBack: () {},
                actions: [_arrowMonth(context)],
              ),
              _headCard(context),
              designChipRow([
                designDropChip('Hesap ya da kart'),
                designDropChip('Kişi'),
              ]),
              Expanded(
                child: SingleChildScrollView(child: DesignDays(items: _items)),
              ),
            ],
          ),
          if (menu)
            const Positioned(
              left: AppSpacing.medium,
              top: 238,
              child: DesignMenu(
                items: [
                  ('Hepsi', '', true),
                  ('Dükkan Kasası', '5100.0000', false),
                  ('Ziraat Vadesiz', '4200.0000', false),
                  ('Bonus Kart', '3180.0000', false),
                ],
              ),
            ),
        ],
      ),
    ),
  );
}
