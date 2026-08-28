import 'package:business_finance_mobile/core/models/tax_fields.dart';
import 'dart:async';
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_models.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_image_source.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_models.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_photo.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_preferences.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_repository.dart';
import 'package:business_finance_mobile/features/receipts/presentation/receipt_prefill.dart';
import 'package:business_finance_mobile/features/receipts/presentation/receipt_scan_controller.dart';
import 'package:business_finance_mobile/features/receipts/presentation/receipt_scan_page.dart';

void main() {
  testWidgets('offers camera and gallery as two equal entries', (tester) async {
    await _pump(tester, _FakeSource(), _FakeRepository());

    expect(find.text('Fotoğraf çek'), findsOneWidget);
    expect(find.text('Galeriden seç'), findsOneWidget);
  });

  testWidgets('names what is being waited for, and lets it be cancelled', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    await _pump(tester, source, repository);

    await tester.tap(find.text('Fotoğraf çek'));
    await tester.pump();
    expect(find.text('Fotoğraf seçiliyor'), findsOneWidget);

    source.complete(_picked());
    await tester.pump();
    expect(find.textContaining('Belge okunuyor'), findsOneWidget);

    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();

    expect(find.textContaining('iptal edildi'), findsOneWidget);
    expect(find.text('Fotoğraf çek'), findsOneWidget);
  });

  // Yön belgeden okunamaz — aynı kira makbuzu iki taraf için ters yönlüdür —
  // bu yüzden fotoğraf çekilmeden önce kullanıcıya sorulur.
  testWidgets(
    'asks for the direction before the photo and defaults to expense',
    (tester) async {
      final source = _FakeSource();
      final repository = _FakeRepository();
      await _pump(tester, source, repository);

      expect(find.text('Harcama'), findsOneWidget);
      expect(find.text('Gelir'), findsOneWidget);

      await tester.tap(find.text('Galeriden seç'));
      await tester.pump();
      source.complete(_picked());
      // pumpAndSettle değil: okuma bilerek tamamlanmıyor, göstergesi dönüyor.
      await tester.pump();
      await tester.pump();

      expect(repository.lastIntent, ReceiptCaptureIntent.expense);
    },
  );

  testWidgets('reads with the direction the user picked and carries it out', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    ReceiptCaptureIntent? handedIntent;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (_, intent) => handedIntent = intent,
    );

    await tester.tap(find.text('Gelir'));
    await tester.pump();
    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(_draft());
    await tester.pumpAndSettle();

    expect(repository.lastIntent, ReceiptCaptureIntent.income);
    // Yön forma da taşınıyor: aynı taslak gelir formuna gitmeli.
    expect(handedIntent, ReceiptCaptureIntent.income);
  });

  // Dekont doğrudan bir forma gitmiyor: ana tutarın ne olduğu belgeden
  // okunamaz — ödeme mi, aktarma mı, borç verme mi — ve önce sorulmalı.
  testWidgets('sends a bank slip to the decision page, not to a form', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    ReceiptDraft? handed;
    QuickAddPrefill? handedExpense;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handedExpense = prefill,
      onBankDocumentReady: (draft) => handed = draft,
      variant: ReceiptScanVariant.bankSlip,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(
      _draft(fee: '4.5000', kind: ReceiptDocumentKind.bankDocument),
    );
    await tester.pumpAndSettle();

    expect(repository.lastIntent, ReceiptCaptureIntent.bankSlip);
    expect(handed, isNotNull);
    expect(handed!.feeAmount, '4.5000');
    expect(handedExpense, isNull);
  });

  // Dekontun kendi sayfası var, çünkü orada sorulan şey yön değil belge sınıfı.
  // Bir dekontun ne olduğu fotoğraf çekilmeden bilinemez ve gerçek dekontların
  // çoğu üçüncü tarafa ödemedir — eski akışta yalnız Harcama seçeneğinden
  // geçebiliyorlardı.
  testWidgets('the bank-slip page reads every kind of slip', (tester) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    ReceiptDraft? handed;
    await _pump(
      tester,
      source,
      repository,
      onBankDocumentReady: (draft) => handed = draft,
      variant: ReceiptScanVariant.bankSlip,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(
      _draft(fee: '1.6300', kind: ReceiptDocumentKind.bankPayment),
    );
    await tester.pumpAndSettle();

    expect(repository.lastIntent, ReceiptCaptureIntent.bankSlip);
    // Yön burada sorulmadı; karar sayfasında sorulacak.
    expect(handed, isNotNull);
    expect(handed!.feeAmount, '1.6300');
  });

  // Dekont sayfasında yön hiç sorulmuyor. Sorulsaydı kullanıcı belgede ne
  // yazdığını görmeden tahmin eder, sonra karar sayfası aynı soruyu tekrar
  // sorup cevabı ezerdi.
  testWidgets('the bank-slip page asks no direction of its own', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeSource(),
      _FakeRepository(),
      variant: ReceiptScanVariant.bankSlip,
    );

    expect(find.text('Dekont'), findsOneWidget); // yalnız başlıkta
    expect(find.byType(SegmentedButton<ReceiptCaptureIntent>), findsNothing);
    expect(find.text('Harcama'), findsNothing);
    expect(find.text('Gelir'), findsNothing);
    expect(find.text('Fotoğraf çek'), findsOneWidget);
  });

  // Fiş sayfası yalnız yönü sorar. Dekont ve transfer buraya geri konsaydı,
  // kullanıcı `İşlem ekle` menüsünde verdiği cevabı ikinci kez verirdi.
  testWidgets('the receipt page asks only the direction', (tester) async {
    await _pump(tester, _FakeSource(), _FakeRepository());

    expect(find.text('Harcama'), findsOneWidget);
    expect(find.text('Gelir'), findsOneWidget);
    expect(find.text('Dekont'), findsNothing);
    expect(find.text('Transfer'), findsNothing);
  });

  // İade yeni bir kayıt üretmez; geri verdiği harcamayı iptal eder. Gider
  // formuna göndermek, geri gelen parayı harcanmış göstermek olurdu.
  testWidgets('sends a refund slip to the refund page, not to a form', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    ReceiptDraft? handedRefund;
    QuickAddPrefill? handedExpense;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handedExpense = prefill,
      onRefundReady: (draft) => handedRefund = draft,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(_draft(kind: ReceiptDocumentKind.refundReceipt));
    await tester.pumpAndSettle();

    expect(handedRefund, isNotNull);
    expect(handedExpense, isNull);
  });

  // Son ödeme tarihi taşıyan belge ödendiğini söylemez; doğrudan gider formuna
  // göndermek, henüz çıkmamış parayı çıkmış göstermek olurdu.
  testWidgets('asks about an invoice before writing an expense', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    ReceiptDraft? handedInvoice;
    QuickAddPrefill? handedExpense;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handedExpense = prefill,
      onInvoiceReady: (draft) => handedInvoice = draft,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(
      _draft(kind: ReceiptDocumentKind.invoiceOrVoucher, dueDate: '2026-09-30'),
    );
    await tester.pumpAndSettle();

    expect(handedInvoice, isNotNull);
    expect(handedInvoice!.dueDate, '2026-09-30');
    expect(handedExpense, isNull);
  });

  // Vadesi olmayan bir fiş soru sormadan forma gider: en sık yol bir adım bile
  // uzamamalı.
  testWidgets('a till receipt still goes straight to the form', (tester) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    var invoiceQuestions = 0;
    QuickAddPrefill? handedExpense;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handedExpense = prefill,
      onInvoiceReady: (_) => invoiceQuestions++,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(_draft());
    await tester.pumpAndSettle();

    expect(invoiceQuestions, 0);
    expect(handedExpense, isNotNull);
  });

  // Taksitli satış tek seferlik tam tutar gideri değildir: 3.000 TL / 3
  // taksitlik bir fişi tek gider yazmak o ayın bütçesini gerçekte çıkmayan
  // 2.000 TL kadar şişirir.
  testWidgets('sends an installment sale to the plan path, not to an expense', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    ReceiptDraft? handedPlan;
    QuickAddPrefill? handedExpense;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handedExpense = prefill,
      onInstallmentReady: (draft) => handedPlan = draft,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(_draft(installments: 3));
    await tester.pumpAndSettle();

    expect(handedPlan, isNotNull);
    expect(handedPlan!.installmentCount, 3);
    expect(handedExpense, isNull);
  });

  // Tek çekim fiş plan yolu görmez; en sık yol bir adım bile uzamamalı.
  testWidgets('a single-payment receipt still goes to the form', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    var planOffers = 0;
    QuickAddPrefill? handedExpense;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handedExpense = prefill,
      onInstallmentReady: (_) => planOffers++,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(_draft());
    await tester.pumpAndSettle();

    expect(planOffers, 0);
    expect(handedExpense, isNotNull);
  });

  testWidgets('hands the read draft to the form as suggestions', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    QuickAddPrefill? handed;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handed = prefill,
    );

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(_draft());
    await tester.pumpAndSettle();

    expect(handed, isNotNull);
    expect(handed!.amount?.value, '847.5000');
    expect(handed!.description?.value, 'TEST MARKET 01');
    // Fişte yalnız "kart" yazıyor; bu bir ipucu, seçim değil — ve türü
    // yazmadığı için kredi kartına da çevrilmiyor.
    expect(handed!.sourceHint, PaymentSourceHint.card);
  });

  testWidgets('a provider failure still leads to the manual form', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    var handedCount = 0;
    QuickAddPrefill? handed;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) {
        handedCount++;
        handed = prefill;
      },
    );

    await tester.tap(find.text('Fotoğraf çek'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.fail(
      const ApiException(
        statusCode: 503,
        code: 'receipt.provider_unavailable',
        message: 'Fiş okuma servisine şu an ulaşılamıyor.',
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('ulaşılamıyor'), findsOneWidget);
    // Okuma çalışmadığında harcama kaydedilemez duruma düşmüyor.
    await tester.tap(find.text('Bilgileri elle gir'));
    await tester.pumpAndSettle();

    expect(handedCount, 1);
    expect(handed, isNull);
  });

  testWidgets('a closed picker is reported without an error tone', (
    tester,
  ) async {
    final source = _FakeSource();
    await _pump(tester, source, _FakeRepository());

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(null);
    await tester.pumpAndSettle();

    expect(find.textContaining('Fotoğraf seçilmedi'), findsOneWidget);
    expect(find.byType(TextButton), findsWidgets);
  });

  testWidgets('an expired session is shown as such, not as a read failure', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    await _pump(tester, source, repository);

    await tester.tap(find.text('Fotoğraf çek'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.fail(
      const ApiException(
        statusCode: 401,
        code: 'authentication.required',
        message: 'Oturumunuz sona erdi.',
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Fotoğraf çek'), findsNothing);
    expect(find.textContaining('Oturum'), findsWidgets);
  });

  group('ücret çevirisi', () {
    // 4,50 veya 1,20 gibi bir tutar için ikinci bir form açmak — üstelik ödeme
    // kaynağını yeniden seçtirerek — kaydın kendisinden pahalı bir iş yüküydü.
    test('rides along with the main record instead of opening a form', () {
      final fee = receiptAutoFeeFrom(_draft(fee: '2.0000'))!;

      expect(fee.amount, '2.0000');
      expect(fee.description, contains('TEST MARKET 01'));
      expect(fee.description, startsWith('İşlem ücreti'));
    });

    // Her dekontta ücret olmaz. Olmayan bir ücret için ekranda satır
    // göstermek, kullanıcıya olmayan bir kararı sordurmak olurdu.
    test('no fee on the slip produces no second record', () {
      expect(_draft().hasFee, isFalse);
      expect(receiptAutoFeeFrom(_draft()), isNull);
    });

    test('an unread fee counts as no fee', () {
      final draft = _draft(fee: '2.0000', feeState: ReceiptFieldState.missing);

      expect(draft.hasFee, isFalse);
      expect(receiptAutoFeeFrom(draft), isNull);
      expect(receiptTransferPrefillFrom(draft).feeAmount, isNull);
      expect(receiptCardPaymentPrefillFrom(draft).feeAmount, isNull);
    });

    test('a zero fee is another way of writing no fee', () {
      final draft = _draft(fee: '0.0000');

      expect(draft.hasFee, isFalse);
      expect(receiptAutoFeeFrom(draft), isNull);
    });

    test('the main prefill carries the fee only when asked', () {
      final draft = _draft(fee: '2.0000');

      expect(receiptPrefillFrom(draft).autoFee, isNull);
      expect(
        receiptPrefillFrom(draft, autoFee: receiptAutoFeeFrom(draft)).autoFee,
        isNotNull,
      );
    });

    test('an installment sale carries the total, not a divided amount', () {
      final draft = _draft(installments: 3);
      final prefill = receiptInstallmentPrefillFrom(draft);

      // Bölmeyi sunucu yapar; istemci finansal toplamı ikinci kez hesaplamaz.
      expect(prefill.totalAmount, '847.5000');
      expect(prefill.installmentCount, 3);
      expect(prefill.isEmpty, isFalse);
      // Kart taşınmıyor: fiş hangi karttan ödendiğini söylemez.
    });

    test('a single payment produces no plan suggestion', () {
      expect(receiptInstallmentPrefillFrom(_draft()).isEmpty, isTrue);
      expect(_draft().isInstallmentSale, isFalse);
      expect(_draft(installments: 3).isInstallmentSale, isTrue);
    });

    // Kart ödemesi yolunda ad boş kalıyordu. Görünürde küçük bir eksik ama
    // sonucu büyük: aynı dekontun ikinci kez okutulduğunu söyleyen uyarı tarih,
    // tutar **ve** adın üçüne birden bakıyor — ad yoksa o yolda hiç çalışmıyor.
    test('the card payment carries the counterparty like the others', () {
      expect(
        receiptCardPaymentPrefillFrom(_draft()).description,
        contains('TEST MARKET 01'),
      );
      expect(
        receiptCardPaymentPrefillFrom(
          _draft(counterpartyMissing: true),
        ).description,
        isNull,
      );
    });

    test('the transfer and card paths can drop the fee', () {
      final draft = _draft(fee: '2.0000');

      expect(receiptTransferPrefillFrom(draft).feeAmount, '2.0000');
      expect(
        receiptTransferPrefillFrom(draft, withFee: false).feeAmount,
        isNull,
      );
      expect(receiptCardPaymentPrefillFrom(draft).feeAmount, '2.0000');
      expect(
        receiptCardPaymentPrefillFrom(draft, withFee: false).feeAmount,
        isNull,
      );
    });
  });

  group('taslak çevirisi', () {
    test('drops a field the server could not read', () {
      final prefill = receiptPrefillFrom(_draft(counterpartyMissing: true));

      expect(prefill.description, isNull);
      expect(prefill.amount, isNotNull);
    });

    test('carries the suspect state instead of flattening it', () {
      final prefill = receiptPrefillFrom(_draft(amountSuspect: true));

      expect(prefill.amount!.isSuspect, isTrue);
      expect(prefill.amount!.helperText, contains('şüpheli'));
    });

    // 27 Ağustos 2026 kabul turu: belgede "KDV %20 46,67" yazarken form
    // "KDV girilmedi" ile açılıyordu. Okunabilen bir bilgiyi kullanıcıya
    // yeniden yazdırmak, muhasebeci paketini elle doldurtmaktı.
    test('belgede yazan KDV forma taşınır', () {
      final prefill = receiptPrefillFrom(
        _draft(
          vat: const VatFields(rate: '0.2000', amount: '46.6700'),
        ),
      );

      expect(prefill.vat!.rate, '0.2000');
      expect(prefill.vat!.amount, '46.6700');
      // Alan yüzde soruyor; sözleşme kesir taşıyor. Birim değişimi bir vergi
      // hesabı değildir.
      expect(prefill.vat!.ratePercentInput, '20');
    });

    test('KDV okunmadıysa öneri de yok', () {
      expect(receiptPrefillFrom(_draft()).vat, isNull);
    });

    // ADR 0016'nın kapısı: oran ile tutar bağımsızdır. Biri boş geldiğinde
    // istemci diğerinden **türetmez** — türetseydi uygulama KDV hesaplayan
    // bir şey olurdu.
    test('yalnız tutar okunduysa oran boş kalır, hesaplanmaz', () {
      final prefill = receiptPrefillFrom(
        _draft(vat: const VatFields(amount: '46.6700')),
      );

      expect(prefill.vat!.amount, '46.6700');
      expect(prefill.vat!.rate, isNull);
    });

    test('an unknown payment hint chooses nothing', () {
      final prefill = receiptPrefillFrom(
        _draft(hint: ReceiptPaymentHint.unknown),
      );

      expect(prefill.sourceHint, isNull);
    });

    // Kredi kartı ayrı bir yazma modeli, banka kartı ise bir hesaptır. İkisi
    // tek "kart" ipucunda birleştiğinde banka kartı fişi kredi kartına
    // yönlendiriliyordu; ayrım burada korunuyor.
    test('keeps credit and debit apart instead of collapsing them', () {
      expect(
        receiptPrefillFrom(
          _draft(hint: ReceiptPaymentHint.creditCard),
        ).sourceHint,
        PaymentSourceHint.creditCard,
      );
      expect(
        receiptPrefillFrom(
          _draft(hint: ReceiptPaymentHint.debitCard),
        ).sourceHint,
        PaymentSourceHint.debitCard,
      );
      expect(
        receiptPrefillFrom(_draft(hint: ReceiptPaymentHint.cash)).sourceHint,
        PaymentSourceHint.cash,
      );
      // Türü yazmayan fiş krediye yuvarlanmaz.
      expect(
        receiptPrefillFrom(_draft(hint: ReceiptPaymentHint.card)).sourceHint,
        PaymentSourceHint.card,
      );
    });
  });

  testWidgets('asks before the first reading and takes no photo until then', (
    tester,
  ) async {
    final source = _FakeSource();
    final preferences = _FakePreferences(consent: false);
    await _pump(tester, source, _FakeRepository(), preferences: preferences);

    // Kamera düğmesi bile görünmüyor: soru, fotoğraf çekilmeden önce sorulur.
    expect(find.textContaining('Google Gemini'), findsOneWidget);
    expect(find.text('Fotoğraf çek'), findsNothing);

    await tester.tap(find.text('Kabul ediyorum, belgeyi okut'));
    await tester.pumpAndSettle();

    expect(preferences.consent, isTrue);
    expect(find.text('Fotoğraf çek'), findsOneWidget);
  });

  testWidgets('declining consent still leaves manual entry open', (
    tester,
  ) async {
    var handedCount = 0;
    final preferences = _FakePreferences(consent: false);
    await _pump(
      tester,
      _FakeSource(),
      _FakeRepository(),
      preferences: preferences,
      onDraftReady: (_, _) => handedCount++,
    );

    await tester.tap(find.text('Bilgileri elle gir'));
    await tester.pumpAndSettle();

    expect(handedCount, 1);
    expect(preferences.consent, isFalse);
  });

  testWidgets('hands the original photo along for the keep-photo switch', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    QuickAddPrefill? handed;
    await _pump(
      tester,
      source,
      repository,
      onDraftReady: (prefill, _) => handed = prefill,
      preferences: _FakePreferences(keepPhoto: false),
    );

    await tester.tap(find.text('Fotoğraf çek'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.complete(_draft());
    await tester.pumpAndSettle();

    // Saklanacak olan orijinal byte'lar, analize giden kopya değil.
    expect(handed!.attachment, isNotNull);
    expect(handed!.attachment!.bytes, orderedEquals(const [1, 2, 3]));
    // Anahtarın cihazda hatırlanan durumu forma taşınıyor.
    expect(handed!.keepAttachmentByDefault, isFalse);
  });

  // Dekont yanlış seçenekle okutulduğunda bu bir okuma hatası değil, bir seçim
  // hatası: fotoğraf duruyor ve düzeltmesi tek dokunuş olmalı.
  testWidgets('offers to re-read a misfiled slip as a bank slip', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    await _pump(tester, source, repository);

    await tester.tap(find.text('Fotoğraf çek'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.fail(
      const ApiException(
        statusCode: 400,
        code: 'receipt.bank_payment_not_transfer',
        message:
            'Bu dekont başkasına yapılan bir ödeme, hesaplarınız arasında '
            'aktarma değil.',
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('aktarma değil'), findsOneWidget);
    expect(find.text('Dekont olarak okut'), findsOneWidget);
    // Aynı fotoğrafı aynı seçenekle yollamak aynı cevabı getirir.
    expect(find.text('Yeniden dene'), findsNothing);

    await tester.tap(find.text('Dekont olarak okut'));
    await tester.pump();
    repository.complete(_draft(kind: ReceiptDocumentKind.bankPayment));
    await tester.pumpAndSettle();

    // Fotoğraf yeniden çekilmedi; yalnız seçenek değişti.
    expect(repository.analyzeCount, 2);
    expect(repository.lastIntent, ReceiptCaptureIntent.bankSlip);
  });

  testWidgets('an unrelated photo gets one clear sentence, not a wall', (
    tester,
  ) async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    await _pump(tester, source, repository);

    await tester.tap(find.text('Galeriden seç'));
    await tester.pump();
    source.complete(_picked());
    await tester.pump();
    repository.fail(
      const ApiException(
        statusCode: 400,
        code: 'receipt.not_a_receipt',
        message: 'Bu fotoğrafta alışveriş fişi görünmüyor.',
      ),
    );
    await tester.pumpAndSettle();

    expect(find.textContaining('alışveriş fişi görünmüyor'), findsOneWidget);
    expect(find.text('Bilgileri elle gir'), findsOneWidget);
    expect(find.text('Transfer olarak kaydet'), findsNothing);
  });
}

Future<void> _pump(
  WidgetTester tester,
  _FakeSource source,
  _FakeRepository repository, {
  void Function(QuickAddPrefill?, ReceiptCaptureIntent)? onDraftReady,
  void Function(ReceiptDraft)? onBankDocumentReady,
  void Function(ReceiptDraft)? onRefundReady,
  void Function(ReceiptDraft)? onInvoiceReady,
  void Function(ReceiptDraft)? onInstallmentReady,
  _FakePreferences? preferences,
  ReceiptScanVariant variant = ReceiptScanVariant.receipt,
}) async {
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: ReceiptScanPage(
        variant: variant,
        controller: ReceiptScanController(
          source,
          repository,
          _PassThroughNormalizer(),
          preferences ?? _FakePreferences(),
          intent: variant.initialIntent,
        ),
        onDraftReady: onDraftReady ?? (_, _) {},
        onBankDocumentReady: onBankDocumentReady,
        onRefundReady: onRefundReady,
        onInvoiceReady: onInvoiceReady,
        onInstallmentReady: onInstallmentReady,
      ),
    ),
  );
  await tester.pumpAndSettle();
}

PickedReceiptImage _picked() => PickedReceiptImage(
  bytes: Uint8List.fromList(const [1, 2, 3]),
  fileName: 'fis.jpg',
  mediaType: 'image/jpeg',
);

ReceiptDraft _draft({
  String? dueDate,
  bool counterpartyMissing = false,
  bool amountSuspect = false,
  ReceiptPaymentHint hint = ReceiptPaymentHint.card,
  String? fee,
  ReceiptFieldState? feeState,
  int? installments,
  ReceiptDocumentKind kind = ReceiptDocumentKind.purchaseReceipt,
  VatFields? vat,
}) => ReceiptDraft(
  installmentCount: installments,
  documentKind: kind,
  counterpartyName: counterpartyMissing ? null : 'TEST MARKET 01',
  counterpartyState: counterpartyMissing
      ? ReceiptFieldState.missing
      : ReceiptFieldState.read,
  purchasedAt: '2026-08-12',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: dueDate,
  dueDateState: dueDate == null
      ? ReceiptFieldState.missing
      : ReceiptFieldState.read,
  totalAmount: '847.5000',
  totalAmountState: amountSuspect
      ? ReceiptFieldState.suspect
      : ReceiptFieldState.read,
  vat: vat,
  vatState: vat == null ? ReceiptFieldState.missing : ReceiptFieldState.read,
  feeAmount: fee,
  feeAmountState:
      feeState ??
      (fee == null ? ReceiptFieldState.missing : ReceiptFieldState.read),
  currencyCode: 'TRY',
  paymentHint: hint,
  categoryId: 'cat-1',
  categoryName: 'Market',
  categoryState: ReceiptFieldState.read,
  warnings: const [],
);

class _FakeSource implements ReceiptImageSourceContract {
  Completer<PickedReceiptImage?>? _pending;

  @override
  Future<PickedReceiptImage?> pick(ReceiptImageOrigin origin) {
    final completer = Completer<PickedReceiptImage?>();
    _pending = completer;
    return completer.future;
  }

  void complete(PickedReceiptImage? image) => _pending!.complete(image);
}

class _FakeRepository implements ReceiptRepositoryContract {
  Completer<ReceiptDraft>? _pending;
  ReceiptCaptureIntent? lastIntent;
  int analyzeCount = 0;

  @override
  Future<ReceiptDraft> analyze(
    ReceiptPhoto photo,
    ReceiptCaptureIntent intent,
  ) {
    analyzeCount++;
    lastIntent = intent;
    final completer = Completer<ReceiptDraft>();
    _pending = completer;
    return completer.future;
  }

  void complete(ReceiptDraft draft) => _pending!.complete(draft);

  void fail(Object error) => _pending!.completeError(error);
}

class _PassThroughNormalizer implements ReceiptImageNormalizerContract {
  @override
  Future<ReceiptPhoto> normalize({
    required Uint8List bytes,
    required String fileName,
    required String mediaType,
  }) async => ReceiptPhoto(
    originalBytes: bytes,
    originalFileName: fileName,
    originalMediaType: mediaType,
    uploadBytes: bytes,
    uploadFileName: fileName,
    uploadWidth: 100,
    uploadHeight: 200,
  );
}

class _FakePreferences implements ReceiptPreferencesContract {
  _FakePreferences({this.consent = true, this.keepPhoto = true});

  bool consent;
  bool keepPhoto;

  @override
  Future<bool> readConsent() async => consent;

  @override
  Future<void> writeConsent(bool granted) async => consent = granted;

  @override
  Future<bool> readKeepPhoto() async => keepPhoto;

  @override
  Future<void> writeKeepPhoto(bool keep) async => keepPhoto = keep;
}
