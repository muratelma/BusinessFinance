using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Receipts;

/// <summary>
/// Which side of the document the user is on.
///
/// <para>
/// A document does not say whether it is income or expense: the same rent
/// voucher is an expense for the tenant and income for the landlord, and the
/// paper is identical. Answering it would mean knowing which of the printed
/// parties is the user — an identity question, not a reading question. So the
/// user answers it before the photo is taken, and the model is told the answer
/// rather than asked for it.
/// </para>
/// <para>
/// Getting this wrong is worse than misreading the amount: a wrong amount is off
/// by a percentage, a wrong direction flips the sign of the report.
/// </para>
/// </summary>
public enum ReceiptCaptureIntent
{
    /// <summary>The user paid. Default, and the only direction before 12.11.</summary>
    Expense = 1,

    /// <summary>The user was paid.</summary>
    Income = 2,

    /// <summary>
    /// Neither: the money moved between the user's own accounts. An ATM
    /// withdrawal or a wire slip is not spending — recording one as an expense
    /// counts the same money twice, because the cash it produced gets spent on
    /// receipts the user also scans. Only the fee on such a slip is real
    /// spending.
    /// </summary>
    Transfer = 3,

    /// <summary>
    /// Kullanıcı bir yön değil, bir **belge sınıfı** bildirir: "bu bir dekont".
    /// </summary>
    /// <remarks>
    /// Diğer üçü yönü fotoğraftan önce sorar ve bu doğrudur — bir kira
    /// makbuzunun kiracı için gider, ev sahibi için gelir olduğu belgeden
    /// okunamaz. Ama dekontta soru daha da zordur: 5.000 TL bir ödeme de
    /// olabilir, borç verme de, kendi hesabına aktarma da, kart borcu ödemesi
    /// de. Kullanıcı fotoğrafı çekmeden önce belgede ne yazdığını bilmiyor.
    ///
    /// <para>
    /// Bu yüzden dekontta yön, okuma bittikten sonra — tutar, tarih ve karşı
    /// taraf ekrandayken — karar sayfasında sorulur. Yön yine kullanıcıdan
    /// gelir (ADR 0011 madde 2 bozulmaz), yalnız daha geç. Üst seçicide bunu
    /// sormak, kullanıcıya tahmin ettirip sonra aynı soruyu tekrar sormaktı.
    /// </para>
    /// </remarks>
    BankSlip = 4
}

/// <summary>
/// What the analyzer is asked to read. The category names are the user's own,
/// passed in so the model chooses from a closed set instead of inventing a
/// bucket that does not exist in this budget.
///
/// The intent is passed too, but only so the model knows **which printed party
/// to extract** as the counterparty. It never decides the direction; it is told.
/// </summary>
public sealed record ReceiptAnalysisRequest(
    ReceiptImage Image,
    IReadOnlyList<string> CategoryNames,
    ReceiptCaptureIntent Intent);

/// <summary>
/// The uploaded photo before it has crossed any trust boundary. The use case
/// owns reading the stream and checking its real length; an endpoint-provided
/// length is metadata, not evidence.
/// </summary>
public sealed record AnalyzeReceiptCommand(
    string FileName,
    string ContentType,
    Stream Content,
    long DeclaredLength,
    ReceiptCaptureIntent Intent = ReceiptCaptureIntent.Expense);

/// <summary>
/// Cost and latency of one reading. Carried on the port rather than logged away
/// because the stage 12.10 measurement run compares models on exactly these two
/// numbers plus accuracy — a comparison that cannot be reconstructed later.
/// </summary>
public sealed record ReceiptAnalysisUsage(
    int InputTokens,
    int OutputTokens,
    int TotalTokens,
    TimeSpan Latency);

/// <summary>
/// The model's answer before anything is believed. Every field is a raw string:
/// parsing, range checks and ownership resolution happen in the validator, so a
/// provider that returns "seven hundred" or a category from another budget still
/// lands here without corrupting a typed field.
///
/// Amounts are strings and not decimals on purpose — a JSON number is a float,
/// and 847.50 does not survive a float round trip intact.
/// </summary>
/// <summary>
/// What the photo turned out to be. The pipeline used to assume "a purchase
/// receipt" and had no way to disagree: handed a bank transfer slip, the model
/// complied and mapped its fields onto receipt fields. A wire transfer is not an
/// expense at all — it moves money between accounts and only its fee is spent —
/// so a perfectly read transfer slip still produces a wrong record. The kind is
/// therefore read first and anything but a purchase document is refused.
/// </summary>
public enum ReceiptDocumentKind
{
    /// <summary>Unrecognised value from the provider; treated as not a receipt.</summary>
    Unknown = 0,

