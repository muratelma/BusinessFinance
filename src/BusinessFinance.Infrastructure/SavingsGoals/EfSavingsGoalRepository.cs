using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.SavingsGoals;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.SavingsGoals;

internal sealed class EfSavingsGoalRepository(BusinessFinanceDbContext dbContext)
    : ISavingsGoalRepository
{
    public async Task AddAsync(SavingsGoal goal, CancellationToken cancellationToken)
    {
        dbContext.SavingsGoals.Add(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SavingsGoal>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
        await dbContext.SavingsGoals.AsNoTracking().Include(x => x.Contributions)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.TargetDate).ThenBy(x => x.Name).ThenBy(x => x.Id)
            .ToArrayAsync(cancellationToken);

    public Task<SavingsGoal?> FindOwnedByIdAsync(
        Guid goalId,
        Guid userId,
        CancellationToken cancellationToken) =>
        dbContext.SavingsGoals.Include(x => x.Contributions)
            .SingleOrDefaultAsync(x => x.Id == goalId && x.UserId == userId, cancellationToken);

    public Task<SavingsGoalContribution?> FindContributionAsync(
        Guid goalId,
        Guid userId,
        Guid clientRequestId,
        CancellationToken cancellationToken) =>
        dbContext.SavingsGoalContributions.SingleOrDefaultAsync(
            x => x.SavingsGoalId == goalId && x.UserId == userId &&
                 x.ClientRequestId == clientRequestId,
            cancellationToken);

    public async Task<SavingsGoalContribution> AddOrGetContributionAsync(
        SavingsGoalContribution contribution,
        CancellationToken cancellationToken)
    {
        dbContext.SavingsGoalContributions.Add(contribution);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return contribution;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            dbContext.ChangeTracker.Clear();
            return await dbContext.SavingsGoalContributions.AsNoTracking().SingleAsync(
                x => x.UserId == contribution.UserId &&
                     x.SavingsGoalId == contribution.SavingsGoalId &&
                     x.ClientRequestId == contribution.ClientRequestId,
                cancellationToken);
        }
    }

    public async Task<SavingsGoalDeletionResult> DeleteOwnedIfWithoutContributionsAsync(
        Guid goalId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        await using var transaction = dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken)
            : null;
        var goal = await dbContext.SavingsGoals.SingleOrDefaultAsync(
            candidate => candidate.Id == goalId && candidate.UserId == userId,
            cancellationToken);
        if (goal is null)
        {
            return SavingsGoalDeletionResult.NotFound;
        }

        var hasContributions = await dbContext.SavingsGoalContributions.AnyAsync(
            contribution => contribution.SavingsGoalId == goalId &&
                            contribution.UserId == userId,
            cancellationToken);
        if (hasContributions)
        {
            return SavingsGoalDeletionResult.HasContributions;
        }

        dbContext.SavingsGoals.Remove(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }
        return SavingsGoalDeletionResult.Deleted;
    }
}
