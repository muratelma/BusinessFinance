using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.Transfers;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Transfers;

internal sealed class EfTransferRepository(BusinessFinanceDbContext dbContext)
    : ITransferRepository
{
    public async Task AddAsync(Transfer transfer, CancellationToken cancellationToken)
    {
        await dbContext.Transfers.AddAsync(transfer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Transfer?> FindOwnedByIdAsync(
        Guid transferId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken)
    {
        IQueryable<Transfer> query = dbContext.Transfers;
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return query.SingleOrDefaultAsync(
            transfer => transfer.Id == transferId && transfer.UserId == userId,
            cancellationToken);
    }

    /// <inheritdoc cref="BusinessFinance.Infrastructure.CreditCards.EfCardChargeRepository.ListAsync" />
    public async Task<BoundedList<Transfer>> ListAsync(
        Guid userId,
        HistoryWindow window,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Transfers
            .AsNoTracking()
            .Where(transfer => transfer.UserId == userId);
        if (window.From is DateOnly from) query = query.Where(transfer => transfer.TransferDate >= from);
        if (window.To is DateOnly to) query = query.Where(transfer => transfer.TransferDate <= to);
        var rows = await query
            .OrderByDescending(transfer => transfer.TransferDate)
            .ThenByDescending(transfer => transfer.Id)
            .Take(HistoryWindow.MaximumRows + 1)
            .ToArrayAsync(cancellationToken);
        return rows.Length > HistoryWindow.MaximumRows
            ? new BoundedList<Transfer>(rows[..HistoryWindow.MaximumRows], true)
            : new BoundedList<Transfer>(rows, false);
    }

    public async Task UpdateOwnedAsync(
        Transfer transfer,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (transfer.UserId != userId)
        {
            throw new InvalidOperationException("Owned transfer was not found.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
