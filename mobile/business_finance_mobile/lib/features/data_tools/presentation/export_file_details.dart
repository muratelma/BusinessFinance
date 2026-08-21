import 'dart:convert';
import 'dart:typed_data';

enum ExportFileKind { transactionsCsv, financialJson, backup }

class ExportFileDetails {
  const ExportFileDetails({
    required this.fileName,
    required this.mimeType,
    required this.bytes,
    required this.createdAt,
    required this.summary,
    this.preview,
  });

  factory ExportFileDetails.fromBytes({
    required String fileName,
    required String mimeType,
    required List<int> bytes,
    required ExportFileKind kind,
    DateTime? downloadedAt,
  }) {
    final data = Uint8List.fromList(bytes);
    final fallbackTime = downloadedAt ?? DateTime.now();
    return switch (kind) {
      ExportFileKind.transactionsCsv => _csv(
        fileName,
        mimeType,
        data,
        fallbackTime,
      ),
      ExportFileKind.financialJson => _json(
        fileName,
        mimeType,
        data,
        fallbackTime,
      ),
      ExportFileKind.backup => _backup(fileName, mimeType, data, fallbackTime),
    };
  }

  final String fileName;
  final String mimeType;
  final Uint8List bytes;
  final DateTime createdAt;
  final List<String> summary;
  final String? preview;

  String get sizeLabel {
    if (bytes.length < 1024) return '${bytes.length} byte';
    if (bytes.length < 1024 * 1024) {
      return '${(bytes.length / 1024).toStringAsFixed(1)} KiB';
    }
    return '${(bytes.length / (1024 * 1024)).toStringAsFixed(1)} MiB';
  }

  String get createdAtLabel {
    final local = createdAt.toLocal();
    String two(int value) => value.toString().padLeft(2, '0');
    return '${two(local.day)}.${two(local.month)}.${local.year} '
        '${two(local.hour)}:${two(local.minute)}';
  }

  static ExportFileDetails _csv(
    String name,
    String type,
    Uint8List bytes,
    DateTime createdAt,
  ) {
    final text = utf8.decode(bytes).replaceFirst('\ufeff', '');
    final lines = const LineSplitter()
        .convert(text)
        .where((line) => line.trim().isNotEmpty)
        .toList(growable: false);
    final recordCount = lines.isEmpty ? 0 : lines.length - 1;
    final previewLines = lines.take(6).toList();
    if (lines.length > previewLines.length) previewLines.add('…');
    return ExportFileDetails(
      fileName: name,
      mimeType: type,
      bytes: bytes,
      createdAt: createdAt,
      summary: ['$recordCount işlem satırı'],
      preview: previewLines.join('\n'),
    );
  }

  static ExportFileDetails _json(
    String name,
    String type,
    Uint8List bytes,
    DateTime createdAt,
  ) {
    final decoded = jsonDecode(utf8.decode(bytes));
    final summary = _collectionSummary(decoded);
    final formatted = const JsonEncoder.withIndent('  ').convert(decoded);
    final preview = formatted.length <= 2000
        ? formatted
        : '${formatted.substring(0, 2000)}\n…';
    return ExportFileDetails(
      fileName: name,
      mimeType: type,
      bytes: bytes,
      createdAt: createdAt,
      summary: summary,
      preview: preview,
    );
  }

  static ExportFileDetails _backup(
    String name,
    String type,
    Uint8List bytes,
    DateTime downloadedAt,
  ) {
    final envelope = jsonDecode(utf8.decode(bytes)) as Map<String, dynamic>;
    final payload = envelope['payload'] as String?;
    final snapshot = payload == null
        ? null
        : jsonDecode(utf8.decode(base64Decode(payload)));
    final count = _countTopLevelRecords(snapshot);
    final createdAt = DateTime.tryParse(
      envelope['createdAtUtc'] as String? ?? '',
    );
    return ExportFileDetails(
      fileName: name,
      mimeType: type,
      bytes: bytes,
      createdAt: createdAt ?? downloadedAt,
      summary: [
        'Yedek sürümü: ${envelope['schemaVersion'] ?? 'Bilinmiyor'}',
        '$count kayıt',
      ],
    );
  }

  static List<String> _collectionSummary(Object? value) {
    if (value is! Map<String, dynamic>) return const ['1 JSON kayıt kökü'];
    final result = <String>[];
    for (final entry in value.entries) {
      if (entry.value is List) {
        result.add('${entry.key}: ${(entry.value as List).length}');
      }
    }
    return result.isEmpty ? const ['JSON veri dosyası'] : result;
  }

  static int _countTopLevelRecords(Object? value) {
    if (value is! Map<String, dynamic>) return 0;
    return value.values.whereType<List<Object?>>().fold(
      0,
      (total, items) => total + items.length,
    );
  }
}
