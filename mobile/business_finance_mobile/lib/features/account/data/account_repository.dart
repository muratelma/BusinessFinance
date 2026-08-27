import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import 'account_models.dart';

abstract interface class AccountRepositoryContract {
  Future<UserAccount> read();

  Future<List<UserSessionSummary>> listSessions();

  /// Tek bir oturumu kapatır. Başka kullanıcının oturumu ve var olmayan oturum
  /// sunucuda aynı `404` sonucuna gider.
  Future<void> revokeSession(String sessionId);

  /// Parolayı değiştirir. Sunucu bütün oturumları kapatır ve bu cihaza taze
  /// bir token çifti verir; çağıran onu benimsemek zorundadır.
  Future<RotatedTokens> changePassword({
    required String currentPassword,
    required String newPassword,
  });

  /// Hesabı ve bütün verisini siler (ADR 0017). Geri dönüşü yoktur.
  Future<void> deleteAccount({required String password});
}

class AccountRepository implements AccountRepositoryContract {
  const AccountRepository(this._apiClient);

  final ApiClient _apiClient;

  @override
  Future<UserAccount> read() async {
    final response = await _apiClient.get('/api/v1/account');
    return UserAccount.fromJson(response.requireObject());
  }

  @override
  Future<List<UserSessionSummary>> listSessions() async {
    final response = await _apiClient.get('/api/v1/account/sessions');
    final json = response.requireObject();
    return JsonReaders.list(json, 'items')
        .map(
          (item) =>
              UserSessionSummary.fromJson(JsonReaders.object(item, 'items')),
        )
        .toList(growable: false);
  }

  @override
  Future<void> revokeSession(String sessionId) async {
    await _apiClient.delete('/api/v1/account/sessions/$sessionId');
  }

  @override
  Future<RotatedTokens> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    final response = await _apiClient.post(
      '/api/v1/account/password',
      body: {'currentPassword': currentPassword, 'newPassword': newPassword},
    );
    return RotatedTokens.fromJson(response.requireObject());
  }

  @override
  Future<void> deleteAccount({required String password}) async {
    // Onay istemcinin kendi kendine doldurduğu bir alan değil: kullanıcı
    // silme penceresini onayladığı ve parolasını yazdığı için buraya kadar
    // geldi. Sunucu ikisini de ayrıca arar.
    await _apiClient.delete(
      '/api/v1/account',
      body: {'password': password, 'confirmed': true},
    );
  }
}
