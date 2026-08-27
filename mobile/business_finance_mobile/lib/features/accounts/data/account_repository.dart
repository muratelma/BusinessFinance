import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import 'account_models.dart';

abstract interface class AccountRepository {
  Future<AccountPage> list({bool? isActive, String? type});

  Future<Account> create({
    required String name,
    required String type,
    required String openingBalance,
    TransactionScope? defaultScope,
  });

  /// Sunucudaki `PUT` **yetkilidir**: gönderilmeyen `defaultScope` "dokunma"
  /// değil "kaldır" demektir. Bu yüzden çağıranın mevcut değeri taşıması
  /// gerekiyor; adı değiştirilen bir hesabın kapsam etiketi aksi hâlde sessizce
  /// silinirdi.
  Future<Account> update({
    required String id,
    required String name,
    required bool isActive,
    TransactionScope? defaultScope,
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
    TransactionScope? defaultScope,
  }) async {
    final response = await _client.post(
      '/api/v1/accounts',
      body: {
        'name': name.trim(),
        'type': type,
        'currency': 'TRY',
        'openingBalance': openingBalance.trim(),
        'defaultScope': defaultScope?.apiValue,
      },
    );
    return Account.fromJson(response.requireObject());
  }

  @override
  Future<Account> update({
    required String id,
    required String name,
    required bool isActive,
    TransactionScope? defaultScope,
  }) async {
    final response = await _client.put(
      '/api/v1/accounts/$id',
      body: {
        'name': name.trim(),
        'isActive': isActive,
        'defaultScope': defaultScope?.apiValue,
      },
    );
    return Account.fromJson(response.requireObject());
  }

  @override
  Future<void> delete(String id) async {
    await _client.delete('/api/v1/accounts/$id');
  }
}
