import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:timezone/timezone.dart' as tz;

import 'reminder_models.dart';

/// Telefonun kendi zamanlayıcısına yazma yeteneği, uygulamanın geri kalanından
/// bir port'la ayrılır.
///
/// Eklenti yalnız [LocalNotificationScheduler] içinde geçer: controller ve
/// planlayıcı testlerinin hiçbiri platform kanalına dokunmaz.
abstract interface class NotificationSchedulerContract {
  /// Kullanıcı bildirime izin verdi mi.
  ///
  /// **Yalnız kullanıcı hatırlatmayı açtığında** çağrılır; izin istemeden
  /// bildirim planlanmaz. Reddedilirse `false` döner ve uygulama sessizce
  /// çalışmaya devam eder.
  Future<bool> requestPermission();

  /// Kurulmuş bütün hatırlatmaları düşürür.
  ///
  /// Yeniden planlama her seferinde bununla başlar: ödenmiş faturanın
  /// bildiriminin ayrı ayrı bulunup iptal edilmesi gerekmez, liste baştan
  /// kurulur ve o kalem artık listede yoktur.
  Future<void> cancelAll();

  Future<void> schedule(ScheduledReminder reminder);
}

class LocalNotificationScheduler implements NotificationSchedulerContract {
  LocalNotificationScheduler({FlutterLocalNotificationsPlugin? plugin})
    : _plugin = plugin ?? FlutterLocalNotificationsPlugin();

  static const _channelId = 'reminders';
  static const _channelName = 'Hatırlatmalar';
  static const _channelDescription =
      'Vadesi gelen kayıtlar için gün başında tek bildirim.';

  final FlutterLocalNotificationsPlugin _plugin;
  bool _initialized = false;

  @override
  Future<bool> requestPermission() async {
    await _ensureInitialized();
    if (defaultTargetPlatform == TargetPlatform.android) {
      final android = _plugin
          .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin
          >();
      return await android?.requestNotificationsPermission() ?? false;
    }
    final ios = _plugin
        .resolvePlatformSpecificImplementation<
          IOSFlutterLocalNotificationsPlugin
        >();
    return await ios?.requestPermissions(alert: true, badge: true) ?? false;
  }

  @override
  Future<void> cancelAll() async {
    await _ensureInitialized();
    await _plugin.cancelAll();
  }

  @override
  Future<void> schedule(ScheduledReminder reminder) async {
    await _ensureInitialized();
    await _plugin.zonedSchedule(
      id: reminder.id,
      title: reminder.title,
      body: reminder.body,
      scheduledDate: tz.TZDateTime.from(reminder.at, tz.local),
      // Kesin alarm (`exact*`) Android 14'ten beri ayrı bir izin ister ve
      // takvim/çalar saat uygulamaları için ayrılmıştır. Sabah 9'da düşecek bir
      // hatırlatmanın saniye hassasiyetine ihtiyacı yok; birkaç dakikalık
      // kayma, kullanıcıyı ikinci bir izin diyaloğuna sokmaktan iyidir.
      androidScheduleMode: AndroidScheduleMode.inexactAllowWhileIdle,
      notificationDetails: const NotificationDetails(
        android: AndroidNotificationDetails(
          _channelId,
          _channelName,
          channelDescription: _channelDescription,
          importance: Importance.defaultImportance,
          priority: Priority.defaultPriority,
        ),
        iOS: DarwinNotificationDetails(),
      ),
    );
  }

  Future<void> _ensureInitialized() async {
    if (_initialized) return;
    _configureLocalTimeZone();
    await _plugin.initialize(
      settings: const InitializationSettings(
        // Uygulamanın kendi başlatıcı ikonu; ayrı bir bildirim ikonu varlığı
        // eklenmedi (Aşama 06.2'nin işi).
        android: AndroidInitializationSettings('@mipmap/ic_launcher'),
        iOS: DarwinInitializationSettings(
          // İzin kullanıcı hatırlatmayı açtığında ayrıca isteniyor; başlatma
          // anında istemek, hiç hatırlatma istemeyen kullanıcıya da diyalog
          // gösterirdi.
          requestAlertPermission: false,
          requestBadgePermission: false,
          requestSoundPermission: false,
        ),
      ),
    );
    _initialized = true;
  }

  /// Cihazın **o anki** UTC farkından tek dilimli bir konum kurar.
  ///
  /// `timezone` paketi zamanlamayı bir IANA konumu üzerinden yapıyor; konumun
  /// adını öğrenmek için ayrı bir yerel eklenti (`flutter_timezone`) gerekirdi.
  /// Üçüncü bir native bağımlılık yerine cihazın bildirdiği fark kullanılıyor.
  ///
  /// Bedeli tek ve sınırlı: yaz saati geçişinin **öbür tarafına** kurulmuş bir
  /// hatırlatma bir saat kayar. Kendi kendini toparlıyor — liste uygulama her
  /// açıldığında ve veri her değiştiğinde o anki farkla yeniden kuruluyor.
  /// Ürünün hedef kitlesi Türkiye'de ve Türkiye 2016'dan beri yaz saati
  /// uygulamıyor.
  void _configureLocalTimeZone() {
    final now = DateTime.now();
    final zone = tz.TimeZone(
      now.timeZoneOffset,
      isDst: false,
      abbreviation: now.timeZoneName,
    );
    tz.setLocalLocation(
      tz.Location(now.timeZoneName, const <int>[], const <int>[], <tz.TimeZone>[
        zone,
      ]),
    );
  }
}
