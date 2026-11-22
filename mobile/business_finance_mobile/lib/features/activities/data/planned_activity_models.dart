import 'activity_models.dart';

enum PlannedKind {
  recurringOccurrence('recurring-occurrence'),
  cardInstallment('card-installment'),
  cardStatement('card-statement'),
  debtInstallment('debt-installment'),
  receivableInstallment('receivable-installment'),
  payableObligation('payable-obligation'),
  receivableObligation('receivable-obligation'),
  counterpartyPayable('counterparty-payable-due'),
  counterpartyReceivable('counterparty-receivable-due');

  const PlannedKind(this.apiValue);
  final String apiValue;

  static PlannedKind fromApi(String value) => switch (value) {
    'recurring-occurrence' => recurringOccurrence,
    'card-installment' => cardInstallment,
    'card-statement' => cardStatement,
    'debt-installment' => debtInstallment,
    'receivable-installment' => receivableInstallment,
    'payable-obligation' => payableObligation,
    'receivable-obligation' => receivableObligation,
    'counterparty-payable-due' => counterpartyPayable,
    'counterparty-receivable-due' => counterpartyReceivable,
    _ => throw FormatException('Bilinmeyen planlanan tür: $value'),
  };

  String get label => switch (this) {
    recurringOccurrence => 'Tekrarlanan',
    cardInstallment => 'Kart taksidi',
    cardStatement => 'Kart ekstresi',
    debtInstallment => 'Borç taksidi',
    receivableInstallment => 'Alacak taksidi',
    payableObligation => 'Ödenecek',
    receivableObligation => 'Tahsil edilecek',
    counterpartyPayable => 'Kişiye ödeme',
    counterpartyReceivable => 'Kişiden tahsilat',
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
  collectDebt('collect-debt'),
  payObligation('pay-obligation'),
  collectObligation('collect-obligation'),
  payCounterparty('pay-counterparty'),
  collectCounterparty('collect-counterparty');

  const PlannedAction(this.apiValue);
  final String apiValue;

  static PlannedAction fromApi(String value) => switch (value) {
    'realize' => realize,
    'pay-card' => payCard,
    'pay-debt' => payDebt,
    'collect-debt' => collectDebt,
    'pay-obligation' => payObligation,
    'collect-obligation' => collectObligation,
    'pay-counterparty' => payCounterparty,
    'collect-counterparty' => collectCounterparty,
    _ => throw FormatException('Bilinmeyen eylem: $value'),
  };

  String get label => switch (this) {
    realize => 'Gerçekleştir',
    payCard => 'Kart ödemesi yap',
    payDebt => 'Öde',
    collectDebt => 'Tahsil et',
    payObligation => 'Öde',
    collectObligation => 'Tahsil et',
    payCounterparty => 'Öde',
    collectCounterparty => 'Tahsil et',
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
    this.postsToCard = false,
    this.isMixedStatement = false,
    this.actionTargetId,
    this.actionSequence,
    this.attentionCode,
    this.description,
    this.sourceId,
    this.sourceName,
    this.categoryId,
    this.categoryName,
    this.recurringTransactionId,
    this.taxKind,
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
        amount: json['amount'] as String?,
        currency: json['currency'] as String,
        title: json['title'] as String,
        description: json['description'] as String?,
        sourceId: json['sourceId'] as String?,
        sourceName: json['sourceName'] as String?,
        categoryId: json['categoryId'] as String?,
        categoryName: json['categoryName'] as String?,
        isProjected: json['isProjected'] as bool,
        isPaymentObligation: json['isPaymentObligation'] as bool,
        postsToCard: json['postsToCard'] as bool? ?? false,
        isMixedStatement: json['isMixedStatement'] as bool? ?? false,
        actionTargetId: json['actionTargetId'] as String?,
        actionSequence: json['actionSequence'] as int?,
        recurringTransactionId: json['recurringTransactionId'] as String?,
        taxKind: json['taxKind'] as String?,
      );

  final String plannedActivityId;
  final PlannedKind plannedKind;
  final ActivityEffect effect;
  final PlannedTiming timing;
  final PlannedReadiness readiness;
  final PlannedAttention? attentionCode;
  final PlannedAction actionKind;
  final String dueDate;

  /// Beklenen tutar. **Boş olabilir**: tutarı ödeme gününe kadar belli olmayan
  /// vergi kalemi (ADR 0018 İ5). Boş tutar sıfır değildir; ekran "Tutar
  /// ödemede girilecek" der, toplamlar onu tahminle saymaz.
  final String? amount;
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
  final bool postsToCard;
  final bool isMixedStatement;

  String get displayTitle => isMixedStatement ? '$title · Karma ekstre' : title;

  String get paymentLabel {
    if (postsToCard || plannedKind == PlannedKind.cardInstallment) {
      return 'Karta yansıyacak';
    }
    if (plannedKind == PlannedKind.cardStatement) return 'Ekstre ödemesi';
    return isPaymentObligation ? 'Ödeme' : 'Beklenen tahsilat';
  }

  /// Satırı üreten tekrarlayan plan; yalnız tekrarlayan kalemde dolu.
  final String? recurringTransactionId;

  /// Vergi planının kalemiyse türü (`social-security-premium` …).
  final String? taxKind;

  bool get isTax => taxKind != null;

  /// Aynı kalem, bu dönem için yazılmış tutarla ("Tutarı gir"). Sunucudan
  /// yeni okuma gelene kadar açık panel eski tutarı göstermesin diye.
  PlannedActivity withAmount(String value) => PlannedActivity(
    plannedActivityId: plannedActivityId,
    plannedKind: plannedKind,
    effect: effect,
    timing: timing,
    readiness: readiness,
    attentionCode: attentionCode,
    actionKind: actionKind,
    dueDate: dueDate,
    amount: value,
    currency: currency,
    title: title,
    description: description,
    sourceId: sourceId,
    sourceName: sourceName,
    categoryId: categoryId,
    categoryName: categoryName,
    isProjected: isProjected,
    isPaymentObligation: isPaymentObligation,
    actionTargetId: actionTargetId,
    actionSequence: actionSequence,
    recurringTransactionId: recurringTransactionId,
    taxKind: taxKind,
    postsToCard: postsToCard,
    isMixedStatement: isMixedStatement,
  );

  bool get needsAttention => readiness == PlannedReadiness.needsAttention;

  String get listKey => '${plannedKind.apiValue}:$plannedActivityId';

  /// Vakti geldi mi.
  bool get isDue => timing != PlannedTiming.upcoming;

  /// Vadesi gelmeden gerçekleştirilebilir mi.
  ///
  /// Tekrarlayan kalem ödendiği güne yazılır, vadeye değil; kirayı erken
  /// ödemek parayı çıkmadığı bir aya koymaz (kullanıcı kararı, 12 Ekim 2026;
  /// 20 Ağustos 2026 kararının yerini alır). Kart taksidi kendi gününe
  /// yazılır ve günü gelmeden gerçekleşmez.
  bool get isRealizableNow =>
      isDue || plannedKind == PlannedKind.recurringOccurrence;

  /// Eylem tek dokunuşla burada tamamlanabilir mi.
  ///
  /// `realize` tutarı ve kaynağı sormaz, kayıt onları taşır; tekrarlayan
  /// kalemde yalnız gün sorulur. Ödeme ve tahsilat ise hangi hesaptan
  /// yapılacağını sorar; onlar kendi ekranlarına gider.
  ///
  /// Vergi kalemi burada gerçekleştirilmez: tutar, ödeme günü ve hesap/kart
  /// ister ve bunları vergi ekranının "Ödedim" paneli sorar (ADR 0018 T4).
  bool get isDirectlyRealizable =>
      actionKind == PlannedAction.realize &&
      actionTargetId != null &&
      !isTax &&
      amount != null &&
      isRealizableNow &&
      !needsAttention;
}

/// Seçilen aralığın özeti (`from` / `to` ile okunduğunda). Bütün tutarlar
/// sunucudandır ve birbirinden düşülmez.
class PlannedPeriod {
  const PlannedPeriod({
    required this.from,
    required this.to,
    required this.paymentTotal,
    required this.paymentCount,
    required this.unknownAmountCount,
    required this.incomingTotal,
    required this.incomingCount,
    required this.cardPostingTotal,
    required this.cardPostingCount,
    required this.earlierOverdueCount,
    required this.earlierOverduePaymentTotal,
    required this.earlierOverdueUnknownAmountCount,
  });

