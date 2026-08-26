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

        Assert.Equal(9, validation.SchemaVersion);
        Assert.Equal(validation.EntityCount, restored.RestoredEntityCount);
        Assert.Equal("restore.destination_not_empty", conflict.Code);
        Assert.Equal("Geri yükleme için hesapta finansal veri bulunmamalıdır.", conflict.Message);
        Assert.Equal(33, validation.EntityCount);
        Assert.Equal(2, await context.Accounts.CountAsync(x => x.UserId == targetUserId));
        Assert.Equal(3, await context.Categories.CountAsync(x => x.UserId == targetUserId));
        Assert.Equal(3, await context.Transactions.CountAsync(x => x.UserId == targetUserId));
        Assert.Single(await context.InstallmentPlans.Where(x => x.UserId == targetUserId).ToArrayAsync());
        Assert.Single(await context.RecurringTransactionOccurrences.Where(x => x.UserId == targetUserId).ToArrayAsync());
        Assert.Single(await context.ImportBatches.Where(x => x.UserId == targetUserId).ToArrayAsync());
        Assert.Equal(2, await context.Accounts.CountAsync(x => x.UserId == sourceUserId));

        // Kapsam yedeğin taşıdığı bir alandır: hem kaydın kendi kapsamı hem
        // hesabın/kategorinin isteğe bağlı varsayılanı geri yüklemede korunur.
        // Tek bir değere sabitlenseydi geri yükleme, kullanıcının işletme ile
        // cebi arasındaki ayrımını sessizce silerdi.
        var restoredTransactions = await context.Transactions
            .AsNoTracking().Where(x => x.UserId == targetUserId).ToArrayAsync();
        Assert.Contains(restoredTransactions, x => x.Scope == TransactionScope.Business);
        Assert.Contains(restoredTransactions, x => x.Scope == TransactionScope.Personal);
        Assert.Equal(
            TransactionScope.Business,
            (await context.Accounts.AsNoTracking()
                .SingleAsync(x => x.UserId == targetUserId && x.Name == "Nakit")).DefaultScope);
        Assert.Equal(
            TransactionScope.Personal,
            (await context.Categories.AsNoTracking()
                .SingleAsync(x => x.UserId == targetUserId && x.Name == "Fatura")).DefaultScope);
        Assert.Null(
            (await context.Accounts.AsNoTracking()
                .SingleAsync(x => x.UserId == targetUserId && x.Name == "Banka")).DefaultScope);

        // Cari defteri v7'nin taşıdığı yeni bilgidir: kişi, notu, aktifliği ve
        // iki hareket türü kayıpsız dönüyor. Tahsilatın iptali de dönüyor —
        // iptal edilmiş bir tahsilatı geri yüklerken "ödendi" saymak,
        // kullanıcının alacağını yok ederdi.
        var restoredManav = await context.Counterparties.AsNoTracking()
            .SingleAsync(x => x.UserId == targetUserId && x.Name == "Sentetik Manav");
        Assert.Equal("Çarşı girişinde", restoredManav.Note);
        Assert.True(restoredManav.IsActive);
        var restoredKapanan = await context.Counterparties.AsNoTracking()
            .SingleAsync(x => x.UserId == targetUserId && x.Name == "Kapanan Bakkal");
        Assert.False(restoredKapanan.IsActive);

        var restoredCharges = await context.CounterpartyCharges.AsNoTracking()
            .Where(x => x.UserId == targetUserId).ToArrayAsync();
        Assert.Equal(2, restoredCharges.Length);
        var receivable = Assert.Single(restoredCharges, x => x.Direction == DebtDirection.Receivable);
        Assert.Equal(restoredManav.Id, receivable.CounterpartyId);
        Assert.Equal(400m, receivable.Amount.Amount);
        // Pasif karşı tarafın borçlandırması da geri geldi.
        Assert.Contains(restoredCharges, x => x.CounterpartyId == restoredKapanan.Id);

        var restoredPayments = await context.CounterpartyPayments.AsNoTracking()
            .Where(x => x.UserId == targetUserId).ToArrayAsync();
        Assert.Equal(2, restoredPayments.Length);
        Assert.Single(restoredPayments, x => x.IsCancelled);
        Assert.All(restoredPayments, x => Assert.Equal(restoredManav.Id, x.CounterpartyId));
    }

    /// <summary>
    /// v6 dosyası reddedilir ve hedef hesaba hiçbir şey yazılmaz.
    /// </summary>
    /// <remarks>
    /// v6 karşı tarafı yalnız sözleşmenin taşıdığı ad olarak biliyordu: açık
    /// cari bakiyesi, borçlandırmaları ve tahsilatları o dosyada hiç yok.
    /// Yükseltmek, alacağı sıfır olan bir müşteri kaydı üretirdi — kullanıcının
    /// parasını sessizce silmek. Kapsam boyutunda verilen kararın aynısı
    /// (ADR 0013): eksik bilgi uydurulmaz, dosya açık bir hatayla reddedilir.
    /// </remarks>
    [Fact]
    public async Task BackupBeforeCounterpartyLedger_IsRejectedAndWritesNothing()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);
        var current = await service.CreateBackupAsync(sourceUserId, default);
        var legacy = DowngradeToSchemaV6(current.Content);

        var validationError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(legacy, default));
        var restoreError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.RestoreBackupAsync(targetUserId, legacy, DateTimeOffset.UtcNow, default));

        Assert.Equal("restore.unsupported_version", validationError.Code);
        Assert.Equal("restore.unsupported_version", restoreError.Code);
        Assert.False(await context.Accounts.AnyAsync(x => x.UserId == targetUserId));
        Assert.False(await context.Counterparties.AnyAsync(x => x.UserId == targetUserId));
    }

    /// <summary>
    /// Rewrites a current backup into the shape the build before the counterparty
    /// ledger produced: schema version 6, no counterparty collections, and debts
    /// carrying the counterparty name instead of its id. The payload hash and length
    /// are recomputed so the version gate is genuinely what rejects it.
    /// </summary>
    private static byte[] DowngradeToSchemaV6(byte[] content)
    {
        var envelope = JsonNode.Parse(Encoding.UTF8.GetString(content))!.AsObject();
        var payloadJson = Encoding.UTF8.GetString(
            Convert.FromBase64String(envelope["payload"]!.GetValue<string>()));
        var snapshot = JsonNode.Parse(payloadJson)!.AsObject();
        var names = snapshot["counterparties"]!.AsArray()
            .ToDictionary(
                item => item!["id"]!.GetValue<string>(),
                item => item!["name"]!.GetValue<string>());
        foreach (var debt in snapshot["debts"]!.AsArray())
        {
            var id = debt!["counterpartyId"]!.GetValue<string>();
            debt.AsObject().Remove("counterpartyId");
            debt.AsObject()["counterpartyName"] = names[id];
        }

        snapshot.Remove("counterparties");
        snapshot.Remove("counterpartyCharges");
        snapshot.Remove("counterpartyPayments");

        var downgraded = Encoding.UTF8.GetBytes(snapshot.ToJsonString());
        envelope["schemaVersion"] = 6;
        envelope["payload"] = Convert.ToBase64String(downgraded);
        envelope["payloadLength"] = downgraded.Length;
        envelope["payloadSha256"] = Convert.ToHexString(SHA256.HashData(downgraded));
        return Encoding.UTF8.GetBytes(envelope.ToJsonString());
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

    /// <summary>
    /// Dışa aktarılan dosya kaydın hangi tarafa yazıldığını da taşır.
    /// </summary>
    /// <remarks>
    /// Kapsamsız bir dosya, kullanıcının kendi arşivinde işletme ile cebini
    /// bir daha ayıramaz hâle getirirdi: aynı kategoride, aynı hesaptan iki
    /// kayıt arasındaki tek fark kapsamdır ve dosyada yazmazsa geri
    /// getirilemez.
    /// </remarks>
    [Fact]
    public async Task TransactionsCsv_CarriesTheScopeOfEachRow()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, userId);
        var service = new EfDataPortabilityRepository(context);

        var file = await service.ExportTransactionsCsvAsync(userId, default);
        var lines = Encoding.UTF8.GetString(file.Content)
            .Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.Length > 0)
            .ToArray();
        var header = lines[0].TrimStart('\uFEFF').Split(',');
        var scopeColumn = Array.IndexOf(header, "scope");
        var scopes = lines.Skip(1)
            .Select(line => line.Split(',')[scopeColumn])
            .ToArray();

        Assert.Equal(Array.IndexOf(header, "type") + 1, scopeColumn);
        Assert.Contains("business", scopes);
        Assert.Contains("personal", scopes);
        // Kararlı makine değeri; ekrandaki Türkçe etiket değil.
        Assert.All(scopes, value => Assert.Contains(value, new[] { "business", "personal" }));
    }

    /// <summary>
    /// Cari defterin kendi dosyası: iki kayıt türü, kendi alanlarıyla.
    /// </summary>
    /// <remarks>
    /// Boş hücre eksik veri değildir. Borçlandırma hesap sormaz, tahsilat
    /// kategori ve kapsam sormaz (ADR 0014); dosyada da sormadıkları soru boş
    /// kalır ve <c>kind</c> kolonu hangisinin okunacağını söyler. Kolonları
    /// eşitlemek, olmayan bir soruyu dosyaya yazmak olurdu.
    /// </remarks>
    [Fact]
    public async Task CounterpartyLedgerCsv_CarriesBothRecordKindsWithTheirOwnFields()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, userId);
        var service = new EfDataPortabilityRepository(context);

        var file = await service.ExportCounterpartyLedgerCsvAsync(userId, default);
        var lines = Encoding.UTF8.GetString(file.Content)
            .Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.Length > 0)
            .ToArray();
        var header = lines[0].TrimStart('\uFEFF').Split(',');
        var rows = lines.Skip(1).Select(line => line.Split(',')).ToArray();
        string Cell(string[] row, string column) => row[Array.IndexOf(header, column)];

        Assert.Equal([0xEF, 0xBB, 0xBF], file.Content[..3]);
        Assert.Equal(4, rows.Length);

        var charge = Assert.Single(rows, row => Cell(row, "kind") == "charge" &&
            Cell(row, "direction") == "receivable");
        Assert.Equal("400.0000", Cell(charge, "amount"));
        Assert.Equal("Sentetik Manav", Cell(charge, "counterpartyName"));
        Assert.Equal("Maaş", Cell(charge, "categoryName"));
        Assert.Equal("business", Cell(charge, "scope"));
        // Borçlandırma para taşımaz: hesap kolonu bilerek boş.
        Assert.Equal(string.Empty, Cell(charge, "accountId"));
        Assert.Equal(string.Empty, Cell(charge, "accountName"));

        var payment = Assert.Single(rows, row => Cell(row, "kind") == "payment" &&
            Cell(row, "isCancelled") == "false");
        Assert.Equal("120.0000", Cell(payment, "amount"));
        Assert.Equal("Banka", Cell(payment, "accountName"));
        // Tahsilat gelir/gider tanımaz: kategori ve kapsam kolonları boş.
        Assert.Equal(string.Empty, Cell(payment, "categoryId"));
        Assert.Equal(string.Empty, Cell(payment, "categoryName"));
        Assert.Equal(string.Empty, Cell(payment, "scope"));

        // İptal edilmiş hareket dosyada kalır ve iptal olduğunu söyler;
        // silinmiş gibi göstermek kullanıcının kendi arşivinde bir kaydı yok
        // etmek olurdu.
        var cancelled = Assert.Single(rows, row => Cell(row, "isCancelled") == "true");
        Assert.NotEqual(string.Empty, Cell(cancelled, "cancelledAtUtc"));
    }

    /// <summary>
    /// Pasif karşı tarafın geçmişi de dosyada; defter kapanmış cariyi de anlatır.
    /// </summary>
    [Fact]
    public async Task CounterpartyLedgerCsv_IncludesInactiveCounterpartiesAndIsFormulaSafe()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, userId);
        var formulaNamed = new Counterparty(Guid.NewGuid(), userId, "=CMD()");
        var incomeCategory = await context.Categories
            .SingleAsync(x => x.UserId == userId && x.Name == "Maaş");
        context.Add(formulaNamed);
        context.Add(new CounterpartyCharge(
            Guid.NewGuid(), userId, formulaNamed, incomeCategory, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Personal,
            new DateOnly(2026, 8, 10), "Market, haftalık"));
        await context.SaveChangesAsync();
        var service = new EfDataPortabilityRepository(context);

        var text = Encoding.UTF8.GetString(
            (await service.ExportCounterpartyLedgerCsvAsync(userId, default)).Content);

        Assert.Contains("Kapanan Bakkal", text, StringComparison.Ordinal);
        Assert.Contains("'=CMD()", text, StringComparison.Ordinal);
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

        Assert.Equal(9, validation.SchemaVersion);
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
    /// Kapsam boyutundan önce alınmış bir yedek reddedilir ve hedef hesaba
    /// hiçbir şey yazılmaz.
    /// </summary>
    /// <remarks>
    /// v5 dosyasında kapsam alanı yok. Eksik alanı doldurmak için bir değer
    /// seçmek, kullanıcının işletme ile cebi arasındaki ayrımını uydurmak
    /// olurdu; o ayrımı yalnız kullanıcı bilir (ADR 0013). Bu yüzden
    /// yükseltilmez, açık bir hatayla reddedilir.
    /// </remarks>
    [Fact]
    public async Task BackupBeforeScope_IsRejectedAndWritesNothing()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);
        var current = await service.CreateBackupAsync(sourceUserId, default);
        var legacy = DowngradeToSchemaV5(current.Content);

        var validationError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(legacy, default));
        var restoreError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.RestoreBackupAsync(targetUserId, legacy, DateTimeOffset.UtcNow, default));

        Assert.Equal("restore.unsupported_version", validationError.Code);
        Assert.Equal("restore.unsupported_version", restoreError.Code);
        Assert.False(await context.Accounts.AnyAsync(x => x.UserId == targetUserId));
        Assert.False(await context.Transactions.AnyAsync(x => x.UserId == targetUserId));
    }

    /// <summary>
    /// Rewrites a current backup into the shape the build before the scope dimension
    /// produced: schema version 5 with no scope properties anywhere. The payload hash
    /// and length are recomputed so the file passes integrity checks and the version
    /// gate is genuinely what rejects it.
    /// </summary>
    private static byte[] DowngradeToSchemaV5(byte[] content)
    {
        var envelope = JsonNode.Parse(Encoding.UTF8.GetString(content))!.AsObject();
        var payloadJson = Encoding.UTF8.GetString(
            Convert.FromBase64String(envelope["payload"]!.GetValue<string>()));
        var snapshot = JsonNode.Parse(payloadJson)!.AsObject();
        foreach (var collection in snapshot)
        {
            if (collection.Value is not JsonArray items)
            {
                continue;
            }

            foreach (var item in items)
            {
                item?.AsObject().Remove("scope");
                item?.AsObject().Remove("defaultScope");
            }
        }

        var downgraded = Encoding.UTF8.GetBytes(snapshot.ToJsonString());
        envelope["schemaVersion"] = 5;
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
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 10), null,
            MonthEndBehavior.ClampToLastDay, "Streaming");
        var charge = new CreditCardCharge(Guid.NewGuid(), userId, card, billCategory,
            new Money(149.9m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 10), "Streaming");

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

    /// <summary>
    /// v8'in taşıdığı üç yeni bilgi kayıpsız dönüyor: yükümlülük ve onu kapatan
    /// nakit hareketi, cari borçlandırmanın vadesi, tekrarlayan planın bitiş
    /// sınırı ve üretilmiş occurrence sayacı.
    /// </summary>
    [Fact]
    public async Task BackupV8_RoundTripsObligationsDueDatesAndOccurrenceLimits()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);

        var backup = await service.CreateBackupAsync(sourceUserId, default);
        var validation = await service.ValidateBackupAsync(backup.Content, default);
        await service.RestoreBackupAsync(targetUserId, backup.Content, DateTimeOffset.UtcNow, default);

        Assert.Equal(9, validation.SchemaVersion);

        var restoredObligations = await context.Obligations.AsNoTracking()
            .Include(x => x.Settlement)
            .Where(x => x.UserId == targetUserId).ToArrayAsync();
        Assert.Equal(2, restoredObligations.Length);

        var acik = Assert.Single(restoredObligations, x => x.Direction == DebtDirection.Payable);
        Assert.Equal(ObligationStatus.Open, acik.Status);
        Assert.Equal(275.5m, acik.Amount.Amount);
        Assert.Equal(new DateOnly(2026, 8, 6), acik.IssueDate);
        Assert.Equal(new DateOnly(2026, 8, 20), acik.DueDate);
        Assert.Equal(TransactionScope.Business, acik.Scope);
        // Karşı taraf bağı hedef kullanıcının kendi kaydına yeniden bağlanır;
        // kaynak kimliği taşınsaydı yükümlülük başka bir kişiyi gösterirdi.
        var restoredManav = await context.Counterparties.AsNoTracking()
            .SingleAsync(x => x.UserId == targetUserId && x.Name == "Sentetik Manav");
        Assert.Equal(restoredManav.Id, acik.CounterpartyId);
        Assert.Null(acik.Settlement);
        // Gecikme dosyada taşınan bir alan değil; okunduğu tarihten türüyor.
        Assert.False(acik.IsOverdueOn(new DateOnly(2026, 8, 20)));
        Assert.True(acik.IsOverdueOn(new DateOnly(2026, 8, 21)));

        var kapanan = Assert.Single(restoredObligations, x => x.Direction == DebtDirection.Receivable);
        Assert.Equal(ObligationStatus.Settled, kapanan.Status);
        Assert.Null(kapanan.CounterpartyId);
        var settlement = Assert.IsType<ObligationSettlement>(kapanan.Settlement);
        Assert.Equal(90m, settlement.Amount.Amount);
        Assert.Equal(new DateOnly(2026, 8, 9), settlement.SettlementDate);
        var restoredBank = await context.Accounts.AsNoTracking()
            .SingleAsync(x => x.UserId == targetUserId && x.Name == "Banka");
        Assert.Equal(restoredBank.Id, settlement.AccountId);
        Assert.False(settlement.IsCancelled);

        var restoredReceivableCharge = await context.CounterpartyCharges.AsNoTracking()
            .SingleAsync(x => x.UserId == targetUserId && x.Direction == DebtDirection.Receivable);
        Assert.Equal(new DateOnly(2026, 8, 20), restoredReceivableCharge.DueDate);
        // Vadesiz borçlandırmaya vade uydurulmaz.
        var restoredPayableCharge = await context.CounterpartyCharges.AsNoTracking()
            .SingleAsync(x => x.UserId == targetUserId && x.Direction == DebtDirection.Payable);
        Assert.Null(restoredPayableCharge.DueDate);

        var restoredPlan = await context.RecurringTransactions.AsNoTracking()
            .SingleAsync(x => x.UserId == targetUserId);
        Assert.Equal(12, restoredPlan.OccurrenceLimit);
        Assert.Equal(1, restoredPlan.GeneratedOccurrenceCount);
    }

    /// <summary>
    /// Elle büyütülmüş occurrence sayacı taşıyan bir dosya geri yüklenmez.
    /// </summary>
    /// <remarks>
    /// Sayaç occurrence geçmişinden türetilir; dosyadaki değer yalnız
    /// doğrulama içindir. Olduğu gibi yazılsaydı, sınırı dolmuş bir plan
    /// yedek üzerinden yeniden üretir hâle getirilebilirdi.
    /// </remarks>
    [Fact]
    public async Task Backup_TamperedOccurrenceCountIsRejectedAndWritesNothing()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);
        var backup = await service.CreateBackupAsync(sourceUserId, default);
        var tampered = RewritePayload(backup.Content, snapshot =>
        {
            foreach (var plan in snapshot["recurringTransactions"]!.AsArray())
                plan!.AsObject()["generatedOccurrenceCount"] = 7;
        });

        // Bütünlük kapısı değil, alan tutarlılığı kapısı reddediyor: hash ve
        // uzunluk yeniden hesaplandığı için dosya sağlam, içeriği tutarsız.
        // Doğrulama grafiği kuru olarak kurduğu için tutarsızlık kullanıcıya
        // restore'a basmadan önce söylenir.
        var validationError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(tampered, default));
        var error = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.RestoreBackupAsync(targetUserId, tampered, DateTimeOffset.UtcNow, default));

        Assert.Equal("restore.invalid_backup", validationError.Code);
        Assert.Equal("restore.invalid_backup", error.Code);
        Assert.False(await context.RecurringTransactions.AnyAsync(x => x.UserId == targetUserId));
        Assert.False(await context.Accounts.AnyAsync(x => x.UserId == targetUserId));
    }

    /// <summary>
    /// Kasa sayımı ve POS tahsilatı yedekten kayıpsız döner.
    /// </summary>
    /// <remarks>
    /// Türetilen hiçbir şey dosyada yok: sayımın beklenen tutarı ve farkı,
    /// tahsilatın net tutarı ve komisyon oranı geri yüklenen kayıttan yeniden
    /// hesaplanıyor. Kapanmış sayımın iptal damgası ve duran sayımın fark
    /// hareketi de dönüyor - hareket <b>yeni</b> kimliğine bağlanmalı, dosyadaki
    /// eskisine değil.
    /// </remarks>
    [Fact]
    public async Task BackupV9_RoundTripsCashCountsAndPosSettlements()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);

        var backup = await service.CreateBackupAsync(sourceUserId, default);
        var validation = await service.ValidateBackupAsync(backup.Content, default);
        await service.RestoreBackupAsync(targetUserId, backup.Content, DateTimeOffset.UtcNow, default);

        Assert.Equal(9, validation.SchemaVersion);

        var counts = await context.CashCounts.AsNoTracking()
            .Where(item => item.UserId == targetUserId)
            .OrderBy(item => item.CountedAmount).ToArrayAsync();
        Assert.Equal(2, counts.Length);
        var kapatilan = counts[0];
        var duran = counts[1];
        Assert.True(kapatilan.IsCancelled);
        Assert.NotNull(kapatilan.CancelledAtUtc);
        Assert.False(duran.IsCancelled);
        Assert.Equal(495m, duran.CountedAmount);
        Assert.Equal(new DateOnly(2026, 8, 10), duran.CountDate);
        Assert.Equal(TransactionScope.Business, duran.Scope);

        // Fark hareketi hedef kullanıcının kendi kaydına bağlanmalı.
        var targetTransactions = await context.Transactions.AsNoTracking()
            .Where(item => item.UserId == targetUserId).ToArrayAsync();
        Assert.NotNull(duran.AdjustmentTransactionId);
        Assert.Contains(targetTransactions, item => item.Id == duran.AdjustmentTransactionId);
        Assert.DoesNotContain(
            await context.Transactions.AsNoTracking()
                .Where(item => item.UserId == sourceUserId).ToArrayAsync(),
            item => item.Id == duran.AdjustmentTransactionId);

        var settlements = await context.PosSettlements.AsNoTracking()
            .Where(item => item.UserId == targetUserId)
            .OrderBy(item => item.SettlementDate).ToArrayAsync();
        Assert.Equal(2, settlements.Length);
        var gecmis = settlements[0];
        var yolda = settlements[1];

        Assert.True(gecmis.IsTransferred);
        Assert.Equal(new DateOnly(2026, 8, 8), gecmis.TransferredOn);
        Assert.Equal(0m, gecmis.CommissionAmount);
        Assert.Null(gecmis.CommissionCategoryId);

        Assert.True(yolda.IsInTransit);
        Assert.Null(yolda.TransferredOn);
        Assert.Equal(500m, yolda.GrossAmount.Amount);
        Assert.Equal(12.5m, yolda.CommissionAmount);
        // Net tutar ve oran dosyadan gelmiyor, paradan çözülüyor.
        Assert.Equal(487.5m, yolda.NetAmount.Amount);
        Assert.Equal(0.025m, yolda.CommissionRate);
        Assert.NotNull(yolda.CommissionCategoryId);
    }

    /// <summary>
    /// v8 dosyası reddedilir ve hedef hesaba hiçbir şey yazılmaz.
    /// </summary>
    /// <remarks>
    /// v8 kasa sayımını ve POS tahsilatını hiç bilmiyordu. Boş dizi yazarak
    /// yükseltmek dürüst olmazdı: o dosyayı yazan kullanıcı kartla yaptığı
    /// satışı elle bir gelir kaydı olarak girmiş olabilir ve hangi gelirin POS
    /// satışı olduğunu yalnız kendisi bilir. Yükseltilseydi aynı satış iki kez
    /// sayılabilirdi. Kapsam (ADR 0013) ve yükümlülük kararının aynısı.
    /// </remarks>
    [Fact]
    public async Task BackupBeforeCashAndPos_IsRejectedAndWritesNothing()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);
        var current = await service.CreateBackupAsync(sourceUserId, default);
        var legacy = DowngradeToSchemaV8(current.Content);

        var validationError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(legacy, default));
        var restoreError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.RestoreBackupAsync(targetUserId, legacy, DateTimeOffset.UtcNow, default));

        Assert.Equal("restore.unsupported_version", validationError.Code);
        Assert.Equal("restore.unsupported_version", restoreError.Code);
        Assert.False(await context.Accounts.AnyAsync(x => x.UserId == targetUserId));
        Assert.False(await context.CashCounts.AnyAsync(x => x.UserId == targetUserId));
        Assert.False(await context.PosSettlements.AnyAsync(x => x.UserId == targetUserId));
    }

    /// <summary>
    /// Rewrites a current backup into the shape the build before the till count
    /// and the pos settlement produced: schema version 8 with neither collection.
    /// The payload hash and length are recomputed so the version gate is genuinely
    /// what rejects it.
    /// </summary>
    private static byte[] DowngradeToSchemaV8(byte[] content) =>
        RewritePayload(
            content,
            snapshot =>
            {
                snapshot.Remove("cashCounts");
                snapshot.Remove("posSettlements");
            },
            schemaVersion: 8);

    /// <summary>
    /// v7 dosyası reddedilir ve hedef hesaba hiçbir şey yazılmaz.
    /// </summary>
    /// <remarks>
    /// v7 yükümlülüğü hiç bilmiyordu: ödenmemiş faturayı tanıyan ekonomik olay
    /// o dosyada yok. Vadesi, kategorisi ve kapsamı yalnız kullanıcının bildiği
    /// bilgiler; uydurmak gideri yanlış aya ve yanlış tarafa yazardı. Cari
    /// vadesi ve planın bitiş sınırı da aynı dosyada eksik. Kapsam (ADR 0013) ve
    /// cari defter kararının aynısı: eksik bilgi tamamlanmaz, dosya reddedilir.
    /// </remarks>
    [Fact]
    public async Task BackupBeforeObligations_IsRejectedAndWritesNothing()
    {
        await using var context = CreateContext();
        var sourceUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        await SeedCompleteGraphAsync(context, sourceUserId);
        await SeedDefaultCategoriesAsync(context, targetUserId);
        var service = new EfDataPortabilityRepository(context);
        var current = await service.CreateBackupAsync(sourceUserId, default);
        var legacy = DowngradeToSchemaV7(current.Content);

        var validationError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.ValidateBackupAsync(legacy, default));
        var restoreError = await Assert.ThrowsAsync<DataPortabilityException>(() =>
            service.RestoreBackupAsync(targetUserId, legacy, DateTimeOffset.UtcNow, default));

        Assert.Equal("restore.unsupported_version", validationError.Code);
        Assert.Equal("restore.unsupported_version", restoreError.Code);
        Assert.False(await context.Accounts.AnyAsync(x => x.UserId == targetUserId));
        Assert.False(await context.Obligations.AnyAsync(x => x.UserId == targetUserId));
    }

    /// <summary>
    /// Rewrites a current backup into the shape the build before obligations
    /// produced: schema version 7 with no obligation collection, no counterparty
    /// due dates and no recurring end limits. The payload hash and length are
    /// recomputed so the version gate is genuinely what rejects it.
    /// </summary>
    private static byte[] DowngradeToSchemaV7(byte[] content) =>
        RewritePayload(
            content,
            snapshot =>
            {
                snapshot.Remove("obligations");
                snapshot.Remove("cashCounts");
                snapshot.Remove("posSettlements");
                foreach (var charge in snapshot["counterpartyCharges"]!.AsArray())
                    charge!.AsObject().Remove("dueDate");
                foreach (var plan in snapshot["recurringTransactions"]!.AsArray())
                {
                    plan!.AsObject().Remove("occurrenceLimit");
                    plan.AsObject().Remove("generatedOccurrenceCount");
                }
            },
            schemaVersion: 7);

    /// <summary>
    /// Applies <paramref name="change"/> to the payload of a backup and recomputes
    /// its length and hash, so the integrity gate is never what rejects the result.
    /// </summary>
    private static byte[] RewritePayload(
        byte[] content,
        Action<JsonObject> change,
        int? schemaVersion = null)
    {
        var envelope = JsonNode.Parse(Encoding.UTF8.GetString(content))!.AsObject();
        var payloadJson = Encoding.UTF8.GetString(
            Convert.FromBase64String(envelope["payload"]!.GetValue<string>()));
        var snapshot = JsonNode.Parse(payloadJson)!.AsObject();
        change(snapshot);

        var rewritten = Encoding.UTF8.GetBytes(snapshot.ToJsonString());
        if (schemaVersion is int version) envelope["schemaVersion"] = version;
        envelope["payload"] = Convert.ToBase64String(rewritten);
        envelope["payloadLength"] = rewritten.Length;
        envelope["payloadSha256"] = Convert.ToHexString(SHA256.HashData(rewritten));
        return Encoding.UTF8.GetBytes(envelope.ToJsonString());
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
        var cash = new Account(Guid.NewGuid(), userId, "Nakit", AccountType.Cash, CurrencyCode.TRY, 1000m,
            TransactionScope.Business);
        var bank = new Account(Guid.NewGuid(), userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 2000m);
        var incomeCategory = new Category(Guid.NewGuid(), userId, "Maaş", CategoryType.Income);
        var expenseCategory = new Category(Guid.NewGuid(), userId, "Market", CategoryType.Expense);
        var billCategory = new Category(Guid.NewGuid(), userId, "Fatura", CategoryType.Expense,
            TransactionScope.Personal);
        var income = new BudgetTransaction(Guid.NewGuid(), userId, bank, incomeCategory,
            new Money(1000m, CurrencyCode.TRY), TransactionType.Income, TransactionScope.Business, new DateOnly(2026, 8, 1), "=SUM(A1:A2)");
        var expense = new BudgetTransaction(Guid.NewGuid(), userId, cash, expenseCategory,
            new Money(100.25m, CurrencyCode.TRY), TransactionType.Expense, TransactionScope.Personal, new DateOnly(2026, 8, 2), "Market, haftalık");
        var budget = new MonthlyBudget(Guid.NewGuid(), userId, expenseCategory,
            new Money(500m, CurrencyCode.TRY), TransactionScope.Business, 2026, 8);
        var transfer = new Transfer(Guid.NewGuid(), userId, bank, cash,
            new Money(250m, CurrencyCode.TRY), new DateOnly(2026, 8, 3), "ATM");
        transfer.Cancel(utc);
        var card = new CreditCard(Guid.NewGuid(), userId, "Kart", new Money(5000m, CurrencyCode.TRY), 10, 20);
        var charge = new CreditCardCharge(Guid.NewGuid(), userId, card, expenseCategory,
            new Money(300m, CurrencyCode.TRY), TransactionScope.Business, new DateOnly(2026, 8, 4), "Taksit");
        var payment = new CreditCardPayment(Guid.NewGuid(), userId, bank, card,
            new Money(100m, CurrencyCode.TRY), new DateOnly(2026, 8, 5), "Ödeme");
        payment.Cancel(utc);
        var plan = new InstallmentPlan(Guid.NewGuid(), userId, card, expenseCategory, Guid.NewGuid(),
            new Money(600m, CurrencyCode.TRY), TransactionScope.Business, 2, new DateOnly(2026, 8, 4), "Telefon");
        plan.GetItem(1).Realize(charge.Id, utc);
        var recurring = new RecurringTransaction(Guid.NewGuid(), userId, cash, billCategory,
            new Money(100.25m, CurrencyCode.TRY), RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 2), null,
            MonthEndBehavior.ClampToLastDay, "Elektrik", 12);
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
        // Cari hesap: bir borçlandırma (tanır) ve bir tahsilat (taşır), artı
        // artık iş yapılmayan ama borcu duran pasif bir karşı taraf. Pasif
        // olanın hareketi, geri yüklemede pasifleştirmenin hareketlerden
        // **sonra** uygulandığını kanıtlıyor.
        var manav = new Counterparty(Guid.NewGuid(), userId, "Sentetik Manav", "Çarşı girişinde");
        var kapanan = new Counterparty(Guid.NewGuid(), userId, "Kapanan Bakkal");
        var veresiye = new CounterpartyCharge(Guid.NewGuid(), userId, manav, incomeCategory,
            DebtDirection.Receivable, new Money(400m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 6), "Veresiye satış", new DateOnly(2026, 8, 20));
        var vadeliAlim = new CounterpartyCharge(Guid.NewGuid(), userId, kapanan, expenseCategory,
            DebtDirection.Payable, new Money(150m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 7), "Vadeli alım");
        var tahsilat = new CounterpartyPayment(Guid.NewGuid(), userId, manav, bank,
            DebtDirection.Receivable, new Money(120m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 8), "Kısmi tahsilat");
        var iptalTahsilat = new CounterpartyPayment(Guid.NewGuid(), userId, manav, bank,
            DebtDirection.Receivable, new Money(50m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 9), "Yanlış tahsilat");
        iptalTahsilat.Cancel(utc);

        // Yükümlülük: biri hâlâ açık ve karşı taraflı (vadesi ileride), biri
        // karşı tarafsız ve kapanmış. Kapanmış olan, nakdi taşıyan settlement'ın
        // da kayıpsız döndüğünü kanıtlıyor.
        var acikFatura = new Obligation(
            Guid.NewGuid(), userId, expenseCategory, DebtDirection.Payable,
            new Money(275.5m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 6), new DateOnly(2026, 8, 20), utc, manav, "Elektrik faturası");
        var kapananAlacak = new Obligation(
            Guid.NewGuid(), userId, incomeCategory, DebtDirection.Receivable,
            new Money(90m, CurrencyCode.TRY), TransactionScope.Personal,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 10), utc, null, "Tek seferlik alacak");
        kapananAlacak.Settle(Guid.NewGuid(), bank, new DateOnly(2026, 8, 9), utc);

        // Gün sonu sayımı: bir gözlem. İkisi aynı gün ve aynı kasa için, ilki
        // ikincisi tarafından kapatılmış - iptal edilmiş gözlem de geçmiştir ve
        // dosyada kalır. Duran sayımın farkı onaylanmış, yani ayrı bir hareketi
        // var; o hareketin geri yüklemede yeni kimliğine bağlandığı sınanıyor.
        var kasaFarki = new BudgetTransaction(Guid.NewGuid(), userId, cash, expenseCategory,
            new Money(5m, CurrencyCode.TRY), TransactionType.Expense, TransactionScope.Business,
            new DateOnly(2026, 8, 10), "Kasa farkı");
        var eskiSayim = new CashCount(Guid.NewGuid(), userId, cash, 480m,
            TransactionScope.Business, new DateOnly(2026, 8, 10), utc, "İlk sayım");
        var sayim = new CashCount(Guid.NewGuid(), userId, cash, 495m,
            TransactionScope.Business, new DateOnly(2026, 8, 10), utc, "Yeniden sayıldı");
        eskiSayim.SupersedeWith(sayim, utc);
        sayim.RecordAdjustment(kasaFarki.Id, utc);

        // POS tahsilatı: biri hâlâ yolda (hesap kıpırdamadı), biri hesaba
        // geçmiş. İkisi birlikte, tek kaydın iki anının da kayıpsız döndüğünü
        // kanıtlıyor. Komisyonsuz olanın gider kategorisi de yoktur.
        var yoldakiTahsilat = new PosSettlement(
            Guid.NewGuid(), userId, bank, incomeCategory,
            new Money(500m, CurrencyCode.TRY), 12.5m, TransactionScope.Business,
            new DateOnly(2026, 8, 9), new DateOnly(2026, 8, 12), utc,
            expenseCategory, "Kartlı satış");
        var gecmisTahsilat = new PosSettlement(
            Guid.NewGuid(), userId, bank, incomeCategory,
            new Money(300m, CurrencyCode.TRY), 0m, TransactionScope.Business,
            new DateOnly(2026, 8, 5), new DateOnly(2026, 8, 8), utc);
        gecmisTahsilat.MarkTransferred(new DateOnly(2026, 8, 8), utc);

        kapanan.Deactivate();

        cash.Deactivate();
        billCategory.Deactivate();
        card.Update(
            card.Name, card.Limit, card.StatementClosingDay, card.PaymentDueDay,
            card.MinimumPaymentRate, false);

        context.AddRange(cash, bank, incomeCategory, expenseCategory, billCategory, income, expense,
            budget, transfer, card, charge, payment, plan, recurring, occurrence, batch,
            manav, kapanan, veresiye, vadeliAlim, tahsilat, iptalTahsilat,
            acikFatura, kapananAlacak, kasaFarki, eskiSayim, sayim,
            yoldakiTahsilat, gecmisTahsilat);
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
