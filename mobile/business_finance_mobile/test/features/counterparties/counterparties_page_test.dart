import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';
import 'package:business_finance_mobile/core/widgets/app_form_error.dart';
import 'package:business_finance_mobile/core/widgets/app_state_views.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_repository.dart';
import 'package:business_finance_mobile/features/counterparties/presentation/counterparties_page.dart';
import 'package:business_finance_mobile/features/obligations/data/obligation_repository.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';

import '../../helpers/accessibility.dart';

void main() {
  group('cari listesi', () {
    testWidgets('kim borçlu kim alacaklı tek bakışta okunuyor', (tester) async {
      await _pump(tester, _FakeRepository());

      expect(find.text('Ahmet Bakkal'), findsOneWidget);
      expect(find.text('Toptancı Zeynep'), findsOneWidget);
      // Net işaretiyle duruyor: eksi, bizim ona borçlu olduğumuz demek.
      expect(find.text('₺400,00'), findsOneWidget);
      expect(find.text('-₺250,00'), findsOneWidget);
      expect(find.text('Vadesi geçmiş alacak ₺150,00'), findsOneWidget);
    });

    testWidgets('liste gecikmeyi belirleyen sorgu tarihini sunucuya taşır', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _pump(tester, repository);

      expect(repository.lastListAsOfDate, isNotNull);
      expect(
        RegExp(r'^\d{4}-\d{2}-\d{2}$').hasMatch(repository.lastListAsOfDate!),
        isTrue,
      );
    });

    // Kapanmış cari listeden düşmez, ayrı okunur: hesabın kapanmış olması o
    // kişiyle iş yapılmadığı anlamına gelmez.
    testWidgets('kapanmış cari ayrı filtreyle okunuyor', (tester) async {
      final repository = _FakeRepository();
      await _pump(tester, repository);

      await tester.tap(find.text('Kapanmış'));
      await tester.pumpAndSettle();

      expect(repository.lastFilter, CounterpartyBalanceFilter.settled);
    });

    testWidgets('boş liste ne yapılacağını söylüyor', (tester) async {
      await _pump(tester, _FakeRepository(counterparties: const []));

      expect(find.byType(AppEmptyView), findsOneWidget);
      expect(find.textContaining('Veresiye sattığınız'), findsOneWidget);
    });

    testWidgets('yetkisiz oturum kendi ekranını gösteriyor', (tester) async {
      await _pump(tester, _FakeRepository(unauthorized: true));

      expect(find.byType(AppUnauthorizedView), findsOneWidget);
    });

    testWidgets('okuma düşerse sebebi ve yeniden deneme görünüyor', (
      tester,
    ) async {
      await _pump(tester, _FakeRepository(failing: true));

      expect(find.byType(AppErrorView), findsOneWidget);
      expect(find.text('Cari alınamadı.'), findsOneWidget);
    });

    // Aynı adlı kişi reddedilince form kapanmaz ve cümleyi kendi içinde yazar;
    // liste yerinde kalır, "Son bilinen bakiye" rozeti çıkmaz (okuma düşmedi)
    // ve vazgeçince listenin üstünde eski ret durmaz.
    testWidgets('reddedilen kişi formu kapatmaz ve listeyi eskitmez', (
      tester,
    ) async {
      const sentence =
          'Bu adı taşıyan bir Cari hesap kaydı zaten var. Farklı bir ad seçin.';
      await _pump(
        tester,
        _FakeRepository(
          createError: const ApiException(
            code: 'counterparties.duplicate_name',
            message: sentence,
            statusCode: 409,
          ),
        ),
      );

      await tester.tap(find.text('Karşı taraf ekle'));
      await tester.pumpAndSettle();
      await tester.enterText(find.byType(TextFormField).first, 'ahmetbakkal');
      await tester.tap(find.text('Ekle'));
      await tester.pumpAndSettle();

      // Form açık, yazılan ad yerinde, cümle formda.
      expect(find.text('Kişi / kurum'), findsOneWidget);
      expect(find.text('ahmetbakkal'), findsOneWidget);
      expect(find.byType(AppFormError), findsOneWidget);
      expect(find.text(sentence), findsOneWidget);

      await tester.tap(find.text('Vazgeç'));
      await tester.pumpAndSettle();

      expect(find.text(sentence), findsNothing);
      expect(find.text('Ahmet Bakkal'), findsOneWidget);
      expect(find.text('Son bilinen bakiye'), findsNothing);
      expect(find.byType(AppErrorView), findsNothing);
    });
  });

  group('cari ayrıntısı', () {
    testWidgets('iki taraf ayrı, sözleşme ayrı gösteriliyor', (tester) async {
      await _pump(tester, _FakeRepository());
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      expect(find.text('Size borcu'), findsOneWidget);
      expect(find.text('Sizin borcunuz'), findsOneWidget);
      // Sözleşme cari hesabın dışında; kalanı bakiyeye eklenmiyor.
      expect(find.text('Taksitli sözleşmeler'), findsOneWidget);
      expect(find.textContaining('bakiyeye'), findsOneWidget);
    });

    // Aşama 02 kabul turunun bulgusu: sözleşme listesi tarihsiz sorulunca
    // sunucu `request.invalid_format` döndürüyordu ve ayrıntı ekranı gerçek
    // API'de hiç açılmıyordu. Kalan tutar bir tarihe göre hesaplanan
    // projection'dır; tarihsiz sorulamaz.
    testWidgets('ayrıntı okuması sözleşmeler için tarih taşır', (tester) async {
      final repository = _FakeRepository();
      await _pump(tester, repository);
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      expect(repository.lastDetailAsOfDate, isNotNull);
      expect(
        RegExp(r'^\d{4}-\d{2}-\d{2}$').hasMatch(repository.lastDetailAsOfDate!),
        isTrue,
        reason: 'Sunucu yalnız yyyy-MM-dd kabul ediyor.',
      );
    });

    testWidgets('hareket geçmişi feed satırlarından geliyor', (tester) async {
      await _pump(tester, _FakeRepository());
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      expect(find.text('Veresiye satış'), findsWidgets);
    });

    // Pasif karşı tarafa yeni borç yazılamaz ama kalan bakiye tahsil
    // edilebilir; aksi hâlde açık hesap kapanamazdı.
    testWidgets('pasif karşı tarafta borçlandırma kapalı, tahsilat açık', (
      tester,
    ) async {
      await _pump(tester, _FakeRepository());
      await tester.tap(find.text('Toptancı Zeynep'));
      await tester.pumpAndSettle();

      final charge = tester.widget<OutlinedButton>(
        find.widgetWithText(OutlinedButton, 'Veresiye satış'),
      );
      final settle = tester.widget<FilledButton>(
        find.widgetWithText(FilledButton, 'Tahsilat'),
      );
      expect(charge.onPressed, isNull);
      expect(settle.onPressed, isNotNull);
      expect(find.textContaining('pasif'), findsOneWidget);
    });

    testWidgets('tahsilat formu açık bakiyeyle dolu açılıyor', (tester) async {
      final repository = _FakeRepository();
      await _pump(tester, repository);
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      await tester.tap(find.widgetWithText(FilledButton, 'Tahsilat'));
      await tester.pumpAndSettle();

      expect(find.widgetWithText(TextFormField, '400'), findsOneWidget);
      expect(find.textContaining('kısmi tutar'), findsOneWidget);
    });

    testWidgets('tahsilat kaydı yön ve hesapla birlikte gidiyor', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _pump(tester, repository);
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      await tester.tap(find.widgetWithText(FilledButton, 'Tahsilat'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      final payment = repository.lastPayment!;
      expect(payment['direction'], 'receivable');
      expect(payment['amount'], '400.0000');
      expect(payment['accountId'], 'account-1');
      // Kategori ve kapsam **yok**: bu kayıt gelir/gider üretmez.
      expect(payment.containsKey('categoryId'), isFalse);
      expect(payment.containsKey('scope'), isFalse);
    });

    // Kartla tahsil (ADR 0019 T5): müşteri borcunu POS'tan öder. Hesap
    // yerine POS sorulur, para yola çıkar; istek `card` bloğunu taşır.
    testWidgets('tahsilat kartla (POS) alınınca hesap değil POS gidiyor', (
      tester,
    ) async {
      final repository = _FakeRepository();
      final pos = _FakePosRepository();
      await _pump(tester, repository, pos: pos);
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      await tester.tap(find.widgetWithText(FilledButton, 'Tahsilat'));
      await tester.pumpAndSettle();
      expect(find.text('Kartla (POS)'), findsOneWidget);
      expect(find.text('Paranın gireceği hesap'), findsOneWidget);

      await tester.tap(find.text('Kartla (POS)'));
      await tester.pumpAndSettle();
      // Ana POS seçili gelir; hesap sorulmaz, önizleme sunucudan.
      expect(find.text('Paranın gireceği hesap'), findsNothing);
      expect(find.text('Garanti POS'), findsOneWidget);
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pumpAndSettle();
      expect(pos.lastPreviewAmount, '400.0000');
      expect(find.text('₺394,00'), findsOneWidget);

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      final payment = repository.lastPayment!;
      expect(payment['direction'], 'receivable');
      expect(payment.containsKey('accountId'), isFalse);
      expect(payment['card'], {'posDefinitionId': 'pos-1'});
    });

    // Tedarikçiye ödeme POS'tan geçmez (ADR 0019 T7): seçenek çıkmaz.
    testWidgets('ödeme formunda kart seçeneği yok', (tester) async {
      await _pump(tester, _FakeRepository(), pos: _FakePosRepository());
      await tester.tap(find.text('Toptancı Zeynep'));
      await tester.pumpAndSettle();

      await tester.tap(find.widgetWithText(FilledButton, 'Ödeme'));
      await tester.pumpAndSettle();

      expect(find.text('Kartla (POS)'), findsNothing);
      expect(find.text('Ödeme hesabı'), findsOneWidget);
    });

    // Kişiye bağlı fatura cari bakiyenin dışındadır (Aşama 06.3 C3–C5):
    // ayrı blokta durur, toplamı sunucudan gelir ve cari `Ödeme` formu onu
    // önermez. Eskiden bakiyeye giriyor ve aynı borç iki yoldan
    // ödenebiliyordu.
    testWidgets('bekleyen fatura ayrı blokta durur, ödeme formu onu önermez', (
      tester,
    ) async {
      final repository = _FakeRepository(
        counterparties: const [
          CounterpartySummary(
            id: 'cp-2',
            name: 'Toptancı Zeynep',
            isActive: true,
            receivable: '0.0000',
            payable: '500.0000',
            net: '-500.0000',
            isSettled: false,
            openPayableObligations: '1000.0000',
          ),
        ],
        pendingObligations: const [
          ObligationItem(
            id: 'ob-1',
            direction: 'payable',
            amount: '1000.0000',
            currency: 'TRY',
            issueDate: '2026-08-05',
            dueDate: '2026-08-20',
            status: 'open',
            isOverdue: true,
            counterpartyId: 'cp-2',
            description: 'Ağustos faturası',
          ),
        ],
      );
      await _pump(tester, repository);
      await tester.tap(find.text('Toptancı Zeynep'));
      await tester.pumpAndSettle();

      expect(find.text('Bekleyen faturalar'), findsOneWidget);
      expect(find.text('Ağustos faturası'), findsOneWidget);
      expect(find.textContaining('Vade 20 Ağustos'), findsOneWidget);
      expect(find.textContaining('Gecikmiş'), findsOneWidget);
      expect(find.text('Ödenecek toplam'), findsOneWidget);
      // Tahsil edilecek bir şey yok: o satır çizilmez.
      expect(find.text('Tahsil edilecek toplam'), findsNothing);

      await tester.tap(find.widgetWithText(FilledButton, 'Ödeme'));
      await tester.pumpAndSettle();

      // Önerilen tutar yalnız cari borçtur; 1.000 liralık fatura yok.
      expect(find.widgetWithText(TextFormField, '500'), findsOneWidget);
      expect(find.widgetWithText(TextFormField, '1.500'), findsNothing);
    });

    // 2.200 liralık veresiye satışa 2.500 liralık tahsilat: alacak −300 olur.
    // Eskiden `Size borcu −₺300,00` diye, gelir tonunda yazılıyor ve alacak
    // gibi okunuyordu (kullanıcı, 9 Ekim 2026). Satırların adı değişmez;
    // fazla alınan para `Sizin borcunuz` satırında, eksi işaretsiz yazar.
    testWidgets(
      'fazla tahsilat borcumuz satırında yazar, eksi alacak diye değil',
      (tester) async {
        await _pump(
          tester,
          _FakeRepository(
            counterparties: const [
              CounterpartySummary(
                id: 'cp-1',
                name: 'Ahmet Bakkal',
                isActive: true,
                receivable: '-300.0000',
                payable: '0.0000',
                owedToYou: '0.0000',
                owedByYou: '300.0000',
                net: '-300.0000',
                isSettled: false,
              ),
            ],
          ),
        );
        await tester.tap(find.text('Ahmet Bakkal'));
        await tester.pumpAndSettle();

        expect(find.text('Size borcu'), findsOneWidget);
        expect(find.text('Sizin borcunuz'), findsOneWidget);
        expect(find.text('Net - Borcunuz'), findsOneWidget);
        expect(find.text('₺0,00'), findsOneWidget);
        // 300 iki yerde: borç satırı ve net; ikisi de eksi işaretsiz.
        expect(find.text('₺300,00'), findsNWidgets(2));
        expect(find.textContaining('-₺'), findsNothing);
      },
    );

    testWidgets('fazla ödeme size borcu satırında yazar', (tester) async {
      await _pump(
        tester,
        _FakeRepository(
          counterparties: const [
            CounterpartySummary(
              id: 'cp-1',
              name: 'Ahmet Bakkal',
              isActive: true,
              receivable: '0.0000',
              payable: '-150.0000',
              owedToYou: '150.0000',
              owedByYou: '0.0000',
              net: '150.0000',
              isSettled: false,
            ),
          ],
        ),
      );
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      expect(find.text('Net - Alacağınız'), findsOneWidget);
      expect(find.text('₺150,00'), findsNWidgets(2));
      expect(find.textContaining('-₺'), findsNothing);
    });

    testWidgets('kapanmış caride net yalnız Net diye yazar', (tester) async {
      await _pump(
        tester,
        _FakeRepository(
          counterparties: const [
            CounterpartySummary(
              id: 'cp-1',
              name: 'Ahmet Bakkal',
              isActive: true,
              receivable: '0.0000',
              payable: '0.0000',
              net: '0.0000',
              isSettled: true,
            ),
          ],
        ),
      );
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      expect(find.text('Net'), findsOneWidget);
      expect(find.text('Hesap kapandı.'), findsOneWidget);
    });

    testWidgets('bekleyen faturası olmayan kişide blok çizilmez', (
      tester,
    ) async {
      await _pump(tester, _FakeRepository());
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      expect(find.text('Bekleyen faturalar'), findsNothing);
    });

    // Borçlandırma kasaya dokunmaz: form hesap sormaz, kategori sorar.
    testWidgets('veresiye satış formu hesap değil kategori soruyor', (
      tester,
    ) async {
      await _pump(tester, _FakeRepository());
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      await tester.tap(find.widgetWithText(OutlinedButton, 'Veresiye satış'));
      await tester.pumpAndSettle();

      expect(find.text('Kategori'), findsOneWidget);
      expect(find.textContaining('kasa'), findsWidgets);
      expect(find.text('Ödeme hesabı'), findsNothing);
    });

    testWidgets('kapsam görünmüyorsa istekte kapsam gitmiyor', (tester) async {
      final repository = _FakeRepository();
      await _pump(tester, repository);
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      await tester.tap(find.widgetWithText(OutlinedButton, 'Veresiye satış'));
      await tester.pumpAndSettle();
      await tester.enterText(find.byType(TextFormField).first, '120');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      final charge = repository.lastCharge!;
      expect(charge['direction'], 'receivable');
      expect(charge['amount'], '120.0000');
      expect(charge['categoryId'], 'category-income');
      expect(charge.containsKey('scope'), isFalse);
      expect(charge.containsKey('dueDate'), isFalse);
    });

    testWidgets('opsiyonel vade seçilirse cari hareket isteğine ekleniyor', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _pump(tester, repository);
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();

      await tester.tap(find.widgetWithText(OutlinedButton, 'Veresiye satış'));
      await tester.pumpAndSettle();
      await tester.enterText(find.byType(TextFormField).first, '120');
      final dueFinder = find.byWidgetPredicate(
        (widget) =>
            widget is AppDateField && widget.label == 'Vade (isteğe bağlı)',
      );
      final dueField = tester.widget<AppDateField>(dueFinder);
      dueField.onChanged('2026-08-20');
      await tester.pump();
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(repository.lastCharge!['dueDate'], '2026-08-20');
    });
  });

  testWidgets('cari ekranları erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: CounterpartiesPage(repository: _FakeRepository()),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

Future<void> _pump(
  WidgetTester tester,
  _FakeRepository repository, {
  PosRepositoryContract? pos,
}) async {
  tester.view.physicalSize = const Size(1080, 3200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: CounterpartiesPage(repository: repository, posRepository: pos),
    ),
  );
  await tester.pumpAndSettle();
}

