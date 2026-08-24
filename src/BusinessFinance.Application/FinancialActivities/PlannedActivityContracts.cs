using BusinessFinance.Domain;

namespace BusinessFinance.Application.FinancialActivities;

public enum PlannedActivityKind
{
    RecurringOccurrence = 1,
    CardInstallment = 2,
    CardStatement = 3,
    DebtInstallment = 4,
    ReceivableInstallment = 5,
    PayableObligation = 6,
    ReceivableObligation = 7
}

public enum PlannedActivityTiming
{
    Overdue = 1,
    Today = 2,
    Upcoming = 3
}

/// <summary>
/// Derived from the current state of the source, never stored. A plan blocked only by
/// a full card becomes ready again the moment the limit recovers, with no stale
/// failure state to clear.
/// </summary>
public enum PlannedActivityReadiness
{
    Ready = 1,
    NeedsAttention = 2
}

/// <summary>
/// A stable machine code the client turns into its own message. It never carries
/// exception text or anything sensitive.
/// </summary>
public enum PlannedActivityAttention
{
    CardInactive = 1,
    CardLimitInsufficient = 2,
    AccountInactive = 3,
    CategoryInactive = 4
}

public enum PlannedActivityAction
{
    Realize = 1,
    PayCard = 2,
    PayDebt = 3,
    CollectDebt = 4,
    PayObligation = 5,
    CollectObligation = 6
}

/// <summary>
/// One obligation or expected movement that has not happened yet.
/// </summary>
/// <remarks>
/// <see cref="PlannedActivityId"/> identifies the planned record (an occurrence, an
/// installment item, a debt installment, or the card itself for a statement, which has
/// no row of its own). <see cref="SourceId"/> is the account or card the money moves
/// through, matching the realized feed so both screens can share one card widget.
/// </remarks>
public sealed record PlannedActivityDto(
    Guid PlannedActivityId,
    PlannedActivityKind PlannedKind,
    FinancialActivityEffect Effect,
    PlannedActivityTiming Timing,
    PlannedActivityReadiness Readiness,
    PlannedActivityAttention? AttentionCode,
    PlannedActivityAction ActionKind,
    DateOnly DueDate,
    decimal Amount,
    CurrencyCode Currency,
    string Title,
    string? Description,
    Guid? SourceId,
    string? SourceName,
    Guid? CategoryId,
    string? CategoryName,
    bool IsProjected,

    /// <summary>
    /// <see cref="ActionKind"/>'ın çağıracağı uç noktanın adreslediği kayıt.
    /// </summary>
    /// <remarks>
    /// <see cref="PlannedActivityId"/> planlanan **satırı** tanımlar; yazma uç
    /// noktaları ise sahibi olan aggregate'i adresler ve ikisi çoğu türde
    /// farklıdır. Bu alan olmadan istemci `ActionKind`'ı biliyor ama onu
    /// hangi kayıt üzerinde çağıracağını bilmiyordu; eylem sunulamıyordu.
    ///
    /// <list type="bullet">
    /// <item>Tekrarlanan: occurrence kimliği; henüz üretilmemişse
    /// <c>null</c> (gerçekleştirilecek satır yok).</item>
    /// <item>Kart taksidi: taksit planı kimliği + <see cref="ActionSequence"/>.</item>
    /// <item>Kart ekstresi: kart kimliği.</item>
    /// <item>Borç/alacak taksidi: borç kimliği + <see cref="ActionSequence"/>.</item>
    /// </list>
    /// </remarks>
    Guid? ActionTargetId,

    /// <summary>
    /// Aggregate içindeki sıra numarası; uç nokta istemiyorsa <c>null</c>.
    /// </summary>
    int? ActionSequence);

public sealed record PlannedActivityQuery(
    DateOnly AsOfDate,
    int DaysAhead,
    TransactionScope? Scope = null);

public sealed record PlannedActivityListResult(
    DateOnly AsOfDate,
    int DaysAhead,
    TransactionScope? Scope,
    int TotalCount,
    DateOnly? NearestDueDate,
    IReadOnlyList<PlannedActivityDto> Items);

/// <summary>
/// The single source of planned movements. The upcoming-payments view reads a narrowed
/// slice of this same projection rather than keeping a second query that could drift.
/// </summary>
public interface IPlannedActivityRepository
{
    /// <summary>
    /// <paramref name="scope"/> boşsa toplam; doluysa kapsamsız satırlar
    /// (kart ekstresi) da düşer.
    /// </summary>
    Task<IReadOnlyList<PlannedActivityDto>> ListAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        TransactionScope? scope,
        CancellationToken cancellationToken);
}

public static class PlannedActivityRules
{
    /// <summary>
    /// The payment-burden slice: what the user owes. Recurring income and money owed to
    /// the user are planned movements but not obligations, so they stay out of the
    /// upcoming-payments view while still coming from the same projection.
    /// </summary>
    public static bool IsPaymentObligation(PlannedActivityDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return item.ActionKind != PlannedActivityAction.CollectDebt &&
               item.ActionKind != PlannedActivityAction.CollectObligation &&
               item.Effect != FinancialActivityEffect.Income;
    }

    public static PlannedActivityTiming Classify(DateOnly dueDate, DateOnly asOfDate)
    {
        if (dueDate < asOfDate) return PlannedActivityTiming.Overdue;
        return dueDate == asOfDate
            ? PlannedActivityTiming.Today
            : PlannedActivityTiming.Upcoming;
    }
}
