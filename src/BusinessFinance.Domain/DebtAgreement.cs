namespace BusinessFinance.Domain;

public sealed class DebtAgreement
{
    public const int MaximumNameLength = 150;
    public const int MaximumDescriptionLength = 500;
    public const int MaximumInstallments = 360;
    private readonly List<DebtInstallment> _installments = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public string CounterpartyName { get; }
    public DebtDirection Direction { get; }
    public Money Principal { get; }
    public Money TotalRepayment { get; }

    /// <summary>
    /// Paradan türetilen nominal yıllık oran; kullanıcıdan alınmaz.
    /// </summary>
    /// <remarks>
    /// Oran kanonik olsaydı yuvarlama kullanıcının yazdığı toplamı
    /// değiştirirdi — 400,00 girip 399,99 görmek kabul edilemez. Bu yüzden
    /// anapara ile toplam kanoniktir, oran onlardan çözülür. Kullanıcı oranı
    /// girmek isterse çevirim uygulama katmanında yapılır ve buraya yine para
    /// gelir.
    /// </remarks>
    public decimal AnnualInterestRate { get; }

    public DebtSourceType SourceType { get; private set; }

    /// <summary>Paranın girdiği (borç) ya da çıktığı (alacak) hesap.</summary>
    public Guid? OpeningAccountId { get; private set; }

    /// <summary>Tüketimin yazıldığı kategori; yalnız gider kaynağında dolu.</summary>
    public Guid? CategoryId { get; private set; }

    /// <summary>
    /// Açılışı henüz kaydedilmemiş bir borç mu — kullanıcıdan tamamlaması
    /// istenecek olan durum.
    /// </summary>
    public bool HasUnrecordedOpening => SourceType == DebtSourceType.Unrecorded;

    public DateOnly StartDate { get; }
    public DateOnly FirstDueDate { get; }
    public int InstallmentCount { get; }
    public string? Description { get; }
    public IReadOnlyCollection<DebtInstallment> Installments => _installments.AsReadOnly();
    public decimal RemainingAmount => _installments.Where(x => !x.IsPaid).Sum(x => x.Amount.Amount);
    public bool IsClosed => _installments.All(x => x.IsPaid);

    /// <summary>Toplam faiz — sözleşmenin gerçek maliyeti.</summary>
    public decimal TotalInterest => TotalRepayment.Amount - Principal.Amount;

    /// <summary>
    /// Taksit başına anapara ve faiz payı, sözleşme anında sabitlenmiş hâliyle.
    /// </summary>
    /// <remarks>
    /// Ayrımı olmayan (bu ayrımdan önce oluşmuş) taksitler tamamı anapara
    /// sayılır. Uydurma bir faiz yazmaktansa sıfır yazmak doğrudur: o borcun
    /// açılışı da kayıtsızdır ve kullanıcı tamamladığında ikisi birden dolar.
    /// </remarks>
    public IReadOnlyList<AmortizationSchedule.InstallmentSplit> InstallmentSplits =>
        [.. _installments.OrderBy(x => x.Sequence).Select(x =>
            new AmortizationSchedule.InstallmentSplit(
                x.PrincipalPortion ?? x.Amount.Amount,
                x.InterestPortion ?? 0m))];

    private DebtAgreement()
    {
        CounterpartyName = null!;
        Principal = null!;
        TotalRepayment = null!;
    }

    public DebtAgreement(
        Guid id,
        Guid userId,
        string counterpartyName,
        DebtDirection direction,
        Money principal,
        Money totalRepayment,
        DebtSourceType sourceType,
        Account? openingAccount,
        Category? category,
        DateOnly startDate,
        DateOnly firstDueDate,
        int installmentCount,
        string? description = null)
        : this(id, userId, counterpartyName, direction, principal, totalRepayment,
            sourceType, openingAccount, category, startDate, firstDueDate,
            installmentCount, description, allowUnrecordedOpening: false)
    {
    }

    /// <summary>
    /// Açılışı kaydedilmemiş bir borcu kurar. Yalnız bu ayrımdan önce açılmış
    /// kayıtların migration'ı ve eski sürüm yedeklerin geri yüklenmesi içindir.
    /// </summary>
    public static DebtAgreement WithUnrecordedOpening(
        Guid id,
        Guid userId,
        string counterpartyName,
        DebtDirection direction,
        Money principal,
        Money totalRepayment,
        DateOnly startDate,
        DateOnly firstDueDate,
        int installmentCount,
        string? description = null) => new(
            id, userId, counterpartyName, direction, principal, totalRepayment,
            DebtSourceType.Unrecorded, null, null, startDate, firstDueDate,
            installmentCount, description, allowUnrecordedOpening: true);

