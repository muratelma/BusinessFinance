using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Accounts.DeleteAccount;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Accounts;

internal sealed class EfUnusedAccountDeletion(BusinessFinanceDbContext dbContext)
    : IUnusedAccountDeletion
{
    public async Task<UnusedAccountDeletionResult> DeleteOwnedIfUnusedAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts.SingleOrDefaultAsync(
            candidate => candidate.Id == accountId && candidate.UserId == userId,
            cancellationToken);
        if (account is null)
        {
            return UnusedAccountDeletionResult.NotFound;
        }

        if (await HasFinancialReferencesAsync(accountId, userId, cancellationToken))
        {
            return UnusedAccountDeletionResult.InUse;
        }

        dbContext.Accounts.Remove(account);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return UnusedAccountDeletionResult.Deleted;
        }
        catch (DbUpdateException exception) when (IsForeignKeyViolation(exception))
        {
            dbContext.Entry(account).State = EntityState.Unchanged;
            return UnusedAccountDeletionResult.InUse;
        }
    }

    private async Task<bool> HasFinancialReferencesAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Transactions.AnyAsync(
                   item => item.UserId == userId && item.AccountId == accountId,
                   cancellationToken) ||
               await dbContext.Transfers.AnyAsync(
                   item => item.UserId == userId &&
                           (item.SourceAccountId == accountId ||
                            item.DestinationAccountId == accountId),
                   cancellationToken) ||
               await dbContext.CreditCardPayments.AnyAsync(
                   item => item.UserId == userId && item.AccountId == accountId,
                   cancellationToken) ||
               await dbContext.RecurringTransactions.AnyAsync(
                   item => item.UserId == userId && item.AccountId == accountId,
                   cancellationToken) ||
               await dbContext.RecurringTransactionOccurrences.AnyAsync(
                   item => item.UserId == userId && item.AccountId == accountId,
                   cancellationToken) ||
               await dbContext.ImportRows.AnyAsync(
                   item => item.UserId == userId && item.AccountId == accountId,
                   cancellationToken) ||
               await dbContext.DebtInstallments.AnyAsync(
                   item => item.UserId == userId && item.PaymentAccountId == accountId,
                   cancellationToken) ||
               await dbContext.SavingsGoals.AnyAsync(
                   item => item.UserId == userId && item.AccountId == accountId,
                   cancellationToken);
    }

    private static bool IsForeignKeyViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqlException { Number: 547 };
    }
}
