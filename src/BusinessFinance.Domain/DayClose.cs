namespace BusinessFinance.Domain;

/// <summary>
/// Bir günün (ya da birkaç günlük bir Z'nin) kapatıldığını söyleyen kimlik:
/// gün sonunun ürettiği gelir ve POS tahsilatı kayıtlarını birbirine bağlar.
/// </summary>
/// <remarks>
/// ADR 0019 İ3: gün sonu yeni bir finansal kayıt türü <b>değildir</b>. Bu
/// kayıt <b>tutar taşımaz</b>; raporlar, işletme neti, bütçe, bakiye ve
/// birleşik akış onu okumaz. Para, ürettiği sıradan kayıtlardadır
/// (<see cref="BudgetTransaction"/> ve <see cref="PosSettlement"/>); bu kayıt
/// yalnız üç soruyu cevaplar: hangi kayıtlar birlikte doğdu, o gün kapatıldı
/// mı, hangi Z numarasıyla.
///
/// Hiç kayıt üretmeyen bir gün sonu meşrudur: gün içinde her satış tek tek
/// girildiyse yazılacak tutar kalmaz, ama gün yine de kapatılmıştır.
/// </remarks>
public sealed class DayClose
{
    public Guid Id { get; }
    public Guid UserId { get; }

    /// <summary>Kapatılan gün; birkaç günlük Z'de aralığın son günü.</summary>
    public DateOnly ClosedOn { get; }

    /// <summary>
    /// Birkaç günlük Z'nin ilk günü; tek günlük gün sonunda boştur. Aralıktaki
    /// bütün günler kapalı sayılır.
    /// </summary>
    public DateOnly? RangeStart { get; }

    /// <summary>
    /// Z raporunun numarası; yalnız fotoğraftan okunur, elle girişte boştur.
    /// </summary>
    public int? ZNumber { get; }

    /// <summary>
    /// Aynı günün ikinci gün sonu (ikinci cihaz). Kullanıcı bunu açıkça
    /// söyler; söylemeden aynı güne ikinci bir gün sonu yazılamaz.
    /// </summary>
    public bool IsAdditional { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    /// <summary>Kapatılan ilk gün.</summary>
    public DateOnly FirstDay => RangeStart ?? ClosedOn;

    private DayClose()
    {
    }

    private DayClose(
        Guid id,
        Guid userId,
        DateOnly closedOn,
        DateOnly? rangeStart,
        int? zNumber,
        bool isAdditional,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Day close id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        }

        if (closedOn == default || closedOn > DateOnly.FromDateTime(createdAtUtc.UtcDateTime))
        {
            throw new ArgumentOutOfRangeException(
                nameof(closedOn),
                "The closed day is required and cannot be in the future.");
        }

        if (rangeStart is DateOnly start && start >= closedOn)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeStart),
                "A range starts before the day it ends on.");
        }

        if (zNumber is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(zNumber),
                "A Z number is a positive number.");
        }

        Id = id;
        UserId = userId;
        ClosedOn = closedOn;
        RangeStart = rangeStart;
        ZNumber = zNumber;
        IsAdditional = isAdditional;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>Verilen gün bu gün sonuyla kapatılmış mı.</summary>
    public bool Covers(DateOnly day) => day >= FirstDay && day <= ClosedOn;

    /// <summary>
    /// Gün sonunu, ürettiği kayıtlarla birlikte yazar.
    /// </summary>
    /// <remarks>
    /// Kayıtlar bu gün sonunun kimliğiyle kurulmuş olmalı ve kapatılan
    /// günlerin içinde durmalıdır; aksi hâlde geri alma başka bir günün
    /// kaydını iptal ederdi. Liste boş olabilir.
    /// </remarks>
    public static DayClose Record(
        Guid id,
        Guid userId,
        DateOnly closedOn,
        DateTimeOffset createdAtUtc,
        IReadOnlyCollection<BudgetTransaction> incomes,
        IReadOnlyCollection<PosSettlement> settlements,
        DateOnly? rangeStart = null,
        int? zNumber = null,
        bool isAdditional = false)
    {
        ArgumentNullException.ThrowIfNull(incomes);
        ArgumentNullException.ThrowIfNull(settlements);
        var close = new DayClose(
            id, userId, closedOn, rangeStart, zNumber, isAdditional, createdAtUtc);

        foreach (var income in incomes)
        {
            if (income.UserId != userId || income.DayCloseId != id)
            {
                throw new ArgumentException(
                    "Incomes must be written for this day close.", nameof(incomes));
            }

            if (income.Type != TransactionType.Income || income.IsCancelled ||
                !close.Covers(income.TransactionDate))
            {
                throw new InvalidOperationException(
                    "A day close produces live income records inside the days it closes.");
            }
        }

        foreach (var settlement in settlements)
        {
            if (settlement.UserId != userId || settlement.DayCloseId != id)
            {
                throw new ArgumentException(
                    "Settlements must be written for this day close.", nameof(settlements));
            }

            if (settlement.IsCancelled || !close.Covers(settlement.SettlementDate))
            {
                throw new InvalidOperationException(
                    "A day close produces live pos settlements inside the days it closes.");
            }
        }

        return close;
    }

    /// <summary>Gün sonunu yedekten kurar; kayıtları ayrıca geri yüklenir.</summary>
    public static DayClose Restore(
        Guid id,
        Guid userId,
        DateOnly closedOn,
        DateOnly? rangeStart,
        int? zNumber,
        bool isAdditional,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? cancelledAtUtc)
    {
        if (cancelledAtUtc is DateTimeOffset cancelled && cancelled.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
        }

        return new DayClose(id, userId, closedOn, rangeStart, zNumber, isAdditional, createdAtUtc)
        {
            IsCancelled = cancelledAtUtc is not null,
            CancelledAtUtc = cancelledAtUtc,
        };
    }

    /// <summary>
    /// Gün sonunu bir bütün olarak geri alır: ürettiği kayıtlar birlikte
    /// iptal olur ve gün yeniden açılır (ADR 0019 İ8).
    /// </summary>
    /// <remarks>
    /// Silme yerine iptal: gün sonu ve kayıtları kalır. Ürettiği bir POS
    /// tahsilatı bir yatışla hesaba geçtiyse geri alma reddedilir; önce yatış
    /// geri alınır. Denetim hiçbir kayıt değişmeden önce yapılır: yarım geri
    /// alınmış bir gün sonu olmaz.
    ///
    /// Çağrı idempotenttir: geri alınmış gün sonunda hiçbir şey yapmaz.
    /// </remarks>
    public void Revert(
        IReadOnlyCollection<BudgetTransaction> incomes,
        IReadOnlyCollection<PosSettlement> settlements,
        DateTimeOffset cancelledAtUtc)
    {
        ArgumentNullException.ThrowIfNull(incomes);
        ArgumentNullException.ThrowIfNull(settlements);
        if (cancelledAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
        }

        if (IsCancelled)
        {
            return;
        }

        if (incomes.Any(income => income.DayCloseId != Id) ||
            settlements.Any(settlement => settlement.DayCloseId != Id))
        {
            throw new InvalidOperationException(
                "A day close is reverted together with the records it produced.");
        }

        if (settlements.Any(settlement => settlement.PosDepositId is not null))
        {
            throw new InvalidOperationException(
                "A day close with a deposited pos settlement cannot be reverted; revert the deposit first.");
        }

        foreach (var income in incomes)
        {
            income.Cancel(cancelledAtUtc);
        }

        foreach (var settlement in settlements)
        {
            settlement.Cancel(cancelledAtUtc);
        }

        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }
}
