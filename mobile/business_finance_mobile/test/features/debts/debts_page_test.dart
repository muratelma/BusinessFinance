import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_row_action.dart';
import 'package:business_finance_mobile/features/debts/data/debt_models.dart';
import 'package:business_finance_mobile/features/debts/data/debt_repository.dart';
import 'package:business_finance_mobile/features/activities/data/receipt_fee_writer.dart';
import 'package:business_finance_mobile/features/debts/presentation/debts_page.dart';
import 'package:business_finance_mobile/features/debts/presentation/lending_prefill.dart';

void main() {
  group('dekonttan borç verme', () {
    // Kişiye havale bir harcama olmayabilir: para geri beklenen bir alacaktır.
    // Gider yazılsaydı hem gider raporu şişer hem alacak hiç kaydedilmezdi.
    testWidgets('opens the receivable form with the slip already filled in', (
      tester,
    ) async {
      final repository = FakeDebtRepository();
      await _pumpWithPrefill(tester, repository);

      expect(find.text('Borç / alacak planı'), findsOneWidget);
      expect(find.widgetWithText(TextFormField, 'ERGÜN ÇETİN'), findsOneWidget);
      expect(find.text('Alınacak'), findsWidgets);
    });

    // Belgede yazan taşınır, yazmayan varsayılır ve görünür kalır: bir dekont
    // paranın ne zaman geri geleceğini söylemez.
    testWidgets('assumes one instalment, no interest, due in a month', (
      tester,
    ) async {
      final repository = FakeDebtRepository();
      await _pumpWithPrefill(tester, repository);

      await tester.tap(find.text('Oluştur'));
      await tester.pumpAndSettle();

      expect(repository.createCount, 1);
      final input = repository.lastCreateInput!;
      expect(input['direction'], 'receivable');
      expect(input['principal'], '1455.0000');
      // Faiz sıfır: uydurulmuş bir faiz, kullanıcının hiç konuşmadığı bir
      // maliyeti deftere yazardı.
      expect(input['totalRepayment'], '1455.0000');
      expect(input['annualInterestRate'], isNull);
      expect(input['installmentCount'], 1);
      expect(input['startDate'], '2022-11-22');
      expect(input['firstDueDate'], '2022-12-22');
    });

    // Ücret alacağa eklenmez: karşı taraf onu sana borçlanmadı, bankaya sen
    // ödedin. Anaparaya eklemek geri beklediğin tutarı şişirirdi.
    testWidgets('writes the fee from the opening account, not into the debt', (
      tester,
    ) async {
      final repository = FakeDebtRepository();
      final fees = <({String sourceId, String amount})>[];
      await _pumpWithPrefill(
        tester,
        repository,
        recordFee:
            ({
              required String sourceId,
              required String amount,
              required String date,
              required String description,
            }) async {
              fees.add((sourceId: sourceId, amount: amount));
              return const ReceiptFeeResult(
                ReceiptFeeStatus.recorded,
                'İşlem ücreti Diğer gider olarak kaydedildi.',
              );
            },
      );

      await tester.tap(find.text('Oluştur'));
      await tester.pumpAndSettle();

      expect(repository.lastCreateInput!['principal'], '1455.0000');
      expect(fees, hasLength(1));
      expect(fees.single.amount, '1.6300');
      expect(fees.single.sourceId, 'account-1');
    });

    // Kart ödeme yolunda öğrenilen ders: öneri her tazelemede yeniden
    // kullanılırsa form kendini yeniden açar ve ücret ikinci kez yazılır.
    testWidgets('uses the slip suggestion only once', (tester) async {
      final repository = FakeDebtRepository();
      var feeCalls = 0;
      await _pumpWithPrefill(
        tester,
        repository,
        recordFee:
            ({
              required String sourceId,
              required String amount,
              required String date,
              required String description,
            }) async {
              feeCalls++;
              return const ReceiptFeeResult(
                ReceiptFeeStatus.recorded,
                'kaydedildi',
              );
            },
      );

      await tester.tap(find.text('Oluştur'));
      await tester.pumpAndSettle();

      expect(repository.createCount, 1);
      expect(feeCalls, 1);
      // Form kendini yeniden açmadı.
      expect(find.text('Borç / alacak planı'), findsNothing);
    });

    // Eşleşme **söylenmezse** kullanıcı, dolu gelen adı kendi yazmış gibi
    // hızla geçer ve kayıt tanımadığı birinin açık bakiyesine eklenir.
    testWidgets('fişten gelen ad defterdeki kişiyle eşleştiğini söyler', (
      tester,
    ) async {
      final repository = FakeDebtRepository();
      await _pumpWithPrefill(
        tester,
        repository,
        matchedCounterpartyId: 'counterparty-1',
      );

      expect(
        find.textContaining('defterinizde kayıtlı'),
        findsOneWidget,
        reason: 'Var olan kişiye bağlanacağı ekranda yazmalı.',
      );
      expect(find.text('Bu kişi değil'), findsOneWidget);
    });

    // Eşleşme yoksa rozet de yok: o adla ilk kez iş yapılıyor olması olağandır
    // ve her fişte bir uyarı göstermek uyarıyı görünmez yapar.
    testWidgets('eşleşme yoksa rozet gösterilmez', (tester) async {
      final repository = FakeDebtRepository();
      await _pumpWithPrefill(tester, repository);

      expect(find.textContaining('defterinizde kayıtlı'), findsNothing);
      expect(find.text('Bu kişi değil'), findsNothing);
    });

    // Reddetme adı **siler**: aynı adı bırakmak sunucunun aynı karşı tarafı
    // yeniden bulmasıyla sonuçlanırdı, yani reddetme hiçbir şeyi değiştirmezdi.
    testWidgets('yanlış eşleşme tek dokunuşla reddedilir', (tester) async {
      final repository = FakeDebtRepository();
      await _pumpWithPrefill(
        tester,
        repository,
        matchedCounterpartyId: 'counterparty-1',
      );

      await tester.tap(find.text('Bu kişi değil'));
      await tester.pumpAndSettle();

      expect(find.textContaining('defterinizde kayıtlı'), findsNothing);
      expect(find.widgetWithText(TextFormField, 'ERGÜN ÇETİN'), findsNothing);

      // Reddedilen öneri geri gelmez: aynı ad yeniden yazılsa bile kullanıcı
      // bu kez kendi seçmiştir.
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Kişi / kurum'),
        'ERGÜN ÇETİN',
      );
      await tester.pumpAndSettle();
      expect(find.textContaining('defterinizde kayıtlı'), findsNothing);
    });

    // Ad değişince eşleşme hükümsüzdür: kullanıcı artık başka birinden söz
    // ediyor ve rozeti bırakmak olmayan bir bağı varmış gibi gösterirdi.
    testWidgets('ad değiştirilince rozet düşer', (tester) async {
      final repository = FakeDebtRepository();
      await _pumpWithPrefill(
        tester,
        repository,
        matchedCounterpartyId: 'counterparty-1',
      );

      await tester.enterText(
        find.widgetWithText(TextFormField, 'ERGÜN ÇETİN'),
        'ERGÜN ÇETİNKAYA',
      );
      await tester.pumpAndSettle();

      expect(find.textContaining('defterinizde kayıtlı'), findsNothing);
    });
  });

  testWidgets('debt form keeps invalid money visible instead of submitting', (
    tester,
  ) async {
    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Kişi / kurum'),
      'Sentetik alacak',
    );
    await tester.tap(find.text('Oluştur'));
    await tester.pump();

    expect(
      find.text('Sıfırdan büyük geçerli bir tutar girin.'),
      findsNWidgets(2),
    );
    expect(find.text('Borç / alacak planı'), findsOneWidget);
    expect(repository.createCount, 0);
  });

  // Paneller controller'larını `await showDialog` döner dönmez bırakıyordu.
  // O `await` `Navigator.pop` anında döner; kapanma animasyonu sürerken hâlâ
  // kare çizen `TextField` ölü controller'a dokunuyor ve uygulama kırılıyordu.
  // Aşağıdaki iki test panelin kapanışını gerçekten sonuna kadar sürer;
  // eski kodda ikisi de "A TextEditingController was used after being
  // disposed" ile düşüyordu.
  testWidgets('borç paneli vazgeçildikten sonra kırılmaz', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: FakeDebtRepository()),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Kişi / kurum'),
      'Sentetik',
    );

    await tester.tap(find.widgetWithText(TextButton, 'Vazgeç'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Borç / alacak planı'), findsNothing);
  });

  testWidgets('borç paneli kaydettikten sonra kırılmaz', (tester) async {
    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await _fillValidDebt(tester);

    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(repository.createCount, 1);
  });

  testWidgets('borç paneli seçilen tarihleri ve notu gönderir', (tester) async {
    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(
          repository: repository,
          // Tarihler artık bugüne sabitlenmiyor; seçilebildiğini kanıtlamak
          // için başlangıç günü ayın ilk gününe alınıyor.
        ),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await _fillValidDebt(tester);
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Not (isteğe bağlı)'),
      'Araba kredisi',
    );

    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    final input = repository.lastCreateInput!;
    expect(input['description'], 'Araba kredisi');
    // Alanlar artık istekte var; eski form ikisini de `today` sabitliyordu.
    expect(input['startDate'], isNotNull);
    expect(input['firstDueDate'], isNotNull);
    expect(input['installmentCount'], 12);
  });

  testWidgets('borç paneli kaynağı ve tek maliyet alanını gönderir', (
    tester,
  ) async {
    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await _fillValidDebt(tester);

    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    // Varsayılan kaynak nakittir ve hesap ister; kategori göndermez.
    final byTotal = repository.lastCreateInput!;
    expect(byTotal['sourceType'], 'cash');
    expect(byTotal['openingAccountId'], 'account-1');
    expect(byTotal['categoryId'], isNull);

    // Toplamı ve oranı birlikte göndermek, çelişirlerse isteğin reddedilmesi
    // demekti. Kullanıcının doldurduğu alan gider, diğeri boş kalır.
    expect(byTotal['totalRepayment'], isNotNull);
    expect(byTotal['annualInterestRate'], isNull);
  });

  testWidgets('gider kaynağı seçilince kategori sorulur, hesap sorulmaz', (
    tester,
  ) async {
    // Borç paneli varsayılan 800x600 test yüzeyine sığmıyor; alanlar
    // yerleşiyor ama görünür alanın dışında kalıyor. `enterText` bunu
    // umursamaz, `tap` ıskalar — yani daha yüksek bir yüzey olmadan test
    // menüyü hiç açmadan geçmeye çalışırdı.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await _fillValidDebt(tester);

    // Dokunma hedefi metin değil alanın kendisi; metne dokunmak sessizce
    // ıskalıyor ve test menüyü hiç açmadan devam ediyordu. Panel de
    // kaydırılabilir olduğu için önce görünür alana getiriliyor.
    const sourceField = Key('debt-source');
    await tester.ensureVisible(find.byKey(sourceField));
    await tester.pumpAndSettle();
    await tester.tap(find.byKey(sourceField));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Bir şey aldım / tükettim').last);
    await tester.pumpAndSettle();

    // Kaynak değişince alan da değişir: nakit hesap ister, gider kategori.
    expect(find.text('Paranın girdiği hesap'), findsNothing);
    expect(find.text('Gider kategorisi'), findsOneWidget);

    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    final input = repository.lastCreateInput!;
    expect(input['sourceType'], 'expense');
    expect(input['categoryId'], 'category-1');
    expect(input['openingAccountId'], isNull);
  });

  testWidgets('alacakta kategorili kaynak gelirdir, gider değil', (
    tester,
  ) async {
    // "Bir şey sattım, parasını taksitle alacağım." Bu kaynak eklenmeden önce
    // durum ancak nakit alacak olarak girilebiliyordu: uygulama hesaptan para
    // çıkmış gibi davranıyor, satış da hiç gelir olmuyordu.
    tester.view.physicalSize = const Size(1200, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await _fillValidDebt(tester);

    final directionField = find.byType(DropdownButtonFormField<String>).first;
    await tester.ensureVisible(directionField);
    await tester.pumpAndSettle();
    await tester.tap(directionField);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Alınacak').last);
    await tester.pumpAndSettle();

    // Yön değişince soru da değişir.
    expect(find.text('Alacağı ne doğurdu'), findsOneWidget);

    const sourceField = Key('debt-source');
    await tester.ensureVisible(find.byKey(sourceField));
    await tester.pumpAndSettle();
    await tester.tap(find.byKey(sourceField));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Bir şey sattım / hizmet verdim').last);
    await tester.pumpAndSettle();

    expect(find.text('Gelir kategorisi'), findsOneWidget);
    expect(find.text('Gider kategorisi'), findsNothing);

    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    final input = repository.lastCreateInput!;
    expect(input['direction'], 'receivable');
    expect(input['sourceType'], 'income');
    expect(input['categoryId'], 'income-1');
    expect(input['openingAccountId'], isNull);
  });

  // Kart tek bakışta üç şeyi söylüyor: kim, hangi yön, ne kadar kaldı. Faiz
  // ikinci sırada ve **yalnız varsa**: "Faiz 0,00 ₺ (%0)" satırı kartı doldurup
  // hiçbir şey söylemiyordu, kişiler arası borçlar da tipik olarak faizsiz.
  testWidgets('borç kartı yönü, kalanı ve faizi ayırır', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: FakeDebtRepository()),
      ),
    );
    await tester.pumpAndSettle();

    // Yön rozetle, tek kelimeyle ve ikonuyla.
    expect(find.text('Borç'), findsOneWidget);
    expect(find.text('Alacak'), findsOneWidget);
    expect(find.text('Kalan'), findsNWidgets(2));
    expect(find.textContaining('1 taksitin 0 tanesi ödendi'), findsNWidgets(2));
    // Faizi olan borçta bir kez; faizi sıfır olan alacakta hiç.
    expect(find.textContaining('Faiz'), findsOneWidget);
  });

  // Durum her satırda görünür ve eylem onun yanında, aynı boyda durur.
  testWidgets('taksit satırı durumu ve eylemi aynı yerde gösterir', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: FakeDebtRepository()),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Synthetic Lender'));
    await tester.pumpAndSettle();

    expect(find.text('1. taksit'), findsOneWidget);
    expect(find.text('Gecikmiş'), findsOneWidget);
    // Tarih ham ISO değil okunur biçimde.
    expect(find.text('11 Ağustos 2026'), findsOneWidget);
    expect(find.text('Öde'), findsOneWidget);
  });

  testWidgets('alacak tahsilatı ödeme değil tahsilat olarak bildirilir', (
    tester,
  ) async {
    // "Taksit ödendi" demek kullanıcıya para verdiğini söylemektir; oysa
    // alacakta para almıştır. Buton etiketi zaten ayrılıyordu, sonuç mesajı
    // ayrılmıyordu.
    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Eski Alacak'));
    await tester.pumpAndSettle();

    // Taksit satırları durum rozetiyle birlikte uzadı; buton test ekranının
    // altında kalabiliyor.
    await tester.ensureVisible(find.text('Tahsil et'));
    await tester.pumpAndSettle();
    // Çerçeveli: çerçevesiz hâlde rozetin yanındaki ikinci bir etiket gibi
    // okunuyor ve tıklanabilir olduğu fark edilmiyordu.
    expect(find.widgetWithText(AppRowAction, 'Tahsil et'), findsOneWidget);
    await tester.tap(find.text('Tahsil et'));
    await tester.pumpAndSettle();
    expect(find.text('Tahsilat hesabı'), findsOneWidget);

    // Hiçbir hesap önceden seçili değil: yanlış hesaptan tahsilat tek dokunuş
    // uzakta olmamalı.
    expect(
      tester
          .widget<FilledButton>(find.widgetWithText(FilledButton, 'Seç'))
          .onPressed,
      isNull,
    );
    await tester.tap(find.byType(DropdownButtonFormField<String>));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Cash').last);
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(FilledButton, 'Seç'));
    await tester.pumpAndSettle();

    expect(find.text('Taksit tahsil edildi.'), findsOneWidget);
    expect(find.text('Taksit ödendi.'), findsNothing);
  });

  testWidgets('açılışı kayıtsız borç sorulur ve tamamlanabilir', (
    tester,
  ) async {
    // Eksik gizlenmez: o borç hesaba ve raporlara hiç girmiyor ve bunu
    // yalnız kullanıcı düzeltebilir.
    final repository = FakeDebtRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: DebtsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Eski Alacak'));
    await tester.pumpAndSettle();
    expect(find.textContaining('açılışı kayıtlı değil'), findsOneWidget);

    await tester.tap(find.text('Tamamla'));
    await tester.pumpAndSettle();

    // Alacakta "borcu ne doğurdu" sorulmaz: alacak her zaman nakittir.
    expect(find.text('Borcu ne doğurdu'), findsNothing);
    expect(find.text('Paranın çıktığı hesap'), findsOneWidget);

    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(repository.openingDebtId, 'debt-legacy');
    expect(repository.lastOpeningInput!['sourceType'], 'cash');
    expect(repository.lastOpeningInput!['openingAccountId'], 'account-1');
  });
}

