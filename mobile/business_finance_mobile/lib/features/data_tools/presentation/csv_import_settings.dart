import 'dart:convert';

enum CsvImportPreset { commonTurkishBank, commonEnglishBank, custom }

class CsvImportSettings {
  const CsvImportSettings({
    required this.dateColumn,
    required this.amountColumn,
    required this.descriptionColumn,
    required this.referenceColumn,
    required this.encoding,
    required this.dateFormat,
    required this.decimalSeparator,
  });

  factory CsvImportSettings.forPreset(CsvImportPreset preset) =>
      switch (preset) {
        CsvImportPreset.commonEnglishBank => const CsvImportSettings(
          dateColumn: 'date',
          amountColumn: 'amount',
          descriptionColumn: 'description',
          referenceColumn: '',
          encoding: 'utf-8',
          dateFormat: 'yyyy-MM-dd',
          decimalSeparator: '.',
        ),
        CsvImportPreset.commonTurkishBank => const CsvImportSettings(
          dateColumn: 'Tarih',
          amountColumn: 'Tutar',
          descriptionColumn: 'Açıklama',
          referenceColumn: '',
          encoding: 'utf-8',
          dateFormat: 'dd.MM.yyyy',
          decimalSeparator: ',',
        ),
        CsvImportPreset.custom => const CsvImportSettings(
          dateColumn: '',
          amountColumn: '',
          descriptionColumn: '',
          referenceColumn: '',
          encoding: 'utf-8',
          dateFormat: 'yyyy-MM-dd',
          decimalSeparator: '.',
        ),
      };

  final String dateColumn;
  final String amountColumn;
  final String descriptionColumn;
  final String referenceColumn;
  final String encoding;
  final String dateFormat;
  final String decimalSeparator;

  static bool isApplicationTransactionExport(List<int> bytes) {
    try {
      final firstLine = const LineSplitter().convert(utf8.decode(bytes)).first;
      return firstLine.trimLeft().replaceFirst('\ufeff', '') ==
          'id,transactionDate,type,amount,currency,accountId,accountName,'
              'categoryId,categoryName,description,isCancelled,cancelledAtUtc';
    } on Object {
      return false;
    }
  }

  Map<String, String> toFields() => {
    'dateColumn': dateColumn.trim(),
    'amountColumn': amountColumn.trim(),
    if (descriptionColumn.trim().isNotEmpty)
      'descriptionColumn': descriptionColumn.trim(),
    if (referenceColumn.trim().isNotEmpty)
      'referenceColumn': referenceColumn.trim(),
    'encoding': encoding,
    'delimiter': 'auto',
    'dateFormat': dateFormat,
    'decimalSeparator': decimalSeparator,
    'currency': 'TRY',
  };
}
