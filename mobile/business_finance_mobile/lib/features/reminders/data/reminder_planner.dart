import '../../activities/data/planned_activity_models.dart';
import 'reminder_models.dart';

/// Sunucudan gelen planlanan listeyi bildirimlere çeviren saf kural.
///
/// **İkinci bir vade mantığı kurmaz**: hangi kaydın ne zaman ödeneceğine
/// planlanan projection karar verir (kanonik kaynak), burası yalnız o listeyi
/// telefonun zamanlayıcısının anlayacağı biçime çevirir. Saf olması bilerek:
/// eklenti olmadan test edilebilir.
class ReminderPlanner {
  const ReminderPlanner();

  /// Bir günden fazlasını tek bildirimde toplamanın sebebi: aynı güne düşen
  /// beş kalem beş bildirim olsaydı kullanıcı ilk gün bildirimleri kapatırdı.
  ///
  /// [now]'dan önceye düşen gün **atlanır**. Geçmişe bildirim kurulamaz;
  /// gecikmiş kalem zaten uygulamanın planlanan görünümünde `Gecikmiş`
  /// rozetiyle duruyor.
  List<ScheduledReminder> plan({
    required List<PlannedActivity> items,
    required ReminderSettings settings,
    required DateTime now,
  }) {
    if (!settings.schedulesAnything) return const [];

    final byDate = <DateTime, List<ReminderKind>>{};
    for (final item in items) {
      final kind = _kindOf(item, settings.kinds);
      if (kind == null) continue;

      final due = _parseDate(item.dueDate);
      if (due == null) continue;

      final at = DateTime(
        due.year,
        due.month,
        due.day,
        settings.hour,
        settings.minute,
      );
      if (!at.isAfter(now)) continue;

      byDate.putIfAbsent(at, () => <ReminderKind>[]).add(kind);
    }

    final dates = byDate.keys.toList()..sort();
    return [
      for (final at in dates)
        ScheduledReminder(
          id: _idOf(at),
          at: at,
          title: 'Bugün vadesi gelenler',
          body: _body(byDate[at]!),
        ),
    ];
  }

  ReminderKind? _kindOf(PlannedActivity item, Set<ReminderKind> selected) {
    for (final kind in ReminderKind.values) {
      if (selected.contains(kind) && kind.matches(item)) return kind;
    }
    return null;
  }

  /// `2 taksit, 1 kart ekstresi` — sıra enum sırasıdır, geldiği sıra değil;
  /// aynı gün iki kez planlandığında metin oynamamalı.
  String _body(List<ReminderKind> kinds) {
    final counts = <ReminderKind, int>{};
    for (final kind in kinds) {
      counts[kind] = (counts[kind] ?? 0) + 1;
    }
    return [
      for (final kind in ReminderKind.values)
        if (counts.containsKey(kind)) '${counts[kind]} ${kind.countedNoun}',
    ].join(', ');
  }

  /// `2026-09-01` → `20260901`. Kimlik günden türediği için yeniden planlama
  /// aynı günün bildirimini çoğaltmaz.
  int _idOf(DateTime at) => at.year * 10000 + at.month * 100 + at.day;

  DateTime? _parseDate(String value) {
    // Sunucu `yyyy-MM-dd` gönderiyor; bozuk bir satır bütün hatırlatmayı
    // düşürmesin diye ayrıştırma hatası sessizce o satırı eler.
    final parsed = DateTime.tryParse(value);
    if (parsed == null) return null;
    return DateTime(parsed.year, parsed.month, parsed.day);
  }
}
