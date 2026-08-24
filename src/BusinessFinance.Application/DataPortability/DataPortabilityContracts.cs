using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.DataPortability;

public sealed record PortableFile(string FileName, string ContentType, byte[] Content);

public sealed record BackupValidationDto(
    int SchemaVersion,
    DateTimeOffset CreatedAtUtc,
    int EntityCount,
    string PayloadSha256);

public sealed record RestoreSummaryDto(
    int SchemaVersion,
    int RestoredEntityCount,
    DateTimeOffset RestoredAtUtc);

public interface IDataPortabilityRepository
{
    Task<PortableFile> ExportTransactionsCsvAsync(Guid userId, CancellationToken cancellationToken);
    Task<PortableFile> ExportCounterpartyLedgerCsvAsync(Guid userId, CancellationToken cancellationToken);
    Task<PortableFile> ExportFinancialJsonAsync(Guid userId, CancellationToken cancellationToken);
    Task<PortableFile> CreateBackupAsync(Guid userId, CancellationToken cancellationToken);
    Task<BackupValidationDto> ValidateBackupAsync(byte[] content, CancellationToken cancellationToken);
    Task<RestoreSummaryDto> RestoreBackupAsync(
        Guid userId,
        byte[] content,
        DateTimeOffset restoredAtUtc,
        CancellationToken cancellationToken);
}

public sealed class DataPortabilityException(
    string code,
    string message,
    ApplicationErrorType errorType = ApplicationErrorType.Validation)
    : Exception(message)
{
    public string Code { get; } = code;
    public ApplicationErrorType ErrorType { get; } = errorType;
}

public static class DataPortabilityErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "auth.authentication_required",
        "Authentication is required.",
        ApplicationErrorType.Unauthorized);
}
