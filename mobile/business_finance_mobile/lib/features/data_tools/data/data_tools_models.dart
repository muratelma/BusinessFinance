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

  /// Açıklaması parayı yalnız **taşıyan** bir işleme benziyor mu: POS yatışı,
  /// kredi kartı borcu ödemesi, kendi hesaplar arası aktarım.
  ///
  /// Bu satırlar içe aktarılırsa gelir/gider olarak ikinci kez sayılır
  /// (ADR 0014; 28 Eylül denetimi U8). Karar değil **ipucudur**: bankaların
  /// açıklama dili farklıdır, liste bilerek temkinli tutuldu ("EFT", "havale"
  /// gibi gerçek gelir/giderde de geçen kelimeler yok). Asıl çözüm içe
  /// aktarımın bu satırları tanımasıdır (`research/fikir-kaydi.md` F05).
  bool get looksLikeCarriedMoney {
    final text = description;
    if (text == null) return false;
    final normalized = _asciiUpper(text);
    return _carriedMoneyPattern.hasMatch(normalized);
  }
}

final _carriedMoneyPattern = RegExp(
  r'\bPOS\b|UYE ISYERI|KREDI KARTI|KART ODEME|KK ODEME|VIRMAN|'
  r'HESAPLAR ARASI|KENDI HESAB',
);

/// Türkçe harfleri ASCII büyük harfe indirir; `toUpperCase` tek başına `i`yi
/// `I` yapar ve "üye işyeri" ile "UYE ISYERI" eşleşmezdi.
String _asciiUpper(String value) {
  const map = {
    'ı': 'I',
    'i': 'I',
    'İ': 'I',
    'ş': 'S',
    'Ş': 'S',
    'ğ': 'G',
    'Ğ': 'G',
    'ü': 'U',
    'Ü': 'U',
    'ö': 'O',
    'Ö': 'O',
    'ç': 'C',
    'Ç': 'C',
  };
  final buffer = StringBuffer();
  for (final rune in value.runes) {
    final char = String.fromCharCode(rune);
    buffer.write(map[char] ?? char.toUpperCase());
  }
  return buffer.toString();
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
