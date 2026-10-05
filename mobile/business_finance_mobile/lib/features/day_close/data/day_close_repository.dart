import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import '../../pos/data/pos_repository.dart';

/// Gün sonu panelinin sunucuya gönderdiği girdi (ADR 0019 T1–T2); önizleme
/// ve kayıt aynı girdiyi alır.
///
/// Nakit, POS satırları ve toplamdan **ikisi yeter**; boş bırakılan alan
/// gönderilmez ve sunucu onu hesaplar. [recordOverrides] yalnız kullanıcının
/// değiştirdiği işaretleri taşır: adı geçmeyen kayıt sunucudaki varsayılanıyla
/// işlenir.
class DayCloseInput {
  const DayCloseInput({
    required this.date,
    this.cashAmount,
    this.posAmounts = const {},
    this.totalAmount,
    this.cashAccountId,
    this.cashCategoryId,
    this.recordOverrides = const {},
    this.isAdditional = false,
  });

  /// `yyyy-MM-dd`.
  final String date;

  /// Dört ondalıklı string; boşsa `null`.
  final String? cashAmount;

  /// POS kimliği → dört ondalıklı tutar; yalnız yazılmış satırlar.
  final Map<String, String> posAmounts;
  final String? totalAmount;
  final String? cashAccountId;
  final String? cashCategoryId;

  /// Kaydın anahtarı ([DayCloseExistingRecord.key]) → işaretli mi.
  final Map<String, bool> recordOverrides;

  /// Aynı günün ikinci gün sonu (ikinci cihaz).
  final bool isAdditional;

  Map<String, Object?> toJson({String? clientRequestId}) => {
    'date': date,
    'cashAmount': cashAmount,
    'posAmounts': [
      for (final entry in posAmounts.entries)
        {'posDefinitionId': entry.key, 'amount': entry.value},
    ],
    'totalAmount': totalAmount,
    'cashAccountId': cashAccountId,
    'cashCategoryId': cashCategoryId,
    'recordOverrides': [
      for (final entry in recordOverrides.entries)
        {
          'kind': entry.key.split('/').first,
          'id': entry.key.split('/').last,
          'included': entry.value,
        },
    ],
    'isAdditional': isAdditional,
    'clientRequestId': ?clientRequestId,
  };
}

/// O gün tek tek girilmiş ve gün sonu tutarının içinde olabilecek bir kayıt.
class DayCloseExistingRecord {
  const DayCloseExistingRecord({
    required this.kind,
    required this.id,
    required this.isCash,
    required this.date,
    required this.amount,
    required this.title,
    required this.accountName,
    required this.includedByDefault,
    required this.included,
    this.posDefinitionId,
    this.isCardCollection = false,
  });

  factory DayCloseExistingRecord.fromJson(Map<String, dynamic> json) =>
      DayCloseExistingRecord(
        kind: JsonReaders.string(json, 'kind'),
        id: JsonReaders.string(json, 'id'),
        isCash: JsonReaders.string(json, 'side') == 'cash',
        date: JsonReaders.date(json, 'date'),
        amount: JsonReaders.money(json, 'amount'),
        title: JsonReaders.string(json, 'title'),
        posDefinitionId: JsonReaders.nullableString(json, 'posDefinitionId'),
        accountName: JsonReaders.string(json, 'accountName'),
        includedByDefault: JsonReaders.boolean(json, 'includedByDefault'),
        included: JsonReaders.boolean(json, 'included'),
        isCardCollection: json['isCardCollection'] == true,
      );

  /// Kart tarafında bir alacağın kartla tahsili (KP7, KP13): satış değildir
  /// ama yazar kasanın KART satırındadır; başlık kişinin adıdır.

  /// `income`, `pos-settlement`, `counterparty-payment`,
  /// `obligation-settlement`.
  final String kind;
  final String id;

  /// Nakit tarafından mı düşülür, kart tarafından mı.
  final bool isCash;
  final String date;
  final String amount;

