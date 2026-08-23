import '../../../core/localization/default_category_labels.dart';
import '../../../core/models/json_readers.dart';

/// Ayın **tek bir** kapsamdaki gelir/gider tablosu.
class ScopeTotals {
  const ScopeTotals({
    required this.income,
    required this.expense,
    required this.net,
  });

  final String income;
  final String expense;
  final String net;

  factory ScopeTotals.fromJson(Map<String, dynamic> json) => ScopeTotals(
    income: JsonReaders.money(json, 'income'),
    expense: JsonReaders.money(json, 'expense'),
    net: JsonReaders.money(json, 'net'),
  );
}

/// Ayın iki tarafı ayrı ayrı: işletme ve şahsi.
///
/// Sunucudan **hazır** gelir. İki tarafı bir çıkarmayla türetmek istemcinin
/// finansal toplamı ikinci kez hesaplaması olurdu; ekrandaki sayı o zaman
/// sunucununkiyle tutmayabilirdi.
class MonthlyScopeBreakdown {
  const MonthlyScopeBreakdown({required this.business, required this.personal});

  final ScopeTotals business;
  final ScopeTotals personal;

  factory MonthlyScopeBreakdown.fromJson(Map<String, dynamic> json) =>
      MonthlyScopeBreakdown(
        business: ScopeTotals.fromJson(
          JsonReaders.object(json['business'], 'business'),
        ),
        personal: ScopeTotals.fromJson(
          JsonReaders.object(json['personal'], 'personal'),
        ),
      );
}

class DashboardReport {
  const DashboardReport({
    required this.year,
    required this.month,
    required this.totalIncome,
    required this.totalExpense,
    required this.net,
    required this.currency,
    required this.categoryExpenses,
    required this.categoryExpenseSlices,
    required this.accountBalances,
    this.scopeBreakdown,
  });

  final int year;
  final int month;
  final String totalIncome;
  final String totalExpense;
  final String net;
  final String currency;
  final List<CategoryExpense> categoryExpenses;

  /// Halka grafiğe sığan hâli: en büyük birkaç kategori ve "Diğer".
  final List<CategoryExpenseSlice> categoryExpenseSlices;
  final List<AccountBalance> accountBalances;

  /// Yalnız **filtresiz** okumada dolu. Kapsam filtresi verilmişse rapor zaten
  /// tek tarafı anlatıyor ve kırılım göndermek dışlanan tarafı sıfır
  /// gösterirdi.
  final MonthlyScopeBreakdown? scopeBreakdown;

  factory DashboardReport.fromJson(Map<String, dynamic> json) =>
      DashboardReport(
        year: JsonReaders.integer(json, 'year'),
        month: JsonReaders.integer(json, 'month'),
        totalIncome: JsonReaders.money(json, 'totalIncome'),
        totalExpense: JsonReaders.money(json, 'totalExpense'),
        net: JsonReaders.money(json, 'net'),
        currency: JsonReaders.string(json, 'currency'),
        categoryExpenses: JsonReaders.list(json, 'categoryExpenses')
            .map(
              (item) => CategoryExpense.fromJson(
                JsonReaders.object(item, 'categoryExpenses'),
              ),
            )
            .toList(growable: false),
        categoryExpenseSlices: JsonReaders.list(json, 'categoryExpenseSlices')
            .map(
              (item) => CategoryExpenseSlice.fromJson(
                JsonReaders.object(item, 'categoryExpenseSlices'),
              ),
            )
            .toList(growable: false),
        accountBalances: JsonReaders.list(json, 'accountBalances')
            .map(
              (item) => AccountBalance.fromJson(
                JsonReaders.object(item, 'accountBalances'),
              ),
            )
            .toList(growable: false),
        scopeBreakdown: json['scopeBreakdown'] == null
            ? null
            : MonthlyScopeBreakdown.fromJson(
                JsonReaders.object(json['scopeBreakdown'], 'scopeBreakdown'),
              ),
      );
}

class CategoryExpense {
  const CategoryExpense({
    required this.categoryId,
    required this.categoryName,
    required this.canonicalName,
    required this.amount,
  });

  final String categoryId;

  /// Kullanıcıya gösterilen ad (varsayılan kategoriler Türkçeleştirilir).
  final String categoryName;

  /// Sunucunun kanonik adı. İkon eşlemesi bunun üzerinden yapılır; etiket
  /// çevirisi değişince ikonun düşmemesi gerekir.
  final String canonicalName;

  final String amount;

  factory CategoryExpense.fromJson(Map<String, dynamic> json) {
    final rawName = JsonReaders.string(json, 'categoryName');
    return CategoryExpense(
      categoryId: JsonReaders.string(json, 'categoryId'),
      categoryName: DefaultCategoryLabels.localized(rawName),
      canonicalName: rawName,
      amount: JsonReaders.money(json, 'amount'),
    );
  }
}

/// Halka grafiğin bir dilimi: en büyük kategorilerden biri ya da "Diğer".
///
/// Gruplamayı ve "Diğer" toplamını sunucu yapıyor; "Diğer" bir finansal
/// toplamdır ve istemci parayı ikinci kez hesaplamaz.
class CategoryExpenseSlice {
  const CategoryExpenseSlice({
    required this.categoryName,
    required this.canonicalName,
    required this.amount,
    this.categoryId,
  });

  /// Yalnız "Diğer" satırında `null`: o satır bir kategori değil, birden
  /// çoğunun toplamı.
  final String? categoryId;
  final String categoryName;

  /// Sunucunun kanonik adı; ikon eşlemesi bunun üzerinden yapılır.
  final String canonicalName;
  final String amount;

  bool get isOther => categoryId == null;

  factory CategoryExpenseSlice.fromJson(Map<String, dynamic> json) {
    final rawName = JsonReaders.string(json, 'categoryName');
    return CategoryExpenseSlice(
      categoryId: JsonReaders.nullableString(json, 'categoryId'),
      categoryName: DefaultCategoryLabels.localized(rawName),
      canonicalName: rawName,
      amount: JsonReaders.money(json, 'amount'),
    );
  }
}

class AccountBalance {
  const AccountBalance({
    required this.accountId,
    required this.accountName,
    required this.balance,
    this.type,
  });

  final String accountId;
  final String accountName;
  final String balance;

  /// `cash` veya `bank`. Alan sözleşmeye sonradan eklendi; göndermeyen bir
  /// sunucuya karşı null kalır ve ikon nötr varsayılana düşer.
  final String? type;

  factory AccountBalance.fromJson(Map<String, dynamic> json) => AccountBalance(
    accountId: JsonReaders.string(json, 'accountId'),
    accountName: JsonReaders.string(json, 'accountName'),
    balance: JsonReaders.money(json, 'balance'),
    type: json['type'] is String ? json['type'] as String : null,
  );
}
