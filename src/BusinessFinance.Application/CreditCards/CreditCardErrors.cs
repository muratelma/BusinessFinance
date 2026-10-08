using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.CreditCards;

public static class CreditCardErrors
{
    /// <summary>
    /// Kapsam ne istekten, ne karttan, ne kategoriden çözülebildi.
    /// </summary>
    public static readonly ApplicationError ScopeUnresolved = new(
        "credit_cards.scope_unresolved",
        "The scope could not be resolved from the request, the card or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "credit_cards.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);
    public static ApplicationError Validation(string message) => new(
        "credit_cards.validation",
        message,
        ApplicationErrorType.Validation);
    public static ApplicationError NotFound(Guid id) => new(
        "credit_cards.not_found",
        $"Credit card '{id}' was not found.",
        ApplicationErrorType.NotFound);

    /// <summary>
    /// Taksit planının yokluğu bir doğrulama hatası değildir. Aşama 06.1 Grup
    /// 3'e kadar öyleydi: gerçekleştirme ucu bulunamayan plan için 400
    /// dönüyordu ve başka kullanıcıya ait plan da aynı 400'ü alıyordu. Sızıntı
    /// değildi ama ürünün kendi kuralını bozuyordu — yok olan kayıt ve
    /// başkasının kaydı <b>aynı 404'e</b> gider.
    /// </summary>
    public static ApplicationError InstallmentPlanNotFound(Guid id) => new(
        "installments.not_found",
        $"Installment plan '{id}' was not found.",
        ApplicationErrorType.NotFound);
    public static readonly ApplicationError DuplicateName = new(
        "credit_cards.duplicate_name",
        "A credit card with the same name already exists.",
        ApplicationErrorType.Conflict);
    public static readonly ApplicationError LimitExceeded = new(
        "credit_cards.limit_exceeded",
        "The charge would exceed the credit card limit.",
        ApplicationErrorType.Conflict);
    public static readonly ApplicationError PaymentExceedsDebt = new(
        "credit_cards.payment_exceeds_debt",
        "The payment cannot exceed the current card debt.",
        ApplicationErrorType.Conflict);
    public static ApplicationError ChargeNotFound(Guid id) => new(
        "credit_cards.charge_not_found",
        $"Credit card charge '{id}' was not found.",
        ApplicationErrorType.NotFound);
    public static ApplicationError PaymentNotFound(Guid id) => new(
        "credit_cards.payment_not_found",
        $"Credit card payment '{id}' was not found.",
        ApplicationErrorType.NotFound);
    public static readonly ApplicationError CancelOriginLocked = new(
        "credit_card_charges.cancel_origin_locked",
        "A charge produced by a recurring plan or an installment cannot be cancelled.",
        ApplicationErrorType.Conflict);
}
