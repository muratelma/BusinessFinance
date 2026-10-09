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
      'Hesaplar ve transferler',
      'Kredi kartlarım',
      'Kasa',
      'Borç ve alacaklar',
      'Yükümlülükler',
      'Tasarruf hedefleri',
      'Planlama ve raporlar',
      'Kategoriler',
      'Hatırlatmalar',
      'Veri ve yedek',
    ]);
    expect(find.text('Vergi'), findsNothing);
    // Hesap kartı + üç grup.
    expect(find.byType(AppCard), findsNWidgets(4));
  });

  /// Cari hesap işletmeye özeldir (Aşama 06.3 C6). Gizleme bir ön ayardır,
  /// kilit değildir: cari hareketi olan kullanıcı kapıyı cevabı ne olursa
  /// olsun görür; aksi hâlde açık hesabına ulaşamazdı.
  testWidgets('cari hesap kapısı işletmesi olana ve cari hareketi olana açık', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1080, 3200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(_host(hasBusiness: false));
    await tester.pumpAndSettle();
    expect(titles(tester), isNot(contains('Cari hesap')));
    // Herkese açık kalan iki komşusu yerinde.
    expect(titles(tester), contains('Borç ve alacaklar'));
    expect(titles(tester), contains('Yükümlülükler'));

    await tester.pumpWidget(_host(hasBusiness: false, hasLedger: true));
    await tester.pumpAndSettle();
    expect(titles(tester), contains('Cari hesap'));

    await tester.pumpWidget(_host(hasBusiness: true));
    await tester.pumpAndSettle();
    expect(titles(tester), contains('Cari hesap'));
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

  testWidgets('hesap kartı e-postayı ve doğrulanmamış adresi söyler', (
    tester,
  ) async {
    await tester.pumpWidget(_host(hasBusiness: false));
    await tester.pumpAndSettle();

    expect(find.text('Hesabım'), findsOneWidget);
    expect(find.text('esnaf@ornek.com'), findsOneWidget);
    expect(find.text('E-posta doğrulanmadı'), findsOneWidget);
    expect(find.text('ES'), findsOneWidget);

    await tester.tap(find.text('Hesabım'));
    await tester.pumpAndSettle();
    expect(find.text('hesap sayfası'), findsOneWidget);
  });

  testWidgets('menü erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(tester, _host(hasBusiness: true));

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

Widget _host({required bool hasBusiness, bool hasLedger = false}) {
  final scope = ScopeController(
    readHasBusiness: () async => hasBusiness,
    readHasCounterpartyLedger: () async => hasLedger,
  );
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
