using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 05 Grup 2: KDV taşınan bir bilgidir; uygulama hiçbir vergi tutarını
/// hesaplamaz veya türetmez (ADR 0016).
/// </summary>
public sealed class VatDetailsTests
{
    /// <summary>
    /// Grubun çıkış ölçütü: KDV alanı boş bırakılabilir ve boş olması eksik
    /// veri değildir.
    /// </summary>
    [Fact]
    public void Vat_IsOptional_AndTheAbsenceIsASingleRepresentation()
    {
        Assert.Null(VatDetails.FromOptional(rate: null, amount: null));

        // İçi boş bir nesne "KDV yok"un ikinci bir anlatımı olurdu; kabul
        // edilmiyor.
        Assert.Throws<ArgumentException>(() => new VatDetails(rate: null, amount: null));
    }

    /// <summary>
    /// Oran ve tutar ayrı ayrı taşınır: biri girilip diğeri boş bırakılabilir
    /// ve boş kalan alan <b>doldurulmaz</b>.
    /// </summary>
    [Fact]
    public void RateAndAmount_AreCarriedIndependently_AndNeitherIsDerivedFromTheOther()
    {
        var onlyRate = VatDetails.FromOptional(rate: 0.20m, amount: null);
        Assert.NotNull(onlyRate);
        Assert.Equal(0.20m, onlyRate.Rate);
        Assert.Null(onlyRate.Amount);

        var onlyAmount = VatDetails.FromOptional(rate: null, amount: 166.6667m);
        Assert.NotNull(onlyAmount);
        Assert.Null(onlyAmount.Rate);
        Assert.Equal(166.6667m, onlyAmount.Amount);
    }

    /// <summary>
    /// Oranla tutar birbirini tutmasa bile kayıt <b>reddedilmez ve
    /// düzeltilmez</b>: belge ne diyorsa doğru olan odur (ADR 0016). Farklı
    /// oranlı kalemler tek belgede toplandığında beklenen durum budur.
    /// </summary>
    [Fact]
    public void MismatchedRateAndAmount_AreKeptAsWritten()
    {
        var vat = new VatDetails(rate: 0.20m, amount: 10m);

        Assert.Equal(0.20m, vat.Rate);
        Assert.Equal(10m, vat.Amount);
    }

    /// <summary>
    /// Uyarı için hesaplanan tutar hiçbir alana yazılmaz; yalnız sorulduğunda
    /// döner. Brüt tutar KDV'yi içerir, bu yüzden bölme <c>tutar × oran ÷
    /// (1 + oran)</c>'dır.
    /// </summary>
    [Fact]
    public void ImpliedAmount_IsAWarningOnly_AndNeverFillsTheCarriedAmount()
    {
        var gross = new Money(1200m, CurrencyCode.TRY);

        Assert.Equal(200m, VatDetails.ImpliedAmount(gross, 0.20m));

        // Kullanıcı hiçbir tutar yazmadıysa alan boş kalır: uyarı için
        // hesaplanan sayı kaydın alanına sızmaz.
        var vat = VatDetails.FromOptional(rate: 0.20m, amount: null);
        Assert.NotNull(vat);
        Assert.Null(vat.Amount);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(1)]
    [InlineData(1.5)]
    [InlineData(0.20001)]
    public void Rate_OutsideItsBounds_IsRejected(decimal rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new VatDetails(rate, amount: null));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0.00001)]
    public void Amount_OutsideItsBounds_IsRejected(decimal amount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new VatDetails(rate: null, amount));
    }

    /// <summary>
    /// Sıfır oran ve sıfır tutar meşrudur: istisna kapsamındaki belgede KDV
    /// sıfırdır ve bu "bilinmiyor" değildir.
    /// </summary>
    [Fact]
    public void ZeroRateAndZeroAmount_AreLegitimate()
    {
        var vat = new VatDetails(rate: 0m, amount: 0m);

        Assert.Equal(0m, vat.Rate);
        Assert.Equal(0m, vat.Amount);
    }

    /// <summary>
    /// Tutar kaydın tutarını aşamaz: bu bir hesaplama değil, sınırdır.
    /// </summary>
    [Fact]
    public void Amount_CannotExceedTheRecordItSitsOn()
    {
        var amount = new Money(100m, CurrencyCode.TRY);

        Assert.Throws<ArgumentOutOfRangeException>(() => VatDetails.EnsureWithinAmount(
            new VatDetails(rate: null, amount: 100.01m), amount, "vat"));

        // Sınırın kendisi kabul edilir ve boş KDV hiçbir şey sormaz.
        VatDetails.EnsureWithinAmount(new VatDetails(null, 100m), amount, "vat");
        VatDetails.EnsureWithinAmount(null, amount, "vat");
    }
}
