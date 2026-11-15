import 'package:business_finance_mobile/features/cards/presentation/card_plan_widgets.dart';
import 'package:business_finance_mobile/features/cards/data/card_page_models.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:business_finance_mobile/core/widgets/app_form_error.dart';
import '../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
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

import '../../support/category_field_helpers.dart';

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

  // 27 Ağustos 2026 kabul turu: taksitli fiş kart seçmeye götürüyor ama
  // hesabında kart yoksa ekranda yalnız mavi yönlendirme ile `Kart ekle`
  // düğmesi kalıyordu. Boş durum, listenin **verisine** bakmadığı için hiç
  // çizilmiyordu — yönlendirme kutusu tek başına listeyi dolu gösteriyordu.
  testWidgets('fişten gelen kartsız kullanıcı hem yönlendirmeyi hem boş '
      'durumu görür', (tester) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(
          repository: _FakeFinanceRepository(),
          installmentPrefill: const InstallmentPrefill(
            totalAmount: '3600.0000',
            installmentCount: 6,
            firstInstallmentDate: '2026-08-19',
            description: 'TEKNOSA',
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('6 taksitli alışveriş için'), findsOneWidget);
    expect(find.text('Henüz kredi kartı yok'), findsOneWidget);
    expect(find.text('Kart ekle'), findsOneWidget);
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

  /// Kabul turunda bulunan kusur: kart detayından yazılan harcama, ödeme ve
  /// taksit planının sonucu **hiçbir yerde** görünmüyordu. Mesaj denetleyiciye
  /// yazılıyor ama yalnız kart listesi çiziyordu; reddedilen bir ödeme sessizce
  /// kayboluyor, kullanıcı kaydedildi sanıyordu.
  testWidgets('kart detayı yazma sonucunu kendi ekranında söyler', (
    tester,
  ) async {
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
    expect(find.byType(MaterialBanner), findsNothing);

    // Sunucunun reddettiği bir yazma: fazla ödeme.
    await controller.submit(
      () => throw const ApiException(
        code: 'credit_cards.payment_exceeds_debt',
        message: 'Ödeme tutarı kartın güncel borcundan büyük olamaz.',
        statusCode: 409,
      ),
      'Kart ödemesi kaydedildi.',
    );
    await tester.pumpAndSettle();

    expect(
      find.text('Ödeme tutarı kartın güncel borcundan büyük olamaz.'),
      findsOneWidget,
    );

    await tester.tap(find.text('Kapat'));
    await tester.pumpAndSettle();
    expect(find.byType(MaterialBanner), findsNothing);

    // Başarı da söylenir: sessiz başarı ile sessiz başarısızlık aynı görünür.
    await controller.submit(() async {}, 'Kart ödemesi kaydedildi.');
    await tester.pumpAndSettle();

    expect(find.text('Kart ödemesi kaydedildi.'), findsOneWidget);
    // Başarı kalıcı şerit değil, kendiliğinden kapanan bildirimdir.
    expect(find.byType(MaterialBanner), findsNothing);
    expect(find.byType(SnackBar), findsOneWidget);
    await tester.pump(const Duration(seconds: 5));
    await tester.pumpAndSettle();
    expect(find.text('Kart ödemesi kaydedildi.'), findsNothing);
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

    // Günler kartın yüzünde, her biri kendi etiketiyle (teslim §1: son ödeme
    // üstte, kesim altta).
    expect(find.textContaining('Kesim', findRichText: true), findsOneWidget);
    expect(
      find.textContaining('Son ödeme', findRichText: true),
      findsOneWidget,
    );
    expect(
      find.textContaining("Ayın 10'u", findRichText: true),
      findsOneWidget,
    );
    expect(
      find.textContaining("Ayın 20'si", findRichText: true),
      findsOneWidget,
    );

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
    expect(find.text('Kategori'), findsOneWidget);

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Toplam tutar'),
      '1400',
    );
    await tester.ensureVisible(categoryField('Kategori'));
    await tester.pumpAndSettle();
    await chooseCategory(tester, 'Sağlık', label: 'Kategori');

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    // Kart, kullanıcıya sorulmadan isteğe girer: asıl kazanç bu.
    expect(repository.lastPlanInput!['creditCardId'], 'card-1');
    expect(repository.lastPlanInput!['categoryId'], 'category-1');
    expect(repository.lastPlanInput!['totalAmount'], '1400');
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

    expect(find.text('Ağustos 2026 dönem borcu'), findsOneWidget);
    // Ekstrenin adı ve asgari tutar tek yardımcı satırdadır.
    expect(find.text('Ağustos 2026 dönem borcu'), findsOneWidget);
    expect(find.text('Asgari ödeme'), findsOneWidget);
    expect(find.text('20 Ağustos'), findsOneWidget);
    expect(find.widgetWithText(FilledButton, 'Ödeme yap'), findsOneWidget);
    expect(find.widgetWithText(OutlinedButton, 'Asgariyi öde'), findsNothing);
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
    expect(find.widgetWithText(FilledButton, 'Ödeme yap'), findsNothing);
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

    await tester.tap(find.widgetWithText(FilledButton, 'Ödeme yap'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Devam et'));
    await tester.pumpAndSettle();

    final amount = tester.widget<TextFormField>(
      find.widgetWithText(TextFormField, '3.500'),
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

    await tester.tap(find.widgetWithText(FilledButton, 'Ödeme yap'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Asgari ödeme').last);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Devam et'));
    await tester.pumpAndSettle();

    expect(find.widgetWithText(TextFormField, '700'), findsOneWidget);
    await tester.tap(find.byTooltip('Tutar seçimi'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Devam et'));
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

    expect(find.text('Tamamı ödendi'), findsOneWidget);
    expect(find.widgetWithText(FilledButton, 'Ödeme yap'), findsNothing);
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

  // Aynı adlı kart reddedilince form kapanmaz ve cümleyi kendi içinde yazar
  // (10 Ekim 2026: form kapanıp cümle sayfanın üstünde çıkıyordu; kullanıcı
  // kartın yazıldığını sanabilirdi).
  testWidgets('reddedilen kart formu kapatmaz ve nedenini formda yazar', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    const sentence =
        'Bu adı taşıyan bir Kredi kartı kaydı zaten var. Farklı bir ad seçin.';
    final repository = _FakeFinanceRepository(
      cardError: const ApiException(
        statusCode: 409,
        code: 'credit_cards.duplicate_name',
        message: sentence,
      ),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: FinancePage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kart ekle'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Kart adı'),
      'garanti',
    );
    await tester.enterText(find.widgetWithText(TextFormField, 'Limit'), '5000');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    // Form açık, yazılan ad yerinde, cümle formda.
    expect(find.widgetWithText(TextFormField, 'Kart adı'), findsOneWidget);
    expect(find.text('garanti'), findsOneWidget);
    expect(find.byType(AppFormError), findsOneWidget);
    expect(find.text(sentence), findsOneWidget);

    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();

    // Vazgeçince arkadaki sayfada eski ret durmaz.
    expect(find.text(sentence), findsNothing);
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

    // Kart alacağı kendi etiketiyle ve eksi işaretsiz yazar.
    expect(find.text('Karttaki fazla ödeme'), findsOneWidget);
    expect(find.text('₺500,00'), findsOneWidget);
    expect(find.text('Güncel borç'), findsNothing);
    expect(find.textContaining('-₺'), findsNothing);
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

    expect(find.text('Karttaki fazla ödeme'), findsOneWidget);
    expect(find.text('Güncel borç'), findsNothing);
    // Kullanılabilir tutar limitin üstüne çıkar: para karttadır.
    expect(find.textContaining('₺10.500,00', findRichText: true), findsWidgets);
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
    // Ortak seçim alanı: alana dokununca menü açılır, kural alt yazıdadır.
    expect(find.text('Kategoride yoksa geçerli'), findsOneWidget);
    await tester.tap(find.text('İşletme'));
    await tester.pumpAndSettle();
    expect(find.text('Kapsamı kategori belirler'), findsOneWidget);
    await tester.tap(find.text('Şahsi').last);
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

    await tester.tap(find.text('Ekstre'));
    await tester.pumpAndSettle();

    expect(find.text('Önceki dönemden devir'), findsOneWidget);
    expect(find.text('Dönem harcamaları'), findsOneWidget);
    expect(find.text('Kesime kadar ödenen'), findsOneWidget);
    expect(find.text('Kesim sonrası ödenen'), findsOneWidget);
    // Biri ekstre penceresinin satırı, biri arkadaki sayfanın dönem borcu
    // kartının başlığı.
    expect(find.text('Dönem borcu'), findsWidgets);
    expect(find.text('Kalan dönem borcu'), findsNWidgets(2));
    expect(find.text('Ekstredeki asgari tutar'), findsOneWidget);

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

    // Eylemler kartın yüzünün hemen altında, tek sırada beş kutudur
    // (11 Kasım 2026): harcama, taksitli harcama, ödeme, ekstre, düzenle.
    for (final label in [
      'Harcama',
      'Taksitli harcama',
      'Ödeme',
      'Ekstre',
      'Düzenle',
    ]) {
      expect(find.text(label), findsOneWidget);
    }
    expect(
      tester.getTopLeft(find.text('Harcama')).dy,
      lessThan(tester.getTopLeft(find.text('Ağustos 2026 dönem borcu')).dy),
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

  testWidgets(
    'hareketler sayfası özeti sunucudan ve durumu adla aynı satırda',
    (tester) async {
      tester.view.physicalSize = const Size(1200, 2400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);

      final repository = _FakeFinanceRepository(cards: [_card()])
        ..cardPage = (query) => _page(
          period: const CardPagePeriod(
            kind: 'statement',
            year: 2026,
            month: 11,
            start: '2026-10-16',
            end: '2026-11-15',
            status: 'current',
          ),
          spending: '6452.2200',
          payments: query.kind == CardMovementKind.all ? '3000.0000' : null,
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

      await tester.tap(find.text('Tümü'));
      await tester.pumpAndSettle();

      expect(find.text('Hareketler'), findsOneWidget);
      expect(find.text('Kasım ekstresi'), findsOneWidget);
      expect(find.text('16 Ekim – 15 Kasım'), findsOneWidget);
      // Durum ekstrenin adıyla aynı satırda, tarihlerin satırında değil.
      expect(
        tester.getCenter(find.text('Açık dönem')).dy,
        closeTo(tester.getCenter(find.text('Kasım ekstresi')).dy, 2),
      );
      expect(find.text('-₺6.452,22'), findsOneWidget);
      expect(find.text('₺3.000,00'), findsOneWidget);
      // Varsayılan dönem çip üretmez.
      expect(find.byTooltip('Filtrele'), findsOneWidget);
    },
  );

  testWidgets('filtre dönemi ve türü seçer, çip ile kaldırılır', (
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
    await tester.tap(find.byTooltip('Filtrele'));
    await tester.pumpAndSettle();
    expect(find.text('Hareketleri filtrele'), findsOneWidget);
    await tester.tap(find.text('Tüm zamanlar').last);
    await tester.tap(find.text('Harcama').last);
    await tester.pumpAndSettle();
    // Seçim taslaktır; düğmeden önce istek gitmez.
    final before = repository.pageQueries.length;
    await tester.tap(find.text('Sonuçları göster'));
    await tester.pumpAndSettle();

    expect(repository.pageQueries.length, greaterThan(before));
    final query = repository.pageQueries.last;
    expect(query.period.all, isTrue);
    expect(query.kind, CardMovementKind.charge);
    expect(find.byTooltip('Filtre, 2 seçili'), findsOneWidget);

    // Çipin ✕'i yalnız o filtreyi kaldırır.
    await tester.tap(find.bySemanticsLabel('Harcama filtresini kaldır'));
    await tester.pumpAndSettle();
    expect(repository.pageQueries.last.kind, CardMovementKind.all);
    expect(repository.pageQueries.last.period.all, isTrue);
  });

  // Panel açıkken dönem değişince kategoriler yeni döneme göre okunur;
  // yoksa kullanıcı kategoriyi seçmek için önce sonuçları görmek zorunda
  // kalıyordu (kullanıcı, 15 Kasım 2026).
  testWidgets('filtrede dönem değişince kategoriler yeni döneme göre gelir', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    FinancePageOption option(String id, String name) => FinancePageOption(
      key: 'category:$id',
      group: 'category',
      amount: '100.0000',
      name: name,
      id: id,
    );
    final repository = _FakeFinanceRepository(cards: [_card()])
      ..cardPage = (query) => CardPageData(
        cardId: 'card-1',
        name: 'Kart',
        period: CardPagePeriod(kind: query.period.all ? 'all' : 'statement'),
        statements: const [],
        spending: '100.0000',
        payments: '0.0000',
        categories: [
          const FinancePageOption(key: 'all', group: 'all', amount: '100.0000'),
          option('c1', 'Market'),
          if (query.period.all) option('c2', 'Eski dönem kategorisi'),
        ],
        items: const [],
        totalCount: 0,
        offset: 0,
        hasMore: false,
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
    await tester.tap(find.text('Tümü'));
    await tester.pumpAndSettle();
    await tester.tap(find.byTooltip('Filtrele'));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Tüm zamanlar').last);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Bütün kategoriler').last);
    await tester.pumpAndSettle();

    expect(find.text('Eski dönem kategorisi'), findsOneWidget);
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

    expect(find.text('Taksitli harcama'), findsWidgets);
    expect(find.widgetWithText(TextFormField, '3.000'), findsOneWidget);
    // Sık sayılardan biri: menülü alanda yazar.
    expect(
      find.descendant(
        of: find.byType(InstallmentCountField),
        matching: find.text('3'),
      ),
      findsOneWidget,
    );
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
            // Günü gelmemiş taksit: kendi gününe yazılır, erken gerçekleşmez.
            InstallmentItemModel(
              sequence: 3,
              amount: '1000.0000',
              currency: 'TRY',
              scheduledDate: '2099-10-19',
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

    // Kart sayfasında tek planın kartı tam genişliktir; dokununca Taksitler
    // sayfası, oradaki karta dokununca ayrıntı açılır.
    await tester.tap(find.text('TEKNOSA'));
    await tester.pumpAndSettle();
    expect(find.text('Taksitler'), findsOneWidget);
    await tester.tap(find.text('TEKNOSA'));
    await tester.pumpAndSettle();

    // Her satırda bir durum: yansıyan, sıradaki, planlanan. Gerçekleştir
    // bilgisi düğmededir (G1): sıra ve tutar düğmenin içinde yazar.
    expect(find.text('Karta yansıdı'), findsOneWidget);
    expect(find.text('Sıradaki'), findsOneWidget);
    expect(find.text('Planlandı'), findsOneWidget);
    expect(find.text('2. taksiti gerçekleştir'), findsOneWidget);
    expect(find.text('19 Ekim 2099'), findsOneWidget);
    // Tarih ham ISO değil okunur biçimde.
    expect(find.text('19 Eylül 2026'), findsOneWidget);
    // Kayıt eylemi satırları büyütmez; panelin ayrı alt alanındadır.
    final action = tester.widget<FilledButton>(
      find.widgetWithText(FilledButton, '2. taksiti gerçekleştir'),
    );
    expect(action.onPressed, isNotNull);
    expect(
      tester.getTopLeft(find.text('2. taksiti gerçekleştir')).dy,
      greaterThan(tester.getBottomLeft(find.text('19 Ekim 2099')).dy),
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

    await tester.tap(find.text('Ödeme'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Devam et'));
    await tester.pumpAndSettle();
    await _chooseAccount(tester);
    await tester.enterText(find.widgetWithText(TextFormField, 'Tutar'), '100');
    await tester.tap(find.widgetWithText(FilledButton, 'Kaydet'));
    await tester.pumpAndSettle();

    expect(repository.payments, hasLength(1));
    expect(feeCalls, 0);
  });
  testWidgets(
    'başka tutar boş açılır ve kayıt reddinde form bilgileri korunur',
    (tester) async {
      final repository = _FakeFinanceRepository(
        cards: [_card()],
        currentStatement: _statement(),
        accounts: const [FinanceChoice(id: 'account', name: 'Banka')],
        paymentError: const ApiException(
          code: 'credit_cards.payment_exceeds_debt',
          message: 'Ödeme tutarı kartın güncel borcundan büyük olamaz.',
          statusCode: 409,
        ),
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
      await tester.tap(find.text('Ödeme'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Başka tutar'));
      await tester.tap(find.text('Devam et'));
      await tester.pumpAndSettle();
      final field = find.widgetWithText(TextFormField, 'Tutar');
      expect(tester.widget<TextFormField>(field).controller!.text, isEmpty);
      await _chooseAccount(tester);
      await tester.enterText(field, '99999');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();
      expect(find.byType(AppFormError), findsOneWidget);
      expect(tester.widget<TextFormField>(field).controller!.text, '99.999');
      expect(repository.payments, isEmpty);
    },
  );

  testWidgets('asgari tamamlandı kalan dönem borcunu kapatmaz', (tester) async {
    final controller = FinanceController(
      _FakeFinanceRepository(
        cards: [_card()],
        currentStatement: _statement(remainingMinimumPayment: '0.0000'),
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
    // Asgari ödendi: asgari satırında yeşil `Ödendi`, ekstre açık kalır.
    expect(find.text('Ödendi'), findsOneWidget);
    expect(find.text('Tamamı ödendi'), findsNothing);
    expect(find.text('Ödeme yap'), findsOneWidget);
    await tester.tap(find.text('Ödeme'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Asgari ödeme').last);
    await tester.tap(find.text('Devam et'));
    await tester.pumpAndSettle();
    expect(find.widgetWithText(TextFormField, '3.500'), findsOneWidget);
  });

  for (final width in [320.0, 360.0, 412.0, 430.0, 800.0]) {
    for (final scale in [1.0, 1.3, 2.0]) {
      testWidgets('kart ve paneller taşmaz $width / $scale', (tester) async {
        tester.view.physicalSize = Size(width, 892);
        tester.view.devicePixelRatio = 1;
        addTearDown(tester.view.reset);
        final controller = FinanceController(
          _FakeFinanceRepository(
            cards: [_card()],
            currentStatement: _statement(),
            periodStatement: _statement(),
          ),
        );
        await controller.load();
        await tester.pumpWidget(
          MaterialApp(
            theme: AppTheme.light(),
            builder: (context, child) => MediaQuery(
              data: MediaQuery.of(
                context,
              ).copyWith(textScaler: TextScaler.linear(scale)),
              child: child!,
            ),
            home: CreditCardDetailPage(
              cardId: 'card-1',
              controller: controller,
            ),
          ),
        );
        await tester.pumpAndSettle();
        expectNoOverflow(tester);
        await tester.ensureVisible(find.text('Ekstre'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Ekstre'));
        await tester.pumpAndSettle();
        expectNoOverflow(tester);
        await tester.ensureVisible(find.text('Ödeme yap').last);
        await tester.pumpAndSettle();
        await tester.tap(find.text('Ödeme yap').last);
        await tester.pumpAndSettle();
        expectNoOverflow(tester);
        await tester.tap(find.text('Devam et'));
        await tester.pumpAndSettle();
        tester.view.viewInsets = FakeViewPadding(bottom: 300);
        await tester.pumpAndSettle();
        expectNoOverflow(tester);
        expect(find.text('Kaydet'), findsOneWidget);
      });
    }
  }

  testWidgets(
    'dar ekranda büyük yazı ve uzun kart tutarı küçültülmeden sığar',
    (tester) async {
      tester.view.physicalSize = const Size(320, 892);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      final repository = _FakeFinanceRepository(
        cards: [
          const CreditCardItem(
            id: 'card-1',
            name: 'İşletmenin uzun isimli alışveriş kredi kartı',
            limit: '2000000.0000',
            currentDebt: '1234567.8900',
            availableLimit: '765432.1100',
            currency: 'TRY',
            statementClosingDay: 15,
            paymentDueDay: 25,
            minimumPaymentRate: '20.0000',
            isActive: true,
          ),
        ],
      );
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.dark(),
          builder: (context, child) => MediaQuery(
            data: MediaQuery.of(
              context,
            ).copyWith(textScaler: const TextScaler.linear(2)),
            child: child!,
          ),
          home: FinancePage(repository: repository),
        ),
      );
      await tester.pumpAndSettle();
      expectNoOverflow(tester);
      expect(find.text('₺1.234.567,89'), findsOneWidget);
      expect(find.byType(FittedBox), findsNothing);
    },
  );
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
    this.accounts = const [],
    this.installmentPlans = const [],
    this.cardError,
    this.paymentError,
  }) : transfers = transfers ?? [];
  final ApiException? loadError;

  /// Kart ekleme ve düzenlemenin reddi.
  final ApiException? cardError;
  final ApiException? paymentError;
  List<TransferItem> transfers;
  final List<CreditCardItem> cards;
  final CardStatement? currentStatement;
  final CardStatement? periodStatement;
  // Eski kart hareketi ucu kart sayfasında artık okunmuyor; sözleşme için
  // boş cevap.
  final CardActivity activity = const CardActivity(charges: [], payments: []);
  final List<FinanceChoice> accounts;
  final List<InstallmentPlanModel> installmentPlans;
  final List<HistoryPeriod> requestedPeriods = [];
  String? cancelledTransferId;
  Map<String, Object?>? lastPlanInput;

  /// Hareketler sayfasının istekleri ve verilecek cevap.
  final List<CardMovementQuery> pageQueries = [];
  CardPageData Function(CardMovementQuery query)? cardPage;
  List<InstallmentPreviewOption> previews = const [];

  @override
  Future<CardPageData> loadCardPage(
    String cardId,
    CardMovementQuery query, {
    int offset = 0,
    int? limit,
  }) async {
    pageQueries.add(query);
    return cardPage?.call(query) ??
        CardPageData(
          cardId: cardId,
          name: 'Kart',
          period: const CardPagePeriod(kind: 'all'),
          statements: const [],
          spending: '0.0000',
          payments: '0.0000',
          categories: const [],
          items: const [],
          totalCount: 0,
          offset: 0,
          hasMore: false,
        );
  }

  @override
  Future<List<InstallmentPreviewOption>> previewInstallments({
    required String totalAmount,
    required String firstInstallmentDate,
    required List<int> counts,
  }) async => previews;
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
  Future<void> createCard(Map<String, Object?> input) async {
    if (cardError case final error?) throw error;
  }

  @override
  Future<void> updateCard(String cardId, Map<String, Object?> input) async {
    if (cardError case final error?) throw error;
    lastCardUpdate = input;
  }

  @override
  Future<void> createCharge(String cardId, Map<String, Object?> input) async {}

  final List<Map<String, Object?>> payments = [];

  @override
  Future<void> createPayment(String cardId, Map<String, Object?> input) async {
    if (paymentError case final error?) throw error;
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

CardPageData _page({
  CardPagePeriod period = const CardPagePeriod(kind: 'all'),
  String spending = '0.0000',
  String? payments = '0.0000',
}) => CardPageData(
  cardId: 'card-1',
  name: 'Test Kart',
  period: period,
  statements: const [
    CardStatementRange(
      year: 2026,
      month: 11,
      start: '2026-10-16',
      end: '2026-11-15',
    ),
  ],
  spending: spending,
  payments: payments,
  categories: const [],
  items: const [],
  totalCount: 0,
  offset: 0,
  hasMore: false,
);
