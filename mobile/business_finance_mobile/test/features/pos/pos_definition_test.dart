import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_controller.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_definitions_page.dart';
import 'package:business_finance_mobile/features/pos/presentation/pos_settlements_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Aşama 06.3 Grup 4: POS tanımı (ADR 0019 T4). Tanım bir kez girilir;
/// tahsilat formu ondan dolar ve akşamki giriş "tutar + kaydet"e iner.
void main() {
  group('oran çevirisi', () {
    test('kesir yüzdeye, yüzde kesre döner', () {
      expect(PosRate.percentText('0.0179'), '1,79');
      expect(PosRate.percentText('0.0200'), '2');
      expect(PosRate.percentText('0.0650'), '6,5');
      expect(PosRate.percentText('0.0000'), isNull);

      expect(PosRate.fractionWire('1,79'), '0.0179');
      expect(PosRate.fractionWire('2'), '0.0200');
      // Boş alan komisyonsuz POS'tur.
      expect(PosRate.fractionWire('  '), '0.0000');
      expect(PosRate.fractionWire('yüzde iki'), isNull);
      expect(PosRate.fractionWire('100'), isNull);
    });

    test('tanımın özeti hesabı, oranı ve geçiş süresini söyler', () {
      expect(_definition().summary, 'Ziraat Vadesiz · %1,79 · 1 iş günü');
      expect(
        _definition(rate: '0.0000', days: 0).summary,
        'Ziraat Vadesiz · Komisyon yok · Aynı gün',
      );
      expect(
        _definition(days: 20, businessDays: false).transferLabel,
        '20 gün',
      );
    });
  });

  group('controller', () {
    test(
      'tanımla yazılan tahsilat yalnız tutarı, günü ve tanımı gönderir',
      () async {
        final repository = _Repository(definitions: [_definition()]);
        final controller = PosController(repository);

        final saved = await controller.create(
          grossAmount: '1000.0000',
          settlementDate: '2026-09-25',
          posDefinitionId: 'ziraat',
        );

        expect(saved, isTrue);
        expect(repository.created!['posDefinitionId'], 'ziraat');
        expect(repository.created!['grossAmount'], '1000.0000');
        // Hesap, kategori, komisyon ve beklenen gün gönderilmez: sunucu
        // tanımdan doldurur.
        expect(repository.created!['accountId'], isNull);
        expect(repository.created!['categoryId'], isNull);
        expect(repository.created!['expectedTransferDate'], isNull);
        expect(repository.created!['commissionRate'], isNull);
        controller.dispose();
      },
    );

    test(
      'tanım kaydı finansal hedef yükseltmez; yalnız listeyi yeniler',
      () async {
        final changes = FinancialDataChanges();
        final repository = _Repository();
        final controller = PosController(repository, changes: changes);
        final cash = changes.cashRevision;
        final feed = changes.activityFeedRevision;

        final saved = await controller.saveDefinition(_input());

        expect(saved, isTrue);
        expect(repository.saved.single.name, 'Ziraat POS');
        expect(controller.definitions, hasLength(1));
        // Tanım para taşımaz.
        expect(changes.cashRevision, cash);
        expect(changes.activityFeedRevision, feed);
        controller.dispose();
      },
    );

    test('silinemeyen tanımın hatası tanım sayfasında kalır', () async {
      final repository = _Repository(
        definitions: [_definition()],
        deleteError: const ApiException(
          code: 'pos_definitions.has_settlements',
          message: 'Bu POS ile tahsilat yazıldığı için silinemez.',
          statusCode: 409,
        ),
      );
      final controller = PosController(repository);
      await controller.loadDefinitions();

      final deleted = await controller.deleteDefinition(_definition());

      expect(deleted, isFalse);
      expect(controller.definitionError, contains('silinemez'));
      // Tahsilat listesinin hatası değildir.
      expect(controller.errorMessage, isNull);
      controller.dispose();
    });

    // Seçenekler saklanırsa sonradan açılan hesap formda görünmez, silinen
    // hesap seçenek olarak kalır.
    test('hesap ve kategori seçenekleri her açılışta yeniden okunur', () async {
      final repository = _Repository();
      final controller = PosController(repository);

      await controller.loadOptions();
      await controller.loadOptions();

      expect(repository.optionLoads, 2);
      controller.dispose();
    });

    test('önizleme hatası formu durdurmaz', () async {
      final controller = PosController(_Repository(previewFails: true));

      expect(
        await controller.preview(
          definitionId: 'ziraat',
          grossAmount: '100.0000',
          settlementDate: '2026-09-25',
        ),
        isNull,
      );
      controller.dispose();
    });
  });

  group('tahsilat formu', () {
    testWidgets('tanım varken yalnız tutar ve gün sorulur, özet sunucudan', (
      tester,
    ) async {
      final repository = _Repository(definitions: [_definition()]);
      final controller = PosController(repository);
      await _pumpForm(tester, controller);

      expect(find.text('Ziraat POS'), findsOneWidget);
      // Hesap, kategori ve komisyon alanları tanımdadır; sorulmaz.
      expect(find.text('Satış kategorisi'), findsNothing);
      expect(find.text('Paranın beklendiği gün'), findsNothing);
      expect(find.text('Komisyon (%1,79)'), findsOneWidget);

      await tester.enterText(find.byType(TextFormField).first, '1000');
      // Önizleme kısa bir duraklamadan sonra istenir.
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pumpAndSettle();

      expect(repository.previewed.single.grossAmount, '1000.0000');
      expect(find.text('₺982,10'), findsOneWidget);
      expect(find.text('-₺17,90'), findsOneWidget);

      await tester.tap(find.text('Tahsilatı kaydet'));
      await tester.pumpAndSettle();

      expect(repository.created!['posDefinitionId'], 'ziraat');
      expect(repository.created!['accountId'], isNull);
      controller.dispose();
    });

    testWidgets('tanım yokken form her şeyi sorar ve tanımlamayı önerir', (
      tester,
    ) async {
      final controller = PosController(_Repository());
      await _pumpForm(tester, controller);

      expect(find.text('POS ekle'), findsOneWidget);
      expect(find.text('Satış kategorisi'), findsOneWidget);
      expect(find.text('Paranın beklendiği gün'), findsOneWidget);
      controller.dispose();
    });

    // Ana POS listede ilk sırada olmasa da formda o seçili gelir.
    testWidgets('form ana POS seçili açılır', (tester) async {
      final controller = PosController(
        _Repository(
          definitions: [
            _definition(id: 'akbank', name: 'Akbank POS'),
            _definition(isDefault: true),
          ],
        ),
      );
      await _pumpForm(tester, controller);

      expect(find.text('Ziraat POS'), findsOneWidget);
      expect(find.text('Akbank POS'), findsNothing);
      controller.dispose();
    });

    testWidgets('"Elle gir" seçilince bütün alanlar gelir', (tester) async {
      final controller = PosController(
        _Repository(definitions: [_definition()]),
      );
      await _pumpForm(tester, controller);

      await tester.tap(find.text('Ziraat POS'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Elle gir').last);
      await tester.pumpAndSettle();

      expect(find.text('Satış kategorisi'), findsOneWidget);
      expect(find.text('Paranın beklendiği gün'), findsOneWidget);
      controller.dispose();
    });
  });

  group("POS'larım", () {
    testWidgets('tanımlar özetleriyle listelenir; pasif olan etiketlenir', (
      tester,
    ) async {
      final controller = PosController(
        _Repository(
          definitions: [
            _definition(),
            _definition(id: 'yemek', name: 'Yemek kartı', active: false),
          ],
        ),
      );
      await _pumpPage(tester, PosDefinitionsPage(controller: controller));

      expect(find.text('Ziraat POS'), findsOneWidget);
      expect(find.text('Ziraat Vadesiz · %1,79 · 1 iş günü'), findsNWidgets(2));
      expect(find.text('Pasif'), findsOneWidget);
      controller.dispose();
    });

    testWidgets('yıldız ana POS\'u seçer; öncekinin işareti kalkar', (
      tester,
    ) async {
      final repository = _Repository(
        definitions: [
          _definition(isDefault: true),
          _definition(id: 'akbank', name: 'Akbank POS'),
        ],
      );
      final controller = PosController(repository);
      await _pumpPage(tester, PosDefinitionsPage(controller: controller));

      expect(find.byTooltip('Ana POS'), findsOneWidget);
      await tester.tap(find.byTooltip('Ana POS yap'));
      await tester.pumpAndSettle();

      expect(repository.defaultId, 'akbank');
      expect(controller.preferredDefinition!.id, 'akbank');
      expect(find.byTooltip('Ana POS'), findsOneWidget);
      expect(find.byTooltip('Ana POS yap'), findsOneWidget);
      controller.dispose();
    });

    // Ad sınırı 80 karakter: en uzun ad listede, formdaki seçimde ve büyük
    // yazıda taşma üretmemeli.
    testWidgets('en uzun POS adı listede ve formda taşmaz', (tester) async {
      final longName = 'Çok uzun adlı bir POS ' * 4;
      final name = longName.substring(0, 80);
      final controller = PosController(
        _Repository(definitions: [_definition(name: name)]),
      );
      await _pumpPage(
        tester,
        PosDefinitionsPage(controller: controller),
        textScale: 2,
      );
      expect(tester.takeException(), isNull);

      await _pumpForm(tester, controller);
      expect(tester.takeException(), isNull);
      controller.dispose();
    });

    testWidgets('tanım yokken boş durum bir sonraki adımı sunar', (
      tester,
    ) async {
      final controller = PosController(_Repository());
      await _pumpPage(tester, PosDefinitionsPage(controller: controller));

      expect(find.text('Henüz POS eklenmedi'), findsOneWidget);
      expect(find.widgetWithText(FilledButton, 'POS ekle'), findsOneWidget);
      controller.dispose();
    });

    testWidgets('okuma hatası yeniden deneme sunar', (tester) async {
      final controller = PosController(
        _Repository(
          listError: const ApiException(
            code: 'network.unavailable',
            message: 'Bağlantı yok.',
          ),
        ),
      );
      await _pumpPage(tester, PosDefinitionsPage(controller: controller));

      expect(find.text('Bağlantı yok.'), findsOneWidget);
      expect(find.text('Tekrar dene'), findsOneWidget);
      controller.dispose();
    });

    testWidgets('form hazır kategorileri seçili açar ve oranı kesre çevirir', (
      tester,
    ) async {
      final repository = _Repository();
      final controller = PosController(repository);
      await _pumpPage(tester, PosDefinitionFormPage(controller: controller));

      // İşletme setinin kategorileri hazır seçili: kullanıcı yalnız adı ve
      // oranı yazar.
      expect(find.text('Satış geliri'), findsOneWidget);

      await tester.enterText(find.byType(TextFormField).at(0), 'Ziraat POS');
      await tester.enterText(find.byType(TextFormField).at(1), '1,79');
      await tester.pumpAndSettle();
      expect(find.text('Banka ve POS komisyonu'), findsOneWidget);

      await tester.ensureVisible(find.text('Kaydet'));
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      final input = repository.saved.single;
      expect(input.name, 'Ziraat POS');
      expect(input.accountId, 'ziraat-hesap');
      expect(input.salesCategoryId, 'satis');
      expect(input.commissionRate, '0.0179');
      expect(input.commissionCategoryId, 'komisyon');
      expect(input.transferDays, 1);
      expect(input.businessDaysOnly, isTrue);
      controller.dispose();
    });

    testWidgets('ad boşsa ve oran okunamıyorsa istek gitmez', (tester) async {
      final repository = _Repository();
      final controller = PosController(repository);
      await _pumpPage(tester, PosDefinitionFormPage(controller: controller));

      await tester.enterText(find.byType(TextFormField).at(1), 'yüzde iki');
      await tester.ensureVisible(find.text('Kaydet'));
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(find.text('POS için bir ad yazın.'), findsOneWidget);
      expect(find.text('Oranı yüzde olarak yazın (ör. 1,79).'), findsOneWidget);
      expect(repository.saved, isEmpty);
      controller.dispose();
    });

    testWidgets('2.0× yazıda taşma olmaz', (tester) async {
      final controller = PosController(
        _Repository(definitions: [_definition()]),
      );
      await _pumpPage(
        tester,
        PosDefinitionsPage(controller: controller),
        textScale: 2,
      );

      expect(tester.takeException(), isNull);
      expect(find.text('Ziraat POS'), findsOneWidget);
      controller.dispose();
    });
  });
}

Future<void> _pumpPage(
  WidgetTester tester,
  Widget page, {
  double textScale = 1,
}) async {
  tester.view.physicalSize = const Size(412 * 3, 1400 * 3);
  tester.view.devicePixelRatio = 3;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      builder: (context, child) => MediaQuery(
        data: MediaQuery.of(
          context,
        ).copyWith(textScaler: TextScaler.linear(textScale)),
        child: child!,
      ),
      home: page,
    ),
  );
  await tester.pumpAndSettle();
}

