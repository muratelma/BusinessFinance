import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/accounts/data/account_models.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_form_page.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/cards/data/card_page_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_models.dart';
import 'package:business_finance_mobile/features/cards/data/finance_repository.dart';
import 'package:business_finance_mobile/features/cards/presentation/finance_page.dart';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'screenshot_harness.dart';

/// Kredi kartları: gerçek sayfa, örnek veriyle (Claude Design, kart taksit
/// ve limit ile kart hareketleri teslimleri). Kareler teslimdeki çerçeveyle
/// yan yana bakılsın diye aynı adlarla üretilir.
void main() {
  Future<void> shot(
    WidgetTester tester,
    String name, {
    CardsDesignRepository? repository,
    Future<void> Function(WidgetTester tester)? before,
  }) async {
    final scope = ScopeController(readHasBusiness: () async => true);
    await tester.runAsync(scope.ensureLoaded);
    await captureScreen(
      tester,
      name,
      FinancePage(repository: repository ?? CardsDesignRepository()),
      withNavBar: false,
      pushed: true,
      wrapApp: (app) => ChangeNotifierProvider<ScopeController?>.value(
        value: scope,
        child: app,
      ),
      before: before,
    );
  }

  Future<void> scrollTo(WidgetTester tester, Finder finder) async {
    await tester.scrollUntilVisible(
      finder,
      200,
      scrollable: find.byType(Scrollable).first,
    );
    await tester.pumpAndSettle();
  }

  Future<void> openMovements(WidgetTester tester) async {
    await scrollTo(tester, find.text('Hareketler'));
    await tester.tap(find.text('Tümü').last);
    await tester.pumpAndSettle();
  }

  testWidgets('01 kart + limit', (tester) async {
    await shot(tester, 'kart-01-ust');
  }, skip: !screenshotsEnabled);

  testWidgets('02 limit dolmak üzere', (tester) async {
    await shot(
      tester,
      'kart-02-dolmak-uzere',
      repository: CardsDesignRepository(low: true),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('03 dönem borcu gecikti', (tester) async {
    await shot(
      tester,
      'kart-03-donem-borcu-gecikti',
      repository: CardsDesignRepository(dueNow: true),
      before: (tester) async {
        await tester.drag(find.byType(ListView).first, const Offset(0, -300));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('04 taksitler ve hareketler bölümü', (tester) async {
    await shot(
      tester,
      'kart-04-taksit-ve-hareketler',
      repository: CardsDesignRepository(planCount: 3),
      before: (tester) async {
        await tester.drag(find.byType(ListView).first, const Offset(0, -560));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('05 tek taksit', (tester) async {
    await shot(
      tester,
      'kart-05-tek-taksit',
      before: (tester) async {
        await tester.drag(find.byType(ListView).first, const Offset(0, -560));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('06 taksitler sayfası', (tester) async {
    await shot(
      tester,
      'kart-06-taksitler-sayfasi',
      repository: CardsDesignRepository(planCount: 3),
      before: (tester) async {
        await scrollTo(tester, find.text('Taksitler'));
        await tester.tap(find.text('Tümü').first);
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('07 taksit ayrıntısı', (tester) async {
    await shot(
      tester,
      'kart-07-taksit-detayi',
      repository: CardsDesignRepository(dueNow: true),
      before: (tester) async {
        await scrollTo(tester, find.text('Soğutucu dolap').first);
        await tester.tap(find.text('Soğutucu dolap').first);
        await tester.pumpAndSettle();
        await tester.tap(find.text('Soğutucu dolap').first);
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('08 hareketler sayfası', (tester) async {
    await shot(tester, 'kart-08-hareketler', before: openMovements);
  }, skip: !screenshotsEnabled);

  testWidgets('09 filtre paneli', (tester) async {
    await shot(
      tester,
      'kart-09-filtre',
      before: (tester) async {
        await openMovements(tester);
        await tester.tap(find.byTooltip('Filtrele'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('10 filtre kategori menüsü', (tester) async {
    await shot(
      tester,
      'kart-10-filtre-kategori',
      before: (tester) async {
        await openMovements(tester);
        await tester.tap(find.byTooltip('Filtrele'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Bütün kategoriler'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('11 filtre tür ödeme', (tester) async {
    await shot(
      tester,
      'kart-11-filtre-odeme',
      before: (tester) async {
        await openMovements(tester);
        await tester.tap(find.byTooltip('Filtrele'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Ödeme').last);
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('12 filtre uygulanmış', (tester) async {
    await shot(
      tester,
      'kart-12-filtre-uygulanmis',
      before: (tester) async {
        await openMovements(tester);
        await tester.tap(find.byTooltip('Filtrele'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Harcama').last);
        await tester.pumpAndSettle();
        await tester.tap(find.text('Sonuçları göster'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('13 tarih aralığı ekstreye göre', (tester) async {
    await shot(
      tester,
      'kart-13-tarih-araligi',
      before: (tester) async {
        await openMovements(tester);
        await tester.tap(find.byTooltip('Filtrele'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Tarih aralığı seç…'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  // İki ekstreye yayılan özel aralık: başlangıç bir ekstrede, ok, bitiş
  // sonrakinde.
  testWidgets('13 tarih aralığı özel', (tester) async {
    await shot(
      tester,
      'kart-13b-tarih-araligi-ozel',
      before: (tester) async {
        await openMovements(tester);
        await tester.tap(find.byTooltip('Filtrele'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Tarih aralığı seç…'));
        await tester.pumpAndSettle();
        await tester.tap(find.byTooltip('Önceki ekstre'));
        await tester.pumpAndSettle();
        await tester.tap(find.bySemanticsLabel('10 Ekim 2026'));
        await tester.pumpAndSettle();
        await tester.tap(find.byTooltip('Sonraki ekstre'));
        await tester.pumpAndSettle();
        await tester.tap(find.bySemanticsLabel('10 Kasım 2026'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('14 kartı düzenle', (tester) async {
    await shot(
      tester,
      'kart-14-karti-duzenle',
      before: (tester) async {
        await tester.tap(find.text('Düzenle'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('15 kartı düzenle · kapsam açık', (tester) async {
    await shot(
      tester,
      'kart-15-kapsam-acik',
      before: (tester) async {
        await tester.tap(find.text('Düzenle'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Varsayılan kapsam'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('16 taksitli harcama', (tester) async {
    await shot(
      tester,
      'kart-16-taksitli-harcama',
      before: (tester) async {
        await tester.tap(find.text('Taksitli harcama').hitTestable().first);
        await tester.pumpAndSettle();
        await tester.enterText(
          find.widgetWithText(TextFormField, 'Toplam tutar'),
          '6500',
        );
        await tester.pump(const Duration(milliseconds: 400));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('17 taksit sayısı menüsü', (tester) async {
    await shot(
      tester,
      'kart-17-taksit-sayisi',
      before: (tester) async {
        await tester.tap(find.text('Taksitli harcama').hitTestable().first);
        await tester.pumpAndSettle();
        await tester.enterText(
          find.widgetWithText(TextFormField, 'Toplam tutar'),
          '6500',
        );
        await tester.pump(const Duration(milliseconds: 400));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Taksit sayısı'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);

  for (final open in [null, 'type', 'scope']) {
    testWidgets('18 hesap ekle ${open ?? 'kapalı'}', (tester) async {
      final scope = ScopeController(readHasBusiness: () async => true);
      await tester.runAsync(scope.ensureLoaded);
      await captureScreen(
        tester,
        'kart-18-hesap-ekle-${open ?? 'kapali'}',
        AccountFormPage(onSave: _noSave),
        withNavBar: false,
        pushed: true,
        wrapApp: (app) => ChangeNotifierProvider<ScopeController?>.value(
          value: scope,
          child: app,
        ),
        before: (tester) async {
          if (open == 'type') await tester.tap(find.text('Nakit'));
          if (open == 'scope') {
            await tester.tap(find.text('Varsayılan kapsam'));
          }
          await tester.pumpAndSettle();
        },
      );
    }, skip: !screenshotsEnabled);
  }

  testWidgets('19 boş', (tester) async {
    await shot(
      tester,
      'kart-19-bos',
      repository: CardsDesignRepository(empty: true),
    );
  }, skip: !screenshotsEnabled);
}

Future<bool> _noSave({
  Account? account,
  required String name,
  required String type,
  required String openingBalance,
  required bool isActive,
  TransactionScope? defaultScope,
}) async => true;

class CardsDesignRepository extends Fake implements FinanceRepositoryContract {
  CardsDesignRepository({
    this.empty = false,
    this.dueNow = false,
    this.low = false,
    this.planCount = 1,
  });
  final bool empty;
  final bool dueNow;
  final bool low;
  final int planCount;

  CreditCardItem get _business => CreditCardItem(
    id: 'isletme',
    name: 'İşletme kartı',
    limit: '50000.0000',
    currentDebt: low ? '46100.0000' : '6532.2200',
    availableLimit: low ? '3900.0000' : '27467.7800',
    currency: 'TRY',
    statementClosingDay: 15,
    paymentDueDay: 25,
    minimumPaymentRate: '20.0000',
    isActive: true,
  );

  static const _personal = CreditCardItem(
    id: 'sahsi',
    name: 'Şahsi kart',
    limit: '30000.0000',
    currentDebt: '4200.0000',
    availableLimit: '25800.0000',
    currency: 'TRY',
    statementClosingDay: 3,
    paymentDueDay: 13,
    minimumPaymentRate: '20.0000',
    isActive: true,
  );

  static const _plans = [
    ('Soğutucu dolap', '6500.0000', 9, 1, '722.2222'),
    ('Kasa ve terazi', '10000.0000', 4, 0, '2500.0000'),
    ('Telefon', '3600.0000', 3, 2, '1200.0000'),
  ];

  @override
  Future<FinanceSnapshot> load({
    HistoryPeriod period = HistoryPeriod.threeMonths,
  }) async => FinanceSnapshot(
    transfers: const [],
    cards: empty ? const [] : [_business, _personal],
    plans: empty
        ? const []
        : [
            for (final (index, plan) in _plans.take(planCount).indexed)
              InstallmentPlanModel(
                id: 'p$index',
                creditCardId: 'isletme',
                totalAmount: plan.$2,
                currency: 'TRY',
                description: plan.$1,
                items: List.generate(plan.$3, (i) {
                  final date = dueNow && i == plan.$4
                      ? DateTime.now()
                      : DateTime(2026, 11 + i, 12);
                  return InstallmentItemModel(
                    sequence: i + 1,
                    amount: plan.$5,
                    currency: 'TRY',
                    scheduledDate:
                        '${date.year}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}',
                    isRealized: i < plan.$4,
                  );
                }),
              ),
          ],
    accounts: const [FinanceChoice(id: 'a1', name: 'Ziraat vadesiz')],
    expenseCategories: const [
      FinanceChoice(id: 'c1', name: 'Demirbaş'),
      FinanceChoice(id: 'c2', name: 'Ticari mal alımı'),
    ],
  );

  static FinancePageActivity _row(
    String id,
    String date,
    String amount, {
    String? category,
    String? description,
    bool payment = false,
    int? sequence,
    int? count,
  }) => FinancePageActivity(
    activity: FinancialActivity.fromJson({
      'activityId': id,
      'activityKind': payment ? 'card-payment' : 'card-charge',
      'effect': payment ? 'neutral' : 'expense',
      'sourceGroup': 'credit-card',
      'origin': sequence == null ? 'manual' : 'installment',
      'status': 'realized',
      'activityDate': date,
      'amount': amount,
      'currency': 'TRY',
      'title': payment ? '' : (description ?? category ?? ''),
      'description': description,
      'categoryName': category,
      'sourceName': payment ? 'Ziraat vadesiz' : 'İşletme kartı',
      'canCancel': true,
      'supportsAttachments': false,
    }),
    amount: payment ? amount : '-$amount',
    installmentSequence: sequence,
    installmentCount: count,
  );

  static final _rows = [
    _row(
      'r1',
      '2026-11-12',
      '722.2200',
      category: 'Demirbaş',
      description: 'Soğutucu dolap',
      sequence: 1,
      count: 9,
    ),
    _row(
      'r2',
      '2026-11-08',
      '1800.0000',
      category: 'Araç ve yakıt',
      description: 'Araç bakımı',
    ),
    _row('r3', '2026-10-25', '3000.0000', payment: true),
    _row(
      'r4',
      '2026-10-20',
      '750.0000',
      category: 'Faturalar',
      description: 'Doğalgaz faturası',
    ),
    _row(
      'r5',
      '2026-10-20',
      '3180.0000',
      category: 'Ticari mal alımı',
      description: 'Kırtasiye toptan',
    ),
  ];

  @override
  Future<CardPageData> loadCardPage(
    String cardId,
    CardMovementQuery query, {
    int offset = 0,
    int? limit,
  }) async {
    final rows = _rows
        .where(
          (r) => switch (query.kind) {
            CardMovementKind.all => true,
            CardMovementKind.payment =>
              r.activity.kind == ActivityKind.cardPayment,
            CardMovementKind.installment => r.installmentSequence != null,
            CardMovementKind.charge =>
              r.activity.kind == ActivityKind.cardCharge &&
                  r.installmentSequence == null,
          },
        )
        .take(limit ?? 50)
        .toList();
    return CardPageData(
      cardId: cardId,
      name: 'İşletme kartı',
      period: query.period.all
          ? const CardPagePeriod(kind: 'all')
          : const CardPagePeriod(
              kind: 'statement',
              year: 2026,
              month: 11,
              start: '2026-10-16',
              end: '2026-11-15',
              status: 'current',
            ),
      statements: const [
        CardStatementRange(
          year: 2026,
          month: 10,
          start: '2026-09-16',
          end: '2026-10-15',
        ),
        CardStatementRange(
          year: 2026,
          month: 11,
          start: '2026-10-16',
          end: '2026-11-15',
        ),
      ],
      spending: query.kind == CardMovementKind.payment ? '0.0000' : '6452.2200',
      payments: query.kind == CardMovementKind.all ? '3000.0000' : null,
      categories: const [
        FinancePageOption(key: 'all', group: 'all', amount: '6452.2200'),
        FinancePageOption(
          key: 'category:c2',
          group: 'category',
          name: 'Ticari mal alımı',
          amount: '3180.0000',
          id: 'c2',
        ),
        FinancePageOption(
          key: 'category:c3',
          group: 'category',
          name: 'Araç ve yakıt',
          amount: '1800.0000',
          id: 'c3',
        ),
        FinancePageOption(
          key: 'category:c4',
          group: 'category',
          name: 'Faturalar',
          amount: '750.0000',
          id: 'c4',
        ),
        FinancePageOption(
          key: 'category:c1',
          group: 'category',
          name: 'Demirbaş',
          amount: '722.2200',
          id: 'c1',
        ),
      ],
      items: rows,
      totalCount: rows.length,
      offset: 0,
      hasMore: false,
    );
  }

  @override
  Future<List<InstallmentPreviewOption>> previewInstallments({
    required String totalAmount,
    required String firstInstallmentDate,
    required List<int> counts,
  }) async => [
    for (final (count, amount) in const [
      (2, '3250.0000'),
      (3, '2166.6666'),
      (4, '1625.0000'),
      (6, '1083.3333'),
      (9, '722.2222'),
      (12, '541.6666'),
    ])
      InstallmentPreviewOption(
        count: count,
        installmentAmount: amount,
        lastInstallmentAmount: amount,
        firstDate: firstInstallmentDate,
        lastDate: '2027-07-12',
      ),
  ];

  @override
  Future<CardStatement> loadStatement(
    String cardId,
    int year,
    int month,
    String asOf,
  ) async => (await loadCurrentStatement(cardId))!;

  @override
  Future<CardStatement?> loadCurrentStatement(String cardId) async =>
      CardStatement(
        periodStart: '2026-09-16',
        closingDate: '2026-10-15',
        dueDate: '2026-10-25',
        year: 2026,
        month: 10,
        previousBalance: '0.0000',
        periodCharges: '4010.0000',
        paymentsThroughClosing: '0.0000',
        statementBalance: '4010.0000',
        paymentsAfterClosing: '0.0000',
        remainingBalance: '4010.0000',
        minimumPayment: '802.0000',
        remainingMinimumPayment: '802.0000',
        minimumPaymentRate: '20.0000',
        currency: 'TRY',
        paymentStatus: dueNow ? 'overdue' : 'open',
      );
}
