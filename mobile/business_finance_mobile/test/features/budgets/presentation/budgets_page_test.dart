import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import '../../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
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

  testWidgets('announces an exceeded budget and validates the create form', (
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

    expect(
      find.text('Sıfırdan büyük bir limit girin (en fazla 4 ondalık).'),
      findsOneWidget,
    );
  });

  testWidgets('warns before the limit is exceeded, not only after', (
    tester,
  ) async {
    final controller = BudgetsController(
      _BudgetPageRepository(
        items: const [
          BudgetItem(
            id: 'near',
            categoryId: 'category',
            categoryName: 'Market',
            limit: '100.0000',
            spent: '85.0000',
            remaining: '15.0000',
            exceeded: '0.0000',
            currency: 'TRY',
            scope: TransactionScope.personal,
            year: 2026,
            month: 8,
          ),
        ],
      ),
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('Limitin yüzde 85 kadarı harcandı'), findsOne);
    expect(find.textContaining('aşıldı'), findsNothing);
  });

  testWidgets('stays quiet well below the threshold', (tester) async {
    final controller = BudgetsController(
      _BudgetPageRepository(
        items: const [
          BudgetItem(
            id: 'calm',
            categoryId: 'category',
            categoryName: 'Market',
            limit: '100.0000',
            spent: '40.0000',
            remaining: '60.0000',
            exceeded: '0.0000',
            currency: 'TRY',
            scope: TransactionScope.personal,
            year: 2026,
            month: 8,
          ),
        ],
      ),
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Kalan: ₺60,00'), findsOneWidget);
    expect(find.textContaining('harcandı'), findsNothing);
  });

  testWidgets('hides the scope when the dimension is not visible', (
    tester,
  ) async {
    final controller = BudgetsController(
      _BudgetPageRepository(),
      scopeController: ScopeController(),
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();
    expect(find.text('İşletme'), findsNothing);
  });

  testWidgets('writes which side the budget limits when visible', (
    tester,
  ) async {
    final controller = BudgetsController(
      _BudgetPageRepository(),
      scopeController: _VisibleScopeController(),
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();
    // Bütçe kategori + kapsam çiftini sınırlar; hangi tarafı
    // sınırladığı yazmazsa öteki tarafın harcaması sayılmıyor gibi görünür.
    expect(find.text('İşletme'), findsOneWidget);
  });

  testWidgets('offers the scope chain in the create form when visible', (
    tester,
  ) async {
    final controller = BudgetsController(
      _BudgetPageRepository(),
      scopeController: _VisibleScopeController(),
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(FilledButton, 'Bütçe'));
    await tester.pumpAndSettle();

    expect(find.text('Kapsam'), findsOneWidget);
    expect(
      find.text('Kategorinin varsayılanından önerildi; değiştirebilirsiniz.'),
      findsOneWidget,
    );
  });

  testWidgets('drops categories that already have a budget this month', (
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

    await tester.tap(find.widgetWithText(FilledButton, 'Bütçe'));
    await tester.pumpAndSettle();

    // `Market` bu ay zaten bütçeli; seçilebilir bırakmak kullanıcıyı
    // sunucunun çakışma cevabına götürürdü.
    expect(
      find.text('Bu ayda bütçesi olmayan gider kategoriniz kalmadı.'),
      findsOneWidget,
    );
  });

  testWidgets('prefills the edit field without the raw four decimals', (
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

    await tester.tap(find.byTooltip('Bütçe eylemleri'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Limiti düzenle'));
    await tester.pumpAndSettle();

    expect(find.widgetWithText(TextFormField, '100'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, '100.0000'), findsNothing);
  });

  testWidgets('deletes a budget only after an explicit confirmation', (
    tester,
  ) async {
    final repository = _BudgetPageRepository();
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.byTooltip('Bütçe eylemleri'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Bütçeyi sil'));
    await tester.pumpAndSettle();

    // Onay reddedilirse hiçbir istek gitmez.
    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();
    expect(repository.deletedIds, isEmpty);

    await tester.tap(find.byTooltip('Bütçe eylemleri'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Bütçeyi sil'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Sil'));
    await tester.pumpAndSettle();

    expect(repository.deletedIds, ['budget']);
    expect(find.text('Bütçe silindi.'), findsOneWidget);
  });

  testWidgets('jumps to a distant month from the period picker', (
    tester,
  ) async {
    final repository = _BudgetPageRepository();
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Ağustos 2026'));
    await tester.pumpAndSettle();
    expect(find.text('Dönem seçin'), findsOneWidget);

    await tester.tap(find.text('Ocak'));
    await tester.pumpAndSettle();

    // Sekiz dokunuş ve sekiz istek değil: tek seçim, tek okuma.
    expect(controller.selectedMonth, DateTime(2026, 1));
    expect(repository.listedMonths.last, (2026, 1));
  });

  testWidgets('opens the spending behind the spent amount', (tester) async {
    final repository = _BudgetPageRepository()
      ..spending = const [
        BudgetSpendingLine(
          date: '2026-08-09',
          title: 'Haftalık alışveriş',
          amount: '125.0000',
          currency: 'TRY',
          sourceName: 'Nakit',
        ),
      ];
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.byTooltip('Bütçe eylemleri'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Harcamaları gör'));
    await tester.pumpAndSettle();

    expect(repository.spendingRequests, ['budget']);
    expect(find.text('Market harcamaları'), findsOneWidget);
    expect(find.text('Haftalık alışveriş'), findsOneWidget);
    expect(find.text('9 Ağustos · Nakit'), findsOneWidget);
  });

  testWidgets('says so when nothing fell into the budget', (tester) async {
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

    await tester.tap(find.byTooltip('Bütçe eylemleri'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Harcamaları gör'));
    await tester.pumpAndSettle();

    expect(find.text('Bu ay bu bütçeye düşen harcama yok.'), findsOneWidget);
  });

  testWidgets('copies the previous month from the empty state', (tester) async {
    final repository = _BudgetPageRepository(items: const []);
    final controller = BudgetsController(
      repository,
      initialMonth: DateTime(2026, 8),
    );
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: BudgetsPage(controller: controller),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Bu ay için bütçe yok'), findsOneWidget);
    await tester.tap(find.text('Geçen ayın bütçelerini kopyala'));
    await tester.pumpAndSettle();

    expect(repository.listedMonths, contains((2026, 7)));
  });
}

class _VisibleScopeController extends ScopeController {
  @override
  bool get isVisible => true;
}

class _BudgetPageRepository implements BudgetRepositoryContract {
  _BudgetPageRepository({this.items = _exceeded});

  static const _exceeded = [
    BudgetItem(
      id: 'budget',
      categoryId: 'category',
      categoryName: 'Market',
      limit: '100.0000',
      spent: '125.0000',
      remaining: '0.0000',
      exceeded: '25.0000',
      currency: 'TRY',
      scope: TransactionScope.business,
      year: 2026,
      month: 8,
    ),
  ];

  final List<BudgetItem> items;
  final deletedIds = <String>[];
  final listedMonths = <(int, int)>[];
  final spendingRequests = <String>[];
  List<BudgetSpendingLine> spending = const [];

  @override
  Future<List<BudgetItem>> list(int year, int month) async {
    listedMonths.add((year, month));
    return year == 2026 && month == 8 ? items : const [];
  }

  @override
  Future<BudgetItem> create(CreateBudgetInput input) =>
      throw UnimplementedError();
  @override
  Future<BudgetItem> update(String id, String limit) =>
      throw UnimplementedError();
  @override
  Future<void> delete(String id) async => deletedIds.add(id);
  @override
  Future<List<BudgetSpendingLine>> listSpending(BudgetItem budget) async {
    spendingRequests.add(budget.id);
    return spending;
  }

  @override
  Future<List<BudgetCategory>> listExpenseCategories() async => const [
    BudgetCategory(
      id: 'category',
      name: 'Market',
      isActive: true,
      defaultScope: TransactionScope.business,
    ),
  ];
}
