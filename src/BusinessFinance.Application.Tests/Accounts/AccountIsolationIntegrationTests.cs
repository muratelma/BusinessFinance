using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Accounts.DeactivateAccount;
using BusinessFinance.Application.Accounts.GetAccount;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Accounts;

public sealed class AccountIsolationIntegrationTests
{
    private static readonly Guid UserA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid UserB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid AccountA = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AccountB = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task UserA_CanReadOwnAccount_ButCannotReadUserBsAccount()
    {
        var repository = CreateRepository();
        var useCase = new GetAccountUseCase(new FakeCurrentUser(UserA), repository);

        var ownResult = await useCase.ExecuteAsync(new GetAccountQuery(AccountA));
        var otherResult = await useCase.ExecuteAsync(new GetAccountQuery(AccountB));

        Assert.True(ownResult.IsSuccess);
        Assert.Equal("User A Cash", ownResult.Value.Name);
        Assert.False(otherResult.IsSuccess);
        Assert.Equal(ApplicationErrorType.NotFound, otherResult.Error.Type);
    }

    [Fact]
    public async Task UserA_CannotDeactivateUserBsAccount()
    {
        var repository = CreateRepository();
        var useCase = new DeactivateAccountUseCase(
            new FakeCurrentUser(UserA),
            repository);

        var result = await useCase.ExecuteAsync(
            new DeactivateAccountCommand(AccountB));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.NotFound, result.Error.Type);
        Assert.True(repository.Accounts.Single(account => account.Id == AccountB).IsActive);
        Assert.Equal(0, repository.UpdateCount);
    }

    [Fact]
    public async Task UserA_DeactivatingOwnAccount_DoesNotChangeUserBsAccount()
    {
        var repository = CreateRepository();
        var useCase = new DeactivateAccountUseCase(
            new FakeCurrentUser(UserA),
            repository);

        var result = await useCase.ExecuteAsync(
            new DeactivateAccountCommand(AccountA));

        Assert.True(result.IsSuccess);
        Assert.False(repository.Accounts.Single(account => account.Id == AccountA).IsActive);
        Assert.True(repository.Accounts.Single(account => account.Id == AccountB).IsActive);
        Assert.Equal(1, repository.UpdateCount);
    }

    [Fact]
    public void AccountOwnershipInputs_ExposeOnlyAccountIdWithoutUserOrAdminBypass()
    {
        var queryProperties = typeof(GetAccountQuery).GetProperties();
        var commandProperties = typeof(DeactivateAccountCommand).GetProperties();

        Assert.Equal(["AccountId"], queryProperties.Select(property => property.Name));
        Assert.Equal(["AccountId"], commandProperties.Select(property => property.Name));
        Assert.DoesNotContain(
            queryProperties.Concat(commandProperties),
            property => property.Name.Contains("Admin", StringComparison.OrdinalIgnoreCase) ||
                        property.Name.Contains("User", StringComparison.OrdinalIgnoreCase));
    }

    private static InMemoryOwnedAccountRepository CreateRepository()
    {
        return new InMemoryOwnedAccountRepository(
        [
            new Account(AccountA, UserA, "User A Cash", AccountType.Cash, CurrencyCode.TRY),
            new Account(AccountB, UserB, "User B Bank", AccountType.Bank, CurrencyCode.TRY)
        ]);
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class InMemoryOwnedAccountRepository(IEnumerable<Account> accounts)
        : IAccountRepository
    {
        public List<Account> Accounts { get; } = [.. accounts];
        public int UpdateCount { get; private set; }

        public Task<Account?> FindOwnedByIdAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(
                Accounts.SingleOrDefault(
                    account => account.Id == accountId && account.UserId == userId));
        }

        public Task UpdateOwnedAsync(
            Account account,
            Guid userId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (account.UserId != userId ||
                !Accounts.Any(stored => stored.Id == account.Id && stored.UserId == userId))
            {
                throw new InvalidOperationException("Owned account update scope was violated.");
            }

            UpdateCount++;
            return Task.CompletedTask;
        }

        public Task AddAsync(Account account, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsByNameAsync(
            Guid userId,
            string normalizedName,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<AccountListPage> ListAsync(
            Guid userId,
            AccountListCriteria criteria,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<decimal> CalculateBalanceAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken) => Task.FromResult(0m);
    }
}
