using BusinessFinance.Domain;

namespace BusinessFinance.Application.Reports;

public sealed record PeriodTotalsDto(
    int Year,
    int Month,
    decimal Income,
    decimal Expense,
    decimal Net);

public sealed record PeriodComparisonDto(
    PeriodTotalsDto Current,
    PeriodTotalsDto Previous,
    decimal IncomeChange,
    decimal ExpenseChange,
    decimal NetChange);

public sealed record CashFlowPointDto(
    int Year,
    int Month,
    decimal Income,
    decimal Expense,
    decimal Net);

public sealed record BudgetVarianceDto(
    Guid CategoryId,
    string CategoryName,
    decimal Limit,
    decimal Spent,
    decimal Remaining,
    bool IsExceeded);

public sealed record FutureLoadDto(
    DateOnly FromDate,
    DateOnly ThroughDate,
    decimal RecurringAmount,
    decimal CreditCardStatementAmount,
    decimal InstallmentAmount,
    decimal DebtInstallmentAmount,
    decimal TotalAmount);

public sealed record CardDebtDto(
    Guid CreditCardId,
    string CreditCardName,
    decimal Debt,
    decimal AvailableLimit);

public sealed record NetWorthDto(
    decimal LiquidAssets,
    decimal CreditCardDebt,
    decimal ReceivableDebt,
    decimal PayableDebt,
    decimal NetWorth);

public sealed record AdvancedFinancialReportDto(
    DateOnly AsOfDate,
    CurrencyCode Currency,
    NetWorthDto NetWorth,
    PeriodComparisonDto PeriodComparison,
    IReadOnlyList<CashFlowPointDto> CashFlowTrend,
    IReadOnlyList<BudgetVarianceDto> BudgetVariances,
    FutureLoadDto FutureLoad,
    IReadOnlyList<AccountBalanceDto> AccountDistribution,
    IReadOnlyList<CardDebtDto> CardDistribution);

public sealed record GetAdvancedFinancialReportQuery(
    int Year,
    int Month,
    DateOnly AsOfDate,
    int TrendMonths,
    int DaysAhead);

public partial interface IFinancialReportRepository
{
    Task<AdvancedFinancialReportDto> GetAdvancedAsync(
        Guid userId,
        int year,
        int month,
        DateOnly asOfDate,
        int trendMonths,
        int daysAhead,
        CancellationToken cancellationToken);
}
