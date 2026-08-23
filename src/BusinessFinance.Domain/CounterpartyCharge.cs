namespace BusinessFinance.Domain;

/// <summary>
/// Karşı tarafı borçlandıran ya da alacaklandıran ekonomik olay: veresiye satış
/// ve tedarikçiden vadeli alım.
/// </summary>
/// <remarks>
/// <b>Tanır, taşımaz</b> (ADR 0014). Gelir ya da gider o gün yazılır; hesap
/// bakiyesi kıpırdamaz, çünkü para henüz el değiştirmemiştir. Parayı
/// <see cref="CounterpartyPayment"/> taşır.
///
/// Yön kategorinin türünü belirler ve tersi olamaz: veresiye satış
/// (<see cref="DebtDirection.Receivable"/>) bir gelirdir, vadeli alım
/// (<see cref="DebtDirection.Payable"/>) bir giderdir. Yön ile kategori
/// birbirini tutmazsa kayıt raporun yanlış tarafına düşerdi.
///
/// <see cref="DebtDirection"/> yeniden kullanılıyor çünkü sorduğu soru aynı:
/// yükümlülük kimin üzerinde. Aynı iki değeri taşıyan ikinci bir enum, aynı
/// kavramı iki adla anlatmak olurdu.
/// </remarks>
public sealed class CounterpartyCharge
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid CounterpartyId { get; }
    public Guid CategoryId { get; }
    public DebtDirection Direction { get; }
    public Money Amount { get; }

    /// <summary>
    /// Gelir/gider raporunu bölen boyut (ADR 0013). Borçlandırma rapora girdiği
    /// için kapsam taşır; tahsilat girmediği için taşımaz.
    /// </summary>
    public TransactionScope Scope { get; }

    public DateOnly ChargeDate { get; }
    public string? Description { get; }
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CounterpartyCharge()
    {
        Amount = null!;
    }

    public CounterpartyCharge(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        Category category,
        DebtDirection direction,
        Money amount,
        TransactionScope scope,
        DateOnly chargeDate,
        string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Charge id cannot be empty.", nameof(id));
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
                "Counterparty and category must belong to the charge user.");
        }

        if (!counterparty.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive counterparty cannot take on a new charge.");
        }

        var requiredType = RequiredCategoryType(direction);
        if (!category.IsActive || category.Type != requiredType)
        {
            throw new InvalidOperationException(
                $"An active {requiredType.ToString().ToLowerInvariant()} category is required " +
                $"for a {direction.ToString().ToLowerInvariant()} charge.");
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));

        if (chargeDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(chargeDate), "Charge date is required.");
        }

        Id = id;
        UserId = userId;
        CounterpartyId = counterparty.Id;
        CategoryId = category.Id;
        Direction = direction;
        Amount = amount;
        Scope = scope;
        ChargeDate = chargeDate;
        Description = NormalizeDescription(description);
    }

    /// <summary>
    /// Alacak doğuran borçlandırma gelir, borç doğuran gider tanır.
    /// </summary>
    public static CategoryType RequiredCategoryType(DebtDirection direction) => direction switch
    {
        DebtDirection.Receivable => CategoryType.Income,
        DebtDirection.Payable => CategoryType.Expense,
        _ => throw new ArgumentOutOfRangeException(
            nameof(direction),
            direction,
            "Charge direction is not supported.")
    };

    /// <summary>
    /// Silme yerine iptal; UTC damgalı ve idempotent.
    /// </summary>
    /// <remarks>
    /// Borçlandırmayı iptal etmek tanınan geliri/gideri geri alır ve cari
    /// bakiyeyi düşürür — ikisi de aynı kaydın türevi olduğu için ayrı bir
    /// düzeltme kaydı gerekmez.
    /// </remarks>
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
                $"Charge description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        return normalized;
    }
}
