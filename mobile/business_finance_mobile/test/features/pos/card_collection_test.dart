import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_detail_sheet.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_tile.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_deposit_sheets.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Kartla tahsil (ADR 0019 T5): veresiye alacağın POS'tan geçen tahsili.
/// Satış değildir, gelir yazmaz; İşlemler'de cari tahsilatın satırıdır,
/// Kasa'da POS listesinde kişi adıyla durur.
void main() {
  group('model', () {
    test('kategorisi olmayan kartla tahsil okunur ve kişiyle adlanır', () {
      final item = PosSettlementItem.fromJson({
        'id': 'pos-settlement-1',
        'accountId': 'bank-1',
        'accountName': 'Garanti Vadesiz',
        'categoryId': null,
        'categoryName': null,
        'grossAmount': '1000.0000',
        'commissionAmount': '15.0000',
        'netAmount': '985.0000',
        'currency': 'TRY',
        'scope': 'business',
        'settlementDate': '2026-10-05',
        'expectedTransferDate': '2026-10-06',
        'isInTransit': true,
        'isCancelled': false,
        'isLate': false,
        'kind': 'collection',
        'counterpartyName': 'Ahmet Bakkal',
      });

      expect(item.isCollection, isTrue);
      expect(item.categoryName, isNull);
      expect(posSettlementTitle(item), 'Ahmet Bakkal');
    });

    test('türü göndermeyen eski sunucu satış sayılır', () {
      expect(PosSettlementKind.fromApi(null), PosSettlementKind.sale);
      expect(
        () => PosSettlementKind.fromApi('refund'),
        throwsA(isA<FormatException>()),
      );
    });

    test('yatış satışla tahsili ayrı taşır; istemci çıkarma yapmaz', () {
      final deposit = PosDeposit.fromJson({
        'id': 'deposit-1',
        'accountName': 'Garanti Vadesiz',
        'depositDate': '2026-10-06',
        'expectedAmount': '985.0000',
        'depositedAmount': '985.0000',
        'deductionAmount': '0.0000',
        'currency': 'TRY',
        'isCancelled': false,
        'settlements': <Object?>[],
        'grossAmount': '1000.0000',
        'commissionAmount': '15.0000',
        'collectionAmount': '1000.0000',
        'saleAmount': '0.0000',
      });

      expect(deposit.hasCollection, isTrue);
      expect(deposit.hasSale, isFalse);
    });

    test('kartla tahsil Kasa\'yı ve bütçeyi yeniler, hesapları yenilemez', () {
      final changes = FinancialDataChanges();
      changes.cardCollectionChanged();

      expect(changes.cashRevision, 1);
      expect(changes.budgetsRevision, 1);
      expect(changes.counterpartiesRevision, 1);
      expect(changes.accountsRevision, 0);
    });
  });

  group('İşlemler', () {
    testWidgets('satır kişiyi, POS\'u ve komisyonu yazar', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: ActivityTile(activity: _collection(), showDate: false),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('Ahmet Bakkal'), findsOneWidget);
      expect(find.text('Ahmet Bakkal → Garanti POS'), findsOneWidget);
      expect(find.text('komisyon ₺15,00'), findsOneWidget);
    });

    testWidgets('ayrıntı POS\'u, neti ve beklenen günü gösterir', (
      tester,
    ) async {
      tester.view.physicalSize = const Size(412, 1400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: SingleChildScrollView(
              child: ActivityDetailSheet(
                activity: _collection(),
                balances: Future.value(const []),
              ),
            ),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('POS'), findsOneWidget);
      expect(find.text('Garanti POS'), findsOneWidget);
      expect(find.text('Net tutar'), findsOneWidget);
      expect(find.text('₺985,00'), findsOneWidget);
      expect(find.text('Beklenen'), findsOneWidget);
      expect(find.text('Karşı taraf'), findsOneWidget);
    });

    testWidgets('yatışla hesaba geçmiş tahsil önce yatışı geri aldırır', (
      tester,
    ) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: SingleChildScrollView(
              child: ActivityDetailSheet(
                activity: _collection(transferredOn: '2026-10-06'),
                balances: Future.value(const []),
              ),
            ),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.textContaining('önce Kasa\'daki yatış'), findsOneWidget);
      expect(find.text('Geçiş günü'), findsOneWidget);
    });
  });
}

FinancialActivity _collection({String? transferredOn}) => FinancialActivity(
  activityId: 'payment-1',
  kind: ActivityKind.counterpartySettlement,
  effect: ActivityEffect.neutral,
  sourceGroup: ActivitySourceGroup.counterparty,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: '2026-10-05',
  amount: '1000.0000',
  currency: 'TRY',
  title: 'Ahmet Bakkal',
  sourceName: 'Garanti Vadesiz',
  destinationName: 'Ahmet Bakkal',
  canCancel: transferredOn == null,
  supportsAttachments: false,
  channelName: 'Garanti POS',
  feeAmount: '15.0000',
  netAmount: '985.0000',
  expectedTransferDate: '2026-10-06',
  transferredOn: transferredOn,
  direction: ActivityDirection.receivable,
);
