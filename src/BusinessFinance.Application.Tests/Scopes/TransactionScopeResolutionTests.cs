using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Scopes;

/// <summary>
/// Kapsam türetme zinciri: kullanıcının açık seçimi → hesabın/kartın etiketi →
/// kategorinin varsayılanı. Üçü de boşsa istek reddedilir; sunucu kapsam
/// uydurmaz.
/// </summary>
public sealed class TransactionScopeResolutionTests
{
    [Fact]
    public void Resolve_PrefersTheExplicitChoiceOverEveryDefault()
    {
        var resolved = TransactionScopeResolution.Resolve(
            TransactionScope.Personal,
            TransactionScope.Business,
            TransactionScope.Business);

        Assert.Equal(TransactionScope.Personal, resolved);
    }

    /// <summary>
    /// Vergide kapsam açık seçimden, yoksa profilin tarafından gelir; ödeme
    /// kaynağının etiketi bakılmaz (ADR 0018 İ9, 30 Eylül 2026). Sonuç hiç boş
    /// değildir.
    /// </summary>
    [Theory]
    [InlineData(null, true, TransactionScope.Business)]
    [InlineData(null, false, TransactionScope.Personal)]
    [InlineData(TransactionScope.Personal, true, TransactionScope.Personal)]
    [InlineData(TransactionScope.Business, false, TransactionScope.Business)]
    public void ResolveTax_TakesTheChoiceOrTheProfileSide(
        TransactionScope? requested,
        bool hasBusiness,
        TransactionScope expected)
    {
        Assert.Equal(expected, TransactionScopeResolution.ResolveTax(requested, hasBusiness));
    }

    [Fact]
    public void Resolve_PrefersTheSourceOverTheCategory()
    {
        var resolved = TransactionScopeResolution.Resolve(
            null,
            TransactionScope.Business,
            TransactionScope.Personal);

        Assert.Equal(TransactionScope.Business, resolved);
    }

    [Fact]
    public void Resolve_FallsBackToTheCategory()
    {
        var resolved = TransactionScopeResolution.Resolve(
            null,
            null,
            TransactionScope.Personal);

        Assert.Equal(TransactionScope.Personal, resolved);
    }

    [Fact]
    public void Resolve_WithNothingToGoOn_ReturnsNull()
    {
        Assert.Null(TransactionScopeResolution.Resolve(null, null, null));
    }

