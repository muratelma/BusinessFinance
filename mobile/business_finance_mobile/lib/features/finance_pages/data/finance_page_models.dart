export '../../../core/models/month_amounts.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/models/transaction_scope.dart';
import '../../activities/data/activity_models.dart';

List<T> _list<T>(
  Map<String, dynamic> json,
  String key,
  T Function(Map<String, dynamic>) read,
) => JsonReaders.list(
  json,
  key,
).map((v) => read(JsonReaders.object(v, key))).toList(growable: false);
String? _moneyOrNull(Map<String, dynamic> json, String key) =>
    json[key] == null ? null : JsonReaders.money(json, key);

class FinancePageOption {
  const FinancePageOption({
    required this.key,
    required this.group,
    required this.amount,
    this.name,
    this.kind,
    this.id,
  });
  final String key;
  final String group;
  final String amount;
  final String? name;
  final String? kind;
  final String? id;
  factory FinancePageOption.fromJson(Map<String, dynamic> json) =>
      FinancePageOption(
        key: JsonReaders.string(json, 'key'),
        group: JsonReaders.string(json, 'group'),
        amount: JsonReaders.money(json, 'amount'),
        name: JsonReaders.nullableString(json, 'name'),
        kind: JsonReaders.nullableString(json, 'kind'),
        id: JsonReaders.nullableString(json, 'id'),
      );
}

class CategoryPageSlice {
  const CategoryPageSlice({
    required this.amount,
    required this.share,
    required this.barRatio,
    this.id,
    this.name,
  });
  final String? id;
  final String? name;
  final String amount;
  final String share;
  final double barRatio;
  factory CategoryPageSlice.fromJson(Map<String, dynamic> json) =>
      CategoryPageSlice(
        id: JsonReaders.nullableString(json, 'id'),
        name: JsonReaders.nullableString(json, 'name'),
        amount: JsonReaders.money(json, 'amount'),
        share: JsonReaders.money(json, 'share'),
        barRatio: double.parse(JsonReaders.money(json, 'barRatio')),
      );
}

/// Özetin altı aylık çubuğundaki bir ay; kaydı olmayan ay çizilmez.
class CategorySummaryMonth {
  const CategorySummaryMonth({
    required this.year,
    required this.month,
    required this.amount,
    required this.barRatio,
    required this.hasData,
  });
  final int year;
  final int month;
  final String amount;
  final double barRatio;
  final bool hasData;
  factory CategorySummaryMonth.fromJson(Map<String, dynamic> json) =>
      CategorySummaryMonth(
        year: JsonReaders.integer(json, 'year'),
        month: JsonReaders.integer(json, 'month'),
        amount: JsonReaders.money(json, 'amount'),
        barRatio: double.parse(JsonReaders.money(json, 'barRatio')),
        hasData: JsonReaders.boolean(json, 'hasData'),
      );
}

class CategorySummaryData {
  const CategorySummaryData({
    required this.year,
    required this.month,
    required this.direction,
    required this.total,
    required this.previousTotal,
    required this.categoryCount,
    required this.categories,
    required this.sources,
    this.scope,
    this.previousChange,
    this.recentMonths = const [],
  });
  final int year;
  final int month;
  final String direction;
  final TransactionScope? scope;
  final String total;
  final String previousTotal;
  final int categoryCount;
  final List<CategoryPageSlice> categories;
  final List<FinancePageOption> sources;

  /// Geçen aya göre işaretli tam yüzde (`"60.0000"` = %60 fazla); geçen ay
  /// sıfırsa boştur. Yalnız kategori özetinde gelir.
  final String? previousChange;
  final List<CategorySummaryMonth> recentMonths;
  bool get isIncome => direction == 'income';
  factory CategorySummaryData.fromJson(Map<String, dynamic> json) =>
      CategorySummaryData(
        year: JsonReaders.integer(json, 'year'),
        month: JsonReaders.integer(json, 'month'),
        direction: JsonReaders.string(json, 'direction'),
        scope: TransactionScope.fromApiOrNull(json['scope']),
        total: JsonReaders.money(json, 'total'),
        previousTotal: JsonReaders.money(json, 'previousTotal'),
        categoryCount: JsonReaders.integer(json, 'categoryCount'),
        categories: _list(json, 'categories', CategoryPageSlice.fromJson),
        sources: _list(json, 'sources', FinancePageOption.fromJson),
        previousChange: json['previousChange'] == null
            ? null
            : JsonReaders.money(json, 'previousChange'),
        recentMonths: json['recentMonths'] == null
            ? const []
            : _list(json, 'recentMonths', CategorySummaryMonth.fromJson),
      );
}

