using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Accounts;

internal sealed class EfAccountRepository(BusinessFinanceDbContext dbContext)
    : IAccountRepository, IAccountDayFlowReader
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken)
    {
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid userId,
        string normalizedName,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Accounts
            .AsNoTracking()
            .Where(account => account.UserId == userId);

        if (string.Equals(
                dbContext.Database.ProviderName,
                "Microsoft.EntityFrameworkCore.InMemory",
                StringComparison.Ordinal))
        {
            var names = await query
                .Select(account => account.Name)
                .ToArrayAsync(cancellationToken);

            return names.Contains(normalizedName, StringComparer.OrdinalIgnoreCase);
        }

        return await query.AnyAsync(
            account => account.Name == normalizedName,
            cancellationToken);
    }

    public async Task<AccountListPage> ListAsync(
        Guid userId,
        AccountListCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Account> query = dbContext.Accounts
            .AsNoTracking()
            .Where(account => account.UserId == userId);

        if (criteria.IsActive is bool isActive)
        {
            query = query.Where(account => account.IsActive == isActive);
        }

        if (criteria.Type is AccountType type)
        {
            query = query.Where(account => account.Type == type);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(account => account.Name)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new AccountListPage(items, totalCount);
    }

    public Task<Account?> FindOwnedByIdAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Accounts.SingleOrDefaultAsync(
            account => account.Id == accountId && account.UserId == userId,
            cancellationToken);
    }

    public async Task UpdateOwnedAsync(
        Account account,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (account.UserId != userId)
        {
            throw new InvalidOperationException("Owned account was not found.");
        }

        var entry = dbContext.Entry(account);
        if (entry.State == EntityState.Detached)
        {
            var exists = await dbContext.Accounts
                .AsNoTracking()
                .AnyAsync(
                    candidate => candidate.Id == account.Id && candidate.UserId == userId,
                    cancellationToken);

            if (!exists)
            {
                throw new InvalidOperationException("Owned account was not found.");
            }

            dbContext.Attach(account);
            entry.Property(candidate => candidate.IsActive).IsModified = true;
            entry.Property(candidate => candidate.Name).IsModified = true;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> CalculateBalanceAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var openingBalance = await dbContext.Accounts
            .AsNoTracking()
            .Where(account => account.Id == accountId && account.UserId == userId)
            .Select(account => (decimal?)account.OpeningBalance)
            .SingleOrDefaultAsync(cancellationToken);

        if (openingBalance is null)
        {
            throw new InvalidOperationException("Owned account was not found.");
        }

        // Hareketlerin listesi tek yerdedir; "işlem sonrası bakiye" de aynı
        // listeyi bir kesim noktasıyla okur.
        return openingBalance.Value + await AccountMovements.SumAsync(
            dbContext, accountId, userId, cutoff: null, cancellationToken);
    }

    // `CalculateBalanceAsync` ile aynı liste, o güne daraltılmış ve yönleri
    // ayrı toplanmış. Açılış bakiyesi bir gün hareketi değildir.
    public Task<(decimal Inflow, decimal Outflow)> CalculateDayFlowAsync(
        Guid accountId,
        Guid userId,
        DateOnly day,
        CancellationToken cancellationToken) =>
        AccountMovements.FlowAsync(dbContext, accountId, userId, day, day, cancellationToken);
}
