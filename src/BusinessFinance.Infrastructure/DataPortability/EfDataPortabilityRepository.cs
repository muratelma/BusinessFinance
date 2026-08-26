using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.DataPortability;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Application.Attachments;

namespace BusinessFinance.Infrastructure.DataPortability;

public sealed class EfDataPortabilityRepository(
    BusinessFinanceDbContext dbContext,
    IAttachmentObjectStore? attachmentStore = null,
    IAttachmentFileInspector? attachmentInspector = null)
    : IDataPortabilityRepository
{
    internal const int SchemaVersion = 10;

    /// <summary>
    /// Versions this build can restore. Only <see cref="SchemaVersion"/> is written.
    /// </summary>
    /// <remarks>
    /// v9 gün sonu kasa sayımını ve POS tahsilatını taşır; <b>yalnız v9
    /// okunur</b>. v8 dosyasında ikisi de hiç yok ve boş dizi yazarak
    /// yükseltmek dürüst olmazdı: o dosyayı yazan kullanıcı kartla yaptığı
    /// satışı elle bir gelir kaydı olarak girmiş olabilir ve hangi gelirin POS
    /// satışı olduğunu yalnız kendisi bilir. Yükseltilseydi aynı satış bir kez
    /// o gelir kaydı, bir kez de sonradan girilen tahsilat olarak sayılırdı.
    /// Aynı gerekçeyle v7 yükümlülükten, v6 cari defterden, v2-v5 kapsam
    /// boyutundan yoksundu (ADR 0013). Bu yüzden eski yedekler yükseltilmez,
    /// <c>restore.unsupported_version</c> ile reddedilir.
    /// </remarks>
    private static readonly int[] SupportedSchemaVersions = [10];

    internal const int MaximumPayloadBytes = 10 * 1024 * 1024;
    internal const int MaximumEntities = 50_000;
    private const string BackupFormat = "business-finance-backup";
    private const string BackupContentType = "application/vnd.business-finance.backup+json";
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    public async Task<PortableFile> ExportTransactionsCsvAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var transactions = await dbContext.Transactions.AsNoTracking()
            .Where(item => item.UserId == userId)
            .OrderBy(item => item.TransactionDate)
            .ThenBy(item => item.Id)
            .ToArrayAsync(cancellationToken);
        var accountNames = await dbContext.Accounts.AsNoTracking()
            .Where(item => item.UserId == userId)
            .ToDictionaryAsync(item => item.Id, item => item.Name, cancellationToken);
        var categoryNames = await dbContext.Categories.AsNoTracking()
            .Where(item => item.UserId == userId)
            .ToDictionaryAsync(item => item.Id, item => item.Name, cancellationToken);

        using var stream = new MemoryStream();
        await stream.WriteAsync(new byte[] { 0xEF, 0xBB, 0xBF }, cancellationToken);
        await using (var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true))
        {
            // `scope` türün hemen yanında: ikisi de kaydın **ne olduğunu**
            // söyleyen boyutlar ve satırı okuyan kişi "expense, personal" diye
            // yan yana okuyor. Kararlı makine değeri yazılır (`business` /
            // `personal`); dosya bir tabloya değil, kullanıcının kendi
            // arşivine gidiyor ve Türkçe etiket onu bir daha okunamaz yapardı.
            await writer.WriteLineAsync(
                "id,transactionDate,type,scope,amount,currency,accountId,accountName,categoryId,categoryName,description,isCancelled,cancelledAtUtc");
            foreach (var item in transactions)
            {
                var cells = new[]
                {
                    item.Id.ToString("D"),
                    item.TransactionDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    item.Type.ToString().ToLowerInvariant(),
                    item.Scope.ToString().ToLowerInvariant(),
                    item.Amount.Amount.ToString("0.0000", CultureInfo.InvariantCulture),
                    item.Amount.Currency.ToString(),
                    item.AccountId.ToString("D"),
                    ProtectSpreadsheetFormula(accountNames[item.AccountId]),
                    item.CategoryId.ToString("D"),
                    ProtectSpreadsheetFormula(categoryNames[item.CategoryId]),
                    ProtectSpreadsheetFormula(item.Description),
                    item.IsCancelled ? "true" : "false",
                    item.CancelledAtUtc?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty
                };
                await writer.WriteLineAsync(string.Join(',', cells.Select(CsvCell)));
            }
        }

        return new PortableFile(
            $"business-finance-transactions-{DateTime.UtcNow:yyyyMMdd}.csv",
            "text/csv; charset=utf-8",
            stream.ToArray());
    }

    /// <summary>
    /// Cari defterin dökümü: borçlandırmalar ve tahsilatlar tek dosyada.
    /// </summary>
    /// <remarks>
    /// İki kayıt türünün alan listesi bilerek farklıdır (ADR 0014):
    /// borçlandırma kategori ve kapsam taşır, hesap taşımaz; tahsilat hesap
    /// taşır, kategori ve kapsam taşımaz. Boş hücre eksik veri değil, o
    /// kaydın sormadığı sorudur — <c>kind</c> kolonu hangisinin okunacağını
    /// söyler.
    ///
    /// Birleştirme bellekte yapılıyor ve bu, birleşik feed'in tek sorgu
    /// kuralını bozmaz: dışa aktarma sayfalanmış bir okuma değil, kullanıcının
    /// bütün defterinin sınırlı bir dökümüdür; sıralanacak küme zaten
    /// tamamıyla belleğe alınmak zorundadır.
    ///
    /// Dosya <b>geri yüklenemez</b>, işlem CSV'si gibi okumak ve arşivlemek
    /// içindir; veriyi taşımanın tek yolu yedektir.
    /// </remarks>
    public async Task<PortableFile> ExportCounterpartyLedgerCsvAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var counterpartyNames = await dbContext.Counterparties.AsNoTracking()
            .Where(item => item.UserId == userId)
            .ToDictionaryAsync(item => item.Id, item => item.Name, cancellationToken);
        var accountNames = await dbContext.Accounts.AsNoTracking()
            .Where(item => item.UserId == userId)
            .ToDictionaryAsync(item => item.Id, item => item.Name, cancellationToken);
        var categoryNames = await dbContext.Categories.AsNoTracking()
            .Where(item => item.UserId == userId)
            .ToDictionaryAsync(item => item.Id, item => item.Name, cancellationToken);
        var charges = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(item => item.UserId == userId).ToArrayAsync(cancellationToken);
        var payments = await dbContext.CounterpartyPayments.AsNoTracking()
            .Where(item => item.UserId == userId).ToArrayAsync(cancellationToken);

        var rows = charges
            .Select(item => new CounterpartyLedgerRow(
                item.Id, item.ChargeDate, "charge", item.Direction, item.Amount,
                item.CounterpartyId, item.CategoryId, null, item.Scope, item.Description,
                item.IsCancelled, item.CancelledAtUtc))
            .Concat(payments.Select(item => new CounterpartyLedgerRow(
                item.Id, item.PaymentDate, "payment", item.Direction, item.Amount,
                item.CounterpartyId, null, item.AccountId, null, item.Description,
                item.IsCancelled, item.CancelledAtUtc)))
            .OrderBy(item => item.Date)
            .ThenBy(item => item.Id)
            .ToArray();

        using var stream = new MemoryStream();
        await stream.WriteAsync(new byte[] { 0xEF, 0xBB, 0xBF }, cancellationToken);
        await using (var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true))
        {
            await writer.WriteLineAsync(
                "id,date,kind,direction,amount,currency,counterpartyId,counterpartyName," +
                "categoryId,categoryName,accountId,accountName,scope,description," +
                "isCancelled,cancelledAtUtc");
            foreach (var item in rows)
            {
                var cells = new[]
                {
                    item.Id.ToString("D"),
                    item.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    item.Kind,
                    item.Direction.ToString().ToLowerInvariant(),
                    item.Amount.Amount.ToString("0.0000", CultureInfo.InvariantCulture),
                    item.Amount.Currency.ToString(),
                    item.CounterpartyId.ToString("D"),
                    ProtectSpreadsheetFormula(counterpartyNames[item.CounterpartyId]),
                    item.CategoryId?.ToString("D") ?? string.Empty,
                    item.CategoryId is Guid categoryId
                        ? ProtectSpreadsheetFormula(categoryNames[categoryId])
                        : string.Empty,
                    item.AccountId?.ToString("D") ?? string.Empty,
                    item.AccountId is Guid accountId
                        ? ProtectSpreadsheetFormula(accountNames[accountId])
                        : string.Empty,
                    item.Scope?.ToString().ToLowerInvariant() ?? string.Empty,
                    ProtectSpreadsheetFormula(item.Description),
                    item.IsCancelled ? "true" : "false",
                    item.CancelledAtUtc?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty
                };
                await writer.WriteLineAsync(string.Join(',', cells.Select(CsvCell)));
            }
        }

        return new PortableFile(
            $"business-finance-counterparty-ledger-{DateTime.UtcNow:yyyyMMdd}.csv",
            "text/csv; charset=utf-8",
            stream.ToArray());
    }

    public async Task<PortableFile> ExportFinancialJsonAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var snapshot = await BuildSnapshotAsync(userId, cancellationToken);
        return new PortableFile(
            $"business-finance-financial-data-{DateTime.UtcNow:yyyyMMdd}.json",
            "application/json",
            SerializeBounded(snapshot));
    }

    public async Task<PortableFile> CreateBackupAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var snapshot = await BuildSnapshotAsync(userId, cancellationToken);
        var payload = SerializeBounded(snapshot);
        var envelope = new BackupEnvelope(
            BackupFormat,
            SchemaVersion,
            DateTimeOffset.UtcNow,
            "base64+utf8-json",
            payload.Length,
            Convert.ToHexStringLower(SHA256.HashData(payload)),
            Convert.ToBase64String(payload));
        return new PortableFile(
            $"business-finance-backup-v{SchemaVersion}-{DateTime.UtcNow:yyyyMMddHHmmss}.bfbackup.json",
            BackupContentType,
            JsonSerializer.SerializeToUtf8Bytes(envelope, JsonOptions));
    }

    public Task<BackupValidationDto> ValidateBackupAsync(
        byte[] content,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var parsed = ParseAndValidate(content);
        _ = BuildRestoredGraph(Guid.NewGuid(), parsed.Snapshot);
        return Task.FromResult(new BackupValidationDto(
            // The version of the file being validated, not the version this build
            // writes. The two are the same today — only v7 is accepted — but
            // reporting the constant would start lying the moment a second version
            // becomes readable, and the summary is what the user confirms against.
            parsed.Envelope.SchemaVersion,
            parsed.Envelope.CreatedAtUtc,
            parsed.Snapshot.EntityCount,
            parsed.Envelope.PayloadSha256));
    }

    public async Task<RestoreSummaryDto> RestoreBackupAsync(
        Guid userId,
        byte[] content,
        DateTimeOffset restoredAtUtc,
        CancellationToken cancellationToken)
    {
        var parsed = ParseAndValidate(content);
        var graph = BuildRestoredGraph(userId, parsed.Snapshot);
        var writtenObjectKeys = new List<string>();
        await using var transaction = dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, cancellationToken)
            : null;
        try
        {
            var replaceableDefaultCategories =
                await FindReplaceableDefaultCategoriesAsync(userId, cancellationToken);
            if (replaceableDefaultCategories is null)
                throw new DataPortabilityException(
                    "restore.destination_not_empty",
                    "Geri yükleme için hesapta finansal veri bulunmamalıdır.",
                    ApplicationErrorType.Conflict);

            dbContext.Categories.RemoveRange(replaceableDefaultCategories);
            dbContext.Accounts.AddRange(graph.Accounts);
            dbContext.Categories.AddRange(graph.Categories);
            dbContext.Transactions.AddRange(graph.Transactions);
            dbContext.MonthlyBudgets.AddRange(graph.Budgets);
            dbContext.Transfers.AddRange(graph.Transfers);
            dbContext.CreditCards.AddRange(graph.Cards);
            dbContext.CreditCardCharges.AddRange(graph.Charges);
            dbContext.CreditCardPayments.AddRange(graph.Payments);
            dbContext.InstallmentPlans.AddRange(graph.InstallmentPlans);
            dbContext.RecurringTransactions.AddRange(graph.RecurringTransactions);
            dbContext.RecurringTransactionOccurrences.AddRange(graph.Occurrences);
            dbContext.ImportBatches.AddRange(graph.ImportBatches);
            dbContext.Counterparties.AddRange(graph.Counterparties);
            dbContext.CounterpartyCharges.AddRange(graph.CounterpartyCharges);
            dbContext.CounterpartyPayments.AddRange(graph.CounterpartyPayments);
            // Kapanış yükümlülüğün navigasyonundan geliyor; ayrı eklenmesi
            // sahipsiz bir settlement yazma yolu açardı.
            dbContext.Obligations.AddRange(graph.Obligations);
            // Sayım bir gözlemdir, farkı yazan hareket ise ayrı bir kayıt:
            // sayım o harekete kimlikle bağlanır, onu içermez.
            dbContext.CashCounts.AddRange(graph.CashCounts);
            dbContext.PosSettlements.AddRange(graph.PosSettlements);
            dbContext.DebtAgreements.AddRange(graph.Debts);
            dbContext.SavingsGoals.AddRange(graph.Goals);
            if (graph.Attachments.Length > 0 && attachmentStore is null)
                throw Invalid("Attachment storage is unavailable.");
            foreach (var item in graph.Attachments)
            {
                await attachmentStore!.WriteAsync(
                    item.Metadata.ObjectKey, item.Content, cancellationToken);
                writtenObjectKeys.Add(item.Metadata.ObjectKey);
            }
            dbContext.FinancialAttachments.AddRange(graph.Attachments.Select(x => x.Metadata));
            await dbContext.SaveChangesAsync(cancellationToken);
            if (transaction is not null) await transaction.CommitAsync(cancellationToken);
            return new RestoreSummaryDto(
                parsed.Envelope.SchemaVersion, parsed.Snapshot.EntityCount, restoredAtUtc);
        }
        catch
        {
            if (transaction is not null) await transaction.RollbackAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();
            if (attachmentStore is not null)
            {
                foreach (var objectKey in writtenObjectKeys)
                    await attachmentStore.DeleteIfExistsAsync(objectKey, CancellationToken.None);
            }
            throw;
        }
    }

    private async Task<FinancialSnapshot> BuildSnapshotAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var accounts = await dbContext.Accounts.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var categories = await dbContext.Categories.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var transactions = await dbContext.Transactions.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var budgets = await dbContext.MonthlyBudgets.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var transfers = await dbContext.Transfers.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var cards = await dbContext.CreditCards.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var charges = await dbContext.CreditCardCharges.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var payments = await dbContext.CreditCardPayments.AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var plans = await dbContext.InstallmentPlans.AsNoTracking().Include(x => x.Items)
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var recurring = await dbContext.RecurringTransactions.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var occurrences = await dbContext.RecurringTransactionOccurrences.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var batches = await dbContext.ImportBatches.AsNoTracking().Include(x => x.Rows)
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var debts = await dbContext.DebtAgreements.AsNoTracking().Include(x => x.Installments)
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);

        // Karşı taraf artık kendi kaydıyla giriyor (v7): adı, notu ve aktifliği
        // yedeğin taşıdığı bilgidir. Sözleşme onu adla değil kimlikle
        // gösteriyor; ad yedeğin içinde tek yerde durur ve iki kayıt aynı
        // kişiyi anlatamaz.
        var counterparties = await dbContext.Counterparties.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var counterpartyCharges = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var counterpartyPayments = await dbContext.CounterpartyPayments.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);

        // Yükümlülük ve onu kapatan nakit hareketi tek okumada geliyor (v8):
        // kapanış yükümlülüğün kendi kaydıdır, ayrı bir koleksiyon olarak
        // yazılsaydı dosyada sahipsiz bir ödeme durabilirdi.
        var obligations = await dbContext.Obligations.AsNoTracking().Include(x => x.Settlement)
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        // Kasa sayımı ve POS tahsilatı (v9). Sayımın beklenen tutarı ve farkı,
        // tahsilatın net tutarı ve komisyon oranı dosyaya yazılmaz: hiçbiri
        // kalıcı alan değil, okunduğu anda türetilen değerlerdir.
        var cashCounts = await dbContext.CashCounts.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var posSettlements = await dbContext.PosSettlements.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var goals = await dbContext.SavingsGoals.AsNoTracking().Include(x => x.Contributions)
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var attachments = await dbContext.FinancialAttachments.AsNoTracking()
            .Where(x => x.UserId == userId).OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        var attachmentBackups = new List<AttachmentBackup>(attachments.Length);
        foreach (var item in attachments)
        {
            if (attachmentStore is null)
                throw new DataPortabilityException(
                    "backup.attachment_storage_unavailable", "Attachment storage is unavailable.");
            await using var content = await attachmentStore.OpenReadAsync(item.ObjectKey, cancellationToken)
                ?? throw new DataPortabilityException(
                    "backup.attachment_missing", $"Attachment '{item.Id}' content is missing.");
            using var buffer = new MemoryStream();
            await content.CopyToAsync(buffer, cancellationToken);
            if (buffer.Length != item.SizeBytes || buffer.Length > FinancialAttachment.MaximumSizeBytes)
                throw new DataPortabilityException(
                    "backup.attachment_invalid", $"Attachment '{item.Id}' size is inconsistent.");
            attachmentBackups.Add(new AttachmentBackup(
                item.Id, item.TransactionId, item.OriginalFileName, item.ContentType,
                item.SizeBytes, item.Sha256, item.CreatedAtUtc,
                Convert.ToBase64String(buffer.ToArray())));
        }

        var snapshot = new FinancialSnapshot(
            accounts.Select(x => new AccountBackup(x.Id, x.Name, x.Type, x.Currency, x.OpeningBalance,
                x.IsActive, x.DefaultScope)).ToArray(),
            categories.Select(x => new CategoryBackup(x.Id, x.Name, x.Type, x.IsActive, x.DefaultScope,
                x.DefaultIsTaxDeductible)).ToArray(),
            transactions.Select(x => new TransactionBackup(x.Id, x.AccountId, x.CategoryId, x.Amount.Amount,
                x.Amount.Currency, x.Type, x.Scope, x.TransactionDate, x.Description, x.IsCancelled,
                x.CancelledAtUtc, x.Vat?.Rate, x.Vat?.Amount, x.IsTaxDeductible)).ToArray(),
            budgets.Select(x => new BudgetBackup(x.Id, x.CategoryId, x.Limit.Amount, x.Limit.Currency,
                x.Scope, x.Year, x.Month)).ToArray(),
            transfers.Select(x => new TransferBackup(x.Id, x.SourceAccountId, x.DestinationAccountId,
                x.Amount.Amount, x.Amount.Currency, x.TransferDate, x.Description, x.IsCancelled, x.CancelledAtUtc)).ToArray(),
            cards.Select(x => new CardBackup(x.Id, x.Name, x.Limit.Amount, x.Limit.Currency,
                x.StatementClosingDay, x.PaymentDueDay, x.IsActive, x.MinimumPaymentRate, x.DefaultScope)).ToArray(),
            charges.Select(x => new ChargeBackup(x.Id, x.CreditCardId, x.CategoryId, x.Amount.Amount,
                x.Amount.Currency, x.Scope, x.ChargeDate, x.Description, x.IsCancelled, x.CancelledAtUtc,
                x.Vat?.Rate, x.Vat?.Amount, x.IsTaxDeductible)).ToArray(),
            payments.Select(x => new PaymentBackup(x.Id, x.AccountId, x.CreditCardId, x.Amount.Amount,
                x.Amount.Currency, x.PaymentDate, x.Description, x.IsCancelled, x.CancelledAtUtc)).ToArray(),
            plans.Select(x => new InstallmentPlanBackup(x.Id, x.CreditCardId, x.CategoryId, x.ClientRequestId,
                x.TotalAmount.Amount, x.TotalAmount.Currency, x.Scope, x.InstallmentCount, x.FirstInstallmentDate,
                x.Description, x.Items.OrderBy(i => i.Sequence).Select(i => new InstallmentItemBackup(
                    i.Id, i.Sequence, i.Amount.Amount, i.Amount.Currency, i.ScheduledDate,
                    i.CreditCardChargeId, i.RealizedAtUtc)).ToArray())).ToArray(),
            recurring.Select(x => new RecurringBackup(x.Id, x.AccountId, x.CategoryId, x.Amount.Amount,
                x.Amount.Currency, x.Kind, x.Scope, x.Frequency, x.StartDate, x.EndDate, x.NextOccurrenceDate,
                x.MonthEndBehavior, x.Description, x.IsActive,
                occurrences.Where(o => o.RecurringTransactionId == x.Id).OrderBy(o => o.ScheduledDate)
                    .Select(o => new OccurrenceBackup(o.Id, o.ScheduledDate, o.BudgetTransactionId,
                        o.RealizedAtUtc, o.CreditCardChargeId)).ToArray(),
                x.SourceType, x.OccurrenceLimit, x.GeneratedOccurrenceCount, x.CreditCardId)).ToArray(),
            batches.Select(x => new ImportBatchBackup(x.Id, x.FileName, x.FileFingerprint, x.FileSizeBytes,
                x.EncodingName, x.Delimiter, x.DateColumn, x.AmountColumn, x.DescriptionColumn,
                x.ReferenceColumn, x.DateFormat, x.DecimalSeparator, x.Status, x.CreatedAtUtc,
                x.Rows.OrderBy(r => r.RowNumber).Select(r => new ImportRowBackup(
                    r.Id, r.RowNumber, r.RawData, r.TransactionDate, r.SignedAmount, r.Currency,
                    r.Description, r.ExternalReference, r.AccountId, r.CategoryId, r.Status,
                    r.ErrorMessage, r.BudgetTransactionId, r.DuplicateTransactionId, r.DuplicateReason)).ToArray())).ToArray(),
            counterparties.Select(x => new CounterpartyBackup(
                x.Id, x.Name, x.Note, x.IsActive)).ToArray(),
            counterpartyCharges.Select(x => new CounterpartyChargeBackup(
                x.Id, x.CounterpartyId, x.CategoryId, x.Direction, x.Amount.Amount, x.Amount.Currency,
                x.Scope, x.ChargeDate, x.Description, x.IsCancelled, x.CancelledAtUtc, x.DueDate,
                x.Vat?.Rate, x.Vat?.Amount, x.IsTaxDeductible)).ToArray(),
            counterpartyPayments.Select(x => new CounterpartyPaymentBackup(
                x.Id, x.CounterpartyId, x.AccountId, x.Direction, x.Amount.Amount, x.Amount.Currency,
                x.PaymentDate, x.Description, x.IsCancelled, x.CancelledAtUtc)).ToArray(),
            obligations.Select(x => new ObligationBackup(
                x.Id, x.CounterpartyId, x.CategoryId, x.Direction, x.Amount.Amount, x.Amount.Currency,
                x.Scope, x.IssueDate, x.DueDate, x.Description, x.CreatedAtUtc,
                x.IsCancelled, x.CancelledAtUtc,
                x.Settlement is null
                    ? null
                    : new ObligationSettlementBackup(
                        x.Settlement.Id, x.Settlement.AccountId, x.Settlement.Amount.Amount,
                        x.Settlement.Amount.Currency, x.Settlement.SettlementDate,
                        x.Settlement.SettledAtUtc),
                x.Vat?.Rate, x.Vat?.Amount, x.IsTaxDeductible)).ToArray(),
            cashCounts.Select(x => new CashCountBackup(
                x.Id, x.AccountId, x.CountedAmount, x.Currency, x.Scope, x.CountDate,
                x.Note, x.CreatedAtUtc, x.AdjustmentTransactionId, x.AdjustedAtUtc,
                x.IsCancelled, x.CancelledAtUtc)).ToArray(),
            posSettlements.Select(x => new PosSettlementBackup(
                x.Id, x.AccountId, x.CategoryId, x.CommissionCategoryId,
                x.GrossAmount.Amount, x.CommissionAmount, x.Currency, x.Scope,
                x.SettlementDate, x.ExpectedTransferDate, x.Description, x.CreatedAtUtc,
                x.TransferredOn, x.TransferredAtUtc, x.IsCancelled, x.CancelledAtUtc,
                x.Vat?.Rate, x.Vat?.Amount)).ToArray(),
            debts.Select(x => new DebtBackup(
                x.Id, x.CounterpartyId, x.Direction, x.Scope, x.Principal.Amount, x.TotalRepayment.Amount,
                x.Principal.Currency, x.AnnualInterestRate, x.StartDate, x.FirstDueDate,
                x.InstallmentCount, x.Description,
                x.Installments.OrderBy(i => i.Sequence).Select(i => new DebtInstallmentBackup(
                    i.Sequence, i.Amount.Amount, i.Amount.Currency, i.DueDate,
                    i.PaymentAccountId, i.PaymentDate, i.PaidAtUtc)).ToArray(),
                x.SourceType, x.OpeningAccountId, x.CategoryId)).ToArray(),
            goals.Select(x => new SavingsGoalBackup(
                x.Id, x.Name, x.TargetAmount.Amount, x.TargetAmount.Currency, x.TargetDate,
                x.TrackingMode, x.AccountId, x.Description, x.CreatedAtUtc,
                x.Contributions.OrderBy(c => c.ContributionDate).ThenBy(c => c.Id)
                    .Select(c => new SavingsGoalContributionBackup(
                        c.Id, c.Amount.Amount, c.Amount.Currency, c.ContributionDate,
                        c.ClientRequestId, c.Note, c.CreatedAtUtc)).ToArray(),
                x.Scope)).ToArray(),
            attachmentBackups.ToArray());
        EnsureEntityLimit(snapshot);
        return snapshot;
    }

    private RestoredGraph BuildRestoredGraph(Guid userId, FinancialSnapshot snapshot)
    {
        EnsureEntityLimit(snapshot);
        EnsureUniqueIds(snapshot.Accounts.Select(x => x.Id), "account");
        EnsureUniqueIds(snapshot.Categories.Select(x => x.Id), "category");
        EnsureUniqueIds(snapshot.Transactions.Select(x => x.Id), "transaction");
        EnsureUniqueIds(snapshot.Budgets.Select(x => x.Id), "budget");
        EnsureUniqueIds(snapshot.Transfers.Select(x => x.Id), "transfer");
        EnsureUniqueIds(snapshot.Cards.Select(x => x.Id), "credit card");
        EnsureUniqueIds(snapshot.Charges.Select(x => x.Id), "card charge");
        EnsureUniqueIds(snapshot.Payments.Select(x => x.Id), "card payment");
        EnsureUniqueIds(snapshot.InstallmentPlans.Select(x => x.Id), "installment plan");
        EnsureUniqueIds(snapshot.RecurringTransactions.Select(x => x.Id), "recurring transaction");
        EnsureUniqueIds(snapshot.ImportBatches.Select(x => x.Id), "import batch");
        EnsureUniqueIds(snapshot.Counterparties.Select(x => x.Id), "counterparty");
        EnsureUniqueIds(snapshot.CounterpartyCharges.Select(x => x.Id), "counterparty charge");
        EnsureUniqueIds(snapshot.CounterpartyPayments.Select(x => x.Id), "counterparty payment");
        EnsureUniqueIds(snapshot.Obligations.Select(x => x.Id), "obligation");
        EnsureUniqueIds(
            snapshot.Obligations.Where(x => x.Settlement is not null).Select(x => x.Settlement!.Id),
            "obligation settlement");
        EnsureUniqueIds(snapshot.CashCounts.Select(x => x.Id), "cash count");
        EnsureUniqueIds(snapshot.PosSettlements.Select(x => x.Id), "pos settlement");
        EnsureUniqueIds(snapshot.Debts.Select(x => x.Id), "debt");
        EnsureUniqueIds(snapshot.SavingsGoals.Select(x => x.Id), "savings goal");
        EnsureUniqueIds(snapshot.Attachments.Select(x => x.Id), "attachment");

        try
        {
            var accountMap = snapshot.Accounts.ToDictionary(
                x => x.Id,
                x => new Account(Guid.NewGuid(), userId, x.Name, x.Type, x.Currency, x.OpeningBalance,
                    x.DefaultScope));
            var categoryMap = snapshot.Categories.ToDictionary(
                x => x.Id,
                x => new Category(Guid.NewGuid(), userId, x.Name, x.Type, x.DefaultScope,
                    x.DefaultIsTaxDeductible));
            var cardMap = snapshot.Cards.ToDictionary(
                x => x.Id,
                x => new CreditCard(Guid.NewGuid(), userId, x.Name, MoneyOf(x.Limit, x.Currency),
                    x.StatementClosingDay, x.PaymentDueDay, x.MinimumPaymentRate, x.DefaultScope));

            var transactionMap = new Dictionary<Guid, BudgetTransaction>();
            foreach (var item in snapshot.Transactions)
            {
                var entity = new BudgetTransaction(
                    Guid.NewGuid(), userId,
                    Required(accountMap, item.AccountId, "transaction account"),
                    Required(categoryMap, item.CategoryId, "transaction category"),
                    MoneyOf(item.Amount, item.Currency), item.Type, item.Scope, item.TransactionDate,
                    item.Description, VatOf(item.VatRate, item.VatAmount), item.IsTaxDeductible);
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                transactionMap.Add(item.Id, entity);
            }

            var budgets = snapshot.Budgets.Select(item => new MonthlyBudget(
                Guid.NewGuid(), userId,
                Required(categoryMap, item.CategoryId, "budget category"),
                MoneyOf(item.Limit, item.Currency), item.Scope, item.Year, item.Month)).ToArray();
            var transfers = snapshot.Transfers.Select(item =>
            {
                var entity = new Transfer(
                    Guid.NewGuid(), userId,
                    Required(accountMap, item.SourceAccountId, "transfer source"),
                    Required(accountMap, item.DestinationAccountId, "transfer destination"),
                    MoneyOf(item.Amount, item.Currency), item.TransferDate, item.Description);
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                return entity;
            }).ToArray();

            var chargeMap = new Dictionary<Guid, CreditCardCharge>();
            foreach (var item in snapshot.Charges)
            {
                var entity = new CreditCardCharge(
                    Guid.NewGuid(), userId,
                    Required(cardMap, item.CreditCardId, "charge card"),
                    Required(categoryMap, item.CategoryId, "charge category"),
                    MoneyOf(item.Amount, item.Currency), item.Scope, item.ChargeDate, item.Description,
                    VatOf(item.VatRate, item.VatAmount), item.IsTaxDeductible);
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                chargeMap.Add(item.Id, entity);
            }

            var payments = snapshot.Payments.Select(item =>
            {
                var entity = new CreditCardPayment(
                    Guid.NewGuid(), userId,
                    Required(accountMap, item.AccountId, "payment account"),
                    Required(cardMap, item.CreditCardId, "payment card"),
                    MoneyOf(item.Amount, item.Currency), item.PaymentDate, item.Description);
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                return entity;
            }).ToArray();

            var installmentPlans = new List<InstallmentPlan>();
            foreach (var item in snapshot.InstallmentPlans)
            {
                var entity = new InstallmentPlan(
                    Guid.NewGuid(), userId,
                    Required(cardMap, item.CreditCardId, "installment card"),
                    Required(categoryMap, item.CategoryId, "installment category"),
                    item.ClientRequestId, MoneyOf(item.TotalAmount, item.Currency), item.Scope,
                    item.InstallmentCount, item.FirstInstallmentDate, item.Description);
                if (item.Items.Length != entity.Items.Count)
                    throw Invalid("Installment item count does not match its plan.");
                foreach (var sourceItem in item.Items)
                {
                    var targetItem = entity.GetItem(sourceItem.Sequence);
                    if (targetItem.Amount != MoneyOf(sourceItem.Amount, sourceItem.Currency) ||
                        targetItem.ScheduledDate != sourceItem.ScheduledDate)
                        throw Invalid("Installment schedule does not match the generated plan.");
                    if ((sourceItem.CreditCardChargeId is null) != (sourceItem.RealizedAtUtc is null))
                        throw Invalid("Installment realization fields must appear together.");
                    if (sourceItem.CreditCardChargeId is Guid chargeId)
                        targetItem.Realize(
                            Required(chargeMap, chargeId, "installment charge").Id,
                            sourceItem.RealizedAtUtc!.Value);
                }
                installmentPlans.Add(entity);
            }

            var recurringTransactions = new List<RecurringTransaction>();
            var occurrences = new List<RecurringTransactionOccurrence>();
            foreach (var item in snapshot.RecurringTransactions)
            {
                var sourceType = item.SourceType;
                if (sourceType == RecurringSourceType.Account
                    ? item.AccountId is null || item.CreditCardId is not null
                    : item.CreditCardId is null || item.AccountId is not null)
                {
                    throw Invalid("Recurring source must be exactly one account or credit card.");
                }

                var amount = MoneyOf(item.Amount, item.Currency);
                var category = Required(categoryMap, item.CategoryId, "recurring category");
                var entity = sourceType == RecurringSourceType.CreditCard
                    ? new RecurringTransaction(
                        Guid.NewGuid(), userId,
                        Required(cardMap, item.CreditCardId!.Value, "recurring credit card"),
                        category, amount, item.Kind, item.Scope, item.Frequency,
                        item.StartDate, item.EndDate, item.MonthEndBehavior, item.Description,
                        item.OccurrenceLimit)
                    : new RecurringTransaction(
                        Guid.NewGuid(), userId,
                        Required(accountMap, item.AccountId!.Value, "recurring account"),
                        category, amount, item.Kind, item.Scope, item.Frequency,
                        item.StartDate, item.EndDate, item.MonthEndBehavior, item.Description,
                        item.OccurrenceLimit);
                foreach (var occurrence in item.Occurrences.OrderBy(x => x.ScheduledDate))
                {
                    if (entity.NextOccurrenceDate != occurrence.ScheduledDate)
                        throw Invalid("Recurring occurrence sequence is not contiguous.");
                    var restoredOccurrence = RecurringTransactionOccurrence.Create(
                        Guid.NewGuid(), entity, occurrence.ScheduledDate);
                    var hasResult = occurrence.BudgetTransactionId is not null ||
                                    occurrence.CreditCardChargeId is not null;
                    if (hasResult != (occurrence.RealizedAtUtc is not null))
                        throw Invalid("Recurring realization fields must appear together.");
                    if (occurrence.BudgetTransactionId is not null &&
                        occurrence.CreditCardChargeId is not null)
                    {
                        throw Invalid("A recurring occurrence cannot carry two realization results.");
                    }

                    if (occurrence.BudgetTransactionId is Guid transactionId)
                        restoredOccurrence.RealizeWithTransaction(
                            Required(transactionMap, transactionId, "occurrence transaction").Id,
                            occurrence.RealizedAtUtc!.Value);
                    if (occurrence.CreditCardChargeId is Guid restoredChargeId)
                        restoredOccurrence.RealizeWithCharge(
                            Required(chargeMap, restoredChargeId, "occurrence charge").Id,
                            occurrence.RealizedAtUtc!.Value);
                    occurrences.Add(restoredOccurrence);
                    entity.AdvanceAfter(occurrence.ScheduledDate);
                }
                if (entity.NextOccurrenceDate != item.NextOccurrenceDate)
                    throw Invalid("Recurring next date does not match its occurrence history.");
                // Sayaç dosyadan olduğu gibi yazılmıyor, occurrence geçmişi
                // yeniden oynanarak türetiliyor; dosyadaki değer yalnız
                // doğrulama için okunuyor. Aksi hâlde elle değiştirilmiş bir
                // sayaç, sınırı dolmuş bir planı yeniden üretir hâle getirirdi.
                if (entity.GeneratedOccurrenceCount != item.GeneratedOccurrenceCount)
                    throw Invalid("Recurring occurrence count does not match its history.");
                if (!item.IsActive && entity.IsActive) entity.Deactivate();
                if (item.IsActive != entity.IsActive)
                    throw Invalid("Recurring active state is inconsistent.");
                recurringTransactions.Add(entity);
            }

            var importBatches = new List<ImportBatch>();
            foreach (var item in snapshot.ImportBatches)
            {
                if (item.Delimiter.Length != 1 || item.DecimalSeparator.Length != 1)
                    throw Invalid("Import delimiter fields must contain one character.");
                var batch = new ImportBatch(
                    Guid.NewGuid(), userId, item.FileName, item.FileFingerprint, item.FileSizeBytes,
                    item.EncodingName, item.Delimiter[0], item.DateColumn, item.AmountColumn,
                    item.DescriptionColumn, item.ReferenceColumn, item.DateFormat,
                    item.DecimalSeparator[0], item.CreatedAtUtc);
                EnsureUniqueIds(item.Rows.Select(x => x.Id), "import row");
                foreach (var sourceRow in item.Rows.OrderBy(x => x.RowNumber))
                {
                    if (sourceRow.Status is ImportRowStatus.Valid or ImportRowStatus.Invalid)
                    {
                        if (sourceRow.AccountId is not null || sourceRow.CategoryId is not null ||
                            sourceRow.BudgetTransactionId is not null ||
                            sourceRow.DuplicateTransactionId is not null || sourceRow.DuplicateReason is not null)
                            throw Invalid("Unmapped import row cannot carry financial links.");
                    }
                    if ((sourceRow.Status == ImportRowStatus.Imported) !=
                        sourceRow.BudgetTransactionId.HasValue)
                        throw Invalid("Only an imported row can carry its created transaction.");
                    var errors = sourceRow.Status == ImportRowStatus.Invalid
                        ? new[] { sourceRow.ErrorMessage ?? "Invalid imported row." }
                        : Array.Empty<string>();
                    var row = new ImportRow(
                        Guid.NewGuid(), userId, batch.Id, sourceRow.RowNumber, sourceRow.RawData,
                        sourceRow.TransactionDate, sourceRow.SignedAmount, sourceRow.Currency,
                        sourceRow.Description, sourceRow.ExternalReference, errors);
                    if (sourceRow.Status is not ImportRowStatus.Valid and not ImportRowStatus.Invalid)
                    {
                        if (sourceRow.TransactionDate is null || sourceRow.SignedAmount is null ||
                            sourceRow.AccountId is null || sourceRow.CategoryId is null)
                            throw Invalid("Mapped import row is missing required fields.");
                        row.ApplyCorrection(
                            sourceRow.TransactionDate.Value, sourceRow.SignedAmount.Value,
                            sourceRow.Description, sourceRow.ExternalReference,
                            Required(accountMap, sourceRow.AccountId.Value, "import account"),
                            Required(categoryMap, sourceRow.CategoryId.Value, "import category"));
                        if (sourceRow.DuplicateTransactionId is Guid duplicateId)
                        {
                            if (sourceRow.DuplicateReason is null)
                                throw Invalid("Duplicate reason is required with a duplicate transaction.");
                            row.FlagDuplicate(
                                Required(transactionMap, duplicateId, "duplicate transaction").Id,
                                sourceRow.DuplicateReason.Value);
                            if (sourceRow.Status != ImportRowStatus.PendingDuplicateReview)
                                row.ResolveDuplicate(sourceRow.Status != ImportRowStatus.SkippedDuplicate);
                        }
                        else if (sourceRow.DuplicateReason is not null)
                        {
                            throw Invalid("Duplicate transaction is required with a duplicate reason.");
                        }
                        if (sourceRow.Status == ImportRowStatus.Imported)
                        {
                            if (sourceRow.BudgetTransactionId is not Guid transactionId)
                                throw Invalid("Imported row requires a budget transaction.");
                            row.MarkImported(Required(transactionMap, transactionId, "import transaction").Id);
                        }
                    }
                    if (row.Status != sourceRow.Status)
                        throw Invalid("Import row status is inconsistent with its fields.");
                    batch.AddRow(row);
                }
                if (item.Status != ImportBatchStatus.Staged) batch.RecordConfirmation();
                if (batch.Status != item.Status)
                    throw Invalid("Import batch status is inconsistent with its rows.");
                importBatches.Add(batch);
            }

            var counterpartyMap = snapshot.Counterparties.ToDictionary(
                x => x.Id,
                x => new Counterparty(Guid.NewGuid(), userId, x.Name, x.Note));

            // Borçlandırma yalnız aktif karşı tarafa ve kategoriye yazılabilir;
            // pasifleştirme, hesap ve kategorilerde olduğu gibi graph kurulduktan
            // sonra uygulanıyor. Sırayı ters kurmak, pasif bir müşterinin
            // geçmişini geri yüklenemez yapardı.
            var counterpartyCharges = new List<CounterpartyCharge>();
            foreach (var item in snapshot.CounterpartyCharges)
            {
                var entity = new CounterpartyCharge(
                    Guid.NewGuid(), userId,
                    Required(counterpartyMap, item.CounterpartyId, "charge counterparty"),
                    Required(categoryMap, item.CategoryId, "counterparty charge category"),
                    item.Direction, MoneyOf(item.Amount, item.Currency), item.Scope,
                    item.ChargeDate, item.Description, item.DueDate,
                    VatOf(item.VatRate, item.VatAmount), item.IsTaxDeductible);
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                counterpartyCharges.Add(entity);
            }

            var counterpartyPayments = new List<CounterpartyPayment>();
            foreach (var item in snapshot.CounterpartyPayments)
            {
                var entity = new CounterpartyPayment(
                    Guid.NewGuid(), userId,
                    Required(counterpartyMap, item.CounterpartyId, "payment counterparty"),
                    Required(accountMap, item.AccountId, "counterparty payment account"),
                    item.Direction, MoneyOf(item.Amount, item.Currency),
                    item.PaymentDate, item.Description);
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                counterpartyPayments.Add(entity);
            }

            // Yükümlülük ekonomik olayı tanır, kapanışı nakdi taşır (ADR 0014).
            // Kapanış bu yüzden yükümlülüğün kendi metodundan doğuyor: tutarı
            // ve yönü dosyadan yeniden okunsaydı, kapanış yükümlülükten farklı
            // bir para taşıyabilirdi.
            var obligations = new List<Obligation>();
            foreach (var item in snapshot.Obligations)
            {
                var entity = new Obligation(
                    Guid.NewGuid(), userId,
                    Required(categoryMap, item.CategoryId, "obligation category"),
                    item.Direction, MoneyOf(item.Amount, item.Currency), item.Scope,
                    item.IssueDate, item.DueDate, item.CreatedAtUtc,
                    item.CounterpartyId is Guid obligationCounterpartyId
                        ? Required(counterpartyMap, obligationCounterpartyId, "obligation counterparty")
                        : null,
                    item.Description, VatOf(item.VatRate, item.VatAmount), item.IsTaxDeductible);
                if (item.Settlement is ObligationSettlementBackup settlement)
                {
                    if (MoneyOf(settlement.Amount, settlement.Currency) != entity.Amount)
                        throw Invalid("Obligation settlement amount does not match its obligation.");
                    entity.Settle(
                        Guid.NewGuid(),
                        Required(accountMap, settlement.AccountId, "obligation settlement account"),
                        settlement.SettlementDate, settlement.SettledAtUtc);
                }
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                obligations.Add(entity);
            }

            // Sayım hesabın bakiyesine dokunmaz; geri yüklenirken de dokunmaz.
            // Beklenen tutar ve fark dosyada olmadığı için burada yeniden
            // hesaplanmaz: ikisi de sayım okunduğu anda hesabın kendi
            // bakiyesinden türer. Farkı onaylanmış sayım, o farkı yazan
            // harekete kimlikle yeniden bağlanıyor - hareket zaten yukarıda
            // yeni kimliğiyle kuruldu, dosyadaki eskisiyle değil.
            var cashCounts = new List<CashCount>();
            foreach (var item in snapshot.CashCounts)
            {
                var account = Required(accountMap, item.AccountId, "cash count account");
                var entity = new CashCount(
                    Guid.NewGuid(), userId, account, item.CountedAmount, item.Scope,
                    item.CountDate, item.CreatedAtUtc, item.Note);
                if (item.AdjustmentTransactionId is Guid adjustmentId)
                {
                    if (item.AdjustedAtUtc is not DateTimeOffset adjustedAtUtc)
                        throw Invalid("Cash count adjustment is missing its timestamp.");
                    var adjustment = Required(transactionMap, adjustmentId, "cash count adjustment");
                    entity.RecordAdjustment(adjustment.Id, adjustedAtUtc);
                }
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                cashCounts.Add(entity);
            }

            // Tahsilat geliri tanır, geçiş parayı taşır (ADR 0014). Geçiş
            // dosyadaki günle yeniden işaretleniyor; hesaba giren net tutar
            // brüt ile komisyondan çözüldüğü için dosyadan okunmuyor.
            var posSettlements = new List<PosSettlement>();
            foreach (var item in snapshot.PosSettlements)
            {
                var entity = new PosSettlement(
                    Guid.NewGuid(), userId,
                    Required(accountMap, item.AccountId, "pos settlement account"),
                    Required(categoryMap, item.CategoryId, "pos settlement category"),
                    MoneyOf(item.GrossAmount, item.Currency), item.CommissionAmount, item.Scope,
                    item.SettlementDate, item.ExpectedTransferDate, item.CreatedAtUtc,
                    item.CommissionCategoryId is Guid commissionCategoryId
                        ? Required(categoryMap, commissionCategoryId, "pos commission category")
                        : null,
                    item.Description, VatOf(item.VatRate, item.VatAmount));
                if (item.TransferredOn is DateOnly transferredOn)
                {
                    if (item.TransferredAtUtc is not DateTimeOffset transferredAtUtc)
                        throw Invalid("Pos settlement transfer is missing its timestamp.");
                    entity.MarkTransferred(transferredOn, transferredAtUtc);
                }
                ApplyCancellation(item.IsCancelled, item.CancelledAtUtc, entity.Cancel);
                posSettlements.Add(entity);
            }

            var debts = new List<DebtAgreement>();
            foreach (var item in snapshot.Debts)
            {
                var counterparty = Required(counterpartyMap, item.CounterpartyId, "debt counterparty");
                // Açılışı kayıtsız borç yedekte de kayıtsız kalır; kullanıcı
                // uygulamada tamamlar. Yedekteki `AnnualInterestRate` okunmuyor:
                // oran artık paradan çözülüyor ve yedekteki değer hiçbir hesaba
                // girmemiş, serbestçe yazılmış bir sayıydı.
                var debt = item.SourceType != DebtSourceType.Unrecorded
                    ? new DebtAgreement(
                        Guid.NewGuid(), userId, counterparty, item.Direction, item.Scope,
                        MoneyOf(item.Principal, item.Currency), MoneyOf(item.TotalRepayment, item.Currency),
                        item.SourceType,
                        item.OpeningAccountId is Guid openingAccountId
                            ? Required(accountMap, openingAccountId, "debt opening account")
                            : null,
                        item.CategoryId is Guid debtCategoryId
                            ? Required(categoryMap, debtCategoryId, "debt category")
                            : null,
                        item.StartDate, item.FirstDueDate, item.InstallmentCount, item.Description)
                    : DebtAgreement.WithUnrecordedOpening(
                        Guid.NewGuid(), userId, counterparty, item.Direction, item.Scope,
                        MoneyOf(item.Principal, item.Currency), MoneyOf(item.TotalRepayment, item.Currency),
                        item.StartDate, item.FirstDueDate, item.InstallmentCount, item.Description);
                if (debt.Installments.Count != item.Installments.Length)
                    throw Invalid("Debt installment count does not match its schedule.");
                foreach (var sourceInstallment in item.Installments)
                {
                    var targetInstallment = debt.GetInstallment(sourceInstallment.Sequence);
                    if (targetInstallment.Amount != MoneyOf(sourceInstallment.Amount, sourceInstallment.Currency) ||
                        targetInstallment.DueDate != sourceInstallment.DueDate)
                        throw Invalid("Debt installment does not match the generated schedule.");
                    var paymentFields = sourceInstallment.PaymentAccountId.HasValue &&
                                        sourceInstallment.PaymentDate.HasValue &&
                                        sourceInstallment.PaidAtUtc.HasValue;
                    if (paymentFields != (sourceInstallment.PaymentAccountId.HasValue ||
                                          sourceInstallment.PaymentDate.HasValue ||
                                          sourceInstallment.PaidAtUtc.HasValue))
                        throw Invalid("Debt payment fields must appear together.");
                    if (paymentFields)
                        targetInstallment.MarkPaid(
                            Required(accountMap, sourceInstallment.PaymentAccountId!.Value,
                                "debt payment account"),
                            sourceInstallment.PaymentDate!.Value,
                            sourceInstallment.PaidAtUtc!.Value);
                }
                debts.Add(debt);
            }

            var goals = new List<SavingsGoal>();
            foreach (var item in snapshot.SavingsGoals)
            {
                Guid? accountId = item.AccountId is Guid sourceAccountId
                    ? Required(accountMap, sourceAccountId, "savings goal account").Id
                    : null;
                var goal = new SavingsGoal(
                    Guid.NewGuid(), userId, item.Name, MoneyOf(item.TargetAmount, item.Currency),
                    item.TargetDate, item.TrackingMode, accountId, item.CreatedAtUtc, item.Description,
                    item.Scope);
                EnsureUniqueIds(item.Contributions.Select(x => x.Id), "savings goal contribution");
                foreach (var contribution in item.Contributions)
                    goal.AddContribution(
                        Guid.NewGuid(), MoneyOf(contribution.Amount, contribution.Currency),
                        contribution.ContributionDate, contribution.ClientRequestId,
                        contribution.CreatedAtUtc, contribution.Note);
                goals.Add(goal);
            }

            var restoredAttachments = new List<RestoredAttachment>();
            foreach (var item in snapshot.Attachments)
            {
                byte[] bytes;
                try { bytes = Convert.FromBase64String(item.ContentBase64); }
                catch (FormatException) { throw Invalid("Attachment content is not valid base64."); }
                if (bytes.LongLength != item.SizeBytes || bytes.LongLength > FinancialAttachment.MaximumSizeBytes)
                    throw Invalid("Attachment content size is inconsistent.");
                var hash = Convert.ToHexStringLower(SHA256.HashData(bytes));
                if (!CryptographicOperations.FixedTimeEquals(
                        Encoding.ASCII.GetBytes(hash), Encoding.ASCII.GetBytes(item.Sha256)))
                    throw Invalid("Attachment content hash is inconsistent.");
                if (attachmentInspector is null)
                    throw Invalid("Attachment inspection is unavailable.");
                var inspection = attachmentInspector.Inspect(
                    item.OriginalFileName, item.ContentType, bytes);
                if (!inspection.IsAccepted || inspection.Sha256 != item.Sha256)
                    throw Invalid("Attachment content failed security inspection.");
                var id = Guid.NewGuid();
                var transaction = Required(transactionMap, item.TransactionId, "attachment transaction");
                var metadata = new FinancialAttachment(
                    id, userId, transaction.Id, item.OriginalFileName, item.ContentType,
                    item.SizeBytes, item.Sha256,
                    $"attachments/{userId:N}/{id:N}{inspection.Extension}", item.CreatedAtUtc);
                restoredAttachments.Add(new RestoredAttachment(metadata, bytes));
            }

            foreach (var source in snapshot.Counterparties.Where(x => !x.IsActive))
                counterpartyMap[source.Id].Deactivate();
            foreach (var source in snapshot.Accounts.Where(x => !x.IsActive)) accountMap[source.Id].Deactivate();
            foreach (var source in snapshot.Categories.Where(x => !x.IsActive)) categoryMap[source.Id].Deactivate();
            foreach (var source in snapshot.Cards.Where(x => !x.IsActive))
            {
                var card = cardMap[source.Id];
                card.Update(
                    card.Name, card.Limit, card.StatementClosingDay, card.PaymentDueDay,
                    card.MinimumPaymentRate, false);
            }

            return new RestoredGraph(
                accountMap.Values.ToArray(), categoryMap.Values.ToArray(), transactionMap.Values.ToArray(),
                budgets, transfers, cardMap.Values.ToArray(), chargeMap.Values.ToArray(), payments,
                installmentPlans.ToArray(), recurringTransactions.ToArray(), occurrences.ToArray(),
                importBatches.ToArray(), counterpartyMap.Values.ToArray(),
                counterpartyCharges.ToArray(), counterpartyPayments.ToArray(),
                obligations.ToArray(), cashCounts.ToArray(), posSettlements.ToArray(),
                debts.ToArray(), goals.ToArray(), restoredAttachments.ToArray());
        }
        catch (DataPortabilityException)
        {
            throw;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            throw Invalid($"Backup domain validation failed: {exception.Message}");
        }
    }

    private async Task<Category[]?> FindReplaceableDefaultCategoriesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var hasUserData =
            await dbContext.Accounts.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.Transactions.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.MonthlyBudgets.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.Transfers.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.CreditCards.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.CreditCardCharges.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.CreditCardPayments.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.InstallmentPlans.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.RecurringTransactions.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.ImportBatches.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.Counterparties.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.CounterpartyCharges.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.CounterpartyPayments.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.Obligations.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.CashCounts.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.PosSettlements.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.DebtAgreements.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.SavingsGoals.AnyAsync(x => x.UserId == userId, cancellationToken) ||
            await dbContext.FinancialAttachments.AnyAsync(x => x.UserId == userId, cancellationToken);
        if (hasUserData) return null;

        var categories = await dbContext.Categories
            .Where(category => category.UserId == userId)
            .ToArrayAsync(cancellationToken);
        return categories.Length == 0 || EfCategoryRepository.IsPristineDefaultSet(categories)
            ? categories
            : null;
    }

    private static ParsedBackup ParseAndValidate(byte[] content)
    {
        if (content.Length == 0 || content.Length > 14 * 1024 * 1024)
            throw new DataPortabilityException("backup.file_too_large", "Backup envelope size is invalid.");
        try
        {
            RejectDuplicateProperties(content);
            var envelope = JsonSerializer.Deserialize<BackupEnvelope>(content, JsonOptions)
                ?? throw Invalid("Backup envelope is empty.");
            if (envelope.Format != BackupFormat)
                throw Invalid("Backup format is not supported.");
            if (!SupportedSchemaVersions.Contains(envelope.SchemaVersion))
                throw new DataPortabilityException(
                    "restore.unsupported_version",
                    $"Backup schema version {envelope.SchemaVersion} is not supported.");
            if (envelope.CreatedAtUtc.Offset != TimeSpan.Zero)
                throw Invalid("Backup creation time must be UTC.");
            if (envelope.PayloadEncoding != "base64+utf8-json")
                throw Invalid("Backup payload encoding is not supported.");
            if (string.IsNullOrWhiteSpace(envelope.Payload) ||
                string.IsNullOrWhiteSpace(envelope.PayloadSha256))
                throw Invalid("Backup payload and SHA-256 are required.");

            byte[] payload;
            try { payload = Convert.FromBase64String(envelope.Payload); }
            catch (FormatException) { throw Invalid("Backup payload is not valid base64."); }
            if (payload.Length == 0 || payload.Length > MaximumPayloadBytes ||
                payload.Length != envelope.PayloadLength)
                throw new DataPortabilityException(
                    "restore.integrity_failed",
                    "Yedek veri uzunluğu dosya zarfındaki bilgiyle eşleşmiyor.");

            byte[] expectedHash;
            try { expectedHash = Convert.FromHexString(envelope.PayloadSha256); }
            catch (FormatException) { throw Invalid("Backup SHA-256 value is invalid."); }
            var actualHash = SHA256.HashData(payload);
            if (expectedHash.Length != actualHash.Length ||
                !CryptographicOperations.FixedTimeEquals(expectedHash, actualHash))
                throw new DataPortabilityException(
                    "restore.integrity_failed",
                    "Yedek veri bütünlüğü doğrulanamadı.");

            RejectDuplicateProperties(payload);
            var snapshot = JsonSerializer.Deserialize<FinancialSnapshot>(payload, JsonOptions)
                ?? throw Invalid("Backup payload is empty.");
            EnsureCollections(snapshot);
            EnsureEntityLimit(snapshot);
            return new ParsedBackup(envelope, snapshot);
        }
        catch (DataPortabilityException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            throw Invalid($"Yedek JSON geçersiz: {exception.Message}");
        }
    }

    private static void RejectDuplicateProperties(byte[] json)
    {
        using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 64 });
        Inspect(document.RootElement);
        static void Inspect(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                var names = new HashSet<string>(StringComparer.Ordinal);
                foreach (var property in element.EnumerateObject())
                {
                    if (!names.Add(property.Name)) throw Invalid("Backup JSON contains a duplicate property.");
                    Inspect(property.Value);
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray()) Inspect(item);
            }
        }
    }

    private static byte[] SerializeBounded<T>(T value)
    {
        var content = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        if (content.Length > MaximumPayloadBytes)
            throw new DataPortabilityException("backup.payload_too_large", "Backup payload exceeds 10 MiB.");
        return content;
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            MaxDepth = 64,
            Encoder = JavaScriptEncoder.Default
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower, false));
        options.Converters.Add(new DecimalStringConverter());
        return options;
    }

    private static void EnsureCollections(FinancialSnapshot snapshot)
    {
        if (snapshot.Accounts is null || snapshot.Categories is null || snapshot.Transactions is null ||
            snapshot.Budgets is null || snapshot.Transfers is null || snapshot.Cards is null ||
            snapshot.Charges is null || snapshot.Payments is null || snapshot.InstallmentPlans is null ||
            snapshot.RecurringTransactions is null || snapshot.ImportBatches is null ||
            snapshot.Counterparties is null || snapshot.CounterpartyCharges is null ||
            snapshot.CounterpartyPayments is null || snapshot.Obligations is null ||
            snapshot.CashCounts is null || snapshot.PosSettlements is null ||
            snapshot.Debts is null || snapshot.SavingsGoals is null || snapshot.Attachments is null)
            throw Invalid("Every backup collection is required.");
        if (snapshot.InstallmentPlans.Any(x => x.Items is null) ||
            snapshot.RecurringTransactions.Any(x => x.Occurrences is null) ||
            snapshot.ImportBatches.Any(x => x.Rows is null) ||
            snapshot.Debts.Any(x => x.Installments is null) ||
            snapshot.SavingsGoals.Any(x => x.Contributions is null))
            throw Invalid("Nested backup collections are required.");
    }

    private static void EnsureEntityLimit(FinancialSnapshot snapshot)
    {
        if (snapshot.EntityCount > MaximumEntities)
            throw new DataPortabilityException(
                "backup.entity_limit_exceeded",
                $"Backup cannot contain more than {MaximumEntities} entities.");
    }

    private static void EnsureUniqueIds(IEnumerable<Guid> values, string kind)
    {
        var ids = values.ToArray();
        if (ids.Any(id => id == Guid.Empty) || ids.Distinct().Count() != ids.Length)
            throw Invalid($"Backup {kind} identifiers must be unique and non-empty.");
    }

    private static TValue Required<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue> values,
        TKey key,
        string relationship) where TKey : notnull =>
        values.TryGetValue(key, out var value)
            ? value
            : throw Invalid($"Backup contains a dangling {relationship} reference.");

    /// <summary>
    /// Dosyadaki iki isteğe bağlı KDV alanını tek bir değere çevirir.
    /// </summary>
    /// <remarks>
    /// İkisi de boşsa kayıt KDV taşımaz; "yok"un tek temsili budur. Oranla
    /// tutarın uyuşup uyuşmadığına <b>bakılmaz</b> — dosyadaki değer
    /// kullanıcının belgesidir ve geri yüklerken yeniden yorumlanmaz
    /// (ADR 0016).
    /// </remarks>
    private static VatDetails? VatOf(decimal? rate, decimal? amount) =>
        VatDetails.FromOptional(rate, amount);

    private static Money MoneyOf(decimal amount, CurrencyCode currency) => new(amount, currency);

    private static void ApplyCancellation(
        bool isCancelled,
        DateTimeOffset? cancelledAtUtc,
        Action<DateTimeOffset> cancel)
    {
        if (isCancelled != cancelledAtUtc.HasValue)
            throw Invalid("Cancellation flag and timestamp must appear together.");
        if (cancelledAtUtc is DateTimeOffset value) cancel(value);
    }

    private static DataPortabilityException Invalid(string message) =>
        new("restore.invalid_backup", message);

    private static string ProtectSpreadsheetFormula(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value[0] is '=' or '+' or '-' or '@' or '\t' or '\r' ? $"'{value}" : value;
    }

    private static string CsvCell(string value) =>
        value.IndexOfAny([',', '"', '\r', '\n']) >= 0
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;

    private sealed class DecimalStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String ||
                !decimal.TryParse(reader.GetString(),
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var value) ||
                decimal.Round(value, 4) != value ||
                decimal.Abs(value) > ImportRow.MaximumAbsoluteAmount)
                throw new JsonException(
                    "Ondalık değerler decimal(19,4) sınırına uymalı ve metin olarak kodlanmalıdır.");
            return value;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString("0.0000", CultureInfo.InvariantCulture));
    }
}

