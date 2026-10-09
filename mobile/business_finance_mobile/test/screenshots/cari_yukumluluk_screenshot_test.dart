import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_repository.dart';
import 'package:business_finance_mobile/features/counterparties/presentation/counterparties_page.dart';
import 'package:business_finance_mobile/features/obligations/data/obligation_repository.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligation_controller.dart';
import 'package:business_finance_mobile/features/obligations/presentation/obligations_page.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// Cari kartında fazla tahsilatın yazılışı, kişinin bekleyen faturaları ve
/// yükümlülüğün iptali (9 Ekim 2026). Veri sentetiktir.
void main() {
  testWidgets('cari 01 fazla tahsilat', (tester) async {
    await captureScreen(
      tester,
      'cari-01-fazla-tahsilat',
      CounterpartiesPage(
        repository: _Counterparties(
          const CounterpartySummary(
            id: 'cp-1',
            name: 'Örnek Elektrik Dağıtım A.Ş.',
            isActive: true,
            receivable: '-300.0000',
            payable: '0.0000',
            owedToYou: '0.0000',
            owedByYou: '300.0000',
            net: '-300.0000',
            isSettled: false,
          ),
          activities: [
            _activity(
              'a-2',
              ActivityKind.counterpartySettlement,
              '2500.0000',
              'Örnek Elektrik Dağıtım A.Ş.',
            ),
            _activity(
              'a-1',
              ActivityKind.counterpartyCharge,
              '2200.0000',
              'Hizmet geliri',
            ),
          ],
        ),
      ),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Örnek Elektrik Dağıtım A.Ş.'));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('cari 02 bekleyen faturalar', (tester) async {
    await captureScreen(
      tester,
      'cari-02-bekleyen-faturalar',
      CounterpartiesPage(
        repository: _Counterparties(
          const CounterpartySummary(
            id: 'cp-1',
            name: 'Tedarikçi A',
            isActive: true,
            receivable: '0.0000',
            payable: '500.0000',
            net: '-500.0000',
            isSettled: false,
            openPayableObligations: '1412.6000',
          ),
          pending: const [
            ObligationItem(
              id: 'ob-1',
              direction: 'payable',
              amount: '1000.0000',
              currency: 'TRY',
              issueDate: '2026-10-01',
              dueDate: '2026-10-05',
              status: 'open',
              isOverdue: true,
              counterpartyId: 'cp-1',
              description: 'Eylül mal faturası',
            ),
            ObligationItem(
              id: 'ob-2',
              direction: 'payable',
              amount: '412.6000',
              currency: 'TRY',
              issueDate: '2026-10-01',
              dueDate: '2026-10-20',
              status: 'open',
              isOverdue: false,
              counterpartyId: 'cp-1',
              description: 'Nakliye faturası',
            ),
          ],
          activities: [
            _activity(
              'a-1',
              ActivityKind.counterpartyCharge,
              '500.0000',
              'Ticari mal alımı',
              effect: ActivityEffect.expense,
            ),
          ],
        ),
      ),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Tedarikçi A'));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yükümlülük 01 kapanış paneli', (tester) async {
    await captureScreen(
      tester,
      'yukumluluk-01-panel-iptal-dugmesi',
      ObligationsPage(controller: ObligationListController(_Obligations())),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Örnek Elektrik Dağıtım A.Ş.').first);
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yükümlülük 02 açık kaydın iptali', (tester) async {
    await captureScreen(
      tester,
      'yukumluluk-02-acik-kayit-onay',
      ObligationsPage(controller: ObligationListController(_Obligations())),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Örnek Elektrik Dağıtım A.Ş.').first);
        for (var i = 0; i < 10; i++) {
          await tester.pump(const Duration(milliseconds: 100));
        }
        await tester.tap(find.text('Kaydı iptal et'));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yükümlülük 03 ödenmiş kaydın iptali', (tester) async {
    await captureScreen(
      tester,
      'yukumluluk-03-odenmis-kayit-onay',
      ObligationsPage(controller: ObligationListController(_Obligations())),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Kapanan'));
        for (var i = 0; i < 10; i++) {
          await tester.pump(const Duration(milliseconds: 100));
        }
        await tester.tap(find.text('Toptancı Zeynep'));
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('yükümlülük 04 kapanan sekmesi', (tester) async {
    await captureScreen(
      tester,
      'yukumluluk-04-kapanan',
      ObligationsPage(controller: ObligationListController(_Obligations())),
      withNavBar: false,
      before: (tester) async {
        await tester.tap(find.text('Kapanan'));
      },
    );
  }, skip: !screenshotsEnabled);
}

FinancialActivity _activity(
  String id,
  ActivityKind kind,
  String amount,
  String title, {
  ActivityEffect effect = ActivityEffect.income,
}) => FinancialActivity(
  activityId: id,
  kind: kind,
  effect: kind == ActivityKind.counterpartySettlement
      ? ActivityEffect.neutral
      : effect,
  sourceGroup: ActivitySourceGroup.counterparty,
  origin: ActivityOrigin.manual,
  status: ActivityStatus.realized,
  activityDate: '2026-10-09',
  amount: amount,
  currency: 'TRY',
  title: title,
  canCancel: true,
  supportsAttachments: false,
);

class _Counterparties implements CounterpartyRepositoryContract {
  _Counterparties(
    this.person, {
    this.pending = const [],
    this.activities = const [],
  });

  final CounterpartySummary person;
  final List<ObligationItem> pending;
  final List<FinancialActivity> activities;

  @override
  Future<CounterpartiesSnapshot> load(
    CounterpartyBalanceFilter filter,
    String asOfDate,
  ) async => CounterpartiesSnapshot(
    counterparties: [person],
    accounts: const [DataChoice('account-1', 'Ziraat vadesiz')],
    categories: const [
      DataChoice('category-income', 'Hizmet geliri', type: 'income'),
    ],
  );

  @override
  Future<CounterpartyDetail> loadDetail(
    String counterpartyId,
    String asOfDate,
  ) async => CounterpartyDetail(
    counterparty: person,
    activities: activities,
    agreements: const [],
    pendingObligations: pending,
  );

  @override
  Future<void> create(String name, String? note) async {}

  @override
  Future<void> update(
    String counterpartyId, {
    required String name,
    required bool isActive,
    String? note,
  }) async {}

  @override
  Future<void> delete(String counterpartyId) async {}

  @override
  Future<void> addCharge(
    String counterpartyId,
    Map<String, Object?> input,
  ) async {}

  @override
  Future<void> addPayment(
    String counterpartyId,
    Map<String, Object?> input,
  ) async {}
}

class _Obligations implements ObligationRepositoryContract {
  @override
  Future<List<ObligationItem>> list({required String asOfDate}) async => const [
    ObligationItem(
      id: 'ob-1',
      direction: 'payable',
      amount: '412.6000',
      currency: 'TRY',
      issueDate: '2026-10-01',
      dueDate: '2026-10-20',
      status: 'open',
      isOverdue: false,
      counterpartyName: 'Örnek Elektrik Dağıtım A.Ş.',
      categoryName: 'İşyeri faturaları',
    ),
    ObligationItem(
      id: 'ob-2',
      direction: 'payable',
      amount: '412.6000',
      currency: 'TRY',
      issueDate: '2026-10-01',
      dueDate: '2026-10-20',
      status: 'open',
      isOverdue: false,
      counterpartyName: 'Örnek Elektrik Dağıtım A.Ş.',
      categoryName: 'İşyeri faturaları',
    ),
    ObligationItem(
      id: 'ob-3',
      direction: 'payable',
      amount: '1250.0000',
      currency: 'TRY',
      issueDate: '2026-09-20',
      dueDate: '2026-10-05',
      status: 'settled',
      isOverdue: false,
      counterpartyName: 'Toptancı Zeynep',
      categoryName: 'Ticari mal alımı',
    ),
    ObligationItem(
      id: 'ob-4',
      direction: 'payable',
      amount: '90.0000',
      currency: 'TRY',
      issueDate: '2026-09-28',
      dueDate: '2026-10-10',
      status: 'cancelled',
      isOverdue: false,
      counterpartyName: 'Kırtasiye Ali',
      categoryName: 'Ofis giderleri',
    ),
  ];

  @override
  Future<List<ObligationAccount>> loadActiveAccounts() async => const [
    ObligationAccount(id: 'account-1', name: 'Ziraat vadesiz'),
  ];

  @override
  Future<void> settle({
    required String obligationId,
    required String? accountId,
    required String settlementDate,
    Map<String, Object?>? card,
  }) async {}

  @override
  Future<void> cancel(String obligationId) async {}

  @override
  Future<ObligationOptions> loadOptions({required String categoryType}) async =>
      const ObligationOptions(categories: [], counterparties: []);

  @override
  Future<void> create(Map<String, Object?> input) async {}
}
