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
    String? currentPassword,
    required String newPassword,
    String? resetCode,
  });

  /// Parolası olmayan hesap için e-postaya parola kodu ister (sıfırlamanın
  /// kodu; sunucu her adrese aynı cevabı verir).
  Future<void> requestPasswordCode(String email);

  /// Hesabı ve bütün verisini siler (ADR 0017). Geri dönüşü yoktur.
  /// Parolası olan hesap [password], Google ile açılmış hesap Google'a
  /// yeniden girişin belirtecini ([googleIdToken]) gönderir.
  Future<void> deleteAccount({String? password, String? googleIdToken});

  /// Kullanıcının kendi adresine doğrulama kodu göndertir.
  Future<VerificationSendResult> sendEmailVerification();

  /// Adresi e-postayla gelen kodla doğrular.
  Future<void> confirmEmail(String code);
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
    String? currentPassword,
    required String newPassword,
    String? resetCode,
  }) async {
    final response = await _apiClient.post(
      '/api/v1/account/password',
      body: {
        'currentPassword': ?currentPassword,
        'newPassword': newPassword,
        'resetCode': ?resetCode,
      },
    );
    return RotatedTokens.fromJson(response.requireObject());
  }

  @override
  Future<void> requestPasswordCode(String email) async {
    await _apiClient.post(
      '/api/v1/auth/password-reset',
      body: {'email': email},
    );
  }

  @override
  Future<VerificationSendResult> sendEmailVerification() async {
    final response = await _apiClient.post(
      '/api/v1/account/email-verification',
    );
    return VerificationSendResult.fromJson(response.requireObject());
  }

  @override
  Future<void> confirmEmail(String code) async {
    await _apiClient.post(
      '/api/v1/account/email-verification/confirm',
      body: {'code': code},
    );
  }

  @override
  Future<void> deleteAccount({String? password, String? googleIdToken}) async {
    // Onay istemcinin kendi kendine doldurduğu bir alan değil: kullanıcı
    // silme penceresini onayladığı ve parolasını yazdığı (ya da Google'a
    // yeniden girdiği) için buraya kadar geldi. Sunucu ikisini de ayrıca arar.
    await _apiClient.delete(
      '/api/v1/account',
      body: {
        'password': ?password,
        'googleIdToken': ?googleIdToken,
        'confirmed': true,
      },
    );
  }
}
