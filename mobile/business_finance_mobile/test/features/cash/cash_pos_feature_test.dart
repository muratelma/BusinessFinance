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
import 'package:business_finance_mobile/features/day_close/data/day_close_repository.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_controller.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_controller.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_settlements_view.dart';
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

  // 28 Eylül denetimi U11: Kasa'dan açılan nakit gider kaydedildi, Kasa
  // ekranı eski bakiyeyi göstermeye devam etti.
  test('başka ekrandaki nakit hareket Kasa ve POS listesini yeniler', () async {
    final cashRepository = _FakeCashRepository();
    final posRepository = _FakePosRepository();
    final changes = FinancialDataChanges();
    final cash = CashCountController(cashRepository, changes: changes);
    final pos = PosController(posRepository, changes: changes);
    await cash.load();
    await pos.load();
    expect(cashRepository.todayLoads, 1);
    expect(cashRepository.accountLoads, 1);
    expect(posRepository.listLoads, 1);

    changes.transactionsChanged();
    await pumpEventQueue();

    expect(cashRepository.todayLoads, 2);
    // Kasa listesi de yeniden okunur: yeni açılan ya da kapanan bir kasa
    // önbellekte kalmasın.
    expect(cashRepository.accountLoads, 2);
    expect(posRepository.listLoads, 2);

    cash.dispose();
    pos.dispose();
    changes.transferChanged();
    await pumpEventQueue();
    expect(cashRepository.todayLoads, 2, reason: 'kapanan ekran dinlemez');
  });

  // 28 Eylül denetimi U12: yanlış POS kaydı düzeltilemiyordu. Hesaba geçişin
  // geri alınması yatışın işidir (`pos_deposit_test.dart`).
  test('yoldaki kayıt iptal edilir; yalnız etkiledikleri yenilenir', () async {
    final repository = _FakePosRepository();
    final changes = FinancialDataChanges();
    final controller = PosController(repository, changes: changes);
    await controller.create(
      accountId: 'bank-account',
      categoryId: 'income-category',
      grossAmount: '100.0000',
      settlementDate: '2026-08-24',
      expectedTransferDate: '2026-08-26',
    );
    final item = repository.current.items.single;
    final budgetsBefore = changes.budgetsRevision;
    final accountsBefore = changes.accountsRevision;
    final loadsBefore = repository.listLoads;

    expect(await controller.cancel(item), isTrue);
    await pumpEventQueue();
    expect(repository.cancelledId, item.id);
    expect(controller.items, isEmpty);
    // İptal satışı ve komisyonu düşürür: bütçe de yenilenir.
    expect(changes.budgetsRevision, budgetsBefore + 1);
    // Yoldaki para hiçbir hesapta değildi: hesaplar yenilenmez.
    expect(changes.accountsRevision, accountsBefore);
    // Kendi değişikliğinden sonra bir kez yüklenir.
    expect(repository.listLoads, loadsBefore + 1);
    controller.dispose();
  });

  test('POS deposu iptali DELETE ile gönderir', () async {
    final requests = <String>[];
    final repository = PosRepository(
      _client((request) async {
        requests.add('${request.method} ${request.url.path}');
        return http.Response('{}', 200);
      }),
    );

    await repository.cancel(settlementId: 'pos-1');

    expect(requests, ['DELETE /api/v1/pos-settlements/pos-1']);
  });

  // Kullanıcı, 29 Eylül: bugünün sayımı "Son sayımlar" listesinde de
  // görünsün; iptal edilen (yerine yenisi yazılan) sayım görünmesin.
  test('Son sayımlar bugünün sayımını da gösterir', () async {
    final today = CashCountItem.fromJson({
      ..._cashCountJson,
      'id': 'today',
      'countDate': '2026-09-25',
    });
    final superseded = CashCountItem.fromJson({
      ..._cashCountJson,
      'id': 'superseded',
      'countDate': '2026-09-25',
      'isCancelled': true,
    });
    final repository = _FakeCashRepository()
      ..currentCount = today
      ..previous = superseded;
    final controller = CashCountController(
      repository,
      clock: () => DateTime(2026, 9, 25, 18),
    );
    await controller.load();

    expect(controller.recentCounts.map((item) => item.id), ['today']);
  });

  test('Kasa kendi sayımından sonra kendini bir kez yükler', () async {
    final repository = _FakeCashRepository();
    final changes = FinancialDataChanges();
    final controller = CashCountController(repository, changes: changes);
    await controller.load();

    await controller.recordCount(
      countedAmount: '100.0000',
      countDate: '2026-08-24',
      scope: TransactionScope.business,
    );
    await pumpEventQueue();

    expect(repository.todayLoads, 2);
    expect(repository.accountLoads, 1);
    controller.dispose();
  });

  test(
    'POS tahsilatı satışı tanır; yatış yalnız hesabı hareket ettirir',
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
        await controller.createDeposit(
          clientRequestId: 'request-1',
          settlementIds: [repository.current.items.single.id],
          depositedAmount: '97.5000',
          depositDate: '2026-08-25',
        ),
        isTrue,
      );
      expect(repository.depositInput?['depositDate'], '2026-08-25');
      expect(controller.items.single.isInTransit, isFalse);
      expect(changes.cashRevision, 2);
      expect(changes.dashboardRevision, 2);
      expect(changes.accountsRevision, 1);
      // Beklendiği kadar yatan para hiçbir gider tanımaz: bütçe yükselmez,
      // yoksa aynı satış iki kez sayılırdı. Feed ise yükselir; yatış kendi
      // satırını doğuruyor.
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

  testWidgets('POS ayrıntısı yoldaki kaydı onayla iptal eder; hesaba geçmiş '
      'kayıt yatışına götürür', (tester) async {
    tester.view.physicalSize = const Size(412, 1200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakePosRepository();
    final controller = PosController(repository);
    final transferred = PosSettlementItem.fromJson({
      ..._posItemJson,
      'transferredOn': '2026-08-25',
      'isInTransit': false,
      'posDepositId': 'deposit-1',
    });

    Future<void> show(PosSettlementItem item) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: PosSettlementSheet(item: item, controller: controller),
          ),
        ),
      );
      await tester.pumpAndSettle();
    }

    // Hesaba geçmiş kayıt yatışa bağlıdır: iptal sunulmaz, yol yatışadır.
    await show(transferred);
    expect(find.text('Yatışı gör'), findsOneWidget);
    expect(find.text('Kaydı iptal et'), findsNothing);
    expect(find.text('Hesaba geçti'), findsOneWidget, reason: 'durum etiketi');
    expect(
      find.text('Kaydı iptal etmek için önce yatışı geri alın.'),
      findsOneWidget,
    );

    // Yoldaki kayıtta görülecek bir yatış yok; iptal vardır ve onaysız
    // çalışmaz.
    await show(PosSettlementItem.fromJson(_posItemJson));
    expect(find.text('Yatışı gör'), findsNothing);
    expect(find.text('Hesaba geçti'), findsOneWidget, reason: 'eylem düğmesi');
    await tester.tap(find.text('Kaydı iptal et'));
    await tester.pumpAndSettle();
    expect(find.text('POS tahsilatı iptal edilsin mi?'), findsOneWidget);
    expect(repository.cancelledId, isNull);
    await tester.tap(find.text('İptal et'));
    await tester.pumpAndSettle();
    expect(repository.cancelledId, 'pos-settlement');

    // Gün sonunda sayılmış kayıt iptal sunmaz; sunucu zaten reddederdi.
    await show(
      PosSettlementItem.fromJson({
        ..._posItemJson,
        'countedInDayCloseId': 'close-1',
      }),
    );
    expect(find.text('Kaydı iptal et'), findsNothing);
    expect(find.text('Hesaba geçti'), findsOneWidget, reason: 'eylem düğmesi');
    expect(
      find.text('Gün sonunda sayıldı. İptal için gün sonunu geri alın.'),
      findsOneWidget,
    );
  });

  // 28 Eylül denetimi U10 (T1b): sayımdan sonra girilen dünkü gider ekranı
  // değiştirmiyor, kart "oturdu" demeye devam ediyordu.
  testWidgets('sayımdan sonra değişen kasa "oturdu" demez', (tester) async {
    tester.view.physicalSize = const Size(412, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeCashRepository()
      ..currentCount = _cashCount(
        difference: '-60.0000',
        adjustmentTransactionId: 'adjustment',
      )
      ..changeSinceCount = '-25.0000';
    await tester.pumpWidget(_app(cash: repository));
    await tester.pumpAndSettle();

    expect(find.text('Sonradan kayıt girildi'), findsOneWidget);
    expect(find.text('Fark kaydedildi'), findsNothing);
    expect(find.text('−₺25,00 sayımdan sonra girildi'), findsOneWidget);
    expect(find.textContaining('oturdu'), findsNothing);
    expect(find.text('Emin olmak için yeniden sayın.'), findsOneWidget);
  });

  // Özet'teki `Yolda` satırı Kasa'yı POS bölümüne kaydırılmış açar.
  testWidgets('Kasa POS bölümüne kaydırılmış açılabilir', (tester) async {
    tester.view.physicalSize = const Size(412, 500);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(_app(initialTab: 3));
    await tester.pumpAndSettle();

    final top = tester.getTopLeft(find.text('POS tahsilatları')).dy;
    expect(top, lessThan(200), reason: 'bölüm başlığı ekranın üstünde');
    // Hiçbir panel açılmaz: bu yalnız bir konumdur.
    expect(find.text('Müşterinin ödediği'), findsNothing);
  });

  // Aşama 06.3 K9: esnafın kasadan kendine aldığı para yeni bir kayıt türü
  // değildir; şahsi hesaba aktarım ya da şahsi gider olarak yazılır.
  group('Kendime aldım', () {
    Future<_FakeCashRepository> open(
      WidgetTester tester, {
      List<DataChoice> personal = const [],
      FinancialDataChanges? changes,
    }) async {
      tester.view.physicalSize = const Size(412, 1400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      final repository = _FakeCashRepository()..personalAccounts = personal;
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: CashPage(
            cashController: CashCountController(
              repository,
              changes: changes,
              clock: () => DateTime(2026, 9, 25, 18),
            ),
            posController: PosController(_FakePosRepository()),
          ),
        ),
      );
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kendime aldım'));
      await tester.pumpAndSettle();
      return repository;
    }

    testWidgets('şahsi hesap varsa aktarım seçili gelir ve transfer yazılır', (
      tester,
    ) async {
      final changes = FinancialDataChanges();
      final repository = await open(
        tester,
        personal: const [DataChoice('wallet', 'Şahsi cüzdan')],
        changes: changes,
      );

      expect(find.text('Şahsi hesaba aktar'), findsOneWidget);
      expect(find.text('Şahsi gider'), findsOneWidget);
      await tester.enterText(find.byType(TextFormField).first, '250');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(
        repository.withdrawals.single,
        startsWith('transfer cash-account'),
      );
      expect(repository.withdrawals.single, contains('>wallet 250.0000'));
      // Transfer gelir/gider değildir: bütçe yenilenmez, kasa ve hesaplar
      // yenilenir.
      expect(changes.budgetsRevision, 0);
      expect(changes.accountsRevision, 1);
      expect(changes.cashRevision, 1);
      expect(changes.activityFeedRevision, 1);
    });

    testWidgets('şahsi hesap yoksa şahsi gider olarak yazılır', (tester) async {
      final changes = FinancialDataChanges();
      final repository = await open(tester, changes: changes);

      // Aktarılacak yer yok: ray çizilmez, kategori sorulur.
      expect(find.text('Şahsi hesaba aktar'), findsNothing);
      await tester.enterText(find.byType(TextFormField).first, '80,50');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();
      expect(find.text('Kategori seçin.'), findsOneWidget);
      expect(repository.withdrawals, isEmpty);

      await tester.tap(find.byKey(const ValueKey('withdrawal-category')));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kasa farkı').last);
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(
        repository.withdrawals.single,
        startsWith('expense cash-account/income-category 80.5000'),
      );
      expect(changes.budgetsRevision, 1);
    });
  });

  // Aşama 06.3 K7: eksik farkta sebep sorulur; fazlada sorulmaz.
  group('fark kaydında sebep', () {
    Future<_FakeCashRepository> open(
      WidgetTester tester,
      String difference,
    ) async {
      tester.view.physicalSize = const Size(412, 1400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      final repository = _FakeCashRepository()
        ..currentCount = _cashCount(difference: difference);
      await tester.pumpWidget(_app(cash: repository));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Farkı kaydet'));
      await tester.pumpAndSettle();
      return repository;
    }

    testWidgets('fazla çıkan farkta sebep sorulmaz', (tester) async {
      await open(tester, '40.0000');

      expect(find.text('Fazlayı kaydet'), findsOneWidget);
      expect(find.text('Bilmiyorum'), findsNothing);
    });

    testWidgets('"Bilmiyorum" kategori sormaz', (tester) async {
      final repository = await open(tester, '-100.0000');

      expect(find.text('Gider'), findsOneWidget);
      await tester.tap(find.text('Bilmiyorum'));
      await tester.pumpAndSettle();
      // Sebebi bilmeyen kullanıcıya kategori seçtirilmez (K10).
      expect(find.byType(DropdownButtonFormField<String>), findsNothing);
      expect(find.textContaining('"Kasa farkı" kategorisine'), findsOneWidget);
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(repository.confirmedUnknown, isTrue);
      expect(repository.confirmedCategoryId, isNull);
    });

    testWidgets('"Kendime aldım" fark kaydı yazmaz, paneli tutarla açar', (
      tester,
    ) async {
      final repository = await open(tester, '-100.0000');

      await tester.tap(find.text('Kendime aldım').last);
      await tester.pumpAndSettle();
      await tester.tap(find.text('Devam'));
      await tester.pumpAndSettle();

      expect(repository.confirmed, isFalse);
      expect(find.text('Tutar'), findsOneWidget);
      expect(find.text('100,00'), findsOneWidget);
    });
  });

  // Aşama 06.3 K8: işletme profilinde şahsi cüzdan Kasa'da gösterilmez.
  group('şahsi etiketli nakit hesap', () {
    test('işletme profilinde gizlenir, etiketsiz ve işletme kalır', () async {
      final controller = CashCountController(
        _FakeCashRepository()..twoAccounts = true,
        hidesPersonalAccounts: () => true,
      );
      await controller.load();

      expect(controller.accounts.map((account) => account.id), [
        'cash-account',
      ]);
      expect(controller.hasOnlyHiddenAccounts, isFalse);
      controller.dispose();
    });

    test('kişisel profilde görünür', () async {
      final controller = CashCountController(
        _FakeCashRepository()..twoAccounts = true,
        hidesPersonalAccounts: () => false,
      );
      await controller.load();

      expect(controller.accounts, hasLength(2));
      controller.dispose();
    });

    testWidgets('hepsi gizliyse Kasa bunu söyler', (tester) async {
      tester.view.physicalSize = const Size(412, 1400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: CashPage(
            cashController: CashCountController(
              _FakeCashRepository()..onlyPersonal = true,
              hidesPersonalAccounts: () => true,
            ),
            posController: PosController(_FakePosRepository()),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('İşletme kasası yok.'), findsOneWidget);
      expect(find.text('Sayılacak bir kasa yok.'), findsNothing);
    });
  });

  // Aşama 06.3 K6: önceki sayımın kaydedilmemiş farkı bilgi satırıdır;
  // bugünkü fark ondan düşülmez ve ikiye bölünmez.
  testWidgets('önceki sayımın kaydedilmemiş farkı bilgi olarak durur', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(412, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeCashRepository()
      ..previous = _previousCount
      ..carriedDifference = '-100.0000';
    await tester.pumpWidget(_app(cash: repository));
    await tester.pumpAndSettle();

    // Sayım girilmeden önce de görünür.
    expect(find.text('Kaydedilmemiş fark'), findsOneWidget);
    expect(find.text('24 Eylül sayımından'), findsOneWidget);
    expect(find.text('-₺100,00'), findsOneWidget);
  });

  testWidgets('aynı fark yeniden sayılınca kayıt öne çıkmaz', (tester) async {
    tester.view.physicalSize = const Size(412, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeCashRepository()
      ..previous = _previousCount
      ..currentCount = _cashCount(difference: '-100.0000')
      ..carriedDifference = '-100.0000'
      ..sameAsPrevious = true;
    await tester.pumpWidget(_app(cash: repository));
    await tester.pumpAndSettle();

    expect(find.text('Fark son sayımdakiyle aynı.'), findsOneWidget);
    expect(find.text('Kaydedilmemiş fark'), findsOneWidget);
    // Fark tek sayıdır; kayıt hâlâ mümkündür ama birincil düğme değildir.
    expect(
      find.ancestor(
        of: find.text('Farkı kaydet'),
        matching: find.byWidgetPredicate((widget) => widget is OutlinedButton),
      ),
      findsOneWidget,
    );
  });

  // 29 Eylül emülatör denemesi: tutan sayımdan sonra dün tarihli 300 TL fatura
  // girildi. Fark gerçek olabilir (fatura dün ödendiyse) ya da olmayabilir;
  // uygulama saati bilmediği için ekran iki ihtimali de söyler.
  testWidgets('sayımdan sonra girilen kayıt iki ihtimali de söyler', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(412, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeCashRepository()
      ..currentCount = _cashCount(difference: '300.0000')
      ..changeSinceCount = '-300.0000';
    await tester.pumpWidget(_app(cash: repository));
    await tester.pumpAndSettle();

    expect(find.text('Sonradan kayıt girildi'), findsOneWidget);
    // Ayrı satır yok: değişim "Uygulamaya göre"nin altında kısa açıklama,
    // altında tek cümle.
    expect(find.text('Sayımdan sonra girilen'), findsNothing);
    expect(find.text('−₺300,00 sayımdan sonra girildi'), findsOneWidget);
    expect(
      find.text('Kayıt sayımdan önce olduysa farkı kaydedin.'),
      findsOneWidget,
    );
    expect(find.text('Farkı kaydet'), findsOneWidget);
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

  // Gün sonu Kasa'nın en üstünde durur (ADR 0019 T1): gün açıkken birincil
  // eylem paneli açar. Gün sonu satışı yazar; kasa sayımı ayrı bir iştir ve
  // kendi kartındadır.
  testWidgets('Kasa gün sonu kartını gösterir ve paneli açar', (tester) async {
    final dayClose = DayCloseController(_FakeDayCloseRepository());
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CashPage(
          cashController: CashCountController(
            _FakeCashRepository(),
            clock: () => DateTime(2026, 9, 25, 18),
          ),
          posController: PosController(_FakePosRepository()),
          dayCloseController: dayClose,
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Gün sonu'), findsOneWidget);
    expect(find.text('Bugün girilmedi'), findsOneWidget);
    expect(find.text('Gün sonu sayımı'), findsNothing);
    await expectMeetsAccessibility(tester);

    await tester.tap(find.text('Gün sonunu gir'));
    await tester.pumpAndSettle();
    expect(find.text('Gün sonunu kaydet'), findsOneWidget);
  });

  // `İşlem ekle > Gün sonu` Kasa'yı paneliyle açar; vazgeçen kullanıcı
  // geldiği ekrana döner.
  testWidgets('menüden gelen gün sonu paneli vazgeçince geri döner', (
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
                    dayCloseController: DayCloseController(
                      _FakeDayCloseRepository(),
                    ),
                    initialTab: 2,
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
    expect(find.text('Gün sonunu kaydet'), findsOneWidget);

    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();
    expect(find.text('Başlangıç'), findsOneWidget);
    expect(find.text('Kasa'), findsNothing);
  });
}

class _FakeDayCloseRepository implements DayCloseRepositoryContract {
  @override
  Future<DayClosePreview> preview(DayCloseInput input) async => DayClosePreview(
    date: input.date,
    currency: 'TRY',
    closedBy: const [],
    cash: const DayCloseCashLine(
      stated: false,
      enteredAmount: '0.0000',
      isComputed: false,
      deductedAmount: '0.0000',
      amountToWrite: '0.0000',
    ),
    posLines: const [],
    totalComputed: '0.0000',
    existingRecords: const [],
    blockerCode: 'day_closes.amounts_required',
  );

  @override
  Future<List<DayClose>> list({
    required String from,
    required String to,
  }) async => const [];

  @override
  Future<DayCloseDay> day({required String date}) async => DayCloseDay(
    date: date,
    closes: const [],
    outsideRecords: const [],
    cashTotal: '0.0000',
    cardTotal: '0.0000',
    currency: 'TRY',
  );

  @override
  Future<DayClose> create({
    required String clientRequestId,
    required DayCloseInput input,
  }) async => throw UnimplementedError();

  @override
  Future<DayClose> get({required String dayCloseId}) async =>
      throw UnimplementedError();

  @override
  Future<DayClose> revert({required String dayCloseId}) async =>
      throw UnimplementedError();

  @override
  Future<DayCloseOptions> loadOptions() async =>
      const DayCloseOptions(cashAccounts: [], categories: []);
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
  List<DataChoice> personalAccounts = const [];
  final withdrawals = <String>[];

  @override
  Future<List<DataChoice>> loadPersonalAccounts() async => personalAccounts;

  @override
  Future<void> withdrawToAccount({
    required String cashAccountId,
    required String personalAccountId,
    required String amount,
    required String date,
  }) async {
    withdrawals.add('transfer $cashAccountId>$personalAccountId $amount $date');
  }

  @override
  Future<void> withdrawAsExpense({
    required String cashAccountId,
    required String categoryId,
    required String amount,
    required String date,
  }) async {
    withdrawals.add('expense $cashAccountId/$categoryId $amount $date');
  }

  CashCountItem? currentCount;
  CashCountItem? previous;
  String? createdScope;
  String? createdAmount;
  String? createdDate;
  bool twoAccounts = false;
  bool onlyPersonal = false;
  bool confirmed = false;
  String? confirmedCategoryId;
  bool confirmedUnknown = false;
  int accountLoads = 0;
  int todayLoads = 0;
  String? changeSinceCount;
  String? carriedDifference;
  bool sameAsPrevious = false;

  @override
  Future<List<CashAccount>> loadCashAccounts() async {
    accountLoads++;
    return [
      if (!onlyPersonal)
        const CashAccount(
          id: 'cash-account',
          name: 'Merkez kasa',
          defaultScope: TransactionScope.business,
        ),
      if (twoAccounts || onlyPersonal)
        const CashAccount(
          id: 'wallet',
          name: 'Şahsi cüzdan',
          defaultScope: TransactionScope.personal,
        ),
    ];
  }

  @override
  Future<CashCountToday> loadToday({required String accountId}) async {
    todayLoads++;
    return CashCountToday(
      accountId: accountId,
      accountName: 'Merkez kasa',
      expectedBalance: '100.0000',
      currency: 'TRY',
      count: currentCount,
      previousCount: previous,
      todayInflow: '2450.0000',
      todayOutflow: '665.0000',
      changeSinceCount: changeSinceCount,
      previousUnrecordedDifference: carriedDifference,
      differenceSameAsPrevious: sameAsPrevious,
    );
  }

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
    String? categoryId,
    bool unknownReason = false,
  }) async {
    confirmed = true;
    confirmedCategoryId = categoryId;
    confirmedUnknown = unknownReason;
    return currentCount = _cashCount(
      difference: '5.0000',
      adjustmentTransactionId: 'adjustment',
    );
  }
}

class _FakePosRepository implements PosRepositoryContract {
  PosSettlementList current = const PosSettlementList(
    items: [],
    moneyInTransit: '0.0000',
    inTransitCount: 0,
  );
  Map<String, Object?>? created;
  Map<String, Object?>? depositInput;
  int listLoads = 0;

  @override
  Future<PosSettlementList> list({
    required bool inTransitOnly,
    String? from,
    String? to,
  }) async {
    listLoads++;
    return current;
  }

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
  Future<PosDepositPreview> previewDeposit({
    required List<String> settlementIds,
    String? depositedAmount,
  }) async => throw UnimplementedError();

  @override
  Future<PosDeposit> createDeposit({
    required String clientRequestId,
    required List<String> settlementIds,
    required String depositedAmount,
    required String depositDate,
    String? deductionCategoryId,
  }) async {
    depositInput = {
      'clientRequestId': clientRequestId,
      'settlementIds': settlementIds,
      'depositedAmount': depositedAmount,
      'depositDate': depositDate,
      'deductionCategoryId': deductionCategoryId,
    };
    current = PosSettlementList(
      items: [
        PosSettlementItem.fromJson({
          ..._posItemJson,
          'transferredOn': depositDate,
          'isInTransit': false,
          'posDepositId': 'deposit-1',
        }),
      ],
      moneyInTransit: '0.0000',
      inTransitCount: 0,
    );
    return PosDeposit(
      id: 'deposit-1',
      accountName: 'Banka',
      depositDate: depositDate,
      expectedAmount: '97.5000',
      depositedAmount: depositedAmount,
      deductionAmount: '0.0000',
      currency: 'TRY',
      isCancelled: false,
      settlements: current.items,
    );
  }

  @override
  Future<PosDeposit> getDeposit({required String depositId}) async =>
      throw UnimplementedError();

  @override
  Future<PosDeposit> revertDeposit({required String depositId}) async =>
      throw UnimplementedError();

  String? cancelledId;

  @override
  Future<void> cancel({required String settlementId}) async {
    cancelledId = settlementId;
    current = const PosSettlementList(
      items: [],
      moneyInTransit: '0.0000',
      inTransitCount: 0,
    );
  }

  @override
  Future<List<PosDefinitionItem>> listDefinitions() async => const [];

  @override
  Future<void> saveDefinition(
    PosDefinitionInput input, {
    String? definitionId,
  }) async {}

  @override
  Future<void> setDefinitionActive({
    required String definitionId,
    required bool isActive,
  }) async {}

  @override
  Future<void> deleteDefinition({required String definitionId}) async {}

  @override
  Future<void> setDefaultDefinition({required String definitionId}) async {}

  @override
  Future<PosPreview> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  }) async => throw UnimplementedError();
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