/// Kartla tahsilin okuduğu iki şey: POS'lar ve önizleme.
class _FakePosRepository implements PosRepositoryContract {
  String? lastPreviewAmount;

  @override
  Future<List<PosDefinitionItem>> listDefinitions() async => const [
    PosDefinitionItem(
      id: 'pos-1',
      name: 'Garanti POS',
      accountId: 'bank-1',
      accountName: 'Garanti Vadesiz',
      salesCategoryId: 'category-income',
      salesCategoryName: 'Satış geliri',
      commissionRate: '0.0150',
      transferDays: 1,
      businessDaysOnly: false,
      isActive: true,
      isDefault: true,
    ),
  ];

  @override
  Future<PosPreview> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  }) async {
    lastPreviewAmount = grossAmount;
    return const PosPreview(
      commissionAmount: '6.0000',
      netAmount: '394.0000',
      currency: 'TRY',
      expectedTransferDate: '2026-10-06',
    );
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _FakeRepository implements CounterpartyRepositoryContract {
  _FakeRepository({
    this.counterparties = _defaultPeople,
    this.failing = false,
    this.unauthorized = false,
    this.pendingObligations = const [],
    this.createError,
  });

  final ApiException? createError;
  final List<CounterpartySummary> counterparties;
  final List<ObligationItem> pendingObligations;
  final bool failing;
  final bool unauthorized;

  CounterpartyBalanceFilter? lastFilter;
  String? lastListAsOfDate;
  String? lastDetailAsOfDate;
  Map<String, Object?>? lastCharge;
  Map<String, Object?>? lastPayment;

  static const _defaultPeople = [
    CounterpartySummary(
      id: 'cp-1',
      name: 'Ahmet Bakkal',
      isActive: true,
      receivable: '400.0000',
      payable: '0.0000',
      overdueReceivable: '150.0000',
      notOverdueReceivable: '250.0000',
      net: '400.0000',
      isSettled: false,
    ),
    CounterpartySummary(
      id: 'cp-2',
      name: 'Toptancı Zeynep',
      isActive: false,
      receivable: '0.0000',
      payable: '250.0000',
      net: '-250.0000',
      isSettled: false,
    ),
  ];

  @override
  Future<CounterpartiesSnapshot> load(
    CounterpartyBalanceFilter filter,
    String asOfDate,
  ) async {
    lastFilter = filter;
    lastListAsOfDate = asOfDate;
    if (unauthorized) {
      throw const ApiException(
        code: 'auth.required',
        message: 'Oturum gerekli.',
        statusCode: 401,
      );
    }
    if (failing) {
      throw const ApiException(
        code: 'counterparties.unavailable',
        message: 'Cari alınamadı.',
        statusCode: 500,
      );
    }
    return CounterpartiesSnapshot(
      counterparties: counterparties,
      accounts: const [DataChoice('account-1', 'Kasa')],
      categories: const [
        DataChoice('category-income', 'Satış geliri', type: 'income'),
        DataChoice('category-expense', 'Mal alımı', type: 'expense'),
      ],
    );
  }

  @override
  Future<CounterpartyDetail> loadDetail(
    String counterpartyId,
    String asOfDate,
  ) async {
    lastDetailAsOfDate = asOfDate;
    final person = counterparties.firstWhere(
      (item) => item.id == counterpartyId,
    );
    return CounterpartyDetail(
      counterparty: person,
      activities: [
        FinancialActivity(
          activityId: 'activity-1',
          kind: ActivityKind.counterpartyCharge,
          effect: ActivityEffect.income,
          sourceGroup: ActivitySourceGroup.counterparty,
          origin: ActivityOrigin.manual,
          status: ActivityStatus.realized,
          activityDate: '2026-08-05',
          amount: '400.0000',
          currency: 'TRY',
          title: 'Veresiye satış',
          canCancel: true,
          supportsAttachments: false,
        ),
      ],
      agreements: person.id == 'cp-1'
          ? const [
              CounterpartyAgreement(
                id: 'debt-1',
                isReceivable: true,
                remaining: '600.0000',
                installmentCount: 2,
                paidCount: 1,
              ),
            ]
          : const [],
      pendingObligations: pendingObligations
          .where((item) => item.counterpartyId == counterpartyId)
          .toList(growable: false),
    );
  }

  @override
  Future<void> create(String name, String? note) async {
    if (createError case final error?) throw error;
  }

  @override
  Future<void> update(
    String counterpartyId, {
    required String name,
    required bool isActive,
    String? note,
  }) async {}

  @override
  Future<void> delete(String counterpartyId) async {}

  @override
  Future<void> addCharge(
    String counterpartyId,
    Map<String, Object?> input,
  ) async => lastCharge = input;

  @override
  Future<void> addPayment(
    String counterpartyId,
    Map<String, Object?> input,
  ) async => lastPayment = input;
}