Future<void> _fillValidDebt(WidgetTester tester) async {
  await tester.enterText(
    find.widgetWithText(TextFormField, 'Kişi / kurum'),
    'Sentetik',
  );
  await tester.enterText(find.widgetWithText(TextFormField, 'Anapara'), '1000');
  await tester.enterText(
    find.widgetWithText(TextFormField, 'Toplam geri ödeme'),
    '1200',
  );
  await tester.enterText(
    find.widgetWithText(TextFormField, 'Taksit sayısı'),
    '12',
  );
}

class FakeDebtRepository implements DebtRepositoryContract {
  int loadCount = 0;
  int createCount = 0;
  Map<String, Object?>? lastCreateInput;
  String? openingDebtId;
  Map<String, Object?>? lastOpeningInput;
  String? paidDebtId;

  @override
  Future<DebtsSnapshot> load(String asOfDate) async {
    loadCount++;
    return const DebtsSnapshot(
      accounts: [DataChoice('account-1', 'Cash', type: 'bank')],
      categories: [
        DataChoice('category-1', 'Food', type: 'expense'),
        DataChoice('income-1', 'Satış geliri', type: 'income'),
      ],
      debts: [
        DebtItem(
          'debt-1',
          'Synthetic Lender',
          'payable',
          '400.0000',
          'cash',
          '20.0000',
          '100.0000',
          [DebtInstallmentItem(1, '400.0000', '2026-08-11', 'overdue')],
        ),
        DebtItem(
          'debt-legacy',
          'Eski Alacak',
          'receivable',
          '150.0000',
          'unrecorded',
          '0.0000',
          '0.0000',
          [DebtInstallmentItem(1, '150.0000', '2026-09-11', 'upcoming')],
        ),
      ],
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async {
    createCount++;
    lastCreateInput = input;
  }

  @override
  Future<void> recordOpening(String debtId, Map<String, Object?> input) async {
    openingDebtId = debtId;
    lastOpeningInput = input;
  }

  @override
  Future<void> pay(
    String debtId,
    int sequence,
    Map<String, Object?> input,
  ) async {
    paidDebtId = debtId;
  }
}

Future<void> _pumpWithPrefill(
  WidgetTester tester,
  FakeDebtRepository repository, {
  ReceiptFeeRecorder? recordFee,
  String? matchedCounterpartyId,
}) async {
  tester.view.physicalSize = const Size(1200, 2600);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: DebtsPage(
        repository: repository,
        recordFee: recordFee,
        lendingPrefill: LendingPrefill(
          counterpartyName: 'ERGÜN ÇETİN',
          matchedCounterpartyId: matchedCounterpartyId,
          amount: '1455.0000',
          date: '2022-11-22',
          feeAmount: '1.6300',
          feeDescription: 'İşlem ücreti — ERGÜN ÇETİN',
        ),
      ),
    ),
  );
  await tester.pumpAndSettle();
}
