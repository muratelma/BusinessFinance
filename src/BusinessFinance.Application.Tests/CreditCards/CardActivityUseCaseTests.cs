using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.Tests.RecurringTransactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.CreditCards;

public sealed class CardActivityUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task Charge_ThatWouldExceedLimit_ReturnsConflictWithoutPersisting()
    {
        var card = CreateCard();
        var chargeRepository = new FakeChargeRepository();
        var useCase = new CreateCardChargeUseCase(
            new FakeCurrentUser(UserId),
            new FakeCardRepository(card, 900m),
            new FakeCategoryRepository(CreateExpense()),
            chargeRepository,
            new FakeUserProfileRepository(true));

        var result = await useCase.ExecuteAsync(new CreateCardChargeCommand(
            card.Id, CreateExpense().Id, 101m, CurrencyCode.TRY,
            TransactionScope.Business,
            new DateOnly(2026, 8, 10), null));

        Assert.False(result.IsSuccess);
        Assert.Equal("credit_cards.limit_exceeded", result.Error.Code);
        Assert.Empty(chargeRepository.Items);
    }

    [Fact]
    public async Task Payment_ThatExceedsDebt_ReturnsConflictWithoutPersisting()
    {
        var card = CreateCard();
        var payments = new FakePaymentRepository();
        var useCase = new CreateCardPaymentUseCase(
            new FakeCurrentUser(UserId),
            new FakeCardRepository(card, 100m),
            new FakeAccountRepository(CreateAccount()),
            payments);

        var result = await useCase.ExecuteAsync(new CreateCardPaymentCommand(
            card.Id, CreateAccount().Id, 101m, CurrencyCode.TRY,
            new DateOnly(2026, 8, 10), null));

        Assert.False(result.IsSuccess);
        Assert.Equal("credit_cards.payment_exceeds_debt", result.Error.Code);
        Assert.Empty(payments.Items);
    }

    [Fact]
    public async Task Payment_ToInactiveCard_IsPersistedForExistingDebt()
    {
        var card = CreateCard();
        card.Update(card.Name, card.Limit, 10, 20, card.MinimumPaymentRate, false);
        var account = CreateAccount();
        var payments = new FakePaymentRepository();
        var useCase = new CreateCardPaymentUseCase(
            new FakeCurrentUser(UserId),
            new FakeCardRepository(card, 100m),
            new FakeAccountRepository(account),
            payments);

        var result = await useCase.ExecuteAsync(new CreateCardPaymentCommand(
            card.Id, account.Id, 100m, CurrencyCode.TRY,
            new DateOnly(2026, 8, 10), null));

        Assert.True(result.IsSuccess);
        Assert.Single(payments.Items);
    }

    private static CreditCard CreateCard() => new(
        Guid.NewGuid(), UserId, "Card", new Money(1000m, CurrencyCode.TRY), 10, 20);
    private static Category CreateExpense() => Expense;
    private static readonly Category Expense = new(
        Guid.NewGuid(), UserId, "Food", CategoryType.Expense);
    private static Account CreateAccount() => Account;
    private static readonly Account Account = new(
        Guid.NewGuid(), UserId, "Bank", AccountType.Bank, CurrencyCode.TRY, 1000m);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakeCardRepository(CreditCard card, decimal debt) : ICreditCardRepository
    {
        public Task<CreditCard?> FindOwnedByIdAsync(Guid creditCardId, Guid userId, bool track, CancellationToken cancellationToken) =>
            Task.FromResult(card.Id == creditCardId && card.UserId == userId ? card : null);
        public Task<decimal> CalculateCurrentDebtAsync(Guid creditCardId, Guid userId, CancellationToken cancellationToken) => Task.FromResult(debt);
        public Task AddAsync(CreditCard creditCard, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<bool> ExistsByNameAsync(Guid userId, string normalizedName, Guid? exceptCreditCardId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<IReadOnlyList<CreditCard>> ListAsync(Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(CreditCard creditCard, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FakeCategoryRepository(Category category) : ICategoryRepository
    {
        public Task<Category?> FindOwnedByIdAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(category.Id == categoryId && category.UserId == userId ? category : null);
        public Task AddAsync(Category category, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<bool> ExistsByNameAndTypeAsync(Guid userId, string name, CategoryType type, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<IReadOnlyList<Category>> ListAsync(Guid userId, CategoryType? type, bool? isActive, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(Category category, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FakeAccountRepository(Account account) : IAccountRepository
    {
        public Task<Account?> FindOwnedByIdAsync(Guid accountId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(account.Id == accountId && account.UserId == userId ? account : null);
        public Task AddAsync(Account account, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<bool> ExistsByNameAsync(Guid userId, string normalizedName, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<AccountListPage> ListAsync(Guid userId, AccountListCriteria criteria, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(Account account, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<decimal> CalculateBalanceAsync(Guid accountId, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FakeChargeRepository : ICardChargeRepository
    {
        public List<CreditCardCharge> Items { get; } = [];
        public Task AddAsync(CreditCardCharge charge, CancellationToken cancellationToken) { Items.Add(charge); return Task.CompletedTask; }
        public Task<CreditCardCharge?> FindOwnedByIdAsync(Guid chargeId, Guid userId, bool track, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<BoundedList<CreditCardCharge>> ListAsync(Guid creditCardId, Guid userId, HistoryWindow window, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(CreditCardCharge charge, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FakePaymentRepository : ICardPaymentRepository
    {
        public List<CreditCardPayment> Items { get; } = [];
        public Task AddAsync(CreditCardPayment payment, CancellationToken cancellationToken) { Items.Add(payment); return Task.CompletedTask; }
        public Task<CreditCardPayment?> FindOwnedByIdAsync(Guid paymentId, Guid userId, bool track, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<BoundedList<CreditCardPayment>> ListAsync(Guid creditCardId, Guid userId, HistoryWindow window, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task UpdateOwnedAsync(CreditCardPayment payment, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
