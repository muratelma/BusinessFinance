namespace BusinessFinance.Domain;

public sealed class MonthlyBudget
{
    public const int MinimumYear = 2000;
    public const int MaximumYear = 2100;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid CategoryId { get; }
    public Money Limit { get; private set; }
    public TransactionScope Scope { get; }
    public int Year { get; }
    public int Month { get; }
    public DateOnly PeriodStart => new(Year, Month, 1);
    public DateOnly PeriodEnd => new(Year, Month, DateTime.DaysInMonth(Year, Month));

    private MonthlyBudget()
    {
        Limit = null!;
    }

    public MonthlyBudget(
        Guid id,
        Guid userId,
        Category category,
        Money limit,
        TransactionScope scope,
        int year,
        int month)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Budget id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(limit);

        if (category.UserId != userId)
        {
            throw new ArgumentException(
                "Category must belong to the budget user.",
                nameof(category));
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException("An inactive category cannot receive a budget.");
        }

        if (category.Type != CategoryType.Expense)
        {
            throw new ArgumentException(
                "A monthly budget requires an expense category.",
                nameof(category));
        }

        TransactionScopeGuard.Validate(scope, nameof(scope));

        if (year is < MinimumYear or > MaximumYear)
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                year,
                $"Year must be between {MinimumYear} and {MaximumYear}.");
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(month),
                month,
                "Month must be between 1 and 12.");
        }

        Id = id;
        UserId = userId;
        CategoryId = category.Id;
        Limit = limit;
        Scope = scope;
        Year = year;
        Month = month;
    }

    public void UpdateLimit(Money limit)
    {
        ArgumentNullException.ThrowIfNull(limit);

        if (limit.Currency != Limit.Currency)
        {
            throw new InvalidOperationException("Budget currency cannot be changed.");
        }

        Limit = limit;
    }
}
