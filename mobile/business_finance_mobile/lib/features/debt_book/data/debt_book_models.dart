import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';

/// Borcun tarafı: `receivable` alacak, `payable` borç.
enum DebtSide {
  receivable('receivable'),
  payable('payable');

  const DebtSide(this.apiValue);
  final String apiValue;

  static DebtSide fromApi(String value) => switch (value) {
    'receivable' => receivable,
    'payable' => payable,
    _ => throw FormatException('Bilinmeyen borç tarafı: $value'),
  };
}

/// Listenin bölümü (`GET /api/v1/debt-book`, `section`).
enum DebtBookSection {
  overdue('overdue'),
  upcoming('upcoming'),
  other('other');

  const DebtBookSection(this.apiValue);
  final String apiValue;

  /// Tanınmayan bölüm `Diğer`e düşer: yeni bir bölüm listeyi bozmaz.
  static DebtBookSection fromApi(String value) => switch (value) {
    'overdue' => overdue,
    'upcoming' => upcoming,
    _ => other,
  };
}

TransactionScope? _scopeOf(Map<String, dynamic> json) {
  final value = JsonReaders.nullableString(json, 'scope');
  if (value == null) return null;
  for (final scope in TransactionScope.values) {
    if (scope.apiValue == value) return scope;
  }
  return null;
}

/// Listenin bir satırı: kişinin taksitsiz hesabı (`ledger`, kimlik kişinin)
/// ya da taksitli veya kişisiz kayıt (`debt`, kimlik borcun). Kişi listede bir
/// kez durur; taksitli kaydı kendi satırıdır (ADR 0022, K14).
class DebtBookItem {
  const DebtBookItem({
    required this.isLedger,
    required this.id,
    required this.side,
    required this.section,
    required this.title,
    required this.remaining,
    required this.overdue,
    this.counterpartyId,
    this.counterpartyName,
    this.planKind,
    this.nextDueDate,
    this.nextDueAmount = '0.0000',
    this.scope,
    this.advanceAmount = '0.0000',
    this.oldestOverdueDate,
    this.installmentCount,
    this.openInstallmentCount = 0,
  });

  factory DebtBookItem.fromJson(Map<String, dynamic> json) => DebtBookItem(
    isLedger: JsonReaders.string(json, 'kind') == 'ledger',
    id: JsonReaders.string(json, 'id'),
    side: DebtSide.fromApi(JsonReaders.string(json, 'direction')),
    section: DebtBookSection.fromApi(JsonReaders.string(json, 'section')),
    title: JsonReaders.string(json, 'title'),
    remaining: JsonReaders.string(json, 'remaining'),
    overdue: JsonReaders.string(json, 'overdue'),
    counterpartyId: JsonReaders.nullableString(json, 'counterpartyId'),
    counterpartyName: JsonReaders.nullableString(json, 'counterpartyName'),
    planKind: JsonReaders.nullableString(json, 'planKind'),
    nextDueDate: JsonReaders.nullableString(json, 'nextDueDate'),
    nextDueAmount:
        JsonReaders.nullableString(json, 'nextDueAmount') ?? '0.0000',
    scope: _scopeOf(json),
    advanceAmount:
        JsonReaders.nullableString(json, 'advanceAmount') ?? '0.0000',
    oldestOverdueDate: JsonReaders.nullableString(json, 'oldestOverdueDate'),
    installmentCount: JsonReaders.nullableInt(json, 'installmentCount'),
    openInstallmentCount:
        JsonReaders.nullableInt(json, 'openInstallmentCount') ?? 0,
  );

  final bool isLedger;
  final String id;
  final DebtSide side;
  final DebtBookSection section;
  final String title;
  final String remaining;
  final String overdue;
  final String? counterpartyId;
  final String? counterpartyName;

  /// Kayıt satırında `installments`, `single` ya da `none`.
  final String? planKind;
  final String? nextDueDate;
  final String nextDueAmount;

  /// Kişi satırında kişinin türü, kayıt satırında kaydın kapsamı. Türü
  /// bilinmeyen eski kişide boştur ve uydurulmaz.
  final TransactionScope? scope;

