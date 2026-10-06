using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Kapsam boyutunun domain invariant'ları: kimin taşımak zorunda olduğu, kimin
/// taşımadığı ve boş bırakılabilen varsayılanın nasıl davrandığı.
/// </summary>
public class TransactionScopeTests
{
    private const TransactionScope Undefined = (TransactionScope)7;

    [Theory]
    [InlineData(TransactionScope.Business)]
    [InlineData(TransactionScope.Personal)]
    public void BudgetTransaction_PreservesScope(TransactionScope scope)
    {
        var userId = Guid.NewGuid();

        var transaction = new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId),
            CreateCategory(userId, CategoryType.Expense),
            new Money(120m, CurrencyCode.TRY),
            TransactionType.Expense,
            scope,
            new DateOnly(2026, 8, 7));

        Assert.Equal(scope, transaction.Scope);
    }

    [Fact]
    public void BudgetTransaction_WithUndefinedScope_ThrowsArgumentOutOfRange()
    {
        var userId = Guid.NewGuid();

        Action act = () => new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId),
            CreateCategory(userId, CategoryType.Expense),
            new Money(120m, CurrencyCode.TRY),
            TransactionType.Expense,
            Undefined,
            new DateOnly(2026, 8, 7));

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Theory]
    [InlineData(TransactionScope.Business)]
    [InlineData(TransactionScope.Personal)]
    public void CreditCardCharge_PreservesScope(TransactionScope scope)
    {
        var userId = Guid.NewGuid();

        var charge = new CreditCardCharge(
            Guid.NewGuid(),
            userId,
            CreateCreditCard(userId),
            CreateCategory(userId, CategoryType.Expense),
            new Money(90m, CurrencyCode.TRY),
            scope,
            new DateOnly(2026, 8, 7));

        Assert.Equal(scope, charge.Scope);
    }

    [Fact]
    public void CreditCardCharge_WithUndefinedScope_ThrowsArgumentOutOfRange()
    {
        var userId = Guid.NewGuid();

        Action act = () => new CreditCardCharge(
            Guid.NewGuid(),
            userId,
            CreateCreditCard(userId),
            CreateCategory(userId, CategoryType.Expense),
            new Money(90m, CurrencyCode.TRY),
            Undefined,
            new DateOnly(2026, 8, 7));

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void InstallmentPlan_PreservesScope_AndRejectsUndefined()
    {
        var userId = Guid.NewGuid();
        var card = CreateCreditCard(userId);
        var category = CreateCategory(userId, CategoryType.Expense);

        var plan = new InstallmentPlan(
            Guid.NewGuid(),
            userId,
            card,
            category,
            Guid.NewGuid(),
            new Money(300m, CurrencyCode.TRY),
            TransactionScope.Business,
            3,
            new DateOnly(2026, 8, 7));

        Action act = () => new InstallmentPlan(
            Guid.NewGuid(),
            userId,
            card,
            category,
            Guid.NewGuid(),
            new Money(300m, CurrencyCode.TRY),
            Undefined,
            3,
            new DateOnly(2026, 8, 7));

        Assert.Equal(TransactionScope.Business, plan.Scope);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void DebtAgreement_PreservesScope_AndRejectsUndefined()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId);

        var lender = new Counterparty(Guid.NewGuid(), userId, "Synthetic lender");

        var debt = new DebtAgreement(
            Guid.NewGuid(),
            userId,
            lender,
            DebtDirection.Payable,
            TransactionScope.Business,
            new Money(300m, CurrencyCode.TRY),
            new Money(330m, CurrencyCode.TRY),
            DebtSourceType.Cash,
            account,
            null,
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 15),
            3);

        Action act = () => DebtAgreement.WithUnrecordedOpening(
            Guid.NewGuid(),
            userId,
            lender,
            DebtDirection.Payable,
            Undefined,
            new Money(300m, CurrencyCode.TRY),
            new Money(330m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 15),
            3);

        Assert.Equal(TransactionScope.Business, debt.Scope);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void RecurringTransaction_CarriesScope_ForBothSources()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId, CategoryType.Expense);

        var fromAccount = new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId),
            category,
            new Money(50m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 1));

        var fromCard = new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            CreateCreditCard(userId),
            category,
            new Money(50m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            TransactionScope.Personal,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 1));

        Assert.Equal(TransactionScope.Business, fromAccount.Scope);
        Assert.Equal(TransactionScope.Personal, fromCard.Scope);
    }

    [Fact]
    public void RecurringTransaction_WithUndefinedScope_ThrowsArgumentOutOfRange()
    {
        var userId = Guid.NewGuid();

        Action act = () => new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId),
            CreateCategory(userId, CategoryType.Expense),
            new Money(50m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            Undefined,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 1));

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void MonthlyBudget_PreservesScope_AndRejectsUndefined()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId, CategoryType.Expense);

        var budget = new MonthlyBudget(
            Guid.NewGuid(),
            userId,
            category,
            new Money(1000m, CurrencyCode.TRY),
            TransactionScope.Business,
            2026,
            8);

        Action act = () => new MonthlyBudget(
            Guid.NewGuid(),
            userId,
            category,
            new Money(1000m, CurrencyCode.TRY),
            Undefined,
            2026,
            8);

        Assert.Equal(TransactionScope.Business, budget.Scope);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    /// <summary>
    /// Transfer ve kart ödemesi gelir/gider raporuna sıfır etki eder (ADR 0002,
    /// ADR 0003). Kapsam sormak, cevabı hiçbir yerde kullanılmayan bir soru
    /// sormak olurdu; bu yüzden alan da yoktur ve eklenmesi bu testi kırar.
    /// </summary>
    [Theory]
    [InlineData(typeof(Transfer))]
    [InlineData(typeof(CreditCardPayment))]
    public void MoneyMovingModels_DoNotCarryScope(Type type)
    {
        Assert.Null(type.GetProperty(nameof(BudgetTransaction.Scope)));
    }

    [Fact]
    public void Account_DefaultScope_IsEmptyUntilSet_AndCanBeCleared()
    {
        var account = CreateAccount(Guid.NewGuid());

        Assert.Null(account.DefaultScope);

        account.SetDefaultScope(TransactionScope.Business);
        Assert.Equal(TransactionScope.Business, account.DefaultScope);

        account.SetDefaultScope(null);
        Assert.Null(account.DefaultScope);
    }

    [Fact]
    public void Category_DefaultScope_IsCarriedFromConstructor()
    {
        var category = new Category(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Ticari mal alımı",
            CategoryType.Expense,
            TransactionScope.Business);

        Assert.Equal(TransactionScope.Business, category.DefaultScope);
    }

    [Fact]
    public void CreditCard_DefaultScope_IsEmptyUntilSet()
    {
        var card = CreateCreditCard(Guid.NewGuid());

        Assert.Null(card.DefaultScope);

        card.SetDefaultScope(TransactionScope.Personal);
        Assert.Equal(TransactionScope.Personal, card.DefaultScope);
    }

    [Fact]
    public void DefaultScope_WithUndefinedValue_ThrowsArgumentOutOfRange()
    {
        var account = CreateAccount(Guid.NewGuid());

        Action act = () => account.SetDefaultScope(Undefined);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    private static Account CreateAccount(Guid userId)
    {
        return new Account(
            Guid.NewGuid(),
            userId,
            "Dükkân kasası",
            AccountType.Cash,
            CurrencyCode.TRY);
    }

    private static Category CreateCategory(Guid userId, CategoryType type)
    {
        return new Category(
            Guid.NewGuid(),
            userId,
            "Synthetic Category",
            type);
    }

    private static CreditCard CreateCreditCard(Guid userId)
    {
        return new CreditCard(
            Guid.NewGuid(),
            userId,
            "Synthetic Card",
            new Money(10000m, CurrencyCode.TRY),
            statementClosingDay: 10,
            paymentDueDay: 20);
    }
}