Future<void> _pumpForm(WidgetTester tester, PosController controller) async {
  await _pumpPage(
    tester,
    Scaffold(
      body: Builder(
        builder: (context) => TextButton(
          onPressed: () => showPosSettlementForm(context, controller),
          child: const Text('aç'),
        ),
      ),
    ),
  );
  await tester.tap(find.text('aç'));
  await tester.pumpAndSettle();
}

PosDefinitionItem _definition({
  String id = 'ziraat',
  String name = 'Ziraat POS',
  String rate = '0.0179',
  int days = 1,
  bool businessDays = true,
  bool active = true,
  bool isDefault = false,
}) => PosDefinitionItem(
  id: id,
  name: name,
  accountId: 'ziraat-hesap',
  accountName: 'Ziraat Vadesiz',
  salesCategoryId: 'satis',
  salesCategoryName: 'Satış geliri',
  commissionCategoryId: 'komisyon',
  commissionCategoryName: 'Banka ve POS komisyonu',
  commissionRate: rate,
  transferDays: days,
  businessDaysOnly: businessDays,
  isActive: active,
  isDefault: isDefault,
);

PosDefinitionInput _input() => const PosDefinitionInput(
  name: 'Ziraat POS',
  accountId: 'ziraat-hesap',
  salesCategoryId: 'satis',
  commissionRate: '0.0179',
  commissionCategoryId: 'komisyon',
  transferDays: 1,
  businessDaysOnly: true,
);

