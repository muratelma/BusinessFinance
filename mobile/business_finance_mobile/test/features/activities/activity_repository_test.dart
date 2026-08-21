import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';

void main() {
  group('list', () {
    test('sends no lower date bound by default', () async {
      final requests = <http.Request>[];
      final repository = _repository(requests);

      await repository.list();

      expect(
        requests.single.url.queryParameters.containsKey('dateFrom'),
        isFalse,
      );
      expect(requests.single.url.queryParameters['pageNumber'], '1');
    });

    test('translates the chosen range into an inclusive lower bound', () async {
      final requests = <http.Request>[];
      final repository = _repository(requests);

      await repository.list(
        filter: const ActivityFilter(
          dateRange: ActivityDateRange.lastThreeMonths,
        ),
        today: DateTime(2026, 8, 14),
      );

      expect(requests.single.url.queryParameters['dateFrom'], '2026-06-01');
    });

    test('maps a quick chip onto the dimension it actually narrows', () async {
      final requests = <http.Request>[];
      final repository = _repository(requests);

      await repository.list(
        filter: const ActivityFilter(
          quickFilter: ActivityQuickFilter.recurring,
        ),
      );

      final query = requests.single.url.queryParameters;
      // Recurring is an origin, not a source group.
      expect(query['origin'], 'recurring');
      expect(query.containsKey('sourceGroup'), isFalse);
    });

    test('omits includeCancelled unless the user hid cancelled rows', () async {
      final requests = <http.Request>[];
      final repository = _repository(requests);

      await repository.list();
      expect(
        requests.single.url.queryParameters.containsKey('includeCancelled'),
        isFalse,
      );

      requests.clear();
      await repository.list(
        filter: const ActivityFilter(includeCancelled: false),
      );
      expect(requests.single.url.queryParameters['includeCancelled'], 'false');
    });
  });

  group('cancel', () {
    /// The feed is a read model, so cancelling has to reach the write model that
    /// produced the row. Sending every kind to the transactions endpoint would
    /// silently do nothing for transfers and card movements.
    test('routes each kind to the endpoint that owns it', () async {
      final requests = <http.Request>[];
      final repository = _repository(requests);

      await repository.cancel(_activity(ActivityKind.accountTransaction));
      await repository.cancel(_activity(ActivityKind.transfer));
      await repository.cancel(_activity(ActivityKind.cardCharge));
      await repository.cancel(_activity(ActivityKind.cardPayment));

      expect(requests.map((request) => request.url.path), [
        '/api/v1/transactions/a',
        '/api/v1/transfers/a',
        '/api/v1/credit-card-charges/a',
        '/api/v1/credit-card-payments/a',
      ]);
      expect(requests.every((request) => request.method == 'DELETE'), isTrue);
    });

    test('refuses a debt movement, which has no reversal at all', () async {
      final repository = _repository([]);

      expect(
        () => repository.cancel(_activity(ActivityKind.debtPayment)),
        throwsStateError,
      );
      expect(
        () => repository.cancel(_activity(ActivityKind.debtCollection)),
        throwsStateError,
      );
    });
  });
}

ActivityRepository _repository(List<http.Request> requests) {
  final client = MockClient((request) async {
    requests.add(request);
    return http.Response(
      jsonEncode(<String, dynamic>{
        'items': <dynamic>[],
        'pagination': {
          'pageNumber': 1,
          'pageSize': 20,
          'totalCount': 0,
          'totalPages': 0,
          'hasPreviousPage': false,
          'hasNextPage': false,
        },
      }),
      200,
      headers: {'content-type': 'application/json'},
    );
  });
  return ActivityRepository(
    ApiClient(
      config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
      httpClient: client,
    ),
  );
}

FinancialActivity _activity(ActivityKind kind) => FinancialActivity(
  activityId: 'a',
  kind: kind,
  effect: ActivityEffect.expense,
  sourceGroup: ActivitySourceGroup.account,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: '2026-08-14',
  amount: '10.0000',
  currency: 'TRY',
  title: 'Market',
  canCancel: true,
  supportsAttachments: false,
);
