import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_form_page.dart';
import 'package:business_finance_mobile/features/accounts/presentation/accounts_and_transfers_page.dart';
import 'package:business_finance_mobile/features/accounts/presentation/accounts_page.dart';
import 'package:business_finance_mobile/features/accounts/presentation/accounts_view_model.dart';

void main() {
  test(
    'account repository preserves money strings and contract fields',
    () async {
      final client = _client((request) async {
        expect(request.method, 'POST');
        expect(jsonDecode(request.body), {
          'name': 'Nakit',
          'type': 'cash',
          'currency': 'TRY',
          'openingBalance': '1250.1250',
          'defaultScope': 'business',
        });
        return http.Response(jsonEncode(_accountJson), 201);
      });

      final account = await ApiAccountRepository(client).create(
        name: ' Nakit ',
        type: 'cash',
        openingBalance: '1250.1250',
        defaultScope: TransactionScope.business,
      );

      expect(account.openingBalance, '1250.1250');
      expect(account.balance, '1300.1250');
    },
  );

  test('account list parses pagination exactly', () async {
    final client = _client((request) async {
      expect(request.url.queryParameters['pageSize'], '100');
      return http.Response(
        jsonEncode({
          'items': [_accountJson],
          'pagination': {
            'pageNumber': 1,
            'pageSize': 100,
            'totalCount': 1,
            'totalPages': 1,
            'hasPreviousPage': false,
            'hasNextPage': false,
          },
        }),
        200,
      );
    });

    final page = await ApiAccountRepository(client).list();

    expect(page.items.single.name, 'Nakit');
    expect(page.pagination.totalCount, 1);
  });

  test('account repository sends delete to the owned account route', () async {
    final client = _client((request) async {
      expect(request.method, 'DELETE');
      expect(request.url.path, '/api/v1/accounts/account-id');
      return http.Response('', 204);
    });

    await ApiAccountRepository(client).delete('account-id');
  });

  test('view model exposes unauthorized and empty states', () async {
    final unauthorized = AccountsViewModel(
      _FakeAccountRepository(
        listError: const ApiException(
          statusCode: 401,
          code: 'authentication.required',
          message: 'Unauthorized',
        ),
      ),
    );
    await unauthorized.load();
    expect(unauthorized.status, AccountsViewStatus.unauthorized);

    final empty = AccountsViewModel(_FakeAccountRepository());
    await empty.load();
    expect(empty.status, AccountsViewStatus.empty);
  });

  test('saving an account invalidates the dashboard only', () async {
    final changes = FinancialDataChanges();
    final viewModel = AccountsViewModel(
      _FakeAccountRepository(),
      financialDataChanges: changes,
    );

    final saved = await viewModel.save(
      name: 'Nakit',
      type: 'cash',
      openingBalance: '100.0000',
      isActive: true,
    );

    expect(saved, isTrue);
    expect(changes.dashboardRevision, 1);
    expect(changes.budgetsRevision, 0);
  });

  test(
    'deleting an account reloads accounts and invalidates dashboard',
    () async {
      final changes = FinancialDataChanges();
      final repository = _FakeAccountRepository();
      final viewModel = AccountsViewModel(
        repository,
        financialDataChanges: changes,
      );

      final error = await viewModel.delete(Account.fromJson(_accountJson));

      expect(error, isNull);
      expect(repository.deletedIds, ['account-id']);
      expect(repository.listCalls, 1);
      expect(changes.dashboardRevision, 1);
      expect(changes.budgetsRevision, 0);
      expect(viewModel.message, 'Hesap silindi.');
    },
  );

  // Sunucu kaydı reddederse (aynı adlı hesap) yüklenmiş liste yerinde kalır ve
  // cümle okunabilir (10 Ekim 2026'da cihazda görüldü: `ziraatvadesiz` reddedildi,
  // form hiçbir şey söylemedi ve geri dönünce liste hata ekranına dönmüştü).
  test('reddedilen kayıt hesap listesini hata ekranına çevirmez', () async {
    final repository = _FakeAccountRepository(
      items: [Account.fromJson(_accountJson)],
      writeError: const ApiException(
        statusCode: 409,
        code: 'accounts.duplicate_name',
        message: 'Bu adı taşıyan bir Hesap kaydı zaten var.',
      ),
    );
    final viewModel = AccountsViewModel(repository);
    await viewModel.load();

    final saved = await viewModel.save(
      name: 'nakit',
      type: 'cash',
      openingBalance: '0',
      isActive: true,
    );

    expect(saved, isFalse);
    expect(viewModel.status, AccountsViewStatus.ready);
    expect(viewModel.accounts, hasLength(1));
    expect(viewModel.message, 'Bu adı taşıyan bir Hesap kaydı zaten var.');

    final deleteError = await viewModel.delete(viewModel.accounts.single);

    expect(deleteError, 'Bu adı taşıyan bir Hesap kaydı zaten var.');
    expect(viewModel.status, AccountsViewStatus.ready);
    expect(viewModel.accounts, hasLength(1));
  });

  test('kayıt sırasında oturum düşerse ekran oturum durumuna geçer', () async {
    final viewModel = AccountsViewModel(
      _FakeAccountRepository(
        items: [Account.fromJson(_accountJson)],
        writeError: const ApiException(
          statusCode: 401,
          code: 'authentication.required',
          message: 'Unauthorized',
        ),
      ),
    );
    await viewModel.load();

    final saved = await viewModel.save(
      name: 'Banka',
      type: 'bank',
      openingBalance: '0',
      isActive: true,
    );

    expect(saved, isFalse);
    expect(viewModel.status, AccountsViewStatus.unauthorized);
  });

  testWidgets('reddedilen kayıt formda nedenini yazar ve liste yerinde kalır', (
    tester,
  ) async {
    const sentence =
        'Bu adı taşıyan bir Hesap kaydı zaten var. Farklı bir ad seçin.';
    final viewModel = AccountsViewModel(
      _FakeAccountRepository(
        items: [Account.fromJson(_accountJson)],
        writeError: const ApiException(
          statusCode: 409,
          code: 'accounts.duplicate_name',
          message: sentence,
        ),
      ),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: AccountsPage(viewModel: viewModel),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.byType(FloatingActionButton));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).first, 'nakit');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    // Form açık kalır ve cümleyi yazar.
    expect(find.text('Hesap ekle'), findsOneWidget);
    expect(find.text(sentence), findsOneWidget);

    await tester.pageBack();
    await tester.pumpAndSettle();

    // Arkadaki liste hata ekranına dönmez. Satırda ad da tür de `Nakit` yazar.
    expect(find.text('Nakit'), findsNWidgets(2));
    expect(find.text('Tekrar dene'), findsNothing);
    expect(find.text(sentence), findsNothing);
  });

  testWidgets('account form validates money and prevents double submit', (
    tester,
  ) async {
    final completer = Completer<bool>();
    var count = 0;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: AccountFormPage(
          onSave:
              ({
                account,
                required name,
                required type,
                required openingBalance,
                required isActive,
                defaultScope,
              }) {
                count++;
                return completer.future;
              },
        ),
      ),
    );
    await tester.enterText(find.byType(TextFormField).first, 'Nakit');
    await tester.enterText(find.byType(TextFormField).last, '12.34567');
    await tester.tap(find.text('Kaydet'));
    await tester.pump();
    expect(
      find.text('En fazla dört ondalık basamaklı bir tutar girin.'),
      findsOneWidget,
    );
    expect(count, 0);

    await tester.enterText(find.byType(TextFormField).last, '12.3456');
    await tester.tap(find.text('Kaydet'));
    await tester.pump();
    await tester.tap(find.byType(FilledButton));
    await tester.pump();
    expect(count, 1);
    expect(find.text('Kaydediliyor'), findsOneWidget);
    completer.complete(false);
    await tester.pumpAndSettle();
  });

  testWidgets('editing form confirms before deleting the account', (
    tester,
  ) async {
    var deleteCalls = 0;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: AccountFormPage(
          account: Account.fromJson(_accountJson),
          onSave:
              ({
                account,
                required name,
                required type,
                required openingBalance,
                required isActive,
                defaultScope,
              }) async => false,
          onDelete: (account) async {
            deleteCalls++;
            return null;
          },
        ),
      ),
    );

    await tester.tap(find.text('Hesabı sil'));
    await tester.pumpAndSettle();
    expect(find.text('Hesap kalıcı olarak silinsin mi?'), findsOneWidget);
    expect(deleteCalls, 0);

    await tester.tap(find.text('Kalıcı olarak sil'));
    await tester.pumpAndSettle();
    expect(deleteCalls, 1);
  });

  testWidgets('editing form keeps deletion conflict visible', (tester) async {
    const message = 'Bu hesap finansal geçmişte kullanıldığı için silinemez.';
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: AccountFormPage(
          account: Account.fromJson(_accountJson),
          onSave:
              ({
                account,
                required name,
                required type,
                required openingBalance,
                required isActive,
                defaultScope,
              }) async => false,
          onDelete: (account) async => message,
        ),
      ),
    );

    await tester.tap(find.text('Hesabı sil'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kalıcı olarak sil'));
    await tester.pumpAndSettle();

    expect(find.text(message), findsOneWidget);
    expect(find.text('Hesabı sil'), findsOneWidget);
  });

  testWidgets('sekmeler arasında gidip gelmek view modeli öldürmez', (
    tester,
  ) async {
    // Cihazda kırmızı ekran veren hata buydu: `ownsViewModel` gömülü
    // `AccountsPage`e geçiriliyordu. `TabBarView` görünmeyen sekmeyi atınca o
    // da sahibi sandığı view model'i dispose ediyor, sekmeye dönüldüğünde ölü
    // model'e listener ekleniyordu. Sahiplik host'ta durmalı.
    final viewModel = AccountsViewModel(_FakeAccountRepository());
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: AccountsAndTransfersPage(
          viewModel: viewModel,
          ownsViewModel: true,
        ),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Transferler'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Hesaplar'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);

    // Kayan buton sekmeye göre değişiyor; Transferler'de görünmemeli.
    expect(find.byType(FloatingActionButton), findsOneWidget);
    await tester.tap(find.text('Transferler'));
    await tester.pumpAndSettle();
    expect(find.byType(FloatingActionButton), findsNothing);
  });
}

