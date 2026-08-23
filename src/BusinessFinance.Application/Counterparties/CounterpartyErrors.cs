using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Counterparties;

public static class CounterpartyErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static ApplicationError Validation(string message) => new(
        "counterparties.validation",
        message,
        ApplicationErrorType.Validation);

    public static ApplicationError NotFound(Guid id) => new(
        "counterparties.not_found",
        $"Counterparty '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError DuplicateName = new(
        "counterparties.duplicate_name",
        "A counterparty with the same name already exists.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Hareketi olan karşı taraf silinmez; boş hesap kuralının aynısı.
    /// </summary>
    public static readonly ApplicationError HasHistory = new(
        "counterparties.has_history",
        "A counterparty with movements cannot be deleted; deactivate it instead.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Pasif karşı tarafa yeni borçlandırma yazılamaz — tahsilat yazılabilir.
    /// </summary>
    public static readonly ApplicationError Inactive = new(
        "counterparties.inactive",
        "An inactive counterparty cannot take on a new charge.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError ScopeUnresolved = new(
        "counterparties.scope_unresolved",
        "The scope could not be resolved from the request or the category.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CategoryUnavailable = new(
        "counterparties.category_unavailable",
        "An active owned category of the matching type is required.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError AccountUnavailable = new(
        "counterparties.account_unavailable",
        "An active owned account is required.",
        ApplicationErrorType.Validation);

    public static ApplicationError ChargeNotFound(Guid id) => new(
        "counterparty_charges.not_found",
        $"Counterparty charge '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError PaymentNotFound(Guid id) => new(
        "counterparty_payments.not_found",
        $"Counterparty payment '{id}' was not found.",
        ApplicationErrorType.NotFound);
}
