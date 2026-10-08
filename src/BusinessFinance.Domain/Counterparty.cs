namespace BusinessFinance.Domain;

/// <summary>
/// Karşı taraf: müşteri, tedarikçi ya da ikisi birden.
/// </summary>
/// <remarks>
/// <b>Müşteri ve tedarikçi ayrı tip değildir.</b> Mahalle esnafında aynı kişi
/// hem alıcı hem satıcıdır — fırıncıdan ekmek alıp ona un satan bakkal gibi.
/// İkiye bölmek onu iki kayıt hâline getirir ve "Ahmet'le hesabım ne?"
/// sorusunu cevapsız bırakırdı. Yön kaydın kendisinde durur (ADR 0014).
///
/// Kimlik alanı taşımaz: adres, vergi numarası ve telefon Aşama 02'nin kapsamı
/// dışında. Tutulan tek şey ad ve kullanıcının kendi notu.
/// </remarks>
public sealed class Counterparty
{
    // Borç sözleşmeleri de adlarını buraya taşıdı; sınır ikisinin en
    // genişidir. Daha dar bir sınır, taşınan bir adı kırpmak ya da
    // sözleşmeyi taşıyamamak demek olurdu.
    public const int MaximumNameLength = 150;
    public const int MaximumNoteLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; private set; }

    /// <summary>
    /// Adın karşılaştırma anahtarı: "bu ad zaten var mı?" sorusunun tek
    /// ölçüsü (<see cref="NameKeyOf"/>). Arama, aynı ad denetimi ve
    /// veritabanındaki teklik kuralı <b>hep bunu</b> okur; üçü ayrı
    /// karşılaştırma kullansaydı bir yer "aynı kişi", öbürü "farklı kişi"
    /// derdi.
    /// </summary>
    public string NameKey { get; private set; }

    /// <summary>Kullanıcının kendi notu; "Çarşı girişindeki manav" gibi.</summary>
    public string? Note { get; private set; }

    public bool IsActive { get; private set; }

    private Counterparty()
    {
        Name = null!;
        NameKey = null!;
    }

    public Counterparty(Guid id, Guid userId, string name, string? note = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Counterparty id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        Id = id;
        UserId = userId;
        Name = NormalizeName(name);
        NameKey = NameKeyOf(Name);
        Note = NormalizeNote(note);
        IsActive = true;
    }

    public void Rename(string name)
    {
        var normalized = NormalizeName(name);
        var key = NameKeyOf(normalized);

        // Yalnız yazımı değişen ad anahtarı yeniden yazmaz: kural sıkılaşmadan
        // önce açılmış aynı adlı ikinci kişi kendi ayırt edici anahtarını
        // taşır ve yazımını düzeltmek onu ilkiyle çakıştırmamalıdır.
        if (!string.Equals(key, NameKeyOf(Name), StringComparison.Ordinal))
        {
            NameKey = key;
        }

        Name = normalized;
    }

    /// <summary>
    /// Bu kişiyi, aynı adı taşıyan daha eski bir kişiden ayrı tutar.
    /// </summary>
    /// <remarks>
    /// Yalnız <b>geçmiş veri</b> içindir: teklik kuralı harf büyüklüğüne
    /// Türkçe harflerle bakmaya başlamadan önce "ÖRNEK ELEKTRİK" ile "Örnek
    /// Elektrik" iki ayrı kişi olarak açılabiliyordu. İkisi de hareket taşıyor
    /// olabilir; birleştirmek kullanıcının kararıdır. Yükseltme ve geri
    /// yükleme ikincisine bu ayırt edici anahtarı verir; yeni kişi bu yoldan
    /// açılmaz.
    /// </remarks>
    public void KeepApartFromSameName() => NameKey = $"{NameKeyOf(Name)}#{Id:D}";

    /// <summary>
    /// İki adın <b>aynı kişi</b> sayılıp sayılmayacağını söyleyen anahtar:
    /// harf büyüklüğü ve boşluk farkı ad farkı değildir.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Dört i harfi (İ, I, ı, i) tek harfe iner. Türkçe kuralı (I→ı) "IKEA"
    /// ile "ikea"yı, İngilizce kuralı (I→i) "IŞIK" ile "ışık"ı ayırırdı;
    /// kullanıcı ikisini de aynı ad olarak yazar.
    /// </para>
    /// <para>
    /// Bundan <b>fazlası yapılmaz</b>: "Örnek" ile "Ornek", "Ahmet" ile
    /// "Ahmed" ayrı adlardır. Teklik bir yasaktır ve yanlış yasak kullanıcıyı
    /// ikinci bir gerçek kişiyi açamaz bırakır; yakın adları bulmak öneri
    /// tarafının işidir.
    /// </para>
    /// </remarks>
    public static string NameKeyOf(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var builder = new System.Text.StringBuilder(name.Length);
        var pendingSpace = false;
        foreach (var character in name)
        {
            if (char.IsWhiteSpace(character))
            {
                pendingSpace = builder.Length > 0;
                continue;
            }

            // "i̇": bazı klavyeler küçük i'yi ayrı bir nokta işaretiyle yazar.
            if (character == '\u0307')
            {
                continue;
            }

            if (pendingSpace)
            {
                builder.Append(' ');
                pendingSpace = false;
            }

            builder.Append(character is 'İ' or 'I' or 'ı' or 'i'
                ? 'i'
                : char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }

    /// <summary><c>null</c> vermek notu siler.</summary>
    public void SetNote(string? note) => Note = NormalizeNote(note);

    /// <summary>
    /// Pasifleştirme yeni iş yapmayı durdurur, geçmişi silmez.
    /// </summary>
    /// <remarks>
    /// Açık bakiyesi olan karşı taraf da pasifleştirilebilir: kullanıcı artık
    /// ondan mal almıyor olabilir ama alacağı durur ve tahsil edilebilir.
    /// Pasif karşı tarafa yeni borçlandırma yazılamaz
    /// (<see cref="CounterpartyCharge"/>), tahsilat ise yazılabilir
    /// (<see cref="CounterpartyPayment"/>) — aksi hâlde bakiye kapatılamaz
    /// hâle gelirdi.
    /// </remarks>
    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Counterparty name is required.", nameof(name));
        }

        var normalized = name.Trim();
        if (normalized.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"Counterparty name cannot exceed {MaximumNameLength} characters.",
                nameof(name));
        }

        return normalized;
    }

    private static string? NormalizeNote(string? note)
    {
        var normalized = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (normalized?.Length > MaximumNoteLength)
        {
            throw new ArgumentException(
                $"Counterparty note cannot exceed {MaximumNoteLength} characters.",
                nameof(note));
        }

        return normalized;
    }
}
