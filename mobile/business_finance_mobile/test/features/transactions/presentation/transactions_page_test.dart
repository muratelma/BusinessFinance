import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import '../../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';
import 'package:business_finance_mobile/features/transactions/presentation/transactions_controller.dart';
import 'package:business_finance_mobile/features/transactions/presentation/transactions_page.dart';

void main() {
  testWidgets('işlemler ekranı erişilebilirlik kapısını geçer', (tester) async {
    final controller = TransactionsController(_PageRepository());
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: TransactionsPage(controller: controller),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  testWidgets('shows cancelled transaction as preserved history', (
    tester,
  ) async {
    final controller = TransactionsController(_PageRepository());
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: MediaQuery(
          data: const MediaQueryData(textScaler: TextScaler.linear(2)),
          child: TransactionsPage(controller: controller),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('İptal edildi'), findsOneWidget);
    expect(find.byTooltip('İşlemi iptal et'), findsNothing);
    expect(
      tester.getSemantics(find.byType(AppCard)).label,
      contains('İptal edildi'),
    );

    await tester.tap(find.byTooltip('İşlemleri filtrele'));
    await tester.pumpAndSettle();

    expect(find.text('Hesap'), findsOneWidget);
    expect(find.text('Kategori'), findsOneWidget);

    await tester.tap(find.text('Temizle'));
    await tester.pumpAndSettle();
    await tester.tap(find.byTooltip('İşlem ekle'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kaydet'));
    await tester.pump();

    expect(
      find.text('Pozitif bir tutar girin (en fazla 4 ondalık).'),
      findsOneWidget,
    );
  });
}

class _PageRepository implements TransactionRepositoryContract {
  @override
  Future<TransactionPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    TransactionFilter filter = const TransactionFilter(),
  }) async => TransactionPage(
    items: const [
      TransactionItem(
        id: 'transaction',
        accountId: 'account',
        categoryId: 'category',
        amount: '25.5000',
        currency: 'TRY',
        kind: TransactionKind.expense,
        transactionDate: '2026-08-09',
        isCancelled: true,
      ),
    ],
    pageNumber: 1,
    pageSize: 20,
    totalCount: 1,
    totalPages: 1,
    hasPreviousPage: false,
    hasNextPage: false,
  );

  @override
  Future<TransactionItem> cancel(String id) => throw UnimplementedError();
  @override
  Future<TransactionItem> create(CreateTransactionInput input) =>
      throw UnimplementedError();
  @override
  Future<List<TransactionChoice>> listAccounts() async => const [
    TransactionChoice(id: 'account', name: 'Nakit', isActive: true),
  ];
  @override
  Future<List<TransactionChoice>> listCategories(TransactionKind kind) async =>
      [
        TransactionChoice(
          id: 'category-${kind.apiValue}',
          name: kind == TransactionKind.income ? 'Maaş' : 'Market',
          isActive: true,
          kind: kind,
        ),
      ];
}
