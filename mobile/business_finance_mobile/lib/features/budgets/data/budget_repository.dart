import '../../../core/localization/default_category_labels.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import 'budget_models.dart';

abstract interface class BudgetRepositoryContract {
  Future<List<BudgetItem>> list(int year, int month);
  Future<BudgetItem> create(CreateBudgetInput input);
  Future<BudgetItem> update(String id, String limit);

  /// Bütçe gerçekten silinir; iptal edilmez. Sınır bir olay değil, bir
  /// niyettir: hiçbir bakiyeyi beslemez ve silinmesi hiçbir tutarı değiştirmez.
  Future<void> delete(String id);

  /// Bütçenin harcanan tutarını oluşturan satırlar.
  Future<List<BudgetSpendingLine>> listSpending(BudgetItem budget);
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
  Future<void> delete(String id) => _apiClient.delete('/api/v1/budgets/$id');

  @override
  Future<List<BudgetSpendingLine>> listSpending(BudgetItem budget) async {
    final periodStart = DateTime(budget.year, budget.month);
    final periodEnd = DateTime(budget.year, budget.month + 1, 0);
    final response = await _apiClient.get(
      Uri(
        path: '/api/v1/financial-activities',
        queryParameters: {
          'pageNumber': '1',
          'pageSize': '100',
          'dateFrom': _isoDate(periodStart),
          'dateTo': _isoDate(periodEnd),
          'categoryId': budget.categoryId,
          'effect': 'expense',
          // Bütçe iptal edilmiş hareketi saymaz; döküm de saymaz.
          'includeCancelled': 'false',
          // Bütçe **kategori + kapsam çiftini** sınırlar; kendi kapsamı
          // dışındaki harcama bu toplama hiç girmedi.
          'scope': budget.scope.apiValue,
        },
      ).toString(),
    );
    final items = response.requireObject()['items'] as List<dynamic>;
    return items
        .map(
          (item) => BudgetSpendingLine.fromJson(item as Map<String, dynamic>),
        )
        .toList(growable: false);
  }

  static String _isoDate(DateTime value) =>
      '${value.year.toString().padLeft(4, '0')}-'
      '${value.month.toString().padLeft(2, '0')}-'
      '${value.day.toString().padLeft(2, '0')}';

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
            defaultScope: TransactionScope.fromApiOrNull(json['defaultScope']),
          );
        })
        .toList(growable: false);
  }
}
