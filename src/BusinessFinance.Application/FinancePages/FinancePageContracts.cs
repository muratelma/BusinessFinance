using BusinessFinance.Domain;
using BusinessFinance.Application.FinancialActivities;

namespace BusinessFinance.Application.FinancePages;

public sealed record CategoryPageQuery(int Year, int Month, TransactionType Type,
    TransactionScope? Scope = null, string? Source = null, string? Person = null, int Offset = 0);
public sealed record AccountPageQuery(int Year, int Month, string? Direction = null,
    string? Selection = null, int Offset = 0);
public sealed record PageOption(string Key, string Group, string? Name, decimal Amount,
    string? Kind = null, Guid? Id = null);
public sealed record CategorySlice(Guid? Id, string? Name, decimal Amount, decimal Share, decimal BarRatio);
public sealed record CategorySummary(int Year, int Month, string Direction, string? Scope,
    decimal Total, decimal PreviousTotal, int CategoryCount, IReadOnlyList<CategorySlice> Categories,
    IReadOnlyList<PageOption> Sources);
public sealed record PageActivity(FinancialActivityRow Activity, decimal Amount, string? Part = null,
    decimal? PartAmount = null, decimal? ParentAmount = null, string? State = null,
    DateOnly? SettledOn = null, string? SettledSource = null, bool ShowScope = false,
    DateOnly? FirstSaleDate = null, DateOnly? LastSaleDate = null, string? Scope = null);
public sealed record CategoryPage(CategorySummary Summary, Guid? CategoryId, string? Name,
    decimal WholeTotal, bool ShowScope, bool HasPeople, IReadOnlyList<PageOption> People,
    IReadOnlyList<PageActivity> Items, int TotalCount, int Offset, bool HasMore);
public sealed record MonthValue(int Month, decimal Amount, decimal BarRatio, bool Enabled);
public sealed record YearMonths(int Year, IReadOnlyList<MonthValue> Months);
public sealed record TransitSummary(decimal Total, int Count, PageActivity? Nearest);
public sealed record TransitPage(Guid AccountId, string Name, decimal Total, int TotalCount,
    IReadOnlyList<PageActivity> Items, int Offset, bool HasMore);
public sealed record AccountPage(Guid AccountId, string Name, string Type, string? DefaultScope,
    bool IsActive, int Year, int Month, decimal Balance, decimal CurrentBalance, bool IsCurrentMonth,
    decimal Inflow, decimal Outflow, decimal Net, decimal SelectionTotal,
    IReadOnlyList<PageOption> Options, TransitSummary Transit, IReadOnlyList<PageActivity> Items,
    int TotalCount, int Offset, bool HasMore);
public sealed record AccountListItem(Guid Id, string Name, string Type, string? DefaultScope,
    bool IsActive, decimal Balance);
public sealed record AccountsOverview(decimal Total, decimal CashTotal, decimal BankTotal,
    decimal InactiveTotal, IReadOnlyList<AccountListItem> Items);
public sealed record ActivityPart(string Kind, decimal Amount, Guid? CategoryId, string? CategoryName);
public sealed record ActivityParts(IReadOnlyList<ActivityPart> Items);

public interface IFinancePageReader
{
    Task<CategorySummary> CategorySummaryAsync(Guid userId, CategoryPageQuery query, CancellationToken ct);
    Task<CategoryPage?> CategoryAsync(Guid userId, Guid? categoryId, CategoryPageQuery query, CancellationToken ct);
    Task<YearMonths?> CategoryMonthsAsync(Guid userId, Guid? categoryId, bool summary,
        CategoryPageQuery query, CancellationToken ct);
    Task<AccountPage?> AccountAsync(Guid userId, Guid accountId, AccountPageQuery query, CancellationToken ct);
    Task<YearMonths?> AccountMonthsAsync(Guid userId, Guid accountId, int year, CancellationToken ct);
    Task<TransitPage?> TransitAsync(Guid userId, Guid accountId, int offset, CancellationToken ct);
    Task<AccountsOverview> AccountsAsync(Guid userId, CancellationToken ct);
    Task<ActivityParts?> PartsAsync(Guid userId, FinancialActivityKind kind, Guid activityId, CancellationToken ct);
}