internal sealed record BackupEnvelope(
    string Format,
    int SchemaVersion,
    DateTimeOffset CreatedAtUtc,
    string PayloadEncoding,
    int PayloadLength,
    string PayloadSha256,
    string Payload);

internal sealed record FinancialSnapshot(
    AccountBackup[] Accounts,
    CategoryBackup[] Categories,
    TransactionBackup[] Transactions,
    BudgetBackup[] Budgets,
    TransferBackup[] Transfers,
    CardBackup[] Cards,
    ChargeBackup[] Charges,
    PaymentBackup[] Payments,
    InstallmentPlanBackup[] InstallmentPlans,
    RecurringBackup[] RecurringTransactions,
    ImportBatchBackup[] ImportBatches,
    CounterpartyBackup[] Counterparties,
    CounterpartyChargeBackup[] CounterpartyCharges,
    CounterpartyPaymentBackup[] CounterpartyPayments,
    ObligationBackup[] Obligations,
    CashCountBackup[] CashCounts,
    PosSettlementBackup[] PosSettlements,
    DebtBackup[] Debts,
    SavingsGoalBackup[] SavingsGoals,
    AttachmentBackup[] Attachments)
{
    [JsonIgnore]
    public int EntityCount => Accounts.Length + Categories.Length + Transactions.Length + Budgets.Length +
        Transfers.Length + Cards.Length + Charges.Length + Payments.Length +
        InstallmentPlans.Length + InstallmentPlans.Sum(x => x.Items.Length) +
        RecurringTransactions.Length + RecurringTransactions.Sum(x => x.Occurrences.Length) +
        ImportBatches.Length + ImportBatches.Sum(x => x.Rows.Length) +
        Counterparties.Length + CounterpartyCharges.Length + CounterpartyPayments.Length +
        Obligations.Length + Obligations.Count(x => x.Settlement is not null) +
        CashCounts.Length + PosSettlements.Length +
        Debts.Length + Debts.Sum(x => x.Installments.Length) +
        SavingsGoals.Length + SavingsGoals.Sum(x => x.Contributions.Length) +
        Attachments.Length;
}

