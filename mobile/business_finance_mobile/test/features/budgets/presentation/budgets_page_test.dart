import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import '../../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_models.dart';
import 'package:business_finance_mobile/features/budgets/data/budget_repository.dart';
import 'package:business_finance_mobile/features/budgets/presentation/budgets_controller.dart';
import 'package:business_finance_mobile/features/budgets/presentation/budgets_page.dart';

void main() {
  testWidgets('bütçeler ekranı erişilebilirlik kapısını geçer', (tester) async {
    final controller = BudgetsController(
      _BudgetPageRepository(),
      initialMonth: DateTime(2026, 8),
    );
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  testWidgets('announces an exceeded budget and keeps edit accessible', (
    tester,
  ) async {
    final controller = BudgetsController(
      _BudgetPageRepository(),
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('Limit ₺25,00 aşıldı'), findsOneWidget);
    expect(find.byTooltip('Bütçe limitini düzenle'), findsOneWidget);
    // Kartın hangi widget'la çizildiğine değil, ekran okuyucunun aşımı
    // duyurduğuna bağlanır.
    expect(
      find.bySemanticsLabel(RegExp('Limit ₺25,00 aşıldı')),
      findsOneWidget,
    );

    // Eylem artık tooltip'e gömülü sade bir ikon değil, etiketi görünen dolgulu
    // bir buton: başlık çubuğunda kayboluyordu ve tooltip yalnız uzun basınca
    // çıktığı için ikon tek başına "neyi ekliyorum" sorusunu cevaplamıyordu.
    await tester.tap(find.widgetWithText(FilledButton, 'Bütçe'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Oluştur'));
    await tester.pump();

    expect(find.text('Kategori seçin.'), findsOneWidget);
    expect(
      find.text('Sıfırdan büyük bir limit girin (en fazla 4 ondalık).'),
      findsOneWidget,
    );
  });
}

class _BudgetPageRepository implements BudgetRepositoryContract {
  @override
  Future<List<BudgetItem>> list(int year, int month) async => const [
    BudgetItem(
      id: 'budget',
      categoryId: 'category',
      categoryName: 'Market',
      limit: '100.0000',
      spent: '125.0000',
      remaining: '0.0000',
      exceeded: '25.0000',
      currency: 'TRY',
      year: 2026,
      month: 8,
    ),
  ];
  @override
  Future<BudgetItem> create(CreateBudgetInput input) =>
      throw UnimplementedError();
  @override
  Future<BudgetItem> update(String id, String limit) =>
      throw UnimplementedError();
  @override
  Future<List<BudgetCategory>> listExpenseCategories() async => const [
    BudgetCategory(id: 'category', name: 'Market', isActive: true),
  ];
}
