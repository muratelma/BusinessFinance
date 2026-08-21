import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';

class DebtInstallmentItem {
  const DebtInstallmentItem(
    this.sequence,
    this.amount,
    this.dueDate,
    this.status,
  );
  factory DebtInstallmentItem.fromJson(Map<String, dynamic> json) =>
      DebtInstallmentItem(
        JsonReaders.integer(json, 'sequence'),
        JsonReaders.string(json, 'amount'),
        JsonReaders.string(json, 'dueDate'),
        JsonReaders.string(json, 'status'),
      );
  final int sequence;
  final String amount;
  final String dueDate;
  final String status;
}

class DebtItem {
  const DebtItem(
    this.id,
    this.name,
    this.direction,
    this.remaining,
    this.sourceType,
    this.annualInterestRate,
    this.totalInterest,
    this.installments,
  );
  factory DebtItem.fromJson(Map<String, dynamic> json) => DebtItem(
    JsonReaders.string(json, 'id'),
    JsonReaders.string(json, 'counterpartyName'),
    JsonReaders.string(json, 'direction'),
    JsonReaders.string(json, 'remainingAmount'),
    JsonReaders.string(json, 'sourceType'),
    JsonReaders.string(json, 'annualInterestRate'),
    JsonReaders.string(json, 'totalInterest'),
    JsonReaders.list(json, 'installments')
        .map(
          (item) => DebtInstallmentItem.fromJson(
            JsonReaders.object(item, 'installment'),
          ),
        )
        .toList(growable: false),
  );
  final String id;
  final String name;
  final String direction;
  final String remaining;

  /// `cash`, `expense`, `income` ya da `unrecorded`.
  final String sourceType;
  final String annualInterestRate;
  final String totalInterest;
  final List<DebtInstallmentItem> installments;

  bool get isReceivable => direction == 'receivable';

  /// Bu ayrımdan önce açılmış, açılışında ne olduğu bilinmeyen borç.
  ///
  /// Uygulama bunu gizlemez: eksik görünür kalır ve gerçeği bilen kullanıcı
  /// tamamlar. Tamamlanana kadar borç ne açılış hareketi ne de gider üretir.
  bool get hasUnrecordedOpening => sourceType == 'unrecorded';
}

/// Borç ekranının tek okumada ihtiyaç duyduğu her şey.
///
/// Hesaplar ve kategoriler burada çünkü borç açarken kaynağı onlardan
/// seçiliyor. Ekran başka hiçbir şey çekmiyor — borç listesi açılırken CSV
/// satırları ya da belge listesi yüklenmesi için bir sebep yok.
class DebtsSnapshot {
  const DebtsSnapshot({
    required this.debts,
    required this.accounts,
    required this.categories,
  });

  final List<DebtItem> debts;
  final List<DataChoice> accounts;
  final List<DataChoice> categories;
}
