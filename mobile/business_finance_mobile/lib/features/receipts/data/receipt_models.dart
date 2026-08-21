import '../../../core/models/json_readers.dart';

/// Bir alanın ne kadar güvenilir okunduğu.
///
/// Üç durum tek bir "okundu mu" bayrağına indirgenmiyor, çünkü onay formunun
/// kullanıcıya söylemesi gereken şey üç ayrı cümle: değer okundu, değer okundu
/// ama şüpheli, değer hiç okunamadı. Şüpheliyi okunmuş saymak kullanıcıyı
/// kontrol etmeden onaylamaya iter.
enum ReceiptFieldState {
  read,
  suspect,
  missing;

  static ReceiptFieldState parse(Map<String, dynamic> json, String key) =>
      switch (JsonReaders.string(json, key)) {
        'read' => ReceiptFieldState.read,
        'suspect' => ReceiptFieldState.suspect,
        'missing' => ReceiptFieldState.missing,
        // Tanınmayan durum sessizce `read` sayılmaz: doğrulanmamış bir değeri
        // doğrulanmış göstermek, sözleşme kaymasını gizlemekten kötüdür.
        _ => throw const FormatException('Unknown receipt field state.'),
      };

  bool get isMissing => this == ReceiptFieldState.missing;
  bool get isSuspect => this == ReceiptFieldState.suspect;
}

/// Okunan belgenin türü.
///
/// İstemcinin bir sonraki adımı buna bağlı: banka belgesinde ana tutarın ne
/// olduğu belgeden okunamaz — ödeme mi, kendi hesabına aktarma mı, kart ödemesi
/// mi, borç verme mi — ve kullanıcıya sorulur. Alışveriş belgesinde böyle bir
/// soru yoktur, doğrudan forma gidilir.
enum ReceiptDocumentKind {
  purchaseReceipt,
  invoiceOrVoucher,
  bankDocument,
  bankPayment,

  /// Kredi kartı borcunun ödendiği dekont. Gider **üretmemeli**: harcamalar
  /// kart harcaması olarak zaten sayıldı.
  bankCardPayment,

  /// İade fişi. Yeni bir kayıt üretmez — geri verdiği **eski** harcamayı
  /// iptal eder. Gider yazılsaydı geri gelen para harcanmış görünürdü.
  refundReceipt;

  static ReceiptDocumentKind parse(Map<String, dynamic> json, String key) =>
      switch (JsonReaders.nullableString(json, key)) {
        'invoice_or_voucher' => ReceiptDocumentKind.invoiceOrVoucher,
        'bank_document' => ReceiptDocumentKind.bankDocument,
        'bank_payment' => ReceiptDocumentKind.bankPayment,
        'bank_card_payment' => ReceiptDocumentKind.bankCardPayment,
        'refund_receipt' => ReceiptDocumentKind.refundReceipt,
        _ => ReceiptDocumentKind.purchaseReceipt,
      };

  /// Ana tutarın ne olduğu kullanıcıya sorulmalı mı.
  bool get needsDecision =>
      this == ReceiptDocumentKind.bankDocument ||
      this == ReceiptDocumentKind.bankPayment ||
      this == ReceiptDocumentKind.bankCardPayment;
}

/// Kullanıcının belgeyi çekmeden **önce** bildirdiği yön.
///
/// Bir belgenin gider mi gelir mi olduğu belgede yazmaz: aynı kira makbuzu
/// kiracı için gider, ev sahibi için gelirdir ve kâğıt aynıdır. Cevabı bilmek,
/// belgedeki taraflardan hangisinin kullanıcı olduğunu bilmeyi gerektirir —
/// okuma değil kimlik sorusudur. Bu yüzden kullanıcı söyler, model söylemez.
///
/// Transfer üçüncü bir yöndür: para harcanmadı ya da kazanılmadı, kullanıcının
/// kendi hesapları arasında taşındı. Dekont gider olarak okunsaydı hiç
/// harcanmamış bir gider yazılır, üstelik çekilen parayla yapılan alışveriş de
/// fişlenince aynı para iki kez sayılırdı.
enum ReceiptCaptureIntent {
  expense,
  income,
  transfer,

  /// Yön değil, **belge sınıfı** bildirimi: "bu bir dekont".
  ///
  /// Dekontta yön diğer belgelerden de zordur: 5.000 TL bir ödeme de olabilir,
  /// borç verme de, kendi hesabına aktarma da, kart borcu ödemesi de — ve
  /// kullanıcı fotoğrafı çekmeden önce belgede ne yazdığını bilmiyor. Bu yüzden
  /// yön burada sorulmaz; okuma bitince, tutar ve karşı taraf ekrandayken karar
  /// sayfasında sorulur. Yön yine kullanıcıdan gelir, yalnız daha geç.
  bankSlip;

  String get wireValue => switch (this) {
    ReceiptCaptureIntent.expense => 'expense',
    ReceiptCaptureIntent.income => 'income',
    ReceiptCaptureIntent.transfer => 'transfer',
    ReceiptCaptureIntent.bankSlip => 'bank_slip',
  };
}

