namespace BusinessFinance.Domain;

public sealed class CreditCardCharge
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid CreditCardId { get; }
    public Guid CategoryId { get; }
    public Money Amount { get; }
    public TransactionScope Scope { get; }
    public DateOnly ChargeDate { get; }
    public string? Description { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CreditCardCharge()
    {
        Amount = null!;
    }

    public CreditCardCharge(
        Guid id,
        Guid userId,
        CreditCard creditCard,
        Category category,
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

        ArgumentNullException.ThrowIfNull(creditCard);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(amount);
        if (creditCard.UserId != userId || category.UserId != userId)
        {
            throw new ArgumentException("Card and category must belong to the charge user.");
        }

        if (!creditCard.IsActive)
        {
            throw new InvalidOperationException("An inactive card cannot receive a new charge.");
        }

        if (!category.IsActive || category.Type != CategoryType.Expense)
        {
            throw new InvalidOperationException("An active expense category is required.");
        }

        if (amount.Currency != creditCard.Limit.Currency)
        {
            throw new ArgumentException("Charge and card must use the same currency.", nameof(amount));
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));

        if (chargeDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(chargeDate), "Charge date is required.");
        }

        Id = id;
        UserId = userId;
        CreditCardId = creditCard.Id;
        CategoryId = category.Id;
        Amount = amount;
        Scope = scope;
        ChargeDate = chargeDate;
        Description = NormalizeDescription(description);
    }

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
