import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/financial_data_changes.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/taxes/data/tax_models.dart';
import 'package:business_finance_mobile/features/taxes/data/tax_repository.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_controller.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_tracking_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

/// Vergi takibi (ADR 0018, Aşama 06.3 Grup 3): ilk kullanım, dolu hâl,
/// "Ödedim" ve toplu ödeme akışları, durum hâlleri.
void main() {
  final today = DateTime(2026, 9, 29);

  group('ilk kullanım', () {
    testWidgets('tanım yokken ana eylem tek tutarla ödemedir', (tester) async {
      await _pump(tester, _Repository(overview: _overview(plans: const [])));

      expect(find.text('Ödediğiniz vergiyi tek tutarla yazın'), findsOneWidget);
      expect(find.text('Vergilerimi tanımla'), findsOneWidget);
      // Ana eylem altta, her hâlde görünür.
      expect(find.text('Vergi ödemesi ekle'), findsOneWidget);
      expect(find.text('Bekleyenler'), findsNothing);
    });

    testWidgets('gider formundan girilen vergi Ödenenler\'de görünür', (
      tester,
    ) async {
      await _pump(
        tester,
        _Repository(
          overview: _overview(plans: const [], payments: [_payment()]),
        ),
      );

      expect(find.text('Ödenenler'), findsOneWidget);
      expect(find.text('Muhtasar'), findsOneWidget);
    });
  });

  group('dolu hâl', () {
    testWidgets('bekleyenler, gecikenler dahil toplam ve belli olmayanlar', (
      tester,
    ) async {
      await _pump(tester, _Repository(overview: _overview()));

      expect(find.text('Gecikenler ve 30 gün'), findsOneWidget);
      expect(find.text('1 gecikti'), findsOneWidget);
      // Toplam sunucudan: istemci bekleyenleri toplamaz.
      expect(find.text('Ödenecek'), findsOneWidget);
      expect(find.text('₺8.950,00'), findsWidgets);
      expect(find.text('Tutarı belli olmayan'), findsOneWidget);
      expect(find.text('2 ödeme'), findsOneWidget);
      expect(find.text('Tutar ödemede girilecek'), findsNWidgets(2));
      expect(find.text('Ödedim'), findsNWidgets(3));
      expect(find.text('Vergilerim'), findsOneWidget);
    });

    testWidgets('duraklatılmış ve şahsi vergi etiketlenir', (tester) async {
      await _pump(
        tester,
        _Repository(
          overview: _overview(
            plans: [
              _plan(id: 'mtv', name: 'Motorlu taşıtlar', personal: true),
              _plan(id: 'tabela', name: 'Tabela', active: false),
            ],
          ),
        ),
        hasBusiness: true,
      );

      expect(find.text('Şahsi'), findsOneWidget);
      expect(find.text('Duraklatıldı'), findsOneWidget);
    });
  });

  group('Ödedim', () {
    testWidgets('tutar, ödeme günü ve kaynakla öder', (tester) async {
      final repository = _Repository(overview: _overview());
      await _pump(tester, repository);

      // Bağkur: tanımda tutar ve kaynak var, yarın vadeli.
      await tester.tap(find.text('Ödedim').at(1));
      await tester.pumpAndSettle();

      expect(find.text('Ödenen tutar'), findsOneWidget);
      expect(find.text('Tanımdan geldi.'), findsOneWidget);
      await tester.tap(find.text('Ödemeyi kaydet'));
      await tester.pumpAndSettle();

      expect(repository.paid, hasLength(1));
      final call = repository.paid.single;
      expect(call.planId, 'bagkur');
      expect(call.scheduledDate, '2026-09-30');
      expect(call.amount, '8950.0000');
      expect(call.paidOn, '2026-09-29');
      expect(call.accountId, 'dukkan');
      expect(call.creditCardId, isNull);
    });

    testWidgets('gecikmiş kalemde tutar boş gelir ve istenir', (tester) async {
      final repository = _Repository(overview: _overview());
      await _pump(tester, repository);

      await tester.tap(find.text('Ödedim').first);
      await tester.pumpAndSettle();
      await tester.tap(find.text('Ödemeyi kaydet'));
      await tester.pumpAndSettle();

      expect(find.text('Ödediğiniz tutarı yazın.'), findsOneWidget);
      expect(find.text('Hesap ya da kart seçin.'), findsOneWidget);
      expect(repository.paid, isEmpty);
    });

    testWidgets('sunucu hatası panelde kalır', (tester) async {
      final repository = _Repository(
        overview: _overview(),
        payError: const ApiException(
          code: 'recurring.paid_on_in_future',
          message: 'Ödeme günü ileri bir tarih olamaz.',
          statusCode: 400,
        ),
      );
      await _pump(tester, repository);

      await tester.tap(find.text('Ödedim').at(1));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Ödemeyi kaydet'));
      await tester.pumpAndSettle();

      expect(find.text('Ödeme günü ileri bir tarih olamaz.'), findsOneWidget);
      expect(find.text('Ödenen tutar'), findsOneWidget);
    });
  });

  testWidgets('Tutarı gir kalemin paneline yeni tutarla döner', (tester) async {
    final repository = _Repository(overview: _overview());
    await _pump(tester, repository);

    // Gecikmiş KDV: tutarı belli değil.
    await tester.tap(find.text('Tutar ödemede girilecek').first);
    await tester.pumpAndSettle();
    expect(find.text('Tutar belli değil'), findsOneWidget);

    await tester.tap(find.text('Tutarı gir'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextField).first, '2900');
    await tester.tap(find.text('Kaydet'));
    await tester.pumpAndSettle();

    expect(repository.amounts, [
      (planId: 'kdv', scheduledDate: '2026-09-28', amount: '2900.0000'),
    ]);
    // Kalemin paneli yeni tutarla geri açıldı; "Ödedim" elinin altında.
    expect(find.text('Tutarı gir'), findsOneWidget);
    expect(find.text('Tutar belli değil'), findsNothing);
    expect(find.text('₺2.900,00'), findsOneWidget);
  });

  testWidgets('toplu ödemede vadesi gelenler seçili gelir', (tester) async {
    final repository = _Repository(overview: _overview());
    await _pump(tester, repository);

    await tester.tap(find.text('Vergi ödemesi ekle'));
    await tester.pumpAndSettle();

    expect(find.text('Bu ödeme hangilerini kapatıyor?'), findsOneWidget);
    await tester.enterText(find.byType(TextField).first, '12500');
    await tester.tap(find.byKey(const ValueKey('tax-source-null')));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Nakit kasa').last);
    await tester.pumpAndSettle();
    await tester.ensureVisible(find.text('Ödemeyi kaydet'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Ödemeyi kaydet'));
    await tester.pumpAndSettle();

    final call = repository.payments.single;
    expect(call.amount, '12500.0000');
    expect(call.accountId, 'kasa');
    expect(call.categoryId, 'vergi');
    // Yalnız gecikmiş KDV vadesi geldi; yarınki Bağkur seçili gelmez.
    expect(call.closes, [(planId: 'kdv', scheduledDate: '2026-09-28')]);
  });

  testWidgets('oturum düşünce yetkisiz görünümü çıkar', (tester) async {
    await _pump(
      tester,
      _Repository(
        overview: _overview(),
        loadError: const ApiException(
          code: 'authentication.unauthorized',
          message: 'Oturum',
          statusCode: 401,
        ),
      ),
    );

    expect(find.text('Vergi ödemesi ekle'), findsNothing);
    expect(find.textContaining('Oturum'), findsWidgets);
  });

  testWidgets('okuma hatası yeniden deneme sunar', (tester) async {
    await _pump(
      tester,
      _Repository(
        overview: _overview(),
        loadError: const ApiException(
          code: 'network.unavailable',
          message: 'Bağlantı yok.',
        ),
      ),
    );

    expect(find.text('Bağlantı yok.'), findsOneWidget);
    expect(find.text('Tekrar dene'), findsOneWidget);
  });

  testWidgets('2.0× yazıda taşma olmaz', (tester) async {
    await _pump(tester, _Repository(overview: _overview()), textScale: 2);

    expect(tester.takeException(), isNull);
    expect(find.text('Gecikenler ve 30 gün'), findsOneWidget);
  });

  test('ödeme değişikliği bekleyenleri ve kasayı yeniler', () async {
    final changes = FinancialDataChanges();
    final repository = _Repository(overview: _overview());
    final controller = TaxController(
      repository,
      changes: changes,
      now: () => today,
    );
    await controller.load();
    final planning = changes.planningRevision;
    final accounts = changes.accountsRevision;

    await controller.undoPayment(_payment());
    await Future<void>.delayed(Duration.zero);

    expect(changes.planningRevision, planning + 1);
    expect(changes.accountsRevision, accounts + 1);
    expect(repository.loads, 2);
    controller.dispose();
  });
}

Future<void> _pump(
  WidgetTester tester,
  _Repository repository, {
  bool hasBusiness = false,
  double textScale = 1,
}) async {
  tester.view.physicalSize = const Size(412 * 3, 1400 * 3);
  tester.view.devicePixelRatio = 3;
  addTearDown(tester.view.reset);
  final scope = ScopeController(
    store: _ScopeStore(hasBusiness),
    readHasBusiness: () async => hasBusiness,
  );
  await scope.ensureLoaded();
  final controller = TaxController(
    repository,
    now: () => DateTime(2026, 9, 29),
  );
  await tester.pumpWidget(
    ChangeNotifierProvider<ScopeController>.value(
      value: scope,
      child: MaterialApp(
        theme: AppTheme.light(),
        builder: (context, child) => MediaQuery(
          data: MediaQuery.of(
            context,
          ).copyWith(textScaler: TextScaler.linear(textScale)),
          child: child!,
        ),
        home: TaxTrackingPage(controller: controller),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

TaxOverview _overview({List<TaxPlan>? plans, List<TaxPayment>? payments}) {
  final defined =
      plans ??
      [
        _plan(id: 'kdv', name: 'KDV', day: 28),
        _plan(
          id: 'bagkur',
          name: 'Bağkur',
          amount: '8950.0000',
          accountId: 'dukkan',
        ),
      ];
  return TaxOverview(
    asOfDate: '2026-09-29',
    plans: defined,
    pending: defined.isEmpty
        ? const []
        : [
            _pending(
              plan: 'kdv',
              title: 'KDV',
              due: '2026-09-28',
              timing: PlannedTiming.overdue,
            ),
            _pending(
              plan: 'bagkur',
              title: 'Bağkur',
              due: '2026-09-30',
              timing: PlannedTiming.upcoming,
              amount: '8950.0000',
              sourceId: 'dukkan',
              sourceName: 'Dükkan hesabı',
            ),
            _pending(
              plan: 'kdv',
              title: 'KDV',
              due: '2026-10-28',
              timing: PlannedTiming.upcoming,
            ),
          ],
    pendingTotal: '8950.0000',
    pendingUnknownAmountCount: defined.isEmpty ? 0 : 2,
    recentPayments: payments ?? [_payment()],
    hasMorePayments: false,
  );
}

TaxPlan _plan({
  required String id,
  required String name,
  int day = 31,
  String? amount,
  String? accountId,
  bool personal = false,
  bool active = true,
}) => TaxPlan(
  id: id,
  name: name,
  taxKind: TaxKind.socialSecurityPremium,
  rhythm: TaxRhythm.monthly,
  startDate: '2026-09-${day == 31 ? 30 : day}',
  categoryId: 'vergi',
  scope: personal ? TransactionScope.personal : TransactionScope.business,
  isActive: active,
  currency: 'TRY',
  amount: amount,
  accountId: accountId,
  dayOfMonth: day,
  nextDate: '2026-09-30',
);

PlannedActivity _pending({
  required String plan,
  required String title,
  required String due,
  required PlannedTiming timing,
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
  taxKind: 'social-security-premium',
  sourceId: sourceId,
  sourceName: sourceName,
);

TaxPayment _payment() => const TaxPayment(
  paymentId: 'odeme',
  isCard: false,
  sourceId: 'dukkan',
  sourceName: 'Dükkan hesabı',
  categoryName: 'SGK ve vergi ödemesi',
  amount: '3240.0000',
  currency: 'TRY',
  paidOn: '2026-09-26',
  scope: TransactionScope.business,
  description: 'Muhtasar',
  closedItems: [],
  isCancelled: false,
);

typedef _PayCall = ({
  String planId,
  String scheduledDate,
  String amount,
  String paidOn,
  String? accountId,
  String? creditCardId,
});

typedef _PaymentCall = ({
  String amount,
  String? accountId,
  String categoryId,
  List<({String planId, String scheduledDate})> closes,
});

class _Repository implements TaxRepositoryContract {
  _Repository({required this.overview, this.loadError, this.payError});

  final TaxOverview overview;
  final ApiException? loadError;
  final ApiException? payError;
  final paid = <_PayCall>[];
  final payments = <_PaymentCall>[];
  final amounts = <({String planId, String scheduledDate, String amount})>[];
  int loads = 0;

  @override
  Future<TaxOverview> loadOverview({required String asOfDate}) async {
    loads++;
    if (loadError != null) throw loadError!;
    return overview;
  }

  @override
  Future<TaxOptions> loadOptions() async => const TaxOptions(
    accounts: [
      TaxChoice(id: 'dukkan', name: 'Dükkan hesabı'),
      TaxChoice(id: 'kasa', name: 'Nakit kasa'),
    ],
    cards: [TaxChoice(id: 'bonus', name: 'Bonus')],
    taxCategories: [TaxChoice(id: 'vergi', name: 'SGK ve vergi ödemesi')],
  );

  @override
  Future<void> pay({
    required String planId,
    required String scheduledDate,
    required String amount,
    required String paidOn,
    String? accountId,
    String? creditCardId,
  }) async {
    if (payError != null) throw payError!;
    paid.add((
      planId: planId,
      scheduledDate: scheduledDate,
      amount: amount,
      paidOn: paidOn,
      accountId: accountId,
      creditCardId: creditCardId,
    ));
  }

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
  }) async {
    payments.add((
      amount: amount,
      accountId: accountId,
      categoryId: categoryId,
      closes: closes,
    ));
    return _payment();
  }

  @override
  Future<void> undoPayment(String paymentId) async {}

  @override
  Future<List<TaxSuggestion>> loadSuggestions() async => const [];

  @override
  Future<TaxPlanDetail> loadPlanDetail(
    String planId, {
    required String asOfDate,
  }) async => throw UnimplementedError();

  @override
  Future<TaxPaymentPage> listPayments({
    required int skip,
    required int take,
  }) async => const TaxPaymentPage(items: [], hasMore: false);

  @override
  Future<void> setAmount({
    required String planId,
    required String scheduledDate,
    required String amount,
  }) async {
    amounts.add((planId: planId, scheduledDate: scheduledDate, amount: amount));
  }

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

class _ScopeStore implements ScopeStore {
  _ScopeStore(this.hasBusiness);

  final bool hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => null;

  @override
  Future<void> writeScope(TransactionScope? scope) async {}

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async {}

  @override
  Future<void> clear() async {}
}
