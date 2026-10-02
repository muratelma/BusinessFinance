import 'dart:async';
import 'dart:convert';

import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_detail_block.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_detail_sheet.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_tile.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';

import '../../helpers/accessibility.dart';

/// İşlemler'de bir kaydın **parçası** ayrı satır olmaz (2 Ekim 2026 emülatör
/// turu): komisyon satışın, kesinti yatışın satırında ve ayrıntısında
/// görünür. Başlık hesap adına düşmez; işlem sonrası bakiye ayrıntıdadır.
void main() {
  group('satır', () {
    testWidgets('POS satışı komisyonunu tutarın altında taşır', (tester) async {
      await _pumpTile(tester, _sale());

      expect(find.text('Uc kisilik aksam yemegi'), findsOneWidget);
      // Sol alt satır kısa kalır: kategori ve POS; hesap ayrıntıdadır.
      expect(find.text('Satış geliri • Yemek kartı'), findsOneWidget);
      expect(find.text('komisyon ₺97,50'), findsOneWidget);
      expect(find.textContaining('Ziraat Vadesiz'), findsNothing);
    });

    testWidgets('komisyonsuz satış ikinci satır çizmez', (tester) async {
      await _pumpTile(tester, _sale(fee: null));

      expect(find.textContaining('komisyon'), findsNothing);
    });

    testWidgets('POS seçilmeden girilen satış hesabını yazar', (tester) async {
      await _pumpTile(tester, _sale(channel: null));

      expect(find.text('Satış geliri • Ziraat Vadesiz'), findsOneWidget);
    });

    testWidgets(
      'yatış türünün adıyla, kesintisi ve tahsilat sayısıyla okunur',
      (tester) async {
        await _pumpTile(tester, _deposit(fee: '14.0000', count: 2));

        // Başlık boş gelir; hesabın adı değil türün adı yazılır.
        expect(find.text('POS yatışı'), findsOneWidget);
        expect(
          find.text('Yemek kartı • Ziraat Vadesiz • 2 tahsilat'),
          findsOneWidget,
        );
        expect(find.text('kesinti ₺14,00'), findsOneWidget);
      },
    );

    testWidgets('tek tahsilatlı, kesintisiz yatış yalın kalır', (tester) async {
      await _pumpTile(tester, _deposit(channel: null));

      expect(find.text('Ziraat Vadesiz'), findsOneWidget);
      expect(find.textContaining('kesinti'), findsNothing);
      expect(find.textContaining('tahsilat'), findsNothing);
    });

    testWidgets('açıklamasız transfer ve kart ödemesi türünün adını taşır', (
      tester,
    ) async {
      await _pumpTile(
        tester,
        _activity(
          kind: ActivityKind.transfer,
          effect: ActivityEffect.neutral,
          source: 'Banka',
          destination: 'Kasa',
        ),
      );
      expect(find.text('Transfer'), findsOneWidget);
      expect(find.text('Banka → Kasa'), findsOneWidget);

      await _pumpTile(
        tester,
        _activity(
          kind: ActivityKind.cardPayment,
          effect: ActivityEffect.neutral,
          source: 'Banka',
          destination: 'Bonus',
        ),
      );
      expect(find.text('Kart ödemesi'), findsOneWidget);
      expect(find.text('Banka → Bonus'), findsOneWidget);
    });

    testWidgets('en uzun adla ve en büyük yazıda taşmaz', (tester) async {
      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: Column(
              children: [
                ActivityTile(
                  showDate: false,
                  activity: _sale(
                    title:
                        'Dugun organizasyonu icin uc yuz kisilik yemek ve '
                        'pasta siparisi on odemesi',
                    channel: 'Yemek karti ve restoran cekleri POS cihazi',
                  ),
                ),
                ActivityTile(
                  showDate: false,
                  activity: _deposit(fee: '14.0000', count: 12),
                ),
              ],
            ),
          ),
        ),
      );

      expectNoOverflow(tester);
    });
  });

  group('ayrıntı', () {
    testWidgets(
      'POS satışı komisyonu, neti ve geçiş gününü tek panelde söyler',
      (tester) async {
        await _pumpSheet(tester, _sale(transferredOn: '2026-10-02'));

        expect(find.text('POS'), findsOneWidget);
        expect(find.text('Yemek kartı'), findsOneWidget);
        expect(find.text('Komisyon'), findsOneWidget);
        expect(find.text('Net tutar'), findsOneWidget);
        expect(find.text('Geçiş günü'), findsOneWidget);
        expect(find.text('Hesap'), findsOneWidget);
        expect(
          find.text(
            "POS tahsilatı Kasa'daki POS tahsilatlarından iptal edilir.",
          ),
          findsOneWidget,
        );
      },
    );

    testWidgets('yoldaki satış beklenen günü söyler', (tester) async {
      await _pumpSheet(tester, _sale());

      expect(find.text('Net tutar'), findsOneWidget);
      expect(find.text('Beklenen'), findsOneWidget);
      expect(find.text('Geçiş günü'), findsNothing);
    });

    // Kullanıcı kararı (2 Ekim 2026): ayrıntı kalabalıktı.
    testWidgets('köken satırı yok', (tester) async {
      await _pumpSheet(tester, _activity());

      expect(find.text('Köken'), findsNothing);
      expect(find.text('Elle eklendi'), findsNothing);
      expect(find.text('Tarih'), findsOneWidget);
    });

    testWidgets('bakiye yerinde bekler, cevap gelince dolar', (tester) async {
      final pending = Completer<List<ActivityBalance>>();
      await _pumpSheet(tester, _activity(), balances: pending.future);

      // Satır baştan yerindedir; panel sonradan büyümez.
      expect(find.text('Bakiye'), findsOneWidget);
      expect(find.text('…'), findsOneWidget);
      final before = tester.getSize(find.byType(ActivityDetailSheet));

      pending.complete(const [
        ActivityBalance(
          isCard: false,
          name: 'Banka',
          balance: '900.0000',
          currency: 'TRY',
          change: ActivityBalanceChange.decreased,
        ),
      ]);
      await tester.pumpAndSettle();

      expect(find.text('…'), findsNothing);
      expect(find.text('Bakiye'), findsOneWidget);
      expect(tester.getSize(find.byType(ActivityDetailSheet)), before);
    });

    testWidgets('transfer iki hesabın bakiyesini adlarıyla yazar', (
      tester,
    ) async {
      await _pumpSheet(
        tester,
        _activity(
          kind: ActivityKind.transfer,
          effect: ActivityEffect.neutral,
          source: 'Banka',
          destination: 'Kasa',
        ),
        balances: Future.value(const [
          ActivityBalance(
            isCard: false,
            name: 'Banka',
            balance: '750.0000',
            currency: 'TRY',
            change: ActivityBalanceChange.decreased,
          ),
          ActivityBalance(
            isCard: false,
            name: 'Kasa',
            balance: '1200.0000',
            currency: 'TRY',
            change: ActivityBalanceChange.increased,
          ),
        ]),
      );

      expect(find.text('Bakiye'), findsNWidgets(2));
      expect(find.text('Banka · ₺750,00'), findsOneWidget);
      expect(find.text('Kasa · ₺1.200,00'), findsOneWidget);
    });

    testWidgets('kart ödemesi hesabın bakiyesini ve kartın borcunu ayırır', (
      tester,
    ) async {
      await _pumpSheet(
        tester,
        _activity(
          kind: ActivityKind.cardPayment,
          effect: ActivityEffect.neutral,
          source: 'Banka',
          destination: 'Bonus',
        ),
        balances: Future.value(const [
          ActivityBalance(
            isCard: false,
            name: 'Banka',
            balance: '630.0000',
            currency: 'TRY',
            change: ActivityBalanceChange.decreased,
          ),
          ActivityBalance(
            isCard: true,
            name: 'Bonus',
            balance: '180.0000',
            currency: 'TRY',
            change: ActivityBalanceChange.decreased,
          ),
        ]),
      );

      expect(find.text('Bakiye'), findsOneWidget);
      expect(find.text('Kart borcu'), findsOneWidget);
    });

    testWidgets('bilinmeyen bakiye satır bırakmaz', (tester) async {
      // Giriş anı tutulmadan önce yazılmış kayıt: sunucu boş liste döner.
      await _pumpSheet(tester, _activity(), balances: Future.value(const []));
      expect(find.text('Bakiye'), findsNothing);

      // Okuma düşerse ayrıntı geri kalanıyla açılır.
      final failing = Completer<List<ActivityBalance>>();
      await _pumpSheet(tester, _activity(), balances: failing.future);
      failing.completeError(const FormatException());
      await tester.pump();
      expect(find.text('Bakiye'), findsNothing);
      expect(find.text('Tarih'), findsOneWidget);
    });

    testWidgets('iptal edilmiş hareket bakiye beklemez', (tester) async {
      final never = Completer<List<ActivityBalance>>().future;
      await _pumpSheet(tester, _activity(cancelled: true), balances: never);
      expect(find.text('Bakiye'), findsNothing);
    });

    testWidgets('bakiyenin rengi paranın yönünü söyler', (tester) async {
      final colors = AppTheme.light().extension<AppFinanceColors>()!;
      Color? balanceColor() => tester
          .widget<Text>(
            find.descendant(
              of: find.widgetWithText(AppDetailRow, 'Bakiye'),
              matching: find.textContaining('₺'),
            ),
          )
          .style
          ?.color;
      ActivityBalance bank(ActivityBalanceChange change) => ActivityBalance(
        isCard: false,
        name: 'Banka',
        balance: '900.0000',
        currency: 'TRY',
        change: change,
      );

      // Para hesaptan çıktı: kırmızı.
      await _pumpSheet(
        tester,
        _activity(),
        balances: Future.value([bank(ActivityBalanceChange.decreased)]),
      );
      expect(balanceColor(), colors.expense);

      // Para hesaba girdi: yeşil.
      await _pumpSheet(
        tester,
        _activity(effect: ActivityEffect.income),
        balances: Future.value([bank(ActivityBalanceChange.increased)]),
      );
      expect(balanceColor(), colors.income);

      // POS satışı gelir yazar ama hesaba dokunmaz: bakiye mavi.
      await _pumpSheet(
        tester,
        _sale(),
        balances: Future.value([bank(ActivityBalanceChange.unchanged)]),
      );
      expect(find.text('Bakiye'), findsOneWidget);
      expect(balanceColor(), colors.neutral);
    });

    testWidgets('en büyük yazıda taşmaz ve erişilebilirlik kapısını geçer', (
      tester,
    ) async {
      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: SingleChildScrollView(
              child: ActivityDetailSheet(
                activity: _sale(transferredOn: '2026-10-02'),
              ),
            ),
          ),
        ),
        surfaceSize: const Size(412, 1600),
      );
      expectNoOverflow(tester);

      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: SingleChildScrollView(
              child: ActivityDetailSheet(
                activity: _activity(
                  kind: ActivityKind.cardPayment,
                  effect: ActivityEffect.neutral,
                  source: 'Banka',
                  destination: 'Bonus',
                ),
                balances: Future.value(const [
                  ActivityBalance(
                    isCard: false,
                    name: 'Banka',
                    balance: '123456789.5000',
                    currency: 'TRY',
                    change: ActivityBalanceChange.decreased,
                  ),
                  ActivityBalance(
                    isCard: true,
                    name: 'Bonus',
                    balance: '123456789.5000',
                    currency: 'TRY',
                    change: ActivityBalanceChange.decreased,
                  ),
                ]),
              ),
            ),
          ),
        ),
        surfaceSize: const Size(412, 1600),
      );
      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    });
  });

  group('depo ve model', () {
    test('satır alanlarını okur; yoksa boş bırakır', () {
      final sale = FinancialActivity.fromJson({
        ..._json,
        'activityKind': 'pos-sale',
        'channelName': 'Yemek kartı',
        'feeAmount': '97.5000',
        'netAmount': '1402.5000',
        'expectedTransferDate': '2026-10-22',
        'transferredOn': null,
        'settlementCount': null,
      });
      expect(sale.channelName, 'Yemek kartı');
      expect(sale.feeAmount, '97.5000');
      expect(sale.hasFee, isTrue);
      expect(sale.netAmount, '1402.5000');
      expect(sale.expectedTransferDate, '2026-10-22');
      expect(sale.transferredOn, isNull);

      final plain = FinancialActivity.fromJson(_json);
      expect(plain.hasFee, isFalse);
      expect(plain.channelName, isNull);
      expect(plain.settlementCount, isNull);
      // Sıfır tutarlı parça gösterilmez.
      expect(
        FinancialActivity.fromJson({..._json, 'feeAmount': '0.0000'}).hasFee,
        isFalse,
      );
    });

    test('bakiye hareketin türü ve kimliğiyle okunur', () async {
      late String path;
      final repository = ActivityRepository(
        ApiClient(
          config: ApiConfig.fromEnvironment(value: 'https://api.test'),
          httpClient: MockClient((request) async {
            path = request.url.path;
            return http.Response.bytes(
              utf8.encode(
                jsonEncode({
                  'items': [
                    {
                      'holder': 'account',
                      'id': 'a',
                      'name': 'Banka',
                      'balance': '630.0000',
                      'currency': 'TRY',
                      'change': 'decreased',
                    },
                    {
                      'holder': 'credit-card',
                      'id': 'c',
                      'name': 'Bonus',
                      'balance': '180.0000',
                      'currency': 'TRY',
                      'change': 'decreased',
                    },
                  ],
                }),
              ),
              200,
              headers: {'content-type': 'application/json; charset=utf-8'},
            );
          }),
        ),
      );

      final balances = await repository.balancesAfter(
        _activity(kind: ActivityKind.cardPayment),
      );

      expect(
        path,
        '/api/v1/financial-activities/card-payment/activity-1/balances',
      );
      expect(balances.map((item) => item.isCard), [false, true]);
      // Para dört basamaklı metin olarak kalır.
      expect(balances.first.balance, '630.0000');
      expect(balances.map((item) => item.change), [
        ActivityBalanceChange.decreased,
        ActivityBalanceChange.decreased,
      ]);
      expect(
        () => ActivityBalance.fromJson({
          'holder': 'wallet',
          'name': 'x',
          'balance': '1.0000',
          'currency': 'TRY',
          'change': 'increased',
        }),
        throwsFormatException,
      );
      expect(
        () => ActivityBalance.fromJson({
          'holder': 'account',
          'name': 'x',
          'balance': '1.0000',
          'currency': 'TRY',
          'change': 'sideways',
        }),
        throwsFormatException,
      );
    });
  });
}

