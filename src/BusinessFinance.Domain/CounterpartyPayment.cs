namespace BusinessFinance.Domain;

/// <summary>
/// Karşı taraftan tahsilat ya da karşı tarafa ödeme.
/// </summary>
/// <remarks>
/// <b>Taşır, tanımaz</b> (ADR 0014). Hesap bakiyesini değiştirir; gelir/gider
/// üretmez, çünkü ekonomik olay borçlandırmada zaten tanınmıştır. İkisini
/// birden saymak aynı satışı iki kez gelir yazmak olurdu.
///
/// Bu yüzden <b>kategori taşımaz</b>: kategori "ne alındı, ne satıldı"
/// sorusunu cevaplar ve o soru borçlandırmada sorulmuştur. Aynı gerekçeyle
/// <b>kapsam da taşımaz</b> — gelir/gider raporuna hiç girmediği için bölünecek
/// bir tarafı yok (ADR 0013). Kart ödemesinin ikisini de taşımamasıyla
/// birebir aynı yapı.
///
/// Yön, kapattığı borçlandırmanın yönüdür: <see cref="DebtDirection.Receivable"/>
/// müşteriden para almaktır (hesap artar), <see cref="DebtDirection.Payable"/>
/// tedarikçiye para vermektir (hesap azalır).
///
/// <b>Kartla tahsil</b> (ADR 0019 T5): müşteri borcunu POS'tan kartla öderse
/// tahsilat bir <see cref="PosSettlement"/> (<see cref="PosSettlementKind.Collection"/>)
/// taşır. Cari o gün brüt tutarla kapanır; hesap ise o gün kıpırdamaz — para
/// yoldadır ve hesaba satıştaki gibi yatışla, net olarak geçer. Tahsilat
/// hesaba ikinci kez yazsaydı aynı para iki kez sayılırdı.
/// </remarks>
public sealed class CounterpartyPayment
{
    public const int MaximumDescriptionLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid CounterpartyId { get; }
    public Guid AccountId { get; }
    public DebtDirection Direction { get; }
    public Money Amount { get; }
    public DateOnly PaymentDate { get; }
    public string? Description { get; }

    /// <summary>
    /// Kartla tahsilde paranın yoldaki kaydı; nakit ya da havaleyle alınan
    /// tahsilatta ve ödemede boştur.
    /// </summary>
    public Guid? PosSettlementId { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CounterpartyPayment()
    {
        Amount = null!;
    }

    public CounterpartyPayment(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        Account account,
        DebtDirection direction,
        Money amount,
        DateOnly paymentDate,
        string? description = null,
        PosSettlement? cardSettlement = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Payment id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(counterparty);
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(amount);
        if (counterparty.UserId != userId || account.UserId != userId)
        {
            throw new ArgumentException(
                "Counterparty and account must belong to the payment user.");
        }

        // Pasif karşı taraf **kasıtlı olarak** engellenmiyor: artık iş
        // yapılmayan bir müşteri kalan borcunu ödeyebilmeli. Engellemek, açık
        // bakiyeyi kapatılamaz hâle getirirdi. Hesabın pasif olması ise gerçek
        // bir engel — kapalı hesaptan para çıkmaz, kapalı hesaba para girmez.
        if (!account.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive account cannot settle a counterparty balance.");
        }

        if (!Enum.IsDefined(direction))
        {
            throw new ArgumentOutOfRangeException(
                nameof(direction),
                direction,
                "Payment direction is not supported.");
        }

        if (amount.Currency != account.Currency)
        {
            throw new ArgumentException(
                "Payment and account must use the same currency.",
                nameof(amount));
        }

        if (paymentDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(paymentDate), "Payment date is required.");
        }

        if (cardSettlement is not null)
        {
            cardSettlement.EnsureCollects(userId, account, direction, amount, paymentDate);
        }

        Id = id;
        UserId = userId;
        PosSettlementId = cardSettlement?.Id;
        CounterpartyId = counterparty.Id;
        AccountId = account.Id;
        Direction = direction;
        Amount = amount;
        PaymentDate = paymentDate;
        Description = NormalizeDescription(description);
    }

    /// <summary>
    /// Hesap bakiyesine etkisi: tahsilat artırır, ödeme azaltır.
    /// </summary>
    /// <remarks>
    /// İşaret tek yerde duruyor ki bakiye ve feed aynı cevabı versin; iki
    /// yerde yazılsaydı biri unutulduğunda para bir tarafta kaybolurdu.
    ///
    /// Kartla tahsilde sıfırdır: para hesaba yatışla girer.
    /// </remarks>
    public decimal SignedAccountEffect => PosSettlementId is not null
        ? 0m
        : Direction == DebtDirection.Receivable
            ? Amount.Amount
            : -Amount.Amount;

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
    }

    private static string? NormalizeDescription(string? description)
    {
        var normalized = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalized?.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException(
                $"Payment description cannot exceed {MaximumDescriptionLength} characters.",
                nameof(description));
        }

        return normalized;
    }
}
