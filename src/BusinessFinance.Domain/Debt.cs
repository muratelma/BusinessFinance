namespace BusinessFinance.Domain;

/// <summary>
/// Borcun ödeme planı: borcun kimliği değil, ayarıdır (ADR 0021 İ1).
/// </summary>
public enum DebtPlanKind : byte
{
    /// <summary>Tek ödeme, bir son gün. Eski adıyla yükümlülük.</summary>
    Single = 1,

    /// <summary>Taksitler. Eski adıyla borç planı.</summary>
    Installments = 2,

    /// <summary>Plan yok; son gün isteğe bağlıdır. Ödendikçe kapanır.</summary>
    None = 3
}

/// <summary>
/// Borcu kapatan kaydın türü. Hepsi kalanı düşürür; yalnız ödeme para taşır
/// (ADR 0021 İ4).
/// </summary>
public enum DebtPaymentKind : byte
{
    /// <summary>Gerçek para hareketi: hesap, kredi kartı ya da POS.</summary>
    Payment = 1,

    /// <summary>Bedel düştü; gelir ya da gider borcun kendi gününde azalır. Para yok.</summary>
    Discount = 2
}

/// <summary>Kapanışların toplamı borcun kalanını aşamaz (ADR 0021 İ2).</summary>
public sealed class DebtAmountExceedsRemainingException(decimal remaining)
    : InvalidOperationException($"The amount exceeds the remaining {remaining:0.0000}.")
{
    public decimal Remaining { get; } = remaining;
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
/// Uygulamadan önceki borç da ikisini yapmaz ama kaynağını bilir (ADR 0021 İ5).
/// </para>
/// <para>
/// <b>Kapanış</b>: borç kendi kayıtlarıyla kapanır (<see cref="DebtPayment"/>).
/// Ödeme hesabı ya da kart borcunu değiştirir, gelir ya da gider yazmaz; indirim
/// para taşımaz, bedeli düşürür. Her kapanış en eski açık kalemden başlayarak
/// kalemleri kapatır (<see cref="DebtPaymentAllocation"/>) ve kalanı aşamaz.
/// </para>
/// <para>
/// <b>Plan değişince</b> eski plandaki kalemler <see cref="DebtScheduleItem.IsSuperseded"/>
/// olur ve kalan yeni kalemlere bölünür. Eski plana yapılmış bir ödeme sonradan
/// geri alınırsa tutarı yeni planın ilk açık kalemine eklenir (ADR 0021 T2).
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
    /// Borcun karşı tarafı. İsteğe bağlıdır (elektrik idaresi); ad burada
    /// durmaz, <see cref="Counterparty"/> kaydından okunur.
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

    /// <summary>
    /// Uygulamadan önce var olan, bilerek devredilmiş borç (ADR 0021 İ5): açılışı
    /// para hareketi ya da gelir/gider yazmaz; kaynağı <see cref="SourceType"/>
    /// söyler, hesap ve kategori boştur, <see cref="StartDate"/> "itibarıyla"
    /// günüdür. Açılışı bilinmeyen eski borçtan (<see cref="HasUnrecordedOpening"/>) ayrıdır.
    /// </summary>
    public bool IsCarriedOver { get; private set; }

    /// <summary>Paranın girdiği (borç) ya da çıktığı (alacak) hesap; nakit kaynakta.</summary>
    public Guid? OpeningAccountId { get; private set; }

    /// <summary>Tanınan gelir ya da giderin kategorisi; mal ya da hizmet kaynağında.</summary>
    public Guid? CategoryId { get; private set; }

    /// <summary>Borcun doğduğu gün: fatura günü, borç alma günü; devirde "itibarıyla".</summary>
    public DateOnly StartDate { get; }

    public DebtPlanKind PlanKind { get; private set; }

    /// <summary>İlk vade; tek ödemede son gün. Planı olmayan borçta boş olabilir.</summary>
    public DateOnly? FirstDueDate { get; private set; }

    public int InstallmentCount { get; private set; }
    public string? Description { get; }

    /// <summary>
    /// Kaydın giriş anı. Tek ödemeli borçta oluşturulduğu an yazılır; taksitli
    /// borçta kalıcılık katmanı kayıt anında yazar. Eski kayıtlarda boş olabilir
    /// ve uydurulmaz.
    /// </summary>
    public DateTimeOffset? CreatedAtUtc { get; private set; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    /// <summary>Kaçıncı plan; her plan değişikliğinde artar.</summary>
    public int PlanRevision { get; private set; }

    /// <summary>
    /// Her değişiklikte artan sürüm. Kalan denetimi ile yazma aynı satırda
    /// sınanır: eşzamanlı iki kapanış kalanı aşamaz (ADR 0021 İ6).
    /// </summary>
    public int Revision { get; private set; }

    public IReadOnlyCollection<DebtScheduleItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<DebtPayment> Payments => _payments.AsReadOnly();

    /// <summary>Geçerli plandaki kalemler, sırasıyla.</summary>
    public IReadOnlyList<DebtScheduleItem> CurrentItems =>
        [.. _items.Where(item => !item.IsSuperseded).OrderBy(item => item.Sequence)];

    public bool HasUnrecordedOpening => SourceType == DebtSourceType.Unrecorded;

    /// <summary>Borcun kalanı: geçerli plandaki kalemlerin açık tutarı.</summary>
    public decimal RemainingAmount => CurrentItems.Sum(OpenAmount);

    /// <summary>İptal edilmemiş kapanışların toplamı (ödeme ve indirim).</summary>
    public decimal ClosedAmount => _payments.Where(payment => !payment.IsCancelled).Sum(payment => payment.Amount.Amount);

    public bool IsClosed => RemainingAmount == 0m;

    /// <summary>Faizli borç: ilk sürümde kısmi taksit ve plan değişikliği yoktur (ADR 0021 T2).</summary>
    public bool BearsInterest => TotalRepayment.Amount > Principal.Amount;

    /// <summary>Toplam faiz — borcun gerçek maliyeti.</summary>
    public decimal TotalInterest => TotalRepayment.Amount - Principal.Amount;

    /// <summary>
    /// Kalem başına anapara ve faiz payı. Ayrımı olmayan eski kalemler tamamı
    /// anapara sayılır.
    /// </summary>
    public IReadOnlyList<AmortizationSchedule.InstallmentSplit> InstallmentSplits =>
        [.. CurrentItems.Select(item =>
            new AmortizationSchedule.InstallmentSplit(
                item.PrincipalPortion ?? item.Amount.Amount,
                item.InterestPortion ?? 0m))];

    /// <summary>Tek ödemeli borcun son günü.</summary>
    public DateOnly? DueDate => FirstDueDate;

    /// <summary>
    /// Tek ödemeli borcun kapanışı (eski yükümlülük sözleşmesi): kapandıysa onu
    /// kapatan son ödeme; iptal edilmiş borçta iptal edilmiş son ödeme.
    /// </summary>
    public DebtPayment? Settlement => PlanKind != DebtPlanKind.Single
        ? null
        : IsCancelled
            ? _payments.LastOrDefault(payment => payment.Kind == DebtPaymentKind.Payment)
            : IsClosed
                ? _payments.LastOrDefault(payment => !payment.IsCancelled && payment.Kind == DebtPaymentKind.Payment)
                : null;

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
        DateOnly? firstDueDate,
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
        if (!Enum.IsDefined(planKind)) throw new ArgumentOutOfRangeException(nameof(planKind));
        if (startDate == default)
            throw new ArgumentOutOfRangeException(nameof(startDate), "Start date is required.");
        ValidatePlan(planKind, firstDueDate, installmentCount, startDate);
        if (planKind == DebtPlanKind.None && totalRepayment.Amount > principal.Amount)
            throw new ArgumentException("An interest-bearing debt needs a plan.", nameof(planKind));
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
            amount.Amount, 0m, planRevision: 0));
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

    /// <summary>
    /// Herhangi bir planla yeni borç (ADR 0021 T1): borç para, sonra ödenecek
    /// alım ya da satış. Kişi isteğe bağlıdır; kişisiz borç açıklama taşır.
    /// </summary>
    public static Debt Create(
        Guid id,
        Guid userId,
        Counterparty? counterparty,
        DebtDirection direction,
        TransactionScope scope,
        Money principal,
        Money totalRepayment,
        DebtSourceType sourceType,
        Account? openingAccount,
        Category? category,
        DateOnly startDate,
        DebtPlanKind planKind,
        DateOnly? firstDueDate,
        int installmentCount,
        string? description,
        DateTimeOffset createdAtUtc)
    {
        if (sourceType == DebtSourceType.Unrecorded)
            throw new ArgumentException("A new debt must record what created it.", nameof(sourceType));
        ArgumentNullException.ThrowIfNull(principal);
        EnsureNewCounterparty(userId, counterparty, description);
        ValidateOpening(userId, direction, sourceType, openingAccount, category, principal.Currency);
        if (createdAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));

        var debt = new Debt(
            id, userId, counterparty?.Id, direction, scope, principal, totalRepayment, sourceType,
            openingAccount?.Id, category?.Id, startDate, planKind, firstDueDate, installmentCount, description)
        {
            CreatedAtUtc = createdAtUtc
        };
        debt.GenerateSchedule();
        return debt;
    }

    /// <summary>
    /// Uygulamadan önceki borç (ADR 0021 İ5, T5): para hareketi ve gelir/gider
    /// yazılmaz. Kaynağı bilinir; tutarlar "itibarıyla" gününün kalanıdır, faizli
    /// borçta anapara kalan anaparadır.
    /// </summary>
    public static Debt CarriedOver(
        Guid id,
        Guid userId,
        Counterparty? counterparty,
        DebtDirection direction,
        TransactionScope scope,
        DebtSourceType sourceType,
        Money remainingPrincipal,
        Money remainingTotal,
        DateOnly asOfDate,
        DebtPlanKind planKind,
        DateOnly? firstDueDate,
        int installmentCount,
        string? description,
        DateTimeOffset createdAtUtc)
    {
        if (sourceType is not (DebtSourceType.Cash or DebtSourceType.Expense or DebtSourceType.Income))
            throw new ArgumentException("A carried-over debt keeps what created it.", nameof(sourceType));
        var categoricalSource = direction == DebtDirection.Payable ? DebtSourceType.Expense : DebtSourceType.Income;
        if (sourceType != DebtSourceType.Cash && sourceType != categoricalSource)
            throw new ArgumentException("The source does not match the direction.", nameof(sourceType));
        EnsureNewCounterparty(userId, counterparty, description);
        if (createdAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));

        var debt = new Debt(
            id, userId, counterparty?.Id, direction, scope, remainingPrincipal, remainingTotal, sourceType,
            null, null, asOfDate, planKind, firstDueDate, installmentCount, description)
        {
            IsCarriedOver = true,
            CreatedAtUtc = createdAtUtc
        };
        debt.GenerateSchedule();
        return debt;
    }

    /// <summary>
    /// Yedekten geri yüklenen borç: durumu olduğu gibi kurar ve tutarlılığını
    /// denetler; hiçbir işlemi yeniden oynatmaz. Plan değişikliğinin anı
    /// tutulmadığı için oynatma, ödemelerin hangi plana yapıldığını
    /// kaybettirirdi. Kimlikleri çağıran verir (yeni kimlikler); hesap, kart ve
    /// POS kimlikleri aynı kullanıcının geri yüklenmiş kayıtlarına eşlenmiştir.
    /// </summary>
    /// <remarks>
    /// Denetlenenler: geçerli plan tam olarak <paramref name="installmentCount"/>
    /// kalemdir ve son plandır, eski planların kalemleri yerini bırakmıştır;
    /// her kapanış kendi tutarı kadar, var olan kalemlere dağılır; hiçbir kalem
    /// iptal edilmemiş kapanışlarla tutarından fazla kapanmaz; ödeme tek bir
    /// yerden (hesap ya da kart) yapılır, indirimin para yeri yoktur ve yalnız
    /// kayıtlı alım ya da satışta olur; iptal edilmiş borcun bütün kapanışları
    /// iptal edilmiştir. Kapanışlar verilen sırayla eklenir.
    /// </remarks>
    public static Debt Restore(
        Guid id,
        Guid userId,
        Counterparty? counterparty,
        DebtDirection direction,
        TransactionScope scope,
        Money principal,
        Money totalRepayment,
        DebtSourceType sourceType,
        bool isCarriedOver,
        Account? openingAccount,
        Category? category,
        DateOnly startDate,
        DebtPlanKind planKind,
        DateOnly? firstDueDate,
        int installmentCount,
        string? description,
        DateTimeOffset? createdAtUtc,
        int planRevision,
        IReadOnlyList<RestoredDebtItem> items,
        IReadOnlyList<RestoredDebtPayment> payments,
        DateTimeOffset? cancelledAtUtc)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(payments);
        if (counterparty is not null && counterparty.UserId != userId)
            throw new ArgumentException("Counterparty must belong to the debt user.", nameof(counterparty));
        if (isCarriedOver)
        {
            if (openingAccount is not null || category is not null || sourceType == DebtSourceType.Unrecorded)
                throw new ArgumentException("A carried-over debt keeps its source but records no opening.");
        }
        else if (sourceType != DebtSourceType.Unrecorded)
        {
            ValidateOpening(userId, direction, sourceType, openingAccount, category, principal.Currency);
        }

        if (createdAtUtc is { } created && created.Offset != TimeSpan.Zero)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        if (planRevision < 0) throw new ArgumentOutOfRangeException(nameof(planRevision));

        var debt = new Debt(
            id, userId, counterparty?.Id, direction, scope, principal, totalRepayment, sourceType,
            openingAccount?.Id, category?.Id, startDate, planKind, firstDueDate, installmentCount, description)
        {
            IsCarriedOver = isCarriedOver,
            CreatedAtUtc = createdAtUtc,
            PlanRevision = planRevision
        };

        if (items.Select(item => item.Sequence).Distinct().Count() != items.Count ||
            items.Any(item => item.Sequence < 1 || item.Amount <= 0m || decimal.Round(item.Amount, 4) != item.Amount ||
                              item.PlanRevision < 0 || item.PlanRevision > planRevision ||
                              item.IsSuperseded == (item.PlanRevision == planRevision)))
            throw new ArgumentException("Debt items do not form one current plan over earlier ones.", nameof(items));
        if (items.Count(item => !item.IsSuperseded) != installmentCount)
            throw new ArgumentException("The current plan does not have the installment count.", nameof(items));

        // Tek kalemli ilk planın kalemi borcun kimliğini taşır (oluşturmadaki kural).
        var firstPlan = items.Where(item => item.PlanRevision == 0).ToArray();
        var keepsDebtId = firstPlan.Length == 1 && !(planRevision == 0 && planKind == DebtPlanKind.Installments);
        var bySequence = new Dictionary<int, DebtScheduleItem>();
        foreach (var source in items.OrderBy(item => item.Sequence))
        {
            var item = new DebtScheduleItem(
                keepsDebtId && source.PlanRevision == 0 ? id : Guid.NewGuid(), userId, id, source.Sequence,
                new Money(source.Amount, principal.Currency), source.DueDate,
                source.PrincipalPortion, source.InterestPortion, source.PlanRevision);
            if (source.IsSuperseded) item.Supersede();
            debt._items.Add(item);
            bySequence[source.Sequence] = item;
        }

        var closedByItem = new Dictionary<int, decimal>();
        foreach (var source in payments)
        {
            if (!Enum.IsDefined(source.Kind)) throw new ArgumentOutOfRangeException(nameof(payments));
            if (source.Amount <= 0m || decimal.Round(source.Amount, 4) != source.Amount)
                throw new ArgumentException("A closing amount must be positive with at most four decimals.", nameof(payments));
            if (source.PaymentDate < startDate)
                throw new ArgumentException("A closing cannot be before the debt started.", nameof(payments));
            var money = new Money(source.Amount, principal.Currency);
            if (source.Kind == DebtPaymentKind.Payment)
            {
                if ((source.Account is null) == (source.Card is null))
                    throw new ArgumentException("A payment is made from exactly one account or credit card.", nameof(payments));
                if (source.Account is { } account && (account.UserId != userId || account.Currency != principal.Currency))
                    throw new ArgumentException("The payment account must belong to the debt user and its currency.", nameof(payments));
                if (source.Card is { } card &&
                    (direction != DebtDirection.Payable || card.UserId != userId || card.Limit.Currency != principal.Currency))
                    throw new ArgumentException("Only a debt can be paid by the user's credit card.", nameof(payments));
                source.CardSettlement?.EnsureCollects(userId, source.Account!, direction, money, source.PaymentDate);
            }
            else if (source.Account is not null || source.Card is not null || source.CardSettlement is not null ||
                     sourceType is not (DebtSourceType.Expense or DebtSourceType.Income) || isCarriedOver)
            {
                throw new ArgumentException("A discount moves no money and applies to a recorded purchase or sale.", nameof(payments));
            }

            if (source.Allocations.Count == 0 || source.Allocations.Sum(part => part.Amount) != source.Amount ||
                source.Allocations.Any(part => part.Amount <= 0m || !bySequence.ContainsKey(part.Sequence)))
                throw new ArgumentException("A closing must be spread over existing items for its whole amount.", nameof(payments));

            var payment = new DebtPayment(
                Guid.NewGuid(), userId, id, source.Kind, direction, money,
                source.PaymentDate, source.PaidAtUtc, source.Account?.Id, source.Card?.Id, source.CardSettlement?.Id);
            foreach (var part in source.Allocations)
            {
                payment.Allocate(bySequence[part.Sequence], part.Amount);
                if (source.CancelledAtUtc is null)
                    closedByItem[part.Sequence] = closedByItem.GetValueOrDefault(part.Sequence) + part.Amount;
            }

            if (source.CancelledAtUtc is { } cancelled) payment.Cancel(cancelled);
            else if (cancelledAtUtc is not null)
                throw new ArgumentException("A cancelled debt keeps no open closing.", nameof(payments));
            debt._payments.Add(payment);
        }

        if (closedByItem.Any(pair => pair.Value > bySequence[pair.Key].Amount.Amount))
            throw new ArgumentException("An item is closed for more than its amount.", nameof(payments));

        if (cancelledAtUtc is { } debtCancelled)
        {
            if (debtCancelled.Offset != TimeSpan.Zero)
                throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
            debt.IsCancelled = true;
            debt.CancelledAtUtc = debtCancelled;
        }

        return debt;
    }

    private static void EnsureNewCounterparty(Guid userId, Counterparty? counterparty, string? description)
    {
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (counterparty is null)
        {
            // Kişisiz borcu listede ayırt eden tek şey açıklamasıdır (CB24).
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A debt without a person needs a description.", nameof(description));
            return;
        }

        if (counterparty.UserId != userId)
            throw new ArgumentException("Counterparty must belong to the debt user.", nameof(counterparty));
        if (!counterparty.IsActive)
            throw new InvalidOperationException("An inactive counterparty cannot take on a new debt.");
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

        return Status == ObligationStatus.Open &&
               CurrentItems.Any(item => item.DueDate < asOfDate && OpenAmount(item) > 0m);
    }

    /// <summary>Kalemi kapatan iptal edilmemiş kapanışlar kalemin tamamını karşılıyor mu.</summary>
    public bool IsPaid(DebtScheduleItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return PaidAmount(item) >= item.Amount.Amount;
    }

    /// <summary>Kalemin açık tutarı; yerine yeni plan geçmiş kalemde sıfır.</summary>
    public decimal OpenAmount(DebtScheduleItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return item.IsSuperseded ? 0m : Math.Max(0m, item.Amount.Amount - PaidAmount(item));
    }

    /// <summary>Kalemi kapatan iptal edilmemiş son ödeme; yoksa boş.</summary>
    public DebtPayment? PaymentOf(DebtScheduleItem item) => _payments
        .Where(payment => !payment.IsCancelled &&
                          payment.Allocations.Any(allocation => allocation.ScheduleItemId == item.Id))
        .OrderBy(payment => payment.PaymentDate)
        .LastOrDefault();

    public decimal PaidAmount(DebtScheduleItem item) => _payments
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
        Revision++;
    }

    public DebtScheduleItem GetInstallment(int sequence) =>
        CurrentItems.SingleOrDefault(item => item.Sequence == sequence) ??
        throw new ArgumentOutOfRangeException(nameof(sequence), "Debt installment was not found.");

    /// <summary>
    /// Borca ödeme ya da tahsilat yazar: en eski açık kalemden başlar, kalanı
    /// aşamaz. Ödemede yer hesap ya da kredi kartı, tahsilatta hesap ya da POS'tur.
    /// </summary>
    /// <remarks>
    /// Kartla ödemede kart borcu artar, gider yazılmaz; gider borcun açıldığı
    /// gün yazılmıştı (ADR 0021 T4). POS'la tahsilde <paramref name="cardSettlement"/>
    /// paranın yoldaki kaydıdır; para hesaba yatışla girer. Faizli borçta yalnız
    /// tam taksitler ödenir.
    /// </remarks>
    public DebtPayment Pay(
        Guid paymentId,
        decimal amount,
        DateOnly paymentDate,
        DateTimeOffset paidAtUtc,
        Account? account,
        CreditCard? card = null,
        PosSettlement? cardSettlement = null,
        bool requireFutureCheck = true)
    {
        EnsureOpenForClosing(amount, paymentDate);
        if ((account is null) == (card is null))
            throw new ArgumentException("A payment is made from exactly one account or credit card.");
        if (card is not null && Direction != DebtDirection.Payable)
            throw new InvalidOperationException("Only a debt can be paid by credit card.");
        if (cardSettlement is not null && (Direction != DebtDirection.Receivable || account is null))
            throw new InvalidOperationException("Only a receivable is collected by card.");
        if (BearsInterest && !CoversWholeItems(amount))
            throw new InvalidOperationException("An interest-bearing installment is paid in full.");

        var payment = DebtPayment.ForPayment(
            paymentId, UserId, Id, Direction, new Money(amount, Principal.Currency),
            paymentDate, paidAtUtc, account, card, cardSettlement, requireFutureCheck);
        Allocate(payment);
        return payment;
    }

    /// <summary>
    /// İndirim: mal ya da hizmet borcunun bedeli düşer. Gelir ya da gider borcun
    /// açıldığı gün azalır; para hareketi yoktur (ADR 0021 İ4, T3).
    /// </summary>
    public DebtPayment ApplyDiscount(Guid discountId, decimal amount, DateOnly discountDate, DateTimeOffset recordedAtUtc)
    {
        if (SourceType is not (DebtSourceType.Expense or DebtSourceType.Income) || IsCarriedOver)
            throw new InvalidOperationException("Only a recorded purchase or sale can be discounted.");
        EnsureOpenForClosing(amount, discountDate);
        if (BearsInterest)
            throw new InvalidOperationException("An interest-bearing debt cannot be discounted.");

        var discount = DebtPayment.ForDiscount(
            discountId, UserId, Id, Direction, new Money(amount, Principal.Currency),
            discountDate, recordedAtUtc);
        Allocate(discount);
        return discount;
    }

    /// <summary>
    /// Bir ödemeyi ya da indirimi geri alır: yalnız kendi kapattığını açar. Eski
    /// plana yapılmışsa tutarı geçerli planın ilk açık kalemine eklenir.
    /// </summary>
    public DebtPayment ReversePayment(Guid paymentId, DateTimeOffset cancelledAtUtc)
    {
        if (IsCancelled) throw new InvalidOperationException("A cancelled debt cannot change.");
        var payment = _payments.SingleOrDefault(item => item.Id == paymentId) ??
            throw new ArgumentOutOfRangeException(nameof(paymentId), "Debt payment was not found.");
        if (payment.IsCancelled)
        {
            return payment;
        }

        payment.Cancel(cancelledAtUtc);
        var reopened = payment.Allocations
            .Where(allocation => _items.Single(item => item.Id == allocation.ScheduleItemId).IsSuperseded)
            .Sum(allocation => allocation.Amount);
        if (reopened > 0m)
        {
            var current = CurrentItems;
            var target = current.FirstOrDefault(item => OpenAmount(item) > 0m) ?? current[^1];
            target.Grow(reopened);
        }

        Revision++;
        return payment;
    }

    /// <summary>
    /// Planı değiştirir: o anki kalan yeni takvime bölünür. Gelir ya da gider
    /// yazılmaz, yapılmış ödeme silinmez (ADR 0021 İ1, T2).
    /// </summary>
    public void ChangePlan(DebtPlanKind planKind, DateOnly? firstDueDate, int installmentCount)
    {
        if (IsCancelled) throw new InvalidOperationException("A cancelled debt cannot change.");
        if (BearsInterest) throw new InvalidOperationException("An interest-bearing debt keeps its plan.");
        if (!Enum.IsDefined(planKind)) throw new ArgumentOutOfRangeException(nameof(planKind));
        ValidatePlan(planKind, firstDueDate, installmentCount, StartDate);
        var remaining = RemainingAmount;
        if (remaining <= 0m) throw new InvalidOperationException("A closed debt has no plan to change.");
        if (remaining < installmentCount * 0.0001m)
            throw new ArgumentOutOfRangeException(nameof(installmentCount), "The remaining amount is too small for that many installments.");

        var keepSplits = _items.All(item => item.PrincipalPortion is not null);
        foreach (var item in CurrentItems)
        {
            item.Supersede();
        }

        PlanRevision++;
        PlanKind = planKind;
        FirstDueDate = firstDueDate;
        InstallmentCount = installmentCount;
        var nextSequence = _items.Max(item => item.Sequence) + 1;
        var baseAmount = decimal.Floor(remaining / installmentCount * 10000m) / 10000m;
        for (var index = 0; index < installmentCount; index++)
        {
            var amount = index == installmentCount - 1
                ? remaining - baseAmount * (installmentCount - 1)
                : baseAmount;
            _items.Add(new DebtScheduleItem(
                Guid.NewGuid(), UserId, Id, nextSequence + index,
                new Money(amount, Principal.Currency), firstDueDate?.AddMonths(index),
                keepSplits ? amount : null, keepSplits ? 0m : null, PlanRevision));
        }

        Revision++;
    }

    /// <summary>
    /// Tek ödemeli borcu kalanıyla kapatır (eski yükümlülük ucu). Kapanmışsa
    /// onu kapatan ödemeyi döndürür; ikinci bir ödeme oluşturmaz.
    /// </summary>
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

        ArgumentNullException.ThrowIfNull(account);
        return Pay(paymentId, RemainingAmount, settlementDate, settledAtUtc, account, null, cardSettlement);
    }

    /// <summary>
    /// Bir taksidi tam tutarla öder (eski borç ucu). Ödemenin kimliği taksidinkidir:
    /// eski uç bir taksidi tek ödemeyle kapatır ve akış satırı taksidin kimliğini taşır.
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
        if (PaidAmount(item) > 0m) throw new InvalidOperationException("Debt installment is already paid.");

        var payment = DebtPayment.ForPayment(
            item.Id, UserId, Id, Direction, new Money(item.Amount.Amount, item.Amount.Currency),
            paymentDate, paidAtUtc, account, null, null, requireFutureCheck: false);
        payment.Allocate(item, item.Amount.Amount);
        _payments.Add(payment);
        Revision++;
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

        Revision++;
    }

    private void EnsureOpenForClosing(decimal amount, DateOnly date)
    {
        if (IsCancelled) throw new InvalidOperationException("A cancelled debt cannot be paid.");
        if (amount <= 0m || decimal.Round(amount, 4) != amount)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive with at most four decimals.");
        if (date == default || date < StartDate)
            throw new ArgumentOutOfRangeException(nameof(date), "The date cannot be before the debt started.");
        var remaining = RemainingAmount;
        if (amount > remaining) throw new DebtAmountExceedsRemainingException(remaining);
    }

    private bool CoversWholeItems(decimal amount)
    {
        var total = 0m;
        foreach (var item in CurrentItems.Where(item => OpenAmount(item) > 0m))
        {
            if (PaidAmount(item) > 0m) return false;
            total += item.Amount.Amount;
            if (total == amount) return true;
            if (total > amount) return false;
        }

        return false;
    }

    /// <summary>Kapanışı en eski açık kalemden başlayarak kalemlere böler.</summary>
    private void Allocate(DebtPayment closing)
    {
        var left = closing.Amount.Amount;
        foreach (var item in CurrentItems)
        {
            var open = OpenAmount(item);
            if (open <= 0m) continue;
            var part = Math.Min(open, left);
            closing.Allocate(item, part);
            left -= part;
            if (left == 0m) break;
        }

        _payments.Add(closing);
        Revision++;
    }

    private static void ValidatePlan(DebtPlanKind planKind, DateOnly? firstDueDate, int installmentCount, DateOnly startDate)
    {
        if (installmentCount is < 1 or > MaximumInstallments)
            throw new ArgumentOutOfRangeException(nameof(installmentCount));
        if (planKind != DebtPlanKind.Installments && installmentCount != 1)
            throw new ArgumentOutOfRangeException(nameof(installmentCount), "Only an installment plan has more than one item.");
        if (planKind != DebtPlanKind.None && firstDueDate is null)
            throw new ArgumentOutOfRangeException(nameof(firstDueDate), "A plan needs a due date.");
        if (firstDueDate is DateOnly due && due < startDate)
            throw new ArgumentOutOfRangeException(nameof(firstDueDate), "First due date cannot be before the start date.");
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
            // Tek kalemli planda kalemin kimliği borcunkidir (eski yükümlülük
            // kuralı); taksitlerin kimliği yenidir.
            _items.Add(new DebtScheduleItem(
                InstallmentCount == 1 && PlanKind != DebtPlanKind.Installments ? Id : Guid.NewGuid(),
                UserId, Id, index + 1,
                new Money(amount, TotalRepayment.Currency), FirstDueDate?.AddMonths(index),
                planRevision: PlanRevision));
        }

        ApplySplits();
    }

    /// <summary>Geçerli plandaki her kaleme anapara/faiz payını yazar.</summary>
    private void ApplySplits()
    {
        var ordered = CurrentItems;
        var splits = AmortizationSchedule.Split(
            Principal.Amount, AnnualInterestRate, [.. ordered.Select(item => item.Amount.Amount)]);
        for (var index = 0; index < ordered.Count; index++)
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

/// <summary>Yedekten gelen bir kalem; kalem sırasıyla tanınır.</summary>
public sealed record RestoredDebtItem(
    int Sequence,
    decimal Amount,
    DateOnly? DueDate,
    decimal? PrincipalPortion,
    decimal? InterestPortion,
    int PlanRevision,
    bool IsSuperseded);

/// <summary>Yedekten gelen bir kapanış ve kalemlere dağılımı.</summary>
public sealed record RestoredDebtPayment(
    DebtPaymentKind Kind,
    decimal Amount,
    DateOnly PaymentDate,
    DateTimeOffset PaidAtUtc,
    Account? Account,
    CreditCard? Card,
    PosSettlement? CardSettlement,
    DateTimeOffset? CancelledAtUtc,
    IReadOnlyList<RestoredDebtAllocation> Allocations);

public sealed record RestoredDebtAllocation(int Sequence, decimal Amount);

/// <summary>Borcun bir vadesi: taksit ya da tek ödemenin son günü.</summary>
public sealed class DebtScheduleItem
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid DebtId { get; }
    public int Sequence { get; }
    public Money Amount { get; private set; }

    /// <summary>
    /// Kalemin anapara payı. Saklanıyor çünkü aylık gider raporu ödenen kalemlerin
    /// faiz payını SQL'de toplamak zorunda; ayrım borç anında sabitlenir. Bu
    /// ayrımdan önce oluşmuş eski kalemlerde boştur (ADR 0009 §5–6).
    /// </summary>
    public decimal? PrincipalPortion { get; private set; }

    /// <summary>Kalemin faiz payı — borcun gerçek maliyeti.</summary>
    public decimal? InterestPortion { get; private set; }

    /// <summary>Vade; planı olmayan borçta boş olabilir.</summary>
    public DateOnly? DueDate { get; }

    /// <summary>Kalemin ait olduğu plan.</summary>
    public int PlanRevision { get; }

    /// <summary>Yerine yeni plan geçti; açık tutarı artık kalana girmez.</summary>
    public bool IsSuperseded { get; private set; }

    private DebtScheduleItem() { Amount = null!; }

    internal DebtScheduleItem(
        Guid id,
        Guid userId,
        Guid debtId,
        int sequence,
        Money amount,
        DateOnly? dueDate,
        decimal? principalPortion = null,
        decimal? interestPortion = null,
        int planRevision = 0)
    {
        Id = id;
        UserId = userId;
        DebtId = debtId;
        Sequence = sequence;
        Amount = amount;
        DueDate = dueDate;
        PrincipalPortion = principalPortion;
        InterestPortion = interestPortion;
        PlanRevision = planRevision;
    }

    internal void SetSplit(decimal principalPortion, decimal interestPortion)
    {
        PrincipalPortion = principalPortion;
        InterestPortion = interestPortion;
    }

    internal void Supersede() => IsSuperseded = true;

    /// <summary>Geri alınan eski plan ödemesi bu kaleme eklenir (faizsiz borçta).</summary>
    internal void Grow(decimal amount)
    {
        Amount = new Money(Amount.Amount + amount, Amount.Currency);
        if (PrincipalPortion is not null)
        {
            PrincipalPortion += amount;
        }
    }
}

