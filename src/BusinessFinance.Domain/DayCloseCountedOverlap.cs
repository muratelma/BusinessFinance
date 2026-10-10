namespace BusinessFinance.Domain;

/// <summary>
/// Bir gün sonunun saydığı kayıtlar arasında <b>iki kayıtta da görünen</b>
/// ama girilen nakit tutarında bir kez yer alan para.
/// </summary>
/// <remarks>
/// ADR 0019 İ2: aynı satış iki kez sayılmaz; aynı para iki kez de düşülmez.
/// Bir satış ve onun (ya da aynı kişinin) tahsilatı birlikte sayıldığında
/// ikisinin tutarı toplanır ve ortak tutar bir kez çıkarılır.
///
/// Ortak tutar kullanıcının verdiği bir bilgidir ve kayıtlardan yeniden
/// hesaplanamaz: cari tahsilat belirli bir satışa bağlı değildir. Bu yüzden
/// saklanır. Bir ödeme dağılımı <b>değildir</b>; cari bakiye, raporlar, işletme
/// neti, bütçe ve akış onu okumaz. Onu yalnız günün ekranı (ne düşüldü) ve
/// geri alma okur. Gün sonu geri alınınca silinir.
///
/// Grup, satışı ile tahsilatı aynı parayı gösterebilecek kayıtları toplar:
/// cari kayıtta kişinin, alacak faturasında faturanın kimliğidir.
/// </remarks>
public sealed class DayCloseCountedOverlap
{
    public Guid UserId { get; }
    public Guid DayCloseId { get; }
    public Guid GroupId { get; }
    public decimal Amount { get; }

    private DayCloseCountedOverlap()
    {
    }

    public DayCloseCountedOverlap(DayClose dayClose, Guid groupId, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(dayClose);
        if (dayClose.IsCancelled)
        {
            throw new InvalidOperationException("A reverted day close counts no records.");
        }

        if (groupId == Guid.Empty)
        {
            throw new ArgumentException("Group id cannot be empty.", nameof(groupId));
        }

        // Sıfır "ayrı ayrı sayıldı" demektir ve saklanacak bir şey bırakmaz.
        if (amount <= 0m || decimal.Round(amount, 4) != amount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "A shared amount is positive and has at most four decimals.");
        }

        UserId = dayClose.UserId;
        DayCloseId = dayClose.Id;
        GroupId = groupId;
        Amount = amount;
    }
}
