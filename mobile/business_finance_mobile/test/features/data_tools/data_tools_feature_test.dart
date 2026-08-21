import 'dart:typed_data';

import 'package:flutter/material.dart';
import '../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/files/device_file_saver.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/features/data_tools/data/data_tools_models.dart';
import 'package:business_finance_mobile/features/data_tools/data/data_tools_repository.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/data_tools/presentation/data_tools_controller.dart';
import 'package:business_finance_mobile/features/data_tools/presentation/data_tools_page.dart';

void main() {
  testWidgets('veri araçları ekranı erişilebilirlik kapısını geçer', (
    tester,
  ) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: DataToolsPage(repository: FakeDataToolsRepository()),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  // Erişilebilirlik kapısı sayfayı yalnız ilk sekmesinde çiziyordu; diğer
  // dört sekme hiç ölçülmüyordu. Kullanıcı `Belgeler` sekmesinde
  // `RenderFlex overflowed` hatası gördü.
  //
  // Sekme çubuğu kaydırılabilir olduğu için 2.0x ölçekte sondaki sekmeler
  // görünür alanın dışında kalıyor; `ensureVisible` olmadan dokunuş sessizce
  // ıskalanıyor ve test beş sekme için de ilk sekmeyi ölçüyordu. Her sekme
  // kendine özgü bir işaretle doğrulanıyor ki bu bir daha sessizce olmasın.
  const tabMarkers = {
    'CSV': 'CSV seç ve önizle',
    'Belgeler': 'Fiş / belge seç',
    'Yedek': 'Dışa aktar',
  };
  for (final entry in tabMarkers.entries) {
    testWidgets('${entry.key} sekmesi en büyük yazı ölçeğinde taşmaz', (
      tester,
    ) async {
      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: DataToolsPage(repository: FakeDataToolsRepository()),
        ),
      );

      await tester.ensureVisible(find.text(entry.key));
      await tester.pumpAndSettle();
      await tester.tap(find.text(entry.key));
      await tester.pumpAndSettle();
      expect(
        find.text(entry.value),
        findsOneWidget,
        reason: '${entry.key} sekmesine gerçekten geçilmedi.',
      );

      if (entry.key == 'Belgeler') {
        // Taşma ancak bir işlem seçilince ortaya çıkıyor: seçici, taşıran
        // metni (seçili öğenin adını) o zaman çiziyor.
        await tester.tap(
          find.byWidgetPredicate(
            (widget) => widget.runtimeType.toString().startsWith(
              'DropdownButtonFormField',
            ),
          ),
        );
        await tester.pumpAndSettle();
        await tester.tap(find.textContaining('İstanbulkart').last);
        await tester.pumpAndSettle();
      }

      expectNoOverflow(tester);
    });
  }

  test('controller loads only what the file tabs need', () async {
    // Borç ve hedef listeleri buradan çıktı; CSV sekmesi açılırken onların
    // da çekilmesi için bir sebep yoktu.
    final repository = FakeDataToolsRepository();
    final controller = DataToolsController(
      repository,
      now: () => DateTime(2026, 8, 11),
    );

    await controller.load();

    expect(controller.snapshot!.accounts.single.name, 'Cash');
    expect(controller.snapshot!.transactions, isNotEmpty);
    expect(repository.loadCount, 1);
  });

  test(
    'controller keeps stale snapshot visible after a refresh error',
    () async {
      final repository = FakeDataToolsRepository();
      final controller = DataToolsController(repository);
      await controller.load();
      repository.loadError = const ApiException(
        statusCode: 503,
        code: 'server.unavailable',
        message: 'Sunucu geçici olarak kullanılamıyor.',
      );

      await controller.load();

      expect(controller.snapshot, isNotNull);
      expect(controller.isStale, isTrue);
      expect(controller.errorMessage, contains('Sunucu'));
    },
  );

  test('controller exposes a safe binary download error', () async {
    final repository = FakeDataToolsRepository()
      ..downloadError = const ApiException(
        statusCode: 503,
        code: 'server.unavailable',
        message: 'Dosya şu anda indirilemiyor.',
      );
    final controller = DataToolsController(repository);

    final response = await controller.downloadExport('/api/v1/export');

    expect(response, isNull);
    expect(controller.errorMessage, 'Dosya şu anda indirilemiyor.');
    expect(controller.isSubmitting, isFalse);
  });

  test('controller rejects attachments over 5 MiB before upload', () async {
    final repository = FakeDataToolsRepository();
    final controller = DataToolsController(repository);

    final uploaded = await controller.uploadAttachment(
      ApiUpload(
        bytes: List<int>.filled(
          DataToolsController.maximumAttachmentBytes + 1,
          0,
        ),
        fileName: 'large.pdf',
        mediaType: 'application/pdf',
      ),
    );

    expect(uploaded, isFalse);
    expect(controller.errorMessage, 'Belge 5 MiB boyut sınırını aşıyor.');
    expect(repository.uploadAttachmentCount, 0);
  });

  test('controller exposes explicit import result counts', () async {
    final repository = FakeDataToolsRepository()
      ..confirmResponse = const ImportBatchItem(
        'batch-1',
        'statement.csv',
        'partially-imported',
        [
          ImportRowItem(id: '1', rowNumber: 1, status: 'imported'),
          ImportRowItem(id: '2', rowNumber: 2, status: 'skipped-duplicate'),
          ImportRowItem(id: '3', rowNumber: 3, status: 'invalid'),
        ],
      );
    final controller = DataToolsController(repository)
      ..importBatch = const ImportBatchItem(
        'batch-1',
        'statement.csv',
        'staged',
        [ImportRowItem(id: '1', rowNumber: 1, status: 'ready')],
      );

    await controller.confirmImport();

    expect(
      controller.importResultSummary?.message,
      '1 satır içe aktarıldı, 1 satır atlandı, 1 satır hatalı kaldı.',
    );
  });

  /// Confirming imported rows creates real transactions. Before this the other
  /// screens kept showing numbers that no longer existed until the user pulled
  /// to refresh.
  test('import confirmation tells other screens to reread', () async {
    final changes = FinancialDataChanges();
    final controller =
        DataToolsController(
            FakeDataToolsRepository(),
            financialDataChanges: changes,
          )
          ..importBatch = const ImportBatchItem(
            'batch-1',
            'statement.csv',
            'staged',
            [ImportRowItem(id: '1', rowNumber: 1, status: 'ready')],
          );

    await controller.confirmImport();

    expect(changes.activityFeedRevision, 1);
    expect(changes.budgetsRevision, 1);
    expect(changes.accountsRevision, 1);
    // Imported rows are account transactions, never card movements.
    expect(changes.cardsRevision, 0);
  });

  testWidgets('veri araçları yalnız dosya sekmelerini gösterir', (
    tester,
  ) async {
    // Borç ve hedefler kendi ekranlarına taşındı; burada kalanlar gerçekten
    // aynı aileden.
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DataToolsPage(repository: FakeDataToolsRepository()),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('CSV'), findsOneWidget);
    expect(find.text('Belgeler'), findsOneWidget);
    expect(find.text('Yedek'), findsOneWidget);
    expect(find.text('Borçlar'), findsNothing);
    expect(find.text('Hedefler'), findsNothing);
  });

  testWidgets('dışa aktarma eylemleri satırı eşit paylaşır', (tester) async {
    // Önceki hâlde butonlar sağa yaslıydı ve soldakinin başlangıcı satırın
    // ortasından sonra kalıyordu. `Expanded` ikisine de yarım satır verir;
    // bu ölçüm test fontundan bağımsızdır çünkü genişlikleri metin değil
    // kısıtlar belirler.
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DataToolsPage(repository: FakeDataToolsRepository()),
      ),
    );
    await pumpBackupTab(tester);

    final card = tester.getRect(find.byType(AppCard).first);
    final secondary = tester.getRect(find.byType(OutlinedButton).first);
    final primary = tester.getRect(find.byType(FilledButton).first);

    expect(secondary.center.dy, closeTo(primary.center.dy, 0.5));
    expect(secondary.width, closeTo(primary.width, 0.5));
    // Kartın sol kenarıyla ilk butonun arası, sağ kenarıyla son butonun
    // arasına eşit olmalı.
    expect(
      secondary.left - card.left,
      closeTo(card.right - primary.right, 0.5),
    );
  });

  testWidgets('en büyük yazı ölçeğinde eylemler alt alta ve tam genişlikte', (
    tester,
  ) async {
    // Yarım satır o ölçekte yetmez; kırpılmış bir etiket alt alta iki
    // butondan kötüdür.
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: DataToolsPage(repository: FakeDataToolsRepository()),
      ),
    );
    await pumpBackupTab(tester);

    final secondary = tester.getRect(find.byType(OutlinedButton).first);
    final primary = tester.getRect(find.byType(FilledButton).first);

    expect(secondary.center.dy, lessThan(primary.center.dy));
    expect(secondary.width, closeTo(primary.width, 0.5));
    expectNoOverflow(tester);
  });

  testWidgets('yazı ölçeği eşiği aşınca eylemler alt alta geçer', (
    tester,
  ) async {
    // Eşik ölçümden geliyor: 1,2x'te en uzun etiket hâlâ sığıyor, üstünde
    // sığmıyor. Karar yazı ölçeğine bakarak veriliyor, ölçülen metin
    // genişliğine değil — bu yüzden test fontundan bağımsız.
    tester.view.physicalSize = const Size(400, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      const MediaQuery(
        data: MediaQueryData(textScaler: TextScaler.linear(1.25)),
        child: _BackupHost(),
      ),
    );
    await pumpBackupTab(tester);

    final secondary = tester.getRect(find.byType(OutlinedButton).first);
    final primary = tester.getRect(find.byType(FilledButton).first);
    expect(secondary.center.dy, lessThan(primary.center.dy));
  });

  testWidgets('dışa aktarma eylemleri tam etiket ve dar dolgu kullanır', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DataToolsPage(repository: FakeDataToolsRepository()),
      ),
    );
    await pumpBackupTab(tester);

    // Etiket tam yazılıyor: buton `Expanded` ile yarım satıra oturunca yer
    // açıldı, kısaltmaya gerek kalmadı.
    expect(find.text('Önizle ve paylaş'), findsNWidgets(3));

    final style = tester
        .widget<FilledButton>(find.byType(FilledButton).first)
        .style;
    final padding =
        style?.padding?.resolve({}) as EdgeInsets? ?? EdgeInsets.zero;
    expect(padding.left, lessThan(AppSpacing.large));
  });

  testWidgets('backup tab has a separate device save action', (tester) async {
    final saver = FakeDeviceFileSaver();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DataToolsPage(
          repository: FakeDataToolsRepository(),
          fileSaver: saver,
        ),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Yedek'));
    await tester.pumpAndSettle();

    expect(find.text('Cihaza kaydet'), findsNWidgets(3));
    await tester.tap(find.text('Cihaza kaydet').first);
    await tester.pumpAndSettle();

    expect(saver.savedFileName, 'transactions.csv');
    expect(find.text('transactions.csv cihaza kaydedildi.'), findsOneWidget);
  });
}

