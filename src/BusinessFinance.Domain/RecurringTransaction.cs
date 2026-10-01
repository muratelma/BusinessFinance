namespace BusinessFinance.Domain;

public sealed class RecurringTransaction
{
    public const int MaximumDescriptionLength = 500;

    /// <summary>Ocak'tan Aralık'a on iki ayın tamamı; bit 0 Ocak'tır.</summary>
    public const int AllMonthsMask = 0xFFF;

    public Guid Id { get; }
    public Guid UserId { get; }

    /// <summary>
    /// Planın kaynağı; yalnız vergi türündeki planda boş olabilir (ADR 0018
    /// T4). Kaynaksız planın kaynağı ödeme anında seçilir.
    /// </summary>
    public RecurringSourceType? SourceType { get; private set; }
    public Guid? AccountId { get; private set; }
    public Guid? CreditCardId { get; private set; }
    public Guid CategoryId { get; private set; }
    public CurrencyCode Currency { get; }

    /// <summary>
    /// Beklenen tutarın ham değeri; kalıcılık bu alanı yazar.
    /// </summary>
    public decimal? AmountValue { get; private set; }

    /// <summary>
    /// Beklenen tutar. Vergi türündeki planda boş olabilir: tutarı ödeme
    /// gününe kadar bilinmeyen vergi meşrudur ve uygulama onu tahminle
    /// doldurmaz (ADR 0018 İ5). "Sıfır" ile "bilinmiyor" ayrı şeylerdir.
    /// </summary>
    public Money? Amount => AmountValue is decimal value ? new Money(value, Currency) : null;

    public RecurringTransactionKind Kind { get; }

    /// <summary>
    /// Planın kapsamı. Gerçekleşme anında yeniden türetilmez; ürettiği her
    /// kayıt bu değeri alır, yoksa aynı plan farklı aylarda farklı kapsam
    /// üretebilirdi.
    /// </summary>
    public TransactionScope Scope { get; private set; }

    /// <summary>
    /// Doluysa plan bir vergidir (ADR 0018 T2); boşsa sıradan bir
    /// tekrarlayan plandır. Bu ayrımdan önce kurulmuş planlarda boştur:
    /// hangisinin vergi olduğu geçmişte bilinmiyor ve uydurulmaz.
    /// </summary>
    public TaxKind? TaxKind { get; }

    public RecurrenceFrequency Frequency { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    /// <summary>
    /// Ayın hangi günü. Boşsa başlangıç tarihinin günüdür (bu alandan önceki
    /// davranış). <c>31</c> ve <see cref="MonthEndBehavior.ClampToLastDay"/>
    /// birlikte "ay sonu" demektir: başlangıç 30 Eylül olsa bile sonraki
    /// kalem 31 Ekim'dir.
    /// </summary>
    public int? DayOfMonth { get; private set; }

    /// <summary>
    /// <see cref="RecurrenceFrequency.SelectedMonths"/> için ay kümesi; bit 0
    /// Ocak, bit 11 Aralık. Diğer sıklıklarda boştur.
    /// </summary>
    public int? SelectedMonths { get; private set; }

    public int? OccurrenceLimit { get; }
    public int GeneratedOccurrenceCount { get; private set; }
    public DateOnly? NextOccurrenceDate { get; private set; }
    public MonthEndBehavior MonthEndBehavior { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsTax => TaxKind is not null;

    private RecurringTransaction()
    {
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
        Money? amount,
        RecurringTransactionKind kind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate = null,
        MonthEndBehavior monthEndBehavior = MonthEndBehavior.ClampToLastDay,
        string? description = null,
        int? occurrenceLimit = null,
        TaxKind? taxKind = null,
        int? dayOfMonth = null,
        int? selectedMonths = null)
        : this(
            id,
            userId,
            RecurringSourceType.Account,
            ValidateAccountSource(account, category, amount, userId),
            null,
            category,
            amount,
            account.Currency,
            kind,
            scope,
            frequency,
            startDate,
            endDate,
            monthEndBehavior,
            description,
            occurrenceLimit,
            taxKind,
            dayOfMonth,
            selectedMonths)
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
        Money? amount,
        RecurringTransactionKind kind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate = null,
        MonthEndBehavior monthEndBehavior = MonthEndBehavior.ClampToLastDay,
        string? description = null,
        int? occurrenceLimit = null,
        TaxKind? taxKind = null,
        int? dayOfMonth = null,
        int? selectedMonths = null)
        : this(
            id,
            userId,
            RecurringSourceType.CreditCard,
            null,
            ValidateCreditCardSource(creditCard, category, amount, kind, userId),
            category,
            amount,
            creditCard.Limit.Currency,
            kind,
            scope,
            frequency,
            startDate,
            endDate,
            monthEndBehavior,
            description,
            occurrenceLimit,
            taxKind,
            dayOfMonth,
            selectedMonths)
    {
    }

    /// <summary>
    /// Kaynaksız bir vergi planı kurar: hesap ya da kart ödeme anında seçilir
    /// (ADR 0018 T4). Yalnız vergi türünde vardır; sıradan bir planın kaynağı
    /// hâlâ tam olarak biridir (ADR 0005).
    /// </summary>
    public RecurringTransaction(
        Guid id,
        Guid userId,
        Category category,
        Money? amount,
        TaxKind taxKind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate = null,
        MonthEndBehavior monthEndBehavior = MonthEndBehavior.ClampToLastDay,
        string? description = null,
        int? dayOfMonth = null,
        int? selectedMonths = null)
        : this(
            id,
            userId,
            null,
            null,
            null,
            ValidateUnsourcedCategory(category, userId),
            amount,
            CurrencyCode.TRY,
            RecurringTransactionKind.Expense,
            scope,
            frequency,
            startDate,
            endDate,
            monthEndBehavior,
            description,
            null,
            taxKind,
            dayOfMonth,
            selectedMonths)
    {
    }

    private static Category ValidateUnsourcedCategory(Category category, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(category);
        if (category.UserId != userId)
        {
            throw new ArgumentException("Category must belong to the recurring transaction user.");
        }

        return category;
    }

    private static Guid ValidateAccountSource(
        Account account,
        Category category,
        Money? amount,
        Guid userId)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);

