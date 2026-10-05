using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Pos;

public static class PosSettlementErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError AccountUnavailable = new(
        "pos_settlements.account_unavailable",
        "An active owned bank account with the matching currency is required.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError CategoryUnavailable = new(
        "pos_settlements.category_unavailable",
        "An active owned income category is required.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CommissionCategoryUnavailable = new(
        "pos_settlements.commission_category_unavailable",
        "An active owned expense category is required for the commission.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CommissionAmbiguous = new(
        "pos_settlements.commission_ambiguous",
        "Send the commission either as an amount or as a rate, not both.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ScopeUnresolved = new(
        "pos_settlements.scope_unresolved",
        "The scope could not be resolved from the request, the account or the category.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError DefinitionUnavailable = new(
        "pos_settlements.definition_unavailable",
        "An owned pos definition is required.",
        ApplicationErrorType.NotFound);

    /// <summary>Tanım verilmediğinde formun kendi doldurması gereken alanlar.</summary>
    public static readonly ApplicationError DetailsRequired = new(
        "pos_settlements.details_required",
        "Account, category and expected transfer date are required without a pos definition.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Yatışa bağlı tahsilat iptal edilemez; önce yatış geri alınır
    /// (ADR 0019 T5).
    /// </summary>
    public static readonly ApplicationError DepositLocked = new(
        "pos_settlements.deposit_locked",
        "A deposited pos settlement cannot be cancelled; revert the deposit first.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Gün sonunun ürettiği tahsilat tek başına iptal edilemez; gün sonu bir
    /// bütün olarak geri alınır (ADR 0019 T1).
    /// </summary>
    public static readonly ApplicationError DayCloseLocked = new(
        "pos_settlements.day_close_locked",
        "A pos settlement produced by a day close cannot be cancelled; revert the day close.",
        ApplicationErrorType.Conflict);

    /// <summary>Tahsilat bir gün sonunda sayıldı; önce gün sonu geri alınır.</summary>
    public static readonly ApplicationError DayCloseCounted = new(
        "pos_settlements.day_close_counted",
        "A pos settlement counted in a day close cannot be cancelled; revert the day close first.",
        ApplicationErrorType.Conflict);

    public static ApplicationError NotFound(Guid id) => new(
        "pos_settlements.not_found",
        $"Pos settlement '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError Conflict(string message) => new(
        "pos_settlements.conflict",
        message,
        ApplicationErrorType.Conflict);

    public static ApplicationError Validation(string message) => new(
        "pos_settlements.validation",
        message,
        ApplicationErrorType.Validation);
}
