import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import 'category_models.dart';

abstract interface class CategoryRepository {
  Future<List<BudgetCategory>> list({String? type, bool? isActive});

  Future<BudgetCategory> create({
    required String name,
    required String type,
    TransactionScope? defaultScope,
  });

  /// Sunucudaki `PUT` **yetkilidir**: gönderilmeyen alan "dokunma" değil
  /// "kaldır" demektir. Bu yüzden kapsam çağırandan gelir.
  Future<BudgetCategory> update({
    required String id,
    required String name,
    required bool isActive,
    TransactionScope? defaultScope,
  });
}

class ApiCategoryRepository implements CategoryRepository {
  ApiCategoryRepository(this._client);

  final ApiClient _client;

  @override
  Future<List<BudgetCategory>> list({String? type, bool? isActive}) async {
    final query = <String, String>{};
    if (type != null) query['type'] = type;
    if (isActive != null) query['isActive'] = '$isActive';
    final path = Uri(
      path: '/api/v1/categories',
      queryParameters: query.isEmpty ? null : query,
    ).toString();
    final response = await _client.get(path);
    return CategoryList.fromJson(response.requireObject()).items;
  }

  @override
  Future<BudgetCategory> create({
    required String name,
    required String type,
    TransactionScope? defaultScope,
  }) async {
    final response = await _client.post(
      '/api/v1/categories',
      body: {
        'name': name.trim(),
        'type': type,
        'defaultScope': defaultScope?.apiValue,
      },
    );
    return BudgetCategory.fromJson(response.requireObject());
  }

  @override
  Future<BudgetCategory> update({
    required String id,
    required String name,
    required bool isActive,
    TransactionScope? defaultScope,
  }) async {
    final response = await _client.put(
      '/api/v1/categories/$id',
      body: {
        'name': name.trim(),
        'isActive': isActive,
        'defaultScope': defaultScope?.apiValue,
      },
    );
    return BudgetCategory.fromJson(response.requireObject());
  }
}
