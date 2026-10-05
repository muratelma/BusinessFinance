namespace BusinessFinance.Domain;

/// <summary>
/// Müşterinin kartla ödediği tahsilat: satış bugün olur, para birkaç gün sonra
/// komisyon düşülmüş olarak hesaba geçer.
/// </summary>
/// <remarks>
/// ADR 0015'in kaydı. Bu <b>kredi kartı değildir</b>: `CreditCard` borçlandığın
/// karttır, bu ise tahsilat aracıdır. İkisi aynı kelimeyle anıldığında
/// tahsilat borç olarak kaydediliyordu.
///
/// ADR 0014'ün ayrımı burada tek kaydın iki anına düşer:
/// <list type="bullet">
/// <item>Tahsilat günü <b>tanır</b>: gelir brüt tutar kadar, komisyon ayrı
/// gider. Hesap bakiyesi kıpırdamaz — para henüz bankada değil.</item>
/// <item>Geçiş günü <b>taşır</b>: hesap net tutar kadar artar, gelir/gider
/// sıfır. Aynı satışın ikinci kez sayılması böyle önlenir.</item>
/// </list>
/// Geçişi <see cref="PosDeposit"/> yazar: bankaya yatan para bir ya da birkaç
/// tahsilatı birlikte kapatır.
/// </remarks>
public sealed class PosSettlement
{
    public const int MaximumDescriptionLength = 500;

    /// <summary>Komisyon oranının ondalık basamak sayısı (ADR 0009 ile aynı).</summary>
    public const int RateDecimals = 4;

    private const int MoneyDecimals = 4;

    public Guid Id { get; }
    public Guid UserId { get; }

    /// <summary>Paranın geçeceği banka hesabı.</summary>
    public Guid AccountId { get; }

    /// <summary>Satış gelirinin yazılacağı gelir kategorisi.</summary>
    public Guid CategoryId { get; }

    /// <summary>
    /// Komisyonun yazılacağı gider kategorisi; komisyon sıfırsa boştur.
    /// </summary>
    public Guid? CommissionCategoryId { get; }

    /// <summary>
    /// Tahsilatın yazıldığı POS tanımı (ADR 0019 T4). Tanımlar gelmeden önce
    /// yazılmış ve tanımsız girilen tahsilatlarda boştur: hangi POS'tan geldiği
    /// bilinmiyor ve uydurulmaz.
    /// </summary>
    public Guid? PosDefinitionId { get; }

    /// <summary>
    /// Tahsilatı üreten gün sonu (ADR 0019 T1); tek tek girilen tahsilatta
    /// boştur. Gün sonundan gelen tahsilat tek başına değil, gün sonuyla
    /// birlikte geri alınır.
    /// </summary>
    public Guid? DayCloseId { get; }

    /// <summary>Müşterinin ödediği tutar. Gelir <b>bu</b> tutar kadar tanınır.</summary>
    public Money GrossAmount { get; }

    /// <summary>
    /// Bankanın kestiği komisyon. <see cref="Money"/> değildir çünkü sıfır
    /// meşrudur: her kartta komisyon kesilmez.
    /// </summary>
    public decimal CommissionAmount { get; }

    public CurrencyCode Currency => GrossAmount.Currency;

    public TransactionScope Scope { get; }
    public DateOnly SettlementDate { get; }

    /// <summary>Paranın hesaba geçmesi beklenen gün.</summary>
    public DateOnly ExpectedTransferDate { get; }

