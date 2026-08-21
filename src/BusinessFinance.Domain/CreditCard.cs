namespace BusinessFinance.Domain;

public sealed class CreditCard
{
    public const int MaximumNameLength = 100;
    public const int MinimumCycleDay = 1;
    public const int MaximumCycleDay = 28;

    /// <summary>
    /// Oran girilmediğinde kullanılan asgari ödeme yüzdesi.
    /// </summary>
    /// <remarks>
    /// Asgari ödeme oranını bankası ve limiti belirliyor; yürürlükteki
    /// yönetmelikte kademeli. Bu değeri koda gömüp "doğru" saymak, yönetmelik
    /// değiştiği gün uygulamanın sessizce yanlış tutar göstermesi demekti.
    /// Bu yüzden oran kart başına saklanıyor ve bu sabit yalnız makul bir
    /// başlangıç: kullanıcı kendi kartının oranını girer.
    /// </remarks>
    public const decimal DefaultMinimumPaymentRate = 20m;

    public const decimal MaximumMinimumPaymentRate = 100m;

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; private set; }
    public Money Limit { get; private set; }
    public int StatementClosingDay { get; private set; }
    public int PaymentDueDay { get; private set; }

    /// <summary>Asgari ödeme yüzdesi (20m = %20), oran değil yüzde.</summary>
    public decimal MinimumPaymentRate { get; private set; }

    public bool IsActive { get; private set; }

    private CreditCard()
    {
        Name = null!;
        Limit = null!;
    }

    public CreditCard(
        Guid id,
        Guid userId,
        string name,
        Money limit,
        int statementClosingDay,
        int paymentDueDay,
        decimal minimumPaymentRate = DefaultMinimumPaymentRate)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Credit card id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        Id = id;
        UserId = userId;
        Name = NormalizeName(name);
        Limit = ValidateLimit(limit);
        ValidateCycleDay(statementClosingDay, nameof(statementClosingDay));
        ValidateCycleDay(paymentDueDay, nameof(paymentDueDay));
        StatementClosingDay = statementClosingDay;
        PaymentDueDay = paymentDueDay;
        MinimumPaymentRate = ValidateMinimumPaymentRate(minimumPaymentRate);
        IsActive = true;
    }

    public void Update(
        string name,
        Money limit,
        int statementClosingDay,
        int paymentDueDay,
        decimal minimumPaymentRate,
        bool isActive)
    {
        Name = NormalizeName(name);
        Limit = ValidateLimit(limit);
        ValidateCycleDay(statementClosingDay, nameof(statementClosingDay));
        ValidateCycleDay(paymentDueDay, nameof(paymentDueDay));
        StatementClosingDay = statementClosingDay;
        PaymentDueDay = paymentDueDay;
        MinimumPaymentRate = ValidateMinimumPaymentRate(minimumPaymentRate);
        IsActive = isActive;
    }

    /// <summary>
    /// Ekstre borcunun asgari ödemesi.
    /// </summary>
    /// <remarks>
    /// Hesap burada, Domain'de: istemci finansal tutarı ikinci kez hesaplamaz.
    /// İki sınır var. Asgari, ekstre borcunu **aşamaz** — oran %100'ün altında
    /// olsa bile kuruş yuvarlaması küçük borçlarda tutarı borcun üstüne
    /// çıkarabilirdi. Ve yukarı yuvarlanır: aşağı yuvarlamak, "asgariyi ödedim"
    /// diyen kullanıcıyı bir kuruş eksikle gecikmeye düşürürdü.
    /// </remarks>
    public decimal CalculateMinimumPayment(decimal statementBalance)
    {
        if (statementBalance <= 0m)
        {
            return 0m;
        }

        var exact = statementBalance * MinimumPaymentRate / 100m;
        var minimum = Math.Ceiling(exact * 100m) / 100m;
        return Math.Min(statementBalance, minimum);
    }

    public decimal CalculateAvailableLimit(decimal currentDebt)
    {
        if (currentDebt < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentDebt));
        }

        return Math.Max(0m, Limit.Amount - currentDebt);
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Credit card name is required.", nameof(name));
        }

        var normalized = name.Trim();
        if (normalized.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"Credit card name cannot exceed {MaximumNameLength} characters.",
                nameof(name));
        }

        return normalized;
    }

    private static Money ValidateLimit(Money limit)
    {
        ArgumentNullException.ThrowIfNull(limit);
        if (limit.Currency != CurrencyCode.TRY)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Only TRY is currently supported.");
        }

        return limit;
    }

    private static decimal ValidateMinimumPaymentRate(decimal rate)
    {
        if (rate is < 0m or > MaximumMinimumPaymentRate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                rate,
                $"Minimum payment rate must be between 0 and {MaximumMinimumPaymentRate}.");
        }

        return rate;
    }

    private static void ValidateCycleDay(int day, string parameterName)
    {
        if (day is < MinimumCycleDay or > MaximumCycleDay)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                day,
                $"Cycle day must be between {MinimumCycleDay} and {MaximumCycleDay}.");
        }
    }
}
