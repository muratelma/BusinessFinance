import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/debt_book/data/debt_book_models.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/counterparty_page_view.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_book_view.dart';
import 'package:business_finance_mobile/core/widgets/app_category_field.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_entry_form_view.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_entry_sheet.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_record_view.dart';
import 'package:business_finance_mobile/features/debt_book/presentation/debt_settle_sheet.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Borç ve alacak ekranlarının Faz 4 kareleri
/// (`stages/06.3-borc-alacak/05-EKRAN-TASARIMI.md`). Veri sentetiktir ve tek
/// tutarlı settir: listedeki satırların toplamı özetteki iki sayıya eşittir.
///
/// ```bash
/// SCREENSHOT_DIR=../../tasarim-onizleme/borc-alacak-2026-11-23 \
///   flutter test test/screenshots/borc_alacak_tasarim_test.dart
/// ```
void main() {
  Widget page(Widget child) =>
      Scaffold(body: SafeArea(bottom: false, child: child));

  for (final (name, tab, scope) in [
    ('01-liste-alacaklar', DebtBookTab.receivables, null),
    ('02-liste-borclar', DebtBookTab.payables, null),
    ('03-liste-kisiler', DebtBookTab.people, null),
    ('04-liste-isletme', DebtBookTab.receivables, TransactionScope.business),
  ]) {
    testWidgets('borç ve alacak $name', (tester) async {
      await captureScreen(
        tester,
        name,
        page(
          DebtBookView(
            book: scope == null ? _book : _businessBook,
            scope: scope,
            scopeVisible: true,
            initialTab: tab,
            onBack: () {},
            onAdd: () {},
          ),
        ),
        withNavBar: false,
      );
    }, skip: !screenshotsEnabled);
  }

  testWidgets('borç ve alacak 05 yalnız gecikenler', (tester) async {
    await captureScreen(
      tester,
      '05-liste-gecikenler',
      page(
        DebtBookView(
          book: _book,
          scopeVisible: true,
          initialOverdueOnly: true,
          onBack: () {},
          onAdd: () {},
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('borç ve alacak 06 arama', (tester) async {
    await captureScreen(
      tester,
      '06-liste-arama',
      page(
        DebtBookView(
          book: _book,
          scopeVisible: true,
          initialQuery: 'ka',
          onBack: () {},
          onAdd: () {},
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('borç ve alacak 07 ilk kullanım', (tester) async {
    await captureScreen(
      tester,
      '07-liste-ilk-kullanim',
      page(
        DebtBookView(
          book: const DebtBook(
            receivableTotal: '0.0000',
            payableTotal: '0.0000',
            items: [],
            people: [],
          ),
          onBack: () {},
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  for (final (name, data, movements) in [
    ('10-kisi-alacak', _ayse, _ayseMovements),
    ('11-kisi-iki-taraf', _hasan, _hasanMovements),
    ('12-kisi-kapali', _selin, const <FinancialActivity>[]),
  ]) {
    testWidgets('kişi $name', (tester) async {
      await captureScreen(
        tester,
        name,
        page(
          CounterpartyPageView(
            page: data,
            movements: movements,
            scopeVisible: true,
            onBack: () {},
            onAdd: () {},
            onMore: () {},
            onCollect: () {},
            onPay: () {},
          ),
        ),
        withNavBar: false,
      );
    }, skip: !screenshotsEnabled);
  }

  testWidgets('panel 20 tahsilat fazla tutar', (tester) async {
    await captureScreen(
      tester,
      '20-panel-tahsilat',
      _SheetHost(
        background: page(
          CounterpartyPageView(
            page: _ayse,
            movements: _ayseMovements,
            scopeVisible: true,
            onBack: () {},
            onAdd: () {},
            onMore: () {},
            onCollect: () {},
          ),
        ),
        sheet: DebtSettleSheet(
          side: DebtSide.receivable,
          personName: 'Ayşe Terzi',
          shortcuts: _ayse.collect,
          options: _collectOptions,
          initialAccountId: 'account:kasa',
          initialAmount: '4000.0000',
          today: DateTime(2026, 11, 23),
          onSubmit: (_) async => null,
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('panel 21 ödeme kalanı indirim', (tester) async {
    await captureScreen(
      tester,
      '21-panel-odeme',
      _SheetHost(
        background: page(
          CounterpartyPageView(
            page: _demir,
            movements: const [],
            scopeVisible: true,
            onBack: () {},
            onAdd: () {},
            onMore: () {},
            onPay: () {},
          ),
        ),
        sheet: DebtSettleSheet(
          side: DebtSide.payable,
          personName: 'Toptancı Demir Ltd.',
          shortcuts: _demir.pay,
          options: _payOptions,
          initialAccountId: 'account:banka',
          initialAmount: '2000.0000',
          initialDiscount: true,
          today: DateTime(2026, 11, 23),
          onSubmit: (_) async => null,
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  for (final (name, active) in [
    ('30-ne-oldu', true),
    ('31-ne-oldu-pasif', false),
  ]) {
    testWidgets('yeni kayıt $name', (tester) async {
      await captureScreen(
        tester,
        name,
        _SheetHost(
          background: page(
            CounterpartyPageView(
              page: _ayse,
              movements: _ayseMovements,
              scopeVisible: true,
              onBack: () {},
              onAdd: () {},
              onMore: () {},
              onCollect: () {},
            ),
          ),
          sheet: DebtEntrySheet(personName: 'Ayşe Terzi', personActive: active),
        ),
        withNavBar: false,
      );
    }, skip: !screenshotsEnabled);
  }

  testWidgets('yeni kayıt 32 satış ve avans', (tester) async {
    await captureScreen(
      tester,
      '32-form-satis-avans',
      page(
        DebtEntryFormView(
          kind: DebtEntryKind.sale,
          today: DateTime(2026, 11, 23),
          personName: 'Ayşe Terzi',
          personLocked: true,
          categoryOptions: _categories,
          initialCategoryId: 'cat-terzilik',
          accountOptions: _collectOptions,
          initialAccountId: 'account:kasa',
          initialAmount: '10000.0000',
          initialUpfront: true,
          initialUpfrontAmount: '4000.0000',
          onBack: () {},
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yeni kayıt 33 borç aldım taksitli', (tester) async {
    await captureScreen(
      tester,
      '33-form-borc-taksitli',
      page(
        DebtEntryFormView(
          kind: DebtEntryKind.borrow,
          today: DateTime(2026, 11, 23),
          personName: 'Ziraat Bankası',
          accountOptions: _payOptions,
          initialAccountId: 'account:banka',
          initialAmount: '120000.0000',
          initialInstallments: true,
          initialInstallmentCount: 12,
          preview: const [
            DebtPreviewInstallment(
              sequence: 1,
              dueDate: '2026-12-23',
              amount: '11250.0000',
            ),
            DebtPreviewInstallment(
              sequence: 2,
              dueDate: '2027-01-23',
              amount: '11250.0000',
            ),
            DebtPreviewInstallment(
              sequence: 3,
              dueDate: '2027-02-23',
              amount: '11250.0000',
            ),
          ],
          previewTotalCount: 12,
          previewLastDate: '2027-11-23',
          onBack: () {},
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yeni kayıt 34 uygulamadan önce', (tester) async {
    await captureScreen(
      tester,
      '34-form-uygulamadan-once',
      page(
        DebtEntryFormView(
          kind: DebtEntryKind.purchase,
          today: DateTime(2026, 11, 23),
          initialAmount: '4000.0000',
          initialCarriedOver: true,
          onBack: () {},
        ),
      ),
      withNavBar: false,
    );
  }, skip: !screenshotsEnabled);

  for (final (name, record) in [
    ('40-kayit-taksitli', _gelinlik),
    ('41-kayit-kredi', _konut),
    ('42-kayit-kisisiz', _sigorta),
  ]) {
    testWidgets('kayıt $name', (tester) async {
      await captureScreen(
        tester,
        name,
        page(
          DebtRecordView(
            record: record,
            scopeVisible: true,
            onBack: () {},
            onMore: () {},
            onPay: () {},
            onOpenPerson: () {},
            onShowSchedule: () {},
          ),
        ),
        withNavBar: false,
      );
    }, skip: !screenshotsEnabled);
  }
}

const _categories = [
  AppCategoryOption(
    id: 'cat-terzilik',
    name: 'Terzilik geliri',
    scope: TransactionScope.business,
  ),
];

List<DebtRecordInstallment> _schedule({
  required int count,
  required int paid,
  required String amount,
  required DateTime first,
}) => [
  for (var i = 0; i < count; i++)
    DebtRecordInstallment(
      sequence: i + 1,
      amount: amount,
      dueDate: _isoDate(DateTime(first.year, first.month + i, first.day)),
      status: i < paid ? 'paid' : 'open',
      openAmount: i < paid ? '0.0000' : amount,
      paidAmount: i < paid ? amount : '0.0000',
    ),
];

String _isoDate(DateTime day) =>
    '${day.year}-${day.month.toString().padLeft(2, '0')}-'
    '${day.day.toString().padLeft(2, '0')}';

/// 12 × 500 = 6.000; 3 taksit ödendi, kalan 4.500 (listedeki satır).
final _gelinlik = DebtRecordData(
  id: 'debt-gelinlik',
  title: 'Gelinlik dikimi',
  side: DebtSide.receivable,
  remaining: '4500.0000',
  planKind: 'installments',
  counterpartyId: 'cp-ayse',
  counterpartyName: 'Ayşe Terzi',
  scope: _business,
  installments: _schedule(
    count: 12,
    paid: 3,
    amount: '500.0000',
    first: DateTime(2026, 9, 15),
  ),
  closings: const [
    DebtRecordClosing(
      id: 'c-1',
      isDiscount: false,
      amount: '500.0000',
      date: '2026-09-15',
      sourceName: 'Nakit kasa',
    ),
    DebtRecordClosing(
      id: 'c-2',
      isDiscount: false,
      amount: '500.0000',
      date: '2026-10-15',
      sourceName: 'Nakit kasa',
    ),
    DebtRecordClosing(
      id: 'c-3',
      isDiscount: false,
      amount: '500.0000',
      date: '2026-11-14',
      sourceName: 'Ana POS',
    ),
  ],
);

/// 120 × 15.000; 37'si uygulamadan önce ödendi (senaryo 27), kalan 83 ×
/// 15.000 = 1.245.000. Kalan anapara sunucudan gelecek alandır (örnek).
final _konut = DebtRecordData(
  id: 'debt-konut',
  title: 'Konut kredisi',
  side: DebtSide.payable,
  remaining: '1245000.0000',
  planKind: 'installments',
  counterpartyId: 'cp-ziraat',
  counterpartyName: 'Ziraat Bankası',
  scope: _personal,
  isInterestBearing: true,
  isCarriedOver: true,
  remainingPrincipal: '1012400.0000',
  installments: _schedule(
    count: 120,
    paid: 37,
    amount: '15000.0000',
    first: DateTime(2023, 11, 2),
  ),
  closings: const [],
);

final _sigorta = DebtRecordData(
  id: 'debt-sigorta',
  title: 'İşyeri sigortası',
  side: DebtSide.payable,
  remaining: '800.0000',
  planKind: 'single',
  scope: _business,
  installments: const [
    DebtRecordInstallment(
      sequence: 1,
      amount: '800.0000',
      dueDate: '2027-01-10',
      status: 'open',
      openAmount: '800.0000',
      paidAmount: '0.0000',
    ),
  ],
  closings: const [],
);

/// Arkadaki sayfanın üstünde paneli açar.

class _SheetHost extends StatefulWidget {
  const _SheetHost({required this.background, required this.sheet});

  final Widget background;
  final Widget sheet;

  @override
  State<_SheetHost> createState() => _SheetHostState();
}

class _SheetHostState extends State<_SheetHost> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      AppFormSheet.show<bool>(context: context, builder: (_) => widget.sheet);
    });
  }

  @override
  Widget build(BuildContext context) => widget.background;
}

// ── Örnek veri: 23 Kasım 2026 ──────────────────────────────────────────

const _business = TransactionScope.business;
const _personal = TransactionScope.personal;

const _receivables = [
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
    oldestOverdueDate: '2026-11-01',
    scope: _business,
  ),
  DebtBookItem(
    isLedger: false,
    id: 'debt-gelinlik',
    side: DebtSide.receivable,
    section: DebtBookSection.upcoming,
    title: 'Gelinlik dikimi',
    counterpartyId: 'cp-ayse',
    counterpartyName: 'Ayşe Terzi',
    planKind: 'installments',
    remaining: '4500.0000',
    overdue: '0.0000',
    nextDueDate: '2026-12-15',
    nextDueAmount: '500.0000',
    installmentCount: 12,
    openInstallmentCount: 9,
    scope: _business,
  ),
  DebtBookItem(
    isLedger: true,
    id: 'cp-kaya',
    side: DebtSide.receivable,
    section: DebtBookSection.upcoming,
    title: 'Kaya Yapı Malzemeleri',
    remaining: '6000.0000',
    overdue: '0.0000',
    nextDueDate: '2026-12-18',
    nextDueAmount: '6000.0000',
    scope: _business,
  ),
  DebtBookItem(
    isLedger: true,
    id: 'cp-annem',
    side: DebtSide.receivable,
    section: DebtBookSection.other,
    title: 'Annem',
    remaining: '5000.0000',
    overdue: '0.0000',
    scope: _personal,
  ),
  DebtBookItem(
    isLedger: true,
    id: 'cp-hasan',
    side: DebtSide.receivable,
    section: DebtBookSection.other,
    title: 'Hasan Bakkal',
    remaining: '1250.0000',
    overdue: '0.0000',
    scope: _business,
  ),
];

const _payables = [
  DebtBookItem(
    isLedger: true,
    id: 'cp-demir',
    side: DebtSide.payable,
    section: DebtBookSection.overdue,
    title: 'Toptancı Demir Ltd.',
    remaining: '2200.0000',
    overdue: '500.0000',
    nextDueDate: '2026-12-05',
    nextDueAmount: '1700.0000',
    oldestOverdueDate: '2026-11-15',
    scope: _business,
  ),
  DebtBookItem(
    isLedger: false,
    id: 'debt-konut',
    side: DebtSide.payable,
    section: DebtBookSection.upcoming,
    title: 'Konut kredisi',
    counterpartyId: 'cp-ziraat',
    counterpartyName: 'Ziraat Bankası',
    planKind: 'installments',
    remaining: '1245000.0000',
    overdue: '0.0000',
    nextDueDate: '2026-12-02',
    nextDueAmount: '15000.0000',
    installmentCount: 120,
    openInstallmentCount: 83,
    scope: _personal,
  ),
  DebtBookItem(
    isLedger: true,
    id: 'cp-hasan',
    side: DebtSide.payable,
    section: DebtBookSection.upcoming,
    title: 'Hasan Bakkal',
    remaining: '400.0000',
    overdue: '0.0000',
    nextDueDate: '2026-12-05',
    nextDueAmount: '400.0000',
    scope: _business,
  ),
  DebtBookItem(
    isLedger: true,
    id: 'cp-mehmet',
    side: DebtSide.payable,
    section: DebtBookSection.other,
    title: 'Mehmet Usta',
    remaining: '2000.0000',
    overdue: '0.0000',
    advanceAmount: '2000.0000',
    scope: _business,
  ),
  DebtBookItem(
    isLedger: false,
    id: 'debt-sigorta',
    side: DebtSide.payable,
    section: DebtBookSection.other,
    title: 'İşyeri sigortası',
    planKind: 'single',
    remaining: '800.0000',
    overdue: '0.0000',
    nextDueDate: '2027-01-10',
    nextDueAmount: '800.0000',
    scope: _business,
  ),
];

const _people = [
  DebtBookPerson(
    counterpartyId: 'cp-annem',
    name: 'Annem',
    owedToYou: '5000.0000',
    owedByYou: '0.0000',
    scope: _personal,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-ayse',
    name: 'Ayşe Terzi',
    owedToYou: '8000.0000',
    owedByYou: '0.0000',
    scope: _business,
    openDebtCount: 1,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-eski',
    name: 'Eski Kiracı Kemal',
    owedToYou: '0.0000',
    owedByYou: '0.0000',
    isActive: false,
    scope: _personal,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-hasan',
    name: 'Hasan Bakkal',
    owedToYou: '1250.0000',
    owedByYou: '400.0000',
    scope: _business,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-kaya',
    name: 'Kaya Yapı Malzemeleri',
    owedToYou: '6000.0000',
    owedByYou: '0.0000',
    scope: _business,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-mehmet',
    name: 'Mehmet Usta',
    owedToYou: '0.0000',
    owedByYou: '2000.0000',
    scope: _business,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-selin',
    name: 'Selin Kaya',
    owedToYou: '0.0000',
    owedByYou: '0.0000',
    scope: _personal,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-demir',
    name: 'Toptancı Demir Ltd.',
    owedToYou: '0.0000',
    owedByYou: '2200.0000',
    scope: _business,
  ),
  DebtBookPerson(
    counterpartyId: 'cp-ziraat',
    name: 'Ziraat Bankası',
    owedToYou: '0.0000',
    owedByYou: '1245000.0000',
    scope: _personal,
    openDebtCount: 1,
  ),
];

/// Alacak 3.500 + 4.500 + 6.000 + 5.000 + 1.250 = 20.250;
/// borç 2.200 + 1.245.000 + 400 + 2.000 + 800 = 1.250.400.
const _book = DebtBook(
  receivableTotal: '20250.0000',
  payableTotal: '1250400.0000',
  receivableOverdue: '2000.0000',
  payableOverdue: '500.0000',
  items: [..._receivables, ..._payables],
  people: _people,
  closedReceivableCount: 2,
  closedPayableCount: 1,
);

/// `İşletme` görünümü: şahsi kişiler ve kayıtlar düşer.
/// Alacak 3.500 + 4.500 + 6.000 + 1.250 = 15.250; borç 2.200 + 400 + 2.000 +
/// 800 = 5.400.
final _businessBook = DebtBook(
  receivableTotal: '15250.0000',
  payableTotal: '5400.0000',
  receivableOverdue: '2000.0000',
  payableOverdue: '500.0000',
  items: [
    for (final item in [..._receivables, ..._payables])
      if (item.scope == _business) item,
  ],
  people: [
    for (final person in _people)
      if (person.scope == _business) person,
  ],
  closedReceivableCount: 1,
  closedPayableCount: 1,
);

const _ayse = CounterpartyPageData(
  id: 'cp-ayse',
  name: 'Ayşe Terzi',
  scope: _business,
  owedToYou: '8000.0000',
  owedByYou: '0.0000',
  overdueReceivable: '2000.0000',
  nextReceivableDueDate: '2026-11-30',
  nextReceivableDueAmount: '1500.0000',
  collect: DebtShortcuts(full: '3500.0000', overdue: '2000.0000'),
  pay: DebtShortcuts(full: '0.0000', overdue: '0.0000'),
  records: [
    DebtBookItem(
      isLedger: false,
      id: 'debt-gelinlik',
      side: DebtSide.receivable,
      section: DebtBookSection.upcoming,
      title: 'Gelinlik dikimi',
      counterpartyId: 'cp-ayse',
      counterpartyName: 'Ayşe Terzi',
      planKind: 'installments',
      remaining: '4500.0000',
      overdue: '0.0000',
      nextDueDate: '2026-12-15',
      nextDueAmount: '500.0000',
      installmentCount: 12,
      openInstallmentCount: 9,
      scope: _business,
    ),
  ],
);

/// 2.600 (1 Kas vadeli) + 1.500 (30 Kas vadeli) − 600 = 3.500; ödeme en eski
/// satıştan düşer, gecikmiş 2.000.
final _ayseMovements = [
  _activity(
    'a-3',
    ActivityKind.counterpartySettlement,
    ActivityEffect.neutral,
    '600.0000',
    '2026-11-20',
    title: 'Ayşe Terzi',
    source: 'Nakit kasa',
  ),
  _activity(
    'a-2',
    ActivityKind.counterpartyCharge,
    ActivityEffect.income,
    '1500.0000',
    '2026-11-03',
    title: 'Kumaş ve dikiş',
    category: 'Terzilik geliri',
  ),
  _activity(
    'a-1',
    ActivityKind.counterpartyCharge,
    ActivityEffect.income,
    '2600.0000',
    '2026-10-15',
    title: 'Gelinlik provası',
    category: 'Terzilik geliri',
  ),
];

const _hasan = CounterpartyPageData(
  id: 'cp-hasan',
  name: 'Hasan Bakkal',
  scope: _business,
  owedToYou: '1250.0000',
  owedByYou: '400.0000',
  nextPayableDueDate: '2026-12-05',
  nextPayableDueAmount: '400.0000',
  collect: DebtShortcuts(full: '1250.0000', overdue: '0.0000'),
  pay: DebtShortcuts(full: '400.0000', overdue: '0.0000'),
);

final _hasanMovements = [
  _activity(
    'h-2',
    ActivityKind.counterpartyCharge,
    ActivityEffect.expense,
    '400.0000',
    '2026-11-05',
    title: 'Hasan Bakkal',
    category: 'Mal alımı',
    direction: ActivityDirection.payable,
  ),
  _activity(
    'h-1',
    ActivityKind.counterpartyCharge,
    ActivityEffect.income,
    '1250.0000',
    '2026-10-28',
    title: 'Hasan Bakkal',
    category: 'Satış geliri',
  ),
];

const _selin = CounterpartyPageData(
  id: 'cp-selin',
  name: 'Selin Kaya',
  scope: _personal,
  owedToYou: '0.0000',
  owedByYou: '0.0000',
  collect: DebtShortcuts(full: '0.0000', overdue: '0.0000'),
  pay: DebtShortcuts(full: '0.0000', overdue: '0.0000'),
);

const _demir = CounterpartyPageData(
  id: 'cp-demir',
  name: 'Toptancı Demir Ltd.',
  scope: _business,
  owedToYou: '0.0000',
  owedByYou: '2200.0000',
  overduePayable: '500.0000',
  nextPayableDueDate: '2026-12-05',
  nextPayableDueAmount: '1700.0000',
  collect: DebtShortcuts(full: '0.0000', overdue: '0.0000'),
  pay: DebtShortcuts(full: '2200.0000', overdue: '500.0000'),
);

const _collectOptions = [
  AppAccountOption(
    id: 'account:kasa',
    name: 'Nakit kasa',
    kind: AppAccountKind.cash,
    scope: 'İşletme',
    amount: '12480.0000',
  ),
  AppAccountOption(
    id: 'account:banka',
    name: 'İşletme hesabı',
    kind: AppAccountKind.bank,
    scope: 'İşletme',
    amount: '48250.0000',
  ),
  AppAccountOption(
    id: 'pos:ana',
    name: 'Ana POS',
    kind: AppAccountKind.pos,
    note: 'İşletme hesabına geçer',
  ),
];

const _payOptions = [
  AppAccountOption(
    id: 'account:banka',
    name: 'İşletme hesabı',
    kind: AppAccountKind.bank,
    scope: 'İşletme',
    amount: '48250.0000',
  ),
  AppAccountOption(
    id: 'card:ana',
    name: 'Ana kart',
    kind: AppAccountKind.card,
    amount: '-4000.0000',
  ),
];

FinancialActivity _activity(
  String id,
  ActivityKind kind,
  ActivityEffect effect,
  String amount,
  String date, {
  required String title,
  String? category,
  String? source,
  ActivityDirection direction = ActivityDirection.receivable,
}) => FinancialActivity(
  activityId: id,
  kind: kind,
  effect: effect,
  sourceGroup: ActivitySourceGroup.counterparty,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: date,
  amount: amount,
  currency: 'TRY',
  title: title,
  categoryName: category,
  sourceName: source,
  direction: direction,
  canCancel: true,
  supportsAttachments: false,
);
