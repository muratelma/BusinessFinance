import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';

/// POS'tan geçen paranın ne olduğu (ADR 0019 T5).
///
/// Satış gelir yazar. Kartla tahsil daha önce tanınmış bir alacağın (veresiye,
/// tek seferlik alacak) POS'tan geçen tahsilidir ve gelir yazmaz; satışla aynı
/// yoldan (yolda bekleme, yatış) hesaba geçer.
enum PosSettlementKind {
  sale('sale'),
  collection('collection');

  const PosSettlementKind(this.apiValue);

  final String apiValue;

  /// Alanı göndermeyen eski sunucu yalnız satış yazıyordu.
  static PosSettlementKind fromApi(Object? value) => switch (value) {
    'collection' => collection,
    null || 'sale' => sale,
    _ => throw FormatException('Unknown pos settlement kind: $value'),
  };
}

/// Bir POS tahsilatı.
///
/// Brüt, komisyon ve net **ayrı** okunur: neti gelir diye göstermek,
/// kullanıcının gerçekten kestiği faturayı küçültür ve bankanın kestiği
/// komisyonu görünmez bir gidere çevirirdi (ADR 0015).
class PosSettlementItem {
  const PosSettlementItem({
    required this.id,
    required this.accountName,
    required this.categoryName,
    required this.grossAmount,
    required this.commissionAmount,
    required this.netAmount,
    required this.currency,
    required this.settlementDate,
    required this.expectedTransferDate,
    required this.isInTransit,
    required this.isLate,
    this.accountId = '',
    this.transferredOn,
    this.description,
    this.scope,
    this.posDefinitionName,
    this.posDepositId,
    this.dayCloseId,
    this.countedInDayCloseId,
    this.kind = PosSettlementKind.sale,
    this.counterpartyName,
  });

  /// Satış mı, kartla tahsil mi.
  final PosSettlementKind kind;

  /// Kartla tahsilde parayı ödeyen kişi; satışta `null`.
  final String? counterpartyName;

  bool get isCollection => kind == PosSettlementKind.collection;

  /// Tek tek girilmiş tahsilatı sayan gün sonu; sayılmamışsa `null`. Sayılan
  /// tahsilat tek başına iptal edilemez, önce gün sonu geri alınır.
  final String? countedInDayCloseId;

  /// Tahsilatı üreten gün sonu; tek tek girilende `null`. Gün sonundan gelen
  /// tahsilat tek başına iptal edilemez, gün sonu geri alınır.
  final String? dayCloseId;

  /// Tahsilatın yazıldığı POS tanımının adı; tanımsız girilende `null`.
  final String? posDefinitionName;

  /// Parayı hesaba geçiren yatış; para yoldaysa `null`. Yatışa bağlı
  /// tahsilat önce yatış geri alınmadan iptal edilemez.
  final String? posDepositId;

  final String id;

  /// Paranın geçeceği hesap. Bir yatış tek hesaba düşer; toplu seçim yalnız
  /// aynı hesaptaki tahsilatları birlikte kapatır.
  final String accountId;
  final String accountName;

  /// Satışın gelir kategorisi; kartla tahsilde `null` (gelir tanımaz).
  final String? categoryName;
  final String grossAmount;
  final String commissionAmount;
  final String netAmount;
  final String currency;
  final String settlementDate;
  final String expectedTransferDate;
  final bool isInTransit;
  final bool isLate;
  final String? transferredOn;
  final String? description;

  /// Satışın kapsamı; eski sunucu göndermezse `null`.
  final TransactionScope? scope;

