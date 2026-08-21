enum TransactionKind {
  income('income'),
  expense('expense');

  const TransactionKind(this.apiValue);
  final String apiValue;

  static TransactionKind fromApi(String value) => switch (value) {
    'income' => income,
    'expense' => expense,
    _ => throw FormatException('Unknown transaction type: $value'),
  };
}

class TransactionItem {
  const TransactionItem({
    required this.id,
    required this.accountId,
    required this.categoryId,
    required this.amount,
    required this.currency,
    required this.kind,
    required this.transactionDate,
    required this.isCancelled,
    this.description,
    this.cancelledAtUtc,
  });

  factory TransactionItem.fromJson(Map<String, dynamic> json) =>
      TransactionItem(
        id: json['id'] as String,
        accountId: json['accountId'] as String,
        categoryId: json['categoryId'] as String,
        amount: json['amount'] as String,
        currency: json['currency'] as String,
        kind: TransactionKind.fromApi(json['type'] as String),
        transactionDate: json['transactionDate'] as String,
        description: json['description'] as String?,
        isCancelled: json['isCancelled'] as bool,
        cancelledAtUtc: json['cancelledAtUtc'] as String?,
      );

  final String id;
  final String accountId;
  final String categoryId;
  final String amount;
  final String currency;
  final TransactionKind kind;
  final String transactionDate;
  final String? description;
  final bool isCancelled;
  final String? cancelledAtUtc;
}

class TransactionPage {
  const TransactionPage({
    required this.items,
    required this.pageNumber,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
    required this.hasPreviousPage,
    required this.hasNextPage,
  });

  factory TransactionPage.fromJson(Map<String, dynamic> json) {
    final pagination = json['pagination'] as Map<String, dynamic>;
    return TransactionPage(
      items: (json['items'] as List<dynamic>)
          .map((item) => TransactionItem.fromJson(item as Map<String, dynamic>))
          .toList(growable: false),
      pageNumber: pagination['pageNumber'] as int,
      pageSize: pagination['pageSize'] as int,
      totalCount: pagination['totalCount'] as int,
      totalPages: pagination['totalPages'] as int,
      hasPreviousPage: pagination['hasPreviousPage'] as bool,
      hasNextPage: pagination['hasNextPage'] as bool,
    );
  }

  final List<TransactionItem> items;
  final int pageNumber;
  final int pageSize;
  final int totalCount;
  final int totalPages;
  final bool hasPreviousPage;
  final bool hasNextPage;
}

class TransactionFilter {
  const TransactionFilter({
    this.dateFrom,
    this.dateTo,
    this.accountId,
    this.categoryId,
    this.kind,
  });

  final String? dateFrom;
  final String? dateTo;
  final String? accountId;
  final String? categoryId;
  final TransactionKind? kind;
}

class CreateTransactionInput {
  const CreateTransactionInput({
    required this.accountId,
    required this.categoryId,
    required this.amount,
    required this.kind,
    required this.transactionDate,
    this.description,
  });

  final String accountId;
  final String categoryId;
  final String amount;
  final TransactionKind kind;
  final String transactionDate;
  final String? description;

  Map<String, Object?> toJson() => {
    'accountId': accountId,
    'categoryId': categoryId,
    'amount': amount,
    'currency': 'TRY',
    'type': kind.apiValue,
    'transactionDate': transactionDate,
    'description': description,
  };
}

class TransactionChoice {
  const TransactionChoice({
    required this.id,
    required this.name,
    required this.isActive,
    this.kind,
  });

  final String id;
  final String name;
  final bool isActive;
  final TransactionKind? kind;
}
