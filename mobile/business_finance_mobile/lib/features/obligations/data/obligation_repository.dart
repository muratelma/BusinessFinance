import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';

class ObligationOptions {
  const ObligationOptions({
    required this.categories,
    required this.counterparties,
  });

  final List<DataChoice> categories;
  final List<DataChoice> counterparties;
}

class ObligationItem {
  const ObligationItem({
    required this.id,
    required this.direction,
    required this.amount,
    required this.currency,
    required this.issueDate,
    required this.dueDate,
    required this.status,
    required this.isOverdue,
    this.counterpartyName,
    this.categoryName,
    this.description,
  });

  final String id;
  final String direction;
  final String amount;
  final String currency;
  final String issueDate;
  final String dueDate;
  final String status;
  final bool isOverdue;
  final String? counterpartyName;
  final String? categoryName;
  final String? description;

  factory ObligationItem.fromJson(Map<String, dynamic> json) => ObligationItem(
    id: JsonReaders.string(json, 'id'),
    direction: JsonReaders.string(json, 'direction'),
    amount: JsonReaders.money(json, 'amount'),
    currency: JsonReaders.string(json, 'currency'),
    issueDate: JsonReaders.date(json, 'issueDate'),
    dueDate: JsonReaders.date(json, 'dueDate'),
    status: JsonReaders.string(json, 'status'),
    isOverdue: JsonReaders.boolean(json, 'isOverdue'),
    counterpartyName: JsonReaders.nullableString(json, 'counterpartyName'),
    categoryName: JsonReaders.nullableString(json, 'categoryName'),
    description: JsonReaders.nullableString(json, 'description'),
  );
}

class ObligationAccount {
  const ObligationAccount({required this.id, required this.name});

  final String id;
  final String name;
}

abstract interface class ObligationRepositoryContract {
  Future<ObligationOptions> loadPayableOptions();

  Future<void> create(Map<String, Object?> input);

  Future<List<ObligationItem>> list({required String asOfDate});

  Future<List<ObligationAccount>> loadActiveAccounts();

  Future<void> settle({
    required String obligationId,
    required String accountId,
    required String settlementDate,
  });
}

class ObligationRepository implements ObligationRepositoryContract {
  const ObligationRepository(this._client);

  final ApiClient _client;

  @override
  Future<ObligationOptions> loadPayableOptions() async {
    final responses = await Future.wait([
      _client.get('/api/v1/categories?type=expense&isActive=true'),
      _client.get('/api/v1/counterparties?isActive=true'),
    ]);
    return ObligationOptions(
      categories: _items(
        responses[0].requireObject(),
      ).map(DataChoice.categoryFromJson).toList(growable: false),
      counterparties: _items(responses[1].requireObject())
          .map(
            (item) => DataChoice(
              JsonReaders.string(item, 'id'),
              JsonReaders.string(item, 'name'),
            ),
          )
          .toList(growable: false),
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async {
    await _client.post('/api/v1/obligations', body: input);
  }

  @override
  Future<List<ObligationItem>> list({required String asOfDate}) async {
    final response = await _client.get(
      '/api/v1/obligations?asOfDate=$asOfDate',
    );
    return _items(
      response.requireObject(),
    ).map(ObligationItem.fromJson).toList(growable: false);
  }

  @override
  Future<List<ObligationAccount>> loadActiveAccounts() async {
    final response = await _client.get(
      '/api/v1/accounts?isActive=true&pageNumber=1&pageSize=100',
    );
    return _items(response.requireObject())
        .map(
          (item) => ObligationAccount(
            id: JsonReaders.string(item, 'id'),
            name: JsonReaders.string(item, 'name'),
          ),
        )
        .toList(growable: false);
  }

  @override
  Future<void> settle({
    required String obligationId,
    required String accountId,
    required String settlementDate,
  }) async {
    await _client.post(
      '/api/v1/obligations/$obligationId/settlement',
      body: {'accountId': accountId, 'settlementDate': settlementDate},
    );
  }

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
