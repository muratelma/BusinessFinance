import '../../../core/localization/default_category_labels.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/models/budget_threshold.dart';

class PlanningChoice {
  const PlanningChoice({required this.id, required this.name, this.type});

  final String id;
  final String name;
  final String? type;

  factory PlanningChoice.fromJson(Map<String, dynamic> json) => PlanningChoice(
    id: JsonReaders.string(json, 'id'),
    name: JsonReaders.string(json, 'name'),
    type: JsonReaders.nullableString(json, 'type'),
  );

  factory PlanningChoice.categoryFromJson(Map<String, dynamic> json) =>
      PlanningChoice(
        id: JsonReaders.string(json, 'id'),
        name: DefaultCategoryLabels.localized(JsonReaders.string(json, 'name')),
        type: JsonReaders.nullableString(json, 'type'),
      );
}

class RecurringTransactionItem {
  const RecurringTransactionItem({
    required this.id,
    required this.sourceType,
    required this.accountId,
    required this.creditCardId,
    required this.categoryId,
    required this.amount,
    required this.currency,
    required this.kind,
    required this.frequency,
    required this.startDate,
    required this.endDate,
    required this.occurrenceLimit,
    required this.generatedOccurrenceCount,
    required this.nextOccurrenceDate,
    required this.monthEndBehavior,
    required this.description,
    required this.isActive,
  });

  final String id;

  /// `account` or `credit-card`. A plan is funded by exactly one of the two, so
  /// the matching id is set and the other is null.
  final String sourceType;
  final String? accountId;
  final String? creditCardId;
  final String categoryId;
  final String amount;
  final String currency;
  final String kind;
  final String frequency;
  final String startDate;
  final String? endDate;
  final int? occurrenceLimit;
  final int generatedOccurrenceCount;
  final String? nextOccurrenceDate;
  final String monthEndBehavior;
  final String? description;
  final bool isActive;

  factory RecurringTransactionItem.fromJson(Map<String, dynamic> json) =>
      RecurringTransactionItem(
        id: JsonReaders.string(json, 'id'),
        // Absent on plans written before credit-card sources existed, which are
        // account funded by definition.
        sourceType: JsonReaders.nullableString(json, 'sourceType') ?? 'account',
        accountId: JsonReaders.nullableString(json, 'accountId'),
        creditCardId: JsonReaders.nullableString(json, 'creditCardId'),
        categoryId: JsonReaders.string(json, 'categoryId'),
        amount: JsonReaders.money(json, 'amount'),
        currency: JsonReaders.string(json, 'currency'),
        kind: JsonReaders.string(json, 'kind'),
        frequency: JsonReaders.string(json, 'frequency'),
        startDate: JsonReaders.date(json, 'startDate'),
        endDate: _nullableDate(json, 'endDate'),
        occurrenceLimit: JsonReaders.nullableInt(json, 'occurrenceLimit'),
        generatedOccurrenceCount: JsonReaders.integer(
          json,
          'generatedOccurrenceCount',
        ),
        nextOccurrenceDate: _nullableDate(json, 'nextOccurrenceDate'),
        monthEndBehavior: JsonReaders.string(json, 'monthEndBehavior'),
        description: JsonReaders.nullableString(json, 'description'),
        isActive: JsonReaders.boolean(json, 'isActive'),
      );
}

class RecurringOccurrenceItem {
  const RecurringOccurrenceItem({
    required this.id,
    required this.recurringTransactionId,
    required this.categoryId,
    required this.amount,
    required this.currency,
    required this.kind,
    required this.scheduledDate,
    required this.description,
    required this.status,
  });

  final String id;

  /// Kaydı üreten plan.
  ///
  /// Ekranın "bu para hangi hesaptan çıkacak" sorusunu cevaplayabilmesi için
  /// gerekiyor: kaynak occurrence'ta değil planda duruyor ve onay penceresi
  /// kaynağı adıyla söylemek zorunda.
  final String recurringTransactionId;
  final String categoryId;
  final String amount;
  final String currency;
  final String kind;
  final String scheduledDate;
  final String? description;
  final String status;

  bool get canRealize => status == 'planned';

