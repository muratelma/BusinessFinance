using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class CreditCardStatementTests
{
    [Fact]
    public void Period_WhenDueDayIsAfterClosing_UsesSameMonthDueDate()
    {
        var period = CreditCardStatementPeriod.ForClosingMonth(CreateCard(10, 20), 2026, 5);

        Assert.Equal(new DateOnly(2026, 4, 11), period.PeriodStart);
        Assert.Equal(new DateOnly(2026, 5, 10), period.ClosingDate);
        Assert.Equal(new DateOnly(2026, 5, 20), period.DueDate);
    }

    [Fact]
    public void Period_WhenDueDayIsNotAfterClosing_UsesNextMonthDueDate()
    {
        var period = CreditCardStatementPeriod.ForClosingMonth(CreateCard(20, 10), 2026, 5);

        Assert.Equal(new DateOnly(2026, 6, 10), period.DueDate);
    }

    [Theory]
    [InlineData("2026-05-15", 100, "Open")]
    [InlineData("2026-05-21", 100, "Overdue")]
    [InlineData("2026-05-20", 350, "Paid")]
    public void Create_CalculatesCarryRemainingAndPaymentStatus(
        string asOf,
        decimal paymentsAfterClosing,
        string expectedStatus)
    {
        var statement = CreditCardStatement.Create(
            CreateCard(10, 20),
            2026,
            5,
            DateOnly.Parse(asOf),
            previousBalance: 100m,
            periodCharges: 300m,
            paymentsThroughClosing: 50m,
            paymentsAfterClosing: paymentsAfterClosing);

        Assert.Equal(350m, statement.StatementBalance);
        Assert.Equal(Math.Max(0m, 350m - paymentsAfterClosing), statement.RemainingBalance);
        Assert.Equal(expectedStatus, statement.PaymentStatus.ToString());
    }

    /// <summary>
    /// Aşama 06 Grup 5: geçen dönemden kalan alacaklı bakiye devreder.
    /// </summary>
    /// <remarks>
    /// Devir kırpılsaydı, kartında parası olan kullanıcıdan bu dönemin
    /// harcamalarının tamamı isteniyordu — borçlu olmadığı bir tutar.
    /// </remarks>
    [Fact]
    public void Create_WhenPreviousPeriodWasOverpaid_CarriesTheCreditForward()
    {
        var statement = CreditCardStatement.Create(
            CreateCard(10, 20),
            2026,
            5,
            new DateOnly(2026, 5, 15),
            previousBalance: -100m,
            periodCharges: 300m,
            paymentsThroughClosing: 0m,
            paymentsAfterClosing: 0m);

        Assert.Equal(200m, statement.StatementBalance);
        Assert.Equal(200m, statement.RemainingBalance);
    }

    /// <summary>
    /// Ödenecek tutarın tabanı **duruyor**: devreden alacak dönem
    /// harcamasından büyük olsa bile ekstre negatif borç göstermez.
    /// "Bu ay ne kadar ödemeliyim" sorusunun cevabı eksi olamaz; kartın
    /// alacaklı bakiyesi ayrı bir sorunun cevabıdır ve orada kırpılmaz.
    /// </summary>
    [Fact]
    public void Create_WhenTheCreditExceedsThePeriod_StillNeverAsksForANegativeAmount()
    {
        var statement = CreditCardStatement.Create(
            CreateCard(10, 20),
            2026,
            5,
            new DateOnly(2026, 5, 15),
            previousBalance: -500m,
            periodCharges: 300m,
            paymentsThroughClosing: 0m,
            paymentsAfterClosing: 0m);

        Assert.Equal(0m, statement.StatementBalance);
        Assert.Equal(0m, statement.RemainingBalance);
        Assert.Equal(0m, statement.MinimumPayment);
        Assert.Equal("Paid", statement.PaymentStatus.ToString());
    }

    [Fact]
    public void Create_BeforeClosingDate_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreditCardStatement.Create(
            CreateCard(10, 20), 2026, 5, new DateOnly(2026, 5, 9), 0m, 0m, 0m, 0m));
    }

    [Fact]
    public void Create_DerivesMinimumPaymentFromTheCardRate()
    {
        var statement = CreditCardStatement.Create(
            CreateCard(10, 20, minimumPaymentRate: 40m),
            2026,
            5,
            new DateOnly(2026, 5, 15),
            previousBalance: 100m,
            periodCharges: 300m,
            paymentsThroughClosing: 50m,
            paymentsAfterClosing: 0m);

        Assert.Equal(350m, statement.StatementBalance);
        Assert.Equal(140m, statement.MinimumPayment);
        Assert.Equal(140m, statement.RemainingMinimumPayment);
        Assert.Equal(40m, statement.MinimumPaymentRate);
    }

    [Fact]
    public void Create_ShrinksTheMinimumByPaymentsMadeAfterClosing()
    {
        // Asgarinin bir kısmını ödeyen kullanıcı, ekranda hâlâ tam asgariyi
        // borçluymuş gibi görünmemeli.
        var statement = CreditCardStatement.Create(
            CreateCard(10, 20),
            2026,
            5,
            new DateOnly(2026, 5, 15),
            previousBalance: 0m,
            periodCharges: 1000m,
            paymentsThroughClosing: 0m,
            paymentsAfterClosing: 150m);

        Assert.Equal(200m, statement.MinimumPayment);
        Assert.Equal(50m, statement.RemainingMinimumPayment);
    }

    [Fact]
    public void Create_StopsAskingForTheMinimumOnceItIsCovered()
    {
        // Asgarinin çok üstünde ödeme yapılmış: borç bitmedi ama asgari
        // yükümlülüğü bitti. İkisi ayrı sorular, aynı sayı değil.
        var statement = CreditCardStatement.Create(
            CreateCard(10, 20),
            2026,
            5,
            new DateOnly(2026, 5, 15),
            previousBalance: 0m,
            periodCharges: 1000m,
            paymentsThroughClosing: 0m,
            paymentsAfterClosing: 950m);

        Assert.Equal(50m, statement.RemainingBalance);
        Assert.Equal(0m, statement.RemainingMinimumPayment);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(150)]
    [InlineData(950)]
    [InlineData(1000)]
    public void Create_KeepsTheRemainingMinimumWithinTheRemainingBalance(
        decimal paymentsAfterClosing)
    {
        // Ekranda "kalan 50, asgari 200" gibi bir çift asla çıkmamalı.
        // Bunu ayrı bir sınırla değil, asgarinin ekstre borcunu aşamaması
        // sağlıyor; kural bozulursa burası düşer.
        var statement = CreditCardStatement.Create(
            CreateCard(10, 20),
            2026,
            5,
            new DateOnly(2026, 5, 15),
            previousBalance: 0m,
            periodCharges: 1000m,
            paymentsThroughClosing: 0m,
            paymentsAfterClosing: paymentsAfterClosing);

        Assert.True(statement.RemainingMinimumPayment <= statement.RemainingBalance);
    }

    [Fact]
    public void MinimumPayment_RoundsUpAndNeverExceedsTheBalance()
    {
        // Aşağı yuvarlama, "asgariyi ödedim" diyen kullanıcıyı bir kuruş
        // eksikle gecikmeye düşürürdü. %100 oranda ise tutar borcu aşamaz.
        var card = CreateCard(10, 20, minimumPaymentRate: 20m);
        Assert.Equal(0.07m, card.CalculateMinimumPayment(0.33m));

        var full = CreateCard(10, 20, minimumPaymentRate: 100m);
        Assert.Equal(0.33m, full.CalculateMinimumPayment(0.33m));
    }

    [Fact]
    public void MinimumPayment_OnASettledStatement_IsZero()
    {
        Assert.Equal(0m, CreateCard(10, 20).CalculateMinimumPayment(0m));
    }

    [Fact]
    public void Card_RejectsARateOutsideTheAllowedRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CreateCard(10, 20, minimumPaymentRate: 100.01m));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CreateCard(10, 20, minimumPaymentRate: -1m));
    }

    private static CreditCard CreateCard(
        int closingDay,
        int dueDay,
        decimal minimumPaymentRate = CreditCard.DefaultMinimumPaymentRate) => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Card",
        new Money(1000m, CurrencyCode.TRY),
        closingDay,
        dueDay,
        minimumPaymentRate);
}
