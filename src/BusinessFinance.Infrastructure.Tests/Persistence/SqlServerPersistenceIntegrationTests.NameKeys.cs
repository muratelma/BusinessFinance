using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// Ad tekliğinin tek kuralı, gerçek SQL Server'da: hesap, kredi kartı,
/// kategori, POS ve kişi (kullanıcı kararı, 9 Ekim 2026).
/// </summary>
/// <remarks>
/// Anahtarda yalnız harfler ve rakamlar kalır: "İş Bankası", "İŞ BANKASI",
/// "işbankası" ve "İş-Bankası." tek addır. Testler kuralın veritabanının harf
/// kuralına değil uygulamanın hesapladığı anahtara dayandığını ve yükseltmenin
/// eski veriyi kaybetmediğini tutar.
/// </remarks>
public sealed partial class SqlServerPersistenceIntegrationTests
{
    /// <summary>
    /// Yükseltme yolu: eski şemada meşru olan aynı adlı kayıtlar yükseltmeden
    /// sonra da hepsi durur; biri adı tutar, ötekiler ayrı bir anahtar alır.
    /// SQL'in doldurduğu anahtar uygulamanın hesapladığıyla aynıdır.
    /// </summary>
    [SqlServerFact]
    public async Task AddNameKeys_BackfillsEveryTableAndKeepsOldDuplicatesApart()
    {
        await using var database = await SqlTestDatabase.CreateAsync(
            GetConnectionString(), "AddCashCountTransferAdjustment");
        var user = CreateUser("name-keys-upgrade@example.test");
        var neighbour = CreateUser("name-keys-upgrade-neighbour@example.test");
        await database.SeedUsersAsync(user, neighbour);

        // Hesaplar: dört yazım tek ad; yalnız biri aktif, adı o tutar.
        var accounts = new (string Name, bool IsActive)[]
        {
            ("İş Bankası", true),
            ("İŞ BANKASI", false),
            ("işbankası", false),
            ("İş-Bankası.", false),
            ("Kasa 1", true),
            ("Is Bankasi", true),
            ("İş Bankası Şahsi", true),
            ("---", true),
            ("***", true),
        };
        var accountIds = accounts.ToDictionary(item => item.Name, _ => Guid.NewGuid());
        foreach (var (name, isActive) in accounts)
        {
            await database.ExecuteAsync(
                "INSERT INTO [Accounts] ([Id], [UserId], [Name], [Type], [Currency], " +
                "[OpeningBalance], [IsActive], [DefaultScope]) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, 0, {5}, NULL)",
                accountIds[name], user.Id, name, (byte)AccountType.Bank, (byte)CurrencyCode.TRY, isActive);
        }

        // Komşunun aynı adlı hesabı başka kullanıcınındır; çakışma sayılmaz.
        var neighbourAccountId = Guid.NewGuid();
        await database.ExecuteAsync(
            "INSERT INTO [Accounts] ([Id], [UserId], [Name], [Type], [Currency], " +
            "[OpeningBalance], [IsActive], [DefaultScope]) " +
            "VALUES ({0}, {1}, {2}, {3}, {4}, 0, 1, NULL)",
            neighbourAccountId, neighbour.Id, "iş bankası", (byte)AccountType.Bank, (byte)CurrencyCode.TRY);

        // Kategoriler: teklik türle birlikte. Gelir ve gider "Diğer" ikisi de
        // adı tutar; giderdeki ikinci yazım ayrılır.
        var categories = new (string Key, string Name, CategoryType Type, bool IsActive)[]
        {
            ("expense-diger", "Diğer", CategoryType.Expense, true),
            ("expense-DIGER", "DİĞER", CategoryType.Expense, false),
            ("income-diger", "Diğer", CategoryType.Income, true),
            ("income-satis", "Satış geliri", CategoryType.Income, true),
        };
        var categoryIds = categories.ToDictionary(item => item.Key, _ => Guid.NewGuid());
        foreach (var (key, name, type, isActive) in categories)
        {
            await database.ExecuteAsync(
                "INSERT INTO [Categories] ([Id], [UserId], [Name], [Type], [IsActive], " +
                "[DefaultScope], [IsTax]) VALUES ({0}, {1}, {2}, {3}, {4}, NULL, 0)",
                categoryIds[key], user.Id, name, (byte)type, isActive);
        }

        // Kartlar: boşluk farkı eski teklikten geçiyordu.
        var cards = new (string Name, bool IsActive)[]
        {
            ("Bonus Kart", true), ("BonusKart", false), ("Maximum", true),
        };
        var cardIds = cards.ToDictionary(item => item.Name, _ => Guid.NewGuid());
        foreach (var (name, isActive) in cards)
        {
            await database.ExecuteAsync(
                "INSERT INTO [CreditCards] ([Id], [UserId], [Name], [Limit], [Currency], " +
                "[StatementClosingDay], [PaymentDueDay], [MinimumPaymentRate], [IsActive], " +
                "[DefaultScope]) VALUES ({0}, {1}, {2}, 1000, {3}, 10, 20, 20, {4}, NULL)",
                cardIds[name], user.Id, name, (byte)CurrencyCode.TRY, isActive);
        }

        // POS: adında hiç teklik yoktu; birebir aynı adla iki POS açılabiliyordu.
        var posIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var posNames = new[] { "Garanti POS", "Garanti POS", "Ziraat POS" };
        for (var index = 0; index < posIds.Length; index++)
        {
            await database.ExecuteAsync(
                "INSERT INTO [PosDefinitions] ([Id], [UserId], [Name], [AccountId], " +
                "[SalesCategoryId], [CommissionCategoryId], [CommissionRate], [TransferDays], " +
                "[BusinessDaysOnly], [IsActive], [CreatedAtUtc], [IsDefault]) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, NULL, 0, 1, 0, {5}, SYSDATETIMEOFFSET(), 0)",
                posIds[index], user.Id, posNames[index], accountIds["Kasa 1"],
                categoryIds["income-satis"], index != 1);
        }

        // Kişiler: anahtar eski kuralla yazılmıştı (boşluk sayılıyordu) ve
        // biri zaten ayrı tutuluyordu.
        var people = new (string Name, string OldKey, bool IsActive)[]
        {
            ("Ali Can", "ali can", true),
            ("Alican", "alican", false),
            ("Örnek Elektrik", "örnek elektrik", true),
            ("Ahmet Bakkal", "ahmet bakkal", true),
        };
        var personIds = people.ToDictionary(item => item.Name, _ => Guid.NewGuid());
        foreach (var (name, oldKey, isActive) in people)
        {
            await database.ExecuteAsync(
                "INSERT INTO [Counterparties] ([Id], [UserId], [Name], [NameKey], [Note], [IsActive]) " +
                "VALUES ({0}, {1}, {2}, {3}, NULL, {4})",
                personIds[name], user.Id, name, oldKey, isActive);
        }

        var keptApartId = Guid.NewGuid();
        await database.ExecuteAsync(
            "INSERT INTO [Counterparties] ([Id], [UserId], [Name], [NameKey], [Note], [IsActive]) " +
            "VALUES ({0}, {1}, {2}, {3}, NULL, 0)",
            keptApartId, user.Id, "ÖRNEK ELEKTRİK", $"örnek elektrik#{keptApartId:D}");

        await database.MigrateToLatestAsync();

        await using var read = database.CreateContext();

        // Hesaplar: hiçbiri kaybolmadı, adlar değişmedi.
        var accountRows = await read.Accounts.AsNoTracking()
            .Where(row => row.UserId == user.Id)
            .ToDictionaryAsync(row => row.Id, CancellationToken.None);
        Assert.Equal(accounts.Length, accountRows.Count);
        Assert.All(accounts, item => Assert.Equal(item.Name, accountRows[accountIds[item.Name]].Name));

        // Çakışmayan her ad için SQL'in yazdığı, uygulamanın hesapladığıdır.
        foreach (var name in new[] { "Kasa 1", "Is Bankasi", "İş Bankası Şahsi", "---", "***" })
        {
            Assert.Equal(NameKeys.Of(name), accountRows[accountIds[name]].NameKey);
        }

        Assert.Equal("kasa1", accountRows[accountIds["Kasa 1"]].NameKey);
        Assert.Equal("---", accountRows[accountIds["---"]].NameKey);

        // Dört yazım: aktif olan adı tutar, üçü kimliğiyle ayrılır.
        const string bankKey = "işbankasi";
        Assert.Equal(bankKey, accountRows[accountIds["İş Bankası"]].NameKey);
        foreach (var name in new[] { "İŞ BANKASI", "işbankası", "İş-Bankası." })
        {
            Assert.Equal(NameKeys.Apart(name, accountIds[name]), accountRows[accountIds[name]].NameKey);
            Assert.Equal($"{bankKey}#{accountIds[name]:D}", accountRows[accountIds[name]].NameKey);
        }

        var neighbourAccount = await read.Accounts.AsNoTracking()
            .SingleAsync(row => row.Id == neighbourAccountId, CancellationToken.None);
        Assert.Equal(bankKey, neighbourAccount.NameKey);

        // Kategoriler: gelir ve gider "Diğer" aynı anahtarı taşır (tür ayırır).
        var categoryRows = await read.Categories.AsNoTracking()
            .Where(row => row.UserId == user.Id)
            .ToDictionaryAsync(row => row.Id, CancellationToken.None);
        Assert.Equal(categories.Length, categoryRows.Count);
        Assert.Equal("diğer", categoryRows[categoryIds["expense-diger"]].NameKey);
        Assert.Equal("diğer", categoryRows[categoryIds["income-diger"]].NameKey);
        Assert.Equal(
            $"diğer#{categoryIds["expense-DIGER"]:D}",
            categoryRows[categoryIds["expense-DIGER"]].NameKey);
        Assert.Equal("satişgeliri", categoryRows[categoryIds["income-satis"]].NameKey);

        // Kartlar.
        var cardRows = await read.CreditCards.AsNoTracking()
            .Where(row => row.UserId == user.Id)
            .ToDictionaryAsync(row => row.Id, CancellationToken.None);
        Assert.Equal("bonuskart", cardRows[cardIds["Bonus Kart"]].NameKey);
        Assert.Equal($"bonuskart#{cardIds["BonusKart"]:D}", cardRows[cardIds["BonusKart"]].NameKey);
        Assert.Equal("maximum", cardRows[cardIds["Maximum"]].NameKey);

        // POS: birebir aynı adlı iki POS'tan aktif olan adı tutar.
        var posRows = await read.PosDefinitions.AsNoTracking()
            .Where(row => row.UserId == user.Id)
            .ToDictionaryAsync(row => row.Id, CancellationToken.None);
        Assert.Equal(3, posRows.Count);
        Assert.Equal("garantipos", posRows[posIds[0]].NameKey);
        Assert.Equal($"garantipos#{posIds[1]:D}", posRows[posIds[1]].NameKey);
        Assert.Equal("ziraatpos", posRows[posIds[2]].NameKey);

        // Kişiler: anahtar yeni kuralla yeniden hesaplandı. Eski kuralda ayrı
        // olan "Ali Can" ile "Alican" artık aynı addır; eskiden ayrı tutulan
        // kişi yeni anahtarla yine ayrıdır.
        var personRows = await read.Counterparties.AsNoTracking()
            .Where(row => row.UserId == user.Id)
            .ToDictionaryAsync(row => row.Id, CancellationToken.None);
        Assert.Equal(people.Length + 1, personRows.Count);
        Assert.Equal("alican", personRows[personIds["Ali Can"]].NameKey);
        Assert.Equal(
            $"alican#{personIds["Alican"]:D}", personRows[personIds["Alican"]].NameKey);
        Assert.Equal("örnekelektrik", personRows[personIds["Örnek Elektrik"]].NameKey);
        Assert.Equal($"örnekelektrik#{keptApartId:D}", personRows[keptApartId].NameKey);
        Assert.Equal("ahmetbakkal", personRows[personIds["Ahmet Bakkal"]].NameKey);

        // Yükseltmeden sonra denetimler adı tutan kaydı görür ve yenisini açmaz.
        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        Assert.True(await accountRepository.ExistsByNameAsync(
            user.Id, "IŞ BANKASI".Replace('I', 'İ'), CancellationToken.None));
        Assert.True(await accountRepository.ExistsByNameAsync(
            user.Id, "İş - Bankası", CancellationToken.None));
        Assert.False(await accountRepository.ExistsByNameAsync(
            user.Id, "İş Bankası 2", CancellationToken.None));
        var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        Assert.True(await categoryRepository.ExistsByNameAndTypeAsync(
            user.Id, "diğer", CategoryType.Income, CancellationToken.None));
        Assert.False(await categoryRepository.ExistsByNameAndTypeAsync(
            user.Id, "Satış geliri", CategoryType.Expense, CancellationToken.None));
        var cardRepository = scope.ServiceProvider.GetRequiredService<ICreditCardRepository>();
        Assert.True(await cardRepository.ExistsByNameAsync(
            user.Id, "BONUS-KART", null, CancellationToken.None));
        Assert.False(await cardRepository.ExistsByNameAsync(
            user.Id, "bonus kart", cardIds["Bonus Kart"], CancellationToken.None));
        var posRepository = scope.ServiceProvider.GetRequiredService<IPosDefinitionRepository>();
        Assert.True(await posRepository.ExistsByNameAsync(
            user.Id, "garanti pos", null, CancellationToken.None));
        Assert.False(await posRepository.ExistsByNameAsync(
            user.Id, "Garanti POS", posIds[0], CancellationToken.None));
    }