Future<void> _pumpTile(WidgetTester tester, FinancialActivity activity) async {
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(body: ActivityTile(activity: activity, showDate: false)),
    ),
  );
  await tester.pumpAndSettle();
}

Future<void> _pumpSheet(
  WidgetTester tester,
  FinancialActivity activity, {
  Future<List<ActivityBalance>>? balances,
}) async {
  tester.view.physicalSize = const Size(412, 1400);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(
        body: SingleChildScrollView(
          // Her çağrıda yeni panel: önceki testin `FutureBuilder` durumu
          // taşınmasın.
          key: UniqueKey(),
          child: ActivityDetailSheet(activity: activity, balances: balances),
        ),
      ),
    ),
  );
  await tester.pump();
  await tester.pump();
}

FinancialActivity _activity({
  ActivityKind kind = ActivityKind.accountTransaction,
  ActivityEffect effect = ActivityEffect.expense,
  String title = '',
  String? source = 'Banka',
  String? destination,
  bool cancelled = false,
}) => FinancialActivity(
  activityId: 'activity-1',
  kind: kind,
  effect: effect,
  sourceGroup: ActivitySourceGroup.account,
  origin: ActivityOrigin.manual,
  status: cancelled ? ActivityStatus.cancelled : ActivityStatus.realized,
  activityDate: '2026-10-02',
  amount: '100.0000',
  currency: 'TRY',
  title: title,
  sourceName: source,
  destinationName: destination,
  canCancel: false,
  supportsAttachments: false,
);

