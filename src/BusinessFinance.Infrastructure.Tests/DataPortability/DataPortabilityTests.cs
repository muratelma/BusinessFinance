using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BusinessFinance.Application.DataPortability;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.DataPortability;
using BusinessFinance.Infrastructure.Categories;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Tests.DataPortability;

public sealed class DataPortabilityTests
{
    [Fact]
    public async Task Backup_ValidatesAndRestoresCompleteSyntheticGraphToEmptyOwner()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);

        var backup = await service.CreateBackupAsync(sourceUserId, default);
        var envelope = JsonNode.Parse(backup.Content)!.AsObject();
        var payload = Encoding.UTF8.GetString(Convert.FromBase64String(envelope["payload"]!.GetValue<string>()));
        Assert.Contains("\"signedAmount\":\"-100.2500\"", payload, StringComparison.Ordinal);
        var validation = await service.ValidateBackupAsync(backup.Content, default);
        var restored = await service.RestoreBackupAsync(
            targetUserId, backup.Content, DateTimeOffset.UtcNow, default);
        var conflict = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.RestoreBackupAsync(targetUserId, backup.Content, DateTimeOffset.UtcNow, default));

        Assert.Equal(5, validation.SchemaVersion);
        Assert.Equal(validation.EntityCount, restored.RestoredEntityCount);
        Assert.Equal("restore.destination_not_empty", conflict.Code);
        Assert.Equal("Geri yükleme için hesapta finansal veri bulunmamalıdır.", conflict.Message);
        Assert.Equal(19, validation.EntityCount);
        Assert.Equal(2, await context.Accounts.CountAsync(x => x.UserId == targetUserId));
        Assert.Equal(3, await context.Categories.CountAsync(x => x.UserId == targetUserId));
        Assert.Equal(2, await context.Transactions.CountAsync(x => x.UserId == targetUserId));
        Assert.Single(await context.InstallmentPlans.Where(x => x.UserId == targetUserId).ToArrayAsync());
        Assert.Single(await context.RecurringTransactionOccurrences.Where(x => x.UserId == targetUserId).ToArrayAsync());
        Assert.Single(await context.ImportBatches.Where(x => x.UserId == targetUserId).ToArrayAsync());
        Assert.Equal(2, await context.Accounts.CountAsync(x => x.UserId == sourceUserId));
    }

    [Fact]
    public async Task Backup_CustomCategoryOnlyDestinationIsRejectedAndPreserved()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        var customCategory = new Category(
            Guid.NewGuid(), targetUserId, "Benim kategorim", CategoryType.Expense);
        context.Add(customCategory);
        await context.SaveChangesAsync();
        var service = new EfDataPortabilityRepository(context);
        var backup = await service.CreateBackupAsync(sourceUserId, default);

        var conflict = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.RestoreBackupAsync(targetUserId, backup.Content, DateTimeOffset.UtcNow, default));

        Assert.Equal("restore.destination_not_empty", conflict.Code);
        Assert.True(await context.Categories.AnyAsync(category => category.Id == customCategory.Id));
        Assert.False(await context.Accounts.AnyAsync(account => account.UserId == targetUserId));
    }

    [Fact]
    public async Task Backup_ChangedPayloadFailsIntegrityAndWritesNothing()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        var service = new EfDataPortabilityRepository(context);
        var backup = await service.CreateBackupAsync(sourceUserId, default);
        var changed = backup.Content.ToArray();
        var marker = Encoding.UTF8.GetBytes("\"payload\":\"");
        var payloadIndex = changed.AsSpan().IndexOf(marker) + marker.Length;
        Assert.True(payloadIndex >= marker.Length);
        changed[payloadIndex] = changed[payloadIndex] == (byte)'A' ? (byte)'B' : (byte)'A';

        var error = await Assert.ThrowsAsync<DataPortabilityException>(
            () => service.RestoreBackupAsync(targetUserId, changed, DateTimeOffset.UtcNow, default));

        Assert.Equal("restore.integrity_failed", error.Code);
        Assert.Equal("Yedek veri bütünlüğü doğrulanamadı.", error.Message);
        Assert.False(await context.Accounts.AnyAsync(x => x.UserId == targetUserId));
    }

    [Fact]
    public async Task TransactionsCsv_IsUtf8BomQuotedAndFormulaSafe()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, userId);
        var service = new EfDataPortabilityRepository(context);

        var file = await service.ExportTransactionsCsvAsync(userId, default);
        var text = Encoding.UTF8.GetString(file.Content);

        Assert.Equal([0xEF, 0xBB, 0xBF], file.Content[..3]);
        Assert.Contains("100.2500", text, StringComparison.Ordinal);
        Assert.Contains("'=SUM(A1:A2)", text, StringComparison.Ordinal);
        Assert.Contains("\"Market, haftalık\"", text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Backup_UnknownVersionPropertyAndDuplicatePropertyAreRejected()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        var service = new EfDataPortabilityRepository(context);
        var backup = await service.CreateBackupAsync(sourceUserId, default);

        var versionEnvelope = JsonNode.Parse(backup.Content)!.AsObject();
        versionEnvelope["schemaVersion"] = 99;
        var versionError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(Encoding.UTF8.GetBytes(versionEnvelope.ToJsonString()), default));
        Assert.Equal("restore.unsupported_version", versionError.Code);

        var unknownEnvelope = JsonNode.Parse(backup.Content)!.AsObject();
        unknownEnvelope["unexpected"] = true;
        var unknownError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(Encoding.UTF8.GetBytes(unknownEnvelope.ToJsonString()), default));
        Assert.Equal("restore.invalid_backup", unknownError.Code);

        var duplicateJson = Encoding.UTF8.GetString(backup.Content)
            .Replace("{\"format\":", "{\"format\":\"duplicate\",\"format\":", StringComparison.Ordinal);
        var duplicateError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(Encoding.UTF8.GetBytes(duplicateJson), default));
        Assert.Equal("restore.invalid_backup", duplicateError.Code);
    }

    /// <summary>
    /// Schema v3 must carry the recurring source and the card-charge realization link
    /// through a full backup and restore, otherwise a card subscription would silently
    /// come back as a bank plan.
    /// </summary>
    [Fact]
    public async Task BackupV3_RoundTripsCreditCardSourcedRecurringPlans()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCardRecurringGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);

        var backup = await service.CreateBackupAsync(sourceUserId, default);
        var validation = await service.ValidateBackupAsync(backup.Content, default);
        await service.RestoreBackupAsync(targetUserId, backup.Content, DateTimeOffset.UtcNow, default);

        Assert.Equal(5, validation.SchemaVersion);
        var restoredPlans = await context.RecurringTransactions
            .AsNoTracking().Where(item => item.UserId == targetUserId).ToArrayAsync();
        var cardPlan = Assert.Single(
            restoredPlans, item => item.SourceType == RecurringSourceType.CreditCard);
        Assert.NotNull(cardPlan.CreditCardId);
        Assert.Null(cardPlan.AccountId);

        var restoredOccurrences = await context.RecurringTransactionOccurrences
            .AsNoTracking()
            .Where(item => item.UserId == targetUserId && item.RecurringTransactionId == cardPlan.Id)
            .OrderBy(item => item.ScheduledDate)
            .ToArrayAsync();
        Assert.Equal(2, restoredOccurrences.Length);

        var realized = restoredOccurrences[0];
        Assert.Equal(RecurringOccurrenceStatus.Realized, realized.Status);
        Assert.NotNull(realized.CreditCardChargeId);
        Assert.Null(realized.BudgetTransactionId);
        // The charge link must be remapped to the restored charge, not the source id.
        var restoredCharges = await context.CreditCardCharges
            .AsNoTracking().Where(item => item.UserId == targetUserId).ToArrayAsync();
        Assert.Contains(restoredCharges, item => item.Id == realized.CreditCardChargeId);

        var planned = restoredOccurrences[1];
        Assert.Equal(RecurringOccurrenceStatus.Planned, planned.Status);
        Assert.Null(planned.CreditCardChargeId);
        Assert.Null(planned.BudgetTransactionId);
    }

    /// <summary>
    /// A backup taken before Stage 12.5 has no source fields at all. It must still
    /// restore, with every recurring row upgraded to an account source.
    /// </summary>
    [Fact]
    public async Task BackupV2_IsStillAcceptedAndUpgradedToAccountSource()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);
        var current = await service.CreateBackupAsync(sourceUserId, default);
        var legacy = DowngradeToSchemaV2(current.Content);

        var validation = await service.ValidateBackupAsync(legacy, default);
        await service.RestoreBackupAsync(targetUserId, legacy, DateTimeOffset.UtcNow, default);

        Assert.Equal(2, validation.SchemaVersion);
        var restored = await context.RecurringTransactions
            .AsNoTracking().Where(item => item.UserId == targetUserId).ToArrayAsync();
        Assert.NotEmpty(restored);
        Assert.All(restored, item =>
        {
            Assert.Equal(RecurringSourceType.Account, item.SourceType);
            Assert.NotNull(item.AccountId);
            Assert.Null(item.CreditCardId);
        });
    }

    /// <summary>
    /// Rewrites a current backup into the shape a pre-Stage-12.5 build produced: schema
    /// version 2 and no source or charge-link properties anywhere. The payload hash and
    /// length are recomputed so the file passes integrity checks and the restore path is
    /// genuinely exercised.
    /// </summary>
    private static byte[] DowngradeToSchemaV2(byte[] content)
    {
        var envelope = JsonNode.Parse(Encoding.UTF8.GetString(content))!.AsObject();
        var payloadJson = Encoding.UTF8.GetString(
            Convert.FromBase64String(envelope["payload"]!.GetValue<string>()));
        var snapshot = JsonNode.Parse(payloadJson)!.AsObject();
        foreach (var plan in snapshot["recurringTransactions"]!.AsArray())
        {
            var planObject = plan!.AsObject();
            planObject.Remove("sourceType");
            planObject.Remove("creditCardId");
            foreach (var occurrence in planObject["occurrences"]!.AsArray())
            {
                occurrence!.AsObject().Remove("creditCardChargeId");
            }
        }

        var downgraded = Encoding.UTF8.GetBytes(snapshot.ToJsonString());
        envelope["schemaVersion"] = 2;
        envelope["payload"] = Convert.ToBase64String(downgraded);
        envelope["payloadLength"] = downgraded.Length;
        envelope["payloadSha256"] = Convert.ToHexString(SHA256.HashData(downgraded));
        return Encoding.UTF8.GetBytes(envelope.ToJsonString());
    }

    private static async Task SeedCardRecurringGraphAsync(
        BusinessFinanceDbContext context,
        Guid userId)
    {
        var utc = new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero);
        var bank = new Account(Guid.NewGuid(), userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 2000m);
        var billCategory = new Category(Guid.NewGuid(), userId, "Abonelik", CategoryType.Expense);
        var card = new CreditCard(
            Guid.NewGuid(), userId, "Kart", new Money(5000m, CurrencyCode.TRY), 10, 20);
        var recurring = new RecurringTransaction(Guid.NewGuid(), userId, card, billCategory,
            new Money(149.9m, CurrencyCode.TRY), RecurringTransactionKind.BillPayment,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 10), null,
            MonthEndBehavior.ClampToLastDay, "Streaming");
        var charge = new CreditCardCharge(Guid.NewGuid(), userId, card, billCategory,
            new Money(149.9m, CurrencyCode.TRY), new DateOnly(2026, 8, 10), "Streaming");

        var realized = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, new DateOnly(2026, 8, 10));
        realized.RealizeWithCharge(charge.Id, utc);
        recurring.AdvanceAfter(new DateOnly(2026, 8, 10));
        var planned = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, new DateOnly(2026, 9, 10));
        // Generation advances the schedule past every occurrence it creates, realized
        // or not, so the stored next date must sit after the planned one.
        recurring.AdvanceAfter(new DateOnly(2026, 9, 10));

        context.AddRange(bank, billCategory, card, charge, recurring, realized, planned);
        await context.SaveChangesAsync();
    }

    private static BusinessFinanceDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
            .UseInMemoryDatabase($"data-portability-{Guid.NewGuid():N}")
            .AddInterceptors(new RowVersionInterceptor())
            .Options;
        return new BusinessFinanceDbContext(options);
    }

    internal static async Task SeedCompleteGraphAsync(BusinessFinanceDbContext context, Guid userId)
    {
        var utc = new DateTimeOffset(2026, 8, 11, 10, 0, 0, TimeSpan.Zero);
        var cash = new Account(Guid.NewGuid(), userId, "Nakit", AccountType.Cash, CurrencyCode.TRY, 1000m);
        var bank = new Account(Guid.NewGuid(), userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 2000m);
        var incomeCategory = new Category(Guid.NewGuid(), userId, "Maaş", CategoryType.Income);
        var expenseCategory = new Category(Guid.NewGuid(), userId, "Market", CategoryType.Expense);
        var billCategory = new Category(Guid.NewGuid(), userId, "Fatura", CategoryType.Expense);
        var income = new BudgetTransaction(Guid.NewGuid(), userId, bank, incomeCategory,
            new Money(1000m, CurrencyCode.TRY), TransactionType.Income, new DateOnly(2026, 8, 1), "=SUM(A1:A2)");
        var expense = new BudgetTransaction(Guid.NewGuid(), userId, cash, expenseCategory,
            new Money(100.25m, CurrencyCode.TRY), TransactionType.Expense, new DateOnly(2026, 8, 2), "Market, haftalık");
        var budget = new MonthlyBudget(Guid.NewGuid(), userId, expenseCategory,
            new Money(500m, CurrencyCode.TRY), 2026, 8);
        var transfer = new Transfer(Guid.NewGuid(), userId, bank, cash,
            new Money(250m, CurrencyCode.TRY), new DateOnly(2026, 8, 3), "ATM");
        transfer.Cancel(utc);
        var card = new CreditCard(Guid.NewGuid(), userId, "Kart", new Money(5000m, CurrencyCode.TRY), 10, 20);
        var charge = new CreditCardCharge(Guid.NewGuid(), userId, card, expenseCategory,
            new Money(300m, CurrencyCode.TRY), new DateOnly(2026, 8, 4), "Taksit");
        var payment = new CreditCardPayment(Guid.NewGuid(), userId, bank, card,
            new Money(100m, CurrencyCode.TRY), new DateOnly(2026, 8, 5), "Ödeme");
        payment.Cancel(utc);
        var plan = new InstallmentPlan(Guid.NewGuid(), userId, card, expenseCategory, Guid.NewGuid(),
            new Money(600m, CurrencyCode.TRY), 2, new DateOnly(2026, 8, 4), "Telefon");
        plan.GetItem(1).Realize(charge.Id, utc);
        var recurring = new RecurringTransaction(Guid.NewGuid(), userId, cash, billCategory,
            new Money(100.25m, CurrencyCode.TRY), RecurringTransactionKind.BillPayment,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 2), null,
            MonthEndBehavior.ClampToLastDay, "Elektrik");
        var occurrence = RecurringTransactionOccurrence.Create(Guid.NewGuid(), recurring, new DateOnly(2026, 8, 2));
        occurrence.RealizeWithTransaction(expense.Id, utc);
        recurring.AdvanceAfter(new DateOnly(2026, 8, 2));
        recurring.Deactivate();
        var batch = new ImportBatch(Guid.NewGuid(), userId, "ekstre.csv", new string('a', 64), 100,
            "utf-8", ';', "Tarih", "Tutar", "Açıklama", "Referans", "yyyy-MM-dd", '.', utc);
        var row = new ImportRow(Guid.NewGuid(), userId, batch.Id, 2,
            "2026-08-02;-100.2500;Market;R1", new DateOnly(2026, 8, 2), -100.25m,
            CurrencyCode.TRY, "Market", "R1", []);
        row.ApplyCorrection(new DateOnly(2026, 8, 2), -100.25m, "Market", "R1", cash, expenseCategory);
        row.MarkImported(expense.Id);
        batch.AddRow(row);
        batch.RecordConfirmation();
        cash.Deactivate();
        billCategory.Deactivate();
        card.Update(
            card.Name, card.Limit, card.StatementClosingDay, card.PaymentDueDay,
            card.MinimumPaymentRate, false);

        context.AddRange(cash, bank, incomeCategory, expenseCategory, billCategory, income, expense,
            budget, transfer, card, charge, payment, plan, recurring, occurrence, batch);
        await context.SaveChangesAsync();
    }

    internal static async Task SeedDefaultCategoriesAsync(
        BusinessFinanceDbContext context,
        Guid userId)
    {
        // Listeyi kopyalamak yerine üretimdekini okuyor: kopya, varsayılanlar
        // değiştiğinde sessizce eskir ve "dokunulmamış hesap" kontrolü
        // testte doğru, gerçekte yanlış davranırdı.
        context.AddRange(
            EfCategoryRepository.DefaultCategories.Select(
                item => new Category(Guid.NewGuid(), userId, item.Name, item.Type)));
        await context.SaveChangesAsync();
    }

    private sealed class RowVersionInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                SetVersions<RecurringTransactionOccurrence>(eventData.Context);
                SetVersions<ImportRow>(eventData.Context);
            }
            return ValueTask.FromResult(result);
        }

        private static void SetVersions<TEntity>(DbContext context) where TEntity : class
        {
            foreach (var entry in context.ChangeTracker.Entries<TEntity>()
                         .Where(item => item.State is EntityState.Added or EntityState.Modified))
                entry.Property<byte[]>("Version").CurrentValue = Guid.NewGuid().ToByteArray();
        }
    }
}
