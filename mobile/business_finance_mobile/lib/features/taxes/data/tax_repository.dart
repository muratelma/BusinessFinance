import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import 'tax_models.dart';

/// Vergi takibinin sunucu kapısı (ADR 0018, sözleşme
/// `financial-activity-api-contract.md` §5).
///
/// Vergi ayrı bir kayıt türü değildir: tanım bir tekrarlayan plandır, ödeme
/// bir gider ya da kart harcamasıdır. Uçlar da bu yüzden tekrarlayan plan
/// uçlarıyla vergi okuma uçlarının karışımıdır.
abstract interface class TaxRepositoryContract {
  Future<TaxOverview> loadOverview({required String asOfDate});
  Future<TaxOptions> loadOptions();
  Future<List<TaxSuggestion>> loadSuggestions();
  Future<TaxPlanDetail> loadPlanDetail(
    String planId, {
    required String asOfDate,
  });
  Future<TaxPaymentPage> listPayments({required int skip, required int take});

  /// "Ödedim": kalemi ödeme gününe, seçilen hesap ya da karttan öder.
  Future<void> pay({
    required String planId,
    required String scheduledDate,
    required String amount,
    required String paidOn,
    String? accountId,
    String? creditCardId,
  });

  /// "Tutar belli oldu": bekleyen kaleme yazar, plan değişmez.
  Future<void> setAmount({
    required String planId,
    required String scheduledDate,
    required String amount,
  });

  /// "Vergi ödemesi ekle": tek tutar, isteğe bağlı kapattığı kalemler.
  Future<TaxPayment> createPayment({
    required String clientRequestId,
    required String amount,
    required String paidOn,
    required String categoryId,
    required List<({String planId, String scheduledDate})> closes,
    String? accountId,
    String? creditCardId,
    String? note,
  });

  /// "Ödedim"i ya da toplu ödemeyi geri alır; sunucu doğru yolu seçer.
  Future<void> undoPayment(String paymentId);

  Future<void> createPlan(TaxPlanInput input);

  /// İlk kurulumdaki çoklu seçim: hepsi yazılır ya da hiçbiri.
  Future<void> createPlans(List<TaxPlanInput> inputs);
  Future<void> updatePlan(String planId, TaxPlanInput input);
  Future<void> setPlanActive(String planId, bool isActive);
  Future<void> deletePlan(String planId);
}

class TaxRepository implements TaxRepositoryContract {
  const TaxRepository(this._client);

  final ApiClient _client;

  @override
  Future<TaxOverview> loadOverview({required String asOfDate}) async {
    final query = Uri(
      queryParameters: {'asOfDate': asOfDate, 'daysAhead': '30'},
    ).query;
    final response = await _client.get('/api/v1/taxes?$query');
    return TaxOverview.fromJson(response.requireObject());
  }

  @override
  Future<TaxOptions> loadOptions() async {
    final responses = await Future.wait([
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
      _client.get('/api/v1/credit-cards'),
      _client.get('/api/v1/categories?type=expense&isActive=true'),
    ]);
    return TaxOptions(
      accounts: [
        for (final item in _items(responses[0].requireObject()))
          TaxChoice(
            id: JsonReaders.string(item, 'id'),
            name: JsonReaders.string(item, 'name'),
          ),
      ],
      cards: [
        for (final item in _items(responses[1].requireObject()))
          if (item['isActive'] == true)
            TaxChoice(
              id: JsonReaders.string(item, 'id'),
              name: JsonReaders.string(item, 'name'),
            ),
      ],
      taxCategories: [
        for (final item in _items(responses[2].requireObject()))
          if (item['isTax'] == true)
            TaxChoice(
              id: JsonReaders.string(item, 'id'),
              name: JsonReaders.string(item, 'name'),
            ),
      ],
    );
  }

  @override
  Future<List<TaxSuggestion>> loadSuggestions() async {
    final response = await _client.get('/api/v1/tax-calendar/suggestions');
    return [
      for (final item in _items(response.requireObject()))
        TaxSuggestion.fromJson(item),
    ];
  }

  @override
  Future<TaxPlanDetail> loadPlanDetail(
    String planId, {
    required String asOfDate,
  }) async {
    final query = Uri(queryParameters: {'asOfDate': asOfDate}).query;
    final response = await _client.get('/api/v1/taxes/plans/$planId?$query');
    return TaxPlanDetail.fromJson(response.requireObject());
  }

  @override
  Future<TaxPaymentPage> listPayments({
    required int skip,
    required int take,
  }) async {
    final response = await _client.get(
      '/api/v1/tax-payments?skip=$skip&take=$take',
    );
    final json = response.requireObject();
    return TaxPaymentPage(
      items: [for (final item in _items(json)) TaxPayment.fromJson(item)],
      hasMore: JsonReaders.boolean(json, 'hasMore'),
    );
  }

  @override
  Future<void> pay({
    required String planId,
    required String scheduledDate,
    required String amount,
    required String paidOn,
    String? accountId,
    String? creditCardId,
  }) async => _client.post(
    '/api/v1/recurring-transactions/$planId/occurrences/realize',
    body: {
      'scheduledDate': scheduledDate,
      'amount': amount,
      'paidOn': paidOn,
      'accountId': accountId,
      'creditCardId': creditCardId,
    },
  );

  @override
  Future<void> setAmount({
    required String planId,
    required String scheduledDate,
    required String amount,
  }) async => _client.post(
    '/api/v1/recurring-transactions/$planId/occurrences/amount',
    body: {'scheduledDate': scheduledDate, 'amount': amount},
  );

  @override
  Future<TaxPayment> createPayment({
    required String clientRequestId,
    required String amount,
    required String paidOn,
    required String categoryId,
    required List<({String planId, String scheduledDate})> closes,
    String? accountId,
    String? creditCardId,
    String? note,
  }) async {
    final response = await _client.post(
      '/api/v1/tax-payments',
      body: {
        'clientRequestId': clientRequestId,
        'amount': amount,
        'paidOn': paidOn,
        'categoryId': categoryId,
        'accountId': accountId,
        'creditCardId': creditCardId,
        'note': note,
        'closes': [
          for (final item in closes)
            {
              'recurringTransactionId': item.planId,
              'scheduledDate': item.scheduledDate,
            },
        ],
      },
    );
    return TaxPayment.fromJson(response.requireObject());
  }

  @override
  Future<void> undoPayment(String paymentId) async =>
      _client.post('/api/v1/tax-payments/$paymentId/undo');

  @override
  Future<void> createPlan(TaxPlanInput input) async => _client.post(
    '/api/v1/recurring-transactions',
    body: input.toCreateJson(),
  );

  @override
  Future<void> createPlans(List<TaxPlanInput> inputs) async => _client.post(
    '/api/v1/taxes/plans',
    body: {
      'items': [for (final input in inputs) input.toCreateJson()],
    },
  );

  @override
  Future<void> updatePlan(String planId, TaxPlanInput input) async =>
      _client.put(
        '/api/v1/recurring-transactions/$planId',
        body: input.toUpdateJson(),
      );

  @override
  Future<void> setPlanActive(String planId, bool isActive) async =>
      _client.patch(
        '/api/v1/recurring-transactions/$planId/active',
        body: {'isActive': isActive},
      );

  @override
  Future<void> deletePlan(String planId) async =>
      _client.delete('/api/v1/recurring-transactions/$planId');

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) => [
    for (final item in JsonReaders.list(json, 'items'))
      JsonReaders.object(item, 'item'),
  ];
}
