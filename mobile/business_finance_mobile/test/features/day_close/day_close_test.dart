import 'dart:convert';

import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/day_close/data/day_close_repository.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_controller.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_detail.dart';
import 'package:business_finance_mobile/features/day_close/presentation/day_close_sheets.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';

import '../../helpers/accessibility.dart';

/// Gün sonu (ADR 0019 T1–T2): panel, gün sonu ayrıntısı ve geri alma,
/// Kasa'daki kart.
void main() {
  group('depo', () {
    test(
      'önizleme yalnız yazılan alanları ve değişen işaretleri gönderir',
      () async {
        late http.Request sent;
        final repository = DayCloseRepository(
          _client((request) async {
            sent = request;
            return _json(_previewJson());
          }),
        );

        final preview = await repository.preview(
          const DayCloseInput(
            date: '2026-10-03',
            cashAmount: '3350.0000',
            posAmounts: {'pos-1': '2680.0000'},
            recordOverrides: {'counterparty-payment/cp-1': true},
          ),
        );

        expect(sent.method, 'POST');
        expect(sent.url.path, '/api/v1/day-closes/preview');
        final body = jsonDecode(sent.body) as Map<String, dynamic>;
        expect(body['cashAmount'], '3350.0000');
        expect(body['totalAmount'], isNull);
        expect(body['posAmounts'], [
          {'posDefinitionId': 'pos-1', 'amount': '2680.0000'},
        ]);
        expect(body['recordOverrides'], [
          {'kind': 'counterparty-payment', 'id': 'cp-1', 'included': true},
        ]);
        // Önizleme istek kimliği taşımaz: hiçbir şey yazmaz.
        expect(body.containsKey('clientRequestId'), isFalse);

        expect(preview.cash.amountToWrite, '2100.0000');
        expect(preview.cash.writes, isTrue);
        expect(preview.posLines.single.commissionAmount, '37.6000');
        expect(preview.existingRecords, hasLength(3));
        expect(preview.existingRecords.first.key, 'income/sale-1');
        expect(preview.existingRecords[1].isCash, isTrue);
        expect(preview.existingRecords[1].included, isFalse);
        expect(preview.existingRecords.last.isCash, isFalse);
        expect(preview.blockerCode, isNull);
        expect(preview.isClosed, isFalse);
      },
    );

    test('kayıt istek kimliğiyle yazılır; liste ve geri alma', () async {
      final requests = <String>[];
      late Map<String, dynamic> created;
      final repository = DayCloseRepository(
        _client((request) async {
          requests.add('${request.method} ${request.url}');
          if (request.method == 'POST') {
            created = jsonDecode(request.body) as Map<String, dynamic>;
            return _json(_dayCloseJson(), statusCode: 201);
          }
          if (request.url.path == '/api/v1/day-closes') {
            return _json({
              'items': [_dayCloseJson()],
            });
          }
          return _json(_dayCloseJson(isCancelled: true));
        }),
      );

      final dayClose = await repository.create(
        clientRequestId: 'request-1',
        input: const DayCloseInput(date: '2026-10-03', isAdditional: true),
      );
      final list = await repository.list(from: '2026-10-03', to: '2026-10-03');
      final reverted = await repository.revert(dayCloseId: 'close-1');

      expect(created['clientRequestId'], 'request-1');
      expect(created['isAdditional'], isTrue);
      expect(requests, [
        'POST https://api.test/api/v1/day-closes',
        'GET https://api.test/api/v1/day-closes?from=2026-10-03&to=2026-10-03',
        'DELETE https://api.test/api/v1/day-closes/close-1',
      ]);
      expect(dayClose.cashAmount, '2100.0000');
      expect(dayClose.recordCount, 2);
      expect(dayClose.settlements.single.dayCloseId, 'close-1');
      expect(dayClose.countedRecords.single.key, 'income/sale-1');
      expect(dayClose.countedCashAmount, '1250.0000');
      expect(list.single.id, 'close-1');
      expect(reverted.isCancelled, isTrue);
    });

    test('para JSON sayısı olarak gelirse reddedilir', () async {
      final repository = DayCloseRepository(
        _client((_) async => _json({..._dayCloseJson(), 'cashAmount': 2100})),
      );

      expect(
        () => repository.get(dayCloseId: 'close-1'),
        throwsFormatException,
      );
    });
  });

  group('controller', () {
    test('kayıt akışı, özeti, bütçeyi, hesapları ve kasayı yeniler', () async {
      final changes = FinancialDataChanges();
      final controller = DayCloseController(
        _FakeRepository(),
        changes: changes,
      );
      addTearDown(controller.dispose);

      final saved = await controller.create(
        clientRequestId: 'r',
        input: const DayCloseInput(date: '2026-10-03', cashAmount: '1.0000'),
      );

      expect(saved, isNotNull);
      expect(changes.activityFeedRevision, 1);
      expect(changes.dashboardRevision, 1);
      expect(changes.budgetsRevision, 1);
      expect(changes.accountsRevision, 1);
      expect(changes.cashRevision, 1);
      expect(changes.cardsRevision, 0);
    });

    test('reddedilen kayıt hiçbir hedefi yükseltmez ve kodunu taşır', () async {
      final changes = FinancialDataChanges();
      final controller = DayCloseController(
        _FakeRepository()
          ..createError = const ApiException(
            code: 'day_closes.already_closed',
            message: 'kapalı',
            statusCode: 409,
          ),
        changes: changes,
      );
      addTearDown(controller.dispose);

      final saved = await controller.create(
        clientRequestId: 'r',
        input: const DayCloseInput(date: '2026-10-03'),
      );

      expect(saved, isNull);
      expect(controller.errorCode, 'day_closes.already_closed');
      expect(changes.activityFeedRevision, 0);
      expect(changes.cashRevision, 0);
    });

    test('başka ekrandan gelen değişiklik bugünü yeniden okutur', () async {
      final changes = FinancialDataChanges();
      final repository = _FakeRepository();
      final controller = DayCloseController(repository, changes: changes);
      addTearDown(controller.dispose);

      await controller.loadDay('2026-10-03');
      expect(controller.today!.isClosed, isFalse);
      expect(repository.dayCalls, 1);

      repository.closes = [_dayClose()];
      // İşlemler'den bir gelir girildi: kasa değişti.
      changes.transactionsChanged();
      await Future<void>.delayed(Duration.zero);

      expect(repository.dayCalls, 2);
      expect(controller.today!.closes, hasLength(1));
    });
  });

  group('gün sonu paneli', () {
    testWidgets('alanları, girilmiş kayıtları ve yazılacakları gösterir', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);

      // Panel boş açılır: POS alanı sunucudan gelir, kayıtlar listelenir.
      expect(find.text('Nakit'), findsOneWidget);
      expect(find.text('Ziraat POS'), findsOneWidget);
      expect(find.text('Toplam'), findsOneWidget);
      expect(find.text('Gün sonu tutarında var mı?'), findsOneWidget);
      expect(find.text('Toptan satış'), findsOneWidget);
      expect(find.text('Ahmet Bakkal'), findsOneWidget);
      // Tutar yazılmadan özet de eksik uyarısı da görünmez.
      expect(find.text('Yazılacak'), findsNothing);
      expect(find.text('Nakit ya da kart tutarını yazın.'), findsNothing);

      await _type(tester, 'day-close-cash', '3350');
      await _type(tester, 'day-close-pos-pos-1', '2680');

      expect(repository.lastPreview!.cashAmount, '3350.0000');
      expect(repository.lastPreview!.posAmounts, {'pos-1': '2680.0000'});
      expect(repository.lastPreview!.totalAmount, isNull);
      expect(find.text('Yazılacak'), findsOneWidget);
      expect(find.text('Nakit satış'), findsOneWidget);
      expect(find.text('Dükkan kasası · ₺1.250,00 düşüldü'), findsOneWidget);
      expect(find.text('₺2.100,00'), findsOneWidget);
      expect(find.text('₺1.880,00'), findsOneWidget);
      expect(
        find.textContaining('komisyon ₺37,60 · 4 Ekim beklenir'),
        findsOneWidget,
      );
      await expectMeetsAccessibility(tester);
    });

    testWidgets('işaret değişince yalnız değişen kayıt gönderilir', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);
      await _type(tester, 'day-close-cash', '3350');

      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();
      expect(repository.lastPreview!.recordOverrides, {
        'counterparty-payment/cp-1': true,
      });

      // Varsayılana dönen işaret gönderilmez.
      await tester.tap(find.text('Ahmet Bakkal'));
      await tester.pumpAndSettle();
      expect(repository.lastPreview!.recordOverrides, isEmpty);
    });

    testWidgets('kaydet girdiyi gönderir ve paneli kapatır', (tester) async {
      final repository = _FakeRepository();
      final changes = FinancialDataChanges();
      await _openForm(tester, repository, changes: changes);
      await _type(tester, 'day-close-cash', '3350');

      await tester.tap(find.text('Gün sonunu kaydet'));
      await tester.pumpAndSettle();

      expect(repository.created, hasLength(1));
      expect(repository.created.single.cashAmount, '3350.0000');
      expect(repository.created.single.date, '2026-10-03');
      expect(find.text('Gün sonunu kaydet'), findsNothing);
      expect(changes.cashRevision, 1);
    });

    testWidgets('tutar yazılmadan kaydedilmez ve eksik söylenir', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);

      await tester.tap(find.text('Gün sonunu kaydet'));
      await tester.pumpAndSettle();

      expect(repository.created, isEmpty);
      expect(find.text('Nakit ya da kart tutarını yazın.'), findsOneWidget);
      expect(find.text('Gün sonunu kaydet'), findsOneWidget);
    });

    testWidgets('işaretli kayıtlar tutarı aşarsa alanın yanında söylenir', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..blocker = 'day_closes.existing_exceeds_cash';
      await _openForm(tester, repository);
      await _type(tester, 'day-close-cash', '1000');

      expect(
        find.textContaining('İşaretli kayıtlar bu tutarı aşıyor'),
        findsOneWidget,
      );
      // Engel varken özet gösterilmez ve kayıt yazılmaz.
      expect(find.text('Yazılacak'), findsNothing);
      await tester.tap(find.text('Gün sonunu kaydet'));
      await tester.pumpAndSettle();
      expect(repository.created, isEmpty);
    });

    testWidgets('toplamdan hesaplanan kart alanın altında yazar', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);
      await _type(tester, 'day-close-cash', '3350');
      await _type(tester, 'day-close-total', '6030');

      expect(repository.lastPreview!.totalAmount, '6030.0000');
      expect(repository.lastPreview!.posAmounts, isEmpty);
      expect(find.text('Toplamdan hesaplandı: ₺2.680,00'), findsOneWidget);
    });

    // Çoğu akşam tek POS'a yazılır: ana POS'un alanı hep görünür, diğerleri
    // istenince açılır (kullanıcı, 4 Ekim 2026: üç POS üç boş alan demekti).
    testWidgets("ana POS görünür; diğer POS'lar istenince açılır", (
      tester,
    ) async {
      final repository = _FakeRepository()..twoPos = true;
      await _openForm(tester, repository);

      expect(find.text('Ziraat POS'), findsOneWidget);
      expect(find.text('Yemek kartı'), findsNothing);

      await tester.tap(find.text("Diğer POS'lar (1)"));
      await tester.pumpAndSettle();

      expect(find.text('Yemek kartı'), findsOneWidget);
      expect(find.textContaining("Diğer POS'lar"), findsNothing);
      await _type(tester, 'day-close-pos-pos-2', '500');
      expect(repository.lastPreview!.posAmounts, {'pos-2': '500.0000'});
    });

    testWidgets('kapalı günde alanlar kilitlidir; ek gün sonu açar', (
      tester,
    ) async {
      final repository = _FakeRepository()..closed = true;
      await _openForm(tester, repository);

      expect(find.textContaining('Bu günün gün sonu girildi'), findsOneWidget);
      expect(_field(tester, 'day-close-cash').enabled, isFalse);
      // Gün kapalıyken yazılacak bir şey yok; liste de görünmez.
      expect(find.text('Gün sonu tutarında var mı?'), findsNothing);

      await tester.tap(find.text('Ek gün sonu'));
      await tester.pumpAndSettle();

      expect(repository.lastPreview!.isAdditional, isTrue);
      expect(_field(tester, 'day-close-cash').enabled, isTrue);
      expect(find.text('Gün sonu tutarında var mı?'), findsOneWidget);
    });

    testWidgets('Kasa kapalı günde paneli ek olarak açar', (tester) async {
      final repository = _FakeRepository()..closed = true;
      await _openForm(tester, repository, additional: true);

      expect(repository.lastPreview!.isAdditional, isTrue);
      expect(_field(tester, 'day-close-cash').enabled, isTrue);
    });

    testWidgets('kasa seçilemediyse kasa ve kategori alanları açılır', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..blocker = 'day_closes.cash_account_required'
        ..cashAccountResolved = false;
      await _openForm(tester, repository);
      await _type(tester, 'day-close-cash', '500');

      expect(find.text('Kasa'), findsOneWidget);
      expect(find.text('Satış kategorisi'), findsOneWidget);
    });

    testWidgets('sunucu hatası panelde kalır', (tester) async {
      final repository = _FakeRepository()
        ..createError = const ApiException(
          code: 'day_closes.concurrent_change',
          message: 'Gün değişti.',
          statusCode: 409,
        );
      await _openForm(tester, repository);
      await _type(tester, 'day-close-cash', '3350');

      await tester.tap(find.text('Gün sonunu kaydet'));
      await tester.pumpAndSettle();

      expect(find.text('Gün değişti.'), findsOneWidget);
      expect(find.text('Gün sonunu kaydet'), findsOneWidget);
    });

    testWidgets('en büyük yazıda taşmaz', (tester) async {
      final repository = _FakeRepository();
      final controller = DayCloseController(repository);
      addTearDown(controller.dispose);
      await pumpAtLargestTextScale(
        tester,
        _host(
          (context) =>
              showDayCloseForm(context, controller, initialDate: '2026-10-03'),
        ),
      );
      await tester.tap(find.text('Aç'));
      await tester.pumpAndSettle();
      await _type(tester, 'day-close-cash', '3350');

      expectNoOverflow(tester);
    });
  });

  group('gün ayrıntısı', () {
    testWidgets('günün toplamını, yazılanı ve sayılanı gösterir', (
      tester,
    ) async {
      await _openDay(tester, _FakeRepository()..closes = [_dayClose()]);

      expect(find.text('Gün sonu'), findsNWidgets(2));
      expect(find.text('3 Ekim 2026'), findsOneWidget);
      expect(find.text('Gün kapatıldı'), findsOneWidget);
      // Günün toplamı sunucudan: yazılan + sayılan.
      expect(find.text('₺3.350,00'), findsOneWidget);
      expect(find.text('Nakit satış'), findsOneWidget);
      expect(find.text('Yazıldı · Dükkan kasası'), findsOneWidget);
      expect(find.text('Ziraat POS'), findsOneWidget);
      expect(find.text('Yazıldı · yolda'), findsOneWidget);
      // Gün sonunun saydığı, tek tek girilmiş kayıt.
      expect(find.text('Toptan satış'), findsOneWidget);
      expect(find.text('Sayıldı · nakit'), findsOneWidget);
      // Gün sonunda sayılmamış kayıt dışarıda durur.
      expect(find.text('Gün sonunun dışında'), findsOneWidget);
      expect(find.text('Ahmet Bakkal'), findsOneWidget);
      expect(find.text('Gün sonunu geri al'), findsOneWidget);
      await expectMeetsAccessibility(tester);
    });

    testWidgets('ana ve ek gün sonu birlikte görünür; ana önce ek ister', (
      tester,
    ) async {
      await _openDay(
        tester,
        _FakeRepository()
          ..closes = [
            _dayClose(),
            DayClose.fromJson({
              ..._dayCloseJson(),
              'id': 'close-2',
              'isAdditional': true,
              'countedRecords': <Object?>[],
            }),
          ],
      );

      expect(find.text('Ek gün sonu'), findsOneWidget);
      // Ana gün sonu, eki geri alınmadan geri alınamaz.
      expect(find.text('Gün sonunu geri al'), findsNothing);
      expect(
        find.text('Geri almak için önce ek gün sonunu geri alın.'),
        findsOneWidget,
      );
      expect(find.text('Eki geri al'), findsOneWidget);
    });

    testWidgets('geri alma onaysız çalışmaz; onaylanınca gün yeniden açılır', (
      tester,
    ) async {
      final repository = _FakeRepository()..closes = [_dayClose()];
      final changes = FinancialDataChanges();
      await _openDay(tester, repository, changes: changes);

      await tester.tap(find.text('Gün sonunu geri al'));
      await tester.pumpAndSettle();
      expect(
        find.textContaining('Yazdığı 2 kayıt birlikte iptal edilir'),
        findsOneWidget,
      );
      await tester.tap(find.text('Vazgeç'));
      await tester.pumpAndSettle();
      expect(repository.reverted, isEmpty);

      await tester.tap(find.text('Gün sonunu geri al'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Geri al'));
      await tester.pumpAndSettle();

      expect(repository.reverted, ['close-1']);
      // Günde gün sonu kalmadı: panel kapanır.
      expect(find.text('Gün kapatıldı'), findsNothing);
      expect(changes.cashRevision, 1);
      expect(changes.budgetsRevision, 1);
    });

    testWidgets('hesaba geçmiş tahsilat ne yapılacağını söyler', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..closes = [_dayClose()]
        ..revertError = const ApiException(
          code: 'day_closes.deposit_locked',
          message: 'Kart parası hesaba geçmiş. Önce yatışı geri alın.',
          statusCode: 409,
        );
      await _openDay(tester, repository);

      await tester.tap(find.text('Gün sonunu geri al'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Geri al'));
      await tester.pumpAndSettle();

      expect(
        find.text('Kart parası hesaba geçmiş. Önce yatışı geri alın.'),
        findsOneWidget,
      );
      expect(find.text('Gün sonunu geri al'), findsOneWidget);
    });

    testWidgets('gün sonu girilmemiş gün bunu söyler', (tester) async {
      await _openDay(tester, _FakeRepository());

      expect(find.text('Gün sonu girilmedi'), findsOneWidget);
      expect(find.text('Tek tek girilenler'), findsOneWidget);
      expect(find.text('Gün sonunu geri al'), findsNothing);
    });

    testWidgets('okunamayan gün yeniden denenir', (tester) async {
      final repository = _FakeRepository()
        ..dayError = const ApiException(
          code: 'network.unavailable',
          message: 'Sunucuya ulaşılamadı.',
        );
      await _openDay(tester, repository);

      expect(find.text('Sunucuya ulaşılamadı.'), findsOneWidget);
      repository.dayError = null;
      await tester.tap(find.text('Tekrar dene'));
      await tester.pumpAndSettle();
      expect(find.text('Gün sonu girilmedi'), findsOneWidget);
    });

    testWidgets('en büyük yazıda taşmaz', (tester) async {
      final controller = DayCloseController(
        _FakeRepository()..closes = [_dayClose()],
      );
      addTearDown(controller.dispose);
      await pumpAtLargestTextScale(
        tester,
        _host(
          (context) => showDayCloseDay(
            context,
            controller: controller,
            date: '2026-10-03',
          ),
        ),
      );
      await tester.tap(find.text('Aç'));
      await tester.pumpAndSettle();

      expectNoOverflow(tester);
    });
  });

  group('Kasa kartı', () {
    Future<List<String>> pumpCard(
      WidgetTester tester,
      _FakeRepository repository,
    ) async {
      final calls = <String>[];
      final controller = DayCloseController(repository);
      addTearDown(controller.dispose);
      await controller.loadDay('2026-10-03');
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: DayCloseTodayCard(
              controller: controller,
              onEnter: ({required additional}) =>
                  calls.add(additional ? 'ek' : 'gir'),
              onOpen: (date) => calls.add('aç $date'),
            ),
          ),
        ),
      );
      return calls;
    }

    testWidgets('gün açıkken birincil eylem gün sonunu girer', (tester) async {
      final calls = await pumpCard(tester, _FakeRepository());

      expect(find.text('Bugün girilmedi'), findsOneWidget);
      await tester.tap(find.text('Gün sonunu gir'));
      expect(calls, ['gir']);
      await expectMeetsAccessibility(tester);
    });

    // Kart günün toplamını tek satırda gösterir (sunucudan); ana ve ek gün
    // sonlarının ayrıntısı günün ekranındadır.
    testWidgets('gün kapalıyken günün toplamını gösterir ve günü açar', (
      tester,
    ) async {
      final calls = await pumpCard(
        tester,
        _FakeRepository()..closes = [_dayClose()],
      );

      expect(find.text('Bugün girildi'), findsOneWidget);
      expect(find.text('Gün sonunu gir'), findsNothing);
      expect(find.text('Nakit ₺3.350,00 · Kart ₺2.680,00'), findsOneWidget);

      await tester.tap(find.text('Nakit ₺3.350,00 · Kart ₺2.680,00'));
      await tester.tap(find.text('Ek gün sonu gir'));
      expect(calls, ['aç 2026-10-03', 'ek']);
    });
  });
}

