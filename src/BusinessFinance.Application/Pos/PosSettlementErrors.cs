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
