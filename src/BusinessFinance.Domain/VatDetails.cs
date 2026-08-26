namespace BusinessFinance.Domain;

/// <summary>
/// Bir kaydın üstünde taşınan KDV bilgisi: belgedeki oran ve belgedeki tutar.
/// </summary>
/// <remarks>
/// ADR 0016'nın kaydı. Bu değer nesnesi <b>hiçbir şey hesaplamaz</b>: oranı
/// tutardan, tutarı orandan türetmez. İkisi de belgeden okunan bilgidir ve
/// belge ne diyorsa doğru olan odur.
///
/// Bu, <see cref="PosSettlement.CommissionRate"/> ile bilinçli olarak
/// ayrışır: komisyonda oran paradan çözülür çünkü orayı tek bir banka tek bir
/// oranla keser. Faturada öyle değildir — aynı belgede farklı oranlı kalemler
/// toplanır, yuvarlama farkı belgenin üstünde durur, tevkifatlı ve istisnalı
/// belgeler kuralın tamamen dışındadır. Oranı tutardan çözmek, belgeye
/// bakmadan belge hakkında hüküm vermek olurdu.
///
/// Tutar <see cref="Money"/> değildir çünkü <b>sıfır meşrudur</b>: istisna
/// kapsamındaki bir belgede KDV sıfırdır ve bu eksik veri değildir. Para birimi
/// de taşınmaz — KDV, üstünde durduğu kaydın para birimindedir; ikinci bir
/// birim alanı sessizce ayrışabilecek ikinci bir gerçek olurdu.
///
/// Nesnenin kendisi boş olamaz: KDV yoksa alan <c>null</c>'dır. İçi boş bir
/// nesne "KDV yok"un ikinci bir anlatımı olur ve iki temsil er geç ayrışır.
/// </remarks>
public sealed record VatDetails
{
    /// <summary>Oranın ondalık basamak sayısı (ADR 0009 ve POS komisyonuyla aynı).</summary>
    public const int RateDecimals = 4;

    /// <summary>Para hassasiyeti: her yerde olduğu gibi dört basamak.</summary>
    public const int MoneyDecimals = 4;

    /// <summary>
    /// Belgedeki KDV oranı (0,20 = %20). Belgede yazmıyorsa boştur; boş olması
    /// eksik veri değildir.
    /// </summary>
    public decimal? Rate { get; }

    /// <summary>
    /// Belgedeki KDV tutarı. Kaydın tutarına <b>dâhildir</b>, ona eklenmez:
    /// kayıt tutarı brüttür ve brüt kalır.
    /// </summary>
    public decimal? Amount { get; }

    public VatDetails(decimal? rate, decimal? amount)
    {
        if (rate is null && amount is null)
        {
            throw new ArgumentException(
                "Vat details must carry a rate, an amount, or both; " +
                "a record without vat carries no vat details at all.",
                nameof(rate));
        }

        if (rate is { } rateValue)
        {
            if (rateValue < 0m || rateValue >= 1m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rate),
                    rateValue,
                    "Vat rate must be at least zero and below one.");
            }

            if (decimal.Round(rateValue, RateDecimals) != rateValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rate),
                    rateValue,
                    $"Vat rate cannot carry more than {RateDecimals} decimals.");
            }
        }

        if (amount is { } amountValue)
        {
            if (amountValue < 0m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amountValue,
                    "Vat amount cannot be negative.");
            }

            if (decimal.Round(amountValue, MoneyDecimals) != amountValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amountValue,
                    $"Vat amount cannot carry more than {MoneyDecimals} decimals.");
            }
        }

        Rate = rate;
        Amount = amount;
    }

    /// <summary>
    /// İkisi de boşsa <c>null</c>, değilse bir nesne üretir.
    /// </summary>
    /// <remarks>
    /// Sözleşme ve geri yükleme katmanları iki isteğe bağlı alan görür; "KDV
    /// yok"u tek bir temsile indiren yer burasıdır.
    /// </remarks>
    public static VatDetails? FromOptional(decimal? rate, decimal? amount) =>
        rate is null && amount is null ? null : new VatDetails(rate, amount);

    /// <summary>
    /// KDV tutarı kaydın tutarını aşamaz.
    /// </summary>
    /// <remarks>
    /// Bu bir hesaplama değil, bir sınırdır: brüt tutarın içindeki KDV, brüt
    /// tutardan büyük olamaz. Oranla tutarın birbirini tutmaması reddedilmez
    /// (ADR 0016) — belge öyle diyor olabilir; ama tutarın kaydı aşması
    /// belgenin değil, yazım hatasının sonucudur.
    /// </remarks>
    public static void EnsureWithinAmount(VatDetails? vat, Money amount, string parameterName)
    {
        if (vat?.Amount is not { } vatAmount)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(amount);

        if (vatAmount > amount.Amount)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                vatAmount,
                "Vat amount cannot exceed the amount of the record that carries it.");
        }
    }

    /// <summary>
    /// Verilen brüt tutar ve oranla belgede beklenen KDV tutarı.
    /// </summary>
    /// <remarks>
    /// <b>Yalnız uyarı içindir.</b> Bu değer hiçbir yere yazılmaz, hiçbir
    /// kaydın alanını doldurmaz ve hiçbir isteği reddetmez; arayüz kullanıcıya
    /// "girdiğiniz oran bu tutarla uyuşmuyor" diyebilsin diye vardır. Kayda
    /// giren tutar her zaman kullanıcının yazdığıdır (ADR 0016).
    ///
    /// Brüt tutar KDV'yi <b>içerir</b>, bu yüzden oran tutara değil, tutarın
    /// KDV'siz kısmına uygulanır: <c>brüt × oran ÷ (1 + oran)</c>.
    /// </remarks>
    public static decimal ImpliedAmount(Money grossAmount, decimal rate)
    {
        ArgumentNullException.ThrowIfNull(grossAmount);
        if (rate < 0m || rate >= 1m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                rate,
                "Vat rate must be at least zero and below one.");
        }

        return decimal.Round(
            grossAmount.Amount * rate / (1m + rate),
            MoneyDecimals,
            MidpointRounding.AwayFromZero);
    }
}
