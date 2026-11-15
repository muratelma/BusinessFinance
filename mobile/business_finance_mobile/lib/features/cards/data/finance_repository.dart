import '../../../core/localization/default_category_labels.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import 'card_page_models.dart';
import 'finance_models.dart';

/// Geçmiş listelerinin dönemi.
///
/// Sunucu tarih verilmediğinde son üç ayı uyguluyor; `all` ayrı ve açık bir
/// niyet çünkü eksik parametreyle sınırsız sorguya düşmek, bir alanı unutmayı
/// sessiz bir performans sorununa çevirirdi. Satır tavanı her seçenekte
/// geçerli, `all` dâhil.
enum HistoryPeriod {
  threeMonths('3 ay', 3),
  year('1 yıl', 12),
  all('Tümü', null);

  const HistoryPeriod(this.label, this.months);
  final String label;
  final int? months;

  /// Uç noktaya eklenecek sorgu dizesi.
  String query(DateTime today) {
    if (months == null) return '?all=true';
    final from = DateTime(today.year, today.month - months!, today.day);
    final month = from.month.toString().padLeft(2, '0');
    final day = from.day.toString().padLeft(2, '0');
    return '?from=${from.year}-$month-$day';
  }
}

abstract interface class FinanceRepositoryContract {
  Future<FinanceSnapshot> load({HistoryPeriod period});
  Future<void> createTransfer(Map<String, Object?> input);
  Future<void> cancelTransfer(String transferId);
  Future<void> createCard(Map<String, Object?> input);
  Future<void> updateCard(String cardId, Map<String, Object?> input);
  Future<void> createCharge(String cardId, Map<String, Object?> input);
  Future<void> createPayment(String cardId, Map<String, Object?> input);
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  );

  /// Kesim tarihi geçmiş en son ekstre; hiç kesim olmamışsa `null`.
  Future<CardStatement?> loadCurrentStatement(String cardId);
  Future<void> createPlan(Map<String, Object?> input);
  Future<void> realizeInstallment(String planId, int sequence);

  /// Kartın hareketler sayfası; `limit` verilirse ilk o kadar satır.
  Future<CardPageData> loadCardPage(
    String cardId,
    CardMovementQuery query, {
    int offset = 0,
    int? limit,
  });

  /// Taksit sayılarının aylık tutarı ve ilk/son günü; bölmeyi sunucu yapar.
  Future<List<InstallmentPreviewOption>> previewInstallments({
    required String totalAmount,
    required String firstInstallmentDate,
    required List<int> counts,
  });
}

abstract interface class BankFeeCategoryProvider {
  Future<FinanceChoice> ensureBankFeeCategory();
}

