import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/features/data_tools/presentation/csv_import_settings.dart';

void main() {
  test('common English bank preset maps date and amount headers', () {
    final fields = CsvImportSettings.forPreset(
      CsvImportPreset.commonEnglishBank,
    ).toFields();

    expect(fields['dateColumn'], 'date');
    expect(fields['amountColumn'], 'amount');
    expect(fields['descriptionColumn'], 'description');
    expect(fields['dateFormat'], 'yyyy-MM-dd');
    expect(fields['decimalSeparator'], '.');
  });

  test('recognizes the application transaction export contract', () {
    final csv = utf8.encode(
      '\ufeffid,transactionDate,type,amount,currency,accountId,accountName,'
      'categoryId,categoryName,description,isCancelled,cancelledAtUtc\n'
      '1,2026-08-14,expense,25.0000,TRY,2,Cash,3,Food,Test,false,',
    );

    expect(CsvImportSettings.isApplicationTransactionExport(csv), isTrue);
    expect(
      CsvImportSettings.isApplicationTransactionExport(
        utf8.encode('Tarih;Tutar\n14.08.2026;-25,00'),
      ),
      isFalse,
    );
  });

  test('common Turkish bank preset keeps localized columns configurable', () {
    final fields = CsvImportSettings.forPreset(
      CsvImportPreset.commonTurkishBank,
    ).toFields();

    expect(fields['dateColumn'], 'Tarih');
    expect(fields['amountColumn'], 'Tutar');
    expect(fields['descriptionColumn'], 'Açıklama');
    expect(fields['dateFormat'], 'dd.MM.yyyy');
    expect(fields['decimalSeparator'], ',');
  });

  test('optional blank mappings are not sent to the API', () {
    final fields = CsvImportSettings.forPreset(
      CsvImportPreset.custom,
    ).toFields();

    expect(fields, isNot(contains('descriptionColumn')));
    expect(fields, isNot(contains('referenceColumn')));
  });
}