  /// Kişi satırında tutarın karşı yöndeki fazla ödemeden (avans) gelen kısmı.
  final String advanceAmount;
  final String? oldestOverdueDate;
  final int? installmentCount;
  final int openInstallmentCount;

  bool get hasOverdue => _positive(overdue);

  /// Taksitli kayıt. Plan türünden okunur: sunucu tek ödemeli kayıtta da
  /// taksit sayısı (1) gönderir ve `0/1 taksit` yazılırdı.
  bool get isInstallment => !isLedger && planKind == 'installments';

  /// Tutarın tamamı avanstır: satır "Alınan avans" yazar.
  bool get isAllAdvance =>
      isLedger && _positive(advanceAmount) && advanceAmount == remaining;

  /// Sıradaki vadenin tutarı kalandan küçük: satır tutarı da yazar.
  bool get nextDueIsPartial =>
      _positive(nextDueAmount) && nextDueAmount != remaining;

  /// Ödenmiş taksit sayısı (sunucunun iki sayısından okunur, hesap değil).
  int get paidInstallmentCount =>
      (installmentCount ?? 0) - openInstallmentCount;
}

/// `Kişiler` sekmesinin satırı: bütün kişiler, ada göre.
class DebtBookPerson {
  const DebtBookPerson({
    required this.counterpartyId,
    required this.name,
    required this.owedToYou,
    required this.owedByYou,
    this.isActive = true,
    this.scope,
    this.openDebtCount = 0,
  });

  factory DebtBookPerson.fromJson(Map<String, dynamic> json) => DebtBookPerson(
    counterpartyId: JsonReaders.string(json, 'counterpartyId'),
    name: JsonReaders.string(json, 'name'),
    owedToYou: JsonReaders.string(json, 'owedToYou'),
    owedByYou: JsonReaders.string(json, 'owedByYou'),
    isActive: (json['isActive'] as bool?) ?? true,
    scope: _scopeOf(json),
    openDebtCount: JsonReaders.nullableInt(json, 'openDebtCount') ?? 0,
  );

  final String counterpartyId;
  final String name;
  final String owedToYou;
  final String owedByYou;
  final bool isActive;
  final TransactionScope? scope;
  final int openDebtCount;

  bool get isSettled => !_positive(owedToYou) && !_positive(owedByYou);
}

/// `GET /api/v1/debt-book`: iki toplam, satırlar, kişiler. Toplamlar
/// sunucudandır; istemci satırları toplamaz.
class DebtBook {
  const DebtBook({
    required this.receivableTotal,
    required this.payableTotal,
    required this.items,
    required this.people,
    this.receivableOverdue = '0.0000',
    this.payableOverdue = '0.0000',
    this.closedReceivableCount = 0,
    this.closedPayableCount = 0,
  });

  factory DebtBook.fromJson(Map<String, dynamic> json) => DebtBook(
    receivableTotal: JsonReaders.string(json, 'receivableTotal'),
    payableTotal: JsonReaders.string(json, 'payableTotal'),
    receivableOverdue:
        JsonReaders.nullableString(json, 'receivableOverdue') ?? '0.0000',
    payableOverdue:
        JsonReaders.nullableString(json, 'payableOverdue') ?? '0.0000',
    items: [
      for (final item in JsonReaders.list(json, 'items'))
        DebtBookItem.fromJson(JsonReaders.object(item, 'item')),
    ],
    people: [
      for (final person in JsonReaders.list(json, 'people'))
        DebtBookPerson.fromJson(JsonReaders.object(person, 'person')),
    ],
    closedReceivableCount:
        JsonReaders.nullableInt(json, 'closedReceivableCount') ?? 0,
    closedPayableCount:
        JsonReaders.nullableInt(json, 'closedPayableCount') ?? 0,
  );

  final String receivableTotal;
  final String payableTotal;
  final String receivableOverdue;
  final String payableOverdue;
  final List<DebtBookItem> items;
  final List<DebtBookPerson> people;
  final int closedReceivableCount;
  final int closedPayableCount;

  /// Hiç kişi ve kayıt yok: ilk kullanım.
  bool get isEmpty => items.isEmpty && people.isEmpty;

