import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'dart:async';
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_controller.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_form_page.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_launcher.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';

void main() {
  group('launcher', () {
    testWidgets('offers every way to record a movement', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(body: QuickAddLauncher()),
        ),
      );
      await tester.pumpAndSettle();

      for (final option in QuickAddOption.values) {
        expect(find.text(option.label), findsOneWidget);
      }
      // Setting something up is not recording money moving today, so account
      // and card creation, CSV import and plans stay on their own screens.
      expect(find.text('Hesap ekle'), findsNothing);
      expect(find.text('CSV içe aktar'), findsNothing);
    });

    // Menü ne okuyabildiğini olduğu gibi söylüyor. Tek bir "Fiş ile ekle"
    // satırı, fatura ve dekont da okunduğu hâlde kullanıcıya bunu tahmin
    // ettiriyordu; dekont ayrı bir satır çünkü ayrı bir soruya gidiyor.
    testWidgets('names the receipt and the bank slip as two doors', (
      tester,
    ) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(body: QuickAddLauncher()),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('Fiş veya fatura okut'), findsOneWidget);
      expect(find.text('Dekont okut'), findsOneWidget);
      expect(find.text('Fiş ile ekle'), findsNothing);
    });

    // Launcher artık pencere sınıfına göre bottom sheet veya dialog açıyor.
    // Genişlik verilmezse test ekranı 800 dp'dir, yani yalnız dialog yolu
    // denenir ve telefonun asıl yolu hiç geçilmez; iki kademe de sürülür.
    for (final window in [
      (name: 'telefonda bottom sheet olarak', width: 400.0),
      (name: 'geniş ekranda dialog olarak', width: 1000.0),
    ]) {
      testWidgets('${window.name} seçilen seçeneği çağırana döndürür', (
        tester,
      ) async {
        tester.view.physicalSize = Size(window.width, 1000);
        tester.view.devicePixelRatio = 1;
        addTearDown(tester.view.reset);

        QuickAddOption? chosen;
        await tester.pumpWidget(
          MaterialApp(
            theme: AppTheme.light(),
            home: Scaffold(
              body: Builder(
                builder: (context) => ElevatedButton(
                  onPressed: () async =>
                      chosen = await QuickAddLauncher.show(context),
                  child: const Text('aç'),
                ),
              ),
            ),
          ),
        );

        await tester.tap(find.text('aç'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Kredi kartı borcu öde'));
        await tester.pumpAndSettle();

        expect(chosen, QuickAddOption.cardPayment);
      });
    }
  });

  group('expense form', () {
    testWidgets('groups accounts and cards instead of mixing them', (
      tester,
    ) async {
      await _pumpForm(tester, isExpense: true);

      await tester.tap(find.text('Ödeme kaynağı'));
      await tester.pumpAndSettle();

      expect(find.text('Hesaplar'), findsWidgets);
      expect(find.text('Kredi kartları'), findsWidgets);
      expect(find.text('Banka'), findsWidgets);
      expect(find.textContaining('Test Kart'), findsWidgets);
    });

    testWidgets('leaves out an inactive card, which cannot take a charge', (
      tester,
    ) async {
      await _pumpForm(tester, isExpense: true);

      await tester.tap(find.text('Ödeme kaynağı'));
      await tester.pumpAndSettle();

      expect(find.textContaining('Kapalı Kart'), findsNothing);
    });

    testWidgets('refuses to submit until the required fields are filled', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      final finance = _FakeFinance();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        finance: finance,
      );

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(find.text('Ödeme kaynağı seçin.'), findsOneWidget);
      expect(find.text('Kategori seçin.'), findsOneWidget);
      expect(find.text('Geçerli bir tutar girin.'), findsOneWidget);
      expect(transactions.created, isEmpty);
      expect(finance.charges, isEmpty);
    });

    /// A card purchase is a card charge on the server, not a bank transaction.
    /// Sending it to the transactions endpoint would move the wrong balance and
    /// never touch the card debt.
    testWidgets('sends a card expense to the card charge endpoint', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      final finance = _FakeFinance();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        finance: finance,
      );

      await _chooseSource(tester, 'Test Kart');
      await _chooseCategory(tester, 'Market');
      await tester.enterText(find.byType(TextFormField).first, '625,50');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created, isEmpty);
      expect(finance.charges, hasLength(1));
      expect(finance.charges.single.$1, 'card-1');
      expect(finance.charges.single.$2['amount'], '625.50');
    });

    testWidgets('sends an account expense to the transactions endpoint', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      final finance = _FakeFinance();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        finance: finance,
      );

      await _chooseSource(tester, 'Banka');
      await _chooseCategory(tester, 'Market');
      await tester.enterText(find.byType(TextFormField).first, '40');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(finance.charges, isEmpty);
      expect(transactions.created, hasLength(1));
      expect(transactions.created.single.kind, TransactionKind.expense);
      expect(transactions.created.single.accountId, 'acc-1');
    });

    testWidgets('shows a failure and keeps the form open', (tester) async {
      final transactions = _FakeTransactions()
        ..createError = const ApiException(
          code: 'transactions.account_unavailable',
          message: 'Hesap kullanılamıyor.',
        );
      await _pumpForm(tester, isExpense: true, transactions: transactions);

      await _chooseSource(tester, 'Banka');
      await _chooseCategory(tester, 'Market');
      await tester.enterText(find.byType(TextFormField).first, '40');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(find.text('Hesap kullanılamıyor.'), findsOneWidget);
      expect(find.text('Kaydet'), findsOneWidget);
    });
  });

  group('income form', () {
    testWidgets('offers accounts only, because a card cannot receive income', (
      tester,
    ) async {
      await _pumpForm(tester, isExpense: false);

      expect(find.text('Hesap'), findsOneWidget);
      expect(find.text('Ödeme kaynağı'), findsNothing);

      await tester.tap(find.text('Hesap'));
      await tester.pumpAndSettle();
      expect(find.textContaining('Test Kart'), findsNothing);
    });

    testWidgets('records income as an income transaction', (tester) async {
      final transactions = _FakeTransactions();
      await _pumpForm(tester, isExpense: false, transactions: transactions);

      await tester.tap(find.text('Hesap'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Banka').last);
      await tester.pumpAndSettle();
      await _chooseCategory(tester, 'Maaş');
      await tester.enterText(find.byType(TextFormField).first, '18000');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created.single.kind, TransactionKind.income);
    });
  });

  group('controller', () {
    /// The server has no idempotency key, so the lock is the only thing keeping
    /// a double tap from writing the same expense twice.
    test('refuses a second submit while the first is still running', () async {
      final gate = Completer<void>();
      final transactions = _FakeTransactions()..createGate = gate;
      final controller = QuickAddController(transactions, _FakeFinance());

      final first = controller.submitIncome(
        accountId: 'acc-1',
        categoryId: 'cat-1',
        amount: '10',
        date: '2026-08-14',
      );
      final second = await controller.submitIncome(
        accountId: 'acc-1',
        categoryId: 'cat-1',
        amount: '10',
        date: '2026-08-14',
      );

      expect(second, isFalse);
      gate.complete();
      expect(await first, isTrue);
      expect(transactions.created, hasLength(1));
    });

    test('a successful submit tells the other screens to reread', () async {
      final changes = FinancialDataChanges();
      final controller = QuickAddController(
        _FakeTransactions(),
        _FakeFinance(),
        financialDataChanges: changes,
      );
      final before = changes.transactionsRevision;

      await controller.submitIncome(
        accountId: 'acc-1',
        categoryId: 'cat-1',
        amount: '10',
        date: '2026-08-14',
      );

      expect(changes.transactionsRevision, greaterThan(before));
    });

    test('an expired session is reported instead of an empty picker', () async {
      final finance = _FakeFinance()
        ..loadError = const ApiException(
          code: 'authentication.unauthorized',
          message: 'Oturum sona erdi',
          statusCode: 401,
        );
      final controller = QuickAddController(_FakeTransactions(), finance);

      await controller.loadExpenseOptions();

      expect(controller.unauthorized, isTrue);
      expect(controller.expenseOptions, isNull);
    });
  });

  group('fişten önerilen form', () {
    testWidgets('opens pre-filled and says the values are suggestions', (
      tester,
    ) async {
      await _pumpForm(tester, isExpense: true, prefill: _receiptPrefill());

      expect(find.text('847,5'), findsOneWidget);
      expect(find.text('TEST MARKET 01'), findsOneWidget);
      expect(find.text('2026-08-12'), findsOneWidget);
      // Kullanıcı formu kendi doldurmuş sanmamalı; her önerilen alan bunu
      // ayrıca söylüyor.
      expect(find.textContaining('Fişten okundu'), findsWidgets);
    });

    testWidgets('leaves the payment source empty and still requires it', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      final finance = _FakeFinance();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        finance: finance,
        prefill: _receiptPrefill(),
      );

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      // Fiş hangi karttan ödendiğini bilmez; kaynağı onun yerine seçmek,
      // onaylanacak tek şeyi kullanıcının yerine seçmek olurdu.
      expect(find.text('Ödeme kaynağı seçin.'), findsOneWidget);
      expect(transactions.created, isEmpty);
      expect(finance.charges, isEmpty);
    });

    testWidgets('sends what the user edited, not what was suggested', (
      tester,
    ) async {
      final finance = _FakeFinance();
      await _pumpForm(
        tester,
        isExpense: true,
        finance: finance,
        prefill: _receiptPrefill(),
      );

      await _chooseSource(tester, 'Test Kart');
      await tester.enterText(find.byType(TextFormField).first, '900,25');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(finance.charges.single.$2['amount'], '900.25');
      expect(finance.charges.single.$2['categoryId'], 'cat-1');
    });

    testWidgets('leaves an unread field empty instead of guessing', (
      tester,
    ) async {
      await _pumpForm(
        tester,
        isExpense: true,
        prefill: const QuickAddPrefill(
          amount: QuickAddSuggestion('847.5000', QuickAddSuggestionState.read),
        ),
      );

      // Tarih okunamadıysa bugüne düşer, uydurulmuş bir fiş tarihi yazılmaz.
      expect(find.text('2026-08-14'), findsOneWidget);
      expect(find.text('TEST MARKET 01'), findsNothing);
    });

    testWidgets('says which field is suspect and shows server warnings', (
      tester,
    ) async {
      await _pumpForm(
        tester,
        isExpense: true,
        prefill: const QuickAddPrefill(
          amount: QuickAddSuggestion(
            '847.5000',
            QuickAddSuggestionState.suspect,
          ),
          warnings: ['Ara toplam ve KDV, genel toplamı tutmuyor.'],
        ),
      );

      expect(find.textContaining('şüpheli'), findsOneWidget);
      expect(
        find.text('Ara toplam ve KDV, genel toplamı tutmuyor.'),
        findsOneWidget,
      );
    });

    testWidgets('a card hint reorders the list without hiding accounts', (
      tester,
    ) async {
      await _pumpForm(
        tester,
        isExpense: true,
        prefill: const QuickAddPrefill(
          sourceHint: PaymentSourceHint.creditCard,
        ),
      );

      expect(find.textContaining('Fişte kredi kartı yazıyor'), findsOneWidget);

      await tester.tap(find.text('Ödeme kaynağı'));
      await tester.pumpAndSettle();

      final cards = tester.getTopLeft(find.text('Kredi kartları').last);
      final accounts = tester.getTopLeft(find.text('Hesaplar').last);
      expect(cards.dy, lessThan(accounts.dy));
      // Fişte kart yazması, kullanıcının nakit ödemiş olmasını imkânsız
      // kılmaz; hesaplar listeden düşmüyor.
      expect(find.text('Banka').last, findsOneWidget);
    });

    testWidgets('a debit card hint points at the accounts, not the cards', (
      tester,
    ) async {
      await _pumpForm(
        tester,
        isExpense: true,
        prefill: const QuickAddPrefill(sourceHint: PaymentSourceHint.debitCard),
      );

      expect(find.textContaining('Fişte banka kartı yazıyor'), findsOneWidget);

      await tester.tap(find.text('Ödeme kaynağı'));
      await tester.pumpAndSettle();

      // Banka kartı bu uygulamada bir hesaptır; kart harcaması değildir.
      final accounts = tester.getTopLeft(find.text('Hesaplar').last);
      final cards = tester.getTopLeft(find.text('Kredi kartları').last);
      expect(accounts.dy, lessThan(cards.dy));
    });

    testWidgets('a card of unknown kind does not reorder anything', (
      tester,
    ) async {
      await _pumpForm(
        tester,
        isExpense: true,
        prefill: const QuickAddPrefill(sourceHint: PaymentSourceHint.card),
      );

      expect(find.textContaining('türü yazmıyor'), findsOneWidget);

      await tester.tap(find.text('Ödeme kaynağı'));
      await tester.pumpAndSettle();

      // Fiş türü söylemiyorsa ekran da söylemez: sıra varsayılan kalır,
      // kullanıcı okunmamış bir bilgiye doğru itilmez.
      final accounts = tester.getTopLeft(find.text('Hesaplar').last);
      final cards = tester.getTopLeft(find.text('Kredi kartları').last);
      expect(accounts.dy, lessThan(cards.dy));
    });

    testWidgets('tells the user when nothing could be read', (tester) async {
      await _pumpForm(
        tester,
        isExpense: true,
        prefill: const QuickAddPrefill(),
      );

      expect(find.textContaining('elle girebilirsiniz'), findsOneWidget);
    });
  });

  group('fişi sakla anahtarı', () {
    testWidgets('sends the original photo after the expense is written', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      final attachments = _FakeAttachments();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        attachments: attachments,
        prefill: _receiptPrefill(withPhoto: true),
      );

      await _chooseSource(tester, 'Banka');
      await tester.enterText(find.byType(TextFormField).first, '100');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created, hasLength(1));
      // Belge, yazılan kaydın kimliğine bağlanıyor ve orijinal byte'ları
      // taşıyor — analize giden küçültülmüş kopyayı değil.
      expect(attachments.uploads.single.$1, 't1');
      expect(attachments.uploads.single.$2.bytes, orderedEquals(const [9, 9]));
    });

    testWidgets('writes nothing anywhere when the switch is off', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      final attachments = _FakeAttachments();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        attachments: attachments,
        prefill: _receiptPrefill(withPhoto: true),
      );

      await _chooseSource(tester, 'Banka');
      await tester.enterText(find.byType(TextFormField).first, '100');
      await tester.tap(find.text('Fişi sakla'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created, hasLength(1));
      expect(attachments.uploads, isEmpty);
    });

    testWidgets('keeps the expense when the document cannot be attached', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      final attachments = _FakeAttachments()
        ..failure = const ApiException(
          code: 'attachments.storage_unavailable',
          message: 'Belge deposuna ulaşılamadı.',
          statusCode: 503,
        );
      // Form gerçekte bir rotanın üstünde açılıyor; uyarı da form kapandıktan
      // sonra, altta kalan ekranın üzerinde görünmeli. Formu tek başına kök
      // rotaya koymak bu davranışı hiç sınamıyordu.
      await _pushForm(
        tester,
        transactions: transactions,
        attachments: attachments,
        prefill: _receiptPrefill(withPhoto: true),
      );

      await _chooseSource(tester, 'Banka');
      await tester.enterText(find.byType(TextFormField).first, '100');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      // Fotoğraf uğruna finansal kayıt geri alınmıyor; kullanıcıya yalnız
      // belgenin eklenemediği söyleniyor.
      expect(transactions.created, hasLength(1));
      expect(find.text('Gider ekle'), findsNothing);
      expect(find.textContaining('fiş fotoğrafı eklenemedi'), findsOneWidget);
    });

    testWidgets('says why a card expense cannot keep the photo', (
      tester,
    ) async {
      final attachments = _FakeAttachments();
      final finance = _FakeFinance();
      await _pumpForm(
        tester,
        isExpense: true,
        finance: finance,
        attachments: attachments,
        prefill: _receiptPrefill(withPhoto: true),
      );

      await _chooseSource(tester, 'Test Kart');
      await tester.enterText(find.byType(TextFormField).first, '100');
      await tester.pumpAndSettle();

      // Sunucudaki belge uç noktası işleme bağlanıyor; kart harcamasının
      // böyle bir kimliği yok. Anahtar sessizce yok sayılmıyor, sebebi
      // yazıyor.
      expect(find.textContaining('Kart harcamasına'), findsOneWidget);

      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(finance.charges, hasLength(1));
      expect(attachments.uploads, isEmpty);
    });

    testWidgets('hides the switch when there is no photo to keep', (
      tester,
    ) async {
      await _pumpForm(tester, isExpense: true, prefill: _receiptPrefill());

      expect(find.text('Fişi sakla'), findsNothing);
    });
  });

  group('dekont işlem ücreti', () {
    // 4,50 veya 1,20 gibi bir tutar için ikinci bir form açmak — üstelik ödeme
    // kaynağını yeniden seçtirerek — kaydın kendisinden pahalı bir iş yüküydü.
    // Ücret artık ana kayıtla aynı gönderimde, aynı kaynaktan yazılıyor.
    testWidgets('is written from the same source without a second form', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        prefill: _receiptPrefill(
          autoFee: const QuickAddAutoFee(
            amount: '2.0000',
            description: 'İşlem ücreti — ENERJİSA',
          ),
        ),
      );
      await _chooseSource(tester, 'Banka');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created, hasLength(2));
      final fee = transactions.created.last;
      expect(fee.amount, '2.0000');
      // Kaynak sorulmadı: banka ücreti ana tutarın çıktığı hesaptan alır.
      expect(fee.accountId, transactions.created.first.accountId);
      expect(fee.categoryId, 'cat-fee');
      expect(fee.description, 'İşlem ücreti — ENERJİSA');
      // Ücret ana tutara **eklenmedi**; belgede yazmayan bir toplam uydurmak
      // olurdu.
      expect(transactions.created.first.amount, '847.5');
    });

    testWidgets('follows a card source instead of the account', (tester) async {
      final transactions = _FakeTransactions();
      final finance = _FakeFinance();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        finance: finance,
        prefill: _receiptPrefill(
          autoFee: const QuickAddAutoFee(
            amount: '2.0000',
            description: 'İşlem ücreti',
          ),
        ),
      );
      await _chooseSource(tester, 'Test Kart');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created, isEmpty);
      expect(finance.charges, hasLength(2));
      expect(finance.charges.last.$2['amount'], '2.0000');
      expect(finance.charges.last.$2['categoryId'], 'cat-fee');
    });

    // Ücret kaydedilemezse ana kayıt geri **alınmaz**: o kayıt doğru ve
    // yazıldı. Sessiz kalmak ise kullanıcıya yazılmamış bir gideri yazılmış
    // gibi gösterirdi.
    testWidgets('says so when the fee category is not in the list', (
      tester,
    ) async {
      final transactions = _FakeTransactions();
      await _pushForm(
        tester,
        transactions: transactions,
        finance: _FakeFinance(hasFeeCategory: false),
        prefill: _receiptPrefill(
          autoFee: const QuickAddAutoFee(
            amount: '2.0000',
            description: 'İşlem ücreti',
          ),
        ),
      );
      await _chooseSource(tester, 'Banka');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created, hasLength(1));
      expect(find.textContaining('Diğer gider'), findsWidgets);
    });

    // Form açılmadan yazılan bir kayıt görünmez bir kayıt olmamalı.
    testWidgets('tells the user before saving that it will be written', (
      tester,
    ) async {
      await _pumpForm(
        tester,
        isExpense: true,
        prefill: _receiptPrefill(
          autoFee: const QuickAddAutoFee(
            amount: '2.0000',
            description: 'İşlem ücreti',
          ),
        ),
      );

      expect(find.textContaining('işlem ücreti de'), findsOneWidget);
    });

    testWidgets('no fee means a single record', (tester) async {
      final transactions = _FakeTransactions();
      await _pumpForm(
        tester,
        isExpense: true,
        transactions: transactions,
        prefill: _receiptPrefill(),
      );
      await _chooseSource(tester, 'Banka');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();

      expect(transactions.created, hasLength(1));
    });
  });
}

