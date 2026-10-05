import 'dart:typed_data';

import '../../../core/models/transaction_scope.dart';

/// Menünün başlıkları: kullanıcının aklındaki **niyet**.
///
/// Düz liste dokuz satıra çıkınca okunmaz oldu; satırlar bir eksene bağlanmak
/// zorundaydı. Eksen kayıt türü değil niyettir, çünkü kullanıcı menüyü açarken
/// "bu bir `CreditCardPayment` mı" diye düşünmez, "param nereye gitti" diye
/// düşünür.
///
/// [carry] ayrı bir başlık, çünkü transfer ile kart borcu ödemesi **gider
/// değildir** (ADR 0014): ödemeyi taşırlar, gelir/gider yazmazlar. İkisini
/// [moneyOut] altına koymak, menünün kendisinin raporu yanlış anlatması
/// olurdu.
///
/// Kasa sayımı bu menüde **yok**: hiçbir para hareketi üretmez, bir
/// gözlemdir ve kendi ekranında (`Kasa > Kasayı say`) durur.
enum QuickAddIntent {
  moneyIn('Para girdi'),
  moneyOut('Para çıktı'),
  carry('Para taşı'),
  scanDocument('Belge okut'),
  plan('Plan kur');

  const QuickAddIntent(this.label);
  final String label;
}

/// The ways a user starts a movement. Account and card creation, CSV import
/// and debt or installment plans stay on their own management screens: those set
/// something up, they do not record money moving today.
///
/// Sıra başlığa göredir: aynı [QuickAddIntent] değerini taşıyan satırlar menüde
/// ardışık görünür.
enum QuickAddOption {
  income('Gelir', QuickAddIntent.moneyIn),
  posCollection('POS tahsilatı', QuickAddIntent.moneyIn),

  /// Günün satışı toplamla girilir: nakit kasaya gelir, kart POS tahsilatı
  /// olur (ADR 0019 T1). İşletme profilinde `POS tahsilatı`nın yerini alır.
  dayClose('Gün sonu', QuickAddIntent.moneyIn),
  expense('Gider', QuickAddIntent.moneyOut),
  obligation('Ödenmemiş fatura', QuickAddIntent.moneyOut),
  transfer('Transfer', QuickAddIntent.carry),
  cardPayment('Kart borcu öde', QuickAddIntent.carry),
  receipt('Fiş veya fatura', QuickAddIntent.scanDocument),
  bankSlip('Banka dekontu', QuickAddIntent.scanDocument),
  recurringPlan('Tekrarlayan işlem', QuickAddIntent.plan);

  const QuickAddOption(this.label, this.intent);
  final String label;

  /// Satırın altında duracağı başlık.
  final QuickAddIntent intent;

  /// Satırın altındaki tek satırlık açıklama; adın söylemediği bir şey
  /// yoksa `null` (tasarım teslimi, 27 Eylül 2026).
  ///
  /// Kutucuklardaki seçenekler (gelir, gider, transfer, belgeler) açıklama
  /// göstermez; açıklama yalnız satırın **yanlış anlaşılabileceği** yerde
  /// durur. POS satışı iki anı olan tek kayıttır (gelir bugün, para sonra);
  /// ödenmemiş fatura bugün para hareket ettirmez.
  String? get description => switch (this) {
    posCollection => 'Kartla satış, sonra hesaba geçer',
    dayClose => 'Günün nakit ve kartlı satışı',
    obligation => 'Vadesi olan, henüz ödenmedi',
    recurringPlan => 'Kira, abonelik, maaş',
    _ => null,
  };

  /// Ekran okuyucunun seçenek için okuyacağı cümle: kutucukta görünmeyen
  /// ayrıntıyı da taşır.
  String get spokenLabel => switch (this) {
    transfer => 'Hesaplar arası transfer',
    cardPayment => 'Kredi kartı borcunu öde, gider yazmaz',
    receipt => 'Fiş veya faturayı fotoğraftan oku',
    bankSlip => 'Banka dekontunu oku: havale, EFT, ATM veya kart ödemesi',
    obligation =>
      'Ödenmemiş fatura: vadesi olan, henüz ödenmedi, para hareket etmez',
    _ => description == null ? label : '$label: $description',
  };
}

