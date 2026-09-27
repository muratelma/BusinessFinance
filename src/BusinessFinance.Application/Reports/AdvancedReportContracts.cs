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

/// <summary>
/// Net varlığın kırılımı.
/// </summary>
/// <remarks>
/// <see cref="LiquidAssets"/> ile <see cref="NetWorth"/> aynı soruya cevap
/// vermez (ADR 0015): ilki "bugün ne harcayabilirim", ikincisi "neyim var".
/// Aradaki fark tam olarak <see cref="MoneyInTransit"/> kadardır — POS'tan
/// geçmiş ama henüz hesaba ulaşmamış para kullanıcının parasıdır, ama bugün
/// harcanamaz.
/// <para>
/// <see cref="TotalAssets"/> ve <see cref="TotalLiabilities"/> net varlığın iki
/// tarafıdır ve sunucuda toplanır; istemci kalemleri kendisi toplamaz. Fazla
/// ödenmiş kartın negatif borcu (kart alacağı) varlık tarafına geçer.
/// </para>
/// </remarks>
public sealed record NetWorthDto(
    decimal LiquidAssets,
    decimal CreditCardDebt,
    decimal ReceivableDebt,
    decimal PayableDebt,
    decimal NetWorth,
    decimal MoneyInTransit,
    decimal TotalAssets = 0m,
    decimal TotalLiabilities = 0m,

    /// <summary>
    /// Yoldaki paranın en yakın hesaba geçiş günü; yolda para yoksa boş.
    /// </summary>
    DateOnly? NextTransitDate = null);

public sealed record AdvancedFinancialReportDto(
    DateOnly AsOfDate,
    CurrencyCode Currency,
    TransactionScope? Scope,
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
    int DaysAhead,

    // Boşsa toplam. Kapsam yalnız dönem karşılaştırmasını, nakit akışı
    // eğilimini ve bütçe sapmasını daraltır; net varlık ile hesap/kart
    // dağılımı toplamı göstermeye devam eder (ADR 0013).
    TransactionScope? Scope = null);

public partial interface IFinancialReportRepository
{
    Task<AdvancedFinancialReportDto> GetAdvancedAsync(
        Guid userId,
        int year,
        int month,
        DateOnly asOfDate,
        int trendMonths,
        int daysAhead,
        TransactionScope? scope,
        CancellationToken cancellationToken);
}
