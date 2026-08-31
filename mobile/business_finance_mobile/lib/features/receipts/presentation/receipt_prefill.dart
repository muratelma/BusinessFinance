import '../../activities/presentation/quick_add_models.dart';
import '../../cards/presentation/transfer_prefill.dart';
import '../../debts/presentation/lending_prefill.dart';
import '../../obligations/data/obligation_direction.dart';
import '../../obligations/presentation/obligation_prefill.dart';
import '../data/receipt_models.dart';
import '../data/receipt_photo.dart';

/// Okunan taslağı, formun anladığı önerilere çevirir.
///
/// Çeviri tek yönlüdür ve tek yerdedir: form fiş sözleşmesini tanımaz, fiş
/// akışı da formun iç durumunu tanımaz. Aradaki tek bağ bu dosya.
QuickAddPrefill receiptPrefillFrom(
  ReceiptDraft draft, {
  ReceiptPhoto? photo,
  bool keepPhoto = true,
  QuickAddAutoFee? autoFee,
}) => QuickAddPrefill(
  autoFee: autoFee,
  amount: _suggestion(draft.totalAmount, draft.totalAmountState),
  date: _suggestion(draft.purchasedAt, draft.purchasedAtState),
  categoryId: _suggestion(draft.categoryId, draft.categoryState),
  // Kaydın adı fişteki işletme adıdır. Kategori adı kullanılmaz: kategori
  // paylaşılan bir kovadır, aynı kategorideki üç fiş birbirinden ayırt
  // edilemez hâle gelirdi.
  description: _suggestion(draft.counterpartyName, draft.counterpartyState),
  // Banka kartı bu uygulamada bir hesaptır (kullanıcı "banka kartı" adıyla bir
  // hesap açar); kredi kartı ayrı bir yazma modelidir. İkisini tek "kart"
  // ipucunda birleştirmek, banka kartı fişini kredi kartına yönlendiriyordu.
  sourceHint: switch (draft.paymentHint) {
    ReceiptPaymentHint.cash => PaymentSourceHint.cash,
    ReceiptPaymentHint.creditCard => PaymentSourceHint.creditCard,
    ReceiptPaymentHint.debitCard => PaymentSourceHint.debitCard,
    ReceiptPaymentHint.card => PaymentSourceHint.card,
    ReceiptPaymentHint.unknown => null,
  },
  // Belgede yazan KDV forma taşınır: okunabilen bir bilgiyi kullanıcıya
  // yeniden yazdırmak, muhasebeci paketini elle doldurtmak olurdu. Okunmadıysa
  // boş kalır — istemci de sunucu da oranı tutardan (ya da tersini) türetmez.
  vat: draft.vatState.isMissing ? null : draft.vat,
  warnings: draft.warnings
      .map((warning) => warning.message)
      .toList(growable: false),
  // Saklanacak olan **orijinal** byte'lar; analize giden küçültülmüş kopya
  // değil. Belge kanıttır, sıkıştırılmış hâli kanıt olmaz.
  attachment: photo == null
      ? null
      : QuickAddAttachment(
          bytes: photo.originalBytes,
          fileName: photo.originalFileName,
          mediaType: photo.originalMediaType,
        ),
  keepAttachmentByDefault: keepPhoto,
);

/// Okunamayan alan öneri üretmez.
///
/// `missing` durumunda değer zaten `null`; boş bir öneri yazmak, kullanıcıya
/// "burası okundu ve boş çıktı" dedirtirdi. Tahmin yok, boş alan var.
QuickAddSuggestion? _suggestion(String? value, ReceiptFieldState state) {
  if (value == null || value.isEmpty || state.isMissing) return null;
  return QuickAddSuggestion(
    value,
    state.isSuspect
        ? QuickAddSuggestionState.suspect
        : QuickAddSuggestionState.read,
  );
}

/// Okunan dekontu transfer formunun anladığı önerilere çevirir.
///
/// Kaynak ve hedef hesap **taşınmıyor**: dekont hangi hesabın kullanıldığını
/// söylemez ve yanlış kaynak, transferi bambaşka bir para hareketine çevirir.
/// İki ucu da kullanıcı seçer.
TransferPrefill receiptTransferPrefillFrom(
  ReceiptDraft draft, {
  bool withFee = true,
}) => TransferPrefill(
  amount: draft.totalAmountState.isMissing ? null : draft.totalAmount,
  date: draft.purchasedAtState.isMissing ? null : draft.purchasedAt,
  description: draft.counterpartyState.isMissing
      ? null
      : draft.counterpartyName,
  // Ücret ayrı kalır: transfere eklenirse hiç harcanmamış bir tutar taşınmış
  // görünür ve gerçekten harcanan ücret hiçbir yere yazılmaz.
  feeAmount: withFee && draft.hasFee ? draft.feeAmount : null,
  feeDescription: _feeDescription(draft),
);

/// Okunan dekontu kart ödeme formunun anladığı önerilere çevirir.
///
/// **Kart taşınmıyor**: dekont hangi kartın borcunun kapatıldığını söylemez.
CardPaymentPrefill receiptCardPaymentPrefillFrom(
  ReceiptDraft draft, {
  bool withFee = true,
}) => CardPaymentPrefill(
  amount: draft.totalAmountState.isMissing ? null : draft.totalAmount,
  date: draft.purchasedAtState.isMissing ? null : draft.purchasedAt,
  description: draft.counterpartyState.isMissing
      ? null
      : draft.counterpartyName,
  feeAmount: withFee && draft.hasFee ? draft.feeAmount : null,
  feeDescription: _feeDescription(draft),
);

