import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_scope_selector.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_page.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';

const _unsplitNote =
    'Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır.';

void main() {
  group('okuma', () {
    test('aktif kapsam her iki rapora da iner', () async {
      final source = _RecordingDataSource();
      final viewModel = _viewModel(
        source,
        await _scopeController(
          hasBusiness: true,
          selected: TransactionScope.business,
        ),
      );

      await viewModel.load();

      expect(source.monthlyScopes, [TransactionScope.business]);
      expect(source.advancedScopes, [TransactionScope.business]);
    });

    test('Hepsi konumunda filtre gönderilmez', () async {
      final source = _RecordingDataSource();
      final viewModel = _viewModel(
        source,
        await _scopeController(hasBusiness: true),
      );

      await viewModel.load();

      expect(source.monthlyScopes, [null]);
    });

    test('işletmesi olmayan kullanıcıda filtre hiç gönderilmez', () async {
      final source = _RecordingDataSource();
      final viewModel = _viewModel(
        source,
        await _scopeController(
          hasBusiness: false,
          selected: TransactionScope.business,
        ),
      );

      await viewModel.load();

      expect(source.monthlyScopes, [null]);
      expect(viewModel.isScopeVisible, isFalse);
    });

    test('anahtar konum değiştirince ekran yeniden okunur', () async {
      final source = _RecordingDataSource();
      final viewModel = _viewModel(
        source,
        await _scopeController(hasBusiness: true),
      );
      await viewModel.load();

      await viewModel.selectScope(TransactionScope.personal);
      await Future<void>.delayed(Duration.zero);

      expect(source.monthlyScopes, [null, TransactionScope.personal]);
    });

    test('aynı konuma dokunmak ikinci bir istek üretmez', () async {
      final source = _RecordingDataSource();
      final viewModel = _viewModel(
        source,
        await _scopeController(
          hasBusiness: true,
          selected: TransactionScope.personal,
        ),
      );
      await viewModel.load();

      await viewModel.selectScope(TransactionScope.personal);
      await Future<void>.delayed(Duration.zero);

      expect(source.monthlyScopes.length, 1);
    });
  });

  group('ekran', () {
    testWidgets('işletme kullanıcısında anahtar başlığın altında durur', (
      tester,
    ) async {
      await _pumpDashboard(tester, hasBusiness: true);

      expect(find.byType(AppScopeSwitch), findsOneWidget);
    });

    testWidgets('işletmesi olmayan kullanıcıda kapsam arayüzü hiç görünmez', (
      tester,
    ) async {
      await _pumpDashboard(tester, hasBusiness: false);

      expect(find.byType(AppScopeSwitch), findsNothing);
      expect(find.text('İşletme'), findsNothing);
      expect(find.text('Şahsi'), findsNothing);
    });

    testWidgets('bölünmeyen bölüm toplam gösterdiğini yazar', (tester) async {
      await _pumpDashboard(
        tester,
        hasBusiness: true,
        selected: TransactionScope.business,
      );

      expect(find.text(_unsplitNote), findsWidgets);
    });

    testWidgets('filtre yokken toplam notu ekranı meşgul etmez', (
      tester,
    ) async {
      await _pumpDashboard(tester, hasBusiness: true);

      expect(find.text(_unsplitNote), findsNothing);
    });

    testWidgets('anahtar yükleme durumunda da yerinde durur', (tester) async {
      final viewModel = _viewModel(
        _PendingDataSource(),
        await _scopeController(hasBusiness: true),
      );
      unawaited(viewModel.load());

      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: ChangeNotifierProvider.value(
            value: viewModel,
            child: const DashboardPage(),
          ),
        ),
      );
      await tester.pump();

      expect(find.byType(AppScopeSwitch), findsOneWidget);
      expect(find.byType(CircularProgressIndicator), findsWidgets);
    });
  });
}

DashboardViewModel _viewModel(
  DashboardDataSource source,
  ScopeController scope,
) => DashboardViewModel(
  source,
  scopeController: scope,
  now: () => DateTime(2026, 8, 9),
);

Future<void> _pumpDashboard(
  WidgetTester tester, {
  required bool hasBusiness,
  TransactionScope? selected,
}) async {
  final viewModel = _viewModel(
    _RecordingDataSource(),
    await _scopeController(hasBusiness: hasBusiness, selected: selected),
  );
  await viewModel.load();

  tester.view.physicalSize = const Size(400, 1600);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: ChangeNotifierProvider.value(
        value: viewModel,
        child: const DashboardPage(),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

Future<ScopeController> _scopeController({
  required bool hasBusiness,
  TransactionScope? selected,
}) async {
  final controller = ScopeController(
    store: _MemoryStore(scope: selected, hasBusiness: hasBusiness),
  );
  await controller.ensureLoaded();
  return controller;
}

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

class _RecordingDataSource implements DashboardDataSource {
  final monthlyScopes = <TransactionScope?>[];
  final advancedScopes = <TransactionScope?>[];

  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) async {
    monthlyScopes.add(scope);
    return DashboardReport(
      year: year,
      month: month,
      totalIncome: '500.0000',
      totalExpense: '125.0000',
      net: '375.0000',
      currency: 'TRY',
      categoryExpenses: const [],
      categoryExpenseSlices: const [],
      accountBalances: const [
        AccountBalance(
          accountId: '22222222-2222-2222-2222-222222222222',
          accountName: 'Nakit',
          balance: '475.0000',
        ),
      ],
    );
  }

  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) {
    advancedScopes.add(scope);
    // Gelişmiş rapor ikincil okumadır; burada düşmesi ay özetini etkilemiyor
    // ve `Varlık durumu` bölümü çizilmiyor.
    return Future.error(Exception('gelişmiş rapor bu testte yapılandırılmadı'));
  }
}

class _PendingDataSource implements DashboardDataSource {
  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) => Completer<DashboardReport>().future;

  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) => Completer<AdvancedReport>().future;
}
