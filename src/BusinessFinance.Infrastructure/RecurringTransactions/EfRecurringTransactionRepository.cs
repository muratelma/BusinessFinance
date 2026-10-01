using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.RecurringTransactions;

internal sealed class EfRecurringTransactionRepository(BusinessFinanceDbContext dbContext)
    : IRecurringTransactionRepository
{
    public Task<RecurringTransaction?> FindOwnedByIdAsync(
        Guid recurringTransactionId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken)
    {
        return RecurringQuery(track).SingleOrDefaultAsync(
            recurring => recurring.Id == recurringTransactionId && recurring.UserId == userId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringTransaction>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await RecurringQuery(false)
            .Where(recurring => recurring.UserId == userId)
            .OrderByDescending(recurring => recurring.IsActive)
            .ThenBy(recurring => recurring.NextOccurrenceDate)
            .ThenBy(recurring => recurring.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringTransactionOccurrence>> ListPlanOccurrencesAsync(
        Guid recurringTransactionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.RecurringTransactionOccurrences
            .Where(occurrence => occurrence.UserId == userId &&
                                 occurrence.RecurringTransactionId == recurringTransactionId)
            .OrderBy(occurrence => occurrence.ScheduledDate)
            .ToArrayAsync(cancellationToken);
    }

    public async Task SaveEditAsync(
        RecurringTransaction recurring,
        IReadOnlyCollection<RecurringTransactionOccurrence> removedOccurrences,
        CancellationToken cancellationToken)
    {
        if (dbContext.Entry(recurring).State == EntityState.Detached)
        {
            throw new InvalidOperationException("Recurring transaction must be tracked before update.");
        }

        // Yeniden kurulan bekleyenler tahmindir; hiçbir finansal kayıt onlara
        // bağlı değildir. Plan değişikliğiyle aynı sınırda silinirler.
        dbContext.RecurringTransactionOccurrences.RemoveRange(removedOccurrences);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringTransactionOccurrence>> ListClosedByAsync(
        Guid userId,
        Guid? transactionId,
        Guid? chargeId,
        CancellationToken cancellationToken)
    {
        if (transactionId is null && chargeId is null) return [];

        return await dbContext.RecurringTransactionOccurrences
            .Where(occurrence => occurrence.UserId == userId &&
                                 ((transactionId != null && occurrence.ClosedByTransactionId == transactionId) ||
                                  (chargeId != null && occurrence.ClosedByChargeId == chargeId)))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> TrySaveOccurrenceChangeAsync(CancellationToken cancellationToken)
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

    public async Task<IReadOnlyList<RecurringTransaction>> ListDueAsync(
        Guid userId,
        DateOnly throughDate,
        CancellationToken cancellationToken)
    {
        return await dbContext.RecurringTransactions
            .Where(recurring => recurring.UserId == userId &&
                                recurring.IsActive &&
                                recurring.NextOccurrenceDate != null &&
                                recurring.NextOccurrenceDate <= throughDate)
            .OrderBy(recurring => recurring.NextOccurrenceDate)
            .ThenBy(recurring => recurring.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(
        RecurringTransaction recurring,
        CancellationToken cancellationToken)
    {
        await dbContext.RecurringTransactions.AddAsync(recurring, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(
        IReadOnlyCollection<RecurringTransaction> plans,
        CancellationToken cancellationToken)
    {
        await dbContext.RecurringTransactions.AddRangeAsync(plans, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        RecurringTransaction recurring,
        CancellationToken cancellationToken)
    {
        if (dbContext.Entry(recurring).State == EntityState.Detached)
        {
            throw new InvalidOperationException("Recurring transaction must be tracked before update.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> TrySaveGeneratedAsync(
        IReadOnlyCollection<RecurringTransactionOccurrence> occurrences,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.RecurringTransactionOccurrences.AddRangeAsync(
                occurrences, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task<RecurringDeletionResult> DeleteOwnedIfUnrealizedAsync(
        Guid recurringTransactionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var schedule = await dbContext.RecurringTransactions.SingleOrDefaultAsync(
            item => item.Id == recurringTransactionId && item.UserId == userId,
            cancellationToken);
        if (schedule is null) return RecurringDeletionResult.NotFound;

        var occurrences = await dbContext.RecurringTransactionOccurrences
            .Where(occurrence => occurrence.UserId == userId &&
                                 occurrence.RecurringTransactionId == recurringTransactionId)
            .ToArrayAsync(cancellationToken);
        // Kapatılmış kalem de geçmiştir: bir vergi ödemesi ona bağlıdır.
        if (occurrences.Any(occurrence =>
                occurrence.Status != RecurringOccurrenceStatus.Planned))
        {
            return RecurringDeletionResult.HasRealizedHistory;
        }

        // Pending occurrences go with the plan: they are forecasts it produced,
        // and nothing financial points at them.
        dbContext.RecurringTransactionOccurrences.RemoveRange(occurrences);
        dbContext.RecurringTransactions.Remove(schedule);
        await dbContext.SaveChangesAsync(cancellationToken);
        return RecurringDeletionResult.Deleted;
    }

    public Task<bool> IsScheduleActiveAsync(
        Guid recurringTransactionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.RecurringTransactions
            .AsNoTracking()
            .AnyAsync(
                schedule => schedule.Id == recurringTransactionId &&
                            schedule.UserId == userId &&
                            schedule.IsActive,
                cancellationToken);
    }

    public Task<RecurringTransactionOccurrence?> FindOccurrenceOwnedByIdAsync(
        Guid occurrenceId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken)
    {
        return OccurrenceQuery(track).SingleOrDefaultAsync(
            occurrence => occurrence.Id == occurrenceId && occurrence.UserId == userId,
            cancellationToken);
    }

    /// <summary>
    /// Owner-scoped like every single-record read: another user's occurrence and a
    /// date that produced none both come back as null.
    /// </summary>
    public Task<RecurringTransactionOccurrence?> FindOccurrenceOwnedByDateAsync(
        Guid recurringTransactionId,
        DateOnly scheduledDate,
        Guid userId,
        CancellationToken cancellationToken,
        bool track = false)
    {
        return OccurrenceQuery(track).SingleOrDefaultAsync(
            occurrence => occurrence.UserId == userId &&
                          occurrence.RecurringTransactionId == recurringTransactionId &&
                          occurrence.ScheduledDate == scheduledDate,
            cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringTransactionOccurrence>> ListOccurrencesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Deactivating a schedule means "stop this plan". A pending occurrence
        // generated before that is a forecast, not a debt, so it stops being
        // offered. Realized ones stay: they are history and already moved money.
        // Reactivating the schedule brings the pending one back.
        return await OccurrenceQuery(false)
            .Where(occurrence => occurrence.UserId == userId)
            .Where(occurrence =>
                occurrence.Status != RecurringOccurrenceStatus.Planned ||
                dbContext.RecurringTransactions.Any(schedule =>
                    schedule.UserId == userId &&
                    schedule.Id == occurrence.RecurringTransactionId &&
                    schedule.IsActive))
            .OrderBy(occurrence => occurrence.ScheduledDate)
            .ThenBy(occurrence => occurrence.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<BudgetTransaction> RealizeAsync(
        RecurringTransactionOccurrence occurrence,
        BudgetTransaction transaction,
        CancellationToken cancellationToken)
    {
        if (occurrence.UserId != transaction.UserId ||
            occurrence.BudgetTransactionId != transaction.Id)
        {
            throw new InvalidOperationException("Recurring occurrence realization is inconsistent.");
        }

        try
        {
            await dbContext.Transactions.AddAsync(transaction, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return transaction;
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            var winner = await dbContext.RecurringTransactionOccurrences
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.Id == occurrence.Id && item.UserId == occurrence.UserId,
                    cancellationToken);
            if (winner?.BudgetTransactionId is not Guid winnerTransactionId)
            {
                throw;
            }

            return await dbContext.Transactions
                .AsNoTracking()
                .SingleAsync(
                    item => item.Id == winnerTransactionId && item.UserId == occurrence.UserId,
                    cancellationToken);
        }
    }

    public async Task<CreditCardCharge> RealizeWithChargeAsync(
        RecurringTransactionOccurrence occurrence,
        CreditCardCharge charge,
        CancellationToken cancellationToken)
    {
        if (occurrence.UserId != charge.UserId ||
            occurrence.CreditCardChargeId != charge.Id)
        {
            throw new InvalidOperationException("Recurring occurrence realization is inconsistent.");
        }

        try
        {
            // One SaveChanges: the tracked occurrence state change and the new charge
            // commit together, so a failure cannot leave a realized occurrence without
            // its charge.
            await dbContext.CreditCardCharges.AddAsync(charge, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return charge;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request realized the same occurrence first. Its rowversion moved,
            // so this write loses and returns the winning charge instead of a second one.
            dbContext.ChangeTracker.Clear();
            var winner = await dbContext.RecurringTransactionOccurrences
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.Id == occurrence.Id && item.UserId == occurrence.UserId,
                    cancellationToken);
            if (winner?.CreditCardChargeId is not Guid winnerChargeId)
            {
                throw;
            }

            return await dbContext.CreditCardCharges
                .AsNoTracking()
                .SingleAsync(
                    item => item.Id == winnerChargeId && item.UserId == occurrence.UserId,
                    cancellationToken);
        }
    }

    private IQueryable<RecurringTransaction> RecurringQuery(bool track)
    {
        IQueryable<RecurringTransaction> query = dbContext.RecurringTransactions;
        return track ? query : query.AsNoTracking();
    }

    private IQueryable<RecurringTransactionOccurrence> OccurrenceQuery(bool track)
    {
        IQueryable<RecurringTransactionOccurrence> query = dbContext.RecurringTransactionOccurrences;
        return track ? query : query.AsNoTracking();
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
