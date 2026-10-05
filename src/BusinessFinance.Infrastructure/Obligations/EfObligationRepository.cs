using BusinessFinance.Application.Obligations;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Obligations;

internal sealed class EfObligationRepository(BusinessFinanceDbContext dbContext)
    : IObligationRepository
{
    public async Task AddAsync(Obligation obligation, CancellationToken cancellationToken)
    {
        await dbContext.Obligations.AddAsync(obligation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ObligationDto>> ListAsync(
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken) => await (
            from obligation in dbContext.Obligations.AsNoTracking()
            join category in dbContext.Categories.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CategoryId }
                equals new { category.UserId, category.Id }
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CounterpartyId }
                equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                into counterparties
            from counterparty in counterparties.DefaultIfEmpty()
            where obligation.UserId == userId
            orderby obligation.DueDate, obligation.Id
            select new ObligationDto(
                obligation.Id,
                obligation.CounterpartyId,
                obligation.CategoryId,
                obligation.Direction,
                obligation.Amount.Amount,
                obligation.Amount.Currency,
                obligation.Scope,
                obligation.IssueDate,
                obligation.DueDate,
                obligation.Description,
                obligation.IsCancelled
                    ? ObligationStatus.Cancelled
                    : obligation.Settlement == null
                        ? ObligationStatus.Open
                        : ObligationStatus.Settled,
                counterparty == null ? null : counterparty.Name,
                category.Name,
                !obligation.IsCancelled && obligation.Settlement == null &&
                    obligation.DueDate < asOfDate,
                obligation.Settlement == null ? null : obligation.Settlement.Id,
                obligation.Settlement == null ? null : obligation.Settlement.AccountId,
                obligation.Settlement == null ? null : obligation.Settlement.SettlementDate))
            .ToArrayAsync(cancellationToken);

    public Task<Obligation?> FindOwnedByIdAsync(
        Guid obligationId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.Obligations : dbContext.Obligations.AsNoTracking())
            .Include(obligation => obligation.Settlement)
            .SingleOrDefaultAsync(
                obligation => obligation.Id == obligationId && obligation.UserId == userId,
                cancellationToken);

    public async Task SaveSettlementAsync(
        PosSettlement? cardSettlement,
        CancellationToken cancellationToken)
    {
        if (cardSettlement is not null)
        {
            await dbContext.PosSettlements.AddAsync(cardSettlement, cancellationToken);
        }

        var newSettlement = dbContext.ChangeTracker
            .Entries<ObligationSettlement>()
            .SingleOrDefault(entry => entry.State == EntityState.Modified);
        if (newSettlement is not null)
        {
            newSettlement.State = EntityState.Added;
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            dbContext.ChangeTracker.Clear();
            throw new ObligationConcurrencyException();
        }
    }
}