    public string? Description { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    /// <summary>
    /// Parayı hesaba geçiren yatış (ADR 0019 T5); para yoldaysa boştur.
    /// </summary>
    /// <remarks>
    /// Tahsilat hesaba <b>yalnız bir yatışla</b> geçer: bu alanı ve aşağıdaki
    /// iki alanı <see cref="PosDeposit"/> yazar ve geri alır. Üçü birlikte
    /// dolar ya da birlikte boş kalır.
    /// </remarks>
    public Guid? PosDepositId { get; private set; }

    /// <summary>
    /// Paranın hesaba geçtiği gün; yatışın günüyle aynıdır, geçmediyse boştur.
    /// </summary>
    /// <remarks>
    /// Yatışın gününün kopyasıdır ve ondan ayrışamaz: SQL'de tahsilat yatışa
    /// <c>(UserId, PosDepositId, TransferredOn)</c> üçlüsüyle bağlanır. Kopya,
    /// tarihe göre okuyan bütün sorguların (bakiye, net varlık, günlük akış)
    /// yatış tablosuna birleşmeden çalışmasını sağlar.
    /// </remarks>
    public DateOnly? TransferredOn { get; private set; }
    public DateTimeOffset? TransferredAtUtc { get; private set; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private PosSettlement()
    {
        GrossAmount = null!;
    }

    public PosSettlement(
        Guid id,
        Guid userId,
        Account account,
        Category category,
        Money grossAmount,
        decimal commissionAmount,
        TransactionScope scope,
        DateOnly settlementDate,
        DateOnly expectedTransferDate,
        DateTimeOffset createdAtUtc,
        Category? commissionCategory = null,
        string? description = null,
        PosDefinition? definition = null,
        Guid? dayCloseId = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Pos settlement id cannot be empty.", nameof(id));
        }

        if (dayCloseId == Guid.Empty)
        {
            throw new ArgumentException("Day close id cannot be empty.", nameof(dayCloseId));
        }

        if (definition is not null && definition.UserId != userId)
        {
            throw new ArgumentException(
                "Pos definition must belong to the pos settlement user.", nameof(definition));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(grossAmount);
        if (account.UserId != userId || category.UserId != userId ||
            commissionCategory is not null && commissionCategory.UserId != userId)
        {
            throw new ArgumentException(
                "Account and categories must belong to the pos settlement user.");
        }

        if (account.Type != AccountType.Bank)
        {
            throw new InvalidOperationException(
                "Pos money arrives in a bank account; a till cannot receive a card payment.");
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive account cannot receive a pos settlement.");
        }

        if (grossAmount.Currency != account.Currency)
        {
            throw new ArgumentException(
                "Pos settlement and account must use the same currency.",
                nameof(grossAmount));
        }

        if (!category.IsActive || category.Type != CategoryType.Income)
        {
            throw new InvalidOperationException(
                "An active income category is required: a pos settlement recognizes a sale.");
        }

        ValidateCommission(commissionAmount, grossAmount, commissionCategory);
        TransactionScopeGuard.Validate(scope, nameof(scope));
        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        }

        if (settlementDate == default ||
            settlementDate > DateOnly.FromDateTime(createdAtUtc.UtcDateTime))
        {
            throw new ArgumentOutOfRangeException(
                nameof(settlementDate),
                "Settlement date is required and cannot be in the future.");
        }

        // Beklenen geçiş günü gelecektedir ve olması gereken de budur; tahsilat
        // gününden önce olamaz çünkü para satıştan önce hesaba geçemez.
        if (expectedTransferDate == default || expectedTransferDate < settlementDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedTransferDate),
                "Expected transfer date cannot be before the settlement date.");
        }

