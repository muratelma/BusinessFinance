import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/widgets/app_responsive_grid.dart';

void main() {
  group('AppResponsiveGrid.columnCountFor', () {
    int columns(double width) => AppResponsiveGrid.columnCountFor(
      maxWidth: width,
      minItemWidth: 320,
      spacing: 16,
    );

    test('telefon genişliğinde tek sütun', () {
      expect(columns(360), 1);
      expect(columns(600), 1);
    });

    test('iki kart sığdığı anda iki sütuna geçer', () {
      // 320 + 16 + 320 = 656
      expect(columns(655), 1);
      expect(columns(656), 2);
    });

    test('üç kart sığan ekranda ikide takılı kalmaz', () {
      // Eski elle hesap 700 dp üstünde her zaman iki sütunda kalıyordu.
      expect(columns(1008), 3);
    });

    test('kart genişliğinden dar alanda bile bir sütun kalır', () {
      expect(columns(100), 1);
      expect(columns(0), 1);
    });
  });

  group('metin ölçeği', () {
    // Pixel 8 manuel kabulünde bulundu: Özet kartları sabit 180 dp
    // genişlikteydi ve 2.0x ölçekte tutar satır ortasından bölünüyordu
    // (`₺20.000,0` / `0`). Kartın okunabilir en küçük genişliği metinle
    // birlikte büyümeli.
    testWidgets('büyük yazı ölçeğinde sütun sayısı azalır', (tester) async {
      Future<int> columnsAt(double scale) async {
        tester.view.physicalSize = const Size(400, 1200);
        tester.view.devicePixelRatio = 1;
        addTearDown(tester.view.reset);
        await tester.pumpWidget(
          MaterialApp(
            theme: AppTheme.light(),
            home: MediaQuery(
              data: MediaQueryData(textScaler: TextScaler.linear(scale)),
              child: Scaffold(
                body: AppResponsiveGrid(
                  minItemWidth: 180,
                  children: [
                    for (var i = 0; i < 3; i++)
                      SizedBox(key: ValueKey('item$i'), height: 40),
                  ],
                ),
              ),
            ),
          ),
        );
        final width = tester.getSize(find.byKey(const ValueKey('item0'))).width;
        return (400 / width).round();
      }

      expect(await columnsAt(1), 2);
      expect(
        await columnsAt(2),
        1,
        reason:
            '2.0x ölçekte 180 dp kart 360 dp ister; iki sütunda kalırsa '
            'tutar bölünür.',
      );
    });
  });

  group('AppResponsiveGrid yerleşimi', () {
    Future<void> pumpAt(WidgetTester tester, double width) async {
      tester.view.physicalSize = Size(width, 1200);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: AppResponsiveGrid(
              children: [
                for (var i = 0; i < 3; i++)
                  SizedBox(key: ValueKey('item$i'), height: 80),
              ],
            ),
          ),
        ),
      );
    }

    testWidgets('dar ekranda kartlar tam genişliği alır', (tester) async {
      await pumpAt(tester, 400);
      final width = tester.getSize(find.byKey(const ValueKey('item0'))).width;
      expect(width, 400);
    });

    testWidgets('geniş ekranda kartlar aralık payıyla bölünür', (tester) async {
      await pumpAt(tester, 800);
      final width = tester.getSize(find.byKey(const ValueKey('item0'))).width;
      expect(width, (800 - 16) / 2);
    });

    testWidgets('boş liste hiçbir şey çizmez', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(body: AppResponsiveGrid(children: [])),
        ),
      );
      expect(find.byType(Wrap), findsNothing);
    });
  });
}
