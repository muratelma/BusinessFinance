import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/categories/data/category_models.dart';
import 'package:business_finance_mobile/features/categories/data/category_repository.dart';
import 'package:business_finance_mobile/features/categories/presentation/categories_page.dart';
import 'package:business_finance_mobile/features/categories/presentation/categories_view_model.dart';

import '../../helpers/accessibility.dart';

void main() {
  group('kategoriler ekranı', () {
    testWidgets('gelir ve gider ayrı sekmelerde listelenir', (tester) async {
      // Tek karışık liste kullanıcıyı her aramada tür ayıklamaya zorluyordu.
      await _pump(tester);

      expect(find.text('Giderler'), findsOneWidget);
      expect(find.text('Gelirler'), findsOneWidget);

      // Açılışta gider sekmesi: gelir kategorileri burada görünmemeli.
      expect(find.text('Market Alışverişi'), findsOneWidget);
      expect(find.text('Maaş'), findsNothing);

      await tester.tap(find.text('Gelirler'));
      await tester.pumpAndSettle();

      expect(find.text('Maaş'), findsOneWidget);
      expect(find.text('Market Alışverişi'), findsNothing);
    });

    testWidgets('pasif kategoriler ayrı başlık altında toplanır', (
      tester,
    ) async {
      // Pasif kategori geçmişte kullanılmış olabilir, silinmez; ama çalışan
      // kategorileri de gömmemeli.
      await _pump(tester);

      expect(find.text('Kullanımda'), findsOneWidget);
      expect(find.text('Pasif'), findsOneWidget);
      expect(find.text('Yeni kayıtta seçilemez'), findsOneWidget);
    });

    testWidgets('her kategori kendi ikonunu alır', (tester) async {
      await _pump(tester);

      // Aynı listede altı gider kategorisi varsa hepsi aynı ikonu
      // paylaşmamalı; ikon ancak ayırt ediyorsa bilgi taşır.
      final icons = tester
          .widgetList<Icon>(find.byType(Icon))
          .map((icon) => icon.icon)
          .toSet();
      expect(icons.length, greaterThan(3));
    });

    testWidgets('boş sekme ne yapılacağını söyler', (tester) async {
      // Hiç kategori yoksa ekranın kendi boş durumu çıkar; burada test edilen
      // tek bir sekmenin boş kalması.
      await _pump(
        tester,
        categories: const [
          BudgetCategory(
            id: '1',
            name: 'Faturalar',
            type: 'expense',
            isActive: true,
          ),
        ],
      );

      await tester.tap(find.text('Gelirler'));
      await tester.pumpAndSettle();

      expect(find.text('Gelir kategorisi yok'), findsOneWidget);
    });

    testWidgets('erişilebilirlik kapısını geçer', (tester) async {
      final viewModel = CategoriesViewModel(_Repository(_categories()));
      await viewModel.load();

      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: CategoriesPage(viewModel: viewModel),
        ),
      );

      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    });
  });
}

Future<void> _pump(
  WidgetTester tester, {
  List<BudgetCategory>? categories,
}) async {
  tester.view.physicalSize = const Size(400, 1400);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  final viewModel = CategoriesViewModel(
    _Repository(categories ?? _categories()),
  );
  await viewModel.load();

  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: CategoriesPage(viewModel: viewModel),
    ),
  );
  await tester.pumpAndSettle();
}

List<BudgetCategory> _categories() => const [
  BudgetCategory(
    id: '1',
    name: 'Market Alışverişi',
    type: 'expense',
    isActive: true,
  ),
  BudgetCategory(id: '2', name: 'Faturalar', type: 'expense', isActive: true),
  BudgetCategory(id: '3', name: 'Ulaşım', type: 'expense', isActive: true),
  BudgetCategory(id: '4', name: 'Sağlık', type: 'expense', isActive: true),
  BudgetCategory(
    id: '5',
    name: 'Eski Abonelik',
    type: 'expense',
    isActive: false,
  ),
  BudgetCategory(id: '6', name: 'Maaş', type: 'income', isActive: true),
  BudgetCategory(id: '7', name: 'Diğer Gelir', type: 'income', isActive: true),
];

class _Repository implements CategoryRepository {
  _Repository(this.categories);

  final List<BudgetCategory> categories;

  @override
  Future<List<BudgetCategory>> list({String? type, bool? isActive}) async =>
      categories;

  @override
  Future<BudgetCategory> create({required String name, required String type}) =>
      throw UnimplementedError();

  @override
  Future<BudgetCategory> update({
    required String id,
    required bool isActive,
    required String name,
  }) => throw UnimplementedError();
}
