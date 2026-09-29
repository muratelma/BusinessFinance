import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/localization/app_locale.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';

import '../../helpers/accessibility.dart';

void main() {
  Future<void> pump(
    WidgetTester tester, {
    String? value,
    required ValueChanged<String> onChanged,
  }) => tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      locale: AppLocale.turkish,
      supportedLocales: AppLocale.supported,
      localizationsDelegates: AppLocale.delegates,
      home: Scaffold(
        body: Padding(
          padding: const EdgeInsets.all(16),
          child: AppDateField(
            label: 'İşlem tarihi',
            value: value,
            onChanged: onChanged,
          ),
        ),
      ),
    ),
  );

  testWidgets('metin alanlarıyla aynı dekorasyonu kullanır', (tester) async {
    await pump(tester, value: '2026-08-16', onChanged: (_) {});

    // Asıl kazanç bu: tarih artık çıplak bir `ListTile` değil, yanındaki
    // girdi alanlarıyla aynı `InputDecorator` kabuğunu taşıyor ve dolgu,
    // kenarlık, etiket temadan geliyor.
    expect(find.byType(InputDecorator), findsOneWidget);
    expect(find.text('İşlem tarihi'), findsOneWidget);
    expect(find.text('2026-08-16'), findsOneWidget);
  });

  testWidgets('değer yokken seçilmediğini söyler', (tester) async {
    await pump(tester, onChanged: (_) {});

    expect(find.text('Seçilmedi'), findsOneWidget);
  });

  // Vergi araştırması V-U6: boş alanda etiket içerik yerine iniyor ve
  // "Seçilmedi" ile üst üste çiziliyordu. Etiket her zaman üstte durur.
  testWidgets('boş alanda etiket yer tutucunun üstünde durur', (tester) async {
    await pump(tester, onChanged: (_) {});

    final label = tester.getRect(find.text('İşlem tarihi'));
    final placeholder = tester.getRect(find.text('Seçilmedi'));
    expect(label.bottom, lessThanOrEqualTo(placeholder.top));
  });

  testWidgets('seçilen günü API biçiminde bildirir', (tester) async {
    String? reported;
    await pump(
      tester,
      value: '2026-08-16',
      onChanged: (value) => reported = value,
    );

    await tester.tap(find.byType(InputDecorator));
    await tester.pumpAndSettle();
    await tester.tap(find.text('20'));
    await tester.tap(find.text('Tamam'));
    await tester.pumpAndSettle();

    expect(reported, '2026-08-20');
  });

  testWidgets('erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Padding(
            padding: const EdgeInsets.all(16),
            child: AppDateField(
              label: 'İşlem tarihi',
              value: '2026-08-16',
              helperText: 'Kaydın düştüğü gün.',
              onChanged: (_) {},
            ),
          ),
        ),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  test('biçimlendirme ve çözümleme birbirinin tersidir', () {
    expect(AppDateField.format(DateTime(2026, 1, 5)), '2026-01-05');
    expect(AppDateField.parse('2026-01-05'), DateTime(2026, 1, 5));
    expect(AppDateField.parse('bozuk'), isNull);
    expect(AppDateField.parse(null), isNull);
  });
}
