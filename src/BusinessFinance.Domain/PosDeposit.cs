namespace BusinessFinance.Domain;

/// <summary>
/// Bankanın POS parasını hesaba yatırdığı an: bir ya da birkaç tahsilat tek
/// para hareketiyle kapanır.
/// </summary>
/// <remarks>
/// ADR 0019 T5. Yatış <b>taşır</b> (ADR 0014): hesap bakiyesini değiştirir,
/// gelir yazmaz — satış tahsilat gününde zaten tanındı.
///
/// Banka beklenenden az yatırdıysa fark <b>kesintidir</b> ve ayrı bir gider
/// kaydıyla (<see cref="DeductionTransactionId"/>) yazılır; yatış o kaydı
/// kimliğiyle taşır. Hesaba kapatılan tahsilatların neti girer, kesinti gideri
/// hesaptan çıkar; ikisinin farkı tam olarak <see cref="DepositedAmount"/>
/// kadardır, yani bankanın gerçekten yatırdığı tutar.
///
/// Beklenenden <b>fazla</b> yatan tutar reddedilir: fazlası gelir değil, fazla
/// yazılmış komisyondur. Gelir yazmak satışı şişirirdi; kullanıcı ilgili
/// tahsilatı doğru komisyonla yeniden girer.
/// </remarks>
public sealed class PosDeposit
{
    private const int MoneyDecimals = 4;

    public Guid Id { get; }
    public Guid UserId { get; }

    /// <summary>Paranın yattığı banka hesabı; kapatılan tahsilatların hesabıdır.</summary>
    public Guid AccountId { get; }

    /// <summary>Bankanın gerçekten yatırdığı tutar; kullanıcının girdiği sayıdır.</summary>
    public Money DepositedAmount { get; }

    /// <summary>
    /// Beklenen ile yatan arasındaki fark. Sıfır meşrudur: çoğu yatış
    /// beklendiği kadar gelir.
    /// </summary>
    public decimal DeductionAmount { get; }

    /// <summary>
    /// Kesintiyi yazan gider kaydı; kesinti sıfırsa boştur. İkisi birlikte
    /// bulunur ya da birlikte bulunmaz.
    /// </summary>
    public Guid? DeductionTransactionId { get; }

    public CurrencyCode Currency => DepositedAmount.Currency;

