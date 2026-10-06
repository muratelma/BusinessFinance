import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_sections.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'design_sample_data.dart';
import 'screenshot_harness.dart';

/// Özet'teki `Yolda` satırı (Aşama 06.3 K5): beklenen gün gelmemişken ve
/// geçmişken.
void main() {
  Future<Widget> card(DateTime today) async {
    final report = await DesignDashboardSource().getAdvanced(2026, 9);
    return Scaffold(
      body: ListView(
        padding: const EdgeInsets.all(AppSpacing.large),
        children: [
          DashboardNetWorthCard(
            report: report,
            today: today,
            onOpenTransit: () {},
          ),
        ],
      ),
    );
  }

  testWidgets('yolda · gün gelmedi', (tester) async {
    final widget = await tester.runAsync(() => card(designToday));
    await captureScreen(tester, 'yolda-01', widget!, withNavBar: false);
  }, skip: !screenshotsEnabled);

  testWidgets('yolda · gün geçti', (tester) async {
    final widget = await tester.runAsync(() => card(DateTime(2026, 9, 29)));
    await captureScreen(tester, 'yolda-02-gecikti', widget!, withNavBar: false);
  }, skip: !screenshotsEnabled);
}
