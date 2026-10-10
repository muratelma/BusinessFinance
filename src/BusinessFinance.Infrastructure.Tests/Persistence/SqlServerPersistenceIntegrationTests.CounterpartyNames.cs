using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// Kişi adının tekliği ve belgedeki adla eşleşmesi, gerçek SQL Server'da.
/// </summary>
/// <remarks>
/// Veritabanının harf kuralı Türkçe İ/i ve I/ı çiftlerini ayrı sayıyor; teklik
/// ona bırakıldığında aynı kişi iki kez açılabiliyordu (8 Ekim 2026'da cihazda
/// görüldü). Buradaki testler kuralın artık veritabanına değil uygulamanın
/// hesapladığı anahtara dayandığını ve eski verinin kaybolmadığını tutar.
/// </remarks>
public sealed partial class SqlServerPersistenceIntegrationTests
{
    /// <summary>
    /// Yükseltme yolu: eski şemada meşru olan aynı adlı iki kişi yükseltmeden
    /// sonra da ikisi birden durur; biri adı tutar, öteki ayrı bir anahtar alır.
    /// SQL'in doldurduğu anahtar uygulamanın hesapladığıyla aynıdır.
    /// </summary>
    [SqlServerFact]
    public async Task AddCounterpartyNameKey_BackfillsTheKeyAndKeepsOldDuplicatesApart()
    {
        await using var database = await SqlTestDatabase.CreateAsync(
            GetConnectionString(), "AddCardCollections");
        var user = CreateUser("name-key-upgrade@example.test");
        var neighbour = CreateUser("name-key-upgrade-neighbour@example.test");
        await database.SeedUsersAsync(user, neighbour);

        var names = new[]
        {
            "ÖRNEK ELEKTRİK DAĞITIM A.Ş.",
            "Örnek Elektrik Dağıtım A.Ş.",
            "IŞIK  Market",
            "IKEA",
            "Çağrı Şükrü Öztürk",
            "Ahmet Bakkal",
        };
        var ids = names.ToDictionary(name => name, _ => Guid.NewGuid());
        foreach (var name in names)
        {
            await InsertLegacyCounterpartyAsync(database, ids[name], user.Id, name, isActive: true);
        }

        // Komşunun aynı adlı kişisi başka kullanıcınındır; çakışma sayılmaz.
        var neighbourId = Guid.NewGuid();
        await InsertLegacyCounterpartyAsync(
            database, neighbourId, neighbour.Id, "örnek elektrik dağıtım a.ş.", isActive: true);

        await database.MigrateToLatestAsync();

        await using var read = database.CreateContext();
        var rows = await read.Counterparties.AsNoTracking()
            .ToDictionaryAsync(row => row.Id, CancellationToken.None);
        Assert.Equal(names.Length + 1, rows.Count);
        Assert.All(names, name => Assert.Equal(name, rows[ids[name]].Name));

        // Çakışmayan her ad için SQL'in yazdığı, uygulamanın hesapladığıdır.
        foreach (var name in names.Skip(2))
        {
            Assert.Equal(Counterparty.NameKeyOf(name), rows[ids[name]].NameKey);
        }

        Assert.Equal("işikmarket", rows[ids["IŞIK  Market"]].NameKey);
        Assert.Equal("ikea", rows[ids["IKEA"]].NameKey);
        Assert.Equal(
            Counterparty.NameKeyOf("örnek elektrik dağıtım a.ş."), rows[neighbourId].NameKey);

        // İki eski kişi: biri adı tutar, öteki kimliğiyle ayrılır.
        var shared = Counterparty.NameKeyOf(names[0]);
        var pair = new[] { rows[ids[names[0]]], rows[ids[names[1]]] };
        var keeper = Assert.Single(pair, row => row.NameKey == shared);
        var apart = Assert.Single(pair, row => row.NameKey != shared);
        Assert.Equal($"{shared}#{apart.Id:D}", apart.NameKey);

        // Adla arama adı tutan kişiyi bulur ve üçüncüsünü açmaz.
        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICounterpartyRepository>();
        var found = await repository.FindOrCreateByNameAsync(
            user.Id, "örnek elektrik dağıtım a.ş.", CancellationToken.None);
        Assert.Equal(keeper.Id, found.Id);
        Assert.True(await repository.ExistsByNameAsync(
            user.Id, "ÖRNEK ELEKTRIK DAĞITIM A.Ş.", null, CancellationToken.None));
    }

