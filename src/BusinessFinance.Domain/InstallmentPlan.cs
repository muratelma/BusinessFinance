namespace BusinessFinance.Domain;

public sealed class InstallmentPlan
{
    public const int MinimumInstallmentCount = 2;
    public const int MaximumInstallmentCount = 60;
    public const int MaximumDescriptionLength = 500;

    private readonly List<InstallmentItem> _items = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid CreditCardId { get; }
    public Guid CategoryId { get; }
    public Guid ClientRequestId { get; }
    public Money TotalAmount { get; }
    public TransactionScope Scope { get; }
    public int InstallmentCount { get; }
    public DateOnly FirstInstallmentDate { get; }
    public string? Description { get; }
    public IReadOnlyCollection<InstallmentItem> Items => _items.AsReadOnly();

    private InstallmentPlan()
    {
        TotalAmount = null!;
    }

    public InstallmentPlan(
        Guid id,
        Guid userId,
        CreditCard creditCard,
        Category category,
        Guid clientRequestId,
        Money totalAmount,
        TransactionScope scope,
        int installmentCount,
        DateOnly firstInstallmentDate,
        string? description = null)
    {
        if (id == Guid.Empty) throw new ArgumentException("Plan id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (clientRequestId == Guid.Empty)
        {
            throw new ArgumentException("Client request id cannot be empty.", nameof(clientRequestId));
        }

        ArgumentNullException.ThrowIfNull(creditCard);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(totalAmount);
        if (creditCard.UserId != userId || category.UserId != userId)
        {
            throw new ArgumentException("Card and category must belong to the plan user.");
        }

        if (!creditCard.IsActive)
        {
            throw new InvalidOperationException("An inactive card cannot receive an installment plan.");
        }

        if (!category.IsActive || category.Type != CategoryType.Expense)
        {
            throw new InvalidOperationException("An active expense category is required.");
        }

        if (totalAmount.Currency != creditCard.Limit.Currency)
        {
            throw new ArgumentException("Plan and card must use the same currency.", nameof(totalAmount));
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));

        if (installmentCount is < MinimumInstallmentCount or > MaximumInstallmentCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(installmentCount),
                $"Installment count must be between {MinimumInstallmentCount} and {MaximumInstallmentCount}.");
        }

        if (firstInstallmentDate == default)
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstInstallmentDate),
                "First installment date is required.");
        }

        var minimumTotal = installmentCount * 0.0001m;
        if (totalAmount.Amount < minimumTotal)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalAmount),
                "Total amount is too small to create positive four-decimal installments.");
        }

        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        if (normalizedDescription?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Plan description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        Id = id;
        UserId = userId;
        CreditCardId = creditCard.Id;
        CategoryId = category.Id;
        ClientRequestId = clientRequestId;
        TotalAmount = totalAmount;
        Scope = scope;
        InstallmentCount = installmentCount;
        FirstInstallmentDate = firstInstallmentDate;
        Description = normalizedDescription;
        GenerateItems();
    }

    public InstallmentItem GetItem(int sequence)
    {
        return _items.SingleOrDefault(item => item.Sequence == sequence) ??
               throw new ArgumentOutOfRangeException(nameof(sequence), "Installment was not found.");
    }

    private void GenerateItems()
    {
        var baseAmount = decimal.Floor(
            TotalAmount.Amount / InstallmentCount * 10000m) / 10000m;
        for (var index = 0; index < InstallmentCount; index++)
        {
            var amount = index == InstallmentCount - 1
                ? TotalAmount.Amount - (baseAmount * (InstallmentCount - 1))
                : baseAmount;
            _items.Add(new InstallmentItem(
                Guid.NewGuid(),
                UserId,
                Id,
                index + 1,
                new Money(amount, TotalAmount.Currency),
                FirstInstallmentDate.AddMonths(index)));
        }
    }
}

public sealed class InstallmentItem
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid InstallmentPlanId { get; }
    public int Sequence { get; }
    public Money Amount { get; }
    public DateOnly ScheduledDate { get; }
    public Guid? CreditCardChargeId { get; private set; }
    public DateTimeOffset? RealizedAtUtc { get; private set; }
    public bool IsRealized => CreditCardChargeId.HasValue;

    private InstallmentItem()
    {
        Amount = null!;
    }

    internal InstallmentItem(
        Guid id,
        Guid userId,
        Guid installmentPlanId,
        int sequence,
        Money amount,
        DateOnly scheduledDate)
    {
        Id = id;
        UserId = userId;
        InstallmentPlanId = installmentPlanId;
        Sequence = sequence;
        Amount = amount;
        ScheduledDate = scheduledDate;
    }

    public void Realize(Guid creditCardChargeId, DateTimeOffset realizedAtUtc)
    {
        if (creditCardChargeId == Guid.Empty)
        {
            throw new ArgumentException("Charge id cannot be empty.", nameof(creditCardChargeId));
        }

        if (realizedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Realization time must be UTC.", nameof(realizedAtUtc));
        }

        if (CreditCardChargeId is Guid existingId)
        {
            if (existingId != creditCardChargeId)
            {
                throw new InvalidOperationException("Installment was already realized by another charge.");
            }

            return;
        }

        CreditCardChargeId = creditCardChargeId;
        RealizedAtUtc = realizedAtUtc;
    }
}
