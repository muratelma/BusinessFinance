using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public class BudgetTransactionTests
{
    [Fact]
    public void Cancel_PreservesTransactionAndMarksItCancelledIdempotently()
    {
        var transaction = CreateTransaction();
        var cancelledAt = new DateTimeOffset(2026, 8, 9, 10, 30, 0, TimeSpan.Zero);

        transaction.Cancel(cancelledAt);
        transaction.Cancel(cancelledAt.AddMinutes(1));

        Assert.True(transaction.IsCancelled);
        Assert.Equal(cancelledAt, transaction.CancelledAtUtc);
    }

    [Fact]
    public void Cancel_WithNonUtcTimestamp_ThrowsArgumentException()
    {
        var transaction = CreateTransaction();

        Action act = () => transaction.Cancel(
            new DateTimeOffset(2026, 8, 9, 13, 30, 0, TimeSpan.FromHours(3)));

        Assert.Throws<ArgumentException>(act);
    }
    [Theory]
    [InlineData(TransactionType.Income, CategoryType.Income)]
    [InlineData(TransactionType.Expense, CategoryType.Expense)]
    public void Constructor_WithValidValues_PreservesValues(
        TransactionType transactionType,
        CategoryType categoryType)
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId);
        var category = CreateCategory(userId, categoryType);
        var amount = new Money(125.50m, CurrencyCode.TRY);
        var date = new DateOnly(2026, 8, 7);

        var transaction = new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            account,
            category,
            amount,
            transactionType,
            date,
            "  Synthetic transaction  ");

        Assert.Equal(userId, transaction.UserId);
        Assert.Equal(account.Id, transaction.AccountId);
        Assert.Equal(category.Id, transaction.CategoryId);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(transactionType, transaction.Type);
        Assert.Equal(date, transaction.TransactionDate);
        Assert.Equal("Synthetic transaction", transaction.Description);
    }

    [Fact]
    public void Constructor_WithWhitespaceDescription_NormalizesToNull()
    {
        var transaction = CreateTransaction(description: "   ");

        Assert.Null(transaction.Description);
    }

    [Fact]
    public void Constructor_WithTooLongDescription_ThrowsArgumentException()
    {
        Action act = () => CreateTransaction(
            description: new string('D', BudgetTransaction.MaximumDescriptionLength + 1));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        Action act = () => CreateTransaction(id: Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsArgumentException()
    {
        var ownerId = Guid.NewGuid();

        Action act = () => new BudgetTransaction(
            Guid.NewGuid(),
            Guid.Empty,
            CreateAccount(ownerId),
            CreateCategory(ownerId, CategoryType.Expense),
            new Money(100m, CurrencyCode.TRY),
            TransactionType.Expense,
            new DateOnly(2026, 8, 7));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithNullAccount_ThrowsArgumentNullException()
    {
        var userId = Guid.NewGuid();

        Action act = () => new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            null!,
            CreateCategory(userId, CategoryType.Expense),
            new Money(100m, CurrencyCode.TRY),
            TransactionType.Expense,
            new DateOnly(2026, 8, 7));

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithNullCategory_ThrowsArgumentNullException()
    {
        var userId = Guid.NewGuid();

        Action act = () => new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId),
            null!,
            new Money(100m, CurrencyCode.TRY),
            TransactionType.Expense,
            new DateOnly(2026, 8, 7));

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithNullAmount_ThrowsArgumentNullException()
    {
        var userId = Guid.NewGuid();

        Action act = () => new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId),
            CreateCategory(userId, CategoryType.Expense),
            null!,
            TransactionType.Expense,
            new DateOnly(2026, 8, 7));

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithAccountOwnedByAnotherUser_ThrowsArgumentException()
    {
        var account = CreateAccount(Guid.NewGuid());

        Action act = () => CreateTransaction(account: account);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithCategoryOwnedByAnotherUser_ThrowsArgumentException()
    {
        var category = CreateCategory(Guid.NewGuid(), CategoryType.Expense);

        Action act = () => CreateTransaction(category: category);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithInactiveAccount_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId);
        account.Deactivate();

        Action act = () => CreateTransaction(userId: userId, account: account);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Constructor_WithInactiveCategory_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId, CategoryType.Expense);
        category.Deactivate();

        Action act = () => CreateTransaction(userId: userId, category: category);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Constructor_WithUnsupportedType_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => CreateTransaction(type: (TransactionType)999);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Theory]
    [InlineData(TransactionType.Income, CategoryType.Expense)]
    [InlineData(TransactionType.Expense, CategoryType.Income)]
    public void Constructor_WithMismatchedCategoryType_ThrowsArgumentException(
        TransactionType transactionType,
        CategoryType categoryType)
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId, categoryType);

        Action act = () => CreateTransaction(
            userId: userId,
            category: category,
            type: transactionType);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithDefaultDate_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => CreateTransaction(transactionDate: default(DateOnly));

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    private static BudgetTransaction CreateTransaction(
        Guid? id = null,
        Guid? userId = null,
        Account? account = null,
        Category? category = null,
        Money? amount = null,
        TransactionType type = TransactionType.Expense,
        DateOnly? transactionDate = null,
        string? description = "Synthetic transaction")
    {
        var resolvedUserId = userId ?? Guid.NewGuid();

        return new BudgetTransaction(
            id ?? Guid.NewGuid(),
            resolvedUserId,
            account ?? CreateAccount(resolvedUserId),
            category ?? CreateCategory(resolvedUserId, CategoryType.Expense),
            amount ?? new Money(100m, CurrencyCode.TRY),
            type,
            transactionDate ?? new DateOnly(2026, 8, 7),
            description);
    }

    private static Account CreateAccount(Guid userId)
    {
        return new Account(
            Guid.NewGuid(),
            userId,
            "Daily Cash",
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
}
