namespace BusinessFinance.Domain;

/// <summary>
/// Sayılan nakit ile uygulamanın beklediği bakiye arasındaki fark.
/// </summary>
/// <remarks>
/// <b>Kalıcı bir alan değildir</b> ve sayımın içinde saklanmaz: beklenen bakiye
/// hareketlerden hesaplanan bir projection'dır ve sonradan bir hareket iptal
/// edilirse değişir. Farkı sayımın yanına yazmak, doğduğu andan itibaren
/// eskiyen ikinci bir gerçek üretirdi — bakiyenin kalıcı kolon olmama
/// gerekçesinin aynısı.
///
/// Yön anlam taşır: fazla çıkan nakit <b>gelir</b>, eksik çıkan nakit
/// <b>gider</b> tarafına yazılır. İşareti kırpmak, kasadan eksileni fazla
/// gibi göstermek olurdu.
/// </remarks>
public sealed record CashCountDifference
{
    internal CashCountDifference(decimal amount, CurrencyCode currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>Sayılan eksi beklenen. Artı: fazla var. Eksi: eksik var.</summary>
    public decimal Amount { get; }

    public CurrencyCode Currency { get; }

    /// <summary>Kasada uygulamanın beklediğinden fazla nakit çıktı.</summary>
    public bool IsSurplus => Amount > 0;

    /// <summary>Kasada uygulamanın beklediğinden az nakit çıktı.</summary>
    public bool IsShortage => Amount < 0;

    /// <summary>Sayım tuttu; düzeltilecek bir şey yok.</summary>
    public bool IsBalanced => Amount == 0;

    /// <summary>
    /// Farkın düzeltme kaydı üretilirse alacağı tür. Dengede fark yoktur ve
    /// tür sorusunun cevabı da yoktur.
    /// </summary>
    public TransactionType RecognizedType => Amount switch
    {
        > 0 => TransactionType.Income,
        < 0 => TransactionType.Expense,
        _ => throw new InvalidOperationException(
            "A balanced cash count has no difference to recognize.")
    };

    /// <summary>
    /// Düzeltme kaydının tutarı: yönü <see cref="RecognizedType"/> taşır, bu
    /// yüzden tutar her zaman pozitiftir (<see cref="Money"/> sözleşmesi).
    /// </summary>
    public Money ToAdjustmentAmount() => new(decimal.Abs(Amount), Currency);
}