    [Fact]
    public async Task CreateTransaction_WithoutScope_TakesTheAccountLabel()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, TransactionScope.Business);
        var category = CreateCategory(userId, TransactionScope.Personal);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, scope: null));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Business, result.Value.Scope);
        Assert.Equal(TransactionScope.Business, Assert.Single(repository.Saved).Scope);
    }

    [Fact]
    public async Task CreateTransaction_WithoutScopeOrAccountLabel_TakesTheCategoryDefault()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, null);
        var category = CreateCategory(userId, TransactionScope.Personal);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, scope: null));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Personal, result.Value.Scope);
    }

    [Fact]
    public async Task CreateTransaction_WithAnExplicitScope_OverridesBothDefaults()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, TransactionScope.Business);
        var category = CreateCategory(userId, TransactionScope.Business);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, TransactionScope.Personal));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Personal, result.Value.Scope);
    }

    /// <summary>
    /// Hiçbir halka dolmadığında kayıt <b>oluşmaz</b>. Bir değer seçmek, yanlış
    /// etiketlenmiş bir hareketi kullanıcı fark edene kadar işletme netinin
    /// içinde bırakmak olurdu.
    /// </summary>
    [Fact]
    public async Task CreateTransaction_WithNothingToGoOn_IsRejectedAndWritesNothing()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, null);
        var category = CreateCategory(userId, null);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, scope: null));

        Assert.False(result.IsSuccess);
        Assert.Equal("transactions.scope_unresolved", result.Error.Code);
        Assert.Empty(repository.Saved);
    }

    /// <summary>
    /// Bütçenin hesabı yoktur; zincir açık seçim ve kategori ile sınırlıdır.
    /// </summary>
    [Fact]
    public async Task CreateBudget_WithoutScope_TakesTheCategoryDefaultOrIsRejected()
    {
        var userId = Guid.NewGuid();
        var labelled = CreateCategory(userId, TransactionScope.Business);
        var unlabelled = CreateCategory(userId, null);

        var resolved = await CreateBudgetUseCase(userId, labelled)
            .ExecuteAsync(BudgetCommand(labelled, scope: null));
        var rejected = await CreateBudgetUseCase(userId, unlabelled)
            .ExecuteAsync(BudgetCommand(unlabelled, scope: null));

        Assert.True(resolved.IsSuccess);
        Assert.Equal(TransactionScope.Business, resolved.Value.Scope);
        Assert.False(rejected.IsSuccess);
        Assert.Equal("budgets.scope_unresolved", rejected.Error.Code);
    }

    private static CreateTransactionUseCase CreateTransactionUseCase(
        Guid userId,
        Account account,
        Category category,
        ITransactionRepository repository)
    {
        return new CreateTransactionUseCase(
            new FakeCurrentUser(userId),
            new FakeAccountRepository(account),
            new FakeCategoryRepository(category),
            repository);
    }

    private static CreateBudgetUseCase CreateBudgetUseCase(Guid userId, Category category)
    {
        return new CreateBudgetUseCase(
            new FakeCurrentUser(userId),
            new FakeCategoryRepository(category),
            new FakeBudgetRepository());
    }

    private static CreateTransactionCommand Command(
        Account account,
        Category category,
        TransactionScope? scope)
    {
        return new CreateTransactionCommand(
            account.Id,
            category.Id,
            100m,
            CurrencyCode.TRY,
            TransactionType.Expense,
            scope,
            new DateOnly(2026, 8, 7),
            "Sentetik gider");
    }

    private static CreateBudgetCommand BudgetCommand(Category category, TransactionScope? scope) =>
        new(category.Id, 1000m, CurrencyCode.TRY, scope, 2026, 8);

    private static Account CreateAccount(Guid userId, TransactionScope? defaultScope) => new(
        Guid.NewGuid(),
        userId,
        "Dükkân kasası",
        AccountType.Cash,
        CurrencyCode.TRY,
        defaultScope: defaultScope);

    private static Category CreateCategory(Guid userId, TransactionScope? defaultScope) => new(
        Guid.NewGuid(),
        userId,
        "Ticari mal alımı",
        CategoryType.Expense,
        defaultScope);

    private sealed class FakeCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingTransactionRepository : ITransactionRepository
    {
        private readonly List<BudgetTransaction> _saved = [];

        public IReadOnlyList<BudgetTransaction> Saved => _saved;

        public Task AddAsync(BudgetTransaction transaction, CancellationToken cancellationToken)
        {
            _saved.Add(transaction);
            return Task.CompletedTask;
        }

        public Task<BudgetTransaction?> FindOwnedByIdAsync(
            Guid transactionId, Guid userId, bool track, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task UpdateOwnedAsync(
            BudgetTransaction transaction, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<TransactionListPage> ListAsync(
            Guid userId, TransactionListCriteria criteria, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class FakeBudgetRepository : IBudgetRepository
    {
        public Task<bool> ExistsAsync(
            Guid userId, Guid categoryId, int year, int month, CancellationToken cancellationToken) =>
            Task.FromResult(false);
        public Task AddAsync(MonthlyBudget budget, CancellationToken cancellationToken) =>
            Task.CompletedTask;
        public Task<MonthlyBudget?> FindOwnedByIdAsync(
            Guid budgetId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task UpdateOwnedAsync(
            MonthlyBudget budget, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<bool> DeleteOwnedAsync(
            Guid budgetId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<IReadOnlyList<BudgetDto>> ListWithProgressAsync(
            Guid userId, int year, int month, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class FakeAccountRepository(Account account) : IAccountRepository
    {
        public Task<Account?> FindOwnedByIdAsync(
            Guid accountId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(account.Id == accountId && account.UserId == userId ? account : null);
        public Task AddAsync(Account value, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<bool> ExistsByNameAsync(
            Guid userId, string normalizedName, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<AccountListPage> ListAsync(
            Guid userId, AccountListCriteria criteria, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task UpdateOwnedAsync(Account value, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<decimal> CalculateBalanceAsync(
            Guid accountId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class FakeCategoryRepository(Category category) : ICategoryRepository
    {
        public Task<Category?> FindOwnedByIdAsync(
            Guid categoryId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(category.Id == categoryId && category.UserId == userId ? category : null);
        public Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken) =>
            Task.CompletedTask;
        public Task AddAsync(Category value, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<bool> ExistsByNameAndTypeAsync(
            Guid userId, string name, CategoryType type, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<IReadOnlyList<Category>> ListAsync(
            Guid userId, CategoryType? type, bool? isActive, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task UpdateOwnedAsync(Category value, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
