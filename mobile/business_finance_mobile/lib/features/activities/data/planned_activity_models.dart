import 'activity_models.dart';

enum PlannedKind {
  recurringOccurrence('recurring-occurrence'),
  cardInstallment('card-installment'),
  cardStatement('card-statement'),
  debtInstallment('debt-installment'),
  receivableInstallment('receivable-installment');

  const PlannedKind(this.apiValue);
  final String apiValue;

  static PlannedKind fromApi(String value) => switch (value) {
    'recurring-occurrence' => recurringOccurrence,
    'card-installment' => cardInstallment,
    'card-statement' => cardStatement,
    'debt-installment' => debtInstallment,
    'receivable-installment' => receivableInstallment,
    _ => throw FormatException('Bilinmeyen planlanan tür: $value'),
  };

  String get label => switch (this) {
    recurringOccurrence => 'Tekrarlanan',
    cardInstallment => 'Kart taksidi',
    cardStatement => 'Kart ekstresi',
    debtInstallment => 'Borç taksidi',
    receivableInstallment => 'Alacak taksidi',
  };
}

enum PlannedTiming {
  overdue('overdue'),
  today('today'),
  upcoming('upcoming');

  const PlannedTiming(this.apiValue);
  final String apiValue;

  static PlannedTiming fromApi(String value) => switch (value) {
    'overdue' => overdue,
    'today' => today,
    'upcoming' => upcoming,
    _ => throw FormatException('Bilinmeyen zamanlama: $value'),
  };

  String get label => switch (this) {
    overdue => 'Gecikmiş',
    today => 'Bugün',
    upcoming => 'Yaklaşan',
  };
}

enum PlannedReadiness {
  ready('ready'),
  needsAttention('needs-attention');

  const PlannedReadiness(this.apiValue);
  final String apiValue;

  static PlannedReadiness fromApi(String value) => switch (value) {
    'ready' => ready,
    'needs-attention' => needsAttention,
    _ => throw FormatException('Bilinmeyen hazır olma durumu: $value'),
  };
}

/// A stable machine code from the server, turned into a sentence here. The API
/// never sends user-facing text for it.
enum PlannedAttention {
  cardInactive('card-inactive'),
  cardLimitInsufficient('card-limit-insufficient'),
  accountInactive('account-inactive'),
  categoryInactive('category-inactive');

  const PlannedAttention(this.apiValue);
  final String apiValue;

  static PlannedAttention fromApi(String value) => switch (value) {
    'card-inactive' => cardInactive,
    'card-limit-insufficient' => cardLimitInsufficient,
    'account-inactive' => accountInactive,
    'category-inactive' => categoryInactive,
    _ => throw FormatException('Bilinmeyen dikkat kodu: $value'),
  };

  String get message => switch (this) {
    cardInactive =>
      'Kart pasif. Gerçekleştirmek için kartı yeniden aktifleştirin.',
    cardLimitInsufficient =>
      'Kullanılabilir kart limiti yetersiz. Limit boşaldığında tekrar deneyin.',
    accountInactive =>
      'Hesap pasif. Gerçekleştirmek için hesabı yeniden aktifleştirin.',
    categoryInactive =>
      'Kategori pasif. Gerçekleştirmek için kategoriyi aktifleştirin.',
  };
}

enum PlannedAction {
  realize('realize'),
  payCard('pay-card'),
  payDebt('pay-debt'),
  collectDebt('collect-debt');

  const PlannedAction(this.apiValue);
  final String apiValue;

  static PlannedAction fromApi(String value) => switch (value) {
    'realize' => realize,
    'pay-card' => payCard,
    'pay-debt' => payDebt,
    'collect-debt' => collectDebt,
    _ => throw FormatException('Bilinmeyen eylem: $value'),
  };

  String get label => switch (this) {
    realize => 'Gerçekleştir',
    payCard => 'Kart ödemesi yap',
    payDebt => 'Öde',
    collectDebt => 'Tahsil et',
  };
}

class PlannedActivity {
  const PlannedActivity({
    required this.plannedActivityId,
    required this.plannedKind,
    required this.effect,
    required this.timing,
    required this.readiness,
    required this.actionKind,
    required this.dueDate,
    required this.amount,
    required this.currency,
    required this.title,
    required this.isProjected,
    required this.isPaymentObligation,
    this.actionTargetId,
    this.actionSequence,
    this.attentionCode,
    this.description,
    this.sourceId,
    this.sourceName,
    this.categoryId,
    this.categoryName,
  });

