import '../../../core/models/transaction_scope.dart';

/// Which real event happened. The backend sends stable machine values; the label
/// shown to the user is built here, because the API deliberately carries no
/// display sentences.
enum ActivityKind {
  accountTransaction('account-transaction'),
  transfer('transfer'),
  cardCharge('card-charge'),
  cardPayment('card-payment'),
  debtPayment('debt-payment'),
  debtCollection('debt-collection'),

  /// Borcun doğduğu an. Nakit kaynakta para el değiştirir, gider kaynakta
  /// tüketim olur; tek tür iki farklı etki taşır.
  debtOpening('debt-opening'),

  /// Veresiye satış ya da vadeli alım: gelir/gider o gün tanınır, kasa
  /// kıpırdamaz.
  counterpartyCharge('counterparty-charge'),

  /// Cari tahsilat ya da ödeme: kasa değişir, gelir/gider üretilmez.
  counterpartySettlement('counterparty-settlement'),

  /// Tek seferlik borç veya alacak doğuşu; nakdi değil gelir/gideri etkiler.
  obligation('obligation'),

  /// Yükümlülüğü kapatan nakit hareketi: kasa değişir, gelir/gider yeniden
  /// tanınmaz — ekonomik olay yükümlülük doğarken tanınmıştı.
  obligationSettlement('obligation-settlement'),

  /// POS satışının tanındığı an: gelir brüt tutar kadar yazılır, hesap
  /// kıpırdamaz. Bankanın kestiği komisyon ayrı satır değildir; bu satırın
  /// parçasıdır ([FinancialActivity.feeAmount]).
  posSale('pos-sale'),

  /// Yoldaki paranın hesaba yattığı an (ADR 0019 T5): bir yatış bir ya da
  /// birkaç tahsilatı kapatır, gelir/gider yeniden tanınmaz. Tutar bankanın
  /// gerçekten yatırdığı tutardır; eksik kalan kısım (kesinti) bu satırın
  /// parçasıdır ([FinancialActivity.feeAmount]).
  posDeposit('pos-deposit');

  const ActivityKind(this.apiValue);
  final String apiValue;

  static ActivityKind fromApi(String value) => switch (value) {
    'account-transaction' => accountTransaction,
    'transfer' => transfer,
    'card-charge' => cardCharge,
    'card-payment' => cardPayment,
    'debt-payment' => debtPayment,
    'debt-collection' => debtCollection,
    'debt-opening' => debtOpening,
    'counterparty-charge' => counterpartyCharge,
    'counterparty-settlement' => counterpartySettlement,
    'obligation' => obligation,
    'obligation-settlement' => obligationSettlement,
    'pos-sale' => posSale,
    'pos-deposit' => posDeposit,
    _ => throw FormatException('Bilinmeyen hareket türü: $value'),
  };

  String get label => switch (this) {
    accountTransaction => 'İşlem',
    transfer => 'Transfer',
    cardCharge => 'Kart harcaması',
    cardPayment => 'Kart ödemesi',
    debtPayment => 'Borç ödemesi',
    debtCollection => 'Alacak tahsilatı',
    // Tek tür hem borcu hem alacağı kapsıyor; satırın kendisi kiminle
    // olduğunu zaten yazıyor.
    debtOpening => 'Borç / alacak açılışı',
    // Yön satırın kendisinde: alacak doğuran kayıt gelir, borç doğuran
    // gider olarak görünür ve etiket ikisini birden karşılar.
    counterpartyCharge => 'Cari hareket',
    counterpartySettlement => 'Cari tahsilat / ödeme',
    obligation => 'Yükümlülük',
    obligationSettlement => 'Yükümlülük ödemesi',
    // Bu iki etikette `kart` kelimesi tek başına geçmez (ADR 0015): borçlandığın
    // kart başka bir şeydir ve ikisi aynı listede yan yana görünüyor.
    posSale => 'POS satışı',
    posDeposit => 'POS yatışı',
  };
}

enum ActivityEffect {
  income('income'),
  expense('expense'),
  neutral('neutral');

  const ActivityEffect(this.apiValue);
  final String apiValue;

  static ActivityEffect fromApi(String value) => switch (value) {
    'income' => income,
    'expense' => expense,
    'neutral' => neutral,
    _ => throw FormatException('Bilinmeyen hareket etkisi: $value'),
  };
}

