import 'dart:typed_data';

import 'package:file_selector/file_selector.dart' as file_selector;
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:share_plus/share_plus.dart';

import '../../../core/files/device_file_saver.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/network/api_client.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/data_tools_models.dart';
import '../data/data_tools_repository.dart';
import 'csv_import_settings.dart';
import 'data_tools_controller.dart';
import 'export_file_details.dart';

class DataToolsPage extends StatefulWidget {
  const DataToolsPage({
    required this.repository,
    this.fileSaver = const AndroidDocumentFileSaver(),
    this.changes,
    super.key,
  });
  final DataToolsRepositoryContract repository;
  final DeviceFileSaver fileSaver;
  final FinancialDataChanges? changes;
  @override
  State<DataToolsPage> createState() => _DataToolsPageState();
}

class _DataToolsPageState extends State<DataToolsPage> {
  late final DataToolsController controller;
  @override
  void initState() {
    super.initState();
    controller = DataToolsController(
      widget.repository,
      financialDataChanges: widget.changes,
    )..addListener(_changed);
    controller.load();
  }

  void _changed() => mounted ? setState(() {}) : null;
  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => DefaultTabController(
    // Borç ve hedefler kendi ekranlarına taşındı; ikisi de finansal
    // kavramdı ve CSV/belge/yedek ile aynı çekmecede duruyordu. Kalan üç
    // sekme gerçekten aynı aileden: hepsi dosya işi.
    length: 3,
    child: Scaffold(
      appBar: AppBar(
        title: const Text('Veri araçları'),
        bottom: const TabBar(
          tabs: [
            Tab(text: 'CSV'),
            Tab(text: 'Belgeler'),
            Tab(text: 'Yedek'),
          ],
        ),
      ),
      body: _body(),
    ),
  );

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.snapshot == null) {
      return const AppLoadingView(message: 'Veri araçları yükleniyor');
    }
    if (controller.snapshot == null) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Veriler alınamadı.',
        onRetry: controller.load,
      );
    }
    return Column(
      children: [
        if (controller.isLoading || controller.isSubmitting)
          const LinearProgressIndicator(),
        if (controller.errorMessage != null ||
            controller.successMessage != null)
          MaterialBanner(
            content: Text(
              controller.errorMessage ??
                  controller.importResultSummary?.message ??
                  controller.successMessage!,
            ),
            leading: Icon(
              controller.errorMessage == null
                  ? Icons.check_circle_outline
                  : Icons.error_outline,
            ),
            actions: [
              if (controller.errorMessage == null &&
                  controller.importResultSummary != null)
                TextButton(
                  onPressed: () => context.go('/transactions'),
                  child: const Text('İşlemlerde görüntüle'),
                ),
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        Expanded(
          child: TabBarView(
            children: [_csvTab(), _attachmentsTab(), _backupTab()],
          ),
        ),
      ],
    );
  }

  Widget _tabList(List<Widget> children) => RefreshIndicator(
    onRefresh: controller.load,
    child: ListView(
      padding: const EdgeInsets.all(AppSpacing.medium),
      children: children,
    ),
  );

  Widget _csvTab() {
    final batch = controller.importBatch;
    return _tabList([
      FilledButton.icon(
        onPressed: controller.isSubmitting ? null : _pickCsv,
        icon: const Icon(Icons.upload_file),
        label: const Text('CSV seç ve önizle'),
      ),
      const SizedBox(height: AppSpacing.medium),
      if (batch == null)
        const SizedBox(
          height: 220,
          child: AppEmptyView(
            icon: Icons.table_view_outlined,
            title: 'CSV önizlemesi yok',
            message: 'Henüz önizlemeye alınmış CSV yok.',
          ),
        )
      else ...[
        Text(batch.fileName, style: Theme.of(context).textTheme.titleMedium),
        Text('İçe aktarma durumu: ${_importBatchStatusLabel(batch.status)}'),
        const SizedBox(height: AppSpacing.small),
        // Parayı yalnız taşıyan satırlar gelir/gider olarak ikinci kez
        // sayılır (ADR 0014; 28 Eylül denetimi U8).
        Card(
          child: Padding(
            padding: const EdgeInsets.all(AppSpacing.small),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(Icons.info_outline),
                const SizedBox(width: AppSpacing.small),
                Expanded(
                  child: Text(
                    'POS parasının hesaba geçişi, kredi kartı borcu ödemesi ve '
                    'kendi hesaplarınız arasındaki aktarımı atlayın. Bunlar '
                    'içe aktarılırsa gelir ya da gider olarak ikinci kez '
                    'sayılır; uygulamada zaten kendi kayıtlarıyla duruyorlar.',
                    style: Theme.of(context).textTheme.bodySmall,
                  ),
                ),
              ],
            ),
          ),
        ),
        for (final row in batch.rows) _importRowCard(row),
        FilledButton(
          onPressed:
              controller.rowsToImport.isNotEmpty && !controller.isSubmitting
              ? controller.confirmImport
              : null,
          child: const Text('Hazır satırları içe aktar'),
        ),
      ],
    ]);
  }

  Widget _importRowCard(ImportRowItem row) => Card(
    child: Padding(
      padding: const EdgeInsets.all(AppSpacing.small),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            'Satır ${row.rowNumber} • '
            '${controller.isSkipped(row) ? 'Atlanacak' : _importRowStatusLabel(row.status)}',
            style: Theme.of(context).textTheme.titleSmall,
          ),
          Text(
            '${row.date ?? 'Tarih yok'} • '
            '${row.amount == null ? 'Tutar yok' : MoneyText.format(row.amount!, 'TRY')}',
          ),
          if (row.description != null) Text(row.description!),
          if (row.error != null)
            Text(
              row.error!,
              style: TextStyle(color: Theme.of(context).colorScheme.error),
            ),
          if (row.looksLikeCarriedMoney &&
              (row.status == 'valid' || row.status == 'ready'))
            Text(
              'Bu satır POS yatışı, kart borcu ödemesi ya da kendi '
              'hesaplarınız arasında aktarım olabilir; öyleyse atlayın.',
              style: TextStyle(color: Theme.of(context).colorScheme.error),
            ),
          if (row.status == 'valid')
            TextButton(
              onPressed: () => _mapRow(row),
              child: const Text('Hesap ve kategori eşle'),
            ),
          if (row.status == 'ready')
            Align(
              alignment: Alignment.centerLeft,
              child: TextButton(
                onPressed: controller.isSubmitting
                    ? null
                    : () => controller.toggleSkip(row),
                child: Text(
                  controller.isSkipped(row) ? 'Atlamayı geri al' : 'Atla',
                ),
              ),
            ),
          if (row.status == 'pending-duplicate-review')
            Wrap(
              spacing: 8,
              children: [
                TextButton(
                  onPressed: () => controller.resolveDuplicate(row, 'skip'),
                  child: const Text('Atla'),
                ),
                FilledButton.tonal(
                  onPressed: () =>
                      controller.resolveDuplicate(row, 'import-anyway'),
                  child: const Text('Yine de içe al'),
                ),
              ],
            ),
        ],
      ),
    ),
  );

  Widget _attachmentsTab() {
    final transactions = controller.snapshot!.transactions;
    return _tabList([
      DropdownButtonFormField<String>(
        initialValue: controller.selectedTransactionId,
        // `isExpanded` olmadan buton en geniş öğesinin genişliğini almaya
        // çalışır; işlem açıklaması kullanıcının yazdığı serbest metin
        // olduğu için satır kolayca taşıyor ve `RenderFlex overflowed`
        // hatası veriyordu.
        isExpanded: true,
        decoration: const InputDecoration(labelText: 'İşlem'),
        items: [
          for (final item in transactions)
            DropdownMenuItem(
              value: item.id,
              child: Text(
                '${DateText.dayMonthYear(item.date)} • ${item.description ?? item.amount}',
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
        ],
        onChanged: controller.isSubmitting
            ? null
            : controller.selectTransaction,
      ),
      const SizedBox(height: AppSpacing.medium),
      FilledButton.icon(
        onPressed:
            controller.selectedTransactionId == null || controller.isSubmitting
            ? null
            : _pickAttachment,
        icon: const Icon(Icons.attach_file),
        label: const Text('Fiş / belge seç'),
      ),
      const SizedBox(height: AppSpacing.medium),
      if (controller.selectedTransactionId != null &&
          controller.attachments.isEmpty)
        const SizedBox(
          height: 180,
          child: AppEmptyView(
            icon: Icons.receipt_long_outlined,
            title: 'Belge yok',
            message: 'Bu işleme bağlı belge yok.',
          ),
        ),
      if (controller.attachments.isNotEmpty)
        AppCard(
          padding: EdgeInsets.zero,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              for (var i = 0; i < controller.attachments.length; i++) ...[
                if (i > 0) const Divider(height: 1, indent: AppSpacing.medium),
                AppListRow(
                  icon: Icons.description_outlined,
                  title: controller.attachments[i].fileName,
                  titleMaxLines: 1,
                  subtitle:
                      '${controller.attachments[i].contentType} • '
                      '${_readableSize(controller.attachments[i].sizeBytes)}',
                  trailing: IconButton(
                    tooltip: 'İndir ve paylaş',
                    icon: const Icon(Icons.share_outlined),
                    onPressed: () =>
                        _shareAttachment(controller.attachments[i]),
                  ),
                ),
              ],
            ],
          ),
        ),
    ]);
  }

  Widget _backupTab() => _tabList([
    const AppSectionHeader(title: 'Dışa aktar'),
    Text(
      'CSV ve JSON görüntüleme ve veri işleme içindir; yedek dosyası geri '
      'yükleme içindir.',
      style: Theme.of(context).textTheme.bodySmall,
    ),
    const SizedBox(height: AppSpacing.small),
    _exportCard(
      title: 'İşlem CSV dosyası',
      path: '/api/v1/exports/transactions.csv',
      name: 'transactions.csv',
      type: 'text/csv',
      kind: ExportFileKind.transactionsCsv,
      icon: Icons.table_view,
    ),
    // Cari defteri kendi dosyasını alır: işlem CSV'si `BudgetTransaction`
    // dökümüdür ve cari hareket orada hiç bulunmaz. Ona bir karşı taraf kolonu
    // eklemek her satırda boş kalırdı; iki dosyanın her biri tek kaydın
    // dökümü olduğu sürece kullanıcı ne okuduğunu bilir.
    _exportCard(
      title: 'Cari hareket CSV dosyası',
      path: '/api/v1/exports/counterparty-ledger.csv',
      name: 'counterparty-ledger.csv',
      type: 'text/csv',
      kind: ExportFileKind.counterpartyLedgerCsv,
      icon: Icons.people_alt_outlined,
    ),
    _exportCard(
      title: 'Finans verisi JSON',
      path: '/api/v1/exports/financial-data.json',
      name: 'financial-data.json',
      type: 'application/json',
      kind: ExportFileKind.financialJson,
      icon: Icons.data_object,
    ),
    _exportCard(
      title: 'Uygulama yedeği',
      path: '/api/v1/backups/download',
      name: 'business-finance.bfbackup.json',
      type: 'application/vnd.business-finance.backup+json',
      kind: ExportFileKind.backup,
      icon: Icons.backup_outlined,
    ),
    const Divider(height: AppSpacing.xLarge),
    const AppSectionHeader(title: 'Geri yükle'),
    Text(
      'Yalnızca finansal verisi olmayan bir hesaba geri yüklenir. Seçilen '
      'dosyanın önce bütünlüğü ve veri ilişkileri doğrulanır.',
      style: Theme.of(context).textTheme.bodySmall,
    ),
    const SizedBox(height: AppSpacing.small),
    FilledButton.icon(
      onPressed: controller.isSubmitting ? null : _pickBackup,
      icon: const Icon(Icons.restore),
      label: const Text('Yedeği doğrula ve geri yükle'),
    ),
  ]);

  Widget _exportCard({
    required String title,
    required String path,
    required String name,
    required String type,
    required ExportFileKind kind,
    required IconData icon,
  }) => Padding(
    padding: const EdgeInsets.only(bottom: AppSpacing.small),
    child: AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppListRow(icon: icon, title: title, subtitle: name),
          const SizedBox(height: AppSpacing.small),
          // İki eylem de çerçeveli: kenarlıksız `TextButton` yanındaki dolu
          // butonun yanında bir bağlantı gibi duruyor, dokunulabilir olduğu
          // anlaşılmıyordu. İkisini de dolu yapmak ise hiyerarşiyi silerdi —
          // kart başına birincil eylem tektir. Bu yüzden ikincil eylem
          // `OutlinedButton`: kendi şekli var, ağırlığı yok.
          _CardActions(
            secondary: OutlinedButton(
              onPressed: controller.isSubmitting
                  ? null
                  : () => _openExport(
                      path: path,
                      name: name,
                      type: type,
                      kind: kind,
                    ),
              style: _compactAction,
              child: const _ActionLabel('Önizle ve paylaş'),
            ),
            primary: FilledButton.tonal(
              onPressed: controller.isSubmitting
                  ? null
                  : () => _saveExport(path: path, name: name, type: type),
              style: _compactAction,
              child: const _ActionLabel('Cihaza kaydet'),
            ),
          ),
        ],
      ),
    ),
  );

  Future<void> _pickCsv() async {
    final file = await _pick(['csv']);
    if (file == null || !mounted) return;
    if (CsvImportSettings.isApplicationTransactionExport(file.bytes)) {
      await showDialog<void>(
        context: context,
        builder: (dialogContext) => AlertDialog(
          title: const Text('Bu CSV yeniden içe aktarılamaz'),
          content: const Text(
            'Uygulamanın işlem CSV dosyası görüntüleme ve tablo analizi '
            'içindir. Gelir/gider türü ayrı kolonda tutulduğu için banka '
            'ekstresi importer\'ına güvenle verilemez. Veriyi başka hesaba '
            'taşımak için Yedek oluştur ve Geri yükle akışını kullanın.',
          ),
          actions: [
            FilledButton(
              onPressed: () => Navigator.pop(dialogContext),
              child: const Text('Anladım'),
            ),
          ],
        ),
      );
      return;
    }
    final values = await AppFormSheet.show<CsvImportSettings>(
      context: context,
      builder: (_) => const _CsvSettingsForm(),
    );
    if (values == null) return;
    await controller.stageCsv(
      ApiUpload(bytes: file.bytes, fileName: file.name, mediaType: 'text/csv'),
      values.toFields(),
    );
  }

  Future<void> _mapRow(ImportRowItem row) async {
    final snapshot = controller.snapshot!;
    if (snapshot.accounts.isEmpty || snapshot.categories.isEmpty) return;
    var account = snapshot.accounts.first.id;
    final type = (double.tryParse(row.amount ?? '') ?? -1) > 0
        ? 'income'
        : 'expense';
    final matching = snapshot.categories
        .where((item) => item.type == type)
        .toList();
    if (matching.isEmpty) return;
    var category = matching.first.id;
    final accepted = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: Text('Satır ${row.rowNumber} eşle'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              DropdownButtonFormField(
                initialValue: account,
                decoration: const InputDecoration(labelText: 'Hesap'),
                items: snapshot.accounts
                    .map(
                      (item) => DropdownMenuItem(
                        value: item.id,
                        child: Text(item.name),
                      ),
                    )
                    .toList(),
                onChanged: (value) => setDialogState(() => account = value!),
              ),
              DropdownButtonFormField(
                initialValue: category,
                decoration: const InputDecoration(labelText: 'Kategori'),
                items: matching
                    .map(
                      (item) => DropdownMenuItem(
                        value: item.id,
                        child: Text(item.name),
                      ),
                    )
                    .toList(),
                onChanged: (value) => setDialogState(() => category = value!),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(dialogContext),
              child: const Text('Vazgeç'),
            ),
            FilledButton(
              onPressed: () => Navigator.pop(dialogContext, true),
              child: const Text('Eşle'),
            ),
          ],
        ),
      ),
    );
    if (accepted == true) await controller.mapRow(row, account, category);
  }

  Future<void> _pickAttachment() async {
    final file = await _pick(['pdf', 'jpg', 'jpeg', 'png']);
    if (file == null) return;
    final extension = file.extension.toLowerCase();
    final type = extension == 'pdf'
        ? 'application/pdf'
        : extension == 'png'
        ? 'image/png'
        : 'image/jpeg';
    await controller.uploadAttachment(
      ApiUpload(bytes: file.bytes, fileName: file.name, mediaType: type),
    );
  }

  Future<void> _shareAttachment(AttachmentItem item) async {
    final response = await controller.downloadAttachment(item.id);
    if (response == null) return;
    await _share(response, item.fileName, item.contentType);
  }

  Future<void> _openExport({
    required String path,
    required String name,
    required String type,
    required ExportFileKind kind,
  }) async {
    final response = await controller.downloadExport(path);
    if (response == null || !mounted) return;
    ExportFileDetails details;
    try {
      details = ExportFileDetails.fromBytes(
        fileName: name,
        mimeType: response.contentType ?? type,
        bytes: response.bytes,
        kind: kind,
      );
    } on Object {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Dosya önizlemesi oluşturulamadı.')),
      );
      return;
    }
    await showDialog<void>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: Text(details.fileName),
        content: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 560, maxHeight: 520),
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text('Tür: ${details.mimeType}'),
                Text('Boyut: ${details.sizeLabel}'),
                Text('Oluşturulma: ${details.createdAtLabel}'),
                for (final item in details.summary) Text(item),
                const SizedBox(height: AppSpacing.medium),
                if (details.preview case final preview?) ...[
                  const Text('Sınırlı içerik önizlemesi'),
                  const SizedBox(height: AppSpacing.small),
                  SelectableText(preview),
                ] else
                  const Text(
                    'Yedek payload içeriği güvenlik nedeniyle burada '
                    'gösterilmez.',
                  ),
              ],
            ),
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext),
            child: const Text('Kapat'),
          ),
          TextButton.icon(
            onPressed: () => _saveDetails(details),
            icon: const Icon(Icons.save_alt),
            label: const Text('Cihaza kaydet'),
          ),
          FilledButton.icon(
            onPressed: () => _shareDetails(details),
            icon: const Icon(Icons.share_outlined),
            label: const Text('Paylaş'),
          ),
        ],
      ),
    );
  }

  Future<void> _saveExport({
    required String path,
    required String name,
    required String type,
  }) async {
    final response = await controller.downloadExport(path);
    if (response == null) return;
    await _saveBytes(
      Uint8List.fromList(response.bytes),
      name,
      response.contentType ?? type,
    );
  }

  Future<void> _saveDetails(ExportFileDetails details) =>
      _saveBytes(details.bytes, details.fileName, details.mimeType);

  Future<void> _saveBytes(Uint8List bytes, String name, String type) async {
    try {
      final saved = await widget.fileSaver.save(
        bytes: bytes,
        fileName: name,
        mimeType: type,
      );
      if (!mounted || !saved) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('$name cihaza kaydedildi.')));
    } on Exception {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Dosya cihaza kaydedilemedi.')),
      );
    }
  }

  Future<void> _shareDetails(ExportFileDetails details) => _share(
    ApiBinaryResponse(bytes: details.bytes, contentType: details.mimeType),
    details.fileName,
    details.mimeType,
  );

  Future<void> _share(
    ApiBinaryResponse response,
    String name,
    String type,
  ) async {
    try {
      await SharePlus.instance.share(
        ShareParams(
          files: [
            XFile.fromData(
              Uint8List.fromList(response.bytes),
              mimeType: (response.contentType ?? type).split(';').first.trim(),
            ),
          ],
          fileNameOverrides: [name],
        ),
      );
    } on Exception {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Dosya paylaşımı başlatılamadı.')),
      );
    }
  }

  Future<void> _pickBackup() async {
    final file = await _pick(['json']);
    if (file == null) return;
    final upload = ApiUpload(
      bytes: file.bytes,
      fileName: file.name,
      mediaType: 'application/vnd.business-finance.backup+json',
    );
    final validation = await controller.validateBackup(upload);
    if (validation == null || !mounted) return;
    final accepted = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Yedek geri yüklensin mi?'),
        content: Text(
          'Yedek sürümü: ${validation['schemaVersion']} • ${validation['entityCount']} kayıt. '
          'Geri yükleme yalnızca finansal verisi olmayan bir hesapta yapılabilir.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext),
            child: const Text('Vazgeç'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(dialogContext, true),
            child: const Text('Geri yükle'),
          ),
        ],
      ),
    );
    if (accepted == true) await controller.restoreBackup(upload);
  }

  Future<({Uint8List bytes, String extension, String name})?> _pick(
    List<String> extensions,
  ) async {
    final file = await file_selector.openFile(
      acceptedTypeGroups: [
        file_selector.XTypeGroup(
          label: 'Desteklenen dosyalar',
          extensions: extensions,
        ),
      ],
    );
    if (file == null) return null;
    try {
      final bytes = await file.readAsBytes();
      final separator = file.name.lastIndexOf('.');
      final extension = separator < 0
          ? ''
          : file.name.substring(separator + 1).toLowerCase();
      return (bytes: bytes, extension: extension, name: file.name);
    } on Exception {
      if (!mounted) return null;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Dosya içeriği okunamadı.')));
      return null;
    }
  }

  static String _importBatchStatusLabel(String status) => switch (status) {
    'staged' => 'Ön izleme hazır',
    'imported' => 'İçe aktarıldı',
    'partially-imported' => 'Kısmen içe aktarıldı',
    _ => 'Bilinmeyen durum',
  };

  static String _importRowStatusLabel(String status) => switch (status) {
    'valid' => 'Geçerli',
    'invalid' => 'Geçersiz',
    'ready' => 'İçe aktarmaya hazır',
    'pending-duplicate-review' => 'Mükerrer kayıt kararı bekliyor',
    'skipped-duplicate' => 'Mükerrer olduğu için atlandı',
    'imported' => 'İçe aktarıldı',
    _ => 'Bilinmeyen durum',
  };
}

