using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Imports;

internal static class ImportErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "imports.authentication_required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static ApplicationError Validation(string message) => new(
        "imports.invalid_csv",
        message,
        ApplicationErrorType.Validation);

    public static ApplicationError NotFound(Guid id) => new(
        "imports.not_found",
        $"Import batch '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError MappingUnavailable = new(
        "imports.mapping_unavailable",
        "The selected account or category is unavailable.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Satırın kapsamı ne hesaptan ne kategoriden çözülebildi. CSV dosyası
    /// kapsam kolonu taşımıyor; sunucu bir değer uydurmaz, isteği reddeder.
    /// </summary>
    public static readonly ApplicationError ScopeUnresolved = new(
        "imports.scope_unresolved",
        "The scope could not be resolved from the account or the category.",
        ApplicationErrorType.Validation);

    public static ApplicationError Conflict(string message) => new(
        "imports.confirmation_conflict",
        message,
        ApplicationErrorType.Conflict);
}
