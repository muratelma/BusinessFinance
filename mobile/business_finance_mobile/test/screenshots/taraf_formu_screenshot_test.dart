import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/categories/data/category_models.dart';
import 'package:business_finance_mobile/features/categories/presentation/category_form_page.dart';
import 'package:business_finance_mobile/features/obligations/data/obligation_repository.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_controller.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_form_page.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_prefill.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'screenshot_harness.dart';

/// Formlardaki taraf bölümü (ADR 0020, Aşama 06.3 Grup 8): tek taraflı
/// kategoride bilgi satırı, iki tarafa açık kategoride çip; kategori formunda
/// `İkisi de` seçeneği. Tasarım teslimi yok; mevcut dille kuruldu.
void main() {
  Widget form() => ObligationFormPage(
    controller: ObligationController(_Obligations()),
    today: DateTime(2026, 10, 8),
    prefill: const ObligationPrefill(),
    scopeController: _VisibleScope(),
  );

  Future<void> chooseCategory(WidgetTester tester, String name) async {
    await tester.tap(find.byType(DropdownButtonFormField<String>).first);
    await tester.pumpAndSettle();
    await tester.tap(find.text(name).last);
    await tester.pumpAndSettle();
  }

  testWidgets('tek taraflı kategori: bilgi satırı', (tester) async {
    await captureScreen(
      tester,
      'taraf-01-bilgi-satiri',
      form(),
      withNavBar: false,
      pushed: true,
      before: (tester) => chooseCategory(tester, 'Ev faturaları'),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('iki tarafa açık kategori: çip', (tester) async {
    await captureScreen(
      tester,
      'taraf-02-cip',
      form(),
      withNavBar: false,
      pushed: true,
      before: (tester) => chooseCategory(tester, 'Sigorta'),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('kategori formu: İkisi de', (tester) async {
    await captureScreen(
      tester,
      'taraf-03-kategori-formu',
      ChangeNotifierProvider<ScopeController?>.value(
        value: _VisibleScope(),
        child: CategoryFormPage(
          category: const BudgetCategory(
            id: 'sigorta',
            name: 'Sigorta',
            type: 'expense',
            isActive: true,
          ),
          onSave:
              ({
                category,
                required name,
                required type,
                required isActive,
                defaultScope,
                isTax = false,
              }) async => true,
        ),
      ),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);
}

class _VisibleScope extends ScopeController {
  @override
  bool get isVisible => true;
}

class _Obligations implements ObligationRepositoryContract {
  @override
  Future<ObligationOptions> loadOptions({required String categoryType}) async =>
      const ObligationOptions(
        categories: [
          DataChoice(
            'ev',
            'Ev faturaları',
            defaultScope: TransactionScope.personal,
          ),
          DataChoice(
            'isyeri',
            'İşyeri faturaları',
            defaultScope: TransactionScope.business,
          ),
          DataChoice('sigorta', 'Sigorta'),
        ],
        counterparties: [],
      );

  @override
  Future<void> create(Map<String, Object?> input) async {}

  @override
  Future<List<ObligationItem>> list({required String asOfDate}) async =>
      const [];

  @override
  Future<List<ObligationAccount>> loadActiveAccounts() async => const [];

  @override
  Future<void> settle({
    required String obligationId,
    required String? accountId,
    required String settlementDate,
    Map<String, Object?>? card,
  }) async {}
}
