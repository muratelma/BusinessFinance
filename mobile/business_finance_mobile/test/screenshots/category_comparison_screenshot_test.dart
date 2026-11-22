import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_sections.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Kategori giderleri kartı: toplamın altında önceki aya göre fark (22 Kasım
/// 2026). Tutarlar örnektir ve birbirini tutar: 4.210 + 2.150 + 1.499 + 640 +
/// 402 = 8.901; geçen ayın aynı gününe kadar 7.660; fark 1.241.
void main() {
  const slices = [
    ('market', 'Market Alışverişi', '4210.0000', '47.0000', 1.0),
    ('ev', 'Ev faturaları', '2150.0000', '24.0000', 0.511),
    ('abonelik', 'Abonelikler', '1499.0000', '17.0000', 0.356),
    ('yeme', 'Yeme-içme', '640.0000', '7.0000', 0.152),
    (null, 'Diğer', '402.0000', '5.0000', 0.095),
  ];
  DashboardReport report(ExpenseComparison? comparison) => DashboardReport(
    year: 2026,
    month: 11,
    totalIncome: '11500.0000',
    totalExpense: '8901.0000',
    net: '2599.0000',
    currency: 'TRY',
    categoryExpenses: const [],
    categoryExpenseSlices: [
      for (final (id, name, amount, share, ratio) in slices)
        CategoryExpenseSlice(
          categoryId: id,
          categoryName: name,
          canonicalName: name,
          amount: amount,
          share: share,
          barRatio: ratio,
        ),
    ],
    accountBalances: const [],
    expenseComparison: comparison,
  );

  Widget page(ExpenseComparison? comparison) => Material(
    color: const Color(0xFFF5F6F8),
    child: ListView(
      padding: const EdgeInsets.all(AppSpacing.medium),
      children: [
        DashboardBlock(
          title: 'Kategori giderleri',
          count: '6 kategori',
          onOpen: () {},
          child: DashboardCategoryCard(report: report(comparison)),
        ),
      ],
    ),
  );

  testWidgets('kategori · bugünkü', (tester) async {
    await captureScreen(
      tester,
      'kategori-bugunku',
      page(null),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('kategori · öneri', (tester) async {
    await captureScreen(
      tester,
      'kategori-oneri',
      page(
        const ExpenseComparison(
          previousExpense: '7660.0000',
          change: '1241.0000',
          throughToday: true,
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('kategori · öneri, azalan geçmiş ay', (tester) async {
    await captureScreen(
      tester,
      'kategori-oneri-az',
      page(
        const ExpenseComparison(
          previousExpense: '9870.0000',
          change: '-969.0000',
          throughToday: false,
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);
}
