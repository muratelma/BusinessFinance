import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/features/day_close/data/day_close_repository.dart';

/// Teslimdeki sentetik örnekler. Sonuçlar sabittir; bu depo para hesaplamaz.
class HandoffDayCloseRepository implements DayCloseRepositoryContract {
  HandoffDayCloseRepository(
    this.frame, {
    this.name,
    this.invoiceCollectionsLarger = false,
    this.extraSale = false,
  });

  final int frame;
  final String? name;
  final bool invoiceCollectionsLarger;
  bool extraSale;
  DayCloseInput? lastInput;
  final created = <DayCloseInput>[];
  final requests = <DayCloseInput>[];

  List<DayCloseExistingRecord> records(DayCloseInput input) {
    DayCloseExistingRecord row(
      String kind,
      String id,
      String title,
      String amount, {
      bool asked = true,
      String? groupId,
      bool cash = true,
    }) => DayCloseExistingRecord(
      kind: kind,
      id: id,
      isCash: cash,
      date: input.date,
      amount: amount,
      title: title,
      accountName: kind == 'counterparty-charge' || kind == 'obligation'
          ? ''
          : 'Dükkan kasası',
      includedByDefault: !asked && !input.isAdditional,
      included:
          input.recordOverrides['$kind/$id'] ??
          (asked ? null : !input.isAdditional),
      requiresAnswer: asked,
      groupId: groupId,
      groupName: title,
      posDefinitionId: cash ? null : 'ziraat',
    );
    if (frame == 5) return [];
    if (frame == 8) {
      return [
        row('income', 'sale', 'Satış geliri', '250.0000', asked: false),
        row(
          'pos-settlement',
          'card',
          'Ziraat POS satışı',
          '800.0000',
          asked: false,
          cash: false,
        ),
        row('counterparty-payment', 'ahmet', 'Ahmet Bakkal', '300.0000'),
      ];
    }
    if (frame <= 3) {
      return [
        row('income', 'sale', 'Satış geliri', '250.0000', asked: false),
        row(
          'pos-settlement',
          'card',
          'Ziraat POS satışı',
          '800.0000',
          asked: false,
          cash: false,
        ),
        row('counterparty-payment', 'ahmet', 'Ahmet Bakkal', '300.0000'),
        row('counterparty-payment', 'mehmet', 'Mehmet Usta', '120.0000'),
        row('counterparty-charge', 'ayse', 'Ayşe Terzi', '450.0000'),
      ];
    }
    if (frame == 6) {
      return [
        row(
          'obligation',
          'invoice',
          name ?? 'Sentetik fatura',
          '400.0000',
          groupId: 'invoice',
        ),
        row(
          'obligation-settlement',
          'payment',
          name ?? 'Sentetik fatura',
          invoiceCollectionsLarger ? '500.0000' : '400.0000',
          groupId: 'invoice',
        ),
      ];
    }
    return [
      row(
        'counterparty-charge',
        'sale-mehmet',
        name ?? 'Mehmet Usta',
        '500.0000',
        groupId: 'mehmet',
      ),
      row(
        'counterparty-payment',
        'payment-mehmet',
        name ?? 'Mehmet Usta',
        '300.0000',
        groupId: 'mehmet',
      ),
      if (extraSale)
        row(
          'counterparty-charge',
          'other-sale',
          'Diğer satış',
          '100.0000',
          groupId: 'mehmet',
        ),
      if (frame == 7) ...[
        row(
          'counterparty-charge',
          'sale-ahmet',
          'Ahmet Bakkal',
          '300.0000',
          groupId: 'ahmet',
        ),
        row(
          'counterparty-payment',
          'payment-ahmet',
          'Ahmet Bakkal',
          '500.0000',
          groupId: 'ahmet',
        ),
      ],
    ];
  }

