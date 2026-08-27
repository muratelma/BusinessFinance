using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class CreditCardTests
{
    [Fact]
    public void Constructor_WithValidValues_PreservesTermsAndCalculatesAvailableLimit()
    {
        var card = CreateCard();

        Assert.Equal("Main Card", card.Name);
        Assert.Equal(new Money(10000m, CurrencyCode.TRY), card.Limit);
        Assert.Equal(10, card.StatementClosingDay);
        Assert.Equal(20, card.PaymentDueDay);
        Assert.True(card.IsActive);
        Assert.Equal(7500m, card.CalculateAvailableLimit(2500m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(29)]
    public void Constructor_WithUnsafeCycleDay_ThrowsArgumentOutOfRangeException(int day)
    {
        Action closing = () => CreateCard(statementClosingDay: day);
        Action due = () => CreateCard(paymentDueDay: day);

        Assert.Throws<ArgumentOutOfRangeException>(closing);
        Assert.Throws<ArgumentOutOfRangeException>(due);
    }

    [Fact]
    public void Update_ChangesTermsAndActiveState()
    {
        var card = CreateCard();

        card.Update(
            "  Reserve Card  ",
            new Money(20000m, CurrencyCode.TRY),
            12,
            24,
            40m,
            false);

        Assert.Equal("Reserve Card", card.Name);
        Assert.Equal(20000m, card.Limit.Amount);
        Assert.Equal(12, card.StatementClosingDay);
        Assert.Equal(24, card.PaymentDueDay);
        Assert.False(card.IsActive);
    }

    /// <summary>
    /// Aşama 06 Grup 5: taban yalnız tek uçta duruyor.
    /// </summary>
    /// <remarks>
    /// Limiti aşmış kartta kullanılabilir tutar sıfırdır — "eksi 2.000
    /// harcayabilirsiniz" diye bir şey yok. Ters uçta ise kırpma <b>kalktı</b>:
    /// kartın alacaklı bakiyesi gerçek bir harcama alanıdır, para karttadır.
    /// Eskiden negatif borç exception atıyordu ve bu, alacaklı bakiyenin
    /// hiçbir yolda temsil edilememesinin sebebiydi.
    /// </remarks>
    [Fact]
    public void CalculateAvailableLimit_ClampsAtZeroButLetsACreditBalanceRaiseIt()
    {
        var card = CreateCard();

        Assert.Equal(0m, card.CalculateAvailableLimit(12000m));
        Assert.Equal(10500m, card.CalculateAvailableLimit(-500m));
    }

    private static CreditCard CreateCard(
        int statementClosingDay = 10,
        int paymentDueDay = 20) => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "  Main Card  ",
        new Money(10000m, CurrencyCode.TRY),
        statementClosingDay,
        paymentDueDay);
}