/// Kart içindeki iki eylem: satırı baştan sona, eşit paylaşarak doldururlar.
///
/// Önceki hâlde `Wrap(alignment: end)` kullanılıyordu; butonlar sağa
/// yaslanıyor, soldaki butonun başlangıcı satırın ortasından sonra kalıyordu.
/// `Expanded` ikisine de satırın yarısını verir, böylece kartın sol ve sağ
/// kenarıyla aralarındaki mesafe eşit olur.
///
/// Büyük yazı ölçeğinde yan yana durmak mümkün değil: etiketler iki katına
/// çıkınca yarım satır yetmez ve metin kırpılır. O kademede butonlar alt alta
/// ve tam genişlikte dizilir — kırpılmış bir etiket, alt alta iki butondan
/// daha kötüdür.
class _CardActions extends StatelessWidget {
  const _CardActions({required this.secondary, required this.primary});

  final Widget secondary;
  final Widget primary;

  /// Bu eşiğin üstünde iki buton bir satıra sığmıyor.
  ///
  /// Değer ölçümden geliyor: en uzun etiket (`Önizle ve paylaş`) 1,0x'te
  /// 113 dp, buton içi kullanılabilir alan ~150 dp. 1,2x'te 136 dp ile hâlâ
  /// sığıyor, 1,35x'te 152 dp ile taşıyor.
  static const _stackAboveScale = 1.2;

