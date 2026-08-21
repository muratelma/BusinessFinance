import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_breakpoints.dart';
import 'package:business_finance_mobile/core/widgets/app_adaptive_sheet.dart';

void main() {
  Future<void> pumpAndOpen(WidgetTester tester, double width) async {
    tester.view.physicalSize = Size(width, 1000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () => AppAdaptiveSheet.show<String>(
                context: context,
                builder: (_) => const Text('panel içeriği'),
              ),
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();
  }

  /// Panel kabuğun iç Navigator'ında değil, kök Navigator'da açılır.
  ///
  /// `StatefulShellRoute` her sekmeye kendi Navigator'ını veriyor ve o Navigator
  /// kabuğun `Scaffold` gövdesinin içinde. Varsayılan `useRootNavigator: false`
  /// ile panel o iç katmanda çiziliyor, yani kayan eylem butonunun ve alt
  /// gezinme çubuğunun **altında** kalıyordu: cihazda işlem ayrıntısının altı
  /// artı butonunun arkasına giriyordu.
  testWidgets('panel iç Navigator'
      "'"
      'ın üstünde açılır', (tester) async {
    tester.view.physicalSize = const Size(400, 1000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final innerNavigator = GlobalKey<NavigatorState>();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          floatingActionButton: FloatingActionButton(
            onPressed: () {},
            child: const Icon(Icons.add),
          ),
          body: Navigator(
            key: innerNavigator,
            onGenerateRoute: (_) => MaterialPageRoute<void>(
              builder: (context) => TextButton(
                onPressed: () => AppAdaptiveSheet.show<void>(
                  context: context,
                  builder: (_) => const Text('panel içeriği'),
                ),
                child: const Text('aç'),
              ),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();

    expect(find.text('panel içeriği'), findsOneWidget);
    expect(
      find.descendant(
        of: find.byKey(innerNavigator),
        matching: find.text('panel içeriği'),
      ),
      findsNothing,
      reason:
          'panel iç Navigator'
          "'"
          'a çizilirse FAB onun üstünde kalır',
    );
  });

  testWidgets('telefon genişliğinde bottom sheet açar', (tester) async {
    await pumpAndOpen(tester, 400);

    expect(find.text('panel içeriği'), findsOneWidget);
    expect(find.byType(BottomSheet), findsOneWidget);
    expect(find.byType(Dialog), findsNothing);
  });

  testWidgets('geniş ekranda dialog açar', (tester) async {
    await pumpAndOpen(tester, 1000);

    expect(find.text('panel içeriği'), findsOneWidget);
    expect(find.byType(Dialog), findsOneWidget);
    expect(find.byType(BottomSheet), findsNothing);
  });

  testWidgets('medium sınıfı da dialog açar', (tester) async {
    await pumpAndOpen(tester, 700);

    expect(find.byType(Dialog), findsOneWidget);
    expect(find.byType(BottomSheet), findsNothing);
  });

  testWidgets('dialog içeriği okunabilir genişlikle sınırlanır', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1400, 1000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () => AppAdaptiveSheet.show<void>(
                context: context,
                // Tam genişliğe yayılmak isteyen içerik: sınır yoksa ekran
                // boyunca gerilir ve tek onay satırı 1400 dp yüzey kaplar.
                builder: (_) => const SizedBox(
                  key: ValueKey('greedy'),
                  width: double.infinity,
                  height: 120,
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

    final size = tester.getSize(find.byKey(const ValueKey('greedy')));
    expect(size.width, lessThanOrEqualTo(AppBreakpoints.contentMaxWidth));
  });

  testWidgets('her iki kademede de sonuç değeri geri döner', (tester) async {
    for (final width in [400.0, 1000.0]) {
      tester.view.physicalSize = Size(width, 1000);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);

      String? result;
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: Builder(
              builder: (context) => TextButton(
                onPressed: () async {
                  result = await AppAdaptiveSheet.show<String>(
                    context: context,
                    builder: (sheetContext) => TextButton(
                      onPressed: () =>
                          Navigator.of(sheetContext).pop('seçildi'),
                      child: const Text('seç'),
                    ),
                  );
                },
                child: const Text('aç'),
              ),
            ),
          ),
        ),
      );

      await tester.tap(find.text('aç'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('seç'));
      await tester.pumpAndSettle();

      expect(result, 'seçildi', reason: '$width dp genişlikte sonuç kayboldu');
    }
  });
}
