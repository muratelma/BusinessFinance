import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';

void main() {
  test('cihazdaki seçim oturumlar arası hatırlanır', () async {
    final store = _FakeStore(
      scope: TransactionScope.business,
      hasBusiness: true,
    );
    final controller = ScopeController(store: store);

    await controller.ensureLoaded();

    expect(controller.scope, TransactionScope.business);
    expect(controller.isVisible, isTrue);
    expect(controller.isLoaded, isTrue);
  });

  test('seçim değişince cihaza yazılır', () async {
    final store = _FakeStore(hasBusiness: true);
    final controller = ScopeController(store: store);
    await controller.ensureLoaded();

    await controller.select(TransactionScope.personal);

    expect(store.scope, TransactionScope.personal);
    expect(controller.scope, TransactionScope.personal);
  });

  test('"Hepsi" seçimi cihazdan silinir, boş değer olarak yazılmaz', () async {
    final store = _FakeStore(
      scope: TransactionScope.personal,
      hasBusiness: true,
    );
    final controller = ScopeController(store: store);
    await controller.ensureLoaded();

    await controller.select(null);

    expect(store.scope, isNull);
    expect(controller.scope, isNull);
  });

  test('sunucudaki cevap cihazdaki kopyayı günceller', () async {
    final store = _FakeStore(hasBusiness: false);
    final controller = ScopeController(
      store: store,
      readHasBusiness: () async => true,
    );

    await controller.ensureLoaded();

    expect(controller.isVisible, isTrue);
    expect(store.hasBusiness, isTrue);
  });

  test(
    'profil okunamazsa cihazdaki kopya geçerli kalır — boyut kaybolmaz',
    () async {
      final store = _FakeStore(hasBusiness: true);
      final controller = ScopeController(
        store: store,
        readHasBusiness: () async => throw Exception('ağ düştü'),
      );

      await controller.ensureLoaded();

      expect(controller.isVisible, isTrue);
    },
  );

  test('işletmesi olmayan kullanıcıda kapsam filtresi uygulanmaz', () async {
    final store = _FakeStore(
      scope: TransactionScope.business,
      hasBusiness: false,
    );
    final controller = ScopeController(store: store);

    await controller.ensureLoaded();

    // Depoda bir seçim duruyor olabilir (kullanıcı cevabını sonradan
    // değiştirmiştir); boyut görünmüyorken uygulanmaz.
    expect(controller.isVisible, isFalse);
    expect(controller.scope, isNull);
  });

  test('ensureLoaded birden çok çağrıda tek okuma yapar', () async {
    final store = _FakeStore(hasBusiness: true);
    var reads = 0;
    final controller = ScopeController(
      store: store,
      readHasBusiness: () async {
        reads++;
        return true;
      },
    );

    await Future.wait([
      controller.ensureLoaded(),
      controller.ensureLoaded(),
      controller.ensureLoaded(),
    ]);

    expect(reads, 1);
  });

  test('oturum kapanınca seçim ve cevap unutulur', () async {
    final store = _FakeStore(
      scope: TransactionScope.business,
      hasBusiness: true,
    );
    final controller = ScopeController(store: store);
    await controller.ensureLoaded();

    await controller.forget();

    expect(controller.scope, isNull);
    expect(controller.isVisible, isFalse);
    expect(store.scope, isNull);
    expect(store.hasBusiness, isNull);
  });

  test('ayarlardan gelen cevap hem uygulanır hem yazılır', () async {
    final store = _FakeStore(hasBusiness: true);
    final controller = ScopeController(store: store);
    await controller.ensureLoaded();

    await controller.applyHasBusiness(false);

    expect(controller.isVisible, isFalse);
    expect(store.hasBusiness, isFalse);
  });

  // Cari hesap kapısı: işletmesi olana her zaman açık; olmayana yalnız cari
  // hareketi varsa (Aşama 06.3 C6 — ön ayar, kilit değil).
  test(
    'cari kapısı işletmesi olmayan kullanıcıda cari harekete bakar',
    () async {
      var asked = 0;
      final withLedger = ScopeController(
        readHasBusiness: () async => false,
        readHasCounterpartyLedger: () async {
          asked++;
          return true;
        },
      );
      await withLedger.ensureLoaded();
      expect(withLedger.isVisible, isFalse);
      expect(withLedger.showsCounterpartyLedger, isTrue);
      expect(asked, 1);

      final withoutLedger = ScopeController(
        readHasBusiness: () async => false,
        readHasCounterpartyLedger: () async => false,
      );
      await withoutLedger.ensureLoaded();
      expect(withoutLedger.showsCounterpartyLedger, isFalse);
    },
  );

  test(
    'işletmesi olan kullanıcıda cari hareket sorulmaz, kapı açıktır',
    () async {
      var asked = 0;
      final controller = ScopeController(
        readHasBusiness: () async => true,
        readHasCounterpartyLedger: () async {
          asked++;
          return false;
        },
      );
      await controller.ensureLoaded();

      expect(controller.showsCounterpartyLedger, isTrue);
      expect(asked, 0);
    },
  );

  test(
    'cevap "işletmem yok" olunca cari hareketi olan kapıyı kaybetmez',
    () async {
      final controller = ScopeController(store: _FakeStore(hasBusiness: true));
      await controller.ensureLoaded();

      await controller.applyHasBusiness(false, hasCounterpartyLedger: true);
      expect(controller.isVisible, isFalse);
      expect(controller.showsCounterpartyLedger, isTrue);

      await controller.forget();
      expect(controller.showsCounterpartyLedger, isFalse);
    },
  );

  test('cari hareket okunamazsa kapı son bilinen hâlinde kalır', () async {
    final controller = ScopeController(
      readHasBusiness: () async => false,
      readHasCounterpartyLedger: () async => throw Exception('ağ düştü'),
    );
    await controller.ensureLoaded();

    expect(controller.showsCounterpartyLedger, isFalse);
  });

  test('aynı seçime dokunmak dinleyicileri uyandırmaz', () async {
    final controller = ScopeController(
      store: _FakeStore(scope: TransactionScope.business, hasBusiness: true),
    );
    await controller.ensureLoaded();
    var notifications = 0;
    controller.addListener(() => notifications++);

    await controller.select(TransactionScope.business);

    expect(notifications, 0);
  });
}

class _FakeStore implements ScopeStore {
  _FakeStore({this.scope, this.hasBusiness});

  TransactionScope? scope;
  bool? hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => scope;

  @override
  Future<void> writeScope(TransactionScope? value) async => scope = value;

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async => hasBusiness = value;

  @override
  Future<void> clear() async {
    scope = null;
    hasBusiness = null;
  }
}
