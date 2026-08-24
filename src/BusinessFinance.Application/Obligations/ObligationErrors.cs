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

    public static readonly ApplicationError AccountUnavailable = new(
        "obligations.account_unavailable",
        "An active owned account with the matching currency is required.",
        ApplicationErrorType.Validation);

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
