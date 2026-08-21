import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';

class GoalItem {
  const GoalItem(
    this.id,
    this.name,
    this.target,
    this.allocated,
    this.remaining,
    this.progress,
    this.mode,
    this.status, {
    // Adlandırılmış: sekiz pozisyonel argümanın sonuna dokuzuncuyu eklemek
    // her çağrı yerini bozuyor ve sıra hatasını derleyiciden gizliyordu.
    this.accountId,
  });
  factory GoalItem.fromJson(Map<String, dynamic> json) => GoalItem(
    JsonReaders.string(json, 'id'),
    JsonReaders.string(json, 'name'),
    JsonReaders.string(json, 'targetAmount'),
    JsonReaders.string(json, 'allocatedAmount'),
    JsonReaders.string(json, 'remainingAmount'),
    JsonReaders.string(json, 'progressPercentage'),
    JsonReaders.string(json, 'trackingMode'),
    JsonReaders.string(json, 'status'),
    accountId: JsonReaders.nullableString(json, 'accountId'),
  );
  final String id;
  final String name;
  final String target;
  final String allocated;
  final String remaining;
  final String progress;
  final String mode;
  final String status;

  /// Bakiyesi izlenen hesap; manuel hedefte `null`.
  final String? accountId;

  /// İlerlemesi elle eklenen katkılardan gelen hedef.
  ///
  /// Bu moddaki katkı **para hareketi değildir**: hiçbir hesaptan para
  /// çıkmaz, yalnız "şu kadarını ayırdım" notu tutulur. Ekranın bunu açıkça
  /// söylemesi gerekiyor — aksi hâlde kullanıcı parasının gerçekten
  /// taşındığını sanıyor.
  bool get isManual => mode == 'manual-contributions';
}

/// Hedef ekranının tek okumada ihtiyaç duyduğu her şey.
///
/// Hesaplar burada çünkü bakiyeye bağlı hedef kurarken hesap seçiliyor.
class GoalsSnapshot {
  const GoalsSnapshot({required this.goals, required this.accounts});

  final List<GoalItem> goals;
  final List<DataChoice> accounts;
}
