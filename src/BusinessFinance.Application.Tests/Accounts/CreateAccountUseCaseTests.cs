using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Accounts.CreateAccount;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Accounts;

public sealed class CreateAccountUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task ExecuteAsync_WithValidCommand_CreatesAndPersistsCurrentUsersAccount()
    {
        var repository = new RecordingAccountRepository();
        var useCase = new CreateAccountUseCase(new FakeCurrentUser(UserId), repository);
        var command = new CreateAccountCommand(
            "  Daily Cash  ",
            AccountType.Cash,
            CurrencyCode.TRY);

        var result = await useCase.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        var response = result.Value;
        var persistedAccount = Assert.Single(repository.AddedAccounts);
        Assert.NotEqual(Guid.Empty, persistedAccount.Id);
        Assert.Equal(UserId, persistedAccount.UserId);
        Assert.Equal("Daily Cash", persistedAccount.Name);
        Assert.Equal(persistedAccount.Id, response.Id);
        Assert.Equal(persistedAccount.Name, response.Name);
        Assert.Equal(AccountType.Cash, response.Type);
        Assert.Equal(CurrencyCode.TRY, response.Currency);
    }

    [Fact]
    public async Task ExecuteAsync_ForwardsCancellationTokenToRepository()
    {
        var repository = new RecordingAccountRepository();
        var useCase = new CreateAccountUseCase(new FakeCurrentUser(UserId), repository);
        var command = new CreateAccountCommand(
            "Bank Account",
            AccountType.Bank,
            CurrencyCode.TRY);
        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await useCase.ExecuteAsync(command, cancellationTokenSource.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(cancellationTokenSource.Token, repository.ReceivedCancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidName_DoesNotPersistAccount()
    {
        var repository = new RecordingAccountRepository();
        var useCase = new CreateAccountUseCase(new FakeCurrentUser(UserId), repository);
        var command = new CreateAccountCommand(
            "   ",
            AccountType.Cash,
            CurrencyCode.TRY);

        var result = await useCase.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Equal("accounts.validation", result.Error.Code);
        Assert.Empty(repository.AddedAccounts);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNameAlreadyExists_ReturnsConflictWithoutPersisting()
    {
        var repository = new RecordingAccountRepository { DuplicateExists = true };
        var useCase = new CreateAccountUseCase(new FakeCurrentUser(UserId), repository);
        var command = new CreateAccountCommand(
            "  Daily Cash  ",
            AccountType.Cash,
            CurrencyCode.TRY);

        var result = await useCase.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Equal("accounts.duplicate_name", result.Error.Code);
        Assert.Equal("Daily Cash", repository.ReceivedNormalizedName);
        Assert.Empty(repository.AddedAccounts);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAuthenticatedUser_ReturnsUnauthorizedWithoutRepositoryCall()
    {
        var repository = new RecordingAccountRepository();
        var useCase = new CreateAccountUseCase(new FakeCurrentUser(null), repository);
        var command = new CreateAccountCommand(
            "Daily Cash",
            AccountType.Cash,
            CurrencyCode.TRY);

        var result = await useCase.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Equal("authentication.required", result.Error.Code);
        Assert.False(repository.ExistsByNameWasCalled);
        Assert.Empty(repository.AddedAccounts);
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingAccountRepository : IAccountRepository
    {
        public List<Account> AddedAccounts { get; } = [];
        public bool DuplicateExists { get; init; }
        public bool ExistsByNameWasCalled { get; private set; }
        public string? ReceivedNormalizedName { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task AddAsync(Account account, CancellationToken cancellationToken)
        {
            AddedAccounts.Add(account);
            ReceivedCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNameAsync(
            Guid userId,
            string normalizedName,
            CancellationToken cancellationToken)
        {
            ExistsByNameWasCalled = true;
            ReceivedNormalizedName = normalizedName;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(DuplicateExists);
        }

        public Task<AccountListPage> ListAsync(
            Guid userId,
            AccountListCriteria criteria,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Account?> FindOwnedByIdAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task UpdateOwnedAsync(
            Account account,
            Guid userId,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<decimal> CalculateBalanceAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
