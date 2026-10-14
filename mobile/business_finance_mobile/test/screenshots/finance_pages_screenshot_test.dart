import 'dart:convert';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_feed_page.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_tile.dart';
import 'package:business_finance_mobile/features/cards/presentation/transfer_prefill.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/accounts_overview_page.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/finance_page_controller.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/finance_report_page.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/transfer_page.dart';
import 'package:business_finance_mobile/features/more/presentation/more_page.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';
import '../support/finance_page_fixture.dart';
import 'screenshot_harness.dart';

void main() {
  const cases = [
    'K0',
    'A1',
    'A2',
    'A3',
    'A4',
    'A5',
    'A6',
    'A7',
    'A8',
    'A8b',
    'A9',
    'A10',
    'A11',
    'A12',
    'B1',
    'B2',
    'B3',
    'B4',
    'B4b',
    'B5',
    'B5b',
    'B6',
    'B7',
    'B8',
    'B8b',
    'B9',
    'B9b',
    'B10',
    'H1',
    'H2',
    'H3',
    'H4',
    'H5',
    'H6',
    'H7',
    'H8',
    'H9',
    'H10',
    'H10b',
    'H11',
    'H12',
    'H13',
    'H15',
    'D1',
    'D2',
    'D2b',
    'D3',
    'D3b',
    'D4',
    'D5',
    'D6',
  ];
  for (final code in cases) {
    testWidgets('teslim $code gerçek Flutter bileşenleri', (tester) async {
      final repo = _ScreenRepository(code);
      final activityRepo = _ActivityRepository(repo);
      final scope = ScopeController(readHasBusiness: () async => code != 'A12');
      await scope.ensureLoaded();
      await scope.select(TransactionScope.business);
      addTearDown(scope.dispose);
      final month = ['A10', 'B10', 'H9', 'H12'].contains(code)
          ? DateTime(2026, 9)
          : DateTime(2026, 10);
      final isIncome = ['A5', 'A6', 'A8', 'A8b', 'B8', 'B8b'].contains(code);
      Widget page;
      if (code == 'K0') {
        page = ChangeNotifierProvider<ScopeController?>.value(
          value: scope,
          child: const MorePage(),
        );
      } else if (code == 'D3' || code == 'D3b') {
        page = TransferPage(
          repository: repo,
          prefill: TransferPrefill(
            amount: '10000.0000',
            date: '2026-10-13',
            feeAmount: code == 'D3b' ? '50.0000' : null,
          ),
        );
      } else if (code == 'D4') {
        page = ActivityFeedPage(repository: activityRepo);
      } else if (code == 'D5' || code == 'D6') {
        page = AccountsOverviewPage(repository: repo, onCreate: () async {});
      } else {
        final kind = code.startsWith('A') && code != 'A8b'
            ? FinancePageKind.summary
            : code.startsWith('B') || code == 'A8b'
            ? FinancePageKind.category
            : code.startsWith('H10')
            ? FinancePageKind.transit
            : FinancePageKind.account;
        page = FinanceReportPage(
          repository: repo,
          kind: kind,
          id: code == 'A8b' ? null : 'bank',
          month: month,
          income: isIncome,
          source: ['A3', 'B4', 'B4b'].contains(code)
              ? _sourceKey('Ziraat Vadesiz')
              : code == 'A4'
              ? 'deferred'
              : ['A9', 'A10'].contains(code)
              ? _sourceKey('Bonus Kart')
              : null,
          scope: code == 'B9' ? TransactionScope.business : null,
          scopeController: scope,
          activityRepository: activityRepo,
          posRepository: _PosRepository(many: code == 'D2b'),
          onAccountSettings: () async {},
        );
      }
      await captureScreen(
        tester,
        'kh-$code',
        page,
        withNavBar: code == 'K0' || code == 'D4',
        selectedTab: code == 'K0' ? 3 : 1,
        before: (t) async {
          Future<void> tap(String label) async {
            await t.tap(find.text(label).first);
            await t.pumpAndSettle();
          }

          if (['A2', 'A6', 'A7', 'B2', 'B8b'].contains(code)) {
            await tap('Hesap / kart');
          }
          if (code == 'B3' || code == 'B4b') await tap('Kişi');
          if (['A9', 'H15'].contains(code)) await tap('Ekim 2026');
          if (['H2', 'H5'].contains(code)) await tap('Giren');
          if (['H3', 'H6', 'H7'].contains(code)) await tap('Çıkan');
          if (['H4', 'H5', 'H6', 'H7', 'H8'].contains(code)) await tap('Tür');
          if (code == 'H7') await tap('Borç taksidi ödemesi');
          if (code == 'H8') {
            await t.tap(find.text('Kategori').last);
            await t.pumpAndSettle();
          }
          if (code == 'D1' || code == 'D2' || code == 'D2b') {
            await t.scrollUntilVisible(
              find.byType(ActivityTile).first,
              200,
              scrollable: find.byType(Scrollable).last,
            );
            await t.tap(find.byType(ActivityTile).first);
            await t.pumpAndSettle();
          }
          if (code == 'D3b') {
            await t.tap(find.byType(DropdownButtonFormField<String>).first);
            await t.pumpAndSettle();
            await t.tap(find.text('Ziraat Vadesiz').last);
            await t.pumpAndSettle();
            await t.tap(find.byType(DropdownButtonFormField<String>).last);
            await t.pumpAndSettle();
            await t.tap(find.text('Dükkan Kasası').last);
            await t.pump(const Duration(milliseconds: 350));
          }
        },
      );
      expect(tester.takeException(), isNull);
    }, skip: !screenshotsEnabled);
  }
}

