using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

internal sealed class EfInstallmentPlanRepository(BusinessFinanceDbContext dbContext)
    : IInstallmentPlanRepository
{
    public Task<InstallmentPlan?> FindByClientRequestIdAsync(
        Guid userId,
        Guid clientRequestId,
        bool track,
        CancellationToken cancellationToken)
    {
        return Query(track).SingleOrDefaultAsync(
            plan => plan.UserId == userId && plan.ClientRequestId == clientRequestId,
            cancellationToken);
    }

    public Task<InstallmentPlan?> FindOwnedByIdAsync(
        Guid installmentPlanId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken)
    {
        return Query(track).SingleOrDefaultAsync(
            plan => plan.Id == installmentPlanId && plan.UserId == userId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<InstallmentPlan>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await Query(false)
            .Where(plan => plan.UserId == userId)
            .OrderBy(plan => plan.FirstInstallmentDate)
            .ThenBy(plan => plan.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(InstallmentPlan plan, CancellationToken cancellationToken)
    {
        await dbContext.InstallmentPlans.AddAsync(plan, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RealizeAsync(
        InstallmentItem item,
        CreditCardCharge charge,
        CancellationToken cancellationToken)
    {
        if (item.UserId != charge.UserId || item.CreditCardChargeId != charge.Id)
        {
            throw new InvalidOperationException("Installment realization is inconsistent.");
        }

        await dbContext.CreditCardCharges.AddAsync(charge, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<InstallmentPlan> Query(bool track)
    {
        IQueryable<InstallmentPlan> query = dbContext.InstallmentPlans.Include(plan => plan.Items);
        return track ? query : query.AsNoTracking();
    }
}
