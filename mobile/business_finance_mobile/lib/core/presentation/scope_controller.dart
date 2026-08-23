import 'package:flutter/foundation.dart';

import '../models/transaction_scope.dart';

/// Kapsam seçiminin ve onboarding cevabının cihazdaki hâli.
///
/// İkisi de secret değil; uygulamanın tek cihaz deposu neyse o kullanılır.
/// Cevabın **kopyası** cihazda duruyor, kaynağı değil: kaynak sunucudaki
/// `UserProfile`. Kopya yalnız sunucu okunamadığında boyutun sessizce
/// kaybolmasını engeller.
abstract interface class ScopeStore {
  Future<TransactionScope?> readScope();

  Future<void> writeScope(TransactionScope? scope);

  /// Hiç okunmamışsa `null`. `false` ile karıştırılmaz: biri "cevap
  /// bilinmiyor", öteki "işletmesi yok" demektir.
  Future<bool?> readHasBusiness();

  Future<void> writeHasBusiness(bool value);

  /// Oturum kapanınca çağrılır; aynı cihazdan giren ikinci kullanıcı
  /// birincisinin anahtar konumunu devralmaz.
  Future<void> clear();
}

/// Sunucudaki onboarding cevabını okuyan dar imza.
///
/// Bütün profil deposu yerine tek bir fonksiyon alınıyor: bu sınıfın profil
/// hakkında bilmesi gereken tek şey bu.
typedef HasBusinessReader = Future<bool> Function();

/// Uygulama genelinde aktif kapsam.
///
/// **Sekme başına ayrı filtre yoktur** (ADR 0013): tek anahtar, bütün bölünen
/// okumalar. Anahtar yalnız gelir/gider tarafını böler; bakiye, kart borcu ve
/// net varlık her konumda toplamı gösterir.
///
/// "İşletmem yok" diyen kullanıcıda boyut arayüzde hiç görünmez ve
/// [scope] daima boştur: filtre gönderilmez, bütün kayıtlar sessizce şahsi
/// olur.
class ScopeController extends ChangeNotifier {
  ScopeController({this.store, this.readHasBusiness});

  /// Cihaz deposu. Boş bırakılırsa seçim hatırlanmaz (test ya da bağlanmamış
  /// kabuk); denetim yine çalışır.
  final ScopeStore? store;

  /// Sunucudaki cevabı okuyan fonksiyon; boşsa yalnız cihazdaki kopya geçerli.
  final HasBusinessReader? readHasBusiness;

  TransactionScope? _scope;
  bool _hasBusiness = false;
  bool _isLoaded = false;
  Future<void>? _loading;

  /// Aktif kapsam; boş değer "iki tarafı birden oku" demektir.
  ///
  /// İşletmesi olmayan kullanıcıda seçim ne olursa olsun boş döner — boyut
  /// görünmediği hâlde bir filtre uygulanıyor olsaydı kullanıcı sebebini
  /// göremediği bir eksik liste görürdü.
  TransactionScope? get scope => _hasBusiness ? _scope : null;

  /// Kapsam boyutu arayüzde görünsün mü.
  bool get isVisible => _hasBusiness;

  /// Profil ve tercih okunmayı bitirdi mi. Yükleme sırasında boyut henüz
  /// çizilmez; yanıp sönen bir anahtar, kullanıcının dokunduğu şeyin ne
  /// olduğunu belirsizleştirir.
  bool get isLoaded => _isLoaded;

  /// Cihazdaki tercihi ve sunucudaki cevabı okur; birden çok çağrıda tek
  /// istek yapılır (kabuk her sekmede çağırabilsin diye).
  Future<void> ensureLoaded() => _loading ??= _load();

  Future<void> _load() async {
    final store = this.store;
    if (store != null) {
      // Önce cihazdaki kopya: sunucu yavaşsa ya da düşmüşse ekran, boyutu hiç
      // olmamış gibi göstermek yerine en son bilinen hâliyle açılır.
      _scope = await store.readScope();
      _hasBusiness = await store.readHasBusiness() ?? false;
    }
    _isLoaded = true;
    notifyListeners();

    final reader = readHasBusiness;
    if (reader == null) return;
    try {
      final hasBusiness = await reader();
      await store?.writeHasBusiness(hasBusiness);
      if (hasBusiness == _hasBusiness) return;
      _hasBusiness = hasBusiness;
      notifyListeners();
    } on Exception {
      // Profil okunamadı. Cihazdaki kopya geçerli kalır: cevabı "hayır"
      // varsaymak, işletme sahibinin boyutunu bir ağ hatası yüzünden
      // kaybettirirdi.
    }
  }

  Future<void> select(TransactionScope? value) async {
    if (_scope == value) return;
    _scope = value;
    notifyListeners();
    await store?.writeScope(value);
  }

  /// Sunucudaki cevap değiştiğinde (ayarlardan) çağrılır.
  Future<void> applyHasBusiness(bool value) async {
    await store?.writeHasBusiness(value);
    if (_hasBusiness == value) return;
    _hasBusiness = value;
    notifyListeners();
  }

  /// Oturum kapandı: seçim de cevap da unutulur.
  Future<void> forget() async {
    _scope = null;
    _hasBusiness = false;
    _isLoaded = false;
    _loading = null;
    notifyListeners();
    await store?.clear();
  }
}