    private DebtAgreement(
        Guid id,
        Guid userId,
        string counterpartyName,
        DebtDirection direction,
        Money principal,
        Money totalRepayment,
        DebtSourceType sourceType,
        Account? openingAccount,
        Category? category,
        DateOnly startDate,
        DateOnly firstDueDate,
        int installmentCount,
        string? description,
        bool allowUnrecordedOpening)
    {
        if (id == Guid.Empty) throw new ArgumentException("Debt id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        var normalizedName = counterpartyName?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.Length > MaximumNameLength)
            throw new ArgumentException($"Counterparty name is required and cannot exceed {MaximumNameLength} characters.", nameof(counterpartyName));
        if (!Enum.IsDefined(direction)) throw new ArgumentOutOfRangeException(nameof(direction));
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(totalRepayment);
        if (principal.Currency != totalRepayment.Currency || totalRepayment.Amount < principal.Amount)
            throw new ArgumentException("Total repayment must use the principal currency and cannot be below principal.", nameof(totalRepayment));
        if (!Enum.IsDefined(sourceType)) throw new ArgumentOutOfRangeException(nameof(sourceType));
        if (startDate == default || firstDueDate < startDate)
            throw new ArgumentOutOfRangeException(nameof(firstDueDate), "First due date cannot be before the start date.");
        if (installmentCount is < 1 or > MaximumInstallments)
            throw new ArgumentOutOfRangeException(nameof(installmentCount));
        if (totalRepayment.Amount < installmentCount * 0.0001m)
            throw new ArgumentOutOfRangeException(nameof(totalRepayment), "Repayment is too small for positive installments.");
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalizedDescription?.Length > MaximumDescriptionLength)
            throw new ArgumentException($"Description cannot exceed {MaximumDescriptionLength} characters.", nameof(description));

        if (sourceType == DebtSourceType.Unrecorded && !allowUnrecordedOpening)
        {
            throw new ArgumentException(
                "A new debt must record what created it.", nameof(sourceType));
        }

        if (sourceType != DebtSourceType.Unrecorded)
        {
            ValidateOpening(userId, direction, sourceType, openingAccount, category, principal.Currency);
        }

        // Oran paradan çözülür. Üst sınırı aşan bir toplam sessizce kırpılmaz;
        // kullanıcı yazdığından farklı bir borç kaydetmiş olurdu.
        AnnualInterestRate = AmortizationSchedule.AnnualInterestRateFor(
            principal.Amount, totalRepayment.Amount, installmentCount);

        Id = id;
        SourceType = sourceType;
        OpeningAccountId = openingAccount?.Id;
        CategoryId = category?.Id;
        UserId = userId;
        CounterpartyName = normalizedName;
        Direction = direction;
        Principal = principal;
        TotalRepayment = totalRepayment;
        StartDate = startDate;
        FirstDueDate = firstDueDate;
        InstallmentCount = installmentCount;
        Description = normalizedDescription;
        GenerateSchedule();
    }

