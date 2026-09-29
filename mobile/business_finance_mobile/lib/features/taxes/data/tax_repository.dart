import '../../../core/network/api_client.dart';
import 'tax_models.dart';

abstract interface class TaxRepositoryContract {
  Future<List<TaxCalendarSuggestion>> loadSuggestions();
}

class TaxRepository implements TaxRepositoryContract {
  const TaxRepository(this._client);

  final ApiClient _client;

  @override
  Future<List<TaxCalendarSuggestion>> loadSuggestions() async {
    final response = await _client.get('/api/v1/tax-calendar/suggestions');
    final items = response.requireObject()['items'] as List<dynamic>;
    return items
        .map(
          (item) =>
              TaxCalendarSuggestion.fromJson(item as Map<String, dynamic>),
        )
        .toList(growable: false);
  }
}
