using BusinessFinance.Domain;

namespace BusinessFinance.Application.Imports;

public sealed record CsvColumnMapping(
    string DateColumn,
    string AmountColumn,
    string? DescriptionColumn,
    string? ReferenceColumn);

public sealed record CsvParsingOptions(
    string EncodingName,
    string Delimiter,
    string DateFormat,
    char DecimalSeparator,
    CsvColumnMapping Columns);

public sealed record ParsedCsvRow(
    int RowNumber,
    string RawData,
    DateOnly? TransactionDate,
    decimal? SignedAmount,
    string? Description,
    string? ExternalReference,
    IReadOnlyList<string> Errors);

public sealed record ParsedCsvFile(
    string FileFingerprint,
    string EncodingName,
    char Delimiter,
    IReadOnlyList<string> Headers,
    IReadOnlyList<ParsedCsvRow> Rows);

public sealed record StageCsvImportCommand(
    Stream Content,
    string FileName,
    long FileSizeBytes,
    CurrencyCode Currency,
    CsvParsingOptions Options);

public sealed record ImportRowDto(
    Guid Id,
    int RowNumber,
    string RawData,
    DateOnly? TransactionDate,
    decimal? SignedAmount,
    CurrencyCode Currency,
    string? Description,
    string? ExternalReference,
    Guid? AccountId,
    Guid? CategoryId,
    TransactionType? TransactionType,
    ImportRowStatus Status,
    string? ErrorMessage,
    Guid? BudgetTransactionId,
    Guid? DuplicateTransactionId,
    ImportDuplicateReason? DuplicateReason);

public sealed record DuplicateMatch(Guid TransactionId, ImportDuplicateReason Reason);

public enum DuplicateDecision
{
    Skip,
    ImportAnyway
}

public sealed record ResolveImportDuplicateCommand(
    Guid BatchId,
    Guid RowId,
    DuplicateDecision Decision);

public sealed record UpdateImportCandidateCommand(
    Guid BatchId,
    Guid RowId,
    DateOnly TransactionDate,
    decimal SignedAmount,
    string? Description,
    string? ExternalReference,
    Guid AccountId,
    Guid CategoryId);

public sealed record ConfirmImportBatchCommand(Guid BatchId, IReadOnlyCollection<Guid> RowIds);

public sealed record ImportBatchDto(
    Guid Id,
    string FileName,
    string FileFingerprint,
    long FileSizeBytes,
    string EncodingName,
    string Delimiter,
    string DateColumn,
    string AmountColumn,
    string? DescriptionColumn,
    string? ReferenceColumn,
    string DateFormat,
    string DecimalSeparator,
    ImportBatchStatus Status,
    DateTimeOffset CreatedAtUtc,
    int TotalRowCount,
    int ValidRowCount,
    int InvalidRowCount,
    IReadOnlyList<ImportRowDto> Rows);

public interface ICsvImportParser
{
    Task<ParsedCsvFile> ParseAsync(
        Stream content,
        CsvParsingOptions options,
        CancellationToken cancellationToken);
}

public interface IImportBatchRepository
{
    Task AddAsync(ImportBatch batch, CancellationToken cancellationToken);
    Task<ImportBatch> AddOrGetExistingAsync(ImportBatch batch, CancellationToken cancellationToken);
    Task<ImportBatch?> FindOwnedByIdAsync(
        Guid batchId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task UpdateAsync(ImportBatch batch, CancellationToken cancellationToken);
    Task ConfirmAsync(
        ImportBatch batch,
        IReadOnlyCollection<BudgetTransaction> transactions,
        CancellationToken cancellationToken);
    Task<DuplicateMatch?> FindDuplicateAsync(
        Guid userId,
        string? externalReference,
        DateOnly transactionDate,
        TransactionType type,
        decimal amount,
        string? description,
        CancellationToken cancellationToken);
}

public sealed class CsvImportParsingException(string message) : Exception(message);

public sealed class ImportConcurrencyException : Exception
{
    public ImportConcurrencyException()
        : base("Import batch changed during this operation.")
    {
    }
}
