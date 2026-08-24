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
      expect(changes.activityFeedRevision, 0);

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
      expect(changes.budgetsRevision, 1);
      expect(changes.activityFeedRevision, 0);
    },
  );

  testWidgets(
    'Kasa iki alt ekranıyla büyük metin erişilebilirlik kapısını geçer',
    (tester) async {
      final cashController = CashCountController(_FakeCashRepository());
      final posController = PosController(_FakePosRepository());
      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: CashPage(
            cashController: cashController,
            posController: posController,
          ),
        ),
        surfaceSize: const Size(400, 1000),
      );

      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);

      await tester.tap(find.text('POS tahsilatları'));
      await tester.pumpAndSettle();
      expect(find.textContaining('Yoldaki para sizindir'), findsOneWidget);
      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    },
  );
}

ApiClient _client(MockClientHandler handler) => ApiClient(
  config: ApiConfig.fromEnvironment(value: 'https://api.test'),
  httpClient: MockClient(handler),
);

class _FakeCashRepository implements CashRepositoryContract {
  CashCountItem? currentCount;
  String? createdScope;

  @override
  Future<List<CashAccount>> loadCashAccounts() async => const [
    CashAccount(
      id: 'cash-account',
      name: 'Merkez kasa',
      defaultScope: TransactionScope.business,
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
      );

  @override
  Future<List<CashCountItem>> list({required String accountId}) async =>
      currentCount == null ? const [] : [currentCount!];

  @override
  Future<CashCountItem> create({
    required String accountId,
    required String countedAmount,
    required String countDate,
    String? scope,
    String? note,
  }) async {
    createdScope = scope;
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
