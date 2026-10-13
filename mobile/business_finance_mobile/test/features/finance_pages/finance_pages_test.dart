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
    expect(find.text('Pay %42,44'), findsOneWidget);
    await tester.tap(find.text('Hesap / kart'));
    await tester.pumpAndSettle();
    expect(find.text('Nakit ve banka'), findsOneWidget);
    await tester.tap(find.text('Ziraat vadesiz'));
    await tester.pumpAndSettle();
    expect(find.text('Ziraat vadesiz'), findsOneWidget);
  });
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
      expect(find.text('Transfer'), findsOneWidget);
      await tester.tap(find.text('Transfer'));
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
    },
  );
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
  testWidgets('pasif bakiye toplam kartının üçüncü hücresindedir', (
    tester,
  ) async {
    await tester.pumpWidget(
      host(AccountsOverviewPage(repository: PageFixture())),
    );
    await tester.pumpAndSettle();
    expect(find.text('Pasif'), findsOneWidget);
    expect(find.text('₺150,00'), findsNWidgets(2));
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
      find.text('Transferi kaydet'),
      200,
      scrollable: find.byType(Scrollable).last,
    );
    await tester.tap(find.text('Transferi kaydet'));
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
}
