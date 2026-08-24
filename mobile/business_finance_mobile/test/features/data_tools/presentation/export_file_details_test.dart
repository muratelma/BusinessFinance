import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/features/data_tools/presentation/export_file_details.dart';

void main() {
  test('CSV details show record count and bounded preview', () {
    final details = ExportFileDetails.fromBytes(
      fileName: 'transactions.csv',
      mimeType: 'text/csv',
      bytes: utf8.encode('date,amount\n2026-08-14,10.0000\n'),
      kind: ExportFileKind.transactionsCsv,
      downloadedAt: DateTime.utc(2026, 8, 14, 9, 30),
    );

    expect(details.summary, contains('1 işlem satırı'));
    expect(details.preview, contains('2026-08-14,10.0000'));
    expect(details.sizeLabel, endsWith('byte'));
  });

  // Cari defterin dosyası kendi cümlesini kurar: aynı "işlem satırı"
  // etiketini paylaşsalardı iki dosya ekranda birbirinden ayırt edilemezdi.
  test('counterparty ledger CSV details count ledger rows', () {
    final details = ExportFileDetails.fromBytes(
      fileName: 'counterparty-ledger.csv',
      mimeType: 'text/csv',
      bytes: utf8.encode(
        'id,date,kind,amount\n1,2026-08-14,charge,10.0000\n'
        '2,2026-08-15,payment,4.0000\n',
      ),
      kind: ExportFileKind.counterpartyLedgerCsv,
    );

    expect(details.summary, contains('2 cari hareket satırı'));
    expect(details.preview, contains('charge'));
  });

  test('financial JSON details summarize top-level collections', () {
    final details = ExportFileDetails.fromBytes(
      fileName: 'financial-data.json',
      mimeType: 'application/json',
      bytes: utf8.encode(
        jsonEncode({
          'accounts': [
            {'id': 'account-1'},
          ],
          'transactions': [
            {'id': 'transaction-1'},
            {'id': 'transaction-2'},
          ],
        }),
      ),
      kind: ExportFileKind.financialJson,
    );

    expect(details.summary, containsAll(['accounts: 1', 'transactions: 2']));
    expect(details.preview, contains('transaction-2'));
  });

  test('backup details expose metadata without payload preview', () {
    final payload = utf8.encode(
      jsonEncode({
        'accounts': [
          {'id': 'account-1'},
        ],
        'transactions': [
          {'id': 'transaction-1'},
        ],
      }),
    );
    final details = ExportFileDetails.fromBytes(
      fileName: 'business-finance.bfbackup.json',
      mimeType: 'application/vnd.business-finance.backup+json',
      bytes: utf8.encode(
        jsonEncode({
          'schemaVersion': 2,
          'createdAtUtc': '2026-08-14T09:30:00Z',
          'payload': base64Encode(payload),
        }),
      ),
      kind: ExportFileKind.backup,
    );

    expect(details.summary, containsAll(['Yedek sürümü: 2', '2 kayıt']));
    expect(details.preview, isNull);
  });
}
