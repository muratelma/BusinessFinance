import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_state_views.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_repository.dart';
import 'package:business_finance_mobile/features/counterparties/presentation/counterparties_page.dart';

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

Future<void> _pump(WidgetTester tester, _FakeRepository repository) async {
  tester.view.physicalSize = const Size(1080, 3200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: CounterpartiesPage(repository: repository),
    ),
  );
  await tester.pumpAndSettle();
}

class _FakeRepository implements CounterpartyRepositoryContract {
  _FakeRepository({
    this.counterparties = _defaultPeople,
    this.failing = false,
    this.unauthorized = false,
  });

  final List<CounterpartySummary> counterparties;
  final bool failing;
  final bool unauthorized;

  CounterpartyBalanceFilter? lastFilter;
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
  Future<CounterpartiesSnapshot> load(CounterpartyBalanceFilter filter) async {
    lastFilter = filter;
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
    );
  }

  @override
  Future<void> create(String name, String? note) async {}

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
