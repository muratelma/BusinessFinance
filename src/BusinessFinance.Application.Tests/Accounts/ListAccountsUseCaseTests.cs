using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Accounts.ListAccounts;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Accounts;

public sealed class ListAccountsUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task ExecuteAsync_WithFilters_RequestsOrderedPageAndMapsDtos()
    {
        var accounts = new[]
        {
            CreateAccount("Bank Account", AccountType.Bank),
            CreateAccount("Daily Cash", AccountType.Cash)
        };
        var repository = new RecordingAccountRepository(new AccountListPage(accounts, 7));
        var useCase = new ListAccountsUseCase(new FakeCurrentUser(UserId), repository);
        var query = new ListAccountsQuery(
            pageNumber: 2,
            pageSize: 2,
            isActive: true,
            type: AccountType.Bank);
        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await useCase.ExecuteAsync(query, cancellationTokenSource.Token);

        Assert.True(result.IsSuccess);
        var response = result.Value;
        Assert.Equal(UserId, repository.ReceivedUserId);
        Assert.Equal(
            new AccountListCriteria(2, 2, true, AccountType.Bank, AccountSortOrder.NameAscending),
            repository.ReceivedCriteria);
        Assert.Equal(cancellationTokenSource.Token, repository.ReceivedCancellationToken);
        Assert.Equal(2, response.PageNumber);
        Assert.Equal(2, response.PageSize);
        Assert.Equal(7, response.TotalCount);
        Assert.Equal(["Bank Account", "Daily Cash"], response.Items.Select(item => item.Name));
        Assert.All(response.Items, item => Assert.Equal(CurrencyCode.TRY, item.Currency));
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryPageIsEmpty_ReturnsEmptyPage()
    {
        var repository = new RecordingAccountRepository(new AccountListPage([], 0));
        var useCase = new ListAccountsUseCase(new FakeCurrentUser(UserId), repository);
        var query = new ListAccountsQuery(pageNumber: 1, pageSize: 20);

        var result = await useCase.ExecuteAsync(query);

        Assert.True(result.IsSuccess);
        var response = result.Value;
        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalCount);
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(20, response.PageSize);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, ListAccountsQuery.MaximumPageSize + 1)]
    public void Constructor_WithInvalidPagination_ThrowsArgumentOutOfRangeException(
        int pageNumber,
        int pageSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ListAccountsQuery(pageNumber, pageSize));
    }

    [Fact]
    public void Constructor_WithUnsupportedAccountType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ListAccountsQuery(1, 20, type: (AccountType)999));
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAuthenticatedUser_ReturnsUnauthorizedWithoutRepositoryCall()
    {
        var repository = new RecordingAccountRepository(new AccountListPage([], 0));
        var useCase = new ListAccountsUseCase(new FakeCurrentUser(null), repository);
        var query = new ListAccountsQuery(1, 20);

        var result = await useCase.ExecuteAsync(query);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.False(repository.ListWasCalled);
    }

    private static Account CreateAccount(string name, AccountType type)
    {
        return new Account(Guid.NewGuid(), UserId, name, type, CurrencyCode.TRY);
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingAccountRepository(AccountListPage page) : IAccountRepository
    {
        public Guid ReceivedUserId { get; private set; }
        public AccountListCriteria? ReceivedCriteria { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }
        public bool ListWasCalled { get; private set; }

        public Task AddAsync(Account account, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<bool> ExistsByNameAsync(
            Guid userId,
            string normalizedName,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<AccountListPage> ListAsync(
            Guid userId,
            AccountListCriteria criteria,
            CancellationToken cancellationToken)
        {
            ListWasCalled = true;
            ReceivedUserId = userId;
            ReceivedCriteria = criteria;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(page);
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
            CancellationToken cancellationToken) => Task.FromResult(0m);
    }
}
