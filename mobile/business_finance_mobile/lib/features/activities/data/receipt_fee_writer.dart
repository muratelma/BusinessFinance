import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../cards/data/finance_repository.dart';
import '../../transactions/data/transaction_models.dart';
import '../../transactions/data/transaction_repository.dart';

/// İşlem ücretinin yazıldığı kategori.
///
/// `Faiz ve finansman gideri` bilerek seçilmedi: o kategori borç faizini
/// raporluyor (ADR 0009) ve banka masrafları oraya karışırsa "borcum bana ne
/// kadar faize mal oldu" sorusunun cevabı bozulur.
const receiptFeeCategoryName = 'Diğer gider';

/// Ücret yazma denemesinin üç sonucu.
///
/// Üçü de çağırana söylenir. Sessizce kaybolmak, kullanıcının kaydedildiğini
/// sandığı bir gideri deftere hiç yazmamak olurdu — formu kaldırmanın bedeli
/// bu olamaz.
enum ReceiptFeeStatus {
  /// Ücret ana kayıtla aynı kaynağa yazıldı.
  recorded,

  /// Kullanıcının listesinde ücret kategorisi yok; uydurulmuş bir kovaya
  /// yazmak yerine yazılmadı.
  categoryMissing,

  /// Sunucu reddetti veya ağ hatası. Ana kayıt geri **alınmaz**: o kayıt
  /// doğru ve yazıldı.
  failed,
}

class ReceiptFeeResult {
  const ReceiptFeeResult(this.status, this.message);

  final ReceiptFeeStatus status;

  /// Kullanıcıya gösterilecek cümle; her sonuçta dolu.
  final String message;

  bool get isRecorded => status == ReceiptFeeStatus.recorded;
}

/// Dekonttaki işlem ücretini ana kayıtla **aynı kaynaktan** yazar.
///
/// Ücretler 1,20 veya 4,50 gibi tutarlardır. Kullanıcıyı bunun için ikinci bir
/// forma çağırmak — üstelik ödeme kaynağını yeniden seçtirerek — kaydın
/// kendisinden pahalı bir iş yükü olurdu. Kaynak sorulmuyor çünkü cevabı
/// belli: bankanın ücreti aldığı yer, ana tutarın çıktığı yerdir.
///
/// Ücret yine **ayrı bir kayıt**: ana tutara eklemek belgede yazmayan bir
/// toplam uydurmak olurdu (ADR 0011 madde 6, aritmetik yok) ve o zaman fatura
/// tutarı da yanlış raporlanırdı.
Future<ReceiptFeeResult> recordReceiptFee({
  required TransactionRepositoryContract transactions,
  required FinanceRepositoryContract finance,
  required String sourceId,
  required bool sourceIsCard,
  required String amount,
  required String date,
  required String description,
  String? categoryId,
  FinancialDataChanges? changes,
}) async {
  try {
    // Kimlik çağıranda hazırsa (form kategorilerini zaten yüklemiştir) ikinci
    // bir tur atılmaz.
    final resolved = categoryId ?? await _findFeeCategoryId(finance);
    if (resolved == null) {
      return const ReceiptFeeResult(
        ReceiptFeeStatus.categoryMissing,
        'İşlem ücreti kaydedilemedi: "$receiptFeeCategoryName" kategorisi '
        'listenizde yok. Ücreti elle girebilirsiniz.',
      );
    }

    if (sourceIsCard) {
      await finance.createCharge(sourceId, {
        'categoryId': resolved,
        'amount': amount,
        'currency': 'TRY',
        'chargeDate': date,
        'description': description,
      });
      changes?.cardSpendingChanged();
    } else {
      await transactions.create(
        CreateTransactionInput(
          accountId: sourceId,
          categoryId: resolved,
          amount: amount,
          kind: TransactionKind.expense,
          transactionDate: date,
          description: description,
        ),
      );
      changes?.transactionsChanged();
    }

    return ReceiptFeeResult(
      ReceiptFeeStatus.recorded,
      'İşlem ücreti $receiptFeeCategoryName olarak kaydedildi.',
    );
  } on ApiException catch (error) {
    return ReceiptFeeResult(
      ReceiptFeeStatus.failed,
      'Ana kayıt yazıldı ama işlem ücreti kaydedilemedi: ${error.message}',
    );
  } on FormatException {
    return const ReceiptFeeResult(
      ReceiptFeeStatus.failed,
      'Ana kayıt yazıldı ama işlem ücreti kaydedilemedi. '
      'Ücreti elle girebilirsiniz.',
    );
  }
}

/// Kategoriyi kullanıcının **kendi** listesinde adıyla arar.
///
/// Bulunamazsa kategori açılmaz: kullanıcının görmediği bir kova yaratmak,
/// istenmeyen ikinci bir yazma olurdu.
Future<String?> _findFeeCategoryId(FinanceRepositoryContract finance) async {
  final snapshot = await finance.load();
  for (final category in snapshot.expenseCategories) {
    if (category.name.toLowerCase() == receiptFeeCategoryName.toLowerCase()) {
      return category.id;
    }
  }
  return null;
}

/// Ücreti belirli bir kaynaktan yazan dar imza.
///
/// Transfer ve kart ödeme ekranları depoları tanımak zorunda değil; router
/// bunu bir kez kurar ve aşağı geçirir (`uploadAttachment` ile aynı desen).
typedef ReceiptFeeRecorder =
    Future<ReceiptFeeResult> Function({
      required String sourceId,
      required String amount,
      required String date,
      required String description,
    });
