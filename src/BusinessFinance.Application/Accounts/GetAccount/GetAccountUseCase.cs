using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Accounts.GetAccount;

public sealed class GetAccountUseCase
{
    private readonly ICurrentUser _currentUser;
    private readonly IAccountRepository _accountRepository;

    public GetAccountUseCase(
        ICurrentUser currentUser,
        IAccountRepository accountRepository)
    {
        _currentUser = currentUser;
        _accountRepository = accountRepository;
    }

    public async Task<ApplicationResult<GetAccountResponse>> ExecuteAsync(
        GetAccountQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (_currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<GetAccountResponse>.Failure(
                AccountErrors.AuthenticationRequired);
        }

        var account = await _accountRepository.FindOwnedByIdAsync(
            query.AccountId,
            userId,
            cancellationToken);

        if (account is null)
        {
            return ApplicationResult<GetAccountResponse>.Failure(
                AccountErrors.NotFound(query.AccountId));
        }

        var balance = await _accountRepository.CalculateBalanceAsync(
            account.Id,
            userId,
            cancellationToken);

        return ApplicationResult<GetAccountResponse>.Success(
            new GetAccountResponse(
                account.Id,
                account.Name,
                account.Type,
                account.Currency,
                account.IsActive,
                account.OpeningBalance,
                balance));
    }
}