TextFormField _field(WidgetTester tester, String key) =>
    tester.widget<TextFormField>(find.byKey(ValueKey(key)));

/// Alanı doldurur ve önizlemenin gelmesini bekler.
Future<void> _type(WidgetTester tester, String key, String text) async {
  await tester.enterText(find.byKey(ValueKey(key)), text);
  await tester.pump(const Duration(milliseconds: 400));
  await tester.pumpAndSettle();
}

Widget _host(void Function(BuildContext context) open) => MaterialApp(
  theme: AppTheme.light(),
  home: Scaffold(
    body: Builder(
      builder: (context) => Center(
        child: TextButton(
          onPressed: () => open(context),
          child: const Text('Aç'),
        ),
      ),
    ),
  ),
);

void _useTallView(WidgetTester tester) {
  tester.view.physicalSize = const Size(412, 1800);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
}

Future<void> _openForm(
  WidgetTester tester,
  _FakeRepository repository, {
  FinancialDataChanges? changes,
  bool additional = false,
}) async {
  _useTallView(tester);
  final controller = DayCloseController(repository, changes: changes);
  addTearDown(controller.dispose);
  await tester.pumpWidget(
    _host(
      (context) => showDayCloseForm(
        context,
        controller,
        initialDate: '2026-10-03',
        additional: additional,
      ),
    ),
  );
  await tester.tap(find.text('Aç'));
  await tester.pumpAndSettle();
}

