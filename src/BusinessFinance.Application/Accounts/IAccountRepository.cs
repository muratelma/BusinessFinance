using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(
        Guid userId,
        string normalizedName,
        CancellationToken cancellationToken);

    Task<AccountListPage> ListAsync(
        Guid userId,
        AccountListCriteria criteria,
        CancellationToken cancellationToken);

    Task<Account?> FindOwnedByIdAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken);

    Task UpdateOwnedAsync(
        Account account,
        Guid userId,
        CancellationToken cancellationToken);

    Task<decimal> CalculateBalanceAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken);
}