  factory PosSettlementItem.fromJson(Map<String, dynamic> json) =>
      PosSettlementItem(
        id: JsonReaders.string(json, 'id'),
        accountId: JsonReaders.string(json, 'accountId'),
        accountName: JsonReaders.string(json, 'accountName'),
        categoryName: JsonReaders.nullableString(json, 'categoryName'),
        grossAmount: JsonReaders.money(json, 'grossAmount'),
        commissionAmount: JsonReaders.money(json, 'commissionAmount'),
        netAmount: JsonReaders.money(json, 'netAmount'),
        currency: JsonReaders.string(json, 'currency'),
        settlementDate: JsonReaders.date(json, 'settlementDate'),
        expectedTransferDate: JsonReaders.date(json, 'expectedTransferDate'),
        isInTransit: JsonReaders.boolean(json, 'isInTransit'),
        isLate: JsonReaders.boolean(json, 'isLate'),
        transferredOn: JsonReaders.nullableString(json, 'transferredOn'),
        description: JsonReaders.nullableString(json, 'description'),
        scope: TransactionScope.fromApiOrNull(json['scope']),
        posDefinitionName: JsonReaders.nullableString(
          json,
          'posDefinitionName',
        ),
        posDepositId: JsonReaders.nullableString(json, 'posDepositId'),
        dayCloseId: JsonReaders.nullableString(json, 'dayCloseId'),
        countedInDayCloseId: JsonReaders.nullableString(
          json,
          'countedInDayCloseId',
        ),
        kind: PosSettlementKind.fromApi(json['kind']),
        counterpartyName: JsonReaders.nullableString(json, 'counterpartyName'),
      );
}

/// Yatış formunun sunucudan gelen önizlemesi (ADR 0019 T5). Beklenen toplam
/// ve kesinti istemcide hesaplanmaz.
class PosDepositPreview {
  const PosDepositPreview({
    required this.accountName,
    required this.settlementCount,
    required this.expectedAmount,
    required this.depositedAmount,
    required this.deductionAmount,
    required this.exceedsExpected,
    required this.currency,
    required this.earliestDepositDate,
    this.deductionCategoryId,
    this.deductionCategoryName,
  });

  factory PosDepositPreview.fromJson(Map<String, dynamic> json) =>
      PosDepositPreview(
        accountName: JsonReaders.string(json, 'accountName'),
        settlementCount: JsonReaders.integer(json, 'settlementCount'),
        expectedAmount: JsonReaders.money(json, 'expectedAmount'),
        depositedAmount: JsonReaders.money(json, 'depositedAmount'),
        deductionAmount: JsonReaders.money(json, 'deductionAmount'),
        exceedsExpected: JsonReaders.boolean(json, 'exceedsExpected'),
        deductionCategoryId: JsonReaders.nullableString(
          json,
          'deductionCategoryId',
        ),
        deductionCategoryName: JsonReaders.nullableString(
          json,
          'deductionCategoryName',
        ),
        currency: JsonReaders.string(json, 'currency'),
        earliestDepositDate: JsonReaders.date(json, 'earliestDepositDate'),
      );

  final String accountName;
  final int settlementCount;

  /// Seçilen tahsilatların net toplamı.
  final String expectedAmount;
  final String depositedAmount;

  /// Beklenen eksi yatan; fazla yatan tutarda sıfır döner.
  final String deductionAmount;

  /// Yatan tutar beklenenden fazla: kayıt reddedilir.
  final bool exceedsExpected;

  /// Kesinti varsa dolu gelecek kategori: POS'un komisyon kategorisi.
  final String? deductionCategoryId;
  final String? deductionCategoryName;
  final String currency;

  /// Yatış bu günden önce olamaz: seçilen tahsilatların en geç olanı.
  final String earliestDepositDate;

  bool get hasDeduction => !_isZero(deductionAmount);
}

/// Bankanın POS parasını hesaba yatırdığı an: bir ya da birkaç tahsilatı tek
/// para hareketiyle kapatır. Eksik yatan kısım kesinti gideridir.
class PosDeposit {
  const PosDeposit({
    required this.id,
    required this.accountName,
    required this.depositDate,
    required this.expectedAmount,
    required this.depositedAmount,
    required this.deductionAmount,
    required this.currency,
    required this.isCancelled,
    required this.settlements,
    this.deductionCategoryName,
    this.balanceAfter,
    this.grossAmount,
    this.commissionAmount,
    this.collectionAmount,
    this.saleAmount,
  });

