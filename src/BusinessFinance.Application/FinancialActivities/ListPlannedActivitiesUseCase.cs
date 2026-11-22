using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.FinancialActivities;

public sealed class ListPlannedActivitiesUseCase(
    ICurrentUser currentUser,
    IPlannedActivityRepository repository)
{
    /// <summary>
    /// Fixed horizons rather than a free number: each one is a deliberate product
    /// choice, and an open range would let a caller ask for an unbounded projection of
    /// future recurring dates.
    /// </summary>
    public static readonly int[] AllowedDaysAhead = [7, 30, 90];

    /// <summary>
    /// Özel aralığın en uzun hâli: bir yıl. Tekrarlayan tarihler ufka kadar
    /// üretildiği için açık uçlu bir aralık sınırsız izdüşüm demektir.
    /// </summary>
    public const int MaxRangeDays = 366;

    public async Task<ApplicationResult<PlannedActivityListResult>> ExecuteAsync(
        PlannedActivityQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PlannedActivityListResult>.Failure(
                PlannedActivityErrors.AuthenticationRequired);
        }

        if (query.AsOfDate == default ||
            query.AsOfDate.Year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear)
        {
            return ApplicationResult<PlannedActivityListResult>.Failure(
                PlannedActivityErrors.InvalidAsOfDate);
        }

        if (query.From is not null || query.To is not null)
        {
            return await ListRangeAsync(userId, query, cancellationToken);
        }

        if (!AllowedDaysAhead.Contains(query.DaysAhead))
        {
            return ApplicationResult<PlannedActivityListResult>.Failure(
                PlannedActivityErrors.InvalidDaysAhead);
        }

        var horizonDate = query.AsOfDate.AddDays(
            query.CountTodayInHorizon ? query.DaysAhead - 1 : query.DaysAhead);
        var items = (await repository.ListAsync(
            userId, query.AsOfDate, horizonDate, query.Scope, cancellationToken, query.IncludeScopedStatements)).ToList();
        if (query.IncludeCounterpartyDues)
        {
            items.AddRange(await repository.ListCounterpartyDuesAsync(
                userId, query.AsOfDate, horizonDate, query.Scope, cancellationToken));
        }

        // Overdue first, then by due date. Planned income, expenses, statements and
        // neutral obligations are never summed into one number; the only total is
        // the outgoing payment obligations that are not yet overdue. An item whose
        // amount is not known yet (a tax) is counted, never estimated.
        var ordered = items
            .OrderByDescending(item => item.Timing == PlannedActivityTiming.Overdue)
            .ThenBy(item => item.DueDate)
            .ThenBy(item => item.PlannedKind)
            .ThenBy(item => item.PlannedActivityId)
            .ToArray();

        var obligations = ordered
            .Where(PlannedActivityRules.IsPaymentObligation)
            .ToArray();
        var outgoing = obligations
            .Where(item => item.Timing != PlannedActivityTiming.Overdue)
            .ToArray();
        var overdue = obligations
            .Where(item => item.Timing == PlannedActivityTiming.Overdue)
            .ToArray();
        return ApplicationResult<PlannedActivityListResult>.Success(
            new PlannedActivityListResult(
                query.AsOfDate,
                query.DaysAhead,
                query.Scope,
                ordered.Length,
                ordered.Length == 0 ? null : ordered.Min(item => item.DueDate),
                ordered,
                outgoing.Sum(item => item.Amount ?? 0m),
                outgoing.Count(item => item.Amount is null),
                overdue.Sum(item => item.Amount ?? 0m),
                overdue.Count(item => item.Amount is null),
                outgoing.Length));
    }

    /// <summary>
    /// Özel aralık: vadesi [From, To] içindeki açık kayıtlar ve seçimden önce
    /// gecikenler. Gecikme bugüne göre belirlenir; ileri bir aralıkta bugün ile
    /// başlangıç arasındaki kayıtlar gecikmiş sayılmaz ve listeye girmez.
    /// </summary>
    private async Task<ApplicationResult<PlannedActivityListResult>> ListRangeAsync(
        Guid userId,
        PlannedActivityQuery query,
        CancellationToken cancellationToken)
    {
        if (query.From is not DateOnly from || query.To is not DateOnly to || from > to ||
            to.DayNumber - from.DayNumber >= MaxRangeDays ||
            to.Year > MonthlyBudget.MaximumYear || from.Year < MonthlyBudget.MinimumYear)
        {
            return ApplicationResult<PlannedActivityListResult>.Failure(
                PlannedActivityErrors.InvalidRange);
        }

        var today = query.AsOfDate;
        var horizon = to > today ? to : today;
        var items = (await repository.ListAsync(
            userId, today, horizon, query.Scope, cancellationToken, query.IncludeScopedStatements)).ToList();
        if (query.IncludeCounterpartyDues)
        {
            items.AddRange(await repository.ListCounterpartyDuesAsync(
                userId, today, horizon, query.Scope, cancellationToken));
        }

        if (query.Type is PlannedTypeFilter type)
        {
            items = [.. items.Where(item => PlannedActivityRules.Matches(item, type))];
        }

        var inPeriod = items
            .Where(item => item.DueDate >= from && item.DueDate <= to)
            .ToArray();
        var earlier = items
            .Where(item => item.DueDate < from && item.Timing == PlannedActivityTiming.Overdue)
            .ToArray();
        var ordered = earlier.Concat(inPeriod)
            .OrderBy(item => item.DueDate)
            .ThenBy(item => item.PlannedKind)
            .ThenBy(item => item.PlannedActivityId)
            .ToArray();

        var payments = inPeriod.Where(PlannedActivityRules.IsPaymentObligation).ToArray();
        var incoming = inPeriod.Where(PlannedActivityRules.IsExpectedIncome).ToArray();
        var cardPostings = inPeriod.Where(PlannedActivityRules.IsCardPosting).ToArray();
        var earlierPayments = earlier.Where(PlannedActivityRules.IsPaymentObligation).ToArray();
        var period = new PlannedPeriodSummary(
            from,
            to,
            payments.Sum(item => item.Amount ?? 0m),
            payments.Length,
            payments.Count(item => item.Amount is null),
            incoming.Sum(item => item.Amount ?? 0m),
            incoming.Length,
            cardPostings.Sum(item => item.Amount ?? 0m),
            cardPostings.Length,
            earlier.Length,
            earlierPayments.Sum(item => item.Amount ?? 0m),
            earlierPayments.Count(item => item.Amount is null));

        return ApplicationResult<PlannedActivityListResult>.Success(
            new PlannedActivityListResult(
                today,
                to.DayNumber - from.DayNumber + 1,
                query.Scope,
                ordered.Length,
                ordered.Length == 0 ? null : ordered.Min(item => item.DueDate),
                ordered,
                Period: period));
    }
}

public static class PlannedActivityErrors
{
    public static readonly ApplicationError InvalidRange = new(
        "planned_activities.invalid_range",
        "From and to must both be given, from must not be after to, and the range must not exceed a year.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError InvalidAsOfDate = new(
        "planned_activities.invalid_as_of_date",
        "As-of date is outside the supported range.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidDaysAhead = new(
        "planned_activities.invalid_days_ahead",
        "Days ahead must be 7, 30 or 90.",
        ApplicationErrorType.Validation);
}
