import 'package:flutter/foundation.dart';

import '../data/account_repository.dart';

/// Uygulama genelinde tek soru: "bu adres doğrulandı mı?".
///
/// Uyarının doğal yeri, gidip düzelteceği sayfanın kapısıdır — Özet ekranının
/// sağ üstündeki hesap ikonu. İkonun üstündeki nokta bunu okur.
///
/// Kendi başına bir yükleme turu olması bilinçli: Özet'in view model'i finansal
/// veriyi okur ve hesabın durumu oraya ait değildir. Hata **sessizdir**: hesap
/// bilgisi alınamadığında nokta hiç çıkmaz, çünkü olmayan bir uyarıyı göstermek
/// kullanıcıyı doğrulanmış bir adresi doğrulamaya gönderirdi.
class AccountStatusController extends ChangeNotifier {
  AccountStatusController(this._repository);

  final AccountRepositoryContract _repository;

  bool _isLoaded = false;
  bool _emailConfirmed = true;

  String? _email;

  /// Yalnız yükleme başarılıysa ve adres doğrulanmamışsa doğrudur.
  bool get needsEmailVerification => _isLoaded && !_emailConfirmed;

  /// Oturumdaki e-posta; okunamadıysa `null`.
  String? get email => _email;

  /// Avatarın baş harfleri: e-postanın yerel kısmından iki harf (`ME`).
  /// Uygulamada profil fotoğrafı yok; e-posta okunamadıysa boş.
  String get initials => accountInitials(_email);

  Future<void> ensureLoaded() async {
    if (_isLoaded) return;
    await refresh();
  }

  Future<void> refresh() async {
    try {
      final account = await _repository.read();
      _emailConfirmed = account.emailConfirmed;
      _email = account.email;
      _isLoaded = true;
      notifyListeners();
    } on Exception {
      // Sessiz: uyarı göstermemek, yanlış uyarı göstermekten iyidir.
    }
  }

  /// Oturum kapanınca unutulur: aynı cihazdan giren ikinci kullanıcı
  /// birincisinin uyarısını devralmamalı.
  void forget() {
    _isLoaded = false;
    _emailConfirmed = true;
    _email = null;
    notifyListeners();
  }
}

/// E-postanın yerel kısmından iki büyük harf; Türkçe `i` → `İ` doğru
/// büyütülür. Harf yoksa boş metin.
String accountInitials(String? email) {
  if (email == null) return '';
  final local = email.split('@').first;
  final letters = local.replaceAll(RegExp(r'[^A-Za-zÇĞİÖŞÜçğıöşü]'), '');
  if (letters.isEmpty) return '';
  final picked = letters.length >= 2 ? letters.substring(0, 2) : letters;
  return picked.replaceAll('i', 'İ').replaceAll('ı', 'I').toUpperCase();
}