enum ActivitySourceGroup {
  account('account'),
  creditCard('credit-card'),
  transfer('transfer'),
  debt('debt'),

  /// Açık cari. Taksitli sözleşme `debt` olarak kalır: aynı kişiye ait
  /// olsalar bile biri yürüyen bir hesap, diğeri vadesi belli bir plandır.
  counterparty('counterparty'),
  obligation('obligation'),

  /// POS tahsilatı. Kredi kartından ayrı bir gruptur ve olmak zorundadır:
  /// biri borçlandığın kart, diğeri müşterinin ödediği para (ADR 0015).
  pos('pos');

  const ActivitySourceGroup(this.apiValue);
  final String apiValue;

  static ActivitySourceGroup fromApi(String value) => switch (value) {
    'account' => account,
    'credit-card' => creditCard,
    'transfer' => transfer,
    'debt' => debt,
    'counterparty' => counterparty,
    'obligation' => obligation,
    'pos' => pos,
    _ => throw FormatException('Bilinmeyen kaynak grubu: $value'),
  };
}

enum ActivityOrigin {
  manual('manual'),
  csvImport('csv-import'),
  recurring('recurring'),
  installment('installment'),

  /// Bir POS yatışının kesinti gideri: yatışla birlikte doğar ve yalnız
  /// onunla birlikte geri alınır.
  posDeposit('pos-deposit');

  const ActivityOrigin(this.apiValue);
  final String apiValue;

  static ActivityOrigin fromApi(String value) => switch (value) {
    'manual' => manual,
    'csv-import' => csvImport,
    'recurring' => recurring,
    'installment' => installment,
    'pos-deposit' => posDeposit,
    _ => throw FormatException('Bilinmeyen hareket kökeni: $value'),
  };

  String get label => switch (this) {
    manual => 'Elle eklendi',
    csvImport => 'CSV içe aktarma',
    recurring => 'Tekrarlayan plan',
    installment => 'Taksit planı',
    posDeposit => 'Yatış kesintisi',
  };
}

enum ActivityStatus {
  realized('realized'),
  cancelled('cancelled');

  const ActivityStatus(this.apiValue);
  final String apiValue;

  static ActivityStatus fromApi(String value) => switch (value) {
    'realized' => realized,
    'cancelled' => cancelled,
    _ => throw FormatException('Bilinmeyen hareket durumu: $value'),
  };
}

class FinancialActivity {
  const FinancialActivity({
    required this.activityId,
    required this.kind,
    required this.effect,
    required this.sourceGroup,
    required this.origin,
    required this.status,
    required this.activityDate,
    required this.amount,
    required this.currency,
    required this.title,
    required this.canCancel,
    required this.supportsAttachments,
    this.description,
    this.categoryId,
    this.categoryName,
    this.sourceId,
    this.sourceName,
    this.destinationId,
    this.destinationName,
    this.cancelledAtUtc,
    this.principalPortion,
    this.interestPortion,
    this.scope,
    this.channelName,
    this.feeAmount,
    this.netAmount,
    this.direction,
    this.expectedTransferDate,
    this.transferredOn,
    this.settlementCount,
  });

  factory FinancialActivity.fromJson(Map<String, dynamic> json) =>
      FinancialActivity(
        activityId: json['activityId'] as String,
        kind: ActivityKind.fromApi(json['activityKind'] as String),
        effect: ActivityEffect.fromApi(json['effect'] as String),
        sourceGroup: ActivitySourceGroup.fromApi(json['sourceGroup'] as String),
        origin: ActivityOrigin.fromApi(json['origin'] as String),
        status: ActivityStatus.fromApi(json['status'] as String),
        activityDate: json['activityDate'] as String,
        amount: json['amount'] as String,
        currency: json['currency'] as String,
        title: json['title'] as String,
        description: json['description'] as String?,
        categoryId: json['categoryId'] as String?,
        categoryName: json['categoryName'] as String?,
        sourceId: json['sourceId'] as String?,
        sourceName: json['sourceName'] as String?,
        destinationId: json['destinationId'] as String?,
        destinationName: json['destinationName'] as String?,
        cancelledAtUtc: json['cancelledAtUtc'] as String?,
        principalPortion: json['principalPortion'] as String?,
        interestPortion: json['interestPortion'] as String?,
        scope: json['scope'] is String
            ? TransactionScope.fromApi(json['scope'] as String)
            : null,
        canCancel: json['canCancel'] as bool,
        supportsAttachments: json['supportsAttachments'] as bool,
        channelName: json['channelName'] as String?,
        feeAmount: json['feeAmount'] as String?,
        netAmount: json['netAmount'] as String?,
        expectedTransferDate: json['expectedTransferDate'] as String?,
        transferredOn: json['transferredOn'] as String?,
        settlementCount: json['settlementCount'] as int?,
        direction: switch (json['direction']) {
          null => null,
          'receivable' => ActivityDirection.receivable,
          'payable' => ActivityDirection.payable,
          final other => throw FormatException('Bilinmeyen yön: $other'),
        },
      );

