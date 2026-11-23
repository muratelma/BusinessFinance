import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:business_finance_mobile/app/business_finance_app.dart';
import 'package:business_finance_mobile/core/routing/app_router.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_controller.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_models.dart';
import 'package:business_finance_mobile/features/debt_book/data/debt_book_models.dart';
import 'package:business_finance_mobile/features/debt_book/data/debt_book_repository.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_entry_prefill.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_entry_sheet.dart';
import 'package:provider/provider.dart';

import '../../helpers/fake_auth.dart';
import '../../helpers/fake_dashboard.dart';

/// Karar sayfasından bir forma geçmek Navigator'ı çökertiyordu.
///
/// Sayfa **üst seviye** bir rotaydı. Kullanıcı fiş ekranından
/// (`/transactions/new/receipt`) geldiği için shell zinciri zaten yığındaydı;
/// üst seviye sayfadan `/transactions/new/expense`'e geçmek o zinciri yeniden
/// kuruyor ve Navigator aynı sayfa anahtarını ikinci kez kaydedip
/// `!keyReservation.contains(key)` ile patlıyordu.
///
/// Testin fiş ekranından geçmesi şart: doğrudan karar sayfasına gidildiğinde
/// hata **çıkmıyor**. Sayfanın kendi widget testleri de bu yüzden görmemişti —
/// gerçek router olmadan çalışıyorlar. Çözüm, karar sayfasını fiş ekranıyla
/// kardeş rotaya taşımak oldu.
void main() {
  testWidgets('choosing an option navigates without a duplicate page key', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    final router = createAppRouter(authController: controller);

    await tester.pumpWidget(_app(controller, router));
    await tester.pumpAndSettle();

    // Gerçek akış: kullanıcı fiş ekranından geliyor.
    router.go('/transactions/new/receipt');
    await tester.pumpAndSettle();
    router.pushReplacement('/transactions/new/bank-document', extra: _draft());
    await tester.pumpAndSettle();

    expect(find.text('Dekont okundu'), findsOneWidget);

    await tester.tap(find.text('Harcama'));
    await tester.pumpAndSettle();
    await tester.drag(find.byType(ListView), const Offset(0, -600));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Devam'));
    await tester.pumpAndSettle();

    // Çökmeden gider rotasına geçildi. Depolar bu testte tanımsız olduğu için
    // form yerine yapılandırma uyarısı çiziliyor; önemli olan gezinmenin
    // Navigator'ı patlatmaması.
    expect(tester.takeException(), isNull);
    expect(find.text('Dekont okundu'), findsNothing);
  });

  // Aynı çökme sınıfı her seçenek için ayrı ayrı geçerli: karar sayfası dört
  // ayrı rotaya gidiyor ve biri kırılırsa diğerlerinin geçmesi bunu gizler.
  testWidgets('every option navigates away without crashing', (tester) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    for (final option in const [
      'Kendi hesabıma aktarma',
      'Kart ödemesi',
      'Geri bekliyorum',
    ]) {
      final repository = FakeAuthSessionRepository()..session = testSession();
      final controller = AuthController(repository);
      await controller.initialize();
      final router = createAppRouter(authController: controller);

      await tester.pumpWidget(_app(controller, router));
      await tester.pumpAndSettle();

      router.go('/transactions/new/receipt');
      await tester.pumpAndSettle();
      router.pushReplacement(
        '/transactions/new/bank-document',
        extra: _draft(),
      );
      await tester.pumpAndSettle();

      await tester.tap(find.text(option));
      await tester.pumpAndSettle();
      await tester.drag(find.byType(ListView), const Offset(0, -600));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Devam'));
      await tester.pumpAndSettle();

      expect(tester.takeException(), isNull, reason: option);
      expect(find.text('Dekont okundu'), findsNothing, reason: option);
    }
  });

  // İade sayfası da fiş ekranıyla kardeş olmak zorunda: kısmi iadede oradan
  // gider formuna geçiliyor ve üst seviye bir rota aynı Navigator çökmesini
  // üretirdi.
  testWidgets('the refund page reaches the expense form without crashing', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    final router = createAppRouter(authController: controller);

    await tester.pumpWidget(_app(controller, router));
    await tester.pumpAndSettle();

    router.go('/transactions/new/receipt');
    await tester.pumpAndSettle();
    router.pushReplacement(refundDecisionLocation, extra: _refundDraft());
    await tester.pumpAndSettle();

    expect(find.text('İade fişi okundu'), findsOneWidget);
    expect(find.text('Bu harcamanızı iptal edeyim mi?'), findsOneWidget);

    // Depolar bu testte tanımsız: iptal çağrısı hiç yapılmıyor ve sayfa
    // yerinde kalıyor. Önemli olan rotanın var olması ve Navigator'ın
    // patlamaması.
    await tester.tap(find.text('İptal et ve kalanı yaz'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
  });

  // Fatura sayfası da kardeş rota: ödendi cevabı gider formuna, ödenmedi
  // cevabı `Vadeli alım` formuna geçiyor ve ikisi de Navigator'ı
  // patlatmamalı.
  testWidgets('the invoice page routes both answers without crashing', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    for (final answer in const ['Ödedim', 'Henüz ödemedim']) {
      final repository = FakeAuthSessionRepository()..session = testSession();
      final controller = AuthController(repository);
      await controller.initialize();
      final router = createAppRouter(
        authController: controller,
        debtBookRepository: _FakeDebtBook(),
      );

      await tester.pumpWidget(_app(controller, router));
      await tester.pumpAndSettle();

      router.go('/transactions/new/receipt');
      await tester.pumpAndSettle();
      router.pushReplacement(invoiceDecisionLocation, extra: _invoiceDraft());
      await tester.pumpAndSettle();

      expect(find.text('Fatura okundu'), findsOneWidget, reason: answer);

      await tester.tap(find.text(answer));
      await tester.pumpAndSettle();

      expect(tester.takeException(), isNull, reason: answer);
      expect(find.text('Fatura okundu'), findsNothing, reason: answer);
      if (answer == 'Henüz ödemedim') {
        expect(find.text('Vadeli alım'), findsOneWidget);
        expect(find.textContaining('Sıklık'), findsNothing);
      }
    }
  });

  // Formdan geri dönen kullanıcı okunmuş faturasını kaybetmemeli: "ödedim
  // mi?" sorusuna döner ve öbür cevabı seçebilir (8 Ekim 2026'da cihazda
  // görüldü: geri tuşu okutmaya başlanan ekrana atıyordu).
  testWidgets('going back from the form returns to the paid question', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    final router = createAppRouter(
      authController: controller,
      debtBookRepository: _FakeDebtBook(),
    );

    await tester.pumpWidget(_app(controller, router));
    await tester.pumpAndSettle();

    router.go('/transactions/new/receipt');
    await tester.pumpAndSettle();
    router.pushReplacement(invoiceDecisionLocation, extra: _invoiceDraft());
    await tester.pumpAndSettle();
    await tester.tap(find.text('Henüz ödemedim'));
    await tester.pumpAndSettle();
    expect(find.text('Vadeli alım'), findsOneWidget);

    // Sistemin geri tuşu: en üstteki sayfa kapanır.
    router.pop();
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Fatura okundu'), findsOneWidget);
    expect(find.text('Henüz ödemedim'), findsOneWidget);
  });

  // Aynı kural öbür cevapta: gider formundan geri dönen kullanıcı da soruya
  // döner ve "henüz ödemedim" diyebilir; okunan fatura yerinde durur.
  testWidgets('going back from the expense form keeps the scanned invoice', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    final router = createAppRouter(
      authController: controller,
      debtBookRepository: _FakeDebtBook(),
    );

    await tester.pumpWidget(_app(controller, router));
    await tester.pumpAndSettle();

    router.go('/transactions/new/receipt');
    await tester.pumpAndSettle();
    router.pushReplacement(invoiceDecisionLocation, extra: _invoiceDraft());
    await tester.pumpAndSettle();
    await tester.tap(find.text('Ödedim'));
    await tester.pumpAndSettle();
    expect(find.text('Fatura okundu'), findsNothing);

    router.pop();
    await tester.pumpAndSettle();
    expect(find.text('Fatura okundu'), findsOneWidget);

    // Öbür cevap aynı faturayla açılır: tutar ve satıcı okunduğu gibi durur.
    await tester.tap(find.text('Henüz ödemedim'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Vadeli alım'), findsOneWidget);
    // Kişi eşleşmedi: okunan ad kaydın açıklamasıdır (K10).
    expect(find.text('ENERJİSA'), findsOneWidget);
  });

  // Kayıt yazılınca soru sayfası da kapanır: kullanıcı aynı faturayı ikinci
  // kez kaydetmeye davet edilmez. Kişi eşleşmediği için kayıt kişisiz yazılır
  // (K10).
  testWidgets('saving the unpaid invoice also closes the paid question', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 1400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    final debtBook = _FakeDebtBook();
    final router = createAppRouter(
      authController: controller,
      debtBookRepository: debtBook,
    );

    await tester.pumpWidget(_app(controller, router));
    await tester.pumpAndSettle();

    router.go('/transactions/new/receipt');
    await tester.pumpAndSettle();
    router.pushReplacement(
      invoiceDecisionLocation,
      extra: _invoiceDraft(categoryId: 'category-1'),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Henüz ödemedim'));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(debtBook.debts, hasLength(1));
    expect(debtBook.debts.single['counterpartyId'], isNull);
    expect(debtBook.debts.single['description'], 'ENERJİSA');
    expect(debtBook.debts.single['firstDueDate'], '2026-09-30');
    expect(find.text('Vadeli alım'), findsNothing);
    expect(find.text('Fatura okundu'), findsNothing);
  });

  // Yeni kayıt formu **önerisiz de** açılabilmeli: `İşlem ekle` menüsünden
  // gelen kullanıcının okunmuş bir belgesi yok.
  testWidgets('the debt form opens with no suggestion at all', (tester) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    final router = createAppRouter(
      authController: controller,
      debtBookRepository: _FakeDebtBook(),
    );

    await tester.pumpWidget(_app(controller, router));
    await tester.pumpAndSettle();

    router.go(
      debtEntryLocation,
      extra: const DebtEntryRequest(DebtEntryKind.sale),
    );
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Veresiye satış'), findsOneWidget);
    expect(find.text('Borç formu açılamadı.'), findsNothing);
  });

  // Dekontun kendi rotası var ve o da fiş ekranıyla kardeş: karar sayfasına
  // oradan geçiliyor. Rota adı tek sabitte (`bankSlipScanLocation`) duruyor,
  // çünkü menüdeki çağrı ile rota tanımı ayrı dizgiler olduğunda uygulama
  // cihazda "no routes for location" ile patlamıştı.
  testWidgets('the bank-slip page reaches the decision page', (tester) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final repository = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(repository);
    await controller.initialize();
    final router = createAppRouter(authController: controller);

    await tester.pumpWidget(_app(controller, router));
    await tester.pumpAndSettle();

    router.go(bankSlipScanLocation);
    await tester.pumpAndSettle();
    expect(tester.takeException(), isNull);

    router.pushReplacement(bankDocumentDecisionLocation, extra: _draft());
    await tester.pumpAndSettle();

    expect(find.text('Dekont okundu'), findsOneWidget);

    await tester.tap(find.text('Kendi hesabıma aktarma'));
    await tester.pumpAndSettle();
    await tester.drag(find.byType(ListView), const Offset(0, -600));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Devam'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Dekont okundu'), findsNothing);
  });
}

