using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Budgets;

public static class BudgetErrors
{
    /// <summary>
    /// Kapsam ne istekten ne kategoriden çözülebildi. Bütçenin bir hesabı
    /// yoktur; zincir açık seçim ve kategori ile sınırlıdır.
    /// </summary>
    public static readonly ApplicationError ScopeUnresolved = new(
        "budgets.scope_unresolved",
        "The scope could not be resolved from the request or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "budgets.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
        ApplicationErrorType.Validation);

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
