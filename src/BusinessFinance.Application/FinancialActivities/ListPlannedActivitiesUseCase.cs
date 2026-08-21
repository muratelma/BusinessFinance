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

        if (!AllowedDaysAhead.Contains(query.DaysAhead))
        {
            return ApplicationResult<PlannedActivityListResult>.Failure(
                PlannedActivityErrors.InvalidDaysAhead);
        }

        var horizonDate = query.AsOfDate.AddDays(query.DaysAhead);
        var items = await repository.ListAsync(
            userId, query.AsOfDate, horizonDate, cancellationToken);

        // Overdue first, then by due date. Nothing is summed: adding planned income,
        // expenses, statements and neutral obligations into one number would be
        // misleading, so the client gets a count and the nearest date instead.
        var ordered = items
            .OrderByDescending(item => item.Timing == PlannedActivityTiming.Overdue)
            .ThenBy(item => item.DueDate)
            .ThenBy(item => item.PlannedKind)
            .ThenBy(item => item.PlannedActivityId)
            .ToArray();

        return ApplicationResult<PlannedActivityListResult>.Success(
            new PlannedActivityListResult(
                query.AsOfDate,
                query.DaysAhead,
                ordered.Length,
                ordered.Length == 0 ? null : ordered.Min(item => item.DueDate),
                ordered));
    }
}

public static class PlannedActivityErrors
{
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
