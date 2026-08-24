import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_models.dart';
import 'package:business_finance_mobile/features/obligations/data/obligation_direction.dart';
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
            direction: ObligationDirection.payable,
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
    // Yön fiş dalında bir adım önce soruldu; form onu tekrar sormaz.
    expect(find.byType(SegmentedButton<ObligationDirection>), findsNothing);
    expect(find.text('Ödenmemiş faturayı kaydet'), findsOneWidget);
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

  /// Elle giriş: fotoğrafı olmayan kullanıcı da aynı kaydı açabilir ve yönü
  /// kendisi seçer. Yön değişince kategori listesi yeniden okunur — gider
  /// kategorisi bir alacağa yazılamaz.
  testWidgets('opens with no suggestion and lets the user pick the direction', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(500, 1200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeRepository();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationFormPage(
          controller: ObligationController(repository),
          today: DateTime(2026, 9, 2),
          prefill: const ObligationPrefill(),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Yükümlülük ekle'), findsOneWidget);
    expect(find.byType(SegmentedButton<ObligationDirection>), findsOneWidget);
    expect(repository.requestedCategoryTypes, ['expense']);
    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);

    await tester.tap(find.text('Tahsil edilecek'));
    await tester.pumpAndSettle();
    expect(repository.requestedCategoryTypes, ['expense', 'income']);

    await tester.tap(find.byType(DropdownButtonFormField<String>));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Veresiye satış').last);
    await tester.pumpAndSettle();
    await tester.enterText(find.widgetWithText(TextFormField, 'Tutar'), '250');
    await tester.pumpAndSettle();

    await tester.ensureVisible(find.text('Yükümlülüğü kaydet'));
    await tester.tap(find.text('Yükümlülüğü kaydet'));
    await tester.pumpAndSettle();

    expect(repository.created, isNotNull);
    expect(repository.created!['direction'], 'receivable');
    expect(repository.created!['categoryId'], 'category-2');
    expect(repository.created!['counterpartyId'], isNull);
    expect(repository.created, isNot(contains('accountId')));
  });
}

class _FakeRepository implements ObligationRepositoryContract {
  Map<String, Object?>? created;
  final List<String> requestedCategoryTypes = [];

  @override
  Future<ObligationOptions> loadOptions({required String categoryType}) async {
    requestedCategoryTypes.add(categoryType);
    return ObligationOptions(
      categories: categoryType == 'income'
          ? const [DataChoice('category-2', 'Veresiye satış')]
          : const [DataChoice('category-1', 'Faturalar')],
      counterparties: const [DataChoice('counterparty-1', 'ENERJİSA')],
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async {
    created = input;
  }

  @override
  Future<List<ObligationItem>> list({required String asOfDate}) async =>
      const [];

  @override
  Future<List<ObligationAccount>> loadActiveAccounts() async => const [];

  @override
  Future<void> settle({
    required String obligationId,
    required String accountId,
    required String settlementDate,
  }) async {}
}
