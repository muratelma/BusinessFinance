namespace BusinessFinance.Domain;

public sealed class RecurringTransaction
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public RecurringSourceType SourceType { get; }
    public Guid? AccountId { get; }
    public Guid? CreditCardId { get; }
    public Guid CategoryId { get; }
    public Money Amount { get; }
    public RecurringTransactionKind Kind { get; }

    /// <summary>
    /// Planın kapsamı. Gerçekleşme anında yeniden türetilmez; ürettiği her
    /// kayıt bu değeri alır, yoksa aynı plan farklı aylarda farklı kapsam
    /// üretebilirdi.
    /// </summary>
    public TransactionScope Scope { get; }

    public RecurrenceFrequency Frequency { get; }
    public DateOnly StartDate { get; }
    public DateOnly? EndDate { get; }
    public int? OccurrenceLimit { get; }
    public int GeneratedOccurrenceCount { get; private set; }
    public DateOnly? NextOccurrenceDate { get; private set; }
    public MonthEndBehavior MonthEndBehavior { get; }
    public string? Description { get; }
    public bool IsActive { get; private set; }

    private RecurringTransaction()
    {
        Amount = null!;
    }

    /// <summary>
    /// Creates a plan funded by a cash or bank account. Income, expense and bill
    /// payment kinds are all allowed for an account source.
    /// </summary>
    public RecurringTransaction(
        Guid id,
        Guid userId,
        Account account,
        Category category,
        Money amount,
        RecurringTransactionKind kind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate = null,
        MonthEndBehavior monthEndBehavior = MonthEndBehavior.ClampToLastDay,
        string? description = null,
        int? occurrenceLimit = null)
        : this(
            id,
            userId,
            RecurringSourceType.Account,
            ValidateAccountSource(account, category, amount, userId),
            null,
            category,
            amount,
            kind,
            scope,
            frequency,
            startDate,
            endDate,
            monthEndBehavior,
            description,
            occurrenceLimit)
    {
    }

    /// <summary>
    /// Creates a plan charged to a credit card, for example a subscription or a
    /// recurring bill. Income is rejected: a card cannot receive income, and card
    /// refunds are not modelled in this stage.
    /// </summary>
    public RecurringTransaction(
        Guid id,
        Guid userId,
        CreditCard creditCard,
        Category category,
        Money amount,
        RecurringTransactionKind kind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate = null,
        MonthEndBehavior monthEndBehavior = MonthEndBehavior.ClampToLastDay,
        string? description = null,
        int? occurrenceLimit = null)
        : this(
            id,
            userId,
            RecurringSourceType.CreditCard,
            null,
            ValidateCreditCardSource(creditCard, category, amount, kind, userId),
            category,
            amount,
            kind,
            scope,
            frequency,
            startDate,
            endDate,
            monthEndBehavior,
            description,
            occurrenceLimit)
    {
    }

    private static Guid ValidateAccountSource(
        Account account,
        Category category,
        Money amount,
        Guid userId)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(amount);

        if (account.UserId != userId || category.UserId != userId)
        {
            throw new ArgumentException("Account and category must belong to the recurring transaction user.");
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException("An inactive account cannot receive a recurring transaction.");
        }

        if (amount.Currency != account.Currency)
        {
            throw new ArgumentException("Recurring transaction and account must use the same currency.", nameof(amount));
        }

        return account.Id;
    }

    private static Guid ValidateCreditCardSource(
        CreditCard creditCard,
        Category category,
        Money amount,
        RecurringTransactionKind kind,
        Guid userId)
    {
        ArgumentNullException.ThrowIfNull(creditCard);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(amount);

        if (creditCard.UserId != userId || category.UserId != userId)
        {
            throw new ArgumentException("Credit card and category must belong to the recurring transaction user.");
        }

        if (kind == RecurringTransactionKind.Income)
        {
            throw new ArgumentException(
                "A recurring income cannot be sourced from a credit card.",
                nameof(kind));
        }

        if (!creditCard.IsActive)
        {
            throw new InvalidOperationException("An inactive credit card cannot receive a recurring transaction.");
        }

        if (amount.Currency != creditCard.Limit.Currency)
        {
            throw new ArgumentException(
                "Recurring transaction and credit card must use the same currency.",
                nameof(amount));
        }

        return creditCard.Id;
    }

    private RecurringTransaction(
        Guid id,
        Guid userId,
        RecurringSourceType sourceType,
        Guid? accountId,
        Guid? creditCardId,
        Category category,
        Money amount,
        RecurringTransactionKind kind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate,
        MonthEndBehavior monthEndBehavior,
        string? description,
        int? occurrenceLimit)
    {
        if (id == Guid.Empty) throw new ArgumentException("Recurring transaction id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));

        if (!category.IsActive)
        {
            throw new InvalidOperationException("An inactive category cannot receive a recurring transaction.");
        }

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "Recurring transaction kind is not supported.");
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));

        if (!Enum.IsDefined(frequency))
        {
            throw new ArgumentOutOfRangeException(nameof(frequency), frequency, "Recurrence frequency is not supported.");
        }

        if (!Enum.IsDefined(monthEndBehavior))
        {
            throw new ArgumentOutOfRangeException(
                nameof(monthEndBehavior),
                monthEndBehavior,
                "Month-end behavior is not supported.");
        }

        if (!IsCategoryCompatible(category.Type, kind))
        {
            throw new ArgumentException("Category type must match the recurring transaction kind.", nameof(category));
        }

        if (startDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), "Start date is required.");
        }

        if (endDate is not null && endDate < startDate)
        {
            throw new ArgumentOutOfRangeException(nameof(endDate), "End date cannot be before the start date.");
        }

        if (occurrenceLimit is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(occurrenceLimit),
                "Occurrence limit must be greater than zero when provided.");
        }

        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalizedDescription?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Recurring transaction description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        if (sourceType == RecurringSourceType.Account
            ? accountId is null || creditCardId is not null
            : creditCardId is null || accountId is not null)
        {
            throw new ArgumentException(
                "A recurring transaction must have exactly one source: an account or a credit card.",
                nameof(sourceType));
        }

        Id = id;
        UserId = userId;
        SourceType = sourceType;
        AccountId = accountId;
        CreditCardId = creditCardId;
        CategoryId = category.Id;
        Amount = amount;
        Kind = kind;
        Scope = scope;
        Frequency = frequency;
        StartDate = startDate;
        EndDate = endDate;
        OccurrenceLimit = occurrenceLimit;
        GeneratedOccurrenceCount = 0;
        NextOccurrenceDate = startDate;
        MonthEndBehavior = monthEndBehavior;
        Description = normalizedDescription;
        IsActive = true;
    }

    public void AdvanceAfter(DateOnly occurrenceDate)
    {
        if (!IsActive || NextOccurrenceDate is null)
        {
            throw new InvalidOperationException("An inactive or completed schedule cannot advance.");
        }

        if (occurrenceDate != NextOccurrenceDate.Value)
        {
            throw new InvalidOperationException("Only the current occurrence can advance the schedule.");
        }

        GeneratedOccurrenceCount++;
        if (OccurrenceLimit is int occurrenceLimit && GeneratedOccurrenceCount >= occurrenceLimit)
        {
            NextOccurrenceDate = null;
            IsActive = false;
            return;
        }

        var followingDate = GetFollowingDate(occurrenceDate);
        if (followingDate is null)
        {
            NextOccurrenceDate = null;
            IsActive = false;
            return;
        }

        NextOccurrenceDate = followingDate.Value;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        if (NextOccurrenceDate is null)
        {
            throw new InvalidOperationException("A completed schedule cannot be activated.");
        }

        IsActive = true;
    }

    public DateOnly? GetFollowingDate(DateOnly occurrenceDate)
    {
        if (occurrenceDate < StartDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(occurrenceDate),
                "Occurrence date cannot be before the schedule start date.");
        }

        var followingDate = CalculateFollowingDate(occurrenceDate);
        return EndDate is not null && followingDate > EndDate.Value
            ? null
            : followingDate;
    }

    private DateOnly CalculateFollowingDate(DateOnly occurrenceDate)
    {
        return Frequency switch
        {
            RecurrenceFrequency.Daily => occurrenceDate.AddDays(1),
            RecurrenceFrequency.Weekly => occurrenceDate.AddDays(7),
            RecurrenceFrequency.Monthly => FindMonthlyDate(occurrenceDate),
            RecurrenceFrequency.Yearly => FindYearlyDate(occurrenceDate),
            _ => throw new InvalidOperationException("Recurrence frequency is not supported.")
        };
    }

    private DateOnly FindMonthlyDate(DateOnly occurrenceDate)
    {
        var targetMonth = new DateOnly(occurrenceDate.Year, occurrenceDate.Month, 1).AddMonths(1);
        while (true)
        {
            var daysInMonth = DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month);
            if (StartDate.Day <= daysInMonth)
            {
                return new DateOnly(targetMonth.Year, targetMonth.Month, StartDate.Day);
            }

            if (MonthEndBehavior == MonthEndBehavior.ClampToLastDay)
            {
                return new DateOnly(targetMonth.Year, targetMonth.Month, daysInMonth);
            }

            targetMonth = targetMonth.AddMonths(1);
        }
    }

    private DateOnly FindYearlyDate(DateOnly occurrenceDate)
    {
        var targetYear = occurrenceDate.Year + 1;
        while (true)
        {
            var daysInMonth = DateTime.DaysInMonth(targetYear, StartDate.Month);
            if (StartDate.Day <= daysInMonth)
            {
                return new DateOnly(targetYear, StartDate.Month, StartDate.Day);
            }

            if (MonthEndBehavior == MonthEndBehavior.ClampToLastDay)
            {
                return new DateOnly(targetYear, StartDate.Month, daysInMonth);
            }

            targetYear++;
        }
    }

    private static bool IsCategoryCompatible(CategoryType categoryType, RecurringTransactionKind kind)
    {
        return (categoryType, kind) switch
        {
            (CategoryType.Income, RecurringTransactionKind.Income) => true,
            (CategoryType.Expense, RecurringTransactionKind.Expense) => true,
            (CategoryType.Expense, RecurringTransactionKind.BillPayment) => true,
            _ => false
        };
    }
}
