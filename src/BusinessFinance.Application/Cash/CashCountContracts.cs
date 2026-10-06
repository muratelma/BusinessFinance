using BusinessFinance.Domain;

namespace BusinessFinance.Application.Cash;

public sealed record CreateCashCountCommand(
    Guid AccountId,
    decimal CountedAmount,
    DateOnly CountDate,
    TransactionScope? Scope,
    string? Note);

/// <summary>
/// Farkın gelir/gider olarak yazılmasını onaylayan ikinci eylem.
/// </summary>
/// <remarks>
/// Kategori istemciden gelir çünkü fark bir kategoriye <b>ait değildir</b>:
/// kasadan eksilen para çalınmış da olabilir, yanlış para üstü de. Sunucunun
/// bir kategori uydurması, kullanıcının raporunu tahminle doldurmak olurdu.
///
/// Tek istisna kullanıcının <b>kendi</b> söylediği "bilmiyorum"dur
/// (<c>UnknownReason</c>, Aşama 06.3 K10): eksik fark standart
/// <see cref="CashCountDefaults.DifferenceCategoryName"/> gider kategorisine
/// yazılır. Bu bir tahmin değildir; kullanıcı o kovayı seçmiştir. Kategori
/// ile "bilmiyorum" birlikte gönderilemez.
/// </remarks>
public sealed record ConfirmCashCountDifferenceCommand(
    Guid CashCountId,
    Guid? CategoryId,
    bool UnknownReason = false);

/// <summary>Kasa sayımının standart adları.</summary>
public static class CashCountDefaults
{
    /// <summary>
    /// Sebebi bilinmeyen kasa eksiğinin yazıldığı gider kategorisi. Varsayılan
    /// kategori setlerinde gelir; seti bundan önce kurulmuş ya da kategoriyi
    /// yeniden adlandırmış kullanıcıda ilk kullanımda açılır.
    /// </summary>
    public const string DifferenceCategoryName = "Kasa farkı";
}

public sealed record CashCountListCriteria(Guid? AccountId, DateOnly From, DateOnly To);

/// <summary>
/// Bir sayımın okunabilir hâli.
/// </summary>
/// <remarks>
/// Günün açık sayımında <see cref="ExpectedBalance"/> ve
/// <see cref="Difference"/> güncel bakiyeden hesaplanır. Geçmiş sayımda ikisi
/// sayımın yanında saklanan <b>o anki gözlemden</b> gelir
/// (<c>CashCount.ExpectedAtCount</c>); gözlem yoksa (alan eklenmeden önceki
/// sayım) boştur. Geçmiş bir sayımın farkını bugünkü bakiyeye
/// karşı yeniden hesaplamak, aradaki bütün hareketleri o günün farkına
/// yazmak olurdu — sayı doğru görünür, anlamı yanlış olurdu. Geçmiş sayımda
/// kalan tek gerçek, sayılan tutar ve varsa yazılmış düzeltme kaydıdır.
/// </remarks>
public sealed record CashCountDto(
    Guid Id,
    Guid AccountId,
    string AccountName,
    DateOnly CountDate,
    decimal CountedAmount,
    CurrencyCode Currency,
    TransactionScope Scope,
    string? Note,
    bool IsCancelled,
    Guid? AdjustmentTransactionId,
    decimal? ExpectedBalance = null,
    decimal? Difference = null);

/// <summary>
/// Kasa ekranının açılışta sorduğu tek soru: bugün ne olmalıydı, ne sayıldı.
/// </summary>
public sealed record CashCountTodayDto(
    Guid AccountId,
    string AccountName,
    decimal ExpectedBalance,
    CurrencyCode Currency,
    CashCountDto? Count,

    /// <summary>Bugünden önceki son (iptal edilmemiş) sayım; yoksa boş.</summary>
    CashCountDto? PreviousCount = null,

    /// <summary>Bugün kasaya giren nakit: gelir, gelen transfer, tahsilat.</summary>
    decimal TodayInflow = 0m,

    /// <summary>Bugün kasadan çıkan nakit: gider, giden transfer, ödeme.</summary>
    decimal TodayOutflow = 0m,

    /// <summary>
    /// Bugünkü sayımdan bu yana kasa bakiyesindeki değişim; sayım yoksa ya da
    /// sayım anının gözlemi bilinmiyorsa boştur.
    /// </summary>
    /// <remarks>
    /// Farkı kaydedilmiş sayımda bakiye sayılan tutara oturmuştu, değişim
    /// güncel bakiye − sayılan tutardır; kaydedilmemiş sayımda güncel bakiye −
    /// sayım anındaki beklenen tutardır. Sıfırdan farklıysa sayımdan sonra
    /// kasaya hareket girmiştir ve ekran "oturdu" diyemez (28 Eylül denetimi
    /// U10). Sunucu hesaplar; istemci çıkarma yapmaz.
    /// </remarks>
    decimal? ChangeSinceCount = null,

    /// <summary>
    /// Bugünden önceki son sayımın <b>kaydedilmemiş</b> farkı (sayılan − o an
    /// beklenen); sayım yoksa, farkı kaydedildiyse, tuttuysa ya da sayım
    /// anının gözlemi bilinmiyorsa boştur.
    /// </summary>
    /// <remarks>
    /// Yalnız bilgidir: bugünkü fark ondan düşülmez ve ikiye bölünmez (Aşama
    /// 06.3 K6). Arada eski farkı açıklayan bir kayıt girilmiş olabilir;
    /// "yeni kısım" diye bir sayı türetmek o durumda yanlış sayı gösterirdi.
    /// </remarks>
    decimal? PreviousUnrecordedDifference = null,

    /// <summary>
    /// Bugünkü sayımın kaydedilmemiş farkı önceki sayımın kaydedilmemiş
    /// farkına eşit: aynı eksik ya da fazla bir kez daha sayıldı.
    /// </summary>
    bool DifferenceSameAsPrevious = false);

public interface ICashCountRepository
{
    /// <summary>
    /// O gün ve o kasa için iptal edilmemiş sayım. Takip edilerek okunur:
    /// yerine yenisi geldiğinde iptal edilecek olan bu kayıttır.
    /// </summary>
    Task<CashCount?> FindOpenAsync(
        Guid userId,
        Guid accountId,
        DateOnly countDate,
        bool track,
        CancellationToken cancellationToken);

    /// <summary>
    /// Yeni sayımı yazar; yerini aldığı sayım varsa iptali <b>aynı</b>
    /// SaveChanges sınırında yazılır. Ayrı yazılsaydı araya düşen bir hata iki
    /// açık sayım bırakırdı ve SQL'deki tekil indeks bunu zaten reddeder.
    /// </summary>
    Task AddAsync(CashCount cashCount, CashCount? superseded, CancellationToken cancellationToken);

    Task<IReadOnlyList<CashCountDto>> ListAsync(
        Guid userId,
        CashCountListCriteria criteria,
        CancellationToken cancellationToken);

    Task<CashCount?> FindOwnedByIdAsync(
        Guid cashCountId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);

    /// <summary>
    /// Düzeltme kaydını ve sayımdaki bağlantısını tek yazma sınırında kaydeder.
    /// </summary>
    Task SaveAdjustmentAsync(BudgetTransaction adjustment, CancellationToken cancellationToken);
}
