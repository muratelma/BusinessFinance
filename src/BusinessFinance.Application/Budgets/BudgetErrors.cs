using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Budgets;

public static class BudgetErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);
    public static ApplicationError Validation(string message) => new(
        "budgets.validation",
        message,
        ApplicationErrorType.Validation);
    public static ApplicationError NotFound(Guid id) => new(
        "budgets.not_found",
        $"Budget '{id}' was not found.",
        ApplicationErrorType.NotFound);
    public static readonly ApplicationError CategoryUnavailable = new(
        "budgets.category_unavailable",
        "An active owned expense category is required.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError DuplicatePeriod = new(
        "budgets.duplicate_period",
        "A budget already exists for this category and month.",
        ApplicationErrorType.Conflict);
}
