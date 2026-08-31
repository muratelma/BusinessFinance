import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/models/tax_fields.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
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

  /// Faturanın KDV'si yükümlülüğe de girer.
  ///
  /// Kabul turunda bulunan kusur buydu: okuyucu KDV'yi okuyordu, "ödedim"
  /// yolundaki gider formu onu taşıyordu, ama "henüz ödemedim" yolu düşürüyordu.
  /// Yükümlülük ADR 0016'nın KDV taşıyan beş kaydından biri ve muhasebeci
  /// paketine giden tutar oradan geliyor; vadesi gelmemiş olması KDV'sini
  /// değiştirmez.
  testWidgets('carries the VAT read from the invoice into the obligation', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(500, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeRepository();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationFormPage(
          controller: ObligationController(repository),
          today: DateTime(2026, 9, 2),
          scopeController: await _visibleScope(),
          prefill: const ObligationPrefill(
            direction: ObligationDirection.payable,
            amount: QuickAddSuggestion(
              '18428.4000',
              QuickAddSuggestionState.read,
            ),
            categoryId: QuickAddSuggestion(
              'category-1',
              QuickAddSuggestionState.read,
            ),
            vat: VatFields(rate: '0.2000', amount: '3071.4000'),
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    // Belgeden okunan KDV bölümü **açık** gelir: kapalı bir bölümün arkasındaki
    // öneriyi kullanıcı kontrol edemez.
    expect(find.text('KDV oranı'), findsOneWidget);
    expect(find.text('KDV tutarı'), findsOneWidget);
    expect(find.textContaining('%20'), findsWidgets);

    await tester.ensureVisible(find.text('Yükümlülüğü kaydet'));
    await tester.tap(find.text('Yükümlülüğü kaydet'));
    await tester.pumpAndSettle();

    expect(repository.created!['vatRate'], '0.2000');
    expect(repository.created!['vatAmount'], '3071.4000');
  });

  /// KDV'si olmayan yükümlülük KDV taşımaz: boş bırakmak geçerli bir cevaptır
  /// ve uygulama oranı tutardan (ya da tersini) türetmez.
  testWidgets('sends no VAT when the section is left empty', (tester) async {
    tester.view.physicalSize = const Size(500, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final repository = _FakeRepository();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ObligationFormPage(
          controller: ObligationController(repository),
          today: DateTime(2026, 9, 2),
          scopeController: await _visibleScope(),
          prefill: const ObligationPrefill(
            direction: ObligationDirection.payable,
            amount: QuickAddSuggestion(
              '900.0000',
              QuickAddSuggestionState.read,
            ),
            categoryId: QuickAddSuggestion(
              'category-1',
              QuickAddSuggestionState.read,
            ),
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('KDV girilmedi'), findsOneWidget);

    await tester.ensureVisible(find.text('Yükümlülüğü kaydet'));
    await tester.tap(find.text('Yükümlülüğü kaydet'));
    await tester.pumpAndSettle();

    expect(repository.created!['vatRate'], isNull);
    expect(repository.created!['vatAmount'], isNull);
  });
}

Future<ScopeController> _visibleScope() async {
  final controller = ScopeController(store: _MemoryStore());
  await controller.ensureLoaded();
  return controller;
}

class _MemoryStore implements ScopeStore {
  TransactionScope? scope;

  @override
  Future<TransactionScope?> readScope() async => scope;

  @override
  Future<void> writeScope(TransactionScope? value) async => scope = value;

  @override
  Future<bool?> readHasBusiness() async => true;

  @override
  Future<void> writeHasBusiness(bool value) async {}

  @override
  Future<void> clear() async => scope = null;
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
          : const [
              DataChoice(
                'category-1',
                'Faturalar',
                defaultScope: TransactionScope.business,
              ),
            ],
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
