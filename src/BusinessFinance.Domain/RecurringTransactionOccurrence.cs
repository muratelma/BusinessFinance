using System.Globalization;

namespace BusinessFinance.Domain;

public sealed class RecurringTransactionOccurrence
{
    public const int OccurrenceKeyLength = 41;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid RecurringTransactionId { get; }
    public string OccurrenceKey { get; }
    public RecurringSourceType SourceType { get; }
    public Guid? AccountId { get; }
    public Guid? CreditCardId { get; }
    public Guid CategoryId { get; }
    public Money Amount { get; }
    public RecurringTransactionKind Kind { get; }

    /// <summary>
    /// Planın kapsamının üretim anındaki kopyası; gerçekleşme anında yeniden
    /// türetilmez.
    /// </summary>
    /// <remarks>
    /// Tutar, tür ve açıklama gibi kapsam da anlık görüntüdür. Plandan
    /// okumak yerine burada tutmak, planlanan görünümün kapsamı bir join
    /// olmadan SQL'de filtreleyebilmesini de sağlar.
    /// </remarks>
    public TransactionScope Scope { get; }

    public DateOnly ScheduledDate { get; }
    public string? Description { get; }
    public RecurringOccurrenceStatus Status { get; private set; }
    public Guid? BudgetTransactionId { get; private set; }
    public Guid? CreditCardChargeId { get; private set; }
    public DateTimeOffset? RealizedAtUtc { get; private set; }
    public bool IsRealized => Status == RecurringOccurrenceStatus.Realized;

    private RecurringTransactionOccurrence()
    {
        OccurrenceKey = null!;
        Amount = null!;
    }

    public static RecurringTransactionOccurrence Create(
        Guid id,
        RecurringTransaction recurringTransaction,
        DateOnly scheduledDate)
    {
        if (id == Guid.Empty) throw new ArgumentException("Occurrence id cannot be empty.", nameof(id));
        ArgumentNullException.ThrowIfNull(recurringTransaction);
        if (!recurringTransaction.IsActive || recurringTransaction.NextOccurrenceDate is null)
        {
            throw new InvalidOperationException("Only an active recurring schedule can create an occurrence.");
        }

        if (scheduledDate != recurringTransaction.NextOccurrenceDate.Value)
        {
            throw new InvalidOperationException("Occurrence date must match the next scheduled date.");
        }

        return new RecurringTransactionOccurrence(
            id,
            recurringTransaction.UserId,
            recurringTransaction.Id,
            recurringTransaction.SourceType,
            recurringTransaction.AccountId,
            recurringTransaction.CreditCardId,
            recurringTransaction.CategoryId,
            recurringTransaction.Amount,
            recurringTransaction.Kind,
            recurringTransaction.Scope,
            scheduledDate,
            recurringTransaction.Description);
    }

    /// <summary>
    /// Realizes an account-sourced occurrence into a budget transaction. Repeating
    /// the call with the same id is a no-op so a retry cannot create a second result.
    /// </summary>
    public void RealizeWithTransaction(Guid budgetTransactionId, DateTimeOffset realizedAtUtc)
    {
        if (SourceType != RecurringSourceType.Account)
        {
            throw new InvalidOperationException(
                "A credit-card occurrence cannot be realized into a budget transaction.");
        }

        if (TryBeginRealization(budgetTransactionId, BudgetTransactionId, realizedAtUtc, nameof(budgetTransactionId)))
        {
            BudgetTransactionId = budgetTransactionId;
        }
    }

    /// <summary>
    /// Realizes a credit-card occurrence into a card charge. Repeating the call with
    /// the same id is a no-op so a retry cannot create a second result.
    /// </summary>
    public void RealizeWithCharge(Guid creditCardChargeId, DateTimeOffset realizedAtUtc)
    {
        if (SourceType != RecurringSourceType.CreditCard)
        {
            throw new InvalidOperationException(
                "An account occurrence cannot be realized into a credit card charge.");
        }

        if (TryBeginRealization(creditCardChargeId, CreditCardChargeId, realizedAtUtc, nameof(creditCardChargeId)))
        {
            CreditCardChargeId = creditCardChargeId;
        }
    }

    /// <summary>
    /// Shared realization guard. Marks the occurrence realized and returns
    /// <see langword="true"/> when the caller should store the result link.
    /// Returns <see langword="false"/> when the same result already realized this
    /// occurrence, which keeps a retry idempotent instead of writing a second result.
    /// </summary>
    private bool TryBeginRealization(
        Guid resultId,
        Guid? existingResultId,
        DateTimeOffset realizedAtUtc,
        string parameterName)
    {
        if (resultId == Guid.Empty)
        {
            throw new ArgumentException("Realization result id cannot be empty.", parameterName);
        }

        if (realizedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Realization time must be UTC.", nameof(realizedAtUtc));
        }

        if (existingResultId is Guid existingId)
        {
            if (existingId != resultId)
            {
                throw new InvalidOperationException("Occurrence was already realized by another result.");
            }

            return false;
        }

        RealizedAtUtc = realizedAtUtc;
        Status = RecurringOccurrenceStatus.Realized;
        return true;
    }

    public TransactionType GetTransactionType()
    {
        return Kind == RecurringTransactionKind.Income
            ? TransactionType.Income
            : TransactionType.Expense;
    }

    public static string CreateOccurrenceKey(Guid recurringTransactionId, DateOnly scheduledDate)
    {
        if (recurringTransactionId == Guid.Empty)
        {
            throw new ArgumentException("Recurring transaction id cannot be empty.", nameof(recurringTransactionId));
        }

        if (scheduledDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(scheduledDate), "Scheduled date is required.");
        }

        return $"{recurringTransactionId:N}:{scheduledDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}";
    }

    private RecurringTransactionOccurrence(
        Guid id,
        Guid userId,
        Guid recurringTransactionId,
        RecurringSourceType sourceType,
        Guid? accountId,
        Guid? creditCardId,
        Guid categoryId,
        Money amount,
        RecurringTransactionKind kind,
        TransactionScope scope,
        DateOnly scheduledDate,
        string? description)
    {
        if (sourceType == RecurringSourceType.Account
            ? accountId is null || creditCardId is not null
            : creditCardId is null || accountId is not null)
        {
            throw new ArgumentException(
                "An occurrence must have exactly one source: an account or a credit card.",
                nameof(sourceType));
        }

        Id = id;
        UserId = userId;
        RecurringTransactionId = recurringTransactionId;
        OccurrenceKey = CreateOccurrenceKey(recurringTransactionId, scheduledDate);
        SourceType = sourceType;
        AccountId = accountId;
        CreditCardId = creditCardId;
        CategoryId = categoryId;
        // Own copy on purpose. Money is mapped as an owned value, and generating several
        // occurrences from one schedule would otherwise hand the same instance to every
        // occurrence and to the schedule itself; persistence cannot track one owned
        // instance under several owners and drops all but one amount.
        Amount = new Money(amount.Amount, amount.Currency);
        Kind = kind;
        Scope = TransactionScopeGuard.Validate(scope, nameof(scope));
        ScheduledDate = scheduledDate;
        Description = description;
        Status = RecurringOccurrenceStatus.Planned;
    }
}