  factory PosDeposit.fromJson(Map<String, dynamic> json) => PosDeposit(
    balanceAfter: JsonReaders.nullableString(json, 'balanceAfter'),
    grossAmount: JsonReaders.nullableString(json, 'grossAmount'),
    commissionAmount: JsonReaders.nullableString(json, 'commissionAmount'),
    collectionAmount: JsonReaders.nullableString(json, 'collectionAmount'),
    saleAmount: JsonReaders.nullableString(json, 'saleAmount'),
    id: JsonReaders.string(json, 'id'),
    accountName: JsonReaders.string(json, 'accountName'),
    depositDate: JsonReaders.date(json, 'depositDate'),
    expectedAmount: JsonReaders.money(json, 'expectedAmount'),
    depositedAmount: JsonReaders.money(json, 'depositedAmount'),
    deductionAmount: JsonReaders.money(json, 'deductionAmount'),
    deductionCategoryName: JsonReaders.nullableString(
      json,
      'deductionCategoryName',
    ),
    currency: JsonReaders.string(json, 'currency'),
    isCancelled: JsonReaders.boolean(json, 'isCancelled'),
    settlements: JsonReaders.list(json, 'settlements')
        .map(
          (item) => PosSettlementItem.fromJson(
            JsonReaders.object(item, 'settlement'),
          ),
        )
        .toList(growable: false),
  );

  final String id;
  final String accountName;
  final String depositDate;
  final String expectedAmount;

  /// Bankanın gerçekten yatırdığı tutar.
  final String depositedAmount;
  final String deductionAmount;
  final String? deductionCategoryName;
  final String currency;

  /// Geri alınmış yatış: kayıt durur, tahsilatları yola dönmüştür.
  final bool isCancelled;

  /// Kapattığı tahsilatlar; geri alınmış yatışta boştur.
  final List<PosSettlementItem> settlements;

  /// Yatıştan hemen sonra hesabın bakiyesi; yalnız yatış okunurken gelir.
  final String? balanceAfter;

  /// Kapattığı tahsilatların brüt satış ve komisyon toplamı; sunucudan gelir.
  /// Geri alınmış yatışta boştur.
  final String? grossAmount;
  final String? commissionAmount;

  /// Brütün satışlardan ve kartla tahsil edilmiş alacaklardan gelen kısmı;
  /// sunucudan gelir, istemci brütten çıkarmaz. Eski sunucuda `null`.
  final String? collectionAmount;
  final String? saleAmount;

  bool get hasCollection =>
      collectionAmount != null && !_isZero(collectionAmount!);

  /// Satış payı var mı; eski sunucu ayırmıyorsa her şey satıştır.
  bool get hasSale => saleAmount == null || !_isZero(saleAmount!);

  bool get hasDeduction => !_isZero(deductionAmount);
}

bool _isZero(String money) =>
    !money.replaceAll(RegExp('[^0-9]'), '').contains(RegExp('[1-9]'));

/// POS tanımı (ADR 0019 T4): bir kez girilen ayar. Para taşımaz; tahsilat
/// formunu doldurur.
class PosDefinitionItem {
  const PosDefinitionItem({
    required this.id,
    required this.name,
    required this.accountId,
    required this.accountName,
    required this.salesCategoryId,
    required this.salesCategoryName,
    required this.commissionRate,
    required this.transferDays,
    required this.businessDaysOnly,
    required this.isActive,
    this.isDefault = false,
    this.commissionCategoryId,
    this.commissionCategoryName,
  });

  /// Ana POS: tahsilat formunda seçili gelir; kullanıcı başına en çok bir tane.
  final bool isDefault;

