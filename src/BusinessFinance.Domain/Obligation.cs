namespace BusinessFinance.Domain;

/// <summary>
/// Tek seferlik, vadeli bir ekonomik olay: ödenmemiş fatura veya tahsil
/// edilecek tek seferlik alacak.
/// </summary>
/// <remarks>
/// <b>Tanır, taşımaz</b> (ADR 0014). Yöne göre gelir ya da gider olayın
/// düzenleme tarihinde tanınır; hesap bakiyesi ancak
/// <see cref="ObligationSettlement"/> oluştuğunda değişir.
/// </remarks>
public sealed class Obligation
{
    public const int MaximumDescriptionLength = 500;
    private ObligationSettlement? _settlement;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid? CounterpartyId { get; }
    public Guid CategoryId { get; }
    public DebtDirection Direction { get; }
    public Money Amount { get; }
    public TransactionScope Scope { get; }
    public DateOnly IssueDate { get; }
    public DateOnly DueDate { get; }
    public string? Description { get; }

    /// <summary>
    /// Belgedeki KDV; yoksa boştur (ADR 0016). Taşınan bir bilgidir: kayıt
    /// tutarını ve gelir/gider raporunu <b>etkilemez</b>.
    /// </summary>
    public VatDetails? Vat { get; }

    /// <summary>
    /// Gider matrahtan düşülebilir mi (ADR 0016). Yalnız işletme kapsamlı gider
    /// kayıtlarında anlamlıdır; boş olması üçüncü bir durum değil, sorunun
    /// sorulmamış olmasıdır. İşletme netini <b>değiştirmez</b>.
    /// </summary>
    public bool? IsTaxDeductible { get; }

    public DateTimeOffset CreatedAtUtc { get; }
    public ObligationSettlement? Settlement => _settlement;
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public ObligationStatus Status => IsCancelled
        ? ObligationStatus.Cancelled
        : _settlement is null
            ? ObligationStatus.Open
            : ObligationStatus.Settled;

    public TransactionType RecognizedType => Direction == DebtDirection.Receivable
        ? TransactionType.Income
        : TransactionType.Expense;

    private Obligation()
    {
        Amount = null!;
    }

    public Obligation(
        Guid id,
        Guid userId,
        Category category,
        DebtDirection direction,
        Money amount,
        TransactionScope scope,
        DateOnly issueDate,
        DateOnly dueDate,
        DateTimeOffset createdAtUtc,
        Counterparty? counterparty = null,
        string? description = null,
        VatDetails? vat = null,
        bool? isTaxDeductible = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Obligation id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(amount);
        if (category.UserId != userId || counterparty is not null && counterparty.UserId != userId)
        {
            throw new ArgumentException(
                "Category and counterparty must belong to the obligation user.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive category cannot receive an obligation.");
        }

        if (counterparty is not null && !counterparty.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive counterparty cannot take on a new obligation.");
        }

        var requiredType = RequiredCategoryType(direction);
        if (category.Type != requiredType)
        {
            throw new InvalidOperationException(
                $"An active {requiredType.ToString().ToLowerInvariant()} category is required " +
                $"for a {direction.ToString().ToLowerInvariant()} obligation.");
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));
        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        }

        if (issueDate == default || issueDate > DateOnly.FromDateTime(createdAtUtc.UtcDateTime))
        {
            throw new ArgumentOutOfRangeException(
                nameof(issueDate),
                "Issue date is required and cannot be in the future.");
        }

        if (dueDate == default || dueDate < issueDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dueDate),
                "Due date cannot be before the issue date.");
        }

        VatDetails.EnsureWithinAmount(vat, amount, nameof(vat));
        var deductibility = TaxDeductibility.Validate(
            isTaxDeductible,
            scope,
            direction == DebtDirection.Payable,
            nameof(isTaxDeductible));

        Id = id;
        UserId = userId;
        CounterpartyId = counterparty?.Id;
        CategoryId = category.Id;
        Direction = direction;
        Amount = amount;
        Scope = scope;
        IssueDate = issueDate;
        DueDate = dueDate;
        CreatedAtUtc = createdAtUtc;
        Description = NormalizeDescription(description);
        Vat = vat;
        IsTaxDeductible = deductibility;
    }

    public static CategoryType RequiredCategoryType(DebtDirection direction) => direction switch
    {
        DebtDirection.Receivable => CategoryType.Income,
        DebtDirection.Payable => CategoryType.Expense,
        _ => throw new ArgumentOutOfRangeException(
            nameof(direction),
            direction,
            "Obligation direction is not supported.")
    };

    /// <summary>
    /// Gecikme saklanmaz; açık durum ve sorgulanan tarihten türetilir.
    /// </summary>
    public bool IsOverdueOn(DateOnly asOfDate)
    {
        if (asOfDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(asOfDate), "As-of date is required.");
        }

        return Status == ObligationStatus.Open && DueDate < asOfDate;
    }

    /// <summary>
    /// Yükümlülüğü kapatan tek nakit hareketini üretir. Sonraki çağrılar aynı
    /// hareketi döndürür; ikinci bir ödeme oluşturmaz.
    /// </summary>
    public ObligationSettlement Settle(
        Guid settlementId,
        Account account,
        DateOnly settlementDate,
        DateTimeOffset settledAtUtc)
    {
        if (IsCancelled)
        {
            throw new InvalidOperationException("A cancelled obligation cannot be settled.");
        }

        if (_settlement is not null)
        {
            return _settlement;
        }

        _settlement = new ObligationSettlement(
            settlementId,
            UserId,
            Id,
            account,
            Direction,
            Amount,
            IssueDate,
            settlementDate,
            settledAtUtc);
        return _settlement;
    }

    /// <summary>
    /// Silme yerine iptal. Kapanmış bir yükümlülük iptal edilirse ona bağlı
    /// nakit hareketi de aynı anda iptal edilir.
    /// </summary>
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
        _settlement?.Cancel(cancelledAtUtc);
    }

    private static string? NormalizeDescription(string? description)
    {
        var normalized = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalized?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Obligation description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        return normalized;
    }
}