  /// Kullanıcının yazdığı ad, yoksa kategori ya da kişi; boş olabilir.
  final String title;
  final String? posDefinitionId;
  final bool isCardCollection;
  final String accountName;
  final bool includedByDefault;
  final bool included;

  String get key => '$kind/$id';
}

/// Panelin nakit satırı.
class DayCloseCashLine {
  const DayCloseCashLine({
    required this.stated,
    required this.enteredAmount,
    required this.isComputed,
    required this.deductedAmount,
    required this.amountToWrite,
    this.accountId,
    this.accountName,
    this.categoryId,
    this.categoryName,
  });

  factory DayCloseCashLine.fromJson(Map<String, dynamic> json) =>
      DayCloseCashLine(
        stated: JsonReaders.boolean(json, 'stated'),
        enteredAmount: JsonReaders.money(json, 'enteredAmount'),
        isComputed: JsonReaders.boolean(json, 'isComputed'),
        deductedAmount: JsonReaders.money(json, 'deductedAmount'),
        amountToWrite: JsonReaders.money(json, 'amountToWrite'),
        accountId: JsonReaders.nullableString(json, 'accountId'),
        accountName: JsonReaders.nullableString(json, 'accountName'),
        categoryId: JsonReaders.nullableString(json, 'categoryId'),
        categoryName: JsonReaders.nullableString(json, 'categoryName'),
      );

  /// Nakit yazıldı ya da toplamdan hesaplandı; değilse nakit tarafına
  /// dokunulmaz.
  final bool stated;
  final String enteredAmount;

  /// Tutar kullanıcıdan değil, toplamdan geldi.
  final bool isComputed;
  final String deductedAmount;
  final String amountToWrite;

  /// Nakit satışın yazılacağı kasa ve kategori; sunucu çözemediyse `null`.
  final String? accountId;
  final String? accountName;
  final String? categoryId;
  final String? categoryName;

  bool get writes => !isZeroMoney(amountToWrite);
}

/// Panelin bir POS satırı.
class DayClosePosLine {
  const DayClosePosLine({
    required this.posDefinitionId,
    required this.name,
    required this.isDefault,
    required this.accountName,
    required this.stated,
    required this.enteredAmount,
    required this.isComputed,
    required this.deductedAmount,
    required this.amountToWrite,
    required this.commissionAmount,
    required this.netAmount,
    required this.expectedTransferDate,
  });

  factory DayClosePosLine.fromJson(Map<String, dynamic> json) =>
      DayClosePosLine(
        posDefinitionId: JsonReaders.string(json, 'posDefinitionId'),
        name: JsonReaders.string(json, 'name'),
        isDefault: JsonReaders.boolean(json, 'isDefault'),
        accountName: JsonReaders.string(json, 'accountName'),
        stated: JsonReaders.boolean(json, 'stated'),
        enteredAmount: JsonReaders.money(json, 'enteredAmount'),
        isComputed: JsonReaders.boolean(json, 'isComputed'),
        deductedAmount: JsonReaders.money(json, 'deductedAmount'),
        amountToWrite: JsonReaders.money(json, 'amountToWrite'),
        commissionAmount: JsonReaders.money(json, 'commissionAmount'),
        netAmount: JsonReaders.money(json, 'netAmount'),
        expectedTransferDate: JsonReaders.date(json, 'expectedTransferDate'),
      );

  final String posDefinitionId;
  final String name;
  final bool isDefault;
  final String accountName;
  final bool stated;
  final String enteredAmount;
  final bool isComputed;
  final String deductedAmount;
  final String amountToWrite;
  final String commissionAmount;
  final String netAmount;
  final String expectedTransferDate;

  bool get writes => !isZeroMoney(amountToWrite);
}

