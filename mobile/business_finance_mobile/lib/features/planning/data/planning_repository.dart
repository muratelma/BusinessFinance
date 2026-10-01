import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import 'planning_models.dart';

abstract interface class PlanningRepositoryContract {
  Future<PlanningSnapshot> load({
    required int year,
    required int month,
    required String asOfDate,
    required int daysAhead,
  });
  Future<void> createRecurring(Map<String, Object?> input);
  Future<void> setRecurringActive(String id, bool isActive);

  /// Removes a plan that never produced a movement. The server refuses with
  /// 409 when it did, so history is never orphaned.
  Future<void> deleteRecurring(String id);
  Future<void> realizeOccurrence(String id);

  /// Henüz occurrence üretilmemiş bir tekrarlayan kaydı planı ve tarihiyle
  /// gerçekleştirir; sunucu eksik kaydı kendi üretir.
  Future<void> realizeDue(String planId, String scheduledDate);
}

class PlanningRepository implements PlanningRepositoryContract {
  const PlanningRepository(this._client);

  final ApiClient _client;

  @override
  Future<PlanningSnapshot> load({
    required int year,
    required int month,
    required String asOfDate,
    required int daysAhead,
  }) async {
    final upcomingQuery = Uri(
      queryParameters: {'asOfDate': asOfDate, 'daysAhead': '$daysAhead'},
    ).query;
    final reportQuery = Uri(
      queryParameters: {
        'year': '$year',
        'month': '$month',
        'asOfDate': asOfDate,
        'trendMonths': '6',
        'daysAhead': '$daysAhead',
      },
    ).query;
    final responses = await Future.wait([
      _client.get('/api/v1/recurring-transactions'),
      _client.get('/api/v1/recurring-transactions/occurrences'),
      _client.get('/api/v1/upcoming-payments?$upcomingQuery'),
      _client.get('/api/v1/reports/advanced?$reportQuery'),
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
      _client.get('/api/v1/categories?isActive=true'),
      _client.get('/api/v1/credit-cards'),
    ]);

    // Vergi planları Tekrarlayanlar'da görünmez; kendi ekranları var
    // (ADR 0018 T2). Tutarı boş olabilen vergi planı ve kalemi burada hiç
    // okunmaz. Yaklaşan ödemelerde kalırlar ama eylemleri Vergi takibi'ne
    // gider.
    final plans = _items(responses[0].requireObject());
    final taxPlanIds = {
      for (final plan in plans)
        if (plan['taxKind'] != null) JsonReaders.string(plan, 'id'),
    };
    final occurrences = _items(responses[1].requireObject());
    final taxSourceIds = {
      ...taxPlanIds,
      for (final occurrence in occurrences)
        if (taxPlanIds.contains(occurrence['recurringTransactionId']))
          JsonReaders.string(occurrence, 'id'),
    };
    return PlanningSnapshot(
      recurringTransactions: plans
          .where((plan) => plan['taxKind'] == null)
          .map(RecurringTransactionItem.fromJson)
          .toList(growable: false),
      occurrences: occurrences
          .where(
            (occurrence) =>
                !taxPlanIds.contains(occurrence['recurringTransactionId']),
          )
          .map(RecurringOccurrenceItem.fromJson)
          .toList(growable: false),
      upcomingPayments: _items(responses[2].requireObject())
          .map(
            (item) => UpcomingPaymentItem.fromJson(
              item,
              isTax:
                  item['sourceType'] == 'recurring-occurrence' &&
                  taxSourceIds.contains(item['sourceId']),
            ),
          )
          .toList(growable: false),
      report: AdvancedReport.fromJson(responses[3].requireObject()),
      accounts: _items(
        responses[4].requireObject(),
      ).map(PlanningChoice.fromJson).toList(growable: false),
      categories: _items(
        responses[5].requireObject(),
      ).map(PlanningChoice.categoryFromJson).toList(growable: false),
      creditCards: _items(responses[6].requireObject())
          .where((card) => card['isActive'] == true)
          .map(PlanningChoice.fromJson)
          .toList(growable: false),
    );
  }

  @override
  Future<void> createRecurring(Map<String, Object?> input) async =>
      _client.post('/api/v1/recurring-transactions', body: input);

  @override
  Future<void> setRecurringActive(String id, bool isActive) async =>
      _client.patch(
        '/api/v1/recurring-transactions/$id/active',
        body: {'isActive': isActive},
      );

  @override
  Future<void> deleteRecurring(String id) async =>
      _client.delete('/api/v1/recurring-transactions/$id');

  @override
  Future<void> realizeOccurrence(String id) async =>
      _client.post('/api/v1/recurring-transactions/occurrences/$id/realize');

  @override
  Future<void> realizeDue(String planId, String scheduledDate) async =>
      _client.post(
        '/api/v1/recurring-transactions/$planId/occurrences/realize',
        body: {'scheduledDate': scheduledDate},
      );

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
