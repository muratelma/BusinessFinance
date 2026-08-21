using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Accounts.DeactivateAccount;

public sealed class DeactivateAccountUseCase
{
    private readonly ICurrentUser _currentUser;
    private readonly IAccountRepository _accountRepository;

    public DeactivateAccountUseCase(
        ICurrentUser currentUser,
        IAccountRepository accountRepository)
    {
        _currentUser = currentUser;
        _accountRepository = accountRepository;
    }

    public async Task<ApplicationResult<DeactivateAccountResponse>> ExecuteAsync(
        DeactivateAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (_currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DeactivateAccountResponse>.Failure(
                AccountErrors.AuthenticationRequired);
        }

        var account = await _accountRepository.FindOwnedByIdAsync(
            command.AccountId,
            userId,
            cancellationToken);

        if (account is null)
        {
            return ApplicationResult<DeactivateAccountResponse>.Failure(
                AccountErrors.NotFound(command.AccountId));
        }

        account.Deactivate();
        await _accountRepository.UpdateOwnedAsync(
            account,
            userId,
            cancellationToken);

        return ApplicationResult<DeactivateAccountResponse>.Success(
            new DeactivateAccountResponse(account.Id, account.IsActive));
    }
}
