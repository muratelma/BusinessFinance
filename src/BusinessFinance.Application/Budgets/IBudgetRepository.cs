using BusinessFinance.Domain;

namespace BusinessFinance.Application.Budgets;

public interface IBudgetRepository
{
    Task<bool> ExistsAsync(
        Guid userId,
        Guid categoryId,
        int year,
        int month,
        CancellationToken cancellationToken);
    Task AddAsync(MonthlyBudget budget, CancellationToken cancellationToken);
    Task<MonthlyBudget?> FindOwnedByIdAsync(
        Guid budgetId,
        Guid userId,
        CancellationToken cancellationToken);
    Task UpdateOwnedAsync(MonthlyBudget budget, Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<BudgetDto>> ListWithProgressAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken);
}