/// Fişte yazan ödeme biçimi ipucu.
///
/// Kaynak seçmez, yalnız kaynak listesini önceler. `unknown`, tanınmayan bir
/// değeri de kapsar: bu alan para yazmadığı için bilinmeyene düşmek güvenlidir.
enum ReceiptPaymentHint {
  unknown,
  cash,

  /// Kartla ödenmiş ama fiş türünü yazmıyor.
  card,
  creditCard,
  debitCard;

  static ReceiptPaymentHint parse(Map<String, dynamic> json, String key) =>
      switch (JsonReaders.nullableString(json, key)) {
        'cash' => ReceiptPaymentHint.cash,
        'credit_card' => ReceiptPaymentHint.creditCard,
        'debit_card' => ReceiptPaymentHint.debitCard,
        'card' => ReceiptPaymentHint.card,
        _ => ReceiptPaymentHint.unknown,
      };
}

/// İade fişinin geri verdiği harcama.
///
/// Bir **öneridir**: sunucu bulur, kullanıcı görür ve onaylar. Yanlış kaydı
/// sessizce iptal etmek, bulamamaktan kötüdür — kullanıcının bakmak için bir
/// sebebi olmazdı.
class ReceiptRefundMatch {
  const ReceiptRefundMatch({
    required this.transactionId,
    required this.transactionDate,
    required this.amount,
    required this.description,
    required this.remainingAmount,
  });

  factory ReceiptRefundMatch.fromJson(Map<String, dynamic> json) =>
      ReceiptRefundMatch(
        transactionId: JsonReaders.string(json, 'transactionId'),
        transactionDate: JsonReaders.date(json, 'transactionDate'),
        amount: JsonReaders.money(json, 'amount'),
        description: JsonReaders.nullableString(json, 'description'),
        remainingAmount: json['remainingAmount'] == null
            ? null
            : JsonReaders.money(json, 'remainingAmount'),
      );

  final String transactionId;
  final String transactionDate;

  /// Harcamanın tutarı; dört ondalıklı string.
  final String amount;
  final String? description;

  /// İade kısmiyse harcamanın ne olması gerektiği; tam iadede `null` ve iptal
  /// tek başına yeterli. **Sunucuda hesaplanır**, burada değil.
  final String? remainingAmount;

  bool get isPartial => remainingAmount != null;
}

/// Sunucunun taslakla birlikte gönderdiği uyarı.
///
/// Metin sunucuda üretilir; istemci uyarı cümlesini yeniden yazmaz, aksi hâlde
/// doğrulayıcı değiştiğinde iki metin ayrışır.
class ReceiptWarning {
  const ReceiptWarning(this.code, this.message);

  factory ReceiptWarning.fromJson(Map<String, dynamic> json) => ReceiptWarning(
    JsonReaders.string(json, 'code'),
    JsonReaders.string(json, 'message'),
  );

  final String code;
  final String message;
}

/// Modelin okuduğu, kullanıcının onaylayacağı taslak.
///
/// Bu bir finansal kayıt değildir ve kendi başına hiçbir şey yazmaz. Okunamayan
/// alan `null` gelir; istemci burada tahmin üretmez, boş bırakır.
class ReceiptDraft {
  const ReceiptDraft({
    required this.documentKind,
    required this.counterpartyName,
    required this.counterpartyState,
    required this.purchasedAt,
    required this.purchasedAtState,
    required this.dueDate,
    required this.dueDateState,
    required this.totalAmount,
    required this.totalAmountState,
    required this.feeAmount,
    required this.feeAmountState,
    required this.installmentCount,
    required this.currencyCode,
    required this.paymentHint,
    required this.categoryId,
    required this.categoryName,
    required this.categoryState,
    required this.warnings,
    this.refundMatch,
  });

  factory ReceiptDraft.fromJson(Map<String, dynamic> json) => ReceiptDraft(
    documentKind: ReceiptDocumentKind.parse(json, 'documentKind'),
    counterpartyName: JsonReaders.nullableString(json, 'counterpartyName'),
    counterpartyState: ReceiptFieldState.parse(json, 'counterpartyState'),
    purchasedAt: _nullableDate(json, 'purchasedAt'),
    purchasedAtState: ReceiptFieldState.parse(json, 'purchasedAtState'),
    dueDate: _nullableDate(json, 'dueDate'),
    dueDateState: ReceiptFieldState.parse(json, 'dueDateState'),
    totalAmount: _nullableMoney(json, 'totalAmount'),
    totalAmountState: ReceiptFieldState.parse(json, 'totalAmountState'),
    feeAmount: _nullableMoney(json, 'feeAmount'),
    feeAmountState: ReceiptFieldState.parse(json, 'feeAmountState'),
    installmentCount: JsonReaders.nullableInt(json, 'installmentCount'),
    currencyCode: JsonReaders.nullableString(json, 'currencyCode'),
    paymentHint: ReceiptPaymentHint.parse(json, 'paymentHint'),
    categoryId: JsonReaders.nullableString(json, 'categoryId'),
    categoryName: JsonReaders.nullableString(json, 'categoryName'),
    categoryState: ReceiptFieldState.parse(json, 'categoryState'),
    refundMatch: json['refundMatch'] == null
        ? null
        : ReceiptRefundMatch.fromJson(JsonReaders.object(json, 'refundMatch')),
    warnings: JsonReaders.list(json, 'warnings')
        .map(
          (item) =>
              ReceiptWarning.fromJson(JsonReaders.object(item, 'warning')),
        )
        .toList(growable: false),
  );

