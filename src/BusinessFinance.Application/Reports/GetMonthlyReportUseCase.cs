using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Reports;

public sealed class GetMonthlyReportUseCase(
    ICurrentUser currentUser,
    IFinancialReportRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<MonthlyReportDto>> ExecuteAsync(
        int year,
        int month,
        TransactionScope? scope = null,
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

        var report = await repository.GetMonthlyAsync(userId, year, month, scope, cancellationToken);
        var comparison = await CompareExpenseAsync(
            userId, year, month, scope, report.TotalExpense, cancellationToken);
        return ApplicationResult<MonthlyReportDto>.Success(
            report with { ExpenseComparison = comparison });
    }

    private async Task<ExpenseComparisonDto?> CompareExpenseAsync(
        Guid userId,
        int year,
        int month,
        TransactionScope? scope,
        decimal currentExpense,
        CancellationToken cancellationToken)
    {
        var today = LocalDay.Today(timeProvider.GetUtcNow());
        var monthStart = new DateOnly(year, month, 1);
        var thisMonth = new DateOnly(today.Year, today.Month, 1);
        if (monthStart > thisMonth || year == MonthlyBudget.MinimumYear && month == 1)
        {
            return null;
        }

        var previousStart = monthStart.AddMonths(-1);
        var throughToday = monthStart == thisMonth;
        // İçinde bulunulan ay: önceki ayın aynı gününe kadar (o gün dahil).
        // Önceki ay daha kısaysa son günü sınırdır (31 Mart → 28 Şubat).
        var previousEndExclusive = throughToday
            ? previousStart.AddDays(Math.Min(today.Day, DateTime.DaysInMonth(previousStart.Year, previousStart.Month)))
            : monthStart;
        var previousExpense = await repository.GetExpenseAsync(
            userId, previousStart, previousEndExclusive, scope, cancellationToken);
        return previousExpense == 0m
            ? null
            : new ExpenseComparisonDto(previousExpense, currentExpense - previousExpense, throughToday);
    }
}
