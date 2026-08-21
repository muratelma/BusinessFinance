import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';

class DataTransaction {
  const DataTransaction(this.id, this.date, this.amount, this.description);
  factory DataTransaction.fromJson(Map<String, dynamic> json) =>
      DataTransaction(
        JsonReaders.string(json, 'id'),
        JsonReaders.string(json, 'transactionDate'),
        JsonReaders.string(json, 'amount'),
        JsonReaders.nullableString(json, 'description'),
      );
  final String id;
  final String date;
  final String amount;
  final String? description;
}

class ImportRowItem {
  const ImportRowItem({
    required this.id,
    required this.rowNumber,
    required this.status,
    this.date,
    this.amount,
    this.description,
    this.error,
    this.duplicateReason,
  });
  factory ImportRowItem.fromJson(Map<String, dynamic> json) => ImportRowItem(
    id: JsonReaders.string(json, 'id'),
    rowNumber: JsonReaders.integer(json, 'rowNumber'),
    status: JsonReaders.string(json, 'status'),
    date: JsonReaders.nullableString(json, 'transactionDate'),
    amount: JsonReaders.nullableString(json, 'signedAmount'),
    description: JsonReaders.nullableString(json, 'description'),
    error: JsonReaders.nullableString(json, 'errorMessage'),
    duplicateReason: JsonReaders.nullableString(json, 'duplicateReason'),
  );
  final String id;
  final int rowNumber;
  final String status;
  final String? date;
  final String? amount;
  final String? description;
  final String? error;
  final String? duplicateReason;
}

class ImportBatchItem {
  const ImportBatchItem(this.id, this.fileName, this.status, this.rows);
  factory ImportBatchItem.fromJson(Map<String, dynamic> json) =>
      ImportBatchItem(
        JsonReaders.string(json, 'id'),
        JsonReaders.string(json, 'fileName'),
        JsonReaders.string(json, 'status'),
        JsonReaders.list(json, 'rows')
            .map(
              (item) => ImportRowItem.fromJson(JsonReaders.object(item, 'row')),
            )
            .toList(growable: false),
      );
  final String id;
  final String fileName;
  final String status;
  final List<ImportRowItem> rows;
}

class AttachmentItem {
  const AttachmentItem(
    this.id,
    this.fileName,
    this.contentType,
    this.sizeBytes,
  );
  factory AttachmentItem.fromJson(Map<String, dynamic> json) => AttachmentItem(
    JsonReaders.string(json, 'id'),
    JsonReaders.string(json, 'fileName'),
    JsonReaders.string(json, 'contentType'),
    JsonReaders.integer(json, 'sizeBytes'),
  );
  final String id;
  final String fileName;
  final String contentType;
  final int sizeBytes;
}

/// Veri Araçları'nın ihtiyaç duyduğu her şey.
///
/// Borç ve hedef listeleri buradan çıktı: ikisi de kendi ekranına taşındı ve
/// burada durdukları sürece CSV sekmesi açılırken borç listesi de çekiliyordu.
/// Kalan üçü gerçekten gerekli — CSV satırı bir hesap ve kategoriye eşleniyor,
/// belge bir işleme bağlanıyor.
class DataToolsSnapshot {
  const DataToolsSnapshot({
    required this.accounts,
    required this.categories,
    required this.transactions,
  });
  final List<DataChoice> accounts;
  final List<DataChoice> categories;
  final List<DataTransaction> transactions;
}
