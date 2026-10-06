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

        var budget = new MonthlyBudget(id, userId, category, limit, TransactionScope.Business, 2026, 2);

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
            TransactionScope.Business,
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
            TransactionScope.Business,
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
            TransactionScope.Business,
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
            TransactionScope.Business,
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
}
