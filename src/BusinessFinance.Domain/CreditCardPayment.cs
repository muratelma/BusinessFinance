namespace BusinessFinance.Domain;

public sealed class CreditCardPayment
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid AccountId { get; }
    public Guid CreditCardId { get; }
    public Money Amount { get; }
    public DateOnly PaymentDate { get; }
    public string? Description { get; }
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CreditCardPayment()
    {
        Amount = null!;
    }

    public CreditCardPayment(
        Guid id,
        Guid userId,
        Account account,
        CreditCard creditCard,
        Money amount,
        DateOnly paymentDate,
        string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Payment id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(creditCard);
        ArgumentNullException.ThrowIfNull(amount);
        if (account.UserId != userId || creditCard.UserId != userId)
        {
            throw new ArgumentException("Account and card must belong to the payment user.");
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException("An inactive account cannot fund a card payment.");
        }

        if (amount.Currency != account.Currency || amount.Currency != creditCard.Limit.Currency)
        {
            throw new ArgumentException(
                "Payment, account and card must use the same currency.",
                nameof(amount));
        }

        if (paymentDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(paymentDate), "Payment date is required.");
        }

        Id = id;
        UserId = userId;
        AccountId = account.Id;
        CreditCardId = creditCard.Id;
        Amount = amount;
        PaymentDate = paymentDate;
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
                $"Payment description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        return normalized;
    }
}