        Id = id;
        UserId = userId;
        AccountId = account.Id;
        CategoryId = category.Id;
        CommissionCategoryId = commissionCategory?.Id;
        PosDefinitionId = definition?.Id;
        DayCloseId = dayCloseId;
        GrossAmount = grossAmount;
        CommissionAmount = commissionAmount;
        Scope = scope;
        SettlementDate = settlementDate;
        ExpectedTransferDate = expectedTransferDate;
        CreatedAtUtc = createdAtUtc;
        Description = NormalizeDescription(description);
    }

    /// <summary>
    /// Hesaba geçen tutar: brüt eksi komisyon.
    /// </summary>
    /// <remarks>
    /// Komisyon brüt tutardan <b>ayrı okunur ve ona eklenmez</b>. Net tutarı
    /// gelir olarak yazmak, kullanıcının gerçekten kestiği faturayı küçültür ve
    /// komisyonu görünmez bir gidere çevirirdi — dekonttaki işlem ücretinde
    /// öğrenilen dersin aynısı.
    /// </remarks>
    public Money NetAmount => new(GrossAmount.Amount - CommissionAmount, Currency);

    /// <summary>
    /// Komisyon oranı <b>saklanmaz, paradan çözülür</b> (ADR 0009'un aynı
    /// kararı): saklansaydı tutarla oran ayrı ayrı düzenlenebilir ve ikisi
    /// sessizce çelişirdi.
    /// </summary>
    public decimal CommissionRate => CommissionAmount == 0m
        ? 0m
        : decimal.Round(
            CommissionAmount / GrossAmount.Amount, RateDecimals, MidpointRounding.AwayFromZero);

    public bool IsTransferred => TransferredOn is not null;

    /// <summary>Yolda: tahsil edildi ama henüz hesaba geçmedi.</summary>
    public bool IsInTransit => !IsCancelled && !IsTransferred;

    /// <summary>
    /// Hesap bakiyesine etkisi. Geçiş gerçekleşene kadar <b>sıfırdır</b>:
    /// para henüz bankada değil.
    /// </summary>
    public decimal SignedAccountEffect =>
        IsCancelled || !IsTransferred ? 0m : NetAmount.Amount;

    /// <summary>
    /// Verilen oranla brüt tutardan komisyonu çözer.
    /// </summary>
    /// <remarks>
    /// Kullanıcı oranı da tutarı da girebilir; ikisini birden saklamak yerine
    /// oran burada tutara çevrilir ve <b>tek gerçek tutardır</b>. Yuvarlama
    /// para tarafında yapılır, çünkü hesaba geçecek olan paradır.
    /// </remarks>
    public static decimal CommissionFromRate(Money grossAmount, decimal rate)
    {
        ArgumentNullException.ThrowIfNull(grossAmount);
        if (rate < 0m || rate >= 1m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                rate,
                "Commission rate must be at least zero and below one.");
        }

        if (decimal.Round(rate, RateDecimals) != rate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                rate,
                $"Commission rate cannot carry more than {RateDecimals} decimals.");
        }

        return decimal.Round(
            grossAmount.Amount * rate, MoneyDecimals, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Tahsilatı bir yatışa bağlar: para o gün hesaba geçmiştir.
    /// </summary>
    /// <remarks>
    /// Bu an <b>hiçbir gelir veya gider yazmaz</b>: satış tahsilat gününde
    /// zaten tanındı. Yazsaydı aynı satış iki kez sayılırdı.
    ///
    /// Yalnız <see cref="PosDeposit"/> çağırır; tahsilatı yatışsız "hesaba
    /// geçti" yapan bir yol yoktur (ADR 0019 T5: iki yol yan yana yaşamaz).
    /// </remarks>
    internal void AttachToDeposit(Guid depositId, DateOnly depositDate, DateTimeOffset attachedAtUtc)
    {
        if (!IsInTransit)
        {
            throw new InvalidOperationException(
                "Only a pos settlement that is still in transit can be deposited.");
        }

        if (depositDate < SettlementDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(depositDate),
                "Deposit date cannot be before the settlement date.");
        }

        PosDepositId = depositId;
        TransferredOn = depositDate;
        TransferredAtUtc = attachedAtUtc;
    }

    /// <summary>
    /// Yatış geri alındığında tahsilatı yeniden yola döndürür.
    /// </summary>
    /// <remarks>
    /// Yalnız hesaba yazılmış net tutar geri çekilir. Gelir ve komisyon
    /// tahsilat gününde tanındı ve olduğu gibi kalır — satış olmuştur, yalnız
    /// para henüz gelmemiştir (ADR 0014).
    /// </remarks>
    internal void DetachFromDeposit(Guid depositId)
    {
        if (PosDepositId != depositId)
        {
            throw new InvalidOperationException(
                "This pos settlement does not belong to the deposit being reverted.");
        }

        PosDepositId = null;
        TransferredOn = null;
        TransferredAtUtc = null;
    }

    /// <summary>
    /// Silme yerine iptal. İptal edilen tahsilat tanıdığı geliri ve komisyonu
    /// kaybeder; yoldaki para da onunla birlikte düşer.
    /// </summary>
    /// <remarks>
    /// Yatışa bağlı tahsilat iptal edilemez: yatış o tahsilatın netini hesaba
    /// taşımıştır ve iptal, yatışın tutarını açıklanamaz bırakırdı. Önce yatış
    /// geri alınır (ADR 0019 T5).
    /// </remarks>
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

        if (PosDepositId is not null)
        {
            throw new InvalidOperationException(
                "A deposited pos settlement cannot be cancelled; revert the deposit first.");
        }

        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }

    private static void ValidateCommission(
        decimal commissionAmount,
        Money grossAmount,
        Category? commissionCategory)
    {
        if (commissionAmount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(commissionAmount),
                "Commission cannot be negative.");
        }

        if (decimal.Round(commissionAmount, MoneyDecimals) != commissionAmount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(commissionAmount),
                "Commission cannot carry more than four decimals.");
        }

        if (commissionAmount >= grossAmount.Amount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(commissionAmount),
                "Commission cannot consume the whole settlement: nothing would reach the account.");
        }

        // Komisyon varsa gideri bir kategoriye yazılır; yoksa sorulacak bir şey
        // yoktur. İkisi birlikte bulunur ya da birlikte bulunmaz.
        if (commissionAmount > 0m)
        {
            if (commissionCategory is null)
            {
                throw new InvalidOperationException(
                    "A commission needs an expense category to be reported under.");
            }

            if (!commissionCategory.IsActive || commissionCategory.Type != CategoryType.Expense)
            {
                throw new InvalidOperationException(
                    "An active expense category is required for the pos commission.");
            }
        }
        else if (commissionCategory is not null)
        {
            throw new InvalidOperationException(
                "A settlement with no commission cannot carry a commission category.");
        }
    }

    private static string? NormalizeDescription(string? description)
    {
        var normalized = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalized?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Pos settlement description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        return normalized;
    }
}
