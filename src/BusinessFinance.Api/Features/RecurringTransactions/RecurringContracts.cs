namespace BusinessFinance.Api.Features.RecurringTransactions;

/// <summary>
/// Backwards compatible: a client that omits <c>SourceType</c> and sends only
/// <c>AccountId</c> keeps working and is treated as an account source.
/// </summary>
public sealed record CreateRecurringTransactionRequest(
    Guid? AccountId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Kind,
    string Frequency,
    string StartDate,
    string? EndDate,
    string MonthEndBehavior,
    string? Description,
    string? SourceType = null,
    Guid? CreditCardId = null);

public sealed record SetRecurringActiveRequest(bool IsActive);

public sealed record GenerateRecurringOccurrencesRequest(string ThroughDate);

/// <summary>
/// Realize the recurring item that falls on <paramref name="ScheduledDate" />,
/// generating its occurrence row first if nobody has generated it yet.
/// </summary>
public sealed record RealizeDueRecurringRequest(string ScheduledDate);

public sealed record RecurringTransactionResponse(
    Guid Id,
    string SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Kind,
    string Frequency,
    string StartDate,
    string? EndDate,
    string? NextOccurrenceDate,
    string MonthEndBehavior,
    string? Description,
    bool IsActive);

public sealed record RecurringTransactionListResponse(
    IReadOnlyList<RecurringTransactionResponse> Items);

public sealed record RecurringOccurrenceResponse(
    Guid Id,
    Guid RecurringTransactionId,
    string OccurrenceKey,
    string SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Kind,
    string ScheduledDate,
    string? Description,
    string Status,
    Guid? BudgetTransactionId,
    Guid? CreditCardChargeId,
    DateTimeOffset? RealizedAtUtc);

/// <summary>
/// A realized occurrence yields exactly one result. <c>sourceType</c> tells the client
/// which side is populated: an account occurrence fills <c>transaction</c>, a
/// credit-card occurrence fills <c>charge</c>.
/// </summary>
public sealed record RealizeRecurringOccurrenceResponse(
    string SourceType,
    Transactions.TransactionResponse? Transaction,
    CreditCards.CardChargeResponse? Charge);

public sealed record RecurringOccurrenceListResponse(
    IReadOnlyList<RecurringOccurrenceResponse> Items);

public sealed record GenerateRecurringOccurrencesResponse(
    IReadOnlyList<RecurringOccurrenceResponse> GeneratedOccurrences,
    bool HasMoreDue);