internal sealed record AccountBackup(Guid Id, string Name, AccountType Type, CurrencyCode Currency, decimal OpeningBalance,
    bool IsActive, TransactionScope? DefaultScope);
internal sealed record CategoryBackup(Guid Id, string Name, CategoryType Type, bool IsActive,
    TransactionScope? DefaultScope, bool? DefaultIsTaxDeductible = null);
/// <remarks>
/// KDV ve indirilebilirlik dosyada <b>taşınan</b> alanlardır (ADR 0016): ikisi
/// de boş olabilir ve boş olmaları eksik veri değildir. Oranla tutar birbirini
/// tutmasa bile olduğu gibi yazılır — geri yüklerken düzeltmek, kullanıcının
/// belgesini yeniden yorumlamak olurdu.
/// </remarks>
internal sealed record TransactionBackup(Guid Id, Guid AccountId, Guid CategoryId, decimal Amount, CurrencyCode Currency,
    TransactionType Type, TransactionScope Scope, DateOnly TransactionDate, string? Description, bool IsCancelled,
    DateTimeOffset? CancelledAtUtc, decimal? VatRate = null, decimal? VatAmount = null,
    bool? IsTaxDeductible = null);
internal sealed record BudgetBackup(Guid Id, Guid CategoryId, decimal Limit, CurrencyCode Currency,
    TransactionScope Scope, int Year, int Month);