  factory RecurringOccurrenceItem.fromJson(Map<String, dynamic> json) =>
      RecurringOccurrenceItem(
        id: JsonReaders.string(json, 'id'),
        recurringTransactionId: JsonReaders.string(
          json,
          'recurringTransactionId',
        ),
        categoryId: JsonReaders.string(json, 'categoryId'),
        amount: JsonReaders.money(json, 'amount'),
        currency: JsonReaders.string(json, 'currency'),
        kind: JsonReaders.string(json, 'kind'),
        scheduledDate: JsonReaders.date(json, 'scheduledDate'),
        description: JsonReaders.nullableString(json, 'description'),
        status: JsonReaders.string(json, 'status'),
      );
}

class UpcomingPaymentItem {
  const UpcomingPaymentItem({
    required this.sourceId,
    required this.sourceType,
    required this.title,
    required this.amount,
    required this.currency,
    required this.dueDate,
    required this.timing,
    required this.description,
    this.isTax = false,
  });

  final String sourceId;
  final String sourceType;
  final String title;

  /// Tutarı ödeme gününe kadar belli olmayan vergi kaleminde boştur
  /// (ADR 0018 İ5); boş tutar sıfır değildir.
  final String? amount;
  final String currency;
  final String dueDate;
  final String timing;
  final String? description;

  /// Vergi planının kalemi mi. Vergi burada gerçekleştirilmez: tutar, ödeme
  /// günü ve hesap/kart ister ve onları Vergi takibi sorar (ADR 0018 T4).
  final bool isTax;

  factory UpcomingPaymentItem.fromJson(
    Map<String, dynamic> json, {
    bool isTax = false,
  }) => UpcomingPaymentItem(
    isTax: isTax,
    sourceId: JsonReaders.string(json, 'sourceId'),
    sourceType: JsonReaders.string(json, 'sourceType'),
    title: JsonReaders.string(json, 'title'),
    amount: json['amount'] == null ? null : JsonReaders.money(json, 'amount'),
    currency: JsonReaders.string(json, 'currency'),
    dueDate: JsonReaders.date(json, 'dueDate'),
    timing: JsonReaders.string(json, 'timing'),
    description: JsonReaders.nullableString(json, 'description'),
  );
}

class PeriodTotals {
  const PeriodTotals({
    required this.year,
    required this.month,
    required this.income,
    required this.expense,
    required this.net,
  });

  final int year;
  final int month;
  final String income;
  final String expense;
  final String net;

  factory PeriodTotals.fromJson(Map<String, dynamic> json) => PeriodTotals(
    year: JsonReaders.integer(json, 'year'),
    month: JsonReaders.integer(json, 'month'),
    income: JsonReaders.money(json, 'income'),
    expense: JsonReaders.money(json, 'expense'),
    net: JsonReaders.money(json, 'net'),
  );
}

class CashFlowPoint {
  const CashFlowPoint({
    required this.year,
    required this.month,
    required this.income,
    required this.expense,
    required this.net,
  });

  final int year;
  final int month;
  final String income;
  final String expense;
  final String net;

  double get netValue => double.parse(net);

  factory CashFlowPoint.fromJson(Map<String, dynamic> json) => CashFlowPoint(
    year: JsonReaders.integer(json, 'year'),
    month: JsonReaders.integer(json, 'month'),
    income: JsonReaders.money(json, 'income'),
    expense: JsonReaders.money(json, 'expense'),
    net: JsonReaders.money(json, 'net'),
  );
}

class BudgetVarianceItem {
  const BudgetVarianceItem({
    required this.categoryName,
    required this.canonicalName,
    required this.limit,
    required this.spent,
    required this.remaining,
    required this.isExceeded,
  });

  final String categoryName;

  /// Sunucunun kanonik adı; ikon eşlemesi bunun üzerinden yapılır.
  ///
  /// Ekrandaki ad yerelleştirilmiş hâlidir ve ikon ondan türetilemez: eşleme
  /// tablosu kanonik adlara göre yazılı.
  final String canonicalName;
  final String limit;
  final String spent;

  /// `limit − spent`; limit aşıldığında **negatif** olur ve aşım tutarı budur.
  /// İstemci bu farkı kendisi hesaplamaz — para aritmetiği sunucudadır.
  final String remaining;
  final bool isExceeded;

