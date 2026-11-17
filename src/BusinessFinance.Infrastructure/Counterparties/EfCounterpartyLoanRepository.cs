using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Counterparties;

internal sealed class EfCounterpartyLoanRepository(BusinessFinanceDbContext dbContext)
    : ICounterpartyLoanRepository
{
    public async Task<bool> AddAsync(CounterpartyLoan loan, CancellationToken cancellationToken)
    {
        await dbContext.CounterpartyLoans.AddAsync(loan, cancellationToken);
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

    public Task<CounterpartyLoan?> FindOwnedAsync(
        Guid loanId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.CounterpartyLoans.SingleOrDefaultAsync(
            loan => loan.Id == loanId && loan.UserId == userId,
            cancellationToken);

    public async Task<bool> TrySaveAsync(CounterpartyLoan loan, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }
}