internal sealed record TransferBackup(Guid Id, Guid SourceAccountId, Guid DestinationAccountId, decimal Amount,
    CurrencyCode Currency, DateOnly TransferDate, string? Description, bool IsCancelled, DateTimeOffset? CancelledAtUtc);
internal sealed record CardBackup(Guid Id, string Name, decimal Limit, CurrencyCode Currency,
    int StatementClosingDay, int PaymentDueDay, bool IsActive, decimal MinimumPaymentRate,
    TransactionScope? DefaultScope);
internal sealed record ChargeBackup(Guid Id, Guid CreditCardId, Guid CategoryId, decimal Amount, CurrencyCode Currency,
    TransactionScope Scope, DateOnly ChargeDate, string? Description, bool IsCancelled, DateTimeOffset? CancelledAtUtc,
    decimal? VatRate = null, decimal? VatAmount = null, bool? IsTaxDeductible = null);
internal sealed record PaymentBackup(Guid Id, Guid AccountId, Guid CreditCardId, decimal Amount, CurrencyCode Currency,
    DateOnly PaymentDate, string? Description, bool IsCancelled, DateTimeOffset? CancelledAtUtc);
internal sealed record InstallmentPlanBackup(Guid Id, Guid CreditCardId, Guid CategoryId, Guid ClientRequestId,
    decimal TotalAmount, CurrencyCode Currency, TransactionScope Scope, int InstallmentCount,
    DateOnly FirstInstallmentDate, string? Description, InstallmentItemBackup[] Items);
