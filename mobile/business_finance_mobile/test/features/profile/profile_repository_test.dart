import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/features/profile/data/profile_repository.dart';

void main() {
  test('cevabı sunucudan okur', () async {
    final requests = <http.Request>[];
    final repository = _repository(requests, hasBusiness: true);

    final profile = await repository.read();

    expect(profile.hasBusiness, isTrue);
    expect(requests.single.method, 'GET');
    expect(requests.single.url.path, '/api/v1/profile');
  });

  test('cevabı değiştirir ve sunucunun döndürdüğü hâli verir', () async {
    final requests = <http.Request>[];
    final repository = _repository(requests, hasBusiness: false);

    final profile = await repository.update(hasBusiness: false);

    expect(profile.hasBusiness, isFalse);
    expect(requests.single.method, 'PUT');
    expect(jsonDecode(requests.single.body), {'hasBusiness': false});
  });

  test('eksik alan sessizce "işletmesi yok" diye okunmaz', () async {
    final repository = _repository(<http.Request>[], body: '{}');

    expect(repository.read(), throwsA(isA<FormatException>()));
  });
}

ProfileRepository _repository(
  List<http.Request> requests, {
  bool hasBusiness = false,
  String? body,
}) {
  final client = MockClient((request) async {
    requests.add(request);
    return http.Response(
      body ?? jsonEncode({'hasBusiness': hasBusiness}),
      200,
      headers: {'content-type': 'application/json'},
    );
  });
  return ProfileRepository(
    ApiClient(
      config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
      httpClient: client,
    ),
  );
}
