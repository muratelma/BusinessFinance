namespace BusinessFinance.Domain;

/// <summary>
/// Borcun ödeme planı: borcun kimliği değil, ayarıdır (ADR 0021 İ1).
/// </summary>
public enum DebtPlanKind : byte
{
    /// <summary>Tek ödeme, bir son gün. Eski adıyla yükümlülük.</summary>
    Single = 1,

    /// <summary>Taksitler. Eski adıyla borç planı.</summary>
    Installments = 2
}

/// <summary>
/// Bir borç ya da alacak: kimin, ne karşılığında, ne kadar ve hangi gün.
/// Yükümlülük ile borç planı tek kayıttır; plan onun ayarıdır (ADR 0021).
/// </summary>
/// <remarks>
/// <para>
/// <b>Açılış tanır ya da taşır</b> (ADR 0014). Mal ya da hizmet kaynaklı borç
/// gelir ya da gideri açılış gününde tanır; nakit kaynaklı borç parayı açılış
/// gününde taşır; açılışı bilinmeyen eski borç ikisini de yapmaz (ADR 0009 §6).
/// </para>
/// <para>
/// <b>Ödeme taşır</b>: borcun kendi ödemeleri (<see cref="DebtPayment"/>) hesabı
/// değiştirir, gelir ya da gider yazmaz; yalnız ödenen taksidin faiz payı
/// ödendiği gün tanınır. Her ödeme hangi kalemi kapattığını taşır
/// (<see cref="DebtPaymentAllocation"/>).
/// </para>
/// <para>
/// Dalga 1'de (ADR 0021 T10) davranış iki eski modelinkinin aynısıdır: tek
/// ödemeli borç tek kapanışla, taksitli borç taksit taksit ve tam tutarla
/// kapanır. Kısmi ödeme ve plan değişikliği dalga 2'dedir.
/// </para>
/// </remarks>
public sealed class Debt
{
    public const int MaximumDescriptionLength = 500;
    public const int MaximumInstallments = 360;

    private readonly List<DebtScheduleItem> _items = [];
    private readonly List<DebtPayment> _payments = [];

    public Guid Id { get; }
    public Guid UserId { get; }

    /// <summary>
    /// Borcun karşı tarafı. Tek ödemeli borçta isteğe bağlıdır (elektrik
    /// idaresi); ad burada durmaz, <see cref="Counterparty"/> kaydından okunur.
    /// </summary>
    public Guid? CounterpartyId { get; }

    public DebtDirection Direction { get; }

    /// <summary>
    /// Kaydın tarafı. Açılış ya da faiz gelir/gider yazdığı için borç da
    /// gelir/gider raporunu etkiler ve taraf taşımak zorundadır.
    /// </summary>
    public TransactionScope Scope { get; }

    public Money Principal { get; }
    public Money TotalRepayment { get; }

    /// <summary>
    /// Paradan türetilen nominal yıllık oran; kullanıcıdan alınmaz.
    /// </summary>
    /// <remarks>
    /// Anapara ile toplam kanoniktir, oran onlardan çözülür; oran kanonik olsaydı
    /// yuvarlama kullanıcının yazdığı toplamı değiştirirdi (ADR 0009 §4).
    /// </remarks>
    public decimal AnnualInterestRate { get; }

    public DebtSourceType SourceType { get; private set; }

    /// <summary>Paranın girdiği (borç) ya da çıktığı (alacak) hesap; nakit kaynakta.</summary>
    public Guid? OpeningAccountId { get; private set; }

    /// <summary>Tanınan gelir ya da giderin kategorisi; mal ya da hizmet kaynağında.</summary>
    public Guid? CategoryId { get; private set; }

    /// <summary>Borcun doğduğu gün: fatura günü, borç alma günü.</summary>
    public DateOnly StartDate { get; }

    public DebtPlanKind PlanKind { get; }

    /// <summary>İlk vade; tek ödemeli borçta son gün.</summary>
    public DateOnly FirstDueDate { get; }

    public int InstallmentCount { get; }
    public string? Description { get; }

