import '../../../core/network/api_client.dart';
import 'account_models.dart';

abstract interface class AccountRepository {
  Future<AccountPage> list({bool? isActive, String? type});

  Future<Account> create({
    required String name,
    required String type,
    required String openingBalance,
  });

  Future<Account> update({
    required String id,
    required String name,
    required bool isActive,
  });

  Future<void> delete(String id);
}

class ApiAccountRepository implements AccountRepository {
  ApiAccountRepository(this._client);

  final ApiClient _client;

  @override
  Future<AccountPage> list({bool? isActive, String? type}) async {
    final query = <String, String>{'pageNumber': '1', 'pageSize': '100'};
    if (isActive != null) query['isActive'] = '$isActive';
    if (type != null) query['type'] = type;
    final path = Uri(
      path: '/api/v1/accounts',
      queryParameters: query,
    ).toString();
    final response = await _client.get(path);
    return AccountPage.fromJson(response.requireObject());
  }

  @override
  Future<Account> create({
    required String name,
    required String type,
    required String openingBalance,
  }) async {
    final response = await _client.post(
      '/api/v1/accounts',
      body: {
        'name': name.trim(),
        'type': type,
        'currency': 'TRY',
        'openingBalance': openingBalance.trim(),
      },
    );
    return Account.fromJson(response.requireObject());
  }

  @override
  Future<Account> update({
    required String id,
    required String name,
    required bool isActive,
  }) async {
    final response = await _client.put(
      '/api/v1/accounts/$id',
      body: {'name': name.trim(), 'isActive': isActive},
    );
    return Account.fromJson(response.requireObject());
  }

  @override
  Future<void> delete(String id) async {
    await _client.delete('/api/v1/accounts/$id');
  }
}
