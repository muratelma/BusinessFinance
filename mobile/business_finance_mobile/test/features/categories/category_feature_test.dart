import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/categories/data/category_models.dart';
import 'package:business_finance_mobile/features/categories/data/category_repository.dart';
import 'package:business_finance_mobile/features/categories/presentation/categories_view_model.dart';
import 'package:business_finance_mobile/features/categories/presentation/category_form_page.dart';

void main() {
  test('category repository sends exact create and update contracts', () async {
    var requestCount = 0;
    final repository = ApiCategoryRepository(
      _client((request) async {
        requestCount++;
        if (requestCount == 1) {
          expect(request.url.path, '/api/v1/categories');
          expect(jsonDecode(request.body), {
            'name': 'Market',
            'type': 'expense',
          });
        } else {
          expect(request.url.path, '/api/v1/categories/category-id');
          expect(jsonDecode(request.body), {
            'name': 'Market',
            'isActive': false,
          });
        }
        return http.Response(jsonEncode(_categoryJson), 200);
      }),
    );

    await repository.create(name: ' Market ', type: 'expense');
    await repository.update(id: 'category-id', name: 'Market', isActive: false);
    expect(requestCount, 2);
  });

  test('category view model exposes empty and unauthorized states', () async {
    final empty = CategoriesViewModel(_FakeCategoryRepository());
    await empty.load();
    expect(empty.status, CategoriesViewStatus.empty);

    final unauthorized = CategoriesViewModel(
      _FakeCategoryRepository(
        error: const ApiException(
          statusCode: 401,
          code: 'authentication.required',
          message: 'Unauthorized',
        ),
      ),
    );
    await unauthorized.load();
    expect(unauthorized.status, CategoriesViewStatus.unauthorized);
  });

  testWidgets('category edit shows active toggle and prevents double submit', (
    tester,
  ) async {
    final completer = Completer<bool>();
    var count = 0;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: CategoryFormPage(
          category: const BudgetCategory(
            id: 'category-id',
            name: 'Market',
            type: 'expense',
            isActive: true,
          ),
          onSave:
              ({category, required name, required type, required isActive}) {
                count++;
                return completer.future;
              },
        ),
      ),
    );
    expect(find.text('Kategori aktif'), findsOneWidget);
    await tester.tap(find.text('Kaydet'));
    await tester.pump();
    await tester.tap(find.byType(FilledButton));
    await tester.pump();
    expect(count, 1);
    expect(find.text('Kaydediliyor'), findsOneWidget);
    completer.complete(false);
    await tester.pumpAndSettle();
  });
}

const _categoryJson = {
  'id': 'category-id',
  'name': 'Market',
  'type': 'expense',
  'isActive': true,
};

ApiClient _client(Future<http.Response> Function(http.Request) handler) =>
    ApiClient(
      config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
      httpClient: MockClient(handler),
    );

class _FakeCategoryRepository implements CategoryRepository {
  _FakeCategoryRepository({this.error});

  final ApiException? error;

  @override
  Future<List<BudgetCategory>> list({String? type, bool? isActive}) async {
    if (error case final value?) throw value;
    return const [];
  }

  @override
  Future<BudgetCategory> create({required String name, required String type}) =>
      throw UnimplementedError();

  @override
  Future<BudgetCategory> update({
    required String id,
    required String name,
    required bool isActive,
  }) => throw UnimplementedError();
}
