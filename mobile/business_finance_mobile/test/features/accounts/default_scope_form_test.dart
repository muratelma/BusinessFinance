import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_form_page.dart';
import 'package:business_finance_mobile/features/categories/data/category_models.dart';
import 'package:business_finance_mobile/features/categories/presentation/category_form_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

/// Varsayılan kapsam alanının üç kapısı: görünürlük, gönderilen değer ve
/// düzenlemede kaybolmaması.
void main() {
  group('hesap formu', () {
    testWidgets('kapsamı görmeyen kullanıcıda alan hiç çıkmaz', (tester) async {
      await tester.pumpWidget(_accountForm(hasBusiness: false));
      await tester.pumpAndSettle();

      expect(find.text('Varsayılan kapsam'), findsNothing);
      expect(find.text('Belirtilmedi'), findsNothing);
    });

    testWidgets('kapsamı gören kullanıcıda üç seçenek çıkar', (tester) async {
      await tester.pumpWidget(_accountForm(hasBusiness: true));
      await tester.pumpAndSettle();

      expect(find.text('Varsayılan kapsam'), findsOneWidget);
      expect(find.text('Belirtilmedi'), findsOneWidget);
      expect(find.text('İşletme'), findsOneWidget);
      expect(find.text('Şahsi'), findsOneWidget);
    });

    testWidgets('seçilen kapsam kaydetmeyle birlikte gider', (tester) async {
      TransactionScope? sent;
      var saved = false;
      await tester.pumpWidget(
        _accountForm(
          hasBusiness: true,
          onSave:
              ({
                account,
                required name,
                required type,
                required openingBalance,
                required isActive,
                defaultScope,
                isTax = false,
              }) async {
                sent = defaultScope;
                saved = true;
                return true;
              },
        ),
      );
      await tester.pumpAndSettle();

      await tester.enterText(find.byType(TextFormField).first, 'Dükkân kasası');
      await tester.tap(find.text('İşletme'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(saved, isTrue);
      expect(sent, TransactionScope.business);
    });

    testWidgets('düzenlemede mevcut kapsam dolu geliyor ve korunuyor', (
      tester,
    ) async {
      // Sunucudaki `PUT` yetkili: alan gönderilmezse silinir. Ad değişikliği
      // hesabın etiketini düşürmemeli.
      TransactionScope? sent;
      await tester.pumpWidget(
        _accountForm(
          hasBusiness: true,
          account: const Account(
            id: 'account-id',
            name: 'Kasa',
            type: 'cash',
            currency: 'TRY',
            isActive: true,
            openingBalance: '0.0000',
            balance: '0.0000',
            defaultScope: TransactionScope.business,
          ),
          onSave:
              ({
                account,
                required name,
                required type,
                required openingBalance,
                required isActive,
                defaultScope,
                isTax = false,
              }) async {
                sent = defaultScope;
                return true;
              },
        ),
      );
      await tester.pumpAndSettle();

      await tester.enterText(find.byType(TextFormField).first, 'Dükkân kasası');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(sent, TransactionScope.business);
    });

    testWidgets('`Belirtilmedi` boş değer gönderir', (tester) async {
      TransactionScope? sent = TransactionScope.business;
      await tester.pumpWidget(
        _accountForm(
          hasBusiness: true,
          account: const Account(
            id: 'account-id',
            name: 'Kasa',
            type: 'cash',
            currency: 'TRY',
            isActive: true,
            openingBalance: '0.0000',
            balance: '0.0000',
            defaultScope: TransactionScope.business,
          ),
          onSave:
              ({
                account,
                required name,
                required type,
                required openingBalance,
                required isActive,
                defaultScope,
                isTax = false,
              }) async {
                sent = defaultScope;
                return true;
              },
        ),
      );
      await tester.pumpAndSettle();

      await tester.tap(find.text('Belirtilmedi'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(sent, isNull);
    });
  });

  group('kategori formu', () {
    testWidgets('kapsamı görmeyen kullanıcıda alan hiç çıkmaz', (tester) async {
      await tester.pumpWidget(_categoryForm(hasBusiness: false));
      await tester.pumpAndSettle();

      expect(find.text('Varsayılan kapsam'), findsNothing);
    });

    testWidgets('seçilen kapsam kaydetmeyle birlikte gider', (tester) async {
      TransactionScope? sent;
      await tester.pumpWidget(
        _categoryForm(
          hasBusiness: true,
          onSave:
              ({
                category,
                required name,
                required type,
                required isActive,
                defaultScope,
                isTax = false,
              }) async {
                sent = defaultScope;
                return true;
              },
        ),
      );
      await tester.pumpAndSettle();

      await tester.enterText(find.byType(TextFormField).first, 'Kira');
      await tester.tap(find.text('İşletme'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(sent, TransactionScope.business);
    });
  });
}

Widget _accountForm({
  required bool hasBusiness,
  Account? account,
  SaveAccount? onSave,
}) => _host(
  hasBusiness: hasBusiness,
  child: AccountFormPage(
    account: account,
    onDelete: account == null ? null : (_) async => null,
    onSave:
        onSave ??
        ({
          account,
          required name,
          required type,
          required openingBalance,
          required isActive,
          defaultScope,
          isTax = false,
        }) async => true,
  ),
);

Widget _categoryForm({
  required bool hasBusiness,
  BudgetCategory? category,
  SaveCategory? onSave,
}) => _host(
  hasBusiness: hasBusiness,
  child: CategoryFormPage(
    category: category,
    onSave:
        onSave ??
        ({
          category,
          required name,
          required type,
          required isActive,
          defaultScope,
          isTax = false,
        }) async => true,
  ),
);

Widget _host({required bool hasBusiness, required Widget child}) {
  final controller = ScopeController(
    store: _FakeScopeStore(hasBusiness: hasBusiness),
    readHasBusiness: () async => hasBusiness,
  )..ensureLoaded();

  return ChangeNotifierProvider<ScopeController>.value(
    value: controller,
    child: MaterialApp(theme: AppTheme.light(), home: child),
  );
}

class _FakeScopeStore implements ScopeStore {
  _FakeScopeStore({required this.hasBusiness});

  final bool hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => null;

  @override
  Future<void> writeScope(TransactionScope? scope) async {}

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async {}

  @override
  Future<void> clear() async {}
}
