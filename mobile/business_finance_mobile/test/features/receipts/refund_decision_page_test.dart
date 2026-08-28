import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_models.dart';
import 'package:business_finance_mobile/features/receipts/presentation/refund_decision_page.dart';

void main() {
  // İade fişi yeni bir kayıt üretmez; geri verdiği harcamayı iptal eder. Gider
  // yazılsaydı geri gelen para harcanmış, gelir yazılsaydı kazanılmış
  // görünürdü.
  testWidgets('shows the expense it would cancel before doing anything', (
    tester,
  ) async {
    ReceiptRefundMatch? confirmed;
    await _pump(tester, _draft(match: _match()), (m) => confirmed = m);

    expect(find.text('Bu harcamanızı iptal edeyim mi?'), findsOneWidget);
    expect(find.text('ECZANE ŞİFA'), findsWidgets);
    expect(find.textContaining('320,00'), findsWidgets);
    // Sayfa kendiliğinden hiçbir şey yapmıyor.
    expect(confirmed, isNull);

    await tester.tap(find.text('Harcamayı iptal et'));
    await tester.pumpAndSettle();

    expect(confirmed, isNotNull);
  });

  // Uydurulmuş bir iptal yapılmaz. Bulunamamış olması iadenin gerçek olmadığı
  // anlamına gelmez; harcama başka bir adla yazılmış olabilir.
  testWidgets('offers no cancellation when nothing matched', (tester) async {
    var confirmations = 0;
    await _pump(tester, _draft(), (_) => confirmations++);

    expect(
      find.textContaining('eşleşen bir harcama bulunamadı'),
      findsOneWidget,
    );
    expect(find.text('Harcamayı iptal et'), findsNothing);
    expect(confirmations, 0);
  });

  // Kısmi iadede iptal tek başına yetmez: harcamanın bir kısmı gerçekten
  // yapıldı. Kalan tutar sunucudan geliyor, burada hesaplanmıyor.
  testWidgets('says what happens to the remainder of a partial refund', (
    tester,
  ) async {
    await _pump(
      tester,
      _draft(
        match: _match(amount: '847.5000', remaining: '647.5000'),
      ),
      (_) {},
    );

    expect(find.textContaining('647,50'), findsWidgets);
    expect(find.text('İptal et ve kalanı yaz'), findsOneWidget);
  });
}

Future<void> _pump(
  WidgetTester tester,
  ReceiptDraft draft,
  void Function(ReceiptRefundMatch) onCancel,
) async {
  tester.view.physicalSize = const Size(500, 1400);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: RefundDecisionPage(draft: draft, onCancelExpense: onCancel),
    ),
  );
  await tester.pumpAndSettle();
}

ReceiptRefundMatch _match({String amount = '320.0000', String? remaining}) =>
    ReceiptRefundMatch(
      transactionId: 'tx-1',
      transactionDate: '2026-08-10',
      amount: amount,
      description: 'ECZANE ŞİFA',
      remainingAmount: remaining,
    );

ReceiptDraft _draft({ReceiptRefundMatch? match}) => ReceiptDraft(
  documentKind: ReceiptDocumentKind.refundReceipt,
  counterpartyName: 'ECZANE ŞİFA',
  counterpartyState: ReceiptFieldState.read,
  purchasedAt: '2026-08-12',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: null,
  dueDateState: ReceiptFieldState.missing,
  totalAmount: '320.0000',
  totalAmountState: ReceiptFieldState.read,
  vat: null,
  vatState: ReceiptFieldState.missing,
  feeAmount: null,
  feeAmountState: ReceiptFieldState.missing,
  installmentCount: null,
  currencyCode: 'TRY',
  paymentHint: ReceiptPaymentHint.unknown,
  categoryId: null,
  categoryName: null,
  categoryState: ReceiptFieldState.missing,
  warnings: const [],
  refundMatch: match,
);
