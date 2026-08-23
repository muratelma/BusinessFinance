import 'package:flutter/foundation.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../cards/data/finance_repository.dart';
import '../../transactions/data/transaction_models.dart';
import '../../transactions/data/transaction_repository.dart';
import '../data/receipt_fee_writer.dart';
import 'quick_add_models.dart';

/// Kaydedilen işleme belge ekleyen dar imza.
typedef QuickAddAttachmentUploader =
    Future<void> Function(String transactionId, QuickAddAttachment attachment);

/// Backs the expense and income forms behind the shared launcher.
///
/// It only chooses which existing write endpoint to call. Every financial rule —
/// limits, ownership, category type — stays on the server, so the form cannot
/// disagree with it.
class QuickAddController extends ChangeNotifier {
  QuickAddController(
    this._transactions,
    this._finance, {
    this.financialDataChanges,
    this.uploadAttachment,
  });

  final TransactionRepositoryContract _transactions;
  final FinanceRepositoryContract _finance;
  final FinancialDataChanges? financialDataChanges;

  /// Kaydedilen işleme belge ekler.
  ///
  /// Dar bir imza olarak alınıyor: bu form belge deposunu, uç noktasını veya
  /// veri araçları özelliğini tanımak zorunda değil. Yoksa belge adımı hiç
  /// çalışmaz, gider yazma bundan etkilenmez.
  final QuickAddAttachmentUploader? uploadAttachment;

  bool isLoading = false;
  bool isSubmitting = false;
  bool unauthorized = false;
  String? errorMessage;

  /// Gider yazıldı ama belge eklenemedi.
  ///
  /// Hata değil uyarıdır ve kaydı geri almaz: fotoğraf uğruna finansal kayıt
  /// silinmez. Kullanıcının bilmesi gerekir, çünkü belgeyi eklediğini sanır.
  String? attachmentWarning;
  ExpenseFormOptions? expenseOptions;
  IncomeFormOptions? incomeOptions;

