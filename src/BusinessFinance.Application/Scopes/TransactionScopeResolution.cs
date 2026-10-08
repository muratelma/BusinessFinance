using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Scopes;

/// <summary>
/// Bir kaydın tarafının nasıl bulunacağı (ADR 0020). Kural tek bir yerde
/// yazılıdır çünkü her oluşturma use case'i aynı soruyu sorar ve farklı cevap
/// vermeleri, kullanıcının aynı harcamayı hangi ekrandan girdiğine göre farklı
/// etiketlenmesi demek olurdu.
/// </summary>
/// <remarks>
/// <para>
/// <b>Kategori, kaydın alabileceği tarafları belirler.</b> Etiketi doluysa
/// kaydın tarafı odur; boşsa kategori iki tarafa açıktır ve tarafı girişin
/// bağlamı, bağlam yoksa kullanıcının seçimi belirler.
/// </para>
/// <para>
/// <b>Hesabın ve kartın etiketi paranın tarafını söyler</b>, kaydın tarafını
/// belirlemez: yalnız iki tarafa açık kategoride, seçim gelmediğinde seçimin
/// ön değeridir. Dükkân kasasından yapılan market alışverişi şahsi bir
/// giderdir; kasanın etiketi onu işletme gideri yapmaz.
/// </para>
/// <para>
/// <b>Çelişen açık seçim reddedilir.</b> İstek, kategorinin izin vermediği ya
/// da girişin bağlamıyla çelişen bir taraf taşıyorsa sunucu onu sessizce
/// düzeltmez; aksi hâlde istemcinin gösterdiği ile yazılan ayrışırdı. Sunucu
/// taraf <b>uydurmaz</b>: hiçbir işaret yoksa kayıt yazılmaz.
/// </para>
/// </remarks>
public static class TransactionScopeResolution
{
    /// <summary>
    /// Bağlamı olmayan giriş (gelir, gider, kart harcaması, plan, borç,
    /// yükümlülük, bütçe): kategori → açık seçim → kaynağın etiketi.
    /// </summary>
    /// <param name="requested">Kullanıcının açık seçimi.</param>
    /// <param name="categorySide">
    /// Kategorinin etiketi; boşsa kategori iki tarafa açıktır. Kategorisi
    /// olmayan kayıt (nakit borç) iki tarafa açık kategori gibi davranır.
    /// </param>
    /// <param name="sourceLabel">Hesabın ya da kartın etiketi; kaynağı olmayan kayıtta boş.</param>
    public static ScopeResolution Resolve(
        TransactionScope? requested,
        TransactionScope? categorySide,
        TransactionScope? sourceLabel)
    {
        if (categorySide is TransactionScope side)
        {
            return requested is null || requested == side
                ? ScopeResolution.Resolved(side)
                : ScopeResolution.Conflict;
        }

        return (requested ?? sourceLabel) is TransactionScope scope
            ? ScopeResolution.Resolved(scope)
            : ScopeResolution.Unresolved;
    }

    /// <summary>
    /// <see cref="Resolve"/> ile aynı kural; hiçbir işaret yoksa kullanıcının
    /// profiline bakar (ADR 0020 İ12).
    /// </summary>
    /// <remarks>
    /// İşletmesi olmayan kullanıcıda taraf sorulmaz: "kullanıcının seçimi"
    /// adımının yerini <c>Şahsi</c> alır. İşletmesi olan kullanıcıda sonuç
    /// çözülmemiş kalır ve çağıran isteği reddeder. Profil yalnız bu son
    /// adımda okunur.
    /// </remarks>
    public static async Task<ScopeResolution> ResolveAsync(
        TransactionScope? requested,
        TransactionScope? categorySide,
        TransactionScope? sourceLabel,
        IUserProfileRepository profileRepository,
        Guid userId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(profileRepository);
        var resolution = Resolve(requested, categorySide, sourceLabel);
        if (resolution.Failure != ScopeResolutionFailure.Unresolved)
        {
            return resolution;
        }

        var profile = await profileRepository.FindAsync(userId, track: false, cancellationToken);
        return profile?.HasBusiness ?? false
            ? resolution
            : ScopeResolution.Resolved(TransactionScope.Personal);
    }

    /// <summary>
    /// Bağlamı olan giriş: girişin kendisi tek bir tarafa aittir (POS satışı,
    /// gün sonu, cari borçlandırma, komisyon ve kesinti işletmenindir).
    /// </summary>
    /// <remarks>
    /// Bağlamların listesi kapalıdır (ADR 0020 T2). Kategori öbür tarafa
    /// özelse ya da istek öbür tarafı istiyorsa sonuç çelişkidir; kaynağın
    /// etiketine bakılmaz.
    /// </remarks>
    public static ScopeResolution ResolveInContext(
        TransactionScope context,
        TransactionScope? requested,
        TransactionScope? categorySide)
    {
        return (categorySide ?? context) == context && (requested ?? context) == context
            ? ScopeResolution.Resolved(context)
            : ScopeResolution.Conflict;
    }

    /// <summary>
    /// Vergi kaydının kapsamı: kullanıcının açık seçimi → profilin tarafı
    /// (ADR 0018 İ9).
    /// </summary>
    /// <remarks>
    /// Ödeme kaynağının etiketi vergide kapsamı <b>belirlemez</b>: işletme
    /// vergisini şahsi kartla ödemek olağandır ve kartın etiketi vergiyi şahsi
    /// yapmamalıdır (kullanıcı kararı, 30 Eylül 2026). Seçim vergi tanımında
    /// sorulur; tanımsız toplu ödemede sorulmaz ve profilin tarafına yazılır.
    /// Profilin cevabı her zaman vardır; bu yüzden sonuç hiçbir zaman boş
    /// değildir — uydurma değil, kullanıcının kendi cevabıdır.
    /// </remarks>
    public static TransactionScope ResolveTax(
        TransactionScope? requested,
        bool hasBusiness)
    {
        return requested ??
            (hasBusiness ? TransactionScope.Business : TransactionScope.Personal);
    }
}

public enum ScopeResolutionFailure
{
    None = 0,

    /// <summary>Hiçbir işaret yok; sunucu taraf uydurmaz.</summary>
    Unresolved = 1,

    /// <summary>Açık seçim ya da kategori, izin verilen tarafla çelişiyor.</summary>
    Conflict = 2,
}

/// <summary>Taraf çözümünün sonucu: ya bir taraf ya reddin nedeni.</summary>
public readonly record struct ScopeResolution
{
    private ScopeResolution(TransactionScope? scope, ScopeResolutionFailure failure)
    {
        Scope = scope;
        Failure = failure;
    }

    public TransactionScope? Scope { get; }

    public ScopeResolutionFailure Failure { get; }

    public static ScopeResolution Unresolved { get; } =
        new(null, ScopeResolutionFailure.Unresolved);

    public static ScopeResolution Conflict { get; } =
        new(null, ScopeResolutionFailure.Conflict);

    public static ScopeResolution Resolved(TransactionScope scope) =>
        new(scope, ScopeResolutionFailure.None);

    /// <summary>
    /// Reddin hata kodu. Her özellik kendi kodunu taşıdığı için ikisini çağıran
    /// verir; çözülmüş bir sonuçta çağrılmaz.
    /// </summary>
    public ApplicationError ToError(ApplicationError unresolved, ApplicationError conflict) =>
        Failure == ScopeResolutionFailure.Conflict ? conflict : unresolved;
}
