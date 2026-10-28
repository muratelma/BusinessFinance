import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_category_field.dart';
import 'package:business_finance_mobile/core/widgets/app_category_icon.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../../support/category_field_helpers.dart';

const _business = TransactionScope.business;
const _personal = TransactionScope.personal;

/// Gider ekle teslimindeki liste: dokuzdan fazla kategori, üç grup.
const _many = [
  AppCategoryOption(id: 'goods', name: 'Ticari mal alımı', scope: _business),
  AppCategoryOption(id: 'bills', name: 'İşyeri faturaları', scope: _business),
  AppCategoryOption(id: 'fuel', name: 'Araç ve yakıt', scope: _business),
  AppCategoryOption(id: 'rent', name: 'İşyeri kirası', scope: _business),
  AppCategoryOption(id: 'home', name: 'Ev faturaları', scope: _personal),
  AppCategoryOption(id: 'market', name: 'Market', scope: _personal),
  AppCategoryOption(id: 'health', name: 'Sağlık', scope: _personal),
  AppCategoryOption(id: 'interest', name: 'Faiz ve finansman gideri'),
  AppCategoryOption(id: 'other', name: 'Diğer gider'),
];

/// Gelir ekle: az kategori, arama ve son kullanılanlar yok.
const _few = [
  AppCategoryOption(id: 'sales', name: 'Satış geliri', scope: _business),
  AppCategoryOption(id: 'rentIn', name: 'Kira geliri', scope: _personal),
  AppCategoryOption(id: 'faiz', name: 'Faiz geliri'),
];