/// Where an expense is paid from. The two are separate write models on the
/// server, so the picker keeps them in named groups instead of pretending a card
/// is just another account.
enum PaymentSourceKind { account, creditCard }

/// Ödemenin nasıl yapıldığına dair ipucu — hangi kaynaktan yapıldığına dair
/// değil.
///
/// `PaymentSourceKind` yetmiyor çünkü hesap tarafına düşen iki ayrı durum var:
/// nakit ve banka kartı. İkisi de hesaptan çıkar ama ekranda aynı cümleyi
/// yazmak yanlış olur, ve kullanıcı "banka kartı" adını verdiği hesabı arıyorsa
/// bunu söylemek gerekir.
///
/// [card], türü yazmayan fişler içindir. Çoğu yazar kasa fişi yalnız "KART"
/// basar; kredi mi banka kartı mı olduğunu uydurmak, banka kartı harcamasını
/// kredi kartı borcuna yazdırır.
enum PaymentSourceHint { cash, debitCard, creditCard, card }

class PaymentSource {
  const PaymentSource({
    required this.id,
    required this.name,
    required this.kind,
    this.availableLimit,
    this.defaultScope,
  });

  final String id;
  final String name;
  final PaymentSourceKind kind;

  /// Kaynağın kapsam etiketi. Türetme zincirinin ikinci halkası: kategoriyi
  /// yener, kullanıcının açık seçimine yenilir.
  final TransactionScope? defaultScope;

  /// Only meaningful for a card; shown so the user sees the room left before
  /// the server refuses the charge.
  final String? availableLimit;

  bool get isCard => kind == PaymentSourceKind.creditCard;
}

/// Kayıt yazıldıktan **sonra** eklenecek belge.
///
/// Forma byte olarak geliyor çünkü belge, kaydın kendisinden bağımsız bir
/// ikinci istektir: önce gider yazılır, sonra belge eklenir. Sıra tersine
/// çevrilemez — belgenin bağlanacağı kayıt henüz yoktur.
class QuickAddAttachment {
  const QuickAddAttachment({
    required this.bytes,
    required this.fileName,
    required this.mediaType,
  });

  final Uint8List bytes;
  final String fileName;
  final String mediaType;
}

/// Bir alanın önerilme biçimi.
///
/// `suspect` ayrı tutuluyor çünkü kullanıcıya söylenecek cümle farklı: değer
/// okundu ama çapraz kontrolü tutmadı. İkisini birleştirmek, kontrol edilmesi
/// gereken tutarı kontrol edilmiş gibi gösterirdi.
enum QuickAddSuggestionState { read, suspect }

/// Forma önerilen tek bir alan değeri.
class QuickAddSuggestion {
  const QuickAddSuggestion(this.value, this.state);

  final String value;
  final QuickAddSuggestionState state;

  bool get isSuspect => state == QuickAddSuggestionState.suspect;

  /// Alanın altında görünen cümle. Öneri olduğunu söylemesi şart: form önü
  /// dolu açıldığında kullanıcı, değerleri kendi girmiş gibi hızla geçiyor.
  String get helperText => isSuspect
      ? 'Fişten okundu, şüpheli — kontrol edin'
      : 'Fişten okundu — kontrol edin';
}

/// Ana kayıtla birlikte, aynı ödeme kaynağından yazılacak işlem ücreti.
///
/// Kendi formu yok ve olmamalı: tutarlar 1,20 veya 4,50 gibidir ve kullanıcıyı
/// bunun için ikinci bir forma çağırmak — üstelik ödeme kaynağını yeniden
/// seçtirerek — kaydın kendisinden pahalı olurdu. Kaynak sorulmuyor çünkü
/// cevabı belli: banka ücreti, ana tutarın çıktığı yerden alır.
///
/// Ücret yine **ayrı bir kayıt**; ana tutara eklenmiyor.
class QuickAddAutoFee {
  const QuickAddAutoFee({required this.amount, required this.description});

