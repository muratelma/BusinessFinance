using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Tests.RecurringTransactions;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Scopes;

/// <summary>
/// Taraf kuralı (ADR 0020): kategori kaydın alabileceği tarafları belirler;
/// iki tarafa açık kategoride girişin bağlamı, bağlam yoksa kullanıcının
/// seçimi belirler. Hesabın etiketi yalnız seçimin ön değeridir. Çelişen
/// açık seçim reddedilir; sunucu taraf uydurmaz.
/// </summary>
public sealed class TransactionScopeResolutionTests
{
    private const TransactionScope Business = TransactionScope.Business;
    private const TransactionScope Personal = TransactionScope.Personal;

    /// <summary>
    /// Durum tablosu: istek × kategori × kaynağın etiketi. Beklenen boşsa
    /// sonuç bir rettir ve nedeni ayrıca verilir.
    /// </summary>
    [Theory]
    // Tek taraflı kategori tarafı söyler; hesabın etiketi onu ezemez.
    [InlineData(null, Personal, null, Personal, ScopeResolutionFailure.None)]
    [InlineData(null, Personal, Business, Personal, ScopeResolutionFailure.None)]
    [InlineData(Personal, Personal, Business, Personal, ScopeResolutionFailure.None)]
    [InlineData(null, Business, Personal, Business, ScopeResolutionFailure.None)]
    // Kategoriyle çelişen açık seçim reddedilir.
    [InlineData(Business, Personal, null, null, ScopeResolutionFailure.Conflict)]
    [InlineData(Personal, Business, Business, null, ScopeResolutionFailure.Conflict)]
    // İki tarafa açık kategori: açık seçim, yoksa kaynağın etiketi.
    [InlineData(Business, null, null, Business, ScopeResolutionFailure.None)]
    [InlineData(Personal, null, Business, Personal, ScopeResolutionFailure.None)]
    [InlineData(null, null, Business, Business, ScopeResolutionFailure.None)]
    [InlineData(null, null, Personal, Personal, ScopeResolutionFailure.None)]
    // Hiçbir işaret yok: sunucu taraf uydurmaz.
    [InlineData(null, null, null, null, ScopeResolutionFailure.Unresolved)]
    public void Resolve_FollowsTheStateTable(
        TransactionScope? requested,
        TransactionScope? categorySide,
        TransactionScope? sourceLabel,
        TransactionScope? expected,
        ScopeResolutionFailure expectedFailure)
    {
        var resolution = TransactionScopeResolution.Resolve(requested, categorySide, sourceLabel);

        Assert.Equal(expected, resolution.Scope);
        Assert.Equal(expectedFailure, resolution.Failure);
    }

    /// <summary>
    /// Bağlamı olan giriş (POS satışı, gün sonu, cari borçlandırma, komisyon
    /// ve kesinti) tek bir tarafa aittir: kategori öbür tarafa özelse ya da
    /// istek öbür tarafı istiyorsa reddedilir.
    /// </summary>
    [Theory]
    [InlineData(null, Business, Business, ScopeResolutionFailure.None)]
    [InlineData(null, null, Business, ScopeResolutionFailure.None)]
    [InlineData(Business, null, Business, ScopeResolutionFailure.None)]
    [InlineData(Personal, null, null, ScopeResolutionFailure.Conflict)]
    [InlineData(null, Personal, null, ScopeResolutionFailure.Conflict)]
    [InlineData(Business, Personal, null, ScopeResolutionFailure.Conflict)]
    public void ResolveInContext_WritesTheContextOrRejects(
        TransactionScope? requested,
        TransactionScope? categorySide,
        TransactionScope? expected,
        ScopeResolutionFailure expectedFailure)
    {
        var resolution = TransactionScopeResolution.ResolveInContext(
            Business, requested, categorySide);

        Assert.Equal(expected, resolution.Scope);
        Assert.Equal(expectedFailure, resolution.Failure);
    }

    /// <summary>
    /// İşletmesi olmayan kullanıcıda taraf sorulmaz: hiçbir işaret yoksa kayıt
    /// şahsidir. İşletmesi olan kullanıcıda aynı durum çözülmemiş kalır.
    /// </summary>
    [Theory]
    [InlineData(false, Personal, ScopeResolutionFailure.None)]
    [InlineData(true, null, ScopeResolutionFailure.Unresolved)]
    public async Task ResolveAsync_WithNothingToGoOn_AsksTheProfile(
        bool hasBusiness,
        TransactionScope? expected,
        ScopeResolutionFailure expectedFailure)
    {
        var resolution = await TransactionScopeResolution.ResolveAsync(
            null, null, null, new FakeUserProfileRepository(hasBusiness), Guid.NewGuid(), default);

        Assert.Equal(expected, resolution.Scope);
        Assert.Equal(expectedFailure, resolution.Failure);
    }

