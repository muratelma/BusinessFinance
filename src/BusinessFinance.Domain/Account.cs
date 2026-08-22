namespace BusinessFinance.Domain;

public sealed class Account
{
    public const int MaximumNameLength = 100;

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; private set; }
    public AccountType Type { get; }
    public CurrencyCode Currency { get; }
    public decimal OpeningBalance { get; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Bu hesap üzerinden girilen kayıtların varsayılan kapsamı.
    /// </summary>
    /// <remarks>
    /// Boş olması meşrudur ve eksik veri değildir: tek hesabıyla her şeyi
    /// yöneten esnaf için kapsam kategoriden türer. Boş bırakmak "kapsamı
    /// bilmiyorum" değil, "bu hesap kapsamı belirlemiyor" demektir.
    /// </remarks>
    public TransactionScope? DefaultScope { get; private set; }

    public Account(
        Guid id,
        Guid userId,
        string name,
        AccountType type,
        CurrencyCode currency,
        decimal openingBalance = 0m,
        TransactionScope? defaultScope = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Account id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name is required.", nameof(name));
        }

        var normalizedName = name.Trim();
        if (normalizedName.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"Account name cannot exceed {MaximumNameLength} characters.",
                nameof(name));
        }

        if (type is not AccountType.Cash and not AccountType.Bank)
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Account type is not supported.");
        }

        if (currency != CurrencyCode.TRY)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currency),
                currency,
                "Only TRY currency is currently supported.");
        }

        if (openingBalance < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(openingBalance),
                "Opening balance cannot be negative.");
        }

        Id = id;
        UserId = userId;
        Name = normalizedName;
        Type = type;
        Currency = currency;
        OpeningBalance = openingBalance;
        IsActive = true;
        DefaultScope = TransactionScopeGuard.ValidateOptional(
            defaultScope,
            nameof(defaultScope));
    }

    /// <summary>
    /// Varsayılan kapsamı belirler; <c>null</c> vermek etiketi kaldırır.
    /// </summary>
    public void SetDefaultScope(TransactionScope? defaultScope)
    {
        DefaultScope = TransactionScopeGuard.ValidateOptional(
            defaultScope,
            nameof(defaultScope));
    }


    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name is required.", nameof(name));
        }

        var normalizedName = name.Trim();
        if (normalizedName.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"Account name cannot exceed {MaximumNameLength} characters.",
                nameof(name));
        }

        Name = normalizedName;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
