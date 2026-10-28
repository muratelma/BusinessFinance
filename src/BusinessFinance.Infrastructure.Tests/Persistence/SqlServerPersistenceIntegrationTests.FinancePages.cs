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

    /// <summary>
    /// Son kullanılanlar bütün tanıma kaynaklarının birleşimini SQL'de gruplar:
    /// çiftler tekil, en çok dört, türün etkin kategorileri; başka kullanıcı boş.
    /// </summary>
    [SqlServerFact]
    public async Task RecentCategories_GroupEveryRecognizingSourceInSql()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("recent-sql-owner@example.test"); var stranger = CreateUser("recent-sql-stranger@example.test");
        await database.SeedUsersAsync(owner, stranger);
        await SeedMoneyFoundationAsync(database, owner.Id);
        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var reader = scope.ServiceProvider.GetRequiredService<BusinessFinance.Application.Categories.IRecentCategoryReader>();
        await using var db = database.CreateContext();
        foreach (var type in new[] { CategoryType.Expense, CategoryType.Income })
        {
            var recent = await reader.ListAsync(owner.Id, type, 4, default);
            Assert.NotEmpty(recent);
            Assert.InRange(recent.Count, 1, 4);
            Assert.Equal(recent.Count, recent.Distinct().Count());
            var ids = recent.Select(item => item.CategoryId).ToArray();
            Assert.All(await db.Categories.Where(c => ids.Contains(c.Id)).ToArrayAsync(), category =>
            {
                Assert.Equal(owner.Id, category.UserId);
                Assert.Equal(type, category.Type);
                Assert.True(category.IsActive);
            });
            Assert.Empty(await reader.ListAsync(stranger.Id, type, 4, default));
        }
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
        // Altı aylık çubuğun son ayı sayfanın tutarı, bir öncekisi geçen aydır.
        Assert.Equal([(2026, 3), (2026, 4), (2026, 5), (2026, 6), (2026, 7), (2026, 8)],
            summary.RecentMonths!.Select(m => (m.Year, m.Month)));
        Assert.Equal(summary.Total, summary.RecentMonths![^1].Amount);
        Assert.Equal(summary.PreviousTotal, summary.RecentMonths![^2].Amount);
        Assert.Equal(1m, summary.RecentMonths!.Max(m => m.BarRatio));
        Assert.All(summary.RecentMonths!.Where(m => !m.HasData), m => Assert.Equal(0m, m.BarRatio));
        Assert.Equal(summary.PreviousTotal == 0m ? null
            : decimal.Round((summary.Total - summary.PreviousTotal) / summary.PreviousTotal * 100m, 0, MidpointRounding.AwayFromZero),
            summary.PreviousChange);
        var category = await reader.CategoryAsync(owner.Id, seed.CommissionId, new(2026, 8, TransactionType.Expense), default);
        Assert.Equal(category!.Summary.Total, category.Items.Sum(i => Math.Abs(i.Amount)));
        Assert.All(category.Items, item => Assert.NotNull(item.Scope));
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
        var strangerSummary = await reader.CategorySummaryAsync(stranger.Id, new(2026, 8, TransactionType.Expense), default);
        Assert.Equal(0m, strangerSummary.Total);
        Assert.All(strangerSummary.RecentMonths!, m => Assert.False(m.HasData));
        // Kategori sayfasının çubuğu kategoriyi ve seçimi izler; son ayı kartın tutarıdır.
        Assert.Equal(category.Summary.Total, category.Summary.RecentMonths![^1].Amount);
        Assert.Equal(category.Summary.PreviousTotal, category.Summary.RecentMonths![^2].Amount);
        foreach (var (source, person) in new[] { ($"account:{seed.BankId:D}", (string?)null), (null, "none") })
        {
            var narrowed = await reader.CategoryAsync(owner.Id, seed.CommissionId,
                new(2026, 8, TransactionType.Expense, Source: source, Person: person), default);
            Assert.Equal(narrowed!.Summary.Total, narrowed.Summary.RecentMonths![^1].Amount);
            Assert.Equal(narrowed.Summary.PreviousTotal, narrowed.Summary.RecentMonths![^2].Amount);
        }
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
        Assert.Equal(100m, overview.InactiveShare); Assert.Equal(0m, overview.CashShare); Assert.Equal(0m, overview.BankShare);
    }

    [SqlServerFact]
    public async Task FinancePages_FilterKnownAmountsWithoutMovingRecognitionToThePaymentSource()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("page-filter-audit@example.test");
        await database.SeedUsersAsync(owner);
        var seed = await SeedMoneyFoundationAsync(database, owner.Id);
        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var reader = scope.ServiceProvider.GetRequiredService<IFinancePageReader>();
        var goods = await reader.CategoryAsync(owner.Id, seed.GoodsId, new(2026, 8, TransactionType.Expense), default);
        Assert.Equal(1270m, goods!.Summary.Total);
        Assert.Equal(5, goods.Items.Count);
        Assert.Equal(200m, goods.Summary.Sources.Single(s => s.Key == $"account:{seed.BankId:D}").Amount);
        Assert.Equal("bank", goods.Summary.Sources.Single(s => s.Key == $"account:{seed.BankId:D}").Kind);
        Assert.Equal(300m, goods.Summary.Sources.Single(s => s.Key == $"card:{seed.CardId:D}").Amount);
        Assert.Equal(770m, goods.Summary.Sources.Single(s => s.Key == "deferred").Amount);
        Assert.Equal(500m, goods.People.Single(p => p.Key == "none").Amount);
        var supplier = Assert.Single(goods.People, p => p.Key.StartsWith("person:", StringComparison.Ordinal));
        Assert.Equal(770m, supplier.Amount);
        var settled = Assert.Single(goods.Items, i => i.State == "settled");
        Assert.Equal(-120m, settled.Amount);
        Assert.Equal(new DateOnly(2026, 8, 13), settled.SettledOn);
        Assert.Equal("Banka", settled.SettledSource);
        foreach (var (source, amount) in new[] { ($"account:{seed.BankId:D}", 200m), ($"card:{seed.CardId:D}", 300m), ("deferred", 770m) })
        {
            var page = await reader.CategoryAsync(owner.Id, seed.GoodsId, new(2026, 8, TransactionType.Expense, Source: source), default);
            Assert.Equal(amount, page!.Summary.Total);
            Assert.Equal(amount, page.Items.Sum(i => Math.Abs(i.Amount)));
            Assert.Equal(1270m, page.WholeTotal);
        }
        var combined = await reader.CategoryAsync(owner.Id, seed.GoodsId,
            new(2026, 8, TransactionType.Expense, Source: "deferred", Person: supplier.Key), default);
        Assert.Equal(770m, combined!.Summary.Total);
        var zero = await reader.CategoryAsync(owner.Id, seed.GoodsId,
            new(2026, 8, TransactionType.Expense, Source: $"account:{seed.TillId:D}"), default);
        Assert.Equal(0m, zero!.Summary.Total);
        Assert.Empty(zero.Items);
        var selected = zero.Summary.Sources.Single(s => s.Key == $"account:{seed.TillId:D}");
        Assert.Equal("cash", selected.Kind);
        Assert.Equal(0m, selected.Amount);
        foreach (var side in new TransactionScope?[] { null, TransactionScope.Business, TransactionScope.Personal })
        {
            var expense = await reader.CategorySummaryAsync(owner.Id, new(2026, 8, TransactionType.Expense, side), default);
            var income = await reader.CategorySummaryAsync(owner.Id, new(2026, 8, TransactionType.Income, side), default);
            Assert.Equal(seed.ExpectedExpense(side), expense.Total);
            Assert.Equal(seed.ExpectedIncome(side), income.Total);
        }
    }

    [SqlServerFact]
    public async Task FinancePages_KeepTransitAndRecognitionOutOfAccountFlowsAndPreserveMonthBalances()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("page-flow-audit@example.test");
        await database.SeedUsersAsync(owner);
        var seed = await SeedMoneyFoundationAsync(database, owner.Id);
        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var reader = scope.ServiceProvider.GetRequiredService<IFinancePageReader>();
        // Hesaba giren: satış 1000 + krediler 3000 + net yatış 975.
        // Çıkan: gider 200 + taksitler 1750 + transfer 300 + kart 150 + cari 100 + borç 120.
        var page = await reader.AccountAsync(owner.Id, seed.BankId, new(2026, 8), default);
        Assert.Equal(4975m, page!.Inflow);
        Assert.Equal(2620m, page.Outflow);
        Assert.Equal(2355m, page.Net);
        Assert.Equal(12355m, page.Balance);
        Assert.Equal(294m, page.Transit.Total);
        Assert.Equal(2, page.Transit.Count);
        Assert.Equal(98m, page.Transit.Nearest!.Amount); // En erken beklenen kartla tahsil.
        foreach (var direction in new[] { "in", "out" })
        {
            var filtered = await reader.AccountAsync(owner.Id, seed.BankId, new(2026, 8, direction), default);
            Assert.Equal(page.Balance, filtered!.Balance);
            Assert.Equal(page.Inflow, filtered.Inflow);
            Assert.Equal(page.Outflow, filtered.Outflow);
            Assert.Equal(direction == "in" ? 4975m : 2620m, filtered.SelectionTotal);
            Assert.All(filtered.Items, item => Assert.True(direction == "in" ? item.Amount > 0m : item.Amount < 0m));
            // Menü sırası sabit: grup sırası, grubun içinde ad.
            var groups = new[] { "category", "collection", "payment", "transfer" };
            var turkish = StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"), true);
            var menu = filtered.Options.Skip(1).ToArray();
            Assert.Equal(menu.OrderBy(o => Array.IndexOf(groups, o.Group))
                .ThenBy(o => o.Name ?? o.Kind ?? string.Empty, turkish).Select(o => o.Key), menu.Select(o => o.Key));
            foreach (var option in filtered.Options.Where(o => o.Key != "all"))
            {
                var selected = await reader.AccountAsync(owner.Id, seed.BankId, new(2026, 8, direction, option.Key), default);
                Assert.Equal(Math.Abs(option.Amount), selected!.SelectionTotal);
                Assert.Equal(Math.Abs(option.Amount), selected.Items.Sum(i => Math.Abs(i.Amount)));
            }
        }
        var emptyMonth = await reader.AccountAsync(owner.Id, seed.BankId, new(2026, 9), default);
        Assert.Empty(emptyMonth!.Items);
        Assert.Equal(12355m, emptyMonth.Balance);
        Assert.Equal(0m, emptyMonth.Inflow);
        Assert.Equal(0m, emptyMonth.Outflow);
        var months = await reader.AccountMonthsAsync(owner.Id, seed.BankId, 2026, default);
        Assert.Equal(10000m, months!.Months.Single(m => m.Month == 7).Amount);
        Assert.Equal(12355m, months.Months.Single(m => m.Month == 8).Amount);
        Assert.Equal(12355m, months.Months.Single(m => m.Month == 9).Amount);
        // İlk kayıt Ağustos'ta: öncesi seçilebilir ama çubuk çizmez.
        Assert.All(months.Months.Where(m => m.Month < 8), m =>
        {
            Assert.True(m.Enabled);
            Assert.Equal(0m, m.BarRatio);
        });
        Assert.True(months.Months.Single(m => m.Month == 8).BarRatio > 0m);
        Assert.True(months.Months.Single(m => m.Month == 9).BarRatio > 0m);
    }
}