/// Okunan dekontu alacak formunun anladığı önerilere çevirir.
///
/// **Hesap taşınmıyor**: dekont paranın hangi hesaptan çıktığını söylemez.
/// Vade ve taksit sayısı da taşınmıyor, çünkü belgede yazmıyor — onların
/// varsayımı formda kuruluyor ve orada değiştirilebiliyor.
///
/// Sunucunun bulduğu karşı taraf **eşleşmesi** taşınıyor: ad okunamadıysa
/// eşleşme de yoktur, o yüzden ikisi aynı koşula bağlı.
LendingPrefill receiptLendingPrefillFrom(
  ReceiptDraft draft, {
  bool withFee = true,
}) => LendingPrefill(
  counterpartyName: draft.counterpartyState.isMissing
      ? null
      : draft.counterpartyName,
  matchedCounterpartyId: draft.counterpartyState.isMissing
      ? null
      : draft.counterpartyId,
  amount: draft.totalAmountState.isMissing ? null : draft.totalAmount,
  date: draft.purchasedAtState.isMissing ? null : draft.purchasedAt,
  feeAmount: withFee && draft.hasFee ? draft.feeAmount : null,
  feeDescription: _feeDescription(draft),
);

/// Ödenmemiş faturayı yükümlülük formunun anladığı önerilere çevirir.
///
/// **Ödeme kaynağı taşınmıyor**: fatura hangi hesaptan ödeneceğini söylemez.
/// Belge tarihi ekonomik olayın tanındığı gün, son ödeme tarihi ise vadedir;
/// iki alan ayrı taşınır. Ödeme kaynağı taşınmaz, çünkü para henüz çıkmamıştır.
///
/// Yön **sabittir**: "henüz ödemedim" cevabı borçlu olduğunu söyler. Formda
/// yeniden sormak, kullanıcının bir adım önce verdiği cevabı tekrar istemek
/// olurdu (ADR 0011: model yönü seçmez, kullanıcı seçer — burada seçti).
ObligationPrefill receiptObligationPrefillFrom(ReceiptDraft draft) =>
    ObligationPrefill(
      direction: ObligationDirection.payable,
      amount: _suggestion(draft.totalAmount, draft.totalAmountState),
      issueDate: _suggestion(draft.purchasedAt, draft.purchasedAtState),
      dueDate: _suggestion(draft.dueDate, draft.dueDateState),
      description: _suggestion(draft.counterpartyName, draft.counterpartyState),
      categoryId: _suggestion(draft.categoryId, draft.categoryState),
      counterpartyId: draft.counterpartyState.isMissing
          ? null
          : draft.counterpartyId,
      // Faturanın KDV'si burada da taşınır. Taşınmadığı sürece, okunan KDV
      // yalnız "ödedim" yolunda kayda giriyordu; ödenmemiş fatura muhasebeci
      // paketine KDV'siz gidiyordu — oysa vadesi gelmemiş olması KDV'sini
      // değiştirmez.
      vat: draft.vatState.isMissing ? null : draft.vat,
      warnings: draft.warnings
          .map((warning) => warning.message)
          .toList(growable: false),
    );

/// Taksitli fişi plan formunun anladığı önerilere çevirir.
///
/// **Kart taşınmıyor** ve taksit tutarı **hesaplanmıyor**: fiş kartı söylemez,
/// bölmeyi de sunucu yapar.
InstallmentPrefill receiptInstallmentPrefillFrom(ReceiptDraft draft) =>
    InstallmentPrefill(
      totalAmount: draft.totalAmountState.isMissing ? null : draft.totalAmount,
      installmentCount: draft.installmentCount,
      firstInstallmentDate: draft.purchasedAtState.isMissing
          ? null
          : draft.purchasedAt,
      description: draft.counterpartyState.isMissing
          ? null
          : draft.counterpartyName,
      categoryId: draft.categoryState.isMissing ? null : draft.categoryId,
    );

/// Ücret kaydının adı; karşı taraf okunduysa onunla.
String _feeDescription(ReceiptDraft draft) {
  final counterparty = draft.counterpartyState.isMissing
      ? null
      : draft.counterpartyName;
  return counterparty == null ? 'İşlem ücreti' : 'İşlem ücreti — $counterparty';
}

/// Dekonttaki işlem ücretini, ana kayıtla birlikte yazılacak ücrete çevirir.
///
/// Ana tutardan **bağımsızdır**: ana tutar harcama, aktarma, kart ödemesi veya
/// borç verme olabilir; banka o ücreti her durumda aldı ve geri gelmeyecek.
///
/// Kendi formu yok. 1,20 veya 4,50 gibi bir tutar için kullanıcıyı ikinci bir
/// forma çağırmak — üstelik ödeme kaynağını yeniden seçtirerek — kaydın
/// kendisinden pahalı bir iş yüküydü. Kaynak artık sorulmuyor: banka ücreti,
/// ana tutarın çıktığı yerden alır.
QuickAddAutoFee? receiptAutoFeeFrom(ReceiptDraft draft) {
  if (!draft.hasFee) return null;
  return QuickAddAutoFee(
    amount: draft.feeAmount!,
    description: _feeDescription(draft),
  );
}
