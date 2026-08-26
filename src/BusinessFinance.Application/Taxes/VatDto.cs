using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

/// <summary>
/// Bir kaydın üstünde taşınan KDV bilgisinin sözleşme karşılığı.
/// </summary>
/// <remarks>
/// ADR 0016: taşınan bir bilgidir. Uygulama katmanı da oranı tutardan, tutarı
/// orandan <b>türetmez</b>; ne geldiyse onu domain'e verir.
/// </remarks>
public sealed record VatDto(decimal? Rate, decimal? Amount)
{
    public static VatDto? From(VatDetails? vat) =>
        vat is null ? null : new VatDto(vat.Rate, vat.Amount);

    /// <summary>
    /// İkisi de boşsa <c>null</c>: "KDV yok"un tek temsili budur.
    /// </summary>
    public VatDetails? ToDomain() => VatDetails.FromOptional(Rate, Amount);
}
