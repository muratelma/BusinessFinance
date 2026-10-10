namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Ad tekliğinin tek kuralı (kullanıcı kararı, 9 Ekim 2026): anahtarda yalnız
/// harfler ve rakamlar kalır. Kişi, hesap, kredi kartı, kategori ve POS aynı
/// kuralı kullanır.
/// </summary>
public sealed class NameKeysTests
{
    [Theory]
    // Harf büyüklüğü ve Türkçe i harfleri.
    [InlineData("İş Bankası", "İŞ BANKASI")]
    [InlineData("İş Bankası", "iş bankası")]
    [InlineData("IŞIK MARKET", "ışık market")]
    [InlineData("IKEA", "ikea")]
    [InlineData("İkea", "IKEA")]
    [InlineData("i̇stanbul Toptan", "İSTANBUL TOPTAN")]
    // Boşluk hiç sayılmaz.
    [InlineData("İş Bankası", "iş  bankası")]
    [InlineData("İş Bankası", "işbankası")]
    [InlineData("İş Bankası", " İş\tBankası ")]
    [InlineData("Ali Can", "Alican")]
    [InlineData("A 101", "A101")]
    // Noktalama sayılmaz.
    [InlineData("İş Bankası", "İş-Bankası")]
    [InlineData("İş Bankası", "İş Bankası.")]
    [InlineData("Örnek Elektrik A.Ş.", "örnek elektrik aş")]
    [InlineData("Kasa 1", "Kasa-1")]
    [InlineData("Ziraat (Şahsi)", "Ziraat Şahsi")]
    [InlineData("Ali'nin Dükkanı", "Alinin Dükkanı")]
    [InlineData("A&B", "AB")]
    public void SameName_WhateverTheCasingSpacingOrPunctuation(string first, string second)
    {
        Assert.Equal(NameKeys.Of(first), NameKeys.Of(second));
    }

    [Theory]
    // Türkçe harf başka harftir: yanlış yasak gerçek ikinci kaydı açtırmaz.
    [InlineData("İş Bankası", "Is Bankasi")]
    [InlineData("Örnek Elektrik", "Ornek Elektrik")]
    [InlineData("Koç", "Koc")]
    [InlineData("Şok", "Sok")]
    [InlineData("Can", "Çan")]
    // Yazım farkı ad farkıdır.
    [InlineData("Ahmet Usta", "Ahmed Usta")]
    [InlineData("Ali Kaya", "Ali Kara")]
    // Ayırt edici ek başka addır.
    [InlineData("İş Bankası", "İş Bankası 2")]
    [InlineData("İş Bankası", "İş Bankası Şahsi")]
    [InlineData("İş Bankası", "İş Bankası 4512")]
    [InlineData("Örnek Elektrik A.Ş.", "Örnek Elektrik")]
    public void DifferentName_StaysDifferent(string first, string second)
    {
        Assert.NotEqual(NameKeys.Of(first), NameKeys.Of(second));
    }

    [Fact]
    public void Key_KeepsOnlyLettersAndDigits()
    {
        Assert.Equal("işbankasi4512", NameKeys.Of("  İŞ-Bankası (4512).  "));
        Assert.Equal("çağrişükrüöztürk", NameKeys.Of("Çağrı Şükrü ÖZTÜRK"));
    }

    /// <summary>
    /// Hiç harf ya da rakam taşımayan ad boş anahtar üretirdi ve böyle iki ad
    /// birbirine eşit sayılırdı.
    /// </summary>
    [Fact]
    public void NameWithoutLettersOrDigits_KeepsItsOwnCharacters()
    {
        Assert.Equal("---", NameKeys.Of(" - - - "));
        Assert.NotEqual(NameKeys.Of("---"), NameKeys.Of("***"));
        Assert.NotEqual(string.Empty, NameKeys.Of("₺"));
    }

    [Fact]
    public void Apart_AddsTheIdAndNeverCollidesWithAPlainKey()
    {
        var id = Guid.NewGuid();

        var apart = NameKeys.Apart("İş Bankası", id);

        Assert.Equal($"işbankasi#{id:D}", apart);
        // "#" harf ya da rakam değildir: hiçbir ad bu anahtarı üretemez.
        Assert.NotEqual(apart, NameKeys.Of($"işbankasi#{id:D}"));
    }

    [Fact]
    public void AfterRename_KeepsTheKeyWhenOnlyTheSpellingChanges()
    {
        var id = Guid.NewGuid();
        var apart = NameKeys.Apart("İŞ BANKASI", id);

        Assert.Equal(apart, NameKeys.AfterRename("İŞ BANKASI", apart, "İş Bankası"));
        Assert.Equal(apart, NameKeys.AfterRename("İŞ BANKASI", apart, "İş-Bankası"));
        Assert.Equal("işbankasişahsi", NameKeys.AfterRename("İŞ BANKASI", apart, "İş Bankası Şahsi"));
    }

    /// <summary>
    /// Beş kayıt türü de anahtarı kurulurken hesaplar, ad değişince günceller
    /// ve eski aynı adlı kaydı ayrı tutabilir.
    /// </summary>
    [Fact]
    public void EveryNamedRecord_CarriesTheSameKey()
    {
        var userId = Guid.NewGuid();
        var now = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);
        const string name = "İş-Bankası";
        const string key = "işbankasi";

        var account = new Account(Guid.NewGuid(), userId, name, AccountType.Bank, CurrencyCode.TRY);
        var income = new Category(Guid.NewGuid(), userId, name, CategoryType.Income);
        var expense = new Category(Guid.NewGuid(), userId, "Komisyon", CategoryType.Expense);
        var card = new CreditCard(
            Guid.NewGuid(), userId, name, new Money(1000m, CurrencyCode.TRY), 10, 20);
        var person = new Counterparty(Guid.NewGuid(), userId, name);
        var pos = new PosDefinition(
            Guid.NewGuid(), userId, name, account, income, 0.015m, expense, 1, false, now);

        Assert.All(
            new[] { account.NameKey, income.NameKey, card.NameKey, person.NameKey, pos.NameKey },
            value => Assert.Equal(key, value));

        // Yalnız yazım değişirse anahtar durur; ad gerçekten değişirse değişir.
        account.Rename("İŞ BANKASI");
        Assert.Equal(key, account.NameKey);
        account.Rename("İş Bankası Şahsi");
        Assert.Equal("işbankasişahsi", account.NameKey);

        income.Rename("iş bankası");
        Assert.Equal(key, income.NameKey);
        income.Rename("Satış");
        Assert.Equal("satiş", income.NameKey);

        card.Update("İş Bankası.", new Money(1000m, CurrencyCode.TRY), 10, 20, 20m, true);
        Assert.Equal(key, card.NameKey);
        card.Update("Bonus", new Money(1000m, CurrencyCode.TRY), 10, 20, 20m, true);
        Assert.Equal("bonus", card.NameKey);

        pos.Update("İş Bankası", account, income, 0.015m, expense, 1, false);
        Assert.Equal(key, pos.NameKey);
        pos.Update("Garanti POS", account, income, 0.015m, expense, 1, false);
        Assert.Equal("garantipos", pos.NameKey);

        // Eski aynı adlı kayıt ayrı tutulur ve yazım düzeltmesi onu geri
        // çakıştırmaz.
        var second = new Account(Guid.NewGuid(), userId, "GARANTİ", AccountType.Bank, CurrencyCode.TRY);
        second.KeepApartFromSameName();
        Assert.Equal($"garanti#{second.Id:D}", second.NameKey);
        second.Rename("Garanti");
        Assert.Equal($"garanti#{second.Id:D}", second.NameKey);
    }
}
