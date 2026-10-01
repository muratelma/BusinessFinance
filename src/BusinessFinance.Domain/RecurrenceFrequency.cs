namespace BusinessFinance.Domain;

public enum RecurrenceFrequency
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Yearly = 4,

    /// <summary>
    /// Üç ayda bir. Geçici vergi ve üç aylık beyanların ritmi budur; aylığın
    /// üç adımlık hâli olduğu için ayrı bir hesap yolu değil, aynı yolun
    /// adımıdır (Aşama 05 Grup 4).
    /// </summary>
    Quarterly = 5,

    /// <summary>
    /// Yalnız seçilen aylarda, aynı günde (ör. emlak vergisi Mayıs ve Kasım,
    /// gelir vergisi Mart ve Temmuz). "Altı ayda bir" bu ritimleri ifade
    /// edemiyor: Mart–Temmuz arası dört, Temmuz–Mart arası sekiz ay
    /// (ADR 0018 T2). Aylar planın <c>SelectedMonths</c> alanındadır.
    /// </summary>
    SelectedMonths = 6
}