        if (account.UserId != userId || category.UserId != userId)
        {
            throw new ArgumentException("Account and category must belong to the recurring transaction user.");
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException("An inactive account cannot receive a recurring transaction.");
        }

        if (amount is not null && amount.Currency != account.Currency)
        {
            throw new ArgumentException("Recurring transaction and account must use the same currency.", nameof(amount));
        }

        return account.Id;
    }

    private static Guid ValidateCreditCardSource(
        CreditCard creditCard,
        Category category,
        Money? amount,
        RecurringTransactionKind kind,
        Guid userId)
    {
        ArgumentNullException.ThrowIfNull(creditCard);
        ArgumentNullException.ThrowIfNull(category);

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

        if (amount is not null && amount.Currency != creditCard.Limit.Currency)
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
        RecurringSourceType? sourceType,
        Guid? accountId,
        Guid? creditCardId,
        Category category,
        Money? amount,
        CurrencyCode currency,
        RecurringTransactionKind kind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate,
        MonthEndBehavior monthEndBehavior,
        string? description,
        int? occurrenceLimit,
        TaxKind? taxKind,
        int? dayOfMonth,
        int? selectedMonths)
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

        if (!IsCategoryCompatible(category.Type, kind))
        {
            throw new ArgumentException("Category type must match the recurring transaction kind.", nameof(category));
        }

        if (occurrenceLimit is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(occurrenceLimit),
                "Occurrence limit must be greater than zero when provided.");
        }

        ValidateTaxKind(taxKind, kind);
        ValidateSource(sourceType, accountId, creditCardId, taxKind);
        ValidateAmount(amount, currency, taxKind);
        ValidateRhythm(frequency, startDate, endDate, monthEndBehavior, dayOfMonth, selectedMonths);

        Id = id;
        UserId = userId;
        SourceType = sourceType;
        AccountId = accountId;
        CreditCardId = creditCardId;
        CategoryId = category.Id;
        Currency = currency;
        AmountValue = amount?.Amount;
        Kind = kind;
        Scope = TransactionScopeGuard.Validate(scope, nameof(scope));
        TaxKind = taxKind;
        Frequency = frequency;
        StartDate = startDate;
        EndDate = endDate;
        DayOfMonth = dayOfMonth;
        SelectedMonths = selectedMonths;
        OccurrenceLimit = occurrenceLimit;
        GeneratedOccurrenceCount = 0;
        NextOccurrenceDate = startDate;
        MonthEndBehavior = monthEndBehavior;
        Description = NormalizeDescription(description, taxKind);
        IsActive = true;
    }

    /// <summary>
    /// Yedekten geri yüklenen bir planı olduğu gibi kurar.
    /// </summary>
    /// <remarks>
    /// Geri yükleme planı geçmişi yeniden oynayarak kuramaz: ritmi sonradan
    /// değiştirilmiş bir planın eski kalemleri yeni ritme uymaz. Bu yüzden
    /// durum anlık görüntü olarak gelir ve kurucunun bütün kuralları, üstüne
    /// sayaç ile sıradaki tarihin tutarlılığı yeniden denetlenir.
    /// </remarks>
    public static RecurringTransaction Restore(
        Guid id,
        Guid userId,
        Account? account,
        CreditCard? creditCard,
        Category category,
        Money? amount,
        RecurringTransactionKind kind,
        TransactionScope scope,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate,
        MonthEndBehavior monthEndBehavior,
        string? description,
        int? occurrenceLimit,
        TaxKind? taxKind,
        int? dayOfMonth,
        int? selectedMonths,
        int generatedOccurrenceCount,
        DateOnly? nextOccurrenceDate,
        bool isActive)
    {
        ArgumentNullException.ThrowIfNull(category);
        if (account is not null && creditCard is not null)
        {
            throw new ArgumentException("A recurring transaction cannot have two sources.");
        }

        // Geri yüklenen kategori ya da kaynak pasif olabilir; plan o hâliyle
        // geçmiştir. Kurucunun "pasif kaynak plan alamaz" kuralı yeni plan
        // içindir, bu yüzden denetim burada ayrıca ve aynı biçimde yapılır.
        RecurringSourceType? sourceType = account is not null
            ? RecurringSourceType.Account
            : creditCard is not null ? RecurringSourceType.CreditCard : null;

        if ((account is not null && account.UserId != userId) ||
            (creditCard is not null && creditCard.UserId != userId) ||
            category.UserId != userId)
        {
            throw new ArgumentException("Restored plan references must belong to the same user.");
        }

        if (!Enum.IsDefined(kind) || !IsCategoryCompatible(category.Type, kind))
        {
            throw new ArgumentException("Category type must match the recurring transaction kind.", nameof(category));
        }

        if (creditCard is not null && kind == RecurringTransactionKind.Income)
        {
            throw new ArgumentException("A recurring income cannot be sourced from a credit card.", nameof(kind));
        }

        if (occurrenceLimit is <= 0 ||
            generatedOccurrenceCount < 0 ||
            (occurrenceLimit is int limit && generatedOccurrenceCount > limit))
        {
            throw new ArgumentOutOfRangeException(nameof(generatedOccurrenceCount), "Occurrence count is inconsistent.");
        }

        var currency = account?.Currency ?? creditCard?.Limit.Currency ?? CurrencyCode.TRY;
        ValidateTaxKind(taxKind, kind);
        ValidateSource(sourceType, account?.Id, creditCard?.Id, taxKind);
        ValidateAmount(amount, currency, taxKind);
        ValidateRhythm(frequency, startDate, endDate, monthEndBehavior, dayOfMonth, selectedMonths);

        if (nextOccurrenceDate is DateOnly next && next < startDate)
        {
            throw new ArgumentOutOfRangeException(nameof(nextOccurrenceDate), "Next date cannot be before the start date.");
        }

        if (isActive && nextOccurrenceDate is null)
        {
            throw new ArgumentException("A completed schedule cannot be active.", nameof(isActive));
        }

        return new RecurringTransaction(
            id,
            userId,
            sourceType,
            account?.Id,
            creditCard?.Id,
            category.Id,
            currency,
            amount?.Amount,
            kind,
            scope,
            taxKind,
            frequency,
            startDate,
            endDate,
            dayOfMonth,
            selectedMonths,
            occurrenceLimit,
            generatedOccurrenceCount,
            nextOccurrenceDate,
            monthEndBehavior,
            NormalizeDescription(description, taxKind),
            isActive);
    }

    private RecurringTransaction(
        Guid id,
        Guid userId,
        RecurringSourceType? sourceType,
        Guid? accountId,
        Guid? creditCardId,
        Guid categoryId,
        CurrencyCode currency,
        decimal? amountValue,
        RecurringTransactionKind kind,
        TransactionScope scope,
        TaxKind? taxKind,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate,
        int? dayOfMonth,
        int? selectedMonths,
        int? occurrenceLimit,
        int generatedOccurrenceCount,
        DateOnly? nextOccurrenceDate,
        MonthEndBehavior monthEndBehavior,
        string? description,
        bool isActive)
    {
        if (id == Guid.Empty) throw new ArgumentException("Recurring transaction id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));

        Id = id;
        UserId = userId;
        SourceType = sourceType;
        AccountId = accountId;
        CreditCardId = creditCardId;
        CategoryId = categoryId;
        Currency = currency;
        AmountValue = amountValue;
        Kind = kind;
        Scope = TransactionScopeGuard.Validate(scope, nameof(scope));
        TaxKind = taxKind;
        Frequency = frequency;
        StartDate = startDate;
        EndDate = endDate;
        DayOfMonth = dayOfMonth;
        SelectedMonths = selectedMonths;
        OccurrenceLimit = occurrenceLimit;
        GeneratedOccurrenceCount = generatedOccurrenceCount;
        NextOccurrenceDate = nextOccurrenceDate;
        MonthEndBehavior = monthEndBehavior;
        Description = description;
        IsActive = isActive;
    }

    /// <summary>
    /// Planın ritim dışındaki alanlarını değiştirir: ad, beklenen tutar,
    /// kategori, kapsam ve kaynak.
    /// </summary>
    /// <remarks>
    /// Tür (gelir/gider) ve vergi türü değişmez: ikisi planın ne olduğudur,
    /// nasıl olduğu değil. Kaynağın ikisi birden boş olabilmesi yalnız vergi
    /// planındadır. Bekleyen kalemleri güncellemek çağıranın işidir
    /// (<see cref="RecurringTransactionOccurrence.FollowPlan"/>).
    /// </remarks>
    public void Update(
        Category category,
        Money? amount,
        TransactionScope scope,
        string? description,
        Account? account,
        CreditCard? creditCard)
    {
        ArgumentNullException.ThrowIfNull(category);
        if (category.UserId != UserId)
        {
            throw new ArgumentException("Category must belong to the recurring transaction user.", nameof(category));
        }

        if (category.Id != CategoryId && !category.IsActive)
        {
            throw new InvalidOperationException("An inactive category cannot receive a recurring transaction.");
        }

        if (!IsCategoryCompatible(category.Type, Kind))
        {
            throw new ArgumentException("Category type must match the recurring transaction kind.", nameof(category));
        }

        if (account is not null && creditCard is not null)
        {
            throw new ArgumentException("A recurring transaction must have at most one source.");
        }

        if (account is not null)
        {
            if (account.UserId != UserId) throw new ArgumentException("Account must belong to the user.", nameof(account));
            if (account.Id != AccountId && !account.IsActive)
            {
                throw new InvalidOperationException("An inactive account cannot receive a recurring transaction.");
            }
        }

        if (creditCard is not null)
        {
            if (creditCard.UserId != UserId) throw new ArgumentException("Card must belong to the user.", nameof(creditCard));
            if (Kind == RecurringTransactionKind.Income)
            {
                throw new ArgumentException("A recurring income cannot be sourced from a credit card.", nameof(creditCard));
            }

            if (creditCard.Id != CreditCardId && !creditCard.IsActive)
            {
                throw new InvalidOperationException("An inactive credit card cannot receive a recurring transaction.");
            }
        }

        var sourceType = account is not null
            ? RecurringSourceType.Account
            : creditCard is not null ? RecurringSourceType.CreditCard : (RecurringSourceType?)null;
        ValidateSource(sourceType, account?.Id, creditCard?.Id, TaxKind);
        ValidateAmount(amount, Currency, TaxKind);
        var normalizedDescription = NormalizeDescription(description, TaxKind);

        CategoryId = category.Id;
        AmountValue = amount?.Amount;
        Scope = TransactionScopeGuard.Validate(scope, nameof(scope));
        Description = normalizedDescription;
        SourceType = sourceType;
        AccountId = account?.Id;
        CreditCardId = creditCard?.Id;
    }

    /// <summary>
    /// Planın ritmini değiştirir; plan yeni ritimle <paramref name="startDate"/>
    /// gününden yeniden başlar.
    /// </summary>
    /// <param name="lastSettledDate">
    /// Ödenmiş ya da kapatılmış en son kalemin günü. Yeni başlangıç ondan
    /// sonra olmalıdır: geçmiş kalemler eski ritmin kaydıdır ve yerinde kalır;
    /// aynı güne ikinci bir kalem düşmesi "tek sonuç" kuralını bozardı.
    /// </param>
    /// <param name="settledCount">
    /// Kalan (ödenmiş ya da kapatılmış) kalem sayısı. Bekleyen kalemler
    /// tahmindir; çağıran onları siler ve sayaç yalnız kalanları sayar.
    /// </param>
    public void Reschedule(
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate,
        MonthEndBehavior monthEndBehavior,
        int? dayOfMonth,
        int? selectedMonths,
        DateOnly? lastSettledDate,
        int settledCount)
    {
        ValidateRhythm(frequency, startDate, endDate, monthEndBehavior, dayOfMonth, selectedMonths);
        if (lastSettledDate is DateOnly settled && startDate <= settled)
        {
            throw new ArgumentOutOfRangeException(
                nameof(startDate),
                "The new rhythm must start after the last settled occurrence.");
        }

        if (settledCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(settledCount), "Settled count cannot be negative.");
        }

        Frequency = frequency;
        StartDate = startDate;
        EndDate = endDate;
        MonthEndBehavior = monthEndBehavior;
        DayOfMonth = dayOfMonth;
        SelectedMonths = selectedMonths;
        GeneratedOccurrenceCount = settledCount;
        if (OccurrenceLimit is int limit && settledCount >= limit)
        {
            NextOccurrenceDate = null;
            IsActive = false;
            return;
        }

        NextOccurrenceDate = startDate;
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

    /// <summary>Ayın günü: açıkça verilmişse o, değilse başlangıç günü.</summary>
    private int EffectiveDay => DayOfMonth ?? StartDate.Day;

    private DateOnly CalculateFollowingDate(DateOnly occurrenceDate)
    {
        return Frequency switch
        {
            RecurrenceFrequency.Daily => occurrenceDate.AddDays(1),
            RecurrenceFrequency.Weekly => occurrenceDate.AddDays(7),
            RecurrenceFrequency.Monthly => FindMonthlyDate(occurrenceDate, monthStep: 1),
            RecurrenceFrequency.Quarterly => FindMonthlyDate(occurrenceDate, monthStep: 3),
            RecurrenceFrequency.Yearly => FindYearlyDate(occurrenceDate),
            RecurrenceFrequency.SelectedMonths => FindSelectedMonthDate(occurrenceDate),
            _ => throw new InvalidOperationException("Recurrence frequency is not supported.")
        };
    }

    private DateOnly FindMonthlyDate(DateOnly occurrenceDate, int monthStep)
    {
        var targetMonth = new DateOnly(occurrenceDate.Year, occurrenceDate.Month, 1)
            .AddMonths(monthStep);
        while (true)
        {
            if (TryPlaceInMonth(targetMonth, out var date)) return date;
            targetMonth = targetMonth.AddMonths(monthStep);
        }
    }

    private DateOnly FindYearlyDate(DateOnly occurrenceDate)
    {
        var targetMonth = new DateOnly(occurrenceDate.Year + 1, StartDate.Month, 1);
        while (true)
        {
            if (TryPlaceInMonth(targetMonth, out var date)) return date;
            targetMonth = targetMonth.AddYears(1);
        }
    }

    /// <summary>
    /// Seçili bir sonraki ay; ayın gününe sığmayan ay "atla" davranışında
    /// geçilir ve bir sonraki seçili aya bakılır.
    /// </summary>
    private DateOnly FindSelectedMonthDate(DateOnly occurrenceDate)
    {
        var mask = SelectedMonths ?? throw new InvalidOperationException("Selected months are missing.");
        var targetMonth = new DateOnly(occurrenceDate.Year, occurrenceDate.Month, 1).AddMonths(1);

        // Maske boş değildir (kurucu denetler), bu yüzden en çok 12 ayda bir
        // seçili aya varılır; "atla" en fazla birkaç tur daha ekler.
        while (true)
        {
            if (IsMonthSelected(mask, targetMonth.Month) && TryPlaceInMonth(targetMonth, out var date))
            {
                return date;
            }

            targetMonth = targetMonth.AddMonths(1);
        }
    }

    private bool TryPlaceInMonth(DateOnly monthStart, out DateOnly date)
    {
        var daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);
        if (EffectiveDay <= daysInMonth)
        {
            date = new DateOnly(monthStart.Year, monthStart.Month, EffectiveDay);
            return true;
        }

        if (MonthEndBehavior == MonthEndBehavior.ClampToLastDay)
        {
            date = new DateOnly(monthStart.Year, monthStart.Month, daysInMonth);
            return true;
        }

        date = default;
        return false;
    }

    public static bool IsMonthSelected(int mask, int month) => (mask & (1 << (month - 1))) != 0;

    private static void ValidateTaxKind(TaxKind? taxKind, RecurringTransactionKind kind)
    {
        if (taxKind is null) return;
        if (!Enum.IsDefined(taxKind.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(taxKind), taxKind, "Tax kind is not supported.");
        }

        // Vergi bir nakit çıkışıdır (İ3); fatura ödemesi türü sıradan
        // planların ayrımıdır ve vergiye bir şey katmaz.
        if (kind != RecurringTransactionKind.Expense)
        {
            throw new ArgumentException("A tax plan must be an expense.", nameof(taxKind));
        }
    }

    private static void ValidateSource(
        RecurringSourceType? sourceType,
        Guid? accountId,
        Guid? creditCardId,
        TaxKind? taxKind)
    {
        var valid = sourceType switch
        {
            RecurringSourceType.Account => accountId is not null && creditCardId is null,
            RecurringSourceType.CreditCard => creditCardId is not null && accountId is null,
            null => taxKind is not null && accountId is null && creditCardId is null,
            _ => false
        };

        if (!valid)
        {
            throw new ArgumentException(
                taxKind is null
                    ? "A recurring transaction must have exactly one source: an account or a credit card."
                    : "A tax plan can have at most one source: an account or a credit card.",
                nameof(sourceType));
        }
    }

    private static void ValidateAmount(Money? amount, CurrencyCode currency, TaxKind? taxKind)
    {
        if (amount is null)
        {
            if (taxKind is null)
            {
                throw new ArgumentException("A recurring transaction that is not a tax requires an amount.", nameof(amount));
            }

            return;
        }

        if (amount.Currency != currency)
        {
            throw new ArgumentException("Recurring amount must use the plan currency.", nameof(amount));
        }
    }

    private static void ValidateRhythm(
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate,
        MonthEndBehavior monthEndBehavior,
        int? dayOfMonth,
        int? selectedMonths)
    {
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

        if (startDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), "Start date is required.");
        }

        if (endDate is not null && endDate < startDate)
        {
            throw new ArgumentOutOfRangeException(nameof(endDate), "End date cannot be before the start date.");
        }

        if (frequency == RecurrenceFrequency.SelectedMonths)
        {
            if (selectedMonths is not int mask || mask is < 1 or > AllMonthsMask)
            {
                throw new ArgumentException("Selected months must name at least one month.", nameof(selectedMonths));
            }

            if (!IsMonthSelected(mask, startDate.Month))
            {
                throw new ArgumentException("Start date must fall in a selected month.", nameof(startDate));
            }
        }
        else if (selectedMonths is not null)
        {
            throw new ArgumentException("Selected months apply only to the selected-months rhythm.", nameof(selectedMonths));
        }

        if (dayOfMonth is null) return;

        if (frequency is RecurrenceFrequency.Daily or RecurrenceFrequency.Weekly)
        {
            throw new ArgumentException("Day of month does not apply to daily or weekly rhythms.", nameof(dayOfMonth));
        }

        if (dayOfMonth is < 1 or > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Day of month must be between 1 and 31.");
        }

        // Başlangıç günü ritmin ilk kalemidir ve ritme uymalıdır: ya tam o gün
        // ya da ay o güne yetmiyorsa ve "ay sonu" seçiliyse ayın son günü.
        var daysInStartMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
        var matches = startDate.Day == dayOfMonth ||
                      (dayOfMonth > daysInStartMonth &&
                       monthEndBehavior == MonthEndBehavior.ClampToLastDay &&
                       startDate.Day == daysInStartMonth);
        if (!matches)
        {
            throw new ArgumentException("Start date must fall on the plan's day of month.", nameof(startDate));
        }
    }

    private static string? NormalizeDescription(string? description, TaxKind? taxKind)
    {
        var normalized = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalized?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Recurring transaction description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        // Vergi kategori adından değil kendi adından tanınır: aynı kategoride
        // Bağkur, KDV ve geçici vergi yan yana durur.
        if (taxKind is not null && normalized is null)
        {
            throw new ArgumentException("A tax plan requires a name.", nameof(description));
        }

        return normalized;
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