  /// Aşılmadı ama eşiği geçti.
  ///
  /// Eşik bütçe ekranıyla aynı yerden gelir (`budgetWarningThreshold`).
  bool get isNearLimit {
    if (isExceeded) return false;
    final limitValue = _scaled(limit);
    final spentValue = _scaled(spent);
    if (limitValue == null || spentValue == null || limitValue <= BigInt.zero) {
      return false;
    }
    return spentValue.toDouble() / limitValue.toDouble() >=
        budgetWarningThreshold;
  }

  /// Harcananın limite oranı; halkayı ve yüzdeyi **çizmek** içindir, yeni
  /// bir tutar üretmez. Limit okunamazsa 0.
  double get spentRatio {
    final limitValue = _scaled(limit);
    final spentValue = _scaled(spent);
    if (limitValue == null || spentValue == null || limitValue <= BigInt.zero) {
      return 0;
    }
    return spentValue.toDouble() / limitValue.toDouble();
  }

  static BigInt? _scaled(String value) {
    final match = RegExp(r'^-?(\d+)\.(\d{4})$').firstMatch(value);
    if (match == null) return null;
    return BigInt.tryParse('${match.group(1)}${match.group(2)}');
  }

  factory BudgetVarianceItem.fromJson(Map<String, dynamic> json) {
    final rawName = JsonReaders.string(json, 'categoryName');
    return BudgetVarianceItem(
      categoryName: DefaultCategoryLabels.localized(rawName),
      canonicalName: rawName,
      limit: JsonReaders.money(json, 'limit'),
      spent: JsonReaders.money(json, 'spent'),
      remaining: JsonReaders.money(json, 'remaining'),
      isExceeded: JsonReaders.boolean(json, 'isExceeded'),
    );
  }
}

class DistributionItem {
  const DistributionItem({required this.name, required this.primaryAmount});

  final String name;
  final String primaryAmount;
}

class AdvancedReport {
  const AdvancedReport({
    required this.asOfDate,
    required this.currency,
    required this.liquidAssets,
    required this.creditCardDebt,
    required this.receivableDebt,
    required this.payableDebt,
    required this.netWorth,
    required this.moneyInTransit,
    required this.currentPeriod,
    required this.previousPeriod,
    required this.cashFlowTrend,
    required this.budgetVariances,
    required this.futureLoad,
    required this.accountDistribution,
    required this.cardDistribution,
    this.totalAssets,
    this.totalLiabilities,
    this.nextTransitDate,
    this.netChange,
  });

  final String asOfDate;
  final String currency;
  final String liquidAssets;
  final String creditCardDebt;

  /// Ödenmemiş **alacak** taksitlerinin toplamı — bir bakiye, akış değil.
  ///
  /// Gelir/gider raporuna hiç girmez: gider borcun doğduğu gün bir kez
  /// yazılır, taksit ödemeleri saf bakiye hareketidir. Bu alan net varlığın
  /// terimlerinden biridir, o yüzden burada.
  final String receivableDebt;

  /// Ödenmemiş **borç** taksitlerinin toplamı. [receivableDebt] ile aynı
  /// kural geçerli.
  final String payableDebt;

  /// `liquidAssets + moneyInTransit − creditCardDebt + receivableDebt −
  /// payableDebt`.
  ///
  /// Terimlerin hepsi sözleşmede var; ekranda biri atlanırsa döküm toplamı
  /// açıklamaz.
  final String netWorth;

  /// POS'tan geçmiş ama henüz hesaba ulaşmamış paranın net toplamı.
  ///
  /// Net varlığa girer, **likit varlığa girmez** (ADR 0015): kullanıcının
  /// parasıdır ama bugün harcanamaz. İki sayının farkı tam olarak budur.
  final String moneyInTransit;
  final PeriodTotals currentPeriod;
  final PeriodTotals previousPeriod;
  final List<CashFlowPoint> cashFlowTrend;
  final List<BudgetVarianceItem> budgetVariances;
  final String futureLoad;
  final List<DistributionItem> accountDistribution;
  final List<DistributionItem> cardDistribution;

  /// Net varlığın varlık tarafı (likit + yolda + alacak + varsa kart
  /// alacağı). Sunucuda toplanır; eski sunucu göndermezse `null`.
  final String? totalAssets;

  /// Net varlığın borç tarafı (kart borcu + borç). Sunucuda toplanır.
  final String? totalLiabilities;