QuickAddPrefill _receiptPrefill({
  bool withPhoto = false,
  QuickAddAutoFee? autoFee,
}) => QuickAddPrefill(
  autoFee: autoFee,
  amount: const QuickAddSuggestion('847.5000', QuickAddSuggestionState.read),
  date: const QuickAddSuggestion('2026-08-12', QuickAddSuggestionState.read),
  categoryId: const QuickAddSuggestion('cat-1', QuickAddSuggestionState.read),
  description: const QuickAddSuggestion(
    'TEST MARKET 01',
    QuickAddSuggestionState.read,
  ),
  sourceHint: PaymentSourceHint.creditCard,
  attachment: withPhoto
      ? QuickAddAttachment(
          bytes: Uint8List.fromList(const [9, 9]),
          fileName: 'IMG_1.jpg',
          mediaType: 'image/jpeg',
        )
      : null,
);

class _FakeAttachments {
  final List<(String, QuickAddAttachment)> uploads = [];
  ApiException? failure;

  Future<void> upload(
    String transactionId,
    QuickAddAttachment attachment,
  ) async {
    if (failure != null) throw failure!;
    uploads.add((transactionId, attachment));
  }
}

Future<void> _pumpForm(
  WidgetTester tester, {
  required bool isExpense,
  _FakeTransactions? transactions,
  _FakeFinance? finance,
  QuickAddPrefill? prefill,
  _FakeAttachments? attachments,
}) async {
  tester.view.physicalSize = const Size(500, 1200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: QuickAddFormPage(
        isExpense: isExpense,
        today: DateTime(2026, 8, 14),
        prefill: prefill,
        controller: QuickAddController(
          transactions ?? _FakeTransactions(),
          finance ?? _FakeFinance(),
          uploadAttachment: attachments?.upload,
        ),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

/// Formu bir rotanın üstünde açar.
///
/// [_pumpForm] formu kök rotaya koyar; kaydettikten sonra sayfa kapanınca
/// altında ekran kalmaz ve kapanışın ardından görünmesi gereken uyarılar
/// sınanamaz.
Future<void> _pushForm(
  WidgetTester tester, {
  _FakeTransactions? transactions,
  _FakeFinance? finance,
  _FakeAttachments? attachments,
  QuickAddPrefill? prefill,
}) async {
  tester.view.physicalSize = const Size(500, 1200);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(
        body: Builder(
          builder: (context) => TextButton(
            onPressed: () => Navigator.of(context).push(
              MaterialPageRoute<bool>(
                builder: (_) => QuickAddFormPage(
                  isExpense: true,
                  today: DateTime(2026, 8, 14),
                  prefill: prefill,
                  controller: QuickAddController(
                    transactions ?? _FakeTransactions(),
                    finance ?? _FakeFinance(),
                    uploadAttachment: attachments?.upload,
                  ),
                ),
              ),
            ),
            child: const Text('Formu aç'),
          ),
        ),
      ),
    ),
  );
  await tester.tap(find.text('Formu aç'));
  await tester.pumpAndSettle();
}

Future<void> _chooseSource(WidgetTester tester, String name) async {
  await tester.tap(find.text('Ödeme kaynağı'));
  await tester.pumpAndSettle();
  await tester.tap(find.textContaining(name).last);
  await tester.pumpAndSettle();
}

Future<void> _chooseCategory(WidgetTester tester, String name) async {
  await tester.tap(find.text('Kategori'));
  await tester.pumpAndSettle();
  await tester.tap(find.text(name).last);
  await tester.pumpAndSettle();
}

class _FakeTransactions implements TransactionRepositoryContract {
  final List<CreateTransactionInput> created = [];
  ApiException? createError;
  Completer<void>? createGate;

  @override
  Future<TransactionItem> create(CreateTransactionInput input) async {
    if (createGate != null) await createGate!.future;
    if (createError != null) throw createError!;
    created.add(input);
    return TransactionItem(
      id: 't1',
      accountId: input.accountId,
      categoryId: input.categoryId,
      amount: input.amount,
      currency: 'TRY',
      kind: input.kind,
      transactionDate: input.transactionDate,
      isCancelled: false,
    );
  }

  @override
  Future<List<TransactionChoice>> listAccounts() async => const [
    TransactionChoice(id: 'acc-1', name: 'Banka', isActive: true),
    TransactionChoice(id: 'acc-2', name: 'Kapalı Hesap', isActive: false),
  ];

  @override
  Future<List<TransactionChoice>> listCategories(TransactionKind kind) async =>
      const [TransactionChoice(id: 'cat-1', name: 'Maaş', isActive: true)];

  @override
  Future<TransactionItem> cancel(String id) => throw UnimplementedError();

  @override
  Future<TransactionPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    TransactionFilter filter = const TransactionFilter(),
  }) => throw UnimplementedError();
}

