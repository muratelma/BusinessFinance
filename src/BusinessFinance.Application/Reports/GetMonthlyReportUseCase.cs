using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Reports;

public sealed class GetMonthlyReportUseCase(
    ICurrentUser currentUser,
    IFinancialReportRepository repository)
{
    public async Task<ApplicationResult<MonthlyReportDto>> ExecuteAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<MonthlyReportDto>.Failure(new ApplicationError(
                "authentication.required",
                "An authenticated user is required.",
                ApplicationErrorType.Unauthorized));
        }
        if (year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear ||
            month is < 1 or > 12)
        {
            return ApplicationResult<MonthlyReportDto>.Failure(new ApplicationError(
                "reports.invalid_period",
                "Year or month is outside the supported range.",
                ApplicationErrorType.Validation));
        }

        return ApplicationResult<MonthlyReportDto>.Success(
            await repository.GetMonthlyAsync(userId, year, month, cancellationToken));
    }
}