  String totalOf(DebtSide side) =>
      side == DebtSide.receivable ? receivableTotal : payableTotal;

  String overdueOf(DebtSide side) =>
      side == DebtSide.receivable ? receivableOverdue : payableOverdue;

  int closedCountOf(DebtSide side) =>
      side == DebtSide.receivable ? closedReceivableCount : closedPayableCount;

  List<DebtBookItem> itemsOf(DebtSide side) => [
    for (final item in items)
      if (item.side == side) item,
  ];
}

/// Tahsilat ya da ödeme panelinin kısayolları (yalnız kişinin hesabı).
class DebtShortcuts {
  const DebtShortcuts({required this.full, required this.overdue});

  factory DebtShortcuts.fromJson(Map<String, dynamic> json) => DebtShortcuts(
    full: JsonReaders.string(json, 'full'),
    overdue: JsonReaders.string(json, 'overdue'),
  );

  final String full;
  final String overdue;

  bool get hasFull => _positive(full);
  bool get hasOverdue => _positive(overdue);
}

/// `GET /api/v1/counterparties/{id}/page`: kişinin özeti. Tutarlar hesabın ve
/// kişili taksitli kayıtların toplamıdır (K14); kısayollar yalnız hesabındır.
class CounterpartyPageData {
  const CounterpartyPageData({
    required this.id,
    required this.name,
    required this.owedToYou,
    required this.owedByYou,
    required this.collect,
    required this.pay,
    this.isActive = true,
    this.scope,
    this.note,
    this.overdueReceivable = '0.0000',
    this.overduePayable = '0.0000',
    this.nextReceivableDueDate,
    this.nextReceivableDueAmount = '0.0000',
    this.nextPayableDueDate,
    this.nextPayableDueAmount = '0.0000',
    this.records = const [],
    this.openingReceivable = '0.0000',
    this.openingPayable = '0.0000',
  });

  factory CounterpartyPageData.fromJson(Map<String, dynamic> json) =>
      CounterpartyPageData(
        id: JsonReaders.string(json, 'id'),
        name: JsonReaders.string(json, 'name'),
        note: JsonReaders.nullableString(json, 'note'),
        isActive: (json['isActive'] as bool?) ?? true,
        scope: _scopeOf(json),
        openingReceivable:
            JsonReaders.nullableString(json, 'openingReceivable') ?? '0.0000',
        openingPayable:
            JsonReaders.nullableString(json, 'openingPayable') ?? '0.0000',
        owedToYou: JsonReaders.string(json, 'owedToYou'),
        owedByYou: JsonReaders.string(json, 'owedByYou'),
        overdueReceivable: JsonReaders.string(json, 'overdueReceivable'),
        overduePayable: JsonReaders.string(json, 'overduePayable'),
        nextReceivableDueDate: JsonReaders.nullableString(
          json,
          'nextReceivableDueDate',
        ),
        nextReceivableDueAmount: JsonReaders.string(
          json,
          'nextReceivableDueAmount',
        ),
        nextPayableDueDate: JsonReaders.nullableString(
          json,
          'nextPayableDueDate',
        ),
        nextPayableDueAmount: JsonReaders.string(json, 'nextPayableDueAmount'),
        collect: DebtShortcuts.fromJson(
          JsonReaders.object(json['collect'], 'collect'),
        ),
        pay: DebtShortcuts.fromJson(JsonReaders.object(json['pay'], 'pay')),
        records: [
          for (final item in JsonReaders.list(json, 'records'))
            DebtBookItem.fromJson(JsonReaders.object(item, 'record')),
        ],
      );

  final String id;
  final String name;
  final String? note;
  final bool isActive;
  final TransactionScope? scope;
  final String owedToYou;
  final String owedByYou;
  final String overdueReceivable;
  final String overduePayable;
  final String? nextReceivableDueDate;
  final String nextReceivableDueAmount;
  final String? nextPayableDueDate;
  final String nextPayableDueAmount;
  final DebtShortcuts collect;
  final DebtShortcuts pay;
  final List<DebtBookItem> records;

