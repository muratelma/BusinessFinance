using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.RecurringTransactions;

/// <param name="Amount">Beklenen tutar; yalnız vergi planında boş olabilir.</param>
/// <param name="SourceType">Yalnız vergi planında boş olabilir (ADR 0018 T4).</param>
/// <param name="TaxKind">Doluysa plan bir vergidir (ADR 0018 T2).</param>
public sealed record RecurringTransactionDto(
    Guid Id,
    RecurringSourceType? SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    decimal? Amount,
    CurrencyCode Currency,
    RecurringTransactionKind Kind,
    TransactionScope Scope,
    RecurrenceFrequency Frequency,
    DateOnly StartDate,
    DateOnly? EndDate,
    int? OccurrenceLimit,
    int GeneratedOccurrenceCount,
    DateOnly? NextOccurrenceDate,
    MonthEndBehavior MonthEndBehavior,
    string? Description,
    bool IsActive,
    TaxKind? TaxKind = null,
    int? DayOfMonth = null,
    int? SelectedMonths = null);

/// <param name="Amount">Bu dönemin tutarı; bilinmiyorsa boştur.</param>
/// <param name="ClosedByTransactionId">
/// Kalemi kapatan toplu vergi ödemesi (hesaptan); yalnız kapatılmış kalemde.
/// </param>
/// <param name="ClosedByChargeId">Kalemi kapatan toplu vergi ödemesi (kartla).</param>
public sealed record RecurringOccurrenceDto(
    Guid Id,
    Guid RecurringTransactionId,
    string OccurrenceKey,
    RecurringSourceType? SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    decimal? Amount,
    CurrencyCode Currency,
    RecurringTransactionKind Kind,
    TransactionScope Scope,
    DateOnly ScheduledDate,
    string? Description,
    RecurringOccurrenceStatus Status,
    Guid? BudgetTransactionId,
    Guid? CreditCardChargeId,
    DateTimeOffset? RealizedAtUtc,
    Guid? ClosedByTransactionId = null,
    Guid? ClosedByChargeId = null,
    DateTimeOffset? ClosedAtUtc = null);

/// <summary>
/// Exactly one of <paramref name="AccountId"/> and <paramref name="CreditCardId"/>
/// must be set, matching <paramref name="SourceType"/>. Only a tax plan may leave
/// all three empty; its source is then chosen when it is paid.
/// </summary>
public sealed record CreateRecurringTransactionCommand(
    RecurringSourceType? SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    decimal? Amount,
    CurrencyCode Currency,
    RecurringTransactionKind Kind,

    // Kullanıcının açık seçimi. Boşsa kaynağın (hesap ya da kart), yoksa
    // kategorinin varsayılanı kullanılır; üçü de boşsa istek reddedilir.
    // Vergi planında zincir yoktur: seçim yoksa profilin tarafıdır; kaynağın
    // etiketine bakılmaz (ADR 0018 İ9). Planın ürettiği her kayıt bu kapsamı alır.
    TransactionScope? Scope,
    RecurrenceFrequency Frequency,
    DateOnly StartDate,
    DateOnly? EndDate,
    MonthEndBehavior MonthEndBehavior,
    string? Description,
    int? OccurrenceLimit = null,
    TaxKind? TaxKind = null,
    int? DayOfMonth = null,
    int? SelectedMonths = null);

/// <summary>
/// Bir planın tam güncel hâli. Ritim alanlarından biri değişirse plan yeni
/// ritimle <see cref="StartDate"/> gününden yeniden başlar: bekleyen kalemler
/// yeniden kurulur, ödenmiş ve kapatılmış kalemler geçmiş olarak kalır.
/// </summary>
public sealed record UpdateRecurringTransactionCommand(
    Guid RecurringTransactionId,
    RecurringSourceType? SourceType,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    decimal? Amount,
    TransactionScope? Scope,
    string? Description,
    RecurrenceFrequency Frequency,
    DateOnly StartDate,
    DateOnly? EndDate,
    MonthEndBehavior MonthEndBehavior,
    int? DayOfMonth,
    int? SelectedMonths);

