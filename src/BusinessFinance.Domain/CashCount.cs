namespace BusinessFinance.Domain;

/// <summary>
/// Gün sonunda kasadaki nakdin sayılması.
/// </summary>
/// <remarks>
/// Sayımın kendisi <b>para hareketi değildir</b>: bir gözlemdir. Hesap
/// bakiyesine dokunmaz, gelir/gider yazmaz ve tek başına hiçbir rapora
/// girmez. Kasadaki para zaten oradaydı; sayması onu değiştirmez.
///
/// <b>Beklenen tutar saklanmaz.</b> Fark, sayım okunduğu anda hesap
/// bakiyesi projection'ından türetilir (<see cref="DifferenceFrom"/>).
/// Saklansaydı, sonradan iptal edilen bir hareket bakiyeyi değiştirdiği anda
/// sayımın yanındaki sayı sessizce yanlışa dönerdi.
/// </remarks>
public sealed class CashCount
{
    public const int MaximumNoteLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid AccountId { get; }
    public DateOnly CountDate { get; }

    /// <summary>
    /// Sayılan tutar. <see cref="Money"/> değildir çünkü <b>sıfır meşrudur</b>:
    /// kasası boşalan esnaf da sayım yapar.
    /// </summary>
    public decimal CountedAmount { get; }

    public CurrencyCode Currency { get; }

    /// <summary>
    /// Farkın düzeltme kaydı üretilirse alacağı kapsam.
    /// </summary>
    /// <remarks>
    /// Kapsam sayım anında sabitlenir, düzeltme anında yeniden türetilmez:
    /// aynı sayım iki farklı günde iki farklı kapsam üretebilirdi. Tekrarlayan
    /// planın kapsamında verilen kararın aynısı.
    /// </remarks>
    public TransactionScope Scope { get; }

    public string? Note { get; }
    public DateTimeOffset CreatedAtUtc { get; }

    /// <summary>Farkı yazan gelir/gider kaydı; kullanıcı onaylamadıysa boştur.</summary>
    public Guid? AdjustmentTransactionId { get; private set; }