  @override
  Widget build(BuildContext context) {
    final scaled = MediaQuery.textScalerOf(context).scale(100) / 100;
    if (scaled > _stackAboveScale) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          secondary,
          const SizedBox(height: AppSpacing.small),
          primary,
        ],
      );
    }
    return Row(
      children: [
        Expanded(child: secondary),
        const SizedBox(width: AppSpacing.small),
        Expanded(child: primary),
      ],
    );
  }
}

/// Buton etiketi; sabit genişliğe oturduğu için esnek olmak zorunda.
///
/// `OutlinedButton.icon` etiketi esnek yapmaz; buton `Expanded` ile yarım
/// satıra oturunca uzun etiket satırı taşırırdı.
///
/// **İkon yok, çünkü ölçüm ikona yer bırakmıyor.** Yarım satır Pixel 8'de
/// ~166 dp; `Önizle ve paylaş` ikonuyla 163 dp'ye çıkıyor, yani 3 dp pay
/// kalıyor ve sistem yazı ölçeği %10 arttığı anda etiket kırpılıyor. İkonsuz
/// aynı etiket 137 dp — 29 dp pay. Kartın başlık satırında zaten türü
/// gösteren bir ikon var; eylem satırında ikinci bir ikon bilgi eklemiyordu,
/// yalnız etiketi kısaltmaya zorluyordu.
class _ActionLabel extends StatelessWidget {
  const _ActionLabel(this.label);

