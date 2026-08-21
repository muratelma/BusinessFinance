namespace BusinessFinance.Api.Features.Imports;

public sealed record ImportRowResponse(
    Guid Id,
    int RowNumber,
    string RawData,
    string? TransactionDate,
    string? SignedAmount,
    string Currency,
    string? Description,
    string? ExternalReference,
    Guid? AccountId,
    Guid? CategoryId,
    string? TransactionType,
    string Status,
    string? ErrorMessage,
    Guid? BudgetTransactionId,
    Guid? DuplicateTransactionId,
    string? DuplicateReason);

public sealed record ResolveImportDuplicateRequest(string Decision);

public sealed record UpdateImportCandidateRequest(
    string TransactionDate,
    string SignedAmount,
    string? Description,
    string? ExternalReference,
    Guid AccountId,
    Guid CategoryId);

public sealed record ConfirmImportBatchRequest(IReadOnlyList<Guid> RowIds);

public sealed record ImportBatchResponse(
    Guid Id,
    string FileName,
    string FileFingerprint,
    long FileSizeBytes,
    string Encoding,
    string Delimiter,
    string DateColumn,
    string AmountColumn,
    string? DescriptionColumn,
    string? ReferenceColumn,
    string DateFormat,
    string DecimalSeparator,
    string Status,
    DateTimeOffset CreatedAtUtc,
    int TotalRowCount,
    int ValidRowCount,
    int InvalidRowCount,
    IReadOnlyList<ImportRowResponse> Rows);
