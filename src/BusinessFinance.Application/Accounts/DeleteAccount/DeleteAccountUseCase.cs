using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Accounts.DeleteAccount;

public sealed class DeleteAccountUseCase(
    ICurrentUser currentUser,
    IUnusedAccountDeletion accountDeletion)
{
    public async Task<ApplicationResult<DeleteAccountResponse>> ExecuteAsync(
        DeleteAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DeleteAccountResponse>.Failure(
                AccountErrors.AuthenticationRequired);
        }

        var result = await accountDeletion.DeleteOwnedIfUnusedAsync(
            command.AccountId,
            userId,
            cancellationToken);

        return result switch
        {
            UnusedAccountDeletionResult.Deleted =>
                ApplicationResult<DeleteAccountResponse>.Success(
                    new DeleteAccountResponse(command.AccountId)),
            UnusedAccountDeletionResult.NotFound =>
                ApplicationResult<DeleteAccountResponse>.Failure(
                    AccountErrors.NotFound(command.AccountId)),
            UnusedAccountDeletionResult.InUse =>
                ApplicationResult<DeleteAccountResponse>.Failure(AccountErrors.InUse),
            _ => throw new InvalidOperationException("Unknown account deletion result.")
        };
    }
}