    /// <summary>
    /// Açılışı kaydedilmemiş bir borcun kaynağını sonradan tamamlar.
    /// </summary>
    /// <remarks>
    /// Yalnız bir kez çalışır. Kaydedilmiş bir açılışı değiştirmek, geçmişte
    /// yazılmış gideri ya da bakiye hareketini geriye dönük silmek olurdu;
    /// düzeltme yolu bu değil, borcun kendisini yeniden kurmaktır.
    /// </remarks>
    public void RecordOpening(DebtSourceType sourceType, Account? openingAccount, Category? category)
    {
        if (!Enum.IsDefined(sourceType)) throw new ArgumentOutOfRangeException(nameof(sourceType));
        if (sourceType == DebtSourceType.Unrecorded)
        {
            throw new ArgumentException(
                "Recording an opening requires a real source.", nameof(sourceType));
        }

        if (!HasUnrecordedOpening)
        {
            throw new InvalidOperationException("This debt already has a recorded opening.");
        }

        ValidateOpening(UserId, Direction, sourceType, openingAccount, category, Principal.Currency);

        SourceType = sourceType;
        OpeningAccountId = openingAccount?.Id;
        CategoryId = category?.Id;

        // Açılışla birlikte taksitlerin anapara/faiz ayrımı da tamamlanır.
        // İkisi de aynı boşluğun parçası: bu ayrımdan önce oluşmuş borçlarda
        // ne kaynak vardı ne de ayrım.
        ApplySplits();
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
        // (gider), alacakta satış (gelir). Ters eşleşme, parayı yanlış tarafa
        // yazardı — satılan bir şey gider, tüketilen bir şey gelir olurdu.
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

        // Tam olarak bir kaynak. İkisi birden dolarsa açılış hem parayı
        // hareket ettirir hem gider/gelir yazar ve aynı olay iki kez sayılır.
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

    public DebtInstallment GetInstallment(int sequence) =>
        _installments.SingleOrDefault(x => x.Sequence == sequence) ??
        throw new ArgumentOutOfRangeException(nameof(sequence), "Debt installment was not found.");

    private void GenerateSchedule()
    {
        var baseAmount = decimal.Floor(TotalRepayment.Amount / InstallmentCount * 10000m) / 10000m;
        for (var index = 0; index < InstallmentCount; index++)
        {
            var amount = index == InstallmentCount - 1
                ? TotalRepayment.Amount - baseAmount * (InstallmentCount - 1)
                : baseAmount;
            _installments.Add(new DebtInstallment(
                Guid.NewGuid(), UserId, Id, index + 1,
                new Money(amount, TotalRepayment.Currency), FirstDueDate.AddMonths(index)));
        }

        ApplySplits();
    }

    /// <summary>
    /// Her taksite anapara/faiz payını yazar.
    /// </summary>
    private void ApplySplits()
    {
        var ordered = _installments.OrderBy(x => x.Sequence).ToArray();
        var splits = AmortizationSchedule.Split(
            Principal.Amount, AnnualInterestRate, [.. ordered.Select(x => x.Amount.Amount)]);
        for (var index = 0; index < ordered.Length; index++)
        {
            ordered[index].SetSplit(splits[index].Principal, splits[index].Interest);
        }
    }
}

public sealed class DebtInstallment
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid DebtAgreementId { get; }
    public int Sequence { get; }
    public Money Amount { get; }

    /// <summary>Taksitin anapara payı.</summary>
    /// <remarks>
    /// Saklanıyor çünkü aylık gider raporu ödenen taksitlerin faiz payını
    /// SQL'de toplamak zorunda; anüite ayrımı taksitin sırasına bağlı bir üs
    /// alma işlemi ve EF LINQ'e çevrilmiyor. Bu bir projeksiyon önbelleği
    /// değil: ayrım sözleşme anında sabitlenir, bankalar da ödeme planına
    /// basar.
    ///
    /// Bu ayrımdan önce oluşmuş taksitlerde <c>null</c>'dır. O borçların
    /// açılışı da kayıtsızdır; ikisi de <see cref="DebtAgreement.RecordOpening"/>
    /// ile birlikte tamamlanır.
    /// </remarks>
    public decimal? PrincipalPortion { get; private set; }

    /// <summary>Taksitin faiz payı — borcun gerçek maliyeti.</summary>
    public decimal? InterestPortion { get; private set; }

    public DateOnly DueDate { get; }
    public Guid? PaymentAccountId { get; private set; }
    public DateOnly? PaymentDate { get; private set; }
    public DateTimeOffset? PaidAtUtc { get; private set; }
    public bool IsPaid => PaymentAccountId.HasValue;

    private DebtInstallment() { Amount = null!; }

    internal DebtInstallment(Guid id, Guid userId, Guid debtAgreementId, int sequence, Money amount, DateOnly dueDate)
    {
        Id = id;
        UserId = userId;
        DebtAgreementId = debtAgreementId;
        Sequence = sequence;
        Amount = amount;
        DueDate = dueDate;
    }

    internal void SetSplit(decimal principalPortion, decimal interestPortion)
    {
        PrincipalPortion = principalPortion;
        InterestPortion = interestPortion;
    }

    public void MarkPaid(Account account, DateOnly paymentDate, DateTimeOffset paidAtUtc)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (account.UserId != UserId || !account.IsActive)
            throw new InvalidOperationException("An active account owned by the debt user is required.");
        if (account.Currency != Amount.Currency)
            throw new ArgumentException("Payment account currency must match the installment.", nameof(account));
        if (paymentDate == default) throw new ArgumentOutOfRangeException(nameof(paymentDate));
        if (paidAtUtc.Offset != TimeSpan.Zero) throw new ArgumentException("Payment time must be UTC.", nameof(paidAtUtc));
        if (IsPaid) throw new InvalidOperationException("Debt installment is already paid.");
        PaymentAccountId = account.Id;
        PaymentDate = paymentDate;
        PaidAtUtc = paidAtUtc;
    }
}
