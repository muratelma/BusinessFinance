import 'package:business_finance_mobile/core/widgets/app_system_buttons_inset.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Test yüzeyi 800×600'dür.
  Future<EdgeInsets> viewPaddingBelow(
    WidgetTester tester, {
    required double system,
    double keyboard = 0,
  }) async {
    late EdgeInsets seen;
    await tester.pumpWidget(
      MaterialApp(
        home: MediaQuery(
          data: MediaQueryData(
            padding: EdgeInsets.only(bottom: keyboard > 0 ? 0 : system),
            viewPadding: EdgeInsets.only(bottom: system),
            viewInsets: EdgeInsets.only(bottom: keyboard),
          ),
          child: AppSystemButtonsInset(
            child: Builder(
              builder: (context) {
                seen = MediaQuery.viewPaddingOf(context);
                return const SizedBox.expand(key: Key('content'));
              },
            ),
          ),
        ),
      ),
    );
    return seen;
  }

  testWidgets('hareket çizgisinde içerik ekranın altına kadar iner', (
    tester,
  ) async {
    final seen = await viewPaddingBelow(tester, system: 24);

    expect(seen.bottom, 24);
    expect(tester.getSize(find.byKey(const Key('content'))).height, 600);
  });

  testWidgets('üç düğmeli gezinmede içerik düğmelerin üstünde biter', (
    tester,
  ) async {
    final seen = await viewPaddingBelow(tester, system: 48);

    // Sayfalar boşluğu sıfır görür: kendi son boşluklarını düğmelerin
    // üstünden uygularlar.
    expect(seen.bottom, 0);
    expect(tester.getSize(find.byKey(const Key('content'))).height, 552);
  });

  testWidgets('klavye açıkken düğmelerin payı ikinci kez eklenmez', (
    tester,
  ) async {
    await viewPaddingBelow(tester, system: 48, keyboard: 300);

    expect(tester.getSize(find.byKey(const Key('content'))).height, 600);
  });
}
