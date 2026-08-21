using BusinessFinance.Domain;
namespace BusinessFinance.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidValues_PreservesValues()
    {
        // Arrange
        const decimal amount = 125.50m;
        const CurrencyCode currency = CurrencyCode.TRY;

        // Act
        var money = new Money(amount, currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveAmount_ThrowsArgumentOutOfRangeException(decimal amount)
    {
        // Act
        Action act = () => new Money(amount, CurrencyCode.TRY);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Constructor_WithUnsupportedCurrency_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const decimal amount = 100m;
        const CurrencyCode currency = (CurrencyCode)999;

        // Act
        Action act = () => new Money(amount, currency);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void TwoMoneyInstances_WithSameValues_AreEqual()
    {
        // Arrange
        var first = new Money(100m, CurrencyCode.TRY);
        var second = new Money(100m, CurrencyCode.TRY);

        // Act
        var areEqual = first == second;

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Add_WithSameCurrency_ReturnsSummedMoney()
    {
        // Arrange
        var first = new Money(100m, CurrencyCode.TRY);
        var second = new Money(50m, CurrencyCode.TRY);

        // Act
        var result = first.Add(second);

        // Assert
        Assert.Equal(new Money(150m, CurrencyCode.TRY), result);
    }

    [Fact]
    public void Add_WithNullMoney_ThrowsArgumentNullException()
    {
        // Arrange
        var money = new Money(100m, CurrencyCode.TRY);

        // Act
        Action act = () => money.Add(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

}
