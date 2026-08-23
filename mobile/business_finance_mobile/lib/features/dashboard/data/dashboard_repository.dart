import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import '../../planning/data/planning_models.dart';
import 'dashboard_models.dart';

abstract interface class DashboardDataSource {
  /// [scope] boşsa filtre gönderilmez ve iki taraf birden okunur.
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  });

  /// Net varlık, dönem karşılaştırması ve nakit akışı eğilimi. Özet için
  /// ikincil bir okumadır: başarısız olursa ay özeti yine gösterilir.
  ///
  /// Kapsam yalnız gelir/gider tarafını böler; **net varlık bölünmez** ve
  /// sunucu onu filtreden bağımsız hesaplar (ADR 0013).
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  });
}

class DashboardRepository implements DashboardDataSource {
  DashboardRepository(this._apiClient);

  final ApiClient _apiClient;

  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) async {
    final query = Uri(
      queryParameters: {
        'year': '$year',
        'month': '$month',
        'scope': ?scope?.apiValue,
      },
    ).query;
    final response = await _apiClient.get('/api/v1/dashboard?$query');
    return DashboardReport.fromJson(response.requireObject());
  }

  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) async {
    // `asOfDate` zorunludur: net varlık "şu an" değil, verilen güne göre
    // hesaplanır. Görüntülenen ayın son günü istenir, böylece geçmiş bir aya
    // bakarken o ayın sonundaki durum görünür.
    final lastDayOfMonth = DateTime(year, month + 1, 0);
    final asOfDate =
        '${lastDayOfMonth.year.toString().padLeft(4, '0')}-'
        '${lastDayOfMonth.month.toString().padLeft(2, '0')}-'
        '${lastDayOfMonth.day.toString().padLeft(2, '0')}';
    final query = Uri(
      queryParameters: {
        'year': '$year',
        'month': '$month',
        'asOfDate': asOfDate,
        'trendMonths': '6',
        'scope': ?scope?.apiValue,
      },
    ).query;
    final response = await _apiClient.get('/api/v1/reports/advanced?$query');
    return AdvancedReport.fromJson(response.requireObject());
  }
}
