using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Accounts;

public static class AccountErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static ApplicationError Validation(string message)
    {
        return new ApplicationError(
            "accounts.validation",
            message,
            ApplicationErrorType.Validation);
    }

    public static ApplicationError NotFound(Guid accountId)
    {
        return new ApplicationError(
            "accounts.not_found",
            $"Account '{accountId}' was not found.",
            ApplicationErrorType.NotFound);
    }

    public static readonly ApplicationError Forbidden = new(
        "accounts.forbidden",
        "The account does not belong to the current user.",
        ApplicationErrorType.Forbidden);

    public static ApplicationError DuplicateName(string name)
    {
        return new ApplicationError(
            "accounts.duplicate_name",
            $"An account named '{name}' already exists.",
            ApplicationErrorType.Conflict);
    }

    public static readonly ApplicationError InUse = new(
        "accounts.in_use",
        "This account is used by financial history and cannot be deleted. Deactivate it instead.",
        ApplicationErrorType.Conflict);
}
