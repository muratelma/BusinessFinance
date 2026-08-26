using System.Globalization;
using System.IO.Compression;
using System.Text;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Application.DataPortability;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Taxes;

/// <summary>
/// Muhasebeci paketinin okuması ve dosyaya yazılması.
/// </summary>
/// <remarks>
/// Satır listesi, aylık raporun <b>aynı kaynaklarını</b> aynı filtrelerle okur;
/// tek farkı toplamak yerine satırları döndürmesidir. Toplam buradan
/// hesaplanmaz — paket toplamlarını rapordan alır ve bir test ikisinin eşit
/// olduğunu tutar.
/// </remarks>
internal sealed class EfAccountantPackageRepository(
    BusinessFinanceDbContext dbContext,
    IAttachmentObjectStore objectStore)
    : IAccountantPackageRepository
{
    /// <summary>
    /// Pakete konacak eklerin toplam boyut tavanı.
    /// </summary>
    /// <remarks>
    /// Tavanı aşan ek listede kalır, dosyası konmaz: eksik paketi tam sanmak,
    /// eksik olduğunu bilerek göndermekten kötüdür.
    /// </remarks>
    internal const long MaximumAttachmentBytes = 20 * 1024 * 1024;

    public async Task<IReadOnlyList<AccountantPackageLineDto>> ListBusinessLinesAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var start = new DateOnly(year, month, 1);
        var endExclusive = start.AddMonths(1);
        const TransactionScope Business = TransactionScope.Business;

        var categoryNames = await dbContext.Categories.AsNoTracking()
            .Where(category => category.UserId == userId)
            .ToDictionaryAsync(category => category.Id, category => category.Name, cancellationToken);
        var counterpartyNames = await dbContext.Counterparties.AsNoTracking()
            .Where(counterparty => counterparty.UserId == userId)
            .ToDictionaryAsync(
                counterparty => counterparty.Id, counterparty => counterparty.Name, cancellationToken);

        var transactions = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId &&
                                  !transaction.IsCancelled &&
                                  transaction.Scope == Business &&
                                  transaction.TransactionDate >= start &&
                                  transaction.TransactionDate < endExclusive)
            .ToArrayAsync(cancellationToken);
        var attachmentCounts = await dbContext.FinancialAttachments.AsNoTracking()
            .Where(attachment => attachment.UserId == userId)
            .GroupBy(attachment => attachment.TransactionId)
            .Select(group => new { TransactionId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(row => row.TransactionId, row => row.Count, cancellationToken);

        var cardCharges = await dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             charge.Scope == Business &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive)
            .ToArrayAsync(cancellationToken);
        var counterpartyCharges = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             charge.Scope == Business &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive)
            .ToArrayAsync(cancellationToken);
        var obligations = await dbContext.Obligations.AsNoTracking()
            .Where(obligation => obligation.UserId == userId &&
                                 !obligation.IsCancelled &&
                                 obligation.Scope == Business &&
                                 obligation.IssueDate >= start &&
                                 obligation.IssueDate < endExclusive)
            .ToArrayAsync(cancellationToken);
        var posSettlements = await dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.Scope == Business &&
                                 settlement.SettlementDate >= start &&
                                 settlement.SettlementDate < endExclusive)
            .ToArrayAsync(cancellationToken);
        var debtOpenings = await dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.SourceType != DebtSourceType.Unrecorded &&
                           debt.Scope == Business &&
                           debt.StartDate >= start &&
                           debt.StartDate < endExclusive)
            .ToArrayAsync(cancellationToken);
        var debtInterest = await (
                from installment in dbContext.DebtInstallments.AsNoTracking()
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                where installment.UserId == userId &&
                      installment.InterestPortion != null &&
                      debt.Scope == Business &&
                      installment.PaymentDate >= start &&
                      installment.PaymentDate < endExclusive
                select new
                {
                    installment.Id,
                    PaymentDate = installment.PaymentDate!.Value,
                    Interest = installment.InterestPortion!.Value,
                    debt.Direction,
                    debt.CounterpartyId,
                    debt.Description
                })
            .ToArrayAsync(cancellationToken);

        var lines = new List<AccountantPackageLineDto>(
            transactions.Length + cardCharges.Length + counterpartyCharges.Length +
            obligations.Length + posSettlements.Length * 2 + debtOpenings.Length +
            debtInterest.Length);

        lines.AddRange(transactions.Select(transaction => new AccountantPackageLineDto(
            "transaction",
            transaction.Id,
            transaction.TransactionDate,
            transaction.Type,
            transaction.Amount.Amount,
            Name(categoryNames, transaction.CategoryId),
            null,
            transaction.Description,
            transaction.Vat?.Rate,
            transaction.Vat?.Amount,
            transaction.IsTaxDeductible,
            attachmentCounts.TryGetValue(transaction.Id, out var count) ? count : 0)));

        lines.AddRange(cardCharges.Select(charge => new AccountantPackageLineDto(
            "card-charge",
            charge.Id,
            charge.ChargeDate,
            TransactionType.Expense,
            charge.Amount.Amount,
            Name(categoryNames, charge.CategoryId),
            null,
            charge.Description,
            charge.Vat?.Rate,
            charge.Vat?.Amount,
            charge.IsTaxDeductible,
            0)));

        lines.AddRange(counterpartyCharges.Select(charge => new AccountantPackageLineDto(
            "counterparty-charge",
            charge.Id,
            charge.ChargeDate,
            charge.Direction == DebtDirection.Receivable
                ? TransactionType.Income
                : TransactionType.Expense,
            charge.Amount.Amount,
            Name(categoryNames, charge.CategoryId),
            Name(counterpartyNames, charge.CounterpartyId),
            charge.Description,
            charge.Vat?.Rate,
            charge.Vat?.Amount,
            charge.IsTaxDeductible,
            0)));

        lines.AddRange(obligations.Select(obligation => new AccountantPackageLineDto(
            "obligation",
            obligation.Id,
            obligation.IssueDate,
            obligation.Direction == DebtDirection.Receivable
                ? TransactionType.Income
                : TransactionType.Expense,
            obligation.Amount.Amount,
            Name(categoryNames, obligation.CategoryId),
            obligation.CounterpartyId is Guid counterpartyId
                ? Name(counterpartyNames, counterpartyId)
                : null,
            obligation.Description,
            obligation.Vat?.Rate,
            obligation.Vat?.Amount,
            obligation.IsTaxDeductible,
            0)));

        // POS tahsilatı iki satırdır: satış brüt tutarla gelir, komisyon ayrı
        // gider (ADR 0015). Netten tek satır yazmak, kesilen faturayı küçültür
        // ve bankanın kesintisini görünmez yapardı.
        lines.AddRange(posSettlements.Select(settlement => new AccountantPackageLineDto(
            "pos-sale",
            settlement.Id,
            settlement.SettlementDate,
            TransactionType.Income,
            settlement.GrossAmount.Amount,
            Name(categoryNames, settlement.CategoryId),
            null,
            settlement.Description,
            settlement.Vat?.Rate,
            settlement.Vat?.Amount,
            null,
            0)));
        lines.AddRange(posSettlements
            .Where(settlement => settlement.CommissionAmount > 0m)
            .Select(settlement => new AccountantPackageLineDto(
                "pos-commission",
                settlement.Id,
                settlement.SettlementDate,
                TransactionType.Expense,
                settlement.CommissionAmount,
                settlement.CommissionCategoryId is Guid commissionCategoryId
                    ? Name(categoryNames, commissionCategoryId)
                    : null,
                null,
                settlement.Description,
                null,
                null,
                null,
                0)));

        // Gider kaynaklı borcun açılışı o gün giderdir, gelir kaynaklının
        // açılışı gelirdir; taksit ödemeleri gider üretmez (ADR 0009).
        lines.AddRange(debtOpenings.Select(debt => new AccountantPackageLineDto(
            "debt-opening",
            debt.Id,
            debt.StartDate,
            debt.SourceType == DebtSourceType.Income
                ? TransactionType.Income
                : TransactionType.Expense,
            debt.Principal.Amount,
            debt.CategoryId is Guid debtCategoryId ? Name(categoryNames, debtCategoryId) : null,
            Name(counterpartyNames, debt.CounterpartyId),
            debt.Description,
            null,
            null,
            null,
            0)));

        // Faiz borcun gerçek maliyetidir; anapara geri ödemesi gider değildir.
        lines.AddRange(debtInterest.Select(row => new AccountantPackageLineDto(
            "debt-interest",
            row.Id,
            row.PaymentDate,
            row.Direction == DebtDirection.Receivable
                ? TransactionType.Income
                : TransactionType.Expense,
            row.Interest,
            null,
            Name(counterpartyNames, row.CounterpartyId),
            row.Description,
            null,
            null,
            null,
            0)));

        return [.. lines.OrderBy(line => line.Date).ThenBy(line => line.Source, StringComparer.Ordinal)
            .ThenBy(line => line.SourceId)];
    }

    public async Task<IReadOnlyList<AccountantPackageAttachmentDto>> ListAttachmentsAsync(
        Guid userId,
        IReadOnlyCollection<Guid> transactionIds,
        CancellationToken cancellationToken)
    {
        if (transactionIds.Count == 0)
        {
            return [];
        }

        var attachments = await dbContext.FinancialAttachments.AsNoTracking()
            .Where(attachment => attachment.UserId == userId &&
                                 transactionIds.Contains(attachment.TransactionId))
            .OrderBy(attachment => attachment.CreatedAtUtc)
            .ThenBy(attachment => attachment.Id)
            .ToArrayAsync(cancellationToken);

        var budget = MaximumAttachmentBytes;
        var results = new List<AccountantPackageAttachmentDto>(attachments.Length);
        foreach (var attachment in attachments)
        {
            var fits = attachment.SizeBytes <= budget;
            if (fits)
            {
                budget -= attachment.SizeBytes;
            }

            results.Add(new AccountantPackageAttachmentDto(
                attachment.Id,
                attachment.TransactionId,
                attachment.OriginalFileName,
                attachment.ContentType,
                attachment.SizeBytes,
                fits));
        }

        return results;
    }

    public async Task<PortableFile> BuildPackageAsync(
        Guid userId,
        AccountantPackageDto package,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(package);

        var objectKeys = await dbContext.FinancialAttachments.AsNoTracking()
            .Where(attachment => attachment.UserId == userId)
            .ToDictionaryAsync(
                attachment => attachment.Id, attachment => attachment.ObjectKey, cancellationToken);

        using var buffer = new MemoryStream();
        using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
        {
            await WriteEntryAsync(archive, "summary.csv", BuildSummaryCsv(package), cancellationToken);
            await WriteEntryAsync(archive, "lines.csv", BuildLinesCsv(package), cancellationToken);
            await WriteEntryAsync(
                archive, "attachments.csv", BuildAttachmentsCsv(package), cancellationToken);

            foreach (var attachment in package.Attachments.Where(item => item.IsIncluded))
            {
                if (!objectKeys.TryGetValue(attachment.Id, out var objectKey))
                {
                    continue;
                }

                await using var content = await objectStore.OpenReadAsync(objectKey, cancellationToken);
                if (content is null)
                {
                    continue;
                }

                var entry = archive.CreateEntry(
                    $"attachments/{attachment.Id:D}-{attachment.FileName}",
                    CompressionLevel.Fastest);
                await using var entryStream = entry.Open();
                await content.CopyToAsync(entryStream, cancellationToken);
            }
        }

        return new PortableFile(
            $"muhasebeci-paketi-{package.Year:D4}-{package.Month:D2}.zip",
            "application/zip",
            buffer.ToArray());
    }

    private static async Task WriteEntryAsync(
        ZipArchive archive,
        string name,
        string content,
        CancellationToken cancellationToken)
    {
        var entry = archive.CreateEntry(name, CompressionLevel.Fastest);
        await using var stream = entry.Open();
        // BOM: dosya kullanıcının kendi bilgisayarında bir tabloya açılıyor ve
        // Türkçe adlar onsuz bozuk okunuyor.
        await stream.WriteAsync(new byte[] { 0xEF, 0xBB, 0xBF }, cancellationToken);
        await stream.WriteAsync(new UTF8Encoding(false).GetBytes(content), cancellationToken);
    }

    private static string BuildSummaryCsv(AccountantPackageDto package)
    {
        var builder = new StringBuilder();
        builder.AppendLine("key,value");
        builder.AppendLine($"year,{package.Year}");
        builder.AppendLine($"month,{package.Month}");
        builder.AppendLine($"scope,business");
        builder.AppendLine($"currency,{package.Currency}");
        builder.AppendLine($"totalIncome,{Money(package.TotalIncome)}");
        builder.AppendLine($"totalExpense,{Money(package.TotalExpense)}");
        builder.AppendLine($"net,{Money(package.Net)}");
        builder.AppendLine($"vatOnIncome,{Money(package.VatOnIncome)}");
        builder.AppendLine($"vatOnExpense,{Money(package.VatOnExpense)}");
        builder.AppendLine($"linesWithoutVat,{package.LinesWithoutVat}");
        builder.AppendLine($"nonDeductibleExpense,{Money(package.NonDeductibleExpense)}");
        builder.AppendLine($"nonDeductibleCount,{package.NonDeductibleCount}");
        builder.AppendLine($"deductibilityUnansweredCount,{package.DeductibilityUnansweredCount}");
        builder.AppendLine($"lineCount,{package.Lines.Count}");
        builder.AppendLine($"attachmentCount,{package.Attachments.Count}");
        return builder.ToString();
    }

    private static string BuildLinesCsv(AccountantPackageDto package)
    {
        var builder = new StringBuilder();
        builder.AppendLine(
            "source,sourceId,date,type,amount,currency,categoryName,counterpartyName," +
            "description,vatRate,vatAmount,isTaxDeductible,attachmentCount");
        foreach (var line in package.Lines)
        {
            var cells = new[]
            {
                line.Source,
                line.SourceId.ToString("D"),
                line.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                line.Type.ToString().ToLowerInvariant(),
                Money(line.Amount),
                package.Currency.ToString(),
                ProtectSpreadsheetFormula(line.CategoryName),
                ProtectSpreadsheetFormula(line.CounterpartyName),
                ProtectSpreadsheetFormula(line.Description),
                line.VatRate is decimal rate ? Money(rate) : string.Empty,
                line.VatAmount is decimal amount ? Money(amount) : string.Empty,
                line.IsTaxDeductible switch
                {
                    true => "true",
                    false => "false",
                    null => string.Empty
                },
                line.AttachmentCount.ToString(CultureInfo.InvariantCulture)
            };
            builder.AppendLine(string.Join(',', cells.Select(CsvCell)));
        }

        return builder.ToString();
    }

    private static string BuildAttachmentsCsv(AccountantPackageDto package)
    {
        var builder = new StringBuilder();
        builder.AppendLine("id,transactionId,fileName,contentType,sizeBytes,isIncluded");
        foreach (var attachment in package.Attachments)
        {
            var cells = new[]
            {
                attachment.Id.ToString("D"),
                attachment.TransactionId.ToString("D"),
                ProtectSpreadsheetFormula(attachment.FileName),
                attachment.ContentType,
                attachment.SizeBytes.ToString(CultureInfo.InvariantCulture),
                attachment.IsIncluded ? "true" : "false"
            };
            builder.AppendLine(string.Join(',', cells.Select(CsvCell)));
        }

        return builder.ToString();
    }

    private static string Name(IReadOnlyDictionary<Guid, string> names, Guid id) =>
        names.TryGetValue(id, out var name) ? name : string.Empty;

    private static string Money(decimal value) =>
        value.ToString("0.0000", CultureInfo.InvariantCulture);

    private static string CsvCell(string? value)
    {
        var text = value ?? string.Empty;
        return text.Contains(',', StringComparison.Ordinal) ||
               text.Contains('"', StringComparison.Ordinal) ||
               text.Contains('\n', StringComparison.Ordinal) ||
               text.Contains('\r', StringComparison.Ordinal)
            ? $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : text;
    }

    /// <summary>
    /// Tabloya açıldığında formül olarak yorumlanacak hücreyi zararsızlaştırır.
    /// </summary>
    private static string ProtectSpreadsheetFormula(string? value) =>
        string.IsNullOrEmpty(value) || !"=+-@\t\r".Contains(value[0], StringComparison.Ordinal)
            ? value ?? string.Empty
            : $"'{value}";
}