    /// <summary>
    /// Kaydın giriş anı. Tek ödemeli borçta oluşturulduğu an yazılır; taksitli
    /// borçta kalıcılık katmanı kayıt anında yazar. Eski kayıtlarda boş olabilir
    /// ve uydurulmaz.
    /// </summary>
    public DateTimeOffset? CreatedAtUtc { get; private set; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public IReadOnlyCollection<DebtScheduleItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<DebtPayment> Payments => _payments.AsReadOnly();

    public bool HasUnrecordedOpening => SourceType == DebtSourceType.Unrecorded;

    /// <summary>Henüz kapanmamış kalemlerin toplamı.</summary>
    public decimal RemainingAmount => _items.Where(item => !IsPaid(item)).Sum(item => item.Amount.Amount);

    public bool IsClosed => _items.All(IsPaid);

    /// <summary>Toplam faiz — borcun gerçek maliyeti.</summary>
    public decimal TotalInterest => TotalRepayment.Amount - Principal.Amount;

    /// <summary>
    /// Kalem başına anapara ve faiz payı. Ayrımı olmayan eski kalemler tamamı
    /// anapara sayılır.
    /// </summary>
    public IReadOnlyList<AmortizationSchedule.InstallmentSplit> InstallmentSplits =>
        [.. _items.OrderBy(item => item.Sequence).Select(item =>
            new AmortizationSchedule.InstallmentSplit(
                item.PrincipalPortion ?? item.Amount.Amount,
                item.InterestPortion ?? 0m))];

    /// <summary>Tek ödemeli borcun son günü.</summary>
    public DateOnly DueDate => FirstDueDate;

    /// <summary>
    /// Tek ödemeli borcun kapanışı: iptal edilmiş olsa da onu kapatan ödeme.
    /// </summary>
    public DebtPayment? Settlement => PlanKind == DebtPlanKind.Single ? _payments.SingleOrDefault() : null;

    public ObligationStatus Status => IsCancelled
        ? ObligationStatus.Cancelled
        : IsClosed
            ? ObligationStatus.Settled
            : ObligationStatus.Open;

    public TransactionType RecognizedType => Direction == DebtDirection.Receivable
        ? TransactionType.Income
        : TransactionType.Expense;

    private Debt()
    {
        Principal = null!;
        TotalRepayment = null!;
    }

    private Debt(
        Guid id,
        Guid userId,
        Guid? counterpartyId,
        DebtDirection direction,
        TransactionScope scope,
        Money principal,
        Money totalRepayment,
        DebtSourceType sourceType,
        Guid? openingAccountId,
        Guid? categoryId,
        DateOnly startDate,
        DebtPlanKind planKind,
        DateOnly firstDueDate,
        int installmentCount,
        string? description)
    {
        if (id == Guid.Empty) throw new ArgumentException("Debt id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (!Enum.IsDefined(direction)) throw new ArgumentOutOfRangeException(nameof(direction));
        TransactionScopeGuard.Validate(scope, nameof(scope));
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(totalRepayment);
        if (principal.Amount <= 0m)
            throw new ArgumentOutOfRangeException(nameof(principal), "Debt amount must be positive.");
        if (principal.Currency != totalRepayment.Currency || totalRepayment.Amount < principal.Amount)
            throw new ArgumentException("Total repayment must use the principal currency and cannot be below principal.", nameof(totalRepayment));
        if (!Enum.IsDefined(sourceType)) throw new ArgumentOutOfRangeException(nameof(sourceType));
        if (startDate == default || firstDueDate < startDate)
            throw new ArgumentOutOfRangeException(nameof(firstDueDate), "First due date cannot be before the start date.");
        if (installmentCount is < 1 or > MaximumInstallments)
            throw new ArgumentOutOfRangeException(nameof(installmentCount));
        if (planKind == DebtPlanKind.Single && installmentCount != 1)
            throw new ArgumentOutOfRangeException(nameof(installmentCount), "A single payment plan has one item.");
        if (totalRepayment.Amount < installmentCount * 0.0001m)
            throw new ArgumentOutOfRangeException(nameof(totalRepayment), "Repayment is too small for positive installments.");

        Id = id;
        UserId = userId;
        CounterpartyId = counterpartyId;
        Direction = direction;
        Scope = scope;
        Principal = principal;
        TotalRepayment = totalRepayment;
        SourceType = sourceType;
        OpeningAccountId = openingAccountId;
        CategoryId = categoryId;
        StartDate = startDate;
        PlanKind = planKind;
        FirstDueDate = firstDueDate;
        InstallmentCount = installmentCount;
        Description = NormalizeDescription(description);

        // Oran paradan çözülür. Üst sınırı aşan bir toplam sessizce kırpılmaz;
        // kullanıcı yazdığından farklı bir borç kaydetmiş olurdu.
        AnnualInterestRate = AmortizationSchedule.AnnualInterestRateFor(
            principal.Amount, totalRepayment.Amount, installmentCount);
    }

    /// <summary>
    /// Tek ödemeli borç: ödenmemiş fatura ya da tek seferlik alacak. Gelir ya da
    /// gider fatura gününde tanınır; hesap ancak ödemeyle değişir.
    /// </summary>
    public static Debt CreateSingle(
        Guid id,
        Guid userId,
        Category category,
        DebtDirection direction,
        Money amount,
        TransactionScope scope,
        DateOnly issueDate,
        DateOnly dueDate,
        DateTimeOffset createdAtUtc,
        Counterparty? counterparty = null,
        string? description = null)
    {
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(amount);
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (category.UserId != userId || counterparty is not null && counterparty.UserId != userId)
        {
            throw new ArgumentException("Category and counterparty must belong to the debt user.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException("An inactive category cannot receive a debt.");
        }

        if (counterparty is not null && !counterparty.IsActive)
        {
            throw new InvalidOperationException("An inactive counterparty cannot take on a new debt.");
        }

        var requiredType = RequiredCategoryType(direction);
        if (category.Type != requiredType)
        {
            throw new InvalidOperationException(
                $"An active {requiredType.ToString().ToLowerInvariant()} category is required " +
                $"for a {direction.ToString().ToLowerInvariant()} debt.");
        }

        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        }

        if (issueDate == default || issueDate > LocalDay.LatestAllowed(createdAtUtc))
        {
            throw new ArgumentOutOfRangeException(
                nameof(issueDate), "Issue date is required and cannot be in the future.");
        }

        if (dueDate == default || dueDate < issueDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dueDate), "Due date cannot be before the issue date.");
        }

        var source = direction == DebtDirection.Receivable ? DebtSourceType.Income : DebtSourceType.Expense;
        var debt = new Debt(
            id, userId, counterparty?.Id, direction, scope, amount,
            new Money(amount.Amount, amount.Currency), source, null, category.Id,
            issueDate, DebtPlanKind.Single, dueDate, 1, description)
        {
            CreatedAtUtc = createdAtUtc
        };

        // Tek kalemin kimliği borcunkidir: eski yükümlülüğün kimliği hem borcu
        // hem vadesini adlandırır ve yeni kayıtlar aynı kuralla kurulur.
        debt._items.Add(new DebtScheduleItem(
            id, userId, id, 1, new Money(amount.Amount, amount.Currency), dueDate,
            amount.Amount, 0m));
        return debt;
    }

