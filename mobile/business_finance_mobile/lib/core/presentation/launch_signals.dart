import 'dart:async';

import 'package:flutter/foundation.dart';

/// Açılış perdesi (`LaunchCurtain`) ile ekranlar arasındaki sinyaller.
///
/// Perde uygulama katmanındadır; giriş ekranı ve Özet onu tanımaz, yalnız
/// bu sinyalleri okur ya da yazar.
abstract final class LaunchHandoff {
  static bool _pending = false;
  static Completer<void>? _curtain;

  /// Perde açılışta kurulurken çağrılır.
  static void begin() {
    _pending = true;
    _curtain = Completer<void>();
    contentReady.value = false;
  }

  /// Oturum açık çıktı: giriş ekranına devredilecek bir işaret yok.
  static void cancelHandoff() => _pending = false;

  /// Perde çekildi.
  static void release() {
    final curtain = _curtain;
    if (curtain != null && !curtain.isCompleted) curtain.complete();
  }

  /// Giriş ekranı açılışını işaretin perdedeki yerinden mi başlatsın?
  /// Yalnız uygulama açılırken ve bir kez doğrudur; çıkış yapıldıktan sonra
  /// açılış her zamanki gibi işaretin belirmesiyle başlar.
  static bool take() {
    final pending = _pending;
    _pending = false;
    return pending;
  }

  /// Perde çekilince tamamlanır. Perde yoksa hemen tamamdır.
  static Future<void> get curtainLifted => _curtain?.future ?? Future.value();

  /// Özet ilk verisini çizdi (ya da hatasını gösterdi): perde kalkabilir.
  static final ValueNotifier<bool> contentReady = ValueNotifier(false);

  @visibleForTesting
  static void reset() {
    release();
    _curtain = null;
    _pending = false;
    contentReady.value = false;
  }
}