class _FakeFinance implements FinanceRepositoryContract {
  _FakeFinance({this.hasFeeCategory = true});

  final List<(String, Map<String, Object?>)> charges = [];
  ApiException? loadError;

  /// Kullanıcı `Diğer gider` kategorisini kapatmış olabilir; ücret o zaman
  /// uydurulmuş bir kovaya yazılmaz.
  final bool hasFeeCategory;

  @override
  Future<FinanceSnapshot> load({
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async {
    if (loadError != null) throw loadError!;
    return FinanceSnapshot(
      transfers: const [],
      cards: const [
        CreditCardItem(
          id: 'card-1',
          name: 'Test Kart',
          limit: '10000.0000',
          currentDebt: '0.0000',
          availableLimit: '10000.0000',
          currency: 'TRY',
          statementClosingDay: 15,
          paymentDueDay: 25,
          minimumPaymentRate: '20.0000',
          isActive: true,
        ),
        CreditCardItem(
          id: 'card-2',
          name: 'Kapalı Kart',
          limit: '5000.0000',
          currentDebt: '0.0000',
          availableLimit: '5000.0000',
          currency: 'TRY',
          statementClosingDay: 15,
          paymentDueDay: 25,
          minimumPaymentRate: '20.0000',
          isActive: false,
        ),
      ],
      plans: const [],
      accounts: const [FinanceChoice(id: 'acc-1', name: 'Banka')],
      expenseCategories: [
        const FinanceChoice(id: 'cat-1', name: 'Market'),
        if (hasFeeCategory)
          const FinanceChoice(id: 'cat-fee', name: 'Diğer gider'),
      ],
    );
  }

  @override
  Future<void> createCharge(String cardId, Map<String, Object?> input) async {
    charges.add((cardId, input));
  }

  @override
  Future<void> cancelTransfer(String transferId) => throw UnimplementedError();
  @override
  Future<void> createCard(Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> updateCard(String cardId, Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> createPayment(String cardId, Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> createPlan(Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<void> createTransfer(Map<String, Object?> input) =>
      throw UnimplementedError();
  @override
  Future<CardActivity> loadActivity(
    String cardId, {
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) => throw UnimplementedError();
  @override
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) => throw UnimplementedError();
  @override
  Future<CardStatement?> loadCurrentStatement(String cardId) =>
      throw UnimplementedError();
  @override
  Future<void> realizeInstallment(String planId, int sequence) =>
      throw UnimplementedError();
}
