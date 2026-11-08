using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// Kullanıcı adı alınmadan açılmış hesaplara e-postadan kullanıcı adı yazan
/// migration, gerçek SQL Server'da (kullanıcı kararı, 8 Kasım 2026).
/// </summary>
public sealed partial class SqlServerPersistenceIntegrationTests
{
    /// <summary>
    /// Yükseltme yolu: e-postanın <c>@</c> öncesi küçük harfe çevrilir, kuralda
    /// olmayan karakterler (Türkçe harf, artı) atılır, kısa kalan ada
    /// "kullanici" eklenir, çakışan ada sayı eklenir; tire, nokta ve alt çizgi
    /// kalır; kullanıcı adı zaten olan hesaba dokunulmaz. Yazılan her ad
    /// uygulamanın kuralına uyar. (İlk sürüm tireyi atıyordu; bu test onu
    /// yakalamıyordu.)
    /// </summary>
    [SqlServerFact]
    public async Task BackfillUsernamesFromEmail_DerivesAValidUniqueNameForEveryAccount()
    {
        await using var database = await SqlTestDatabase.CreateAsync(
            GetConnectionString(), "AddCounterpartyDiscountsAndOpeningBalances");

        static ApplicationUser User(string email, int minute, string? username = null)
        {
            var user = new ApplicationUser(
                Guid.NewGuid(),
                email,
                new DateTimeOffset(2026, 8, 8, 10, minute, 0, TimeSpan.Zero),
                username);
            // UserManager doldurur; doğrudan yazarken burada doldurulur.
            user.NormalizedEmail = email.ToUpperInvariant();
            user.NormalizedUserName = user.UserName!.ToUpperInvariant();
            return user;
        }

        var named = User("secilmis@example.test", 0, "murat2");
        var first = User("Murat@example.test", 1);
        var second = User("murat@other.test", 2);
        var dotted = User("Murat.Elma+test@example.test", 3);
        var shortName = User("ab@example.test", 4);
        var turkish = User("Çağrı@example.test", 5);
        var hyphen = User("selin-k@example.test", 6);
        var spaced = User("ali veli_1@example.test", 7);
        await database.SeedUsersAsync(
            named, first, second, dotted, shortName, turkish, hyphen, spaced);

        await database.MigrateToLatestAsync();

        await using var read = database.CreateContext();
        var users = await read.Users.AsNoTracking()
            .ToDictionaryAsync(user => user.Id, CancellationToken.None);

        Assert.Equal("murat2", users[named.Id].UserName);
        Assert.Equal("murat", users[first.Id].UserName);
        // "murat2" zaten seçilmiş: sıradaki sayı.
        Assert.Equal("murat3", users[second.Id].UserName);
        Assert.Equal("murat.elmatest", users[dotted.Id].UserName);
        Assert.Equal("kullaniciab", users[shortName.Id].UserName);
        Assert.Equal("kullaniciar", users[turkish.Id].UserName);
        // Tire, nokta ve alt çizgi kalır; boşluk atılır.
        Assert.Equal("selin-k", users[hyphen.Id].UserName);
        Assert.Equal("aliveli_1", users[spaced.Id].UserName);

        Assert.All(users.Values, user =>
        {
            Assert.True(Usernames.IsValid(user.UserName!), user.UserName);
            Assert.Equal(user.UserName!.ToUpperInvariant(), user.NormalizedUserName);
        });
    }
}
