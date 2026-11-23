import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/debt_book/data/debt_book_models.dart';
import 'package:business_finance_mobile/features/debt_book/data/debt_book_repository.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/counterparty_page_controller.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/counterparty_page_view.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_settle_sheet.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_book_controller.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_book_view.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_record_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// `Borç ve alacak` (Faz 5): sunucu cevabının okunması, kapsam ve yenileme,
/// listenin sekme/süzgeç/arama davranışı ve taşma.
void main() {
  group('DebtBook.fromJson', () {
    test('satırları, kişileri ve toplamları sunucudan okur', () {
      final book = DebtBook.fromJson(_json);

      expect(book.receivableTotal, '7000.0000');
      expect(book.payableOverdue, '500.0000');
      expect(book.closedReceivableCount, 2);
      final ayse = book.items.first;
      expect(ayse.isLedger, isTrue);
      expect(ayse.section, DebtBookSection.overdue);
      expect(ayse.scope, TransactionScope.business);
      expect(ayse.hasOverdue, isTrue);
      final kredi = book.items[1];
      expect(kredi.isInstallment, isTrue);
      expect(kredi.paidInstallmentCount, 3);
      // Tek ödemeli kayıtta da taksit sayısı gelir; taksitli sayılmaz.
      final single = DebtBookItem.fromJson({
        ..._json['items'][1] as Map<String, dynamic>,
        'planKind': 'single',
        'installmentCount': 1,
        'openInstallmentCount': 1,
      });
      expect(single.isInstallment, isFalse);
      // Tanınmayan bölüm listeyi bozmaz, `Diğer`e düşer.
      expect(book.items[2].section, DebtBookSection.other);
      expect(book.people.single.isActive, isFalse);
    });

    test('kişi sayfası cevabı okunur (sunucunun gerçek cevabı)', () {
      final page = CounterpartyPageData.fromJson(const {
        'id': '5ec0b59c-fced-498e-87e3-45c49009b493',
        'name': 'Deneme Kişi',
        'note': null,
        'isActive': true,
        'scope': 'business',
        'openingReceivable': '0.0000',
        'openingPayable': '0.0000',
        'owedToYou': '1000.0000',
        'owedByYou': '0.0000',
        'overdueReceivable': '0.0000',
        'overduePayable': '0.0000',
        'nextReceivableDueDate': null,
        'nextReceivableDueAmount': '0.0000',
        'nextPayableDueDate': null,
        'nextPayableDueAmount': '0.0000',
        'collect': {'full': '1000.0000', 'overdue': '0.0000'},
        'pay': {'full': '0.0000', 'overdue': '0.0000'},
        'records': <Object>[],
      });
      expect(page.collect.full, '1000.0000');
      expect(page.pay.hasFull, isFalse);
      expect(page.owesYou, isTrue);
      expect(page.scope, TransactionScope.business);
    });

    test('tutarın tamamı avanssa satır avans yazar', () {
      const item = DebtBookItem(
        isLedger: true,
        id: 'cp',
        side: DebtSide.payable,
        section: DebtBookSection.other,
        title: 'Mehmet Usta',
        remaining: '2000.0000',
        overdue: '0.0000',
        advanceAmount: '2000.0000',
      );
      expect(item.isAllAdvance, isTrue);
    });
  });

  group('DebtBookController', () {
    test('kapsamı anahtardan okur ve değişince yeniden okur', () async {
      final scope = ScopeController();
      await scope.applyHasBusiness(true);
      final repository = _Repository();
      final controller = DebtBookController(
        repository,
        scopeController: scope,
        now: () => DateTime(2026, 11, 23),
      );
      await controller.load();
      expect(repository.calls.single, (asOf: '2026-11-23', scope: null));

      await scope.select(TransactionScope.business);
      await pumpEventQueue();
      expect(repository.calls.last.scope, TransactionScope.business);
      expect(controller.book, isNotNull);
      controller.dispose();
    });

    test('yenileme düşerse eski liste kalır ve uyarı verilir', () async {
      final repository = _Repository();
      final controller = DebtBookController(repository);
      await controller.load();
      repository.fail = true;
      await controller.load();
      expect(controller.book, isNotNull);
      expect(controller.isStale, isTrue);
      expect(controller.unauthorized, isFalse);
      controller.dispose();
    });
  });

  group('akış türleri', () {
    test('borç para ve vazgeçme tanınır, yönle adlanır', () {
      expect(
        ActivityKind.fromApi('counterparty-loan'),
        ActivityKind.counterpartyLoan,
      );
      expect(
        ActivityKind.counterpartyLoan.labelFor(ActivityDirection.receivable),
        'Borç verdim',
      );
      expect(
        ActivityKind.counterpartyWriteOff.labelFor(ActivityDirection.payable),
        'Bağışlanan borç',
      );
    });
  });

  group('CounterpartyPageController', () {
    CounterpartyPageController controller(_Repository repository) =>
        CounterpartyPageController(
          repository,
          counterpartyId: 'cp-hasan',
          now: () => DateTime(2026, 11, 23),
        );

    test(
      'özet ve hareketler; sayfa sayfa yüklenir, süzgeç sunucuya gider',
      () async {
        final repository = _Repository();
        final page = controller(repository);
        await page.load();
        expect(page.page?.name, _person.name);
        expect(page.movements, hasLength(2));
        expect(page.movements.first.kind, ActivityKind.counterpartyLoan);
        expect(page.hasMore, isTrue);
        await page.loadMore();
        expect(page.movements, hasLength(3));
        expect(page.hasMore, isFalse);

        await page.applyFilter(
          const PersonMovementFilter(
            kind: PersonMovementKind.trade,
            side: DebtSide.payable,
          ),
        );
        final last = repository.movementCalls.last;
        expect(last.page, 1);
        expect(last.filter.kind.apiValue, 'counterparty-charge');
        expect(last.filter.side, DebtSide.payable);
        page.dispose();
      },
    );

    test('tahsilat ve ödeme yerine göre kurulur', () async {
      final repository = _Repository();
      final page = controller(repository);
      DebtSettlement settlement(String source, {bool discount = false}) =>
          DebtSettlement(
            side: source.startsWith('card')
                ? DebtSide.payable
                : DebtSide.receivable,
            amount: '250.0000',
            date: '2026-11-23',
            sourceId: source,
            clientRequestId: 'req-1',
            closeRemainderAsDiscount: discount,
          );
      expect(await page.settle(settlement('account:a1')), isNull);
      expect(await page.settle(settlement('pos:p1')), isNull);
      expect(await page.settle(settlement('card:c1', discount: true)), isNull);

      expect(repository.settled[0], containsPair('accountId', 'a1'));
      expect(repository.settled[0], containsPair('direction', 'receivable'));
      expect(repository.settled[0], containsPair('clientRequestId', 'req-1'));
      expect(repository.settled[1]['card'], {'posDefinitionId': 'p1'});
      expect(repository.settled[1].containsKey('accountId'), isFalse);
      expect(repository.settled[2], containsPair('creditCardId', 'c1'));
      expect(repository.settled[2], containsPair('direction', 'payable'));
      expect(
        repository.settled[2],
        containsPair('closeRemainderAsDiscount', true),
      );
      page.dispose();
    });

    test('reddedilen kayıt panelde söylenecek cümleyi döner', () async {
      final repository = _Repository()..refuse = 'credit_cards.limit_exceeded';
      final page = controller(repository);
      final message = await page.settle(
        const DebtSettlement(
          side: DebtSide.payable,
          amount: '9000.0000',
          date: '2026-11-23',
          sourceId: 'card:c1',
          clientRequestId: 'req-2',
        ),
      );
      expect(message, isNotNull);
      page.dispose();
    });

    test('indirim satışın kimliğiyle yazılır', () async {
      final repository = _Repository();
      final page = controller(repository);
      await page.load();
      final charge = page.movements[1];
      expect(
        await page.discount(
          charge,
          amount: '200.0000',
          date: '2026-11-23',
          clientRequestId: 'req-3',
        ),
        isNull,
      );
      expect(repository.discounts.single.$1, charge.activityId);
      expect(
        repository.discounts.single.$2,
        containsPair('amount', '200.0000'),
      );
      page.dispose();
    });
  });

  group('DebtBookView', () {
    testWidgets('sekme tarafı değiştirir, süzgeç yalnız gecikenleri bırakır', (
      tester,
    ) async {
      await tester.pumpWidget(_host(DebtBookView(book: _book)));
      expect(find.text('Ayşe Terzi'), findsOneWidget);
      expect(find.text('Kaya Yapı'), findsOneWidget);
      expect(find.text('Toptancı Demir'), findsNothing);

      await tester.tap(find.text('Yalnız gecikenler'));
      await tester.pump();
      expect(find.text('Ayşe Terzi'), findsOneWidget);
      expect(find.text('Kaya Yapı'), findsNothing);

      await tester.tap(find.text('Borçlar'));
      await tester.pump();
      expect(find.text('Toptancı Demir'), findsOneWidget);
      expect(find.text('Ayşe Terzi'), findsNothing);
      // Borç tarafında gecikme yok: süzgeç çizilmez.
      expect(find.text('Yalnız gecikenler'), findsNothing);
    });

    testWidgets('pasif kişi Kişiler sekmesinde yok, aramada Pasif yazar', (
      tester,
    ) async {
      await tester.pumpWidget(
        _host(DebtBookView(book: _book, initialTab: DebtBookTab.people)),
      );
      expect(find.text('Eski Kiracı'), findsNothing);

      await tester.tap(find.byTooltip('Ara'));
      await tester.pump();
      await tester.enterText(find.byType(TextField), 'kira');
      await tester.pump();
      expect(find.text('Eski Kiracı'), findsOneWidget);
      expect(find.textContaining('Pasif'), findsOneWidget);
    });

    testWidgets('hiç kayıt yoksa ilk kullanım; + başlıkta çizilmez', (
      tester,
    ) async {
      await tester.pumpWidget(
        _host(
          const DebtBookView(
            book: DebtBook(
              receivableTotal: '0.0000',
              payableTotal: '0.0000',
              items: [],
              people: [],
            ),
          ),
        ),
      );
      expect(find.text('Henüz borç ya da alacak yok'), findsOneWidget);
      expect(find.byTooltip('Borç veya alacak ekle'), findsNothing);
    });
  });

  // Faz 5 kapısı: 320–800 dp ve 1.0–2.0 yazı ölçeğinde taşma yok.
  for (final width in [320.0, 360.0, 412.0, 600.0, 800.0]) {
    for (final scale in [1.0, 1.3, 2.0]) {
      testWidgets('taşma yok: $width dp, ${scale}x', (tester) async {
        tester.view.physicalSize = Size(width, 900);
        tester.view.devicePixelRatio = 1;
        addTearDown(tester.view.reset);
        for (final page in [
          DebtBookView(book: _book, scopeVisible: true),
          DebtBookView(
            book: _book,
            scopeVisible: true,
            initialTab: DebtBookTab.people,
          ),
          const CounterpartyPageView(
            page: _person,
            movements: [],
            scopeVisible: true,
          ),
          DebtRecordView(record: _record, scopeVisible: true),
        ]) {
          await tester.pumpWidget(_host(page, scale: scale));
          await tester.pump();
          expect(tester.takeException(), isNull);
        }
      });
    }
  }
}