/// <summary>
/// Bekleyen bir kalemin bu dönemki tutarı ("tutar belli oldu", ADR 0018 T3).
/// Plan değişmez.
/// </summary>
public sealed record SetOccurrenceAmountCommand(
    Guid RecurringTransactionId,
    DateOnly ScheduledDate,
    decimal Amount);

/// <summary>
/// Gerçekleşmiş bir kalemin ödemesini geri alır: ürettiği kayıt iptal
/// edilir, kalem bekleyene döner (ADR 0018 İ7).
/// </summary>
public sealed record UndoRecurringOccurrenceCommand(Guid OccurrenceId);

/// <summary>
/// Ödemenin yapıldığı hesap ya da kart. İkisi birden verilemez; ikisi de
/// boşsa kalemin kendi kaynağı kullanılır.
/// </summary>
public sealed record RecurringPaymentDetails(
    DateOnly? PaidOn = null,
    Guid? AccountId = null,
    Guid? CreditCardId = null);

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

/// <summary>
/// Bekleyen bir occurrence'ı gerçekleştirir.
/// </summary>
/// <param name="Amount">
/// Bu dönemin gerçek tutarı; boşsa plandaki tutar yazılır. Planın tutarı bir
/// <b>beklentidir</b> ve bazı kalemlerde her dönem değişir (elektrik faturası,
/// KDV beyanı, geçici vergi); beklentiyi gerçekleşmiş hareket olarak yazmak
/// olmamış bir tutarı finansal geçmişe koymak olurdu. Planın kendi tutarı
/// değişmez — düzeltilen bu dönemdir.
/// </param>
/// <param name="Payment">
/// "Ödedim" ayrıntısı: ödeme günü ve kaynağı (ADR 0018 T4). Boşsa kayıt vade
/// gününe, kalemin kendi kaynağından yazılır (bu alandan önceki davranış).
/// </param>
public sealed record RealizeRecurringOccurrenceCommand(
    Guid OccurrenceId,
    decimal? Amount = null,
    RecurringPaymentDetails? Payment = null);

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
    DateOnly ScheduledDate,
    decimal? Amount = null,
    RecurringPaymentDetails? Payment = null);

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

    /// <summary>
    /// Bir planın bütün kalemleri, izlenerek (düzenleme bekleyenleri günceller
    /// ya da yeniden kurar).
    /// </summary>
    Task<IReadOnlyList<RecurringTransactionOccurrence>> ListPlanOccurrencesAsync(
        Guid recurringTransactionId,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// İzlenen plan ve kalemlerindeki değişiklikleri tek <c>SaveChanges</c>
    /// ile yazar; <paramref name="removedOccurrences"/> (yeniden kurulan
    /// bekleyenler) aynı sınırda silinir.
    /// </summary>
    Task SaveEditAsync(
        RecurringTransaction recurring,
        IReadOnlyCollection<RecurringTransactionOccurrence> removedOccurrences,
        CancellationToken cancellationToken);

    /// <summary>
    /// Bir vergi ödemesinin kapattığı kalemler, izlenerek. Ödeme iptal edilince
    /// aynı sınırda bekleyene dönerler.
    /// </summary>
    Task<IReadOnlyList<RecurringTransactionOccurrence>> ListClosedByAsync(
        Guid userId,
        Guid? transactionId,
        Guid? chargeId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<RecurringTransaction>> ListDueAsync(
        Guid userId,
        DateOnly throughDate,
        CancellationToken cancellationToken);
    Task AddAsync(RecurringTransaction recurring, CancellationToken cancellationToken);

    /// <summary>Planların hepsini tek <c>SaveChanges</c> ile yazar.</summary>
    Task AddRangeAsync(IReadOnlyCollection<RecurringTransaction> plans, CancellationToken cancellationToken);
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
        CancellationToken cancellationToken,
        bool track = false);

    /// <summary>
    /// İzlenen kalem değişikliğini ve iptal edilen sonuç kaydını tek
    /// <c>SaveChanges</c> ile yazar. Kalemin sürüm damgası çakışırsa (başka bir
    /// istek aynı kalemi değiştirdi) <see langword="false"/> döner.
    /// </summary>
    Task<bool> TrySaveOccurrenceChangeAsync(CancellationToken cancellationToken);
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
