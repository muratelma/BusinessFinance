import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_row.dart';
import 'package:business_finance_mobile/features/account/data/account_models.dart';
import 'package:business_finance_mobile/features/account/data/account_repository.dart';
import 'package:business_finance_mobile/features/account/presentation/account_status_controller.dart';
import 'package:business_finance_mobile/features/more/presentation/more_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../helpers/accessibility.dart';

void main() {
  List<String> titles(WidgetTester tester) => tester
      .widgetList<AppRow>(find.byType(AppRow))
      .map((row) => row.title)
      .toList(growable: false);

  /// Kapılar kullanıcının sorusuna göre gruplanır (06.2 Grup 1). Kişisel
  /// profilde üçüncü sekme `Bütçeler` olduğu için `Kasa` paranın durduğu
  /// yerlerin yanına iner; vergi grubu hiç yoktur.
  testWidgets('kişisel profilde kasa Para ve hesaplar altında durur', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1080, 3200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(_host(hasBusiness: false));
    await tester.pumpAndSettle();

    expect(titles(tester), [
      'Hesaplar',
      'Kredi kartlarım',
      'Kasa',
      'Borç ve alacak',
      'Kategori özeti',
      'Kategori yönetimi',
      'Planlananlar',
      'Tekrarlayanlar',
      'Tasarruf hedefleri',
      'Planlama ve raporlar',
      'Hatırlatmalar',
      'Veri ve yedek',
    ]);
    expect(find.text('Vergi'), findsNothing);
    // Hesap kartı + üç grup.
    expect(find.byType(AppCard), findsNWidgets(5));
  });

  /// Cari hesap ayrı bir menü değildir (ADR 0022, K5): `Borç ve alacak`
  /// herkese açıktır, `Cari hesap` ve `Yükümlülükler` kalktı.
  testWidgets('borç ve alacak tek giriştir', (tester) async {
    tester.view.physicalSize = const Size(1080, 3200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    for (final hasBusiness in [false, true]) {
      await tester.pumpWidget(_host(hasBusiness: hasBusiness));
      await tester.pumpAndSettle();
      final shown = titles(tester);
      expect(shown, contains('Borç ve alacak'));
      expect(shown, isNot(contains('Cari hesap')));
      expect(shown, isNot(contains('Yükümlülükler')));
    }
  });

  testWidgets('işletme profilinde bütçeler ve vergi grubu gelir', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1080, 3200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(_host(hasBusiness: true));
    await tester.pumpAndSettle();

    final shown = titles(tester);
    expect(shown, contains('Bütçeler'));
    expect(shown, isNot(contains('Kasa')));
    expect(shown, contains('Vergi takibi'));
    expect(shown, isNot(contains('Muhasebeci paketi')));
    for (final label in ['Para ve hesaplar', 'Planlama', 'Vergi', 'Ayarlar']) {
      expect(find.text(label), findsOneWidget);
    }
  });

  testWidgets('hesap kartı doğrulanmamış adresi söyler, e-postayı yazmaz', (
    tester,
  ) async {
    await tester.pumpWidget(_host(hasBusiness: false));
    await tester.pumpAndSettle();

    // E-posta adresi bu kartta yazmaz; başlık kullanıcı adıdır (yoksa
    // `Hesabım`).
    expect(find.text('Hesap, güvenlik ve oturumlar'), findsOneWidget);
    expect(find.text('esnaf@ornek.com'), findsNothing);
    expect(find.text('E-posta doğrulanmadı'), findsOneWidget);
    expect(find.text('ES'), findsOneWidget);

    await tester.tap(find.text('Hesap, güvenlik ve oturumlar'));
    await tester.pumpAndSettle();
    expect(find.text('hesap sayfası'), findsOneWidget);
  });

  testWidgets('menü erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(tester, _host(hasBusiness: true));

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

Widget _host({required bool hasBusiness}) {
  final scope = ScopeController(readHasBusiness: () async => hasBusiness);
  final status = AccountStatusController(_Account())..ensureLoaded();
  scope.ensureLoaded();
  return MultiProvider(
    providers: [
      ChangeNotifierProvider<ScopeController?>.value(value: scope),
      ChangeNotifierProvider<AccountStatusController?>.value(value: status),
    ],
    child: MaterialApp.router(
      theme: AppTheme.light(),
      routerConfig: GoRouter(
        routes: [
          GoRoute(path: '/', builder: (_, _) => const MorePage()),
          GoRoute(
            path: '/more/account',
            builder: (_, _) => const Text('hesap sayfası'),
          ),
          // Menüdeki her satırın gerçekten bir hedefi olduğunu doğrulamak
          // için hepsi tanımlı; eksik bir rota testte değil kullanıcıda
          // patlardı.
          for (final path in [
            '/more/accounts',
            '/more/cards',
            '/more/cash',
            '/more/budgets',
            '/more/categories',
            '/more/counterparties',
            '/more/obligations',
            '/more/debts',
            '/more/debt-book',
            '/more/goals',
            '/more/planning',
            '/more/tax-calendar',
            '/more/reminders',
            '/more/data-tools',
          ])
            GoRoute(path: path, builder: (_, _) => const SizedBox.shrink()),
        ],
      ),
    ),
  );
}

class _Account extends Fake implements AccountRepositoryContract {
  @override
  Future<UserAccount> read() async => UserAccount(
    userId: 'u1',
    email: 'esnaf@ornek.com',
    emailConfirmed: false,
    createdAtUtc: DateTime.utc(2026, 3, 14),
    activeSessionCount: 1,
  );
}