    /// <summary>
    /// Farkı açıklayan aktarım: eksik para kasadan şahsi hesaba geçmişse.
    /// </summary>
    /// <remarks>
    /// Bir sayımın farkı <b>tek</b> kayıtla açıklanır: ya bir gelir/gider
    /// (<see cref="AdjustmentTransactionId"/>) ya bu aktarım. İkisi birden
    /// dolu olamaz. Bağ kayıt iptal edilse de <b>silinmez</b>: sayımın farkının
    /// bir kez açıklandığı ve o açıklamanın geri alındığı bilgisi geçmiştir.
    /// </remarks>
    public Guid? AdjustmentTransferId { get; private set; }
    public DateTimeOffset? AdjustedAtUtc { get; private set; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    /// <summary>Fark kaydı üretildi mi? Üretilmemiş olması normaldir.</summary>
    public bool IsAdjusted =>
        AdjustmentTransactionId is not null || AdjustmentTransferId is not null;

    /// <summary>
    /// Sayım yazıldığı anda uygulamanın bu kasada beklediği bakiye.
    /// </summary>
    /// <remarks>
    /// Güncel bir türetme <b>değildir</b>, tarihsel bir gözlemdir: geçmiş bir
    /// sayımın farkını bugünkü bakiyeye karşı hesaplamak aradaki bütün
    /// hareketleri o günün farkına yazmak olurdu. Bu alan "o gün ne
    /// bekleniyordu" sorusunu kayda geçirir; günün açık sayımının farkı yine
    /// güncel bakiyeden hesaplanır. Alan eklenmeden önce yazılmış sayımlarda
    /// bilinmez ve boştur.
    /// </remarks>
    public decimal? ExpectedAtCount { get; }

    private CashCount()
    {
    }

    public CashCount(
        Guid id,
        Guid userId,
        Account account,
        decimal countedAmount,
        TransactionScope scope,
        DateOnly countDate,
        DateTimeOffset createdAtUtc,
        string? note = null,
        decimal? expectedAtCount = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cash count id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(account);
        if (account.UserId != userId)
        {
            throw new ArgumentException(
                "Account must belong to the cash count user.",
                nameof(account));
        }

        if (account.Type != AccountType.Cash)
        {
            throw new InvalidOperationException(
                "Only a cash account can be counted; a bank balance is not counted by hand.");
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException("An inactive account cannot be counted.");
        }

        if (countedAmount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(countedAmount),
                "Counted cash cannot be negative; an empty till is zero.");
        }

        if (decimal.Round(countedAmount, 4) != countedAmount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(countedAmount),
                "Counted cash cannot carry more than four decimals.");
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));
        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        }

        if (countDate == default || countDate > LocalDay.LatestAllowed(createdAtUtc))
        {
            throw new ArgumentOutOfRangeException(
                nameof(countDate),
                "Count date is required and cannot be in the future.");
        }

        Id = id;
        UserId = userId;
        AccountId = account.Id;
        Currency = account.Currency;
        CountedAmount = countedAmount;
        Scope = scope;
        CountDate = countDate;
        CreatedAtUtc = createdAtUtc;
        Note = NormalizeNote(note);
        ExpectedAtCount = expectedAtCount is decimal expected
            ? decimal.Round(expected, 4)
            : null;
    }

    /// <summary>
    /// Sayılan tutar ile o an hesaptan okunan beklenen bakiyenin farkı.
    /// </summary>
    /// <remarks>
    /// Beklenen bakiye dışarıdan gelir çünkü hesabın bakiyesi bu aggregate'in
    /// bilgisi değildir: hareketlerden hesaplanır ve Domain katmanı sorgu
    /// çalıştırmaz. Fark saklanmadığı için aynı sayım, sonradan iptal edilen
    /// bir hareketten sonra farklı bir fark verir — doğru davranış budur.
    /// </remarks>
    public CashCountDifference DifferenceFrom(decimal expectedBalance)
    {
        if (decimal.Round(expectedBalance, 4) != expectedBalance)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedBalance),
                "Expected balance cannot carry more than four decimals.");
        }

        return new CashCountDifference(CountedAmount - expectedBalance, Currency);
    }

    /// <summary>
    /// Bu sayımın farkı hâlâ kaydedilebilir mi, yoksa kasa yeniden mi
    /// sayılmalı? Fark yalnız <b>güncel</b> sayıma yazılır.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sayımdan sonra kasaya bir kayıt girildiyse uygulama onun ne zaman
    /// <b>olduğunu</b> bilemez: sayımdan sonra yapılmış bir satış da olabilir,
    /// sayımdan önce unutulmuş bir gider de. İlkinde fark sayım anındakidir,
    /// ikincisinde bugünkü bakiyeye göre olandır; yanlış olanı seçmek kasaya
    /// olmamış bir gider yazar (900 sayılıp 1.000 beklenirken sonradan 200
    /// liralık satış girildiğinde 100 yerine 300 yazılıyordu). Belirsizliği
    /// yalnız yeni bir sayım kaldırır.
    /// </para>
    /// <para>
    /// Bakiyenin aynı kalması yetmez: aynı tutarda bir giriş ve bir çıkış
    /// bakiyeyi değiştirmez ama kasanın hareketlerini değiştirir. Bu yüzden
    /// iki işaret birden sorulur. Sayım anındaki bakiyesi bilinmeyen eski
    /// sayım da yeniden sayım ister.
    /// </para>
    /// </remarks>
    /// <param name="currentBalance">Kasanın şu anki bakiyesi.</param>
    /// <param name="accountChangedSince">
    /// Sayımdan sonra bu kasaya kayıt girildi ya da daha yeni bir sayım var.
    /// </param>
    public bool RequiresRecount(decimal currentBalance, bool accountChangedSince) =>
        accountChangedSince ||
        ExpectedAtCount is not decimal atCount ||
        atCount != currentBalance;

    /// <summary>
    /// Kullanıcı farkı onayladığında üretilen tek düzeltme kaydını bağlar.
    /// </summary>
    /// <remarks>
    /// Sayım bunu <b>kendiliğinden yapmaz</b>: sayım hatasını gerçek bir para
    /// hareketi gibi yazmak, olmamış bir gideri kayda geçirmek olurdu. Kayıt
    /// ikinci ve ayrı bir eylemdir, kullanıcının açık onayıyla oluşur.
    ///
    /// Çağrı idempotenttir: aynı kimlikle tekrarlamak ikinci kayıt üretmez.
    /// Farklı bir kimlikle tekrarlamak reddedilir — bir sayımın iki düzeltmesi
    /// olamaz.
    /// </remarks>
    public void RecordAdjustment(Guid budgetTransactionId, DateTimeOffset adjustedAtUtc)
    {
        if (budgetTransactionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Adjustment transaction id cannot be empty.",
                nameof(budgetTransactionId));
        }

        if (adjustedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Adjustment time must be UTC.", nameof(adjustedAtUtc));
        }

        if (IsCancelled)
        {
            throw new InvalidOperationException(
                "A cancelled cash count cannot record an adjustment.");
        }

        if (AdjustmentTransactionId == budgetTransactionId)
        {
            return;
        }

        if (IsAdjusted)
        {
            throw new InvalidOperationException(
                "This cash count already carries a different adjustment.");
        }

        AdjustmentTransactionId = budgetTransactionId;
        AdjustedAtUtc = adjustedAtUtc;
    }

    /// <summary>
    /// Farkı açıklayan aktarımı bağlar: eksik para kasadan şahsi hesaba
    /// geçmiştir (<c>Kendime aldım</c>).
    /// </summary>
    /// <remarks>
    /// Gelir/gider yazılmaz; para yer değiştirir. Kural
    /// <see cref="RecordAdjustment"/> ile aynıdır: çağrı idempotenttir ve bir
    /// sayımın iki açıklaması olamaz.
    /// </remarks>
    public void RecordTransferAdjustment(Guid transferId, DateTimeOffset adjustedAtUtc)
    {
        if (transferId == Guid.Empty)
        {
            throw new ArgumentException(
                "Adjustment transfer id cannot be empty.",
                nameof(transferId));
        }

        if (adjustedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Adjustment time must be UTC.", nameof(adjustedAtUtc));
        }

        if (IsCancelled)
        {
            throw new InvalidOperationException(
                "A cancelled cash count cannot record an adjustment.");
        }

        if (AdjustmentTransferId == transferId)
        {
            return;
        }

        if (IsAdjusted)
        {
            throw new InvalidOperationException(
                "This cash count already carries a different adjustment.");
        }

        AdjustmentTransferId = transferId;
        AdjustedAtUtc = adjustedAtUtc;
    }

    /// <summary>
    /// Aynı gün ve aynı hesap için yeni bir sayım yapıldığında bu sayımı
    /// kapatır.
    /// </summary>
    /// <remarks>
    /// Eski sayımın <b>üzerine yazılmaz</b>: sayım bir gözlemdir ve o gözlem
    /// gerçekten yapılmıştı. İptal edilmiş hâliyle kalır; düzeltme kaydı
    /// varsa o kayıt da ayrıca iptal edilir — ama bunu çağıran katman yapar,
    /// çünkü <see cref="BudgetTransaction"/> ayrı bir aggregate'tir.
    /// </remarks>
    public void SupersedeWith(CashCount replacement, DateTimeOffset supersededAtUtc)
    {
        ArgumentNullException.ThrowIfNull(replacement);
        if (replacement.Id == Id)
        {
            throw new InvalidOperationException("A cash count cannot supersede itself.");
        }

        if (replacement.UserId != UserId ||
            replacement.AccountId != AccountId ||
            replacement.CountDate != CountDate)
        {
            throw new InvalidOperationException(
                "Only a count of the same account and day can supersede this one.");
        }

        Cancel(supersededAtUtc);
    }

    /// <summary>Silme yerine iptal; tekrarlanan çağrı ilk damgayı korur.</summary>
    public void Cancel(DateTimeOffset cancelledAtUtc)
    {
        if (cancelledAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
        }

        if (IsCancelled)
        {
            return;
        }

        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }

    private static string? NormalizeNote(string? note)
    {
        var normalized = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (normalized?.Length > MaximumNoteLength)
        {
            throw new ArgumentException(
                $"Cash count note cannot exceed {MaximumNoteLength} characters.",
                nameof(note));
        }

        return normalized;
    }
}
