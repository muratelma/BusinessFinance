import '../../../core/models/json_readers.dart';

class FinanceChoice {
  const FinanceChoice({required this.id, required this.name});
  final String id;
  final String name;
}

class TransferItem {
  const TransferItem({
    required this.id,
    required this.sourceAccountId,
    required this.destinationAccountId,
    required this.amount,
    required this.currency,
    required this.date,
    required this.description,
    required this.isCancelled,
  });
  final String id;
  final String sourceAccountId;
  final String destinationAccountId;
  final String amount;
  final String currency;
  final String date;
  final String? description;
  final bool isCancelled;

  factory TransferItem.fromJson(Map<String, dynamic> json) => TransferItem(
    id: JsonReaders.string(json, 'id'),
    sourceAccountId: JsonReaders.string(json, 'sourceAccountId'),
    destinationAccountId: JsonReaders.string(json, 'destinationAccountId'),
    amount: JsonReaders.money(json, 'amount'),
    currency: JsonReaders.string(json, 'currency'),
    date: JsonReaders.date(json, 'transferDate'),
    description: JsonReaders.nullableString(json, 'description'),
    isCancelled: JsonReaders.boolean(json, 'isCancelled'),
  );
}

class CreditCardItem {
  const CreditCardItem({
    required this.id,
    required this.name,
    required this.limit,
    required this.currentDebt,
    required this.availableLimit,
    required this.currency,
    required this.statementClosingDay,
    required this.paymentDueDay,
    required this.minimumPaymentRate,
    required this.isActive,
  });
  final String id;
  final String name;
  final String limit;
  final String currentDebt;
  final String availableLimit;
  final String currency;
  final int statementClosingDay;
  final int paymentDueDay;

  /// Asgari ödeme yüzdesi ("20.0000" = %20), oran değil yüzde.
  final String minimumPaymentRate;
  final bool isActive;

  factory CreditCardItem.fromJson(Map<String, dynamic> json) => CreditCardItem(
    id: JsonReaders.string(json, 'id'),
    name: JsonReaders.string(json, 'name'),
    limit: JsonReaders.money(json, 'limit'),
    currentDebt: JsonReaders.money(json, 'currentDebt'),
    availableLimit: JsonReaders.money(json, 'availableLimit'),
    currency: JsonReaders.string(json, 'currency'),
    statementClosingDay: JsonReaders.integer(json, 'statementClosingDay'),
    paymentDueDay: JsonReaders.integer(json, 'paymentDueDay'),
    minimumPaymentRate: JsonReaders.money(json, 'minimumPaymentRate'),
    isActive: JsonReaders.boolean(json, 'isActive'),
  );
}

class CardChargeItem {
  const CardChargeItem({
    required this.id,
    required this.amount,
    required this.currency,
    required this.date,
    required this.description,
    required this.isCancelled,
  });
  final String id;
  final String amount;
  final String currency;
  final String date;
  final String? description;
  final bool isCancelled;

  factory CardChargeItem.fromJson(Map<String, dynamic> json) => CardChargeItem(
    id: JsonReaders.string(json, 'id'),
    amount: JsonReaders.money(json, 'amount'),
    currency: JsonReaders.string(json, 'currency'),
    date: JsonReaders.date(json, 'chargeDate'),
    description: JsonReaders.nullableString(json, 'description'),
    isCancelled: JsonReaders.boolean(json, 'isCancelled'),
  );
}

class CardPaymentItem {
  const CardPaymentItem({
    required this.id,
    required this.amount,
    required this.currency,
    required this.date,
    required this.description,
    required this.isCancelled,
  });
  final String id;
  final String amount;
  final String currency;
  final String date;
  final String? description;
  final bool isCancelled;

  factory CardPaymentItem.fromJson(Map<String, dynamic> json) =>
      CardPaymentItem(
        id: JsonReaders.string(json, 'id'),
        amount: JsonReaders.money(json, 'amount'),
        currency: JsonReaders.string(json, 'currency'),
        date: JsonReaders.date(json, 'paymentDate'),
        description: JsonReaders.nullableString(json, 'description'),
        isCancelled: JsonReaders.boolean(json, 'isCancelled'),
      );
}

class CardActivity {
  const CardActivity({
    required this.charges,
    required this.payments,
    this.hasMore = false,
  });
  final List<CardChargeItem> charges;
  final List<CardPaymentItem> payments;

  /// İstenen dönemde satır tavanından fazla kayıt vardı; liste kırpıldı.
  ///
  /// Ekran bunu söylemek zorunda: sessizce kırpılmış bir liste, kullanıcıya
  /// "hepsi bu" diye yanlış bir tamlık iddia eder.
  final bool hasMore;

  factory CardActivity.fromJson(Map<String, dynamic> json) => CardActivity(
    charges: JsonReaders.list(json, 'charges')
        .map(
          (value) =>
              CardChargeItem.fromJson(JsonReaders.object(value, 'charge')),
        )
        .toList(growable: false),
    payments: JsonReaders.list(json, 'payments')
        .map(
          (value) =>
              CardPaymentItem.fromJson(JsonReaders.object(value, 'payment')),
        )
        .toList(growable: false),
    hasMore: JsonReaders.boolean(json, 'hasMore'),
  );
}