/// <summary>
/// Borcun bir kapanışı: ödeme ya da tahsilat (<b>taşır</b>, ADR 0014) veya
/// indirim (bedeli düşürür, para yok). Ödemede yer hesap ya da kredi kartıdır;
/// kartla tahsilde paranın yoldaki kaydını taşır, hesap o gün kıpırdamaz.
/// </summary>
public sealed class DebtPayment
{
    private readonly List<DebtPaymentAllocation> _allocations = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid DebtId { get; }
    public DebtPaymentKind Kind { get; }
    public DebtDirection Direction { get; }

    /// <summary>Paranın çıktığı ya da girdiği hesap; kartla ödemede ve indirimde boş.</summary>
    public Guid? AccountId { get; }

    /// <summary>Borcu ödeyen kredi kartı; kart borcu bu tutarla artar.</summary>
    public Guid? CreditCardId { get; }

    public Money Amount { get; }
    public DateOnly PaymentDate { get; }
    public DateTimeOffset PaidAtUtc { get; }

    /// <summary>Kartla tahsilde paranın yoldaki kaydı; değilse boş.</summary>
    public Guid? PosSettlementId { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public IReadOnlyCollection<DebtPaymentAllocation> Allocations => _allocations.AsReadOnly();

    /// <summary>
    /// Hesabın değişimi: tahsilat artırır, ödeme azaltır; kartla ödemede, kartla
    /// tahsilde ve indirimde sıfırdır.
    /// </summary>
    public decimal SignedAccountEffect => AccountId is null || PosSettlementId is not null
        ? 0m
        : Direction == DebtDirection.Receivable
            ? Amount.Amount
            : -Amount.Amount;

    private DebtPayment() { Amount = null!; }

    internal DebtPayment(
        Guid id,
        Guid userId,
        Guid debtId,
        DebtPaymentKind kind,
        DebtDirection direction,
        Money amount,
        DateOnly paymentDate,
        DateTimeOffset paidAtUtc,
        Guid? accountId,
        Guid? creditCardId,
        Guid? posSettlementId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Payment id cannot be empty.", nameof(id));
        if (userId == Guid.Empty || debtId == Guid.Empty)
            throw new ArgumentException("Payment ownership is required.");
        ArgumentNullException.ThrowIfNull(amount);
        if (!Enum.IsDefined(direction))
            throw new ArgumentOutOfRangeException(nameof(direction), direction, "Payment direction is not supported.");
        if (paidAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Payment time must be UTC.", nameof(paidAtUtc));
        if (paymentDate == default) throw new ArgumentOutOfRangeException(nameof(paymentDate));

        Id = id;
        UserId = userId;
        DebtId = debtId;
        Kind = kind;
        Direction = direction;
        AccountId = accountId;
        CreditCardId = creditCardId;
        Amount = amount;
        PaymentDate = paymentDate;
        PaidAtUtc = paidAtUtc;
        PosSettlementId = posSettlementId;
    }

    internal static DebtPayment ForPayment(
        Guid id,
        Guid userId,
        Guid debtId,
        DebtDirection direction,
        Money amount,
        DateOnly paymentDate,
        DateTimeOffset paidAtUtc,
        Account? account,
        CreditCard? card,
        PosSettlement? cardSettlement,
        bool requireFutureCheck)
    {
        if (account is not null)
        {
            if (account.UserId != userId)
                throw new ArgumentException("Account must belong to the debt user.", nameof(account));
            if (!account.IsActive)
                throw new InvalidOperationException("An inactive account cannot pay a debt.");
            if (amount.Currency != account.Currency)
                throw new ArgumentException("Payment and account must use the same currency.", nameof(amount));
        }

        if (card is not null)
        {
            if (card.UserId != userId)
                throw new ArgumentException("Card must belong to the debt user.", nameof(card));
            if (!card.IsActive)
                throw new InvalidOperationException("An inactive card cannot pay a debt.");
            if (amount.Currency != card.Limit.Currency)
                throw new ArgumentException("Payment and card must use the same currency.", nameof(amount));
        }

        if (paidAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Payment time must be UTC.", nameof(paidAtUtc));
        if (requireFutureCheck && paymentDate > LocalDay.LatestAllowed(paidAtUtc))
            throw new ArgumentOutOfRangeException(nameof(paymentDate), "Payment date cannot be in the future.");
        if (cardSettlement is not null)
            cardSettlement.EnsureCollects(userId, account!, direction, amount, paymentDate);

        return new DebtPayment(
            id, userId, debtId, DebtPaymentKind.Payment, direction, amount, paymentDate, paidAtUtc,
            account?.Id, card?.Id, cardSettlement?.Id);
    }

    internal static DebtPayment ForDiscount(
        Guid id,
        Guid userId,
        Guid debtId,
        DebtDirection direction,
        Money amount,
        DateOnly discountDate,
        DateTimeOffset recordedAtUtc)
    {
        if (discountDate > LocalDay.LatestAllowed(recordedAtUtc))
            throw new ArgumentOutOfRangeException(nameof(discountDate), "Discount date cannot be in the future.");
        return new DebtPayment(
            id, userId, debtId, DebtPaymentKind.Discount, direction, amount, discountDate, recordedAtUtc,
            null, null, null);
    }

    internal void Allocate(DebtScheduleItem item, decimal amount)
    {
        if (amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount));
        _allocations.Add(new DebtPaymentAllocation(UserId, Id, item.Id, amount));
    }

    internal void Cancel(DateTimeOffset cancelledAtUtc)
    {
        if (cancelledAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
        if (IsCancelled)
        {
            return;
        }

        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }
}

/// <summary>Bir kapanışın bir kalemi ne kadar kapattığı.</summary>
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