FinancialActivity _sale({
  String title = 'Uc kisilik aksam yemegi',
  String? channel = 'Yemek kartı',
  String? fee = '97.5000',
  String? transferredOn,
}) => FinancialActivity(
  activityId: 'sale-1',
  kind: ActivityKind.posSale,
  effect: ActivityEffect.income,
  sourceGroup: ActivitySourceGroup.pos,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: '2026-10-02',
  amount: '1500.0000',
  currency: 'TRY',
  title: title,
  description: title,
  categoryName: 'Satış geliri',
  destinationName: 'Ziraat Vadesiz',
  canCancel: false,
  supportsAttachments: false,
  scope: TransactionScope.business,
  channelName: channel,
  feeAmount: fee,
  netAmount: '1402.5000',
  expectedTransferDate: '2026-10-22',
  transferredOn: transferredOn,
);

FinancialActivity _deposit({
  String? channel = 'Yemek kartı',
  String? fee,
  int count = 1,
}) => FinancialActivity(
  activityId: 'deposit-1',
  kind: ActivityKind.posDeposit,
  effect: ActivityEffect.neutral,
  sourceGroup: ActivitySourceGroup.pos,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: '2026-10-02',
  amount: '1402.5000',
  currency: 'TRY',
  title: '',
  destinationName: 'Ziraat Vadesiz',
  canCancel: false,
  supportsAttachments: false,
  channelName: channel,
  feeAmount: fee,
  settlementCount: count,
);

const _json = <String, Object?>{
  'activityId': 'activity-1',
  'activityKind': 'account-transaction',
  'effect': 'expense',
  'sourceGroup': 'account',
  'origin': 'manual',
  'status': 'realized',
  'activityDate': '2026-10-02',
  'amount': '100.0000',
  'currency': 'TRY',
  'title': 'Market',
  'canCancel': true,
  'supportsAttachments': true,
};
