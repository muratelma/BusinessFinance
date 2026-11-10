import 'dart:async';

import '../features/dashboard/presentation/dashboard_view_model.dart';

/// Girişte Özet'i önceden yükler.
///
/// Sunucu girişi kabul edince oturum anahtarı kaydedilmiştir ama giriş ekranı
/// henüz açıktır: düğme dönmeye devam ederken Özet'in verisi istenir. Veri
/// gelince geçiş oynar ve Özet verisiyle açılır; arada "yükleniyor" ekranı
/// görünmez (kullanıcı kararı, 10 Kasım 2026). Yönlendirici Özet'i kurarken
/// hazır görünüm modelini [take] ile alır; ikinci bir istek gitmez.
class HomePrefetch {
  HomePrefetch(this._create, {this.wait = const Duration(seconds: 3)});

  final DashboardViewModel Function() _create;

  /// Veri bu süreden uzun sürerse geçiş yine oynar; Özet kendi yükleme
  /// hâliyle açılır. Giriş, yavaş ağda sonsuza dek bekletilmez.
  final Duration wait;

  DashboardViewModel? _ready;

  /// Özet'in görünüm modelini kurar ve ilk verisini bekler.
  Future<void> load() async {
    _ready?.dispose();
    final viewModel = _create();
    _ready = viewModel;
    try {
      await viewModel.load().timeout(wait);
    } on TimeoutException {
      // Geçiş beklemez; yükleme Özet'in içinde sürer.
    }
  }

  /// Hazırlanmış görünüm modeli; bir kez verilir.
  DashboardViewModel? take() {
    final ready = _ready;
    _ready = null;
    return ready;
  }

  void dispose() {
    _ready?.dispose();
    _ready = null;
  }
}
