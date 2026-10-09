import '../../../core/network/api_client.dart';
import '../../../core/models/json_readers.dart';

/// Kaydolurken sorulan tek sorunun cevabı.
///
/// Cihazda değil sunucuda duruyor: uygulamayı silip yeniden kuran ya da ikinci
/// cihazdan giren kullanıcı işletme sahibi olmayı kaybetmemeli.
class UserProfile {
  const UserProfile({
    required this.hasBusiness,
    this.hasCounterpartyLedger = false,
  });

  final bool hasBusiness;

  /// Kullanıcının cari hareketi var. Cevap "işletmem yok" olsa da `Cari
  /// hesap` kapısı bu durumda görünür: gizleme bir ön ayardır.
  final bool hasCounterpartyLedger;

  factory UserProfile.fromJson(Map<String, dynamic> json) => UserProfile(
    hasBusiness: JsonReaders.boolean(json, 'hasBusiness'),
    hasCounterpartyLedger: json['hasCounterpartyLedger'] == true,
  );
}

abstract interface class ProfileRepositoryContract {
  Future<UserProfile> read();

  /// Cevabı değiştirir. **Kategorilere dokunmaz**: varsayılan set yalnız hiç
  /// kategorisi olmayan kullanıcıya bir kez kurulur, sonradan geri getirilmez.
  Future<UserProfile> update({required bool hasBusiness});
}

class ProfileRepository implements ProfileRepositoryContract {
  const ProfileRepository(this._apiClient);

  final ApiClient _apiClient;

  @override
  Future<UserProfile> read() async {
    final response = await _apiClient.get('/api/v1/profile');
    return UserProfile.fromJson(response.requireObject());
  }

  @override
  Future<UserProfile> update({required bool hasBusiness}) async {
    final response = await _apiClient.put(
      '/api/v1/profile',
      body: {'hasBusiness': hasBusiness},
    );
    return UserProfile.fromJson(response.requireObject());
  }
}