  final String label;

  @override
  Widget build(BuildContext context) =>
      Text(label, maxLines: 1, overflow: TextOverflow.ellipsis);
}

/// Kart içindeki ikili eylem satırı için daraltılmış buton dolgusu.
///
/// Buton `Expanded` ile yarım satıra oturduğu için dolgu **görünür bir boşluk
/// değil**, yalnız etikete kalan alanı belirleyen bir alt sınırdır. Dar
/// tutulması etikete 16 dp daha yer bırakır. Dokunma hedefi yüksekliği
/// (48 dp) temadan gelmeye devam ediyor.
final ButtonStyle _compactAction = ButtonStyle(
  padding: WidgetStatePropertyAll(
    EdgeInsets.symmetric(horizontal: AppSpacing.small),
  ),
);

/// `1048576 byte` kimseye bir şey söylemiyor.
String _readableSize(int bytes) {
  if (bytes < 1024) return '$bytes B';
  final kib = bytes / 1024;
  if (kib < 1024) return '${kib.toStringAsFixed(0)} KB';
  return '${(kib / 1024).toStringAsFixed(1)} MB';
}

/// CSV kolon eşlemesi. Controller'lar bu `State` ile birlikte ölür.
class _CsvSettingsForm extends StatefulWidget {
  const _CsvSettingsForm();