  final String activityId;
  final ActivityKind kind;
  final ActivityEffect effect;
  final ActivitySourceGroup sourceGroup;
  final ActivityOrigin origin;
  final ActivityStatus status;
  final String activityDate;
  final String amount;
  final String currency;
  final String title;
  final String? description;
  final String? categoryId;
  final String? categoryName;
  final String? sourceId;
  final String? sourceName;
  final String? destinationId;
  final String? destinationName;
  final String? cancelledAtUtc;

  /// Borç taksidinin anapara ve faiz payı; diğer türlerde `null`.
  ///
  /// Ayrı bir hareket **değil**, bu hareketin bölünmesi: 2.600 TL'lik taksidin
  /// içindeki 100 TL faiz. Ayrı satır olsaydı işlemler toplamı hesaptan çıkan
  /// parayla tutmazdı (2.600 + 100 = 2.700).
  ///
  /// İkisi de sunucudan geliyor; anapara burada çıkarılmıyor çünkü istemci
  /// para aritmetiği yapmaz.
  final String? principalPortion;

  /// Faiz payı; ayrıntısı [principalPortion] belgesinde.
  final String? interestPortion;
  final bool canCancel;
  final bool supportsAttachments;

  /// Kaydın kapsamı; transfer ve kart ödemesinde `null` (gelir/gider
  /// raporuna sıfır etki ederler ve kapsam taşımazlar).
  final TransactionScope? scope;

  /// Kaydın geldiği POS'un adı (POS satışı ve yatışı); POS seçilmeden
  /// girilende `null`.
  final String? channelName;

  /// Kaydın **parçası** olan gider: POS satışında bankanın kestiği komisyon,
  /// yatışta eksik yatan kesinti. Ayrı satır değildir; bağlı olduğu kaydın
  /// satırında ve ayrıntısında gösterilir. Sıfırsa ya da yoksa `null`.
  final String? feeAmount;

  /// POS satışında hesaba geçecek (ya da geçmiş) net tutar.
  final String? netAmount;

  /// POS satışında paranın beklendiği gün ve, geçtiyse, geçtiği gün.
  final String? expectedTransferDate;
  final String? transferredOn;

  /// Yatışın kapattığı tahsilat sayısı.
  final int? settlementCount;

  /// Cari ve yükümlülük kayıtlarında yön; diğer türlerde boştur.
  final ActivityDirection? direction;

  /// Türün adı. Tahsilat ile ödeme aynı türdür; hangisi olduğunu yön söyler.
  String get kindLabel => switch ((kind, direction)) {
    (ActivityKind.counterpartySettlement, ActivityDirection.receivable) =>
      'Cari tahsilat',
    (ActivityKind.counterpartySettlement, ActivityDirection.payable) =>
      'Cari ödeme',
    (ActivityKind.obligationSettlement, ActivityDirection.receivable) =>
      'Alacak tahsilatı',
    (ActivityKind.obligationSettlement, ActivityDirection.payable) =>
      'Borç ödemesi',
    _ => kind.label,
  };

  /// Para karşı taraftan hesaba mı geliyor: alacağın tahsilatı.
  bool get isCollection =>
      direction == ActivityDirection.receivable &&
      (kind == ActivityKind.counterpartySettlement ||
          kind == ActivityKind.obligationSettlement);

  bool get hasFee =>
      feeAmount != null &&
      feeAmount!.replaceAll(RegExp('[^0-9]'), '').contains(RegExp('[1-9]'));

  bool get isCancelled => status == ActivityStatus.cancelled;

  /// Ids repeat across write models, so a list key needs the kind as well.
  String get listKey => '${kind.apiValue}:$activityId';
}

