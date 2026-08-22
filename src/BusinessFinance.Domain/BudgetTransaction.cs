namespace BusinessFinance.Domain;

public sealed class BudgetTransaction
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid AccountId { get; }
    public Guid CategoryId { get; }
    public Money Amount { get; }
    public TransactionType Type { get; }
    public TransactionScope Scope { get; }
    public DateOnly TransactionDate { get; }
    public string? Description { get; }
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private BudgetTransaction()
    {
        Amount = null!;
    }

    public BudgetTransaction(
        Guid id,
        Guid userId,
        Account account,
        Category category,
        Money amount,
        TransactionType type,
        TransactionScope scope,
        DateOnly transactionDate,
        string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Transaction id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(amount);

        if (account.UserId != userId)
        {
            throw new ArgumentException(
                "Account must belong to the transaction user.",
                nameof(account));
        }

        if (category.UserId != userId)
        {
            throw new ArgumentException(
                "Category must belong to the transaction user.",
                nameof(category));
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException("An inactive account cannot receive a transaction.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException("An inactive category cannot receive a transaction.");
        }

        if (type is not TransactionType.Income and not TransactionType.Expense)
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Transaction type is not supported.");
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));

        if (!IsCategoryCompatible(category.Type, type))
        {
            throw new ArgumentException(
                "Category type must match the transaction type.",
                nameof(category));
        }

        if (transactionDate == default)
        {
            throw new ArgumentOutOfRangeException(
                nameof(transactionDate),
                transactionDate,
                "Transaction date is required.");
        }

        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        if (normalizedDescription?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Transaction description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        Id = id;
        UserId = userId;
        AccountId = account.Id;
        CategoryId = category.Id;
        Amount = amount;
        Type = type;
        Scope = scope;
        TransactionDate = transactionDate;
        Description = normalizedDescription;
        IsCancelled = false;
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

    private static bool IsCategoryCompatible(
        CategoryType categoryType,
        TransactionType transactionType)
    {
        return (categoryType, transactionType) switch
        {
            (CategoryType.Income, TransactionType.Income) => true,
            (CategoryType.Expense, TransactionType.Expense) => true,
            _ => false
        };
    }
}