    public DateOnly DepositDate { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    /// <summary>
    /// Kapatılan tahsilatların net toplamı: yatan tutar artı kesinti.
    /// </summary>
    public decimal ExpectedAmount => DepositedAmount.Amount + DeductionAmount;

    private PosDeposit()
    {
        DepositedAmount = null!;
    }

    private PosDeposit(
        Guid id,
        Guid userId,
        Account account,
        Money depositedAmount,
        decimal deductionAmount,
        BudgetTransaction? deductionTransaction,
        DateOnly depositDate,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Pos deposit id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(depositedAmount);
        if (account.UserId != userId)
        {
            throw new ArgumentException("Account must belong to the pos deposit user.", nameof(account));
        }

        if (account.Type != AccountType.Bank)
        {
            throw new InvalidOperationException(
                "Pos money arrives in a bank account; a till cannot receive a deposit.");
        }

        if (depositedAmount.Currency != account.Currency)
        {
            throw new ArgumentException(
                "Pos deposit and account must use the same currency.", nameof(depositedAmount));
        }

        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        }

        if (depositDate == default ||
            depositDate > DateOnly.FromDateTime(createdAtUtc.UtcDateTime))
        {
            throw new ArgumentOutOfRangeException(
                nameof(depositDate),
                "Deposit date is required and cannot be in the future.");
        }

        if (deductionAmount < 0m ||
            decimal.Round(deductionAmount, MoneyDecimals) != deductionAmount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deductionAmount),
                "Deduction cannot be negative or carry more than four decimals.");
        }

        ValidateDeduction(
            userId, account, deductionAmount, deductionTransaction, depositDate, depositedAmount.Currency);

        Id = id;
        UserId = userId;
        AccountId = account.Id;
        DepositedAmount = depositedAmount;
        DeductionAmount = deductionAmount;
        DeductionTransactionId = deductionTransaction?.Id;
        DepositDate = depositDate;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Seçilen tahsilatlar için kesintiyi hesaplar: beklenen net toplam eksi
    /// yatan tutar.
    /// </summary>
    /// <remarks>
    /// Kesinti gideri yatıştan <b>önce</b> kurulur (yatış onu kimliğiyle
    /// taşır), bu yüzden tutarı ayrıca sorulabilir olmalıdır. Kural tek
    /// yerdedir: önizleme de kayıt da bunu çağırır.
    /// </remarks>
    public static decimal DeductionFor(
        IReadOnlyCollection<PosSettlement> settlements,
        Money depositedAmount)
    {
        ArgumentNullException.ThrowIfNull(depositedAmount);
        var expected = ExpectedFor(settlements);
        if (depositedAmount.Amount > expected)
        {
            throw new ArgumentOutOfRangeException(
                nameof(depositedAmount),
                "The deposited amount cannot exceed the expected net amount.");
        }

        return expected - depositedAmount.Amount;
    }

    /// <summary>Seçilen tahsilatların net toplamı; hesaba geçmesi beklenen tutar.</summary>
    public static decimal ExpectedFor(IReadOnlyCollection<PosSettlement> settlements)
    {
        ArgumentNullException.ThrowIfNull(settlements);
        if (settlements.Count == 0)
        {
            throw new ArgumentException(
                "A pos deposit closes at least one settlement.", nameof(settlements));
        }

        return settlements.Sum(settlement => settlement.NetAmount.Amount);
    }

    /// <summary>
    /// Yatışı yazar ve seçilen tahsilatları ona bağlar.
    /// </summary>
    /// <remarks>
    /// Tahsilatların hepsi aynı hesaba geçmeli ve hâlâ yolda olmalıdır: bir
    /// yatış tek bir hesaba düşen tek bir para hareketidir. Yatış günü hiçbir
    /// tahsilatın gününden önce olamaz — para satıştan önce yatamaz.
    /// </remarks>
    public static PosDeposit Record(
        Guid id,
        Guid userId,
        Account account,
        IReadOnlyCollection<PosSettlement> settlements,
        Money depositedAmount,
        DateOnly depositDate,
        DateTimeOffset createdAtUtc,
        BudgetTransaction? deductionTransaction = null)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(settlements);
        var deduction = DeductionFor(settlements, depositedAmount);

        if (settlements.Select(settlement => settlement.Id).Distinct().Count() != settlements.Count)
        {
            throw new ArgumentException(
                "A settlement cannot be deposited twice.", nameof(settlements));
        }

        foreach (var settlement in settlements)
        {
            if (settlement.UserId != userId)
            {
                throw new ArgumentException(
                    "Settlements must belong to the pos deposit user.", nameof(settlements));
            }

            if (settlement.AccountId != account.Id)
            {
                throw new InvalidOperationException(
                    "A deposit closes settlements of a single account.");
            }

            if (!settlement.IsInTransit)
            {
                throw new InvalidOperationException(
                    "Only a pos settlement that is still in transit can be deposited.");
            }

            if (depositDate < settlement.SettlementDate)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(depositDate),
                    "Deposit date cannot be before a settlement it closes.");
            }
        }

        if (deductionTransaction is { IsCancelled: true })
        {
            throw new InvalidOperationException(
                "A cancelled expense cannot carry a deposit's deduction.");
        }

        var deposit = new PosDeposit(
            id, userId, account, depositedAmount, deduction, deductionTransaction,
            depositDate, createdAtUtc);
        foreach (var settlement in settlements)
        {
            settlement.AttachToDeposit(id, depositDate, createdAtUtc);
        }

        return deposit;
    }

    /// <summary>
    /// Geri alınmış bir yatışı yedekten kurar.
    /// </summary>
    /// <remarks>
    /// Geri alınan yatış tahsilatlarını bırakmıştır; yeniden kurulurken
    /// bağlanacak tahsilatı yoktur ve <see cref="Record"/> yolundan geçemez.
    /// </remarks>
    public static PosDeposit RestoreCancelled(
        Guid id,
        Guid userId,
        Account account,
        Money depositedAmount,
        decimal deductionAmount,
        BudgetTransaction? deductionTransaction,
        DateOnly depositDate,
        DateTimeOffset createdAtUtc,
        DateTimeOffset cancelledAtUtc)
    {
        if (cancelledAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
        }

        if (deductionTransaction is { IsCancelled: false })
        {
            throw new InvalidOperationException(
                "A reverted deposit cannot keep a live deduction expense.");
        }

        return new PosDeposit(
            id, userId, account, depositedAmount, deductionAmount, deductionTransaction,
            depositDate, createdAtUtc)
        {
            IsCancelled = true,
            CancelledAtUtc = cancelledAtUtc,
        };
    }

    /// <summary>
    /// Yatışı geri alır: kapattığı tahsilatlar yeniden yola çıkar, kesinti
    /// gideri iptal olur.
    /// </summary>
    /// <remarks>
    /// Silme yerine iptal: yatış kaydı kalır. Üç değişiklik birlikte yapılır;
    /// kesinti gideri yatışsız kalsaydı olmamış bir kesinti gider yazılı
    /// dururdu, tahsilatlar bağlı kalsaydı gelmemiş para hesapta sayılırdı.
    ///
    /// Çağrı idempotenttir: geri alınmış yatışta hiçbir şey yapmaz.
    /// </remarks>
    public void Revert(
        IReadOnlyCollection<PosSettlement> settlements,
        BudgetTransaction? deductionTransaction,
        DateTimeOffset cancelledAtUtc)
    {
        ArgumentNullException.ThrowIfNull(settlements);
        if (cancelledAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
        }

        if (IsCancelled)
        {
            return;
        }

        // Yatışın bütün tahsilatları birlikte bırakılır; eksik bir liste
        // geride hesaba geçmiş görünen ama yatışı iptal bir tahsilat bırakırdı.
        if (settlements.Any(settlement => settlement.PosDepositId != Id) ||
            settlements.Sum(settlement => settlement.NetAmount.Amount) != ExpectedAmount)
        {
            throw new InvalidOperationException(
                "A deposit is reverted together with every settlement it closed.");
        }

        if (DeductionTransactionId != deductionTransaction?.Id)
        {
            throw new InvalidOperationException(
                "A deposit is reverted together with its deduction expense.");
        }

        foreach (var settlement in settlements)
        {
            settlement.DetachFromDeposit(Id);
        }

        deductionTransaction?.Cancel(cancelledAtUtc);
        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }

    private static void ValidateDeduction(
        Guid userId,
        Account account,
        decimal deductionAmount,
        BudgetTransaction? deductionTransaction,
        DateOnly depositDate,
        CurrencyCode currency)
    {
        if (deductionAmount == 0m)
        {
            if (deductionTransaction is not null)
            {
                throw new InvalidOperationException(
                    "A deposit with no deduction cannot carry a deduction expense.");
            }

            return;
        }

        if (deductionTransaction is null)
        {
            throw new InvalidOperationException(
                "A deduction needs an expense record to be reported under.");
        }

        // Kesinti gideri yatışın öbür yüzüdür: aynı hesaptan, aynı gün, tam
        // fark kadar. Biri bile tutmasa hesaba giren ile yatan ayrışırdı.
        if (deductionTransaction.UserId != userId ||
            deductionTransaction.AccountId != account.Id ||
            deductionTransaction.Type != TransactionType.Expense ||
            deductionTransaction.TransactionDate != depositDate ||
            deductionTransaction.Amount.Amount != deductionAmount ||
            deductionTransaction.Amount.Currency != currency)
        {
            throw new InvalidOperationException(
                "The deduction expense must match the deposit's account, day and difference.");
        }
    }
}