class FinancePageActivity {
  const FinancePageActivity({
    required this.activity,
    required this.amount,
    this.part,
    this.partAmount,
    this.parentAmount,
    this.state,
    this.settledOn,
    this.settledSource,
    this.showScope = false,
    this.firstSaleDate,
    this.lastSaleDate,
    this.scope,
  });
  final FinancialActivity activity;
  final String amount;
  final String? part;
  final String? partAmount;
  final String? parentAmount;
  final String? state;
  final String? settledOn;
  final String? settledSource;
  final bool showScope;
  final String? firstSaleDate;
  final String? lastSaleDate;
  final TransactionScope? scope;
  String get listKey => '${activity.listKey}:${part ?? 'whole'}';
  factory FinancePageActivity.fromJson(Map<String, dynamic> json) =>
      FinancePageActivity(
        activity: FinancialActivity.fromJson(
          JsonReaders.object(json['activity'], 'activity'),
        ),
        amount: JsonReaders.money(json, 'amount'),
        part: JsonReaders.nullableString(json, 'part'),
        partAmount: _moneyOrNull(json, 'partAmount'),
        parentAmount: _moneyOrNull(json, 'parentAmount'),
        state: JsonReaders.nullableString(json, 'state'),
        settledOn: JsonReaders.nullableString(json, 'settledOn'),
        settledSource: JsonReaders.nullableString(json, 'settledSource'),
        showScope: JsonReaders.boolean(json, 'showScope'),
        firstSaleDate: JsonReaders.nullableString(json, 'firstSaleDate'),
        lastSaleDate: JsonReaders.nullableString(json, 'lastSaleDate'),
        scope: TransactionScope.fromApiOrNull(json['scope']),
      );
}

class CategoryPageData {
  const CategoryPageData({
    required this.summary,
    required this.wholeTotal,
    required this.showScope,
    required this.hasPeople,
    required this.people,
    required this.items,
    required this.totalCount,
    required this.offset,
    required this.hasMore,
    this.categoryId,
    this.name,
  });
  final CategorySummaryData summary;
  final String? categoryId;
  final String? name;
  final String wholeTotal;
  final bool showScope;
  final bool hasPeople;
  final List<FinancePageOption> people;
  final List<FinancePageActivity> items;
  final int totalCount;
  final int offset;
  final bool hasMore;
  CategoryPageData withItems(List<FinancePageActivity> value) =>
      CategoryPageData(
        summary: summary,
        wholeTotal: wholeTotal,
        showScope: showScope,
        hasPeople: hasPeople,
        people: people,
        items: value,
        totalCount: totalCount,
        offset: offset,
        hasMore: hasMore,
        categoryId: categoryId,
        name: name,
      );
  factory CategoryPageData.fromJson(Map<String, dynamic> json) =>
      CategoryPageData(
        summary: CategorySummaryData.fromJson(
          JsonReaders.object(json['summary'], 'summary'),
        ),
        categoryId: JsonReaders.nullableString(json, 'categoryId'),
        name: JsonReaders.nullableString(json, 'name'),
        wholeTotal: JsonReaders.money(json, 'wholeTotal'),
        showScope: JsonReaders.boolean(json, 'showScope'),
        hasPeople: JsonReaders.boolean(json, 'hasPeople'),
        people: _list(json, 'people', FinancePageOption.fromJson),
        items: _list(json, 'items', FinancePageActivity.fromJson),
        totalCount: JsonReaders.integer(json, 'totalCount'),
        offset: JsonReaders.integer(json, 'offset'),
        hasMore: JsonReaders.boolean(json, 'hasMore'),
      );
}

class AccountTransitSummary {
  const AccountTransitSummary({
    required this.total,
    required this.count,
    this.nearest,
  });
  final String total;
  final int count;
  final FinancePageActivity? nearest;
  factory AccountTransitSummary.fromJson(Map<String, dynamic> json) =>
      AccountTransitSummary(
        total: JsonReaders.money(json, 'total'),
        count: JsonReaders.integer(json, 'count'),
        nearest: json['nearest'] == null
            ? null
            : FinancePageActivity.fromJson(
                JsonReaders.object(json['nearest'], 'nearest'),
              ),
      );
}

