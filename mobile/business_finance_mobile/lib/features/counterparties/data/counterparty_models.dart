import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../activities/data/activity_models.dart';
import '../../obligations/data/obligation_repository.dart';

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
    this.overdueReceivable = '0.0000',
    this.overduePayable = '0.0000',
    this.notOverdueReceivable = '0.0000',
    this.notOverduePayable = '0.0000',
    required this.net,
    required this.isSettled,
    this.openReceivableObligations = '0.0000',
    this.openPayableObligations = '0.0000',
    String? owedToYou,
    String? owedByYou,
    this.note,
  }) : owedToYou = owedToYou ?? receivable,
       owedByYou = owedByYou ?? payable;

  factory CounterpartySummary.fromJson(Map<String, dynamic> json) =>
      CounterpartySummary(
        owedToYou: JsonReaders.nullableString(json, 'owedToYou'),
        owedByYou: JsonReaders.nullableString(json, 'owedByYou'),
        openReceivableObligations:
            JsonReaders.nullableString(json, 'openReceivableObligations') ??
            '0.0000',
        openPayableObligations:
            JsonReaders.nullableString(json, 'openPayableObligations') ??
            '0.0000',
        id: JsonReaders.string(json, 'id'),
        name: JsonReaders.string(json, 'name'),
        isActive: JsonReaders.boolean(json, 'isActive'),
        receivable: JsonReaders.string(json, 'receivable'),
        payable: JsonReaders.string(json, 'payable'),
        overdueReceivable: JsonReaders.string(json, 'overdueReceivable'),
        overduePayable: JsonReaders.string(json, 'overduePayable'),
        notOverdueReceivable: JsonReaders.string(json, 'notOverdueReceivable'),
        notOverduePayable: JsonReaders.string(json, 'notOverduePayable'),
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

  /// Vadesi geçmiş ve ödemelerden sonra açık kalan alacak.
  final String overdueReceivable;

  /// Vadesi geçmiş ve ödemelerden sonra açık kalan borç.
  final String overduePayable;

  /// İleri vadeli veya vadesi girilmemiş açık alacak.
  final String notOverdueReceivable;

  /// İleri vadeli veya vadesi girilmemiş açık borç.
  final String notOverduePayable;

  /// `receivable - payable`; sunucudan gelir, istemci çıkarma yapmaz.
  final String net;

  /// İki taraf da sıfır: kapanmış cari. Eksi bakiye **kapanmış sayılmaz** —
  /// fazla tahsilat kırpılmıyor ve hâlâ konuşulacak bir para var.
  final bool isSettled;

  /// Bu kişiye bağlı, tahsil edilmeyi bekleyen tek seferlik alacakların
  /// toplamı. **Bilgidir**: [receivable] ve [net] bunu içermez; her biri
  /// kendi kapanışıyla kapanır. Sunucudan gelir.
  final String openReceivableObligations;

  /// Bu kişiye bağlı, ödenmeyi bekleyen faturaların toplamı; [payable] ve
  /// [net] bunu içermez.
  final String openPayableObligations;

  /// Karşı tarafın bize borcu, **ekranda yazılacak hâliyle**: sıfır ya da
  /// artıdır. Fazla ödeme [payable]'ı eksiye düşürür; o tutar burada bize
  /// borç olarak gelir. Sunucudan gelir; verilmezse [receivable] kullanılır.
  final String owedToYou;

  /// Bizim ona borcumuz, ekranda yazılacak hâliyle. Fazla tahsilat
  /// [receivable]'ı eksiye düşürür; o tutar burada bizim borcumuzdur.
  final String owedByYou;

  /// Yalnız ayrıntı okumasında dolu; liste yanıtı notu taşımaz.
  final String? note;

  /// Kullanıcının bize borçlu olduğu yön mü. Sıfırda `false`; ekran o durumda
  /// zaten "kapandı" diyor.
  bool get isReceivableSide => !net.startsWith('-') && !isSettled;

  bool get hasOverdueReceivable => overdueReceivable != '0.0000';
  bool get hasOverduePayable => overduePayable != '0.0000';
  bool get hasOpenReceivableObligations =>
      openReceivableObligations != '0.0000';
  bool get hasOpenPayableObligations => openPayableObligations != '0.0000';
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

/// Bir karşı tarafın ayrıntısı: bakiyesi, hareketleri, varsa sözleşmeleri ve
/// bekleyen faturaları.
///
/// Dört kaynak tek ekranda buluşuyor ama **hiçbir toplam burada üretilmiyor**:
/// bakiye ve bekleyen faturaların toplamı sunucunun cevabı, hareketler
/// birleşik feed'in cevabı. İstemci aralarında aritmetik yapsaydı ekranda
/// üçüncü bir doğru belirirdi.
class CounterpartyDetail {
  const CounterpartyDetail({
    required this.counterparty,
    required this.activities,
    required this.agreements,
    this.pendingObligations = const [],
  });

  final CounterpartySummary counterparty;
  final List<FinancialActivity> activities;

  /// O kişiyle yapılmış taksitli sözleşmeler; cari hesabın dışında dururlar.
  final List<CounterpartyAgreement> agreements;

  /// Bu kişiye bağlı, henüz kapanmamış tek seferlik faturalar ve alacaklar.
  /// Cari bakiyeye girmezler ve buradan kapatılmazlar; her biri
  /// `Yükümlülükler`deki kendi kapanışıyla kapanır.
  final List<ObligationItem> pendingObligations;
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