    /// <summary>
    /// Taksitli borç: borç para, taksitli alış ya da taksitli satış.
    /// </summary>
    public static Debt CreateInstallments(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        DebtDirection direction,
        TransactionScope scope,
        Money principal,
        Money totalRepayment,
        DebtSourceType sourceType,
        Account? openingAccount,
        Category? category,
        DateOnly startDate,
        DateOnly firstDueDate,
        int installmentCount,
        string? description = null) =>
        CreateInstallments(
            id, userId, counterparty, direction, scope, principal, totalRepayment, sourceType,
            openingAccount, category, startDate, firstDueDate, installmentCount, description,
            allowUnrecordedOpening: false, allowInactiveCounterparty: false);

    /// <summary>
    /// Açılışı kaydedilmemiş taksitli borç. Yalnız bu ayrımdan önce açılmış
    /// kayıtların geri yüklenmesi içindir (ADR 0009 §6).
    /// </summary>
    /// <remarks>
    /// Geçmiş kaydı kurduğu için <b>pasif karşı tarafı da kabul eder</b>: geri
    /// yüklenen bir borcun karşı tarafı bugün pasif olabilir.
    /// </remarks>
    public static Debt WithUnrecordedOpening(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        DebtDirection direction,
        TransactionScope scope,
        Money principal,
        Money totalRepayment,
        DateOnly startDate,
        DateOnly firstDueDate,
        int installmentCount,
        string? description = null) =>
        CreateInstallments(
            id, userId, counterparty, direction, scope, principal, totalRepayment,
            DebtSourceType.Unrecorded, null, null, startDate, firstDueDate, installmentCount,
            description, allowUnrecordedOpening: true, allowInactiveCounterparty: true);