    /// <summary>
    /// Teklik veritabanında da durur: uygulamadaki denetimi atlayan ikinci bir
    /// yazma aynı adı yine açamaz. Başka kullanıcı ve (kategoride) başka tür
    /// aynı adı kullanabilir.
    /// </summary>
    [SqlServerFact]
    public async Task Names_AreUniquePerUserWhateverTheCasingSpacingOrPunctuation()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("name-keys-unique@example.test");
        var neighbour = CreateUser("name-keys-unique-neighbour@example.test");
        await database.SeedUsersAsync(user, neighbour);
        var now = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);
        static Money Limit() => new(1000m, CurrencyCode.TRY);

        var bank = new Account(Guid.NewGuid(), user.Id, "İş Bankası", AccountType.Bank, CurrencyCode.TRY);
        var sales = new Category(Guid.NewGuid(), user.Id, "Satış geliri", CategoryType.Income);
        var other = new Category(Guid.NewGuid(), user.Id, "Diğer", CategoryType.Expense);
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(bank, sales, other);
            seed.Add(new Account(
                Guid.NewGuid(), neighbour.Id, "İŞ BANKASI", AccountType.Bank, CurrencyCode.TRY));
            seed.Add(new Category(Guid.NewGuid(), user.Id, "Diğer", CategoryType.Income));
            seed.Add(new CreditCard(Guid.NewGuid(), user.Id, "Bonus Kart", Limit(), 10, 20));
            seed.Add(new PosDefinition(
                Guid.NewGuid(), user.Id, "Garanti POS", bank, sales, 0m, null, 1, false, now));
            // Hesap ile kart ayrı listelerdir: aynı adı taşıyabilirler.
            seed.Add(new CreditCard(Guid.NewGuid(), user.Id, "İş Bankası", Limit(), 10, 20));
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        var refused = new Func<object>[]
        {
            () => new Account(Guid.NewGuid(), user.Id, "İŞ BANKASI", AccountType.Bank, CurrencyCode.TRY),
            () => new Account(Guid.NewGuid(), user.Id, "işbankası", AccountType.Cash, CurrencyCode.TRY),
            () => new Account(Guid.NewGuid(), user.Id, "İş-Bankası.", AccountType.Bank, CurrencyCode.TRY),
            () => new Category(Guid.NewGuid(), user.Id, "DİĞER", CategoryType.Expense),
            () => new Category(Guid.NewGuid(), user.Id, "satışgeliri", CategoryType.Income),
            () => new CreditCard(Guid.NewGuid(), user.Id, "BonusKart", Limit(), 10, 20),
            () => new CreditCard(Guid.NewGuid(), user.Id, "bonus-kart", Limit(), 10, 20),
        };
        foreach (var create in refused)
        {
            await using var write = database.CreateContext();
            write.Add(create());
            await Assert.ThrowsAsync<DbUpdateException>(
                () => write.SaveChangesAsync(CancellationToken.None));
        }

        await using (var write = database.CreateContext())
        {
            write.Attach(bank);
            write.Attach(sales);
            write.Add(new PosDefinition(
                Guid.NewGuid(), user.Id, "garanti-pos", bank, sales, 0m, null, 1, false, now));
            await Assert.ThrowsAsync<DbUpdateException>(
                () => write.SaveChangesAsync(CancellationToken.None));
        }

        // Harfi ya da eki gerçekten farklı olan ad ayrı kayıttır.
        await using (var write = database.CreateContext())
        {
            write.Add(new Account(Guid.NewGuid(), user.Id, "Is Bankasi", AccountType.Bank, CurrencyCode.TRY));
            write.Add(new Account(Guid.NewGuid(), user.Id, "İş Bankası Şahsi", AccountType.Bank, CurrencyCode.TRY));
            write.Add(new Account(Guid.NewGuid(), user.Id, "İş Bankası 4512", AccountType.Bank, CurrencyCode.TRY));
            write.Add(new Category(Guid.NewGuid(), user.Id, "Satış geliri", CategoryType.Expense));
            write.Add(new CreditCard(Guid.NewGuid(), user.Id, "Bonus Kart 2", Limit(), 10, 20));
            await write.SaveChangesAsync(CancellationToken.None);
        }

        await using var read = database.CreateContext();
        Assert.Equal(4, await read.Accounts.CountAsync(
            row => row.UserId == user.Id, CancellationToken.None));
        Assert.Equal(4, await read.Categories.CountAsync(
            row => row.UserId == user.Id, CancellationToken.None));
        Assert.Equal(3, await read.CreditCards.CountAsync(
            row => row.UserId == user.Id, CancellationToken.None));
        Assert.Equal(1, await read.PosDefinitions.CountAsync(
            row => row.UserId == user.Id, CancellationToken.None));
    }
}
