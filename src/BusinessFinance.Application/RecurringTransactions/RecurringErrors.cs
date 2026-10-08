using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.RecurringTransactions;

public static class RecurringErrors
{
    /// <summary>
    /// Kapsam ne istekten, ne kaynaktan (hesap/kart), ne kategoriden
    /// çözülebildi.
    /// </summary>
    public static readonly ApplicationError ScopeUnresolved = new(
        "recurring.scope_unresolved",
        "The scope could not be resolved from the request, the source or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "recurring.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
        ApplicationErrorType.Validation);

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
    /// <summary>
    /// Vergi planının kategorisi vergi işaretli bir gider kategorisi olmalı:
    /// vergi ekranı ödenenleri işaretli kategorilerden okur (ADR 0018 T6).
    /// </summary>
    public static readonly ApplicationError CategoryNotTax = new(
        "recurring.category_not_tax",
        "A tax plan requires an active expense category marked as tax.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Tutarı bilinmeyen kalem tutarsız gerçekleşemez; ödeme tutarı
    /// kullanıcıdan gelir (ADR 0018 İ1, İ5).
    /// </summary>
    public static readonly ApplicationError AmountRequired = new(
        "recurring.amount_required",
        "This item has no amount yet, so the paid amount is required.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Ödenmemiş vergi hiçbir toplamı etkilemez (İ3); ileri tarihli bir ödeme
    /// henüz olmamış bir nakit çıkışını geçmişe yazardı.
    /// </summary>
    public static readonly ApplicationError PaidOnInFuture = new(
        "recurring.paid_on_in_future",
        "The payment date cannot be in the future.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Kalem toplu bir vergi ödemesiyle kapatıldı; geri alma o ödemeden yapılır.
    /// </summary>
    public static readonly ApplicationError ClosedByPayment = new(
        "recurring.closed_by_payment",
        "The item was closed by a tax payment; undo that payment instead.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Kalem zaten ödendi ya da kapatıldı; ikinci bir sonuç "tek sonuç"
    /// kuralını bozardı.
    /// </summary>
    public static readonly ApplicationError AlreadySettled = new(
        "recurring.already_settled",
        "The item was already paid or closed.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Yeni ritim son ödenen ya da kapatılan kalemden sonra başlamalı.
    /// </summary>
    public static readonly ApplicationError RescheduleBeforeHistory = new(
        "recurring.reschedule_before_history",
        "The new rhythm must start after the last paid or closed item.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Kaynaksız vergi kaleminin ödemesi hesap ya da kart ister (ADR 0018 T4).
    /// </summary>
    public static readonly ApplicationError SourceRequired = new(
        "recurring.source_required",
        "Choose the account or credit card the payment was made from.",
        ApplicationErrorType.Validation);

    /// <summary>Başka bir istek aynı kalemi aynı anda değiştirdi.</summary>
    public static readonly ApplicationError ConcurrentChange = new(
        "recurring.concurrent_change",
        "The item was changed by another request. Refresh and try again.",
        ApplicationErrorType.Conflict);

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
