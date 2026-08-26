using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Taxes;

/// <summary>
/// Aşama 05 Grup 3: indirilebilirlik zinciri — kullanıcının açık seçimi →
/// kategorinin varsayılanı. Soru yalnız işletme kapsamlı giderde sorulur
/// (ADR 0016).
/// </summary>
public sealed class TaxDeductibilityResolutionTests
{
    [Fact]
    public void Resolve_PrefersTheExplicitAnswerOverTheCategoryDefault()
    {
        var resolved = TaxDeductibilityResolution.Resolve(
            requested: false,
            categoryDefault: true,
            TransactionScope.Business,
            recognizesExpense: true);

        Assert.False(resolved);
    }

    [Fact]
    public void Resolve_FallsBackToTheCategoryDefault()
    {
        var resolved = TaxDeductibilityResolution.Resolve(
            requested: null,
            categoryDefault: true,
            TransactionScope.Business,
            recognizesExpense: true);

        Assert.True(resolved);
    }

    /// <summary>
    /// Soru sorulmayan kayıtta kategorinin cevabı <b>sessizce düşer</b>:
    /// kullanıcı onu istemedi, kategori söyledi.
    /// </summary>
    [Theory]
    [InlineData(TransactionScope.Personal, true)]
    [InlineData(TransactionScope.Business, false)]
    public void Resolve_DropsTheCategoryDefault_WhereTheQuestionIsNotAsked(
        TransactionScope scope,
        bool recognizesExpense)
    {
        var resolved = TaxDeductibilityResolution.Resolve(
            requested: null,
            categoryDefault: true,
            scope,
            recognizesExpense);

        Assert.Null(resolved);
    }

    /// <summary>
    /// Kullanıcının kendi cevabı düşürülmez; olduğu gibi geçer ve çağıran onu
    /// reddeder. Sessizce yok saymak, kaydedilmeyen bir şeyi kaydedilmiş gibi
    /// göstermek olurdu.
    /// </summary>
    [Fact]
    public void Resolve_KeepsTheExplicitAnswerSoTheCallerCanRefuseIt()
    {
        var resolved = TaxDeductibilityResolution.Resolve(
            requested: true,
            categoryDefault: null,
            TransactionScope.Personal,
            recognizesExpense: true);

        Assert.True(resolved);
    }

    [Fact]
    public async Task CreateTransaction_WithoutAnAnswer_TakesTheCategoryDefault()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId);
        var category = CreateCategory(userId, defaultIsTaxDeductible: true);
        var repository = new RecordingTransactionRepository();

        var result = await UseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsTaxDeductible);
        Assert.True(Assert.Single(repository.Saved).IsTaxDeductible);
    }

    /// <summary>
    /// Şahsi bir kayıt aynı kategoriyle yazıldığında kategorinin varsayılanı
    /// kayda geçmez: soru orada sorulmaz.
    /// </summary>
    [Fact]
    public async Task CreateTransaction_OnAPersonalRecord_DoesNotInheritTheDefault()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId);
        var category = CreateCategory(userId, defaultIsTaxDeductible: true);
        var repository = new RecordingTransactionRepository();

        var result = await UseCase(userId, account, category, repository)
            .ExecuteAsync(Command(account, category, scope: TransactionScope.Personal));

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.IsTaxDeductible);
    }

    /// <summary>
    /// Şahsi kayda açıkça cevap gönderilirse istek reddedilir ve geriye kayıt
    /// kalmaz.
    /// </summary>
    [Fact]
    public async Task CreateTransaction_AnsweringForAPersonalRecord_IsRefusedAndWritesNothing()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId);
        var category = CreateCategory(userId, defaultIsTaxDeductible: null);
        var repository = new RecordingTransactionRepository();

        var result = await UseCase(userId, account, category, repository)
            .ExecuteAsync(Command(
                account,
                category,
                scope: TransactionScope.Personal,
                isTaxDeductible: true));

        Assert.False(result.IsSuccess);
        Assert.Empty(repository.Saved);
    }

    private static CreateTransactionUseCase UseCase(
        Guid userId,
        Account account,
        Category category,
        ITransactionRepository repository) => new(
        new FakeCurrentUser(userId),
        new FakeAccountRepository(account),
        new FakeCategoryRepository(category),
        repository);

    private static CreateTransactionCommand Command(
        Account account,
        Category category,
        TransactionScope? scope = TransactionScope.Business,
        bool? isTaxDeductible = null) => new(
        account.Id,
        category.Id,
        100m,
        CurrencyCode.TRY,
        TransactionType.Expense,
        scope,
        new DateOnly(2026, 8, 26),
        "Sentetik gider",
        Vat: null,
        IsTaxDeductible: isTaxDeductible);

    private static Account CreateAccount(Guid userId) => new(
        Guid.NewGuid(), userId, "Dükkân kasası", AccountType.Cash, CurrencyCode.TRY);

    private static Category CreateCategory(Guid userId, bool? defaultIsTaxDeductible) => new(
        Guid.NewGuid(),
        userId,
        "Ticari mal alımı",
        CategoryType.Expense,
        null,
        defaultIsTaxDeductible);

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
