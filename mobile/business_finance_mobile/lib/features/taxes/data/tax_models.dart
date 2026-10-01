import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../activities/data/planned_activity_models.dart';

/// Vergi planının türü. Sunucu kararlı makine değerini gönderir; ad ve ipucu
/// istemcinin cümlesidir (ADR 0018 T2).
///
/// Hazır türler yalnız ritim ve gün önerir; **tutar önermez**. İpuçları
/// "genelde" diliyle yazılır ve kullanıcıyı muhasebecisine bırakır: uygulama
/// vergi uzmanı gibi konuşmaz.
enum TaxKind {
  socialSecurityPremium(
    'social-security-premium',
    'Bağkur',
    'Esnafın çoğu öder',
  ),
  vatReturn('vat-return', 'KDV', 'Basit usuldeyseniz genelde yok'),
  withholdingReturn(
    'withholding-return',
    'Muhtasar ve prim hizmet',
    'Çalışanınız varsa',
  ),
  advanceTax('advance-tax', 'Geçici vergi', 'Gerçek usuldeyseniz'),
  annualIncomeTax(
    'annual-income-tax',
    'Yıllık gelir vergisi',
    'Gerçek usuldeyseniz',
  ),
  propertyTax(
    'property-tax',
    'Emlak ve çevre temizlik',
    'Dükkânınız varsa; belediyeye',
  ),
  motorVehicleTax('motor-vehicle-tax', 'Motorlu taşıtlar', 'Aracınız varsa'),
  advertisingTax(
    'advertising-tax',
    'İlan-reklam (tabela)',
    'Tabelanız varsa; belediyeye',
  ),
  custom('custom', 'Kendi türüm', null);

  const TaxKind(this.apiValue, this.label, this.hint);

  final String apiValue;
  final String label;
  final String? hint;

  static TaxKind fromApi(String value) => TaxKind.values.firstWhere(
    (kind) => kind.apiValue == value,
    orElse: () => throw FormatException('Bilinmeyen vergi türü: $value'),
  );

  static TaxKind? fromApiOrNull(Object? value) =>
      value is String ? fromApi(value) : null;
}

/// Vergi tanımında sunulan ritimler. Günlük ve haftalık vergi yoktur.
enum TaxRhythm {
  selectedMonths('selected-months', 'Seçilen aylarda'),
  monthly('monthly', 'Her ay'),
  quarterly('quarterly', 'Üç ayda bir'),
  yearly('yearly', 'Yılda bir');

  const TaxRhythm(this.apiValue, this.label);

  final String apiValue;
  final String label;

  static TaxRhythm fromApi(String value) => TaxRhythm.values.firstWhere(
    (rhythm) => rhythm.apiValue == value,
    orElse: () => throw FormatException('Bilinmeyen vergi ritmi: $value'),
  );
}

/// Tanımlı bir vergi: vergi türü dolu tekrarlayan plan.
class TaxPlan {
  const TaxPlan({
    required this.id,
    required this.name,
    required this.taxKind,
    required this.rhythm,
    required this.startDate,
    required this.categoryId,
    required this.scope,
    required this.isActive,
    required this.currency,
    this.amount,
    this.accountId,
    this.creditCardId,
    this.dayOfMonth,
    this.months = const [],
    this.nextDate,
  });

  factory TaxPlan.fromJson(Map<String, dynamic> json) => TaxPlan(
    id: JsonReaders.string(json, 'id'),
    name: JsonReaders.nullableString(json, 'description') ?? '',
    taxKind: TaxKind.fromApi(JsonReaders.string(json, 'taxKind')),
    rhythm: TaxRhythm.fromApi(JsonReaders.string(json, 'frequency')),
    startDate: JsonReaders.date(json, 'startDate'),
    categoryId: JsonReaders.string(json, 'categoryId'),
    scope: TransactionScope.fromApi(JsonReaders.string(json, 'scope')),
    isActive: JsonReaders.boolean(json, 'isActive'),
    currency: JsonReaders.string(json, 'currency'),
    amount: json['amount'] == null ? null : JsonReaders.money(json, 'amount'),
    accountId: JsonReaders.nullableString(json, 'accountId'),
    creditCardId: JsonReaders.nullableString(json, 'creditCardId'),
    dayOfMonth: JsonReaders.nullableInt(json, 'dayOfMonth'),
    months: [
      for (final month in (json['months'] as List<dynamic>? ?? const []))
        month as int,
    ],
    nextDate: JsonReaders.nullableString(json, 'nextOccurrenceDate'),
  );