Widget _host(Widget child, {double scale = 1.0}) => MaterialApp(
  theme: AppTheme.light(),
  home: Builder(
    builder: (context) => MediaQuery(
      data: MediaQuery.of(
        context,
      ).copyWith(textScaler: TextScaler.linear(scale)),
      child: Scaffold(body: child),
    ),
  ),
);

class _Repository implements DebtBookRepositoryContract {
  final calls = <({String asOf, TransactionScope? scope})>[];
  final movementCalls = <({PersonMovementFilter filter, int page})>[];
  final settled = <Map<String, Object?>>[];
  final discounts = <(String, Map<String, Object?>)>[];
  bool fail = false;
  String? refuse;

  @override
  Future<CounterpartyPageData> loadPerson(
    String counterpartyId, {
    required String asOfDate,
  }) async => _person;

  @override
  Future<ActivityPage> loadMovements(
    String counterpartyId, {
    required PersonMovementFilter filter,
    required DateTime today,
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    movementCalls.add((filter: filter, page: pageNumber));
    return ActivityPage.fromJson({
      'items': [
        for (var i = 0; i < (pageNumber == 1 ? 2 : 1); i++)
          {
            'activityId': 'a-$pageNumber-$i',
            'activityKind': i == 0
                ? 'counterparty-loan'
                : 'counterparty-charge',
            'effect': 'neutral',
            'sourceGroup': 'counterparty',
            'origin': 'manual',
            'status': 'realized',
            'activityDate': '2026-11-20',
            'amount': '100.0000',
            'currency': 'TRY',
            'title': 'Hasan',
            'canCancel': true,
            'supportsAttachments': false,
            'direction': 'receivable',
          },
      ],
      'pagination': {
        'pageNumber': pageNumber,
        'pageSize': pageSize,
        'totalCount': 3,
        'totalPages': 2,
        'hasPreviousPage': pageNumber > 1,
        'hasNextPage': pageNumber == 1,
      },
    });
  }

  @override
  Future<DebtSettleSources> loadSettleSources() async =>
      const DebtSettleSources(accounts: [], cards: [], posDefinitions: []);

  @override
  Future<void> settle(String counterpartyId, Map<String, Object?> body) async {
    if (refuse != null) throw ApiException.local(refuse!);
    settled.add(body);
  }

  @override
  Future<void> discountCharge(
    String chargeId,
    Map<String, Object?> body,
  ) async => discounts.add((chargeId, body));

  @override
  Future<DebtBook> load({
    required String asOfDate,
    TransactionScope? scope,
  }) async {
    calls.add((asOf: asOfDate, scope: scope));
    if (fail) throw ApiException.network();
    return _book;
  }
}

const _json = <String, dynamic>{
  'receivableTotal': '7000.0000',
  'payableTotal': '500.0000',
  'receivableOverdue': '2000.0000',
  'payableOverdue': '500.0000',
  'closedReceivableCount': 2,
  'closedPayableCount': 0,
  'items': [
    {
      'kind': 'ledger',
      'id': 'cp-1',
      'direction': 'receivable',
      'section': 'overdue',
      'title': 'Ayşe Terzi',
      'remaining': '3500.0000',
      'overdue': '2000.0000',
      'nextDueDate': '2026-11-30',
      'nextDueAmount': '1500.0000',
      'hasClosings': false,
      'isCarriedOver': false,
      'startDate': null,
      'scope': 'business',
      'advanceAmount': '0.0000',
    },
    {
      'kind': 'debt',
      'id': 'd-1',
      'direction': 'receivable',
      'section': 'upcoming',
      'title': 'Gelinlik',
      'counterpartyId': 'cp-1',
      'counterpartyName': 'Ayşe Terzi',
      'planKind': 'installments',
      'remaining': '3000.0000',
      'overdue': '0.0000',
      'nextDueDate': '2026-12-15',
      'nextDueAmount': '500.0000',
      'hasClosings': true,
      'isCarriedOver': false,
      'startDate': '2026-09-01',
      'scope': 'business',
      'installmentCount': 9,
      'openInstallmentCount': 6,
    },
    {
      'kind': 'ledger',
      'id': 'cp-2',
      'direction': 'payable',
      'section': 'someday',
      'title': 'Toptancı',
      'remaining': '500.0000',
      'overdue': '500.0000',
      'nextDueAmount': '0.0000',
      'hasClosings': false,
      'isCarriedOver': false,
    },
  ],
  'people': [
    {
      'counterpartyId': 'cp-9',
      'name': 'Eski Kiracı',
      'owedToYou': '0.0000',
      'owedByYou': '0.0000',
      'ledgerOwedToYou': '0.0000',
      'ledgerOwedByYou': '0.0000',
      'openDebtCount': 0,
      'isActive': false,
      'scope': 'personal',
    },
  ],
};

const _book = DebtBook(
  receivableTotal: '9500.0000',
  payableTotal: '1245000.0000',
  receivableOverdue: '2000.0000',
  items: [
    DebtBookItem(
      isLedger: true,
      id: 'cp-ayse',
      side: DebtSide.receivable,
      section: DebtBookSection.overdue,
      title: 'Ayşe Terzi',
      remaining: '3500.0000',
      overdue: '2000.0000',
      nextDueDate: '2026-11-30',
      nextDueAmount: '1500.0000',
      scope: TransactionScope.business,
    ),
    DebtBookItem(
      isLedger: true,
      id: 'cp-kaya',
      side: DebtSide.receivable,
      section: DebtBookSection.upcoming,
      title: 'Kaya Yapı',
      remaining: '6000.0000',
      overdue: '0.0000',
      nextDueDate: '2026-12-18',
      nextDueAmount: '6000.0000',
      scope: TransactionScope.business,
    ),
    DebtBookItem(
      isLedger: true,
      id: 'cp-demir',
      side: DebtSide.payable,
      section: DebtBookSection.other,
      title: 'Toptancı Demir',
      remaining: '1245000.0000',
      overdue: '0.0000',
      scope: TransactionScope.personal,
    ),
  ],
  people: [
    DebtBookPerson(
      counterpartyId: 'cp-ayse',
      name: 'Ayşe Terzi',
      owedToYou: '3500.0000',
      owedByYou: '0.0000',
      scope: TransactionScope.business,
    ),
    DebtBookPerson(
      counterpartyId: 'cp-eski',
      name: 'Eski Kiracı',
      owedToYou: '0.0000',
      owedByYou: '0.0000',
      isActive: false,
    ),
    DebtBookPerson(
      counterpartyId: 'cp-hasan',
      name: 'Hasan Bakkal Gıda Toptan ve Perakende Ticaret',
      owedToYou: '1250.0000',
      owedByYou: '1245000.0000',
      scope: TransactionScope.business,
    ),
  ],
  closedReceivableCount: 1,
);

const _person = CounterpartyPageData(
  id: 'cp-hasan',
  name: 'Hasan Bakkal Gıda Toptan ve Perakende Ticaret',
  scope: TransactionScope.business,
  owedToYou: '1250.0000',
  owedByYou: '1245000.0000',
  overdueReceivable: '1250.0000',
  nextPayableDueDate: '2026-12-05',
  nextPayableDueAmount: '400.0000',
  collect: DebtShortcuts(full: '1250.0000', overdue: '1250.0000'),
  pay: DebtShortcuts(full: '1245000.0000', overdue: '0.0000'),
);

final _record = DebtRecordData(
  id: 'd-1',
  title: 'Konut kredisi',
  side: DebtSide.payable,
  remaining: '1245000.0000',
  planKind: 'installments',
  counterpartyName: 'Ziraat Bankası Anonim Şirketi',
  scope: TransactionScope.personal,
  isInterestBearing: true,
  remainingPrincipal: '1012400.0000',
  installments: [
    for (var i = 1; i <= 5; i++)
      DebtRecordInstallment(
        sequence: i,
        amount: '15000.0000',
        dueDate: '2026-${(i + 7).toString().padLeft(2, '0')}-02',
        status: i < 3 ? 'paid' : (i == 3 ? 'overdue' : 'upcoming'),
        openAmount: i < 3 ? '0.0000' : '15000.0000',
        paidAmount: i < 3 ? '15000.0000' : '0.0000',
      ),
  ],
  closings: const [],
);
