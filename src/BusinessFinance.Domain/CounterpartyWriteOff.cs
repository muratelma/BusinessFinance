namespace BusinessFinance.Domain;

/// <summary>
/// Kişinin hesabında kalan tutardan vazgeçmek: alınmayacak alacak ya da
/// ödenmeyecek borç (ADR 0021 İ4; ADR 0022).
/// </summary>
/// <remarks>
/// <b>Tanır, taşımaz</b> (ADR 0014). Para hareket etmez; kişinin bakiyesi
/// ödeme gibi düşer (en eski kalemden başlayarak, ADR 0022 İ2) ve o gün gelir
/// ya da gider yazılır:
/// <list type="bullet">
/// <item><see cref="DebtDirection.Receivable"/>: alacaktan vazgeçtik — gider.</item>
/// <item><see cref="DebtDirection.Payable"/>: borcumuz bağışlandı — gelir.</item>
/// </list>
/// Kategori bu yüzden borçlandırmanın tersidir: alacakta gider, borçta gelir
/// kategorisi. Kapsam her gelir ve gider gibi taşınır.
///
/// İndirimden farkı: indirim bir alımın <b>bedelini</b> düşürür ve alım
/// gününde gelir ya da gideri azaltır; vazgeçme bedele dokunmaz, kalan parayı
/// kendi gününde kapatır. Devreden bakiye ve borç para bedeli olan bir alım
/// olmadığı için yalnız vazgeçmeyle kapanır.
/// </remarks>
public sealed class CounterpartyWriteOff
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }

    public Guid UserId { get; }

    public Guid CounterpartyId { get; }

    public Guid CategoryId { get; }

    public DebtDirection Direction { get; }

    public Money Amount { get; }

    public TransactionScope Scope { get; }

    /// <summary>Vazgeçilen gün; ileri bir gün olamaz.</summary>
    public DateOnly WriteOffDate { get; }

    public string? Description { get; }

    public bool IsCancelled { get; private set; }

    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CounterpartyWriteOff()
    {
        Amount = null!;
    }

    public CounterpartyWriteOff(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        Category category,
        DebtDirection direction,
        Money amount,
        TransactionScope scope,
        DateOnly writeOffDate,
        string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Write-off id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(counterparty);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(amount);
        if (counterparty.UserId != userId || category.UserId != userId)
        {
            throw new ArgumentException(
                "Counterparty and category must belong to the write-off user.");
        }

        if (!Enum.IsDefined(direction))
        {
            throw new ArgumentOutOfRangeException(
                nameof(direction), direction, "Write-off direction is not supported.");
        }

        if (!category.IsActive || category.Type != RequiredCategoryType(direction))
        {
            throw new ArgumentException(
                "A write-off needs an active category of the opposite type to its charges.",
                nameof(category));
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));

        if (writeOffDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(writeOffDate), "Write-off date is required.");
        }

        Id = id;
        UserId = userId;
        CounterpartyId = counterparty.Id;
        CategoryId = category.Id;
        Direction = direction;
        Amount = amount;
        Scope = scope;
        WriteOffDate = writeOffDate;
        Description = NormalizeDescription(description);
    }

    /// <summary>
    /// Alacaktan vazgeçmek gider, bağışlanan borç gelirdir: borçlandırmanın
    /// istediği kategori türünün tersi.
    /// </summary>
    public static CategoryType RequiredCategoryType(DebtDirection direction) =>
        direction == DebtDirection.Receivable ? CategoryType.Expense : CategoryType.Income;

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
                $"Write-off description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        return normalized;
    }
}