Future<void> _openDay(
  WidgetTester tester,
  _FakeRepository repository, {
  FinancialDataChanges? changes,
}) async {
  _useTallView(tester);
  final controller = DayCloseController(repository, changes: changes);
  addTearDown(controller.dispose);
  await tester.pumpWidget(
    _host(
      (context) =>
          showDayCloseDay(context, controller: controller, date: '2026-10-03'),
    ),
  );
  await tester.tap(find.text('Aç'));
  await tester.pumpAndSettle();
}

DayClose _dayClose({bool isCancelled = false}) =>
    DayClose.fromJson(_dayCloseJson(isCancelled: isCancelled));

/// Sunucunun yerini tutar: girdiye göre önizleme kurar ve gelen istekleri
/// saklar. Tutarlar sabittir; hesap sunucunun işidir.
class _FakeRepository implements DayCloseRepositoryContract {
  DayCloseInput? lastPreview;
  final created = <DayCloseInput>[];
  final reverted = <String>[];
  List<DayClose> closes = [];
  int listCalls = 0;
  int dayCalls = 0;
  ApiException? dayError;
  bool closed = false;
  bool cashAccountResolved = true;
  bool twoPos = false;
  String? blocker;
  ApiException? createError;
  ApiException? revertError;

  @override
  Future<DayClosePreview> preview(DayCloseInput input) async {
    lastPreview = input;
    final cash = input.cashAmount != null;
    final card = input.posAmounts.isNotEmpty;
    final fromTotal = cash && !card && input.totalAmount != null;
    return DayClosePreview.fromJson(
      _previewJson(
        cashStated: cash,
        cardStated: card || fromTotal,
        cardComputed: fromTotal,
        closed: closed,
        cashAccountResolved: cashAccountResolved,
        twoPos: twoPos,
        additional: input.isAdditional,
        overrides: input.recordOverrides,
        blocker:
            blocker ??
            (closed && !input.isAdditional
                ? 'day_closes.already_closed'
                : cash || card
                ? null
                : 'day_closes.amounts_required'),
      ),
    );
  }