  /// Dört ondalıklı string.
  final String amount;

  final String description;
}

/// Formun önünü dolduran öneriler.
///
/// Fiş sözleşmesinden bilerek bağımsız: form, değerin fotoğraftan mı başka bir
/// kaynaktan mı geldiğini bilmek zorunda değil, yalnız "bu bir öneri" bilgisine
/// ihtiyacı var. Bu sayede `activities` özelliği `receipts`e bağımlı olmuyor.
class QuickAddPrefill {
  const QuickAddPrefill({
    this.amount,
    this.date,
    this.categoryId,
    this.categoryNameHint,
    this.description,
    this.sourceHint,
    this.warnings = const [],
    this.attachment,
    this.keepAttachmentByDefault = true,
    this.autoFee,
  });

  /// Dört ondalıklı string; forma kullanıcının göreceği biçimde yazılır.
  final QuickAddSuggestion? amount;

  /// `yyyy-MM-dd`.
  final QuickAddSuggestion? date;

  /// Kullanıcının kendi kategorisine çözülmüş kimlik; çözülemediyse `null`.
  final QuickAddSuggestion? categoryId;

  /// Kimlik yerine **ad** ile önerilen kategori.
  ///
  /// Banka işlem ücreti gibi, kaynağı sunucudan kimlikle gelmeyen ama doğru
  /// kovası bilinen kayıtlar için. Ad, seçenekler yüklendiğinde kullanıcının
  /// kendi listesinde aranır; bulunamazsa alan boş kalır — uydurulmuş bir
  /// kategori yazılmaz.
  final String? categoryNameHint;

  /// Kaydın adı olacak metin (fişte işletme adı).
  final QuickAddSuggestion? description;

  /// Fişte yazan ödeme biçimi. Kaynağı **seçmez**: fiş hangi karttan ödendiğini
  /// bilmez ve kullanıcı fişteki yazının aksine ödemiş olabilir. Yalnız listeyi
  /// sıralar ve ekranda bir not olarak görünür.
  final PaymentSourceHint? sourceHint;

  /// Sunucunun ürettiği uyarılar; metin istemcide yeniden yazılmaz.
  final List<String> warnings;

  /// Kaydın yanına eklenebilecek **orijinal** fotoğraf.
  ///
  /// Ön işlenmiş kopya değil: belge kanıttır, saklanan şey gerçek fotoğraf
  /// olmalı. `null` ise saklanacak bir şey yok ve anahtar da görünmez.
  final QuickAddAttachment? attachment;

  /// Anahtarın açılış durumu; cihazda hatırlananı taşır.
  final bool keepAttachmentByDefault;

  /// Ana kayıtla birlikte, **ikinci bir form açılmadan** yazılacak işlem
  /// ücreti. `null` ise yazılacak ücret yok.
  final QuickAddAutoFee? autoFee;

  bool get isEmpty =>
      amount == null &&
      date == null &&
      categoryId == null &&
      description == null;
}

class QuickAddChoice {
  const QuickAddChoice({
    required this.id,
    required this.name,
    this.defaultScope,
  });
  final String id;
  final String name;

  /// Kategorinin ya da hesabın varsayılan kapsamı; zincirin son halkası.
  final TransactionScope? defaultScope;
}

class ExpenseFormOptions {
  const ExpenseFormOptions({required this.sources, required this.categories});
  final List<PaymentSource> sources;
  final List<QuickAddChoice> categories;

  List<PaymentSource> get accounts =>
      sources.where((source) => !source.isCard).toList(growable: false);
  List<PaymentSource> get cards =>
      sources.where((source) => source.isCard).toList(growable: false);
}

class IncomeFormOptions {
  const IncomeFormOptions({required this.accounts, required this.categories});
  final List<QuickAddChoice> accounts;
  final List<QuickAddChoice> categories;
}
