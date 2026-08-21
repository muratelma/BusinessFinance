import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';

void main() {
  test('create sends and reads money as an exact JSON string', () async {
    final repository = TransactionRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          expect(request.url.path, '/api/v1/transactions');
          final body = jsonDecode(request.body) as Map<String, dynamic>;
          expect(body['amount'], '123.4567');
          expect(body['amount'], isA<String>());
          return http.Response(
            jsonEncode({
              'id': 'transaction',
              'accountId': 'account',
              'categoryId': 'category',
              'amount': '123.4567',
              'currency': 'TRY',
              'type': 'expense',
              'transactionDate': '2026-08-09',
              'description': null,
              'isCancelled': false,
              'cancelledAtUtc': null,
            }),
            201,
          );
        }),
      ),
    );

    final item = await repository.create(
      const CreateTransactionInput(
        accountId: 'account',
        categoryId: 'category',
        amount: '123.4567',
        kind: TransactionKind.expense,
        transactionDate: '2026-08-09',
      ),
    );
    expect(item.amount, '123.4567');
  });
}