  /// Yoldaki paranın en yakın hesaba geçiş günü (`yyyy-MM-dd`).
  final String? nextTransitDate;

  /// Bu ayın neti eksi geçen ayın neti; sunucudan gelir.
  final String? netChange;

  factory AdvancedReport.fromJson(Map<String, dynamic> json) {
    final netWorth = JsonReaders.object(json['netWorth'], 'netWorth');
    final comparison = JsonReaders.object(
      json['periodComparison'],
      'periodComparison',
    );
    final futureLoad = JsonReaders.object(json['futureLoad'], 'futureLoad');
    return AdvancedReport(
      asOfDate: JsonReaders.date(json, 'asOfDate'),
      currency: JsonReaders.string(json, 'currency'),
      liquidAssets: JsonReaders.money(netWorth, 'liquidAssets'),
      creditCardDebt: JsonReaders.money(netWorth, 'creditCardDebt'),
      receivableDebt: JsonReaders.money(netWorth, 'receivableDebt'),
      payableDebt: JsonReaders.money(netWorth, 'payableDebt'),
      netWorth: JsonReaders.money(netWorth, 'netWorth'),
      moneyInTransit: JsonReaders.money(netWorth, 'moneyInTransit'),
      totalAssets: netWorth['totalAssets'] is String
          ? JsonReaders.money(netWorth, 'totalAssets')
          : null,
      totalLiabilities: netWorth['totalLiabilities'] is String
          ? JsonReaders.money(netWorth, 'totalLiabilities')
          : null,
      nextTransitDate: JsonReaders.nullableString(netWorth, 'nextTransitDate'),
      netChange: comparison['netChange'] is String
          ? JsonReaders.money(comparison, 'netChange')
          : null,
      currentPeriod: PeriodTotals.fromJson(
        JsonReaders.object(comparison['current'], 'current'),
      ),
      previousPeriod: PeriodTotals.fromJson(
        JsonReaders.object(comparison['previous'], 'previous'),
      ),
      cashFlowTrend: JsonReaders.list(json, 'cashFlowTrend')
          .map(
            (item) => CashFlowPoint.fromJson(
              JsonReaders.object(item, 'cashFlowPoint'),
            ),
          )
          .toList(growable: false),
      budgetVariances: JsonReaders.list(json, 'budgetVariances')
          .map(
            (item) => BudgetVarianceItem.fromJson(
              JsonReaders.object(item, 'budgetVariance'),
            ),
          )
          .toList(growable: false),
      futureLoad: JsonReaders.money(futureLoad, 'totalAmount'),
      accountDistribution: JsonReaders.list(json, 'accountDistribution')
          .map((item) {
            final value = JsonReaders.object(item, 'accountDistribution');
            return DistributionItem(
              name: JsonReaders.string(value, 'accountName'),
              primaryAmount: JsonReaders.money(value, 'balance'),
            );
          })
          .toList(growable: false),
      cardDistribution: JsonReaders.list(json, 'cardDistribution')
          .map((item) {
            final value = JsonReaders.object(item, 'cardDistribution');
            return DistributionItem(
              name: JsonReaders.string(value, 'creditCardName'),
              primaryAmount: JsonReaders.money(value, 'debt'),
            );
          })
          .toList(growable: false),
    );
  }
}

class PlanningSnapshot {
  const PlanningSnapshot({
    required this.recurringTransactions,
    required this.occurrences,
    required this.upcomingPayments,
    required this.report,
    required this.accounts,
    required this.categories,
    required this.creditCards,
  });

  final List<RecurringTransactionItem> recurringTransactions;
  final List<RecurringOccurrenceItem> occurrences;
  final List<UpcomingPaymentItem> upcomingPayments;
  final AdvancedReport report;
  final List<PlanningChoice> accounts;
  final List<PlanningChoice> categories;

  /// Active cards only. An inactive card cannot take a charge, so offering it
  /// would just produce a rejection at realization time.
  final List<PlanningChoice> creditCards;
}

String? _nullableDate(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value == null) return null;
  if (value is! String ||
      !RegExp(r'^\d{4}-\d{2}-\d{2}$').hasMatch(value) ||
      DateTime.tryParse(value) == null) {
    throw FormatException('Invalid $key.');
  }
  return value;
}
