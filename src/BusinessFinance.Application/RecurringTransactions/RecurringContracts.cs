using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.RecurringTransactions;

public sealed record RecurringTransactionDto(
    Guid Id,
    RecurringSourceType SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    RecurringTransactionKind Kind,
    TransactionScope Scope,
    RecurrenceFrequency Frequency,
    DateOnly StartDate,
    DateOnly? EndDate,
    DateOnly? NextOccurrenceDate,
    MonthEndBehavior MonthEndBehavior,
    string? Description,
    bool IsActive);

public sealed record RecurringOccurrenceDto(
    Guid Id,
    Guid RecurringTransactionId,
    string OccurrenceKey,
    RecurringSourceType SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    RecurringTransactionKind Kind,
    TransactionScope Scope,
    DateOnly ScheduledDate,
    string? Description,
    RecurringOccurrenceStatus Status,
    Guid? BudgetTransactionId,
    Guid? CreditCardChargeId,
    DateTimeOffset? RealizedAtUtc);

/// <summary>
/// Exactly one of <paramref name="AccountId"/> and <paramref name="CreditCardId"/>
/// must be set, matching <paramref name="SourceType"/>.
/// </summary>
public sealed record CreateRecurringTransactionCommand(
    RecurringSourceType SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    RecurringTransactionKind Kind,
    TransactionScope Scope,
    RecurrenceFrequency Frequency,
    DateOnly StartDate,
    DateOnly? EndDate,
    MonthEndBehavior MonthEndBehavior,
    string? Description);

/// <summary>
/// A realized occurrence produces exactly one result, decided by its source: an
/// account occurrence becomes a budget transaction, a credit-card occurrence a card
/// charge. The unused side is always null.
/// </summary>
public sealed record RealizeRecurringOccurrenceResult(
    RecurringSourceType SourceType,
    TransactionDto? Transaction,
    CardChargeDto? Charge);

public sealed record SetRecurringActiveCommand(Guid RecurringTransactionId, bool IsActive);

public sealed record GenerateRecurringOccurrencesCommand(DateOnly ThroughDate);

public sealed record GenerateRecurringOccurrencesResult(
    IReadOnlyList<RecurringOccurrenceDto> GeneratedOccurrences,
    bool HasMoreDue);

public sealed record RealizeRecurringOccurrenceCommand(Guid OccurrenceId);

/// <summary>
/// Realize the recurring item that falls on one date, generating its occurrence
/// row first if nobody has generated it yet.
/// </summary>
/// <remarks>
/// <para>
/// The planned view shows two kinds of recurring row: occurrences that exist,
/// and dates <b>projected</b> from the schedule that nobody has generated yet.
/// Only the first kind could be acted on, and generating was a button on a
/// different screen — so the projected rows carried a permanently disabled
/// "Gerçekleştir" and nothing on that screen said why.
/// </para>
/// <para>
/// Generating is bookkeeping, not a decision: the user's only decision is to
/// realize. This command carries the decision and lets the server do the
/// bookkeeping it needs.
/// </para>
/// </remarks>
public sealed record RealizeDueRecurringCommand(
    Guid RecurringTransactionId,
    DateOnly ScheduledDate);

public sealed record DeleteRecurringTransactionCommand(Guid RecurringTransactionId);

/// <summary>
/// Outcome of removing a plan that never produced money.
/// </summary>
public enum RecurringDeletionResult
{
    Deleted = 1,
    NotFound = 2,

    /// <summary>At least one occurrence was realized, so history depends on it.</summary>
    HasRealizedHistory = 3
}

public interface IRecurringTransactionRepository
{
    Task<RecurringTransaction?> FindOwnedByIdAsync(
        Guid recurringTransactionId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<RecurringTransaction>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<RecurringTransaction>> ListDueAsync(
        Guid userId,
        DateOnly throughDate,
        CancellationToken cancellationToken);
    Task AddAsync(RecurringTransaction recurring, CancellationToken cancellationToken);
    Task UpdateAsync(RecurringTransaction recurring, CancellationToken cancellationToken);
    Task<bool> TrySaveGeneratedAsync(
        IReadOnlyCollection<RecurringTransactionOccurrence> occurrences,
        CancellationToken cancellationToken);
    /// <summary>
    /// Removes a plan that never produced a movement, together with the pending
    /// occurrences it generated, which are only forecasts. A plan with realized
    /// history is refused rather than silently orphaning it.
    /// </summary>
    Task<RecurringDeletionResult> DeleteOwnedIfUnrealizedAsync(
        Guid recurringTransactionId,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// True when the schedule behind the occurrence is still active. A plan the
    /// user switched off must not produce new money.
    /// </summary>
    Task<bool> IsScheduleActiveAsync(
        Guid recurringTransactionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<RecurringTransactionOccurrence?> FindOccurrenceOwnedByIdAsync(
        Guid occurrenceId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);

    /// <summary>
    /// The occurrence a schedule produced for one date, or null when nobody has
    /// generated it yet.
    /// </summary>
    Task<RecurringTransactionOccurrence?> FindOccurrenceOwnedByDateAsync(
        Guid recurringTransactionId,
        DateOnly scheduledDate,
        Guid userId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<RecurringTransactionOccurrence>> ListOccurrencesAsync(
        Guid userId,
        CancellationToken cancellationToken);
    Task<BudgetTransaction> RealizeAsync(
        RecurringTransactionOccurrence occurrence,
        BudgetTransaction transaction,
        CancellationToken cancellationToken);

    /// <summary>
    /// Persists the occurrence state change and its card charge inside one
    /// transaction, so a failure cannot leave a realized occurrence without a charge
    /// or a charge without its occurrence link.
    /// </summary>
    Task<CreditCardCharge> RealizeWithChargeAsync(
        RecurringTransactionOccurrence occurrence,
        CreditCardCharge charge,
        CancellationToken cancellationToken);
}
