using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

/// <summary>
/// Bir gider kaydının indirilebilirlik cevabının nasıl bulunacağı.
/// </summary>
/// <remarks>
/// Kapsam zincirinin (`TransactionScopeResolution`) yanındaki ikinci zincir ve
/// aynı gerekçeyle tek yerde yazılı: her oluşturma use case'i aynı soruyu
/// sorar, farklı cevap vermeleri aynı harcamanın hangi ekrandan girildiğine
/// göre farklı işaretlenmesi olurdu.
///
/// Kapsam zincirinden bir farkı var: cevap bulunamazsa istek <b>reddedilmez</b>.
/// Kapsam boşsa rapor bozulur; indirilebilirlik boşsa yalnız muhasebeci
/// paketinde "cevaplanmamış" olarak durur ve muhasebeci zaten ona bakar.
/// </remarks>
public static class TaxDeductibilityResolution
{
    /// <summary>
    /// Türetme sırası: kullanıcının açık seçimi → kategorinin varsayılanı.
    /// </summary>
    /// <remarks>
    /// Soru yalnız <b>işletme kapsamlı gider</b> kaydında sorulur. Şahsi kayıtta
    /// ya da gelirde kategorinin varsayılanı <b>sessizce düşer</b>: kullanıcı o
    /// cevabı istemedi, kategori söyledi. Kullanıcının kendi yazdığı cevap ise
    /// düşürülmez — çağıran onu reddeder, çünkü sessizce yok saymak kullanıcıya
    /// kaydettiği bir şeyin kaydedilmediğini söylemeden geçmek olurdu.
    /// </remarks>
    public static bool? Resolve(
        bool? requested,
        bool? categoryDefault,
        TransactionScope scope,
        bool recognizesExpense)
    {
        // Soru sorulmuyorsa kategorinin cevabı düşer; kullanıcının cevabı
        // düşmez, çağıranın onu reddetmesi için olduğu gibi geçer.
        return scope != TransactionScope.Business || !recognizesExpense
            ? requested
            : requested ?? categoryDefault;
    }
}