internal sealed record InstallmentItemBackup(Guid Id, int Sequence, decimal Amount, CurrencyCode Currency,
    DateOnly ScheduledDate, Guid? CreditCardChargeId, DateTimeOffset? RealizedAtUtc);
internal sealed record RecurringBackup(Guid Id, Guid? AccountId, Guid CategoryId, decimal Amount,
    CurrencyCode Currency, RecurringTransactionKind Kind, TransactionScope Scope, RecurrenceFrequency Frequency,
    DateOnly StartDate, DateOnly? EndDate, DateOnly? NextOccurrenceDate, MonthEndBehavior MonthEndBehavior,
    string? Description, bool IsActive, OccurrenceBackup[] Occurrences,
    RecurringSourceType SourceType, int? OccurrenceLimit, int GeneratedOccurrenceCount,
    Guid? CreditCardId = null);
internal sealed record OccurrenceBackup(Guid Id, DateOnly ScheduledDate,
    Guid? BudgetTransactionId, DateTimeOffset? RealizedAtUtc,
    Guid? CreditCardChargeId = null);
internal sealed record ImportBatchBackup(Guid Id, string FileName, string FileFingerprint, long FileSizeBytes,
    string EncodingName, string Delimiter, string DateColumn, string AmountColumn,
    string? DescriptionColumn, string? ReferenceColumn, string DateFormat, string DecimalSeparator,
    ImportBatchStatus Status, DateTimeOffset CreatedAtUtc, ImportRowBackup[] Rows);
