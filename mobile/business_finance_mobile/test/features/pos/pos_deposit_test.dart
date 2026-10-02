import 'dart:async';
import 'dart:convert';

import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_controller.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_deposit_controller.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_deposit_sheets.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_settlements_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';

import '../../helpers/accessibility.dart';

/// POS yatışı (ADR 0019 T5): "Hesaba geçenleri işaretle", yatış ayrıntısı ve
/// geri alma.
void main() {
  group('depo', () {
    test(
      'önizleme seçimi sorgu dizesiyle, tutarı metin olarak gönderir',
      () async {
        late Uri requested;
        final repository = PosRepository(
          _client((request) async {
            requested = request.url;
            expect(request.method, 'GET');
            return _json(_previewJson);
          }),
        );

        final preview = await repository.previewDeposit(
          settlementIds: ['a', 'b'],
          depositedAmount: '190.0000',
        );

        expect(requested.path, '/api/v1/pos-deposits/preview');
        expect(requested.queryParametersAll['settlementIds'], ['a', 'b']);
        expect(requested.queryParameters['depositedAmount'], '190.0000');
        expect(preview.expectedAmount, '195.0000');
        expect(preview.deductionAmount, '5.0000');
        expect(preview.hasDeduction, isTrue);
        expect(preview.deductionCategoryId, 'expense-category');
        expect(preview.earliestDepositDate, '2026-08-25');
      },
    );

    test('tutar verilmeyen önizleme tutar parametresi taşımaz', () async {
      late Uri requested;
      final repository = PosRepository(
        _client((request) async {
          requested = request.url;
          return _json({
            ..._previewJson,
            'depositedAmount': '195.0000',
            'deductionAmount': '0.0000',
          });
        }),
      );

      final preview = await repository.previewDeposit(settlementIds: ['a']);

      expect(requested.queryParameters.containsKey('depositedAmount'), isFalse);
      expect(preview.hasDeduction, isFalse);
    });

    test('yatış gövdeyle yazılır, DELETE ile geri alınır', () async {
      final requests = <String>[];
      late Map<String, dynamic> sent;
      final repository = PosRepository(
        _client((request) async {
          requests.add('${request.method} ${request.url.path}');
          if (request.method == 'POST') {
            sent = jsonDecode(request.body) as Map<String, dynamic>;
            return _json(_depositJson, statusCode: 201);
          }
          return _json(
            request.method == 'DELETE'
                ? {..._depositJson, 'isCancelled': true, 'settlements': []}
                : _depositJson,
          );
        }),
      );

      final created = await repository.createDeposit(
        clientRequestId: 'request-1',
        settlementIds: ['a', 'b'],
        depositedAmount: '190.0000',
        depositDate: '2026-08-27',
        deductionCategoryId: 'expense-category',
      );
      final read = await repository.getDeposit(depositId: 'deposit-1');
      final reverted = await repository.revertDeposit(depositId: 'deposit-1');

      expect(requests, [
        'POST /api/v1/pos-deposits',
        'GET /api/v1/pos-deposits/deposit-1',
        'DELETE /api/v1/pos-deposits/deposit-1',
      ]);
      expect(sent['clientRequestId'], 'request-1');
      expect(sent['settlementIds'], ['a', 'b']);
      expect(sent['depositedAmount'], '190.0000');
      expect(sent['depositedAmount'], isA<String>());
      expect(sent['depositDate'], '2026-08-27');
      expect(sent['deductionCategoryId'], 'expense-category');
      expect(created.depositedAmount, '190.0000');
      expect(created.hasDeduction, isTrue);
      expect(created.settlements, hasLength(2));
      expect(created.settlements.first.posDepositId, 'deposit-1');
      expect(read.deductionCategoryName, 'POS komisyonu');
      expect(reverted.isCancelled, isTrue);
      expect(reverted.settlements, isEmpty);
    });

    test('para JSON sayısı olarak gelirse reddedilir', () {
      expect(
        () => PosDeposit.fromJson({..._depositJson, 'depositedAmount': 190.0}),
        throwsFormatException,
      );
      expect(
        () => PosDepositPreview.fromJson({
          ..._previewJson,
          'expectedAmount': 195,
        }),
        throwsFormatException,
      );
    });
  });

  group('controller', () {
    test(
      'kesintili yatış bütçeyi de yeniler; kesintisiz yatış yenilemez',
      () async {
        final repository = _FakeRepository();
        final changes = FinancialDataChanges();
        final controller = PosController(repository, changes: changes);
        await controller.load();

        expect(
          await controller.createDeposit(
            clientRequestId: 'request-1',
            settlementIds: ['a'],
            depositedAmount: '97.5000',
            depositDate: '2026-08-27',
          ),
          isTrue,
        );
        // Beklendiği kadar yatan para gider tanımaz.
        expect(changes.budgetsRevision, 0);
        expect(changes.accountsRevision, 1);
        expect(changes.activityFeedRevision, 1);
        expect(changes.dashboardRevision, 1);
        expect(changes.cashRevision, 1);

        expect(
          await controller.createDeposit(
            clientRequestId: 'request-2',
            settlementIds: ['b'],
            depositedAmount: '90.0000',
            depositDate: '2026-08-27',
            deductionCategoryId: 'expense-category',
          ),
          isTrue,
        );
        // Eksik yatan kısım kesinti gideridir: bütçe de yenilenir.
        expect(changes.budgetsRevision, 1);
        expect(changes.accountsRevision, 2);
        controller.dispose();
      },
    );

    test('yatış hatası panele yazılır ve hiçbir hedef yükselmez', () async {
      final repository = _FakeRepository()
        ..createError = ApiException.local(
          'pos_deposits.amount_exceeds_expected',
          statusCode: 400,
        );
      final changes = FinancialDataChanges();
      final controller = PosController(repository, changes: changes);
      await controller.load();

      expect(
        await controller.createDeposit(
          clientRequestId: 'request-1',
          settlementIds: ['a'],
          depositedAmount: '500.0000',
          depositDate: '2026-08-27',
        ),
        isFalse,
      );
      expect(controller.errorMessage, contains('beklenenden fazla olamaz'));
      expect(controller.isSubmitting, isFalse);
      expect(changes.accountsRevision, 0);
      expect(changes.activityFeedRevision, 0);
      controller.dispose();
    });

    test('önizleme hatası formu durdurmaz, nedenini taşır', () async {
      final repository = _FakeRepository()
        ..previewError = ApiException.local(
          'pos_deposits.settlement_not_in_transit',
          statusCode: 409,
        );
      final controller = PosController(repository);

      expect(await controller.previewDeposit(settlementIds: ['a']), isNull);
      expect(controller.depositPreviewError, contains('yolda değil'));

      repository.previewError = null;
      expect(await controller.previewDeposit(settlementIds: ['a']), isNotNull);
      expect(controller.depositPreviewError, isNull);
      controller.dispose();
    });

    test(
      'yatış ayrıntısı geri almayı duyurur; kesinti varsa bütçeyle',
      () async {
        final repository = _FakeRepository()
          ..deposit = _deposit(deduction: '5.0000');
        final changes = FinancialDataChanges();
        final controller = PosDepositController(
          repository,
          'deposit-1',
          changes: changes,
        );
        await controller.load();
        expect(controller.deposit!.hasDeduction, isTrue);

        expect(await controller.revert(), isTrue);

        expect(repository.revertedId, 'deposit-1');
        expect(controller.deposit!.isCancelled, isTrue);
        expect(changes.accountsRevision, 1);
        expect(changes.cashRevision, 1);
        expect(changes.activityFeedRevision, 1);
        // Kesinti gideri iptal oldu: bütçe de yenilenir.
        expect(changes.budgetsRevision, 1);
        // Geri alınmış yatış ikinci kez geri alınmaz.
        expect(await controller.revert(), isFalse);
        expect(changes.accountsRevision, 1);
        controller.dispose();
      },
    );

    test('yatış ayrıntısı yükleme hatasını ve biten oturumu ayırır', () async {
      final repository = _FakeRepository()
        ..getError = ApiException.local(
          'pos_deposits.not_found',
          statusCode: 404,
        );
      final controller = PosDepositController(repository, 'deposit-1');
      await controller.load();
      expect(controller.deposit, isNull);
      expect(controller.errorMessage, isNotNull);
      expect(controller.unauthorized, isFalse);

      repository.getError = ApiException.local(
        'authentication.required',
        statusCode: 401,
      );
      await controller.load();
      expect(controller.unauthorized, isTrue);

      repository
        ..getError = null
        ..deposit = _deposit();
      await controller.load();
      expect(controller.deposit, isNotNull);
      expect(controller.errorMessage, isNull);
      expect(controller.unauthorized, isFalse);
      controller.dispose();
    });
  });

  group('yatış formu', () {
    testWidgets('günü gelmiş tahsilatları seçili açar ve beklenen tutarı '
        'sunucudan yazar', (tester) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);

      expect(find.text('Hesaba geçenleri işaretle'), findsOneWidget);
      expect(_box(tester, 'a').value, isTrue);
      expect(_box(tester, 'b').value, isTrue);
      // Günü gelmemiş tahsilat seçili gelmez.
      expect(_box(tester, 'c').value, isFalse);
      // Beklenen tutar istemcide toplanmaz: tutarsız ilk önizlemeden gelir.
      expect(repository.previews.single.amount, isNull);
      expect(repository.previews.single.ids, unorderedEquals(['a', 'b']));
      expect(_amountText(tester), '195');
      expect(find.text('Kesinti'), findsNothing);
      expect(find.text('Kesinti kategorisi'), findsNothing);
    });

    testWidgets('başka hesabın tahsilatı aynı yatışta seçilemez', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);

      expect(_box(tester, 'c').onChanged, isNull);
      expect(find.textContaining('Bir yatış tek hesaba düşer'), findsOneWidget);

      // Seçim boşalınca her tahsilat yeniden seçilebilir.
      await tester.tap(find.text('Pazartesi satışı'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Salı satışı'));
      await tester.pumpAndSettle();
      expect(_box(tester, 'c').onChanged, isNotNull);
      expect(_amountText(tester), isEmpty);

      await tester.tap(find.text('Yemek kartı'));
      await tester.pumpAndSettle();
      expect(_box(tester, 'a').onChanged, isNull);
      expect(_amountText(tester), '50');
    });

    testWidgets('tek tahsilattan açılınca yalnız o seçili gelir', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository, initial: 'b');

      expect(_box(tester, 'a').value, isFalse);
      expect(_box(tester, 'b').value, isTrue);
      expect(repository.previews.single.ids, ['b']);
    });

    testWidgets('eksik yatan tutar kesintiyi dolu kategoriyle gösterir ve '
        'öyle kaydeder', (tester) async {
      final repository = _FakeRepository();
      final changes = FinancialDataChanges();
      await _openForm(tester, repository, changes: changes);

      await tester.enterText(_amountField, '190');
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pumpAndSettle();

      // Kesinti sunucudan gelir; kategori POS'un komisyon kategorisiyle dolu.
      expect(repository.previews.last.amount, '190.0000');
      expect(find.text('Kesinti'), findsOneWidget);
      expect(find.text('Kesinti kategorisi'), findsOneWidget);
      expect(find.text('POS komisyonu'), findsOneWidget);

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      final created = repository.created!;
      expect(created['settlementIds'], unorderedEquals(['a', 'b']));
      expect(created['depositedAmount'], '190.0000');
      expect(created['depositDate'], AppDateField.format(DateTime.now()));
      expect(created['deductionCategoryId'], 'expense-category');
      expect(created['clientRequestId'], isNotEmpty);
      expect(find.text('Hesaba geçenleri işaretle'), findsNothing);
      expect(changes.budgetsRevision, 1);
      expect(changes.accountsRevision, 1);
    });

    testWidgets('beklendiği kadar yatan para kategori sormadan kaydedilir', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(repository.created!['depositedAmount'], '195.0000');
      expect(repository.created!['deductionCategoryId'], isNull);
    });

    testWidgets('beklenenden fazla tutar kaydedilmez', (tester) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);

      await tester.enterText(_amountField, '200');
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pumpAndSettle();

      expect(find.textContaining('Beklenenden fazla olamaz'), findsOneWidget);
      // Fazla yatan tutarda kesinti alanı çıkmaz.
      expect(find.text('Kesinti kategorisi'), findsNothing);

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(repository.created, isNull);
      expect(find.text('Hesaba geçenleri işaretle'), findsOneWidget);
    });

    testWidgets('seçim olmadan kaydedilmez', (tester) async {
      final repository = _FakeRepository()
        ..items = [_settlement('c', 'Yemek kartı', expected: '2999-01-01')];
      await _openForm(tester, repository);

      expect(_box(tester, 'c').value, isFalse);
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(find.text('En az bir tahsilat seçin.'), findsOneWidget);
      expect(repository.created, isNull);
    });

    testWidgets('sunucu hatası panelde görünür ve panel açık kalır', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..createError = ApiException.local(
          'pos_deposits.settlement_not_in_transit',
          statusCode: 409,
        );
      await _openForm(tester, repository);

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(find.textContaining('artık yolda değil'), findsOneWidget);
      expect(find.text('Hesaba geçenleri işaretle'), findsOneWidget);
    });

    testWidgets('yolda tahsilat yokken kaydetme kapalıdır', (tester) async {
      final repository = _FakeRepository()..items = [];
      await _openForm(tester, repository);

      expect(find.text('Yolda tahsilat yok.'), findsOneWidget);
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();
      expect(repository.created, isNull);
    });

    testWidgets('en büyük yazı ölçeğinde taşmaz', (tester) async {
      final repository = _FakeRepository();
      final controller = PosController(repository);
      await controller.load();
      await pumpAtLargestTextScale(
        tester,
        _host((context) => showPosDepositForm(context, controller)),
        surfaceSize: const Size(412, 1400),
      );
      await tester.tap(find.text('Aç'));
      await tester.pumpAndSettle();
      await tester.enterText(_amountField, '190');
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pumpAndSettle();

      expect(find.text('Kesinti kategorisi'), findsOneWidget);
      expectNoOverflow(tester);
    });

    testWidgets('erişilebilirlik kapısını geçer', (tester) async {
      final repository = _FakeRepository();
      await _openForm(tester, repository);
      await tester.enterText(_amountField, '190');
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pumpAndSettle();

      await expectMeetsAccessibility(tester);
    });
  });

  group('yatış ayrıntısı', () {
    testWidgets(
      'kesintiyi ve kapattığı tahsilatları gösterir, onayla geri alır',
      (tester) async {
        final repository = _FakeRepository()
          ..deposit = _deposit(deduction: '5.0000');
        final changes = FinancialDataChanges();
        await _openDetail(tester, repository, changes: changes);

        expect(find.text('POS yatışı'), findsOneWidget);
        expect(find.text('Hesaba yatan tutar'), findsOneWidget);
        // Yatan tutarın nereden geldiği: satış, komisyon, beklenen.
        expect(find.text('Satış'), findsOneWidget);
        expect(find.text('Komisyon'), findsOneWidget);
        expect(find.text('₺200,00'), findsOneWidget);
        expect(find.text('-₺5,00'), findsNWidgets(2));
        expect(find.text('Beklenen'), findsOneWidget);
        expect(find.text('Kesinti'), findsOneWidget);
        expect(find.text('POS komisyonu'), findsOneWidget);
        expect(find.text('Kapattığı tahsilatlar'), findsOneWidget);
        expect(find.text('Pazartesi satışı'), findsOneWidget);
        expect(find.text('Salı satışı'), findsOneWidget);

        await tester.tap(find.text('Yatışı geri al'));
        await tester.pumpAndSettle();
        expect(find.text('Yatış geri alınsın mı?'), findsOneWidget);
        expect(repository.revertedId, isNull, reason: 'onaysız geri alınmaz');
        expect(
          find.textContaining('kesinti gideri iptal edilir'),
          findsOneWidget,
        );

        await tester.tap(find.text('Geri al'));
        await tester.pumpAndSettle();

        expect(repository.revertedId, 'deposit-1');
        expect(find.text('POS yatışı'), findsNothing, reason: 'panel kapanır');
        expect(changes.accountsRevision, 1);
        expect(changes.budgetsRevision, 1);
      },
    );

    testWidgets('kesintisiz yatış kesinti satırı göstermez', (tester) async {
      final repository = _FakeRepository()..deposit = _deposit();
      await _openDetail(tester, repository);

      expect(find.textContaining('Kesinti'), findsNothing);
      expect(
        find.text(
          'Yatış gelir yazmaz; satış ve komisyon tahsil edildiği gün yazıldı.',
        ),
        findsOneWidget,
      );
    });

    testWidgets('geri alınmış yatış kayıt olarak durur, geri alma sunmaz', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..deposit = _deposit(deduction: '5.0000', cancelled: true);
      await _openDetail(tester, repository);

      expect(find.text('Geri alındı'), findsOneWidget);
      expect(find.text('Yatışı geri al'), findsNothing);
      expect(find.text('Kapattığı tahsilatlar'), findsNothing);
      expect(
        find.text('Kayıt duruyor; tahsilatlar yeniden yolda.'),
        findsOneWidget,
      );
    });

    testWidgets('yükleme hatası yeniden denenebilir', (tester) async {
      final repository = _FakeRepository()
        ..getError = ApiException.local('network.unavailable');
      await _openDetail(tester, repository);

      expect(find.text('POS yatışı'), findsNothing);
      repository
        ..getError = null
        ..deposit = _deposit();
      await tester.tap(find.text('Tekrar dene'));
      await tester.pumpAndSettle();

      expect(find.text('POS yatışı'), findsOneWidget);
    });

    // 2 Ekim 2026 emülatör turu: "Yatış yükleniyor" ekranın %90'ını kaplayan
    // bir pencere açıyor, içerik gelince pencere küçülüyordu.
    testWidgets('beklerken ekranı kaplamaz', (tester) async {
      final repository = _FakeRepository()
        ..deposit = _deposit()
        ..getPending = Completer<void>();
      _useTallView(tester);
      await tester.pumpWidget(
        _host(
          (context) => showPosDepositDetail(
            context,
            repository: repository,
            depositId: 'deposit-1',
          ),
        ),
      );
      await tester.tap(find.text('Aç'));
      await tester.pump();
      await tester.pump(const Duration(milliseconds: 400));

      expect(find.text('Yatış yükleniyor'), findsOneWidget);
      expect(
        tester.getSize(find.byType(PosDepositDetailSheet)).height,
        lessThan(400),
      );

      repository.getPending!.complete();
      await tester.pumpAndSettle();
      expect(find.text('POS yatışı'), findsOneWidget);
    });

    testWidgets('özetle açılınca hemen ve son boyutunda çizilir', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..deposit = _deposit(deduction: '5.0000')
        ..getPending = Completer<void>();
      _useTallView(tester);
      await tester.pumpWidget(
        _host(
          (context) => showPosDepositDetail(
            context,
            repository: repository,
            depositId: 'deposit-1',
            initial: const PosDepositSummary(
              depositedAmount: '190.0000',
              depositDate: '2026-08-27',
              accountName: 'Ziraat',
              currency: 'TRY',
              isCancelled: false,
              deductionAmount: '5.0000',
              settlementCount: 2,
            ),
          ),
        ),
      );
      await tester.tap(find.text('Aç'));
      await tester.pump();
      await tester.pump(const Duration(milliseconds: 400));

      expect(find.text('Yatış yükleniyor'), findsNothing);
      expect(find.text('Hesaba yatan tutar'), findsOneWidget);
      expect(find.text('Kesinti'), findsOneWidget);
      expect(find.text('Kapattığı tahsilatlar'), findsOneWidget);
      expect(find.text('Bakiye'), findsOneWidget);
      final before = tester.getSize(find.byType(PosDepositDetailSheet)).height;

      repository.getPending!.complete();
      await tester.pumpAndSettle();

      expect(find.text('Pazartesi satışı'), findsOneWidget);
      expect(find.text('…'), findsNothing);
      // Kategori satırı yüklenen kayıttan gelir; panel yalnız onun kadar
      // büyür, tam ekrandan küçülme yoktur.
      final after = tester.getSize(find.byType(PosDepositDetailSheet)).height;
      expect(after - before, inInclusiveRange(0, 60));
    });

    testWidgets('özetle açılan panelde okuma hatası yeniden denenir', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..getError = ApiException.local('network.unavailable');
      _useTallView(tester);
      await tester.pumpWidget(
        _host(
          (context) => showPosDepositDetail(
            context,
            repository: repository,
            depositId: 'deposit-1',
            initial: const PosDepositSummary(
              depositedAmount: '195.0000',
              depositDate: '2026-08-27',
              accountName: 'Ziraat',
              currency: 'TRY',
              isCancelled: false,
              settlementCount: 2,
            ),
          ),
        ),
      );
      await tester.tap(find.text('Aç'));
      await tester.pumpAndSettle();

      expect(find.text('Hesaba yatan tutar'), findsOneWidget);
      expect(find.text('Tekrar dene'), findsOneWidget);
      repository
        ..getError = null
        ..deposit = _deposit();
      await tester.tap(find.text('Tekrar dene'));
      await tester.pumpAndSettle();

      expect(find.text('Tekrar dene'), findsNothing);
      expect(find.text('Salı satışı'), findsOneWidget);
    });

    testWidgets('geri alma hatası panelde görünür', (tester) async {
      final repository = _FakeRepository()
        ..deposit = _deposit()
        ..revertError = ApiException.local(
          'pos_deposits.concurrent_change',
          statusCode: 409,
        );
      await _openDetail(tester, repository);

      await tester.tap(find.text('Yatışı geri al'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Geri al'));
      await tester.pumpAndSettle();

      expect(find.textContaining('bu sırada değişti'), findsOneWidget);
      expect(find.text('Yatışı geri al'), findsOneWidget);
    });

    testWidgets('erişilebilirlik kapısını geçer ve büyük yazıda taşmaz', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..deposit = _deposit(deduction: '5.0000');
      await pumpAtLargestTextScale(
        tester,
        _host(
          (context) => showPosDepositDetail(
            context,
            repository: repository,
            depositId: 'deposit-1',
          ),
        ),
        surfaceSize: const Size(412, 1400),
      );
      await tester.tap(find.text('Aç'));
      await tester.pumpAndSettle();

      expect(find.text('Yatışı geri al'), findsOneWidget);
      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    });
  });

  group('POS tahsilatları bölümü', () {
    testWidgets('yolda tahsilat varken "Hesaba geçenleri işaretle" durur', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _pumpSection(tester, repository);

      await tester.tap(find.text('Hesaba geçenleri işaretle'));
      await tester.pumpAndSettle();

      expect(find.text('Yoldaki tahsilatlar'), findsOneWidget);
      expect(repository.previews, isNotEmpty);
    });

    testWidgets('yolda tahsilat yokken işaretleme eylemi görünmez', (
      tester,
    ) async {
      final repository = _FakeRepository()
        ..items = [
          _settlement('a', 'Pazartesi satışı', depositId: 'deposit-1'),
        ];
      await _pumpSection(tester, repository);

      expect(find.text('Hesaba geçenleri işaretle'), findsNothing);
      expect(find.text('Pazartesi satışı'), findsOneWidget);
    });

    testWidgets('ayrıntıdaki "Hesaba geçti" yatış panelini o tahsilatla açar', (
      tester,
    ) async {
      final repository = _FakeRepository();
      await _pumpSection(tester, repository);

      await tester.tap(find.text('Salı satışı'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Hesaba geçti'));
      await tester.pumpAndSettle();

      // Ayrıntı kapandı, yerine yatış paneli açıldı.
      expect(find.text('Kaydı iptal et'), findsNothing);
      expect(find.text('Yoldaki tahsilatlar'), findsOneWidget);
      expect(repository.previews.single.ids, ['b']);
    });

    testWidgets('hesaba geçmiş tahsilat yatışına götürür', (tester) async {
      final repository = _FakeRepository()
        ..items = [_settlement('a', 'Pazartesi satışı', depositId: 'deposit-1')]
        ..deposit = _deposit();
      await _pumpSection(tester, repository);

      await tester.tap(find.text('Pazartesi satışı'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Yatışı gör'));
      await tester.pumpAndSettle();

      expect(repository.requestedDepositId, 'deposit-1');
      expect(find.text('POS yatışı'), findsOneWidget);
      expect(find.text('Yatışı geri al'), findsOneWidget);
    });
  });
}

final _amountField = find.widgetWithText(TextFormField, 'Yatan tutar');

String _amountText(WidgetTester tester) =>
    tester.widget<TextFormField>(_amountField).controller!.text;

/// Yatış formundaki onay kutusu; tahsilatın kimliğiyle bulunur.
Checkbox _box(WidgetTester tester, String id) =>
    tester.widget<Checkbox>(find.byKey(ValueKey('deposit-settlement-$id')));

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
  tester.view.physicalSize = const Size(412, 1400);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
}

Future<PosController> _openForm(
  WidgetTester tester,
  _FakeRepository repository, {
  String? initial,
  FinancialDataChanges? changes,
}) async {
  _useTallView(tester);
  final controller = PosController(repository, changes: changes);
  addTearDown(controller.dispose);
  await controller.load();
  await tester.pumpWidget(
    _host(
      (context) =>
          showPosDepositForm(context, controller, initialSettlementId: initial),
    ),
  );
  await tester.tap(find.text('Aç'));
  await tester.pumpAndSettle();
  return controller;
}

Future<void> _openDetail(
  WidgetTester tester,
  _FakeRepository repository, {
  FinancialDataChanges? changes,
}) async {
  _useTallView(tester);
  await tester.pumpWidget(
    _host(
      (context) => showPosDepositDetail(
        context,
        repository: repository,
        depositId: 'deposit-1',
        changes: changes,
      ),
    ),
  );
  await tester.tap(find.text('Aç'));
  await tester.pumpAndSettle();
}

Future<void> _pumpSection(
  WidgetTester tester,
  _FakeRepository repository,
) async {
  _useTallView(tester);
  final controller = PosController(repository);
  addTearDown(controller.dispose);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(
        body: SingleChildScrollView(child: PosSection(controller: controller)),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

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

PosSettlementItem _settlement(
  String id,
  String description, {
  String accountId = 'bank-1',
  String accountName = 'Ziraat',
  String net = '97.5000',
  String expected = '2026-08-26',
  String? depositId,
}) => PosSettlementItem(
  id: id,
  accountId: accountId,
  accountName: accountName,
  categoryName: 'Satış',
  grossAmount: net,
  commissionAmount: '0.0000',
  netAmount: net,
  currency: 'TRY',
  settlementDate: '2026-08-24',
  expectedTransferDate: expected,
  isInTransit: depositId == null,
  isLate: false,
  transferredOn: depositId == null ? null : '2026-08-27',
  description: description,
  posDepositId: depositId,
);

PosDeposit _deposit({String deduction = '0.0000', bool cancelled = false}) {
  final deducted = double.parse(deduction);
  return PosDeposit(
    id: 'deposit-1',
    accountName: 'Ziraat',
    depositDate: '2026-08-27',
    expectedAmount: '195.0000',
    grossAmount: cancelled ? null : '200.0000',
    commissionAmount: cancelled ? null : '5.0000',
    depositedAmount: (195 - deducted).toStringAsFixed(4),
    deductionAmount: deduction,
    deductionCategoryName: deducted > 0 ? 'POS komisyonu' : null,
    currency: 'TRY',
    isCancelled: cancelled,
    settlements: cancelled
        ? const []
        : [
            _settlement('a', 'Pazartesi satışı', depositId: 'deposit-1'),
            _settlement('b', 'Salı satışı', depositId: 'deposit-1'),
          ],
  );
}

class _PreviewCall {
  const _PreviewCall(this.ids, this.amount);

  final List<String> ids;
  final String? amount;
}

/// Sunucunun yerini tutar: beklenen toplamı ve kesintiyi **o** hesaplar,
/// testteki form yalnız gösterir.
class _FakeRepository implements PosRepositoryContract {
  List<PosSettlementItem> items = [
    _settlement('a', 'Pazartesi satışı'),
    _settlement('b', 'Salı satışı', expected: '2026-08-27'),
    // Başka hesaba geçer ve günü gelmedi.
    _settlement(
      'c',
      'Yemek kartı',
      accountId: 'bank-2',
      accountName: 'Garanti',
      net: '50.0000',
      expected: '2999-01-01',
    ),
  ];

  final List<_PreviewCall> previews = [];
  Map<String, Object?>? created;
  PosDeposit? deposit;
  String? requestedDepositId;
  String? revertedId;
  ApiException? previewError;
  ApiException? createError;
  ApiException? getError;
  ApiException? revertError;

  /// Verilirse yatış okuması bu tamamlanana kadar bekler.
  Completer<void>? getPending;

  @override
  Future<PosSettlementList> list({required bool inTransitOnly}) async =>
      PosSettlementList(
        items: items,
        moneyInTransit: '245.0000',
        inTransitCount: items.where((item) => item.isInTransit).length,
      );

  @override
  Future<PosOptions> loadOptions() async => const PosOptions(
    accounts: [DataChoice('bank-1', 'Ziraat')],
    incomeCategories: [DataChoice('income-category', 'Satış')],
    expenseCategories: [
      DataChoice('expense-category', 'POS komisyonu'),
      DataChoice('other-category', 'Banka gideri'),
    ],
  );

  @override
  Future<PosDepositPreview> previewDeposit({
    required List<String> settlementIds,
    String? depositedAmount,
  }) async {
    previews.add(_PreviewCall(settlementIds, depositedAmount));
    if (previewError case final error?) throw error;
    final selected = items.where((item) => settlementIds.contains(item.id));
    final expected = selected.fold<double>(
      0,
      (total, item) => total + double.parse(item.netAmount),
    );
    final deposited = depositedAmount == null
        ? expected
        : double.parse(depositedAmount);
    final exceeds = deposited > expected;
    return PosDepositPreview(
      accountName: selected.first.accountName,
      settlementCount: selected.length,
      expectedAmount: expected.toStringAsFixed(4),
      depositedAmount: deposited.toStringAsFixed(4),
      deductionAmount: (exceeds ? 0 : expected - deposited).toStringAsFixed(4),
      exceedsExpected: exceeds,
      deductionCategoryId: 'expense-category',
      deductionCategoryName: 'POS komisyonu',
      currency: 'TRY',
      earliestDepositDate: '2026-08-24',
    );
  }

  @override
  Future<PosDeposit> createDeposit({
    required String clientRequestId,
    required List<String> settlementIds,
    required String depositedAmount,
    required String depositDate,
    String? deductionCategoryId,
  }) async {
    if (createError case final error?) throw error;
    created = {
      'clientRequestId': clientRequestId,
      'settlementIds': settlementIds,
      'depositedAmount': depositedAmount,
      'depositDate': depositDate,
      'deductionCategoryId': deductionCategoryId,
    };
    items = [
      for (final item in items)
        if (!settlementIds.contains(item.id)) item,
    ];
    return PosDeposit(
      id: 'deposit-1',
      accountName: 'Ziraat',
      depositDate: depositDate,
      expectedAmount: depositedAmount,
      depositedAmount: depositedAmount,
      deductionAmount: deductionCategoryId == null ? '0.0000' : '5.0000',
      currency: 'TRY',
      isCancelled: false,
      settlements: const [],
    );
  }

  @override
  Future<PosDeposit> getDeposit({required String depositId}) async {
    requestedDepositId = depositId;
    await getPending?.future;
    if (getError case final error?) throw error;
    return deposit!;
  }

  @override
  Future<PosDeposit> revertDeposit({required String depositId}) async {
    if (revertError case final error?) throw error;
    revertedId = depositId;
    final current = deposit!;
    return deposit = _deposit(
      deduction: current.deductionAmount,
      cancelled: true,
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async {}

  @override
  Future<void> cancel({required String settlementId}) async {}

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

const _previewJson = <String, Object?>{
  'accountId': 'bank-1',
  'accountName': 'Ziraat',
  'settlementCount': 2,
  'expectedAmount': '195.0000',
  'depositedAmount': '190.0000',
  'deductionAmount': '5.0000',
  'exceedsExpected': false,
  'deductionCategoryId': 'expense-category',
  'deductionCategoryName': 'POS komisyonu',
  'currency': 'TRY',
  'earliestDepositDate': '2026-08-25',
};

const _settlementJson = <String, Object?>{
  'id': 'a',
  'accountId': 'bank-1',
  'accountName': 'Ziraat',
  'categoryId': 'income-category',
  'categoryName': 'Satış',
  'grossAmount': '100.0000',
  'commissionAmount': '2.5000',
  'netAmount': '97.5000',
  'currency': 'TRY',
  'scope': 'business',
  'settlementDate': '2026-08-24',
  'expectedTransferDate': '2026-08-26',
  'transferredOn': '2026-08-27',
  'description': 'Pazartesi satışı',
  'isInTransit': false,
  'isCancelled': false,
  'isLate': false,
  'posDepositId': 'deposit-1',
};

final _depositJson = <String, Object?>{
  'id': 'deposit-1',
  'accountId': 'bank-1',
  'accountName': 'Ziraat',
  'depositDate': '2026-08-27',
  'expectedAmount': '195.0000',
  'grossAmount': '200.0000',
  'commissionAmount': '5.0000',
  'depositedAmount': '190.0000',
  'deductionAmount': '5.0000',
  'deductionTransactionId': 'transaction-1',
  'deductionCategoryId': 'expense-category',
  'deductionCategoryName': 'POS komisyonu',
  'currency': 'TRY',
  'isCancelled': false,
  'cancelledAtUtc': null,
  'settlements': [
    _settlementJson,
    {..._settlementJson, 'id': 'b', 'description': 'Salı satışı'},
  ],
};
