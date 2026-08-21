import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import 'data_tools_models.dart';

abstract interface class DataToolsRepositoryContract {
  Future<DataToolsSnapshot> load(String asOfDate);
  Future<ImportBatchItem> stageCsv(
    ApiUpload upload,
    Map<String, String> fields,
  );
  Future<ImportBatchItem> getImport(String id);
  Future<void> mapImportRow(
    String batchId,
    String rowId,
    Map<String, Object?> input,
  );
  Future<void> resolveDuplicate(String batchId, String rowId, String decision);
  Future<ImportBatchItem> confirmImport(String batchId, List<String> rowIds);
  Future<List<AttachmentItem>> listAttachments(String transactionId);
  Future<void> uploadAttachment(String transactionId, ApiUpload upload);
  Future<ApiBinaryResponse> downloadAttachment(String attachmentId);
  Future<ApiBinaryResponse> downloadExport(String path);
  Future<Map<String, dynamic>> validateBackup(ApiUpload upload);
  Future<Map<String, dynamic>> restoreBackup(ApiUpload upload);
}

class DataToolsRepository implements DataToolsRepositoryContract {
  const DataToolsRepository(this._client);
  final ApiClient _client;

  @override
  Future<DataToolsSnapshot> load(String asOfDate) async {
    final responses = await Future.wait([
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
      _client.get('/api/v1/categories?isActive=true'),
      _client.get('/api/v1/transactions?pageNumber=1&pageSize=100'),
    ]);
    return DataToolsSnapshot(
      accounts: _items(
        responses[0].requireObject(),
      ).map(DataChoice.fromJson).toList(),
      categories: _items(
        responses[1].requireObject(),
      ).map(DataChoice.categoryFromJson).toList(),
      transactions: _items(
        responses[2].requireObject(),
      ).map(DataTransaction.fromJson).toList(),
    );
  }

  @override
  Future<ImportBatchItem> stageCsv(
    ApiUpload upload,
    Map<String, String> fields,
  ) async => ImportBatchItem.fromJson(
    (await _client.postMultipart(
      '/api/v1/imports/csv/stage',
      upload: upload,
      fields: fields,
    )).requireObject(),
  );
  @override
  Future<ImportBatchItem> getImport(String id) async =>
      ImportBatchItem.fromJson(
        (await _client.get('/api/v1/imports/$id')).requireObject(),
      );
  @override
  Future<void> mapImportRow(
    String batchId,
    String rowId,
    Map<String, Object?> input,
  ) async => _client.patch('/api/v1/imports/$batchId/rows/$rowId', body: input);
  @override
  Future<void> resolveDuplicate(
    String batchId,
    String rowId,
    String decision,
  ) async => _client.patch(
    '/api/v1/imports/$batchId/rows/$rowId/duplicate-decision',
    body: {'decision': decision},
  );
  @override
  Future<ImportBatchItem> confirmImport(
    String batchId,
    List<String> rowIds,
  ) async => ImportBatchItem.fromJson(
    (await _client.post(
      '/api/v1/imports/$batchId/confirm',
      body: {'rowIds': rowIds},
    )).requireObject(),
  );
  @override
  Future<List<AttachmentItem>> listAttachments(String transactionId) async =>
      _items(
        (await _client.get(
          '/api/v1/transactions/$transactionId/attachments',
        )).requireObject(),
      ).map(AttachmentItem.fromJson).toList();
  @override
  Future<void> uploadAttachment(String transactionId, ApiUpload upload) async =>
      _client.postMultipart(
        '/api/v1/transactions/$transactionId/attachments',
        upload: upload,
      );
  @override
  Future<ApiBinaryResponse> downloadAttachment(String attachmentId) =>
      _client.getBytes('/api/v1/attachments/$attachmentId/content');
  @override
  Future<ApiBinaryResponse> downloadExport(String path) =>
      _client.getBytes(path);
  @override
  Future<Map<String, dynamic>> validateBackup(ApiUpload upload) async =>
      (await _client.postMultipart(
        '/api/v1/backups/validate',
        upload: upload,
      )).requireObject();
  @override
  Future<Map<String, dynamic>> restoreBackup(ApiUpload upload) async =>
      (await _client.postMultipart(
        '/api/v1/backups/restore',
        upload: upload,
      )).requireObject();

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
