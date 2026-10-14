import 'dart:async';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/routing/app_router.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_form_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import '../../support/finance_page_fixture.dart';

void main() {
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

class _AccountWrites implements AccountRepository {
  String? updatedId;
  String? updatedName;
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
