import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/taxes/data/tax_models.dart';
import 'package:business_finance_mobile/features/taxes/data/tax_repository.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_controller.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_plan_detail_page.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_sheets.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_tracking_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Tasarımın `Vergiler` verisi (`design/vergiler-handoff/screenshots`): bugün
/// 29 Eylül 2026; KDV 28 Eylül'de gecikti ve tutarı belli değil, Bağkur yarın
/// ve tanımdan 8.950; Motorlu taşıtlar temmuzda kartla ödendi, eylülde bir
/// toplu ödeme Bağkur'un iki kalemini kapattı.
void main() {
  late TaxController controller;

  Widget page() {
    controller = TaxController(
      _DesignTaxes(),
      now: () => DateTime(2026, 9, 29),
    );
    return TaxTrackingPage(controller: controller);
  }

  BuildContext host(WidgetTester tester) =>
      tester.element(find.text('Vergi takibi'));

  Future<void> shot(
    WidgetTester tester,
    String name, {
    Widget? screen,
    Future<void> Function(WidgetTester tester)? before,
  }) => captureScreen(
    tester,
    name,
    screen ?? page(),
    withNavBar: false,
    pushed: true,
    before: before,
  );

  testWidgets('v2 dolu', (tester) async {
    await shot(tester, 'vergi-03-dolu');
  }, skip: !screenshotsEnabled);

  testWidgets('v3 ödedim', (tester) async {
    await shot(
      tester,
      'vergi-05-odedim',
      before: (tester) async {
        showTaxPaySheet(host(tester), controller, _bagkurPending);
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('v3 ödedim · gecikmiş', (tester) async {
    await shot(
      tester,
      'vergi-06-odedim-gecikmis',
      before: (tester) async {
        showTaxPaySheet(host(tester), controller, _kdvOverdue);
      },
    );
  }, skip: !screenshotsEnabled);

  // Tutar alanının ortalanması: boş, kısa, orta ve uzun tutarla.
  for (final (index, text) in ['', '750', '12500', '1284500,50'].indexed) {
    testWidgets('tutarı gir · $index', (tester) async {
      await shot(
        tester,
        'vergi-tutar-$index',
        before: (tester) async {
          showTaxAmountSheet(host(tester), controller, _kdvOverdue);
          await tester.pump();
          await tester.pump(const Duration(seconds: 1));
          if (text.isNotEmpty) {
            await tester.enterText(find.byType(TextField), text);
          }
        },
      );
    }, skip: !screenshotsEnabled);
  }

  // Cihazdaki hâl: durum çubuğu ve alt gezinme payı varken panelin tepesi
  // başlığı örtmez (harness'in çerçevesinde bu paylar sıfırdır).
  testWidgets('v5 vergi ödemesi ekle · cihaz payları', (tester) async {
    tester.view.padding = const FakeViewPadding(top: 96, bottom: 48);
    tester.view.viewPadding = const FakeViewPadding(top: 96, bottom: 48);
    await shot(
      tester,
      'vergi-08-odeme-ekle-cihaz',
      before: (tester) async {
        showTaxPaymentSheet(host(tester), controller);
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('v5 vergi ödemesi ekle', (tester) async {
    await shot(
      tester,
      'vergi-08-odeme-ekle',
      before: (tester) async {
        showTaxPaymentSheet(host(tester), controller);
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
        await tester.enterText(find.byType(TextField).first, '17400');
        await tester.tap(find.byKey(const ValueKey('tax-source-null')));
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
        await tester.tap(find.text('Nakit kasa').last);
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('v8 bekleyen ayrıntı', (tester) async {
    await shot(
      tester,
      'vergi-12-bekleyen',
      before: (tester) async {
        showTaxPendingSheet(
          host(tester),
          controller,
          _kdvOverdue,
          onOpenPlan: () {},
        );
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('v9 ödenmiş', (tester) async {
    await shot(
      tester,
      'vergi-13-odenmis',
      before: (tester) async {
        showTaxPaidSheet(host(tester), controller, _mtvPayment);
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('v9 geri alma onayı', (tester) async {
    await shot(
      tester,
      'vergi-14-geri-alma',
      before: (tester) async {
        showTaxPaidSheet(host(tester), controller, _mtvPayment);
        await tester.pump();
        await tester.pump(const Duration(seconds: 1));
        await tester.tap(find.text('Ödemeyi geri al').last);
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('v9 toplu ödeme', (tester) async {
    await shot(
      tester,
      'vergi-15-toplu',
      before: (tester) async {
        showTaxPaidSheet(host(tester), controller, _bulkPayment);
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('v10 duraklat · sil', (tester) async {
    controller = TaxController(
      _DesignTaxes(),
      now: () => DateTime(2026, 9, 29),
    );
    await controller.load();
    await shot(
      tester,
      'vergi-18-duraklat-sil',
      screen: TaxPlanDetailPage(controller: controller, planId: 'bagkur'),
      before: (tester) async {
        await tester.drag(find.byType(ListView), const Offset(0, -600));
      },
    );
  }, skip: !screenshotsEnabled);
}

TaxPlan _plan(
  String id,
  String name,
  TaxKind kind, {
  int day = 31,
  String? amount,
  String? accountId,
  TaxRhythm rhythm = TaxRhythm.monthly,
  List<int> months = const [],
  String next = '2026-09-30',
  TransactionScope scope = TransactionScope.business,
  bool active = true,
}) => TaxPlan(
  id: id,
  name: name,
  taxKind: kind,
  rhythm: rhythm,
  startDate: '2026-06-30',
  categoryId: 'vergi',
  scope: scope,
  isActive: active,
  currency: 'TRY',
  amount: amount,
  accountId: accountId,
  dayOfMonth: day,
  months: months,
  nextDate: next,
);

final _plans = [
  _plan(
    'bagkur',
    'Bağkur',
    TaxKind.socialSecurityPremium,
    amount: '8950.0000',
    accountId: 'dukkan',
  ),
  _plan('kdv', 'KDV', TaxKind.vatReturn, day: 28, next: '2026-10-28'),
  _plan(
    'gecici',
    'Geçici vergi',
    TaxKind.advanceTax,
    day: 17,
    rhythm: TaxRhythm.selectedMonths,
    months: [2, 5, 8, 11],
    next: '2026-11-17',
  ),
  _plan(
    'mtv',
    'Motorlu taşıtlar',
    TaxKind.motorVehicleTax,
    rhythm: TaxRhythm.selectedMonths,
    months: [1, 7],
    amount: '2180.0000',
    next: '2027-01-31',
    scope: TransactionScope.personal,
  ),
  _plan(
    'tabela',
    'Tabela',
    TaxKind.advertisingTax,
    rhythm: TaxRhythm.yearly,
    months: [1],
    next: '2027-01-31',
    active: false,
  ),
];

PlannedActivity _pending(
  String plan,
  String title,
  String kind,
  String due,
  PlannedTiming timing, {
  String? amount,
  String? sourceId,
  String? sourceName,
}) => PlannedActivity(
  plannedActivityId: '$plan-$due',
  plannedKind: PlannedKind.recurringOccurrence,
  effect: ActivityEffect.expense,
  timing: timing,
  readiness: PlannedReadiness.ready,
  actionKind: PlannedAction.realize,
  dueDate: due,
  amount: amount,
  currency: 'TRY',
  title: title,
  isProjected: true,
  isPaymentObligation: true,
  actionTargetId: plan,
  recurringTransactionId: plan,
  taxKind: kind,
  sourceId: sourceId,
  sourceName: sourceName,
);

final _kdvOverdue = _pending(
  'kdv',
  'KDV',
  'vat-return',
  '2026-09-28',
  PlannedTiming.overdue,
);

final _bagkurPending = _pending(
  'bagkur',
  'Bağkur',
  'social-security-premium',
  '2026-09-30',
  PlannedTiming.upcoming,
  amount: '8950.0000',
  sourceId: 'dukkan',
  sourceName: 'Dükkan hesabı',
);

const _mtvPayment = TaxPayment(
  paymentId: 'mtv-temmuz',
  isCard: true,
  sourceId: 'bonus',
  sourceName: 'Bonus',
  categoryName: 'SGK ve vergi ödemesi',
  amount: '2180.0000',
  currency: 'TRY',
  paidOn: '2026-07-31',
  scope: TransactionScope.personal,
  description: 'Motorlu taşıtlar',
  realizedItem: TaxSettledItem(
    occurrenceId: 'mtv-1',
    recurringTransactionId: 'mtv',
    scheduledDate: '2026-07-31',
    name: 'Motorlu taşıtlar',
    taxKind: TaxKind.motorVehicleTax,
  ),
  closedItems: [],
  isCancelled: false,
);

const _bulkPayment = TaxPayment(
  paymentId: 'toplu',
  isCard: false,
  sourceId: 'kasa',
  sourceName: 'Nakit kasa',
  categoryName: 'Vergi ödemesi',
  amount: '12500.0000',
  currency: 'TRY',
  paidOn: '2026-09-15',
  scope: TransactionScope.business,
  description: 'Temmuz–Ağustos Bağkur',
  closedItems: [
    TaxSettledItem(
      occurrenceId: 'bagkur-7',
      recurringTransactionId: 'bagkur',
      scheduledDate: '2026-07-31',
      name: 'Bağkur',
      taxKind: TaxKind.socialSecurityPremium,
    ),
    TaxSettledItem(
      occurrenceId: 'bagkur-8',
      recurringTransactionId: 'bagkur',
      scheduledDate: '2026-08-31',
      name: 'Bağkur',
      taxKind: TaxKind.socialSecurityPremium,
    ),
  ],
  isCancelled: false,
);

class _DesignTaxes implements TaxRepositoryContract {
  @override
  Future<TaxOverview> loadOverview({required String asOfDate}) async =>
      TaxOverview(
        asOfDate: asOfDate,
        plans: _plans,
        pending: [
          _kdvOverdue,
          _bagkurPending,
          _pending(
            'kdv',
            'KDV',
            'vat-return',
            '2026-10-28',
            PlannedTiming.upcoming,
          ),
        ],
        pendingTotal: '8950.0000',
        pendingUnknownAmountCount: 2,
        recentPayments: const [_bulkPayment, _mtvPayment],
        hasMorePayments: false,
      );

  @override
  Future<TaxOptions> loadOptions() async => const TaxOptions(
    accounts: [
      TaxChoice(id: 'dukkan', name: 'Dükkan hesabı'),
      TaxChoice(id: 'kasa', name: 'Nakit kasa'),
    ],
    cards: [TaxChoice(id: 'bonus', name: 'Bonus')],
    taxCategories: [TaxChoice(id: 'vergi', name: 'Vergi ödemesi')],
  );

  @override
  Future<TaxPlanDetail> loadPlanDetail(
    String planId, {
    required String asOfDate,
  }) async => TaxPlanDetail(
    plan: _plans.first,
    upcoming: [
      _bagkurPending,
      _pending(
        'bagkur',
        'Bağkur',
        'social-security-premium',
        '2026-10-31',
        PlannedTiming.upcoming,
        amount: '8950.0000',
      ),
      _pending(
        'bagkur',
        'Bağkur',
        'social-security-premium',
        '2026-11-30',
        PlannedTiming.upcoming,
        amount: '8950.0000',
      ),
    ],
    history: const [
      TaxHistoryItem(
        occurrenceId: 'bagkur-8',
        scheduledDate: '2026-08-31',
        isClosed: true,
        amount: '8950.0000',
        payment: _bulkPayment,
      ),
      TaxHistoryItem(
        occurrenceId: 'bagkur-7',
        scheduledDate: '2026-07-31',
        isClosed: true,
        amount: '8950.0000',
        payment: _bulkPayment,
      ),
    ],
  );

  @override
  Future<List<TaxSuggestion>> loadSuggestions() async => const [];

  @override
  Future<TaxPaymentPage> listPayments({
    required int skip,
    required int take,
  }) async => const TaxPaymentPage(items: [], hasMore: false);

  @override
  Future<void> pay({
    required String planId,
    required String scheduledDate,
    required String amount,
    required String paidOn,
    String? accountId,
    String? creditCardId,
  }) async {}

  @override
  Future<void> setAmount({
    required String planId,
    required String scheduledDate,
    required String amount,
  }) async {}

  @override
  Future<TaxPayment> createPayment({
    required String clientRequestId,
    required String amount,
    required String paidOn,
    required String categoryId,
    required List<({String planId, String scheduledDate})> closes,
    String? accountId,
    String? creditCardId,
    String? note,
  }) async => _bulkPayment;

  @override
  Future<void> undoPayment(String paymentId) async {}

  @override
  Future<void> createPlan(TaxPlanInput input) async {}

  @override
  Future<void> createPlans(List<TaxPlanInput> inputs) async {}

  @override
  Future<void> updatePlan(String planId, TaxPlanInput input) async {}

  @override
  Future<void> setPlanActive(String planId, bool isActive) async {}

  @override
  Future<void> deletePlan(String planId) async {}
}
