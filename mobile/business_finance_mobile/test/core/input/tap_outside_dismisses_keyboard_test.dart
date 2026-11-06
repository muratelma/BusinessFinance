import 'package:business_finance_mobile/core/input/tap_outside_dismisses_keyboard.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  late FocusNode first;
  late FocusNode second;
  late int pressed;

  setUp(() {
    first = FocusNode();
    second = FocusNode();
    pressed = 0;
  });

  tearDown(() {
    first.dispose();
    second.dispose();
  });

  Widget harness({bool wrapped = true}) => MaterialApp(
    builder: wrapped
        ? (context, child) => TapOutsideDismissesKeyboard(child: child!)
        : null,
    home: Scaffold(
      body: ListView(
        children: [
          TextField(key: const Key('first'), focusNode: first),
          const SizedBox(height: 120, key: Key('blank')),
          TextField(key: const Key('second'), focusNode: second),
          Center(
            child: TextButton(
              onPressed: () => pressed++,
              child: const Text('Kaydet'),
            ),
          ),
          const SizedBox(height: 1200),
        ],
      ),
    ),
  );

  Future<void> focusFirst(WidgetTester tester) async {
    await tester.tap(find.byKey(const Key('first')));
    await tester.pump();
    expect(first.hasFocus, isTrue);
  }

  testWidgets(
    'a tap on empty space closes the keyboard',
    (tester) async {
      await tester.pumpWidget(harness());
      await focusFirst(tester);

      await tester.tapAt(tester.getCenter(find.byKey(const Key('blank'))));
      await tester.pump();

      expect(first.hasFocus, isFalse);
    },
    variant: TargetPlatformVariant.mobile(),
  );

  testWidgets('scrolling keeps the keyboard open', (tester) async {
    await tester.pumpWidget(harness());
    await focusFirst(tester);

    await tester.dragFrom(
      tester.getCenter(find.byKey(const Key('blank'))),
      const Offset(0, -200),
    );
    await tester.pump();

    expect(first.hasFocus, isTrue);
  }, variant: TargetPlatformVariant.mobile());

  testWidgets(
    'a tap on another field moves the focus there',
    (tester) async {
      await tester.pumpWidget(harness());
      await focusFirst(tester);

      await tester.tap(find.byKey(const Key('second')));
      await tester.pump();

      expect(first.hasFocus, isFalse);
      expect(second.hasFocus, isTrue);
    },
    variant: TargetPlatformVariant.mobile(),
  );

  testWidgets(
    'a tap on a button presses it and closes the keyboard',
    (tester) async {
      await tester.pumpWidget(harness());
      await focusFirst(tester);

      await tester.tap(find.text('Kaydet'));
      await tester.pump();

      expect(pressed, 1);
      expect(first.hasFocus, isFalse);
    },
    variant: TargetPlatformVariant.mobile(),
  );

  testWidgets(
    'without the wrapper a phone keeps the keyboard open',
    (tester) async {
      await tester.pumpWidget(harness(wrapped: false));
      await focusFirst(tester);

      await tester.tapAt(tester.getCenter(find.byKey(const Key('blank'))));
      await tester.pump();

      expect(first.hasFocus, isTrue);
    },
    variant: TargetPlatformVariant.mobile(),
  );
}
