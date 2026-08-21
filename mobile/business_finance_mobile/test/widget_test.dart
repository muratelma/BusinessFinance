import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/app/business_finance_app.dart';
import 'package:business_finance_mobile/core/localization/app_locale.dart';
import 'package:business_finance_mobile/core/routing/app_router.dart';
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

Widget _testApp() => ChangeNotifierProvider.value(
  value: testDashboardViewModel(),
  child: BusinessFinanceApp(
    router: createAppRouter(
      transactionRepository: _ShellTransactionRepository(),
    ),
  ),
);

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
