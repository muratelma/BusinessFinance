namespace BusinessFinance.Api.Features.Attachments;

public sealed record AttachmentResponse(
    Guid Id,
    Guid TransactionId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string Sha256,
    DateTimeOffset CreatedAtUtc);

public sealed record AttachmentListResponse(IReadOnlyList<AttachmentResponse> Items);
