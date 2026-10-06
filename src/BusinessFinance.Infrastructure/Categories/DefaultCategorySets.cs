using BusinessFinance.Application.Cash;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Categories;

/// <summary>
/// Yeni kullanıcıya açılışta kurulan iki kategori seti.
/// </summary>
/// <remarks>
/// <para>
/// Set, kaydolurken sorulan tek soruya göre seçilir: işletmeniz var mı.
/// Seçim <b>hiçbir özelliği kapatmaz</b>; yalnız hangi kalemlerle
/// başlanacağını ve kapsam boyutunun arayüzde görünüp görünmeyeceğini
/// belirler.
/// </para>
/// <para>
/// <b>İşletme seti iki parçalıdır ve olmak zorundadır.</b> Esnafın market
/// alışverişi de aynı uygulamaya giriyor: şahıs şirketinde kasa ile cep
/// hukuken ayrı değil (ADR 0013). Yalnız işletme kalemleri koymak, kullanıcıyı
/// ilk şahsi harcamasında kategori uydurmaya zorlardı.
/// </para>
/// <para>
/// <b>Ödeme yöntemi kategori değildir.</b> "Satış geliri (kart)" gibi bir kalem
/// açılmaz; tahsilatın kartla mı nakit mi olduğu hesaptan bellidir ve kategoriye
/// taşınması aynı geliri iki kovaya bölerdi.
/// </para>
/// <para>
/// <b>Varlık sınıfı kategori değildir.</b> "Borsa" ya da "Kripto" gibi bir
/// kategori alımı da satımı da aynı kovaya atar ve rapor anlamsızlaşır; kazanç
/// <c>Yatırım getirisi</c> ve <c>Temettü</c> olarak izlenir.
/// </para>
/// <para>
/// Aynı ad iki tipte birden bulunabilir — <c>Hediye</c> hem alınır hem verilir —
/// çünkü teklik <c>(kullanıcı, ad, tip)</c> üçlüsündedir. Bir set içinde aynı
/// <c>(ad, tip)</c> ikilisi <b>iki kez geçemez</b>; bir test bunu koruyor.
/// </para>
/// <para>
/// <c>LegacyName</c> yalnız kişisel setin ilk sekizinde anlamlı: o kayıtlar bir
/// zamanlar İngilizce adlarla oluşturulmuştu ve okunurken Türkçeye taşınıyorlar.
/// Sonradan eklenenler baştan Türkçe, o yüzden iki ad aynı.
/// </para>
/// </remarks>
internal static class DefaultCategorySets
{
    /// <param name="IsTax">
    /// Vergi işareti (ADR 0018 T6). Her sette tek bir kategori işaretli gelir;
    /// vergi ekranı ödenen vergiyi o kategoriye yazar ve "Ödenenler" listesini
    /// işaretli kategorilerden okur. Kullanıcı sonradan başka kategorileri de
    /// işaretleyebilir.
    /// </param>
    internal readonly record struct DefaultCategory(
        string LegacyName,
        string Name,
        CategoryType Type,
        TransactionScope Scope,
        bool IsTax = false);

    /// <summary>İşletme setinin vergi kategorisi.</summary>
    internal const string BusinessTaxCategoryName = "SGK ve vergi ödemesi";

    /// <summary>Kişisel setin vergi kategorisi.</summary>
    internal const string PersonalTaxCategoryName = "Vergi ve harç";

    private static DefaultCategory Personal(string name, CategoryType type) =>
        new(name, name, type, TransactionScope.Personal);

    private static DefaultCategory Business(string name, CategoryType type) =>
        new(name, name, type, TransactionScope.Business);