  factory PosDefinitionItem.fromJson(Map<String, dynamic> json) =>
      PosDefinitionItem(
        id: JsonReaders.string(json, 'id'),
        name: JsonReaders.string(json, 'name'),
        accountId: JsonReaders.string(json, 'accountId'),
        accountName: JsonReaders.string(json, 'accountName'),
        salesCategoryId: JsonReaders.string(json, 'salesCategoryId'),
        salesCategoryName: JsonReaders.string(json, 'salesCategoryName'),
        commissionCategoryId: JsonReaders.nullableString(
          json,
          'commissionCategoryId',
        ),
        commissionCategoryName: JsonReaders.nullableString(
          json,
          'commissionCategoryName',
        ),
        commissionRate: JsonReaders.string(json, 'commissionRate'),
        transferDays: JsonReaders.integer(json, 'transferDays'),
        businessDaysOnly: JsonReaders.boolean(json, 'businessDaysOnly'),
        isActive: JsonReaders.boolean(json, 'isActive'),
        isDefault: json['isDefault'] as bool? ?? false,
      );

  final String id;
  final String name;
  final String accountId;
  final String accountName;
  final String salesCategoryId;
  final String salesCategoryName;
  final String? commissionCategoryId;
  final String? commissionCategoryName;

  /// Ondalık kesir, dört basamak: `0.0179` = %1,79. Para değildir.
  final String commissionRate;
  final int transferDays;
  final bool businessDaysOnly;
  final bool isActive;

  /// `%1,79` · oran sıfırsa `Komisyon yok`.
  String get rateLabel {
    final percent = PosRate.percentText(commissionRate);
    return percent == null ? 'Komisyon yok' : '%$percent';
  }

  /// `Aynı gün` · `1 iş günü` · `20 gün`.
  String get transferLabel => transferDays == 0
      ? 'Aynı gün'
      : '$transferDays ${businessDaysOnly ? 'iş günü' : 'gün'}';

  /// Liste satırının alt yazısı: `Ziraat Vadesiz · %1,79 · 1 iş günü`.
  String get summary => '$accountName · $rateLabel · $transferLabel';
}

/// Oranın kullanıcıya gösterilen yüzdesi ile sözleşmedeki kesri arasındaki
/// çeviri. Oran para değildir; yuvarlama burada kayıp üretmez çünkü iki yanda
/// da en çok dört (kesir) / iki (yüzde) basamak vardır.
abstract final class PosRate {
  /// `0.0179` → `1,79`; sıfır ya da okunamayan değer `null`.
  static String? percentText(String fraction) {
    final value = double.tryParse(fraction);
    if (value == null || value <= 0) return null;
    var text = (value * 100).toStringAsFixed(2);
    if (text.contains('.')) {
      text = text.replaceFirst(RegExp(r'0+$'), '');
      text = text.replaceFirst(RegExp(r'\.$'), '');
    }
    return text.replaceAll('.', ',');
  }

  /// `1,79` → `0.0179`; boş alan sıfır orandır. Okunamazsa ya da aralık
  /// dışındaysa `null`.
  static String? fractionWire(String percentInput) {
    final trimmed = percentInput.trim();
    if (trimmed.isEmpty) return '0.0000';
    final percent = double.tryParse(trimmed.replaceAll(',', '.'));
    if (percent == null || percent < 0 || percent >= 100) return null;
    return (percent / 100).toStringAsFixed(4);
  }
}

/// Tanımla yazılacak tahsilatın sunucudan gelen önizlemesi. İstemci
/// komisyonu ve neti kendisi hesaplamaz.
class PosPreview {
  const PosPreview({
    required this.commissionAmount,
    required this.netAmount,
    required this.currency,
    required this.expectedTransferDate,
  });

  factory PosPreview.fromJson(Map<String, dynamic> json) => PosPreview(
    commissionAmount: JsonReaders.money(json, 'commissionAmount'),
    netAmount: JsonReaders.money(json, 'netAmount'),
    currency: JsonReaders.string(json, 'currency'),
    expectedTransferDate: JsonReaders.date(json, 'expectedTransferDate'),
  );

  final String commissionAmount;
  final String netAmount;
  final String currency;
  final String expectedTransferDate;
}

/// POS tanımı formunun gönderdiği alanlar.
class PosDefinitionInput {
  const PosDefinitionInput({
    required this.name,
    required this.accountId,
    required this.salesCategoryId,
    required this.commissionRate,
    required this.transferDays,
    required this.businessDaysOnly,
    this.commissionCategoryId,
  });

