using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;

namespace BusinessFinance.Infrastructure.Tests.Categories;

/// <summary>
/// İki varsayılan kategori setinin taşıması gereken özellikler. Set içeriği
/// ürün kararıdır ve değişebilir; bu testler içeriği değil, <b>kurallarını</b>
/// koruyor.
/// </summary>
public sealed class DefaultCategorySetTests
{
    public static TheoryData<bool> Sets => new(false, true);

    /// <summary>
    /// Taraf ya tanımlı bir değerdir ya boştur; boş "iki tarafa açık" demektir
    /// (ADR 0020).
    /// </summary>
    [Theory]
    [MemberData(nameof(Sets))]
    public void EverySet_GivesEveryCategoryAKnownSideOrLeavesItOpen(bool hasBusiness)
    {
        var set = DefaultCategorySets.For(hasBusiness);

        Assert.NotEmpty(set);
        Assert.All(set, item => Assert.True(
            item.Scope is not TransactionScope scope || Enum.IsDefined(scope)));
    }

    /// <summary>
    /// Adı iki tarafta aynı anlama gelen üç kalem iki tarafa açıktır: vergi
    /// formu şahsi vergiyi de aynı kaleme yazar, şahsi kredinin faizi ve kasko
    /// işletme gideri değildir. Tek taraflı kalsalardı bağlayıcı kural bu
    /// kayıtları ya yanlış tarafa yazar ya reddederdi.
    /// </summary>
    [Theory]
    [InlineData("SGK ve vergi ödemesi")]
    [InlineData("Faiz ve finansman gideri")]
    [InlineData("Sigorta")]
    public void BusinessSet_LeavesTheSharedItemsOpenToBothSides(string name)
    {
        var item = Assert.Single(
            DefaultCategorySets.BusinessSet,
            item => item.Name == name && item.Type == CategoryType.Expense);

        Assert.Null(item.Scope);
    }

    /// <summary>
    /// Ad tarafı söyler: çift hâlinde duran kalemlerin işyeri ve ev tarafı
    /// adından anlaşılır ve her biri kendi tarafına özeldir.
    /// </summary>
    [Theory]
    [InlineData("İşyeri kirası", TransactionScope.Business)]
    [InlineData("Ev kirası ve aidat", TransactionScope.Personal)]
    [InlineData("İşyeri faturaları", TransactionScope.Business)]
    [InlineData("Ev faturaları", TransactionScope.Personal)]
    [InlineData("Personel giderleri", TransactionScope.Business)]
    public void BusinessSet_NamesSayWhichSideTheCategoryBelongsTo(
        string name,
        TransactionScope side)
    {
        var item = Assert.Single(
            DefaultCategorySets.BusinessSet,
            item => item.Name == name && item.Type == CategoryType.Expense);

        Assert.Equal(side, item.Scope);
    }

    /// <summary>
    /// Teklik <c>(kullanıcı, ad, tip)</c> üçlüsündedir; bir set aynı ikiliyi
    /// iki kez içerirse kurulum tekil indekse takılır.
    /// </summary>
    [Theory]
    [MemberData(nameof(Sets))]
    public void EverySet_HasNoDuplicateNameAndTypePair(bool hasBusiness)
    {
        var duplicates = DefaultCategorySets.For(hasBusiness)
            .GroupBy(item => (item.Name, item.Type))
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key.Name} ({group.Key.Type})")
            .ToArray();

