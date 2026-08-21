using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

internal sealed class EfCreditCardRepository(BusinessFinanceDbContext dbContext)
    : ICreditCardRepository
{
    public async Task AddAsync(CreditCard creditCard, CancellationToken cancellationToken)
    {
        await dbContext.CreditCards.AddAsync(creditCard, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid userId,
        string normalizedName,
        Guid? exceptCreditCardId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.CreditCards
            .AsNoTracking()
            .Where(card => card.UserId == userId && card.Id != exceptCreditCardId);

        if (string.Equals(
                dbContext.Database.ProviderName,
                "Microsoft.EntityFrameworkCore.InMemory",
                StringComparison.Ordinal))
        {
            var names = await query.Select(card => card.Name).ToArrayAsync(cancellationToken);
            return names.Contains(normalizedName, StringComparer.OrdinalIgnoreCase);
        }

        return await query.AnyAsync(card => card.Name == normalizedName, cancellationToken);
    }

    public Task<CreditCard?> FindOwnedByIdAsync(
        Guid creditCardId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken)
    {
        IQueryable<CreditCard> query = dbContext.CreditCards;
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return query.SingleOrDefaultAsync(
            card => card.Id == creditCardId && card.UserId == userId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<CreditCard>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.CreditCards
            .AsNoTracking()
            .Where(card => card.UserId == userId)
            .OrderBy(card => card.Name)
            .ThenBy(card => card.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<decimal> CalculateCurrentDebtAsync(
        Guid creditCardId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var charges = await dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.CreditCardId == creditCardId &&
                             charge.UserId == userId &&
                             !charge.IsCancelled)
            .SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var payments = await dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.CreditCardId == creditCardId &&
                              payment.UserId == userId &&
                              !payment.IsCancelled)
            .SumAsync(payment => payment.Amount.Amount, cancellationToken);
        return Math.Max(0m, charges - payments);
    }

    public async Task UpdateOwnedAsync(
        CreditCard creditCard,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (creditCard.UserId != userId)
        {
            throw new InvalidOperationException("Owned credit card was not found.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
