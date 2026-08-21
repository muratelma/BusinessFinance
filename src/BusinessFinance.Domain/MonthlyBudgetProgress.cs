namespace BusinessFinance.Domain;

public sealed record MonthlyBudgetProgress
{
    public decimal SpentAmount { get; }
    public decimal RemainingAmount { get; }
    public decimal ExceededAmount { get; }
    public CurrencyCode Currency { get; }
    public bool IsExceeded => ExceededAmount > 0;

    internal MonthlyBudgetProgress(
        decimal spentAmount,
        decimal remainingAmount,
        decimal exceededAmount,
        CurrencyCode currency)
    {
        if (spentAmount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(spentAmount));
        }

        if (remainingAmount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(remainingAmount));
        }

        if (exceededAmount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(exceededAmount));
        }

        SpentAmount = spentAmount;
        RemainingAmount = remainingAmount;
        ExceededAmount = exceededAmount;
        Currency = currency;
    }
}
