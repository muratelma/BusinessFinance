import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_models.dart';
import 'package:business_finance_mobile/features/receipts/presentation/invoice_decision_page.dart';
import 'package:business_finance_mobile/features/receipts/presentation/receipt_prefill.dart';

void main() {
  // Son ödeme tarihi yazması ödendiğini söylemez. Doğrudan gider yazmak, henüz
  // çıkmamış parayı çıkmış göstermek olurdu.
  testWidgets('asks whether the invoice was paid and preselects nothing', (
    tester,
  ) async {
    final answers = <bool>[];
    await _pump(tester, _draft(), answers.add);

    expect(find.text('Bu faturayı ödediniz mi?'), findsOneWidget);
    expect(find.text('Ödedim'), findsOneWidget);
    expect(find.text('Henüz ödemedim'), findsOneWidget);
    expect(find.textContaining('30.09.2026'), findsNothing);
    // Vade ekranda okunur biçimde; ham ISO yalnız sözleşmede kalır.
    expect(find.textContaining('30 Eylül 2026'), findsOneWidget);
    // Sayfa kendiliğinden bir yön seçmiyor.
    expect(answers, isEmpty);
  });

  testWidgets('reports paid and unpaid as different answers', (tester) async {
    final answers = <bool>[];
    await _pump(tester, _draft(), answers.add);

    await tester.tap(find.text('Ödedim'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Henüz ödemedim'));
    await tester.pumpAndSettle();

    expect(answers, [true, false]);
  });

  // Belge tarihi giderin tanınma, son ödeme tarihi borcun vade günüdür.
  test('keeps the invoice issue and due dates separate', () {
    final prefill = receiptObligationPrefillFrom(_draft());

    expect(prefill.issueDate?.value, '2026-09-01');
    expect(prefill.dueDate?.value, '2026-09-30');
    expect(prefill.amount?.value, '412.6000');
    expect(prefill.description?.value, 'ENERJİSA');
    expect(prefill.counterpartyId, 'counterparty-1');
    // Ödeme kaynağı taşınmıyor: fatura hangi hesaptan ödeneceğini söylemez.
    expect(prefill.isEmpty, isFalse);
  });

  test('a document without a due date asks nothing', () {
    expect(_draft(dueDate: null).needsPaidQuestion, isFalse);
    expect(_draft().needsPaidQuestion, isTrue);
  });
}

Future<void> _pump(
  WidgetTester tester,
  ReceiptDraft draft,
  void Function(bool) onDecided,
) async {
  tester.view.physicalSize = const Size(500, 1200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: InvoiceDecisionPage(draft: draft, onDecided: onDecided),
    ),
  );
  await tester.pumpAndSettle();
}

ReceiptDraft _draft({String? dueDate = '2026-09-30'}) => ReceiptDraft(
  documentKind: ReceiptDocumentKind.invoiceOrVoucher,
  counterpartyName: 'ENERJİSA',
  counterpartyState: ReceiptFieldState.read,
  counterpartyId: 'counterparty-1',
  purchasedAt: '2026-09-01',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: dueDate,
  dueDateState: dueDate == null
      ? ReceiptFieldState.missing
      : ReceiptFieldState.read,
  totalAmount: '412.6000',
  totalAmountState: ReceiptFieldState.read,
  feeAmount: null,
  feeAmountState: ReceiptFieldState.missing,
  installmentCount: null,
  currencyCode: 'TRY',
  paymentHint: ReceiptPaymentHint.unknown,
  categoryId: 'cat-1',
  categoryName: 'Faturalar',
  categoryState: ReceiptFieldState.read,
  warnings: const [],
);
