using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Imports;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace BusinessFinance.Infrastructure.Imports;

public sealed class EfImportBatchRepository(BusinessFinanceDbContext dbContext)
    : IImportBatchRepository
{
    public async Task AddAsync(ImportBatch batch, CancellationToken cancellationToken)
    {
        dbContext.ImportBatches.Add(batch);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ImportBatch> AddOrGetExistingAsync(
        ImportBatch batch,
        CancellationToken cancellationToken)
    {
        var existing = await FindByFingerprintAsync(batch.UserId, batch.FileFingerprint, cancellationToken);
        if (existing is not null) return existing;
        try
        {
            dbContext.ImportBatches.Add(batch);
            await dbContext.SaveChangesAsync(cancellationToken);
            return batch;
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            dbContext.ChangeTracker.Clear();
            return await FindByFingerprintAsync(batch.UserId, batch.FileFingerprint, cancellationToken)
                ?? throw new InvalidOperationException("The winning import batch was not found.");
        }
    }

    public Task<ImportBatch?> FindOwnedByIdAsync(
        Guid batchId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.ImportBatches : dbContext.ImportBatches.AsNoTracking())
            .Include(batch => batch.Rows)
            .SingleOrDefaultAsync(
                batch => batch.Id == batchId && batch.UserId == userId,
                cancellationToken);

    public async Task UpdateAsync(ImportBatch batch, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            throw new ImportConcurrencyException();
        }
    }

    public async Task ConfirmAsync(
        ImportBatch batch,
        IReadOnlyCollection<BudgetTransaction> transactions,
        CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsRelational())
        {
            try
            {
                dbContext.Transactions.AddRange(transactions);
                await dbContext.SaveChangesAsync(cancellationToken);
                return;
            }
            catch (DbUpdateConcurrencyException)
            {
                dbContext.ChangeTracker.Clear();
                throw new ImportConcurrencyException();
            }
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            dbContext.Transactions.AddRange(transactions);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();
            throw new ImportConcurrencyException();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<DuplicateMatch?> FindDuplicateAsync(
        Guid userId,
        string? externalReference,
        DateOnly transactionDate,
        TransactionType type,
        decimal amount,
        string? description,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(externalReference))
        {
            var referenceMatches = await dbContext.ImportRows.AsNoTracking()
                .Where(row => row.UserId == userId &&
                              row.Status == ImportRowStatus.Imported &&
                              row.ExternalReference == externalReference &&
                              row.BudgetTransactionId != null)
                .Select(row => new { row.ExternalReference, row.BudgetTransactionId })
                .ToArrayAsync(cancellationToken);
            var exact = referenceMatches.FirstOrDefault(match =>
                string.Equals(match.ExternalReference, externalReference, StringComparison.Ordinal));
            if (exact is not null)
                return new DuplicateMatch(exact.BudgetTransactionId!.Value, ImportDuplicateReason.BankReference);
        }

        var candidates = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId &&
                                  !transaction.IsCancelled &&
                                  transaction.TransactionDate == transactionDate &&
                                  transaction.Type == type &&
                                  transaction.Amount.Amount == amount)
            .Select(transaction => new { transaction.Id, transaction.Description })
            .ToArrayAsync(cancellationToken);
        var normalized = NormalizeDescription(description);
        var match = candidates.FirstOrDefault(candidate =>
            NormalizeDescription(candidate.Description) == normalized);
        return match is null
            ? null
            : new DuplicateMatch(match.Id, ImportDuplicateReason.DateAmountDescription);
    }

    private Task<ImportBatch?> FindByFingerprintAsync(
        Guid userId,
        string fingerprint,
        CancellationToken cancellationToken) =>
        dbContext.ImportBatches.AsNoTracking()
            .Include(batch => batch.Rows)
            .SingleOrDefaultAsync(
                batch => batch.UserId == userId && batch.FileFingerprint == fingerprint,
                cancellationToken);

    private static string NormalizeDescription(string? value) =>
        string.Join(' ', (value ?? string.Empty)
            .Trim()
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}
