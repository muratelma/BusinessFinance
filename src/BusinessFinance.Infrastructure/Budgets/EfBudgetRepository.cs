using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Reports;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Budgets;

internal sealed class EfBudgetRepository(BusinessFinanceDbContext dbContext)
    : IBudgetRepository
{
    public Task<bool> ExistsAsync(
        Guid userId,
        Guid categoryId,
        int year,
        int month,
        CancellationToken cancellationToken) => dbContext.MonthlyBudgets.AsNoTracking().AnyAsync(
        budget => budget.UserId == userId &&
                  budget.CategoryId == categoryId &&
                  budget.Year == year &&
                  budget.Month == month,
        cancellationToken);

    public async Task AddAsync(MonthlyBudget budget, CancellationToken cancellationToken)
    {
        await dbContext.MonthlyBudgets.AddAsync(budget, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<MonthlyBudget?> FindOwnedByIdAsync(
        Guid budgetId,
        Guid userId,
        CancellationToken cancellationToken) => dbContext.MonthlyBudgets.SingleOrDefaultAsync(
        budget => budget.Id == budgetId && budget.UserId == userId,
        cancellationToken);

    public async Task UpdateOwnedAsync(
        MonthlyBudget budget,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (budget.UserId != userId)
        {
            throw new InvalidOperationException("Owned budget was not found.");
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteOwnedAsync(
        Guid budgetId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var budget = await dbContext.MonthlyBudgets.SingleOrDefaultAsync(
            value => value.Id == budgetId && value.UserId == userId,
            cancellationToken);
        if (budget is null)
        {
            return false;
        }
        dbContext.MonthlyBudgets.Remove(budget);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<BudgetDto>> ListWithProgressAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var periodStart = new DateOnly(year, month, 1);
        var periodEndExclusive = periodStart.AddMonths(1);
        var budgets = await dbContext.MonthlyBudgets.AsNoTracking()
            .Where(budget => budget.UserId == userId &&
                             budget.Year == year && budget.Month == month)
            .OrderBy(budget => budget.CategoryId)
            .ToArrayAsync(cancellationToken);
        var categoryIds = budgets.Select(budget => budget.CategoryId).ToArray();
        var names = await dbContext.Categories.AsNoTracking()
            .Where(category => category.UserId == userId && categoryIds.Contains(category.Id))
            .ToDictionaryAsync(category => category.Id, category => category.Name, cancellationToken);


        // Bütçenin harcaması, raporda o kategorinin o kapsamdaki gideridir
        // (kullanıcı kararı, 6 Ekim 2026). Kaynakların listesi tek yerdedir
        // (RecognizedItems): hesaptan gider, kart harcaması, vadeli alım,
        // borçla alınan mal, tek seferlik borç, POS komisyonu ve ödenen borç
        // faizi. Harcama kategoriyle değil, kategori + kapsam çiftiyle
        // okunur: aynı kategori hem işletme hem şahsi harcama tutabildiği
        // için ikisini birden saymak, kullanıcının koymadığı bir sınırı
        // aşılmış gösterirdi.
        var spent = await RecognizedItems.ExpenseByCategoryAndScopeAsync(
            dbContext, userId, periodStart, periodEndExclusive, cancellationToken);

        return budgets.Select(budget =>
        {
            var spentAmount = spent.GetValueOrDefault((budget.CategoryId, budget.Scope));
            return new BudgetDto(
                budget.Id,
                budget.CategoryId,
                names[budget.CategoryId],
                budget.Limit.Amount,
                spentAmount,
                Math.Max(budget.Limit.Amount - spentAmount, 0m),
                Math.Max(spentAmount - budget.Limit.Amount, 0m),
                budget.Limit.Currency,
                budget.Scope,
                budget.Year,
                budget.Month);
        }).OrderBy(item => item.CategoryName).ThenBy(item => item.Id).ToArray();
    }
}
