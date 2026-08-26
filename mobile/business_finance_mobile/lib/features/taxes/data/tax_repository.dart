import '../../../core/network/api_client.dart';
import 'tax_models.dart';

abstract interface class TaxRepositoryContract {
  Future<List<TaxCalendarSuggestion>> loadSuggestions();
  Future<AccountantPackage> loadPackage(int year, int month);
  Future<ApiBinaryResponse> downloadPackage(int year, int month);
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

  @override
  Future<AccountantPackage> loadPackage(int year, int month) async {
    final response = await _client.get(
      '/api/v1/accountant-package?year=$year&month=$month',
    );
    return AccountantPackage.fromJson(response.requireObject());
  }

  /// Paketin tek dosyası.
  ///
  /// Dosya kullanıcının cihazına iner ve oradan paylaşılır; sunucu üçüncü
  /// kişiye hiçbir şey göndermez.
  @override
  Future<ApiBinaryResponse> downloadPackage(int year, int month) =>
      _client.getBytes(
        '/api/v1/exports/accountant-package.zip?year=$year&month=$month',
      );
}