  /// Devreden (uygulamadan önceki) bakiye; bakiyenin içindedir.
  final String openingReceivable;
  final String openingPayable;

  bool get owesYou => _positive(owedToYou);
  bool get youOwe => _positive(owedByYou);
}

/// Sunucunun dört ondalıklı tutarı sıfırdan büyük mü (karşılaştırma; hesap
/// değil).
bool isPositiveAmount(String amount) => _positive(amount);

bool _positive(String amount) {
  final value = double.tryParse(amount);
  return value != null && value > 0;
}

/// Taksitli ya da kişisiz kaydın bir taksiti (`DebtResponse.installments`).
class DebtRecordInstallment {
  const DebtRecordInstallment({
    required this.sequence,
    required this.amount,
    required this.status,
    required this.openAmount,
    required this.paidAmount,
    this.dueDate,
  });

  factory DebtRecordInstallment.fromJson(Map<String, dynamic> json) =>
      DebtRecordInstallment(
        sequence: JsonReaders.integer(json, 'sequence'),
        amount: JsonReaders.string(json, 'amount'),
        dueDate: JsonReaders.nullableString(json, 'dueDate'),
        status: JsonReaders.string(json, 'status'),
        openAmount: JsonReaders.string(json, 'openAmount'),
        paidAmount: JsonReaders.string(json, 'paidAmount'),
      );

  final int sequence;
  final String amount;
  final String? dueDate;

  /// `paid` · `open` · `overdue`.
  final String status;
  final String openAmount;
  final String paidAmount;

  bool get isPaid => status == 'paid';
  bool get isOverdue => status == 'overdue';

  /// Kısmen ödenmiş açık taksit.
  bool get isPartlyPaid => !isPaid && _positive(paidAmount);
}

/// Kaydın bir kapanışı: ödeme ya da indirim (`DebtResponse.closings`).
class DebtRecordClosing {
  const DebtRecordClosing({
    required this.id,
    required this.isDiscount,
    required this.amount,
    required this.date,
    this.sourceName,
    this.isCancelled = false,
    this.accountId,
    this.creditCardId,
    this.isPos = false,
  });

  factory DebtRecordClosing.fromJson(Map<String, dynamic> json) =>
      DebtRecordClosing(
        id: JsonReaders.string(json, 'id'),
        isDiscount: JsonReaders.string(json, 'kind') == 'discount',
        amount: JsonReaders.string(json, 'amount'),
        date: JsonReaders.string(json, 'date'),
        isCancelled: (json['isCancelled'] as bool?) ?? false,
        accountId: JsonReaders.nullableString(json, 'accountId'),
        creditCardId: JsonReaders.nullableString(json, 'creditCardId'),
        isPos: JsonReaders.nullableString(json, 'posSettlementId') != null,
      );

  final String? accountId;
  final String? creditCardId;

  /// Kartla (POS) tahsil edildi.
  final bool isPos;

  /// Paranın yerinin adı verilmiş kopya.
  DebtRecordClosing named(String? name) => DebtRecordClosing(
    id: id,
    isDiscount: isDiscount,
    amount: amount,
    date: date,
    sourceName: name,
    isCancelled: isCancelled,
    accountId: accountId,
    creditCardId: creditCardId,
    isPos: isPos,
  );

  final String id;
  final bool isDiscount;
  final String amount;
  final String date;

  /// Paranın yeri (hesap, kart, POS). Cevap yalnız kimliği taşır; adı
  /// hesap listesinden ekran kurulurken verilir.
  final String? sourceName;
  final bool isCancelled;
}

/// Taksitli ya da kişisiz kaydın sayfası (`GET /api/v1/debts/{id}`).
class DebtRecordData {
  const DebtRecordData({
    required this.id,
    required this.title,
    required this.side,
    required this.remaining,
    required this.planKind,
    required this.installments,
    required this.closings,
    this.counterpartyId,
    this.counterpartyName,
    this.scope,
    this.isInterestBearing = false,
    this.remainingPrincipal,
    this.isCarriedOver = false,
    this.sourceType,
  });

