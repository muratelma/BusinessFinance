import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_submit_button.dart';

void main() {
  Future<void> pump(WidgetTester tester, Widget button) => tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(body: button),
    ),
  );

  testWidgets('istek uçarken ikinci dokunuş ikinci yazma üretmez', (
    tester,
  ) async {
    // Sunucuda idempotency anahtarı yok: ikinci POST ikinci kayıt olurdu.
    var calls = 0;
    final gate = Completer<void>();

    await pump(
      tester,
      AppSubmitButton(
        label: 'Kaydet',
        onSubmit: () async {
          calls++;
          await gate.future;
        },
      ),
    );

    await tester.tap(find.text('Kaydet'));
    await tester.pump();
    await tester.tap(find.text('Kaydet'));
    await tester.pump();

    expect(calls, 1);

    gate.complete();
    await tester.pumpAndSettle();
  });

  testWidgets('gönderim bittikten sonra yeniden basılabilir', (tester) async {
    var calls = 0;
    await pump(
      tester,
      AppSubmitButton(label: 'Kaydet', onSubmit: () async => calls++),
    );

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(calls, 2);
  });

  testWidgets('gönderim sırasında ilerleme göstergesi çıkar', (tester) async {
    final gate = Completer<void>();
    await pump(
      tester,
      AppSubmitButton(label: 'Kaydet', onSubmit: () => gate.future),
    );

    expect(find.byType(CircularProgressIndicator), findsNothing);

    await tester.tap(find.text('Kaydet'));
    await tester.pump();

    expect(find.byType(CircularProgressIndicator), findsOneWidget);

    gate.complete();
    await tester.pumpAndSettle();
    expect(find.byType(CircularProgressIndicator), findsNothing);
  });

  testWidgets('hata atan gönderimden sonra buton kilitli kalmaz', (
    tester,
  ) async {
    var calls = 0;
    await pump(
      tester,
      AppSubmitButton(
        label: 'Kaydet',
        onSubmit: () async {
          calls++;
          throw StateError('sunucu hatası');
        },
      ),
    );

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();
    expect(
      tester.takeException(),
      isA<StateError>(),
      reason: 'Kaçan hata sessizce yutulmamalı, görünür biçimde bildirilmeli.',
    );

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();
    expect(tester.takeException(), isA<StateError>());

    expect(
      calls,
      2,
      reason: 'Hata sonrası buton kalıcı olarak kilitlenmemeli.',
    );
  });

  testWidgets('dışarıdan yönetilen meşguliyet butonu devre dışı bırakır', (
    tester,
  ) async {
    var calls = 0;
    await pump(
      tester,
      AppSubmitButton(
        label: 'Kaydet',
        isBusy: true,
        onSubmit: () async => calls++,
      ),
    );

    await tester.tap(find.text('Kaydet'));
    await tester.pump();

    expect(calls, 0);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
  });

  testWidgets('onSubmit null ise buton devre dışıdır', (tester) async {
    await pump(tester, const AppSubmitButton(label: 'Kaydet', onSubmit: null));

    final button = tester.widget<FilledButton>(find.byType(FilledButton));
    expect(button.onPressed, isNull);
  });
}
