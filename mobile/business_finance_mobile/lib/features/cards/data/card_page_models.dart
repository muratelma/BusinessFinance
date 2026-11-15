import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../finance_pages/data/finance_page_models.dart';

/// Kart hareketlerinin türü; sunucunun `kind` değeri.
enum CardMovementKind {
  all(null, 'Hepsi'),
  charge('charge', 'Harcama'),
  installment('installment', 'Taksitli'),
  payment('payment', 'Ödeme');

  const CardMovementKind(this.apiValue, this.label);
  final String? apiValue;
  final String label;
}

/// Hareketlerin dönemi: bir ekstre, tarih aralığı ya da tüm zamanlar.
/// Boş ekstre (`year == null`) açık ekstredir; sunucu bugüne göre seçer.
class CardPeriodChoice {
  const CardPeriodChoice.current()
    : year = null,
      month = null,
      from = null,
      to = null,
      all = false;
  const CardPeriodChoice.statement(int this.year, int this.month)
    : from = null,
      to = null,
      all = false;
  const CardPeriodChoice.range(String this.from, String this.to)
    : year = null,
      month = null,
      all = false;
  const CardPeriodChoice.everything()
    : year = null,
      month = null,
      from = null,
      to = null,
      all = true;

  final int? year;
  final int? month;
  final String? from;
  final String? to;
  final bool all;

  bool get isCurrent => !all && year == null && from == null;
  bool get isRange => from != null;

  Map<String, String> get query => {
    if (all) 'all': 'true',
    if (year != null) 'year': '$year',
    if (month != null) 'month': '$month',
    'from': ?from,
    'to': ?to,
  };

  @override
  bool operator ==(Object other) =>
      other is CardPeriodChoice &&
      other.year == year &&
      other.month == month &&
      other.from == from &&
      other.to == to &&
      other.all == all;

  @override
  int get hashCode => Object.hash(year, month, from, to, all);
}

/// Hareketler sayfasının bütün seçimi; filtre paneli bunu kurar.
class CardMovementQuery {
  const CardMovementQuery({
    this.kind = CardMovementKind.all,
    this.scope,
    this.categoryId,
    this.period = const CardPeriodChoice.current(),
  });

  final CardMovementKind kind;
  final TransactionScope? scope;
  final String? categoryId;
  final CardPeriodChoice period;

  /// Ödemenin kapsamı ve kategorisi yoktur; tür ödemeyken ikisi düşer.
  CardMovementQuery normalized() => kind == CardMovementKind.payment
      ? CardMovementQuery(kind: kind, period: period)
      : this;

  Map<String, String> toQuery({int offset = 0, int? limit}) => {
    ...period.query,
    'kind': ?kind.apiValue,
    if (scope != null) 'scope': scope!.apiValue,
    'category': ?categoryId,
    if (offset > 0) 'offset': '$offset',
    if (limit != null) 'limit': '$limit',
  };

  CardMovementQuery copyWith({
    CardMovementKind? kind,
    TransactionScope? Function()? scope,
    String? Function()? categoryId,
    CardPeriodChoice? period,
  }) => CardMovementQuery(
    kind: kind ?? this.kind,
    scope: scope == null ? this.scope : scope(),
    categoryId: categoryId == null ? this.categoryId : categoryId(),
    period: period ?? this.period,
  );

  @override
  bool operator ==(Object other) =>
      other is CardMovementQuery &&
      other.kind == kind &&
      other.scope == scope &&
      other.categoryId == categoryId &&
      other.period == period;

  @override
  int get hashCode => Object.hash(kind, scope, categoryId, period);
}

/// Bir ekstrenin dönemi: kesim ayı, ilk gün ve kesim günü.
class CardStatementRange {
  const CardStatementRange({
    required this.year,
    required this.month,
    required this.start,
    required this.end,
  });
  final int year;
  final int month;
  final String start;
  final String end;

  factory CardStatementRange.fromJson(Map<String, dynamic> json) =>
      CardStatementRange(
        year: JsonReaders.integer(json, 'year'),
        month: JsonReaders.integer(json, 'month'),
        start: JsonReaders.date(json, 'start'),
        end: JsonReaders.date(json, 'end'),
      );
}

