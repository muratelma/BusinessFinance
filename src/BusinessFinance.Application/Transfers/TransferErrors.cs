using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Transfers;

public static class TransferErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError AccountUnavailable = new(
        "transfers.account_unavailable",
        "Both accounts must exist, belong to the user and be active.",
        ApplicationErrorType.Validation);

    public static ApplicationError Validation(string message) => new(
        "transfers.validation",
        message,
        ApplicationErrorType.Validation);

    public static ApplicationError NotFound(Guid id) => new(
        "transfers.not_found",
        $"Transfer '{id}' was not found.",
        ApplicationErrorType.NotFound);
}
