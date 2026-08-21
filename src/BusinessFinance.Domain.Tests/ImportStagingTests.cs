namespace BusinessFinance.Domain.Tests;

public sealed class ImportStagingTests
{
    [Fact]
    public void Batch_CollectsValidAndInvalidRowsWithoutCreatingFinancialTransactions()
    {
        var userId = Guid.NewGuid();
        var batch = CreateBatch(userId);
        batch.AddRow(new ImportRow(
            Guid.NewGuid(), userId, batch.Id, 2, "2026-08-11;-10,5000;Market",
            new DateOnly(2026, 8, 11), -10.5m, CurrencyCode.TRY, "Market", null, []));
        batch.AddRow(new ImportRow(
            Guid.NewGuid(), userId, batch.Id, 3, "bad;zero;Broken",
            null, null, CurrencyCode.TRY, "Broken", null,
            ["Date is invalid.", "Amount is invalid."]));

        Assert.Equal(2, batch.TotalRowCount);
        Assert.Equal(1, batch.ValidRowCount);
        Assert.Equal(1, batch.InvalidRowCount);
        Assert.Equal(ImportBatchStatus.Staged, batch.Status);
        Assert.Equal(100, batch.FileSizeBytes);
        Assert.Contains("Date is invalid.", batch.Rows.Last().ErrorMessage);
    }

    [Fact]
    public void Batch_RejectsRowOwnedByAnotherUser()
    {
        var batch = CreateBatch(Guid.NewGuid());
        var foreignRow = new ImportRow(
            Guid.NewGuid(), Guid.NewGuid(), batch.Id, 2, "row",
            new DateOnly(2026, 8, 11), 1m, CurrencyCode.TRY, null, null, []);

        Assert.Throws<ArgumentException>(() => batch.AddRow(foreignRow));
    }

    [Fact]
    public void Row_AmountBeyondSqlDecimalMagnitude_RemainsInvalidWithoutPersistingOverflow()
    {
        var row = new ImportRow(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2, "overflow",
            new DateOnly(2026, 8, 11), 1000000000000000.0000m,
            CurrencyCode.TRY, null, null, []);

        Assert.Equal(ImportRowStatus.Invalid, row.Status);
        Assert.Null(row.SignedAmount);
        Assert.Contains("decimal(19,4)", row.ErrorMessage);
    }

    [Fact]
    public void DuplicateReview_RequiresExplicitDecisionAndSkippedRowCompletesBatch()
    {
        var userId = Guid.NewGuid();
        var batch = CreateBatch(userId);
        var account = new Account(Guid.NewGuid(), userId, "Duplicate", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(Guid.NewGuid(), userId, "Expense duplicate", CategoryType.Expense);
        var row = new ImportRow(
            Guid.NewGuid(), userId, batch.Id, 2, "duplicate",
            new DateOnly(2026, 8, 11), -10m, CurrencyCode.TRY, "Same", "bank-1", []);
        batch.AddRow(row);
        row.ApplyCorrection(new DateOnly(2026, 8, 11), -10m, "Same", "bank-1", account, category);
        var existingId = Guid.NewGuid();
        row.FlagDuplicate(existingId, ImportDuplicateReason.BankReference);

        Assert.Throws<InvalidOperationException>(() =>
            row.CreateTransaction(account, category, Guid.NewGuid()));
        row.ResolveDuplicate(importAnyway: false);
        batch.RecordConfirmation();

        Assert.Equal(ImportRowStatus.SkippedDuplicate, row.Status);
        Assert.Equal(existingId, row.DuplicateTransactionId);
        Assert.Equal(ImportBatchStatus.Imported, batch.Status);
    }

    [Theory]
    [InlineData("100.2500", TransactionType.Income, CategoryType.Income)]
    [InlineData("-100.2500", TransactionType.Expense, CategoryType.Expense)]
    public void CorrectAndImport_MapsSignedAmountWithoutLosingPrecision(
        string signedAmountText,
        TransactionType expectedType,
        CategoryType categoryType)
    {
        var signedAmount = decimal.Parse(signedAmountText, System.Globalization.CultureInfo.InvariantCulture);
        var userId = Guid.NewGuid();
        var batch = CreateBatch(userId);
        var row = new ImportRow(
            Guid.NewGuid(), userId, batch.Id, 2, "row", null, null,
            CurrencyCode.TRY, null, null, ["Initially invalid."]);
        batch.AddRow(row);
        var account = new Account(Guid.NewGuid(), userId, "Import", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(Guid.NewGuid(), userId, "Mapped", categoryType);

        row.ApplyCorrection(new DateOnly(2026, 8, 11), signedAmount, "Corrected", "ref", account, category);
        var transaction = row.CreateTransaction(account, category, Guid.NewGuid());
        row.MarkImported(transaction.Id);
        batch.RecordConfirmation();

        Assert.Equal(expectedType, transaction.Type);
        Assert.Equal(100.2500m, transaction.Amount.Amount);
        Assert.Equal(ImportRowStatus.Imported, row.Status);
        Assert.Equal(transaction.Id, row.BudgetTransactionId);
        Assert.Equal(ImportBatchStatus.Imported, batch.Status);
        Assert.Throws<InvalidOperationException>(() => row.ApplyCorrection(
            new DateOnly(2026, 8, 12), signedAmount, null, null, account, category));
    }

    private static ImportBatch CreateBatch(Guid userId) => new(
        Guid.NewGuid(), userId, "statement.csv", new string('a', 64), 100, "utf-8", ';',
        "Date", "Amount", "Description", null, "yyyy-MM-dd", ',',
        new DateTimeOffset(2026, 8, 11, 8, 0, 0, TimeSpan.Zero));
}
