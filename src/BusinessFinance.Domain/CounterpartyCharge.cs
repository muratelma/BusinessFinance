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

    private readonly List<CounterpartyChargeDiscount> _discounts = [];

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
    public DateOnly? DueDate { get; }
    public string? Description { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    /// <summary>Alımın indirimleri; iptal edilmişler geri alınmıştır.</summary>
    public IReadOnlyCollection<CounterpartyChargeDiscount> Discounts => _discounts.AsReadOnly();

    /// <summary>
    /// Bedel: ilk tutar, iptal edilmemiş indirimler düşülmüş. Gelir ya da gider
    /// alım gününde bu kadardır (ADR 0021 İ4, T3).
    /// </summary>
    public decimal NetAmount => Amount.Amount - _discounts.Where(item => !item.IsCancelled).Sum(item => item.Amount);

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
        string? description = null,
        DateOnly? dueDate = null)
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

        if (dueDate is DateOnly date && date < chargeDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dueDate),
                "Due date cannot be before the charge date.");
        }

        Id = id;
        UserId = userId;
        CounterpartyId = counterparty.Id;
        CategoryId = category.Id;
        Direction = direction;
        Amount = amount;
        Scope = scope;
        ChargeDate = chargeDate;
        DueDate = dueDate;
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
        foreach (var discount in _discounts)
        {
            discount.Cancel(cancelledAtUtc);
        }
    }

    /// <summary>
    /// İndirim: alımın bedeli düşer, gelir ya da gider alım gününde azalır,
    /// cari bakiye azalır; para hareketi yoktur (ADR 0021 İ4, T3). Aynı kimlik
    /// aynı indirimi döner.
    /// </summary>
    public CounterpartyChargeDiscount ApplyDiscount(
        Guid discountId, decimal amount, DateOnly discountDate, DateTimeOffset recordedAtUtc)
    {
        if (_discounts.SingleOrDefault(item => item.Id == discountId) is { } existing)
        {
            return existing;
        }

        if (IsCancelled) throw new InvalidOperationException("A cancelled charge cannot be discounted.");
        if (amount <= 0m || decimal.Round(amount, 4) != amount)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive with at most four decimals.");
        if (amount > NetAmount)
            throw new DebtAmountExceedsRemainingException(NetAmount);
        if (recordedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Recording time must be UTC.", nameof(recordedAtUtc));
        if (discountDate < ChargeDate || discountDate > LocalDay.LatestAllowed(recordedAtUtc))
            throw new ArgumentOutOfRangeException(
                nameof(discountDate), "The discount date cannot be before the purchase or in the future.");

        var discount = new CounterpartyChargeDiscount(
            discountId, UserId, Id, CounterpartyId, Direction, amount, discountDate, recordedAtUtc);
        _discounts.Add(discount);
        return discount;
    }

    /// <summary>İndirimi geri alır; bedel eski hâline döner.</summary>
    public CounterpartyChargeDiscount ReverseDiscount(Guid discountId, DateTimeOffset cancelledAtUtc)
    {
        if (IsCancelled) throw new InvalidOperationException("A cancelled charge cannot change.");
        var discount = _discounts.SingleOrDefault(item => item.Id == discountId) ??
            throw new ArgumentOutOfRangeException(nameof(discountId), "Discount was not found.");
        discount.Cancel(cancelledAtUtc);
        return discount;
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

/// <summary>
/// Bir cari alımın indirimi. Karşı tarafı ve yönü alımınkinin kopyasıdır:
/// cari bakiye indirimi alıma gitmeden düşebilsin diye taşınır.
/// </summary>
public sealed class CounterpartyChargeDiscount
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid ChargeId { get; }
    public Guid CounterpartyId { get; }
    public DebtDirection Direction { get; }
    public decimal Amount { get; }
    public DateOnly DiscountDate { get; }
    public DateTimeOffset RecordedAtUtc { get; }
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CounterpartyChargeDiscount() { }

    internal CounterpartyChargeDiscount(
        Guid id,
        Guid userId,
        Guid chargeId,
        Guid counterpartyId,
        DebtDirection direction,
        decimal amount,
        DateOnly discountDate,
        DateTimeOffset recordedAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Discount id cannot be empty.", nameof(id));
        Id = id;
        UserId = userId;
        ChargeId = chargeId;
        CounterpartyId = counterpartyId;
        Direction = direction;
        Amount = amount;
        DiscountDate = discountDate;
        RecordedAtUtc = recordedAtUtc;
    }

    internal void Cancel(DateTimeOffset cancelledAtUtc)
    {
        if (IsCancelled)
        {
            return;
        }

        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }
}
