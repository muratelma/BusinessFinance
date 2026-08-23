import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';

/// Kapsam anahtarının cihazdaki hâli.
///
/// İkisi de secret değil; yine de `flutter_secure_storage` kullanılıyor çünkü
/// uygulamanın tek cihaz deposu bu (`ReceiptPreferences` ile aynı gerekçe).
/// İki değer için ikinci bir depolama paketi eklemek, bakımı olan yeni bir
/// bağımlılık demekti.
class ScopePreferences implements ScopeStore {
  ScopePreferences({FlutterSecureStorage? storage})
    : _storage = storage ?? const FlutterSecureStorage();

  static const _scopeKey = 'active_transaction_scope_v1';
  static const _hasBusinessKey = 'profile_has_business_v1';

  final FlutterSecureStorage _storage;

  @override
  Future<TransactionScope?> readScope() async {
    final value = await _storage.read(key: _scopeKey);
    // Tanınmayan bir değer `Hepsi`ye düşer: eski bir sürümden kalan yazı
    // yüzünden uygulama açılışta patlamamalı.
    for (final scope in TransactionScope.values) {
      if (scope.apiValue == value) return scope;
    }
    return null;
  }

  @override
  Future<void> writeScope(TransactionScope? scope) => scope == null
      ? _storage.delete(key: _scopeKey)
      : _storage.write(key: _scopeKey, value: scope.apiValue);

  @override
  Future<bool?> readHasBusiness() async {
    final value = await _storage.read(key: _hasBusinessKey);
    if (value == null) return null;
    return value == 'true';
  }

  @override
  Future<void> writeHasBusiness(bool value) =>
      _storage.write(key: _hasBusinessKey, value: value ? 'true' : 'false');

  @override
  Future<void> clear() async {
    await _storage.delete(key: _scopeKey);
    await _storage.delete(key: _hasBusinessKey);
  }
}
