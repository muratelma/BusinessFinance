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
        // Karşılaştırma adın anahtarıyla yapılır (`NameKeys`).
        var key = NameKeys.Of(normalizedName);
        return await dbContext.CreditCards
            .AsNoTracking()
            .AnyAsync(
                card => card.UserId == userId &&
                        card.Id != exceptCreditCardId &&
                        card.NameKey == key,
                cancellationToken);
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
        return await CardDebt.SumAsync(
            dbContext, creditCardId, userId, cutoff: null, cancellationToken);
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
