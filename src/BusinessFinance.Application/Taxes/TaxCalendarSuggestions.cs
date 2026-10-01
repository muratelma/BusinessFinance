using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

/// <summary>
/// Vergi ekranının hazır türleri (ADR 0018 T2; ayrıntı
/// <c>research/vergi/YENI-YAKLASIM.md</c> §6.6).
/// </summary>
/// <remarks>
/// <para>
/// Bunlar <b>önerilerdir, mevzuat değildir</b>. Uygulama mevzuat takibi yapmaz;
/// öneri kurulduğu an kullanıcının verisi olur, kullanıcı düzenler ve siler,
/// uygulama onu sonradan kendiliğinden güncellemez.
/// </para>
/// <para>
/// Öneri yalnız <b>ritim ve gün</b> taşır, <b>tutar taşımaz</b> (V-K3, V-K7):
/// tutar kullanıcınındır ve bu kalemlerin çoğunda her dönem değişir; bir sayı
/// önermek, hesaplanmış bir vergi tutarı iddia etmek olurdu (İ1). Kategori de
/// taşımaz: vergi, kullanıcının vergi işaretli kategorisine yazılır.
/// </para>
/// <para>
/// Kurulum <b>ikinci bir yazma yolu açmaz</b>: tür, mevcut tekrarlayan plan
/// ucundan (<c>POST /api/v1/recurring-transactions</c>) vergi türüyle kurulur.
/// </para>
/// </remarks>
public static class TaxCalendarSuggestions
{
    /// <summary>
    /// Hazır türün sözleşme karşılığı.
    /// </summary>
    /// <param name="TaxKind">Türün kararlı makine değeri; adı ve ipucunu istemci kurar.</param>
    /// <param name="Months">
    /// "Seçilen aylarda" ve "yılda bir" ritminin ayları (1–12); diğerlerinde boş.
    /// </param>
    /// <param name="DayOfMonth">
    /// Önerilen gün; <c>31</c> "ay sonu" demektir (kısa aylarda son gün). Bir
    /// <b>başlangıç noktasıdır</b>: kurulduktan sonra tarih kullanıcınındır.
    /// </param>
    public sealed record TaxCalendarSuggestion(
        TaxKind TaxKind,
        RecurrenceFrequency Frequency,
        IReadOnlyList<int> Months,
        int DayOfMonth);

    /// <summary>"Ay sonu": kısa aylarda ayın son gününe iner.</summary>
    public const int MonthEnd = 31;

    public static IReadOnlyList<TaxCalendarSuggestion> All { get; } =
    [
        new(TaxKind.SocialSecurityPremium, RecurrenceFrequency.Monthly, [], MonthEnd),
        new(TaxKind.VatReturn, RecurrenceFrequency.Monthly, [], 28),
        new(TaxKind.WithholdingReturn, RecurrenceFrequency.Monthly, [], 26),
        new(TaxKind.AdvanceTax, RecurrenceFrequency.SelectedMonths, [2, 5, 8, 11], 17),
        new(TaxKind.AnnualIncomeTax, RecurrenceFrequency.SelectedMonths, [3, 7], MonthEnd),
        new(TaxKind.PropertyTax, RecurrenceFrequency.SelectedMonths, [5, 11], MonthEnd),
        new(TaxKind.MotorVehicleTax, RecurrenceFrequency.SelectedMonths, [1, 7], MonthEnd),
        new(TaxKind.AdvertisingTax, RecurrenceFrequency.Yearly, [1], MonthEnd)
    ];
}
