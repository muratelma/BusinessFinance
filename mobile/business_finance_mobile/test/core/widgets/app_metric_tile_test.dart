import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_metric_tile.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';

import '../../helpers/contrast.dart';

void main() {
  Future<void> pump(WidgetTester tester, {required bool tinted}) =>
      tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: AppMetricTile(
              label: 'Gelir',
              amount: '18000.0000',
              currency: 'TRY',
              icon: Icons.south_west,
              effect: AppMoneyEffect.income,
              tinted: tinted,
            ),
          ),
        ),
      );

  Color moneyColor(WidgetTester tester) =>
      tester.widget<Text>(find.textContaining('18.000')).style!.color!;

  testWidgets('tintli kutu zeminini rolünün container tonundan alır', (
    tester,
  ) async {
    await pump(tester, tinted: true);

    final card = tester.widget<AppCard>(find.byType(AppCard));
    expect(card.background, AppFinanceColors.light.incomeContainer);
  });

  testWidgets('tintsiz kutu kart zemininde kalır', (tester) async {
    await pump(tester, tinted: false);

    final card = tester.widget<AppCard>(find.byType(AppCard));
    expect(card.background, isNull);
  });

  // Zemin değişince metin rengi de değişmek zorunda: yüzeye göre seçilmiş
  // `income` tonu gelir zemini üzerinde 3,95:1'e düşüyor ve AA eşiğini
  // geçemiyor. Bu test o regresyonu sayıyla yakalar.
  testWidgets('tintli kutuda tutar kendi zeminine karşı AA geçer', (
    tester,
  ) async {
    await pump(tester, tinted: true);

    final colors = AppFinanceColors.light;
    expect(moneyColor(tester), colors.onIncomeContainer);
    expect(
      contrastRatio(colors.onIncomeContainer, colors.incomeContainer),
      greaterThanOrEqualTo(wcagAaNormalText),
    );
  });

  testWidgets('tintsiz kutuda tutar yüzey tonunu kullanır', (tester) async {
    await pump(tester, tinted: false);

    expect(moneyColor(tester), AppFinanceColors.light.income);
  });
}
