import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import 'reminder_models.dart';

/// Hatırlatma ayarının cihazdaki hâli.
///
/// Secret değil; yine de `flutter_secure_storage` kullanılıyor çünkü uygulamanın
/// tek cihaz deposu bu (`ScopePreferences` ve `ReceiptPreferences` ile aynı
/// gerekçe). Üç değer için ikinci bir depolama paketi eklemek, bakımı olan yeni
/// bir bağımlılık demekti.
abstract interface class ReminderPreferencesContract {
  Future<ReminderSettings> read();

  Future<void> write(ReminderSettings settings);

  /// Oturum kapanınca çağrılır: aynı cihazdan giren ikinci kullanıcı
  /// birincisinin hatırlatma ayarını devralmamalı.
  Future<void> clear();
}

class ReminderPreferences implements ReminderPreferencesContract {
  ReminderPreferences({FlutterSecureStorage? storage})
    : _storage = storage ?? const FlutterSecureStorage();

  static const _enabledKey = 'reminders_enabled_v1';
  static const _kindsKey = 'reminders_kinds_v1';
  static const _timeKey = 'reminders_time_v1';

  final FlutterSecureStorage _storage;

  @override
  Future<ReminderSettings> read() async {
    const fallback = ReminderSettings.initial;
    final enabled = await _storage.read(key: _enabledKey) == 'true';
    final kinds = await _readKinds() ?? fallback.kinds;
    final time = await _readTime();

    return ReminderSettings(
      isEnabled: enabled,
      kinds: kinds,
      hour: time?.$1 ?? fallback.hour,
      minute: time?.$2 ?? fallback.minute,
    );
  }

  @override
  Future<void> write(ReminderSettings settings) async {
    await _storage.write(
      key: _enabledKey,
      value: settings.isEnabled ? 'true' : 'false',
    );
    await _storage.write(
      key: _kindsKey,
      value: settings.kinds.map((kind) => kind.storageValue).join(','),
    );
    await _storage.write(
      key: _timeKey,
      value: '${settings.hour}:${settings.minute}',
    );
  }

  @override
  Future<void> clear() async {
    await _storage.delete(key: _enabledKey);
    await _storage.delete(key: _kindsKey);
    await _storage.delete(key: _timeKey);
  }

  /// Hiç yazılmamışsa `null` döner ve varsayılan küme kullanılır; **boş dizge**
  /// ise gerçekten boş kümedir — kullanıcı bütün türleri kapatmış olabilir.
  Future<Set<ReminderKind>?> _readKinds() async {
    final value = await _storage.read(key: _kindsKey);
    if (value == null) return null;
    if (value.isEmpty) return <ReminderKind>{};

    // Tanınmayan değer sessizce düşer: eski bir sürümden kalan bir tür yüzünden
    // uygulama açılışta patlamamalı.
    return {
      for (final part in value.split(',')) ?ReminderKind.fromStorage(part),
    };
  }

  Future<(int, int)?> _readTime() async {
    final value = await _storage.read(key: _timeKey);
    final parts = value?.split(':');
    if (parts == null || parts.length != 2) return null;

    final hour = int.tryParse(parts[0]);
    final minute = int.tryParse(parts[1]);
    if (hour == null || minute == null) return null;
    if (hour < 0 || hour > 23 || minute < 0 || minute > 59) return null;
    return (hour, minute);
  }
}
