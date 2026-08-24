import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_models.dart';
import 'package:business_finance_mobile/features/obligations/data/obligation_repository.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_controller.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_form_page.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_prefill.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../../helpers/accessibility.dart';

void main() {
  testWidgets('records an unpaid invoice without account or frequency', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(500, 1100);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeRepository();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationFormPage(
          controller: ObligationController(repository),
          today: DateTime(2026, 9, 2),
          prefill: const ObligationPrefill(
            amount: QuickAddSuggestion(
              '412.6000',
              QuickAddSuggestionState.read,
            ),
            issueDate: QuickAddSuggestion(
              '2026-09-01',
              QuickAddSuggestionState.read,
            ),
            dueDate: QuickAddSuggestion(
              '2026-09-30',
              QuickAddSuggestionState.read,
            ),
            description: QuickAddSuggestion(
              'ENERJİSA',
              QuickAddSuggestionState.read,
            ),
            categoryId: QuickAddSuggestion(
              'category-1',
              QuickAddSuggestionState.read,
            ),
            counterpartyId: 'counterparty-1',
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Belge tarihi'), findsOneWidget);
    expect(find.text('Son ödeme tarihi'), findsOneWidget);
    expect(find.textContaining('Sıklık'), findsNothing);
    expect(find.textContaining('Hesap seç'), findsNothing);
    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);

    await tester.ensureVisible(find.text('Yükümlülüğü kaydet'));
    await tester.tap(find.text('Yükümlülüğü kaydet'));
    await tester.pumpAndSettle();

    expect(repository.created, isNotNull);
    expect(repository.created!['direction'], 'payable');
    expect(repository.created!['issueDate'], '2026-09-01');
    expect(repository.created!['dueDate'], '2026-09-30');
    expect(repository.created!['counterpartyId'], 'counterparty-1');
    expect(repository.created, isNot(contains('accountId')));
  });
}

class _FakeRepository implements ObligationRepositoryContract {
  Map<String, Object?>? created;

  @override
  Future<ObligationOptions> loadPayableOptions() async =>
      const ObligationOptions(
        categories: [DataChoice('category-1', 'Faturalar')],
        counterparties: [DataChoice('counterparty-1', 'ENERJİSA')],
      );

  @override
  Future<void> create(Map<String, Object?> input) async {
    created = input;
  }
}
