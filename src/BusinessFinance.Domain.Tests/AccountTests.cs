using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public class AccountTests
{
    [Fact]
    public void Constructor_WithOpeningBalance_PreservesNonNegativeValue()
    {
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Synthetic Cash",
            AccountType.Cash,
            CurrencyCode.TRY,
            1250.75m);

        Assert.Equal(1250.75m, account.OpeningBalance);
    }

    [Fact]
    public void Constructor_WithNegativeOpeningBalance_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Synthetic Cash",
            AccountType.Cash,
            CurrencyCode.TRY,
            -0.01m);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
    [Fact]
    public void Constructor_WithValidValues_PreservesValuesAndStartsActive()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var account = new Account(
            id,
            userId,
            "  Daily Cash  ",
            AccountType.Cash,
            CurrencyCode.TRY);

        Assert.Equal(id, account.Id);
        Assert.Equal(userId, account.UserId);
        Assert.Equal("Daily Cash", account.Name);
        Assert.Equal(AccountType.Cash, account.Type);
        Assert.Equal(CurrencyCode.TRY, account.Currency);
        Assert.True(account.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        Action act = () => CreateAccount(id: Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsArgumentException()
    {
        Action act = () => CreateAccount(userId: Guid.Empty);

        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string? name)
    {
        Action act = () => CreateAccount(name: name!);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithUnsupportedType_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => CreateAccount(type: (AccountType)999);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Constructor_WithTooLongName_ThrowsArgumentException()
    {
        Action act = () => CreateAccount(name: new string('A', Account.MaximumNameLength + 1));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithUnsupportedCurrency_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => CreateAccount(currency: (CurrencyCode)999);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Deactivate_ThenActivate_UpdatesActiveStatus()
    {
        var account = CreateAccount();

        account.Deactivate();
        Assert.False(account.IsActive);

        account.Activate();
        Assert.True(account.IsActive);
    }

    private static Account CreateAccount(
        Guid? id = null,
        Guid? userId = null,
        string name = "Daily Cash",
        AccountType type = AccountType.Cash,
        CurrencyCode currency = CurrencyCode.TRY)
    {
        return new Account(
            id ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid(),
            name,
            type,
            currency);
    }
}
