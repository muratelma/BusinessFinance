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
    // İsteğe bağlı: boş bırakılırsa sunucu kapsamı türetir, türetemezse
    // isteği reddeder ve bir değer uydurmaz.
    string? Scope,
    string Frequency,
    string StartDate,
    string? EndDate,
    string MonthEndBehavior,
    string? Description,
    string? SourceType = null,
    Guid? CreditCardId = null,
    int? OccurrenceLimit = null);

public sealed record SetRecurringActiveRequest(bool IsActive);

public sealed record GenerateRecurringOccurrencesRequest(string ThroughDate);

/// <summary>
/// Realize the recurring item that falls on <paramref name="ScheduledDate" />,
/// generating its occurrence row first if nobody has generated it yet.
/// </summary>
public sealed record RealizeDueRecurringRequest(string ScheduledDate, string? Amount = null);

/// <summary>
/// Gerçekleştirme isteğinin gövdesi; tamamı isteğe bağlıdır.
/// </summary>
/// <param name="Amount">
/// Bu dönemin gerçek tutarı. Boşsa plandaki beklenti yazılır; plan tutarı
/// hiçbir hâlde değişmez.
/// </param>
public sealed record RealizeRecurringOccurrenceRequest(string? Amount = null);

public sealed record RecurringTransactionResponse(
    Guid Id,
    string SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Kind,
    string Scope,
    string Frequency,
    string StartDate,
    string? EndDate,
    int? OccurrenceLimit,
    int GeneratedOccurrenceCount,
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
    string Scope,
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
