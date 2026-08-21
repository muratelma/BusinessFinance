using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Accounts.GetAccount;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Accounts;

public sealed class GetAccountUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid AccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task ExecuteAsync_WhenAccountIsOwned_ReturnsDetailsAndForwardsCurrentUser()
    {
        var account = new Account(
            AccountId,
            UserId,
            "Daily Cash",
            AccountType.Cash,
            CurrencyCode.TRY);
        var repository = new RecordingAccountRepository(account);
        var useCase = new GetAccountUseCase(new FakeCurrentUser(UserId), repository);
        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await useCase.ExecuteAsync(
            new GetAccountQuery(AccountId),
            cancellationTokenSource.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(AccountId, result.Value.Id);
        Assert.Equal("Daily Cash", result.Value.Name);
        Assert.Equal(UserId, repository.ReceivedUserId);
        Assert.Equal(AccountId, repository.ReceivedAccountId);
        Assert.Equal(cancellationTokenSource.Token, repository.ReceivedCancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOwnedAccountIsNotFound_ReturnsNotFound()
    {
        var repository = new RecordingAccountRepository(null);
        var useCase = new GetAccountUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new GetAccountQuery(AccountId));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.NotFound, result.Error.Type);
        Assert.Equal("accounts.not_found", result.Error.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAuthenticatedUser_DoesNotQueryRepository()
    {
        var repository = new RecordingAccountRepository(null);
        var useCase = new GetAccountUseCase(new FakeCurrentUser(null), repository);

        var result = await useCase.ExecuteAsync(new GetAccountQuery(AccountId));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.False(repository.FindWasCalled);
    }

    [Fact]
    public void Constructor_WithEmptyAccountId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new GetAccountQuery(Guid.Empty));
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingAccountRepository(Account? account) : IAccountRepository
    {
        public bool FindWasCalled { get; private set; }
        public Guid ReceivedAccountId { get; private set; }
        public Guid ReceivedUserId { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<Account?> FindOwnedByIdAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            FindWasCalled = true;
            ReceivedAccountId = accountId;
            ReceivedUserId = userId;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(account);
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

        public Task UpdateOwnedAsync(
            Account account,
            Guid userId,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<decimal> CalculateBalanceAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken) => Task.FromResult(0m);
    }
}