/// Sayfanın seçili dönemi. [kind]: `statement`, `range`, `all`. Ekstrede
/// [status]: `current` (kesilmedi), `open` (ödeme bekleniyor), `paid`,
/// `overdue`.
class CardPagePeriod {
  const CardPagePeriod({
    required this.kind,
    this.year,
    this.month,
    this.start,
    this.end,
    this.status,
  });
  final String kind;
  final int? year;
  final int? month;
  final String? start;
  final String? end;
  final String? status;

  factory CardPagePeriod.fromJson(Map<String, dynamic> json) => CardPagePeriod(
    kind: JsonReaders.string(json, 'kind'),
    year: JsonReaders.nullableInt(json, 'year'),
    month: JsonReaders.nullableInt(json, 'month'),
    start: JsonReaders.nullableString(json, 'start'),
    end: JsonReaders.nullableString(json, 'end'),
    status: JsonReaders.nullableString(json, 'status'),
  );
}

/// Kartın hareketler sayfası: özet, kategori toplamları ve ortak işlem
/// satırları. Bütün tutarlar sunucudandır.
class CardPageData {
  const CardPageData({
    required this.cardId,
    required this.name,
    required this.period,
    required this.statements,
    required this.spending,
    required this.payments,
    required this.categories,
    required this.items,
    required this.totalCount,
    required this.offset,
    required this.hasMore,
  });
  final String cardId;
  final String name;
  final CardPagePeriod period;
  final List<CardStatementRange> statements;

  /// Seçimdeki harcamaların toplamı (artı).
  final String spending;

  /// Seçimdeki ödemelerin toplamı; seçim ödemeyi dışarıda bırakıyorsa boş.
  final String? payments;

  /// İlki `all` (bütün kategoriler), sonra tutara göre kategoriler.
  final List<FinancePageOption> categories;
  final List<FinancePageActivity> items;
  final int totalCount;
  final int offset;
  final bool hasMore;

  factory CardPageData.fromJson(Map<String, dynamic> json) => CardPageData(
    cardId: JsonReaders.string(json, 'cardId'),
    name: JsonReaders.string(json, 'name'),
    period: CardPagePeriod.fromJson(
      JsonReaders.object(json['period'], 'period'),
    ),
    statements: JsonReaders.list(json, 'statements')
        .map((v) => CardStatementRange.fromJson(JsonReaders.object(v, 'range')))
        .toList(growable: false),
    spending: JsonReaders.money(json, 'spending'),
    payments: json['payments'] == null
        ? null
        : JsonReaders.money(json, 'payments'),
    categories: JsonReaders.list(json, 'categories')
        .map(
          (v) => FinancePageOption.fromJson(JsonReaders.object(v, 'category')),
        )
        .toList(growable: false),
    items: JsonReaders.list(json, 'items')
        .map((v) => FinancePageActivity.fromJson(JsonReaders.object(v, 'item')))
        .toList(growable: false),
    totalCount: JsonReaders.integer(json, 'totalCount'),
    offset: JsonReaders.integer(json, 'offset'),
    hasMore: JsonReaders.boolean(json, 'hasMore'),
  );
}

/// Taksit sayısının önizlemesi: aylık tutar ve ilk/son gün sunucudan gelir.
class InstallmentPreviewOption {
  const InstallmentPreviewOption({
    required this.count,
    required this.installmentAmount,
    required this.lastInstallmentAmount,
    required this.firstDate,
    required this.lastDate,
  });
  final int count;
  final String installmentAmount;
  final String lastInstallmentAmount;
  final String firstDate;
  final String lastDate;

  factory InstallmentPreviewOption.fromJson(Map<String, dynamic> json) =>
      InstallmentPreviewOption(
        count: JsonReaders.integer(json, 'installmentCount'),
        installmentAmount: JsonReaders.money(json, 'installmentAmount'),
        lastInstallmentAmount: JsonReaders.money(json, 'lastInstallmentAmount'),
        firstDate: JsonReaders.date(json, 'firstInstallmentDate'),
        lastDate: JsonReaders.date(json, 'lastInstallmentDate'),
      );
}