    private static Debt CreateInstallments(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        DebtDirection direction,
        TransactionScope scope,
        Money principal,
        Money totalRepayment,
        DebtSourceType sourceType,
        Account? openingAccount,
        Category? category,
        DateOnly startDate,
        DateOnly firstDueDate,
        int installmentCount,
        string? description,
        bool allowUnrecordedOpening,
        bool allowInactiveCounterparty)
    {
        ArgumentNullException.ThrowIfNull(counterparty);
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (counterparty.UserId != userId)
            throw new ArgumentException("Counterparty must belong to the debt user.", nameof(counterparty));
        if (!counterparty.IsActive && !allowInactiveCounterparty)
            throw new InvalidOperationException("An inactive counterparty cannot take on a new agreement.");
        if (sourceType == DebtSourceType.Unrecorded && !allowUnrecordedOpening)
            throw new ArgumentException("A new debt must record what created it.", nameof(sourceType));
        ArgumentNullException.ThrowIfNull(principal);
        if (sourceType != DebtSourceType.Unrecorded)
        {
            ValidateOpening(userId, direction, sourceType, openingAccount, category, principal.Currency);
        }

        var debt = new Debt(
            id, userId, counterparty.Id, direction, scope, principal, totalRepayment, sourceType,
            openingAccount?.Id, category?.Id, startDate, DebtPlanKind.Installments, firstDueDate,
            installmentCount, description);
        debt.GenerateSchedule();
        return debt;
    }

    public static CategoryType RequiredCategoryType(DebtDirection direction) => direction switch
    {
        DebtDirection.Receivable => CategoryType.Income,
        DebtDirection.Payable => CategoryType.Expense,
        _ => throw new ArgumentOutOfRangeException(
            nameof(direction), direction, "Debt direction is not supported.")
    };

    /// <summary>Gecikme saklanmaz; açık durum ve sorgulanan tarihten türetilir.</summary>
    public bool IsOverdueOn(DateOnly asOfDate)
    {
        if (asOfDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(asOfDate), "As-of date is required.");
        }

