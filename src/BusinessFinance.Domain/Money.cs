namespace BusinessFinance.Domain;

public sealed record Money
{
    public decimal Amount { get; }
    public CurrencyCode Currency { get; }

    public Money(decimal amount, CurrencyCode currency)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Money amount must be greater than zero.");
        }

        if (currency != CurrencyCode.TRY)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currency),
                currency,
                "Only TRY currency is currently supported.");
        }

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return new Money(
            Amount + other.Amount,
            Currency);
    }

}