internal sealed record ImportRowBackup(Guid Id, int RowNumber, string RawData, DateOnly? TransactionDate,
    decimal? SignedAmount, CurrencyCode Currency, string? Description, string? ExternalReference,
    Guid? AccountId, Guid? CategoryId, ImportRowStatus Status, string? ErrorMessage,
    Guid? BudgetTransactionId, Guid? DuplicateTransactionId, ImportDuplicateReason? DuplicateReason);
internal sealed record CounterpartyBackup(Guid Id, string Name, string? Note, bool IsActive);
/// <remarks>
/// Borçlandırma <b>tanır</b>: kapsam ve kategori taşır, hesap taşımaz.
/// Tahsilat <b>taşır</b>: hesap taşır, kategori ve kapsam taşımaz (ADR 0014).
/// İkisinin alan listesi bu yüzden bilerek farklı; yedekte eşitlemek, olmayan
/// bir soruyu dosyaya yazmak olurdu.
/// </remarks>
internal sealed record CounterpartyChargeBackup(
    Guid Id, Guid CounterpartyId, Guid CategoryId, DebtDirection Direction, decimal Amount,
    CurrencyCode Currency, TransactionScope Scope, DateOnly ChargeDate, string? Description,
    bool IsCancelled, DateTimeOffset? CancelledAtUtc, DateOnly? DueDate = null,
    decimal? VatRate = null, decimal? VatAmount = null, bool? IsTaxDeductible = null);
