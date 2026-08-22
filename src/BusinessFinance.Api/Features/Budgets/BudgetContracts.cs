namespace BusinessFinance.Api.Features.Budgets;

public sealed record CreateBudgetRequest(
    Guid CategoryId,
    string Limit,
    string Currency,
    // İsteğe bağlı: boş bırakılırsa sunucu kapsamı türetir, türetemezse
    // isteği reddeder ve bir değer uydurmaz.
    string? Scope,
    int Year,
    int Month);
public sealed record UpdateBudgetRequest(string Limit, string Currency);
public sealed record BudgetResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string Limit,
    string Spent,
    string Remaining,
    string Exceeded,
    string Currency,
    string Scope,
    int Year,
    int Month);
public sealed record BudgetListResponse(IReadOnlyList<BudgetResponse> Items);
