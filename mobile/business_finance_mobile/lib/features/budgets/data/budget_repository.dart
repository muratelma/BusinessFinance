import '../../../core/localization/default_category_labels.dart';
import '../../../core/network/api_client.dart';
import 'budget_models.dart';

abstract interface class BudgetRepositoryContract {
  Future<List<BudgetItem>> list(int year, int month);
  Future<BudgetItem> create(CreateBudgetInput input);
  Future<BudgetItem> update(String id, String limit);
  Future<List<BudgetCategory>> listExpenseCategories();
}

class BudgetRepository implements BudgetRepositoryContract {
  const BudgetRepository(this._apiClient);
  final ApiClient _apiClient;

  @override
  Future<List<BudgetItem>> list(int year, int month) async {
    final response = await _apiClient.get(
      '/api/v1/budgets?year=$year&month=$month',
    );
    final items = response.requireObject()['items'] as List<dynamic>;
    return items
        .map((item) => BudgetItem.fromJson(item as Map<String, dynamic>))
        .toList(growable: false);
  }

  @override
  Future<BudgetItem> create(CreateBudgetInput input) async {
    final response = await _apiClient.post(
      '/api/v1/budgets',
      body: input.toJson(),
    );
    return BudgetItem.fromJson(response.requireObject());
  }

  @override
  Future<BudgetItem> update(String id, String limit) async {
    final response = await _apiClient.put(
      '/api/v1/budgets/$id',
      body: {'limit': limit, 'currency': 'TRY'},
    );
    return BudgetItem.fromJson(response.requireObject());
  }

  @override
  Future<List<BudgetCategory>> listExpenseCategories() async {
    final response = await _apiClient.get(
      '/api/v1/categories?type=expense&isActive=true',
    );
    final items = response.requireObject()['items'] as List<dynamic>;
    return items
        .map((item) {
          final json = item as Map<String, dynamic>;
          return BudgetCategory(
            id: json['id'] as String,
            name: DefaultCategoryLabels.localized(json['name'] as String),
            isActive: json['isActive'] as bool,
          );
        })
        .toList(growable: false);
  }
}
