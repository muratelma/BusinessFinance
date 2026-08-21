using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithValidValues_PreservesValuesAndStartsActive()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var category = new Category(
            id,
            userId,
            "  Groceries  ",
            CategoryType.Expense);

        Assert.Equal(id, category.Id);
        Assert.Equal(userId, category.UserId);
        Assert.Equal("Groceries", category.Name);
        Assert.Equal(CategoryType.Expense, category.Type);
        Assert.True(category.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        Action act = () => CreateCategory(id: Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsArgumentException()
    {
        Action act = () => CreateCategory(userId: Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string? name)
    {
        Action act = () => CreateCategory(name: name!);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithUnsupportedType_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => CreateCategory(type: (CategoryType)999);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Constructor_WithTooLongName_ThrowsArgumentException()
    {
        Action act = () => CreateCategory(name: new string('C', Category.MaximumNameLength + 1));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Deactivate_ThenActivate_UpdatesActiveStatus()
    {
        var category = CreateCategory();

        category.Deactivate();
        Assert.False(category.IsActive);

        category.Activate();
        Assert.True(category.IsActive);
    }

    private static Category CreateCategory(
        Guid? id = null,
        Guid? userId = null,
        string name = "Groceries",
        CategoryType type = CategoryType.Expense)
    {
        return new Category(
            id ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid(),
            name,
            type);
    }
}