        return Status == ObligationStatus.Open && DueDate < asOfDate;
    }

    /// <summary>Kalemi kapatan iptal edilmemiş ödemeler kalemin tamamını karşılıyor mu.</summary>
    public bool IsPaid(DebtScheduleItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return PaidAmount(item) >= item.Amount.Amount;
    }

    /// <summary>Kalemi kapatan iptal edilmemiş ödemenin günü; yoksa boş.</summary>
    public DebtPayment? PaymentOf(DebtScheduleItem item) => _payments
        .Where(payment => !payment.IsCancelled &&
                          payment.Allocations.Any(allocation => allocation.ScheduleItemId == item.Id))
        .OrderBy(payment => payment.PaymentDate)
        .LastOrDefault();

    private decimal PaidAmount(DebtScheduleItem item) => _payments
        .Where(payment => !payment.IsCancelled)
        .SelectMany(payment => payment.Allocations)
        .Where(allocation => allocation.ScheduleItemId == item.Id)
        .Sum(allocation => allocation.Amount);

    /// <summary>
    /// Açılışı kaydedilmemiş bir borcun kaynağını sonradan tamamlar. Yalnız bir
    /// kez çalışır; taksitlerin anapara/faiz ayrımı da birlikte tamamlanır.
    /// </summary>
    public void RecordOpening(DebtSourceType sourceType, Account? openingAccount, Category? category)
    {
        if (!Enum.IsDefined(sourceType)) throw new ArgumentOutOfRangeException(nameof(sourceType));
        if (sourceType == DebtSourceType.Unrecorded)
        {
            throw new ArgumentException("Recording an opening requires a real source.", nameof(sourceType));
        }

        if (!HasUnrecordedOpening)
        {
            throw new InvalidOperationException("This debt already has a recorded opening.");
        }

        ValidateOpening(UserId, Direction, sourceType, openingAccount, category, Principal.Currency);

        SourceType = sourceType;
        OpeningAccountId = openingAccount?.Id;
        CategoryId = category?.Id;
        ApplySplits();
    }

    public DebtScheduleItem GetInstallment(int sequence) =>
        _items.SingleOrDefault(item => item.Sequence == sequence) ??
        throw new ArgumentOutOfRangeException(nameof(sequence), "Debt installment was not found.");

    /// <summary>
    /// Tek ödemeli borcu kapatan tek nakit hareketini üretir. Sonraki çağrılar
    /// aynı hareketi döndürür; ikinci bir ödeme oluşturmaz.
    /// </summary>
    /// <remarks>
    /// Alacak kartla (POS) tahsil edildiyse <paramref name="cardSettlement"/>
    /// paranın yoldaki kaydıdır (ADR 0019 T5): alacak bugün kapanır, para hesaba
    /// yatışla geçer.
    /// </remarks>
    public DebtPayment Settle(
        Guid paymentId,
        Account account,
        DateOnly settlementDate,
        DateTimeOffset settledAtUtc,
        PosSettlement? cardSettlement = null)
    {
        if (PlanKind != DebtPlanKind.Single)
        {
            throw new InvalidOperationException("Only a single payment debt is settled in one step.");
        }

        if (IsCancelled)
        {
            throw new InvalidOperationException("A cancelled debt cannot be settled.");
        }

        if (Settlement is DebtPayment existing)
        {
            return existing;
        }

        if (settlementDate == default || settlementDate < StartDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(settlementDate), "Settlement date cannot be before the issue date.");
        }

        var item = _items.Single();
        var payment = new DebtPayment(
            paymentId, UserId, Id, account, Direction,
            new Money(item.Amount.Amount, item.Amount.Currency),
            settlementDate, settledAtUtc, cardSettlement, requireFutureCheck: true);
        payment.Allocate(item, item.Amount.Amount);
        _payments.Add(payment);
        return payment;
    }

    /// <summary>
    /// Bir taksidi tam tutarla öder. Ödemenin kimliği taksidinkidir: dalga 1'de
    /// bir taksit tek ödemeyle kapanır ve akış satırı taksidin kimliğini taşır.
    /// </summary>
    public DebtPayment PayInstallment(int sequence, Account account, DateOnly paymentDate, DateTimeOffset paidAtUtc)
    {
        var item = GetInstallment(sequence);
        ArgumentNullException.ThrowIfNull(account);
        if (account.UserId != UserId || !account.IsActive)
            throw new InvalidOperationException("An active account owned by the debt user is required.");
        if (account.Currency != item.Amount.Currency)
            throw new ArgumentException("Payment account currency must match the installment.", nameof(account));
        if (paymentDate == default) throw new ArgumentOutOfRangeException(nameof(paymentDate));
        if (paidAtUtc.Offset != TimeSpan.Zero) throw new ArgumentException("Payment time must be UTC.", nameof(paidAtUtc));
        if (IsCancelled) throw new InvalidOperationException("A cancelled debt cannot be paid.");
        if (IsPaid(item)) throw new InvalidOperationException("Debt installment is already paid.");

        var payment = new DebtPayment(
            item.Id, UserId, Id, account, Direction,
            new Money(item.Amount.Amount, item.Amount.Currency),
            paymentDate, paidAtUtc, cardSettlement: null, requireFutureCheck: false);
        payment.Allocate(item, item.Amount.Amount);
        _payments.Add(payment);
        return payment;
    }

    /// <summary>
    /// Silme yerine iptal. Borç iptal edilirse ödemeleri de aynı anda iptal edilir.
    /// </summary>
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
        foreach (var payment in _payments)
        {
            payment.Cancel(cancelledAtUtc);
        }
    }

    private static void ValidateOpening(
        Guid userId,
        DebtDirection direction,
        DebtSourceType sourceType,
        Account? openingAccount,
        Category? category,
        CurrencyCode currency)
    {
        // Kategorili kaynak yöne göre değişir ve simetriktir: borçta tüketim
        // (gider), alacakta satış (gelir).
        var categoricalSource = direction == DebtDirection.Payable
            ? DebtSourceType.Expense
            : DebtSourceType.Income;
        if (sourceType != DebtSourceType.Cash && sourceType != categoricalSource)
        {
            throw new ArgumentException(
                $"A {direction.ToString().ToLowerInvariant()} can only be sourced from cash " +
                $"or a {categoricalSource.ToString().ToLowerInvariant()} category.",
                nameof(sourceType));
        }

        // Tam olarak bir kaynak: ikisi birden dolarsa açılış hem parayı hareket
        // ettirir hem gider/gelir yazar ve aynı olay iki kez sayılır.
        if (sourceType == DebtSourceType.Cash
            ? openingAccount is null || category is not null
            : category is null || openingAccount is not null)
        {
            throw new ArgumentException(
                "A debt must have exactly one source: a cash account or a category.",
                nameof(sourceType));
        }

        if (openingAccount is not null)
        {
            if (openingAccount.UserId != userId || !openingAccount.IsActive)
                throw new InvalidOperationException("An active account owned by the debt user is required.");
            if (openingAccount.Currency != currency)
                throw new ArgumentException("Opening account currency must match the principal.", nameof(openingAccount));
        }

        if (category is not null)
        {
            if (category.UserId != userId || !category.IsActive)
                throw new InvalidOperationException("An active category owned by the debt user is required.");

            var requiredType = sourceType == DebtSourceType.Income
                ? CategoryType.Income
                : CategoryType.Expense;
            if (category.Type != requiredType)
            {
                throw new ArgumentException(
                    $"A {sourceType.ToString().ToLowerInvariant()} sourced opening requires " +
                    $"a {requiredType.ToString().ToLowerInvariant()} category.",
                    nameof(category));
            }
        }
    }

    private void GenerateSchedule()
    {
        var baseAmount = decimal.Floor(TotalRepayment.Amount / InstallmentCount * 10000m) / 10000m;
        for (var index = 0; index < InstallmentCount; index++)
        {
            var amount = index == InstallmentCount - 1
                ? TotalRepayment.Amount - baseAmount * (InstallmentCount - 1)
                : baseAmount;
            _items.Add(new DebtScheduleItem(
                Guid.NewGuid(), UserId, Id, index + 1,
                new Money(amount, TotalRepayment.Currency), FirstDueDate.AddMonths(index)));
        }

        ApplySplits();
    }

    /// <summary>Her kaleme anapara/faiz payını yazar.</summary>
    private void ApplySplits()
    {
        var ordered = _items.OrderBy(item => item.Sequence).ToArray();
        var splits = AmortizationSchedule.Split(
            Principal.Amount, AnnualInterestRate, [.. ordered.Select(item => item.Amount.Amount)]);
        for (var index = 0; index < ordered.Length; index++)
        {
            ordered[index].SetSplit(splits[index].Principal, splits[index].Interest);
        }
    }

    private static string? NormalizeDescription(string? description)
    {
        var normalized = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalized?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Description cannot exceed {MaximumDescriptionLength} characters.", nameof(description));
        }

        return normalized;
    }
}

