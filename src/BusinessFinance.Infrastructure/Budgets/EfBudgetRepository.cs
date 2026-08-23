using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Domain;
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

        // Harcama kategoriyle değil, kategori + kapsam çiftiyle toplanır. Aynı
        // kategori hem işletme hem şahsi harcama tutabildiği için bütçenin
        // hangi tarafı sınırladığı kendi kapsamından okunur; ikisini birden
        // saymak, kullanıcının koymadığı bir sınırı aşılmış gösterirdi. Aynı
        // kural MonthlyBudget.CalculateProgress içinde de yazılı.
        var transactionSpent = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId &&
                                  categoryIds.Contains(transaction.CategoryId) &&
                                  transaction.Type == TransactionType.Expense &&
                                  !transaction.IsCancelled &&
                                  transaction.TransactionDate >= periodStart &&
                                  transaction.TransactionDate < periodEndExclusive)
            .GroupBy(transaction => new { transaction.CategoryId, transaction.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Amount = group.Sum(x => x.Amount.Amount)
            })
            .ToDictionaryAsync(
                value => (value.CategoryId, value.Scope),
                value => value.Amount,
                cancellationToken);
        var cardSpent = await dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             categoryIds.Contains(charge.CategoryId) &&
                             !charge.IsCancelled &&
                             charge.ChargeDate >= periodStart &&
                             charge.ChargeDate < periodEndExclusive)
            .GroupBy(charge => new { charge.CategoryId, charge.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Amount = group.Sum(x => x.Amount.Amount)
            })
            .ToDictionaryAsync(
                value => (value.CategoryId, value.Scope),
                value => value.Amount,
                cancellationToken);

        // Gider kaynaklı borcun açılışı bütçeyi de tüketir: tüketim gerçek ve
        // kategorilidir. Taksit ödemesi bütçeye dokunmaz — dokunsaydı aynı
        // tüketim bütçeden iki kez düşerdi.
        var debtSpent = await dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.SourceType == DebtSourceType.Expense &&
                           debt.CategoryId != null &&
                           categoryIds.Contains(debt.CategoryId.Value) &&
                           debt.StartDate >= periodStart &&
                           debt.StartDate < periodEndExclusive)
            .GroupBy(debt => new { CategoryId = debt.CategoryId!.Value, debt.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Amount = group.Sum(x => x.Principal.Amount)
            })
            .ToDictionaryAsync(
                value => (value.CategoryId, value.Scope),
                value => value.Amount,
                cancellationToken);

        // Vadeli alım da bütçeyi tüketir; tahsilat tüketmez. Rapor
        // tarafındaki `GetBudgetVariancesAsync` ile aynı kural.
        var counterpartySpent = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             categoryIds.Contains(charge.CategoryId) &&
                             !charge.IsCancelled &&
                             charge.Direction == DebtDirection.Payable &&
                             charge.ChargeDate >= periodStart &&
                             charge.ChargeDate < periodEndExclusive)
            .GroupBy(charge => new { charge.CategoryId, charge.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Amount = group.Sum(x => x.Amount.Amount)
            })
            .ToDictionaryAsync(
                value => (value.CategoryId, value.Scope),
                value => value.Amount,
                cancellationToken);

        return budgets.Select(budget =>
        {
            var key = (budget.CategoryId, budget.Scope);
            var spentAmount = transactionSpent.GetValueOrDefault(key) +
                              cardSpent.GetValueOrDefault(key) +
                              debtSpent.GetValueOrDefault(key) +
                              counterpartySpent.GetValueOrDefault(key);
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
