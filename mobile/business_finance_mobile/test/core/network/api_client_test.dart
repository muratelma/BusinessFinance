import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';

void main() {
  test('sends JSON with the bearer access token and decodes success', () async {
    final httpClient = MockClient((request) async {
      expect(
        request.url.toString(),
        'https://api.example.test/api/v1/accounts',
      );
      expect(request.headers['Authorization'], 'Bearer access-token');
      expect(request.headers['Content-Type'], contains('application/json'));
      expect(jsonDecode(request.body), {'name': 'Ücretli Çalışma'});
      return http.Response('{"id":"account-id"}', 201);
    });
    final client = _createClient(
      httpClient,
      accessTokenProvider: () async => 'access-token',
    );

    final response = await client.post(
      '/api/v1/accounts',
      body: {'name': 'Ücretli Çalışma'},
    );

    expect(response.statusCode, 201);
    expect(response.requireObject()['id'], 'account-id');
  });

  test('does not send an empty bearer token', () async {
    final httpClient = MockClient((request) async {
      expect(request.headers.containsKey('Authorization'), isFalse);
      return http.Response('{}', 200);
    });
    final client = _createClient(
      httpClient,
      accessTokenProvider: () async => '   ',
    );

    await client.get('/api/v1/accounts');
  });

  test('maps ProblemDetails and invokes unauthorized callback', () async {
    var unauthorizedCount = 0;
    final client = _createClient(
      MockClient(
        (_) async => http.Response(
          jsonEncode({
            'title': 'Authentication is required.',
            'status': 401,
            'detail': 'A valid bearer access token is required.',
            'code': 'authentication.required',
            'traceId': 'trace-123',
          }),
          401,
        ),
      ),
      onUnauthorized: () async {
        unauthorizedCount++;
        return null;
      },
    );

    await expectLater(
      client.get('/api/v1/accounts'),
      throwsA(
        isA<ApiException>()
            .having((error) => error.code, 'code', 'authentication.required')
            .having((error) => error.traceId, 'traceId', 'trace-123')
            .having((error) => error.isUnauthorized, 'isUnauthorized', isTrue),
      ),
    );
    expect(unauthorizedCount, 1);
  });

  test('maps a non-JSON error without exposing response content', () async {
    final client = _createClient(
      MockClient((_) async => http.Response('sensitive proxy response', 502)),
    );

    await expectLater(
      client.get('/api/v1/accounts'),
      throwsA(
        isA<ApiException>()
            .having((error) => error.code, 'code', 'server.request_failed')
            .having(
              (error) => error.message,
              'message',
              'İstek tamamlanamadı.',
            ),
      ),
    );
  });

  test('localizes known English authentication messages by stable code', () {
    final error = ApiException.fromProblemDetails(401, {
      'code': 'authentication.invalid_credentials',
      'detail': 'Email or password is invalid.',
    });

    expect(error.message, 'E-posta veya parola geçersiz.');
  });

  test('decodes UTF-8 ProblemDetails without a charset declaration', () async {
    final client = _createClient(
      MockClient(
        (_) async => http.Response.bytes(
          utf8.encode(
            jsonEncode({
              'detail': 'Katkı geçmişi bulunan tasarruf hedefi silinemez.',
              'code': 'goal.has_contributions',
            }),
          ),
          409,
          headers: {'content-type': 'application/problem+json'},
        ),
      ),
    );

    await expectLater(
      client.delete('/api/v1/goals/goal-id'),
      throwsA(
        isA<ApiException>().having(
          (error) => error.message,
          'message',
          'Katkı geçmişi bulunan tasarruf hedefi silinemez.',
        ),
      ),
    );
  });

  test('maps client failures to a safe network error', () async {
    final client = _createClient(
      MockClient((_) async => throw http.ClientException('private detail')),
    );

    await expectLater(
      client.get('/api/v1/accounts'),
      throwsA(
        isA<ApiException>().having(
          (error) => error.code,
          'code',
          'network.unavailable',
        ),
      ),
    );
  });

  test('maps slow requests to a timeout error', () async {
    final client = _createClient(
      MockClient((_) async {
        await Future<void>.delayed(const Duration(milliseconds: 50));
        return http.Response('{}', 200);
      }),
      timeout: const Duration(milliseconds: 1),
    );

    await expectLater(
      client.get('/api/v1/accounts'),
      throwsA(
        isA<ApiException>().having(
          (error) => error.code,
          'code',
          'network.timeout',
        ),
      ),
    );
  });

  test('refreshes and retries an unauthorized request only once', () async {
    var requestCount = 0;
    var refreshCount = 0;
    final client = _createClient(
      MockClient((request) async {
        requestCount++;
        if (requestCount == 1) {
          expect(request.headers['Authorization'], 'Bearer expired-token');
          return http.Response('{"code":"authentication.required"}', 401);
        }
        expect(request.headers['Authorization'], 'Bearer rotated-token');
        return http.Response('{"ok":true}', 200);
      }),
      accessTokenProvider: () async => 'expired-token',
      onUnauthorized: () async {
        refreshCount++;
        return 'rotated-token';
      },
    );

    final response = await client.get('/api/v1/accounts');

    expect(response.requireObject()['ok'], isTrue);
    expect(requestCount, 2);
    expect(refreshCount, 1);
  });

  test(
    'multipart upload preserves auth, fields, filename and media type',
    () async {
      final client = _createClient(
        MockClient((request) async {
          expect(request.headers['Authorization'], 'Bearer upload-token');
          expect(
            request.headers['Content-Type'],
            startsWith('multipart/form-data; boundary='),
          );
          final body = latin1.decode(request.bodyBytes);
          expect(body, contains('name="dateColumn"'));
          expect(body, contains('transactionDate'));
          expect(body, contains('filename="statement.csv"'));
          expect(body.toLowerCase(), contains('content-type: text/csv'));
          expect(body, contains('date,amount'));
          return http.Response('{"id":"batch-1"}', 201);
        }),
        accessTokenProvider: () async => 'upload-token',
      );

      final response = await client.postMultipart(
        '/api/v1/imports/csv/stage',
        upload: const ApiUpload(
          bytes: [100, 97, 116, 101, 44, 97, 109, 111, 117, 110, 116],
          fileName: 'statement.csv',
          mediaType: 'text/csv',
        ),
        fields: const {'dateColumn': 'transactionDate'},
      );

      expect(response.requireObject()['id'], 'batch-1');
    },
  );

  test('a multipart upload may outlast the default client budget', () async {
    // Fiş okuma sunucuda 60 saniyeye kadar sürüyor; varsayılan bütçe onu
    // okumuş bir cevabı beklerken keserdi. Bu kapı, isteğe özel sürenin
    // gerçekten uygulandığını gösteriyor.
    final client = _createClient(
      MockClient((_) async {
        await Future<void>.delayed(const Duration(milliseconds: 60));
        return http.Response('{"counterpartyState":"read"}', 200);
      }),
      timeout: const Duration(milliseconds: 20),
    );

    final response = await client.postMultipart(
      '/api/v1/receipts/analyze',
      upload: const ApiUpload(
        bytes: [1, 2, 3],
        fileName: 'fis.jpg',
        mediaType: 'image/jpeg',
      ),
      timeout: const Duration(seconds: 5),
    );

    expect(response.statusCode, 200);
  });

  test(
    'a multipart upload without an override keeps the default budget',
    () async {
      final client = _createClient(
        MockClient((_) async {
          await Future<void>.delayed(const Duration(milliseconds: 60));
          return http.Response('{}', 200);
        }),
        timeout: const Duration(milliseconds: 20),
      );

      await expectLater(
        client.postMultipart(
          '/api/v1/imports/csv/stage',
          upload: const ApiUpload(bytes: [1], fileName: 'a.csv'),
        ),
        throwsA(
          isA<ApiException>().having(
            (error) => error.code,
            'code',
            'network.timeout',
          ),
        ),
      );
    },
  );

  test('binary download returns exact bytes and response metadata', () async {
    final client = _createClient(
      MockClient(
        (_) async => http.Response.bytes(
          [0, 1, 2, 255],
          200,
          headers: {
            'content-type': 'application/pdf',
            'content-disposition': 'attachment; filename=receipt.pdf',
          },
        ),
      ),
    );

    final response = await client.getBytes('/api/v1/attachments/id/content');

    expect(response.bytes, [0, 1, 2, 255]);
    expect(response.contentType, 'application/pdf');
    expect(response.contentDisposition, contains('receipt.pdf'));
  });
}

ApiClient _createClient(
  http.Client httpClient, {
  AccessTokenProvider? accessTokenProvider,
  UnauthorizedCallback? onUnauthorized,
  Duration timeout = const Duration(seconds: 15),
}) {
  return ApiClient(
    config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
    httpClient: httpClient,
    accessTokenProvider: accessTokenProvider,
    onUnauthorized: onUnauthorized,
    timeout: timeout,
  );
}
