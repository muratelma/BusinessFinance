using BusinessFinance.Application.Counterparties;

namespace BusinessFinance.Application.Tests.Counterparties;

/// <summary>
/// Belgedeki adın kişilerle hoşgörülü eşleşmesi. Sonuç bir öneridir; bu
/// yüzden büyük-küçük harf, şirket unvanı ve küçük yazım farkı eşleşmeyi
/// bozmaz. Ama yanlış kişiyi seçili getirmek, hiç getirmemekten kötüdür:
/// testlerin yarısı <b>eşleşmemesi gereken</b> adlardır.
/// </summary>
public sealed class CounterpartyNameMatcherTests
{
    private static readonly Guid Wanted = Guid.NewGuid();
    private static readonly Guid Other = Guid.NewGuid();
    private static readonly Guid Third = Guid.NewGuid();

    private static Guid? Match(string? scanned, params (Guid Id, string Name)[] people) =>
        CounterpartyNameMatcher.FindBest(scanned, people);

    /// <summary>
    /// 8 Ekim 2026'da cihazda görülen durum: belge büyük harfle ve unvanla
    /// yazıyor, kişi küçük harfle kayıtlı. İ/i ve I/ı eşleşmeyi bozuyordu.
    /// </summary>
    [Theory]
    [InlineData("ÖRNEK ELEKTRİK DAĞITIM A.Ş.", "Örnek Elektrik Dağıtım A.Ş.")]
    [InlineData("ÖRNEK ELEKTRİK DAĞITIM A.Ş.", "Örnek Elektrik Dağıtım")]
    [InlineData("ÖRNEK ELEKTRİK DAĞITIM ANONİM ŞİRKETİ", "örnek elektrik dağıtım")]
    [InlineData("ORNEK ELEKTRIK DAGITIM", "Örnek Elektrik Dağıtım A.Ş.")]
    [InlineData("YILMAZ GIDA SAN. VE TİC. LTD. ŞTİ.", "Yılmaz Gıda")]
    [InlineData("Örnek  Elektrik-Dağıtım", "Örnek Elektrik Dağıtım")]
    public void CaseTurkishLettersPunctuationAndCompanyTitle_DoNotBreakTheMatch(
        string scanned, string saved)
    {
        Assert.Equal(Wanted, Match(scanned, (Wanted, saved), (Other, "Toptancı Ali")));
    }

    /// <summary>Boşluk farkı ad farkı değildir.</summary>
    [Theory]
    [InlineData("A 101", "A101")]
    [InlineData("ENERJİ SA", "Enerjisa")]
    public void ASpacingDifference_StillMatches(string scanned, string saved)
    {
        Assert.Equal(Wanted, Match(scanned, (Wanted, saved), (Other, "Toptancı Ali")));
    }

    /// <summary>
    /// Kullanıcı kısa adı yazmıştır; belgede uzun unvan geçer. Tersi de olur.
    /// </summary>
    [Theory]
    [InlineData("ENERJİSA BAŞKENT ELEKTRİK PERAKENDE SATIŞ A.Ş.", "Enerjisa")]
    [InlineData("BİM BİRLEŞİK MAĞAZALAR A.Ş.", "Bim")]
    [InlineData("ENERJİSA", "Enerjisa Başkent Elektrik")]
    [InlineData("ÖZEL ÖRNEK HASTANESİ SAĞLIK HİZMETLERİ", "Örnek Hastanesi")]
    public void AShortName_MatchesTheLongOne(string scanned, string saved)
    {
        Assert.Equal(Wanted, Match(scanned, (Wanted, saved), (Other, "Toptancı Ali")));
    }

    /// <summary>
    /// Sektör kelimesi kimseyi ayırt etmez: tek kelimelik ad yalnız uzun adın
    /// ilk kelimesiyse sayılır. Çok kelimeli ad da dağınık değil, bitişik ve
    /// aynı sırada geçmelidir.
    /// </summary>
    [Theory]
    [InlineData("ŞOK MARKET", "Market")]
    [InlineData("ÖRNEK ELEKTRİK DAĞITIM A.Ş.", "Elektrik")]
    [InlineData("BAŞKA ELEKTRİK DAĞITIM A.Ş.", "Örnek Elektrik Dağıtım")]
    [InlineData("YILMAZ GIDA VE AHMET İNŞAAT", "Ahmet Yılmaz")]
    [InlineData("SU", "Su Arıtma Servisi")]
    [InlineData("AL", "Al Yapı Market")]
    public void AGenericOrScatteredWord_MatchesNobody(string scanned, string saved)
    {
        Assert.Null(Match(scanned, (Wanted, saved), (Other, "Toptancı Ali")));
    }

    /// <summary>Tek harflik yazım farkı, yeterince uzun tek bir kelimede.</summary>
    [Theory]
    [InlineData("Tedarikci Ahmet", "Tedarikçi Ahmed")]
    [InlineData("ÖRNEK ELEKTRİK DAĞITIM", "Örnek Elektirk Dağıtım")]
    [InlineData("ÖRNEK ELEKTRİK DAĞITIM", "Örnek Elektrk Dağıtım")]
    [InlineData("Toptanci Ali", "Toptancıı Ali")]
    public void ASingleLetterTypoInOneWord_StillMatches(string scanned, string saved)
    {
        Assert.Equal(Wanted, Match(scanned, (Wanted, saved), (Other, "Bambaşka Biri")));
    }