  final ReceiptDocumentKind documentKind;
  final String? counterpartyName;
  final ReceiptFieldState counterpartyState;

  /// `yyyy-MM-dd`; diğer sözleşmelerdeki gibi string kalır.
  final String? purchasedAt;
  final ReceiptFieldState purchasedAtState;

  /// Faturanın son ödeme tarihi; belgenin kendi tarihinin yerine geçmez.
  final String? dueDate;
  final ReceiptFieldState dueDateState;

  /// Belge bir fatura ve ödenip ödenmediği sorulmalı mı.
  ///
  /// Son ödeme tarihi taşıyan bir belge, ödendiğini söylemez — yalnız ne zaman
  /// ödenmesi gerektiğini söyler. Gider olarak yazmak, henüz çıkmamış parayı
  /// çıkmış göstermek olurdu.
  bool get needsPaidQuestion => dueDate != null && !dueDateState.isMissing;

  /// Dört ondalıklı string. İstemci parayı yeniden hesaplamaz, taşır.
  final String? totalAmount;
  final ReceiptFieldState totalAmountState;

  /// Dekonttaki işlem ücreti. Taşınan tutardan **ayrı** durur ve ona asla
  /// eklenmez: transfer parayı taşır, harcanan yalnız bu ücrettir.
  final String? feeAmount;
  final ReceiptFieldState feeAmountState;

  /// Fişte yazan taksit sayısı; iki ve üstü, tek çekimde `null`.
  final int? installmentCount;

  /// Taksitli bir satış mı — tek seferlik tam tutar gideri yerine taksit planı
  /// önerilmeli mi.
  ///
  /// 3.000 TL / 3 taksitlik bir fişi tek gider yazmak, o ayın bütçesini
  /// gerçekte çıkmayan 2.000 TL kadar şişirir.
  bool get isInstallmentSale => (installmentCount ?? 0) >= 2;

  final String? currencyCode;
  final ReceiptPaymentHint paymentHint;

  /// Kullanıcının kendi kategorisine çözülmüş kimlik; çözülemediyse `null`.
  final String? categoryId;
  final String? categoryName;
  final ReceiptFieldState categoryState;

  final List<ReceiptWarning> warnings;

  /// İade fişinde, geri verildiği anlaşılan harcama; bulunamadıysa `null`.
  final ReceiptRefundMatch? refundMatch;

  /// Hiçbir alanı okunamamış taslak.
  ///
  /// Sunucu 200 dönse bile boş bir taslak kullanıcıya "okundu" diye
  /// gösterilmez; ekran bunu boş durum olarak ele alır.
  bool get isEmpty =>
      counterpartyState.isMissing &&
      purchasedAtState.isMissing &&
      totalAmountState.isMissing &&
      categoryState.isMissing;

  /// Belgede gerçekten bir işlem ücreti yazıyor mu.
  ///
  /// Her dekontta ücret olmaz ve modelin ücreti okuyamamış olması da olmaması
  /// demektir. Olmayan bir ücret için ekranda satır göstermek, kullanıcıya
  /// olmayan bir kararı sordurmak olurdu — üstelik "kapat"a basmadığı sürece
  /// sıfırlık bir gider yazma riskiyle.
  ///
  /// Tek kural burada duruyor; karar sayfası da çeviri katmanı da bunu okur,
  /// yoksa iki yer ayrışır ve satır bir yerde çıkıp diğerinde çıkmaz.
  bool get hasFee {
    final fee = feeAmount;
    if (fee == null || feeAmountState.isMissing) return false;
    // Sıfır ücret, ücret yokluğunun başka bir yazılışı.
    return (double.tryParse(fee) ?? 0) > 0;
  }

  /// Kullanıcının onaylamadan önce mutlaka bakması gereken bir şey var mı.
  bool get needsAttention =>
      warnings.isNotEmpty ||
      totalAmountState != ReceiptFieldState.read ||
      purchasedAtState != ReceiptFieldState.read;

  static String? _nullableDate(Map<String, dynamic> json, String key) =>
      json[key] == null ? null : JsonReaders.date(json, key);

  static String? _nullableMoney(Map<String, dynamic> json, String key) =>
      json[key] == null ? null : JsonReaders.money(json, key);
}
