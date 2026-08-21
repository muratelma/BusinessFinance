using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Transfers;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Transfers;

public sealed class TransferUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task Create_WithOwnedAccounts_PersistsOneTransferEvent()
    {
        var accounts = new FakeAccountRepository(
            CreateAccount(UserId, "Source"),
            CreateAccount(UserId, "Destination"));
        var transfers = new FakeTransferRepository();
        var useCase = new CreateTransferUseCase(
            new FakeCurrentUser(UserId),
            accounts,
            transfers);

        var result = await useCase.ExecuteAsync(new CreateTransferCommand(
            accounts.Items[0].Id,
            accounts.Items[1].Id,
            250m,
            CurrencyCode.TRY,
            new DateOnly(2026, 8, 10),
            "Reserve"));

        Assert.True(result.IsSuccess);
        var persisted = Assert.Single(transfers.Items);
        Assert.Equal(UserId, persisted.UserId);
        Assert.Equal(accounts.Items[0].Id, persisted.SourceAccountId);
        Assert.Equal(accounts.Items[1].Id, persisted.DestinationAccountId);
    }

    [Fact]
    public async Task Create_WithForeignAccount_ReturnsValidationWithoutPersisting()
    {
        var owned = CreateAccount(UserId, "Owned");
        var foreign = CreateAccount(Guid.NewGuid(), "Foreign");
        var accounts = new FakeAccountRepository(owned, foreign);
        var transfers = new FakeTransferRepository();
        var useCase = new CreateTransferUseCase(
            new FakeCurrentUser(UserId),
            accounts,
            transfers);

        var result = await useCase.ExecuteAsync(new CreateTransferCommand(
            owned.Id,
            foreign.Id,
            10m,
            CurrencyCode.TRY,
            new DateOnly(2026, 8, 10),
            null));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Empty(transfers.Items);
    }

    [Fact]
    public async Task Create_WithoutUser_ReturnsUnauthorizedWithoutAccountLookup()
    {
        var accounts = new FakeAccountRepository();
        var transfers = new FakeTransferRepository();
        var useCase = new CreateTransferUseCase(
            new FakeCurrentUser(null),
            accounts,
            transfers);

        var result = await useCase.ExecuteAsync(new CreateTransferCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10m,
            CurrencyCode.TRY,
            new DateOnly(2026, 8, 10),
            null));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Equal(0, accounts.FindCalls);
    }

    [Fact]
    public async Task Get_ForeignTransfer_ReturnsNotFound()
    {
        var foreignUser = Guid.NewGuid();
        var transfer = new Transfer(
            Guid.NewGuid(),
            foreignUser,
            CreateAccount(foreignUser, "Source"),
            CreateAccount(foreignUser, "Destination"),
            new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10));
        var repository = new FakeTransferRepository(transfer);

        var result = await new GetTransferUseCase(
            new FakeCurrentUser(UserId),
            repository).ExecuteAsync(transfer.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task Cancel_IsIdempotentAndUsesUtcClock()
    {
        var source = CreateAccount(UserId, "Source");
        var destination = CreateAccount(UserId, "Destination");
        var transfer = new Transfer(
            Guid.NewGuid(),
            UserId,
            source,
            destination,
            new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10));
        var repository = new FakeTransferRepository(transfer);
        var now = new DateTimeOffset(2026, 8, 10, 14, 0, 0, TimeSpan.Zero);
        var useCase = new CancelTransferUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FixedTimeProvider(now));

        var first = await useCase.ExecuteAsync(new CancelTransferCommand(transfer.Id));
        var second = await useCase.ExecuteAsync(new CancelTransferCommand(transfer.Id));

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(now, transfer.CancelledAtUtc);
        Assert.Equal(2, repository.UpdateCalls);
    }

    private static Account CreateAccount(Guid userId, string name) => new(
        Guid.NewGuid(), userId, name, AccountType.Bank, CurrencyCode.TRY);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FakeAccountRepository(params Account[] accounts) : IAccountRepository
    {
        public IReadOnlyList<Account> Items { get; } = accounts;
        public int FindCalls { get; private set; }

        public Task<Account?> FindOwnedByIdAsync(Guid accountId, Guid userId, CancellationToken cancellationToken)
        {
            FindCalls++;
            return Task.FromResult(Items.SingleOrDefault(item => item.Id == accountId && item.UserId == userId));
        }

        public Task AddAsync(Account account, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<bool> ExistsByNameAsync(Guid userId, string normalizedName, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<AccountListPage> ListAsync(Guid userId, AccountListCriteria criteria, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(Account account, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<decimal> CalculateBalanceAsync(Guid accountId, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FakeTransferRepository(params Transfer[] transfers) : ITransferRepository
    {
        public List<Transfer> Items { get; } = [.. transfers];
        public int UpdateCalls { get; private set; }

        public Task AddAsync(Transfer transfer, CancellationToken cancellationToken)
        {
            Items.Add(transfer);
            return Task.CompletedTask;
        }

        public Task<Transfer?> FindOwnedByIdAsync(Guid transferId, Guid userId, bool track, CancellationToken cancellationToken) =>
            Task.FromResult(Items.SingleOrDefault(item => item.Id == transferId && item.UserId == userId));

        public Task<BoundedList<Transfer>> ListAsync(
            Guid userId,
            HistoryWindow window,
            CancellationToken cancellationToken) =>
            Task.FromResult(new BoundedList<Transfer>(
                Items.Where(item => item.UserId == userId && window.Contains(item.TransferDate)).ToArray(),
                false));

        public Task UpdateOwnedAsync(Transfer transfer, Guid userId, CancellationToken cancellationToken)
        {
            UpdateCalls++;
            return Task.CompletedTask;
        }
    }
}
