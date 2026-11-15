import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_filter_menu.dart';
import 'package:business_finance_mobile/core/widgets/app_filter_sheet.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Filtrenin kategori alanı: menü hareketle açılır, seçince ve dışarıya
/// dokununca kapanır; altında yer varsa aşağı açılır.
void main() {
  String? chosen;

  Widget page({double top = 80}) => MaterialApp(
    theme: AppTheme.light(),
    home: Scaffold(
      body: Padding(
        padding: EdgeInsets.only(top: top, left: 16, right: 16),
        child: Align(
          alignment: Alignment.topCenter,
          child: AppFilterSelectField(
            label: 'Bütün kategoriler',
            icon: Icons.layers_outlined,
            selected: null,
            onSelected: (key) => chosen = key,
            options: const [
              AppFilterOption(key: null, label: 'Hepsi'),
              AppFilterOption(key: 'a', label: 'Ticari mal alımı'),
              AppFilterOption(key: 'b', label: 'Faturalar'),
            ],
          ),
        ),
      ),
    ),
  );

  setUp(() => chosen = null);

  testWidgets('açılır, seçince kapanır ve seçimi bildirir', (tester) async {
    tester.view.physicalSize = const Size(412, 860);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(page());

    await tester.tap(find.text('Bütün kategoriler'));
    await tester.pump(const Duration(milliseconds: 60));
    // Açılış sürüyor: menü çiziliyor ama henüz tam görünür değil.
    expect(find.text('Faturalar'), findsOneWidget);
    await tester.pumpAndSettle();

    final field = tester.getRect(find.byType(InputDecorator));
    expect(
      tester.getRect(find.text('Faturalar')).top,
      greaterThan(field.bottom),
    );

    await tester.tap(find.text('Faturalar'));
    await tester.pumpAndSettle();
    expect(chosen, 'b');
    expect(find.text('Faturalar'), findsNothing);
  });

  testWidgets('dışarıya dokununca kapanır', (tester) async {
    tester.view.physicalSize = const Size(412, 860);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(page());
    await tester.tap(find.text('Bütün kategoriler'));
    await tester.pumpAndSettle();

    await tester.tapAt(const Offset(200, 820));
    await tester.pumpAndSettle();
    expect(find.text('Faturalar'), findsNothing);
    expect(chosen, isNull);
  });

  testWidgets('altında yer yoksa yukarı açılır', (tester) async {
    tester.view.physicalSize = const Size(412, 860);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(page(top: 760));
    await tester.tap(find.text('Bütün kategoriler'));
    await tester.pumpAndSettle();

    final field = tester.getRect(find.byType(InputDecorator));
    expect(tester.getRect(find.text('Faturalar')).bottom, lessThan(field.top));
  });
}