    /// <summary>
    /// İşletmesi olmayan kullanıcının seti. Devralınan liste olduğu gibi kaldı;
    /// yalnız her kalem <c>Şahsi</c> varsayılan kapsamını taşıyor, böylece
    /// kapsam boyutu arayüzde hiç görünmese bile türetme zinciri her kayıtta
    /// çözülüyor.
    /// </summary>
    internal static readonly DefaultCategory[] PersonalSet =
    [
        new("Salary", "Maaş", CategoryType.Income, TransactionScope.Personal),
        Personal("Ek iş", CategoryType.Income),
        Personal("Kira geliri", CategoryType.Income),
        Personal("Yatırım getirisi", CategoryType.Income),
        Personal("Temettü", CategoryType.Income),
        Personal("Faiz geliri", CategoryType.Income),
        Personal("Hediye", CategoryType.Income),
        Personal("İade ve geri ödeme", CategoryType.Income),
        new("Other Income", "Diğer Gelir", CategoryType.Income, TransactionScope.Personal),

        new("Groceries", "Market Alışverişi", CategoryType.Expense, TransactionScope.Personal),
        new("Housing", "Konut", CategoryType.Expense, TransactionScope.Personal),
        new("Bills", "Faturalar", CategoryType.Expense, TransactionScope.Personal),
        new("Transport", "Ulaşım", CategoryType.Expense, TransactionScope.Personal),
        Personal("Yakıt", CategoryType.Expense),
        new("Health", "Sağlık", CategoryType.Expense, TransactionScope.Personal),
        Personal("Eğitim", CategoryType.Expense),
        Personal("Yeme-içme", CategoryType.Expense),
        Personal("Giyim", CategoryType.Expense),
        new("Entertainment", "Eğlence", CategoryType.Expense, TransactionScope.Personal),
        Personal("Abonelikler", CategoryType.Expense),
        Personal("Kişisel bakım", CategoryType.Expense),
        Personal("Ev eşyası", CategoryType.Expense),
        Personal("Evcil hayvan", CategoryType.Expense),
        Personal("Hediye", CategoryType.Expense),
        Personal(PersonalTaxCategoryName, CategoryType.Expense) with { IsTax = true },
        Personal("Sigorta", CategoryType.Expense),

        // Borcun faizi gerçek bir giderdir ve şimdiye kadar kategorisizdi.
        Personal("Faiz ve finansman gideri", CategoryType.Expense),
        Personal("Bağış", CategoryType.Expense),

        // Sebebi bilinmeyen kasa eksiği buraya yazılır (Aşama 06.3 K10).
        Personal(CashCountDefaults.DifferenceCategoryName, CategoryType.Expense),
        Personal("Diğer gider", CategoryType.Expense)
    ];

    /// <summary>
    /// Esnafın seti: işletme kalemleri ve patronun gündelik hayatı için derli
    /// toplu bir şahsi alt küme.
    /// </summary>
    internal static readonly DefaultCategory[] BusinessSet =
    [
        Business("Satış geliri", CategoryType.Income),
        Business("Hizmet geliri", CategoryType.Income),
        Business("Diğer işletme geliri", CategoryType.Income),
        Personal("Kira geliri", CategoryType.Income),
        Personal("Faiz geliri", CategoryType.Income),
        Personal("Hediye", CategoryType.Income),
        Personal("Diğer Gelir", CategoryType.Income),

        Business("Ticari mal alımı", CategoryType.Expense),
        Business("İşyeri kirası", CategoryType.Expense),
        Business("Personel ücreti", CategoryType.Expense),
        Business(BusinessTaxCategoryName, CategoryType.Expense) with { IsTax = true },
        Business("Elektrik, su, doğalgaz", CategoryType.Expense),
        Business("İletişim", CategoryType.Expense),
        Business("Nakliye ve kargo", CategoryType.Expense),
        Business("Ambalaj ve sarf malzemesi", CategoryType.Expense),
        Business("Bakım-onarım", CategoryType.Expense),
        Business("Araç ve yakıt", CategoryType.Expense),
        Business("Muhasebeci ve danışmanlık", CategoryType.Expense),
        Business("Banka ve POS komisyonu", CategoryType.Expense),
        Business("Reklam", CategoryType.Expense),
        Business("Sigorta", CategoryType.Expense),
        Business("Faiz ve finansman gideri", CategoryType.Expense),

        // Sebebi bilinmeyen kasa eksiği buraya yazılır (Aşama 06.3 K10).
        Business(CashCountDefaults.DifferenceCategoryName, CategoryType.Expense),
        Business("Diğer işletme gideri", CategoryType.Expense),

        Personal("Market Alışverişi", CategoryType.Expense),
        Personal("Konut", CategoryType.Expense),
        Personal("Faturalar", CategoryType.Expense),
        Personal("Ulaşım", CategoryType.Expense),
        Personal("Sağlık", CategoryType.Expense),
        Personal("Yeme-içme", CategoryType.Expense),
        Personal("Giyim", CategoryType.Expense),
        Personal("Eğitim", CategoryType.Expense),
        Personal("Kişisel bakım", CategoryType.Expense),
        Personal("Eğlence", CategoryType.Expense),
        Personal("Abonelikler", CategoryType.Expense),
        Personal("Diğer gider", CategoryType.Expense)
    ];

    internal static DefaultCategory[] For(bool hasBusiness) =>
        hasBusiness ? BusinessSet : PersonalSet;
}
