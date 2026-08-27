using BusinessFinance.Domain;

namespace BusinessFinance.Application.Budgets;

public sealed record BudgetDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    decimal Limit,
    decimal Spent,
    decimal Remaining,
    decimal Exceeded,
    CurrencyCode Currency,
    TransactionScope Scope,
    int Year,
    int Month);

public sealed record CreateBudgetCommand(
    Guid CategoryId,
    decimal Limit,
    CurrencyCode Currency,

    // Kullanıcının açık seçimi; boşsa kategorinin varsayılanı kullanılır.
    TransactionScope? Scope,
    int Year,
    int Month);

public sealed record UpdateBudgetCommand(
    Guid BudgetId,
    decimal Limit,
    CurrencyCode Currency);

public sealed record DeleteBudgetCommand(Guid BudgetId);
