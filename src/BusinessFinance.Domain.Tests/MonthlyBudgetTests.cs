using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public class MonthlyBudgetTests
{
    [Fact]
    public void Constructor_WithValidValues_PreservesValuesAndCalculatesPeriod()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId);
        var limit = new Money(1_000m, CurrencyCode.TRY);

        var budget = new MonthlyBudget(id, userId, category, limit, 2026, 2);

        Assert.Equal(id, budget.Id);
        Assert.Equal(userId, budget.UserId);
        Assert.Equal(category.Id, budget.CategoryId);
        Assert.Equal(limit, budget.Limit);
        Assert.Equal(2026, budget.Year);
        Assert.Equal(2, budget.Month);
        Assert.Equal(new DateOnly(2026, 2, 1), budget.PeriodStart);
        Assert.Equal(new DateOnly(2026, 2, 28), budget.PeriodEnd);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        Action act = () => CreateBudget(id: Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsArgumentException()
    {
        var ownerId = Guid.NewGuid();

        Action act = () => new MonthlyBudget(
            Guid.NewGuid(),
            Guid.Empty,
            CreateCategory(ownerId),
            new Money(1_000m, CurrencyCode.TRY),
            2026,
            8);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithNullCategory_ThrowsArgumentNullException()
    {
        Action act = () => new MonthlyBudget(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null!,
            new Money(1_000m, CurrencyCode.TRY),
            2026,
            8);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithNullLimit_ThrowsArgumentNullException()
    {
        var userId = Guid.NewGuid();

        Action act = () => new MonthlyBudget(
            Guid.NewGuid(),
            userId,
            CreateCategory(userId),
            null!,
            2026,
            8);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithCategoryOwnedByAnotherUser_ThrowsArgumentException()
    {
        var category = CreateCategory(Guid.NewGuid());

        Action act = () => CreateBudget(category: category);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithInactiveCategory_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId);
        category.Deactivate();

        Action act = () => CreateBudget(userId: userId, category: category);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Constructor_WithIncomeCategory_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId, CategoryType.Income);

        Action act = () => CreateBudget(userId: userId, category: category);

        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Constructor_WithInvalidMonth_ThrowsArgumentOutOfRangeException(int month)
    {
        Action act = () => CreateBudget(month: month);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Theory]
    [InlineData(MonthlyBudget.MinimumYear - 1)]
    [InlineData(MonthlyBudget.MaximumYear + 1)]
    public void Constructor_WithInvalidYear_ThrowsArgumentOutOfRangeException(int year)
    {
        Action act = () => CreateBudget(year: year);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Theory]
    [InlineData(MonthlyBudget.MinimumYear)]
    [InlineData(MonthlyBudget.MaximumYear)]
    public void Constructor_WithBoundaryYear_AcceptsYear(int year)
    {
        var budget = CreateBudget(year: year);

        Assert.Equal(year, budget.Year);
    }

    [Fact]
    public void CalculateProgress_WithNoTransactions_ReturnsFullRemainingAmount()
    {
        var budget = CreateBudget(limit: new Money(1_000m, CurrencyCode.TRY));

        var progress = budget.CalculateProgress([]);

        Assert.Equal(0m, progress.SpentAmount);
        Assert.Equal(1_000m, progress.RemainingAmount);
        Assert.Equal(0m, progress.ExceededAmount);
        Assert.False(progress.IsExceeded);
    }

    [Fact]
    public void CalculateProgress_WithMatchingTransactions_ReturnsSpentAndRemainingAmounts()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId);
        var budget = CreateBudget(
            userId: userId,
            category: category,
            limit: new Money(1_000m, CurrencyCode.TRY));
        var transactions = new[]
        {
            CreateTransaction(userId, category, 250m, new DateOnly(2026, 8, 1)),
            CreateTransaction(userId, category, 300m, new DateOnly(2026, 8, 31))
        };

        var progress = budget.CalculateProgress(transactions);

        Assert.Equal(550m, progress.SpentAmount);
        Assert.Equal(450m, progress.RemainingAmount);
        Assert.Equal(0m, progress.ExceededAmount);
        Assert.False(progress.IsExceeded);
        Assert.Equal(CurrencyCode.TRY, progress.Currency);
    }

    [Fact]
    public void CalculateProgress_WithOverspending_ReturnsExceededAmount()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId);
        var budget = CreateBudget(
            userId: userId,
            category: category,
            limit: new Money(500m, CurrencyCode.TRY));
        var transaction = CreateTransaction(
            userId,
            category,
            650m,
            new DateOnly(2026, 8, 15));

        var progress = budget.CalculateProgress([transaction]);

        Assert.Equal(650m, progress.SpentAmount);
        Assert.Equal(0m, progress.RemainingAmount);
        Assert.Equal(150m, progress.ExceededAmount);
        Assert.True(progress.IsExceeded);
    }

    [Fact]
    public void CalculateProgress_IgnoresTransactionsOutsideCategoryOrPeriod()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId);
        var otherCategory = CreateCategory(userId);
        var budget = CreateBudget(userId: userId, category: category);
        var transactions = new[]
        {
            CreateTransaction(userId, category, 100m, new DateOnly(2026, 7, 31)),
            CreateTransaction(userId, category, 100m, new DateOnly(2026, 9, 1)),
            CreateTransaction(userId, otherCategory, 100m, new DateOnly(2026, 8, 15))
        };

        var progress = budget.CalculateProgress(transactions);

        Assert.Equal(0m, progress.SpentAmount);
        Assert.Equal(1_000m, progress.RemainingAmount);
    }

    [Fact]
    public void CalculateProgress_IgnoresCancelledTransactions()
    {
        var userId = Guid.NewGuid();
        var category = CreateCategory(userId);
        var budget = CreateBudget(userId: userId, category: category);
        var transaction = CreateTransaction(
            userId,
            category,
            400m,
            new DateOnly(2026, 8, 15));
        transaction.Cancel(new DateTimeOffset(2026, 8, 16, 10, 0, 0, TimeSpan.Zero));

        var progress = budget.CalculateProgress([transaction]);

        Assert.Equal(0m, progress.SpentAmount);
        Assert.Equal(1_000m, progress.RemainingAmount);
        Assert.Equal(0m, progress.ExceededAmount);
        Assert.False(progress.IsExceeded);
    }

    [Fact]
    public void CalculateProgress_WithNullTransactions_ThrowsArgumentNullException()
    {
        var budget = CreateBudget();

        Action act = () => budget.CalculateProgress(null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    private static MonthlyBudget CreateBudget(
        Guid? id = null,
        Guid? userId = null,
        Category? category = null,
        Money? limit = null,
        int year = 2026,
        int month = 8)
    {
        var resolvedUserId = userId ?? Guid.NewGuid();

        return new MonthlyBudget(
            id ?? Guid.NewGuid(),
            resolvedUserId,
            category ?? CreateCategory(resolvedUserId),
            limit ?? new Money(1_000m, CurrencyCode.TRY),
            year,
            month);
    }

    private static Category CreateCategory(
        Guid userId,
        CategoryType type = CategoryType.Expense)
    {
        return new Category(
            Guid.NewGuid(),
            userId,
            "Synthetic Category",
            type);
    }

    private static BudgetTransaction CreateTransaction(
        Guid userId,
        Category category,
        decimal amount,
        DateOnly date)
    {
        var account = new Account(
            Guid.NewGuid(),
            userId,
            "Daily Cash",
            AccountType.Cash,
            CurrencyCode.TRY);

        return new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            account,
            category,
            new Money(amount, CurrencyCode.TRY),
            TransactionType.Expense,
            date);
    }
}