  final String id;

  /// Kullanıcının verdiği ad ("Bağkur"); kimlik ada bağlanmaz (İ8).
  final String name;
  final TaxKind taxKind;
  final TaxRhythm rhythm;
  final String startDate;
  final String categoryId;
  final TransactionScope scope;
  final bool isActive;
  final String currency;

  /// Beklenen tutar; her dönem değişen vergide boştur (İ5).
  final String? amount;
  final String? accountId;
  final String? creditCardId;

  /// Ayın günü; `31` ay sonudur.
  final int? dayOfMonth;
  final List<int> months;
  final String? nextDate;

  String? get sourceId => accountId ?? creditCardId;

  /// Ritmin gün kısmı: `ay sonu` ya da başlangıç günü.
  int get effectiveDay =>
      dayOfMonth ?? int.parse(startDate.substring(startDate.length - 2));
}

/// Bir ödemenin ödediği ya da kapattığı tanımlı vergi kalemi.
class TaxSettledItem {
  const TaxSettledItem({
    required this.occurrenceId,
    required this.recurringTransactionId,
    required this.scheduledDate,
    this.name,
    this.taxKind,
  });

  factory TaxSettledItem.fromJson(Map<String, dynamic> json) => TaxSettledItem(
    occurrenceId: JsonReaders.string(json, 'occurrenceId'),
    recurringTransactionId: JsonReaders.string(json, 'recurringTransactionId'),
    scheduledDate: JsonReaders.date(json, 'scheduledDate'),
    name: JsonReaders.nullableString(json, 'name'),
    taxKind: TaxKind.fromApiOrNull(json['taxKind']),
  );

  final String occurrenceId;
  final String recurringTransactionId;
  final String scheduledDate;
  final String? name;
  final TaxKind? taxKind;

  String get label => name ?? taxKind?.label ?? 'Vergi';
}

/// Ödenmiş vergi: vergi işaretli kategorideki bir gider ya da kart harcaması
/// (ADR 0018 T6). Ayrı bir kayıt türü değildir; kimliği kaydın kimliğidir.
class TaxPayment {
  const TaxPayment({
    required this.paymentId,
    required this.isCard,
    required this.sourceId,
    required this.sourceName,
    required this.categoryName,
    required this.amount,
    required this.currency,
    required this.paidOn,
    required this.scope,
    required this.closedItems,
    required this.isCancelled,
    this.description,
    this.realizedItem,
  });

  factory TaxPayment.fromJson(Map<String, dynamic> json) => TaxPayment(
    paymentId: JsonReaders.string(json, 'paymentId'),
    isCard: JsonReaders.string(json, 'sourceType') == 'credit-card',
    sourceId: JsonReaders.string(json, 'sourceId'),
    sourceName: JsonReaders.string(json, 'sourceName'),
    categoryName: JsonReaders.string(json, 'categoryName'),
    amount: JsonReaders.money(json, 'amount'),
    currency: JsonReaders.string(json, 'currency'),
    paidOn: JsonReaders.date(json, 'paidOn'),
    scope: TransactionScope.fromApi(JsonReaders.string(json, 'scope')),
    description: JsonReaders.nullableString(json, 'description'),
    realizedItem: json['realizedItem'] == null
        ? null
        : TaxSettledItem.fromJson(
            JsonReaders.object(json['realizedItem'], 'realizedItem'),
          ),
    closedItems: [
      for (final item in JsonReaders.list(json, 'closedItems'))
        TaxSettledItem.fromJson(JsonReaders.object(item, 'closedItem')),
    ],
    isCancelled: JsonReaders.boolean(json, 'isCancelled'),
  );

  final String paymentId;
  final bool isCard;
  final String sourceId;
  final String sourceName;
  final String categoryName;
  final String amount;
  final String currency;
  final String paidOn;
  final TransactionScope scope;
  final String? description;