  @override
  State<_CsvSettingsForm> createState() => _CsvSettingsFormState();
}

class _CsvSettingsFormState extends State<_CsvSettingsForm> {
  var _preset = CsvImportPreset.commonTurkishBank;
  late var _settings = CsvImportSettings.forPreset(_preset);
  late final _date = TextEditingController(text: _settings.dateColumn);
  late final _amount = TextEditingController(text: _settings.amountColumn);
  late final _description = TextEditingController(
    text: _settings.descriptionColumn,
  );
  late final _reference = TextEditingController(
    text: _settings.referenceColumn,
  );
  late var _encoding = _settings.encoding;
  late var _dateFormat = _settings.dateFormat;
  late var _decimalSeparator = _settings.decimalSeparator;

  @override
  void dispose() {
    _date.dispose();
    _amount.dispose();
    _description.dispose();
    _reference.dispose();
    super.dispose();
  }

  bool get _isComplete =>
      _date.text.trim().isNotEmpty && _amount.text.trim().isNotEmpty;

  @override
  Widget build(BuildContext context) => AppFormSheet<CsvImportSettings>(
    title: 'CSV kolon eşlemesi',
    description:
        'Kolon adları CSV dosyasının ilk satırında nasıl yazıyorsa buraya '
        'aynı şekilde girilir.',
    submitLabel: 'Önizle',
    onSubmit: _isComplete
        ? () async => CsvImportSettings(
            dateColumn: _date.text,
            amountColumn: _amount.text,
            descriptionColumn: _description.text,
            referenceColumn: _reference.text,
            encoding: _encoding,
            dateFormat: _dateFormat,
            decimalSeparator: _decimalSeparator,
          )
        : null,
    children: [
      AppFormField(
        child: DropdownButtonFormField<CsvImportPreset>(
          initialValue: _preset,
          isExpanded: true,
          decoration: const InputDecoration(labelText: 'CSV biçimi'),
          items: const [
            DropdownMenuItem(
              value: CsvImportPreset.commonTurkishBank,
              child: Text('Türkçe banka — yaygın'),
            ),
            DropdownMenuItem(
              value: CsvImportPreset.commonEnglishBank,
              child: Text('İngilizce banka — yaygın'),
            ),
            DropdownMenuItem(
              value: CsvImportPreset.custom,
              child: Text('Özel'),
            ),
          ],
          onChanged: (value) {
            if (value == null) return;
            setState(() {
              _preset = value;
              _settings = CsvImportSettings.forPreset(value);
              _date.text = _settings.dateColumn;
              _amount.text = _settings.amountColumn;
              _description.text = _settings.descriptionColumn;
              _reference.text = _settings.referenceColumn;
              _encoding = _settings.encoding;
              _dateFormat = _settings.dateFormat;
              _decimalSeparator = _settings.decimalSeparator;
            });
          },
        ),
      ),
      AppFormField(
        child: TextField(
          controller: _date,
          // Gönderim butonu bu iki alanın dolu olmasına bağlı; her tuşta
          // yeniden değerlendirilmezse buton yanlış anda kapalı kalır.
          onChanged: (_) => setState(() {}),
          decoration: const InputDecoration(
            labelText: 'Tarih kolonu',
            hintText: 'transactionDate veya Tarih',
          ),
        ),
      ),
      AppFormField(
        child: TextField(
          controller: _amount,
          onChanged: (_) => setState(() {}),
          decoration: const InputDecoration(
            labelText: 'Tutar kolonu',
            hintText: 'amount veya Tutar',
          ),
        ),
      ),
      AppFormField(
        child: TextField(
          controller: _description,
          decoration: const InputDecoration(
            labelText: 'Açıklama kolonu (isteğe bağlı)',
          ),
        ),
      ),
      AppFormField(
        child: TextField(
          controller: _reference,
          decoration: const InputDecoration(
            labelText: 'Referans kolonu (isteğe bağlı)',
          ),
        ),
      ),
      AppFormField(
        child: DropdownButtonFormField<String>(
          key: ValueKey(_encoding),
          initialValue: _encoding,
          isExpanded: true,
          decoration: const InputDecoration(labelText: 'Metin kodlaması'),
          items: const [
            DropdownMenuItem(value: 'utf-8', child: Text('UTF-8')),
            DropdownMenuItem(
              value: 'windows-1254',
              child: Text('Windows-1254 (eski Türkçe dosyalar)'),
            ),
          ],
          onChanged: (value) => setState(() => _encoding = value ?? 'utf-8'),
        ),
      ),
      AppFormField(
        child: DropdownButtonFormField<String>(
          key: ValueKey(_dateFormat),
          initialValue: _dateFormat,
          isExpanded: true,
          decoration: const InputDecoration(labelText: 'Tarih biçimi'),
          items: const [
            DropdownMenuItem(value: 'yyyy-MM-dd', child: Text('2026-08-14')),
            DropdownMenuItem(value: 'dd.MM.yyyy', child: Text('14.08.2026')),
            DropdownMenuItem(value: 'dd/MM/yyyy', child: Text('14/08/2026')),
          ],
          onChanged: (value) =>
              setState(() => _dateFormat = value ?? 'yyyy-MM-dd'),
        ),
      ),
      AppFormField(
        child: DropdownButtonFormField<String>(
          key: ValueKey(_decimalSeparator),
          initialValue: _decimalSeparator,
          isExpanded: true,
          decoration: const InputDecoration(labelText: 'Ondalık ayırıcı'),
          items: const [
            DropdownMenuItem(value: '.', child: Text('Nokta (25.50)')),
            DropdownMenuItem(value: ',', child: Text('Virgül (25,50)')),
          ],
          onChanged: (value) =>
              setState(() => _decimalSeparator = value ?? '.'),
        ),
      ),
    ],
  );
}