void main() {
  testWidgets('kalabalık listede arama, son kullanılanlar ve üç grup çıkar', (
    tester,
  ) async {
    await _pump(
      tester,
      options: _many,
      recent: const [
        AppCategoryValue('goods', _business),
        AppCategoryValue('interest', _personal),
        AppCategoryValue('gone', _business),
      ],
    );
    await tester.tap(categoryField());
    await tester.pumpAndSettle();

    expect(find.text('Kategori ara'), findsOneWidget);
    expect(find.text('Son kullanılanlar'), findsOneWidget);
    for (final group in ['İşletme', 'Şahsi', 'İşletme veya şahsi']) {
      expect(find.text(group), findsWidgets);
    }
    // Listede olmayan kategori son kullanılanlarda da çizilmez; iki kapsama
    // açık kategori kapsamıyla durur.
    expect(find.text('Faiz ve finansman gideri'), findsNWidgets(2));
  });

  testWidgets('son kullanılan satır kategoriyi ve kapsamı birden seçer', (
    tester,
  ) async {
    final chosen = <AppCategoryValue?>[];
    await _pump(
      tester,
      options: _many,
      recent: const [AppCategoryValue('interest', _personal)],
      onChanged: chosen.add,
    );
    await tester.tap(categoryField());
    await tester.pumpAndSettle();
    await tester.tap(find.text('Faiz ve finansman gideri').first);
    await tester.pumpAndSettle();

    expect(chosen.single, const AppCategoryValue('interest', _personal));
    expect(find.text('Kategori ara'), findsNothing);
  });

  testWidgets('az kategoride arama ve son kullanılanlar yok', (tester) async {
    await _pump(
      tester,
      options: _few,
      recent: const [AppCategoryValue('sales', _business)],
    );
    await tester.tap(categoryField());
    await tester.pumpAndSettle();

    expect(find.text('Kategori ara'), findsNothing);
    expect(find.text('Son kullanılanlar'), findsNothing);
  });

  testWidgets('iki kapsama açık kategori açılır, kapsam satırı seçer', (
    tester,
  ) async {
    final chosen = <AppCategoryValue?>[];
    await _pump(tester, options: _few, onChanged: chosen.add);
    await tester.tap(categoryField());
    await tester.pumpAndSettle();
    await tester.tap(categoryMenuRow('Faiz geliri'));
    await tester.pumpAndSettle();

    expect(chosen, isEmpty);
    expect(find.byIcon(Icons.expand_less), findsOneWidget);
    await tester.tap(categoryMenuRow('İşletme'));
    await tester.pumpAndSettle();

    expect(chosen.single, const AppCategoryValue('faiz', _business));
  });

  testWidgets('arama Türkçe harfleri tanır; eşleşme yoksa ekleme önerir', (
    tester,
  ) async {
    final added = <String>[];
    await _pump(tester, options: _many, onAdd: added.add);
    await tester.tap(categoryField());
    await tester.pumpAndSettle();

    await tester.enterText(find.byType(TextField), 'İŞYERİ');
    await tester.pumpAndSettle();
    expect(categoryMenuRow('İşyeri faturaları'), findsOneWidget);
    expect(find.text('Market'), findsNothing);

    await tester.enterText(find.byType(TextField), 'kırtasiye');
    await tester.pumpAndSettle();
    expect(find.text('Bu adla kategori yok'), findsOneWidget);
    await tester.tap(find.text('"kırtasiye" ekle'));
    await tester.pumpAndSettle();

    expect(added, ['kırtasiye']);
  });

  testWidgets('sabit kapsamlı ekranda yalnız o kapsam ve açık kategoriler', (
    tester,
  ) async {
    final chosen = <AppCategoryValue?>[];
    await _pump(
      tester,
      options: _few,
      fixedScope: _business,
      onChanged: chosen.add,
    );
    await tester.tap(categoryField());
    await tester.pumpAndSettle();

    expect(find.text('Kira geliri'), findsNothing);
    expect(find.byIcon(Icons.expand_more), findsNothing);
    await tester.tap(categoryMenuRow('Faiz geliri'));
    await tester.pumpAndSettle();

    expect(chosen.single, const AppCategoryValue('faiz', _business));
  });

  testWidgets('kapsamı görmeyen kullanıcıda düz liste, kapsam yazılmaz', (
    tester,
  ) async {
    final chosen = <AppCategoryValue?>[];
    await _pump(tester, options: _few, showScope: false, onChanged: chosen.add);
    await tester.tap(categoryField());
    await tester.pumpAndSettle();

    expect(find.text('İşletme veya şahsi'), findsNothing);
    await tester.tap(categoryMenuRow('Faiz geliri'));
    await tester.pumpAndSettle();

    expect(chosen.single, const AppCategoryValue('faiz'));
  });

  testWidgets('geri tuşu yalnız menüyü kapatır', (tester) async {
    await _pump(tester, options: _few);
    await tester.tap(categoryField());
    await tester.pumpAndSettle();
    expect(find.text('İşletme veya şahsi'), findsOneWidget);

    final popped = await tester.binding.handlePopRoute();
    await tester.pumpAndSettle();

    expect(popped, isTrue);
    expect(find.text('İşletme veya şahsi'), findsNothing);
    expect(categoryField(), findsOneWidget);
  });

  testWidgets('menünün dışına dokunmak seçmeden kapatır', (tester) async {
    final chosen = <AppCategoryValue?>[];
    await _pump(tester, options: _few, onChanged: chosen.add);
    await tester.tap(categoryField());
    await tester.pumpAndSettle();
    expect(find.text('İşletme veya şahsi'), findsOneWidget);

    // Menünün altındaki boş yer de dışarıdır.
    final menuBottom = tester.getBottomLeft(find.text('Faiz geliri')).dy;
    await tester.tapAt(Offset(200, menuBottom + 120));
    await tester.pumpAndSettle();

    expect(find.text('İşletme veya şahsi'), findsNothing);
    expect(chosen, isEmpty);
  });

  testWidgets('hiç kategori yoksa menü yerine ekleme kartı açılır', (
    tester,
  ) async {
    final added = <String>[];
    await _pump(tester, options: const [], onAdd: added.add);
    await tester.tap(categoryField());
    await tester.pumpAndSettle();

    expect(find.text('Henüz kategori yok'), findsOneWidget);
    await tester.tap(find.text('Kategori ekle'));
    await tester.pumpAndSettle();

    expect(added, ['']);
  });

  testWidgets('panelde gövde alan + menü kadar boylanır, kapanınca döner', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(412, 892);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Builder(
          builder: (context) => Scaffold(
            body: Center(
              child: TextButton(
                onPressed: () => AppFormSheet.show<bool>(
                  context: context,
                  builder: (_) => AppFormSheet<bool>(
                    title: 'Panel',
                    submitLabel: 'Kaydet',
                    onSubmit: () async => true,
                    children: [
                      for (final label in ['A', 'B', 'C', 'D', 'E', 'F'])
                        Padding(
                          padding: const EdgeInsets.only(bottom: 16),
                          child: TextField(
                            decoration: InputDecoration(labelText: label),
                          ),
                        ),
                      AppCategoryField(
                        options: _few,
                        value: null,
                        fixedScope: _business,
                        onChanged: (_) {},
                      ),
                      for (final label in ['G', 'H', 'I'])
                        Padding(
                          padding: const EdgeInsets.only(top: 16),
                          child: TextField(
                            decoration: InputDecoration(labelText: label),
                          ),
                        ),
                    ],
                  ),
                ),
                child: const Text('Aç'),
              ),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('Aç'));
    await tester.pumpAndSettle();
    final before = tester.getSize(find.byType(AppFormSheet<bool>)).height;

    await tester.tap(categoryField());
    await tester.pumpAndSettle();
    final open = tester.getSize(find.byType(AppFormSheet<bool>)).height;
    // Alan gövdenin başında, menünün altı boş değil.
    expect(open, lessThan(before));
    final menuBottom = tester.getBottomLeft(find.text('Faiz geliri')).dy;
    final actions = tester.getTopLeft(find.text('Kaydet')).dy;
    expect(actions - menuBottom, lessThan(80));

    await tester.tapAt(const Offset(200, 40));
    await tester.pumpAndSettle();
    expect(
      tester.getSize(find.byType(AppFormSheet<bool>)).height,
      closeTo(before, 1),
    );
  });

  test('simge adın yazımına bakmaz, bilinmeyen ad etiket simgesi alır', () {
    expect(categoryIcon('İŞYERİ FATURALARI'), Icons.receipt_long_outlined);
    expect(categoryIcon('işyeri-faturaları.'), Icons.receipt_long_outlined);
    expect(categoryIcon('Kırtasiye'), Icons.local_offer_outlined);
  });
}

Future<void> _pump(
  WidgetTester tester, {
  required List<AppCategoryOption> options,
  List<AppCategoryValue> recent = const [],
  TransactionScope? fixedScope,
  bool showScope = true,
  ValueChanged<AppCategoryValue?>? onChanged,
  ValueChanged<String>? onAdd,
}) async {
  tester.view.physicalSize = const Size(412, 1200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  AppCategoryValue? value;
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(
        body: StatefulBuilder(
          builder: (context, setState) => ListView(
            padding: const EdgeInsets.all(16),
            children: [
              const TextField(decoration: InputDecoration(labelText: 'Hesap')),
              const SizedBox(height: 16),
              AppCategoryField(
                options: options,
                value: value,
                recent: recent,
                fixedScope: fixedScope,
                showScope: showScope,
                onAdd: onAdd,
                onChanged: (next) {
                  onChanged?.call(next);
                  setState(() => value = next);
                },
              ),
            ],
          ),
        ),
      ),
    ),
  );
  await tester.pumpAndSettle();
}
