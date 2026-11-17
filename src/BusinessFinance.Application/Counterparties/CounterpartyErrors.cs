using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Counterparties;

public static class CounterpartyErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static ApplicationError Validation(string message) => new(
        "counterparties.validation",
        message,
        ApplicationErrorType.Validation);

    public static ApplicationError NotFound(Guid id) => new(
        "counterparties.not_found",
        $"Counterparty '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError DuplicateName = new(
        "counterparties.duplicate_name",
        "A counterparty with the same name already exists.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Hareketi olan karşı taraf silinmez; boş hesap kuralının aynısı.
    /// </summary>
    public static readonly ApplicationError HasHistory = new(
        "counterparties.has_history",
        "A counterparty with movements cannot be deleted; deactivate it instead.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Pasif karşı tarafa yeni borçlandırma yazılamaz — tahsilat yazılabilir.
    /// </summary>
    public static readonly ApplicationError Inactive = new(
        "counterparties.inactive",
        "An inactive counterparty cannot take on a new charge.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Satışın, alımın, tahsilatın ya da ödemenin günü ileride. Vade ileride
    /// olabilir; olmuş olan olamaz.
    /// </summary>
    public static readonly ApplicationError DateInFuture = new(
        "counterparties.date_in_future",
        "The charge date and the payment date cannot be in the future.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ScopeUnresolved = new(
        "counterparties.scope_unresolved",
        "The scope could not be resolved from the request or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "counterparties.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CategoryUnavailable = new(
        "counterparties.category_unavailable",
        "An active owned category of the matching type is required.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError AccountUnavailable = new(
        "counterparties.account_unavailable",
        "An active owned account is required.",
        ApplicationErrorType.Validation);

    public static ApplicationError DiscountNotFound(Guid id) => new(
        "counterparty_charges.discount_not_found",
        $"Discount '{id}' was not found on this charge.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError ChargeCancelled = new(
        "counterparty_charges.cancelled",
        "A cancelled charge cannot change.",
        ApplicationErrorType.Conflict);

    /// <summary>İndirim alımın kalan bedelini aşamaz.</summary>
    public static readonly ApplicationError DiscountExceedsCharge = new(
        "counterparty_charges.discount_exceeds_amount",
        "The discount cannot exceed what is left of the charge.",
        ApplicationErrorType.Validation);

    public static ApplicationError ChargeNotFound(Guid id) => new(
        "counterparty_charges.not_found",
        $"Counterparty charge '{id}' was not found.",
        ApplicationErrorType.NotFound);

    /// <summary>Tahsilat bir gün sonunda sayıldı; önce gün sonu geri alınır.</summary>
    /// <summary>
    /// Kartla tahsilin parası bir yatışla hesaba geçti; önce yatış geri alınır
    /// (ADR 0019 T5).
    /// </summary>
    public static readonly ApplicationError PaymentDepositLocked = new(
        "counterparty_payments.deposit_locked",
        "A card collection that reached the account through a deposit cannot be cancelled; revert the deposit first.",
        ApplicationErrorType.Conflict);

    /// <summary>Kartla yalnız tahsilat alınır; tedarikçiye ödeme POS'tan geçmez.</summary>
    public static readonly ApplicationError CardRequiresCollection = new(
        "counterparty_payments.card_requires_collection",
        "Only a collection can be taken by card.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError PaymentChanged = new(
        "counterparty_payments.concurrent_change",
        "The payment changed while it was being saved; reload and try again.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError ChargeDayCloseCounted = new(
        "counterparty_charges.day_close_counted",
        "A sale counted in a day close cannot be cancelled; revert the day close first.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError PaymentDayCloseCounted = new(
        "counterparty_payments.day_close_counted",
        "A payment counted in a day close cannot be cancelled; revert the day close first.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Kişiye kredi kartıyla yalnız ödeme yapılır (borç yönü); hesapla ya da
    /// kartla tahsille birlikte verilemez.
    /// </summary>
    public static readonly ApplicationError CardRequiresPayment = new(
        "counterparties.card_requires_payment",
        "A credit card can only pay a counterparty, alone.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CardUnavailable = new(
        "counterparties.card_unavailable",
        "An active owned credit card is required.",
        ApplicationErrorType.Validation);

    /// <summary>Vazgeçilen tutar o taraftaki kalanı aşıyor.</summary>
    public static readonly ApplicationError WriteOffExceedsBalance = new(
        "counterparties.write_off_exceeds_balance",
        "A write-off cannot exceed what is left on that side.",
        ApplicationErrorType.Validation);

    public static ApplicationError WriteOffNotFound(Guid id) => new(
        "counterparties.write_off_not_found",
        $"Write-off '{id}' was not found.",
        ApplicationErrorType.NotFound);

    /// <summary>
    /// <c>Kalanı indirim say</c>: kalan, indirim düşülebilecek alımlardan büyük
    /// (devreden bakiye ya da borç paradan geliyor, ya da alım gün sonunda
    /// sayılmış). Kalan vazgeçmeyle kapatılır.
    /// </summary>
    public static readonly ApplicationError DiscountExceedsCharges = new(
        "counterparties.discount_exceeds_charges",
        "The remainder is larger than what the open purchases can be discounted; write it off instead.",
        ApplicationErrorType.Validation);

    /// <summary>Ödemeyle birlikte yazılan indirim tek başına geri alınamaz.</summary>
    public static readonly ApplicationError DiscountBelongsToPayment = new(
        "counterparty_charges.discount_belongs_to_payment",
        "This discount was written with a payment; cancel the payment instead.",
        ApplicationErrorType.Conflict);

    /// <summary>Satışla birlikte yazılan peşin ya da avans tutarı satışı aşıyor.</summary>
    public static readonly ApplicationError PaidNowExceedsCharge = new(
        "counterparties.paid_now_exceeds_charge",
        "The amount paid now cannot exceed the charge.",
        ApplicationErrorType.Validation);

    public static ApplicationError LoanNotFound(Guid id) => new(
        "counterparties.loan_not_found",
        $"Loan '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError LoanChanged = new(
        "counterparties.concurrent_change",
        "The record changed while it was being saved. Read it again.",
        ApplicationErrorType.Conflict);

    public static ApplicationError PaymentNotFound(Guid id) => new(
        "counterparty_payments.not_found",
        $"Counterparty payment '{id}' was not found.",
        ApplicationErrorType.NotFound);
}