  factory DebtRecordData.fromJson(Map<String, dynamic> json) {
    final name = JsonReaders.nullableString(json, 'counterpartyName');
    final description = JsonReaders.nullableString(json, 'description');
    final interest = double.tryParse(JsonReaders.string(json, 'totalInterest'));
    return DebtRecordData(
      id: JsonReaders.string(json, 'id'),
      title: description?.trim().isNotEmpty == true
          ? description!
          : (name ?? ''),
      side: DebtSide.fromApi(JsonReaders.string(json, 'direction')),
      remaining: JsonReaders.string(json, 'remainingAmount'),
      planKind: JsonReaders.string(json, 'planKind'),
      counterpartyId: JsonReaders.nullableString(json, 'counterpartyId'),
      counterpartyName: name == null || name.isEmpty ? null : name,
      scope: _scopeOf(json),
      isInterestBearing: interest != null && interest > 0,
      remainingPrincipal: JsonReaders.nullableString(
        json,
        'remainingPrincipal',
      ),
      isCarriedOver: (json['isCarriedOver'] as bool?) ?? false,
      sourceType: JsonReaders.nullableString(json, 'sourceType'),
      installments: [
        for (final item in JsonReaders.list(json, 'installments'))
          DebtRecordInstallment.fromJson(
            JsonReaders.object(item, 'installment'),
          ),
      ],
      closings: [
        for (final item in JsonReaders.list(json, 'closings'))
          DebtRecordClosing.fromJson(JsonReaders.object(item, 'closing')),
      ],
    );
  }

  final String id;
  final String title;
  final DebtSide side;
  final String remaining;

  /// `installments` · `single` · `none`.
  final String planKind;
  final String? counterpartyId;
  final String? counterpartyName;
  final TransactionScope? scope;
  final bool isInterestBearing;

  /// Kalan anapara (`remainingPrincipal`, 23 Kasım 2026): açık taksitlerin
  /// planlı anapara payı, sunucudan. İstemci taksitlerden toplamaz; satır
  /// yalnız faizli kayıtta çizilir.
  final String? remainingPrincipal;
  final bool isCarriedOver;

  /// `cash` (borç para) · `expense` / `income` (kayıtlı alım ya da satış).
  final String? sourceType;
  final List<DebtRecordInstallment> installments;
  final List<DebtRecordClosing> closings;

  bool get isInstallment => planKind == 'installments';

  List<DebtRecordInstallment> get openInstallments => [
    for (final item in installments)
      if (!item.isPaid) item,
  ];

  int get paidCount => installments.where((item) => item.isPaid).length;

  bool get isClosed => !_positive(remaining);

  /// İndirim yalnız kayıtlı alım ya da satışta, devredilmemiş ve faizsiz
  /// borçta (`debt.discount_unavailable`).
  bool get canDiscount =>
      !isClosed &&
      !isCarriedOver &&
      !isInterestBearing &&
      (sourceType == 'expense' || sourceType == 'income');

  /// Faizli borcun planı değişmez (kullanıcı kararı, 16 Ekim 2026).
  bool get canChangePlan => !isClosed && !isInterestBearing;

  /// Faizli borçta yalnız tam taksit ödenir: tutar sıradaki taksittir.
  String? get fixedPaymentAmount =>
      isInterestBearing ? openInstallments.firstOrNull?.openAmount : null;
}

/// Kişinin hareketlerinde tür süzgeci (K15).
enum PersonMovementKind {
  all('Hepsi', null),
  trade('Satış ve alım', 'counterparty-charge'),
  settlement('Tahsilat ve ödeme', 'counterparty-settlement');

  const PersonMovementKind(this.label, this.apiValue);
  final String label;
  final String? apiValue;
}

/// Kişinin hareketlerinde dönem. Varsayılan bütün zamanlar; ay seçimi
/// süzgecin seçeneğidir (`05-EKRAN-TASARIMI.md` §3.2, K15 sapması).
enum PersonMovementPeriod {
  all('Tümü'),
  thisMonth('Bu ay'),
  lastMonth('Geçen ay'),
  lastThreeMonths('Son 3 ay');

  const PersonMovementPeriod(this.label);
  final String label;

