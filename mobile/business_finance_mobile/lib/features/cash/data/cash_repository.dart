import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';

/// Bir gün sonu sayımı.
///
/// [expectedBalance] ve [difference] **yalnız günün açık sayımında** dolu
/// gelir. Geçmiş bir sayımda boştur: o günün farkını bugünkü bakiyeye karşı
/// yeniden hesaplamak, aradaki bütün hareketleri o günün farkına yazmak
/// olurdu. İstemci farkı kendisi hesaplamaz; sunucunun cevabını gösterir.
class CashCountItem {
  const CashCountItem({
    required this.id,
    required this.accountId,
    required this.accountName,
    required this.countDate,
    required this.countedAmount,
    required this.currency,
    required this.isCancelled,
    this.note,
    this.adjustmentTransactionId,
    this.expectedBalance,
    this.difference,
  });

  final String id;
  final String accountId;
  final String accountName;
  final String countDate;
  final String countedAmount;
  final String currency;
  final bool isCancelled;
  final String? note;
  final String? adjustmentTransactionId;
  final String? expectedBalance;
  final String? difference;

  bool get isAdjusted => adjustmentTransactionId != null;

  factory CashCountItem.fromJson(Map<String, dynamic> json) => CashCountItem(
    id: JsonReaders.string(json, 'id'),
    accountId: JsonReaders.string(json, 'accountId'),
    accountName: JsonReaders.string(json, 'accountName'),
    countDate: JsonReaders.date(json, 'countDate'),
    countedAmount: JsonReaders.money(json, 'countedAmount'),
    currency: JsonReaders.string(json, 'currency'),
    isCancelled: JsonReaders.boolean(json, 'isCancelled'),
    note: JsonReaders.nullableString(json, 'note'),
    adjustmentTransactionId: JsonReaders.nullableString(
      json,
      'adjustmentTransactionId',
    ),
    expectedBalance: JsonReaders.nullableString(json, 'expectedBalance'),
    difference: JsonReaders.nullableString(json, 'difference'),
  );
}

/// Kasa ekranının açılışta sorduğu tek soru: bugün ne olmalıydı, ne sayıldı.
class CashCountToday {
  const CashCountToday({
    required this.accountId,
    required this.accountName,
    required this.expectedBalance,
    required this.currency,
    this.count,
    this.previousCount,
    this.todayInflow,
    this.todayOutflow,
    this.changeSinceCount,
  });

  final String accountId;
  final String accountName;
  final String expectedBalance;
  final String currency;
  final CashCountItem? count;

  /// Bugünden önceki son sayım; beklenen tutarın başlangıç noktası.
  final CashCountItem? previousCount;

  /// Bugün kasaya giren ve çıkan nakit; sunucuda toplanır. Eski sunucu
  /// göndermezse `null`.
  final String? todayInflow;
  final String? todayOutflow;

  /// Bugünkü sayımdan bu yana kasa bakiyesindeki değişim; sunucu hesaplar.
  /// Sayım yoksa ya da bilinmiyorsa `null`. Sıfırdan farklıysa sayımdan sonra
  /// kasaya hareket girmiştir ve ekran "oturdu" diyemez.
  final String? changeSinceCount;

  factory CashCountToday.fromJson(Map<String, dynamic> json) => CashCountToday(
    accountId: JsonReaders.string(json, 'accountId'),
    accountName: JsonReaders.string(json, 'accountName'),
    expectedBalance: JsonReaders.money(json, 'expectedBalance'),
    currency: JsonReaders.string(json, 'currency'),
    count: json['count'] == null
        ? null
        : CashCountItem.fromJson(JsonReaders.object(json['count'], 'count')),
    previousCount: json['previousCount'] == null
        ? null
        : CashCountItem.fromJson(
            JsonReaders.object(json['previousCount'], 'previousCount'),
          ),
    todayInflow: json['todayInflow'] is String
        ? JsonReaders.money(json, 'todayInflow')
        : null,
    todayOutflow: json['todayOutflow'] is String
        ? JsonReaders.money(json, 'todayOutflow')
        : null,
    changeSinceCount: json['changeSinceCount'] is String
        ? JsonReaders.money(json, 'changeSinceCount')
        : null,
  );
}

class CashAccount {
  const CashAccount({required this.id, required this.name, this.defaultScope});

  final String id;
  final String name;

  /// Hesabın etiketi; kapsam zincirinin ikinci halkası. Boş olması meşrudur.
  final TransactionScope? defaultScope;
}

abstract interface class CashRepositoryContract {
  /// Yalnız nakit hesaplar: banka bakiyesi elle sayılmaz.
  Future<List<CashAccount>> loadCashAccounts();

  Future<CashCountToday> loadToday({required String accountId});

  Future<List<CashCountItem>> list({required String accountId});

  Future<CashCountItem> create({
    required String accountId,
    required String countedAmount,
    required String countDate,
    String? scope,
    String? note,
  });

  /// Fark için gelir ya da gider kategorisi; hangisinin isteneceğini farkın
  /// yönü belirler.
  Future<List<DataChoice>> loadCategories({required String type});

  Future<CashCountItem> confirmDifference({
    required String cashCountId,
    required String categoryId,
  });
}

class CashRepository implements CashRepositoryContract {
  const CashRepository(this._client);

  final ApiClient _client;

  @override
  Future<List<CashAccount>> loadCashAccounts() async {
    final response = await _client.get(
      '/api/v1/accounts?isActive=true&type=cash&pageNumber=1&pageSize=100',
    );
    return _items(response.requireObject())
        .map(
          (item) => CashAccount(
            id: JsonReaders.string(item, 'id'),
            name: JsonReaders.string(item, 'name'),
            defaultScope: TransactionScope.fromApiOrNull(item['defaultScope']),
          ),
        )
        .toList(growable: false);
  }

  @override
  Future<CashCountToday> loadToday({required String accountId}) async {
    final response = await _client.get(
      '/api/v1/cash-counts/today?accountId=$accountId',
    );
    return CashCountToday.fromJson(response.requireObject());
  }

  @override
  Future<List<CashCountItem>> list({required String accountId}) async {
    final response = await _client.get(
      '/api/v1/cash-counts?accountId=$accountId',
    );
    return _items(
      response.requireObject(),
    ).map(CashCountItem.fromJson).toList(growable: false);
  }

  @override
  Future<CashCountItem> create({
    required String accountId,
    required String countedAmount,
    required String countDate,
    String? scope,
    String? note,
  }) async {
    final response = await _client.post(
      '/api/v1/cash-counts',
      body: {
        'accountId': accountId,
        'countedAmount': countedAmount,
        'countDate': countDate,
        'scope': scope,
        'note': note,
      },
    );
    return CashCountItem.fromJson(response.requireObject());
  }

  @override
  Future<List<DataChoice>> loadCategories({required String type}) async {
    final response = await _client.get(
      '/api/v1/categories?type=$type&isActive=true',
    );
    return _items(
      response.requireObject(),
    ).map(DataChoice.categoryFromJson).toList(growable: false);
  }

  @override
  Future<CashCountItem> confirmDifference({
    required String cashCountId,
    required String categoryId,
  }) async {
    final response = await _client.post(
      '/api/v1/cash-counts/$cashCountId/adjustment',
      body: {'categoryId': categoryId},
    );
    return CashCountItem.fromJson(response.requireObject());
  }

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
