using System.Data.Common;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Accounts.DeleteAccount;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Application.Reports;
using BusinessFinance.Application.Transfers;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.Debts;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Infrastructure.Categories;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Application.UpcomingPayments;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.Imports;
using BusinessFinance.Infrastructure.Imports;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using System.Text;
using BusinessFinance.Infrastructure.DataPortability;
using BusinessFinance.Application.DataPortability;
using BusinessFinance.Infrastructure.Tests.DataPortability;
using BusinessFinance.Application.SavingsGoals;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Receipts;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

public sealed class SqlServerPersistenceIntegrationTests
{
    private const string ConnectionEnvironmentName =
        "BUSINESS_FINANCE_SQL_TEST_CONNECTION";

    [SqlServerFact]
    public async Task CategoryDefaults_TranslatesLegacyEnglishNamesWithoutChangingCustomNames()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("category-localization@example.test");
        await database.SeedUsersAsync(user);
        await using (var seed = database.CreateContext())
        {
            seed.Categories.AddRange(
                new Category(Guid.NewGuid(), user.Id, "Salary", CategoryType.Income),
                new Category(Guid.NewGuid(), user.Id, "Groceries", CategoryType.Expense),
                new Category(Guid.NewGuid(), user.Id, "Evcil Hayvan", CategoryType.Expense));
            await seed.SaveChangesAsync();
        }

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        await repository.EnsureDefaultsAsync(user.Id, default);