  /// Gün aralığı (`yyyy-MM-dd`, iki uç dahil); bütün zamanlarda boş.
  ({String from, String to})? range(DateTime today) {
    String iso(DateTime day) =>
        '${day.year.toString().padLeft(4, '0')}-'
        '${day.month.toString().padLeft(2, '0')}-'
        '${day.day.toString().padLeft(2, '0')}';
    final first = DateTime(today.year, today.month);
    return switch (this) {
      all => null,
      thisMonth => (from: iso(first), to: iso(today)),
      lastMonth => (
        from: iso(DateTime(today.year, today.month - 1)),
        to: iso(first.subtract(const Duration(days: 1))),
      ),
      lastThreeMonths => (
        from: iso(DateTime(today.year, today.month - 2)),
        to: iso(today),
      ),
    };
  }
}

/// Kişinin hareket süzgeci; yön yalnız iki yönlü kişide sorulur.
class PersonMovementFilter {
  const PersonMovementFilter({
    this.period = PersonMovementPeriod.all,
    this.kind = PersonMovementKind.all,
    this.side,
  });

  final PersonMovementPeriod period;
  final PersonMovementKind kind;
  final DebtSide? side;

  bool get isEmpty =>
      period == PersonMovementPeriod.all &&
      kind == PersonMovementKind.all &&
      side == null;

  /// Süzgecin seçili parçaları; listenin üstünde kaldırılabilir çiptir.
  List<String> get labels => [
    if (period != PersonMovementPeriod.all) period.label,
    if (kind != PersonMovementKind.all) kind.label,
    if (side == DebtSide.receivable) 'Alacak tarafı',
    if (side == DebtSide.payable) 'Borç tarafı',
  ];
}

/// Tahsilat ve ödeme panelinin bir yeri (hesap, kart ya da POS).
class DebtSettleSource {
  const DebtSettleSource({
    required this.id,
    required this.name,
    this.type,
    this.note,
  });

  final String id;
  final String name;

  /// Hesapta `cash` ya da `bank`.
  final String? type;

  /// POS'ta paranın geçeceği hesap.
  final String? note;
}

class DebtSettleSources {
  const DebtSettleSources({
    required this.accounts,
    required this.cards,
    required this.posDefinitions,
  });

  final List<DebtSettleSource> accounts;
  final List<DebtSettleSource> cards;
  final List<DebtSettleSource> posDefinitions;
}

/// Formdaki kişi: var olan (kimlikli) ya da kayıtta oluşturulacak yeni kişi.
class DebtPersonChoice {
  const DebtPersonChoice({required this.name, this.id, this.scope});

  final String? id;
  final String name;

  /// Yeni kişinin türü (kapsam görünen kullanıcıda sorulur).
  final TransactionScope? scope;

  bool get isNew => id == null;
}

/// Sunucunun takvim önizlemesi (`POST /api/v1/debts/schedule-preview`).
class DebtSchedulePreview {
  const DebtSchedulePreview({
    required this.totalRepayment,
    required this.items,
  });

  factory DebtSchedulePreview.fromJson(Map<String, dynamic> json) =>
      DebtSchedulePreview(
        totalRepayment: JsonReaders.string(json, 'totalRepayment'),
        items: [
          for (final item in JsonReaders.list(json, 'installments'))
            (
              sequence: JsonReaders.integer(
                JsonReaders.object(item, 'installment'),
                'sequence',
              ),
              dueDate: JsonReaders.string(
                JsonReaders.object(item, 'installment'),
                'dueDate',
              ),
              amount: JsonReaders.string(
                JsonReaders.object(item, 'installment'),
                'amount',
              ),
            ),
        ],
      );

  final String totalRepayment;
  final List<({int sequence, String dueDate, String amount})> items;
}

/// Yeni kayıt formunun seçenekleri: kategoriler ve paranın yerleri.
class DebtEntryOptions {
  const DebtEntryOptions({
    required this.incomeCategories,
    required this.expenseCategories,
    required this.sources,
  });

  final List<({String id, String name, TransactionScope? scope})>
  incomeCategories;
  final List<({String id, String name, TransactionScope? scope})>
  expenseCategories;
  final DebtSettleSources sources;
}
