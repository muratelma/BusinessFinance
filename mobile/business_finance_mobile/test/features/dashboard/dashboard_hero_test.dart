import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_metric_tile.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_page.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';
import 'package:business_finance_mobile/features/planning/data/planning_models.dart';

import '../../helpers/accessibility.dart';

void main() {
  group('model', () {
    test('kırılım sunucudan geldiği gibi okunur', () {
      final report = DashboardReport.fromJson(
        _reportJson(
          scopeBreakdown: {
            'business': {
              'income': '600.0000',
              'expense': '200.0000',
              'net': '400.0000',
            },
            'personal': {
              'income': '0.0000',
              'expense': '50.0000',
              'net': '-50.0000',
            },
          },
        ),
      );

      expect(report.scopeBreakdown?.business.net, '400.0000');
      expect(report.scopeBreakdown?.personal.expense, '50.0000');
    });

    test('kırılım yoksa alan boş kalır, sıfır uydurulmaz', () {
      final report = DashboardReport.fromJson(_reportJson());

      expect(report.scopeBreakdown, isNull);
    });
  });

  group('hero', () {
    testWidgets('işletme kullanıcısında üç sayı birlikte ve tutarlı', (
      tester,
    ) async {
      await _pumpDashboard(tester, hasBusiness: true);

      expect(find.text('İşletme neti'), findsOneWidget);
      expect(find.text('Şahsi çekim'), findsOneWidget);
      expect(find.text('Bu ayın neti'), findsOneWidget);

      final business = _amount(tester, 'dashboard-summary-Net');
      final personal = _amount(tester, 'dashboard-summary-Personal');
      final total = _amount(tester, 'dashboard-summary-Total');

      expect(business, '400.0000');
      expect(personal, '-50.0000');
      expect(total, '350.0000');
      // Üç sayı birbirini tutuyor. Toplamı istemci hesaplamıyor — üçü de
      // sunucudan geliyor — ama ekranda yan yana durdukları için tutmaları
      // şart: tutmasalardı kullanıcı hangisine güveneceğini bilemezdi.
      expect(
        _parse(business) + _parse(personal),
        closeTo(_parse(total), 0.0001),
      );
    });

    testWidgets('"kâr" kelimesi ekranda geçmez', (tester) async {
      await _pumpDashboard(tester, hasBusiness: true);

      // Ürün sınırı: hesaplanan şey nakit esaslı işletme netidir. Muhasebe
      // kârı satılan malın maliyetini ister; yanlış kelime kullanıcıyı vergi
      // beyanında yanıltır. Aranan şapkalı `â` yalnız bu kelimede geçiyor.
      expect(find.textContaining('âr', findRichText: true), findsNothing);
    });

    testWidgets('şahsi taraf artıdaysa "çekim" denmez', (tester) async {
      await _pumpDashboard(
        tester,
        hasBusiness: true,
        personal: const ScopeTotals(
          income: '900.0000',
          expense: '50.0000',
          net: '850.0000',
        ),
      );

      expect(find.text('Şahsi net'), findsOneWidget);
      expect(find.text('Şahsi çekim'), findsNothing);
    });

    testWidgets('işletmesi olmayan kullanıcıda ekran bugünkü hâlini korur', (
      tester,
    ) async {
      await _pumpDashboard(tester, hasBusiness: false);

      expect(find.text('Bu ayın neti'), findsOneWidget);
      expect(find.text('İşletme neti'), findsNothing);
      expect(find.text('Şahsi çekim'), findsNothing);
      expect(_amount(tester, 'dashboard-summary-Net'), '350.0000');
    });

    testWidgets('bir taraf seçiliyken hero hangi tarafı okuduğunu yazar', (
      tester,
    ) async {
      await _pumpDashboard(
        tester,
        hasBusiness: true,
        selected: TransactionScope.business,
      );

      expect(find.text('İşletme neti'), findsOneWidget);
      // Filtreli okumada kırılım gelmiyor; ikinci ve üçüncü sayı da yok.
      expect(find.text('Şahsi çekim'), findsNothing);
      expect(find.text('Bu ayın neti'), findsNothing);
      expect(find.text('Yalnız işletme tarafı'), findsOneWidget);
    });

    testWidgets('üç sayılı hero en büyük yazı ölçeğinde taşmaz', (
      tester,
    ) async {
      await _pumpDashboard(
        tester,
        hasBusiness: true,
        textScale: 2,
        surfaceSize: const Size(400, 3000),
      );

      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    });
  });
}

String _amount(WidgetTester tester, String key) =>
    tester.widget<AppMetricTile>(find.byKey(ValueKey(key))).amount;

double _parse(String amount) => double.parse(amount);

Future<void> _pumpDashboard(
  WidgetTester tester, {
  required bool hasBusiness,
  TransactionScope? selected,
  ScopeTotals? personal,
  double textScale = 1,
  Size surfaceSize = const Size(400, 1600),
}) async {
  final controller = ScopeController(
    store: _MemoryStore(scope: selected, hasBusiness: hasBusiness),
  );
  await controller.ensureLoaded();
  final viewModel = DashboardViewModel(
    _FakeDataSource(personal: personal),
    scopeController: controller,
    now: () => DateTime(2026, 8, 9),
  );
  await viewModel.load();

  tester.view.physicalSize = surfaceSize;
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(
    MediaQuery(
      data: MediaQueryData(textScaler: TextScaler.linear(textScale)),
      child: MaterialApp(
        theme: AppTheme.light(),
        home: ChangeNotifierProvider.value(
          value: viewModel,
          child: const DashboardPage(),
        ),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

Map<String, dynamic> _reportJson({Map<String, dynamic>? scopeBreakdown}) => {
  'year': 2026,
  'month': 8,
  'totalIncome': '600.0000',
  'totalExpense': '250.0000',
  'net': '350.0000',
  'currency': 'TRY',
  'categoryExpenses': <dynamic>[],
  'categoryExpenseSlices': <dynamic>[],
  'accountBalances': <dynamic>[],
  'scopeBreakdown': ?scopeBreakdown,
};

class _MemoryStore implements ScopeStore {
  _MemoryStore({this.scope, this.hasBusiness});

  TransactionScope? scope;
  bool? hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => scope;

  @override
  Future<void> writeScope(TransactionScope? value) async => scope = value;

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async => hasBusiness = value;

  @override
  Future<void> clear() async {
    scope = null;
    hasBusiness = null;
  }
}

class _FakeDataSource implements DashboardDataSource {
  _FakeDataSource({this.personal});

  final ScopeTotals? personal;

  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) async {
    final personalSide =
        personal ??
        const ScopeTotals(
          income: '0.0000',
          expense: '50.0000',
          net: '-50.0000',
        );
    return DashboardReport(
      year: year,
      month: month,
      totalIncome: '600.0000',
      totalExpense: '250.0000',
      net: '350.0000',
      currency: 'TRY',
      categoryExpenses: const [],
      categoryExpenseSlices: const [],
      accountBalances: const [],
      // Sunucu kırılımı yalnız filtresiz okumada gönderiyor; sahte kaynak da
      // aynı sözleşmeye uyuyor.
      scopeBreakdown: scope != null
          ? null
          : MonthlyScopeBreakdown(
              business: const ScopeTotals(
                income: '600.0000',
                expense: '200.0000',
                net: '400.0000',
              ),
              personal: personalSide,
            ),
    );
  }

  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) => Future.error(Exception('gelişmiş rapor bu testte yapılandırılmadı'));
}
