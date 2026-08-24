namespace BusinessFinance.Domain;

/// <summary>
/// Tahsil edilmiş ama henüz hesaba geçmemiş paranın toplamı — kullanıcının
/// dilinde <b>yolda</b> olan para.
/// </summary>
/// <remarks>
/// <b>Kalıcı kolon değildir ve bir hesap türü de değildir</b> (ADR 0015).
/// Geçişi gerçekleşmemiş tahsilatların net toplamından her sorguda hesaplanır.
/// Hesap yapılsaydı kullanıcı oradan transfer edebilir, kart borcu ödeyebilir
/// ve onu kasa sayımına katabilirdi; üçü de olmamış parayı harcamaktır.
///
/// Toplanan <b>net</b> tutardır: hesaba geçecek olan odur. Brüt toplamak,
/// bankanın kestiği komisyonu kullanıcının cebinde sayardı.
/// </remarks>
public sealed record PosTransitBalance
{
    private PosTransitBalance(decimal amount, int count)
    {
        Amount = amount;
        Count = count;
    }

    /// <summary>Yoldaki net toplam.</summary>
    public decimal Amount { get; }

    /// <summary>Yolda bekleyen tahsilat adedi.</summary>
    public int Count { get; }

    public bool HasMoneyInTransit => Count > 0;

    /// <summary>
    /// İptal edilmiş ve geçmiş tahsilatlar sayılmaz: ilki hiç olmadı, ikincisi
    /// artık hesabın kendi bakiyesinde duruyor. İkisini de saymak aynı parayı
    /// iki yerde göstermek olurdu.
    /// </summary>
    public static PosTransitBalance Calculate(IEnumerable<PosSettlement> settlements)
    {
        ArgumentNullException.ThrowIfNull(settlements);

        var amount = 0m;
        var count = 0;
        foreach (var settlement in settlements)
        {
            if (!settlement.IsInTransit)
            {
                continue;
            }

            amount += settlement.NetAmount.Amount;
            count++;
        }

        return new PosTransitBalance(amount, count);
    }
}