  @override
  Future<DayClose> create({
    required String clientRequestId,
    required DayCloseInput input,
  }) async {
    if (createError case final error?) throw error;
    created.add(input);
    return _dayClose();
  }

  @override
  Future<List<DayClose>> list({
    required String from,
    required String to,
  }) async {
    listCalls++;
    return closes;
  }

  /// Sunucunun gün okuması: gün sonları (geri alınan düşer), dışarıda kalan
  /// kayıt ve günün toplamı.
  @override
  Future<DayCloseDay> day({required String date}) async {
    dayCalls++;
    if (dayError case final error?) throw error;
    final live = [
      for (final close in closes)
        if (!reverted.contains(close.id)) close,
    ];
    return DayCloseDay(
      date: date,
      closes: live,
      outsideRecords: [
        DayCloseExistingRecord.fromJson(
          _recordJson(
            'counterparty-payment',
            'cp-1',
            'cash',
            '300.0000',
            'Ahmet Bakkal',
            byDefault: false,
            overrides: const {},
          ),
        ),
      ],
      cashTotal: live.isEmpty ? '0.0000' : '3350.0000',
      cardTotal: live.isEmpty ? '0.0000' : '2680.0000',
      currency: 'TRY',
    );
  }

  @override
  Future<DayClose> get({required String dayCloseId}) async => _dayClose();

