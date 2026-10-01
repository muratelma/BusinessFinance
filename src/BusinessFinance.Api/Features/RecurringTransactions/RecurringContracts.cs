namespace BusinessFinance.Api.Features.RecurringTransactions;

/// <summary>
/// Backwards compatible: a client that omits <c>SourceType</c> and sends only
/// <c>AccountId</c> keeps working and is treated as an account source. A tax plan
/// (<c>TaxKind</c> set) may omit every source field; its source is then chosen
/// when it is paid (ADR 0018 T4).
/// </summary>
/// <param name="Amount">
/// Beklenen tutar. Yalnız vergi planında boş olabilir: tutarı ödeme gününe
/// kadar bilinmeyen vergi meşrudur (ADR 0018 İ5).
/// </param>
/// <param name="DayOfMonth">
/// Ayın günü; boşsa başlangıç günü. <c>31</c> ay sonudur (kısa aylarda son gün).
/// </param>
/// <param name="Months">"selected-months" sıklığının ayları (1–12).</param>
public sealed record CreateRecurringTransactionRequest(
    Guid? AccountId,
    Guid CategoryId,
    string? Amount,
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
    int? OccurrenceLimit = null,
    string? TaxKind = null,
    int? DayOfMonth = null,
    IReadOnlyList<int>? Months = null);

/// <summary>
/// Planın tam güncel hâli (ADR 0018 T2: vergi düzenlenir). Tür, vergi türü ve
/// para birimi değişmez. Ritim alanlarından biri değişirse plan yeni ritimle
/// <c>StartDate</c> gününden yeniden başlar; bekleyen kalemler yeniden kurulur,
/// ödenmiş ve kapatılmış kalemler geçmiş olarak kalır.
/// </summary>
public sealed record UpdateRecurringTransactionRequest(
    Guid? AccountId,
    Guid CategoryId,
    string? Amount,
    string? Scope,
    string Frequency,
    string StartDate,
    string? EndDate,
    string MonthEndBehavior,
    string? Description,
    string? SourceType = null,
    Guid? CreditCardId = null,
    int? DayOfMonth = null,
    IReadOnlyList<int>? Months = null);

public sealed record SetRecurringActiveRequest(bool IsActive);

public sealed record GenerateRecurringOccurrencesRequest(string ThroughDate);

/// <summary>
/// Realize the recurring item that falls on <paramref name="ScheduledDate" />,
/// generating its occurrence row first if nobody has generated it yet.
/// </summary>
/// <param name="PaidOn">
/// "Ödedim": ödemenin yapıldığı gün (ADR 0018 T4). Verilirse kayıt bu güne
/// yazılır ve vadesi gelmemiş kalem de ödenebilir; verilmezse vade gününe.
/// </param>
/// <param name="AccountId">Ödemenin yapıldığı hesap; kalemin kaynağını geçersiz kılar.</param>
/// <param name="CreditCardId">Ödemenin yapıldığı kart; kart harcaması yazılır.</param>
public sealed record RealizeDueRecurringRequest(
    string ScheduledDate,
    string? Amount = null,
    string? PaidOn = null,
    Guid? AccountId = null,
    Guid? CreditCardId = null);

/// <summary>
/// Gerçekleştirme isteğinin gövdesi; tamamı isteğe bağlıdır.
/// </summary>
/// <param name="Amount">
/// Bu dönemin gerçek tutarı. Boşsa kalemdeki tutar yazılır; kalemin tutarı yoksa
/// zorunludur. Plan tutarı hiçbir hâlde değişmez.
/// </param>
public sealed record RealizeRecurringOccurrenceRequest(
    string? Amount = null,
    string? PaidOn = null,
    Guid? AccountId = null,
    Guid? CreditCardId = null);

/// <summary>"Tutar belli oldu": bekleyen kalemin bu dönemki tutarı.</summary>
public sealed record SetOccurrenceAmountRequest(string ScheduledDate, string Amount);

public sealed record RecurringTransactionResponse(
    Guid Id,
    string? SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    string? Amount,
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
    bool IsActive,
    string? TaxKind,
    int? DayOfMonth,
    IReadOnlyList<int>? Months);

public sealed record RecurringTransactionListResponse(
    IReadOnlyList<RecurringTransactionResponse> Items);

public sealed record RecurringOccurrenceResponse(
    Guid Id,
    Guid RecurringTransactionId,
    string OccurrenceKey,
    string? SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    string? Amount,
    string Currency,
    string Kind,
    string Scope,
    string ScheduledDate,
    string? Description,
    string Status,
    Guid? BudgetTransactionId,
    Guid? CreditCardChargeId,
    DateTimeOffset? RealizedAtUtc,
    Guid? ClosedByTransactionId,
    Guid? ClosedByChargeId,
    DateTimeOffset? ClosedAtUtc);

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
