import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';

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
    this.transferredOn,
    this.description,
    this.scope,
  });

  final String id;
  final String accountName;
  final String categoryName;
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
        accountName: JsonReaders.string(json, 'accountName'),
        categoryName: JsonReaders.string(json, 'categoryName'),
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
      );
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

  Future<void> markTransferred({
    required String settlementId,
    required String transferDate,
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
  Future<void> markTransferred({
    required String settlementId,
    required String transferDate,
  }) async {
    await _client.post(
      '/api/v1/pos-settlements/$settlementId/transfer',
      body: {'transferDate': transferDate},
    );
  }

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