  /// Bir kalemin "Ödedim" sonucuysa o kalem.
  final TaxSettledItem? realizedItem;

  /// Toplu ödemenin kapattığı kalemler.
  final List<TaxSettledItem> closedItems;
  final bool isCancelled;

  bool get isBulk => realizedItem == null && closedItems.isNotEmpty;

  /// Kaydın adı: kalemin adı, yoksa kullanıcının yazdığı, yoksa kategori
  /// (İşlemler'deki adla aynı; `açıklama ?? kategori`). Toplu ödeme tek bir
  /// vergiye ait değildir: adı sabittir, kullanıcının yazdığı [note]'tur.
  String get title =>
      realizedItem?.label ??
      (isBulk ? 'Vergi ödemesi' : description ?? categoryName);

  /// Kullanıcının ödemeye düştüğü not. Kalemin "Ödedim" sonucunda açıklama
  /// kalemin adıdır, not değildir.
  String? get note => realizedItem == null ? description : null;
}

class TaxPaymentPage {
  const TaxPaymentPage({required this.items, required this.hasMore});

  final List<TaxPayment> items;
  final bool hasMore;
}

/// Vergi ekranının tek okuması.
class TaxOverview {
  const TaxOverview({
    required this.asOfDate,
    required this.plans,
    required this.pending,
    required this.pendingTotal,
    required this.pendingUnknownAmountCount,
    required this.recentPayments,
    required this.hasMorePayments,
  });

  factory TaxOverview.fromJson(Map<String, dynamic> json) => TaxOverview(
    asOfDate: JsonReaders.date(json, 'asOfDate'),
    plans: [
      for (final item in JsonReaders.list(json, 'plans'))
        TaxPlan.fromJson(JsonReaders.object(item, 'plan')),
    ],
    pending: [
      for (final item in JsonReaders.list(json, 'pending'))
        PlannedActivity.fromJson(JsonReaders.object(item, 'pending')),
    ],
    pendingTotal: JsonReaders.money(json, 'pendingTotal'),
    pendingUnknownAmountCount: JsonReaders.integer(
      json,
      'pendingUnknownAmountCount',
    ),
    recentPayments: [
      for (final item in JsonReaders.list(json, 'recentPayments'))
        TaxPayment.fromJson(JsonReaders.object(item, 'payment')),
    ],
    hasMorePayments: JsonReaders.boolean(json, 'hasMorePayments'),
  );

  final String asOfDate;
  final List<TaxPlan> plans;

  /// Gecikenler ve 30 gün: planlanan projection'ın vergi dilimi.
  final List<PlannedActivity> pending;

  /// Bekleyenlerin (gecikenler dahil) tutarı belli olanlarının toplamı.
  final String pendingTotal;
  final int pendingUnknownAmountCount;
  final List<TaxPayment> recentPayments;
  final bool hasMorePayments;

  TaxPlan? planOf(String? planId) {
    for (final plan in plans) {
      if (plan.id == planId) return plan;
    }
    return null;
  }
}

/// Bir vergi kaleminin geçmişi: ödendi ya da kapatıldı.
class TaxHistoryItem {
  const TaxHistoryItem({
    required this.occurrenceId,
    required this.scheduledDate,
    required this.isClosed,
    required this.payment,
    this.amount,
  });

  factory TaxHistoryItem.fromJson(Map<String, dynamic> json) => TaxHistoryItem(
    occurrenceId: JsonReaders.string(json, 'occurrenceId'),
    scheduledDate: JsonReaders.date(json, 'scheduledDate'),
    isClosed: JsonReaders.string(json, 'status') == 'closed',
    amount: json['amount'] == null ? null : JsonReaders.money(json, 'amount'),
    payment: TaxPayment.fromJson(
      JsonReaders.object(json['payment'], 'payment'),
    ),
  );

  final String occurrenceId;
  final String scheduledDate;
  final bool isClosed;

  /// Kalemin kendi beklenen tutarı; toplu ödemede ödenen tutar kalemlere
  /// dağıtılmadığı için tek bilinen tutar budur.
  final String? amount;
  final TaxPayment payment;
}

