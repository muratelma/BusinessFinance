namespace BusinessFinance.CSharpSandbox;

public class ExpenseDraft
{
    public ExpenseDraft(
        decimal amount,
        DateOnly transactionDate,
        string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Gider tutarı sıfırdan büyük olmalıdır.");
        }

        Amount = amount;
        TransactionDate = transactionDate;

        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }

    public decimal Amount { get; }

    public DateOnly TransactionDate { get; }

    public string? Description { get; }
}

public class Okul
{
    public Okul(string name, int kurulusYili)
    {
        Name = name;
        KurulusYili = kurulusYili;
    }

    public string Name { get; }

    public int KurulusYili { get; }
}
