using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Reports;

namespace BusinessFinance.Api.Features.Reports;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/reports/monthly", GetMonthlyAsync)
            .WithTags("Reports")
            .WithName("GetMonthlyReport")
            .RequireAuthorization()
            .Produces<MonthlyReportResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        endpoints.MapGet("/api/v1/dashboard", GetMonthlyAsync)
            .WithTags("Dashboard")
            .WithName("GetDashboard")
            .RequireAuthorization()
            .Produces<MonthlyReportResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        endpoints.MapGet("/api/v1/reports/advanced", GetAdvancedAsync)
            .WithTags("Reports")
            .WithName("GetAdvancedFinancialReport")
            .RequireAuthorization()
            .Produces<AdvancedFinancialReportResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        return endpoints;
    }

    private static async Task<IResult> GetAdvancedAsync(
        int year,
        int month,
        string asOfDate,
        GetAdvancedFinancialReportUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        int trendMonths = 6,
        int daysAhead = 30,
        string? scope = null)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var parsedAsOfDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "As-of date must use the yyyy-MM-dd format.",
                "reports.invalid_as_of_date");
        }
        if (!FinanceContract.TryParseOptionalScope(scope, out var parsedScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "reports.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(new GetAdvancedFinancialReportQuery(
            year,
            month,
            parsedAsOfDate,
            trendMonths,
            daysAhead,
            parsedScope), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToAdvancedResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetMonthlyAsync(
        int year,
        int month,
        GetMonthlyReportUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? scope = null)
    {
        if (!FinanceContract.TryParseOptionalScope(scope, out var parsedScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "reports.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(year, month, parsedScope, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }
        var report = result.Value;
        return Results.Ok(new MonthlyReportResponse(
            report.Year,
            report.Month,
            FinanceContract.OptionalScopeValue(report.Scope),
            FinanceContract.Money(report.TotalIncome),
            FinanceContract.Money(report.TotalExpense),
            FinanceContract.Money(report.Net),
            report.Currency.ToString(),
            report.CategoryExpenses.Select(item => new CategoryExpenseResponse(
                item.CategoryId,
                item.CategoryName,
                FinanceContract.Money(item.Amount))).ToArray(),
            report.CategoryExpenseSlices.Select(item => new CategoryExpenseSliceResponse(
                item.CategoryId,
                item.CategoryName,
                FinanceContract.Money(item.Amount))).ToArray(),
            report.AccountBalances.Select(item => new AccountBalanceResponse(
                item.AccountId,
                item.AccountName,
                FinanceContract.Money(item.Balance),
                FinanceContract.AccountTypeValue(item.Type))).ToArray(),
            report.ScopeBreakdown is { } breakdown
                ? new MonthlyScopeBreakdownResponse(
                    ToScopeTotals(breakdown.Business),
                    ToScopeTotals(breakdown.Personal))
                : null));
    }

    private static ScopeTotalsResponse ToScopeTotals(ScopeTotalsDto totals) => new(
        FinanceContract.Money(totals.Income),
        FinanceContract.Money(totals.Expense),
        FinanceContract.Money(totals.Net));

    internal static AdvancedFinancialReportResponse ToAdvancedResponse(
        AdvancedFinancialReportDto report) => new(
        FinanceContract.Date(report.AsOfDate),
        report.Currency.ToString(),
        FinanceContract.OptionalScopeValue(report.Scope),
        new NetWorthResponse(
            FinanceContract.Money(report.NetWorth.LiquidAssets),
            FinanceContract.Money(report.NetWorth.CreditCardDebt),
            FinanceContract.Money(report.NetWorth.ReceivableDebt),
            FinanceContract.Money(report.NetWorth.PayableDebt),
            FinanceContract.Money(report.NetWorth.NetWorth),
            FinanceContract.Money(report.NetWorth.MoneyInTransit)),
        new PeriodComparisonResponse(
            ToPeriodResponse(report.PeriodComparison.Current),
            ToPeriodResponse(report.PeriodComparison.Previous),
            FinanceContract.Money(report.PeriodComparison.IncomeChange),
            FinanceContract.Money(report.PeriodComparison.ExpenseChange),
            FinanceContract.Money(report.PeriodComparison.NetChange)),
        report.CashFlowTrend.Select(item => new CashFlowPointResponse(
            item.Year,
            item.Month,
            FinanceContract.Money(item.Income),
            FinanceContract.Money(item.Expense),
            FinanceContract.Money(item.Net))).ToArray(),
        report.BudgetVariances.Select(item => new BudgetVarianceResponse(
            item.CategoryId,
            item.CategoryName,
            FinanceContract.Money(item.Limit),
            FinanceContract.Money(item.Spent),
            FinanceContract.Money(item.Remaining),
            item.IsExceeded)).ToArray(),
        new FutureLoadResponse(
            FinanceContract.Date(report.FutureLoad.FromDate),
            FinanceContract.Date(report.FutureLoad.ThroughDate),
            FinanceContract.Money(report.FutureLoad.RecurringAmount),
            FinanceContract.Money(report.FutureLoad.CreditCardStatementAmount),
            FinanceContract.Money(report.FutureLoad.InstallmentAmount),
            FinanceContract.Money(report.FutureLoad.DebtInstallmentAmount),
            FinanceContract.Money(report.FutureLoad.TotalAmount)),
        report.AccountDistribution.Select(item => new AccountBalanceResponse(
            item.AccountId,
            item.AccountName,
            FinanceContract.Money(item.Balance),
            FinanceContract.AccountTypeValue(item.Type))).ToArray(),
        report.CardDistribution.Select(item => new CardDebtResponse(
            item.CreditCardId,
            item.CreditCardName,
            FinanceContract.Money(item.Debt),
            FinanceContract.Money(item.AvailableLimit))).ToArray());

    private static PeriodTotalsResponse ToPeriodResponse(PeriodTotalsDto period) => new(
        period.Year,
        period.Month,
        FinanceContract.Money(period.Income),
        FinanceContract.Money(period.Expense),
        FinanceContract.Money(period.Net));
}
