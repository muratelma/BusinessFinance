import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';
import 'package:business_finance_mobile/core/widgets/app_pick_field.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Panelde açılan yüzen menü panelin sabit eylem satırına taşmaz ve yarım
/// kalmaz: panel menüye yer açar (kullanıcı, 15 Kasım 2026: `Kart ödemesi`
/// panelinde hesap menüsü `Vazgeç` ve `Kaydet`in üstüne biniyordu).
void main() {
  const options = [
    AppAccountOption(id: 'c1', name: 'Kasa', kind: AppAccountKind.cash),
    AppAccountOption(id: 'c2', name: 'Cüzdan', kind: AppAccountKind.cash),
    AppAccountOption(id: 'c3', name: 'Çekmece', kind: AppAccountKind.cash),
    AppAccountOption(id: 'b1', name: 'Garanti', kind: AppAccountKind.bank),
    AppAccountOption(id: 'b2', name: 'Yapı Kredi', kind: AppAccountKind.bank),
    AppAccountOption(id: 'b3', name: 'Ziraat', kind: AppAccountKind.bank),
    AppAccountOption(id: 'k1', name: 'İş kartı', kind: AppAccountKind.card),
    AppAccountOption(id: 'k2', name: 'Ortak kart', kind: AppAccountKind.card),
  ];

  Future<void> openSheet(WidgetTester tester, Size size) async {
    tester.view.physicalSize = size;
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () => AppFormSheet.show<void>(
                context: context,
                builder: (_) => AppFormSheet<void>(
                  title: 'Kart ödemesi',
                  submitLabel: 'Kaydet',
                  onSubmit: () async {},
                  children: [
                    AppAccountField(
                      label: 'Ödeme hesabı',
                      options: options,
                      value: null,
                      onChanged: (_) {},
                    ),
                    const TextField(
                      decoration: InputDecoration(labelText: 'Tutar'),
                    ),
                  ],
                ),
              ),
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();
    await tester.tap(find.byType(AppAccountField));
    await tester.pumpAndSettle();
  }

  for (final size in const [Size(412, 915), Size(360, 740)]) {
    testWidgets('hesap menüsü eylem satırının üstünde biter (${size.width})', (
      tester,
    ) async {
      await openSheet(tester, size);

      final field = tester.getRect(find.byType(AppAccountField));
      final actions = tester.getRect(find.text('Kaydet'));
      final rows = find.byType(AppPickMenuRow);
      expect(rows, findsWidgets);
      final menu = tester.getRect(
        find.ancestor(of: rows.first, matching: find.byType(Material)).first,
      );
      // Menü alanın altından açılır, eylem satırına değmez.
      expect(menu.top, greaterThanOrEqualTo(field.top + 56));
      expect(menu.bottom, lessThanOrEqualTo(actions.top));
      // Menü kırpılmış bir kutu değil: en az üç hesap görünür.
      expect(menu.height, greaterThan(3 * 56));
      expect(tester.takeException(), isNull);
    });
  }

  // Başlığın yüksekliği panelden panele değişmez: yanında geri oku ya da
  // başka bir düğme olan başlık da düğmesiz başlıkla aynı yerde başlar.
  testWidgets('düğmeli panel başlığı düğmesiz başlıkla aynı yükseklikte', (
    tester,
  ) async {
    Future<double> titleTop({Widget? leading, Widget? trailing}) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: Align(
              alignment: Alignment.topCenter,
              child: AppFormSheet<void>(
                title: 'Başlık',
                submitLabel: 'Kaydet',
                onSubmit: () async {},
                leading: leading,
                trailing: trailing,
                children: const [SizedBox(height: 40)],
              ),
            ),
          ),
        ),
      );
      final title = tester.getRect(find.text('Başlık'));
      return title.center.dy - title.height / 2;
    }

    final plain = await titleTop();
    final withBack = await titleTop(
      leading: IconButton(onPressed: () {}, icon: const Icon(Icons.arrow_back)),
    );
    final withAction = await titleTop(
      trailing: IconButton(onPressed: () {}, icon: const Icon(Icons.close)),
    );
    expect(withBack, closeTo(plain, 2));
    expect(withAction, closeTo(plain, 2));
  });
}
