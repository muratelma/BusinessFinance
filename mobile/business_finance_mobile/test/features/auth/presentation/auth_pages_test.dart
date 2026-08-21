import 'dart:async';

import 'package:flutter/material.dart';
import '../../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/auth/presentation/login_page.dart';
import 'package:business_finance_mobile/features/auth/presentation/register_page.dart';

void main() {
  testWidgets('giriş ekranı erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: LoginPage(onSubmit: (_, _) async {}, onRegister: () {}),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  testWidgets('login validates fields before submitting', (tester) async {
    var submitCount = 0;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: LoginPage(
          onSubmit: (_, _) async => submitCount++,
          onRegister: () {},
        ),
      ),
    );

    await tester.tap(find.text('Giriş yap'));
    await tester.pump();

    expect(find.text('E-posta adresinizi girin.'), findsOneWidget);
    expect(find.text('Parolanızı girin.'), findsOneWidget);
    expect(submitCount, 0);
  });

  testWidgets('login prevents a second submit while request is running', (
    tester,
  ) async {
    final completer = Completer<void>();
    var submitCount = 0;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: LoginPage(
          onSubmit: (_, _) {
            submitCount++;
            return completer.future;
          },
          onRegister: () {},
        ),
      ),
    );
    await tester.enterText(
      find.byType(TextFormField).first,
      'user@example.test',
    );
    await tester.enterText(find.byType(TextFormField).last, 'password');

    await tester.tap(find.text('Giriş yap'));
    await tester.pump();
    await tester.tap(find.byType(FilledButton));
    await tester.pump();

    expect(submitCount, 1);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    completer.complete();
    await tester.pumpAndSettle();
  });

  testWidgets('login shows safe API detail and password visibility semantics', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: LoginPage(
          onSubmit: (_, _) => throw const ApiException(
            statusCode: 401,
            code: 'authentication.invalid_credentials',
            message: 'E-posta veya parola geçersiz.',
          ),
          onRegister: () {},
        ),
      ),
    );
    await tester.enterText(
      find.byType(TextFormField).first,
      'user@example.test',
    );
    await tester.enterText(find.byType(TextFormField).last, 'wrong');

    expect(find.byTooltip('Parolayı göster'), findsOneWidget);
    await tester.tap(find.text('Giriş yap'));
    await tester.pumpAndSettle();

    expect(find.text('E-posta veya parola geçersiz.'), findsOneWidget);
  });

  testWidgets('registration enforces policy and returns email after success', (
    tester,
  ) async {
    String? completedEmail;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: RegisterPage(
          onSubmit: (_, _) async {},
          onCompleted: (email) => completedEmail = email,
          onBackToLogin: () {},
        ),
      ),
    );
    await tester.enterText(
      find.byType(TextFormField).first,
      'new@example.test',
    );
    await tester.enterText(find.byType(TextFormField).last, 'short');
    await tester.tap(find.text('Kayıt ol'));
    await tester.pump();

    expect(find.text('Parola en az 12 karakter olmalıdır.'), findsOneWidget);

    await tester.enterText(
      find.byType(TextFormField).last,
      'Valid-Password-123!',
    );
    await tester.tap(find.text('Kayıt ol'));
    await tester.pumpAndSettle();

    expect(completedEmail, 'new@example.test');
  });
}
