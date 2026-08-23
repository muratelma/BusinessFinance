import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../activities/data/activity_models.dart';

/// Bir kişiyle olan hesabın o anki hâli.
///
/// İki taraf ayrı ayrı duruyor: aynı kişi hem müşteri hem tedarikçi olabilir
/// ve mahalle esnafında bu sık. [net] ikisini tek cümleye indirir; artı ise
/// karşı taraf bize borçlu, eksi ise biz ona.
class CounterpartySummary {
  const CounterpartySummary({
    required this.id,
    required this.name,
    required this.isActive,
    required this.receivable,
    required this.payable,
    required this.net,
    required this.isSettled,
    this.note,
  });

  factory CounterpartySummary.fromJson(Map<String, dynamic> json) =>
      CounterpartySummary(
        id: JsonReaders.string(json, 'id'),
        name: JsonReaders.string(json, 'name'),
        isActive: JsonReaders.boolean(json, 'isActive'),
        receivable: JsonReaders.string(json, 'receivable'),
        payable: JsonReaders.string(json, 'payable'),
        net: JsonReaders.string(json, 'net'),
        isSettled: JsonReaders.boolean(json, 'isSettled'),
        note: JsonReaders.nullableString(json, 'note'),
      );

  final String id;
  final String name;
  final bool isActive;

  /// Karşı tarafın bize kalan borcu.
  final String receivable;

  /// Bizim ona kalan borcumuz.
  final String payable;

  /// `receivable - payable`; sunucudan gelir, istemci çıkarma yapmaz.
  final String net;

  /// İki taraf da sıfır: kapanmış cari. Eksi bakiye **kapanmış sayılmaz** —
  /// fazla tahsilat kırpılmıyor ve hâlâ konuşulacak bir para var.
  final bool isSettled;

  /// Yalnız ayrıntı okumasında dolu; liste yanıtı notu taşımaz.
  final String? note;

  /// Kullanıcının bize borçlu olduğu yön mü. Sıfırda `false`; ekran o durumda
  /// zaten "kapandı" diyor.
  bool get isReceivableSide => !net.startsWith('-') && !isSettled;
}

/// Liste ekranının hangi tarafı okuduğu.
///
/// Sıfır bakiyeli karşı taraf silinmez ve listeden düşmez; yalnız ayrı okunur.
/// Bir müşteriyle hesabın kapanmış olması onunla iş yapılmadığı anlamına
/// gelmez.
enum CounterpartyBalanceFilter {
  all('all', 'Tümü'),
  open('open', 'Açık hesap'),
  settled('settled', 'Kapanmış');

  const CounterpartyBalanceFilter(this.apiValue, this.label);
  final String apiValue;
  final String label;
}

/// Liste ekranının tek okumada ihtiyaç duyduğu her şey.
///
/// Hesaplar ve kategoriler burada çünkü hızlı tahsilat ve borçlandırma formu
/// karşı taraf ekranından açılıyor; ikinci bir ekrana gitmeden yazılabilmesi
/// listenin kendisiyle aynı okumaya bağlı.
class CounterpartiesSnapshot {
  const CounterpartiesSnapshot({
    required this.counterparties,
    required this.accounts,
    required this.categories,
  });

  final List<CounterpartySummary> counterparties;
  final List<DataChoice> accounts;
  final List<DataChoice> categories;

  List<DataChoice> categoriesOfType(String type) =>
      categories.where((item) => item.type == type).toList(growable: false);
}

/// Bir karşı tarafın ayrıntısı: bakiyesi, hareketleri ve varsa sözleşmeleri.
///
/// Üç kaynak tek ekranda buluşuyor ama **hiçbir toplam burada üretilmiyor**:
/// bakiye sunucunun cevabı, hareketler birleşik feed'in cevabı. İstemci
/// aralarında aritmetik yapsaydı ekranda üçüncü bir doğru belirirdi.
class CounterpartyDetail {
  const CounterpartyDetail({
    required this.counterparty,
    required this.activities,
    required this.agreements,
  });

  final CounterpartySummary counterparty;
  final List<FinancialActivity> activities;

  /// O kişiyle yapılmış taksitli sözleşmeler; cari hesabın dışında dururlar.
  final List<CounterpartyAgreement> agreements;
}

/// Karşı tarafın taksitli sözleşmesinin ekranda gösterilen özeti.
class CounterpartyAgreement {
  const CounterpartyAgreement({
    required this.id,
    required this.isReceivable,
    required this.remaining,
    required this.installmentCount,
    required this.paidCount,
  });

  factory CounterpartyAgreement.fromJson(Map<String, dynamic> json) {
    final installments = JsonReaders.list(json, 'installments')
        .map((item) => JsonReaders.object(item, 'installment'))
        .toList(growable: false);
    return CounterpartyAgreement(
      id: JsonReaders.string(json, 'id'),
      isReceivable: JsonReaders.string(json, 'direction') == 'receivable',
      remaining: JsonReaders.string(json, 'remainingAmount'),
      installmentCount: installments.length,
      paidCount: installments
          .where((item) => JsonReaders.string(item, 'status') == 'paid')
          .length,
    );
  }

  final String id;
  final bool isReceivable;
  final String remaining;
  final int installmentCount;
  final int paidCount;
}