class AccountDetailData {
  const AccountDetailData({
    required this.accountId,
    required this.name,
    required this.type,
    required this.isActive,
    required this.year,
    required this.month,
    required this.balance,
    required this.currentBalance,
    required this.isCurrentMonth,
    required this.inflow,
    required this.outflow,
    required this.net,
    required this.selectionTotal,
    required this.options,
    required this.transit,
    required this.items,
    required this.totalCount,
    required this.offset,
    required this.hasMore,
    this.defaultScope,
  });
  final String accountId;
  final String name;
  final String type;
  final TransactionScope? defaultScope;
  final bool isActive;
  final int year;
  final int month;
  final String balance;
  final String currentBalance;
  final bool isCurrentMonth;
  final String inflow;
  final String outflow;
  final String net;
  final String selectionTotal;
  final List<FinancePageOption> options;
  final AccountTransitSummary transit;
  final List<FinancePageActivity> items;
  final int totalCount;
  final int offset;
  final bool hasMore;
  AccountDetailData withItems(List<FinancePageActivity> value) =>
      AccountDetailData(
        accountId: accountId,
        name: name,
        type: type,
        isActive: isActive,
        year: year,
        month: month,
        balance: balance,
        currentBalance: currentBalance,
        isCurrentMonth: isCurrentMonth,
        inflow: inflow,
        outflow: outflow,
        net: net,
        selectionTotal: selectionTotal,
        options: options,
        transit: transit,
        items: value,
        totalCount: totalCount,
        offset: offset,
        hasMore: hasMore,
        defaultScope: defaultScope,
      );
  factory AccountDetailData.fromJson(Map<String, dynamic> json) =>
      AccountDetailData(
        accountId: JsonReaders.string(json, 'accountId'),
        name: JsonReaders.string(json, 'name'),
        type: JsonReaders.string(json, 'type'),
        defaultScope: TransactionScope.fromApiOrNull(json['defaultScope']),
        isActive: JsonReaders.boolean(json, 'isActive'),
        year: JsonReaders.integer(json, 'year'),
        month: JsonReaders.integer(json, 'month'),
        balance: JsonReaders.money(json, 'balance'),
        currentBalance: JsonReaders.money(json, 'currentBalance'),
        isCurrentMonth: JsonReaders.boolean(json, 'isCurrentMonth'),
        inflow: JsonReaders.money(json, 'inflow'),
        outflow: JsonReaders.money(json, 'outflow'),
        net: JsonReaders.money(json, 'net'),
        selectionTotal: JsonReaders.money(json, 'selectionTotal'),
        options: _list(json, 'options', FinancePageOption.fromJson),
        transit: AccountTransitSummary.fromJson(
          JsonReaders.object(json['transit'], 'transit'),
        ),
        items: _list(json, 'items', FinancePageActivity.fromJson),
        totalCount: JsonReaders.integer(json, 'totalCount'),
        offset: JsonReaders.integer(json, 'offset'),
        hasMore: JsonReaders.boolean(json, 'hasMore'),
      );
}

class TransitPageData {
  const TransitPageData({
    required this.accountId,
    required this.name,
    required this.total,
    required this.totalCount,
    required this.items,
    required this.offset,
    required this.hasMore,
  });
  final String accountId;
  final String name;
  final String total;
  final int totalCount;
  final List<FinancePageActivity> items;
  final int offset;
  final bool hasMore;
  TransitPageData withItems(List<FinancePageActivity> value) => TransitPageData(
    accountId: accountId,
    name: name,
    total: total,
    totalCount: totalCount,
    items: value,
    offset: offset,
    hasMore: hasMore,
  );
  factory TransitPageData.fromJson(Map<String, dynamic> json) =>
      TransitPageData(
        accountId: JsonReaders.string(json, 'accountId'),
        name: JsonReaders.string(json, 'name'),
        total: JsonReaders.money(json, 'total'),
        totalCount: JsonReaders.integer(json, 'totalCount'),
        items: _list(json, 'items', FinancePageActivity.fromJson),
        offset: JsonReaders.integer(json, 'offset'),
        hasMore: JsonReaders.boolean(json, 'hasMore'),
      );
}

