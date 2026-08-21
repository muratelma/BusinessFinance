import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_breakpoints.dart';

void main() {
  group('AppBreakpoints.sizeOfWidth', () {
    test('telefon dikey genişliği compact üretir', () {
      expect(AppBreakpoints.sizeOfWidth(360), AppWindowSize.compact);
      expect(AppBreakpoints.sizeOfWidth(599.9), AppWindowSize.compact);
    });

    test('eşik değerin kendisi bir üst sınıfa girer', () {
      expect(AppBreakpoints.sizeOfWidth(600), AppWindowSize.medium);
      expect(AppBreakpoints.sizeOfWidth(840), AppWindowSize.expanded);
    });

    test('telefon yatay ve küçük tablet medium üretir', () {
      expect(AppBreakpoints.sizeOfWidth(720), AppWindowSize.medium);
      expect(AppBreakpoints.sizeOfWidth(839.9), AppWindowSize.medium);
    });

    test('tablet ve üzeri expanded üretir', () {
      expect(AppBreakpoints.sizeOfWidth(1000), AppWindowSize.expanded);
    });
  });

  group('AppWindowSize yardımcıları', () {
    test('sınıf karşılaştırmaları doğru sırada', () {
      expect(AppWindowSize.compact.isCompact, isTrue);
      expect(AppWindowSize.compact.isAtLeastMedium, isFalse);
      expect(AppWindowSize.medium.isAtLeastMedium, isTrue);
      expect(AppWindowSize.medium.isExpanded, isFalse);
      expect(AppWindowSize.expanded.isAtLeastMedium, isTrue);
      expect(AppWindowSize.expanded.isExpanded, isTrue);
    });
  });

  group('context.windowSize', () {
    Future<AppWindowSize> sizeAt(WidgetTester tester, double width) async {
      late AppWindowSize captured;
      await tester.pumpWidget(
        MediaQuery(
          data: MediaQueryData(size: Size(width, 800)),
          child: Builder(
            builder: (context) {
              captured = context.windowSize;
              return const SizedBox.shrink();
            },
          ),
        ),
      );
      return captured;
    }

    testWidgets('genişliğe göre doğru sınıfı okur', (tester) async {
      expect(await sizeAt(tester, 400), AppWindowSize.compact);
      expect(await sizeAt(tester, 700), AppWindowSize.medium);
      expect(await sizeAt(tester, 1000), AppWindowSize.expanded);
    });
  });
}
