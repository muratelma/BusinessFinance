using BusinessFinance.Domain;

namespace BusinessFinance.Application.Attachments;

public sealed record UploadAttachmentCommand(
    Guid TransactionId,
    string FileName,
    string ContentType,
    Stream Content,
    long DeclaredLength);

public sealed record AttachmentInspection(
    bool IsAccepted,
    string? RejectionReason,
    string? NormalizedContentType,
    string? Extension,
    string? Sha256);

public sealed record AttachmentDto(
    Guid Id,
    Guid TransactionId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string Sha256,
    DateTimeOffset CreatedAtUtc);

public sealed record AttachmentDownload(
    AttachmentDto Metadata,
    Stream Content);

public interface IAttachmentFileInspector
{
    AttachmentInspection Inspect(string fileName, string contentType, ReadOnlySpan<byte> content);
}

public interface IAttachmentObjectStore
{
    Task WriteAsync(string objectKey, ReadOnlyMemory<byte> content, CancellationToken cancellationToken);
    Task<Stream?> OpenReadAsync(string objectKey, CancellationToken cancellationToken);
    Task DeleteIfExistsAsync(string objectKey, CancellationToken cancellationToken);
}

public interface IAttachmentRepository
{
    Task AddAsync(FinancialAttachment attachment, CancellationToken cancellationToken);
    Task<IReadOnlyList<FinancialAttachment>> ListAsync(
        Guid transactionId, Guid userId, CancellationToken cancellationToken);
    Task<FinancialAttachment?> FindOwnedByIdAsync(
        Guid attachmentId, Guid userId, CancellationToken cancellationToken);
}
