using BusinessFinance.Application.Counterparties;

namespace BusinessFinance.Application.Tests.Counterparties;

/// <summary>
/// Kişinin kartında yazılan iki tutar eksiye düşmez; fazla tahsilat bizim
/// borcumuza, fazla ödeme karşı tarafın borcuna geçer ve fark her zaman nete
/// eşittir.
/// </summary>
public sealed class CounterpartyDisplayTests
{
    [Theory]
    // Olağan: iki taraf da artı.
    [InlineData(400, 0, 400, 0)]
    [InlineData(0, 250, 0, 250)]
    [InlineData(400, 250, 400, 250)]
    // 2.200 liralık satışa 2.500 liralık tahsilat: 300 lira bizim borcumuz.
    [InlineData(-300, 0, 0, 300)]
    // Fazla tahsilat + vadeli alım borcu birlikte.
    [InlineData(-300, 200, 0, 500)]
    // Fazla ödeme: karşı taraf bize borçlu.
    [InlineData(0, -150, 150, 0)]
    [InlineData(100, -150, 250, 0)]
    // İki taraf da fazla: yer değiştirirler.
    [InlineData(-50, -80, 80, 50)]
    [InlineData(0, 0, 0, 0)]
    public void DisplayedSidesNeverGoNegative_AndTheirDifferenceIsTheNet(
        decimal receivable,
        decimal payable,
        decimal owedToYou,
        decimal owedByYou)
    {
        var summary = new CounterpartyBalanceSummary(
            Guid.NewGuid(), "Kişi", true, receivable, payable, 0m, 0m);

        Assert.Equal(owedToYou, summary.OwedToYou);
        Assert.Equal(owedByYou, summary.OwedByYou);
        Assert.Equal(summary.Net, summary.OwedToYou - summary.OwedByYou);
        Assert.True(summary.OwedToYou >= 0m && summary.OwedByYou >= 0m);
    }
}