  factory PlannedPeriod.fromJson(Map<String, dynamic> json) => PlannedPeriod(
    from: json['from'] as String,
    to: json['to'] as String,
    paymentTotal: json['paymentTotal'] as String,
    paymentCount: json['paymentCount'] as int,
    unknownAmountCount: json['unknownAmountCount'] as int,
    incomingTotal: json['incomingTotal'] as String,
    incomingCount: json['incomingCount'] as int,
    cardPostingTotal: json['cardPostingTotal'] as String,
    cardPostingCount: json['cardPostingCount'] as int,
    earlierOverdueCount: json['earlierOverdueCount'] as int,
    earlierOverduePaymentTotal: json['earlierOverduePaymentTotal'] as String,
    earlierOverdueUnknownAmountCount:
        json['earlierOverdueUnknownAmountCount'] as int,
  );

  final String from;
  final String to;
  final String paymentTotal;
  final int paymentCount;

  /// Ödemelerden tutarı belli olmayanların sayısı; toplama girmez.
  final int unknownAmountCount;
  final String incomingTotal;
  final int incomingCount;
  final String cardPostingTotal;
  final int cardPostingCount;

  /// Vadesi hem seçimin başlangıcından hem bugünden önce olan açık kayıtlar.
  final int earlierOverdueCount;
  final String earlierOverduePaymentTotal;
  final int earlierOverdueUnknownAmountCount;
}

class PlannedActivityPage {
  const PlannedActivityPage({
    required this.asOfDate,
    required this.daysAhead,
    required this.totalCount,
    required this.items,
    this.nearestDueDate,
    this.upcomingOutgoingTotal,
    this.upcomingPaymentCount,
    this.unknownAmountCount,
    this.overdueOutgoingTotal,
    this.period,
    this.skippedCount = 0,
  });

