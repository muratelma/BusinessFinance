using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Transactions;

internal sealed class EfTransactionRepository(BusinessFinanceDbContext dbContext)
    : ITransactionRepository
{
    public async Task AddAsync(
        BudgetTransaction transaction,
        CancellationToken cancellationToken)
    {
        await dbContext.Transactions.AddAsync(transaction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<BudgetTransaction?> FindOwnedByIdAsync(
        Guid transactionId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken)
    {
        IQueryable<BudgetTransaction> query = dbContext.Transactions;
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return query.SingleOrDefaultAsync(
            transaction => transaction.Id == transactionId && transaction.UserId == userId,
            cancellationToken);
    }

    public async Task UpdateOwnedAsync(
        BudgetTransaction transaction,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (transaction.UserId != userId)
        {
            throw new InvalidOperationException("Owned transaction was not found.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TransactionListPage> ListAsync(
        Guid userId,
        TransactionListCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<BudgetTransaction> query = dbContext.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.UserId == userId);
        if (criteria.DateFrom is DateOnly from)
        {
            query = query.Where(transaction => transaction.TransactionDate >= from);
        }
        if (criteria.DateTo is DateOnly to)
        {
            query = query.Where(transaction => transaction.TransactionDate <= to);
        }
        if (criteria.AccountId is Guid accountId)
        {
            query = query.Where(transaction => transaction.AccountId == accountId);
        }
        if (criteria.CategoryId is Guid categoryId)
        {
            query = query.Where(transaction => transaction.CategoryId == categoryId);
        }
        if (criteria.Type is TransactionType type)
        {
            query = query.Where(transaction => transaction.Type == type);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(transaction => transaction.TransactionDate)
            .ThenByDescending(transaction => transaction.Id)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);
        return new TransactionListPage(items, totalCount);
    }
}