  final String name;
  final String accountId;
  final String salesCategoryId;
  final String commissionRate;
  final int transferDays;
  final bool businessDaysOnly;
  final String? commissionCategoryId;

  Map<String, Object?> toJson() => {
    'name': name,
    'accountId': accountId,
    'salesCategoryId': salesCategoryId,
    'commissionRate': commissionRate,
    'transferDays': transferDays,
    'businessDaysOnly': businessDaysOnly,
    'commissionCategoryId': commissionCategoryId,
  };
}

class PosSettlementList {
  const PosSettlementList({
    required this.items,
    required this.moneyInTransit,
    required this.inTransitCount,
  });

  final List<PosSettlementItem> items;

  /// Yoldaki net toplam. Liste penceresinden bağımsızdır.
  final String moneyInTransit;
  final int inTransitCount;

  factory PosSettlementList.fromJson(Map<String, dynamic> json) =>
      PosSettlementList(
        items: JsonReaders.list(json, 'items')
            .map(
              (item) =>
                  PosSettlementItem.fromJson(JsonReaders.object(item, 'item')),
            )
            .toList(growable: false),
        moneyInTransit: JsonReaders.money(json, 'moneyInTransit'),
        inTransitCount: JsonReaders.integer(json, 'inTransitCount'),
      );
}

class PosOptions {
  const PosOptions({
    required this.accounts,
    required this.incomeCategories,
    required this.expenseCategories,
  });

  /// Yalnız banka hesapları: POS parası tezgâhın çekmecesine değil bankaya
  /// geçer.
  final List<DataChoice> accounts;
  final List<DataChoice> incomeCategories;
  final List<DataChoice> expenseCategories;
}

abstract interface class PosRepositoryContract {
  Future<PosSettlementList> list({required bool inTransitOnly});

  Future<PosOptions> loadOptions();

  Future<void> create(Map<String, Object?> input);

  /// Seçilen yoldaki tahsilatlar için beklenen toplam ve kesinti. Tutar
  /// verilmezse beklenen tutar yatmış sayılır.
  Future<PosDepositPreview> previewDeposit({
    required List<String> settlementIds,
    String? depositedAmount,
  });

  /// "Hesaba geçenleri işaretle": seçilen tahsilatları tek yatışla kapatır.
  /// [clientRequestId] aynı isteğin ikinci kez yazılmasını önler.
  Future<PosDeposit> createDeposit({
    required String clientRequestId,
    required List<String> settlementIds,
    required String depositedAmount,
    required String depositDate,
    String? deductionCategoryId,
  });

  Future<PosDeposit> getDeposit({required String depositId});

  /// Yatışı geri alır: tahsilatlar yola döner, kesinti gideri iptal olur.
  Future<PosDeposit> revertDeposit({required String depositId});

  /// Silme yerine iptal: satış ve komisyon düşer, yoldaki tutar kalkar.
  /// Yatışa bağlı tahsilat reddedilir; önce yatış geri alınır.
  Future<void> cancel({required String settlementId});

  /// Kullanıcının POS tanımları; önce aktifler.
  Future<List<PosDefinitionItem>> listDefinitions();

  /// [definitionId] verilirse düzenler, verilmezse yeni tanım açar.
  Future<void> saveDefinition(PosDefinitionInput input, {String? definitionId});

  Future<void> setDefinitionActive({
    required String definitionId,
    required bool isActive,
  });

  /// Ana POS'u seçer; öncekinin işareti sunucuda kalkar.
  Future<void> setDefaultDefinition({required String definitionId});

  /// Tahsilatı olan tanım silinmez (409); pasife alınır.
  Future<void> deleteDefinition({required String definitionId});

  Future<PosPreview> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  });
}

class PosRepository implements PosRepositoryContract {
  const PosRepository(this._client);

  final ApiClient _client;

  @override
  Future<PosSettlementList> list({required bool inTransitOnly}) async {
    final response = await _client.get(
      '/api/v1/pos-settlements?inTransitOnly=$inTransitOnly',
    );
    return PosSettlementList.fromJson(response.requireObject());
  }