const _accountJson = {
  'id': 'account-id',
  'name': 'Nakit',
  'type': 'cash',
  'currency': 'TRY',
  'isActive': true,
  'openingBalance': '1250.1250',
  'balance': '1300.1250',
};

ApiClient _client(Future<http.Response> Function(http.Request) handler) =>
    ApiClient(
      config: ApiConfig.fromEnvironment(value: 'https://api.example.test'),
      httpClient: MockClient(handler),
    );

class _FakeAccountRepository implements AccountRepository {
  _FakeAccountRepository({
    this.listError,
    this.writeError,
    this.items = const [],
  });

  final ApiException? listError;

  /// Kayıt, güncelleme ve silmenin reddi.
  final ApiException? writeError;
  final List<Account> items;
  final List<String> deletedIds = [];
  final List<TransactionScope?> createdScopes = [];
  final List<TransactionScope?> updatedScopes = [];
  int listCalls = 0;

  @override
  Future<AccountPage> list({bool? isActive, String? type}) async {
    listCalls++;
    if (listError case final error?) throw error;
    return AccountPage(
      items: items,
      pagination: AccountPagination(
        pageNumber: 1,
        pageSize: 100,
        totalCount: items.length,
        totalPages: items.isEmpty ? 0 : 1,
        hasPreviousPage: false,
        hasNextPage: false,
      ),
    );
  }

  @override
  Future<Account> create({
    required String name,
    required String type,
    required String openingBalance,
    TransactionScope? defaultScope,
  }) async {
    if (writeError case final error?) throw error;
    createdScopes.add(defaultScope);
    return Account.fromJson(_accountJson);
  }

  @override
  Future<Account> update({
    required String id,
    required String name,
    required bool isActive,
    TransactionScope? defaultScope,
  }) async {
    if (writeError case final error?) throw error;
    updatedScopes.add(defaultScope);
    return Account.fromJson(_accountJson);
  }

  @override
  Future<void> delete(String id) async {
    if (writeError case final error?) throw error;
    deletedIds.add(id);
  }
}