class FinanceRepository
    implements FinanceRepositoryContract, BankFeeCategoryProvider {
  FinanceRepository(this._client);
  final ApiClient _client;

  @override
  Future<FinanceChoice> ensureBankFeeCategory() async {
    final json = (await _client.post(
      '/api/v1/categories/bank-fee',
    )).requireObject();
    return FinanceChoice(
      id: JsonReaders.string(json, 'id'),
      name: JsonReaders.string(json, 'name'),
      role: JsonReaders.string(json, 'role'),
    );
  }

  @override
  Future<FinanceSnapshot> load({
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async {
    final responses = await Future.wait([
      _client.get('/api/v1/transfers${period.query(DateTime.now())}'),
      _client.get('/api/v1/credit-cards'),
      _client.get('/api/v1/installment-plans'),
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
      _client.get('/api/v1/categories?type=expense&isActive=true'),
    ]);
    final transfers = _items(
      responses[0].requireObject(),
    ).map(TransferItem.fromJson).toList(growable: false);
    final cards = _items(
      responses[1].requireObject(),
    ).map(CreditCardItem.fromJson).toList(growable: false);
    final plans = _items(
      responses[2].requireObject(),
    ).map(InstallmentPlanModel.fromJson).toList(growable: false);
    final accounts = _items(responses[3].requireObject())
        .map(
          (json) => FinanceChoice(
            id: JsonReaders.string(json, 'id'),
            name: JsonReaders.string(json, 'name'),
            defaultScope: TransactionScope.fromApiOrNull(json['defaultScope']),
            role: JsonReaders.nullableString(json, 'role'),
          ),
        )
        .toList(growable: false);
    final categories = _items(responses[4].requireObject())
        .map(
          (json) => FinanceChoice(
            id: JsonReaders.string(json, 'id'),
            name: DefaultCategoryLabels.localized(
              JsonReaders.string(json, 'name'),
            ),
            defaultScope: TransactionScope.fromApiOrNull(json['defaultScope']),
            role: JsonReaders.nullableString(json, 'role'),
          ),
        )
        .toList(growable: false);
    return FinanceSnapshot(
      transfers: transfers,
      cards: cards,
      plans: plans,
      accounts: accounts,
      expenseCategories: categories,
      transfersHaveMore: JsonReaders.boolean(
        responses[0].requireObject(),
        'hasMore',
      ),
    );
  }

  @override
  Future<void> createTransfer(Map<String, Object?> input) async =>
      _client.post('/api/v1/transfers', body: input);
  @override
  Future<void> cancelTransfer(String transferId) async =>
      _client.delete('/api/v1/transfers/$transferId');
  @override
  Future<void> createCard(Map<String, Object?> input) async =>
      _client.post('/api/v1/credit-cards', body: input);
  @override
  Future<void> createCharge(String cardId, Map<String, Object?> input) async =>
      _client.post('/api/v1/credit-cards/$cardId/charges', body: input);
  @override
  Future<void> createPayment(String cardId, Map<String, Object?> input) async =>
      _client.post('/api/v1/credit-cards/$cardId/payments', body: input);

  /// Kartın ham hareket listesi. Uygulama artık kart sayfasını
  /// (`loadCardPage`) okur; bu uç yalnız kabul testinde kullanılır.
  Future<CardActivity> loadActivity(
    String cardId, {
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async => CardActivity.fromJson(
    (await _client.get(
      '/api/v1/credit-cards/$cardId/activity${period.query(DateTime.now())}',
    )).requireObject(),
  );

  @override
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) async => CardStatement.fromJson(
    (await _client.get(
      '/api/v1/credit-cards/$cardId/statements/$year/$month?asOf=$asOf',
    )).requireObject(),
  );
  @override
  Future<void> updateCard(String cardId, Map<String, Object?> input) async =>
      _client.put('/api/v1/credit-cards/$cardId', body: input);

  @override
  Future<CardStatement?> loadCurrentStatement(String cardId) async {
    final json = (await _client.get(
      '/api/v1/credit-cards/$cardId/statements/current',
    )).requireObject();
    final statement = json['statement'];
    return statement == null
        ? null
        : CardStatement.fromJson(JsonReaders.object(statement, 'statement'));
  }

  @override
  Future<void> createPlan(Map<String, Object?> input) async =>
      _client.post('/api/v1/installment-plans', body: input);
  @override
  Future<void> realizeInstallment(String planId, int sequence) async =>
      _client.post('/api/v1/installment-plans/$planId/items/$sequence/realize');

  @override
  Future<CardPageData> loadCardPage(
    String cardId,
    CardMovementQuery query, {
    int offset = 0,
    int? limit,
  }) async => CardPageData.fromJson(
    (await _client.get(
      Uri(
        path: '/api/v1/credit-cards/$cardId/page',
        queryParameters: query.toQuery(offset: offset, limit: limit),
      ).toString(),
    )).requireObject(),
  );

  @override
  Future<List<InstallmentPreviewOption>> previewInstallments({
    required String totalAmount,
    required String firstInstallmentDate,
    required List<int> counts,
  }) async {
    final json = (await _client.get(
      Uri(
        path: '/api/v1/installment-plans/preview',
        queryParameters: {
          'totalAmount': totalAmount,
          'firstInstallmentDate': firstInstallmentDate,
          'counts': counts.join(','),
        },
      ).toString(),
    )).requireObject();
    return JsonReaders.list(json, 'options')
        .map(
          (v) => InstallmentPreviewOption.fromJson(
            JsonReaders.object(v, 'option'),
          ),
        )
        .toList(growable: false);
  }

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(json, 'items')
          .map((value) => JsonReaders.object(value, 'item'))
          .toList(growable: false);
}
