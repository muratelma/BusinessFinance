import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_breakpoints.dart';
import 'package:business_finance_mobile/core/widgets/app_content_width.dart';

void main() {
  Future<Size> pumpAt(WidgetTester tester, double width) async {
    tester.view.physicalSize = Size(width, 800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: AppContentWidth(
            child: Container(key: const ValueKey('content'), height: 100),
          ),
        ),
      ),
    );
    return tester.getSize(find.byKey(const ValueKey('content')));
  }

  testWidgets('telefon genişliğinde içeriği daraltmaz', (tester) async {
    expect((await pumpAt(tester, 400)).width, 400);
  });

  testWidgets('sınırın altındaki genişlikte şeffaftır', (tester) async {
    expect(
      (await pumpAt(tester, AppBreakpoints.contentMaxWidth)).width,
      AppBreakpoints.contentMaxWidth,
    );
  });

  testWidgets('geniş ekranda okunabilir genişlikle sınırlar', (tester) async {
    expect(
      (await pumpAt(tester, 1400)).width,
      AppBreakpoints.contentMaxWidth,
      reason:
          'Sınırsız genişlikte liste satırı ekran boyunca uzar ve okunmaz '
          'hale gelir.',
    );
  });

  testWidgets('sınırlanan içerik ortalanır', (tester) async {
    await pumpAt(tester, 1400);
    final center = tester.getCenter(find.byKey(const ValueKey('content')));
    expect(center.dx, closeTo(700, 0.5));
  });
}