  @override
  Future<PosOptions> loadOptions() async {
    final responses = await Future.wait([
      _client.get(
        '/api/v1/accounts?isActive=true&type=bank&pageNumber=1&pageSize=100',
      ),
      _client.get('/api/v1/categories?type=income&isActive=true'),
      _client.get('/api/v1/categories?type=expense&isActive=true'),
    ]);
    return PosOptions(
      accounts: _items(responses[0].requireObject())
          .map(
            (item) => DataChoice(
              JsonReaders.string(item, 'id'),
              JsonReaders.string(item, 'name'),
            ),
          )
          .toList(growable: false),
      incomeCategories: _items(
        responses[1].requireObject(),
      ).map(DataChoice.categoryFromJson).toList(growable: false),
      expenseCategories: _items(
        responses[2].requireObject(),
      ).map(DataChoice.categoryFromJson).toList(growable: false),
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async {
    await _client.post('/api/v1/pos-settlements', body: input);
  }

  @override
  Future<PosDepositPreview> previewDeposit({
    required List<String> settlementIds,
    String? depositedAmount,
  }) async {
    final query = [
      for (final id in settlementIds) 'settlementIds=$id',
      if (depositedAmount != null) 'depositedAmount=$depositedAmount',
    ].join('&');
    final response = await _client.get('/api/v1/pos-deposits/preview?$query');
    return PosDepositPreview.fromJson(response.requireObject());
  }

  @override
  Future<PosDeposit> createDeposit({
    required String clientRequestId,
    required List<String> settlementIds,
    required String depositedAmount,
    required String depositDate,
    String? deductionCategoryId,
  }) async {
    final response = await _client.post(
      '/api/v1/pos-deposits',
      body: {
        'clientRequestId': clientRequestId,
        'settlementIds': settlementIds,
        'depositedAmount': depositedAmount,
        'depositDate': depositDate,
        'deductionCategoryId': deductionCategoryId,
      },
    );
    return PosDeposit.fromJson(response.requireObject());
  }

  @override
  Future<PosDeposit> getDeposit({required String depositId}) async {
    final response = await _client.get('/api/v1/pos-deposits/$depositId');
    return PosDeposit.fromJson(response.requireObject());
  }

  @override
  Future<PosDeposit> revertDeposit({required String depositId}) async {
    final response = await _client.delete('/api/v1/pos-deposits/$depositId');
    return PosDeposit.fromJson(response.requireObject());
  }

  @override
  Future<void> cancel({required String settlementId}) async {
    await _client.delete('/api/v1/pos-settlements/$settlementId');
  }

  @override
  Future<List<PosDefinitionItem>> listDefinitions() async {
    final response = await _client.get('/api/v1/pos-definitions');
    return _items(
      response.requireObject(),
    ).map(PosDefinitionItem.fromJson).toList(growable: false);
  }

  @override
  Future<void> saveDefinition(
    PosDefinitionInput input, {
    String? definitionId,
  }) async {
    if (definitionId == null) {
      await _client.post('/api/v1/pos-definitions', body: input.toJson());
    } else {
      await _client.put(
        '/api/v1/pos-definitions/$definitionId',
        body: input.toJson(),
      );
    }
  }

  @override
  Future<void> setDefinitionActive({
    required String definitionId,
    required bool isActive,
  }) async {
    await _client.patch(
      '/api/v1/pos-definitions/$definitionId/active',
      body: {'isActive': isActive},
    );
  }

  @override
  Future<void> setDefaultDefinition({required String definitionId}) async {
    await _client.put('/api/v1/pos-definitions/$definitionId/default');
  }

  @override
  Future<void> deleteDefinition({required String definitionId}) async {
    await _client.delete('/api/v1/pos-definitions/$definitionId');
  }

  @override
  Future<PosPreview> preview({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  }) async {
    final response = await _client.get(
      '/api/v1/pos-definitions/$definitionId/preview'
      '?grossAmount=$grossAmount&settlementDate=$settlementDate',
    );
    return PosPreview.fromJson(response.requireObject());
  }

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