    /// <summary>
    /// A retail counter sale: till receipt, market/store receipt, restaurant
    /// bill. Whoever holds this paper is the buyer, so it can only be an expense.
    /// </summary>
    PurchaseReceipt = 1,

    /// <summary>
    /// An invoice, voucher or payslip that names **both** parties: e-arşiv
    /// fatura, kira makbuzu, serbest meslek makbuzu, maaş bordrosu.
    ///
    /// <para>
    /// Deliberately one kind for both directions. The paper cannot tell you which
    /// party is the user, and splitting it into "income document" and "expense
    /// document" would ask the model to infer exactly the thing it must never
    /// infer. The user's intent decides; this kind only says the document is
    /// capable of either.
    /// </para>
    /// </summary>
    InvoiceOrVoucher = 6,

    /// <summary>
    /// A slip that moves money between the user's own accounts: ATM withdrawal,
    /// virman, a havale where the sender and the recipient are the same person.
    /// Refused unless the user declared a transfer.
    /// </summary>
    BankDocument = 2,

    /// <summary>
    /// A bank slip that pays someone else: havale/EFT to a different named
    /// person, or a bill payment naming a company and an invoice number.
    ///
    /// <para>
    /// Split out from <see cref="BankDocument" /> because the two are opposite
    /// economically and the single kind gave wrong advice for both. Money paid
    /// to a third party has left for good — it is spending, not moving — and
    /// refusing it with "kaydedin as a transfer" told the user to record a
    /// destination account that does not exist. The 19 August 2026 field run
    /// found that all three real dekont samples were this kind, not the other.
    /// </para>
    /// <para>
    /// The distinction is read off the paper, not inferred about the user: a
    /// slip naming a different recipient, a company or an invoice number is a
    /// payment. Whether that payment is an expense or a loan is the user's call,
    /// which is what the intent already answers.
    /// </para>
    /// </summary>
    BankPayment = 7,

    /// <summary>
    /// A slip that pays down a credit card: "KREDİ KARTI BORÇ ÖDEMESİ", a card
    /// number, a statement reference.
    ///
    /// <para>
    /// Split from <see cref="BankPayment" /> because recording one as spending
    /// double-counts: the purchases were already counted as card charges and the
    /// payment only settles them. The kind does not refuse anything by itself —
    /// the user still decides — but it lets the app put the right answer first
    /// and say what it sees.
    /// </para>
    /// </summary>
    CardPaymentSlip = 8,

    /// <summary>
    /// A refund slip: money came back. Read as a receipt it would post an expense
    /// with the sign reversed — the user would be charged twice for a purchase
    /// they undid.
    /// </summary>
    RefundReceipt = 5,

    /// <summary>A document, but not one this flow can record.</summary>
    OtherDocument = 3,

    /// <summary>No document in the photo at all.</summary>
    NotADocument = 4
}

public sealed record RawReceiptReading(
    ReceiptDocumentKind DocumentKind,
    string? CounterpartyName,
    string? PurchasedAt,

    /// <summary>
    /// Faturanın son ödeme tarihi. Belgenin kendi tarihinin **yerine geçmez**:
    /// ikisi farklı şeylerdir ve karıştırılırsa henüz ödenmemiş bir fatura
    /// düzenlendiği gün harcanmış gibi görünür.
    /// </summary>
    string? DueDate,
    string? SubtotalAmount,
    string? TaxAmount,
    string? TotalAmount,
    string? FeeAmount,

    /// <summary>
    /// Fişte yazan taksit sayısı ("3 TAKSİT", "6 TAKSITLI"). Tek çekimde boş.
    /// Taksitli bir satış tek seferlik tam tutar gideri değildir: o ay bütçeden
    /// çıkan yalnız bir taksittir.
    /// </summary>
    string? InstallmentCount,
    string? CurrencyCode,
    string? PaymentMethodHint,
    string? CategoryName,
    IReadOnlyList<string> UnreadableFields,
    ReceiptAnalysisUsage Usage);

/// <summary>
/// The validated suggestion plus the measurements needed by the stage 12.10
/// comparison run. The API exposes only the draft; normalization and provider
/// usage remain server-side diagnostics and are never persisted.
/// </summary>
public sealed record AnalyzedReceipt(
    ReceiptDraft Draft,
    ReceiptImageNormalization Normalization,
    ReceiptAnalysisUsage Usage);

