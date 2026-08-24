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

abstract interface class ObligationRepositoryContract {
  Future<ObligationOptions> loadPayableOptions();

  Future<void> create(Map<String, Object?> input);
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

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
