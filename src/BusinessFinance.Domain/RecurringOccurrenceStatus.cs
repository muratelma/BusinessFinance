namespace BusinessFinance.Domain;

public enum RecurringOccurrenceStatus
{
    Planned = 1,
    Realized = 2,

    /// <summary>
    /// Toplu bir vergi ödemesiyle kapatıldı (ADR 0018 T5): kalemin kendi sonuç
    /// kaydı yoktur, kapatan ödemenin kimliğini taşır. Aynı ödeme birden çok
    /// kalemi kapatabilir; ödeme iptal edilince kalem bekleyene döner.
    /// </summary>
    Closed = 3
}