public static class ReceiptAnalysisErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "auth.authentication_required",
        "Authentication is required.",
        ApplicationErrorType.Unauthorized);

    /// <summary>
    /// No API key configured. The rest of the application still runs; only this
    /// endpoint reports that the feature is switched off.
    /// </summary>
    public static readonly ApplicationError Disabled = new(
        "receipt.disabled",
        "Fiş okuma şu an kapalı; sunucuda okuma servisi tanımlı değil.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ProviderUnavailable = new(
        "receipt.provider_unavailable",
        "Fiş okuma servisine şu an ulaşılamıyor. Bilgileri elle girebilirsiniz.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ProviderRateLimited = new(
        "receipt.provider_rate_limited",
        "Fiş okuma servisinin kotası doldu. Bir süre sonra tekrar deneyin.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError Unreadable = new(
        "receipt.unreadable",
        "Fiş okunamadı. Daha net bir fotoğraf çekebilir veya bilgileri elle girebilirsiniz.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError FileTooLarge = new(
        "receipt.file_too_large",
        "Fiş fotoğrafı 5 MiB boyut sınırını aşıyor.",
        ApplicationErrorType.Validation);

    public static ApplicationError UnsupportedFile(string reason) => new(
        "receipt.unsupported_file", reason, ApplicationErrorType.Validation);

    /// <summary>
    /// A bank slip is refused rather than read. Havale/EFT is a transfer between
    /// accounts, not an expense; recording it as one both invents an expense that
    /// never happened and hides the only real one, the fee.
    /// </summary>
    public static readonly ApplicationError BankDocument = new(
        "receipt.bank_document",
        "Bu dekont, kendi hesaplarınız arasında bir aktarma. Gider değildir; "
            + "Transfer seçeneğiyle okutun, gider olan yalnız işlem ücretidir.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// A payment slip captured as a transfer. Refused because the money left
    /// the user's accounts: asking for a destination account would invent one.
    /// </summary>
    public static readonly ApplicationError BankPaymentNotTransfer = new(
        "receipt.bank_payment_not_transfer",
        "Bu dekont başkasına yapılan bir ödeme, hesaplarınız arasında aktarma "
            + "değil. Harcama seçeneğiyle okutun.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// A refund is refused rather than read: the flow has no way to record money
    /// coming back, and reading it as an expense inverts the sign.
    /// </summary>
    public static readonly ApplicationError RefundDocument = new(
        "receipt.refund_document",
        "Bu bir iade fişi. İade gider değildir — para geri gelmiştir. "
            + "İlgili gideri iptal edip yeniden yazabilirsiniz.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// The user said one thing and the paper says another. Refused rather than
    /// silently converted: a till receipt captured as income would post money the
    /// user never earned, and the sign is the expensive half of the record.
    /// </summary>
    public static readonly ApplicationError IntentMismatch = new(
        "receipt.intent_mismatch",
        "Bu belge gelir belgesi değil, bir satın alma fişi. Gelir kaydı için "
            + "makbuz, fatura veya bordro fotoğrafı kullanın.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Transfer was declared but the paper is a purchase document. Refused for
    /// the same reason as the other direction: a transfer that is really a
    /// purchase would move money without recording that it was spent.
    /// </summary>
    public static readonly ApplicationError NotATransferDocument = new(
        "receipt.not_a_transfer_document",
        "Bu bir dekont değil, alışveriş belgesi. Transfer kaydı için havale, "
            + "EFT veya ATM makbuzu fotoğrafı kullanın.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Dekont seçildi ama kâğıt bir alışveriş belgesi. Reddedilir: dekont
    /// yolunun sonunda "bu tutar ne?" sorusu var ve o soru bir market fişi için
    /// anlamsızdır.
    /// </summary>
    public static readonly ApplicationError NotABankSlip = new(
        "receipt.not_a_bank_slip",
        "Bu bir dekont değil, alışveriş belgesi. Dekont için havale/EFT, ATM "
            + "veya fatura ödeme makbuzunun fotoğrafını kullanın.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError NotAReceipt = new(
        "receipt.not_a_receipt",
        "Bu fotoğrafta alışveriş fişi görünmüyor. Fişin tamamının kadraja "
            + "girdiği bir fotoğraf deneyin.",
        ApplicationErrorType.Validation);
}

/// <summary>
/// Reads a receipt photo. The single outbound dependency of this application:
/// everything provider-specific lives behind this port, so swapping the provider
/// touches one class and leaves the use case, the API contract and the tests
/// alone.
/// </summary>
public interface IReceiptAnalyzer
{
    Task<ApplicationResult<RawReceiptReading>> AnalyzeAsync(
        ReceiptAnalysisRequest request,
        CancellationToken cancellationToken);
}
