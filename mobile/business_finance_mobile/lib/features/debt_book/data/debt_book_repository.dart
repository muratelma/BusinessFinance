import '../../../core/localization/default_category_labels.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import '../../../core/network/api_response.dart';
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

  /// `GET /api/v1/debts/{id}`: taksitli ya da kişisiz kaydın sayfası.
  Future<DebtRecordData> loadRecord(String debtId, {required String asOfDate});

  /// Kayda ödeme ya da tahsilat, indirim, plan değişikliği; geri alma ve
  /// iptal. Hepsi kaydın güncel hâlini döner (iptal hariç).
  Future<void> payRecord(String debtId, Map<String, Object?> body);
  Future<void> reverseRecordPayment(
    String debtId,
    String paymentId, {
    required String asOfDate,
  });
  Future<void> discountRecord(String debtId, Map<String, Object?> body);
  Future<void> changeRecordPlan(String debtId, Map<String, Object?> body);
  Future<void> cancelRecord(String debtId, {required String asOfDate});

  /// Yeni kayıt formu: kategoriler, hesaplar, kartlar ve POS'lar.
  Future<DebtEntryOptions> loadEntryOptions();

  /// Yeni kişi; kimliğini döner.
  Future<String> createCounterparty(String name, {TransactionScope? scope});

  Future<void> createCharge(String counterpartyId, Map<String, Object?> body);
  Future<void> createLoan(String counterpartyId, Map<String, Object?> body);
  Future<void> recordPayment(String counterpartyId, Map<String, Object?> body);

  /// `PUT /api/v1/counterparties/{id}/opening-balance`.
  Future<void> setOpeningBalance(
    String counterpartyId, {
    required String receivable,
    required String payable,
  });

  /// `POST /api/v1/debts`; kaydın kimliğini döner.
  Future<String> createDebt(Map<String, Object?> body);

  Future<DebtSchedulePreview> previewSchedule(Map<String, Object?> body);
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
  Future<DebtRecordData> loadRecord(
    String debtId, {
    required String asOfDate,
  }) async {
    final response = await _client.get(
      '/api/v1/debts/$debtId?asOfDate=$asOfDate',
    );
    return DebtRecordData.fromJson(response.requireObject());
  }

  @override
  Future<void> payRecord(String debtId, Map<String, Object?> body) async =>
      _client.post('/api/v1/debts/$debtId/payments', body: body);

  @override
  Future<void> reverseRecordPayment(
    String debtId,
    String paymentId, {
    required String asOfDate,
  }) async => _client.delete(
    '/api/v1/debts/$debtId/payments/$paymentId?asOfDate=$asOfDate',
  );

  @override
  Future<void> discountRecord(String debtId, Map<String, Object?> body) async =>
      _client.post('/api/v1/debts/$debtId/discounts', body: body);

  @override
  Future<void> changeRecordPlan(
    String debtId,
    Map<String, Object?> body,
  ) async => _client.put('/api/v1/debts/$debtId/plan', body: body);

  @override
  Future<void> cancelRecord(String debtId, {required String asOfDate}) async =>
      _client.delete('/api/v1/debts/$debtId?asOfDate=$asOfDate');

  @override
  Future<DebtEntryOptions> loadEntryOptions() async {
    final results = await Future.wait([
      _client.get('/api/v1/categories?isActive=true'),
      loadSettleSources(),
    ]);
    final categories = [
      for (final item
          in (results[0] as ApiResponse).requireObject()['items'] as List? ??
              const [])
        if (item is Map<String, dynamic>) item,
    ];
    ({String id, String name, TransactionScope? scope}) choice(
      Map<String, dynamic> item,
    ) => (
      id: item['id'] as String,
      name: DefaultCategoryLabels.localized(item['name'] as String),
      scope: TransactionScope.fromApiOrNull(item['defaultScope']),
    );
    return DebtEntryOptions(
      incomeCategories: [
        for (final item in categories)
          if (item['type'] == 'income') choice(item),
      ],
      expenseCategories: [
        for (final item in categories)
          if (item['type'] == 'expense') choice(item),
      ],
      sources: results[1] as DebtSettleSources,
    );
  }

  @override
  Future<String> createCounterparty(
    String name, {
    TransactionScope? scope,
  }) async {
    final response = await _client.post(
      '/api/v1/counterparties',
      body: {'name': name, 'scope': ?scope?.apiValue},
    );
    return response.requireObject()['id'] as String;
  }

  @override
  Future<void> createCharge(
    String counterpartyId,
    Map<String, Object?> body,
  ) async => _client.post(
    '/api/v1/counterparties/$counterpartyId/charges',
    body: body,
  );

  @override
  Future<void> createLoan(
    String counterpartyId,
    Map<String, Object?> body,
  ) async =>
      _client.post('/api/v1/counterparties/$counterpartyId/loans', body: body);

  @override
  Future<void> recordPayment(
    String counterpartyId,
    Map<String, Object?> body,
  ) => settle(counterpartyId, body);

  @override
  Future<void> setOpeningBalance(
    String counterpartyId, {
    required String receivable,
    required String payable,
  }) async => _client.put(
    '/api/v1/counterparties/$counterpartyId/opening-balance',
    body: {'receivable': receivable, 'payable': payable},
  );

  @override
  Future<String> createDebt(Map<String, Object?> body) async {
    final response = await _client.post('/api/v1/debts', body: body);
    return response.requireObject()['id'] as String;
  }

  @override
  Future<DebtSchedulePreview> previewSchedule(Map<String, Object?> body) async {
    final response = await _client.post(
      '/api/v1/debts/schedule-preview',
      body: body,
    );
    return DebtSchedulePreview.fromJson(response.requireObject());
  }

  @override
  Future<void> settle(String counterpartyId, Map<String, Object?> body) async =>
      _client.post(
        '/api/v1/counterparties/$counterpartyId/payments',
        body: body,
      );
}
