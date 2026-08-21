namespace BusinessFinance.Application.Abstractions.Queries;

/// <summary>
/// Tarihli bir geçmiş listesinin sınırı.
/// </summary>
/// <remarks>
/// Kart hareketleri ve transferler her gün büyüyen kayıtlar; ikisi de tarih
/// filtresi ve üst sınır olmadan tüm geçmişi çekiyordu. Bu ilk aylarda
/// görünmez, sonra sessizce yavaşlar ve düzeltilmesi zorlaşır.
///
/// Sınır **iki** katmanlı, çünkü tek başına ikisi de yetmiyor:
/// tarih penceresi kullanıcının ne gördüğünü belirler (varsayılan son üç ay),
/// satır tavanı ise pencere ne kadar geniş olursa olsun sorgunun büyüklüğünü
/// sabitler. Kullanıcı "tümü" dediğinde bile sunucu sınırsız satır dönmez;
/// bunun yerine kırpıldığını söyler.
/// </remarks>
public sealed record HistoryWindow(DateOnly? From, DateOnly? To)
{
    /// <summary>Tarih verilmediğinde geriye bakılan ay sayısı.</summary>
    public const int DefaultMonths = 3;

    /// <summary>
    /// Bir listede dönebilecek en fazla satır.
    /// </summary>
    /// <remarks>
    /// Ekranda taranabilecek bir sayı değil, sorgunun üst sınırı. Aşılırsa
    /// veri gizlenmez: <c>hasMore</c> ile bildirilir ve kullanıcı pencereyi
    /// daraltarak eskiye iner.
    /// </remarks>
    public const int MaximumRows = 200;

    /// <summary>
    /// Tarih verilmemişse son <see cref="DefaultMonths"/> ayı kapsayan pencere.
    /// </summary>
    /// <remarks>
    /// Varsayılanı sunucu koyuyor: istemci alan göndermeyi unutursa sessizce
    /// sınırsız sorguya dönmemeli. Açıkça <c>from=null</c> istenemez; "tümü"
    /// isteği <see cref="Unbounded"/> ile ayrı bir niyet olarak taşınır.
    /// </remarks>
    public static HistoryWindow DefaultFor(DateOnly today) =>
        new(today.AddMonths(-DefaultMonths), null);

    /// <summary>Tarih sınırı olmayan pencere; satır tavanı yine geçerlidir.</summary>
    public static HistoryWindow Unbounded { get; } = new(null, null);

    public bool Contains(DateOnly date) =>
        (From is not DateOnly from || date >= from) &&
        (To is not DateOnly to || date <= to);
}

/// <summary>
/// Sınırlanmış bir liste ve kırpılıp kırpılmadığı.
/// </summary>
/// <param name="HasMore">
/// Pencerede tavandan daha fazla kayıt vardı. Kullanıcıya "hepsi bu" demek
/// yerine eskisinin de olduğunu söylemek için var: sessizce kırpmak, ekranı
/// yanlış bir tamlık iddiasında bırakırdı.
/// </param>
public sealed record BoundedList<T>(IReadOnlyList<T> Items, bool HasMore);
