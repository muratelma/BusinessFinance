namespace BusinessFinance.Application.Receipts;

/// <summary>
/// How much a single field can be trusted. Three states rather than a boolean,
/// because "we read it but something does not add up" is genuinely different
/// from both "we read it" and "we did not read it" — and the user needs to be
/// told which one they are looking at.
/// </summary>
public enum ReceiptFieldState
{
    /// <summary>Read and passed every check.</summary>
    Read = 1,

    /// <summary>Read, but a cross-check failed. Shown, and flagged.</summary>
    Suspect = 2,

    /// <summary>Not readable, or read and rejected. Left blank, never guessed.</summary>
    Missing = 3
}

/// <summary>
/// What the receipt says about how it was paid. Narrows the source picker; it
/// never chooses the account or card, because the receipt does not say which one
/// and a wrong source turns a card charge into an account expense.
///
/// <para>
/// Credit and debit are separate members because in this app they are separate
/// write models: a credit card payment becomes a <c>CreditCardCharge</c>, a bank
/// card payment is an ordinary expense on an <c>Account</c> — users keep a "banka
/// kartı" account for exactly that. Collapsing both into one "card" hint pointed
/// every card receipt at the credit cards.
/// </para>
/// <para>
/// <see cref="Card" /> stays because most Turkish receipts print only "KART" or
/// "POS" without the type, and some ÖKC devices print "KREDİ KARTI" for a debit
/// payment. Where the receipt does not say, the reading does not say either;
/// guessing here would put a bank-card expense on a credit card.
/// </para>
/// </summary>
public enum ReceiptPaymentHint
{
    Unknown = 0,
    Cash = 1,

    /// <summary>Paid by card, but the receipt does not say which kind.</summary>
    Card = 2,
    CreditCard = 3,
    DebitCard = 4
}

public sealed record ReceiptWarning(string Code, string Message);

/// <summary>One of the user's own categories, offered to the model and back.</summary>
public sealed record ReceiptCategoryOption(Guid Id, string Name);

/// <summary>
/// A suggestion, never a record. Nothing here has been written anywhere; the
/// user edits it, chooses a payment source, and confirms before any of it
/// becomes an expense.
///
/// Subtotal and tax are deliberately absent: they are read only to check that
/// the totals add up, and this stage does not store line-level money.
/// </summary>
/// <remarks>
/// <see cref="CounterpartyName" /> bilerek "merchant" değildir: bir belgenin
/// yönü belgede yazmaz, bakan tarafa göre değişir. Aynı kira makbuzu kiracı
/// için gider, ev sahibi için gelirdir — "satıcı" adı gelir belgesinde
/// kullanıcının kendisini gösterirdi ve kayıt listesinde işe yaramazdı. Karşı
/// taraf, yön ne olursa olsun **işlemin öteki ucundaki** kişi veya kurumdur.
/// </remarks>
public sealed record ReceiptDraft(
    /// <summary>
    /// Hangi tür belge okundu. Taslakta duruyor çünkü istemcinin bir sonraki
    /// adımı buna bağlı: banka belgesinde ana tutarın ne olduğu belgeden
    /// okunamaz (ödeme mi, aktarma mı, borç verme mi) ve kullanıcıya sorulur.
    /// Alışveriş belgesinde böyle bir soru yoktur.
    /// </summary>
    ReceiptDocumentKind DocumentKind,
    string? CounterpartyName,
    ReceiptFieldState CounterpartyState,

    /// <summary>
    /// Okunan adın eşleştiği <b>mevcut</b> karşı taraf; yalnız bir öneri.
    /// </summary>
    /// <remarks>
    /// Dolu olması hiçbir şeyin yazıldığı anlamına gelmez (ADR 0011): model
    /// karşı tarafı <b>seçmez</b>, uygulama okunan adı kullanıcının kendi
    /// kayıtlarında arar ve bulduğunu gösterir. Kullanıcı reddederse alan
    /// boşalır ve ad düz metin olarak kalır; onaylarsa kayıt o karşı tarafa
    /// bağlanır.
    ///
    /// Boş olması bir hata değildir: o adla ilk kez iş yapılıyor olabilir ve
    /// karşı taraf ilk kayıtla birlikte kurulur.
    ///
    /// Eşleşme <b>tam ad</b> üzerinedir (harf duyarsız). Benzeyen adı
    /// eşleştirmek, "Ahmet Market" ile "Ahmet Manav"ı aynı kişi saymak
    /// olurdu — model önerir, uygulama uydurmaz.
    /// </remarks>
    Guid? CounterpartyId,
    DateOnly? PurchasedAt,
    ReceiptFieldState PurchasedAtState,

    /// <summary>
    /// <summary>
    /// Fişte yazan taksit sayısı; iki ve üstü. Tek çekimde <c>null</c>.
    /// Taksitli bir satış tek seferlik tam tutar gideri **değildir**: o ay
    /// bütçeden çıkan yalnız bir taksittir.
    /// </summary>
    int? InstallmentCount,

    /// Faturanın son ödeme tarihi. Belgenin kendi tarihinin yerine geçmez:
    /// ikisi karıştırılırsa henüz ödenmemiş bir fatura, düzenlendiği gün
    /// harcanmış görünür. Dolu olması istemciye "ödendi mi?" diye sordurur.
    /// </summary>
    DateOnly? DueDate,
    ReceiptFieldState DueDateState,
    decimal? TotalAmount,
    ReceiptFieldState TotalAmountState,

    /// <summary>
    /// The fee printed on a transfer slip, kept apart from the amount moved.
    /// On a dekont these are two numbers on two lines and adding them produced
    /// a 5.004,50 expense that was never spent; the transfer moves money without
    /// spending it, and only this fee is real spending.
    /// </summary>
    decimal? FeeAmount,
    ReceiptFieldState FeeAmountState,
    string? CurrencyCode,
    ReceiptPaymentHint PaymentHint,
    Guid? CategoryId,
    string? CategoryName,
    ReceiptFieldState CategoryState,
    IReadOnlyList<ReceiptWarning> Warnings,

    /// <summary>
    /// İade fişinde, geri verildiği anlaşılan harcama. Yalnız
    /// <c>refund_receipt</c> okunduğunda dolabilir ve dolu olması bile bir
    /// **öneridir**: iptal ancak kullanıcı kaydı görüp onayladığında olur.
    /// </summary>
    ReceiptRefundMatch? RefundMatch = null);

public static class ReceiptWarnings
{
    public const string FieldUnreadable = "receipt.field_unreadable";
    public const string AmountUnparsed = "receipt.amount_unparsed";
    public const string AmountOutOfRange = "receipt.amount_out_of_range";
    public const string DateUnparsed = "receipt.date_unparsed";
    public const string DateInFuture = "receipt.date_in_future";
    public const string DateTooOld = "receipt.date_too_old";
    public const string TotalsDoNotAddUp = "receipt.totals_do_not_add_up";
    public const string CategoryUnknown = "receipt.category_unknown";
    public const string CurrencyUnexpected = "receipt.currency_unexpected";

    /// <summary>
    /// A record with the same date, amount and counterparty already exists.
    /// A warning and not a refusal: buying the same coffee twice in a day is
    /// ordinary, and blocking it would make the flow wrong more often than the
    /// double entry it prevents.
    /// </summary>
    public const string PossibleDuplicate = "receipt.possible_duplicate";
}
