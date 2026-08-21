using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.RecurringTransactions;

public static class RecurringErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);
    public static ApplicationError Validation(string message) => new(
        "recurring.validation",
        message,
        ApplicationErrorType.Validation);
    public static ApplicationError NotFound(Guid id) => new(
        "recurring.not_found",
        $"Recurring transaction '{id}' was not found.",
        ApplicationErrorType.NotFound);
    public static ApplicationError OccurrenceNotFound(Guid id) => new(
        "recurring.occurrence_not_found",
        $"Recurring occurrence '{id}' was not found.",
        ApplicationErrorType.NotFound);
    public static readonly ApplicationError AccountUnavailable = new(
        "recurring.account_unavailable",
        "The account was not found or is inactive.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError CategoryUnavailable = new(
        "recurring.category_unavailable",
        "The category was not found or is inactive.",
        ApplicationErrorType.Validation);
    /// <summary>
    /// Conflict rather than validation: the request is well formed and becomes
    /// valid again the moment the schedule is reactivated.
    /// </summary>
    public static readonly ApplicationError ScheduleInactive = new(
        "recurring.schedule_inactive",
        "The recurring plan is inactive, so its planned record cannot be realized.",
        ApplicationErrorType.Conflict);
    /// <summary>
    /// The plan already produced real movements, so deleting it would strand
    /// them: their origin badge and cancel lock both read from this link.
    /// Deactivating stops it instead, which is what the message points at.
    /// </summary>
    public static readonly ApplicationError HasRealizedHistory = new(
        "recurring.has_realized_history",
        "The plan already produced movements, so it can only be deactivated.",
        ApplicationErrorType.Conflict);
    /// <summary>
    /// The requested date has not arrived yet.
    /// </summary>
    /// <remarks>
    /// A recurring plan is a forecast until its date comes. Realizing next
    /// month's rent today would put money in a month it did not leave, and every
    /// report that reads by date would be wrong from then on. The screen already
    /// hides the action on future rows; this is the second gate, on the server,
    /// because the client is not where a financial rule is enforced.
    /// </remarks>
    public static readonly ApplicationError NotDueYet = new(
        "recurring.not_due_yet",
        "The planned date has not arrived yet.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError InvalidSource = new(
        "recurring.invalid_source",
        "Exactly one of account or credit card must be supplied as the source.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError IncomeCardSourceNotSupported = new(
        "recurring.income_card_source_not_supported",
        "A recurring income cannot be sourced from a credit card.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError CardUnavailable = new(
        "recurring.card_unavailable",
        "The credit card was not found or is inactive.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Conflict, not validation: the plan is valid and the user may retry once the
    /// available limit recovers. The occurrence stays planned.
    /// </summary>
    public static readonly ApplicationError CardLimitInsufficient = new(
        "recurring.card_limit_insufficient",
        "The available credit card limit is not enough for this occurrence.",
        ApplicationErrorType.Conflict);
}