  @override
  Future<DayClose> revert({required String dayCloseId}) async {
    if (revertError case final error?) throw error;
    reverted.add(dayCloseId);
    return _dayClose(isCancelled: true);
  }

  @override
  Future<DayCloseOptions> loadOptions() async => const DayCloseOptions(
    cashAccounts: [DataChoice('till', 'Dükkan kasası')],
    categories: [DataChoice('sales', 'Satış geliri')],
  );
}

Map<String, Object?> _previewJson({
  bool cashStated = true,
  bool cardStated = true,
  bool cardComputed = false,
  bool closed = false,
  bool cashAccountResolved = true,
  bool twoPos = false,
  bool additional = false,
  Map<String, bool> overrides = const {},
  String? blocker,
}) => {
  'date': '2026-10-03',
  'rangeStart': null,
  'currency': 'TRY',
  'closedBy': [
    if (closed)
      {
        'id': 'close-1',
        'closedOn': '2026-10-03',
        'rangeStart': null,
        'zNumber': null,
        'isAdditional': false,
      },
  ],
  'cash': {
    'stated': cashStated,
    'enteredAmount': cashStated ? '3350.0000' : '0.0000',
    'isComputed': false,
    'deductedAmount': cashStated ? '1250.0000' : '0.0000',
    'amountToWrite': cashStated ? '2100.0000' : '0.0000',
    'accountId': cashAccountResolved ? 'till' : null,
    'accountName': cashAccountResolved ? 'Dükkan kasası' : null,
    'categoryId': 'sales',
    'categoryName': 'Satış geliri',
  },
  'posLines': [
    {
      'posDefinitionId': 'pos-1',
      'name': 'Ziraat POS',
      'isDefault': true,
      'accountName': 'Ziraat',
      'stated': cardStated,
      'enteredAmount': cardStated ? '2680.0000' : '0.0000',
      'isComputed': cardComputed,
      'deductedAmount': cardStated ? '800.0000' : '0.0000',
      'amountToWrite': cardStated ? '1880.0000' : '0.0000',
      'commissionAmount': cardStated ? '37.6000' : '0.0000',
      'netAmount': cardStated ? '1842.4000' : '0.0000',
      'expectedTransferDate': '2026-10-04',
    },
    if (twoPos)
      {
        'posDefinitionId': 'pos-2',
        'name': 'Yemek kartı',
        'isDefault': false,
        'accountName': 'Ziraat',
        'stated': false,
        'enteredAmount': '0.0000',
        'isComputed': false,
        'deductedAmount': '0.0000',
        'amountToWrite': '0.0000',
        'commissionAmount': '0.0000',
        'netAmount': '0.0000',
        'expectedTransferDate': '2026-10-23',
      },
  ],
  'totalEntered': null,
  'totalComputed': '6030.0000',
  'totalDifference': null,
  'existingRecords': [
    _recordJson(
      'income',
      'sale-1',
      'cash',
      '1250.0000',
      'Toptan satış',
      byDefault: !additional,
      overrides: overrides,
    ),
    _recordJson(
      'counterparty-payment',
      'cp-1',
      'cash',
      '300.0000',
      'Ahmet Bakkal',
      byDefault: false,
      overrides: overrides,
    ),
    _recordJson(
      'pos-settlement',
      'pos-sale-1',
      'card',
      '800.0000',
      'Satış geliri',
      byDefault: !additional,
      overrides: overrides,
      posDefinitionId: 'pos-1',
    ),
  ],
  'blockerCode': blocker,
};

