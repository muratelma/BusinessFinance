using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.UpcomingPayments;

public sealed class GetUpcomingPaymentsUseCase(
    ICurrentUser currentUser,
    IUpcomingPaymentRepository repository)
{
    public const int MinimumDaysAhead = 1;
    public const int MaximumDaysAhead = 90;

    public async Task<ApplicationResult<IReadOnlyList<UpcomingPaymentDto>>> ExecuteAsync(
        GetUpcomingPaymentsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<UpcomingPaymentDto>>.Failure(
                UpcomingPaymentErrors.AuthenticationRequired);
        }

        if (query.AsOfDate == default ||
            query.AsOfDate.Year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear)
        {
            return ApplicationResult<IReadOnlyList<UpcomingPaymentDto>>.Failure(
                UpcomingPaymentErrors.Validation("As-of date is outside the supported range."));
        }

        if (query.DaysAhead is < MinimumDaysAhead or > MaximumDaysAhead)
        {
            return ApplicationResult<IReadOnlyList<UpcomingPaymentDto>>.Failure(
                UpcomingPaymentErrors.Validation(
                    $"Days ahead must be between {MinimumDaysAhead} and {MaximumDaysAhead}."));
        }

        var horizonDate = query.AsOfDate.AddDays(query.DaysAhead);
        var candidates = await repository.ListCandidatesAsync(
            userId, query.AsOfDate, horizonDate, cancellationToken);
        return ApplicationResult<IReadOnlyList<UpcomingPaymentDto>>.Success(candidates
            .Select(candidate => new UpcomingPaymentDto(
                candidate.SourceId,
                candidate.SourceType,
                candidate.Title,
                candidate.Amount,
                candidate.Currency,
                candidate.DueDate,
                Classify(candidate.DueDate, query.AsOfDate),
                candidate.Description))
            .OrderBy(item => item.DueDate)
            .ThenBy(item => item.SourceType)
            .ThenBy(item => item.SourceId)
            .ToArray());
    }

    internal static UpcomingPaymentTiming Classify(DateOnly dueDate, DateOnly asOfDate)
    {
        if (dueDate < asOfDate) return UpcomingPaymentTiming.Overdue;
        return dueDate == asOfDate
            ? UpcomingPaymentTiming.Today
            : UpcomingPaymentTiming.Upcoming;
    }
}

public static class UpcomingPaymentErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);
    public static ApplicationError Validation(string message) => new(
        "upcoming_payments.validation",
        message,
        ApplicationErrorType.Validation);
}
