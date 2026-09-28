import 'dart:convert';

import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/cash/data/cash_repository.dart';
import 'package:business_finance_mobile/features/cash/presentation/cash_controller.dart';
import 'package:business_finance_mobile/features/cash/presentation/cash_page.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_controller.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';

import '../../helpers/accessibility.dart';

void main() {
  test(
    'kasa deposu para metnini hesaplamadan API sözleşmesine taşır',
    () async {
      late Map<String, dynamic> sent;
      final repository = CashRepository(
        _client((request) async {
          expect(request.method, 'POST');
          expect(request.url.path, '/api/v1/cash-counts');
          sent = jsonDecode(request.body) as Map<String, dynamic>;
          return http.Response(jsonEncode(_cashCountJson), 201);
        }),
      );

      final item = await repository.create(
        accountId: 'cash-account',
        countedAmount: '123456789012345.6789',
        countDate: '2026-08-24',
        scope: 'business',
        note: 'Akşam sayımı',
      );

      expect(sent['countedAmount'], '123456789012345.6789');
      expect(sent['countedAmount'], isA<String>());
      expect(sent['scope'], 'business');
      expect(item.difference, '5.0000');
      expect(
        () => CashCountItem.fromJson({..._cashCountJson, 'difference': 5.0}),
        throwsFormatException,
      );
    },
  );

  test('POS deposu brüt, komisyon ve net değerlerini ayrı okur', () async {
    final repository = PosRepository(
      _client(
        (_) async => http.Response.bytes(
          utf8.encode(
            jsonEncode({
              'items': [_posItemJson],
              'moneyInTransit': '97.5000',
              'inTransitCount': 1,
            }),
          ),
          200,
          headers: {'content-type': 'application/json; charset=utf-8'},
        ),
      ),
    );

    final result = await repository.list(inTransitOnly: true);

    expect(result.items.single.grossAmount, '100.0000');
    expect(result.items.single.commissionAmount, '2.5000');
    expect(result.items.single.netAmount, '97.5000');
    expect(result.moneyInTransit, '97.5000');
  });

  test(
    'sayım gözlemdir; yalnız açık onay finansal hedefleri yeniler',
    () async {
      final repository = _FakeCashRepository();
      final changes = FinancialDataChanges();
      final controller = CashCountController(repository, changes: changes);
      await controller.load();

      final recorded = await controller.recordCount(
        countedAmount: '105.0000',
        countDate: '2026-08-24',
        scope: TransactionScope.business,
      );

      expect(recorded, isTrue);
      expect(controller.hasOpenDifference, isTrue);
      expect(controller.differenceCategoryType, 'income');
      expect(repository.createdScope, 'business');
      expect(changes.cashRevision, 1);
      expect(changes.dashboardRevision, 0);
      expect(changes.accountsRevision, 0);
      expect(changes.budgetsRevision, 0);
      expect(changes.activityFeedRevision, 0);

      expect(await controller.confirmDifference('income-category'), isTrue);
      expect(controller.hasOpenDifference, isFalse);
      expect(changes.cashRevision, 2);
      expect(changes.dashboardRevision, 1);
      expect(changes.accountsRevision, 1);
      expect(changes.budgetsRevision, 1);
      expect(changes.activityFeedRevision, 1);
    },
  );

  test(
    'POS tahsilatı satışı tanır; geçiş yalnız hesabı hareket ettirir',
    () async {
      final repository = _FakePosRepository();
      final changes = FinancialDataChanges();
      final controller = PosController(repository, changes: changes);
      await controller.load();

      final created = await controller.create(
        accountId: 'bank-account',
        categoryId: 'income-category',
        grossAmount: '100.0000',
        settlementDate: '2026-08-24',
        expectedTransferDate: '2026-08-26',
        scope: TransactionScope.business,
        commissionRate: '2.5000',
        commissionCategoryId: 'expense-category',
      );

      expect(created, isTrue);
      expect(repository.created?['grossAmount'], '100.0000');
      expect(repository.created?['commissionAmount'], isNull);
      expect(repository.created?['commissionRate'], '2.5000');
      expect(changes.cashRevision, 1);
      expect(changes.dashboardRevision, 1);
      expect(changes.budgetsRevision, 1);
      expect(changes.accountsRevision, 0);
      // Tahsilat feed'de iki satır doğuruyor (satış ve komisyon), bu yüzden
      // feed de yenilenmeli. Yenilenmeseydi kullanıcı İşlemler'e geçtiğinde
      // az önce girdiği satışı göremezdi — cihaz kabulünde tam olarak bu oldu.
      expect(changes.activityFeedRevision, 1);

      expect(
        await controller.markTransferred(
          repository.current.items.single,
          '2026-08-25',
        ),
        isTrue,
      );
      expect(repository.transferDate, '2026-08-25');
      expect(changes.cashRevision, 2);
      expect(changes.dashboardRevision, 2);
      expect(changes.accountsRevision, 1);
      // Geçiş hiçbir gider tanımaz: bütçe yükselmez, yoksa aynı satış iki kez
      // sayılırdı. Feed ise yükselir; geçiş kendi satırını doğuruyor.
      expect(changes.budgetsRevision, 1);
      expect(changes.activityFeedRevision, 2);
    },
  );

  // `İşlem ekle > POS tahsilatı` Kasa'ya gelir ve formu doğrudan açar. Ekran
  // sayımla açılıp kalsaydı menüden gelen kullanıcı `+ Ekle`yi elle bulmak
  // zorunda kalır ve menü yolun yarısında bırakırdı.
  testWidgets('POS yolu açılışta tahsilat formunu açar', (tester) async {
    await tester.pumpWidget(_app(initialTab: 1));
    await tester.pumpAndSettle();

    expect(find.text('POS tahsilatı'), findsOneWidget);
  });

  // Menüden açılan Kasa alt çubuğun dışında, ayrı bir sayfadır. Vazgeçen
  // kullanıcı orada kalsaydı alt çubuksuz bir ekranda ve geri oksuz kalırdı
  // (cihaz kabulü, 28 Eylül 2026).
  testWidgets('POS formundan vazgeçen kullanıcı geldiği ekrana döner', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Builder(
          builder: (context) => Scaffold(
            body: TextButton(
              onPressed: () => Navigator.of(context).push(
                MaterialPageRoute<void>(
                  builder: (_) => CashPage(
                    cashController: CashCountController(
                      _FakeCashRepository(),
                      clock: () => DateTime(2026, 9, 25, 18),
                    ),
                    posController: PosController(_FakePosRepository()),
                    initialTab: 1,
                  ),
                ),
              ),
              child: const Text('Başlangıç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('Başlangıç'));
    await tester.pumpAndSettle();
    expect(find.text('POS tahsilatı'), findsOneWidget);

    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();

    expect(find.byType(CashPage), findsNothing);
    expect(find.text('Başlangıç'), findsOneWidget);
  });

  testWidgets('ayrı sayfa olarak açılan Kasa geri ok taşır', (tester) async {
    await tester.pumpWidget(_app());
    await tester.pumpAndSettle();
    // Kök sayfada (ana sekme) geri gidilecek yer yok.
    expect(find.byTooltip('Geri'), findsNothing);

    final navigator = tester.state<NavigatorState>(find.byType(Navigator));
    navigator.push(
      MaterialPageRoute<void>(
        builder: (_) => CashPage(
          cashController: CashCountController(
            _FakeCashRepository(),
            clock: () => DateTime(2026, 9, 25, 18),
          ),
          posController: PosController(_FakePosRepository()),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.byTooltip('Geri'), findsOneWidget);
    await tester.tap(find.byTooltip('Geri'));
    await tester.pumpAndSettle();
    expect(find.byType(CashPage), findsOneWidget);
  });

  testWidgets('Kasa tek akıştır: sayım, yoldaki POS ve son sayımlar', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(412, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeCashRepository()..previous = _previousCount;
    await tester.pumpWidget(_app(cash: repository));
    await tester.pumpAndSettle();

    expect(find.byType(TabBar), findsNothing);
    expect(find.text('25 Eylül Cuma'), findsOneWidget);
    expect(find.text('Sayılmadı'), findsOneWidget);
    expect(find.text('Dünkü sayım'), findsOneWidget);
    expect(find.text('+₺2.450,00'), findsOneWidget);
    expect(find.text('-₺665,00'), findsOneWidget);
    expect(find.text('POS tahsilatları'), findsOneWidget);
    expect(find.text('Son sayımlar'), findsOneWidget);
    // Geçmiş satırı sunucunun o anki gözlemini okur: beklenen ve kaydedilen
    // fark.
    expect(find.text('Beklenen ₺21.520,00'), findsOneWidget);
    expect(find.text('Eksik · kaydedildi'), findsOneWidget);
  });

  testWidgets('banknotla sayım tam aritmetikle toplanır ve farkı önizler', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(412, 1600);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeCashRepository();
    await tester.pumpWidget(_app(cash: repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Sayımı gir'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Banknotla say'));
    await tester.pumpAndSettle();

    // 100 TL beklenen; 50 + 20 + 20 + 5 + 0,10 = 95,10 → 4,90 eksik.
    for (final note in [50, 20, 20, 5]) {
      await tester.tap(find.byTooltip('$note lira artır'));
      await tester.pump();
    }
    await tester.enterText(find.byType(TextField), '0,10');
    await tester.pumpAndSettle();
    expect(find.text('₺4,90 eksik'), findsOneWidget);

    await tester.tap(find.text('Sayımı kaydet'));
    await tester.pumpAndSettle();
    expect(repository.createdAmount, '95.1000');
    expect(repository.createdDate, '2026-09-25');
  });

  testWidgets('toplam alanı Türkçe binlik ayırıcıyla yazılır', (tester) async {
    tester.view.physicalSize = const Size(412, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeCashRepository();
    await tester.pumpWidget(_app(cash: repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Sayımı gir'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextField).first, '23100');
    await tester.pumpAndSettle();
    expect(find.text('23.100'), findsOneWidget);
    expect(find.text('₺23.000,00 fazla'), findsOneWidget);

    await tester.tap(find.text('Sayımı kaydet'));
    await tester.pumpAndSettle();
    expect(repository.createdAmount, '23100.0000');
  });

  testWidgets('Kasa büyük metin erişilebilirlik kapısını geçer', (
    tester,
  ) async {
    final repository = _FakeCashRepository()..previous = _previousCount;
    await pumpAtLargestTextScale(
      tester,
      _app(cash: repository, twoAccounts: true),
      surfaceSize: const Size(400, 1000),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);

    await tester.ensureVisible(find.text('Sayımı gir'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Sayımı gir'));
    await tester.pumpAndSettle();
    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);

    await tester.tap(find.text('Banknotla say'));
    await tester.pumpAndSettle();
    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

Widget _app({
  _FakeCashRepository? cash,
  int initialTab = 0,
  bool twoAccounts = false,
}) {
  final repository = (cash ?? _FakeCashRepository())..twoAccounts = twoAccounts;
  return MaterialApp(
    theme: AppTheme.light(),
    home: CashPage(
      cashController: CashCountController(
        repository,
        clock: () => DateTime(2026, 9, 25, 18),
      ),
      posController: PosController(_FakePosRepository()),
      initialTab: initialTab,
    ),
  );
}

final _previousCount = CashCountItem.fromJson({
  ..._cashCountJson,
  'id': 'previous',
  'countDate': '2026-09-24',
  'countedAmount': '21400.0000',
  'expectedBalance': '21520.0000',
  'difference': '-120.0000',
  'adjustmentTransactionId': 'adjustment',
});

ApiClient _client(MockClientHandler handler) => ApiClient(
  config: ApiConfig.fromEnvironment(value: 'https://api.test'),
  httpClient: MockClient(handler),
);

class _FakeCashRepository implements CashRepositoryContract {
  CashCountItem? currentCount;
  CashCountItem? previous;
  String? createdScope;
  String? createdAmount;
  String? createdDate;
  bool twoAccounts = false;

  @override
  Future<List<CashAccount>> loadCashAccounts() async => [
    const CashAccount(
      id: 'cash-account',
      name: 'Merkez kasa',
      defaultScope: TransactionScope.business,
    ),
    if (twoAccounts)
      const CashAccount(
        id: 'wallet',
        name: 'Şahsi cüzdan',
        defaultScope: TransactionScope.personal,
      ),
  ];

  @override
  Future<CashCountToday> loadToday({required String accountId}) async =>
      CashCountToday(
        accountId: accountId,
        accountName: 'Merkez kasa',
        expectedBalance: '100.0000',
        currency: 'TRY',
        count: currentCount,
        previousCount: previous,
        todayInflow: '2450.0000',
        todayOutflow: '665.0000',
      );

  @override
  Future<List<CashCountItem>> list({required String accountId}) async => [
    ?currentCount,
    ?previous,
  ];

  @override
  Future<CashCountItem> create({
    required String accountId,
    required String countedAmount,
    required String countDate,
    String? scope,
    String? note,
  }) async {
    createdScope = scope;
    createdAmount = countedAmount;
    createdDate = countDate;
    return currentCount = _cashCount(difference: '5.0000');
  }

  @override
  Future<List<DataChoice>> loadCategories({required String type}) async =>
      const [DataChoice('income-category', 'Kasa farkı')];

  @override
  Future<CashCountItem> confirmDifference({
    required String cashCountId,
    required String categoryId,
  }) async => currentCount = _cashCount(
    difference: '5.0000',
    adjustmentTransactionId: 'adjustment',
  );
}

class _FakePosRepository implements PosRepositoryContract {
  PosSettlementList current = const PosSettlementList(
    items: [],
    moneyInTransit: '0.0000',
    inTransitCount: 0,
  );
  Map<String, Object?>? created;
  String? transferDate;

  @override
  Future<PosSettlementList> list({required bool inTransitOnly}) async =>
      current;

  @override
  Future<PosOptions> loadOptions() async => const PosOptions(
    accounts: [DataChoice('bank-account', 'Banka')],
    incomeCategories: [DataChoice('income-category', 'Satış')],
    expenseCategories: [DataChoice('expense-category', 'POS komisyonu')],
  );

  @override
  Future<void> create(Map<String, Object?> input) async {
    created = input;
    current = PosSettlementList(
      items: [PosSettlementItem.fromJson(_posItemJson)],
      moneyInTransit: '97.5000',
      inTransitCount: 1,
    );
  }

  @override
  Future<void> markTransferred({
    required String settlementId,
    required String transferDate,
  }) async {
    this.transferDate = transferDate;
    current = PosSettlementList(
      items: [
        PosSettlementItem.fromJson({
          ..._posItemJson,
          'transferredOn': transferDate,
          'isInTransit': false,
        }),
      ],
      moneyInTransit: '0.0000',
      inTransitCount: 0,
    );
  }
}

CashCountItem _cashCount({
  required String difference,
  String? adjustmentTransactionId,
}) => CashCountItem.fromJson({
  ..._cashCountJson,
  'difference': difference,
  'adjustmentTransactionId': adjustmentTransactionId,
});

const _cashCountJson = <String, Object?>{
  'id': 'cash-count',
  'accountId': 'cash-account',
  'accountName': 'Merkez kasa',
  'countDate': '2026-08-24',
  'countedAmount': '105.0000',
  'currency': 'TRY',
  'scope': 'business',
  'note': null,
  'adjustmentTransactionId': null,
  'expectedBalance': '100.0000',
  'difference': '5.0000',
  'isCancelled': false,
};

const _posItemJson = <String, Object?>{
  'id': 'pos-settlement',
  'accountId': 'bank-account',
  'accountName': 'Banka',
  'categoryId': 'income-category',
  'categoryName': 'Satış',
  'commissionCategoryId': 'expense-category',
  'commissionCategoryName': 'POS komisyonu',
  'grossAmount': '100.0000',
  'commissionAmount': '2.5000',
  'netAmount': '97.5000',
  'commissionRate': '2.5000',
  'currency': 'TRY',
  'scope': 'business',
  'settlementDate': '2026-08-24',
  'expectedTransferDate': '2026-08-26',
  'transferredOn': null,
  'description': 'Tezgâh satışı',
  'isInTransit': true,
  'isCancelled': false,
  'isLate': false,
};
