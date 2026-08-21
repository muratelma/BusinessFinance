using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Accounts.ListAccounts;

public sealed class ListAccountsUseCase
{
    private readonly ICurrentUser _currentUser;
    private readonly IAccountRepository _accountRepository;

    public ListAccountsUseCase(
        ICurrentUser currentUser,
        IAccountRepository accountRepository)
    {
        _currentUser = currentUser;
        _accountRepository = accountRepository;
    }

    public async Task<ApplicationResult<ListAccountsResponse>> ExecuteAsync(
        ListAccountsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (_currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<ListAccountsResponse>.Failure(
                AccountErrors.AuthenticationRequired);
        }

        var criteria = new AccountListCriteria(
            query.PageNumber,
            query.PageSize,
            query.IsActive,
            query.Type,
            AccountSortOrder.NameAscending);

        var page = await _accountRepository.ListAsync(
            userId,
            criteria,
            cancellationToken);

        var items = new List<AccountListItemDto>(page.Items.Count);
        foreach (var account in page.Items)
        {
            var balance = await _accountRepository.CalculateBalanceAsync(
                account.Id,
                userId,
                cancellationToken);
            items.Add(new AccountListItemDto(
                account.Id,
                account.Name,
                account.Type,
                account.Currency,
                account.IsActive,
                account.OpeningBalance,
                balance));
        }

        return ApplicationResult<ListAccountsResponse>.Success(
            new ListAccountsResponse(
                items,
                query.PageNumber,
                query.PageSize,
                page.TotalCount));
    }
}
