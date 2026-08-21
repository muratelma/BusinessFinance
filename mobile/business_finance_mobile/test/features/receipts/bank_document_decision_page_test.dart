import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_models.dart';
import 'package:business_finance_mobile/features/receipts/presentation/bank_document_decision_page.dart';

void main() {
  testWidgets('nothing is preselected and Devam stays disabled', (
    tester,
  ) async {
    await _pump(tester, _draft());
    await _scrollDown(tester);

    // Varsayılan bir seçim, dalgın bir dokunuşla kart ödemesini gider
    // yazdırırdı ve aynı harcama iki kez sayılırdı.
    expect(_continueButton(tester).onPressed, isNull);
  });

  testWidgets('a choice enables Devam and is reported with the fee switch', (
    tester,
  ) async {
    BankDocumentDecision? decision;
    bool? fee;
    await _pump(
      tester,
      _draft(),
      onDecided: (value, recordFee) {
        decision = value;
        fee = recordFee;
      },
    );

    await tester.tap(find.text('Harcama'));
    await tester.pumpAndSettle();
    await _scrollDown(tester);
    await tester.tap(find.text('Devam'));
    await tester.pumpAndSettle();

    expect(decision, BankDocumentDecision.expense);
    // Ücret anahtarı ücret okunduğunda varsayılan açık: banka o parayı aldı.
    expect(fee, isTrue);
  });

  testWidgets('the fee switch can be turned off before continuing', (
    tester,
  ) async {
    bool? fee;
    await _pump(tester, _draft(), onDecided: (_, recordFee) => fee = recordFee);

    await tester.tap(find.text('Harcama'));
    await tester.pumpAndSettle();
    await _scrollDown(tester);
    await tester.tap(find.byType(SwitchListTile));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Devam'));
    await tester.pumpAndSettle();

    expect(fee, isFalse);
  });

  // Her dekontta ücret olmaz. Satır ancak belgede ücret **yazıyorsa** çıkar;
  // olmayan bir ücret için anahtar göstermek, kullanıcıya olmayan bir kararı
  // sordurmak olurdu.
  testWidgets('no fee on the slip means no fee switch at all', (tester) async {
    await _pump(tester, _draft(fee: null));
    await _scrollDown(tester);

    expect(find.byType(SwitchListTile), findsNothing);
  });

  testWidgets('a fee the model could not read is treated as no fee', (
    tester,
  ) async {
    await _pump(
      tester,
      _draft(fee: '4.5000', feeState: ReceiptFieldState.missing),
    );
    await _scrollDown(tester);

    expect(find.byType(SwitchListTile), findsNothing);
  });

  testWidgets('a zero fee shows no switch either', (tester) async {
    await _pump(tester, _draft(fee: '0.0000'));
    await _scrollDown(tester);

    expect(find.byType(SwitchListTile), findsNothing);
  });

  // İpucu sıralar, seçmez: ATM/virman belgesinde aktarma başa alınır ama
  // hiçbir seçenek işaretlenmez.
  testWidgets('an own-account slip lists the transfer option first', (
    tester,
  ) async {
    await _pump(tester, _draft(kind: ReceiptDocumentKind.bankDocument));

    final transfer = tester.getTopLeft(find.text('Kendi hesabıma aktarma'));
    final expense = tester.getTopLeft(find.text('Harcama'));
    expect(transfer.dy, lessThan(expense.dy));
  });

  testWidgets('a payment slip lists the expense option first', (tester) async {
    await _pump(tester, _draft(kind: ReceiptDocumentKind.bankPayment));

    final expense = tester.getTopLeft(find.text('Harcama'));
    final transfer = tester.getTopLeft(find.text('Kendi hesabıma aktarma'));
    expect(expense.dy, lessThan(transfer.dy));
  });

  // Kart ödeme dekontu tanınırsa doğru cevap başa alınır ve ne görüldüğü
  // söylenir — ama seçim yine yapılmaz: kart ödemesini sessizce varsaymak,
  // kullanıcının görmediği bir karar vermek olurdu.
  testWidgets('a card payment slip lists that option first and says so', (
    tester,
  ) async {
    await _pump(tester, _draft(kind: ReceiptDocumentKind.bankCardPayment));

    expect(
      find.textContaining('kart borcu ödemesi gibi görünüyor'),
      findsOneWidget,
    );

    final cardPayment = tester.getTopLeft(find.text('Kart ödemesi'));
    final expense = tester.getTopLeft(find.text('Harcama'));
    expect(cardPayment.dy, lessThan(expense.dy));

    await _scrollDown(tester);
    expect(_continueButton(tester).onPressed, isNull);
  });

  testWidgets('every option says what it does to the ledger', (tester) async {
    await _pump(tester, _draft());

    expect(find.textContaining('gider olarak yazılır'), findsOneWidget);
    expect(find.textContaining('gelir/gider toplamı değişmez'), findsOneWidget);
    expect(find.textContaining('gider yazılmaz'), findsOneWidget);
    expect(find.textContaining('Alacak olarak kaydedilir'), findsOneWidget);
  });
}

/// `Devam` liste sonunda; test görünümü kısa olduğu için aşağı kaydırılıyor.
Future<void> _scrollDown(WidgetTester tester) async {
  await tester.drag(find.byType(ListView), const Offset(0, -600));
  await tester.pumpAndSettle();
}

FilledButton _continueButton(WidgetTester tester) =>
    tester.widget<FilledButton>(find.widgetWithText(FilledButton, 'Devam'));

Future<void> _pump(
  WidgetTester tester,
  ReceiptDraft draft, {
  void Function(BankDocumentDecision, bool)? onDecided,
}) async {
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: BankDocumentDecisionPage(
        draft: draft,
        onDecided: onDecided ?? (_, _) {},
      ),
    ),
  );
  await tester.pumpAndSettle();
}

ReceiptDraft _draft({
  String? fee = '4.5000',
  ReceiptFieldState? feeState,
  ReceiptDocumentKind kind = ReceiptDocumentKind.bankPayment,
}) => ReceiptDraft(
  documentKind: kind,
  counterpartyName: 'İBRAHİM YEŞİLAĞAÇ',
  counterpartyState: ReceiptFieldState.read,
  purchasedAt: '2019-04-04',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: null,
  dueDateState: ReceiptFieldState.missing,
  totalAmount: '5000.0000',
  totalAmountState: ReceiptFieldState.read,
  installmentCount: null,
  feeAmount: fee,
  feeAmountState:
      feeState ??
      (fee == null ? ReceiptFieldState.missing : ReceiptFieldState.read),
  currencyCode: 'TRY',
  paymentHint: ReceiptPaymentHint.unknown,
  categoryId: null,
  categoryName: null,
  categoryState: ReceiptFieldState.missing,
  warnings: const [],
);