/// <summary>Borcun bir vadesi: taksit ya da tek ödemenin son günü.</summary>
public sealed class DebtScheduleItem
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid DebtId { get; }
    public int Sequence { get; }
    public Money Amount { get; }

    /// <summary>
    /// Kalemin anapara payı. Saklanıyor çünkü aylık gider raporu ödenen kalemlerin
    /// faiz payını SQL'de toplamak zorunda; ayrım borç anında sabitlenir. Bu
    /// ayrımdan önce oluşmuş eski kalemlerde boştur (ADR 0009 §5–6).
    /// </summary>
    public decimal? PrincipalPortion { get; private set; }

    /// <summary>Kalemin faiz payı — borcun gerçek maliyeti.</summary>
    public decimal? InterestPortion { get; private set; }

    public DateOnly DueDate { get; }

    private DebtScheduleItem() { Amount = null!; }

    internal DebtScheduleItem(
        Guid id,
        Guid userId,
        Guid debtId,
        int sequence,
        Money amount,
        DateOnly dueDate,
        decimal? principalPortion = null,
        decimal? interestPortion = null)
    {
        Id = id;
        UserId = userId;
        DebtId = debtId;
        Sequence = sequence;
        Amount = amount;
        DueDate = dueDate;
        PrincipalPortion = principalPortion;
        InterestPortion = interestPortion;
    }

    internal void SetSplit(decimal principalPortion, decimal interestPortion)
    {
        PrincipalPortion = principalPortion;
        InterestPortion = interestPortion;
    }
}

