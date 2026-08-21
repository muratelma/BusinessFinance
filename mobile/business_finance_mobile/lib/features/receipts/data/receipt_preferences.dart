import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Fiş akışının cihazda hatırladığı iki şey.
///
/// İkisi de secret değil; yine de `flutter_secure_storage` kullanılıyor çünkü
/// uygulamanın zaten tek cihaz deposu bu. İki boolean için ikinci bir depolama
/// paketi eklemek, bakımı olan yeni bir bağımlılık demekti.
///
/// **Rıza kaydı v1'de cihazdadır.** Gerçek kullanıcıya açılırsa sunucuya
/// taşınması gerekir; KVKK kaydı cihazda tutulamaz. Yeniden kurulumda tekrar
/// sorulması zararsızdır — soru, veriyi göndermeden önce sorulur.
abstract interface class ReceiptPreferencesContract {
  /// Kullanıcı fotoğrafın Google'a gideceğini kabul etti mi.
  ///
  /// Hiç sorulmadıysa `false`: rıza varsayılmaz.
  Future<bool> readConsent();

  Future<void> writeConsent(bool granted);

  /// **Fişi sakla** anahtarının son durumu.
  ///
  /// Varsayılan **açık**: belge biriktirmek kullanıcının sonradan geri
  /// alabileceği bir şey (belgeyi siler), ama kaçırdığı fiş geri gelmez.
  Future<bool> readKeepPhoto();

  Future<void> writeKeepPhoto(bool keep);
}

class ReceiptPreferences implements ReceiptPreferencesContract {
  ReceiptPreferences({FlutterSecureStorage? storage})
    : _storage = storage ?? const FlutterSecureStorage();

  static const _consentKey = 'receipt_provider_consent_v1';
  static const _keepPhotoKey = 'receipt_keep_photo_v1';

  final FlutterSecureStorage _storage;

  @override
  Future<bool> readConsent() async =>
      await _storage.read(key: _consentKey) == 'true';

  @override
  Future<void> writeConsent(bool granted) =>
      _storage.write(key: _consentKey, value: granted ? 'true' : 'false');

  @override
  Future<bool> readKeepPhoto() async =>
      await _storage.read(key: _keepPhotoKey) != 'false';

  @override
  Future<void> writeKeepPhoto(bool keep) =>
      _storage.write(key: _keepPhotoKey, value: keep ? 'true' : 'false');
}
