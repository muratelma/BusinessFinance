using BusinessFinance.Application.Cash;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Cash;

internal sealed class EfCashCountRepository(BusinessFinanceDbContext dbContext)
    : ICashCountRepository
{
    public Task<CashCount?> FindOpenAsync(
        Guid userId,
        Guid accountId,
        DateOnly countDate,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.CashCounts : dbContext.CashCounts.AsNoTracking())
            .SingleOrDefaultAsync(
                count => count.UserId == userId &&
                         count.AccountId == accountId &&
                         count.CountDate == countDate &&
                         !count.IsCancelled,
                cancellationToken);

    public async Task AddAsync(
        CashCount cashCount,
        CashCount? superseded,
        CancellationToken cancellationToken)
    {
        await dbContext.CashCounts.AddAsync(cashCount, cancellationToken);

        // Yerini aldığı sayım aynı çağrıda iptal edilmiş olarak geliyor; ikisi
        // tek SaveChanges sınırında yazılır. Ayrı yazılsaydı SQL'deki filtreli
        // tekil indeks araya düşen ikinci açık sayımı reddederdi.
        if (superseded is not null &&
            dbContext.Entry(superseded).State == EntityState.Detached)
        {
            dbContext.CashCounts.Attach(superseded);
            dbContext.Entry(superseded).State = EntityState.Modified;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CashCountDto>> ListAsync(
        Guid userId,
        CashCountListCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query =
            from count in dbContext.CashCounts.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { count.UserId, Id = count.AccountId }
                equals new { account.UserId, account.Id }
            where count.UserId == userId &&
                  count.CountDate >= criteria.From &&
                  count.CountDate <= criteria.To
            select new { count, account.Name };

        if (criteria.AccountId is Guid accountId)
        {
            query = query.Where(row => row.count.AccountId == accountId);
        }

        var rows = await query
            .OrderByDescending(row => row.count.CountDate)
            .ThenByDescending(row => row.count.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        // Beklenen bakiye ve fark burada **doldurulmaz**: geçmiş bir günün
        // farkını bugünkü bakiyeye karşı hesaplamak, aradaki bütün hareketleri
        // o günün farkına yazmak olurdu. Günün sayımını beklenen bakiyesiyle
        // birlikte okuyan ayrı bir uç var.
        return rows
            .Select(row => new CashCountDto(
                row.count.Id,
                row.count.AccountId,
                row.Name,
                row.count.CountDate,
                row.count.CountedAmount,
                row.count.Currency,
                row.count.Scope,
                row.count.Note,
                row.count.IsCancelled,
                row.count.AdjustmentTransactionId))
            .ToArray();
    }

    public Task<CashCount?> FindOwnedByIdAsync(
        Guid cashCountId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.CashCounts : dbContext.CashCounts.AsNoTracking())
            .SingleOrDefaultAsync(
                count => count.Id == cashCountId && count.UserId == userId,
                cancellationToken);

    public async Task SaveAdjustmentAsync(
        BudgetTransaction adjustment,
        CancellationToken cancellationToken)
    {
        await dbContext.Transactions.AddAsync(adjustment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
