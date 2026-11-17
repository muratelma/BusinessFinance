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

    /// <summary>Paranın çıktığı ya da girdiği hesap; kartla ödemede boştur.</summary>
    public Guid? AccountId { get; }

    /// <summary>
    /// Kişiye kredi kartıyla yapılan ödemede kart (ADR 0022; S2): kart
    /// borcu artar, hesap kıpırdamaz, gider yazılmaz — gider alım günü
    /// yazılmıştı. Yalnız ödemede (borç yönü) olur; tam olarak biri doludur:
    /// hesap ya da kart.
    /// </summary>
    public Guid? CreditCardId { get; }
    public DebtDirection Direction { get; }
    public Money Amount { get; }
    public DateOnly PaymentDate { get; }
    public string? Description { get; }

    /// <summary>
    /// Kartla tahsilde paranın yoldaki kaydı; nakit ya da havaleyle alınan
    /// tahsilatta ve ödemede boştur.
    /// </summary>
    public Guid? PosSettlementId { get; }

    /// <summary>
    /// Satış ya da alımla birlikte yazılmış peşin ya da avans tutarında o
    /// borçlandırma; tek başına yazılan tahsilatta ve ödemede boştur.
    /// </summary>
    /// <remarks>
    /// Bağ yalnız birlikte yazıldığını söyler; ödeme yine kişinin hesabına
    /// yazılır ve en eski kalemden başlayarak düşer (ADR 0022 İ2). Satış iptal
    /// edilince ödeme kendiliğinden iptal olmaz: para gerçekten alındıysa
    /// kişinin hesabında kalır. Kullanıcı yanlış girdiyse ikisini birlikte
    /// iptal edebilir.
    /// </remarks>
    public Guid? PaidWithChargeId { get; }

    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private CounterpartyPayment()
    {
        Amount = null!;
    }

    /// <summary>
    /// Kişiye kredi kartıyla ödeme. Kart borcu tutar kadar artar; kart
    /// harcaması değildir (gider yok) ama kartın limitini kullanır. Limit
    /// denetimi kullanım durumundadır (kart harcamasıyla aynı kural).
    /// </summary>
    public static CounterpartyPayment PayByCard(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        CreditCard card,
        Money amount,
        DateOnly paymentDate,
        string? description = null,
        CounterpartyCharge? paidWithCharge = null) =>
        new(id, userId, counterparty, card, amount, paymentDate, description, paidWithCharge);

    private CounterpartyPayment(
        Guid id,
        Guid userId,
        Counterparty counterparty,
        CreditCard card,
        Money amount,
        DateOnly paymentDate,
        string? description,
        CounterpartyCharge? paidWithCharge)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Payment id cannot be empty.", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(counterparty);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(amount);
        if (userId == Guid.Empty || counterparty.UserId != userId || card.UserId != userId)
        {
            throw new ArgumentException("Counterparty and card must belong to the payment user.");
        }

        if (!card.IsActive)
        {
            throw new InvalidOperationException("An inactive card cannot pay a counterparty.");
        }

        if (amount.Currency != card.Limit.Currency)
        {
            throw new ArgumentException("Payment and card must use the same currency.", nameof(amount));
        }

        if (paymentDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(paymentDate), "Payment date is required.");
        }

        if (paidWithCharge is not null)
        {
            EnsurePaidWith(paidWithCharge, userId, counterparty, DebtDirection.Payable, amount,
                paymentDate, null);
        }

        Id = id;
        UserId = userId;
        CounterpartyId = counterparty.Id;
        CreditCardId = card.Id;
        Direction = DebtDirection.Payable;
        Amount = amount;
        PaymentDate = paymentDate;
        Description = NormalizeDescription(description);
        PaidWithChargeId = paidWithCharge?.Id;
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
        PosSettlement? cardSettlement = null,
        CounterpartyCharge? paidWithCharge = null)
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

        if (paidWithCharge is not null)
        {
            EnsurePaidWith(paidWithCharge, userId, counterparty, direction, amount, paymentDate,
                cardSettlement);
        }

        Id = id;
        UserId = userId;
        PosSettlementId = cardSettlement?.Id;
        PaidWithChargeId = paidWithCharge?.Id;
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
    public decimal SignedAccountEffect => PosSettlementId is not null || CreditCardId is not null
        ? 0m
        : Direction == DebtDirection.Receivable
            ? Amount.Amount
            : -Amount.Amount;

    /// <summary>
    /// Peşin ya da avans tutarı satışla aynı kişiye, aynı yöne ve aynı güne
    /// aittir ve satışın tutarını aşamaz. Kartla tahsil bu yolda yoktur.
    /// </summary>
    private static void EnsurePaidWith(
        CounterpartyCharge charge,
        Guid userId,
        Counterparty counterparty,
        DebtDirection direction,
        Money amount,
        DateOnly paymentDate,
        PosSettlement? cardSettlement)
    {
        if (charge.UserId != userId || charge.CounterpartyId != counterparty.Id)
        {
            throw new ArgumentException(
                "A paid-now amount must belong to the same counterparty as its charge.");
        }

        if (charge.Direction != direction || charge.ChargeDate != paymentDate)
        {
            throw new ArgumentException(
                "A paid-now amount must have the direction and the day of its charge.");
        }

        if (amount.Currency != charge.Amount.Currency || amount.Amount > charge.Amount.Amount)
        {
            throw new ArgumentException(
                "A paid-now amount cannot exceed its charge.", nameof(amount));
        }

        if (cardSettlement is not null)
        {
            throw new ArgumentException("A paid-now amount cannot be a card collection.");
        }
    }

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
