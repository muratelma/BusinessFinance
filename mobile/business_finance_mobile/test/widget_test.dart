import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/app/business_finance_app.dart';
import 'package:business_finance_mobile/core/localization/app_locale.dart';
import 'package:business_finance_mobile/core/routing/app_router.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_breakpoints.dart';
import 'package:business_finance_mobile/core/widgets/app_content_width.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';
import 'package:provider/provider.dart';

import 'helpers/fake_dashboard.dart';

void main() {
  testWidgets('phone shell navigates between primary destinations', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    await tester.pumpWidget(_testApp());
    await tester.pumpAndSettle();

    expect(find.byType(BottomAppBar), findsOneWidget);
    expect(find.text('Kategori giderleri'), findsOneWidget);

    // Ortadaki buton çubuğun çentiğine oturur ve her sekmede kalır: sekmeye
    // göre kaybolsaydı çentik boş bir oyuk olarak dururdu.
    expect(_globalTransactionAction(), findsOneWidget);
    expect(find.byType(FloatingActionButton), findsOneWidget);

    await tester.tap(find.text('İşlemler').last);
    await tester.pump();

    expect(_globalTransactionAction(), findsOneWidget);
    // Sekmenin kendi kayan butonu yok; ikisi aynı ekranda yarışıyordu.
    expect(find.byType(FloatingActionButton), findsOneWidget);

    await tester.tap(find.text('Özet').last);
    await tester.pumpAndSettle();

    expect(find.text('Kategori giderleri'), findsOneWidget);
    expect(_globalTransactionAction(), findsOneWidget);
  });

  testWidgets('wide shell uses an adaptive navigation rail', (tester) async {
    tester.view.physicalSize = const Size(1000, 800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    await tester.pumpWidget(_testApp());
    await tester.pumpAndSettle();

    expect(find.byType(NavigationRail), findsOneWidget);
    expect(find.byType(BottomAppBar), findsNothing);
    // Ray etiketi ve sayfanın kendi başlığı ayrı şeylerdir; shell artık
    // başlık çizmediği için "Özet" hem rayda hem sayfa başlığında görünür.
    expect(find.text('Özet'), findsWidgets);
    expect(find.text('Diğer'), findsOneWidget);
  });

  testWidgets(
    'işletme profilinde üçüncü sekme Kasa, Bütçeler Diğer altındadır',
    (tester) async {
      final scopeController = ScopeController(
        store: _ShellScopeStore(hasBusiness: true),
      );
      await scopeController.ensureLoaded();

      await tester.pumpWidget(_testApp(scopeController: scopeController));
      await tester.pumpAndSettle();

      expect(find.text('Kasa'), findsOneWidget);
      expect(find.text('Bütçeler'), findsNothing);

      await tester.tap(find.text('Kasa'));
      await tester.pumpAndSettle();
      expect(find.text('Kasa servisi yapılandırılmadı.'), findsOneWidget);

      await tester.tap(find.text('Diğer'));
      await tester.pumpAndSettle();
      expect(find.text('Bütçeler'), findsOneWidget);
      await tester.tap(find.text('Bütçeler'));
      await tester.pumpAndSettle();
      expect(find.text('Bütçe servisi yapılandırılmadı.'), findsOneWidget);
    },
  );

  testWidgets('kişisel profilde üçüncü sekme Bütçeler, Kasa Diğer altındadır', (
    tester,
  ) async {
    final scopeController = ScopeController(
      store: _ShellScopeStore(hasBusiness: false),
    );
    await scopeController.ensureLoaded();

    await tester.pumpWidget(_testApp(scopeController: scopeController));
    await tester.pumpAndSettle();

    expect(find.text('Bütçeler'), findsOneWidget);
    expect(find.text('Kasa'), findsNothing);

    await tester.tap(find.text('Bütçeler'));
    await tester.pumpAndSettle();
    expect(find.text('Bütçe servisi yapılandırılmadı.'), findsOneWidget);

    await tester.tap(find.text('Diğer'));
    await tester.pumpAndSettle();
    expect(find.text('Kasa'), findsOneWidget);
    await tester.tap(find.text('Kasa'));
    await tester.pumpAndSettle();
    expect(find.text('Kasa servisi yapılandırılmadı.'), findsOneWidget);
  });

  group('shell üç pencere sınıfında doğru gezinmeyi kurar', () {
    Future<void> pumpAt(WidgetTester tester, double width) async {
      tester.view.physicalSize = Size(width, 800);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.resetPhysicalSize);
      addTearDown(tester.view.resetDevicePixelRatio);

      await tester.pumpWidget(_testApp());
      await tester.pumpAndSettle();
    }

    testWidgets('compact: alt gezinme çubuğu', (tester) async {
      await pumpAt(tester, 400);

      expect(find.byType(BottomAppBar), findsOneWidget);
      expect(find.byType(NavigationRail), findsNothing);
    });

    testWidgets('medium: daraltılmış ray', (tester) async {
      await pumpAt(tester, 700);

      expect(find.byType(BottomAppBar), findsNothing);
      final rail = tester.widget<NavigationRail>(find.byType(NavigationRail));
      expect(
        rail.extended,
        isFalse,
        reason:
            'Genişletilmiş ray 700 dp ekranda içeriğe ayrılan yeri yer; bu '
            'kademede yalnız ikon+etiket rayı açılır.',
      );
    });

    testWidgets('expanded: genişletilmiş ray', (tester) async {
      await pumpAt(tester, 1000);

      expect(find.byType(BottomAppBar), findsNothing);
      final rail = tester.widget<NavigationRail>(find.byType(NavigationRail));
      expect(rail.extended, isTrue);
    });

    testWidgets('geniş ekranda içerik okunabilir genişlikle sınırlanır', (
      tester,
    ) async {
      await pumpAt(tester, 1400);

      // Ölçülen, sarmalayıcının hizalama kutusu değil gerçekten sınırlanan
      // içeriktir; `AppContentWidth`'in kökü tanım gereği tam genişliği kaplar.
      final contentWidth = tester
          .getSize(
            find
                .descendant(
                  of: find.byType(AppContentWidth),
                  matching: find.byType(ConstrainedBox),
                )
                .first,
          )
          .width;
      expect(
        contentWidth,
        lessThanOrEqualTo(AppBreakpoints.contentMaxWidth),
        reason:
            'Sayfa içeriği 1400 dp boyunca gerilirse liste satırı okunmaz '
            'hale gelir.',
      );
    });
  });

  testWidgets('uygulama Material metinlerini Türkçe kurar', (tester) async {
    await tester.pumpWidget(_testApp());
    await tester.pumpAndSettle();

    // Delege verilmezse uygulamanın kendi metinleri Türkçe kalır ama
    // Material'in ürettikleri (tarih seçici butonları, ay adları, metin
    // alanı bağlam menüsü) sessizce İngilizceye döner. Bu kapı sapmayı
    // ekranda değil testte yakalar.
    final context = tester.element(find.text('Kategori giderleri'));
    expect(Localizations.localeOf(context), AppLocale.turkish);
    expect(MaterialLocalizations.of(context).okButtonLabel, 'Tamam');
  });

  testWidgets('app follows the system dark theme', (tester) async {
    tester.binding.platformDispatcher.platformBrightnessTestValue =
        Brightness.dark;
    addTearDown(
      tester.binding.platformDispatcher.clearPlatformBrightnessTestValue,
    );

    await tester.pumpWidget(_testApp());
    await tester.pumpAndSettle();

    final context = tester.element(find.text('Kategori giderleri'));
    expect(Theme.of(context).brightness, Brightness.dark);
  });
}