class CardStatement {
  const CardStatement({
    required this.periodStart,
    required this.closingDate,
    required this.dueDate,
    required this.year,
    required this.month,
    required this.previousBalance,
    required this.periodCharges,
    required this.paymentsThroughClosing,
    required this.statementBalance,
    required this.paymentsAfterClosing,
    required this.remainingBalance,
    required this.minimumPayment,
    required this.remainingMinimumPayment,
    required this.minimumPaymentRate,
    required this.currency,
    required this.paymentStatus,
  });
  final String periodStart;
  final String closingDate;
  final String dueDate;
  final int year;
  final int month;
  final String previousBalance;
  final String periodCharges;

  /// Dönem içinde, kesimden önce yapılan ödemeler.
  ///
  /// API bunu hep gönderiyordu ama model okumuyordu; ekrandaki
  /// `önceki devir + dönem harcaması = ekstre borcu` satırları bu yüzden
  /// toplanmıyor, kullanıcı ekstreyi doğrulayamıyordu.
  final String paymentsThroughClosing;
  final String statementBalance;

  /// Kesimden sonra yapılan ödemeler; kalanı bunlar eritir.
  final String paymentsAfterClosing;
  final String remainingBalance;
  final String minimumPayment;
  final String remainingMinimumPayment;

  /// Asgari ödeme yüzdesi ("20.0000" = %20).
  final String minimumPaymentRate;
  final String currency;
  final String paymentStatus;

  bool get isPaid => paymentStatus == 'paid';
  bool get isOverdue => paymentStatus == 'overdue';

  factory CardStatement.fromJson(Map<String, dynamic> json) => CardStatement(
    periodStart: JsonReaders.date(json, 'periodStart'),
    closingDate: JsonReaders.date(json, 'closingDate'),
    dueDate: JsonReaders.date(json, 'dueDate'),
    year: JsonReaders.integer(json, 'year'),
    month: JsonReaders.integer(json, 'month'),
    previousBalance: JsonReaders.money(json, 'previousBalance'),
    periodCharges: JsonReaders.money(json, 'periodCharges'),
    paymentsThroughClosing: JsonReaders.money(json, 'paymentsThroughClosing'),
    statementBalance: JsonReaders.money(json, 'statementBalance'),
    paymentsAfterClosing: JsonReaders.money(json, 'paymentsAfterClosing'),
    remainingBalance: JsonReaders.money(json, 'remainingBalance'),
    minimumPayment: JsonReaders.money(json, 'minimumPayment'),
    remainingMinimumPayment: JsonReaders.money(json, 'remainingMinimumPayment'),
    minimumPaymentRate: JsonReaders.money(json, 'minimumPaymentRate'),
    currency: JsonReaders.string(json, 'currency'),
    paymentStatus: JsonReaders.string(json, 'paymentStatus'),
  );
}

class InstallmentItemModel {
  const InstallmentItemModel({
    required this.sequence,
    required this.amount,
    required this.currency,
    required this.scheduledDate,
    required this.isRealized,
  });
  final int sequence;
  final String amount;
  final String currency;
  final String scheduledDate;
  final bool isRealized;

  factory InstallmentItemModel.fromJson(Map<String, dynamic> json) =>
      InstallmentItemModel(
        sequence: JsonReaders.integer(json, 'sequence'),
        amount: JsonReaders.money(json, 'amount'),
        currency: JsonReaders.string(json, 'currency'),
        scheduledDate: JsonReaders.date(json, 'scheduledDate'),
        isRealized: JsonReaders.boolean(json, 'isRealized'),
      );
}

class InstallmentPlanModel {
  const InstallmentPlanModel({
    required this.id,
    required this.creditCardId,
    required this.totalAmount,
    required this.currency,
    required this.description,
    required this.items,
  });
  final String id;
  final String creditCardId;
  final String totalAmount;
  final String currency;
  final String? description;
  final List<InstallmentItemModel> items;

  factory InstallmentPlanModel.fromJson(
    Map<String, dynamic> json,
  ) => InstallmentPlanModel(
    id: JsonReaders.string(json, 'id'),
    creditCardId: JsonReaders.string(json, 'creditCardId'),
    totalAmount: JsonReaders.money(json, 'totalAmount'),
    currency: JsonReaders.string(json, 'currency'),
    description: JsonReaders.nullableString(json, 'description'),
    items: JsonReaders.list(json, 'items')
        .map(
          (value) =>
              InstallmentItemModel.fromJson(JsonReaders.object(value, 'item')),
        )
        .toList(growable: false),
  );
}

class FinanceSnapshot {
  const FinanceSnapshot({
    required this.transfers,
    required this.cards,
    required this.plans,
    required this.accounts,
    required this.expenseCategories,
    this.transfersHaveMore = false,
  });
  final List<TransferItem> transfers;
  final List<CreditCardItem> cards;
  final List<InstallmentPlanModel> plans;
  final List<FinanceChoice> accounts;
  final List<FinanceChoice> expenseCategories;

  /// Transfer listesi satır tavanında kırpıldı.
  final bool transfersHaveMore;
}