        Assert.Empty(duplicates);
    }

    /// <summary>
    /// Rapor faizi kanonik ada göre buluyor; ad her iki sette de bulunmazsa
    /// faiz kategorisiz kalır ve gider toplamı ile kategori listesi tutmaz.
    /// </summary>
    [Theory]
    [MemberData(nameof(Sets))]
    public void EverySet_CarriesTheCategoriesTheReportsLookUpByName(bool hasBusiness)
    {
        var set = DefaultCategorySets.For(hasBusiness);

        Assert.Contains(set, item =>
            item.Type == CategoryType.Expense &&
            item.Name == EfCategoryRepository.InterestExpenseCategoryName);
        Assert.Contains(set, item =>
            item.Type == CategoryType.Income &&
            item.Name == EfCategoryRepository.InterestIncomeCategoryName);
    }

    /// <summary>
    /// İşletmesi olmayan kullanıcı için kapsam gerçek bir soru değildir:
    /// arayüzde hiç görünmez ve her kayıt sessizce şahsi olur. Bu ancak setin
    /// tamamı <c>Şahsi</c> olduğunda doğru çalışır.
    /// </summary>
    [Fact]
    public void PersonalSet_IsEntirelyPersonal()
    {
        Assert.All(
            DefaultCategorySets.PersonalSet,
            item => Assert.Equal(TransactionScope.Personal, item.Scope));
    }

    /// <summary>
    /// İşletme seti iki parçalı olmak zorunda: esnafın market alışverişi de
    /// aynı uygulamaya giriyor. Yalnız işletme kalemleri koymak, kullanıcıyı
    /// ilk şahsi harcamasında kategori uydurmaya zorlardı.
    /// </summary>
    [Fact]
    public void BusinessSet_CarriesBothSidesOfTheOwnersLife()
    {
        var set = DefaultCategorySets.BusinessSet;

        Assert.Contains(set, item => item.Scope == TransactionScope.Business);
        Assert.Contains(set, item => item.Scope == TransactionScope.Personal);
        Assert.Contains(set, item =>
            item.Scope == TransactionScope.Personal && item.Type == CategoryType.Expense);
    }

    /// <summary>
    /// İşletme seti, aşama belgesinin saydığı kalemleri kapsıyor mu.
    /// </summary>
    [Theory]
    [InlineData("Satış geliri", CategoryType.Income)]
    [InlineData("Hizmet geliri", CategoryType.Income)]
    [InlineData("Ticari mal alımı", CategoryType.Expense)]
    [InlineData("İşyeri kirası", CategoryType.Expense)]
    [InlineData("Personel giderleri", CategoryType.Expense)]
    [InlineData("İşyeri faturaları", CategoryType.Expense)]
    [InlineData("İletişim", CategoryType.Expense)]
    [InlineData("Nakliye ve kargo", CategoryType.Expense)]
    [InlineData("Ambalaj ve sarf malzemesi", CategoryType.Expense)]
    [InlineData("Bakım-onarım", CategoryType.Expense)]
    [InlineData("Araç ve yakıt", CategoryType.Expense)]
    [InlineData("Muhasebeci ve danışmanlık", CategoryType.Expense)]
    [InlineData("Banka ve POS komisyonu", CategoryType.Expense)]
    [InlineData("Reklam", CategoryType.Expense)]
    public void BusinessSet_CoversTheTradeItems(string name, CategoryType type)
    {
        Assert.Contains(
            DefaultCategorySets.BusinessSet,
            item => item.Name == name &&
                    item.Type == type &&
                    item.Scope == TransactionScope.Business);
    }

    /// <summary>
    /// Ödeme yöntemi kategori değildir: kartla mı nakit mi tahsil edildiği
    /// hesaptan bellidir ve kategoriye taşınması aynı geliri iki kovaya bölerdi.
    /// </summary>
    [Theory]
    [MemberData(nameof(Sets))]
    public void EverySet_KeepsPaymentMethodOutOfCategoryNames(bool hasBusiness)
    {
        string[] paymentWords = ["kart", "nakit", "pos ile", "havale", "eft"];

        var offenders = DefaultCategorySets.For(hasBusiness)
            .Where(item => paymentWords.Any(word =>
                item.Name.Contains(word, StringComparison.OrdinalIgnoreCase)))
            .Select(item => item.Name)
            .ToArray();

        // "Banka ve POS komisyonu" bir ödeme yöntemi değil, bankanın kestiği
        // gerçek bir masraftır; kelime listesi onu yakalamamalı.
        Assert.Empty(offenders);
    }
}
