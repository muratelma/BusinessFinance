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

/// <summary>
/// Bir günün bakiye hareketleri, iki yön ayrı: giren ve çıkan.
/// </summary>
/// <remarks>
/// Hesap bakiyesiyle aynı kaynakları okur, yalnız o güne daraltır; bakiye
/// kuralı ikinci bir yerde yeniden yazılmaz. Kasa ekranı beklenen tutarın
/// nereden geldiğini bununla gösterir.
/// </remarks>
public interface IAccountDayFlowReader
{
    Task<(decimal Inflow, decimal Outflow)> CalculateDayFlowAsync(
        Guid accountId,
        Guid userId,
        DateOnly day,
        CancellationToken cancellationToken);
}
