import 'dart:async';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_repository.dart';

FinancialActivity pageActivity({
  String id = 'transfer-1',
  String kind = 'transfer',
  String amount = '10000.0000',
  String effect = 'neutral',
  String? fee = '50.0000',
  String? interest,
  String? category,
  String source = 'Ziraat vadesiz',
  String? destination = 'Kasa',
  String? scope,
  String date = '2026-10-25',
  String? net,
  String? expected,
}) => FinancialActivity.fromJson({
  'activityId': id,
  'activityKind': kind,
  'effect': effect,
  'sourceGroup': kind == 'transfer' ? 'transfer' : 'account',
  'origin': 'manual',
  'status': 'realized',
  'activityDate': date,
  'amount': amount,
  'currency': 'TRY',
  'title': category ?? 'Havale',
  'sourceName': source,
  'destinationName': destination,
  'categoryName': category,
  'canCancel': true,
  'supportsAttachments': false,
  'feeAmount': fee,
  'interestPortion': interest,
  'principalPortion': interest == null ? null : '2500.0000',
  'scope': scope,
  'netAmount': net,
  'expectedTransferDate': expected,
});

class PageFixture implements FinancePageRepositoryContract {
  CategorySummaryData summaryData = const CategorySummaryData(
    year: 2026,
    month: 10,
    direction: 'expense',
    total: '26930.0000',
    previousTotal: '24860.0000',
    categoryCount: 3,
    categories: [
      CategoryPageSlice(
        id: 'goods',
        name: 'Ticari mal',
        amount: '11430.0000',
        share: '42.4434',
        barRatio: 1,
      ),
      CategoryPageSlice(
        id: 'fee',
        name: 'Banka ve POS komisyonu',
        amount: '1167.0000',
        share: '4.3335',
        barRatio: 0.102,
      ),
      CategoryPageSlice(
        id: null,
        name: null,
        amount: '123.0000',
        share: '0.4567',
        barRatio: 0.011,
      ),
    ],
    sources: [
      FinancePageOption(
        key: 'account:bank',
        group: 'accounts',
        name: 'Ziraat vadesiz',
        amount: '12440.0000',
      ),
      FinancePageOption(
        key: 'account:cash',
        group: 'accounts',
        name: 'Kasa',
        amount: '6950.0000',
      ),
      FinancePageOption(
        key: 'card:card',
        group: 'cards',
        name: 'Bonus kredi kartı',
        amount: '3930.0000',
      ),
      FinancePageOption(
        key: 'deferred',
        group: 'deferred',
        amount: '3610.0000',
      ),
    ],
  );
  List<FinancePageActivity> rows = [
    FinancePageActivity(
      activity: pageActivity(),
      amount: '-10050.0000',
      part: 'fee',
      partAmount: '50.0000',
    ),
  ];
  Object? error;
  Future<CategorySummaryData> Function(DateTime)? summaryRead;
  final calls = <String>[];
  final inputs = <TransferInput>[];
  bool empty = false;
  String categoryName = 'Ticari mal';
  String accountName = 'Ziraat vadesiz';
  bool peopleVisible = true;
  int transitCount = 1;
  bool futureEnabled = false;
  bool needsScope = false;
  Completer<void>? saveWait;
  void fail() {
    if (error != null) throw error!;
  }

  CategorySummaryData selectedSummary(DateTime month, bool income) =>
      CategorySummaryData(
        year: month.year,
        month: month.month,
        direction: income ? 'income' : 'expense',
        total: empty ? '0.0000' : summaryData.total,
        previousTotal: summaryData.previousTotal,
        categoryCount: empty ? 0 : summaryData.categoryCount,
        categories: empty ? [] : summaryData.categories,
        sources: summaryData.sources,
      );
  @override
  Future<CategorySummaryData> summary(
    DateTime month, {
    bool income = false,
    TransactionScope? scope,
    String? source,
  }) async {
    calls.add('summary:$source:${scope?.apiValue}:$income');
    fail();
    return summaryRead == null
        ? selectedSummary(month, income)
        : summaryRead!(month);
  }

  @override
  Future<CategoryPageData> category(
    String? id,
    DateTime month, {
    bool income = false,
    TransactionScope? scope,
    String? source,
    String? person,
    int offset = 0,
  }) async {
    calls.add('category:$source:$person:$offset');
    fail();
    return CategoryPageData(
      summary: selectedSummary(month, income),
      wholeTotal: '11430.0000',
      showScope: true,
      hasPeople: peopleVisible,
      categoryId: id,
      name: id == null ? null : categoryName,
      people: const [
        FinancePageOption(
          key: 'person:ali',
          group: 'people',
          name: 'Ali Bey',
          amount: '1234.0000',
        ),
      ],
      items: empty ? [] : rows,
      totalCount: empty ? 0 : rows.length,
      offset: offset,
      hasMore: false,
    );
  }

  @override
  Future<FinanceYearMonths> categoryMonths(
    int year, {
    String? id,
    bool summary = false,
    bool income = false,
    TransactionScope? scope,
    String? source,
    String? person,
  }) async {
    calls.add('months:$source:$person:$year');
    fail();
    return FinanceYearMonths(
      year: year,
      months: [
        for (var i = 1; i <= 12; i++)
          FinanceMonthValue(
            month: i,
            amount: i == 10 ? '26930.0000' : '0.0000',
            barRatio: i == 10 ? 1 : 0,
            enabled: i <= 10 || futureEnabled,
          ),
      ],
    );
  }