class AccountOverviewItem {
  const AccountOverviewItem({
    required this.id,
    required this.name,
    required this.type,
    required this.isActive,
    required this.balance,
    this.defaultScope,
  });
  final String id;
  final String name;
  final String type;
  final bool isActive;
  final String balance;
  final TransactionScope? defaultScope;
  factory AccountOverviewItem.fromJson(Map<String, dynamic> json) =>
      AccountOverviewItem(
        id: JsonReaders.string(json, 'id'),
        name: JsonReaders.string(json, 'name'),
        type: JsonReaders.string(json, 'type'),
        isActive: JsonReaders.boolean(json, 'isActive'),
        balance: JsonReaders.money(json, 'balance'),
        defaultScope: TransactionScope.fromApiOrNull(json['defaultScope']),
      );
}

class AccountsOverviewData {
  const AccountsOverviewData({
    required this.total,
    required this.cashTotal,
    required this.bankTotal,
    required this.inactiveTotal,
    required this.items,
    this.cashShare,
    this.bankShare,
    this.inactiveShare,
  });
  final String total;
  final String cashTotal;
  final String bankTotal;
  final String inactiveTotal;

  /// Toplam içindeki pay, tam yüzde (`58.0000`). Sunucu hesaplar; eksi
  /// bakiyeli hesap varken boş gelir.
  final String? cashShare;
  final String? bankShare;
  final String? inactiveShare;
  final List<AccountOverviewItem> items;
  factory AccountsOverviewData.fromJson(Map<String, dynamic> json) =>
      AccountsOverviewData(
        total: JsonReaders.money(json, 'total'),
        cashTotal: JsonReaders.money(json, 'cashTotal'),
        bankTotal: JsonReaders.money(json, 'bankTotal'),
        inactiveTotal: JsonReaders.money(json, 'inactiveTotal'),
        cashShare: _moneyOrNull(json, 'cashShare'),
        bankShare: _moneyOrNull(json, 'bankShare'),
        inactiveShare: _moneyOrNull(json, 'inactiveShare'),
        items: _list(json, 'items', AccountOverviewItem.fromJson),
      );
}

class ActivityPartData {
  const ActivityPartData({
    required this.kind,
    required this.amount,
    this.categoryId,
    this.categoryName,
  });
  final String kind;
  final String amount;
  final String? categoryId;
  final String? categoryName;
  factory ActivityPartData.fromJson(Map<String, dynamic> json) =>
      ActivityPartData(
        kind: JsonReaders.string(json, 'kind'),
        amount: JsonReaders.money(json, 'amount'),
        categoryId: JsonReaders.nullableString(json, 'categoryId'),
        categoryName: JsonReaders.nullableString(json, 'categoryName'),
      );
}

class TransferInput {
  const TransferInput({
    required this.sourceAccountId,
    required this.destinationAccountId,
    required this.amount,
    required this.date,
    this.feeAmount = '0.0000',
    this.feeScope,
    this.description,
  });
  final String sourceAccountId;
  final String destinationAccountId;
  final String amount;
  final String date;
  final String feeAmount;
  final TransactionScope? feeScope;
  final String? description;
  Map<String, Object?> toJson() => {
    'sourceAccountId': sourceAccountId,
    'destinationAccountId': destinationAccountId,
    'amount': amount,
    'currency': 'TRY',
    'transferDate': date,
    'feeAmount': feeAmount,
    'feeScope': feeScope?.apiValue,
    'description': description,
  };
}

class TransferPreviewData {
  const TransferPreviewData({
    required this.sourceAccountId,
    required this.sourceAccountName,
    required this.totalOutflow,
    required this.feeAmount,
    required this.needsScope,
    this.feeCategoryId,
    this.feeCategoryName,
    this.resolvedScope,
  });
  final String sourceAccountId;
  final String sourceAccountName;
  final String totalOutflow;
  final String feeAmount;
  final String? feeCategoryId;
  final String? feeCategoryName;
  final TransactionScope? resolvedScope;
  final bool needsScope;
  factory TransferPreviewData.fromJson(Map<String, dynamic> json) =>
      TransferPreviewData(
        sourceAccountId: JsonReaders.string(json, 'sourceAccountId'),
        sourceAccountName: JsonReaders.string(json, 'sourceAccountName'),
        totalOutflow: JsonReaders.money(json, 'totalOutflow'),
        feeAmount: JsonReaders.money(json, 'feeAmount'),
        needsScope: JsonReaders.boolean(json, 'needsScope'),
        feeCategoryId: JsonReaders.nullableString(json, 'feeCategoryId'),
        feeCategoryName: JsonReaders.nullableString(json, 'feeCategoryName'),
        resolvedScope: TransactionScope.fromApiOrNull(json['resolvedScope']),
      );
}
