import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:business_finance_mobile/app/business_finance_app.dart';
import 'package:business_finance_mobile/core/routing/app_router.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_controller.dart';
import 'package:provider/provider.dart';

import '../../helpers/fake_auth.dart';
import '../../helpers/fake_dashboard.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';

void main() {
  testWidgets('restoring session does not flash login or protected shell', (
    tester,
  ) async {
    final controller = AuthController(FakeAuthSessionRepository());

    await tester.pumpWidget(
      _authApp(controller, createAppRouter(authController: controller)),
    );
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 100));

    expect(find.text('Oturum kontrol ediliyor'), findsOneWidget);
    expect(find.text('Tekrar hoş geldiniz'), findsNothing);
    expect(find.byType(BottomAppBar), findsNothing);
  });

  testWidgets('unauthenticated user is redirected to login', (tester) async {
    final controller = AuthController(FakeAuthSessionRepository());
    await controller.initialize();

    await tester.pumpWidget(
      _authApp(controller, createAppRouter(authController: controller)),
    );
    await tester.pumpAndSettle();

    expect(find.text('Tekrar hoş geldiniz'), findsOneWidget);
    expect(find.text('Finansal özetiniz hazır olacak'), findsNothing);
  });

  testWidgets('login returns user to the originally requested branch', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
    final repository = FakeAuthSessionRepository();
    final controller = AuthController(repository);
    await controller.initialize();

    await tester.pumpWidget(
      _authApp(
        controller,
        createAppRouter(
          initialLocation: '/budgets',
          authController: controller,
        ),
      ),
    );
    await tester.pumpAndSettle();
    await tester.enterText(
      find.byType(TextFormField).first,
      'user@example.test',
    );
    await tester.enterText(find.byType(TextFormField).last, 'password');
    await tester.tap(find.text('Giriş yap'));
    await tester.pumpAndSettle();

    expect(find.text('Bütçe servisi yapılandırılmadı.'), findsOneWidget);
    expect(find.byType(BottomAppBar), findsOneWidget);
  });

  testWidgets('authenticated user cannot remain on login route', (
    tester,
  ) async {
    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();

    await tester.pumpWidget(
      _authApp(
        controller,
        createAppRouter(initialLocation: '/login', authController: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Kategori giderleri'), findsOneWidget);
    expect(find.text('Tekrar hoş geldiniz'), findsNothing);
  });

  testWidgets('changing users rebuilds protected state before loading data', (
    tester,
  ) async {
    // Özet ekranı `Yaklaşanlar` bölümüyle birlikte 600 px'lik varsayılan
    // görünüm alanına sığmıyor; hesap bakiyeleri `ListView` tarafından hiç
    // kurulmuyordu. Bu testlerin konusu hangi kullanıcının ekranının
    // gösterildiği, sayfanın boyu değil — o yüzden alan yükseltiliyor.
    tester.view.physicalSize = const Size(400, 3000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
    final repository = FakeAuthSessionRepository()
      ..session = testSession(email: 'first@example.test');
    final controller = AuthController(repository);
    await controller.initialize();
    final router = createAppRouter(
      authController: controller,
      dashboardViewModelFactory: () => DashboardViewModel(
        _SessionDashboardDataSource(
          controller.session?.email ?? 'signed-out@example.test',
        ),
        now: () => DateTime(2026, 8, 13),
      ),
    );

    await tester.pumpWidget(
      BusinessFinanceApp(authController: controller, router: router),
    );
    await tester.pumpAndSettle();

    expect(find.text('first@example.test hesabı'), findsOneWidget);

    await controller.logout();
    await tester.pumpAndSettle();

    expect(find.text('Tekrar hoş geldiniz'), findsOneWidget);
    expect(find.text('first@example.test hesabı'), findsNothing);

    repository.loginSession = testSession(
      userId: '22222222-2222-2222-2222-222222222222',
      email: 'second@example.test',
    );
    await controller.login('second@example.test', 'password');
    await tester.pumpAndSettle();

    expect(find.text('second@example.test hesabı'), findsOneWidget);
    expect(find.text('first@example.test hesabı'), findsNothing);
  });

  testWidgets('restored session replaces protected pages without assertion', (
    tester,
  ) async {
    // Özet ekranı `Yaklaşanlar` bölümüyle birlikte 600 px'lik varsayılan
    // görünüm alanına sığmıyor; hesap bakiyeleri `ListView` tarafından hiç
    // kurulmuyordu. Bu testlerin konusu hangi kullanıcının ekranının
    // gösterildiği, sayfanın boyu değil — o yüzden alan yükseltiliyor.
    tester.view.physicalSize = const Size(400, 3000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
    final repository = FakeAuthSessionRepository()
      ..session = testSession(email: 'restored@example.test');
    final controller = AuthController(repository);
    final router = createAppRouter(
      authController: controller,
      dashboardViewModelFactory: () => DashboardViewModel(
        _SessionDashboardDataSource(
          controller.session?.email ?? 'signed-out@example.test',
        ),
        now: () => DateTime(2026, 8, 13),
      ),
    );

    await tester.pumpWidget(
      BusinessFinanceApp(authController: controller, router: router),
    );
    expect(find.text('Oturum kontrol ediliyor'), findsOneWidget);

    await controller.initialize();
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('restored@example.test hesabı'), findsOneWidget);
  });
}

Widget _authApp(AuthController controller, GoRouter router) =>
    ChangeNotifierProvider.value(
      value: testDashboardViewModel(),
      child: BusinessFinanceApp(authController: controller, router: router),
    );

class _SessionDashboardDataSource implements DashboardDataSource {
  const _SessionDashboardDataSource(this.email);

  final String email;

  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) async => DashboardReport(
    year: year,
    month: month,
    totalIncome: '0.0000',
    totalExpense: '0.0000',
    net: '0.0000',
    currency: 'TRY',
    categoryExpenses: const [],
    categoryExpenseSlices: const [],
    accountBalances: [
      AccountBalance(
        accountId: email,
        accountName: '$email hesabı',
        balance: '0.0000',
      ),
    ],
  );
  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) => Future.error(Exception('gelişmiş rapor bu testte yapılandırılmadı'));
}
