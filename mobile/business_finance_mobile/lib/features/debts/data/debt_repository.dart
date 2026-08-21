import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import 'debt_models.dart';

abstract interface class DebtRepositoryContract {
  Future<DebtsSnapshot> load(String asOfDate);
  Future<void> create(Map<String, Object?> input);
  Future<void> recordOpening(String debtId, Map<String, Object?> input);
  Future<void> pay(String debtId, int sequence, Map<String, Object?> input);
}

class DebtRepository implements DebtRepositoryContract {
  const DebtRepository(this._client);
  final ApiClient _client;

  @override
  Future<DebtsSnapshot> load(String asOfDate) async {
    final responses = await Future.wait([
      _client.get('/api/v1/debts?asOfDate=$asOfDate'),
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
      _client.get('/api/v1/categories?isActive=true'),
    ]);
    return DebtsSnapshot(
      debts: _items(
        responses[0].requireObject(),
      ).map(DebtItem.fromJson).toList(growable: false),
      accounts: _items(
        responses[1].requireObject(),
      ).map(DataChoice.fromJson).toList(growable: false),
      categories: _items(
        responses[2].requireObject(),
      ).map(DataChoice.categoryFromJson).toList(growable: false),
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async =>
      _client.post('/api/v1/debts', body: input);

  @override
  Future<void> recordOpening(String debtId, Map<String, Object?> input) async =>
      _client.post('/api/v1/debts/$debtId/opening', body: input);

  @override
  Future<void> pay(
    String debtId,
    int sequence,
    Map<String, Object?> input,
  ) async => _client.post(
    '/api/v1/debts/$debtId/installments/$sequence/pay',
    body: input,
  );

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
