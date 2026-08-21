import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_repository.dart';

void main() {
  test('list uses year/month route and preserves all money strings', () async {
    final repository = BudgetRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          expect(request.url.path, '/api/v1/budgets');
          expect(request.url.queryParameters, {'year': '2026', 'month': '8'});
          return http.Response(
            jsonEncode({
              'items': [
                {
                  'id': 'budget',
                  'categoryId': 'category',
                  'categoryName': 'Market',
                  'limit': '100.0000',
                  'spent': '125.2500',
                  'remaining': '0.0000',
                  'exceeded': '25.2500',
                  'currency': 'TRY',
                  'year': 2026,
                  'month': 8,
                },
              ],
            }),
            200,
          );
        }),
      ),
    );

    final item = (await repository.list(2026, 8)).single;
    expect(item.limit, '100.0000');
    expect(item.spent, '125.2500');
    expect(item.exceeded, '25.2500');
  });
}
