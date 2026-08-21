using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

internal sealed class EfCardChargeRepository(BusinessFinanceDbContext dbContext)
    : ICardChargeRepository
{
    public async Task AddAsync(CreditCardCharge charge, CancellationToken cancellationToken)
    {
        await dbContext.CreditCardCharges.AddAsync(charge, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<CreditCardCharge?> FindOwnedByIdAsync(Guid chargeId, Guid userId, bool track, CancellationToken cancellationToken)
    {
        IQueryable<CreditCardCharge> query = dbContext.CreditCardCharges;
        if (!track) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(
            charge => charge.Id == chargeId && charge.UserId == userId,
            cancellationToken);
    }

    /// <summary>
    /// Kartın harcamaları, pencereyle ve satır tavanıyla sınırlı.
    /// </summary>
    /// <remarks>
    /// Tavandan bir fazlası çekiliyor: kırpma olup olmadığını ikinci bir
    /// <c>COUNT</c> sorgusu olmadan anlamanın yolu bu.
    /// </remarks>
    public async Task<BoundedList<CreditCardCharge>> ListAsync(
        Guid creditCardId,
        Guid userId,
        HistoryWindow window,
        CancellationToken cancellationToken)
    {
        var query = dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.CreditCardId == creditCardId && charge.UserId == userId);
        if (window.From is DateOnly from) query = query.Where(charge => charge.ChargeDate >= from);
        if (window.To is DateOnly to) query = query.Where(charge => charge.ChargeDate <= to);
        var rows = await query
            .OrderByDescending(charge => charge.ChargeDate).ThenByDescending(charge => charge.Id)
            .Take(HistoryWindow.MaximumRows + 1)
            .ToArrayAsync(cancellationToken);
        return rows.Length > HistoryWindow.MaximumRows
            ? new BoundedList<CreditCardCharge>(rows[..HistoryWindow.MaximumRows], true)
            : new BoundedList<CreditCardCharge>(rows, false);
    }

    public async Task UpdateOwnedAsync(CreditCardCharge charge, Guid userId, CancellationToken cancellationToken)
    {
        if (charge.UserId != userId) throw new InvalidOperationException("Owned charge was not found.");
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