/// Günü kapatan bir gün sonunun kısa hâli.
class DayCloseSummary {
  const DayCloseSummary({
    required this.id,
    required this.closedOn,
    required this.isAdditional,
    this.zNumber,
  });

  factory DayCloseSummary.fromJson(Map<String, dynamic> json) =>
      DayCloseSummary(
        id: JsonReaders.string(json, 'id'),
        closedOn: JsonReaders.date(json, 'closedOn'),
        zNumber: JsonReaders.nullableInt(json, 'zNumber'),
        isAdditional: JsonReaders.boolean(json, 'isAdditional'),
      );

  final String id;
  final String closedOn;
  final int? zNumber;
  final bool isAdditional;
}

/// Gün sonu panelinin sunucudan gelen önizlemesi. Hiçbir tutar istemcide
/// hesaplanmaz.
class DayClosePreview {
  const DayClosePreview({
    required this.date,
    required this.currency,
    required this.closedBy,
    required this.cash,
    required this.posLines,
    required this.totalComputed,
    required this.existingRecords,
    this.totalEntered,
    this.totalDifference,
    this.blockerCode,
  });

  factory DayClosePreview.fromJson(Map<String, dynamic> json) =>
      DayClosePreview(
        date: JsonReaders.date(json, 'date'),
        currency: JsonReaders.string(json, 'currency'),
        closedBy: _objects(
          json,
          'closedBy',
        ).map(DayCloseSummary.fromJson).toList(growable: false),
        cash: DayCloseCashLine.fromJson(
          JsonReaders.object(json['cash'], 'cash'),
        ),
        posLines: _objects(
          json,
          'posLines',
        ).map(DayClosePosLine.fromJson).toList(growable: false),
        totalEntered: JsonReaders.nullableString(json, 'totalEntered'),
        totalComputed: JsonReaders.money(json, 'totalComputed'),
        totalDifference: JsonReaders.nullableString(json, 'totalDifference'),
        existingRecords: _objects(
          json,
          'existingRecords',
        ).map(DayCloseExistingRecord.fromJson).toList(growable: false),
        blockerCode: JsonReaders.nullableString(json, 'blockerCode'),
      );

  final String date;
  final String currency;

  /// Bu günü zaten kapatmış gün sonları; boşsa gün açık.
  final List<DayCloseSummary> closedBy;
  final DayCloseCashLine cash;
  final List<DayClosePosLine> posLines;
  final String? totalEntered;
  final String totalComputed;

  /// Yazılan toplam ile nakit + kart arasındaki fark; tutuyorsa `null`.
  final String? totalDifference;
  final List<DayCloseExistingRecord> existingRecords;

  /// Bu girdiyle kayıt reddedilecekse sunucunun hata kodu.
  final String? blockerCode;

  bool get isClosed => closedBy.isNotEmpty;
}

/// Gün sonunun ürettiği nakit gelir.
class DayCloseIncome {
  const DayCloseIncome({
    required this.transactionId,
    required this.accountName,
    required this.categoryName,
    required this.amount,
    required this.date,
    required this.isCancelled,
  });

  factory DayCloseIncome.fromJson(Map<String, dynamic> json) => DayCloseIncome(
    transactionId: JsonReaders.string(json, 'transactionId'),
    accountName: JsonReaders.string(json, 'accountName'),
    categoryName: JsonReaders.string(json, 'categoryName'),
    amount: JsonReaders.money(json, 'amount'),
    date: JsonReaders.date(json, 'date'),
    isCancelled: JsonReaders.boolean(json, 'isCancelled'),
  );

  final String transactionId;
  final String accountName;
  final String categoryName;
  final String amount;
  final String date;
  final bool isCancelled;
}

