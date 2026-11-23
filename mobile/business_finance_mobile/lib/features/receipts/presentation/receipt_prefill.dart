import '../../activities/presentation/quick_add_models.dart';
import '../../cards/presentation/transfer_prefill.dart';
import '../../debt_book/data/debt_book_models.dart';
import '../../debt_book/presentation/debt_entry_prefill.dart';
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

/// Okunan dekontu `Borç verdim` formunun anladığı önerilere çevirir.
///
/// **Hesap taşınmıyor**: dekont paranın hangi hesaptan çıktığını söylemez.
/// Vade de taşınmıyor, çünkü belgede yazmıyor.
///
/// Sunucunun bulduğu kişi **eşleşmesi** kişi önerisi olur: yanlış eşleşme iki
/// kişinin hesabını karıştırırdı, bu yüzden kişi formda görünür ve
/// değiştirilebilir (ADR 0011: model önerir, kullanıcı onaylar). Eşleşme
/// yoksa okunan ad açıklamaya gider; kişiyi kullanıcı seçer ya da ekler.
DebtEntryPrefill receiptLendingPrefillFrom(
  ReceiptDraft draft, {
  bool withFee = true,
}) {
  final person = _matchedPerson(draft);
  return DebtEntryPrefill(
    person: person,
    amount: _read(draft.totalAmount, draft.totalAmountState),
    date: _read(draft.purchasedAt, draft.purchasedAtState),
    description: person == null
        ? _read(draft.counterpartyName, draft.counterpartyState)
        : null,
    notices: _notices(draft),
    feeAmount: withFee && draft.hasFee ? draft.feeAmount : null,
    feeDescription: _feeDescription(draft),
  );
}

/// Ödenmemiş faturayı `Vadeli aldım` formunun anladığı önerilere çevirir
/// (K10): kişi eşleştiyse kişinin hesabına vadeli alım, eşleşmediyse okunan
/// ad kaydın açıklamasıdır ve kayıt kişisiz yazılabilir.
///
/// **Ödeme kaynağı taşınmıyor**: para henüz çıkmadı. Belge tarihi giderin
/// tanındığı gün, son ödeme tarihi vadedir; iki alan ayrı taşınır. Yön
/// sorulmaz: "henüz ödemedim" cevabı borçlu olduğunu söyledi.
DebtEntryPrefill receiptPurchasePrefillFrom(ReceiptDraft draft) {
  final person = _matchedPerson(draft);
  return DebtEntryPrefill(
    person: person,
    amount: _read(draft.totalAmount, draft.totalAmountState),
    date: _read(draft.purchasedAt, draft.purchasedAtState),
    dueDate: _read(draft.dueDate, draft.dueDateState),
    description: person == null
        ? _read(draft.counterpartyName, draft.counterpartyState)
        : null,
    categoryId: _read(draft.categoryId, draft.categoryState),
    notices: _notices(draft),
  );
}

DebtPersonChoice? _matchedPerson(ReceiptDraft draft) {
  final id = draft.counterpartyId;
  final name = draft.counterpartyName;
  if (draft.counterpartyState.isMissing ||
      id == null ||
      name == null ||
      name.trim().isEmpty) {
    return null;
  }
  return DebtPersonChoice(id: id, name: name);
}

String? _read(String? value, ReceiptFieldState state) =>
    value == null || value.isEmpty || state.isMissing ? null : value;

/// Belgenin uyarıları; okunan bir alan emin değilse bir cümle daha.
List<String> _notices(ReceiptDraft draft) => [
  for (final warning in draft.warnings) warning.message,
  if ([
    draft.totalAmountState,
    draft.purchasedAtState,
    draft.dueDateState,
    draft.counterpartyState,
  ].any((state) => state.isSuspect))
    'Belgeden okunan bazı alanlardan emin değiliz; kaydetmeden önce kontrol '
        'edin.',
];

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
