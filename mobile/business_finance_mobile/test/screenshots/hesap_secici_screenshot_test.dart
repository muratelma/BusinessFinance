import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_directory.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_sections.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import '../support/account_field_helpers.dart';
import 'screenshot_harness.dart';

/// Hesap seçici ve Özet'teki hesap bakiyeleri, Claude Design teslimiyle
/// (`design/hesap-secici-handoff/screenshots/`) aynı örnek veriyle.
void main() {
  const accounts = [
    ('kasa', 'Dükkan Kasası', 'cash', TransactionScope.business, '89520.0000'),
    ('cuzdan', 'Şahsi cüzdan', 'cash', TransactionScope.personal, '650.0000'),
    ('zsahsi', 'Ziraat şahsi', 'cash', TransactionScope.personal, '1000.0000'),
    (
      'ziraat',
      'Ziraat vadesiz',
      'bank',
      TransactionScope.business,
      '30658.3500',
    ),
    (
      'garanti',
      'Garanti Vadesiz',
      'bank',
      TransactionScope.business,
      '14500.0000',
    ),
  ];

  Future<Widget> host(Widget child) async {
    final scope = ScopeController(readHasBusiness: () async => true);
    await scope.ensureLoaded();
    final directory = AccountDirectory(
      loadAccounts: () async => [
        for (final a in accounts)
          AccountOverviewItem(
            id: a.$1,
            name: a.$2,
            type: a.$3,
            isActive: true,
            balance: a.$5,
            defaultScope: a.$4,
          ),
      ],
      loadCards: () async => [
        const CreditCardItem(
          id: 'kart',
          name: 'İşletme kartı',
          limit: '50000.0000',
          currentDebt: '3930.0000',
          availableLimit: '46070.0000',
          currency: 'TRY',
          statementClosingDay: 5,
          paymentDueDay: 15,
          minimumPaymentRate: '20.0000',
          isActive: true,
          defaultScope: TransactionScope.business,
        ),
      ],
    );
    directory.ensureLoaded();
    return MultiProvider(
      providers: [
        ChangeNotifierProvider<ScopeController?>.value(value: scope),
        ChangeNotifierProvider<AccountDirectory?>.value(value: directory),
      ],
      child: child,
    );
  }

  Widget form(
    String title, {
    required bool expense,
    bool transfer = false,
    bool empty = false,
    String? initial,
  }) => _FormFrame(
    title: title,
    expense: expense,
    transfer: transfer,
    empty: empty,
    initial: initial,
    accountIds: [for (final a in accounts) (a.$1, a.$2)],
  );

  for (final (name, page, before) in [
    (
      'h1-gelir-menu-acik',
      form('Gelir ekle', expense: false, initial: 'kasa'),
      (WidgetTester t) async => t.tap(accountField('Hesap')),
    ),
    (
      'h1b-hesap-secildi',
      form('Gelir ekle', expense: false, initial: 'ziraat'),
      null,
    ),
    (
      'h1g-gider-kart-dahil',
      form('Gider ekle', expense: true, initial: 'card:kart'),
      (WidgetTester t) async => t.tap(accountField('Hesap')),
    ),
    (
      'h6-transfer-nereye',
      form('Transfer', expense: false, transfer: true, initial: 'ziraat'),
      (WidgetTester t) async => t.tap(accountField('Nereye')),
    ),
    (
      'h7-hesap-yok-kapali',
      form('Gelir ekle', expense: false, empty: true),
      null,
    ),
    (
      'h7o-hesap-yok-acik',
      form('Gelir ekle', expense: false, empty: true),
      (WidgetTester t) async => t.tap(accountField('Hesap')),
    ),
  ]) {
    testWidgets('hesap seçici $name', (tester) async {
      await captureScreen(
        tester,
        'hs-$name',
        await host(page),
        selectedTab: 1,
        before: before,
      );
    }, skip: !screenshotsEnabled);
  }

  testWidgets('özet hesap bakiyeleri', (tester) async {
    final report = DashboardReport.fromJson({
      'year': 2026,
      'month': 10,
      'totalIncome': '0.0000',
      'totalExpense': '0.0000',
      'net': '0.0000',
      'currency': 'TRY',
      'categoryExpenseSlices': const [],
      'categoryExpenses': const [],
      'accountBalances': [
        for (final a in accounts)
          {
            'accountId':
                '00000000-0000-0000-0000-00000000000${accounts.indexOf(a)}',
            'accountName': a.$2,
            'balance': a.$5,
            'type': a.$3,
            'isActive': true,
            'defaultScope': a.$4.apiValue,
          },
        {
          'accountId': '00000000-0000-0000-0000-000000000009',
          'accountName': 'Garanti',
          'balance': '20000.0000',
          'type': 'bank',
          'isActive': false,
        },
      ],
    });
    await captureScreen(
      tester,
      'hs-v1-ozet-hesap-bakiyeleri',
      await host(
        Scaffold(
          body: SafeArea(
            child: ListView(
              padding: const EdgeInsets.all(AppSpacing.medium),
              children: [
                DashboardBlock(
                  title: 'Hesap bakiyeleri',
                  onOpen: () {},
                  child: DashboardAccountBalances(
                    report: report,
                    onOpen: () {},
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }, skip: !screenshotsEnabled);
}

class _FormFrame extends StatefulWidget {
  const _FormFrame({
    required this.title,
    required this.expense,
    required this.transfer,
    required this.empty,
    required this.accountIds,
    this.initial,
  });
  final String title;
  final bool expense;
  final bool transfer;
  final bool empty;
  final String? initial;
  final List<(String, String)> accountIds;
  @override
  State<_FormFrame> createState() => _FormFrameState();
}

class _FormFrameState extends State<_FormFrame> {
  late String? value = widget.initial;
  String? from = 'kasa';

  @override
  Widget build(BuildContext context) {
    final choices = AccountChoices.of(context);
    List<AppAccountOption> options({String? disabled}) => widget.empty
        ? const []
        : [
            for (final (id, name) in widget.accountIds)
              choices.account(
                id,
                name,
                disabledNote: id == disabled ? 'Para buradan çıkıyor' : null,
              ),
            if (widget.expense)
              choices.card('kart', 'İşletme kartı').withId('card:kart'),
          ];
    return Scaffold(
      appBar: AppBar(title: Text(widget.title)),
      body: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          if (widget.transfer) ...[
            AppAccountField(
              label: 'Nereden',
              value: from,
              options: options(),
              onChanged: (v) => setState(() => from = v),
            ),
            const SizedBox(height: AppSpacing.medium),
          ],
          AppAccountField(
            label: widget.transfer ? 'Nereye' : 'Hesap',
            value: value,
            options: options(disabled: widget.transfer ? from : null),
            onChanged: (v) => setState(() => value = v),
            onAdd: () {},
          ),
          const SizedBox(height: AppSpacing.medium),
          DropdownButtonFormField<String>(
            decoration: const InputDecoration(labelText: 'Kategori'),
            items: const [],
            onChanged: (_) {},
          ),
          const SizedBox(height: AppSpacing.medium),
          const TextField(decoration: InputDecoration(labelText: 'Tutar')),
          const SizedBox(height: AppSpacing.medium),
          AppDateField(label: 'Tarih', value: '2026-10-27', onChanged: (_) {}),
          const SizedBox(height: AppSpacing.medium),
          const TextField(
            decoration: InputDecoration(
              labelText: 'Ad (isteğe bağlı)',
              helperText: 'Boş bırakılırsa kategori adı kullanılır.',
            ),
          ),
          const SizedBox(height: AppSpacing.medium),
          FilledButton(onPressed: () {}, child: const Text('Kaydet')),
        ],
      ),
    );
  }
}
