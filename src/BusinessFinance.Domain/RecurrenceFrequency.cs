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
    Quarterly = 5
}
