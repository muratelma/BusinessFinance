using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Application.Cash;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// "Sayımdan sonra bu kasa değişti mi?" sorusu, gerçek SQL Server'da. Fark
/// yalnız güncel sayıma yazılır; bu okuma o kuralın veritabanı tarafıdır.
/// </summary>
public sealed partial class SqlServerPersistenceIntegrationTests
{
    [SqlServerFact]
    public async Task CashCount_SeesEntriesAndNewerCountsOfItsOwnAccountOnly()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("cash-recount-read@example.test");
        await database.SeedUsersAsync(user);
        // Sayımın günü gelecekte olamaz; ikinci sayım bir gün sonraya yazılır.
        var today = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var till = new Account(
            Guid.NewGuid(), user.Id, "Kasa", AccountType.Cash, CurrencyCode.TRY, 1000m);
        var otherTill = new Account(
            Guid.NewGuid(), user.Id, "İkinci kasa", AccountType.Cash, CurrencyCode.TRY, 500m);
        var sales = new Category(
            Guid.NewGuid(), user.Id, "Satış", CategoryType.Income, TransactionScope.Business);
        var before = new BudgetTransaction(Guid.NewGuid(), user.Id, till, sales,
            new Money(50m, CurrencyCode.TRY), TransactionType.Income, TransactionScope.Business, today);
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(till, otherTill, sales, before);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await Task.Delay(20);
        var count = new CashCount(
            Guid.NewGuid(), user.Id, till, 950m, TransactionScope.Business, today,
            DateTimeOffset.UtcNow, expectedAtCount: 1050m);
        await using (var seed = database.CreateContext())
        {
            seed.Add(count);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var services = CreateServiceProvider(database.ConnectionString);

        async Task<bool> ChangedAsync(CashCount target)
        {
            await using var scope = services.CreateAsyncScope();
            return await scope.ServiceProvider.GetRequiredService<ICashCountRepository>()
                .HasAccountChangedSinceAsync(target, CancellationToken.None);
        }

        async Task AddAsync(object entity)
        {
            await Task.Delay(20);
            await using var write = database.CreateContext();
            write.Add(entity);
            await write.SaveChangesAsync(CancellationToken.None);
        }

        // Sayımdan önce girilen kayıt sayımı eskitmez.
        Assert.False(await ChangedAsync(count));

        // Başka kasaya girilen kayıt da eskitmez.
        await AddAsync(new BudgetTransaction(Guid.NewGuid(), user.Id, otherTill, sales,
            new Money(70m, CurrencyCode.TRY), TransactionType.Income, TransactionScope.Business, today));
        Assert.False(await ChangedAsync(count));

        // Günü geçmişte olsa da sayımdan sonra girilen kayıt eskitir.
        var backdated = new BudgetTransaction(Guid.NewGuid(), user.Id, till, sales,
            new Money(30m, CurrencyCode.TRY), TransactionType.Income, TransactionScope.Business, today.AddDays(-20));
        await AddAsync(backdated);
        Assert.True(await ChangedAsync(count));

        // O kayıt iptal edilince yeni satır kalmaz; bakiye de eski hâline
        // döner ve sayım yeniden günceldir.
        await using (var write = database.CreateContext())
        {
            var tracked = await write.Transactions.FindAsync([backdated.Id], CancellationToken.None);
            tracked!.Cancel(DateTimeOffset.UtcNow);
            await write.SaveChangesAsync(CancellationToken.None);
        }

        Assert.False(await ChangedAsync(count));

        // Daha yeni bir sayım eskisini kapatır; yenisi günceldir.
        await Task.Delay(20);
        var newer = new CashCount(
            Guid.NewGuid(), user.Id, till, 1000m, TransactionScope.Business, today.AddDays(1),
            DateTimeOffset.UtcNow, expectedAtCount: 1050m);
        await AddAsync(newer);
        Assert.True(await ChangedAsync(count));
        Assert.False(await ChangedAsync(newer));
    }

    /// <summary>
    /// Sayımın farkı tek kayıtla açıklanır: aktarım bağı veritabanına yazılır,
    /// iptali oradan okunur ve aynı sayıma ikinci bir açıklama (gelir/gider)
    /// veritabanı seviyesinde reddedilir.
    /// </summary>
    [SqlServerFact]
    public async Task CashCount_CarriesOneExplanationAndReadsItsCancellation()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("cash-transfer-link@example.test");
        await database.SeedUsersAsync(user);
        var day = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        var countedAt = DateTimeOffset.UtcNow;

        var till = new Account(
            Guid.NewGuid(), user.Id, "Kasa", AccountType.Cash, CurrencyCode.TRY, 1000m);
        var wallet = new Account(
            Guid.NewGuid(), user.Id, "Cüzdan", AccountType.Cash, CurrencyCode.TRY, 0m);
        var food = new Category(
            Guid.NewGuid(), user.Id, "Market", CategoryType.Expense, TransactionScope.Personal);
        var transfer = new Transfer(
            Guid.NewGuid(), user.Id, till, wallet, new Money(100m, CurrencyCode.TRY), day,
            "Kendime aldım");
        var stray = new BudgetTransaction(Guid.NewGuid(), user.Id, till, food,
            new Money(5m, CurrencyCode.TRY), TransactionType.Expense, TransactionScope.Personal, day);
        var count = new CashCount(
            Guid.NewGuid(), user.Id, till, 900m, TransactionScope.Business, day, countedAt,
            expectedAtCount: 1000m);
        count.RecordTransferAdjustment(transfer.Id, countedAt);
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(till, wallet, food, transfer, stray, count);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var services = CreateServiceProvider(database.ConnectionString);

        async Task<bool> CancelledAsync()
        {
            await using var scope = services.CreateAsyncScope();
            return await scope.ServiceProvider.GetRequiredService<ICashCountRepository>()
                .IsAdjustmentCancelledAsync(count, CancellationToken.None);
        }

        Assert.False(await CancelledAsync());

        await Assert.ThrowsAnyAsync<Exception>(() => database.ExecuteAsync(
            "UPDATE [CashCounts] SET [AdjustmentTransactionId] = {0} WHERE [Id] = {1}",
            stray.Id, count.Id));

        await using (var write = database.CreateContext())
        {
            var tracked = await write.Transfers.FindAsync([transfer.Id], CancellationToken.None);
            tracked!.Cancel(DateTimeOffset.UtcNow);
            await write.SaveChangesAsync(CancellationToken.None);
        }

        Assert.True(await CancelledAsync());
        await using var read = database.CreateContext();
        var stored = await read.CashCounts.FindAsync([count.Id], CancellationToken.None);
        Assert.Equal(transfer.Id, stored!.AdjustmentTransferId);
    }
}
