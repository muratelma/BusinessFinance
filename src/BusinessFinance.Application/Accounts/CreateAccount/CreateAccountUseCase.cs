using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts.CreateAccount;

public sealed class CreateAccountUseCase
{
    private readonly ICurrentUser _currentUser;
    private readonly IAccountRepository _accountRepository;

    public CreateAccountUseCase(
        ICurrentUser currentUser,
        IAccountRepository accountRepository)
    {
        _currentUser = currentUser;
        _accountRepository = accountRepository;
    }

    public async Task<ApplicationResult<CreateAccountResponse>> ExecuteAsync(
        CreateAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (_currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CreateAccountResponse>.Failure(
                AccountErrors.AuthenticationRequired);
        }

        Account account;

        try
        {
            account = new Account(
                Guid.NewGuid(),
                userId,
                command.Name,
                command.Type,
                command.Currency,
                command.OpeningBalance,
                command.DefaultScope);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CreateAccountResponse>.Failure(
                AccountErrors.Validation(exception.Message));
        }

        var duplicateExists = await _accountRepository.ExistsByNameAsync(
            userId,
            account.Name,
            cancellationToken);

        if (duplicateExists)
        {
            return ApplicationResult<CreateAccountResponse>.Failure(
                AccountErrors.DuplicateName(account.Name));
        }

        await _accountRepository.AddAsync(account, cancellationToken);

        return ApplicationResult<CreateAccountResponse>.Success(
            new CreateAccountResponse(
                account.Id,
                account.Name,
                account.Type,
                account.Currency,
                account.OpeningBalance,
                account.DefaultScope));
    }
}