/// Yeni kayıt formunun okuduğu ve yazdığı uçlar; diğerleri çağrılmaz.
class _FakeDebtBook implements DebtBookRepositoryContract {
  final debts = <Map<String, Object?>>[];

  @override
  Future<DebtEntryOptions> loadEntryOptions() async => const DebtEntryOptions(
    incomeCategories: [(id: 'category-2', name: 'Satışlar', scope: null)],
    expenseCategories: [(id: 'category-1', name: 'Faturalar', scope: null)],
    sources: DebtSettleSources(accounts: [], cards: [], posDefinitions: []),
  );

  @override
  Future<String> createDebt(Map<String, Object?> body) async {
    debts.add(body);
    return 'debt-${debts.length}';
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

Widget _app(AuthController controller, GoRouter router) =>
    ChangeNotifierProvider.value(
      value: testDashboardViewModel(),
      child: BusinessFinanceApp(authController: controller, router: router),
    );

ReceiptDraft _draft() => const ReceiptDraft(
  documentKind: ReceiptDocumentKind.bankPayment,
  counterpartyName: 'Enerjisa Başkent Satış',
  counterpartyState: ReceiptFieldState.read,
  purchasedAt: '2020-05-23',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: null,
  dueDateState: ReceiptFieldState.missing,
  totalAmount: '47.8000',
  totalAmountState: ReceiptFieldState.read,
  feeAmount: '2.0000',
  feeAmountState: ReceiptFieldState.read,
  installmentCount: null,
  currencyCode: 'TRY',
  paymentHint: ReceiptPaymentHint.unknown,
  categoryId: null,
  categoryName: null,
  categoryState: ReceiptFieldState.missing,
  warnings: [],
);

ReceiptDraft _refundDraft() => const ReceiptDraft(
  documentKind: ReceiptDocumentKind.refundReceipt,
  counterpartyName: 'ECZANE ŞİFA',
  counterpartyState: ReceiptFieldState.read,
  purchasedAt: '2026-08-12',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: null,
  dueDateState: ReceiptFieldState.missing,
  totalAmount: '200.0000',
  totalAmountState: ReceiptFieldState.read,
  feeAmount: null,
  feeAmountState: ReceiptFieldState.missing,
  installmentCount: null,
  currencyCode: 'TRY',
  paymentHint: ReceiptPaymentHint.unknown,
  categoryId: null,
  categoryName: null,
  categoryState: ReceiptFieldState.missing,
  warnings: [],
  refundMatch: ReceiptRefundMatch(
    transactionId: 'tx-1',
    transactionDate: '2026-08-10',
    amount: '847.5000',
    description: 'ECZANE ŞİFA',
    remainingAmount: '647.5000',
  ),
);

ReceiptDraft _invoiceDraft({String? categoryId}) => ReceiptDraft(
  documentKind: ReceiptDocumentKind.invoiceOrVoucher,
  counterpartyName: 'ENERJİSA',
  counterpartyState: ReceiptFieldState.read,
  purchasedAt: '2026-09-01',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: '2026-09-30',
  dueDateState: ReceiptFieldState.read,
  totalAmount: '412.6000',
  totalAmountState: ReceiptFieldState.read,
  feeAmount: null,
  feeAmountState: ReceiptFieldState.missing,
  installmentCount: null,
  currencyCode: 'TRY',
  paymentHint: ReceiptPaymentHint.unknown,
  categoryId: categoryId,
  categoryName: null,
  categoryState: categoryId == null
      ? ReceiptFieldState.missing
      : ReceiptFieldState.read,
  warnings: const [],
);
