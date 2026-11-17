namespace BusinessFinance.Domain;

/// <summary>
/// Kişinin hesabında gelir ya da gider yazmadan bakiyeyi <b>artıran</b> para:
/// verilen ve alınan borç (ADR 0022 İ3).
/// </summary>
/// <remarks>
/// <see cref="CounterpartyCharge"/> tanır (satış geliri, alım gideri) ve hesaba
/// dokunmaz; bu kayıt <b>taşır</b>: hesap bakiyesini değiştirir, gelir/gider
/// üretmez ve bu yüzden ne kategori ne kapsam taşır (ADR 0014).
///
/// Borçlandırmayla aynı tabloda durmaz. Borçlandırmayı okuyan her yer onu
/// "gelir ya da gider" diye okur; aynı tabloda bir tür olsaydı unutulan tek
/// bir süzgeç arkadaşa verilen borcu sessizce gelir yazardı.
///
/// Yön bakiyenin yönüdür: <see cref="DebtDirection.Receivable"/> verdiğimiz
/// paradır (alacağımız artar, hesaptan çıkar); <see cref="DebtDirection.Payable"/>
/// aldığımız paradır (borcumuz artar, hesaba girer).
///
/// Avans bu kayıt değildir: iş ya da mal öncesi alınan ya da verilen para
/// sıradan bir tahsilat ya da ödemedir. Karşısında borç yokken yazılınca
/// karşı yönde bakiye olur ve sonraki satış ya da alımdan düşer (senaryo 22).
/// Ayrı kayıt olsaydı ayrı bir borç olarak durur, satıştan düşmezdi.
/// </remarks>
public sealed class CounterpartyLoan
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }

    public Guid UserId { get; }

    public Guid CounterpartyId { get; }

    public Guid AccountId { get; }

    public DebtDirection Direction { get; }

    public Money Amount { get; }

    /// <summary>Paranın el değiştirdiği gün; ileri bir gün olamaz.</summary>
    public DateOnly LoanDate { get; }

    /// <summary>Geri ödemenin beklendiği gün; verilmeyebilir.</summary>
    public DateOnly? DueDate { get; }

    public string? Description { get; }

    public bool IsCancelled { get; private set; }

    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CounterpartyLoan()
    {
        Amount = null!;
    }

    public CounterpartyLoan(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        Account account,
        DebtDirection direction,
        Money amount,
        DateOnly loanDate,
        DateOnly? dueDate = null,
        string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Loan id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(counterparty);
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(amount);

        if (counterparty.UserId != userId || account.UserId != userId)
        {
            throw new ArgumentException(
                "Counterparty and account must belong to the loan user.");
        }

        // Yeni bir borç yeni bir ilişkidir; pasif kişiyle açılmaz
        // (borçlandırmayla aynı kural). Kalan bakiyesi ödenebilir.
        if (!counterparty.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive counterparty cannot take on a new loan.");
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive account cannot give or receive a loan.");
        }

        if (!Enum.IsDefined(direction))
        {
            throw new ArgumentOutOfRangeException(
                nameof(direction), direction, "Loan direction is not supported.");
        }

        if (amount.Currency != account.Currency)
        {
            throw new ArgumentException(
                "Loan and account must use the same currency.", nameof(amount));
        }

        if (loanDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(loanDate), "Loan date is required.");
        }

        if (dueDate is DateOnly due && due < loanDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dueDate), "Due date cannot be before the loan date.");
        }

        Id = id;
        UserId = userId;
        CounterpartyId = counterparty.Id;
        AccountId = account.Id;
        Direction = direction;
        Amount = amount;
        LoanDate = loanDate;
        DueDate = dueDate;
        Description = NormalizeDescription(description);
    }

    /// <summary>
    /// Hesap bakiyesine etkisi: verilen para azaltır, alınan para artırır.
    /// </summary>
    /// <remarks>
    /// Tahsilat ve ödemenin tersidir: orada alacak yönü hesabı artırır (para
    /// geri gelir), burada azaltır (para verilir).
    /// </remarks>
    public decimal SignedAccountEffect => Direction == DebtDirection.Receivable
        ? -Amount.Amount
        : Amount.Amount;

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

    private static string? NormalizeDescription(string? description)
    {
        var normalized = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalized?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Loan description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        return normalized;
    }
}
