namespace BusinessFinance.Domain;

public sealed class FinancialAttachment
{
    public const int MaximumOriginalFileNameLength = 255;
    public const int MaximumContentTypeLength = 50;
    public const int MaximumObjectKeyLength = 180;
    public const int Sha256HexLength = 64;
    public const long MaximumSizeBytes = 5 * 1024 * 1024;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid TransactionId { get; }
    public string OriginalFileName { get; }
    public string ContentType { get; }
    public long SizeBytes { get; }
    public string Sha256 { get; }
    public string ObjectKey { get; }
    public DateTimeOffset CreatedAtUtc { get; }

    private FinancialAttachment()
    {
        OriginalFileName = null!;
        ContentType = null!;
        Sha256 = null!;
        ObjectKey = null!;
    }

    public FinancialAttachment(
        Guid id,
        Guid userId,
        Guid transactionId,
        string originalFileName,
        string contentType,
        long sizeBytes,
        string sha256,
        string objectKey,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty || userId == Guid.Empty || transactionId == Guid.Empty)
            throw new ArgumentException("Attachment identity and ownership are required.");
        var safeName = Path.GetFileName(originalFileName?.Trim());
        if (string.IsNullOrWhiteSpace(safeName) || safeName.Length > MaximumOriginalFileNameLength)
            throw new ArgumentException("Attachment file name is invalid.", nameof(originalFileName));
        if (string.IsNullOrWhiteSpace(contentType) || contentType.Length > MaximumContentTypeLength)
            throw new ArgumentException("Attachment content type is invalid.", nameof(contentType));
        if (sizeBytes is < 1 or > MaximumSizeBytes)
            throw new ArgumentOutOfRangeException(nameof(sizeBytes));
        if (sha256?.Length != Sha256HexLength || sha256.Any(character =>
                !char.IsAsciiHexDigit(character) || char.IsUpper(character)))
            throw new ArgumentException("Attachment SHA-256 is invalid.", nameof(sha256));
        if (string.IsNullOrWhiteSpace(objectKey) || objectKey.Length > MaximumObjectKeyLength ||
            objectKey.Contains("..", StringComparison.Ordinal) || Path.IsPathRooted(objectKey))
            throw new ArgumentException("Attachment object key is invalid.", nameof(objectKey));
        if (createdAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));

        Id = id;
        UserId = userId;
        TransactionId = transactionId;
        OriginalFileName = safeName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        Sha256 = sha256;
        ObjectKey = objectKey;
        CreatedAtUtc = createdAtUtc;
    }
}