typedef _PreviewCall = ({
  String definitionId,
  String grossAmount,
  String settlementDate,
});

class _Repository implements PosRepositoryContract {
  _Repository({
    List<PosDefinitionItem>? definitions,
    this.listError,
    this.deleteError,
    this.previewFails = false,
  }) : definitions = [...?definitions];

  final List<PosDefinitionItem> definitions;
  final ApiException? listError;
  final ApiException? deleteError;
  final bool previewFails;

  Map<String, Object?>? created;
  final saved = <PosDefinitionInput>[];
  final previewed = <_PreviewCall>[];

  @override
  Future<PosSettlementList> list({
    required bool inTransitOnly,
    String? from,
    String? to,
  }) async => const PosSettlementList(
    items: [],
    moneyInTransit: '0.0000',
    inTransitCount: 0,
  );

  int optionLoads = 0;

  @override
  Future<PosOptions> loadOptions() async {
    optionLoads++;
    return _options;
  }

  static const _options = PosOptions(
    accounts: [
      DataChoice(
        'ziraat-hesap',
        'Ziraat Vadesiz',
        defaultScope: TransactionScope.business,
      ),
    ],
    incomeCategories: [
      DataChoice('satis', 'Satış geliri'),
      DataChoice('diger-gelir', 'Diğer gelir'),
    ],
    expenseCategories: [
      DataChoice('kira', 'Kira'),
      DataChoice('komisyon', 'Banka ve POS komisyonu'),
    ],
  );

