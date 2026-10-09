import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import '../../activities/data/activity_models.dart';
import '../../obligations/data/obligation_repository.dart';
import 'counterparty_models.dart';

abstract interface class CounterpartyRepositoryContract {
  Future<CounterpartiesSnapshot> load(
    CounterpartyBalanceFilter filter,
    String asOfDate,
  );

  /// Bir kişinin bakiyesi, hareket geçmişi ve sözleşmeleri.
  ///
  /// [asOfDate] sözleşme listesinin **zorunlu** parametresidir: taksitli
  /// borcun kalanı kalıcı bir kolon değil, bir tarihe göre hesaplanan
  /// projection'dır ve tarihsiz sorulamaz.
  Future<CounterpartyDetail> loadDetail(String counterpartyId, String asOfDate);

  Future<void> create(String name, String? note);

  Future<void> update(
    String counterpartyId, {
    required String name,
    required bool isActive,
    String? note,
  });

  /// Hiç hareketi olmayan karşı tarafı siler; hareketi varsa sunucu `409`
  /// döner ve kullanıcı pasifleştirmeye yönlendirilir.
  Future<void> delete(String counterpartyId);

  /// Veresiye satış ya da vadeli alım.
  Future<void> addCharge(String counterpartyId, Map<String, Object?> input);

  /// Tahsilat ya da ödeme.
  Future<void> addPayment(String counterpartyId, Map<String, Object?> input);
}

class CounterpartyRepository implements CounterpartyRepositoryContract {
  const CounterpartyRepository(this._client);
  final ApiClient _client;

  @override
  Future<CounterpartiesSnapshot> load(
    CounterpartyBalanceFilter filter,
    String asOfDate,
  ) async {
    final responses = await Future.wait([
      _client.get(
        '/api/v1/counterparties?balance=${filter.apiValue}'
        '&asOfDate=$asOfDate',
      ),
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
      _client.get('/api/v1/categories?isActive=true'),
    ]);
    return CounterpartiesSnapshot(
      counterparties: _items(
        responses[0].requireObject(),
      ).map(CounterpartySummary.fromJson).toList(growable: false),
      accounts: _items(
        responses[1].requireObject(),
      ).map(DataChoice.fromJson).toList(growable: false),
      categories: _items(
        responses[2].requireObject(),
      ).map(DataChoice.categoryFromJson).toList(growable: false),
    );
  }

  /// Dört okuma tek ekran için: kişi, hareketleri, sözleşmeleri ve bekleyen
  /// faturaları.
  ///
  /// Hareketler birleşik feed'den geliyor, ikinci bir geçmiş modelinden değil:
  /// aynı hareketi iki ayrı yerden okumak, iki farklı sıralama ve iki farklı
  /// iptal kuralı demek olurdu. Sözleşmeler borç listesinden okunup bu kişiye
  /// göre daraltılıyor — cari hesabın dışında dururlar ve toplamları
  /// birbirine karışmaz.
  @override
  Future<CounterpartyDetail> loadDetail(
    String counterpartyId,
    String asOfDate,
  ) async {
    final responses = await Future.wait([
      _client.get('/api/v1/counterparties/$counterpartyId?asOfDate=$asOfDate'),
      _client.get(
        '/api/v1/financial-activities'
        '?pageNumber=1&pageSize=50&counterpartyId=$counterpartyId',
      ),
      // `asOfDate` isteğe bağlı değil: kalan tutar bir tarihe göre hesaplanır
      // ve tarihsiz istek `request.invalid_format` ile reddedilir. Borç ekranı
      // da aynı parametreyi gönderiyor; ikisi aynı soruyu soruyor.
      _client.get('/api/v1/debts?asOfDate=$asOfDate'),
      // Kişiye bağlı açık faturalar: cari bakiyenin dışındadır, ayrı blokta
      // gösterilir. Toplamları kişinin cevabında gelir; burada toplanmaz.
      _client.get('/api/v1/obligations?asOfDate=$asOfDate'),
    ]);
    final pendingObligations = _items(responses[3].requireObject())
        .map(ObligationItem.fromJson)
        .where(
          (item) =>
              item.counterpartyId == counterpartyId && item.status == 'open',
        )
        .toList(growable: false);
    final agreements = _items(responses[2].requireObject())
        .where(
          (item) =>
              JsonReaders.string(item, 'counterpartyId') == counterpartyId,
        )
        .map(CounterpartyAgreement.fromJson)
        .toList(growable: false);
    return CounterpartyDetail(
      counterparty: CounterpartySummary.fromJson(responses[0].requireObject()),
      activities: ActivityPage.fromJson(responses[1].requireObject()).items,
      agreements: agreements,
      pendingObligations: pendingObligations,
    );
  }

  @override
  Future<void> create(String name, String? note) async => _client.post(
    '/api/v1/counterparties',
    body: {'name': name, if (note != null && note.isNotEmpty) 'note': note},
  );

  @override
  Future<void> update(
    String counterpartyId, {
    required String name,
    required bool isActive,
    String? note,
  }) async => _client.put(
    '/api/v1/counterparties/$counterpartyId',
    body: {
      'name': name,
      'isActive': isActive,
      if (note != null && note.isNotEmpty) 'note': note,
    },
  );

  @override
  Future<void> delete(String counterpartyId) async =>
      _client.delete('/api/v1/counterparties/$counterpartyId');

  @override
  Future<void> addCharge(
    String counterpartyId,
    Map<String, Object?> input,
  ) async => _client.post(
    '/api/v1/counterparties/$counterpartyId/charges',
    body: input,
  );

  @override
  Future<void> addPayment(
    String counterpartyId,
    Map<String, Object?> input,
  ) async => _client.post(
    '/api/v1/counterparties/$counterpartyId/payments',
    body: input,
  );

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
