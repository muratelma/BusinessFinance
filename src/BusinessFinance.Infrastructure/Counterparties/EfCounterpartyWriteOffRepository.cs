using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Counterparties;

internal sealed class EfCounterpartyWriteOffRepository(BusinessFinanceDbContext dbContext)
    : ICounterpartyWriteOffRepository
{
    public async Task<bool> AddAsync(CounterpartyWriteOff writeOff, CancellationToken cancellationToken)
    {
        await dbContext.CounterpartyWriteOffs.AddAsync(writeOff, cancellationToken);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Aynı istek kimliğiyle ikinci yazma: var olan kayıt döner.
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public Task<CounterpartyWriteOff?> FindOwnedAsync(
        Guid writeOffId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.CounterpartyWriteOffs.SingleOrDefaultAsync(
            writeOff => writeOff.Id == writeOffId && writeOff.UserId == userId,
            cancellationToken);

    public Task SaveAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
