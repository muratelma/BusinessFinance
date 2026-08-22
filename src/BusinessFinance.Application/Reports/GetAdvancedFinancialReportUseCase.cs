using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.UpcomingPayments;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Reports;

public sealed class GetAdvancedFinancialReportUseCase(
    ICurrentUser currentUser,
    IFinancialReportRepository repository)
{
    public const int MinimumTrendMonths = 2;
    public const int MaximumTrendMonths = 12;

    public async Task<ApplicationResult<AdvancedFinancialReportDto>> ExecuteAsync(
        GetAdvancedFinancialReportQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<AdvancedFinancialReportDto>.Failure(new ApplicationError(
                "authentication.required",
                "An authenticated user is required.",
                ApplicationErrorType.Unauthorized));
        }

        if (query.Year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear ||
            query.Month is < 1 or > 12)
        {
            return ApplicationResult<AdvancedFinancialReportDto>.Failure(Validation(
                "Year or month is outside the supported range."));
        }
        if (query.AsOfDate == default ||
            query.AsOfDate.Year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear)
        {
            return ApplicationResult<AdvancedFinancialReportDto>.Failure(Validation(
                "As-of date is outside the supported range."));
        }
        if (query.TrendMonths is < MinimumTrendMonths or > MaximumTrendMonths)
        {
            return ApplicationResult<AdvancedFinancialReportDto>.Failure(Validation(
                $"Trend months must be between {MinimumTrendMonths} and {MaximumTrendMonths}."));
        }
        if (query.DaysAhead is < GetUpcomingPaymentsUseCase.MinimumDaysAhead or
            > GetUpcomingPaymentsUseCase.MaximumDaysAhead)
        {
            return ApplicationResult<AdvancedFinancialReportDto>.Failure(Validation(
                $"Days ahead must be between {GetUpcomingPaymentsUseCase.MinimumDaysAhead} and " +
                $"{GetUpcomingPaymentsUseCase.MaximumDaysAhead}."));
        }

        return ApplicationResult<AdvancedFinancialReportDto>.Success(
            await repository.GetAdvancedAsync(
                userId,
                query.Year,
                query.Month,
                query.AsOfDate,
                query.TrendMonths,
                query.DaysAhead,
                query.Scope,
                cancellationToken));
    }

    private static ApplicationError Validation(string message) => new(
        "reports.advanced_validation",
        message,
        ApplicationErrorType.Validation);
}
