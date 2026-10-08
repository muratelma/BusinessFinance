using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Cash;

public static class CashCountErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError AccountUnavailable = new(
        "cash_counts.account_unavailable",
        "An active owned cash account is required.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError ScopeUnresolved = new(
        "cash_counts.scope_unresolved",
        "The scope could not be resolved from the request or the account.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CategoryUnavailable = new(
        "cash_counts.category_unavailable",
        "An active owned category matching the difference is required.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError UnknownReasonNotApplicable = new(
        "cash_counts.unknown_reason_not_applicable",
        "An unknown reason applies only to a shortage and cannot come with a category.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError NothingToAdjust = new(
        "cash_counts.nothing_to_adjust",
        "The count matched the expected balance; there is no difference to record.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError RecountRequired = new(
        "cash_counts.recount_required",
        "The cash account changed after this count; count again to record a difference.",
        ApplicationErrorType.Conflict);

    public static ApplicationError NotFound(Guid id) => new(
        "cash_counts.not_found",
        $"Cash count '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError Conflict(string message) => new(
        "cash_counts.conflict",
        message,
        ApplicationErrorType.Conflict);

    public static ApplicationError Validation(string message) => new(
        "cash_counts.validation",
        message,
        ApplicationErrorType.Validation);
}
