import 'package:flutter/foundation.dart';

import '../../activities/data/activity_repository.dart';
import '../../activities/data/planned_activity_models.dart';
import '../data/notification_scheduler.dart';
import '../data/reminder_models.dart';
import '../data/reminder_planner.dart';
import '../data/reminder_preferences.dart';

/// Hatırlatmanın tek denetimi: ayarı okur, yazar ve zamanlayıcıyı güncel tutar.
///
/// Uygulama genelinde tektir ve kompozisyon kökünde kurulur — ayar ekranı
/// kapalıyken de (veri değişince) yeniden planlama yapması gerekir.
class ReminderController extends ChangeNotifier {
  ReminderController(
    this._preferences,
    this._scheduler,
    this._activityRepository, {
    this._planner = const ReminderPlanner(),
    DateTime Function()? now,
  }) : _now = now ?? DateTime.now;

  /// Hatırlatma **bir aylık** ufuktan kurulur.
  ///
  /// Doksan günlük ufuk da var ama üç ay sonrasına bildirim kurmak, o güne
  /// kadar defalarca değişecek bir listeyi telefona yazmak olurdu. Liste zaten
  /// uygulama her açıldığında ve veri her değiştiğinde yenileniyor.
  static const horizon = PlannedHorizon.month;

  final ReminderPreferencesContract _preferences;
  final NotificationSchedulerContract _scheduler;
  final ActivityRepositoryContract _activityRepository;
  final ReminderPlanner _planner;
  final DateTime Function() _now;

  ReminderSettings _settings = ReminderSettings.initial;
  bool _isLoaded = false;
  bool _isBusy = false;
  bool _permissionDenied = false;
  bool _syncFailed = false;
  int _scheduledCount = 0;

  ReminderSettings get settings => _settings;
  bool get isLoaded => _isLoaded;
  bool get isBusy => _isBusy;

  /// Kullanıcı hatırlatmayı açmak istedi ama cihaz izni vermedi. Uygulama
  /// çalışmaya devam eder; ekran bunu söyler.
  bool get permissionDenied => _permissionDenied;

  /// Planlanan liste okunamadı. **Kurulu bildirimler düşürülmez**: dünkü
  /// hatırlatmayı silmek, sunucuya ulaşılamadığı için kullanıcıyı faturasından
  /// habersiz bırakırdı.
  bool get syncFailed => _syncFailed;

  /// Şu an telefonun zamanlayıcısında duran gün sayısı.
  int get scheduledCount => _scheduledCount;

  Future<void> ensureLoaded() async {
    if (_isLoaded) return;
    _settings = await _preferences.read();
    _isLoaded = true;
    notifyListeners();
    await sync();
  }

  /// Kullanıcı hatırlatmayı açıp kapatır.
  ///
  /// İzin **yalnız burada** istenir: kapalıyken hiçbir izin sorulmaz ve hiçbir
  /// bildirim planlanmaz.
  Future<void> setEnabled(bool enabled) async {
    if (enabled) {
      _permissionDenied = false;
      _isBusy = true;
      notifyListeners();
      final granted = await _scheduler.requestPermission();
      _isBusy = false;
      if (!granted) {
        _permissionDenied = true;
        notifyListeners();
        return;
      }
    }
    await _apply(_settings.copyWith(isEnabled: enabled));
  }

  Future<void> setKindEnabled(ReminderKind kind, bool enabled) async {
    final kinds = Set<ReminderKind>.from(_settings.kinds);
    if (enabled) {
      kinds.add(kind);
    } else {
      kinds.remove(kind);
    }
    await _apply(_settings.copyWith(kinds: kinds));
  }

  Future<void> setTime({required int hour, required int minute}) =>
      _apply(_settings.copyWith(hour: hour, minute: minute));

  /// Planlanan listeyi yeniden okur ve zamanlayıcıyı baştan kurar.
  ///
  /// Veri değiştiğinde (ödeme yapıldı, yükümlülük kapandı) çağrılır: kurulu
  /// liste komple silinip yenisi yazıldığı için kapanan kalemin bildirimi
  /// kendiliğinden düşer.
  Future<void> sync() async {
    if (!_settings.schedulesAnything) {
      await _scheduler.cancelAll();
      _scheduledCount = 0;
      _syncFailed = false;
      notifyListeners();
      return;
    }

    _isBusy = true;
    notifyListeners();
    try {
      final page = await _activityRepository.listPlanned(horizon: horizon);
      final reminders = _planner.plan(
        items: page.items,
        settings: _settings,
        now: _now(),
      );
      await _scheduler.cancelAll();
      for (final reminder in reminders) {
        await _scheduler.schedule(reminder);
      }
      _scheduledCount = reminders.length;
      _syncFailed = false;
    } on Exception {
      _syncFailed = true;
    } finally {
      _isBusy = false;
      notifyListeners();
    }
  }

  /// Oturum kapanınca hem kurulu bildirimler hem ayar unutulur: aynı cihazdan
  /// giren ikinci kullanıcı birincisinin hatırlatmalarını devralmamalı.
  Future<void> forget() async {
    _settings = ReminderSettings.initial;
    _isLoaded = false;
    _permissionDenied = false;
    _syncFailed = false;
    _scheduledCount = 0;
    notifyListeners();
    await _preferences.clear();
    await _scheduler.cancelAll();
  }

  Future<void> _apply(ReminderSettings settings) async {
    _settings = settings;
    notifyListeners();
    await _preferences.write(settings);
    await sync();
  }
}
