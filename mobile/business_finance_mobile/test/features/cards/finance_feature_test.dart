import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import '../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_row_action.dart';
import 'package:business_finance_mobile/core/widgets/app_status_chip.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:provider/provider.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/cards/presentation/finance_controller.dart';
import 'package:business_finance_mobile/features/activities/data/receipt_fee_writer.dart';
import 'package:business_finance_mobile/features/cards/presentation/finance_page.dart';
import 'package:business_finance_mobile/features/cards/presentation/transfer_prefill.dart';

void main() {
  testWidgets('kartlar ekranı erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(repository: _FakeFinanceRepository()),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  test('credit card model preserves exact money strings', () {
    final card = CreditCardItem.fromJson(_cardJson);
    expect(card.currentDebt, '250.5000');
    expect(card.availableLimit, '749.5000');

    expect(
      () => CreditCardItem.fromJson({..._cardJson, 'limit': 1000.0}),
      throwsFormatException,
    );
  });

  test('transfer repository sends amount as a JSON string', () async {
    final repository = FinanceRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          expect(request.url.path, '/api/v1/transfers');
          final body = jsonDecode(request.body) as Map<String, dynamic>;
          expect(body['amount'], '125.5000');
          expect(body['amount'], isA<String>());
          return http.Response(jsonEncode({'id': 'ignored'}), 201);
        }),
      ),
    );

    await repository.createTransfer({
      'sourceAccountId': 'source',
      'destinationAccountId': 'destination',
      'amount': '125.5000',
      'currency': 'TRY',
      'transferDate': '2026-08-10',
    });
  });

  test('transfer repository uses the cancellation endpoint', () async {
    final repository = FinanceRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          expect(request.method, 'DELETE');
          expect(request.url.path, '/api/v1/transfers/transfer-id');
          return http.Response(jsonEncode(_transferJson), 200);
        }),
      ),
    );

    await repository.cancelTransfer('transfer-id');
  });

  test(
    'repository localizes categories without changing account names',
    () async {
      final repository = FinanceRepository(
        ApiClient(
          config: ApiConfig.fromEnvironment(value: 'https://api.test'),
          httpClient: MockClient((request) async {
            final items = switch (request.url.path) {
              '/api/v1/accounts' => [
                {'id': 'account', 'name': 'Groceries'},
              ],
              '/api/v1/categories' => [
                {'id': 'category', 'name': 'Groceries'},
              ],
              _ => <Map<String, dynamic>>[],
            };
            return http.Response(
              jsonEncode({'items': items, 'hasMore': false}),
              200,
            );
          }),
        ),
      );

      final snapshot = await repository.load();

      expect(snapshot.accounts.single.name, 'Groceries');
      expect(snapshot.expenseCategories.single.name, 'Market Alışverişi');
    },
  );

  test('controller exposes unauthorized and prevents double submit', () async {
    final unauthorized = FinanceController(
      _FakeFinanceRepository(
        loadError: const ApiException(
          statusCode: 401,
          code: 'authentication.required',
          message: 'Oturum gerekli.',
        ),
      ),
    );
    await unauthorized.load();
    expect(unauthorized.unauthorized, isTrue);

    final controller = FinanceController(_FakeFinanceRepository());
    final completer = Completer<void>();
    var calls = 0;
    final first = controller.submit(() {
      calls++;
      return completer.future;
    }, 'Kaydedildi');
    final second = await controller.submit(() async => calls++, 'Tekrar');
    expect(second, isFalse);
    expect(calls, 1);
    completer.complete();
    expect(await first, isTrue);
  });

  test('card spending invalidates dashboard and budgets', () async {
    final changes = FinancialDataChanges();
    final controller = FinanceController(
      _FakeFinanceRepository(),
      financialDataChanges: changes,
    );

    final saved = await controller.submit(
      () async {},
      'Kart harcaması kaydedildi.',
      impact: FinanceMutationImpact.cardCharge,
    );

    expect(saved, isTrue);
    expect(changes.dashboardRevision, 1);
    expect(changes.budgetsRevision, 1);
    expect(changes.cardsRevision, 1);
    expect(changes.activityFeedRevision, 1);
    // A card purchase never moves an account balance.
    expect(changes.accountsRevision, 0);
  });

  test('card payment invalidates dashboard but not budgets', () async {
    final changes = FinancialDataChanges();
    final controller = FinanceController(
      _FakeFinanceRepository(),
      financialDataChanges: changes,
    );

    final saved = await controller.submit(
      () async {},
      'Kart ödemesi kaydedildi.',
      impact: FinanceMutationImpact.cardPayment,
    );

    expect(saved, isTrue);
    expect(changes.dashboardRevision, 1);
    expect(changes.accountsRevision, 1);
    expect(changes.cardsRevision, 1);
    // Paying the card settles debt already counted as expense, so it is not a
    // second expense and budgets stay put.
    expect(changes.budgetsRevision, 0);
  });

  testWidgets('kart ekranı transfer taşımıyor ve girişi doğruluyor', (
    tester,
  ) async {
    // Transferler hesaplara taşındı; bu ekran yalnız kartları gösteriyor ve
    // artık sekmesi yok.
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(repository: _FakeFinanceRepository()),
      ),
    );
    await tester.pumpAndSettle();
    expect(find.text('Kredi kartları'), findsOneWidget);
    expect(find.text('Transferler'), findsNothing);
    expect(find.text('Henüz kredi kartı yok'), findsOneWidget);
    await tester.tap(find.text('Kart ekle'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Kesim günü (1–28)'),
      '29',
    );
    await tester.tap(find.text('Kaydet'));
    await tester.pump();
    expect(find.text('1–28 arasında gün girin.'), findsOneWidget);
  });

  testWidgets('kart sayfası tarihleri ayrı satırlarda ve taşmadan gösterir', (
    tester,
  ) async {
    // Kesim ve son ödeme günü tek satırda `Kesim 10 • Son ödeme 20` diye
    // sıkışıyordu; hangi sayının neyin günü olduğu okumadan anlaşılmıyordu.
    final controller = FinanceController(
      _FakeFinanceRepository(cards: [_card()]),
    );
    await controller.load();

    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );

    expect(find.text('Hesap kesim günü'), findsOneWidget);
    expect(find.text('Son ödeme günü'), findsOneWidget);
    expect(find.text('Her ayın 10.'), findsOneWidget);
    expect(find.text('Her ayın 20.'), findsOneWidget);

    // Dört eylem butonu iki gruba ayrıldı; en büyük yazı ölçeğinde alt satıra
    // düşen buton üsttekine yapışmamalı ve satır taşmamalı.
    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  testWidgets('taksitli harcama kartın içinde ve kart sorulmuyor', (
    tester,
  ) async {
    // Taksitli harcama bir kart harcamasıdır. Ayrı bir sekmedeyken hangi
    // karta ait olduğu ancak formu açınca görünüyor ve kullanıcı yanlış kartı
    // seçebiliyordu.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(cards: [_card()]);
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();

    // Ayrı sekme kalmadı.
    expect(find.text('Taksitler'), findsNothing);

    await tester.tap(find.text('Test Kart'));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Taksitli harcama'));
    await tester.pumpAndSettle();

    // Kartın sayfasındayken "hangi kart" diye sormak, verilen bilgiyi geri
    // istemek olurdu.
    expect(find.text('Kart'), findsNothing);
    expect(find.text('Gider kategorisi'), findsOneWidget);

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Toplam'),
      '1400',
    );
    final categoryField = find.byType(DropdownButtonFormField<String>).first;
    await tester.ensureVisible(categoryField);
    await tester.pumpAndSettle();
    await tester.tap(categoryField);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Sağlık').last);
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    // Kart, kullanıcıya sorulmadan isteğe girer: asıl kazanç bu.
    expect(repository.lastPlanInput!['creditCardId'], 'card-1');
    expect(repository.lastPlanInput!['categoryId'], 'category-1');
    expect(repository.lastPlanInput!['totalAmount'], '1400');
  });

  testWidgets('active transfer opens details and can be cancelled', (
    tester,
  ) async {
    // Transfer bölümü artık Hesaplar ekranının içinde gömülü çiziliyor.
    final repository = _FakeFinanceRepository(
      transfers: [TransferItem.fromJson(_transferJson)],
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: FinancePage(
            repository: repository,
            section: FinanceSection.transfers,
            embedded: true,
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('₺125,50'));
    await tester.pumpAndSettle();
    expect(find.text('Transfer ayrıntısı'), findsOneWidget);
    // Durum etiketli bir metin satırı değil, rozet.
    expect(find.text('Durum: Aktif'), findsNothing);
    expect(find.widgetWithText(AppStatusChip, 'Aktif'), findsOneWidget);
    // Transferin asıl hikâyesi iki uç: nereden nereye.
    expect(find.text('Çıkan hesap'), findsOneWidget);
    expect(find.text('Giren hesap'), findsOneWidget);
    expect(find.textContaining('gelir veya gider sayılmaz'), findsOneWidget);

    await tester.tap(find.text('Transferi iptal et'));
    await tester.pumpAndSettle();
    expect(find.text('Transfer iptal edilsin mi?'), findsOneWidget);
    await tester.tap(
      find.descendant(
        of: find.byType(Dialog),
        matching: find.widgetWithText(FilledButton, 'Transferi iptal et'),
      ),
    );
    await tester.pumpAndSettle();

    expect(repository.cancelledTransferId, 'transfer-id');
    expect(find.textContaining('İptal'), findsWidgets);
  });
  testWidgets('kart özeti ekstreyi güncel borcun üstünde manşetle gösterir', (
    tester,
  ) async {
    // Bankalarda ödenecek tutar en üstte durur. Güncel borç ile dönem borcu
    // farklı şeyler: ilki kesimden sonraki harcamaları da içerir.
    final repository = _FakeFinanceRepository(
      cards: [_card()],
      currentStatement: _statement(),
    );
    final controller = FinanceController(repository);
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Dönem borcu'), findsOneWidget);
    expect(find.text('Ağustos 2026 ekstresi'), findsOneWidget);
    expect(find.text('Asgari ödeme'), findsOneWidget);
    expect(find.text('20 Ağustos'), findsOneWidget);
    expect(find.widgetWithText(FilledButton, 'Ekstreyi öde'), findsOneWidget);
    expect(find.widgetWithText(OutlinedButton, 'Asgariyi öde'), findsOneWidget);
  });

  testWidgets('kesim gelmemiş kart ekstreyi hata gibi göstermez', (
    tester,
  ) async {
    // Yeni kartta ekstre yokluğu bir kusur değil; hata gibi göstermek
    // kullanıcıyı olmayan bir sorunu aramaya iterdi.
    final controller = FinanceController(
      _FakeFinanceRepository(cards: [_card()]),
    );
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('Henüz ekstre kesilmedi'), findsOneWidget);
    expect(find.widgetWithText(FilledButton, 'Ekstreyi öde'), findsNothing);
  });

  testWidgets('ekstreyi öde, ödeme panelini kalan tutar dolu açar', (
    tester,
  ) async {
    // Eski akışta ekstre "Kalan 3.500" diyor ama oradan ödenemiyordu;
    // kullanıcı pencereyi kapatıp tutarı elden yazmak zorundaydı.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(
      cards: [_card()],
      currentStatement: _statement(),
    );
    final controller = FinanceController(repository);
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(FilledButton, 'Ekstreyi öde'));
    await tester.pumpAndSettle();

    final amount = tester.widget<TextFormField>(
      find.widgetWithText(TextFormField, '3500'),
    );
    expect(amount, isNotNull);
  });

  testWidgets('asgariyi öde, asgari tutarı doldurur', (tester) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final controller = FinanceController(
      _FakeFinanceRepository(cards: [_card()], currentStatement: _statement()),
    );
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(OutlinedButton, 'Asgariyi öde'));
    await tester.pumpAndSettle();

    expect(find.widgetWithText(TextFormField, '700'), findsOneWidget);
  });

  testWidgets('ödenmiş ekstrede ödeme kısayolu çıkmaz', (tester) async {
    final controller = FinanceController(
      _FakeFinanceRepository(
        cards: [_card()],
        currentStatement: _statement(
          remainingBalance: '0.0000',
          remainingMinimumPayment: '0.0000',
          paymentStatus: 'paid',
        ),
      ),
    );
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Ödendi'), findsOneWidget);
    expect(find.widgetWithText(FilledButton, 'Ekstreyi öde'), findsNothing);
  });

  testWidgets('kart düzenleme asgari ödeme oranını gönderir', (tester) async {
    // Oran karta ait bir ayar; uygulamada hiç kart düzenleme yolu yoktu, o
    // yüzden kullanıcı kendi bankasının oranını hiç giremiyordu.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(cards: [_card()]);
    final controller = FinanceController(repository);
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.byTooltip('Kartı düzenle'));
    await tester.pumpAndSettle();

    // Mevcut oran dolu geliyor; kullanıcı yalnız değiştiriyor. Alan
    // etiketiyle bulunuyor: '20' metni son ödeme günü alanıyla da eşleşirdi.
    final rateField = tester.widget<TextFormField>(
      find.widgetWithText(TextFormField, 'Asgari ödeme oranı'),
    );
    expect(rateField.controller?.text, '20');
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Asgari ödeme oranı'),
      '40',
    );
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(repository.lastCardUpdate?['minimumPaymentRate'], '40');
  });

  testWidgets('kart formu 100 üstü oranı reddeder', (tester) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final controller = FinanceController(_FakeFinanceRepository());
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(repository: controller.repository),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kart ekle'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Asgari ödeme oranı'),
      '101',
    );
    await tester.tap(find.text('Kaydet'));
    await tester.pump();

    expect(find.text('0 ile 100 arasında bir oran girin.'), findsOneWidget);
  });

  testWidgets('fazla ödenmiş kart borç değil alacak gösterir', (tester) async {
    // Aşama 06 Grup 5: kartta duran para kullanıcınındır. "Borç −₺500,00"
    // yazmak ona borcu varmış gibi okunuyordu.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final controller = FinanceController(
      _FakeFinanceRepository(cards: [_overpaidCard()]),
    );
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(repository: controller.repository),
      ),
    );
    await tester.pumpAndSettle();

    expect(
      find.textContaining('Kartınızda ₺500,00 alacağınız var'),
      findsOneWidget,
    );
    expect(find.textContaining('Borç -'), findsNothing);
  });

  testWidgets('kart ayrıntısı alacaklı bakiyeyi kendi etiketiyle yazar', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final controller = FinanceController(
      _FakeFinanceRepository(cards: [_overpaidCard()]),
    );
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Kart alacağınız'), findsOneWidget);
    expect(find.text('Güncel borç'), findsNothing);
    // Kullanılabilir tutar limitin üstüne çıkar: para karttadır.
    expect(find.text('₺10.500,00'), findsWidgets);
  });

  testWidgets('kart formu varsayılan kapsamı gönderir', (tester) async {
    // Zincirin orta halkası: kartın etiketi, kullanıcı açık seçim yapmadığında
    // harcamanın hangi tarafa yazılacağını söyler.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(
      cards: [_card(defaultScope: TransactionScope.business)],
    );
    final controller = FinanceController(repository);
    await controller.load();

    await tester.pumpWidget(
      _scopedApp(
        hasBusiness: true,
        child: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.byTooltip('Kartı düzenle'));
    await tester.pumpAndSettle();

    expect(find.text('Varsayılan kapsam'), findsOneWidget);
    await tester.tap(find.text('Şahsi'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(repository.lastCardUpdate?['defaultScope'], 'personal');
  });

  testWidgets('kapsamı görmeyen kullanıcıda kart formunda alan yok', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final controller = FinanceController(_FakeFinanceRepository());
    await controller.load();

    await tester.pumpWidget(
      _scopedApp(
        hasBusiness: false,
        child: FinancePage(repository: controller.repository),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kart ekle'));
    await tester.pumpAndSettle();

    expect(find.text('Varsayılan kapsam'), findsNothing);
  });

  testWidgets('ekstre penceresi hesabı toplanabilir satırlarla gösterir', (
    tester,
  ) async {
    // Eski pencere "önceki devir" ve "dönem harcaması" gösteriyor ama
    // kesime kadar yapılan ödemeyi hiç okumuyordu; rakamlar toplanmıyor,
    // kullanıcı ekstreyi doğrulayamıyordu.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final controller = FinanceController(
      _FakeFinanceRepository(cards: [_card()], periodStatement: _statement()),
    );
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(OutlinedButton, 'Ekstre'));
    await tester.pumpAndSettle();

    expect(find.text('Önceki devir'), findsOneWidget);
    expect(find.text('Dönem harcaması'), findsOneWidget);
    expect(find.text('Kesime kadar ödeme'), findsOneWidget);
    expect(find.text('Kesimden sonra ödeme'), findsOneWidget);
    expect(find.text('Ekstre borcu'), findsOneWidget);
    expect(find.text('Kalan'), findsOneWidget);
    expect(find.text('Asgari ödeme (%20)'), findsOneWidget);

    // 1.000 + 3.000 − 500 = 3.500; ekrandaki satırlar bu hesabı veriyor.
    expect(find.text('+ ₺3.000,00'), findsOneWidget);
    expect(find.text('− ₺500,00'), findsOneWidget);
  });

  testWidgets('özet kartı ekstre ve altı eylemle birlikte taşmaz', (
    tester,
  ) async {
    // Kartın en yoğun hâli: ekstre manşeti + iki ödeme kısayolu + dört kart
    // eylemi, hepsi tek kutunun içinde. Eylemler kartın dışındayken bu
    // kombinasyon hiç ölçülmüyordu.
    final controller = FinanceController(
      _FakeFinanceRepository(cards: [_card()], currentStatement: _statement()),
    );
    await controller.load();

    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );

    // Eylemler özet kutusunun içinde: kartın altında serbest durduklarında
    // neye ait oldukları yalnız yakınlıktan anlaşılıyordu.
    expect(
      find.descendant(
        of: find.ancestor(
          of: find.text('Dönem borcu'),
          matching: find.byType(AppCard),
        ),
        matching: find.widgetWithText(FilledButton, 'Harcama'),
      ),
      findsOneWidget,
    );
    expectNoOverflow(tester);
  });

  test('dönem sorgusu varsayılan olarak son üç ayı ister', () {
    // Asıl kusur buydu: liste tarih filtresi olmadan tüm geçmişi çekiyordu.
    final today = DateTime(2026, 8, 17);

    expect(HistoryPeriod.threeMonths.query(today), '?from=2026-05-17');
    expect(HistoryPeriod.year.query(today), '?from=2025-08-17');
    // "Tümü" ayrı ve açık bir niyet; eksik parametreyle sınırsız sorguya
    // düşülmemesi için sunucu da bunu böyle bekliyor.
    expect(HistoryPeriod.all.query(today), '?all=true');
  });

  testWidgets('kart hareketleri kırpıldığında ekran tamlık iddia etmez', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final controller = FinanceController(
      _FakeFinanceRepository(
        cards: [_card()],
        activity: const CardActivity(
          charges: [
            CardChargeItem(
              id: 'charge-1',
              amount: '100.0000',
              currency: 'TRY',
              date: '2026-08-01',
              description: 'Market',
              isCancelled: false,
            ),
          ],
          payments: [],
          hasMore: true,
        ),
      ),
    );
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('daha fazla kayıt var'), findsOneWidget);
    expect(find.text('3 ay'), findsOneWidget);
  });

  testWidgets('dönem değişince liste yeni dönemle yeniden okunur', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(cards: [_card()]);
    final controller = FinanceController(repository);
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Tümü'));
    await tester.pumpAndSettle();

    expect(controller.period, HistoryPeriod.all);
    expect(repository.requestedPeriods, contains(HistoryPeriod.all));
  });

  testWidgets('dönem seçici satırı kaplar', (tester) async {
    // İlk yazımda `Align` içindeydi: sağında ölü boşluk bırakıyor ve
    // Planlananlar ekranındaki aynı işi yapan bardan farklı görünüyordu.
    //
    // Ölçüm **normal** yazı ölçeğinde: en büyük ölçekte butonun kendi
    // genişliği zaten satırı dolduruyor ve yaslama farkı ölçülemiyor —
    // ilk yazımda test o yüzden `Align` geri konduğunda da geçiyordu.
    tester.view.physicalSize = const Size(400, 800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(
          repository: _FakeFinanceRepository(),
          section: FinanceSection.transfers,
        ),
      ),
    );
    await tester.pumpAndSettle();

    final selector = find.byType(SegmentedButton<HistoryPeriod>);
    expect(selector, findsOneWidget);

    // Kart kenarlarına **tam** oturuyor: iki yandaki `medium` boşluk dışında
    // satırın tamamı. Gevşek bir alt sınır yeterli değildi — butonun kendi
    // genişliği zaten ona yakın olduğu için `Align` geri konduğunda da
    // geçiyordu. Ölçü birebir olmalı.
    final width = tester.getSize(selector).width;
    final screen = tester.getSize(find.byType(Scaffold)).width;
    expect(width, screen - 2 * AppSpacing.medium);
  });

  testWidgets('dönem seçici en büyük yazı ölçeğinde taşmaz', (tester) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(
          repository: _FakeFinanceRepository(),
          section: FinanceSection.transfers,
        ),
      ),
    );

    expect(find.byType(SegmentedButton<HistoryPeriod>), findsOneWidget);
    expectNoOverflow(tester);
  });

  // Dekonttan gelen öneri formu kendiliğinden açıyor. Ödeme kaydedilince liste
  // tazeleniyor ve tazeleme aynı öneriyi yeniden kullanıyordu: form kendini
  // yeniden açıyor, ücret de her açılışta yeniden yazılırdı.
  testWidgets('dekonttan gelen ödeme önerisi yalnız bir kez kullanılır', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(
      cards: [_card()],
      accounts: const [FinanceChoice(id: 'acc-1', name: 'Banka')],
    );
    final controller = FinanceController(repository);
    await controller.load();
    final fees = <({String sourceId, String amount, String description})>[];

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(
          cardId: 'card-1',
          controller: controller,
          paymentPrefill: const CardPaymentPrefill(
            amount: '1455.0000',
            date: '2026-08-19',
            feeAmount: '2.0000',
            feeDescription: 'İşlem ücreti — BANKA',
          ),
          recordFee:
              ({
                required String sourceId,
                required String amount,
                required String date,
                required String description,
              }) async {
                fees.add((
                  sourceId: sourceId,
                  amount: amount,
                  description: description,
                ));
                return const ReceiptFeeResult(
                  ReceiptFeeStatus.recorded,
                  'İşlem ücreti Diğer gider olarak kaydedildi.',
                );
              },
        ),
      ),
    );
    await tester.pumpAndSettle();

    await _chooseAccount(tester);
    await tester.tap(find.widgetWithText(FilledButton, 'Kaydet'));
    await tester.pumpAndSettle();

    expect(repository.payments, hasLength(1));
    // Ücret ödemenin çıktığı **hesaptan**, tek kez.
    expect(fees, hasLength(1));
    expect(fees.single.amount, '2.0000');
    expect(fees.single.description, 'İşlem ücreti — BANKA');
    // Form kendini yeniden açmadı.
    expect(find.widgetWithText(FilledButton, 'Kaydet'), findsNothing);
  });

  // Taksitli fişten gelindiğinde plan formu kendiliğinden açılıyor ve fişin
  // TOPLAM tutarını taşıyor: bölmeyi sunucu yapar, istemci finansal toplamı
  // ikinci kez hesaplamaz.
  testWidgets('taksitli fişten gelen plan formu toplam tutarla açılır', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(
      cards: [_card()],
      accounts: const [FinanceChoice(id: 'acc-1', name: 'Banka')],
    );
    final controller = FinanceController(repository);
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(
          cardId: 'card-1',
          controller: controller,
          installmentPrefill: const InstallmentPrefill(
            totalAmount: '3000.0000',
            installmentCount: 3,
            firstInstallmentDate: '2026-08-19',
            description: 'TEKNOSA',
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Yeni taksit planı'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, '3000'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, '3'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, 'TEKNOSA'), findsOneWidget);
    // Plan yalnız niyettir: form açıldı, hiçbir şey yazılmadı.
    expect(repository.plans, isEmpty);
  });

  // Gerçekleşmiş ve gerçekleşmemiş taksit aynı listede duruyor; ikisi aynı
  // bilginin iki hâli. Eskiden biri kocaman bir `Chip`, diğeri dolgulu bir
  // butondu ve satırlar farklı yükseklikte görünüyordu.
  testWidgets('taksit satırları durumu aynı yerde ve aynı boyda gösterir', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(
      cards: [_card()],
      installmentPlans: const [
        InstallmentPlanModel(
          id: 'plan-1',
          creditCardId: 'card-1',
          totalAmount: '3000.0000',
          currency: 'TRY',
          description: 'TEKNOSA',
          items: [
            InstallmentItemModel(
              sequence: 1,
              amount: '1000.0000',
              currency: 'TRY',
              scheduledDate: '2026-08-19',
              isRealized: true,
            ),
            InstallmentItemModel(
              sequence: 2,
              amount: '1000.0000',
              currency: 'TRY',
              scheduledDate: '2026-09-19',
              isRealized: false,
            ),
          ],
        ),
      ],
    );
    final controller = FinanceController(repository);
    await controller.load();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(cardId: 'card-1', controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('TEKNOSA'));
    await tester.pumpAndSettle();

    // Her satırda bir rozet; eylem yalnız gerçekleşmemiş olanda.
    expect(find.text('Gerçekleşti'), findsOneWidget);
    expect(find.text('Planlandı'), findsOneWidget);
    expect(find.text('Gerçekleştir'), findsOneWidget);
    // Tarih ham ISO değil okunur biçimde.
    expect(find.text('19 Eylül 2026'), findsOneWidget);
    // Eylem `AppRowAction`: rozetle aynı yükseklikte, yumuşak dolgulu.
    // Çerçevesiz metin butonu rozetin yanında ikinci bir etiket gibi okunuyor,
    // ince çizgili hap ise içi boş bir hayalet gibi duruyordu.
    expect(find.widgetWithText(AppRowAction, 'Gerçekleştir'), findsOneWidget);
    final action = tester.widget<FilledButton>(
      find.descendant(
        of: find.byType(AppRowAction),
        matching: find.byType(FilledButton),
      ),
    );
    final scheme = Theme.of(
      tester.element(find.byType(AppRowAction)),
    ).colorScheme;
    // Birincil dolgu **değil**: o ağırlık ekranın ana eylemine ait.
    expect(
      action.style?.backgroundColor?.resolve(const <WidgetState>{}),
      scheme.secondaryContainer,
    );
  });

  // Kullanıcının elle açtığı ödeme dekontla ilgisizdir; ona ücret iliştirmek
  // olmayan bir gider yazmak olurdu.
  testWidgets('elle açılan ödeme ücret yazmaz', (tester) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = _FakeFinanceRepository(
      cards: [_card()],
      accounts: const [FinanceChoice(id: 'acc-1', name: 'Banka')],
    );
    final controller = FinanceController(repository);
    await controller.load();
    var feeCalls = 0;

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CreditCardDetailPage(
          cardId: 'card-1',
          controller: controller,
          recordFee:
              ({
                required String sourceId,
                required String amount,
                required String date,
                required String description,
              }) async {
                feeCalls++;
                return const ReceiptFeeResult(
                  ReceiptFeeStatus.recorded,
                  'kaydedildi',
                );
              },
        ),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(FilledButton, 'Ödeme'));
    await tester.pumpAndSettle();
    await _chooseAccount(tester);
    await tester.enterText(find.widgetWithText(TextFormField, 'Tutar'), '100');
    await tester.tap(find.widgetWithText(FilledButton, 'Kaydet'));
    await tester.pumpAndSettle();

    expect(repository.payments, hasLength(1));
    expect(feeCalls, 0);
  });
}

