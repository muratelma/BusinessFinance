import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/auth/presentation/password_reset_page.dart';

import '../../../helpers/accessibility.dart';

void main() {
  testWidgets('kod istenmeden yeni parola alanı çıkmaz', (tester) async {
    await tester.pumpWidget(_page());

    expect(find.widgetWithText(TextFormField, 'E-posta'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, 'Doğrulama kodu'), findsNothing);
    expect(find.widgetWithText(FilledButton, 'Kod gönder'), findsOneWidget);
  });

  testWidgets('geçersiz adres için istek gitmez', (tester) async {
    final requests = <String>[];
    await tester.pumpWidget(
      _page(onRequestCode: (email) async => requests.add(email)),
    );

    await tester.enterText(
      find.widgetWithText(TextFormField, 'E-posta'),
      'gecersiz',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Kod gönder'));
    await tester.pumpAndSettle();

    expect(requests, isEmpty);
    expect(find.text('Geçerli bir e-posta adresi girin.'), findsOneWidget);
  });

  testWidgets('cevap adresin kayıtlı olup olmadığını söylemez', (tester) async {
    final requests = <String>[];
    await tester.pumpWidget(
      _page(onRequestCode: (email) async => requests.add(email)),
    );

    await tester.enterText(
      find.widgetWithText(TextFormField, 'E-posta'),
      'kimse@example.test',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Kod gönder'));
    await tester.pumpAndSettle();

    expect(requests, ['kimse@example.test']);
    // "Hesap bulunamadı" ya da "kod gönderildi" değil: koşullu tek cümle.
    expect(
      find.text(
        'Adres kayıtlıysa kod gönderildi. Gelen kutunuzu kontrol edin.',
      ),
      findsOneWidget,
    );
    expect(
      find.widgetWithText(TextFormField, 'Doğrulama kodu'),
      findsOneWidget,
    );
  });

  testWidgets('eksik kod ve zayıf parola istek üretmez', (tester) async {
    var resetCount = 0;
    await tester.pumpWidget(
      _page(
        onReset:
            ({
              required String email,
              required String code,
              required String newPassword,
            }) async => resetCount++,
      ),
    );
    await _requestCode(tester);

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Doğrulama kodu'),
      '123',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Parolamı değiştir'));
    await tester.pumpAndSettle();
    expect(resetCount, 0);
    expect(find.text('Altı haneli kodu yazın.'), findsOneWidget);

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Doğrulama kodu'),
      '123456',
    );
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Yeni parola'),
      'kisa',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Parolamı değiştir'));
    await tester.pumpAndSettle();

    expect(resetCount, 0);
    expect(find.text('Parola en az 12 karakter olmalıdır.'), findsOneWidget);
  });

  testWidgets('doğru kod ve parola sıfırlamayı tamamlar', (tester) async {
    String? completedEmail;
    ({String code, String email, String password})? sent;
    await tester.pumpWidget(
      _page(
        onReset:
            ({
              required String email,
              required String code,
              required String newPassword,
            }) async =>
                sent = (email: email, code: code, password: newPassword),
        onCompleted: (email) => completedEmail = email,
      ),
    );
    await _requestCode(tester);

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Doğrulama kodu'),
      '123456',
    );
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Yeni parola'),
      'Another-Password-456!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Parolamı değiştir'));
    await tester.pumpAndSettle();

    expect(sent?.email, 'esnaf@example.test');
    expect(sent?.code, '123456');
    expect(sent?.password, 'Another-Password-456!');
    expect(completedEmail, 'esnaf@example.test');
  });

  testWidgets('sunucunun reddi ekranda görünür', (tester) async {
    await tester.pumpWidget(
      _page(
        onReset:
            ({
              required String email,
              required String code,
              required String newPassword,
            }) async => throw const ApiException(
              code: 'authentication.invalid_reset_code',
              message: 'Kod geçersiz veya süresi dolmuş.',
              statusCode: 400,
            ),
      ),
    );
    await _requestCode(tester);

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Doğrulama kodu'),
      '123456',
    );
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Yeni parola'),
      'Another-Password-456!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Parolamı değiştir'));
    await tester.pumpAndSettle();

    expect(find.text('Kod geçersiz veya süresi dolmuş.'), findsOneWidget);
  });

  testWidgets('sıfırlama ekranı erişilebilirlik kapısını geçer', (
    tester,
  ) async {
    await pumpAtLargestTextScale(tester, _page());

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

Future<void> _requestCode(WidgetTester tester) async {
  await tester.enterText(
    find.widgetWithText(TextFormField, 'E-posta'),
    'esnaf@example.test',
  );
  await tester.tap(find.widgetWithText(FilledButton, 'Kod gönder'));
  await tester.pumpAndSettle();
}

Widget _page({
  Future<void> Function(String email)? onRequestCode,
  Future<void> Function({
    required String email,
    required String code,
    required String newPassword,
  })?
  onReset,
  void Function(String email)? onCompleted,
}) => MaterialApp(
  theme: AppTheme.light(),
  home: PasswordResetPage(
    onRequestCode: onRequestCode ?? (_) async {},
    onReset:
        onReset ??
        ({
          required String email,
          required String code,
          required String newPassword,
        }) async {},
    onCompleted: onCompleted ?? (_) {},
    onBackToLogin: () {},
  ),
);