Widget _testApp({ScopeController? scopeController}) {
  Widget app = BusinessFinanceApp(
    router: createAppRouter(
      transactionRepository: _ShellTransactionRepository(),
      scopeController: scopeController,
    ),
  );
  if (scopeController != null) {
    app = ChangeNotifierProvider.value(value: scopeController, child: app);
  }
  return ChangeNotifierProvider.value(
    value: testDashboardViewModel(),
    child: app,
  );
}

Finder _globalTransactionAction() => find.byWidgetPredicate(
  (widget) =>
      widget is FloatingActionButton &&
      widget.heroTag == 'main-shell-new-transaction',
);

class _ShellTransactionRepository implements TransactionRepositoryContract {
  @override
  Future<TransactionPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    TransactionFilter filter = const TransactionFilter(),
  }) async => TransactionPage(
    items: const [],
    pageNumber: pageNumber,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  );

  @override
  Future<TransactionItem> create(CreateTransactionInput input) =>
      throw UnimplementedError();

  @override
  Future<TransactionItem> cancel(String id) => throw UnimplementedError();

  @override
  Future<List<TransactionChoice>> listAccounts() async => const [];

  @override
  Future<List<TransactionChoice>> listCategories(TransactionKind kind) async =>
      const [];
}

class _ShellScopeStore implements ScopeStore {
  _ShellScopeStore({required this.hasBusiness});

  bool hasBusiness;
  TransactionScope? scope;

  @override
  Future<void> clear() async {
    hasBusiness = false;
    scope = null;
  }

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => scope;

  @override
  Future<void> writeHasBusiness(bool value) async => hasBusiness = value;

  @override
  Future<void> writeScope(TransactionScope? value) async => scope = value;
}
