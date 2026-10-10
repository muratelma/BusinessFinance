import '../data/day_close_repository.dart';

/// Gün sonu panelinde kullanıcının verdiği cevaplar: hangi kayıt yazılan
/// tutarın içinde ve bir grubun satışı ile tahsilatında ortak olan tutar.
///
/// Panel hiçbir tutarı hesaplamaz; bu sınıf yalnız **ne söylendiğini** tutar
/// ve sunucuya gidecek girdiyi besler. İki güvence buradadır:
///
/// - Bir grubun seçimi değişince o grubun eski ortak tutar cevabı kullanılmaz;
///   satış ve tahsilat toplamları değişmiştir, soru yeniden sorulur.
/// - Liste yenilenince listede duran kayıtların cevabı silinmez; yalnız artık
///   listede olmayan kaydın cevabı (ve onun grubunun ortak tutarı) düşer.
class DayCloseAnswers {
  final _records = <String, bool>{};

  /// Cevabı tutulan kaydın grubu; kayıt listeden düşerse hangi grubun ortak
  /// tutarının eskidiği buradan bilinir.
  final _groupOf = <String, String>{};
  final _overlaps = <String, String>{};

  /// Kaydın anahtarı → yazılan tutarın içinde mi. Varsayılanıyla aynı olan
  /// hazır işaret burada yer almaz; cevap isteyen kaydın cevabı hep yer alır.
  Map<String, bool> get recordOverrides => Map.unmodifiable(_records);

  /// Grubun kimliği → ikisinde de görünen tutar (dört ondalıklı). Ortak tutar
  /// yalnız nakit tutarı yazılmışken sorulur; panel nakit boşken göndermez.
  Map<String, String> get overlaps => Map.unmodifiable(_overlaps);

  /// Kaydın bu andaki durumu: kullanıcının cevabı, yoksa önizlemedeki değer.
  /// `null`: cevap bekliyor.
  bool? included(DayCloseExistingRecord record) =>
      _records[record.key] ?? record.included;

  /// Kaydın işaretini açıkça verir.
  void setIncluded(DayCloseExistingRecord record, bool value) {
    final groupId = record.groupId;
    if (!record.requiresAnswer && value == record.includedByDefault) {
      _records.remove(record.key);
      _groupOf.remove(record.key);
    } else {
      _records[record.key] = value;
      if (groupId != null) _groupOf[record.key] = groupId;
    }
    if (groupId != null) _overlaps.remove(groupId);
  }

  /// Satıra dokunuş: cevap bekleyen kayıt "içinde" olur, sonra "değil" ile
  /// "içinde" arasında gider. Cevapsız hâle geri dönülmez.
  void toggle(DayCloseExistingRecord record) =>
      setIncluded(record, !(included(record) ?? false));

  /// Toplu cevap: verilen kayıtların hepsi içinde ya da hiçbiri.
  void setAll(Iterable<DayCloseExistingRecord> records, bool value) {
    for (final record in records) {
      setIncluded(record, value);
    }
  }

  /// Toplu cevabın bu andaki hâli: hepsi içindeyse `true`, hiçbiri değilse
  /// `false`; karışıksa, cevap bekleyen varsa ya da kayıt yoksa `null`.
  bool? allIncluded(Iterable<DayCloseExistingRecord> records) {
    bool? common;
    for (final record in records) {
      final value = included(record);
      if (value == null || (common != null && value != common)) return null;
      common = value;
    }
    return common;
  }

  /// Grubun ortak tutarını verir; [amount] dört ondalıklı tutardır.
  void setOverlap(String groupId, String amount) => _overlaps[groupId] = amount;

  void clearOverlap(String groupId) => _overlaps.remove(groupId);

  /// Yeni önizlemeyle hizalar: listede artık olmayan kaydın cevabını ve artık
  /// sorulmayan grubun ortak tutarını bırakır. Bir cevap düştüyse `true`
  /// döner; sunucu eski cevabı taşıyan girdiyi reddettiği için önizleme
  /// yeniden istenir.
  bool reconcile(DayClosePreview preview) {
    final listed = {for (final record in preview.existingRecords) record.key};
    final gone = [
      for (final key in _records.keys)
        if (!listed.contains(key)) key,
    ];
    var changed = gone.isNotEmpty;
    for (final key in gone) {
      _records.remove(key);
      // Kaydı düşen grubun toplamları değişti: ortak tutarı yeniden sorulur.
      _overlaps.remove(_groupOf.remove(key));
    }
    // Nakit yazılmadıysa hiçbir grup sorulmaz; cevaplar nakit yeniden
    // yazılana kadar bekler.
    if (preview.cash.stated) {
      final asked = {for (final group in preview.overlapGroups) group.groupId};
      final before = _overlaps.length;
      _overlaps.removeWhere((groupId, _) => !asked.contains(groupId));
      changed = changed || _overlaps.length != before;
    }
    return changed;
  }

  /// Başka günün kayıtları başka kayıtlardır.
  void clear() {
    _records.clear();
    _groupOf.clear();
    _overlaps.clear();
  }
}
