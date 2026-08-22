using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Debts;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Debts;

public sealed class RecordDebtOpeningUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly DateOnly AsOf = new(2026, 8, 17);

    [Fact]
    public async Task ExecuteAsync_CompletesAnUnrecordedOpeningAndItsSplit()
    {
        // Bu ayrımdan önce açılmış borçların açılışı hiçbir sayıdan
        // türetilemiyor; onu yalnız kullanıcı biliyor.
        var debt = UnrecordedDebt();
        var category = ExpenseCategory();
        var repository = new FakeDebtRepository(debt);
        var useCase = Build(repository, category: category);

        var result = await useCase.ExecuteAsync(
            new RecordDebtOpeningCommand(debt.Id, DebtSourceType.Expense, null, category.Id), AsOf);

        Assert.True(result.IsSuccess);
        Assert.Equal(DebtSourceType.Expense, result.Value.SourceType);
        Assert.Equal(category.Id, result.Value.CategoryId);
        Assert.True(repository.OpeningSaved);
        Assert.All(result.Value.Installments, item => Assert.True(item.InterestPortion >= 0m));
        Assert.Equal(30m, result.Value.Installments.Sum(item => item.InterestPortion));
    }

    [Fact]
    public async Task ExecuteAsync_WhenTheOpeningWasAlreadyRecorded_ReportsAConflict()
    {
        // Kaydedilmiş bir açılışı değiştirmek, geçmişte yazılmış gideri ya da
        // bakiye hareketini geriye dönük silmek olurdu.
        var account = ActiveAccount();
        var debt = new DebtAgreement(
            Guid.NewGuid(), UserId, "Lender", DebtDirection.Payable,
            TransactionScope.Business,
            new Money(300m, CurrencyCode.TRY), new Money(330m, CurrencyCode.TRY),
            DebtSourceType.Cash, account, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 15), 3);
        var useCase = Build(new FakeDebtRepository(debt), account);

        var result = await useCase.ExecuteAsync(
            new RecordDebtOpeningCommand(debt.Id, DebtSourceType.Cash, account.Id, null), AsOf);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTheDebtBelongsToSomeoneElse_IsNotFound()
    {
        var useCase = Build(new FakeDebtRepository(null));

        var result = await useCase.ExecuteAsync(
            new RecordDebtOpeningCommand(Guid.NewGuid(), DebtSourceType.Cash, Guid.NewGuid(), null), AsOf);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTheCategoryIsNotAnActiveExpenseCategory_IsRejected()
    {
        var debt = UnrecordedDebt();
        var income = new Category(Guid.NewGuid(), UserId, "Maaş", CategoryType.Income);
        var useCase = Build(new FakeDebtRepository(debt), category: income);

        var result = await useCase.ExecuteAsync(
            new RecordDebtOpeningCommand(debt.Id, DebtSourceType.Expense, null, income.Id), AsOf);

        Assert.False(result.IsSuccess);
        Assert.Equal("debt.category_unavailable", result.Error.Code);
    }

    private static RecordDebtOpeningUseCase Build(
        FakeDebtRepository repository,
        Account? account = null,
        Category? category = null) => new(
            new FakeCurrentUser(UserId),
            repository,
            new FakeAccountRepository(account),
            new FakeCategoryRepository(category));

    private static DebtAgreement UnrecordedDebt() => DebtAgreement.WithUnrecordedOpening(
        Guid.NewGuid(), UserId, "Legacy lender", DebtDirection.Payable,
        TransactionScope.Business,
        new Money(300m, CurrencyCode.TRY), new Money(330m, CurrencyCode.TRY),
        new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 15), 3);

    private static Account ActiveAccount() =>
        new(Guid.NewGuid(), UserId, "Nakit", AccountType.Cash, CurrencyCode.TRY);

    private static Category ExpenseCategory() =>
        new(Guid.NewGuid(), UserId, "Ulaşım", CategoryType.Expense);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    // Yalnız sahiplik kapsamlı arama gerçek; kalan port yüzeyi bu use case'in
    // yolunda değil ve çağrılırsa test sessizce yanlış şeyi doğrulamak yerine
    // düşer.
    private sealed class FakeAccountRepository(Account? account) : IAccountRepository
    {
        public Task<Account?> FindOwnedByIdAsync(
            Guid accountId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(account?.Id == accountId ? account : null);

        public Task AddAsync(Account item, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsByNameAsync(
            Guid userId, string normalizedName, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<AccountListPage> ListAsync(
            Guid userId, AccountListCriteria criteria, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task UpdateOwnedAsync(Account item, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<decimal> CalculateBalanceAsync(
            Guid accountId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class FakeCategoryRepository(Category? category) : ICategoryRepository
    {
        public Task<Category?> FindOwnedByIdAsync(
            Guid categoryId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(category?.Id == categoryId ? category : null);

        public Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddAsync(Category item, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsByNameAndTypeAsync(
            Guid userId, string name, CategoryType type, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Category>> ListAsync(
            Guid userId, CategoryType? type, bool? isActive, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task UpdateOwnedAsync(Category item, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class FakeDebtRepository(DebtAgreement? debt) : IDebtRepository
    {
        public bool OpeningSaved { get; private set; }

        public Task<DebtAgreement?> FindOwnedByIdAsync(
            Guid id, Guid userId, bool track, CancellationToken cancellationToken) =>
            Task.FromResult(debt);

        public Task SaveOpeningAsync(DebtAgreement agreement, CancellationToken cancellationToken)
        {
            OpeningSaved = true;
            return Task.CompletedTask;
        }

        public Task AddAsync(DebtAgreement agreement, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<DebtAgreement>> ListAsync(
            Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task SavePaymentAsync(DebtInstallment installment, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
