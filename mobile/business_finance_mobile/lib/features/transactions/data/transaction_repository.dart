import '../../../core/localization/default_category_labels.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import 'transaction_models.dart';

abstract interface class TransactionRepositoryContract {
  Future<TransactionPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    TransactionFilter filter = const TransactionFilter(),
  });
  Future<TransactionItem> create(CreateTransactionInput input);
  Future<TransactionItem> cancel(String id);
  Future<List<TransactionChoice>> listAccounts();
  Future<List<TransactionChoice>> listCategories(TransactionKind kind);
}

class TransactionRepository implements TransactionRepositoryContract {
  const TransactionRepository(this._apiClient);
  final ApiClient _apiClient;

  @override
  Future<TransactionPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    TransactionFilter filter = const TransactionFilter(),
  }) async {
    final query = <String, String>{
      'pageNumber': '$pageNumber',
      'pageSize': '$pageSize',
      'sort': 'date-desc',
      if (filter.dateFrom != null) 'dateFrom': filter.dateFrom!,
      if (filter.dateTo != null) 'dateTo': filter.dateTo!,
      if (filter.accountId != null) 'accountId': filter.accountId!,
      if (filter.categoryId != null) 'categoryId': filter.categoryId!,
      if (filter.kind != null) 'type': filter.kind!.apiValue,
    };
    final response = await _apiClient.get(
      Uri(path: '/api/v1/transactions', queryParameters: query).toString(),
    );
    return TransactionPage.fromJson(response.requireObject());
  }

  @override
  Future<TransactionItem> create(CreateTransactionInput input) async {
    final response = await _apiClient.post(
      '/api/v1/transactions',
      body: input.toJson(),
    );
    return TransactionItem.fromJson(response.requireObject());
  }

  @override
  Future<TransactionItem> cancel(String id) async {
    final response = await _apiClient.delete('/api/v1/transactions/$id');
    return TransactionItem.fromJson(response.requireObject());
  }

  @override
  Future<List<TransactionChoice>> listAccounts() async {
    final response = await _apiClient.get(
      '/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true',
    );
    final items = response.requireObject()['items'] as List<dynamic>;
    return items
        .map(
          (item) => TransactionChoice(
            id: (item as Map<String, dynamic>)['id'] as String,
            name: item['name'] as String,
            isActive: item['isActive'] as bool,
            defaultScope: TransactionScope.fromApiOrNull(item['defaultScope']),
          ),
        )
        .toList(growable: false);
  }

  @override
  Future<List<TransactionChoice>> listCategories(TransactionKind kind) async {
    final response = await _apiClient.get(
      '/api/v1/categories?type=${kind.apiValue}&isActive=true',
    );
    final items = response.requireObject()['items'] as List<dynamic>;
    return items
        .map((item) {
          final json = item as Map<String, dynamic>;
          return TransactionChoice(
            id: json['id'] as String,
            name: DefaultCategoryLabels.localized(json['name'] as String),
            isActive: json['isActive'] as bool,
            kind: TransactionKind.fromApi(json['type'] as String),
            defaultScope: TransactionScope.fromApiOrNull(json['defaultScope']),
            defaultIsTaxDeductible: json['defaultIsTaxDeductible'] as bool?,
          );
        })
        .toList(growable: false);
  }
}
