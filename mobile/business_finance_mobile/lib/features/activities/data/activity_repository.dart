import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import 'activity_models.dart';
import 'planned_activity_models.dart';

abstract interface class ActivityRepositoryContract {
  /// [scope] boşsa filtre gönderilmez. Doluyken feed **kapsamsız satırları da
  /// eler**: transfer, kart ödemesi ve kart ekstresi kapsam taşımaz ve ikisini
  /// birden iki tarafta göstermek aynı para hareketini iki kez saydırırdı.
  Future<ActivityPage> list({
    int pageNumber,
    int pageSize,
    ActivityFilter filter,
    DateTime? today,
    TransactionScope? scope,
  });

  /// Cancels the movement through the endpoint that owns its write model.
  Future<void> cancel(FinancialActivity activity);

  /// Reads what has not happened yet, within one of the offered horizons.
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  });

  /// Planlanan bir satırı gerçek harekete çevirir.
  ///
  /// [cancel]'ın kardeşi: planlanan görünüm de bir okuma modeli, o yüzden
  /// eylem satırı üreten yazma modeline geri gider. Yönlendirme burada, veri
  /// katmanında; ekran hangi uç noktanın çağrılacağını bilmez.
  Future<void> realizePlanned(PlannedActivity activity);
}

class ActivityRepository implements ActivityRepositoryContract {
  const ActivityRepository(this._apiClient);
  final ApiClient _apiClient;

  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) async {
    final dateFrom = filter.dateRange.dateFrom(today ?? DateTime.now());
    final query = <String, String>{
      'pageNumber': '$pageNumber',
      'pageSize': '$pageSize',
      // Absent means no filter. The default range sends nothing, so the feed
      // reaches back through the whole history instead of stopping at a window
      // the user never chose.
      'dateFrom': ?dateFrom,
      if (filter.quickFilter.sourceGroup != null)
        'sourceGroup': filter.quickFilter.sourceGroup!.apiValue,
      if (filter.quickFilter.origin != null)
        'origin': filter.quickFilter.origin!.apiValue,
      if (filter.effect != null) 'effect': filter.effect!.apiValue,
      if (filter.accountId != null) 'accountId': filter.accountId!,
      if (filter.creditCardId != null) 'creditCardId': filter.creditCardId!,
      if (filter.categoryId != null) 'categoryId': filter.categoryId!,
      if (!filter.includeCancelled) 'includeCancelled': 'false',
      'scope': ?scope?.apiValue,
    };
    final response = await _apiClient.get(
      Uri(
        path: '/api/v1/financial-activities',
        queryParameters: query,
      ).toString(),
    );
    return ActivityPage.fromJson(response.requireObject());
  }

  @override
  Future<void> cancel(FinancialActivity activity) async {
    // The feed is a read model, so cancelling goes back to the write model that
    // produced the row. Debt movements have no reversal at all, which is why the
    // server reports canCancel false for them and there is no path here.
    final path = switch (activity.kind) {
      ActivityKind.accountTransaction =>
        '/api/v1/transactions/${activity.activityId}',
      ActivityKind.transfer => '/api/v1/transfers/${activity.activityId}',
      ActivityKind.cardCharge =>
        '/api/v1/credit-card-charges/${activity.activityId}',
      ActivityKind.cardPayment =>
        '/api/v1/credit-card-payments/${activity.activityId}',
      // Açılış satırı sözleşmenin kendisidir; iptali borcu silmek olurdu ve
      // ödenmiş taksitler sahipsiz kalırdı.
      ActivityKind.debtPayment ||
      ActivityKind.debtCollection ||
      ActivityKind.debtOpening ||
      ActivityKind.obligation ||
      ActivityKind.obligationSettlement => throw StateError(
        'Borç hareketi iptal edilemez: ${activity.activityId}',
      ),
      // POS tahsilatı tek kaydın üç satırıdır; birini iptal etmek diğer ikisini
      // sahipsiz bırakırdı. İptal, kaydın kendi ekranından tek eylemle yapılır
      // ve üç satırı birlikte kapatır. Sunucu da bu üçü için canCancel:false
      // döndürüyor, yani bu dal normalde hiç çalışmaz.
      ActivityKind.posSale ||
      ActivityKind.posCommission ||
      ActivityKind.posTransfer => throw StateError(
        'POS tahsilatı feed üzerinden iptal edilemez: ${activity.activityId}',
      ),
      // Cari hareketin ikisi de iptal edilebilir: tek başına duran kayıtlar,
      // geri dönüşü olmayan bir planın sonucu değiller.
      ActivityKind.counterpartyCharge =>
        '/api/v1/counterparty-charges/${activity.activityId}',
      ActivityKind.counterpartySettlement =>
        '/api/v1/counterparty-payments/${activity.activityId}',
    };
    await _apiClient.delete(path);
  }

  @override
  Future<void> realizePlanned(PlannedActivity activity) async {
    final target = activity.actionTargetId;
    if (!activity.isDirectlyRealizable || target == null) {
      throw StateError(
        'Bu satır tek dokunuşla gerçekleştirilemez: ${activity.listKey}',
      );
    }
    // Henüz occurrence üretilmemiş tekrarlanan satır planı ve tarihi adresler;
    // sunucu eksik kaydı kendi üretip gerçekleştirir. Üretmek bir kullanıcı
    // kararı değil, sistemin idempotentlik için tuttuğu defter işi.
    if (activity.plannedKind == PlannedKind.recurringOccurrence &&
        activity.isProjected) {
      await _apiClient.post(
        '/api/v1/recurring-transactions/$target/occurrences/realize',
        body: {'scheduledDate': activity.dueDate},
      );
      return;
    }

    final path = switch (activity.plannedKind) {
      PlannedKind.recurringOccurrence =>
        '/api/v1/recurring-transactions/occurrences/$target/realize',
      PlannedKind.cardInstallment =>
        '/api/v1/installment-plans/$target/items/${activity.actionSequence}/realize',
      // Bunlar `realize` değil ödeme/tahsilat; buraya hiç gelmemeleri gerekir.
      PlannedKind.cardStatement ||
      PlannedKind.debtInstallment ||
      PlannedKind.receivableInstallment ||
      PlannedKind.payableObligation ||
      PlannedKind.receivableObligation => throw StateError(
        'Bu tür ödeme ekranından yürütülür: ${activity.plannedKind.apiValue}',
      ),
    };
    await _apiClient.post(path);
  }

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async {
    final asOf = today ?? DateTime.now();
    final month = asOf.month.toString().padLeft(2, '0');
    final day = asOf.day.toString().padLeft(2, '0');
    final response = await _apiClient.get(
      Uri(
        path: '/api/v1/financial-activities/planned',
        queryParameters: {
          'asOfDate': '${asOf.year}-$month-$day',
          'daysAhead': '${horizon.days}',
          'scope': ?scope?.apiValue,
        },
      ).toString(),
    );
    return PlannedActivityPage.fromJson(response.requireObject());
  }
}