Future<void> _chooseAccount(WidgetTester tester) async {
  await tester.tap(find.text('Ödeme hesabı'));
  await tester.pumpAndSettle();
  await tester.tap(find.text('Banka').last);
  await tester.pumpAndSettle();
}

const _transferJson = {
  'id': 'transfer-id',
  'sourceAccountId': 'source',
  'destinationAccountId': 'destination',
  'amount': '125.5000',
  'currency': 'TRY',
  'transferDate': '2026-08-10',
  'description': 'Test transferi',
  'isCancelled': false,
};

const _cardJson = {
  'id': 'card',
  'name': 'Main',
  'limit': '1000.0000',
  'currentDebt': '250.5000',
  'availableLimit': '749.5000',
  'currency': 'TRY',
  'statementClosingDay': 10,
  'paymentDueDay': 20,
  'minimumPaymentRate': '20.0000',
  'isActive': true,
};

class _FakeFinanceRepository implements FinanceRepositoryContract {
  _FakeFinanceRepository({
    this.loadError,
    List<TransferItem>? transfers,
    this.cards = const [],
    this.currentStatement,
    this.periodStatement,
    this.activity = const CardActivity(charges: [], payments: []),
    this.accounts = const [],
    this.installmentPlans = const [],
  }) : transfers = transfers ?? [];
  final ApiException? loadError;
  List<TransferItem> transfers;
  final List<CreditCardItem> cards;
  final CardStatement? currentStatement;
  final CardStatement? periodStatement;
  final CardActivity activity;
  final List<FinanceChoice> accounts;
  final List<InstallmentPlanModel> installmentPlans;
  final List<HistoryPeriod> requestedPeriods = [];
  String? cancelledTransferId;
  Map<String, Object?>? lastPlanInput;
  Map<String, Object?>? lastCardUpdate;