/// Bir gün sonu ve ürettiği kayıtlar. Gün sonu **tutar taşımaz** (ADR 0019
/// İ3); buradaki toplamlar sunucunun kayıtlardan topladığıdır.
class DayClose {
  const DayClose({
    required this.id,
    required this.closedOn,
    required this.isAdditional,
    required this.isCancelled,
    required this.incomes,
    required this.settlements,
    required this.cashAmount,
    required this.cardGrossAmount,
    required this.commissionAmount,
    required this.currency,
    this.zNumber,
    this.countedRecords = const [],
    this.countedCashAmount = '0.0000',
    this.countedCardAmount = '0.0000',
  });

  factory DayClose.fromJson(Map<String, dynamic> json) => DayClose(
    id: JsonReaders.string(json, 'id'),
    closedOn: JsonReaders.date(json, 'closedOn'),
    zNumber: JsonReaders.nullableInt(json, 'zNumber'),
    isAdditional: JsonReaders.boolean(json, 'isAdditional'),
    isCancelled: JsonReaders.boolean(json, 'isCancelled'),
    incomes: _objects(
      json,
      'incomes',
    ).map(DayCloseIncome.fromJson).toList(growable: false),
    settlements: _objects(
      json,
      'settlements',
    ).map(PosSettlementItem.fromJson).toList(growable: false),
    cashAmount: JsonReaders.money(json, 'cashAmount'),
    cardGrossAmount: JsonReaders.money(json, 'cardGrossAmount'),
    commissionAmount: JsonReaders.money(json, 'commissionAmount'),
    currency: JsonReaders.string(json, 'currency'),
    countedRecords: _objects(
      json,
      'countedRecords',
    ).map(DayCloseExistingRecord.fromJson).toList(growable: false),
    countedCashAmount: JsonReaders.money(json, 'countedCashAmount'),
    countedCardAmount: JsonReaders.money(json, 'countedCardAmount'),
  );

  /// Gün sonunun saydığı, tek tek girilmiş kayıtlar: tutardan düşüldüler ve
  /// gün sonu geri alınana kadar tek başlarına iptal edilemezler. Geri
  /// alınmış gün sonunda boştur.
  final List<DayCloseExistingRecord> countedRecords;
  final String countedCashAmount;
  final String countedCardAmount;

  final String id;
  final String closedOn;
  final int? zNumber;
  final bool isAdditional;

  /// Geri alınmış gün sonu: kayıt durur, ürettiği kayıtlar iptal edilmiştir.
  final bool isCancelled;
  final List<DayCloseIncome> incomes;
  final List<PosSettlementItem> settlements;
  final String cashAmount;
  final String cardGrossAmount;
  final String commissionAmount;
  final String currency;

  int get recordCount => incomes.length + settlements.length;
}

/// Bir günün bütünü: ana ve ek gün sonları (ilk giren üstte), dışarıda kalan
/// tek tek girilmiş kayıtlar ve günün toplamı. Toplam sunucudan gelir:
/// yazılan ile sayılanın toplamı, yani kullanıcının o gün için yazdığı tutar.
class DayCloseDay {
  const DayCloseDay({
    required this.date,
    required this.closes,
    required this.outsideRecords,
    required this.cashTotal,
    required this.cardTotal,
    required this.currency,
  });

  factory DayCloseDay.fromJson(Map<String, dynamic> json) => DayCloseDay(
    date: JsonReaders.date(json, 'date'),
    closes: _objects(
      json,
      'closes',
    ).map(DayClose.fromJson).toList(growable: false),
    outsideRecords: _objects(
      json,
      'outsideRecords',
    ).map(DayCloseExistingRecord.fromJson).toList(growable: false),
    cashTotal: JsonReaders.money(json, 'cashTotal'),
    cardTotal: JsonReaders.money(json, 'cardTotal'),
    currency: JsonReaders.string(json, 'currency'),
  );

  final String date;
  final List<DayClose> closes;

  /// O gün tek tek girilmiş ama hiçbir gün sonunda sayılmamış kayıtlar.
  final List<DayCloseExistingRecord> outsideRecords;
  final String cashTotal;
  final String cardTotal;
  final String currency;