class _ScreenRepository extends PageFixture {
  _ScreenRepository(this.code) {
    final sample = _sample;
    summaryData = CategorySummaryData.fromJson(
      sample[['A5', 'A6', 'A8', 'A8b'].contains(code) ? 'income' : 'summary'],
    );
    accountName = code == 'H11'
        ? 'Dükkan Kasası'
        : code == 'H12'
        ? 'Garanti Vadesiz'
        : 'Ziraat Vadesiz';
    empty = ['A10', 'A11', 'B10', 'H12'].contains(code);
    categoryName = switch (code) {
      'B5' || 'B5b' => 'İşyeri faturaları',
      'B6' => 'Banka ve POS komisyonu',
      'B7' => 'Faiz ve finansman gideri',
      'B8' || 'B8b' => 'Satış geliri',
      'B9' || 'B9b' => 'Sigorta',
      'B10' => 'Kasa farkı',
      _ => 'Ticari mal alımı',
    };
    peopleVisible = !['B5', 'B5b', 'B6', 'B9', 'B9b', 'B10'].contains(code);
    if (code == 'A7') {
      summaryData = CategorySummaryData(
        year: 2026,
        month: 10,
        direction: 'expense',
        total: '1404650.4000',
        previousTotal: '1280400.0000',
        categoryCount: 3,
        categories: const [
          CategoryPageSlice(
            id: 'large1',
            name: 'Ticari mal alımı',
            amount: '1000000.0000',
            share: '71.0000',
            barRatio: 0.7119,
          ),
          CategoryPageSlice(
            id: 'large2',
            name: 'Personel giderleri',
            amount: '300000.0000',
            share: '21.0000',
            barRatio: 0.2136,
          ),
          CategoryPageSlice(
            id: 'large3',
            name: 'İşyeri faturaları',
            amount: '104650.4000',
            share: '8.0000',
            barRatio: 0.0745,
          ),
        ],
        sources: const [
          FinancePageOption(key: 'all', group: 'all', amount: '1404650.4000'),
          FinancePageOption(
            key: 'account:cash',
            group: 'accounts',
            name: 'Dükkan Kasası',
            amount: '98400.0000',
          ),
          FinancePageOption(
            key: 'account:branch',
            group: 'accounts',
            name: 'Şube 2 kasası',
            amount: '41250.0000',
          ),
          FinancePageOption(
            key: 'account:bank',
            group: 'accounts',
            name: 'Ziraat Vadesiz',
            amount: '512560.0000',
          ),
          FinancePageOption(
            key: 'account:long',
            group: 'accounts',
            name: 'Türkiye İş Bankası Ticari Vadesiz',
            amount: '384200.4000',
          ),
          FinancePageOption(
            key: 'account:pos',
            group: 'accounts',
            name: 'Vakıfbank POS hesabı',
            amount: '96000.0000',
          ),
          FinancePageOption(
            key: 'card:bonus',
            group: 'cards',
            name: 'Bonus Kart',
            amount: '93930.0000',
          ),
          FinancePageOption(
            key: 'card:axess',
            group: 'cards',
            name: 'Axess Business',
            amount: '58310.0000',
          ),
          FinancePageOption(
            key: 'deferred',
            group: 'deferred',
            amount: '120000.0000',
          ),
        ],
      );
    }
    rows = [
      if (code == 'B6')
        FinancePageActivity(
          activity: pageActivity(),
          amount: '-50.0000',
          part: 'fee',
          partAmount: '50.0000',
          parentAmount: '10000.0000',
          scope: TransactionScope.business,
        ),
      if (code == 'B7')
        FinancePageActivity(
          activity: pageActivity(
            kind: 'debt-payment',
            amount: '3000.0000',
            interest: '450.0000',
            fee: null,
          ),
          amount: '-450.0000',
          part: 'interest',
          partAmount: '450.0000',
          parentAmount: '3000.0000',
        ),
      if (code == 'B5' || code == 'B5b')
        FinancePageActivity(
          activity: pageActivity(
            kind: 'obligation',
            amount: '1110.0000',
            fee: null,
            category: 'Elektrik faturası',
            source: 'Test enerji',
            destination: null,
          ),
          amount: '-1110.0000',
          state: code == 'B5' ? 'settled' : 'open',
          settledOn: '2026-10-20',
          settledSource: 'Ziraat vadesiz',
        ),
      if (code == 'B8' || code == 'B8b' || code == 'H13')
        FinancePageActivity(
          activity: pageActivity(
            kind: 'pos-sale',
            amount: '8400.0000',
            effect: 'income',
            fee: '326.0000',
            net: '8074.0000',
            expected: '2026-10-28',
          ),
          amount: '8400.0000',
          state: 'transit',
        ),
      if (code == 'B9' || code == 'B9b')
        FinancePageActivity(
          activity: pageActivity(
            kind: 'account-transaction',
            amount: '1200.0000',
            effect: 'expense',
            fee: null,
            category: 'İşyeri sigortası',
            scope: 'business',
            destination: null,
          ),
          amount: '-1200.0000',
          showScope: code == 'B9b',
          scope: TransactionScope.business,
        ),
      if (code == 'B9b')
        FinancePageActivity(
          activity: pageActivity(
            id: 'personal',
            kind: 'card-charge',
            amount: '800.0000',
            effect: 'expense',
            fee: null,
            source: 'Bonus kart',
            destination: null,
            category: 'Kasko',
            scope: 'personal',
          ),
          amount: '-800.0000',
          showScope: true,
          scope: TransactionScope.personal,
        ),
    ];
    if (rows.isEmpty) {
      rows = [
        FinancePageActivity(
          activity: pageActivity(),
          amount: code.startsWith('B') ? '-4200.0000' : '-10050.0000',
          part: code.startsWith('B') ? null : 'fee',
          partAmount: code.startsWith('B') ? null : '50.0000',
        ),
      ];
    }
    if (code == 'D2' || code == 'D2b') {
      rows = [
        FinancePageActivity(
          activity: pageActivity(
            id: 'deposit',
            kind: 'pos-deposit',
            amount: '7169.0000',
            fee: '40.0000',
            destination: 'Ziraat vadesiz',
            source: 'Ziraat POS',
          ),
          amount: '7169.0000',
          part: 'deduction',
          partAmount: '40.0000',
          firstSaleDate: '2026-10-14',
          lastSaleDate: '2026-10-14',
        ),
      ];
    }
  }
  @override
  Future<CategorySummaryData> summary(
    DateTime month, {
    bool income = false,
    TransactionScope? scope,
    String? source,
  }) async {
    final snapshot = source == null
        ? null
        : _sample['summarySources']?[income ? 'income' : 'expense']?[source];
    final raw = Map<String, dynamic>.from(
      snapshot ?? (income ? _sample['income'] : _sample['summary']),
    );
    if (code == 'A7') return summaryData;
    if (code == 'A12') {
      raw['total'] = '18350.0000';
      raw['previousTotal'] = '0.0000';
      raw['categoryCount'] = 5;
      raw['categories'] = [
        {
          'id': 'home',
          'name': 'Kira',
          'amount': '7500.0000',
          'share': '41.0000',
          'barRatio': '0.4087',
        },
        {
          'id': 'market',
          'name': 'Market',
          'amount': '6200.0000',
          'share': '34.0000',
          'barRatio': '0.3379',
        },
        {
          'id': 'bills',
          'name': 'Faturalar',
          'amount': '2050.0000',
          'share': '11.0000',
          'barRatio': '0.1117',
        },
        {
          'id': 'transport',
          'name': 'Ulaşım',
          'amount': '1800.0000',
          'share': '10.0000',
          'barRatio': '0.0981',
        },
        {
          'id': 'health',
          'name': 'Sağlık',
          'amount': '800.0000',
          'share': '4.0000',
          'barRatio': '0.0436',
        },
      ];
      raw['sources'] = [
        {'key': 'all', 'group': 'all', 'amount': '18350.0000'},
        {
          'key': 'account:personal',
          'group': 'accounts',
          'name': 'Cüzdan',
          'amount': '18350.0000',
        },
      ];
    }
    if (code == 'A8') {
      raw['total'] = '44900.0000';
      raw['categoryCount'] = 4;
      raw['categories'] = [
        ...raw['categories'],
        {
          'id': null,
          'name': null,
          'amount': '350.0000',
          'share': '1.0000',
          'barRatio': '0.0078',
        },
      ];
      raw['sources'] = [
        for (final row in raw['sources'])
          {
            ...row as Map<String, dynamic>,
            'amount': row['key'] == 'all'
                ? '44900.0000'
                : row['name'] == 'Ziraat Vadesiz'
                ? '25100.0000'
                : row['name'] == 'Dükkan Kasası'
                ? '15300.0000'
                : row['amount'],
          },
      ];
    }
    if (empty) {
      raw['total'] = '0.0000';
      raw['categories'] = [];
      raw['categoryCount'] = 0;
      raw['sources'] = [
        for (final row in raw['sources'])
          if (row['key'] == 'all' || row['key'] == source)
            {...row as Map<String, dynamic>, 'amount': '0.0000'},
      ];
    }
    raw['year'] = month.year;
    raw['month'] = month.month;
    return CategorySummaryData.fromJson(raw);
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
    if (code == 'B9' || code == 'B9b' || code == 'A8b') {
      final isMissing = code == 'A8b';
      final total = isMissing
          ? '350.0000'
          : code == 'B9'
          ? '1200.0000'
          : '2000.0000';
      final items = isMissing
          ? [
              FinancePageActivity(
                activity: pageActivity(
                  id: 'legacy1',
                  kind: 'account-transaction',
                  amount: '200.0000',
                  effect: 'income',
                  fee: null,
                  destination: null,
                  category: 'Havale · Mehmet Yılmaz',
                  date: '2026-10-22',
                ),
                amount: '200.0000',
              ),
              FinancePageActivity(
                activity: pageActivity(
                  id: 'legacy2',
                  kind: 'account-transaction',
                  amount: '150.0000',
                  effect: 'income',
                  fee: null,
                  source: 'Dükkan Kasası',
                  destination: null,
                  category: 'Nakit giriş',
                  date: '2026-10-09',
                ),
                amount: '150.0000',
              ),
            ]
          : rows;
      return CategoryPageData(
        summary: CategorySummaryData(
          year: month.year,
          month: month.month,
          direction: income ? 'income' : 'expense',
          total: total,
          previousTotal: '0.0000',
          categoryCount: 1,
          categories: [],
          sources: [
            FinancePageOption(key: 'all', group: 'all', amount: total),
            FinancePageOption(
              key: _sourceKey('Ziraat Vadesiz'),
              group: 'accounts',
              name: 'Ziraat Vadesiz',
              amount: isMissing ? '200.0000' : '1200.0000',
            ),
            if (code != 'B9')
              FinancePageOption(
                key: isMissing
                    ? _sourceKey('Dükkan Kasası')
                    : _sourceKey('Bonus Kart'),
                group: isMissing ? 'accounts' : 'cards',
                name: isMissing ? 'Dükkan Kasası' : 'Bonus Kart',
                amount: isMissing ? '150.0000' : '800.0000',
              ),
          ],
        ),
        wholeTotal: isMissing ? total : '2000.0000',
        showScope: !isMissing,
        hasPeople: false,
        categoryId: id,
        name: isMissing ? null : 'Sigorta',
        people: [],
        items: items,
        totalCount: items.length,
        offset: 0,
        hasMore: false,
      );
    }
    final raw = _sample['categoryPages'][categoryName] ?? _sample['category'];
    final input = Map<String, dynamic>.from(
      ['B4', 'B4b'].contains(code) ? _sample['category'] : raw,
    );
    if (empty) {
      input['items'] = [];
      input['totalCount'] = 0;
      input['summary'] = {
        ...input['summary'] as Map<String, dynamic>,
        'total': '0.0000',
      };
    }
    if (code == 'B5b') {
      input['items'] = [
        for (final row in input['items'])
          {
            ...row as Map<String, dynamic>,
            'state': row['activity']['activityKind'] == 'obligation'
                ? 'open'
                : row['state'],
            'settledOn': null,
            'settledSource': null,
          },
      ];
    }
    return CategoryPageData.fromJson(input);
  }

