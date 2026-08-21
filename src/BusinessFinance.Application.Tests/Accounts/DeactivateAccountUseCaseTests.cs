using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Accounts.DeactivateAccount;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Accounts;

public sealed class DeactivateAccountUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid AccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task ExecuteAsync_WhenAccountIsOwned_DeactivatesAndUpdatesWithCurrentUser()
    {
        var account = new Account(
            AccountId,
            UserId,
            "Daily Cash",
            AccountType.Cash,
            CurrencyCode.TRY);
        var repository = new RecordingAccountRepository(account);
        var useCase = new DeactivateAccountUseCase(
            new FakeCurrentUser(UserId),
            repository);
        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await useCase.ExecuteAsync(
            new DeactivateAccountCommand(AccountId),
            cancellationTokenSource.Token);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsActive);
        Assert.False(account.IsActive);
        Assert.True(repository.UpdateWasCalled);
        Assert.Equal(UserId, repository.ReceivedUserId);
        Assert.Equal(cancellationTokenSource.Token, repository.ReceivedCancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOwnedAccountIsNotFound_DoesNotUpdate()
    {
        var repository = new RecordingAccountRepository(null);
        var useCase = new DeactivateAccountUseCase(
            new FakeCurrentUser(UserId),
            repository);

        var result = await useCase.ExecuteAsync(
            new DeactivateAccountCommand(AccountId));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.NotFound, result.Error.Type);
        Assert.False(repository.UpdateWasCalled);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAuthenticatedUser_DoesNotUseRepository()
    {
        var repository = new RecordingAccountRepository(null);
        var useCase = new DeactivateAccountUseCase(
            new FakeCurrentUser(null),
            repository);

        var result = await useCase.ExecuteAsync(
            new DeactivateAccountCommand(AccountId));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.False(repository.FindWasCalled);
        Assert.False(repository.UpdateWasCalled);
    }

    [Fact]
    public void Constructor_WithEmptyAccountId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DeactivateAccountCommand(Guid.Empty));
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingAccountRepository(Account? account) : IAccountRepository
    {
        public bool FindWasCalled { get; private set; }
        public bool UpdateWasCalled { get; private set; }
        public Guid ReceivedUserId { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<Account?> FindOwnedByIdAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            FindWasCalled = true;
            ReceivedUserId = userId;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(account);
        }

        public Task UpdateOwnedAsync(
            Account accountToUpdate,
            Guid userId,
            CancellationToken cancellationToken)
        {
            UpdateWasCalled = true;
            ReceivedUserId = userId;
            ReceivedCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }

        public Task AddAsync(Account accountToAdd, CancellationToken cancellationToken) =>
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
            CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
