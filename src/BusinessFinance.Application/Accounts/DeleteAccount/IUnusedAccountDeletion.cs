namespace BusinessFinance.Application.Accounts.DeleteAccount;

public enum UnusedAccountDeletionResult
{
    Deleted,
    NotFound,
    InUse
}

public interface IUnusedAccountDeletion
{
    Task<UnusedAccountDeletionResult> DeleteOwnedIfUnusedAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken);
}