  @override
  Future<AccountDetailData> account(
    String id,
    DateTime month, {
    String? direction,
    String? selection,
    int offset = 0,
  }) async {
    calls.add('account:$direction:$selection:$offset');
    fail();
    return AccountDetailData(
      accountId: id,
      name: accountName,
      type: 'bank',
      isActive: true,
      year: month.year,
      month: month.month,
      balance: '21376.0000',
      currentBalance: '21376.0000',
      isCurrentMonth: month.month == 10,
      inflow: '27936.0000',
      outflow: '31560.0000',
      net: '-3624.0000',
      selectionTotal: '10050.0000',
      options: direction == null
          ? const [
              FinancePageOption(
                key: 'in:category',
                group: 'in',
                kind: 'category',
                amount: '4200.0000',
              ),
              FinancePageOption(
                key: 'out:transfer',
                group: 'out',
                kind: 'transfer',
                amount: '-10050.0000',
              ),
            ]
          : const [
              FinancePageOption(
                key: 'out:category:goods',
                group: 'category',
                name: 'Ticari mal',
                amount: '-4310.0000',
              ),
              FinancePageOption(
                key: 'out:transfer:cash',
                group: 'transfer',
                name: 'Kasa',
                amount: '-10050.0000',
              ),
              FinancePageOption(
                key: 'out:payment:4',
                group: 'payment',
                kind: 'card-payment',
                amount: '-1100.0000',
              ),
            ],
      transit: AccountTransitSummary(
        total: '8074.0000',
        count: transitCount,
        nearest: FinancePageActivity(
          activity: pageActivity(
            id: 'pos1',
            kind: 'pos-sale',
            amount: '8300.0000',
            effect: 'income',
            fee: '226.0000',
            net: '8074.0000',
            expected: '2026-10-28',
          ),
          amount: '8074.0000',
        ),
      ),
      items: empty ? [] : rows,
      totalCount: empty ? 0 : rows.length,
      offset: offset,
      hasMore: false,
    );
  }

  @override
  Future<FinanceYearMonths> accountMonths(String id, int year) =>
      categoryMonths(year);
  @override
  Future<TransitPageData> transit(String id, {int offset = 0}) async {
    fail();
    return TransitPageData(
      accountId: id,
      name: 'Ziraat vadesiz',
      total: '8074.0000',
      totalCount: 1,
      items: [
        FinancePageActivity(
          activity: pageActivity(
            id: 'pos1',
            kind: 'pos-sale',
            amount: '8300.0000',
            effect: 'income',
            fee: '226.0000',
            net: '8074.0000',
            expected: '2026-10-28',
          ),
          amount: '8074.0000',
          state: 'transit',
        ),
      ],
      offset: offset,
      hasMore: false,
    );
  }

  @override
  Future<AccountsOverviewData> accounts() async {
    fail();
    return const AccountsOverviewData(
      total: '48330.0000',
      cashTotal: '12454.0000',
      bankTotal: '35876.0000',
      inactiveTotal: '150.0000',
      items: [
        AccountOverviewItem(
          id: 'bank',
          name: 'Ziraat vadesiz',
          type: 'bank',
          isActive: true,
          balance: '21376.0000',
          defaultScope: TransactionScope.business,
        ),
        AccountOverviewItem(
          id: 'cash',
          name: 'Kasa',
          type: 'cash',
          isActive: true,
          balance: '12454.0000',
          defaultScope: TransactionScope.business,
        ),
        AccountOverviewItem(
          id: 'old',
          name: 'Eski hesap',
          type: 'bank',
          isActive: false,
          balance: '150.0000',
        ),
      ],
    );
  }

  @override
  Future<Account> accountSettings(String id) async => Account.fromJson({
    'id': id,
    'name': 'Ziraat vadesiz',
    'type': 'bank',
    'openingBalance': '25000.0000',
    'currency': 'TRY',
    'isActive': true,
  });
  @override
  Future<List<ActivityPartData>> parts(FinancialActivity activity) async {
    fail();
    return const [
      ActivityPartData(
        kind: 'fee',
        amount: '50.0000',
        categoryId: 'fee',
        categoryName: 'Bankanın aldığı ücretler',
      ),
    ];
  }

  @override
  Future<TransferPreviewData> previewTransfer(TransferInput input) async {
    calls.add('preview:${input.amount}:${input.feeAmount}');
    fail();
    return TransferPreviewData(
      sourceAccountId: input.sourceAccountId,
      sourceAccountName: 'Ziraat vadesiz',
      totalOutflow: '10050.0000',
      feeAmount: input.feeAmount,
      needsScope: needsScope && input.feeScope == null,
      feeCategoryName: 'Bankanın aldığı ücretler',
      resolvedScope: input.feeScope ?? TransactionScope.business,
    );
  }

  @override
  Future<void> createTransfer(TransferInput input) async {
    inputs.add(input);
    if (saveWait != null) await saveWait!.future;
    fail();
  }
}
