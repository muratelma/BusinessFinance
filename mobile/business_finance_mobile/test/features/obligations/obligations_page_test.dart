import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/obligations/data/obligation_repository.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_controller.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligations_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../../helpers/accessibility.dart';

void main() {
  testWidgets('gecikmiş yükümlülük hesaptan ödenip kapanır', (tester) async {
    final repository = _FakeRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationsPage(controller: ObligationListController(repository)),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Geciken'));
    await tester.pumpAndSettle();
    expect(find.text('Gecikmiş'), findsOneWidget);
    expect(find.textContaining('Ödenecek'), findsOneWidget);

    await tester.tap(find.text('Enerji Tedarik'));
    await tester.pumpAndSettle();
    await tester.tap(find.byType(DropdownButtonFormField<String>));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Sentetik kasa'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Öde ve kapat'));
    await tester.pumpAndSettle();

    expect(repository.settledId, 'obligation-1');
    expect(find.text('Ödemeyi kaydet'), findsNothing);
  });

  // Yanlış yazılan kayıt iptal edilebilir (9 Ekim 2026'ya kadar yoktu: iki
  // kez okutulan fatura düzeltilemiyordu). Açık kayıtta düğme kapanış
  // panelindedir ve onay ister.
  testWidgets('açık kayıt kapanış panelinden iptal edilir', (tester) async {
    final repository = _FakeRepository();
    await tester.pumpWidget(_host(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Geciken'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Enerji Tedarik'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydı iptal et'));
    await tester.pumpAndSettle();

    // Panel kapandı, onay soruluyor; henüz hiçbir şey iptal edilmedi.
    expect(find.text('Ödemeyi kaydet'), findsNothing);
    expect(find.text('Kayıt iptal edilsin mi?'), findsOneWidget);
    expect(find.textContaining('Enerji Tedarik · ₺412,60'), findsOneWidget);
    expect(find.text('Giderlerden ve bekleyenlerden düşer.'), findsOneWidget);
    expect(repository.cancelledId, isNull);

    await tester.tap(find.widgetWithText(FilledButton, 'Kaydı iptal et'));
    await tester.pumpAndSettle();

    expect(repository.cancelledId, 'obligation-1');
    expect(find.text('Kayıt iptal edildi.'), findsOneWidget);
    await tester.tap(find.text('Kapanan'));
    await tester.pumpAndSettle();
    expect(find.text('İptal'), findsOneWidget);
  });

  testWidgets('onaydan vazgeçilirse kayıt yerinde kalır', (tester) async {
    final repository = _FakeRepository();
    await tester.pumpWidget(_host(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Geciken'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Enerji Tedarik'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydı iptal et'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();

    expect(repository.cancelledId, isNull);
    expect(find.text('Enerji Tedarik'), findsOneWidget);
  });

  // Kapanmış kayıtta iptal ödemeyi de geri alır; diyalog bunu söyler.
  testWidgets('ödenmiş kayıt ödemesiyle birlikte iptal edilir', (tester) async {
    final repository = _FakeRepository()..settledId = 'obligation-1';
    await tester.pumpWidget(_host(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kapanan'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Enerji Tedarik'));
    await tester.pumpAndSettle();

    expect(
      find.text('Ödemesi de iptal edilir; para hesaba geri döner.'),
      findsOneWidget,
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Kaydı iptal et'));
    await tester.pumpAndSettle();
    expect(repository.cancelledId, 'obligation-1');
  });

  // Sunucu reddederse (tahsilat gün sonunda sayılmış, kart parası hesaba
  // geçmiş) sebep ekranda yazar ve kayıt listede kalır.
  testWidgets('sunucu iptali reddederse sebebi yazar, kayıt kalır', (
    tester,
  ) async {
    final repository = _FakeRepository()
      ..settledId = 'obligation-1'
      ..refuseCancel = true;
    await tester.pumpWidget(_host(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kapanan'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Enerji Tedarik'));
    await tester.pumpAndSettle();
    await tester.tap(find.widgetWithText(FilledButton, 'Kaydı iptal et'));
    await tester.pumpAndSettle();

    expect(find.textContaining('önce yatışı geri alın'), findsOneWidget);
    expect(find.text('Kapandı'), findsOneWidget);
    expect(find.text('İptal'), findsNothing);
  });

  testWidgets('iptal edilmiş kayda dokunmak bir şey açmaz', (tester) async {
    final repository = _FakeRepository()..cancelledId = 'obligation-1';
    await tester.pumpWidget(_host(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kapanan'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Enerji Tedarik'));
    await tester.pumpAndSettle();

    expect(find.text('Kayıt iptal edilsin mi?'), findsNothing);
    expect(find.text('Ödemeyi kaydet'), findsNothing);
  });

  testWidgets(
    'kapanış paneli iptal düğmesiyle erişilebilirlik kapısını geçer',
    (tester) async {
      await pumpAtLargestTextScale(tester, _host(_FakeRepository()));
      await tester.tap(find.text('Geciken'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Enerji Tedarik'));
      await tester.pumpAndSettle();

      expect(find.text('Kaydı iptal et'), findsOneWidget);
      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    },
  );

  testWidgets('yükümlülük listesi erişilebilirlik kapısını geçer', (
    tester,
  ) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationsPage(
          controller: ObligationListController(_FakeRepository()),
        ),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

Widget _host(_FakeRepository repository) => MaterialApp(
  theme: AppTheme.light(),
  home: ObligationsPage(controller: ObligationListController(repository)),
);

class _FakeRepository implements ObligationRepositoryContract {
  String? settledId;
  String? cancelledId;
  bool refuseCancel = false;

  @override
  Future<void> cancel(String obligationId) async {
    if (refuseCancel) {
      throw const ApiException(
        code: 'obligations.deposit_locked',
        message: 'Kart parası hesaba geçmiş. İptal için önce yatışı geri alın.',
        statusCode: 409,
      );
    }
    cancelledId = obligationId;
  }

  String get _status => cancelledId != null
      ? 'cancelled'
      : settledId == null
      ? 'open'
      : 'settled';

  @override
  Future<List<ObligationItem>> list({required String asOfDate}) async => [
    ObligationItem(
      id: 'obligation-1',
      direction: 'payable',
      amount: '412.6000',
      currency: 'TRY',
      issueDate: '2026-08-05',
      dueDate: '2026-08-20',
      status: _status,
      isOverdue: _status == 'open',
      counterpartyName: 'Enerji Tedarik',
      categoryName: 'Faturalar',
    ),
  ];

  @override
  Future<List<ObligationAccount>> loadActiveAccounts() async => const [
    ObligationAccount(id: 'account-1', name: 'Sentetik kasa'),
  ];

  @override
  Future<void> settle({
    required String obligationId,
    required String? accountId,
    required String settlementDate,
    Map<String, Object?>? card,
  }) async {
    settledId = obligationId;
  }

  @override
  Future<ObligationOptions> loadOptions({required String categoryType}) async =>
      const ObligationOptions(categories: [], counterparties: []);

  @override
  Future<void> create(Map<String, Object?> input) async {}
}
