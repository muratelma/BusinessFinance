using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Imports;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Imports;

public sealed class ImportConfirmationUseCaseTests
{
    [Fact]
    public async Task Confirm_RejectsMixedSelectionBeforeRepositoryThenImportsReadySubset()
    {
        var userId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), userId, "Import", AccountType.Bank, CurrencyCode.TRY,
            defaultScope: TransactionScope.Business);
        var category = new Category(Guid.NewGuid(), userId, "Expense", CategoryType.Expense);
        var batch = new ImportBatch(
            Guid.NewGuid(), userId, "test.csv", new string('a', 64), 100, "utf-8", ',',
            "Date", "Amount", null, null, "yyyy-MM-dd", '.',
            new DateTimeOffset(2026, 8, 11, 9, 0, 0, TimeSpan.Zero));
        var ready = new ImportRow(
            Guid.NewGuid(), userId, batch.Id, 2, "ready",
            new DateOnly(2026, 8, 11), -10m, CurrencyCode.TRY, null, null, []);
        ready.ApplyCorrection(
            new DateOnly(2026, 8, 11), -10m, "Corrected", null, account, category);
        var unmapped = new ImportRow(
            Guid.NewGuid(), userId, batch.Id, 3, "unmapped",
            new DateOnly(2026, 8, 12), -20m, CurrencyCode.TRY, null, null, []);
        batch.AddRow(ready);
        batch.AddRow(unmapped);
        var repository = new FakeImportRepository(batch);
        var useCase = new ConfirmImportBatchUseCase(
            new FakeCurrentUser(userId), repository,
            new FakeAccountRepository(account), new FakeCategoryRepository(category));

        var rejected = await useCase.ExecuteAsync(
            new ConfirmImportBatchCommand(batch.Id, [ready.Id, unmapped.Id]));
        Assert.False(rejected.IsSuccess);
        Assert.Equal(0, repository.ConfirmCalls);
        Assert.Equal(ImportRowStatus.Ready, ready.Status);

        var confirmed = await useCase.ExecuteAsync(
            new ConfirmImportBatchCommand(batch.Id, [ready.Id]));
        Assert.True(confirmed.IsSuccess);
        Assert.Equal(1, repository.ConfirmCalls);
        var transaction = Assert.Single(repository.Transactions);
        Assert.Equal(TransactionType.Expense, transaction.Type);
        Assert.Equal(TransactionScope.Business, transaction.Scope);
        Assert.Equal(10m, transaction.Amount.Amount);
        Assert.Equal(ImportBatchStatus.PartiallyImported, batch.Status);
    }

    [Fact]
    public async Task Confirm_ConcurrencyLoss_ReturnsConflict()
    {
        var userId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), userId, "Race", AccountType.Bank, CurrencyCode.TRY,
            defaultScope: TransactionScope.Business);
        var category = new Category(Guid.NewGuid(), userId, "Race expense", CategoryType.Expense);
        var batch = new ImportBatch(
            Guid.NewGuid(), userId, "race.csv", new string('b', 64), 50, "utf-8", ',',
            "Date", "Amount", null, null, "yyyy-MM-dd", '.',
            new DateTimeOffset(2026, 8, 11, 9, 0, 0, TimeSpan.Zero));
        var row = new ImportRow(
            Guid.NewGuid(), userId, batch.Id, 2, "race",
            new DateOnly(2026, 8, 11), -5m, CurrencyCode.TRY, null, null, []);
        row.ApplyCorrection(new DateOnly(2026, 8, 11), -5m, null, null, account, category);
        batch.AddRow(row);
        var repository = new FakeImportRepository(batch) { ThrowConcurrency = true };
        var useCase = new ConfirmImportBatchUseCase(
            new FakeCurrentUser(userId), repository,
            new FakeAccountRepository(account), new FakeCategoryRepository(category));

        var result = await useCase.ExecuteAsync(new ConfirmImportBatchCommand(batch.Id, [row.Id]));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task Confirm_SelectionAboveLimit_IsRejectedBeforeRepositoryLookup()
    {
        var userId = Guid.NewGuid();
        var batch = new ImportBatch(
            Guid.NewGuid(), userId, "limit.csv", new string('c', 64), 50, "utf-8", ',',
            "Date", "Amount", null, null, "yyyy-MM-dd", '.',
            new DateTimeOffset(2026, 8, 11, 9, 0, 0, TimeSpan.Zero));
        var repository = new FakeImportRepository(batch);
        var useCase = new ConfirmImportBatchUseCase(
            new FakeCurrentUser(userId), repository,
            new FakeAccountRepository(null), new FakeCategoryRepository(null));

        var result = await useCase.ExecuteAsync(new ConfirmImportBatchCommand(
            batch.Id,
            Enumerable.Range(0, ConfirmImportBatchUseCase.MaximumConfirmationRows + 1)
                .Select(_ => Guid.NewGuid()).ToArray()));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Equal(0, repository.FindCalls);
    }

    private sealed class FakeCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakeImportRepository(ImportBatch batch) : IImportBatchRepository
    {
        public int ConfirmCalls { get; private set; }
        public int FindCalls { get; private set; }
        public bool ThrowConcurrency { get; init; }
        public IReadOnlyCollection<BudgetTransaction> Transactions { get; private set; } = [];
        public Task AddAsync(ImportBatch value, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<ImportBatch> AddOrGetExistingAsync(ImportBatch value, CancellationToken cancellationToken) =>
            Task.FromResult(value);
        public Task<ImportBatch?> FindOwnedByIdAsync(
            Guid batchId, Guid userId, bool track, CancellationToken cancellationToken)
        {
            FindCalls++;
            return Task.FromResult(batch.Id == batchId && batch.UserId == userId ? batch : null);
        }

        public Task UpdateAsync(ImportBatch value, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<DuplicateMatch?> FindDuplicateAsync(
            Guid userId, string? externalReference, DateOnly transactionDate,
            TransactionType type, decimal amount, string? description,
            CancellationToken cancellationToken) => Task.FromResult<DuplicateMatch?>(null);

        public Task ConfirmAsync(
            ImportBatch value,
            IReadOnlyCollection<BudgetTransaction> transactions,
            CancellationToken cancellationToken)
        {
            ConfirmCalls++;
            if (ThrowConcurrency) throw new ImportConcurrencyException();
            Transactions = transactions;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAccountRepository(Account? account) : IAccountRepository
    {
        public Task<Account?> FindOwnedByIdAsync(Guid accountId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(account?.Id == accountId && account.UserId == userId ? account : null);
        public Task AddAsync(Account value, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<bool> ExistsByNameAsync(Guid userId, string normalizedName, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<AccountListPage> ListAsync(Guid userId, AccountListCriteria criteria, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(Account value, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<decimal> CalculateBalanceAsync(Guid accountId, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FakeCategoryRepository(Category? category) : ICategoryRepository
    {
        public Task<Category?> FindOwnedByIdAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(category?.Id == categoryId && category.UserId == userId ? category : null);
        public Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task AddAsync(Category value, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<bool> ExistsByNameAndTypeAsync(Guid userId, string name, CategoryType type, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<IReadOnlyList<Category>> ListAsync(Guid userId, CategoryType? type, bool? isActive, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(Category value, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