  bool get isClosed => closes.isNotEmpty;
}

/// Gün sonu panelinin seçenekleri: nakit hesaplar ve gelir kategorileri.
class DayCloseOptions {
  const DayCloseOptions({required this.cashAccounts, required this.categories});

  final List<DataChoice> cashAccounts;
  final List<DataChoice> categories;
}

bool isZeroMoney(String money) =>
    !money.replaceAll(RegExp('[^0-9]'), '').contains(RegExp('[1-9]'));

List<Map<String, dynamic>> _objects(Map<String, dynamic> json, String key) =>
    JsonReaders.list(
      json,
      key,
    ).map((item) => JsonReaders.object(item, key)).toList(growable: false);

abstract interface class DayCloseRepositoryContract {
  /// Yazılacak tutarlar, düşülen kayıtlar, komisyon ve beklenen gün. Hiçbir
  /// şey yazmaz.
  Future<DayClosePreview> preview(DayCloseInput input);

  /// Gün sonunu yazar. [clientRequestId] aynı isteğin ikinci kez yazılmasını
  /// önler.
  Future<DayClose> create({
    required String clientRequestId,
    required DayCloseInput input,
  });

  /// Verilen aralıktaki günleri kapatan, geri alınmamış gün sonları.
  Future<List<DayClose>> list({required String from, required String to});

  /// Bir günün bütünü: gün sonları, yazdıkları, saydıkları ve dışarıda
  /// kalanlar.
  Future<DayCloseDay> day({required String date});

  Future<DayClose> get({required String dayCloseId});

  /// Gün sonunu bir bütün olarak geri alır: ürettiği kayıtlar iptal olur.
  Future<DayClose> revert({required String dayCloseId});

  Future<DayCloseOptions> loadOptions();
}

class DayCloseRepository implements DayCloseRepositoryContract {
  const DayCloseRepository(this._client);

  static const _path = '/api/v1/day-closes';

  final ApiClient _client;

  @override
  Future<DayClosePreview> preview(DayCloseInput input) async {
    final response = await _client.post('$_path/preview', body: input.toJson());
    return DayClosePreview.fromJson(response.requireObject());
  }

  @override
  Future<DayClose> create({
    required String clientRequestId,
    required DayCloseInput input,
  }) async {
    final response = await _client.post(
      _path,
      body: input.toJson(clientRequestId: clientRequestId),
    );
    return DayClose.fromJson(response.requireObject());
  }

  @override
  Future<List<DayClose>> list({
    required String from,
    required String to,
  }) async {
    final response = await _client.get('$_path?from=$from&to=$to');
    return _objects(
      response.requireObject(),
      'items',
    ).map(DayClose.fromJson).toList(growable: false);
  }

  @override
  Future<DayCloseDay> day({required String date}) async {
    final response = await _client.get('$_path/day?date=$date');
    return DayCloseDay.fromJson(response.requireObject());
  }

  @override
  Future<DayClose> get({required String dayCloseId}) async {
    final response = await _client.get('$_path/$dayCloseId');
    return DayClose.fromJson(response.requireObject());
  }

  @override
  Future<DayClose> revert({required String dayCloseId}) async {
    final response = await _client.delete('$_path/$dayCloseId');
    return DayClose.fromJson(response.requireObject());
  }

  @override
  Future<DayCloseOptions> loadOptions() async {
    final responses = await Future.wait([
      _client.get(
        '/api/v1/accounts?isActive=true&type=cash&pageNumber=1&pageSize=100',
      ),
      _client.get('/api/v1/categories?type=income&isActive=true'),
    ]);
    return DayCloseOptions(
      cashAccounts: _objects(
        responses[0].requireObject(),
        'items',
      ).map(DataChoice.fromJson).toList(growable: false),
      categories: _objects(
        responses[1].requireObject(),
        'items',
      ).map(DataChoice.categoryFromJson).toList(growable: false),
    );
  }
}
