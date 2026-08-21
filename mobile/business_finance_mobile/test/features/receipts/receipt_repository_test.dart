import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_models.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_photo.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_repository.dart';

void main() {
  test('uploads the shrunk copy as JPEG under the file field', () async {
    late Uri url;
    late String body;
    final repository = _repository((request) async {
      url = request.url;
      body = request.body;
      return http.Response(jsonEncode(_responseJson()), 200, headers: _json);
    });

    await repository.analyze(_photo(), ReceiptCaptureIntent.expense);

    expect(url.path, '/api/v1/receipts/analyze');
    expect(body, contains('name="file"'));
    expect(body, contains('filename="fis.jpg"'));
    expect(body.toLowerCase(), contains('content-type: image/jpeg'));
  });

  // Dekontta masraf taşınan tutardan ayrı bir alandır; ikisi birleşseydi hiç
  // harcanmamış bir tutar taşınmış görünürdü.
  test('reads the transfer fee as a field of its own', () async {
    final repository = _repository(
      (_) async => http.Response(
        jsonEncode(
          _responseJson()
            ..['feeAmount'] = '4.5000'
            ..['feeAmountState'] = 'read',
        ),
        200,
        headers: _json,
      ),
    );

    final draft = await repository.analyze(
      _photo(),
      ReceiptCaptureIntent.transfer,
    );

    expect(draft.totalAmount, '847.5000');
    expect(draft.feeAmount, '4.5000');
    expect(draft.feeAmountState, ReceiptFieldState.read);
  });

  // Yön sunucuya bir form alanı olarak gider; sunucu onu belgeden çıkarmaz.
  test('sends the declared direction as a form field', () async {
    var body = '';
    final repository = _repository((request) async {
      body = request.body;
      return http.Response(jsonEncode(_responseJson()), 200, headers: _json);
    });

    await repository.analyze(_photo(), ReceiptCaptureIntent.income);

    expect(body, contains('name="intent"'));
    expect(body, contains('income'));
  });

  test('reads every field state the contract defines', () async {
    final repository = _repository(
      (_) async =>
          http.Response(jsonEncode(_responseJson()), 200, headers: _json),
    );

    final draft = await repository.analyze(
      _photo(),
      ReceiptCaptureIntent.expense,
    );

    expect(draft.counterpartyName, 'Sentetik Market');
    expect(draft.counterpartyState, ReceiptFieldState.read);
    expect(draft.purchasedAt, '2026-08-18');
    expect(draft.totalAmount, '847.5000');
    expect(draft.totalAmountState, ReceiptFieldState.suspect);
    expect(draft.paymentHint, ReceiptPaymentHint.card);
    expect(draft.categoryId, '11111111-1111-1111-1111-111111111111');
    expect(draft.warnings.single.code, 'receipt.totals_do_not_add_up');
    expect(draft.needsAttention, isTrue);
  });

  // Sunucu kart türünü ayrı değerlerle gönderiyor; tanınmayan bir değer
  // uydurulmuş bir karta değil, bilinmeyene düşer.
  test('parses every payment hint the server can send', () {
    for (final (wire, expected) in const [
      ('cash', ReceiptPaymentHint.cash),
      ('credit_card', ReceiptPaymentHint.creditCard),
      ('debit_card', ReceiptPaymentHint.debitCard),
      ('card', ReceiptPaymentHint.card),
      ('unknown', ReceiptPaymentHint.unknown),
      ('kredi_karti', ReceiptPaymentHint.unknown),
    ]) {
      expect(
        ReceiptPaymentHint.parse({'paymentHint': wire}, 'paymentHint'),
        expected,
        reason: wire,
      );
    }
  });

  test('keeps an unreadable field null instead of guessing', () async {
    final repository = _repository(
      (_) async => http.Response(
        jsonEncode(
          _responseJson()
            ..['totalAmount'] = null
            ..['totalAmountState'] = 'missing',
        ),
        200,
        headers: _json,
      ),
    );

    final draft = await repository.analyze(
      _photo(),
      ReceiptCaptureIntent.expense,
    );

    expect(draft.totalAmount, isNull);
    expect(draft.totalAmountState.isMissing, isTrue);
  });

  test('rejects an unknown field state rather than trusting it', () async {
    final repository = _repository(
      (_) async => http.Response(
        jsonEncode(_responseJson()..['counterpartyState'] = 'probably'),
        200,
        headers: _json,
      ),
    );

    // Tanınmayan durumu `read` saymak, doğrulanmamış değeri doğrulanmış
    // göstermek olurdu.
    expect(
      () => repository.analyze(_photo(), ReceiptCaptureIntent.expense),
      throwsA(isA<FormatException>()),
    );
  });

  test('rejects an amount that is not four-decimal money', () async {
    final repository = _repository(
      (_) async => http.Response(
        jsonEncode(_responseJson()..['totalAmount'] = '847.5'),
        200,
        headers: _json,
      ),
    );

    expect(
      () => repository.analyze(_photo(), ReceiptCaptureIntent.expense),
      throwsA(isA<FormatException>()),
    );
  });

  test('carries the provider quota error through as its own code', () async {
    final repository = _repository(
      (_) async => http.Response(
        jsonEncode({
          'title': 'Receipt provider rate limit reached.',
          'detail': 'Fiş okuma servisinin kotası doldu.',
          'code': 'receipt.provider_rate_limited',
        }),
        429,
        headers: _json,
      ),
    );

    await expectLater(
      repository.analyze(_photo(), ReceiptCaptureIntent.expense),
      throwsA(
        isA<ApiException>().having(
          (error) => error.code,
          'code',
          'receipt.provider_rate_limited',
        ),
      ),
    );
  });
}

const Map<String, String> _json = {'content-type': 'application/json'};

ReceiptRepository _repository(MockClientHandler handler) => ReceiptRepository(
  ApiClient(
    config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
    httpClient: MockClient(handler),
  ),
);

ReceiptPhoto _photo() => ReceiptPhoto(
  originalBytes: Uint8List.fromList(const [1, 2, 3, 4]),
  originalFileName: 'IMG_1.heic',
  originalMediaType: 'image/heic',
  uploadBytes: Uint8List.fromList(const [5, 6, 7, 8]),
  uploadFileName: 'fis.jpg',
  uploadWidth: 1800,
  uploadHeight: 2400,
);

Map<String, dynamic> _responseJson() => <String, dynamic>{
  'counterpartyName': 'Sentetik Market',
  'counterpartyState': 'read',
  'purchasedAt': '2026-08-18',
  'purchasedAtState': 'read',
  'dueDate': null,
  'dueDateState': 'missing',
  'feeAmount': null,
  'feeAmountState': 'missing',
  'installmentCount': null,
  'totalAmount': '847.5000',
  'totalAmountState': 'suspect',
  'currencyCode': 'TRY',
  'paymentHint': 'card',
  'categoryId': '11111111-1111-1111-1111-111111111111',
  'categoryName': 'Market Alışverişi',
  'categoryState': 'read',
  'warnings': [
    {
      'code': 'receipt.totals_do_not_add_up',
      'message': 'Ara toplam ve KDV, genel toplamı tutmuyor.',
    },
  ],
};