    /// <summary>
    /// Teklik veritabanında da durur: uygulamadaki denetimi atlayan ikinci bir
    /// yazma aynı kişiyi yine açamaz. Başka kullanıcı aynı adı kullanabilir.
    /// </summary>
    [SqlServerFact]
    public async Task CounterpartyName_IsUniquePerUserAcrossTurkishCasing()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("name-key-unique@example.test");
        var neighbour = CreateUser("name-key-unique-neighbour@example.test");
        await database.SeedUsersAsync(user, neighbour);

        await using (var seed = database.CreateContext())
        {
            seed.Add(new Counterparty(Guid.NewGuid(), user.Id, "Işık Elektrik"));
            seed.Add(new Counterparty(Guid.NewGuid(), neighbour.Id, "IŞIK ELEKTRİK"));
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        foreach (var sameName in new[] { "IŞIK ELEKTRİK", "ışık elektrik", "IŞIK ELEKTRIK", "Işık   Elektrik" })
        {
            await using var write = database.CreateContext();
            write.Add(new Counterparty(Guid.NewGuid(), user.Id, sameName));
            await Assert.ThrowsAsync<DbUpdateException>(
                () => write.SaveChangesAsync(CancellationToken.None));
        }

        // Harfi gerçekten farklı olan ad ayrı kişidir.
        await using (var write = database.CreateContext())
        {
            write.Add(new Counterparty(Guid.NewGuid(), user.Id, "Isik Elektronik"));
            write.Add(new Counterparty(Guid.NewGuid(), user.Id, "Işık Elektrik 2"));
            await write.SaveChangesAsync(CancellationToken.None);
        }

        await using var read = database.CreateContext();
        Assert.Equal(3, await read.Counterparties.CountAsync(
            row => row.UserId == user.Id, CancellationToken.None));
    }

    /// <summary>
    /// Belgedeki adla eşleşme yalnız kullanıcının kendi <b>aktif</b> kişilerine
    /// bakar: başkasının kişisi ve pasif kişi önerilmez; eşleşme kimseyi açmaz.
    /// </summary>
    [SqlServerFact]
    public async Task ScannedName_MatchesOnlyTheOwnersActivePeopleAndCreatesNobody()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("name-match@example.test");
        var neighbour = CreateUser("name-match-neighbour@example.test");
        await database.SeedUsersAsync(user, neighbour);

        var electric = new Counterparty(Guid.NewGuid(), user.Id, "Örnek Elektrik Dağıtım");
        var retired = new Counterparty(Guid.NewGuid(), user.Id, "Eski Toptancı");
        retired.Deactivate();
        var neighbours = new Counterparty(Guid.NewGuid(), neighbour.Id, "Komşu Su İdaresi");
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(electric, retired, neighbours);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICounterpartyRepository>();

        var match = await repository.FindOwnedByNameAsync(
            user.Id, "ÖRNEK ELEKTRİK DAĞITIM A.Ş.", CancellationToken.None);
        Assert.Equal(electric.Id, match?.Id);

        Assert.Null(await repository.FindOwnedByNameAsync(
            user.Id, "KOMŞU SU İDARESİ", CancellationToken.None));
        Assert.Null(await repository.FindOwnedByNameAsync(
            user.Id, "ESKİ TOPTANCI", CancellationToken.None));
        Assert.Null(await repository.FindOwnedByNameAsync(
            neighbour.Id, "ÖRNEK ELEKTRİK DAĞITIM A.Ş.", CancellationToken.None));
        Assert.Null(await repository.FindOwnedByNameAsync(
            user.Id, "Hiç Tanınmayan Firma", CancellationToken.None));

        await using var read = database.CreateContext();
        Assert.Equal(3, await read.Counterparties.CountAsync(CancellationToken.None));
    }

    private static Task InsertLegacyCounterpartyAsync(
        SqlTestDatabase database,
        Guid id,
        Guid userId,
        string name,
        bool isActive) =>
        database.ExecuteAsync(
            "INSERT INTO [Counterparties] ([Id], [UserId], [Name], [Note], [IsActive]) " +
            "VALUES ({0}, {1}, {2}, NULL, {3})",
            id, userId, name, isActive);
}
