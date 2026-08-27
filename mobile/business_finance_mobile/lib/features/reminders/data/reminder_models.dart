import '../../activities/data/planned_activity_models.dart';

/// Kullanıcının hatırlatma isteyip istemediğini tür tür seçtiği kovalar.
///
/// Planlanan görünümün yedi türü burada beşe iniyor: kullanıcı "kart taksidi mi
/// borç taksidi mi" diye ayrı ayrı düşünmüyor, "taksitlerimi hatırlat" diyor.
/// Eşleme tek yerde ([matches]) durur; ikinci bir kopyası olsaydı yeni bir
/// planlanan tür eklendiğinde biri sessizce geride kalırdı.
enum ReminderKind {
  obligation('obligation', 'Ödenecek yükümlülükler', 'ödenecek yükümlülük'),
  receivable('receivable', 'Tahsil edilecekler', 'tahsil edilecek alacak'),
  recurring('recurring', 'Tekrarlanan kayıtlar', 'tekrarlanan kayıt'),
  cardStatement('card-statement', 'Kart ekstreleri', 'kart ekstresi'),
  installment('installment', 'Taksitler', 'taksit');

  const ReminderKind(this.storageValue, this.label, this.countedNoun);

  /// Cihazdaki ayar dosyasına yazılan kararlı değer. Enum adının kendisi
  /// yazılsaydı bir yeniden adlandırma kullanıcının seçimini silerdi.
  final String storageValue;

  /// Ayar ekranındaki satır başlığı.
  final String label;

  /// Bildirim gövdesinde sayıyla birlikte okunan hâli ("2 taksit").
  final String countedNoun;

  static ReminderKind? fromStorage(String value) {
    for (final kind in ReminderKind.values) {
      if (kind.storageValue == value) return kind;
    }
    return null;
  }

  /// Vergi takvimi kaleminin kendi kovası **yok**: kalem tekrarlayan bir
  /// plandır (Aşama 05) ve planlanan görünüme `recurring-occurrence` olarak
  /// düşer. Ona ayrı bir kova açmak, aynı kaydı iki yerden hatırlatırdı.
  bool matches(PlannedActivity item) => switch (this) {
    obligation => item.plannedKind == PlannedKind.payableObligation,
    receivable => item.plannedKind == PlannedKind.receivableObligation,
    recurring => item.plannedKind == PlannedKind.recurringOccurrence,
    cardStatement => item.plannedKind == PlannedKind.cardStatement,
    installment =>
      item.plannedKind == PlannedKind.cardInstallment ||
          item.plannedKind == PlannedKind.debtInstallment ||
          item.plannedKind == PlannedKind.receivableInstallment,
  };
}

/// Hatırlatmanın cihazdaki ayarı.
///
/// Sunucuda durmaz ve durmamalı: hatırlatma telefonun kendi zamanlayıcısına
/// yazılır, bilgisayar kapalıyken de çalışır ve hangi cihazın ne zaman uyaracağı
/// o cihazın kararıdır (Aşama 06 Grup 3).
class ReminderSettings {
  const ReminderSettings({
    required this.isEnabled,
    required this.kinds,
    required this.hour,
    required this.minute,
  });

  /// Hiç dokunulmamış hâl: **kapalı**.
  ///
  /// Bildirim izni sorulmadan hatırlatma kurulmaz; açık gelseydi uygulama
  /// kullanıcıya sormadığı bir şeyi yapmaya başlardı.
  static const initial = ReminderSettings(
    isEnabled: false,
    kinds: {
      ReminderKind.obligation,
      ReminderKind.receivable,
      ReminderKind.recurring,
      ReminderKind.cardStatement,
      ReminderKind.installment,
    },
    hour: 9,
    minute: 0,
  );

  final bool isEnabled;
  final Set<ReminderKind> kinds;
  final int hour;
  final int minute;

  /// Açık ama hiçbir tür seçilmemişse kurulacak bildirim de yoktur.
  bool get schedulesAnything => isEnabled && kinds.isNotEmpty;

  String get timeLabel =>
      '${hour.toString().padLeft(2, '0')}:${minute.toString().padLeft(2, '0')}';

  ReminderSettings copyWith({
    bool? isEnabled,
    Set<ReminderKind>? kinds,
    int? hour,
    int? minute,
  }) => ReminderSettings(
    isEnabled: isEnabled ?? this.isEnabled,
    kinds: kinds ?? this.kinds,
    hour: hour ?? this.hour,
    minute: minute ?? this.minute,
  );

  @override
  bool operator ==(Object other) =>
      other is ReminderSettings &&
      other.isEnabled == isEnabled &&
      other.hour == hour &&
      other.minute == minute &&
      other.kinds.length == kinds.length &&
      other.kinds.containsAll(kinds);

  @override
  int get hashCode =>
      Object.hash(isEnabled, hour, minute, Object.hashAllUnordered(kinds));
}

/// Zamanlayıcıya yazılacak tek bildirim.
///
/// Gövdesinde **tutar ve karşı taraf adı geçmez**: kilit ekranında görünen bir
/// metin finansal veri taşıyamaz. Ne olduğunu söyler, ne kadar olduğunu değil.
class ScheduledReminder {
  const ScheduledReminder({
    required this.id,
    required this.at,
    required this.title,
    required this.body,
  });

  /// Günden türetilen kararlı kimlik (`20260901`). Aynı gün ikinci kez
  /// planlandığında yeni bir bildirim doğmaz, mevcut olanın üstüne yazılır.
  final int id;
  final DateTime at;
  final String title;
  final String body;

  @override
  bool operator ==(Object other) =>
      other is ScheduledReminder &&
      other.id == id &&
      other.at == at &&
      other.title == title &&
      other.body == body;

  @override
  int get hashCode => Object.hash(id, at, title, body);
}