    /// <summary>
    /// Profil yalnız son adımda sorulur: çelişki, kullanıcının işletmesi
    /// olmasa da şahsiye çevrilmez.
    /// </summary>
    [Fact]
    public async Task ResolveAsync_OnAConflict_DoesNotFallBackToTheProfile()
    {
        var resolution = await TransactionScopeResolution.ResolveAsync(
            Business, Personal, null, new FakeUserProfileRepository(false), Guid.NewGuid(), default);

        Assert.Null(resolution.Scope);
        Assert.Equal(ScopeResolutionFailure.Conflict, resolution.Failure);
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

    /// <summary>
    /// Kasadan market: kasa işletme etiketli, kategori şahsi. Kayıt şahsi
    /// giderdir; kasanın etiketi onu işletme gideri yapmaz.
    /// </summary>
    [Fact]
    public async Task CreateTransaction_WithoutScope_TakesTheCategorySideOverTheAccountLabel()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, TransactionScope.Business);
        var category = CreateCategory(userId, TransactionScope.Personal);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, scope: null));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Personal, result.Value.Scope);
        Assert.Equal(TransactionScope.Personal, Assert.Single(repository.Saved).Scope);
    }

    [Fact]
    public async Task CreateTransaction_WithACategoryOpenToBoth_TakesTheAccountLabel()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, TransactionScope.Business);
        var category = CreateCategory(userId, null);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, scope: null));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Business, result.Value.Scope);
    }

    [Fact]
    public async Task CreateTransaction_WithACategoryOpenToBoth_TakesTheExplicitChoice()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, TransactionScope.Business);
        var category = CreateCategory(userId, null);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, TransactionScope.Personal));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Personal, result.Value.Scope);
    }

    /// <summary>
    /// Kategoriyle çelişen açık seçim sessizce düzeltilmez: kayıt oluşmaz.
    /// Aksi hâlde formun gösterdiği ile yazılan ayrışırdı.
    /// </summary>
    [Fact]
    public async Task CreateTransaction_WithAScopeTheCategoryForbids_IsRejectedAndWritesNothing()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, TransactionScope.Business);
        var category = CreateCategory(userId, TransactionScope.Personal);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, TransactionScope.Business));

        Assert.False(result.IsSuccess);
        Assert.Equal("transactions.scope_conflict", result.Error.Code);
        Assert.Empty(repository.Saved);
    }

    /// <summary>
    /// Hiçbir işaret yoksa işletmesi olan kullanıcıda kayıt <b>oluşmaz</b>. Bir
    /// değer seçmek, yanlış etiketlenmiş bir hareketi kullanıcı fark edene
    /// kadar işletme netinin içinde bırakmak olurdu.
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
    /// Aynı durumda işletmesi olmayan kullanıcının kaydı şahsi yazılır: o
    /// kullanıcıya taraf hiç sorulmaz (8 Ekim 2026'da cihazda görülen hata).
    /// </summary>
    [Fact]
    public async Task CreateTransaction_WithNothingToGoOn_IsPersonalForAUserWithoutABusiness()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, null);
        var category = CreateCategory(userId, null);
        var repository = new RecordingTransactionRepository();

        var result = await CreateTransactionUseCase(
                userId, account, category, repository, hasBusiness: false)
            .ExecuteAsync(Command(account, category, scope: null));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Personal, Assert.Single(repository.Saved).Scope);
    }

    /// <summary>
    /// Bütçenin hesabı yoktur: kategori tarafı söyler, iki tarafa açıksa
    /// seçim gerekir, çelişen seçim reddedilir.
    /// </summary>
    [Fact]
    public async Task CreateBudget_TakesTheCategorySideOrNeedsAChoice()
    {
        var userId = Guid.NewGuid();
        var labelled = CreateCategory(userId, TransactionScope.Business);
        var unlabelled = CreateCategory(userId, null);

        var resolved = await CreateBudgetUseCase(userId, labelled)
            .ExecuteAsync(BudgetCommand(labelled, scope: null));
        var conflicting = await CreateBudgetUseCase(userId, labelled)
            .ExecuteAsync(BudgetCommand(labelled, TransactionScope.Personal));
        var rejected = await CreateBudgetUseCase(userId, unlabelled)
            .ExecuteAsync(BudgetCommand(unlabelled, scope: null));

        Assert.True(resolved.IsSuccess);
        Assert.Equal(TransactionScope.Business, resolved.Value.Scope);
        Assert.False(conflicting.IsSuccess);
        Assert.Equal("budgets.scope_conflict", conflicting.Error.Code);
        Assert.False(rejected.IsSuccess);
        Assert.Equal("budgets.scope_unresolved", rejected.Error.Code);
    }

    private static CreateTransactionUseCase CreateTransactionUseCase(
        Guid userId,
        Account account,
        Category category,
        ITransactionRepository repository,
        bool hasBusiness = true)
    {
        return new CreateTransactionUseCase(
            new FakeCurrentUser(userId),
            new FakeAccountRepository(account),
            new FakeCategoryRepository(category),
            repository,
            new FakeUserProfileRepository(hasBusiness));
    }

    private static CreateBudgetUseCase CreateBudgetUseCase(Guid userId, Category category)
    {
        return new CreateBudgetUseCase(
            new FakeCurrentUser(userId),
            new FakeCategoryRepository(category),
            new FakeBudgetRepository(),
            new FakeUserProfileRepository(true));
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
