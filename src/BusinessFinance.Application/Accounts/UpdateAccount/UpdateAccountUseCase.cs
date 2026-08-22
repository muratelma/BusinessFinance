using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Accounts.UpdateAccount;

public sealed class UpdateAccountUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository)
{
    public async Task<ApplicationResult<UpdateAccountResponse>> ExecuteAsync(
        UpdateAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<UpdateAccountResponse>.Failure(
                AccountErrors.AuthenticationRequired);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId,
            userId,
            cancellationToken);
        if (account is null)
        {
            return ApplicationResult<UpdateAccountResponse>.Failure(
                AccountErrors.NotFound(command.AccountId));
        }

        try
        {
            var previousName = account.Name;
            account.Rename(command.Name);
            if (!string.Equals(previousName, account.Name, StringComparison.OrdinalIgnoreCase) &&
                await accountRepository.ExistsByNameAsync(userId, account.Name, cancellationToken))
            {
                return ApplicationResult<UpdateAccountResponse>.Failure(
                    AccountErrors.DuplicateName(account.Name));
            }

            account.SetDefaultScope(command.DefaultScope);

            if (command.IsActive)
            {
                account.Activate();
            }
            else
            {
                account.Deactivate();
            }
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<UpdateAccountResponse>.Failure(
                AccountErrors.Validation(exception.Message));
        }

        await accountRepository.UpdateOwnedAsync(account, userId, cancellationToken);

        return ApplicationResult<UpdateAccountResponse>.Success(
            new UpdateAccountResponse(
                account.Id, account.Name, account.IsActive, account.DefaultScope));
    }
}
