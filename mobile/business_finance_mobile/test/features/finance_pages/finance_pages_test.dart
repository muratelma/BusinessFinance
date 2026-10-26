import 'dart:async';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_month_picker.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_detail_sheet.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/accounts_overview_page.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/finance_page_controller.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/finance_report_page.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/transfer_controller.dart';
import 'package:business_finance_mobile/features/finance_pages/presentation/transfer_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import '../../support/finance_page_fixture.dart';

Widget host(Widget child, {double scale = 1, bool dark = false}) => MaterialApp(
  theme: dark ? AppTheme.dark() : AppTheme.light(),
  builder: (context, widget) => MediaQuery(
    data: MediaQuery.of(context).copyWith(textScaler: TextScaler.linear(scale)),
    child: widget!,
  ),
  home: child,
);

void main() {
  for (final transfer in [false, true]) {
    testWidgets(
      '${transfer ? 'Transfer' : 'Hesaplar'} 2× yazıda 375 dp ve karanlık temada taşmaz',
      (tester) async {
        tester.view.physicalSize = const Size(375, 1000);
        tester.view.devicePixelRatio = 1;
        addTearDown(tester.view.reset);
        final repo = PageFixture();
        await tester.pumpWidget(
          host(
            transfer
                ? TransferPage(repository: repo)
                : AccountsOverviewPage(repository: repo),
            scale: 2,
            dark: true,
          ),
        );
        await tester.pumpAndSettle();
        expect(tester.takeException(), isNull);
        await tester.drag(find.byType(ListView).first, const Offset(0, -450));
        await tester.pumpAndSettle();
        expect(tester.takeException(), isNull);
        if (!transfer) expect(find.byType(TabBar), findsNothing);
      },
    );
  }
  test(
    'filtre değişince eski yanıt yeni ayı ezmez; disposed yanıt bildirmez',
    () async {
      final repo = PageFixture();
      final first = Completer<CategorySummaryData>();
      repo.summaryRead = (_) => first.future;
      final c = FinancePageController(
        repo,
        kind: FinancePageKind.summary,
        month: DateTime(2026, 10),
      );
      final older = c.load();
      repo.summaryRead = (m) async => repo.selectedSummary(m, false);
      await c.chooseMonth(DateTime(2026, 9));
      first.complete(repo.selectedSummary(DateTime(2026, 10), false));
      await older;
      expect(c.summary!.month, 9);
      final pending = Completer<CategorySummaryData>();
      repo.summaryRead = (_) => pending.future;
      final after = c.load();
      c.dispose();
      pending.complete(repo.summaryData);
      await after;
    },
  );
  test('kategori kaynak ve kişi filtreleri birlikte sunucuya gider', () async {
    final repo = PageFixture();
    final c = FinancePageController(
      repo,
      kind: FinancePageKind.category,
      id: 'goods',
    );
    await c.chooseSource('account:bank');
    await c.choosePerson('person:ali');
    await c.months(2026);
    expect(repo.calls, contains('category:account:bank:person:ali:0'));
    expect(repo.calls, contains('months:account:bank:person:ali:2026'));
    c.dispose();
  });
  test(
    'yenileme hatasında veri bayat; 401 sonrasında eski tutarlar temizlenir',
    () async {
      final repo = PageFixture();
      final c = FinancePageController(repo, kind: FinancePageKind.summary);
      await c.load();
      repo.error = const ApiException(
        statusCode: 503,
        code: 'network_error',
        message: 'Bağlantı kurulamadı.',
      );
      await c.load();
      expect(c.isStale, true);
      expect(c.summary!.total, '26930.0000');
      repo.error = const ApiException(
        statusCode: 401,
        code: 'unauthorized',
        message: 'Oturum sona erdi.',
      );
      await c.load();
      expect(c.unauthorized, true);
      expect(c.hasData, false);
      c.dispose();
    },
  );
  test(
    'transfer masrafı bütçeleri de yeniler; masrafsız transfer yenilemez',
    () {
      final changes = FinancialDataChanges();
      final old = changes.budgetsRevision;
      changes.transferChanged();
      expect(changes.budgetsRevision, old);
      changes.transferChanged(fee: true);
      expect(changes.budgetsRevision, old + 1);
      changes.dispose();
    },
  );
  for (final kind in [
    FinancePageKind.summary,
    FinancePageKind.category,
    FinancePageKind.account,
    FinancePageKind.transit,
  ]) {
    for (final dark in [false, true]) {
      testWidgets(
        '$kind ${dark ? 'karanlık' : 'aydınlık'} 2× metin ve 375 dp taşmaz',
        (tester) async {
          tester.view.physicalSize = const Size(375, 1000);
          tester.view.devicePixelRatio = 1;
          addTearDown(tester.view.reset);
          final repo = PageFixture();
          await tester.pumpWidget(
            host(
              FinanceReportPage(
                repository: repo,
                kind: kind,
                id: 'bank',
                month: DateTime(2026, 10),
              ),
              scale: 2,
              dark: dark,
            ),
          );
          await tester.pumpAndSettle();
          expect(tester.takeException(), isNull);
          await tester.drag(find.byType(ListView).first, const Offset(0, -450));
          await tester.pumpAndSettle();
          expect(tester.takeException(), isNull);
        },
      );
    }
  }
  testWidgets('kategori özeti sunucu toplamını ve payını gösterir', (
    tester,
  ) async {
    await tester.pumpWidget(
      host(
        FinanceReportPage(
          repository: PageFixture(),
          kind: FinancePageKind.summary,
          month: DateTime(2026, 10),
        ),
      ),
    );
    await tester.pumpAndSettle();
    expect(find.text('₺26.930,00'), findsOneWidget);
    expect(find.text('%42,44'), findsOneWidget);
    expect(find.text('Pay %42,44'), findsNothing);
    // Yüzde sunucudandır; yön renkle değil okla söylenir.
    expect(find.text('Geçen aydan %8 fazla'), findsOneWidget);
    expect(find.byIcon(Icons.arrow_upward), findsOneWidget);
    expect(find.bySemanticsLabel('Son altı ay'), findsOneWidget);
    await tester.tap(find.text('Hesap / kart'));
    await tester.pumpAndSettle();
    expect(find.text('Nakit ve banka'), findsOneWidget);
    await tester.tap(find.text('Ziraat vadesiz'));
    await tester.pumpAndSettle();
    expect(find.text('Ziraat vadesiz'), findsOneWidget);
  });
  for (final (change, previous, text, icon) in [
    (null, '0.0000', 'Geçen ay kayıt yok', null),
    ('0.0000', '26930.0000', 'Geçen ayla aynı', null),
    ('-40.0000', '44883.3333', 'Geçen aydan %40 az', Icons.arrow_downward),
  ]) {
    testWidgets('kategori özetinin karşılaştırma satırı: $text', (
      tester,
    ) async {
      final repo = PageFixture();
      final base = repo.summaryData;
      repo.summaryData = CategorySummaryData(
        year: base.year,
        month: base.month,
        direction: base.direction,
        total: base.total,
        previousTotal: previous,
        previousChange: change,
        recentMonths: base.recentMonths,
        categoryCount: base.categoryCount,
        categories: base.categories,
        sources: base.sources,
      );
      await tester.pumpWidget(
        host(
          FinanceReportPage(
            repository: repo,
            kind: FinancePageKind.summary,
            month: DateTime(2026, 10),
          ),
        ),
      );
      await tester.pumpAndSettle();
      expect(find.text(text), findsOneWidget);
      expect(find.byIcon(Icons.arrow_upward), findsNothing);
      expect(
        find.byIcon(Icons.arrow_downward),
        icon == null ? findsNothing : findsOneWidget,
      );
    });
  }
  testWidgets(
    'hesap menüsü Tümü durumunda grupları seçer; Çıkan durumunda ara başlıklar seçilmez',
    (tester) async {
      final repo = PageFixture();
      await tester.pumpWidget(
        host(
          FinanceReportPage(
            repository: repo,
            kind: FinancePageKind.account,
            id: 'bank',
            month: DateTime(2026, 10),
          ),
        ),
      );
      await tester.pumpAndSettle();
      await tester.tap(find.text('Tür'));
      await tester.pumpAndSettle();
      final transferOption = find.ancestor(
        of: find.text('Transfer'),
        matching: find.byType(MenuItemButton),
      );
      expect(transferOption, findsOneWidget);
      await tester.tap(transferOption);
      await tester.pumpAndSettle();
      expect(repo.calls.last, 'account:null:out:transfer:0');
      await tester.tap(find.text('Çıkan').first);
      await tester.pumpAndSettle();
      expect(repo.calls.last, 'account:out:null:0');
      await tester.tap(find.text('Tür'));
      await tester.pumpAndSettle();
      expect(find.text('Ödeme'), findsOneWidget);
      expect(find.text('Kart ödemesi'), findsOneWidget);
      expect(tester.takeException(), isNull);
    },
  );
  testWidgets(
    'hesap satırının büyük tutarı masraflı çıkış; ayrıntının başlığı masrafsızdır',
    (tester) async {
      final repo = PageFixture();
      await tester.pumpWidget(
        host(
          FinanceReportPage(
            repository: repo,
            kind: FinancePageKind.account,
            id: 'bank',
            month: DateTime(2026, 10),
          ),
        ),
      );
      await tester.pumpAndSettle();
      await tester.drag(find.byType(ListView).first, const Offset(0, -400));
      await tester.pumpAndSettle();
      expect(find.text('-₺10.050,00'), findsOneWidget);
      expect(find.text('₺50,00 masraf dahil'), findsOneWidget);
      await tester.pumpWidget(
        host(
          Scaffold(
            body: ActivityDetailSheet(
              activity: pageActivity(),
              parts: repo.parts(pageActivity()),
            ),
          ),
        ),
      );
      await tester.pumpAndSettle();
      expect(find.text('₺10.000,00'), findsOneWidget);
      expect(find.text('Masrafın kategorisi'), findsOneWidget);
      expect(find.text('Bankanın aldığı ücretler'), findsOneWidget);
      expect(find.text('Çıkan hesap'), findsOneWidget);
      expect(find.text('Giren hesap'), findsOneWidget);
      expect(
        find.textContaining('Masraf gider olarak yazılır'),
        findsOneWidget,
      );
      expect(find.text('Gelir ya da gider sayılmaz.'), findsNothing);
    },
  );
  testWidgets('kartlı satışın parası günüyle ayrı durum satırında okunur', (
    tester,
  ) async {
    FinancePageActivity sale(
      String id,
      String state, {
      String? expected,
      String? transferred,
    }) => FinancePageActivity(
      activity: pageActivity(
        id: id,
        kind: 'pos-sale',
        amount: '8400.0000',
        effect: 'income',
        fee: '326.0000',
        expected: expected,
        transferred: transferred,
      ),
      amount: '8400.0000',
      state: state,
    );
    final repo = PageFixture()
      ..rows = [
        sale('pos-transit', 'transit', expected: '2026-10-26'),
        sale('pos-deposited', 'deposited', transferred: '2026-10-15'),
      ];
    await tester.pumpWidget(
      host(
        FinanceReportPage(
          repository: repo,
          kind: FinancePageKind.category,
          id: 'goods',
          month: DateTime(2026, 10),
        ),
      ),
    );
    await tester.pumpAndSettle();
    expect(find.text('Yolda · 26 Eki bekleniyor'), findsOneWidget);
    expect(find.text('Hesaba geçti · 15 Eki'), findsOneWidget);
    expect(find.textContaining('Ziraat vadesiz · Yolda'), findsNothing);
    expect(tester.takeException(), isNull);
  });
  for (final source in [null, 'account:bank']) {
    testWidgets(
      'kategori kartı tek alt satır taşır: ${source == null ? 'geçen ayla fark' : 'seçimde kategorinin tamamı'}',
      (tester) async {
        await tester.pumpWidget(
          host(
            FinanceReportPage(
              repository: PageFixture(),
              kind: FinancePageKind.category,
              id: 'goods',
              month: DateTime(2026, 10),
              source: source,
            ),
          ),
        );
        await tester.pumpAndSettle();
        expect(
          find.text('Geçen aydan %8 fazla'),
          source == null ? findsOneWidget : findsNothing,
        );
        expect(
          find.byIcon(Icons.arrow_upward),
          source == null ? findsOneWidget : findsNothing,
        );
        expect(
          find.text('Kategorinin tamamı ₺11.430,00'),
          source == null ? findsNothing : findsOneWidget,
        );
        expect(find.textContaining('Geçen ay ₺'), findsNothing);
        expect(find.bySemanticsLabel('Son altı ay'), findsOneWidget);
      },
    );
  }
  testWidgets('kategorisiz liste salt okunur; kategori atama akışı yok', (
    tester,
  ) async {
    await tester.pumpWidget(
      host(
        FinanceReportPage(
          repository: PageFixture(),
          kind: FinancePageKind.category,
          month: DateTime(2026, 10),
        ),
      ),
    );
    await tester.pumpAndSettle();
    expect(find.text('Kategorisi yok'), findsOneWidget);
    expect(find.textContaining('Kategori seçilmeli'), findsNothing);
    expect(find.text('Kategori seç'), findsNothing);
  });
  testWidgets('pasif bakiye toplamda dilim, listede kendi grubudur', (
    tester,
  ) async {
    await tester.pumpWidget(
      host(AccountsOverviewPage(repository: PageFixture())),
    );
    await tester.pumpAndSettle();
    // Yüzdeler sunucunun payıdır; istemci hesaplamaz.
    expect(find.text('%26'), findsOneWidget);
    expect(find.text('%74'), findsOneWidget);
    // `Pasif` toplam kartında dilim, listede hesabın etiketidir.
    expect(find.text('Pasif'), findsWidgets);
    expect(find.text('₺150,00'), findsWidgets);
    expect(find.byTooltip('Transfer yap'), findsOneWidget);
    await tester.drag(find.byType(ListView).first, const Offset(0, -500));
    await tester.pumpAndSettle();
    expect(find.text('Pasif · 1'), findsOneWidget);
    expect(find.text('Eski hesap'), findsOneWidget);
    expect(find.textContaining('Toplama girmez'), findsNothing);
  });
  testWidgets('ay seçici ileri boş ayı kapatır; kaydı olan ileri ay açılır', (
    tester,
  ) async {
    final repo = PageFixture();
    Future<void> open(WidgetTester t) async {
      await t.pumpWidget(
        host(
          Scaffold(
            body: Builder(
              builder: (context) => TextButton(
                onPressed: () => AppMonthPicker.showWithAmounts(
                  context: context,
                  initialMonth: DateTime(2026, 10),
                  load: (year) => repo.categoryMonths(year),
                  caption: 'Aylık gider',
                ),
                child: const Text('Aç'),
              ),
            ),
          ),
        ),
      );
      await t.tap(find.text('Aç'));
      await t.pumpAndSettle();
    }

    await open(tester);
    expect(
      tester
          .widget<OutlinedButton>(
            find.ancestor(
              of: find.text('Kas'),
              matching: find.byType(OutlinedButton),
            ),
          )
          .onPressed,
      isNull,
    );
    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();
    repo.futureEnabled = true;
    await open(tester);
    expect(
      tester
          .widget<OutlinedButton>(
            find.ancestor(
              of: find.text('Kas'),
              matching: find.byType(OutlinedButton),
            ),
          )
          .onPressed,
      isNotNull,
    );
  });
  testWidgets('transfer formu dört ondalığı ve masrafı tek istekte taşır', (
    tester,
  ) async {
    final repo = PageFixture();
    await tester.pumpWidget(host(TransferPage(repository: repo)));
    await tester.pumpAndSettle();
    await tester.tap(find.byType(DropdownButtonFormField<String>).first);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Ziraat vadesiz').last);
    await tester.pumpAndSettle();
    await tester.tap(find.byType(DropdownButtonFormField<String>).last);
    await tester.pumpAndSettle();
    await tester.tap(find.text('Kasa').last);
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).at(0), '10.000,1234');
    await tester.enterText(find.byType(TextFormField).at(1), '50,0001');
    await tester.pump(const Duration(milliseconds: 300));
    await tester.pumpAndSettle();
    await tester.scrollUntilVisible(
      find.text('Kaydet'),
      200,
      scrollable: find.byType(Scrollable).last,
    );
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();
    expect(repo.inputs.single.amount, '10000.1234');
    expect(repo.inputs.single.feeAmount, '50.0001');
    expect(repo.inputs.single.feeScope, TransactionScope.business);
  });
  testWidgets('transfer önizlemesi olmadan ve kapsam eksikken kaydetmez', (
    tester,
  ) async {
    final repo = PageFixture()..needsScope = true;
    final c = TransferController(repo);
    c.setInput(
      const TransferInput(
        sourceAccountId: 'bank',
        destinationAccountId: 'cash',
        amount: '100.0000',
        date: '2026-10-13',
        feeAmount: '1.0000',
      ),
    );
    expect(await c.save(), false);
    await tester.pump(const Duration(milliseconds: 300));
    expect(await c.save(), false);
    expect(repo.inputs, isEmpty);
    c.dispose();
  });
  testWidgets('para yönü nötr hesap satırında da işaretle okunur', (
    tester,
  ) async {
    await tester.pumpWidget(
      host(
        const Scaffold(
          body: Column(
            children: [
              AppMoneyText(
                amount: '-10050.0000',
                currency: 'TRY',
                effect: AppMoneyEffect.neutral,
                signedByAmount: true,
              ),
              AppMoneyText(
                amount: '100.0000',
                currency: 'TRY',
                effect: AppMoneyEffect.neutral,
                signedByAmount: true,
              ),
            ],
          ),
        ),
      ),
    );
    expect(find.text('-₺10.050,00'), findsOneWidget);
    expect(find.text('+₺100,00'), findsOneWidget);
  });

  testWidgets(
    'transfer kaydı sürerken kapsam değişikliği yazılan isteği ve yenileme sinyalini bozmaz',
    (tester) async {
      final wait = Completer<void>();
      final repo = PageFixture()..saveWait = wait;
      final changes = FinancialDataChanges();
      final c = TransferController(repo, changes: changes);
      const original = TransferInput(
        sourceAccountId: 'bank',
        destinationAccountId: 'cash',
        amount: '100.0000',
        date: '2026-10-13',
        feeAmount: '1.0000',
        feeScope: TransactionScope.business,
      );
      c.setInput(original);
      await tester.pump(const Duration(milliseconds: 300));
      final saving = c.save();
      c.setInput(null);
      expect(c.input, same(original));
      expect(c.preview, isNotNull);
      expect(await c.save(), false);
      wait.complete();
      expect(await saving, true);
      expect(repo.inputs, [original]);
      expect(changes.budgetsRevision, 1);
      c.dispose();
      changes.dispose();
    },
  );

  testWidgets(
    'faizli nötr ödemenin ayrıntısı faiz giderini rapordan dışlanmış göstermez',
    (tester) async {
      await tester.pumpWidget(
        host(
          Scaffold(
            body: ActivityDetailSheet(
              activity: pageActivity(
                kind: 'debt-payment',
                amount: '1000.0000',
                interest: '150.0000',
                fee: null,
              ),
            ),
          ),
        ),
      );
      await tester.pumpAndSettle();
      await tester.drag(find.byType(Scrollable).first, const Offset(0, -500));
      await tester.pumpAndSettle();
      expect(find.textContaining('Faiz gider olarak yazılır'), findsOneWidget);
      expect(find.text('Gelir ya da gider sayılmaz.'), findsNothing);
      expect(tester.takeException(), isNull);
    },
  );
}