        await using var read = database.CreateContext();
        var names = await read.Categories
            .Where(item => item.UserId == user.Id)
            .Select(item => item.Name)
            .OrderBy(item => item)
            .ToArrayAsync();
        Assert.Equal(["Evcil Hayvan", "Maaş", "Market Alışverişi"], names);
    }

    [SqlServerFact]
    public async Task SavingsGoalDeletion_RemovesOnlyGoalWithoutContributionHistory()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("goal-deletion@example.test");
        await database.SeedUsersAsync(user);
        var createdAt = new DateTimeOffset(2026, 8, 13, 10, 0, 0, TimeSpan.Zero);
        var unused = new SavingsGoal(
            Guid.NewGuid(), user.Id, "Unused goal",
            new Money(1000m, CurrencyCode.TRY), new DateOnly(2027, 8, 13),
            SavingsGoalTrackingMode.ManualContributions, null, createdAt);
        var used = new SavingsGoal(
            Guid.NewGuid(), user.Id, "Used goal",
            new Money(1000m, CurrencyCode.TRY), new DateOnly(2027, 8, 13),
            SavingsGoalTrackingMode.ManualContributions, null, createdAt);
        used.AddContribution(
            Guid.NewGuid(), new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 13), Guid.NewGuid(), createdAt);
        await using (var seed = database.CreateContext())
        {
            seed.SavingsGoals.AddRange(unused, used);
            await seed.SaveChangesAsync();
        }

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISavingsGoalRepository>();

        var unusedResult = await repository.DeleteOwnedIfWithoutContributionsAsync(
            unused.Id, user.Id, default);
        var usedResult = await repository.DeleteOwnedIfWithoutContributionsAsync(
            used.Id, user.Id, default);

        Assert.Equal(SavingsGoalDeletionResult.Deleted, unusedResult);
        Assert.Equal(SavingsGoalDeletionResult.HasContributions, usedResult);
        await using var read = database.CreateContext();
        Assert.False(await read.SavingsGoals.AnyAsync(item => item.Id == unused.Id));
        Assert.True(await read.SavingsGoals.AnyAsync(item => item.Id == used.Id));
        Assert.True(await read.SavingsGoalContributions.AnyAsync(
            item => item.SavingsGoalId == used.Id));
    }

    [SqlServerFact]
    public async Task UnusedAccountDeletion_RemovesOnlyAccountWithoutFinancialReferences()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("account-deletion@example.test");
        await database.SeedUsersAsync(user);
        var unusedAccountId = Guid.NewGuid();
        var usedAccountId = Guid.NewGuid();
        await using (var seed = database.CreateContext())
        {
            var unusedAccount = new Account(
                unusedAccountId, user.Id, "Unused", AccountType.Cash,
                CurrencyCode.TRY, 500m);
            var usedAccount = new Account(
                usedAccountId, user.Id, "Used", AccountType.Bank,
                CurrencyCode.TRY);
            var category = new Category(
                Guid.NewGuid(), user.Id, "Expense", CategoryType.Expense);
            seed.Accounts.AddRange(unusedAccount, usedAccount);
            seed.Categories.Add(category);
            seed.Transactions.Add(new BudgetTransaction(
                Guid.NewGuid(), user.Id, usedAccount, category,
                new Money(10m, CurrencyCode.TRY), TransactionType.Expense,
                TransactionScope.Business,
                new DateOnly(2026, 8, 13)));
            await seed.SaveChangesAsync();
        }

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var deletion = scope.ServiceProvider.GetRequiredService<IUnusedAccountDeletion>();

        var unusedResult = await deletion.DeleteOwnedIfUnusedAsync(
            unusedAccountId, user.Id, default);
        var usedResult = await deletion.DeleteOwnedIfUnusedAsync(
            usedAccountId, user.Id, default);

        Assert.Equal(UnusedAccountDeletionResult.Deleted, unusedResult);
        Assert.Equal(UnusedAccountDeletionResult.InUse, usedResult);
        await using var read = database.CreateContext();
        Assert.False(await read.Accounts.AnyAsync(item => item.Id == unusedAccountId));
        Assert.True(await read.Accounts.AnyAsync(item => item.Id == usedAccountId));
    }

    [SqlServerFact]
    public async Task DebtPayment_ConcurrentWritersAllowOnlyOneAndChangeBalanceOnce()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("debt-concurrency@example.test");
        await database.SeedUsersAsync(user);
        var accountId = Guid.NewGuid();
        var debtId = Guid.NewGuid();
        await using (var seed = database.CreateContext())
        {
            var debtAccount = new Account(
                accountId, user.Id, "Debt Account", AccountType.Bank, CurrencyCode.TRY, 1000m);
            seed.Accounts.Add(debtAccount);
            var lender = new Counterparty(Guid.NewGuid(), user.Id, "Synthetic Lender");
            seed.Counterparties.Add(lender);
            seed.DebtAgreements.Add(new DebtAgreement(
                debtId, user.Id, lender, DebtDirection.Payable,
                TransactionScope.Business,
                new Money(300m, CurrencyCode.TRY), new Money(400m, CurrencyCode.TRY),
                DebtSourceType.Cash, debtAccount, null,
                new DateOnly(2026, 7, 1), new DateOnly(2026, 8, 10), 1));
            await seed.SaveChangesAsync();
        }

        await using var firstContext = database.CreateContext();
        await using var secondContext = database.CreateContext();
        var firstDebt = await firstContext.DebtAgreements.Include(x => x.Installments)
            .SingleAsync(x => x.Id == debtId);
        var secondDebt = await secondContext.DebtAgreements.Include(x => x.Installments)
            .SingleAsync(x => x.Id == debtId);
        var firstAccount = await firstContext.Accounts.SingleAsync(x => x.Id == accountId);
        var secondAccount = await secondContext.Accounts.SingleAsync(x => x.Id == accountId);
        var now = new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero);
        firstDebt.GetInstallment(1).MarkPaid(firstAccount, new DateOnly(2026, 8, 11), now);
        secondDebt.GetInstallment(1).MarkPaid(secondAccount, new DateOnly(2026, 8, 11), now);

        await firstContext.SaveChangesAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondContext.SaveChangesAsync());

        await using var read = database.CreateContext();
        var installment = await read.DebtInstallments.AsNoTracking().SingleAsync();
        Assert.Equal(accountId, installment.PaymentAccountId);
        var openingBalance = await read.Accounts.Where(x => x.Id == accountId)
            .Select(x => x.OpeningBalance).SingleAsync();
        var paidAmount = await read.DebtInstallments
            .Where(x => x.PaymentAccountId == accountId)
            .SumAsync(x => x.Amount.Amount);
        // 1000 açılış bakiyesi + 300 borç açılışı (nakit kaynaklı borçta para
        // hesaba girer) − 400 ödenen taksit = 900. Açılış hareketi eklenmeden
        // önce burada 600 yazıyordu: 300 hiçbir yerden gelmemiş sayılıyor, ama
        // taksitler yine de hesaptan çıkıyordu.
        var balance = openingBalance + 300m - paidAmount;
        Assert.Equal(900m, balance);

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        Assert.Equal(900m, await accountRepository.CalculateBalanceAsync(accountId, user.Id, default));
        var reportRepository = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var monthly = await reportRepository.GetMonthlyAsync(user.Id, 2026, 8, null, default);

        // Anapara gider değildir — borç azaldı, para azaldı, servet değişmedi.
        // Faiz ise karşılığında hiçbir şey alınmayan gerçek bir maliyettir:
        // 300 anaparaya 400 geri ödeme, yani 100 faiz, ödendiği ay gidere
        // yazılır. Anaparanın 300'ü hiçbir yerde gider olarak görünmez.
        Assert.Equal(100m, monthly.TotalExpense);
        Assert.Equal(900m, Assert.Single(monthly.AccountBalances).Balance);
        var advanced = await reportRepository.GetAdvancedAsync(
            user.Id, 2026, 8, new DateOnly(2026, 8, 11), 2, 30, null, default);
        Assert.Equal(900m, advanced.NetWorth.LiquidAssets);
        Assert.Equal(0m, advanced.NetWorth.PayableDebt);
        Assert.Equal(900m, advanced.NetWorth.NetWorth);
    }

    [SqlServerFact]
    public async Task OutstandingDebt_IsRemainingPrincipal_NotRemainingPayments()
    {
        // Net varlık, doğduğu gün bir borçtan **etkilenmemelidir**: para
        // hesaba girer, aynı tutarda yükümlülük doğar, servet değişmez.
        //
        // Açık borç kalan **ödemelerin** toplamı sayıldığında bu bozuluyordu:
        // 300 anapara / 400 toplam bir kredi çekildiği anda hesap 300 artıyor,
        // borç 400 görünüyor ve net varlık daha ilk gün 100 azalıyordu — oysa
        // hiçbir faiz tahakkuk etmemiş, gider raporu da haklı olarak sıfır
        // diyordu. Bilanço ile gelir tablosu birbirini tutmuyordu.
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("outstanding-principal@example.test");
        await database.SeedUsersAsync(user);
        var accountId = Guid.NewGuid();
        await using (var seed = database.CreateContext())
        {
            var account = new Account(
                accountId, user.Id, "Nakit", AccountType.Cash, CurrencyCode.TRY, 1000m);
            seed.Accounts.Add(account);
            var lender = new Counterparty(Guid.NewGuid(), user.Id, "Lender");
            seed.Counterparties.Add(lender);
            var debt = new DebtAgreement(
                Guid.NewGuid(), user.Id, lender, DebtDirection.Payable,
                TransactionScope.Business,
                new Money(300m, CurrencyCode.TRY), new Money(400m, CurrencyCode.TRY),
                DebtSourceType.Cash, account, null,
                new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 10), 1);
            seed.DebtAgreements.Add(debt);
            await seed.SaveChangesAsync();
        }

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var reportRepository = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var advanced = await reportRepository.GetAdvancedAsync(
            user.Id, 2026, 8, new DateOnly(2026, 8, 5), 2, 30, null, default);

        // 1000 açılış + 300 borç açılışı; taksit henüz ödenmedi.
        Assert.Equal(1300m, advanced.NetWorth.LiquidAssets);
        // Kalan anapara 300; 400 olsaydı gelecekteki 100 faiz bugünden
        // düşülmüş olurdu.
        Assert.Equal(300m, advanced.NetWorth.PayableDebt);
        // Servet değişmedi: borç almak kimseyi zenginleştirmez de
        // fakirleştirmez de.
        Assert.Equal(1000m, advanced.NetWorth.NetWorth);
        // Gelir tablosu da aynı şeyi söylüyor: henüz gider yok.
        Assert.Equal(0m, (await reportRepository.GetMonthlyAsync(user.Id, 2026, 8, null, default)).TotalExpense);
    }

    [SqlServerFact]
    public async Task LegacyInstallmentWithoutSplit_ContributesNoInterest()
    {
        // Anapara/faiz kolonlarından önce yazılmış taksitler `null` ayrımla
        // materialize olur; Domain'de o durumu üretecek bir yol yok. Uydurma
        // bir faiz yazmak gider raporunu bozardı, o yüzden sıfır katarlar.
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("legacy-split@example.test");
        await database.SeedUsersAsync(user);
        var accountId = Guid.NewGuid();
        var debtId = Guid.NewGuid();
        await using (var seed = database.CreateContext())
        {
            var account = new Account(
                accountId, user.Id, "Nakit", AccountType.Cash, CurrencyCode.TRY, 1000m);
            seed.Accounts.Add(account);
            var lender = new Counterparty(Guid.NewGuid(), user.Id, "Lender");
            seed.Counterparties.Add(lender);
            var debt = new DebtAgreement(
                debtId, user.Id, lender, DebtDirection.Payable,
                TransactionScope.Business,
                new Money(300m, CurrencyCode.TRY), new Money(400m, CurrencyCode.TRY),
                DebtSourceType.Cash, account, null,
                new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 10), 1);
            debt.GetInstallment(1).MarkPaid(
                account, new DateOnly(2026, 8, 11),
                new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero));
            seed.DebtAgreements.Add(debt);
            await seed.SaveChangesAsync();
        }

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var reportRepository = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();

        // Ayrım doluyken faiz gidere giriyor.
        Assert.Equal(100m, (await reportRepository.GetMonthlyAsync(user.Id, 2026, 8, null, default)).TotalExpense);

        // Aynı satırın ayrımı eski kayıtlardaki gibi boşaltılınca faiz düşüyor,
        // bakiye ve kalan borç etkilenmiyor.
        await using (var legacy = database.CreateContext())
        {
            await legacy.Database.ExecuteSqlRawAsync(
                "UPDATE DebtInstallments SET PrincipalPortion = NULL, InterestPortion = NULL");
        }

        await using var afterScope = services.CreateAsyncScope();
        var afterRepository = afterScope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var monthly = await afterRepository.GetMonthlyAsync(user.Id, 2026, 8, null, default);
        Assert.Equal(0m, monthly.TotalExpense);
        Assert.Equal(900m, Assert.Single(monthly.AccountBalances).Balance);
    }

    [SqlServerFact]
    public async Task SavingsGoal_StaleContributionRetryPersistsOneIdempotentRow()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("goal-idempotency@example.test");
        await database.SeedUsersAsync(user);
        var goalId = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero);
        await using (var seed = database.CreateContext())
        {
            seed.SavingsGoals.Add(new SavingsGoal(
                goalId, user.Id, "Emergency fund", new Money(1000m, CurrencyCode.TRY),
                new DateOnly(2026, 12, 31), SavingsGoalTrackingMode.ManualContributions,
                null, createdAt));
            await seed.SaveChangesAsync();
        }

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var firstScope = services.CreateAsyncScope();
        await using var secondScope = services.CreateAsyncScope();
        var firstRepository = firstScope.ServiceProvider.GetRequiredService<ISavingsGoalRepository>();
        var secondRepository = secondScope.ServiceProvider.GetRequiredService<ISavingsGoalRepository>();
        var firstGoal = await firstRepository.FindOwnedByIdAsync(goalId, user.Id, default);
        var secondGoal = await secondRepository.FindOwnedByIdAsync(goalId, user.Id, default);
        var clientRequestId = Guid.NewGuid();
        var firstContribution = firstGoal!.AddContribution(
            Guid.NewGuid(), new Money(250.125m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 11), clientRequestId, createdAt);
        var staleContribution = secondGoal!.AddContribution(
            Guid.NewGuid(), new Money(250.125m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 11), clientRequestId, createdAt);

        var winner = await firstRepository.AddOrGetContributionAsync(firstContribution, default);
        var retryResult = await secondRepository.AddOrGetContributionAsync(staleContribution, default);

        Assert.Equal(winner.Id, retryResult.Id);
        await using var read = database.CreateContext();
        var persisted = await read.SavingsGoalContributions.AsNoTracking()
            .Where(x => x.UserId == user.Id && x.SavingsGoalId == goalId)
            .ToArrayAsync();
        Assert.Equal(250.125m, Assert.Single(persisted).Amount.Amount);
    }

    [SqlServerFact]
    public async Task AttachmentMetadata_CompositeForeignKeyRejectsForeignTransactionOwner()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("attachment-sql-owner@example.test");
        var other = CreateUser("attachment-sql-other@example.test");
        await database.SeedUsersAsync(owner, other);
        var transactionId = Guid.NewGuid();
        await using (var seed = database.CreateContext())
        {
            var account = new Account(
                Guid.NewGuid(), owner.Id, "Attachment Account", AccountType.Bank, CurrencyCode.TRY);
            var category = new Category(
                Guid.NewGuid(), owner.Id, "Attachment Expense", CategoryType.Expense);
            seed.Accounts.Add(account);
            seed.Categories.Add(category);
            seed.Transactions.Add(new BudgetTransaction(
                transactionId, owner.Id, account, category,
                new Money(10m, CurrencyCode.TRY), TransactionType.Expense,
                TransactionScope.Business,
                new DateOnly(2026, 8, 11)));
            await seed.SaveChangesAsync();
        }

        var now = new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero);
        await using var context = database.CreateContext();
        context.FinancialAttachments.Add(new FinancialAttachment(
            Guid.NewGuid(), owner.Id, transactionId, "receipt.png", "image/png", 8,
            new string('a', 64), $"attachments/{owner.Id:N}/{Guid.NewGuid():N}.png", now));
        await context.SaveChangesAsync();
        Assert.Single(await context.FinancialAttachments.AsNoTracking().ToArrayAsync());
        context.ChangeTracker.Clear();

        context.FinancialAttachments.Add(new FinancialAttachment(
            Guid.NewGuid(), other.Id, transactionId, "foreign.png", "image/png", 8,
            new string('b', 64), $"attachments/{other.Id:N}/{Guid.NewGuid():N}.png", now));
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [SqlServerFact]
    public async Task DataPortability_RoundTripAndFailedRestoreAreAtomic()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var attachmentStore = new TestAttachmentObjectStore();
        var attachmentInspector = new TestAttachmentInspector();
        var source = CreateUser("backup-source@example.test");
        var restoredOwner = CreateUser("backup-target@example.test");
        var rollbackOwner = CreateUser("backup-rollback@example.test");
        var occupiedOwner = CreateUser("backup-occupied@example.test");
        await database.SeedUsersAsync(source, restoredOwner, rollbackOwner, occupiedOwner);
        await using (var seed = database.CreateContext())
        {
            await DataPortabilityTests.SeedCompleteGraphAsync(seed, source.Id);
            await DataPortabilityTests.SeedDefaultCategoriesAsync(seed, restoredOwner.Id);
            var account = await seed.Accounts.SingleAsync(x => x.UserId == source.Id && x.IsActive);
            var transaction = await seed.Transactions.FirstAsync(x => x.UserId == source.Id);
            var now = new DateTimeOffset(2026, 8, 8, 10, 0, 0, TimeSpan.Zero);
            var backupLender = new Counterparty(Guid.NewGuid(), source.Id, "Backup lender");
            seed.Counterparties.Add(backupLender);
            var debt = new DebtAgreement(
                Guid.NewGuid(), source.Id, backupLender, DebtDirection.Payable,
                TransactionScope.Business,
                new Money(300m, CurrencyCode.TRY), new Money(400m, CurrencyCode.TRY),
                DebtSourceType.Cash, account, null,
                new DateOnly(2026, 7, 1), new DateOnly(2026, 8, 10), 1);
            debt.GetInstallment(1).MarkPaid(account, new DateOnly(2026, 8, 11), now);
            seed.DebtAgreements.Add(debt);
            var goal = new SavingsGoal(
                Guid.NewGuid(), source.Id, "Backup goal", new Money(1000m, CurrencyCode.TRY),
                new DateOnly(2026, 12, 31), SavingsGoalTrackingMode.ManualContributions,
                null, now);
            goal.AddContribution(
                Guid.NewGuid(), new Money(250m, CurrencyCode.TRY),
                new DateOnly(2026, 8, 8), Guid.NewGuid(), now);
            seed.SavingsGoals.Add(goal);
            var bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            var attachmentId = Guid.NewGuid();
            var objectKey = $"attachments/{source.Id:N}/{attachmentId:N}.png";
            var hash = Convert.ToHexStringLower(SHA256.HashData(bytes));
            await attachmentStore.WriteAsync(objectKey, bytes, default);
            seed.FinancialAttachments.Add(new FinancialAttachment(
                attachmentId, source.Id, transaction.Id, "receipt.png", "image/png",
                bytes.Length, hash, objectKey, now));
            await seed.SaveChangesAsync();
        }
        await using (var occupied = database.CreateContext())
        {
            occupied.Accounts.Add(new Account(
                Guid.NewGuid(), occupiedOwner.Id, "Existing", AccountType.Cash, CurrencyCode.TRY));
            await occupied.SaveChangesAsync();
        }

        byte[] backup;
        int entityCount;
        await using (var sourceContext = database.CreateContext())
        {
            var service = new EfDataPortabilityRepository(
                sourceContext, attachmentStore, attachmentInspector);
            var file = await service.CreateBackupAsync(source.Id, default);
            backup = file.Content;
            entityCount = (await service.ValidateBackupAsync(backup, default)).EntityCount;
        }

        await using (var restoreContext = database.CreateContext())
        {
            var service = new EfDataPortabilityRepository(
                restoreContext, attachmentStore, attachmentInspector);
            var invalidButChecksummed = CreateDuplicateCategoryBackup(backup);
            await Assert.ThrowsAsync<DbUpdateException>(() => service.RestoreBackupAsync(
                rollbackOwner.Id, invalidButChecksummed, DateTimeOffset.UtcNow, default));
            Assert.False(await restoreContext.Accounts.AnyAsync(x => x.UserId == rollbackOwner.Id));
            Assert.False(await restoreContext.Categories.AnyAsync(x => x.UserId == rollbackOwner.Id));
            Assert.False(await restoreContext.Transactions.AnyAsync(x => x.UserId == rollbackOwner.Id));

            var summary = await service.RestoreBackupAsync(
                restoredOwner.Id, backup, DateTimeOffset.UtcNow, default);
            Assert.Equal(entityCount, summary.RestoredEntityCount);
            var conflict = await Assert.ThrowsAsync<DataPortabilityException>(() => service.RestoreBackupAsync(
                occupiedOwner.Id, backup, DateTimeOffset.UtcNow, default));
            Assert.Equal("restore.destination_not_empty", conflict.Code);
        }

        await using var read = database.CreateContext();
        Assert.Equal(2, await read.Accounts.CountAsync(x => x.UserId == restoredOwner.Id));
        Assert.Equal(3, await read.Categories.CountAsync(x => x.UserId == restoredOwner.Id));
        Assert.Equal(2, await read.Transactions.CountAsync(x => x.UserId == restoredOwner.Id));
        Assert.Single(await read.ImportBatches.Where(x => x.UserId == restoredOwner.Id).ToArrayAsync());
        Assert.Single(await read.DebtAgreements.Where(x => x.UserId == restoredOwner.Id).ToArrayAsync());
        Assert.Single(await read.SavingsGoals.Where(x => x.UserId == restoredOwner.Id).ToArrayAsync());
        var restoredAttachment = Assert.Single(
            await read.FinancialAttachments.Where(x => x.UserId == restoredOwner.Id).ToArrayAsync());
        await using var restoredContent = await attachmentStore.OpenReadAsync(
            restoredAttachment.ObjectKey, default);
        Assert.NotNull(restoredContent);
        Assert.Equal(8, restoredContent!.Length);
        Assert.Equal(2, await read.Accounts.CountAsync(x => x.UserId == source.Id));

        static byte[] CreateDuplicateCategoryBackup(byte[] original)
        {
            var envelope = JsonNode.Parse(original)!.AsObject();
            var payload = Convert.FromBase64String(envelope["payload"]!.GetValue<string>());
            var snapshot = JsonNode.Parse(payload)!.AsObject();
            var categories = snapshot["categories"]!.AsArray();
            var expenses = categories
                .Where(item => item!["type"]!.GetValue<string>() == "expense")
                .ToArray();
            Assert.True(expenses.Length >= 2);
            expenses[1]!["name"] = expenses[0]!["name"]!.GetValue<string>();
            var changedPayload = Encoding.UTF8.GetBytes(snapshot.ToJsonString());
            envelope["payloadLength"] = changedPayload.Length;
            envelope["payloadSha256"] = Convert.ToHexStringLower(SHA256.HashData(changedPayload));
            envelope["payload"] = Convert.ToBase64String(changedPayload);
            return Encoding.UTF8.GetBytes(envelope.ToJsonString());
        }
    }

    [SqlServerFact]
    public async Task ImportConfirmation_TwoStaleContexts_LeavesOneTransaction()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("import-race@example.test");
        await database.SeedUsersAsync(user);
        var account = new Account(Guid.NewGuid(), user.Id, "Import race", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(Guid.NewGuid(), user.Id, "Import expense", CategoryType.Expense);
        var batch = new ImportBatch(
            Guid.NewGuid(), user.Id, "race.csv", new string('a', 64), 100, "utf-8", ',',
            "Date", "Amount", null, null, "yyyy-MM-dd", '.',
            new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero));
        var row = new ImportRow(
            Guid.NewGuid(), user.Id, batch.Id, 2, "2026-08-11,-10",
            new DateOnly(2026, 8, 11), -10m, CurrencyCode.TRY, null, null, []);
        row.ApplyCorrection(new DateOnly(2026, 8, 11), -10m, "Race", null, account, category);
        batch.AddRow(row);
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(account, category, batch);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var firstContext = database.CreateContext();
        await using var secondContext = database.CreateContext();
        await using var stalePatchContext = database.CreateContext();
        var firstRepository = new EfImportBatchRepository(firstContext);
        var secondRepository = new EfImportBatchRepository(secondContext);
        var stalePatchRepository = new EfImportBatchRepository(stalePatchContext);
        var firstBatch = await firstRepository.FindOwnedByIdAsync(batch.Id, user.Id, true, CancellationToken.None);
        var secondBatch = await secondRepository.FindOwnedByIdAsync(batch.Id, user.Id, true, CancellationToken.None);
        var stalePatchBatch = await stalePatchRepository.FindOwnedByIdAsync(batch.Id, user.Id, true, CancellationToken.None);
        var firstAccount = await firstContext.Accounts.SingleAsync(item => item.Id == account.Id);
        var secondAccount = await secondContext.Accounts.SingleAsync(item => item.Id == account.Id);
        var firstCategory = await firstContext.Categories.SingleAsync(item => item.Id == category.Id);
        var secondCategory = await secondContext.Categories.SingleAsync(item => item.Id == category.Id);
        var stalePatchAccount = await stalePatchContext.Accounts.SingleAsync(item => item.Id == account.Id);
        var stalePatchCategory = await stalePatchContext.Categories.SingleAsync(item => item.Id == category.Id);

        var firstTransaction = PrepareConfirmation(firstBatch!, firstAccount, firstCategory);
        var secondTransaction = PrepareConfirmation(secondBatch!, secondAccount, secondCategory);
        var outcomes = await Task.WhenAll(
            CaptureAsync(() => firstRepository.ConfirmAsync(firstBatch!, [firstTransaction], CancellationToken.None)),
            CaptureAsync(() => secondRepository.ConfirmAsync(secondBatch!, [secondTransaction], CancellationToken.None)));

        Assert.Single(outcomes, exception => exception is null);
        Assert.Single(outcomes, exception => exception is ImportConcurrencyException);
        stalePatchBatch!.Rows.Single().ApplyCorrection(
            new DateOnly(2026, 8, 12), -11m, "Late patch", null,
            stalePatchAccount, stalePatchCategory);
        await Assert.ThrowsAsync<ImportConcurrencyException>(() =>
            stalePatchRepository.UpdateAsync(stalePatchBatch, CancellationToken.None));
        await using var read = database.CreateContext();
        Assert.Single(await read.Transactions.AsNoTracking().ToArrayAsync());
        var persistedRow = await read.ImportRows.AsNoTracking().SingleAsync();
        Assert.Equal(ImportRowStatus.Imported, persistedRow.Status);
        Assert.NotNull(persistedRow.BudgetTransactionId);

        static BudgetTransaction PrepareConfirmation(
            ImportBatch candidateBatch,
            Account candidateAccount,
            Category candidateCategory)
        {
            var candidateRow = candidateBatch.Rows.Single();
            var transaction = candidateRow.CreateTransaction(
                candidateAccount, candidateCategory, TransactionScope.Business, Guid.NewGuid());
            candidateRow.MarkImported(transaction.Id);
            candidateBatch.RecordConfirmation();
            return transaction;
        }

        static async Task<Exception?> CaptureAsync(Func<Task> action)
        {
            try
            {
                await action();
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }

    [SqlServerFact]
    public async Task ImportIdempotency_ConcurrentStageAndOwnerScopedDuplicateMatching()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("import-idempotent@example.test");
        var other = CreateUser("import-idempotent-other@example.test");
        await database.SeedUsersAsync(owner, other);
        var fingerprint = new string('a', ImportBatch.FingerprintLength);

        await using var firstContext = database.CreateContext();
        await using var secondContext = database.CreateContext();
        var firstRepository = new EfImportBatchRepository(firstContext);
        var secondRepository = new EfImportBatchRepository(secondContext);
        var firstCandidate = CreateBatch(owner.Id, fingerprint, "first.csv");
        var secondCandidate = CreateBatch(owner.Id, fingerprint, "retry.csv");
        var staged = await Task.WhenAll(
            firstRepository.AddOrGetExistingAsync(firstCandidate, CancellationToken.None),
            secondRepository.AddOrGetExistingAsync(secondCandidate, CancellationToken.None));
        Assert.Equal(staged[0].Id, staged[1].Id);

        await using (var otherContext = database.CreateContext())
        {
            var otherBatch = await new EfImportBatchRepository(otherContext)
                .AddOrGetExistingAsync(CreateBatch(other.Id, fingerprint, "other.csv"), CancellationToken.None);
            Assert.NotEqual(staged[0].Id, otherBatch.Id);
        }

        var account = new Account(Guid.NewGuid(), owner.Id, "Idempotency", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(Guid.NewGuid(), owner.Id, "Idempotency expense", CategoryType.Expense);
        var existing = new BudgetTransaction(
            Guid.NewGuid(), owner.Id, account, category, new Money(25m, CurrencyCode.TRY),
            TransactionType.Expense, TransactionScope.Business, new DateOnly(2026, 8, 11), "Market   payment");
        var importedBatch = CreateBatch(owner.Id, new string('b', 64), "imported.csv");
        var importedRow = new ImportRow(
            Guid.NewGuid(), owner.Id, importedBatch.Id, 2, "row",
            new DateOnly(2026, 8, 11), -25m, CurrencyCode.TRY,
            "Market payment", "BANK-REF-1", []);
        importedRow.ApplyCorrection(
            new DateOnly(2026, 8, 11), -25m, "Market payment", "BANK-REF-1", account, category);
        importedRow.MarkImported(existing.Id);
        importedBatch.AddRow(importedRow);
        importedBatch.RecordConfirmation();
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(account, category, existing, importedBatch);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var matchContext = database.CreateContext();
        var matchRepository = new EfImportBatchRepository(matchContext);
        var referenceMatch = await matchRepository.FindDuplicateAsync(
            owner.Id, "BANK-REF-1", new DateOnly(2026, 1, 1),
            TransactionType.Income, 1m, null, CancellationToken.None);
        var fallbackMatch = await matchRepository.FindDuplicateAsync(
            owner.Id, null, new DateOnly(2026, 8, 11),
            TransactionType.Expense, 25m, " market payment ", CancellationToken.None);
        var foreignMatch = await matchRepository.FindDuplicateAsync(
            other.Id, "BANK-REF-1", new DateOnly(2026, 8, 11),
            TransactionType.Expense, 25m, "Market payment", CancellationToken.None);
        Assert.Equal(existing.Id, referenceMatch!.TransactionId);
        Assert.Equal(ImportDuplicateReason.BankReference, referenceMatch.Reason);
        Assert.Equal(existing.Id, fallbackMatch!.TransactionId);
        Assert.Equal(ImportDuplicateReason.DateAmountDescription, fallbackMatch.Reason);
        Assert.Null(foreignMatch);

        static ImportBatch CreateBatch(Guid userId, string value, string fileName) => new(
            Guid.NewGuid(), userId, fileName, value, 50, "utf-8", ',',
            "Date", "Amount", null, null, "yyyy-MM-dd", '.',
            new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero));
    }

    [SqlServerFact]
    public async Task AccountRepository_UsesOwnerScopedSqlQueriesAcrossScopes()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var firstUser = CreateUser("owner-one@example.test");
        var secondUser = CreateUser("owner-two@example.test");
        await database.SeedUsersAsync(firstUser, secondUser);

        await using (var provider = CreateServiceProvider(database.ConnectionString))
        {
            await using var scope = provider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            await repository.AddAsync(
                new Account(
                    Guid.NewGuid(),
                    firstUser.Id,
                    "Cash",
                    AccountType.Cash,
                    CurrencyCode.TRY),
                CancellationToken.None);
            await repository.AddAsync(
                new Account(
                    Guid.NewGuid(),
                    secondUser.Id,
                    "Cash",
                    AccountType.Cash,
                    CurrencyCode.TRY),
                CancellationToken.None);
        }

        await using var readProvider = CreateServiceProvider(database.ConnectionString);
        await using var readScope = readProvider.CreateAsyncScope();
        var readRepository = readScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var firstPage = await readRepository.ListAsync(
            firstUser.Id,
            new AccountListCriteria(1, 20, null, null, AccountSortOrder.NameAscending),
            CancellationToken.None);
        var secondUserCannotReadFirstAccount = await readRepository.FindOwnedByIdAsync(
            firstPage.Items.Single().Id,
            secondUser.Id,
            CancellationToken.None);

        Assert.Single(firstPage.Items);
        Assert.Equal(firstUser.Id, firstPage.Items.Single().UserId);
        Assert.Null(secondUserCannotReadFirstAccount);
        Assert.True(await readRepository.ExistsByNameAsync(
            firstUser.Id,
            "cash",
            CancellationToken.None));
    }

    [SqlServerFact]
    public async Task RefreshRepository_PersistsRotationAcrossScopes()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("refresh-owner@example.test");
        await database.SeedUsersAsync(user);
        var createdAt = new DateTimeOffset(2026, 8, 8, 12, 0, 0, TimeSpan.Zero);
        var current = new RefreshSession(
            Guid.NewGuid(),
            user.Id,
            new string('A', 64),
            createdAt,
            createdAt.AddDays(30));

        await using (var provider = CreateServiceProvider(database.ConnectionString))
        {
            await using var scope = provider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRefreshSessionRepository>();
            await repository.AddAsync(current, CancellationToken.None);
        }

        var replacement = new RefreshSession(
            Guid.NewGuid(),
            user.Id,
            new string('B', 64),
            createdAt.AddMinutes(1),
            createdAt.AddDays(30));

        await using (var provider = CreateServiceProvider(database.ConnectionString))
        {
            await using var scope = provider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRefreshSessionRepository>();
            var persistedCurrent = await repository.FindByTokenHashAsync(
                current.TokenHash,
                CancellationToken.None);

            Assert.NotNull(persistedCurrent);
            persistedCurrent.Rotate(replacement.Id, createdAt.AddMinutes(1));
            await repository.RotateAsync(
                persistedCurrent,
                replacement,
                CancellationToken.None);
        }

        await using var readProvider = CreateServiceProvider(database.ConnectionString);
        await using var readScope = readProvider.CreateAsyncScope();
        var readRepository = readScope.ServiceProvider
            .GetRequiredService<IRefreshSessionRepository>();
        var rotated = await readRepository.FindByTokenHashAsync(
            current.TokenHash,
            CancellationToken.None);
        var persistedReplacement = await readRepository.FindByTokenHashAsync(
            replacement.TokenHash,
            CancellationToken.None);

        Assert.Equal(replacement.Id, rotated?.ReplacedBySessionId);
        Assert.True(rotated?.IsRevoked);
        Assert.NotNull(persistedReplacement);
    }

    [SqlServerFact]
    public async Task MonthlyBudgetUniqueConstraint_RejectsDuplicateOwnerCategoryAndPeriod()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("budget-owner@example.test");
        var category = new Category(
            Guid.NewGuid(),
            user.Id,
            "Groceries",
            CategoryType.Expense);
        await database.SeedUsersAsync(user);

        await using var context = database.CreateContext();
        context.Categories.Add(category);
        context.MonthlyBudgets.Add(new MonthlyBudget(
            Guid.NewGuid(),
            user.Id,
            category,
            new Money(5000m, CurrencyCode.TRY),
            TransactionScope.Business,
            2026,
            8));
        await context.SaveChangesAsync(CancellationToken.None);

        context.MonthlyBudgets.Add(new MonthlyBudget(
            Guid.NewGuid(),
            user.Id,
            category,
            new Money(6000m, CurrencyCode.TRY),
            TransactionScope.Business,
            2026,
            8));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            context.SaveChangesAsync(CancellationToken.None));
        var sqlException = Assert.IsType<SqlException>(exception.InnerException);

        Assert.Contains(sqlException.Number, new[] { 2601, 2627 });
    }

    [SqlServerFact]
    public async Task TransferRepository_PersistsOneEventAndMovesBalanceWithoutReportIncomeExpense()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("transfer-owner@example.test");
        await database.SeedUsersAsync(user);
        var source = new Account(
            Guid.NewGuid(), user.Id, "Source", AccountType.Bank, CurrencyCode.TRY, 1000m);
        var destination = new Account(
            Guid.NewGuid(), user.Id, "Destination", AccountType.Bank, CurrencyCode.TRY, 100m);

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var transfers = scope.ServiceProvider.GetRequiredService<ITransferRepository>();
        var reports = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        await accounts.AddAsync(source, CancellationToken.None);
        await accounts.AddAsync(destination, CancellationToken.None);
        await transfers.AddAsync(
            new Transfer(
                Guid.NewGuid(),
                user.Id,
                source,
                destination,
                new Money(250m, CurrencyCode.TRY),
                new DateOnly(2026, 8, 10)),
            CancellationToken.None);

        var sourceBalance = await accounts.CalculateBalanceAsync(
            source.Id, user.Id, CancellationToken.None);
        var destinationBalance = await accounts.CalculateBalanceAsync(
            destination.Id, user.Id, CancellationToken.None);
        var report = await reports.GetMonthlyAsync(user.Id, 2026, 8, null, CancellationToken.None);

        Assert.Equal(750m, sourceBalance);
        Assert.Equal(350m, destinationBalance);
        Assert.Equal(0m, report.TotalIncome);
        Assert.Equal(0m, report.TotalExpense);
        Assert.Equal(0m, report.Net);
        Assert.Single((await transfers.ListAsync(
            user.Id, HistoryWindow.Unbounded, CancellationToken.None)).Items);
    }

    [SqlServerFact]
    public async Task CreditCardRepository_AllowsSameNameAcrossUsersAndRejectsOwnerDuplicate()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var firstUser = CreateUser("card-owner-one@example.test");
        var secondUser = CreateUser("card-owner-two@example.test");
        await database.SeedUsersAsync(firstUser, secondUser);

        await using var context = database.CreateContext();
        context.CreditCards.AddRange(
            new CreditCard(
                Guid.NewGuid(), firstUser.Id, "Main", new Money(10000m, CurrencyCode.TRY), 10, 20),
            new CreditCard(
                Guid.NewGuid(), secondUser.Id, "Main", new Money(12000m, CurrencyCode.TRY), 10, 20));
        await context.SaveChangesAsync(CancellationToken.None);
        context.CreditCards.Add(new CreditCard(
            Guid.NewGuid(), firstUser.Id, "Main", new Money(15000m, CurrencyCode.TRY), 12, 24));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            context.SaveChangesAsync(CancellationToken.None));
        var sqlException = Assert.IsType<SqlException>(exception.InnerException);

        Assert.Contains(sqlException.Number, new[] { 2601, 2627 });
    }

    /// <summary>
    /// A slip already recorded as a transfer, a card payment or a receivable is
    /// found — not only one already recorded as an expense.
    ///
    /// <para>
    /// The lookup used to read the transaction table and nothing else, so three
    /// of the four things a bank slip can become had no protection at all. The
    /// three legs stay the same everywhere (date, amount, name); what changes
    /// is how many shelves are searched, and that follows the intent.
    /// </para>
    /// </summary>
    [SqlServerFact]
    public async Task ReceiptDuplicateLookup_FindsSlipRecordsOnEveryShelf()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("slip-duplicate@example.test");
        var stranger = CreateUser("slip-duplicate-other@example.test");
        await database.SeedUsersAsync(user, stranger);

        var date = new DateOnly(2026, 8, 18);
        var amount = new Money(1250m, CurrencyCode.TRY);
        const string name = "Sentetik Banka";

        var source = new Account(
            Guid.NewGuid(), user.Id, "Bank", AccountType.Bank, CurrencyCode.TRY, 5000m);
        var destination = new Account(
            Guid.NewGuid(), user.Id, "Kasa", AccountType.Cash, CurrencyCode.TRY, 0m);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Card", new Money(9000m, CurrencyCode.TRY), 10, 20);

        await using (var seed = database.CreateContext())
        {
            seed.AddRange(source, destination, card);
            seed.Add(new Transfer(
                Guid.NewGuid(), user.Id, source, destination, amount, date, name));
            seed.Add(new CreditCardPayment(
                Guid.NewGuid(), user.Id, source, card, amount, date, name));
            var borrower = new Counterparty(Guid.NewGuid(), user.Id, name);
            seed.Add(borrower);
            seed.Add(new DebtAgreement(
                Guid.NewGuid(), user.Id, borrower, DebtDirection.Receivable,
                TransactionScope.Business,
                // Owned value objects cannot share one instance: EF tracks each
                // as a separate slot on the row.
                new Money(1250m, CurrencyCode.TRY),
                new Money(1250m, CurrencyCode.TRY),
                DebtSourceType.Cash, source, null,
                date, new DateOnly(2026, 9, 18), 1));
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var lookup = scope.ServiceProvider
            .GetRequiredService<IReceiptDuplicateLookup>();

        var slip = await lookup.FindAsync(
            user.Id, date, amount.Amount, name,
            ReceiptCaptureIntent.BankSlip, CancellationToken.None);
        Assert.NotNull(slip);
        Assert.Equal(ReceiptDuplicateKind.Transfer, slip.Kind);

        // The transfer intent stops at the transfer shelf; it cannot be
        // duplicated by a card payment or a receivable.
        var asTransfer = await lookup.FindAsync(
            user.Id, date, amount.Amount, name,
            ReceiptCaptureIntent.Transfer, CancellationToken.None);
        Assert.NotNull(asTransfer);
        Assert.Equal(ReceiptDuplicateKind.Transfer, asTransfer.Kind);

        // An expense document cannot be duplicated by any of the three: there
        // is no transaction row here at all.
        var asExpense = await lookup.FindAsync(
            user.Id, date, amount.Amount, name,
            ReceiptCaptureIntent.Expense, CancellationToken.None);
        Assert.Null(asExpense);

        // Another user's identical slip is not a duplicate of this one.
        var other = await lookup.FindAsync(
            stranger.Id, date, amount.Amount, name,
            ReceiptCaptureIntent.BankSlip, CancellationToken.None);
        Assert.Null(other);

        // A different name on the same day and amount is not a match: the
        // strict third leg is what keeps the warning trustworthy.
        var renamed = await lookup.FindAsync(
            user.Id, date, amount.Amount, "Başka Banka",
            ReceiptCaptureIntent.BankSlip, CancellationToken.None);
        Assert.Null(renamed);
    }

    /// <summary>
    /// With the transfer gone, the card payment and then the receivable are the
    /// ones reported — each shelf is really searched, not just the first.
    /// </summary>
    [SqlServerFact]
    public async Task ReceiptDuplicateLookup_FallsThroughToPaymentThenReceivable()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("slip-duplicate-fallthrough@example.test");
        await database.SeedUsersAsync(user);

        var date = new DateOnly(2026, 8, 18);
        var amount = new Money(1250m, CurrencyCode.TRY);
        const string name = "Sentetik Banka";

        var account = new Account(
            Guid.NewGuid(), user.Id, "Bank", AccountType.Bank, CurrencyCode.TRY, 5000m);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Card", new Money(9000m, CurrencyCode.TRY), 10, 20);
        var paymentId = Guid.NewGuid();

        await using (var seed = database.CreateContext())
        {
            seed.AddRange(account, card);
            seed.Add(new CreditCardPayment(
                paymentId, user.Id, account, card, amount, date, name));
            var borrower = new Counterparty(Guid.NewGuid(), user.Id, name);
            seed.Add(borrower);
            seed.Add(new DebtAgreement(
                Guid.NewGuid(), user.Id, borrower, DebtDirection.Receivable,
                TransactionScope.Business,
                // Owned value objects cannot share one instance: EF tracks each
                // as a separate slot on the row.
                new Money(1250m, CurrencyCode.TRY),
                new Money(1250m, CurrencyCode.TRY),
                DebtSourceType.Cash, account, null,
                date, new DateOnly(2026, 9, 18), 1));
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var lookup = scope.ServiceProvider
            .GetRequiredService<IReceiptDuplicateLookup>();

        var payment = await lookup.FindAsync(
            user.Id, date, amount.Amount, name,
            ReceiptCaptureIntent.BankSlip, CancellationToken.None);
        Assert.NotNull(payment);
        Assert.Equal(ReceiptDuplicateKind.CardPayment, payment.Kind);

        // Cancelling the payment is the user undoing it; what is left is the
        // receivable, and that is what the next reading must report.
        await using (var context = database.CreateContext())
        {
            var stored = await context.CreditCardPayments.SingleAsync(
                item => item.Id == paymentId);
            stored.Cancel(DateTimeOffset.UtcNow);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var receivable = await lookup.FindAsync(
            user.Id, date, amount.Amount, name,
            ReceiptCaptureIntent.BankSlip, CancellationToken.None);
        Assert.NotNull(receivable);
        Assert.Equal(ReceiptDuplicateKind.Receivable, receivable.Kind);
    }

    [SqlServerFact]
    public async Task CardChargeAndPayment_ProduceDebtAndBalanceWithoutDoubleExpense()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("card-activity@example.test");
        await database.SeedUsersAsync(user);
        var account = new Account(
            Guid.NewGuid(), user.Id, "Bank", AccountType.Bank, CurrencyCode.TRY, 1000m);
        var category = new Category(Guid.NewGuid(), user.Id, "Food", CategoryType.Expense);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Card", new Money(500m, CurrencyCode.TRY), 10, 20);
        var charge = new CreditCardCharge(
            Guid.NewGuid(), user.Id, card, category, new Money(300m, CurrencyCode.TRY),
            TransactionScope.Business,
            new DateOnly(2026, 8, 10));
        var payment = new CreditCardPayment(
            Guid.NewGuid(), user.Id, account, card, new Money(100m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10));

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, category, card, charge, payment);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var cards = scope.ServiceProvider.GetRequiredService<ICreditCardRepository>();
        var reports = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var statements = scope.ServiceProvider.GetRequiredService<ICreditCardStatementRepository>();
        var debt = await cards.CalculateCurrentDebtAsync(card.Id, user.Id, CancellationToken.None);
        var balance = await accounts.CalculateBalanceAsync(account.Id, user.Id, CancellationToken.None);
        var report = await reports.GetMonthlyAsync(user.Id, 2026, 8, null, CancellationToken.None);
        var statementPeriod = CreditCardStatementPeriod.ForClosingMonth(card, 2026, 8);
        var statementActivity = await statements.GetActivityAsync(
            card.Id,
            user.Id,
            statementPeriod,
            new DateOnly(2026, 8, 15),
            CancellationToken.None);
        var statement = CreditCardStatement.Create(
            card,
            2026,
            8,
            new DateOnly(2026, 8, 15),
            statementActivity.PreviousBalance,
            statementActivity.PeriodCharges,
            statementActivity.PaymentsThroughClosing,
            statementActivity.PaymentsAfterClosing);

        Assert.Equal(200m, debt);
        Assert.Equal(900m, balance);
        Assert.Equal(300m, report.TotalExpense);
        Assert.Equal(0m, report.TotalIncome);
        Assert.Equal(300m, Assert.Single(report.CategoryExpenses).Amount);
        Assert.Equal(200m, statement.StatementBalance);
        Assert.Equal(StatementPaymentStatus.Open, statement.PaymentStatus);
    }

    /// <summary>
    /// Ödenen borç faizi gider toplamına **ve** kategori dağılımına girer.
    /// </summary>
    /// <remarks>
    /// Faiz toplama zaten giriyordu ama dağılımda hiç yoktu: ekranda "Gider"
    /// ile kategori listesi açıklamasız biçimde tutmuyordu ve faiz hiçbir
    /// yerde kendi adıyla görünmüyordu. Kalıcı bir hareket üretilmiyor —
    /// taksit ödemesi hesabı tutarın tamamı kadar düşürdüğü için ikinci bir
    /// kayıt aynı parayı iki kez düşerdi.
    /// </remarks>
    [SqlServerFact]
    public async Task DebtInterest_CountsInBothTheExpenseTotalAndTheCategoryBreakdown()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("debt-interest-report@example.test");
        await database.SeedUsersAsync(user);
        var account = new Account(
            Guid.NewGuid(), user.Id, "Bank", AccountType.Bank, CurrencyCode.TRY, 10_000m);
        var interestCategory = new Category(
            Guid.NewGuid(),
            user.Id,
            EfCategoryRepository.InterestExpenseCategoryName,
            CategoryType.Expense);

        // 1.000 anapara, 1.100 toplam: 100 faiz, iki taksitte.
        var bank = new Counterparty(Guid.NewGuid(), user.Id, "Banka");
        var debt = new DebtAgreement(
            Guid.NewGuid(), user.Id, bank, DebtDirection.Payable,
            TransactionScope.Business,
            new Money(1000m, CurrencyCode.TRY), new Money(1100m, CurrencyCode.TRY),
            DebtSourceType.Cash, account, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 10), 2, "Kredi");

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, interestCategory, bank, debt);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var provider = CreateServiceProvider(database.ConnectionString))
        await using (var scope = provider.CreateAsyncScope())
        {
            var debts = scope.ServiceProvider.GetRequiredService<IDebtRepository>();
            var persisted = await debts.FindOwnedByIdAsync(
                debt.Id, user.Id, true, CancellationToken.None);
            persisted!.GetInstallment(1).MarkPaid(
                account,
                new DateOnly(2026, 8, 10),
                new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero));
            await debts.SavePaymentAsync(
                persisted.GetInstallment(1), CancellationToken.None);
        }

        await using var readProvider = CreateServiceProvider(database.ConnectionString);
        await using var readScope = readProvider.CreateAsyncScope();
        var reports = readScope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var report = await reports.GetMonthlyAsync(user.Id, 2026, 8, null, CancellationToken.None);

        var paidInterest = debt.GetInstallment(1).InterestPortion!.Value;
        Assert.True(paidInterest > 0m, "Test kurgusu faiz üretmeli.");
        Assert.Equal(paidInterest, report.TotalExpense);

        // Asıl kapı: toplam ile dağılım birbirini tutuyor ve faizin adı var.
        var row = Assert.Single(report.CategoryExpenses);
        Assert.Equal(EfCategoryRepository.InterestExpenseCategoryName, row.CategoryName);
        Assert.Equal(paidInterest, row.Amount);
        Assert.Equal(report.TotalExpense, report.CategoryExpenses.Sum(item => item.Amount));
    }

    /// <summary>
    /// Kart hareketleri tarih penceresiyle sınırlı okunur.
    /// </summary>
    /// <remarks>
    /// Veri kümesi bilerek tavanın altında: ilk yazımda pencere ve tavan aynı
    /// testteydi ve pencere filtresi silindiğinde test yine geçiyordu, çünkü
    /// tavan zaten en eski satırı kırpıyordu. İki sınırın ayrı testleri var.
    /// </remarks>
    [SqlServerFact]
    public async Task CardActivity_ExcludesChargesOutsideTheWindow()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("card-history-window@example.test");
        await database.SeedUsersAsync(user);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Card", new Money(1_000_000m, CurrencyCode.TRY), 10, 20);
        var category = new Category(Guid.NewGuid(), user.Id, "Shopping", CategoryType.Expense);

        await using (var context = database.CreateContext())
        {
            context.AddRange(card, category);
            context.Add(new CreditCardCharge(
                Guid.NewGuid(), user.Id, card, category,
                new Money(5m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2025, 1, 1), "Eski"));
            context.Add(new CreditCardCharge(
                Guid.NewGuid(), user.Id, card, category,
                new Money(1m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 1), "Yeni"));
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var charges = scope.ServiceProvider.GetRequiredService<ICardChargeRepository>();

        var windowed = await charges.ListAsync(
            card.Id,
            user.Id,
            new HistoryWindow(new DateOnly(2026, 7, 1), null),
            CancellationToken.None);
        var all = await charges.ListAsync(
            card.Id, user.Id, HistoryWindow.Unbounded, CancellationToken.None);

        Assert.Equal("Yeni", Assert.Single(windowed.Items).Description);
        Assert.False(windowed.HasMore);
        Assert.Equal(2, all.Items.Count);
    }

    /// <summary>
    /// Satır tavanı pencere ne kadar geniş olursa olsun geçerlidir ve kırpma
    /// sessizce yapılmaz.
    /// </summary>
    [SqlServerFact]
    public async Task CardActivity_TruncatesAtTheRowCeilingAndSaysSo()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("card-history-ceiling@example.test");
        await database.SeedUsersAsync(user);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Card", new Money(1_000_000m, CurrencyCode.TRY), 10, 20);
        var category = new Category(Guid.NewGuid(), user.Id, "Shopping", CategoryType.Expense);

        await using (var context = database.CreateContext())
        {
            context.AddRange(card, category);
            for (var index = 0; index < HistoryWindow.MaximumRows + 5; index++)
            {
                context.Add(new CreditCardCharge(
                    Guid.NewGuid(), user.Id, card, category,
                    new Money(1m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 1), $"Yeni {index}"));
            }
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var charges = scope.ServiceProvider.GetRequiredService<ICardChargeRepository>();

        // "Tümü" tarih sınırını kaldırır ama tavanı kaldırmaz; aksi hâlde bu
        // seçenek sınırsız sorgu sorununu geri getirirdi.
        var all = await charges.ListAsync(
            card.Id, user.Id, HistoryWindow.Unbounded, CancellationToken.None);

        Assert.Equal(HistoryWindow.MaximumRows, all.Items.Count);
        Assert.True(all.HasMore);
    }

    [SqlServerFact]
    public async Task Transfers_AreBoundedByTheWindow()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("transfer-history-window@example.test");
        await database.SeedUsersAsync(user);
        var source = new Account(
            Guid.NewGuid(), user.Id, "Source", AccountType.Bank, CurrencyCode.TRY, 10_000m);
        var destination = new Account(
            Guid.NewGuid(), user.Id, "Destination", AccountType.Bank, CurrencyCode.TRY, 0m);

        await using (var context = database.CreateContext())
        {
            context.AddRange(source, destination);
            context.Add(new Transfer(
                Guid.NewGuid(), user.Id, source, destination,
                new Money(10m, CurrencyCode.TRY), new DateOnly(2026, 8, 1), "Yeni"));
            context.Add(new Transfer(
                Guid.NewGuid(), user.Id, source, destination,
                new Money(10m, CurrencyCode.TRY), new DateOnly(2025, 1, 1), "Eski"));
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var transfers = scope.ServiceProvider.GetRequiredService<ITransferRepository>();

        var windowed = await transfers.ListAsync(
            user.Id, new HistoryWindow(new DateOnly(2026, 7, 1), null), CancellationToken.None);
        var all = await transfers.ListAsync(
            user.Id, HistoryWindow.Unbounded, CancellationToken.None);

        Assert.Equal("Yeni", Assert.Single(windowed.Items).Description);
        Assert.False(windowed.HasMore);
        Assert.Equal(2, all.Items.Count);
    }

    [SqlServerFact]
    public async Task InstallmentPlan_PersistsScheduleAndRealizesOneChargeAtomically()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("installment-owner@example.test");
        await database.SeedUsersAsync(user);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Card", new Money(1000m, CurrencyCode.TRY), 10, 20);
        var category = new Category(Guid.NewGuid(), user.Id, "Shopping", CategoryType.Expense);
        var requestId = Guid.NewGuid();
        var plan = new InstallmentPlan(
            Guid.NewGuid(), user.Id, card, category, requestId,
            new Money(100m, CurrencyCode.TRY), TransactionScope.Business, 3, new DateOnly(2026, 8, 10), "Laptop");

        await using (var context = database.CreateContext())
        {
            context.AddRange(card, category);
            await context.SaveChangesAsync(CancellationToken.None);
        }
        await using (var provider = CreateServiceProvider(database.ConnectionString))
        await using (var scope = provider.CreateAsyncScope())
        {
            var plans = scope.ServiceProvider.GetRequiredService<IInstallmentPlanRepository>();
            await plans.AddAsync(plan, CancellationToken.None);
        }

        await using (var provider = CreateServiceProvider(database.ConnectionString))
        await using (var scope = provider.CreateAsyncScope())
        {
            var plans = scope.ServiceProvider.GetRequiredService<IInstallmentPlanRepository>();
            var persisted = await plans.FindByClientRequestIdAsync(
                user.Id, requestId, true, CancellationToken.None);
            Assert.NotNull(persisted);
            Assert.Equal([33.3333m, 33.3333m, 33.3334m],
                persisted.Items.OrderBy(item => item.Sequence).Select(item => item.Amount.Amount));
            var item = persisted.GetItem(1);
            var charge = new CreditCardCharge(
                Guid.NewGuid(), user.Id, card, category, item.Amount, TransactionScope.Business, item.ScheduledDate, "Taksit 1/3");
            item.Realize(
                charge.Id,
                new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero));
            await plans.RealizeAsync(item, charge, CancellationToken.None);
        }

        await using var readProvider = CreateServiceProvider(database.ConnectionString);
        await using var readScope = readProvider.CreateAsyncScope();
        var readPlans = readScope.ServiceProvider.GetRequiredService<IInstallmentPlanRepository>();
        var charges = readScope.ServiceProvider.GetRequiredService<ICardChargeRepository>();
        var reports = readScope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var persistedPlan = await readPlans.FindOwnedByIdAsync(
            plan.Id, user.Id, false, CancellationToken.None);
        var activity = await charges.ListAsync(
            card.Id, user.Id, HistoryWindow.Unbounded, CancellationToken.None);
        var report = await reports.GetMonthlyAsync(user.Id, 2026, 8, null, CancellationToken.None);

        Assert.True(persistedPlan!.GetItem(1).IsRealized);
        Assert.False(persistedPlan.GetItem(2).IsRealized);
        Assert.Single(activity.Items);
        Assert.Equal(33.3333m, report.TotalExpense);
    }

    [SqlServerFact]
    public async Task RecurringTransaction_PersistsScheduleAndRejectsCrossUserReferences()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("recurring-owner@example.test");
        var otherUser = CreateUser("recurring-other@example.test");
        await database.SeedUsersAsync(owner, otherUser);
        var account = new Account(
            Guid.NewGuid(), owner.Id, "Bills", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(
            Guid.NewGuid(), owner.Id, "Subscriptions", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(),
            owner.Id,
            account,
            category,
            new Money(249.90m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31),
            monthEndBehavior: MonthEndBehavior.ClampToLastDay,
            description: "Synthetic subscription");

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, category, recurring);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var readContext = database.CreateContext())
        {
            var persisted = await readContext.RecurringTransactions.AsNoTracking().SingleAsync();

            Assert.Equal(owner.Id, persisted.UserId);
            Assert.Equal(new Money(249.90m, CurrencyCode.TRY), persisted.Amount);
            Assert.Equal(new DateOnly(2026, 8, 31), persisted.NextOccurrenceDate);
            Assert.Equal(RecurringTransactionKind.BillPayment, persisted.Kind);
        }

        var invalid = new RecurringTransaction(
            Guid.NewGuid(),
            owner.Id,
            account,
            category,
            new Money(100m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            TransactionScope.Business,
            RecurrenceFrequency.Weekly,
            new DateOnly(2026, 8, 11));
        await using var invalidContext = database.CreateContext();
        invalidContext.RecurringTransactions.Add(invalid);
        invalidContext.Entry(invalid).Property(item => item.UserId).CurrentValue = otherUser.Id;

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            invalidContext.SaveChangesAsync(CancellationToken.None));
        var sqlException = Assert.IsType<SqlException>(exception.InnerException);

        Assert.Equal(547, sqlException.Number);
    }

    [SqlServerFact]
    public async Task RecurringOccurrence_GeneratesOnceAndRealizesOneTransactionAtomically()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("occurrence-owner@example.test");
        var otherUser = CreateUser("occurrence-other@example.test");
        await database.SeedUsersAsync(owner, otherUser);
        var account = new Account(
            Guid.NewGuid(), owner.Id, "Bills", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(
            Guid.NewGuid(), owner.Id, "Rent", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(),
            owner.Id,
            account,
            category,
            new Money(1250m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31),
            description: "Synthetic rent");

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, category, recurring);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        Guid occurrenceId;
        await using (var provider = CreateServiceProvider(database.ConnectionString))
        await using (var scope = provider.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRecurringTransactionRepository>();
            var due = Assert.Single(await repository.ListDueAsync(
                owner.Id, new DateOnly(2026, 8, 31), CancellationToken.None));
            var occurrence = RecurringTransactionOccurrence.Create(
                Guid.NewGuid(), due, due.NextOccurrenceDate!.Value);
            occurrenceId = occurrence.Id;
            due.AdvanceAfter(occurrence.ScheduledDate);

            Assert.True(await repository.TrySaveGeneratedAsync(
                [occurrence], CancellationToken.None));
        }

        await using (var provider = CreateServiceProvider(database.ConnectionString))
        await using (var scope = provider.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRecurringTransactionRepository>();
            Assert.Empty(await repository.ListDueAsync(
                owner.Id, new DateOnly(2026, 8, 31), CancellationToken.None));
            Assert.Empty(await repository.ListOccurrencesAsync(
                otherUser.Id, CancellationToken.None));
            var occurrence = await repository.FindOccurrenceOwnedByIdAsync(
                occurrenceId, owner.Id, true, CancellationToken.None);
            Assert.NotNull(occurrence);
            var transaction = new BudgetTransaction(
                Guid.NewGuid(),
                owner.Id,
                account,
                category,
                occurrence.Amount,
                occurrence.GetTransactionType(),
                TransactionScope.Business,
                occurrence.ScheduledDate,
                occurrence.Description);
            occurrence.RealizeWithTransaction(
                transaction.Id,
                new DateTimeOffset(2026, 8, 31, 12, 0, 0, TimeSpan.Zero));

            var persisted = await repository.RealizeAsync(
                occurrence, transaction, CancellationToken.None);
            Assert.Equal(transaction.Id, persisted.Id);
        }

        await using var readContext = database.CreateContext();
        var persistedOccurrence = await readContext.RecurringTransactionOccurrences
            .AsNoTracking()
            .SingleAsync(CancellationToken.None);
        var persistedTransaction = await readContext.Transactions
            .AsNoTracking()
            .SingleAsync(CancellationToken.None);

        Assert.Equal(RecurringOccurrenceStatus.Realized, persistedOccurrence.Status);
        Assert.Equal(persistedTransaction.Id, persistedOccurrence.BudgetTransactionId);
        Assert.Equal(1250m, persistedTransaction.Amount.Amount);
        Assert.Equal(TransactionType.Expense, persistedTransaction.Type);
    }

    [SqlServerFact]
    public async Task RecurringOccurrence_ConcurrentRealizationKeepsOnlyWinningTransaction()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("occurrence-concurrency@example.test");
        await database.SeedUsersAsync(user);
        var account = new Account(
            Guid.NewGuid(), user.Id, "Concurrency", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(
            Guid.NewGuid(), user.Id, "Rent", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            account,
            category,
            new Money(500m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, category, recurring, occurrence);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var firstProvider = CreateServiceProvider(database.ConnectionString);
        await using var secondProvider = CreateServiceProvider(database.ConnectionString);
        await using var firstScope = firstProvider.CreateAsyncScope();
        await using var secondScope = secondProvider.CreateAsyncScope();
        var firstRepository = firstScope.ServiceProvider.GetRequiredService<IRecurringTransactionRepository>();
        var secondRepository = secondScope.ServiceProvider.GetRequiredService<IRecurringTransactionRepository>();
        var firstOccurrence = await firstRepository.FindOccurrenceOwnedByIdAsync(
            occurrence.Id, user.Id, true, CancellationToken.None);
        var secondOccurrence = await secondRepository.FindOccurrenceOwnedByIdAsync(
            occurrence.Id, user.Id, true, CancellationToken.None);
        Assert.NotNull(firstOccurrence);
        Assert.NotNull(secondOccurrence);
        var firstTransaction = new BudgetTransaction(
            Guid.NewGuid(), user.Id, account, category, occurrence.Amount,
            TransactionType.Expense, TransactionScope.Business, occurrence.ScheduledDate);
        var secondTransaction = new BudgetTransaction(
            Guid.NewGuid(), user.Id, account, category, occurrence.Amount,
            TransactionType.Expense, TransactionScope.Business, occurrence.ScheduledDate);
        var now = new DateTimeOffset(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);
        firstOccurrence.RealizeWithTransaction(firstTransaction.Id, now);
        secondOccurrence.RealizeWithTransaction(secondTransaction.Id, now);

        var winner = await firstRepository.RealizeAsync(
            firstOccurrence, firstTransaction, CancellationToken.None);
        var retryResult = await secondRepository.RealizeAsync(
            secondOccurrence, secondTransaction, CancellationToken.None);

        Assert.Equal(winner.Id, retryResult.Id);
        await using var readContext = database.CreateContext();
        Assert.Single(await readContext.Transactions.AsNoTracking().ToArrayAsync());
        var persistedOccurrence = await readContext.RecurringTransactionOccurrences
            .AsNoTracking()
            .SingleAsync();
        Assert.Equal(winner.Id, persistedOccurrence.BudgetTransactionId);
    }

    [SqlServerFact]
    public async Task UpcomingPayments_CombinesOwnerScopedRecurringInstallmentAndStatementSources()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("upcoming-owner@example.test");
        var other = CreateUser("upcoming-other@example.test");
        await database.SeedUsersAsync(owner, other);
        var account = new Account(
            Guid.NewGuid(), owner.Id, "Bills", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(
            Guid.NewGuid(), owner.Id, "Housing", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(), owner.Id, account, category,
            new Money(125m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 10),
            description: "Internet");
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var card = new CreditCard(
            Guid.NewGuid(), owner.Id, "Main Card", new Money(2000m, CurrencyCode.TRY), 10, 20);
        var charge = new CreditCardCharge(
            Guid.NewGuid(), owner.Id, card, category,
            new Money(300m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 5), "Groceries");
        var installment = new InstallmentPlan(
            Guid.NewGuid(), owner.Id, card, category, Guid.NewGuid(),
            new Money(200m, CurrencyCode.TRY), TransactionScope.Business, 2, new DateOnly(2026, 8, 11), "Desk");

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, category, recurring, occurrence, card, charge, installment);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUpcomingPaymentRepository>();
        var candidates = await repository.ListCandidatesAsync(
            owner.Id,
            new DateOnly(2026, 8, 11),
            new DateOnly(2026, 8, 31),
            CancellationToken.None);
        var otherCandidates = await repository.ListCandidatesAsync(
            other.Id,
            new DateOnly(2026, 8, 11),
            new DateOnly(2026, 8, 31),
            CancellationToken.None);

        Assert.Empty(otherCandidates);
        Assert.Equal(3, candidates.Count);
        Assert.Equal(
            125m,
            Assert.Single(candidates, item =>
                item.SourceType == UpcomingPaymentSourceType.RecurringOccurrence).Amount);
        Assert.Equal(
            100m,
            Assert.Single(candidates, item =>
                item.SourceType == UpcomingPaymentSourceType.Installment).Amount);
        var statement = Assert.Single(candidates, item =>
            item.SourceType == UpcomingPaymentSourceType.CreditCardStatement);
        Assert.Equal(300m, statement.Amount);
        Assert.Equal(new DateOnly(2026, 8, 20), statement.DueDate);
    }

    [SqlServerFact]
    public async Task AdvancedReport_CalculatesFixtureTotalsAndExcludesOtherUser()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("advanced-report-owner@example.test");
        var other = CreateUser("advanced-report-other@example.test");
        await database.SeedUsersAsync(owner, other);
        var primary = new Account(
            Guid.NewGuid(), owner.Id, "Primary", AccountType.Bank, CurrencyCode.TRY, 1000m);
        var reserve = new Account(
            Guid.NewGuid(), owner.Id, "Reserve", AccountType.Bank, CurrencyCode.TRY, 500m);
        var otherAccount = new Account(
            Guid.NewGuid(), other.Id, "Foreign", AccountType.Bank, CurrencyCode.TRY, 9999m);
        var incomeCategory = new Category(
            Guid.NewGuid(), owner.Id, "Salary", CategoryType.Income);
        var expenseCategory = new Category(
            Guid.NewGuid(), owner.Id, "Housing", CategoryType.Expense);
        var card = new CreditCard(
            Guid.NewGuid(), owner.Id, "Main Card", new Money(2000m, CurrencyCode.TRY), 10, 20);
        var julyIncome = new BudgetTransaction(
            Guid.NewGuid(), owner.Id, primary, incomeCategory,
            new Money(2000m, CurrencyCode.TRY), TransactionType.Income,
            TransactionScope.Business,
            new DateOnly(2026, 7, 1));
        var julyExpense = new BudgetTransaction(
            Guid.NewGuid(), owner.Id, primary, expenseCategory,
            new Money(500m, CurrencyCode.TRY), TransactionType.Expense,
            TransactionScope.Business,
            new DateOnly(2026, 7, 2));
        var augustIncome = new BudgetTransaction(
            Guid.NewGuid(), owner.Id, primary, incomeCategory,
            new Money(3000m, CurrencyCode.TRY), TransactionType.Income,
            TransactionScope.Business,
            new DateOnly(2026, 8, 1));
        var augustExpense = new BudgetTransaction(
            Guid.NewGuid(), owner.Id, primary, expenseCategory,
            new Money(800m, CurrencyCode.TRY), TransactionType.Expense,
            TransactionScope.Business,
            new DateOnly(2026, 8, 2));
        var transfer = new Transfer(
            Guid.NewGuid(), owner.Id, primary, reserve,
            new Money(200m, CurrencyCode.TRY), new DateOnly(2026, 8, 3));
        var julyCharge = new CreditCardCharge(
            Guid.NewGuid(), owner.Id, card, expenseCategory,
            new Money(100m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 7, 5));
        var augustCharge = new CreditCardCharge(
            Guid.NewGuid(), owner.Id, card, expenseCategory,
            new Money(300m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 5));
        var payment = new CreditCardPayment(
            Guid.NewGuid(), owner.Id, primary, card,
            new Money(50m, CurrencyCode.TRY), new DateOnly(2026, 8, 8));
        var budget = new MonthlyBudget(
            Guid.NewGuid(), owner.Id, expenseCategory,
            new Money(1000m, CurrencyCode.TRY), TransactionScope.Business, 2026, 8);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(), owner.Id, primary, expenseCategory,
            new Money(125m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 15));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var installment = new InstallmentPlan(
            Guid.NewGuid(), owner.Id, card, expenseCategory, Guid.NewGuid(),
            new Money(200m, CurrencyCode.TRY), TransactionScope.Business, 2, new DateOnly(2026, 8, 11));

        await using (var context = database.CreateContext())
        {
            context.AddRange(
                primary, reserve, otherAccount, incomeCategory, expenseCategory, card,
                julyIncome, julyExpense, augustIncome, augustExpense, transfer,
                julyCharge, augustCharge, payment, budget, recurring, occurrence, installment);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var report = await repository.GetAdvancedAsync(
            owner.Id,
            2026,
            8,
            new DateOnly(2026, 8, 11),
            3,
            30,
            null,
            CancellationToken.None);

        Assert.Equal(5150m, report.NetWorth.LiquidAssets);
        Assert.Equal(350m, report.NetWorth.CreditCardDebt);
        Assert.Equal(4800m, report.NetWorth.NetWorth);
        Assert.Equal(new PeriodTotalsDto(2026, 8, 3000m, 1100m, 1900m),
            report.PeriodComparison.Current);
        Assert.Equal(new PeriodTotalsDto(2026, 7, 2000m, 600m, 1400m),
            report.PeriodComparison.Previous);
        Assert.Equal(1000m, report.PeriodComparison.IncomeChange);
        Assert.Equal(500m, report.PeriodComparison.ExpenseChange);
        Assert.Equal(500m, report.PeriodComparison.NetChange);
        Assert.Equal(
            [
                new CashFlowPointDto(2026, 6, 0m, 0m, 0m),
                new CashFlowPointDto(2026, 7, 2000m, 600m, 1400m),
                new CashFlowPointDto(2026, 8, 3000m, 1100m, 1900m)
            ],
            report.CashFlowTrend);
        var variance = Assert.Single(report.BudgetVariances);
        Assert.Equal(1000m, variance.Limit);
        Assert.Equal(1100m, variance.Spent);
        Assert.Equal(-100m, variance.Remaining);
        Assert.True(variance.IsExceeded);
        Assert.Equal(125m, report.FutureLoad.RecurringAmount);
        Assert.Equal(350m, report.FutureLoad.CreditCardStatementAmount);
        Assert.Equal(100m, report.FutureLoad.InstallmentAmount);
        Assert.Equal(575m, report.FutureLoad.TotalAmount);
        Assert.Equal([4450m, 700m], report.AccountDistribution.Select(item => item.Balance));
        var cardDebt = Assert.Single(report.CardDistribution);
        Assert.Equal(350m, cardDebt.Debt);
        Assert.Equal(1650m, cardDebt.AvailableLimit);
    }

    [SqlServerFact]
    public async Task AdvancedReport_LargeFixtureStaysWithinQueryCountAndTimeBudget()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("advanced-performance@example.test");
        await database.SeedUsersAsync(user);
        var account = new Account(
            Guid.NewGuid(), user.Id, "Performance", AccountType.Bank, CurrencyCode.TRY, 1000m);
        var incomeCategory = new Category(
            Guid.NewGuid(), user.Id, "Income", CategoryType.Income);
        var expenseCategory = new Category(
            Guid.NewGuid(), user.Id, "Expense", CategoryType.Expense);
        var transactions = Enumerable.Range(0, 5000)
            .Select(index =>
            {
                var isIncome = index % 2 == 0;
                var month = index % 3 == 0 ? 7 : 8;
                return new BudgetTransaction(
                    Guid.NewGuid(),
                    user.Id,
                    account,
                    isIncome ? incomeCategory : expenseCategory,
                    new Money(1m, CurrencyCode.TRY),
                    isIncome ? TransactionType.Income : TransactionType.Expense,
                    TransactionScope.Business,
                    new DateOnly(2026, month, (index % 28) + 1));
            })
            .ToArray();
        var cards = Enumerable.Range(1, 20)
            .Select(index => new CreditCard(
                Guid.NewGuid(),
                user.Id,
                $"Card {index:D2}",
                new Money(1000m, CurrencyCode.TRY),
                (index % 18) + 1,
                20))
            .ToArray();
        var charges = cards
            .Select(card => new CreditCardCharge(
                Guid.NewGuid(),
                user.Id,
                card,
                expenseCategory,
                new Money(10m, CurrencyCode.TRY),
                TransactionScope.Business,
                new DateOnly(2026, 8, 5)))
            .ToArray();

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, incomeCategory, expenseCategory);
            context.AddRange(transactions);
            context.AddRange(cards);
            context.AddRange(charges);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var counter = new CountingCommandInterceptor();
        await using var provider = CreateServiceProvider(database.ConnectionString, counter);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var stopwatch = Stopwatch.StartNew();
        var report = await repository.GetAdvancedAsync(
            user.Id,
            2026,
            8,
            new DateOnly(2026, 8, 11),
            6,
            30,
            null,
            CancellationToken.None);
        stopwatch.Stop();

        Assert.Equal(6, report.CashFlowTrend.Count);
        Assert.Equal(20, report.CardDistribution.Count);
        // The budget guards against work that grows with the data: a per-card or per-row
        // query would push this into the hundreds against 20 cards and 5.000 movements.
        // It moved from 30 to 40 when the report started reading the shared planned
        // projection, which adds four fixed queries for card limits and statement
        // windows. Four more against 20 cards is flat cost, not a fan-out.
        // 40 → 44 when debts gained an opening and an interest share: the debt
        // opening and the interest total are each one grouped query per period,
        // and a period is fixed by the request rather than by the data. A
        // per-debt query would put this in the hundreds instead of at 42.
        Assert.InRange(counter.ReaderCommandCount, 1, 44);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(5),
            $"Advanced report took {stopwatch.Elapsed.TotalMilliseconds:N0} ms.");
    }

    /// <summary>
    /// Cari bakiye hareketlerden hesaplanır ve <b>tek sorguda</b> gelir: elli üç
    /// karşı taraf da bir ifadeyle okunur. Kişi başına toplam sorgusu açan bir
    /// uygulama burayı yüzün üzerine çıkarırdı.
    /// </summary>
    [SqlServerFact]
    public async Task CounterpartyBalances_ComeFromOneQueryAndStayInsideTheOwner()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("counterparty-owner@example.test");
        var stranger = CreateUser("counterparty-stranger@example.test");
        await database.SeedUsersAsync(owner, stranger);
        var seeded = await SeedCounterpartyGraphAsync(database, owner.Id);
        var strangerGraph = await SeedCounterpartyGraphAsync(database, stranger.Id);

        var counter = new CountingCommandInterceptor();
        await using var provider = CreateServiceProvider(database.ConnectionString, counter);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICounterpartyRepository>();

        var balances = await repository.ListBalancesAsync(
            owner.Id, CounterpartyBalanceFilter.All, null, CancellationToken.None);

        Assert.Equal(1, counter.ReaderCommandCount);

        // Yabancının birebir aynı grafiği görünmüyor: sayı ikiye katlanmıyor.
        Assert.Equal(53, balances.Count);
        var byId = balances.ToDictionary(item => item.CounterpartyId);

        // Elli düz kayıt: 100(i+1) veresiye satış, 40(i+1) tahsilat.
        var tenth = byId[seeded.RunningIds[9]];
        Assert.Equal(600m, tenth.Receivable);
        Assert.Equal(0m, tenth.Payable);
        Assert.Equal(600m, tenth.Net);
        Assert.False(tenth.IsSettled);

        // İptal edilmiş borçlandırma hiç sayılmaz.
        Assert.Equal(0m, byId[seeded.CancelledId].Receivable);
        Assert.True(byId[seeded.CancelledId].IsSettled);

        // Kapanmış cari listede kalır; iki tarafı da sıfırdır.
        Assert.True(byId[seeded.SettledId].IsSettled);

        // Fazla tahsilat kırpılmaz: taraf eksiye düşer ve gerçek kalır.
        Assert.Equal(-250m, byId[seeded.OverpaidId].Receivable);
        Assert.False(byId[seeded.OverpaidId].IsSettled);

        // Aynı kişi hem alıcı hem satıcı: iki taraf ayrı ayrı durur, net ikisini
        // tek cümleye indirir.
        var both = byId[seeded.BothSidesId];
        Assert.Equal(800m, both.Receivable);
        Assert.Equal(300m, both.Payable);
        Assert.Equal(500m, both.Net);

        // Sıralama veritabanında: açık hesabı en büyük olan başta.
        Assert.Equal(3000m, balances[0].Net);

        var open = await repository.ListBalancesAsync(
            owner.Id, CounterpartyBalanceFilter.Open, null, CancellationToken.None);
        var settled = await repository.ListBalancesAsync(
            owner.Id, CounterpartyBalanceFilter.Settled, null, CancellationToken.None);
        Assert.DoesNotContain(open, item => item.CounterpartyId == seeded.SettledId);
        Assert.Contains(settled, item => item.CounterpartyId == seeded.SettledId);
        Assert.Contains(open, item => item.CounterpartyId == seeded.OverpaidId);
        Assert.Equal(balances.Count, open.Count + settled.Count);

        var inactive = await repository.ListBalancesAsync(
            owner.Id, CounterpartyBalanceFilter.All, false, CancellationToken.None);
        Assert.Equal(seeded.InactiveId, Assert.Single(inactive).CounterpartyId);

        // Sahiplik: yabancının karşı tarafı ne bakiye ne kayıt olarak okunabilir.
        Assert.Null(await repository.FindBalanceAsync(
            strangerGraph.SettledId, owner.Id, CancellationToken.None));
        Assert.Null(await repository.FindOwnedByIdAsync(
            strangerGraph.SettledId, owner.Id, CancellationToken.None));
        var mine = await repository.FindBalanceAsync(
            seeded.BothSidesId, owner.Id, CancellationToken.None);
        Assert.NotNull(mine);
        Assert.Equal(500m, mine.Net);
    }

    private static async Task<CounterpartyGraph> SeedCounterpartyGraphAsync(
        SqlTestDatabase database,
        Guid userId)
    {
        var account = new Account(
            Guid.NewGuid(), userId, "Kasa", AccountType.Cash, CurrencyCode.TRY, 10_000m);
        var income = new Category(Guid.NewGuid(), userId, "Veresiye satış", CategoryType.Income);
        var expense = new Category(Guid.NewGuid(), userId, "Tedarik", CategoryType.Expense);
        var charges = new List<CounterpartyCharge>();
        var payments = new List<CounterpartyPayment>();
        var counterparties = new List<Counterparty>();
        var runningIds = new List<Guid>();

        Counterparty Add(string name)
        {
            var counterparty = new Counterparty(Guid.NewGuid(), userId, name);
            counterparties.Add(counterparty);
            return counterparty;
        }

        void Charge(
            Counterparty counterparty,
            DebtDirection direction,
            decimal amount,
            bool cancelled = false)
        {
            var charge = new CounterpartyCharge(
                Guid.NewGuid(),
                userId,
                counterparty,
                direction == DebtDirection.Receivable ? income : expense,
                direction,
                new Money(amount, CurrencyCode.TRY),
                TransactionScope.Business,
                new DateOnly(2026, 8, 10));
            if (cancelled)
            {
                charge.Cancel(new DateTimeOffset(2026, 8, 11, 9, 0, 0, TimeSpan.Zero));
            }

            charges.Add(charge);
        }

        void Pay(Counterparty counterparty, DebtDirection direction, decimal amount)
        {
            payments.Add(new CounterpartyPayment(
                Guid.NewGuid(),
                userId,
                counterparty,
                account,
                direction,
                new Money(amount, CurrencyCode.TRY),
                new DateOnly(2026, 8, 12)));
        }

        for (var index = 1; index <= 50; index++)
        {
            var counterparty = Add($"Cari {index:D2}");
            runningIds.Add(counterparty.Id);
            Charge(counterparty, DebtDirection.Receivable, 100m * index);
            Pay(counterparty, DebtDirection.Receivable, 40m * index);
        }

        // Kapanmış cari: satış tamamen tahsil edildi, kayıt listede kalır.
        var settledParty = Add("Kapanmış cari");
        Charge(settledParty, DebtDirection.Receivable, 750m);
        Pay(settledParty, DebtDirection.Receivable, 750m);

        // İptal edilmiş borçlandırma: hiç olmamış gibi sayılır.
        var cancelledParty = Add("İptalli cari");
        Charge(cancelledParty, DebtDirection.Receivable, 900m, cancelled: true);

        // Aynı kişi hem alıcı hem satıcı; iki taraf ayrı ayrı durur.
        var bothSides = Add("İki yönlü cari");
        Charge(bothSides, DebtDirection.Receivable, 800m);
        Charge(bothSides, DebtDirection.Payable, 300m);

        // Pasif karşı taraf: geçmişi durur, yeni borçlandırma alamaz.
        counterparties[0].Deactivate();

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, income, expense);
            context.AddRange(counterparties);
            context.AddRange(charges);
            context.AddRange(payments);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // Fazla tahsilat pasifleştirmeden sonra yazılıyor: kalan borcu kapatmak
        // pasif tarafta da mümkün olmalı (Grup 2 kararı) ve fazlası kırpılmıyor.
        await using (var context = database.CreateContext())
        {
            var reloaded = await context.Counterparties.SingleAsync(
                item => item.Id == counterparties[0].Id, CancellationToken.None);
            var reloadedAccount = await context.Accounts.SingleAsync(
                item => item.Id == account.Id, CancellationToken.None);
            context.CounterpartyPayments.Add(new CounterpartyPayment(
                Guid.NewGuid(),
                userId,
                reloaded,
                reloadedAccount,
                DebtDirection.Receivable,
                new Money(310m, CurrencyCode.TRY),
                new DateOnly(2026, 8, 13)));
            await context.SaveChangesAsync(CancellationToken.None);
        }

        return new CounterpartyGraph(
            runningIds,
            settledParty.Id,
            cancelledParty.Id,
            counterparties[0].Id,
            counterparties[0].Id,
            bothSides.Id);
    }

    private sealed record CounterpartyGraph(
        IReadOnlyList<Guid> RunningIds,
        Guid SettledId,
        Guid CancelledId,
        Guid OverpaidId,
        Guid InactiveId,
        Guid BothSidesId);

    /// <summary>
    /// Dolu bir veritabanında yükseltme: sözleşmelerdeki her ad bir karşı taraf
    /// olur, aynı ad iki kez kurulmaz ve hiçbir sözleşme karşı tarafını
    /// kaybetmez.
    /// </summary>
    /// <remarks>
    /// Bu adımın üretilmiş hâli ad kolonunu düşürüp yerine boş bir kimlik
    /// koyuyordu; test o yolun bir daha açılmadığını kanıtlıyor. Şema
    /// <c>AddCounterparties</c> adımında durduruluyor, satırlar o zamanki
    /// şemayla yazılıyor ve yükseltme ondan sonra çalışıyor.
    /// </remarks>
    [SqlServerFact]
    public async Task LinkDebtsToCounterparties_TurnsEveryExistingNameIntoOneOwnedCounterparty()
    {
        await using var database = await SqlTestDatabase.CreateAsync(
            GetConnectionString(), "AddCounterparties");
        var owner = CreateUser("debt-link-owner@example.test");
        var stranger = CreateUser("debt-link-stranger@example.test");
        await database.SeedUsersAsync(owner, stranger);

        // Kullanıcının zaten kayıtlı bir karşı tarafı var: yükseltme onu
        // yeniden kurmamalı, yoksa aynı kişi iki kez yaşardı.
        var existingId = Guid.NewGuid();
        await database.ExecuteAsync(
            "INSERT INTO [Counterparties] ([Id], [UserId], [Name], [Note], [IsActive]) " +
            "VALUES ({0}, {1}, N'Ahmet Bakkal', NULL, 1)",
            existingId, owner.Id);

        var firstDebtId = Guid.NewGuid();
        var secondDebtId = Guid.NewGuid();
        var thirdDebtId = Guid.NewGuid();
        var strangerDebtId = Guid.NewGuid();
        await SeedLegacyDebtAsync(database, firstDebtId, owner.Id, "Ahmet Bakkal");
        await SeedLegacyDebtAsync(database, secondDebtId, owner.Id, "Ahmet Bakkal");
        await SeedLegacyDebtAsync(database, thirdDebtId, owner.Id, "Zeynep Manav");

        // Aynı ad başka kullanıcıda ayrı bir karşı taraftır.
        await SeedLegacyDebtAsync(database, strangerDebtId, stranger.Id, "Ahmet Bakkal");

        await database.MigrateToLatestAsync();

        await using var context = database.CreateContext();
        var counterparties = await context.Counterparties.AsNoTracking().ToArrayAsync();
        Assert.Equal(3, counterparties.Length);
        Assert.Equal(2, counterparties.Count(item => item.UserId == owner.Id));
        Assert.All(counterparties, item => Assert.True(item.IsActive));

        var debts = await context.DebtAgreements.AsNoTracking()
            .ToDictionaryAsync(item => item.Id, item => item.CounterpartyId);

        // Var olan kayıt kullanıldı; iki sözleşme aynı kişiye bağlandı.
        Assert.Equal(existingId, debts[firstDebtId]);
        Assert.Equal(existingId, debts[secondDebtId]);

        // Yeni ad yeni kayıt üretti, ama sahibinin içinde.
        var manav = Assert.Single(counterparties, item => item.Name == "Zeynep Manav");
        Assert.Equal(owner.Id, manav.UserId);
        Assert.Equal(manav.Id, debts[thirdDebtId]);

        // Yabancının aynı adı kendi kaydına gitti.
        Assert.NotEqual(existingId, debts[strangerDebtId]);
        Assert.Equal(
            stranger.Id,
            counterparties.Single(item => item.Id == debts[strangerDebtId]).UserId);
    }

    /// <remarks>
    /// Açılışı kayıtsız (<c>SourceType = 0</c>) bir sözleşme yazılıyor: hesap ya
    /// da kategori bağlamadan geçerli olan tek kaynak bu ve testin sorusu
    /// kaynak değil, ad.
    /// </remarks>
    private static Task SeedLegacyDebtAsync(
        SqlTestDatabase database,
        Guid debtId,
        Guid userId,
        string counterpartyName)
    {
        return database.ExecuteAsync(
            "INSERT INTO [DebtAgreements] " +
            "([Id], [UserId], [CounterpartyName], [Direction], [Scope], [Principal], [Currency], " +
            " [TotalRepayment], [TotalCurrency], [AnnualInterestRate], [SourceType], " +
            " [StartDate], [FirstDueDate], [InstallmentCount], [Description]) " +
            "VALUES ({0}, {1}, {2}, 1, 1, 300, 'TRY', 330, 'TRY', 0, 0, " +
            " '2026-08-01', '2026-08-15', 3, NULL)",
            debtId, userId, counterpartyName);
    }

    /// <summary>
    /// Aynı karşı tarafın iki kaynağı birbirini toplamaz: taksitli sözleşme net
    /// varlığa anaparasıyla girer, açık cari kendi bakiyesinde durur ve hiçbir
    /// tutar iki kez sayılmaz.
    /// </summary>
    /// <remarks>
    /// İki kaynak artık tek kişide buluştuğu için karışma riski bu adımda
    /// doğdu: sözleşme adını karşı taraftan okuyor, cari hareket de aynı
    /// karşı tarafa yazılıyor. Cari bakiyenin net varlığa katılması Grup 5'in
    /// işi; bugün oraya girmediği için test onu net varlıkta <b>aramıyor</b>,
    /// borcun tutarını şişirmediğini arıyor.
    /// </remarks>
    [SqlServerFact]
    public async Task CounterpartyWithBothLedgers_CountsEachAmountOnce()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("counterparty-double-count@example.test");
        await database.SeedUsersAsync(user);
        var account = new Account(
            Guid.NewGuid(), user.Id, "Kasa", AccountType.Cash, CurrencyCode.TRY, 1000m);
        var supplier = new Counterparty(Guid.NewGuid(), user.Id, "Toptancı Ahmet");
        var supplies = new Category(Guid.NewGuid(), user.Id, "Mal alımı", CategoryType.Expense);

        // Vadeli alım: gider bugün tanınır, kasa kıpırdamaz.
        var charge = new CounterpartyCharge(
            Guid.NewGuid(), user.Id, supplier, supplies, DebtDirection.Payable,
            new Money(500m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 5));
        var payment = new CounterpartyPayment(
            Guid.NewGuid(), user.Id, supplier, account, DebtDirection.Payable,
            new Money(200m, CurrencyCode.TRY), new DateOnly(2026, 8, 6));

        // Aynı kişiyle taksitli bir sözleşme: 600 anapara, iki taksit.
        var debt = new DebtAgreement(
            Guid.NewGuid(), user.Id, supplier, DebtDirection.Payable,
            TransactionScope.Business,
            new Money(600m, CurrencyCode.TRY), new Money(600m, CurrencyCode.TRY),
            DebtSourceType.Cash, account, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 20), 2);

        await using (var seed = database.CreateContext())
        {
            seed.AddRange(account, supplier, supplies, charge, payment, debt);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var counterparties = scope.ServiceProvider.GetRequiredService<ICounterpartyRepository>();
        var reports = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();

        var balance = await counterparties.FindBalanceAsync(
            supplier.Id, user.Id, CancellationToken.None);
        var report = await reports.GetAdvancedAsync(
            user.Id, 2026, 8, new DateOnly(2026, 8, 10), 1, 30, null, CancellationToken.None);

        // Cari bakiye yalnız cari hareketlerden: 500 − 200. Sözleşmenin 600'ü
        // buraya karışmıyor.
        Assert.NotNull(balance);
        Assert.Equal(300m, balance.Payable);
        Assert.Equal(0m, balance.Receivable);

        // Net varlık yalnız sözleşmenin kalan anaparasından: 600. Cari borç
        // buraya iki kez eklenmiyor.
        Assert.Equal(600m, report.NetWorth.PayableDebt);
        Assert.Equal(0m, report.NetWorth.ReceivableDebt);

        // Cari borçlandırma gelir/gider raporuna **henüz** girmiyor: birleşik
        // feed ve raporlara katılması Grup 5'in işi. Satır bugünün gerçeğini
        // yazıyor ve o grup geldiğinde 500'e dönerek kendini hatırlatacak.
        Assert.Equal(0m, report.PeriodComparison.Current.Expense);
    }

    private static string GetConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            ConnectionEnvironmentName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"{ConnectionEnvironmentName} is required for SQL Server integration tests.");
        }

        return connectionString;
    }

    /// <summary>
    /// Every planned source reaches the planned view once, carrying readiness derived
    /// from the current state of its source. The duplicate case matters most: a schedule
    /// whose next date already has an occurrence must not also project that same date.
    /// </summary>
    [SqlServerFact]
    public async Task PlannedActivities_ProjectEveryKindWithReadinessAndNoDuplicates()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("planned-owner@example.test");
        var stranger = CreateUser("planned-stranger@example.test");
        await database.SeedUsersAsync(owner, stranger);
        var seeded = await SeedPlannedGraphAsync(database, owner.Id);
        await SeedPlannedGraphAsync(database, stranger.Id);

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPlannedActivityRepository>();

        var items = await repository.ListAsync(
            owner.Id, PlannedAsOfDate, PlannedAsOfDate.AddDays(30), null, CancellationToken.None);
        var byId = items.ToLookup(item => item.PlannedActivityId);

        // Owner isolation: the stranger's identical graph must not appear. Nine because
        // both installments of the two-part plan fall inside the 30 day horizon.
        Assert.Equal(9, items.Count);
        Assert.Equal(2, items.Count(item =>
            item.PlannedKind == PlannedActivityKind.CardInstallment));

        // The schedule behind this occurrence still points at the same date. It must be
        // reported once, from the occurrence, not also as a projected date.
        var income = Assert.Single(byId[seeded.IncomeOccurrenceId]);
        Assert.Equal(PlannedActivityKind.RecurringOccurrence, income.PlannedKind);
        Assert.Equal(FinancialActivityEffect.Income, income.Effect);
        Assert.Equal(PlannedActivityAction.Realize, income.ActionKind);
        Assert.Equal(PlannedActivityReadiness.Ready, income.Readiness);
        Assert.False(income.IsProjected);
        Assert.DoesNotContain(items, item =>
            item.IsProjected && item.DueDate == income.DueDate);

        var overLimit = Assert.Single(byId[seeded.CardOccurrenceId]);
        Assert.Equal(PlannedActivityReadiness.NeedsAttention, overLimit.Readiness);
        Assert.Equal(PlannedActivityAttention.CardLimitInsufficient, overLimit.AttentionCode);

        var closedAccount = Assert.Single(byId[seeded.ClosedAccountOccurrenceId]);
        Assert.Equal(PlannedActivityReadiness.NeedsAttention, closedAccount.Readiness);
        Assert.Equal(PlannedActivityAttention.AccountInactive, closedAccount.AttentionCode);

        // A schedule with no occurrence yet is reported against the schedule id.
        // Same rule on the planned side: several subscriptions filed under one
        // category must not all read as that category.
        Assert.Equal("Streaming", overLimit.Title);

        var projected = Assert.Single(items, item => item.IsProjected);
        Assert.Equal(seeded.ProjectedScheduleId, projected.PlannedActivityId);
        Assert.Equal(new DateOnly(2026, 9, 1), projected.DueDate);

        var installment = Assert.Single(byId[seeded.InstallmentItemId]);
        Assert.Equal(PlannedActivityKind.CardInstallment, installment.PlannedKind);
        Assert.Equal(FinancialActivityEffect.Expense, installment.Effect);
        Assert.Equal(PlannedActivityReadiness.Ready, installment.Readiness);

        // A statement has no row of its own, so it is keyed by its card.
        var statement = Assert.Single(byId[seeded.FullCardId]);
        Assert.Equal(PlannedActivityKind.CardStatement, statement.PlannedKind);
        Assert.Equal(FinancialActivityEffect.Neutral, statement.Effect);
        Assert.Equal(PlannedActivityAction.PayCard, statement.ActionKind);
        Assert.Equal(1000m, statement.Amount);

        var debt = Assert.Single(byId[seeded.DebtInstallmentId]);
        Assert.Equal(PlannedActivityAction.PayDebt, debt.ActionKind);
        var receivable = Assert.Single(byId[seeded.ReceivableInstallmentId]);
        Assert.Equal(PlannedActivityKind.ReceivableInstallment, receivable.PlannedKind);
        Assert.Equal(PlannedActivityAction.CollectDebt, receivable.ActionKind);

        // Timing is relative to the as-of date, not to today.
        Assert.Equal(PlannedActivityTiming.Overdue, installment.Timing);
        Assert.Equal(PlannedActivityTiming.Upcoming, projected.Timing);
    }

    /// <summary>
    /// Upcoming payments is a narrowed view of the planned projection, not a second
    /// query. Its scope is deliberately smaller — money owed to the user and recurring
    /// income are planned but not obligations — while every shared item must match the
    /// planned view exactly. Two independent queries would eventually disagree.
    /// </summary>
    [SqlServerFact]
    public async Task UpcomingPayments_ReadNarrowedSliceOfPlannedProjectionWithoutDrift()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("planned-narrowing@example.test");
        await database.SeedUsersAsync(owner);
        var seeded = await SeedPlannedGraphAsync(database, owner.Id);

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var planned = await scope.ServiceProvider
            .GetRequiredService<IPlannedActivityRepository>()
            .ListAsync(owner.Id, PlannedAsOfDate, PlannedAsOfDate.AddDays(30), null, CancellationToken.None);
        var upcoming = await scope.ServiceProvider
            .GetRequiredService<IUpcomingPaymentRepository>()
            .ListCandidatesAsync(
                owner.Id, PlannedAsOfDate, PlannedAsOfDate.AddDays(30), CancellationToken.None);

        // Scope differs by design.
        Assert.DoesNotContain(upcoming, item => item.SourceId == seeded.IncomeOccurrenceId);
        Assert.DoesNotContain(upcoming, item => item.SourceId == seeded.ReceivableInstallmentId);
        Assert.Equal(planned.Count - 2, upcoming.Count);

        // Shared items must not drift in amount or date.
        var plannedById = planned.ToDictionary(item => item.PlannedActivityId);
        Assert.All(upcoming, candidate =>
        {
            var match = plannedById[candidate.SourceId];
            Assert.Equal(match.Amount, candidate.Amount);
            Assert.Equal(match.Currency, candidate.Currency);
            Assert.Equal(match.DueDate, candidate.DueDate);
            Assert.Equal(match.Title, candidate.Title);
        });
        Assert.Contains(upcoming, item => item.SourceId == seeded.DebtInstallmentId);
        Assert.Contains(upcoming, item => item.SourceId == seeded.FullCardId);
    }

    /// <summary>
    /// An overdue schedule generates several occurrences in one save. Each takes its
    /// amount from the same schedule, so they must not share one owned Money instance:
    /// persistence cannot track a single owned instance under several owners and would
    /// write NULL amounts. Previous coverage only ever saved one occurrence at a time.
    /// </summary>
    [SqlServerFact]
    public async Task RecurringOccurrences_GeneratedTogetherKeepTheirOwnAmounts()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("occurrence-batch@example.test");
        await database.SeedUsersAsync(owner);
        var account = new Account(
            Guid.NewGuid(), owner.Id, "Bills", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(Guid.NewGuid(), owner.Id, "Rent", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(), owner.Id, account, category,
            new Money(125.5m, CurrencyCode.TRY), RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 6, 1));

        await using (var context = database.CreateContext())
        {
            context.AddRange(account, category, recurring);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var provider = CreateServiceProvider(database.ConnectionString))
        await using (var scope = provider.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRecurringTransactionRepository>();
            var due = Assert.Single(await repository.ListDueAsync(
                owner.Id, new DateOnly(2026, 8, 1), CancellationToken.None));
            var generated = new List<RecurringTransactionOccurrence>();
            while (due.NextOccurrenceDate is DateOnly next && next <= new DateOnly(2026, 8, 1))
            {
                generated.Add(RecurringTransactionOccurrence.Create(Guid.NewGuid(), due, next));
                due.AdvanceAfter(next);
            }

            Assert.Equal(3, generated.Count);
            Assert.True(await repository.TrySaveGeneratedAsync(generated, CancellationToken.None));
        }

        await using var readContext = database.CreateContext();
        var stored = await readContext.RecurringTransactionOccurrences
            .AsNoTracking()
            .Where(item => item.UserId == owner.Id)
            .ToArrayAsync(CancellationToken.None);

        Assert.Equal(3, stored.Length);
        Assert.All(stored, item =>
        {
            Assert.Equal(125.5m, item.Amount.Amount);
            Assert.Equal(CurrencyCode.TRY, item.Amount.Currency);
        });
    }

    /// <summary>
    /// Every accepted realized event reaches the unified feed exactly once, with the
    /// classification the reports depend on. A miss here means an event silently
    /// disappears from the user's history or is counted under the wrong effect.
    /// </summary>
    [SqlServerFact]
    public async Task FinancialActivityFeed_ClassifiesEveryRealizedKindOnce()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("activity-feed-owner@example.test");
        var stranger = CreateUser("activity-feed-stranger@example.test");
        await database.SeedUsersAsync(owner, stranger);
        var seeded = await SeedActivityFeedGraphAsync(database, owner.Id);
        await SeedActivityFeedGraphAsync(database, stranger.Id);

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFinancialActivityRepository>();

        var page = await repository.ListAsync(owner.Id, AllActivities(), CancellationToken.None);
        var byId = page.Items.ToDictionary(item => item.ActivityId);

        // Owner isolation: the stranger's identical graph must not leak in.
        // 12 = 10 önceki hareket + iki borç açılışı (biri borç, biri alacak).
        // Açılış artık kendi başına bir hareket: para hesaba girdiği ya da
        // çıktığı an feed'de görünür, yalnız taksitlerde değil.
        Assert.Equal(12, page.TotalCount);
        Assert.Equal(12, page.Items.Count);

        // A category is a bucket many movements share, so it cannot identify one.
        // Whatever the user wrote wins; the category names the row only when they
        // wrote nothing. Without this, several subscriptions filed under one
        // category all read the same in the list.
        Assert.Equal("Streaming", byId[seeded.RecurringTransactionId].Title);
        Assert.Equal("Groceries", byId[seeded.ManualChargeId].Title);

        var income = byId[seeded.IncomeId];
        Assert.Equal(FinancialActivityKind.AccountTransaction, income.ActivityKind);
        Assert.Equal(FinancialActivityEffect.Income, income.Effect);
        Assert.Equal(FinancialActivitySourceGroup.Account, income.SourceGroup);
        Assert.Equal(FinancialActivityOrigin.Manual, income.Origin);
        Assert.Equal(FinancialActivityStatus.Realized, income.Status);
        Assert.Equal("Salary", income.CategoryName);
        Assert.Equal("Bank", income.SourceName);

        var cancelled = byId[seeded.CancelledExpenseId];
        Assert.Equal(FinancialActivityStatus.Cancelled, cancelled.Status);
        Assert.NotNull(cancelled.CancelledAtUtc);

        var transfer = byId[seeded.TransferId];
        Assert.Equal(FinancialActivityKind.Transfer, transfer.ActivityKind);
        Assert.Equal(FinancialActivityEffect.Neutral, transfer.Effect);
        Assert.Equal("Bank", transfer.SourceName);
        Assert.Equal("Cash", transfer.DestinationName);

        var charge = byId[seeded.ManualChargeId];
        Assert.Equal(FinancialActivityKind.CardCharge, charge.ActivityKind);
        Assert.Equal(FinancialActivityEffect.Expense, charge.Effect);
        Assert.Equal(FinancialActivitySourceGroup.CreditCard, charge.SourceGroup);
        Assert.Equal(FinancialActivityOrigin.Manual, charge.Origin);

        var payment = byId[seeded.PaymentId];
        Assert.Equal(FinancialActivityKind.CardPayment, payment.ActivityKind);
        // Paying the card is not a second expense.
        Assert.Equal(FinancialActivityEffect.Neutral, payment.Effect);
        Assert.Equal("Bank", payment.SourceName);
        Assert.Equal("Card", payment.DestinationName);

        Assert.Equal(FinancialActivityKind.DebtPayment, byId[seeded.DebtPaymentId].ActivityKind);
        Assert.Equal(
            FinancialActivityKind.DebtCollection, byId[seeded.DebtCollectionId].ActivityKind);

        // Origin detection: the source link decides, not the row's own table.
        Assert.Equal(FinancialActivityOrigin.Recurring, byId[seeded.RecurringTransactionId].Origin);
        Assert.Equal(FinancialActivityOrigin.Installment, byId[seeded.InstallmentChargeId].Origin);
        Assert.Equal(FinancialActivityOrigin.CsvImport, byId[seeded.ImportedTransactionId].Origin);

        // Planned occurrences and unpaid installments belong to the planned view.
        Assert.DoesNotContain(page.Items, item => item.ActivityId == seeded.PlannedOccurrenceId);
    }

    /// <summary>
    /// The merge, ordering, paging and count must all run in SQL. If any of it moved to
    /// memory this would still return the right page while quietly reading the owner's
    /// whole history on every request, so the guard is the command count and the OFFSET
    /// clause rather than the returned data.
    /// </summary>
    [SqlServerFact]
    public async Task FinancialActivityFeed_MergesAndPagesInsideSqlAsHistoryGrows()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("activity-feed-paging@example.test");
        await database.SeedUsersAsync(owner);
        await SeedActivityFeedGraphAsync(database, owner.Id);
        await SeedExtraTransactionsAsync(database, owner.Id, 40);

        var criteria = AllActivities() with { PageNumber = 3, PageSize = 5 };
        var small = await ReadWithCommandCountAsync(database, owner.Id, criteria);

        Assert.Equal(5, small.Page.Items.Count);

        // 52 = 10 + 40 ek hareket + iki borç açılışı.
        Assert.Equal(52, small.Page.TotalCount);
        // One command for the count, one for the page. Merging in memory would need one
        // per source table instead.
        Assert.Equal(2, small.ReaderCommandCount);
        Assert.Contains(
            small.CommandTexts,
            text => text.Contains("OFFSET", StringComparison.OrdinalIgnoreCase) &&
                    text.Contains("FETCH NEXT", StringComparison.OrdinalIgnoreCase));

        await SeedExtraTransactionsAsync(database, owner.Id, 200);
        var large = await ReadWithCommandCountAsync(database, owner.Id, criteria);

        // 252 = 10 + 40 + 200 hareket + iki borç açılışı.
        Assert.Equal(252, large.Page.TotalCount);
        Assert.Equal(5, large.Page.Items.Count);
        // The cost of a fixed page does not grow with the history.
        Assert.Equal(small.ReaderCommandCount, large.ReaderCommandCount);

        // Stable order: the same page twice returns the same ids in the same order.
        var repeat = await ReadWithCommandCountAsync(database, owner.Id, criteria);
        Assert.Equal(
            large.Page.Items.Select(item => item.ActivityId),
            repeat.Page.Items.Select(item => item.ActivityId));
        Assert.Equal(
            large.Page.Items.Select(item => item.ActivityDate).OrderByDescending(date => date),
            large.Page.Items.Select(item => item.ActivityDate));
    }

    [SqlServerFact]
    public async Task FinancialActivityFeed_AppliesFiltersWithoutLeakingOtherSources()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var owner = CreateUser("activity-feed-filters@example.test");
        await database.SeedUsersAsync(owner);
        var seeded = await SeedActivityFeedGraphAsync(database, owner.Id);

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFinancialActivityRepository>();

        var cards = await repository.ListAsync(
            owner.Id,
            AllActivities() with { SourceGroup = FinancialActivitySourceGroup.CreditCard },
            CancellationToken.None);
        Assert.All(cards.Items, item =>
            Assert.Equal(FinancialActivitySourceGroup.CreditCard, item.SourceGroup));

        var recurring = await repository.ListAsync(
            owner.Id,
            AllActivities() with { Origin = FinancialActivityOrigin.Recurring },
            CancellationToken.None);
        var recurringItem = Assert.Single(recurring.Items);
        Assert.Equal(seeded.RecurringTransactionId, recurringItem.ActivityId);

        var withoutCancelled = await repository.ListAsync(
            owner.Id,
            AllActivities() with { IncludeCancelled = false },
            CancellationToken.None);
        Assert.DoesNotContain(
            withoutCancelled.Items, item => item.Status == FinancialActivityStatus.Cancelled);

        // 11 = 12 hareketin iptal edilmiş olanı düşülmüş hâli.
        Assert.Equal(11, withoutCancelled.TotalCount);

        // An account filter must not match a credit card that sits in the same position.
        var byCard = await repository.ListAsync(
            owner.Id,
            AllActivities() with { AccountId = seeded.CardId },
            CancellationToken.None);
        Assert.Empty(byCard.Items);

        // Inclusive bounds on both ends.
        var singleDay = await repository.ListAsync(
            owner.Id,
            AllActivities() with
            {
                DateFrom = new DateOnly(2026, 8, 3),
                DateTo = new DateOnly(2026, 8, 3)
            },
            CancellationToken.None);
        Assert.All(singleDay.Items, item =>
            Assert.Equal(new DateOnly(2026, 8, 3), item.ActivityDate));
        Assert.NotEmpty(singleDay.Items);

        // Kapsam filtresi kapsamsız satırları da eler: transfer ve kart ödemesi
        // gelir/gider raporuna sıfır etki eder ve kapsam taşımaz (ADR 0002,
        // ADR 0003). İkisini birden iki listede birden göstermek, kullanıcı
        // tarafları karşılaştırdığında aynı para hareketini iki kez saydırırdı.
        var business = await repository.ListAsync(
            owner.Id,
            AllActivities() with { Scope = TransactionScope.Business },
            CancellationToken.None);
        Assert.NotEmpty(business.Items);
        Assert.All(business.Items, item =>
            Assert.Equal(TransactionScope.Business, item.Scope));
        Assert.DoesNotContain(
            business.Items,
            item => item.ActivityKind is FinancialActivityKind.Transfer
                or FinancialActivityKind.CardPayment);

        var personal = await repository.ListAsync(
            owner.Id,
            AllActivities() with { Scope = TransactionScope.Personal },
            CancellationToken.None);
        Assert.All(personal.Items, item =>
            Assert.Equal(TransactionScope.Personal, item.Scope));

        // Kapsamsız satırlar yalnız filtresiz okumada görünür; iki tarafın
        // toplamı bu yüzden toplamdan küçüktür ve olması gereken budur.
        var all = await repository.ListAsync(owner.Id, AllActivities(), CancellationToken.None);
        var scopeless = all.Items.Count(item => item.Scope is null);
        Assert.True(scopeless > 0);
        Assert.Equal(all.TotalCount, business.TotalCount + personal.TotalCount + scopeless);
    }

    /// <summary>
    /// Two requests realizing the same card occurrence must leave exactly one charge.
    /// Without the rowversion and the filtered unique index on the charge link this
    /// would double the card debt and the reported expense.
    /// </summary>
    [SqlServerFact]
    public async Task RecurringCardOccurrence_ConcurrentRealizationKeepsOnlyWinningCharge()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("card-occurrence-concurrency@example.test");
        await database.SeedUsersAsync(user);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Concurrency", new Money(10_000m, CurrencyCode.TRY), 15, 25);
        var category = new Category(
            Guid.NewGuid(), user.Id, "Subscriptions", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            card,
            category,
            new Money(500m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);

        await using (var context = database.CreateContext())
        {
            context.AddRange(card, category, recurring, occurrence);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var firstProvider = CreateServiceProvider(database.ConnectionString);
        await using var secondProvider = CreateServiceProvider(database.ConnectionString);
        await using var firstScope = firstProvider.CreateAsyncScope();
        await using var secondScope = secondProvider.CreateAsyncScope();
        var firstRepository = firstScope.ServiceProvider.GetRequiredService<IRecurringTransactionRepository>();
        var secondRepository = secondScope.ServiceProvider.GetRequiredService<IRecurringTransactionRepository>();
        var firstOccurrence = await firstRepository.FindOccurrenceOwnedByIdAsync(
            occurrence.Id, user.Id, true, CancellationToken.None);
        var secondOccurrence = await secondRepository.FindOccurrenceOwnedByIdAsync(
            occurrence.Id, user.Id, true, CancellationToken.None);
        Assert.NotNull(firstOccurrence);
        Assert.NotNull(secondOccurrence);
        var firstCharge = new CreditCardCharge(
            Guid.NewGuid(), user.Id, card, category, occurrence.Amount, TransactionScope.Business, occurrence.ScheduledDate);
        var secondCharge = new CreditCardCharge(
            Guid.NewGuid(), user.Id, card, category, occurrence.Amount, TransactionScope.Business, occurrence.ScheduledDate);
        var now = new DateTimeOffset(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);
        firstOccurrence.RealizeWithCharge(firstCharge.Id, now);
        secondOccurrence.RealizeWithCharge(secondCharge.Id, now);

        var winner = await firstRepository.RealizeWithChargeAsync(
            firstOccurrence, firstCharge, CancellationToken.None);
        var retryResult = await secondRepository.RealizeWithChargeAsync(
            secondOccurrence, secondCharge, CancellationToken.None);

        Assert.Equal(winner.Id, retryResult.Id);
        await using var readContext = database.CreateContext();
        Assert.Single(await readContext.CreditCardCharges.AsNoTracking().ToArrayAsync());
        var persistedOccurrence = await readContext.RecurringTransactionOccurrences
            .AsNoTracking()
            .SingleAsync(CancellationToken.None);
        Assert.Equal(winner.Id, persistedOccurrence.CreditCardChargeId);
        Assert.Null(persistedOccurrence.BudgetTransactionId);
        // A card occurrence must not also create a budget transaction.
        Assert.Empty(await readContext.Transactions.AsNoTracking().ToArrayAsync());
    }

    private static readonly DateOnly PlannedAsOfDate = new(2026, 8, 15);

    private sealed record PlannedSeed(
        Guid FullCardId,
        Guid IncomeOccurrenceId,
        Guid CardOccurrenceId,
        Guid ClosedAccountOccurrenceId,
        Guid ProjectedScheduleId,
        Guid InstallmentItemId,
        Guid DebtInstallmentId,
        Guid ReceivableInstallmentId);

    /// <summary>
    /// One of every planned source, arranged so each readiness branch is exercised: a
    /// card with no room left, an account that was closed after its schedule was made,
    /// and a schedule whose next date already has an occurrence.
    /// </summary>
    private static async Task<PlannedSeed> SeedPlannedGraphAsync(
        SqlTestDatabase database,
        Guid userId)
    {
        var bank = new Account(Guid.NewGuid(), userId, "Bank", AccountType.Bank, CurrencyCode.TRY, 5000m);
        var closing = new Account(Guid.NewGuid(), userId, "Closing", AccountType.Bank, CurrencyCode.TRY, 100m);
        var salary = new Category(Guid.NewGuid(), userId, "Salary", CategoryType.Income);
        var bills = new Category(Guid.NewGuid(), userId, "Bills", CategoryType.Expense);
        var openCard = new CreditCard(
            Guid.NewGuid(), userId, "OpenCard", new Money(10_000m, CurrencyCode.TRY), 10, 20);
        var fullCard = new CreditCard(
            Guid.NewGuid(), userId, "FullCard", new Money(1000m, CurrencyCode.TRY), 10, 20);
        // Uses the whole limit and produces the statement due on 2026-08-20.
        var fullCardCharge = new CreditCardCharge(Guid.NewGuid(), userId, fullCard, bills,
            new Money(1000m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 5));

        // Income: the schedule still points at the occurrence's date, so the projection
        // must suppress that date instead of reporting it twice.
        var incomePlan = new RecurringTransaction(Guid.NewGuid(), userId, bank, salary,
            new Money(18_000m, CurrencyCode.TRY), RecurringTransactionKind.Income,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 20));
        var incomeOccurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), incomePlan, new DateOnly(2026, 8, 20));

        // Card expense that no longer fits in the remaining limit.
        var cardPlan = new RecurringTransaction(Guid.NewGuid(), userId, fullCard, bills,
            new Money(500m, CurrencyCode.TRY), RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 25),
            description: "Streaming");
        var cardOccurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), cardPlan, new DateOnly(2026, 8, 25));
        cardPlan.AdvanceAfter(new DateOnly(2026, 8, 25));

        // The account is closed after the schedule was created, which is the only way to
        // reach this state: the constructor rejects an inactive account.
        var closedPlan = new RecurringTransaction(Guid.NewGuid(), userId, closing, bills,
            new Money(200m, CurrencyCode.TRY), RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 28));
        var closedOccurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), closedPlan, new DateOnly(2026, 8, 28));
        closedPlan.AdvanceAfter(new DateOnly(2026, 8, 28));
        closing.Deactivate();

        // No occurrence at all: reported as a projected date against the schedule.
        var projectedPlan = new RecurringTransaction(Guid.NewGuid(), userId, bank, bills,
            new Money(90m, CurrencyCode.TRY), RecurringTransactionKind.Expense,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 1));

        var installmentPlan = new InstallmentPlan(Guid.NewGuid(), userId, openCard, bills,
            Guid.NewGuid(), new Money(600m, CurrencyCode.TRY), TransactionScope.Business, 2, new DateOnly(2026, 8, 12));

        var lender = new Counterparty(Guid.NewGuid(), userId, "Lender");
        var friend = new Counterparty(Guid.NewGuid(), userId, "Friend");
        var payable = new DebtAgreement(Guid.NewGuid(), userId, lender, DebtDirection.Payable,
            TransactionScope.Business,
            new Money(600m, CurrencyCode.TRY), new Money(600m, CurrencyCode.TRY),
            DebtSourceType.Cash, bank, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 22), 2);
        var receivable = new DebtAgreement(Guid.NewGuid(), userId, friend, DebtDirection.Receivable,
            TransactionScope.Business,
            new Money(400m, CurrencyCode.TRY), new Money(400m, CurrencyCode.TRY),
            DebtSourceType.Cash, bank, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 23), 2);

        await using (var context = database.CreateContext())
        {
            context.AddRange(bank, closing, salary, bills, openCard, fullCard, fullCardCharge);
            context.AddRange(incomePlan, incomeOccurrence, cardPlan, cardOccurrence);
            context.AddRange(closedPlan, closedOccurrence, projectedPlan);
            context.AddRange(lender, friend, installmentPlan, payable, receivable);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        return new PlannedSeed(
            fullCard.Id,
            incomeOccurrence.Id,
            cardOccurrence.Id,
            closedOccurrence.Id,
            projectedPlan.Id,
            installmentPlan.GetItem(1).Id,
            payable.GetInstallment(1).Id,
            receivable.GetInstallment(1).Id);
    }

    private static FinancialActivityListCriteria AllActivities() => new(
        PageNumber: 1,
        PageSize: 100,
        DateFrom: null,
        DateTo: null,
        SourceGroup: null,
        ActivityKind: null,
        Effect: null,
        Origin: null,
        AccountId: null,
        CreditCardId: null,
        CategoryId: null,
        Scope: null,
        IncludeCancelled: true);

    private sealed record ActivityFeedSeed(
        Guid BankAccountId,
        Guid CardId,
        Guid IncomeId,
        Guid CancelledExpenseId,
        Guid TransferId,
        Guid ManualChargeId,
        Guid PaymentId,
        Guid DebtPaymentId,
        Guid DebtCollectionId,
        Guid RecurringTransactionId,
        Guid InstallmentChargeId,
        Guid ImportedTransactionId,
        Guid PlannedOccurrenceId);

    private sealed record ActivityFeedRead(
        FinancialActivityPage Page,
        int ReaderCommandCount,
        IReadOnlyList<string> CommandTexts);

    private static async Task<ActivityFeedRead> ReadWithCommandCountAsync(
        SqlTestDatabase database,
        Guid userId,
        FinancialActivityListCriteria criteria)
    {
        var interceptor = new CountingCommandInterceptor();
        await using var provider = CreateServiceProvider(database.ConnectionString, interceptor);
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFinancialActivityRepository>();
        var page = await repository.ListAsync(userId, criteria, CancellationToken.None);
        return new ActivityFeedRead(page, interceptor.ReaderCommandCount, interceptor.CommandTexts);
    }

    /// <summary>
    /// One of every accepted realized event, plus the planned records that must stay out
    /// of the feed.
    /// </summary>
    private static async Task<ActivityFeedSeed> SeedActivityFeedGraphAsync(
        SqlTestDatabase database,
        Guid userId)
    {
        var utc = new DateTimeOffset(2026, 8, 20, 9, 0, 0, TimeSpan.Zero);
        var bank = new Account(Guid.NewGuid(), userId, "Bank", AccountType.Bank, CurrencyCode.TRY, 5000m);
        var cash = new Account(Guid.NewGuid(), userId, "Cash", AccountType.Cash, CurrencyCode.TRY, 500m);
        var salary = new Category(Guid.NewGuid(), userId, "Salary", CategoryType.Income);
        var groceries = new Category(Guid.NewGuid(), userId, "Groceries", CategoryType.Expense);
        var card = new CreditCard(
            Guid.NewGuid(), userId, "Card", new Money(10_000m, CurrencyCode.TRY), 15, 25);

        var income = new BudgetTransaction(Guid.NewGuid(), userId, bank, salary,
            new Money(1000m, CurrencyCode.TRY), TransactionType.Income, TransactionScope.Business, new DateOnly(2026, 8, 1));
        var cancelledExpense = new BudgetTransaction(Guid.NewGuid(), userId, cash, groceries,
            new Money(50m, CurrencyCode.TRY), TransactionType.Expense, TransactionScope.Business, new DateOnly(2026, 8, 2));
        cancelledExpense.Cancel(utc);
        var transfer = new Transfer(Guid.NewGuid(), userId, bank, cash,
            new Money(250m, CurrencyCode.TRY), new DateOnly(2026, 8, 3));
        var manualCharge = new CreditCardCharge(Guid.NewGuid(), userId, card, groceries,
            new Money(300m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 3));
        var payment = new CreditCardPayment(Guid.NewGuid(), userId, bank, card,
            new Money(100m, CurrencyCode.TRY), new DateOnly(2026, 8, 4));

        var lender = new Counterparty(Guid.NewGuid(), userId, "Lender");
        var friend = new Counterparty(Guid.NewGuid(), userId, "Friend");
        var payable = new DebtAgreement(Guid.NewGuid(), userId, lender, DebtDirection.Payable,
            TransactionScope.Business,
            new Money(600m, CurrencyCode.TRY), new Money(600m, CurrencyCode.TRY),
            DebtSourceType.Cash, bank, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 5), 2);
        payable.GetInstallment(1).MarkPaid(bank, new DateOnly(2026, 8, 5), utc);
        var receivable = new DebtAgreement(Guid.NewGuid(), userId, friend, DebtDirection.Receivable,
            TransactionScope.Business,
            new Money(400m, CurrencyCode.TRY), new Money(400m, CurrencyCode.TRY),
            DebtSourceType.Cash, bank, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 6), 2);
        receivable.GetInstallment(1).MarkPaid(bank, new DateOnly(2026, 8, 6), utc);

        // Recurring: one realized occurrence and one still planned.
        var recurring = new RecurringTransaction(Guid.NewGuid(), userId, bank, groceries,
            new Money(75m, CurrencyCode.TRY), RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 7));
        var recurringResult = new BudgetTransaction(Guid.NewGuid(), userId, bank, groceries,
            new Money(75m, CurrencyCode.TRY), TransactionType.Expense, TransactionScope.Business, new DateOnly(2026, 8, 7),
            "Streaming");
        var realizedOccurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, new DateOnly(2026, 8, 7));
        realizedOccurrence.RealizeWithTransaction(recurringResult.Id, utc);
        recurring.AdvanceAfter(new DateOnly(2026, 8, 7));
        var plannedOccurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, new DateOnly(2026, 9, 7));
        recurring.AdvanceAfter(new DateOnly(2026, 9, 7));

        // Installment: item 1 realized into a charge, item 2 still pending.
        var plan = new InstallmentPlan(Guid.NewGuid(), userId, card, groceries, Guid.NewGuid(),
            new Money(600m, CurrencyCode.TRY), TransactionScope.Business, 2, new DateOnly(2026, 8, 8));
        var installmentCharge = new CreditCardCharge(Guid.NewGuid(), userId, card, groceries,
            new Money(300m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 8));
        plan.GetItem(1).Realize(installmentCharge.Id, utc);

        // CSV import: a confirmed row pointing at its transaction.
        var importedTransaction = new BudgetTransaction(Guid.NewGuid(), userId, cash, groceries,
            new Money(40m, CurrencyCode.TRY), TransactionType.Expense, TransactionScope.Business, new DateOnly(2026, 8, 9));
        var batch = new ImportBatch(Guid.NewGuid(), userId, "statement.csv", new string('b', 64), 100,
            "utf-8", ';', "Date", "Amount", "Description", "Reference", "yyyy-MM-dd", '.', utc);
        var row = new ImportRow(Guid.NewGuid(), userId, batch.Id, 2,
            "2026-08-09;-40.0000;Market;R9", new DateOnly(2026, 8, 9), -40m,
            CurrencyCode.TRY, "Market", "R9", []);
        row.ApplyCorrection(new DateOnly(2026, 8, 9), -40m, "Market", "R9", cash, groceries);
        row.MarkImported(importedTransaction.Id);
        batch.AddRow(row);
        batch.RecordConfirmation();

        await using (var context = database.CreateContext())
        {
            context.AddRange(bank, cash, salary, groceries, card);
            context.AddRange(income, cancelledExpense, transfer, manualCharge, payment);
            context.AddRange(lender, friend, payable, receivable);
            context.AddRange(recurring, recurringResult, realizedOccurrence, plannedOccurrence);
            context.AddRange(plan, installmentCharge);
            context.AddRange(importedTransaction, batch);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        return new ActivityFeedSeed(
            bank.Id,
            card.Id,
            income.Id,
            cancelledExpense.Id,
            transfer.Id,
            manualCharge.Id,
            payment.Id,
            payable.GetInstallment(1).Id,
            receivable.GetInstallment(1).Id,
            recurringResult.Id,
            installmentCharge.Id,
            importedTransaction.Id,
            plannedOccurrence.Id);
    }

    private static async Task SeedExtraTransactionsAsync(
        SqlTestDatabase database,
        Guid userId,
        int count)
    {
        await using var context = database.CreateContext();
        var account = await context.Accounts
            .SingleAsync(item => item.UserId == userId && item.Name == "Bank", CancellationToken.None);
        var category = await context.Categories
            .SingleAsync(item => item.UserId == userId && item.Name == "Groceries", CancellationToken.None);
        for (var index = 0; index < count; index++)
        {
            context.Add(new BudgetTransaction(
                Guid.NewGuid(),
                userId,
                account,
                category,
                new Money(10m + index, CurrencyCode.TRY),
                TransactionType.Expense,
                TransactionScope.Business,
                new DateOnly(2026, 7, 1).AddDays(index % 28)));
        }

        await context.SaveChangesAsync(CancellationToken.None);
    }

    /// <summary>
    /// Aynı ay üç kapsamda okunduğunda gelir/gider değişir, <b>bakiye ve net
    /// varlık değişmez</b>.
    /// </summary>
    /// <remarks>
    /// Bu, aşamanın en kolay sessizce bozulacak kuralı. Kapsam bir raporlama
    /// boyutudur; kullanıcının kasasındaki para ile kartına olan borcu tek
    /// havuzdur ve anahtarın konumuna göre değişseydi "ne kadar param var"
    /// sorusunun aynı anda iki farklı doğru cevabı olurdu (ADR 0013).
    /// </remarks>
    [SqlServerFact]
    public async Task ScopeFilter_SplitsIncomeAndExpenseButLeavesBalanceAndNetWorthWhole()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("scope-report@example.test");
        await database.SeedUsersAsync(user);

        var account = new Account(
            Guid.NewGuid(), user.Id, "Dükkân kasası", AccountType.Cash, CurrencyCode.TRY, 1_000m);
        var incomeCategory = new Category(
            Guid.NewGuid(), user.Id, "Satış geliri", CategoryType.Income);
        var expenseCategory = new Category(
            Guid.NewGuid(), user.Id, "Ticari mal alımı", CategoryType.Expense);
        var card = new CreditCard(
            Guid.NewGuid(), user.Id, "Kart", new Money(10_000m, CurrencyCode.TRY), 10, 20);

        var businessIncome = new BudgetTransaction(
            Guid.NewGuid(), user.Id, account, incomeCategory, new Money(600m, CurrencyCode.TRY),
            TransactionType.Income, TransactionScope.Business, new DateOnly(2026, 8, 3));
        var businessExpense = new BudgetTransaction(
            Guid.NewGuid(), user.Id, account, expenseCategory, new Money(200m, CurrencyCode.TRY),
            TransactionType.Expense, TransactionScope.Business, new DateOnly(2026, 8, 4));
        var personalExpense = new BudgetTransaction(
            Guid.NewGuid(), user.Id, account, expenseCategory, new Money(50m, CurrencyCode.TRY),
            TransactionType.Expense, TransactionScope.Personal, new DateOnly(2026, 8, 5));
        var personalCharge = new CreditCardCharge(
            Guid.NewGuid(), user.Id, card, expenseCategory, new Money(30m, CurrencyCode.TRY),
            TransactionScope.Personal, new DateOnly(2026, 8, 6));

        await using (var seed = database.CreateContext())
        {
            seed.AddRange(account, incomeCategory, expenseCategory, card);
            seed.AddRange(businessIncome, businessExpense, personalExpense, personalCharge);
            await seed.SaveChangesAsync();
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var reports = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var asOfDate = new DateOnly(2026, 8, 31);

        var all = await reports.GetMonthlyAsync(user.Id, 2026, 8, null, CancellationToken.None);
        var business = await reports.GetMonthlyAsync(
            user.Id, 2026, 8, TransactionScope.Business, CancellationToken.None);
        var personal = await reports.GetMonthlyAsync(
            user.Id, 2026, 8, TransactionScope.Personal, CancellationToken.None);

        // Gelir/gider bölünür ve iki taraf toplamı verir.
        Assert.Equal(600m, all.TotalIncome);
        Assert.Equal(280m, all.TotalExpense);
        Assert.Equal(600m, business.TotalIncome);
        Assert.Equal(200m, business.TotalExpense);
        Assert.Equal(0m, personal.TotalIncome);
        Assert.Equal(80m, personal.TotalExpense);
        Assert.Equal(all.TotalIncome, business.TotalIncome + personal.TotalIncome);
        Assert.Equal(all.TotalExpense, business.TotalExpense + personal.TotalExpense);

        // Bakiye bölünmez: 1.000 açılış + 600 gelir - 250 gider.
        var expectedBalance = 1_350m;
        foreach (var report in new[] { all, business, personal })
        {
            Assert.Equal(
                expectedBalance,
                Assert.Single(report.AccountBalances, item => item.AccountId == account.Id).Balance);
        }

        var netWorths = new List<decimal>();
        foreach (var filter in new TransactionScope?[] { null, TransactionScope.Business, TransactionScope.Personal })
        {
            var advanced = await reports.GetAdvancedAsync(
                user.Id, 2026, 8, asOfDate, 3, 30, filter, CancellationToken.None);
            netWorths.Add(advanced.NetWorth.NetWorth);

            // Kart borcu ve hesap dağılımı da bölünmez.
            Assert.Equal(30m, advanced.NetWorth.CreditCardDebt);
            Assert.Equal(
                expectedBalance,
                Assert.Single(advanced.AccountDistribution, item => item.AccountId == account.Id).Balance);
        }

        Assert.Single(netWorths.Distinct());
        Assert.Equal(expectedBalance - 30m, netWorths[0]);

        // Kırılım, iki tarafı ayrı ayrı okumakla **aynı** cevabı verir: özet
        // ekranı bu yüzden tek istekle üç sayıyı kurabiliyor ve istemcinin
        // hiçbir çıkarma yapması gerekmiyor.
        Assert.NotNull(all.ScopeBreakdown);
        var breakdown = all.ScopeBreakdown;
        Assert.Equal(business.TotalIncome, breakdown.Business.Income);
        Assert.Equal(business.TotalExpense, breakdown.Business.Expense);
        Assert.Equal(business.Net, breakdown.Business.Net);
        Assert.Equal(personal.TotalIncome, breakdown.Personal.Income);
        Assert.Equal(personal.TotalExpense, breakdown.Personal.Expense);
        Assert.Equal(personal.Net, breakdown.Personal.Net);
        Assert.Equal(all.Net, breakdown.Business.Net + breakdown.Personal.Net);

        // Filtreli okuma kırılım taşımaz: dışlanan taraf sıfır görünürdü ve
        // "o tarafta hiç hareket yok" demek olurdu.
        Assert.Null(business.ScopeBreakdown);
        Assert.Null(personal.ScopeBreakdown);
    }

    private static ApplicationUser CreateUser(string email)
    {
        return new ApplicationUser(
            Guid.NewGuid(),
            email,
            new DateTimeOffset(2026, 8, 8, 10, 0, 0, TimeSpan.Zero));
    }

    private static ServiceProvider CreateServiceProvider(
        string connectionString,
        DbCommandInterceptor? commandInterceptor = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:BusinessFinance"] = connectionString,
                ["Jwt:Issuer"] = "BusinessFinance.SqlTests",
                ["Jwt:Audience"] = "BusinessFinance.SqlTests",
                ["Jwt:SigningKey"] =
                    "synthetic-sql-tests-only-signing-key-123456789",
                ["Jwt:AccessTokenLifetime"] = "00:15:00",
                ["Jwt:RefreshTokenLifetime"] = "30.00:00:00"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);
        if (commandInterceptor is not null)
        {
            services.RemoveAll<BusinessFinanceDbContext>();
            services.RemoveAll<DbContextOptions<BusinessFinanceDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<BusinessFinanceDbContext>>();
            services.AddDbContext<BusinessFinanceDbContext>(options =>
                options
                    .UseSqlServer(connectionString)
                    .AddInterceptors(commandInterceptor));
        }

        return services.BuildServiceProvider();
    }

    private sealed class CountingCommandInterceptor : DbCommandInterceptor
    {
        public int ReaderCommandCount { get; private set; }
        public List<string> CommandTexts { get; } = [];

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            ReaderCommandCount++;
            CommandTexts.Add(command.CommandText);
            return ValueTask.FromResult(result);
        }
    }

    private sealed class TestAttachmentInspector : IAttachmentFileInspector
    {
        public AttachmentInspection Inspect(
            string fileName,
            string contentType,
            ReadOnlySpan<byte> content)
        {
            var accepted = fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                           contentType == "image/png" &&
                           content.StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
            return accepted
                ? new AttachmentInspection(
                    true, null, "image/png", ".png",
                    Convert.ToHexStringLower(SHA256.HashData(content)))
                : new AttachmentInspection(false, "Rejected.", null, null, null);
        }
    }

    private sealed class TestAttachmentObjectStore : IAttachmentObjectStore
    {
        private readonly Dictionary<string, byte[]> files = new(StringComparer.Ordinal);

        public Task WriteAsync(
            string objectKey,
            ReadOnlyMemory<byte> content,
            CancellationToken cancellationToken)
        {
            files.Add(objectKey, content.ToArray());
            return Task.CompletedTask;
        }

        public Task<Stream?> OpenReadAsync(string objectKey, CancellationToken cancellationToken) =>
            Task.FromResult<Stream?>(files.TryGetValue(objectKey, out var content)
                ? new MemoryStream(content, writable: false)
                : null);

        public Task DeleteIfExistsAsync(string objectKey, CancellationToken cancellationToken)
        {
            files.Remove(objectKey);
            return Task.CompletedTask;
        }
    }

    private sealed class SqlTestDatabase(
        string connectionString,
        DbContextOptions<BusinessFinanceDbContext> options) : IAsyncDisposable
    {
        public string ConnectionString { get; } = connectionString;

        /// <param name="targetMigration">
        /// Şemanın duracağı adım. Verilmezse zincirin tamamı uygulanır; verilirse
        /// veritabanı o adımda kalır ve yükseltme yolunun kendisi test edilebilir.
        /// </param>
        public static async Task<SqlTestDatabase> CreateAsync(
            string baseConnectionString,
            string? targetMigration = null)
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = $"BusinessFinanceIntegration_{Guid.NewGuid():N}"
            };
            var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
                .UseSqlServer(builder.ConnectionString)
                .Options;
            var database = new SqlTestDatabase(builder.ConnectionString, options);

            await using var context = database.CreateContext();
            if (targetMigration is null)
            {
                await context.Database.MigrateAsync(CancellationToken.None);
            }
            else
            {
                await context.GetService<IMigrator>().MigrateAsync(
                    targetMigration, cancellationToken: CancellationToken.None);
            }

            return database;
        }

        public async Task MigrateToLatestAsync()
        {
            await using var context = CreateContext();
            await context.Database.MigrateAsync(CancellationToken.None);
        }

        public async Task ExecuteAsync(string sql, params object[] parameters)
        {
            await using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public BusinessFinanceDbContext CreateContext()
        {
            return new BusinessFinanceDbContext(options);
        }

        public async Task SeedUsersAsync(params ApplicationUser[] users)
        {
            await using var context = CreateContext();
            context.Users.AddRange(users);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        public async ValueTask DisposeAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync(CancellationToken.None);
        }
    }
}

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(
                "BUSINESS_FINANCE_SQL_TEST_CONNECTION")))
        {
            Skip = "BUSINESS_FINANCE_SQL_TEST_CONNECTION is not configured.";
        }
    }
}