  @override
  Future<AccountDetailData> account(
    String id,
    DateTime month, {
    String? direction,
    String? selection,
    int offset = 0,
  }) async {
    if (['D1', 'D2', 'D2b'].contains(code)) {
      return super.account(
        id,
        month,
        direction: direction,
        selection: selection,
        offset: offset,
      );
    }
    final raw = Map<String, dynamic>.from(
      code == 'H9'
          ? _sample['accountPrevious']
          : code == 'H11'
          ? _sample['cashAccount']
          : direction == null
          ? _sample['account']
          : _sample['accountDirections'][direction],
    );
    raw['isCurrentMonth'] = month.month == 10;
    raw['name'] = accountName;
    if (empty) {
      raw['items'] = [];
      raw['totalCount'] = 0;
    }
    if (code == 'H13') {
      raw['name'] = 'Türkiye İş Bankası Ticari Vadesiz';
      raw['balance'] = '12845320.7500';
      raw['currentBalance'] = '12845320.7500';
      raw['inflow'] = '2450000.0000';
      raw['outflow'] = '1284650.4000';
      raw['net'] = '1165349.6000';
      raw['transit'] = {'total': '0.0000', 'count': 0, 'nearest': null};
      raw['items'] = [];
      raw['totalCount'] = 3;
      return AccountDetailData.fromJson(raw).withItems([
        FinancePageActivity(
          activity: pageActivity(
            kind: 'obligation-settlement',
            amount: '1284650.4000',
            fee: null,
            category: 'Örnek Elektrik Dağıtım A.Ş.',
            source: 'Türkiye İş Bankası Ticari Vadesiz',
            destination: null,
            date: '2026-10-20',
          ),
          amount: '-1284650.4000',
        ),
        FinancePageActivity(
          activity: pageActivity(
            id: 'receivable',
            kind: 'debt-collection',
            amount: '2000.0000',
            fee: null,
            category: 'Kemal Bey',
            source: 'Türkiye İş Bankası Ticari Vadesiz',
            destination: null,
            date: '2026-10-12',
          ),
          amount: '2000.0000',
        ),
        FinancePageActivity(
          activity: pageActivity(
            id: 'incoming',
            amount: '2448000.0000',
            fee: null,
            category: 'Transfer',
            source: 'Garanti Vadesiz',
            destination: 'Türkiye İş Bankası Ticari Vadesiz',
            date: '2026-10-05',
          ),
          amount: '2448000.0000',
        ),
      ]);
    }
    if (code == 'H12') {
      raw['balance'] = '4500.0000';
      raw['currentBalance'] = '14500.0000';
      raw['inflow'] = '0.0000';
      raw['outflow'] = '0.0000';
      raw['net'] = '0.0000';
      raw['transit'] = {'total': '0.0000', 'count': 0, 'nearest': null};
    }
    if (selection != null && _sample['accountSelections']?[selection] != null) {
      return AccountDetailData.fromJson({
        ..._sample['accountSelections'][selection] as Map<String, dynamic>,
        'isCurrentMonth': month.month == 10,
      });
    }
    if (selection != null) {
      raw['selectionTotal'] = selection.contains('payment')
          ? '-4000.0000'
          : '-10900.0000';
      raw['items'] = [
        for (final row in raw['items'])
          if (selection.contains('payment')
              ? (row['activity']['activityKind'] == 'debt-payment')
              : (row['activity']['effect'] == 'expense'))
            row,
      ];
      raw['totalCount'] = (raw['items'] as List).length;
    }
    return AccountDetailData.fromJson(raw);
  }

