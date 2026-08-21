using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Debts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Debts;

internal sealed class EfDebtRepository(BusinessFinanceDbContext dbContext) : IDebtRepository
{
    public async Task AddAsync(DebtAgreement debt, CancellationToken cancellationToken)
    {
        dbContext.DebtAgreements.Add(debt);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DebtAgreement>> ListAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.DebtAgreements.AsNoTracking().Include(x => x.Installments)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.FirstDueDate).ThenBy(x => x.Id)
            .ToArrayAsync(cancellationToken);

    public Task<DebtAgreement?> FindOwnedByIdAsync(
        Guid id, Guid userId, bool track, CancellationToken cancellationToken) =>
        (track ? dbContext.DebtAgreements : dbContext.DebtAgreements.AsNoTracking())
            .Include(x => x.Installments)
            .SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    // Açılış tamamlanırken taksitlerin anapara/faiz ayrımı da doluyor, yani
    // aggregate'in tamamı tek `SaveChanges` sınırında yazılır. Yarım yazılırsa
    // kaynağı olan ama ayrımı olmayan bir borç kalırdı.
    public async Task SaveOpeningAsync(DebtAgreement debt, CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    public async Task SavePaymentAsync(DebtInstallment installment, CancellationToken cancellationToken)
    {
        try { await dbContext.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            throw new DebtConcurrencyException();
        }
    }
}
