using System.Text;

namespace BusinessFinance.Domain;

/// <summary>
/// "Bu ad zaten var mı?" sorusunun tek ölçüsü: iki adın <b>aynı ad</b>
/// sayılıp sayılmayacağını söyleyen anahtar.
/// </summary>
/// <remarks>
/// <para>
/// Kişi, hesap, kredi kartı, kategori ve POS aynı kuralı kullanır (kullanıcı
/// kararı, 9 Ekim 2026). Anahtarda <b>yalnız harfler ve rakamlar</b> kalır:
/// harf büyüklüğü, boşluk ve noktalama ad farkı değildir. "İş Bankası",
/// "İŞ BANKASI", "işbankası" ve "İş-Bankası." tek addır; ikinci bir İş
/// Bankası hesabı açmak isteyen kullanıcı ayırt edici bir ek yazar ("İş
/// Bankası Şahsi", "İş Bankası 4512").
/// </para>
/// <para>
/// Dört i harfi (İ, I, ı, i) tek harfe iner. Türkçe kuralı (I→ı) "IKEA" ile
/// "ikea"yı, İngilizce kuralı (I→i) "IŞIK" ile "ışık"ı ayırırdı; kullanıcı
/// ikisini de aynı ad olarak yazar.
/// </para>
/// <para>
/// Bundan <b>fazlası yapılmaz</b>: "Örnek" ile "Ornek", "Koç" ile "Koc",
/// "Ahmet" ile "Ahmed" ayrı adlardır. Teklik bir yasaktır ve yanlış yasak
/// kullanıcıyı gerçekten farklı ikinci bir kaydı açamaz bırakır; yakın adları
/// bulmak öneri tarafının işidir.
/// </para>
/// <para>
/// Anahtar uygulamada hesaplanır ve veritabanında ikili karşılaştırılır;
/// veritabanının harf kuralı sonucu etkilemez (o kural Türkçe İ/i ve I/ı
/// çiftlerini ayrı sayıyordu).
/// </para>
/// </remarks>
public static class NameKeys
{
    /// <summary>
    /// Kural sıkılaşmadan önce açılmış aynı adlı ikinci kaydın ayırt edici
    /// ekinin ("#" + kimlik) uzunluğu; anahtar kolonu adın en uzun hâline bu
    /// kadar pay bırakır.
    /// </summary>
    public const int ApartSuffixAllowance = 40;

    /// <summary>Adın karşılaştırma anahtarı.</summary>
    /// <remarks>
    /// Hiç harf ya da rakam taşımayan ad ("---", "₺") boş anahtar üretirdi ve
    /// böyle iki ad birbirine eşit sayılırdı; o durumda anahtar adın boşluksuz,
    /// küçük harfli hâlidir.
    /// </remarks>
    public static string Of(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var key = new StringBuilder(name.Length);
        var fallback = new StringBuilder(name.Length);
        foreach (var character in name)
        {
            // "i̇": bazı klavyeler küçük i'yi ayrı bir nokta işaretiyle yazar.
            if (char.IsWhiteSpace(character) || character == '̇')
            {
                continue;
            }

            var folded = character is 'İ' or 'I' or 'ı' or 'i'
                ? 'i'
                : char.ToLowerInvariant(character);
            fallback.Append(folded);
            if (char.IsLetterOrDigit(character))
            {
                key.Append(folded);
            }
        }

        return key.Length > 0 ? key.ToString() : fallback.ToString();
    }

    /// <summary>
    /// Aynı adı taşıyan daha eski bir kayıttan ayrı tutulan kaydın anahtarı.
    /// </summary>
    /// <remarks>
    /// Yalnız <b>geçmiş veri</b> içindir (yükseltme ve geri yükleme): kural
    /// sıkılaşmadan önce açılmış aynı adlı iki kayıt da hareket taşıyor
    /// olabilir; birleştirmek kullanıcının kararıdır. Yeni kayıt bu yoldan
    /// açılmaz. "#" bir harf ya da rakam değildir, bu yüzden hiçbir adın
    /// anahtarıyla çakışmaz.
    /// </remarks>
    public static string Apart(string name, Guid id) => $"{Of(name)}#{id:D}";

    /// <summary>
    /// Ad değişince anahtarın ne olacağı. Yalnız yazımı değişen ad (aynı
    /// anahtar) mevcut anahtarı korur: ayrı tutulan eski bir kaydın yazımını
    /// düzeltmek onu aynı adlı öbür kayıtla çakıştırmamalıdır.
    /// </summary>
    public static string AfterRename(string currentName, string currentKey, string newName)
    {
        var key = Of(newName);
        return string.Equals(key, Of(currentName), StringComparison.Ordinal) ? currentKey : key;
    }
}