/// Ölçek testleri için sabit kurulum.
Future<void> pumpBackupTab(WidgetTester tester) async {
  await tester.pumpAndSettle();
  await tester.ensureVisible(find.text('Yedek'));
  await tester.tap(find.text('Yedek'));
  await tester.pumpAndSettle();
}

class _BackupHost extends StatelessWidget {
  const _BackupHost();

  @override
  Widget build(BuildContext context) => MaterialApp(
    theme: AppTheme.light(),
    home: DataToolsPage(repository: FakeDataToolsRepository()),
  );
}

class FakeDeviceFileSaver implements DeviceFileSaver {
  String? savedFileName;

  @override
  Future<bool> save({
    required Uint8List bytes,
    required String fileName,
    required String mimeType,
  }) async {
    savedFileName = fileName;
    return true;
  }
}

class FakeDataToolsRepository implements DataToolsRepositoryContract {
  int loadCount = 0;
  int createDebtCount = 0;
  Map<String, Object?>? lastDebtInput;
  String? openingDebtId;
  Map<String, Object?>? lastOpeningInput;
  int uploadAttachmentCount = 0;
  ApiException? loadError;
  ApiException? downloadError;
  ImportBatchItem confirmResponse = const ImportBatchItem(
    'batch-1',
    'statement.csv',
    'imported',
    [],
  );