/// Bir hareketten hemen sonra hesabın bakiyesi ya da kartın borcu. Sunucu
/// hesaplar; istemci para aritmetiği yapmaz.
/// Hareketin o bakiyeye ne yaptığı. Sunucu söyler; istemci türden türetmez
/// (cari tahsilatın yönü satırda yoktur).
enum ActivityBalanceChange {
  increased,
  decreased,

  /// Hareket hesaba dokunmadı: POS satışı tanır, para henüz yoldadır.
  unchanged,
}

/// Cari ve yükümlülük kaydının yönü: `receivable` karşı taraf bize borçlu,
/// `payable` biz ona borçluyuz.
enum ActivityDirection { receivable, payable }

/// Bakiyesi gösterilen yer.
enum ActivityBalanceHolder {
  account,
  card,

  /// Karşı tarafın açık cari bakiyesi.
  counterparty,

  /// Borç anlaşmasının kalan tutarı.
  debt,
}

/// Cari ve borç bakiyesinin tarafı. Tutar hep artıdır; kimin kime borçlu
/// olduğunu sunucu söyler.
enum ActivityBalanceSide { receivable, payable, settled }

class ActivityBalance {
  const ActivityBalance({
    required this.holder,
    required this.name,
    required this.balance,
    required this.currency,
    required this.change,
    this.availableLimit,
    this.side,
    this.previousSide,
  });

  factory ActivityBalance.fromJson(Map<String, dynamic> json) =>
      ActivityBalance(
        holder: switch (json['holder']) {
          'account' => ActivityBalanceHolder.account,
          'credit-card' => ActivityBalanceHolder.card,
          'counterparty' => ActivityBalanceHolder.counterparty,
          'debt' => ActivityBalanceHolder.debt,
          final other => throw FormatException(
            'Bilinmeyen bakiye yeri: $other',
          ),
        },
        availableLimit: json['availableLimit'] as String?,
        side: _side(json['side']),
        previousSide: _side(json['previousSide']),
        name: json['name'] as String,
        balance: json['balance'] as String,
        currency: json['currency'] as String,
        change: switch (json['change']) {
          'increased' => ActivityBalanceChange.increased,
          'decreased' => ActivityBalanceChange.decreased,
          'unchanged' => ActivityBalanceChange.unchanged,
          final other => throw FormatException(
            'Bilinmeyen bakiye değişimi: $other',
          ),
        },
      );

  final ActivityBalanceHolder holder;
  final String name;

  /// Hesapta bakiye, kartta borç, caride açık tutar, borçta kalan tutar.
  final String balance;
  final String currency;
  final ActivityBalanceChange change;

  /// Yalnız kartta: o andaki borca göre kalan limit. Kartın bugünkü
  /// limitiyle hesaplanır; limitin geçmişi tutulmaz.
  final String? availableLimit;

  /// Yalnız cari ve borçta.
  final ActivityBalanceSide? side;

  /// Yalnız caride: hareketten hemen önceki taraf.
  final ActivityBalanceSide? previousSide;

  bool get isCard => holder == ActivityBalanceHolder.card;

  /// Hareket tarafı çevirdi: alacak kapandı ve geriye borç kaldı, ya da tersi.
  /// İlk kayıt (önce açık hesap yoktu) ve kapanış çevirme sayılmaz.
  bool get flippedSide =>
      (previousSide == ActivityBalanceSide.receivable &&
          side == ActivityBalanceSide.payable) ||
      (previousSide == ActivityBalanceSide.payable &&
          side == ActivityBalanceSide.receivable);

  static ActivityBalanceSide? _side(Object? value) => switch (value) {
    null => null,
    'receivable' => ActivityBalanceSide.receivable,
    'payable' => ActivityBalanceSide.payable,
    'settled' => ActivityBalanceSide.settled,
    final other => throw FormatException('Bilinmeyen bakiye tarafı: $other'),
  };
}

class ActivityPagination {
  const ActivityPagination({
    required this.pageNumber,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
    required this.hasPreviousPage,
    required this.hasNextPage,
  });

  factory ActivityPagination.fromJson(Map<String, dynamic> json) =>
      ActivityPagination(
        pageNumber: json['pageNumber'] as int,
        pageSize: json['pageSize'] as int,
        totalCount: json['totalCount'] as int,
        totalPages: json['totalPages'] as int,
        hasPreviousPage: json['hasPreviousPage'] as bool,
        hasNextPage: json['hasNextPage'] as bool,
      );

