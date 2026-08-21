using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Categories;

public static class CategoryErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static ApplicationError Validation(string message) => new(
        "categories.validation",
        message,
        ApplicationErrorType.Validation);

    public static ApplicationError NotFound(Guid id) => new(
        "categories.not_found",
        $"Category '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError DuplicateName(string name) => new(
        "categories.duplicate_name",
        $"A category named '{name}' with the same type already exists.",
        ApplicationErrorType.Conflict);
}