class TaxPlanDetail {
  const TaxPlanDetail({
    required this.plan,
    required this.upcoming,
    required this.history,
  });

  factory TaxPlanDetail.fromJson(Map<String, dynamic> json) => TaxPlanDetail(
    plan: TaxPlan.fromJson(JsonReaders.object(json['plan'], 'plan')),
    upcoming: [
      for (final item in JsonReaders.list(json, 'upcoming'))
        PlannedActivity.fromJson(JsonReaders.object(item, 'upcoming')),
    ],
    history: [
      for (final item in JsonReaders.list(json, 'history'))
        TaxHistoryItem.fromJson(JsonReaders.object(item, 'history')),
    ],
  );

  final TaxPlan plan;

  /// Gecikmiş kalemlerin hepsi ve vadesi gelmemiş ilk üçü.
  final List<PlannedActivity> upcoming;
  final List<TaxHistoryItem> history;
}

/// Hazır türün önerisi: ritim ve gün, tutar yok.
class TaxSuggestion {
  const TaxSuggestion({
    required this.taxKind,
    required this.rhythm,
    required this.months,
    required this.dayOfMonth,
  });

  factory TaxSuggestion.fromJson(Map<String, dynamic> json) => TaxSuggestion(
    taxKind: TaxKind.fromApi(JsonReaders.string(json, 'taxKind')),
    rhythm: TaxRhythm.fromApi(JsonReaders.string(json, 'frequency')),
    months: [
      for (final month in (json['months'] as List<dynamic>? ?? const []))
        month as int,
    ],
    dayOfMonth: JsonReaders.integer(json, 'dayOfMonth'),
  );

  final TaxKind taxKind;
  final TaxRhythm rhythm;
  final List<int> months;
  final int dayOfMonth;
}

/// Ödeme ve tanım formlarının seçenekleri.
class TaxOptions {
  const TaxOptions({
    required this.accounts,
    required this.cards,
    required this.taxCategories,
  });

  final List<TaxChoice> accounts;
  final List<TaxChoice> cards;

  /// Vergi işaretli aktif gider kategorileri. Vergi yalnız bunlara yazılır.
  final List<TaxChoice> taxCategories;

  String? nameOf(String? id) {
    for (final choice in [...accounts, ...cards]) {
      if (choice.id == id) return choice.name;
    }
    return null;
  }

  bool isCard(String? id) => cards.any((card) => card.id == id);
}

class TaxChoice {
  const TaxChoice({required this.id, required this.name});

  final String id;
  final String name;
}

/// Vergi tanımı: oluşturma ve düzenleme aynı alanları gönderir.
class TaxPlanInput {
  const TaxPlanInput({
    required this.name,
    required this.taxKind,
    required this.rhythm,
    required this.startDate,
    required this.dayOfMonth,
    required this.categoryId,
    this.months = const [],
    this.amount,
    this.accountId,
    this.creditCardId,
    this.scope,
  });

  final String name;
  final TaxKind taxKind;
  final TaxRhythm rhythm;
  final String startDate;
  final int dayOfMonth;
  final String categoryId;
  final List<int> months;
  final String? amount;
  final String? accountId;
  final String? creditCardId;

  /// Açık seçim; boşsa sunucu profilin tarafını yazar (İ9).
  final TransactionScope? scope;

  Map<String, Object?> toCreateJson() => {
    ..._common(),
    'currency': 'TRY',
    'kind': 'expense',
    'taxKind': taxKind.apiValue,
  };

  Map<String, Object?> toUpdateJson() => _common();

  Map<String, Object?> _common() => {
    'sourceType': accountId != null
        ? 'account'
        : (creditCardId != null ? 'credit-card' : null),
    'accountId': accountId,
    'creditCardId': creditCardId,
    'categoryId': categoryId,
    'amount': amount,
    'scope': scope?.apiValue,
    'frequency': rhythm.apiValue,
    'startDate': startDate,
    'endDate': null,
    'monthEndBehavior': 'clamp-to-last-day',
    'description': name,
    'dayOfMonth': dayOfMonth,
    'months': rhythm == TaxRhythm.selectedMonths
        ? (months.toList()..sort())
        : null,
  };
}
