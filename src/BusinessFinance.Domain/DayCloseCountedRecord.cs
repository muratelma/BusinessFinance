namespace BusinessFinance.Domain;

/// <summary>O gün tek tek girilmiş ve bir gün sonunda sayılabilecek kaydın türü.</summary>
public enum DayCloseRecordKind
{
    /// <summary>Nakit hesaba tek tek girilmiş satış geliri.</summary>
    Income = 1,

    /// <summary>Tek tek girilmiş POS tahsilatı.</summary>
    PosSettlement = 2,

    /// <summary>Nakit hesaba cari tahsilat.</summary>
    CounterpartyPayment = 3,

    /// <summary>Tek seferlik alacağın nakit hesaba tahsilatı.</summary>
    ObligationSettlement = 4,

    /// <summary>
    /// O gün yazılmış veresiye satış (cari borçlandırma, alacak yönü): geliri
    /// yazılmıştır, parası alınmamış olabilir.
    /// </summary>
    CounterpartyCharge = 5,

    /// <summary>O gün yazılmış alacak faturası (alacak yönlü yükümlülük).</summary>
    Obligation = 6
}

/// <summary>
/// Bir gün sonunun <b>saydığı</b> kayıt: tek tek girilmişti, gün sonu tutarının
/// içindeydi ve o tutardan düşüldü.
/// </summary>
/// <remarks>
/// ADR 0019 İ2: aynı satış iki kez gelir sayılmaz. Gün sonu, düştüğü kaydı bu
/// bağla sahiplenir; bağ olmasaydı aynı kayıt bir "ek gün sonu"nda ikinci kez
/// düşülebilir ya da sonradan iptal edilip günün gelirini sessizce
/// eksiltebilirdi.
///
/// Bağ <b>tutar taşımaz</b>: yalnız "şu gün sonu şu kaydı saydı" der. Kayıt
/// olduğu gibi kalır; raporlara kendi tablosundan girer. Bir kayıt en çok bir
/// gün sonunda sayılır. Gün sonu geri alınınca bağ silinir ve kayıt yeniden
/// sayılabilir hâle gelir.
/// </remarks>
public sealed class DayCloseCountedRecord
{
    public Guid UserId { get; }
    public Guid DayCloseId { get; }
    public DayCloseRecordKind Kind { get; }
    public Guid RecordId { get; }

    private DayCloseCountedRecord()
    {
    }

    public DayCloseCountedRecord(DayClose dayClose, DayCloseRecordKind kind, Guid recordId)
    {
        ArgumentNullException.ThrowIfNull(dayClose);
        if (dayClose.IsCancelled)
        {
            throw new InvalidOperationException("A reverted day close counts no records.");
        }

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "Record kind is not supported.");
        }

        if (recordId == Guid.Empty)
        {
            throw new ArgumentException("Record id cannot be empty.", nameof(recordId));
        }

        UserId = dayClose.UserId;
        DayCloseId = dayClose.Id;
        Kind = kind;
        RecordId = recordId;
    }
}
