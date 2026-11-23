import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import '../../activities/data/activity_models.dart';
import 'debt_book_models.dart';

/// `Borç ve alacak` sayfalarının sunucu okumaları ve yazmaları
/// (`documentation/debt-api-contract.md`).
abstract interface class DebtBookRepositoryContract {
  /// `GET /api/v1/debt-book`: toplamlar, satırlar ve kişiler; kapsam boşsa
  /// hepsi.
  Future<DebtBook> load({required String asOfDate, TransactionScope? scope});

  /// `GET /api/v1/counterparties/{id}/page`: kişinin özeti.
  Future<CounterpartyPageData> loadPerson(
    String counterpartyId, {
    required String asOfDate,
  });

  /// Kişinin hareketleri (akıştan): yön, tür ve gün aralığıyla süzülür.
  Future<ActivityPage> loadMovements(
    String counterpartyId, {
    required PersonMovementFilter filter,
    required DateTime today,
    int pageNumber = 1,
    int pageSize = 20,
  });

  /// Tahsilat ve ödeme panelinin yerleri: hesaplar, POS'lar, kredi kartları.
  Future<DebtSettleSources> loadSettleSources();

  /// `POST /api/v1/counterparties/{id}/payments`.
  Future<void> settle(String counterpartyId, Map<String, Object?> body);

  /// `POST /api/v1/counterparty-charges/{id}/discounts`.
  Future<void> discountCharge(String chargeId, Map<String, Object?> body);
}

class DebtBookRepository implements DebtBookRepositoryContract {
  const DebtBookRepository(this._client);

  final ApiClient _client;

  @override
  Future<DebtBook> load({
    required String asOfDate,
    TransactionScope? scope,
  }) async {
    final query = StringBuffer('asOfDate=$asOfDate');
    if (scope != null) query.write('&scope=${scope.apiValue}');
    final response = await _client.get('/api/v1/debt-book?$query');
    return DebtBook.fromJson(response.requireObject());
  }

  @override
  Future<CounterpartyPageData> loadPerson(
    String counterpartyId, {
    required String asOfDate,
  }) async {
    final response = await _client.get(
      '/api/v1/counterparties/$counterpartyId/page?asOfDate=$asOfDate',
    );
    return CounterpartyPageData.fromJson(response.requireObject());
  }

  @override
  Future<ActivityPage> loadMovements(
    String counterpartyId, {
    required PersonMovementFilter filter,
    required DateTime today,
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    final range = filter.period.range(today);
    final query = <String, String>{
      'pageNumber': '$pageNumber',
      'pageSize': '$pageSize',
      'counterpartyId': counterpartyId,
      'direction': ?filter.side?.apiValue,
      'activityKind': ?filter.kind.apiValue,
      'dateFrom': ?range?.from,
      'dateTo': ?range?.to,
    };
    final response = await _client.get(
      Uri(
        path: '/api/v1/financial-activities',
        queryParameters: query,
      ).toString(),
    );
    return ActivityPage.fromJson(response.requireObject());
  }

  @override
  Future<DebtSettleSources> loadSettleSources() async {
    final responses = await Future.wait([
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
      _client.get('/api/v1/credit-cards'),
      _client.get('/api/v1/pos-definitions'),
    ]);
    List<Map<String, dynamic>> items(Map<String, dynamic> json) => [
      for (final item in json['items'] is List ? json['items'] as List : [])
        if (item is Map<String, dynamic>) item,
    ];
    final cards = responses[1].requireObject();
    final pos = responses[2].requireObject();
    return DebtSettleSources(
      accounts: [
        for (final item in items(responses[0].requireObject()))
          DebtSettleSource(
            id: item['id'] as String,
            name: item['name'] as String,
            type: item['type'] as String?,
          ),
      ],
      cards: [
        for (final item in items(cards))
          if (item['isActive'] != false)
            DebtSettleSource(
              id: item['id'] as String,
              name: item['name'] as String,
            ),
      ],
      posDefinitions: [
        for (final item in items(pos))
          if (item['isActive'] != false)
            DebtSettleSource(
              id: item['id'] as String,
              name: item['name'] as String,
              note: item['accountName'] as String?,
            ),
      ],
    );
  }

  @override
  Future<void> discountCharge(
    String chargeId,
    Map<String, Object?> body,
  ) async => _client.post(
    '/api/v1/counterparty-charges/$chargeId/discounts',
    body: body,
  );

  @override
  Future<void> settle(String counterpartyId, Map<String, Object?> body) async =>
      _client.post(
        '/api/v1/counterparties/$counterpartyId/payments',
        body: body,
      );
}
