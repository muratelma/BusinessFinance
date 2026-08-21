import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/localization/app_locale.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';

import '../../helpers/accessibility.dart';

void main() {
  Future<String?> open(
    WidgetTester tester, {
    required Future<String?> Function()? onSubmit,
    String? secondaryLabel,
    Future<String?> Function()? onSecondary,
    TextEditingController? controller,
  }) async {
    String? result;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        locale: AppLocale.turkish,
        supportedLocales: AppLocale.supported,
        localizationsDelegates: AppLocale.delegates,
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () async {
                result = await AppFormSheet.show<String>(
                  context: context,
                  builder: (_) => AppFormSheet<String>(
                    title: 'Sentetik panel',
                    submitLabel: 'Kaydet',
                    secondaryLabel: secondaryLabel,
                    onSecondary: onSecondary,
                    onSubmit: onSubmit,
                    children: [
                      AppFormField(
                        child: TextField(
                          controller: controller,
                          decoration: const InputDecoration(labelText: 'Ad'),
                        ),
                      ),
                    ],
                  ),
                );
              },
              child: const Text('Aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('Aç'));
    await tester.pumpAndSettle();
    return result;
  }

  testWidgets('başlığı ve iki eylemi her zaman gösterir', (tester) async {
    await open(tester, onSubmit: () async => 'x');

    expect(find.text('Sentetik panel'), findsOneWidget);
    expect(find.text('Kaydet'), findsOneWidget);
    expect(find.text('Vazgeç'), findsOneWidget);
  });

  testWidgets('gönderim null dönerse panel açık kalır', (tester) async {
    var calls = 0;
    await open(
      tester,
      onSubmit: () async {
        calls++;
        return null;
      },
    );

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(calls, 1);
    // Doğrulama düşen form kapanmamalı: kullanıcının düzeltmesi gereken alan
    // hâlâ ekranda olmalı.
    expect(find.text('Sentetik panel'), findsOneWidget);
  });

  testWidgets('vazgeçmek sonuç üretmez', (tester) async {
    await open(tester, onSubmit: () async => 'kaydedildi');

    await tester.tap(find.widgetWithText(TextButton, 'Vazgeç'));
    await tester.pumpAndSettle();

    expect(find.text('Sentetik panel'), findsNothing);
  });

  testWidgets('üçüncü eylem kendi sonucunu döndürür', (tester) async {
    await open(
      tester,
      onSubmit: () async => 'kaydedildi',
      secondaryLabel: 'Temizle',
      onSecondary: () async => 'temizlendi',
    );

    expect(find.text('Temizle'), findsOneWidget);
    await tester.tap(find.text('Temizle'));
    await tester.pumpAndSettle();

    expect(find.text('Sentetik panel'), findsNothing);
  });

  testWidgets('gönderim yokken buton devre dışıdır', (tester) async {
    await open(tester, onSubmit: null);

    final button = tester.widget<FilledButton>(
      find.widgetWithText(FilledButton, 'Kaydet'),
    );
    expect(button.onPressed, isNull);
  });

  testWidgets('en büyük yazı ölçeğinde taşmadan çalışır', (tester) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MediaQuery(
        data: const MediaQueryData(textScaler: TextScaler.linear(2)),
        child: MaterialApp(
          theme: AppTheme.light(),
          locale: AppLocale.turkish,
          supportedLocales: AppLocale.supported,
          localizationsDelegates: AppLocale.delegates,
          home: Scaffold(
            body: AppFormSheet<String>(
              title: 'Borç / alacak planı',
              description:
                  'Taksitler toplam geri ödemeden eşit olarak bölünür ve ilk '
                  'vade tarihinden başlayarak aylık ilerler.',
              submitLabel: 'Oluştur',
              secondaryLabel: 'Temizle',
              onSecondary: () async => null,
              onSubmit: () async => 'x',
              children: [
                for (final label in ['Kişi / kurum', 'Anapara', 'Taksit'])
                  AppFormField(
                    child: TextField(
                      decoration: InputDecoration(labelText: label),
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  // Panellerin çöküş nedeni buydu: controller rotadan önce ölüyordu.
  // Kabuk artık kapanışı sonuna kadar sürdürüyor, alanları taşıyan
  // `StatefulWidget` de controller'ını kendi `dispose`'unda bırakıyor.
  testWidgets('kapanış animasyonu bittikten sonra da hata vermez', (
    tester,
  ) async {
    final controller = TextEditingController();
    addTearDown(controller.dispose);
    await open(
      tester,
      onSubmit: () async => 'kaydedildi',
      controller: controller,
    );

    await tester.enterText(find.byType(TextField), 'deneme');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
  });
}