internal sealed record CounterpartyPaymentBackup(
    Guid Id, Guid CounterpartyId, Guid AccountId, DebtDirection Direction, decimal Amount,
    CurrencyCode Currency, DateOnly PaymentDate, string? Description,
    bool IsCancelled, DateTimeOffset? CancelledAtUtc);
/// <remarks>
/// Yükümlülük <b>tanır</b>: kategori ve kapsam taşır, hesap taşımaz. Onu
/// kapatan <see cref="Settlement"/> <b>taşır</b>: hesap taşır, kategori ve
/// kapsam taşımaz (ADR 0014). Kapanış ayrı bir koleksiyon değil, yükümlülüğün
/// içinde durur — bire bir bağ dosyada da böyle görünür ve sahipsiz bir ödeme
/// yazılamaz. Gecikme dosyada <b>yok</b>: kalıcı bir alan değil, vade ile
/// okunduğu günün karşılaştırmasıdır.
/// </remarks>
internal sealed record ObligationBackup(
    Guid Id, Guid? CounterpartyId, Guid CategoryId, DebtDirection Direction, decimal Amount,
    CurrencyCode Currency, TransactionScope Scope, DateOnly IssueDate, DateOnly DueDate,
    string? Description, DateTimeOffset CreatedAtUtc, bool IsCancelled,
    DateTimeOffset? CancelledAtUtc, ObligationSettlementBackup? Settlement,
    decimal? VatRate = null, decimal? VatAmount = null, bool? IsTaxDeductible = null);
