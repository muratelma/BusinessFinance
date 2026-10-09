using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Obligations;

public static class ObligationErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError CategoryUnavailable = new(
        "obligations.category_unavailable",
        "An active owned category of the matching type is required.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CounterpartyUnavailable = new(
        "obligations.counterparty_unavailable",
        "The counterparty was not found or cannot take on a new obligation.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError ScopeUnresolved = new(
        "obligations.scope_unresolved",
        "The scope could not be resolved from the request or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "obligations.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
        ApplicationErrorType.Validation);

    /// <summary>Kartla (POS) yalnız alacak kapanır; ödenecek fatura POS'tan geçmez.</summary>
    public static readonly ApplicationError CardRequiresReceivable = new(
        "obligations.card_requires_receivable",
        "Only a receivable can be collected by card.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError AccountUnavailable = new(
        "obligations.account_unavailable",
        "An active owned account with the matching currency is required.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Alacak kartla tahsil edilmiş ve parası bir yatışla hesaba geçmiş; önce
    /// yatış geri alınır (ADR 0019 T5).
    /// </summary>
    public static readonly ApplicationError DepositLocked = new(
        "obligations.deposit_locked",
        "An obligation collected by card that reached the account through a deposit cannot be cancelled; revert the deposit first.",
        ApplicationErrorType.Conflict);

    /// <summary>Kapanışı bir gün sonunda sayıldı; önce gün sonu geri alınır.</summary>
    public static readonly ApplicationError DayCloseCounted = new(
        "obligations.day_close_counted",
        "An obligation whose settlement was counted in a day close cannot be cancelled; revert the day close first.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError Changed = new(
        "obligations.concurrent_change",
        "The obligation changed while it was being cancelled; reload and try again.",
        ApplicationErrorType.Conflict);

    public static ApplicationError NotFound(Guid id) => new(
        "obligations.not_found",
        $"Obligation '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError Conflict(string message) => new(
        "obligations.conflict",
        message,
        ApplicationErrorType.Conflict);

    public static ApplicationError Validation(string message) => new(
        "obligations.validation",
        message,
        ApplicationErrorType.Validation);
}