  @override
  Future<FinanceSnapshot> load({
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async {
    if (loadError case final error?) throw error;
    return FinanceSnapshot(
      transfers: transfers,
      cards: cards,
      plans: installmentPlans,
      accounts: accounts,
      expenseCategories: const [
        FinanceChoice(id: 'category-1', name: 'Sağlık'),
      ],
    );
  }

  @override
  Future<void> cancelTransfer(String transferId) async {
    cancelledTransferId = transferId;
    transfers = transfers
        .map(
          (item) => item.id == transferId
              ? TransferItem(
                  id: item.id,
                  sourceAccountId: item.sourceAccountId,
                  destinationAccountId: item.destinationAccountId,
                  amount: item.amount,
                  currency: item.currency,
                  date: item.date,
                  description: item.description,
                  isCancelled: true,
                )
              : item,
        )
        .toList(growable: false);
  }

  @override
  Future<void> createCard(Map<String, Object?> input) async {}
  @override
  Future<void> updateCard(String cardId, Map<String, Object?> input) async {
    lastCardUpdate = input;
  }

  @override
  Future<void> createCharge(String cardId, Map<String, Object?> input) async {}

  final List<Map<String, Object?>> payments = [];

  @override
  Future<void> createPayment(String cardId, Map<String, Object?> input) async {
    payments.add(input);
  }

  @override
  Future<void> createPlan(Map<String, Object?> input) async {
    lastPlanInput = input;
    plans.add(input);
  }

  final List<Map<String, Object?>> plans = [];

  @override
  Future<void> createTransfer(Map<String, Object?> input) async {}
  @override
  Future<void> realizeInstallment(String planId, int sequence) async {}
  @override
  Future<CardActivity> loadActivity(
    String cardId, {
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async {
    requestedPeriods.add(period);
    return activity;
  }

  @override
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) async => periodStatement ?? (throw UnimplementedError());

  @override
  Future<CardStatement?> loadCurrentStatement(String cardId) async =>
      currentStatement;
}

Widget _scopedApp({required bool hasBusiness, required Widget child}) {
  final controller = ScopeController(
    store: _FakeScopeStore(hasBusiness: hasBusiness),
    readHasBusiness: () async => hasBusiness,
  )..ensureLoaded();

  return ChangeNotifierProvider<ScopeController>.value(
    value: controller,
    child: MaterialApp(theme: AppTheme.light(), home: child),
  );
}

class _FakeScopeStore implements ScopeStore {
  _FakeScopeStore({required this.hasBusiness});

  final bool hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => null;

  @override
  Future<void> writeScope(TransactionScope? scope) async {}

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async {}

  @override
  Future<void> clear() async {}
}

/// Fazla ödenmiş kart: borcu eksi, kullanılabilir tutarı limitin üstünde.
CreditCardItem _overpaidCard() => const CreditCardItem(
  id: 'card-1',
  name: 'Test Kart',
  limit: '10000.0000',
  currentDebt: '-500.0000',
  availableLimit: '10500.0000',
  currency: 'TRY',
  statementClosingDay: 10,
  paymentDueDay: 20,
  minimumPaymentRate: '20.0000',
  isActive: true,
);

CreditCardItem _card({TransactionScope? defaultScope}) => CreditCardItem(
  id: 'card-1',
  name: 'Test Kart',
  limit: '10000.0000',
  currentDebt: '0.0000',
  availableLimit: '10000.0000',
  currency: 'TRY',
  statementClosingDay: 10,
  paymentDueDay: 20,
  minimumPaymentRate: '20.0000',
  isActive: true,
  defaultScope: defaultScope,
);

CardStatement _statement({
  String remainingBalance = '3500.0000',
  String remainingMinimumPayment = '700.0000',
  String paymentStatus = 'open',
}) => CardStatement(
  periodStart: '2026-07-11',
  closingDate: '2026-08-10',
  dueDate: '2026-08-20',
  year: 2026,
  month: 8,
  previousBalance: '1000.0000',
  periodCharges: '3000.0000',
  paymentsThroughClosing: '500.0000',
  statementBalance: '3500.0000',
  paymentsAfterClosing: '0.0000',
  remainingBalance: remainingBalance,
  minimumPayment: '700.0000',
  remainingMinimumPayment: remainingMinimumPayment,
  minimumPaymentRate: '20.0000',
  currency: 'TRY',
  paymentStatus: paymentStatus,
);
