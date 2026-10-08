using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Transactions;

public static class TransactionErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);
    public static ApplicationError Validation(string message) => new(
        "transactions.validation",
        message,
        ApplicationErrorType.Validation);
    public static ApplicationError NotFound(Guid id) => new(
        "transactions.not_found",
        $"Transaction '{id}' was not found.",
        ApplicationErrorType.NotFound);
    public static readonly ApplicationError AccountUnavailable = new(
        "transactions.account_unavailable",
        "The account was not found or is inactive.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError CategoryUnavailable = new(
        "transactions.category_unavailable",
        "The category was not found or is inactive.",
        ApplicationErrorType.Validation);
    /// <summary>
    /// Kapsam ne istekten, ne hesaptan, ne kategoriden çözülebildi. Sunucu bir
    /// değer uydurmaz: yanlış etiketlenmiş kayıt işletme netini sessizce bozar.
    /// </summary>
    public static readonly ApplicationError ScopeUnresolved = new(
        "transactions.scope_unresolved",
        "The scope could not be resolved from the request, the account or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "transactions.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
        ApplicationErrorType.Validation);
    /// <summary>
    /// Kayıt bir gün sonunda sayıldı (tutardan düşüldü); iptal edilseydi günün
    /// geliri sessizce eksilirdi. Önce gün sonu geri alınır.
    /// </summary>
    public static readonly ApplicationError DayCloseCounted = new(
        "transactions.day_close_counted",
        "A transaction counted in a day close cannot be cancelled; revert the day close first.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError CancelOriginLocked = new(
        "transactions.cancel_origin_locked",
        "A transaction produced by a recurring plan cannot be cancelled.",
        ApplicationErrorType.Conflict);
}