  Future<void> loadExpenseOptions() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      // One snapshot carries accounts, cards and expense categories together,
      // so the picker is built from a single round trip.
      final snapshot = await _finance.load();
      expenseOptions = ExpenseFormOptions(
        sources: [
          for (final account in snapshot.accounts)
            PaymentSource(
              id: account.id,
              name: account.name,
              kind: PaymentSourceKind.account,
              defaultScope: account.defaultScope,
            ),
          for (final card in snapshot.cards.where((card) => card.isActive))
            PaymentSource(
              id: card.id,
              name: card.name,
              kind: PaymentSourceKind.creditCard,
              availableLimit: card.availableLimit,
              defaultScope: card.defaultScope,
            ),
        ],
        categories: [
          for (final category in snapshot.expenseCategories)
            QuickAddChoice(
              id: category.id,
              name: category.name,
              defaultScope: category.defaultScope,
            ),
        ],
      );
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> loadIncomeOptions() async {
    if (isLoading) return;
    isLoading = true;
    errorMessage = null;
    notifyListeners();
    try {
      final accounts = await _transactions.listAccounts();
      final categories = await _transactions.listCategories(
        TransactionKind.income,
      );
      incomeOptions = IncomeFormOptions(
        // Income can only land in a cash or bank account. A card cannot receive
        // income, and card refunds are not modelled.
        accounts: [
          for (final account in accounts.where((item) => item.isActive))
            QuickAddChoice(
              id: account.id,
              name: account.name,
              defaultScope: account.defaultScope,
            ),
        ],
        categories: [
          for (final category in categories.where((item) => item.isActive))
            QuickAddChoice(
              id: category.id,
              name: category.name,
              defaultScope: category.defaultScope,
            ),
        ],
      );
      unauthorized = false;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  /// Sends the expense to the endpoint that owns the chosen source: a card
  /// purchase is a card charge, not a bank transaction.
  ///
  /// [scope] boş gönderilebilir: sunucu kapsamı kendi türetir. Form onu
  /// dolduruyor çünkü kullanıcı kaydın hangi tarafa yazıldığını **kaydetmeden
  /// önce** görmeli.
  Future<bool> submitExpense({
    required PaymentSource source,
    required String categoryId,
    required String amount,
    required String date,
    String? description,
    QuickAddAttachment? attachment,
    TransactionScope? scope,
  }) => _submit(isCardSpending: source.isCard, () async {
    if (source.isCard) {
      await _finance.createCharge(source.id, {
        'categoryId': categoryId,
        'amount': amount,
        'currency': 'TRY',
        'chargeDate': date,
        'description': description,
        'scope': scope?.apiValue,
      });
      return;
    }
    final created = await _transactions.create(
      CreateTransactionInput(
        accountId: source.id,
        categoryId: categoryId,
        amount: amount,
        kind: TransactionKind.expense,
        transactionDate: date,
        description: description,
        scope: scope,
      ),
    );
    if (attachment != null) await _attach(created.id, attachment);
  });

  /// Dekonttaki işlem ücretini ana kayıttan **sonra**, aynı kaynağa yazar.
  ///
  /// Ana kayıt yazılmadıysa çağrılmaz: harcanmamış bir ücret yoktur. Sonucu
  /// döndürür ve `errorMessage`'a dokunmaz — ücret başarısız olsa bile ana
  /// kayıt yazıldı ve doğru; formu hatalıymış gibi göstermek yanlış olurdu.
  Future<ReceiptFeeResult> submitAutoFee({
    required PaymentSource source,
    required QuickAddAutoFee fee,
    required String date,
  }) => recordReceiptFee(
    transactions: _transactions,
    finance: _finance,
    sourceId: source.id,
    sourceIsCard: source.isCard,
    amount: fee.amount,
    date: date,
    description: fee.description,
    // Kategoriler formla birlikte zaten yüklendi; ikinci bir tur atılmaz.
    categoryId: _feeCategoryId,
    changes: financialDataChanges,
  );

  /// Ücret kategorisini kullanıcının kendi listesinde arar; yoksa `null` ve
  /// yazıcı kullanıcıya bunu söyler.
  String? get _feeCategoryId {
    for (final category in expenseOptions?.categories ?? const []) {
      if (category.name.toLowerCase() == receiptFeeCategoryName.toLowerCase()) {
        return category.id;
      }
    }
    return null;
  }

  /// Belge ekleme, giderin yazılmasından **sonra** ve ondan bağımsız.
  ///
  /// Başarısız olursa hata yukarı taşınmaz: gider zaten yazıldı ve doğru.
  /// Kullanıcıya yalnız belgenin eklenemediği söylenir.
  Future<void> _attach(
    String transactionId,
    QuickAddAttachment attachment,
  ) async {
    final upload = uploadAttachment;
    if (upload == null) return;
    try {
      await upload(transactionId, attachment);
    } on ApiException catch (error) {
      attachmentWarning =
          'Gider kaydedildi ama fiş fotoğrafı eklenemedi: ${error.message}';
    } on FormatException {
      attachmentWarning =
          'Gider kaydedildi ama fiş fotoğrafı eklenemedi. '
          'Belgeyi daha sonra ekleyebilirsiniz.';
    }
  }

  Future<bool> submitIncome({
    required String accountId,
    required String categoryId,
    required String amount,
    required String date,
    String? description,
    TransactionScope? scope,
  }) => _submit(
    () => _transactions.create(
      CreateTransactionInput(
        accountId: accountId,
        categoryId: categoryId,
        amount: amount,
        kind: TransactionKind.income,
        transactionDate: date,
        description: description,
        scope: scope,
      ),
    ),
  );

  /// The lock is the only thing stopping a double tap from writing twice: the
  /// server has no idempotency key yet.
  Future<bool> _submit(
    Future<void> Function() action, {
    bool isCardSpending = false,
  }) async {
    if (isSubmitting) return false;
    isSubmitting = true;
    errorMessage = null;
    attachmentWarning = null;
    notifyListeners();
    try {
      await action();
      if (isCardSpending) {
        financialDataChanges?.cardSpendingChanged();
      } else {
        financialDataChanges?.transactionsChanged();
      }
      return true;
    } on ApiException catch (error) {
      unauthorized = error.isUnauthorized;
      errorMessage = error.message;
      return false;
    } on FormatException {
      errorMessage = 'Sunucudan beklenmeyen bir yanıt alındı.';
      return false;
    } finally {
      isSubmitting = false;
      notifyListeners();
    }
  }
}
