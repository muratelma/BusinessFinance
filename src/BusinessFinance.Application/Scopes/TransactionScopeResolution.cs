using BusinessFinance.Domain;

namespace BusinessFinance.Application.Scopes;

/// <summary>
/// Bir kaydın kapsamının nasıl bulunacağı. Sıra tek bir yerde yazılıdır çünkü
/// her oluşturma use case'i aynı soruyu sorar ve farklı cevap vermeleri,
/// kullanıcının aynı harcamayı hangi ekrandan girdiğine göre farklı
/// etiketlenmesi demek olurdu.
/// </summary>
public static class TransactionScopeResolution
{
    /// <summary>
    /// Türetme sırası: kullanıcının açık seçimi → hesabın/kartın etiketi →
    /// kategorinin varsayılanı.
    /// </summary>
    /// <remarks>
    /// Sıra keyfi değil: en özelden en genele gider. Kullanıcının o kayıt için
    /// yazdığı şey her şeyi yener; hesap ya da kart kategoriden daha çok bilgi
    /// taşır, çünkü dükkânın kasası hangi kategoriye girerse girsin işletmenin
    /// parasıdır; kategori ise paylaşılan bir kovadır ve en zayıf ipucudur.
    ///
    /// Üçü de boşsa sonuç <c>null</c>'dır ve çağıran isteği reddeder. Sunucu
    /// kapsam <b>uydurmaz</b>: yanlış etiketlenmiş bir kayıt, kullanıcının
    /// işletme netini sessizce bozar ve düzeltilene kadar fark edilmez.
    /// </remarks>
    public static TransactionScope? Resolve(
        TransactionScope? requested,
        TransactionScope? sourceDefault,
        TransactionScope? categoryDefault)
    {
        return requested ?? sourceDefault ?? categoryDefault;
    }

    /// <summary>
    /// Kaynağı olmayan kayıtlar için (bütçe gibi): açık seçim → kategori.
    /// </summary>
    public static TransactionScope? Resolve(
        TransactionScope? requested,
        TransactionScope? categoryDefault)
    {
        return Resolve(requested, null, categoryDefault);
    }
}