  @override
  Future<DataToolsSnapshot> load(String asOfDate) async {
    loadCount++;
    if (loadError case final error?) throw error;
    return const DataToolsSnapshot(
      accounts: [DataChoice('account-1', 'Cash', type: 'bank')],
      categories: [
        DataChoice('category-1', 'Food', type: 'expense'),
        DataChoice('income-1', 'Satış geliri', type: 'income'),
      ],
      transactions: [
        // Açıklama kullanıcının yazdığı serbest metindir ve uzun olabilir.
        // Kısa bir örnek, açılır listedeki taşmayı gizliyordu.
        DataTransaction(
          'transaction-1',
          '2026-08-11',
          '10.0000',
          'İstanbulkart yükleme ve Marmaray geçişi için ödeme',
        ),
      ],
    );
  }

  @override
  Future<ImportBatchItem> stageCsv(
    ApiUpload upload,
    Map<String, String> fields,
  ) async => const ImportBatchItem('batch-1', 'statement.csv', 'staged', []);
  @override
  Future<ImportBatchItem> getImport(String id) async =>
      const ImportBatchItem('batch-1', 'statement.csv', 'staged', []);
  @override
  Future<ImportBatchItem> confirmImport(
    String batchId,
    List<String> rowIds,
  ) async => confirmResponse;
  @override
  Future<void> mapImportRow(
    String batchId,
    String rowId,
    Map<String, Object?> input,
  ) async {}
  @override
  Future<void> resolveDuplicate(
    String batchId,
    String rowId,
    String decision,
  ) async {}
  @override
  Future<List<AttachmentItem>> listAttachments(String transactionId) async =>
      attachments;

  // Gerçek dosya adları uzundur; boş liste taşma hatalarını gizliyordu.
  List<AttachmentItem> attachments = const [
    AttachmentItem(
      'attachment-1',
      'market-alisverisi-fisi-2026-08-11-kasa-3.pdf',
      'application/pdf',
      284913,
    ),
  ];
  @override
  Future<void> uploadAttachment(String transactionId, ApiUpload upload) async {
    uploadAttachmentCount++;
  }

  @override
  Future<ApiBinaryResponse> downloadAttachment(String attachmentId) async =>
      const ApiBinaryResponse(bytes: [1]);
  @override
  Future<ApiBinaryResponse> downloadExport(String path) async {
    if (downloadError case final error?) throw error;
    return const ApiBinaryResponse(bytes: [1]);
  }

  @override
  Future<Map<String, dynamic>> validateBackup(ApiUpload upload) async => {
    'schemaVersion': 2,
    'entityCount': 1,
  };
  @override
  Future<Map<String, dynamic>> restoreBackup(ApiUpload upload) async => {
    'schemaVersion': 2,
    'restoredEntityCount': 1,
  };
}
