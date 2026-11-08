import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_flow.dart';
import 'package:business_finance_mobile/features/auth/presentation/widgets/auth_form_parts.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Giriş akışındaki bir alanın kendisi. Etiket alanın üstünde ayrı bir yazı
/// olduğu için alan, etiketini taşıyan [AuthField] içinden bulunur.
Finder authField(String label) => find.descendant(
  of: find.widgetWithText(AuthField, label),
  matching: find.byType(TextFormField),
);

/// Ana düğme (başlıkla aynı yazıyı taşıyabilir: "Giriş yap").
Finder authButton(String label) => find.widgetWithText(FilledButton, label);

Widget authFlowApp({
  AuthLogin? onLogin,
  AuthRegister? onRegister,
  Future<void> Function(String email)? onRequestPasswordReset,
  AuthView initialView = AuthView.login,
  ThemeData? theme,
}) => MaterialApp(
  debugShowCheckedModeBanner: false,
  theme: theme ?? AppTheme.light(),
  home: AuthFlow(
    initialView: initialView,
    onLogin: onLogin ?? (_, _, {onVerified}) async {},
    onRegister:
        onRegister ?? (_, _, {required hasBusiness, onVerified}) async {},
    onRequestPasswordReset: onRequestPasswordReset ?? (_) async {},
    onResetPassword:
        ({
          required String email,
          required String code,
          required String newPassword,
        }) async {},
  ),
);

/// Akışı kurar ve açılış animasyonunu bitirir.
Future<void> pumpAuthFlow(WidgetTester tester, Widget app) async {
  await tester.pumpWidget(app);
  await tester.pumpAndSettle();
}
