import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:integration_test/integration_test.dart';
import 'package:business_finance_mobile/app/app_dependencies.dart';
import 'package:business_finance_mobile/app/business_finance_app.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_form_page.dart';
import 'package:business_finance_mobile/features/accounts/presentation/accounts_page.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_page.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';
import 'package:provider/provider.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('register, login, account, expense and dashboard smoke flow', (
    tester,
  ) async {
    final suffix = DateTime.now().microsecondsSinceEpoch;
    final email = 'stage8.smoke.$suffix@example.com';
    const password = 'Stage8-Smoke-2026';
    final accountName = 'Stage 8 Cash $suffix';
    final description = 'Stage 8 smoke expense $suffix';

    final dependencies = AppDependencies.create();
    addTearDown(dependencies.dispose);
    dependencies.authController.initialize();
    await tester.pumpWidget(BusinessFinanceApp(dependencies: dependencies));
    await _waitFor(tester, find.text('Hesap oluştur'));

    await tester.tap(find.text('Hesap oluştur'));
    await _waitFor(tester, find.text('Kayıt ol'));
    await tester.enterText(find.byType(TextFormField).at(0), email);
    await tester.enterText(find.byType(TextFormField).at(1), password);
    await tester.tap(find.text('Kayıt ol'));

    await _waitFor(tester, find.text('Giriş yap'));
    expect(_fieldText(tester, 0), email);
    await tester.enterText(find.byType(TextFormField).at(1), password);
    await tester.tap(find.text('Giriş yap'));
    await _hideKeyboard(tester);

    await _waitFor(tester, find.text('Özet'));
    await tester.tap(find.text('Diğer'));
    await _waitFor(tester, find.text('Hesaplar'));
    await tester.tap(find.text('Hesaplar'));

    await _waitFor(tester, find.text('Hesap ekle'));
    final accountsFinder = find.byType(AccountsPage);
    final accountsPage = tester.widget<AccountsPage>(accountsFinder);
    final router = GoRouter.of(tester.element(accountsFinder));
    Navigator.of(tester.element(accountsFinder)).push(
      MaterialPageRoute<void>(
        builder: (_) => AccountFormPage(onSave: accountsPage.viewModel.save),
      ),
    );
    await tester.pumpAndSettle();
    await _waitFor(tester, find.byType(AccountFormPage));
    await tester.enterText(find.byType(TextFormField).at(0), accountName);
    await tester.enterText(find.byType(TextFormField).at(1), '1000');
    final accountSaved = await accountsPage.viewModel.save(
      name: accountName,
      type: 'cash',
      openingBalance: '1000',
      isActive: true,
    );
    expect(accountSaved, isTrue, reason: accountsPage.viewModel.message);
    Navigator.of(tester.element(find.byType(AccountFormPage))).pop(true);
    await tester.pumpAndSettle();

    await _waitForAbsent(tester, find.byType(AccountFormPage));
    await _waitFor(tester, find.text(accountName));
    router.go('/transactions/new');
    await tester.pumpAndSettle();

    await _waitFor(tester, find.text('Yeni işlem'));
    await _waitFor(
      tester,
      find.byType(DropdownButtonFormField<String>),
      minimumCount: 2,
    );
    await tester.tap(find.byType(DropdownButtonFormField<String>).at(0));
    await tester.pumpAndSettle();
    await tester.tap(find.text(accountName).last);
    await tester.pumpAndSettle();
    await tester.tap(find.byType(DropdownButtonFormField<String>).at(1));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Groceries').last);
    await tester.pumpAndSettle();

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Tutar'),
      '25,50',
    );
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Açıklama (isteğe bağlı)'),
      description,
    );
    await _hideKeyboard(tester);
    final saveTransactionButton = find.text('Kaydet').last;
    await tester.ensureVisible(saveTransactionButton);
    await tester.tap(saveTransactionButton);

    await _waitFor(tester, find.text(description));
    router.go('/dashboard');
    await tester.pumpAndSettle();
    await _waitFor(tester, find.byType(DashboardPage));
    final dashboardViewModel = tester
        .element(find.byType(DashboardPage))
        .read<DashboardViewModel>();
    await dashboardViewModel.load();
    await tester.pumpAndSettle();
    await _waitFor(tester, find.text(accountName));

    expect(find.text('Groceries'), findsOneWidget);
    expect(find.text('₺25,50'), findsWidgets);
    expect(find.text('₺974,50'), findsOneWidget);
  });
}

String _fieldText(WidgetTester tester, int index) {
  return tester
      .widget<TextFormField>(find.byType(TextFormField).at(index))
      .controller!
      .text;
}

Future<void> _hideKeyboard(WidgetTester tester) async {
  FocusManager.instance.primaryFocus?.unfocus();
  await SystemChannels.textInput.invokeMethod<void>('TextInput.hide');
  await tester.pumpAndSettle();
}

Future<void> _waitFor(
  WidgetTester tester,
  Finder finder, {
  int minimumCount = 1,
  Duration timeout = const Duration(seconds: 20),
}) async {
  final deadline = DateTime.now().add(timeout);
  while (finder.evaluate().length < minimumCount &&
      DateTime.now().isBefore(deadline)) {
    await tester.pump(const Duration(milliseconds: 200));
  }
  expect(finder, findsAtLeast(minimumCount));
}

Future<void> _waitForAbsent(
  WidgetTester tester,
  Finder finder, {
  Duration timeout = const Duration(seconds: 20),
}) async {
  final deadline = DateTime.now().add(timeout);
  while (finder.evaluate().isNotEmpty && DateTime.now().isBefore(deadline)) {
    await tester.pump(const Duration(milliseconds: 200));
  }
  expect(finder, findsNothing);
}
