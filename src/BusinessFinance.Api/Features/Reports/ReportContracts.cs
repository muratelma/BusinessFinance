namespace BusinessFinance.Api.Features.Reports;

public sealed record CategoryExpenseResponse(Guid CategoryId, string CategoryName, string Amount);

/// <summary>
/// Kategori dağılımının halka grafiğe sığan hâli: en büyük birkaç kategori ve
/// geri kalanının toplamı. Toplama sunucuda yapılır çünkü "Diğer" bir
/// finansal toplamdır.
/// </summary>
/// <param name="CategoryId">
/// Yalnız "Diğer" satırında <c>null</c>: o satır bir kategori değil, birden
/// çoğunun toplamıdır.
/// </param>
public sealed record CategoryExpenseSliceResponse(
    Guid? CategoryId,
    string CategoryName,
    string Amount);
public sealed record AccountBalanceResponse(
    Guid AccountId,
    string AccountName,
    string Balance,
    string Type);
public sealed record ScopeTotalsResponse(string Income, string Expense, string Net);

/// <summary>
/// Ayın iki tarafı ayrı ayrı toplanmış hâli.
/// </summary>
/// <remarks>
/// Yalnız <b>filtresiz</b> okumada döner. Kapsam verilmişse rapor zaten tek
/// tarafı anlatıyor demektir ve dışlanan taraf sıfır görünürdü.
///
/// İki tarafın toplamı raporun kendi toplamına eşittir; istemci bu çıkarmayı
/// kendi yapmaz — finansal toplamı ikinci kez hesaplamak, ekranda sunucununkiyle
/// tutmayan bir sayı üretmenin en kolay yolu.
/// </remarks>
public sealed record MonthlyScopeBreakdownResponse(
    ScopeTotalsResponse Business,
    ScopeTotalsResponse Personal);

public sealed record MonthlyReportResponse(
    int Year,
    int Month,

    // Hangi kapsamla okunduğu; boşsa toplam. Hesap bakiyeleri bu filtreden
    // etkilenmez ve her zaman toplamı gösterir (ADR 0013).
    string? Scope,
    string TotalIncome,
    string TotalExpense,
    string Net,
    string Currency,
    IReadOnlyList<CategoryExpenseResponse> CategoryExpenses,
    IReadOnlyList<CategoryExpenseSliceResponse> CategoryExpenseSlices,
    IReadOnlyList<AccountBalanceResponse> AccountBalances,
    MonthlyScopeBreakdownResponse? ScopeBreakdown);

public sealed record PeriodTotalsResponse(
    int Year,
    int Month,
    string Income,
    string Expense,
    string Net);

public sealed record PeriodComparisonResponse(
    PeriodTotalsResponse Current,
    PeriodTotalsResponse Previous,
    string IncomeChange,
    string ExpenseChange,
    string NetChange);

public sealed record CashFlowPointResponse(
    int Year,
    int Month,
    string Income,
    string Expense,
    string Net);

public sealed record BudgetVarianceResponse(
    Guid CategoryId,
    string CategoryName,
    string Limit,
    string Spent,
    string Remaining,
    bool IsExceeded);

public sealed record FutureLoadResponse(
    string FromDate,
    string ThroughDate,
    string RecurringAmount,
    string CreditCardStatementAmount,
    string InstallmentAmount,
    string DebtInstallmentAmount,
    string TotalAmount);

public sealed record CardDebtResponse(
    Guid CreditCardId,
    string CreditCardName,
    string Debt,
    string AvailableLimit);

public sealed record NetWorthResponse(
    string LiquidAssets,
    string CreditCardDebt,
    string ReceivableDebt,
    string PayableDebt,
    string NetWorth,
    /// <summary>
    /// POS'tan geçmiş ama henüz hesaba ulaşmamış paranın net toplamı.
    /// Kullanılabilir bakiye ile net varlığın farkı tam olarak budur
    /// (ADR 0015).
    /// </summary>
    string MoneyInTransit);

public sealed record AdvancedFinancialReportResponse(
    string AsOfDate,
    string Currency,

    // Hangi kapsamla okunduğu; boşsa toplam. Net varlık ile hesap/kart dağılımı
    // bu filtreden etkilenmez ve her zaman toplamı gösterir (ADR 0013).
    string? Scope,
    NetWorthResponse NetWorth,
    PeriodComparisonResponse PeriodComparison,
    IReadOnlyList<CashFlowPointResponse> CashFlowTrend,
    IReadOnlyList<BudgetVarianceResponse> BudgetVariances,
    FutureLoadResponse FutureLoad,
    IReadOnlyList<AccountBalanceResponse> AccountDistribution,
    IReadOnlyList<CardDebtResponse> CardDistribution);
