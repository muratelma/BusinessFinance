import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_models.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_repository.dart';

/// Türkçe karakterli gövde `http.Response`'un latin1 varsayılanından geçmez;
/// gerçek sunucu da UTF-8 gönderiyor.
http.Response _json(Object body, int statusCode) =>
    http.Response.bytes(utf8.encode(jsonEncode(body)), statusCode);

void main() {
  test('list uses year/month route and preserves all money strings', () async {
    final repository = BudgetRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          expect(request.url.path, '/api/v1/budgets');
          expect(request.url.queryParameters, {'year': '2026', 'month': '8'});
          return _json({
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
                'scope': 'business',
                'year': 2026,
                'month': 8,
              },
            ],
          }, 200);
        }),
      ),
    );

    final item = (await repository.list(2026, 8)).single;
    expect(item.limit, '100.0000');
    expect(item.spent, '125.2500');
    expect(item.exceeded, '25.2500');
    // Bütçe kapsam taşır; hangi tarafı sınırladığı buradan okunur.
    expect(item.scope, TransactionScope.business);
  });

  test('create sends the chosen scope and omits it when absent', () async {
    final bodies = <Map<String, dynamic>>[];
    final repository = BudgetRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          bodies.add(jsonDecode(request.body) as Map<String, dynamic>);
          return _json({
            'id': 'budget',
            'categoryId': 'category',
            'categoryName': 'Market',
            'limit': '100.0000',
            'spent': '0.0000',
            'remaining': '100.0000',
            'exceeded': '0.0000',
            'currency': 'TRY',
            'scope': 'personal',
            'year': 2026,
            'month': 8,
          }, 201);
        }),
      ),
    );

    await repository.create(
      const CreateBudgetInput(
        categoryId: 'category',
        limit: '100',
        year: 2026,
        month: 8,
        scope: TransactionScope.business,
      ),
    );
    await repository.create(
      const CreateBudgetInput(
        categoryId: 'category',
        limit: '100',
        year: 2026,
        month: 8,
      ),
    );

    expect(bodies.first['scope'], 'business');
    // Boş bırakıldığında alan hiç gitmez: "gönderilmedi" ile "kaldır" aynı
    // şey değildir ve sunucu zinciri yürütür.
    expect(bodies.last.containsKey('scope'), isFalse);
  });

  test('delete calls the budget route', () async {
    var deletedPath = '';
    final repository = BudgetRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient((request) async {
          expect(request.method, 'DELETE');
          deletedPath = request.url.path;
          return http.Response('', 204);
        }),
      ),
    );

    await repository.delete('budget-id');
    expect(deletedPath, '/api/v1/budgets/budget-id');
  });

  test('expense categories carry their default scope', () async {
    final repository = BudgetRepository(
      ApiClient(
        config: ApiConfig.fromEnvironment(value: 'https://api.test'),
        httpClient: MockClient(
          (request) async => _json({
            'items': [
              {
                'id': 'fuel',
                'name': 'Yakıt',
                'isActive': true,
                'defaultScope': 'business',
              },
              {'id': 'market', 'name': 'Market', 'isActive': true},
            ],
          }, 200),
        ),
      ),
    );

    final categories = await repository.listExpenseCategories();
    expect(categories.first.defaultScope, TransactionScope.business);
    // Boş olması eksik veri değil: o kategori kapsam belirlemiyor.
    expect(categories.last.defaultScope, isNull);
  });
}