  factory PlannedActivity.fromJson(Map<String, dynamic> json) =>
      PlannedActivity(
        plannedActivityId: json['plannedActivityId'] as String,
        plannedKind: PlannedKind.fromApi(json['plannedKind'] as String),
        effect: ActivityEffect.fromApi(json['effect'] as String),
        timing: PlannedTiming.fromApi(json['timing'] as String),
        readiness: PlannedReadiness.fromApi(json['readiness'] as String),
        attentionCode: json['attentionCode'] == null
            ? null
            : PlannedAttention.fromApi(json['attentionCode'] as String),
        actionKind: PlannedAction.fromApi(json['actionKind'] as String),
        dueDate: json['dueDate'] as String,
        amount: json['amount'] as String,
        currency: json['currency'] as String,
        title: json['title'] as String,
        description: json['description'] as String?,
        sourceId: json['sourceId'] as String?,
        sourceName: json['sourceName'] as String?,
        categoryId: json['categoryId'] as String?,
        categoryName: json['categoryName'] as String?,
        isProjected: json['isProjected'] as bool,
        isPaymentObligation: json['isPaymentObligation'] as bool,
        actionTargetId: json['actionTargetId'] as String?,
        actionSequence: json['actionSequence'] as int?,
      );

  final String plannedActivityId;
  final PlannedKind plannedKind;
  final ActivityEffect effect;
  final PlannedTiming timing;
  final PlannedReadiness readiness;
  final PlannedAttention? attentionCode;
  final PlannedAction actionKind;
  final String dueDate;
  final String amount;
  final String currency;
  final String title;
  final String? description;
  final String? sourceId;
  final String? sourceName;
  final String? categoryId;
  final String? categoryName;

  /// True while no occurrence row exists yet, so acting has to generate one
  /// first. The list says so, because the item is a forecast until then.
  final bool isProjected;

  /// Eylemin çağıracağı uç noktanın adreslediği kayıt.
  ///
  /// Satırın kendi kimliğinden farklı olabilir: kart taksidinde plan, ekstrede
  /// kart, borçta borç, henüz üretilmemiş tekrarlanan satırda **plan**
  /// kimliğidir.
  final String? actionTargetId;

  /// Aggregate içindeki sıra; uç nokta istemiyorsa `null`.
  final int? actionSequence;

  /// Kullanıcının **ödemesi gereken** bir yükümlülük mü.
  ///
  /// Sunucudan geliyor; `effect` ve `actionKind`'dan burada türetilseydi aynı
  /// kuralın ikinci bir kopyası doğar ve iki taraf sessizce ayrışabilirdi.
  final bool isPaymentObligation;

  bool get needsAttention => readiness == PlannedReadiness.needsAttention;

  String get listKey => '${plannedKind.apiValue}:$plannedActivityId';

  /// Vakti geldi mi.
  ///
  /// Plan, tarihi gelene kadar bir tahmindir. Gelecek ayın kirasını bugün
  /// gerçekleştirmek parayı çıkmadığı bir aya yazar ve tarihe göre okuyan her
  /// rapor o andan sonra yanlış olur (kullanıcı kararı, 20 Ağustos 2026).
  bool get isDue => timing != PlannedTiming.upcoming;

  /// Eylem tek dokunuşla burada tamamlanabilir mi.
  ///
  /// `realize` gövde istemiyor: onay dışında sorulacak bir şey yok, kayıt
  /// zaten tutarı ve tarihi taşıyor. Ödeme ve tahsilat ise hangi hesaptan
  /// yapılacağını sorar; onlar kendi ekranlarına gider.
  bool get isDirectlyRealizable =>
      actionKind == PlannedAction.realize &&
      actionTargetId != null &&
      isDue &&
      !needsAttention;
}

class PlannedActivityPage {
  const PlannedActivityPage({
    required this.asOfDate,
    required this.daysAhead,
    required this.totalCount,
    required this.items,
    this.nearestDueDate,
  });

  factory PlannedActivityPage.fromJson(Map<String, dynamic> json) =>
      PlannedActivityPage(
        asOfDate: json['asOfDate'] as String,
        daysAhead: json['daysAhead'] as int,
        totalCount: json['totalCount'] as int,
        nearestDueDate: json['nearestDueDate'] as String?,
        items: (json['items'] as List<dynamic>)
            .map(
              (item) => PlannedActivity.fromJson(item as Map<String, dynamic>),
            )
            .toList(growable: false),
      );

  final String asOfDate;
  final int daysAhead;
  final int totalCount;

  /// No single total is offered on purpose: adding planned income, expenses,
  /// statements and neutral obligations into one number would mislead.
  final String? nearestDueDate;
  final List<PlannedActivity> items;
}

/// The horizons the server accepts. Anything else is rejected there, so the UI
/// offers exactly these.
enum PlannedHorizon {
  week(7, '7 gün'),
  month(30, '30 gün'),
  quarter(90, '90 gün');

  const PlannedHorizon(this.days, this.label);
  final int days;
  final String label;
}

enum PlannedTypeFilter {
  all('Tümü'),
  recurring('Tekrarlanan'),
  installment('Taksit'),
  statement('Ekstre'),
  debt('Borç/Alacak');

  const PlannedTypeFilter(this.label);
  final String label;

  bool matches(PlannedActivity item) => switch (this) {
    all => true,
    recurring => item.plannedKind == PlannedKind.recurringOccurrence,
    installment => item.plannedKind == PlannedKind.cardInstallment,
    statement => item.plannedKind == PlannedKind.cardStatement,
    debt =>
      item.plannedKind == PlannedKind.debtInstallment ||
          item.plannedKind == PlannedKind.receivableInstallment,
  };
}
