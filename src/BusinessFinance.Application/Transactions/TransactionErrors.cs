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
    public static readonly ApplicationError CancelOriginLocked = new(
        "transactions.cancel_origin_locked",
        "A transaction produced by a recurring plan cannot be cancelled.",
        ApplicationErrorType.Conflict);
}