  final int pageNumber;
  final int pageSize;
  final int totalCount;
  final int totalPages;
  final bool hasPreviousPage;
  final bool hasNextPage;
}

class ActivityPage {
  const ActivityPage({required this.items, required this.pagination});

  factory ActivityPage.fromJson(Map<String, dynamic> json) => ActivityPage(
    items: (json['items'] as List<dynamic>)
        .map((item) => FinancialActivity.fromJson(item as Map<String, dynamic>))
        .toList(growable: false),
    pagination: ActivityPagination.fromJson(
      json['pagination'] as Map<String, dynamic>,
    ),
  );

  final List<FinancialActivity> items;
  final ActivityPagination pagination;
}

/// The quick chips are shortcuts over two independent dimensions, not one enum:
/// three of them narrow the source group while `recurring` narrows the origin.
enum ActivityQuickFilter {
  all('Tümü'),
  accounts('Hesaplar'),
  creditCards('Kredi kartları'),
  transfers('Transferler'),
  debts('Borçlar'),
  recurring('Tekrarlananlar');

  const ActivityQuickFilter(this.label);
  final String label;

  ActivitySourceGroup? get sourceGroup => switch (this) {
    accounts => ActivitySourceGroup.account,
    creditCards => ActivitySourceGroup.creditCard,
    transfers => ActivitySourceGroup.transfer,
    debts => ActivitySourceGroup.debt,
    all || recurring => null,
  };

  ActivityOrigin? get origin =>
      this == recurring ? ActivityOrigin.recurring : null;
}

/// Ready-made ranges so narrowing the history is a visible choice. The default is
/// [all]: the feed never hides older movements behind a window the user did not
/// ask for.
enum ActivityDateRange {
  all('Tümü'),
  thisMonth('Bu ay'),
  lastThreeMonths('Son 3 ay'),
  lastTwelveMonths('Son 12 ay');

  const ActivityDateRange(this.label);
  final String label;

  String? dateFrom(DateTime today) {
    final from = switch (this) {
      all => null,
      thisMonth => DateTime(today.year, today.month, 1),
      lastThreeMonths => DateTime(today.year, today.month - 2, 1),
      lastTwelveMonths => DateTime(today.year, today.month - 11, 1),
    };
    if (from == null) return null;
    final month = from.month.toString().padLeft(2, '0');
    final day = from.day.toString().padLeft(2, '0');
    return '${from.year}-$month-$day';
  }
}

class ActivityFilter {
  const ActivityFilter({
    this.quickFilter = ActivityQuickFilter.all,
    this.dateRange = ActivityDateRange.all,
    this.effect,
    this.accountId,
    this.creditCardId,
    this.categoryId,
    this.includeCancelled = true,
    this.search,
  });

  final ActivityQuickFilter quickFilter;

  /// Kayıt adı, kategori ve hesap adında aranan metin; sunucuda aranır, çünkü
  /// sayfalı listenin yalnız yüklenen kısmını süzmek eksik sonuç verirdi.
  final String? search;
  final ActivityDateRange dateRange;
  final ActivityEffect? effect;
  final String? accountId;
  final String? creditCardId;
  final String? categoryId;
  final bool includeCancelled;

  bool get hasAdvancedFilters =>
      dateRange != ActivityDateRange.all ||
      effect != null ||
      accountId != null ||
      creditCardId != null ||
      categoryId != null ||
      !includeCancelled;

  ActivityFilter copyWith({
    ActivityQuickFilter? quickFilter,
    ActivityDateRange? dateRange,
    ActivityEffect? effect,
    String? accountId,
    String? creditCardId,
    String? categoryId,
    bool? includeCancelled,
    bool clearEffect = false,
    bool clearAccountId = false,
    bool clearCreditCardId = false,
    bool clearCategoryId = false,
    String? search,
    bool clearSearch = false,
  }) => ActivityFilter(
    search: clearSearch ? null : (search ?? this.search),
    quickFilter: quickFilter ?? this.quickFilter,
    dateRange: dateRange ?? this.dateRange,
    effect: clearEffect ? null : (effect ?? this.effect),
    accountId: clearAccountId ? null : (accountId ?? this.accountId),
    creditCardId: clearCreditCardId
        ? null
        : (creditCardId ?? this.creditCardId),
    categoryId: clearCategoryId ? null : (categoryId ?? this.categoryId),
    includeCancelled: includeCancelled ?? this.includeCancelled,
  );
}