/// <summary>
/// Borcun ödemesi ya da tahsilatı. <b>Taşır, tanımaz</b> (ADR 0014): hesap
/// bakiyesini değiştirir; ekonomik olay borcun açılışında tanındığı için kategori
/// ve kapsam taşımaz. Kartla (POS) tahsilde paranın yoldaki kaydını taşır; hesap
/// o gün kıpırdamaz, para yatışla girer.
/// </summary>
public sealed class DebtPayment
{
    private readonly List<DebtPaymentAllocation> _allocations = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid DebtId { get; }
    public DebtDirection Direction { get; }
    public Guid AccountId { get; }
    public Money Amount { get; }
    public DateOnly PaymentDate { get; }
    public DateTimeOffset PaidAtUtc { get; }

    /// <summary>Kartla tahsilde paranın yoldaki kaydı; değilse boş.</summary>
    public Guid? PosSettlementId { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public IReadOnlyCollection<DebtPaymentAllocation> Allocations => _allocations.AsReadOnly();

    /// <summary>
    /// Tahsilat hesabı artırır, ödeme azaltır; kartla tahsilde sıfırdır çünkü
    /// para hesaba yatışla girer.
    /// </summary>
    public decimal SignedAccountEffect => PosSettlementId is not null
        ? 0m
        : Direction == DebtDirection.Receivable
            ? Amount.Amount
            : -Amount.Amount;

    private DebtPayment() { Amount = null!; }

    internal DebtPayment(
        Guid id,
        Guid userId,
        Guid debtId,
        Account account,
        DebtDirection direction,
        Money amount,
        DateOnly paymentDate,
        DateTimeOffset paidAtUtc,
        PosSettlement? cardSettlement,
        bool requireFutureCheck)
    {
        if (id == Guid.Empty) throw new ArgumentException("Payment id cannot be empty.", nameof(id));
        if (userId == Guid.Empty || debtId == Guid.Empty)
            throw new ArgumentException("Payment ownership is required.");
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(amount);
        if (account.UserId != userId)
            throw new ArgumentException("Account must belong to the debt user.", nameof(account));
        if (!account.IsActive)
            throw new InvalidOperationException("An inactive account cannot pay a debt.");
        if (!Enum.IsDefined(direction))
            throw new ArgumentOutOfRangeException(nameof(direction), direction, "Payment direction is not supported.");
        if (amount.Currency != account.Currency)
            throw new ArgumentException("Payment and account must use the same currency.", nameof(amount));
        if (paidAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Payment time must be UTC.", nameof(paidAtUtc));
        if (requireFutureCheck && paymentDate > LocalDay.LatestAllowed(paidAtUtc))
            throw new ArgumentOutOfRangeException(nameof(paymentDate), "Payment date cannot be in the future.");

        cardSettlement?.EnsureCollects(userId, account, direction, amount, paymentDate);

        Id = id;
        UserId = userId;
        DebtId = debtId;
        Direction = direction;
        AccountId = account.Id;
        Amount = amount;
        PaymentDate = paymentDate;
        PaidAtUtc = paidAtUtc;
        PosSettlementId = cardSettlement?.Id;
    }

    internal void Allocate(DebtScheduleItem item, decimal amount)
    {
        if (amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount));
        _allocations.Add(new DebtPaymentAllocation(UserId, Id, item.Id, amount));
    }

    internal void Cancel(DateTimeOffset cancelledAtUtc)
    {
        if (IsCancelled)
        {
            return;
        }

        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }
}

/// <summary>Bir ödemenin bir kalemi ne kadar kapattığı.</summary>
public sealed class DebtPaymentAllocation
{
    public Guid UserId { get; }
    public Guid PaymentId { get; }
    public Guid ScheduleItemId { get; }
    public decimal Amount { get; }

    private DebtPaymentAllocation() { }

    internal DebtPaymentAllocation(Guid userId, Guid paymentId, Guid scheduleItemId, decimal amount)
    {
        UserId = userId;
        PaymentId = paymentId;
        ScheduleItemId = scheduleItemId;
        Amount = amount;
    }
}