    /// <summary>
    /// Türkçe adlar birbirine çok yakındır; yazım hoşgörüsü ayrı insanları
    /// birleştirmemelidir. Kısa kelimede, iki harfte ya da iki ayrı kelimede
    /// fark varsa eşleşme yoktur.
    /// </summary>
    [Theory]
    [InlineData("ALİ KARA", "Ali Kaya")]
    [InlineData("MEHMET YILDIZ", "Mehmet Yılmaz")]
    [InlineData("AHMET DEMİREL", "Ahmet Demir")]
    [InlineData("AYŞE KOÇ", "Ayşe Koç Butik Giyim")]
    [InlineData("HASAM USTO", "Hasan Usta")]
    [InlineData("Bambaşka Bir Firma", "Örnek Elektrik Dağıtım")]
    public void DifferentPeopleWithCloseNames_DoNotMatch(string scanned, string saved)
    {
        // "Ayşe Koç" ile "Ayşe Koç Butik Giyim" eşleşir; satır o yüzden ayrı.
        var expected = scanned == "AYŞE KOÇ" ? Wanted : (Guid?)null;

        Assert.Equal(expected, Match(scanned, (Wanted, saved), (Other, "Toptancı Ali")));
    }

    /// <summary>
    /// Bilinen sınır: beş harfli iki ad tek harfle ayrılıyorsa (Hasan/Hakan)
    /// kural onları ayıramaz. Kişilerde ikisi de varsa belirsizlik kuralı
    /// devreye girer ve kimse önerilmez; yalnız biri varsa o önerilir ve
    /// kullanıcı formda görür.
    /// </summary>
    [Fact]
    public void TwoPeopleOneLetterApart_SuggestTheExactOne()
    {
        var match = Match("HAKAN USTA", (Wanted, "Hakan Usta"), (Other, "Hasan Usta"));

        Assert.Equal(Wanted, match);
    }

    /// <summary>
    /// Yanlış kişiyi seçili getirmek, hiç getirmemekten kötüdür: aynı ölçüde
    /// uyan iki aday varsa hiçbiri önerilmez.
    /// </summary>
    [Theory]
    [InlineData("AHMET YILMAZ", "Ahmet Yılmaz Gıda", "Ahmet Yılmaz İnşaat")]
    [InlineData("HAZAN USTA", "Hasan Usta", "Hakan Usta")]
    [InlineData("ÖRNEK ELEKTRİK", "ÖRNEK ELEKTRİK", "Örnek Elektrik A.Ş.")]
    public void TwoEquallyGoodCandidates_SuggestNobody(string scanned, string first, string second)
    {
        Assert.Null(Match(scanned, (Wanted, first), (Other, second)));
    }

    /// <summary>Daha çok kelimesi uyan aday, daha azı uyanın önüne geçer.</summary>
    [Fact]
    public void TheMoreSpecificName_Wins()
    {
        var match = Match(
            "ENERJİSA BAŞKENT ELEKTRİK PERAKENDE SATIŞ A.Ş.",
            (Other, "Enerjisa"),
            (Wanted, "Enerjisa Başkent"),
            (Third, "Başkent Doğalgaz"));

        Assert.Equal(Wanted, match);
    }

    /// <summary>Aynı ad, içinde geçen adın ve yazım farkının önüne geçer.</summary>
    [Fact]
    public void AnExactName_BeatsEveryOtherKind()
    {
        var match = Match(
            "Şok Market",
            (Other, "Şok"),
            (Wanted, "ŞOK MARKET"),
            (Third, "Şokk Market"));

        Assert.Equal(Wanted, match);
    }

    /// <summary>
    /// Unvan kelimeleri yalnız sondan atılır; adın içindeki aynı kelime adın
    /// parçasıdır ve tamamı unvan olan ad boşalmaz.
    /// </summary>
    [Fact]
    public void TitleWordsInsideAName_AreKept()
    {
        Assert.Equal(Wanted, Match("A PLUS MARKET A.Ş.", (Wanted, "A Plus Market"), (Other, "Plus Market")));
        Assert.Null(Match("AYŞE VE FATMA BUTİK", (Wanted, "Ayşe Fatma Butik")));
        Assert.Equal(Wanted, Match("AŞ", (Wanted, "Aş")));
        Assert.Null(Match("A PLUS MARKET", (Wanted, "Plus Market Toptan")));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("...")]
    [InlineData(null)]
    public void AnEmptyName_MatchesNobody(string? scanned)
    {
        Assert.Null(Match(scanned, (Wanted, "Örnek Elektrik Dağıtım"), (Other, "...")));
    }

    [Fact]
    public void NoPeople_MatchesNobody()
    {
        Assert.Null(Match("Örnek Elektrik Dağıtım"));
    }
}
