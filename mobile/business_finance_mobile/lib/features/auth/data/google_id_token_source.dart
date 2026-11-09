import 'package:flutter/foundation.dart';
import 'package:google_sign_in/google_sign_in.dart';

import 'auth_models.dart';

/// Google penceresini açıp kimlik belirtecini alır. Testlerde sahtesi
/// kullanılır.
abstract interface class GoogleIdTokenSource {
  /// Kişi pencereyi kapatırsa null döner (hata değildir, düğme geri açılır).
  /// Başka her sorun [GoogleSignInUnavailable] fırlatır.
  Future<String?> obtainIdToken();

  /// Çıkışta Google oturumu da kapanır; bir sonraki girişte hesap yeniden
  /// seçilebilir.
  Future<void> signOut();
}

/// Google Cloud projesindeki istemciler (9 Kasım 2026). Client ID gizli
/// bilgi değildir; uygulamanın içinde açıkça durur. Sunucu
/// (`GoogleSignIn:ClientIds`) belirteci Web ve iOS kimliğine karşı doğrular.
class GoogleClientIds {
  const GoogleClientIds._();

  /// Sunucunun doğruladığı istemci; Android belirteci bunun adına ister.
  static const web =
      '320912581988-u9v68e00a0s76khebq580du89onfob26.apps.googleusercontent.com';

  static const ios =
      '320912581988-pabn0nm9dttaeum2ld9shdlgdl9qefhk.apps.googleusercontent.com';
}

class GoogleSignInGateway implements GoogleIdTokenSource {
  Future<void>? _initialized;

  Future<void> _initialize() =>
      _initialized ??= GoogleSignIn.instance.initialize(
        clientId: defaultTargetPlatform == TargetPlatform.iOS
            ? GoogleClientIds.ios
            : null,
        serverClientId: GoogleClientIds.web,
      );

  @override
  Future<String?> obtainIdToken() async {
    try {
      await _initialize();
      final account = await GoogleSignIn.instance.authenticate();
      final idToken = account.authentication.idToken;
      if (idToken == null) throw const GoogleSignInUnavailable();
      return idToken;
    } on GoogleSignInException catch (error) {
      if (error.code == GoogleSignInExceptionCode.canceled) return null;
      throw const GoogleSignInUnavailable();
    }
  }

  @override
  Future<void> signOut() async {
    if (_initialized == null) return;
    try {
      await _initialized;
      await GoogleSignIn.instance.signOut();
    } on Object {
      // Google oturumunu kapatamamak uygulamadan çıkışı engellemez.
    }
  }
}
