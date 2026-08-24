using BusinessFinance.Application.Obligations;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Obligations;

internal sealed class EfObligationRepository(BusinessFinanceDbContext dbContext)
    : IObligationRepository
{
    public async Task AddAsync(Obligation obligation, CancellationToken cancellationToken)
    {
        await dbContext.Obligations.AddAsync(obligation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