Map<String, Object?> _recordJson(
  String kind,
  String id,
  String side,
  String amount,
  String title, {
  required bool byDefault,
  required Map<String, bool> overrides,
  String? posDefinitionId,
}) => {
  'kind': kind,
  'id': id,
  'side': side,
  'date': '2026-10-03',
  'amount': amount,
  'title': title,
  'posDefinitionId': posDefinitionId,
  'accountName': side == 'cash' ? 'Dükkan kasası' : 'Ziraat',
  'includedByDefault': byDefault,
  'included': overrides['$kind/$id'] ?? byDefault,
};

Map<String, Object?> _dayCloseJson({bool isCancelled = false}) => {
  'id': 'close-1',
  'closedOn': '2026-10-03',
  'rangeStart': null,
  'zNumber': null,
  'isAdditional': false,
  'isCancelled': isCancelled,
  'cancelledAtUtc': isCancelled ? '2026-10-03T19:00:00Z' : null,
  'createdAtUtc': '2026-10-03T18:00:00Z',
  'incomes': [
    {
      'transactionId': 'income-1',
      'accountId': 'till',
      'accountName': 'Dükkan kasası',
      'categoryId': 'sales',
      'categoryName': 'Satış geliri',
      'amount': '2100.0000',
      'date': '2026-10-03',
      'scope': 'business',
      'isCancelled': isCancelled,
    },
  ],
  'settlements': [
    {
      'id': 'settlement-1',
      'accountId': 'bank-1',
      'accountName': 'Ziraat',
      'categoryId': 'sales',
      'categoryName': 'Satış geliri',
      'commissionCategoryId': 'commission',
      'commissionCategoryName': 'POS komisyonu',
      'grossAmount': '1880.0000',
      'commissionAmount': '37.6000',
      'netAmount': '1842.4000',
      'commissionRate': '0.0200',
      'currency': 'TRY',
      'scope': 'business',
      'settlementDate': '2026-10-03',
      'expectedTransferDate': '2026-10-04',
      'transferredOn': null,
      'description': null,
      'isInTransit': !isCancelled,
      'isCancelled': isCancelled,
      'isLate': false,
      'posDefinitionId': 'pos-1',
      'posDefinitionName': 'Ziraat POS',
      'posDepositId': null,
      'dayCloseId': 'close-1',
    },
  ],
  'cashAmount': '2100.0000',
  'cardGrossAmount': '1880.0000',
  'commissionAmount': '37.6000',
  'currency': 'TRY',
  'countedRecords': [
    if (!isCancelled)
      _recordJson(
        'income',
        'sale-1',
        'cash',
        '1250.0000',
        'Toptan satış',
        byDefault: true,
        overrides: const {},
      ),
  ],
  'countedCashAmount': isCancelled ? '0.0000' : '1250.0000',
  'countedCardAmount': '0.0000',
};

ApiClient _client(MockClientHandler handler) => ApiClient(
  config: ApiConfig.fromEnvironment(value: 'https://api.test'),
  httpClient: MockClient(handler),
);

http.Response _json(Map<String, Object?> body, {int statusCode = 200}) =>
    http.Response.bytes(
      utf8.encode(jsonEncode(body)),
      statusCode,
      headers: {'content-type': 'application/json; charset=utf-8'},
    );
