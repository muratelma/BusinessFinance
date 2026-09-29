import 'package:flutter/foundation.dart';

import '../../../core/network/api_client.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/data_tools_models.dart';
import '../data/data_tools_repository.dart';

class ImportResultSummary {
  const ImportResultSummary({
    required this.imported,
    required this.skipped,
    required this.failed,
  });

  final int imported;
  final int skipped;
  final int failed;

  String get message =>
      '$imported satır içe aktarıldı, $skipped satır atlandı, '
      '$failed satır hatalı kaldı.';
}

class DataToolsController extends ChangeNotifier {
  static const int maximumAttachmentBytes = 5 * 1024 * 1024;

  DataToolsController(
    this._repository, {
    DateTime Function()? now,
    this.financialDataChanges,
  }) : _now = now ?? DateTime.now;

  final DataToolsRepositoryContract _repository;

  /// Confirming an import, paying a debt and restoring a backup all create or
  /// replace real movements, so the screens showing them have to reread. Before
  /// this they stayed on numbers that no longer existed until a manual refresh.
  final FinancialDataChanges? financialDataChanges;
  final DateTime Function() _now;
  DataToolsSnapshot? snapshot;
  ImportBatchItem? importBatch;

  /// Kullanıcının içe aktarmadan çıkardığı hazır satırlar. Sunucuya
  /// gönderilmez: onay isteği yalnız seçilen satırları taşır, atlanan satır
  /// hiç gönderilmez (28 Eylül denetimi U8).
  Set<String> skippedRowIds = const {};

  bool isSkipped(ImportRowItem row) => skippedRowIds.contains(row.id);

  void toggleSkip(ImportRowItem row) {
    final next = {...skippedRowIds};
    if (!next.remove(row.id)) next.add(row.id);
    skippedRowIds = next;
    notifyListeners();
  }

  /// İçe aktarılacak satırlar: hazır olanlardan atlananlar çıkarılır.
  List<ImportRowItem> get rowsToImport => [
    for (final row in importBatch?.rows ?? const <ImportRowItem>[])
      if (row.status == 'ready' && !skippedRowIds.contains(row.id)) row,
  ];
  List<AttachmentItem> attachments = const [];
  String? selectedTransactionId;
  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;
  String? successMessage;
  ImportResultSummary? importResultSummary;

  String get today => _date(_now());
  bool get isStale => snapshot != null && errorMessage != null;

  Future<void> load() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      snapshot = await _repository.load(today);
      unauthorized = false;
      if (selectedTransactionId != null &&
          !snapshot!.transactions.any(
            (item) => item.id == selectedTransactionId,
          )) {
        selectedTransactionId = null;
        attachments = const [];
      }
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir veri araçları yanıtı alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> stageCsv(ApiUpload upload, Map<String, String> fields) =>
      _submit(
        () async {
          importResultSummary = null;
          skippedRowIds = const {};
          importBatch = await _repository.stageCsv(upload, fields);
        },
        'CSV satırları önizlemeye alındı.',
        reload: false,
      );
  Future<bool> mapRow(ImportRowItem row, String accountId, String categoryId) =>
      _submit(
        () async {
          await _repository.mapImportRow(importBatch!.id, row.id, {
            'transactionDate': row.date,
            'signedAmount': row.amount,
            'description': row.description,
            'externalReference': null,
            'accountId': accountId,
            'categoryId': categoryId,
          });
          importBatch = await _repository.getImport(importBatch!.id);
        },
        'Satır eşlendi.',
        reload: false,
      );
  Future<bool> resolveDuplicate(ImportRowItem row, String decision) => _submit(
    () async {
      await _repository.resolveDuplicate(importBatch!.id, row.id, decision);
      importBatch = await _repository.getImport(importBatch!.id);
    },
    decision == 'skip' ? 'Satır atlandı.' : 'Satır içe alınmaya hazır.',
    reload: false,
  );
  Future<bool> confirmImport() =>
      _submit(onSuccess: (changes) => changes.importConfirmed(), () async {
        final ids = rowsToImport.map((row) => row.id).toList();
        importBatch = await _repository.confirmImport(importBatch!.id, ids);
        importResultSummary = ImportResultSummary(
          imported: importBatch!.rows
              .where((row) => row.status == 'imported')
              .length,
          skipped: importBatch!.rows
              .where((row) => row.status == 'skipped-duplicate')
              .length,
          failed: importBatch!.rows
              .where((row) => row.status == 'invalid')
              .length,
        );
      }, 'İçe aktarma tamamlandı.');
  Future<bool> selectTransaction(String? id) async {
    selectedTransactionId = id;
    attachments = const [];
    notifyListeners();
    if (id == null) return true;
    return _submit(
      () async => attachments = await _repository.listAttachments(id),
      'Belgeler yüklendi.',
      reload: false,
      showSuccess: false,
    );
  }

  Future<bool> uploadAttachment(ApiUpload upload) {
    if (upload.bytes.length > maximumAttachmentBytes) {
      successMessage = null;
      errorMessage = 'Belge 5 MiB boyut sınırını aşıyor.';
      notifyListeners();
      return Future.value(false);
    }
    return _submit(
      () async {
        await _repository.uploadAttachment(selectedTransactionId!, upload);
        attachments = await _repository.listAttachments(selectedTransactionId!);
      },
      'Belge güvenle yüklendi.',
      reload: false,
    );
  }

  Future<ApiBinaryResponse?> downloadAttachment(String id) =>
      _download(() => _repository.downloadAttachment(id));
  Future<ApiBinaryResponse?> downloadExport(String path) =>
      _download(() => _repository.downloadExport(path));
  Future<Map<String, dynamic>?> validateBackup(ApiUpload upload) async {
    Map<String, dynamic>? result;
    final success = await _submit(
      () async => result = await _repository.validateBackup(upload),
      'Yedek doğrulandı.',
      reload: false,
    );
    return success ? result : null;
  }

  Future<bool> restoreBackup(ApiUpload upload) => _submit(
    // A restore replaces everything the user has, so every screen is stale.
    onSuccess: (changes) => changes.restoreCompleted(),
    () => _repository.restoreBackup(upload),
    'Yedek geri yüklendi.',
  );

  Future<ApiBinaryResponse?> _download(
    Future<ApiBinaryResponse> Function() action,
  ) async {
    if (isSubmitting) return null;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      return await action();
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return null;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir dosya yanıtı alındı.';
      return null;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  Future<bool> _submit(
    Future<void> Function() action,
    String success, {
    bool reload = true,
    bool showSuccess = true,
    void Function(FinancialDataChanges changes)? onSuccess,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await action();
      final changes = financialDataChanges;
      if (changes != null && onSuccess != null) onSuccess(changes);
      if (reload) await load();
      if (showSuccess) successMessage = success;
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }

  void clearMessage() {
    errorMessage = null;
    successMessage = null;
    importResultSummary = null;
    notifyListeners();
  }

  static String _date(DateTime value) =>
      '${value.year.toString().padLeft(4, '0')}-${value.month.toString().padLeft(2, '0')}-${value.day.toString().padLeft(2, '0')}';
}
