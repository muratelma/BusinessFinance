using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;
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

        // Bütçenin harcaması, raporda o kategorinin o kapsamdaki gideridir
        // (kullanıcı kararı, 6 Ekim 2026). Tek seferlik borç, POS komisyonu ve
        // ödenen borç faizi de tanınmış giderlerdir; burada sayılmadıkları
        // için Bütçeler ekranı ile kategori dağılımı aynı kategoriye iki ayrı
        // sayı gösteriyordu.
        var obligationSpent = await dbContext.Obligations.AsNoTracking()
            .Where(obligation => obligation.UserId == userId &&
                                 categoryIds.Contains(obligation.CategoryId) &&
                                 !obligation.IsCancelled &&
                                 obligation.Direction == DebtDirection.Payable &&
                                 obligation.IssueDate >= periodStart &&
                                 obligation.IssueDate < periodEndExclusive)
            .GroupBy(obligation => new { obligation.CategoryId, obligation.Scope })
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
        var posCommissionSpent = await dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 settlement.CommissionCategoryId != null &&
                                 categoryIds.Contains(settlement.CommissionCategoryId.Value) &&
                                 !settlement.IsCancelled &&
                                 settlement.SettlementDate >= periodStart &&
                                 settlement.SettlementDate < periodEndExclusive)
            // Komisyonlu kaydın kapsamı doludur (satışta da kartla tahsilde de).
            .GroupBy(settlement => new
            {
                CategoryId = settlement.CommissionCategoryId!.Value,
                Scope = settlement.Scope!.Value
            })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Amount = group.Sum(x => x.CommissionAmount)
            })
            .ToDictionaryAsync(
                value => (value.CategoryId, value.Scope),
                value => value.Amount,
                cancellationToken);
        var interestCategoryId = await dbContext.Categories.AsNoTracking()
            .Where(category => category.UserId == userId &&
                               category.Type == CategoryType.Expense &&
                               category.Name == EfCategoryRepository.InterestExpenseCategoryName)
            .Select(category => (Guid?)category.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var interestSpent = interestCategoryId is Guid interestId && categoryIds.Contains(interestId)
            ? await (
                    from installment in dbContext.DebtInstallments.AsNoTracking()
                    join debt in dbContext.DebtAgreements.AsNoTracking()
                        on new { installment.UserId, DebtId = installment.DebtAgreementId }
                        equals new { debt.UserId, DebtId = debt.Id }
                    where installment.UserId == userId &&
                          installment.InterestPortion != null &&
                          debt.Direction == DebtDirection.Payable &&
                          installment.PaymentDate >= periodStart &&
                          installment.PaymentDate < periodEndExclusive
                    group installment by debt.Scope into scopeGroup
                    select new
                    {
                        Scope = scopeGroup.Key,
                        Amount = scopeGroup.Sum(x => x.InterestPortion!.Value)
                    })
                .ToDictionaryAsync(value => value.Scope, value => value.Amount, cancellationToken)
            : [];

        return budgets.Select(budget =>
        {
            var key = (budget.CategoryId, budget.Scope);
            var spentAmount = transactionSpent.GetValueOrDefault(key) +
                              cardSpent.GetValueOrDefault(key) +
                              debtSpent.GetValueOrDefault(key) +
                              counterpartySpent.GetValueOrDefault(key) +
                              obligationSpent.GetValueOrDefault(key) +
                              posCommissionSpent.GetValueOrDefault(key) +
                              (budget.CategoryId == interestCategoryId
                                  ? interestSpent.GetValueOrDefault(budget.Scope)
                                  : 0m);
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