  @override
  Future<TransitPageData> transit(String id, {int offset = 0}) async {
    if (code != 'H10b') return TransitPageData.fromJson(_sample['transit']);
    const values = ['8074.0000', '7267.0000', '5767.0000', '7192.0000'];
    return TransitPageData(
      accountId: id,
      name: 'Ziraat Vadesiz',
      total: '28300.0000',
      totalCount: 4,
      offset: offset,
      hasMore: false,
      items: [
        for (var i = 0; i < 4; i++)
          FinancePageActivity(
            activity: pageActivity(
              id: 'transit$i',
              kind: 'pos-sale',
              amount: values[i],
              effect: 'income',
              fee: null,
              expected: '2026-10-${28 - i}',
              date: '2026-10-${25 - i}',
            ),
            amount: values[i],
            state: 'transit',
          ),
      ],
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
    final amount = source == null
        ? summaryData.total
        : (_sample['summarySources'][income
                  ? 'income'
                  : 'expense'][source]?['total'] ??
              '0.0000');
    return FinanceYearMonths(
      year: year,
      months: [
        for (var i = 1; i <= 12; i++)
          FinanceMonthValue(
            month: i,
            amount: i == 10 ? amount : '0.0000',
            barRatio: i == 10 ? 1 : 0,
            enabled: i <= 10,
          ),
      ],
    );
  }

  @override
  Future<FinanceYearMonths> accountMonths(String id, int year) async {
    return FinanceYearMonths.fromJson(_sample['accountMonths']);
  }

  final String code;
  @override
  Future<AccountsOverviewData> accounts() async => code == 'D6'
      ? const AccountsOverviewData(
          total: '0.0000',
          cashTotal: '0.0000',
          bankTotal: '0.0000',
          inactiveTotal: '0.0000',
          items: [],
        )
      : AccountsOverviewData.fromJson(_sample['accounts']);
}

class _ActivityRepository implements ActivityRepositoryContract {
  _ActivityRepository(this.repo);
  final PageFixture repo;
  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) async => ActivityPage(
    items: [
      pageActivity(),
      pageActivity(
        id: 'interest',
        kind: 'debt-payment',
        amount: '1000.0000',
        interest: '150.0000',
        fee: null,
      ),
    ],
    pagination: const ActivityPagination(
      pageNumber: 1,
      pageSize: 20,
      totalCount: 2,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false,
    ),
  );
  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async => const PlannedActivityPage(
    asOfDate: '2026-10-13',
    daysAhead: 30,
    totalCount: 0,
    items: [],
  );
  @override
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async => [
    const ActivityBalance(
      holder: ActivityBalanceHolder.account,
      name: 'Ziraat vadesiz',
      balance: '27076.0000',
      currency: 'TRY',
      change: ActivityBalanceChange.decreased,
    ),
    const ActivityBalance(
      holder: ActivityBalanceHolder.account,
      name: 'Garanti vadesiz',
      balance: '14500.0000',
      currency: 'TRY',
      change: ActivityBalanceChange.increased,
    ),
  ];
  @override
  Future<void> cancel(FinancialActivity activity) async {}
  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _PosRepository implements PosRepositoryContract {
  _PosRepository({required this.many});
  final bool many;
  @override
  Future<PosDeposit> getDeposit({required String depositId}) async =>
      PosDeposit(
        id: depositId,
        accountName: 'Ziraat vadesiz',
        depositDate: '2026-10-15',
        expectedAmount: '7209.0000',
        depositedAmount: '7169.0000',
        deductionAmount: '40.0000',
        deductionCategoryName: 'Banka ve POS komisyonu',
        currency: 'TRY',
        isCancelled: false,
        balanceAfter: '34076.0000',
        grossAmount: '7500.0000',
        commissionAmount: '291.0000',
        settlements: [
          for (var i = 0; i < (many ? 3 : 1); i++)
            PosSettlementItem(
              id: 's$i',
              accountName: 'Ziraat vadesiz',
              categoryName: 'Satış geliri',
              grossAmount: many ? '2500.0000' : '7500.0000',
              commissionAmount: many ? '97.0000' : '291.0000',
              netAmount: many ? '2403.0000' : '7209.0000',
              currency: 'TRY',
              settlementDate: '2026-10-${14 - i}',
              expectedTransferDate: '2026-10-15',
              isInTransit: false,
              isLate: false,
              posDefinitionName: 'Ziraat POS',
            ),
        ],
      );
  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

Map<String, dynamic> get _sample =>
    jsonDecode(
          File(
            Platform.environment['FINANCE_PAGE_CONTRACT'] ??
                'test/fixtures/category-account-contract.json',
          ).readAsStringSync().replaceAll('2026-09-', '2026-10-'),
        )
        as Map<String, dynamic>;
String _sourceKey(String name) =>
    (_sample['summary']['sources'] as List).firstWhere(
          (row) => row['name'] == name,
        )['key']
        as String;