internal sealed record ObligationSettlementBackup(
    Guid Id, Guid AccountId, decimal Amount, CurrencyCode Currency,
    DateOnly SettlementDate, DateTimeOffset SettledAtUtc);
/// <remarks>
/// Sayım bir <b>gözlemdir</b>: hesap bakiyesine dokunmaz, gelir/gider yazmaz.
/// Beklenen bakiye ve fark dosyada <b>yok</b> - ikisi de kalıcı alan değil,
/// okunduğu anda hesap bakiyesinden türetilir. Yazılsalardı geri yüklenen
/// veritabanında sayımın yanındaki sayı hesabın gerçek bakiyesiyle
/// çelişebilirdi. Farkı yazan kayıt varsa <see cref="AdjustmentTransactionId"/>
/// ile bağlanır; onaylanmamış bir sayımda bu alan boştur ve boş olması
/// normaldir.
/// </remarks>
internal sealed record CashCountBackup(
    Guid Id, Guid AccountId, decimal CountedAmount, CurrencyCode Currency,
    TransactionScope Scope, DateOnly CountDate, string? Note, DateTimeOffset CreatedAtUtc,
    Guid? AdjustmentTransactionId, DateTimeOffset? AdjustedAtUtc,
    bool IsCancelled, DateTimeOffset? CancelledAtUtc);
/// <remarks>
/// POS tahsilatı tek kaydın <b>iki anını</b> taşır (ADR 0014): tahsilat günü
/// gelir brüt tutar kadar tanınır ve komisyon ayrı gider yazılır, hesap
/// kıpırdamaz; <see cref="TransferredOn"/> dolduğu gün hesap net tutar kadar
/// artar ve hiçbir gelir/gider yeniden yazılmaz.
///
/// Net tutar ve komisyon <b>oranı</b> dosyada yok: ikisi de brüt ile komisyon
/// tutarından çözülür. Oran yazılsaydı kuruşa yuvarlanmış komisyonla çelişen
/// ikinci bir gerçek kaynağı doğardı (ADR 0009 ile aynı karar). "Yolda mı" da
/// yazılmaz; iptal ve geçiş bilgisinden türer.
/// </remarks>
internal sealed record PosSettlementBackup(
    Guid Id, Guid AccountId, Guid CategoryId, Guid? CommissionCategoryId,
    decimal GrossAmount, decimal CommissionAmount, CurrencyCode Currency,
    TransactionScope Scope, DateOnly SettlementDate, DateOnly ExpectedTransferDate,
    string? Description, DateTimeOffset CreatedAtUtc,
    DateOnly? TransferredOn, DateTimeOffset? TransferredAtUtc,
    bool IsCancelled, DateTimeOffset? CancelledAtUtc,
    decimal? VatRate = null, decimal? VatAmount = null);
/// <remarks>
/// <see cref="AnnualInterestRate"/> hâlâ yazılıyor ama geri yüklerken
/// okunmuyor: oran artık paradan çözülüyor, yedekteki değer ise hiçbir hesaba
/// girmemiş serbest bir sayıydı.
/// </remarks>
internal sealed record DebtBackup(
    Guid Id, Guid CounterpartyId, DebtDirection Direction, TransactionScope Scope, decimal Principal,
    decimal TotalRepayment, CurrencyCode Currency, decimal AnnualInterestRate,
    DateOnly StartDate, DateOnly FirstDueDate, int InstallmentCount, string? Description,
    DebtInstallmentBackup[] Installments,
    DebtSourceType SourceType,
    Guid? OpeningAccountId = null,
    Guid? CategoryId = null);
internal sealed record DebtInstallmentBackup(
    int Sequence, decimal Amount, CurrencyCode Currency, DateOnly DueDate,
    Guid? PaymentAccountId, DateOnly? PaymentDate, DateTimeOffset? PaidAtUtc);
internal sealed record SavingsGoalBackup(
    Guid Id, string Name, decimal TargetAmount, CurrencyCode Currency, DateOnly TargetDate,
    SavingsGoalTrackingMode TrackingMode, Guid? AccountId, string? Description,
    DateTimeOffset CreatedAtUtc, SavingsGoalContributionBackup[] Contributions,
    TransactionScope? Scope = null);
internal sealed record SavingsGoalContributionBackup(
    Guid Id, decimal Amount, CurrencyCode Currency, DateOnly ContributionDate,
    Guid ClientRequestId, string? Note, DateTimeOffset CreatedAtUtc);
internal sealed record AttachmentBackup(
    Guid Id, Guid TransactionId, string OriginalFileName, string ContentType,
    long SizeBytes, string Sha256, DateTimeOffset CreatedAtUtc, string ContentBase64);

internal sealed record CounterpartyLedgerRow(
    Guid Id,
    DateOnly Date,
    string Kind,
    DebtDirection Direction,
    Money Amount,
    Guid CounterpartyId,
    Guid? CategoryId,
    Guid? AccountId,
    TransactionScope? Scope,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

internal sealed record ParsedBackup(BackupEnvelope Envelope, FinancialSnapshot Snapshot);
internal sealed record RestoredGraph(
    Account[] Accounts,
    Category[] Categories,
    BudgetTransaction[] Transactions,
    MonthlyBudget[] Budgets,
    Transfer[] Transfers,
    CreditCard[] Cards,
    CreditCardCharge[] Charges,
    CreditCardPayment[] Payments,
    InstallmentPlan[] InstallmentPlans,
    RecurringTransaction[] RecurringTransactions,
    RecurringTransactionOccurrence[] Occurrences,
    ImportBatch[] ImportBatches,
    Counterparty[] Counterparties,
    CounterpartyCharge[] CounterpartyCharges,
    CounterpartyPayment[] CounterpartyPayments,
    Obligation[] Obligations,
    CashCount[] CashCounts,
    PosSettlement[] PosSettlements,
    DebtAgreement[] Debts,
    SavingsGoal[] Goals,
    RestoredAttachment[] Attachments);
internal sealed record RestoredAttachment(FinancialAttachment Metadata, byte[] Content);
