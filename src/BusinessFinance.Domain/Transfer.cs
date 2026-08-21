namespace BusinessFinance.Domain;

public sealed class Transfer
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid SourceAccountId { get; }
    public Guid DestinationAccountId { get; }
    public Money Amount { get; }
    public DateOnly TransferDate { get; }
    public string? Description { get; }
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private Transfer()
    {
        Amount = null!;
    }

    public Transfer(
        Guid id,
        Guid userId,
        Account sourceAccount,
        Account destinationAccount,
        Money amount,
        DateOnly transferDate,
        string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Transfer id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(sourceAccount);
        ArgumentNullException.ThrowIfNull(destinationAccount);
        ArgumentNullException.ThrowIfNull(amount);

        if (sourceAccount.UserId != userId || destinationAccount.UserId != userId)
        {
            throw new ArgumentException(
                "Both transfer accounts must belong to the transfer user.");
        }

        if (sourceAccount.Id == destinationAccount.Id)
        {
            throw new ArgumentException(
                "Source and destination accounts must be different.",
                nameof(destinationAccount));
        }

        if (!sourceAccount.IsActive || !destinationAccount.IsActive)
        {
            throw new InvalidOperationException("An inactive account cannot participate in a transfer.");
        }

        if (sourceAccount.Currency != destinationAccount.Currency ||
            amount.Currency != sourceAccount.Currency)
        {
            throw new ArgumentException(
                "Transfer amount and both accounts must use the same currency.",
                nameof(amount));
        }

        if (transferDate == default)
        {
            throw new ArgumentOutOfRangeException(
                nameof(transferDate),
                transferDate,
                "Transfer date is required.");
        }

        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        if (normalizedDescription?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Transfer description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        Id = id;
        UserId = userId;
        SourceAccountId = sourceAccount.Id;
        DestinationAccountId = destinationAccount.Id;
        Amount = amount;
        TransferDate = transferDate;
        Description = normalizedDescription;
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
}
