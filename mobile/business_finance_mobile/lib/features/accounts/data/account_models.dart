import '../../../core/models/transaction_scope.dart';

class Account {
  const Account({
    required this.id,
    required this.name,
    required this.type,
    required this.currency,
    required this.isActive,
    required this.openingBalance,
    required this.balance,
    this.defaultScope,
  });

  final String id;
  final String name;
  final String type;
  final String currency;
  final bool isActive;
  final String openingBalance;
  final String balance;

  /// Hesabın varsayılan kapsamı; boş olması meşrudur, eksik veri değildir.
  ///
  /// Kapsam türetme zincirinin **orta halkası**: kullanıcının açık seçimi yoksa
  /// kayıt bunu alır, o da boşsa kategorinin varsayılanına düşer.
  final TransactionScope? defaultScope;

  factory Account.fromJson(Map<String, dynamic> json) => Account(
    id: _string(json, 'id'),
    name: _string(json, 'name'),
    type: _string(json, 'type'),
    currency: _string(json, 'currency'),
    isActive: _bool(json, 'isActive'),
    openingBalance: _string(json, 'openingBalance'),
    balance: _string(json, 'balance'),
    defaultScope: TransactionScope.fromApiOrNull(json['defaultScope']),
  );
}

class AccountPage {
  const AccountPage({required this.items, required this.pagination});

  final List<Account> items;
  final AccountPagination pagination;

  factory AccountPage.fromJson(Map<String, dynamic> json) {
    final rawItems = json['items'];
    final rawPagination = json['pagination'];
    if (rawItems is! List || rawPagination is! Map<String, dynamic>) {
      throw const FormatException('Account list response is invalid.');
    }
    return AccountPage(
      items: rawItems
          .map((item) {
            if (item is! Map<String, dynamic>) {
              throw const FormatException('Account item is invalid.');
            }
            return Account.fromJson(item);
          })
          .toList(growable: false),
      pagination: AccountPagination.fromJson(rawPagination),
    );
  }
}

class AccountPagination {
  const AccountPagination({
    required this.pageNumber,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
    required this.hasPreviousPage,
    required this.hasNextPage,
  });

  final int pageNumber;
  final int pageSize;
  final int totalCount;
  final int totalPages;
  final bool hasPreviousPage;
  final bool hasNextPage;

  factory AccountPagination.fromJson(Map<String, dynamic> json) =>
      AccountPagination(
        pageNumber: _int(json, 'pageNumber'),
        pageSize: _int(json, 'pageSize'),
        totalCount: _int(json, 'totalCount'),
        totalPages: _int(json, 'totalPages'),
        hasPreviousPage: _bool(json, 'hasPreviousPage'),
        hasNextPage: _bool(json, 'hasNextPage'),
      );
}

String _string(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value is String) return value;
  throw FormatException('$key must be a string.');
}

bool _bool(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value is bool) return value;
  throw FormatException('$key must be a boolean.');
}

int _int(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value is int) return value;
  throw FormatException('$key must be an integer.');
}