  @override
  Future<DayClosePreview> preview(DayCloseInput input) async {
    lastInput = input;
    requests.add(input);
    final listed = records(input);
    final cash = input.cashAmount != null;
    final card = input.posAmounts.containsKey('ziraat');
    final unanswered =
        cash && listed.any((row) => row.requiresAnswer && row.included == null);
    final groups = <DayCloseOverlapGroup>[];
    if (cash && frame >= 4 && frame <= 7) {
      for (final id
          in frame == 6
              ? ['invoice']
              : frame == 7
              ? ['mehmet', 'ahmet']
              : ['mehmet']) {
        final pair = listed.where((row) => row.groupId == id);
        if (!pair.any((row) => row.isDeferredSale && row.included == true) ||
            !pair.any((row) => row.isCollection && row.included == true)) {
          continue;
        }
        final invoice = id == 'invoice';
        final larger = id == 'ahmet' || (invoice && invoiceCollectionsLarger);
        final overlap = input.overlaps[id];
        groups.add(
          DayCloseOverlapGroup(
            groupId: id,
            name:
                name ??
                (invoice
                    ? 'Sentetik fatura'
                    : larger
                    ? 'Ahmet Bakkal'
                    : 'Mehmet Usta'),
            isInvoice: invoice,
            salesAmount: invoice
                ? '400.0000'
                : larger
                ? '300.0000'
                : '500.0000',
            collectionsAmount: invoice
                ? invoiceCollectionsLarger
                      ? '500.0000'
                      : '400.0000'
                : larger
                ? '500.0000'
                : '300.0000',
            maximumOverlap: invoice ? '400.0000' : '300.0000',
            separateAmount: invoice && invoiceCollectionsLarger
                ? '900.0000'
                : '800.0000',
            insideAmount: invoice && !invoiceCollectionsLarger
                ? '400.0000'
                : '500.0000',
            collectionsLarger: larger,
            overlapAmount: overlap,
            deductedAmount: overlap == null
                ? null
                : switch (overlap) {
                    '0.0000' =>
                      invoice && invoiceCollectionsLarger
                          ? '900.0000'
                          : '800.0000',
                    '200.0000' => '600.0000',
                    '400.0000' =>
                      invoiceCollectionsLarger ? '500.0000' : '400.0000',
                    _ => '500.0000',
                  },
          ),
        );
      }
    }
    var deduction = '0.0000';
    var write = input.cashAmount ?? '0.0000';
    var breakdown = const DayCloseCashDeductions();
    if (cash && frame <= 3) {
      final key = ['ahmet', 'mehmet', 'ayse']
          .map(
            (id) =>
                input.recordOverrides['${id == 'ayse' ? 'counterparty-charge' : 'counterparty-payment'}/$id'] ==
                    true
                ? '1'
                : '0',
          )
          .join();
      final values = const {
        '000': ('250.0000', '1420.0000', '0.0000'),
        '100': ('550.0000', '1120.0000', '300.0000'),
        '010': ('370.0000', '1300.0000', '120.0000'),
        '110': ('670.0000', '1000.0000', '420.0000'),
        '001': ('700.0000', '970.0000', '0.0000'),
        '101': ('1000.0000', '670.0000', '300.0000'),
        '011': ('820.0000', '850.0000', '120.0000'),
        '111': ('1120.0000', '550.0000', '420.0000'),
      }[key]!;
      deduction = values.$1;
      write = values.$2;
      breakdown = DayCloseCashDeductions(
        salesAmount: '250.0000',
        collectionsAmount: values.$3,
        creditSalesAmount: key.endsWith('1') ? '450.0000' : '0.0000',
      );
    } else if (cash && frame >= 4 && frame <= 7 && groups.isNotEmpty) {
      final overlap = input.overlaps[frame == 6 ? 'invoice' : 'mehmet'];
      deduction = frame == 7
          ? '1300.0000'
          : groups.first.deductedAmount ?? '0.0000';
      write = frame == 7
          ? '1100.0000'
          : switch (overlap) {
              '0.0000' =>
                frame == 6 && invoiceCollectionsLarger
                    ? '700.0000'
                    : '800.0000',
              '200.0000' => '1000.0000',
              '400.0000' =>
                invoiceCollectionsLarger ? '1100.0000' : '1200.0000',
              _ => '1100.0000',
            };
      breakdown = DayCloseCashDeductions(
        collectionsAmount: frame == 7
            ? '800.0000'
            : frame == 6
            ? invoiceCollectionsLarger
                  ? '500.0000'
                  : '400.0000'
            : '300.0000',
        creditSalesAmount: frame == 7
            ? '800.0000'
            : frame == 6
            ? '0.0000'
            : '500.0000',
        invoicesAmount: frame == 6 ? '400.0000' : '0.0000',
        sharedAmount: frame == 7 ? '300.0000' : overlap ?? '0.0000',
      );
    }
    return DayClosePreview(
      date: input.date,
      currency: 'TRY',
      closedBy: const [],
      cash: DayCloseCashLine(
        stated: cash,
        enteredAmount: input.cashAmount ?? '0.0000',
        deductedAmount: deduction,
        amountToWrite: write,
        deductions: breakdown,
        accountId: 'kasa',
        accountName: 'Dükkan kasası',
        categoryId: 'sales',
        categoryName: 'Satış geliri',
      ),
      posLines: [
        for (final id in ['ziraat', 'yemek', 'ikinci'])
          DayClosePosLine(
            posDefinitionId: id,
            name: id == 'ziraat'
                ? 'Ziraat POS'
                : id == 'yemek'
                ? 'Yemek kartı'
                : 'İkinci POS',
            isDefault: id == 'ziraat',
            accountName: 'Ziraat',
            stated: id == 'ziraat' && card,
            enteredAmount: id == 'ziraat'
                ? input.posAmounts[id] ?? '0.0000'
                : '0.0000',
            deductedAmount: id == 'ziraat' && card && frame == 8
                ? '800.0000'
                : '0.0000',
            amountToWrite: id == 'ziraat' && card ? '500.0000' : '0.0000',
            commissionAmount: id == 'ziraat' && card ? '10.0000' : '0.0000',
            netAmount: id == 'ziraat' && card ? '490.0000' : '0.0000',
            expectedTransferDate: '2026-10-12',
          ),
      ],
      totalEntered: input.totalAmount,
      totalComputed: '0.0000',
      totalDifference: input.totalAmount == null ? null : '1300.0000',
      existingRecords: listed,
      overlapGroups: groups,
      blockerCode: !cash && !card
          ? 'day_closes.amounts_required'
          : unanswered
          ? 'day_closes.records_unanswered'
          : groups.any((group) => group.overlapAmount == null)
          ? 'day_closes.overlap_unanswered'
          : null,
    );
  }

  @override
  Future<DayClose> create({
    required String clientRequestId,
    required DayCloseInput input,
  }) async {
    created.add(input);
    return DayClose(
      id: 'close',
      closedOn: input.date,
      isAdditional: input.isAdditional,
      isCancelled: false,
      incomes: const [],
      settlements: const [],
      cashAmount: '0.0000',
      cardGrossAmount: '0.0000',
      commissionAmount: '0.0000',
      currency: 'TRY',
    );
  }

  @override
  Future<DayCloseOptions> loadOptions() async => const DayCloseOptions(
    cashAccounts: [DataChoice('kasa', 'Dükkan kasası')],
    categories: [DataChoice('sales', 'Satış geliri')],
  );
  @override
  Future<List<DayClose>> list({
    required String from,
    required String to,
  }) async => [];
  @override
  Future<DayClose> get({required String dayCloseId}) async =>
      throw UnimplementedError();
  @override
  Future<DayClose> revert({required String dayCloseId}) async =>
      throw UnimplementedError();
  @override
  Future<DayCloseDay> day({required String date}) async =>
      throw UnimplementedError();
}
