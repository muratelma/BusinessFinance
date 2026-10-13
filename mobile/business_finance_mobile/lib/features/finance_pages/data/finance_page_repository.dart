import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import '../../accounts/data/account_models.dart';
import '../../activities/data/activity_models.dart';
import 'finance_page_models.dart';

abstract interface class FinancePageRepositoryContract {
  Future<CategorySummaryData> summary(
    DateTime month, {
    bool income,
    TransactionScope? scope,
    String? source,
  });
  Future<CategoryPageData> category(
    String? id,
    DateTime month, {
    bool income,
    TransactionScope? scope,
    String? source,
    String? person,
    int offset,
  });
  Future<FinanceYearMonths> categoryMonths(
    int year, {
    String? id,
    bool summary,
    bool income,
    TransactionScope? scope,
    String? source,
    String? person,
  });
  Future<AccountDetailData> account(
    String id,
    DateTime month, {
    String? direction,
    String? selection,
    int offset,
  });
  Future<FinanceYearMonths> accountMonths(String id, int year);
  Future<TransitPageData> transit(String id, {int offset});
  Future<AccountsOverviewData> accounts();
  Future<Account> accountSettings(String id);
  Future<List<ActivityPartData>> parts(FinancialActivity activity);
  Future<TransferPreviewData> previewTransfer(TransferInput input);
  Future<void> createTransfer(TransferInput input);
}

class FinancePageRepository implements FinancePageRepositoryContract {
  const FinancePageRepository(this._client);
  final ApiClient _client;
  String _path(String path, Map<String, String> query) =>
      Uri(path: path, queryParameters: query).toString();
  Map<String, String> _categoryQuery(
    DateTime month,
    bool income,
    TransactionScope? scope,
    String? source,
    String? person,
  ) => {
    'year': '${month.year}',
    'month': '${month.month}',
    'direction': income ? 'income' : 'expense',
    'scope': ?scope?.apiValue,
    'source': ?source,
    'person': ?person,
  };

  @override
  Future<CategorySummaryData> summary(
    DateTime month, {
    bool income = false,
    TransactionScope? scope,
    String? source,
  }) async => CategorySummaryData.fromJson(
    (await _client.get(
      _path(
        '/api/v1/category-summary',
        _categoryQuery(month, income, scope, source, null),
      ),
    )).requireObject(),
  );
  @override
  Future<CategoryPageData> category(
    String? id,
    DateTime month, {
    bool income = false,
    TransactionScope? scope,
    String? source,
    String? person,
    int offset = 0,
  }) async => CategoryPageData.fromJson(
    (await _client.get(
      _path('/api/v1/categories/${id ?? 'uncategorized'}/page', {
        ..._categoryQuery(month, income, scope, source, person),
        'offset': '$offset',
      }),
    )).requireObject(),
  );
  @override
  Future<FinanceYearMonths> categoryMonths(
    int year, {
    String? id,
    bool summary = false,
    bool income = false,
    TransactionScope? scope,
    String? source,
    String? person,
  }) async => FinanceYearMonths.fromJson(
    (await _client.get(
      _path(
        summary
            ? '/api/v1/category-summary/months'
            : '/api/v1/categories/${id ?? 'uncategorized'}/months',
        _categoryQuery(DateTime(year), income, scope, source, person),
      ),
    )).requireObject(),
  );
  @override
  Future<AccountDetailData> account(
    String id,
    DateTime month, {
    String? direction,
    String? selection,
    int offset = 0,
  }) async => AccountDetailData.fromJson(
    (await _client.get(
      _path('/api/v1/accounts/$id/page', {
        'year': '${month.year}',
        'month': '${month.month}',
        'direction': ?direction,
        'selection': ?selection,
        'offset': '$offset',
      }),
    )).requireObject(),
  );
  @override
  Future<FinanceYearMonths> accountMonths(String id, int year) async =>
      FinanceYearMonths.fromJson(
        (await _client.get(
          '/api/v1/accounts/$id/months?year=$year',
        )).requireObject(),
      );
  @override
  Future<TransitPageData> transit(String id, {int offset = 0}) async =>
      TransitPageData.fromJson(
        (await _client.get(
          '/api/v1/accounts/$id/in-transit?offset=$offset',
        )).requireObject(),
      );
  @override
  Future<AccountsOverviewData> accounts() async =>
      AccountsOverviewData.fromJson(
        (await _client.get('/api/v1/accounts/overview')).requireObject(),
      );
  @override
  Future<Account> accountSettings(String id) async => Account.fromJson(
    (await _client.get('/api/v1/accounts/$id')).requireObject(),
  );
  @override
  Future<List<ActivityPartData>> parts(FinancialActivity activity) async {
    final data = (await _client.get(
      '/api/v1/financial-activities/${activity.kind.apiValue}/${activity.activityId}/parts',
    )).requireObject();
    return JsonReaders.list(data, 'items')
        .map((v) => ActivityPartData.fromJson(JsonReaders.object(v, 'part')))
        .toList(growable: false);
  }

  @override
  Future<TransferPreviewData> previewTransfer(TransferInput input) async =>
      TransferPreviewData.fromJson(
        (await _client.post(
          '/api/v1/transfers/preview',
          body: input.toJson(),
        )).requireObject(),
      );
  @override
  Future<void> createTransfer(TransferInput input) async {
    await _client.post('/api/v1/transfers', body: input.toJson());
  }
}