  /// Uygulamanın tanımadığı türdeki satır atlanır ve sayılır; ekran bunu
  /// söyler. Önceki hâlde tek bilinmeyen tür bütün listeyi açılmaz yapıyordu
  /// (yeni sunucu, eski uygulama).
  factory PlannedActivityPage.fromJson(Map<String, dynamic> json) {
    final items = <PlannedActivity>[];
    var skipped = 0;
    for (final item in json['items'] as List<dynamic>) {
      try {
        items.add(PlannedActivity.fromJson(item as Map<String, dynamic>));
      } on FormatException {
        skipped++;
      }
    }
    return PlannedActivityPage(
      upcomingOutgoingTotal: json['upcomingOutgoingTotal'] as String?,
      upcomingPaymentCount: json['upcomingPaymentCount'] as int?,
      unknownAmountCount: json['unknownAmountCount'] as int?,
      overdueOutgoingTotal: json['overdueOutgoingTotal'] as String?,
      asOfDate: json['asOfDate'] as String,
      daysAhead: json['daysAhead'] as int,
      totalCount: json['totalCount'] as int,
      nearestDueDate: json['nearestDueDate'] as String?,
      items: List.unmodifiable(items),
      period: json['period'] == null
          ? null
          : PlannedPeriod.fromJson(json['period'] as Map<String, dynamic>),
      skippedCount: skipped,
    );
  }

  /// `from` / `to` ile okunduğunda seçilen aralığın özeti; yoksa `null`.
  final PlannedPeriod? period;

  /// Bu sürümün gösteremediği satır sayısı.
  final int skippedCount;

  final String asOfDate;
  final int daysAhead;
  final int totalCount;

  /// No mixed total is offered: planned income, expenses, statements and
  /// neutral obligations are never added into one number.
  final String? nearestDueDate;
  final List<PlannedActivity> items;

  /// Pencere içinde vadesi gelmemiş **ödeme yükümlülüklerinin** toplamı
  /// ("7 günde çıkacak"). Sunucuda toplanır; gelir, tahsilat ve gecikmişler
  /// girmez. Eski sunucu göndermezse `null`.
  final String? upcomingOutgoingTotal;
  final int? upcomingPaymentCount;
  final int? unknownAmountCount;

  /// Gecikmiş ödeme yükümlülüklerinin tutarı belli olanlarının toplamı;
  /// pencereden bağımsız. Sunucuda toplanır, eski sunucu göndermezse `null`.
  final String? overdueOutgoingTotal;
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

/// Planlananlar'ın tür seçimi (Codex 18-02, 20 Kasım 2026). Kişiye ödeme ve
/// tahsilat yönüne göre borç ve alacak taksitleriyle birliktedir; vergi
/// tekrarlayandan ayrıdır.
enum PlannedTypeFilter {
  all('Tüm türler', null),
  recurring('Tekrarlayan', 'recurring'),
  statement('Kart ekstresi', 'statement'),
  installment('Kart taksidi', 'installment'),
  payable('Borç ödemesi', 'payable'),
  receivable('Alacak tahsilatı', 'receivable'),
  tax('Vergi', 'tax');

  const PlannedTypeFilter(this.label, this.apiValue);
  final String label;

  /// Sunucunun `type` değeri; tümünde gönderilmez.
  final String? apiValue;

  bool matches(PlannedActivity item) => switch (this) {
    all => true,
    recurring =>
      item.plannedKind == PlannedKind.recurringOccurrence && !item.isTax,
    installment => item.plannedKind == PlannedKind.cardInstallment,
    statement => item.plannedKind == PlannedKind.cardStatement,
    payable =>
      item.plannedKind == PlannedKind.debtInstallment ||
          item.plannedKind == PlannedKind.counterpartyPayable ||
          item.plannedKind == PlannedKind.payableObligation,
    receivable =>
      item.plannedKind == PlannedKind.receivableInstallment ||
          item.plannedKind == PlannedKind.counterpartyReceivable ||
          item.plannedKind == PlannedKind.receivableObligation,
    tax => item.isTax,
  };
}
