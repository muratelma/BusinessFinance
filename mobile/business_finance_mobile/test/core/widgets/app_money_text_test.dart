import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';

void main() {
  Future<Text> pump(
    WidgetTester tester,
    AppMoneyText widget, {
    Brightness brightness = Brightness.light,
  }) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: brightness == Brightness.dark
            ? AppTheme.dark()
            : AppTheme.light(),
        home: Scaffold(body: widget),
      ),
    );
    return tester.widget<Text>(find.byType(Text));
  }

  group('görünen metin', () {
    testWidgets('işaretsiz kullanımda tutarı olduğu gibi biçimler', (
      tester,
    ) async {
      final text = await pump(
        tester,
        const AppMoneyText(amount: '1234.5600', currency: 'TRY'),
      );
      expect(text.data, '₺1.234,56');
    });

    testWidgets('işaretli kullanımda gelire + gidere - koyar', (tester) async {
      final income = await pump(
        tester,
        const AppMoneyText(
          amount: '100.0000',
          currency: 'TRY',
          effect: AppMoneyEffect.income,
          signed: true,
        ),
      );
      expect(income.data, '+₺100,00');

      final expense = await pump(
        tester,
        const AppMoneyText(
          amount: '100.0000',
          currency: 'TRY',
          effect: AppMoneyEffect.expense,
          signed: true,
        ),
      );
      expect(expense.data, '-₺100,00');
    });

    testWidgets('nötr hareket işaret almaz', (tester) async {
      final text = await pump(
        tester,
        const AppMoneyText(
          amount: '100.0000',
          currency: 'TRY',
          effect: AppMoneyEffect.neutral,
          signed: true,
        ),
      );
      // Transfer ne kazanç ne kayıptır; işaret koymak onu gelir/gidere benzetir.
      expect(text.data, '₺100,00');
    });
  });

  group('renk', () {
    testWidgets('her etki kendi finans token rengini alır', (tester) async {
      for (final entry in {
        AppMoneyEffect.income: AppFinanceColors.light.income,
        AppMoneyEffect.expense: AppFinanceColors.light.expense,
        AppMoneyEffect.neutral: AppFinanceColors.light.neutral,
      }.entries) {
        final text = await pump(
          tester,
          AppMoneyText(amount: '1.0000', currency: 'TRY', effect: entry.key),
        );
        expect(text.style?.color, entry.value);
      }
    });

    testWidgets('karanlık temada karanlık paletten okur', (tester) async {
      final text = await pump(
        tester,
        const AppMoneyText(
          amount: '1.0000',
          currency: 'TRY',
          effect: AppMoneyEffect.income,
        ),
        brightness: Brightness.dark,
      );
      expect(text.style?.color, AppFinanceColors.dark.income);
      expect(text.style?.color, isNot(AppFinanceColors.light.income));
    });

    testWidgets('iptal edilmiş tutar etkiden bağımsız nötrleşir', (
      tester,
    ) async {
      final text = await pump(
        tester,
        const AppMoneyText(
          amount: '1.0000',
          currency: 'TRY',
          effect: AppMoneyEffect.income,
          isCancelled: true,
        ),
      );
      expect(text.style?.color, AppFinanceColors.light.cancelled);
    });

    testWidgets('etki verilmezse varsayılan metin rengi kalır', (tester) async {
      final text = await pump(
        tester,
        const AppMoneyText(amount: '1.0000', currency: 'TRY'),
      );
      expect(text.style?.color, AppTheme.light().colorScheme.onSurface);
    });
  });

  testWidgets('sabit genişlikli rakam kullanır', (tester) async {
    final text = await pump(
      tester,
      const AppMoneyText(amount: '1.0000', currency: 'TRY'),
    );
    expect(
      text.style?.fontFeatures,
      contains(const FontFeature.tabularFigures()),
      reason: 'Orantılı rakamlarda tutar sütunu kayar ve karşılaştırılamaz.',
    );
  });

  group('ekran okuyucu', () {
    testWidgets('para simgesi yerine okunabilir cümle verir', (tester) async {
      final text = await pump(
        tester,
        const AppMoneyText(
          amount: '1234.5600',
          currency: 'TRY',
          effect: AppMoneyEffect.expense,
          signed: true,
        ),
      );
      // Görünen metin `-₺1.234,56`; ekran okuyucu bunu glif glif okurdu.
      expect(text.semanticsLabel, '1.234,56 lira gider');
    });

    testWidgets('bilinmeyen para birimi kodu olduğu gibi okunur', (
      tester,
    ) async {
      final text = await pump(
        tester,
        const AppMoneyText(amount: '5.0000', currency: 'USD'),
      );
      expect(text.semanticsLabel, '5,00 USD');
    });

    testWidgets('ek bağlam cümlenin sonuna eklenir', (tester) async {
      final text = await pump(
        tester,
        const AppMoneyText(
          amount: '5.0000',
          currency: 'TRY',
          effect: AppMoneyEffect.income,
          isCancelled: true,
          semanticsSuffix: 'İptal edildi',
        ),
      );
      expect(text.semanticsLabel, '5,00 lira gelir. İptal edildi');
    });
  });
}
