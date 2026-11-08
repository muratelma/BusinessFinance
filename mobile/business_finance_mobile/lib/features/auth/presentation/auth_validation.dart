/// Giriş ve kayıt alanlarının istemci denetimi.
///
/// Cümleler "siz" diye konuşur (kullanıcı kararı, 8 Kasım 2026: "sen"li
/// deneme beğenilmedi, eski cümleler geri geldi). Değişen yalnız parola
/// kuralıdır: alanın altına tek satır sığar.
abstract final class AuthValidation {
  static const emailEmpty = 'E-posta adresinizi girin.';
  static const emailInvalid = 'Geçerli bir e-posta adresi girin.';
  static const identifierEmpty =
      'E-posta adresinizi ya da kullanıcı adınızı girin.';
  static const usernameLoginNotReady =
      'Kullanıcı adıyla giriş henüz açılmadı; e-posta adresinizi girin.';
  static const usernameEmpty = 'Bir kullanıcı adı seçin.';
  static const usernameTooShort = 'Kullanıcı adı en az 3 karakter olmalıdır.';
  static const usernameTooLong = 'Kullanıcı adı en çok 30 karakter olabilir.';
  static const usernameCharacters =
      'Yalnız a–z, rakam, nokta ve alt çizgi kullanın.';
  static const passwordEmpty = 'Parolanızı girin.';

  /// Parola kuralı tek cümledir ve alanın altına tek satır sığar (Figtree
  /// 11,5 pt'de 243 pt; sınır ~255; "En az 12 karakter; …" 266 pt ile taşıyordu). Hangi
  /// koşul tutmazsa tutmasın aynısı yazar: kuralın tamamını görmek, bir
  /// eksiği düzeltince sıradakiyle karşılaşmaktan iyidir.
  static const passwordRule = '12+ karakter; büyük/küçük harf, rakam, sembol.';

  static String? email(String? value) {
    final email = value?.trim() ?? '';
    if (email.isEmpty) {
      return emailEmpty;
    }
    final at = email.indexOf('@');
    if (at <= 0 ||
        at == email.length - 1 ||
        !email.substring(at + 1).contains('.')) {
      return emailInvalid;
    }
    return null;
  }

  /// Girişteki tek alan: e-posta ya da kullanıcı adı. Sunucu kullanıcı adıyla
  /// girişi henüz bilmiyor (Aşama 06.4, sunucu adımı sırada); kullanıcı adı
  /// yazan kişiye istek gitmeden söylenir, "parola yanlış" diye yanıltılmaz.
  static String? loginIdentifier(String? value) {
    final text = value?.trim() ?? '';
    if (text.isEmpty) {
      return identifierEmpty;
    }
    if (!text.contains('@')) {
      return usernameLoginNotReady;
    }
    return email(text);
  }

  /// Kullanıcı adı: 3–30 karakter; Türkçe olmayan harf, rakam, nokta ve alt
  /// çizgi. Cümle "harf" demez, "a–z" der: "ı" yazan kişi "harf olabilir"
  /// okuyup neden reddedildiğini anlamazdı.
  static String? username(String? value) {
    final name = value?.trim() ?? '';
    if (name.isEmpty) {
      return usernameEmpty;
    }
    if (name.length < 3) {
      return usernameTooShort;
    }
    if (name.length > 30) {
      return usernameTooLong;
    }
    if (!RegExp(r'^[a-zA-Z0-9._]+$').hasMatch(name)) {
      return usernameCharacters;
    }
    return null;
  }

  static String? loginPassword(String? value) {
    if (value == null || value.isEmpty) {
      return passwordEmpty;
    }
    return null;
  }

  static String? registrationPassword(String? value) {
    final password = value ?? '';
    if (password.length < 12 ||
        !password.contains(RegExp('[a-z]')) ||
        !password.contains(RegExp('[A-Z]')) ||
        !password.contains(RegExp('[0-9]')) ||
        !password.contains(RegExp(r'[^a-zA-Z0-9]'))) {
      return passwordRule;
    }
    return null;
  }
}
