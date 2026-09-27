import 'package:business_finance_mobile/features/activities/presentation/quick_add_launcher.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

void main() {
  testWidgets('10 işlem ekle', (tester) async {
    await captureScreen(
      tester,
      '10-islem-ekle',
      Builder(
        builder: (context) => Scaffold(
          body: Center(
            child: TextButton(
              onPressed: () => QuickAddLauncher.show(context),
              child: const Text('aç'),
            ),
          ),
        ),
      ),
      before: (tester) async => tester.tap(find.text('aç')),
    );
  }, skip: !screenshotsEnabled);
}
