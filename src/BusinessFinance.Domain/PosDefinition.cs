namespace BusinessFinance.Domain;

/// <summary>
/// Kullanıcının bir kez girdiği POS ayarı: paranın geçeceği hesap, satışın ve
/// komisyonun yazılacağı kategoriler, varsayılan oran ve geçiş süresi.
/// </summary>
/// <remarks>
/// ADR 0019 T4. Tanım <b>hiçbir para hareketi üretmez</b>: yalnız POS
/// tahsilatı formunu doldurur. Her akşam aynı beş seçimi yapmak yerine
/// kullanıcı tutarı yazar; gerisi buradan gelir.
///
/// Oran burada saklanır, tahsilatta saklanmaz: tahsilat tutarı taşır ve oran
/// ondan çözülür (ADR 0009). Tanımdaki oran sonradan değişse de geçmiş
/// tahsilatlar yazıldıkları tutarla kalır.
///
/// Yemek kartı ayrı bir özellik değildir; kendi oranı ve geçiş süresi olan bir
/// POS tanımıdır.
/// </remarks>
public sealed class PosDefinition
{
    public const int MaximumNameLength = 80;

    /// <summary>Geçiş süresinin üst sınırı; yemek kartlarının en uzun vadesini aşar.</summary>
    public const int MaximumTransferDays = 60;

    public Guid Id { get; }
    public Guid UserId { get; }

    /// <summary>Kullanıcının verdiği ad ("Ziraat POS", "Yemek kartı").</summary>
    public string Name { get; private set; }

    /// <summary>
    /// Adın karşılaştırma anahtarı (<see cref="NameKeys"/>): "bu ad zaten var
    /// mı?" denetimi ve veritabanındaki teklik bunu okur.
    /// </summary>
    public string NameKey { get; private set; }

    /// <summary>Paranın geçeceği banka hesabı.</summary>
    public Guid AccountId { get; private set; }

    /// <summary>Satış gelirinin yazılacağı gelir kategorisi.</summary>
    public Guid SalesCategoryId { get; private set; }

    /// <summary>
    /// Komisyonun yazılacağı gider kategorisi. Oran sıfırdan büyükse
    /// zorunludur; sıfırsa boş olabilir.
    /// </summary>
    public Guid? CommissionCategoryId { get; private set; }

    /// <summary>Varsayılan komisyon oranı (0,0179 = %1,79); sıfır meşrudur.</summary>
    public decimal CommissionRate { get; private set; }

    /// <summary>Satıştan kaç gün sonra paranın hesaba geçtiği.</summary>
    public int TransferDays { get; private set; }

    /// <summary>
    /// Geçiş süresi iş günüyle sayılır ve beklenen gün hafta sonuna düşmez.
    /// Tatil takvimi yoktur; gün formda her zaman elle düzeltilebilir.
    /// </summary>
    public bool BusinessDaysOnly { get; private set; }

    public bool IsActive { get; private set; }

    /// <summary>
    /// Kullanıcının ana POS'u: tahsilat formunda seçili gelir. Kullanıcı
    /// başına en çok bir tane olması uygulama katmanında korunur; pasif POS
    /// varsayılan olamaz.
    /// </summary>
    public bool IsDefault { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; }

    private PosDefinition()
    {
        Name = null!;
        NameKey = null!;
    }

    /// <summary>
    /// Bu kaydı, aynı adı taşıyan daha eski bir kayıttan ayrı tutar; yalnız
    /// yükseltme ve geri yükleme içindir (<see cref="NameKeys.Apart"/>).
    /// </summary>
    public void KeepApartFromSameName() => NameKey = NameKeys.Apart(Name, Id);

    public PosDefinition(
        Guid id,
        Guid userId,
        string name,
        Account account,
        Category salesCategory,
        decimal commissionRate,
        Category? commissionCategory,
        int transferDays,
        bool businessDaysOnly,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Pos definition id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        }

