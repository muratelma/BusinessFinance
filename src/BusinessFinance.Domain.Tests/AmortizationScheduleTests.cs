using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class AmortizationScheduleTests
{
    [Fact]
    public void ZeroRate_LeavesTotalEqualToPrincipal()
    {
        Assert.Equal(300m, AmortizationSchedule.TotalRepaymentFor(300m, 0m, 3));
        Assert.Equal(0m, AmortizationSchedule.AnnualInterestRateFor(300m, 300m, 3));
    }

    [Fact]
    public void AnnuityCostsLessThanSimpleInterest()
    {
        // Modelin seçilme sebebi bu farktır. Basit faiz 300 TL'nin tamamına üç
        // ay boyunca faiz işletir: 300 × (1 + 0,10 × 3/12) = 307,50. Anüite
        // faizi azalan bakiyeye işlettiği için daha azını ister. İki model
        // karışırsa bu iddia düşer.
        var total = AmortizationSchedule.TotalRepaymentFor(300m, 10m, 3);

        Assert.True(total > 300m, $"Faizli borç anaparadan büyük olmalı, {total} bulundu.");
        Assert.True(total < 307.50m, $"Anüite basit faizden ucuz olmalı, {total} bulundu.");
        Assert.Equal(305.0138m, total);
    }

    [Fact]
    public void SingleInstallment_ChargesExactlyOneMonthOfInterest()
    {
        // Tek taksitte anüite formülü sadeleşir: ödeme = anapara × (1 + i).
        // %12 yıllık, aylık %1 → 100 TL için 101 TL.
        Assert.Equal(101m, AmortizationSchedule.TotalRepaymentFor(100m, 12m, 1));
    }

    [Fact]
    public void LongestTermAtHighestRate_DoesNotOverflow()
    {
        // (1+i)^n bu uçta 10^94 civarındadır ve decimal taşar; hesap bu yüzden
        // 1/(1+i) çarpanını tekrarlar. Bu test o kararı korur.
        var total = AmortizationSchedule.TotalRepaymentFor(
            1000m, AmortizationSchedule.MaximumAnnualInterestRate, 360);

        Assert.True(total > 1000m);
        Assert.Equal(total, decimal.Round(total, 4));
    }

    [Theory]
    [InlineData(300, 10, 3)]
    [InlineData(1000, 24, 12)]
    [InlineData(50000, 45.75, 36)]
    [InlineData(750.25, 0.5, 6)]
    [InlineData(100000, 999.9999, 360)]
    public void RateSurvivesTheRoundTrip(decimal principal, decimal rate, int count)
    {
        var total = AmortizationSchedule.TotalRepaymentFor(principal, rate, count);
        var recovered = AmortizationSchedule.AnnualInterestRateFor(principal, total, count);

        // Dönüş birebir değil: toplam dört ondalığa yuvarlanınca oranın son
        // basamağı kaybolur. Kayıp yuvarlamanın kendisinden gelir, çözücüden
        // değil — bu yüzden eşitlik değil pay aranır.
        Assert.True(
            Math.Abs(recovered - rate) < 0.01m,
            $"Oran {rate} girildi, {recovered} geri geldi.");
    }

    [Fact]
    public void SolvedRate_ReproducesTheTotalTheUserTyped()
    {
        // Kullanıcının senaryosu: anapara 300, toplam 400, dört taksit. Oranı
        // biz buluyoruz ve o oran aynı toplamı geri vermeli.
        var rate = AmortizationSchedule.AnnualInterestRateFor(300m, 400m, 4);
        var total = AmortizationSchedule.TotalRepaymentFor(300m, rate, 4);

        Assert.Equal(151.0780m, rate);

        // Kullanıcının yazdığı toplam birebir geri geliyor. Para kanonik,
        // oran türetilmiş olduğu için zaten plan bu toplamdan bölünecek —
        // ama çevirimin de aynı sayıyı vermesi tasarımın kendini tutması
        // demektir.
        Assert.Equal(400m, total);
    }

    [Fact]
    public void UnreachableTotal_IsRejectedInsteadOfClampedSilently()
    {
        // Üst sınırın üstündeki bir toplam sessizce %1000'e kırpılırsa
        // kullanıcı yazdığından farklı bir borç kaydeder.
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AmortizationSchedule.AnnualInterestRateFor(300m, 30000m, 3));
    }

    [Fact]
    public void TotalBelowPrincipal_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AmortizationSchedule.AnnualInterestRateFor(300m, 299m, 3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(361)]
    public void InstallmentCountOutsideDomainLimits_IsRejected(int count)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AmortizationSchedule.TotalRepaymentFor(300m, 10m, count));
    }

    [Theory]
    [InlineData(-0.0001)]
    [InlineData(1000.0001)]
    public void RateOutsideDomainLimits_IsRejected(decimal rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AmortizationSchedule.TotalRepaymentFor(300m, rate, 3));
    }

    [Fact]
    public void Split_AddsUpToPrincipalAndInterestExactly()
    {
        // Kuruş kaçarsa borç kapandığında anapara kapanmamış görünür.
        var principal = 1000m;
        var rate = 24m;
        var total = AmortizationSchedule.TotalRepaymentFor(principal, rate, 12);
        var amounts = EqualInstallments(total, 12);

        var splits = AmortizationSchedule.Split(principal, rate, amounts);

        Assert.Equal(principal, splits.Sum(x => x.Principal));
        Assert.Equal(total - principal, splits.Sum(x => x.Interest));
        Assert.Equal(total, splits.Sum(x => x.Principal + x.Interest));
    }

    [Fact]
    public void Split_ShiftsFromInterestToPrincipalOverTime()
    {
        // Anüitenin tanımı: faiz azalan bakiyeye işler, dolayısıyla ilk taksit
        // en çok faizi, son taksit en çok anaparayı taşır. Bu tersine dönerse
        // hesap anüite değil başka bir şeydir.
        var total = AmortizationSchedule.TotalRepaymentFor(1000m, 24m, 12);
        var splits = AmortizationSchedule.Split(1000m, 24m, EqualInstallments(total, 12));

        Assert.True(splits[0].Interest > splits[^1].Interest);
        Assert.True(splits[0].Principal < splits[^1].Principal);
        for (var index = 1; index < splits.Count; index++)
        {
            Assert.True(
                splits[index].Interest <= splits[index - 1].Interest,
                $"{index}. taksitin faizi bir öncekinden büyük çıktı.");
        }
    }

    [Fact]
    public void Split_WithoutInterest_IsAllPrincipal()
    {
        var splits = AmortizationSchedule.Split(300m, 0m, EqualInstallments(300m, 3));

        Assert.All(splits, split => Assert.Equal(0m, split.Interest));
        Assert.Equal(300m, splits.Sum(x => x.Principal));
    }

    [Fact]
    public void Split_RejectsScheduleThatCannotCoverPrincipal()
    {
        Assert.Throws<ArgumentException>(
            () => AmortizationSchedule.Split(300m, 10m, [100m, 100m]));
    }

    // `DebtAgreement.GenerateSchedule` ile aynı bölme: baştakiler aşağı
    // yuvarlanır, sonuncusu farkı üstlenir. Test planı gerçekte üretildiği
    // gibi bölmezse Split'i gerçekte gelmeyecek girdilerle sınar.
    private static decimal[] EqualInstallments(decimal total, int count)
    {
        var baseAmount = decimal.Floor(total / count * 10000m) / 10000m;
        var amounts = new decimal[count];
        for (var index = 0; index < count; index++)
        {
            amounts[index] = index == count - 1
                ? total - baseAmount * (count - 1)
                : baseAmount;
        }

        return amounts;
    }
}
