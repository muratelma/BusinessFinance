import 'dart:async';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/routing/app_router.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_form_page.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import '../../support/finance_page_fixture.dart';

void main() {
  testWidgets(
    'formdaki Hesap ekle ekleme formunu açar, kaydedince forma yeni hesapla döner',
    (tester) async {
      final pages = _GrowingAccounts();
      final writes = _AccountWrites();
      final changes = FinancialDataChanges();
      final router = createAppRouter(
        initialLocation: transferLocation,
        financePageRepository: pages,
        accountRepository: writes,
        financialDataChanges: changes,
      );
      addTearDown(router.dispose);
      addTearDown(changes.dispose);
      await tester.pumpWidget(
        MaterialApp.router(theme: AppTheme.light(), routerConfig: router),
      );
      await tester.pumpAndSettle();

      await tester.tap(find.text('Çıkan hesap'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Hesap ekle'));
      await tester.pumpAndSettle();
      // Hesaplar sayfası araya girmez: doğrudan ekleme formu.
      expect(find.byType(AccountFormPage), findsOneWidget);
      expect(find.text('Hesaplar'), findsNothing);

      await tester.enterText(find.byType(TextFormField).first, 'Ziraat');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(writes.createdName, 'Ziraat');
      expect(find.byType(AccountFormPage), findsNothing);
      expect(find.text('Çıkan hesap'), findsOneWidget);
      // Liste yeniden okundu ve yeni hesap alanda seçili.
      expect(pages.reads, 2);
      expect(
        find.descendant(
          of: find.byType(AppAccountField).first,
          matching: find.text('Ziraat'),
        ),
        findsOneWidget,
      );
      expect(changes.accountsRevision, 1);
      expect(tester.takeException(), isNull);
    },
  );

  testWidgets('ekleme formundan vazgeçilince form olduğu gibi kalır', (
    tester,
  ) async {
    final pages = _GrowingAccounts();
    final writes = _AccountWrites();
    final router = createAppRouter(
      initialLocation: transferLocation,
      financePageRepository: pages,
      accountRepository: writes,
    );
    addTearDown(router.dispose);
    await tester.pumpWidget(
      MaterialApp.router(theme: AppTheme.light(), routerConfig: router),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Çıkan hesap'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Hesap ekle'));
    await tester.pumpAndSettle();
    await tester.pageBack();
    await tester.pumpAndSettle();

    expect(find.byType(AccountFormPage), findsNothing);
    expect(writes.createdName, isNull);
    expect(pages.reads, 1);
    expect(tester.takeException(), isNull);
  });

  testWidgets('Hesap ekle gerçek Hesaplar rotasından açılır', (tester) async {
    final pages = _SettingsFixture();
    final router = createAppRouter(
      initialLocation: accountsLocation,
      financePageRepository: pages,
      accountRepository: _AccountWrites(),
    );
    addTearDown(router.dispose);
    await tester.pumpWidget(
      MaterialApp.router(theme: AppTheme.light(), routerConfig: router),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.byTooltip('Hesap ekle'));
    await tester.pumpAndSettle();
    expect(find.byType(AccountFormPage), findsOneWidget);
    expect(
      tester.widget<AccountFormPage>(find.byType(AccountFormPage)).account,
      isNull,
    );
    expect(pages.settingsIds, isEmpty);
    expect(tester.takeException(), isNull);
  });

  testWidgets('hesap sayfası ayarları yükler, düzenler ve aynı hesaba döner', (
    tester,
  ) async {
    final pages = _SettingsFixture();
    final writes = _AccountWrites();
    final changes = FinancialDataChanges();
    final router = createAppRouter(
      initialLocation: '$accountsLocation/bank',
      financePageRepository: pages,
      accountRepository: writes,
      financialDataChanges: changes,
    );
    addTearDown(router.dispose);
    addTearDown(changes.dispose);
    await tester.pumpWidget(
      MaterialApp.router(theme: AppTheme.light(), routerConfig: router),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.byTooltip('Hesap ayarları'));
    await tester.pumpAndSettle();
    expect(pages.settingsIds, ['bank']);
    expect(find.byType(AccountFormPage), findsOneWidget);
    final name = find.byType(TextFormField).first;
    await tester.enterText(name, 'Yeni hesap adı');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();
    expect(writes.updatedId, 'bank');
    expect(writes.updatedName, 'Yeni hesap adı');
    expect(find.byType(AccountFormPage), findsNothing);
    expect(find.byTooltip('Hesap ayarları'), findsOneWidget);
    expect(tester.takeException(), isNull);
  });

  testWidgets(
    'ayarlar beklerken ilerleme, sunucu hatasında yeniden deneme gösterir',
    (tester) async {
      final pages = _SettingsFixture()..pending = Completer<Account>();
      final router = createAppRouter(
        initialLocation: '$accountsLocation/bank',
        financePageRepository: pages,
        accountRepository: _AccountWrites(),
      );
      addTearDown(router.dispose);
      await tester.pumpWidget(
        MaterialApp.router(theme: AppTheme.light(), routerConfig: router),
      );
      await tester.pumpAndSettle();
      await tester.tap(find.byTooltip('Hesap ayarları'));
      await tester.pump();
      await tester.pump(const Duration(milliseconds: 400));
      expect(find.text('Hesap ayarları alınıyor'), findsOneWidget);
      pages.pending!.completeError(
        const ApiException(
          code: 'server.unexpected_error',
          message: 'Sunucu hatası',
          statusCode: 500,
        ),
      );
      await tester.pumpAndSettle();
      expect(find.text('Sunucu hatası'), findsOneWidget);
      pages.pending = null;
      await tester.tap(find.text('Tekrar dene'));
      await tester.pumpAndSettle();
      expect(pages.settingsIds, ['bank', 'bank']);
      expect(find.byType(AccountFormPage), findsOneWidget);
      expect(tester.takeException(), isNull);
    },
  );
}

const _account = Account(
  id: 'bank',
  name: 'Ziraat vadesiz',
  type: 'bank',
  currency: 'TRY',
  isActive: true,
  openingBalance: '25000.0000',
  balance: '21376.0000',
  defaultScope: TransactionScope.business,
);

class _SettingsFixture extends PageFixture {
  final settingsIds = <String>[];
  Completer<Account>? pending;
  @override
  Future<Account> accountSettings(String id) async {
    settingsIds.add(id);
    return pending == null ? _account : pending!.future;
  }
}

/// Hesabı olmayan kullanıcı: `Hesap ekle`den sonra yeni hesap listede.
class _GrowingAccounts extends PageFixture {
  int reads = 0;
  @override
  Future<AccountsOverviewData> accounts() async {
    reads++;
    return AccountsOverviewData(
      total: '0.0000',
      cashTotal: '0.0000',
      bankTotal: '0.0000',
      inactiveTotal: '0.0000',
      items: [
        if (reads > 1)
          const AccountOverviewItem(
            id: 'new',
            name: 'Ziraat',
            type: 'cash',
            isActive: true,
            balance: '0.0000',
          ),
      ],
    );
  }
}

class _AccountWrites implements AccountRepository {
  String? updatedId;
  String? updatedName;
  String? createdName;

  @override
  Future<Account> create({
    required String name,
    required String type,
    required String openingBalance,
    TransactionScope? defaultScope,
  }) async {
    createdName = name;
    return Account(
      id: 'new',
      name: name,
      type: type,
      currency: 'TRY',
      isActive: true,
      openingBalance: openingBalance,
      balance: openingBalance,
    );
  }

  @override
  Future<Account> update({
    required String id,
    required String name,
    required bool isActive,
    TransactionScope? defaultScope,
  }) async {
    updatedId = id;
    updatedName = name;
    return _account;
  }

  @override
  Future<AccountPage> list({bool? isActive, String? type}) async =>
      const AccountPage(
        items: [_account],
        pagination: AccountPagination(
          pageNumber: 1,
          pageSize: 100,
          totalCount: 1,
          totalPages: 1,
          hasPreviousPage: false,
          hasNextPage: false,
        ),
      );
  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}