  @override
  Future<void> create(Map<String, Object?> input) async => created = input;

  @override
  Future<PosDepositPreview> previewDeposit({
    required List<String> settlementIds,
    String? depositedAmount,
  }) async => throw UnimplementedError();

  @override
  Future<PosDeposit> createDeposit({
    required String clientRequestId,
    required List<String> settlementIds,
    required String depositedAmount,
    required String depositDate,
    String? deductionCategoryId,
  }) async => throw UnimplementedError();

  @override
  Future<PosDeposit> getDeposit({required String depositId}) async =>
      throw UnimplementedError();

  @override
  Future<PosDeposit> revertDeposit({required String depositId}) async =>
      throw UnimplementedError();

  @override
  Future<void> cancel({required String settlementId}) async {}

  @override
  Future<List<PosDefinitionItem>> listDefinitions() async {
    if (listError != null) throw listError!;
    return [...definitions];
  }

  @override
  Future<void> saveDefinition(
    PosDefinitionInput input, {
    String? definitionId,
  }) async {
    saved.add(input);
    definitions.add(_definition(id: 'yeni', name: input.name));
  }

  @override
  Future<void> setDefinitionActive({
    required String definitionId,
    required bool isActive,
  }) async {}

  String? defaultId;

  @override
  Future<void> setDefaultDefinition({required String definitionId}) async {
    defaultId = definitionId;
    for (var i = 0; i < definitions.length; i++) {
      final item = definitions[i];
      definitions[i] = _definition(
        id: item.id,
        name: item.name,
        active: item.isActive,
        isDefault: item.id == definitionId,
      );
    }
  }

  @override
  Future<void> deleteDefinition({required String definitionId}) async {
    if (deleteError != null) throw deleteError!;
    definitions.removeWhere((item) => item.id == definitionId);
  }

  @override
  Future<PosPreview> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  }) async {
    if (previewFails) {
      throw const ApiException(code: 'network.unavailable', message: 'Yok');
    }
    previewed.add((
      definitionId: definitionId,
      grossAmount: grossAmount,
      settlementDate: settlementDate,
    ));
    return const PosPreview(
      commissionAmount: '17.9000',
      netAmount: '982.1000',
      currency: 'TRY',
      expectedTransferDate: '2026-09-28',
    );
  }
}
