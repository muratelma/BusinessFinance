import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/obligations/data/obligation_repository.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_controller.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligations_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../../helpers/accessibility.dart';

void main() {
  testWidgets('gecikmiş yükümlülük hesaptan ödenip kapanır', (tester) async {
    final repository = _FakeRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationsPage(controller: ObligationListController(repository)),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Geciken'));
    await tester.pumpAndSettle();
    expect(find.text('Gecikmiş'), findsOneWidget);
    expect(find.textContaining('Ödenecek'), findsOneWidget);

    await tester.tap(find.text('Enerji Tedarik'));
    await tester.pumpAndSettle();
    await tester.tap(find.byType(DropdownButtonFormField<String>));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Sentetik kasa'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Öde ve kapat'));
    await tester.pumpAndSettle();

    expect(repository.settledId, 'obligation-1');
    expect(find.text('Ödemeyi kaydet'), findsNothing);
  });

  testWidgets('yükümlülük listesi erişilebilirlik kapısını geçer', (
    tester,
  ) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationsPage(
          controller: ObligationListController(_FakeRepository()),
        ),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

class _FakeRepository implements ObligationRepositoryContract {
  String? settledId;

  @override
  Future<List<ObligationItem>> list({required String asOfDate}) async => [
    ObligationItem(
      id: 'obligation-1',
      direction: 'payable',
      amount: '412.6000',
      currency: 'TRY',
      issueDate: '2026-08-05',
      dueDate: '2026-08-20',
      status: settledId == null ? 'open' : 'settled',
      isOverdue: settledId == null,
      counterpartyName: 'Enerji Tedarik',
      categoryName: 'Faturalar',
    ),
  ];

  @override
  Future<List<ObligationAccount>> loadActiveAccounts() async => const [
    ObligationAccount(id: 'account-1', name: 'Sentetik kasa'),
  ];

  @override
  Future<void> settle({
    required String obligationId,
    required String accountId,
    required String settlementDate,
  }) async {
    settledId = obligationId;
  }

  @override
  Future<ObligationOptions> loadOptions({required String categoryType}) async =>
      const ObligationOptions(categories: [], counterparties: []);

  @override
  Future<void> create(Map<String, Object?> input) async {}
}