        Id = id;
        UserId = userId;
        CreatedAtUtc = createdAtUtc;
        IsActive = true;
        Name = null!;
        NameKey = null!;
        Apply(name, account, salesCategory, commissionRate, commissionCategory,
            transferDays, businessDaysOnly);
    }

    /// <summary>
    /// Tanımı düzenler. Geçmiş tahsilatlar değişmez: onlar yazıldıkları hesabı,
    /// kategoriyi ve tutarı kendileri taşır.
    /// </summary>
    public void Update(
        string name,
        Account account,
        Category salesCategory,
        decimal commissionRate,
        Category? commissionCategory,
        int transferDays,
        bool businessDaysOnly) =>
        Apply(name, account, salesCategory, commissionRate, commissionCategory,
            transferDays, businessDaysOnly);

    /// <summary>
    /// Silme yerine pasifleştirme: pasif tanım yeni tahsilatta seçilemez,
    /// geçmiş tahsilatların bağı kalır.
    /// </summary>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        if (!isActive)
        {
            IsDefault = false;
        }
    }

    /// <summary>
    /// Ana POS işaretini koyar ya da kaldırır. Pasif POS seçilemez: yeni
    /// tahsilatta zaten kullanılamaz.
    /// </summary>
    public void SetDefault(bool isDefault)
    {
        if (isDefault && !IsActive)
        {
            throw new InvalidOperationException(
                "An inactive pos definition cannot be the default.");
        }

        IsDefault = isDefault;
    }

    /// <summary>
    /// Verilen satış günü için paranın beklendiği gün.
    /// </summary>
    /// <remarks>
    /// İş günü seçeneği açıksa süre hafta içi günlerle sayılır ve sonuç hafta
    /// sonuna düşmez: cuma satışının "ertesi günü" pazartesidir. Aksi hâlde
    /// hafta sonuna denk gelen her beklenen gün sahte bir "Gecikti" üretirdi.
    /// </remarks>
    public DateOnly ExpectedTransferDate(DateOnly settlementDate)
    {
        if (!BusinessDaysOnly)
        {
            return settlementDate.AddDays(TransferDays);
        }

        var date = settlementDate;
        for (var remaining = TransferDays; remaining > 0;)
        {
            date = date.AddDays(1);
            if (IsBusinessDay(date))
            {
                remaining--;
            }
        }

        while (!IsBusinessDay(date))
        {
            date = date.AddDays(1);
        }

        return date;
    }

    private static bool IsBusinessDay(DateOnly date) =>
        date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);

    private void Apply(
        string name,
        Account account,
        Category salesCategory,
        decimal commissionRate,
        Category? commissionCategory,
        int transferDays,
        bool businessDaysOnly)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(salesCategory);

        var normalizedName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        if (normalizedName is null || normalizedName.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"Pos definition name is required and cannot exceed {MaximumNameLength} characters.",
                nameof(name));
        }

        if (account.UserId != UserId || salesCategory.UserId != UserId ||
            commissionCategory is not null && commissionCategory.UserId != UserId)
        {
            throw new ArgumentException(
                "Account and categories must belong to the pos definition user.");
        }

        if (account.Type != AccountType.Bank)
        {
            throw new InvalidOperationException(
                "Pos money arrives in a bank account; a till cannot receive a card payment.");
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive account cannot be the target of a pos definition.");
        }

        if (!salesCategory.IsActive || salesCategory.Type != CategoryType.Income)
        {
            throw new InvalidOperationException(
                "An active income category is required for pos sales.");
        }

        if (commissionRate < 0m || commissionRate >= 1m ||
            decimal.Round(commissionRate, PosSettlement.RateDecimals) != commissionRate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(commissionRate),
                commissionRate,
                $"Commission rate must be at least zero, below one and carry at most {PosSettlement.RateDecimals} decimals.");
        }

        if (commissionCategory is not null &&
            (!commissionCategory.IsActive || commissionCategory.Type != CategoryType.Expense))
        {
            throw new InvalidOperationException(
                "An active expense category is required for the pos commission.");
        }

        if (commissionRate > 0m && commissionCategory is null)
        {
            throw new InvalidOperationException(
                "A commission rate needs an expense category to be reported under.");
        }

        if (transferDays is < 0 or > MaximumTransferDays)
        {
            throw new ArgumentOutOfRangeException(
                nameof(transferDays),
                transferDays,
                $"Transfer days must be between 0 and {MaximumTransferDays}.");
        }

        // İlk kurulumda ad henüz yoktur; düzenlemede yalnız yazımı değişen ad
        // mevcut anahtarı korur.
        NameKey = Name is null
            ? NameKeys.Of(normalizedName)
            : NameKeys.AfterRename(Name, NameKey, normalizedName);
        Name = normalizedName;
        AccountId = account.Id;
        SalesCategoryId = salesCategory.Id;
        CommissionCategoryId = commissionCategory?.Id;
        CommissionRate = commissionRate;
        TransferDays = transferDays;
        BusinessDaysOnly = businessDaysOnly;
    }
}
