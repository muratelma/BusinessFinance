using BusinessFinance.Application.FinancePages;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.FinancialActivities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

public sealed partial class SqlServerPersistenceIntegrationTests
{
    [SqlServerFact]
    public async Task CategoryRole_FirstUseInParallelKeepsOneOwnedCategory()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("role-parallel@example.test"); await database.SeedUsersAsync(owner);
        await using var provider = CreateServiceProvider(database.ConnectionString);
        async Task<Guid> Open()
        {
            await using var scope = provider.CreateAsyncScope();
            var category = await scope.ServiceProvider.GetRequiredService<BusinessFinance.Application.Categories.ICategoryRepository>()
                .FindOrOpenRoleAsync(owner.Id, CategoryRole.BankFee, default);
            return category.Id;
        }
        var ids = await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => Open()));
        Assert.Single(ids.Distinct());
        await using var db = database.CreateContext();
        Assert.Equal(1, await db.Categories.CountAsync(c => c.UserId == owner.Id && c.Role == CategoryRole.BankFee));
    }
    [SqlServerFact]
    public async Task CategoryRoles_UpgradePopulatedCategoriesBeforeApplyingConstraints()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString(), "AddDayCloseOverlaps");
        var owner = CreateUser("role-upgrade@example.test");
        await database.SeedUsersAsync(owner);
        var names = new[] { ("Faiz geliri", "faizgeliri", 1), ("Faiz ve finansman gideri", "faizvefinansmangideri", 2),
            ("Kasa farkı", "kasafarki", 2), ("Banka ve POS komisyonu", "bankaveposkomisyonu", 2), ("Özel", "özel", 2) };
        foreach (var (name, key, type) in names)
            await database.ExecuteAsync("INSERT INTO [Categories] ([Id],[UserId],[Name],[NameKey],[Type],[IsActive],[DefaultScope],[IsTax]) VALUES ({0},{1},{2},{3},{4},1,2,0)",
                Guid.NewGuid(), owner.Id, name, key, type);
        await database.MigrateToLatestAsync();
        await using var db = database.CreateContext();
        var categories = await db.Categories.Where(c => c.UserId == owner.Id).ToArrayAsync();
        Assert.Equal(5, categories.Length);
        Assert.Equal(4, categories.Count(c => c.Role != null));
        Assert.All(categories.Where(c => c.Role != null), c => Assert.Null(c.DefaultScope));
        Assert.Equal(TransactionScope.Personal, categories.Single(c => c.Name == "Özel").DefaultScope);
        var role = categories.Single(c => c.Role == CategoryRole.BankFee);
        await Assert.ThrowsAsync<SqlException>(() => database.ExecuteAsync(
            "UPDATE [Categories] SET [DefaultScope] = 1 WHERE [Id] = {0}", role.Id));
        await Assert.ThrowsAsync<SqlException>(() => database.ExecuteAsync(
            "UPDATE [Categories] SET [Role] = 4, [DefaultScope] = NULL WHERE [UserId] = {0} AND [NameKey] = N'özel'", owner.Id));
    }

    [SqlServerFact]
    public async Task FinancePages_UseTheCanonicalSourcesAndKeepEveryTotalExplainable()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("page-source-owner@example.test"); var stranger = CreateUser("page-source-stranger@example.test");
        await database.SeedUsersAsync(owner, stranger);
        var seed = await SeedMoneyFoundationAsync(database, owner.Id);
        var count = new CountingCommandInterceptor();
        await using var provider = CreateServiceProvider(database.ConnectionString, count);
        await using var scope = provider.CreateAsyncScope();
        var reader = scope.ServiceProvider.GetRequiredService<IFinancePageReader>();
        var summary = await reader.CategorySummaryAsync(owner.Id, new(2026, 8, TransactionType.Expense), default);
        Assert.Equal(seed.ExpectedExpense(null), summary.Total);
        Assert.Equal(summary.Total, summary.Categories.Sum(c => c.Amount));
        Assert.Equal(summary.Total, summary.Sources.Where(s => s.Key != "all").Sum(s => s.Amount));
        var category = await reader.CategoryAsync(owner.Id, seed.CommissionId, new(2026, 8, TransactionType.Expense), default);
        Assert.Equal(category!.Summary.Total, category.Items.Sum(i => Math.Abs(i.Amount)));
        Assert.Contains(category.Items, i => i.Activity.ActivityKind == BusinessFinance.Application.FinancialActivities.FinancialActivityKind.CounterpartySettlement);
        foreach (var item in category.Items.Where(i => i.Part is not null))
        {
            var parts = await reader.PartsAsync(owner.Id, item.Activity.ActivityKind, item.Activity.ActivityId, default);
            var part = Assert.Single(parts!.Items);
            Assert.Equal(item.PartAmount, part.Amount);
            Assert.Equal(seed.CommissionId, part.CategoryId);
            Assert.Null(await reader.PartsAsync(stranger.Id, item.Activity.ActivityKind, item.Activity.ActivityId, default));
        }
        var beforeAccount = count.ReaderCommandCount;
        var account = await reader.AccountAsync(owner.Id, seed.BankId, new(2026, 8), default);
        Assert.Equal(seed.BankBalance, account!.CurrentBalance);
        Assert.Equal(account.Inflow, account.Items.Where(i => i.Amount > 0m).Sum(i => i.Amount));
        Assert.Equal(account.Outflow, account.Items.Where(i => i.Amount < 0m).Sum(i => -i.Amount));
        Assert.Equal(account.Inflow - account.Outflow, account.Net);
        Assert.InRange(count.ReaderCommandCount - beforeAccount, 1, 15);
        var transit = await reader.TransitAsync(owner.Id, seed.BankId, 0, default);
        Assert.Equal(transit!.Total, transit.Items.Sum(i => i.Amount));
        Assert.Null(await reader.CategoryAsync(stranger.Id, seed.GoodsId, new(2026, 8, TransactionType.Expense), default));
        Assert.Null(await reader.AccountAsync(stranger.Id, seed.BankId, new(2026, 8), default));
        Assert.Null(await reader.TransitAsync(stranger.Id, seed.BankId, 0, default));
        Assert.Equal(0m, (await reader.CategorySummaryAsync(stranger.Id, new(2026, 8, TransactionType.Expense), default)).Total);
    }

    [SqlServerFact]
    public async Task FinancePages_PaginateInSqlAndKeepInactiveBalancesInTheOverview()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("page-pagination@example.test"); await database.SeedUsersAsync(owner);
        var account = new Account(Guid.NewGuid(), owner.Id, "Pasif", AccountType.Cash, CurrencyCode.TRY, 100m);
        var category = new Category(Guid.NewGuid(), owner.Id, "Gider", CategoryType.Expense);
        await using (var db = database.CreateContext())
        {
            db.AddRange(account, category);
            for (var n = 0; n < 75; n++) db.Add(new BudgetTransaction(Guid.NewGuid(), owner.Id, account, category,
                new Money(0.1234m, CurrencyCode.TRY), TransactionType.Expense, TransactionScope.Personal, new(2026, 8, 1)));
            account.Deactivate(); await db.SaveChangesAsync();
        }
        var count = new CountingCommandInterceptor();
        await using var provider = CreateServiceProvider(database.ConnectionString, count);
        await using var scope = provider.CreateAsyncScope(); var reader = scope.ServiceProvider.GetRequiredService<IFinancePageReader>();
        var first = await reader.CategoryAsync(owner.Id, category.Id, new(2026, 8, TransactionType.Expense), default);
        var second = await reader.CategoryAsync(owner.Id, category.Id, new(2026, 8, TransactionType.Expense, Offset: 50), default);
        Assert.Equal(50, first!.Items.Count); Assert.True(first.HasMore);
        Assert.Equal(25, second!.Items.Count); Assert.False(second.HasMore);
        Assert.Empty(first.Items.Select(i => i.Activity.ActivityId).Intersect(second.Items.Select(i => i.Activity.ActivityId)));
        Assert.Contains(count.CommandTexts, sql => sql.Contains("OFFSET", StringComparison.OrdinalIgnoreCase));
        var overview = await reader.AccountsAsync(owner.Id, default);
        Assert.Equal(100m - 75m * 0.1234m, overview.Total);
        Assert.Equal(overview.Total, overview.InactiveTotal); Assert.Equal(0m, overview.CashTotal);
    }
}
