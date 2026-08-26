using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

/// <summary>
/// Vergi ve SGK takviminin hazır kalemleri.
/// </summary>
/// <remarks>
/// ADR 0016: bunlar <b>önerilerdir, mevzuat değildir</b>. Uygulama mevzuat
/// takibi yapmaz; öneri kurulduğu an kullanıcının verisi olur, kullanıcı
/// düzenler ve siler, uygulama onu sonradan kendiliğinden güncellemez.
///
/// Öneri <b>tutar taşımaz</b>. Tutar kullanıcınındır ve bu kalemlerin çoğunda
/// her dönem değişir; bir sayı önermek, hesaplanmış bir vergi tutarı iddia
/// etmek olurdu.
///
/// Kurulum <b>ikinci bir yazma yolu açmaz</b>: kalem, mevcut tekrarlayan plan
/// ucundan (`POST /api/v1/recurring-transactions`) önerinin doldurduğu formla
/// kurulur. Yeni bir zamanlayıcı, yeni bir tablo ve ikinci bir "yaklaşanlar"
/// kaynağı yoktur — kalem tekrarlayan bir plandır ve planlanan görünüme
/// oradan düşer.
/// </remarks>
public static class TaxCalendarSuggestions
{
    /// <summary>
    /// Hazır kalemin sözleşme karşılığı.
    /// </summary>
    /// <param name="Key">Kararlı makine değeri; kullanıcıya gösterilecek cümleyi istemci kurar.</param>
    /// <param name="SuggestedDayOfMonth">
    /// Önerilen gün. Bir <b>başlangıç noktasıdır</b>: kullanıcı kurarken
    /// değiştirebilir ve kurulduktan sonra tarih tamamen ona aittir.
    /// </param>
    /// <param name="SuggestedCategoryName">
    /// İşletme kategori setinde bu kalemin düştüğü kategori. Kategori yoksa
    /// istemci kullanıcıya seçtirir; sunucu kategori <b>oluşturmaz</b>.
    /// </param>
    public sealed record TaxCalendarSuggestion(
        string Key,
        RecurrenceFrequency Frequency,
        int SuggestedDayOfMonth,
        string SuggestedCategoryName,
        RecurringTransactionKind Kind,
        TransactionScope Scope);

    private const string TaxCategory = "SGK ve vergi ödemesi";

    public static IReadOnlyList<TaxCalendarSuggestion> All { get; } =
    [
        new(
            "vat-return",
            RecurrenceFrequency.Monthly,
            28,
            TaxCategory,
            RecurringTransactionKind.Expense,
            TransactionScope.Business),
        new(
            "withholding-return",
            RecurrenceFrequency.Monthly,
            26,
            TaxCategory,
            RecurringTransactionKind.Expense,
            TransactionScope.Business),
        new(
            "social-security-premium",
            RecurrenceFrequency.Monthly,
            30,
            TaxCategory,
            RecurringTransactionKind.Expense,
            TransactionScope.Business),
        new(
            "advance-tax",
            RecurrenceFrequency.Quarterly,
            17,
            TaxCategory,
            RecurringTransactionKind.Expense,
            TransactionScope.Business)
    ];
}
